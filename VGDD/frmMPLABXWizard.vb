Imports System.IO
Imports System.Xml
Imports VGDDCommon
Imports System.Drawing
Imports VirtualFabUtils.Utils

Public Class frmMPLABXWizard

    Private blnDevBoardSelected As Boolean = False
    Private blnPIMSelected As Boolean = False
    Private blnExpBoardSelected As Boolean = False
    Private blnDispBoardSelected As Boolean = False

    Private bmpDisplay As Bitmap
    Private WizardLogMsgNum As Integer = 0

    Private blnLoadingWizard As Boolean = True

    Private oSelectedDevBoard As MplabX.DevelopmentBoard
    Private oSelectedExpBoard As MplabX.ExpansionBoard
    Private oSelectedDispBoard As MplabX.DisplayBoard
    Private oSelectedPimBoard As MplabX.PIMBoard

    'Private Sub frmMPLABXWizard_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
    '    If Me.Opacity = 0 Then
    '        FadeForm.FadeIn(Me, 99)
    '    End If
    'End Sub

    'Private Sub frmMPLABXWizard_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    '    FadeForm.FadeOutAndWait(Me)
    'End Sub

    Private Sub frmMPLABXWizard_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'CodeGenLocation1.BeginInit()
        pnlProject.Enabled = True
        Me.Cursor = Cursors.WaitCursor
        lstLog.Columns(0).Width = lstLog.Width - 100 - 40
        LoadTemplates()
        picDevBoard.SizeMode = PictureBoxSizeMode.Zoom
        picDevBoard.BackColor = Color.Transparent
        lblWarnGenerateCode.Visible = Not Common.CodeHasBeenGenerated
        txtHeapSize.Text = Common.ProjectHeapSize
        If Common.ProjectLastComputedHeapSize = 0 Then
            lblSuggestedHeap.Visible = False
        Else
            lblSuggestedHeap.Text = String.Format("Suggested Heap Size (footprint Heap * 1.5): {0} bytes", Common.ProjectLastComputedHeapSize * 1.5)
            lblSuggestedHeap.Visible = True
        End If
        CodeGenLocation1.Framework1.CheckMal()
        EnableBtnModfy()
        grpSkeletonCreate.Enabled = False

#If CONFIG <> "Debug" Then
        chkDebugInfo.Visible = False
        chkDebugInfo.Checked = False
