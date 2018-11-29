using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Utilities;
using RA.Services;
using Services = RA.Services.ServiceHelperV2;
using RA.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;

namespace RegistryAPI.Controllers
{
    public class SearchController : Controller
    {
        string thisClassName = "SearchController";
		Services services = new Services();

		[Route( "search/tempsearch" )]
		public string TempSearch()
		{
			//Test server
			Request.Headers.Add( "Authorization", "ApiToken b1fd3f1d-d0ac-472c-99ee-e4f234036e2d" );
			var data = Ctdl( new SearchRequest()
			{
				Skip = 0,
				Take = 10,
				Query = "{ \"@type\": [\"ceterms:Certificate\"] }"
			} );

			return JsonConvert.SerializeObject( data );
		}
		//

		/// <summary>
		/// Search endpoint expecting query in CTDL Json Format
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[Route( "search/ctdl" )]
		public JsonResult Ctdl( SearchRequest request )
		{
			try
			{
				//Ensure query exists
				if ( request == null )
				{
					return JsonHelper.GetJsonWithWrapper( null, false, "Error: You must provide a valid search request.", null );
				}

				//Track query data for statistics
				var queryJson = GremlinServices.JsonStringToDictionary( request.Query );
				var queryTargetTypes = GremlinServices.GetQueryMainTargetTypes( request.Query );

				//Clamp page number and size
				request.Skip = request.Skip < 0 ? 0 : request.Skip;
				request.Take = request.Take < 0 ? 5 : request.Take > 100 ? 100 : request.Take;

				//Get API key
				//Note - the search API in the accounts system expects there to be an API key (this is how it looks up the organization), and will fail if it is empty/null
				var apiKeyHeader = Request.Headers[ "Authorization" ] ?? "";
				var authorizationParts = apiKeyHeader.ToLower().Split( ' ' );
				var authorizationType = authorizationParts.Count() == 2 ? authorizationParts[ 0 ] : null;
				var apiKey = authorizationParts.Count() == 2 ? authorizationParts[ 1 ] : null;
				if ( string.IsNullOrWhiteSpace( apiKeyHeader ) || string.IsNullOrWhiteSpace( authorizationType ) || string.IsNullOrWhiteSpace( apiKey ) )
				{
					return JsonHelper.GetJsonWithWrapper( null, false, "Error: You must provide an API key via the Authorization header using the format: Authorization: ApiToken [YOUR API KEY]", null );
				}

				//Prepare the search
				var rawContextSets = GremlinServices.GetCTDLAndCTDLASNContexts();
				var context = new GremlinServices.GremlinContext( true, rawContextSets );
				var gremlinQuery = GremlinServices.CTDLQueryToGremlinQuery( request.Query, request.Skip, request.Take, context );
				var searchAPIURL = UtilityManager.GetAppKeyValue( "accountsSearchApi" );
				var consumeRequest = new AccountConsumeRequest()
				{
					ApiKey = apiKey,
					Skip = request.Skip,
					Take = request.Take,
					CTDLQuery = request.Query,
					GremlinQuery = gremlinQuery
				};

				//Do the search
				var client = new HttpClient();
				var result = client.PostAsync( searchAPIURL, new StringContent( JsonConvert.SerializeObject( consumeRequest ), Encoding.UTF8, "application/json" ) ).Result;
				var resultContent = result.Content.ReadAsStringAsync().Result;
				var totalResults = 0;
				var resultData = GremlinServices.JsonStringToDictionary( resultContent );
				if ( ( bool ) resultData[ "valid" ] == false )
				{
					//Forward the error
					return JsonHelper.GetJsonWithWrapper( resultData[ "data" ], ( bool ) resultData[ "valid" ], ( string ) resultData[ "status" ], resultData[ "extra" ] );
				}
				else
				{
					//Return the results
					var resultJson = GremlinServices.GremlinResponseToDictionarySearchResults( (string) resultData[ "data" ], ref totalResults );
					return JsonHelper.GetJsonWithWrapper( resultJson, result.IsSuccessStatusCode, "", new { TotalResults = totalResults, GremlinQuery = gremlinQuery } );
				}
			}
			catch ( Exception ex )
			{
				return JsonHelper.GetJsonWithWrapper( null, false, ex.Message, null );
			}
		}



