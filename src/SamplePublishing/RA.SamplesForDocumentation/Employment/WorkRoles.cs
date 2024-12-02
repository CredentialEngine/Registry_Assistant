using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using APIRequestResource = RA.Models.Input.WorkRole;
using APIRequest = RA.Models.Input.WorkRoleRequest;

namespace RA.SamplesForDocumentation.Employment
{
	public class WorkRoles
	{
		public void PublishWorkRole( string requestType = "format" )
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
				Name = "Acme WorkRole",
				CTID = entityCTID,
 			Description = "Assess problems and resources, taking a leadership role in the development, implementation and outcomes evaluation of a plan. Provides professional interventions at critical times. Position requires providing a service to one or more age groups from young adult upwards. ...",
				CodedNotation = "100-1234",
			};

			//


			//Add the WorkRole to the request
			myRequest.WorkRole = myData;

			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "WorkRole",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.WorkRole.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "WorkRole_{0}_payload.json", myRequest.WorkRole.CTID ), req.FormattedPayload, "", false );

		}

		/// <summary>
		/// Publish a list of WorkRoles
		/// - input can be plain WorkRoles or CTDL (JSON-LD) WorkRoles
		/// </summary>
		public void PublishWorkRoleList()
		{
			//there is no format option for WorkRoleList
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

			var myData = new List<WorkRole>()
			{
				new WorkRole()
				{
					Name = "Accountant Work Roles",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Analyze financial information and prepare financial reports to determine or maintain record of assets, liabilities, profit and loss, tax liability, or other financial activities within an organization.",
					Comment = new List<string>() { "Some comment for this WorkRole" }
				},
				new WorkRole()
				{
					Name = "Auditor Work Roles",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Examine and analyze accounting records to determine financial status of establishment and prepare financial reports concerning operating procedures.",
					Comment = new List<string>() { "Some comment for this WorkRole" }
				},
				new WorkRole()
				{
					Name = "Treasurers and Controllers Work Roles",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Direct financial activities, such as planning, procurement, and investments for all or part of an organization."
				}

			};


			//This holds the WorkRole List and the identifier (CTID) for the owning organization
			var myRequest = new WorkRoleListRequest()
			{
				HasLanguageMaps = false,//set to false if input is a list of 'plain' WorkRoles (versus already format as JSON-LD)
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//list of WorkRoles
			myRequest.WorkRoleList.AddRange( myData );
			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "WorkRole",
				RequestType = "publishlist",        //Endpoint is publishlist. NOTE: There is no format option for WorkRoleList
				OrganizationApiKey = apiKey,
				Identifier = "testing",     //useful for logging, might use the organization ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "WorkRolesFor_{0}_payload.json", organizationIdentifierFromAccountsSite ), req.FormattedPayload, "", false );

		}

	}
}
