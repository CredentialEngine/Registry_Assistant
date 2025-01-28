using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using RA.Models.Input;
using RA.Models.Input.profiles.QData;

using APIRequest = RA.Models.Input.DataSetProfileRequest;
using APIRequestResource = RA.Models.Input.profiles.QData.DataSetProfile;

namespace RA.SamplesForDocumentation.OutcomeData
{
    public class PublishDataSetProfile
    {
        /// <summary>
        /// 25-01-01 A dataSetProfile must now include metrics, via HasMetric, and observations, via HasObservations. 
        /// </summary>
        /// <param name="requestType"></param>
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
            }

            //Assign a CTID for the entity being published and keep track of it
            var myCTID = "ce-" + Guid.NewGuid().ToString();

            //A simple DataSetProfile object - see below for sample class definition
            var myData = new APIRequestResource()
            {
                Name = "My dataset profile Name",
                Description = "This is some text that describes my dataset profile.",
                CTID = myCTID,
                Source = "http://example.com/DataSetProfile/Source",
                PublicationStatusType = "Published",
                DateEffective = "1999-09-01",
                License = "http://example.com/DataSetProfile/license",
                Rights = "Information about rights held in and over this resource."
            };
            //typically the ownedBy is the same as the CTID for the data owner
            myData.DataProvider = new OrganizationReference()
            {
                CTID = organizationIdentifierFromAccountsSite
            };

            // RelevantDataSetFor
            // - optional
            // - if the dataSetProfile is about only one resource, then ReleventDataSetFor can be used rather than creating a Dimension with one entry. 
            //  - provide one or more (unlikely) CTIDs.
            myData.RelevantDataSetFor = ( new List<string>()
            {
                "ce-541da30c-15dd-4ead-881b-729796024b8f"
            } );

            myData.Identifier.Add( new IdentifierValue()
            {
                IdentifierTypeName = "Some Identifer For Outcome Data",
                IdentifierValueCode = "Catalog: xyz1234 "
            } );


            // DataSetTemporalCoverage (DataSetTimeFrame)
            // This a required class with at least one of StartDate/EndDate, or TimeInterval
            myData.DataSetTemporalCoverage = new DataSetTimeFrame()
            {
                Name = "An optional name for the time period.",
                Description = "The information on this page is based on employment for 2020.",
                StartDate = "2010-01-01",
                EndDate = "2020-12-31",
                TimeInterval = "P10Y" //TimeInterval will be an ISO8601 duration string
            };
            // NOTE/Reminder:
            //  The DataProfile (DataSetTimeFrame.DataAttributes) is deprecated
            //  and not allowed for new DataSetProfiles.
            //  This can be distinguished by thinking of dataSetProfile.DataSetTimePeriod being deprecated, and only using DataSetTemporalCoverage.

            // HasMetric
            //  - this is a required property.
            //  - it is list of URIs such as the CTID for a published Metric or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)

            myData.HasMetric = new List<string>()
            {
                "ce-224ebc0b-8d7e-4792-b0e0-8502e27c15fd",
                "ce-453faad5-d302-4e7f-8dbc-a50eed9e2282"
            };

            // HasDimension
            // Dimensions are optional. If a dataSetProfile relates to one resource (say a credential) and one time period/interval, then there is no need to add an arbitrary dimension
            //  - it is list of URIs such as the CTID for a published resource or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)

            // NOTE: a valid blank node identifier has the following pattern:
            //      _: following by a guid, all lowercase
            //  ex  "_:dafdadb9-3f15-4d75-81e1-82dec7e0a3d4
            // However for clarity, these examples will use 'friendlier' codes
            myData.HasDimension = new List<string>()
            {
                "_:dimension1",
                "_:dimension2",
            };

            // HasObservation
            // Observations are required. The observation relates to a Metric, and if dimensions are present, to points in a dimension
            //  - it is list of URIs such as the CTID for a published resource or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)
            myData.HasObservation = new List<string>()
            {
                "_:observation1",
                "_:observation2",
                "_:observation3",
                "_:observation4",
            };


            // Blank Nodes
            // Create the objects that will metrics (recommended to use published metrics rather than bnodes), dimensions, observations, and more
            Dimension dimension1 = new Dimension()
            {
                Type = "qdata:Dimension",
                BlankNodeId = "_:dimension1",
                Name = "Credentials",
                Description = "A list of credentials that will have observations.",
                DimensionType = "ceterms:Credential",
                // comparing three credentials 
                HasPoint = new List<string>()
                {
                    "ce-66f641d8-6556-42b8-961a-ad4253e501c3",
                    "ce-80d44af0-10a6-4b13-8bd0-19c675df9b14",
                    "ce-0199633b-c59a-49a2-9071-e2a457ee3551"
                }
            };

            Dimension dimension2 = new Dimension()
            {
                Type = "qdata:Dimension",
                BlankNodeId = "_:dimension2",
                Name = "Time Periods",
                Description = "List of time periods that will have observations",
                DimensionType = "qdta:DataSetTimeFrame",
                // comparing three timeperiods 
                HasPoint = new List<string>()
                {
                    "_:timeperiod1",
                    "_:timeperiod2",
                    "_:timeperiod3",
                }
            };

            // now the observations
            Observation obs1 = new Observation()
            {
                Type = "qdata:Observation",
                BlankNodeId = "_:observation1",
                Comment = "An optional comment to support the .",
                DimensionType = "ceterms:Credential",
                // comparing three credentials 
                HasPoint = new List<string>()
                {
                    "ce-66f641d8-6556-42b8-961a-ad4253e501c3",
                    "ce-80d44af0-10a6-4b13-8bd0-19c675df9b14",
                    "ce-0199633b-c59a-49a2-9071-e2a457ee3551"
                }
            };


            List<object> referenceObjects = new List<object>();
            referenceObjects.Add(dimension1);
            referenceObjects.Add(dimension2);
            //====================	DataSetProfile REQUEST ====================
            //This holds the DataSetProfile and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                DataSetProfile = myData,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite,
                ReferenceObjects = referenceObjects
            };

            //Serialize the DataSetProfile request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API

            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "DataSetProfile",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.DataSetProfile.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );

            return req.FormattedPayload;
        }

    }
}
