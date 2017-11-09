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
	/// Registry Assistant for Organizations
	/// </summary>
	public class OrganizationController : ApiController
	{

		string statusMessage = "";

		/// <summary>
		/// Handle request to format an Organization document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "organization/format" )]
		public RegistryAssistantResponse Format( OrganizationRequest request)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if( request == null || request.Organization == null )
				{
					response.Messages.Add( "Error - please provide a valid Organization request." );
					return response;
				}
				string origCTID = request.Organization.Ctid ?? "";

				response.Payload = AgentServices.FormatAsJson( request, ref isValid, ref messages );
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
		public RegistryAssistantResponse Publish( OrganizationRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string registryEnvelopeId = "";
			string payload = "";
			try
			{
				if ( !ServiceHelper.ValidateApiKey( request.APIKey, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					if ( request == null || request.Organization == null )
					{
						response.Messages.Add( "Error - please provide a valid Organization request." );
						return response;
					}
					string origCTID = request.Organization.Ctid ?? "";
					
					AgentServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );
					response.Successful = isValid;

					response.Payload = payload;
					if ( isValid )
					{
						response.RegistryEnvelopeIdentifier = registryEnvelopeId;
						response.CTID = request.Organization.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this Organization. If not provided, the future request will be treated as a new Organization." );
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