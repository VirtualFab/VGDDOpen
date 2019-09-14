Imports VGDDCommon
Imports System.Xml
Imports System.IO

Public Class frmMplabXSelectProject

    Private Sub frmMplabXSelectProject_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtProjectPath.Text = Common.MplabXProjectPath
        cmbMplabXConfig.Text = Common.MplabXSelectedConfig
        btnOk.Enabled = True
        CheckConfigs()
    End Sub

    Private Sub CheckConfigs()
        Try
            If Not Common.MplabxCheckProjectPath(Common.MplabXProjectPath) Then
                'btnOk.Enabled = False
                txtProjectPath.ForeColor = Color.Red
            Else
                'btnOk.Enabled = True
                txtProjectPath.ForeColor = Color.Green
                MplabX.LoadMplabxProject()
                cmbMplabXConfig.Items.Clear()
                cmbMplabXConfig.Text = ""
                For Each strConfigName As String In MplabX.ProjectConfigs
                    cmbMplabXConfig.Items.Add(strConfigName)
                    If cmbMplabXConfig.Text = "" Or strConfigName = Common.MplabXSelectedConfig Then
                        cmbMplabXConfig.Text = strConfigName
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnChooseProject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChooseProject.Click
        Dim dlg As New FolderBrowserDialog
        dlg.Description = "Open Existing MPLAB X Project"
        dlg.RootFolder = Environment.SpecialFolder.MyComputer
        dlg.ShowNewFolderButton = False
        If txtProjectPath.Text <> "" Then
            dlg.SelectedPath = txtProjectPath.Text
        Else
            dlg.SelectedPath = IIf(Common.MplabXProjectPath <> "", Common.MplabXProjectPath, Common.VGDDProjectPath)
        End If
        SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
        If dlg.ShowDialog = DialogResult.OK Then
            txtProjectPath.Text = dlg.SelectedPath
            cmbMplabXConfig.Items.Clear()
            cmbMplabXConfig.Text = ""
            If Not Common.MplabxCheckProjectPath(txtProjectPath.Text) Then
                'btnOk.Enabled = False
                txtProjectPath.ForeColor = Color.Red
            Else
                'btnOk.Enabled = True
                txtProjectPath.ForeColor = Color.Green
                Common.MplabXProjectPath = txtProjectPath.Text
                Try
                    CheckConfigs()
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

    Private Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Common.MplabXProjectPath = txtProjectPath.Text
        Common.MplabXSelectedConfig = cmbMplabXConfig.Text
        'Me.Close()
    End Sub

End Class