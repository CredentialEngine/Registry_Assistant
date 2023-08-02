using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.CollectionRequest;
using APIRequestEntity = RA.Models.Input.Collection;
using Newtonsoft.Json;

using RA.Models.Input;
using System.Linq;

namespace RA.SamplesForDocumentation.Collections
{
    /// <summary>
    /// Code samples for publishing collections
    /// </summary>
    public class PublishCollection
    {
		List<string> testCollectionCredentials = new List<string>()
			{
				"ce-db250674-0dc5-4f1b-af90-a86992b6e741",
				"ce-db91447a-23da-4949-a71a-4203031d9032",
				"ce-474a55b1-0806-4f5d-ae27-d3ca79b20e29",
				"ce-34d50921-dff5-4613-9dcf-f9732bbfe88a",
				"ce-34d50921-dff5-4613-9dcf-f9732bbfe88a", //yes a duplicate to test that a duplicate will be recognized and ignored
			};
	/// <summary>
	/// Simple example just using HasMember and URIs
	/// </summary>
	/// <param name="requestType">Format or Publish</param>
	/// <returns></returns>
	public bool Simple( string requestType = "format" )
		{

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
			}//
			 //Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-bb733c95-1df3-445e-98b6-bdaf7ca74a36";// "ce-" + Guid.NewGuid().ToString();

			var myData = new APIRequestEntity()
			{
				Name = "A sample collection of credentials - College Of DuPage.",
				Description = "This collection uses the HasMember property to list members of this collection using the CTIDs of a published credentials.",
				CTID = myCTID,
				InLanguage = new List<string>() { "en-US" },

			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//a few members to start with
			myData.HasMember = testCollectionCredentials;
			//myData.HasMember.Add( "ce-34d50921-dff5-4613-9dcf-f9732bbfe88a" ); //test a duplicate that should be skipped.

			//This holds the main entity and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Collection = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the request object
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "collection",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Collection.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			var result = new SampleServices().PublishRequest( req );
			if ( req.Messages.Count > 0 )
			{

			}
			return result;

		}

		/// <summary>
		/// Code sample including a MemberCondition, CollectionType, and LifeCycleStatusType. 
		/// As well uses the CollectionMember class to provide additional information about members of this collection including:
		/// - Name
		/// - Optional start and end dates (for membership in this collection)
		/// 
		/// <see href="https://sandbox.credentialengineregistry.org/graph/ce-3bc3d4a3-c2de-4c16-8d7b-caca771b12f4"/>
		/// </summary>
		/// <param name="requestType">Format or Publish</param>
		/// <returns></returns>
		public bool PublishWithCollectionMembers( string requestType = "format" )
		{

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
			}//
			 //Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-3bc3d4a3-c2de-4c16-8d7b-caca771b12f4";// "ce-" + Guid.NewGuid().ToString();

