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
using Utilities;
namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Registry Assistant for Condition Manifests
	/// </summary>
	public class ConditionManifestController : ApiController
	{
		string thisClassName = "ConditionManifestController";
        RA.Models.RequestHelper helper = new RequestHelper();
        /// <summary>
        /// Handle request to format a ConditionManifest document as CTDL Json-LD
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "conditionManifest/format" )]
		public RegistryAssistantResponse Format( ConditionManifestRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.ConditionManifest == null )
				{
					response.Messages.Add( "Error - please provide a valid ConditionManifest request." );
					return response;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. ApiKey: {1}, IPaddress: {2}, requestBy: {3}, ctid: {4}", thisClassName, ServiceHelper.GetCurrentIP(),  request.ConditionManifest.Ctid ) );
				//foreach ( ConditionManifest item in request.ConditionManifests )
				//{}
				string origCTID = request.ConditionManifest.Ctid ?? "";
				RequestHelper helper = new RequestHelper();
				helper.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );
				//do this in controller

				response.Payload = ConditionManifestServices.FormatAsJson( request, ref isValid, helper );
				response.Successful = isValid;
				if ( isValid )
				{
                    response.CTID = request.ConditionManifest.Ctid;
                    //check for any (likely warnings) messages
                    if ( helper.Messages.Count > 0 )
                        response.Messages = helper.GetAllMessages();
                    if ( response.CTID != origCTID )
                    {
                        response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this ConditionManifest. If not provided, the future request will be treated as a new ConditionManifest." );
                    }
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
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(),  request.ConditionManifest.Ctid, request.RegistryEnvelopeId ) );
                
                helper.OwnerCtid = request.PublishForOrganizationIdentifier;

                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
                    helper.SerializedInput = ServiceHelper.LogInputFile( request, request.ConditionManifest.Ctid, "ConditionManifest", "Publish", 5 );
                    string origCTID = request.ConditionManifest.Ctid ?? "";
					
					ConditionManifestServices.Publish( request, ref isValid, helper);

					response.CTID = request.ConditionManifest.Ctid;
					response.Payload = helper.Payload;
					response.Successful = isValid;

					if ( isValid )
					{
						response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
						response.CTID = request.ConditionManifest.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this ConditionManifest. If not provided, the future request will be treated as a new ConditionManifest." );
						}

						//would want to return any error messages
						if (helper.Messages.Count > 0)
							response.Messages = helper.GetAllMessages();
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
                    RegistryServices cer = new RegistryServices( "ConditionManifest", "", request.CTID );

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
        /// Delete request of an ConditionManifest by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "conditionmanifest/envelopeDelete" )]
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
