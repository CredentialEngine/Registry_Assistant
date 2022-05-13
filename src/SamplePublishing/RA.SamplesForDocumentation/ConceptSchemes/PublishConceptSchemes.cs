using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishConceptSchemes
	{

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

			//A simple ConceptScheme object - see github for full class definition
			var myData = new ConceptScheme()
			{
				 Name = "My Concept Scheme Name",
				Description = "This is some text that describes my Concept Scheme.",
				CTID = myCTID,
				Publisher = new List<OrganizationReference>() 
					{ new OrganizationReference() { CTID = organizationIdentifierFromAccountsSite } }
			};

			//This holds the data and the identifier (CTID) for the owning organization
			var myRequest = new ConceptSchemeRequest()
			{
				ConceptScheme = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add competencies
			//example of a flat framework
			myRequest.Concepts.Add( MapConcept( myCTID, "Beginner Level" ) );
			myRequest.Concepts.Add( MapConcept( myCTID, "Medium Level" ) );
			myRequest.Concepts.Add( MapConcept( myCTID, "Advanced Level" ) );
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
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/ConceptScheme/publish/";
				// Perform the actual publish action and return the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			};

			//Return the result
			return result;
		}
		public static Concept MapConcept( string conceptSchemeCTID, string concept )
		{
			var output = new Concept()
			{
				PrefLabel = concept,
				CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
				InScheme = conceptSchemeCTID
			};
			return output;
		}
	}
}

