using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishTransferValueProfileList
	{

		public string PublishList( bool usingSimplePost = true )
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			// Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();

			//===================================================================================


			// This holds the transfer value profile and the identifier (CTID) for the owning organization
			var myRequest = new TransferValueProfileBulkRequest()
			{
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//add some transfer value profiles
			myRequest.TransferValueProfiles.Add( GetTVPOne( organizationIdentifierFromAccountsSite ) );
			myRequest.TransferValueProfiles.Add( GetTVPTwo( organizationIdentifierFromAccountsSite ) );
			// Serialize the request object
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );


			//assign publish endpoint
			var assistantUrl = SampleServices.GetAppKeyValue( "registryAssistantApi" ) + "transfervalue/publish/";
			string jsonldPayload = "";
			List<string> messages = new List<string>();
			if ( usingSimplePost )
			{
				//use a simple method that returns a string
				result = new SampleServices().SimplePost( assistantUrl, payload, apiKey, ref jsonldPayload, ref messages );

				// Return the result
				return result;
			}

			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = "bulkpublish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfiles[ 0 ].CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			return result;
		}




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
			myData.TransferValue = new List<ValueProfile>()
		{
			new ValueProfile()
			{
				Value=3,
				CreditUnitType = new List<string>() {"DegreeCredit"},
				CreditLevelType = new List<string>() {"LowerDivisionLevel"}
			}
		};

			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			myData.TransferValueFrom.Add( new LearningOpportunity()
			{
				//Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."

			} );

			//						optional
			//coded Notation could be replaced by Identifier in the near future
			myData.StartDate = "2020-01-01";
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
			myData.TransferValue = new List<ValueProfile>()
		{
			new ValueProfile()
			{
				Value=3,
				CreditUnitType = new List<string>() {"DegreeCredit"},
				CreditLevelType = new List<string>() {"LowerDivisionLevel"}
			}
		};

			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			myData.TransferValueFrom.Add( new LearningOpportunity()
			{
				//Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."

			} );

			//						optional
			//coded Notation will likely be replaced by Identifier in the near future
			myData.StartDate = "2020-01-01";
			myData.EndDate = "2021-12-21";

			//===================================================================================
			//				additions in pending ( in near future)
			//identifier will likely replace codedNotation for more flexibility. Although the name may change
			// A third party version of the entity being referenced that has been modified in meaning through editing, extension or refinement.
			myData.Identifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "ACE Course Code",
				IdentifierType = "Internal Code",   //Formal name or acronym of the identifier type
				IdentifierValueCode = "0276"        //Alphanumeric string identifier of the entity
			} );

			return myData;
		}


	}
	
}
