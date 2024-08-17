Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Newtonsoft.Json
Imports RA.Models.input
Imports YourCredential = VisualBasicPublishingSamples.Credential
Imports APIRequestCredential = RA.Models.input.Credential
Imports APIRequest = RA.Models.input.CredentialRequest

'C# code, but seems to still work
Imports RA.SamplesForDocumentation

Public Class PublishCredential
	Public Function PublishDetailedRecord(ByVal Optional requestType As String = "publish") As String
		Dim result = ""
		Dim apiKey = SampleServices.GetMyApiKey()

		If String.IsNullOrWhiteSpace(apiKey) Then
			Return "Ensure you have added your apiKey to the app.config"
		End If
		Dim organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID()
		If String.IsNullOrWhiteSpace(organizationIdentifierFromAccountsSite) Then
			Return "Ensure you have added your organization account CTID to the app.config"
		End If

		Dim myCTID = "ce-5a33409a-f3db-42f3-8a3c-b00c7bb393af"  '"ce-" & Guid.NewGuid().ToString().ToLower()
		Dim myData = New APIRequestCredential() With {
				.Name = "My Certification Name",
				.Description = "This is some text that describes my credential.",
				.Ctid = myCTID,
				.SubjectWebpage = "https://example.com/credential/1234",
				.CredentialType = "ceterms:Certification",
				.InLanguage = New List(Of String)() From {
					"en-US"
				},
				.Keyword = New List(Of String)() From {
					"Credentials",
					"Technical Information",
					"Credential Registry"
				},
				.Naics = New List(Of String)() From {
					"333922",
					"333923",
					"333924"
				}
			}
		myData.OwnedBy.Add(New OrganizationReference() With {
				.CTID = organizationIdentifierFromAccountsSite
			})
		myData.AccreditedBy.Add(New OrganizationReference() With {
				.CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			})
		myData.AccreditedBy.Add(New OrganizationReference() With {
				.Type = "QACredentialOrganization",
				.Name = "Council on Social Work Education (CSWE)",
				.SubjectWebpage = "https://www.cswe.org/",
				.Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			})
		myData.Jurisdiction.Add(Jurisdictions.SampleJurisdiction())
		myData.RecognizedIn.Add(Jurisdictions.SampleJurisdictionAssertion())
		myData.Requires = New List(Of ConditionProfile)() From {
				New ConditionProfile() With {
					.Description = "To earn this credential the following conditions must be met, and the target learning opportunity must be completed.",
					.Condition = New List(Of String)() From {
						"Complete High School",
						"Have a drivers licence."
					},
					.TargetLearningOpportunity = New List(Of EntityReference)() From {
						New EntityReference() With {
							.CTID = "ce-ccd00a32-d5ad-41e7-b14c-5c096bc9eea0"
						},
						New EntityReference() With {
							.Type = "LearningOpportunity",
							.Name = "Another required learning opportunity (external)",
							.Description = "A required learning opportunity that has not been published to Credential Registry. The type, name, and subject webpage are required. The description while useful is optional. ",
							.SubjectWebpage = "https://example.org?t=anotherLopp",
							.CodedNotation = "Learning 101"
						}
					}
				}
			}
		myData.EstimatedCost.Add(New CostProfile() With {
				.Description = "A required description of the cost profile",
				.CostDetails = "https://example.com/t=loppCostProfile",
				.Currency = "USD",
				.CostItems = New List(Of CostProfileItem)() From {
					New CostProfileItem() With {
						.DirectCostType = "Application",
						.Price = 100
					},
					New CostProfileItem() With {
						.DirectCostType = "Tuition",
						.Price = 12999,
						.PaymentPattern = "Full amount due at time of registration"
					}
				}
			})
		PopulateOccupations(myData)
		PopulateIndustries(myData)
		PopulatePrograms(myData)
		Dim isPreparationFor = New ConditionProfile With {
				.Description = "This certification will prepare a student for the target credential",
				.TargetCredential = New List(Of EntityReference)() From {
					New EntityReference() With {
						.Type = "MasterDegree",
						.Name = "Cybersecurity Technology Master's Degree  ",
						.Description = "A helpful description",
						.SubjectWebpage = "https://example.org?t=masters"
					}
				}
			}
		myData.IsPreparationFor.Add(isPreparationFor)
		Dim preparationFrom = New ConditionProfile With {
				.Description = "This credential will prepare a student for this credential",
				.TargetCredential = New List(Of EntityReference)() From {
					New EntityReference() With {
						.CTID = "ce-40c3e860-5034-4375-80e8-f7455ff86a48"
					}
				}
			}
		myData.PreparationFrom.Add(preparationFrom)
		Dim myRequest = New APIRequest() With {
				.Credential = myData,
				.DefaultLanguage = "en-us",
				.PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			}
		Dim payload As String = JsonConvert.SerializeObject(myRequest, SampleServices.GetJsonSettings())
		Dim req As SampleServices.AssistantRequestHelper = New SampleServices.AssistantRequestHelper() With {
				.EndpointType = "credential",
				.RequestType = requestType,
				.OrganizationApiKey = apiKey,
				.CTID = myRequest.Credential.CTID.ToLower(),
				.Identifier = "testing",
				.InputPayload = payload
			}
		Dim isValid As Boolean = New SampleServices().PublishRequest(req)
		Return req.FormattedPayload
	End Function


	Public Function PublishFromInputClass(ByVal input As YourCredential, ByVal Optional requestType As String = "publish") As String
		Dim result = ""
		Dim apiKey = SampleServices.GetMyApiKey()
		If String.IsNullOrWhiteSpace(apiKey) Then
			Return "Ensure you have added your apiKey to the app.config"
		End If
		Dim organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID()
		If String.IsNullOrWhiteSpace(organizationIdentifierFromAccountsSite) Then
			Return "Ensure you have added your organization account CTID to the app.config"
		End If
		Dim myData = New APIRequestCredential With {
				.Name = input.Name,
				.Description = input.Description,
				.InLanguage = New List(Of String)() From {
					"en-US"
				},
				.CredentialType = "BachelorDegree",
				.Ctid = input.Ctid,
				.CredentialStatusType = "Active",
				.DateEffective = input.DateEffective,
				.Image = input.ImageUrl,
				.Subject = input.Subject,
				.Keyword = input.Keyword
			}
		myData.OwnedBy.Add(New OrganizationReference() With {
				.CTID = organizationIdentifierFromAccountsSite
			})

		myData.AccreditedBy.Add(New OrganizationReference() With {
				.Type = "CredentialOrganization",
				.Name = "Council on Social Work Education (CSWE)",
				.SubjectWebpage = "https://www.cswe.org/",
				.Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			})
		myData.EstimatedCost.Add(New CostProfile() With {
				.Description = "A required description of the cost profile",
				.CostDetails = "https://example.com/t=loppCostProfile",
				.Currency = "USD",
				.CostItems = New List(Of CostProfileItem)() From {
					New CostProfileItem() With {
						.DirectCostType = "Application",
						.Price = 100
					},
					New CostProfileItem() With {
						.DirectCostType = "Tuition",
						.Price = 12999,
						.PaymentPattern = "Full amount due at time of registration"
					}
				}
			})
		PopulateOccupations(myData)
		PopulateIndustries(myData)
		PopulatePrograms(myData)
		FinancialAssistanceProfiles.PopulateSimpleFinancialAssistanceProfile(myData)
		Dim myRequest = New APIRequest() With {
				.Credential = myData,
				.DefaultLanguage = "en-US",
				.PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			}
		Dim payload As String = JsonConvert.SerializeObject(myRequest, SampleServices.GetJsonSettings())
		Dim req As SampleServices.AssistantRequestHelper = New SampleServices.AssistantRequestHelper() With {
				.EndpointType = "credential",
				.RequestType = requestType,
				.OrganizationApiKey = apiKey,
				.CTID = myRequest.Credential.Ctid.ToLower(),
				.Identifier = "testing",
				.InputPayload = payload
			}
		Dim isValid As Boolean = New SampleServices().PublishRequest(req)
		Return req.FormattedPayload
	End Function

	Public Shared Sub PopulateOccupations(ByVal request As APIRequestCredential)
		request.OccupationType = New List(Of FrameworkItem) From {
				New FrameworkItem() With {
					.Framework = "https://www.onetonline.org/",
					.FrameworkName = "Standard Occupational Classification",
					.Name = "Information Security Analysts",
					.TargetNode = "https://www.onetonline.org/link/summary/15-1122.00",
					.CodedNotation = "15-1122.00",
					.Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
				},
				New FrameworkItem() With {
					.Framework = "https://www.onetonline.org/",
					.FrameworkName = "Standard Occupational Classification",
					.Name = "Computer Network Support Specialists",
					.TargetNode = "https://www.onetonline.org/link/summary/15-1152.00",
					.CodedNotation = "15-1152.00",
					.Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
				}
			}
		request.AlternativeOccupationType = New List(Of String)() From {
				"Cybersecurity",
				"Forensic Scientist",
				"Forensic Anthropologist"
			}
		request.ONET_Codes = New List(Of String)() From {
				"13-2099.01",
				"13-2052.00",
				"13-2061.00",
				"13-2051.00"
			}
	End Sub

	Public Shared Sub PopulateIndustries(ByVal request As APIRequestCredential)
			request.IndustryType = New List(Of FrameworkItem) From {
				New FrameworkItem() With {
					.Framework = "https://www.naics.com/",
					.FrameworkName = "NAICS - North American Industry Classification System",
					.Name = "National Security",
					.TargetNode = "https://www.naics.com/naics-code-description/?code=928110",
					.CodedNotation = "928110",
					.Description = "This industry comprises government establishments of the Armed Forces, including the National Guard, primarily engaged in national security and related activities."
				},
				New FrameworkItem() With {
					.Framework = "https://www.naics.com/",
					.FrameworkName = "NAICS - North American Industry Classification System",
					.Name = "Regulation and Administration of Transportation Programs",
					.TargetNode = "https://www.naics.com/naics-code-description/?code=926120",
					.CodedNotation = "926120",
					.Description = "This industry comprises government establishments primarily engaged in the administration, regulation, licensing, planning, inspection, and investigation of transportation services and facilities. Included in this industry are government establishments responsible for motor vehicle and operator licensing, the Coast Guard (except the Coast Guard Academy), and parking authorities."
				}
			}
			request.AlternativeIndustryType = New List(Of String)() From {
				"Cybersecurity",
				"Forensic Science",
				"Forensic Anthropology"
			}
			request.Naics = New List(Of String)() From {
				"9271",
				"927110",
				"9281",
				"928110"
			}
		End Sub

	Public Shared Sub PopulatePrograms(ByVal request As APIRequestCredential)
		request.InstructionalProgramType = New List(Of FrameworkItem) From {
				New FrameworkItem() With {
					.Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					.FrameworkName = "Classification of Instructional Program",
					.Name = "Medieval and Renaissance Studies",
					.TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.1301",
					.CodedNotation = "30.1301",
					.Description = "A program that focuses on the  study of the Medieval and/or Renaissance periods in European and circum-Mediterranean history from the perspective of various disciplines in the humanities and social sciences, including history and archeology, as well as studies of period art and music."
				},
				New FrameworkItem() With {
					.Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					.FrameworkName = "Classification of Instructional Program",
					.Name = "Classical, Ancient Mediterranean and Near Eastern Studies and Archaeology",
					.TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.2202",
					.CodedNotation = "30.2202",
					.Description = "A program that focuses on the cultures, environment, and history of the ancient Near East, Europe, and the Mediterranean basin from the perspective of the humanities and social sciences, including archaeology."
				}
			}
		request.AlternativeInstructionalProgramType = New List(Of String)() From {
				"Cybersecurity 101",
				"Forensic Science 120",
				"Forensic Anthropology 400"
			}
		request.CIP_Codes = New List(Of String)() From {
				"31.0504",
				"31.0505",
				"31.0599",
				"31.9999"
			}
	End Sub
End Class

