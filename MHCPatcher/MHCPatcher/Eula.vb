Imports System.IO
Imports VirtualFabUtils.Utils
Imports System.Reflection

Public Class Eula

    Private Sub Eula_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim strNameSpace As String = Assembly.GetExecutingAssembly().ToString.Split(",")(0).Trim
            Dim oStream As IO.Stream = GetResource(strNameSpace & ".Eula.html", System.Reflection.Assembly.GetExecutingAssembly())
            Dim oReader As TextReader = New StreamReader(oStream)
            Dim strHtml As String = oReader.ReadToEnd
            WebBrowser1.DocumentText = strHtml
            Me.Focus()
            Me.TopMost = True
        Catch ex As Exception
            Me.Close()
        End Try
    End Sub

    Private Sub btnAgree_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAgree.Click
        Me.Close()
    End Sub

    Private Sub btnDisagree_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDisagree.Click
        Me.Close()
    End Sub
End Class