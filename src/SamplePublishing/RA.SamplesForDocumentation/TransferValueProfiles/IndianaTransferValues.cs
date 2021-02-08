using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class IndianaTransferValues
	{
		public string IntroductiontoBusinessTVP( bool doingPublish = false )
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
				//0d9d86e0-f7cc-43e8-be0c-21ec24025758
			}
			// This is the CTID of the organization that owns the data being published
			//TODO - publish Indiana - copy from production
			//var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			var orgCTID = "ce-b412e8bc-f1e3-44b3-8785-1195e34caf80";
			var organizationIdentifierFromAccountsSite = orgCTID;

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-7f7c95eb-e8c1-4c21-9911-91dc8919d931";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "Business, Introduction to",
				Description = "IWU, USF, IUSB: Ancilla course will transfer as undistributed credit.",
				CTID = myCTID,
				SubjectWebpage = "https://transferin.net/transfer-resources/transfer-databases/core-transfer-library",
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
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
				}
			};

			//==============	transfer value from ===================================================
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var ancillaCollegeLopp = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "Introduction to Business-FROM",
				SubjectWebpage = "https://example.org/?tbd=tbd=introToBusFrom",

			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Ancilla College",
				SubjectWebpage = "https://example.org/?tbd=url",
			};
			ancillaCollegeLopp.OfferedBy = new List<OrganizationReference>();
			ancillaCollegeLopp.OfferedBy.Add( ownedBy );
			ancillaCollegeLopp.Identifier = new List<IdentifierValue>()
			{
				new IdentifierValue()
				{
					IdentifierTypeName="Course Number",
					IdentifierValueCode="BADM 100"
				}
			};
			myData.TransferValueFrom.Add( ancillaCollegeLopp );
			//
			var genericLoppFor = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "Introduction to Business - CTL",
				SubjectWebpage = "https://example.org/?tbd=introToBusFor",

			};
			//
			var offeredBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Indiana Commission for Higher Education",
				SubjectWebpage = "https://example.org/?tbd=url",
			};
			genericLoppFor.OfferedBy.Add( offeredBy );
			myData.TransferValueFor.Add( genericLoppFor );
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
