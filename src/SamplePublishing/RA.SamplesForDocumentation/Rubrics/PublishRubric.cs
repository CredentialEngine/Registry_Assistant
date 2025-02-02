using Newtonsoft.Json;

using RA.Models.Input;

using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.RubricRequest;
using APIRequestResource = RA.Models.Input.Rubric;

namespace RA.SamplesForDocumentation
{
	public class PublishRubric
	{
		/// <summary>
		/// Publish a Rubric
		/// <returns></returns>
		public string Publish( string requestType = "publish" )
		{
			//Holds the result of the publish action
			var result = string.Empty;
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}

			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
			//typically would have been stored prior to retrieving for publishing

			//create request object
			var myRequest = new APIRequest()
			{
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			
			//Populate the Rubric object
			var myData = new APIRequestResource()
			{
				Name = "My Rubric Name",
				Description = "This is some text that describes my Rubric.",
				CTID = myCTID,
				SubjectWebpage = "https://example.org/?t=APIRequestResource1234",
				CodedNotation = "ru:123-abc",
				InLanguage = new List<string>() { "en-US" },
				LifeCycleStatusType = "Active",
				DateCopyrighted = "1997",
				DateCreated = "1997-04-15",
				DateModified = "2018-05-17"
			};
			//add Publisher
			//ce-928d9c1b-5c7c-46f6-903d-13d111a73561
			//
			myData.Publisher.Add( "ce-a4041983-b1ae-4ad4-a43d-284a5b4b2d73" );
			myData.Creator = myData.Publisher;
			//Optional
			myData.AudienceType = new List<string>() { "FullTime", "PartTime" };
			myData.AudienceLevelType = new List<string>() { "BeginnerLevel" };
			myData.DeliveryType = new List<string>() { "BlendedDelivery" };
			myData.EducationLevelType = new List<string>() { "audLevel:BeginnerLevel" };
			//concept
			myData.EvaluatorType = new List<string>() { "evalCat:Authority" };

			//Classification- using a blank node to an object in ReferenceObjects
			//1. create a blank node Id
			var bnodeId = "_:" + Guid.NewGuid().ToString().ToLowerInvariant();
			//2. create the concept
			var concept = new Concept()
			{
				Id = bnodeId,
				Type = "skos:Concept",
				PrefLabel = "Equity Goal"
			};
			//add the bnodeId to Classification
			myData.Classification = new List<string>() { bnodeId };
			//add the blank node object to ReferenceObjects
			myRequest.ReferenceObjects.Add( concept );

			myData.HasRubricCriterion = new List<string>()
			{
				"ce-4b2191ec-c118-4ca3-afaa-c7b34c751e01",
				"ce-4b2191ec-c118-4ca3-afaa-c7b34c751e02",
				"ce-4b2191ec-c118-4ca3-afaa-c7b34c751e03",
				"ce-4b2191ec-c118-4ca3-afaa-c7b34c751e04"
			};

			myData.HasRubricLevel = new List<string>()
			{
				"_:03083eeb-a8a1-4a8a-a546-16b6e18b3701",
				"_:03083eeb-a8a1-4a8a-a546-16b6e18b3702",
				"_:03083eeb-a8a1-4a8a-a546-16b6e18b3703",
				"_:03083eeb-a8a1-4a8a-a546-16b6e18b3704"
			};

			myRequest.RubricCriterion = new List<RubricCriterion>()
			{
				new RubricCriterion()
				{
					CTID = "ce-4b2191ec-c118-4ca3-afaa-c7b34c751e01",
					Name = "State a position",
					Weight= 0.25M,
					HasCriterionLevel = new List<string>()
					{
						"_:d5046cff-ce48-4e51-a141-82043bb7368b",
						"_:d45df98e-656e-414b-af34-065f3940a90a",
						"_:fdbcfdba-6423-4cca-a1e1-4e67472da6d8",
						"_:529ca329-5f0d-4bdb-b17a-9eb9e9fc7dad"
					}
				},new RubricCriterion()
				{
					CTID = "ce-4b2191ec-c118-4ca3-afaa-c7b34c751e02",
					Name = "Support hypothesis",
					Weight= 0.25M,
					HasCriterionLevel = new List<string>()
					{
						"_:95c0fde9-a8bb-47af-867e-818d6f7dede5",
						"_:443f34eb-a440-49fb-a577-1d07b982f188",
						"_:4717b222-45fd-427d-8f88-cce4fccdaa7d",
						"_:f0f7ac2b-a98d-4cd8-b0bf-ad5ddcba376c"
					}
				},
				new RubricCriterion()
				{
					CTID = "ce-4b2191ec-c118-4ca3-afaa-c7b34c751e03",
					Name = "Organization",
					Weight= 0.25M,
					HasCriterionLevel = new List<string>()
					{
						"_:51e3008c-8a29-475d-95c1-7a1977221c81",
						"_:7f5b18e8-df2d-41a6-9ca7-11ef44828766",
						"_:074d2f67-4df8-4881-ad64-812fcc1e9582",
						"_:7b84a1eb-adf4-4f67-9243-e3b8c8a55135"
					}
				},
				new RubricCriterion()
				{
					CTID = "ce-4b2191ec-c118-4ca3-afaa-c7b34c751e04",
					Name = "Overall impression",
					Weight= 0.25M,
					HasCriterionLevel = new List<string>()
					{
						"_:fdb0575a-9e79-4431-8156-78a08ccdef80",
						"_:da067d54-2f56-4790-8d34-de6411601244",
						"_:081cdeae-f100-414d-a650-ae058d18c8b8",
						"_:1d0af181-309b-4ec2-891f-4f6b27a46c5e"
					}
				}
			};

			myRequest.RubricLevel = new List<RubricLevel>()
			{
				new RubricLevel()
				{
					Id = "_:03083eeb-a8a1-4a8a-a546-16b6e18b3701",
					Name = "Excellent",
					HasCriterionLevel = new List<string>()
					{
						"_:d5046cff-ce48-4e51-a141-82043bb7368b",
						"_:95c0fde9-a8bb-47af-867e-818d6f7dede5",
						"_:51e3008c-8a29-475d-95c1-7a1977221c81",
						"_:fdb0575a-9e79-4431-8156-78a08ccdef80"
					}
				}, 
				new RubricLevel()
				{
					Id = "_:03083eeb-a8a1-4a8a-a546-16b6e18b3702",
					Name = "Proficient",
					HasCriterionLevel = new List<string>()
					{
						"_:d45df98e-656e-414b-af34-065f3940a90a",
						"_:443f34eb-a440-49fb-a577-1d07b982f188",
						"_:7f5b18e8-df2d-41a6-9ca7-11ef44828766",
						"_:da067d54-2f56-4790-8d34-de6411601244"
					}
				},
				new RubricLevel()
				{
					Id = "_:03083eeb-a8a1-4a8a-a546-16b6e18b3703",
					Name = "Satisfactory",
					HasCriterionLevel = new List<string>()
					{
						"_:fdbcfdba-6423-4cca-a1e1-4e67472da6d8",
						"_:4717b222-45fd-427d-8f88-cce4fccdaa7d",
						"_:074d2f67-4df8-4881-ad64-812fcc1e9582",
						"_:081cdeae-f100-414d-a650-ae058d18c8b8"
					}
				},
				new RubricLevel()
				{
					Id = "_:03083eeb-a8a1-4a8a-a546-16b6e18b3704",
					Name = "Unsatisfactory",
					HasCriterionLevel = new List<string>()
					{
						"_:529ca329-5f0d-4bdb-b17a-9eb9e9fc7dad",
						"_:f0f7ac2b-a98d-4cd8-b0bf-ad5ddcba376c",
						"_:7b84a1eb-adf4-4f67-9243-e3b8c8a55135",
						"_:1d0af181-309b-4ec2-891f-4f6b27a46c5e"
					}
				}
			};

			//
			myRequest.CriterionLevel = new List<CriterionLevel>()
			{
				new CriterionLevel()
				{
					Id = "_:d5046cff-ce48-4e51-a141-82043bb7368b",
					BenchmarkText = "Makes a strong stand and defines the context in the introductory paragraph. Position is restated throughout and reinforced with examples and included in the conclusion.",
					Percentage = 1.0M,
				},
				new CriterionLevel()
				{
					Id = "_:d45df98e-656e-414b-af34-065f3940a90a",
					BenchmarkText = "Makes a strong stand and defines the context in the introductory paragraph.",
					Percentage = 0.75M,
				},
				new CriterionLevel()
				{
					Id = "_:fdbcfdba-6423-4cca-a1e1-4e67472da6d8",
					BenchmarkText = "Makes a strong stand, but could be more powerful.",
					Percentage = 0.5M,
				},

				new CriterionLevel()
				{
					Id = "_:529ca329-5f0d-4bdb-b17a-9eb9e9fc7dad",
					BenchmarkText = "Doesn't take a stand or provide context.",
					Percentage = .25M,
				},
				new CriterionLevel()
				{
					Id = "_:95c0fde9-a8bb-47af-867e-818d6f7dede5",
					BenchmarkText = "Provided more than three arguments in support of the hypothesis. Arguments supported with more than citations. Writer incorporated own opinions. Examples incorporated in storytelling fashion.",
					Percentage = 1.0M,
				},
				new CriterionLevel()
				{
					Id = "_:443f34eb-a440-49fb-a577-1d07b982f188",
					BenchmarkText = "Provides three main arguments in support of the hypothesis. Gives clear and accurate examples and development of the three main arguments.",
					Percentage = 0.75M,
				},
				new CriterionLevel()
				{
					Id = "_:51e3008c-8a29-475d-95c1-7a1977221c81",
					BenchmarkText = "Fewer than three main arguments and incomplete examples in support of arguments.",
					Percentage = 0.5M,
				},
				new CriterionLevel()
				{
					Id = "_:fdb0575a-9e79-4431-8156-78a08ccdef80",
					BenchmarkText = "Another criterion level.",
					Percentage = 0.5M,
				},

				new CriterionLevel()
				{
					Id = "_:7f5b18e8-df2d-41a6-9ca7-11ef44828766",
					BenchmarkText = "Fewer than three main arguments and incomplete examples in support of arguments.",
					Percentage = 0.5M,
				},
				new CriterionLevel()
				{
					Id = "_:da067d54-2f56-4790-8d34-de6411601244",
					BenchmarkText = "Minimal idea development, limited and/or unrelated details. Doesn't give arguments in support of the hypothesis.",
					Percentage = 0.25M,
				},
				new CriterionLevel()
				{
					Id = "_:fdbcfdba-6423-4cca-a1e1-4e67472da6d8",
					BenchmarkText = "Depth and complexity of ideas supported by rich, engaging and/or pertinent details. Evidence analysis, reflection and insight.",
					Percentage = 1.0M,
				},


				new CriterionLevel()
				{
					Id = "_:4717b222-45fd-427d-8f88-cce4fccdaa7d",
					BenchmarkText = "Logical organization. Includes a compelling introduction, strong informative body, and satisfying conclusion. Has appropriate paragraph format.",
					Percentage = 0.75M,
				},
				new CriterionLevel()
				{
					Id = "_:074d2f67-4df8-4881-ad64-812fcc1e9582",
					BenchmarkText = "Writing has a clear beginning, middle, and end. General use of appropriate paragraph format.",
					Percentage = .5M,
				},
				new CriterionLevel()
				{
					Id = "_:081cdeae-f100-414d-a650-ae058d18c8b8",
					BenchmarkText = "Random or weak organization. No introduction and/or conclusion. Paragraphs lack development and coherence.",
					Percentage = 0.25M,
				},

				new CriterionLevel()
				{
					Id = "_:529ca329-5f0d-4bdb-b17a-9eb9e9fc7dad",
					BenchmarkText = "Superior in all ways. Precise, rich language. Establishes and maintains clear focus; evidence of distinctive voice and/or information.",
					Percentage = 1.0M,
				},


				new CriterionLevel()
				{
					Id = "_:f0f7ac2b-a98d-4cd8-b0bf-ad5ddcba376c",
					BenchmarkText = "Accomplished writing that entices reader to continue. Good flow and description.",
					Percentage = 0.75M,
				},
				new CriterionLevel()
				{
					Id = "_:7b84a1eb-adf4-4f67-9243-e3b8c8a55135",
					BenchmarkText = "Spoke to audience and drew them in. Purpose established. Acceptable, effective language.",
					Percentage = .5M,
				},
				new CriterionLevel()
				{
					Id = "_:1d0af181-309b-4ec2-891f-4f6b27a46c5e",
					BenchmarkText = "Limited awareness of audience and/or purpose. Incorrect and/or ineffective language.",
					Percentage = 0.25M,
				},
			};
			//This holds the Rubric and the identifier (CTID) for the owning organization
			myRequest.Rubric = myData;

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "Rubric",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Rubric.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;
		}


	};

}

