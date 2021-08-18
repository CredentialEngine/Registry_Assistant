Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

'Namespace VisualBasicSamplePublishing
Public Class Organization
	Inherits BaseModel

	Public Property OrganizationType As String = "CredentialOrganization"
	Public Property ImageUrl As String
	Public Property Email As List(Of String)
	Public Property AgentType As List(Of String)
	Public Property Keywords As List(Of String)
	Public Property AgentSectorType As String = "PrivateNonProfit"

End Class
'End Namespace
