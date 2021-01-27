using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.JobRequest;
using RA.Services;
using MgrV2 = RA.Services.JobServices;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Job Controller
	/// </summary>
	public class JobController : BaseController
	{
		string statusMessage = "";
		readonly string thisClassName = "JobController";
		readonly string controllerEntity = "Job";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		MgrV2 mgr = new MgrV2();


		/// <summary>
		/// Handle request to format an Job document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Job/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.Job == null )
				{
					response.Messages.Add( "Error - please provide a valid Job request." );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.Job.CTID, "Job", "Format" );

				response.Payload = new JobServices().FormatAsJson( request, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "Job", mgr.CurrentEntityName, request.Job.CTID, ref statusMessage );

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
		/// Handle request to publish an Job document to the Credential Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Job/publish" )]
		public RegistryAssistantResponse Publish( EntityRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.Job == null )
				{
					response.Messages.Add( "Error - please provide a valid Job request." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishRequest. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Job.CTID, request.RegistryEnvelopeId ) );
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
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Job.CTID, "Job", "Publish", 5 );

					new JobServices().Publish( request, ref isValid, helper );

					response.CTID = request.Job.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();

						UpdateResponse( helper, response );

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Job", response.CTID );
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
		/// Delete request of an Job by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "Job/delete" )]
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
