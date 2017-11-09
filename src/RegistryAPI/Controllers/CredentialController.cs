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


namespace RegistryAPI.Controllers
{
	/// <summary>
	/// Registry Assistant for Credentials
	/// </summary>
    public class CredentialController : ApiController
	{

		/// <summary>
		/// Handle request to format a Credential document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "credential/format" )]
		public RegistryAssistantResponse Format( CredentialRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			
			try
			{
				if ( request == null || request.Credential == null)
				{
					response.Messages.Add( "Error - please provide a valid credential request." );
					return response;
				}
				string origCTID = request.Credential.Ctid ?? "";

				response.Payload = CredentialServices.FormatAsJson( request, ref isValid, ref messages );
				response.Successful = isValid;
				if ( isValid )
				{
					response.Successful = true;
					response.CTID = request.Credential.Ctid;
					if ( response.CTID != origCTID )
					{
						response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this credential. If not provided, the future request will be treated as a new credential." );
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
		/// Publish a credential to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "credential/publish" )]
		public RegistryAssistantResponse Publish( CredentialRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";
			string registryEnvelopeId = "";
			string payload = "";
			try
			{
				if ( request == null || request.Credential == null )
				{
					response.Messages.Add( "Error - please provide a valid credential request." );
					return response;
				}

				if ( !ServiceHelper.ValidateApiKey( request.APIKey, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage);
				}
				else
				{
					string origCTID = request.Credential.Ctid ?? "";
					//CredentialRequestHelper helper = new CredentialRequestHelper( request );

					CredentialServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );
					response.CTID = request.Credential.Ctid;
					response.Payload = payload;
					response.Successful = isValid;

					if ( isValid )
					{
						response.Successful = true;
						response.RegistryEnvelopeIdentifier = registryEnvelopeId;
						response.CTID = request.Credential.Ctid;

						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this credential. If not provided, the future request will be treated as a new credential." );
						}
					}
					else
					{
						//if not valid, could return the payload as reference?
						response.Messages = messages;
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