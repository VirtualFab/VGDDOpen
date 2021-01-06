'Namespace Shell


Partial Public Class MainShell

    ' <summary>
    ' Required designer variable.
    ' </summary>
    Private components As System.ComponentModel.IContainer = Nothing
    Friend WithEvents mainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents MenuExit As System.Windows.Forms.MenuItem
    Friend WithEvents MenuCut As System.Windows.Forms.MenuItem
    Friend WithEvents MenuCopy As System.Windows.Forms.MenuItem
    Friend WithEvents MenuPaste As System.Windows.Forms.MenuItem
    Friend WithEvents MenuDelete As System.Windows.Forms.MenuItem
    Friend WithEvents MenuSelectAll As System.Windows.Forms.MenuItem
    Friend WithEvents MenuAlignLefts As System.Windows.Forms.MenuItem
    Friend WithEvents MenuAlignCenters As System.Windows.Forms.MenuItem
    Friend WithEvents MenuAlignRights As System.Windows.Forms.MenuItem
    Friend WithEvents MenuAlignTops As System.Windows.Forms.MenuItem
    Friend WithEvents MenuAlignMiddles As System.Windows.Forms.MenuItem
    Friend WithEvents MenuAlignBottoms As System.Windows.Forms.MenuItem
    Friend WithEvents MenuProjectSettings As System.Windows.Forms.MenuItem

    ' <summary>
    ' Clean up any resources being used.
    ' </summary>
    ' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If (disposing _
                    AndAlso (Not (components) Is Nothing)) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        Catch ex As Exception
        End Try
    End Sub

    ' <summary>
    ' Required method for Designer support - do not modify
    ' the contents of this method with the code editor.
    ' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim MenuAdd As System.Windows.Forms.MenuItem
        Dim MenuFile As System.Windows.Forms.MenuItem
        Dim MenuExport As System.Windows.Forms.MenuItem
        Dim MenuAlign As System.Windows.Forms.MenuItem
        Dim MenuOrder As System.Windows.Forms.MenuItem
        Dim MenuTools As System.Windows.Forms.MenuItem
        Dim DockPanelSkin1 As WeifenLuo.WinFormsUI.Docking.DockPanelSkin = New WeifenLuo.WinFormsUI.Docking.DockPanelSkin()
        Dim AutoHideStripSkin1 As WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin = New WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin()
        Dim DockPanelGradient1 As WeifenLuo.WinFormsUI.Docking.DockPanelGradient = New WeifenLuo.WinFormsUI.Docking.DockPanelGradient()
        Dim TabGradient1 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPaneStripSkin1 As WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin = New WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin()
        Dim DockPaneStripGradient1 As WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient = New WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient()
        Dim TabGradient2 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPanelGradient2 As WeifenLuo.WinFormsUI.Docking.DockPanelGradient = New WeifenLuo.WinFormsUI.Docking.DockPanelGradient()
        Dim TabGradient3 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPaneStripToolWindowGradient1 As WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient = New WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient()
        Dim TabGradient4 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim TabGradient5 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPanelGradient3 As WeifenLuo.WinFormsUI.Docking.DockPanelGradient = New WeifenLuo.WinFormsUI.Docking.DockPanelGradient()
        Dim TabGradient6 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim TabGradient7 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainShell))
        Me.MenuScreenNew = New System.Windows.Forms.MenuItem()
        Me.MenuScreenAddExisting = New System.Windows.Forms.MenuItem()
        Me.MenuImportScheme = New System.Windows.Forms.MenuItem()
        Me.MenuOpenProject = New System.Windows.Forms.MenuItem()
        Me.MenuCloseProject = New System.Windows.Forms.MenuItem()
        Me.MenuOpenRecent = New System.Windows.Forms.MenuItem()
        Me.MenuItem1 = New System.Windows.Forms.MenuItem()
        Me.MenuSaveProjectN = New System.Windows.Forms.MenuItem()
        Me.MenuSaveProjectAsN = New System.Windows.Forms.MenuItem()
        Me.MenuCloseScreen = New System.Windows.Forms.MenuItem()
        Me.MenuExportScreen = New System.Windows.Forms.MenuItem()
        Me.MenuExportScheme = New System.Windows.Forms.MenuItem()
        Me.MenuExportProject = New System.Windows.Forms.MenuItem()
        Me.MenuExportPlayer = New System.Windows.Forms.MenuItem()
        Me.MenuExportScreenshots = New System.Windows.Forms.MenuItem()
        Me.MenuExit = New System.Windows.Forms.MenuItem()
        Me.MenuAlignLefts = New System.Windows.Forms.MenuItem()
        Me.MenuAlignCenters = New System.Windows.Forms.MenuItem()
        Me.MenuAlignRights = New System.Windows.Forms.MenuItem()
        Me.MenuAlignTops = New System.Windows.Forms.MenuItem()
        Me.MenuAlignMiddles = New System.Windows.Forms.MenuItem()
        Me.MenuAlignBottoms = New System.Windows.Forms.MenuItem()
        Me.MenuBringToFront = New System.Windows.Forms.MenuItem()
        Me.MenuSendToBack = New System.Windows.Forms.MenuItem()
        Me.MenuLayout = New System.Windows.Forms.MenuItem()
        Me.MenuLayoutSave = New System.Windows.Forms.MenuItem()
        Me.MenuLayoutSaveAs = New System.Windows.Forms.MenuItem()
        Me.MenuLayoutLoad = New System.Windows.Forms.MenuItem()
        Me.MenuLayoutResetWindows = New System.Windows.Forms.MenuItem()
        Me.MenuPreferences = New System.Windows.Forms.MenuItem()
        Me.MenuProjectSettings = New System.Windows.Forms.MenuItem()
        Me.MenuGenerateCode = New System.Windows.Forms.MenuItem()
        Me.MenuMPLABXWizard = New System.Windows.Forms.MenuItem()
        Me.MenuMPLABXWizardTemplatesEditor = New System.Windows.Forms.MenuItem()
        Me.MenuExternalWidgets = New System.Windows.Forms.MenuItem()
        Me.MenuExploreTemplatesFolder = New System.Windows.Forms.MenuItem()
        Me.MenuShowSplashPage = New System.Windows.Forms.MenuItem()
        Me.MenuBitmapChooser = New System.Windows.Forms.MenuItem()
        Me.MenuFontChooser = New System.Windows.Forms.MenuItem()
        Me.MenuCustomWidgetsDefine = New System.Windows.Forms.MenuItem()
        Me.MenuEdit = New System.Windows.Forms.MenuItem()
        Me.MenuCut = New System.Windows.Forms.MenuItem()
        Me.MenuCopy = New System.Windows.Forms.MenuItem()
        Me.MenuPaste = New System.Windows.Forms.MenuItem()
        Me.MenuDelete = New System.Windows.Forms.MenuItem()
        Me.MenuSelectAll = New System.Windows.Forms.MenuItem()
        Me.MenuUndo = New System.Windows.Forms.MenuItem()
        Me.MenuRedo = New System.Windows.Forms.MenuItem()
        Me.MenuMakeSameSize = New System.Windows.Forms.MenuItem()
        Me.MenuEditAllEvents = New System.Windows.Forms.MenuItem()
        Me.MenuHelp = New System.Windows.Forms.MenuItem()
        Me.MenuLicense = New System.Windows.Forms.MenuItem()
        Me.MenuWiki = New System.Windows.Forms.MenuItem()
        Me.mainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
        Me.MenuView = New System.Windows.Forms.MenuItem()
        Me.MenuViewWidgetProperties = New System.Windows.Forms.MenuItem()
        Me.MenuViewEvents = New System.Windows.Forms.MenuItem()
        Me.MenuViewSchemes = New System.Windows.Forms.MenuItem()
        Me.MenuViewToolBox = New System.Windows.Forms.MenuItem()
        Me.MenuViewProjectExplorer = New System.Windows.Forms.MenuItem()
        Me.MenuViewEventsEditor = New System.Windows.Forms.MenuItem()
        Me.MenuViewOutputWindow = New System.Windows.Forms.MenuItem()
        Me.MenuViewWidgetInfo = New System.Windows.Forms.MenuItem()
        Me.MenuViewProjectSummary = New System.Windows.Forms.MenuItem()
        Me.MenuViewHtmlEditor = New System.Windows.Forms.MenuItem()
        Me.BottomToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.TopToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.RightToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.LeftToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.ContentPanel = New System.Windows.Forms.ToolStripContentPanel()
        Me._DockPanel1 = New WeifenLuo.WinFormsUI.Docking.DockPanel()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripProjectSettings = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripGenerateCode = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripNewScreen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSaveProject = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripCloseProject = New System.Windows.Forms.ToolStripButton()
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripDeleteWidget = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripCut = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripCopy = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripPaste = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripUndo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripRedo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripGridOnOff = New System.Windows.Forms.ToolStripSplitButton()
        Me.ToolStripGridSizeLabel = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripGridSize = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripMagnify = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBitmapChooser = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripFontChooser = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSize = New System.Windows.Forms.ToolStripSplitButton()
        Me.ToolStripSameSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSameW = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSameH = New System.Windows.Forms.ToolStripMenuItem()
        Me.DistributeHorizontally = New System.Windows.Forms.ToolStripMenuItem()
        Me.DistributeVertically = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripAlign = New System.Windows.Forms.ToolStripSplitButton()
        Me.ToolStripAlignLeft = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripAlignUp = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripAlignDown = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripAlignRight = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripCenterVert = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripCenterHoriz = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripBringToFront = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSendToBack = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripPlayer = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ToolStripPlayNow = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripExportPackage = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlayerOptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMPLABX = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripdrpLanguage = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripHelp = New System.Windows.Forms.ToolStripButton()
        Me.cmnuWidgets = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BringToFrontToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SendToBackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpProvider1 = New System.Windows.Forms.HelpProvider()
        Me.cmuWidget = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblMouseCoords = New System.Windows.Forms.ToolStripStatusLabel()
        Me._MainSplash = New VGDDIDE.MainSplash()
        Me._EventsEditor = New VGDDIDE.EventsEditor()
        Me._WidgetPropertyGrid = New VGDDIDE.WidgetPropertyGrid()
        Me._EventTree = New VGDDIDE.EventsTree()
        Me._ProjectSummary = New VGDDIDE.ProjectSummary()
        Me._WidgetInfo = New VGDDIDE.WidgetInfo()
        Me._OutputWindow = New VGDDIDE.OutputWindow()
        Me._HTMLEditor = New VGDDIDE.HTMLEditor()
        Me._SolutionExplorer = New VGDDIDE.SolutionExplorer()
        Me._SchemesChooser = New VGDDIDE.SchemesChooser()
        Me._Toolbox = New VGDDIDE.CollapseToolbox()
        MenuAdd = New System.Windows.Forms.MenuItem()
        MenuFile = New System.Windows.Forms.MenuItem()
        MenuExport = New System.Windows.Forms.MenuItem()
        MenuAlign = New System.Windows.Forms.MenuItem()
        MenuOrder = New System.Windows.Forms.MenuItem()
        MenuTools = New System.Windows.Forms.MenuItem()
        Me.ToolStrip1.SuspendLayout()
        Me.cmnuWidgets.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me._MainSplash.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuAdd
        '
        MenuAdd.Index = 6
        MenuAdd.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuScreenNew, Me.MenuScreenAddExisting, Me.MenuImportScheme})
        MenuAdd.Text = "Add"
        '
        'MenuScreenNew
        '
        Me.MenuScreenNew.Index = 0
        Me.MenuScreenNew.Text = "New Screen (Ctrl-N)"
        '
        'MenuScreenAddExisting
        '
        Me.MenuScreenAddExisting.Index = 1
        Me.MenuScreenAddExisting.Text = "Existing Screen(s)"
        '
        'MenuImportScheme
        '
        Me.MenuImportScheme.Index = 2
        Me.MenuImportScheme.Text = "Import Scheme"
        '
        'MenuFile
        '
        MenuFile.Index = 0
        MenuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuOpenProject, Me.MenuCloseProject, Me.MenuOpenRecent, Me.MenuItem1, Me.MenuSaveProjectN, Me.MenuSaveProjectAsN, MenuAdd, Me.MenuCloseScreen, MenuExport, Me.MenuExit})
        MenuFile.Text = "&File"
        '
        'MenuOpenProject
        '
        Me.MenuOpenProject.Index = 0
        Me.MenuOpenProject.Text = "Open Project"
        '
        'MenuCloseProject
        '
        Me.MenuCloseProject.Index = 1
        Me.MenuCloseProject.Text = "Close Project"
        '
        'MenuOpenRecent
        '
        Me.MenuOpenRecent.Index = 2
        Me.MenuOpenRecent.Text = "Open Recent Project"
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 3
        Me.MenuItem1.Text = "Save Screen (Ctrl-Shift-S)"
        '
        'MenuSaveProjectN
        '
        Me.MenuSaveProjectN.Index = 4
        Me.MenuSaveProjectN.Text = "Save Project (Ctrl-S)"
        '
        'MenuSaveProjectAsN
        '
        Me.MenuSaveProjectAsN.Index = 5
        Me.MenuSaveProjectAsN.Text = "SaveProject As ..."
        '
        'MenuCloseScreen
        '
        Me.MenuCloseScreen.Index = 7
        Me.MenuCloseScreen.Text = "Close Screen"
        '
        'MenuExport
        '
        MenuExport.Index = 8
        MenuExport.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuExportScreen, Me.MenuExportScheme, Me.MenuExportProject, Me.MenuExportPlayer, Me.MenuExportScreenshots})
        MenuExport.Text = "Export"
        '
        'MenuExportScreen
        '
        Me.MenuExportScreen.Index = 0
        Me.MenuExportScreen.Text = "Screen"
        '
        'MenuExportScheme
        '
        Me.MenuExportScheme.Index = 1
        Me.MenuExportScheme.Text = "Scheme"
        '
        'MenuExportProject
        '
        Me.MenuExportProject.Index = 2
        Me.MenuExportProject.Text = "Project"
        '
        'MenuExportPlayer
        '
        Me.MenuExportPlayer.Index = 3
        Me.MenuExportPlayer.Text = "Player Package"
        '
        'MenuExportScreenshots
        '
        Me.MenuExportScreenshots.Index = 4
        Me.MenuExportScreenshots.Text = "Screenshots"
        '
        'MenuExit
        '
        Me.MenuExit.Index = 9
        Me.MenuExit.Text = "E&xit (Alt-F4)"
        '
        'MenuAlign
        '
        MenuAlign.Index = 5
        MenuAlign.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuAlignLefts, Me.MenuAlignCenters, Me.MenuAlignRights, Me.MenuAlignTops, Me.MenuAlignMiddles, Me.MenuAlignBottoms})
        MenuAlign.Text = "&Align"
        '
        'MenuAlignLefts
        '
        Me.MenuAlignLefts.Index = 0
        Me.MenuAlignLefts.Text = "&Lefts"
        '
        'MenuAlignCenters
        '
        Me.MenuAlignCenters.Index = 1
        Me.MenuAlignCenters.Text = "&Centers"
        '
        'MenuAlignRights
        '
        Me.MenuAlignRights.Index = 2
        Me.MenuAlignRights.Text = "&Rights"
        '
        'MenuAlignTops
        '
        Me.MenuAlignTops.Index = 3
        Me.MenuAlignTops.Text = "&Tops"
        '
        'MenuAlignMiddles
        '
        Me.MenuAlignMiddles.Index = 4
        Me.MenuAlignMiddles.Text = "&Middles"
        '
        'MenuAlignBottoms
        '
        Me.MenuAlignBottoms.Index = 5
        Me.MenuAlignBottoms.Text = "&Bottoms"
        '
        'MenuOrder
        '
        MenuOrder.Index = 8
        MenuOrder.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuBringToFront, Me.MenuSendToBack})
        MenuOrder.Text = "Order"
        '
        'MenuBringToFront
        '
        Me.MenuBringToFront.Index = 0
        Me.MenuBringToFront.Text = "Bring to &Front"
        '
        'MenuSendToBack
        '
        Me.MenuSendToBack.Index = 1
        Me.MenuSendToBack.Text = "Send To Bac&k"
        '
        'MenuTools
        '
        MenuTools.Index = 3
        MenuTools.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuLayout, Me.MenuPreferences, Me.MenuProjectSettings, Me.MenuGenerateCode, Me.MenuMPLABXWizard, Me.MenuMPLABXWizardTemplatesEditor, Me.MenuExternalWidgets, Me.MenuExploreTemplatesFolder, Me.MenuShowSplashPage, Me.MenuBitmapChooser, Me.MenuFontChooser, Me.MenuCustomWidgetsDefine})
        MenuTools.Text = "&Tools"
        '
        'MenuLayout
        '
        Me.MenuLayout.Index = 0
        Me.MenuLayout.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuLayoutSave, Me.MenuLayoutSaveAs, Me.MenuLayoutLoad, Me.MenuLayoutResetWindows})
        Me.MenuLayout.Text = "Window Layout"
        '
        'MenuLayoutSave
        '
        Me.MenuLayoutSave.Index = 0
        Me.MenuLayoutSave.Text = "Save"
        '
        'MenuLayoutSaveAs
        '
        Me.MenuLayoutSaveAs.Index = 1
        Me.MenuLayoutSaveAs.Text = "Save as..."
        '
        'MenuLayoutLoad
        '
        Me.MenuLayoutLoad.Index = 2
        Me.MenuLayoutLoad.Text = "Load"
        '
        'MenuLayoutResetWindows
        '
        Me.MenuLayoutResetWindows.Index = 3
        Me.MenuLayoutResetWindows.Text = "Reset Windows Positions"
        '
        'MenuPreferences
        '
        Me.MenuPreferences.Index = 1
        Me.MenuPreferences.Text = "Global Preferences"
        '
        'MenuProjectSettings
        '
        Me.MenuProjectSettings.Index = 2
        Me.MenuProjectSettings.Text = "&Project Settings"
        '
        'MenuGenerateCode
        '
        Me.MenuGenerateCode.Index = 3
        Me.MenuGenerateCode.Text = "Generate Code"
        '
        'MenuMPLABXWizard
        '
        Me.MenuMPLABXWizard.Index = 4
        Me.MenuMPLABXWizard.Text = "MPLAB X Wizard"
        '
        'MenuMPLABXWizardTemplatesEditor
        '
        Me.MenuMPLABXWizardTemplatesEditor.Index = 5
        Me.MenuMPLABXWizardTemplatesEditor.Text = "MPLAB X Wizard Templates Editor"
        '
        'MenuExternalWidgets
        '
        Me.MenuExternalWidgets.Index = 6
        Me.MenuExternalWidgets.Text = "External Widgets..."
        '
        'MenuExploreTemplatesFolder
        '
        Me.MenuExploreTemplatesFolder.Index = 7
        Me.MenuExploreTemplatesFolder.Text = "Explore Templates Folder"
        '
        'MenuShowSplashPage
        '
        Me.MenuShowSplashPage.Index = 8
        Me.MenuShowSplashPage.Text = "Show Initial Page"
        '
        'MenuBitmapChooser
        '
        Me.MenuBitmapChooser.Index = 9
        Me.MenuBitmapChooser.Text = "Bitmap Chooser"
        '
        'MenuFontChooser
        '
        Me.MenuFontChooser.Index = 10
        Me.MenuFontChooser.Text = "Font Chooser"
        '
        'MenuCustomWidgetsDefine
        '
        Me.MenuCustomWidgetsDefine.Index = 11
        Me.MenuCustomWidgetsDefine.Text = "Define Custom Widgets"
        '
        'MenuEdit
        '
        Me.MenuEdit.Index = 1
        Me.MenuEdit.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuCut, Me.MenuCopy, Me.MenuPaste, Me.MenuDelete, Me.MenuSelectAll, MenuAlign, Me.MenuUndo, Me.MenuRedo, MenuOrder, Me.MenuMakeSameSize, Me.MenuEditAllEvents})
        Me.MenuEdit.Text = "&Edit"
        '
        'MenuCut
        '
        Me.MenuCut.Index = 0
        Me.MenuCut.Text = "&Cut (Ctrl-X)"
        '
        'MenuCopy
        '
        Me.MenuCopy.Index = 1
        Me.MenuCopy.Text = "C&opy (Ctrl-C)"
        '
        'MenuPaste
        '
        Me.MenuPaste.Index = 2
        Me.MenuPaste.Text = "&Paste (Ctrl-V)"
        '
        'MenuDelete
        '
        Me.MenuDelete.Index = 3
        Me.MenuDelete.Text = "&Delete (Del)"
        '
        'MenuSelectAll
        '
        Me.MenuSelectAll.Index = 4
        Me.MenuSelectAll.Text = "&Select All (Ctrl-A)"
        '
        'MenuUndo
        '
        Me.MenuUndo.Index = 6
        Me.MenuUndo.Text = "Undo (Ctrl-Z)"
        '
        'MenuRedo
        '
        Me.MenuRedo.Index = 7
        Me.MenuRedo.Text = "Redo (Ctrl-Shift-Z)"
        '
        'MenuMakeSameSize
        '
        Me.MenuMakeSameSize.Index = 9
        Me.MenuMakeSameSize.Text = "Make same Size"
        '
        'MenuEditAllEvents
        '
        Me.MenuEditAllEvents.Index = 10
        Me.MenuEditAllEvents.Text = "Edit all events"
        '
        'MenuHelp
        '
        Me.MenuHelp.Index = 4
        Me.MenuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuLicense, Me.MenuWiki})
        Me.MenuHelp.Text = "&Help"
        '
        'MenuLicense
        '
        Me.MenuLicense.Index = 0
        Me.MenuLicense.Text = "License"
        '
        'MenuWiki
        '
        Me.MenuWiki.Index = 1
        Me.MenuWiki.Text = "Wiki"
        '
        'mainMenu1
        '
        Me.mainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {MenuFile, Me.MenuEdit, Me.MenuView, MenuTools, Me.MenuHelp})
        '
        'MenuView
        '
        Me.MenuView.Index = 2
        Me.MenuView.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuViewWidgetProperties, Me.MenuViewEvents, Me.MenuViewSchemes, Me.MenuViewToolBox, Me.MenuViewProjectExplorer, Me.MenuViewEventsEditor, Me.MenuViewOutputWindow, Me.MenuViewWidgetInfo, Me.MenuViewProjectSummary, Me.MenuViewHtmlEditor})
        Me.MenuView.Text = "View"
        '
        'MenuViewWidgetProperties
        '
        Me.MenuViewWidgetProperties.Checked = True
        Me.MenuViewWidgetProperties.Index = 0
        Me.MenuViewWidgetProperties.Text = "Widget Properties"
        '
        'MenuViewEvents
        '
        Me.MenuViewEvents.Checked = True
        Me.MenuViewEvents.Index = 1
        Me.MenuViewEvents.Text = "Events Tree"
        '
        'MenuViewSchemes
        '
        Me.MenuViewSchemes.Checked = True
        Me.MenuViewSchemes.Index = 2
        Me.MenuViewSchemes.Text = "Schemes"
        '
        'MenuViewToolBox
        '
        Me.MenuViewToolBox.Checked = True
        Me.MenuViewToolBox.Index = 3
        Me.MenuViewToolBox.Text = "ToolBox"
        '
        'MenuViewProjectExplorer
        '
        Me.MenuViewProjectExplorer.Index = 4
        Me.MenuViewProjectExplorer.Text = "Project Explorer"
        '
        'MenuViewEventsEditor
        '
        Me.MenuViewEventsEditor.Index = 5
        Me.MenuViewEventsEditor.Text = "Events Editor"
        '
        'MenuViewOutputWindow
        '
        Me.MenuViewOutputWindow.Index = 6
        Me.MenuViewOutputWindow.Text = "Output Window"
        '
        'MenuViewWidgetInfo
        '
        Me.MenuViewWidgetInfo.Index = 7
        Me.MenuViewWidgetInfo.Text = "Widget Info"
        '
        'MenuViewProjectSummary
        '
        Me.MenuViewProjectSummary.Index = 8
        Me.MenuViewProjectSummary.Text = "Project Summary"
        '
        'MenuViewHtmlEditor
        '
        Me.MenuViewHtmlEditor.Index = 9
        Me.MenuViewHtmlEditor.Text = "HTML Editor"
        '
        'BottomToolStripPanel
        '
        Me.BottomToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.BottomToolStripPanel.Name = "BottomToolStripPanel"
        Me.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.BottomToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.BottomToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'TopToolStripPanel
        '
        Me.TopToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.TopToolStripPanel.Name = "TopToolStripPanel"
        Me.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.TopToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.TopToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'RightToolStripPanel
        '
        Me.RightToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.RightToolStripPanel.Name = "RightToolStripPanel"
        Me.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.RightToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.RightToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'LeftToolStripPanel
        '
        Me.LeftToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.LeftToolStripPanel.Name = "LeftToolStripPanel"
        Me.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.LeftToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.LeftToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'ContentPanel
        '
        Me.ContentPanel.AutoScroll = True
        Me.ContentPanel.Size = New System.Drawing.Size(1068, 411)
        '
        '_DockPanel1
        '
        Me._DockPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me._DockPanel1.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi
        Me._DockPanel1.Location = New System.Drawing.Point(148, 42)
        Me._DockPanel1.Name = "_DockPanel1"
        Me._DockPanel1.ShowDocumentIcon = True
        Me._DockPanel1.Size = New System.Drawing.Size(719, 427)
        DockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight
        DockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight
        AutoHideStripSkin1.DockStripGradient = DockPanelGradient1
        TabGradient1.EndColor = System.Drawing.SystemColors.Control
        TabGradient1.StartColor = System.Drawing.SystemColors.Control
        TabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark
        AutoHideStripSkin1.TabGradient = TabGradient1
        AutoHideStripSkin1.TextFont = New System.Drawing.Font("Segoe UI", 9.0!)
        DockPanelSkin1.AutoHideStripSkin = AutoHideStripSkin1
        TabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight
        TabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight
        TabGradient2.TextColor = System.Drawing.SystemColors.ControlText
        DockPaneStripGradient1.ActiveTabGradient = TabGradient2
        DockPanelGradient2.EndColor = System.Drawing.SystemColors.Control
        DockPanelGradient2.StartColor = System.Drawing.SystemColors.Control
        DockPaneStripGradient1.DockStripGradient = DockPanelGradient2
        TabGradient3.EndColor = System.Drawing.SystemColors.ControlLight
        TabGradient3.StartColor = System.Drawing.SystemColors.ControlLight
        TabGradient3.TextColor = System.Drawing.SystemColors.ControlText
        DockPaneStripGradient1.InactiveTabGradient = TabGradient3
        DockPaneStripSkin1.DocumentGradient = DockPaneStripGradient1
        DockPaneStripSkin1.TextFont = New System.Drawing.Font("Segoe UI", 9.0!)
        TabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption
        TabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        TabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption
        TabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText
        DockPaneStripToolWindowGradient1.ActiveCaptionGradient = TabGradient4
        TabGradient5.EndColor = System.Drawing.SystemColors.Control
        TabGradient5.StartColor = System.Drawing.SystemColors.Control
        TabGradient5.TextColor = System.Drawing.SystemColors.ControlText
        DockPaneStripToolWindowGradient1.ActiveTabGradient = TabGradient5
        DockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight
        DockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight
        DockPaneStripToolWindowGradient1.DockStripGradient = DockPanelGradient3
        TabGradient6.EndColor = System.Drawing.SystemColors.InactiveCaption
        TabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        TabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption
        TabGradient6.TextColor = System.Drawing.SystemColors.InactiveCaptionText
        DockPaneStripToolWindowGradient1.InactiveCaptionGradient = TabGradient6
        TabGradient7.EndColor = System.Drawing.Color.Transparent
        TabGradient7.StartColor = System.Drawing.Color.Transparent
        TabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark
        DockPaneStripToolWindowGradient1.InactiveTabGradient = TabGradient7
        DockPaneStripSkin1.ToolWindowGradient = DockPaneStripToolWindowGradient1
        DockPanelSkin1.DockPaneStripSkin = DockPaneStripSkin1
        Me._DockPanel1.Skin = DockPanelSkin1
        Me._DockPanel1.TabIndex = 0
        '
        'ToolStrip1
        '
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripProjectSettings, Me.ToolStripGenerateCode, Me.ToolStripSeparator8, Me.ToolStripNewScreen, Me.ToolStripOpen, Me.ToolStripSaveProject, Me.ToolStripCloseProject, Me.toolStripSeparator, Me.ToolStripDeleteWidget, Me.ToolStripCut, Me.ToolStripCopy, Me.ToolStripPaste, Me.ToolStripSeparator9, Me.ToolStripUndo, Me.ToolStripRedo, Me.ToolStripSeparator2, Me.ToolStripGridOnOff, Me.ToolStripMagnify, Me.ToolStripBitmapChooser, Me.ToolStripFontChooser, Me.ToolStripSeparator7, Me.ToolStripSize, Me.ToolStripAlign, Me.ToolStripBringToFront, Me.ToolStripSendToBack, Me.ToolStripSeparator5, Me.ToolStripPlayer, Me.ToolStripSeparator3, Me.ToolStripMPLABX, Me.ToolStripSeparator6, Me.ToolStripdrpLanguage, Me.ToolStripSeparator1, Me.ToolStripHelp})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1110, 39)
        Me.ToolStrip1.TabIndex = 3
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripProjectSettings
        '
        Me.ToolStripProjectSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripProjectSettings.Image = Global.My.Resources.Resources.Tools
        Me.ToolStripProjectSettings.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripProjectSettings.Name = "ToolStripProjectSettings"
        Me.ToolStripProjectSettings.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripProjectSettings.Text = "Project Settings"
        '
        'ToolStripGenerateCode
        '
        Me.ToolStripGenerateCode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripGenerateCode.Enabled = False
        Me.ToolStripGenerateCode.Image = Global.My.Resources.Resources.documentaccept
        Me.ToolStripGenerateCode.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripGenerateCode.Name = "ToolStripGenerateCode"
        Me.ToolStripGenerateCode.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripGenerateCode.Text = "Generate Code"
        Me.ToolStripGenerateCode.ToolTipText = "Generate Code"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripNewScreen
        '
        Me.ToolStripNewScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripNewScreen.Image = Global.My.Resources.Resources.documentadd
        Me.ToolStripNewScreen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripNewScreen.Name = "ToolStripNewScreen"
        Me.ToolStripNewScreen.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripNewScreen.Text = "&New Screen"
        Me.ToolStripNewScreen.ToolTipText = "New Screen"
        '
        'ToolStripOpen
        '
        Me.ToolStripOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripOpen.Image = Global.My.Resources.Resources.FolderOpen1
        Me.ToolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripOpen.Name = "ToolStripOpen"
        Me.ToolStripOpen.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripOpen.Text = "&Open Project"
        '
        'ToolStripSaveProject
        '
        Me.ToolStripSaveProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSaveProject.Enabled = False
        Me.ToolStripSaveProject.Image = Global.My.Resources.Resources.save2
        Me.ToolStripSaveProject.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSaveProject.Name = "ToolStripSaveProject"
        Me.ToolStripSaveProject.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripSaveProject.Text = "&Save Project"
        '
        'ToolStripCloseProject
        '
        Me.ToolStripCloseProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripCloseProject.Enabled = False
        Me.ToolStripCloseProject.Image = Global.My.Resources.Resources.Folder
        Me.ToolStripCloseProject.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripCloseProject.Name = "ToolStripCloseProject"
        Me.ToolStripCloseProject.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripCloseProject.Text = "Close Project"
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripDeleteWidget
        '
        Me.ToolStripDeleteWidget.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripDeleteWidget.Enabled = False
        Me.ToolStripDeleteWidget.Image = Global.My.Resources.Resources.Close
        Me.ToolStripDeleteWidget.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDeleteWidget.Name = "ToolStripDeleteWidget"
        Me.ToolStripDeleteWidget.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripDeleteWidget.Text = "Delete Widget(s)"
        '
        'ToolStripCut
        '
        Me.ToolStripCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripCut.Enabled = False
        Me.ToolStripCut.Image = Global.My.Resources.Resources.cut
        Me.ToolStripCut.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripCut.Name = "ToolStripCut"
        Me.ToolStripCut.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripCut.Text = "C&ut"
        '
        'ToolStripCopy
        '
        Me.ToolStripCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripCopy.Enabled = False
        Me.ToolStripCopy.Image = Global.My.Resources.Resources.Copy
        Me.ToolStripCopy.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripCopy.Name = "ToolStripCopy"
        Me.ToolStripCopy.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripCopy.Text = "&Copy"
        '
        'ToolStripPaste
        '
        Me.ToolStripPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripPaste.Enabled = False
        Me.ToolStripPaste.Image = Global.My.Resources.Resources.Paste
        Me.ToolStripPaste.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripPaste.Name = "ToolStripPaste"
        Me.ToolStripPaste.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripPaste.Text = "&Paste"
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripUndo
        '
        Me.ToolStripUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripUndo.Enabled = False
        Me.ToolStripUndo.Image = Global.My.Resources.Resources.arrow_rotate_anticlockwisered
        Me.ToolStripUndo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripUndo.Name = "ToolStripUndo"
        Me.ToolStripUndo.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripUndo.Text = "Undo"
        '
        'ToolStripRedo
        '
        Me.ToolStripRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripRedo.Enabled = False
        Me.ToolStripRedo.Image = Global.My.Resources.Resources.arrow_rotate_clockwise
        Me.ToolStripRedo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripRedo.Name = "ToolStripRedo"
        Me.ToolStripRedo.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripRedo.Text = "Redo"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripGridOnOff
        '
        Me.ToolStripGridOnOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripGridOnOff.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripGridSizeLabel})
        Me.ToolStripGridOnOff.Image = Global.My.Resources.Resources.grid
        Me.ToolStripGridOnOff.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripGridOnOff.Name = "ToolStripGridOnOff"
        Me.ToolStripGridOnOff.Size = New System.Drawing.Size(48, 36)
        Me.ToolStripGridOnOff.Text = "Grid on/off"
        '
        'ToolStripGridSizeLabel
        '
        Me.ToolStripGridSizeLabel.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripGridSize})
        Me.ToolStripGridSizeLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripGridSizeLabel.Name = "ToolStripGridSizeLabel"
        Me.ToolStripGridSizeLabel.Size = New System.Drawing.Size(119, 22)
        Me.ToolStripGridSizeLabel.Text = "Grid Size"
        Me.ToolStripGridSizeLabel.ToolTipText = "Set Grid Size"
        '
        'ToolStripGridSize
        '
        Me.ToolStripGridSize.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripGridSize.Name = "ToolStripGridSize"
        Me.ToolStripGridSize.Size = New System.Drawing.Size(100, 23)
        Me.ToolStripGridSize.Text = "10"
        '
        'ToolStripMagnify
        '
        Me.ToolStripMagnify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripMagnify.Image = Global.My.Resources.Resources.Zoom
        Me.ToolStripMagnify.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripMagnify.Name = "ToolStripMagnify"
        Me.ToolStripMagnify.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripMagnify.Text = "Magnifier on/off"
        '
        'ToolStripBitmapChooser
        '
        Me.ToolStripBitmapChooser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBitmapChooser.Enabled = False
        Me.ToolStripBitmapChooser.Image = Global.My.Resources.Resources.pictures
        Me.ToolStripBitmapChooser.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBitmapChooser.Name = "ToolStripBitmapChooser"
        Me.ToolStripBitmapChooser.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripBitmapChooser.Text = "Bitmap Chooser"
        Me.ToolStripBitmapChooser.ToolTipText = "Opens Bitmap Chooser"
        '
        'ToolStripFontChooser
        '
        Me.ToolStripFontChooser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripFontChooser.Enabled = False
        Me.ToolStripFontChooser.Image = Global.My.Resources.Resources.font_go
        Me.ToolStripFontChooser.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripFontChooser.Name = "ToolStripFontChooser"
        Me.ToolStripFontChooser.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripFontChooser.Text = "Font Chooser"
        Me.ToolStripFontChooser.ToolTipText = "Opens Font Chooser"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripSize
        '
        Me.ToolStripSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSize.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripSameSize, Me.ToolStripSameW, Me.ToolStripSameH, Me.DistributeHorizontally, Me.DistributeVertically})
        Me.ToolStripSize.Enabled = False
        Me.ToolStripSize.Image = Global.My.Resources.Resources.transform_move
        Me.ToolStripSize.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSize.Name = "ToolStripSize"
        Me.ToolStripSize.Size = New System.Drawing.Size(48, 36)
        Me.ToolStripSize.Text = "Size"
        Me.ToolStripSize.ToolTipText = "Size"
        '
        'ToolStripSameSize
        '
        Me.ToolStripSameSize.Image = Global.My.Resources.Resources.canvas_size
        Me.ToolStripSameSize.Name = "ToolStripSameSize"
        Me.ToolStripSameSize.Size = New System.Drawing.Size(208, 38)
        Me.ToolStripSameSize.Text = "Make same size"
        Me.ToolStripSameSize.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay
        '
        'ToolStripSameW
        '
        Me.ToolStripSameW.Image = Global.My.Resources.Resources.size_horizontal
        Me.ToolStripSameW.Name = "ToolStripSameW"
        Me.ToolStripSameW.Size = New System.Drawing.Size(208, 38)
        Me.ToolStripSameW.Text = "Make same Width"
        '
        'ToolStripSameH
        '
        Me.ToolStripSameH.Image = Global.My.Resources.Resources.size_vertical
        Me.ToolStripSameH.Name = "ToolStripSameH"
        Me.ToolStripSameH.Size = New System.Drawing.Size(208, 38)
        Me.ToolStripSameH.Text = "Make same Height"
        '
        'DistributeHorizontally
        '
        Me.DistributeHorizontally.Name = "DistributeHorizontally"
        Me.DistributeHorizontally.Size = New System.Drawing.Size(208, 38)
        Me.DistributeHorizontally.Text = "Distribute Horizontally"
        '
        'DistributeVertically
        '
        Me.DistributeVertically.Name = "DistributeVertically"
        Me.DistributeVertically.Size = New System.Drawing.Size(208, 38)
        Me.DistributeVertically.Text = "Distribute Vertically"
        '
        'ToolStripAlign
        '
        Me.ToolStripAlign.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripAlign.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripAlignLeft, Me.ToolStripAlignUp, Me.ToolStripAlignDown, Me.ToolStripAlignRight, Me.ToolStripCenterVert, Me.ToolStripCenterHoriz})
        Me.ToolStripAlign.Enabled = False
        Me.ToolStripAlign.Image = Global.My.Resources.Resources.shape_align1
        Me.ToolStripAlign.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripAlign.Name = "ToolStripAlign"
        Me.ToolStripAlign.Size = New System.Drawing.Size(48, 36)
        Me.ToolStripAlign.Text = "Alignment"
        Me.ToolStripAlign.ToolTipText = "Alignment"
        '
        'ToolStripAlignLeft
        '
        Me.ToolStripAlignLeft.Image = Global.My.Resources.Resources.shape_align_left
        Me.ToolStripAlignLeft.Name = "ToolStripAlignLeft"
        Me.ToolStripAlignLeft.Size = New System.Drawing.Size(192, 38)
        Me.ToolStripAlignLeft.Text = "Align Left"
        '
        'ToolStripAlignUp
        '
        Me.ToolStripAlignUp.Image = Global.My.Resources.Resources.shape_align_top
        Me.ToolStripAlignUp.Name = "ToolStripAlignUp"
        Me.ToolStripAlignUp.Size = New System.Drawing.Size(192, 38)
        Me.ToolStripAlignUp.Text = "Align Top"
        '
        'ToolStripAlignDown
        '
        Me.ToolStripAlignDown.Image = Global.My.Resources.Resources.shape_align_bottom
        Me.ToolStripAlignDown.Name = "ToolStripAlignDown"
        Me.ToolStripAlignDown.Size = New System.Drawing.Size(192, 38)
        Me.ToolStripAlignDown.Text = "Align Bottom"
        '
        'ToolStripAlignRight
        '
        Me.ToolStripAlignRight.Image = Global.My.Resources.Resources.shape_align_right
        Me.ToolStripAlignRight.Name = "ToolStripAlignRight"
        Me.ToolStripAlignRight.Size = New System.Drawing.Size(192, 38)
        Me.ToolStripAlignRight.Text = "Align Right"
        '
        'ToolStripCenterVert
        '
        Me.ToolStripCenterVert.Image = Global.My.Resources.Resources.shape_align_center
        Me.ToolStripCenterVert.Name = "ToolStripCenterVert"
        Me.ToolStripCenterVert.Size = New System.Drawing.Size(192, 38)
        Me.ToolStripCenterVert.Text = "Center Vertically"
        '
        'ToolStripCenterHoriz
        '
        Me.ToolStripCenterHoriz.Image = Global.My.Resources.Resources.shape_align_middle
        Me.ToolStripCenterHoriz.Name = "ToolStripCenterHoriz"
        Me.ToolStripCenterHoriz.Size = New System.Drawing.Size(192, 38)
        Me.ToolStripCenterHoriz.Text = "Center Horizontally"
        '
        'ToolStripBringToFront
        '
        Me.ToolStripBringToFront.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBringToFront.Enabled = False
        Me.ToolStripBringToFront.Image = Global.My.Resources.Resources.shape_move_forwards
        Me.ToolStripBringToFront.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBringToFront.Name = "ToolStripBringToFront"
        Me.ToolStripBringToFront.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripBringToFront.Text = "Bring to Front"
        Me.ToolStripBringToFront.ToolTipText = "Bring to Front"
        '
        'ToolStripSendToBack
        '
        Me.ToolStripSendToBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSendToBack.Enabled = False
        Me.ToolStripSendToBack.Image = Global.My.Resources.Resources.shape_move_back
        Me.ToolStripSendToBack.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSendToBack.Name = "ToolStripSendToBack"
        Me.ToolStripSendToBack.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripSendToBack.Text = "Send to Back"
        Me.ToolStripSendToBack.ToolTipText = "Send to Back"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripPlayer
        '
        Me.ToolStripPlayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripPlayer.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripPlayNow, Me.ToolStripExportPackage, Me.PlayerOptionsToolStripMenuItem})
        Me.ToolStripPlayer.Enabled = False
        Me.ToolStripPlayer.Image = Global.My.Resources.Resources.play1
        Me.ToolStripPlayer.Name = "ToolStripPlayer"
        Me.ToolStripPlayer.Size = New System.Drawing.Size(45, 36)
        Me.ToolStripPlayer.Text = "Player"
        Me.ToolStripPlayer.ToolTipText = "Play (Simulate) GUI"
        '
        'ToolStripPlayNow
        '
        Me.ToolStripPlayNow.Image = Global.My.Resources.Resources.play1
        Me.ToolStripPlayNow.Name = "ToolStripPlayNow"
        Me.ToolStripPlayNow.Size = New System.Drawing.Size(171, 38)
        Me.ToolStripPlayNow.Text = "Play now"
        '
        'ToolStripExportPackage
        '
        Me.ToolStripExportPackage.Image = Global.My.Resources.Resources.forwardG
        Me.ToolStripExportPackage.Name = "ToolStripExportPackage"
        Me.ToolStripExportPackage.Size = New System.Drawing.Size(171, 38)
        Me.ToolStripExportPackage.Text = "Export Package"
        '
        'PlayerOptionsToolStripMenuItem
        '
        Me.PlayerOptionsToolStripMenuItem.Image = Global.My.Resources.Resources.edit
        Me.PlayerOptionsToolStripMenuItem.Name = "PlayerOptionsToolStripMenuItem"
        Me.PlayerOptionsToolStripMenuItem.Size = New System.Drawing.Size(171, 38)
        Me.PlayerOptionsToolStripMenuItem.Text = "Player Options"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripMPLABX
        '
        Me.ToolStripMPLABX.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripMPLABX.Enabled = False
        Me.ToolStripMPLABX.Image = Global.My.Resources.Resources.MPLABX
        Me.ToolStripMPLABX.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripMPLABX.Name = "ToolStripMPLABX"
        Me.ToolStripMPLABX.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripMPLABX.Text = "MPLAB X Wizard"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripdrpLanguage
        '
        Me.ToolStripdrpLanguage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripdrpLanguage.Image = CType(resources.GetObject("ToolStripdrpLanguage.Image"), System.Drawing.Image)
        Me.ToolStripdrpLanguage.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripdrpLanguage.Name = "ToolStripdrpLanguage"
        Me.ToolStripdrpLanguage.Size = New System.Drawing.Size(45, 36)
        Me.ToolStripdrpLanguage.Text = "StringsPool"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripHelp
        '
        Me.ToolStripHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripHelp.Image = Global.My.Resources.Resources.help
        Me.ToolStripHelp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripHelp.Name = "ToolStripHelp"
        Me.ToolStripHelp.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripHelp.Text = "He&lp"
        '
        'cmnuWidgets
        '
        Me.cmnuWidgets.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BringToFrontToolStripMenuItem, Me.SendToBackToolStripMenuItem})
        Me.cmnuWidgets.Name = "cmnuWidgets"
        Me.cmnuWidgets.Size = New System.Drawing.Size(146, 48)
        '
        'BringToFrontToolStripMenuItem
        '
        Me.BringToFrontToolStripMenuItem.Name = "BringToFrontToolStripMenuItem"
        Me.BringToFrontToolStripMenuItem.Size = New System.Drawing.Size(145, 22)
        Me.BringToFrontToolStripMenuItem.Text = "Bring to front"
        '
        'SendToBackToolStripMenuItem
        '
        Me.SendToBackToolStripMenuItem.Name = "SendToBackToolStripMenuItem"
        Me.SendToBackToolStripMenuItem.Size = New System.Drawing.Size(145, 22)
        Me.SendToBackToolStripMenuItem.Text = "Send to back"
        '
        'HelpProvider1
        '
        Me.HelpProvider1.HelpNamespace = "http://virtualfab.it/mediawiki/index.php/VGDD:ContextHelp"
        '
        'cmuWidget
        '
        Me.cmuWidget.Name = "cmuWidget"
        Me.cmuWidget.Size = New System.Drawing.Size(61, 4)
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.lblMouseCoords})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 496)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1110, 22)
        Me.StatusStrip1.TabIndex = 5
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = False
        Me.lblStatus.Font = New System.Drawing.Font("Tahoma", 9.75!)
        Me.lblStatus.ForeColor = System.Drawing.Color.Blue
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(1007, 17)
        Me.lblStatus.Spring = True
        Me.lblStatus.Text = "STATUS"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopLeft
        '
        'lblMouseCoords
        '
        Me.lblMouseCoords.Name = "lblMouseCoords"
        Me.lblMouseCoords.Size = New System.Drawing.Size(88, 17)
        Me.lblMouseCoords.Text = "X=1024 Y=1080"
        '
        '_MainSplash
        '
        Me._MainSplash.ClientSize = New System.Drawing.Size(719, 390)
        Me._MainSplash.CloseButton = False
        Me._MainSplash.CloseButtonVisible = False
        Me._MainSplash.Controls.Add(Me._EventsEditor)
        Me._MainSplash.Controls.Add(Me._WidgetPropertyGrid)
        Me._MainSplash.Controls.Add(Me._EventTree)
        Me._MainSplash.DockAreas = CType((WeifenLuo.WinFormsUI.Docking.DockAreas.Float Or WeifenLuo.WinFormsUI.Docking.DockAreas.Document), WeifenLuo.WinFormsUI.Docking.DockAreas)
        Me._MainSplash.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._MainSplash.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._MainSplash.Icon = CType(resources.GetObject("_MainSplash.Icon"), System.Drawing.Icon)
        Me._MainSplash.Location = New System.Drawing.Point(148, 42)
        Me._MainSplash.Name = "_MainSplash"
        Me._MainSplash.ShowInTaskbar = False
        Me._MainSplash.TabText = "Start"
        Me._MainSplash.Text = "Start"
        Me._MainSplash.Visible = False
        '
        '_EventsEditor
        '
        Me._EventsEditor.ClientSize = New System.Drawing.Size(430, 260)
        Me._EventsEditor.CloseButtonVisible = False
        Me._EventsEditor.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._EventsEditor.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._EventsEditor.Icon = CType(resources.GetObject("_EventsEditor.Icon"), System.Drawing.Icon)
        Me._EventsEditor.Location = New System.Drawing.Point(0, 130)
        Me._EventsEditor.MinimumSize = New System.Drawing.Size(430, 260)
        Me._EventsEditor.Name = "_EventsEditor"
        Me._EventsEditor.ParentScreenName = Nothing
        Me._EventsEditor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
        Me._EventsEditor.ShowInTaskbar = False
        Me._EventsEditor.TabText = "Events Editor"
        Me._EventsEditor.Text = "Events Editor"
        Me._EventsEditor.Visible = False
        '
        '_WidgetPropertyGrid
        '
        Me._WidgetPropertyGrid.ClientSize = New System.Drawing.Size(237, 115)
        Me._WidgetPropertyGrid.CloseButtonVisible = False
        Me._WidgetPropertyGrid.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._WidgetPropertyGrid.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._WidgetPropertyGrid.Icon = CType(resources.GetObject("_WidgetPropertyGrid.Icon"), System.Drawing.Icon)
        Me._WidgetPropertyGrid.Location = New System.Drawing.Point(513, 61)
        Me._WidgetPropertyGrid.Name = "_WidgetPropertyGrid"
        Me._WidgetPropertyGrid.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
        Me._WidgetPropertyGrid.ShowInTaskbar = False
        Me._WidgetPropertyGrid.TabText = "Widget Properties"
        Me._WidgetPropertyGrid.Text = "Widget Properties"
        Me._WidgetPropertyGrid.Visible = False
        '
        '_EventTree
        '
        Me._EventTree.ClientSize = New System.Drawing.Size(240, 133)
        Me._EventTree.CloseButtonVisible = False
        Me._EventTree.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._EventTree.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._EventTree.Icon = CType(resources.GetObject("_EventTree.Icon"), System.Drawing.Icon)
        Me._EventTree.Location = New System.Drawing.Point(684, 2)
        Me._EventTree.Name = "_EventTree"
        Me._EventTree.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
        Me._EventTree.ShowInTaskbar = False
        Me._EventTree.TabText = "Events"
        Me._EventTree.Text = "Events"
        Me._EventTree.Visible = False
        '
        '_ProjectSummary
        '
        Me._ProjectSummary.ClientSize = New System.Drawing.Size(284, 155)
        Me._ProjectSummary.CloseButtonVisible = False
        Me._ProjectSummary.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._ProjectSummary.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._ProjectSummary.Icon = CType(resources.GetObject("_ProjectSummary.Icon"), System.Drawing.Icon)
        Me._ProjectSummary.Location = New System.Drawing.Point(519, 364)
        Me._ProjectSummary.Name = "_ProjectSummary"
        Me._ProjectSummary.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
        Me._ProjectSummary.ShowInTaskbar = False
        Me._ProjectSummary.TabText = "Summary"
        Me._ProjectSummary.Text = "Summary"
        Me._ProjectSummary.Visible = False
        '
        '_WidgetInfo
        '
        Me._WidgetInfo.AutoScroll = True
        Me._WidgetInfo.AutoScrollMinSize = New System.Drawing.Size(0, 400)
        Me._WidgetInfo.BackColor = System.Drawing.Color.White
        Me._WidgetInfo.ClientSize = New System.Drawing.Size(21, 149)
        Me._WidgetInfo.CloseButtonVisible = False
        Me._WidgetInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._WidgetInfo.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._WidgetInfo.Icon = CType(resources.GetObject("_WidgetInfo.Icon"), System.Drawing.Icon)
        Me._WidgetInfo.Location = New System.Drawing.Point(237, 353)
        Me._WidgetInfo.Name = "_WidgetInfo"
        Me._WidgetInfo.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
        Me._WidgetInfo.ShowInTaskbar = False
        Me._WidgetInfo.TabText = "Widget Info"
        Me._WidgetInfo.Text = "Widget Info"
        Me._WidgetInfo.Visible = False
        '
        '_OutputWindow
        '
        Me._OutputWindow.ClientSize = New System.Drawing.Size(220, 148)
        Me._OutputWindow.CloseButtonVisible = False
        Me._OutputWindow.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._OutputWindow.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._OutputWindow.Icon = CType(resources.GetObject("_OutputWindow.Icon"), System.Drawing.Icon)
        Me._OutputWindow.Location = New System.Drawing.Point(870, 156)
        Me._OutputWindow.Name = "_OutputWindow"
        Me._OutputWindow.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
        Me._OutputWindow.ShowInTaskbar = False
        Me._OutputWindow.TabText = "Output"
        Me._OutputWindow.Text = "Output"
        Me._OutputWindow.Visible = False
        '
        '_HTMLEditor
        '
        Me._HTMLEditor.ClientSize = New System.Drawing.Size(220, 148)
        Me._HTMLEditor.CloseButtonVisible = False
        Me._HTMLEditor.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._HTMLEditor.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._HTMLEditor.Icon = CType(resources.GetObject("_HTMLEditor.Icon"), System.Drawing.Icon)
        Me._HTMLEditor.Location = New System.Drawing.Point(765, 364)
        Me._HTMLEditor.Name = "_HTMLEditor"
        Me._HTMLEditor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
        Me._HTMLEditor.ShowInTaskbar = False
        Me._HTMLEditor.TabText = "HTML Editor"
        Me._HTMLEditor.Text = "HTML Editor"
        Me._HTMLEditor.Visible = False
        '
        '_SolutionExplorer
        '
        Me._SolutionExplorer.ClientSize = New System.Drawing.Size(640, 171)
        Me._SolutionExplorer.CloseButtonVisible = False
        Me._SolutionExplorer.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._SolutionExplorer.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._SolutionExplorer.Icon = CType(resources.GetObject("_SolutionExplorer.Icon"), System.Drawing.Icon)
        Me._SolutionExplorer.Location = New System.Drawing.Point(148, 349)
        Me._SolutionExplorer.Name = "_SolutionExplorer"
        Me._SolutionExplorer.ProjectPathName = "New"
        Me._SolutionExplorer.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft
        Me._SolutionExplorer.ShowInTaskbar = False
        Me._SolutionExplorer.TabText = "Project Explorer"
        Me._SolutionExplorer.Text = "Project Explorer"
        Me._SolutionExplorer.Visible = False
        '
        '_SchemesChooser
        '
        Me._SchemesChooser.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me._SchemesChooser.ClientSize = New System.Drawing.Size(240, 200)
        Me._SchemesChooser.CloseButtonVisible = False
        Me._SchemesChooser.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._SchemesChooser.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._SchemesChooser.Icon = CType(resources.GetObject("_SchemesChooser.Icon"), System.Drawing.Icon)
        Me._SchemesChooser.Location = New System.Drawing.Point(870, 320)
        Me._SchemesChooser.MinimumSize = New System.Drawing.Size(240, 134)
        Me._SchemesChooser.Name = "_SchemesChooser"
        Me._SchemesChooser.SelectedObject = Nothing
        Me._SchemesChooser.SelectedSchemeName = ""
        Me._SchemesChooser.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
        Me._SchemesChooser.ShowInTaskbar = False
        Me._SchemesChooser.TabText = "Schemes"
        Me._SchemesChooser.Text = "Schemes"
        Me._SchemesChooser.Visible = False
        '
        '_Toolbox
        '
        Me._Toolbox.ClientSize = New System.Drawing.Size(142, 451)
        Me._Toolbox.CloseButtonVisible = False
        Me._Toolbox.DesignerHost = Nothing
        Me._Toolbox.FilePath = Nothing
        Me._Toolbox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._Toolbox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me._Toolbox.Icon = CType(resources.GetObject("_Toolbox.Icon"), System.Drawing.Icon)
        Me._Toolbox.Location = New System.Drawing.Point(3, 42)
        Me._Toolbox.Name = "_Toolbox"
        Me._Toolbox.SelectedCategory = Nothing
        Me._Toolbox.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft
        Me._Toolbox.ShowInTaskbar = False
        Me._Toolbox.TabText = "ToolBox"
        Me._Toolbox.Text = "ToolBox"
        Me._Toolbox.Visible = False
        '
        'MainShell
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1110, 518)
        Me.Controls.Add(Me._SchemesChooser)
        Me.Controls.Add(Me._MainSplash)
        Me.Controls.Add(Me._ProjectSummary)
        Me.Controls.Add(Me._WidgetInfo)
        Me.Controls.Add(Me._OutputWindow)
        Me.Controls.Add(Me._HTMLEditor)
        Me.Controls.Add(Me._SolutionExplorer)
        Me.Controls.Add(Me._Toolbox)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me._DockPanel1)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.HelpProvider1.SetHelpKeyword(Me, "Main")
        Me.HelpProvider1.SetHelpNavigator(Me, System.Windows.Forms.HelpNavigator.Topic)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.KeyPreview = True
        Me.Menu = Me.mainMenu1
        Me.MinimumSize = New System.Drawing.Size(640, 480)
        Me.Name = "MainShell"
        Me.HelpProvider1.SetShowHelp(Me, True)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.cmnuWidgets.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me._MainSplash.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuScreenNew As System.Windows.Forms.MenuItem
    Friend WithEvents MenuOpenProject As System.Windows.Forms.MenuItem
    Friend WithEvents MenuScreenAddExisting As System.Windows.Forms.MenuItem
    Friend WithEvents MenuExportScreen As System.Windows.Forms.MenuItem
    Friend WithEvents MenuExportScheme As System.Windows.Forms.MenuItem
    Friend WithEvents MenuGenerateCode As System.Windows.Forms.MenuItem
    Friend WithEvents MenuLicense As System.Windows.Forms.MenuItem
    Friend WithEvents BottomToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents TopToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents RightToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents LeftToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents ContentPanel As System.Windows.Forms.ToolStripContentPanel
    Friend WithEvents _Toolbox As VGDDIDE.CollapseToolbox
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripNewScreen As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripOpen As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSaveProject As System.Windows.Forms.ToolStripButton
    Friend WithEvents toolStripSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripCut As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripPaste As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripHelp As System.Windows.Forms.ToolStripButton
    Friend WithEvents MenuUndo As System.Windows.Forms.MenuItem
    Friend WithEvents MenuRedo As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripGenerateCode As System.Windows.Forms.ToolStripButton
    Friend WithEvents MenuCloseProject As System.Windows.Forms.MenuItem
    Friend WithEvents MenuCloseScreen As System.Windows.Forms.MenuItem
    Friend WithEvents MenuBringToFront As System.Windows.Forms.MenuItem
    Friend WithEvents MenuSendToBack As System.Windows.Forms.MenuItem
    Friend WithEvents MenuOpenRecent As System.Windows.Forms.MenuItem
    Friend WithEvents MenuMakeSameSize As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripProjectSettings As System.Windows.Forms.ToolStripButton
    Friend WithEvents MenuCustomWidgetsDefine As System.Windows.Forms.MenuItem
    Friend WithEvents MenuExportProject As System.Windows.Forms.MenuItem
    Friend WithEvents _SolutionExplorer As VGDDIDE.SolutionExplorer
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents MenuExportPlayer As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripPlayer As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents ToolStripExportPackage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripPlayNow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuLayoutResetWindows As System.Windows.Forms.MenuItem
    Friend WithEvents MenuMPLABXWizard As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripMPLABX As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents MenuMPLABXWizardTemplatesEditor As System.Windows.Forms.MenuItem
    Friend WithEvents MenuExternalWidgets As System.Windows.Forms.MenuItem
    Friend WithEvents cmnuWidgets As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents BringToFrontToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SendToBackToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripBringToFront As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSendToBack As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripMagnify As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripUndo As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripRedo As System.Windows.Forms.ToolStripButton
    Friend WithEvents MenuExploreTemplatesFolder As System.Windows.Forms.MenuItem
    Friend WithEvents MenuShowSplashPage As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripCloseProject As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripDeleteWidget As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripGridOnOff As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents ToolStripGridSizeLabel As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripGridSize As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents MenuEdit As System.Windows.Forms.MenuItem
    Friend WithEvents MenuHelp As System.Windows.Forms.MenuItem
    Friend WithEvents MenuBitmapChooser As System.Windows.Forms.MenuItem
    Friend WithEvents MenuFontChooser As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripSize As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents ToolStripSameSize As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSameW As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSameH As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripAlign As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents ToolStripAlignLeft As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripAlignUp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripAlignDown As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripAlignRight As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripCenterVert As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripCenterHoriz As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripBitmapChooser As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmuWidget As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripFontChooser As System.Windows.Forms.ToolStripButton
    Friend WithEvents MenuPreferences As System.Windows.Forms.MenuItem
    Friend WithEvents _DockPanel1 As WeifenLuo.WinFormsUI.Docking.DockPanel
    Friend WithEvents _WidgetInfo As VGDDIDE.WidgetInfo
    Friend WithEvents _SchemesChooser As VGDDIDE.SchemesChooser
    Friend WithEvents _EventTree As VGDDIDE.EventsTree
    Friend WithEvents _WidgetPropertyGrid As VGDDIDE.WidgetPropertyGrid
    Friend WithEvents _OutputWindow As VGDDIDE.OutputWindow
    Friend WithEvents _HTMLEditor As VGDDIDE.HTMLEditor
    Friend WithEvents _EventsEditor As VGDDIDE.EventsEditor
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents _ProjectSummary As VGDDIDE.ProjectSummary
    Friend WithEvents _MainSplash As VGDDIDE.MainSplash
    Friend WithEvents MenuLayout As System.Windows.Forms.MenuItem
    Friend WithEvents MenuLayoutSave As System.Windows.Forms.MenuItem
    Friend WithEvents MenuLayoutSaveAs As System.Windows.Forms.MenuItem
    Friend WithEvents MenuLayoutLoad As System.Windows.Forms.MenuItem
    Friend WithEvents lblMouseCoords As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
    Friend WithEvents MenuView As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewEvents As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewEventsEditor As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewSchemes As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewToolBox As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewProjectExplorer As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewOutputWindow As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewWidgetInfo As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewProjectSummary As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewHtmlEditor As System.Windows.Forms.MenuItem
    Friend WithEvents MenuViewWidgetProperties As System.Windows.Forms.MenuItem
    Friend WithEvents MenuExportScreenshots As System.Windows.Forms.MenuItem
    Friend WithEvents MenuWiki As System.Windows.Forms.MenuItem
    Friend WithEvents MenuImportScheme As System.Windows.Forms.MenuItem
    Friend WithEvents DistributeHorizontally As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DistributeVertically As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuSaveProjectN As System.Windows.Forms.MenuItem
    Friend WithEvents MenuSaveProjectAsN As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripdrpLanguage As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents MenuEditAllEvents As System.Windows.Forms.MenuItem
    Friend WithEvents PlayerOptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
'End Namespace