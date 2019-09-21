Imports System
Imports System.Drawing
Imports System.Drawing.Design
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Windows.Forms
Imports System.Windows.Forms.Design
Imports System.Data
Imports System.IO
Imports System.Xml
Imports System.Text
Imports VGDDCommon
Imports System.Security.Cryptography
Imports WeifenLuo.WinFormsUI.Docking
Imports VirtualFabUtils.Utils

Partial Public Class MainShell
    Inherits Form

    Public _hostSurfaceManager As HostSurfaceManager = Nothing
    Public WithEvents _CurrentHost As HostControl = Nothing
    Private _SelectedControls As Collections.ArrayList
    Public WithEvents oKeyStrokeMessageFilter As KeystrokMessageFilter
    Private WithEvents oMicrochipCommon As New Common
    Private DimensionSet As Boolean = False
    Private oXmlProjectDoc As XmlDocument = New XmlDocument
    Private WithEvents oShowSource As frmGenCode
    Private dStartTime As Date = Date.Now
    Private oFrmMplabxWizard As frmMPLABXWizard
    Private WithEvents tmrMouseCoords As Timer
    Private oFrmMagnify As frmMagnify
    Private WithEvents tmrUpdateUndo As Timer
    Public WithEvents tmrMigrate As Timer
    Public sngMigrateRatioWidth As Single
    Public sngMigrateRatioHeight As Single

    Public _MagnifyLockStatus As MagnifyStatuses = MagnifyStatuses.Unlocked
    Public MagnyfyPoint As Point

    Private _selectionService As ISelectionService
    Private _ComponentChangeService As IComponentChangeService

    Private CurrentSelection() As Object = Nothing
    Private LastSelection() As Object
    Private WithEvents tmrUpdateEvents As New Timer
    Private dProjectLoadStart As Date
    Private WithEvents tmrCheckMiniature As New Timer

    Private MainPane As DockPane

    Public Sub New()
        MyBase.New()
        InitializeComponent()
        'ResizeMe()
        'Me.Opacity = 0
        Me._DockPanel1.Left = 0
        Me._DockPanel1.Top = ToolStrip1.Height
        Me._DockPanel1.DocumentStyle = DocumentStyle.DockingMdi
        _DockPanel1.Theme = New WeifenLuo.WinFormsUI.Docking.VS2012LightTheme
        tmrUpdateUndo = New Timer
        tmrUpdateUndo.Interval = 100
        tmrUpdateUndo.Enabled = False
        tmrCheckMiniature.Interval = 500
        tmrCheckMiniature.Enabled = False
        Common.HelpProvider = Me.HelpProvider1
        Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_Main"
        Common.VGDDIsRunning = True
    End Sub

    Private Sub MainShell_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        If e.Control And e.KeyCode = Keys.S Then
            SaveProject()
        ElseIf e.Control And e.Shift And e.KeyCode = Keys.S Then
            SaveScreenOnTab(_CurrentHost)
        ElseIf e.Control And e.KeyCode = Keys.N Then
            MenuScreenNew.PerformClick()
        End If
    End Sub

    Private Sub MainShell_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Common.GetOs()
            'MessageBox.Show(String.Format("OS={0} SP={1} Platform={2}", Common.OsVersion, Common.OsServicePack, Common.OsPlatform))
            If Common.OsVersion.StartsWith("5.") And Common.OsServicePack <> "SP3" Then
                If MessageBox.Show("You are running VGDD under Windows XP. This software has known issues if Windows XP Service Pack 3 (SP3) isn't installed." & vbCrLf &
                                "Do you want to continue anyway?", "Warning", MessageBoxButtons.YesNo) = DialogResult.No Then
                    End
                End If
            End If

            Try
                If Not My.Settings.SettingsUpgraded Then
                    My.Settings.Upgrade()
                    'My.Settings.Recent = My.Settings.GetPreviousVersion("Recent")
                    My.Settings.SettingsUpgraded = True
                End If

            Catch ex As Configuration.ConfigurationErrorsException
                Dim filename As String = CType(ex.InnerException, Configuration.ConfigurationErrorsException).Filename
                If MessageBox.Show("VGDD has detected that your user settings file has become corrupted. " & vbCrLf &
                                      "This may be due to a crash or improper exiting of the program." & vbCrLf & vbCrLf &
                                      "VGDD must reset your user settings in order to continue." & vbCrLf & vbCrLf &
                                      "Click Yes to reset your user settings and continue." & vbCrLf & vbCrLf &
                                      "Click No if you wish to attempt manual repair or to rescue information before proceeding." & vbCrLf & vbCrLf &
                                      "The file containing the corrupted settings is " & filename,
                                      "Corrupt user settings", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.Yes Then

                    File.Delete(filename)
                    My.MySettings.Default.Reload()
                    MessageBox.Show("Please restart application")
                    End
                Else
                    Process.GetCurrentProcess().Kill()
                End If
            End Try
            VGDDCommon.Mal.ConfiguredFrameworks = My.Settings.ConfiguredFrameworks
            Common.ProjectJavaCommand = My.Settings.JavaCommand
            Common.ProjectFallbackGRCPath = My.Settings.FallbackGRCPath
            Common.ProjectCompiler = My.Settings.DefaultCompiler
            Select Case Common.ProjectCompiler
                Case "C30", "XC16"
                    Common.ProjectCompilerFamily = "C30"
                Case "C32", "XC32"
                    Common.ProjectCompilerFamily = "C32"
            End Select
            Common.UserTemplatesFolder = My.Settings.MyUserTemplatesFolder
            Common.ProjectMakeBackups = My.Settings.MyMakeBackups

            Common.MplabXIpcIpAddress = My.Settings.MplabXIpcIpAddress
            Common.MplabXIpcPort = My.Settings.MplabXIpcPort
            If Common.MplabXIpcIpAddress = String.Empty Then
                Common.MplabXIpcIpAddress = "127.0.0.1" 'MplabX.GetLocalIPv4Address()
                My.Settings.MplabXIpcIpAddress = Common.MplabXIpcIpAddress
                My.Settings.Save()
            End If
            MplabX.IpcEnabled = My.Settings.MplabXUseIpc

            LoadDefaultScheme()
            RebuildRecent(Nothing)
            Common.ProjectHeight = My.Settings.DefaultHeight
            Common.ProjectWidth = My.Settings.DefaultWidth
            Common.ProjectColourDepth = My.Settings.DefaultColourDepth
            Common.ProjectUsePalette = My.Settings.UseIndexedColours
            Common.ProjectUseMultiByteChars = My.Settings.UseMultiByteChars
            Common.ProjectMultiLanguageTranslations = My.Settings.MultiLanguageTranslations
            Common.ProjectFallbackGRCPath = My.Settings.FallbackGRCPath
            'Common.DevelopmentBoardID = My.Settings.MplabXWizardDevBoard
            'Common.ExpansionBoardID = My.Settings.MplabXWizardExpBoard
            'Common.DisplayBoardID = My.Settings.MplabXWizardDispBoard
            'Common.DisplayBoardOrientation = My.Settings.MplabXWizardDispOrientation
            VGDDCommon.Mal.MalPath = My.Settings.MalPath
            If VGDDCommon.Mal.MalPath.StartsWith("\\") Then
                VGDDCommon.Mal.MalPath = "\\" & VGDDCommon.Mal.MalPath.Substring(2).Replace("\\", "\").Replace("\", "\\")
            Else
                VGDDCommon.Mal.MalPath = VGDDCommon.Mal.MalPath.Replace("\\", "\")
            End If

            'MplabX.MplabXFilesAssembly = Assembly.GetExecutingAssembly
            MplabX.MplabXFilesAssembly = MyAssemblyResolveEventHandler(Me, New ResolveEventArgs("VGDDCommonEmbedded.dll"))

            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            Common.MplabXExtractTemplates()
            Me.Cursor = Cursors.Default

            Mal.CheckMalVersion(VGDDCommon.Mal.MalPath)
            'CodeGen.LoadCodeGenTemplates()
            LoadExternalWidgets()

            DockLoadLayout()
            DockLayoutHideAll()

            tmrMouseCoords = New Timer
            tmrMouseCoords.Interval = 100
            tmrMouseCoords.Enabled = True
            ToolStripDeleteWidget.Enabled = False
            _MainSplash.Show(_DockPanel1)
            If Mal.ConfiguredFrameworks Is Nothing OrElse Mal.ConfiguredFrameworks.Count = 0 Then
                MessageBox.Show("Preferences are not yet configured. The Global Preferences form will be shown, where you have to select at least one framework (MLA / MLA Legacy / Harmony) to be used from now on.")
                frmPreferences.ShowDialog()
            End If
            oKeyStrokeMessageFilter = New KeystrokMessageFilter
            Application.AddMessageFilter(oKeyStrokeMessageFilter)
            _WidgetPropertyGrid.PropertyGrid.PropertySort = My.Settings.MyPropertySort

            VGDDCommon.VGDDEventsEditorNew.EditForm = _EventsEditor
            ClearProject()

        Catch ex As Exception
            MessageBox.Show("VGDD could not load the main form." & vbCrLf & _
                      "This may be due to a crash or improper exiting of the program." & vbCrLf & vbCrLf & _
                      "The program will now exit." & vbCrLf & vbCrLf & _
                      "If this issue persists, restart VGDD while pressing the SHIFT key." & vbCrLf & vbCrLf & _
                      "This will reset VGDD to a post-installation state.", "Error")
            CatchException(ex)
            End
        End Try

        Dim pop As Graphics = Me.CreateGraphics
        If pop.DpiX <> 96 Or pop.DpiY <> 96 Then
            MessageBox.Show("You are running Windows with an enhanced display setting (DPI greater than 96)" & vbCrLf & _
                            vbCrLf & _
                            "VGDD needs a display set to its default DPI setting to correctly display texts and Widgets as they will be displayed on hardware screens" & vbCrLf & _
                            vbCrLf & _
                            "To fix this, go to " & vbCrLf & _
                            "Control Panel -> Appearance -> Make text larger or smaller" & vbCrLf & _
                            "and reset to 100% (default)" & vbCrLf & _
                            vbCrLf & _
                            "If you continue to work in VGDD with current DPI settings, texts will appear bigger on PC screen and many of them won't fit into their widgets.", "Warning - Magnifier detected")
        End If
        pop.Dispose()
        MplabX.IPC_Start()
    End Sub

    Public Shared Sub LoadDefaultScheme()
        Common.DefaultSchemeXML = My.Settings.DefaultSchemeXML
    End Sub

    Public Sub UpdateCaption()
        Dim strLicenseType As String = "Open Source" ' LT()
        Dim strAppDescr As String = My.Application.Info.Trademark
#If CHINAMASTERS Then
        strAppDescr &= " - Microchip Far East RTC Edition"
#End If
        Me.Text = My.Application.Info.Title & " - " & strAppDescr & " " & VGDDCommon.Common.VGDDVERSION
        'If strLicenseType <> "" And (Not strLicenseType.Contains("DEMO") OrElse (strLicenseType.Contains("DEMO") AndAlso Not Me.Text.Contains("DEMO"))) Then
        '    Me.Text &= " - " & LT()
        'End If
        If Common.ProjectName <> "" AndAlso Common.ProjectName <> "New" Then
            Me.Text &= " - " & Common.ProjectName
        End If
    End Sub

    Public Sub LoadExternalWidgets()
        Try
            oExternalWidgetsHandler.LoadAll()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MainShell_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            'If Me.WindowState = FormWindowState.Normal Then
            My.Settings.MyTop = Me.Top
            My.Settings.MyLeft = Me.Left
            My.Settings.MyHeight = Me.Height
            My.Settings.MyWidth = Me.Width
            My.Settings.MyWindowState = Me.WindowState
            My.Settings.Save()
        Catch ex As Exception
        End Try
        Select Case CheckModifiedProject()
            Case DialogResult.Yes, DialogResult.No
                e.Cancel = False
            Case DialogResult.Abort, DialogResult.Cancel
                e.Cancel = True
            Case Else
                Debug.Print("?")
        End Select
        If Not e.Cancel Then
            CloseAllScreens()
            Try
                _DockPanel1.SaveAsXml(LayoutFileName)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub MainShell_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        If Not DimensionSet Then
            DimensionSet = True
            ResizeMe()
            Application.DoEvents()
            Me.WindowState = My.Settings.MyWindowState
        End If
    End Sub

    Private Sub ClearProject()
        lblStatus.Text = "Clearing Project..."
        Common.ProjectPathName = "New"
        Common.ProjectMultiLanguageTranslations = 0

        UpdateCaption()
        If Common.oEventsFontNormal Is Nothing Then
            Common.oEventsFontNormal = New Font(_EventTree.tvEvents.Font, FontStyle.Regular)
            Common.oEventsFontBold = New Font(_EventTree.tvEvents.Font, FontStyle.Bold)
        End If

        CloseAllScreens()

        If _selectionService IsNot Nothing Then
            RemoveHandler _selectionService.SelectionChanged, AddressOf Me._CurrentHost_SelectionChanged
            RemoveHandler _ComponentChangeService.ComponentAdded, AddressOf Me._CurrentHost_ControlAdded
            'RemoveHandler _ComponentChangeService.ComponentAdding, AddressOf Me._CurrentHost_ControlAdding
            RemoveHandler _ComponentChangeService.ComponentRemoved, AddressOf Me._CurrentHost_ControlRemoved
            RemoveHandler _ComponentChangeService.ComponentChanged, AddressOf Me._CurrentHost_ControlChanged
            _selectionService = Nothing
            _ComponentChangeService = Nothing
        End If
        LastSelection = Nothing

        If _EventTree.tvEvents.Nodes.Count > 0 Then
            For Each oWidgetNode As TreeNode In _EventTree.tvEvents.Nodes(0).Nodes
                For Each oEventNode As TreeNode In oWidgetNode.Nodes
                    If oEventNode.Tag IsNot Nothing Then
                        Try
                            CType(oEventNode.Tag, VGDDEventsCollection).Clear()
                        Catch ex As Exception
                        End Try
                    End If
                Next
            Next
        End If
        Try
            _EventTree.tvEvents.Nodes.Clear()
        Catch ex As Exception
        End Try

        Me._SolutionExplorer.ClearSolution()

        Me._EventsEditor.Clear()

        Dim i As Integer = 0
        Do While i < Common.aScreens.Count
            Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens.Item(i)
            'If oScreenAttr.Screen IsNot Nothing Then
            '    oScreenAttr.Screen.Dispose()
            'End If
            If oScreenAttr.Hc Is Nothing Then
                i += 1
            Else
                If oScreenAttr.Hc IsNot Nothing AndAlso oScreenAttr.Hc.HostSurface IsNot Nothing Then
                    Dim SelectionService As ISelectionService = oScreenAttr.Hc.HostSurface.GetService(GetType(ISelectionService))
                    Dim ComponentChangeService As IComponentChangeService = oScreenAttr.Hc.HostSurface.GetService(GetType(IComponentChangeService))
                    If SelectionService IsNot Nothing Then
                        RemoveHandler SelectionService.SelectionChanged, AddressOf Me._CurrentHost_SelectionChanged
                    End If
                    If ComponentChangeService IsNot Nothing Then
                        RemoveHandler ComponentChangeService.ComponentAdded, AddressOf Me._CurrentHost_ControlAdded
                        'RemoveHandler ComponentChangeService.ComponentAdding, AddressOf Me._CurrentHost_ControlAdding
                        RemoveHandler ComponentChangeService.ComponentRemoved, AddressOf Me._CurrentHost_ControlRemoved
                        RemoveHandler ComponentChangeService.ComponentChanged, AddressOf Me._CurrentHost_ControlChanged
                    End If
                    SelectionService = Nothing
                    ComponentChangeService = Nothing
                    'hc.DesignerHost.RootComponent.Dispose()
                    oScreenAttr.Screen.Dispose()
                    oScreenAttr.Screen = Nothing
                    oScreenAttr.Hc.Dispose()
                    oScreenAttr.Hc = Nothing
                Else
                    i += 1
                End If
            End If
        Loop

        Common.aScreens.Clear()

        If _hostSurfaceManager IsNot Nothing Then
            _hostSurfaceManager.RemoveService(GetType(IToolboxService))
            _hostSurfaceManager.RemoveService(GetType(VGDDIDE.SolutionExplorer))
            _hostSurfaceManager.RemoveService(GetType(VGDDIDE.OutputWindow))
            _hostSurfaceManager.RemoveService(GetType(System.Windows.Forms.PropertyGrid))
            _hostSurfaceManager.Dispose()
        End If

        Try
            If _DockPanel1 IsNot Nothing Then
                For i = 0 To _DockPanel1.Contents.Count - 1
                    Dim oDoc As DockContent = _DockPanel1.Contents(i)
                    If TypeOf (oDoc) Is HostControl Then
                        Dim hc As HostControl = oDoc
                        If hc.DesignerHost IsNot Nothing AndAlso hc.DesignerHost.Loading Then
                            oDoc.Close()
                        End If
                    End If
                Next
            End If
            If _SelectedControls IsNot Nothing Then _SelectedControls.Clear()
            Me._OutputWindow.RichTextBox.Clear()
        Catch ex As Exception

        End Try

        Common.ClearBitmaps()
        For Each oScheme As VGDDScheme In Common._Schemes.Values
            oScheme.Font = Nothing
            oScheme.UsedBy.Clear()
            oScheme = Nothing
        Next
        Common._Schemes.Clear()

        For Each VFont As VGDDFont In Common._Fonts
            VFont.UsedBy.Clear()
            VFont = Nothing
        Next
        Common._Fonts.Clear()

        Common.MplabXProjectPath = String.Empty
        Common.CurrentScreen = Nothing
        Common.oGrcProjectInternal = Nothing
        Common.oGrcProjectExternal = Nothing
        Common.oGrcBinBmpOnSd = Nothing
        Common.WizardOptions.Clear()

        GC.Collect()

        _hostSurfaceManager = New HostSurfaceManager
        _hostSurfaceManager.AddService(GetType(IToolboxService), Me._Toolbox)
        _hostSurfaceManager.AddService(GetType(VGDDIDE.SolutionExplorer), Me._SolutionExplorer)
        _hostSurfaceManager.AddService(GetType(VGDDIDE.OutputWindow), Me._OutputWindow)
        _hostSurfaceManager.AddService(GetType(System.Windows.Forms.PropertyGrid), _WidgetPropertyGrid.PropertyGrid)

        Common.ProjectPlayerBgBitmapName = ""
        Common.ProjectPlayerBgColour = Color.White
        Common.ProjectHtmlWebPagesFolder = String.Empty
        MyNameCreationService.ContainersCount = 0
        ToolStripMPLABX.Enabled = False
        ToolStripGenerateCode.Enabled = False
        ToolStripEditEnDis(False)
        ToolStripCloseProject.Enabled = False
        ToolStripSaveProject.Enabled = False
        ToolStripPlayer.Enabled = False
        ToolStripDeleteWidget.Enabled = False
        'ToolStripMPLABX.Enabled = False
        ToolStripBitmapChooser.Enabled = False
        ToolStripFontChooser.Enabled = False
        ToolStripdrpLanguage.Enabled = False
        tmrUpdateUndo.Enabled = True
        _SchemesChooser.Clear()
        _ProjectSummary.FootPrint1.Visible = False
        _WidgetPropertyGrid.PropertyGrid.SelectedObjects = Nothing
        _WidgetInfo.Enabled = False
        Common.ProjectChanged = False
        Common.WizardOptions.Clear()
        MplabX.MplabxFiles.Clear()
        MplabX.MplabxFilesOrig.Clear()
        Common.ProjectStringPool.Clear()

        CheckMplabXToolstrip()

        lblStatus.Text = "Ready"
    End Sub

    Private Sub ResizeMe()
        Try
            If My.Settings.MyTop < -20 Or _
                My.Settings.MyLeft < -20 Or _
                My.Settings.MyWidth < Me.MinimumSize.Width Or _
                My.Settings.MyHeight < Me.MinimumSize.Height Then
                DockLayoutReset()
            End If
            If Windows.Forms.Control.ModifierKeys And Keys.Shift Then
                My.Settings.MyTop = 0
                My.Settings.MyLeft = 0
            End If

            If Me.WindowState <> FormWindowState.Maximized Then
                Me.Top = My.Settings.MyTop
                Me.Left = My.Settings.MyLeft
                Me.Width = My.Settings.MyWidth
                Me.Height = My.Settings.MyHeight
            End If

        Catch ex As Exception
        End Try
    End Sub

    Private Function CheckModifiedProject() As DialogResult
        Dim blnUnsaved As Boolean
        If oFrmMagnify IsNot Nothing Then
            oFrmMagnify.Close()
            oFrmMagnify = Nothing
        End If
        CheckModifiedProject = DialogResult.Yes
        If Not Common.ProjectChanged Then
            For Each ocontent As DockContent In _DockPanel1.Contents
                If TypeOf (ocontent) Is HostControl Then
                    Dim oTabDoc As HostControl = ocontent
                    If oTabDoc.Text.EndsWith("*") Then
                        blnUnsaved = True
                        Exit For
                    End If
                End If
            Next
        End If
        If Common.ProjectChanged Or blnUnsaved Then
            CheckModifiedProject = MessageBox.Show("There are unsaved items. Save project?", "Save project", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            If CheckModifiedProject = DialogResult.Yes Then
                If Not SaveProject() Then
                    Return DialogResult.Cancel
                End If
            Else
                Return CheckModifiedProject
            End If
        End If
        Return Windows.Forms.DialogResult.Yes
    End Function

    Private Sub MenuCloseProject_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuCloseProject.Click
        CloseProject()
    End Sub

    Private Sub CloseProject()
        Select Case CheckModifiedProject()
            Case DialogResult.Yes, DialogResult.No
                Me.ClearProject()
                RefreshCmbScheme()
                DockLayoutHideAll()
                _MainSplash.Show(_DockPanel1)
                ToolStripMPLABX.Enabled = False
            Case DialogResult.Abort, DialogResult.Cancel
                Exit Sub
            Case Else
                Debug.Print("?")
        End Select
    End Sub

    Private Sub RebuildRecent(ByVal NewPathname As String)
        Try
            Dim NewRecent As New Collections.Specialized.StringCollection
            Dim oMenuItem As MenuItem
            Dim i As Integer = 1
            MenuOpenRecent.MenuItems.Clear()
            _MainSplash.lstRecent.Items.Clear()
            If NewPathname IsNot Nothing Then
                NewRecent.Add(NewPathname)
                _MainSplash.lstRecent.Items.Add(NewPathname)
                oMenuItem = New MenuItem("1 " & NewPathname)
                MenuOpenRecent.MenuItems.Clear()
                MenuOpenRecent.MenuItems.Add(oMenuItem)
                AddHandler (oMenuItem.Click), AddressOf MenuOpenRecent_Click
                i = 2
                MenuOpenRecent.Enabled = True
            End If
            If My.Settings.Recent IsNot Nothing Then
                For Each Recent As String In My.Settings.Recent
                    If Recent <> NewPathname AndAlso File.Exists(Recent) Then
                        _MainSplash.lstRecent.Items.Add(Recent)
                        NewRecent.Add(Recent)
                        oMenuItem = New MenuItem(String.Format("{0} {1}", i, Recent))
                        AddHandler (oMenuItem.Click), AddressOf MenuOpenRecent_Click
                        MenuOpenRecent.MenuItems.Add(oMenuItem)
                        If i = 16 Then Exit For
                        i += 1
                    End If
                Next
                MenuOpenRecent.Enabled = True
            End If
            My.Settings.Recent = NewRecent
        Catch ex As Exception
        End Try
    End Sub

    Private Sub _DockPanel1_ActivePaneChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _DockPanel1.ActivePaneChanged
        If _DockPanel1.ActivePane IsNot Nothing AndAlso TypeOf (_DockPanel1.ActivePane.ActiveContent) Is HostControl Then
            If Not Common.ProjectLoading Then
                ActiveHCChanged(_DockPanel1.ActivePane.ActiveContent)
            End If
        End If
    End Sub

    Private Sub _DockPanel1_ActiveDocumentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _DockPanel1.ActiveDocumentChanged
        If _DockPanel1.ActiveDocument IsNot Nothing AndAlso TypeOf (_DockPanel1.ActiveDocument) Is HostControl Then
            _DockPanel1.BringToFront()
            If Not Common.ProjectLoading Then
                ActiveHCChanged(_DockPanel1.ActiveDocument)
            End If
        End If
    End Sub

    Private Sub ActiveHCChanged(ByVal hc As HostControl)
        If hc.DockState = DockState.Hidden Then
            hc.FloatAt(New Rectangle(0, 0, 0, 0))
            hc.DockState = DockState.Document
        End If
        CheckMiniature(hc.Screen)
        If _CurrentHost Is hc Then
            Exit Sub
        End If
        If _selectionService IsNot Nothing Then
            RemoveHandler _selectionService.SelectionChanged, AddressOf Me._CurrentHost_SelectionChanged
            RemoveHandler _ComponentChangeService.ComponentAdded, AddressOf Me._CurrentHost_ControlAdded
            'RemoveHandler _ComponentChangeService.ComponentAdding, AddressOf Me._CurrentHost_ControlAdding
            RemoveHandler _ComponentChangeService.ComponentRemoved, AddressOf Me._CurrentHost_ControlRemoved
            RemoveHandler _ComponentChangeService.ComponentChanged, AddressOf Me._CurrentHost_ControlChanged
            _selectionService = Nothing
            _ComponentChangeService = Nothing
        End If
        _CurrentHost = hc
        If hc Is Nothing Then
            Common.CurrentScreen = Nothing
            Exit Sub 'Project closing
        End If

        Try
            Me.MenuEdit.Enabled = True
        Catch ex As Exception
        End Try

        If _CurrentHost.HostSurface Is Nothing Then
            Exit Sub
        End If

        _selectionService = CType(_CurrentHost.HostSurface.GetService(GetType(ISelectionService)), ISelectionService)
        If _selectionService IsNot Nothing Then
            AddHandler _selectionService.SelectionChanged, AddressOf Me._CurrentHost_SelectionChanged
        End If

        _ComponentChangeService = CType(_CurrentHost.HostSurface.GetService(GetType(IComponentChangeService)), IComponentChangeService)
        If _ComponentChangeService IsNot Nothing Then
            AddHandler _ComponentChangeService.ComponentAdded, AddressOf Me._CurrentHost_ControlAdded
            'AddHandler _ComponentChangeService.ComponentAdding, AddressOf Me._CurrentHost_ControlAdding
            AddHandler _ComponentChangeService.ComponentRemoved, AddressOf Me._CurrentHost_ControlRemoved
            AddHandler _ComponentChangeService.ComponentChanged, AddressOf Me._CurrentHost_ControlChanged
        End If

        Try
            _hostSurfaceManager.ActiveDesignSurface = _CurrentHost.HostSurface
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error ActiveHCChanged", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try
        oKeyStrokeMessageFilter.SetHostAndMDIForm(_CurrentHost.DesignerHost, _hostSurfaceManager.ActiveDesignSurface)
        Me._Toolbox.DesignerHost = _CurrentHost.DesignerHost
        If _hostSurfaceManager.ActiveDesignSurface Is Nothing Then
            Common.CurrentScreen = Nothing
            Exit Sub
        End If
        Common.CurrentScreen = _CurrentHost.Screen ' _hostSurfaceManager.ActiveDesignSurface.ComponentContainer.Components(0)
        If Common.CurrentScreen Is Nothing Then
            Exit Sub
        End If
        _WidgetPropertyGrid.PropertyGrid.SelectedObjects = _selectionService.GetSelectedComponents 'New Object() {Common.CurrentScreen}
        _CurrentHost.HostSurface.GetMyUndoEngine.Enabled = True
        tmrUpdateUndo.Enabled = True
        If _EventTree.tvEvents.Nodes.Count > 0 AndAlso _EventTree.tvEvents.Nodes(0).Nodes(Common.CurrentScreen.Name) IsNot Nothing Then
            TvSelectScreen()
            PopulateEvents(Common.CurrentScreen)
            If Not Common.ProjectLoading Then
                _CurrentHost_SelectionChanged(Nothing, Nothing)
            End If
        ElseIf Common.CurrentScreen.IsMasterScreen Then
            _CurrentHost_SelectionChanged(Nothing, Nothing)
        End If
    End Sub

    Public Function SaveProject() As Boolean
        If _SolutionExplorer.tvSolution.Nodes(0).Nodes("Screens").Nodes.Count = 0 Then Exit Function ' tabControl1.TabPages.Count 
        If Me._SolutionExplorer.ProjectPathName Is Nothing OrElse Me._SolutionExplorer.ProjectPathName = "New" Then
            Return SaveProjectAs()
        Else
            Return SaveProject(Me._SolutionExplorer.ProjectPathName)
        End If
    End Function

    Private strLastSavedProject As String
    Public Function SaveProjectAs() As Boolean
        Dim dlg As SaveFileDialog = New SaveFileDialog
        dlg.Title = "Save Project As..."
        dlg.DefaultExt = "vdp"
        dlg.Filter = "Visual Graphics Display Project Files|*.vdp"
        dlg.FileName = Common.ProjectName
        dlg.InitialDirectory = Common.VGDDProjectPath
        If Not dlg.FileName.EndsWith(".vdp") Then dlg.FileName = dlg.FileName & ".vdp"
        If (dlg.ShowDialog = DialogResult.OK) Then
            Me._SolutionExplorer.ProjectPathName = Path.Combine(Path.GetDirectoryName(dlg.FileName), Common.CleanName(Path.GetFileNameWithoutExtension(dlg.FileName)) & ".vdp")
            If Me.SaveProject(dlg.FileName, Path.GetDirectoryName(dlg.FileName)) Then
                strLastSavedProject = dlg.FileName
                UpdateCaption()
                Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function

    Public Function SaveProject(ByVal strFileName As String) As Boolean
        Return SaveProject(strFileName, Nothing)
    End Function

    Private Sub SaveProjectScreenNode(ByVal Nodes As TreeNodeCollection, ByVal ExportPath As String)
        Dim oXmlNode As XmlNode
        For Each oNode As TreeNode In Nodes
            If oNode.Nodes.Count > 0 Then
                SaveProjectScreenNode(oNode.Nodes, ExportPath)
            ElseIf TypeOf oNode.Tag Is VGDD.VGDDScreenAttr Then
                Dim oScreenAttr As VGDD.VGDDScreenAttr = oNode.Tag
                oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "Screen", "")
                Dim nameAttr As XmlAttribute = oXmlProjectDoc.CreateAttribute("FileName")
                Dim strScreenFileName As String = oScreenAttr.FileName
                If ExportPath IsNot Nothing Then
                    If Path.GetDirectoryName(strScreenFileName) = String.Empty Then 'new screens not yet saved
                        strScreenFileName = Path.Combine(ExportPath, strScreenFileName)
                    End If
                    If Not File.Exists(strScreenFileName) Then
                        MessageBox.Show("Screen " & strScreenFileName & " could not be found!", "Error exporting Project")
                        Common.ProjectSaving = False
                        Exit Sub
                    End If
                    Dim strScreenFileName2 As String = Path.Combine(ExportPath, Path.GetFileName(strScreenFileName))
                    If strScreenFileName2 <> strScreenFileName AndAlso Path.GetDirectoryName(strScreenFileName) <> String.Empty AndAlso File.Exists(strScreenFileName) Then
                        Try
                            File.Copy(strScreenFileName, strScreenFileName2, True)
                        Catch ex As Exception
                            MessageBox.Show("Cannot save" & vbCrLf & strScreenFileName & vbCrLf & "to" & vbCrLf & strScreenFileName2, "Error saving screen", MessageBoxButtons.OK)
                        End Try
                    End If
                    nameAttr.Value = Path.GetFileName(strScreenFileName2)
                Else
                    nameAttr.Value = RelativePath.Evaluate(Common.VGDDProjectPath, strScreenFileName)
                    Do While nameAttr.Value.StartsWith(".\")
                        nameAttr.Value = nameAttr.Value.Substring(2)
                    Loop
                End If
                oXmlNode.Attributes.Append(nameAttr)

                Dim shownAttr As XmlAttribute = oXmlProjectDoc.CreateAttribute("Shown")
                shownAttr.Value = oScreenAttr.Shown
                oXmlNode.Attributes.Append(shownAttr)

                oXmlProjectDoc.DocumentElement.AppendChild(oXmlNode)
            End If
        Next
    End Sub

    Public Function SaveProject(ByVal ProjectFileName As String, ByVal ExportPath As String) As Boolean
        Me.Cursor = Cursors.WaitCursor
        Try
            Common.ProjectPathName = ProjectFileName
            Me._SolutionExplorer.ProjectPathName = ProjectFileName

            Common.ProjectSaving = True
            oXmlProjectDoc = New XmlDocument
            Dim oProjectNode As XmlNode = oXmlProjectDoc.AppendChild(oXmlProjectDoc.CreateElement("VGDDProject"))
            Dim propertyAttributes() As Attribute = {}
            If _CurrentHost Is Nothing Then
                _CurrentHost = _hostSurfaceManager.GetNewHost(Nothing, GetType(VGDD.VGDDScreen))
            End If

            Dim errors As New ArrayList
            Dim oXmlNode As XmlNode
            Dim oAttr As XmlAttribute

            oAttr = oXmlProjectDoc.CreateAttribute("ColourDepth")
            oAttr.Value = Common.ProjectColourDepth
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("Width")
            oAttr.Value = Common.ProjectWidth
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("Height")
            oAttr.Value = Common.ProjectHeight
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("UseMultiByteChars")
            oAttr.Value = Common.ProjectUseMultiByteChars
            oProjectNode.Attributes.Append(oAttr)

            If Common.ProjectMultiLanguageTranslations > 0 Then
                oAttr = oXmlProjectDoc.CreateAttribute("MultiLanguageTranslations")
                oAttr.Value = Common.ProjectMultiLanguageTranslations
                oProjectNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("ActiveLanguage")
                oAttr.Value = Common.ProjectActiveLanguage
                oProjectNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("ProjectStringsPoolGenerateHeader")
                oAttr.Value = Common.ProjectStringsPoolGenerateHeader
                oProjectNode.Attributes.Append(oAttr)
            End If

            oAttr = oXmlProjectDoc.CreateAttribute("Compiler")
            oAttr.Value = Common.ProjectCompiler
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("UseIndexedCustomColours")
            oAttr.Value = Common.ProjectUsePalette

            Select Case Common.ProjectColourDepth
                Case 4, 8
                    oProjectNode.Attributes.Append(oAttr)
                Case Else
            End Select

            oAttr = oXmlProjectDoc.CreateAttribute("PlayerBgBitmap")
            oAttr.Value = Common.ProjectPlayerBgBitmapName
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("PlayerBgColour")
            oAttr.Value = String.Format("{0}, {1}, {2}", Common.ProjectPlayerBgColour.R, Common.ProjectPlayerBgColour.G, Common.ProjectPlayerBgColour.B)
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("CodeGenLocation")
            oAttr.Value = Common.CodeGenLocation
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("UseBmpPrefix")
            oAttr.Value = Common.ProjectUseBmpPrefix
            oProjectNode.Attributes.Append(oAttr)

            If Common.CodeGenLocation = 4 Then
                oAttr = oXmlProjectDoc.CreateAttribute("CodeGenDestPath")
                oAttr.Value = RelativePath.Evaluate(Common.VGDDProjectPath, Common.CodeGenDestPath)
                oProjectNode.Attributes.Append(oAttr)
            End If

            If Common.MplabXProjectPath <> "" Then
                oAttr = oXmlProjectDoc.CreateAttribute("MplabXProjectPath")
                oAttr.Value = RelativePath.Evaluate(Common.VGDDProjectPath, Common.MplabXProjectPath)
                If oAttr.Value = "" Then oAttr.Value = "./" 'VGDD Project in MplabX project folder
                oProjectNode.Attributes.Append(oAttr)
            End If

            If Common.MplabXSelectedConfig <> "" Then
                oAttr = oXmlProjectDoc.CreateAttribute("MplabXSelectedConfig")
                oAttr.Value = Common.MplabXSelectedConfig
                oProjectNode.Attributes.Append(oAttr)
            End If

            If Mal.MalPath <> "" Then
                oAttr = oXmlProjectDoc.CreateAttribute("MALPath")
                oAttr.Value = RelativePath.Evaluate(Common.VGDDProjectPath, Mal.MalPath)
                oProjectNode.Attributes.Append(oAttr)
            End If

            oAttr = oXmlProjectDoc.CreateAttribute("FrameworkName")
            oAttr.Value = Mal.FrameworkName
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("ProjectPathGRC")
            oAttr.Value = Common.ProjectPathGRC
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("CopyBitmapsInVgddProjectFolder")
            oAttr.Value = Common.ProjectCopyBitmapsInVgddProjectFolder
            oProjectNode.Attributes.Append(oAttr)

            If Common.BitmapsBinUsed Then
                oAttr = oXmlProjectDoc.CreateAttribute("BitmapsBinPath")
                oAttr.Value = Common.BitmapsBinPath
                oProjectNode.Attributes.Append(oAttr)
            End If

            oAttr = oXmlProjectDoc.CreateAttribute("HeapSize")
            oAttr.Value = Common.ProjectHeapSize
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("LastComputedHeapSize")
            oAttr.Value = Common.ProjectLastComputedHeapSize
            oProjectNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("VGDDVersion")
            oAttr.Value = Common.VGDDVERSION
            oProjectNode.Attributes.Append(oAttr)

            oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "HTMLEditor", "")
            oProjectNode.AppendChild(oXmlNode)

            oAttr = oXmlProjectDoc.CreateAttribute("SpitterDistance")
            oAttr.Value = _HTMLEditor.SplitContainer1.SplitterDistance
            oXmlNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("WebPagesFolder")
            oAttr.Value = Common.ProjectHtmlWebPagesFolder
            oXmlNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("OutputType")
            oAttr.Value = Common.ProjectHtmlOutputType
            oXmlNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("TargetURL")
            oAttr.Value = Common.ProjectHtmlTargetUrl
            oXmlNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("TargetUser")
            oAttr.Value = Common.ProjectHtmlTargetUser
            oXmlNode.Attributes.Append(oAttr)

            oAttr = oXmlProjectDoc.CreateAttribute("TargetPassword")
            oAttr.Value = Common.ProjectHtmlTargetPassword
            oXmlNode.Attributes.Append(oAttr)

            If Common.ProjectTouchAllScreens Then
                oAttr = oXmlProjectDoc.CreateAttribute("TouchAllScreens")
                oAttr.Value = Common.ProjectTouchAllScreens
                oXmlNode.Attributes.Append(oAttr)
            End If

            For Each oFont As VGDDFont In Common._Fonts
                oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "VGDDFont", "")
                oProjectNode.AppendChild(oXmlNode)

                oAttr = oXmlProjectDoc.CreateAttribute("Name")
                oAttr.Value = oFont.Name
                oXmlNode.Attributes.Append(oAttr)

                oFont.ToXml(oXmlNode)
                oXmlProjectDoc.DocumentElement.AppendChild(oXmlNode)
            Next

            If Common.ProjectMultiLanguageTranslations > 0 Then
                Common.ProjectActiveLanguage = 0
            End If
            For Each oContent As DockContent In _DockPanel1.Contents
                If TypeOf (oContent) Is HostControl Then
                    Dim oPage As HostControl = oContent
                    If oPage.Text.EndsWith("*") Then
                        Dim strOldName As String = oPage.Screen.Name
                        Dim strNewName As String = SaveScreenOnTab(oPage)
                        Dim oScreenNode As TreeNode = Me._SolutionExplorer.tvSolution.Nodes(0).Nodes("Screens").Nodes(strOldName)
                        If oScreenNode IsNot Nothing Then
                            Dim oScreenAttr As VGDD.VGDDScreenAttr = oScreenNode.Tag
                            If strNewName Is Nothing Then
                                oScreenNode.Remove()
                            Else
                                If strNewName = "KO" Then
                                    Common.ProjectSaving = False
                                    Return False
                                End If
                                If oScreenAttr.FileName Is Nothing OrElse Not oScreenAttr.FileName.Contains(".vds") OrElse strNewName <> oScreenAttr.FileName Then
                                    oPage.Name = Path.GetFileNameWithoutExtension(strNewName)
                                    oPage.Text = oPage.Name
                                    If oScreenNode Is Nothing Then
                                        Debug.Print("No?")
                                    Else
                                        oScreenNode.Name = oPage.Name
                                        oScreenNode.Text = oPage.Name
                                        oScreenAttr.FileName = oScreenAttr.Screen._FileName
                                        Me._SolutionExplorer.Refresh()
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Next

            For Each oVgddImage As VGDDImage In Common._Bitmaps
                oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "Bitmap", "")
                oProjectNode.AppendChild(oXmlNode)
                oAttr = oXmlProjectDoc.CreateAttribute("FileName")
                If ExportPath IsNot Nothing Then
                    oAttr.Value = Path.GetFileName(oVgddImage.FileName)
                    Dim strDestBitmapFilename As String = Path.GetFullPath(Path.Combine(ExportPath, Path.GetFileName(oVgddImage.FileName)))
                    If strDestBitmapFilename <> oVgddImage.FileName Then
                        Try
                            File.Copy(oVgddImage.FileName, strDestBitmapFilename, True)
                        Catch ex As Exception
                            MessageBox.Show("Error copying Bitmap" & vbCrLf & oVgddImage.FileName & vbCrLf & "to " & strDestBitmapFilename & vbCrLf & ex.Message)
                        End Try
                    End If
                Else
                    oAttr.Value = RelativePath.Evaluate(Path.GetDirectoryName(Me._SolutionExplorer.ProjectPathName), oVgddImage.FileName)
                    Do While oAttr.Value.StartsWith(".\")
                        oAttr.Value = oAttr.Value.Substring(2)
                    Loop
                End If
                oXmlNode.Attributes.Append(oAttr)

                If oVgddImage.Type = VGDDImage.PictureType.BINBMP_ON_SDFAT Then
                    oAttr = oXmlProjectDoc.CreateAttribute("SDFileName")
                    oAttr.Value = oVgddImage.SDFileName
                    oXmlNode.Attributes.Append(oAttr)
                End If

                oAttr = oXmlProjectDoc.CreateAttribute("Name")
                oAttr.Value = oVgddImage.Name
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("Type")
                oAttr.Value = oVgddImage.Type.ToString
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("CompressionType")
                oAttr.Value = oVgddImage.CompressionType.ToString
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("ColourDepth")
                oAttr.Value = oVgddImage.ColourDepth.ToString
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("AllowScaling")
                oAttr.Value = oVgddImage.AllowScaling.ToString
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("Size")
                oAttr.Value = oVgddImage.Size.ToString
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("InterpolationMode")
                oAttr.Value = oVgddImage.InterpolationMode.ToString
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("ForceInclude")
                oAttr.Value = oVgddImage.ForceInclude.ToString
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("GroupName")
                oAttr.Value = oVgddImage.GroupName
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("RotateFlip")
                oAttr.Value = oVgddImage.RotateFlip
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("BitmapSize")
                oAttr.Value = oVgddImage.BitmapSize
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("BitmapCompressedSize")
                oAttr.Value = oVgddImage.BitmapCompressedSize
                oXmlNode.Attributes.Append(oAttr)

                oXmlProjectDoc.DocumentElement.AppendChild(oXmlNode)
            Next

            For Each oScheme As VGDDScheme In Common._Schemes.Values
                oScheme.ToXml(oXmlProjectDoc)
            Next

            SaveProjectScreenNode(Me._SolutionExplorer.tvSolution.Nodes(0).Nodes("SCREENS").Nodes, ExportPath)

            oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "BitmapsBinPath", "")
            oProjectNode.AppendChild(oXmlNode)
            oAttr = oXmlProjectDoc.CreateAttribute("Path")
            If ExportPath IsNot Nothing Then
                oAttr.Value = "."
            Else
                oAttr.Value = RelativePath.Evaluate(Common.VGDDProjectPath, Common.BitmapsBinPath)
                Do While oAttr.Value.StartsWith(".\")
                    oAttr.Value = oAttr.Value.Substring(2)
                Loop
            End If
            oXmlNode.Attributes.Append(oAttr)
            oXmlProjectDoc.DocumentElement.AppendChild(oXmlNode)

            oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "Wizard", "")
            oProjectNode.AppendChild(oXmlNode)
            Dim oNodeWizard As XmlNode = oProjectNode.AppendChild(oXmlNode)

            oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "DevBoardID", "")
            oXmlNode.InnerText = Common.DevelopmentBoardID
            oNodeWizard.AppendChild(oXmlNode)

            If Common.PIMBoardID <> String.Empty Then
                oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "PIMBoardID", "")
                oXmlNode.InnerText = Common.PIMBoardID
                oNodeWizard.AppendChild(oXmlNode)
            End If

            oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "ExpansionBoardID", "")
            oXmlNode.InnerText = Common.ExpansionBoardID
            oNodeWizard.AppendChild(oXmlNode)

            oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "DisplayBoardID", "")
            oXmlNode.InnerText = Common.DisplayBoardID
            oNodeWizard.AppendChild(oXmlNode)

            oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "DisplayBoardOrientation", "")
            oXmlNode.InnerText = Common.DisplayBoardOrientation
            oNodeWizard.AppendChild(oXmlNode)

            For Each strOptionName As String In Common.WizardOptions.Keys
                oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "Option", "")

                oAttr = oXmlProjectDoc.CreateAttribute("Name")
                oAttr.Value = strOptionName
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("Checked")
                oAttr.Value = Common.WizardOptions(strOptionName)
                oXmlNode.Attributes.Append(oAttr)
                oNodeWizard.AppendChild(oXmlNode)
            Next

            If Common.ProjectMultiLanguageTranslations > 0 Then
                oXmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "MultiLanguageTranslations", "")

                oAttr = oXmlProjectDoc.CreateAttribute("Languages")
                oAttr.Value = Common.ProjectMultiLanguageTranslations
                oXmlNode.Attributes.Append(oAttr)

                oAttr = oXmlProjectDoc.CreateAttribute("SortColumn")
                oAttr.Value = Common.StringsPoolSortColumn
                oXmlNode.Attributes.Append(oAttr)

                Dim oNodeTranslations As XmlNode = oProjectNode.AppendChild(oXmlNode)

                For Each oStringSet As VGDDCommon.MultiLanguageStringSet In Common.ProjectStringPool.Values
                    Dim oStringNode As XmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "String", "")
                    oAttr = oXmlProjectDoc.CreateAttribute("ID")
                    oAttr.Value = oStringSet.StringID
                    oStringNode.Attributes.Append(oAttr)
                    If oStringSet.StringAltID <> String.Empty Then
                        oAttr = oXmlProjectDoc.CreateAttribute("AltID")
                        oAttr.Value = oStringSet.StringAltID
                        oStringNode.Attributes.Append(oAttr)
                    End If
                    oAttr = oXmlProjectDoc.CreateAttribute("InUse")
                    oAttr.Value = oStringSet.InUse
                    oStringNode.Attributes.Append(oAttr)
                    oAttr = oXmlProjectDoc.CreateAttribute("AutoWrap")
                    oAttr.Value = oStringSet.AutoWrap
                    oStringNode.Attributes.Append(oAttr)
                    oAttr = oXmlProjectDoc.CreateAttribute("Dynamic")
                    oAttr.Value = oStringSet.Dynamic
                    oStringNode.Attributes.Append(oAttr)
                    oAttr = oXmlProjectDoc.CreateAttribute("FontName")
                    oAttr.Value = oStringSet.FontName
                    oStringNode.Attributes.Append(oAttr)
                    Dim oStringValueNode As XmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "Value", "")
                    oStringValueNode.InnerText = oStringSet.Strings(0)
                    oStringNode.AppendChild(oStringValueNode)
                    oNodeTranslations.AppendChild(oStringNode)
                    For j As Integer = 1 To Common.ProjectMultiLanguageTranslations
                        Dim oTransNode As XmlNode = oXmlProjectDoc.CreateNode(XmlNodeType.Element, "", "Translation", "")
                        oAttr = oXmlProjectDoc.CreateAttribute("LangID")
                        oAttr.Value = j
                        oTransNode.Attributes.Append(oAttr)
                        If j < oStringSet.Strings.Length Then
                            oTransNode.InnerText = oStringSet.Strings(j)
                        End If
                        oStringNode.AppendChild(oTransNode)
                    Next
                Next
            End If

            Dim sw As New StringWriter
            Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
            xtw.Formatting = Formatting.Indented
            oXmlProjectDoc.WriteTo(xtw)
            Try
                Common.WriteFileWithBackup(ProjectFileName, sw.ToString, New System.Text.UnicodeEncoding)

            Catch ex As Exception
                MessageBox.Show("Error writing to " & ProjectFileName & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            ToolStripSaveProject.Enabled = False
            Common.ProjectChanged = False
            Common.ProjectSaving = False
            RebuildRecent(ProjectFileName)
            Return True

        Catch ex As Exception
            MessageBox.Show("Cannot save project to " & ProjectFileName & vbCrLf & ex.Message, "Error saving project", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Function

    Public Function SaveScreenOnTab(ByVal oPage As HostControl) As String
        If oPage Is Nothing Then
            Return Nothing
        End If
        Dim oLoader As BasicHostLoader = oPage.HostSurface.Loader
        If Not oLoader.Save(oPage.Screen.Name) Then
            Return "KO"
        End If
        If oPage.Text.EndsWith("*") Then
            oPage.Text = oPage.Text.Substring(0, oPage.Text.Length - 1)
        End If
        oPage.Screen._FileName = oLoader.FileName
        Return oPage.Screen.Name
    End Function

    Private Sub ExportScreenMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuExportScreen.Click
        If _CurrentHost Is Nothing OrElse _CurrentHost.HostSurface Is Nothing Then
            MessageBox.Show("No screen selected. Please select screen before exporting")
            Exit Sub
        End If
        Try
            _CurrentHost.HostSurface.Loader.Save(_CurrentHost.Name, True, True)
            Me._OutputWindow.RichTextBox.Text = (Me._OutputWindow.RichTextBox.Text + "Saved host." & vbLf)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error exporting screen")
        End Try
        'End If
    End Sub

    Private Sub OpenProjectMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuOpenProject.Click
        OpenProjectDialog("")
    End Sub

    Private Sub OpenProjectDialog(ByVal strDir As String)
        Dim strProjectFileName As String = Nothing
        If CheckModifiedProject() = DialogResult.Cancel Then Exit Sub
        ' Open File Dialog
        Dim dlg As OpenFileDialog = New OpenFileDialog
        dlg.DefaultExt = "vds"
        dlg.Filter = "Visual Graphics Display Project Files|*.vdp"
        dlg.RestoreDirectory = False
        If strDir <> "" Then dlg.InitialDirectory = strDir
        If (dlg.ShowDialog = Windows.Forms.DialogResult.OK) Then
            strProjectFileName = dlg.FileName
        End If
        If (strProjectFileName = Nothing) Then
            Return
        End If
        OpenProject(strProjectFileName)
    End Sub

    Private Sub MenuOpenRecent_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim oMenuItem As MenuItem = sender
        If CheckModifiedProject() = DialogResult.Cancel Then
            Exit Sub
        End If
        Dim strRecentProjectPathName As String = oMenuItem.Text.Substring(oMenuItem.Text.IndexOf(" ")).Trim
        If Not OpenProject(strRecentProjectPathName) Then
            RebuildRecent(Nothing)
            Exit Sub
        End If

        RebuildRecent(Common.ProjectPathName)
    End Sub

    Public Sub CheckMplabXToolstrip()
        Select Case Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                ToolStripMPLABX.Enabled = True
            Case "MLA"
                ToolStripMPLABX.Enabled = True
            Case "HARMONY"
                ToolStripMPLABX.Enabled = False
        End Select
    End Sub

    Private Function OpenProject(ByVal ProjectFileName As String) As Boolean
        Try
            ClearProject()
            _HTMLEditor.btnSettings.Enabled = True
            dProjectLoadStart = Now
            lblStatus.Text = "Opening Project " & ProjectFileName & "..."
            lblMouseCoords.Text = ""
            Application.DoEvents()
            Common.ProjectLoading = True
            _SolutionExplorer.ProjectPathName = ProjectFileName
            Common.ProjectPathName = ProjectFileName
            UpdateCaption()
            Common.BitmapsBinPath = Common.VGDDProjectPath

            DockLayoutUnhideAll()
            _MainSplash.Hide()
            ToolStripNewScreen.Enabled = False
            ToolStripProjectSettings.Enabled = False
            ToolStripOpen.Enabled = False
            ToolStripdrpLanguage.Enabled = False

            Try
                oXmlProjectDoc = New XmlDocument
                oXmlProjectDoc.PreserveWhitespace = True
                oXmlProjectDoc.Load(ProjectFileName)
                If Common.LoadProject(oXmlProjectDoc.DocumentElement) = "ASKFRAMEWORK" Then
                    MessageBox.Show("This project has been created with a previous version of VGDD and the Framework to be used cannot be determined." & vbCrLf & _
                    vbCrLf & "Please click OK and proceed to the Framework selection to tell VGDD which one (MLALegacy, MLA or Harmony) has to be used for this project.", "Framework selection")
                    frmSelectMAL.ShowDialog()
                End If
                RefreshCmbScheme()
            Catch ex As Exception
                MessageBox.Show("Error loading project: " & ex.Message)
                If My.Settings.Recent IsNot Nothing AndAlso My.Settings.Recent.Contains(ProjectFileName) Then
                    My.Settings.Recent.Remove(ProjectFileName)
                End If
                ToolStripNewScreen.Enabled = True
                ToolStripProjectSettings.Enabled = True
                ToolStripOpen.Enabled = True
                Return False
            End Try

            With _SolutionExplorer
                Dim intMiniatureW As Integer, intMiniatureH As Integer
                intMiniatureW = Common.ProjectWidth \ 2
                intMiniatureH = Common.ProjectHeight \ 2
                If intMiniatureW > intMiniatureH Then
                    If intMiniatureW <= 256 Then
                        .lstThumbnails.LargeImageList.ImageSize = New Drawing.Size(intMiniatureW, intMiniatureH)
                    Else
                        .lstThumbnails.LargeImageList.ImageSize = New Drawing.Size(256, 256 * intMiniatureH \ intMiniatureW)
                    End If
                Else
                    If intMiniatureH <= 256 Then
                        .lstThumbnails.LargeImageList.ImageSize = New Drawing.Size(intMiniatureW, intMiniatureH)
                    Else
                        .lstThumbnails.LargeImageList.ImageSize = New Drawing.Size(256 * intMiniatureW \ intMiniatureH, 256)
                    End If
                End If
                .lstThumbnails.LargeImageList.ColorDepth = ColorDepth.Depth32Bit
            End With

            Dim intActiveLanguage As Integer = Common.ProjectActiveLanguage
            Common.ProjectActiveLanguage = 0
            Dim i As Integer = 0
            Do While i < Common.aScreens.Count
                Try
                    Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens.Item(i)
                    lblStatus.Text = "Loading screen " & oScreenAttr.FileName
                    Application.DoEvents()
                    OpenScreen(oScreenAttr.FileName, oScreenAttr.Shown)
                    CheckMiniature(oScreenAttr.Screen)

                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error opening screen")
                End Try
                i += 1
            Loop
            Common.ProjectActiveLanguage = intActiveLanguage

            lblStatus.Text = "Project loaded!"
            Application.DoEvents()

            Me._SolutionExplorer.tvSolution.ExpandAll()
            Me._SolutionExplorer.tvSolution.Nodes(0).EnsureVisible()
            RebuildRecent(ProjectFileName)
            ToolStripNewScreen.Enabled = True
            ToolStripProjectSettings.Enabled = True
            ToolStripOpen.Enabled = True
            ToolStripPlayer.Enabled = True
            ToolStripGenerateCode.Enabled = True
            ToolStripBringToFront.Enabled = True
            ToolStripSendToBack.Enabled = True
            '#If CONFIG = "Release" Or CONFIG = "Debug" Then
            CheckMplabXToolstrip()
            MenuMPLABXWizard.Enabled = True
            ToolStripCloseProject.Enabled = True
            ToolStripBitmapChooser.Enabled = True
            ToolStripFontChooser.Enabled = True

            Me._Toolbox.InitializeToolbox()

            If Common.ProjectMultiLanguageTranslations = 0 Then
                ToolStripdrpLanguage.Enabled = False
            Else
                ToolStripdrpLanguage.Enabled = True
                ToolStripdrpLanguage.DropDownItems.Clear()
                ToolStripdrpLanguage.DropDownItems.Add("Edit StringsPool")
                ToolStripdrpLanguage.DropDownItems.Add("Reference Language")
                For i = 1 To Common.ProjectMultiLanguageTranslations
                    Me.ToolStripdrpLanguage.DropDownItems.Add("Translation " & i)
                Next
            End If
            MyNameCreationService.ContainersCount = Common.aScreens.Count
            Common.ProjectLoading = False
            Try
                If _DockPanel1.ActivePane IsNot Nothing AndAlso TypeOf (_DockPanel1.ActivePane.ActiveContent) Is HostControl Then
                    ActiveHCChanged(_DockPanel1.ActivePane.ActiveContent)
                ElseIf Common.aScreens.Count > 0 AndAlso Common.aScreens(0) IsNot Nothing Then
                    Dim newHost As HostControl = FindScreen(Common.aScreens(0).Name)
                    If newHost IsNot Nothing Then
                        ActiveHCChanged(newHost)
                    End If
                End If
            Catch ex As Exception
            End Try
            LastSelection = Nothing
            lblStatus.Text = "Ready"
            lblMouseCoords.Text = ""
            If Common.HtmlEditorSplitterDistance > 0 AndAlso _HTMLEditor.SplitContainer1.Width > Common.HtmlEditorSplitterDistance Then
                Try
                    _HTMLEditor.SplitContainer1.SplitterDistance = Common.HtmlEditorSplitterDistance
                Catch ex As Exception
                End Try
            End If
            If _OutputWindow.RichTextBox.Text <> String.Empty Then
                If _OutputWindow.DockState > 2 And _OutputWindow.DockState < 5 Then
                    _OutputWindow.Show(_OutputWindow.DockPanel, _OutputWindow.DockState + 5)
                End If
            End If

            If Common.ProjectTouchAllScreens Then
                For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                    Common.TouchScreen(oScreenAttr.Screen.Name)
                Next
                Common.ProjectTouchAllScreens = False
            End If

            Return True

        Catch ex As Exception
            lblMouseCoords.Text = ""
            MessageBox.Show("Problems loading Project " & ProjectFileName & vbCrLf & "Try to exit and re-launch application", "Problems loading project file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblStatus.Text = "Cannot open project"
        End Try
        Return False
    End Function

    Private Sub MenuScreenAddExisting_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuScreenAddExisting.Click
        Try
            Dim fileNames As String() = Nothing
            Dim dlg As OpenFileDialog = New OpenFileDialog
            dlg.DefaultExt = "vds"
            dlg.Multiselect = True
            dlg.RestoreDirectory = False
            dlg.Filter = "Visual Graphics Display Screen Files|*.vds" 'Visual Graphics Display Project Files|*.vdp
            If (dlg.ShowDialog = Windows.Forms.DialogResult.OK) Then
                fileNames = dlg.FileNames
            Else
                Return
            End If
            If Common._Schemes.Count = 0 Then
                Common.NewScheme(Nothing)
            End If
            For Each fileName As String In dlg.FileNames
                If fileName.EndsWith(".vds") Then
                    Common.aScreens.Add(fileName, True)
                    OpenScreen(fileName, True)
                    Common.aScreens(fileName).Shown = True
                End If
            Next
            ProjectChanged()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub OpenScreen(ByRef XmlNode As XmlNode)
        Try
            _CurrentHost = _hostSurfaceManager.GetNewHost(XmlNode, GetType(VGDD.VGDDScreen))
            AddTabForNewHost(XmlNode.Attributes("Name").Value, _CurrentHost, True)
            Common.CurrentScreen = _hostSurfaceManager.ActiveDesignSurface.ComponentContainer.Components(0)

        Catch ex As Exception
            MessageBox.Show("Error in creating new host", "Shell Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Public Sub RemoveScreenFromProject(ByVal ScreenName As String)
        Dim aNodes As TreeNode() = _SolutionExplorer.tvSolution.Nodes(0).Nodes("SCREENS").Nodes.Find(ScreenName, True)
        Common.ProjectChanged = True
        If aNodes.Length = 0 Then Exit Sub
        Dim oNode As TreeNode = aNodes(0)
        If oNode IsNot Nothing Then oNode.Remove()
        If Common.aScreens.ContainsKey(ScreenName.ToUpper) Then
            Common.aScreens.Remove(ScreenName)
        End If
        If _SolutionExplorer.lstThumbnails.LargeImageList.Images.ContainsKey(ScreenName) Then
            _SolutionExplorer.lstThumbnails.LargeImageList.Images.RemoveByKey(ScreenName)
            _SolutionExplorer.lstThumbnails.Items.RemoveByKey(ScreenName)
        End If
        Dim oContent As IDockContent = FindScreen(ScreenName)
        If oContent IsNot Nothing AndAlso TypeOf (oContent) Is HostControl Then
            Dim hc As HostControl = oContent
            If hc.Screen IsNot Nothing AndAlso hc.Screen.FileName IsNot Nothing Then
                CloseScreen(ScreenName, True)
            End If
        End If
        If _EventTree.tvEvents.Nodes.Count > 0 Then
            oNode = _EventTree.tvEvents.Nodes(0).Nodes(ScreenName)
            If oNode IsNot Nothing Then
                oNode.Remove()
            End If
        End If
    End Sub

    Public Function OpenScreen(ByVal FileName As String, ByVal blnShow As Boolean) As HostControl
        Dim hc As HostControl = Nothing

        Dim oScreenAttr As VGDD.VGDDScreenAttr
        Try
            Common.ScreenLoading = True
            For Each oScreenAttr In Common.aScreens.Values
                If oScreenAttr.FileName = FileName Then
                    If oScreenAttr.Hc IsNot Nothing Then
                        hc = oScreenAttr.Hc
                        If hc.IsDisposed Then
                            hc = Nothing
                        End If
                        Exit For
                    End If
                End If
            Next

            Application.DoEvents()
            If hc Is Nothing Then
                Try
                    hc = _hostSurfaceManager.GetNewHost(FileName)
                    If hc Is Nothing Then
                        If MessageBox.Show(String.Format("Remove {0} screen from project?", FileName), "Invalid Screen", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = vbYes Then
                            RemoveScreenFromProject(Path.GetFileNameWithoutExtension(FileName))
                        End If
                        Common.ScreenLoading = False
                        Return Nothing
                        Exit Function
                    End If
                    If hc.Loader.errors.Count > 0 Then
                        For Each oError As String In hc.Loader.errors
                            _OutputWindow.WriteMessage(oError)
                        Next
                    End If
                    oScreenAttr = Common.aScreens(hc.Screen.Name)
                    oScreenAttr.Hc = hc
                    oScreenAttr.Screen = hc.Screen
                Catch ex As System.Exception
                    MessageBox.Show("Error in creating new host" & vbCrLf & ex.Message, "Shell Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Common.ScreenLoading = False
                    Return Nothing
                End Try
            Else
                oScreenAttr = Common.aScreens(hc.Screen.Name)
            End If

            Application.DoEvents()
            Dim oScreen As VGDD.VGDDScreen = hc.HostSurface.ComponentContainer.Components(0)
            oScreen._FileName = FileName

            Common.SetNewZOrder(oScreen)
            oScreenAttr.Shown = blnShow
            If Not blnShow Then
                Dim strScreenName As String = oScreen.ToString.Split(" ")(0)
                oScreen.Name = strScreenName
                'oScreen.ScreenShotClear()
                Common.aScreens(strScreenName.ToUpper).Screen = oScreen
                AddScreenToProjectExplorer(strScreenName, FileName)
                If Not Common.ProjectLoading Then PopulateEvents(oScreen)
            Else
                AddTabForNewHost(FileName, hc, True)
                Common.CurrentScreen._FileName = FileName
                PopulateEvents(Common.CurrentScreen)
            End If

            If Common.ProjectMultiLanguageTranslations > 0 Then
                For Each oControl As Control In oScreen.Controls
                    If TypeOf oControl Is VGDDMicrochip.VGDDWidget Then
                        Dim oWidget As VGDDMicrochip.VGDDWidget = oControl
                        If Not Common.ProjectStringPool.ContainsKey(oWidget.TextStringID) Then
                            For Each oStringSet As VGDDCommon.MultiLanguageStringSet In VGDDCommon.Common.ProjectStringPool.Values
                                If oStringSet.Strings(0) = oWidget.Text Then
                                    oWidget.TextStringID = oStringSet.StringID
                                    VGDDCommon.Common.TouchScreen(oScreen.Name)
                                    Exit For
                                End If
                            Next
                        Else
                            Dim strText As String = Common.ProjectStringPool(oWidget.TextStringID).Strings(0)
                            If oWidget.Text <> strText Then
                                oWidget.Text = strText
                                VGDDCommon.Common.TouchScreen(oScreen.Name)
                            End If
                        End If
                    End If
                Next
            End If

            Common.ScreenLoading = False

        Catch ex As Exception
            MessageBox.Show(ex.Message & vbCrLf & "Please close and reopen application as soon as possible", "OpenScreen Error")
        End Try

        Return hc
    End Function

    Private Sub MenuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuExit.Click
        Me.Close()
    End Sub

    Private Sub AddScreenToProjectExplorer(ByVal strScreenName As String, ByVal filename As String)
        Dim aNodes() As TreeNode = Me._SolutionExplorer.tvSolution.Nodes(0).Nodes("SCREENS").Nodes.Find(strScreenName, True)
        If aNodes.Length > 0 Then Return
        Me._SolutionExplorer.AddFileNode(strScreenName, filename, "SCREENS", 1)
    End Sub

    Public Sub AddTabForNewHost(ByVal tabText As String, ByVal hc As HostControl, ByVal blnShow As Boolean)
        'Dim oTabPage As TabPage = New TabPage
        Dim blnNew As Boolean = False
        Dim strScreenName As String
        If hc Is Nothing Then Exit Sub
        Try
            Common.CurrentScreen = hc.HostSurface.ComponentContainer.Components(0)
            strScreenName = Common.CurrentScreen.Name '.ToString.Split(" ")(0)
            If hc.Text = "*" Then
                hc.Text = strScreenName & "*"
            Else
                hc.Text = strScreenName
            End If
            hc.Dock = DockStyle.Fill
            If blnShow And Not Common.ProjectLoading Then
                ActiveHCChanged(hc)
            End If
            hc.ShowHint = DockState.Document
            hc.Dock = DockStyle.None
            hc.CloseButtonVisible = True
            hc.CloseButton = True
            hc.Icon = New Icon(GetResource("monitor.ico", Assembly.GetExecutingAssembly))
            If blnShow AndAlso (hc.DockState = DockState.Unknown Or hc.DockState = DockState.Hidden) Then
                hc.Show(_DockPanel1, DockState.Document)
            End If
            hc.DockHandler.GetPersistStringCallback = AddressOf hc.GetPersistingStringCallBack
            hc.DockHandler.HideOnClose = True
            If blnShow Then
                _hostSurfaceManager.ActiveDesignSurface = hc.HostSurface
                Me.MenuEdit.Enabled = True
                Application.DoEvents()
            End If
            If Not Common.aScreens.ContainsKey(strScreenName.ToUpper) Then
                blnNew = True
                If Not tabText.Contains(Path.DirectorySeparatorChar) Then
                    Try
                        Common.CurrentScreen.Height = Common.ProjectHeight
                        Common.CurrentScreen.Width = Common.ProjectWidth
                    Catch ex As Exception
                    End Try
                End If
                Common.aScreens.Add(strScreenName, Common.CurrentScreen)
            End If
            Common.CurrentScreen.Name = strScreenName 'Common.CurrentScreen.ToString.Split(" ")(0) ' strScreenName ZIOBECCO!
            Common.aScreens(Common.CurrentScreen.Name.ToUpper).Screen = Common.CurrentScreen
            If blnShow Or Common.ProjectLoading Then
                PopulateEvents(Common.CurrentScreen)
            End If

            AddScreenToProjectExplorer(strScreenName, tabText)
            Try
                For Each oControl As Control In Common.CurrentScreen.Controls
                    If TypeOf oControl Is VGDDMicrochip.OutTextXY Then
                        oControl.Invalidate()
                    End If
                Next
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try
    End Sub

    Public Sub PopulateEvents(ByRef oScreen As VGDD.VGDDScreen)
        Try
            If Not Common.ProjectLoading Then
                _EventTree.tvEvents.BeginUpdate()
            End If
            If _EventTree.tvEvents.Nodes.Count = 0 Then
                _EventTree.tvEvents.Nodes.Add(Common.ProjectName)
                _EventTree.tvEvents.Font = Common.oEventsFontBold
            End If

            If oScreen Is Nothing Then Exit Sub
            Dim oScreenNode As TreeNode
            oScreenNode = _EventTree.tvEvents.Nodes(0).Nodes(oScreen.Name)
            If oScreenNode IsNot Nothing Then
                oScreenNode.Nodes.Clear()
            Else
                oScreenNode = _EventTree.tvEvents.Nodes(0).Nodes.Add(oScreen.Name, oScreen.Name & "   ")
                oScreenNode.NodeFont = Common.oEventsFontNormal
            End If
            oScreenNode.Tag = oScreen
            oScreenNode.ImageIndex = 0
            oScreenNode.SelectedImageIndex = 0
            For Each oControl As Control In oScreen.Controls
                PopulateControlEvents(oControl, False)
            Next
            If Not Common.ProjectLoading Then
                oScreenNode.Expand()
                _EventTree.tvEvents.Nodes(0).Expand()
                _EventTree.tvEvents.EndUpdate()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub PopulateControlEvents(ByRef oControl As Control, ByVal blnExpand As Boolean)
        If oControl.Parent Is Nothing OrElse Not oControl.Parent.GetType Is GetType(VGDD.VGDDScreen) Then Exit Sub
        Dim oScreen As VGDD.VGDDScreen = oControl.Parent
        If oScreen Is Nothing Then Exit Sub
        Dim oScreenNode As TreeNode, oControlNode As TreeNode
        oScreenNode = _EventTree.tvEvents.Nodes(0).Nodes(oScreen.Name)
        If oScreenNode Is Nothing Then
            oScreenNode = _EventTree.tvEvents.Nodes(0).Nodes.Add(oScreen.Name, oScreen.Name)
        End If
        oControlNode = oScreenNode.Nodes(oControl.Name)
        If oControlNode Is Nothing Then
            oControlNode = oScreenNode.Nodes.Add(oControl.Name, oControl.Name)
        Else
            oControlNode.Nodes.Clear()
        End If
        oControlNode.ImageIndex = 1
        oControlNode.SelectedImageIndex = 1

        oControlNode.NodeFont = Common.oEventsFontNormal
        If TypeOf oControl Is Common.IVGDDEvents Then
            Dim oEventsCollection As VGDDEventsCollection = CType(oControl, Common.IVGDDEvents).VGDDEvents
            If oEventsCollection IsNot Nothing Then
                oControlNode.Tag = oEventsCollection
                For Each oEvent As VGDDEvent In oEventsCollection
                    Dim oEventNode As TreeNode = oControlNode.Nodes.Add(oEvent.Name, oEvent.Name)
                    If oEvent.Code <> String.Empty Then
                        oControlNode.NodeFont = Common.oEventsFontBold
                        oEventNode.NodeFont = oControlNode.NodeFont
                        oEventNode.SelectedImageIndex = 3
                        oEventNode.ImageIndex = 3
                        If blnExpand Then
                            oControlNode.Expand()
                            oScreenNode.Expand()
                        End If
                    Else
                        oEventNode.SelectedImageIndex = 2
                        oEventNode.ImageIndex = 2
                        oEventNode.NodeFont = Common.oEventsFontNormal
                    End If
                    oEventNode.Tag = oEvent
                    oEventNode.ToolTipText = oEvent.Description
                Next
            End If
        End If
    End Sub

    Private Sub MenuScreenNewItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuScreenNew.Click
        Try
            _MainSplash.Hide()
            DockLayoutUnhideAll()
            If Common.SelectedScheme Is Nothing Then
                _SchemesChooser.mnuNew.PerformClick()
            End If
            Dim hc As HostControl = _hostSurfaceManager.GetNewHost(GetType(VGDD.VGDDScreen), LoaderType.BasicDesignerLoader)
            Common.CurrentScreen = hc.Screen '_CurrentHost.HostSurface.ComponentContainer.Components(0)
            '_CurrentHost.Controls(0).Controls(0).Controls(0).Name = "Screen" & _formCount.ToString
            Common.CurrentScreen.Name = hc.HostSurface.ComponentContainer.Components(0).ToString.Split(" ")(0)
            Common.CurrentScreen.Height = Common.ProjectHeight
            Common.CurrentScreen.Width = Common.ProjectWidth
            Common.aScreens.Add(Common.CurrentScreen.Name, Common.CurrentScreen)
            Common.aScreens(Common.CurrentScreen.Name).Shown = True
            AddTabForNewHost(Common.CurrentScreen.Name, hc, True)
            _CurrentHost = hc
            If Not hc.Text.EndsWith("*") Then
                hc.Text &= "*"
            End If
            ToolStripEditEnDis(True)
            ToolStripGenerateCode.Enabled = True
            ToolStripPlayer.Enabled = True
            ToolStripCloseProject.Enabled = True
            ToolStripBitmapChooser.Enabled = True
            ToolStripFontChooser.Enabled = True
            MenuMPLABXWizard.Enabled = True
            _HTMLEditor.btnSettings.Enabled = True
            _SolutionExplorer.tvSolution.ExpandAll()
            ProjectChanged()

        Catch ex As System.Exception
            MessageBox.Show("Error in creating new host", "Shell Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ToolStripEditEnDis(ByVal EnDis As Boolean)
        ToolStripCopy.Enabled = EnDis
        ToolStripCut.Enabled = EnDis
        ToolStripPaste.Enabled = EnDis
        ToolStripBringToFront.Enabled = EnDis
        ToolStripSendToBack.Enabled = EnDis
    End Sub

#Region "Edit Actions & Shortcuts"

    Private Sub MenuEditClick(ByVal sender As Object, ByVal e As EventArgs) Handles _
            MenuAlignTops.Click, _
            MenuSelectAll.Click, _
            MenuAlignRights.Click, _
            MenuPaste.Click, _
            MenuAlignMiddles.Click, _
            MenuAlignLefts.Click, _
            MenuDelete.Click, _
            MenuCut.Click, _
            MenuCopy.Click, _
            MenuAlignCenters.Click, _
            MenuAlignBottoms.Click, _
            MenuSendToBack.Click, _
            MenuBringToFront.Click
        If (_CurrentHost Is Nothing) Then
            Return
        End If

        Dim ims As IMenuCommandService = CType(_CurrentHost.HostSurface.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        If ims Is Nothing Then
            Exit Sub
        End If
        Dim strMenuText As String = (CType(sender, MenuItem).Text)
        If strMenuText.Contains(" (") Then
            strMenuText = strMenuText.Split("(")(0).Trim
        End If
        Try
            Select Case strMenuText
                Case "&Cut"
                    ims.GlobalInvoke(StandardCommands.Cut)
                Case "C&opy"
                    ims.GlobalInvoke(StandardCommands.Copy)
                Case "&Paste"
                    ims.GlobalInvoke(StandardCommands.Paste)
                Case "&Undo"
                    ims.GlobalInvoke(StandardCommands.Undo)
                Case "&Redo"
                    ims.GlobalInvoke(StandardCommands.Redo)
                Case "&Delete"
                    ims.GlobalInvoke(StandardCommands.Delete)
                    PopulateEvents(Common.CurrentScreen)
                Case "&Select All"
                    ims.GlobalInvoke(StandardCommands.SelectAll)
                Case "Select &None"
                    ims.GlobalInvoke(StandardCommands.Properties)
                Case "Select &None"
                    ims.GlobalInvoke(StandardCommands.Properties)
                Case "&Lefts"
                    ims.GlobalInvoke(StandardCommands.AlignLeft)
                Case "&Centers"
                    ims.GlobalInvoke(StandardCommands.AlignHorizontalCenters)
                Case "&Rights"
                    ims.GlobalInvoke(StandardCommands.AlignRight)
                Case "&Tops"
                    ims.GlobalInvoke(StandardCommands.AlignTop)
                Case "&Middles"
                    ims.GlobalInvoke(StandardCommands.AlignVerticalCenters)
                Case "&Bottoms"
                    ims.GlobalInvoke(StandardCommands.AlignBottom)
                Case "Bring to &Front"
                    ims.GlobalInvoke(StandardCommands.BringToFront)
                Case "Send To Bac&k"
                    ims.GlobalInvoke(StandardCommands.SendToBack)
                Case "F1Help"
                    ims.GlobalInvoke(StandardCommands.F1Help)
                Case Else
                    Debug.Print("IMenuCommandService *" & Text & "*")
            End Select
        Catch ex As System.Exception
            Me._OutputWindow.RichTextBox.Text = (Me._OutputWindow.RichTextBox.Text + ("Error in performing the action: " + Text.Replace("&", "")))
        End Try
    End Sub

    Private Sub PerformAction(ByVal text As String)
    End Sub

#End Region

    Private Sub openMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuProjectSettings.Click
        Dim oProjectSettings As New frmProjectSettings
        oProjectSettings.ShowDialog()
    End Sub

#Region "Widget/Screen/Project changed"
    Public Sub ProjectChanged()
        Common.ProjectChanged = True
        ToolStripSaveProject.Enabled = True
    End Sub

    Public Sub ScreenChanged()
        ScreenChanged(Nothing)
    End Sub

    Public Sub ScreenChanged(ByVal strScreenName As String)
        tmrUpdateUndo.Enabled = True
        Dim hc As HostControl
        If strScreenName Is Nothing Then
            hc = _CurrentHost
        Else
            hc = FindScreen(strScreenName)
            If hc Is Nothing Then
                Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(strScreenName)
                hc = oScreenAttr.Hc
            End If
        End If
        If hc Is Nothing Then
            Exit Sub
        End If
        If Not hc.Text.EndsWith("*") Then
            hc.Text &= "*"
            hc.Loader.unsaved = True
        End If
        'If Not hc.Loader.IsDirty Then
        '    hc.HostSurface.Flush()
        '    hc.Loader.PerformFlushWorker()
        'End If
        ProjectChanged()
        _WidgetPropertyGrid.PropertyGrid.Refresh()
    End Sub

    Public Sub RenameScreen(ByVal OldName As String, ByVal NewName As String)
        Dim oNode As TreeNode
        If Common.CurrentScreen IsNot Nothing Then
            Common.CurrentScreen.Name = NewName
            oNode = _EventTree.tvEvents.Nodes(0).Nodes(OldName)
            If oNode IsNot Nothing Then
                oNode.Name = NewName
                oNode.Text = NewName
                PopulateEvents(Common.CurrentScreen)
            End If
        End If
        oNode = _SolutionExplorer.tvSolution.Nodes(0).Nodes("SCREENS").Nodes(OldName)
        If oNode IsNot Nothing Then
            oNode.Name = NewName
            oNode.Text = NewName
        End If
        _CurrentHost.Name = NewName
        _CurrentHost.Text = NewName & "*"
        Common.aScreens.Rename(OldName, NewName)
        For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
            If ScreenCheckEvents(oScreenAttr.Screen, OldName, NewName) Then
                Dim oAlreadyOpenedHc As HostControl = FindScreen(oScreenAttr.Screen.Name)
                If oAlreadyOpenedHc Is Nothing Then ' TODO: Chiudere e riaprire?
                    oAlreadyOpenedHc = OpenScreen(oScreenAttr.FileName, True)
                    ScreenCheckEvents(oAlreadyOpenedHc.Screen, OldName, NewName)
                End If
                ScreenChanged(oScreenAttr.Name)
            End If
        Next

        Dim strAllEvents As String = AllEventsToXml(Common.CurrentScreen)
        Dim strNewAllEvents As String = strAllEvents.Replace("ID_" & OldName & "_", "ID_" & NewName & "_")
        If strAllEvents <> strNewAllEvents Then
            AllEventsFromXML(strNewAllEvents)
        End If

        _SolutionExplorer.lstThumbnails.Items(OldName).Remove()
        _SolutionExplorer.lstThumbnails.Items.Add(NewName, NewName, NewName)
        tmrCheckMiniature.Enabled = True
    End Sub

    Public Function ScreenCheckEvents(ByVal oScreenCheck As VGDD.VGDDScreen, ByVal OldName As String, ByVal NewName As String) As Boolean
        ScreenCheckEvents = False
        For Each oWidget As Object In oScreenCheck.Controls
            If TypeOf (oWidget) Is Common.IVGDDEvents Then
                Dim oEvents As Common.IVGDDEvents = oWidget
                If oEvents.VGDDEvents IsNot Nothing Then
                    For Each oEvent As VGDDEvent In oEvents.VGDDEvents
                        For Each strState As String In New String() {"CREATE", "UPDATE", "DISPLAY", "EDITED"}
                            Dim strOldState As String = strState & "_" & OldName.ToUpper
                            If oEvent.Code.Contains(strOldState) Then
                                Dim strNewState As String = strState & "_" & NewName.ToUpper
                                oEvent.Code = oEvent.Code.Replace(strOldState, strNewState)
                                ScreenCheckEvents = True
                            End If
                        Next
                    Next
                End If
            End If
        Next
    End Function

    Private Sub _WidgetPropertyGrid_PropertyValueChanged(ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles _WidgetPropertyGrid.PropertyValueChanged
        Try
            If e IsNot Nothing Then
                If (e.ChangedItem.ToString.Contains("Width") Or e.ChangedItem.ToString.Contains("Height")) Then
                    If TypeOf _WidgetPropertyGrid.PropertyGrid.SelectedObject Is VGDD.VGDDScreen Then
                        Dim oScreen As VGDD.VGDDScreen = _WidgetPropertyGrid.PropertyGrid.SelectedObject
                        If oScreen IsNot Nothing Then
                            Dim intOldWidth As Integer = oScreen.Width
                            Dim intOldHeight As Integer = oScreen.Height
                            If e.ChangedItem.ToString.Contains("Width") Then
                                intOldWidth = e.OldValue
                            Else
                                intOldHeight = e.OldValue
                            End If
                            sngMigrateRatioWidth = oScreen.Width / intOldWidth
                            sngMigrateRatioHeight = oScreen.Height / intOldHeight
                            If MessageBox.Show(String.Format("Migrate screen from {0}x{1} to {2}x{3}?", intOldWidth, intOldHeight, oScreen.Width, oScreen.Height), "Migrate screen size", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                                MigrateScreen(oScreen, sngMigrateRatioWidth, sngMigrateRatioHeight)
                            End If
                            oScreen.Invalidate()
                        End If
                    End If
                ElseIf e.ChangedItem.Label = "(Name)" Or e.ChangedItem.Label = "Name" Then
                    If _WidgetPropertyGrid.PropertyGrid.SelectedObject.GetType Is GetType(VGDD.VGDDScreen) Then ' _SelectedControls Is Nothing OrElse _SelectedControls.Count = 0 Then 'Screen!
                        'If strCleanName <> e.ChangedItem.Value Then
                        '    MessageBox.Show("This name will cause compilation errors. Please correct it and use only alphabetic characters, digits and _ (underscore)." & vbCrLf & "Example: " & strCleanName, "Warning - Invalid Screen name")
                        '    _WidgetPropertyGrid.PropertyGrid.ResetSelectedProperty()
                        'Else
                        '    RenameScreen(e.OldValue, e.ChangedItem.Value)
                        'End If
                    Else
                        Dim strCleanName As String = Common.CleanName(e.ChangedItem.Value)
                        If strCleanName <> e.ChangedItem.Value Then
                            MessageBox.Show("This name will cause compilation errors. Please correct it and use only alphabetic characters, digits and _ (underscore)." & vbCrLf & "Example: " & strCleanName, "Warning - Invalid Widget name")
                            _WidgetPropertyGrid.PropertyGrid.ResetSelectedProperty()
                        End If

                        Dim strAllEvents As String = AllEventsToXml(_WidgetPropertyGrid.PropertyGrid.SelectedObject)
                        Dim strNewAllEvents As String = strAllEvents.Replace("ID_" & Common.CurrentScreen.Name & "_" & e.OldValue, "ID_" & Common.CurrentScreen.Name & "_" & e.ChangedItem.Value)
                        If strAllEvents <> strNewAllEvents Then
                            AllEventsFromXML(strNewAllEvents)
                        End If

                        Me.PopulateEvents(Common.CurrentScreen)
                    End If
                ElseIf e.ChangedItem.Label = "Group" Then
                    _SolutionExplorer.ChangeGroup(Common.CurrentScreen.Name, e.ChangedItem.Value)
                ElseIf e.ChangedItem.Label = "MasterScreens" Or e.ChangedItem.Label = "ShowMasterScreens" Then
                    Common.CurrentScreen.Invalidate()
                ElseIf e.ChangedItem.Label = "TransparentColour" Then
                    For Each obj As Object In Common.CurrentScreen.Controls
                        If obj.GetType Is GetType(VGDDMicrochip.Picture) Then
                            Dim oPicture As VGDDMicrochip.Picture = obj
                            oPicture.ParentTransparentColour = Common.CurrentScreen.TransparentColour
                        ElseIf obj.GetType Is GetType(VGDDMicrochip.PutImage) Then
                            Dim oPutiImage As VGDDMicrochip.PutImage = obj
                            oPutiImage.ParentTransparentColour = Common.CurrentScreen.TransparentColour
                        End If
                    Next
                    Common.CurrentScreen.Invalidate()
                End If
            ElseIf e.ChangedItem.Label = "ButtonTexts" Then
                CType(_WidgetPropertyGrid.PropertyGrid.SelectedObject, VGDDMicrochip.VGDDWidget).Invalidate()
            End If
        Catch ex As Exception
        End Try
        ScreenChanged()
    End Sub

    Private Sub tmrCheckMiniature_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrCheckMiniature.Tick
        tmrCheckMiniature.Enabled = False
        CheckMiniature(Common.CurrentScreen)
        If _SchemesChooser.cmbScheme.Items.Count <> Common._Schemes.Count Then
            RefreshCmbScheme()
        End If
    End Sub

    Private Sub CheckMiniature(ByVal oScreen As VGDD.VGDDScreen)
        If oScreen Is Nothing Then Exit Sub
        Try
            Using oScreenshot As Bitmap = Common.Render2Bitmap(oScreen, oScreen.Width, oScreen.Height)
                Dim oLstImgMiniature As ImageList = _SolutionExplorer.lstThumbnails.LargeImageList
                If oLstImgMiniature IsNot Nothing Then
                    Using oMiniature As New Bitmap(oLstImgMiniature.ImageSize.Width + 2, oLstImgMiniature.ImageSize.Height + 2)
                        Using g As Graphics = Graphics.FromImage(oMiniature)
                            g.DrawRectangle(Pens.Black, 0, 0, oMiniature.Width - 1, oMiniature.Height - 1)
                            g.DrawImage(oScreenshot, 1, 1, oMiniature.Width - 2, oMiniature.Height - 2)
                            If oLstImgMiniature.Images.ContainsKey(oScreen.Name) Then
                                Try
                                    oLstImgMiniature.Images(oScreen.Name).Dispose()
                                    oLstImgMiniature.Images.RemoveByKey(oScreen.Name)
                                Catch ex As Exception
                                End Try
                            End If
                            oLstImgMiniature.Images.Add(oScreen.Name, oMiniature.Clone)
                        End Using
                    End Using
                End If
            End Using
        Catch ex As Exception
            Debug.Print("!")
        End Try
    End Sub

    Private Sub _CurrentHost_ControlChanged(ByVal sender As Object, ByVal e As System.ComponentModel.Design.ComponentChangedEventArgs)
        If e.Member IsNot Nothing Then
            If e.Member.Name <> "Locked" AndAlso Not tmrUpdateUndo.Enabled Then
                ScreenChanged()
                '_WidgetPropertyGrid.PropertyGrid.SelectedObject = e.Component
            End If
            If e.Member.Name = "Name" Or e.Member.Name = "(Name)" Then
                Dim strCleanName As String = Common.CleanName(e.NewValue)
                If e.NewValue <> strCleanName Then
                    Dim PropDesColl As PropertyDescriptorCollection = TypeDescriptor.GetProperties(e.Component)
                    If PropDesColl IsNot Nothing Then
                        Dim PropDes As PropertyDescriptor = PropDesColl("Name")
                        If PropDes IsNot Nothing Then
                            PropDes.SetValue(e.Component, strCleanName)
                        End If
                    End If
                End If
                If TypeOf (e.Component) Is Common.IVGDDWidget Then
                    If CType(e.Component, Common.IVGDDWidget).Text = e.OldValue Then
                        CType(e.Component, Common.IVGDDWidget).Text = strCleanName
                    End If
                ElseIf TypeOf (e.Component) Is VGDD.VGDDScreen Then
                    Dim oScreen As VGDD.VGDDScreen = e.Component
                    Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(e.OldValue)
                    If oScreen.FileName = Nothing Then 'new screen not yet saved
                        '    '_CurrentHost.Site.Name = strCleanName
                        '_CurrentHost.Name = strCleanName
                        _CurrentHost.Text = _CurrentHost.Text.Replace(e.OldValue, strCleanName)
                        oScreen.Name = strCleanName
                        oScreenAttr.Name = strCleanName
                        oScreenAttr.FileName = strCleanName & ".vds"
                        '    '_CurrentHost.HostSurface.Loader.FileName = strCleanName
                    ElseIf MessageBox.Show("Do you want to rename screen's filename too?", "Rename screen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                        Dim strOldFilename As String = _CurrentHost.Name
                        Dim strNewName As String = Path.Combine(Path.GetDirectoryName(strOldFilename), strCleanName & ".vds")
                        _CurrentHost.HostSurface.Loader.FileName = strNewName
                        _CurrentHost.HostSurface.Loader.Save(strNewName, False, False)
                        oScreenAttr.Screen = Nothing
                        _CurrentHost.Text = _CurrentHost.Text.Replace("*", "")
                        CloseScreen(oScreen.Name, True)
                        RemoveScreenFromProject(e.OldValue)
                        Application.DoEvents()
                        Try
                            File.Delete(strOldFilename)
                        Catch ex As Exception
                        End Try
                        Application.DoEvents()

                        Common.aScreens.Add(strNewName, True)
                        _CurrentHost = OpenScreen(strNewName, True)
                        Application.DoEvents()
                        SaveScreenOnTab(_CurrentHost)
                        Application.DoEvents()
                        _CurrentHost.Text = _CurrentHost.Text.Replace("*", "")
                        oScreenAttr = Common.aScreens(strCleanName)
                        oScreenAttr.Shown = True
                    End If
                    'End If
                End If
            End If
            tmrCheckMiniature.Enabled = True
        End If
    End Sub
#End Region

    Private WithEvents tmrControlAdded As New Timer
    Private aControlsAdded As New Collection

    Private WithEvents tmrHighLightPropertyGridItem As New Timer
    Private HighLightPropertyGridItemCount As Integer
    Private PropertyGridItemNameToHighLight As String

    Private Sub tmrHighLightPropertyGridItem_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrHighLightPropertyGridItem.Tick
        Try
            If HighLightPropertyGridItemCount = 11 OrElse _WidgetPropertyGrid.PropertyGrid.SelectedGridItem.Value <> "" Then
                tmrHighLightPropertyGridItem.Enabled = False
                Exit Sub
            End If
            HighLightPropertyGridItemCount += 1
            _WidgetPropertyGrid.PropertyGrid.Focus()
            If HighLightPropertyGridItemCount / 2 = CInt(HighLightPropertyGridItemCount / 2) Then
                SendKeys.Send("+{tab}")
                tmrHighLightPropertyGridItem.Interval = 50
            Else
                SendKeys.Send("{tab}")
                tmrHighLightPropertyGridItem.Interval = 20
            End If

        Catch ex As Exception
            tmrHighLightPropertyGridItem.Enabled = False
        End Try
    End Sub

    Public Sub HighLightPropertyGridItem(ByVal ItemName As String)
        _WidgetPropertyGrid.PropertyGrid.Focus()
        PropertyGridItemNameToHighLight = ItemName
        '_ControlPropertyGrid.SelectedGridItem = _WidgetPropertyGrid._ControlPropertyGrid.GridItems("Bitmap")
        HighLightPropertyGridItemCount = 0
        tmrHighLightPropertyGridItem.Interval = 500
        tmrHighLightPropertyGridItem.Enabled = True
    End Sub

    Private Sub _CurrentHost_ControlAdded(ByVal sender As Object, ByVal e As System.ComponentModel.Design.ComponentEventArgs)
        If TypeOf (e.Component) Is VGDD.VGDDScreen Then
            PopulateEvents(e.Component)
        Else
            If TypeOf (e.Component) Is VGDDMicrochip.Picture Then
                Dim oPicture As VGDDMicrochip.Picture = e.Component
                If oPicture.Bitmap = "" Then
                    HighLightPropertyGridItem("Bitmap")
                End If
            ElseIf TypeOf (e.Component) Is VGDDMicrochip.PutImage Then
                Dim oPutImage As VGDDMicrochip.PutImage = e.Component
                If oPutImage.Bitmap = "" Then
                    HighLightPropertyGridItem("Bitmap")
                End If
            ElseIf TypeOf (e.Component) Is VGDDMicrochip.VGDDBase Then
                Dim oVgddBase As VGDDMicrochip.VGDDBase = e.Component

            End If

            Dim oControl As Control = e.Component
            If Not aControlsAdded.Contains(oControl.Name) Then
                ScreenChanged()
                tmrControlAdded.Interval = 500
                tmrControlAdded.Enabled = True
                aControlsAdded.Add(oControl, oControl.Name)
                'PopulateEvents(Common.CurrentScreen)
            End If
        End If
        tmrCheckMiniature.Enabled = True
    End Sub

    Private Sub _CurrentHost_ControlRemoved(ByVal sender As Object, ByVal e As System.ComponentModel.Design.ComponentEventArgs)
        If Common.CurrentScreen IsNot Nothing AndAlso Not _EventTree.IsDisposed AndAlso _EventTree.tvEvents.Nodes(0) IsNot Nothing AndAlso _EventTree.tvEvents.Nodes(0).Nodes(Common.CurrentScreen.Name) IsNot Nothing Then
            For Each oNode As TreeNode In _EventTree.tvEvents.Nodes(0).Nodes(Common.CurrentScreen.Name).Nodes
                If oNode.Text = e.Component.Site.Name Then
                    Try
                        oNode.Remove()
                    Catch ex As Exception
                    End Try
                    Exit For
                End If
            Next
        End If
        tmrControlAdded.Interval = 500
        tmrControlAdded.Enabled = True
        tmrCheckMiniature.Enabled = True
    End Sub

    Private Sub tmrControlAdded_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrControlAdded.Tick
        tmrControlAdded.Enabled = False
        Try
            For Each ControlAdded As Control In aControlsAdded
                PopulateControlEvents(ControlAdded, True)
                If TypeOf (ControlAdded) Is VGDDCustom Then
                    Dim strCustType As String = Me._Toolbox.LastToolBoxItemSelected.DisplayName
                    Dim oVgddCustom As VGDDCustom = ControlAdded
                    If Not oVgddCustom.IsDisposed Then
                        oVgddCustom.CustomWidgetType = strCustType
                        Dim oVGDDWidget As Common.IVGDDWidget = ControlAdded
                        oVgddCustom.Name = strCustType & oVGDDWidget.Instances.ToString
                    End If
                End If
            Next

        Catch ex As Exception

        End Try
        aControlsAdded.Clear()
        LastSelection = Nothing
        _CurrentHost_SelectionChanged(Nothing, Nothing)
    End Sub

    Private Sub tmrUpdateEvents_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrUpdateEvents.Tick
        Try
            tmrUpdateEvents.Enabled = False
            Dim oNode As TreeNode
            If LastSelection IsNot Nothing Then
                For Each oControl As Control In LastSelection
                    If TypeOf (oControl) Is VGDD.VGDDScreen Then
                        oNode = _EventTree.tvEvents.Nodes(0).Nodes(oControl.Name)
                    Else
                        oNode = _EventTree.tvEvents.Nodes(0).Nodes(Common.CurrentScreen.Name).Nodes(oControl.Name)
                    End If
                    If oNode IsNot Nothing Then
                        If oNode.IsExpanded Then
                            oNode.Collapse()
                        End If
                        'oNode.NodeFont = Common.oEventsFontNormal ' IIf(oNode.NodeFont.Bold, Common.oEventsFontBold, Common.oEventsFontNormal)
                        oNode.ForeColor = _EventTree.tvEvents.ForeColor
                    End If
                Next
                Application.DoEvents()
            End If

            If _SelectedControls.Count = 1 Then
                If TypeOf (_SelectedControls(0)) Is VGDD.VGDDScreen Then
                    oNode = _EventTree.tvEvents.Nodes(0).Nodes(_SelectedControls(0).Name)
                Else
                    oNode = _EventTree.tvEvents.Nodes(0).Nodes(Common.CurrentScreen.Name).Nodes(_SelectedControls(0).Name)
                End If
                If oNode IsNot Nothing Then
                    oNode.ForeColor = Color.BlueViolet
                    oNode.Text &= "   "
                    oNode.NodeFont = Common.oEventsFontBold
                    oNode.Text = oNode.Text.Trim
                    oNode.Expand()
                    oNode.EnsureVisible()
                    _EventTree.tvEvents.SelectedNode = oNode
                End If
            End If
            LastSelection = CurrentSelection

        Catch ex As Exception

        End Try
    End Sub

    Private Sub _CurrentHost_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs) '(ByRef selection() As Object) 'Handles _CurrentHost.HCSelectionChanged
        tmrUpdateEvents.Enabled = False
        Try
            Dim blnSame As Boolean = True
            If LastSelection IsNot Nothing AndAlso LastSelection.Length = _selectionService.SelectionCount Then
                Dim oNewSelection(_selectionService.SelectionCount) As Object
                _selectionService.GetSelectedComponents().CopyTo(oNewSelection, 0)
                For i As Integer = 0 To _selectionService.SelectionCount - 1
                    If oNewSelection(i) IsNot LastSelection(i) Then
                        blnSame = False
                        Exit For
                    End If
                Next
            Else
                blnSame = False
            End If
            If blnSame Then
                Return
            End If
            If _selectionService Is Nothing Then
                '_WidgetPropertyGrid.PropertyGrid.SelectedObjects = Nothing
            ElseIf _selectionService.SelectionCount = 0 Then
                '_WidgetPropertyGrid.PropertyGrid.SelectedObjects = Nothing
            Else
                ReDim CurrentSelection(_selectionService.SelectionCount - 1)
                _selectionService.GetSelectedComponents().CopyTo(CurrentSelection, 0)
                Try
                    _WidgetPropertyGrid.PropertyGrid.SelectedObjects = CurrentSelection
                Catch ex As Exception
                End Try
            End If

            _SelectedControls = New Collections.ArrayList
            If CurrentSelection Is Nothing Then
                ToolStripDeleteWidget.Enabled = False
                _WidgetInfo.Visible = False
                ToolStripAlign.Enabled = False
                ToolStripSize.Enabled = False
            Else
                ToolStripAlign.Enabled = CurrentSelection.Length > 1
                ToolStripSize.Enabled = CurrentSelection.Length > 1
                ToolStripSameH.Enabled = CurrentSelection.Length > 1
                ToolStripSameSize.Enabled = CurrentSelection.Length > 1
                ToolStripSameW.Enabled = CurrentSelection.Length > 1
                ToolStripEditEnDis(CurrentSelection.Length > 0)
                ToolStripAlignDown.Enabled = CurrentSelection.Length > 1
                ToolStripAlignLeft.Enabled = CurrentSelection.Length > 1
                ToolStripAlignRight.Enabled = CurrentSelection.Length > 1
                ToolStripAlignUp.Enabled = CurrentSelection.Length > 1
                ToolStripCenterHoriz.Enabled = CurrentSelection.Length > 1
                ToolStripCenterVert.Enabled = CurrentSelection.Length > 1
                ToolStripBringToFront.Enabled = CurrentSelection.Length > 0
                ToolStripSendToBack.Enabled = CurrentSelection.Length > 0
                ToolStripDeleteWidget.Enabled = True
                For Each obj As Object In CurrentSelection
                    If TypeOf (obj) Is VGDD.VGDDScreen Then
                        ToolStripDeleteWidget.Enabled = False
                        _WidgetPropertyGrid.PropertyGrid.SelectedObjects = CurrentSelection
                        If Common.ProjectUsePalette AndAlso Common.CurrentScreen.PaletteName <> String.Empty Then
                            For Each oScheme As VGDDScheme In Common._Schemes.Values
                                If oScheme.Palette IsNot Nothing AndAlso oScheme.Palette.Name = Common.CurrentScreen.PaletteName Then
                                    Common.SelectedScheme = oScheme
                                    _SchemesChooser.cmbScheme.SelectedItem = oScheme
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                    _SelectedControls.Add(obj)
                    If TypeOf obj Is Common.IVGDDWidget Then
                        Dim strSchemeName As String = CType(obj, Common.IVGDDWidget).Scheme
                        If strSchemeName Is Nothing OrElse strSchemeName = String.Empty Then
                            If Common.SelectedScheme IsNot Nothing Then
                                CType(obj, Common.IVGDDWidget).Scheme = Common.SelectedScheme.Name
                            End If
                        Else
                            _SchemesChooser.SelectedSchemeName = strSchemeName
                        End If
                    End If
                Next

                cmuWidget.Items.Clear()
                If _SelectedControls.Count = 1 Then
                    If TypeOf _SelectedControls(0) Is VGDDMicrochip.Picture Then
                        cmuWidget.Items.Add("Resize to fit")
                    End If
                    If TypeOf _SelectedControls(0) Is Common.IVGDDEvents AndAlso Common.CurrentScreen IsNot Nothing Then
                        _EventsEditor.ParentScreenName = Common.CurrentScreen.Name
                        _EventsEditor.SetEditedControl(_SelectedControls(0))
                        _EventsEditor.DialogResult = DialogResult.Retry
                    End If
                End If

                If Not Common.ProjectLoading AndAlso Common.CurrentScreen IsNot Nothing AndAlso _EventTree.tvEvents.Nodes.Count > 0 _
                                             AndAlso _EventTree.tvEvents.Nodes(0).Nodes(Common.CurrentScreen.Name) IsNot Nothing Then
                    With tmrUpdateEvents
                        .Interval = 10
                        .Enabled = True
                    End With
                End If
                If CurrentSelection.Length = 1 Then
                    _WidgetInfo.Enabled = True
                    Dim SelObj As Object = CurrentSelection(0)
                    Dim strSelObjType As String = SelObj.GetType.ToString.Split(".")(1)
                    If strSelObjType = "VGDDCustom" Then
                        strSelObjType = CType(SelObj, VGDDCustom).CustomWidgetType
                    End If
                    _WidgetInfo.lblWidgetHelp.Text = String.Format("Online Help on {0}", strSelObjType)
                    Dim oStream As System.IO.Stream
                    oStream = Assembly.GetExecutingAssembly.GetManifestResourceStream(String.Format("{0}_Info.png", strSelObjType))
                    If oStream Is Nothing Then
                        oStream = SelObj.GetType.Assembly.GetManifestResourceStream(String.Format("{0}_Info.png", strSelObjType))
                    End If
                    If oStream IsNot Nothing Then
                        Dim ImgHelp As New Bitmap(oStream)
                        _WidgetInfo.pictSchemeHelp.Image = ImgHelp
                    Else
                        _WidgetInfo.pictSchemeHelp.Image = Nothing
                    End If
                    Dim SelType As Type = SelObj.GetType
                    If SelType Is Nothing Then
                        _WidgetInfo.Visible = False
                    Else
                        _WidgetInfo.Visible = True
                        If TypeOf SelObj Is Common.IVGDDWidget Then
                            _WidgetInfo.pnlFootPrints.Visible = True
                            Try
                                Dim strWidgetType As String = SelType.ToString.Split(".")(1)
                                _WidgetInfo.lblROM.Text = CodeGen.FootPrintValue(strWidgetType, "ROM" & Common.ProjectPicFamily)
                                _WidgetInfo.lblRAM.Text = CodeGen.FootPrintValue(strWidgetType, "RAM" & Common.ProjectPicFamily)
                                _WidgetInfo.lblHEAP.Text = CodeGen.FootPrintValue(strWidgetType, "HEAP" & Common.ProjectPicFamily)
                                Dim oFootPrint As Common.IVGDDWidget = SelObj
                                _WidgetInfo.lblInstances.Text = oFootPrint.Instances
                                _WidgetInfo.lblInstances.Visible = True
                            Catch ex As Exception
                            End Try
                        Else
                            _WidgetInfo.pnlFootPrints.Visible = False
                            _WidgetInfo.lblInstances.Visible = False
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TvSelectScreen()
        Dim oNode As TreeNode, oNodeCurrent As TreeNode = Nothing
        Try
            If Not Common.ProjectLoading AndAlso _EventTree.tvEvents.Nodes.Count > 0 AndAlso Common.CurrentScreen IsNot Nothing Then
                _EventTree.tvEvents.BeginUpdate()
                For Each oNode In _EventTree.tvEvents.Nodes(0).Nodes 'Screens
                    If oNode.Text = Common.CurrentScreen.Name Then
                        oNodeCurrent = oNode
                        oNode.Expand()
                        oNode.ForeColor = Color.DarkGreen
                        oNode.NodeFont = Common.oEventsFontBold
                    Else
                        If oNode.IsExpanded Then
                            oNode.Collapse()
                        End If
                        If oNode.NodeFont IsNot Nothing AndAlso oNode.NodeFont.Style = FontStyle.Bold Then
                            oNode.NodeFont = Common.oEventsFontNormal
                        End If
                        oNode.ForeColor = _EventTree.tvEvents.ForeColor
                    End If
                Next
                If oNodeCurrent IsNot Nothing Then oNodeCurrent.EnsureVisible()
            End If
        Catch ex As Exception
        End Try
        _EventTree.tvEvents.EndUpdate()
    End Sub

    Private Sub GenerateCodeMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuGenerateCode.Click
        If CheckModifiedProject() = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        If Me._SolutionExplorer.tvSolution.Nodes(0).Nodes("SCREENS") Is Nothing Then Exit Sub
        oShowSource = New frmGenCode
        With oShowSource
            .SourceEditor.Visible = False
            .pnlProgressBar.Visible = False
            .Height = .btnGenerateCode.Top + .btnGenerateCode.Height + 60 ' 160 '.pnlGenerateOptions.Height
            .ProgressBar1.Value = 0
            .ProgressBar1.Maximum = Common._Bitmaps.Count + Common._Fonts.Count + Common._Schemes.Count + Common.aScreens.Count * 2 + 4
            .lblBinBitmaps.Visible = False
            .btnBinBitmaps.Visible = False
            Common.CodegenProgress = 0
            Application.DoEvents()
            .ShowDialog()
        End With
    End Sub

    Public Sub GenerateCode()
        Dim ScreenFileName As String
        Dim blnMasterScreensUsed As Boolean = False
        Dim hc As HostControl = Nothing
        Dim aMissingFiles As New Collection, aMissingFolders As New Collection

#If CONFIG = "Debug" Then
        CodeGen.LoadCodeGenTemplates()
#End If

        Common.CodegenGeneratingCode = True
        Me.Cursor = Cursors.WaitCursor
        With oShowSource
            .pnlProgress.Visible = True
            '.ProgressBar1.ForeColor = Color.Blue
            .btnGenerateCode.Enabled = False
            .btnClose.Enabled = False
            .Timer1.Interval = 500
            .Timer1.Enabled = True
            FootPrint.Clear()
        End With
        Application.DoEvents()

        If Common.BitmapsBinPath Is Nothing Then Common.BitmapsBinPath = Common.VGDDProjectPath

        MplabX.LoadMplabxProject()

        CodeGen.Clear(Common.ProjectName)
        _OutputWindow.RichTextBox.Text = String.Empty

        Dim strEventsHelper As String = "// " & Path.GetFileName(Common.ProjectFileName_EventsHelperH) & vbCrLf & _
                        "// Put here all references needed for " & Path.GetFileName(Common.ProjectFileName_ScreensC) & " to compile correctly" & vbCrLf

        Select Case Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                strEventsHelper &= "#include ""vgdd_main.h"""
                CreateEmptyFileIfNotExists(Common.ProjectFileName_EventsHelperH, strEventsHelper)
                CodeGen.CodeEventsHelper = ReadFile(Common.ProjectFileName_EventsHelperH)
                MplabX.FileLoad("GraphicsConfig.h")
                MplabX.FileDisableDefine("GraphicsConfig.h", "USE_TRANSPARENT_COLOR", False)
            Case "MLA"
                strEventsHelper &= "#include ""vgdd_main.h"""
                CreateEmptyFileIfNotExists(Common.ProjectFileName_EventsHelperH, strEventsHelper)
                MplabX.FileLoad("gfx_config.h")
                MplabX.FileLoad("system_config.h")
                MplabX.FileLoad("system.c")
                MplabX.FileLoad("system.h")
                MplabX.GraphicsConfigEnableDefine("GFX_CONFIG_TRANSPARENT_COLOR_DISABLE")
                CodeGen.CodeEventsHelper = ReadFile(Common.ProjectFileName_EventsHelperH)
            Case "HARMONY"
                strEventsHelper &= "#include ""vgdd_main.h"""
                CreateEmptyFileIfNotExists(Common.ProjectFileName_EventsHelperH, strEventsHelper)
                CodeGen.CodeEventsHelper = ReadFile(Common.ProjectFileName_EventsHelperH)
                MplabX.FileLoad("gfx_config.h")
                MplabX.FileLoad("system_config.h")
                MplabX.FileLoad("system_definitions.h")
                MplabX.FileLoad("system.c")
                MplabX.GraphicsConfigEnableDefine("GFX_CONFIG_TRANSPARENT_COLOR_DISABLE")
        End Select
        MplabX.FileLoad("vgdd_main.h")

        Dim ScreenMaxHeap As Integer = 0
        Common.ProjectUseGol = False
        For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
            Common.CurrentScreen = oScreenAttr.Screen
            If Common.ProjectUsePalette AndAlso Common.CurrentScreen.PaletteName <> String.Empty Then
                For Each oScheme As VGDDScheme In Common._Schemes.Values
                    If oScheme.Palette IsNot Nothing AndAlso oScheme.Palette.Name = Common.CurrentScreen.PaletteName Then
                        Common.SelectedScheme = oScheme
                        Exit For
                    End If
                Next
            End If
            If oScreenAttr.Screen Is Nothing Then Continue For
            Common.CodegenProgress += 1
            Application.DoEvents()
            ScreenFileName = oScreenAttr.FileName
            Dim oScreen As VGDD.VGDDScreen = oScreenAttr.Screen
            If oScreen.IsMasterScreen Then
                blnMasterScreensUsed = True
            End If

            oShowSource.lblStatus.Text = oScreenAttr.Name
            Application.DoEvents()

            CodeGen.ScreenToCode(oScreenAttr)

            Dim ScreenTotalHeap As Integer = 0
            For Each intHeap As Integer In FootPrint.Widgets.Values
                ScreenTotalHeap += intHeap
            Next
            If ScreenMaxHeap < ScreenTotalHeap Then
                ScreenMaxHeap = ScreenTotalHeap
            End If
            FootPrint.Widgets.Clear()

            If oScreenAttr.Screen IsNot Nothing AndAlso oScreenAttr.Screen.TransparentColour <> Color.Transparent AndAlso oScreenAttr.Screen.TransparentColour.Name <> "0" Then
                Select Case Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        MplabX.FileLoad("HardwareProfile.h")
                        If MplabX.MplabxFiles.Count > 0 AndAlso MplabX.MplabxFiles.ContainsKey("HardwareProfile.h") Then
                            If MplabX.MplabxFiles.Item("HardwareProfile.h").Contains("#define GFX_USE_DISPLAY_CONTROLLER_S1D13517") Then
                                MplabX.FileSetValueOfDefine("GraphicsConfig.h", "#define TRANSPARENTCOLOR", CodeGen.Color2Num(oScreenAttr.Screen.TransparentColour), "S1D13517 Application-wide Transparent Color as set on Screen " & oScreenAttr.Name)
                            Else
                                MplabX.GraphicsConfigEnableDefine("USE_TRANSPARENT_COLOR")
                            End If
                        End If
                    Case "MLA"
                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_TRANSPARENT_COLOR_DISABLE")
                    Case "HARMONY"
                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_TRANSPARENT_COLOR_DISABLE")
                        'TODO: Transparency per S1D13517
                End Select
            End If
        Next

        Dim strGolFree As String = String.Empty
        If Common.ProjectUseGol Then
            strGolFree = CodeGen.XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/ProjectTemplates/GOLFree").InnerText
        End If
        CodeGen.Code = CodeGen.RemoveEmptyLines(CodeGen.Code.Replace("[GOLFREE]", strGolFree))

        oShowSource.lblStatus.Text = "Runnning GRC..."
        Try
            If Not CodeGen.ResourcesToCode() Then
                Common.ResourcesToConvert = True
                Throw New Exception("Cannot convert graphics resources")
            End If
            Common.ResourcesToConvert = False
            For Each oBitmap As VGDDImage In Common._Bitmaps
                If oBitmap.Type = VGDDImage.PictureType.FLASH_VGDD Then
                    FootPrint.BitmapsFlash += oBitmap.BitmapCompressedSize
                End If
            Next

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            With oShowSource
                .Timer1.Enabled = False
                .ProgressBar1.Value = .ProgressBar1.Maximum
                .ProgressBar1.ForeColor = Color.Red
                .ProgressBar1.TextFormat = ""
                .btnShowSource.Enabled = False
                .btnClose.Enabled = True
                .lblStatus.Text = "Failed"
            End With
            MessageBox.Show(ex.Message, "GenerateCode.ResourcesToCode Error")
            Exit Sub
        End Try


        If Common.ProjectMultiLanguageTranslations > 0 Then
            Dim strIncludeStringsPool As String = "#include """ & Path.GetFileName(Common.ProjectFileName_StringsPoolH) & """"
            Dim strPath As String = String.Empty
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    strPath = ""
                Case "MLA"
                    strPath = "appMLA/system_config/{0}/vgdd"
                Case "HARMONY"
                    strPath = "app/system_config/{0}/vgdd"
            End Select
            aMissingFiles.Add(Common.ProjectFileName_StringsPoolC)
            aMissingFolders.Add(String.Format("Source Files/" & strPath, Common.MplabXSelectedConfig))
            If Common.ProjectStringsPoolGenerateHeader Then
                aMissingFiles.Add(Common.ProjectFileName_StringsPoolH)
                aMissingFolders.Add(String.Format("Header Files/" & strPath, Common.MplabXSelectedConfig))
            End If
            'End If
            Common.StringsPoolMaxStringID = 0
            Dim oStringSet As VGDDCommon.MultiLanguageStringSet
            For Each oStringSet In Common.ProjectStringPool.Values
                If Common.StringsPoolMaxStringID < oStringSet.StringID Then Common.StringsPoolMaxStringID = oStringSet.StringID
            Next

            CodeGen.Headers &= CodeGen.GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/Header", 0, 0) _
                            .Replace("[NUMSTRINGS]", Common.ProjectStringPool.Values.Count) _
                            .Replace("[NUMTRANSLATIONS]", Common.ProjectMultiLanguageTranslations + 1)
            Dim strCodeStringsPool As String = CodeGen.ReplaceProjectStrings( _
                CodeGen.XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/StringsPoolTemplate/Code").InnerText)

            Dim StringsPool As String = String.Empty
            Dim StringsPoolHeader As String = String.Empty
            Dim oNodeStringsPoolHeaderDefine As XmlNode = CodeGen.XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/StringsPoolTemplate/StringsPoolHeaderDefine")
            Dim strStringsPoolHeaderDefine As String = String.Empty
            If oNodeStringsPoolHeaderDefine IsNot Nothing Then
                strStringsPoolHeaderDefine = oNodeStringsPoolHeaderDefine.InnerText.Replace(vbCrLf, "").Trim()
            End If
            For j As Integer = 1 To Common.StringsPoolMaxStringID
                oStringSet = Nothing
                If Common.ProjectStringPool.ContainsKey(j) Then
                    oStringSet = Common.ProjectStringPool(j)
                Else
                    CodeGen.Errors &= String.Format("String id {0} is undefined. Please consider renumbering your string IDs", j) & vbCrLf
                    oStringSet = New VGDDCommon.MultiLanguageStringSet
                    oStringSet.InUse = False
                    Dim aStrings(Common.ProjectMultiLanguageTranslations) As String
                    For k As Integer = 0 To Common.ProjectMultiLanguageTranslations
                        aStrings(k) = String.Empty
                    Next
                    oStringSet.Strings = aStrings
                    oStringSet.FontName = CType(Common._Fonts(0), VGDDFont).Name
                End If
                Dim strTranslations As String = String.Empty
                For i As Integer = 0 To Common.ProjectMultiLanguageTranslations
                    Dim strVisibleText As String = String.Empty
                    If oStringSet.Strings(i) Is Nothing Then oStringSet.Strings(i) = String.Empty
                    Dim strArrayText As String = String.Empty
                    'strArrayText = oStringSet.Strings(i).Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, "\n")
                    Dim strComma As String = IIf(i < Common.ProjectMultiLanguageTranslations, ",", " ")
                    If oStringSet.Strings(i) Is Nothing Then oStringSet.Strings(i) = String.Empty
                    Dim oFont As VGDDFont = Common.GetFont(oStringSet.FontName, Nothing)
                    If oFont Is Nothing Then
                        CodeGen.Errors &= String.Format("String id {0} has unknown font {1}!", j, oStringSet.FontName) & vbCrLf
                    Else
                        oFont.SmartCharSetAddString(oStringSet.Strings(i))
                        strArrayText = CodeGen.QText(oStringSet.Strings(i), oFont, strVisibleText, oStringSet.Dynamic)
                        strTranslations = CodeGen.QtextBinary(strArrayText, strTranslations, strVisibleText, i, oStringSet.Strings(i).Length, strComma)
                    End If
                    'End If
                Next
                StringsPool &= CodeGen.GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/StringsTemplate", 0, 0) _
                            .Replace(vbCrLf, "") _
                            .Replace("[STRINGID]", j - 1) _
                            .Replace("[INUSE]", IIf(oStringSet.InUse, "", " (Not In Use)")) _
                            .Replace("[QTEXT]", oStringSet.Strings(0).Replace(vbCrLf, "\n")) _
                            .Replace("[TRANSLATIONS]", strTranslations) _
                            .Replace("[COMMA]", IIf(j < Common.StringsPoolMaxStringID, ",", "")) _
                            .Replace("[NEWLINE]", vbCrLf) _
                            .Replace("[TAB]", "    ") '_
                '& "    } " & vbCrLf
                If Common.ProjectStringsPoolGenerateHeader Then
                    Dim strStringHeader As String
                    If oStringSet.StringAltID <> String.Empty Then
                        strStringHeader = UCase(oStringSet.StringAltID)
                    Else
                        strStringHeader = UCase(oStringSet.Strings(0))
                        If strStringHeader = String.Empty Then
                            strStringHeader = "EMPTY"
                        End If
                        If strStringHeader.Contains(vbCr) Then
                            strStringHeader = strStringHeader.Split(vbCr)(0)
                        End If
                        strStringHeader = Common.CleanName(strStringHeader)
                    End If
                    strStringHeader = strStringsPoolHeaderDefine.Replace("[REFSTRING]", strStringHeader)
                    Dim strStringHeaderDefine As String = String.Format("#define {0} VGDDString({1})", strStringHeader, j - 1)
                    If Not StringsPoolHeader.Contains(String.Format("#define {0} ", strStringHeader)) Then
                        StringsPoolHeader &= strStringHeaderDefine & vbCrLf
                        CodeGen.Headers = CodeGen.Headers.Replace(String.Format("VGDDString({0})", j - 1), strStringHeader)
                    End If
                End If
            Next
            WriteFile(Common.ProjectFileName_StringsPoolC, _
                strCodeStringsPool.Replace("[STRINGSPOOL]", StringsPool), Encoding.UTF8)
            If Common.ProjectStringsPoolGenerateHeader Then
                WriteFile(Common.ProjectFileName_StringsPoolH, _
                CodeGen.XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/StringsPoolTemplate/StringsPoolHeader").InnerText _
                .Replace("[STRINGSPOOLDEFINES]", StringsPoolHeader).Replace("[NEWLINE]", vbCrLf), Encoding.UTF8)
            End If
        End If

        oShowSource.lblStatus.Text = "Checking MPLAB X Project..."
        Common.CodegenProgress += 1
        CodeGen.ToCodeClose()

        Dim strIncludeResources As String = "#include """ & Path.GetFileName(Common.ProjectFileName_InternalResourcesH) & """"
        Dim strIncludeExternalResources As String = "#include """ & Path.GetFileName(Common.ProjectFileName_ExternalResourcesH) & """"
        Dim strIncludeBmpOnSdResources As String = "#include """ & Path.GetFileName(Common.ProjectFileName_BmpOnSdResourcesH) & """"
        Select Case Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                Static blnWarnGraphicsConfig As Boolean = False
                If MplabX.MplabxFiles.Count = 0 OrElse MplabX.MplabxFiles.Item("GraphicsConfig.h") Is Nothing Then
                    If Not blnWarnGraphicsConfig Then
                        MessageBoxWithCheckBox.Show("Could not check GraphicsConfig.h because no valid MPLAB X project found.", "Warning", MessageBoxButtons.OK, "Do not show this message again for this session.", blnWarnGraphicsConfig)
                    End If
                Else
                    For Each WidgetNode As XmlNode In CodeGen.XmlTemplatesDoc.SelectNodes("VGDDCodeTemplate/ControlsTemplates/*")
                        Dim GraphicsConfigNode As XmlNode = WidgetNode.SelectSingleNode("GraphicsConfig")
                        If GraphicsConfigNode IsNot Nothing Then
                            For Each strUseFlag As String In GraphicsConfigNode.InnerText.Trim.Split(vbCrLf)
                                strUseFlag = strUseFlag.Trim
                                If strUseFlag <> "" Then
                                    MplabX.FileDisableDefine("GraphicsConfig.h", strUseFlag, False)
                                End If
                            Next
                        End If

                        'TODO: Override unused MAL modules
                        'For Each GraphicsConfigNode In WidgetNode.SelectNodes("Project/Folder/AddFile/*")
                        '    MplabX.MplabXOverrideConfProperty("default","",Replace(GraphicsConfigNode.InnerText.Replace("$MAL",Mal.MalPath)),
                        'Next
                    Next

                    For Each ActiveWidgetType As String In FootPrint.ProjectWidgets.Keys
                        If ActiveWidgetType.Contains(".") Then
                            ActiveWidgetType = ActiveWidgetType.Split(".")(1)
                        End If
                        Dim GraphicsConfigNode As XmlNode = CodeGen.XmlTemplatesDoc.SelectSingleNode( _
                            String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/GraphicsConfig", ActiveWidgetType))
                        If GraphicsConfigNode Is Nothing Then
                            GraphicsConfigNode = VGDDCustom.XmlCustomTemplatesDoc.SelectSingleNode( _
                            String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/GraphicsConfig", ActiveWidgetType))
                        End If
                        If GraphicsConfigNode IsNot Nothing Then
                            For Each strUseFlag As String In GraphicsConfigNode.InnerText.Trim.Split(vbCrLf)
                                strUseFlag = strUseFlag.Trim & " "
                                If strUseFlag <> "" Then
                                    MplabX.GraphicsConfigEnableDefine(strUseFlag)
                                End If
                            Next
                        End If
                    Next

                    If Common.ProjectUsePalette Then
                        MplabX.GraphicsConfigEnableDefine("USE_PALETTE")
                        MplabX.GraphicsConfigEnableDefine("WHITE 255")
                        MplabX.GraphicsConfigEnableDefine("BRIGHTRED 128")
                        MplabX.GraphicsConfigEnableDefine("BLACK 0")
                    End If

                    If Common.ProjectUseGradient Then
                        MplabX.GraphicsConfigEnableDefine("USE_GRADIENT")
                    Else
                        MplabX.FileDisableDefine("GraphicsConfig.h", "USE_GRADIENT", False)
                    End If

                    If Common.ProjectUseMultiByteChars Then
                        MplabX.GraphicsConfigEnableDefine("USE_MULTIBYTECHAR")
                    Else
                        MplabX.FileDisableDefine("GraphicsConfig.h", "USE_MULTIBYTECHAR", False)
                    End If
                    MplabX.FileSetValueOfDefine("GraphicsConfig.h", "#define COLOR_DEPTH", Common.ProjectColourDepth, "Project colour depth set in VGDD project properties")

                    MplabX.FileSave("GraphicsConfig.h")
                End If
                If Common.IPU_MAX_COMPRESSED_BUFFER_SIZE > 0 Then
                    MplabX.FileLoad("HardwareProfile.h")
                    MplabX.FileSetValueOfDefine("HardwareProfile.h", "#define GFX_COMPRESSED_BUFFER_SIZE", Common.IPU_MAX_COMPRESSED_BUFFER_SIZE, Nothing)
                    MplabX.FileSetValueOfDefine("HardwareProfile.h", "#define GFX_DECOMPRESSED_BUFFER_SIZE", Common.IPU_MAX_DECOMPRESSED_BUFFER_SIZE, Nothing)
                    MplabX.FileSave("HardwareProfile.h")
                End If
            Case "MLA", "HARMONY"
                Dim SystemH As String
                If Mal.FrameworkName.ToUpper = "HARMONY" Then
                    SystemH = "gfx_vgdd_definitions.h"
                    MplabX.FileLoad(Common.ProjectFileName_ScreensH)
                    Dim SysTasks As String = Path.Combine(Common.CodeGenDestPath, "system_tasks.c")
                    If MplabX.FileLoad(SysTasks) Then
                        Dim strVgddTasks As String = "GFX_VGDD_Tasks(sysObj.gfxObject0);"
                        If Not MplabX.MplabxFiles(SysTasks).Contains(strVgddTasks) Then
                            MplabX.ReplaceInFile(SysTasks, "GFX_Tasks(sysObj.gfxObject0);", "<pattern>" & vbCrLf & vbCrLf & _
                                                     "    /* Maintain VGDD screen creation and handling */" & vbCrLf & _
                                                     "    " & strVgddTasks)
                        End If
                    End If

                    If MplabX.MplabXProjectSearchItem(Path.GetFileName(SystemH)) Is Nothing Then
                        aMissingFolders.Add(String.Format("Source Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                        aMissingFiles.Add(Common.ProjectFileName_ScreensC)
                        aMissingFolders.Add(String.Format("Header Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                        aMissingFiles.Add(Common.ProjectFileName_ScreensH)
                    End If
                Else
                    SystemH = "system.h"
                    'MplabX.FileLoad(SystemH)
                End If
                If Mal.FrameworkName.ToUpper = "HARMONY" Then
                    If Common.oGrcProjectInternal.ResourcesCount > 0 AndAlso _
                            Not CodeGen.HeadersIncludes.Contains(strIncludeResources) Then
                        CodeGen.HeadersIncludes &= vbCrLf & strIncludeResources
                    End If
                    If Common.oGrcProjectExternal.ResourcesCount > 0 AndAlso _
                        Not CodeGen.HeadersIncludes.Contains(strIncludeExternalResources) Then
                        CodeGen.HeadersIncludes &= vbCrLf & strIncludeExternalResources
                    End If
                    If Common.oGrcBinBmpOnSd.ResourcesCount > 0 AndAlso _
                        Not CodeGen.HeadersIncludes.Contains(strIncludeBmpOnSdResources) Then
                        CodeGen.HeadersIncludes &= vbCrLf & strIncludeBmpOnSdResources
                    End If
                ElseIf MplabX.MplabxFiles.ContainsKey(SystemH) Then 'MLA
                    If Common.oGrcProjectInternal.ResourcesCount > 0 AndAlso _
                            Not MplabX.MplabxFiles(SystemH).Contains(strIncludeResources) Then
                        MplabX.MplabxFiles(SystemH) = CodeGen.AddInclude(MplabX.MplabxFiles(SystemH), strIncludeResources, "#include ""system_definitions.h""")
                    End If
                    If Common.oGrcProjectExternal.ResourcesCount > 0 AndAlso _
                        Not MplabX.MplabxFiles(SystemH).Contains(strIncludeExternalResources) Then
                        MplabX.MplabxFiles(SystemH) = CodeGen.AddInclude(MplabX.MplabxFiles(SystemH), strIncludeExternalResources)
                    End If
                    If Common.oGrcBinBmpOnSd.ResourcesCount > 0 AndAlso _
                        Not MplabX.MplabxFiles(SystemH).Contains(strIncludeBmpOnSdResources) Then
                        MplabX.MplabxFiles(SystemH) = CodeGen.AddInclude(MplabX.MplabxFiles(SystemH), strIncludeBmpOnSdResources)
                    End If
                End If

                If Mal.FrameworkName.ToUpper = "HARMONY" Then
                    If MplabX.FileLoad(Path.GetFileName(Common.ProjectFileName_InternalResourcesC)) Then
                        MplabX.DisableInclude("#include ""system_config.h""", MplabX.MplabxFiles(Path.GetFileName(Common.ProjectFileName_InternalResourcesC)))
                    End If
                    If MplabX.FileLoad(Path.GetFileName(Common.ProjectFileName_InternalResourcesRefC)) Then
                        MplabX.ReplaceInFile(Common.ProjectFileName_InternalResourcesRefC, _
                                             "#if(GRAPHICS_LIBRARY_VERSION != 0x0400)", "#if(GRAPHICS_LIBRARY_VERSION < 0x0400)")
                    End If
                    If MplabX.FileLoad(Path.GetFileName(Common.ProjectFileName_InternalResourcesH)) Then
                        MplabX.ReplaceInFile(Common.ProjectFileName_InternalResourcesH, _
                                             "#if(GRAPHICS_LIBRARY_VERSION != 0x0400)", "#if(GRAPHICS_LIBRARY_VERSION < 0x0400)")
                    End If
                End If

                If Common.ProjectUseMultiByteChars Then
                    MplabX.GraphicsConfigEnableDefine("GFX_CONFIG_FONT_CHAR_SIZE 16")
                Else
                    MplabX.FileDisableDefine("gfx_config.h", "GFX_CONFIG_FONT_CHAR_SIZE", False)
                End If

                If Common.oGrcProjectExternal.ResourcesCount = 0 Then
                    MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_IMAGE_EXTERNAL_DISABLE", "#define _GRAPHICS_CONFIG_H")
                    MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_FONT_EXTERNAL_DISABLE", "#define _GRAPHICS_CONFIG_H")
                End If

                If Common.ProjectUseAlphaBlend Then
                    MplabX.FileDisableDefine("gfx_config.h", "GFX_CONFIG_ALPHABLEND_DISABLE", False)
                Else
                    MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_ALPHABLEND_DISABLE", "#define _GRAPHICS_CONFIG_H")
                End If
                If Mal.FrameworkName.ToUpper <> "HARMONY" Then
                    MplabX.FileSetValueOfDefine("gfx_config.h", "#define GFX_CONFIG_COLOR_DEPTH", Common.ProjectColourDepth, "Project colour depth set in VGDD project properties")
                End If
                MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_PALETTE_DISABLE", "#define _GRAPHICS_CONFIG_H")
                MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_FOCUS_DISABLE", "#define _GRAPHICS_CONFIG_H")
                If Common.ProjectColourDepth < 16 Then
                    MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_ALPHABLEND_DISABLE", "#define _GRAPHICS_CONFIG_H")
                    MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_GRADIENT_DISABLE", "#define _GRAPHICS_CONFIG_H")
                End If

                If Common.ProjectColourDepth = 8 Then
                    MplabX.FileDisableDefine("gfx_config.h", "GFX_CONFIG_PALETTE_DISABLE", False)
                    MplabX.GraphicsConfigEnableDefine("WHITE 255")
                    MplabX.GraphicsConfigEnableDefine("BRIGHTRED 128")
                    MplabX.GraphicsConfigEnableDefine("BLACK 0")
                ElseIf Common.ProjectColourDepth = 4 Then
                    If Common.ProjectUsePalette Then
                        MplabX.FileDisableDefine("gfx_config.h", "GFX_CONFIG_PALETTE_DISABLE", False)
                    End If
                    MplabX.GraphicsConfigEnableDefine("BLACK 0")
                    MplabX.GraphicsConfigEnableDefine("WHITE 15")
                    If Mal.FrameworkName.ToUpper = "HARMONY" Then
                        MplabX.GraphicsConfigEnableDefine("GRAY014 14")
                        MplabX.GraphicsConfigEnableDefine("GRAY012 12")
                        MplabX.GraphicsConfigEnableDefine("GRAY010 10")
                        MplabX.GraphicsConfigEnableDefine("GRAY006 6")
                        MplabX.GraphicsConfigEnableDefine("GRAY004 4")
                    Else 'MLA
                        MplabX.GraphicsConfigEnableDefine("GRAY015 15")
                        MplabX.GraphicsConfigEnableDefine("GRAY000 0")
                    End If
                    MplabX.GraphicsConfigEnableDefine("GRAY008 7")
                ElseIf Common.ProjectColourDepth = 1 Then
                    MplabX.GraphicsConfigEnableDefine("WHITE 1")
                    MplabX.GraphicsConfigEnableDefine("BLACK 0")
                End If

                If Common.ProjectColourDepth < 8 Then
                    MplabX.FileEnableDefine("gfx_config.h", "GFX_CONFIG_FONT_ANTIALIASED_DISABLE", "#define _GRAPHICS_CONFIG_H")
                End If

                Dim strScreenStates As String = String.Format("#include ""{0}""", Path.GetFileName(Common.ProjectFileName_ScreensH))
                If MplabX.MplabxFiles.ContainsKey("vgdd_main.h") AndAlso Not MplabX.MplabxFiles("vgdd_main.h").Contains(strScreenStates) Then
                    MplabX.MplabxFiles("vgdd_main.h") = CodeGen.AddInclude(MplabX.MplabxFiles("vgdd_main.h"), strScreenStates, "SCREEN_STATES;")
                End If
        End Select

        Dim aFilesToSave As New ArrayList
        For Each strFileName As String In MplabX.MplabxFiles.Keys
            If MplabX.MplabxFiles.Item(strFileName) <> MplabX.MplabxFilesOrig.Item(strFileName) Then
                aFilesToSave.Add(strFileName)
            End If
        Next
        For Each strFileName As String In aFilesToSave
            MplabX.FileSave(strFileName)
        Next

        If Common.MplabXProjectPath <> "" Then
            Dim oFolder As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oRootFolderNode, "Source Files/VGDD", False)
            If oFolder IsNot Nothing Or MplabX.IpcEnabled Then
                If Common.oGrcProjectInternal.ResourcesCount > 0 Then
                    If CodeGen.CodeEventsHelper.IndexOf(strIncludeResources) < 0 Then
                        CodeGen.CodeEventsHelper = CodeGen.AddInclude(CodeGen.CodeEventsHelper, strIncludeResources)
                    End If
                    Select Case Mal.FrameworkName.ToUpper
                        Case "MLALEGACY"
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesH)) Is Nothing Then
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesH)
                                aMissingFolders.Add("Header Files/VGDD")
                            End If
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesC)) Is Nothing Then
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesC)
                                aMissingFolders.Add("Source Files/VGDD")
                            End If
                        Case "MLA"
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesC)) Is Nothing Then
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesC)
                                aMissingFolders.Add(String.Format("Source Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                            End If
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesRefC)) Is Nothing Then
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesRefC)
                                aMissingFolders.Add(String.Format("Source Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                            End If
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesH)) Is Nothing Then
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesH)
                                aMissingFolders.Add(String.Format("Header Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                            End If
                        Case "HARMONY"
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesC)) Is Nothing Then
                                aMissingFolders.Add(String.Format("Source Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesC)
                            End If
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesRefC)) Is Nothing Then
                                aMissingFolders.Add(String.Format("Source Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesRefC)
                            End If
                            If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_InternalResourcesH)) Is Nothing Then
                                aMissingFolders.Add(String.Format("Header Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                                aMissingFiles.Add(Common.ProjectFileName_InternalResourcesH)
                            End If
                    End Select
                End If
            End If

            If Common.oGrcProjectExternal.ResourcesCount > 0 Then
                If CodeGen.CodeEventsHelper.IndexOf(strIncludeExternalResources) < 0 Then
                    CodeGen.CodeEventsHelper = CodeGen.AddInclude(CodeGen.CodeEventsHelper, strIncludeExternalResources)
                End If
                If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_ExternalResourcesRefC)) Is Nothing Then
                    Select Case Mal.FrameworkName.ToUpper
                        Case "MLALEGACY"
                            aMissingFiles.Add(Common.ProjectFileName_ExternalResourcesRefC)
                            aMissingFolders.Add("Source Files/VGDD")
                            aMissingFiles.Add(Common.ProjectFileName_ExternalResourcesH)
                            aMissingFolders.Add("Header Files/VGDD")
                        Case "MLA"
                            aMissingFiles.Add(Common.ProjectFileName_ExternalResourcesRefC)
                            aMissingFolders.Add(String.Format("Source Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                            aMissingFiles.Add(Common.ProjectFileName_ExternalResourcesH)
                            aMissingFolders.Add(String.Format("Header Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                        Case "HARMONY"
                            aMissingFiles.Add(Common.ProjectFileName_ExternalResourcesRefC)
                            aMissingFolders.Add(String.Format("Source Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                            aMissingFiles.Add(Common.ProjectFileName_ExternalResourcesH)
                            aMissingFolders.Add(String.Format("Header Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                    End Select
                End If
            End If

            If Common.oGrcBinBmpOnSd.ResourcesCount > 0 Then
                If CodeGen.CodeEventsHelper.IndexOf(strIncludeBmpOnSdResources) < 0 Then
                    CodeGen.CodeEventsHelper = CodeGen.AddInclude(CodeGen.CodeEventsHelper, strIncludeBmpOnSdResources)
                End If
                'If MplabX.MplabXProjectSearchItem(Path.GetFileName(Common.ProjectFileName_BmpOnSdResourcesC)) Is Nothing Then
                Select Case Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        aMissingFiles.Add(Common.ProjectFileName_BmpOnSdResourcesC)
                        aMissingFolders.Add("Source Files/VGDD")
                        aMissingFiles.Add(Common.ProjectFileName_BmpOnSdResourcesH)
                        aMissingFolders.Add("Header Files/VGDD")
                    Case "MLA"
                        aMissingFiles.Add(Common.ProjectFileName_BmpOnSdResourcesC)
                        aMissingFolders.Add(String.Format("Source Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                        aMissingFiles.Add(Common.ProjectFileName_BmpOnSdResourcesH)
                        aMissingFolders.Add(String.Format("Header Files/appMLA/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                    Case "HARMONY"
                        aMissingFiles.Add(Common.ProjectFileName_BmpOnSdResourcesC)
                        aMissingFolders.Add(String.Format("Source Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                        aMissingFiles.Add(Common.ProjectFileName_BmpOnSdResourcesH)
                        aMissingFolders.Add(String.Format("Header Files/app/system_config/{0}/vgdd", Common.MplabXSelectedConfig))
                End Select
                'End If
                For Each oBitmap As VGDDImage In Common.Bitmaps
                    If oBitmap.Type = VGDDImage.PictureType.BINBMP_ON_SDFAT _
                            AndAlso oBitmap.SDFileName IsNot Nothing _
                            AndAlso oBitmap.SDFileName.Length > 12 Then
                        Const DEFINE_SUPPORT_LFN As String = "#define SUPPORT_LFN"
                        MplabX.FileLoad("HardwareProfile.h")
                        If MplabX.MplabxFiles.Item("HardwareProfile.h").IndexOf(DEFINE_SUPPORT_LFN) < 0 Then
                            MplabX.MplabxFiles.Item("HardwareProfile.h") = CodeGen.AddInclude(MplabX.MplabxFiles.Item("HardwareProfile.h"), DEFINE_SUPPORT_LFN)
                            MplabX.FileSave("HardwareProfile.h")
                            MessageBox.Show("One or more of your BitmapOnSD has a long filename, that involves activating LFN - Long Filename Support in FSIO.c." & vbCrLf & _
                                "This has a BIG cost in term of FLASH: about 23K of more code is added when enabled." & vbCrLf & _
                                "If you want to save FLASH space, then simply keep your SD filenames in 8.3 DOS short mode (see BitmapChooser).", _
                                "Warning: Long Filename Support needed")
                        End If
                    End If
                Next

            End If

            If aMissingFiles.Count > 0 Then
                For i As Integer = 1 To aMissingFiles.Count
                    MplabX.MplabXAddFileIfNotExist(aMissingFolders(i), aMissingFiles(i))
                Next
            End If
        End If

        If MplabX.OriginalProjectXml IsNot Nothing AndAlso MplabX.CleanMplabXProject(MplabX.OriginalProjectXml.InnerXml) <> MplabX.CleanMplabXProject(MplabX.oProjectXmlDoc.InnerXml) Then
            If Not MplabX.SaveMplabXProject() Then
                MessageBox.Show("Cannot save modifications to MPLAB X Project. Close MPLAB X and retry.", "Error saving MPLAB X Project")
                Exit Sub
            End If
            If Not MplabX.MplabXClearMakeFile() Then
                Exit Sub
            End If
        End If

        Select Case Mal.FrameworkName.ToUpper
            Case "HARMONY"
                CodeGen.AllScreensEventMsgCode = CodeGen.AllScreensEventMsgCode.Replace("GFX_GOL_ObjectFind(ID_", "GFX_GOL_ObjectFind(GFX_INDEX_0,ID_")
        End Select

        Common.CodegenProgress += 1
        CodeGen.FinalCode = (CodeGen.CodeHead & vbCrLf & _
                CodeGen.Code & vbCrLf & _
                CodeGen.AllScreensEventMsgCode & vbCrLf & _
                CodeGen.sbCodeBitmap.ToString & vbCrLf & _
                CodeGen.CodeFoot & vbCrLf)
        CodeGen.Headers = CodeGen.Headers.Replace("[HEADERSINCLUDES]", CodeGen.HeadersIncludes)

        If Common.ProjectMultiLanguageTranslations > 0 AndAlso _
            Common.ProjectStringsPoolGenerateHeader Then
            '#include "VGDD_TestBug_StringsPool.h"
            CodeGen.Headers = CodeGen.AddInclude(CodeGen.Headers, String.Format("#include ""{0}""", Path.GetFileName(Common.ProjectFileName_StringsPoolH)))
        End If


        Common.CodegenProgress += 1

        Common.CodegenProgress += 1
        CodeGen.FinalCode = CodeGen.ReplaceProjectStrings(CodeGen.FinalCode).Replace("[EMPTYLINE]", "")

        With oShowSource
            .Timer1.Enabled = False
            .ProgressBar1.Value = .ProgressBar1.Maximum
            .ProgressBar1.ForeColor = Color.Green
            .btnShowSource.Enabled = True

            FootPrint.Heap += ScreenMaxHeap
            .ShowFootPrint()

            .lblStatus.Text = "Finished!"
            If Not _ProjectSummary.FootPrint1.Caption.Contains("CodeGen") Then
                _ProjectSummary.FootPrint1.Caption &= " from last CodeGen"
            End If
            _ProjectSummary.FootPrint1.UpdatePanel()
            _ProjectSummary.FootPrint1.Visible = True
        End With

        If Not WriteFile(Common.ProjectFileName_ScreensC, CodeGen.FinalCode, New System.Text.UTF8Encoding) Then
            Me.Cursor = Cursors.Default
            Exit Sub
        End If

        Do While CodeGen.Headers.Contains(vbCrLf & vbCrLf)
            CodeGen.Headers = CodeGen.Headers.Replace(vbCrLf & vbCrLf, vbCrLf)
        Loop
        CodeGen.Headers &= vbCrLf
        If Not WriteFile(Common.ProjectFileName_ScreensH, CodeGen.Headers, New System.Text.UTF8Encoding) Then
            Me.Cursor = Cursors.Default
            Exit Sub
        End If

        If Not WriteFile(Common.ProjectFileName_ScreenStatesH, CodeGen.ScreenStates) Then
            Me.Cursor = Cursors.Default
            Exit Sub
        End If

        If Mal.FrameworkName.ToUpper <> "HARMONY" Then
            If Not CodeGen.CodeEventsHelper.Contains("#include ""vgdd_main.h""") Then
                CodeGen.CodeEventsHelper = CodeGen.AddInclude(CodeGen.CodeEventsHelper, "#include ""vgdd_main.h""")
            End If

            If Not WriteFile(Common.ProjectFileName_EventsHelperH, CodeGen.CodeEventsHelper, New System.Text.UTF8Encoding) Then
                Me.Cursor = Cursors.Default
                Exit Sub
            End If
        End If

        CreateEmptyFileIfNotExists(Common.ProjectFileName_InternalResourcesC)
        Select Case VGDDCommon.Mal.FrameworkName.ToUpper
            Case "MLA", "HARMONY"
                CreateEmptyFileIfNotExists(Common.ProjectFileName_InternalResourcesRefC)
        End Select
        CreateEmptyFileIfNotExists(Common.ProjectFileName_InternalResourcesH)

        Me.Cursor = Cursors.Default
        Common.CodegenGeneratingCode = False
        Common.CodeHasBeenGenerated = True
        Common.ProjectLastComputedHeapSize = FootPrint.Heap
        If oFrmMplabxWizard IsNot Nothing AndAlso oFrmMplabxWizard.Visible Then
            oFrmMplabxWizard.btnGenerateCode.Enabled = Not Common.CodeHasBeenGenerated
            oFrmMplabxWizard.lblWarnGenerateCode.Visible = Not Common.CodeHasBeenGenerated
        End If
        oShowSource.btnClose.Enabled = True
        If Common.BitmapsBinUsed > 0 Then
            oShowSource.lblBinBitmaps.Text = Common.BitmapsBinUsed & " Bitmaps in BIN format:"
            oShowSource.lblBinBitmaps.Visible = True
            oShowSource.lblBinBitmaps.ForeColor = Color.Black
            oShowSource.btnBinBitmaps.Visible = True
        ElseIf Common.oGrcProjectExternal.OutputFormat = GrcProject.OutputFormats.Binary AndAlso Common.oGrcProjectExternal.LastOutputFile <> String.Empty Then
            oShowSource.lblBinBitmaps.Text = " Generated BIN resource file: " & Common.oGrcProjectExternal.LastOutputFile.Replace(".S", ".bin")
            oShowSource.lblBinBitmaps.Visible = True
            oShowSource.lblBinBitmaps.ForeColor = Color.Green
            oShowSource.btnBinBitmaps.Visible = False
        End If

        If CodeGen.Errors <> String.Empty Then
            _OutputWindow.WriteMessage("Codegen errors and warnings:" & vbCrLf & CodeGen.Errors)
            If _OutputWindow.DockState > 2 And _OutputWindow.DockState < 5 Then
                _OutputWindow.Show(_OutputWindow.DockPanel, _OutputWindow.DockState + 5)
            End If
        End If

    End Sub

    Public Sub CreateEmptyFileIfNotExists(ByVal strFileName As String)
        CreateEmptyFileIfNotExists(strFileName, "")
    End Sub

    Public Sub CreateEmptyFileIfNotExists(ByVal strFileName As String, ByVal content As String)
        If Not File.Exists(strFileName) Then
            WriteFile(strFileName, content)
        End If
    End Sub

    Private Sub ToolStripOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripOpen.Click
        MenuOpenProject.PerformClick()
    End Sub

    Private Sub ToolStripSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripSaveProject.Click
        MenuSaveProjectN.PerformClick()
    End Sub

    Private Sub ToolStripCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripCut.Click
        MenuCut.PerformClick()
    End Sub

    Private Sub ToolStripCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripCopy.Click
        MenuCopy.PerformClick()
    End Sub

    Private Sub ToolStripDeleteWidget_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripDeleteWidget.Click
        MenuDelete.PerformClick()
    End Sub

    Private Sub ToolStripPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripPaste.Click
        MenuPaste.PerformClick()
    End Sub

    Private Sub ToolStripHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripHelp.Click
        MenuHelp.PerformClick()
    End Sub

    Private Sub ToolStripNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripNewScreen.Click
        MenuScreenNew.PerformClick()
    End Sub

    Private Sub oMicrochipCommon_BitmapAdded(ByVal vImage As VGDDImage) Handles oMicrochipCommon.BitmapAdded
        If Me._SolutionExplorer.tvSolution.Nodes(0).Nodes("BITMAPS") Is Nothing Then
            Me._SolutionExplorer.tvSolution.Nodes(0).Nodes.Add("BITMAPS", "Bitmaps")
        End If
        Dim oBitmapNode As TreeNode = Me._SolutionExplorer.tvSolution.Nodes(0).Nodes("BITMAPS").Nodes.Add(vImage.Name, vImage.Name)
        If vImage.Bitmap IsNot Nothing Then
            Try
                Me._SolutionExplorer.ProjectImageList.Images.Add(vImage.Bitmap)
                oBitmapNode.ImageIndex = Me._SolutionExplorer.ProjectImageList.Images.Count - 1
                oBitmapNode.SelectedImageIndex = oBitmapNode.ImageIndex
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub oMicrochipCommon_BitmapRemoved(ByVal vImage As VGDDImage) Handles oMicrochipCommon.BitmapRemoved
        Dim oNode As TreeNode = Me._SolutionExplorer.tvSolution.Nodes(0).Nodes("BITMAPS").Nodes(vImage.Name)
        If oNode IsNot Nothing Then
            oNode.Remove()
            Me._SolutionExplorer.tvSolution.Refresh()
        End If
    End Sub

    Private Sub ToolStripGenerateCode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripGenerateCode.Click
        MenuGenerateCode.PerformClick()
    End Sub

    Private Sub MenuCloseScreen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuCloseScreen.Click
        Try
            Dim strScreenName As String = _CurrentHost.Screen.Name
            If MessageBox.Show("Remove " & strScreenName & " from project?", "Confirm Close Screen", MessageBoxButtons.YesNo) = vbYes Then
                CloseScreen(strScreenName, True)
                RemoveScreenFromProject(strScreenName)
                tmrCheckMiniature.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show("Can't remove screen:" & ex.Message)
        End Try
    End Sub

    Private Sub oMicrochipCommon_FontAdded(ByVal VFont As VGDDFont) Handles oMicrochipCommon.FontAdded
        Dim strFontKey As String = Common.FontToString(VFont.Font) 'String.Format("{0} {1} {2}", VFont.Font.FontFamily.Name, VFont.Font.Size, VFont.Font.Style.ToString)
        Me._SolutionExplorer.FontAdd(strFontKey)
    End Sub

    Private Sub oMicrochipCommon_FontRemoving(ByVal VFont As VGDDFont, ByRef Cancel As Boolean) Handles oMicrochipCommon.FontRemoving
        Try
            If VFont IsNot Nothing Then
                If _SelectedControls IsNot Nothing AndAlso _SelectedControls.Count > 0 Then
                    Dim strMessage As String = String.Empty
                    For i As Integer = 0 To VFont.UsedBy.Count - 1
                        Dim o As Object = VFont.UsedBy(i)
                        If o IsNot _SelectedControls(0) Then
                            strMessage &= o.Name & " (" & o.GetType.ToString & ","
                        End If
                    Next
                    If strMessage <> String.Empty Then
                        If MessageBox.Show(VFont.Name & " could be used by " & strMessage.Substring(0, strMessage.Length - 1) & "). Remove it anyway?", "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbNo Then
                            Cancel = True
                            Exit Sub
                        End If
                    End If
                End If
                Dim strFontKey As String = Common.FontToString(VFont.Font) 'String.Format("{0} {1} {2}", VFont.Font.FontFamily.Name, VFont.Font.Size, VFont.Font.Style.ToString)
                Me._SolutionExplorer.FontRemove(strFontKey)
            End If

        Catch ex As Exception

        End Try
    End Sub

#Region "SchemesChooser"
    Public Sub RefreshCmbScheme()
        RefreshCmbScheme(Nothing)
    End Sub

    Public Sub RefreshCmbScheme(ByVal SelectedScheme As String)
        _SchemesChooser.LoadSchemes(Common._Schemes, SelectedScheme)
    End Sub

    Private Sub SchemesChooser_DeleteScheme(ByVal SchemeToDelete As Object) Handles _SchemesChooser.DeleteScheme
        Try
            Dim oScheme As VGDDScheme = SchemeToDelete
            oScheme.Font.RemoveUsedBy(oScheme)
            Common._Schemes.Remove(oScheme.Name)
            'Common._Schemes = Common._Schemes.Clone
        Catch ex As Exception
        End Try
        ProjectChanged()
        RefreshCmbScheme()
    End Sub

    Private Sub SchemesChooser_SelectedSchemeChanged(ByVal SelectedScheme As Object) Handles _SchemesChooser.SelectedSchemeChanged
        Common.SelectedScheme = SelectedScheme
    End Sub

    Private Sub SchemesChooser_NewScheme() Handles _SchemesChooser.NewScheme
        Dim oScheme As VGDDScheme = Common.NewScheme(Common.SelectedScheme)
        oScheme.GradientType = oScheme.GradientType
        RefreshCmbScheme(oScheme.Name)
    End Sub

    Private Sub _SchemesChooser_SelectedSchemeFontChanged(ByVal SelectedScheme As Object) Handles _SchemesChooser.SelectedSchemeFontChanged
        Dim oSelectedScheme As VGDDScheme = SelectedScheme
        For Each oWidget As Common.IVGDDWidget In oSelectedScheme.UsedBy.Clone
            Try
                'If TypeOf obj Is Common.IVGDDWidget Then
                'Dim oWidget As Common.IVGDDWidget = obj
                oWidget.Scheme = Nothing
                oWidget.Scheme = _SchemesChooser.SelectedSchemeName
                'End If
            Catch ex As Exception
            End Try
        Next
        Dim s As ISelectionService = DirectCast(_CurrentHost.DesignerHost.GetService(GetType(ISelectionService)), ISelectionService)
        Dim oCurrentSelection As ICollection = s.GetSelectedComponents
        s.SetSelectedComponents(Nothing)
        s.SetSelectedComponents(oCurrentSelection)
        ScreenChanged()
    End Sub

    Private Sub _SchemesChooser_SchemePropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles _SchemesChooser.SchemePropertyValueChanged
        Try
            Select Case e.ChangedItem.Label
                Case "(Name)", "Name"
                    Dim oScheme As VGDDScheme = Common._Schemes(e.OldValue)
                    If Common._Schemes.ContainsKey(e.OldValue) Then
                        Common._Schemes.Remove(e.OldValue)
                    End If
                    Dim strNewName As String = e.ChangedItem.Value
                    If Common._Schemes.ContainsKey(strNewName) Then
                        Dim i As Integer
                        Do While Common._Schemes.ContainsKey(strNewName & Chr(i + Asc("a")))
                            i += 1
                        Loop
                        strNewName &= Chr(i + Asc("a"))
                    End If
                    oScheme.Name = strNewName
                    Common._Schemes.Add(strNewName, oScheme)
                    RefreshCmbScheme()
                    _SchemesChooser.cmbScheme.SelectedItem = oScheme
                Case "GradientType"
                    RefreshCmbScheme(_SchemesChooser.SelectedSchemeName)
                    'Case "Font"
                    '    Dim oldFont As VGDDFont = e.OldValue
                    '    Dim oNewFont As VGDDFont = e.ChangedItem.Value
                    '    For Each oScheme As VGDDScheme In Common._Schemes
                    '        If oScheme.Font.Name = oldFont.Name Then
                    '            oScheme.Font = oNewFont
                    '        End If
                    '    Next
                    'Case "Font"
                    '    Dim oNewFont As VGDDFont = e.ChangedItem.Value
                    '    Dim oOldFont As VGDDFont = e.OldValue
                    '    For Each oVFont As VGDDFont In Common._Fonts
                    '        If oVFont.Name = oOldFont.Name Then
                    '            oVFont.Name = oNewFont.Name
                    '            oVFont.Font = oNewFont.Font
                    '        End If
                    '    Next
                Case Else
                    Debug.Print(e.ChangedItem.Label)
            End Select
            'Dim oLoader As BasicHostLoader = CurrentDocumentsHostControl.HostSurface.Loader
            'oLoader.PerformFlushWork()
        Catch ex As Exception
        End Try

        If _CurrentHost IsNot Nothing Then
            _CurrentHost.Invalidate()
            CType(_hostSurfaceManager.ActiveDesignSurface.View, Control).Refresh()
        End If
        ProjectChanged()

    End Sub

    Private Sub SchemesChooser_ApplyScheme(ByVal SelectedSchemeName As String) Handles _SchemesChooser.ApplyScheme
        Try
            Dim SetScheme As VGDDScheme = Common.GetScheme(SelectedSchemeName)
            If _SelectedControls IsNot Nothing Then
                For Each obj As Object In _SelectedControls
                    Try
                        If TypeOf obj Is Common.IVGDDWidget Then
                            Dim oWidget As Common.IVGDDWidget = obj
                            oWidget.Scheme = Nothing
                            oWidget.Scheme = SelectedSchemeName
                            ScreenChanged()
                        End If
                    Catch ex As Exception
                    End Try
                Next
            End If
            For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                Dim oScreen As VGDD.VGDDScreen = oScreenAttr.Screen
                If FindScreen(oScreen.Name) IsNot Nothing Then
                    For Each obj As Object In oScreen.Controls
                        Try
                            If TypeOf obj Is Common.IVGDDWidget Then
                                If CType(obj, Common.IVGDDWidget).Scheme = SelectedSchemeName Then
                                    CType(obj, Common.IVGDDWidget).SchemeObj = SetScheme 'Refresh font
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Next
                End If
            Next
            _CurrentHost.Invalidate()
            _CurrentHost.DesignerHost.Activate()

        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region "EventsTree"

    Private Sub SelectScreen(ByVal strScreenName As String)
        Dim hc As HostControl = FindScreen(strScreenName)
        If hc IsNot Nothing Then
            hc.Focus()
        Else
            Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(strScreenName)
            If oScreenAttr IsNot Nothing Then
                AddTabForNewHost(oScreenAttr.Name, oScreenAttr.Hc, True)
                'Dim oNode As TreeNode = _SolutionExplorer.tvSolution.Nodes(0).Nodes("SCREENS").Nodes(strScreenName)
                'If oNode IsNot Nothing Then
                '    OpenScreen(CType(oNode.Tag, VGDD.VGDDScreenAttr).FileName, True)
                'End If
            End If
        End If
    End Sub


    Private oSelectedEventNode As TreeNode
    Private strSelectedEventNodeSpecificEvent As String

    Private Function _EventTree_GetSelectedObject() As Object
        Try
            strSelectedEventNodeSpecificEvent = String.Empty
            If oSelectedEventNode IsNot Nothing Then
                'oSelectedNode.NodeFont = Common.oEventsFontNormal
                oSelectedEventNode.ForeColor = _EventTree.tvEvents.ForeColor
            End If
            oSelectedEventNode = _EventTree.tvEvents.SelectedNode
            If _CurrentHost Is Nothing OrElse _CurrentHost.DesignerHost Is Nothing Then Return Nothing
            Dim s As ISelectionService = DirectCast(_CurrentHost.DesignerHost.GetService(GetType(ISelectionService)), ISelectionService)
            Dim oControl As Control = Nothing
            If TypeOf (oSelectedEventNode.Tag) Is VGDD.VGDDScreen Then
                Common.CurrentScreen = oSelectedEventNode.Tag
                oControl = Common.CurrentScreen
                SelectScreen(Common.CurrentScreen.Name)
                's.SetSelectedComponents({Common.CurrentScreen})
                Return Common.CurrentScreen
            ElseIf TypeOf (oSelectedEventNode.Tag) Is VGDDEvent Then
                If Common.CurrentScreen IsNot oSelectedEventNode.Parent.Parent.Tag Then
                    Common.CurrentScreen = oSelectedEventNode.Parent.Parent.Tag
                    SelectScreen(Common.CurrentScreen.Name)
                    If Common.CurrentScreen IsNot oSelectedEventNode.Parent.Parent.Tag Then
                        Return Nothing
                    End If
                End If
                oControl = Common.CurrentScreen.Controls.Find(oSelectedEventNode.Parent.Name, False)(0)
                strSelectedEventNodeSpecificEvent = oSelectedEventNode.Text
            ElseIf oSelectedEventNode.Text <> "" AndAlso _CurrentHost IsNot Nothing AndAlso Common.CurrentScreen IsNot Nothing Then
                Dim aControls() As Control = Common.CurrentScreen.Controls.Find(oSelectedEventNode.Text, True)
                If aControls.Length > 0 Then
                    oControl = aControls(0)
                End If
            End If
            If oControl IsNot Nothing Then
                s.SetSelectedComponents({oControl})
            End If
            Return oControl
        Catch ex As Exception
        End Try
        Return Nothing
    End Function

    Private Sub _EventTree_EventNodeClick() Handles _EventTree.EventNodeClick
        Dim oControl As Control = _EventTree_GetSelectedObject()
    End Sub

    Private Sub _EventTree_EventNodeDoubleClick() Handles _EventTree.EventNodeDoubleClick
        Try
            Dim oControl As Control = _EventTree_GetSelectedObject()
            If oControl IsNot Nothing AndAlso TypeOf (oControl) Is Common.IVGDDEvents Then
                With _EventsEditor
                    .SetEditedControl(oControl)
                    .ParentScreenName = Common.CurrentScreen.Name
                    .DialogResult = DialogResult.Retry
                    If strSelectedEventNodeSpecificEvent <> String.Empty Then
                        .cmbEvents.Text = strSelectedEventNodeSpecificEvent
                    End If
                    .DisplayModal()
                End With
            End If
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

    Private Sub GetAllEvents(ByRef oXmlEventsDocument As XmlDocument, ByVal oControl As Control)
        Dim oAttr As XmlAttribute
        Dim oEventNode As XmlNode
        Dim oEvents As Common.IVGDDEvents = oControl
        If oEvents.VGDDEvents Is Nothing Then Exit Sub
        For Each oEvent As VGDDEvent In oEvents.VGDDEvents
            If oEvent.Code <> String.Empty Then
                oEventNode = oXmlEventsDocument.CreateElement("EventCode")
                Dim oScreen As VGDD.VGDDScreen = Nothing
                If TypeOf (oControl.Parent) Is VGDD.VGDDScreen Then
                    oScreen = oControl.Parent
                End If
                If oScreen IsNot Nothing Then
                    oAttr = oXmlEventsDocument.CreateAttribute("Screen")
                    oAttr.Value = oScreen.Name
                    oEventNode.Attributes.Append(oAttr)
                End If
                oAttr = oXmlEventsDocument.CreateAttribute("Widget")
                oAttr.Value = oControl.Name
                oEventNode.Attributes.Append(oAttr)

                oAttr = oXmlEventsDocument.CreateAttribute("Event")
                oAttr.Value = oEvent.Name
                oEventNode.Attributes.Append(oAttr)

                oEventNode.InnerText = vbCrLf & oEvent.Code
                Do While oEventNode.InnerText.EndsWith(vbCrLf)
                    oEventNode.InnerText = oEventNode.InnerText.Substring(0, oEventNode.InnerText.Length - 2)
                Loop
                oEventNode.InnerText &= vbCrLf
                oXmlEventsDocument.DocumentElement.AppendChild(oEventNode)
            End If
        Next
    End Sub

#Region "AllEvents"

    Public Function AllEventsToXml(ByVal oControl As Control) As String
        Dim oXmlEventsDocument As New XmlDocument
        Dim oEventsNode As XmlNode = oXmlEventsDocument.CreateElement("AllEvents")
        oXmlEventsDocument.AppendChild(oEventsNode)

        If oControl Is Nothing Then 'Project
            For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                'Dim hc As HostControl = FindScreen(oScreenAttr.Name)
                'If hc Is Nothing Then
                '    AddTabForNewHost(oScreenAttr.Name, oScreenAttr.Hc, True)
                'End If
                GetAllEvents(oXmlEventsDocument, oScreenAttr.Screen)
                For Each oScreenControl As Control In oScreenAttr.Screen.Controls
                    If TypeOf (oScreenControl) Is Common.IVGDDEvents Then
                        GetAllEvents(oXmlEventsDocument, oScreenControl)
                    End If
                Next
            Next
        ElseIf TypeOf (oControl) Is VGDD.VGDDScreen Then
            GetAllEvents(oXmlEventsDocument, oControl)
            For Each oScreenControl As Control In oControl.Controls
                If TypeOf (oScreenControl) Is Common.IVGDDEvents Then
                    GetAllEvents(oXmlEventsDocument, oScreenControl)
                End If
            Next
        ElseIf TypeOf (oControl) Is Common.IVGDDEvents Then
            GetAllEvents(oXmlEventsDocument, oControl)
        End If

        Dim sw As New StringWriter
        Dim xtw As New XmlTextWriter(sw)
        xtw.Formatting = Formatting.Indented
        oXmlEventsDocument.WriteTo(xtw)
        Dim strEventsXml As String = sw.ToString.Replace("  <", "<")
        Return strEventsXml
    End Function

    Public Sub AllEventsFromXML(ByVal strXml As String)
        Dim oXmlEventsDocument As New XmlDocument
        Dim oAttr As XmlAttribute = Nothing
        Try
            oXmlEventsDocument.LoadXml(strXml)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error reloading events XML")
            Exit Sub
        End Try
        For Each oEventNode As XmlNode In oXmlEventsDocument.DocumentElement.ChildNodes
            Try
                Dim oWidget As VGDDCommon.Common.IVGDDEvents = Nothing
                Dim strWidgetName As String = oEventNode.Attributes("Widget").Value
                Dim strEventName As String = oEventNode.Attributes("Event").Value
                Dim oScreen As VGDD.VGDDScreen = Nothing
                If oEventNode.Attributes("Screen") IsNot Nothing Then
                    Dim strScreenName As String = oEventNode.Attributes("Screen").Value
                    'For Each ds As System.ComponentModel.Design.DesignSurface In oMainShell._hostSurfaceManager.DesignSurfaces
                    '    oScreen = ds.ComponentContainer.Components(0)
                    '    If oScreen.Name = strScreenName Then
                    '        oWidget = oScreen.Controls(strWidgetName)
                    '        Exit For
                    '    End If
                    'Next
                    'If oScreen Is Nothing Then
                    oScreen = VGDDCommon.Common.aScreens.Item(strScreenName).Screen
                    'End If
                    oWidget = oScreen.Controls(strWidgetName)
                Else
                    oScreen = VGDDCommon.Common.aScreens.Item(strWidgetName).Screen
                    oWidget = oScreen
                End If
                If oWidget IsNot Nothing Then
                    Dim strEventCode As String = oEventNode.InnerText
                    Do While strEventCode.StartsWith(vbCrLf)
                        strEventCode = strEventCode.Substring(2)
                    Loop
                    Do While strEventCode.EndsWith(vbCrLf)
                        strEventCode = strEventCode.Substring(0, strEventCode.Length - 2)
                    Loop
                    Dim oEvent As VGDDEvent = oWidget.VGDDEvents(strEventName)
                    If oEvent IsNot Nothing AndAlso oEvent.Code <> strEventCode Then
                        Dim _EventsPropertyDescriptor As PropertyDescriptor = TypeDescriptor.GetProperties(oWidget)("VGDDEvents")
                        Dim _EventsCollection As VGDDEventsCollection = _EventsPropertyDescriptor.GetValue(oWidget)
                        oEvent = _EventsCollection(strEventName)
                        oEvent.Code = strEventCode
                        oEvent.Handled = (strEventCode.Trim <> String.Empty)
                        _EventsPropertyDescriptor.SetValue(oWidget, _EventsCollection)
                        oMainShell.ScreenChanged(oScreen.Name)
                    End If
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Sub _EventTree_EventNodeEditAllEvents() Handles _EventTree.EventNodeEditAllEvents
        EditAllEvents()
    End Sub

    Private Sub MenuEditAllEvents_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuEditAllEvents.Click
        EditAllEvents()
    End Sub

    Public Sub EditAllEvents()
        Try
            Dim oEventsAllEditor As New VGDDIDE.EventsAllEditor
            With oEventsAllEditor
                .DialogResult = DialogResult.Retry
                .TextEditorControl1.ActiveEditor.Document.TextContent = AllEventsToXml(_EventTree_GetSelectedObject())
                If .ShowDialog() = DialogResult.OK Then
                    '_EventsEditor.RefreshEventCode()
                    'For Each strScreenName As String In .aChangedScreenNames
                    '    CloseScreen(strScreenName, True)
                    '    ScreenChanged(strScreenName)
                    'Next
                End If
            End With
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

#End Region

#End Region

#Region "ContextHelp"

    Private Sub MainShell_HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs) Handles Me.HelpRequested
        Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_Main"
    End Sub

    Private Sub _EventTree_HelpRequested() Handles _EventTree.HelpRequested
        Help.ShowHelp(_WidgetPropertyGrid, Common.HELPNAMESPACEBASE & "_Events")
    End Sub

    Private Sub _SchemesChooser_HelpRequested() Handles _SchemesChooser.HelpRequested
        Help.ShowHelp(_SchemesChooser, Common.HELPNAMESPACEBASE & "_Schemes", _SchemesChooser._SchemePropertyGrid.SelectedGridItem.Label)
    End Sub

    Private Sub _SolutionExplorer_HelpRequested() Handles _SolutionExplorer.HelpRequested
        Help.ShowHelp(_SolutionExplorer, Common.HELPNAMESPACEBASE & "_SolutionExplorer")
    End Sub

    Private Sub _WidgetInfo_HelpRequested() Handles _WidgetInfo.HelpRequested
        Dim strWidgetType As String = _WidgetPropertyGrid.PropertyGrid.SelectedObject.ToString.Split("[")(1).Split(".")(1)
        If strWidgetType.Contains("]") Then
            strWidgetType = strWidgetType.Substring(0, strWidgetType.IndexOf("]"))
        End If
        Help.ShowHelp(_WidgetPropertyGrid, Common.HELPNAMESPACEBASE & "_" & strWidgetType)
    End Sub

    Private Sub _WidgetPropertyGrid_HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs) Handles _WidgetPropertyGrid.HelpRequested
        Dim strSubHelp As String = _WidgetPropertyGrid.PropertyGrid.SelectedObject.ToString.Split("[")(1).Split(".")(1)
        If strSubHelp.Contains("]") Then
            strSubHelp = strSubHelp.Substring(0, strSubHelp.IndexOf("]"))
        End If
        Dim strLabel As String = _WidgetPropertyGrid.PropertyGrid.SelectedGridItem.Label
        If strLabel.StartsWith("(") Then
            strLabel = strLabel.Substring(1, strLabel.Length - 2)
        End If
        Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_" & strSubHelp
        Common.HelpProvider.SetHelpKeyword(_WidgetPropertyGrid, strLabel)
        Help.ShowHelp(_WidgetPropertyGrid, Common.HelpProvider.HelpNamespace, strLabel)
    End Sub

#End Region

#Region "WidgetPropertyGrid"

    Public Sub RefreshPropertyGrid()
        _WidgetPropertyGrid.PropertyGrid.Refresh()
    End Sub


    Private Sub _WidgetPropertyGrid_PropertySortChanged() Handles _WidgetPropertyGrid.PropertySortChanged
        If Me.Visible Then
            My.Settings.MyPropertySort = _WidgetPropertyGrid.PropertyGrid.PropertySort
        End If
    End Sub
#End Region

    'Private Sub oKeyStrokeMessageFilter_Deleted() Handles oKeyStrokeMessageFilter.Deleted
    '    PopulateEvents(Common.CurrentScreen)
    'End Sub

    Private Sub ToolStripProjectSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripProjectSettings.Click
        MenuProjectSettings.PerformClick()
    End Sub

    Private Sub MenuHelp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuHelp.Click
        VGDDCommon.Common.RunBrowser("http://virtualfab.it/VGDD")
    End Sub

    Private Sub MenuWiki_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuWiki.Click
        MenuHelp.PerformClick()
    End Sub

    Private Sub ToolStripSameH_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripSameH.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.SizeToControlHeight)
    End Sub

    Private Sub ToolStripSameW_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripSameW.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.SizeToControlWidth)
    End Sub

    Private Sub ToolStripSameSize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripSameSize.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.SizeToControl)
    End Sub

    Private Sub MenuCustomWidgetsDefine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuCustomWidgetsDefine.Click
#If CONFIG <> "DemoRelease" And CONFIG <> "DemoDebug" Then
        Dim oCustomEditor As New frmCustomWidgetEditor
        oCustomEditor.ShowDialog()
        oCustomEditor = Nothing
        VGDDCustom.LoadCustomTemplatesDoc()
        VGDDCustom.UsedCustomProps.Clear()
        If _CurrentHost IsNot Nothing Then _CurrentHost.Invalidate()
        Me._Toolbox.InitializeToolbox()
#Else
        MessageBox.Show("This feature is available only in the Full Version of VGDD")
#End If
    End Sub

    Private Sub MenuExportProject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuExportProject.Click
        If CheckModifiedProject() = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If

        Dim strExportDir As String = ""
        Dim dlg As New FolderBrowserDialog
        dlg.Description = "Choose export parent folder - A subfolder will be created"
        dlg.ShowNewFolderButton = True
        dlg.RootFolder = Environment.SpecialFolder.MyComputer
        dlg.SelectedPath = Common.VGDDProjectPath
        SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
        If (dlg.ShowDialog = DialogResult.OK) Then
            Dim i As Integer = 1
            Do While True
                strExportDir = Path.Combine(dlg.SelectedPath, Common.ProjectName & "_Export" & i)
                If Not Directory.Exists(strExportDir) Then Exit Do
                i += 1
            Loop
            Try
                MkDir(strExportDir)
            Catch ex As Exception
                MessageBox.Show("Unable to create Export Folder " & strExportDir, "Error")
                Exit Sub
            End Try
            If Me.SaveProject(Path.Combine(strExportDir, Common.ProjectFileName), strExportDir) Then
                If MessageBox.Show("Project successfully exported." & vbCrLf & "Do you want to open " & strExportDir & " folder ?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.Yes Then
                    Try
                        Process.Start(strExportDir)
                    Catch ex As Exception
                        MessageBox.Show("Error opening folder " & strExportDir & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End If
        End If
    End Sub

    Private Sub MenuExportPlayer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuExportPlayer.Click
#If CONFIG = "DemoRelease" Or CONFIG = "DemoDebug" Then
        MessageBox.Show("This feature is available only in the Full Version of VGDD")
#Else
        If CheckModifiedProject() = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If

        Dim dlg As New SaveFileDialog
        dlg.Title = "Export Player Package!"
        dlg.DefaultExt = "vpp"
        dlg.Filter = "VGDD Player Package|*.vpp"
        dlg.FileName = Path.GetFileNameWithoutExtension(Common.ProjectFileName) & ".vpp"
        dlg.AddExtension = True
        If Not dlg.ShowDialog = DialogResult.OK Then
            Exit Sub
        End If

        Dim strPackageFile As String = dlg.FileName

        Dim oXmlPackageDoc As XmlDocument = New XmlDocument
        Dim oPackageNode As XmlNode = oXmlPackageDoc.AppendChild(oXmlPackageDoc.CreateElement("VGDDPackage"))

        Dim oProjectAttr As XmlAttribute = oXmlPackageDoc.CreateAttribute("Name")
        oProjectAttr.Value = Path.GetFileNameWithoutExtension(strPackageFile)
        oPackageNode.Attributes.Append(oProjectAttr)

        Dim oProjectNodes As XmlElement = oXmlPackageDoc.ImportNode(oXmlProjectDoc.DocumentElement, True)
        For Each oNode As XmlNode In oProjectNodes.ChildNodes
            If oNode.Name = "Bitmap" Then
                Dim strBitmapFileName As String = oNode.Attributes("FileName").Value
                strBitmapFileName = Path.GetFileName(strBitmapFileName)
                oNode.Attributes("FileName").Value = strBitmapFileName
            End If
        Next
        oPackageNode.AppendChild(oProjectNodes)

        Dim oScreensNode As XmlNode = oPackageNode.AppendChild(oXmlPackageDoc.CreateElement("Screens"))

        For Each oScreenNode As XmlNode In oProjectNodes.ChildNodes
            If oScreenNode.Name = "Screen" Then
                Dim strScreenFileName As String = Path.Combine(Common.VGDDProjectPath, oScreenNode.Attributes("FileName").Value)
                oScreenNode.Attributes("FileName").Value = Path.GetFileName(strScreenFileName) 'Tolgo il Path

                Try
                    Dim sr As StreamReader = New StreamReader(New FileStream(strScreenFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    Dim cleandown As String = sr.ReadToEnd
                    cleandown = "<Screen Name=""" & oScreenNode.Attributes("FileName").Value & """>" & cleandown & "</Screen>"
                    Dim oXmlScreenDoc As New XmlDocument
                    oXmlScreenDoc.LoadXml(cleandown)
                    Dim oOrigScreenNode As XmlElement = oXmlPackageDoc.ImportNode(oXmlScreenDoc.DocumentElement, True)
                    oScreensNode.AppendChild(oOrigScreenNode)

                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error exporting player package")
                End Try
            End If
        Next

        Dim oBitmapsNode As XmlNode = oPackageNode.AppendChild(oXmlPackageDoc.CreateElement("Bitmaps"))

        For Each oVgddImage As VGDDImage In Common._Bitmaps
            Dim oXmlNode As XmlNode = oXmlPackageDoc.CreateNode(XmlNodeType.Element, "", "Bitmap", "")
            oBitmapsNode.AppendChild(oXmlNode)

            Dim nameAttr As XmlAttribute = oXmlPackageDoc.CreateAttribute("FileName")
            nameAttr.Value = Path.GetFileName(oVgddImage.FileName)
            oXmlNode.Attributes.Append(nameAttr)

            Dim aImageBytes() As Byte = oVgddImage.GetBytes
            If aImageBytes IsNot Nothing Then
                Dim lenAttr As XmlAttribute = oXmlPackageDoc.CreateAttribute("Len")
                lenAttr.Value = aImageBytes.Length
                oXmlNode.Attributes.Append(lenAttr)

                oXmlNode.InnerText = vbCrLf & Space(8) & System.Convert.ToBase64String(aImageBytes, Base64FormattingOptions.InsertLineBreaks).Replace(vbCrLf, vbCrLf & Space(8)) & vbCrLf & Space(4)
            End If
            oBitmapsNode.AppendChild(oXmlNode)
        Next

        Dim oFirstScreenAttr As XmlAttribute = oXmlPackageDoc.CreateAttribute("FirstScreen")
        oFirstScreenAttr.Value = Common.aScreens(0).Name
        oPackageNode.Attributes.Append(oFirstScreenAttr)

        'If Me._Lm.IsLicensed = xxx Then
        Dim oSignAttr As XmlAttribute = oXmlPackageDoc.CreateAttribute("Sign")
            oPackageNode.Attributes.Append(oSignAttr)
            Dim MD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim dataMd5 As Byte() = MD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(oPackageNode.InnerXml.Replace(" ", "").Replace(vbCr, "").Replace(vbLf, "") & "VGDDPlayer"))
            Dim sb As New StringBuilder()
            For i As Integer = 0 To dataMd5.Length - 1
                sb.AppendFormat("{0:x2}", dataMd5(i))
            Next
            oSignAttr.Value = sb.ToString
        'End If


        Dim sw As StringWriter
        sw = New StringWriter
        Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
        xtw.Formatting = Formatting.Indented
        oXmlPackageDoc.WriteTo(xtw)
        Try
            Dim file As StreamWriter = New StreamWriter(strPackageFile, False, New System.Text.UnicodeEncoding)
            file.Write(sw.ToString)
            file.Close()

        Catch ex As Exception
            MessageBox.Show("Error writing to " & strPackageFile & vbCrLf & ex.Message, "Error exporting player package", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
#End If
    End Sub

    Private Sub ToolStripPlayNow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripPlayNow.Click
        If CheckModifiedProject() <> Windows.Forms.DialogResult.Yes Then
            Exit Sub
        End If

        Dim xmlPlayer As New XmlDocument
        Dim rootNode As XmlElement = xmlPlayer.CreateElement("VGDDPlayer")

        Dim oAttr As XmlAttribute = xmlPlayer.CreateAttribute("ProjectFileName")
        oAttr.Value = Path.Combine(Common.VGDDProjectPath, Common.ProjectFileName) ' Me.oSolutionExplorer1.ProjectFileName
        rootNode.Attributes.Append(oAttr)
        xmlPlayer.AppendChild(rootNode)

        'Dim oPlayer As New VGDDPlayer.Player("d:\temp\Player.xml")
#If CONFIG <> "DemoRelease" And CONFIG <> "DemoDebug" Then
        'If Me._Lm.IsLicensed <> xxx Then
        '    Common.AnimationEnable = False
        'End If
        Player.Play(xmlPlayer, True) 'Me._Lm.IsLicensed = xxx)
#Else
        Player.Play(xmlPlayer, False)
#End If
    End Sub

    Private Sub ToolStripExportPackcage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripExportPackage.Click
        MenuExportPlayer.PerformClick()
    End Sub

    Private Sub PlayerOptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayerOptionsToolStripMenuItem.Click
        frmPlayerOptions.ShowDialog()
    End Sub

    Private Sub MenuMPLABXWizard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuMPLABXWizard.Click
        If Not CheckModifiedProject() = DialogResult.Yes Then
            Exit Sub
        End If
        oFrmMplabxWizard = New frmMPLABXWizard
        oFrmMplabxWizard.Top = 20
        oFrmMplabxWizard.Left = 20
        oFrmMplabxWizard.Height = Me.Height - 40
        oFrmMplabxWizard.Width = Me.Width - 40
        oFrmMplabxWizard.ShowDialog()
        If Common.ProjectChanged Then
            ToolStripSaveProject.Enabled = True
        End If
    End Sub

    Private Sub ToolStripMPLABX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMPLABX.Click
        MenuMPLABXWizard.PerformClick()
    End Sub

    Private Sub oShowSource_GenerateCode() Handles oShowSource.GenerateCode
        GenerateCode()
    End Sub

    Private Sub oShowSource_ProjectModified() Handles oShowSource.ProjectModified
        ProjectChanged()
    End Sub

    Private Sub oShowSource_Register() Handles oShowSource.Register
        MenuLicense.PerformClick()
    End Sub

    Public Function FindScreen(ByVal text As String) As IDockContent
        Try
            For Each content As IDockContent In _DockPanel1.Documents
                If TypeOf (content) Is HostControl Then
                    Dim oScreen As VGDD.VGDDScreen = CType(content, HostControl).Screen
                    If content.DockHandler.TabText = text Or (oScreen IsNot Nothing AndAlso (oScreen.Name = text Or oScreen.FileName = text)) Then
                        Return content
                    End If
                End If
            Next
            For Each oPane As DockPane In _DockPanel1.Panes
                If oPane.DockState = DockState.Float Then
                    If TypeOf (oPane.ActiveContent) Is HostControl Then
                        Dim content As IDockContent = oPane.ActiveContent
                        Dim oScreen As VGDD.VGDDScreen = CType(content, HostControl).Screen
                        If content.DockHandler.TabText = text Or oScreen.Name = text Or oScreen.FileName = text Then
                            Return content
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
        End Try
        Return Nothing
    End Function

    Public Function CloseScreen(ByVal strName As String, ByVal RemoveHc As Boolean) As Boolean
        Application.DoEvents()
        Dim hc As HostControl = FindScreen(strName)
        If hc IsNot Nothing AndAlso RemoveHc Then
            If Not CheckClosingScreen(hc, MessageBoxButtons.YesNo) Then
                Return False
            End If
        End If
        Dim strScreenName As String = Path.GetFileNameWithoutExtension(strName).ToUpper
        Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(strScreenName)
        If oScreenAttr IsNot Nothing Then
            oScreenAttr.Shown = False
            Common.ProjectChanged = True
        End If
        If RemoveHc Then
            If hc Is Nothing Then
                If oScreenAttr IsNot Nothing Then
                    hc = oScreenAttr.Hc
                End If
            End If
            Try
                Common.aScreens.Remove(strScreenName)
            Catch ex As Exception
            End Try

            If hc IsNot Nothing Then
                If hc.HostSurface IsNot Nothing Then
                    Dim SelectionService As ISelectionService = hc.HostSurface.GetService(GetType(ISelectionService))
                    Dim ComponentChangeService As IComponentChangeService = hc.HostSurface.GetService(GetType(IComponentChangeService))
                    If SelectionService IsNot Nothing Then
                        RemoveHandler SelectionService.SelectionChanged, AddressOf Me._CurrentHost_SelectionChanged
                    End If
                    If ComponentChangeService IsNot Nothing Then
                        RemoveHandler ComponentChangeService.ComponentAdded, AddressOf Me._CurrentHost_ControlAdded
                        'RemoveHandler ComponentChangeService.ComponentAdding, AddressOf Me._CurrentHost_ControlAdding
                        RemoveHandler ComponentChangeService.ComponentRemoved, AddressOf Me._CurrentHost_ControlRemoved
                        RemoveHandler ComponentChangeService.ComponentChanged, AddressOf Me._CurrentHost_ControlChanged
                    End If
                    SelectionService = Nothing
                    ComponentChangeService = Nothing

                End If
                If Not hc.IsDisposed Then
                    'hc.HostSurface.Dispose()
                    hc.Dispose()
                End If
            Else
                Return False
            End If
        End If

        Return True
    End Function

    Public Function CheckClosingScreen(ByVal oPage As HostControl, ByVal Buttons As MessageBoxButtons) As Boolean
        If oPage.Text.EndsWith("*") Then
            oPage.Select()
            Dim rc As DialogResult = MessageBox.Show("Screen " & oPage.Text.Substring(0, oPage.Text.Length - 1) & " has been modified. Save it before closing?", "Save screen", Buttons, MessageBoxIcon.Question)
            Select Case rc
                Case DialogResult.Yes
                    SaveScreenOnTab(oPage)
                    ProjectChanged()
                Case Windows.Forms.DialogResult.Cancel
                    Return False
            End Select
            If Common.CurrentScreen IsNot Nothing AndAlso Common.CurrentScreen.FileName = "" AndAlso Common.CurrentScreen.Controls.Count > 0 Then
                If MessageBox.Show("Warning: Screen has never been saved. You are going to lose all its content!" & vbCrLf & "Cancel closing screen?", "WARNING: UNSAVED SCREEN", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.Yes Then
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Private Sub MenuMPLABXWizardTemplatesEditor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuMPLABXWizardTemplatesEditor.Click
#If CONFIG = "DemoRelease" Or CONFIG = "DemoDebug" Then
        MessageBox.Show("This feature is available only in the Full Version of VGDD")
#Else
        frmMplabXTemplatesEditor.Show()
#End If
    End Sub

#Region "ExternalWidgets"

    Public WithEvents oExternalWidgetsHandler As New ExternalWidgetsHandler

    Private Sub oExternalWidgetsHandler_Changed() Handles oExternalWidgetsHandler.Changed
        _Toolbox.InitializeToolbox()
    End Sub

    Private Sub MenuExternalWidgets_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuExternalWidgets.Click
        'frmExternalWidgets.ShowDialog()
        Dim oFrmExternal As New frmExternalWidgets
        oFrmExternal.ShowDialog()
    End Sub

    Public Function GetLibraryAssembly(ByVal strAssemblyName As String) As Assembly
        If strAssemblyName.StartsWith("VW-") Then strAssemblyName = "VirtualWidgets"
        If ExternalWidgetsHandler.ExternalWidgets.ContainsKey(strAssemblyName) Then
            Return ExternalWidgetsHandler.ExternalWidgets(strAssemblyName).Assembly
        ElseIf strAssemblyName.StartsWith("System") Then
            Debug.Print(strAssemblyName)
            Return Nothing
        End If
        '_OutputWindow.WriteMessage("External Widgets Library not found: " & strAssemblyName)
        Return Nothing
    End Function

    Public Function ExternalWidgetsGetLibraryType(ByVal strType As String, ByVal WarnIfNotFound As Boolean) As Type
        For Each oEw As ExternalWidget In ExternalWidgetsHandler.ExternalWidgets.Values
            Dim oAssembly As Assembly = oEw.Assembly
            Dim oType As Type = oAssembly.GetType(strType)
            If oType IsNot Nothing Then
                Return oType
            End If
        Next
        If WarnIfNotFound Then
            _OutputWindow.WriteMessage("Could not instantiate Library Type: " & strType)
        End If
        Return Nothing
    End Function

#End Region

    Private Sub ToolStripAlignLeft_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripAlignLeft.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.AlignLeft)
    End Sub

    Private Sub ToolStripAlignUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripAlignUp.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.AlignTop)
    End Sub

    Private Sub ToolStripAlignDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripAlignDown.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.AlignBottom)
    End Sub

    Private Sub ToolStripAlignRight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripAlignRight.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.AlignRight)
    End Sub

    Private Sub ToolStripCenterHoriz_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripCenterHoriz.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.AlignHorizontalCenters)
    End Sub

    Private Sub ToolStripCenterVert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripCenterVert.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.AlignVerticalCenters)
    End Sub

    Private Sub DistributeHorizontally_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DistributeHorizontally.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.HorizSpaceMakeEqual)
    End Sub

    Private Sub DistributeVertically_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DistributeVertically.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.VertSpaceMakeEqual)
    End Sub

    Private Sub ToolStripBringToFront_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripBringToFront.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.BringToFront)
        Dim oParent As Control = Nothing
        For Each ctl As Control In _SelectedControls
            If TypeOf ctl Is Common.IVGDDBase Then
                Dim intMaxZorder As Integer = 0
                For Each obj As Object In ctl.Parent.Controls
                    If Not obj Is ctl AndAlso TypeOf obj Is Common.IVGDDBase Then
                        Dim intZorder As Integer = CType(obj, Common.IVGDDBase).Zorder
                        If intMaxZorder < intZorder Then intMaxZorder = intZorder
                    End If
                Next
                CType(ctl, Common.IVGDDBase).Zorder = intMaxZorder + 1
                If oParent Is Nothing Then oParent = ctl.Parent
            End If
        Next
        If oParent IsNot Nothing Then Common.SetNewZOrder(oParent)
        _WidgetPropertyGrid.PropertyGrid.Refresh()
    End Sub

    Private Sub ToolStripSendToBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripSendToBack.Click
        If _CurrentHost Is Nothing Then Exit Sub
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        mcs.GlobalInvoke(MenuCommands.SendToBack)
        Dim oParent As Control = Nothing
        For Each ctl As Control In _SelectedControls
            If TypeOf ctl Is Common.IVGDDBase Then
                Dim intMinZorder As Integer = 256
                For Each obj As Object In ctl.Parent.Controls
                    If Not obj Is ctl AndAlso TypeOf obj Is Common.IVGDDBase Then
                        Dim intZorder As Integer = CType(obj, Common.IVGDDBase).Zorder
                        If intMinZorder > intZorder Then intMinZorder = intZorder
                    End If
                Next
                If intMinZorder >= 1 Then
                    intMinZorder -= 1
                End If
                CType(ctl, Common.IVGDDBase).Zorder = intMinZorder
                If oParent Is Nothing Then oParent = ctl.Parent
            End If
        Next
        If oParent IsNot Nothing Then Common.SetNewZOrder(oParent)
        _WidgetPropertyGrid.PropertyGrid.Refresh()
    End Sub

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub tmrMouseCoords_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrMouseCoords.Tick
        Try
            Dim LocalMousePosition As Point
            If Common.ProjectLoading Then
                tmrMouseCoords.Interval = 1000
                lblMouseCoords.Visible = True
                lblMouseCoords.Text = String.Format("Load time: {0}s", DateDiff(DateInterval.Second, dProjectLoadStart, Now))
            ElseIf _CurrentHost IsNot Nothing AndAlso Not _CurrentHost.IsDisposed Then
                tmrMouseCoords.Interval = 100
                LocalMousePosition = Common.CurrentScreen.PointToClient(Cursor.Position)
                If LocalMousePosition.X > Common.ProjectWidth Or _
                    LocalMousePosition.Y > Common.ProjectHeight Or _
                    LocalMousePosition.X < 0 Or _
                    LocalMousePosition.Y < 0 Then
                    lblMouseCoords.Visible = False
                Else
                    lblMouseCoords.Text = String.Format("X:{0} Y:{1}", LocalMousePosition.X, LocalMousePosition.Y)
                    lblMouseCoords.Visible = True
                End If
            Else
                tmrMouseCoords.Interval = 500
                lblMouseCoords.Visible = False
            End If
        Catch ex As Exception
        End Try
        ToolStripSaveProject.Enabled = Common.ProjectChanged
    End Sub

    Private Sub BringToFrontToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BringToFrontToolStripMenuItem.Click, SendToBackToolStripMenuItem.Click
        Dim mcs As IMenuCommandService = TryCast(_CurrentHost.DesignerHost.GetService(GetType(IMenuCommandService)), IMenuCommandService)
        If mcs IsNot Nothing Then
            If sender Is BringToFrontToolStripMenuItem Then
                mcs.GlobalInvoke(MenuCommands.BringToFront)
            ElseIf sender Is SendToBackToolStripMenuItem Then
                mcs.GlobalInvoke(MenuCommands.SendToBack)
            End If
        End If
    End Sub

    Private Sub ToolStripMagnify_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMagnify.Click
        If oFrmMagnify Is Nothing Then
            oFrmMagnify = New frmMagnify
            oFrmMagnify.Show()
            Me.Focus()
        Else
            oFrmMagnify.Close()
            oFrmMagnify = Nothing
        End If
    End Sub

    Private Sub tmrUpdateUndo_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrUpdateUndo.Tick
        tmrUpdateUndo.Enabled = False
        If _CurrentHost IsNot Nothing _
            AndAlso _CurrentHost.HostSurface IsNot Nothing _
            AndAlso _CurrentHost.HostSurface.GetMyUndoEngine IsNot Nothing Then
            ToolStripUndo.Enabled = _CurrentHost.HostSurface.GetMyUndoEngine.EnableUndo
            ToolStripRedo.Enabled = _CurrentHost.HostSurface.GetMyUndoEngine.EnableRedo
        End If
    End Sub

    Private Sub ToolStripUndo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripUndo.Click
        If _CurrentHost IsNot Nothing _
            AndAlso _CurrentHost.HostSurface IsNot Nothing _
            AndAlso _CurrentHost.HostSurface.GetMyUndoEngine IsNot Nothing Then
            _CurrentHost.HostSurface.GetMyUndoEngine.Undo()
            tmrUpdateUndo.Enabled = True
        End If
    End Sub

    Private Sub ToolStripRedo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripRedo.Click
        If _CurrentHost IsNot Nothing _
            AndAlso _CurrentHost.HostSurface IsNot Nothing _
            AndAlso _CurrentHost.HostSurface.GetMyUndoEngine IsNot Nothing Then
            _CurrentHost.HostSurface.GetMyUndoEngine.Redo()
            tmrUpdateUndo.Enabled = True
        End If
    End Sub

    Private Sub MenuExploreTemplatesFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuExploreTemplatesFolder.Click
        Dim strLastVersionDir As String = My.Application.Info.Version.ToString
        For Each strDirectory As String In Directory.GetDirectories(Path.Combine(Application.CommonAppDataPath, ".."))
            If Path.GetFileName(strDirectory).Contains(".") AndAlso Path.GetFileName(strDirectory) <> strLastVersionDir Then
                Try
                    Directory.Delete(strDirectory, True)
                Catch ex As Exception
                End Try
            End If
        Next
        Try
            Process.Start(Path.GetDirectoryName(Application.CommonAppDataPath))
        Catch ex As Exception
            MessageBox.Show("Error opening folder " & Path.GetDirectoryName(Application.CommonAppDataPath) & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#Region "MainSplash"

    Private Sub _MainSplash_NewProject() Handles _MainSplash.NewProject
        '_DockPanel1.Contents.RemoveContent(_MainSplash)
        MenuProjectSettings.PerformClick()
        _MainSplash.Hide()
        MenuScreenNew.PerformClick()
    End Sub

    Private Sub _MainSplash_OpenProject() Handles _MainSplash.OpenProject
        MenuOpenProject.PerformClick()
    End Sub

    Private Sub _MainSplash_OpenDemo() Handles _MainSplash.OpenDemo
        Dim dlg As New FolderBrowserDialog
        dlg.Description = "Choose Demos Extraction folder for " & _MainSplash.SelectedDemo
        dlg.ShowNewFolderButton = True
        dlg.RootFolder = Environment.SpecialFolder.MyComputer
        If My.Settings.DemoExtractFolder <> "" Then
            dlg.SelectedPath = My.Settings.DemoExtractFolder
            SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
        End If
        If (dlg.ShowDialog = DialogResult.OK) Then
            My.Settings.DemoExtractFolder = dlg.SelectedPath
            Dim strExportDir As String = dlg.SelectedPath
            If Not ExtractZippedResource(_MainSplash.SelectedDemo & ".zip", strExportDir, System.Reflection.Assembly.GetExecutingAssembly(), Nothing) Then
                MessageBox.Show("Could not extract Demo to " & strExportDir)
                Exit Sub
            End If
            Directory.SetCurrentDirectory(strExportDir)
            OpenProjectDialog(strExportDir)
        End If
    End Sub

    Private Sub MenuShowSplashPage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Common.aScreens.Count > 0 Then
            MessageBox.Show("Initial page will be visible when project will be closed.")
        Else
            _MainSplash.Show(_DockPanel1)
        End If
    End Sub

    Private Sub _MainSplash_OpenRecent() Handles _MainSplash.OpenRecent
        Try
            If _MainSplash.lstRecent Is Nothing OrElse _MainSplash.lstRecent.Items.Count = 0 Then Exit Sub
            Dim strProjectName As String = _MainSplash.lstRecent.SelectedItem
            If strProjectName Is Nothing Then Exit Sub
            If Windows.Forms.Control.ModifierKeys And Keys.Shift Then
                Dim intTimes As Integer = InputBox("Iterations:")
                If intTimes > 0 Then
                    For i As Integer = 1 To intTimes
                        OpenProject(strProjectName)
                        For j As Integer = 1 To 5
                            Application.DoEvents()
                            System.Threading.Thread.Sleep(200)
                        Next j
                        MenuCloseProject.PerformClick()
                        Application.DoEvents()
                    Next
                    Return
                End If
            End If
            OpenProject(strProjectName)

        Catch ex As Exception

        End Try
    End Sub
#End Region

    Private Sub ToolStripCloseProject_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripCloseProject.Click
        MenuCloseProject.PerformClick()
    End Sub

    Private Sub ToolStripGridSize_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripGridSize.TextChanged
        If Not Integer.TryParse(ToolStripGridSize.Text, Nothing) Then
            MessageBox.Show("Please enter an integer between 1 and 255", "Invalid Grid Size value")
        Else
            If _CurrentHost IsNot Nothing Then
                HostSurfaceManager.GridSize = ToolStripGridSize.Text
                _hostSurfaceManager.ActivateDesignerOptions()
                _CurrentHost.Refresh()
            End If
        End If
    End Sub

    Private Sub ToolStripGridOnOff_ButtonClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripGridOnOff.ButtonClick
        HostSurfaceManager.ShowGrid = Not HostSurfaceManager.ShowGrid
        If HostSurfaceManager.ShowGrid Then
            ToolStripGridOnOff.Image = My.Resources.gridOn
            HostSurfaceManager.UseSnapLines = False
            HostSurfaceManager.SnapToGrid = True
        Else
            ToolStripGridOnOff.Image = My.Resources.grid
            HostSurfaceManager.UseSnapLines = True
            HostSurfaceManager.SnapToGrid = False
        End If
        If _CurrentHost IsNot Nothing AndAlso _CurrentHost.DesignerHost IsNot Nothing Then
            _hostSurfaceManager.ActivateDesignerOptions()
            _CurrentHost.Refresh()
        End If
    End Sub

    Private Sub btnDemoHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Common.RunBrowser("http://virtualfab.it/mediawiki/index.php/VGDD:Extract_Demo")
    End Sub

    Public Property MagnifyLockStatus As MagnifyStatuses
        Get
            Return _MagnifyLockStatus
        End Get
        Set(ByVal value As MagnifyStatuses)
            _MagnifyLockStatus = value
            If _CurrentHost IsNot Nothing AndAlso oFrmMagnify IsNot Nothing Then
                Select Case _MagnifyLockStatus
                    Case MagnifyStatuses.Unlocked
                        _CurrentHost.Cursor = Cursors.Default
                        oFrmMagnify.btnLock.Image = Global.My.Resources.Resources.lock_open
                    Case MagnifyStatuses.Locked
                        _CurrentHost.Cursor = Cursors.Default
                        oFrmMagnify.btnLock.Image = Global.My.Resources.Resources.lock
                    Case MagnifyStatuses.Locking
                        _CurrentHost.Cursor = Cursors.UpArrow
                        oFrmMagnify.btnLock.Image = Global.My.Resources.Resources.lock_go
                End Select
            End If
        End Set
    End Property

    Public Sub ScreenTouch(ByVal oScreenAttr As VGDD.VGDDScreenAttr)
        Try
            If oScreenAttr.Hc IsNot Nothing Then
                oScreenAttr.Hc.HostSurface.Flush()
                If oScreenAttr.Hc.HostSurface IsNot Nothing Then
                    oScreenAttr.Hc.Loader.PerformFlushWorker()
                    CType(oScreenAttr.Hc, HostControl).Focus()
                    _CurrentHost = oScreenAttr.Hc
                End If
            End If
            ScreenChanged()
            SendKeys.Send("^a{ESC}")

        Catch ex As Exception

        End Try
        Application.DoEvents()
    End Sub

    Private Sub tmrMigrate_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrMigrate.Tick
        tmrMigrate.Enabled = False
        For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
            'SelectScreen(oScreenAttr.Name)
            oScreenAttr.Screen.Width = Common.ProjectWidth
            oScreenAttr.Screen.Height = Common.ProjectHeight
            MigrateScreen(oScreenAttr.Screen, sngMigrateRatioWidth, sngMigrateRatioHeight)
            ScreenTouch(oScreenAttr)
        Next
        Me.Cursor = Cursors.Default
        Application.DoEvents()
        MessageBox.Show("Warning: Pictures have not been resized but only repositioned and Font sizes are to be manually adjusted.", _
                        "Migration done", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Public Shared Sub MigrateScreen(ByVal oScreen As VGDD.VGDDScreen, ByVal sngRatioWidth As Single, ByVal sngRatioHeight As Single)
        'oScreen.Size = New Size(oScreen.Width * sngRatioWidth, oScreen.Height * sngRatioHeight)
        For Each oControl As Control In oScreen.Controls
            oControl.Location = New Point(oControl.Location.X * sngRatioWidth, oControl.Location.Y * sngRatioHeight)
            If TypeOf oControl Is VGDDMicrochip.VGDDWidgetWithBitmap Then
                Dim oWidget As VGDDMicrochip.VGDDWidgetWithBitmap = oControl
                oWidget.SetSize(New Size(oControl.Width * sngRatioWidth, oControl.Height * sngRatioHeight))
            Else
                oControl.Size = New Size(oControl.Width * sngRatioWidth, oControl.Height * sngRatioHeight)
            End If
            oScreen.Invalidate()
            Application.DoEvents()
            'If oScreen.IsMasterScreen Then
            '    oScreen.ScreenShotUpdate()
            'End If
        Next
    End Sub

#Region "TripleDES"
    Public Class cTripleDES

        ' define the triple des provider
        Private m_des As New TripleDESCryptoServiceProvider

        ' define the string handler
        Private m_utf8 As New UTF8Encoding

        ' define the local property arrays
        Private m_key() As Byte
        Private m_iv() As Byte

        Public Sub New(ByVal key() As Byte, ByVal iv() As Byte)
            Me.m_key = key
            Me.m_iv = iv
        End Sub

        Public Function Encrypt(ByVal input() As Byte) As Byte()
            Return Transform(input, m_des.CreateEncryptor(m_key, m_iv))
        End Function

        Public Function Decrypt(ByVal input() As Byte) As Byte()
            Return Transform(input, m_des.CreateDecryptor(m_key, m_iv))
        End Function

        Public Function Encrypt(ByVal text As String) As String
            Dim input() As Byte = m_utf8.GetBytes(text)
            Dim output() As Byte = Transform(input, _
                            m_des.CreateEncryptor(m_key, m_iv))
            Return Convert.ToBase64String(output)
        End Function

        Public Function Decrypt(ByVal text As String) As String
            Dim input() As Byte = Convert.FromBase64String(text)
            Dim output() As Byte = Transform(input, _
                             m_des.CreateDecryptor(m_key, m_iv))
            Return m_utf8.GetString(output)
        End Function

        Private Function Transform(ByVal input() As Byte, _
            ByVal CryptoTransform As ICryptoTransform) As Byte()
            ' create the necessary streams
            Dim memStream As MemoryStream = New MemoryStream
            Dim cryptStream As CryptoStream = New  _
                CryptoStream(memStream, CryptoTransform, _
                CryptoStreamMode.Write)
            ' transform the bytes as requested
            cryptStream.Write(input, 0, input.Length)
            cryptStream.FlushFinalBlock()
            ' Read the memory stream and convert it back into byte array
            memStream.Position = 0
            Dim result(CType(memStream.Length - 1, System.Int32)) As Byte
            memStream.Read(result, 0, CType(result.Length, System.Int32))
            ' close and release the streams
            memStream.Close()
            cryptStream.Close()
            ' hand back the encrypted buffer
            Return result
        End Function
    End Class
#End Region

    Private Sub pnlWidgetInfo_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)
        _WidgetInfo.Select()
    End Sub

    Private Sub MenuBitmapChooser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuBitmapChooser.Click
        Dim oBitmapChooser As New frmBitmapChooser
        oBitmapChooser.ShowDialog()
        '        Case "Fonts"
        '            Dim ofrmFontChooser As New frmFontChooser
        '            ofrmFontChooser.ShowDialog()
        '    End Select
        'End Sub
    End Sub

    Private Sub ToolStripBitmapChooser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripBitmapChooser.Click
        Dim oFrmBitmapChooser As New frmBitmapChooser
        oFrmBitmapChooser.ShowDialog()
    End Sub

    Private Sub ToolStripFontChooser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripFontChooser.Click
        Dim oFrmFontChooser As New frmFontChooser
        oFrmFontChooser.ShowDialog()
    End Sub

    Private Sub MenuFontChooser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuFontChooser.Click
        Dim oFontChooser As New frmFontChooser
        oFontChooser.ShowDialog()
    End Sub

    Private Sub cmuWidget_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles cmuWidget.ItemClicked
        Select Case e.ClickedItem.Text
            'Case "Resize to fit"
            '    MessageBox.Show("TBI")
            Case Else
                MessageBox.Show("Unhandled context menu " & e.ClickedItem.Text & " for Widget " & cmuWidget.SourceControl.GetType.ToString)
        End Select
    End Sub

    Public Sub OpenAllScreens()
        Try
            Common.ProjectLoading = True
            For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                If FindScreen(oScreenAttr.Name) Is Nothing Then
                    AddTabForNewHost(oScreenAttr.Name, oScreenAttr.Hc, True)
                    'OpenScreen(oScreenAttr.FileName, True)
                End If
            Next
        Catch ex As Exception
        End Try
        Common.ProjectLoading = False
    End Sub

    Public Sub CloseAllScreens()
        Try
            Common.ProjectLoading = True
            _DockPanel1.SuspendLayout()
            For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                Dim oScreen As VGDD.VGDDScreen = oScreenAttr.Screen
                If oScreen IsNot Nothing Then
                    CloseScreen(oScreen.Name, False)
                End If
            Next
            Dim i As Integer = 0
            Do While i < _DockPanel1.Contents.Count
                Dim content As IDockContent = _DockPanel1.Contents(i)
                If TypeOf (content) Is HostControl Then
                    Dim hc As HostControl = content
                    hc.SuspendLayout()
                    Dim oScreen As VGDD.VGDDScreen = hc.Screen
                    If oScreen IsNot Nothing Then
                        If Not CloseScreen(oScreen.Name, True) Then
                            i += 1
                        End If
                    Else
                        i += 1
                    End If
                Else
                    i += 1
                End If
            Loop
            _DockPanel1.ResumeLayout()
        Catch ex As Exception
        End Try
        Common.ProjectLoading = False
    End Sub

#Region "SolutionExlorer"

    Private Sub _SolutionExplorer_FileSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _SolutionExplorer.FileSelected
        Dim oExistingContent As HostControl = FindScreen(_SolutionExplorer.FileSelected_FileName)
        If oExistingContent IsNot Nothing Then
            oExistingContent.Focus()
            ActiveHCChanged(oExistingContent)
            Exit Sub
        End If
        For Each oScreenattr As VGDD.VGDDScreenAttr In Common.aScreens.Values
            If oScreenattr.FileName = _SolutionExplorer.FileSelected_FileName Then
                AddTabForNewHost(oScreenattr.Name, oScreenattr.Hc, True)
                oScreenattr.Shown = True
                Common.ProjectChanged = True
                Exit For
            End If
        Next
        '        OpenScreen(_SolutionExplorer.FileSelected_FileName, True)
    End Sub

    Private Sub _SolutionExplorer_OpenAllScreens(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SolutionExplorer.OpenAllScreens
        OpenAllScreens()
    End Sub

    Private Sub _SolutionExplorer_CloseAllScreens(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SolutionExplorer.CloseAllScreens
        Common.ProjectLoading = True
        Try
            _DockPanel1.SuspendLayout()
            For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                Dim oScreen As VGDD.VGDDScreen = oScreenAttr.Screen
                oScreenAttr.Hc.hide()
                If oScreen IsNot Nothing Then
                    CloseScreen(oScreen.Name, False)
                End If
            Next
            _DockPanel1.ResumeLayout()
        Catch ex As Exception
        End Try
        Common.ProjectLoading = False
    End Sub

    Private Sub _SolutionExplorer_RemoveScreen(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SolutionExplorer.RemoveScreen
        Me.Cursor = Cursors.WaitCursor
        Dim strScreenToRemove As String = CType(sender, TreeNode).Text
        CloseScreen(strScreenToRemove, True)
        RemoveScreenFromProject(strScreenToRemove)
        tmrCheckMiniature.Enabled = True
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub _SolutionExplorer_NewScreen(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SolutionExplorer.NewScreen
        ToolStripNewScreen.PerformClick()
    End Sub

    Private Sub _SolutionExplorer_DuplicateScreen(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SolutionExplorer.DuplicateScreen
        Try
            'yyy()

            Dim hc As HostControl = FindScreen(_SolutionExplorer.FileSelected_FileName)
            If hc IsNot Nothing Then
                Dim strNewName As String = Path.GetFileNameWithoutExtension(hc.Name) & "_copy"
                Dim i As Integer = 1
                Do While Common.aScreens.ContainsKey(strNewName.ToUpper)
                    strNewName = Path.GetFileNameWithoutExtension(hc.Name) & "_copy" & i.ToString
                    i += 1
                Loop
                strNewName = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), strNewName & ".vds")
                Dim strSaveFileName As String = hc.HostSurface.Loader.FileName
                hc.HostSurface.Loader.FileName = strNewName
                hc.HostSurface.Loader.Save(strNewName, False, True)

                hc.HostSurface.Loader.FileName = strSaveFileName
                Common.aScreens.Add(strNewName, True)
                OpenScreen(strNewName, True)
                hc = FindScreen(strNewName)
                hc.HostSurface.Loader.FileName = Nothing
                hc.Text &= "*"
                _CurrentHost = Nothing
                If TypeOf (_DockPanel1.ActivePane.ActiveContent) Is HostControl Then
                    ActiveHCChanged(_DockPanel1.ActivePane.ActiveContent)
                Else
                    ActiveHCChanged(hc)
                End If
                ProjectChanged()
            End If

        Catch ex As System.Exception
            MessageBox.Show("Error in duplicating screen", "Shell Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

    Private Sub MenuPreferences_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuPreferences.Click
        frmPreferences.ShowDialog()
    End Sub

    Private Sub MainShell_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me._DockPanel1.Width = Me.ClientSize.Width
        Me._DockPanel1.Height = Me.ClientSize.Height - StatusStrip1.Height - ToolStrip1.Height
    End Sub

#Region "Window Layout"
    Private Sub MenuLayoutSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuLayoutSave.Click
        _DockPanel1.SaveAsXml(LayoutFileName)
    End Sub

    Private Sub MenuLayoutSaveAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuLayoutSaveAs.Click
        Dim dlg As SaveFileDialog = New SaveFileDialog
        dlg.Title = "Save Window Layout"
        dlg.DefaultExt = "vwl"
        dlg.Filter = "VGDD Window Layout Files|*.vwl"
        dlg.FileName = Path.GetFileName(LayoutFileName)
        dlg.InitialDirectory = Path.GetDirectoryName(Application.CommonAppDataPath)
        If (dlg.ShowDialog = DialogResult.OK) Then
            LayoutFileName = dlg.FileName
            _DockPanel1.SaveAsXml(LayoutFileName)
        End If
    End Sub

    Private Sub MenuLayoutLoad_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuLayoutLoad.Click
        Dim dlg As OpenFileDialog = New OpenFileDialog
        dlg.Title = "Open Window Layout"
        dlg.DefaultExt = "vwl"
        dlg.Filter = "VGDD Window Layout Files|*.vwl"
        dlg.FileName = Path.GetFileName(LayoutFileName)
        dlg.InitialDirectory = Path.GetDirectoryName(LayoutFileName)
        If dlg.InitialDirectory = "" Then
            dlg.InitialDirectory = Path.GetDirectoryName(Application.CommonAppDataPath)
        End If
        If (dlg.ShowDialog = DialogResult.OK) Then
            LayoutFileName = dlg.FileName
            '_DockPanel1.Contents.RemoveAll()
            DockLoadLayout()
        End If
        _DockPanel1.SaveAsXml(LayoutFileName)
    End Sub

    Private Sub DockLayoutHideSingle(ByVal window As WeifenLuo.WinFormsUI.Docking.DockContent, ByVal title As String)
        Static blnWarning As Boolean
        Try
            window.Hide()
        Catch ex As Exception
            If Not blnWarning Then
                MessageBox.Show("Cannot hide " & title & " window. Please close and re-open VGDD.", "Warning")
                blnWarning = True
            End If
        End Try
    End Sub

    Private Sub DockLayoutHideAll()
        DockLayoutHideSingle(_Toolbox, "Toolbox")
        DockLayoutHideSingle(_WidgetPropertyGrid, "WidgetPropertyGrid")
        DockLayoutHideSingle(_SchemesChooser, "SchemesChooser")
        DockLayoutHideSingle(_EventTree, "EventTree")
        DockLayoutHideSingle(_SolutionExplorer, "SolutionExplorer")
        DockLayoutHideSingle(_OutputWindow, "OutputWindow")
        DockLayoutHideSingle(_WidgetInfo, "WidgetInfo")
        DockLayoutHideSingle(_ProjectSummary, "ProjectSummary")
        DockLayoutHideSingle(_EventsEditor, "EventsEditor")
        DockLayoutHideSingle(_HTMLEditor, "HTMLEditor")
    End Sub

    Private Sub DockLayoutUnhideAll()
        _Toolbox.Show()
        _WidgetPropertyGrid.Show()
        _SchemesChooser.Show()
        _EventTree.Show()
        _SolutionExplorer.Show()
        _OutputWindow.Show()
        _WidgetInfo.Show()
        _ProjectSummary.Show()
        _EventsEditor.Show()
        _HTMLEditor.Show()
        Application.DoEvents()
    End Sub

    Private Sub DockLoadLayout()
        Dim strCrashFlagFile As String = Path.Combine(Application.LocalUserAppDataPath, "VGDDCrash.flg")
        If File.Exists(strCrashFlagFile) Then
            Try
                File.Delete(strCrashFlagFile)
            Catch ex As Exception
            End Try
            DockLayoutReset()
            _DockPanel1.SaveAsXml(LayoutFileName)
        Else
            Try
                Dim oCrashFile As FileStream = File.Create(strCrashFlagFile)
                oCrashFile.Close()
                With _DockPanel1
                    '.Contents.RemoveAll()
                    .Left = 0
                    .Top = ToolStrip1.Height
                    .Width = Me.Width
                    .Height = Me.Height - .Top
                End With
                If File.Exists(LayoutFileName) Then
                    Try
                        Dim oDockContentFromPersistString As New DeserializeDockContent(AddressOf DockContentFromPersistString)
                        _DockPanel1.LoadFromXml(LayoutFileName, oDockContentFromPersistString)
                    Catch ex As Exception
                        File.Delete(LayoutFileName)
                        DockLayoutReset()
                    End Try
                Else
                    DockLayoutReset()
                    'If _DockPanel1.Panes.Count = 0 Then
                    'End If
                End If
                File.Delete(strCrashFlagFile)
            Catch ex As Exception
                DockLayoutReset()
            End Try
        End If
    End Sub

    Private Function DockContentFromPersistString(ByVal persistString As String) As IDockContent
        If persistString = GetType(VGDDIDE.MainSplash).ToString() Then
            Return _MainSplash
        ElseIf persistString = GetType(VGDDIDE.CollapseToolbox).ToString() Then
            Return _Toolbox
        ElseIf persistString = GetType(VGDDIDE.WidgetPropertyGrid).ToString() Then
            Return _WidgetPropertyGrid
        ElseIf persistString = GetType(VGDDIDE.SchemesChooser).ToString() Then
            Return _SchemesChooser
        ElseIf persistString = GetType(VGDDIDE.EventsTree).ToString() Then
            Return _EventTree
        ElseIf persistString = GetType(VGDDIDE.SolutionExplorer).ToString() Then
            Return _SolutionExplorer
        ElseIf persistString = GetType(VGDDIDE.WidgetInfo).ToString() Then
            Return _WidgetInfo
        ElseIf persistString = GetType(VGDDIDE.ProjectSummary).ToString() Then
            Return _ProjectSummary
        ElseIf persistString = GetType(VGDDIDE.OutputWindow).ToString() Then
            Return _OutputWindow
        ElseIf persistString = GetType(VGDDIDE.HTMLEditor).ToString() Then
            Return _HTMLEditor
        ElseIf persistString = GetType(VGDDIDE.EventsEditor).ToString() Then
            Return _EventsEditor
        ElseIf persistString.StartsWith("HostControl") Then
            Try
                Dim strScreenName As String = Path.GetFileNameWithoutExtension(persistString.Split(",")(1))
                'If Common.aScreens.Contains(strScreenName) Then
                Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(strScreenName)
                If oScreenAttr IsNot Nothing Then
                    Dim hc As HostControl
                    'If oScreenAttr.Hc Is Nothing Then
                    '    hc = _hostSurfaceManager.GetNewHost(oScreenAttr.FileName)
                    'Else
                    hc = oScreenAttr.Hc
                    'End If
                    AddTabForNewHost(oScreenAttr.Name, hc, False)
                    Return hc
                    'End If
                End If
            Catch ex As Exception
            End Try
            'OpenScreen(Common.aScreens.Item(0).FileName, True, False)

            'Else
            '    ' DummyDoc overrides GetPersistString to add extra information into persistString.
            '    ' Any DockContent may override this value to add any needed information for deserialization.

            '    Dim parsedStrings As String() = persistString.Split(New Char() {","c})
            '    If parsedStrings.Length <> 3 Then
            '        Return Nothing
            '    End If

            '    If parsedStrings(0) <> GetType(DummyDoc).ToString() Then
            '        Return Nothing
            '    End If

            '    Dim dummyDoc As New DummyDoc()
            '    If parsedStrings(1) <> String.Empty Then
            '        dummyDoc.FileName = parsedStrings(1)
            '    End If
            '    If parsedStrings(2) <> String.Empty Then
            '        dummyDoc.Text = parsedStrings(2)
            '    End If

            '    Return dummyDoc
        Else
            Debug.Print(persistString)
        End If
        Return Nothing
    End Function

    Private Sub MenuResetWindows_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuLayoutResetWindows.Click
        DockLayoutReset()
    End Sub

    Private Sub DockCheck()
        Try
            If _HTMLEditor Is Nothing Then
                _HTMLEditor = New VGDDIDE.HTMLEditor
            End If
            If _HTMLEditor.DockState = DockState.Unknown Then
                _HTMLEditor.ShowHint = DockState.DockBottomAutoHide
                _HTMLEditor.Show(_DockPanel1)
                _HTMLEditor.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)
                _HTMLEditor.DockState = DockState.DockBottomAutoHide
            End If
            If _EventsEditor.DockState = DockState.Unknown Then
                _EventsEditor.ShowHint = DockState.DockBottomAutoHide
                _EventsEditor.Show(_DockPanel1)
                _EventsEditor.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)
                _EventsEditor.DockState = DockState.DockBottomAutoHide
            End If
            If Common.aScreens.Count > 0 Then
                If MenuViewEvents.Checked Then
                    _EventTree.DockState = DockState.DockRight
                    _EventTree.Show()
                Else
                    _EventTree.Hide()
                End If
                If MenuViewEventsEditor.Checked Then
                    _EventsEditor.DockState = DockState.DockBottom
                    _EventsEditor.Show()
                Else
                    _EventsEditor.Hide()
                End If
                If MenuViewHtmlEditor.Checked Then
                    _HTMLEditor.DockState = DockState.DockBottom
                    _HTMLEditor.Show()
                Else
                    _HTMLEditor.Hide()
                End If
                If MenuViewOutputWindow.Checked Then
                    _OutputWindow.DockState = DockState.DockBottom
                    _OutputWindow.Show()
                Else
                    _OutputWindow.Hide()
                End If
                If MenuViewProjectExplorer.Checked Then
                    _SolutionExplorer.DockState = DockState.DockBottom
                    _SolutionExplorer.Show()
                Else
                    _SolutionExplorer.Hide()
                End If
                If MenuViewProjectSummary.Checked Then
                    _ProjectSummary.DockState = DockState.DockBottom
                    _ProjectSummary.Show()
                Else
                    _ProjectSummary.Hide()
                End If
                If MenuViewSchemes.Checked Then
                    _SchemesChooser.DockState = DockState.DockRight
                    _SchemesChooser.Show()
                Else
                    _SchemesChooser.Hide()
                End If
                If MenuViewToolBox.Checked Then
                    _Toolbox.DockState = DockState.DockLeft
                    _Toolbox.Show()
                Else
                    _Toolbox.Hide()
                End If
                If MenuViewWidgetInfo.Checked Then
                    _WidgetInfo.DockState = DockState.DockBottom
                    _WidgetInfo.Show()
                Else
                    _WidgetInfo.Hide()
                End If
                If MenuViewWidgetProperties.Checked Then
                    _WidgetPropertyGrid.DockState = DockState.DockRight
                    _WidgetPropertyGrid.Show()
                Else
                    _WidgetPropertyGrid.Hide()
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub DockLayoutReset()
        Try
            '_DockPanel1.Dispose()
            'If _Toolbox Is Nothing OrElse _Toolbox.IsDisposed Then
            '    _Toolbox = New VGDDIDE.CollapseToolbox()
            'End If
            'If _MainSplash Is Nothing OrElse _MainSplash.IsDisposed Then
            '    _MainSplash = New VGDDIDE.MainSplash()
            'End If
            'If _WidgetPropertyGrid Is Nothing OrElse _WidgetPropertyGrid.IsDisposed Then
            '    _WidgetPropertyGrid = New VGDDIDE.WidgetPropertyGrid()
            'End If
            'If _SchemesChooser Is Nothing OrElse _SchemesChooser.IsDisposed Then
            '    _SchemesChooser = New VGDDIDE.SchemesChooser()
            'End If
            'If _EventTree Is Nothing OrElse _EventTree.IsDisposed Then
            '    _EventTree = New VGDDIDE.EventsTree()
            'End If
            'If _SolutionExplorer Is Nothing OrElse _SolutionExplorer.IsDisposed Then
            '    _SolutionExplorer = New VGDDIDE.SolutionExplorer()
            'End If
            'If _OutputWindow Is Nothing OrElse _OutputWindow.IsDisposed Then
            '    _OutputWindow = New VGDDIDE.OutputWindow()
            'End If
            'If _WidgetInfo Is Nothing OrElse _WidgetInfo.IsDisposed Then
            '    _WidgetInfo = New VGDDIDE.WidgetInfo()
            'End If
            'If _ProjectSummary Is Nothing OrElse _ProjectSummary.IsDisposed Then
            '    _ProjectSummary = New VGDDIDE.ProjectSummary()
            'End If
            'If _EventsEditor Is Nothing OrElse _EventsEditor.IsDisposed Then
            '    _EventsEditor = New VGDDIDE.EventsEditor()
            'End If
            'If _HTMLEditor Is Nothing OrElse _HTMLEditor.IsDisposed Then
            '    _HTMLEditor = New VGDDIDE.HTMLEditor()
            'End If

            '_DockPanel1 = New DockPanel
            With _DockPanel1
                .DocumentStyle = DocumentStyle.DockingSdi
                '.HoverTimeout = 3200
                .Left = 0
                .Top = ToolStrip1.Height
                .Width = Me.Width
                .Height = Me.Height - .Top - StatusStrip1.Height - 20
                .ShowDocumentIcon = True
                '.Contents.RemoveAll()
                '.Contents.RemoveContent(_WidgetPropertyGrid)
                '.Contents.RemoveContent(_SchemesChooser)
                '.Contents.RemoveContent(_EventTree)
                '.Contents.RemoveContent(_ProjectSummary)
                '.Contents.RemoveContent(_WidgetInfo)
                '.Contents.RemoveContent(_OutputWindow)
                '.Contents.RemoveContent(_SolutionExplorer)
                '.Contents.RemoveContent(_HTMLEditor)
                '.Contents.RemoveContent(_EventsEditor)
                '.Contents.RemoveContent(_Toolbox)

                'For Each oContent As DockContent In .Contents
                '    oContent.ShowHint = DockState.Document
                '    'oContent.VisibleState = DockState.Document
                '    'oContent.DockHandler.VisibleState = DockState.Document
                '    oContent.Dock = DockStyle.None
                'Next
            End With

            _WidgetPropertyGrid.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
            _SchemesChooser.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
            _EventTree.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
            _ProjectSummary.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _WidgetInfo.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _OutputWindow.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _SolutionExplorer.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _HTMLEditor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _EventsEditor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _Toolbox.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft

            If Common.ProjectFileName = "New" Then
                _MainSplash.Show(_DockPanel1)
            End If
            _Toolbox.Show(_DockPanel1, DockState.DockRight)
            _WidgetPropertyGrid.Show(_DockPanel1, DockState.DockRight)
            _SchemesChooser.Show(_DockPanel1, DockState.DockRightAutoHide)
            _EventTree.Show(_DockPanel1, DockState.DockRightAutoHide)
            _SolutionExplorer.Show(_DockPanel1, DockState.DockBottom)
            _OutputWindow.Show(_DockPanel1, DockState.DockBottom)
            _WidgetInfo.Show(_DockPanel1, DockState.DockBottom)
            _ProjectSummary.Show(_DockPanel1, DockState.DockBottom)
            _EventsEditor.Show(_DockPanel1, DockState.DockBottom)
            _HTMLEditor.Show(_DockPanel1, DockState.DockBottom)

            DockCheck()

            _SolutionExplorer.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)
            _OutputWindow.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)
            _WidgetInfo.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)
            _ProjectSummary.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)
            _EventsEditor.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)
            _HTMLEditor.DockHandler.DockTo(_DockPanel1, DockStyle.Bottom)

            _WidgetPropertyGrid.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
            _SchemesChooser.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
            _EventTree.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
            _ProjectSummary.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _WidgetInfo.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _OutputWindow.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _HTMLEditor.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _EventsEditor.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _SolutionExplorer.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            _Toolbox.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft

            _WidgetPropertyGrid.DockHandler.DockTo(_SchemesChooser.Pane, DockStyle.Left, 0)
            _EventTree.DockHandler.DockTo(_SchemesChooser.Pane, DockStyle.Top, 0)

            _DockPanel1.DockLeftPortion = 0.12
            _DockPanel1.DockRightPortion = 0.35

            If Common.ProjectFileName <> "New" Then
                '_DockPanel1.Contents.RemoveContent(_MainSplash)
                'For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                '    OpenScreen(oScreenAttr.FileName, True)
                'Next
            Else
                '_MainSplash.Show(_DockPanel1)
            End If

        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
        SuspendLayout()
        WindowState = FormWindowState.Normal
        'ClientSize = MinimumSize
        My.Settings.MyLeft = Screen.PrimaryScreen.Bounds.Width * 0.05
        My.Settings.MyTop = Screen.PrimaryScreen.Bounds.Height * 0.05
        My.Settings.MyWidth = Screen.PrimaryScreen.Bounds.Width * 0.9 '797 'Width
        My.Settings.MyHeight = Screen.PrimaryScreen.Bounds.Height * 0.9 '614 ' Height
        ResumeLayout()
        ResizeMe()
        OpenAllScreens()
        'ProjectChanged()
        'WindowState = FormWindowState.Maximized
    End Sub
#End Region

    Private Sub _CurrentHost_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles _CurrentHost.FormClosing
        If Not CheckClosingScreen(sender, MessageBoxButtons.YesNoCancel) Then
            e.Cancel = True
        ElseIf Common.CurrentScreen IsNot Nothing AndAlso Common.CurrentScreen.FileName = "" Then
            RemoveScreenFromProject(Common.CurrentScreen.Name)
        End If
    End Sub

    Private Sub _CurrentHost_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _CurrentHost.DockStateChanged
        If _CurrentHost.DockState = DockState.Hidden Then
            CheckClosingScreen(sender, MessageBoxButtons.YesNo)
            Dim Hc As HostControl = sender
            Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(Hc.Name)
            If oScreenAttr IsNot Nothing Then
                oScreenAttr.Shown = False
                Common.ProjectChanged = True
            End If
        End If
    End Sub

    Private Sub _DockPanel1_ContentRemoved(ByVal sender As Object, ByVal e As WeifenLuo.WinFormsUI.Docking.DockContentEventArgs) Handles _DockPanel1.ContentRemoved
        If TypeOf (e.Content) Is HostControl Then
            If Not _WidgetPropertyGrid.IsDisposed Then _WidgetPropertyGrid.PropertyGrid.SelectedObject = Nothing
            Dim hc As HostControl = e.Content
            Dim strScreenName As String = hc.Name
            If strScreenName <> "HostControl" Then
                CloseScreen(strScreenName, True)
            End If
            'If strScreenName = "" Then
            '    RemoveScreenFromProject(Common.CurrentScreen.Name)
            'End If
        End If
    End Sub

    Private Sub MenuView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuViewEvents.Click,
            MenuViewEventsEditor.Click,
            MenuViewHtmlEditor.Click,
            MenuViewOutputWindow.Click,
            MenuViewProjectExplorer.Click,
            MenuViewProjectSummary.Click,
            MenuViewSchemes.Click,
            MenuViewToolBox.Click,
            MenuViewWidgetInfo.Click,
            MenuViewWidgetProperties.Click
        Dim oMenuItem As MenuItem = sender
        oMenuItem.Checked = Not oMenuItem.Checked
        DockCheck()
    End Sub

    Private Sub MenuExportScreenshots_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuExportScreenshots.Click
        Const BORDER As Integer = 0
        If Common.ProjectName = "New" Then Exit Sub
        Dim oFolderDialog As New FolderBrowserDialog()
        With oFolderDialog
            .SelectedPath = Common.VGDDProjectPath
            .Description = "Choose export folder for ScreenShots"
            Application.DoEvents()
            SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
            If .ShowDialog() = DialogResult.OK Then
                Try
                    For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                        Using oScreenShot As Bitmap = Common.Render2Bitmap(oScreenAttr.Screen, oScreenAttr.Screen.Width, oScreenAttr.Screen.Height)
                            Using oScreenShotToSave As New Bitmap(oScreenAttr.Screen.Width + BORDER * 2, oScreenAttr.Screen.Height + BORDER * 2)
                                Using g As Graphics = Graphics.FromImage(oScreenShotToSave)
                                    g.DrawRectangle(Pens.Black, 0, 0, oScreenShotToSave.Width - 1, oScreenShotToSave.Height - 1)
                                    g.DrawImageUnscaled(oScreenShot, BORDER, BORDER, oScreenShot.Width, oScreenShot.Height)
                                End Using
                                oScreenShotToSave.Save(Path.Combine(.SelectedPath, oScreenAttr.Screen.Name & ".png"), Imaging.ImageFormat.Png)
                            End Using
                        End Using
                    Next
                    MessageBox.Show(Common.aScreens.Count & " screenshots succesfully created in " & .SelectedPath, "Operation completed")
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error export screenshots")
                End Try
            End If
        End With
    End Sub

    Private Sub MenuExportScheme_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuExportScheme.Click
        _SchemesChooser.ExportScheme(_SchemesChooser._SchemePropertyGrid.SelectedObject)
    End Sub

    Private Sub MenuImportScheme_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuImportScheme.Click
        _SchemesChooser.ImportScheme()
    End Sub

    Private Sub MenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuSaveProjectN.Click
        SaveProject()
    End Sub

    Private Sub MenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuSaveProjectAsN.Click
        If SaveProjectAs() Then
            ClearProject()
            OpenProject(strLastSavedProject)
        End If
    End Sub

    Public Shared Function VGDDGetType(ByVal strType As String) As Type
        Dim oType As Type
        oType = oMainShell.ExternalWidgetsGetLibraryType(strType.Split(",")(0), False)
        If oType Is Nothing Then
            oType = System.Type.GetType(strType)
        End If
        If oType Is Nothing Then
            oType = System.Type.GetType(strType.Split(",")(0) & ",VGDDCommonEmbedded")
        End If
        If oType Is Nothing Then
            oType = System.Type.GetType("VGDDMicrochip." & strType.Split(",")(0).Split(".")(1) & ",VGDDCommonEmbedded")
        End If
        Return oType
    End Function

    Private Sub ToolStripdrpLanguage_DropDownItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ToolStripdrpLanguage.DropDownItemClicked
        Dim intIndex As Integer = ToolStripdrpLanguage.DropDownItems.IndexOf(e.ClickedItem)
        If intIndex = 0 Then
            Dim oSp As New StringsPool
            oSp.SelectOnly = False
            oSp.ShowDialog()
        Else
            Common.ProjectActiveLanguage = intIndex - 1
            _CurrentHost.Refresh()
        End If
    End Sub

    Private Sub _SolutionExplorer_TouchAllScreens(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SolutionExplorer.TouchAllScreens
        If MessageBox.Show("Do you want to ""touch"" every ascreen in the project, so they all get saved at next save?", "Confirm touch", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            Me.Cursor = Cursors.WaitCursor
            Try
                For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                    VGDDCommon.Common.TouchScreen(oScreenAttr.Name)
                Next
            Catch ex As Exception
            End Try
            Me.Cursor = Cursors.Default
        End If
    End Sub

#Region "Docking"

    Private Sub _SchemesChooser_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles _SchemesChooser.FormClosing
        e.Cancel = True
        MenuViewSchemes.PerformClick()
    End Sub

    Private Sub _OutputWindow_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles _OutputWindow.FormClosing
        e.Cancel = True
        _OutputWindow.DockState = DockState.Hidden
    End Sub

    Private Sub _OutputWindow_DockStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _OutputWindow.DockStateChanged
        Try
            If _OutputWindow.DockState <> DockState.Unknown Then
                MenuViewOutputWindow.Checked = Not _OutputWindow.DockState = DockState.Hidden
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub _HTMLEditor_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles _HTMLEditor.FormClosing
        e.Cancel = True
        _HTMLEditor.DockState = DockState.Hidden
    End Sub

    Private Sub _EventTree_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles _EventTree.FormClosing
        e.Cancel = True
        _EventTree.DockState = DockState.Hidden
    End Sub

    Private Sub _SolutionExplorer_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles _SolutionExplorer.FormClosing
        e.Cancel = True
        _SolutionExplorer.DockState = DockState.Hidden
    End Sub

#End Region

End Class

Public Class KeystrokMessageFilter
    Implements System.Windows.Forms.IMessageFilter

    Private host As IDesignerHost
    Private _DesignSurface As VGDDHostSurface
    'Public AllowDeleteKey As Boolean = True
    'Public Event Deleted()

    Public Sub New()
    End Sub

    Public Sub SetHostAndMDIForm(ByVal host As IDesignerHost, ByVal DesSurface As VGDDHostSurface)
        Me.host = host
        _DesignSurface = DesSurface
    End Sub

#Region "Implementation of IMessageFilter"


    '<System.Diagnostics.DebuggerStepThrough()> _
    Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
        If _DesignSurface Is Nothing Then
            Return False
        End If
        'return false;
        ' Catch WM_KEYCHAR if the designerView has focus
        'System.Diagnostics.Trace.WriteLine(m.Msg.ToString());
        'only send these messages out if the design surface has the focus
        '0x0100
        Dim oControl As Control = Nothing
        Try
            If Not _DesignSurface.IsDisposed AndAlso _DesignSurface.IsLoaded Then
                oControl = TryCast(_DesignSurface.View, Control)
            End If
        Catch ex As Exception
        End Try
        If (m.Msg = 256) AndAlso oControl IsNot Nothing AndAlso oControl.Focused Then
            Dim mcs As IMenuCommandService = TryCast(host.GetService(GetType(IMenuCommandService)), IMenuCommandService)
            ' WM_KEYCHAR only tells us the last key pressed. Thus we check
            ' Control for modifier keys (Control, Shift, etc.)
            '
            Select Case CInt(m.WParam) Or CInt(Control.ModifierKeys)

                Case CInt(Keys.Delete)
                    mcs.GlobalInvoke(MenuCommands.Delete)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Insert),
                     CInt(Keys.Control Or Keys.C)
                    mcs.GlobalInvoke(MenuCommands.Copy)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Insert),
                    CInt(Keys.Control Or Keys.V)
                    mcs.GlobalInvoke(MenuCommands.Paste)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Delete),
                    CInt(Keys.Control Or Keys.X)
                    mcs.GlobalInvoke(MenuCommands.Cut)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Z)
                    _DesignSurface.GetMyUndoEngine.Undo()
                    'MainShell.tmrUpdateUndo.Enabled = True

                    'mcs.GlobalInvoke(MenuCommands.Undo)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Control Or Keys.Z)
                    _DesignSurface.GetMyUndoEngine.Redo()
                    'mcs.GlobalInvoke(MenuCommands.Redo)
                    Exit Select

                Case CInt(Keys.Control Or Keys.G)
                    mcs.GlobalInvoke(MenuCommands.Group)
                    Exit Select

                Case CInt(Keys.Control Or Keys.R)
                    mcs.GlobalInvoke(MenuCommands.ShowGrid)
                    Exit Select

                Case CInt(Keys.Control Or Keys.A)
                    mcs.GlobalInvoke(MenuCommands.SelectAll)
                    Exit Select

                Case CInt(Keys.Control Or Keys.F)
                    mcs.GlobalInvoke(MenuCommands.BringToFront)
                    Exit Select

                Case CInt(Keys.Control Or Keys.B)
                    mcs.GlobalInvoke(MenuCommands.SendToBack)
                    Exit Select

                Case CInt(Keys.Up)
                    mcs.GlobalInvoke(MenuCommands.KeyMoveUp)
                    Exit Select

                Case CInt(Keys.Down)
                    mcs.GlobalInvoke(MenuCommands.KeyMoveDown)
                    Exit Select

                Case CInt(Keys.Right)
                    mcs.GlobalInvoke(MenuCommands.KeyMoveRight)
                    Exit Select

                Case CInt(Keys.Left)
                    mcs.GlobalInvoke(MenuCommands.KeyMoveLeft)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Up)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeUp)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Down)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeDown)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Right)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeRight)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Left)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeLeft)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Up)
                    mcs.GlobalInvoke(MenuCommands.KeySizeHeightDecrease)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Down)
                    mcs.GlobalInvoke(MenuCommands.KeySizeHeightIncrease)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Right)
                    mcs.GlobalInvoke(MenuCommands.KeySizeWidthIncrease)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Left)
                    mcs.GlobalInvoke(MenuCommands.KeySizeWidthDecrease)
                    Exit Select
                Case CInt(Keys.Control Or Keys.Shift Or Keys.Up)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeHeightDecrease)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Shift Or Keys.Down)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeHeightIncrease)
                    Exit Select

                Case CInt(Keys.Control Or Keys.Shift Or Keys.Right)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeWidthIncrease)
                    Exit Select

                Case CInt(Keys.ControlKey Or Keys.Shift Or Keys.Left)
                    mcs.GlobalInvoke(MenuCommands.KeyNudgeWidthDecrease)
                    Exit Select

                Case CInt(Keys.Escape)
                    mcs.GlobalInvoke(MenuCommands.KeyCancel)
                    Exit Select

                Case CInt(Keys.Shift Or Keys.Escape)
                    mcs.GlobalInvoke(MenuCommands.KeyReverseCancel)
                    Exit Select

                Case CInt(Keys.Enter)
                    mcs.GlobalInvoke(MenuCommands.KeyDefaultAction)
                    Exit Select

                    'Case CInt(Keys.Shift Or Keys.Delete)
                    '    If AllowDeleteKey Then
                    '        mcs.GlobalInvoke(MenuCommands.Delete)
                    '        RaiseEvent Deleted()
                    '    End If
                    'Exit Select

                Case CInt(Keys.Control Or Keys.S)
                    mcs.GlobalInvoke(MenuCommands.SizeToControl)
                    Exit Select

                    'Case CInt(Keys.Control Or Keys.T)
                    '    mcs.GlobalInvoke(MenuCommands.AlignTop)
                    '    Exit Select

                    'Case CInt(Keys.Control Or Keys.B)
                    '    mcs.GlobalInvoke(MenuCommands.AlignBottom)
                    '    Exit Select

            End Select
        End If
        Return False
    End Function

#End Region
End Class


