using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishCostManifest
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

			//A simple CostManifest object - see below for sample class definition
			var myData = new CostManifest()
			{
				Name = "My CostManifest Name",
				Description = "This is some text that describes my assessment.",
				CTID = myCTID,
				CostDetails = "http://www.credreg.net/assessment/1234",
				CostManifestOf = new OrganizationReference()
				{
					CTID = organizationIdentifierFromAccountsSite
				}
			};

			CostProfile cp = new CostProfile()
			{
				Name = "My Cost Profile",
				Description = "Required description of a cost profile.",
				Currency = "USD",
				StartDate = "2017-09-01",
				Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
			};
			cp.CostItems.Add( new CostProfileItem()
			{
				Price = 99.99M,
				PaymentPattern = "yearly",
				DirectCostType = "costType:Application"
			} );
			myData.EstimatedCost.Add( cp );

			//This holds the resource being published and the identifier (CTID) for the owning organization
			var myRequest = new CostManifestRequest()
			{
				CostManifest = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "CostManifest", "publish", payload, apiKey );

			//Return the result
			return result;
		}

	}
}
