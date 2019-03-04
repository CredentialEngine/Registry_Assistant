using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishConditionManifest
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
			var myCTID = "ce-" + Guid.NewGuid().ToString();
			//DataService.SaveCostManifestCTID( myCTID );

			//A simple ConditionManifest object - see below for sample class definition
			var myEntity = new ConditionManifest()
			{
				Name = "My ConditionManifest Name",
				Description = "This is some text that describes my assessment.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com?t=subjectwebpage",
				//if this ID/CTID is not known, use a third party reference
				ConditionManifestOf = new OrganizationReference()
				{
					Type = "CredentialOrganization",
					Name = "Owning Organization of this CostManifest",
					SubjectWebpage = "http://example.com?t=subjectWebpage"
				}
			};

			myEntity.RequiredConditions = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Name = "My Requirements",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				};

			//This holds the assessment and the identifier (CTID) for the owning organization
			var myData = new CostManifestRequest()
			{
				ConditionManifest = myEntity,
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
				var publishEndpoint = "https://credentialengine.org/sandbox/assessment/publish/";

				//Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}

			//Return the result
			return result;
		}

		public class CostManifestRequest
		{
			public ConditionManifest ConditionManifest { get; set; }
			public string PublishForOrganizationIdentifier { get; set; }
		}

		public class ConditionManifest
		{
			public string Name { get; set; }
			public OrganizationReference ConditionManifestOf { get; set; } = new OrganizationReference();
			public string Description { get; set; }
			public string SubjectWebpage { get; set; } //URL
			public string CTID { get; set; }
			public List<ConditionProfile> RequiredConditions { get; set; } = new List<ConditionProfile>();
			//Other properties
		}

	}
}
