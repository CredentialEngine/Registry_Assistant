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
	/// Registry Assistant for Cost Manifests
	/// </summary>
	public class CostManifestController : BaseController
	{
		string thisClassName = "CostManifestController";
		string controller = "CostManifest";
		RA.Models.RequestHelper helper = new RequestHelper();
		/// <summary>
		/// Handle request to format a CostManifest document as CTDL Json-LD
		/// OBSOLETE - redirects to FormatV2
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "costManifest/format" )]
		public RegistryAssistantResponse Format( CostManifestRequest request )
		{
			var response = FormatV2( request );
			response.Messages.Add( FormatObsoleteEndpoint( controller, "formatV2" ) );
			return response;
			//bool isValid = true;
			//var response = new RegistryAssistantResponse();

			//try
			//{
			//	if ( request == null || request.CostManifest == null )
			//	{
			//		response.Messages.Add( "Error - please provide a valid CostManifest request." );
			//		return response;
			//	}

			//             LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}", thisClassName, ServiceHelper.GetCurrentIP(), request.CostManifest.Ctid ) );

			//             string origCTID = request.CostManifest.Ctid ?? "";
			//             RequestHelper helper = new RequestHelper();
			//             helper.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );

			//             response.Payload = CostManifestServices.FormatAsJson( request, ref isValid, helper );
			//	response.Successful = isValid;

			//             if ( isValid )
			//             {
			//                 response.CTID = request.CostManifest.Ctid;
			//                 //check for any (likely warnings) messages
			//                 if ( helper.Messages.Count > 0 )
			//                     response.Messages = helper.GetAllMessages();
			//                 if ( response.CTID != origCTID )
			//                 {
			//                     response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this CostManifest. If not provided, the future request will be treated as a new CostManifest." );
			//                 }
			//             }
			//             else
			//             {
			//                 response.Messages = helper.GetAllMessages();
			//             }


			//         }
			//catch ( Exception ex )
			//{
			//	response.Messages.Add( ex.Message );
			//	response.Successful = false;
			//}
			//return response;
		} //

		/// <summary>
		/// Publish a CostManifest to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "costManifest/publish" )]
		public RegistryAssistantResponse Publish( CostManifestRequest request )
		{
			var response = PublishV2( request );
			response.Messages.Add( FormatObsoleteEndpoint( controller, "publishV2" ) );
			return response;
			//bool isValid = true;
			//List<string> messages = new List<string>();
			//var response = new RegistryAssistantResponse();
			//string statusMessage = "";


			//try
			//{
			//	if ( request == null || request.CostManifest == null )
			//	{
			//		response.Messages.Add( "Error - please provide a valid CostManifest request." );
			//		return response;
			//	}
			//	LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(),  request.CostManifest.Ctid, request.RegistryEnvelopeId ) );

			//             helper.OwnerCtid = request.PublishForOrganizationIdentifier;
			//             if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
			//	{
			//		response.Messages.Add( statusMessage );
			//	}
			//	else
			//	{
			//                 helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CostManifest.Ctid, "CostManifest", "Publish", 5 );

			//                 //registryEnvelopeId = request.RegistryEnvelopeId;
			//		string origCTID = request.CostManifest.Ctid ?? "";

			//		CostManifestServices.Publish( request, ref isValid, helper );
			//		response.CTID = request.CostManifest.Ctid;
			//                 response.Payload = helper.Payload;
			//                 response.Successful = isValid;

			//		if ( isValid )
			//		{
			//                     response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
			//                     response.CTID = request.CostManifest.Ctid;
			//			if ( response.CTID != origCTID )
			//			{
			//				response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this CostManifest. If not provided, the future request will be treated as a new CostManifest." );
			//			}
			//                     //would want to return any error messages
			//                     if ( helper.Messages.Count > 0 )
			//                         response.Messages = helper.GetAllMessages();
			//                 }
			//		else
			//		{
			//                     //response.Messages = messages;
			//                     response.Messages = helper.GetAllMessages();
			//                 }
			//	}
			//}
			//catch ( Exception ex )
			//{
			//	response.Messages.Add( ex.Message );
			//	response.Successful = false;
			//}
			//return response;
		} //
        /// <summary>
        /// Handle request to format a CostManifest document as CTDL Json-LD
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "costManifest/formatv2" )]
        public RegistryAssistantResponse FormatV2( CostManifestRequest request )
        {
            bool isValid = true;
            var response = new RegistryAssistantResponse();

            try
            {
                if ( request == null || request.CostManifest == null )
                {
                    response.Messages.Add( "Error - please provide a valid CostManifest request." );
                    return response;
                }

                LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}", thisClassName, ServiceHelper.GetCurrentIP(), request.CostManifest.Ctid ) );

                string origCTID = request.CostManifest.Ctid ?? "";
                RequestHelper helper = new RequestHelper();
                helper.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );

                response.Payload = new CostManifestServicesV2().FormatAsJson( request, ref isValid, helper );
                response.Successful = isValid;

                if ( isValid )
                {
                    response.CTID = request.CostManifest.Ctid;
                    //check for any (likely warnings) messages
                    if ( helper.Messages.Count > 0 )
                        response.Messages = helper.GetAllMessages();
                    if ( response.CTID != origCTID )
                    {
                        response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this CostManifest. If not provided, the future request will be treated as a new CostManifest." );
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
        /// Publish a CostManifest to the Credential Engine Registry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "costManifest/publishv2" )]
        public RegistryAssistantResponse PublishV2( CostManifestRequest request )
        {
            bool isValid = true;
            List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            string statusMessage = "";


            try
            {
                if ( request == null || request.CostManifest == null )
                {
                    response.Messages.Add( "Error - please provide a valid CostManifest request." );
                    return response;
                }
                LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.CostManifest.Ctid, request.RegistryEnvelopeId ) );

                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
                    helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CostManifest.Ctid, "CostManifest", "Publish", 5 );

                    //registryEnvelopeId = request.RegistryEnvelopeId;
                    string origCTID = request.CostManifest.Ctid ?? "";

                    new CostManifestServicesV2().Publish( request, ref isValid, helper );
                    response.CTID = request.CostManifest.Ctid;
                    response.Payload = helper.Payload;
                    response.Successful = isValid;

                    if ( isValid )
                    {
                        response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
                        response.CTID = request.CostManifest.Ctid;
                        if ( response.CTID != origCTID )
                        {
                            response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this CostManifest. If not provided, the future request will be treated as a new CostManifest." );
                        }
                        //would want to return any error messages
                        if ( helper.Messages.Count > 0 )
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
        /// Delete request of an CostManifest by CTID and owning organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "costmanifest/delete" )]
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
                    RegistryServices cer = new RegistryServices( "CostManifest", "", request.CTID );

                    isValid = cer.ManagedDelete( request.PublishForOrganizationIdentifier, request.CTID, helper.ApiKey, ref statusMessage );

                    response.Successful = isValid;

                    if ( isValid )
                    {
                        response.Successful = true;
                    }
                    else
                    {
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

        /// <summary>
        /// Delete request of an CostManifest by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "costmanifest/envelopeDelete" )]
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
                    RegistryServices cer = new RegistryServices( "CostManifest", "", request.CTID );
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
