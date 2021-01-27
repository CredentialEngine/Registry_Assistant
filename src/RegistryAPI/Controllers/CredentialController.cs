using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Description;
//using System.Web.Mvc;

using System.Web.Http;
using RA.Models;
using RA.Models.Input;
using Newtonsoft.Json;
using RA.Services;
using ServiceHelper = RA.Services.ServiceHelperV2;
using MgrV2 = RA.Services.CredentialServicesV2;

using Utilities;
namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Registry Assistant for Credentials
	/// </summary>
    public class CredentialController : BaseController
	{
		string thisClassName = "CredentialController";
		string controllerEntity = "Credential";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();

		string env = UtilityManager.GetAppKeyValue("environment");
        MgrV2 mgr = new MgrV2();

		  /// <summary>
		  /// Handle request to format a Credential document as CTDL Json-LD
		  /// </summary>
		  /// <param name="request"></param>
		  /// <returns></returns>
		[HttpPost, Route( "credential/format" )]
        public RegistryAssistantFormatResponse Format( CredentialRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantFormatResponse();
			string statusMessage = "";
			if ( ThereIsASystemOutage( ref statusMessage ) )
			{
				response.Messages.Add( statusMessage );
				return response;
			}
			try
            {
                if ( request == null || request.Credential == null )
                {
                    response.Messages.Add( "Error - please provide a valid credential request." );
                    return response;
                }
				if ( string.IsNullOrWhiteSpace( request.Credential.Ctid ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for the Credential." );
					return response;
				}
				helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Credential.Ctid, "Credential", "Format" );
				string originalCTID = request.Credential.Ctid ?? "";
				mgr = new MgrV2();

				mgr.FormatAsJson( request, helper, ref isValid );
				new SupportServices().AddActivityForFormat( helper, "Credential", mgr.CurrentEntityName, request.Credential.Ctid, ref statusMessage );

				response.Payload = helper.Payload;
				response.Successful = isValid;
                if ( isValid )
                {
                    response.Successful = true;
					//check for any (likely warnings) messages
					if ( helper.Messages.Count > 0 )
						response.Messages = helper.GetAllMessages();
					//response.CTID = request.Credential.Ctid;
					//if ( response.CTID != originalCTID )
					//{
					//    response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this credential. If not provided, the future request will be treated as a new credential." );
					//}
				}
                else
                {
					response.Messages = helper.GetAllMessages();
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
		/// Publish a credential to the Credential Engine Registry using an @graph object and language maps. 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route("credential/publish")]
		public RegistryAssistantResponse Publish( CredentialRequest request )
        {
			
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            string statusMessage = "";
			if ( ThereIsASystemOutage( ref statusMessage ) )
			{
				response.Messages.Add( statusMessage );
				return response;
			}
			//	response.Messages.Add(statusMessage);
			//	return response;
			//}

			try
            {
                if ( request == null || request.Credential == null )
                {
                    response.Messages.Add("Error - please provide a valid credential request.");
                    return response;
                }
				if ( string.IsNullOrWhiteSpace( request.Credential.Ctid ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for the Credential." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace(request.PublishForOrganizationIdentifier)  )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace(2, string.Format("RegistryAssistant.{0}.PublishRrequest. IPaddress: {1}, ctid: {2}, owningOrgCTID:{3}, envelopeId: {4}", thisClassName, ServiceHelper.GetCurrentIP(), request.Credential.Ctid, request.PublishForOrganizationIdentifier, request.RegistryEnvelopeId));

                helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
                if ( !new AuthorizationServices().ValidateRequest(helper, ref statusMessage) )
                {
                    response.Messages.Add(statusMessage);
                }
                else
                {
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Credential.Ctid, "Credential", "Publish", 5 );
					string originalCTID = request.Credential.Ctid ?? "";

                    mgr.Publish(request, ref isValid, helper);

                    response.CTID = request.Credential.Ctid.ToLower();
                    response.Payload = helper.Payload;
                    response.Successful = isValid;

                    if ( isValid )
                    {
						if (helper.Messages.Count > 0)
							response.Messages = helper.GetAllMessages();

						UpdateResponse(helper, response);

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Credential", response.CTID);
                    }
                    else
                    {
                        //if not valid, could return the payload as reference?
                        //response.Messages = messages;
                        response.Messages = helper.GetAllMessages();
                    }
                }
            }
            catch ( Exception ex )
            {
                response.Messages.Add(ex.Message);
                response.Successful = false;
            }
            return response;
        } //


		/// <summary>
		/// Use Request.Content to more easily handle larger frameworks
		/// </summary>
		/// <returns></returns>
		[ApiExplorerSettings( IgnoreApi = true )]
		[HttpPost, Route( "credential/publishGraph" )]
		public RegistryAssistantResponse PublishGraph()
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();

			HttpContent requestContent = Request.Content;
			string jsonContent = requestContent.ReadAsStringAsync().Result;
			var request = JsonConvert.DeserializeObject<GraphContentRequest>( jsonContent );
			string statusMessage = "";

			try
			{
				if ( UtilityManager.GetAppKeyValue( "environment" ) != "environment")
				{
					response.Messages.Add( "Error - this feature is under development, and cannot be accessed outside the latter." );
					return response;
				}

				if ( request == null || request.GraphInput == null )
				{
					response.Messages.Add( "Error - please provide a valid Credential request with the JSON Graph." );
					return response;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, PublishForOrganizationIdentifier: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.PublishForOrganizationIdentifier, request.RegistryEnvelopeId ) );
				helper = new RequestHelper();
				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					string outputCTID = "";
					//actually need the serializedInput to write to history
					helper.SerializedInput = JsonConvert.SerializeObject( request, ServiceHelper.GetJsonSettings() );
					mgr.PublishGraph( request, ref isValid, helper, ref outputCTID );

					//****as there is no CTID in input, write file with returned ctid
					ServiceHelper.LogInputFile( request, outputCTID, "Credential", "PublishGraph", 5 );

					response.CTID = outputCTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if( isValid )
					{
						if( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();
						UpdateResponse( helper, response );

					}
					else
					{
						response.Messages = helper.GetAllMessages();
					}
				}
			}
			catch( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		}
		/// <summary>
		/// Publish a list of credentials to the Credential Engine Registry. Current maximum requests are 10.
		/// NOTE: EXPERIMENTAL, ONLY AVAILABLE IN THE SANDBOX ENVIROMENT
		/// </summary>
		/// <param name="requests"></param>
		/// <returns></returns>
		[ApiExplorerSettings( IgnoreApi = true )]
		[HttpPost, Route( "credential/bulkpublish" )]
		public List<RegistryAssistantResponse> BulkPublish(BulkCredentialRequest bulkRequest )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var responses = new List<RegistryAssistantResponse>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";
			if ( env == "production" )
			{
				response.Messages.Add( "Error - this endpoint is not valid in this environment." );
				responses.Add( response );
				return responses;
			}
			//
			try
			{
				if ( bulkRequest == null || bulkRequest.Credentials == null || bulkRequest.Credentials.Count == 0 )
				{
					response.Messages.Add( "Error - please provide a valid list of credential requests." );
					responses.Add( response );
					return responses;
				} else if ( bulkRequest.Credentials.Count > 10)
				{
					response.Messages.Add( "Error - Currently only a maximum of 10 requests are allowed for a bulk publish." );
					responses.Add( response );
					return responses;
				}
				int cntr = 0;
				int maxCredentials = UtilityManager.GetAppKeyValue( "maxRecordsForBulkPublish", 10);
				//extract common stuff
				var ownerCtid = bulkRequest.PublishForOrganizationIdentifier;
				//store stuff from original credential in mgr, so don't have to pass
				//or populate credential.Credential from current list item!
				mgr = new MgrV2();
				//downside is the envelopeId is outside the credential, so can't include!
				foreach ( var credential in bulkRequest.Credentials )
				{
					cntr++;
					if( cntr > maxCredentials )
					{
						response.Messages.Add( string.Format( "Error - The Bulk Publish only allows {0} at this time. Any additional credentials are ignored.", maxCredentials ) );
						responses.Add( response );
						break;
						//return responses;
					}
					response = new RegistryAssistantResponse();
					if ( credential == null || credential == null )
					{
						response.Messages.Add( string.Format("Error - credential #{0} is not a valid credential credential.", cntr) );
						responses.Add( response );
						continue;
						//return responses;
					}

					LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.BulkPublish. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), credential.Ctid, bulkRequest.RegistryEnvelopeId ) );

					helper = new RequestHelper
					{
						OwnerCtid = bulkRequest.PublishForOrganizationIdentifier
					};
					//this only needs to be done once
					if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
					{
						response.Messages.Add( statusMessage );
					}
					else
					{
						helper.SerializedInput = ServiceHelper.LogInputFile( credential, credential.Ctid, "Credential", "Publish", 5 );
						string originalCTID = credential.Ctid ?? "";
						var request = new CredentialRequest()
						{
							Credential = credential,
							RegistryEnvelopeId = "",
							Community = bulkRequest.Community,
							PublishForOrganizationIdentifier = bulkRequest.PublishForOrganizationIdentifier
						};
						mgr.Publish( request, ref isValid, helper );

						response.CTID = credential.Ctid.ToLower();
						response.Payload = helper.Payload;
						response.Successful = isValid;

						if ( isValid )
						{
							if ( helper.Messages.Count > 0 )
								response.Messages = helper.GetAllMessages();
							UpdateResponse(helper, response);

							response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Credential", response.CTID);
							
						}
						else
						{
							//if not valid, could return the payload as reference?
							//response.Messages = messages;
							response.Messages = helper.GetAllMessages();
						}
						
					} //
					responses.Add( response );
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
				responses.Add( response );
			}
			return responses;
		} //
		[ApiExplorerSettings( IgnoreApi = true )]
		[HttpPost, Route( "credential/bulkpublish2" )]
		private List<RegistryAssistantResponse> BulkPublish2( List<CredentialRequest> requests )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var responses = new List<RegistryAssistantResponse>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";
			if ( env == "production" )
			{
				response.Messages.Add( "Error - this endpoint is not valid in this environment." );
				responses.Add( response );
				return responses;
			}
			//CredentialRequest request = new CredentialRequest();
			try
			{
				if ( requests == null || requests.Count == 0 )
				{
					response.Messages.Add( "Error - please provide a valid list of credential requests." );
					responses.Add( response );
					return responses;
				}
				else if ( requests.Count > 10 )
				{
					response.Messages.Add( "Error - Currently only a maximum of 10 requests are allowed for a bulk publish." );
					responses.Add( response );
					return responses;
				}
				int cntr = 0;
				foreach ( var request in requests )
				{
					cntr++;
					response = new RegistryAssistantResponse();
					helper = new RequestHelper();
					if ( request == null || request.Credential == null )
					{
						response.Messages.Add( string.Format( "Error - request #{0} is not a valid credential request.", cntr ) );
						responses.Add( response );
						continue;
						//return responses;
					}

					LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.BulkPublish. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Credential.Ctid, request.RegistryEnvelopeId ) );

					helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
					if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
					{
						response.Messages.Add( statusMessage );
					}
					else
					{
						helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Credential.Ctid, "Credential", "Publish", 5 );
						string originalCTID = request.Credential.Ctid ?? "";

						mgr.Publish( request, ref isValid, helper );

						response.CTID = request.Credential.Ctid.ToLower();
						response.Payload = helper.Payload;
						response.Successful = isValid;

						if ( isValid )
						{
							if ( helper.Messages.Count > 0 )
								response.Messages = helper.GetAllMessages();
							UpdateResponse(helper, response);

							//response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
							//response.EnvelopeUrl = mgr.credentialRegistryBaseUrl + "envelopes/" + response.RegistryEnvelopeIdentifier;
							response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Credential", response.CTID);
							
						}
						else
						{
							//if not valid, could return the payload as reference?
							//response.Messages = messages;
							response.Messages = helper.GetAllMessages();
						}
						responses.Add( response );
					} //
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
				responses.Add( response );
			}
			return responses;
		} //
		  /// <summary>
		  /// Use to replace (delete and add) a document created with the CER keys with the registry managed keys.
		  /// </summary>
		  /// <param name="request">Will need to include an envelope id</param>
		  /// <returns></returns>
		[ApiExplorerSettings( IgnoreApi = true )]
		[HttpPost, Route( "credential/replace" )]
		private RegistryAssistantResponse Replace( CredentialRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            string statusMessage = "";

            try
            {
                if (request == null || request.Credential == null)
                {
                    response.Messages.Add( "Error - please provide a valid credential request." );
                    return response;
                } else 
                if ( string.IsNullOrWhiteSpace( request.Credential.Ctid)
                    || string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier )
                    || string.IsNullOrWhiteSpace( request.RegistryEnvelopeId )
                    )
                {
                    response.Messages.Add( "Error - please provide a valid delete request with a CTID, the envelope Id, and the owning organization." );
                    return response;
                }

                LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Credential.Ctid, request.RegistryEnvelopeId ) );

                helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
                //API key will be required!
                if (!new AuthorizationServices().ValidateRequest( helper, ref statusMessage ))
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
					EnvelopeDelete deleteRequest = new EnvelopeDelete
					{
						CTID = request.Credential.Ctid,
						PublishForOrganizationIdentifier = request.PublishForOrganizationIdentifier,
						RegistryEnvelopeId = request.RegistryEnvelopeId
					};

					var resp = CustomDelete( deleteRequest );
					if ( !resp.Successful )
					{
						response.Messages = resp.Messages;
						return response;
					}

					//now publish
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Credential.Ctid, "Credential", "Replace", 5 );
					string originalCTID = request.Credential.Ctid ?? "";

					mgr.Publish( request, ref isValid, helper );

                    //CredentialServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );

                    response.CTID = request.Credential.Ctid;
                    response.Payload = helper.Payload;
                    response.Successful = isValid;

                    if (isValid)
                    {
                        response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
                        if (helper.Messages.Count > 0)
                            response.Messages = helper.GetAllMessages();
                        response.CTID = request.Credential.Ctid;

                        if (response.CTID != originalCTID)
                        {
                            response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this credential. If not provided, the future request will be treated as a new credential." );
                        }
                    }
                    else
                    {
                        //if not valid, could return the payload as reference?
                        //response.Messages = messages;
                        response.Messages = helper.GetAllMessages();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Messages.Add( ex.Message );
                response.Successful = false;
            }
            return response;
        } //

		/// <summary>
		/// Delete request of a credential by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "credential/delete" )]
		public RegistryAssistantDeleteResponse Delete(DeleteRequest request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
			try
			{
				if ( request == null || request.CTID == null )
				{
					response.Messages.Add( "Error - please provide a valid delete request with a valid CTID." );
					return response;
				}
				//Why have entity type twice????
				//the first one is instantitated in class, so available to all methods
				RegistryServices cer2 = new RegistryServices( controllerEntity, "", request.CTID );
				isValid = cer2.DeleteRequest( request, controllerEntity, ref messages );
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

		/// <summary>
		/// Delete request of a credential by EnvelopeId and CTID
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "credential/envelopeDelete" )]
		public RegistryAssistantDeleteResponse CustomDelete(EnvelopeDelete request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
			string statusMessage = "";

			try
			{
				//envelopeId is not applicable for managed keys!, could have a separate endpoint?
				if ( request == null
					|| string.IsNullOrWhiteSpace( request.CTID )
					|| string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier )
					|| string.IsNullOrWhiteSpace( request.RegistryEnvelopeId )
					)
				{
					response.Messages.Add( "Error - please provide a valid delete request with a CTID, the envelope Id, and the owning organization." );
					return response;
				}
				//ACTUALLY not used directly here, will need mechanism to ensure a valid request. 
				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage, true ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					//only valid token at this time is from CER
					RegistryServices cer = new RegistryServices( controllerEntity, "", request.CTID );
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