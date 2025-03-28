﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	/// <summary>
	/// Sep. 14, 2020
	/// - Added use of dateEffective and expirtationDate for learningopportunity in an EntityReference
	/// </summary>
	public class PublishTransferValueProfile
	{
		/* Usage
		 * - update App.config with your ApiKey and CTID of the owning org
		 * - note the apiKey is not required in the sandbox
		 * Example of this record published on sandbox
		 * https://sandbox.credentialengineregistry.org/graph/ce-fd515001-6a9c-4f43-b401-3e65127fc807
		 */
		/// <summary>
		/// Sample publish method
		/// </summary>
		/// <returns>If successful, returns the formatted graph from the registry.</returns>
		public string PublishSimpleRecord( bool usingSimplePost = true )
		{
			// Holds the result of the publish action
			var result = string.Empty;
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
			//from previous test
			myCTID = "ce-fd515001-6a9c-4f43-b401-3e65127fc807";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Name",
				Description = "This is some text that describes my transfer value profile.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/1234",
				StartDate = "2020-01-01",
				EndDate = "2021-12-21"
			};
			// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
			// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
			// Required
			// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
			// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
			myData.LifeCycleStatusType = "Active";

			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.Identifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "ACE Course Code",
				IdentifierValueCode = "0276"        //Alphanumeric string identifier of the entity
			} );
			//============== TransferValue ================================
			//Required. Provide a transfer value amount using Value and CreditUnitType. ex. DegreeCredit This is a concept scheme and so has a strict vocabulary. 
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//development prpcess profile
			myData.DevelopmentProcess = new List<ProcessProfile>()
			{
				new ProcessProfile() 
				{
					 Description="Description of the development process for this transfer value profile",
					 DateEffective="1991-05-16",
					 SubjectWebpage="https://example.org?t=tvpswp",
					 ExternalInputType = new List<string>() {"Business", "Education Administrators", "Associations" },
					 ProcessFrequency="New development occurs on an as needed basis."
				}
			};

            //==============	transfer value from ===================================================
            //Resource that provides the transfer value described by this resource, according to the entity providing this resource.
            //A list of entity references. If the CTID is known, then just provide it.
            myData.TransferValueFrom.Add( new EntityReference()
            {
                CTID = "ce-476c1aca-6cd9-4dbe-ba91-16960bfb19ac"
            } );
            //If not provided as much information as is available
            //see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
            //NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
            var transferValueFrom = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."
			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Organization that owns this LearningOpportunity",
				SubjectWebpage = "https://myOrganization.com",
				Description = "While optional, a description is helpful."
			};
            transferValueFrom.OwnedBy = new List<OrganizationReference>
            {
                ownedBy
            };

            myData.TransferValueFrom.Add( transferValueFrom );
			myData.TransferValueFrom.Add( AddTransferFromLearningOpportunity() );

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
				RequestType = "publish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}

		/// <summary>
		/// Use SameAs property to equate a LearningOpportunity in the credential registry with a learning opportunity referenced in a TransferValueFrom
		/// </summary>
		/// <param name="usingSimplePost"></param>
		/// <returns></returns>
		public string PublishTVPSameAsRecord( bool usingSimplePost = true )
		{
			// Holds the result of the publish action
			var result = string.Empty;
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
			//from previous test
			//myCTID = "ce-fd515001-6a9c-4f43-b401-3e65127fc807";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Name",
				Description = "This is some text that describes my transfer value profile.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/1234",
				StartDate = "2020-01-01",
				EndDate = "2021-12-21"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
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
			//A list of entity references. 
			//Where the CTID is not known for a document in the registry, provide as much information as is available
			//NOTE: see separate example that uses SameAs to equate a learning opportunity in the registry with a learning opportunity described in a transfer value from (or for)
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var transferValueFrom = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed.",
				SameAs= new List<string>() { "ce-6b839fea-b62b-4d65-a9f7-1461cccb61f4" }		//same as a learning opportunity in the registry
			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Organization that owns this LearningOpportunity",
				SubjectWebpage = "https://myOrganization.com",
				Description = "While optional, a description is helpful."
			};
			transferValueFrom.OwnedBy.Add( ownedBy );
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
				RequestType = "publish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}
		/// <summary>
		/// Format a learning opportunity for use in a transfer value.
		/// Resources of this type are not in the registry
		/// </summary>
		/// <returns></returns>
		private LearningOpportunity AddTransferFromLearningOpportunity()
		{
			var myLopp = new LearningOpportunity()
			{
				Type = "LearningOpportunityProfile",
				Name = "Packet Switched Networks",
				Description = "Understanding of terminology and issues and their application in functioning packet switched networks.",
				LearningMethodDescription = "Methods of instruction include lecture, discussion, laboratory exercises, videotapes, and a final examination."
			};
			myLopp.Teaches = new List<CredentialAlignmentObject>() { new CredentialAlignmentObject()
			{
				TargetNodeDescription="Upon successful completion of this course, the student will be able to describe packet technology; and discuss the history of public packet networks--their practical implementation and management issues related to implementation."
			} };

			myLopp.OwnedBy = new List<OrganizationReference>() {  new OrganizationReference()
			{
				Type="CredentialOrganization",
				Name="Ameritech",
				SubjectWebpage="https://www.ameritech.edu/",
				Address = new List<Place>{ new Place()
				{
					City= "Waukesha",
					AddressRegion= "WI",
					Country="United States"
				} }

			} };

			return myLopp;
		}
	}
}
