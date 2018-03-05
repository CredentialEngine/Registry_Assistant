using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Samples
{
    public class PublishCredential
    {
        public string PublishSimpleRecord()
        {
            //Holds the result of the publish action
            var result = "";
            //assign the api key - acquired from organization account of the organization doing the publishing
            var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
            //this is the CTID of the organization that owns the data being published
            var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

            //Assign a CTID for the entity being published and keep track of it
            var myCredCTID = "ce-" + Guid.NewGuid().ToString();
            //DataService.SaveCredentialCTID( myCredCTID );

            //A simple credential object - see below for sample class definition
            var myCred = new SampleCredential()
            {
                Name = "My Credential Name",
                Description = "This is some text that describes my credential.",
                CTID = myCredCTID,
                SubjectWebpage = "http://www.credreg.net/credential/1234",
                Type = "ceterms:Certificate",
                Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
                IndustryType = new List<string>() { "333922", "333923", "333924" },
                Requires = new List<ConditionProfile>()
                {
                    new ConditionProfile()
                    {
                        Name = "My Requirements",
                        Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
                    }
                }
            };

            //This holds the credential and the identifier (CTID) for the owning organization
            var myData = new CredentialRequest()
            {
                Credential = myCred,
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Serialize the credential request object
            var json = JsonConvert.SerializeObject( myData );

            //Use HttpClient to perform the publish
            using ( var client = new HttpClient() )
            {
                //Accept JSON
                client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
                //add API Key (for a publish request)
                client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );

                //Format the json as content
                var content = new StringContent( json, Encoding.UTF8, "application/json" );

                //The endpoint to publish to
                var publishEndpoint = "https://credentialengine.org/sandbox/credential/publish/";

                //Perform the actual publish action and store the result
                result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
            }

            //Return the result
            return result;
        }

        public class CredentialRequest
        {
            public SampleCredential Credential { get; set; }
            public string PublishForOrganizationIdentifier { get; set; }
        }

        public class SampleCredential
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string SubjectWebpage { get; set; }
            public string CTID { get; set; }
            public string Type { get; set; }
            public string DateEffective { get; set; }
            public List<string> Keyword { get; set; }
            public List<string> AudienceLevelType { get; set; }
            public List<string> IndustryType { get; set; }
            public List<string> OccupationType { get; set; }
            public List<ConditionProfile> Requires { get; set; }
            public List<ConditionProfile> Recommends { get; set; }
            //Other properties
        }

        public class ConditionProfile
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<string> Condition { get; set; }
            //Othe properties
        }
    }
}
