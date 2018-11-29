using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Utilities;
using RA.Services;
using Services = RA.Services.ServiceHelperV2;
using RA.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace RegistryAPI.Controllers
{
    public class SearchController : BaseController
    {
        string thisClassName = "SearchController";
		Services services = new Services();

		[HttpGet, Route( "search/tempsearch" )]
		public string TempSearch()
		{
			//Test server
			Request.Headers.Add( "Authorization", "ApiToken b1fd3f1d-d0ac-472c-99ee-e4f234036e2d" );
			var data = Ctdl( new SearchRequest()
			{
				Skip = 0,
				Take = 10,
				Query = JObject.Parse( "{ \"@type\": [\"ceterms:Certificate\"] }" )
			} );

			return JsonConvert.SerializeObject( data );
		}
		//

		/// <summary>
		/// Search endpoint expecting query in CTDL Json Format
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "search/ctdl" )]
		public JsonResponseMessage Ctdl( SearchRequest request )
		{
			//Cast the Query to a JObject
			var requestQuery = new JObject();
			try
			{
				requestQuery = ( JObject ) request.Query;
				if ( requestQuery == null || requestQuery.ToString() == "{}" )
				{
					throw new Exception();
				}
			}
			catch
			{
				return JsonResponse( null, false, "Error: Please provide a well-formatted CTDL JSON query.", null );
			}

			//Enable tracking query data for statistics (this is also done in the accounts system)
			var queryTargetTypes = GremlinServices.GetQueryMainTargetTypes( requestQuery );

			//Clamp page number and size
			request.Skip = request.Skip < 0 ? 0 : request.Skip;
			request.Take = request.Take < 0 ? 5 : request.Take > 100 ? 100 : request.Take;

			//Prepare the query
			var rawContextSets = GremlinServices.GetCTDLAndCTDLASNContexts();
			var context = new GremlinServices.GremlinContext( true, rawContextSets );
			var gremlinQuery = GremlinServices.CTDLQueryToGremlinQuery( requestQuery, request.Skip, request.Take, context );
			var consumeRequest = new AccountConsumeRequest()
			{
				Skip = request.Skip,
				Take = request.Take,
				CTDLQuery = requestQuery,
				GremlinQuery = gremlinQuery
			};

			//Do the search
			return Search( consumeRequest );
		}
		//

		/// <summary>
		/// Search endpoint expecting query in Gremlin Query String format
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route("search/gremlin")]
		public JsonResponseMessage Gremlin( SearchRequest request )
		{
			//Cast the Query to a string
			var requestQuery = "";
			try
			{
				requestQuery = (( string ) request.Query).Replace( "\n", "" );
				if ( string.IsNullOrWhiteSpace( requestQuery ) || requestQuery.IndexOf( "g.V()" ) != 0 )
				{
					throw new Exception();
				}
			}
			catch
			{
				return JsonResponse( null, false, "Error: Please provide a well-formatted Gremlin query.", null );
			}

			//Can't track statistics data
			//Can't clamp page number and size

			//Prepare the query
			var consumeRequest = new AccountConsumeRequest()
			{
				GremlinQuery = requestQuery,
				IsGremlinOnly = true
			};

			return Search( consumeRequest );
		}
		//

		private JsonResponseMessage Search( AccountConsumeRequest consumeRequest )
		{
			try
			{
				//The bulk of the search process is the same regardless of CTDL JSON or Raw Gremlin
				var client = new HttpClient();
				HttpResponseMessage result;
				var totalResults = 0;

				//Handle the query based on whether or not we are in the sandbox (determined by requiring API key)
				var requiringAPIKey = System.Configuration.ConfigurationManager.AppSettings[ "requiringHeaderToken" ] == "true";

				//Do the search via the accounts system
				if ( requiringAPIKey )
				{
					//Get API key
					//Note - the search API in the accounts system expects there to be an API key (this is how it looks up the organization), and will fail if it is empty/null
					var apiKeyHeader = HttpContext.Current.Request.Headers[ "Authorization" ] ?? "";
					var authorizationParts = apiKeyHeader.ToLower().Split( ' ' );
					var authorizationType = authorizationParts.Count() == 2 ? authorizationParts[ 0 ] : null;
					var apiKey = authorizationParts.Count() == 2 ? authorizationParts[ 1 ] : null;
					consumeRequest.ApiKey = apiKey;
					if ( string.IsNullOrWhiteSpace( apiKeyHeader ) || string.IsNullOrWhiteSpace( authorizationType ) || string.IsNullOrWhiteSpace( apiKey ) )
					{
						return JsonResponse( null, false, "Error: You must provide an API key via the Authorization header using the format: Authorization: ApiToken [YOUR API KEY]", null );
					}

					//Do the search via the accounts system
					var searchAPIURL = UtilityManager.GetAppKeyValue( "accountsSearchApi" );
					result = client.PostAsync( searchAPIURL, new StringContent( JsonConvert.SerializeObject( consumeRequest ), Encoding.UTF8, "application/json" ) ).Result;
					var resultContent = result.Content.ReadAsStringAsync().Result;
					var resultData = GremlinServices.JsonStringToDictionary( resultContent );

					//Handle the result
					if ( ( bool ) resultData[ "valid" ] == true )
					{
						//Return the results
						var resultJson = GremlinServices.GremlinResponseToDictionarySearchResults( (string) resultData[ "data" ], ref totalResults );
						return JsonResponse( resultJson, true, "okay", new { TotalResults = totalResults, GremlinQuery = consumeRequest.GremlinQuery } );
					}
					else
					{
						//Forward the error
						return JsonResponse( resultData[ "data" ], ( bool ) resultData[ "valid" ], ( string ) resultData[ "status" ], resultData[ "extra" ] );
					}
				}
				//Query against the Registry directly
				else
				{
					//Do the search via the registry directly, using Credential Engine's keys
					var searchAPIURL = UtilityManager.GetAppKeyValue( "GremlinSearchEndpoint" );
					var registryAuthorizationToken = UtilityManager.GetAppKeyValue( "CredentialRegistryAuthorizationToken" ); //Admin-level access to the registry for CE's account
					client.DefaultRequestHeaders.TryAddWithoutValidation( "Authorization", "ApiToken " + registryAuthorizationToken );
					result = client.PostAsync( searchAPIURL, new StringContent( "{ \"gremlin\": \"" + consumeRequest.GremlinQuery + "\" }", Encoding.UTF8, "application/json" ) ).Result;
					var resultContent = result.Content.ReadAsStringAsync().Result;
					var resultData = GremlinServices.GremlinResponseToDictionarySearchResults( resultContent, ref totalResults );

					//Handle the result
					if ( result.IsSuccessStatusCode )
					{
						//Return the results
						return JsonResponse( resultData, true, "okay", new { TotalResults = totalResults, GremlinQuery = consumeRequest.GremlinQuery } );
					}
					else
					{
						//Forward the error
						return JsonResponse( null, false, "Error: " + result.ReasonPhrase, null );
					}
				}

			}
			catch ( Exception ex )
			{
				return JsonResponse( null, false, ex.Message, null );
			}
		}
		//

    }
    public class SearchRequest
    {
		public int Skip { get; set; }
		public int Take { get; set; }
		public JToken Query { get; set; }
    }
   
    public class RequestMessage
    {
        public string Message { get; set; }
        public bool IsWarning { get; set; }
    }
    public class ApiResponse
    {
        public ApiResponse()
        {
            Messages = new List<string>();
            Payload = "";
        }
        public bool Successful { get; set; }

        public List<string> Messages { get; set; }

        /// <summary>
        /// Payload of request to registry, containing properties formatted as CTDL - JSON-LD
        /// </summary>
        public string Payload { get; set; }
		public int TotalResults { get; set; }
		public object Results { get; set; }
    }
}
