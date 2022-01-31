using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using RA.Models.Input;
using RA.Models;
using APIRequestEntity = RA.Models.Input.Collection;
using APIRequest = RA.Models.Input.CollectionRequest;

namespace RA.SamplesForDocumentation.Collections
{
    public class PublishCollection
    {
		/// <summary>
		/// Simple example just using HasMember and URIs
		/// </summary>
		/// <param name="requestType"></param>
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
				Name = "A sample collection for TBD ...",
				Description = "Some purpose for this collection",
				CTID = myCTID,
				InLanguage = new List<string>() { "en-US" },
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//a few members to start with
			myData.HasMember = new List<string>()
			{
				"ce-08cc45f3-6384-4605-a009-784293be4359",
				"ce-37c98f6d-1afc-48a9-8b84-4816c47f1d7f",
				"ce-a7c102d3-2f45-48f7-aa22-9718941d0996"
			};


			//This holds the learningOpportunity and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Collection = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the learningOpportunity request object
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

			return result;

		}

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
			var myCTID = "ce-de4b127d-8234-4f80-9df8-98ba2aae1eff";// "ce-" + Guid.NewGuid().ToString();

			var myData = new APIRequestEntity()
			{
				Name = "A sample collection for TBD ...",
				Description = "Some purpose for this collection",
				CTID = myCTID,
				InLanguage = new List<string>() { "en-US" },
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//in this case, HasMember is empty - a mix can be used however.
			myData.HasMember = new List<string>();
		

			//This holds the learningOpportunity and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Collection = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add collection members that have additional information about members
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A Indigenous Leadership",
				Description = "An optional description.",
				ProxyFor = "ce-08cc45f3-6384-4605-a009-784293be4359",//a published object
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A. Early Childhood Education",
				Description = "An optional description.",
				ProxyFor = "ce-37c98f6d-1afc-48a9-8b84-4816c47f1d7f",//a published object
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );
			myRequest.CollectionMembers.Add( new CollectionMember()
			{
				Name = "Associate’s Degree A.A. Liberal Education",
				Description = "An optional description.",
				ProxyFor = "ce-a7c102d3-2f45-48f7-aa22-9718941d0996",//a published object
				StartDate = "2020-01-01",
				EndDate = "2023-12-31"
			} );


			//Serialize the learningOpportunity request object
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

			return result;

		}

	}
}
