using Newtonsoft.Json;

using RA.Models.Input;

using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.ScheduledOfferingRequest;
using APIRequestResource = RA.Models.Input.ScheduledOffering;

namespace RA.SamplesForDocumentation
{
	public class PublishScheduledOffering
	{
		/// <summary>
		/// Publish a ScheduledOffering
		/// <returns></returns>
		public string Publish( string requestType = "publish" )
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
			var myCTID = "ce-a11fe49a-b546-4853-b83b-17f24d1ac42b";// "ce-" + Guid.NewGuid().ToString().ToLower();
			//typically would have been stored prior to retrieving for publishing

			//Populate the Scheduled Offering object
			var myData = new APIRequestResource()
			{
				Name = "My Scheduled Offering Name",
				Description = "This is some text that describes my Scheduled Offering.",
				CTID = myCTID,
				SubjectWebpage = "https://example.org/?t=APIRequestResource1234",
				DeliveryType = new List<string>() { "BlendedDelivery" },
				AvailabilityListing = new List<string>() { "https://example.org/?t=whereIsAvailable" },
				AvailableOnlineAt = new List<string>() { "https://example.org/?t=whereIsAvailableOnline" },
				DeliveryTypeDescription= "Optional description of the delivery type.",
            };
			//add offeredBy
			myData.OfferedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="One University Plaza",
					City="Springfield",
					PostalCode="62703",
					AddressRegion="IL",
					Country="United States"
				}
			};
			//

			//include valid concepts, with or without the namespace
			//Type of frequency at which a resource is offered
			myData.OfferFrequencyType = new List<string>() { "EventBased" };
			//Type of frequency with which events typically occur
			myData.ScheduleFrequencyType = new List<string>() { "Weekly" };
			//Type of time at which events typically occur; 
			myData.ScheduleTimingType = new List<string>() { "Evening" };
			//
			//duration for a program that is exactly 9 months
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Months=9
					}
				}
			};
			//add costs
			//Must be a valid CTDL cost type.
			// Example: Tuition, Application, AggregateCost, RoomOrResidency
			//see: https://credreg.net/ctdl/terms#CostType
			//Description and CostDetails are required properties
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



			//This holds the Scheduled Offering and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				ScheduledOffering = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "scheduledOffering",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.ScheduledOffering.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;
		}


	};

}

