using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.WorkRoleRequest;
using RA.Services;
using MgrV2 = RA.Services.WorkRoleServices;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
	/// <summary>
	/// WorkRole Controller
	/// </summary>
	public class WorkRoleController : BaseController
	{
		string statusMessage = "";
		readonly string thisClassName = "WorkRoleController";
		readonly string controllerEntity = "WorkRole";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		MgrV2 mgr = new MgrV2();


		/// <summary>
		/// Handle request to format an WorkRole document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "WorkRole/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.WorkRole == null )
				{
					response.Messages.Add( "Error - please provide a valid WorkRole request." );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.WorkRole.CTID, "WorkRole", "Format" );

				response.Payload = new WorkRoleServices().FormatAsJson( request, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "WorkRole", mgr.CurrentEntityName, request.WorkRole.CTID, ref statusMessage );

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
		/// Handle request to publish an WorkRole document to the Credential Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "WorkRole/publish" )]
		public RegistryAssistantResponse Publish( EntityRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.WorkRole == null )
				{
					response.Messages.Add( "Error - please provide a valid WorkRole request." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishRequest. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.WorkRole.CTID, request.RegistryEnvelopeId ) );
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
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.WorkRole.CTID, "WorkRole", "Publish", 5 );

					new WorkRoleServices().Publish( request, ref isValid, helper );

					response.CTID = request.WorkRole.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();

						UpdateResponse( helper, response );

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "WorkRole", response.CTID );
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
		/// Delete request of an WorkRole by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "WorkRole/delete" )]
		public RegistryAssistantDeleteResponse Delete( DeleteRequest request )
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
	}
}
