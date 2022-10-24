using System;
using System.Collections.Generic;

using APIRequest = RA.Models.Input.TransferIntermediaryRequest;
using APIRequestEntity = RA.Models.Input.TransferIntermediary;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
    public class PublishTransferIntermediary
    {
		static string thisClassName = "PublishTransferIntermediary";

		#region TransferIntermediary with IntermediaryFor URIs to transfer value profiles
		/// <summary>
		/// Publish a Transfer Intermediary with a list of transfer values CTIDs in TransferIntermediary.IntermediaryFor
		/// 
		/// </summary>
		/// <returns></returns>
		public bool PublishWithIntermediaryFor()
		{
			string requestType = "publish";
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
			}
			// Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-8391465c-38e1-4a76-a729-18601cf37285";// "ce-" + Guid.NewGuid().ToString().ToLower();

			//===================================================================================
			var myData = new APIRequestEntity()
			{
				Name = "A Transfer Intermediary for ....",
				Description = "A useful description is coming soon. .",
				CodedNotation = "someCode:101",
				CTID = myCTID,
				Subject = new List<string>() { "Finance", "Accounting", "Bookkeeping" },
				SubjectWebpage = "https://example.org?t=ti22"
			};
			myData.OwnedBy.Add( new OrganizationReference()
			{
				//ACE
				CTID = "ce-9c1c2d37-e525-43a3-9cea-97f076b2fe38"// organizationIdentifierFromAccountsSite
			} );
			myData.CreditValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//add list of transferValuesProfile CTIDs (must already be published)
			myData.IntermediaryFor = new List<string>()
			{
				"ce-9ed831ea-773c-4ec4-bf4a-efca819b96aa",
				"ce-180d4be2-d22d-4795-8acb-de4c219b7c8b",
				"ce-de49e230-f92d-4377-8e25-8aea6c72776b",
				"ce-07a645c7-1f5b-4a23-9733-13d9dcf4290e"
			};
			//This holds the main entity and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				TransferIntermediary = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			// Serialize the request object
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "TransferIntermediary",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferIntermediary.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};
			var isvalid = new SampleServices().PublishRequest( req );
			if ( req.Messages.Count > 0 )
			{
				string status = string.Join( ",", req.Messages.ToArray() );
				LoggingHelper.DoTrace( 5, thisClassName + " Publish request had some error messages: " + status );
			}
			return isvalid;

		}
		#endregion



	}
}
