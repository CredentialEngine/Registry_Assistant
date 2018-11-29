using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Caching;
using System.Net.Http;

namespace RA.Services
{
	public class GremlinServices
	{
		public static List<JObject> GremlinResponseToJObjectSearchResults( string rawGremlinResponse, ref int totalResults )
		{
			var results = new List<JObject>();
			var rawData = JObject.Parse( rawGremlinResponse );

			//Try to get results, or return an empty list
			//The gremlin queries are configured to return a value consisting of a two-item array where:
			//The first item contains a count of all results, and
			//The second item contains a list of results, constrained by the query's skip/limit parameters
			try
			{
				totalResults = ( int ) rawData[ "result" ][ "data" ][ "@value" ].FirstOrDefault( m => ( string ) m[ "@type" ] == "g:Int64" )[ "@value" ];
				results = (( JArray ) rawData[ "result" ][ "data" ][ "@value" ].FirstOrDefault( m => ( string ) m[ "@type" ] == "g:List" )[ "@value" ]).ToList().ConvertAll( m => ( JObject ) m ).ToList();
			}
			catch
			{
				results = new List<JObject>();
				totalResults = 0;
			}

			return results;
		}
		//

		public static List<string> GetQueryMainTargetTypes( string rawQuery )
		{
			var data = JObject.Parse( rawQuery );
			return GetQueryMainTargetTypes( data );
		}
		public static List<string> GetQueryMainTargetTypes( JObject query )
		{
			var type = query.Properties().FirstOrDefault( m => m.Name == "@type" );
			return type == null ? new List<string>() : type.Type == JTokenType.Array ? type.ToList().ConvertAll( m => m.ToString() ).ToList() : new List<string>() { type.ToString() };
		}
		//

		public static Dictionary<string, object> JsonStringToDictionary( string rawData )
		{
			var root = JObject.Parse( rawData );
			var result = JObjectToDictionary( root );
			return result;
		}
		//

		public static List<Dictionary<string, object>> GremlinResponseToDictionarySearchResults( string rawGremlinResponse, ref int totalResults )
		{
			var data = GremlinResponseToJObjectSearchResults( rawGremlinResponse, ref totalResults );
			return data.ConvertAll( m => JObjectToDictionary( m ) ).ToList();
		}
		public static Dictionary<string, object> JObjectToDictionary( JObject token )
		{
			var result = new Dictionary<string, object>();
			foreach ( var prop in token.Properties() )
			{
				switch ( prop.Value.Type )
				{
					case JTokenType.Array:
					{
						var list = new List<object>();
						foreach ( var item in ( JArray ) prop.Value )
						{
							if ( item.Type == JTokenType.Object )
							{
								list.Add( JObjectToDictionary( ( JObject ) item ) );
							}
							else
							{
								list.Add( item.ToObject<object>() );
							}
						}
						result.Add( prop.Name, list );
						break;
					}
					case JTokenType.Object:
					{
						result.Add( prop.Name, JObjectToDictionary( ( JObject ) prop.Value ) );
						break;
					}
					default:
					{
						result.Add( prop.Name, prop.Value.ToObject<object>() );
						break;
					}
				}
			}
			return result;
		}
		//

		//May not need this method if we're going to pipe requests through the account system
		//Maybe keep it around just in case?
		//Note that there is currently a copy of the CE authorization token in the web.config to support this method
		public static string DoGremlinQuery( string query )
		{
			//var url = "https://sandbox.credentialengineregistry.org/gremlin";
			var gremlinURL = System.Configuration.ConfigurationManager.AppSettings[ "GremlinSearchEndpoint" ];
			var authorizationToken = System.Configuration.ConfigurationManager.AppSettings[ "CredentialRegistryAuthorizationToken" ];

			var queryJSON = JsonConvert.SerializeObject( new { gremlin = query } );

			var client = new HttpClient();
			client.DefaultRequestHeaders.Add( "Authorization", string.Format( "Bearer {0}", authorizationToken ) );

			try
			{
				var response = client.PostAsync( gremlinURL, new StringContent( queryJSON, Encoding.UTF8, "application/json" ) ).Result;
				var responseBody = response.Content.ReadAsStringAsync().Result;
				return responseBody;
			}
			catch ( Exception ex )
			{
				return ex.Message;
			}
		}
		//

