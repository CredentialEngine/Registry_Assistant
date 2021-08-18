Public Class Credential
	Inherits BaseModel

	Public Property CredentialType As String
	Public Property AvailabilityListing As String
	Public Property AvailableOnlineAt As String
	Public Property CodedNotation As String
	Public Property ImageUrl As String
	Public Property LatestVersionUrl As String
	Public Property PreviousVersion As String
	Public Property Subject As List(Of String)
	Public Property Keyword As List(Of String)
End Class

