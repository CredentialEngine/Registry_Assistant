using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using RA.Models.Input.profiles.QData;
using YourCredential = RA.SamplesForDocumentation.SampleModels.Credential;
using APIRequestCredential = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.DataSetProfileRequest;


namespace RA.SamplesForDocumentation.PerformanceExamples
{
	public class WIOAExamples
	{
		public void Example1( string requestType = "format" )
		{

			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
				//		
			}
			//Coffeyville Community College
			var organizationIdentifierFromAccountsSite = "ce-3382bacb-2f29-4037-874e-9a53e2398661";// SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//
			RequestHelper helper = new RA.Models.RequestHelper();
			//create a new CTID (then save for reuse) - add to spreadsheet
			var datasetProfileCTID = "ce-7a4977bd-9245-46b4-a235-6af3b8ab5a17";// "ce-" + Guid.NewGuid().ToString().ToLower(); 

			var myData = new DataSetProfile()
			{
				CTID = datasetProfileCTID,
				Name = "Sample employment outcomes for review purposes",
				Source = "https://example.org?t=dspSample",
				Description = "Sample employment outcomes for review purposes",
			};
			//credential:Nursing Aide Certificate - Mapped to NSC Framework
			myData.About = new List<EntityReference>()
			{
				new EntityReference()
				{
					Type="ceterms:Certificate",
					CTID="ce-9f88702f-b63b-478f-a495-2dc7f1ada0bf"
				}
			};
			//
			myData.DataProvider = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				CTID = organizationIdentifierFromAccountsSite	//NOTE change if different from publisher
			};

			DataSetTimeFrame dstp2018 = new DataSetTimeFrame()
			{
				Description = "Nursing Aide Certificate timeframe",
				StartDate = "1990-01-02",//
				EndDate = "2021-03-24",
				DataSourceCoverageType = new List<string>() { "Global"}
			};
			//==================== 2018 =======================
			var dataProfile1 = new DataProfile()
			{
				Description = "Data Profile for credential",
				HoldersInSet = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value = 7,
						Description = "Successful Completers"
					}
				},
				TotalWIOAExiters = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value = 6,
						Description = "Completed Successfully and Exited WIOA"
					}
				},
				TotalWIOACompleters = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value = 7,
						Description = "All Completers"
					}
				},
				TotalWIOAParticipants = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value=8,
						Description="Total Enrollment (Including Currently Enrolled)"
					}
				}
			};
			dstp2018.DataAttributes.Add( dataProfile1 );
			//
			var dataProfile2 = new DataProfile()
			{
				Description = "Training unrelated employment",
				UnrelatedEmployment = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value = 1, //TODO - ensure a value of zero can be published
						Description = "Hired for a Non-Training Related Job"
					}
				}
			};
			dstp2018.DataAttributes.Add( dataProfile2 );
			//
			var dataProfile3 = new DataProfile()
			{
				Description = "Training related employment",
				RelatedEmployment = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value = 3, 
						Description = "Hired for a Training-Related Job"
					}
				},
				EarningsAmount = new List<MonetaryAmount>()
				{
					new MonetaryAmount()
					{
						Value=11,
						Currency="USD",
						Description="Average Wage per Hour"
					}
				}
			};
			dstp2018.DataAttributes.Add( dataProfile3 );


			//==========================================
			//populate all
			myData.DataSetTimePeriod.Add( dstp2018 );
			//==========================================

			//This holds the datasetprofile and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				DataSetProfile = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};


			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "datasetprofile",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.DataSetProfile.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			//LoggingHelper.WriteLogFile( 2, string.Format( "coffeyfille_{0}_payload.json", myRequest.Credential.CTID ), req.FormattedPayload, "", false );

		}
	}
}
