Imports RA.Models.input

'Namespace VisualBasicSamplePublishing
Public Class BaseModel
	Public Property Name As String
	Public Property Description As String
	Public Property SubjectWebpage As String
	Public Property Ctid As String
	Public Property DateEffective As String
	Public Property Address As List(Of Place) = New List(Of Place)
End Class
'End Namespace
