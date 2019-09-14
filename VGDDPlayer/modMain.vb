Imports System.IO
Imports VGDDCommon
Imports VGDDPlayerLib
Imports System.Windows.Forms

Module modMain
    Public Sub Main()
        Dim strFile As String = ""
        Dim strCommand As String = ""
        If My.Application.CommandLineArgs.Count > 0 Then strCommand = My.Application.CommandLineArgs(0)
        If strCommand <> String.Empty Then
            strFile = strCommand
            If Not File.Exists(strFile) Then
                MessageBox.Show("File " & strFile & " does not exist. Please choose one manually.", "VGDD Player Open Package")
                strFile = ""
            End If
        End If
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
        If strFile = "" Then Application.Exit()
        Common.ProjectPathName = strFile
        Player.Mode = Player.RunMode.Standalone
        Player.PlayPackage(strFile)
        Application.Run()
    End Sub

End Module
