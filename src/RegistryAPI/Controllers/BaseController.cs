using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using System.Web.Http.Description;

using RA.Models;
using RA.Models.Input;
using RA.Services;
using Utilities;

namespace RegistryAPI.Controllers
{
    public class BaseController : ApiController
    {
		public string baseClassName = "BaseController";
		//protected BaseController()
		//{
		//	//string environment = UtilityManager.GetAppKeyValue( "environment" );
		//	//string queryString = GetRequestContext();
		//	//try
		//	//{
		//	//	if ( environment == "staging" && queryString.IndexOf( "//staging.credentialengine.org/assistant" ) > -1 )
		//	//	{
		//	//		queryString = queryString.Replace( "//staging.", "//sandbox." );
		//	//		LoggingHelper.DoTrace( 1, "redirecting from staging to sandbox: " + queryString );
		//	//		Response.Redirect( queryString, true );
		//	//		return;
		//	//	}
		//	//}
		//	//catch ( exception ex )
		//	//{

		//	//}
		//	//catch (Exception ex)
		//	//{

		//	//}

		//}
		private static string GetRequestContext()
		{
			string queryString = "batch";
			try
			{
				queryString = HttpContext.Current.Request.Url.AbsoluteUri.ToString();
			}
			catch ( Exception exc )
			{
				return "";
			}
			return queryString;
		}
		protected string FormatObsoleteEndpoint(string controller, string method)
		{
			string domain = "https://" + Request.RequestUri.Authority;
			string oldpath = "https://" + Request.RequestUri.AbsolutePath;
			string message = string.Format( "The requested endpoint ({0}) is now obsolete. Your request was redirected to the /{1}/{2}V2 endpoint. Please use the latter endpoint for future requests.", oldpath, controller, method) ;

			return message;
		}
		//

		public class JsonResponseMessage : HttpResponseMessage
		{
			public JsonResponseMessage( object data, bool valid, string status, object extra )
			{
				Content = new StringContent( JsonConvert.SerializeObject( new { data = data, valid = valid, status = status, extra = extra } ), Encoding.UTF8, "application/json" );
			}
		}
		//
		[ApiExplorerSettings( IgnoreApi = true )]
		protected static JsonResponseMessage JsonResponse( object data, bool valid, string status, object extra = null )
		{
			return new JsonResponseMessage( data, valid, status, extra );
		}
		//
		/// <summary>
		/// Assign URIs for response
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="response"></param>
		protected static void UpdateResponse( RequestHelper helper, RegistryAssistantResponse response)
		{
			if (helper == null || response == null )
			{
				return;
			}
			response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
			response.GraphUrl = helper.GraphUrl;
			response.EnvelopeUrl = helper.EnvelopeUrl;
		}
		protected static bool ThereIsASystemOutage(ref string message)
		{
			bool outage = false;
			var systemOutageFromDate = UtilityManager.GetAppKeyValue("systemOutageFromDate");
			var systemOutageToDate = UtilityManager.GetAppKeyValue("systemOutageToDate");
			if (!string.IsNullOrWhiteSpace(systemOutageFromDate)
				&& DateTime.TryParse(systemOutageFromDate, out DateTime startdate)
				&& DateTime.TryParse(systemOutageToDate, out DateTime enddate)
				)
			{
				if (System.DateTime.Now > startdate && System.DateTime.Now < enddate)
				{
					message = UtilityManager.GetAppKeyValue("systemOutageMessage");
					//message = "WARNING: The search API is offline for data center maintenance. The API should be available again by approximately 6:00 AM CDT on Saturday, August 31st.";
					return true;
				}
			}

			return outage;
		}

