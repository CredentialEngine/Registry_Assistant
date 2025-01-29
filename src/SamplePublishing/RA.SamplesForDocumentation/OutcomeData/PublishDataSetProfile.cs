using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using RA.Models.Input;
using RA.Models.Input.profiles.QData;

using APIRequest = RA.Models.Input.DataSetProfileRequest;
using APIRequestResource = RA.Models.Input.profiles.QData.DataSetProfile;

namespace RA.SamplesForDocumentation.OutcomeData
{
    /// <summary>
    /// 25-01-01 A dataSetProfile must now include metrics, via HasMetric, and observations, via HasObservations. 
    /// </summary>
    public class PublishDataSetProfile
    {
        /// <summary>
        /// Sample for publishing outcome data about a single resource and a single timeframe.
        /// Metrics and Observations are still required.
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        public string PublishSimpleData( string requestType = "publish" )
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
                Name = "Outcome data for my credential and a single time period",
                Description = "This is some text that describes my dataset profile.",
                CTID = myCTID,
                Source = "http://example.com/DataSetProfile/Source",
                PublicationStatusType = "Published",
                DateEffective = "2020-09-01",
                License = "http://example.com/DataSetProfile/license",
                Rights = "Information about rights held in and over this resource."
            };
            // Often the data provider is different from the owner of the resource - this is the data provider
            myData.DataProvider = new OrganizationReference()
            {
                CTID = organizationIdentifierFromAccountsSite
            };

            // some optional data
            myData.Identifier.Add( new IdentifierValue()
            {
                IdentifierTypeName = "Some Identifer For Outcome Data",
                IdentifierValueCode = "Catalog: xyz1234 "
            } );

            // RelevantDataSetFor
            // - this is optional, but will want to use it in this case to indicate to what the outcome data refers
            //  - provide one or more (unlikely) CTIDs.
            myData.RelevantDataSetFor = ( new List<string>()
            {
                "ce-541da30c-15dd-4ead-881b-729796024b8f"
            } );

            // DataSetTemporalCoverage (DataSetTimeFrame)
            // This a required class with at least one of StartDate/EndDate, or TimeInterval
            myData.DataSetTemporalCoverage = new DataSetTimeFrame()
            {
                Name = "Ten years after graduation.",
                Description = "The information on this page is based on employment for 2020.",
                StartDate = "2014-07-01",
                EndDate = "2023-06-30",
                TimeInterval = "P10Y" //TimeInterval will be an ISO8601 duration string
            };

            // NOTE/Reminder:
            //  The DataProfile (DataSetTimeFrame.DataAttributes) is deprecated
            //  and not allowed for new DataSetProfiles.
            //  This can be distinguished by thinking of dataSetProfile.DataSetTimeFrame being deprecated, and only using DataSetTemporalCoverage.

            // HasMetric
            //  - this is a required property.
            //  - it is list of URIs such as the CTID for a published Metric or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)
            // - the next example will show how a Metric can be published within the dataSetProfile workflow

            myData.HasMetric = new List<string>()
            {
                "ce-224ebc0b-8d7e-4792-b0e0-8502e27c15fd",
                "ce-453faad5-d302-4e7f-8dbc-a50eed9e2282"
            };

            // There will be a lot of blank nodes used in dataSetProfile publishing. 
            // A bnode can be of any type. 
            // For this example, a work property is used to show storing bnodes. 
            // At the end, this property will be added to the APIRequest.ReferenceObjects
            List<object> referenceObjects = new List<object>();

            // HasDimension
            // Dimensions are optional and not used for this scenario

            // HasObservation
            // Observations are required. The observation relates to a Metric, and if dimensions are present, to points in a dimension
            //  - it is list of URIs such as the CTID for a published resource or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)
            myData.HasObservation = new List<string>()
            {
                "_:observation1",
                "_:observation2",
            };


