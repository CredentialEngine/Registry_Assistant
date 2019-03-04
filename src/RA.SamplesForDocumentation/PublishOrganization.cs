using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
    public class PublishOrganization
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
			//this must be permantently stored in partner system and used with all future updates. 
			var myOrgCTID = "ce-" + Guid.NewGuid().ToString();
			DataService.SaveOrganizationCTID( myOrgCTID );

			//A simple organization object - see below for sample class definition
			var myOrg = new SampleOrganization()
			{
				Name = "My Organization Name",
				Description = "This is some text that describes my organization.",
				CTID = myOrgCTID,
				SubjectWebpage = "http://example.com",
				Type = "ceterms:CredentialOrganization",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				AgentSectorType = "PrivateNonProfit",
				Email = new List<string>() { "info@credreg.net" }
			};
			//use organization reference to add a department for the organization
			myOrg.Department.Add( new OrganizationReference()
			{
				Name = "A Department for my organization",
				Description = "A test Department - third party format",
				SubjectWebpage = "http://example.com?t=testDepartment",
				Type = OrganizationReference.CredentialOrganization
			} );
			//if i know the CTID, then only specify CTID
			myOrg.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//if the CTID is not known, or if not sure a QA organization is in the registry, use a refer
			myOrg.Department.Add( new OrganizationReference()
			{
				Name = "A Quality Assurance Organization",
				SubjectWebpage = "http://example.com/qualityAssuranceIsUs",
				Type = OrganizationReference.QACredentialOrganization
			} );

			//This holds the organization and the identifier (CTID) for the owning organization
			var myData = new OrganizationRequest()
			{
				Organization = myOrg,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the organization request object
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
				var publishEndpoint = "https://credentialengine.org/raSandbox/organization/publish/";
				//Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			//Return the result
			return result;
		}

		/*
		 * 
		 */
		public string ThirdPartyPublishSimpleRecord()
		{
			//Holds the result of the publish action
			var result = "";

			//assign the api key - this is the API key for the third party organization
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

			//this is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

			//Assign a CTID for the entity being published and keep track of it
			var myOrgCTID = "ce-" + Guid.NewGuid().ToString();
			DataService.SaveOrganizationCTID( myOrgCTID );
			//A simple organization object - see below for sample class definition
			var myOrg = new SampleOrganization()
			{
				Name = "My Organization Name",
				Description = "This is some text that describes my organization.",
				CTID = myOrgCTID,
				SubjectWebpage = "http://example.com",
				Type = "ceterms:CredentialOrganization",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				AgentSectorType = "agentSector:PrivateNonProfit",
				Email = new List<string>() { "info@credreg.net" }
			};
			//This holds the organization and the identifier (CTID) for the owning organization
			var myData = new OrganizationRequest()
			{
				Organization = myOrg,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the organization request object
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
				var publishEndpoint = "https://credentialengine.org/raSandbox/organization/publish/";
				//Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			//Return the result
			return result;
		}
		public class DataService
		{
			public static void SaveOrganizationCTID( string ctid )
			{
			}
		}
		public class OrganizationRequest
		{
			public SampleOrganization Organization { get; set; }
			public string PublishForOrganizationIdentifier { get; set; }
		}
		public class SampleOrganization
		{
			public string Type { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string SubjectWebpage { get; set; }
			public string CTID { get; set; }
			public string Image { get; set; }
			public string AgentPurpose { get; set; }
			public string AgentPurposeDescription { get; set; }
			public List<string> AgentType { get; set; }
			public List<string> Address { get; set; }
			public List<string> Email { get; set; }
			public List<string> SocialMedia { get; set; }
			public List<string> Keyword { get; set; }
			public List<string> ServiceType { get; set; }
			public string AgentSectorType { get; set; }
			public List<OrganizationReference> ParentOrganization { get; set; }
			public List<OrganizationReference> Department { get; set; }
			public List<OrganizationReference> AccreditedBy { get; set; }
			public List<OrganizationReference> ApprovedBy { get; set; }
			public List<OrganizationReference> RecognizedBy { get; set; }
			public List<OrganizationReference> RegulatedBy { get; set; }

		}
	}
}
