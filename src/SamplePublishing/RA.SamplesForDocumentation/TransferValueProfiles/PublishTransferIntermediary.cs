using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.TransferIntermediaryRequest;
using APIRequestEntity = RA.Models.Input.TransferIntermediary;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
    public class PublishTransferIntermediary
    {
		public bool PublishOne()
		{
			string requestType = "publish";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}
			// Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-c6b70e53-f7db-4df8-b48e-56bbee89ed91";// "ce -" + Guid.NewGuid().ToString().ToLower();

			//===================================================================================
			var myData = new APIRequestEntity()
			{
				Name = "A Transfer Intermediary for ....",
				Description = "A useful description is coming soon. .",
				CodedNotation="Accounting 101",
				CTID = myCTID,
				Subject = new List<string>() { "Finance", "Accounting", "Bookkeeping"},
				SubjectWebpage = "https://example.org?t=ti22"
			};
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.CreditValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//This holds the main entity and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				TransferIntermediary = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//add some transfer value profiles
			myRequest.TransferValueProfiles.Add( GetTVPOne( organizationIdentifierFromAccountsSite ) );
			myRequest.TransferValueProfiles.Add( GetTVPTwo( organizationIdentifierFromAccountsSite ) );
			myRequest.TransferValueProfiles.Add( GetTVPEnvironmentalChallenges( organizationIdentifierFromAccountsSite ) );

			// Serialize the request object
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "TransferIntermediary",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferIntermediary.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			return new SampleServices().PublishRequest( req );

		}
		/// <summary>
		/// uses transferValueFrom and TransferValueFor
		/// </summary>
		/// <param name="owningOrganizationCtid"></param>
		/// <returns></returns>
		public TransferValueProfile GetTVPOne( string owningOrganizationCtid )
		{
			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from previous test
			myCTID = "ce-fd515001-6a9c-4f43-b401-3e65127fc807";
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Name",
				Description = "This is some text that describes my transfer value profile.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/tvp1"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = owningOrganizationCtid
			} );
			//============== TransferValue ====================================================
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//==============	transfer value from ===========================================
			//Resource that provides the transfer value described by this resource, according to the entity providing this resource.
			//A list of entity references. If the CTID is known, then just provide it.
			//A type is also required
			//22-02-11 mp - this has to be verified/looked up, so do we really need the type? Perhaps as an xref?
			myData.TransferValueFrom.Add( new EntityReference()
			{
				Type="ceterms:LearningOpportunityProfile",
				CTID = "ce-568e16ad-9697-4429-8c02-6428f49e87cc"
			} );
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			myData.TransferValueFrom.Add( new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed.",
				OwnedBy = new List<OrganizationReference>()
                {
					new OrganizationReference()
                    {
						Type="Organization", Name="ACME Publications", SubjectWebpage="https://example.org?t=acme"
                    }
                }

			} );

			//						optional
			//coded Notation could be replaced by Identifier in the near future
			myData.StartDate = "2015-01-01";
			myData.EndDate = "2021-12-21";

			return myData;
		}
		public TransferValueProfile GetTVPTwo( string owningOrganizationCtid )
		{
			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from previous test
			//
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Number Two ",
				Description = "This is some text that describes my transfer value profile number 2.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/tvp2"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = owningOrganizationCtid
			} );

			//============== TransferValue ====================================================
			//REQUIRED
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//==============	transfer value from ===========================================
			//Resource that provides the transfer value described by this resource, according to the entity providing this resource.
			//TransferValueFrom is list of objects. Currently the classes handled are LearningOpportunity and Assessment. 
			myData.TransferValueFrom.Add( new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."

			} );

			//==============	transfer value For
			//Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
			//TransferValueFor is list of objects. Currently the classes handled are LearningOpportunity and Assessment. 
			myData.TransferValueFor.Add( new Assessment()
			{
				Type = "AssessmentProfile",
				Name = "name of the target assessment",
				Description = "Description of the assessment",
				SubjectWebpage = "https://example.com/targetAssessment",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the assessment is conducted."
			} );


			//						optional
			myData.StartDate = "2020-01-01";
			myData.EndDate = "2021-12-21";
			myData.Identifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "ACE Course Code",
				IdentifierValueCode = "0276"        //Alphanumeric string identifier of the entity
			} );

			return myData;
		}

		/// <summary>
		/// Environmental Challenges And Solutions
		/// </summary>
		/// <param name="owningOrganizationCtid"></param>
		/// <returns></returns>
		public TransferValueProfile GetTVPEnvironmentalChallenges( string owningOrganizationCtid )
		{

			//from previous test
			//
			var myData = new TransferValueProfile()
			{
				Name = "Environmental Challenges And Solutions",
				Description = "To provide knowledge of the scope and severity of environmental illnesses.",
				CTID = "ce-489406de-1c64-40bd-af31-f7a502b8b850",
				SubjectWebpage = "https://stagingweb.acenet.edu/national-guide/Pages/Course.aspx?org=Huntington%20College%20of%20Health%20Sciences&cid=ffb1a50b-82c4-ea11-a812-000d3a33232a"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = owningOrganizationCtid
			} );
			myData.StartDate = "1994-09-01";
			myData.EndDate = "2001-06-30";

			//============== TransferValue ====================================================
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//==============	transfer value from ===========================================
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs

			var learningOpportunity = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "Environmental Challenges And Solutions",
				Description = "To provide knowledge of the scope and severity of environmental illnesses.",
				SubjectWebpage = "https://stagingweb.acenet.edu/national-guide/Pages/Course.aspx?org=Huntington%20College%20of%20Health%20Sciences&cid=ffb1a50b-82c4-ea11-a812-000d3a33232a",
				DateEffective = "1994-09-01",
				ExpirationDate = "2001-06-30",
				EstimatedDuration = new List<DurationProfile>()
				{
					new DurationProfile() { Description= "135 hours (self-paced)" }
				}
			};
			learningOpportunity.OwnedBy = new List<OrganizationReference>() {  new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Huntington College of Health Sciences",
				Description = "To provide knowledge of the scope and severity of environmental illnesses.",
				SubjectWebpage = "https://stagingweb.acenet.edu/national-guide/Pages/Course.aspx?org=Huntington%20College%20of%20Health%20Sciences&cid=ffb1a50b-82c4-ea11-a812-000d3a33232a"
			} };
			learningOpportunity.Teaches = new List<CredentialAlignmentObject>()
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
					TargetNodeName="Know how to find alternative solutions to chemicals"
				}
			};
			myData.TransferValueFrom.Add( learningOpportunity );

			return myData;
		}
	}
}
