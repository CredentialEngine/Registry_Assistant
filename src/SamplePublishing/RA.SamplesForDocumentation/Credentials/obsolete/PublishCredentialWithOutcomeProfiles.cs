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
using APIRequestEntity = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class PublishCredentialWithOutcomeProfilesObsolete
	{
		[Obsolete]
		public bool CredentialWithEmploymentOutcomeProfiles( string requestType = "format" )
		{

			// Assign the api key - acquired from organization account of the organization doing the publishing
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
			 //Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-f5b8d26a-690a-4671-bda6-d8fc92647a05";// "ce-" + Guid.NewGuid().ToString();

			var myData = new Credential()
			{
				Name = "My Credential With Outcome data, including: Holders Profile, EarningsProfile, and EmploymentOutcomeProfile",
				Description = "This credential has a Holders Profile, EarningsProfile, and EmploymentOutcomeProfile, with all data provided for the DataProfile.",
				Ctid = myCTID,
				SubjectWebpage = "https://example.com/?t=credentialwhp",
				CredentialType = "ceterms:Certification",
				InLanguage = new List<string>() { "en-US" },
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//add holders profile to the request
			myData.Holders.Add( FormatHoldersProfile( organizationIdentifierFromAccountsSite ) );
			myData.Earnings.Add( FormatEarningsProfile( organizationIdentifierFromAccountsSite ) );
			myData.EmploymentOutcome.Add( FormatEmploymentOutcomeProfile( organizationIdentifierFromAccountsSite ) );


			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};


			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			//var result = new SampleServices().SimplePost( "credential", requestType, payload, apiKey );

			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "credential",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Credential.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			return new SampleServices().PublishRequest( req );

		}

		[Obsolete]
		private HoldersProfile FormatHoldersProfile( string owningOrganizationCTID )
		{

			/*
			 * The HoldersProfile is an entity describing the count and related statistical information of holders of a given credential.
				An EarningsProfile is currently only published with a Credential   
				The EarningsProfile has the following relationships
			  	EarningsProfile
			  		DataSetProfile
			  			DataSetTimeFrame
			  				DataProfile
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
			var hpctid = "ce-27915af9-fa02-41dc-8dee-7c79d34ee913";//"ce-" + Guid.NewGuid().ToString().ToLower();
			var datasetProfileCtid = "ce-5d551a88-6916-4bfc-a271-073f2caf1930";// "ce-" + Guid.NewGuid().ToString().ToLower();

			HoldersProfile output = new HoldersProfile()
			{
				CTID = hpctid,
				Name = "My Holders Profile for a particular outcome.",
				DateEffective = "2018-01-15",
				Description = "Description of 'My Holders Profile for a particular outcome.'",
				DemographicInformation = "Description of Demographic Information",
				NumberAwarded = 234,
				Source = "https://example.org/?t=holdersProfileSource"
			};

			output.Jurisdiction.Add( Jurisdictions.SampleJurisdiction() );
			//============= DataSetProfile ===================
			//referenced from a HoldersProfile (RelevantDataSet)
			var relevantDataSet = new DataSetProfile()
			{
				Name = "Dataset profile for holdersprofile",
				Description = "A helpful description of this dataset profile.",
				CTID = datasetProfileCtid,
				DataProvider = new OrganizationReference() { CTID = owningOrganizationCTID },
				//RelevantDataSetFor = hpctid //this will be derived by the API
			};

			//DataSetTimeFrame referenced from a DataSetProfile (DataAttributes)
			DataSetTimeFrame dstp = new DataSetTimeFrame()
			{
				Description = "Holders Profile DataSetTimeFrame",
				DataSourceCoverageType = new List<string>() { "Global" },
				StartDate = "2017-01-11",
				EndDate = "2019-03-20"
			};
			////DataProfile referenced from a DataSetTimeFrame ()
			var dataProfile = new DataProfile()
			{
				AdministrativeRecordType = "adminRecord:Tax1099",
				Description = "Data Profile with all properties",
				Adjustment = "Describes whether and how the provided earnings have been adjusted for factors such as inflation, participant demographics and economic conditions. ",
				EarningsDefinition = "Definition of \"earnings\" used by the data source in the context of the reporting group.",
				EmploymentDefinition = "Statement of criteria used to determine whether sufficient levels of work time and/or earnings have been met to be considered employed during the earning time period.",
				IncomeDeterminationType = "AnnualizedEarnings",
				WorkTimeThreshold = "Statement of earnings thresholds used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the chosen employment and earnings time period."

			};
			#region 
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 15, "Data Available spring" ) );
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 45, "Data Available fall" ) );
			dataProfile.DataNotAvailable.Add( SampleServices.AddQuantitativeValue( 22, "Number of credential holders in the reporting group for which employment and earnings data has not been included in the data set" ) );
			dataProfile.DemographicEarningsRate.Add( SampleServices.AddQuantitativeValue( 29.12M, "Earnings rate for a demographic category." ) );
			dataProfile.DemographicEmploymentRate.Add( SampleServices.AddQuantitativePercentage( 8.4M, "Employment rate for a demographic category." ) );
			//
			dataProfile.EarningsDistribution.Add( new MonetaryAmountDistribution()
			{
				Currency = "USD",
				Median = 52000,
				Percentile10 = 29000,
				Percentile25 = 36000,
				Percentile75 = 58000,
				Percentile90 = 67000
			} );
			//
			dataProfile.EmploymentOutlook.Add( SampleServices.AddQuantitativeValue( 333, "Projected employment estimate." ) );
			dataProfile.EmploymentRate.Add( SampleServices.AddQuantitativeValue( 45.4M, "Rate computed by dividing the number of holders or subjects meeting the data set's criteria of employment (meetEmploymentCriteria) by the number of holders or subjects for which data was available (dataAvailable)." ) );
			dataProfile.IndustryRate.Add( SampleServices.AddQuantitativeValue( 12.4M, "Employment rate for an industry category." ) );
			dataProfile.InsufficientEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 555, "Number of holders that do not meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set." ) );
			dataProfile.MeetEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 555, "Number of holders that meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set." ) );
			dataProfile.NonCompleters.Add( SampleServices.AddQuantitativeValue( 33, "Non-holders who departed or are likely to depart higher education prematurely." ) );
			dataProfile.NonHoldersInSet.Add( SampleServices.AddQuantitativeValue( 44, "Non-holder subject actively pursuing the credential through a program or assessment." ) );
			//
			dataProfile.OccupationRate.Add( SampleServices.AddQuantitativeValue( 12M, "Employment rate for an occupation category." ) );
			dataProfile.RegionalEarningsDistribution.Add( SampleServices.AddQuantitativeValue( 44000, "Reference to an entity describing median earnings as well as earnings at various percentiles for holders or subjects in the region." ) );
			dataProfile.RegionalEmploymentRate.Add( SampleServices.AddQuantitativeValue( 44000, "Reference to an entity describing median earnings as well as earnings at various percentiles for holders or subjects in the region." ) );
			dataProfile.RelatedEmployment.Add( SampleServices.AddQuantitativeValue( 321, "Number of people employed in the area of work (e.g., industry, occupation) in which the credential provided preparation." ) );
			dataProfile.SubjectsInSet.Add( SampleServices.AddQuantitativeValue( 235, "Total credential holders and non-holders in the final data collection and reporting." ) );
			dataProfile.SufficientEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 55, "Number of holders that meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set" ) );
			dataProfile.UnrelatedEmployment.Add( SampleServices.AddQuantitativeValue( 55, "Number of people employed outside the area of work (e.g., industry, occupation) in which the credential provided preparation." ) );


			//subjects
			//21-01-26 - removed SubjectIncluded and SubjectExcluded from QData.DataProfile
			//dataProfile.SubjectIncluded.Add( new SubjectProfile()
			//{
			//	Name = "Subject Included Name",
			//	Description = "Subject Included description",
			//	SubjectType = new List<string>() { "CredentialHolder", "CredentialSeeker" }, 
			//	SubjectValue = new List<QuantitativeValue>() { new QuantitativeValue()
			//		{
			//			Value=22, Description="some description"
			//		} 
			//	}
			//} );
			//dataProfile.SubjectExcluded.Add( new SubjectProfile()
			//{
			//	Name = "Subject Excluded Name",
			//	Description = "Subject Excluded description",
			//	SubjectType = new List<string>() { "Enrollee", "InsufficientDataAvailable" },
			//	SubjectValue = new List<QuantitativeValue>() { new QuantitativeValue()
			//		{
			//			Value=22, Description="some description"
			//		}
			//	}
			//} );


			//
			dataProfile.TotalWIOACompleters.Add( SampleServices.AddQuantitativeValue( 415, "All Completers" ) );
			dataProfile.HoldersInSet.Add( SampleServices.AddQuantitativeValue( 344, "Successful Completers" ) );
			dataProfile.TotalWIOAExiters.Add( SampleServices.AddQuantitativeValue( 329, "Completed Successfully and Exited WIOA" ) );
			dataProfile.TotalWIOAParticipants.Add( SampleServices.AddQuantitativeValue( 416, "Total Enrollment (Including Currently Enrolled)" ) );

			//fill out


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
			output.RelevantDataSet.Add( relevantDataSet );
			#endregion

			//
			return output;
		}
		[Obsolete]
		private EarningsProfile FormatEarningsProfile( string owningOrganizationCTID )
		{

			/*
			   The EarningsProfile is an entity that describes earning and related statistical information for a given credential.
			   An EarningsProfile is currently only published with a Credential
			   The EarningsProfile uses the same pattern of relationships as a HolderProfile
			  	EarningsProfile
			  		DataSetProfile
			  			DataSetTimeFrame
			  				DataProfile
			 * The EarningsProfile requires a CTID at this time (may change)
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
			var ctid = "ce-27915af9-4101-41dc-8dee-7c79d34ee913";//"ce-" + Guid.NewGuid().ToString().ToLower();
			var datasetProfileCtid = "ce-5d551a88-4101-4bfc-a271-073f2caf1930";// "ce-" + Guid.NewGuid().ToString().ToLower();

			var output = new EarningsProfile()
			{
				CTID = ctid,
				Name = "My Earnings Profile for a particular outcome.",
				DateEffective = "2021-02-02",
				Description = "Description of 'My Earnings Profile for a particular outcome.'",
				LowEarnings = 33000,
				MedianEarnings = 43000,
				HighEarnings = 57000,
				PostReceiptMonths = 36,
				Source = "https://example.org/?t=EarningsProfileSource"
			};
			output.Jurisdiction.Add( Jurisdictions.SampleJurisdiction() );


			//============= DataSetProfile ===================
			//referenced from a EarningsProfile (RelevantDataSet)
			var relevantDataSet = new DataSetProfile()
			{
				Name = "Dataset profile for Earnings Profile",
				Description = "A helpful description of this dataset profile.",
				CTID = datasetProfileCtid,
				DataProvider = new OrganizationReference() { CTID = owningOrganizationCTID },
				//RelevantDataSetFor = hpctid //this will be derived by the API
			};

			//DataSetTimeFrame referenced from a DataSetProfile (DataAttributes)
			DataSetTimeFrame dstp = new DataSetTimeFrame()
			{
				Description = "Earnings Profile DataSetTimeFrame",
				DataSourceCoverageType = new List<string>() { "Global" },
				StartDate = "2017-01-11",
				EndDate = "2019-03-20"
			};
			////DataProfile referenced from a DataSetTimeFrame ()
			var dataProfile = new DataProfile()
			{
				AdministrativeRecordType = "adminRecord:Tax1099",
				Description = "Data Profile with all properties",
				Adjustment = "Describes whether and how the provided earnings have been adjusted for factors such as inflation, participant demographics and economic conditions. ",
				EarningsDefinition = "Definition of \"earnings\" used by the data source in the context of the reporting group.",
				EmploymentDefinition = "Statement of criteria used to determine whether sufficient levels of work time and/or earnings have been met to be considered employed during the earning time period.",
				IncomeDeterminationType = "AnnualizedEarnings",
				WorkTimeThreshold = "Statement of earnings thresholds used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the chosen employment and earnings time period."

			};
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 15, "Data Available spring" ) );
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 45, "Data Available fall" ) );
			dataProfile.DataNotAvailable.Add( SampleServices.AddQuantitativeValue( 22, "Number of credential Earnings in the reporting group for which employment and earnings data has not been included in the data set" ) );
			dataProfile.DemographicEarningsRate.Add( SampleServices.AddQuantitativeValue( 29.12M, "Earnings rate for a demographic category." ) );
			dataProfile.DemographicEmploymentRate.Add( SampleServices.AddQuantitativePercentage( 8.4M, "Employment rate for a demographic category." ) );
			//
			dataProfile.EarningsDistribution.Add( new MonetaryAmountDistribution()
			{
				Currency = "USD",
				Median = 52000,
				Percentile10 = 29000,
				Percentile25 = 36000,
				Percentile75 = 58000,
				Percentile90 = 67000
			} );
			//
			dataProfile.EmploymentOutlook.Add( SampleServices.AddQuantitativeValue( 333, "Projected employment estimate." ) );
			dataProfile.EmploymentRate.Add( SampleServices.AddQuantitativeValue( 45.4M, "Rate computed by dividing the number of Earnings or subjects meeting the data set's criteria of employment (meetEmploymentCriteria) by the number of Earnings or subjects for which data was available (dataAvailable)." ) );
			dataProfile.IndustryRate.Add( SampleServices.AddQuantitativeValue( 12.4M, "Employment rate for an industry category." ) );
			dataProfile.InsufficientEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 555, "Number of Earnings that do not meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set." ) );
			dataProfile.MeetEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 555, "Number of Earnings that meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set." ) );
			dataProfile.NonCompleters.Add( SampleServices.AddQuantitativeValue( 33, "Non-Earnings who departed or are likely to depart higher education prematurely." ) );
			dataProfile.NonHoldersInSet.Add( SampleServices.AddQuantitativeValue( 44, "Non-holder subject actively pursuing the credential through a program or assessment." ) );
			//
			dataProfile.OccupationRate.Add( SampleServices.AddQuantitativeValue( 12M, "Employment rate for an occupation category." ) );
			dataProfile.RegionalEarningsDistribution.Add( SampleServices.AddQuantitativeValue( 44000, "Reference to an entity describing median earnings as well as earnings at various percentiles for Earnings or subjects in the region." ) );
			dataProfile.RegionalEmploymentRate.Add( SampleServices.AddQuantitativeValue( 44000, "Reference to an entity describing median earnings as well as earnings at various percentiles for Earnings or subjects in the region." ) );
			dataProfile.RelatedEmployment.Add( SampleServices.AddQuantitativeValue( 321, "Number of people employed in the area of work (e.g., industry, occupation) in which the credential provided preparation." ) );
			dataProfile.SubjectsInSet.Add( SampleServices.AddQuantitativeValue( 235, "Total credential Earnings and non-Earnings in the final data collection and reporting." ) );
			dataProfile.SufficientEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 55, "Number of Earnings that meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set" ) );
			dataProfile.UnrelatedEmployment.Add( SampleServices.AddQuantitativeValue( 55, "Number of people employed outside the area of work (e.g., industry, occupation) in which the credential provided preparation." ) );

			//
			dataProfile.TotalWIOACompleters.Add( SampleServices.AddQuantitativeValue( 415, "All Completers" ) );
			dataProfile.HoldersInSet.Add( SampleServices.AddQuantitativeValue( 344, "Successful Completers" ) );
			dataProfile.TotalWIOAExiters.Add( SampleServices.AddQuantitativeValue( 329, "Completed Successfully and Exited WIOA" ) );
			dataProfile.TotalWIOAParticipants.Add( SampleServices.AddQuantitativeValue( 416, "Total Enrollment (Including Currently Enrolled)" ) );

			//fill out


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
			output.RelevantDataSet.Add( relevantDataSet );


			//
			return output;
		}
		[Obsolete]
		private EmploymentOutcomeProfile FormatEmploymentOutcomeProfile( string owningOrganizationCTID )
		{

			/*
			   The EmploymentOutcomeProfile is an entity that describes employment outcomes and related statistical information for a given credential.
			   An EmploymentOutcomeProfile is currently only published with a Credential
			   The EmploymentOutcomeProfile uses the same pattern of relationships as a HolderProfile
			  	EmploymentOutcomeProfile
			  		DataSetProfile
			  			DataSetTimeFrame
			  				DataProfile
			 * The EmploymentOutcomeProfile requires a CTID at this time (may change)
			 * The profile has high level statistical information plus a relevantDataset (a list for multiple) which is the DataSetProfile class 
			 * 
			 * DataSetProfile -  Particular characteristics or properties of a data set and its records.
			 * Requires a CTID.
			 * A key property is qdata:dataSetTimePeriod which a list of the class: qdata:DataSetTimeFrame
			 * 
			 * qdata:DataSetTimeFrame - Time frame including EmploymentOutcome and employment start and end dates of the data set.
			 * This class describes the timeframe for a set of statistics. 
			 * The property: qdata:dataAttributes is a list of the class: qdata:DataProfile. 
			 * 
			 * qdata:DataProfile - Entity describing the attributes of the data set, its subjects and their values.
			 * This class has a large number of properties for describing statitics.
			 */
			var ctid = "ce-27915af9-5555-41dc-8dee-7c79d34ee913";//"ce-" + Guid.NewGuid().ToString().ToLower();
			var datasetProfileCtid = "ce-5d551a88-5555-4bfc-a271-073f2caf1930";// "ce-" + Guid.NewGuid().ToString().ToLower();

			var output = new EmploymentOutcomeProfile()
			{
				CTID = ctid,
				Name = "My EmploymentOutcome Profile for a particular outcome.",
				DateEffective = "2018-01-15",
				Description = "Description of 'My EmploymentOutcome Profile for a particular outcome.'",
				Source = "https://example.org/?t=EmploymentOutcomeProfileSource"
			};
			output.JobsObtainedList = new List<QuantitativeValue>()
			{
				new QuantitativeValue()
				{
					Value=88,
					Description="Relevent information about this value for JobsObtained. "
				}
			};
			output.Jurisdiction.Add( Jurisdictions.SampleJurisdiction() );


			//============= DataSetProfile ===================
			//referenced from a EmploymentOutcomeProfile (RelevantDataSet)
			var relevantDataSet = new DataSetProfile()
			{
				Name = "Dataset profile for EmploymentOutcomeProfile",
				Description = "A helpful description of this dataset profile.",
				CTID = datasetProfileCtid,
				DataProvider = new OrganizationReference() { CTID = owningOrganizationCTID },
				//RelevantDataSetFor = hpctid //this will be derived by the API
			};

			//DataSetTimeFrame referenced from a DataSetProfile (DataAttributes)
			DataSetTimeFrame dstp = new DataSetTimeFrame()
			{
				Description = "EmploymentOutcome Profile DataSetTimeFrame",
				DataSourceCoverageType = new List<string>() { "Global" },
				StartDate = "2017-01-11",
				EndDate = "2019-03-20"
			};
			////DataProfile referenced from a DataSetTimeFrame ()
			var dataProfile = new DataProfile()
			{
				AdministrativeRecordType = "adminRecord:Tax1099",
				Description = "Data Profile with all properties",
				Adjustment = "Describes whether and how the provided EmploymentOutcome have been adjusted for factors such as inflation, participant demographics and economic conditions. ",
				EarningsDefinition = "Definition of \"EmploymentOutcome\" used by the data source in the context of the reporting group.",
				EmploymentDefinition = "Statement of criteria used to determine whether sufficient levels of work time and/or EmploymentOutcome have been met to be considered employed during the earning time period.",
				IncomeDeterminationType = "AnnualizedEarnings",
				WorkTimeThreshold = "Statement of EmploymentOutcome thresholds used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the chosen employment and EmploymentOutcome time period."

			};
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 15, "Data Available spring" ) );
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 45, "Data Available fall" ) );
			dataProfile.DataNotAvailable.Add( SampleServices.AddQuantitativeValue( 22, "Number of credential holders in the reporting group for which employment and EmploymentOutcome data has not been included in the data set" ) );
			dataProfile.DemographicEarningsRate.Add( SampleServices.AddQuantitativeValue( 29.12M, "EmploymentOutcome rate for a demographic category." ) );
			dataProfile.DemographicEmploymentRate.Add( SampleServices.AddQuantitativePercentage( 8.4M, "Employment rate for a demographic category." ) );
			//
			dataProfile.EarningsDistribution.Add( new MonetaryAmountDistribution()
			{
				Currency = "USD",
				Median = 52000,
				Percentile10 = 29000,
				Percentile25 = 36000,
				Percentile75 = 58000,
				Percentile90 = 67000
			} );
			//
			dataProfile.EmploymentOutlook.Add( SampleServices.AddQuantitativeValue( 333, "Projected employment estimate." ) );
			dataProfile.EmploymentRate.Add( SampleServices.AddQuantitativeValue( 45.4M, "Rate computed by dividing the number of holders or subjects meeting the data set's criteria of employment (meetEmploymentCriteria) by the number of holders or subjects for which data was available (dataAvailable)." ) );
			dataProfile.IndustryRate.Add( SampleServices.AddQuantitativeValue( 12.4M, "Employment rate for an industry category." ) );
			dataProfile.InsufficientEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 555, "Number of holders that do not meet the prescribed employment threshold in terms of EmploymentOutcome or time engaged in work as defined for the data set." ) );
			dataProfile.MeetEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 555, "Number of holders that meet the prescribed employment threshold in terms of EmploymentOutcome or time engaged in work as defined for the data set." ) );
			dataProfile.NonCompleters.Add( SampleServices.AddQuantitativeValue( 33, "Non-holders who departed or are likely to depart higher education prematurely." ) );
			dataProfile.NonHoldersInSet.Add( SampleServices.AddQuantitativeValue( 44, "Non-holder subject actively pursuing the credential through a program or assessment." ) );
			//
			dataProfile.OccupationRate.Add( SampleServices.AddQuantitativeValue( 12M, "Employment rate for an occupation category." ) );
			dataProfile.RegionalEarningsDistribution.Add( SampleServices.AddQuantitativeValue( 44000, "Reference to an entity describing median EmploymentOutcome as well as EmploymentOutcome at various percentiles for holders or subjects in the region." ) );
			dataProfile.RegionalEmploymentRate.Add( SampleServices.AddQuantitativeValue( 44000, "Reference to an entity describing median EmploymentOutcome as well as EmploymentOutcome at various percentiles for holders or subjects in the region." ) );
			dataProfile.RelatedEmployment.Add( SampleServices.AddQuantitativeValue( 321, "Number of people employed in the area of work (e.g., industry, occupation) in which the credential provided preparation." ) );
			dataProfile.SubjectsInSet.Add( SampleServices.AddQuantitativeValue( 235, "Total credential holders and non-holders in the final data collection and reporting." ) );
			dataProfile.SufficientEmploymentCriteria.Add( SampleServices.AddQuantitativeValue( 55, "Number of holders that meet the prescribed employment threshold in terms of EmploymentOutcome or time engaged in work as defined for the data set" ) );
			dataProfile.UnrelatedEmployment.Add( SampleServices.AddQuantitativeValue( 55, "Number of people employed outside the area of work (e.g., industry, occupation) in which the credential provided preparation." ) );

			//
			dataProfile.TotalWIOACompleters.Add( SampleServices.AddQuantitativeValue( 415, "All Completers" ) );
			dataProfile.HoldersInSet.Add( SampleServices.AddQuantitativeValue( 344, "Successful Completers" ) );
			dataProfile.TotalWIOAExiters.Add( SampleServices.AddQuantitativeValue( 329, "Completed Successfully and Exited WIOA" ) );
			dataProfile.TotalWIOAParticipants.Add( SampleServices.AddQuantitativeValue( 416, "Total Enrollment (Including Currently Enrolled)" ) );

			//fill out


			//Data profile for unrelated employment
			var dataProfileUnrelatedEmployment = new DataProfile() { Description = "Training unrelated employment", };
			dataProfileUnrelatedEmployment.UnrelatedEmployment.Add( SampleServices.AddQuantitativeValue( 22, "Hired for a Non-Training Related Job" ) );
			//EmploymentOutcomeAmount
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
			output.RelevantDataSet.Add( relevantDataSet );


			//
			return output;
		}
	}
}
