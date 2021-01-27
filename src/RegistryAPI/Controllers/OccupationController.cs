using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using MJ = RA.Models.JsonV2;

using RA.Services;

using Utilities;

using EntityRequest = RA.Models.Input.OccupationRequest;
using MgrV2 = RA.Services.OccupationServices;
using ServiceHelper = RA.Services.ServiceHelperV2;


namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Occupation Controller
	/// </summary>
	public class OccupationController : BaseController
	{
		string statusMessage = "";
		readonly string thisClassName = "OccupationController";
		readonly string controllerEntity = "Occupation";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		MgrV2 mgr = new MgrV2();


		/// <summary>
		/// Handle request to format an Occupation document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Occupation/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.Occupation == null )
				{
					response.Messages.Add( "Error - please provide a valid Occupation request." );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.Occupation.CTID, "Occupation", "Format" );

				response.Payload = new OccupationServices().FormatAsJson( request, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "Occupation", mgr.CurrentEntityName, request.Occupation.CTID, ref statusMessage );

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
		/// Handle request to publish an Occupation document to the Credential Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Occupation/publish" )]
		public RegistryAssistantResponse Publish( EntityRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.Occupation == null )
				{
					response.Messages.Add( "Error - please provide a valid Occupation request." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishRequest. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Occupation.CTID, request.RegistryEnvelopeId ) );
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
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Occupation.CTID, "Occupation", "Publish", 5 );

					new OccupationServices().Publish( request, ref isValid, helper );

					response.CTID = request.Occupation.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();

						UpdateResponse( helper, response );

						response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Occupation", response.CTID );
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
		/// Use Request.Content to more easily handle larger documents
		/// </summary>
		/// <returns></returns>
		[HttpPost, Route( "Occupation/publishlist" )]
		public List<RegistryAssistantResponse> PublishList()
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();
			var responses = new List<RegistryAssistantResponse>();

			LoggingHelper.DoTrace( 6, thisClassName + "/PublishList. Entered" );
			HttpContent requestContent = Request.Content;
			string outputCTID = "";
			try
			{
				string jsonContent = requestContent.ReadAsStringAsync().Result;
				LoggingHelper.DoTrace( 5, string.Format( thisClassName + "/PublishList. requestLength: {0}, starts: '{1}' ....", jsonContent.Length, jsonContent.Substring( 0, jsonContent.Length > 100 ? 100 : jsonContent.Length ) ) );
				var request = JsonConvert.DeserializeObject<OccupationListRequest>( jsonContent );

				if ( request == null || request.OccupationList == null || request.OccupationList.Count == 0 )
				{
					responses.Add( new RegistryAssistantResponse()
					{
						Messages = new List<string>()
						{ "Error - please provide a valid Occupation List request with a list of Occupations." }
					}
					);
					return responses;
				}

				return PublishList( request );
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "Occupation/publishList" );
				response.Messages.Add( ex.Message );
				response.Successful = false;
				responses.Add( response );
				return responses;
			}
		}
		//
		private List<RegistryAssistantResponse> PublishList( OccupationListRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();
			var responses = new List<RegistryAssistantResponse>();

			try
			{
				if ( request == null || request.OccupationList == null || request.OccupationList.Count == 0 )
				{
					responses.Add( new RegistryAssistantResponse()
					{
						Messages = new List<string>()
						{ "Error - please provide a valid Occupation request with at least one Occupation." }
					}
					);
					return responses;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					responses.Add( new RegistryAssistantResponse()
					{
						Messages = new List<string>()
						{ "Error - please provide a valid CTID for PublishForOrganizationIdentifier." }
					}
					);
					return responses;
				}
				//TODO use this method for input with and without language maps
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.PublishRequest. IPaddress: {1}, Number of Occupation: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.OccupationList.Count, request.RegistryEnvelopeId ) );
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
					foreach ( var item in request.OccupationList )
					{
						helper.ResetHelper();
						EntityRequest r = new EntityRequest()
						{
							DefaultLanguage = request.DefaultLanguage,
							PublishForOrganizationIdentifier = request.PublishForOrganizationIdentifier,
							RegistryEnvelopeId = request.RegistryEnvelopeId,
							Community = request.Community
						};
						//either do the type check here or in services?
						var ctid = "";
						if ( request.HasLanguageMaps )
						{
							r.FormattedOccupation = ( ( Newtonsoft.Json.Linq.JObject )item ).ToObject<MJ.Occupation>();
							r.Occupation = null;
							ctid = r.FormattedOccupation.CTID;
						}
						else
						{
							r.Occupation = ( ( Newtonsoft.Json.Linq.JObject )item ).ToObject<Occupation>();
							r.FormattedOccupation = null;
							ctid = r.Occupation.CTID;
						}
						//TODO - make this optional if greater than 'n' tasks
						if ( request.OccupationList.Count < 50 )
							helper.SerializedInput = ServiceHelper.LogInputFile( request, ctid, "Occupation", "Publish", 5 );

						new OccupationServices().Publish( r, ref isValid, helper );

						response.CTID = ctid.ToLower();
						response.Payload = helper.Payload;
						response.Successful = isValid;
						if ( isValid )
						{
							if ( helper.Messages.Count > 0 )
								response.Messages = helper.GetAllMessages();

							UpdateResponse( helper, response );
							//unlikely to have a single detail?
							//response.CredentialFinderUrl = string.Format( mgr.credentialFinderDetailUrl, "Occupation", response.CTID );
						}
						else
						{
							response.Messages = helper.GetAllMessages();
						}
						responses.Add( response );
					}
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "Occupation/PublishList" );

				response.Messages.Add( ex.Message );
				response.Successful = false;
				responses.Add( response );
			}
			return responses;
		} //


		/// <summary>
		/// Delete request of an Occupation by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "Occupation/delete" )]
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
