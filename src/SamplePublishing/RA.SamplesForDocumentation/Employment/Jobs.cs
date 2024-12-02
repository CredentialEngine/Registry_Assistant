using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using APIRequestResource = RA.Models.Input.Job;
using APIRequest = RA.Models.Input.JobRequest;

namespace RA.SamplesForDocumentation.Employment
{
	public class Jobs
	{
		public void PublishJob( string requestType = "format" )
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
			var entityCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();

			//create request object.
			//This holds the resource being published and the identifier( CTID ) for the publishing organization
			var myRequest = new APIRequest()
			{
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			var myData = new Job()
			{
				Name = "Acme Job",
				CTID = entityCTID,
				SubjectWebpage = "https://example.com?t=acmeJob",
				Description = "Assess problems and resources, taking a leadership role in the development, implementation and outcomes evaluation of a plan. Provides professional interventions at critical times. Position requires providing a service to one or more age groups from young adult upwards. ...",
				CodedNotation = "100-1234",
			};
			myData.OfferedBy = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="ceterms:Organization",
					Name="ACME",
					SubjectWebpage="https://example.com?t=acmeOrg",
					Address = new List<Place>()
					{
						new Place()
						{
							Address1="123 Acme Way",
							City="New York",
							AddressRegion="New York", 
							PostalCode="11223",
							Country="USA"
						}
					}
				}
			};
					
			//
			myData.Requires = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Description="Acme Job requires License to Practice",
					TargetCredential= new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="license",
							Name="License to practise",
							SubjectWebpage="https://example.net?t=licenseToPractice",
							RecognizedIn= new List<JurisdictionAssertion>()
							{
								new JurisdictionAssertion()
								{
									MainJurisdiction = new Place()
									{
										AddressRegion="New York",
										Country = "USA"
									}
								}
							}
						}
					}
				}
			};


			//Add the Job to the request
			myRequest.Job = myData;

			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "Job",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Job.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "Job_{0}_payload.json", myRequest.Job.CTID ), req.FormattedPayload, "", false );

		}

		/// <summary>
		/// Publish a list of Jobs
		/// - input can be plain Jobs or CTDL (JSON-LD) Jobs
		/// </summary>
		public void PublishJobList()
		{
			//there is no format option for JobList
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

			var myData = new List<Job>()
			{
				new Job()
				{
					Name = "Accountants",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Analyze financial information and prepare financial reports to determine or maintain record of assets, liabilities, profit and loss, tax liability, or other financial activities within an organization.",
					SubjectWebpage = "https://www.cacareerzone.org/profile/13-2011.01",
				},
				new Job()
				{
					Name = "Auditors",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Examine and analyze accounting records to determine financial status of establishment and prepare financial reports concerning operating procedures.",
					SubjectWebpage = "https://www.cacareerzone.org/profile/13-2011.02",
				},
				new Job()
				{
					Name = "Treasurers and Controllers",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Direct financial activities, such as planning, procurement, and investments for all or part of an organization.",
					SubjectWebpage = "https://www.cacareerzone.org/profile/11-3031.01",
				}

			};


			//This holds the Job List and the identifier (CTID) for the owning organization
			var myRequest = new JobListRequest()
			{
				HasLanguageMaps = false,//set to false if input is a list of 'plain' Jobs (versus already format as JSON-LD)
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//list of Jobs
			myRequest.JobList.AddRange( myData );
			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "Job",
				RequestType = "publishlist",        //Endpoint is publishlist. NOTE: There is no format option for JobList
				OrganizationApiKey = apiKey,
				Identifier = "testing",     //useful for logging, might use the organization ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "JobsFor_{0}_payload.json", organizationIdentifierFromAccountsSite ), req.FormattedPayload, "", false );

		}

	}
}
