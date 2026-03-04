using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models.Input;

using APIRequest = RA.Models.Input.GraphContentRequest;

namespace RA.SamplesForDocumentation.Credentials
{
    public class GraphPublishing
    {
        /// <summary>
        /// Publish a credential using the graph endpoint.
        /// Generally speaking it is now recommended to publish using a JSON-LD formatted graph:
        /// - all Uris must be formatted correctly rather than letting the Assitant Api perform this task
        /// - Properties that are CredentialAlignmentObjects have to be fully formatted 
        ///     including the credreg.net Uris for the targetNode and framework
        /// - OccupationType, IndustryType, and InstructionalProgramType have to be properly formatted 
        ///     rather using the helper properties to provide a list of codes and have the Assistant Api handle validating, populating and formatting.
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        public string PublishDetailedRecord( string requestType = "publish" )
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
            }//


            //Assign a CTID for the entity being published and keep track of it
            var myCTID = "ce-2dc6873f-1e94-4146-bb2b-fcebeed10c08";

            //A simple credential object - see below for sample class definition
            //For a complete list of all credential types, see:
            //	https://credreg.net/page/typeslist#ceterms_Credential
            GraphInput graphInput = new GraphInput()
            {
                Context = "https://credreg.net/ctdl/schema/context/json",
                Id = "https://sandbox.credentialengineregistry.org/graph/ce-2dc6873f-1e94-4146-bb2b-fcebeed10c08"
            };

            // See the friendlier version of the following Json at:
            //see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/SamplePublishing/RA.SamplesForDocumentation/Credentials/files/CredentialGraphSampleInput.json

