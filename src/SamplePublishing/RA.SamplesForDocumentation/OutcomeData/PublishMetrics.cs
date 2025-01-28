using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using RA.Models;
using RA.Models.Input.profiles.QData;

using APIRequest = RA.Models.Input.MetricRequest;

namespace RA.SamplesForDocumentation
{
    public class PublishMetric
    {
        public bool DoPublish( string requestType = "format" )
        {

            var apiKey = SampleServices.GetMyApiKey();
            if ( string.IsNullOrWhiteSpace( apiKey ) )
            {
                //ensure you have added your apiKey to the app.config
            }
            var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
            if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
            {
                //ensure you have added your organization account CTID to the app.config
            }

            //create a new CTID (then save for reuse).
            var entityCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();

            //create request object.
            //This holds the resource being published and the identifier( CTID ) for the publishing organization
            var myRequest = new APIRequest()
            {
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            var myData = new Metric()
            {
                Name = "Median Wages",
                CTID = entityCTID,
                Description = "Median earnings are for graduates who graduated between academic years 2004-05 and 2011-12, as of their 10th year after graduation.  For example, earnings for a 2004-05 graduate were earned in 2014-15.  All earnings are in inflation-adjusted 2021 dollars.",
                IncomeDeterminationType = "incomeDetermination:ActualEarnings",
            };
            myData.Publisher = new List<string>()
            {
                "ce-0199633b-c59a-49a2-9071-e2a457ee3551"
            };

            myData.MetricType = new List<string>()
            {
                "Earnings"
            };
            // optional information
            myData.EmploymentDefinition = "Median earnings of graduates who were employed in Pennsylvania as of their 10th year after graduation.  For example, earnings for a 2004-05 graduate were earned in 2014-15.  All earnings are in inflation-adjusted to the year of the dataset.";

            //Add the Metric to the request
            myRequest.Metric = myData;

            //create a literal to hold data to use with ARC
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API
            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "Metric",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.Metric.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );

            LoggingHelper.WriteLogFile( 2, string.Format( "Metric_{0}_payload.json", myRequest.Metric.CTID ), req.FormattedPayload, "", false );

            return isValid;
        }

    }
}
