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
    /// For a sample of publishing of Metrics, see:
    ///     RA.SamplesForDocumentation.OutcomeData.PublishMetric
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
            myData.Identifier = new List<IdentifierValue>()
            {
                new IdentifierValue()
                {
                    IdentifierTypeName = "Some Identifer For Outcome Data",
                    IdentifierValueCode = "Catalog: xyz1234 "
                }
            };

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
            //  This can be distinguished by thinking of dataSetProfile.DataSetTimePeriod as being deprecated, and only using DataSetTemporalCoverage.

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
                    // this 'point' identifier will be referenced in HasObservation.AtPoint
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
                    // this 'point' identifier will be referenced in HasObservation.AtPoint
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
                // be sure to inclue the referenceObjects in the request
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
        /// Publish observations about a resources for three time periods.
        /// Bonus: publish a new Metric as part of the same publishing transaction.
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

            myData.Identifier = new List<IdentifierValue>()
            {
                new IdentifierValue()
                {
                    IdentifierTypeName = "Some Identifer For Outcome Data",
                    IdentifierValueCode = "Catalog: xyz1234 "
                }
            };


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
            // BONUS: this endpoint has the option of being able to publish Metric as part of the same publish step as the dataSetProfile
            // using a variable for easy reference, even though only one metric
            var metric1 = "ce-5756ddbf-cb7e-4d0c-9a1e-db330dbcdf6b";

            myData.HasMetric = new List<string>()
            {
                // this metric had not been published yet, and will be added to ReferenceObjects
                // the API will first check if a CTID is in the ReferenceObjects before checking for existance in the registry.
                metric1
            };

            // There will be a lot of blank nodes used in dataSetProfile publishing. 
            // A bnode can be of any type. 
            // For this example, a work property is used to show storing bnodes. 
            // At the end, this property will be added to the APIRequest.ReferenceObjects
            List<object> referenceObjects = new List<object>();

            // add the new Metric
            var metric = new Metric()
            {
                CTID = metric1,
                // an @id is still required (for general validation)
                Id = "_:5756ddbf-cb7e-4d0c-9a1e-db330dbcdf6b",
                // always need an  @type
                Type = "qdata:Metric",
                Name = "Employment Rate",
                Description = "Percent of graduates who were employed in Pennsylvania at 10 years after graduation.",
                // a publisher is required
                Publisher = new List<string>() { organizationIdentifierFromAccountsSite },
                MetricType = new List<string>() { "Employment", "metricCat:Earnings" }
            };
            referenceObjects.Add( metric );

            // HasDimension
            // Dimensions are optional. If a dataSetProfile relates to one resource (say a credential) and one time period/interval, then there is no need to add an arbitrary dimension
            //  - it is list of URIs such as the CTID for a published resource or
            //      a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)


            // Blank Node Identifiers
            // NOTE: a valid blank node identifier has the following pattern:
            //      _: following by a guid, all lowercase
            //  ex  "_:dafdadb9-3f15-4d75-81e1-82dec7e0a3d4
            // However for clarity, these examples will use properties with 'friendlier' names for easy reference
            var dimensionTimePeriod = "_:04dc0493-aa99-4f1d-a612-10b6873ea9ce";
            var dimensionCredentials = "_:e0459386-0f2b-424e-9d05-e8e09426aefd";
            var timeperiod1 = "_:1aaebe1e-3b16-42ac-aed8-71fdf6a2cfed";
            var timeperiod2 = "_:6b7a2c20-7000-4718-a725-6fe4a04bd1e9";


            var credential1 = "ce-3ff09833-2cf8-4017-bcfa-0cdc3232b3b4";
            var credential2 = "ce-ab15e029-d112-412c-96fd-1c2a79943eab";
            var credential3 = "ce-7051cdf9-2023-0202-dfea-37cfeb64fdfe";

            // there will be one dimension for time periods, and one for credentials
            myData.HasDimension = new List<string>()
            {
                dimensionTimePeriod,
                dimensionCredentials
            };

            // Blank Nodes
            // add the dimension bnodes
            referenceObjects.Add(
                new Dimension()
                {
                    Type = "qdata:Dimension",
                    // this 'point' identifier will be referenced in HasObservation.AtPoint
                    BlankNodeId =dimensionTimePeriod,
                    Name = "Time periods for wage statistics",
                    // HasPoint will contain bnode ids for two time periods (which will be qdata:DataSetTimeFrame objects)
                    HasPoint = new List<string>()
                    {
                        timeperiod1,
                        timeperiod2,
                    }

                } );
            // add dimension for credentials
            referenceObjects.Add(
                new Dimension()
                {
                    Type = "qdata:Dimension",
                    // this 'point' identifier will be referenced in HasObservation.AtPoint
                    BlankNodeId = dimensionCredentials,
                    Name = "Credentials for wage statistics",
                    // HasPoint will contain CTIDs for published resources, but could reference a bnode
                    HasPoint = new List<string>()
                    {
                        credential1,
                        credential2,
                        credential3,
                    }

                } );

            // add the two time periods
            referenceObjects.Add(
                new DataSetTimeFrame()
                {
                    Type = "qdata:DataSetTimeFrame",
                    // this must match an entry in dimenstion.HasPoint
                    BlankNodeId = timeperiod1,
                    Name = "Five years after graduation",
                    StartDate = "2014-06-30",
                    EndDate = "2019-07-01",
                    TimeInterval = "P5Y"

                } );

            referenceObjects.Add(
                new DataSetTimeFrame()
                {
                    Type = "qdata:DataSetTimeFrame",
                    // this must match an entry in dimenstion.HasPoint
                    BlankNodeId = timeperiod2,
                    Name = "Ten years after graduation",
                    StartDate = "2014-06-30",
                    EndDate = "2024-07-01",
                    TimeInterval = "P10Y"

                } );


            // HasObservation
            // with 1 metric, 3 credentials, and 2 time periods, there will be 6 observations
            // Observations are required. The observation relates to a Metric, and if dimensions are present, to points in a dimension
            //  - it is list of URIs such as the CTID for a published resource or
            //    a blank node identifier (for a bnode that will be added to the Request.ReferenceObjects property)
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
                    // A Metric is required in IsObservationOf
                    IsObservationOf = metric1,
                    AtPoint = new List<string>()
                    {
                        timeperiod1,
                        credential1,
                    },
                    Value = 45811
                } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred2Time1",
                    IsObservationOf = metric1,
                    AtPoint = new List<string>()
                    {
                        timeperiod1,
                        credential2,
                    },
                    Value = 45821
                } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred3Time1",
                    IsObservationOf = metric1,
                    AtPoint = new List<string>()
                    {
                        timeperiod1,
                        credential3,
                    },
                    Value = 45831
                } );

            // time period 2
            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred1Time2",
                    IsObservationOf = metric1,
                    AtPoint = new List<string>()
                    {
                        timeperiod1,
                        credential2,
                    },
                    Value = 45812
                } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred2Time2",
                    IsObservationOf = metric1,
                    AtPoint = new List<string>()
                    {
                        timeperiod2,
                        credential2,
                    },
                    Value = 45822
                } );

            referenceObjects.Add(
                new Observation()
                {
                    Type = "qdata:Observation",
                    BlankNodeId = "_:obsCred3Time2",
                    IsObservationOf = metric1,
                    AtPoint = new List<string>()
                    {
                        timeperiod2,
                        credential3,
                    },
                    Value = 45882
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
