using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;
using RA.Models.Input.profiles.QData;
using APIRequestResource = RA.Models.Input.profiles.QData.DataSetProfile;
using APIRequest = RA.Models.Input.DataSetProfileRequest;

namespace RA.SamplesForDocumentation.OutcomeData
{
    public class PublishDataSetProfile
    {

        public string PublishLegacy(string requestType = "publish")
        {
            //Holds the result of the publish action
            var result = "";

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
            }//


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
                License= "http://example.com/DataSetProfile/license",
                Rights= "Information about rights held in and over this resource."
            };
            //typically the ownedBy is the same as the CTID for the data owner
            myData.DataProvider = new OrganizationReference()
            {
                CTID = organizationIdentifierFromAccountsSite
            };

            //CTID for Subject matter of the resource.
            myData.About.Add( new EntityReference()
            {
                CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
            } );
            myData.DistributionFile = new List<string>()
            {
                "https://exmple.org/distribution"
            };
            myData.Identifier.Add( new IdentifierValue()
            {
                IdentifierTypeName = "Some Identifer For Outcome Data",
                IdentifierValueCode = "Catalog: xyz1234 "       
            } );


            //	INDUSTRIES
            //obsolete
            myData.InstructionalProgramType = new List<FrameworkItem>
            {
				//Using existing frameworks such as CIP
				//programs from a framework like Classification of Instructional Program - where the information is stored locally and can be included in publishing
				new FrameworkItem()
                {
                    Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
                    FrameworkName = "Classification of Instructional Program",
                    Name = "Medieval and Renaissance Studies",
                    TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.1301",
                    CodedNotation = "30.1301",
                    Description = "A program that focuses on the  study of the Medieval and/or Renaissance periods in European and circum-Mediterranean history from the perspective of various disciplines in the humanities and social sciences, including history and archeology, as well as studies of period art and music."
                },
                new FrameworkItem()
                {
                    Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
                    FrameworkName = "Classification of Instructional Program",
                    Name = "Classical, Ancient Mediterranean and Near Eastern Studies and Archaeology",
                    TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.2202",
                    CodedNotation = "30.2202",
                    Description = "A program that focuses on the cultures, environment, and history of the ancient Near East, Europe, and the Mediterranean basin from the perspective of the humanities and social sciences, including archaeology."
                }
            };


            //DataSetTimeFrame
            //  referenced from a DataSetProfile (DataAttributes)
            DataSetTimeFrame dstp = new DataSetTimeFrame()
            {
                Name = "An optional name for the time period.",
                Description = "The information on this page is based on employment for 2018.",
                StartDate = "2020-01-01",
                EndDate = "2020-12-31",
                DataSourceCoverageType = new List<string>() { "Global" },
            };

            //DataProfile referenced from a DataSetTimeFrame ()
            var dataProfile = new DataProfile()
            {
                AdministrativeRecordType = "adminRecord:Tax1099",	
                Description = "CareerBridge DataProfile",
                IncomeDeterminationType = "ActualEarnings"	
            };

            //
            dataProfile.TotalWIOACompleters.Add( SampleServices.AddQuantitativeValue( 415, "All Completers" ) );
            dataProfile.HoldersInSet.Add( SampleServices.AddQuantitativeValue( 344, "Successful Completers" ) );
            dataProfile.TotalWIOAExiters.Add( SampleServices.AddQuantitativeValue( 329, "Completed Successfully and Exited WIOA" ) );
            dataProfile.TotalWIOAParticipants.Add( SampleServices.AddQuantitativeValue( 416, "Total Enrollment (Including Currently Enrolled)" ) );

            //Data profile for unrelated employment
            var dataProfileUnrelatedEmployment = new DataProfile() { Description = "Training unrelated employment", };
            dataProfileUnrelatedEmployment.UnrelatedEmployment.Add( SampleServices.AddQuantitativeValue( 22, "Hired for a Non-Training Related Job" ) );
            //EarningsAmount
            dataProfileUnrelatedEmployment.EarningsAmount.Add( new MonetaryAmount()
            {
                Currency = "USD",
                Value = 15.91M,
                Description = "Average Wage"
            } );

            //Data profile for related employment
            var dataProfileRelatedEmployment = new DataProfile() { Description = "Training related employment", };
            dataProfileRelatedEmployment.RelatedEmployment.Add( SampleServices.AddQuantitativeValue( 195, "Hired for a Training-Related Job" ) );
            dataProfileRelatedEmployment.EarningsAmount.Add( new MonetaryAmount()
            {
                Currency = "USD",
                Value = 19.35M,
                Description = "Average Wage"
            } );
            //==========================================
            //populate all
            dstp.DataAttributes.Add( dataProfile );
            dstp.DataAttributes.Add( dataProfileUnrelatedEmployment );
            dstp.DataAttributes.Add( dataProfileRelatedEmployment );

            myData.DataSetTimePeriod.Add( dstp );
            //====================	DataSetProfile REQUEST ====================
            //This holds the DataSetProfile and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                DataSetProfile = myData,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
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
            //Return the result
            return req.FormattedPayload;
        }

    }
}