		//Get raw context documents, to be processed later
		public static List<string> GetCTDLAndCTDLASNContexts()
		{
			var cache = MemoryCache.Default;
			var contextFiles = ( List<string> ) cache[ "CTDLContexts" ];
			if ( contextFiles == null )
			{
				//Download the context files
				contextFiles = new List<string>();
				contextFiles.Add( DownloadContextFile( "ctdl" ) );
				contextFiles.Add( DownloadContextFile( "ctdlasn" ) );

				//Cache the data
				cache.Remove( "CTDLContexts" );
				cache.Add( "CTDLContexts", contextFiles, new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddHours( 6 ) } );
			}

			return contextFiles;
		}
		public static string DownloadContextFile( string schemaName )
		{
			var url = System.Configuration.ConfigurationManager.AppSettings[ "SchemaContextJsonURLTemplate" ];
			return new HttpClient().GetAsync( string.Format( url, schemaName ) ).Result.Content.ReadAsStringAsync().Result;
		}
		//

		public static string CTDLQueryToGremlinQuery( string jsonText, int skip, int take, GremlinContext context = null )
		{
			var token = JObject.Parse( jsonText );
			return CTDLQueryToGremlinQuery( token, skip, take, context );
		}
		//

		public static string CTDLQueryToGremlinQuery( JObject token, int skip, int take, GremlinContext context = null )
		{
			//Initialize context
			context = context ?? new GremlinContext( true );

			//Start Query
			//g.V().has('@id').as('main')
			//.and(...
			var query = new GremlinItem( "g.V", "" ) { AllowEmptyArguments = true };
			var nextPart = query
				.SetNext( new GremlinItem( "has", new List<GremlinItem>() { new GremlinItem( "@id" ), new GremlinItem( DefaultGremlinHandler.WrapTextInRegex( "http", DefaultGremlinHandler.TextMatchType.StartsWith ) ) { PrintStringValueWithoutQuotes = true } } ) ) //Filter out any node that isn't a top-level node
				.SetNext( new GremlinItem( "as", "main" ) )
				.SetNext( new GremlinItem( "and", "" ) );

			//Apply Query
			//Hierarchical query structure, transforming CTDL JSON into Gremlin
			var properties = (( JObject ) token).Properties().ToList();
			if ( properties.Count() > 0 )
			{
				foreach ( var property in properties )
				{
					HandleGremlin( property.Name, property.Value, nextPart, context, false );
				}
			}

			//Finish Query
			//...)
			//.union(__.count(),__.select('main').skip(0).limit(10).values('__payload').fold())
			var union = nextPart
				.SetNext( new GremlinItem( "union", "" ) );
			union.Arguments.Add( new GremlinItem( "count", "" ) { AllowEmptyArguments = true } );
			var finish = new GremlinItem( "select", "main" );
			finish
				.SetNext( new GremlinItem( "skip", skip ) )
				.SetNext( new GremlinItem( "limit", take ) )
				.SetNext( new GremlinItem( "values", "__payload" ) )
				.SetNext( new GremlinItem( "fold", "" ) { AllowEmptyArguments = true } );
			union.Arguments.Add( finish );

			//Call .ToString() - this triggers the internal .ToString() methods recursively
			var gremlin = query.ToString();
			return gremlin;
		}
		//

		//Container for a part of a gremlin query
		public class GremlinItem
		{
			public GremlinItem()
			{
				Arguments = new List<GremlinItem>();
			}
			public GremlinItem( JToken selfDirectValue )
			{
				DirectValue = selfDirectValue;
				IsDirectValue = true;
				Arguments = new List<GremlinItem>();
			}
			public GremlinItem( string method, JToken underlyingDirectValue )
			{
				Method = method;
				Arguments = new List<GremlinItem>()
				{
					new GremlinItem( underlyingDirectValue )
				};
			}
			public GremlinItem( string method, GremlinItem argument )
			{
				Method = method;
				Arguments = new List<GremlinItem>() { argument };
			}
			public GremlinItem( string method, List<GremlinItem> arguments )
			{
				Method = method;
				Arguments = arguments;
			}
			public GremlinItem SetNext( GremlinItem next )
			{
				Next = next;
				return next;
			}
			public string Method { get; set; }
			public List<GremlinItem> Arguments { get; set; }
			public GremlinItem Next { get; set; }
			public JToken DirectValue { get; set; }
			public bool IsDirectValue { get; set; }
			public bool AllowEmptyArguments { get; set; }
			public bool SkipIfEmptyNext { get; set; }
			public bool PrintStringValueWithoutQuotes { get; set; }
			public override string ToString()
			{
				if ( IsDirectValue )
				{
					if ( DirectValue == null )
					{
						return "";
					}

					var value = DirectValue.ToString().Replace( "\\", "" );
					if ( DirectValue.Type == JTokenType.String )
					{
						return value.Length > 0 ? (PrintStringValueWithoutQuotes ? value : "'" + value + "'") : "";
					}
					else if ( DirectValue.Type == JTokenType.Boolean )
					{
						return value.ToLower();
					}
					else
					{
						return value;
					}
				}
				else
				{
					if ( string.IsNullOrWhiteSpace( Method ) || (Arguments.Count() == 0 && !AllowEmptyArguments) )
					{
						return "";
					}
					else
					{
						var argumentStrings = Arguments.ConvertAll( m => m.ToString() ).Where( m => !string.IsNullOrWhiteSpace( m ) ).ToList();
						var result = Method + "(" + string.Join( ",", argumentStrings ) + ")";
						if ( result == (Method + "()") && !AllowEmptyArguments )
						{
							return "";
						}
						else
						{
							var nextString = Next == null ? "" : Next.ToString();
							return result + (string.IsNullOrWhiteSpace( nextString ) ? "" : "." + nextString);
						}
					}
				}
			}
		}
		//

