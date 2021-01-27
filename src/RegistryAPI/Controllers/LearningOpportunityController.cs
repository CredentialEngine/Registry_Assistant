using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using Newtonsoft.Json;
using RA.Services;
using Mgr = RA.Services.LearningOpportunityServicesV2;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Registry Assistant for Learning Opportunities
	/// </summary>
	public class LearningOpportunityController : BaseController
    {
		string statusMessage = "";
		string thisClassName = "LearningOpportunityController";
		string controllerEntity = "LearningOpportunity";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		Mgr mgr = new Mgr();

		/// <summary>
		/// Handle request to format a Learning Opportunity document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "learningopportunity/format" )]
        public RegistryAssistantFormatResponse Format( LearningOpportunityRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantFormatResponse();

            try
            {
                if ( request == null || request.LearningOpportunity == null )
                {
                    response.Messages.Add( "Error - please provide a valid LearningOpportunity request." );
                    return response;
                }
                ServiceHelper.LogInputFile( request, request.LearningOpportunity.Ctid, "LearningOpportunity", "Format" );

                response.Payload = new LearningOpportunityServicesV2().FormatAsJson( request, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "LearningOpportunity", mgr.CurrentEntityName, request.LearningOpportunity.Ctid, ref statusMessage );

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
        /// Publish a Learning Opportunity to the Credential Engine Registry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "learningopportunity/publish" )]
        public RegistryAssistantResponse Publish( LearningOpportunityRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();

            string registryEnvelopeId = "";
            try
            {
                if ( request == null || request.LearningOpportunity == null )
                {
                    response.Messages.Add( "Error - please provide a valid Learning Opportunity request." );
                    return response;
                }
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.LearningOpportunity.Ctid, request.RegistryEnvelopeId ) );

                helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
                if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
                    helper.SerializedInput = ServiceHelper.LogInputFile( request, request.LearningOpportunity.Ctid, "LearningOpportunity", "Publish", 5 );

                    registryEnvelopeId = request.RegistryEnvelopeId;
                    string originalCTID = request.LearningOpportunity.Ctid ?? "";

                    new LearningOpportunityServicesV2().Publish( request, ref isValid, helper );

                    //CredentialServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );

                    response.CTID = request.LearningOpportunity.Ctid.ToLower();
                    response.Payload = helper.Payload;
                    response.Successful = isValid;

                    if ( isValid )
                    {
						if (helper.Messages.Count > 0)
							response.Messages = helper.GetAllMessages();

						UpdateResponse(helper, response);

						//response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;						
						//response.EnvelopeUrl = mgr.credentialRegistryBaseUrl + "envelopes/" + response.RegistryEnvelopeIdentifier;

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "LearningOpportunity", response.CTID);

                        //if ( response.CTID != originalCTID )
                        //{
                        //    response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this Learning Opportunity. If not provided, the future request will be treated as a new Learning Opportunity." );
                        //}
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
        /// Delete request of an LearningOpportunity by CTID and owning organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "learningopportunity/delete" )]
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
        /// Delete request of an Learning Opportunity by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "learningopportunity/envelopeDelete" )]
        public RegistryAssistantDeleteResponse CustomDelete(EnvelopeDelete request)
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
                    RegistryServices cer = new RegistryServices( "LearningOpportunity", "", request.CTID );
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
