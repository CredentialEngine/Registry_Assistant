using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models.Input;

using APIRequestResource = RA.Models.Input.SupportService;

namespace RA.SamplesForDocumentation
{
    public class PublishSupportServiceList
    {

        /// <summary>
        /// Publish a list of support services
        /// </summary>
        /// <returns></returns>
        public string Publish()
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


        private APIRequestResource AddSupportService1( string owningOrgCtid )
        {

            // Assign a CTID for the entity being published and keep track of it
            // typically would have been stored prior to retrieving for publishing
            // If running this test project multiple times, the actual CTIDs should be hard-coded (so there are no duplicates) or the previous data should be deleted
            var myCTID = "ce-b47445af-10a0-4ba7-8a38-54bda6db699d"; // SampleServices.GetNewCTID();

            //Populate the Support Service object
            var myData = new APIRequestResource()
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


        private APIRequestResource AddSupportService2( string owningOrgCtid )
        {

            // Assign a CTID for the entity being published and keep track of it
            // typically would have been stored prior to retrieving for publishing
            // If running this test project multiple times, the actual CTIDs should be hard-coded (so there are no duplicates) or the previous data should be deleted
            var myCTID = "ce-51b33d3a-3165-4603-b3ab-c871c1db4d65";

            //Populate the Support Service object
            var myData = new APIRequestResource()
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


        private APIRequestResource AddSupportService3( string owningOrgCtid )
        {

            // Assign a CTID for the entity being published and keep track of it
            // typically would have been stored prior to retrieving for publishing
            // If running this test project multiple times, the actual CTIDs should be hard-coded (so there are no duplicates) or the previous data should be deleted
            var myCTID = "ce-e68dd702-f0ba-4f92-921d-077c3ff066ca";

            //Populate the Support Service object
            var myData = new APIRequestResource()
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
    }
}
