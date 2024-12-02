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
            // Holds the result of the publish action
            var result = string.Empty;
            // Assign the api key - acquired from organization account of the organization doing the publishing
            var apiKey = SampleServices.GetMyApiKey();
            if ( string.IsNullOrWhiteSpace( apiKey ) )
            {
                //ensure you have added your apiKey to the app.config
                return $"Error - a publishing API Key was not provided. ";
            }

            // This is the CTID of the organization that owns the data being published
            var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
            if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
            {
                //ensure you have added your organization account CTID to the app.config
                return $"Error - a owining organization CTID was not provided. ";
            }

            //Assign a CTID for the entity being published and keep track of it
            var myCTID = SampleServices.GetNewCTID();
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
                DateEffective = "2010-06-30",
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

            //HasSpecificService
            //Add list of specific support services that are related to the current support service or could be considered like members of a collection.
            //Using HasSpecificService can be useful for "grouping" together support services. Then a resource like a learning opportunity can just refer to the "top level" support service rather than all available support services.
            myData.HasSpecificService = new List<string>() { "ce-d0740c95-336d-46ca-a9ee-06d080a948e7", "ce-a3a8162d-1f1b-4a71-bcb3-0d2697eab952" };

            //IsSpecificServiceOf.
            //Use this property to reference a support service that includes this support service (the inverse of HasSpecificService). Typically only one of HasSpecificService or IsSpecificServiceOf would be used unless there is a hierarchical structure like may be seen in say a competency framework. 
            myData.IsSpecificServiceOf = new List<string>() { "ce-b64e0c4e-01a3-45a0-bc6b-d2b62b979bbe" };

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

            //AccommodationType- Type of modification to facilitate equal access for people to a physical location, resource, or service.
            //include valid concepts, with or without the namespace
            //These must be valid concepts in the Accommodation('https://credreg.net/ctdl/terms/Accommodation') concept scheme
            myData.AccommodationType = new List<string>() { "AccessibleParking", "AccessibleRestroom", "PhysicalAccessibility" };
            //SupportServiceType - Types of support services offered by an agent; select from an existing enumeration of such types.
            //include valid concepts, with or without the namespace
            //These must be valid concepts in the SupportServiceCategory ('https://credreg.net/ctdl/terms/SupportServiceCategory') concept scheme
            myData.SupportServiceType = new List<string>() { "Mentoring", "Academic Advising" };

            //add costs
            //Must be a valid CTDL cost type.
            // Example: Tuition, Application, AggregateCost, RoomOrResidency
            //see: https://credreg.net/ctdl/terms#CostType
            //Description and CostDetails are now optional
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
            // Return the result
            result = req.FormattedPayload;

            return result;
        }

        /// <summary>
        /// Publish a list of support services
        /// </summary>
        /// <returns></returns>
        public string PublishList()
        {
            //Holds the result of the publish action
            var result = string.Empty;
            // Assign the api key - acquired from organization account of the organization doing the publishing
            var apiKey = SampleServices.GetMyApiKey();
            if ( string.IsNullOrWhiteSpace( apiKey ) )
            {
                //ensure you have added your apiKey to the app.config
                return $"Error - a publishing API Key was not provided. ";
            }

            // This is the CTID of the organization that owns the data being published
            var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
            if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
            {
                //ensure you have added your organization account CTID to the app.config
                return $"Error - a owining organization CTID was not provided. ";
            }

            //This holds the Support Service and the identifier (CTID) for the owning organization
            var myRequest = new BulkSupportServiceRequest()
            {
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite,
                SupportServices = new List<APIRequestResource>()
            };

            // add the support services
            myRequest.SupportServices.Add( AddSupportService1( organizationIdentifierFromAccountsSite ) );
            myRequest.SupportServices.Add( AddSupportService2( organizationIdentifierFromAccountsSite ) );
            myRequest.SupportServices.Add( AddSupportService3( organizationIdentifierFromAccountsSite ) );

            //Serialize the credential request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API
            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "SupportService",
                RequestType = "bulkpublish",
                OrganizationApiKey = apiKey,
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            // Return the result
            // NOTE: any "list" endpiont will return a list of response objects.
            //      For test purposes here, only the last payload is returned
            //      All payloads will be written to the log folder 
            //      See the trace log for all messages
            result = req.FormattedPayload;

            if ( req.Messages.Count > 0 )
            {
                // errors were encountered. These would have been written to the trace log.
            }
            return result;
        }


        private SupportService AddSupportService1( string owningOrgCtid )
        {

            // Assign a CTID for the entity being published and keep track of it
            // typically would have been stored prior to retrieving for publishing
            // If running this test project multiple times, the actual CTIDs should be hard-coded (so there are no duplicates) or the previous data should be deleted
            var myCTID = "ce-b47445af-10a0-4ba7-8a38-54bda6db699d"; // SampleServices.GetNewCTID();

            //Populate the Support Service object
            var myData = new SupportService()
            {
                Name = "My Support Service One",
                Description = "This is some text that describes my Support Service.",
                CTID = myCTID,
                SubjectWebpage = "https://example.org/?t=SupportService01",
                DeliveryType = new List<string>() { "BlendedDelivery" },
                InLanguage = new List<string>() { "en-US" },
                LifeCycleStatusType = "Active"
            };

            // add ownedBy
            myData.OwnedBy.Add( new OrganizationReference()
            {
                CTID = owningOrgCtid
            } );

            // General AvailableAt should only be included if a full address is available
            myData.AvailableAt = new List<Place>()
            {
                new Place()
                {
                    Name = "Office of Student Financial Aid",
                    Address1="One University Plaza",
                    City="Springfield",
                    PostalCode="62703",
                    AddressRegion="IL",
                    Country="United States"
                }
            };


            //SupportServiceType - Types of support services offered by an agent; select from an existing enumeration of such types.
            //include valid concepts, with or without the namespace
            //These must be valid concepts in the SupportServiceCategory ('https://credreg.net/ctdl/terms/SupportServiceCategory') concept scheme
            myData.SupportServiceType = new List<string>()
            {
                "support:AcademicGuidance",
                "support:Counseling",
                "support:MentalHealthService",
                "support:BenefitsSupport",
                "support:CareerAdvising",
                "support:HousingAccommodation",
                "support:PeerService"
            };


            return myData;

        }


        private SupportService AddSupportService2( string owningOrgCtid )
        {

            // Assign a CTID for the entity being published and keep track of it
            // typically would have been stored prior to retrieving for publishing
            // If running this test project multiple times, the actual CTIDs should be hard-coded (so there are no duplicates) or the previous data should be deleted
            var myCTID = "ce-51b33d3a-3165-4603-b3ab-c871c1db4d65";

            //Populate the Support Service object
            var myData = new SupportService()
            {
                Name = "My Support Service Two",
                Description = "This is some text that describes my Support Service.",
                CTID = myCTID,
                SubjectWebpage = "https://example.org/?t=SupportService02",
                InLanguage = new List<string>() { "en-US" },
                LifeCycleStatusType = "Active"
            };

            // add ownedBy
            myData.OwnedBy.Add( new OrganizationReference()
            {
                CTID = owningOrgCtid
            } );


            //AccommodationType- Type of modification to facilitate equal access for people to a physical location, resource, or service.
            //include valid concepts, with or without the namespace
            //These must be valid concepts in the Accommodation('https://credreg.net/ctdl/terms/Accommodation') concept scheme
            myData.AccommodationType = new List<string>() {
                "accommodation:PhysicalAccessibility",
                "accommodation:AccessibleHousing",
                "accommodation:AccessibleParking",
                "accommodation:AccessibleRestroom",
                "accommodation:AdjustableLighting",
                "accommodation:AdjustableWorkstations",
                "accommodation:AlternativeFormats",
                "accommodation:AssistiveTechnology",
                "accommodation:AudioCaptioning",
                "accommodation:CaptioningAndTranscripts",
                "accommodation:ClearSignage",
                "accommodation:ColorBlindness",
                "accommodation:Communication",
                "accommodation:DietaryAccommodation",
                "accommodation:FacilityAccommodation",
                "accommodation:FlexibleSchedule",
                "accommodation:HearingLoops",
                "accommodation:MultipleLanguage",
                "accommodation:PhysicalAccessibility",
                "accommodation:PlainLanguage",
                "accommodation:ResourceAndServiceAccommodation",
                "accommodation:ScreenReader",
                "accommodation:Sensory",
                "accommodation:ServiceAnimal",
                "accommodation:TactileSignage" };

            //SupportServiceType - Types of support services offered by an agent; select from an existing enumeration of such types.
            //include valid concepts, with or without the namespace
            //These must be valid concepts in the SupportServiceCategory ('https://credreg.net/ctdl/terms/SupportServiceCategory') concept scheme
            myData.SupportServiceType = new List<string>() { "support:AssistiveTechnologySupport", "support:AudiologicalHealthCare", "support:NeurodivergenceService", "support:TestAssistance", "support:Transportation" };


            return myData;

        }


        private SupportService AddSupportService3( string owningOrgCtid )
        {

            // Assign a CTID for the entity being published and keep track of it
            // typically would have been stored prior to retrieving for publishing
            // If running this test project multiple times, the actual CTIDs should be hard-coded (so there are no duplicates) or the previous data should be deleted
            var myCTID = "ce-e68dd702-f0ba-4f92-921d-077c3ff066ca";

            //Populate the Support Service object
            var myData = new SupportService()
            {
                Name = "The Career Center Support Services",
                Description = "This is some text that describes my Support Service.",
                CTID = myCTID,
                SubjectWebpage = "https://example.org/?t=SupportService03",
                InLanguage = new List<string>() { "en-US" },
                LifeCycleStatusType = "Active"
            };

            // add ownedBy
            myData.OwnedBy.Add( new OrganizationReference()
            {
                CTID = owningOrgCtid
            } );

            //SupportServiceType - Types of support services offered by an agent; select from an existing enumeration of such types.
            //include valid concepts, with or without the namespace
            //These must be valid concepts in the SupportServiceCategory ('https://credreg.net/ctdl/terms/SupportServiceCategory') concept scheme
            myData.SupportServiceType = new List<string>() { "support:CareerExploration", "support:JobPlacement" };


            return myData;

        }
    };

}

