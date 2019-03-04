using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.ConceptSchemeRequest;
using CASSEntityRequest = RA.Models.Input.CASSConceptSchemeRequest;
using RA.Services;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
    public class ConceptSchemeController : BaseController
	{
		string thisClassName = "ConceptSchemeController";
		RA.Models.RequestHelper helper = new RequestHelper();
		string statusMessage = "";
		ConceptSchemeServices manager = new ConceptSchemeServices();

		/// <summary>
		/// Publish competency frameworks that have been exported from CASS
		/// The input will already be in the proper format. 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "ConceptScheme/publishGraph" )]
		private RegistryAssistantResponse GraphPublish( CASSEntityRequest request )
		{
			bool isValid = true;
			//List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.ConceptSchemeGraph == null )
				{
					response.Messages.Add( "Error - please provide a valid Concept Scheme request with the JSON Graph." );
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
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CTID, "ConceptScheme", "Publish", 5 );
					string origCTID = request.CTID ?? "";

					manager.PublishGraph( request, ref isValid, helper );

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

		//[HttpPost, Route( "ConceptScheme/publishFromCassV2" )]
		//public RegistryAssistantResponse CassPublishV2( CASSEntityRequest request )
		//{

		//	return CassPublish( request );
		//}

		//[HttpDelete, Route( "ConceptScheme/delete" )]
		//public RegistryAssistantResponse Delete( CASSEntityRequest request )
		//{
		//	return Delete( request );
		//}

		[HttpDelete, Route( "ConceptScheme/delete" )]
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
					RegistryServices cer = new RegistryServices( "ConceptScheme", "", request.CTID );

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
		/// Delete request of an ConceptScheme by EnvelopeId and CTID
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "ConceptScheme/envelopeDelete" )]
		private RegistryAssistantResponse CustomDelete( EnvelopeDelete request )
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
					RegistryServices cer = new RegistryServices( "ConceptScheme", "", request.CTID );
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
