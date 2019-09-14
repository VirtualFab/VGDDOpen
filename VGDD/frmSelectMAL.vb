Imports System.IO

Public Class frmSelectMAL

    Private oFramework As VGDDCommon.VGDDFramework

    Private Sub frmSelectMAL_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If VGDDCommon.Mal.ConfiguredFrameworks Is Nothing Then
            VGDDCommon.Mal.ConfiguredFrameworks = New VGDDCommon.VGDDFrameworkCollection
        End If
        DisplayFrameworks()
        btnUseFramework.Enabled = txtPathMAL.Text <> String.Empty
        'txtPathMAL_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Private Sub DisplayFrameworks()
        txtPathMAL.Items.Clear()
        For Each oFramework As VGDDCommon.VGDDFramework In VGDDCommon.Mal.ConfiguredFrameworks
            txtPathMAL.Items.Add(oFramework.Description)
            If oFramework.FrameworkPath = VGDDCommon.Mal.MalPath Then
                txtPathMAL.Text = oFramework.Description
            End If
        Next
    End Sub

    Private Sub btnUseFramework_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUseFramework.Click
        oFramework = VGDDCommon.Mal.SelectMalFromDescription(txtPathMAL.Text)
    End Sub

    Private Sub btnSelectPathMAL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddFramework.Click
        frmSelectMALHint.Show()

        Dim fileDialog As New OpenFileDialog()
        fileDialog.Title = "Legacy MLA: ""mal.xml"", MLA: ""module_versions.xml"", Harmony:""gfx.h"""
        fileDialog.InitialDirectory = txtPathMAL.Text
        fileDialog.FileName = ""
        fileDialog.Filter = "MLA manifest/Graphics Library Header|mal.xml; module_versions.xml; gfx.h"
        fileDialog.RestoreDirectory = True
        If VGDDCommon.Common.ShowOpenFileDialogEx(Me, fileDialog) <> "" Then
            Dim strNewPath As String = Path.GetDirectoryName(fileDialog.FileName)
            Dim rc As String = VGDDCommon.Mal.CheckMalVersion(strNewPath)
            If rc = "OK" Then
                Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        strNewPath = strNewPath
                    Case "MLA"
                        strNewPath = Path.GetFullPath(Path.Combine(strNewPath, ".."))
                    Case "HARMONY"
                        strNewPath = Path.GetFullPath(Path.Combine(Path.Combine(strNewPath, ".."), ".."))
                End Select

                DisplayFrameworks()
                For Each oFramework In VGDDCommon.Mal.ConfiguredFrameworks
                    If oFramework.FrameworkPath = strNewPath Then
                        txtPathMAL.SelectedText = oFramework.Description
                        Exit Sub
                    End If
                Next
                oFramework = New VGDDCommon.VGDDFramework
                oFramework.FrameworkName = VGDDCommon.Mal.FrameworkName
                oFramework.FrameworkPath = strNewPath
                oFramework.MLAVersion = VGDDCommon.Mal.MalVersion
                VGDDCommon.Mal.ConfiguredFrameworks.Add(oFramework)
                My.Settings.ConfiguredFrameworks = VGDDCommon.Mal.ConfiguredFrameworks
                My.Settings.Save()
                frmSelectMAL_Load(Nothing, Nothing)
            End If
        End If
        frmSelectMALHint.Close()
    End Sub

    Private Sub txtPathMAL_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPathMAL.TextChanged
        btnRemove.Enabled = (txtPathMAL.Text <> String.Empty)
        btnUseFramework.Enabled = btnRemove.Enabled
    End Sub

    Private Sub txtPathMAL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPathMAL.SelectedIndexChanged
        For Each oFramework As VGDDCommon.VGDDFramework In VGDDCommon.Mal.ConfiguredFrameworks
            If txtPathMAL.Text = oFramework.Description Then
                Dim rc As String = VGDDCommon.Mal.CheckMalVersion(oFramework.FrameworkPath)
                If rc = "OK" Then
                    lblLog.Text = "Found MLA manifest - Version " & VGDDCommon.Mal.MalVersion & " V." & VGDDCommon.Mal.MalVersionNum
                    lblLog.ForeColor = Color.Green
                    VGDDCommon.Mal.MalPath = oFramework.FrameworkPath
                    btnUseFramework.Enabled = True
                    btnRemove.Enabled = True
                Else
                    lblLog.Text = rc
                    lblLog.ForeColor = Color.Red
                    btnUseFramework.Enabled = False
                    btnRemove.Enabled = True
                End If
            End If
        Next
    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        For i As Integer = 0 To VGDDCommon.Mal.ConfiguredFrameworks.Count - 1
            Dim oFramework As VGDDCommon.VGDDFramework = VGDDCommon.Mal.ConfiguredFrameworks(i)
            If txtPathMAL.Text = oFramework.Description Then
                VGDDCommon.Mal.ConfiguredFrameworks.RemoveAt(i)
                My.Settings.ConfiguredFrameworks = VGDDCommon.Mal.ConfiguredFrameworks
                My.Settings.Save()
                txtPathMAL.Text = String.Empty
                Exit For
            End If
        Next
        DisplayFrameworks()
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        VGDDCommon.Common.RunBrowser("http://www.microchip.com/mplab/microchip-libraries-for-applications")
        System.Threading.Thread.Sleep(2000)
        VGDDCommon.Common.RunBrowser("http://www.microchip.com/mplab/mplab-harmony")
    End Sub
End Class