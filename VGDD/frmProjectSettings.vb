Imports System.IO
Imports System.Xml
Imports VGDDCommon

Public Class frmProjectSettings

    Private Loaded As Boolean = False

    Private Const VGAModes As String = "QQVGA (160x120)|HQVGA (240x160)|*QVGA (320x240)|*WQVGA1 (400x240)|*WQVGA2 (480x272)|HVGA (480x320)|nHD (640x360)|*VGA (640x480)|*WVGA (800x480)|FWVGA (854x480)|SVGA (800x600)|qHD (960x540)|DVGA (960x640)|WSVGA1 (1024x576)|WSVGA2 (1024x600)|XGA (1024x768)|HD (1280x720)|WXGA1 (1366x768)|WXGA2 (1360x768)|WXGA3 (1280x800)|XGA+ (1152x864)|WXGA+ (1440x900)|SXGA (1280x1024)|SXGA+ (1400x1050)|WSXGA+ (1680x1050)|UXGA (1600x1200)|FHD (1920x1080)|WUXGA (1920x1200)|QWXGA (2048x1152)|QXGA (2048x1536)|QHD (2560x1440)|WQXGA (2560x1600)|QSXGA (2560x2048)|WQXGA+ (3200x1800)|WQSXGA (3200x2048)|QUXGA (3200x2400)|UHD [4K] (3840x2160)|WQUXGA (3840x2400)|HXGA (4096x3072)|WHXGA (5120x3200)|HSXGA (5120x4096)|WHSXGA (6400x4096)|HUXGA (6400x4800)|UHD [8K] (7680x4320)|WHUXGA (7680x4800)"

    Private blnMultilanguageWarning As Boolean = False

    Private Sub frmOptions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Mal.MalPath = String.Empty Then
            frmPreferences.ShowDialog()
        End If
        Me.txtDefaultHeight.Text = Common.ProjectHeight
        Me.txtDefaultWidth.Text = Common.ProjectWidth
        btnMigrate.Enabled = False
        chkUseIndexedColours.Checked = Common.ProjectUsePalette
        chkUseMultiByteChars.Checked = Common.ProjectUseMultiByteChars
        MultiLanguageTranslations.Value = Common.ProjectMultiLanguageTranslations
        ActiveLanguage.Value = Common.ProjectActiveLanguage
        ActiveLanguage.Maximum = MultiLanguageTranslations.Value
        ActiveLanguage.Enabled = ActiveLanguage.Maximum > 0
        btnStringsPool.Enabled = ActiveLanguage.Maximum > 0
        chkStringsPoolHeader.Enabled = ActiveLanguage.Maximum > 0
        chkStringsPoolHeader.Checked = Common.ProjectStringsPoolGenerateHeader

        Select Case Common.ProjectColourDepth
            Case 1
                rbtColorDepth1.Checked = True
                chkUseIndexedColours.Checked = False
                Common.ProjectUsePalette = False
            Case 2
                rbtColorDepth2.Checked = True
                chkUseIndexedColours.Checked = False
                Common.ProjectUsePalette = False
            Case 4
                rbtColorDepth4.Checked = True
            Case 8
                rbtColorDepth8.Checked = True
            Case 16
                rbtColorDepth16.Checked = True
                chkUseIndexedColours.Checked = False
                Common.ProjectUsePalette = False
            Case 24
                rbtColorDepth24.Checked = True
                chkUseIndexedColours.Checked = False
                Common.ProjectUsePalette = False
        End Select
        Select Case Common.ProjectCompiler
            Case "C30"
                Me.rbtCompilerC30.Checked = True
            Case "C32"
                Me.rbtCompilerC32.Checked = True
            Case "XC16"
                Me.rbtCompilerXC16.Checked = True
            Case "XC32"
                Me.rbtCompilerXC32.Checked = True
        End Select
        btnMigrate.Visible = Common.aScreens.Count > 0
        LoadVgaModes()
        If Common.DoFadings Then Me.Opacity = 0
        Loaded = True
        rbtColorDepth_CheckedChanged(Nothing, Nothing)
        CheckCompilers()
    End Sub

    Private Sub LoadVgaModes()
        cmbQuick.Items.Clear()
        cmbQuick.Items.Add("")
        For Each strMode As String In VGAModes.Split("|")
            If strMode.StartsWith("*") Or chkExtendedModes.Checked Then
                If strMode.StartsWith("*") Then strMode = strMode.Substring(1)
                cmbQuick.Items.Add(strMode)
            End If
        Next
    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        My.Settings.DefaultHeight = Me.txtDefaultHeight.Text
        My.Settings.DefaultWidth = Me.txtDefaultWidth.Text
        My.Settings.DefaultColourDepth = Common.ProjectColourDepth
        If rbtCompilerC30.Checked Then
            My.Settings.DefaultCompiler = "C30"
            Common.ProjectCompiler = "C30"
            Common.ProjectCompilerFamily = "C30"
        ElseIf rbtCompilerC32.Checked Then
            My.Settings.DefaultCompiler = "C32"
            Common.ProjectCompiler = "C32"
            Common.ProjectCompilerFamily = "C32"
        ElseIf rbtCompilerXC16.Checked Then
            My.Settings.DefaultCompiler = "XC16"
            Common.ProjectCompiler = "XC16"
            Common.ProjectCompilerFamily = "C30"
        ElseIf rbtCompilerXC32.Checked Then
            My.Settings.DefaultCompiler = "XC32"
            Common.ProjectCompiler = "XC32"
            Common.ProjectCompilerFamily = "C32"
        End If
        My.Settings.Save()

        Common.ProjectWidth = Me.txtDefaultWidth.Text
        Common.ProjectHeight = Me.txtDefaultHeight.Text
        Common.ProjectUseMultiByteChars = chkUseMultiByteChars.Checked
        Common.ProjectMultiLanguageTranslations = MultiLanguageTranslations.Value
        Common.ProjectStringsPoolGenerateHeader = chkStringsPoolHeader.Checked

        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        Common.MplabXExtractTemplates()
        Me.Cursor = Cursors.Default

        Mal.CheckMalVersion(VGDDCommon.Mal.MalPath)
        CodeGen.LoadCodeGenTemplates()
        oMainShell._Toolbox.InitializeToolbox()

        Me.Close()
        If blnMultilanguageWarning Then
            Common.ProjectTouchAllScreens = True
            MessageBox.Show("Please close and reopen the project in order to activate MultiLanguage support.", "Multi-Language support activation")
        End If
    End Sub

    Private Sub rbtColorDepth_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtColorDepth1.CheckedChanged, rbtColorDepth2.CheckedChanged, rbtColorDepth8.CheckedChanged, rbtColorDepth16.CheckedChanged, rbtColorDepth24.CheckedChanged
        If Loaded Then
            If rbtColorDepth1.Checked Then
                Common.ProjectColourDepth = 1
                chkUseIndexedColours.Enabled = False
                'xxxyyy
            ElseIf rbtColorDepth2.Checked Then
                Common.ProjectColourDepth = 2
                chkUseIndexedColours.Enabled = False
            ElseIf rbtColorDepth4.Checked Then
                Common.ProjectColourDepth = 4
                chkUseIndexedColours.Enabled = True
            ElseIf rbtColorDepth8.Checked Then
                Common.ProjectColourDepth = 8
                chkUseIndexedColours.Enabled = True
            ElseIf rbtColorDepth16.Checked Then
                Common.ProjectColourDepth = 16
                chkUseIndexedColours.Enabled = False
                chkUseIndexedColours.Enabled = False
            ElseIf rbtColorDepth24.Checked Then
                Common.ProjectColourDepth = 24
                chkUseIndexedColours.Enabled = False
            End If
            Common.ProjectChanged = True
            Common.DesignScheme = Nothing
            'Common.CreateDesignScheme()
        End If
    End Sub

    Private Sub chkUseIndexedColours_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkUseIndexedColours.CheckedChanged
        Common.ProjectUsePalette = chkUseIndexedColours.Checked
        If Loaded Then oMainShell.ProjectChanged()
    End Sub

    Private Sub chkUseMultiByteChars_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkUseMultiByteChars.Click
        Common.ProjectUseMultiByteChars = chkUseMultiByteChars.Checked
        If Loaded Then oMainShell.ProjectChanged()
    End Sub

    Private Sub Framework1_FrameworkChanged() Handles Framework1.FrameworkChanged
        CheckCompilers()
        oMainShell.CheckMplabXToolstrip()
    End Sub

    Private Sub CheckCompilers()
        rbtCompilerC32.Enabled = True
        rbtCompilerXC32.Enabled = True
        rbtCompilerC30.Enabled = True
        rbtCompilerXC16.Enabled = True
        Select Case Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
            Case "MLA"
                rbtCompilerC32.Enabled = False
                rbtCompilerXC32.Enabled = False
                rbtCompilerC30.Enabled = False
                rbtCompilerXC16.Checked = True
            Case "HARMONY"
                rbtCompilerC32.Enabled = False
                rbtCompilerC30.Enabled = False
                rbtCompilerXC16.Enabled = False
                rbtCompilerXC32.Checked = True
        End Select
    End Sub

    Private Sub chkCopyBitmaps_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Loaded Then oMainShell.ProjectChanged()
    End Sub

    Private Sub chkUseMultiByteChars_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkUseMultiByteChars.CheckedChanged
        If Loaded Then oMainShell.ProjectChanged()
    End Sub

    Private Sub rbtCompiler_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtCompilerC30.CheckedChanged, rbtCompilerC32.CheckedChanged, rbtCompilerXC16.CheckedChanged, rbtCompilerXC32.CheckedChanged
        If Loaded Then oMainShell.ProjectChanged()
    End Sub

    Private Sub txtDefaultWidth_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultWidth.TextChanged
        If Loaded AndAlso Common.VGDDProjectPath <> "" Then oMainShell.ProjectChanged()
        btnMigrate.Enabled = True
    End Sub

    Private Sub txtDefaultHeight_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultHeight.TextChanged
        If Loaded AndAlso Common.VGDDProjectPath <> "" Then oMainShell.ProjectChanged()
        btnMigrate.Enabled = True
    End Sub

    Private Sub btnMigrate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMigrate.Click
        Try
            oMainShell.sngMigrateRatioWidth = txtDefaultWidth.Text / Common.ProjectWidth
            oMainShell.sngMigrateRatioHeight = txtDefaultHeight.Text / Common.ProjectHeight
            If MessageBox.Show("Do you want to proportionally change sizes and positions of EXISTING screens and their Widgets" & vbCrLf & _
                     "from " & Common.ProjectWidth & "x" & Common.ProjectHeight & " to " & txtDefaultWidth.Text & "x" & txtDefaultHeight.Text & "?", _
                     "Migrate/Resize entire project", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then
                If MessageBox.Show("Although the migration process happens in memory and nothing is going to be saved before you decide so, HAVE YOU MADE A BACKUP OF YOUR PROJECT?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) = vbNo Then
                    Exit Sub
                End If
                Me.Cursor = Cursors.WaitCursor
                oMainShell.OpenAllScreens()
                oMainShell.tmrMigrate = New Timer
                oMainShell.tmrMigrate.Interval = 1000
                oMainShell.tmrMigrate.Enabled = True
                btnOk.PerformClick()
                Application.DoEvents()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnPreferences_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPreferences.Click
        frmPreferences.ShowDialog()
    End Sub

    Private Sub chkExtendedModes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkExtendedModes.Click
        LoadVgaModes()
    End Sub

    Private Sub cmbQuick_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbQuick.SelectionChangeCommitted
        If cmbQuick.SelectedItem.ToString = String.Empty Then Exit Sub
        Dim strMode As String = cmbQuick.SelectedItem.ToString.Split("(")(1).Replace(")", "")
        Dim aRes() As String = strMode.Split("x")
        If chkSwapWH.Checked Then
            txtDefaultWidth.Text = aRes(1)
            txtDefaultHeight.Text = aRes(0)
        Else
            txtDefaultWidth.Text = aRes(0)
            txtDefaultHeight.Text = aRes(1)
        End If
    End Sub

    Private Sub chkSwapWH_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSwapWH.Click
        Dim s As String = txtDefaultHeight.Text
        txtDefaultHeight.Text = txtDefaultWidth.Text
        txtDefaultWidth.Text = s
    End Sub

    Private Sub MultiLanguageTranslations_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MultiLanguageTranslations.ValueChanged
        If Common.ProjectMultiLanguageTranslations = 0 And MultiLanguageTranslations.Value > 0 Then
            blnMultilanguageWarning = True
        End If
        Common.ProjectMultiLanguageTranslations = MultiLanguageTranslations.Value
        ActiveLanguage.Maximum = MultiLanguageTranslations.Value
        ActiveLanguage.Enabled = ActiveLanguage.Maximum > 0
        btnStringsPool.Enabled = ActiveLanguage.Maximum > 0
        chkStringsPoolHeader.Enabled = ActiveLanguage.Maximum > 0
        Common.ProjectChanged = True
    End Sub

    Private Sub ActiveLanguage_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveLanguage.ValueChanged
        Common.ProjectActiveLanguage = ActiveLanguage.Value
        Common.ProjectChanged = True
    End Sub

    Private Sub btnStringsPool_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStringsPool.Click
        Dim oSp As New StringsPool
        oSp.SelectOnly = False
        oSp.ShowDialog()
    End Sub

End Class