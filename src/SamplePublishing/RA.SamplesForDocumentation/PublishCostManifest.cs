﻿using System;
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
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			//this is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString();
			//DataService.SaveCostManifestCTID( myCTID );
			//provide the ctid of the organization that 'owns' this cost manifest.
			var myOrgCTID = "ce-" + Guid.NewGuid().ToString();
			//A simple CostManifest object - see below for sample class definition
			var myEntity = new CostManifest()
			{
				Name = "My CostManifest Name",
				Description = "This is some text that describes my assessment.",
				Ctid = myCTID,
				CostDetails = "http://www.credreg.net/assessment/1234",
				CostManifestOf = new OrganizationReference()
				{
					CTID = myOrgCTID
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
			myEntity.EstimatedCost.Add( cp );

			//This holds the assessment and the identifier (CTID) for the owning organization
			var myData = new CostManifestRequest()
			{
				CostManifest = myEntity,
				DefaultLanguage = "en-us",
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

	}
}
