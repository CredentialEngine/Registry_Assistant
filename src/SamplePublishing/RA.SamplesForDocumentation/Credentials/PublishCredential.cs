using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;
using YourCredential = RA.SamplesForDocumentation.SampleModels.Credential;
using APIRequestCredential = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class PublishCredential
	{

		public string PublishDetailedRecord( string requestType = "publish" )
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
			var myCTID = "ce-5a33409a-f3db-42f3-8a3c-b00c7bb393af"; //"ce-" + Guid.NewGuid().ToString();

			//A simple credential object - see below for sample class definition
			var myData = new Credential()
			{
				Name = "My Certification Name",
				Description = "This is some text that describes my credential.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com/credential/1234",
				CredentialType = "ceterms:Certification",
				CredentialStatusType = "Active",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				Naics = new List<string>() { "333922", "333923", "333924" }
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//Specify available delivery types
			myData.LearningDeliveryType = new List<string>() 
			{ 
				"deliveryType:InPerson", 
				"deliveryType:OnlineOnly" 
			};

			//==================== QUALITY ASSURANCE RECEIVED ====================

			//CTID for Higher learning commission.
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//Add organization that is NOT in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "QACredentialOrganization",
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="1101 S. Yakima",
					City="Tacoma",
					PostalCode="98405",
					AddressRegion="WA",
					Country="United States"
				}
			};
			//==================== JURISDICTION and Recognized In (specialized jurisdiction) ====================

			myData.Jurisdiction.Add( Jurisdictions.SampleJurisdiction() );
			//Add a jurisdiction assertion for Recognized in 
			myData.RecognizedIn.Add( Jurisdictions.SampleJurisdictionAssertion() );

			//==================== CONDITION PROFILE ====================
			// add a requires Condition profile with conditions and a required learning opportunity. 
			// See the code sample for a ConditionProfile for more detailed information
			//	https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/SamplePublishing/RA.SamplesForDocumentation/SupportingData/ConditionProfiles.cs
			/*Scenario: 
				- The learning opportunity will be published to the credential registry
				- The credential must be published before the learning opportunity
				- The learning opportunity is referenced using the Condition Profile property of TargetLearningOpportunity
				- Only the CTID need be provided for a learning opportunity that will be published

			*/
			myData.Requires = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Description = "To earn this credential the following conditions must be met, and the target learning opportunity must be completed.",
					Condition = new List<string>() { "Complete High School", "Have a drivers licence." },
					TargetLearningOpportunity = new List<EntityReference>()
					{
						//if the target learning opportunity exists in the registry, then only the CTID has to be provided in the EntityReference
						new EntityReference()
						{
							CTID="ce-ccd00a32-d5ad-41e7-b14c-5c096bc9eea0"
						},
						new EntityReference()
						{
							//Learning opportunities not in the registry may still be published as 'blank nodes'
							//The type, name, and subject webpage are required. The description while useful is optional.
							Type="LearningOpportunity",
							Name="Another required learning opportunity (external)",
							Description="A required learning opportunity that has not been published to Credential Registry. The type, name, and subject webpage are required. The description while useful is optional. ",
							SubjectWebpage="https://example.org?t=anotherLopp",
							 CodedNotation="Learning 101"
						}
					}
				},
				//a condition profile that indicate the required credit hours, using the CreditValue property and a credit type of SemesterHours
				new ConditionProfile()
				{
					Description = "To earn this credential the following conditions must be met.",
					//credit Value
					CreditValue = new List<ValueProfile>()
					{
						new ValueProfile()
						{
							//CreditUnitType- The type of credit associated with the credit awarded or required.
							// - ConceptScheme: ceterms:CreditUnit (https://credreg.net/ctdl/terms/CreditUnit#CreditUnit)
							// - Concepts: provide with the namespace (creditUnit:SemesterHour) or just the text (SemesterHour). examples
							// - creditUnit:ClockHour, creditUnit:ContactHour, creditUnit:DegreeCredit
							CreditUnitType = new List<string>() {"SemesterHour"},
							Value=10
						}
					}
				}
			};
			//common conditions
			//An organization may publish common condition information such as pre-requisties using a ConditionManifest.
			//Each credential can then reference these common conditions using the CommonCondition property rather than having to repeat the information.
			//This propery is a list of CTIDs (recommended) for each published ConditionManifest or the actual credential registry URIs 
			myData.CommonConditions = new List<string>()
			{
				"ce-82a854b6-1e17-4cd4-845d-0b9b6df2fb5c"
			};

			//duration for a range from 8 to 12 weeks
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					MinimumDuration = new DurationItem()
                    {
						Weeks=8
                    }, 
					MaximumDuration = new DurationItem()
                    {
						Weeks=12
                    }
				}
			};
			//====================	COSTS	====================
			//Must be a valid CTDL cost type.
			// Example: Tuition, Application, AggregateCost, RoomOrResidency
			//see: https://credreg.net/ctdl/terms#CostType
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

			//common costs
			//An organization may publish common cost information such as Tuition costs using a CostManifest. Each credential can then reference these common costs using the CommonCost property rather than having to repeat the information. This propery is a list of CTIDs (recommended) for each published costManifest or the actual credential registry URIs 
			myData.CommonCosts = new List<string>()
			{
				"ce-a37b5ac4-6a15-4cf1-9f06-8132e18e95eb", "ce-975b466b-ed8e-46c7-8629-2f2dc74153a2"
			};
			//====================	OCCUPATIONS ====================
			PopulateOccupations( myData );
			//====================	INDUSTRIES	====================
			PopulateIndustries( myData );
			//====================	PROGRAMS	====================
			PopulatePrograms( myData );

			//====================	CONNECTIONS ====================
			//Connections between credentials can be published using properties such as
			//- isPreparationFor, PreparationFrom, isAdvancedStandingFor, AdvancedStandingFrom, IsRequiredFor, and IsRecommendedFor. 
			//example of a connection to a credential for which the current credential will prepare a student.
			var isPreparationFor = new Connections
			{
				Description = "This certification will prepare a student for the target credential",
				TargetCredential = new List<EntityReference>()
				{
					//the referenced credential could be for an external credential, not known to be in the credential registry
					new EntityReference()
					{
						Type="MasterDegree",
						Name="Cybersecurity Technology Master's Degree  ",
						Description="A helpful description",
						SubjectWebpage="https://example.org?t=masters"
					}
				}
			};
			myData.IsPreparationFor.Add( isPreparationFor );

			//add credential that prepares for this credential. 
			var preparationFrom = new Connections
			{
				Description = "This credential will prepare a student for this credential",
				TargetCredential = new List<EntityReference>()
				{
					//the referenced credential is known to be in the credential registry, so only the CTID need be provided
					new EntityReference()
					{
						CTID="ce-40c3e860-5034-4375-80e8-f7455ff86a48"
					}
				}
			};
			myData.PreparationFrom.Add( preparationFrom );

			//====================	CREDENTIAL REQUEST ====================
			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
		
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "credential",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Credential.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;
		}

		/// <summary>
		/// Publish a credential using an input class
		/// An organization will have its data stored somewhere. The first step would be to have a process retrieve the information and send that data to a method to do the publishing. 		
		/// In this example: 
		/// -	YourCredential would be defined by the organization
		///-	A process would be developed to read data from the organization data source( s) and populate the class
		///		o   Important: a CTID must be created in the data source and populated
		///-	The Publish method would be called with the credential data.
		///Steps:
		///		o Establishes the apiKey, and the CTID for the owning organization
		///		o   Instantiates the credential class the is part of the request class used by the API
		///		o   Maps input data to output
		///		o   Adds the OwnedBy property
		///		o A sample for adding AccreditedBY using just the CTID for The Higher Learning Commission
		///		o	( Add all applicable properties as needed)
		///		o Create the request class used by the API, and assign the Credential, and other properties
		///		o Serialize the request class into JSON
		///			The sample using the “Newtonsoft.Json” library
		///			JsonConvert.SerializeObject( myRequest);
		///		o Formats the HttpClient with the header, content
		///		o   Gets the desired publishing endpoint
		///		o   Calls the endpoint, and hopefully gets a successful result
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string PublishFromInputClass( YourCredential input )
		{
			//Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();

			var myData = new APIRequestCredential
			{
				Name = input.Name,
				Description = input.Description,
				InLanguage = new List<string>() { "en-US" },
				//provide valid concept from schema 
				CredentialType = "BachelorDegree",
				//*** the source data must assign a CTID and use for all transactions
				CTID = input.CTID,
				DateEffective = input.DateEffective,
				Image = input.ImageUrl,
				Subject = input.Subject,
				Keyword = input.Keyword
			};

			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//==================== Quality Assurance Received ====================

			//CTID for Higher learning commission.
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//Add organization that is not in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );
			//add costs
			//Must be a valid CTDL cost type.
			// Example: Tuition, Application, AggregateCost, RoomOrResidency
			//see: https://credreg.net/ctdl/terms#CostType
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

			//add occupations
			PopulateOccupations( myData );
			//industries
			PopulateIndustries( myData );
			//Programs
			PopulatePrograms( myData );
			//Financial Assistance
			FinancialAssistanceProfiles.PopulateSimpleFinancialAssistanceProfile( myData );

			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "credential", "publish", payload, apiKey );

			return result;
		}
		/// <summary>
		/// Possible Input Types
		/// - List of frameworks
		/// - list of occupation names
		/// - List of SOC codes
		/// 
		/// </summary>
		/// <param name="request"></param>
		public static void PopulateOccupations( Credential request )
		{
			request.OccupationType = new List<FrameworkItem>
			{

				//occupations from a framework like ONet - where the information is stored locally and can be included in publishing
				new FrameworkItem()
				{
					Framework = "https://www.onetonline.org/",
					FrameworkName = "Standard Occupational Classification",
					Name = "Information Security Analysts",
					TargetNode = "https://www.onetonline.org/link/summary/15-1122.00",
					CodedNotation = "15-1122.00",
					Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
				},
				new FrameworkItem()
				{
					Framework = "https://www.onetonline.org/",
					FrameworkName = "Standard Occupational Classification",
					Name = "Computer Network Support Specialists",
					TargetNode = "https://www.onetonline.org/link/summary/15-1152.00",
					CodedNotation = "15-1152.00",
					Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
				}
			};


			//Occupations not in a known framework, list of strings
			request.AlternativeOccupationType = new List<string>() { "Cybersecurity", "Forensic Scientist", "Forensic Anthropologist" };

			//O*Net helper - ALternately provided a list of O*Net codes. The Assistant API will validate the codes and format the output including the framework name and URL, the occupation, description, and code
			request.ONET_Codes = new List<string>() { "13-2099.01", "13-2052.00", "13-2061.00", "13-2051.00" };
		}

		/// <summary>
		/// Possible Input Types
		/// - List of frameworks
		/// - list of industry names
		/// - List of NAICS codes
		/// </summary>
		/// <param name="request"></param>
		public static void PopulateIndustries( Credential request )
		{
			request.IndustryType = new List<FrameworkItem>
			{

				//occupations from a framework like NAICS - where the information is stored locally and can be included in publishing
				new FrameworkItem()
				{
					Framework = "https://www.naics.com/",
					FrameworkName = "NAICS - North American Industry Classification System",
					Name = "National Security",
					TargetNode = "https://www.naics.com/naics-code-description/?code=928110",
					CodedNotation = "928110",
					Description = "This industry comprises government establishments of the Armed Forces, including the National Guard, primarily engaged in national security and related activities."
				},
				new FrameworkItem()
				{
					Framework = "https://www.naics.com/",
					FrameworkName = "NAICS - North American Industry Classification System",
					Name = "Regulation and Administration of Transportation Programs",
					TargetNode = "https://www.naics.com/naics-code-description/?code=926120",
					CodedNotation = "926120",
					Description = "This industry comprises government establishments primarily engaged in the administration, regulation, licensing, planning, inspection, and investigation of transportation services and facilities. Included in this industry are government establishments responsible for motor vehicle and operator licensing, the Coast Guard (except the Coast Guard Academy), and parking authorities."
				}
			};


			//Industries not in a known framework, list of strings
			request.AlternativeIndustryType = new List<string>() { "Cybersecurity", "Forensic Science", "Forensic Anthropology" };

			//NAICS helper - ALternately provided a list of NAICS codes. The Assistant API will validate the codes and format the output including the framework name and URL, the name, description, and code
			request.Naics = new List<string>() { "9271", "927110", "9281", "928110" };
		}

		/// <summary>
		/// Possible Input Types
		/// - List of frameworks
		/// - list of program names
		/// - List of CIP codes
		/// </summary>
		/// <param name="request"></param>
		public static void PopulatePrograms( Credential request )
		{
			request.InstructionalProgramType = new List<FrameworkItem>
			{

				//programs from a framework like Classification of Instructional Program - where the information is stored locally and can be included in publishing
				new FrameworkItem()
				{
					Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					FrameworkName = "Classification of Instructional Program",
					Name = "Medieval and Renaissance Studies",
					TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.1301",
					CodedNotation = "30.1301",
					Description = "A program that focuses on the  study of the Medieval and/or Renaissance periods in European and circum-Mediterranean history from the perspective of various disciplines in the humanities and social sciences, including history and archeology, as well as studies of period art and music."
				},
				new FrameworkItem()
				{
					Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					FrameworkName = "Classification of Instructional Program",
					Name = "Classical, Ancient Mediterranean and Near Eastern Studies and Archaeology",
					TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.2202",
					CodedNotation = "30.2202",
					Description = "A program that focuses on the cultures, environment, and history of the ancient Near East, Europe, and the Mediterranean basin from the perspective of the humanities and social sciences, including archaeology."
				}
			};


			//programs not in a known framework, list of strings
			request.AlternativeInstructionalProgramType = new List<string>() { "Cybersecurity 101", "Forensic Science 120", "Forensic Anthropology 400" };

			//CIP code helper - ALternately provided a list of CIP codes. The Assistant API will validate the codes and format the output including the framework name and URL, the name, description, and code
			request.CIP_Codes = new List<string>() { "31.0504", "31.0505", "31.0599", "31.9999" };
		}


	}
}
