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
			var result = string.Empty;
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
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
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
			myData.Identifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "Some Identifer For Resource",
				IdentifierValueCode = "Catalog: xyz1234 "        //Alphanumeric string identifier of the entity
			} );
			myData.VersionIdentifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "MyVersion",
				IdentifierValueCode = "2023-09-01"        //Alphanumeric string identifier of the entity
			} );
			//								Assesses competencies
			//	Where an assessment assesses one or more competencies, they can be published in the Assesses property
			//	List<CredentialAlignmentObject> Assesses
			//  Ideally, the competencies would be part of a competency framework that could be published to the registry.
			//  If the competencies are 'free floating' they can be published just using the name and an optional description
			myData.Assesses = new List<CredentialAlignmentObject>()
            {
                new CredentialAlignmentObject()
                {
                    TargetNodeName="Identify characteristics of major food-borne pathogens, foods involved in outbreaks, and methods of"
                },
                new CredentialAlignmentObject()
                {
                    TargetNodeName="Describe food service management safety and sanitation program procedures"
                },
                new CredentialAlignmentObject()
                {
                    TargetNodeName="Identify good personal hygiene and health procedures and report symptoms of illness",
                    TargetNodeDescription = "An important description providing more details about this competency"
                }
            };
            //if the competencies are from a published framework, additional properties can be included
            myData.Assesses.Add( new CredentialAlignmentObject()
            {
                Framework = "https://credentialengineregistry.org/resources/ce-86971432-c365-4a9a-a67c-393a7d719e68",
                TargetNode = "https://credentialengineregistry.org/resources/ce-216f6de3-a749-4d9b-bc5f-dc1a14764f7d",
                FrameworkName = "WorkKeys Graphic Literacy",
                TargetNodeName = "Characteristics of Items"
            } );

            //A competency framework can contain many competencies. If an assessment assesses all competencies in a framework, the helper property: AssessesCompetencyFramework may be used for efficiency. Rather than listing 10, 50, 500 competencies, only the CTID for the competency framework needs to be provided. The API will validate the framework, then fetch all competencies in the framework and populate the Assesses property.
            //NOTE: The framework must have already been published to the credential registry. 
            myData.AssessesCompetencyFramework = new List<string>()
           {
                "ce-6fdd56d3-0214-4a67-b0c4-bb4c16ce9a13"
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

			myData.IsRequiredFor = new List<ConditionProfile>()
			{
				new ConditionProfile()
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
				new ConditionProfile()
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
            //add occupationType, IndustryType, InstructionalProgram
            List<string> alternateTypes = new List<string>();
            List<string> codes = new List<string>();
            //====================	OCCUPATIONS ====================
            myData.OccupationType = OccupationsHelper.PopulateOccupations( ref alternateTypes, ref codes );
            if (alternateTypes != null && alternateTypes.Count > 0)
                myData.AlternativeOccupationType = alternateTypes;
            if (codes != null && codes.Count > 0)
                myData.ONET_Codes = codes;
            //====================	INDUSTRIES	====================
            myData.IndustryType = Industries.PopulateIndustries( ref alternateTypes, ref codes );
            if (alternateTypes != null && alternateTypes.Count > 0)
                myData.AlternativeIndustryType = alternateTypes;
            if (codes != null && codes.Count > 0)
                myData.NaicsList = codes;
            //====================	INSTRUCTIONAL PROGRAMS	====================
            myData.InstructionalProgramType = InstructionalPrograms.PopulatePrograms( ref alternateTypes, ref codes );
            if (alternateTypes != null && alternateTypes.Count > 0)
                myData.AlternativeInstructionalProgramType = alternateTypes;
            if (codes != null && codes.Count > 0)
                myData.CIP_Codes = codes;


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
