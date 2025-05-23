﻿using Newtonsoft.Json;

using RA.Models.Input;

using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.CredentialingActionRequest;
using APIRequestResource = RA.Models.Input.CredentialingAction;

namespace RA.SamplesForDocumentation
{
    public class PublishCredentialingAction
    {
        /// <summary>
        /// Publish a CredentialingAction or quality assurance action
        /// <returns></returns>
        public string Publish( string requestType = "publish" )
        {
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
            var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
            //typically would have been stored prior to retrieving for publishing

            //create request object
            var myRequest = new APIRequest()
            {
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Populate the CredentialingAction object
            var myData = new APIRequestResource()
            {
                Type = "ceterms:AccreditAction",
                Name = "My Accredit Action Name",
                Description = "This is some text that describes my CredentialingAction.",
                CTID = myCTID,
                ActionStatusType = "ActiveActionStatus",
                StartDate = "1997-04-15",
                EndDate = "2018-05-17"
            };
            //add Publisher
            myData.ActingAgent.Add( new OrganizationReference()
            {
                CTID = "ce-a4041983-b1ae-4ad4-a43d-284a5b4b2d73"
            } );
            //Entity that proves that the action occured or that the action continues to be valid.
            //URI
            myData.EvidenceOfAction = "https://www.qualitymatters.org/reviews-certifications/qm-certified-courses";
            // Object that helped the agent perform the action. e.g. John wrote a book with a pen.
            // Typically this will be one or more CTIDs to an object in the credential registry,
            // Range: any credential, 
            myData.Instrument = new List<string>() { "FullTime", "PartTime" };

            //Object upon which the action is carried out, whose state is kept intact or changed.
            // Range: any Credential, any Learning Opportunity, Assessment, any Organization, ceasn:Competency or ceasn: CompetencyFramework
            // If a blank node is to be used (see next example), add the bnodeId here and add the object to request.ReferenceObjects
            myData.Object = new List<string>() { "ce-de25d94f-eda9-4931-8934-ddc1c065b7c8" };

            //Co-agents that participated in the action indirectly.
            myData.Participant = new List<OrganizationReference>()
            {
                new OrganizationReference()
                {
                    CTID = "ce-accb1835-a067-4a9d-9102-9226cb020404"
                }
            };


            //This holds the CredentialingAction and the identifier (CTID) for the owning organization
            myRequest.CredentialingAction = myData;

            //Serialize the credential request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API
            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "CredentialingAction",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.CredentialingAction.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }

        /// <summary>
        /// A similar example that uses a blank node for Object.
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        public string PublishWithObjectAsReference( string requestType = "publish" )
        {
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
            var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
            //typically would have been stored prior to retrieving for publishing

            //create request object
            var myRequest = new APIRequest()
            {
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Populate the CredentialingAction object
            var myData = new APIRequestResource()
            {
                Type = "ceterms:AccreditAction",
                Name = "My Accredit Action Name",
                Description = "This is some text that describes my CredentialingAction.",
                CTID = myCTID,
                ActionStatusType = "ActiveActionStatus",
                StartDate = "1997-04-15",
                EndDate = "2018-05-17"
            };
            //add Publisher
            //ce-928d9c1b-5c7c-46f6-903d-13d111a73561
            //
            myData.ActingAgent.Add( new OrganizationReference()
            {
                CTID = "ce-a4041983-b1ae-4ad4-a43d-284a5b4b2d73"
            } );
            //Entity that proves that the action occured or that the action continues to be valid.
            //URI
            myData.EvidenceOfAction = "https://www.qualitymatters.org/reviews-certifications/qm-certified-courses";
            // Object that helped the agent perform the action. e.g. John wrote a book with a pen.
            // Typically this will be one or more CTIDs to an object in the credential registry,
            // Range: any credential, 
            myData.Instrument = new List<string>() { "FullTime", "PartTime" };

            //Object upon which the action is carried out, whose state is kept intact or changed.
            // Range: any Credential, any Learning Opportunity, Assessment, any Organization, ceasn:Competency or ceasn: CompetencyFramework
            //
            // This an example of using a blank node. The blank node id would be referenced here. Then an object is created that uses the latter in the BlankNodeId property, and all additional properties would be added. Then the object is added to request.ReferenceObjects

            //create the bnodeId (prefix of "_:" followed by a UUID (V4)
            var thisBNodeId = "_:de25d94f-eda9-4931-8934-ddc1c065b7c8";
            myData.Object = new List<string>()
            {
                thisBNodeId
            };
            //create the organization data
            var organization = new Organization()
            {
                BlankNodeId = thisBNodeId,
                Type = "ceterms:QACredentialOrganization",
                Name = "A QA Organization name",
                Description = "An optional description.",
                SubjectWebpage = "https://example.org/notRequiredButHelpful",
                HasCatalog = new List<string>() { "https://example.org/ourCourseCatalog" },
            };
            //add the organization to the ReferenceObjects (a list of objects)
            myRequest.ReferenceObjects.Add( organization );

            //Co-agents that participated in the action indirectly.
            myData.Participant = new List<OrganizationReference>()
            {
                new OrganizationReference()
                {
                    CTID = "ce-accb1835-a067-4a9d-9102-9226cb020404"
                }
            };


            //This holds the CredentialingAction and the identifier (CTID) for the owning organization
            myRequest.CredentialingAction = myData;

            //Serialize the credential request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API
            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "CredentialingAction",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.CredentialingAction.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }

    };

}

