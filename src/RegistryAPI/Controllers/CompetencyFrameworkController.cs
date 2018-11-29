using System;
using System.Collections.Generic;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.CompetencyFrameworkRequest;
using CASSEntityRequest = RA.Models.Input.CASSCompetencyFrameworkRequest;
using RA.Services;
using Utilities;

namespace RegistryAPI.Controllers
{
    public class CompetencyFrameworkController : BaseController
    {
        string thisClassName = "CompetencyFrameworkController";
        RA.Models.RequestHelper helper = new RequestHelper();
        string statusMessage = "";
        CompetencyServices manager = new CompetencyServices();

        /// <summary>
        /// Request to publish a competence framework and all related competencies
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ HttpPost, Route( "CompetencyFramework/publish" )]
        private RegistryAssistantResponse Publish( EntityRequest request )
        {
            bool isValid = true;
            //List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            if (1 == 1)
            {
                response.Messages.Add( "Error - this endpoint is not available to the public at this time." );
                return response;
            }

            //try
            //{
            //    if ( request == null || request.CompetencyFramework == null )
            //    {
            //        response.Messages.Add( "Error - please provide a valid Competency Framework request." );
            //        return response;
            //    }
            //    if ( string.IsNullOrWhiteSpace( request.CTID ) )
            //    {
            //        response.Messages.Add( "Error - please provide a valid CTID for this framework" );
            //        return response;
            //    }
            //    LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.CTID, request.RegistryEnvelopeId ) );
            //    helper = new RequestHelper();
            //    helper.OwnerCtid = request.PublishForOrganizationIdentifier;
            //    if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
            //    {
            //        response.Messages.Add( statusMessage );
            //    }
            //    else
            //    {
            //        helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CompetencyFramework.Ctid, "CompetencyFramework", "Publish", 5 );
            //        string origCTID = request.CTID ?? "";

            //        new CompetencyServices().Publish( request, ref isValid, helper );

            //        response.CTID = request.CompetencyFramework.Ctid;
            //        response.Payload = helper.Payload;
            //        response.Successful = isValid;
            //        if ( isValid )
            //        {
            //            response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
            //            if ( helper.Messages.Count > 0 )
            //                response.Messages = helper.GetAllMessages();
            //            response.CTID = request.CompetencyFramework.Ctid;
            //            if ( response.CTID != origCTID )
            //            {
            //                response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this CompetencyFramework. If not provided, the future request will be treated as a new CompetencyFramework." );
            //            }

            //        }
            //        else
            //        {
            //            response.Messages = helper.GetAllMessages();
            //        }
            //    }
            //}
            //catch ( Exception ex )
            //{
            //    response.Messages.Add( ex.Message );
            //    response.Successful = false;
            //}
            //return response;
        }

        /// <summary>
        /// Publish competency frameworks that have been exported from CASS
        /// The input will already be in the proper format. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "CompetencyFramework/publishFromCass" )]
        public RegistryAssistantResponse CassPublish( CASSEntityRequest request )
        {
            bool isValid = true;
            //List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            if (1 == 1)
            {
                //response.Messages.Add( "Error - this endpoint is not available to the public at this time." );
                //return response;
            }
            try
            {
                if ( request == null || request.CompetencyFrameworkGraph == null )
                {
                    response.Messages.Add( "Error - please provide a valid Competency Framework request with the JSON Graph." );
                    return response;
                }

                LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.CTID, request.RegistryEnvelopeId ) );
                helper = new RequestHelper();
                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
                    helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CTID, "CompetencyFramework", "Publish", 5 );
                    string origCTID = request.CTID ?? "";

                    manager.PublishFromCASS( request, ref isValid, helper );

                    response.CTID = request.CTID;
                    response.Payload = helper.Payload;
                    response.Successful = isValid;
                    if ( isValid )
                    {
                        response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
                        if ( helper.Messages.Count > 0 )
                            response.Messages = helper.GetAllMessages();
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
        }

        /// <summary>
        /// The V2 vesion is the same as CassPublish. Created to allow for use with general V2 publishing
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "CompetencyFramework/publishFromCassV2" )]
        public RegistryAssistantResponse CassPublishV2( CASSEntityRequest request )
        {
            
            return CassPublish( request);
        }

		/// <summary>
		/// All deletes are done thru the publisher at this time
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "CompetencyFramework/delete" )]
		private RegistryAssistantResponse Delete( DeleteRequest request )
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
					RegistryServices cer = new RegistryServices( "CompetencyFramework", "", request.CTID );

					isValid = cer.ManagedDelete( request.PublishForOrganizationIdentifier, request.CTID, helper.ApiKey, ref statusMessage ) ;

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
	}
}