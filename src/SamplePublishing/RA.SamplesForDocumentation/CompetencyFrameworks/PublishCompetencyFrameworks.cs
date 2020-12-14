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
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application / json" ) );
				// Add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				// Format the json as content
				var content = new StringContent( payload, Encoding.UTF8, "application / json" );
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
		#region sample frameworks for ACE
		public string PublishACEPrinFin( bool doingPublish  = false )
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-d3913e58-b578-42be-8c8e-a55c0b66f5ab";// "ce-" + Guid.NewGuid().ToString().ToLower();
			var targetUrl = "https://sandbox.credentialengineregistry.org/resources/";
			//A simple CompetencyFramework
			var myData = new CompetencyFramework()
			{
				name = "ACE - Principles of Finance Learning Outcomes",
				description = "Learning Outcomes for the Principles of Finance program.",
				CTID = myCTID,
				publisherName= new List<string>() { "American Council on Education" },
				publisher = new List<string>() { targetUrl + organizationIdentifierFromAccountsSite }

			};

			//This holds the data to be published and the identifier (CTID) for the owning organization
			var myRequest = new CompetencyFrameworkRequest()
			{
				CompetencyFramework = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//competencies

			myRequest.Competencies = new List<Competency>()
			{
				new Competency()
				{
					CTID = "ce-f3984fc3-4b1f-433b-91a1-d5ad81ed7ca9",
					competencyText = "describe the operating environment of the firm, the role of financial institutions, and the structure of the capital markets and explain their impact on the financial performance of the firm"
				},
				new Competency()
				{
					CTID = "ce-581bac7e-a735-4755-a4cc-313cd705b986",
					competencyText = "interpret financial statements and financial ratios"
				},
				new Competency()
				{
					CTID = "ce-49ce65e5-132e-4430-8eb0-4ff06ec451eb",
					competencyText = "explain the time value of money and calculate future value and present value for single and multiple cash flows"
				},
				new Competency()
				{
					CTID = "ce-82339b05-b49f-4fa4-b0d8-b37d054a420f",
					competencyText = "describe the characteristics of capital market instruments including stocks, bonds, and money market instruments"
				},
				new Competency()
				{
					CTID = "ce-faa89f86-7a55-4ee5-866e-91609a54cd1d",
					competencyText = "explain the capital budgeting process and the basic elements of working capital management"
				},
				new Competency()
				{
					CTID = "ce-07428927-1794-40ed-b9b7-9e0cffc458bb",
					competencyText = "describe the sources of long-term capital and its importance to sustaining operations"
				}
			};

			//Serialize the request object
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "CompetencyFramework",
				RequestType = doingPublish ? "publish" : "format",
				OrganizationApiKey = apiKey,
				CTID = myRequest.CompetencyFramework.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		
		}

		public string PublishACEIntroductorySociology( bool doingPublish = false )
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-d3913e58-b578-42be-8c8e-a55c0b66f5ab";// "ce-" + Guid.NewGuid().ToString().ToLower();
			var targetUrl = "https://sandbox.credentialengineregistry.org/resources/";
			//A simple CompetencyFramework
			var myData = new CompetencyFramework()
			{
				name = "ACE - Introductory Sociology Skills Measured",
				description = "Skills measured for the Introductory Sociology Skills Measured exam.",
				CTID = myCTID,
				publisherName = new List<string>() { "American Council on Education" },
				publisher = new List<string>() { targetUrl + organizationIdentifierFromAccountsSite }

			};

			//This holds the data to be published and the identifier (CTID) for the owning organization
			var myRequest = new CompetencyFrameworkRequest()
			{
				CompetencyFramework = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//competencies

			myRequest.Competencies = new List<Competency>()
			{
				new Competency()
				{
					CTID = "ce-f53c9247-9775-4186-a1f1-edd0a965edeb",
					competencyText="Identification of specific names, facts, and concepts from sociological literature"
				},
				new Competency()
				{
					CTID = "ce-7351578b-f8d3-4ca6-8ff5-1818dd4092db",
					competencyText="Understanding of relationships between concepts, empirical generalizations, and theoretical propositions of sociology"
				},
				new Competency()
				{
					CTID = "ce-2b3b27a9-e42c-4021-bb7c-178766320134",
					competencyText="Understanding of the methods by which sociological relationships are established; "
				},
				new Competency()
				{
					CTID = "ce-6c4d0738-cb85-4263-badd-78ed1437b465",
					competencyText="application of concepts, propositions, and methods to hypothetical situations"
				},
				new Competency()
				{
					CTID = "ce-ec073a6d-ce14-4e21-a548-bcb9a5d5bb24",
					competencyText="Interpretation of tables and charts"
				},

				new Competency()
				{
					CTID = "ce-1cfa68c0-5ad5-483b-ad8a-e622f33d4a1a",
					competencyText="institutions"
				},
				new Competency()
				{
					CTID = "ce-30c840c0-2978-49b9-9044-d74721c827c1",
					competencyText="social patterns"
				},
				new Competency()
				{
					CTID = "ce-5d0cbcee-ef29-46fc-a8e4-b260cb737d3e",
					competencyText="social process"
				},
				new Competency()
				{
					CTID = "ce-f7b31a05-4b6d-47f6-b001-1c7ad32ad6d1",
					competencyText="social stratification (process and structure)"
				},
				new Competency()
				{
					CTID = "ce-90311565-e23b-40ec-8927-e1d9d04b599c",
					competencyText="the socialogical perspective"
				}
			};

			//Serialize the request object
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "CompetencyFramework",
				RequestType = doingPublish ? "publish" : "format",
				OrganizationApiKey = apiKey,
				CTID = myRequest.CompetencyFramework.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;

		}
		#endregion

	}
}

