using Newtonsoft.Json;
using RA.Models.Input;
using System.Collections.Generic;
using APIRequest = RA.Models.Input.CredentialTypeRequest;
using APIRequestResource = RA.Models.Input.CredentialType;

namespace RA.SamplesForDocumentation
{
    public class PublishCredentialType
    {

        public string Publish( string requestType = "publish" )
        {
            //Holds the result of the publish action
            var result = string.Empty;

            // Assign the api key - acquired from organization account of the organization doing the publishing
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
            }

            //Assign a CTID for the entity being published and keep track of it
            var myCTID = "ce-76a92743-7836-43aa-9752-ada24e577582";

            // Populate the CredentialType object
            var myData = new APIRequestResource()
            {
                Name = "Higher Certificate",
                Description = "This is some text that describes my resource.",
                CTID = myCTID,
                SubjectWebpage = "http://example.com/credentialType/1234",
                CredentialStatusType = "Active",
                Keyword = new List<string>() { "Credential Type", "Technical Information", "Credential Registry" },
                Naics = new List<string>() { "333922", "333923", "333924" },
                DateEffective = "1999-09-01",
                ExpirationDate = "2049-06-15"
            };

            //typically the ownedBy is the same as the CTID for the data owner
            myData.OwnedBy.Add( new OrganizationReference()
            {
                CTID = organizationIdentifierFromAccountsSite
            } );

            /*
             * other required properties
             * RegulatedBy
             * RegulatedIn
             */

            //CTID for an org that provides regulation
            myData.RegulatedBy.Add( new OrganizationReference()
            {
                CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
            } );

            myData.RegulatedIn.Add( Jurisdictions.SampleJurisdictionAssertion() );

            // Optional

            // subclassOf a broader type or class than the one being described.
            myData.SubclassOf = "ceterms:Certificate";

            myData.ApprovedBy.Add( new OrganizationReference()
            {
                CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
            } );

            myData.ApprovedIn.Add( Jurisdictions.SampleJurisdictionAssertion() );
            myData.OfferedIn.Add( Jurisdictions.SampleJurisdictionAssertion() );

            myData.AudienceLevelType = new List<string> { "Undergraduate Level" };

            // one or more published credentialTypes (the CTID is all that is needed, or a URL if not in the credential registry) that:
            //      that covers all of the relevant concepts in the item being described
            //      as well as additional relevant concepts.
            myData.BroadAlignment = new List<string> { "ce-8e52f06b-b52a-4596-89d9-93c1daf9551a" };

            // duration for a range from 8 to 12 weeks
            myData.EstimatedDuration = new List<DurationProfile>()
            {
                new DurationProfile()
                {
                    MinimumDuration = new DurationItem()
                    {
                        Weeks=8
                    },
                    MaximumDuration = new DurationItem()
                    {
                        Weeks=12
                    }
                }
            };

            // optional identifier(s)
            myData.Identifier.Add( new IdentifierValue()
            {
                IdentifierTypeName = "Some Identifer For Resource",
                IdentifierValueCode = "Catalog: xyz1234 "        //Alphanumeric string identifier of the entity
            } );

            //InCatalog - An inventory or listing of resources that includes this resource.
            myData.InCatalog = "https://example.org/ourCatalog";

            myData.InLanguage = new List<string>() { "en-US" };


            //==================== JURISDICTION and Recognized In (specialized jurisdiction) ====================

            myData.Jurisdiction.Add( Jurisdictions.SampleJurisdiction() );
            //Add a jurisdiction assertion for Recognized in
            myData.RecognizedIn.Add( Jurisdictions.SampleJurisdictionAssertion() );

            #region  CONDITION PROFILE
            // add a requires Condition profile with conditions references to required resources like learning opportunities and credentials

