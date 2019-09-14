Option Strict On

Imports System.IO
Imports System.Windows.Forms
Imports System.Threading
Imports System.ComponentModel
Imports System.Reflection

Public Class UpdateCheck
    Implements System.IDisposable

    Private Shared _CheckNewVersionUrl As String
    Public Event Ready()
    Public Event NewVersionAvailable()
    Public Shared LatestVersion As String
    Public Shared Checking As Boolean = False
    Private Shared responseData As String = ""
    Private WithEvents LmBgWorker As BackgroundWorker
    Public Shared SoftwareName As String = "VGDD OPEN"
    Public Shared SoftwareVersion As String = VGDDCommon.Common.VGDDVERSION

    Public Sub CheckNewVersion(ByVal strUrl As String)
        Try
            _CheckNewVersionUrl = strUrl
            LmBgWorker = New BackgroundWorker
            Me.LmBgWorker.RunWorkerAsync()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub LmBgWorker_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles LmBgWorker.DoWork
        CheckNewVersionAsync()
    End Sub

    Private Sub LmBgWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles LmBgWorker.RunWorkerCompleted
        If responseData <> String.Empty AndAlso responseData.Length < 128 AndAlso Not SoftwareVersion.StartsWith(responseData.Trim) Then
            LatestVersion = responseData.Trim
            Try
                If responseData > SoftwareVersion.Split(CChar(" "))(0) Then
                    RaiseEvent NewVersionAvailable()
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub CheckNewVersionAsync()
        Try
            System.Net.ServicePointManager.Expect100Continue = False ' http://stackoverflow.com/questions/566437/http-post-returns-the-error-417-expectation-failed-c
            Dim oHttpRequest As Net.HttpWebRequest = CType(Net.WebRequest.Create(_CheckNewVersionUrl), Net.HttpWebRequest)
            oHttpRequest.Accept = "*/*"
            oHttpRequest.AllowAutoRedirect = True
            oHttpRequest.UserAgent = SoftwareName & " " & SoftwareVersion
            oHttpRequest.Timeout = 60000
            oHttpRequest.Method = "POST"
            oHttpRequest.ContentType = "application/x-www-form-urlencoded"
            oHttpRequest.KeepAlive = False
            Dim encoding As New System.Text.ASCIIEncoding() 'Use UTF8Encoding for XML requests
            Dim strPost As String = "a=" & SoftwareName & "&v=" & SoftwareVersion
            Dim postByteArray() As Byte = encoding.GetBytes(strPost)
            oHttpRequest.ContentLength = postByteArray.Length
            Dim postStream As IO.Stream = oHttpRequest.GetRequestStream()
            postStream.Write(postByteArray, 0, postByteArray.Length)
            postStream.Close()
            Dim hwresponse As Net.HttpWebResponse = CType(oHttpRequest.GetResponse(), Net.HttpWebResponse)
            If hwresponse.StatusCode = Net.HttpStatusCode.OK Then
                Dim responseStream As IO.StreamReader =
                  New IO.StreamReader(hwresponse.GetResponseStream())
                responseData = responseStream.ReadToEnd()
            End If
            hwresponse.Close()
        Catch e As Exception
        End Try
    End Sub


#Region "IDisposable Support"
    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
#End Region
End Class
