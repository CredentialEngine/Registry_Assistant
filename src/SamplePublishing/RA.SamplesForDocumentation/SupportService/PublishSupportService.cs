using Newtonsoft.Json;

using RA.Models.Input;

using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.SupportServiceRequest;
using APIRequestResource = RA.Models.Input.SupportService;

namespace RA.SamplesForDocumentation
{
    public class PublishSupportService
    {
        /// <summary>
        /// Publish a SupportService
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
            var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
                                                                   //typically would have been stored prior to retrieving for publishing

            //Populate the Support Service object
            var myData = new APIRequestResource()
            {
                Name = "My Support Service Name",
                Description = "This is some text that describes my Support Service.",
                CTID = myCTID,
                SubjectWebpage = "https://example.org/?t=APIRequestResource1234",
                DeliveryType = new List<string>() { "BlendedDelivery" },
                AvailabilityListing = new List<string>() { "https://example.org/?t=whereIsAvailable" },
                AvailableOnlineAt = new List<string>() { "https://example.org/?t=whereIsAvailableOnline" },
                DateEffective="2010-06-30",
                InLanguage = new List<string>() { "en-US" },
                Keyword = new List<string>() { "wheel chair accessible", "language support" },
                LifeCycleStatusType = "Active"
            };
            //add offeredBy
            myData.OwnedBy.Add( new OrganizationReference()
            {
                CTID = organizationIdentifierFromAccountsSite
            } );
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
            myData.AccomodationType = new List<string>() { "AccessibleParking", "AccessibleRestroom", "PhysicalAccessibility" };
            //Type of frequency with which events typically occur
            myData.SupportServiceType = new List<string>() { "Weekly" };

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



            //This holds the Support Service and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                SupportService = myData,
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
                EndpointType = "SupportService",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.SupportService.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }


    };

}

