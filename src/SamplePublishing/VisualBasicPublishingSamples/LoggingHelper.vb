Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web

'Namespace VisualBasicSamplePublishing
Public Class LoggingHelper
	Const thisClassName As String = "LoggingHelper"
	Private Shared DefaultSubject As String = "Credential Publisher Application Exception encountered"

	Public Sub New()
	End Sub

	Public Shared Sub LogError(ByVal ex As Exception, ByVal message As String)
		Dim subject As String = DefaultSubject
		Dim notifyAdmin As Boolean = False
		If SampleServices.GetAppKeyValue("notifyOnException", "no").ToLower() = "yes" Then notifyAdmin = True
		LogError(ex, message, notifyAdmin, subject)
	End Sub

	Public Shared Sub LogError(ByVal ex As Exception, ByVal message As String, ByVal notifyAdmin As Boolean)
		Dim subject As String = DefaultSubject
		If SampleServices.GetAppKeyValue("notifyOnException", "no").ToLower() = "yes" Then notifyAdmin = True
		LogError(ex, message, notifyAdmin, subject)
	End Sub

	Public Shared Sub LogError(ByVal ex As Exception, ByVal message As String, ByVal subject As String)
		Dim notifyAdmin As Boolean = False
		If SampleServices.GetAppKeyValue("notifyOnException", "no").ToLower() = "yes" Then notifyAdmin = True
		LogError(ex, message, notifyAdmin, subject)
	End Sub

	Public Shared Sub LogError(ByVal ex As Exception, ByVal message As String, ByVal notifyAdmin As Boolean, ByVal Optional subject As String = "Credential Publisher Application Exception encountered")
		Dim sessionId As String = "unknown"
		Dim remoteIP As String = "unknown"
		Dim path As String = "unknown"
		Dim queryString As String = "unknown"
		Dim url As String = "unknown"
		Dim parmsString As String = ""
		Dim lRefererPage As String = ""

		Try

			If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
				sessionId = HttpContext.Current.Session.SessionID.ToString()
				remoteIP = HttpContext.Current.Request.ServerVariables("REMOTE_HOST")

				If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then
					lRefererPage = HttpContext.Current.Request.UrlReferrer.ToString()
				End If

				Dim serverName As String = SampleServices.GetAppKeyValue("serverName", HttpContext.Current.Request.ServerVariables("LOCAL_ADDR"))
				path = serverName & HttpContext.Current.Request.Path

				If IsValidRequestString() = True Then
					queryString = HttpContext.Current.Request.Url.AbsoluteUri.ToString()
					url = HttpContext.Current.Server.UrlDecode(queryString)
				Else
					url = "suspicious url encountered!!"
				End If
			End If

		Catch
		End Try

		Try
			Dim exceptions As String = FormatExceptions(ex)
			Dim errMsg As String = message & vbCrLf & "Type: " & ex.[GetType]().ToString() & ";" & vbCrLf & "Session Id - " & sessionId & "____IP - " & remoteIP & vbCr & vbCr & "Referrer: " & lRefererPage & ";" & vbCrLf & "Exception: " & exceptions & ";" & vbCrLf & "Stack Trace: " & ex.StackTrace.ToString() & vbCrLf & "Server\Template: " & path & vbCrLf & "Url: " & url
			If parmsString.Length > 0 Then errMsg += vbCrLf & "Parameters: " & parmsString
			LoggingHelper.LogError(errMsg, notifyAdmin, subject)
		Catch
		End Try
	End Sub

	Public Shared Sub LogError(ByVal message As String, ByVal Optional subject As String = "")
		If SampleServices.GetAppKeyValue("notifyOnException", "no").ToLower() = "yes" Then
			LogError(message, True, subject)
		Else
			LogError(message, False, subject)
		End If
	End Sub

	Public Shared Sub LogError(ByVal message As String, ByVal notifyAdmin As Boolean, ByVal Optional subject As String = "")
		If String.IsNullOrWhiteSpace(subject) Then subject = DefaultSubject

		If SampleServices.GetAppKeyValue("logErrors").ToString().Equals("yes") Then

			Try
				Dim datePrefix As String = System.DateTime.Today.ToString("yyyy-dd")
				Dim logFile As String = SampleServices.GetAppKeyValue("path.error.log", "")

				If Not String.IsNullOrWhiteSpace(logFile) Then
					Dim outputFile As String = logFile.Replace("[date]", datePrefix)

					If System.IO.File.Exists(outputFile) Then
						If System.IO.File.GetLastWriteTime(outputFile).Month <> DateTime.Now.Month Then System.IO.File.Delete(outputFile)
					Else
						Dim f As System.IO.FileInfo = New System.IO.FileInfo(outputFile)
						f.Directory.Create()
					End If

					Dim file As StreamWriter = System.IO.File.AppendText(outputFile)
					file.WriteLine(DateTime.Now & ": " & message)
					file.WriteLine("---------------------------------------------------------------------")
					file.Close()

					If notifyAdmin Then
					End If
				End If

			Catch ex As Exception
				DoTrace(5, thisClassName & ".LogError(string message, bool notifyAdmin). Exception: " & ex.Message)
			End Try
		End If
	End Sub

	Public Shared Function FormatExceptions(ByVal ex As Exception) As String
		Dim message As String = ex.Message

		If ex.InnerException IsNot Nothing Then
			message += "; " & vbCrLf & "InnerException: " & ex.InnerException.Message

			If ex.InnerException.InnerException IsNot Nothing Then
				message += "; " & vbCrLf & "InnerException2: " & ex.InnerException.InnerException.Message

				If ex.InnerException.InnerException.InnerException IsNot Nothing Then
					message += "; " & vbCrLf & "InnerException3: " & ex.InnerException.InnerException.InnerException.Message
				End If
			End If
		End If

		Return message
	End Function

	Private Shared Function ShouldMessagesBeSkipped(ByVal message As String) As Boolean
		If message.IndexOf("Server cannot set status after HTTP headers have been sent") > 0 Then Return True
		Return False
	End Function

	Public Shared Sub DoTrace(ByVal message As String)
		Dim appTraceLevel As Integer = SampleServices.GetAppKeyValue("appTraceLevel", 8)
		If appTraceLevel < 8 Then appTraceLevel = 8
		DoTrace(appTraceLevel, message, True)
	End Sub

	Public Shared Sub DoTrace(ByVal level As Integer, ByVal message As String, ByVal Optional showingDatetime As Boolean = True)
		Dim msg As String = ""
		Dim appTraceLevel As Integer = SampleServices.GetAppKeyValue("appTraceLevel", 6)
		Const NumberOfRetries As Integer = 4
		Const DelayOnRetry As Integer = 1000

		If showingDatetime Then
			msg = vbLf & " " & System.DateTime.Now.ToString() & " - " & message
		Else
			msg = vbLf & " " & message
		End If

		Dim datePrefix1 As String = System.DateTime.Today.ToString("u").Substring(0, 10)
		Dim datePrefix As String = System.DateTime.Today.ToString("yyyy-dd")
		Dim logFile As String = SampleServices.GetAppKeyValue("path.trace.log", "")

		If String.IsNullOrWhiteSpace(logFile) OrElse level > appTraceLevel Then
			Return
		End If

		Dim outputFile As String = ""

		For i As Integer = 1 To NumberOfRetries

			Try
				outputFile = logFile.Replace("[date]", datePrefix & (If(i < 3, "", "_" & i.ToString())))

				If System.IO.File.Exists(outputFile) Then
					If System.IO.File.GetLastWriteTime(outputFile).Month <> DateTime.Now.Month Then System.IO.File.Delete(outputFile)
				Else
					Dim f As System.IO.FileInfo = New System.IO.FileInfo(outputFile)
					f.Directory.Create()
				End If

				Dim file As StreamWriter = System.IO.File.AppendText(outputFile)
				file.WriteLine(msg)
				file.Close()
				Console.WriteLine(msg)
				Exit For
			Catch e As IOException When i <= NumberOfRetries
				Thread.Sleep(DelayOnRetry)
			Catch ex As Exception

				If ex.Message.IndexOf("Access to the path") > -1 Then
					Exit For
				End If
			End Try
		Next
	End Sub

	Public Shared Sub WriteLogFile(ByVal level As Integer, ByVal filename As String, ByVal message As String, ByVal Optional datePrefixOverride As String = "", ByVal Optional appendingText As Boolean = True)
		Dim appTraceLevel As Integer = 0

		Try
			appTraceLevel = SampleServices.GetAppKeyValue("appTraceLevel", 6)

			If level <= appTraceLevel Then
				Dim datePrefix1 As String = System.DateTime.Today.ToString("u").Substring(0, 10)
				Dim datePrefix As String = System.DateTime.Today.ToString("yyyy-dd")
				Dim dateTemplate As String = "[date]"

				If datePrefixOverride = "__" Then
					datePrefix = ""
					dateTemplate = "[date]_"
				ElseIf Not String.IsNullOrWhiteSpace(datePrefixOverride) Then
					datePrefix = datePrefixOverride
				End If

				Dim logFile As String = SampleServices.GetAppKeyValue("path.log.file", "C:\LOGS.txt")
				Dim outputFile As String = logFile.Replace(dateTemplate, datePrefix).Replace("[filename]", filename)

					If outputFile.IndexOf("csv.txt") > 1 Then
						outputFile = outputFile.Replace("csv.txt", "csv")
					ElseIf outputFile.IndexOf("csv.json") > 1 Then
						outputFile = outputFile.Replace("csv.json", "csv")
					ElseIf outputFile.IndexOf("cs.json") > 1 Then
						outputFile = outputFile.Replace("cs.json", "cs")
					ElseIf outputFile.IndexOf("json.txt") > 1 Then
						outputFile = outputFile.Replace("json.txt", "json")
					ElseIf outputFile.IndexOf("json.json") > 1 Then
						outputFile = outputFile.Replace("json.json", "json")
					ElseIf outputFile.IndexOf("txt.json") > 1 Then
						outputFile = outputFile.Replace("txt.json", "txt")
					End If

					If appendingText Then
						Dim file As StreamWriter = System.IO.File.AppendText(outputFile)
						file.WriteLine(message)
						file.Close()
					Else
						File.WriteAllText(outputFile, message)
					End If
				End If

			Catch
			End Try
		End Sub

		Public Shared Function IsValidRequestString() As Boolean
			Dim isValid As Boolean = True

			Try
				Dim request As String = HttpContext.Current.Request.QueryString.ToString()

				If request.ToLower().IndexOf("<script") > -1 OrElse request.ToLower().IndexOf("javascript") > -1 Then
					Return False
				End If

			Catch ex As Exception
			End Try

			Return isValid
		End Function
	End Class
'End Namespace