		//Recursive-friendly method to construct a gremlin query
		public static void HandleGremlin( string property, JToken token, GremlinItem outerContainer, GremlinContext context, bool isInArray )
		{
			var handler = context.Handlers.FirstOrDefault( m => m.AppliesToTerms.Contains( property ) ) ?? context.DefaultHandler;
			switch ( token.Type )
			{
				case JTokenType.Array:
				{
					//Skip empty arrays
					var data = ( JArray ) token;
					if ( data.Count() == 0 )
					{
						return;
					}
					handler.ArrayHandler( property, data, outerContainer, context );
					break;
				}
				case JTokenType.Object:
				{
					//Skip empty objects
					var data = ( JObject ) token;
					if ( data.Properties().Count() == 0 )
					{
						return;
					}
					handler.ObjectHandler( property, data, outerContainer, context, isInArray );
					break;
				}
				default:
				{
					//Skip empty values
					var data = ( JValue ) token;
					if ( data.Type == JTokenType.String && string.IsNullOrWhiteSpace( ( string ) data ) )
					{
						return;
					}
					handler = context.Handlers.FirstOrDefault( m => m.AppliesToTerms.Contains( data.ToString() ) ) ?? handler;
					handler.ValueHandler( property, data, outerContainer, context );
					break;
				}
			}
		}
		//

		//Context object that indicates which properties should be used with which handlers
		public class GremlinContext
		{
			public GremlinContext( bool useDefaultHandlers = true, List<string> rawContextDocuments = null )
			{
				Handlers = new List<IGremlinHandlerSet>();
				DefaultHandler = new DefaultGremlinHandler();
				if ( useDefaultHandlers )
				{
					AddDefaultHandlers();
				}
				if ( rawContextDocuments != null && rawContextDocuments.Count() > 0 )
				{
					ProcessRawContextDocuments( rawContextDocuments );
				}
			}
			public List<IGremlinHandlerSet> Handlers { get; set; }
			public IGremlinHandlerSet DefaultHandler { get; set; }

			public void AddDefaultHandlers()
			{
				Handlers.Add( new TermGroupHandler( "search:andTerms", "and" ) );
				Handlers.Add( new TermGroupHandler( "search:orTerms", "or" ) );
				//Handlers.Add( new TermGroupHandler( "search:notTerms", "not" ) ); //.not() doesn't seem to work like .and() and .or()
				Handlers.Add( new AnyValueHandler() );
			}

			private void ProcessRawContextDocuments( List<string> rawContextDocuments )
			{
				//Properties
				var languageMapProperties = new List<string>();
				var stringProperties = new List<string>();
				var numberProperties = new List<string>();

				//Document processing
				foreach ( var context in rawContextDocuments )
				{
					var document = JObject.Parse( context );
					foreach ( var prop in (( JObject ) document[ "@context" ]).Properties() )
					{
						//Object Properties
						if ( prop.Value.Type == JTokenType.Object )
						{
							foreach ( var valueProperty in (( JObject ) prop.Value).Properties() )
							{
								//Language Map Properties
								if ( valueProperty.Name == "@container" && valueProperty.Value.Type == JTokenType.String && ( string ) valueProperty.Value == "@language" )
								{
									languageMapProperties.Add( prop.Name );
								}

								//String Properties
								if ( valueProperty.Name == "@type" && valueProperty.Value.Type == JTokenType.String && ( string ) valueProperty.Value == "xsd:string" )
								{
									stringProperties.Add( prop.Name );
								}

								//Integer and Float Properties
								if ( valueProperty.Name == "@type" && valueProperty.Value.Type == JTokenType.String && (( string ) valueProperty.Value == "xsd:integer" || ( string ) valueProperty.Value == "xsd:float") )
								{
									numberProperties.Add( prop.Name );
								}
							}
						}
					}

					//TODO: date properties, anything else important that needs its own handler
				}

				//Add handlers
				if ( languageMapProperties.Count() > 0 )
				{
					Handlers.Add( new LanguageMapHandler( languageMapProperties.Distinct().ToList() ) );
				}

				if ( stringProperties.Count() > 0 )
				{
					Handlers.Add( new StringValueHandler( stringProperties.Distinct().ToList() ) );
				}

				if ( numberProperties.Count() > 0 )
				{
					Handlers.Add( new NumberHandler( numberProperties.Distinct().ToList() ) );
				}
			}
		}
		//

