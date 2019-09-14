Imports System.IO
Imports VGDDCommon
'Imports VGDDPlayerLib
Imports System.Windows.Forms

Module modMain
    Public Sub Main()
        Dim strFile As String = ""
        Dim strCommand As String = ""
        Dim currentDomain As AppDomain = AppDomain.CurrentDomain
        AddHandler currentDomain.UnhandledException, AddressOf MYExceptionHandler
        'AddHandler Application.ThreadException, AddressOf MYThreadHandler

        If My.Application.CommandLineArgs.Count > 0 Then strCommand = My.Application.CommandLineArgs(0)
        If strCommand <> String.Empty Then
            strFile = strCommand
            If Not File.Exists(strFile) Then
                MessageBox.Show("File " & strFile & " does not exist. Please choose one manually.", "VGDD Player Open Package")
                strFile = ""
            End If
        End If
        Try
            If strFile = "" Then
                Dim dlg As OpenFileDialog = New OpenFileDialog
                dlg.Title = "Open VGDD Player Package"
                dlg.DefaultExt = "vpp"
                dlg.Filter = "VGDD Player Package|*.vpp"
                If (dlg.ShowDialog = Windows.Forms.DialogResult.OK) Then
                    strFile = dlg.FileName
                Else
                    Application.Exit()
                End If
            End If
            If Not File.Exists(strFile) Then End
            Common.ProjectPathName = strFile
            Player.Mode = Player.RunMode.Standalone
            Player.PlayPackage(strFile)
            Application.Run()

        Catch ex As Exception
            MessageBox.Show("Player has encountered an exception:" & vbCrLf & ex.Message)
            Application.Exit()
        End Try
    End Sub

    Public Sub MYExceptionHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim FileName As String = ""
        Dim strFileMessage As String = ""
        Try
            Dim ex As Exception = e.ExceptionObject
            frmPlayer.Panel1.Controls.Clear()
            Dim oTextBox As New TextBox
            oTextBox.Dock = DockStyle.Fill
            oTextBox.Text = ex.Message & vbCrLf
            If ex.StackTrace IsNot Nothing Then
                oTextBox.Text &= vbCrLf & ex.StackTrace.ToString
            End If
        Catch ex As Exception
        End Try
    End Sub
End Module
