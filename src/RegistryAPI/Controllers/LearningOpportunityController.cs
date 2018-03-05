﻿using System;
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
	/// Registry Assistant for Learning Opportunities
	/// </summary>
	public class LearningOpportunityController : ApiController
    {
		string statusMessage = "";
		string thisClassName = "LearningOpportunityController";
        RA.Models.RequestHelper helper = new RequestHelper();

        /// <summary>
        /// Handle request to format a Learning Opportunity document as CTDL Json-LD
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route( "learningopportunity/format" )]
		public RegistryAssistantResponse Format( LearningOpportunityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				response.Payload = LearningOpportunityServices.FormatAsJson( request, ref isValid, ref messages );
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
			string payload = "";
			string registryEnvelopeId = "";
			try
			{
				if ( request == null || request.LearningOpportunity == null )
				{
					response.Messages.Add( "Error - please provide a valid Learning Opportunity request." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Trace request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(),  request.LearningOpportunity.Ctid, request.RegistryEnvelopeId ) );

                helper.OwnerCtid = request.PublishForOrganizationIdentifier;
                if ( !ServiceHelper.ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					registryEnvelopeId = request.RegistryEnvelopeId;
					string origCTID = request.LearningOpportunity.Ctid ?? "";

					LearningOpportunityServices.Publish( request, helper.ApiKey, ref isValid, ref messages, ref payload, ref registryEnvelopeId );
					response.Payload = payload;
					response.Successful = isValid;

					if ( isValid )
					{
						response.RegistryEnvelopeIdentifier = registryEnvelopeId;
						response.CTID = request.LearningOpportunity.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this Learning Opportunity. If not provided, the future request will be treated as a new Learning Opportunity." );
						}
					}
					else
					{
						response.Messages = messages;
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
                    RegistryServices cer = new RegistryServices( "LearningOpportunity", "", request.CTID );

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

        [HttpDelete, Route( "learningopportunity/envelopeDelete" )]
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
