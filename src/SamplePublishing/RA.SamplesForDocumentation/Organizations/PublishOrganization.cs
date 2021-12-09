using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;
using YourOrganization = RA.SamplesForDocumentation.SampleModels.Organization;
using APIRequestOrganization = RA.Models.Input.Organization;
using APIRequest = RA.Models.Input.OrganizationRequest;

namespace RA.SamplesForDocumentation
{
    public class PublishOrganization
    {
		public string PublishSimpleRecord()
		{

			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			//Assign a CTID for the entity being published and keep track of it
			//this must be permantently stored in partner system and used with all future updates. 
			var myOrgCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			DataService.SaveOrganizationCTID( myOrgCTID );

			//A simple organization object - see below for sample class definition
			var myData = new Organization()
			{
				Name = "My Organization Name",
				Description = "This is some text that describes my organization.",
				CTID = myOrgCTID,
				SubjectWebpage = "http://example.com",
				Type = "ceterms:CredentialOrganization",
				Keyword = new List<string>() { "Credentials", "Technical Training", "Credential Registry Consulting" },
				Email = new List<string>() { "info@myOrg.com" }
			};
			//required-concept from AgentSector: https://credreg.net/ctdl/terms/agentSectorType#AgentSector
			myData.AgentSectorType = "PrivateNonProfit";
			//required-One or more concepts from OrganizationType: https://credreg.net/ctdl/terms/agentType#OrganizationType
			myData.AgentType.Add( "Business" );
			//add addresses and contact points
			var mainAddress = new Place()
			{
				Address1 = "123 Main Street",
				Address2 = "Suite 2", //Address1 and Address2 are concatenated in the registry
				City = "Springfield",
				AddressRegion = "Illinois",
				PostalCode = "62704",
				Country = "United States"
			};
			mainAddress.ContactPoint = new List<ContactPoint>()
			{ 
				new ContactPoint()
				{	ContactType="Information", 
					Name="Toll-Free", 
					PhoneNumbers = new List<string>() {"800-555-1212" }
				}
			};
			myData.Address.Add( mainAddress );

			//Add a tech support contact point without an address, including a phone number and email
			var techSupport = new Place()
			{
				ContactPoint = new List<ContactPoint>()
				{
					new ContactPoint()
					{   ContactType="Tech-Support",
						PhoneNumbers = new List<string>() {"800-555-1212" }, 
						Emails = new List<string>() {"techSupport@myOrg.com"}
					}
				}
			};
			myData.Address.Add( techSupport );


			//use organization reference to add a department for the organization
			myData.Department.Add( new OrganizationReference()
			{
				Name = "A Department for my organization",
				Description = "A test Department - third party format",
				SubjectWebpage = "http://example.com?t=testDepartment",
				Type = OrganizationReference.CredentialOrganization
			} );
			//		QA
			//if you know the CTID, then only specify CTID
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );

			//Add organization that is not in the credential registry (OR the CTID is not known)
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );


			//This holds the organization and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Organization = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the organization request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//Holds the result of the publish action
			var //call the Assistant API
			result = new SampleServices().SimplePost( "organization", "publish", payload, apiKey );
			//Return the result
			return result;
		}
		/// <summary>
		/// Publish using data from an input class (populated from local data stores)
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string Publish( YourOrganization input )
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			//this is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

			APIRequestOrganization myData = new APIRequestOrganization
			{
				Name = input.Name,
				Description = input.Description,
				Type = input.OrganizationType,
				//*** the source data must assign a CTID and use for all transactions
				CTID = input.CTID,
				Image = input.ImageUrl,
				Keyword = input.Keywords
			};
			//required-concept from AgentSector: https://credreg.net/ctdl/terms/agentSectorType#AgentSector
			myData.AgentSectorType = "PrivateNonProfit";
			//required-One or more concepts from OrganizationType: https://credreg.net/ctdl/terms/agentType#OrganizationType
			myData.AgentType.Add( "Business" );
			//add addresses and contact points
			if ( input.Address != null && input.Address.Count > 0)
			{
				foreach(var item in input.Address )
				{
					//a minimum check for data, probably would be more expansive
					if ( !string.IsNullOrWhiteSpace( item.City ) )
					{
						var mainAddress = new Place()
						{
							Address1 = item.Address1,
							Address2 = item.Address2, //Address1 and Address2 are concatenated in the registry
							City = item.City,
							AddressRegion = item.AddressRegion,
							PostalCode = item.PostalCode,
							Country = item.Country
						};
						myData.Address.Add( mainAddress );
					}
				}
				
			}
			//an organization can optional publish a 'owns' entry from all of its credentials, etc. 
			//Alternately it is acceptable to only published the ownedBy assertions with credentials, etc. 
			myData.Owns.Add( new EntityReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//Add QA assertions such as Accredited by
			//CTID for Higher learning commission.
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );


			//Create the 'request' class
			//This holds the organization and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Organization = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//Use HttpClient to perform the publish
			using ( var client = new HttpClient() )
			{
				//Accept JSON
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/payload" ) );
				//add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				//Format the payload as content
				var content = new StringContent( payload, Encoding.UTF8, "application/payload" );
				//The endpoint to publish to
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/credential/publish/";
				//Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			//Return the result
			return result;
		}
		/*
		 * In progress, not complete!!!!!!!!!!!
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
			var myOrgCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			DataService.SaveOrganizationCTID( myOrgCTID );
			//A simple organization object - see below for sample class definition
			var myData = new Organization()
			{
				Name = "My Organization Name",
				Description = "This is some text that describes my organization.",
				CTID = myOrgCTID,
				SubjectWebpage = "http://example.com",
				Type = "ceterms:CredentialOrganization",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				Email = new List<string>() { "info@myOrg.com" }
			};
			//required-concept from AgentSector: https://credreg.net/ctdl/terms/agentSectorType#AgentSector
			myData.AgentSectorType = "PrivateNonProfit";
			//required-One or more concepts from OrganizationType: https://credreg.net/ctdl/terms/agentType#OrganizationType
			myData.AgentType.Add( "Business" );
			//This holds the organization and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Organization = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the organization request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "organization", "publish", payload, apiKey );
			//Return the result
			return result;
		}
		public class DataService
		{
			public static void SaveOrganizationCTID( string ctid )
			{
			}
		}

	}
}
