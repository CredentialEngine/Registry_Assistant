using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RAResponse = RA.Models.RegistryAssistantResponse;

namespace RA.SamplesForDocumentation
{
	public class SampleServices
	{
		public static string thisClassName = "SampleServices";
		public string environment = GetAppKeyValue( "envType" );
		//helpers. Could hard code or retrieve from the config file. 
		public static string GetMyApiKey()
		{
			return GetAppKeyValue( "myOrgApiKey" );
		} //
		public static string GetMyOrganizationCTID()
		{
			return GetAppKeyValue( "myOrgCTID" );
		} //

		#region === Application Keys Methods ===

		/// <summary>
		/// Gets the value of an application key from web.config. Returns blanks if not found
		/// </summary>
		/// <remarks>This property is explicitly thread safe.</remarks>
		public static string GetAppKeyValue( string keyName )
		{

			return GetAppKeyValue( keyName, "" );
		} //

		/// <summary>
		/// Gets the value of an application key from web.config. Returns the default value if not found
		/// </summary>
		/// <remarks>This property is explicitly thread safe.</remarks>
		public static string GetAppKeyValue( string keyName, string defaultValue )
		{
			string appValue = "";

			try
			{
				appValue = System.Configuration.ConfigurationManager.AppSettings[ keyName ];
				if ( appValue == null )
					appValue = defaultValue;
			}
			catch
			{
				appValue = defaultValue;
				if ( HasMessageBeenPreviouslySent( keyName ) == false )
					LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
			}

			return appValue;
		} //
		public static int GetAppKeyValue( string keyName, int defaultValue )
		{
			int appValue = -1;

			try
			{
				appValue = Int32.Parse( System.Configuration.ConfigurationManager.AppSettings[ keyName ] );

				// If we get here, then number is an integer, otherwise we will use the default
			}
			catch
			{
				appValue = defaultValue;
				if ( HasMessageBeenPreviouslySent( keyName ) == false )
					LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
			}

			return appValue;
		} //
		public static bool GetAppKeyValue( string keyName, bool defaultValue, bool reportMissingKey = true )
		{
			bool appValue = false;

			try
			{
				appValue = bool.Parse( System.Configuration.ConfigurationManager.AppSettings[ keyName ] );
			}
			catch ( Exception ex )
			{
				appValue = defaultValue;
				if ( reportMissingKey && HasMessageBeenPreviouslySent( keyName ) == false )
					LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
			}

			return appValue;
		} //

		public static bool HasMessageBeenPreviouslySent( string keyName )
		{

			string key = "appkey_" + keyName;
			//check cache for keyName
			if ( HttpRuntime.Cache[ key ] != null )
			{
				return true;
			}
			else
			{
				//not really much to store
				HttpRuntime.Cache.Insert( key, keyName );
			}

			return false;
		}
		#endregion

		#region publishing 
		/// <summary>
		/// Method to post to a Registry Assistant publish or format endpoint
		/// </summary>
		/// <param name="entityType">Credential, Organization, etc. </param>
		/// <param name="requestType">format or publish</param>
		/// <param name="payload">Serialized input request</param>
		/// <param name="apiKey">The organization Api Key from the accounts site. </param>
		/// <returns></returns>
		public string SimplePost( string entityType, string requestType, string payload, string apiKey )
		{
			string serviceUri = GetAppKeyValue( "registryAssistantApi" );
			string assistantUrl = serviceUri + string.Format( "{0}/{1}", entityType, requestType );
			return SimplePost( assistantUrl, payload, apiKey ); ;
		}
		public string SimplePost( string assistantUrl, string payload, string apiKey )
		{
			var result = "";
			using ( var client = new HttpClient() )
			{
				// Accept JSON
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
				// Add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				// Format the json as content
				var content = new StringContent( payload, Encoding.UTF8, "application/json" );
				// The endpoint to publish to
				var publishEndpoint = assistantUrl;
				// Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			// Return the result
			return result;
		}

		/// <summary>
		/// Helper where many types of publishing may occur, and so only need to get the assistant URL once
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public bool PublishRequest( AssistantRequestHelper request )
		{
			string serviceUri = GetAppKeyValue( "registryAssistantApi" );
			request.EndpointUrl = serviceUri + string.Format( "{0}/{1}", request.EndpointType, request.RequestType );

			return PostRequest( request );
		}
		public bool PostRequest( AssistantRequestHelper request )
		{
			RAResponse response = new RAResponse();
			//for a bulk request, a list of Responses will be returned. 
			var listResponse = new List<RAResponse>();
			LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".PostRequest, RequestType: {0}, CTID: {1}, payloadLen: {2}, starts: '{3}' ....", request.RequestType, request.CTID, ( request.InputPayload ?? "" ).Length, request.InputPayload.Substring( 0, request.InputPayload.Length > 200 ? 200 : request.InputPayload.Length ) ) );
			string responseContents = "";
			DateTime started = DateTime.Now;
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					//for initial prototyping, check for OrganizationApiKey
					//not required in the sandbox
					if ( string.IsNullOrWhiteSpace( request.AuthorizationToken ) )
						request.AuthorizationToken = request.OrganizationApiKey;

