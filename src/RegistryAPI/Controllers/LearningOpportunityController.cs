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
	/// <summary>
	/// Registry Assistant for Learning Opportunities
	/// </summary>
	public class LearningOpportunityController : ApiController
    {
		string statusMessage = "";

		/// <summary>
		/// Handle request to format a Learning Opportunity document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "learningopportunity/format" )]
		public RegistryAssistantResponse Format( LearningOpportunityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				response.Payload = LearningOpportunityServices.FormatAsJson( request, ref isValid, ref messages );
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
		/// Publish a Learning Opportunity to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "learningopportunity/publish" )]
		public RegistryAssistantResponse Publish( LearningOpportunityRequest request )
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

					if ( request == null || request.LearningOpportunity == null )
					{
						response.Messages.Add( "Error - please provide a valid Learning Opportunity request." );
						return response;
					}
					string origCTID = request.LearningOpportunity.Ctid ?? "";

					LearningOpportunityServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );
					response.Payload = payload;
					response.Successful = isValid;

					if ( isValid )
					{
						response.RegistryEnvelopeIdentifier = registryEnvelopeId;
						response.CTID = request.LearningOpportunity.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this Learning Opportunity. If not provided, the future request will be treated as a new Learning Opportunity." );
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
