using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class ACETransferValues
	{

		#region IntroductorySociology
		/// <summary>
		/// Sample publish method
		/// </summary>
		/// <returns>If successful, returns the formatted graph from the registry.</returns>
		public string ACEIntroductorySociologyTVP( bool doingPublish = false )
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			//var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			var aceCTID = "ce-9c1c2d37-e525-43a3-9cea-97f076b2fe38";
			var organizationIdentifierFromAccountsSite = aceCTID;

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from github
			myCTID = "ce-fde8e91f-608b-4488-ba8d-b3d81b036b50";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "Introductory Sociology",
				Description = "Credit is recommended for candidates scoring 50 and above.",
				Ctid = myCTID,
				SubjectWebpage = "https://www.acenet.edu/National-Guide/Pages/Course.aspx?org=College%20Board%27s%20College-Level%20Examination%20Program%20(CLEP)&cid=fe05b1f6-84c4-ea11-a812-000d3a33232a",
				StartDate = "2018-12-01",
				EndDate = "2023-11-30"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.Identifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "ACE Course Code",
				IdentifierValueCode = "CLEP-0026"        //Alphanumeric string identifier of the entity
			} );
			//============== TransferValue ================================
			//Required. Provide a transfer value amount using Value and UnitText. ex. DegreeCredit This is a concept scheme and so has a strict vocabulary. 
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"},
					Description="Credit is recommended for candidates scoring 50 and above.",
					Subject = new List<string>(){ "Introductory Sociology" }
				}
			};

			//==============	transfer value from ===================================================
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var transferValueFrom = new Assessment()
			{
				Type = "AssessmentProfile",
				Name = "Introductory Sociology",
				Description = "The Introductory Sociology examination is designed to assess an individual's knowledge of the material typically presented in a one-semester introductory-level sociology course at most colleges and universities. The examination emphasizes basic facts and concepts as well as general theoretical approaches used by sociologists on the topics of institutions, social patterns, social processes, social stratifications, and the sociological perspective. Highly specialized knowledge of the subject and the methodology of the discipline is not required or measured by the test content.The exam contains approximately 100 questions to be answered in 90 minutes. Some of these are pretest questions that will not be scored. Any time test takers spend on tutorials and providing personal information is in addition to the actual testing time.",
				SubjectWebpage = "https://clep.collegeboard.org/?t=introductorySocialogy",
				DeliveryType = new List<string>() { "deliveryType:InPerson" }
			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "College Board's College-Level Examination Program (CLEP)",
				SubjectWebpage = "https://clep.collegeboard.org/"
			};
			transferValueFrom.OwnedBy = new List<OrganizationReference>();
			transferValueFrom.OwnedBy.Add( ownedBy );
			transferValueFrom.Assesses = AssignIntroductorySociologyCompetencies();

			myData.TransferValueFrom.Add( transferValueFrom );

			//===================================================================================


			// This holds the transfer value profile and the identifier (CTID) for the owning organization
			var myRequest = new TransferValueProfileRequest()
			{
				TransferValueProfile = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			// Serialize the request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			List<string> messages = new List<string>();
			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = doingPublish ? "publish" :"format",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}
		public List<CredentialAlignmentObject> AssignIntroductorySociologyCompetencies()
		{
			var frameworkName = "ACE - Introductory Sociology Skills Measured";
			var frameworkCTID = "ce-85ea2bf0-b0a3-422c-9074-14f2efe51480";
			var targetUrl = "https://sandbox.credentialengineregistry.org/resources/";

			var output = new List<CredentialAlignmentObject>()
			{
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-f53c9247-9775-4186-a1f1-edd0a965edeb",
					TargetNodeName="Identification of specific names, facts, and concepts from sociological literature"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-7351578b-f8d3-4ca6-8ff5-1818dd4092db",
					TargetNodeName="Understanding of relationships between concepts, empirical generalizations, and theoretical propositions of sociology"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-2b3b27a9-e42c-4021-bb7c-178766320134",                   
					TargetNodeName="Understanding of the methods by which sociological relationships are established; "
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-6c4d0738-cb85-4263-badd-78ed1437b465",
					TargetNodeName="application of concepts, propositions, and methods to hypothetical situations"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-ec073a6d-ce14-4e21-a548-bcb9a5d5bb24",                   
					TargetNodeName="Interpretation of tables and charts"
				},

				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName, Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-1cfa68c0-5ad5-483b-ad8a-e622f33d4a1a",                   
					TargetNodeName="institutions", Weight=.2M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-30c840c0-2978-49b9-9044-d74721c827c1",                   
					TargetNodeName="social patterns", Weight=.1M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-5d0cbcee-ef29-46fc-a8e4-b260cb737d3e",                   
					TargetNodeName="social process", Weight=.25M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-f7b31a05-4b6d-47f6-b001-1c7ad32ad6d1",                   
					TargetNodeName="social stratification (process and structure)", Weight=.25M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-90311565-e23b-40ec-8927-e1d9d04b599c",                   
					TargetNodeName="the socialogical perspective", Weight=.2M
				}
			};

			return output;
		}

		#endregion


		#region Principles of Finance
		/// <summary>
		/// Sample publish method
		/// </summary>
		/// <returns>If successful, returns the formatted graph from the registry.</returns>
		public string ACEPrinciplesOfFinanceTVP( bool doingPublish = false )
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			//var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			var aceCTID = "ce-9c1c2d37-e525-43a3-9cea-97f076b2fe38";
			var organizationIdentifierFromAccountsSite = aceCTID;

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from github
			myCTID = "ce-e4f5301e-0164-47f8-b020-23984defdb92";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "Principles of Finance",
				Description = "NOTE: Credit is recommended for candidates scoring 50 and above.",
				Ctid = myCTID,
				SubjectWebpage = "https://www.acenet.edu/National-Guide/Pages/Course.aspx?org=SOPHIA+Learning%2c+LLC&cid=6bb6586f-94c4-ea11-a812-000d3a378a3a",
				StartDate = "2020-06-01",
				EndDate = "2023-05-30"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.Identifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "ACE Course Number",
				IdentifierValueCode = "SOPH-033"        //Alphanumeric string identifier of the entity
			} );
			//============== TransferValue ================================
			//Required. Provide a transfer value amount using Value and UnitText. ex. DegreeCredit This is a concept scheme and so has a strict vocabulary. 
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"},
					Subject = new List<string>(){ "principles of finance" }
				}
			};

			//==============	transfer value from ===================================================
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var transferValueFrom = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "Principles of Finance",
				SubjectWebpage = "https://www.sophia.org/?t=principlesOfFinance",
				DeliveryType = new List<string>() { "deliveryType:OnlineOnly" }
			};

			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Sophia Learning, LLC",
				SubjectWebpage = "https://www.sophia.org/"
			};
			transferValueFrom.OwnedBy = new List<OrganizationReference>();
			transferValueFrom.OwnedBy.Add( ownedBy );
			//
			transferValueFrom.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile(){ Description = "This is a self-paced course. Students have 60 days with 30 days extension (if needed) to complete"}
			};
			//
			transferValueFrom.Identifier = new List<IdentifierValue>() {
				new IdentifierValue()
			{
					IdentifierValueCode="FIN1001",
					IdentifierTypeName = "Organization Course Number"
			} };
			transferValueFrom.AssessmentMethodType = new List<string>() { "assessMethod:Exam" };
			transferValueFrom.LearningMethodType = new List<string>() { "learnMethod:Lecture",
				"learnMethod:Laboratory",
				"learnMethod:Prerecorded"};
			transferValueFrom.Requires = new List<ConditionProfile>() {
				new ConditionProfile()
				{
					Description="The Minimum Passing Score for this learning opportunity is 70%.",
					Condition = new List<string>() { "Minimum Passing Score: 70%" }
				}
			};
			//
			transferValueFrom.Subject = new List<string>() { "Goals and organizations of financial management",
				"Statements, taxes, and cash flow",
				"Analyzing financial statements",
				"Forecasting financial statements",
				"Time value of cash flows",
				"Bond valuation",
				"Stock valuation",
				"Risk and return",
				"Market efficiency and returns",
				"Cost of capital",
				"Capital structure",
				"The role of risk in capital budgeting",
				"Obtaining capital",
				"Working capital management",
				"Dividends and dividend policy"
			};
			//
			transferValueFrom.Teaches = AssignPrinciplesFinanceCompetencies();

			myData.TransferValueFrom.Add( transferValueFrom );

			//===================================================================================


			// This holds the transfer value profile and the identifier (CTID) for the owning organization
			var myRequest = new TransferValueProfileRequest()
			{
				TransferValueProfile = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			// Serialize the request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			List<string> messages = new List<string>();
			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = doingPublish ? "publish" : "format",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}
		public List<CredentialAlignmentObject> AssignPrinciplesFinanceCompetencies()
		{
			var frameworkName = "ACE - Principles of Finance Learning Outcomes";
			var frameworkCTID = "ce-d3913e58-b578-42be-8c8e-a55c0b66f5ab";
			var targetUrl = "https://sandbox.credentialengineregistry.org/resources/";
			var output = new List<CredentialAlignmentObject>()
			{
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-f3984fc3-4b1f-433b-91a1-d5ad81ed7ca9",
					TargetNodeName="describe the operating environment of the firm, the role of financial institutions, and the structure of the capital markets and explain their impact on the financial performance of the firm"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-581bac7e-a735-4755-a4cc-313cd705b986",                 
					TargetNodeName="interpret financial statements and financial ratios"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-49ce65e5-132e-4430-8eb0-4ff06ec451eb",                 
					TargetNodeName="explain the time value of money and calculate future value and present value for single and multiple cash flows"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-82339b05-b49f-4fa4-b0d8-b37d054a420f",                 
					TargetNodeName="describe the characteristics of capital market instruments including stocks, bonds, and money market instruments"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-faa89f86-7a55-4ee5-866e-91609a54cd1d",                 
					TargetNodeName="explain the capital budgeting process and the basic elements of working capital management"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-07428927-1794-40ed-b9b7-9e0cffc458bb",                 
					TargetNodeName="describe the sources of long-term capital and its importance to sustaining operations"
				}
			};

			return output;
		}

		#endregion
	}
}