#End If
        If Common.DoFadings Then Me.Opacity = 0

        CodeGen.ProjectUseGOL()

        'CheckGenerateOptions()
        'CheckHardwareDevAndExp()
        'GenerateOptions()
        'CheckAvailableOptions()
        rbBoard_CheckedChanged(Nothing, Nothing)
        SetSkeletonLabels()
        CodeGenLocation1_OptionsChanged(Nothing, Nothing)
        Me.Cursor = Cursors.Default
        blnLoadingWizard = False
    End Sub

    Private Sub LoadTemplates()
        Mal.CheckMalVersion(Mal.MalPath)
        WizardLogWrite("Loading Templates")
        If Not MplabX.LoadTemplates() Then
            Me.Close()
        End If
        WizardLogWrite(Nothing, "OK")
        WizardLogWrite("Checking Templates")
        pnlDevBoard.Controls.Clear()

        drpSelectPIM.Items.Clear()
        For Each oBoard As MplabX.PIMBoard In MplabX.PIMBoards.Values
            drpSelectPIM.Items.Add(oBoard.Description)
        Next
        Dim i As Integer = 0
        For Each oBoard As MplabX.DevelopmentBoard In MplabX.DevBoards.Values
            Dim rbDevBoard As New RadioButton
            With rbDevBoard
                .Text = oBoard.Description
                If oBoard.PartNumber <> "" Then .Text &= " (" & oBoard.PartNumber & ")"
                .Tag = oBoard
                .CheckAlign = ContentAlignment.TopLeft
                .Enabled = oBoard.Enabled
                AddHandler .MouseUp, AddressOf rbBoard_CheckedChanged
                AddHandler .CheckedChanged, AddressOf rbBoard_CheckedChanged
                .Top = (i) * 18
                .Left = 10
                .Height = 18
                .Width = grpDevBoard.Width - .Left - 10
                .Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Top
            End With
            pnlDevBoard.Controls.Add(rbDevBoard)
            i += 1
        Next
        pnlDevBoard.AutoScrollMinSize = New Size(240, pnlDevBoard.Controls.Count * 18)

        i = 0
        pnlExpBoard.Controls.Clear()
        For Each oBoard As MplabX.ExpansionBoard In MplabX.ExpBoards.Values
            Dim rbExpBoard As New RadioButton
            With rbExpBoard
                .Text = oBoard.Description
                If oBoard.PartNumber <> "" Then .Text &= " (" & oBoard.PartNumber & ")"
                .Tag = oBoard
                .CheckAlign = ContentAlignment.TopLeft
                .Enabled = oBoard.Enabled
                AddHandler .Click, AddressOf rbBoard_CheckedChanged
                AddHandler .CheckedChanged, AddressOf rbBoard_CheckedChanged
                .Top = i * 18
                .Left = 10
                .Height = 18
                .Width = grpDevBoard.Width - .Left - 10
                .Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Top
            End With
            pnlExpBoard.Controls.Add(rbExpBoard)
            i += 1
        Next
        pnlExpBoard.AutoScrollMinSize = New Size(240, pnlExpBoard.Controls.Count * 18)

        i = 0
        pnlDisplay.Controls.Clear()
        For Each oBoard As MplabX.DisplayBoard In MplabX.DisplayBoards.Values
            Dim rbDispBoard As New RadioButton
            With rbDispBoard
                .Text = oBoard.Description
                If oBoard.PartNumber <> "" Then .Text &= " (" & oBoard.PartNumber & ")"
                .Tag = oBoard
                .CheckAlign = ContentAlignment.TopLeft
                .Enabled = oBoard.Enabled
                AddHandler .Click, AddressOf rbBoard_CheckedChanged
                AddHandler .CheckedChanged, AddressOf rbBoard_CheckedChanged
                .Top = i * 18
                .Left = 10
                .Height = 18
                .Width = grpDevBoard.Width - .Left - 10
                .Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Top
            End With
            pnlDisplay.Controls.Add(rbDispBoard)
            i += 1
        Next
        pnlDisplay.AutoScrollMinSize = New Size(240, pnlDisplay.Controls.Count * 18)

        For Each rbDevBoard As RadioButton In pnlDevBoard.Controls
            Dim oBoard As MplabX.DevelopmentBoard = rbDevBoard.Tag
            If Common.DevelopmentBoardID = oBoard.ID Then
                rbDevBoard.Checked = True
                Exit For
            End If
        Next

        For Each rbExpBoard As RadioButton In pnlExpBoard.Controls
            Dim oBoard As MplabX.ExpansionBoard = rbExpBoard.Tag
            If Common.ExpansionBoardID = oBoard.ID Then
                rbExpBoard.Checked = True
                Exit For
            End If
        Next

        For Each rbDispBoard As RadioButton In pnlDisplay.Controls
            Dim oBoard As MplabX.DisplayBoard = rbDispBoard.Tag
            If Common.DisplayBoardID = oBoard.ID Then
                rbDispBoard.Checked = True
                Exit For
            End If
        Next

        Select Case Common.DisplayBoardOrientation
            Case 0, -1
                rbOrientation0.Checked = True
            Case 90
                rbOrientation90.Checked = True
            Case 180
                rbOrientation180.Checked = True
            Case 270
                rbOrientation270.Checked = True
        End Select

        CheckHardwareDevAndExp()

        WizardLogWrite(Nothing, "OK")
        'btnModifyProject.Enabled = False
        EnableBtnModfy()
    End Sub

    Public Sub GenerateOptions()
        pnlOptions.Controls.Clear()
        For Each oOptionNode As XmlNode In MplabX.MplabXTemplateDocEl.SelectNodes("Option")
            AddOption(oOptionNode)
        Next
        For Each oBoardNode As XmlNode In MplabX.MplabXTemplateDocEl.SelectNodes("/MplabXWizard/*/Board")
            For Each oOptionNode As XmlNode In oBoardNode.SelectNodes("Option")
                AddOption(oOptionNode)
            Next
        Next

        ' Set default values with check to AlternativeTo
        For Each oControl As Control In pnlOptions.Controls
            If TypeOf (oControl) Is VGDDWizardCheckBox Then
                Dim oChkOption As VGDDWizardCheckBox = oControl
                If oChkOption.DefaultValue = True And Not oChkOption.Checked Then
                    Dim blnChecked As Boolean = True
                    For Each strAltChkName As String In oChkOption.AlternativeTo.Split(",")
                        Dim oControls As Control() = Me.Controls.Find(strAltChkName.Trim, True)
                        If oControls.Length > 0 Then
                            Dim oAltChk As VGDDWizardCheckBox = oControls(0)
                            If oAltChk.Checked Then blnChecked = False
                            Exit For
                        End If
                    Next
                    If blnChecked Then
                        oChkOption.Checked = True
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub CheckAvailableOptions()
        Try
            For i As Integer = 0 To Common.WizardOptions.Keys.Count - 1
                Dim strOptionName As String = Common.WizardOptions.Keys(i)
                Dim aOptionControls As Control() = pnlOptions.Controls.Find(strOptionName, True)
                If aOptionControls.Length > 0 Then
                    Dim chkOption As VGDDWizardCheckBox = aOptionControls(0)
                    If Not chkOption.Enabled And chkOption.Checked Then
                        chkOption.Checked = False
                    End If
                    If strOptionName = "chkGOL" And Common.ProjectUseGol Then
                        If Not chkOption.Checked Then
                            chkOption.Checked = True
                            chkGenerateMainC.Checked = True
                            Common.ProjectChanged = True
                        End If
                    End If
                Else
                    If Common.WizardOptions(Common.WizardOptions.Keys(i)) = True Then
                        Common.WizardOptions.Set(Common.WizardOptions.Keys(i), False)
                    End If
                End If
            Next

            'Remove disabled options
            '"lbl" & oOptionNode.Attributes("Name").Value.Substring(3)
            If pnlOptions.Controls.Count > 0 Then
                Dim i As Integer = 0
                Do
                    Dim oControl As Control = pnlOptions.Controls(i)
                    If Not oControl.Enabled Then
                        pnlOptions.Controls.Remove(oControl)
                        'If TypeOf (oControl) Is VGDDWizardCheckBox Then
                        '    Common.WizardOptions.Remove(oControl.Name)
                        'End If
                    Else
                        oControl.Top = ((i / 2)) * 20
                        If TypeOf (oControl) Is Label Then
                            oControl.Top -= 6
                        End If
                        i += 1
                    End If
                Loop While i < pnlOptions.Controls.Count
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub AddOption(ByVal oOptionNode As XmlNode)
        Dim chkOption As New VGDDWizardCheckBox
        Dim lblOption As New Label
        With chkOption
            .Name = oOptionNode.Attributes("Name").Value
            .Left = 20
            .Top = ((pnlOptions.Controls.Count / 2) + 1) * 20
            .Width = 20
            .CheckAlign = ContentAlignment.MiddleLeft
            .Enabled = False
            If oOptionNode.Attributes("AlternativeTo") IsNot Nothing Then
                .AlternativeTo = oOptionNode.Attributes("AlternativeTo").Value.ToString.Trim
            End If
            If oOptionNode.Attributes("DependsOn") IsNot Nothing Then
                .DependsOn = oOptionNode.Attributes("DependsOn").Value.ToString.Trim
            End If
            If oOptionNode.Attributes("Default") IsNot Nothing Then
                .DefaultValue = oOptionNode.Attributes("Default").Value.ToString.Trim
            End If
            If oOptionNode.Attributes("AlwaysAvailable") IsNot Nothing Then
                .AlwaysAvailable = oOptionNode.Attributes("AlwaysAvailable").Value.ToString.Trim
                If .AlwaysAvailable Then
                    .Enabled = True
                End If
            End If

            If Common.WizardOptions(chkOption.Name) IsNot Nothing Then
                .Checked = Common.WizardOptions(.Name)
            Else
                Common.WizardOptions.Add(chkOption.Name, False)
            End If
            AddHandler .CheckStateChanged, AddressOf Option_CheckedChanged
        End With
        With lblOption
            .Name = "lbl" & oOptionNode.Attributes("Name").Value.Substring(3)
            .Text = oOptionNode.Attributes("Description").Value
            .Left = 40
            .Top = chkOption.Top + 6
            .Height = 20
            .Width = pnlOptions.Width - .Left - 40
            .Font = New Font(.Font, FontStyle.Underline)
            .ForeColor = Color.Blue
            .Tag = oOptionNode.InnerText
            .Enabled = False
            If chkOption.AlwaysAvailable Then
                .Enabled = True
            End If
            AddHandler .Click, AddressOf Option_DisplayHelp
        End With
        pnlOptions.Controls.Add(chkOption)
        pnlOptions.Controls.Add(lblOption)
    End Sub

    Private Sub Option_DisplayHelp(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim oLbl As Label = sender
        txtOptionHelp.Text = ""
        If oLbl.Tag IsNot Nothing Then
            txtOptionHelp.Text = oLbl.Tag
        End If
    End Sub

    Private Sub Option_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim oChk As VGDDWizardCheckBox = sender
        If oChk.Checked AndAlso oChk.AlternativeTo IsNot Nothing Then
            For Each strAltChkName As String In oChk.AlternativeTo.Split(",")
                Dim oControls As Control() = Me.Controls.Find(strAltChkName.Trim, True)
                If oControls.Length > 0 Then
                    Dim oAltChk As VGDDWizardCheckBox = oControls(0)
                    oAltChk.Checked = False
                End If
            Next
        End If
        If oChk.Checked AndAlso oChk.DependsOn IsNot Nothing Then
            For Each strAltChkName As String In oChk.DependsOn.Split(",")
                Dim oControls As Control() = Me.Controls.Find(strAltChkName.Trim, True)
                If oControls.Length > 0 Then
                    Dim oAltChk As VGDDWizardCheckBox = oControls(0)
                    oAltChk.Checked = True
                End If
            Next
        End If
        Common.WizardOptions(oChk.Name) = oChk.Checked
        If Not blnLoadingWizard AndAlso (Common.WizardOptions(oChk.Name) IsNot Nothing AndAlso CBool(Common.WizardOptions(oChk.Name)) <> oChk.Checked) Then
            Common.ProjectChanged = True
        End If
    End Sub

    Private Sub WizardLogClear()
        WizardLogMsgNum = 0
        lstLog.Items.Clear()
    End Sub

    Private Sub ModifySkeletonFile(ByVal FileName As String, ByVal ClearSection As Boolean)
        Dim strLog As String = MplabX.ModifySkeletonFile(FileName, ClearSection)
        WizardLogWrite(strLog)
        WizardLogWrite(Nothing, "OK")
    End Sub

    Private Sub btnModifyProject_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnModifyProject.Click
        Dim strLog As String = String.Empty
        Me.Cursor = Cursors.WaitCursor
        WizardLogClear()
        Common.WizardWarnings = String.Empty
        blnLoadingWizard = True
        LoadTemplates()
        blnLoadingWizard = False
        MplabX.LoadMplabxProject()
        MplabX.OverridesList.Clear()
        'Dim oMplabXExternalFolderNode As XmlNode = MplabXGetSubFolder(oMplabXRootFolderNode, "ExternalFiles", True)
        'Dim oMplabXSourceFolderNode As XmlNode = MplabXGetSubFolder(oMplabXRootFolderNode, "SourceFiles", True)
        'Dim oMplabXHeadersFolderNode As XmlNode = MplabXGetSubFolder(oMplabXRootFolderNode, "HeaderFiles", True)

        Dim blnAllChecksOk As Boolean = True
        For Each oCheckNode As XmlNode In MplabX.MplabXTemplateDocEl.SelectNodes("Checks")
            Dim blnCheck As Boolean = False

            Dim oAttr As XmlAttribute
            oAttr = oCheckNode.Attributes("ExpBoardIds")
            If oAttr IsNot Nothing AndAlso oAttr.Value.Contains(Common.ExpansionBoardID) Then
                'TODO: Checks!
            End If

        Next

        MplabXClearFolders(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DevelopmentBoards/Board[@ID='{0}']/Project/ClearFolder", Common.DevelopmentBoardID)))
        MplabXClearFolders(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("ExpansionBoards/Board[@ID='{0}']/Project/ClearFolder", Common.ExpansionBoardID)))
        MplabXClearFolders(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DisplayBoards/Board[@ID='{0}']/Project/ClearFolder", Common.DisplayBoardID)))

        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                If Not File.Exists(Common.ProjectFileName_EventsHelperH) Then
                    ExtractResourceToFile("VGDDEventsHelper.h", Common.ProjectFileName_EventsHelperH, System.Reflection.Assembly.GetExecutingAssembly())
                End If
                MplabX.MplabXAddFileIfNotExist("Source Files/VGDD", Common.ProjectFileName_ScreensC)
                MplabX.MplabXAddFileIfNotExist("Source Files/VGDD", Common.ProjectFileName_InternalResourcesC)
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Common.ProjectFileName_ScreensH)
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Common.ProjectFileName_ScreenStatesH)
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Common.ProjectFileName_InternalResourcesH)
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Common.ProjectFileName_EventsHelperH)
            Case "MLA"
                Dim strDestFolderName As String
                strDestFolderName = String.Format("Source Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_ScreensC)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_InternalResourcesC)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_InternalResourcesRefC)
                strDestFolderName = String.Format("Header Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_ScreensH)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_ScreenStatesH)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_InternalResourcesH)
            Case "HARMONY"
                Dim strDestFolderName As String
                strDestFolderName = String.Format("Source Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_ScreensC)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_InternalResourcesC)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName, Common.ProjectFileName_InternalResourcesRefC)
                strDestFolderName = String.Format("Header Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig)
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Common.ProjectFileName_ScreensH)
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Common.ProjectFileName_ScreenStatesH)
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Common.ProjectFileName_InternalResourcesH)
        End Select
        WizardFlushLog()

        Dim strDestPath As String = Common.CodeGenDestPath
        If Not Directory.Exists(strDestPath) Then
            Directory.CreateDirectory(strDestPath)
        End If

        If chkGenerateSkeleton.Checked Then
            If chkGenerateMainC.Checked Then
                If File.Exists(Path.Combine(strDestPath, "vgdd_main.c")) Then
                    If MessageBox.Show("Warning: a vgdd_main.c already exists in the project path. Overwrite it?", "Overwrite main?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.No Then
                        WizardLogWrite(Nothing, "KO")
                        CustomTabControl1.SelectedTab = tabOptions
                        MessageBox.Show("OK, vgdd_main.c has NOT been overwritten. Please uncheck the relative Generate Skeleton Files checkbox, revise your choices and proceed.")
                        Me.Cursor = Cursors.Default
                        Exit Sub
                    End If
                End If
                Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        GenerateSkeletonFile("VGDDmain_MLALegacy.c", "vgdd_main.c", strDestPath)
                        GenerateSkeletonFile("VGDDmain_MLALegacy.h", "vgdd_main.h", strDestPath)
                    Case "MLA"
                        GenerateSkeletonFile("VGDDmain_MLA.c", "vgdd_main.c", strDestPath)
                        GenerateSkeletonFile("VGDDmain_MLA.h", "vgdd_main.h", strDestPath)
                    Case "HARMONY"
                        GenerateSkeletonFile("VGDDmain_Harmony.c", "vgdd_main.c", strDestPath)
                        GenerateSkeletonFile("VGDDmain_Harmony.h", "vgdd_main.h", strDestPath)
                End Select
            End If
            If chkGenerateGraphicsConfig.Checked Then
                Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        GenerateSkeletonFile("GraphicsConfig.h", "GraphicsConfig.h", strDestPath)
                    Case "MLA"
                        GenerateSkeletonFile("gfx_config.h", "gfx_config.h", strDestPath)
                    Case "HARMONY"
                        GenerateSkeletonFile("gfx_config.h", "gfx_config.h", strDestPath)
                End Select
            End If
            If chkGenerateHardwareProfile.Checked Then
                Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        'GenerateSkeletonFile("HardwareProfile.h", "HardwareProfile.h", strDestPath)
                        If chkGenerateGraphicsConfig.Checked Then
                            GenerateSkeletonFile("GraphicsConfig.h", "GraphicsConfig.h", strDestPath)
                        End If
                        If chkGenerateHardwareProfile.Checked Then
                            GenerateSkeletonFile("HardwareProfile.h", "HardwareProfile.h", strDestPath)
                        End If
                    Case "MLA"
                        GenerateSkeletonFile("system.c", "system.c", strDestPath)
                        GenerateSkeletonFile("system.h", "system.h", strDestPath)
                        GenerateSkeletonFile("system_config.h", "system_config.h", strDestPath)
                    Case "HARMONY"
                        GenerateSkeletonFile("app.c", "app.c", strDestPath)
                        GenerateSkeletonFile("app.h", "app.h", strDestPath)
                        GenerateSkeletonFile("system_init.c", "system_init.c", strDestPath)
                        GenerateSkeletonFile("system_config.c", "system_config.c", strDestPath)
                        GenerateSkeletonFile("system_interrupt.c", "system_interrupt.c", strDestPath)
                        GenerateSkeletonFile("system_tasks.c", "system_tasks.c", strDestPath)
                        GenerateSkeletonFile("system_config_harmony.h", "system_config.h", strDestPath)
                        GenerateSkeletonFile("system_definitions.h", "system_definitions.h", strDestPath)
                        'GenerateSkeletonFile("timer_tick.c", "timer_tick.c", strDestPath)
                End Select
            End If
        End If

        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                MplabX.MplabXAddFileIfNotExist("Source Files/VGDD", Path.Combine(strDestPath, "vgdd_main.c"))
                MplabX.MplabXAddFileIfNotExist("Header Files/VGDD", Path.Combine(strDestPath, "vgdd_main.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files", Path.Combine(strDestPath, "GraphicsConfig.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files", Path.Combine(strDestPath, "HardwareProfile.h"))
            Case "MLA"
                MplabX.MplabXAddFileIfNotExist("Source Files/appMLA", Path.Combine(strDestPath, "vgdd_main.c"))
                MplabX.MplabXAddFileIfNotExist("Header Files/appMLA", Path.Combine(strDestPath, "vgdd_main.h"))
                MplabX.MplabXAddFileIfNotExist("Source Files/appMLA", Path.Combine(strDestPath, "system.c"))
                MplabX.MplabXAddFileIfNotExist("Header Files/appMLA", Path.Combine(strDestPath, "system.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files/appMLA", Path.Combine(strDestPath, "gfx_config.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files/appMLA", Path.Combine(strDestPath, "system_config.h"))
            Case "HARMONY"
                Dim strDestFolderName As String
                strDestFolderName = String.Format("Source Files/app/system_config/{0}", Common.MplabXSelectedConfig)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName & "/vgdd", Path.Combine(strDestPath, "vgdd_main.c"))
                MplabX.MplabXAddFileIfNotExist("Source Files/app", Path.Combine(strDestPath, "app.c"))
                MplabX.MplabXAddFileIfNotExist("Source Files/app/system_config", Path.Combine(strDestPath, "system_config.c"))
                MplabX.MplabXAddFileIfNotExist("Source Files/app/system_config", Path.Combine(strDestPath, "system_init.c"))
                MplabX.MplabXAddFileIfNotExist("Source Files/app/system_config", Path.Combine(strDestPath, "system_interrupt.c"))
                MplabX.MplabXAddFileIfNotExist("Source Files/app/system_config", Path.Combine(strDestPath, "system_tasks.c"))
                strDestFolderName = String.Format("Header Files/app/system_config/{0}", Common.MplabXSelectedConfig)
                MplabX.MplabXAddFileIfNotExist(strDestFolderName & "/vgdd", Path.Combine(strDestPath, "vgdd_main.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files/app", Path.Combine(strDestPath, "app.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files", Path.Combine(strDestPath, "gfx_config.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files", Path.Combine(strDestPath, "system_config.h"))
                MplabX.MplabXAddFileIfNotExist("Header Files", Path.Combine(strDestPath, "system_definitions.h"))
        End Select
        WizardFlushLog()

        Common.WizardForceAddVgddFiles = chkGenerateVGDDFiles.Checked

        'If Common.ProjectUseGol Then
        '    MplabX.MplabXAddProjectFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("FixedGroups/FixedGroup[@Name='{0}']/Project/Folder", "GOL")))
        'End If

        MplabX.MplabXAddProjectFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DevelopmentBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/Folder", Common.DevelopmentBoardID)))
        If Common.PIMBoardID <> "" Then
            MplabX.MplabXAddProjectFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("PIMBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/Folder", Common.PIMBoardID)))
        End If
        If Common.ExpansionBoardID <> "" Then
            MplabX.MplabXAddProjectFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("ExpansionBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/Folder", Common.ExpansionBoardID)))
        End If
        MplabX.MplabXAddProjectFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DisplayBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/Folder", Common.DisplayBoardID)))

        aGroupFilesToAdd.Clear()
        MplabXAddGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DevelopmentBoards/Board[@ID='{0}']/AddGroup", Common.DevelopmentBoardID)))
        If Common.PIMBoardID <> "" Then
            MplabXAddGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("PIMBoards/Board[@ID='{0}']/AddGroup", Common.PIMBoardID)))
        End If
        If Common.ExpansionBoardID <> "" Then
            MplabXAddGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("ExpansionBoards/Board[@ID='{0}']/AddGroup", Common.ExpansionBoardID)))
        End If
        MplabXAddGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DisplayBoards/Board[@ID='{0}']/AddGroup", Common.DisplayBoardID)))

        MplabXRemoveGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DevelopmentBoards/Board[@ID='{0}']/RemoveGroup", Common.DevelopmentBoardID)))
        If Common.PIMBoardID <> "" Then
            MplabXRemoveGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("PIMBoards/Board[@ID='{0}']/RemoveGroup", Common.PIMBoardID)))
        End If
        If Common.ExpansionBoardID <> "" Then
            MplabXRemoveGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("ExpansionBoards/Board[@ID='{0}']/RemoveGroup", Common.ExpansionBoardID)))
        End If
        MplabXRemoveGroupFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DisplayBoards/Board[@ID='{0}']/RemoveGroup", Common.DisplayBoardID)))
        MplabXAddGroupFiles(Nothing)

        MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "vgdd_main.c"), True)
        MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "vgdd_main.h"), True)
        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "GraphicsConfig.h"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "HardwareProfile.h"), True)
            Case "MLA"
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system.c"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system.h"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "gfx_config.h"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system_config.h"), True)
            Case "HARMONY"
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "app.h"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "app.c"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system_config.c"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system_init.c"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system_config.h"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system_interrupt.c"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system_definitions.h"), True)
                MplabX.ModifySkeletonFile(Path.Combine(strDestPath, "system_tasks.c"), True)
        End Select

        WizardFlushLog()

        'Dim strCompiler As String = XmlMplabxTemplatesDoc.DocumentElement.SelectSingleNode(String.Format("DevelopmentBoards/Board[@ID='{0}']", cmbDevBoards.SelectedValue)).Attributes("Compiler").Value
        Dim strCodeGenPath As String = ""
        Select Case Common.CodeGenLocation
            Case 1 'In MPLABX Project's Folder
                strCodeGenPath = "."
            Case 2 'In VGDD Project's Folder
                strCodeGenPath = RelativePath.Evaluate(Common.MplabXProjectPath, Common.CodeGenDestPath).Replace("\", "/")
            Case 3 'In MPLABX Project's Parent Folder (suggested)
                strCodeGenPath = ".."
            Case 4 'Other:
                strCodeGenPath = RelativePath.Evaluate(Common.MplabXProjectPath, Common.CodeGenDestPath).Replace("\", "/")
        End Select
        If strCodeGenPath = String.Empty Then
            strCodeGenPath = "."
        End If
        strCodeGenPath &= ";"
        MplabX.IncludeSearchPath = strCodeGenPath
        Dim strRelDestPath As String = RelativePath.Evaluate(Common.MplabXProjectPath, Common.CodeGenDestPath).Replace("\", "/")
        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                MplabX.IncludeSearchPath &= strRelDestPath & ";"
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/../Graphics/Common;").Replace("Microchip/../", "")
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/../Board Support Package;").Replace("Microchip/../", "")
                MplabX.IncludeSearchPath &= MplabX.MalRelativePath & "/Include;"
                MplabX.IncludeSearchPath &= MplabX.MalRelativePath & "/Include/Graphics;"
                MplabX.IncludeSearchPath &= "./;../"
            Case "MLA"
                MplabX.IncludeSearchPath &= strRelDestPath & ";"
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/../src;")
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/framework;")
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/framework/gfx;")
            Case "HARMONY"
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/../src;")
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/framework;")
                MplabX.IncludeSearchPath &= (MplabX.MalRelativePath & "/framework/gfx;")

        End Select

        MplabX.MplabXAddIncludeSearch(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DevelopmentBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/AddIncludeSearchPath", Common.DevelopmentBoardID)))
        If Common.ExpansionBoardID <> "" Then
            MplabX.MplabXAddIncludeSearch(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("ExpansionBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/AddIncludeSearchPath", Common.ExpansionBoardID)))
        End If
        If Common.PIMBoardID <> "" Then
            MplabX.MplabXAddIncludeSearch(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("PIMBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/AddIncludeSearchPath", Common.PIMBoardID)))
        End If
        MplabX.MplabXAddIncludeSearch(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("DisplayBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/AddIncludeSearchPath", Common.DisplayBoardID)))

        If Common.MplabXSelectedConfig Is Nothing OrElse Common.MplabXSelectedConfig = "" Then
            Common.MplabXSelectedConfig = "default"
        End If
        If MplabX.MplabXSetConfProperty(Common.MplabXSelectedConfig, "", "", "extra-include-directories", MplabX.IncludeSearchPath, strLog) Is Nothing Then
            MessageBox.Show("Cannot set Project Property ""extra-include-directories"": MPLAB X Project corrupted. Please check/recreate and then re-launch Wizard", "Error")
            Me.Cursor = Cursors.Default
            Exit Sub
        End If
        'If Common.ProjectCompilerFamily = "XC32" Then
        '    MplabX.MplabXSetConfProperty(Common.MplabXSelectedConfig, "C32-AS", "", "extra-include-directories-for-preprocessor", MplabX.IncludeSearchPath, strLog)
        'End If
        For Each strVerb As String In "SetConfig,AddConfig".Split(",")
            Dim aXPaths() As String = { _
            String.Format("DevelopmentBoards/Board[@ID='{0}']/Project/{1}", Common.DevelopmentBoardID, strVerb),
            String.Format("DevelopmentBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/{1}", Common.DevelopmentBoardID, strVerb),
            String.Format("ExpansionBoards/Board[@ID='{0}']/Project/{1}", Common.ExpansionBoardID, strVerb),
            String.Format("ExpansionBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/{1}", Common.ExpansionBoardID, strVerb),
            String.Format("DisplayBoards/Board[@ID='{0}']/Project/{1}", Common.DisplayBoardID, strVerb),
            String.Format("DisplayBoards/Board[@ID='{0}']/Project/" & VGDDCommon.Mal.FrameworkName & "/{1}", Common.DisplayBoardID, strVerb)
        }
            For Each XPath As String In aXPaths
                For Each oNode As XmlNode In MplabX.MplabXTemplateDocEl.SelectNodes(XPath)
                    Dim blnApply As Boolean = True
                    Dim strSection As String = oNode.Attributes("Section").Value.Replace("[COMPILER]", Common.ProjectCompilerFamily)
                    Select Case strSection
                        Case "C30", "C30-LD"
                            If Common.ProjectCompiler <> "C30" AndAlso Common.ProjectCompiler <> "XC16" Then
                                blnApply = False
                            End If
                        Case "C32", "C32-LD"
                            If Common.ProjectCompiler <> "C32" AndAlso Common.ProjectCompiler <> "XC32" Then
                                blnApply = False
                            End If
                    End Select
                    If blnApply Then
                        Dim strValue As String = oNode.Attributes("value").Value.Replace("[HEAPSIZE]", txtHeapSize.Text)
                        If strVerb = "SetConfig" Then
                            If MplabX.MplabXSetConfProperty(Common.MplabXSelectedConfig, strSection, "", oNode.Attributes("key").Value, strValue, strLog) Is Nothing Then
                                Me.Cursor = Cursors.Default
                                Exit Sub
                            End If
                        Else
                            Dim strCurValue As String = MplabX.MplabXGetConfProperty(Common.MplabXSelectedConfig, strSection, "", oNode.Attributes("key").Value)
                            If Not strCurValue.Contains(strValue) Then
                                strValue = strCurValue & ";" & strValue
                                If MplabX.MplabXSetConfProperty(Common.MplabXSelectedConfig, strSection, "", oNode.Attributes("key").Value, strValue, strLog) Is Nothing Then
                                    Me.Cursor = Cursors.Default
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                Next
            Next
        Next
        WizardFlushLog()

        If MplabX.OverridesList.Count > 0 Then
            MplabX.MplabXApplyOverrides()
        End If

        If MplabX.oMplabxIpc IsNot Nothing AndAlso MplabX.oMplabxIpc.IsConnected Then
            Me.Cursor = Cursors.Default
            MessageBox.Show("Successfully modified MPLAB X project via VGDD-Link plug-in." & vbCrLf & "Now you can execute the last step - Generate Code - and then switch back to MPLAB X and run your project.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Question)
            chkGenerateSkeleton.Checked = False
        Else
            If chkUpdateSourceRoots.Checked Then
                MplabXUpdateSourceRootList(strCodeGenPath)
            End If

            'Dim oConfNode As XmlNode = MplabX.oProjectXmlDoc.SelectSingleNode(String.Format("configurationDescriptor/confs/conf[@name='{0}']/toolsSet/targetHeader", "default"))
            'If oConfNode IsNot Nothing Then
            '    oConfNode.InnerXml = ""
            'End If

            Dim strDir As String = Path.Combine(Path.Combine(Path.Combine(Common.MplabXProjectPath, "dist"), Common.MplabXSelectedConfig), "production")
            If Not Directory.Exists(strDir) Then
                Directory.CreateDirectory(strDir)
            End If

            Dim NewXml As New XmlDocument
            NewXml.InnerXml = MplabX.CleanMplabXProject(MplabX.oProjectXmlDoc.InnerXml)
            '#If DEBUG Then
            '        Using sw As New XmlTextWriter(Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "VGDDOriginal.xml"), New System.Text.UnicodeEncoding)
            '            sw.Formatting = Formatting.Indented
            '            MplabX.OriginalProjectXml.WriteTo(sw)
            '            sw.Flush()
            '            sw.Close()
            '        End Using
            '        Using sw As New XmlTextWriter(Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "VGDDNew.xml"), New System.Text.UnicodeEncoding)
            '            sw.Formatting = Formatting.Indented
            '            NewXml.WriteTo(sw)
            '            sw.Flush()
            '            sw.Close()
            '        End Using
            '#End If

            If MplabX.OriginalProjectXml.InnerXml <> NewXml.InnerXml Then
                If Not MplabX.SaveMplabXProject() Then
                    WizardFlushLog()
                    Me.Cursor = Cursors.Default
                    Exit Sub
                End If
                WizardFlushLog()

                WizardLogWrite("Clearing MPLAB X MakeFile to force recreation")
                If Not MplabX.MplabXClearMakeFile() Then
                    WizardLogWrite(Nothing, "KO")
                    Me.Cursor = Cursors.Default
                    Exit Sub
                End If
                WizardLogWrite(Nothing, "OK")

                Me.Cursor = Cursors.Default
                MessageBox.Show("Successfully modified MPLAB X project." & vbCrLf & "Now you can launch MPLAB X and run your project." & vbCrLf & "Was MPLAB X project CLOSED while I was working? If not, modifications will not be effective until you close and restart MPLAB X!", "Success - Check MPLAB X", MessageBoxButtons.OK, MessageBoxIcon.Question)
                chkGenerateSkeleton.Checked = False
            Else
                Me.Cursor = Cursors.Default
                WizardLogWrite("No modifications to write to MPLAB X Project File " & Common.MplabXProjectXmlPathName)
                MessageBox.Show("Successfully modified skeleton files. MPLAB X project didn't need to be modified," & vbCrLf & "so now you can go back to MPLAB X and directly run your project.", "Success", MessageBoxButtons.OK, MessageBoxIcon.None)
                chkGenerateSkeleton.Checked = False
            End If
        End If
        Me.Cursor = Cursors.Default
        chkGenerateVGDDFiles.Checked = False
        btnNext4ModifyProject.Enabled = True
        btnGenerateCode.Enabled = True
        If Common.WizardWarnings <> String.Empty Then
            MessageBox.Show(Common.WizardWarnings, "Warnings")
        End If
    End Sub

    Private Sub chkGenerateSkeleton_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkGenerateSkeleton.CheckedChanged
        If chkGenerateSkeleton.Checked Then
            grpSkeletonCreate.Enabled = True
            chkModifySkeleton.Checked = True
            chkGenerateVGDDFiles.Checked = True
        Else
            grpSkeletonCreate.Enabled = False
            chkGenerateMainC.Checked = False
            chkGenerateGraphicsConfig.Checked = False
            chkGenerateHardwareProfile.Checked = False
            chkGenerateVGDDFiles.Checked = False
        End If
    End Sub

    Private Sub chkModifySkeleton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkModifySkeleton.CheckedChanged
        If chkModifySkeleton.Checked Then
            grpSkeletonModify.Enabled = True
            chkModifyMainC.Checked = True
            chkModifyGraphicsConfig.Checked = True
            chkModifyHardwareProfile.Checked = True
        Else
            grpSkeletonModify.Enabled = False
            chkModifyMainC.Checked = False
            chkModifyGraphicsConfig.Checked = False
            chkModifyHardwareProfile.Checked = False
        End If
    End Sub

    Public Sub WizardLogWrite(ByVal Message As String)
        WizardLogWrite(Message, Nothing)
    End Sub

    Public Sub WizardLogWrite(ByVal Message As String, ByVal Result As String)
        If Result Is Nothing Then
            WizardLogMsgNum += 1
            Message = WizardLogMsgNum.ToString & "-" & Message & "..."
        End If
        Dim oItem As ListViewItem
        If Message IsNot Nothing Then
            oItem = New ListViewItem(Message)
        Else
            oItem = lstLog.Items(lstLog.Items.Count - 1)
        End If
        If Result IsNot Nothing Then
            If oItem.Text.EndsWith("...") Then
                oItem.Text = oItem.Text.Substring(0, oItem.Text.Length - 3)
            End If
            Dim oSubItem As New ListViewItem.ListViewSubItem(oItem, Result)
            If Result.StartsWith("KO") Then
                oItem.BackColor = Color.DarkRed
                oItem.ForeColor = Color.White
            End If
            oItem.SubItems.Add(oSubItem)
        End If
        If Message IsNot Nothing Then
            lstLog.Items.Add(oItem)
        End If
        lstLog.EnsureVisible(lstLog.Items.Count - 1)
        Application.DoEvents()
    End Sub

    Private Sub EnableCompatibleOption(ByVal strOptionName As String)
        If strOptionName.Trim <> String.Empty Then
            Dim aOptionControls As Control() = pnlOptions.Controls.Find(strOptionName, True)
            If aOptionControls.Length > 0 Then
                Dim chkOption As CheckBox = aOptionControls(0)
                chkOption.Enabled = True
                Dim aControl As Control() = pnlOptions.Controls.Find("lbl" & strOptionName.Substring(3), True)
                If aControl.Length > 0 Then
                    aControl(0).Enabled = True
                End If
            End If
        End If
    End Sub

    Public Function IsBoardCompatibleWithSelectedFramework(ByVal BoardID As String) As Boolean
        Dim oBoardNode As XmlNode = Common.XmlMplabxTemplatesDoc.DocumentElement.SelectSingleNode(String.Format("//Board[@ID='{0}']", BoardID))
        If oBoardNode IsNot Nothing Then
            Dim oFrameworkNode As XmlNode = oBoardNode.SelectSingleNode(String.Format("CompatibleFrameworks[@Framework='{0}']", Mal.FrameworkName))
            If oFrameworkNode IsNot Nothing AndAlso oFrameworkNode.Attributes("Compatible").Value = "No" Then
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub CheckHardwareDevAndExp()
        Dim i As Integer

        For Each oBoard As MplabX.PIMBoard In MplabX.PIMBoards.Values
            If Not IsBoardCompatibleWithSelectedFramework(oBoard.ID) Then
                drpSelectPIM.Items(oBoard.Description).enabled = False
            End If
        Next

        For Each rbDevBoard As RadioButton In pnlDevBoard.Controls
            Dim oBoard As MplabX.DevelopmentBoard = rbDevBoard.Tag
            If Not IsBoardCompatibleWithSelectedFramework(oBoard.ID) Then
                rbDevBoard.Enabled = False
                rbDevBoard.Visible = False
                Continue For
            End If
            If rbDevBoard.Checked Then
                blnDevBoardSelected = True
                If oBoard.HasPIM Then
                    blnPIMSelected = (drpSelectPIM.SelectedIndex <> -1)
                Else
                    blnPIMSelected = True
                End If
            End If
            If Common.DevelopmentBoardID = oBoard.ID Then
                For Each strOptionName As String In oBoard.CompatibleOptions
                    EnableCompatibleOption(strOptionName)
                Next
            End If
        Next

        For Each rbExpBoard As RadioButton In pnlExpBoard.Controls
            Dim oBoard As MplabX.ExpansionBoard = rbExpBoard.Tag
            If Not IsBoardCompatibleWithSelectedFramework(oBoard.ID) Then
                rbExpBoard.Enabled = False
                rbExpBoard.Visible = False
                Continue For
            End If
            If rbExpBoard.Checked Then
                blnExpBoardSelected = True
            End If
            If Common.ExpansionBoardID = oBoard.ID Then
                For Each strOptionName As String In oBoard.CompatibleOptions
                    EnableCompatibleOption(strOptionName)
                Next
                'Exit For
            End If
        Next

        For Each rbDispBoard As RadioButton In pnlDisplay.Controls
            Dim oBoard As MplabX.DisplayBoard = rbDispBoard.Tag
            If Not IsBoardCompatibleWithSelectedFramework(oBoard.ID) Then
                rbDispBoard.Enabled = False
                rbDispBoard.Visible = False
            End If
            If Common.DisplayBoardID = oBoard.ID Then
                For Each strOptionName As String In oBoard.CompatibleOptions
                    EnableCompatibleOption(strOptionName)
                Next
                'Exit For
            End If
            If rbDispBoard.Checked Then
                blnDispBoardSelected = True
            End If
        Next

        Dim oCheckedButton As RadioButton = Nothing

        i = 0
        For Each oButton As RadioButton In pnlDevBoard.Controls
            If oButton.Enabled Then
                oButton.Visible = True
                oButton.Top = i * 18
                i += 1
                oCheckedButton = oButton
            Else
                oButton.Top = 0
                oButton.Visible = False
            End If
        Next
        If i = 1 Then oCheckedButton.Checked = True

        i = 0
        For Each oButton As RadioButton In pnlExpBoard.Controls
            If oButton.Enabled Then
                oButton.Visible = True
                oButton.Top = i * 18
                i += 1
                oCheckedButton = oButton
            Else
                oButton.Top = 0
                oButton.Visible = False
            End If
        Next
        If i = 1 Then oCheckedButton.Checked = True

        pnlDisplay.AutoScroll = False
        i = 0
        For Each oButton As RadioButton In pnlDisplay.Controls
            If oButton.Enabled Then
                oButton.Visible = True
                oButton.Top = i * 18
                i += 1
                oCheckedButton = oButton
            Else
                oButton.Top = 0
                oButton.Visible = False
            End If
        Next
        If i = 1 Then oCheckedButton.Checked = True
        If i > pnlDisplay.Height / Me.Font.Height Then
            pnlDisplay.AutoScroll = True
        End If

    End Sub

    Private Sub rbBoard_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim oRb As RadioButton = sender
        Dim pnl As Panel = Nothing
        If oRb IsNot Nothing Then
            pnl = oRb.Parent
            If pnl IsNot Nothing Then
                If pnl Is pnlDevBoard Then
                    blnExpBoardSelected = False
                    blnDispBoardSelected = False
                    oSelectedExpBoard = Nothing
                    oSelectedDispBoard = Nothing
                ElseIf pnl Is pnlExpBoard Then
                    blnDispBoardSelected = False
                    oSelectedDispBoard = Nothing
                End If
            End If
        End If
        picExpBoard.Image = Nothing
        picDisplay.Image = Nothing
        'oSelectedDispBoard = Nothing
        'oSelectedExpBoard = Nothing


        For Each oButton As RadioButton In pnlExpBoard.Controls
            If oButton.Checked Then
                oSelectedExpBoard = oButton.Tag
                Common.ExpansionBoardID = oSelectedExpBoard.ID
                picExpBoard.Image = oSelectedExpBoard.Image
                txtExpBoardNote.Text = oSelectedExpBoard.Note
                For Each oRbDisplay As RadioButton In pnlDisplay.Controls
                    Dim oDispBoard As MplabX.DisplayBoard = oRbDisplay.Tag
                    If oSelectedExpBoard.CompatibleDisplayBoards.Contains("*") Or _
                       oSelectedExpBoard.CompatibleDisplayBoards.Contains(oDispBoard.ID) Then
                        oRbDisplay.Enabled = True
                        If oRbDisplay.Checked Then
                            oSelectedDispBoard = oDispBoard
                            Common.DisplayBoardID = oDispBoard.ID
                        End If
                    Else
                        oRbDisplay.Enabled = False
                    End If
                Next
            End If
        Next

        For Each oButton As RadioButton In pnlDevBoard.Controls
            If oButton.Checked Then
                oSelectedDevBoard = oButton.Tag
                Common.DevelopmentBoardID = oSelectedDevBoard.ID
                picDevBoard.Image = oSelectedDevBoard.Image
                txtDevBoardNote.Text = oSelectedDevBoard.Note

                For Each oRbDisplay As RadioButton In pnlDisplay.Controls
                    Dim oDispBoard As MplabX.DisplayBoard = oRbDisplay.Tag
                    If oRbDisplay.Enabled = True AndAlso _
                       Not (oSelectedDevBoard.CompatibleDisplayBoards.Count = 0 Or _
                            oSelectedDevBoard.CompatibleDisplayBoards.Contains("*") Or _
                            oSelectedDevBoard.CompatibleDisplayBoards.Contains(oDispBoard.ID)) Then
                        oRbDisplay.Enabled = False
                    End If
                Next

                For Each oRbExpansion As RadioButton In pnlExpBoard.Controls
                    Dim oExpBoard As MplabX.ExpansionBoard = oRbExpansion.Tag
                    If oSelectedDevBoard.CompatibleExpansionBoards.Contains(oExpBoard.ID) Then
                        oRbExpansion.Enabled = True
                    Else
                        oRbExpansion.Enabled = False
                        oRbExpansion.Checked = False
                    End If
                Next
                'Dim cCompatibleDisplays As Collection = oSelectedDevBoard.CompatibleDisplayBoards
                'If oSelectedExpBoard IsNot Nothing Then
                '    If oSelectedDevBoard.CompatibleDisplayBoards.Count > 0 Then
                '        cCompatibleDisplays = oSelectedDevBoard.CompatibleDisplayBoards
                '    End If
                'End If
                'For Each oRbDisplay As RadioButton In pnlDisplay.Controls
                '    Dim oDispBoard As MplabX.DisplayBoard = oRbDisplay.Tag
                '    If cCompatibleDisplays.Contains(oDispBoard.ID) Then
                '        oRbDisplay.Enabled = True
                '        If oRbDisplay.Checked Then
                '            oSelectedDispBoard = oDispBoard
                '            Common.DisplayBoardID = oDispBoard.ID
                '        End If
                '    Else
                '        oRbDisplay.Enabled = False
                '        oRbDisplay.Checked = False
                '    End If
                'Next

                pnlPIM.Visible = oSelectedDevBoard.HasPIM
                If oSelectedDevBoard.HasPIM Then
                    If Common.PIMBoardID <> String.Empty Then
                        For Each pb As MplabX.PIMBoard In MplabX.PIMBoards.Values
                            If pb.ID = Common.PIMBoardID Then
                                'If drpSelectPIM.SelectedItem <> pb.Description Then
                                drpSelectPIM.SelectedItem = pb.Description
                                'End If
                                oSelectedPimBoard = pb
                                Exit For
                            End If
                        Next
                    Else
                        'drpSelectPIM_SelectedIndexChanged(Nothing, Nothing)
                    End If
                Else
                    Common.PIMBoardID = String.Empty
                End If

            End If
        Next

        'If pnl IsNot Nothing AndAlso pnl Is pnlDisplay Then
        If oSelectedDispBoard IsNot Nothing Then
            picDisplay.SizeMode = PictureBoxSizeMode.Zoom
            bmpDisplay = oSelectedDispBoard.Image
            picDisplay.Image = Nothing
            txtDispBoardNote.Text = oSelectedDispBoard.Note
            If Not blnLoadingWizard Then
                blnLoadingWizard = True
                rbOrientation_CheckedChanged(Nothing, Nothing)
                blnLoadingWizard = False
            Else
                rbOrientation_CheckedChanged(Nothing, Nothing)
            End If

            Dim Orient As Integer
            If Common.DisplayBoardOrientation = -1 Then
                Orient = oSelectedDispBoard.DefaultOrientation
            Else
                Orient = Common.DisplayBoardOrientation
            End If
            Select Case Orient
                Case 0
                    rbOrientation0.Checked = True
                Case 90
                    rbOrientation90.Checked = True
                Case 180
                    rbOrientation180.Checked = True
                Case 270
                    rbOrientation270.Checked = True
            End Select
        End If
        'bmpDisplay = Nothing
        'For Each oRbDisplay As RadioButton In pnlDisplay.Controls
        '    If oRbDisplay.Enabled And oRbDisplay.Checked Then
        '        Dim oDispBoard As MplabX.DisplayBoard = oRbDisplay.Tag
        '        Common.DisplayBoardID = oDispBoard.ID
        '        picDisplay.SizeMode = PictureBoxSizeMode.Zoom
        '        bmpDisplay = oDispBoard.Image
        '        picDisplay.Image = Nothing
        '        txtDispBoardNote.Text = oDispBoard.Note
        '        'rbOrientation_CheckedChanged(Nothing, Nothing)
        '        Exit For
        '    End If
        'Next

        'End If

        If Not blnLoadingWizard Then
            Common.ProjectChanged = True
        End If
        GenerateOptions()
        CheckHardwareDevAndExp()
        CheckAvailableOptions()
        EnableBtnModfy()

    End Sub

    Private Sub rbOrientation_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbOrientation0.CheckedChanged, rbOrientation90.CheckedChanged, rbOrientation180.CheckedChanged, rbOrientation270.CheckedChanged

        Dim tmpBitmap As Bitmap
        Dim rt As RotateFlipType
        If rbOrientation0.Checked Then
            If Not blnLoadingWizard Then Common.DisplayBoardOrientation = 0
            rt = RotateFlipType.RotateNoneFlipNone
        ElseIf rbOrientation90.Checked Then
            If Not blnLoadingWizard Then Common.DisplayBoardOrientation = 90
            rt = RotateFlipType.Rotate90FlipNone
        ElseIf rbOrientation180.Checked Then
            If Not blnLoadingWizard Then Common.DisplayBoardOrientation = 180
            rt = RotateFlipType.Rotate180FlipNone
        ElseIf rbOrientation270.Checked Then
            If Not blnLoadingWizard Then Common.DisplayBoardOrientation = 270
            rt = RotateFlipType.Rotate270FlipNone
        End If
        If bmpDisplay IsNot Nothing Then
            tmpBitmap = New Bitmap(bmpDisplay)
            tmpBitmap.RotateFlip(rt)
            picDisplay.Image = tmpBitmap
        End If
        If Not blnLoadingWizard Then
            Common.ProjectChanged = True
        End If
    End Sub

    Private Sub EnableBtnModfy()
        btnModifyProject.Enabled = False
        If Common.CodeGenLocationOptionsOk _
            And CodeGenLocation1.Framework1.lblMalPath.ForeColor <> Color.Red _
            And blnDevBoardSelected And blnPIMSelected And blnExpBoardSelected And blnDispBoardSelected Then
            btnModifyProject.Enabled = True
        End If
    End Sub

#Region "CodegenLocation"

    'Private Sub CodeGenLocation1_BadChoices(ByVal sender As Object, ByVal e As System.EventArgs) Handles CodeGenLocation1.BadChoices
    '    btnNext2.Enabled = False
    'End Sub
    Private Sub SetSkeletonLabels()
        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                chkGenerateGraphicsConfig.Text = "Generate GraphicsConfig.h"
                chkGenerateHardwareProfile.Text = "Generate HardwareProfile.h"
                chkModifyGraphicsConfig.Text = "Modify GraphicsConfig.h"
                chkModifyHardwareProfile.Text = "Modify HardwareProfile.h"
            Case "MLA", "HARMONY"
                chkGenerateGraphicsConfig.Text = "Generate gfx_config.h"
                chkGenerateHardwareProfile.Text = "Generate system.h/system_config.h"
                chkModifyGraphicsConfig.Text = "Modify gfx_config.h"
                chkModifyHardwareProfile.Text = "Modify system.h/system_config.h"
        End Select
    End Sub

    Private Sub CodeGenLocation1_OptionsChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CodeGenLocation1.OptionsChanged
        CheckGenerateOptions()
        WizardLogClear()
        WizardLogWrite("Checking MPLAB X Project Path " & Common.MplabXProjectPath)
        If Common.MplabXProjectPath = "" Then
            WizardLogWrite(Nothing, "Not defined yet")
        Else
            If Not Common.MplabxCheckProjectPath(Common.MplabXProjectPath) Then
                CodeGenLocation1.lblMplabXProject.ForeColor = Color.Red
                WizardLogWrite(Nothing, "Wrong Path!")
            Else
                CodeGenLocation1.lblMplabXProject.ForeColor = Color.Green
                WizardLogWrite(Nothing, "OK")
                OpenMplabxProject()
                SetSkeletonLabels()
            End If
            EnableBtnModfy()
        End If
        If Not blnLoadingWizard Then
            Common.ProjectChanged = True
        End If
        Common.QuickCodeGen = False
    End Sub

    Private Sub CheckGenerateOptions()
        If Common.CodeGenDestPath IsNot Nothing Then
            WizardLogWrite("Checking Project Files")
            chkGenerateMainC.Checked = Not FileExistsCaseSensitive(Path.Combine(Common.CodeGenDestPath, "vgdd_main.c"))
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    chkGenerateGraphicsConfig.Checked = Not FileExistsCaseSensitive(Path.Combine(Common.CodeGenDestPath, "GraphicsConfig.h"))
                    chkGenerateHardwareProfile.Checked = Not FileExistsCaseSensitive(Path.Combine(Common.CodeGenDestPath, "HardwareProfile.h"))
                Case "MLA", "HARMONY"
                    chkGenerateGraphicsConfig.Checked = Not FileExistsCaseSensitive(Path.Combine(Common.CodeGenDestPath, "gfx_config.h"))
                    chkGenerateHardwareProfile.Checked = Not FileExistsCaseSensitive(Path.Combine(Common.CodeGenDestPath, "system_config.h"))
            End Select
            If chkGenerateMainC.Checked Or chkGenerateGraphicsConfig.Checked Or chkGenerateHardwareProfile.Checked Then
                chkGenerateSkeleton.CheckState = CheckState.Checked
            Else
                chkGenerateSkeleton.CheckState = CheckState.Unchecked
            End If
            chkModifySkeleton.Checked = True
            pnlProject.Enabled = True
            WizardLogWrite(Nothing, "OK")
            EnableBtnModfy()
        End If
    End Sub

#End Region

    Private Sub OpenMplabxProject()
        MplabX.LoadMplabxProject()
        WizardFlushLog()
        CheckGenerateOptions()
    End Sub

    Private Sub WizardFlushLog()
        For Each line As String In MplabX.Log.ToString.Split(vbCrLf)
            line = line.Replace(vbCrLf, "").Trim
            If line <> "" Then
                WizardLogWrite(line)
                If line.EndsWith("ERROR") Then
                    WizardLogWrite(Nothing, "KO")
                Else
                    WizardLogWrite(Nothing, "OK")
                End If
            End If
        Next
        MplabX.Log.Length = 0
    End Sub

    Private Sub MplabXClearFolders(ByVal oFoldersToClear As XmlNodeList)
        If oFoldersToClear.Count = 0 Then Exit Sub
        If MplabX.oMplabxIpc.IsConnected Then
            'TODO: ClearFolders via IPC
        Else
            Try
                Dim oRootFolderNode As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oProjectXmlDoc.DocumentElement, "root", True)
                For Each oFolderNode As XmlNode In oFoldersToClear
                    Try
                        Dim strFolderToClear As String = oFolderNode.Attributes("Name").Value
                        WizardLogWrite("Clearing Folder " & strFolderToClear)
                        Dim oFolderProjectNode As XmlNode = MplabX.MplabXGetSubFolder(oRootFolderNode, strFolderToClear, False)
                        If oFolderProjectNode IsNot Nothing Then
                            Do While oFolderProjectNode.HasChildNodes
                                Dim oFileNode As XmlNode = oFolderProjectNode.ChildNodes(0)
                                oFolderProjectNode.RemoveChild(oFileNode)
                            Loop
                        End If
                        WizardLogWrite(Nothing, "OK")
                    Catch ex As Exception
                    End Try
                Next
            Catch ex As Exception
            End Try
        End If
    End Sub

    Dim aGroupFilesToAdd As New ArrayList
    Private Sub MplabXAddGroupFiles(ByVal oGroupToAdd As XmlNodeList)
        If oGroupToAdd Is Nothing Then
            For Each strGroupName As String In aGroupFilesToAdd
                MplabXClearFolders(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("Groups/Group[@Name='{0}']/Project/ClearFolder", strGroupName)))
            Next
            For Each strGroupName As String In aGroupFilesToAdd
                MplabX.MplabXAddProjectFiles(MplabX.MplabXTemplateDocEl.SelectNodes(String.Format("Groups/Group[@Name='{0}']/Project/Folder", strGroupName)))
            Next
            Return
        End If
        For Each oAddGroupNode As XmlNode In oGroupToAdd
            aGroupFilesToAdd.Add(oAddGroupNode.Attributes("Name").Value)
        Next
    End Sub

    Private Sub MplabXRemoveGroupFiles(ByVal oGroupToRemove As XmlNodeList)
        For Each strGroupName As String In oGroupToRemove
            Dim intIdx As Integer = aGroupFilesToAdd.IndexOf(strGroupName)
            If intIdx > 0 Then
                aGroupFilesToAdd.RemoveAt(intIdx)
            End If
        Next
    End Sub

    Private Sub GenerateSkeletonFile(ByVal srcFileName As String, ByVal destFileName As String, ByVal strDestPath As String)
        'Dim strFileContent As String
        'Using oSr As New StreamReader(Common.GetResourceStream(FileName))
        '    strFileContent = oSr.ReadToEnd.Replace("[ProjectName]", Common.ProjectName)
        '    oSr.Close()
        'End Using

        Dim strDestFilename As String = Path.Combine(strDestPath, destFileName)
        'If File.Exists(strDestFilename) Then
        '    If MessageBox.Show("Skeleton file " & strDestFilename & " already exists. Overwrite it?", "Confirm overwrite", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
        '        Exit Sub
        '    End If
        'End If
        'Using oSw As New StreamWriter(strDestFilename)
        '    oSw.Write(strFileContent)
        '    oSw.Flush()
        '    oSw.Close()
        'End Using
        Try
            Dim strFileContent As String, strTemplateFile As String
            strTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, srcFileName)
            If Not FileExistsCaseSensitive(strTemplateFile) Then
                strTemplateFile = Path.Combine(Path.Combine(Common.MplabXTemplatesFolder, Mal.FrameworkName), srcFileName)
            End If
            If Not FileExistsCaseSensitive(strTemplateFile) Then
                MessageBox.Show("Cannot find Template file " & strTemplateFile)
                Exit Sub
            End If
            Using oSr As New StreamReader(strTemplateFile)
                strFileContent = oSr.ReadToEnd
                oSr.Close()
            End Using
            Common.WriteFileWithBackup(strDestFilename, strFileContent)
        Catch ex As Exception
            MessageBox.Show("Cannot overwrite " & strDestFilename & vbCrLf & ex.Message, "Skeleton file creation error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Function MplabXUpdateSourceRootList(ByVal strPath As String) As XmlNode
        If MplabX.oProjectXmlDoc Is Nothing Then Return Nothing

        MplabXUpdateSourceRootList = MplabX.oProjectXmlDoc.DocumentElement.SelectSingleNode("sourceRootList")
        If MplabXUpdateSourceRootList Is Nothing Then
            WizardLogWrite("Creating Project sourceRootList")
            MplabXUpdateSourceRootList = MplabX.oProjectXmlDoc.CreateElement("sourceRootList")
            MplabX.oProjectXmlDoc.DocumentElement.AppendChild(MplabXUpdateSourceRootList)
            WizardLogWrite(Nothing, "OK")
        End If
        Dim blnFound As Boolean = False
        MplabXUpdateSourceRootList = MplabX.oProjectXmlDoc.DocumentElement.SelectSingleNode("sourceRootList")
        For Each oElem As XmlNode In MplabXUpdateSourceRootList
            If oElem.InnerText = strPath Then
                blnFound = True
                Return oElem
            End If
        Next
        If Not blnFound Then
            WizardLogWrite("Addin " & strPath & " to sourceRootList in MPLAB X project")
            Dim oElem As XmlNode = MplabX.oProjectXmlDoc.CreateElement("Elem")
            oElem.InnerText = strPath
            MplabXUpdateSourceRootList.AppendChild(oElem)
            WizardLogWrite(Nothing, "OK")
            Return oElem
        End If
    End Function

    Private Sub btnNext0Start_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext0Start.Click
        CustomTabControl1.SelectedTab = tabMplabX
    End Sub

    Private Sub btnNext1SetMplabXProject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext1SetMplabXProject.Click
        CustomTabControl1.SelectedTab = tabHardware
    End Sub

    Private Sub btnNext3Options_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext3Options.Click
        CustomTabControl1.SelectedTab = tabCreateProject
    End Sub

    Private Sub btnNext2Hardware_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext2Hardware.Click
        CustomTabControl1.SelectedTab = tabOptions
    End Sub

    Private Sub btnNext4ModifyProject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext4ModifyProject.Click
        CustomTabControl1.SelectedTab = tabFinish
    End Sub

    Private Sub btnCloseWizard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCloseWizard.Click
        Me.Close()
    End Sub


    Private Sub picDevBoard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picDevBoard.DoubleClick, picExpBoard.DoubleClick, picDisplay.DoubleClick
        Dim strUrl As String = Nothing
        If sender Is picDevBoard Then
            strUrl = MplabX.DevBoards(Common.DevelopmentBoardID).URL
        ElseIf sender Is picExpBoard Then
            strUrl = MplabX.ExpBoards(Common.ExpansionBoardID).URL
        ElseIf sender Is picDisplay Then
            strUrl = MplabX.DisplayBoards(Common.DisplayBoardID).URL
        End If
        If strUrl IsNot Nothing Then
            VGDDCommon.Common.RunBrowser(strUrl)
        Else
            MessageBox.Show("Sorry, URL not defined for this board.", "Missing URL")
        End If
    End Sub
    Private Sub picLogoMplabX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picLogoMplabX.Click
        Try
            System.Diagnostics.Process.Start(Common.MplabXTemplatesFolder)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnGenerateCode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateCode.Click
        oMainShell.MenuGenerateCode.PerformClick()
        btnCloseWizard.Enabled = True
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        Common.RunBrowser("http://virtualfab.it/mediawiki/index.php/MPLABX_Wizard#Troubleshooting")
    End Sub

    Private Sub lnkTestedHardware_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkTestedHardware.LinkClicked
        Common.RunBrowser("http://virtualfab.it/mediawiki/index.php/MPLABX_Wizard#Tested_Hardware_Configurations")
    End Sub

    Private Sub OverWriteWarning(ByVal strFilename As String, ByVal chkCheckBox As CheckBox)
        If chkCheckBox.Checked AndAlso File.Exists(Path.Combine(Common.CodeGenDestPath, strFilename)) Then
#If CONFIG <> "Debug" Then
            MessageBox.Show(String.Format("Warning: {0} already exists! If you leave this option checked, it will be overwritten." & vbCrLf & "If you wrote user code in {0}, your modifications will be lost!", strFilename), "Overwrite Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
#End If
        End If
    End Sub

    Private Sub chkGenerateMainC_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkGenerateMainC.CheckedChanged
        OverWriteWarning("vgdd_main.c", chkGenerateMainC)
    End Sub

    Private Sub chkGenerateGraphicsConfig_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkGenerateGraphicsConfig.CheckedChanged
        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                OverWriteWarning("GraphicsConfig.h", chkGenerateGraphicsConfig)
            Case "MLA", "HARMONY"
                OverWriteWarning("gfx_config.h", chkGenerateGraphicsConfig)
        End Select
    End Sub

    Private Sub chkGenerateHardwareProfile_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkGenerateHardwareProfile.CheckedChanged
        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                OverWriteWarning("HardwareProfile.h", chkGenerateHardwareProfile)
            Case "MLA", "HARMONY"
                OverWriteWarning("system_config.h", chkGenerateHardwareProfile)
        End Select
    End Sub

    Private Sub chkDebugInfo_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDebugInfo.CheckedChanged
        Common.InsertDebugInfo = chkDebugInfo.Checked
    End Sub

    Private Sub txtHeapSize_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHeapSize.TextChanged
        Integer.TryParse(txtHeapSize.Text, Common.ProjectHeapSize)
        Dim CurrentHeap As Integer = FootPrint.Heap
        If CurrentHeap = 0 Then CurrentHeap = Common.ProjectLastComputedHeapSize
        If Common.ProjectHeapSize > CurrentHeap * 1.5 Then
            txtHeapSize.ForeColor = Color.DarkGreen
        ElseIf Common.ProjectHeapSize > CurrentHeap Then
            txtHeapSize.ForeColor = Color.Orange
        Else
            txtHeapSize.ForeColor = Color.Red
        End If
    End Sub

    Private Sub CustomTabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomTabControl1.SelectedIndexChanged
        If CustomTabControl1.SelectedTab Is tabHardware Then
            'CheckHardwareDevAndExp()
            'rbBoard_CheckedChanged(pnlDevBoard.Controls(0), Nothing)
            'LoadTemplates()
            blnLoadingWizard = True
            rbBoard_CheckedChanged(Nothing, Nothing)
            blnLoadingWizard = False
        ElseIf CustomTabControl1.SelectedTab Is tabCreateProject Then
            txtWarnings.Text = String.Empty
            If Not Common.CodeGenLocationOptionsOk Then
                txtWarnings.Text &= "MPLAB X Project options not correctly set. Please go back to step 1." & vbCrLf & vbCrLf
            End If
            If CodeGenLocation1.Framework1.lblMalPath.ForeColor = Color.Red Then
                txtWarnings.Text &= "Framework to use not set. Please go back to step 1." & vbCrLf & vbCrLf
            End If
            If Not blnDevBoardSelected Then
                txtWarnings.Text &= "Development board not set. Please go back to step 2." & vbCrLf & vbCrLf
            End If
            If Not blnPIMSelected Then
                txtWarnings.Text &= "PIM board not set. Please go back to step 2." & vbCrLf & vbCrLf
            End If
            If Not blnExpBoardSelected Then
                txtWarnings.Text &= "Expansion board not set. Please go back to step 2." & vbCrLf & vbCrLf
            End If
            If Not blnDispBoardSelected Then
                txtWarnings.Text &= "Display board not set. Please go back to step 2." & vbCrLf & vbCrLf
            End If
            If txtWarnings.Text = String.Empty Then
                If Not MplabX.IpcEnabled Then
                    txtWarnings.Text = "Warning: Before clicking on ""Modify Project"" be sure MPLAB X is NOT running (preferred) or at least that the MPLAB X project is CLOSED." & vbCrLf & vbCrLf
                    txtWarnings.Text &= "If you get errors in MPLAB X after having run the Wizard, please consult the" & vbCrLf & vbCrLf
                    Dim LinkLabel1 As New LinkLabel
                    LinkLabel1.Text = "MPLAB X Wizard Troubleshooting Page"
                    LinkLabel1.AutoSize = True
                    LinkLabel1.Location = Me.txtWarnings.GetPositionFromCharIndex(Me.txtWarnings.TextLength)
                    Me.txtWarnings.Controls.Add(LinkLabel1)
                    Me.txtWarnings.AppendText(LinkLabel1.Text & "   ")
                    Me.txtWarnings.SelectionStart = Me.txtWarnings.TextLength
                    AddHandler LinkLabel1.LinkClicked, AddressOf LinkLabel1_LinkClicked
                End If
            End If
        End If
    End Sub

    Private Sub drpSelectPIM_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpSelectPIM.SelectedIndexChanged
        If drpSelectPIM.SelectedItem = String.Empty Then
            Common.PIMBoardID = String.Empty
        Else
            For Each oPimBoard As MplabX.PIMBoard In MplabX.PIMBoards.Values
                If oPimBoard.Description = drpSelectPIM.SelectedItem Then
                    Common.PIMBoardID = oPimBoard.ID
                    Exit For
                End If
            Next
        End If
        'EnableBtnModfy()
        rbBoard_CheckedChanged(Nothing, Nothing)
    End Sub

End Class

Public Class VGDDWizardCheckBox
    Inherits CheckBox

    Private _DependsOn As String
    Private _AlternativeTo As String
    Private _DefaultValue As Boolean = False
    Private _AlwaysAvailable As Boolean

    Public Overloads Property AlwaysAvailable As Boolean
        Set(ByVal value As Boolean)
            _AlwaysAvailable = value
        End Set
        Get
            Return _AlwaysAvailable
        End Get
    End Property

    Public Overloads Property Enabled As Boolean
        Set(ByVal value As Boolean)
            MyBase.Enabled = value
        End Set
        Get
            Return MyBase.Enabled
        End Get
    End Property

    Public Property DependsOn As String
        Get
            Return _DependsOn
        End Get
        Set(ByVal value As String)
            _DependsOn = value
        End Set
    End Property

    Public Property AlternativeTo As String
        Get
            Return _AlternativeTo
        End Get
        Set(ByVal value As String)
            _AlternativeTo = value
        End Set
    End Property

    Public Property DefaultValue As Boolean
        Get
            Return _DefaultValue
        End Get
        Set(ByVal value As Boolean)
            _DefaultValue = value
        End Set
    End Property
End Class