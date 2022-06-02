using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishCompetencyFrameworks
	{
		#region examples
		public string PublishSimpleRecord()
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();

			//A simple CompetencyFramework object - see github for full class definition
			var myData = new CompetencyFramework()
			{
				name = "My Competency Framework Name",
				description = "This is some text that describes my Competency Framework.",
				CTID = myCTID,
				publicationStatusType="Published",
				publisher = new List<string>() { organizationIdentifierFromAccountsSite }
			};

			//This holds the data and the identifier (CTID) for the owning organization
			var myRequest = new CompetencyFrameworkRequest()
			{
				CompetencyFramework = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add competencies
			//example of a flat framework
			myRequest.Competencies.Add( MapCompetency( myCTID, "Looks both ways before crossing street" ) );
			myRequest.Competencies.Add( MapCompetency( myCTID, "Looks before leaping" ) );
			myRequest.Competencies.Add( MapCompetency( myCTID, "Deals with the faults of others as gently as their own" ) );
			myRequest.Competencies.Add( MapCompetency( myCTID, "Knows what he/she knows and does not know what he/she does not know " ) );

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
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/CompetencyFramework/publish/";
				// Perform the actual publish action and return the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			};

			//Return the result
			return result;
		}
		public static Competency MapCompetency( string frameworkCTID, string competency )
		{
			Competency output = new Competency()
			{
				competencyText_map = new LanguageMap( competency ),
				CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
				isPartOf = frameworkCTID
			};
			//add keywords
			//output.conceptKeyword_maplist = new LanguageMapList( new List<string>() { "concept 1", "concept 2", "concept 3" } );
			//output.conceptKeyword_maplist.Add( "fr", new List<string>() { "le concept un", "la concept deux", "les concept thois" } );
			return output;
		}

		#endregion
	

	}
}

