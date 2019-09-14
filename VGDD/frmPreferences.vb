Imports System.IO
Imports System.Xml
Imports VGDDCommon

Public Class frmPreferences

    Private Loaded As Boolean = False

    Private Sub frmPreferences_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        chkCopyBitmaps.Checked = Common.ProjectCopyBitmapsInVgddProjectFolder
        chkBmpPrefix.Checked = Common.ProjectUseBmpPrefix
        chkMakeBackups.Checked = Common.ProjectMakeBackups
        lblUserTemplatesFolder.Text = Common.UserTemplatesFolder
        txtJavaCommand.Text = Common.ProjectJavaCommand
        txtFallbackGRCPath.Text = Common.ProjectFallbackGRCPath
        Loaded = True
    End Sub

    Private Sub frmPreferences_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If VGDDCommon.Mal.MalPath.StartsWith("\\") Then
            My.Settings.MalPath = "\\" & VGDDCommon.Mal.MalPath.Substring(2).Replace("\\", "\").Replace("\", "\\")
        Else
            My.Settings.MalPath = VGDDCommon.Mal.MalPath.Replace("\\", "\").Replace("\", "\\")
        End If
        My.Settings.JavaCommand = txtJavaCommand.Text
        My.Settings.FallbackGRCPath = txtFallbackGRCPath.Text
        My.Settings.CopyBitmapsInVgddProjectFolder = chkCopyBitmaps.Checked
        My.Settings.ProjectUseBmpPrefix = chkBmpPrefix.Checked
        My.Settings.MyUserTemplatesFolder = Common.UserTemplatesFolder
        My.Settings.MyMakeBackups = chkMakeBackups.Checked
        My.Settings.Save()

        Common.ProjectCopyBitmapsInVgddProjectFolder = My.Settings.CopyBitmapsInVgddProjectFolder
        Common.ProjectUseBmpPrefix = My.Settings.ProjectUseBmpPrefix
        Common.ProjectJavaCommand = My.Settings.JavaCommand
        Common.ProjectMakeBackups = My.Settings.MyMakeBackups
        Common.ProjectFallbackGRCPath = My.Settings.FallbackGRCPath

    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        If VGDDCommon.Mal.ConfiguredFrameworks Is Nothing OrElse VGDDCommon.Mal.ConfiguredFrameworks.Count = 0 Then
            MessageBox.Show("No Frameworks configured, defaulting to MLA-Legacy")
            Mal.FrameworkName = "MLALegacy"
            Mal.SetTemplateName()
            CodeGen.LoadCodeGenTemplates()
        End If
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnSelectPathJava_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectPathJava.Click
        Dim fileDialog As New OpenFileDialog()
        fileDialog.Filter = "JRE java.exe|*.exe|All Files|*.*"
        fileDialog.RestoreDirectory = True
        If fileDialog.ShowDialog() = DialogResult.OK Then
            txtJavaCommand.Text = fileDialog.FileName & " -jar"
        End If
    End Sub

    Private Sub chkCopyBitmaps_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCopyBitmaps.CheckedChanged
        If Loaded Then oMainShell.ProjectChanged()
    End Sub

    Private Sub chkBmpPrefix_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkBmpPrefix.CheckedChanged
        Common.ResourcesToConvert = True
    End Sub

    Private Sub btnUserTemplates_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUserTemplates.Click
        Dim oFolderDialog As New FolderBrowserDialog()
        oFolderDialog.SelectedPath = Common.UserTemplatesFolder
        SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
        If oFolderDialog.ShowDialog() = DialogResult.OK Then
            Common.UserTemplatesFolder = oFolderDialog.SelectedPath
            lblUserTemplatesFolder.Text = Common.UserTemplatesFolder
            Dim strFolder As String = Path.Combine(Common.UserTemplatesFolder, "Boards")
            If Not Directory.Exists(strFolder) Then
                Try
                    Directory.CreateDirectory(strFolder)
                Catch ex As Exception
                    MessageBox.Show("Can't create folder " & strFolder & ":" & vbCrLf & ex.Message, "Error")
                End Try
            End If
        End If
    End Sub

    Private Sub lblUserTemplatesFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblUserTemplatesFolder.Click
        Try
            System.Diagnostics.Process.Start(lblUserTemplatesFolder.Text)
        Catch ex As Exception
            MessageBox.Show("Error running command " & vbCrLf & lblUserTemplatesFolder.Text & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnTestJava_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTestJava.Click
        Dim strCommand As String
        If txtJavaCommand.Text.Contains("-jar") Then
            strCommand = txtJavaCommand.Text.Split("-jar")(0)
        Else
            strCommand = txtJavaCommand.Text.Split(" ")(0)
        End If
        Dim strResult As String = Common.TestJava(strCommand)
        lblJavaLog.Text = strResult
        If strResult.StartsWith("OK:") Then
            lblJavaLog.ForeColor = Color.Green
        Else
            lblJavaLog.ForeColor = Color.Red
            If txtJavaCommand.Text <> "java.exe -jar" Then
                btnResetJavaCommand.Visible = True
            Else
                btnGetJava.Visible = True
            End If
        End If
    End Sub

    Private Sub btnResetJavaCommand_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetJavaCommand.Click
        txtJavaCommand.Text = "java.exe -jar"
        btnResetJavaCommand.Visible = False
    End Sub

    Private Sub btnGetJava_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetJava.Click
        Common.RunBrowser("https://java.com/download/")
        MessageBox.Show("Opening Java download site. Please install latest Java Runtime." & vbCrLf & vbCrLf & "After having installed Java, please close and re-run VGDD", "Java Download")
        Me.Close()
    End Sub

    Private Sub txtFallbackGRCPath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFallbackGRCPath.TextChanged
        If File.Exists(txtFallbackGRCPath.Text) Then
            txtFallbackGRCPath.ForeColor = Color.Green
        Else
            txtFallbackGRCPath.ForeColor = Color.Red
        End If
    End Sub

    Private Sub btnSelectFallbackGRCPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectFallbackGRCPath.Click
        Dim fileDialog As New OpenFileDialog()
        fileDialog.Title = "Path for fallback GRC jar"
        fileDialog.Filter = "Graphics Resources Converter|grc.jar"
        fileDialog.RestoreDirectory = True
        If fileDialog.ShowDialog() = DialogResult.OK Then
            txtFallbackGRCPath.Text = fileDialog.FileName
        End If
    End Sub
End Class