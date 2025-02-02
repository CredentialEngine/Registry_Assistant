using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using APIRequestResource = RA.Models.Input.Occupation;
using APIRequest = RA.Models.Input.OccupationRequest;

namespace RA.SamplesForDocumentation.Employment
{
	public class Occupations
	{
		public void PublishOccupation( string requestType = "format" )
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

			var myData = new APIRequestResource()
			{
				Name = "Credentials Publisher",
				CTID = entityCTID,
				Description = "Credentials Publisher description",
				SubjectWebpage = "https://example.com/?t=CredentialsPublisher",
			};
			//add asserting organization
			myData.AssertedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//instructionalPrograms
			myData.OccupationType = new List<FrameworkItem>()
			{
				new FrameworkItem()
				{
					CodedNotation="53-3032", Name="Heavy and Tractor-Trailer Truck Drivers"
				}
			};

			//Classification- using a blank node to an object in ReferenceObjects
			//1. create a blank node Id
			var bnodeId = "_:" + Guid.NewGuid().ToString().ToLowerInvariant();
			//2. create the concept
			var concept = new Concept()
			{
				Id = bnodeId,
				Type = "skos:Concept",
				PrefLabel = "Equity Goal"
			};
			//add the bnodeId to Classification
			myData.Classification = new List<string>() { bnodeId };
			//add the blank node object to ReferenceObjects
			myRequest.ReferenceObjects.Add( concept );

			//Add the Occupation to the request
			myRequest.Occupation = myData;

			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "Occupation",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Occupation.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "Occupation_{0}_payload.json", myRequest.Occupation.CTID ), req.FormattedPayload, "", false );

		}

		/// <summary>
		/// Publish a list of Occupations
		/// - input can be plain Occupations or CTDL (JSON-LD) Occupations
		/// </summary>
		public void PublishOccupationList()
		{
			//there is no format option for OccupationList
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

			var myData = new List<Occupation>()
			{
				new Occupation()
				{
					Name = "Accountants",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Analyze financial information and prepare financial reports to determine or maintain record of assets, liabilities, profit and loss, tax liability, or other financial activities within an organization.",
					SubjectWebpage = "https://www.cacareerzone.org/profile/13-2011.01",
				},
				new Occupation()
				{
					Name = "Auditors",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Examine and analyze accounting records to determine financial status of establishment and prepare financial reports concerning operating procedures.",
					SubjectWebpage = "https://www.cacareerzone.org/profile/13-2011.02",
				},
				new Occupation()
				{
					Name = "Treasurers and Controllers",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Direct financial activities, such as planning, procurement, and investments for all or part of an organization.",
					SubjectWebpage = "https://www.cacareerzone.org/profile/11-3031.01",
				}

			};


			//This holds the Occupation List and the identifier (CTID) for the owning organization
			var myRequest = new OccupationListRequest()
			{
				HasLanguageMaps = false,//set to false if input is a list of 'plain' occupations (versus already format as JSON-LD)
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//list of occupations
			myRequest.OccupationList.AddRange( myData );
			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "Occupation",
				RequestType = "publishlist",        //Endpoint is publishlist. NOTE: There is no format option for OccupationList
				OrganizationApiKey = apiKey,
				Identifier = "testing",     //useful for logging, might use the organization ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "OccupationsFor_{0}_payload.json", organizationIdentifierFromAccountsSite ), req.FormattedPayload, "", false );

		}

	}
}
