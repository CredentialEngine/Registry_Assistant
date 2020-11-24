using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

using System.Runtime.Caching;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace RA.Services
{
	public class SPARQLServices
	{
		#region Initialization
		public SPARQLServices()
		{
			Log( "Service 0" );
			//Use cache if available
			SchemaContext = ( SchemaContextInfo ) MemoryCache.Default[ "SPARQL_SchemaContext" ];
			if( SchemaContext == null )
			{
				Log( "Service 1" );
				//Get and process contexts
				SchemaContext = new SchemaContextInfo();
				Log( "Service 1A" );
				var contextData = GetAsyncData( new List<string>()
				{
					"https://credreg.net/ctdl/schema/context/json",
					"https://credreg.net/ctdlasn/schema/context/json",
					"https://credreg.net/qdata/schema/context/json",
					"https://credreg.net/navy/schema/context/json"
				} );
				foreach( var item in contextData )
				{
					Log( "Service " + item + " start" );
					ProcessContext( item.Key, item.Value );
					Log( "Service " + item + " finish" );
				}

				Log( "Service 2" );
				//Credential Types
				var ctdl = JObject.Parse( new HttpClient().GetAsync( "https://credreg.net/ctdl/schema/encoding/json" ).Result.Content.ReadAsStringAsync().Result );
				GetSubclassTree( (JArray) ctdl["@graph"], (JObject) ctdl[ "@graph" ].FirstOrDefault( m => m[ "@id" ] != null && m[ "@id" ].ToString() == "ceterms:Credential" ), SchemaContext.CredentialTypes );

				//Cache the data
				MemoryCache.Default.Remove( "SPARQL_SchemaContext" );
				MemoryCache.Default.Add( "SPARQL_SchemaContext", SchemaContext, new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddHours( 1 ) } );
				Log( "Service 3" );
			}
		}
		private void ProcessContext( string url, string rawContextData )
		{
			Log( "URL: " + url );
			Log( "Raw Context: " + rawContextData );
			//Raw Context
			var context = JObject.Parse( rawContextData );
			SchemaContext.Contexts.Add( url, context );
			var contextProperties = ( ( JObject ) context[ "@context" ] ).Properties();

			//Prefixes
			var prefixProperties = contextProperties.Where( m => m.Value.Type == JTokenType.String ).ToList();
			foreach( var item in prefixProperties )
			{
				if ( !SchemaContext.URIPrefixes.ContainsKey( item.Name ) )
				{
					SchemaContext.URIPrefixes.Add( item.Name, item.Value.ToString() );
				}
			}

			//Properties
			var objectProperties = contextProperties.Where( m => m.Value.Type == JTokenType.Object ).ToList();
			ProcessContext( objectProperties, "@container", "@language", SchemaContext.LanguageMapProperties );
			ProcessContext( objectProperties, "@type", "@id", SchemaContext.URIProperties );
			ProcessContext( objectProperties, "@type", "xsd:string", SchemaContext.StringProperties );
			ProcessContext( objectProperties, "@type", "xsd:language", SchemaContext.LanguageBCP47Properties );
			ProcessContext( objectProperties, "@type", "xsd:date", SchemaContext.DateProperties );
			ProcessContext( objectProperties, "@type", "xsd:dateTime", SchemaContext.DateTimeProperties );
			ProcessContext( objectProperties, "@type", "xsd:boolean", SchemaContext.BooleanProperties );
			ProcessContext( objectProperties, "@type", "xsd:integer", SchemaContext.IntegerProperties );
			ProcessContext( objectProperties, "@type", "xsd:float", SchemaContext.FloatProperties );
			ProcessContext( objectProperties, "@type", "xsd:decimal", SchemaContext.DecimalProperties );
		}
		private void ProcessContext( List<JProperty> objectProperties, string checkInternalProperty, string checkInternalValue, List<string> addToList )
		{
			var name = objectProperties.FirstOrDefault().Name;
			var val = objectProperties.FirstOrDefault().Value;
			var matches = objectProperties.Where( m => ( ( string ) m.Value[ checkInternalProperty ] ?? "" ) == checkInternalValue ).Select( m => m.Name ).ToList();
			addToList.AddRange( matches.Where( m => !addToList.Contains( m ) ).ToList() );
		}
		private void GetSubclassTree( JArray graph, JObject currentSubclass, List<string> addTo )
		{
			var id = currentSubclass[ "@id" ].ToString();
			addTo.Add( id );
			foreach ( JObject subclass in graph.Where( m => m["rdfs:subClassOf"] != null && ((JArray) m[ "rdfs:subClassOf" ] ).Values().Contains( id ) ).ToList() )
			{
				GetSubclassTree( graph, subclass, addTo );
			}
		}

		[Serializable]
		public class SchemaContextInfo
		{
			public SchemaContextInfo()
			{
				Contexts = new JObject();
				this.GetType().GetProperties().Where( m => m.PropertyType == typeof( List<string> ) ).ToList().ForEach( m => m.SetValue( this, new List<string>() ) );
				URIPrefixes = new Dictionary<string, string>()
				{
					{ "credreg", "https://credreg.net/" }, //Accommodate the credreg:__prefix references
					{ "search", "https://credreg.net/search/" }, //Accommodate the search: references
					{ "neptune-fts", "http://aws.amazon.com/neptune/vocab/v01/services/fts#" }
				};
			}
			public JObject Contexts { get; set; }
			public Dictionary<string, string> URIPrefixes { get; set; }
			public List<string> LanguageMapProperties { get; set; }
			public List<string> URIProperties { get; set; }
			public List<string> StringProperties { get; set; }
			public List<string> DateProperties { get; set; }
			public List<string> DateTimeProperties { get; set; }
			public List<string> BooleanProperties { get; set; }
			public List<string> IntegerProperties { get; set; } //xsd:integer
			public List<string> FloatProperties { get; set; } //xsd:float
			public List<string> DecimalProperties { get; set; } //xsd:decimal

			public List<string> CredentialTypes { get; set; }
			public List<string> LanguageBCP47Properties { get; set; } //xsd:language
		}
		//

		public class QueryContextInfo
		{
			public QueryContextInfo() { }
			public QueryContextInfo( JObject ctdlJSONQuery, int skip, int take, DescriptionSetType descriptionSetType, SortOrders sortOrder, bool orderByDescending, string apiKey, string referrer, int relatedURIsLimit, int relatedItemsLimit, bool includeDebugInfo )
			{
				CTDLJSONQuery = ctdlJSONQuery;
				Skip = skip;
				Take = take;
				DescriptionSetType = descriptionSetType;
				SortOrder = sortOrder;
				OrderByDescending = orderByDescending;
				APIKey = apiKey;
				Referrer = referrer;
				RelatedItemsLimit = relatedItemsLimit;
				RelatedURIsLimit = relatedURIsLimit;
				IncludeDebugInfo = includeDebugInfo;
			}
			public JObject CTDLJSONQuery { get; set; }
			public int Skip { get; set; }
			public int Take { get; set; }
			public DescriptionSetType DescriptionSetType { get; set; }
			public SortOrders SortOrder { get; set; }
			public bool OrderByDescending { get; set; }
			public string APIKey { get; set; }
			public string Referrer { get; set; }
			public int RelatedItemsLimit { get; set; }
			public int RelatedURIsLimit { get; set; }
			public bool IncludeDebugInfo { get; set; }
		}
		//

		#endregion

		//Global variables
		public SchemaContextInfo SchemaContext { get; set; }
		public QueryContextInfo QueryContext { get; set; }
		public enum SortOrders { DEFAULT, CREATED, UPDATED }
		public enum DescriptionSetType { Resource, Resource_Graph, Resource_RelatedURIs, Resource_RelatedURIs_Graph, Resource_RelatedURIs_RelatedData, Resource_RelatedURIs_Graph_RelatedData }
		//

		private static void Log( string text )
		{
			try
			{
				//System.IO.File.AppendAllText( "C:/@logs/assistantsparql.txt", text + "\r\n" );
			}
			catch { }
		}
		//

		//Processing
		public static SPARQLResultSet DoSPARQLSearch( SPARQLRequest request, string apiKey, string referrer )
		{
			return DoSPARQLSearch( request.Query, request.Skip, request.Take, request.DescriptionSetType, request.OrderBy, request.OrderDescending, apiKey, referrer, request.DescriptionSetRelatedURIsLimit, request.DescriptionSetRelatedItemsLimit, request.IncludeDebugInfo );
		}
		public static SPARQLResultSet DoSPARQLSearch( JObject ctdlQuery, int skip, int take, DescriptionSetType descriptionSetType, SortOrders sortOrder, bool orderDescending, string apiKey, string referrer, int relatedURIsLimit, int relatedItemsLimit, bool includeDebugInfo )
		{
			Log( "SPARQL 0" );
			var resultSet = new SPARQLResultSet();
			var SPARQLServices = new SPARQLServices();

			Log( "SPARQL 1" );
			//Add query context
			SPARQLServices.QueryContext = new QueryContextInfo( ctdlQuery, skip, take, descriptionSetType, sortOrder, orderDescending, apiKey, referrer, relatedURIsLimit, relatedItemsLimit, includeDebugInfo );

			Log( "SPARQL 2" );
			//Get the total results and page of results
			var resultsAndTotalQueryData = SPARQLServices.CTDLQueryToSPARQLQuery( ctdlQuery, skip, take, sortOrder, orderDescending );
			Log( "SPARQL 3" );

			/*
			//Run both subqueries simultaneously to speed things up
			ThreadPool.QueueUserWorkItem( delegate
			{
				try { resultsAndTotalQueryData.TotalResultsData = SPARQLServices.RunSPARQLQuery( resultsAndTotalQueryData.TotalResultsQuery, apiKey, skip, take, referrer ); } catch { }
				resultsAndTotalQueryData.TotalResultsReady = true;
			} );
			ThreadPool.QueueUserWorkItem( delegate
			{
				try { resultsAndTotalQueryData.ResultPayloadData = SPARQLServices.RunSPARQLQuery( resultsAndTotalQueryData.ResultPayloadQuery, apiKey, skip, take, referrer, true ); } catch { }
				resultsAndTotalQueryData.ResultPayloadReady = true;
			} );
			while( !resultsAndTotalQueryData.TotalResultsReady || !resultsAndTotalQueryData.ResultPayloadReady )
			{
				Thread.Sleep( 10 );
			}
			*/

			resultSet.DebugInfo[ "TotalResultsQuery" ] = resultsAndTotalQueryData.TotalResultsQuery;
			resultSet.DebugInfo[ "ResultPayloadQuery" ] = resultsAndTotalQueryData.ResultPayloadQuery;
			//resultSet.DebugInfo[ "TotalResultsData" ] = resultsAndTotalQueryData.TotalResultsData;
			//resultSet.DebugInfo[ "ResultPayloadData" ] = resultsAndTotalQueryData.ResultPayloadData;

			//Run the fancy new merged query
			try
			{
				resultsAndTotalQueryData.CombinedQueryData = SPARQLServices.RunSPARQLQuery( resultsAndTotalQueryData.CombinedQuery, apiKey, skip, take, referrer, false ); //Switch logRequest to true to enable logging
				resultSet.DebugInfo[ "CombinedResultsQuery" ] = resultsAndTotalQueryData.CombinedQuery;
				resultSet.DebugInfo[ "CombinedResultsData" ] = resultsAndTotalQueryData.CombinedQueryData;
			}
			catch( Exception ex )
			{
				resultSet.DebugInfo[ "CombinedResultsQuery" ] = resultsAndTotalQueryData.CombinedQuery;
				resultSet.DebugInfo[ "CombinedQueryError" ] = ex.Message;
			}

			/*
			var resultsAndTotal = SPARQLServices.RunSPARQLQuery( resultsAndTotalQuery, apiKey, skip, take, referrer );
			
			resultSet.DebugInfo[ "SPARQLQuery" ] = resultsAndTotalQuery;
			resultSet.DebugInfo[ "SPARQLQueryDebug" ] = "SELECT" + resultsAndTotalQuery.Split( new string[] { "> SELECT" }, StringSplitOptions.RemoveEmptyEntries )[ 1 ];
			resultSet.DebugInfo[ "ResultsAndTotal" ] = resultsAndTotal;
			*/

			//Get the description set URI map (or related data URI map for non-description sets)
			try
			{
				//Get the total results and result payloads
				//resultSet.TotalResults = int.Parse( resultsAndTotalQueryData.TotalResultsData[ "results" ][ "bindings" ].FirstOrDefault( m => m[ "totalResults" ] != null )[ "totalResults" ][ "value" ].ToString() );
				//resultSet.SearchResults = resultsAndTotalQueryData.ResultPayloadData[ "results" ][ "bindings" ].Where( m => m[ "searchResultPayload" ] != null ).Select( m => JObject.Parse( m[ "searchResultPayload" ][ "value" ].ToString() ) ).ToList();

				resultSet.TotalResults = int.Parse( resultsAndTotalQueryData.CombinedQueryData[ "results" ][ "bindings" ].FirstOrDefault( m => m[ "totalResults" ] != null )[ "totalResults" ][ "value" ].ToString() );
				resultSet.SearchResults = resultsAndTotalQueryData.CombinedQueryData[ "results" ][ "bindings" ].Where( m => m[ "searchResultPayload" ] != null ).Select( m => JObject.Parse( m[ "searchResultPayload" ][ "value" ].ToString() ) ).ToList();

				//For each result, get the related data (bnodes and (if applicable) description sets)
				var uriAndTypeData = new Dictionary<string, string>();
				foreach ( var searchResult in resultSet.SearchResults )
				{
					uriAndTypeData.Add( ( searchResult[ "@id" ] ?? "" ).ToString(), ( searchResult[ "@type" ] ?? "" ).ToString() );
				}

				resultSet.RelatedItemsData = SPARQLServices.GetRelatedItemsData( uriAndTypeData, descriptionSetType, apiKey, referrer, relatedURIsLimit, relatedItemsLimit, includeDebugInfo );
				resultSet.DebugInfo[ "RelatedItemsDebugInfo" ] = resultSet.RelatedItemsData.DebugInfo;

				/*
				//Try to find relevance scores and add them (if applicable - not all searches will have these, since they only apply to a SortOrder of Default (relevance) for queries that have one or more text queries)
				try
				{
					foreach( var resultMap in resultSet.RelatedItemsData.RelatedItemsMap.Properties() )
					{
						var match = resultsAndTotalQueryData.ResultPayloadData[ "results" ][ "bindings" ].FirstOrDefault( m => m[ "searchResultPayload" ] != null && m[ "relevance_final_score" ] != null && JObject.Parse( m[ "searchResultPayload" ][ "value" ].ToString() )[ "@id" ].ToString().Replace( "/graph/", "/resources/" ).ToLower() == resultMap.Name.Replace( "/graph/", "/resources" ).ToLower() );
						if( match != null )
						{
							( ( JObject ) resultSet.RelatedItemsData.RelatedItemsMap[ resultMap.Name ] ).Add( "search:relevanceScore", match[ "relevance_final_score" ][ "value" ] );
						}
						else
						{
							resultSet.DebugInfo[ "Error_" + Guid.NewGuid().ToString() ] = "Error calculating relevance score for " + resultMap.Name;
						}
					}
				}
				catch( Exception ex ) 
				{
					resultSet.DebugInfo[ "Error_" + Guid.NewGuid().ToString() ] = "Error calculating relevance score: " + ex.Message;
				}
				*/
			}
			catch ( Exception ex )
			{
				resultSet.Error = ex.Message;
			}

			//Return the data
			return resultSet;
		}
		//

		public class TotalAndPayloadQuery
		{
			public string TotalResultsQuery { get; set; }
			public JObject TotalResultsData { get; set; }
			public bool TotalResultsReady { get; set; }
			public string ResultPayloadQuery { get; set; }
			public JObject ResultPayloadData { get; set; }
			public bool ResultPayloadReady { get; set; }
			public string CombinedQuery { get; set; }
			public JObject CombinedQueryData { get; set; }
			public bool CombinedQueryReady { get; set; }
		}
		//

		public TotalAndPayloadQuery CTDLQueryToSPARQLQuery( JObject ctdlQuery, int skip, int take, SortOrders sortOrder = SortOrders.DEFAULT, bool orderDescending = false )
		{
			//Convert user query to SPARQL
			//IdentifyNodes( ctdlQuery );
			//ctdlQuery[ "@SPARQLID" ] = "?id";
			var sparql = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.HighestNode, PredicateProperty = null, ObjectVariable = "?id" };
			TranslateJSON( "", ctdlQuery, sparql );

			//Duplicate user query in order to get both a count and the desired page of search results
			var userQueryMetadata = new SPARQLNode.Metadata();
			var userQuerySPARQL = sparql.ToString( userQueryMetadata );
			userQuerySPARQL = userQuerySPARQL.Substring( 1, userQuerySPARQL.Length - 2 ).Trim(); //Trim the outermost { and } since we need to inject the payload line later

			//Get all of the properties, deduplicate them, remove search-specific ones, then use them as a pre-filter to screen out all results that can't ever possibly match the query
			//Actually - we can't use this, since it breaks queries that rely on reverse connections (e.g. "find me all the X where Y points in at X").  There may be a way around that, if necessary.
			var queryProperties = new List<string>();
			GetAllQueryPropertiesForPayload( ctdlQuery, queryProperties, true );
			var uniqueSearchProperties = queryProperties.Distinct().Where( m => ( m.Contains( ":" ) || m.Contains( "@id" ) ) && !m.Contains( "search:" ) ).ToList();
			var propertyFilter = "";// uniqueSearchProperties.Count() > 0 ? " ?id credreg:__payload ?searchResultPayloadFilter FILTER (regex(?searchResultPayloadFilter, '" + string.Join("|", uniqueSearchProperties ) + "')) . " : "";

			//Combine the query parts
			//Ensure we only select top-level items by looking for things with a CTID
			var finalUserQuery = "?id ceterms:ctid ?anyValue . " + propertyFilter + " " + userQuerySPARQL;

			//Detect whether the user included a sort order
			var orderByVariable = "";
			var orderByString = "";
			var groupByString = "";
			if( userQuerySPARQL.Contains( "?orderByMeAscending" ) )
			{
				orderByVariable = "(?orderByMeAscending AS ?relevance_score)";
				orderByString = "ORDER BY ASC(?relevance_score)";
			}
			else if( userQuerySPARQL.Contains( "?orderByMeDescending" ) )
			{
				orderByVariable = "(?orderByMeDescending AS ?relevance_score)";
				orderByString = "ORDER BY DESC(?relevance_score)";
			}
			else if ( sortOrder == SortOrders.DEFAULT && userQuerySPARQL.Contains( "?relevance_points" ) )
			{
				//Do not use
				//Somehow this locks up the Finder's IIS Worker Process
				//It makes no sense
				//var sumItems = Regex.Matches( userQuerySPARQL, @"relevance_points_[a-f0-9]{32}" ).Cast<Match>().Select( m => m.Value ).Distinct().ToList();
				//var sumText = "SUM(" + string.Join( " + ", sumItems ) + ")";
				//orderByVariable = "(" + sumText + " AS ?relevance_score)";
				//

				orderByVariable = "(SUM(" + string.Join( " + ", userQueryMetadata.RelevancePointsVariables.Distinct().Select( m => "COALESCE(" + m + ", 0)" ).ToList() ) + ") AS ?relevance_score)";
				//orderByVariable = "(SUM(?relevance_points) AS ?relevance_score)";
				groupByString = "GROUP BY ?id ?searchResultPayload ?relevance_score";
				//orderByString = "ORDER BY " + ( orderDescending ? "DESC" : "ASC" ) + "(?relevance_score)";
				orderByString = "ORDER BY DESC(?relevance_score)"; //Force the most relevant stuff to display if no explicit ordering is given, regardless of orderDescending
			}
			else if( sortOrder == SortOrders.CREATED )
			{
				orderByVariable = "?recordDate";
				orderByString = "ORDER BY " + ( orderDescending ? "DESC" : "ASC" ) + "(?recordDate)";
				finalUserQuery += " ?id ( credreg:__graph? / credreg:__createdAt ) ?recordDate . ";
			}
			else if( sortOrder == SortOrders.UPDATED )
			{
				orderByVariable = "?recordDate";
				orderByString = "ORDER BY " + ( orderDescending ? "DESC" : "ASC" ) + "(?recordDate)";
				finalUserQuery += " ?id ( credreg:__graph? / credreg:__updatedAt ) ?recordDate . ";
			}
			else
			{
				orderByString = "ORDER BY DESC(?id)";
			}

			var result = new TotalAndPayloadQuery();

			//Figure out which prefixes apply
			var prefixSPARQL = GetContextPrefixes( "credreg: " + finalUserQuery );

			//Force the query to be evaluated in the specified order
			var queryHint = "<http://aws.amazon.com/neptune/vocab/v01/QueryHints#Query> <http://aws.amazon.com/neptune/vocab/v01/QueryHints#joinOrder> 'Ordered' ."; 

			//Build the Total Results query
			result.TotalResultsQuery = prefixSPARQL + " SELECT (COUNT(DISTINCT ?id) AS ?totalResults) WHERE { " + queryHint + " " + finalUserQuery + " }";

			//Build the Result Payload query
			result.ResultPayloadQuery = prefixSPARQL + " SELECT DISTINCT ?searchResultPayload " + orderByVariable + " WHERE { " + queryHint + " " + finalUserQuery + " ?id credreg:__payload ?searchResultPayload . } " + groupByString + " " + orderByString + " OFFSET " + skip + " LIMIT " + take;

			//Build the Combined Query
			result.CombinedQuery = prefixSPARQL + " SELECT ?totalResults ?id ?searchResultPayload ?relevance_score WITH { SELECT DISTINCT ?id ?searchResultPayload " + orderByVariable + " WHERE { " + queryHint + " " + finalUserQuery + " ?id credreg:__payload ?searchResultPayload . }  " + groupByString + " } AS %mainQuery WHERE { { SELECT (COUNT(DISTINCT ?id) AS ?totalResults) WHERE { INCLUDE %mainQuery } } UNION { SELECT ?id ?searchResultPayload ?relevance_score ?recordDate WHERE { INCLUDE %mainQuery } " + orderByString + " OFFSET " + skip + " LIMIT " + take + " } } " + orderByString;

			return result;

			/*
			//Build the query
			var wrappedQuery = "SELECT ?totalResults ?searchResultPayload " + orderByVariable + " WHERE { { SELECT (COUNT(DISTINCT ?id) AS ?totalResults) WHERE { " + userQuerySPARQL + " } } UNION " +
				"{ SELECT DISTINCT ?searchResultPayload " + orderByVariable + " WHERE { " + userQuerySPARQL + " ?id credreg:__payload ?searchResultPayload . } } } " + orderByString + " OFFSET " + skip + " LIMIT " + take;
			var prefixSPARQL = GetContextPrefixes( wrappedQuery );

			//Return the full query
			var finalQuery = prefixSPARQL + " " + wrappedQuery; //Debugging
			return finalQuery;
			*/
		}
		//

		public void GetAllQueryPropertiesForPayload( JObject queryPart, List<string> queryProperties, bool allowURIProperties )
		{
			foreach( var property in queryPart.Properties() )
			{
				//Don't traverse out to other external objects, since we're using this to examine the payload of the root/search result object
				//However, do include the URI properties at the root level itself, just not anything beyond that
				if( !allowURIProperties && SchemaContext.URIProperties.Contains( property.Name ) )
				{
					continue;
				}

				queryProperties.Add( property.Name );
				if( property.Value.Type == JTokenType.Array )
				{
					foreach( var propertyValueItem in ((JArray) property.Value).Where( m => m.Type == JTokenType.Object ).ToList() )
					{
						GetAllQueryPropertiesForPayload( ( JObject ) propertyValueItem, queryProperties, true );
					}
				}
				else if( property.Value.Type == JTokenType.Object )
				{
					GetAllQueryPropertiesForPayload( ( JObject ) property.Value, queryProperties, property.Name.Contains( "search:" ) );
				}
			}
		}

		public string GetContextPrefixes( string sparql )
		{
			var usedPrefixes = SchemaContext.URIPrefixes.Where( m => sparql.Contains( m.Key + ":" ) ).ToList();
			var prefixSPARQL = string.Join( " ", usedPrefixes.Select( m => "PREFIX " + m.Key + ": <" + m.Value + ">" ).ToList() );
			return prefixSPARQL;
		}
		//

		public void TranslateJSON( string property, JToken token, SPARQLNode container )
		{
			var propertyLower = property.ToLower();
			if ( false ) //propertyLower == "search:termgroup" || propertyLower == "search:value" )
			{
				//var hasAndTerms = token.Parent != null && token.Parent.Parent != null && ( ( JObject ) token.Parent.Parent ).Properties().FirstOrDefault( m => m.Name.ToLower() == "search:operator" && m.Value.ToString().ToLower() == "search:andterms" ) != null;
				//var group = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.TermGroup, JoinChildrenWithUNION = !hasAndTerms }; //TODO: Fix this to be hasOrTerms and test for negative impact
				var hasOrTerms = token.Parent != null && token.Parent.Parent != null && ( ( JObject ) token.Parent.Parent ).Properties().FirstOrDefault( m => m.Name.ToLower() == "search:operator" && m.Value.ToString().ToLower() == "search:orterms" ) != null;
				var group = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.TermGroup, JoinChildrenWithUNION = hasOrTerms };
				container.AddChild( group );
				group.ObjectVariable = container.ObjectVariable; //Ensure this falls through to the child object(s)
				TranslateJSON( "", token, group );
			}

			//Term Groups
			if ( propertyLower == "search:termgroup" )
			{
				//var hasAndTerms = token.Parent != null && token.Parent.Parent != null && ( ( JObject ) token.Parent.Parent ).Properties().FirstOrDefault( m => m.Name.ToLower() == "search:operator" && m.Value.ToString().ToLower() == "search:andterms" ) != null;
				//var group = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.TermGroup, JoinChildrenWithUNION = !hasAndTerms }; //TODO: Fix this to be hasOrTerms and test for negative impact
				//var hasOrTerms = token.Parent != null && token.Parent.Parent != null && ( ( JObject ) token.Parent.Parent ).Properties().FirstOrDefault( m => m.Name.ToLower() == "search:operator" && m.Value.ToString().ToLower() == "search:orterms" ) != null;
				var hasOrTerms = token.Type == JTokenType.Array || ( token.Type == JTokenType.Object && ( ( JObject ) token ).Properties().FirstOrDefault( m => m.Name.ToLower() == "search:operator" && m.Value.ToString().ToLower() == "search:orterms" ) != null );
				var group = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.TermGroup, JoinChildrenWithUNION = hasOrTerms };
				container.AddChild( group );
				group.ObjectVariable = container.ObjectVariable; //Ensure this falls through to the child object(s)
				TranslateJSON( "", token, group );
			}

			//Value Arrays
			else if ( propertyLower == "search:value" )
			{
				//var hasAndTerms = token.Parent != null && token.Parent.Parent != null && ( ( JObject ) token.Parent.Parent ).Properties().FirstOrDefault( m => m.Name.ToLower() == "search:operator" && m.Value.ToString().ToLower() == "search:andterms" ) != null;
				var hasAndTerms = token.Parent != null && token.Parent.Type == JTokenType.Object && ( ( JObject ) token.Parent ).Properties().FirstOrDefault( m => m.Name.ToLower() == "search:operator" && m.Value.ToString().ToLower() == "search:andterms" ) != null;
				var group = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.TermGroup, JoinChildrenWithUNION = !hasAndTerms };
				container.AddChild( group );
				group.ObjectVariable = container.ObjectVariable; //Ensure this falls through to the child object(s)
				TranslateJSON( "", token, group );
			}


			//AndValues/OrValues
			else if ( propertyLower == "search:andvalues" || propertyLower == "search:orvalues" )
			{
				var group = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.TermGroup, JoinChildrenWithUNION = propertyLower == "search:orvalues" };
				container.AddChild( group );
				group.ObjectVariable = container.ObjectVariable; //Ensure this falls through to the child object(s)
				TranslateJSON( "", token, group );
			}

			//NotTerms
			else if( propertyLower == "search:notvalues" )
			{
				var group = new SPARQLNode() { SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.NotGroup, JoinChildrenWithUNION = token.Type == JTokenType.Array };
				container.AddChild( group );
				group.ObjectVariable = container.ObjectVariable; //Ensure this falls through to the child object(s)
				TranslateJSON( "", token, group );
			}

			//Array Tokens
			else if ( token.Type == JTokenType.Array )
			{
				var array = ( JArray ) token;
				var selfContainer = container;

				//Normal Tokens
				if( array.Any( m => m.Type == JTokenType.Object ) )
				{
					if( selfContainer.SPARQLNodeType != SPARQLNode.SPARQLNodeTypes.TermGroup )
					{
						selfContainer.JoinChildrenWithUNION = true;
					}
					foreach ( var item in array )
					{
						TranslateJSON( property, item, selfContainer );
					}
				}
				else
				{
					HandleValues( property, token, selfContainer, container.ObjectVariable );
				}
			}
			//Object Tokens
			else if( token.Type == JTokenType.Object )
			{
				var selfProperties = ( ( JObject ) token ).Properties();
				var searchProperties = selfProperties.Where( m => m.Name.Contains( "search:" ) ).ToList();
				var nonSearchProperties = selfProperties.Where( m => !m.Name.Contains( "search:" ) ).ToList();

				//Create an object to hold the content
				var selfContainer = SPARQLNode.CreateObjectWrapper( container.ObjectVariable, property );
				container.AddChild( selfContainer );
				if ( selfContainer.Parent != null && ( selfContainer.Parent.SPARQLNodeType == SPARQLNode.SPARQLNodeTypes.TermGroup || selfContainer.Parent.SPARQLNodeType == SPARQLNode.SPARQLNodeTypes.NotGroup || selfContainer.Parent.SPARQLNodeType == SPARQLNode.SPARQLNodeTypes.HighestNode ) )
				{
					selfContainer.ObjectVariable = selfContainer.Parent.ObjectVariable; //Ensure this falls through
					selfContainer.JoinChildrenWithUNION = selfContainer.Parent.JoinChildrenWithUNION; //Ensure search:andTerms and search:orTerms falls through
				}

				//Handle logical AND/OR override
				if( searchProperties.Count() > 0 )
				{
					var searchOperator = searchProperties.FirstOrDefault( m => m.Name.ToLower() == "search:operator" );
					if( searchOperator != null )
					{
						selfContainer.JoinChildrenWithUNION = searchOperator.Value.ToString().ToLower() == "search:orterms";
					}
				}
				//Handle language maps queried as objects
				if ( SchemaContext.LanguageMapProperties.Contains( property ) )
				{
					HandleLanguageMap_Node( property, token, selfContainer );
				}
				//Otherwise just treat the object as normal
				else
				{
					foreach ( var selfProperty in selfProperties )
					{
						TranslateJSON( selfProperty.Name, selfProperty.Value, selfContainer );
					}
				}
			}
			//Value Tokens
			else
			{
				HandleValues( property, token, container, container.ObjectVariable );
			}
		}
		//

		public void HandleValues( string property, JToken token, SPARQLNode container, string subjectVariable = null )
		{
			//Don't let search: properties leak into the query unless explicitly allowed
			var allowableSearchProperties = new List<string>() { "search:orderbyascending", "search:orderbydescending" };
			if ( property.ToLower().Contains( "search:" ) && !allowableSearchProperties.Contains( property.ToLower() ) )
			{
				return;
			}

			//Normalize the value
			subjectVariable = subjectVariable ?? Guid.NewGuid().ToString();
			var normalizedValue = NormalizeTokenValues( token );
			var result = new SPARQLNode( property, normalizedValue, SPARQLNode.ValueWrapperTypes.None, subjectVariable ) { JoinChildrenWithUNION = token.Type == JTokenType.Array };

			//Normalize the predicate
			result.PredicateDirection = property.IndexOf( ">" ) == 0 ? SPARQLNode.PredicateDirections.Outbound : property.IndexOf( "<" ) == 0 ? SPARQLNode.PredicateDirections.Inbound : SPARQLNode.PredicateDirections.Both;
			result.PredicateProperty = property.Replace( ">", "" ).Replace( "<", "" ); //Account for possible explicit directionality
			var normalizedProperty = result.PredicateProperty;

			//Special properties
			if( property == "@type" )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.JSONLD_Type;
			}
			else if( property == "@id" )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.JSONLD_ID;
			}
			else if( property == "ceterms:ctid" )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.CTIDNode;
			}
			else if( normalizedValue.Select( m => m.ToString().ToLower()).ToList().Contains("search:anyvalue") )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.SearchAnyValue;
			}
			else if ( normalizedProperty.ToLower() == "search:orderbyascending" || normalizedProperty.ToLower() == "search:orderbydescending" )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.SortOrderNode;
			}
			else if ( SchemaContext.BooleanProperties.Concat( SchemaContext.IntegerProperties ).Contains( normalizedProperty ) )
			{
				result.ValueWrapperType = SPARQLNode.ValueWrapperTypes.None;
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.IntegerNode;
			}
			else if ( SchemaContext.BooleanProperties.Concat( SchemaContext.FloatProperties ).Contains( normalizedProperty ) )
			{
				result.ValueWrapperType = SPARQLNode.ValueWrapperTypes.None;
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.FloatNode;
			}
			else if ( SchemaContext.BooleanProperties.Concat( SchemaContext.DecimalProperties ).Contains( normalizedProperty ) )
			{
				result.ValueWrapperType = SPARQLNode.ValueWrapperTypes.None;
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.DecimalNode;
			}
			else if ( SchemaContext.StringProperties.Contains( normalizedProperty ) )
			{
				result.ValueWrapperType = SPARQLNode.ValueWrapperTypes.SurroundWithSingleQuotes;
				result.SPARQLNodeType = QueryContext.SortOrder == SortOrders.DEFAULT ? SPARQLNode.SPARQLNodeTypes.StringNode_WithRelevance : SPARQLNode.SPARQLNodeTypes.StringNode;
			}
			else if ( SchemaContext.URIProperties.Contains( normalizedProperty ) )
			{
				result.ObjectValues = result.ObjectValues.Select( m => ExpandVocabularyURI( m ) ).ToList();
				result.ValueWrapperType = SPARQLNode.ValueWrapperTypes.SurroundWithSingleQuotes;
				result.SPARQLNodeType = QueryContext.SortOrder == SortOrders.DEFAULT ? SPARQLNode.SPARQLNodeTypes.URINode_WithRelevance : SPARQLNode.SPARQLNodeTypes.URINode;
			}
			else if ( SchemaContext.LanguageMapProperties.Contains( normalizedProperty ) )
			{
				result = HandleLanguageMap_Node( normalizedProperty, token );
			}
			else if ( SchemaContext.DateProperties.Contains( normalizedProperty ) )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.DateNode;
				result.ObjectValues = normalizedValue;
			}
			else if ( SchemaContext.DateTimeProperties.Contains( normalizedProperty ) )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.DateTimeNode;
				result.ObjectValues = normalizedValue;
			}
			else if ( SchemaContext.LanguageBCP47Properties.Contains( normalizedProperty ) )
			{
				result.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageBCP47Node;
				result.ObjectValues = normalizedValue;
			}
			else
			{
				if( token.Type == JTokenType.String )
				{
					result.ValueWrapperType = SPARQLNode.ValueWrapperTypes.SurroundWithSingleQuotes;
				}
			}

			container.AddChild( result );

		}
		public string ExpandVocabularyURI( string shortURI )
		{
			//If the shortURI is a prefix, expand it
			var prefix = SchemaContext.URIPrefixes.Where( m => shortURI.Trim().ToLower().IndexOf( m.Key.ToLower() + ":" ) == 0 ).ToList();
			if ( prefix.Count() > 0 )
			{
				return prefix.FirstOrDefault().Value + ( shortURI.Split( new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries ).LastOrDefault() ?? "" );
			}

			//Otherwise just return it
			return shortURI;
		}
		//

		public SPARQLNode HandleLanguageMap_Node( string property, JToken token, SPARQLNode container = null )
		{
			//Setup the container
			container = container ?? new SPARQLNode();
			container.PredicateProperty = property;
			container.ObjectVariable = string.IsNullOrWhiteSpace( container.ObjectVariable ) ? SPARQLNode.GenerateVariable() : container.ObjectVariable;
			container.SPARQLNodeType = QueryContext.SortOrder == SortOrders.DEFAULT ? SPARQLNode.SPARQLNodeTypes.LanguageMap_Node_WithRelevance : SPARQLNode.SPARQLNodeTypes.LanguageMap_Node;
			container.JoinChildrenWithUNION = false;

			//Normalize the value
			if( token.Type == JTokenType.Array )
			{
				var objectValues = token.Values().Where( m => m.Type == JTokenType.Object ).ToList();
				var stringValues = token.Values().Where( m => m.Type == JTokenType.String ).ToList();
				
				//Handle object list
				foreach( var item in objectValues )
				{
					HandleLanguageMapPairSet( ( JObject ) item, container );
				}

				//Handle string list
				if( stringValues.Count() > 0 )
				{
					var pair = CreateLanguageMap_LanguageValuePair( null, stringValues.Select( m => m.ToString() ).ToList(), true, container );
					pair.Children.ForEach( m => m.JoinChildrenWithUNION = true );
					container.AddChild( pair );
				}
			}
			else if( token.Type == JTokenType.Object )
			{
				HandleLanguageMapPairSet( ( JObject ) token, container );
			}
			else
			{
				var pair = CreateLanguageMap_LanguageValuePair( null, new List<string>() { token.ToString() }, true, container );
				pair.Children.ForEach( m => m.JoinChildrenWithUNION = true );
				container.AddChild( pair );
			}

			return container;
		}
		//

		public void HandleLanguageMapPairSet( JObject token, SPARQLNode container )
		{
			var properties = token.Properties();
			var searchLanguageCode = properties.FirstOrDefault( m => m.Name.ToLower() == "search:languagecode" );
			var searchLanguageValue = properties.FirstOrDefault( m => m.Name.ToLower() == "search:languagevalue" );
			var searchValue = properties.FirstOrDefault( m => m.Name.ToLower() == "search:value" );
			var searchOperator = properties.FirstOrDefault( m => m.Name.ToLower() == "search:operator" );
			var languageCodeProperties = properties.Where( m => !m.Name.Contains( "search:" ) ).ToList();

			//Allow exactly one of a a languagecode/languagevalue/operator set or value/operator set or a code/value set
			if ( searchLanguageCode != null || searchLanguageValue != null  )
			{
				container.JoinChildrenWithUNION = false;
				var pair = new SPARQLNode()
				{
					ObjectVariable = container.ObjectVariable,
					SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_LanguageValuePair,
					JoinChildrenWithUNION = searchOperator != null && searchOperator.Value.ToString().ToLower() == "search:orterms"
				};

				if ( searchLanguageCode != null )
				{
					var child = NormalizeLanguageMap_LanguageValuePairToken( searchLanguageCode.Value );
					child.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_Language;
					pair.AddChild( child );
				}

				if ( searchLanguageValue != null )
				{
					var child = NormalizeLanguageMap_LanguageValuePairToken( searchLanguageValue.Value );
					child.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_Value;
					pair.AddChild( child );
				}

				container.AddChild( pair );
			}
			else if( searchValue != null )
			{
				container.JoinChildrenWithUNION = false;
				var pair = new SPARQLNode()
				{
					ObjectVariable = container.ObjectVariable,
					SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_LanguageValuePair,
					JoinChildrenWithUNION = false
				};

				var child = NormalizeLanguageMap_LanguageValuePairToken( searchValue.Value );
				child.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_Value;
				child.JoinChildrenWithUNION = searchOperator != null && searchOperator.Value.ToString().ToLower() == "search:orterms";
				pair.AddChild( child );

				container.AddChild( pair );
			}
			else
			{
				container.JoinChildrenWithUNION = searchOperator != null && searchOperator.Value.ToString().ToLower() == "search:orterms";

				foreach ( var codeValuePair in languageCodeProperties )
				{
					var pair = new SPARQLNode()
					{
						ObjectVariable = container.ObjectVariable,
						SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_LanguageValuePair,
						JoinChildrenWithUNION = false
					};

					var code = codeValuePair.Name;
					var value = NormalizeLanguageMap_LanguageValuePairToken( codeValuePair.Value );
					value.SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_Value;

					pair.AddChild( new SPARQLNode()
					{
						SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_Language,
						ObjectValues = new List<string>() { code }
					} );
					pair.AddChild( value );

					container.AddChild( pair );
				}
			}
		}
		//

		public SPARQLNode NormalizeLanguageMap_LanguageValuePairToken( JToken token )
		{
			var node = new SPARQLNode();

			if ( token.Type == JTokenType.Object )
			{
				var valueObject = ( JObject ) token;
				var valueObjectProperties = valueObject.Properties();
				var searchValue = valueObjectProperties.FirstOrDefault( m => m.Name.ToLower() == "search:value" );
				var searchOperator = valueObjectProperties.FirstOrDefault( m => m.Name.ToLower() == "search:operator" );
				var searchAndValues = valueObjectProperties.FirstOrDefault( m => m.Name.ToLower() == "search:andvalues" );
				var searchOrValues = valueObjectProperties.FirstOrDefault( m => m.Name.ToLower() == "search:orvalues" );

				node.JoinChildrenWithUNION = ( searchOperator != null && searchOperator.Value.ToString().ToLower() == "search:orterms" ) || ( searchOrValues != null );
				var valueProperty = searchValue ?? searchAndValues ?? searchOrValues;
				if ( valueProperty != null )
				{
					node.ObjectValues = NormalizeTokenValues( valueProperty.Value );
				}
			}
			else
			{
				node.ObjectValues = NormalizeTokenValues( token );
				node.JoinChildrenWithUNION = true;
			}

			return node;
		}
		//

		public SPARQLNode CreateLanguageMap_LanguageValuePair( List<string> languageCodes, List<string> languageValues, bool joinChildrenWithUNION, SPARQLNode container )
		{
			var pair = new SPARQLNode()
			{
				SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_LanguageValuePair,
				ObjectVariable = container.ObjectVariable,
				JoinChildrenWithUNION = joinChildrenWithUNION
			};
			if ( languageCodes != null && languageCodes.Count() > 0 )
			{
				pair.AddChild( new SPARQLNode()
				{
					SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_Language,
					ObjectValues = languageCodes
				} );
			}
			if ( languageValues != null && languageValues.Count() > 0 )
			{
				pair.AddChild( new SPARQLNode()
				{
					SPARQLNodeType = SPARQLNode.SPARQLNodeTypes.LanguageMap_Value,
					ObjectValues = languageValues
				} );
			}
			return pair;
		}
		//

		private List<string> NormalizeTokenValues( JToken token )
		{
			if( token.Type == JTokenType.Array )
			{
				return ( ( JArray ) token ).Select( m => m.ToString() ).ToList();
			}
			else
			{
				return new List<string>() { token.ToString() };
			}
		}

		public RelatedItemsData GetRelatedItemsData( Dictionary<string, string> resultURIAndType, DescriptionSetType descriptionSetType, string apiKey, string referrer, int relatedURIsLimit, int relatedItemsLimit, bool includeDebugInfo )
		{
			//Ensure that if there is a related URIs limit, the related items limit is the same or lower
			if( relatedURIsLimit >= 1 )
			{
				relatedItemsLimit = relatedItemsLimit > relatedURIsLimit ? relatedURIsLimit : relatedItemsLimit;
			}

			//Setup container
			var relatedData = new RelatedItemsData() { 
				IncludeDebugInfo = includeDebugInfo,
				RelatedItemsLimit = relatedItemsLimit,
				RelatedURIsLimit = relatedURIsLimit
			};
			//Flat list of related nodes (actual data)
			//Map of URIs to make it easier to connect stuff in the related items list to the actual search results
			//e.g. { "https://uri1": { "bnodes": { "ceterms:AssessmentProfile": [ "_:123", "_:456" ] }, "somethingelse": [ ... ] }, "https://uri2": { ... } }
			foreach ( var item in resultURIAndType )
			{
				relatedData.ResultURIs.Add( item.Key );
			}

			//Use this to hold related URIs for description sets that are to be retrieved asynchronously
			var dspURIs = new ConcurrentBag<string>(); //Thread-Safe, unordered equivalent of List<T>

			//Handle Description Sets
			if ( descriptionSetType == DescriptionSetType.Resource )
			{
				//Do nothing
			}
			else if( descriptionSetType == DescriptionSetType.Resource_Graph ) //Backwards compatibility with old search default, deprecated
			{
				GetRelatedGraph( relatedData, resultURIAndType );
				AddRelatedGraphItemsWithLimit( relatedData );
			}
			else if( descriptionSetType == DescriptionSetType.Resource_RelatedURIs ) //Cool people use this new one
			{
				GetRelatedURIsFromRegistry( relatedData, dspURIs, apiKey, referrer, false );
				//GetRelatedURIsFromSPARQL( relatedData, dspURIs, apiKey, referrer );
			}
			else if( descriptionSetType == DescriptionSetType.Resource_RelatedURIs_Graph ) //Not used
			{
				GetRelatedURIsFromRegistry( relatedData, dspURIs, apiKey, referrer, false );
				//GetRelatedURIsFromSPARQL( relatedData, dspURIs, apiKey, referrer );
				GetRelatedGraph( relatedData, resultURIAndType );
				AddRelatedGraphItemsWithLimit( relatedData );
			}
			else if ( descriptionSetType == DescriptionSetType.Resource_RelatedURIs_RelatedData )
			{
				GetRelatedURIsFromRegistry( relatedData, dspURIs, apiKey, referrer, true );
			}
			else if( descriptionSetType == DescriptionSetType.Resource_RelatedURIs_Graph_RelatedData ) //Not used
			{
				GetRelatedURIsFromRegistry( relatedData, dspURIs, apiKey, referrer, true );
				//GetRelatedURIsFromSPARQL( relatedData, dspURIs, apiKey, referrer );
				GetRelatedGraph( relatedData, resultURIAndType );
				//GetRelatedDescriptionSetData( relatedData, apiKey, referrer );
			}

			return relatedData;
		}
		//

		public void GetRelatedURIsFromRegistry( RelatedItemsData relatedData, ConcurrentBag<string> dspURIs, string apiKey, string referrer, bool includeData )
		{
			//Setup
			var getDescriptionSetsURL = Utilities.UtilityManager.GetAppKeyValue( "accountsGetDescriptionSetsForCTIDsAPI" );
			var client = new HttpClient();
			var request = new AccountConsumeRequest()
			{
				ApiKey = apiKey,
				DescriptionSetCTIDs = relatedData.ResultURIs.Select( m => "ce-" + m.Split( new string[] { "/ce-" }, StringSplitOptions.RemoveEmptyEntries ).Last() ).Distinct().ToList(),
				DescriptionSetRelatedURIsLimit = relatedData.RelatedURIsLimit,
				DescriptionSetRelatedItemsLimit = relatedData.RelatedItemsLimit,
				ShouldLogRequest = true,
				DescriptionSetIncludeData = includeData
			};

			//Add the referrer if present
			try
			{
				client.DefaultRequestHeaders.Referrer = new Uri( referrer );
			}
			catch { }

			//Prepare to do the request
			var status = System.Net.HttpStatusCode.OK;
			var result = new JObject();

			//Check the query
			relatedData.DebugInfo.Add( "Related URIs Limit", relatedData.RelatedURIsLimit );
			relatedData.DebugInfo.Add( "Related Items Limit", relatedData.RelatedItemsLimit );

			try
			{
				//Do the request
				var httpResult = client.PostAsync( getDescriptionSetsURL, new StringContent( JsonConvert.SerializeObject( request ), Encoding.UTF8, "application/json" ) ).Result;
				status = httpResult.StatusCode;
				var httpResultBody = httpResult.Content.ReadAsStringAsync().Result;
				relatedData.DebugInfo[ "Get Related URIs: Raw Status Code" ] = status.ToString();
				relatedData.DebugInfo[ "Get Related URIs: Raw Content" ] = httpResultBody;
				result = JObject.Parse( httpResultBody );

				//Store debug info first
				relatedData.DebugInfo[ "Get Related URIs: Raw Result" ] = result;

				//Process data
				relatedData.RelatedItems = ( ( JArray ) result[ "RelatedItems" ] ).Select( m => ( JObject ) m ).ToList();
				relatedData.RelatedItemsMap = ( ( JArray ) result[ "RelatedItemsMap" ] ).Select( m => ( ( JObject ) m ).ToObject<RelatedItemsSet>() ).ToList();
			}
			catch( Exception ex )
			{
				relatedData.DebugInfo[ "Error loading related item URIs" ] = ex.Message;
			}
		}
		//

		public void GetRelatedURIsFromRegistryOLD( RelatedItemsData relatedData, ConcurrentBag<string> dspURIs, string apiKey, string referrer )
		{
			var DSPAPIURL = Utilities.UtilityManager.GetAppKeyValue( "GetDescriptionSetByCTIDEndpoint" ); //Not populated!
			var registryAuthorizationToken = Utilities.UtilityManager.GetAppKeyValue( "CredentialRegistryAuthorizationToken" ); //Admin-level access to the registry for CE's account

			var client = new HttpClient();
			client.DefaultRequestHeaders.TryAddWithoutValidation( "Authorization", "Token " + registryAuthorizationToken );

			var allURIsList = new List<string>();
			foreach( var uri in relatedData.ResultURIs )
			{
				var requestURL = "";
				try
				{
					var ctid = "ce-" + uri.Split( new string[] { "/ce-" }, StringSplitOptions.RemoveEmptyEntries ).Last();
					requestURL = DSPAPIURL + ctid + ( relatedData.RelatedItemsLimit > 0 ? "?limit=" + relatedData.RelatedItemsLimit : "" );

					var httpResult = client.GetAsync( requestURL ).Result;
					var httpResultBody = httpResult.Content.ReadAsStringAsync().Result;
					var resultData = JArray.Parse( httpResultBody );

					var itemMap = new RelatedItemsSet();
					itemMap.ResourceURI = uri.Replace( "/graph/", "/resources/" );

					foreach ( JObject rawMap in resultData )
					{
						var uris = ( ( JArray ) rawMap[ "uris" ] ).Select( m => m.ToString() ).ToList();
						itemMap.RelatedItems.Add( new RelatedItemsPath()
						{
							Path = rawMap[ "path" ].ToString(),
							URIs = uris,
							TotalURIs = ( int ) rawMap[ "total" ]
						} );

						allURIsList.AddRange( uris );
					}

					relatedData.RelatedItemsMap.Add( itemMap );
				}
				catch ( Exception ex )
				{
					relatedData.DebugInfo[ "Error getting related data for " + uri ] = new JObject()
					{
						{ "Error", ex.Message },
						{ "Request URL", requestURL }
					};
				}
			}

			//Add URIs to the DSP list
			var allReferencedURIs = allURIsList.Distinct().ToList();
			foreach( var uri in allReferencedURIs )
			{
				dspURIs.Add( uri );
			}
		}
		//

		public void GetRelatedURIsFromSPARQL( RelatedItemsData relatedData, ConcurrentBag<string> dspURIs, string apiKey, string referrer )
		{
			try
			{
				//Get the result URIs and build the query
				var uriValues = "VALUES ?id { " + string.Join( " ", relatedData.ResultURIs.Select( m => "<" + m + ">" ).ToList() ) + " }";
				var sparql = "PREFIX credreg: <https://credreg.net/> SELECT DISTINCT ?id ?itemMapPath ?itemMapURI WHERE { " + uriValues + " ?id ( credreg:__descriptionSet / credreg:__relatedItemsMap ) ?relatedItemsMap . ?relatedItemsMap credreg:__dspPath ?itemMapPath . ?relatedItemsMap credreg:__dspURI ?itemMapURI . }";

				relatedData.DebugInfo[ "RelatedItemsQuery" ] = sparql;
				//Run the query
				var rawResults = RunSPARQLQuery( sparql, apiKey, 0, -1, referrer, false );
				relatedData.DebugInfo[ "rawResults" ] = rawResults ?? new JObject() { { "empty", "null" } };
				var bindings = ( ( JArray ) rawResults[ "results" ][ "bindings" ] ).Select( m => ( JObject ) m ).ToList();

				//For each result...
				foreach ( var uri in relatedData.ResultURIs )
				{
					//Find the relevant bindings and create somewhere to put them
					var bindingsForResult = bindings.Where( m => m[ "id" ][ "value" ].ToString() == uri ).ToList();
					var itemMap = new RelatedItemsSet();

					itemMap.ResourceURI = uri.Replace( "/graph/", "/resources/" );

					//Add the individual path/URI list data
					var paths = bindingsForResult.Select( m => m[ "itemMapPath" ][ "value" ].ToString() ).Distinct().ToList();
					foreach ( var path in paths )
					{
						itemMap.RelatedItems.Add( new RelatedItemsPath()
						{
							Path = path,
							URIs = bindingsForResult.Where( m => m[ "itemMapPath" ][ "value" ].ToString() == path ).Select( m => m[ "itemMapURI" ][ "value" ].ToString() ).ToList()
						} );
					}

					relatedData.RelatedItemsMap.Add( itemMap );
				}

				//Add URIs to the DSP list
				var allReferencedURIs = bindings.Select( m => m[ "itemMapURI" ][ "value" ].ToString() ).Distinct().Where( m => !dspURIs.Contains( m ) ).ToList();
				foreach ( var uri in allReferencedURIs )
				{
					dspURIs.Add( uri );
				}
			}
			catch ( Exception ex )
			{
				relatedData.DebugInfo[ "Error" ] = ex.Message;
			}
		}
		//

		private void AddRelatedGraphItemsWithLimit( RelatedItemsData relatedData )
		{
			//Dump all of the non-bnode temp graph items into the result, up to the limit
			//For each result's related graph items...
			foreach ( var resultURIAndGraphItems in relatedData.TemporaryRelatedGraphItems )
			{
				//Take the first n items and deduplicate after, so that a consistent number of related items are returned
				//If there's no limit, just add all the items that aren't already in the array
				var itemsToAdd = resultURIAndGraphItems.Value.Take( relatedData.RelatedItemsLimit <= 0 ? resultURIAndGraphItems.Value.Count() : relatedData.RelatedItemsLimit ).ToList();
				relatedData.RelatedItems.AddRange( itemsToAdd.Where( m => relatedData.RelatedItems.Where( n => n[ "@id" ] == m[ "@id" ] ).Count() == 0 ).ToList() );
			}
		}
		//

		public void GetRelatedGraph( RelatedItemsData relatedData, Dictionary<string, string> resultURIAndType )
		{
			var graphURIs = resultURIAndType.Select( m => m.Key.ToString().ToLower().Replace( "/resources/", "/graph/" ) ).Distinct().ToList();
			var relatedItemURIs = new List<string>();
			//Get all of the graphs
			var graphs = GetAsyncData( graphURIs );
			foreach ( var graph in graphs )
			{
				var resourceURI = graph.Key.Replace( "/graph/", "/resources/" );
				try
				{
					//Parse the graph JSON
					var graphData = JObject.Parse( graph.Value );
					//This will hold the related bnodes for a given URI
					var graphRelatedNodes = new List<JObject>();
					//For each thing in the @graph...
					foreach ( JObject node in ( JArray ) graphData[ "@graph" ] )
					{
						//Get the @id
						var id = ( node[ "@id" ] ?? "" ).ToString();
						//Skip the main item so that the related items array doesn't contain the search results themselves
						if ( id != resourceURI && id != graph.Key )
						{
							//Add the node to the list of related nodes (will be processed into the map) regardless of whether the related items array contains it
							if( id.IndexOf( "_:" ) == 0 )
							{
								relatedData.RelatedItems.Add( node ); //Store the node in the array that ultimately makes it back to the user
							}
							else if( id.IndexOf( "http" ) > -1 )
							{
								//If the node isn't already in the related items array, add it (and track its ID in the list)
								if ( !relatedItemURIs.Contains( id ) )
								{
									graphRelatedNodes.Add( node ); //Store the node for later processing
									relatedItemURIs.Add( id ); //Store the node's @id in this temporary array that helps with deduplication/efficiency
								}
							}

						}
					}

					//Track any nodes that showed up in the graph but not in the DSP
					//Note - this can happen when the related URIs limit is set and graph data is retrieved
					//For example, a limit of 10 and a competency framework with 20 competencies will yield 10 related graph items (the first 10 are counted in the DSP and the second 10 are from the graph)
					var relatedItemMapForResult = relatedData.RelatedItemsMap.FirstOrDefault( m => m.ResourceURI == resourceURI ) ?? new RelatedItemsSet();
					var nodesForItem = relatedItemMapForResult.RelatedItems.SelectMany( m => m.URIs ).Distinct().ToList();
					var unaccountedFor = graphRelatedNodes.Where( m => !nodesForItem.Contains( m[ "@id" ].ToString() ) ).ToList();
					if( unaccountedFor.Count() > 0 )
					{
						relatedItemMapForResult.RelatedItems.Add( new RelatedItemsPath()
						{
							Path = "> search:relatedGraphItem > search:Object",
							URIs = unaccountedFor.Select( m => m[ "@id" ].ToString() ).ToList(),
							TotalURIs = unaccountedFor.Count()
						} );
					}

					//Store the related nodes
					relatedData.TemporaryRelatedGraphItems.Add( resourceURI, graphRelatedNodes );
				}
				catch ( Exception ex )
				{
					var relatedMap = relatedData.RelatedItemsMap.FirstOrDefault( m => m.ResourceURI == resourceURI ) ?? new RelatedItemsSet();
					relatedMap.DebugInfo.Add( "GraphNodes_Error", ex.Message );
					relatedMap.DebugInfo.Add( "GraphNodes_RawResponse", graph.Value );
				}
			}
		}
		//

		//TODO:
		//x 1. Get the @graph
		//x 2. Store the bnodes from the graph in the related results and track appropriately
		//x 3. Store the other nodes from the graph (ie competencies) somewhere else temporarily
		//x 4. In the method below, merge in the other nodes or retrieve them as necessary, up to the specified limit
		//x 5. Also, program in the limit
		public void GetRelatedDescriptionSetDataOLD( RelatedItemsData relatedData, ConcurrentBag<string> dspURIs, List<string> relatedItemURIs, string apiKey, string referrer )
		{
			//Figure out which URIs need to be retrieved (DSP URIs minus the items already in the relatedItemURIs list)
			var loadTheRest = dspURIs.Distinct().Where( m => !relatedItemURIs.Contains( m ) ).ToList();

			//Load the data and merge it into the relatedItems array
			var selectedToLoad = loadTheRest.Where( m => m.Contains( "://credentialengineregistry.org/" ) || m.IndexOf( "_:" ) == 0 ).Select( m => m.Replace( "_:", "https://credreg.net/bnodes/" ) ).ToList();
			if ( selectedToLoad.Count() > 0 )
			{
				var rawItemsFromSPARQL = RunSPARQLQuery( PayloadQuery( selectedToLoad ), apiKey, -1, -1, referrer );
				foreach ( JObject item in ( ( JArray ) rawItemsFromSPARQL[ "results" ][ "bindings" ] ) )
				{
					try
					{
						relatedData.RelatedItems.Add( JObject.Parse( item[ "payload" ][ "value" ].ToString().Replace( "https://credreg.net/bnodes/" , "_:" ) ) );
					}
					catch ( Exception ex )
					{
						relatedData.RelatedItems.Add( new JObject() { { "error", ex.Message }, { "rawData", item } } );
					}
				}
			}
		}
		//

		public void GetRelatedDescriptionSetData( RelatedItemsData relatedData, string apiKey, string referrer )
		{
			//Hold data
			var urisToLoad = new List<string>();
			var urisToGetFromRegistry = new List<string>();
			var alreadyLoaded = relatedData.RelatedItems.Select( m => ( m[ "@id" ] ?? "" ).ToString() ).ToList();

			//For each search result...
			foreach( var searchResultRelatedItems in relatedData.RelatedItemsMap )
			{
				//For each path/URI object for that search result...
				foreach ( var pathAndURIs in searchResultRelatedItems.RelatedItems ) 
				{
					//Take the first n URIs, or all of them if no limit
					var toLoadForPath = pathAndURIs.URIs.Take( relatedData.RelatedItemsLimit <= 0 ? pathAndURIs.URIs.Count() : relatedData.RelatedItemsLimit ).ToList();
					urisToLoad.AddRange( toLoadForPath );
				}
			}

			//Figure out which URIs still need to be loaded
			urisToLoad = urisToLoad.Distinct().Where( m => !alreadyLoaded.Contains( m ) ).ToList();

			//Where available, pull items from the @graphs that were previously loaded
			var preloadedGraphNodes = relatedData.TemporaryRelatedGraphItems.SelectMany( m => m.Value ).ToList();
			foreach( var uri in urisToLoad )
			{
				var match = preloadedGraphNodes.FirstOrDefault( m => m[ "@id" ].ToString() == uri );
				if( match != null )
				{
					relatedData.RelatedItems.Add( match );
				}
				else
				{
					urisToGetFromRegistry.Add( uri );
				}
			}

			//If there's anything left to get from the Registry, do so
			urisToGetFromRegistry = urisToGetFromRegistry.Distinct().ToList();
			if( urisToGetFromRegistry.Count() > 0 )
			{
				//Get as many items as possible from the Registry directly
				var ctids = urisToGetFromRegistry.Where( m => m.Contains( "/ce-" ) ).Select( m => "ce-" + ( m.Split( new string[] { "/ce-" }, StringSplitOptions.RemoveEmptyEntries ).LastOrDefault() ?? "" ) ).ToList();
				//relatedData.DebugInfo[ "CTIDs to be retrieved via multi-get" ] = JArray.FromObject( ctids );
				var ctidsLoaded = new List<string>();
				var ctidBasedResults = GetItemsByCTID( ctids );
				foreach( JObject item in ctidBasedResults )
				{
					var ctid = item[ "ceterms:ctid" ].ToString().ToLower();
					var match = urisToGetFromRegistry.FirstOrDefault( m => m.ToLower().Contains( ctid ) );
					if( match != null )
					{
						urisToGetFromRegistry.Remove( match );
						relatedData.RelatedItems.Add( item );
						ctidsLoaded.Add( ctid );
					}
				}
				//relatedData.DebugInfo[ "CTIDs loaded via multi-get" ] = JArray.FromObject( ctidsLoaded );

				//Collect any stragglers
				if( urisToGetFromRegistry.Count() > 0 )
				{
					relatedData.DebugInfo[ "URIs missing from CTID multi-get" ] = JArray.FromObject( urisToGetFromRegistry );
					var rawItemsFromSPARQL = RunSPARQLQuery( PayloadQuery( urisToGetFromRegistry ), apiKey, -1, -1, referrer );
					foreach ( JObject item in ( ( JArray ) rawItemsFromSPARQL[ "results" ][ "bindings" ] ) )
					{
						try
						{
							relatedData.RelatedItems.Add( JObject.Parse( item[ "payload" ][ "value" ].ToString().Replace( "https://credreg.net/bnodes/", "_:" ) ) );
						}
						catch ( Exception ex )
						{
							relatedData.RelatedItems.Add( new JObject() { { "error", ex.Message }, { "rawData", item } } );
						}
					}
				}
			}
		}
		//

		public static Dictionary<string, string> GetAsyncData( List<string> allURIs )
		{
			//Get things from the cache where available
			var results = new Dictionary<string, string>();
			var tasks = new Dictionary<string, Task<HttpResponseMessage>>();
			foreach( var uri in allURIs )
			{
				var result = (string) MemoryCache.Default.Get( "GetAsyncData_" + uri );
				if( result == null )
				{
					var client = new HttpClient();
					tasks.Add( uri, client.GetAsync( uri ) );
				}
				else
				{
					results.Add( uri, result );
				}
			}

			//Wait for all tasks to finish
			while( tasks.Where( m => !m.Value.IsCompleted ).Count() != 0 )
			{
				Thread.Sleep( 25 );
			}

			//Populate the results
			foreach( var task in tasks )
			{
				var result = tasks[ task.Key ].Result.Content.ReadAsStringAsync().Result;
				MemoryCache.Default.Remove( "GetAsyncData_" + task.Key );
				MemoryCache.Default.Add( "GetAsyncData_" + task.Key, result, DateTime.Now.AddMinutes( 15 ) );
				results.Add( task.Key, result );
			}

			//Return the data
			return results;
		}
		//

		public static JArray GetItemsByCTID( List<string> ctids )
		{
			var apiURL = Utilities.UtilityManager.GetAppKeyValue( "GetByCTIDsEndpoint" );
			var registryAuthorizationToken = Utilities.UtilityManager.GetAppKeyValue( "CredentialRegistryAuthorizationToken" ); //Admin-level access to the registry for CE's account

			var client = new HttpClient();
			client.DefaultRequestHeaders.TryAddWithoutValidation( "Authorization", "Token " + registryAuthorizationToken );
			var jsonQuery = new JObject()
			{
				{ "ctids", JArray.FromObject( ctids ) }
			};

			var httpResult = client.PostAsync( apiURL, new StringContent( jsonQuery.ToString( Formatting.None ), Encoding.UTF8, "application/json" ) ).Result;
			var httpResultBody = httpResult.Content.ReadAsStringAsync().Result;
			var resultData = JArray.Parse( httpResultBody );

			return resultData;
		}
		//

		public static JObject RunSPARQLQuery( string sparqlQuery, string userAPIKey, int skip, int take, string referrer, bool logRequest = false )
		{
			//Setup the request mechanism
			var client = new HttpClient();
			var consumeRequest = new AccountConsumeRequest()
			{
				ShouldLogRequest = logRequest,
				SPARQLQuery = sparqlQuery,
				Skip = skip,
				Take = take
			};

			//Add the referrer if present
			try
			{
				if ( referrer != null )
				{
					client.DefaultRequestHeaders.Referrer = new Uri( referrer );
				}
			}
			catch { }

			//If the API key is required, go through the accounts system
			if ( Utilities.UtilityManager.GetAppKeyValue( "requiringHeaderToken" ) == "true" )
			{
				consumeRequest.ApiKey = userAPIKey;

				//Run the query
				var searchAPIURL = Utilities.UtilityManager.GetAppKeyValue( "accountsSPARQLSearchApi" );
				var httpResult = client.PostAsync( searchAPIURL, new StringContent( JsonConvert.SerializeObject( consumeRequest ), Encoding.UTF8, "application/json" ) ).Result;
				var httpResultBody = httpResult.Content.ReadAsStringAsync().Result;
				var resultData = JObject.Parse( httpResultBody );
				return resultData;
			}
			//Otherwise, query the registry directly
			else
			{
				var searchAPIURL = Utilities.UtilityManager.GetAppKeyValue( "SPARQLSearchEndpoint" );
				var registryAuthorizationToken = Utilities.UtilityManager.GetAppKeyValue( "CredentialRegistryAuthorizationToken" ); //Admin-level access to the registry for CE's account

				client.DefaultRequestHeaders.TryAddWithoutValidation( "Authorization", "Token " + registryAuthorizationToken );
				var jsonQuery = new JObject()
				{
					{ "query", consumeRequest.SPARQLQuery }
				};

				var httpResult = client.PostAsync( searchAPIURL, new StringContent( jsonQuery.ToString( Formatting.None ), Encoding.UTF8, "application/json" ) ).Result;
				var httpResultBody = httpResult.Content.ReadAsStringAsync().Result;
				var resultData = JObject.Parse( httpResultBody );

				return resultData;
			}
		}
		//

		public enum TextMatchType { StartsWith, EndsWith, Contains, ExactMatch, ExactPartialMatch }
		public static string WrapTextInRegex( string variableName, string text, TextMatchType matchType = TextMatchType.Contains, bool isCaseSensitive = false, bool wrapExactMatchInQuotes = true, bool useStringification = true )
		{
			var escapeSpecialCharacters = new List<string>() { "!", "@", "%", "&", "[", "^", "$", "|", "?", "+", "(", ")", "{", "}", "/", "*", "'" };
			var insertText = ( text ?? "" ).Replace( "\\", "." ).Replace( ".", "\\." ).Replace( "(", "\\(" ).Replace( ")", "\\)" );
			foreach ( var character in escapeSpecialCharacters )
			{
				insertText = insertText.Replace( character, "." ); //Replace special characters with .
			}
			var exactMatchText = insertText.Replace( "\"", "" );
			var exactPartialMatchText = string.Join( "|", exactMatchText.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries ).Where( m => m.Length > 2 ).ToList() );
			var regexedText = RegexifyString( insertText );
			var caseSensitiveText = isCaseSensitive ? "" : ", 'i'";

			//Temporary override to force this to output something that matches the gremlin query
			//regexedText = Regex.Replace( insertText, "(?:y|e|s|es|ies|er|ing|ement)(\\s|$)", "(?:y|e|s|es|ies|er|ing|ement)$1", RegexOptions.IgnoreCase );

			var variableNameText = useStringification ? "str(" + variableName + ")" : variableName;

			var finalText = "";
			switch ( matchType )
			{
				case TextMatchType.StartsWith:
				finalText = "regex(" + variableNameText + ", '^" + regexedText + "'" + caseSensitiveText + ")"; break;
				case TextMatchType.EndsWith:
				finalText = "regex(" + variableNameText + ", '" + regexedText + "$'" + caseSensitiveText + ")"; break;
				case TextMatchType.Contains:
				finalText = "regex(" + variableNameText + ", '" + ( regexedText.Contains( "(" + exactMatchText + ")" ) ? "" : "(" + exactMatchText + ")|" ) + regexedText + "'" + caseSensitiveText + ")"; break;
				case TextMatchType.ExactMatch:
				finalText = "regex(" + variableNameText + ", '" + exactMatchText + "'" + caseSensitiveText + ")"; break;
				case TextMatchType.ExactPartialMatch:
				finalText = "regex(" + variableNameText + ", '.*" + exactPartialMatchText + ".*'" + caseSensitiveText + ")"; break;
				default:
				finalText = wrapExactMatchInQuotes ? ( "'" + exactMatchText + "'" ) : exactMatchText; break;
			}

			//Remove any trailing slashes and then encode the remaining slashes
			finalText = Regex.Replace( finalText, @"\\$", "", RegexOptions.IgnoreCase ).Replace( "\\", "%5C%5C" );

			return finalText;
		}
		private static string RegexifyString( string text )
		{
			var parts = text.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries );
			var quoteToken = new List<string>();
			var final = new List<string>();

			var inQuotes = false;
			foreach( var part in parts )
			{
				//If the word is in quotes, add it verbatim
				if( part.First() == '"' && part.Last() == '"' )
				{
					final.Add( part.Replace( "\"", "" ) );
				}
				//If we're starting a phrase, handle it
				else if( part.First() == '"' )
				{
					inQuotes = true;
					quoteToken.Add( part.Replace( "\"", "" ) );
				}
				//If we're ending a phrase, handle it
				else if( part.Last() == '"' )
				{
					inQuotes = false;
					quoteToken.Add( part.Replace( "\"", "" ) );
					final.Add( string.Join( " ", quoteToken ) );
					quoteToken = new List<string>();
				}
				//If we're mid-phrase, do not modify anything
				else if ( inQuotes )
				{
					quoteToken.Add( part );
				}
				//Drop really short words unless they are the entire string
				else if( part.Length <= 2 )
				{
					if( parts.Count() == 1 )
					{
						final.Add( part );
					}
					else
					{
						//Drop it
					}
				}
				//Keep short words as-is since we can't remove the outer regex parts and we need to catch acronyms, even if this means also catching short words like "a" and "of"
				//else if( part.Length <= 6 )
				//{
				//	final.Add( part );
				//}
				//Otherwise...
				else
				{
					//Since parts are joined with |, we can check for matches to the direct string as well as the branching matches below
					final.Add( part );

					//Stem known word endings
					//var modified = Regex.Replace( part, "(?:y|e|s|es|ies|er|ing|ement)(\\s|$)", "(?:y|e|s|es|ies|er|ing|ement)$1", RegexOptions.IgnoreCase );
					var modified = Regex.Replace( part, "(?:y|e|s|es|ies|er|ing|ement)(\\s|$)", "$1", RegexOptions.IgnoreCase );
					if ( modified.Contains( "(?:" ) )
					{
						final.Add( modified );
					}
					//For other words, trim the last few characters depending on the length of the word
					else
					{
						if( part.Length >= 5 )
						{
							var trim = ( int ) Math.Ceiling( ( double ) ( ( part.Length / 2 ) - ( part.Length / 5 ) ) );
							trim = trim > 5 ? 5 : trim;
							var leaveAlone = part.Length - trim;
							var finalPart = Regex.Replace( part.Substring( 0, leaveAlone ), @"\\$", "", RegexOptions.IgnoreCase );
							final.Add( finalPart );
						}
					}
				}
			}

			//Handle stray quotes
			if( quoteToken.Count() > 0 )
			{
				final.Add( string.Join( " ", quoteToken ) );
			}

			return "(" + string.Join( ")|(", final.Distinct() ).Replace( "\"", "" ) + ")"; //Ensure any stray " are filtered out
		}
		//

		public static List<TokenResult> Tokenize_LangString( List<string> text )
		{
			var resultSet = new List<TokenResult>();

			foreach( var item in text )
			{
				var result = new TokenResult();
				var normalized = Regex.Replace( item.Trim().ToLower(), "[^a-z0-9 ]", " " ); //Remove any unwanted characters
				normalized = Regex.Replace( normalized, " +", " " ); //Remove any double spaces caused by the previous line

				result.NormalizedFullText = normalized;
				result.MainTokens = normalized.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries ).Distinct().ToList();
				result.PhraseRegex = string.Join( " ", normalized.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries ).Select( m => HandlePhraseRegexWord( m ) ).ToList() ); //Join with a space
				result.PhraseRegex = Regex.Replace( result.PhraseRegex, @"\.\{0,4\}$", "" ); //Strip off any trailing .{0,4} as it is not necessary for the last word
				result.UniqueCharacters = normalized.Select( m => m ).Distinct().ToList();

				var edgeRegex = "(y|ier|iest|s|er|ing|ed|able|ible)$";
				foreach( var token in result.MainTokens )
				{
					result.StemmedTokens.Add( Regex.Replace( token, edgeRegex, "" ) );
				}
				result.StemmedTokens = result.StemmedTokens.Distinct().ToList();
				result.PartialTokens = GetPartialTokens( result.MainTokens );

				resultSet.Add( result );
			}

			return resultSet;
		}
		private static string HandlePhraseRegexWord( string word )
		{
			var edgeRegex = "(y|ier|iest|s|er|ing|ed|able|ible)$";
			return Regex.Replace( word, edgeRegex, ".{0,4}" ); //If there was a word ending, the .{0,4} allows for stemming that word.
		}
		private static List<string> GetPartialTokens( List<string> source, int minLength = 5 )
		{
			var result = new List<string>();
			minLength = minLength < 1 ? 1 : minLength;

			foreach( var item in source )
			{
				if( item.Length >= minLength )
				{
					var removeCharacters = (item.Length - minLength) > 3 ? 3 : item.Length - minLength;
					var truncated = item.Substring( 0, item.Length - removeCharacters );
					if( truncated.Length >= 4 )
					{
						truncated = truncated.Substring( 1 );
					}
					result.Add( truncated );
				}
				else
				{
					result.Add( item );
				}
			}

			return result.Distinct().ToList();
		}
		//

		public static List<TokenResult> Tokenize_URIString( List<string> text )
		{
			var resultSet = new List<TokenResult>();

			foreach ( var item in text )
			{
				var result = new TokenResult();
				var normalized = item.Trim().ToLower();
				normalized = Regex.Replace( normalized, @"^http:\/\/", "" );
				normalized = Regex.Replace( normalized, @"^https:\/\/", "" );
				normalized = Regex.Replace( normalized, @"[^a-z0-9_\.\/\?\=\-\:\%]", " " );
				var escaped = normalized;
				var specialURLCharacters = new List<string>() { ".", "/", "?", "=", ":", ":", "%" };
				foreach(var character in specialURLCharacters )
				{
					escaped = escaped.Replace( character, @"\" + character );
				}

				result.NormalizedFullText = escaped.Replace( " ", "" );
				result.MainTokens = escaped.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries ).Distinct().ToList();

				var rawPartialTokens = GetPartialTokens( normalized.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries ).Distinct().ToList() );
				var escapedPartialTokens = new List<string>();
				foreach( var partialToken in rawPartialTokens )
				{
					var escapedPartialToken = partialToken;
					foreach( var character in specialURLCharacters )
					{
						escapedPartialToken = escapedPartialToken.Replace( character, @"\" + character );
					}
					escapedPartialTokens.Add( escapedPartialToken );
				}
				result.PartialTokens = escapedPartialTokens;

				result.PhraseRegex = "(" + string.Join( ")|(", result.PartialTokens ) + ")"; //Join with a |

				resultSet.Add( result );
			}

			return resultSet;
		}
		//

		public static List<TokenResult> Tokenize_PlainString( List<string> text )
		{
			var resultSet = new List<TokenResult>();

			foreach ( var item in text )
			{
				var result = new TokenResult();
				var normalized = Regex.Replace( item.Trim().ToLower(), "[^a-z0-9 ]", " " );

				result.NormalizedFullText = normalized;
				result.MainTokens = normalized.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries ).Distinct().ToList();
				result.PartialTokens = GetPartialTokens( result.MainTokens );

				resultSet.Add( result );
			}

			return resultSet;
		}
		//

		#region Support Classes

		public class TokenResult
		{
			public TokenResult()
			{
				MainTokens = new List<string>();
				PartialTokens = new List<string>();
				StemmedTokens = new List<string>();
				UniqueCharacters = new List<char>();
			}
			public string NormalizedFullText { get; set; }
			public string PhraseRegex { get; set; }
			public List<string> MainTokens { get; set; }
			public List<string> PartialTokens { get; set; }
			public List<string> StemmedTokens { get; set; }
			public List<char> UniqueCharacters { get; set; }
		}

		public class SPARQLNode
		{
			public SPARQLNode() 
			{
				ObjectValues = new List<string>();
				ValueWrapperType = ValueWrapperTypes.None;
				Children = new List<SPARQLNode>();
				SPARQLNodeType = SPARQLNodeTypes.Normal;
				PredicateDirection = PredicateDirections.Both;
			}
			public SPARQLNode( string predicateProperty, List<string> objectValues, ValueWrapperTypes valueWrapperType = ValueWrapperTypes.None, string subjectVariable = null, string objectVariable = null )
			{
				SubjectVariable = subjectVariable ?? GenerateVariable();
				ObjectVariable = objectVariable ?? GenerateVariable();
				ObjectValues = objectValues ?? new List<string>();
				ValueWrapperType = ValueWrapperTypes.None;
				PredicateProperty = predicateProperty;
				Children = new List<SPARQLNode>();
				SPARQLNodeType = SPARQLNodeTypes.Normal;
				PredicateDirection = PredicateDirections.Both;
			}
			public static SPARQLNode CreateObjectWrapper( string subjectVariable, string predicateProperty, string objectVariable = null, bool joinChildrenWithUNION = false )
			{
				var node = new SPARQLNode()
				{
					SubjectVariable = subjectVariable ?? GenerateVariable(),
					ObjectVariable = objectVariable ?? GenerateVariable(),
					PredicateProperty = predicateProperty,
					JoinChildrenWithUNION = joinChildrenWithUNION,
					SPARQLNodeType = SPARQLNodeTypes.ObjectWrapper,
					PredicateDirection = PredicateDirections.Both
				};
				return node;
			}
			public static string GenerateVariable()
			{
				return "?" + Guid.NewGuid().ToString().Replace( "-", "" );
			}

			public SPARQLNode Parent { get; set; }
			public enum ValueWrapperTypes { None, SurroundWithSingleQuotes, SurroundWithAngleBrackets }
			public enum SPARQLNodeTypes { HighestNode, Normal, JSONLD_ID, JSONLD_Type, LanguageMap_Node, LanguageMap_LanguageValuePair, LanguageMap_Value, LanguageMap_Language, ObjectWrapper, TermGroup, NotGroup, SearchAnyValue, URINode, ConceptURINode, StringNode, DateNode, DateTimeNode, LanguageBCP47Node, CTIDNode, SortOrderNode, LanguageMap_Node_WithRelevance, StringNode_WithRelevance, URINode_WithRelevance, IntegerNode, FloatNode, DecimalNode }
			public enum PredicateDirections { Both, Outbound, Inbound }
			public ValueWrapperTypes ValueWrapperType { get; set; }
			public List<string> ObjectValues { get; set; }
			public string SubjectVariable { get; set; }
			public string PredicateProperty { get; set; }
			public string ObjectVariable { get; set; }
			public List<SPARQLNode> Children { get; set; }
			public bool JoinChildrenWithUNION { get; set; }
			public SPARQLNodeTypes SPARQLNodeType { get; set; }
			public PredicateDirections PredicateDirection { get; set; }

			public void AddChild( SPARQLNode child )
			{
				Children.Add( child );
				child.Parent = this;
				child.SubjectVariable = this.ObjectVariable;
			}
			public static string JoinValues( List<string> values, ValueWrapperTypes valueWrapperType = ValueWrapperTypes.None, string injectFakeItem = null )
			{
				if ( values == null || values.Count() == 0 )
				{
					return "";
				}

				//For some reason, Neptune's SPARQL handler takes measurably longer to process VALUES arrays that are one item long than it does to process arrays that are two or more items long
				if( !string.IsNullOrWhiteSpace( injectFakeItem ) && values.Count() == 1 )
				{
					values.Add( injectFakeItem );
				}

				switch ( valueWrapperType )
				{
					case ValueWrapperTypes.None: return string.Join( " ", values );
					case ValueWrapperTypes.SurroundWithSingleQuotes: return "'" + string.Join( "' '", values ) + "'";
					case ValueWrapperTypes.SurroundWithAngleBrackets: return "<" + string.Join( "> <", values ) + ">";
					default: return "";
				}
			}

			public override string ToString()
			{
				return ToString( new Metadata(), TextMatchType.Contains, false, false );
			}
			public string ToString( Metadata metadata )
			{
				return ToString( metadata, TextMatchType.Contains, false, false );
			}
			public string ToString( Metadata metadata, TextMatchType textMatchType, bool isCaseSensitive, bool stringifyText )
			{
				var neptuneFullTextSearchEndpoint = Utilities.ConfigHelper.GetConfigValue( "NeptuneAWSSPARQLElasticSearchIntegrationEndpoint", "" );
				PredicateProperty = PredicateProperty == null ? null : PredicateProperty.Replace( "<", "" ).Replace( ">", "" ); //Just in case
				if( SPARQLNodeType == SPARQLNodeTypes.HighestNode )
				{
					return "{ " + string.Join( JoinChildrenWithUNION ? " UNION " : " ", Children.Select( m => m.ToString( metadata ) ) ) + " }";
				}
				//Elasticsearch integration
				else if ( false && ( SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Node || SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Node_WithRelevance ) )
				{
					var resultItems = new List<string>();

					foreach(var pair in Children.Where(m => m.SPARQLNodeType == SPARQLNodeTypes.LanguageMap_LanguageValuePair ).ToList() )
					{
						//Use a unique ?tokens variable to avoid scoping problems
						var tokensVar = "?tokens_" + Guid.NewGuid().ToString().Replace( "-", "" );
						//Get the list of language tags
						var onlyForLanguages = pair.Children.Where( m => m.SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Language ).SelectMany( m => m.ObjectValues ).ToList();
						//Get the list of text values
						var childrenValues = pair.Children.Where( m => m.SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Value ).SelectMany( m => m.ObjectValues ).ToList();

						//If there are any language tags, insert them
						var languagePart = "";
						if ( onlyForLanguages.Count() > 0 )
						{
							languagePart = " VALUES ?language { " + string.Join( " ", onlyForLanguages.Select( m => "'" + m + "'" ).ToList() ) + " } " + tokensVar + " ( credreg:__tokenLanguage ) ?language . ";
						}

						//For each text value in the list of text values (will usually only be one item in this list)
						var textPart = "";
						if ( childrenValues.Count() > 0 )
						{
							//Get the tokens for this value
							var tokens = Tokenize_LangString( childrenValues );
							textPart = "";
							var textPartItems = new List<string>();

							//Generate the elasticsearch query
							foreach ( var token in tokens )
							{
								textPartItems.Add(
									"{ " +
										"SERVICE neptune-fts:search { " +
											"neptune-fts:config neptune-fts:endpoint '" + neptuneFullTextSearchEndpoint + "' . " +
											"neptune-fts:config neptune-fts:queryType 'query_string' . " +
											"neptune-fts:config neptune-fts:field " + PredicateProperty + " . " +
											"neptune-fts:config neptune-fts:query '" + token.NormalizedFullText.Replace( " ", " OR " ) + "' . " +
											"neptune-fts:config neptune-fts:return " + tokensVar + " . " +
										" }" +
									" }"
								);
							}
							textPart += string.Join( " UNION ", textPartItems );
						}

						//Construct the outer part of this part of the query
						resultItems.Add( "{ " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + tokensVar + " . { " + languagePart + " " + textPart + " } }" );
					}

					return "{ " + string.Join( JoinChildrenWithUNION ? " UNION " : " ", resultItems ) + " }";
				}
				//Free text search but with regex
				else if ( SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Node || SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Node_WithRelevance )
				{
					var resultItems = new List<string>();

					foreach( var pair in Children.Where( m => m.SPARQLNodeType == SPARQLNodeTypes.LanguageMap_LanguageValuePair ).ToList() )
					{
						//Use a unique ?tokens variable to avoid scoping problems
						var tokensVar = "?tokens_" + Guid.NewGuid().ToString().Replace( "-", "" );
						//Get the list of language tags
						var onlyForLanguages = pair.Children.Where( m => m.SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Language ).SelectMany( m => m.ObjectValues ).ToList();
						//Get the list of text values
						var childrenValues = pair.Children.Where( m => m.SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Value ).SelectMany( m => m.ObjectValues ).ToList();

						//If there are any language tags, insert them
						var languagePart = "";
						if ( onlyForLanguages.Count() > 0 ) {
							languagePart = " VALUES ?language { " + string.Join( " ", onlyForLanguages.Select( m => "'" + m + "'" ).ToList() ) + " } " + tokensVar + " ( credreg:__tokenLanguage ) ?language . ";
						}

						//For each text value in the list of text values (will usually only be one item in this list)
						var textPart = "";
						if( childrenValues.Count() > 0 )
						{
							//Get the tokens for this value
							var tokens = Tokenize_LangString( childrenValues );
							textPart = "";
							var textPartItems = new List<string>();
							//For each token, add the relevance and matching
							foreach ( var token in tokens )
							{
								var pointsVar = "?relevance_points_" + Guid.NewGuid().ToString().Replace( "-", "" );
								var normalizedVar = "?normalized_" + Guid.NewGuid().ToString().Replace( "-", "" );
								metadata.RelevancePointsVariables.Add( pointsVar );
								textPartItems.Add(
									"{ " +
										tokensVar + " ( credreg:__tokenFullNormalized ) " + normalizedVar + " . " + //This has to be internal to the { } in order to scope the ?normalized variable correctly, otherwise the BINDs and such don't work
										"BIND(" +
											"IF(REGEX(" + normalizedVar + ", '" + string.Join( "|", token.PartialTokens ) + "')," +
												"IF(" +
													"REGEX(" + normalizedVar + ", '" + token.NormalizedFullText + "'), " + 
													( 250 * token.NormalizedFullText.Length ) + ", " + //If there is a perfect match, add a huge bonus
													"IF(" +
														"REGEX(" + normalizedVar + ", '" + token.PhraseRegex + "'), " + 
														( 25 * token.NormalizedFullText.Length ) + ", " + //If there is a rough phrase match, add a big bonus
														string.Join( " + ", token.PartialTokens.Select( m => "IF(REGEX(" + normalizedVar + ", '" + m + "'), " + (m.Length * m.Length) + ", 0)" ).ToList() ) + //If there is only a token match, add a bonus based on each matching token's size
													")" +
												")" +
											", 0)" +
											" AS " + pointsVar +
										")" +
										" FILTER(" + pointsVar + " > 0)" +
										//" BIND(" + pointsVar + " + ?relevance_points AS ?relevance_points)" +
									" }"
								);
							}
							textPart += string.Join( " UNION ", textPartItems );
						}

						//Construct the outer part of this part of the query
						resultItems.Add( "{ " + Parent.ObjectVariable + " ( " + PredicateProperty + "__tokenData ) " + tokensVar + " . { " + languagePart + " " + textPart + " } }" );
					}

					return "{ " + string.Join( JoinChildrenWithUNION ? " UNION " : " ", resultItems ) + " }";
				}
				else if ( SPARQLNodeType == SPARQLNodeTypes.LanguageMap_LanguageValuePair || SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Value || SPARQLNodeType == SPARQLNodeTypes.LanguageMap_Language )
				{
					return ""; //Should not happen
				}
				else if ( SPARQLNodeType == SPARQLNodeTypes.DateNode || SPARQLNodeType == SPARQLNodeTypes.DateTimeNode )
				{
					var minDate = ObjectValues.First();
					var maxDate = ObjectValues.Last();
					var dataTypeTag = SPARQLNodeType == SPARQLNodeTypes.DateTimeNode ? "^^xsd:dateTime" : "^^xsd:date";
					var hasMinDate = !string.IsNullOrWhiteSpace( minDate );
					var hasMaxDate = !string.IsNullOrWhiteSpace( maxDate );
					if ( hasMinDate || hasMaxDate )
					{
						var minText = hasMinDate ? ObjectVariable + " >= '" + minDate + "'" + dataTypeTag : "";
						var maxText = hasMaxDate ? ObjectVariable + " <= '" + minDate + "'" + dataTypeTag : "";

						//Handle custom search properties for the record itself
						if( PredicateProperty.ToLower() == "search:datecreated" )
						{
							PredicateProperty = "credreg:__graph? / credreg:__createdAt";
						}
						else if(PredicateProperty.ToLower() == "search:dateupdated" )
						{
							PredicateProperty = "credreg:__graph? / credreg:__updatedAt";
						}

						//return "{ " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . FILTER( " + minText + ( hasMinDate && hasMaxDate ? " %26%26 " : "" ) + maxText + " ) . }";
						return "{ " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . FILTER( " + minText + ( hasMinDate && hasMaxDate ? " && " : "" ) + maxText + " ) . }";
					}
					else
					{
						return "";
					}
				}
				else if( SPARQLNodeType == SPARQLNodeTypes.SortOrderNode )
				{
					var orderByVariable = PredicateProperty.ToLower() == "search:orderbydescending" ? "?orderByMeDescending" : "?orderByMeAscending";
					var orderByProperty = ObjectValues.FirstOrDefault();
					if( orderByProperty.ToLower() == "search:datecreated" )
					{
						return "{ " + Parent.ObjectVariable + " ( credreg:__graph? / credreg:__createdAt ) " + orderByVariable + " . }";
					}
					else if ( orderByProperty.ToLower() == "search:dateupdated" )
					{
						return "{ " + Parent.ObjectVariable + " ( credreg:__graph? / credreg:__updatedAt ) " + orderByVariable + " . }";
					}
					else
					{
						return "{ " + Parent.ObjectVariable + " " + ( new SPARQLServices().SchemaContext.LanguageMapProperties.Contains( orderByProperty ) ? orderByProperty + "__plaintext" : orderByProperty ) + " " + orderByVariable + " . }";
					}
				}
				else if ( SPARQLNodeType == SPARQLNodeTypes.JSONLD_ID )
				{
					var exactURLs = ObjectValues.Where( m => m.IndexOf( "http" ) == 0 ).ToList();
					var partialURLs = ObjectValues.Where( m => m.IndexOf( "http" ) != 0 ).ToList();
					var tokens = Tokenize_URIString( partialURLs );

					var exactMatchQuery = "{ VALUES " + Parent.ObjectVariable + " { " + JoinValues( ObjectValues, ValueWrapperTypes.SurroundWithAngleBrackets ) + " } " + Parent.ObjectVariable + " ?p ?o . }";
					var partialMatchQuery = "{ " + Parent.ObjectVariable + " ?p ?o FILTER( " + string.Join( JoinChildrenWithUNION ? " || " : " && ", tokens.Select( m => "REGEX(LCASE(STR(" + Parent.ObjectVariable + ")), '" + m.NormalizedFullText + "')" ).ToList() ) + " ) . }";
					return "{ " + ( exactURLs.Count() > 0 ? exactMatchQuery : "" ) + ( exactURLs.Count() > 0 && partialURLs.Count() > 0 ? " UNION " : "" ) + ( partialURLs.Count() > 0 ? partialMatchQuery : "" ) + " }";
					/*
					return 
						"{" +
							"{ VALUES " + Parent.ObjectVariable + " { " + JoinValues( ObjectValues, ValueWrapperTypes.SurroundWithAngleBrackets, "credreg:__null" ) + " } " + Parent.ObjectVariable + " ?p ?o . }" +
							" UNION " +
							"{ " + Parent.ObjectVariable + " ?p ?o FILTER( " + string.Join( JoinChildrenWithUNION ? " || " : " && ", tokens.Select( m => "REGEX(LCASE(STR(" + Parent.ObjectVariable + ")), '" + m.PhraseRegex + "')" ).ToList() ) + " ) . }" +
						"}";
					*/
					//return "{ VALUES " + Parent.ObjectVariable + " { " + JoinValues( ObjectValues, ValueWrapperTypes.SurroundWithAngleBrackets, "credreg:__null" ) + " } " + Parent.ObjectVariable + " ?p ?o . }";
				}
				else if( SPARQLNodeType == SPARQLNodeTypes.JSONLD_Type )
				{
					return "{ VALUES " + ObjectVariable + " { " + JoinValues( ObjectValues, ValueWrapperType, "credreg:__null" ) + " } " + Parent.ObjectVariable + " a " + ObjectVariable + " . }";
				}
				else if( SPARQLNodeType == SPARQLNodeTypes.TermGroup )
				{
					return "{ " + string.Join( JoinChildrenWithUNION ? " UNION " : " ", Children.Select( m => m.ToString( metadata ) ).ToList() ) + " }";
				}
				else if( SPARQLNodeType == SPARQLNodeTypes.NotGroup )
				{
					return "FILTER NOT EXISTS { " + string.Join( JoinChildrenWithUNION ? " UNION " : " ", Children.Select( m => m.ToString( metadata ) ).ToList() ) + " }";
				}
				else if( SPARQLNodeType == SPARQLNodeTypes.SearchAnyValue )
				{
					return "{ " + Parent.ObjectVariable + " " + HandleDirectionalPredicate( PredicateProperty, PredicateDirection ) + " ?anyValue_" + Guid.NewGuid().ToString().Replace( "-", "" ) + " ." + " }";
				}
				else if ( SPARQLNodeType == SPARQLNodeTypes.URINode || SPARQLNodeType == SPARQLNodeTypes.URINode_WithRelevance )
				{
					//Use a unique ?tokens variable to avoid scoping problems
					var tokensVar = "?tokens_" + Guid.NewGuid().ToString().Replace( "-", "" );

					//Get the tokens for this value
					var tokens = Tokenize_URIString( ObjectValues );
					var textPart = "";
					var textPartItems = new List<string>();

					//For each token, add the relevance and matching
					foreach ( var token in tokens )
					{
						var pointsVar = "?relevance_points_" + Guid.NewGuid().ToString().Replace( "-", "" );
						var normalizedVar = "?normalized_" + Guid.NewGuid().ToString().Replace( "-", "" );
						metadata.RelevancePointsVariables.Add( pointsVar );
						textPartItems.Add(
							"{ " +
								tokensVar + " ( credreg:__tokenFullNormalized ) " + normalizedVar + " . " + //This has to be internal to the { } in order to scope the ?normalized variable correctly, otherwise the BINDs and such don't work
								"BIND(" +
									"IF(REGEX(" + normalizedVar + ", '" + token.PhraseRegex + "', 'i')," +
										"IF(" +
											"REGEX(" + normalizedVar + ", '" + token.NormalizedFullText + "', 'i'), " +
											( 250 * token.NormalizedFullText.Length ) + ", " + //If there is a perfect match, add a huge bonus
											"IF(" +
												"REGEX(" + normalizedVar + ", '" + token.PhraseRegex + "', 'i'), " +
												( 25 * token.PhraseRegex.Length ) + ", " + //If there is a rough phrase match, add a big bonus
												string.Join( " + ", token.PartialTokens.Select( m => "IF(REGEX(" + normalizedVar + ", '" + m + "', 'i'), " + ( m.Length * m.Length ) + ", 0)" ).ToList() ) + //If there is only a token match, add a bonus based on each matching token's size
											")" +
										")" +
									", 0)" +
									" AS " + pointsVar +
								")" +
								" FILTER(" + pointsVar + " > 0)" +
								//" BIND(" + pointsVar + " + ?relevance_points AS ?relevance_points)" +
							" }"
						);
					}
					textPart += string.Join( " UNION ", textPartItems );

					return "{ " + Parent.ObjectVariable + " ( " + PredicateProperty + "__tokenData" + " )" + " " + tokensVar + " . { " + textPart + " } }";
				}
				else if ( SPARQLNodeType == SPARQLNodeTypes.LanguageBCP47Node )
				{
					//return " { " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . " + "FILTER ( " + string.Join( JoinChildrenWithUNION ? " || " : " %26%26 ", ObjectValues.Select( m => WrapTextInRegex( ObjectVariable, m, textMatchType, isCaseSensitive, true, true ) ).ToList() ) + " )" + " . }";
					return " { " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . " + "FILTER ( " + string.Join( JoinChildrenWithUNION ? " || " : " && ", ObjectValues.Select( m => WrapTextInRegex( ObjectVariable, m, textMatchType, isCaseSensitive, true, true ) ).ToList() ) + " )" + " . }";
				}
				else if( SPARQLNodeType == SPARQLNodeTypes.CTIDNode )
				{
					var values = ObjectValues.Select( m => Regex.Replace( m.ToLower(), @"[^a-f0-9-]", "" ) ).ToList();
					var validCTIDs = values.Where( m => m.IndexOf( "ce-" ) == 0 && m.Length == 39 ).ToList();
					if( validCTIDs.Count() == values.Count() ) //Fast matching
					{
						return "{ VALUES " + ObjectVariable + " { " + string.Join( " ", validCTIDs.Select( m => "'" + m + "'" ).ToList() ) + " } " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . }";
					}
					else //REGEX matching
					{
						return " { " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . " + "FILTER ( " + string.Join( " || ", values.Select( m => "REGEX(" + ObjectVariable + ", '" + m + "')" ).ToList() ) + " )" + " . }";
					}
				}
				else if ( SPARQLNodeType == SPARQLNodeTypes.StringNode || SPARQLNodeType == SPARQLNodeTypes.StringNode_WithRelevance )
				{
					//Use a unique ?tokens variable to avoid scoping problems
					var tokensVar = "?tokens_" + Guid.NewGuid().ToString().Replace( "-", "" );
					//Get the tokens for this value
					var tokens = Tokenize_PlainString( ObjectValues );
					var textPart = "";
					var textPartItems = new List<string>();

					//For each token, add the relevance and matching
					foreach ( var token in tokens )
					{
						var pointsVar = "?relevance_points_" + Guid.NewGuid().ToString().Replace( "-", "" );
						var normalizedVar = "?normalized_" + Guid.NewGuid().ToString().Replace( "-", "" );
						metadata.RelevancePointsVariables.Add( pointsVar );
						textPartItems.Add(
							"{ " +
								tokensVar + " ( credreg:__tokenFullNormalized ) " + normalizedVar + " . " + //This has to be internal to the { } in order to scope the ?normalized variable correctly, otherwise the BINDs and such don't work
								"BIND(" +
									"IF(REGEX(" + normalizedVar + ", '" + string.Join( "|", token.PartialTokens ) + "')," +
										"IF(" +
											"REGEX(" + normalizedVar + ", '" + token.NormalizedFullText + "'), " +
											( 250 * token.NormalizedFullText.Length ) + ", " + //If there is a perfect match, add a huge bonus
											"IF(" +
												"REGEX(" + normalizedVar + ", '" + token.PhraseRegex + "'), " +
												( 25 * token.NormalizedFullText.Length ) + ", " + //If there is a rough phrase match, add a big bonus
												string.Join( " + ", token.PartialTokens.Select( m => "IF(REGEX(" + normalizedVar + ", '" + m + "'), " + ( m.Length * m.Length ) + ", 0)" ).ToList() ) + //If there is only a token match, add a bonus based on each matching token's size
											")" +
										")" +
									", 0)" +
									" AS " + pointsVar +
								")" +
								" FILTER(" + pointsVar + " > 0)" +
								//" BIND(" + pointsVar + " + ?relevance_points AS ?relevance_points)" +
							" }"
						);
					}
					textPart += string.Join( " UNION ", textPartItems );

					return "{ " + Parent.ObjectVariable + " ( " + PredicateProperty + "__tokenData" + " )" + " " + tokensVar + " . { " + textPart + " } }";
				}
				else if ( Children == null || Children.Count() == 0 )
				{
					var dataTypeTag =
						SPARQLNodeType == SPARQLNodeTypes.IntegerNode ? "^^xsd:integer" :
						SPARQLNodeType == SPARQLNodeTypes.FloatNode ? "^^xsd:float" :
						SPARQLNodeType == SPARQLNodeTypes.DecimalNode ? "^^xsd:decimal" :
						"";

					//Range values
					if( ObjectValues.Count() > 1 )
					{
						var minValue = ObjectValues.First();
						var maxValue = ObjectValues.Last();
						var hasMinValue = !string.IsNullOrWhiteSpace( minValue );
						var hasMaxValue = !string.IsNullOrWhiteSpace( maxValue );
						if ( hasMinValue || hasMaxValue )
						{

							var minText = hasMinValue ? ObjectVariable + " >= '" + minValue + "'" + dataTypeTag : "";
							var maxText = hasMaxValue ? ObjectVariable + " <= '" + maxValue + "'" + dataTypeTag : "";

							return "{ " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . FILTER( " + minText + ( hasMinValue && hasMaxValue ? " && " : "" ) + maxText + " ) . }";
						}
					}

					//Simple values
					return "{ VALUES " + ObjectVariable + " { " + JoinValues( ObjectValues, ValueWrapperType, "credreg:__null" ) + " } " + Parent.ObjectVariable + " ( " + PredicateProperty + " ) " + ObjectVariable + " . }";

					/*
					//Special handling for these number data types
					if ( SPARQLNodeType == SPARQLNodeTypes.IntegerNode )
					{
						ObjectValues = ObjectValues.Select( m => "'" + m + "'^^xsd:integer" ).ToList();
					}
					else if ( SPARQLNodeType == SPARQLNodeTypes.FloatNode )
					{
						ObjectValues = ObjectValues.Select( m => "'" + m + "'^^xsd:float" ).ToList();
					}
					else if ( SPARQLNodeType == SPARQLNodeTypes.DecimalNode )
					{
						ObjectValues = ObjectValues.Select( m => "'" + m + "'^^xsd:decimal" ).ToList();
					}

					return "{ VALUES " + ObjectVariable + " { " + JoinValues( ObjectValues, ValueWrapperType, "credreg:__null" ) + " } " + Parent.ObjectVariable + " ( " + PredicateProperty + " | ^" + PredicateProperty + " ) " + ObjectVariable + " . }";
					*/
				}
				else if ( !string.IsNullOrWhiteSpace( Parent.ObjectVariable ) && !string.IsNullOrWhiteSpace( PredicateProperty ) && !string.IsNullOrWhiteSpace( ObjectVariable ) )
				{
					return "{ " + Parent.ObjectVariable + " " + HandleDirectionalPredicate( PredicateProperty, PredicateDirection ) + " " + ObjectVariable + " . " + string.Join( JoinChildrenWithUNION ? " UNION " : " ", Children.Select( m => m.ToString( metadata ) ) ) + " }";
				}
				else
				{
					return "{ " + string.Join( JoinChildrenWithUNION ? " UNION " : " ", Children.Select( m => m.ToString( metadata ) ).ToList() ) + " }";
				}

			}

			public class Metadata
			{
				public Metadata()
				{
					RelevancePointsVariables = new List<string>();
				}
				public List<string> RelevancePointsVariables { get; set; }
			}
		}
		//Enable explicit directionality
		public static string HandleDirectionalPredicate( string predicateProperty, SPARQLNode.PredicateDirections direction, bool includeGraphLinks = true )
		{
			if ( !string.IsNullOrWhiteSpace( predicateProperty ) )
			{
				var forwardString = predicateProperty + ( includeGraphLinks ? "/^credreg:__graph?" : "" );
				var reverseString = ( includeGraphLinks ? "credreg:__graph?/" : "" ) + "^" + predicateProperty;
				if ( direction == SPARQLNode.PredicateDirections.Outbound )
				{
					return "( " + forwardString + " )";
				}
				else if( direction == SPARQLNode.PredicateDirections.Inbound )
				{
					return "( " + reverseString + " )";
				}
				else
				{
					return "( (" + forwardString + ") | (" + reverseString + ") )";
				}
			}
			return "";
		}
		//

		public class SPARQLRequest
		{
			public SPARQLRequest()
			{
				OrderBy = SPARQLServices.SortOrders.DEFAULT;
				DescriptionSetType = SPARQLServices.DescriptionSetType.Resource_Graph;
			}
			public JObject Query { get; set; }
			public int Skip { get; set; }
			public int Take { get; set; }
			public SPARQLServices.SortOrders OrderBy { get; set; }
			public bool OrderDescending { get; set; }
			public bool IncludeDebugInfo { get; set; }
			[JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
			public SPARQLServices.DescriptionSetType DescriptionSetType { get; set; } //Optional
			public int DescriptionSetRelatedItemsLimit { get; set; }
			public int DescriptionSetRelatedURIsLimit { get; set; }
		}
		//

		public class SPARQLResultSet
		{
			public SPARQLResultSet()
			{
				SearchResults = new List<JObject>();
				RelatedItemsData = new RelatedItemsData();
				DebugInfo = new JObject();
			}
			public List<JObject> SearchResults { get; set; }
			public int TotalResults { get; set; }
			public RelatedItemsData RelatedItemsData { get; set; }
			public JObject DebugInfo { get; set; }
			public string Error { get; set; }
		}
		//

		public class RelatedItemsData
		{
			public RelatedItemsData()
			{
				ResultURIs = new List<string>();
				RelatedItemsMap = new List<RelatedItemsSet>();
				RelatedItems = new List<JObject>();
				TemporaryRelatedGraphItems = new Dictionary<string, List<JObject>>();
				DebugInfo = new JObject();
			}
			public List<string> ResultURIs { get; set; }
			public List<RelatedItemsSet> RelatedItemsMap { get; set; }
			public List<JObject> RelatedItems { get; set; }
			public Dictionary<string, List<JObject>> TemporaryRelatedGraphItems { get; set; }
			public int RelatedItemsLimit { get; set; }
			public int RelatedURIsLimit { get; set; }
			public JObject DebugInfo { get; set; }
			public bool IncludeDebugInfo { get; set; }
		}
		//

		public class RelatedItemsSet
		{
			public RelatedItemsSet()
			{
				RelatedItems = new List<RelatedItemsPath>();
				DebugInfo = new JObject();
			}
			public string ResourceURI { get; set; }
			public List<RelatedItemsPath> RelatedItems { get; set; }
			public JObject DebugInfo { get; set; }
		}
		//

		public class RelatedItemsPath
		{
			public RelatedItemsPath()
			{
				URIs = new List<string>();
			}
			public string Path { get; set; }
			public List<string> URIs { get; set; }
			public int TotalURIs { get; set; }
		}
		//

		#endregion

		#region Description Set Queries

		public string PayloadQuery( List<string> allURIs )
		{
			var query = @"
			SELECT DISTINCT ?payload
			WHERE {
				VALUES ?resultURI {
					<" + string.Join("> <", allURIs.Distinct() ) + @">
				}
				?resultURI credreg:__payload ?payload
			}";
			query = GetContextPrefixes(query) + " " + query.Replace("\r\n", " ").Replace("\n", " ").Replace("\t", "");

			return query;
		}

		public DescriptionSetQueryResult DescriptionSetQuery_Credential_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			//Direct credential-to-credential connections
			var credentialConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:isPartOf", "ceterms:hasPart", "ceterms:majorAlignment", "ceterms:minorAlignment", "ceterms:exactAlignment", "ceterms:narrowAlignment", "ceterms:broadAlignment", "^ceterms:isPartOf", "^ceterms:hasPart", "^ceterms:majorAlignment", "^ceterms:minorAlignment", "^ceterms:exactAlignment", "^ceterms:narrowAlignment", "^ceterms:broadAlignment" }, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" );
			} );

			//Quality Assurance
			var qualityAssuranceConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:revokedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:revokes" }, delegate ( string property )
			{
				return new DSPStep( property, "ceterms:Agent" );
			} );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLOwnsOffersOrganizationConnections(), "OwnsOffersOrganizationConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, qualityAssuranceConnectionsQuery, "QualityAssuranceConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, credentialConnectionsQuery, "CredentialConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLConditionProfileOutboundConnections(), "ConditionProfileOutboundConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLConditionProfileInboundConnections(), "ConditionProfileInboundConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLManifestConnections(), "ManifestConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All(m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;
		}
		//

		public DescriptionSetQueryResult DescriptionSetQuery_Organization_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			//Direct organization-to-organization connections
			var ctdlOrganizationConnections = new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:accredits", "ceterms:approves", "ceterms:recognizes", "ceterms:regulates", "ceterms:department", "ceterms:subOrganization", "ceterms:parentOrganization", "^ceterms:accreditedBy", "^ceterms:approvedBy", "^ceterms:recognizedBy", "^ceterms:regulatedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:department", "^ceterms:subOrganization", "^ceterms:parentOrganization" };
			var organizationConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlOrganizationConnections, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:Agent", true, "?search_:_ignore_:_AgentType" );
			} );

			//Connections to other CTDL stuff
			var ctdlConnectionProperties = new List<string>() { "ceterms:owns", "ceterms:offers", "ceterms:accredits", "ceterms:approves", "ceterms:recognizes", "ceterms:regulates", "ceterms:revokes", "^ceterms:ownedBy", "^ceterms:offeredBy", "^ceterms:accreditedBy", "^ceterms:approvedBy", "^ceterms:recognizedBy", "^ceterms:regulatedBy", "^ceterms:revokedBy" };
			var credentialConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" );
			} );
			var assessmentConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:AssessmentProfile" );
			} );
			var learningOpportunityConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:LearningOpportunityProfile" );
			} );

			//Connections to CTDL-ASN stuff
			var ctdlasnConnectionProperties = new List<string>() { "^ceasn:creator", "^ceasn:publisher" };
			var competencyFrameworkConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceasn:CompetencyFramework" );
			} );
			var conceptSchemeConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "skos:ConceptScheme" );
			} );

			var manifestSteps = new List<DSPStep>()
			{
				new DSPStep( "ceterms:hasCostManifest", "ceterms:CostManifest" ),
				new DSPStep( "ceterms:hasConditionManifest", "ceterms:ConditionManifest" ),
				new DSPStep( "^ceterms:costManifestOf", "ceterms:CostManifest" ),
				new DSPStep( "^ceterms:conditionManifestOf", "ceterms:ConditionManifest" )
			};

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, organizationConnectionsQuery, "OrganizationConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, credentialConnectionsQuery, "CredentialConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, assessmentConnectionsQuery, "AssessmentConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, learningOpportunityConnectionsQuery, "LearningOpportunityConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, competencyFrameworkConnectionsQuery, "CompetencyFrameworkConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, conceptSchemeConnectionsQuery, "ConceptSchemeConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, manifestSteps, "ManifestConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;
		}
		//

		public DescriptionSetQueryResult DescriptionSetQuery_AssessmentProfile_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			//Quality Assurance
			var qualityAssuranceConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:revokedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:revokes" }, delegate ( string property )
			{
				return new DSPStep( property, "ceterms:Agent" );
			} );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLOwnsOffersOrganizationConnections(), "OwnsOffersOrganizationConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, qualityAssuranceConnectionsQuery, "QualityAssuranceConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLConditionProfileOutboundConnections(), "ConditionProfileOutboundConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLConditionProfileInboundConnections(), "ConditionProfileInboundConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLManifestConnections(), "ManifestConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;
		}
		//

		public DescriptionSetQueryResult DescriptionSetQuery_LearningOpportunityProfile_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			//Direct credential-to-credential connections
			var learningOpportunityConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:isPartOf", "ceterms:hasPart", "^ceterms:isPartOf", "^ceterms:hasPart" }, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:LearningOpportunityProfile" );
			} );

			//Quality Assurance
			var qualityAssuranceConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:revokedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:revokes" }, delegate ( string property )
			{
				return new DSPStep( property, "ceterms:Agent" );
			} );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLOwnsOffersOrganizationConnections(), "OwnsOffersOrganizationConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, qualityAssuranceConnectionsQuery, "QualityAssuranceConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, learningOpportunityConnectionsQuery, "LearningOpportunityConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLConditionProfileOutboundConnections(), "ConditionProfileOutboundConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLConditionProfileInboundConnections(), "ConditionProfileInboundConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLManifestConnections(), "ManifestConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;
		}
		//

		public DescriptionSetQueryResult DescriptionSetQuery_CompetencyFramework_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			//Framework Alignments
			var frameworkAlignmentProperties = new List<string>() { "ceasn:alignFrom", "ceasn:alignTo", "^ceasn:alignFrom", "^ceasn:alignTo" };
			var frameworkAlignmentQuery = GetDSP_DSPStepTemplateForProperties( frameworkAlignmentProperties, delegate ( string property )
			{
				return new DSPStep( property, "ceasn:CompetencyFramework" );
			} );

			//Framework Concepts
			var frameworkConceptsProperties = new List<string>() { "ceasn:conceptTerm", "ceasn:educationLevelType", "ceasn:publicationStatusType" };
			var frameworkConceptsQuery = GetDSP_DSPStepTemplateForProperties( frameworkConceptsProperties, delegate ( string property )
			{
				return DSPStep_ConceptToConceptSchemeChunk( property );
			} );

			//Competency Steps
			var competencySteps = new List<DSPStep>();

			//Competency Alignments
			var competencyAlignmentProperties = new List<string>() { "ceasn:alignFrom", "ceasn:alignTo", "ceasn:comprisedOf", "ceasn:broadAlignment", "ceasn:narrowAlignment", "ceasn:majorAlignment", "ceasn:minorAlignment", "ceasn:exactAlignment", "ceasn:prerequisiteAlignment", "ceasn:crossSubjectAlignment", "^ceasn:alignFrom", "^ceasn:alignTo", "^ceasn:comprisedOf", "^ceasn:broadAlignment", "^ceasn:narrowAlignment", "^ceasn:majorAlignment", "^ceasn:minorAlignment", "^ceasn:exactAlignment", "^ceasn:prerequisiteAlignment", "^ceasn:crossSubjectAlignment" };
			foreach ( var property in competencyAlignmentProperties )
			{
				competencySteps.Add( DSPStep_CompetencyToCompetencyFrameworkChunk( property ) );
			}

			//Competency Concepts
			var competencyConceptsProperties = new List<string>() { "ceasn:conceptTerm", "ceasn:educationLevelType", "ceasn:complexityLevel" };
			foreach ( var property in competencyConceptsProperties )
			{
				competencySteps.Add( DSPStep_ConceptToConceptSchemeChunk( property ) );
			}

			//Competency Connections
			competencySteps.Add( new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ) { IgnorePath = true, IgnoreTarget = true }.AddChildren( new List<DSPStep>()
			{
				//Assessment that assesses the competency
				new DSPStep( "^ceterms:assesses", "ceterms:AssessmentProfile", false ).AddChildren( new List<DSPStep>()
				{
					//And entities that require(etc) the assessment
					new DSPStep( "^ceterms:targetAssessment", "ceterms:ConditionProfile", false ){ IgnoreTarget = true }.AddChildren(
						DescriptionSetQuery_CompetencyFramework_GetInboundConditionProfileConnections() //Calling this three times ensures we avoid any potential pass-by-reference errors when these things get processed
					)
				//And entities that the assessment is required(etc) for
				}.Concat( DescriptionSetQuery_CompetencyFramework_GetOutboundConditionProfileConnections() ).ToList() ),
				//Learning opportunity that teaches the competency
				new DSPStep( "^ceterms:teaches", "ceterms:LearningOpportunityProfile", false ).AddChildren( new List<DSPStep>()
				{
					//And entities that require(etc) the learning opportunity
					new DSPStep( "^ceterms:targetLearningOpportunity", "ceterms:ConditionProfile", false ){ IgnoreTarget = true }.AddChildren(
						DescriptionSetQuery_CompetencyFramework_GetInboundConditionProfileConnections()
					)
				//And entities that the learning opportunity is required(etc) for
				}.Concat( DescriptionSetQuery_CompetencyFramework_GetOutboundConditionProfileConnections() ).ToList() ),
				//Entities that require(etc) the competency
				new DSPStep( "^ceterms:targetCompetency", "ceterms:ConditionProfile", false ){ IgnoreTarget = true }.AddChildren(
					DescriptionSetQuery_CompetencyFramework_GetInboundConditionProfileConnections()
				)
			} ) );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Get the framework bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, GetDSP_CTDLASNCreatorPublisherOrganizationConnections(), "CreatorPublisherConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, frameworkConceptsQuery, "FrameworkConcepts", apiKey, referrer ) );
			var competencyStep = new DSPStep( "^ceasn:isPartOf", "ceasn:Competency" );
			var competencyReference = QueryDSPStep( uri, null, competencyStep, "IsPartOf", apiKey, referrer );
			references.Add( competencyReference );


			//Divide up the competency bindings and get those in chunks (regardless of whether the other threads are done yet)
			var competencyListReferences = QuerySubDSPStep( uri, "CompetencyStep", apiKey, referrer, competencyStep, competencyReference, competencySteps );

			//Wait for all pieces to be done
			while( !references.All( m => m.IsFinished ) || !competencyListReferences.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).Concat( competencyListReferences.Select( m => m.DebugInfo ) ).ToList() );

			//Merge the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Fix the variable names for competencies and merge them in
			MergeSubDSPStepBindings( finalResult.Bindings, competencyListReferences, competencyStep );

			//Return the merged bindings
			return finalResult;
		}
		private List<DSPStep> DescriptionSetQuery_CompetencyFramework_GetInboundConditionProfileConnections()
		{
			var conditionProfileConnections = new List<string>() { "^ceterms:requires", "^ceterms:recommends", "^ceterms:preparationFrom", "^ceterms:advancedStandingFrom", "^ceterms:isRequiredFor", "^ceterms:isRecommendedFor", "^ceterms:isPreparationFor", "^ceterms:isAdvancedStandingFor", "^ceterms:entryCondition", "^ceterms:corequisite" };
			//var conditionProfileConnections = new List<string>() { "^ceterms:requires", "^ceterms:recommends" }; //Simplified for performance
			var credentialChildren = new List<DSPStep>();
			var assessmentChildren = new List<DSPStep>();
			var learningOpportunityChildren = new List<DSPStep>();
			//var conditionManifestChildren = new List<DSPStep>();
			foreach ( var connection in conditionProfileConnections )
			{
				credentialChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:Credential", false, "?search_:_ignore_:_CredentialType" ) );
				assessmentChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:AssessmentProfile", false ) );
				learningOpportunityChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:LearningOpportunityProfile", false ) );
				//conditionManifestChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:ConditionManifest", false ) );
			}

			return credentialChildren.Concat( assessmentChildren ).Concat( learningOpportunityChildren ).ToList();
		}
		private List<DSPStep> DescriptionSetQuery_CompetencyFramework_GetOutboundConditionProfileConnections()
		{
			var conditionProfileConnections = new List<string>() { "ceterms:isRequiredFor", "ceterms:isRecommendedFor", "ceterms:isPreparationFor", "ceterms:isAdvancedStandingFor" };
			var conditionProfileSteps = new List<DSPStep>();
			foreach ( var connection in conditionProfileConnections )
			{
				conditionProfileSteps.Add( new DSPStep( connection, "ceterms:ConditionProfile", false ) { IgnoreTarget = true }.AddChildren( new List<DSPStep>()
				{
					new DSPStep( "ceterms:targetCredential", "ceterms:Credential" ),
					new DSPStep( "ceterms:targetAssessment", "ceterms:AssessmentProfile" ),
					new DSPStep( "ceterms:targetLearningOpportunity", "ceterms:LearningOpportunityProfile" )
				} ) );
			}
			return conditionProfileSteps;
		}
		//

		public DescriptionSetQueryResult DescriptionSetQuery_ConceptScheme_InChunks( string uri, string apiKey, string referrer )
		{
			//Scheme Concepts
			var schemeConceptsProperties = new List<string>() { "ceasn:conceptTerm", "ceasn:publicationStatusType" };
			var schemeConceptsQuery = GetDSP_DSPStepTemplateForProperties( schemeConceptsProperties, delegate ( string property )
			{
				return DSPStep_ConceptToConceptSchemeChunk( property );
			} );

			//Concept Steps
			var conceptSteps = new List<DSPStep>();

			//Concept Alignments
			var conceptAlignmentProperties = new List<string>() { "skos:broadMatch", "skos:narrowMatch", "skos:exactMatch", "skos:closeMatch", "skos:related", "^skos:broadMatch", "^skos:narrowMatch", "^skos:exactMatch", "^skos:closeMatch", "^skos:related" };
			foreach ( var property in conceptAlignmentProperties )
			{
				conceptSteps.Add( new DSPStep( property, "ceasn:Competency", false ) );
			}

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Get the framework bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, schemeConceptsQuery, "CreatorPublisherConnections", apiKey, referrer ) );
			var conceptStep = new DSPStep( "^skos:inScheme", "skos:Concept" );
			var conceptReference = QueryDSPStep( uri, null, conceptStep, "InScheme", apiKey, referrer );
			references.Add( conceptReference );

			finalResult.DebugInfo[ "QuerySets" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Divide up the concept bindings and get those in chunks (regardless of whether the other threads are done yet)
			var conceptListReferences = QuerySubDSPStep( uri, "ConceptSteps", apiKey, referrer, conceptStep, conceptReference, conceptSteps );

			//Wait for all pieces to be done
			while ( !references.All( m => m.IsFinished ) || !conceptListReferences.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).Concat( conceptListReferences.Select( m => m.DebugInfo ) ).ToList() );

			//Merge the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Fix the variable names for competencies and merge them in
			MergeSubDSPStepBindings( finalResult.Bindings, conceptListReferences, conceptStep );

			//Return the merged bindings
			return finalResult;

		}
		//

		public DescriptionSetQueryResult DescriptionSet_Rating_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			var directReferences = new List<DSPStep>()
			{
				DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
			};

			var occupationAndCredentialQuery = new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
			{
				new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
				{
					DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
				} )
			} );

			var jobAndOccupationalTasksQuery = DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false ).AddChildren( new List<DSPStep>()
			{
				new DSPStep( "navy:hasOccupationalTask", "navy:OccupationalTask", false ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "navy:hasWorkRole", "navy:WorkRole", false ),
					DSPStep_ConceptToConceptSchemeChunk( "navy:hasFunctionalGroup" ),
					DSPStep_ConceptToConceptSchemeChunk( "navy:hasWorkActivity" ),
					DSPStep_ConceptToConceptSchemeChunk( "navy:hasPayGradeType" ),
					DSPStep_ConceptToConceptSchemeChunk( "navy:hasTaskFlagType" ),
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
				} )
			} );

			var maintenanceTasksQuery = new DSPStep( "^navy:hasRating", "navy:MaintenanceTask", false ).AddChildren( new List<DSPStep>()
			{
				new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
				{
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
				} ),
				new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
					} )
				} ),
				new DSPStep( "^navy:hasMaintenanceTask", "navy:System", false ).AddChildren( new List<DSPStep>()
				{
					DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
					new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
					new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
						DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
						new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
					})
				} )
			} );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, directReferences, "DirectReferenceConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, occupationAndCredentialQuery, "OccupationAndCredentialConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, jobAndOccupationalTasksQuery, "JobAndOccupationalTaskConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, maintenanceTasksQuery, "MaintenanceTaskConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;
		}
		//

		public DescriptionSetQueryResult DescriptionSet_Job_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			var ratingQuery = new DSPStep( "navy:hasRating", "navy:Rating" ).AddChildren( new List<DSPStep>()
			{
				DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
				new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
					{
						DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
					} )
				} ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false ),
				new DSPStep( "^navy:hasRating", "navy:MaintenanceTask", false ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
					} ),
					new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
						} )
					} ),
					new DSPStep( "^navy:hasMaintenanceTask", "navy:System", false ).AddChildren( new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
						new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
						new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
							DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
							new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
						})
					} )
				} )
			} );

			var occupationalTaskQuery = new DSPStep( "navy:hasOccupationalTask", "navy:OccupationalTask", false ).AddChildren( new List<DSPStep>()
			{
				new DSPStep( "navy:hasWorkRole", "navy:WorkRole", false ),
				DSPStep_ConceptToConceptSchemeChunk( "navy:hasFunctionalGroup" ),
				DSPStep_ConceptToConceptSchemeChunk( "navy:hasWorkActivity" ),
				DSPStep_ConceptToConceptSchemeChunk( "navy:hasPayGradeType" ),
				DSPStep_ConceptToConceptSchemeChunk( "navy:hasTaskFlagType" ),
				DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
				DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
				DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
			} );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, ratingQuery, "RatingConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, occupationalTaskQuery, "OccupationalTaskConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;
		}
		//

		public DescriptionSetQueryResult DescriptionSet_EnlistedClassification_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			var ratingQuery = new DSPStep( "navy:hasRating", "navy:Rating" ).AddChildren( new List<DSPStep>()
			{
				DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
				new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
					{
						DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
					} )
				} ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
				DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false ).AddChildren( new List<DSPStep>(){
					new DSPStep( "navy:hasOccupationalTask", "navy:OccupationalTask", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "navy:hasWorkRole", "navy:WorkRole", false ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasFunctionalGroup" ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasWorkActivity" ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasPayGradeType" ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasTaskFlagType" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
						} )
				}),
				new DSPStep( "^navy:hasRating", "navy:MaintenanceTask", false ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
					} ),
					new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
						} )
					} ),
					new DSPStep( "^navy:hasMaintenanceTask", "navy:System", false ).AddChildren( new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
						new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
						new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
							DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
							new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
						})
					} )
				} )
			} );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, ratingQuery, "RatingConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;

		}
		//

		public DescriptionSetQueryResult DescriptionSet_System_InChunks( string uri, string apiKey, string referrer )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			var directConnections = new List<DSPStep>()
			{
				DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
				new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
			};

			//Applies if this is the top level system
			var reverseIsPartOfQuery = new DSPStep( "^ceasn:isPartOf", "navy:System" ).AddChildren( new List<DSPStep>()
			{
				DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
				new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
				new DSPStep( "navy:hasMaintenanceTask", "navy:MaintenanceTask" ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
					} ),
					new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
						} )
					} ),
					new DSPStep( "navy:hasRating", "navy:Rating" ).AddChildren(new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
						new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
							{
								DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
							} )
						} ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false )
					} )
				} )
			} );

			//Applies if this is not the lop level system
			var isPartOfQuery = new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
				DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
				new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
			} );

			var isChildOfQuery = new DSPStep( "ceasn:isChildOf", "navy:System" ).AddChildren( new List<DSPStep>()
			{
				DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
				new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
			} );

			//Applies regardless
			var hasChildQuery = new DSPStep( "ceasn:hasChild", "navy:System" ).AddChildren( new List<DSPStep>()
			{
				DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
				new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
			} );

			var maintenanceTaskQuery =	new DSPStep( "navy:hasMaintenanceTask", "navy:MaintenanceTask" ).AddChildren( new List<DSPStep>()
			{
				new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
				{
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
					DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
				} ),
				new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
				{
					new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
					} )
				} ),
				new DSPStep("navy:hasRating", "navy:Rating").AddChildren(new List<DSPStep>()
				{
					DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
					new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
						{
							DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
						} )
					} ),
					DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
					DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
					DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false )
				} )
			} );

			//Hold the result
			var finalResult = new DescriptionSetQueryResult();

			//Handle the bindings
			var references = new List<QueryReference>();
			references.Add( QueryDSPStep( uri, null, directConnections, "DirectConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, reverseIsPartOfQuery, "ReverseIsPartOfConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, isPartOfQuery, "IsPartOfConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, isChildOfQuery, "IsChildOfConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, hasChildQuery, "HasChildConnections", apiKey, referrer ) );
			references.Add( QueryDSPStep( uri, null, maintenanceTaskQuery, "MaintenanceTaskConnections", apiKey, referrer ) );

			//Wait for all the pieces to be done
			while ( !references.All( m => m.IsFinished ) )
			{
				Thread.Sleep( 25 );
			}

			//Track the queries
			finalResult.DebugInfo[ "DescriptionSetQueryParts" ] = JArray.FromObject( references.Select( m => m.DebugInfo ).ToList() );

			//Get the main bindings
			finalResult.Bindings = references.SelectMany( m => m.Bindings ).ToList();

			//Return the data
			return finalResult;
		}
		//

		//DSP Helper Classes and methods

		public class DSPStep
		{
			public DSPStep()
			{
				Children = new List<DSPStep>();
				Lines = new List<DSPStep>();
			}
			public DSPStep( string path, string target, bool withGraph = true )
			{
				Path = path;
				Target = target;
				Children = new List<DSPStep>();
				Lines = new List<DSPStep>();
				WithGraph = withGraph;
			}

			public DSPStep Parent { get; set; }
			public string Path { get; set; }
			public string Target { get; set; }
			public string OverridePath { get; set; }
			public List<DSPStep> Children { get; set; }
			public List<DSPStep> Lines { get; set; }
			public bool IgnorePath { get; set; }
			public bool IgnoreTarget { get; set; }
			public bool Verbatim { get; set; }
			public bool WithGraph { get; set; }
			public List<string> Values { get; set; }
			public string ValuesVariable { get; set; }

			public DSPStep AddChildren( List<DSPStep> children )
			{
				foreach ( var child in children )
				{
					child.Parent = this;
				}
				Children.AddRange( children );
				return this;
			}
			public DSPStep AddLines( List<DSPStep> lines )
			{
				foreach ( var line in lines )
				{
					line.Parent = this;
				}
				Lines.AddRange( lines );
				return this;
			}

			public List<DSPStep> GetParents()
			{
				var result = new List<DSPStep>();
				var temp = Parent;
				while ( temp != null )
				{
					result.Add( temp );
					temp = temp.Parent;
				}
				result.Reverse();
				return result;
			}
			public string GetSelfName( bool includeIgnoredParts )
			{
				if ( includeIgnoredParts )
				{
					return GetSelfPathName() + GetSelfTargetName();
				}
				else
				{
					return ( GetSelfPathName() + GetSelfTargetName()  ).Replace( "ignore_:_", "" );
				}
			}
			public string GetSelfPathName()
			{
				var isReverse = Path[ 0 ] == '^';
				return ( isReverse ? "_<_" : "_>_" ) + ( IgnorePath ? "ignore_:_" : "" ) + Path.Replace( ":", "_:_" ).Replace( "^", "" ).Replace( "?", "_Q_" );
			}
			public string GetSelfTargetName()
			{
				var isReverse = Path[ 0 ] == '^';
				return ( isReverse ? "_<_" : "_>_" ) + ( IgnoreTarget ? "ignore_:_" : "" ) + Target.Replace( ":", "_:_" ).Replace( "?", "_Q_" );
			}
			public string GetSelfFullName()
			{
				if ( Parent != null )
				{
					return string.Join( "", GetParents().Select( m => m.GetSelfName( false ) ).ToList() ) + GetSelfName( true );
				}
				else
				{
					return GetSelfName( true );
				}
			}
			public string Stringify()
			{
				var isReverse = Path[ 0 ] == '^';
				var subjectText = Parent == null ? "resultURI" : Parent.GetSelfFullName();
				var predicateText = string.IsNullOrWhiteSpace( OverridePath ) ? ( WithGraph ? ( isReverse ? "credreg:__graph?/" + Path : Path + "/^credreg:__graph?" ) : Path ) : OverridePath;
				var objectText = GetSelfFullName();
				var linesText = Lines != null && Lines.Count() > 0 ? string.Join( " ", Lines.Select( m => m.Stringify() ) ) : "";
				var childrenText = Children != null && Children.Count() > 0 ? " { { } UNION " + string.Join( " UNION ", Children.Select( m => m.Stringify() ) ) + " } " : "";
				var valuesText = Values != null && Values.Count() > 0 && !string.IsNullOrWhiteSpace( ValuesVariable ) ? " VALUES ?" + ValuesVariable + " { " + string.Join( " ", Values ) + " } " : "";

				if ( Verbatim || Path == "a" )
				{
					return " { " + valuesText + " ?" + subjectText + " " + Path + " " + Target + " } ";
				}
				else
				{
					return " { " + valuesText + " ?" + subjectText + " ( " + predicateText + " ) ?" + objectText + " . " + linesText + " " + childrenText + " } ";
				}
			}
		}

		public DSPStep DSPStep_ConceptToConceptSchemeChunk( string property, bool ignorePath = false, string overridePath = null )
		{
			return new DSPStep( property, "skos:Concept", false ) { IgnorePath = ignorePath, OverridePath = overridePath }.AddChildren( new List<DSPStep>()
			{
				new DSPStep( "skos:inScheme", "skos:ConceptScheme" )/*,
				new DSPStep( "search:autoParents_skos:Concept", "skos:Concept", false ){ OverridePath = "skos:broader*" },
				new DSPStep( "search:autoParents_skos:Concept", "skos:Concept", false ){ OverridePath = "^skos:narrower*" }*/
			} );
		}

		public DSPStep DSPStep_CompetencyToCompetencyFrameworkChunk( string property, bool ignorePath = false, string overridePath = null )
		{
			return new DSPStep( property, "ceasn:Competency", false ) { IgnorePath = ignorePath, OverridePath = overridePath }.AddChildren( new List<DSPStep>()
			{
				new DSPStep( "ceasn:isPartOf", "ceasn:CompetencyFramework" )/*,
				new DSPStep( "search:autoParents_ceasn:Competency", "ceasn:Competency", false ){ OverridePath = "ceasn:isChildOf*" },
				new DSPStep( "search:autoParents_ceasn:Competency", "ceasn:Competency", false ){ OverridePath = "^ceasn:hasChild*" }*/
			} );
		}

		public DSPStep DSPStep_TargetAndTypeChunk( string property, string target, bool withGraph = true, string type = null, bool ignorePath = false, bool ignoreTarget = false, string overridePath = null )
		{
			return new DSPStep( property, target, withGraph ) { IgnorePath = ignorePath, IgnoreTarget = ignoreTarget, OverridePath = overridePath }.AddLines( new List<DSPStep>()
			{
				new DSPStep( "a", string.IsNullOrWhiteSpace( type ) ? target : type )
			} );
		}


		public List<DSPStep> GetDSP_CTDLConditionProfileOutboundConnections()
		{
			var steps = new List<DSPStep>();

			foreach ( var property in new List<string>() { "ceterms:requires", "ceterms:recommends", "ceterms:preparationFrom", "ceterms:advancedStandingFrom", "ceterms:entryCondition", "ceterms:corequisite", "ceterms:isRequiredFor", "ceterms:isRecommendedFor", "ceterms:isPreparationFor", "ceterms:isAdvancedStandingFor" } )
			{
				steps.Add( new DSPStep( property, "ceterms:ConditionProfile", false ) { IgnoreTarget = true }.AddChildren( new List<DSPStep>()
				{
					new DSPStep( "ceterms:targetCredential", "ceterms:Credential" ),
					new DSPStep( "ceterms:targetAssessment", "ceterms:AssessmentProfile" ).AddChildren(new List<DSPStep>()
					{
						new DSPStep( "ceterms:assesses", "ceterms:CredentialAlignmentObject", false ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
						} )
					} ),
					new DSPStep( "ceterms:targetLearningOpportunity", "ceterms:LearningOpportunityProfile" ).AddChildren(new List<DSPStep>()
					{
						new DSPStep( "ceterms:teaches", "ceterms:CredentialAlignmentObject", false ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
						} )
					} ),
					new DSPStep( "ceterms:targetCompetency", "ceterms:CredentialAlignmentObject" ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
					} )
				} ) );
			}

			return steps;
		}
		//

		//Experimental
		//Objective: Get all outbound connections via a variable as the predicate, and track which properties match, then substitute them into the Paths later
		//Maybe competencies would be good for this?
		//Concepts?
		public List<DSPStep> GetDSP_CTDLConditionProfileOutboundConnectionsDynamic()
		{
			var steps = new List<DSPStep>();
			var outboundConnectionProperties = new List<string>() { "ceterms:requires", "ceterms:recommends", "ceterms:preparationFrom", "ceterms:advancedStandingFrom", "ceterms:entryCondition", "ceterms:corequisite", "ceterms:isRequiredFor", "ceterms:isRecommendedFor", "ceterms:isPreparationFor", "ceterms:isAdvancedStandingFor" };

			steps.Add( new DSPStep( "?outboundConnectionProperty", "ceterms:ConditionProfile", false ) { IgnoreTarget = true, Verbatim = true, ValuesVariable = "?outboundConnectionProperty", Values = outboundConnectionProperties }.AddChildren( new List<DSPStep>()
				{
					new DSPStep( "ceterms:targetCredential", "ceterms:Credential" ),
					new DSPStep( "ceterms:targetAssessment", "ceterms:AssessmentProfile" ).AddChildren(new List<DSPStep>()
					{
						new DSPStep( "ceterms:assesses", "ceterms:CredentialAlignmentObject", false ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
						} )
					} ),
					new DSPStep( "ceterms:targetLearningOpportunity", "ceterms:LearningOpportunityProfile" ).AddChildren(new List<DSPStep>()
					{
						new DSPStep( "ceterms:teaches", "ceterms:CredentialAlignmentObject", false ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
						} )
					} ),
					new DSPStep( "ceterms:targetCompetency", "ceterms:CredentialAlignmentObject" ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
					} )
			} ) );

			return steps;
		}
		//

		public List<DSPStep> GetDSP_CTDLConditionProfileInboundConnections()
		{
			var steps = new List<DSPStep>();

			foreach ( var property in new List<string>() { "^ceterms:targetCredential", "^ceterms:targetLearningOpportunity", "^ceterms:targetAssessment" } )
			{
				var connections = new List<string>() { "^ceterms:requires", "^ceterms:recommends", "^ceterms:preparationFrom", "^ceterms:advancedStandingFrom", "^ceterms:isRequiredFor", "^ceterms:isRecommendedFor", "^ceterms:isPreparationFor", "^ceterms:isAdvancedStandingFor", "^ceterms:entryCondition", "^ceterms:corequisite" };
				var credentialChildren = new List<DSPStep>();
				var assessmentChildren = new List<DSPStep>();
				var learningOpportunityChildren = new List<DSPStep>();
				var conditionManifestChildren = new List<DSPStep>();
				foreach ( var connection in connections )
				{
					credentialChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:Credential", false, "?search_:_ignore_:_CredentialType" ) );
					assessmentChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:AssessmentProfile", false ) );
					learningOpportunityChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:LearningOpportunityProfile", false ) );
					conditionManifestChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:ConditionManifest", false ) );
				}

				steps.Add( new DSPStep( property, "ceterms:ConditionProfile" ) { IgnoreTarget = true }.AddChildren(
					credentialChildren.Concat( assessmentChildren ).Concat( learningOpportunityChildren ).Concat( conditionManifestChildren ).ToList()
				) );
			}

			return steps;
		}
		//

		public List<DSPStep> GetDSP_CTDLManifestConnections()
		{
			var steps = new List<DSPStep>();

			steps.Add( new DSPStep( "ceterms:commonCosts", "ceterms:CostManifest" ) );
			steps.Add( new DSPStep( "ceterms:commonConditions", "ceterms:CondtionManifest" ).AddChildren(
				GetDSP_CTDLConditionProfileOutboundConnections()
			) );

			return steps;
		}
		//

		public string GetDSP_CTDLConditionProfileConnections()
		{
			var query = "";

			//Outbound properties
			foreach ( var property in new List<string>() { "ceterms:requires", "ceterms:recommends", "ceterms:preparationFrom", "ceterms:advancedStandingFrom", "ceterms:entryCondition", "ceterms:corequisite", "ceterms:isRequiredFor", "ceterms:isRecommendedFor", "ceterms:isPreparationFor", "ceterms:isAdvancedStandingFor" } )
			{
				query += new DSPStep( property, "ceterms:ConditionProfile", false ) { IgnoreTarget = true }.AddChildren( new List<DSPStep>()
				{
					new DSPStep( "ceterms:targetCredential", "ceterms:Credential" ),
					new DSPStep( "ceterms:targetAssessment", "ceterms:AssessmentProfile" ).AddChildren(new List<DSPStep>()
					{
						new DSPStep( "ceterms:assesses", "ceterms:CredentialAlignmentObject", false ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
						} )
					} ),
					new DSPStep( "ceterms:targetLearningOpportunity", "ceterms:LearningOpportunityProfile" ).AddChildren(new List<DSPStep>()
					{
						new DSPStep( "ceterms:teaches", "ceterms:CredentialAlignmentObject", false ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
						} )
					} ),
					new DSPStep( "ceterms:targetCompetency", "ceterms:CredentialAlignmentObject" ){ IgnoreTarget = true }.AddChildren(new List<DSPStep>()
					{
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceterms:targetNode" )
					} )
				} ).Stringify() + "UNION";
			}

			//Manifest connections
			query += 
				new DSPStep( "ceterms:commonCosts", "ceterms:CostManifest" ).Stringify() + "UNION" +
				new DSPStep( "ceterms:commonConditions", "ceterms:CondtionManifest" ).Stringify() + "UNION";

			//Reverse connections
			var reverseParts = new List<string>();
			foreach ( var property in new List<string>() { "^ceterms:targetCredential", "^ceterms:targetLearningOpportunity", "^ceterms:targetAssessment" } )
			{
				var connections = new List<string>() { "^ceterms:requires", "^ceterms:recommends", "^ceterms:preparationFrom", "^ceterms:advancedStandingFrom", "^ceterms:isRequiredFor", "^ceterms:isRecommendedFor", "^ceterms:isPreparationFor", "^ceterms:isAdvancedStandingFor", "^ceterms:entryCondition", "^ceterms:corequisite" };
				var credentialChildren = new List<DSPStep>();
				var assessmentChildren = new List<DSPStep>();
				var learningOpportunityChildren = new List<DSPStep>();
				var conditionManifestChildren = new List<DSPStep>();
				foreach ( var connection in connections )
				{
					credentialChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:Credential", false, "?search_:_ignore_:_CredentialType" ) );
					assessmentChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:AssessmentProfile", false ) );
					learningOpportunityChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:LearningOpportunityProfile", false ) );
					conditionManifestChildren.Add( DSPStep_TargetAndTypeChunk( connection, "ceterms:ConditionManifest", false ) );
				}

				reverseParts.Add( new DSPStep( property, "ceterms:ConditionProfile" ) { IgnoreTarget = true }.AddChildren(
					credentialChildren.Concat( assessmentChildren ).Concat( learningOpportunityChildren ).Concat( conditionManifestChildren ).ToList()
				).Stringify() );
			}

			query += string.Join( "UNION", reverseParts );

			return query;
		}
		//

		public List<DSPStep> GetDSP_OrganizationConnections( List<string> connectionProperties )
		{
			var dspParts = new List<DSPStep>();
			var qualityAssuranceProperties = new List<string>() { "ceterms:accreditedBy", "^ceterms:accredits", "ceterms:approvedBy", "^ceterms:approves", "ceterms:regulatedBy", "^ceterms:regulates", "ceterms:recognizedBy", "^ceterms:recognizes" };
			foreach ( var connectionProperty in connectionProperties )
			{
				var qualityAssurances = new List<DSPStep>();
				foreach ( var qualityAssuranceProperty in qualityAssuranceProperties )
				{
					qualityAssurances.Add( new DSPStep( qualityAssuranceProperty, "ceterms:Agent" ) );
				}
				dspParts.Add( new DSPStep( connectionProperty, "ceterms:Agent" ).AddChildren( qualityAssurances ) );
			}

			return dspParts;
		}
		//

		public string GetDSP_CTDLOwnsOffersOrganizationConnections()
		{
			var parts = GetDSP_OrganizationConnections( new List<string>() { "ceterms:ownedBy", "ceterms:offeredBy", "^ceterms:owns", "^ceterms:offers" } );
			return string.Join( "UNION", parts.Select( m => m.Stringify() ).ToList() );
		}
		//

		public string GetDSP_CTDLASNCreatorPublisherOrganizationConnections()
		{
			var parts = GetDSP_OrganizationConnections( new List<string>() { "ceasn:creator", "ceasn:publisher", "ceasn:rightsHolder" } );
			return string.Join( "UNION", parts.Select( m => m.Stringify() ).ToList() );
		}
		//

		public string GetDSP_DSPStepTemplateForProperties( List<string> properties, Func<string, DSPStep> templateFunction )
		{
			var dspParts = new List<DSPStep>();
			foreach( var property in properties )
			{
				dspParts.Add( templateFunction( property ) );
			}
			var query = string.Join( "UNION", dspParts.Select( m => m.Stringify() ).ToList() );

			return query;
		}
		//


		public class QueryReference
		{
			public QueryReference()
			{
				RawResult = new JObject();
				Bindings = new List<JObject>();
				DebugInfo = new JObject();
			}
			public JObject RawResult { get; set; }
			public List<JObject> Bindings { get; set; }
			public bool IsFinished { get; set; }
			public JObject DebugInfo { get; set; }
		}
		//

		public QueryReference QueryDSPStep( string uri, List<string> uriList, List<DSPStep> steps, string cacheTag, string apiKey, string referrer )
		{
			return QueryDSPStep( uri, uriList, string.Join( "UNION", steps.Select( m => m.Stringify() ).ToList() ), cacheTag, apiKey, referrer );
		}
		public QueryReference QueryDSPStep( string uri, List<string> uriList, DSPStep step, string cacheTag, string apiKey, string referrer )
		{
			return QueryDSPStep( uri, uriList, step.Stringify(), cacheTag, apiKey, referrer );
		}
		public QueryReference QueryDSPStep( string uri, List<string> uriList, string stringifiedStep, string cacheTag, string apiKey, string referrer )
		{
			var reference = new QueryReference();
			ThreadPool.QueueUserWorkItem( delegate
			{
				try
				{
					var cacheName = "QueryDSPStep_" + uri + "_" + cacheTag;
					var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );
					var uriString = "<" + ( uriList == null || uriList.Count() == 0 ? uri : string.Join( "> <", uriList ) ) + ">";
					var queryTextRaw = @"
					{
						VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
						VALUES ?resultURI {
							" + uriString + @"
						}
						{ " + stringifiedStep + @" }
					}";

					//Get the description set query text
					var queryTextSPARQLized = queryTextRaw.Replace( "_>_", "_R_" ).Replace( "_<_", "_L_" ).Replace( "_:_", "_C_" ); //Now with valid SPARQL variable names
					var queryTextFlattened = queryTextSPARQLized.Replace( "\r\n", " " ).Replace( "\n", " " ).Replace( "\t", "" ); //Now a single line string
					var queryVariables = Regex.Matches( queryTextFlattened, @" \?_\S* " ).Cast<Match>().Where( m => !m.Value.Contains( "_ignore_" ) ).Select( m => m.Value ).Distinct().ToList(); //Relevant variables
					var queryVariablesFlattened = string.Join( " ", queryVariables );
					var queryPrefixes = GetContextPrefixes( "credreg: " + queryTextFlattened );
					var queryForGettingData = ( queryPrefixes + " SELECT DISTINCT " + queryVariablesFlattened + " WHERE " + queryTextFlattened ).Replace( "  ", " " ).Replace( "  ", " " );

					reference.DebugInfo[ "Tag" ] = cacheTag;
					reference.DebugInfo[ "QueryDSPStep" ] = queryForGettingData;

					//Get the data from the cache if available
					var cachedBindings = ( List<JObject> ) MemoryCache.Default.Get( cacheName );
					if( cachedBindings != null )
					{
						reference.Bindings = cachedBindings;
						reference.DebugInfo[ "LoadedFromCache" ] = true;
					}
					//Otherwise, query it
					else
					{
						reference.RawResult = RunSPARQLQuery( queryForGettingData, apiKey, 0, -1, referrer );
						reference.Bindings = ( ( JArray ) reference.RawResult[ "results" ][ "bindings" ] ).Select( m => ( JObject ) m ).ToList();
						MemoryCache.Default.Remove( cacheName );
						MemoryCache.Default.Add( cacheName, reference.Bindings, DateTime.Now.AddMinutes( 15 ) );

						reference.DebugInfo[ "LoadedFromCache" ] = false;
					}
				}
				catch( Exception ex )
				{
					reference.DebugInfo[ "ErrorMessage" ] = ex.Message;
				}

				reference.IsFinished = true;
			} );
			return reference;
		}
		//

		public List<QueryReference> QuerySubDSPStep( string uri, string cacheTag, string apiKey, string referrer, DSPStep branchStep, QueryReference branchReference, List<DSPStep> leafSteps, int chunkSize = 250 )
		{
			while ( !branchReference.IsFinished )
			{
				Thread.Sleep( 10 );
			}

			var referenceVariable = branchStep.GetSelfFullName().Replace( "_>_", "_R_" ).Replace( "_<_", "_L_" ).Replace( "_:_", "_C_" );
			var referenceURIs = branchReference.Bindings.Select( m => m[ referenceVariable ][ "value" ].ToString() ).Distinct().ToList();
			var referenceList = new List<QueryReference>();
			for( var i = 0; i < branchReference.Bindings.Count(); i += chunkSize )
			{
				referenceList.Add( QueryDSPStep( "", referenceURIs.Skip( i ).Take( chunkSize ).ToList(), string.Join( "UNION", leafSteps.Select( m => m.Stringify() ).ToList() ), uri + "_" + cacheTag + "_" + i, apiKey, referrer ) );
			}

			return referenceList;
		}
		public void MergeSubDSPStepBindings( List<JObject> finalBindings, List<QueryReference> subStepReferences, DSPStep branchStep )
		{
			var referenceVariable = branchStep.GetSelfFullName().Replace( "_>_", "_R_" ).Replace( "_<_", "_L_" ).Replace( "_:_", "_C_" );
			var referenceBindings = subStepReferences.SelectMany( m => m.Bindings ).ToList();
			foreach( var binding in referenceBindings )
			{
				foreach( var item in binding.Properties() )
				{
					finalBindings.Add( new JObject() { { referenceVariable + item.Name, item.Value } } );
				}
			}
		}
		//

		public class DescriptionSetQueryResult
		{
			public DescriptionSetQueryResult()
			{
				Bindings = new List<JObject>();
				DebugInfo = new JObject();
			}
			public List<JObject> Bindings { get; set; }
			public JObject DebugInfo { get; set; }
		}
		//

		#endregion

		#region Old Stuff - Do not use!

		/*
		public void GetRelatedURIs( RelatedItemsData relatedData, Dictionary<string, string> resultURIAndType, ConcurrentBag<string> dspURIs, string apiKey, string referrer )
		{
			var descriptionSetMap = new Dictionary<List<string>, Func<string, string>>()
			{
				{ SchemaContext.CredentialTypes, DescriptionSetQuery_Credential },
				{ new List<string>() { "ceterms:CredentialOrganization", "ceterms:QACredentialOrganization" }, DescriptionSetQuery_Organization },
				{ new List<string>() { "ceterms:AssessmentProfile" }, DescriptionSetQuery_AssessmentProfile },
				{ new List<string>() { "ceterms:LearningOpportunityProfile" }, DescriptionSetQuery_LearningOpportunityProfile },
				{ new List<string>() { "ceasn:CompetencyFramework" }, DescriptionSetQuery_CompetencyFramework },
				{ new List<string>() { "skos:ConceptScheme" }, DescriptionSet_ConceptScheme },
				{ new List<string>() { "navy:Rating" }, DescriptionSet_Rating },
				{ new List<string>() { "navy:Job" }, DescriptionSet_Job },
				{ new List<string>() { "navy:EnlistedClassification" }, DescriptionSet_EnlistedClassification },
				{ new List<string>() { "navy:System" }, DescriptionSet_System }
			};

			//For each result...
			var progressMonitor = new Dictionary<string, bool>();
			foreach ( var uriAndType in resultURIAndType )
			{
				progressMonitor.Add( uriAndType.Key, false );
				ThreadPool.QueueUserWorkItem( delegate
				{
					try
					{
						//Figure out what kind of description set to get for it based on its @type
						var method = descriptionSetMap.FirstOrDefault( m => m.Key.Contains( uriAndType.Value ) ).Value;
						if ( method != null )
						{
							//Get the item map for this result
							var itemMapForResult = ( JObject ) relatedData.RelatedItemsMap[ uriAndType.Key ];
							if ( relatedData.IncludeDebugInfo )
							{
								itemMapForResult[ "DebugInfo" ] = new JObject();
							}

							//Get the description set query text
							var queryTextRaw = method( uriAndType.Key ); //Verbatim from the DSP method
							var queryTextSPARQLized = queryTextRaw.Replace( "_>_", "_R_" ).Replace( "_<_", "_L_" ).Replace( "_:_", "_C_" ); //Now with valid SPARQL variable names
							var queryTextFlattened = queryTextSPARQLized.Replace( "\r\n", " " ).Replace( "\n", " " ).Replace( "\t", "" ); //Now a single line string
							var queryVariables = Regex.Matches( queryTextFlattened, @" \?_\S* " ).Cast<Match>().Where( m => !m.Value.Contains( "_ignore_" ) ).Select( m => m.Value ).Distinct().ToList(); //Relevant variables
							var queryVariablesFlattened = string.Join( " ", queryVariables );
							var queryPrefixes = GetContextPrefixes( queryTextFlattened );
							var queryForGettingData = ( queryPrefixes + " SELECT DISTINCT " + queryVariablesFlattened + " WHERE " + queryTextFlattened ).Replace( "  ", " " ).Replace( "  ", " " );

							try
							{

								//Use this to make deduplication more efficient at the end
								var nodesToCleanUp = new List<string>();

								//Try to get the DSP from the cache
								var result = ( JObject ) MemoryCache.Default.Get( "GetRelatedURIs_" + uriAndType.Key );
								var loadedFromCache = true;
								if( result == null )
								{
									//Run the description set query and cache the result
									loadedFromCache = false;
									result = RunSPARQLQuery( queryForGettingData, apiKey, -1, -1, referrer );
									MemoryCache.Default.Remove( "GetRelatedURIs_" + uriAndType.Key );
									MemoryCache.Default.Add( "GetRelatedURIs_" + uriAndType.Key, result, DateTime.Now.AddMinutes( 30 ) );
								}

								//Use this to help with debugging
								if ( relatedData.IncludeDebugInfo )
								{
									( ( JObject ) itemMapForResult[ "DebugInfo" ] ).Add( "SPARQLQuery", queryForGettingData );
									( ( JObject ) itemMapForResult[ "DebugInfo" ] ).Add( "RawBindings", result );
									( ( JObject ) itemMapForResult[ "DebugInfo" ] ).Add( "LoadedFromCache", loadedFromCache );
								}

								//Get the binding items
								var bindingItems = ( ( JArray ) result[ "results" ][ "bindings" ] ).Select( m => ( JObject ) m ).ToList();

								//Keep track of their properties
								var propertyURIMap = new Dictionary<string, List<string>>();
								var importantProperties = bindingItems.SelectMany( m => m.Properties() ).Where( m => !m.Name.Contains( "_ignore_" ) ).ToList(); //All properties from all relevant bindings
								var importantPropertyNames = importantProperties.Select( m => m.Name ).Distinct().ToList(); //Unique names

								nodesToCleanUp.AddRange( importantPropertyNames );
								foreach ( var name in importantPropertyNames )
								{
									//Get the unique URIs for all binding items for a given property
									var bindingValues = bindingItems
										.Where( m => m[ name ] != null && m[ name ][ "value" ] != null )
										.Select( m => m[ name ][ "value" ].ToString().ToLower().Replace( "/graph/", "/resources/" ).Replace( "https://credreg.net/bnodes/", "_:" ) )
										.Distinct().ToList();

									//Track the URIs for later
									foreach ( var value in bindingValues )
									{
										dspURIs.Add( value );
									}

									//Add the URIs to the map for this result
									propertyURIMap[ name ] = bindingValues;
									//itemMapForResult[ name ] = JArray.FromObject( bindingValues );
								}

								//Construct a path object
								var pathSets = new List<JObject>();

								//Clean up the item maps
								foreach ( var nodeName in nodesToCleanUp.Distinct().ToList() )
								{
									//itemMapForResult[ nodeName ] = JArray.FromObject( ( ( JArray ) itemMapForResult[ nodeName ] ).Distinct() );

									var path = nodeName.Replace( "_R_", " > " ).Replace( "_L_", " < " ).Replace( "_C_", ":" ).Trim();
									pathSets.Add( new JObject()
									{
										{ "Path", path },
										{ "URIs", JArray.FromObject( propertyURIMap[nodeName].Distinct().ToList() ) }
									} );
								}

								itemMapForResult[ "RelatedData" ] = JArray.FromObject( pathSets );

							}
							catch ( Exception ex )
							{
								itemMapForResult.Add( "DescriptionSetProcessingError_" + Guid.NewGuid().ToString(), new JObject() { { "Error", ex.Message }, { "SPARQLQuery", queryForGettingData } } );
							}
						}
					}
					catch { }

					//Guarantee the thread finishes
					progressMonitor[ uriAndType.Key ] = true;
				} );
			}

			//Wait for all the threads to finish
			while ( !progressMonitor.All( m => m.Value == true ) )
			{
				Thread.Sleep( 25 );
			}
		}
		//
		*/

		/*
		public string DescriptionSetQuery_Credential( string uri )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );
			
			//Direct credential-to-credential connections
			var credentialConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:isPartOf", "ceterms:hasPart", "ceterms:majorAlignment", "ceterms:minorAlignment", "ceterms:exactAlignment", "ceterms:narrowAlignment", "ceterms:broadAlignment", "^ceterms:isPartOf", "^ceterms:hasPart", "^ceterms:majorAlignment", "^ceterms:minorAlignment", "^ceterms:exactAlignment", "^ceterms:narrowAlignment", "^ceterms:broadAlignment" }, delegate ( string property )
			 {
				 return DSPStep_TargetAndTypeChunk( property, "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" );
			 } );

			//Quality Assurance
			var qualityAssuranceConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:revokedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:revokes" }, delegate ( string property )
			 {
				 return new DSPStep( property, "ceterms:Agent" );
			 } );

			//Query
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{ " + 
					GetDSP_CTDLOwnsOffersOrganizationConnections() + "UNION" +
					qualityAssuranceConnectionsQuery + "UNION" +
					credentialConnectionsQuery + "UNION" +
					GetDSP_CTDLConditionProfileConnections() + @"
				}
			}";

			return query;
		}
		//

		public string DescriptionSetQuery_Organization( string uri )
		{
			var credentialTypes = string.Join(" ", SchemaContext.CredentialTypes);

			//Direct organization-to-organization connections
			var ctdlOrganizationConnections = new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:accredits", "ceterms:approves", "ceterms:recognizes", "ceterms:regulates", "ceterms:department", "ceterms:subOrganization", "ceterms:parentOrganization", "^ceterms:accreditedBy", "^ceterms:approvedBy", "^ceterms:recognizedBy", "^ceterms:regulatedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:department", "^ceterms:subOrganization", "^ceterms:parentOrganization" };
			var organizationConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlOrganizationConnections, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:Agent", true, "?search_:_ignore_:_AgentType" );
			} );

			//Connections to other CTDL stuff
			var ctdlConnectionProperties = new List<string>() { "ceterms:owns", "ceterms:offers", "ceterms:accredits", "ceterms:approves", "ceterms:recognizes", "ceterms:regulates", "ceterms:revokes", "^ceterms:ownedBy", "^ceterms:offeredBy", "^ceterms:accreditedBy", "^ceterms:approvedBy", "^ceterms:recognizedBy", "^ceterms:regulatedBy", "^ceterms:revokedBy" };
			var credentialConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" );
			} );
			var assessmentConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:AssessmentProfile" );
			} );
			var learningOpportunityConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:LearningOpportunityProfile" );
			} );

			//Connections to CTDL-ASN stuff
			var ctdlasnConnectionProperties = new List<string>() { "^ceasn:creator", "^ceasn:publisher" };
			var competencyFrameworkConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceasn:CompetencyFramework" );
			} );
			var conceptSchemeConnectionsQuery = GetDSP_DSPStepTemplateForProperties( ctdlConnectionProperties, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "skos:ConceptScheme" );
			} );

			//Query
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?search_:_ignore_:_AgentType { ceterms:CredentialOrganization ceterms:QACredentialOrganization }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{" +
					organizationConnectionsQuery + "UNION" +
					credentialConnectionsQuery + "UNION" +
					assessmentConnectionsQuery + "UNION" +
					learningOpportunityConnectionsQuery + "UNION" +
					competencyFrameworkConnectionsQuery + "UNION" +
					conceptSchemeConnectionsQuery + "UNION" +
					new DSPStep( "ceterms:hasCostManifest", "ceterms:CostManifest" ) + "UNION" +
					new DSPStep( "^ceterms:costManifestOf", "ceterms:CostManifest" ) + "UNION" +
					new DSPStep( "ceterms:hasConditionManifest", "ceterms:ConditionManifest" ) + "UNION" +
					new DSPStep( "^ceterms:conditionManifestOf", "ceterms:ConditionManifest" ) + @"
				}
			}";

			return query;
		}
		//

		public string DescriptionSetQuery_AssessmentProfile( string uri )
		{
			var credentialTypes = string.Join(" ", SchemaContext.CredentialTypes);

			//Quality Assurance
			var qualityAssuranceConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:revokedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:revokes" }, delegate ( string property )
			{
				return new DSPStep( property, "ceterms:Agent" );
			} );

			//Query
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{ " +
					GetDSP_CTDLOwnsOffersOrganizationConnections() + "UNION" +
					qualityAssuranceConnectionsQuery + "UNION" +
					GetDSP_CTDLConditionProfileConnections() + @"
				}
			}";

			return query;
		}
		//

		public string DescriptionSetQuery_LearningOpportunityProfile( string uri )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			//Direct credential-to-credential connections
			var learningOpportunityConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:isPartOf", "ceterms:hasPart", "^ceterms:isPartOf", "^ceterms:hasPart" }, delegate ( string property )
			{
				return DSPStep_TargetAndTypeChunk( property, "ceterms:LearningOpportunityProfile" );
			} );

			//Quality Assurance
			var qualityAssuranceConnectionsQuery = GetDSP_DSPStepTemplateForProperties( new List<string>() { "ceterms:accreditedBy", "ceterms:approvedBy", "ceterms:recognizedBy", "ceterms:regulatedBy", "ceterms:revokedBy", "^ceterms:accredits", "^ceterms:approves", "^ceterms:recognizes", "^ceterms:regulates", "^ceterms:revokes" }, delegate ( string property )
			{
				return new DSPStep( property, "ceterms:Agent" );
			} );

			//Query
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{ " +
					GetDSP_CTDLOwnsOffersOrganizationConnections() + "UNION" +
					qualityAssuranceConnectionsQuery + "UNION" +
					learningOpportunityConnectionsQuery + "UNION" +
					GetDSP_CTDLConditionProfileConnections() + @"
				}
			}";

			return query;
		}
		//

		public string DescriptionSetQuery_CompetencyFramework( string uri )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );

			//Framework Alignments
			var frameworkAlignmentProperties = new List<string>() { "ceasn:alignFrom", "ceasn:alignTo", "^ceasn:alignFrom", "^ceasn:alignTo" };
			var frameworkAlignmentQuery = GetDSP_DSPStepTemplateForProperties( frameworkAlignmentProperties, delegate ( string property )
			{
				return new DSPStep( property, "ceasn:CompetencyFramework" );
			} );

			//Framework Concepts
			var frameworkConceptsProperties = new List<string>() { "ceasn:conceptTerm", "ceasn:educationLevelType", "ceasn:publicationStatusType" };
			var frameworkConceptsQuery = GetDSP_DSPStepTemplateForProperties( frameworkConceptsProperties, delegate ( string property )
			{
				return DSPStep_ConceptToConceptSchemeChunk( property );
			} );

			//Competency Steps
			var competencySteps = new List<DSPStep>();

			//Competency Alignments
			var competencyAlignmentProperties = new List<string>() { "ceasn:alignFrom", "ceasn:alignTo", "ceasn:comprisedOf", "ceasn:broadAlignment", "ceasn:narrowAlignment", "ceasn:majorAlignment", "ceasn:minorAlignment", "ceasn:exactAlignment", "ceasn:prerequisiteAlignment", "ceasn:crossSubjectAlignment", "^ceasn:alignFrom", "^ceasn:alignTo", "^ceasn:comprisedOf", "^ceasn:broadAlignment", "^ceasn:narrowAlignment", "^ceasn:majorAlignment", "^ceasn:minorAlignment", "^ceasn:exactAlignment", "^ceasn:prerequisiteAlignment", "^ceasn:crossSubjectAlignment" };
			foreach(var property in competencyAlignmentProperties )
			{
				competencySteps.Add( DSPStep_CompetencyToCompetencyFrameworkChunk( property ) );
			}

			//Competency Concepts
			var competencyConceptsProperties = new List<string>() { "ceasn:conceptTerm", "ceasn:educationLevelType", "ceasn:complexityLevel" };
			foreach ( var property in competencyConceptsProperties )
			{
				competencySteps.Add( DSPStep_ConceptToConceptSchemeChunk( property ) );
			}

			//Competency Connections
			competencySteps.Add( new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ) { IgnorePath = true, IgnoreTarget = true }.AddChildren( new List<DSPStep>()
			{
				//Assessment that assesses the competency
				new DSPStep( "^ceterms:assesses", "ceterms:AssessmentProfile", false ).AddChildren( new List<DSPStep>()
				{
					//And entities that require(etc) the assessment
					new DSPStep( "^ceterms:targetAssessment", "ceterms:ConditionProfile", false ){ IgnoreTarget = true }.AddChildren(
						DescriptionSetQuery_CompetencyFramework_GetInboundConditionProfileConnections() //Calling this three times ensures we avoid any potential pass-by-reference errors when these things get processed
					)
				//And entities that the assessment is required(etc) for
				} ),//.Concat( DescriptionSetQuery_CompetencyFramework_GetOutboundConditionProfileConnections() ).ToList() ), //Simplified for performance
				//Learning opportunity that teaches the competency
				new DSPStep( "^ceterms:teaches", "ceterms:LearningOpportunityProfile", false ).AddChildren( new List<DSPStep>()
				{
					//And entities that require(etc) the learning opportunity
					new DSPStep( "^ceterms:targetLearningOpportunity", "ceterms:ConditionProfile", false ){ IgnoreTarget = true }.AddChildren(
						DescriptionSetQuery_CompetencyFramework_GetInboundConditionProfileConnections()
					)
				//And entities that the learning opportunity is required(etc) for
				} ),//.Concat( DescriptionSetQuery_CompetencyFramework_GetOutboundConditionProfileConnections() ).ToList() ),
				//Entities that require(etc) the competency
				new DSPStep( "^ceterms:targetCompetency", "ceterms:ConditionProfile", false ){ IgnoreTarget = true }.AddChildren(
					DescriptionSetQuery_CompetencyFramework_GetInboundConditionProfileConnections()
				)
			} ) );

			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{" +
					GetDSP_CTDLASNCreatorPublisherOrganizationConnections() + "UNION" +
					frameworkAlignmentQuery + "UNION" +
					frameworkConceptsQuery + "UNION" +
					new DSPStep( "^ceasn:isPartOf", "ceasn:Competency" ).AddChildren(
						competencySteps
					).Stringify() + @"
				}
			}";

			return query;

		}
		//

		public string DescriptionSet_ConceptScheme ( string uri )
		{
			//Scheme Concepts
			var schemeConceptsProperties = new List<string>() { "ceasn:conceptTerm", "ceasn:publicationStatusType" };
			var schemeConceptsQuery = GetDSP_DSPStepTemplateForProperties( schemeConceptsProperties, delegate ( string property )
			{
				return DSPStep_ConceptToConceptSchemeChunk( property );
			} );

			//Concept Steps
			var conceptSteps = new List<DSPStep>();

			//Concept Alignments
			var conceptAlignmentProperties = new List<string>() { "skos:broadMatch", "skos:narrowMatch", "skos:exactMatch", "skos:closeMatch", "skos:related", "^skos:broadMatch", "^skos:narrowMatch", "^skos:exactMatch", "^skos:closeMatch", "^skos:related" };
			foreach ( var property in conceptAlignmentProperties )
			{
				conceptSteps.Add( new DSPStep( property, "ceasn:Competency", false ) );
			}

			var query = @"
			{
				VALUES ?resultURI {
					<" + uri + @">
				}
				{" +
					GetDSP_CTDLASNCreatorPublisherOrganizationConnections() + "UNION" +
					schemeConceptsQuery + "UNION" +
					new DSPStep( "^skos:inScheme", "skos:Concept" ).AddChildren(
						conceptSteps
					).Stringify() + @"
				}
			}";

			return query;
		}
		//

		public string DescriptionSet_Rating( string uri )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{" +
					DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ).Stringify() + "UNION" +
					new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
						{
							DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
						} )
					} ).Stringify() + "UNION" +
					DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ).Stringify() + "UNION" +
					DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ).Stringify() + "UNION" +
					DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "navy:hasOccupationalTask", "navy:OccupationalTask", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "navy:hasWorkRole", "navy:WorkRole", false ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasFunctionalGroup" ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasWorkActivity" ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasPayGradeType" ),
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasTaskFlagType" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
						} )
					} ).Stringify() + "UNION" +
					new DSPStep( "^navy:hasRating", "navy:MaintenanceTask", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
						} ),
						new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
							{
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
							} )
						} ),
						new DSPStep( "^navy:hasMaintenanceTask", "navy:System", false ).AddChildren( new List<DSPStep>()
						{
							DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
							new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
							new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
								DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
								new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
							})
						} )
					} ).Stringify() + @"
				}
			}";

			return query;

		}
		//

		public string DescriptionSet_Job( string uri )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{" +
					new DSPStep("navy:hasRating", "navy:Rating").AddChildren(new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
						new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
							{
								DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
							} )
						} ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false ),
						new DSPStep( "^navy:hasRating", "navy:MaintenanceTask", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
							{
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
							} ),
							new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
							{
								new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
								{
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
								} )
							} ),
							new DSPStep( "^navy:hasMaintenanceTask", "navy:System", false ).AddChildren( new List<DSPStep>()
							{
								DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
								new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
								new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
									DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
									new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
								})
							} )
						} )
					} ).Stringify() + "UNION" +
					new DSPStep( "navy:hasOccupationalTask", "navy:OccupationalTask", false ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "navy:hasWorkRole", "navy:WorkRole", false ),
						DSPStep_ConceptToConceptSchemeChunk( "navy:hasFunctionalGroup" ),
						DSPStep_ConceptToConceptSchemeChunk( "navy:hasWorkActivity" ),
						DSPStep_ConceptToConceptSchemeChunk( "navy:hasPayGradeType" ),
						DSPStep_ConceptToConceptSchemeChunk( "navy:hasTaskFlagType" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
						DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
					} ).Stringify() + @"
				}
			}";

			return query;

		}
		//

		public string DescriptionSet_EnlistedClassification( string uri )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{" +
					new DSPStep( "navy:hasRating", "navy:Rating" ).AddChildren( new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
						new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
							{
								DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
							} )
						} ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
						DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false ).AddChildren( new List<DSPStep>(){
							new DSPStep( "navy:hasOccupationalTask", "navy:OccupationalTask", false ).AddChildren( new List<DSPStep>()
								{
									new DSPStep( "navy:hasWorkRole", "navy:WorkRole", false ),
									DSPStep_ConceptToConceptSchemeChunk( "navy:hasFunctionalGroup" ),
									DSPStep_ConceptToConceptSchemeChunk( "navy:hasWorkActivity" ),
									DSPStep_ConceptToConceptSchemeChunk( "navy:hasPayGradeType" ),
									DSPStep_ConceptToConceptSchemeChunk( "navy:hasTaskFlagType" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
								} )
						}),
						new DSPStep( "^navy:hasRating", "navy:MaintenanceTask", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
							{
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
							} ),
							new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
							{
								new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
								{
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
								} )
							} ),
							new DSPStep( "^navy:hasMaintenanceTask", "navy:System", false ).AddChildren( new List<DSPStep>()
							{
								DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
								new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
								new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
									DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
									new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
								})
							} )
						} )
					} ).Stringify() + @"
				}
			}";

			return query;
		}
		//

		public string DescriptionSet_System( string uri )
		{
			var credentialTypes = string.Join( " ", SchemaContext.CredentialTypes );
			var query = @"
			{
				VALUES ?search_:_ignore_:_CredentialType { " + credentialTypes + @" }
				VALUES ?resultURI {
					<" + uri + @">
				}
				{" +
					//Describe self
					DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ).Stringify() + "UNION" +
					new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ).Stringify() + "UNION" + 
					//Applies if this is the top level system
					new DSPStep( "^ceasn:isPartOf", "navy:System" ).AddChildren(new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
						new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false ),
						new DSPStep( "navy:hasMaintenanceTask", "navy:MaintenanceTask" ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
							{
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
							} ),
							new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
							{
								new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
								{
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
									DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
								} )
							} ),
							new DSPStep( "navy:hasRating", "navy:Rating" ).AddChildren(new List<DSPStep>()
							{
								DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
								new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
								{
									new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
									{
										DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
									} )
								} ),
								DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
								DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
								DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false )
							} )
						} )
					} ).Stringify() + "UNION" +
					//Applies if this is not the lop level system
					new DSPStep( "ceasn:isPartOf", "navy:System", false ).AddChildren( new List<DSPStep>(){
						DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
						new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
					} ).Stringify() + "UNION" +
					new DSPStep( "ceasn:isChildOf", "navy:System" ).AddChildren(new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
						new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
					} ).Stringify() + "UNION" +
					//Applies regardless
					new DSPStep( "ceasn:hasChild", "navy:System" ).AddChildren(new List<DSPStep>()
					{
						DSPStep_ConceptToConceptSchemeChunk( "navy:systemType" ),
						new DSPStep( "navy:hasSourceIdentifier", "navy:SourceIdentifier", false )
					} ).Stringify() + "UNION" +
					new DSPStep( "navy:hasMaintenanceTask", "navy:MaintenanceTask" ).AddChildren( new List<DSPStep>()
					{
						new DSPStep( "navy:hasPerformanceObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
						{
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
							DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
						} ),
						new DSPStep( "navy:hasTrainingTask", "navy:TrainingTask", false ).AddChildren( new List<DSPStep>()
						{
							new DSPStep( "navy:hasLearningObjective", "ceasn:Competency", false ).AddChildren( new List<DSPStep>()
							{
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:knowledgeEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:skillEmbodied" ),
								DSPStep_CompetencyToCompetencyFrameworkChunk( "ceasn:abilityEmbodied" )
							} )
						} ),
						new DSPStep("navy:hasRating", "navy:Rating").AddChildren(new List<DSPStep>()
						{
							DSPStep_ConceptToConceptSchemeChunk( "navy:hasDoDOccupationType" ),
							new DSPStep( "navy:hasOccupationType", "skos:Concept", false ).AddChildren( new List<DSPStep>()
							{
								new DSPStep( "^ceterms:targetNode", "ceterms:CredentialAlignmentObject", false ){ IgnorePath = true, IgnoreTarget = true }.AddLines( new List<DSPStep>()
								{
									DSPStep_TargetAndTypeChunk( "^ceterms:occupationType", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" )
								} )
							} ),
							DSPStep_TargetAndTypeChunk( "^navy:hasRating", "ceterms:Credential", true, "?search_:_ignore_:_CredentialType" ),
							DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:EnlistedClassification", false ),
							DSPStep_TargetAndTypeChunk( "^navy:hasRating", "navy:Job", false )
						} )
					} ).Stringify() + @"
				}
			}";

			return query;
		}
		//

		*/

		#endregion

	}
}
