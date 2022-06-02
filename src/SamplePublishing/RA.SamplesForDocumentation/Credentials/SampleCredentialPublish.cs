using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;

using RA.Models.Input;

using MyCredential = RA.Models.Input.Credential;

namespace RA.SamplesForDocumentation.Credentials
{

	public class SampleCredentialPublish
	{
		/// <summary>
		/// Publish a simple credentail
		/// Sample for use on credreg.net
		/// <see cref="https://credreg.net/registry/assistant#credential_codesample"/>
		/// </summary>
		/// <returns></returns>
		public string PublishSimpleRecord()
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			// Assign a CTID for the entity being published and keep track of it
			var myCredCTID = "ce-" + Guid.NewGuid().ToString();

			// A simple credential object-see below for sample class definition
			var myData = new MyCredential()
			{
				Name = "My Credential Name",
				Description = "This is some text that describes my credential.",
				CTID = myCredCTID,
				SubjectWebpage = "https:/example.org/credential/1234",
				CredentialType = "ceterms:Certificate",
				CredentialStatusType = "Active",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				Naics = new List<string>() { "333922", "333923", "333924" },
				Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Description = "My requirements for this credential",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" },
						TargetLearningOpportunity = new List<EntityReference>()
						{
							new EntityReference()
							{
								Type="LearningOpportunity",
								CTID="ce-" + Guid.NewGuid().ToString().ToLower()
							}
						}
					}
				}
			};

			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="One University Plaza",
					City="Springfield",
					PostalCode="62703",
					AddressRegion="IL",
					Country="United States"
				}
			};
			//This holds the Assessment and the identifier (CTID) for the owning organization
			var myRequest = new CredentialRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the request object
			var payload = JsonConvert.SerializeObject( myRequest );
			// Use HttpClient to perform the publish
			using ( var client = new HttpClient() )
			{
				// Accept JSON
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
				// Add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				// Format the json as content
				var content = new StringContent( payload, Encoding.UTF8, "application/json" );
				// The endpoint to publish to
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/credential/publish/";
				// Perform the actual publish action and return the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			};
			return result;
		}

		public void CodeSnippets( MyCredential myData)
		{
			//====================	CONNECTIONS ====================
			//Connections between credentials can be published using properties such as
			//- isPreparationFor, PreparationFrom, isAdvancedStandingFor, AdvancedStandingFrom, IsRequiredFor, and IsRecommendedFor. 
			//example of a connection to a credential for which the current credential will prepare a student.

			var isPreparationFor = new Connections
			{
				Description = "This certification will prepare a student for the target credential",
				TargetCredential = new List<EntityReference>()
				{
					//the referenced credential could be for an external credential, not known to be in the credential registry
					new EntityReference()
					{
						Type="MasterDegree",
						Name="Cybersecurity Technology Master's Degree  ",
						Description="A helpful description",
						SubjectWebpage="https://example.org?t=masters"
					}
				}
			};
			myData.IsPreparationFor.Add( isPreparationFor );

			//add credential that prepares for this credential. 
			var preparationFrom = new Connections
			{
				Description = "This credential will prepare a student for this credential",
				TargetCredential = new List<EntityReference>()
				{
					//the referenced credential is known to be in the credential registry, so only the CTID need be provided
					new EntityReference()
					{
						CTID="ce-40c3e860-5034-4375-80e8-f7455ff86a48"
					}
				}
			};
			myData.PreparationFrom.Add( preparationFrom );
		}
	}
}