            graphInput.Graph = @"
            {
                ""PublishForOrganizationIdentifier"": ""ce-a588a18b-2616-4c74-bdcd-6507c0693d0e"",
	            ""DefaultLanguage"": ""en - US"",
	            ""GraphInput"": 
                    {
                        ""@context"": ""https://credreg.net/ctdl/schema/context/json"",
                        ""@id"": ""https://sandbox.credentialengineregistry.org/graph/ce-2dc6873f-1e94-4146-bb2b-fcebeed10c08"",
                        ""@graph"": [
            
                        {
                        ""@id"": ""https://sandbox.credentialengineregistry.org/resources/ce-2dc6873f-1e94-4146-bb2b-fcebeed10c08"",
                        ""@type"": ""ceterms: AssociateDegree"",
				        ""ceterms: ctid"": ""ce-2dc6873f-1e94-4146-bb2b-fcebeed10c08"",
				        ""ceterms: name"": {
                                    ""en - US"": ""Sample Credential In Graph Format""
                        },
				        ""ceterms: ownedBy"": [
                            ""https://sandbox.credentialengineregistry.org/resources/ce-a588a18b-2616-4c74-bdcd-6507c0693d0e""
				        ],
				        ""ceterms: inLanguage"": [
                            ""en-US""
                        ],
				        ""ceterms: description"": {
                                    ""en - US"": ""Description of credential.""
                        },
				        ""ceterms: credentialStatusType"": {
                                    ""@type"": ""ceterms: CredentialAlignmentObject"",
					        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/CredentialStatus"",
                                    ""ceterms: targetNode"": ""credentialStat: Active"",
					        ""ceterms: frameworkName"": {
                                        ""en - US"": ""CredentialStatus""
                            },
					        ""ceterms: targetNodeName"": {
                                        ""en - US"": ""Active""
                            }
                        },
				        ""ceterms: recognizedBy"": [
                            ""https://sandbox.credentialengineregistry.org/resources/ce-e4c8e9bd-88bd-4704-9e3a-78faebdca59d""
				        ],
				        ""ceterms: estimatedCost"": [
                            {
                                ""@type"": ""ceterms: CostProfile"",
						        ""ceterms: name"": {
                                        ""en - US"": ""Total Annual Cost( estimated )""
                                },
						        ""ceterms: price"": 10594.0,
						        ""ceterms: currency"": ""USD"",
						        ""ceterms: costDetails"": ""http://www.hutchcc.edu"",
                                ""ceterms: description"": {
                                        ""en - US"": ""Total annual cost refers to the typical amount paid for tuition, fees, room, board, books, and supplies in a given academic year by recent Kansas graduates of this program of study.""    
                                },
						        ""ceterms: residencyType"": [
                                    {
                                        ""@type"": ""ceterms: CredentialAlignmentObject"",
								        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/Residency"",
                                        ""ceterms: targetNode"": ""residency: InState"",
								        ""ceterms: frameworkName"": {
                                            ""en - US"": ""Residency""
                                        },
								        ""ceterms: targetNodeName"": {
                                            ""en - US"": ""In - State""
                                        }
                                    }
						        ],
						        ""ceterms: directCostType"": 
                                {
                                    ""@type"": ""ceterms: CredentialAlignmentObject"",
							        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/CostType"",
                                    ""ceterms: targetNode"": ""costType: AggregateCost"",
							        ""ceterms: frameworkName"": {
                                            ""en - US"": ""CostType""
                                    },
							        ""ceterms: targetNodeName"": {
                                            ""en - US"": ""Aggregate Cost""
                                    }
                                }
                            },
					        {
                                    ""@type"": ""ceterms: CostProfile"",
						        ""ceterms: name"": {
                                        ""en - US"": ""Resident Tuition""
                                },
						        ""ceterms: price"": 2643.0,
						        ""ceterms: currency"": ""USD"",
						        ""ceterms: costDetails"": ""http://www.hutchcc.edu"",
                                    ""ceterms: description"": {
                                        ""en - US"": ""The typical dollar amount charged as in-state tuition for this program of study.""    
                                },
						        ""ceterms: residencyType"": [

                                    {
                                        ""@type"": ""ceterms: CredentialAlignmentObject"",
								        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/Residency"",
                                        ""ceterms: targetNode"": ""residency: InState"",
								        ""ceterms: frameworkName"": {
                                            ""en - US"": ""Residency""
                                        },
								        ""ceterms: targetNodeName"": {
                                            ""en - US"": ""In - State""
                                        }
                                    }
						        ],
						        ""ceterms: directCostType"": {
                                        ""@type"": ""ceterms: CredentialAlignmentObject"",
							        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/CostType"",
                                        ""ceterms: targetNode"": ""costType: Tuition"",
							        ""ceterms: frameworkName"": {
                                            ""en - US"": ""CostType""
                                    },
							        ""ceterms: targetNodeName"": {
                                            ""en - US"": ""Tuition""
                                    }
                                    }
                                }
				        ],
				        ""ceterms: occupationType"": [
                            {
                                ""@type"": ""ceterms: CredentialAlignmentObject"",
						        ""ceterms: framework"": ""https://www.onetcenter.org/taxonomy.html"",
                                ""ceterms: targetNode"": ""https://www.onetonline.org/link/summary/51-4121.00"",
                                ""ceterms: codedNotation"": ""51 - 4121.00"",
						        ""ceterms: frameworkName"": {
                                        ""en - US"": ""Standard Occupational Classification""
                                },
						        ""ceterms: targetNodeName"": {
                                        ""en - US"": ""Welders, Cutters, Solderers, and Brazers""
                                },
						        ""ceterms: targetNodeDescription"": {
                                        ""en - US"": ""Use hand-welding, flame - cutting, hand soldering, or brazing equipment to weld or join metal components or to fill holes, indentations, or seams of fabricated metal products.""
                                }
                            },
					        {
                                ""@type"": ""ceterms: CredentialAlignmentObject"",
						        ""ceterms: framework"": ""https://www.onetcenter.org/taxonomy.html"",
                                ""ceterms: targetNode"": ""https://www.onetonline.org/link/summary/51-4122.00"",
                                ""ceterms: codedNotation"": ""51 - 4122.00"",
						        ""ceterms: frameworkName"": {
                                    ""en - US"": ""Standard Occupational Classification""
                                },
						        ""ceterms: targetNodeName"": {
                                    ""en - US"": ""Welding, Soldering, and Brazing Machine Setters, Operators, and Tenders""
                                },
						        ""ceterms: targetNodeDescription"": {
                                     ""en - US"": ""Set up, operate, or tend welding, soldering, or brazing machines or robots that weld, braze, solder, or heat treat metal products, components, or assemblies.Includes workers who operate laser cutters or laser-beam machines.""
                                }
                            }
				        ],
				        ""ceterms: subjectWebpage"": ""http://www.example.org/sampleGraph"",
                        ""ceterms: audienceLevelType"": [        
                            {
                                ""@type"": ""ceterms: CredentialAlignmentObject"",
						        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/AudienceLevel"",
                                ""ceterms: targetNode"": ""audLevel: AssociatesDegreeLevel"",
						        ""ceterms: frameworkName"": {
                                    ""en - US"": ""AudienceLevel""
                                },
						        ""ceterms: targetNodeName"": {
                                    ""en - US"": ""Associates Degree Level""
                                }
                            }
				        ],
				        ""ceterms: estimatedDuration"": [
                            {
                                ""@type"": ""ceterms: DurationProfile"",
						        ""ceterms: description"": {
                                    ""en - US"": ""The anticipated time for a full-time student( 30 annual semester hours ) to complete this program of study.""
                                },
						        ""ceterms: exactDuration"": ""P2Y""
                            }
				        ],
				        ""ceterms: financialAssistance"": [
                            {
                                 ""@type"": ""ceterms: FinancialAssistanceProfile"",
						        ""ceterms: name"": {
                                    ""en - US"": ""Scholarships and Grants""
                                },
						        ""ceterms: description"": {
                                    ""en - US"": ""The typical dollar value of scholarships, grants, or tuition waivers received by graduates of this degree program. Scholarships and grants do not need repaid.""
                                },
						        ""ceterms: financialAssistanceType"": [
                                    {
                                        ""@type"": ""ceterms: CredentialAlignmentObject"",
								        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/FinancialAssistance"",
                                        ""ceterms: targetNode"": ""financialAid: Scholarship"",
								        ""ceterms: frameworkName"": {
                                            ""en - US"": ""FinancialAssistance""
                                        },
								        ""ceterms: targetNodeName"": {
                                            ""en - US"": ""Scholarship""
                                        }
                                    },
							        {
                                        ""@type"": ""ceterms: CredentialAlignmentObject"",
								        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/FinancialAssistance"",
                                        ""ceterms: targetNode"": ""financialAid: Grant"",
								        ""ceterms: frameworkName"": {
                                            ""en - US"": ""FinancialAssistance""
                                        },
								        ""ceterms: targetNodeName"": {
                                            ""en - US"": ""Grant""
                                        }
                                    }
						        ],
						        ""ceterms: financialAssistanceValue"": [
                                    {
                                        ""@type"": ""schema: QuantitativeValue"",
								        ""schema: value"": 5651.0,
								        ""schema: unitText"": {
                                            ""@type"": ""ceterms: CredentialAlignmentObject"",
									        ""ceterms: targetNodeName"": {
                                                ""en - US"": ""USD""
                                            }
                                        },
								        ""qdata: percentage"": 0.0
                                    }
						        ]
					        },
					        {
                                ""@type"": ""ceterms: FinancialAssistanceProfile"",
						        ""ceterms: name"": {
                                    ""en - US"": ""Assistance Available Via Loans""
                                },
						        ""ceterms: description"": {
                                    ""en - US"": ""The typical dollar value of loans borrowed by graduates of this degree program. Loans do require repayment.""
                                },
						        ""ceterms: financialAssistanceType"": [
                                    {
                                        ""@type"": ""ceterms: CredentialAlignmentObject"",
								        ""ceterms: framework"": ""https://credreg.net/ctdl/terms/FinancialAssistance"",
                                        ""ceterms: targetNode"": ""financialAid: Loan"",
								        ""ceterms: frameworkName"": {
                                            ""en - US"": ""FinancialAssistance""
                                        },
								        ""ceterms: targetNodeName"": {
                                            ""en - US"": ""Loan""
                                        }
                                    }
						        ]
					        }
				        ],
				        ""ceterms: instructionalProgramType"": [
                            {
                                ""@type"": ""ceterms: CredentialAlignmentObject"",
						        ""ceterms: framework"": ""https://nces.ed.gov/ipeds/cipcode/Default.aspx?y=55"",
                                ""ceterms: codedNotation"": ""48.0508"",
						        ""ceterms: frameworkName"": {
                                    ""en - US"": ""Classification of Instructional Programs""
                                },
						        ""ceterms: targetNodeName"": {
                                     ""en - US"": ""Welding Technology/ Welder.""
                                },
						        ""ceterms: targetNodeDescription"": {
                                     ""en - US"": ""A program that prepares individuals to apply technical knowledge and skills to join or cut metal surfaces.Includes instruction in arc welding, resistance welding, brazing and soldering, cutting, high - energy beam welding and cutting, solid state welding, ferrous and non-ferrous materials, oxidation - reduction reactions, welding metallurgy, welding processes and heat treating, structural design, safety, and applicable codes and standards.""
                                }
                            }
				        ]
			        }
		        ]
	        }
            }";


            //====================	CREDENTIAL REQUEST ====================
            //This holds the credential and the identifier (CTID) for the owning organization
            var myRequest = new APIRequest()
            {
                GraphInput = graphInput,
                DefaultLanguage = "en-US",
                PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
            };

            //Serialize the credential request object
            //Preferably, use method that will exclude null/empty properties
            string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

            //call the Assistant API

            SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
            {
                EndpointType = "credential",
                RequestType = "publishGraph",
                OrganizationApiKey = apiKey,
                CTID = myCTID,   //added here for logging
                Identifier = "testing",     //useful for logging, might use the ctid
                InputPayload = payload
            };

            bool isValid = new SampleServices().PublishRequest( req );
            //Return the result
            return req.FormattedPayload;
        }

    }
}
