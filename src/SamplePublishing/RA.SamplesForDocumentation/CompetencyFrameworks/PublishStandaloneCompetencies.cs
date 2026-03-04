using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation.CompetencyFrameworks
{
    public class PublishStandaloneCompetencies
    {
        public string PublishListOfCompetencies()
        {
            //Holds the result of the publish action
            var result = string.Empty;
            //assign the api key - acquired from organization account of the organization doing the publishing
            var apiKey = SampleServices.GetMyApiKey();
            if ( string.IsNullOrWhiteSpace( apiKey ) )
            {
                //ensure you have added your apiKey to the app.config
            }
            // This is the CTID of the organization that owns the data being published
            var organizationCTIDFromAccountsSite = SampleServices.GetMyOrganizationCTID();


            //This holds the data and the identifier (CTID) for the owning organization
            var myRequest = new CompetencyListRequest()
            {
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationCTIDFromAccountsSite
            };

            //add competencies
            // Creator is required
            // using the organizationCTIDFromAccountsSite
            myRequest.Competencies.Add( MapCompetency( organizationCTIDFromAccountsSite, "Looks both ways before crossing street" ) );
            myRequest.Competencies.Add( MapCompetency( organizationCTIDFromAccountsSite, "Looks before leaping" ) );
            myRequest.Competencies.Add( MapCompetency( organizationCTIDFromAccountsSite, "Deals with the faults of others as gently as their own" ) );
            myRequest.Competencies.Add( MapCompetency( organizationCTIDFromAccountsSite, "Knows what he/she knows and does not know what he/she does not know " ) );

            //Serialize the request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API

            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "competency",
                RequestType = "publish",
                OrganizationApiKey = apiKey,
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }

        private static Competency MapCompetency( string creatorCtid, string competency )
        {
            Competency output = new Competency()
            {
                CompetencyText = competency,
                CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
                Creator = new List<string>() { creatorCtid },
                Author = "X. Ample",
                DateCreated = System.DateTime.Now.ToString("yyyy-MM-dd"),
            };

            //add keywords
            //output.conceptKeyword_maplist = new LanguageMapList( new List<string>() { "concept 1", "concept 2", "concept 3" } );
            //output.conceptKeyword_maplist.Add( "fr", new List<string>() { "le concept un", "la concept deux", "les concept thois" } );
            return output;
        }

    }
}
