using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using RA.Models;
using RA.Models.Input;
using Newtonsoft.Json;
using RA.Services;

namespace RegistryAPI.Controllers
{
    public class AssessmentController : ApiController
    {
		string statusMessage = "";

		/// <summary>
		/// Handle request to format an Assessment document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Assessment/format" )]
		public RegistryAssistantResponse Format( AssessmentRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				response.Payload = AssessmentServices.FormatAsJson( request, ref isValid, ref messages );
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
		/// Publish an Assessment to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Assessment/publish" )]
		public RegistryAssistantResponse Publish( AssessmentRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string payload = "";
			string registryEnvelopeId = "";
			try
			{
				if ( !ServiceHelper.ValidateApiKey( request.APIKey, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					if ( request == null || request.Assessment == null )
					{
						response.Messages.Add( "Error - please provide a valid Assessment request." );
						return response;
					}
					string origCTID = request.Assessment.Ctid ?? "";

					AssessmentServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );
					response.Payload = payload;

					response.Successful = isValid;
					if ( isValid )
					{
						response.RegistryEnvelopeIdentifier = registryEnvelopeId;
						response.CTID = request.Assessment.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this Assessment. If not provided, the future request will be treated as a new Assessment." );
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
	}
}
