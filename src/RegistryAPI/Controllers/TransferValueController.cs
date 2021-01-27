using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.TransferValueProfileRequest;
using BulkEntityRequest = RA.Models.Input.TransferValueProfileBulkRequest;
using RA.Services;
using MgrV2 = RA.Services.TransferValueServices;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
    //[ApiExplorerSettings( IgnoreApi = true )]
    public class TransferValueController : BaseController
    {
        string statusMessage = "";
        readonly string thisClassName = "TransferValueController";
        readonly string controllerEntity = "TransferValue";
        RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
        MgrV2 mgr = new MgrV2();

		/// <summary>
		/// Handle request to format an TransferValue document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "TransferValue/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.TransferValueProfile == null  )
				{
					response.Messages.Add( "Error - please provide a valid TransferValueProfile request." );
					//responses.Add( response );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.PublishForOrganizationIdentifier, "TransferValue", "Format" );
				mgr = new MgrV2();

				response.Payload = mgr.FormatAsJson( request, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "TransferValue", mgr.CurrentEntityName, request.TransferValueProfile.CTID, ref statusMessage );

				response.Successful = isValid;
					response.Messages = messages;

					//responses.Add( response );
				//}

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
		/// Handle request to publish a TransferValue document to the Credential Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "TransferValue/publish" )]
        public RegistryAssistantResponse Publish(EntityRequest request)
        {
            bool isValid = true;
            var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.TransferValueProfile == null || string.IsNullOrWhiteSpace( request.TransferValueProfile.CTID ))
				{
					response.Messages.Add( "Error - please provide a valid TransferValueProfile request with a CTID." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishRequest. IPaddress: {1}, PublishForOrganizationIdentifier: {2}", thisClassName, ServiceHelper.GetCurrentIP(), request.PublishForOrganizationIdentifier ) );

				helper = new RequestHelper
				{
					OwnerCtid = request.PublishForOrganizationIdentifier.ToLower()
				};
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
					return response;
				}
				else
				{
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.TransferValueProfile.CTID, "TransferValue", "Publish", 5 );

					mgr.Publish( request, ref isValid, helper );
					//
					response.Payload = helper.Payload;
					response.CTID = request.TransferValueProfile.CTID.ToLower();

					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();

						UpdateResponse( helper, response );

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "TransferValueProfile", response.CTID );
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
		/// Proposed option for bulk publishing. Is there a use case for this?
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "TransferValue/bulkpublish" )]
		public List<RegistryAssistantResponse> BulkPublish( BulkEntityRequest request )
		{
			bool isValid = true;
			var responses = new List<RegistryAssistantResponse>();
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.TransferValueProfiles == null || request.TransferValueProfiles.Count == 0 )
				{
					response.Messages.Add( "Error - please provide a valid TransferValue request." );
					responses.Add( response );
					return responses;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishRequest. IPaddress: {1}, PublishForOrganizationIdentifier: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.PublishForOrganizationIdentifier, request.RegistryEnvelopeId ) );

				helper = new RequestHelper
				{
					OwnerCtid = request.PublishForOrganizationIdentifier
				};
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}

				int cntr = 0;
				mgr = new MgrV2();
				//
				foreach ( var tvp in request.TransferValueProfiles )
				{
					cntr++;
					response = new RegistryAssistantResponse();
					helper = new RequestHelper
					{
						OwnerCtid = request.PublishForOrganizationIdentifier
					};
					helper.SerializedInput = ServiceHelper.LogInputFile( tvp, tvp.CTID, "TransferValue", "Publish", 5 );

					var thisRequest = new TransferValueProfileRequest()
					{
						TransferValueProfile = tvp,
						RegistryEnvelopeId = "",
						Community = request.Community,
						PublishForOrganizationIdentifier = request.PublishForOrganizationIdentifier
					};
					//do we need to newUp the mgr on each iteration?
					mgr.Publish( thisRequest, ref isValid, helper );

					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();

						UpdateResponse( helper, response );

						//response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "TransferValue", response.CTID );
					}
					else
					{
						response.Messages = helper.GetAllMessages();
					}

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

		/// <summary>
		/// Delete request of an TransferValue by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "TransferValue/delete" )]
        public RegistryAssistantDeleteResponse Delete(DeleteRequest request)
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
        /// Delete request of an TransferValue by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "TransferValue/envelopeDelete" )]
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
                    RegistryServices cer = new RegistryServices( "TransferValue", "", request.CTID );
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
