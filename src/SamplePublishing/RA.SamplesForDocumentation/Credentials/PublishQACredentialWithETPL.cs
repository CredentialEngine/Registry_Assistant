using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;
using YourCredential = RA.SamplesForDocumentation.SampleModels.Credential;
using APIRequestCredential = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation.Credentials
{
	public class PublishQACredentialWithETPL
	{
		/// <summary>
		/// Publish a QualityAssurance credential with HasETPLResource = list of credentials on a (for example) state's ETPL list
		/// </summary>
		/// <param name="doingPublish"></param>
		/// <returns></returns>
		public string Publish( bool doingPublish = false )
		{
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-18d7670d-372c-4170-9131-95b83a0d66ec"; //"ce-" + Guid.NewGuid().ToString();

			//A simple credential object - see below for sample class definition
			var myData = new Credential()
			{
				Name = "My Quality Assurance Credential with ETPL resources",
				Description = "This is some text that describes my quality assurance credential particularly related to the list of credential that are the state ETPL list.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com/?t=qacredential",
				CredentialType = "ceterms:QualityAssuranceCredential",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "ETPL", "QualityAssurance" }
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//list of CTIDs for credentials that have been published to the registry
			//API will validate that the target credential exists. If not found, the request will be rejected. 
			myData.HasETPLResource = new List<string>()
			{
				"ce-3ee672ff-4ccc-4331-b98b-94f47d265c57",
				"ce-c39a8e9f-f436-4f63-933f-3da67da62460",
				"ce-0e9b268b-72d4-4fba-ba19-91fcfdcc076d"
			};

			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "credential",
				RequestType = doingPublish ? "publish" : "format",
				OrganizationApiKey = apiKey,
				CTID = myRequest.Credential.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;
		}

	}
}
