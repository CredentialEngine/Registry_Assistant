using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishAssessment
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
			//DataService.SaveAssessmentCTID( myCTID );

			//A simple assessment object - see below for sample class definition
			var myAsmt = new SampleAssessment()
			{
				Name = "My Assessment Name",
				Description = "This is some text that describes my assessment.",
				CTID = myCTID,
				SubjectWebpage = "http://www.credreg.net/assessment/1234",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				AssessmentMethodType = new List<string>() { "assessMethod:Exam", "assessMethod:Performance" },
				Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Name = "My Requirements",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				}
			};

			//This holds the assessment and the identifier (CTID) for the owning organization
			var myData = new AssessmentRequest()
			{
				Assessment = myAsmt,
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

		public class AssessmentRequest
		{
			public SampleAssessment Assessment { get; set; }
			public string PublishForOrganizationIdentifier { get; set; }
		}

		public class SampleAssessment
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public string SubjectWebpage { get; set; }
			public string CTID { get; set; }
			public List<string> Keyword { get; set; }
			public List<string> AssessmentMethodType { get; set; }
			public List<string> AssessmentUseType { get; set; }
			public List<string> DeliveryType { get; set; }
			public List<ConditionProfile> Requires { get; set; }
			public List<ConditionProfile> Recommends { get; set; }
			//Other properties
		}

		public class ConditionProfile
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public List<string> Condition { get; set; }
			//Othe properties
		}
	}
}