		/// <summary>
		/// Search endpoint expecting Gremlin formatted query
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "search/graph" )]
        public JsonResult GraphSearch( SearchRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new ApiResponse();
            string statusMessage = "";
            RA.Models.RequestHelper helper = new RequestHelper();

            try
            {
                if ( request == null )
                {
					return JsonHelper.GetJsonWithWrapper( null, false, "Error: You must provide a valid search request.", null );
				}

                if ( !ValidateRequest( request, helper, ref messages ) )
                {
                    response.Messages = messages;
					return JsonHelper.GetJsonWithWrapper( null, false, string.Join( ",", messages.ToArray() ), null );
				}
                else
                {
					//Clamp page number and size
					request.Skip = request.Skip < 0 ? 0 : request.Skip;
					request.Take = request.Take < 0 ? 5 : request.Take > 100 ? 100 : request.Take;

					//Prepare the search
					var rawContextSets = GremlinServices.GetCTDLAndCTDLASNContexts();
					var context = new GremlinServices.GremlinContext( true, rawContextSets );
					var gremlinQuery = GremlinServices.CTDLQueryToGremlinQuery( request.Query, request.Skip, request.Take, context );
					var searchAPIURL = UtilityManager.GetAppKeyValue( "accountsSearchApi" );
					var consumeRequest = new AccountConsumeRequest()
					{
						ApiKey = helper.ApiKey,
						Skip = request.Skip,
						Take = request.Take,
						CTDLQuery = request.Query,
						GremlinQuery = gremlinQuery
					};

					//Do the search
					var client = new HttpClient();
					var result = client.PostAsync( searchAPIURL, new StringContent( JsonConvert.SerializeObject( consumeRequest ), Encoding.UTF8, "application/json" ) ).Result;
					var resultContent = result.Content.ReadAsStringAsync().Result;
					var totalResults = 0;
					var resultData = GremlinServices.JsonStringToDictionary( resultContent );
					if ( ( bool )resultData[ "valid" ] == false )
					{
						//Forward the error
						return JsonHelper.GetJsonWithWrapper( resultData[ "data" ], ( bool )resultData[ "valid" ], ( string )resultData[ "status" ], resultData[ "extra" ] );
					}
					else
					{
						//Return the results
						var resultJson = GremlinServices.GremlinResponseToDictionarySearchResults( ( string )resultData[ "data" ], ref totalResults );
						return JsonHelper.GetJsonWithWrapper( resultJson, result.IsSuccessStatusCode, "", new { TotalResults = totalResults, GremlinQuery = gremlinQuery } );
					}


					//old
					//do the search request
					//response.Payload = PostRequests( request, helper.ApiKey, ref isValid, ref statusMessage );
     //               response.Successful = isValid;

     //               if ( isValid )
     //               {
     //                   if ( helper.Messages.Count > 0 )
     //                       response.Messages = helper.GetAllMessages();
     //               }
     //               else
     //               {
     //                   //if not valid, could return the payload as reference?
     //                   //response.Messages = messages;
     //                   response.Messages = helper.GetAllMessages();
     //               }
                }
            }
            catch ( Exception ex )
            {
                //response.Messages.Add( ex.Message );
                //response.Successful = false;
				return JsonHelper.GetJsonWithWrapper( null, false, ex.Message, null );
			}

            //return response;
        }


        /// <summary>
        /// The actual validation will be via a call to the accounts api
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        private bool ValidateRequest( SearchRequest request, RequestHelper helper, ref List<string> messages, bool isDeleteRequest = false )
        {
            bool isValid = true;
            string clientIdentifier = "";


			bool isTokenRequired = UtilityManager.GetAppKeyValue( "requiringHeaderToken", true );
            if ( isDeleteRequest )
                isTokenRequired = true;

            //api key will be passed in the header
            string apiToken = "";
			string statusMessage = "";

			if ( RA.Services.ServiceHelper.IsAuthTokenValid( isTokenRequired, ref apiToken, ref clientIdentifier, ref statusMessage ) == false )
            {
				messages.Add( statusMessage );
                return false;
            }
            helper.ApiKey = apiToken;
            helper.ClientIdentifier = clientIdentifier ?? "";

            if ( isTokenRequired )
            {
                if ( clientIdentifier.ToLower().StartsWith( "cerpublisher" ) == false )
                {
                    messages.Add( "Error - a valid CTID for the related organization must be provided.");
                    return false;
                }
            }

			//if ( string.IsNullOrWhiteSpace( request.OrganizationIdentifier ) )
			//{
			//	messages.Add( "Error - a valid Organization Identifier must be provided.");
			//	return false;
			//} else if ( !services.IsCtidValid( request.OrganizationIdentifier, ref messages ) )

			if ( string.IsNullOrWhiteSpace(request.Query) )
			{
				messages.Add( "Error - a valid query must be provided.");
			}

			if ( request.Skip < 0 )
			{
				request.Skip = 0;
			}

			if ( messages.Count > 0 )
				return false;

			return isValid;
        }

        private string PostRequests ( SearchRequest request,
                string apiKey,
                ref bool valid,
                ref string status)
        {
            valid = true;
            List<string> messages = new List<string>();
			AccountConsumeRequest apr = new AccountConsumeRequest
			{
				ApiKey = apiKey,
				Skip = request.Skip,
				Take = request.Take,
				CTDLQuery = request.Query
			};

			//already serialized!
			string postBody = JsonConvert.SerializeObject( apr );

            //get accounts url
            string serviceUri = UtilityManager.GetAppKeyValue( "accountsSearchApi" );       
            string contents = "";

            try
            {
                using ( var client = new HttpClient() )
                {
                    client.DefaultRequestHeaders.
                        Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
 
                    var task = client.PostAsync( serviceUri,
                        new StringContent( postBody, Encoding.UTF8, "application/json" ) );
                    task.Wait();
                    var response = task.Result;
                    //should get envelope_id from contents?
                    //the accounts endpoint will return the registry response verbatim 
                    contents = task.Result.Content.ReadAsStringAsync().Result;

                    if ( response.IsSuccessStatusCode == false )
                    {
                        //note the accounts publish will always return successful otherwise any error messages get lost
                        if ( contents.ToLower().IndexOf( "accountresponse" ) > 0 )
                        {
                            AccountConsumeResponse acctResponse = JsonConvert.DeserializeObject<AccountConsumeResponse>( contents );
                            if ( acctResponse.Messages != null && acctResponse.Messages.Count > 0 )
                            {
                                status = string.Join( ",", acctResponse.Messages.ToArray() );
                                messages.AddRange( acctResponse.Messages );
                            }
                        }
                    
                        else
                        {
                            status = contents;
                            messages.Add( status );
                        }
                        //
                        valid = false;
                        string queryString = RegistryServices.GetRequestContext();
                        //now null:
                        //+ "\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )
                        LoggingHelper.LogError(   " RegistryServices.Consuming Failed:"
                            + "\n\rURL:\n\r " + queryString
                            + "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject( response )
                            + "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject( contents ),
                            false, "CredentialRegistry consuming failed for "  );
                   
                        LoggingHelper.WriteLogFile( 5,  "_query_failed.json", request.Query, "", false );
                    }
                    else
                    {
                        //accounts publisher can return errors like: {"data":null,"valid":false,"status":"Error: Owning organization not found.","extra":null}
                        if ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
                        {
                            valid = false;
                            status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
                            messages.Add( status );

                        }
                       
                        else
                        {
                            valid = true;//maybe
                            LoggingHelper.DoTrace( 7, "contents after successful search.\r\n" + contents );
                        }

                    }

                    return contents;
                }
            }
            catch ( Exception ex )
            {

                LoggingHelper.LogError( ex, "SearchController.PostRequests: " + contents );
                valid = false;
                status = "Failed on Registry Consuming: " + LoggingHelper.FormatExceptions( ex );
                messages.Add( status );
                return status;
            }

        }
		
    }
    public class SearchRequest
    {
		public int Skip { get; set; }
		public int Take { get; set; }
		public string Query { get; set; }
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
