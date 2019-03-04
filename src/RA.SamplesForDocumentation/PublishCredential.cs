using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishCredential
	{
		public string PublishSimpleRecord()
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			//this is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			//Assign a CTID for the entity being published and keep track of it
			var myCredCTID = "ce-" + Guid.NewGuid().ToString();
			DataService.SaveCredentialCTID( myCredCTID );

			//A simple credential object - see below for sample class definition
			var myCred = new SampleCredential()
			{
				Name = "My Credential Name",
				Description = "This is some text that describes my credential.",
				CTID = myCredCTID,
				SubjectWebpage = "http://example.com/credential/1234",
				Type = "ceterms:Certificate",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				Naics = new List<string>() { "333922", "333923", "333924" }
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myCred.OwnedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//CTID for Higher learning commission.
			myCred.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			myCred.Requires = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Name = "My Requirements",
					Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
				}
			};
			
			//This holds the credential and the identifier (CTID) for the owning organization
			var myData = new CredentialRequest()
			{
				Credential = myCred,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			var json = JsonConvert.SerializeObject( myData );
			//Use HttpClient to perform the publish
			using ( var client = new HttpClient() )
			{
				//Accept JSON
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
				//add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				//Format the json as content
				var content = new StringContent( json, Encoding.UTF8, "application/json" );
				//The endpoint to publish to
				var publishEndpoint = "https://credentialengine.org/raSandbox/credential/publish/";
				//Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			//Return the result
			return result;
		}


		public class CredentialRequest
		{
			public SampleCredential Credential { get; set; }
			public string PublishForOrganizationIdentifier { get; set; }
		}
		public class SampleCredential
		{
			public string Type { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string SubjectWebpage { get; set; }
			public string CTID { get; set; }
			public List<string> InLanguage { get; set; }
			public List<OrganizationReference> OwnedBy { get; set; }
			public List<OrganizationReference> AccreditedBy { get; set; }
			public string DateEffective { get; set; }
			public List<string> Keyword { get; set; }


			public List<string> AudienceLevelType { get; set; }
			public List<string> Naics { get; set; }
			public List<string> OccupationType { get; set; }
			public List<ConditionProfile> Requires { get; set; }
			public List<ConditionProfile> Recommends { get; set; }
			//Other properties
		}

		public class DataService
		{
			internal static void SaveCredentialCTID( string myCredCTID )
			{
				throw new NotImplementedException();
			}
		}

	}
}
