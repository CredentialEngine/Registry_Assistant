using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using RA.Models.Input.profiles.QData;

using APIRequest = RA.Models.Input.DataSetProfileRequest;

namespace RA.SamplesForDocumentation.OutcomeData
{
	public class IndianaPassRates
	{
		public void BallStateUniversity( string requestType = "format" )
		{

			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
				//		
			}
			//Indiana State Board of Nursing
			var organizationIdentifierFromAccountsSite = "ce-41b7d06d-8e99-4af8-937c-f1482c4eb565";// SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//
			RequestHelper helper = new RA.Models.RequestHelper();
			//create a new CTID (then save for reuse) - add to spreadsheet
			var datasetProfileCTID = "ce-2fb858b4-9bed-49f1-b09e-ceaf9475aa15";
			//credential: B.S. in Nursing (Pre-Licensure)
			var myData = new DataSetProfile()
			{
				CTID = datasetProfileCTID,
				Name= "Indiana BSN Programs Annual Pass Rates",
				Source = "https://www.in.gov/pla/files/BSN-Program-Pass-Rates-2010-2020.pdf",
				Description = "The National Council Licensure Examination-Registered Nurse (NCLEX-RN) is developed by the National Council of State Boards of Nursing, Inc. (NCSBN) and is used by all 50 states as a requirement for licensure.  The national pass rate on the NCLEX-RN for first-time exam takers who graduated from baccalaureate pre-licensure programs was 90.3 percent for 2020.",
 
			};
			myData.About = new List<EntityReference>()
			{
				new EntityReference()
				{
					Type="ceterms:BachelorDegree",
					CTID="ce-fa7df781-72c9-473e-9119-db9970d34320"
				}
			};
			//
			myData.DataProvider = new OrganizationReference()
			{
					Type="CredentialOrganization",
					CTID= "ce-2ecc2ce8-b134-4a3a-8b17-863aa118f36e", //BallStateUniversity
			};

