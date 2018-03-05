using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Samples
{
    public class PublishOrganization
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
            var myOrgCTID = "ce-" + Guid.NewGuid().ToString();
            //DataService.SaveOrganizationCTID( myOrgCTID );

            //A simple organization object - see below for sample class definition
            var myOrg = new SampleOrganization()
            {
                Name = "My Organization Name",
                Description = "This is some text that describes my organization.",
                CTID = myOrgCTID,
                SubjectWebpage = "http://www.credreg.net",
                Type = "ceterms:CredentialOrganization",
                Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
                AgentSectorType = "agentSector:PrivateNonProfit"
                
            };
            myOrg.Email.Add( "info@credreg.net" );
            //This holds the organization and the identifier (CTID) for the owning organization
            var myData = new OrganizationRequest()
            {
                Organization = myOrg,
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Serialize the organization request object
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
                var publishEndpoint = "https://credentialengine.org/sandbox/organization/publish/";

                //Perform the actual publish action and store the result
                result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
            }

            //Return the result
            return result;
        }

        public class OrganizationRequest
        {
            public SampleOrganization Organization { get; set; }
            public string PublishForOrganizationIdentifier { get; set; }
        }

        public class SampleOrganization
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string SubjectWebpage { get; set; }
            public string CTID { get; set; }
            public string Image { get; set; }
            public string AgentPurpose { get; set; }
            public string AgentPurposeDescription { get; set; }
            public List<string> AgentType { get; set; }
            public string AgentSectorType { get; set; }
            public List<string> Address { get; set; }
            public List<string> Email { get; set; }
            public List<string> SocialMedia { get; set; }
            public List<string> Keyword { get; set; }
            public List<string> ServiceType { get; set; }
            public List<string> AccreditedBy { get; set; }
            public List<string> ApprovedBy { get; set; }
            public List<string> RecognizedBy { get; set; }
            public List<string> RegulatedBy { get; set; }
            public List<string> ParentOrganization { get; set; }

        }
    }
}
