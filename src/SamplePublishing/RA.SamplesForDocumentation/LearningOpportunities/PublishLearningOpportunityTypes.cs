using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishLearningOpportunityTypes
	{
		/// <summary>
		/// The Learning Opportunity, Course and Learning Program all use the same input request class. 
		/// Oct/2021 Currently the only difference is that SCED is only valid for a Course
		/// There is a separate endpoint for learningopportunity: 
		///		/assistant/learningopportunity/publish
		/// </summary>
		/// <returns></returns>
		public string PublishLearningOpportunity( string requestType = "format" )
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//typically would have been stored prior to retrieving for publishing
			//DataService.SaveLearningOpportunityCTID( myCTID );

			//Populate the learning opportunity object
			var myData = new LearningOpportunity()
			{
				Name = "My Learning Opportunity Name",
				Description = "This is some text that describes my learning opportunity.",
				CTID = myCTID,
				LifeCycleStatusType="Active",
				SubjectWebpage = "https://example.org/?t=learningopportunity1234",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				LearningMethodType = new List<string>() { "learnMethod:Lecture", "learnMethod:Laboratory" },
				DeliveryType = new List<string>() { "BlendedLearning" },
				Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Description = "My Requirements",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				}
			};
			//add one of ownedBy or offeredBy, or both
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.OfferedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="One University Plaza",
					City="Springfield",
					PostalCode="62703",
					AddressRegion="IL",
					Country="United States"
				}
			};
			myData.Identifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "Some Identifer For Resource",
				IdentifierValueCode = "Catalog: xyz1234 "        //Alphanumeric string identifier of the entity
			} );
			myData.VersionIdentifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "MyVersion",
				IdentifierValueCode = "2023-09-01"        //Alphanumeric string identifier of the entity
			} );
			//								Teaches competencies
			//	Where a learning opportunity teaches one or more competencies, they can be published in the Teaches property
			//	List<CredentialAlignmentObject> Teaches
			//  Ideally, the competencies would be part of a competency framework that could be published to the registry.
			//  If the competencies are 'free floating' they can be published just using the name and an optional description
			myData.Teaches = new List<CredentialAlignmentObject>()
            {
                new CredentialAlignmentObject()
                {
                    TargetNodeName="Upon successful completion of this course, the student will be able to recognize causes and effects of chemically induced illness"
                },
                new CredentialAlignmentObject()
                {
                    TargetNodeName="And understand the role proper nutrition plays in avoiding and/or mitigating the damage these chemicals cause"
                },
                new CredentialAlignmentObject()
                {
                    TargetNodeName="Know how to find alternative solutions to chemicals",
					TargetNodeDescription = "An important description providing more details about this competency"
                }
            };
			//if the competencies are from a published framework, additional properties can be included
			myData.Teaches.Add( new CredentialAlignmentObject()
			{
                Framework= "https://credentialengineregistry.org/resources/ce-6fdd56d3-0214-4a67-b0c4-bb4c16ce9a13",
				TargetNode= "https://credentialengineregistry.org/resources/ce-a3246950-f245-4da5-9fa1-ee697db66d7f",
				FrameworkName ="SNHU Competency Framework",
				TargetNodeName = "Balance competing priorities in making decisions for your team that support organizational goals",
				CodedNotation="balance101"
			} );

            //A competency framework can contain many competencies. If a learning opportunity teaches all competencies in a framework, the helper property: TeachesCompetencyFramework may be used for efficiency. Rather than listing 10, 50, 500 competencies, only the CTID for the competency framework needs to be provided. The API will validate the framework, then fetch all competencies in the framework and populate the Teaches property.
            //NOTE: The framework must have already been published to the credential registry. 
            myData.TeachesCompetencyFramework = new List<string>()
			{
                "ce-6fdd56d3-0214-4a67-b0c4-bb4c16ce9a13"
            };

            //
            //A learning opportunity is usually connected to a credential. It is useful to provide the relationships where possible
            //The connection can be made using a Required condition profile in the Credential or using a RequiredFor from the learning opportunity

            myData.IsRequiredFor = new List<ConnectionProfile>()
			{
				new ConnectionProfile()
				{
					Description="This learning opportunity is required for the 'Acme Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //optional, but helpful
							CTID="ce-f5d9bf2a-d930-4e77-a69b-85788943851c"
						}
					}
				},
				//if the credential is not in the registry (often where the owner is not the same as the owner of the learning opportunity), or the publisher doesn't have the CTID, a full EntityReference can be provided. 
				new ConnectionProfile()
				{
					Description="This learning opportunity is required for the 'Third Party Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //required here
							Name="Third Party Credential",
							SubjectWebpage="https://example.com?t=thisCredential",
							Description="Description of this credential"
						}
					}
				}
			};
			//duration for a program that is exactly 9 months
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Months=9
					}
				}
			};
			//add occupationType, IndustryType, InstructionalProgram
            List<string> alternateTypes = new List<string>();
            List<string> codes = new List<string>();
            //====================	OCCUPATIONS ====================
            myData.OccupationType = OccupationsHelper.PopulateOccupations( ref alternateTypes, ref codes );
            if (alternateTypes != null && alternateTypes.Count > 0)
                myData.AlternativeOccupationType = alternateTypes;
            if (codes != null && codes.Count > 0)
                myData.ONET_Codes = codes;
            //====================	INDUSTRIES	====================
            myData.IndustryType = Industries.PopulateIndustries( ref alternateTypes, ref codes );
            if (alternateTypes != null && alternateTypes.Count > 0)
                myData.AlternativeIndustryType = alternateTypes;
            if (codes != null && codes.Count > 0)
                myData.NaicsList = codes;
            //====================	INSTRUCTIONAL PROGRAMS	====================
            myData.InstructionalProgramType = InstructionalPrograms.PopulatePrograms( ref alternateTypes, ref codes );
            if (alternateTypes != null && alternateTypes.Count > 0)
                myData.AlternativeInstructionalProgramType = alternateTypes;
            if (codes != null && codes.Count > 0)
                myData.CIP_Codes = codes;

            //add costs
            //Must be a valid CTDL cost type.
            // Example: Tuition, Application, AggregateCost, RoomOrResidency
            //see: https://credreg.net/ctdl/terms#CostType
            //Description and CostDetails are required properties
            myData.EstimatedCost.Add( new CostProfile()
			{
				Description = "A required description of the cost profile",
				CostDetails = "https://example.com/t=loppCostProfile",
				Currency="USD",
				CostItems = new List<CostProfileItem>()
				 {
					 new CostProfileItem()
					 {
						 DirectCostType="Application",
						 Price=100,
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="Tuition",
						 Price=12999,
						 PaymentPattern="Full amount due at time of registration"
					 }
				 }
			} );

			//Add organization that is not in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "ceterms:QACredentialOrganization",
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );

            //NEW for use with registered apprenticeships
            //	Add organization that is not in the credential registry
            //	NOTE: an open item is defining any rules as to when this can be used. One example could be the requirement for learning types like learnMethod:WorkBased
            myData.RegisteredBy.Add( new OrganizationReference()
            {
                Type = "ceterms:QACredentialOrganization",
                Name = "United States Department of Labor, Employment and Training Administration, Office of Apprenticeship",
                SubjectWebpage = "https://www.dol.gov/agencies/eta/apprenticeship"
            } );

            //This holds the learning opportunity and the identifier (CTID) for the owning organization
            var myRequest = new LearningOpportunityRequest()
			{
				LearningOpportunity = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			result = new SampleServices().SimplePost( "learningopportunity", "publish", payload, apiKey );

			//Return the result
			return result;
		}

		/// <summary>
		/// The Course request uses the same request class as Learning Opportunity, and Learning Program.
		/// There is a separate endpoint for course: 
		///		/assistant/course/publish
		/// </summary>
		/// <returns></returns>
		public string PublishCourse( string requestType = "format" )
		{
			//Holds the result of the publish action
			var result = "";
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
			var myCTID = "ce-aaa5d617-f00d-4e94-89af-ad77e9f26389";// "ce-" + Guid.NewGuid().ToString().ToLower();
			//typically would have been stored prior to retrieving for publishing
			//DataService.SaveLearningOpportunityCTID( myCTID );

			//Populate the learning opportunity/Course object
			var myData = new LearningOpportunity()
			{
				Name = "My Course Name",
				Description = "This is some text that describes my Course.",
				CTID = myCTID,
				LifeCycleStatusType = "Active",
				SubjectWebpage = "https://example.org/?t=course1234",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				LearningMethodType = new List<string>() { "learnMethod:Lecture", "learnMethod:Laboratory" },
				DeliveryType = new List<string>() { "BlendedDelivery" },
				Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Description = "My Requirements",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				}
			};
			//School Courses for the Exchange of Data
			myData.SCED = "100-101";
			//add one of ownedBy or offeredBy, or both
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.OfferedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="One University Plaza",
					City="Springfield",
					PostalCode="62703",
					AddressRegion="IL",
					Country="United States"
				}
			};
			//
			//The connection can be made using a Required condition profile in the Credential or using a RequiredFor from the Course

			myData.IsRequiredFor = new List<ConnectionProfile>()
			{
				new ConnectionProfile()
				{
					Description="This Course is required for the 'Acme Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //optional, but helpful
							CTID="ce-f5d9bf2a-d930-4e77-a69b-85788943851c"
						}
					}
				},
				//if the credential is not in the registry (often where the owner is not the same as the owner of the Course), or the publisher doesn't have the CTID, a full EntityReference can be provided. 
				new ConnectionProfile()
				{
					Description="This Course is required for the 'Third Party Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //required here
							Name="Third Party Credential",
							SubjectWebpage="https://example.com?t=thisCredential",
							Description="Description of this credential"
						}
					}
				}
			};
			//duration for a program that is exactly 9 months
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Months=9
					}
				}
			};
			//add costs
			//Must be a valid CTDL cost type.
			// Example: Tuition, Application, AggregateCost, RoomOrResidency
			//see: https://credreg.net/ctdl/terms#CostType
			//Description and CostDetails are required properties
			myData.EstimatedCost.Add( new CostProfile()
			{
				Description = "A required description of the cost profile",
				CostDetails = "https://example.com/t=loppCostProfile",
				Currency = "USD",
				CostItems = new List<CostProfileItem>()
				 {
					 new CostProfileItem()
					 {
						 DirectCostType="Application",
						 Price=100,
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="Tuition",
						 Price=12999,
						 PaymentPattern="Full amount due at time of registration"
					 }
				 }
			} );

			//Add organization that is not in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "ceterms:QACredentialOrganization", //should not be a requirement?
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );

			//This holds the resource being published and the identifier (CTID) for the owning organization
			var myRequest = new LearningOpportunityRequest()
			{
				LearningOpportunity = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "course",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.LearningOpportunity.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;

		}


		/// <summary>
		/// The Learning Program request uses the same request class as Learning Opportunity, and Course.
		/// There is a separate endpoint for Learning Program: 
		///		/assistant/LearningProgram/publish
		/// </summary>
		/// <returns></returns>
		public string PublishLearningProgram( string requestType = "format" )
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//

			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//typically would have been stored prior to retrieving for publishing
			//DataService.SaveLearningOpportunityCTID( myCTID );

			//Populate the learning opportunity/Learning Program object
			var myData = new LearningOpportunity()
			{
				Name = "My Learning Program Name",
				Description = "This is some text that describes my Learning Program.",
				CTID = myCTID,
				LifeCycleStatusType = "Active",
				SubjectWebpage = "https://example.org/?t=LearningProgram1234",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				LearningMethodType = new List<string>() { "learnMethod:Lecture", "learnMethod:Laboratory" },
				DeliveryType = new List<string>() { "BlendedLearning" },
				Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Description = "My Requirements",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				}
			};
			//add one of ownedBy or offeredBy, or both
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.OfferedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="One University Plaza",
					City="Springfield",
					PostalCode="62703",
					AddressRegion="IL",
					Country="United States"
				}
			};
			//
			//A Learning Program *must* be connected to a credential in order to be published.
			//The connection can be made using a Required condition profile in the Credential or using a RequiredFor from the Learning Program

			myData.IsRequiredFor = new List<ConnectionProfile>()
			{
				new ConnectionProfile()
				{
					Description="This Learning Program is required for the 'Acme Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //optional, but helpful
							CTID="ce-f5d9bf2a-d930-4e77-a69b-85788943851c"
						}
					}
				},
				//if the credential is not in the registry (often where the owner is not the same as the owner of the Learning Program), or the publisher doesn't have the CTID, a full EntityReference can be provided. 
				new ConnectionProfile()
				{
					Description="This Learning Program is required for the 'Third Party Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //required here
							Name="Third Party Credential",
							SubjectWebpage="https://example.com?t=thisCredential",
							Description="Description of this credential"
						}
					}
				}
			};
			//duration for a program that is exactly 9 months
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Months=9
					}
				}
			};
			//add costs
			//Must be a valid CTDL cost type.
			// Example: Tuition, Application, AggregateCost, RoomOrResidency
			//see: https://credreg.net/ctdl/terms#CostType
			//Description and CostDetails are required properties
			myData.EstimatedCost.Add( new CostProfile()
			{
				Description = "A required description of the cost profile",
				CostDetails = "https://example.com/t=loppCostProfile",
				Currency = "USD",
				CostItems = new List<CostProfileItem>()
				 {
					 new CostProfileItem()
					 {
						 DirectCostType="Application",
						 Price=100,
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="Tuition",
						 Price=12999,
						 PaymentPattern="Full amount due at time of registration"
					 }
				 }
			} );

			//Add organization that is not in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "ceterms:QACredentialOrganization",
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );

			//This holds the Learning Program and the identifier (CTID) for the owning organization
			var myRequest = new LearningOpportunityRequest()
			{
				LearningOpportunity = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "learningProgram",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.LearningOpportunity.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;

		}

	};

}
