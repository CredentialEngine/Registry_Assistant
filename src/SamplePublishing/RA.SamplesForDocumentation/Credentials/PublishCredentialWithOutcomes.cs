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
using APIRequestResource = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class PublishCredentialWithOutcomes
	{
		/// <summary>
		/// Publish a credential with aggregate data
		/// sandbox registry: <see cref="https://sandbox.credentialengineregistry.org/graph/ce-f5b8d26a-690a-4671-bda6-d8fc92647a05"/>
		/// sandbox finder: <see cref="https://sandbox.credentialengine.org/finder/credential/ce-f5b8d26a-690a-4671-bda6-d8fc92647a05"/>
		/// <param name="requestType">Format or Publish</param>
		public bool CredentialWithOutcomeData( string requestType = "format" )
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
				Name = "My Credential With Outcome data, using: Aggregate Data Profile",
				Description = "This credential has outcomes data including all data provided for the DataProfile.",
				CTID = myCTID,
				SubjectWebpage = "https://example.com/?t=credentialwhp",
				CredentialType = "ceterms:Certification",
				CredentialStatusType = "Active",
				InLanguage = new List<string>() { "en-US" },
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="One University Plaza",
					City="Springfield",
					PostalCode="62703",
					AddressRegion="IL",
					Country="United States"
				}
			};
			//
			//format an AggregateDataProfile
			myData.AggregateData = new List<AggregateDataProfile>() { FormatAggregateDataProfile( organizationIdentifierFromAccountsSite ) };

			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-US",
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
				CTID = myRequest.Credential.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			return new SampleServices().PublishRequest( req );

		}

		private AggregateDataProfile FormatAggregateDataProfile( string owningOrganizationCTID )
		{

			/*
			 * The AggregateDataProfile is a resource containing summary statistical data..
				The AggregateDataProfile has the following relationships
			  	AggregateDataProfile
			  		DataSetProfile
			  			DataSetTimeFrame
			  				DataProfile
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
			var datasetProfileCtid = "ce-5d551a88-6916-4bfc-a271-073f2caf1930";// "ce-" + Guid.NewGuid().ToString().ToLower();

			var output = new AggregateDataProfile()
			{
				Name = "My Aggregate Data Profile for a particular outcome.",
				DateEffective = "2018-01-15",
				Description = "Description of 'My AggregateDataProfile Profile for a particular set of outcomes.'",
				DemographicInformation = "Description of Demographic Information",
				NumberAwarded = 234,
				Source = "https://example.org/?t=AggregateDataProfileProfileSource",
				JobsObtained = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{ 
						Value = 188, 
						Description = "Program graduates employed in the region."
					}
				}
			};

			output.Jurisdiction.Add( Jurisdictions.SampleJurisdiction() );
			//============= DataSetProfile ===================
			//referenced from a AggregateDataProfileProfile (RelevantDataSet)
			var relevantDataSet = new DataSetProfile()
			{
				Name = "Dataset profile for AggregateDataProfileprofile",
				AlternateName = new List<string>() { "alternate Dos", "Alternate Deux" },
				Description = "A helpful description of this dataset profile.",
				CTID = datasetProfileCtid,
				DataProvider = new OrganizationReference() { CTID = owningOrganizationCTID }
			};
			relevantDataSet.Jurisdiction = Jurisdictions.SampleJurisdictions();

			//DataSetTimeFrame referenced from a DataSetProfile (DataAttributes)
			DataSetTimeFrame dstp = new DataSetTimeFrame()
			{
				Name = "Optional name for time frame",
				AlternateName = new List<string>() {"alternate Dos", "Alternate Deux" },
				Description = "Description of a DataSetTimeFrame",
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
				EarningsDefinition= "Definition of \"earnings\" used by the data source in the context of the reporting group.",
				EmploymentDefinition= "Statement of criteria used to determine whether sufficient levels of work time and/or earnings have been met to be considered employed during the earning time period.",
				IncomeDeterminationType= "AnnualizedEarnings",
				WorkTimeThreshold= "Statement of earnings thresholds used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the chosen employment and earnings time period."

			};
			dataProfile.EarningsThreshold = "earnings threshold used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the time period of the data set";
			dataProfile.FacultyToStudentRatio = "1:21";
			#region 
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 15, "Data Available spring" ) );
			dataProfile.DataAvailable.Add( SampleServices.AddQuantitativeValue( 45, "Data Available fall" ) );
			dataProfile.DataNotAvailable.Add( SampleServices.AddQuantitativeValue( 22, "Number of credential AggregateDataProfile in the reporting group for which employment and earnings data has not been included in the data set" ) );
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
			dataProfile.SubjectExcluded.Add( SampleServices.AddQuantitativeValue( 13, "Category of subject excluded from the data." ) );
			//
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


	}
}
