using Newtonsoft.Json;

using RA.Models.Input;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.AlignmentMapRequest;
using APIRequestResource = RA.Models.Input.AlignmentMap;

namespace RA.SamplesForDocumentation.AlignmentMaps
{
    public class PublishingAlignmentMaps
    {
        public string Publish( string requestType = "publish" )
        {
            //Holds the result of the publish action
            var result = string.Empty;
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
            var myCTID = "ce-95517789-e925-4f40-ae32-8cc3007ada94";

            //Populate the Alignment Map object
            var myData = new APIRequestResource()
            {
                Name = "My Alignment Map Name",
                Description = "This is some text that describes my Alignment Map.",
                CTID = myCTID,
                SubjectWebpage = "https://example.org/?t=QFramework",

            };

            // organization reference(s) for the resource owner
            myData.OwnedBy.Add( new OrganizationReference()
            {
                CTID = organizationIdentifierFromAccountsSite
            } );

            // organization reference(s) for the resource creator
            myData.Creator.Add( new OrganizationReference()
            {
                CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
            } );

            // organization reference(s) for the resource publisher
            myData.Publisher.Add( new OrganizationReference()
            {
                CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
            } );

            //Add organization that is NOT in the credential registry
            myData.CopyrightHolder.Add( new OrganizationReference()
            {
                Type = "QACredentialOrganization",
                Name = "Name of the organization that holds the copyright for this resource",
                SubjectWebpage = "https://example.org/SubjectWebpageUrl",
                Description = "Description of the organization that holds the copyright for this resource."
            } );


            myData.CodedNotation = "qf-12345";
            myData.DateEffective = "2020-01-01";
            myData.ExpirationDate = "2030-12-31";
            myData.DateCopyrighted = "2021";
            myData.DateCreated = "2015-01-01"; ;
            myData.DateModified = "2024-05-16"; ;
            myData.DateValidFrom = "2020-01-01"; ;
            myData.DateValidUntil = "2030-01-01"; ;


            myData.Identifier.Add( new IdentifierValue()
            {
                IdentifierTypeName = "Some Identifer For this framework",
                IdentifierValueCode = "Catalog: xyz1234 "        //Alphanumeric string identifier of the entity
            } );

            myData.Keyword = new List<string>() { "Credentials", "Qualifications" };

            myData.License = "https://example.org/licenseUrl";

            myData.LifeCycleStatusType = "Active";

            myData.LatestVersion = "https://example.org/LatestVersionUrl";
            myData.PreviousVersion = "https://example.org/PreviousVersionUrl";
            myData.NextVersion = "https://example.org/NextVersionUrl";
            myData.SupersededBy = "https://example.org/SupersededByUrl";
            myData.Supersedes = "https://example.org/SupersedesUrl";

            myData.ProcessStandards = "https://example.org/ProcessStandardsUrl";
            myData.ProcessStandardsDescription = "Description of the process standards related to this resource.";

            myData.PublisherName = new List<string>() { "Name of the publisher" };
            myData.PublicationStatusType ="Active";
            myData.Rights = "Description of rights related to this resource.";

            myData.Source = new List<string>() { "Name of the publisher" };

            // Human-readable information resource other than a competency framework from which this resource was generated or derived by humans or machines.
            myData.SourceDocumentation = new List<string>() { "https://example.org/SourceDocumentation" };
            myData.Subject = new List<string>() { "Credentials", "Qualifications" };

            myData.VersionIdentifier.Add( new IdentifierValue()
            {
                IdentifierTypeName = "MyVersion",
                IdentifierValueCode = "2023-09-01"        //Alphanumeric string identifier of the entity
            } );
            myData.VersionCode = "abc:123";

            //This holds the Alignment Map and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                AlignmentMap = myData,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API
            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "AlignmentMap",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.AlignmentMap.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }


    }
}
