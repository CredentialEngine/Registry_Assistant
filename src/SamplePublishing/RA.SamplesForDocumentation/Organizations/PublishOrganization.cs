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
using RA.SamplesForDocumentation.SupportingData;

namespace RA.SamplesForDocumentation
{
    public class PublishOrganization
    {
		public string PublishSimpleRecord()
		{

			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
            if ( string.IsNullOrWhiteSpace( apiKey ) )
            {
                //ensure you have added your apiKey to the app.config
            }
            // This is the CTID of the organization that owns the data being published
            var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
            if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
            {
                //ensure you have added your organization account CTID to the app.config
            }
			//
             //Assign a CTID for the entity being published and keep track of it
             //this must be permantently stored in partner system and used with all future updates. 
            var myOrgCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//

			//A simple organization object - see below for sample class definition
			var myData = new Organization()
			{
				Name = "My API Example Organization Name",
				Description = "This is some text that describes my organization.",
				LifeCycleStatusType = "Active",
				CTID = myOrgCTID,
				SubjectWebpage = "http://example.com",
				Type = "ceterms:CredentialOrganization",
				Keyword = new List<string>() { "Credentials", "Technical Training", "Credential Registry Consulting" },
				Email = new List<string>() { "info@myOrg.com" }
			};
			//
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
			//an organization can have multiple addresses, like for many campuses
			myData.Address.Add( new Place()
			{
				Name = "Main Campus",
				Address1 = "200 Daniels Way",
				City = "Bloomington",
				AddressRegion = "Indiana",
				PostalCode = "47704"
			} );
			myData.Address.Add( new Place()
            {
				Name="Evansville Campus",
				Address1 = "2501 N. First Avenue", City ="Evansville", AddressRegion="Indiana", PostalCode="47710"
            } );
			myData.Address.Add( new Place()
			{
				Name = "Madison Campus",
				Address1 = "590 Ivy Tech Drive",
				City = "Madison",
				AddressRegion = "Indiana",
				PostalCode = "47250"
			} );
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
			myData.AdministrationProcess.Add(ProcessProfiles.GetAdministrativeProcessProfile( myOrgCTID ));
            myData.DevelopmentProcess.Add( ProcessProfiles.GetDevelopementProcessProfile() );

            /*
             * 2023-03-31 VerificationServiceProfile is now obsolete. A VerificationServiceProfile is published separately with a CTID. 
            myData.VerificationServiceProfile.Add( FormatVerificationServiceProfile( myOrgCTID ) );
			*/
            //Organization will now use HasVerificationService as a list of CTIDs for published VerificationServiceProfiles
            myData.HasVerificationService = new List<string>()
			{
                "ce-73bb7406-5282-414c-b7e8-aefaad4a64fe", "ce-51595ce1-6e4a-4dac-bd44-2c076c7698a4"
            };
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
        public VerificationServiceProfile FormatVerificationServiceProfile( string orgCTID )
        {
			VerificationServiceProfile output = new VerificationServiceProfile()
			{
				Description = "Sample verification profile",
				DateEffective="2020-01-01",
                HolderMustAuthorize=true,
				SubjectWebpage= "https://example.com/ourVerificationSite",
                VerificationDirectory = new List<string>() { "https://example.com/ourVerificationDirectory" },
				VerificationService = new List<string>() { "https://example.com/ourActualVerificationServices" },
				VerificationMethodDescription = "A summary of our verification methods."
			};
			//offeredBy will be required once standalone
			output.OfferedBy = new List<OrganizationReference>()
			{
				new OrganizationReference() { Type="Organization", CTID= orgCTID }
			};
			//list of target credentials
			//these will have to have been published to the registry
			output.TargetCredential = new List<EntityReference>()
			{
				new EntityReference()
				{
					Type="Certification",
					CTID= "ce-969da20e-c127-4175-93f3-0722027ca7fc",
				},
				new EntityReference()
                {
                    Type="Certification",
                    CTID= "ce-652f6f2c-4fff-45d0-9b2f-44a5bb61f927",
                },
				new EntityReference()
                {
                    Type="Certification",
                    CTID= "ce-c7619be1-e35d-4e9e-b921-9d463f9dc15f",
                }
            };
			return output;
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

			//assign the api key - this is the API key for the third party organization (not the data owner)
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