					if ( !string.IsNullOrWhiteSpace( request.AuthorizationToken ) )
					{
						client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + request.AuthorizationToken );
					}
					else
					{
						//also need to check if a format request
						if ( request.RequestType == "publish" && ( environment == "production" || environment == "staging" ) )
						{
							request.Messages.Add( "Error - an apiKey was not found for the owning organization. The owning organization must be approved in the Credential Engine Accounts site before being able to publish data." );
							return false;
						}
					}

					LoggingHelper.DoTrace( 6, "Publisher.PostRequest: doing PostAsync to: " + request.EndpointUrl );
					var task = client.PostAsync( request.EndpointUrl, new StringContent( request.InputPayload, Encoding.UTF8, "application/json" ) );

					//LoggingHelper.DoTrace( 6, "Publisher.PostRequest: calling task.Wait()" );
					task.Wait();
					//LoggingHelper.DoTrace( 6, "Publisher.PostRequest: getting task.Result" );
					var result = task.Result;
					//LoggingHelper.DoTrace( 6, "Publisher.PostRequest: reading task.Result.Content" );
					responseContents = task.Result.Content.ReadAsStringAsync().Result;

					if ( result.IsSuccessStatusCode == false )
					{
						LoggingHelper.DoTrace( 6, "Publisher.PostRequest: result.IsSuccessStatusCode == false" );
						response = JsonConvert.DeserializeObject<RAResponse>( responseContents );
						//logging???
						//string queryString = GetRequestContext();
						string status = string.Join( ",", response.Messages.ToArray() );
						request.FormattedPayload = response.Payload ?? "";
						request.Messages.AddRange( response.Messages );

						LoggingHelper.DoTrace( 4, thisClassName + string.Format( ".PostRequest() {0} {1} failed: {2}", request.EndpointType, request.RequestType, status ) );
						LoggingHelper.LogError( thisClassName + string.Format( ".PostRequest()  {0} {1}. Failed\n\rMessages: {2}" + "\r\nResponse: " + response + "\n\r" + responseContents + ". payload: " + response.Payload, request.EndpointType, request.RequestType, status ) );
						return false;
					}
					else
					{
						//NEW: for 'bulk' requests, will need to handle a list of responses.
						//FOr example: From PathwaySet/publish or transfervalue/bulkpublish
						//a simple check
						if ( responseContents.IndexOf( "[" ) > 5 )
						{
							//single response
							response = JsonConvert.DeserializeObject<RAResponse>( responseContents );
							//
							if ( response.Successful )
							{
								LoggingHelper.WriteLogFile( 5, request.Identifier + "_payload_Successful.json", response.Payload, "", false );

								request.FormattedPayload = response.Payload;
								request.EnvelopeIdentifier = response.RegistryEnvelopeIdentifier;
								//may have some warnings to display
								request.Messages.AddRange( response.Messages );
							}
							else
							{
								string status = string.Join( ",", response.Messages.ToArray() );
								LoggingHelper.DoTrace( 5, thisClassName + " PostRequest FAILED. result: " + status );
								request.Messages.AddRange( response.Messages );
								request.FormattedPayload = response.Payload;
								return false;
							}
						}
						else
						{
							//list response
							listResponse = JsonConvert.DeserializeObject<List<RAResponse>>( responseContents );
							int cntr = 0;
							foreach ( var lresponse in listResponse )
							{
								cntr++;
								if ( lresponse.Successful )
								{
									LoggingHelper.WriteLogFile( 5, request.Identifier + string.Format( "_{0}_payload_Successful.json", cntr ), lresponse.Payload, "", false );

									//will assume we only want to return the last payload, the pathwaySet for example. 
									if ( cntr == listResponse.Count() )
									{
										request.FormattedPayload = lresponse.Payload;
										request.EnvelopeIdentifier = lresponse.RegistryEnvelopeIdentifier;
									}
									//may have some warnings to display
									request.Messages.AddRange( lresponse.Messages );
								}
								else
								{
									string status = string.Join( ",", lresponse.Messages.ToArray() );
									LoggingHelper.DoTrace( 5, thisClassName + string.Format( " PostRequest #{0} FAILED. result: {1}", cntr, status ) );
									request.Messages.AddRange( lresponse.Messages );
									if ( cntr == listResponse.Count() )
									{
										request.FormattedPayload = lresponse.Payload;
									}
									return false;
								}
							}
						}

					}
					return result.IsSuccessStatusCode;
				}
			}
			catch ( AggregateException ae )
			{
				var timeOutNote = "";
				TimeSpan duration = DateTime.Now.Subtract( started );
				if ( duration.TotalSeconds > 60 )
				{
					timeOutNote = " The call to the API timed out. However, the publish activity may have still been successful!";
				}

				LoggingHelper.LogError( ae, string.Format( "PostRequest.AggregateException. RequestType:{0}, Identifier: {1}", request.RequestType, request.Identifier ) );
				string message = LoggingHelper.FormatExceptions( ae );
				request.Messages.Add( message + timeOutNote );
				return false;
			}
			catch ( Exception exc )
			{
				LoggingHelper.LogError( exc, string.Format( "PostRequest. RequestType:{0}, Identifier: {1}. /n/r responseContents: {2}", request.RequestType, request.Identifier, ( responseContents ?? "empty" ) ) );
				string message = LoggingHelper.FormatExceptions( exc );
				if ( message.IndexOf( "Time out" ) > -1 )
				{
					message = "The request took too long and has timed out waiting for a reply. Your request may still have been successful. Please contact System Administration. ";
				}
				request.Messages.Add( message );
				return false;

			}
			finally
			{
				LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".PostRequest. Exiting." ) );
			}

		}


		#endregion


		#region JSON helpers
		public static JsonSerializerSettings GetJsonSettings()
		{
			var settings = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				ContractResolver = new AlphaNumericContractResolver(),
				Formatting = Formatting.Indented,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};

			return settings;
		}

		//Force properties to be serialized in alphanumeric order
		public class AlphaNumericContractResolver : DefaultContractResolver
		{
			protected override System.Collections.Generic.IList<JsonProperty> CreateProperties( System.Type type, MemberSerialization memberSerialization )
			{
				return base.CreateProperties( type, memberSerialization ).OrderBy( m => m.PropertyName ).ToList();
			}
		}
		/// <summary>
		/// NOTE: previously inherited from AlphaNumericContractResolver. the latter would sort by property name, which we don't want - must be @context, @id, and @graph
		/// </summary>
		public class EmptyNullResolver : DefaultContractResolver
		{
			protected override JsonProperty CreateProperty( MemberInfo member, MemberSerialization memberSerialization )
			{
				var property = base.CreateProperty( member, memberSerialization );
				var isDefaultValueIgnored = ( ( property.DefaultValueHandling ?? DefaultValueHandling.Ignore ) & DefaultValueHandling.Ignore ) != 0;

				if ( isDefaultValueIgnored )
					if ( !typeof( string ).IsAssignableFrom( property.PropertyType ) && typeof( IEnumerable ).IsAssignableFrom( property.PropertyType ) )
					{
						Predicate<object> newShouldSerialize = obj =>
						{
							var collection = property.ValueProvider.GetValue( obj ) as ICollection;
							return collection == null || collection.Count != 0;
						};
						Predicate<object> oldShouldSerialize = property.ShouldSerialize;
						property.ShouldSerialize = oldShouldSerialize != null ? o => oldShouldSerialize( oldShouldSerialize ) && newShouldSerialize( oldShouldSerialize ) : newShouldSerialize;
					}
					else if ( typeof( string ).IsAssignableFrom( property.PropertyType ) )
					{
						Predicate<object> newShouldSerialize = obj =>
						{
							var value = property.ValueProvider.GetValue( obj ) as string;
							return !string.IsNullOrEmpty( value );
						};

						Predicate<object> oldShouldSerialize = property.ShouldSerialize;
						property.ShouldSerialize = oldShouldSerialize != null ? o => oldShouldSerialize( oldShouldSerialize ) && newShouldSerialize( oldShouldSerialize ) : newShouldSerialize;
					}
				return property;
			}
		}
		#endregion
		public class AssistantRequestHelper
		{
			public AssistantRequestHelper()
			{
				//response = new RAResponse();
				Messages = new List<string>();
				OrganizationApiKey = "";
			}
			//input
			public string RequestType { get; set; }
			public string AuthorizationToken { get; set; }

			//when we are ready to really use the apiKey, change to an alias for AuthorizationToken
			public string OrganizationApiKey { get; set; }
			public string CTID { get; set; } //used for deletes
			public string Submitter { get; set; }
			public string InputPayload { get; set; }
			public string EndpointType { get; set; }
			public string EndpointUrl { get; set; }

			public string Identifier { get; set; }
			public string EnvelopeIdentifier { get; set; }

			public string FormattedPayload { get; set; }

			//public string Status { get; set; }
			public List<string> Messages { get; set; }
			//public RAResponse response { get; set; }
		}
		public class RegistryResponseContent
		{
			[JsonProperty( PropertyName = "errors" )]
			public List<string> Errors { get; set; }

			[JsonProperty( PropertyName = "json_schema" )]
			public List<string> JsonSchema { get; set; }

		}
	}
}
