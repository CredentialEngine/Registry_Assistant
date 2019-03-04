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
				CTID = myCTID,
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
			myEntity.EstimatedCosts.Add( cp );

			//This holds the assessment and the identifier (CTID) for the owning organization
			var myData = new CostManifestRequest()
			{
				CostManifest = myEntity,
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
			public CostManifest CostManifest { get; set; }
			public string PublishForOrganizationIdentifier { get; set; }
		}

		public class CostManifest
		{
			public string Name { get; set; }
			public OrganizationReference CostManifestOf { get; set; } = new OrganizationReference();
			public string Description { get; set; }
			public string CostDetails { get; set; }
			public string CTID { get; set; }
			public List<CostProfile> EstimatedCosts { get; set; } = new List<CostProfile>();
			//Other properties
		}

		public class CostProfile
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public string CostDetails { get; set; }
			public string Currency { get; set; }
			/// <summary>
			/// Start date or effective date of this cost profile
			/// </summary>
			public string StartDate { get; set; }
			/// <summary>
			/// End date or expiry date of this cost profile
			/// </summary>
			public string EndDate { get; set; }
			/// <summary>
			/// List of condtions, containing:
			/// A single condition or aspect of experience that refines the conditions under which the resource being described is applicable.
			/// </summary>
			public List<string> Condition { get; set; }
			public List<CostProfileItem> CostItems { get; set; } = new List<CostProfileItem>();
			//Othe properties
		}
		public class CostProfileItem
		{
			/// <summary>
			/// Initialize
			/// </summary>
			public CostProfileItem()
			{
				ResidencyType = new List<string>();
				AudienceType = new List<string>();
			}

			/// <summary>
			/// Must be a valid CTDL cost type
			/// </summary>
			public string DirectCostType { get; set; }
			/// <summary>
			/// List of Residency items
			/// </summary>
			public List<string> ResidencyType { get; set; }

			/// <summary>
			/// List of Audience Types
			/// </summary>
			public List<string> AudienceType { get; set; }

			/// <summary>
			/// Payment Pattern
			/// </summary>
			public string PaymentPattern { get; set; }
			/// <summary>
			/// Price for this cost - optional
			/// </summary>
			public decimal Price { get; set; }




		}
	}
}
