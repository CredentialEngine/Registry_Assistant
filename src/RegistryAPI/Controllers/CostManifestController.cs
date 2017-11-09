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
	/// Registry Assistant for Cost Manifests
	/// </summary>
	public class CostManifestController : ApiController
	{

		/// <summary>
		/// Handle request to format a CostManifest document as CTDL Json-LD
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "costManifest/format" )]
		public RegistryAssistantResponse Format( CostManifestRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();

			try
			{
				if ( request == null || request.CostManifest == null )
				{
					response.Messages.Add( "Error - please provide a valid CostManifest request." );
					return response;
				}

				string origCTID = request.CostManifest.Ctid ?? "";

				response.Payload = CostManifestServices.FormatAsJson( request, ref isValid, ref messages );
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
		/// Publish a CostManifest to the Credential Engine Registry
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "costManifest/publish" )]
		public RegistryAssistantResponse Publish( CostManifestRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantResponse();
			string statusMessage = "";
			string payload = "";
			string registryEnvelopeId = "";

			try
			{
				if ( request == null || request.CostManifest == null )
				{
					response.Messages.Add( "Error - please provide a valid CostManifest request." );
					return response;
				}

				if ( !ServiceHelper.ValidateApiKey( request.APIKey, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					string origCTID = request.CostManifest.Ctid ?? "";

					CostManifestServices.Publish( request, ref isValid, ref messages, ref payload, ref registryEnvelopeId );
					response.CTID = request.CostManifest.Ctid;
					response.Payload = payload;

					response.Successful = isValid;

					if ( isValid )
					{
						response.RegistryEnvelopeIdentifier = registryEnvelopeId;
						response.CTID = request.CostManifest.Ctid;
						if ( response.CTID != origCTID )
						{
							response.Messages.Add( "Warning - a CTID was generated for this request. This CTID must be used for any future requests to update this CostManifest. If not provided, the future request will be treated as a new CostManifest." );
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