		//Default handler for terms
		public class DefaultGremlinHandler : IGremlinHandlerSet
		{
			public DefaultGremlinHandler()
			{
				AppliesToTerms = new List<string>();
			}
			public List<string> AppliesToTerms { get; set; }
			public virtual void ArrayHandler( string property, JArray token, GremlinItem container, GremlinContext context )
			{
				//If it is an array of objects, use out('property').or(...)
				var innerContainer = new GremlinItem();
				if ( token.First().Type == JTokenType.Object )
				{
					innerContainer = new GremlinItem( "out", property ) { SkipIfEmptyNext = true };
					innerContainer.Next = new GremlinItem( "or", "" );
					foreach ( var item in token )
					{
						HandleGremlin( property, item, innerContainer.Next, context, true );
					}
					//Don't allow ending in .out(...)
					//TODO: test this to see if the .SkipIfEmptyNext setting makes this redundant
					if ( string.IsNullOrWhiteSpace( innerContainer.Next.ToString() ) )
					{
						return;
					}
				}
				//Otherwise, just use or(...)
				else
				{
					innerContainer.Method = "or";
					foreach ( var item in token )
					{
						HandleGremlin( property, item, innerContainer, context, true );
					}
					//May need an empty check here?
				}
				//Add the data
				container.Arguments.Add( innerContainer );
			}
			//

			public virtual void ObjectHandler( string property, JObject token, GremlinItem container, GremlinContext context, bool isInArray )
			{
				var innerContainer = new GremlinItem();
				var tokenProperties = token.Properties().ToList();

				//Check for custom value handling
				//Should this really happen at the HandleGremlin() level?
				var searchValue = tokenProperties.FirstOrDefault( m => m.Name == "search:value" );
				var searchOperator = GetStringValue( tokenProperties, "search:operator" );
				if ( searchValue != null )
				{
					innerContainer = new GremlinItem( searchOperator == "search:andTerms" ? "and" : "or", "" ); //Default: or
					HandleGremlin( property, searchValue.Value, innerContainer, context, isInArray );  //Should isInArray be true? false? dynamic?
																									   //Don't allow empty values
					if ( string.IsNullOrWhiteSpace( innerContainer.ToString() ) )
					{
						return;
					}
				}
				//Normal value handling
				else
				{
					if ( isInArray )
					{
						innerContainer = new GremlinItem( searchOperator == "search:orTerms" ? "or" : "and", "" ); //Default: and
						foreach ( var itemProperty in tokenProperties )
						{
							HandleGremlin( itemProperty.Name, itemProperty.Value, innerContainer, context, false );
						}
						//Don't allow ending in .out(...)
						//TODO: test this to see if the .SkipIfEmptyNext setting makes this redundant
						if ( string.IsNullOrWhiteSpace( innerContainer.ToString() ) )
						{
							return;
						}
					}
					else
					{
						innerContainer = new GremlinItem( "out", new GremlinItem( property ) ) { SkipIfEmptyNext = true };
						innerContainer.Next = new GremlinItem( searchOperator == "search:orTerms" ? "or" : "and", "" ); //Default: and
						foreach ( var itemProperty in tokenProperties )
						{
							HandleGremlin( itemProperty.Name, itemProperty.Value, innerContainer.Next, context, false );
						}
						//Don't allow ending in .out(...)
						//TODO: test this to see if the .SkipIfEmptyNext setting makes this redundant
						if ( string.IsNullOrWhiteSpace( innerContainer.Next.ToString() ) )
						{
							return;
						}
					}
				}
				container.Arguments.Add( innerContainer );
			}
			//

			public virtual void ValueHandler( string property, JValue token, GremlinItem container, GremlinContext context )
			{
				//Handle the value
				container.Arguments.Add( new GremlinItem( "has", new List<GremlinItem>() { new GremlinItem( property ), new GremlinItem( token ) } ) );
			}
			//

