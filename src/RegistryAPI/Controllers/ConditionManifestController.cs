using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;

using System.Web.Http;
using RA.Models;
using RA.Models.Input;
using Newtonsoft.Json;
using RA.Services;
using Mgr = RA.Services.ConditionManifestServicesV2;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;
namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Registry Assistant for Condition Manifests
	/// </summary>
	public class ConditionManifestController : BaseController
	{
		string thisClassName = "ConditionManifestController";
		string controllerEntity = "ConditionManifest";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		Mgr mgr = new Mgr();
        /// <summary>
        /// Handle request to format a ConditionManifest document as CTDL Json-LD
        /// OBSOLETE - redirects to FormatV2
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpPost, Route( "conditionManifest/format" )]
        //public RegistryAssistantResponse Format( ConditionManifestRequest request )
        //{
        //	var response = FormatV2( request );
        //	response.Messages.Add( FormatObsoleteEndpoint( controllerEntity, "formatV2" ) );
        //	return response;
        //} //

        /// <summary>
        /// Handle request to format a ConditionManifest document as CTDL Json-LD
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "conditionManifest/format" )]
        public RegistryAssistantFormatResponse Format( ConditionManifestRequest request )
        {
            bool isValid = true;
            var response = new RegistryAssistantFormatResponse();

            try
            {
                if ( request == null || request.ConditionManifest == null )
                {
                    response.Messages.Add( "Error - please provide a valid ConditionManifest request." );
                    return response;
                }

                LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, IPaddress: {2}", thisClassName, ServiceHelper.GetCurrentIP(), request.ConditionManifest.CTID ) );
                //foreach ( ConditionManifest item in request.ConditionManifests )
                //{}
                string originalCTID = request.ConditionManifest.CTID ?? "";
                RequestHelper helper = new RequestHelper();
                helper.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );
                //do this in controllerEntity

                response.Payload = new ConditionManifestServicesV2().FormatAsJson( request, ref isValid, helper );
                response.Successful = isValid;
                if ( isValid )
                {
					//check for any (likely warnings) messages
					if ( helper.Messages.Count > 0 )
						response.Messages = helper.GetAllMessages();
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
        /// Publish a ConditionManifest to the Credential Engine Registry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "conditionManifest/publish" )]
        public RegistryAssistantResponse Publish( ConditionManifestRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            string statusMessage = "";

            try
            {
                if ( request == null || request.ConditionManifest == null )
                {
                    response.Messages.Add( "Error - please provide a valid ConditionManifest request." );
                    return response;
                }
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.ConditionManifest.CTID, request.RegistryEnvelopeId ) );

                helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();

                if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
                    helper.SerializedInput = ServiceHelper.LogInputFile( request, request.ConditionManifest.CTID, "ConditionManifest", "Publish", 5 );
                    string originalCTID = request.ConditionManifest.CTID ?? "";

                    mgr.Publish( request, ref isValid, helper );

                    response.CTID = request.ConditionManifest.CTID.ToLower();
                    response.Payload = helper.Payload;
                    response.Successful = isValid;

                    if ( isValid )
                    {
						if (helper.Messages.Count > 0)
							response.Messages = helper.GetAllMessages();

						UpdateResponse(helper, response);

						//response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
						//response.EnvelopeUrl = mgr.credentialRegistryBaseUrl + "envelopes/" + response.RegistryEnvelopeIdentifier;

						//if ( response.CTID != originalCTID )
      //                  {
      //                      response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this ConditionManifest. If not provided, the future request will be treated as a new ConditionManifest." );
      //                  }

                    }
                    else
                    {
                        //response.Messages = messages;
                        response.Messages = helper.GetAllMessages();
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
          /// Delete request of an ConditionManifest by CTID and owning organization
          /// </summary>
          /// <param name="request"></param>
          /// <returns></returns>
        [HttpDelete, Route( "conditionmanifest/delete" )]
        public RegistryAssistantDeleteResponse Delete( DeleteRequest request )
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
        /// Delete request of an ConditionManifest by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "conditionmanifest/envelopeDelete" )]
        public RegistryAssistantDeleteResponse CustomDelete( EnvelopeDelete request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantDeleteResponse();
            string statusMessage = "";

            try
            {
                if ( request == null
                    || string.IsNullOrWhiteSpace( request.CTID )
                    || string.IsNullOrWhiteSpace( request.RegistryEnvelopeId )
                    )
                {
                    response.Messages.Add( "Error - please provide a valid delete request with a CTID, and envelope ID." );
                    return response;
                }
                helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
                if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage, true ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
                    RegistryServices cer = new RegistryServices( "ConditionManifest", "", request.CTID );
                    isValid = cer.CredentialRegistry_SelfManagedKeysDelete( request, "registry assistant", ref statusMessage );

                    response.Successful = isValid;

                    if ( isValid )
                    {
                        response.Successful = true;
                    }
                    else
                    {
                        //if not valid, could return the payload as reference?
                        response.Messages.Add(statusMessage);
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
