using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using RA.Models.Input.profiles.QData;

using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class ProPathExamples
	{
		/// <summary>
		/// Publish a ProPath credential with a holders profile
		/// Based on 160 Driving Academy <see cref="https://apps.illinoisworknet.com/cis/wioatraining/program/1006549_1007283"/>
		/// See published JSON-LD: <see cref="https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/SamplePublishing/RA.SamplesForDocumentation/ProPath/ProPath_Published.json"/>
		/// </summary>
		/// <param name="requestType">Format or Publish</param>
		public void CredentialWithHoldersProfile( string requestType = "format" )
		{

			var apiKey = SampleServices.GetMyApiKey();
			if (string.IsNullOrWhiteSpace( apiKey ) )
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
			var credCtid = "ce-8c0497aa-c1bf-42ba-ade6-ca0234d0b35f";// "ce-" + Guid.NewGuid().ToString().ToLower();
			var hpctid = "ce-80f07b38-0dc0-4886-8252-ed2cab8cedb6";// "ce-" + Guid.NewGuid().ToString().ToLower();
			var datasetProfileCtid = "ce-d9f17339-7839-4b3c-8893-e624428d5dfb";// "ce-" + Guid.NewGuid().ToString().ToLower();

			var myData = new Credential()
			{
				Name = "160 Driving Academy",
				Ctid = credCtid,
				Description = "Truck Driver Training School - CDL",
				SubjectWebpage = "https://160drivingacademy.com/",
				CredentialType = "License",
				CredentialStatusType = "Active"
			};
			//20-10-29 updated to use the CTID for the organization in the sandbox
			myData.OwnedBy = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="CredentialOrganization",
					CTID="ce-3593828b-a3ce-43e6-bedf-66e5865e524a"
				}
			};
			//where owner also offers:
			myData.OfferedBy = myData.OwnedBy;
			//QA
			myData.ApprovedBy = AddApprovedBy();
			//instructionalPrograms
			myData.InstructionalProgramType = new List<FrameworkItem>()
			{
				new FrameworkItem()
				{
					CodedNotation="49-0205", Name="Truck and Bus Driver/Commercial Vehicle Operator and Instructor"
				}
			};
			//instructionalPrograms
			myData.OccupationType = new List<FrameworkItem>()
			{
				new FrameworkItem()
				{
					CodedNotation="53-3032", Name="Heavy and Tractor-Trailer Truck Drivers"
				}
			};

			//add a require learning opportunity, referencing the CTID only. 
			//the credential is published first and then the learning opportunity.
			myData.Requires = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Description="The 160 Driving Academy program is required for this credential",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-f8885b28-134d-4188-8c86-063aadff14ea"
						}
					}
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
				CTID = hpctid,
				DateEffective = "2018-01-15",
				Description = "ProPath HoldersProfile",
			};

			//============= DataSetProfile ===================
			//referenced from a HoldersProfile (RelevantDataSet)
			var relevantDataSet = new DataSetProfile()
			{
				Name = "Dataset profile for holdersprofile",
				CTID = datasetProfileCtid,
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

			LoggingHelper.WriteLogFile( 2, string.Format( "ProPath_red_{0}_payload.json", myRequest.Credential.Ctid ), req.FormattedPayload, "", false );

		}

		/// <summary>
		/// Publish a ProPath credential with a holders profile
		/// Based on 160 Driving Academy <see cref="https://apps.illinoisworknet.com/cis/wioatraining/program/1006549_1007283"/>
		/// See published JSON-LD: <see cref="https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/SamplePublishing/RA.SamplesForDocumentation/ProPath/LearningOpportunity_160%20Driving%20Academy_published.json"/>
		/// </summary>
		/// <param name="requestType">Format or Publish</param>
		public void RelatedLearningOpportunity( string requestType = "format" )
		{
			//assign the api key - acquired from organization account of the organization doing the publishing
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
			var myLoppCTID = "ce-f8885b28-134d-4188-8c86-063aadff14ea";// "ce-" + Guid.NewGuid().ToString().ToLower();
			//typically would have been stored prior to retrieving for publishing

			//Populate the learning opportunity object
			var myData = new LearningOpportunity()
			{
				Name = "160 Driving Academy",
				Description = "Truck Driver Training School - CDL",
				Ctid = myLoppCTID,
				SubjectWebpage = "https://160drivingacademy.com/",
				LearningMethodType = new List<string>() { "learnMethod:Applied" },
				DeliveryType = new List<string>() { "InPerson","OnlineOnly"}
			};

			//20-10-29 updated to use the CTID for the organization in the sandbox
			myData.OwnedBy = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="CredentialOrganization",
					CTID="ce-3593828b-a3ce-43e6-bedf-66e5865e524a"
				}
			};
			//where owner also offers:
			myData.OfferedBy = myData.OwnedBy;
			//QA
			myData.ApprovedBy = AddApprovedBy();
			//instructionalPrograms
			myData.InstructionalProgramType = new List<FrameworkItem>()
			{
				new FrameworkItem()
				{
					CodedNotation="49-0205", Name="Truck and Bus Driver/Commercial Vehicle Operator and Instructor"
				}
			};
			//instructionalPrograms
			myData.OccupationType = new List<FrameworkItem>()
			{
				new FrameworkItem()
				{
					CodedNotation="53-3032", Name="Heavy and Tractor-Trailer Truck Drivers"
				}
			};
			//estimated duration. 4 weeks, or 160 hours
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Weeks=4
					},
				},
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Hours=160
					}
				}
			};
			//cost 
			myData.EstimatedCost = new List<CostProfile>()
			{
				new CostProfile()
				{
					Description="Estimated cost for this program",
					CostDetails="https://160drivingacademy.com/",
					Currency="USD",
					CostItems = new List<CostProfileItem>()
					{
						new CostProfileItem()
						{
							DirectCostType="Tuition",
							Price=4695.00M
						}
					}
				}
			};
			myData.FinancialAssistance = new List<FinancialAssistanceProfile>()
			{
				new FinancialAssistanceProfile()
				{
					FinancialAssistanceType = new List<string>() { "financialAid:PrivateLoan", "financialAid:WIOA" } //at this time financialAid:WIOA is not in production, so using grant
				}
			};//
			myData.EntryCondition = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Description="In-Person interview. Should bring drivers license and 3 year history",
					Condition = new List<string>() { "Drug/Alchohol Screening", "Other - DOT Physical Exam" }
				}
			};
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="5270 North Belt West",
					City="Belleville", AddressRegion = "Illinois", PostalCode="62226", Country="USA",
					ContactPoint = new List<ContactPoint>()
					{
						new ContactPoint()
						{
							PhoneNumbers = new List<string>() { "(309) 266-1601" }
						}
					}
				},
				new Place()
				{
					Address1="320 S Green Bay Rd",
					City="Waukegan", AddressRegion = "Illinois", PostalCode="60085", Country="USA",
					ContactPoint = new List<ContactPoint>()
					{
						new ContactPoint()
						{
							PhoneNumbers = new List<string>() { "(309) 266-1601" }
						}
					}
				},
				new Place()
				{
					Address1="2935 E Clear Lake Avenue, Suite 2B",
					City="Springfield", AddressRegion = "Illinois", PostalCode="62702", Country="USA",
					ContactPoint = new List<ContactPoint>()
					{
						new ContactPoint()
						{
							PhoneNumbers = new List<string>() { "(309) 266-1601" }
						}
					}
				}
			};

			//This holds the learning and the identifier (CTID) for the owning organization
			var myRequest = new LearningOpportunityRequest()
			{
				LearningOpportunity = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "LearningOpportunity",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.LearningOpportunity.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			LoggingHelper.WriteLogFile( 2, string.Format( "ProPath_lopp_{0}_payload.json", myRequest.LearningOpportunity.Ctid ), req.FormattedPayload, "", false );


		}
		public List<OrganizationReference> AddApprovedBy()
		{
			List<OrganizationReference> output = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="QACredentialOrganization",
					Name="Illinois Community College Board",
					SubjectWebpage="https://www.iccb.org/iccb/"
				},
				new OrganizationReference()
				{
					Type="QACredentialOrganization",
					Name="Illinois Secretary of State",
					SubjectWebpage="https://www.cyberdriveillinois.com/"
				},
				new OrganizationReference()
				{
					Type="QACredentialOrganization",
					Name="Industry Standard: Illinois Secretary of State Licensed Facility",
					SubjectWebpage="https://www.cyberdriveillinois.com/facilities/facilitylist.html"
				}
			};

			return output;
		}

	}
}