            myData.Requires = new List<ConditionProfile>()
            {
                new ConditionProfile()
                {
                    Description = "To earn this credential the following conditions must be met, and the target learning opportunity must be completed.",
                    Condition = new List<string>() { "Complete High School", "Have a drivers licence." },
                    TargetLearningOpportunity = new List<EntityReference>()
                    {
						//if the target learning opportunity exists in the registry, then only the CTID has to be provided in the EntityReference
						new EntityReference()
                        {
                            CTID="ce-ccd00a32-d5ad-41e7-b14c-5c096bc9eea0"
                        },
                        new EntityReference()
                        {
							//Learning opportunities not in the registry may still be published as 'blank nodes'
							//The type, name, and subject webpage are required. The description while useful is optional.
							Type="LearningOpportunity",
                            Name="Another required learning opportunity (external)",
                            Description="A required learning opportunity that has not been published to Credential Registry. The type, name, and subject webpage are required. The description while useful is optional. ",
                            SubjectWebpage="https://example.org?t=anotherLopp",
                             CodedNotation="Learning 101"
                        }
                    }
                },
				//a condition profile that indicate the required credit hours, using the CreditValue property and a credit type of SemesterHours
				new ConditionProfile()
                {
                    Description = "To earn this credential the following conditions must be met.",
					//credit Value
					CreditValue = new List<ValueProfile>()
                    {
                        new ValueProfile()
                        {
							//CreditUnitType- The type of credit associated with the credit awarded or required.
							// - ConceptScheme: ceterms:CreditUnit (https://credreg.net/ctdl/terms/CreditUnit#CreditUnit)
							// - Concepts: provide with the namespace (creditUnit:SemesterHour) or just the text (SemesterHour). examples
							// - creditUnit:ClockHour, creditUnit:ContactHour, creditUnit:DegreeCredit
							CreditUnitType = new List<string>() {"SemesterHour"},
                            Value=10
                        }
                    }
                }
            };

            var isPreparationFor = new ConditionProfile
            {
                Description = "This certification will prepare a student for the target credential",
                TargetCredential = new List<EntityReference>()
                {
					//the referenced credential could be for an external credential, not known to be in the credential registry
					new EntityReference()
                    {
                        Type="MasterDegree",
                        Name="Cybersecurity Technology Master's Degree  ",
                        Description="A helpful description",
                        SubjectWebpage="https://example.org?t=masters"
                    }
                }
            };
            myData.IsPreparationFor.Add( isPreparationFor );


            //common conditions
            //An organization may publish common condition information such as pre-requisties using a ConditionManifest.
            //Each resource can then reference these common conditions using the CommonCondition property rather than having to repeat the information.
            //This propery is a list of CTIDs (recommended) for each published ConditionManifest or the actual credential registry URIs
            myData.CommonConditions = new List<string>()
            {
                "ce-82a854b6-1e17-4cd4-845d-0b9b6df2fb5c"
            };

            #endregion

            //====================	INDUSTRIES	====================
            PopulateIndustries( myData );

            myData.VersionCode = "Some useful code";
            myData.VersionIdentifier.Add( new IdentifierValue()
            {
                IdentifierTypeName = "CurrentVersion",
                IdentifierValueCode = "2023-09-01"        //Alphanumeric string identifier of the entity
            } );


            //====================	API REQUEST ====================
            //This holds the credential type and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                CredentialType = myData,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Serialize the request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API

            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "CredentialType",
                RequestType = requestType,
                OrganizationApiKey = apiKey,
                CTID = myRequest.CredentialType.CTID.ToLower(),   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }

        /// <summary>
        /// Possible Input Types
        /// - List of frameworks
        /// - list of industry names
        /// - List of NAICS codes
        /// </summary>
        /// <param name="request"></param>
        public static void PopulateIndustries( CredentialType request )
        {
            request.IndustryType = new List<FrameworkItem>
            {

				//occupations from a framework like NAICS - where the information is stored locally and can be included in publishing
				new FrameworkItem()
                {
                    Framework = "https://www.naics.com/",
                    FrameworkName = "NAICS - North American Industry Classification System",
                    Name = "National Security",
                    TargetNode = "https://www.naics.com/naics-code-description/?code=928110",
                    CodedNotation = "928110",
                    Description = "This industry comprises government establishments of the Armed Forces, including the National Guard, primarily engaged in national security and related activities."
                },
                new FrameworkItem()
                {
                    Framework = "https://www.naics.com/",
                    FrameworkName = "NAICS - North American Industry Classification System",
                    Name = "Regulation and Administration of Transportation Programs",
                    TargetNode = "https://www.naics.com/naics-code-description/?code=926120",
                    CodedNotation = "926120",
                    Description = "This industry comprises government establishments primarily engaged in the administration, regulation, licensing, planning, inspection, and investigation of transportation services and facilities. Included in this industry are government establishments responsible for motor vehicle and operator licensing, the Coast Guard (except the Coast Guard Academy), and parking authorities."
                }
            };


            //Industries not in a known framework, list of strings
            request.AlternativeIndustryType = new List<string>() { "Cybersecurity", "Forensic Science", "Forensic Anthropology" };

            //NAICS helper - ALternately provided a list of NAICS codes. The Assistant API will validate the codes and format the output including the framework name and URL, the name, description, and code
            request.Naics = new List<string>() { "9271", "927110", "9281", "928110" };
        }

    }
}
