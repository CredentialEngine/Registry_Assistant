using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.AssessmentRequest;
using RA.Services;
using MgrV2 = RA.Services.AssessmentServicesV2;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
    public class AssessmentController : BaseController
    {
		string statusMessage = "";
		readonly string thisClassName = "AssessmentController";
		readonly string controllerEntity = "assessment";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		MgrV2 mgr = new MgrV2();

        /// <summary>
        /// Handle request to format an Assessment document as CTDL Json-LD
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "Assessment/format" )]
        public RegistryAssistantFormatResponse Format( EntityRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantFormatResponse();

            try
            {
                if ( request == null || request.Assessment == null )
                {
                    response.Messages.Add( "Error - please provide a valid Assessment request." );
                    return response;
                }

                ServiceHelper.LogInputFile( request, request.Assessment.Ctid, "Assessment", "Format" );

                response.Payload = new AssessmentServicesV2().FormatAsJson( request, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "Assessment", mgr.CurrentEntityName, request.Assessment.Ctid, ref statusMessage );

				response.Successful = isValid;

                if ( !isValid )
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
        /// Handle request to publish an Assessment document to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "Assessment/publish" )]
        public RegistryAssistantResponse Publish( EntityRequest request )
        {
            bool isValid = true;
            var response = new RegistryAssistantResponse();

            try
            {
                //var domain = Request.RequestUri.Host;
                //string domainName = HttpContext.Current.Request.Url.GetLeftPart( UriPartial.Authority );

                if ( request == null || request.Assessment == null )
                {
                    response.Messages.Add( "Error - please provide a valid Assessment request." );
                    return response;
                }
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishRequest. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Assessment.Ctid, request.RegistryEnvelopeId ) );
				helper = new RequestHelper
				{
					OwnerCtid = request.PublishForOrganizationIdentifier
				};
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
                    helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Assessment.Ctid, "Assessment", "Publish", 5 );

                    new AssessmentServicesV2().Publish( request, ref isValid, helper );

                    response.CTID = request.Assessment.Ctid.ToLower();
                    response.Payload = helper.Payload;
                    response.Successful = isValid;
                    if ( isValid )
                    {
						if (helper.Messages.Count > 0)
							response.Messages = helper.GetAllMessages();

						UpdateResponse(helper, response);

						response.CredentialFinderUrl = string.Format(mgr.credentialFinderDetailUrl, "Assessment", response.CTID);

                    }
                    else
                    {
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
        /// Delete request of an Assessment by CTID and owning organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "assessment/delete" )]
        public RegistryAssistantDeleteResponse Delete( DeleteRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantDeleteResponse();

			try
			{

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
        /// Delete request of an Assessment by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "assessment/envelopeDelete" )]
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
                    RegistryServices cer = new RegistryServices( "Assessment", "", request.CTID );
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
