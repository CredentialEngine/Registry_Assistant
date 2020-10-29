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
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class ProPathExamples
	{
		/// <summary>
		/// Publish a ProPath credential with a holders profile
		/// Based on 160 Driving Academy <see cref="https://apps.illinoisworknet.com/cis/wioatraining/program/1006549_1007283"/>
		/// See published JSON-LD: <see cref="https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/SamplePublishing/RA.SamplesForDocumentation/Credentials/ProPath_Published.json"/>
		/// </summary>
		/// <param name="requestType"></param>
		public void CredentialWithHoldersProfile( string requestType = "format" )
		{

			//
			var apiKey = SampleServices.GetMyApiKey();
			var organizationIdentifierFromAccountsSite = "ce-a4041983-b1ae-4ad4-a43d-284a5b4b2d73";// SampleServices.GetAppKeyValue( "myOrgCTID" );
			//
			RequestHelper helper = new RA.Models.RequestHelper();
			//create a new CTID (then save for reuse.
			var hpctid = "ce-a86c6fb9-bf6b-436e-82bf-7850d1f13379";// "ce-" + Guid.NewGuid().ToString().ToLower();
			var myData = new Credential()
			{
					Name = "160 Driving Academy",
					Ctid = hpctid,
					Description = "Credential for ProPath with Holders Profile",
					SubjectWebpage = "https://apps.illinoisworknet.com/cis/wioatraining/program/1006549_1007283",
					CredentialType = "License",
					CredentialStatusType = "Active"
			};
			myData.OwnedBy = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="CredentialOrganization",
					Name="Illinois Department of Commerce and Economic Opportunity",
					SubjectWebpage="https://www2.illinois.gov/dceo/Pages/default.aspx"
				}
			};
			/*
			 * The HoldersProfile is an entity describing the count and related statistical information of holders of a given credential. 
			 * The HoldersProfile requires a CTID at this time (may change)
			 * The profile has high level statistical information plus a relevantDataset (a list for multiple) which is the DataSetProfile class 
			 * 
			 * DataSetProfile -  Particular characteristics or properties of a data set and its records.
			 * Requires a CTID.
			 * A key property is qdata:dataSetTimePeriod which a list of the class: qdata:DataSetTimeFrame
			 * 
			 * qdata:DataSetTimeFrame - Time frame including earnings and employment start and end dates of the data set.
			 * This class describes the timeframe for a set of statistics. 
			 * The property: qdata:dataAttributes is a list of the class: qdata:DataProfile. 
			 * 
			 * qdata:DataProfile - Entity describing the attributes of the data set, its subjects and their values.
			 * This class has a large number of properties for describing statitics.
			 * Three new properties are being added that are not yet shown on https://credreg.net/qdata/terms/DataProfile#DataProfile:
			 * - TotalWIOACompleters
			 * - TotalWIOAExiters
			 * - TotalWIOAParticipants
			 * 
			 * Additional properties are also expected to be added. 
			 */

			HoldersProfile hp = new HoldersProfile()
			{
				Ctid = "ce-b81e50ff-0744-47d1-bbdb-38e3fa877f20",//"ce-" + Guid.NewGuid().ToString().ToLower(),
				DateEffective = "2018-01-15",
				Description = "ProPath HoldersProfile",
			};

			//============= DataSetProfile ===================
			//referenced from a HoldersProfile (RelevantDataSet)
			var relevantDataSet = new DataSetProfile()
			{
				Name = "Dataset profile for holdersprofile",
				CTID = "ce-7a16ae76-9f1c-49f6-8742-5ab757a77f85", //"ce-" + Guid.NewGuid().ToString().ToLower(),
				Description = "ProPath DataSetProfile",
				DataProvider = myData.OwnedBy[ 0 ],
				//RelevantDataSetFor = hpctid //this will be derived by the API
			};
			//DataSetTimeFrame referenced from a DataSetProfile (DataAttributes)
			DataSetTimeFrame dstp = new DataSetTimeFrame()
			{
				Description = "ProPath DataSetTimeFrame",
				DataSourceCoverageType = new List<string>() { "Global" },
				StartDate = "2017-01-11",   
				EndDate = "2019-03-20"      
			};
			////DataProfile referenced from a DataSetTimeFrame ()
			var dataProfile = new DataProfile()
			{
				//AdministrativeRecordType = "adminRecord:Tax1099",	//??
				Description = "ProPath DataProfile",
				//IncomeDeterminationType = "ActualEarnings"	//??
			};

			//
			dataProfile.TotalWIOACompleters.Add( SampleServices.AddQuantitativeValue( 415, "All Completers" ) );
			dataProfile.HoldersInSet.Add( SampleServices.AddQuantitativeValue( 344, "Successful Completers" ) );
			dataProfile.TotalWIOAExiters.Add( SampleServices.AddQuantitativeValue( 329, "Completed Successfully and Exited WIOA" ) );
			dataProfile.TotalWIOAParticipants.Add( SampleServices.AddQuantitativeValue( 416, "Total Enrollment (Including Currently Enrolled)" ) );
			//
			var dataProfileUnrelatedEmployment = new DataProfile() { Description = "Training unrelated employment", };
			dataProfileUnrelatedEmployment.UnrelatedEmployment.Add( SampleServices.AddQuantitativeValue( 22, "Hired for a Non-Training Related Job" ) );
			//EarningsAmount
			dataProfileUnrelatedEmployment.EarningsAmount.Add( new MonetaryAmount()
			{
				Currency = "USD",
				Value = 15.91M,
				Description = "Average Wage"
			}
			);
			//
			var dataProfileRelatedEmployment = new DataProfile() { Description = "Training related employment", };
			dataProfileRelatedEmployment.RelatedEmployment.Add( SampleServices.AddQuantitativeValue( 195, "Hired for a Training-Related Job" ) );
			dataProfileRelatedEmployment.EarningsAmount.Add( new MonetaryAmount()
			{
				Currency = "USD",
				Value = 19.35M,
				Description = "Average Wage"
			}
);
			//==========================================
			//populate all
			dstp.DataAttributes.Add( dataProfile );
			dstp.DataAttributes.Add( dataProfileUnrelatedEmployment );
			dstp.DataAttributes.Add( dataProfileRelatedEmployment );

			relevantDataSet.DataSetTimePeriod.Add( dstp );
			hp.RelevantDataSet.Add( relevantDataSet );
			

			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add holders profile to the request
			myRequest.HoldersProfile.Add( hp );

			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "credential",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Credential.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "cred_{0}_payload.json", "ProPath" ), req.FormattedPayload, "", false );
		
		}
	}
}