			public enum TextMatchType { StartsWith, EndsWith, Contains, ExactMatch }
			public static string WrapTextInRegex( string text, TextMatchType matchType = TextMatchType.Contains, bool isCaseSensitive = false, bool wrapExactMatchInQuotes = true )
			{
				var insertText = text.ToString().Replace( "/", "\\/" ).Replace( "\\", "" );
				var exactMatchNoRegex = text.ToString().Replace( "\\", "" );
				var caseSensitivityText = isCaseSensitive ? "" : "(?i)";
				switch ( matchType )
				{
					case TextMatchType.StartsWith:
					return "test({x,y -> x ==~ y}, /" + caseSensitivityText + insertText + ".*/)";
					case TextMatchType.EndsWith:
					return "test({x,y -> x ==~ y}, /" + caseSensitivityText + ".*" + insertText + "/)";
					case TextMatchType.Contains:
					return "test({x,y -> x ==~ y}, /" + caseSensitivityText + ".*" + insertText + ".*/)";
					case TextMatchType.ExactMatch:
					return isCaseSensitive ? "test({x,y -> x ==~ y}, /" + caseSensitivityText + insertText + "/)" : wrapExactMatchInQuotes ? ("'" + exactMatchNoRegex + "'") : exactMatchNoRegex;
					default:
					return wrapExactMatchInQuotes ? ("'" + exactMatchNoRegex + "'") : exactMatchNoRegex;
				}
			}
			//

			public static string GetStringValue( List<JProperty> properties, string propertyName )
			{
				try
				{
					return ( string ) properties.FirstOrDefault( m => m.Name == propertyName ).Value;
				}
				catch
				{
					return "";
				}
			}
			//

			public static List<string> GetStringListValue( List<JProperty> properties, string propertyName )
			{
				try
				{
					return (( JArray ) properties.FirstOrDefault( m => m.Name == propertyName ).Value).ToList().ConvertAll( m => ( string ) m ).ToList();
				}
				catch
				{
					return new List<string>();
				}
			}
			//

			public static bool? GetBooleanValue( List<JProperty> properties, string propertyName )
			{
				try
				{
					return ( bool ) properties.FirstOrDefault( m => m.Name == propertyName ).Value;
				}
				catch
				{
					return null;
				}
			}
			//

			public static float? GetFloatValue( List<JProperty> properties, string propertyName )
			{
				try
				{
					return ( float ) properties.FirstOrDefault( m => m.Name == propertyName ).Value;
				}
				catch
				{
					return null;
				}
			}
			//

			public static TextMatchType GetTextMatchType( List<JProperty> properties, string propertyName = "search:matchType" )
			{
				try
				{
					var match = ( string ) properties.FirstOrDefault( m => m.Name == propertyName ).Value;
					switch ( match.ToLower() )
					{
						case "search:startswith":
						return TextMatchType.StartsWith;
						case "search:endswith":
						return TextMatchType.EndsWith;
						case "search:contains":
						return TextMatchType.Contains;
						case "search:exactmatch":
						return TextMatchType.ExactMatch;
						default:
						return TextMatchType.Contains;
					}
				}
				catch
				{
					return TextMatchType.Contains;
				}
			}
			//

			public static bool GetTextCaseSensitive( List<JProperty> properties, string propertyName = "search:caseSensitive" )
			{
				try
				{
					return ( bool ) properties.FirstOrDefault( m => m.Name == "search:isCaseSensitive" ).Value == true;
				}
				catch
				{
					return false;
				}
			}
			//

			public static bool GetAnyValue( JToken value )
			{
				try
				{
					return value.ToString().ToLower() == "search:anyvalue";
				}
				catch
				{
					return false;
				}
			}
			//

			public static void AddArgumentIfNotNull( GremlinItem holder, string method, string property, string function, JToken value )
			{
				if ( value != null )
				{
					//e.g. has('ceterms:price', gte(500))
					holder.Arguments.Add( new GremlinItem( method, new List<GremlinItem>() { new GremlinItem( property ), new GremlinItem( function, value ) } ) );
				}
			}
			//
		}
		//

		//Handle and(), or(), and not() term groups
		//NOTE - should phase this out in favor of structured queries
		//e.g. "ceterms:property": { "search:value": [ "value 1", "value 2"], "search:operator": "search:andTerms" }
		//May not be necessary
		public class TermGroupHandler : DefaultGremlinHandler
		{
			public TermGroupHandler( string searchTerm, string gremlinMethod )
			{
				SearchTerm = searchTerm;
				GremlinMethod = gremlinMethod;
				AppliesToTerms = new List<string>() { SearchTerm };
			}

