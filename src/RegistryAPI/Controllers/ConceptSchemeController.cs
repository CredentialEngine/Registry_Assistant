using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.ConceptSchemeRequest;
using GraphEntityRequest = RA.Models.Input.ConceptSchemeGraphRequest;
//using GraphEntityRequest2 = RA.Models.Input.GraphConceptSchemeRequest;
//
using RA.Services;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
	/// <summary>
	/// ConceptSchemes publishing, etc.
	/// </summary>
    public class ConceptSchemeController : BaseController
	{
		string thisClassName = "ConceptSchemeController";
		string controllerEntity = "ConceptScheme";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		string statusMessage = "";
		ConceptSchemeServices manager = new ConceptSchemeServices();

		/// <summary>
		/// Publish a Concept Scheme that is already formatted as CTDL JSON-LD, for example that has been exported from CASS
		/// The input must already be in the proper graph format.
		/// Using Request.Content to more easily handle larger frameworks
		/// </summary>
		/// <returns></returns>
		[HttpPost, Route( "ConceptScheme/publishGraph" )]
		public RegistryAssistantResponse PublishStream()
		{
			LoggingHelper.DoTrace( 6, "ConceptSchemeController.PublishStream - Entered" );

			HttpContent requestContent = Request.Content;
			string jsonContent = requestContent.ReadAsStringAsync().Result;
			var request = JsonConvert.DeserializeObject<GraphEntityRequest>( jsonContent );

			LoggingHelper.DoTrace( 6, "ConceptSchemeController.PublishStream - Calling Publish." );

			return PublishGraph( request );
		}

		/// <summary>
		/// Publish concept scheme that is already formatted as CTDL JSON-LD, for example that has been exported from CASS
		/// The input will already be in the proper graph format. 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[ApiExplorerSettings( IgnoreApi = true )]
		[HttpPost, Route( "ConceptScheme/publishGraph2" )]
		public RegistryAssistantResponse PublishGraph( GraphEntityRequest request )
		{
			bool isValid = true;
			//List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.GraphInput == null )
				{
					response.Messages.Add( "Error - please provide a valid Concept Scheme request with the CTDL JSON-LD Graph." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				//pointless here, not enough info
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishGraph request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.CTID, request.RegistryEnvelopeId ) );
				helper = new RequestHelper();
				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CTID, "ConceptScheme", "Publish", 5 );
					string originalCTID = request.CTID ?? "";

					manager.PublishGraph( request, ref isValid, helper );

					response.CTID = request.CTID ?? "".ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if (helper.Messages.Count > 0)
							response.Messages = helper.GetAllMessages();
						UpdateResponse(helper, response);

						//response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
						//response.EnvelopeUrl = manager.credentialRegistryBaseUrl + "envelopes/" + response.RegistryEnvelopeIdentifier;
						
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

		//
		/// <summary>
		/// Handle request to format an ConceptScheme document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "ConceptScheme/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.ConceptScheme == null )
				{
					response.Messages.Add( "Error - please provide a valid Competency Framework request." );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.ConceptScheme.CTID, "ConceptScheme", "Format" );

				response.Payload = manager.FormatAsJson( request, ref isValid, ref messages );
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
		/// Publish a Concept Scheme 
		/// The input will use plain/non-JSON-LD properties 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "ConceptScheme/publish" )]
		public RegistryAssistantResponse Publish( ConceptSchemeRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();
			try
			{
				if ( request == null || request.ConceptScheme == null )
				{
					response.Messages.Add( "Error - please provide a valid Concept Scheme request." );
					return response;
				}
				if ( request.ConceptScheme.CTID == null )
				{
					response.Messages.Add( "Error - please provide a valid CTID for the Concept Scheme request." );
					return response;
				}

				//should not require separate CTID!!
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.ConceptScheme.CTID, request.RegistryEnvelopeId ) );
				helper = new RequestHelper
				{
					OwnerCtid = request.PublishForOrganizationIdentifier
				};
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.ConceptScheme.CTID, "ConceptScheme", "Publish", 5 );
					string originalCTID = request.ConceptScheme.CTID ?? "";

					manager.Publish( request, ref isValid, helper );

					response.CTID = request.ConceptScheme.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if (helper.Messages.Count > 0)
							response.Messages = helper.GetAllMessages();
						UpdateResponse(helper, response);

						//response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
						//response.EnvelopeUrl = manager.credentialRegistryBaseUrl + "envelopes/" + response.RegistryEnvelopeIdentifier;
						
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
		///// <summary>
		///// Publish competency frameworks that have been exported from CASS
		///// The input will already be in the proper format. 
		///// </summary>
		///// <param name="request"></param>
		///// <returns></returns>
		//[HttpPost, Route( "ConceptScheme/publishSkosGraph" )]
		//private RegistryAssistantResponse PublishSkosGraph( SkosGraphConceptSchemeRequest request )
		//{
		//	bool isValid = true;
		//	//List<string> messages = new List<string>();
		//	var response = new RegistryAssistantResponse();

		//	try
		//	{
		//		if ( request == null || request.ConceptSchemeGraph == null )
		//		{
		//			response.Messages.Add( "Error - please provide a valid Skos Concept Scheme graph request." );
		//			return response;
		//		}

		//		LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishSkos request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.CTID, request.RegistryEnvelopeId ) );
		//		helper = new RequestHelper
		//		{
		//			OwnerCtid = request.PublishForOrganizationIdentifier
		//		};
		//		if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
		//		{
		//			response.Messages.Add( statusMessage );
		//		}
		//		else
		//		{
		//			helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CTID, "ConceptScheme", "PublishSkos", 5 );
		//			string originalCTID = request.CTID ?? "";

		//			manager.PublishFromSkosGraph( request, ref isValid, helper );

		//			response.CTID = request.CTID;
		//			response.Payload = helper.Payload;
		//			response.Successful = isValid;
		//			if ( isValid )
		//			{
		//				if (helper.Messages.Count > 0)
		//					response.Messages = helper.GetAllMessages();
		//				UpdateResponse(helper, response);

		//				//response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
		//				//response.EnvelopeUrl = manager.credentialRegistryBaseUrl + "envelopes/" + response.RegistryEnvelopeIdentifier;

		//			}
		//			else
		//			{
		//				response.Messages = helper.GetAllMessages();
		//			}
		//		}
		//	}
		//	catch ( Exception ex )
		//	{
		//		response.Messages.Add( ex.Message );
		//		response.Successful = false;
		//	}
		//	return response;
		//}

		///// <summary>
		///// Publish competency frameworks that have been exported from CASS
		///// The input will already be in the proper format. 
		///// </summary>
		///// <param name="request"></param>
		///// <returns></returns>
		//[HttpPost, Route( "ConceptScheme/publishFromSkos" )]
		//public RegistryAssistantResponse PublishSkos( SkosConceptSchemeRequest request )
		//{
		//	bool isValid = true;
		//	//List<string> messages = new List<string>();
		//	var response = new RegistryAssistantResponse();

		//	try
		//	{
		//		if ( request == null || request.ConceptScheme == null )
		//		{
		//			response.Messages.Add( "Error - please provide a valid Concept Scheme request." );
		//			return response;
		//		}

		//		LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishSkos request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.CTID, request.RegistryEnvelopeId ) );
		//		helper = new RequestHelper
		//		{
		//			OwnerCtid = request.PublishForOrganizationIdentifier
		//		};
		//		if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
		//		{
		//			response.Messages.Add( statusMessage );
		//		}
		//		else
		//		{
		//			helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CTID, "ConceptScheme", "PublishSkos", 5 );
		//			string originalCTID = request.CTID ?? "";

		//			manager.PublishFromSkos( request, ref isValid, helper );

		//			response.CTID = request.CTID;
		//			response.Payload = helper.Payload;
		//			response.Successful = isValid;
		//			if ( isValid )
		//			{
		//				response.RegistryEnvelopeIdentifier = helper.RegistryEnvelopeId;
		//				if ( helper.Messages.Count > 0 )
		//					response.Messages = helper.GetAllMessages();
		//			}
		//			else
		//			{
		//				response.Messages = helper.GetAllMessages();
		//			}
		//		}
		//	}
		//	catch ( Exception ex )
		//	{
		//		response.Messages.Add( ex.Message );
		//		response.Successful = false;
		//	}
		//	return response;
		//}

		/// <summary>
		/// Delete request of an ConceptScheme by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "ConceptScheme/delete" )]
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
		/// Delete request of an ConceptScheme by EnvelopeId and CTID
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "ConceptScheme/envelopeDelete" )]
		public RegistryAssistantDeleteResponse CustomDelete(EnvelopeDelete request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
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
				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage, true ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					//only valid token at this time is from CER
					RegistryServices cer = new RegistryServices( "ConceptScheme", "", request.CTID );
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
