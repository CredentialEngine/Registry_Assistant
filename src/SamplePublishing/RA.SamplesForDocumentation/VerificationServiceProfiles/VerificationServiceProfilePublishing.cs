using Newtonsoft.Json;

using RA.Models.Input;

using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.VerificationServiceProfileRequest;
using APIRequestResource = RA.Models.Input.VerificationServiceProfile;

namespace RA.SamplesForDocumentation
{
    public class VerificationServiceProfilePublishing
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
            var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
                                                                   //typically would have been stored prior to retrieving for publishing

            //Populate the VerificationServiceProfile object
            var output = new VerificationServiceProfile()
            {
                CTID= myCTID,       //required
                Description = "Sample verification profile",    //required
                DateEffective = "2020-01-01",
                HolderMustAuthorize = true,
                SubjectWebpage = "https://example.com/ourVerificationSite",
                VerificationDirectory = new List<string>() { "https://example.com/ourVerificationDirectory" },
                VerificationService = new List<string>() { "https://example.com/ourActualVerificationServices" },
                VerificationMethodDescription = "A summary of our verification methods."
            };
            //offeredBy is required 
            output.OfferedBy = new List<OrganizationReference>()
            {
                new OrganizationReference() { Type="Organization", CTID= organizationIdentifierFromAccountsSite }
            };
            //VerifiedClaimType: Type of claim provided through a verification service; select from an existing enumeration of such types.
            output.VerifiedClaimType = new List<string>()
            {
                "claimType:BadgeClaim", "claimType:TranscriptClaim"
            };
            //

            //add costs
            //Must be a valid CTDL cost type.
            // Example: Tuition, Application, AggregateCost, RoomOrResidency
            //see: https://credreg.net/ctdl/terms#CostType
            //Description and CostDetails are required properties
            output.EstimatedCost.Add( new CostProfile()
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


            //list of target credentials
            //these will have to have been published to the registry
            //OBSOLETE: LIKELY WILL BE REMOVED
            output.TargetCredential = new List<EntityReference>()
            {
                new EntityReference()
                {
                    Type="Certification",
                    CTID= "ce-969da20e-c127-4175-93f3-0722027ca7fc",
                },
                new EntityReference()
                {
                    Type="Certification",
                    CTID= "ce-652f6f2c-4fff-45d0-9b2f-44a5bb61f927",
                },
                new EntityReference()
                {
                    Type="Certification",
                    CTID= "ce-c7619be1-e35d-4e9e-b921-9d463f9dc15f",
                }
            };

            //This holds the Verification Service Profile and the identifier (CTID) for the offering organization
            var myRequest = new APIRequest()
            {
                VerificationServiceProfile = output,
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
                EndpointType = "VerificationServiceProfile",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.VerificationServiceProfile.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }

    }
}