			public string SearchTerm { get; set; }
			public string GremlinMethod { get; set; }

			public override void ArrayHandler( string property, JArray token, GremlinItem container, GremlinContext context )
			{
				//Handling here is the same for arrays of objects and arrays of values
				var innerContainer = new GremlinItem( GremlinMethod, "" );
				foreach ( var item in token )
				{
					//We account for the custom method in the innerContainer, so force using an object at the next layer
					HandleGremlin( "search:andTerms", item, innerContainer, context, true );
				}
				container.Arguments.Add( innerContainer );
			}

			public override void ObjectHandler( string property, JObject token, GremlinItem container, GremlinContext context, bool isInArray )
			{
				//Handle an object similarly to handling an array
				var innerContainer = new GremlinItem( GremlinMethod, "" );
				foreach ( var item in token.Properties() )
				{
					HandleGremlin( item.Name, item.Value, innerContainer, context, false );
				}
				container.Arguments.Add( innerContainer );
			}
		}
		//

		//Enables checking to see if a property exists in a document, regardless of its value
		public class AnyValueHandler : DefaultGremlinHandler
		{
			public AnyValueHandler()
			{
				AppliesToTerms = new List<string>() { "search:anyValue" };
			}
			public override void ValueHandler( string property, JValue token, GremlinItem container, GremlinContext context )
			{
				container.Arguments.Add( AnyValueForProperty( property ) );
			}
			public static GremlinItem AnyValueForProperty( string property )
			{
				return new GremlinItem( "or", new List<GremlinItem>() { new GremlinItem( "has", property ), new GremlinItem( "out", property ) } );
			}
		}
		//

		//Handle non-language-dependent strings (e.g., URIs, CTIDs, etc.)
		public class StringValueHandler : DefaultGremlinHandler
		{
			public StringValueHandler()
			{
				//Non-language-map string terms
				AppliesToTerms = new List<string>();
			}
			public StringValueHandler( List<string> stringValueProperties )
			{
				AppliesToTerms = stringValueProperties;
			}
			public override void ObjectHandler( string property, JObject token, GremlinItem container, GremlinContext context, bool isInArray )
			{
				//Get the data
				var properties = token.Properties().ToList();
				var searchValue = GetStringValue( properties, "search:value" );
				var matchType = GetTextMatchType( properties );
				var isCaseSensitive = GetTextCaseSensitive( properties );

				//Add the query part
				var wrapper = new GremlinItem( "has", property );
				var targetValue = WrapTextInRegex( searchValue, matchType, isCaseSensitive );
				wrapper.Arguments.Add( new GremlinItem( targetValue ) { PrintStringValueWithoutQuotes = true } );

				//Add the data
				container.Arguments.Add( wrapper );
			}
			public override void ValueHandler( string property, JValue token, GremlinItem container, GremlinContext context )
			{
				//Regex string fields for case-insensitive matches
				var searchValue = ( string ) token;
				var wrapper = new GremlinItem( "has", property );
				var targetValue = WrapTextInRegex( searchValue );
				wrapper.Arguments.Add( new GremlinItem( targetValue ) { PrintStringValueWithoutQuotes = true } );

				//Add the data
				container.Arguments.Add( wrapper );
			}
		}
		//

		//Handle Language Maps
		public class LanguageMapHandler : DefaultGremlinHandler
		{
			public LanguageMapHandler()
			{
				AppliesToTerms = new List<string>();
			}
			public LanguageMapHandler( List<string> languageMapProperties )
			{
				AppliesToTerms = languageMapProperties;
			}

			//Handle Array query
			//e.g., "ceterms:name": [ "name 1", "name 2", "le name 1", "le name 2" ]
			//e.g., "ceterms:name": [ { "en": [ "name 1", "name 2" ] },	{ "fr": [ "le name 1", "le name 2" ] } ]
			public override void ArrayHandler( string property, JArray token, GremlinItem container, GremlinContext context )
			{
				//Handle arrays regardless of their contents
				var objectValues = token.Where( m => m.Type == JTokenType.Object ).ToList();
				var normalValues = token.Where( m => m.Type != JTokenType.Object ).ToList();

				//Default object handler works for object values
				foreach ( var item in objectValues )
				{
					base.ObjectHandler( property, ( JObject ) item, container, context, true );
				}

				//Custom handling for text values to accommodate the jump from the property name to the actual value with any unknown language code in between
				//e.g. "ceterms:keyword": [ "keyword 1", "keyword 2", "keyword 3" ]
				//e.g. out('ceterms:keyword').properties().or(hasValue(test(...)),hasValue(test(...)))
				var wrapper = new GremlinItem( "out", property );
				wrapper.Next = new GremlinItem( "properties().or", "" );
				foreach ( var item in normalValues )
				{
					wrapper.Next.Arguments.Add( new GremlinItem( "hasValue", new GremlinItem( WrapTextInRegex( item.ToString() ) ) { PrintStringValueWithoutQuotes = true } ) );
				}
				if ( normalValues.Count() > 0 && !string.IsNullOrWhiteSpace( wrapper.Next.ToString() ) )
				{
					container.Arguments.Add( wrapper );
				}

			}