			DataSetTimeFrame dstp2018 = new DataSetTimeFrame()
			{
				Description = "Pass Rates are updated annually.",
				StartDate = "January 1, 2018",//verify this format will work properly
				EndDate = "December 31, 2018"
			};
			//==================== 2018 =======================
			var dataProfile2018 = new DataProfile()
			{
				PassRate = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Percentage=93.3M,
						Value=126,
						Description="Percent and Number of Program Graduates"
					}
				},
				SubjectsInSet = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value=135
					}
				}
			};
			dstp2018.DataAttributes.Add( dataProfile2018 );

			//==================== 2019 =======================
			DataSetTimeFrame dstp2019 = new DataSetTimeFrame()
			{
				Description = "Pass Rates are updated annually.",
				StartDate = "January 1, 2019",//verify this format will work properly
				EndDate = "December 31, 2019",
				DataAttributes = new List<DataProfile>()
				{
					new DataProfile()
					{
						PassRate = new List<QuantitativeValue>()
						{
							new QuantitativeValue()
							{
								Percentage=95.6M,
								Value=131,
								Description="Percent and Number of Program Graduates"
							}
						},
						SubjectsInSet = new List<QuantitativeValue>()
						{
							new QuantitativeValue()
							{
								Value=137
							}
						}
					}
				}
			};

			//==================== 2020 =======================
			DataSetTimeFrame dstp2020 = new DataSetTimeFrame()
			{
				Description = "Pass Rates are updated annually.",
				StartDate = "January 1, 2020",//verify this format will work properly
				EndDate = "December 31, 2020",
				DataAttributes = new List<DataProfile>()
				{
					new DataProfile()
					{
						PassRate = new List<QuantitativeValue>()
						{
							new QuantitativeValue()
							{
								Percentage=91.8M,
								Value=102,
								Description="Percent and Number of Program Graduates"
							}
						},
						SubjectsInSet = new List<QuantitativeValue>()
						{
							new QuantitativeValue()
							{
								Value=111
							}
						}
					}
				}
			};


			//==========================================
			//populate all
			myData.DataSetTimePeriod.Add( dstp2018 );
			myData.DataSetTimePeriod.Add( dstp2019 );
			myData.DataSetTimePeriod.Add( dstp2020 );
			//==========================================

			//This holds the datasetprofile and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				DataSetProfile = myData,
				DefaultLanguage = "en-US",
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

		public void AndersonUniversity( string requestType = "format" )
		{

			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
				//		
			}
			//Indiana State Board of Nursing
			var organizationIdentifierFromAccountsSite = "ce-41b7d06d-8e99-4af8-937c-f1482c4eb565";// SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//
			RequestHelper helper = new RA.Models.RequestHelper();
			//create a new CTID (then save for reuse) - add to spreadsheet
			var datasetProfileCTID = "ce-cbcc672a-43c5-4e79-9eb3-9d43cbbae292";
			//credential: B.S. in Nursing (Pre-Licensure)
			var myData = new DataSetProfile()
			{
				CTID = datasetProfileCTID,
				Name = "Indiana BSN Programs Annual Pass Rates",
				Source = "https://www.in.gov/pla/files/BSN-Program-Pass-Rates-2010-2020.pdf",
				Description = "The National Council Licensure Examination-Registered Nurse (NCLEX-RN) is developed by the National Council of State Boards of Nursing, Inc. (NCSBN) and is used by all 50 states as a requirement for licensure.  The national pass rate on the NCLEX-RN for first-time exam takers who graduated from baccalaureate pre-licensure programs was 90.3 percent for 2020.",

			};
			myData.About = new List<EntityReference>()
			{
				new EntityReference()
				{
					Type="ceterms:BachelorDegree",
					Name="B.S. in Nursing (Pre-Licensure)",
					SubjectWebpage = "https://anderson.edu/academics/nursing/",
					OwnedBy = new List<OrganizationReference>()
					{
						new OrganizationReference()
						{
							Type="CredentialOrganization", 
							Name="Anderson University",
							SubjectWebpage="https://anderson.edu/",
							Description="With about 1,600 students calling Anderson University home, it can be hard to sum up the experience of life on campus. We have more than 130 ways to get involved, including 20 NCAA Division III sports, intramurals, clubs, and service opportunities. Our community gathers twice per week for Chapel, which is just one of the ways we are a safe place to grow in your faith journey. This is a place to know and be known – all while preparing for lives of faith and service to the church and society."
						}
					}
				}
			};
			//
			myData.DataProvider = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Anderson University",
				SubjectWebpage = "https://anderson.edu/",
				Description = "With about 1,600 students calling Anderson University home, it can be hard to sum up the experience of life on campus. We have more than 130 ways to get involved, including 20 NCAA Division III sports, intramurals, clubs, and service opportunities. Our community gathers twice per week for Chapel, which is just one of the ways we are a safe place to grow in your faith journey. This is a place to know and be known – all while preparing for lives of faith and service to the church and society."
			};

			DataSetTimeFrame dstp2018 = new DataSetTimeFrame()
			{
				Description = "Pass Rates are updated annually.",
				StartDate = "January 1, 2018",//verify this format will work properly
				EndDate = "December 31, 2018"
			};
			//==================== 2018 =======================
			var dataProfile2018 = new DataProfile()
			{
				PassRate = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Percentage=100M,
						Value=33,
						Description="Percent and Number of Program Graduates"
					}
				},
				SubjectsInSet = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Value=33
					}
				}
			};
			dstp2018.DataAttributes.Add( dataProfile2018 );

			//==================== 2019 =======================
			//DataSetTimeFrame dstp2019 = new DataSetTimeFrame()
			//{
			//	Description = "Pass Rates are updated annually.",
			//	StartDate = "January 1, 2019",//verify this format will work properly
			//	EndDate = "December 31, 2019",
			//	DataAttributes = new List<DataProfile>()
			//	{
			//		new DataProfile()
			//		{
			//			PassRate = new List<QuantitativeValue>()
			//			{
			//				new QuantitativeValue()
			//				{
			//					Percentage=95.6M,
			//					Value=131,
			//					Description="Percent and Number of Program Graduates"
			//				}
			//			},
			//			SubjectsInSet = new List<QuantitativeValue>()
			//			{
			//				new QuantitativeValue()
			//				{
			//					Value=137
			//				}
			//			}
			//		}
			//	}
			//};

			//==================== 2020 =======================
			//DataSetTimeFrame dstp2020 = new DataSetTimeFrame()
			//{
			//	Description = "Pass Rates are updated annually.",
			//	StartDate = "January 1, 2020",//verify this format will work properly
			//	EndDate = "December 31, 2020",
			//	DataAttributes = new List<DataProfile>()
			//	{
			//		new DataProfile()
			//		{
			//			PassRate = new List<QuantitativeValue>()
			//			{
			//				new QuantitativeValue()
			//				{
			//					Percentage=91.8M,
			//					Value=102,
			//					Description="Percent and Number of Program Graduates"
			//				}
			//			},
			//			SubjectsInSet = new List<QuantitativeValue>()
			//			{
			//				new QuantitativeValue()
			//				{
			//					Value=111
			//				}
			//			}
			//		}
			//	}
			//};


			//==========================================
			//populate all
			myData.DataSetTimePeriod.Add( dstp2018 );
			//myData.DataSetTimePeriod.Add( dstp2019 );
			//myData.DataSetTimePeriod.Add( dstp2020 );
			//==========================================

			//This holds the datasetprofile and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				DataSetProfile = myData,
				DefaultLanguage = "en-US",
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
