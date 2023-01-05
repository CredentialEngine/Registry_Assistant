using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishAssessment
	{
		public string PublishSimpleRecord()
		{
			//Holds the result of the publish action
			var result = "";
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
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//DataService.SaveAssessmentCTID( myCTID );

			//A simple assessment object - see below for sample class definition
			var myData = new Assessment()
			{
				Name = "My Assessment Name",
				Description = "This is some text that describes my assessment.",
				CTID = myCTID,
				SubjectWebpage = "http://www.credreg.net/assessment/1234",
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				AssessmentMethodType = new List<string>() { "assessMethod:Exam", "assessMethod:Performance" },
				Requires = new List<ConditionProfile>()
				{
					new ConditionProfile()
					{
						Name = "My Requirements",
						Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
					}
				}
			};
			// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
			// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
			// Required
			// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
			// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
			myData.LifeCycleStatusType = "Active";

			//add one of ownedBy or offeredBy, or both
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.OfferedBy.Add( new OrganizationReference()
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
			//Add organization that is not in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );

			//
			//A assessment *must* be connected to a credential in order to be published.
			//The connection can be made using a Required condition profile in the Credential or using a RequiredFor Connection from the assessment

			myData.IsRequiredFor = new List<Connections>()
			{
				new Connections()
				{
					Description="This assessment is required for the 'Acme Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //optional, but helpful
							CTID="ce-f5d9bf2a-d930-4e77-a69b-85788943851c"
						}
					}
				},
				//if the credential is not in the registry (often where the owner is not the same as the owner of the assessment), or the publisher doesn't have the CTID, a full EntityReference can be provided. 
				new Connections()
				{
					Description="This assessment is required for the 'Third Party Credential'.",
					TargetCredential = new List<EntityReference>()
					{
						new EntityReference()
						{
							Type="Certificate", //required here
							Name="Third Party Credential",
							SubjectWebpage="https://example.com?t=thisCredential",
							Description="Description of this credential"
						}
					}
				}
			};
			//duration the assessment is exactly 2 hours. This example uses the ISO8601 format
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					ExactDuration = new DurationItem()
					{
						Duration_ISO8601="PT2H"
					}
				}
			};
			//This holds the assessment and the identifier (CTID) for the owning organization
			var myRequest = new AssessmentRequest()
			{
				Assessment = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			result = new SampleServices().SimplePost( "assessment", "publish", payload, apiKey );
			//Return the result
			return result;
		}


	}
}
