Imports Newtonsoft.Json
Imports RA.Models.input
Imports APIRequest = RA.Models.input.OrganizationRequest
Imports APIRequestOrganization = RA.Models.input.Organization
Imports YourOrganization = VisualBasicPublishingSamples.Organization

'Namespace VisualBasicPublishingSamples
Public Class PublishOrganization
	Public Function PublishSimpleRecord(ByVal Optional requestType As String = "publish") As String
		Dim apiKey = SampleServices.GetMyApiKey()
		If String.IsNullOrWhiteSpace(apiKey) Then
			Return "Ensure you have added your apiKey to the app.config"
		End If
		Dim organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID()
		If String.IsNullOrWhiteSpace(organizationIdentifierFromAccountsSite) Then
			Return "Ensure you have added your organization account CTID to the app.config"
		End If

		Dim myOrgCTID = "ce-" & Guid.NewGuid().ToString().ToLower()
		DataService.SaveOrganizationCTID(myOrgCTID)
		Dim myData = New APIRequestOrganization() With {
				.Name = "My Organization Name",
				.Description = "This is some text that describes my organization.",
				.Ctid = myOrgCTID,
				.SubjectWebpage = "http://example.com",
				.Type = "ceterms:CredentialOrganization",
				.Keyword = New List(Of String)() From {
					"Credentials",
					"Technical Training",
					"Credential Registry Consulting"
				},
				.Email = New List(Of String)() From {
					"info@myOrg.com"
				}
			}
		myData.AgentSectorType = "PrivateNonProfit"
		myData.AgentType.Add("Business")
		Dim mainAddress = New Place() With {
				.Address1 = "123 Main Street",
				.Address2 = "Suite 2",
				.City = "Springfield",
				.AddressRegion = "Illinois",
				.PostalCode = "62704",
				.Country = "United States"
			}
		mainAddress.ContactPoint = New List(Of ContactPoint)() From {
				New ContactPoint() With {
					.ContactType = "Information",
					.Name = "Toll-Free",
					.PhoneNumbers = New List(Of String)() From {
						"800-555-1212"
					}
				}
			}
		myData.Address.Add(mainAddress)
		Dim techSupport = New Place() With {
				.ContactPoint = New List(Of ContactPoint)() From {
					New ContactPoint() With {
						.ContactType = "Tech-Support",
						.PhoneNumbers = New List(Of String)() From {
							"800-555-1212"
						},
						.Emails = New List(Of String)() From {
							"techSupport@myOrg.com"
						}
					}
				}
			}
		myData.Address.Add(techSupport)
		myData.Department.Add(New OrganizationReference() With {
				.Name = "A Department for my organization",
				.Description = "A test Department - third party format",
				.SubjectWebpage = "http://example.com?t=testDepartment",
				.Type = OrganizationReference.CredentialOrganization
			})
		myData.AccreditedBy.Add(New OrganizationReference() With {
				.CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			})
		myData.AccreditedBy.Add(New OrganizationReference() With {
				.Type = "CredentialOrganization",
				.Name = "Council on Social Work Education (CSWE)",
				.SubjectWebpage = "https://www.cswe.org/",
				.Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			})
		Dim myRequest = New APIRequest() With {
				.Organization = myData,
				.PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			}
		Dim payload As String = JsonConvert.SerializeObject(myRequest, SampleServices.GetJsonSettings())
		Dim result = New SampleServices().SimplePost("organization", requestType, payload, apiKey)
		Return result
	End Function

	''' <summary>
	''' Sample publish of an organization using an input class (that would be populated from local data stores)
	''' </summary>
	''' <param name="input"></param>
	''' <param name="requestType"></param>
	''' <returns></returns>
	Public Function Publish(ByVal input As YourOrganization, ByVal Optional requestType As String = "publish") As String
		Dim result = ""
		Dim apiKey = SampleServices.GetMyApiKey()
		If String.IsNullOrWhiteSpace(apiKey) Then
			Return "Ensure you have added your apiKey to the app.config"
		End If
		Dim organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID()
		If String.IsNullOrWhiteSpace(organizationIdentifierFromAccountsSite) Then
			Return "Ensure you have added your organization account CTID to the app.config"
		End If
		'
		Dim myData As APIRequestOrganization = New APIRequestOrganization With {
				.Name = input.Name,
				.Description = input.Description,
				.Type = input.OrganizationType,
				.Ctid = input.Ctid,
				.SubjectWebpage = input.SubjectWebpage,
				.Image = input.ImageUrl,
				.Keyword = input.Keywords
			}
		myData.AgentSectorType = input.AgentSectorType
		myData.AgentType = input.AgentType
		myData.Email = input.Email

		If input.Address IsNot Nothing AndAlso input.Address.Count > 0 Then

			For Each item In input.Address
				'a minimum check for data, probably would be more expansive
				If Not String.IsNullOrWhiteSpace(item.City) Then
					Dim mainAddress = New Place() With {
							.Address1 = item.Address1,
							.Address2 = item.Address2,
							.City = item.City,
							.AddressRegion = item.AddressRegion,
							.PostalCode = item.PostalCode,
							.Country = item.Country
						}
					myData.Address.Add(mainAddress)
				End If
			Next
		End If
		'Can include 'owned' credentials
		myData.Owns.Add(New EntityReference() With {
				.CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			})
		myData.AccreditedBy.Add(New OrganizationReference() With {
				.Type = "QACredentialOrganization",
				.Name = "Higher Learning Commission",
				.SubjectWebpage = "https://www.hlcommission.org/"
			})
		Dim myRequest = New APIRequest() With {
				.Organization = myData,
				.DefaultLanguage = "en-US",
				.PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			}
		Dim payload As String = JsonConvert.SerializeObject(myRequest, SampleServices.GetJsonSettings())

		result = New SampleServices().SimplePost("organization", requestType, payload, apiKey)

		Return result
	End Function

	Public Function ThirdPartyPublishSimpleRecord() As String
			Dim result = ""
			Dim apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
			Dim organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
			Dim myOrgCTID = "ce-" & Guid.NewGuid().ToString().ToLower()
			DataService.SaveOrganizationCTID(myOrgCTID)
			Dim myData = New APIRequestOrganization() With {
				.Name = "My Organization Name",
				.Description = "This is some text that describes my organization.",
				.Ctid = myOrgCTID,
				.SubjectWebpage = "http://example.com",
				.Type = "ceterms:CredentialOrganization",
				.Keyword = New List(Of String)() From {
					"Credentials",
					"Technical Information",
					"Credential Registry"
				},
				.Email = New List(Of String)() From {
					"info@myOrg.com"
				}
			}
			myData.AgentSectorType = "PrivateNonProfit"
			myData.AgentType.Add("Business")
			Dim myRequest = New APIRequest() With {
				.Organization = myData,
				.PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			}
			Dim payload As String = JsonConvert.SerializeObject(myRequest, SampleServices.GetJsonSettings())
			result = New SampleServices().SimplePost("organization", "publish", payload, apiKey)
			Return result
		End Function

		Public Class DataService
			Public Shared Sub SaveOrganizationCTID(ByVal ctid As String)
			End Sub
		End Class
	End Class
'End Namespace