			var myData = new APIRequestEntity()
			{
				Name = "A sample collection of credentials using CollectionMembers.",
				AlternateName = new List<string>() { "alternate Dos", "Alternate Deux" },
				Description = "This collection uses the CollectionMembers property (part of the CollectionRequest object) to list members of this collection. A CollectionMember has additional properties to describe the member such as the (optional) start date and end date, as well as a name and description. The CollectionMember uses teh ProxyFor proper to 'point' to a publshed resource. Typically just a CTID is used for this property. The API will create a property credential registry URI based on the current publishing environment.",
				CTID = myCTID,
				SubjectWebpage="https://example.org?t=collectionSWP",
				DateEffective="2017-12-07",
				ExpirationDate="2035-12-31",
				InLanguage = new List<string>() { "en-US" },
				Subject = new List<string>() { "testing publishing", "complete testing" },
				Keyword = new List<string>() { "testing publishing", "complete testing" },
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//list type
			myData.CollectionType = new List<string>() { "ETPL" };
			//lifeCycleStatus
			myData.LifeCycleStatusType = "Active";
			myData.License = "https://example.com/t=license";
			//in this case, HasMember is empty - a mix can be used however.
			myData.HasMember = new List<string>();
			//add membership conditions
			myData.MembershipCondition.Add( new ConditionProfile()
			{
				Description= "Text describing the requirements for a resource to be a member of this collection",
				Condition = new List<string>()
                {
					"Requirement one",
					"Requirement two",
					"Requirement three",
				}
			} );

			List<string> alternateTypes = new List<string>();
			List<string> codes = new List<string>();
			//====================	OCCUPATIONS ====================
			myData.OccupationType = OccupationsHelper.PopulateOccupations( ref alternateTypes, ref codes );
			if (alternateTypes != null && alternateTypes.Any())
				myData.AlternativeOccupationType = alternateTypes;
            if (codes != null && codes.Any())
                myData.ONET_Codes = codes;
			//====================	INDUSTRIES	====================
			myData.IndustryType = Industries.PopulateIndustries( ref alternateTypes, ref codes );
            if (alternateTypes != null && alternateTypes.Any())
                myData.AlternativeIndustryType = alternateTypes;
            if (codes != null && codes.Any())
                myData.NaicsList = codes;
			//====================	INSTRUCTIONAL PROGRAMS	====================
			myData.InstructionalProgramType = InstructionalPrograms.PopulatePrograms( ref alternateTypes, ref codes);
            if (alternateTypes != null && alternateTypes.Any())
                myData.AlternativeInstructionalProgramType = alternateTypes;
            if (codes != null && codes.Any())
                myData.CIP_Codes = codes;

			//This holds the learningOpportunity and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Collection = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add collection members that have additional information about members
			//CollectionMembers is a property of the Request object, not the Collection. Upon publish, blank nodes will be added to the graph, and the Ids of the blank nodes will be added to the HasMembers property. 
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A Indigenous Leadership",
				Description = "An optional description.",
				ProxyFor = testCollectionCredentials[0],//a published object
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A. Early Childhood Education",
				Description = "An optional description.",
				ProxyFor = testCollectionCredentials[1],	//a published object
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A. Liberal Education",
				Description = "An optional description.",
				ProxyFor = testCollectionCredentials[2],    //a published object
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A.S. Business Management",
				Description = "An optional description.",
				ProxyFor = testCollectionCredentials[3],    //a published object
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A. Liberal Education, STEM Emphasis",
				Description = "An optional description.",
				ProxyFor = testCollectionCredentials[4],    //this is a duplicate CTID and should be rejected
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );

			//Serialize the request object
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "collection",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Collection.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			var result = new SampleServices().PublishRequest( req );
			if (req.Messages.Count > 0)
            {

            }
			return result;

		}

		#region publish like a framework
		public bool PublishLikeAFrameworkWithCompetencies( string requestType = "format" )
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
			var myData = new APIRequestEntity()
			{
				Name = "My Sample Collection",
				Description = "This is some text that describes my Collection.",
				CTID = myCTID,
				Keyword = new List<string>() { "Testing", "Prototype"},
				 
				ONET_Codes= new List<string>() { "19-4090", "21-1090" },
				OwnedBy = new List<OrganizationReference>() 
				{ 
					new OrganizationReference() 
					{
						Type="Organization",
						CTID = organizationIdentifierFromAccountsSite
					} 
				}
			};

			//This holds the data and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Collection = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add competencies
			//example of a flat framework
			myRequest.Members.Add( MapCompetency( myCTID, "Looks both ways before crossing street" ) );
			myRequest.Members.Add( MapCompetency( myCTID, "Looks before leaping" ) );
			myRequest.Members.Add( MapCompetency( myCTID, "Deals with the faults of others as gently as their own" ) );
			myRequest.Members.Add( MapCompetency( myCTID, "Knows what he/she knows and does not know what he/she does not know " ) );

			//Serialize the request object
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "collection",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Collection.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			//Serialize the request object
			return new SampleServices().PublishRequest( req );

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
	}
}
