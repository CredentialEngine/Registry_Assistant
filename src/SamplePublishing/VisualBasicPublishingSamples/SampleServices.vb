Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports System.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports RA.Models
Imports RA.Models.input
Imports RA.Models.input.profiles.QData
Imports RAResponse = RA.Models.RegistryAssistantResponse

'Namespace VisualBasicSamplePublishing
Public Class SampleServices
		Public Shared thisClassName As String = "SampleServices"
	Public environment As String = GetAppKeyValue("environment")

	Public Shared Function GetMyApiKey() As String
			Return GetAppKeyValue("myOrgApiKey")
		End Function

		Public Shared Function GetMyOrganizationCTID() As String
			Return GetAppKeyValue("myOrgCTID")
		End Function

		Public Shared Function AddQuantitativeValue(ByVal value As Integer, ByVal description As String) As QuantitativeValue
			Dim output = New QuantitativeValue() With {
				.Value = value,
				.Description = If(Not String.IsNullOrWhiteSpace(description), description, String.Format("Adding value of: {0}", value))
			}
			Return output
		End Function

		Public Shared Function AddQuantitativeValue(ByVal minValue As Integer, ByVal maxValue As Integer, ByVal description As String) As QuantitativeValue
			Dim output = New QuantitativeValue() With {
				.MinValue = minValue,
				.MaxValue = maxValue,
				.Description = description
			}
			Return output
		End Function

		Public Shared Function AddQuantitativeValue(ByVal value As Decimal, ByVal description As String) As QuantitativeValue
			Dim output = New QuantitativeValue() With {
				.Value = value,
				.Description = If(Not String.IsNullOrWhiteSpace(description), description, String.Format("Adding value of: {0}", value))
			}
			Return output
		End Function

		Public Shared Function AddQuantitativePercentage(ByVal value As Decimal, ByVal description As String) As QuantitativeValue
			Dim output = New QuantitativeValue() With {
				.Percentage = value,
				.Description = If(Not String.IsNullOrWhiteSpace(description), description, String.Format("Adding value of: {0}", value))
			}
			Return output
		End Function

		Public Shared Function GetAppKeyValue(ByVal keyName As String) As String
			Return GetAppKeyValue(keyName, "")
		End Function

		Public Shared Function GetAppKeyValue(ByVal keyName As String, ByVal defaultValue As String) As String
			Dim appValue As String = ""

			Try
				appValue = System.Configuration.ConfigurationManager.AppSettings(keyName)
				If appValue Is Nothing Then appValue = defaultValue
			Catch
				appValue = defaultValue
				If HasMessageBeenPreviouslySent(keyName) = False Then LoggingHelper.LogError(String.Format("@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue))
			End Try

			Return appValue
		End Function

		Public Shared Function GetAppKeyValue(ByVal keyName As String, ByVal defaultValue As Integer) As Integer
			Dim appValue As Integer = -1

			Try
				appValue = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings(keyName))
			Catch
				appValue = defaultValue
				If HasMessageBeenPreviouslySent(keyName) = False Then LoggingHelper.LogError(String.Format("@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue))
			End Try

			Return appValue
		End Function

		Public Shared Function GetAppKeyValue(ByVal keyName As String, ByVal defaultValue As Boolean, ByVal Optional reportMissingKey As Boolean = True) As Boolean
			Dim appValue As Boolean = False

			Try
				appValue = Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings(keyName))
			Catch ex As Exception
				appValue = defaultValue
				If reportMissingKey AndAlso HasMessageBeenPreviouslySent(keyName) = False Then LoggingHelper.LogError(String.Format("@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue))
			End Try

			Return appValue
		End Function

		Public Shared Function HasMessageBeenPreviouslySent(ByVal keyName As String) As Boolean
			Dim key As String = "appkey_" & keyName

			If HttpRuntime.Cache(key) IsNot Nothing Then
				Return True
			Else
				HttpRuntime.Cache.Insert(key, keyName)
			End If

			Return False
		End Function

	Public Function SimplePost(ByVal entityType As String, ByVal requestType As String, ByVal payload As String, ByVal apiKey As String) As String

		Dim jsonldPayload As String = ""
		Dim messages As List(Of String) = New List(Of String)()
		Dim serviceUri As String = GetAppKeyValue("registryAssistantApi")
		Dim assistantUrl As String = serviceUri & String.Format("{0}/{1}", entityType, requestType)

		Return SimplePost(assistantUrl, payload, apiKey, jsonldPayload, messages)
	End Function

	Public Function LessSimplePost(ByVal entityType As String, ByVal requestType As String, ByVal payload As String, ByVal apiKey As String, ByRef jsonldPayload As String, ByRef messages As List(Of String)) As String

		Dim serviceUri As String = GetAppKeyValue("registryAssistantApi")
		Dim assistantUrl As String = serviceUri & String.Format("{0}/{1}", entityType, requestType)

		Return SimplePost(assistantUrl, payload, apiKey, jsonldPayload, messages)
	End Function

	Public Function SimplePost(ByVal assistantUrl As String, ByVal payload As String, ByVal apiKey As String, ByRef jsonldPayload As String, ByRef messages As List(Of String)) As String
			Dim result = ""
			Dim response As RAResponse = New RAResponse()
			Dim responseContents As String = ""

			Using client = New HttpClient()
				client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
				client.DefaultRequestHeaders.Add("Authorization", "ApiToken " & apiKey)
				Dim content = New StringContent(payload, Encoding.UTF8, "application/json")
				Dim publishEndpoint = assistantUrl
				Dim task = client.PostAsync(publishEndpoint, content)
				task.Wait()
				Dim taskResult = task.Result
				responseContents = task.Result.Content.ReadAsStringAsync().Result

			If taskResult.IsSuccessStatusCode = False Then
				response = JsonConvert.DeserializeObject(Of RAResponse)(responseContents)
				Dim status As String = String.Join(",", response.Messages.ToArray())
				jsonldPayload = If(response.Payload, "")
				messages.AddRange(response.Messages)
			Else
				response = JsonConvert.DeserializeObject(Of RAResponse)(responseContents)
				jsonldPayload = If(response.Payload, "")
			End If

		End Using

		Return result
	End Function

	Public Function PublishRequest(ByVal request As AssistantRequestHelper) As Boolean
			Dim serviceUri As String = GetAppKeyValue("registryAssistantApi")

			If System.DateTime.Now.Day = 10 Then
			End If

			request.EndpointUrl = serviceUri & String.Format("{0}/{1}", request.EndpointType, request.RequestType)
			Return PostRequest(request)
		End Function

		Public Function PostRequest(ByVal request As AssistantRequestHelper) As Boolean
			Dim response As RAResponse = New RAResponse()
			Dim listResponse = New List(Of RAResponse)()
			LoggingHelper.DoTrace(5, String.Format(thisClassName & ".PostRequest, RequestType: {0}, CTID: {1}, payloadLen: {2}, starts: '{3}' ....", request.RequestType, request.CTID, (If(request.InputPayload, "")).Length, request.InputPayload.Substring(0, If(request.InputPayload.Length > 200, 200, request.InputPayload.Length))))
			LoggingHelper.WriteLogFile(5, request.EndpointType & "_" & request.CTID & "_" & request.RequestType & "_rainput.json", request.InputPayload, "", False)
			Dim responseContents As String = ""
			Dim started As DateTime = DateTime.Now

			Try

				Using client = New HttpClient()
					client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
					If String.IsNullOrWhiteSpace(request.AuthorizationToken) Then request.AuthorizationToken = request.OrganizationApiKey

					If Not String.IsNullOrWhiteSpace(request.AuthorizationToken) Then
						client.DefaultRequestHeaders.Add("Authorization", "ApiToken " & request.AuthorizationToken)
					Else

						If request.RequestType = "publish" AndAlso (environment = "production" OrElse environment = "staging") Then
							request.Messages.Add("Error - an apiKey was not found for the owning organization. The owning organization must be approved in the Credential Engine Accounts site before being able to publish data.")
							Return False
						End If
					End If

					LoggingHelper.DoTrace(6, "Publisher.PostRequest: doing PostAsync to: " & request.EndpointUrl)
					Dim task = client.PostAsync(request.EndpointUrl, New StringContent(request.InputPayload, Encoding.UTF8, "application/json"))
					task.Wait()
					Dim result = task.Result
					responseContents = task.Result.Content.ReadAsStringAsync().Result

					If result.IsSuccessStatusCode = False Then
						LoggingHelper.DoTrace(6, "Publisher.PostRequest: result.IsSuccessStatusCode == false")
						response = JsonConvert.DeserializeObject(Of RAResponse)(responseContents)
						Dim status As String = String.Join(",", response.Messages.ToArray())
						request.FormattedPayload = If(response.Payload, "")
						request.Messages.AddRange(response.Messages)
						LoggingHelper.DoTrace(4, thisClassName & String.Format(".PostRequest() {0} {1} failed: {2}", request.EndpointType, request.RequestType, status))
						LoggingHelper.LogError(thisClassName & String.Format(".PostRequest()  {0} {1}. Failed. Messages: {2}" & vbCrLf & "Response: " & vbLf & vbCr & responseContents & ". payload: " + response.Payload, request.EndpointType, request.RequestType, status))
						Return False
					Else

						If responseContents.IndexOf("[") > 5 Then
							response = JsonConvert.DeserializeObject(Of RAResponse)(responseContents)

							If response.Successful Then
								LoggingHelper.WriteLogFile(5, request.EndpointType & "_" & request.CTID & "_payload_Successful.json", response.Payload, "", False)
								request.FormattedPayload = response.Payload
								request.EnvelopeIdentifier = response.RegistryEnvelopeIdentifier
								request.Messages.AddRange(response.Messages)
								Dim publishedUrl = response.GraphUrl
							Else
								Dim status As String = String.Join(",", response.Messages.ToArray())
								LoggingHelper.DoTrace(5, thisClassName & " PostRequest FAILED. result: " & status)
								request.Messages.AddRange(response.Messages)
								request.FormattedPayload = response.Payload
								Return False
							End If
						Else
							listResponse = JsonConvert.DeserializeObject(Of List(Of RAResponse))(responseContents)
							Dim cntr As Integer = 0

							For Each lresponse In listResponse
								cntr += 1

								If lresponse.Successful Then
									LoggingHelper.WriteLogFile(5, request.EndpointType & String.Format("_(#{0})_{1}_payload_Successful.json", cntr, lresponse.CTID), lresponse.Payload, "", False)

									If cntr = listResponse.Count() Then
										request.FormattedPayload = lresponse.Payload
										request.EnvelopeIdentifier = lresponse.RegistryEnvelopeIdentifier
									End If

									request.Messages.AddRange(lresponse.Messages)
								Else
									Dim status As String = String.Join(",", lresponse.Messages.ToArray())
									LoggingHelper.DoTrace(5, thisClassName & String.Format(" PostRequest #{0} FAILED. result: {1}", cntr, status))
									request.Messages.AddRange(lresponse.Messages)

									If cntr = listResponse.Count() Then
										request.FormattedPayload = lresponse.Payload
									End If

									Return False
								End If
							Next
						End If
					End If

					Return result.IsSuccessStatusCode
				End Using

			Catch ae As AggregateException
				Dim timeOutNote = ""
				Dim duration As TimeSpan = DateTime.Now.Subtract(started)

				If duration.TotalSeconds > 60 Then
					timeOutNote = " The call to the API timed out. However, the publish activity may have still been successful!"
				End If

				LoggingHelper.LogError(ae, String.Format("PostRequest.AggregateException. RequestType:{0}, Identifier: {1}", request.RequestType, request.Identifier))
				Dim message As String = LoggingHelper.FormatExceptions(ae)
				request.Messages.Add(message & timeOutNote)
				Return False
			Catch exc As Exception
				LoggingHelper.LogError(exc, String.Format("PostRequest. RequestType:{0}, Identifier: {1}. /n/r responseContents: {2}", request.RequestType, request.Identifier, (If(responseContents, "empty"))))
				Dim message As String = LoggingHelper.FormatExceptions(exc)

				If message.IndexOf("Time out") > -1 Then
					message = "The request took too long and has timed out waiting for a reply. Your request may still have been successful. Please contact System Administration. "
				End If

				request.Messages.Add(message)
				Return False
			Finally
				LoggingHelper.DoTrace(5, String.Format(thisClassName & ".PostRequest. Exiting."))
			End Try
		End Function

		Public Shared Function GetJsonSettings() As JsonSerializerSettings
			Dim settings = New JsonSerializerSettings() With {
				.NullValueHandling = NullValueHandling.Ignore,
				.DefaultValueHandling = DefaultValueHandling.Ignore,
				.ContractResolver = New EmptyNullResolver(),
				.Formatting = Formatting.Indented,
				.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			}
			Return settings
		End Function

		Public Class AlphaNumericContractResolver
			Inherits DefaultContractResolver

			Protected Overrides Function CreateProperties(ByVal type As System.Type, ByVal memberSerialization As MemberSerialization) As System.Collections.Generic.IList(Of JsonProperty)
				Return MyBase.CreateProperties(type, memberSerialization).OrderBy(Function(m) m.PropertyName).ToList()
			End Function
		End Class

		Public Class EmptyNullResolver
			Inherits DefaultContractResolver

			Protected Overrides Function CreateProperty(ByVal member As MemberInfo, ByVal memberSerialization As MemberSerialization) As JsonProperty
				Dim [property] = MyBase.CreateProperty(member, memberSerialization)
				Dim isDefaultValueIgnored = ((If([property].DefaultValueHandling, DefaultValueHandling.Ignore)) And DefaultValueHandling.Ignore) <> 0

				If isDefaultValueIgnored Then

					If Not GetType(String).IsAssignableFrom([property].PropertyType) AndAlso GetType(IEnumerable).IsAssignableFrom([property].PropertyType) Then
						Dim newShouldSerialize As Predicate(Of Object) = Function(obj)
																			 Dim collection = TryCast([property].ValueProvider.GetValue(obj), ICollection)
																			 Return collection Is Nothing OrElse collection.Count <> 0
																		 End Function

						Dim oldShouldSerialize As Predicate(Of Object) = [property].ShouldSerialize
						[property].ShouldSerialize = If(oldShouldSerialize IsNot Nothing, Function(o) oldShouldSerialize(oldShouldSerialize) AndAlso newShouldSerialize(oldShouldSerialize), newShouldSerialize)
					ElseIf GetType(String).IsAssignableFrom([property].PropertyType) Then
						Dim newShouldSerialize As Predicate(Of Object) = Function(obj)
																			 Dim value = TryCast([property].ValueProvider.GetValue(obj), String)
																			 Return Not String.IsNullOrEmpty(value)
																		 End Function

						Dim oldShouldSerialize As Predicate(Of Object) = [property].ShouldSerialize
						[property].ShouldSerialize = If(oldShouldSerialize IsNot Nothing, Function(o) oldShouldSerialize(oldShouldSerialize) AndAlso newShouldSerialize(oldShouldSerialize), newShouldSerialize)
					End If
				End If

				Return [property]
			End Function
		End Class

		Public Class AssistantRequestHelper
			Public Sub New()
				Messages = New List(Of String)()
				OrganizationApiKey = ""
			End Sub

			Public Property RequestType As String
			Public Property AuthorizationToken As String
			Public Property OrganizationApiKey As String
			Public Property CTID As String
			Public Property Submitter As String
			Public Property InputPayload As String
			Public Property EndpointType As String
			Public Property EndpointUrl As String
			Public Property Identifier As String
			Public Property EnvelopeIdentifier As String
			Public Property FormattedPayload As String
			Public Property Messages As List(Of String)
		End Class

		Public Class RegistryResponseContent
			<JsonProperty(PropertyName:="errors")>
			Public Property Errors As List(Of String)
			<JsonProperty(PropertyName:="json_schema")>
			Public Property JsonSchema As List(Of String)
		End Class
	End Class
'End Namespace
