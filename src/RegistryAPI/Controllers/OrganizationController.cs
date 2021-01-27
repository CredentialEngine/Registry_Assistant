using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.OrganizationRequest;
using RA.Services;
using Mgr = RA.Services.OrganizationServicesV2;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Registry Assistant for Organizations
	/// </summary>
	public class OrganizationController : BaseController
	{
		string thisClassName = "OrganizationController";
		string controllerEntity = "organization";
		string statusMessage = "";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		Mgr mgr = new Mgr();

		/// <summary>
		/// Handle request to format an Organization document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "organization/format" )]
		public RegistryAssistantFormatResponse Format(EntityRequest request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.Organization == null )
				{
					response.Messages.Add( "Error - please provide a valid Organization request." );
					return response;
				}
				ServiceHelper.LogInputFile( request, request.Organization.Ctid, "Organization", "Format" );
				//owner is not required for a format
				//helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if ( request == null || request.Organization == null )
				{
					response.Messages.Add( "WARNING - A valid CTID for the publishing organization is missing. This is required for publishing." );
					return response;
				}

				response.Payload = mgr.FormatAsJson( request, helper, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "Organization", mgr.CurrentEntityName, request.Organization.Ctid, ref statusMessage );

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
		public RegistryAssistantResponse Publish(EntityRequest request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";
			if ( ThereIsASystemOutage( ref statusMessage ) )
			{
				response.Messages.Add( statusMessage );
				return response;
			}
			try
			{
				if ( request == null || request.Organization == null )
				{
					response.Messages.Add( "Error - please provide a valid Organization request." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Organization.Ctid, request.RegistryEnvelopeId ) );

				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Organization.Ctid, "Organization", "Publish", 5 );

					string originalCTID = request.Organization.Ctid ?? "";

					mgr.Publish( request, ref isValid, helper );

					response.CTID = request.Organization.Ctid.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;

					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();
						UpdateResponse( helper, response );

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Organization", response.CTID );

					}
					else
					{
						response.Messages = helper.GetAllMessages();
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
		/// Delete request of an Organization by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "organization/delete" )]
		public RegistryAssistantDeleteResponse Delete(DeleteRequest request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();

			try
			{
				if ( request == null || request.CTID == null )
				{
					response.Messages.Add( "Error - please provide a valid Organization delete request." );
					return response;
				}
				RegistryServices cer = new RegistryServices( controllerEntity, "", request.CTID );
				//TODO - should a delete be allowed if credentials exist
				isValid = cer.DeleteRequest( request, controllerEntity, ref messages );
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
		/// Delete request of an Organization by EnvelopeId and CTID
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "organization/envelopeDelete" )]
		public RegistryAssistantDeleteResponse EnvelopeDelete(EnvelopeDelete request)
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
					RegistryServices cer = new RegistryServices( "Organization", "", request.CTID );
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


		/// <summary>
		/// TBD: enable just replacing the verification service of an existing organization
		/// </summary>
		/// <param name="request">MAY want a different class. At least make clear that only verification profile is processed</param>
		/// <returns></returns>
		[ApiExplorerSettings( IgnoreApi = true )]
		[HttpPost, Route( "organization/UpdateVerification" )]
		public RegistryAssistantResponse UpdateVerification(EntityRequest request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";

			try
			{
				if ( request == null || request.Organization == null )
				{
					response.Messages.Add( "Error - please provide a valid organization request." );
					return response;
				}
				//if ( request.BetaTestingAuthorizationKey == null || request.BetaTestingAuthorizationKey != "DoesAnyone?KnowWhatDayItis?" )
				//{
				//	response.Messages.Add( "Error - THIS FEATURE IS NOT PUBLICALLY AVAILABLE AT THIS TIME." );
				//	return response;
				//}
				if ( string.IsNullOrWhiteSpace( request.Organization.Ctid )
					|| string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier )
					|| ( request.Organization.VerificationServiceProfiles == null || request.Organization.VerificationServiceProfiles.Count == 0 )
					)
				{
					response.Messages.Add( "Error - please provide a valid delete request with a CTID, the owning organization, and at least one verification service profile." );
					return response;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.UpdateVerification request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Organization.Ctid, request.RegistryEnvelopeId ) );

				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				//API key will be required!
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{

					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Organization.Ctid, "Organization", "UpdateVerification", 5 );

					isValid = mgr.ReplaceVerificationProfiles( request, helper );

					response.CTID = request.Organization.Ctid.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;

					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();
						UpdateResponse( helper, response );

						//response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
						//response.EnvelopeUrl = mgr.credentialRegistryBaseUrl + "envelopes/" + response.RegistryEnvelopeIdentifier;

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Organization", response.CTID );



					}
					else
					{
						//if not valid, could return the payload as reference?
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

	}
}