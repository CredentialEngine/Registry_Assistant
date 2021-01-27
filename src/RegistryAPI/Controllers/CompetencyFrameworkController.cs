using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using RA.Services;

using Utilities;

using EntityRequest = RA.Models.Input.CompetencyFrameworkRequest;
using GraphRequest = RA.Models.Input.CompetencyFrameworkGraphRequest;
using ServiceHelper = RA.Services.ServiceHelperV2;


namespace RegistryAPI.Controllers
{
	public class CompetencyFrameworkController : BaseController
    {
        string thisClassName = "CompetencyFrameworkController";
		string controllerEntity = "CompetencyFramework";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
        string statusMessage = "";
        CompetencyFrameworkServices manager = new CompetencyFrameworkServices();
		//
		/// <summary>
		/// Request to format a competence framework and all related competencies.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "CompetencyFramework/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.CompetencyFramework == null )
				{
					response.Messages.Add( "Error - please provide a valid Competency Framework request." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				ServiceHelper.LogInputFile( request, request.CompetencyFramework.CTID, "CompetencyFramework", "Format" );

				response.Payload = new CompetencyFrameworkServices().FormatAsJson( request, ref isValid, ref messages );
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
		/*
			public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.CompetencyFramework == null )
				{
					response.Messages.Add( "Error - please provide a valid Competency Framework request." );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.CompetencyFramework.Ctid, "CompetencyFramework", "Format" );

				response.Payload = new CompetencyFrameworkServices().FormatAsJson( request, ref isValid, ref messages );
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

	*/
		#region Publish methods

		/// <summary>
		/// Use Request.Content to more easily handle larger frameworks
		/// </summary>
		/// <returns></returns>
		[HttpPost, Route( "CompetencyFramework/publishGraph" )]
		public RegistryAssistantResponse PublishGraph()
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();
			LoggingHelper.DoTrace( 6, "CompetencyFramework/publishGraph. Entered" );
			HttpContent requestContent = Request.Content;
			string outputCTID = "";
			try
			{
				string jsonContent = requestContent.ReadAsStringAsync().Result;
				LoggingHelper.DoTrace( 5, string.Format("CompetencyFramework/publishGraph. requestLength: {0}, starts: '{1}' ....", jsonContent.Length, jsonContent.Substring(0, jsonContent.Length > 100 ? 100 : jsonContent.Length )) );
				var request = JsonConvert.DeserializeObject<GraphRequest>( jsonContent );

				if ( request == null || request.CompetencyFrameworkGraph == null )
				{
					response.Messages.Add( "Error - please provide a valid Competency Framework request with the JSON Graph." );
					return response;
				}
				
				//extract ctid
				if ( !string.IsNullOrWhiteSpace(request.CTID) && request.CTID.Length == 39 )
				{
					outputCTID = request.CTID;
				}
				else if ( ( request.CompetencyFrameworkGraph.Id ?? "" ).IndexOf( "/ce-" ) > 0 )
				{
					outputCTID = request.CompetencyFrameworkGraph.Id.Substring( request.CompetencyFrameworkGraph.Id.IndexOf( "/ce-" ) + 1 );
				}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, PublishForOrganizationIdentifier: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.PublishForOrganizationIdentifier, request.RegistryEnvelopeId ) );

				//****as there is no CTID in input, write file with returned ctid
				ServiceHelper.LogInputFile( request, outputCTID, "CompetencyFramework", "PublishGraph", 5 );

				helper = new RequestHelper();
				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					
					//actually need the serializedInput to write to history
					helper.SerializedInput = JsonConvert.SerializeObject( request, ServiceHelper.GetJsonSettings() );
					manager.PublishGraph( request, ref isValid, helper, ref outputCTID );


					response.CTID = outputCTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if( isValid )
					{
						if( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();
						UpdateResponse( helper, response );
					}
					else
					{
						response.Messages = helper.GetAllMessages();
					}
				}
			}
			catch( Exception ex )
			{
				LoggingHelper.LogError( ex, "CompetencyFramework/publishGraph" );
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		}

		/// <summary>
		/// Request to publish a competence framework and all related competencies.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[ HttpPost, Route( "CompetencyFramework/publish" )]
        public RegistryAssistantResponse Publish( EntityRequest request )
        {
            bool isValid = true;
            //List<string> messages = new List<string>();
            var response = new RegistryAssistantResponse();
            //if (1 == 1)
            //{
            //    response.Messages.Add( "NOTICE - this endpoint is not available to the public at this time." );
            //    return response;
            //}

			try
			{
				if ( request == null || request.CompetencyFramework == null )
				{
					response.Messages.Add( "Error - please provide a valid Competency Framework request." );
					return response;
				}
				//if ( string.IsNullOrWhiteSpace( request.CTID ) )
				//{
				//	response.Messages.Add( "Error - please provide a valid CTID for this framework" );
				//	return response;
				//}
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.CompetencyFramework.CTID ?? "NO_CTID", request.RegistryEnvelopeId ) );
				helper = new RequestHelper();
				helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.CompetencyFramework.CTID, "CompetencyFramework", "Publish", 5 );
					string originalCTID = request.CompetencyFramework.CTID ?? "";

					new CompetencyFrameworkServices().Publish( request, ref isValid, helper );

					response.CTID = request.CompetencyFramework.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();

						UpdateResponse(helper, response);

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

		/// <summary>
		/// Publish competency frameworks that have been exported from CASS
		/// The input will already be in the proper format. 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "CompetencyFramework/publishGraphOLD" )]
        public RegistryAssistantResponse PublishGraphOLD( GraphRequest request )
        {
            bool isValid = true;
            var response = new RegistryAssistantResponse();

			HttpContent requestContent = Request.Content;
			string jsonContent = requestContent.ReadAsStringAsync().Result;
			var data = JsonConvert.DeserializeObject<GraphRequest>( jsonContent );

			//Request.InputStream.Position = 0;
			//var requestData = JObject.Parse( new StreamReader( Request.InputStream ).ReadToEnd() );
			//var data = requestData[ "Requests" ].ToObject<List<GraphRequest>>();

			try
            {
                if ( request == null || request.CompetencyFrameworkGraph == null )
                {
                    response.Messages.Add( "Error - please provide a valid Competency Framework request with the JSON Graph." );
                    return response;
                }

                LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish request. IPaddress: {1}, PublishForOrganizationIdentifier: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.PublishForOrganizationIdentifier, request.RegistryEnvelopeId ) );
                helper = new RequestHelper();
                helper.OwnerCtid = request.PublishForOrganizationIdentifier.ToLower();
                if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
                {
                    response.Messages.Add( statusMessage );
                }
                else
                {
					string outputCTID = "";
					//actually need the serializedInput to write to history
					helper.SerializedInput = JsonConvert.SerializeObject( request, ServiceHelper.GetJsonSettings() );
					manager.PublishGraph( request, ref isValid, helper, ref outputCTID );

					//****as there is no CTID in input, write file with returned ctid
					ServiceHelper.LogInputFile( request, outputCTID, "CompetencyFramework", "PublishGraph", 5 );

					response.CTID = outputCTID.ToLower();
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


		#endregion

		/// <summary>
		/// All deletes are done thru the publisher at this time
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "CompetencyFramework/delete" )]
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

		/// <summary>
		/// Delete request of an CompetencyFramework by EnvelopeId and CTID
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "CompetencyFramework/envelopeDelete" )]
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
					RegistryServices cer = new RegistryServices( "CompetencyFramework", "", request.CTID );
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
 