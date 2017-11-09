using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;

using System.Web.Http;
using RA.Models;
using RA.Models.Input;
using Newtonsoft.Json;
using RA.Services;
using Utilities;
namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Registry Assistant for Condition Manifests
	/// </summary>
	public class ConditionManifestController : ApiController
	{

		/// <summary>
		/// Handle request to format a ConditionManifest document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "conditionManifest/format" )]
		public RegistryAssistantResponse Format( ConditionManifestRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.ConditionManifest == null )
				{
					response.Messages.Add( "Error - please provide a valid ConditionManifest request." );
					return response;
				}
				//foreach ( ConditionManifest item in request.ConditionManifests )
				//{}
				string origCTID = request.ConditionManifest.Ctid ?? "";
				RequestStatus reqStatus = new RequestStatus();
				reqStatus.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );
				//do this in controller

				response.Payload = ConditionManifestServices.FormatAsJson( request, ref isValid, reqStatus );
				response.Successful = isValid;
				if ( isValid )
				{
					//check for any (likely warnings) messages
					if ( reqStatus.Messages.Count > 0)
					{

					}
				}
				else
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
		/// Publish a ConditionManifest to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "conditionManifest/publish" )]
		public RegistryAssistantResponse Publish( ConditionManifestRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";
			
			try
			{
				if ( request == null || request.ConditionManifest == null )
				{
					response.Messages.Add( "Error - please provide a valid ConditionManifest request." );
					return response;
				}

				if ( !ServiceHelper.ValidateApiKey( request.APIKey, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					string origCTID = request.ConditionManifest.Ctid ?? "";
					RA.Models.RequestStatus reqStatus = new RequestStatus();
					ConditionManifestServices.Publish( request, ref isValid, reqStatus );
					response.CTID = request.ConditionManifest.Ctid;
					response.Payload = reqStatus.Payload;
					response.Successful = isValid;

					if ( isValid )
					{
						response.RegistryEnvelopeIdentifier = reqStatus.RegistryEnvelopeId;
						response.CTID = request.ConditionManifest.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this ConditionManifest. If not provided, the future request will be treated as a new ConditionManifest." );
						}

						//would want to return any error messages
						if (reqStatus.Messages.Count > 0)
							response.Messages = reqStatus.GetAllMessages();
					}
					else
					{
						//response.Messages = messages;
						response.Messages = reqStatus.GetAllMessages();
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
