﻿using System;
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
	public class CareerBridgeCredentials
	{
		/// <summary>
		/// Publish a CareerBridge credential with a holders profile
		/// Based on A+/MCP Certification Training <see cref="https://credentialfinder.org/credential/18228/A_MCP_Certification_Training"/>
		/// See published JSON-LD: <see cref="https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/SamplePublishing/RA.SamplesForDocumentation/CareerBridge/CareerBridge_Published.json"/>
		/// </summary>
		/// <param name="requestType">Format or Publish</param>
		public void CredentialWithHoldersProfile( string requestType = "format" )
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
			}//
			RequestHelper helper = new RA.Models.RequestHelper();
			//create a new CTID (then save for reuse).
			var credCtid = "ce-343120e2-c068-4630-bb39-0a09e56d5613"; ;
			var hpctid = "ce-" + Guid.NewGuid().ToString().ToLower();
			var datasetProfileCtid = "ce-" + Guid.NewGuid().ToString().ToLower();

			var myData = new Credential()
			{
				Name = "A+/MCP Certification Training",
				Ctid = credCtid,
				Description = "Structured as a shorter alternative to the computer networking systems technician program, students prepare for employment as computer systems technicians. Instruction includes A+, CCNA, the Cisco Networking Academy, MCP (Microsoft Certified Professional) Windows operating systems, and one of two electives: Fundamentals of UNIX or Career Advancement Strategies. Students are encouraged to obtain industry certifications before graduating, including Cisco Networking Academy, CompTIA A+ (two tests), Microsoft Certified Professional (one test).",
				SubjectWebpage = "https://batestech.edu/",
				CredentialType = "Certificate",
				CredentialStatusType = "Active"
			};
			//20-11-09 - Bates Technical College doesn't exist in sandbox, so using a reference org.
			myData.OwnedBy = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="CredentialOrganization",
					Name="Bates Technical College",
					Description="Bates Technical College is a Technical College located in Tacoma, WA", 
					SubjectWebpage="http://www.batestech.edu/"
				}
			};
			//where owner also offers:
			myData.OfferedBy = myData.OwnedBy;

			//duration
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Months=9
					}
				}
			};
			//learning delivery type
			myData.LearningDeliveryType = new List<string>()
			{
				"In-Person","Online Only"
			};
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="1101 S. Yakima",
					City="Tacoma",
					PostalCode="98405",
					AddressRegion="WA",
					Country="United States"
				}
			};

			//Conditions
			//NONE



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
				Ctid = hpctid,
				Description = "Consumer Report Card for A+/MCP Certification Training",
			};

			//============= DataSetProfile ===================
			//referenced from a HoldersProfile (RelevantDataSet)
			var relevantDataSet = new DataSetProfile()
			{
				CTID = datasetProfileCtid,
				Description = "Consumer Report Card for A+/MCP Certification Training",
				DataProvider = myData.OwnedBy[ 0 ],
				//RelevantDataSetFor = hpctid //this will be derived by the API
			};
			//DataSetTimeFrame referenced from a DataSetProfile (DataAttributes)
			DataSetTimeFrame dstp = new DataSetTimeFrame()
			{
				Description = "CareerBridge DataSetTimeFrame",
				DataSourceCoverageType = new List<string>() { "Global" },
			};
			////DataProfile referenced from a DataSetTimeFrame ()
			var dataProfile = new DataProfile()
			{
				//AdministrativeRecordType = "adminRecord:Tax1099",	//??
				Description = "CareerBridge DataProfile",
				//IncomeDeterminationType = "ActualEarnings"	//??
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

			LoggingHelper.WriteLogFile( 2, string.Format( "CareerBridge_red_{0}_payload.json", myRequest.Credential.Ctid ), req.FormattedPayload, "", false );

		}

	}
}