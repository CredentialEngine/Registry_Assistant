﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using APIRequestEntity = RA.Models.Input.Occupation;
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
			var entityCTID = "ce-" + Guid.NewGuid().ToString().ToLower();

			var myData = new Occupation()
			{
				Name = "Credentials Publisher",
				CTID = entityCTID,
				Description = "Credentials Publisher description",
				SubjectWebpage = "https://example.com/?t=CredentialsPublisher",

			};

			//instructionalPrograms
			myData.OccupationType = new List<FrameworkItem>()
			{
				new FrameworkItem()
				{
					CodedNotation="53-3032", Name="Heavy and Tractor-Trailer Truck Drivers"
				}
			};


			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Occupation = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

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

	}
}