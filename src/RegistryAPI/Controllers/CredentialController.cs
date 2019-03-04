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
		string controller = "credential";
        RA.Models.RequestHelper helper = new RequestHelper();
        string env = UtilityManager.GetAppKeyValue("environment");
        MgrV2 mgr = new MgrV2();

	
		/// <summary>
		/// Handle request to format a Credential document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "credential/format" )]
        public RegistryAssistantResponse Format( CredentialRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();

            try
            {
                if ( request == null || request.Credential == null )
                {
                    response.Messages.Add( "Error - please provide a valid credential request." );
                    return response;
                }
                ServiceHelper.LogInputFile( request, "Format" );
                string origCTID = request.Credential.Ctid ?? "";

                response.Payload = mgr.FormatAsJson( request, ref isValid, ref messages );
                response.Successful = isValid;
                if ( isValid )
                {
                    response.Successful = true;
                    response.CTID = request.Credential.Ctid;
                    if ( response.CTID != origCTID )
                    {
                        response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this credential. If not provided, the future request will be treated as a new credential." );
                    }
                }
                else
                {
                    response.Messages = messages;
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
            //if (env == "production")
            //{
            //    response.Messages.Add("Error - this endpoint is not valid in this environment.");
            //    return response;
            //}

            try
            {
                if ( request == null || request.Credential == null )
                {
                    response.Messages.Add("Error - please provide a valid credential request.");
                    return response;
                }

                LoggingHelper.DoTrace(2, string.Format("RegistryAssistant.{0}.PublishRrequest. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Credential.Ctid, request.RegistryEnvelopeId));

                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest(helper, ref statusMessage) )
                {
                    response.Messages.Add(statusMessage);
                }
                else
                {
                    helper.SerializedInput = ServiceHelper.LogInputFile(request, "Publish", 5);
                    string origCTID = request.Credential.Ctid ?? "";

                    mgr.Publish(request, ref isValid, helper);

                    response.CTID = request.Credential.Ctid;
                    response.Payload = helper.Payload;
                    response.Successful = isValid;

                    if ( isValid )
                    {
                        response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
                        if ( helper.Messages.Count > 0 )
                            response.Messages = helper.GetAllMessages();
                        response.CTID = request.Credential.Ctid;

                        if ( response.CTID != origCTID )
                        {
                            response.Messages.Add("Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this credential. If not provided, the future request will be treated as a new credential.");
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
            catch ( Exception ex )
            {
                response.Messages.Add(ex.Message);
                response.Successful = false;
            }
            return response;
        } //

        /// <summary>
        /// Delete request of an Credential by CTID and owning organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "credential/delete" )]
		public RegistryAssistantResponse Delete( DeleteRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";

			try
			{
                //envelopeId is not applicable for managed keys!, could have a separate endpoint?
                if ( request == null 
					|| string.IsNullOrWhiteSpace(request.CTID)
					|| string.IsNullOrWhiteSpace(request.PublishForOrganizationIdentifier ) 
					)
				{
					response.Messages.Add( "Error - please provide a valid delete request with a CTID, and the owning organization." );
					return response;
				}

                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage, true ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					RegistryServices cer = new RegistryServices( "Credential", "", request.CTID );

                    isValid = cer.ManagedDelete( request.PublishForOrganizationIdentifier, request.CTID, helper.ApiKey, ref statusMessage );
					
					response.Successful = isValid;

					if ( isValid )
					{
						response.Successful = true;
					}
					else
					{
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

        /// <summary>
        /// Delete request of an Credential by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "credential/envelopeDelete" )]
        public RegistryAssistantResponse CustomDelete( EnvelopeDelete request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
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
                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage, true ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
					//only valid token at this time is from CER
					RegistryServices cer = new RegistryServices( "Credential", "", request.CTID );
                    isValid = cer.CredentialRegistry_SelfManagedKeysDelete( request.RegistryEnvelopeId, request.CTID, "registry assistant", ref statusMessage );

                    response.Successful = isValid;

                    if ( isValid )
                    {
                        response.Successful = true;
                        response.RegistryEnvelopeIdentifier = request.RegistryEnvelopeId;
                        response.CTID = request.CTID;
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