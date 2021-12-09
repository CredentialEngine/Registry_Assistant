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

namespace RA.SamplesForDocumentation.Credentials
{
	public class PublishCredentialWithMultipleCampuses
	{
		/// <summary>
		/// Publish credential that uses AlternateCondition to show that the credential can be attained at multiple campuses.
		/// Feb. 18, 2021 - created for Minnesota to illustrate how to handle the scenario where a learning opportunity that is required for a credential and is offered at multiple locations. 
		/// </summary>
		/// <param name="requestType"></param>
		/// <returns></returns>
		public bool PublishRecord( string requestType = "format" )
		{
			//Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );

			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}           // This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-7824b28e-1210-46f4-ba51-f1e82679f598";// "ce-" + Guid.NewGuid().ToString();

			//A simple credential object - see below for sample class definition
			var myData = new Credential()
			{
				Name = "Bachelor Degree from multiple locations",
				Description = "This same degree is offered at multiple locations. View the AlternativeConditions in the Requires condition profile.",
				CTID = myCTID,
				SubjectWebpage = "http://example.com/?t=credentialwAlternativeConditions",
				CredentialType = "ceterms:BachelorDegree",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "General Management", "Office Management", "etc." },
			};
			//typically the ownedBy is the same as the CTID for the data owner
			//where multiple campuses are possible, publish with the main campuses. Then for the other campuses, publish using the CTID for this credential in the Offers property
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//CTID for Higher learning commission.
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//add a requires Condition profile with conditions and a required learning opportunity.
			/*Scenario: 
				- The learning opportunity will be published to the credential registry
				- The credential must be published before the learning opportunity
				- The learning opportunity is referenced using the Condition Profile property of TargetLearningOpportunity
				- Only the CTID need be provided for a learning opportunity that will be published

			*/

			var alternativeConditions = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Name="campus 1",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-bdd69580-1dba-4aad-a2fe-608ae14e19a0"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 2",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-34d92f0d-1a09-4bab-8144-41147df5f531"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 3",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-35fdd620-4fa6-4919-9ab8-33ded8f0f0b8"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 4",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-6a8dc43a-5347-4160-a4c6-804f13f02113"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 5",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-38614e2e-dc03-454f-834a-971107268aa7"
						}
					}
				}
			};
			myData.Requires = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Description = "To earn this credential the following conditions must be met, and the one of the programs referenced in the AlternativeCondition must be completed.",
					Condition = new List<string>() { "Complete High School", "Have a drivers licence." },
					AlternativeCondition = alternativeConditions
				}
			};


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
			//result = new SampleServices().SimplePost( "credential", requestType, payload, apiKey );

			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "credential",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Credential.CTID.ToLower(),   //added here for logging
				Identifier = myRequest.Credential.CTID.ToLower(),     //useful for logging, might use the ctid
				InputPayload = payload
			};

			return new SampleServices().PublishRequest( req );
			
		}

	}
}