			//Handle Object query
			//e.g., "ceterms:name": { "en": "name 1", "fr": "le name 1" }
			//e.g., "ceterms:name": { "en": "name 1", "fr": "le name 1", "search:operator": "search:orTerms" }
			//e.g., "ceterms:name": { "en": [ "name 1", "name 2" ], "fr": [ "le name 1", "le name 2" ] }
			//e.g., "ceterms:name": { "search:value": "name 1", "search:languageCode": [ "en", "fr" ] }
			//e.g., "ceterms:name": { "search:value": [ "name 1", "name 2", "le name 1", "le name 2" ],	"search:languageCode": [ "en", "fr" ] }
			public override void ObjectHandler( string property, JObject token, GremlinItem container, GremlinContext context, bool isInArray )
			{
				//Get values
				var properties = token.Properties().ToList();
				var searchValue = GetStringValue( properties, "search:value" );
				var searchValues = GetStringListValue( properties, "search:value" );
				var searchOperator = GetStringValue( properties, "search:operator" );
				var searchLanguages = GetStringListValue( properties, "search:languageCode" );
				var searchLanguage = GetStringValue( properties, "search:languageCode" );
				var searchMatchType = GetTextMatchType( properties );
				var searchCaseSensitive = GetTextCaseSensitive( properties );
				var langProperties = properties.Where( m => !m.Name.Contains( "search:" ) ).ToList();

				//Normalize search languages
				//Note: only applies in cases where searchValue or searchValues are in use
				if ( !string.IsNullOrWhiteSpace( searchLanguage ) )
				{
					searchLanguages.Add( searchLanguage );
				}
				if ( searchLanguages.Count() == 0 )
				{
					searchLanguages.Add( "search:anyValue" );
				}

				//isInArray should never be true - maybe log it if it is?
				//Otherwise, handling would need to be setup here
				var wrapper = new GremlinItem( "out", property );

				//Handle based on which keys are present
				//Using singular search:value
				//e.g., "ceterms:name": { "search:value": "name 1", "search:languageCode": [ "en", "fr" ] }
				if ( !string.IsNullOrWhiteSpace( searchValue ) )
				{
					wrapper.Next = new GremlinItem( searchOperator == "search:andTerms" ? "and" : "or", "" );
					foreach ( var lang in searchLanguages )
					{
						var has = HasLanguageValues( lang, searchValue, searchMatchType, searchCaseSensitive );
						wrapper.Next.Arguments.Add( has );
					}
				}
				//Using array search:value
				//e.g., "ceterms:name": { "search:value": [ "name 1", "name 2", "le name 1", "le name 2" ],	"search:languageCode": [ "en", "fr" ] }
				else if ( searchValues != null && searchValues.Count() > 0 )
				{
					wrapper.Next = new GremlinItem( searchOperator == "search:andTerms" ? "and" : "or", "" );
					foreach ( var lang in searchLanguages )
					{
						foreach ( var val in searchValues )
						{
							var has = HasLanguageValues( lang, val, searchMatchType, searchCaseSensitive );
							wrapper.Next.Arguments.Add( has );
						}
					}
				}
				//Using language code values
				//e.g., "ceterms:name": { "en": "name 1", "fr": "le name 1" }
				//e.g., "ceterms:name": { "en": "name 1", "fr": "le name 1", "search:operator": "search:orTerms" }
				//e.g., "ceterms:name": { "en": [ "name 1", "name 2" ], "fr": [ "le name 1", "le name 2" ] }
				else
				{
					//Update the wrapper
					//e.g., out("ceterms:name").or/and(has('en', test(...)),has('fr', test(...)))
					wrapper.Next = new GremlinItem( searchOperator == "search:orTerms" ? "or" : "and", "" );
					foreach ( var prop in langProperties )
					{
						//Array values
						//e.g. "en": [ "name 1", "name 2" ]
						if ( prop.Value.Type == JTokenType.Array )
						{
							foreach ( var val in prop.Value )
							{
								var has = HasLanguageValues( prop.Name, val.ToString(), searchMatchType, searchCaseSensitive );
								wrapper.Next.Arguments.Add( has );
							}
						}
						//Singular values
						//e.g. "en": "name 1"
						else
						{
							var has = HasLanguageValues( prop.Name, prop.Value.ToString(), searchMatchType, searchCaseSensitive );
							wrapper.Next.Arguments.Add( has );
						}
					}
				}

				//Add the value if it isn't empty
				if ( !string.IsNullOrWhiteSpace( wrapper.ToString() ) )
				{
					container.Arguments.Add( wrapper );
				}
			}
			private GremlinItem HasLanguageValues( string propertyName, string textValue, TextMatchType searchMatchType, bool searchCaseSensitive )
			{
				//e.g. "ceterms:name": { "en": "search:anyValue" }
				if ( GetAnyValue( textValue ) )
				{
					return AnyValueHandler.AnyValueForProperty( propertyName );
				}
				//e.g. "ceterms:name": { "search:value": "some text", "search:languageCode": "search:anyValue" }
				//Note - the code above injects search:anyValue if search:languageCode is empty/null
				else if ( GetAnyValue( propertyName ) )
				{
					return new GremlinItem( "properties().hasValue", new GremlinItem( WrapTextInRegex( textValue, searchMatchType, searchCaseSensitive ) ) { PrintStringValueWithoutQuotes = true } );
				}
				else
				{
					return new GremlinItem( "has", new List<GremlinItem>() { new GremlinItem( propertyName ), new GremlinItem( WrapTextInRegex( textValue, searchMatchType, searchCaseSensitive ) ) { PrintStringValueWithoutQuotes = true } } );
				}
			}
			//Handle Value query
			//e.g., "ceterms:name": "name 1"
			public override void ValueHandler( string property, JValue token, GremlinItem container, GremlinContext context )
			{
				//Get the data
				var wrappedValue = WrapTextInRegex( token.ToString() );

				//Add the query part
				//out('ceterms:name')
				var wrapper = new GremlinItem( "out", property );

				//Add the properties
				//out('ceterms:name').properties().hasValue('test...')
				wrapper.Next = new GremlinItem( "properties().hasValue", new GremlinItem( wrappedValue ) { PrintStringValueWithoutQuotes = true } );

				//Add the data, but don't allow .out(...) with no followup
				if ( !string.IsNullOrWhiteSpace( wrapper.Next.ToString() ) )
				{
					container.Arguments.Add( wrapper );
				}
			}
		}
		//

