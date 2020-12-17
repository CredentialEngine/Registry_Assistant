using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models.Input;
namespace RA.SamplesForDocumentation
{
	public class NCCRSTransferValues
	{
		public string AdvancedLegalIssuesFraudInvestigationTVP( bool doingPublish = false )
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
			//TODO - publish NCCRS??
			//var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			var orgCTID = "ce-91bef066-cbec-4021-8c8d-45cde1f39c2b";
			var organizationIdentifierFromAccountsSite = orgCTID;

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from github
			myCTID = "ce-4602d328-4a38-42d0-b6b8-3a8de4b8f4ed";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "Advanced Legal Issues in Fraud Investigation and Defense",
				Description = "In the upper division baccalaureate degree category OR in the graduate degree category, 1 semester hour in Insurance, Insurance Law or Business Law (5/10) (5/15 revalidation) (6/20 revalidation). NOTE: Courses 513, 514, and 515 or 516 constitute 3 semester hours in Insurance Fraud. NOTE: Courses 510, 512, 515 or 516 constitute 3 semester hours in Insurance Fraud.",
				CTID = myCTID,
				SubjectWebpage = "http://www.nationalccrs.org/american-educational-institute/advanced-legal-issues-fraud",
				StartDate = "2004-05"		//ensure can handle a partial date
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//============== TransferValue ================================
			//Required. Provide a transfer value amount using Value and UnitText. ex. DegreeCredit This is a concept scheme and so has a strict vocabulary. 
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=1,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"},
					Subject = new List<string>(){ "Insurance, Insurance Law, or Business Law" }
				}
			};

			//==============	transfer value from ===================================================
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var transferValueFrom = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "Advanced Legal Issues in Fraud Investigation and Defense (515)",
				Description = "In addition to AEI produced textbook, students receive actual court decisions, which relate to the subject matter and present real-life court opinions that illustrate how courts have ruled in the past and are likely to rule in the future on important issues in claims. A proctored examination is administered for both levels of the credit recommendation. Scenario and case study-based questions, built around actual claims situations, challenge students to analyze and solve problems using applicable principles of claims law that parallel their own claim files. To broaden students' knowledge of the subject, graded exams are returned with helpful comments that provide a written explanation of why each answer is correct or incorrect. For the graduate level credit recommendation, students also prepare and submit an appropriate graduate level research project on a pre-approved topic or issue, in accordance with AEI's specific guidelines.",
				DeliveryType = new List<string>() { "deliveryType:OnlineOnly" }, 
				LearningMethodType = new List<string>() { "SelfPaced", "WorkBased"}, 
				//LearningMethodDescription="Learning method description", 
				//AssessmentMethodType = new List<string>() { "Exam"}, 
				//AssessmentMethodDescription= "AssessmentMethodDescription"
			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "American Educational Institute, Inc. (AEI)",
				SubjectWebpage = "http://www.nationalccrs.org/organizations/american-educational-institute"
			};
			transferValueFrom.OfferedBy = new List<OrganizationReference>();
			transferValueFrom.OfferedBy.Add( ownedBy );
			transferValueFrom.Identifier = new List<IdentifierValue>()
			{
				new IdentifierValue()
				{ IdentifierTypeName="Course Code", IdentifierValueCode="515"}
			};

			//transferValueFrom.Assesses = AssignCompetencies();
			transferValueFrom.AvailableAt = new List<Place>() { new Place() { Description = "Independent Study program administered through the offices of American Educational Institute." } };
			transferValueFrom.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile() { Description="Approximately 19 hours of structured independent study."}
			};
			transferValueFrom.Subject = new List<string>() {"Law of Claims Fraud Investigation and Defense", "defamation",
				"using information",
				"hearsay",
				"surveillance"
			};
			transferValueFrom.Teaches = new List<CredentialAlignmentObject>()
			{
				new CredentialAlignmentObject()
				{
					TargetNodeDescription = "Upon successful completion of the course, students will be able to identify and discuss advanced legal issues and investigation techniques in insurance fraud."
				}
			};
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
				CTID = myRequest.TransferValueProfile.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}

		public string PublishNCCRS( bool doingPublish = false )
		{

			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			//this is the CTID of the organization that owns the data being published
//			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			//Assign a CTID for the entity being published and keep track of it
			//this must be permantently stored in partner system and used with all future updates. 
			var myOrgCTID = "ce-91bef066-cbec-4021-8c8d-45cde1f39c2b";
			var organizationIdentifierFromAccountsSite = myOrgCTID;
			//A simple organization object - see below for sample class definition
			var myData = new Organization()
			{
				Name = "National College Credit Recommendation Service - NCCRS",
				Description = "In 1973, the Board of Regents of the University of the State of New York initiated a pilot study to assess the potential of a college credit advisory service. This project was a direct outcome of recommendations made by the Commission on Non-Traditional Study, a two-year venture supported by the Carnegie Corporation, to provide a national perspective on the future of higher education, including recognizing and granting credit for postsecondary learning undertaken in noncollegiate settings. The Commission called for a system to establish college credit equivalencies for courses offered by government, industry, and other noncollegiate sponsors.",
				Ctid = myOrgCTID,
				SubjectWebpage = "http://www.nationalccrs.org/",
				Type = "ceterms:QACredentialOrganization",
				Email = new List<string>() { " nccrs@nysed.gov" }
			};
			//required-concept from AgentSector: https://credreg.net/ctdl/terms/agentSectorType#AgentSector
			myData.AgentSectorType = "PrivateNonProfit";
			//required-One or more concepts from OrganizationType: https://credreg.net/ctdl/terms/agentType#OrganizationType
			myData.AgentType.Add( "Business" );
			//add addresses and contact points
			var mainAddress = new Place()
			{
				Address1 = "89 Washington Avenue",
				Address2 = "Education Building", //Address1 and Address2 are concatenated in the registry
				City = "Albany",
				AddressRegion = "New York",
				PostalCode = "12234",
				Country = "United States"
			};
			mainAddress.ContactPoint = new List<ContactPoint>()
			{
				new ContactPoint()
				{   ContactType="Information",
					Name="General Inquiries",
					PhoneNumbers = new List<string>() { "518-486-2070" }
				}
			};
			myData.Address.Add( mainAddress );


			//This holds the organization and the identifier (CTID) for the owning organization
			var myRequest = new OrganizationRequest()
			{
				Organization = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the organization request object
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			List<string> messages = new List<string>();
			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "organization",
				RequestType = doingPublish ? "publish" : "format",
				OrganizationApiKey = apiKey,
				CTID = myOrgCTID.ToLower(),  
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}
	}
}
