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
	/// <summary>
	/// Registry Assistant for Organizations
	/// </summary>
	public class OrganizationController : ApiController
	{
		string thisClassName = "OrganizationController";
		string statusMessage = "";
        RA.Models.RequestHelper helper = new RequestHelper();

        /// <summary>
        /// Handle request to format an Organization document as CTDL Json-LD
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "organization/format" )]
		public RegistryAssistantResponse Format( OrganizationRequest request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if( request == null || request.Organization == null )
				{
					response.Messages.Add( "Error - please provide a valid Organization request." );
					return response;
				}
                ServiceHelper.LogInputFile( request, request.Organization.Ctid, "Organization", "Format" );

				response.Payload = OrganizationServices.FormatAsJson( request, ref isValid, ref messages );
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
		/// Publish an Organization to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "organization/publish" )]
		public RegistryAssistantResponse Publish( OrganizationRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					if ( request == null || request.Organization == null )
					{
						response.Messages.Add( "Error - please provide a valid Organization request." );
						return response;
					}
					LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(),  request.Organization.Ctid, request.RegistryEnvelopeId ) );

                    helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                    if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
					{
						response.Messages.Add( statusMessage );

					}
					else
					{
                        helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Organization.Ctid, "Organization", "Publish", 5 );

						string origCTID = request.Organization.Ctid ?? "";

                        OrganizationServices.Publish(request, ref isValid, helper);
                        //OrganizationServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );
                        response.CTID = request.Organization.Ctid;
                        response.Payload = helper.Payload;
                        response.Successful = isValid;

                        if ( isValid )
						{
							response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
							response.CTID = request.Organization.Ctid;
                            if (helper.Messages.Count > 0)
                                response.Messages = helper.GetAllMessages();
                            if ( response.CTID != origCTID )
							{
								response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this Organization. If not provided, the future request will be treated as a new Organization." );
							}

						}
						else
						{
                            response.Messages = helper.GetAllMessages();
                            response.Successful = false;
                        }
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
        /// Delete request of an Organization by CTID and owning organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "organization/delete" )]
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
                    RegistryServices cer = new RegistryServices( "Organization", "", request.CTID );

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
        /// Delete request of an Organization by EnvelopeId and CTID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete, Route( "organization/envelopeDelete" )]
		public RegistryAssistantResponse EnvelopeDelete( EnvelopeDelete request )
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