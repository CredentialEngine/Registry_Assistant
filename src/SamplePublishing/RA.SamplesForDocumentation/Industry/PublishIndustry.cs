using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RA.Models;
using RA.Models.Input;

using APIRequest = RA.Models.Input.IndustryRequest;

namespace RA.SamplesForDocumentation
{
    public class PublishIndustry
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

            RequestHelper helper = new RA.Models.RequestHelper();
            //create a new CTID (then save for reuse).
            var entityCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();

            //create request object.
            //This holds the resource being published and the identifier( CTID ) for the publishing organization
            var myRequest = new APIRequest()
            {
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            var myData = new Industry()
            {
                Name = "Health Industry",
                CTID = entityCTID,
                SubjectWebpage = "https://example.com?t=acmeIndustry",
                Description = "A description of this industry",
                CodedNotation = "100-1234",
            };
            myData.AssertedBy = new List<OrganizationReference>()
            {
                new OrganizationReference()
                {
                    Type="ceterms:Organization",
                    Name="ACME",
                    SubjectWebpage="https://example.com?t=acmeOrg",
                    Address = new List<Place>()
                    {
                        new Place()
                        {
                            Address1="123 Acme Way",
                            City="New York",
                            AddressRegion="New York",
                            PostalCode="11223",
                            Country="USA"
                        }
                    }
                }
            };

            List<string> alternateTypes = new List<string>();
            List<string> codes = new List<string>();
            myData.IndustryType = Industries.PopulateIndustries( ref alternateTypes, ref codes );


            //Add the Industry to the request
            myRequest.Industry = myData;

            //create a literal to hold data to use with ARC
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API
            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "Industry",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.Industry.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );

            LoggingHelper.WriteLogFile( 2, string.Format( "Industry_{0}_payload.json", myRequest.Industry.CTID ), req.FormattedPayload, "", false );

            return isValid;
        }

    }
}
