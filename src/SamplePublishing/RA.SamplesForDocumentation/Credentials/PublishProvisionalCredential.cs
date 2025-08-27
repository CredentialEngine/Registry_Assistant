using Newtonsoft.Json;

using APIRequest = RA.Models.Input.Provisional.CredentialRequest;
using APIRequestResource = RA.Models.Input.Provisional.Credential;


namespace RA.SamplesForDocumentation
{
    public class ProvisionalCredential
    {
        /// <summary>
        /// Example of the minimum data that can be published for a provisional credential
        /// </summary>
        /// <returns></returns>
        public string PublishAbsoluteMinimum()
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
            var myCTID = "ce-082c7bb4-5b86-4542-9055-968b654a063b";

            //A simple credential object - see below for sample class definition
            //For a complete list of all credential types, see:
            //	https://credreg.net/page/typeslist#ceterms_Credential
            var myData = new APIRequestResource()
            {
                Name = "My Provisional Certification Name",
                CTID = myCTID,
                CredentialType = "ceterms:Certification",
            };

            // must have at least one of ownedBy or offeredBy
            // NOTE: blank nodes are not allowed, so this will be one or more CTIDs
            myData.OwnedBy.Add( organizationIdentifierFromAccountsSite );


            //====================	CREDENTIAL REQUEST ====================
            //This holds the credential and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                Credential = myData,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Serialize the credential request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API

            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "credential",
                RequestType = "provisional",
                OrganizationApiKey = apiKey,
                CTID = myRequest.Credential.CTID.ToLowerInvariant(),   //added here for logging
                InputPayload = payload
            };

            new SampleServices().PublishRequest( req );

            //Return the result
            return req.FormattedPayload;
        }

        /// <summary>
        /// Publish provisional credential with all allowed data
        /// </summary>
        /// <returns></returns>
        public string PublishMaxForProvisionalResource()
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
            var myCTID = "ce-2b19648c-fefe-4f21-94d6-7e747220febb";

            //A simple credential object - see below for sample class definition
            //For a complete list of all credential types, see:
            //	https://credreg.net/page/typeslist#ceterms_Credential
            var myData = new APIRequestResource()
            {
                Name = "My Provisional Certification Name",
                Description = "This is some text that describes my credential.",
                CTID = myCTID,
                SubjectWebpage = "http://example.com/credential/1234",
                CredentialType = "ceterms:Certification",
            };

            // must have at least one of ownedBy or offeredBy
            // NOTE: blank nodes are not allowed, so this will be one or more CTIDs
            myData.OwnedBy.Add( organizationIdentifierFromAccountsSite );
            myData.OfferedBy.Add( organizationIdentifierFromAccountsSite );

            //====================	CREDENTIAL REQUEST ====================
            //This holds the credential and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                Credential = myData,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Serialize the credential request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API

            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "credential",
                RequestType = "provisional",
                OrganizationApiKey = apiKey,
                CTID = myRequest.Credential.CTID.ToLowerInvariant(),   //added here for logging
                InputPayload = payload
            };

            new SampleServices().PublishRequest( req );

            //Return the result
            return req.FormattedPayload;
        }

    }
}