		protected static RegistryAssistantDeleteResponse BaseDeleteOLD(DeleteRequest request, string entityType )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
			try
			{
				if ( request == null || request.CTID == null )
				{
					response.Messages.Add( "Error - please provide a valid delete request." );
					return response;
				}
				RegistryServices cer2 = new RegistryServices( entityType, "", request.CTID );
				isValid = cer2.DeleteRequest( request, entityType, ref messages );
				if ( isValid )
				{
					response.Successful = true;
				}
				else
				{
					response.Messages.AddRange( messages );
					response.Successful = false;
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		} //
		protected RegistryAssistantDeleteResponse BaseDelete(DeleteRequest request, string entityType = "don'tReallyCare??")
		{
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
			string statusMessage = "";
			LoggingHelper.DoTrace( 7, baseClassName + ".BaseDelete - Entered" );
			try
			{
				if ( request == null )
				{
					response.Messages.Add( "Error: A valid Delete Request must be provided." );
					return response;
				}

				string apiKey = "";
				//can the api validation and request validation be done at the same time to minimize 
				if ( !AuthorizationServices.IsAuthTokenValid( true, ref apiKey, ref statusMessage ) )
				{
					response.Messages.Add( "Error: A valid Apikey was not provided in an Authorization Header. " + statusMessage );
					return response;
				}

				if ( string.IsNullOrWhiteSpace( request.CTID ) )
					messages.Add( "The Entity CTID must be provided." );
				//
				if ( string.IsNullOrWhiteSpace( apiKey ) )
					messages.Add( "The Super Publisher Organization apiKey must be provided." );
				//
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
					messages.Add( "The PublishForOrganizationIdentifier (Data Owner CTID) must be provided." );

				var ceApiKey = UtilityManager.GetAppKeyValue( "ceApiKey" );

				if ( messages.Count > 0 )
				{
					response.Successful = false;
					response.Messages.AddRange( messages );
					return response;
				}
				//
				if ( !string.IsNullOrWhiteSpace( request.Community ) )
				{
					if ( request.Community.ToLower() != "navy" )
					{
						if ( request.Community == "true" )
						{
							request.Community = "";
						}
						else
						{
							//just set to default
							LoggingHelper.LogError( "Error - an invalid community was provided: " + request.Community, false );
							request.Community = "";
							//return JsonConvert.SerializeObject( JsonResponse( null, false, "Error - an invalid community was provided: " + community, null ).Data );
							response.Messages.Add( "Error - an invalid community was provided: " + request.Community );
							return response;
						}
					}
					else
					{
						//may want to restrict usage by apiKey (for example)

					}
				}
				else
					request.Community = "";

				request.CTID = request.CTID.ToLower();

				// ===========================================================================
				//do validation
				//basically a valid apiKey associated with valid org approved to publish
				var trxCheck = new SupportServices().ValidateRegistryRequest( apiKey, request.PublishForOrganizationIdentifier, RegistryServices.REGISTRY_ACTION_DELETE, ref messages );
				if ( trxCheck.Successful == false )
				{
					response.Successful = false;
					response.Messages.AddRange( messages );
					return response;
				}
				//
				// =========================================================
				string environment = UtilityManager.GetAppKeyValue( "environment" );
				//get ce key key
				var registryAuthorizationToken = UtilityManager.GetAppKeyValue( "CredentialRegistryAuthorizationToken" );

				var registryEndpoint = UtilityManager.GetAppKeyValue( "CredentialRegistryDeleteEndpoint" );
				//https://credentialengineregistry.org/{0}resources/documents/{1}?purge=false
				if ( !string.IsNullOrWhiteSpace( request.Community ) )
				{
					request.Community += "/";
				}

				//
				//should we first get the record for references?
				string ctdlType = "";
				var ro = new RegistryObject();
				//look up document (good to get for stats)
				//why graph? For large competency frameworks, resource would be better!
				var document = RegistryServices.GetResource( request.CTID, ref ctdlType, ref statusMessage, request.Community );
				if ( document == null || document.Length == 0 || ( document.IndexOf( "\"errors\":" ) > -1 && document.IndexOf( "\"errors\":" ) < 100 ) )
				{
					//need a better check that this - maybe just continue
					//|| document.IndexOf( "\"errors\":" ) > -1
					response.Successful = false;
					response.Messages.Add( "Error: The requested document was not found, so you could say the Delete was successful, or maybe check the input CTID! " + statusMessage );
					return response;
				}
				else
				{
					ro = RegistryServices.GetResourceObject( document );
				}

				// ===========================================================================
				var requestUrl = string.Format( registryEndpoint, request.Community, request.CTID );

				//
				LoggingHelper.DoTrace( 5, string.Format( baseClassName + ".Delete of {0} CTID: {1} from Organization CTID: {2} ; Url: {3} ", ro.CtdlType, request.CTID, request.PublishForOrganizationIdentifier, ro.CtdlId ) );
				using ( var client = new HttpClient() )
				{

					client.DefaultRequestHeaders.Add( "Authorization", "Token " + registryAuthorizationToken );

					//expected format is a delete:
					/*
						DELETE /resources/documents/{ctid}
						DELETE /{community_name}/resources/documents/{ctid}?purge=false
					 * */
					HttpRequestMessage requestMsg = new HttpRequestMessage
					{
						Method = HttpMethod.Delete,
						RequestUri = new Uri( requestUrl )
					};
					var result = client.SendAsync( requestMsg ).Result;
					//result will contain the envelope if successful
					var resultContent = result.Content.ReadAsStringAsync().Result;
					//CHECK result
					//confirm is full envelope returned
					if ( result.IsSuccessStatusCode == false )
					{
						//is this what occurs if the Delete previously done: 
						//	{"errors":["Envelope not found"]}
						LoggingHelper.DoTrace( 5, baseClassName + ".Registry.Delete: Failed " + Environment.NewLine + resultContent + Environment.NewLine + JsonConvert.SerializeObject( resultContent ) );

						response.Successful = false;
						response.Messages.Add( resultContent );
					}
					else
					{
						LoggingHelper.DoTrace( 6, baseClassName + ".Registry.Delete: Succeeded " );
						//add a registry history event for the new owner
						//may want to deserialize to get key data for history
						//this record or something else must signal the correct owner on the next publish.
						string status = "";
						var entityName = ro.Name ?? "Missing".ToString();
						SupportServices mgr = new SupportServices( entityName, request.Community );
						mgr.AddHistory( request.PublishForOrganizationIdentifier.ToLower(), "", RegistryServices.REGISTRY_ACTION_PURGE, ro.CtdlType, ro.CtdlType, request.CTID, "", "", ref status, trxCheck.PublishingOrganizationCTID, true, false );

						//anythihg to return ??
						response.Messages.Add( resultContent );
						response.Successful = true;
					}
				}

			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;

		} //
		/// <summary>
		/// Delete request of an SourceIdentifier by EnvelopeId and CTID
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>

		protected static RegistryAssistantDeleteResponse BaseDeleteByEnvelope(EnvelopeDelete request, string entityType)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
			string statusMessage = "";

			try
			{
				if ( request == null )
				{
					response.Messages.Add( "Error - please provide a valid delete request with the proper structure of: RegistryEnvelopeId, EntityType, CTID, and PublishForOrganizationIdentifier" );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.CTID ) || string.IsNullOrWhiteSpace( request.RegistryEnvelopeId ) )
				{
					response.Messages.Add( "Error - please provide a valid delete request with a CTID, and RegistryEnvelopeId." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( entityType ) )
				{
					response.Messages.Add( "Error - please provide a valid delete request that includes EntityType." );
					return response;
				}
				RA.Models.RequestHelper helper = new RequestHelper();
				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage, true ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					RegistryServices cer = new RegistryServices( entityType, "", request.CTID );
					isValid = cer.CredentialRegistry_SelfManagedKeysDelete( request, "registry assistant", ref statusMessage );

					response.Successful = isValid;

					if ( isValid )
					{
						response.Successful = true;
					}
					else
					{
						//if not valid, could return the payload as reference?
						response.Messages.Add( statusMessage );
						response.Successful = false;
					}
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		} //
	}

}
