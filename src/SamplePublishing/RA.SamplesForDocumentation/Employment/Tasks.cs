using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using APIRequestEntity = RA.Models.Input.Task;
using APIRequest = RA.Models.Input.TaskRequest;

namespace RA.SamplesForDocumentation.Employment
{
	public class Tasks
	{
		public void PublishTask( string requestType = "format" )
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

			var myData = new Task()
			{
				Name = "Acme Task",
				CTID = entityCTID,
				Description = "Assess problems and resources, taking a leadership role in the development, implementation and outcomes evaluation of a plan. Provides professional interventions at critical times. Position requires providing a service to one or more age groups from young adult upwards. ...",
				CodedNotation = "100-1234",
			};

			//


			//This holds the Task and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Task = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "Task",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Task.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "Task_{0}_payload.json", myRequest.Task.CTID ), req.FormattedPayload, "", false );

		}

		/// <summary>
		/// Publish a list of Tasks
		/// - input can be plain Tasks or CTDL (JSON-LD) Tasks
		/// </summary>
		public void PublishTaskList()
		{
			//there is no format option for TaskList
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

			var myData = new List<Task>()
			{
				new Task()
				{
					Name = "Accountant Tasks",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Analyze financial information and prepare financial reports to determine or maintain record of assets, liabilities, profit and loss, tax liability, or other financial activities within an organization.",
					Comment = new List<string>() { "Some comment for this Task" }
				},
				new Task()
				{
					Name = "Auditor Tasks",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Examine and analyze accounting records to determine financial status of establishment and prepare financial reports concerning operating procedures.",
					Comment = new List<string>() { "Some comment for this Task" }
				},
				new Task()
				{
					Name = "Treasurers and Controllers Tasks",
					CTID = "ce-" + Guid.NewGuid().ToString().ToLower(),
					Description = "Direct financial activities, such as planning, procurement, and investments for all or part of an organization."
				}

			};


			//This holds the Task List and the identifier (CTID) for the owning organization
			var myRequest = new TaskListRequest()
			{
				HasLanguageMaps = false,//set to false if input is a list of 'plain' Tasks (versus already format as JSON-LD)
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//list of Tasks
			myRequest.TaskList.AddRange( myData );
			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "Task",
				RequestType = "publishlist",        //Endpoint is publishlist. NOTE: There is no format option for TaskList
				OrganizationApiKey = apiKey,
				Identifier = "testing",     //useful for logging, might use the organization ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			LoggingHelper.WriteLogFile( 2, string.Format( "TasksFor_{0}_payload.json", organizationIdentifierFromAccountsSite ), req.FormattedPayload, "", false );

		}

	}
}
