using System;
using System.Collections.Generic;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.RubricRequest;
using RA.Services;
using Mgr = RA.Services.RubricServices;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
    public class RubricController : BaseController
	{
		string statusMessage = "";
		readonly string thisClassName = "RubricController";
		readonly string controllerEntity = "Rubric";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		Mgr mgr = new Mgr();

		/// <summary>
		/// Request to format a Rubric and all related components.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Rubric/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.Rubric == null )
				{
					response.Messages.Add( "Error - please provide a valid Rubric request." );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.Rubric.CTID, "Rubric", "Format" );

				response.Payload = new RubricServices().FormatAsJson( request, ref isValid, ref messages );
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
		/// Request to publish a Rubric and all related components.
		/// NOT AVAILABLE IN PRODUCTION AT THIS TIME
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Rubric/publish" )]
		public RegistryAssistantResponse Publish( EntityRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();
			List<string> messages = new List<string>();
			try
			{
				if ( request == null || request.Rubric == null  )
				{
					response.Messages.Add( "Error - please provide a valid Rubric request with a Rubric." );
					return response;
				}
				if ( !ServiceHelper.IsCtidValid( request.Rubric.CTID, "Rubric CTID", ref messages ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for the Rubric." );
					return response;
				}
				if ( !ServiceHelper.IsCtidValid( request.PublishForOrganizationIdentifier, "Rubric PublishForOrganizationIdentifier", ref messages ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for the PublishForOrganizationIdentifier." );
					return response;
				}
				if ( messages.Count > 0 )
				{
					response.Messages.AddRange( messages );
					return response;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Rubric.CTID, request.RegistryEnvelopeId ) );
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
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Rubric.CTID, "Rubric", "Publish", 5 );
					string originalCTID = request.Rubric.CTID ?? "";

					new RubricServices().Publish( request, ref isValid, helper );

					response.CTID = request.Rubric.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();
						UpdateResponse(helper, response);
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
		/// Delete request of an Rubric by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "Rubric/delete" )]
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

	}
}
