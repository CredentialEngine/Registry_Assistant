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
			var apiKey = SampleServices.GetMyApiKey();
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//DataService.SaveCostManifestCTID( myCTID );

			//A simple ConditionManifest object - see below for sample class definition
			var myData = new ConditionManifest()
			{
				Name = "My ConditionManifest Name",
				Description = "This is some text that describes my assessment.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com?t=subjectwebpage",
				//if this ID/CTID is not known, use a third party reference
				ConditionManifestOf = new OrganizationReference()
				{
					CTID = organizationIdentifierFromAccountsSite
				}
			};

			myData.Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Name = "My Requirements",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				};

			//This holds the assessment and the identifier (CTID) for the owning organization
			var myRequest = new ConditionManifestRequest()
			{
				ConditionManifest = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "ConditionManifest", "publish", payload, apiKey );

			//Return the result
			return result;
		}

	}
}
