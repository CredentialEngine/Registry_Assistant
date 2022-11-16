using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using RA.Models.Input;

using MyAssessment = RA.Models.Input.Assessment;
namespace RA.SamplesForDocumentation.Assessments
{
	public class AssessmentPublisher
	{
		/// Publish a simple assessment profile
		/// Sample for use on credreg.net
		/// <see cref="https://credreg.net/registry/assistant#assessment_codesample"/>
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

			// A simple Assessment object-see below for sample class definition
			var myData = new MyAssessment()
			{
				Name = "My Assessment Name",
				Description = "This is some required text that describes my Assessment.",
				CTID = myCredCTID,
				SubjectWebpage = "https:/example.org/Assessment/1234",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "Assessments", "Technical Information" },
				AssessmentMethodType = new List<string>() { "assessMethod:Exam", "assessMethod:Performance" },
				Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Description = "My requirements for this Assessment",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				}
			};

			//An assessment must have at one of AvailableOnlineAt, AvailabilityListing or AvailableAt
			myData.AvailableOnlineAt = new List<string>() { "https://example.com/availableOnlineAt" };
			myData.AvailabilityListing = new List<string>() { "https://example.com/AvailabilityListing" };
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//
			//A assessment *must* be connected to a credential in order to be published.
			//The connection can be made using a Required condition profile in the Credential or using a RequiredFor Connection from the assessment
			myData.IsRequiredFor = new List<Connections>()
			{
				new Connections()
				{
					Description="This assessment is required for the 'Acme Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //optional, but helpful
							CTID="ce-f5d9bf2a-d930-4e77-a69b-85788943851c"
						}
					}
				}
			};
			//
			//This holds the Assessment and the identifier (CTID) for the owning organization
			var myRequest = new AssessmentRequest()
			{
				Assessment = myData,
				DefaultLanguage = "en-US",
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
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/Assessment/publish/";
				// Perform the actual publish action and return the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			};
			return result;
		}

	}//class
}