            // Blank Nodes
            // Create the bnodes
            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    // this must match an entry in HasObservation
                    BlankNodeId = "_:observation1",
                    Comment = "An optional comment to support the observation.",
                    // A Metric is required in IsObservationOf
                    IsObservationOf = "ce-224ebc0b-8d7e-4792-b0e0-8502e27c15fd",
                    // as there are no dimension, then AtPoint is not used
                    AtPoint = new List<string>(),
                    // specify the observation values
                    Value = 12333,
                    SizeOfData = 95,
                    SizeOfPopulation = 100,
                } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    // this must match an entry in HasObservation
                    BlankNodeId = "_:observation2",
                    Comment = "An optional comment to support the observation.",
                    // A Metric is required in IsObservationOf
                    IsObservationOf = "ce-453faad5-d302-4e7f-8dbc-a50eed9e2282",
                    // as there are no dimension, then AtPoint is not used
                    AtPoint = new List<string>(),
                    // specify the observation values
                    Percentage = 37,
                } );


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

            new SampleServices().PublishRequest( req );

            return req.FormattedPayload;
        }


        /// <summary>
        /// Publish observations about a resources for three time periods
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        public string PublishWithDimensions( string requestType = "publish" )
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
                Name = "Outcome data for my credential and three time periods",
                Description = "This is some text that describes my dataset profile.",
                CTID = myCTID,
                Source = "http://example.com/DataSetProfile/Source",
                PublicationStatusType = "Published",
                DateEffective = "1999-09-01",
                License = "http://example.com/DataSetProfile/license",
                Rights = "Information about rights held in and over this resource."
            };

            // Often the data provider is different from the owner of the resource - this is the data provider
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
            //  This can be distinguished by thinking of dataSetProfile.DataSetTimeFrame being deprecated, and only using DataSetTemporalCoverage.

            // HasMetric
            //  - this is a required property.
            //  - it is list of URIs such as the CTID for a published Metric or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)

            myData.HasMetric = new List<string>()
            {
                "ce-224ebc0b-8d7e-4792-b0e0-8502e27c15fd",
                "ce-453faad5-d302-4e7f-8dbc-a50eed9e2282"
            };

            // There will be a lot of blank nodes used in dataSetProfile publishing. 
            // A bnode can be of any type. 
            // For this example, a work property is used to show storing bnodes. 
            // At the end, this property will be added to the APIRequest.ReferenceObjects
            List<object> referenceObjects = new List<object>();

            // HasDimension
            // Dimensions are optional. If a dataSetProfile relates to one resource (say a credential) and one time period/interval, then there is no need to add an arbitrary dimension
            //  - it is list of URIs such as the CTID for a published resource or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)

            // NOTE: a valid blank node identifier has the following pattern:
            //      _: following by a guid, all lowercase
            //  ex  "_:dafdadb9-3f15-4d75-81e1-82dec7e0a3d4
            // However for clarity, these examples will use 'friendlier' codes
            // there will be one dimension for time periods, and one for credentials
            myData.HasDimension = new List<string>()
            {
                "_:dimensionTimePeriod",
                "_:dimensionCredentials"
            };

            // Blank Nodes
            // add the dimension bnodes
            referenceObjects.Add(
                new Dimension()
                {
                    Type = "qdata:Dimension",
                    // this must match an entry in HasObservation
                    BlankNodeId = "_:dimensionTimePeriod",
                    Name = "Time periods for wage statistics",
                    // HasPoint will contain bnode ids for two time periods (which will be qdata:DataSetTimeFrame objects)
                    HasPoint = new List<string>()
                    {
                        "_:timeperiod1",
                        "_:timeperiod2",
                    }

                } );
            // add dimension for credentials
            referenceObjects.Add(
                new Dimension()
                {
                    Type = "qdata:Dimension",
                    // this must match an entry in HasObservation
                    BlankNodeId = "_:dimensionCredentials",
                    Name = "Credentials for wage statistics",
                    // HasPoint will contain CTIDs for published resources, but could reference a bnode
                    HasPoint = new List<string>()
                    {
                        "ce-04c2939d-cr01-4b45-b65c-7eca87d4245e",
                        "ce-04c2939d-cr02-4b45-b65c-7eca87d4245e",
                        "ce-04c2939d-cr03-4b45-b65c-7eca87d4245e",
                    }

                } );

            // add the two time periods
            referenceObjects.Add(
                new DataSetTimeFrame()
                {
                    Type = "qdata:DataSetTimeFrame",
                    // this must match an entry in dimenstion.HasPoint
                    BlankNodeId = "_:timeperiod1",
                    Name = "Five years after graduation",
                    StartDate = "2014-06-30",
                    EndDate="2019-07-01",
                    TimeInterval = "P5Y"

                } );

            referenceObjects.Add(
                new DataSetTimeFrame()
                {
                    Type = "qdata:DataSetTimeFrame",
                    // this must match an entry in dimenstion.HasPoint
                    BlankNodeId = "_:timeperiod2",
                    Name = "Ten years after graduation",
                    StartDate = "2014-06-30",
                    EndDate = "2024-07-01",
                    TimeInterval = "P10Y"

                } );


            // HasObservation
            // with 3 credentials, and 2 time periods, there will be 6 observations
            // Observations are required. The observation relates to a Metric, and if dimensions are present, to points in a dimension
            //  - it is list of URIs such as the CTID for a published resource or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)
            myData.HasObservation = new List<string>()
            {
                "_:obsCred1Time1",
                "_:obsCred2Time1",
                "_:obsCred3Time1",
                "_:obsCred1Time2",
                "_:obsCred2Time2",
                "_:obsCred3Time2",
            };


            // now the observations
            // time period 1
            referenceObjects.Add( 
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred1Time1",
                    AtPoint = new List<string>()
                    {
                        "_:timeperiod1",
                        "ce-04c2939d-cr01-4b45-b65c-7eca87d4245e",
                    },
                    Value = 45811
                });

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred2Time1",
                    AtPoint = new List<string>()
                {
                                "_:timeperiod1",
                                "ce-04c2939d-cr02-4b45-b65c-7eca87d4245e",
                },
                    Value = 45821
                } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred3Time1",
                    AtPoint = new List<string>()
                {
                                "_:timeperiod1",
                                "ce-04c2939d-cr03-4b45-b65c-7eca87d4245e",
                },
                    Value = 45831
                } );

            // time period 2
            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred1Time2",
                    AtPoint = new List<string>()
                    {
                        "_:timeperiod1",
                        "ce-04c2939d-cr02-4b45-b65c-7eca87d4245e",
                    },
                    Value = 45812
                } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred2Time2",
                    AtPoint = new List<string>()
                    {
                        "_:timeperiod2",
                        "ce-04c2939d-cr02-4b45-b65c-7eca87d4245e",
                    },
                        Value = 45822
                    } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred3Time2",
                    AtPoint = new List<string>()
                    {
                        "_:timeperiod2",
                        "ce-04c2939d-cr03-4b45-b65c-7eca87d4245e",
                    },
                    Value = 458832
                } );
            //====================	DataSetProfile REQUEST ====================
            //This holds the DataSetProfile and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                DataSetProfile = myData,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite,
                // include the blank node list (reference objects)
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