		//Handle integers and floats
		public class NumberHandler : DefaultGremlinHandler
		{
			public NumberHandler()
			{
				AppliesToTerms = new List<string>();
			}
			public NumberHandler( List<string> numberValueProperties )
			{
				AppliesToTerms = numberValueProperties;
			}
			//Custom handling for objects in order to accommodate range queries
			public override void ObjectHandler( string property, JObject token, GremlinItem container, GremlinContext context, bool isInArray )
			{
				//Get the data
				var properties = token.Properties().ToList();
				var greaterThan = GetFloatValue( properties, "search:greaterThan" );
				var greaterThanOrEqualTo = GetFloatValue( properties, "search:greaterThanOrEqualTo" );
				var lessThan = GetFloatValue( properties, "search:lessThan" );
				var lessThanOrEqualTo = GetFloatValue( properties, "search:lessThanOrEqualTo" );
				var searchOperator = GetStringValue( properties, "search:operator" );

				//Add the query part
				//e.g. and(has('ceterms:price', gt(50)),has('ceterms:price', lte(5000)))
				var wrapper = new GremlinItem( searchOperator == "search:orTerms" ? "or" : "and", "" );
				AddArgumentIfNotNull( wrapper, "has", property, "gt", greaterThan );
				AddArgumentIfNotNull( wrapper, "has", property, "gte", greaterThanOrEqualTo );
				AddArgumentIfNotNull( wrapper, "has", property, "lt", lessThan );
				AddArgumentIfNotNull( wrapper, "has", property, "lte", lessThanOrEqualTo );

				//Add the data
				if ( wrapper.Arguments.Count() > 0 && !string.IsNullOrWhiteSpace( wrapper.ToString() ) )
				{
					container.Arguments.Add( wrapper );
				}
			}
		}
		//

		//Interface for handlers
		public interface IGremlinHandlerSet
		{
			List<string> AppliesToTerms { get; set; }
			void ArrayHandler( string property, JArray token, GremlinItem container, GremlinContext context );
			void ObjectHandler( string property, JObject token, GremlinItem container, GremlinContext context, bool isInArray );
			void ValueHandler( string property, JValue token, GremlinItem container, GremlinContext context );
		}
		//
	}
}
