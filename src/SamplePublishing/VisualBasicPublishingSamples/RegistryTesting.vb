Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports VisualBasicPublishingSamples
Imports YourOrganization = VisualBasicPublishingSamples.Organization
Imports YourCredential = VisualBasicPublishingSamples.Credential

<TestClass()> Public Class RegistryTesting

	<TestMethod()> Public Sub TestMethod1()

	End Sub

#Region "Organization"
	<TestMethod>
	Public Sub FormatOrganization()
		Dim result = New PublishOrganization().PublishSimpleRecord("format")
	End Sub

	<TestMethod>
	Public Sub PublishOrganization()
		Dim result = New PublishOrganization().PublishSimpleRecord("publish")
	End Sub

	<TestMethod>
	Public Sub PublishYourOrganization()
		Dim org = New YourOrganization With {
			.OrganizationType = "CredentialOrganization",
			.Name = "My Test Organization",
			.Description = "Description for my organization.",
			.SubjectWebpage = "https://example.org/t=testOrg",
			.AgentSectorType = "PrivateNonProfit",
			.AgentType = New List(Of String)(New String() {"Business"}),
			.Email = New List(Of String)(New String() {"me@myorg.com"})
		}
		'if doing a publish, save the value generated from the following and use rathner than
		org.Ctid = "ce-" & Guid.NewGuid().ToString().ToLower()
		org.Address = New List(Of RA.Models.input.Place) From {
			New RA.Models.input.Place With {
				.Address1 = "123 Main Str",
				.City = "Trenton",
				.AddressRegion = "New Jersey",
				.PostalCode = "01234",
				.Country = "USA"
			}
		}
		Dim result = New PublishOrganization().Publish(org, "format")
	End Sub
#End Region


#Region "Credential"
	<TestMethod>
	Public Sub FormatCredential()
		Dim result = New PublishCredential().PublishDetailedRecord("format")

	End Sub

	<TestMethod>
	Public Sub PublishCredential()
		Dim result = New PublishCredential().PublishDetailedRecord("publish")
	End Sub

	<TestMethod>
	Public Sub PublishYourCredential()
		Dim credential = New YourCredential With {
			.CredentialType = "Certification",
			.Name = "My Test Certification",
			.Description = "Description for my Certification.",
			.SubjectWebpage = "https://example.org/t=Certification"
		}
		'if doing a publish, save the value generated from the following and use rather than publishing duplicate test credentails
		credential.Ctid = "ce-" & Guid.NewGuid().ToString().ToLower()
		credential.Address = New List(Of RA.Models.input.Place) From {
			New RA.Models.input.Place With {
				.Address1 = "123 Main Str",
				.City = "Trenton",
				.AddressRegion = "New Jersey",
				.PostalCode = "01234",
				.Country = "USA"
			}
		}
		Dim result = New PublishCredential().PublishFromInputClass(credential, "format")
	End Sub
#End Region


End Class