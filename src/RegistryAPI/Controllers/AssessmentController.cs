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
using Utilities;

namespace RegistryAPI.Controllers
{
    public class AssessmentController : ApiController
    {
		string statusMessage = "";
		string thisClassName = "AssessmentController";
        RA.Models.RequestHelper helper = new RequestHelper();

        /// <summary>
        /// Handle request to format an Assessment document as CTDL Json-LD
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "Assessment/format" )]
		public RegistryAssistantResponse Format( AssessmentRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
                if ( request == null || request.Assessment == null )
                {
                    response.Messages.Add( "Error - please provide a valid Assessment request." );
                    return response;
                }

                ServiceHelper.LogInputFile( request, request.Assessment.Ctid, "Assessment", "Format" );

                response.Payload = AssessmentServices.FormatAsJson( request, ref isValid, ref messages );
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
		/// Publish an Assessment to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Assessment/publish" )]
		public RegistryAssistantResponse Publish( AssessmentRequest request )
		{
			bool isValid = true;
			//List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.Assessment == null )
				{
					response.Messages.Add( "Error - please provide a valid Assessment request." );
					return response;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(),  request.Assessment.Ctid, request.RegistryEnvelopeId ) );
                helper = new RequestHelper();
                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
                    helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Assessment.Ctid, "Assessment", "Publish", 5 );
					string origCTID = request.Assessment.Ctid ?? "";

					AssessmentServices.Publish( request, ref isValid, helper );

                    //CredentialServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );

                    response.CTID = request.Assessment.Ctid;
                    response.Payload = helper.Payload;
                    response.Successful = isValid;
                    if ( isValid )
					{
                        response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
                        if ( helper.Messages.Count > 0 )
                            response.Messages = helper.GetAllMessages();
                        response.CTID = request.Assessment.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this Assessment. If not provided, the future request will be treated as a new Assessment." );
						}

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
        public RegistryAssistantResponse Delete( DeleteRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            string statusMessage = "";

            try
            {
                if ( request == null
                    || string.IsNullOrWhiteSpace( request.CTID )
                    || string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier )
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
                    RegistryServices cer = new RegistryServices( "Assessment", "", request.CTID );

                    isValid = cer.ManagedDelete( request.PublishForOrganizationIdentifier, request.CTID, helper.ApiKey, ref statusMessage );

                    response.Successful = isValid;

                    if ( isValid )
                    {
                        response.Successful = true;
                    }
                    else
                    {
                        response.Messages = messages;
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
        /// Delete request of an Assessment by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "assessment/envelopeDelete" )]
        public RegistryAssistantResponse CustomDelete( EnvelopeDelete request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
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

                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {

                    isValid = RegistryServices.CredentialRegistry_SelfManagedKeysDelete( request.RegistryEnvelopeId, request.CTID, helper.ApiKey, ref statusMessage );

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
                        response.Messages = messages;
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
