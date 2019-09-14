Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.IO
Imports System.Xml
Imports VGDDCommon
Imports VGDD

Namespace VGDDIDE

    Public Class SolutionExplorer
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent ' System.Windows.Forms.UserControl

        Public WithEvents tvSolution As TreeView
        'Public ProjectPath As String '= Path.GetDirectoryName(Application.ExecutablePath)
        Private _ProjectFileName As String = "New"
        Public ProjectImageList As System.Windows.Forms.ImageList

        Public Event FileSelected As EventHandler
        Public Event OpenAllScreens As EventHandler
        Public Event CloseAllScreens As EventHandler
        Public Event TouchAllScreens As EventHandler
        Public Event RemoveScreen As EventHandler
        Public Event NewScreen As EventHandler
        Public Event DuplicateScreen As EventHandler
        Public FileSelected_FileName As String
        Friend WithEvents cmnuSolutionExplorer As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents OpenAllProjectScreensToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RemoveScreenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents CloseAllProjectScreensToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents NewScreenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents lstThumbnails As System.Windows.Forms.ListView
        Friend WithEvents imgMiniature As System.Windows.Forms.ImageList
        Friend WithEvents DuplicateScreenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents TouchAllProjectScreensToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

        'Public Event NodeDoubleClick As EventHandler
        'Public ClickedNode As String

        ' <summary>
        ' Required designer variable.
        ' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        Public Sub New()
            MyBase.New()
            InitializeComponent()
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False

            tvSolution.ContextMenuStrip = cmnuSolutionExplorer
        End Sub

        ' <summary>
        ' Clean up any resources being used.
        ' </summary>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                tvSolution.Nodes.Clear()
                If (Not (components) Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        ' <summary>
        ' Required method for Designer support - do not modify 
        ' the contents of this method with the code editor.
        ' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SolutionExplorer))
            Me.tvSolution = New System.Windows.Forms.TreeView()
            Me.ProjectImageList = New System.Windows.Forms.ImageList(Me.components)
            Me.cmnuSolutionExplorer = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.OpenAllProjectScreensToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.CloseAllProjectScreensToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.RemoveScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.NewScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.DuplicateScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
            Me.lstThumbnails = New System.Windows.Forms.ListView()
            Me.imgMiniature = New System.Windows.Forms.ImageList(Me.components)
            Me.TouchAllProjectScreensToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.cmnuSolutionExplorer.SuspendLayout()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.SuspendLayout()
            '
            'tvSolution
            '
            Me.tvSolution.AllowDrop = True
            Me.tvSolution.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tvSolution.ImageIndex = 0
            Me.tvSolution.ImageList = Me.ProjectImageList
            Me.tvSolution.Location = New System.Drawing.Point(0, 0)
            Me.tvSolution.Name = "tvSolution"
            Me.tvSolution.SelectedImageIndex = 0
            Me.tvSolution.Size = New System.Drawing.Size(145, 248)
            Me.tvSolution.TabIndex = 1
            '
            'ProjectImageList
            '
            Me.ProjectImageList.ImageStream = CType(resources.GetObject("ProjectImageList.ImageStream"), System.Windows.Forms.ImageListStreamer)
            Me.ProjectImageList.TransparentColor = System.Drawing.Color.Transparent
            Me.ProjectImageList.Images.SetKeyName(0, "Projects Folder Badged.png")
            Me.ProjectImageList.Images.SetKeyName(1, "monitor.png")
            Me.ProjectImageList.Images.SetKeyName(2, "font.jpg")
            Me.ProjectImageList.Images.SetKeyName(3, "Bitmap Image.bmp")
            Me.ProjectImageList.Images.SetKeyName(4, "monitor_go.png")
            '
            'cmnuSolutionExplorer
            '
            Me.cmnuSolutionExplorer.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenAllProjectScreensToolStripMenuItem, Me.CloseAllProjectScreensToolStripMenuItem, Me.TouchAllProjectScreensToolStripMenuItem, Me.RemoveScreenToolStripMenuItem, Me.NewScreenToolStripMenuItem, Me.DuplicateScreenToolStripMenuItem})
            Me.cmnuSolutionExplorer.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
            Me.cmnuSolutionExplorer.Name = "cmnuSolutionExplorer"
            Me.cmnuSolutionExplorer.Size = New System.Drawing.Size(225, 158)
            '
            'OpenAllProjectScreensToolStripMenuItem
            '
            Me.OpenAllProjectScreensToolStripMenuItem.Name = "OpenAllProjectScreensToolStripMenuItem"
            Me.OpenAllProjectScreensToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
            Me.OpenAllProjectScreensToolStripMenuItem.Text = "Open all project screens"
            '
            'CloseAllProjectScreensToolStripMenuItem
            '
            Me.CloseAllProjectScreensToolStripMenuItem.Name = "CloseAllProjectScreensToolStripMenuItem"
            Me.CloseAllProjectScreensToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
            Me.CloseAllProjectScreensToolStripMenuItem.Text = "Close all project screens"
            '
            'RemoveScreenToolStripMenuItem
            '
            Me.RemoveScreenToolStripMenuItem.Name = "RemoveScreenToolStripMenuItem"
            Me.RemoveScreenToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
            Me.RemoveScreenToolStripMenuItem.Text = "Remove Screen from Project"
            '
            'NewScreenToolStripMenuItem
            '
            Me.NewScreenToolStripMenuItem.Name = "NewScreenToolStripMenuItem"
            Me.NewScreenToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
            Me.NewScreenToolStripMenuItem.Text = "New Screen"
            '
            'DuplicateScreenToolStripMenuItem
            '
            Me.DuplicateScreenToolStripMenuItem.Name = "DuplicateScreenToolStripMenuItem"
            Me.DuplicateScreenToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
            Me.DuplicateScreenToolStripMenuItem.Text = "Duplicate Screen"
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.tvSolution)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.lstThumbnails)
            Me.SplitContainer1.Size = New System.Drawing.Size(437, 248)
            Me.SplitContainer1.SplitterDistance = 145
            Me.SplitContainer1.TabIndex = 2
            '
            'lstThumbnails
            '
            Me.lstThumbnails.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lstThumbnails.LargeImageList = Me.imgMiniature
            Me.lstThumbnails.Location = New System.Drawing.Point(0, 0)
            Me.lstThumbnails.Name = "lstThumbnails"
            Me.lstThumbnails.Size = New System.Drawing.Size(288, 248)
            Me.lstThumbnails.TabIndex = 0
            Me.lstThumbnails.UseCompatibleStateImageBehavior = False
            '
            'imgMiniature
            '
            Me.imgMiniature.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
            Me.imgMiniature.ImageSize = New System.Drawing.Size(16, 16)
            Me.imgMiniature.TransparentColor = System.Drawing.Color.Transparent
            '
            'TouchAllProjectScreensToolStripMenuItem
            '
            Me.TouchAllProjectScreensToolStripMenuItem.Name = "TouchAllProjectScreensToolStripMenuItem"
            Me.TouchAllProjectScreensToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
            Me.TouchAllProjectScreensToolStripMenuItem.Text = "Touch all project screens"
            '
            'SolutionExplorer
            '
            Me.ClientSize = New System.Drawing.Size(437, 248)
            Me.CloseButtonVisible = False
            Me.Controls.Add(Me.SplitContainer1)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "SolutionExplorer"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            Me.TabText = "Project Explorer"
            Me.Text = "Project Explorer"
            Me.cmnuSolutionExplorer.ResumeLayout(False)
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Public Sub ClearSolution()
            If Me.tvSolution.IsDisposed Then
                Me.tvSolution = New System.Windows.Forms.TreeView()
            Else
                If tvSolution.Nodes.Count > 0 Then
                    For Each oNode As TreeNode In tvSolution.Nodes(0).Nodes("SCREENS").Nodes
                        oNode.Tag = Nothing
                        'If oNode.Tag IsNot Nothing AndAlso TypeOf oNode.Tag Is VGDD.VGDDScreenAttr Then
                        '    Dim oScreenAttr As VGDD.VGDDScreenAttr = oNode.Tag
                        '    If Not oScreenAttr.Screen.IsDisposed Then oScreenAttr.Screen.Dispose()
                        'End If
                    Next
                End If
                Me.tvSolution.Nodes.Clear()
            End If

            For Each oImage As Image In imgMiniature.Images
                oImage.Dispose()
            Next

            Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Project")
            TreeNode1.ImageIndex = 0
            TreeNode1.SelectedImageIndex = 0
            TreeNode1.Name = "Node0"
            TreeNode1.Text = "Project"
            Me.tvSolution.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1})
            TreeNode1.Nodes.Add("SCREENS", "Screens", 1, 1)
            TreeNode1.Nodes.Add("FONTS", "Fonts", 2, 2)
            TreeNode1.Nodes.Add("BITMAPS", "Bitmaps", 3, 3)
            ProjectPathName = "New"
            lstThumbnails.Items.Clear()
            lstThumbnails.LargeImageList.Images.Clear()
            imgMiniature.Images.Clear()
        End Sub

        Public Sub AddFileNode(ByVal ScreenName As String, ByVal fileName As String, ByVal Section As String, ByVal ImageIndex As Integer)
            Dim RelPath As String = fileName
            If _ProjectFileName IsNot Nothing Then
                RelPath = RelativePath.Evaluate(Path.GetDirectoryName(_ProjectFileName), fileName)
                If RelPath.StartsWith(".\") Then RelPath = RelPath.Substring(2)
            End If
            If Me.tvSolution.Nodes(0).Nodes(Section) Is Nothing Then
                Me.tvSolution.Nodes(0).Nodes.Add(Section.ToUpper, Section)
            End If
            Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(ScreenName) '.Add(fileName)
            Dim tnScreens As TreeNode = Me.tvSolution.Nodes(0).Nodes(Section)
            If oScreenAttr.Screen.Group <> "" Then
                Dim tnGroup As TreeNode = tnScreens.Nodes("GR_" & oScreenAttr.Screen.Group)
                If tnGroup Is Nothing Then
                    tnGroup = tnScreens.Nodes.Add("GR_" & oScreenAttr.Screen.Group, oScreenAttr.Screen.Group)
                    tnGroup.ImageIndex = 4
                End If
                tnScreens = tnGroup
            End If
            Dim tn As TreeNode = tnScreens.Nodes.Add(oScreenAttr.Name, oScreenAttr.Name)
            tn.Tag = oScreenAttr ' fileName 'RelPath
            tn.ImageIndex = ImageIndex
            tn.SelectedImageIndex = ImageIndex

            'Dim oItem As New ListViewItem
            'oItem.Text = oScreenAttr.Screen.Name
            'oItem.ImageKey = oItem.Text
            lstThumbnails.Items.Add(oScreenAttr.Screen.Name, oScreenAttr.Screen.Name, oScreenAttr.Screen.Name)
        End Sub

        Public Sub FontAdd(ByVal strFontName As String)
            If Me.tvSolution.Nodes(0).Nodes("FONTS").Nodes(strFontName) Is Nothing Then
                Dim oNodeFont As TreeNode = Me.tvSolution.Nodes(0).Nodes("FONTS").Nodes.Add(strFontName, strFontName)
                oNodeFont.ImageIndex = 2
                oNodeFont.SelectedImageIndex = oNodeFont.ImageIndex
            End If
        End Sub

        Public Sub FontRemove(ByVal strFontName As String)
            Dim oNodeFont As TreeNode = Me.tvSolution.Nodes(0).Nodes("FONTS").Nodes(strFontName)
            If oNodeFont IsNot Nothing Then
                Me.tvSolution.Nodes.Remove(oNodeFont)
            End If
        End Sub

        Public Sub ChangeGroup(ByVal strScreenName As String, ByVal NewGroup As String)
            Dim tnScreen As TreeNode = tvSolution.Nodes.Find(strScreenName, True)(0)
            Dim atnNewGroup As TreeNode() = tvSolution.Nodes.Find("GR_" & NewGroup, True)
            Dim tnNewGroup As TreeNode
            If NewGroup = String.Empty Then
                tnNewGroup = tvSolution.Nodes(0).Nodes("Screens")
            Else
                If atnNewGroup.Length = 0 Then
                    tnNewGroup = tvSolution.Nodes(0).Nodes("Screens").Nodes.Add("GR_" & NewGroup, NewGroup)
                Else
                    tnNewGroup = atnNewGroup(0)
                End If
            End If
            tvSolution.Nodes.Remove(tnScreen)
            tnNewGroup.Nodes.Insert(0, tnScreen)
            tnNewGroup.ExpandAll()
        End Sub

#Region "DragDrop"

        Private Sub tvSolution_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles tvSolution.DragDrop
            If e.Data.GetDataPresent("System.Windows.Forms.TreeNode", True) = False Then Exit Sub
            Dim selectedTreeview As TreeView = CType(sender, TreeView)
            Dim dropNode As TreeNode = CType(e.Data.GetData("System.Windows.Forms.TreeNode"), TreeNode)
            Dim targetNode As TreeNode = selectedTreeview.SelectedNode
            Dim oDropScreen As VGDD.VGDDScreenAttr = Nothing
            Dim oTargetScreen As VGDD.VGDDScreenAttr = Nothing

            If targetNode Is Nothing OrElse targetNode.Parent Is Nothing OrElse targetNode Is dropNode Then
                Exit Sub
            Else
                If TypeOf dropNode.Tag Is VGDD.VGDDScreenAttr Then
                    oDropScreen = dropNode.Tag
                End If
                If TypeOf targetNode.Tag Is VGDD.VGDDScreenAttr Then
                    oTargetScreen = targetNode.Tag
                End If
                Dim DropMode As Integer = 2
                If oTargetScreen IsNot Nothing AndAlso oDropScreen IsNot Nothing Then
                    If oTargetScreen.Screen.Group <> oDropScreen.Screen.Group Then
                        DropMode = 1
                    ElseIf oTargetScreen.Screen.Group <> "" Or oDropScreen.Screen.Group <> "" Then
                        DropMode = 1
                    End If
                ElseIf oTargetScreen Is Nothing AndAlso targetNode.Parent.Text = "Screens" Then
                    DropMode = 1
                    '            If targetNode.Parent.Text = "Screens" _
                    'Or targetNode.Text = "Screens" _
                End If
                If DropMode = 1 Then
                    If dropNode.Tag IsNot Nothing AndAlso TypeOf dropNode.Tag Is VGDD.VGDDScreenAttr Then
                        FileSelected_FileName = CType(dropNode.Tag, VGDD.VGDDScreenAttr).FileName
                        RaiseEvent FileSelected(Nothing, Nothing)
                        Application.DoEvents()
                    End If
                    dropNode.Remove()
                    If targetNode.Text = "Screens" Then
                        oDropScreen.Screen.Group = ""
                        targetNode.Nodes.Insert(0, dropNode)
                    ElseIf targetNode.Parent.Text = "Screens" Then
                        If oTargetScreen Is Nothing Then
                            oDropScreen.Screen.Group = targetNode.Text
                            targetNode.Nodes.Insert(0, dropNode)
                        Else
                            oDropScreen.Screen.Group = ""
                            targetNode.Parent.Nodes.Add(dropNode)
                        End If
                    Else
                        oDropScreen.Screen.Group = targetNode.Parent.Text
                        targetNode.Parent.Nodes.Add(dropNode)
                    End If
                    oMainShell.ScreenChanged()
                    'Dim oTab As TabPage = MainShell._MainTabs.TabControl.TabPages(oDropScreen.Name)
                    'If oTab IsNot Nothing Then
                    '    oTab.Refresh()
                    'End If
                    If VGDDCommon.Common.CurrentScreen.Name = oDropScreen.Name Then
                        oMainShell.RefreshPropertyGrid()
                    End If
                Else
                    If targetNode.Text = "Screens" Then
                        dropNode.Remove()
                        targetNode.Nodes.Insert(targetNode.Index, dropNode)
                    ElseIf targetNode.Parent IsNot Nothing Then
                        dropNode.Remove()
                        targetNode.Parent.Nodes.Insert(targetNode.Index, dropNode)
                    Else
                        Exit Sub
                    End If
                    Dim aNewScreens As New VGDD.VGDDScreenCollection
                    Dim oNode As TreeNode = Nothing
                    For Each oNode In tvSolution.Nodes(0).Nodes("Screens").Nodes
                        If oNode.Tag IsNot Nothing Then
                            aNewScreens.Add(oNode.Text.ToUpper, Common.aScreens(oNode.Text))
                            Exit For
                        End If
                    Next
                    For Each key As String In Common.aScreens.Keys
                        If key <> oNode.Text.ToUpper Then
                            aNewScreens.Add(key, Common.aScreens(key))
                        End If
                    Next
                    Common.aScreens = aNewScreens
                    oMainShell.ProjectChanged()
                End If
            End If
            'Ensure the newley created node is visible to the user and select it
            dropNode.EnsureVisible()
            selectedTreeview.SelectedNode = dropNode
        End Sub

        Private Sub tvSolution_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles tvSolution.DragEnter
            If e.Data.GetDataPresent("System.Windows.Forms.TreeNode", True) Then
                Dim oNode As TreeNode = CType(e.Data.GetData("System.Windows.Forms.TreeNode"), TreeNode)
                If TypeOf oNode.Tag Is VGDD.VGDDScreenAttr OrElse oNode.Text = "Screens" Then
                    e.Effect = DragDropEffects.Move
                Else
                    e.Effect = DragDropEffects.None
                End If
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub

        Private Sub tvSolution_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles tvSolution.DragOver
            If e.Data.GetDataPresent("System.Windows.Forms.TreeNode", True) = False Then Exit Sub
            Dim selectedTreeview As TreeView = CType(sender, TreeView)

            If selectedTreeview IsNot Me.tvSolution Then Exit Sub
            Dim pt As Point = CType(sender, TreeView).PointToClient(New Point(e.X, e.Y))
            Dim targetNode As TreeNode = selectedTreeview.GetNodeAt(pt)
            If targetNode IsNot Nothing _
                AndAlso ((targetNode.Tag IsNot Nothing AndAlso TypeOf targetNode.Tag Is VGDD.VGDDScreenAttr) Or targetNode.Text = "Screens" Or (targetNode.Parent IsNot Nothing AndAlso targetNode.Parent.Text = "Screens")) Then
                If Not (selectedTreeview.SelectedNode Is targetNode) Then
                    selectedTreeview.SelectedNode = targetNode
                    Dim dropNode As TreeNode = CType(e.Data.GetData("System.Windows.Forms.TreeNode"), TreeNode)
                    Do Until targetNode Is Nothing
                        If targetNode Is dropNode Then
                            e.Effect = DragDropEffects.None
                            Exit Sub
                        End If
                        targetNode = targetNode.Parent
                    Loop
                    e.Effect = DragDropEffects.Move
                End If
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub

        Private Sub tvSolution_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles tvSolution.ItemDrag
            If CType(e.Item, TreeNode).Tag IsNot Nothing Then
                DoDragDrop(e.Item, DragDropEffects.Move)
            End If
        End Sub

#End Region

        Private Sub tvSolution_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvSolution.NodeMouseClick
            tvSolution.SelectedNode = e.Node
            OpenAllProjectScreensToolStripMenuItem.Visible = False
            CloseAllProjectScreensToolStripMenuItem.Visible = False
            TouchAllProjectScreensToolStripMenuItem.Visible = False
            RemoveScreenToolStripMenuItem.Visible = False
            DuplicateScreenToolStripMenuItem.Visible = False
            NewScreenToolStripMenuItem.Visible = False
            If TypeOf (e.Node.Tag) Is VGDDScreenAttr Then
                RemoveScreenToolStripMenuItem.Text = "Remove " & e.Node.Text & " from project"
                RemoveScreenToolStripMenuItem.Visible = True
                DuplicateScreenToolStripMenuItem.Visible = True
                Exit Sub
            ElseIf e.Node.Text = "Screens" AndAlso Common.ProjectName <> "" Then
                OpenAllProjectScreensToolStripMenuItem.Visible = True
                CloseAllProjectScreensToolStripMenuItem.Visible = True
                NewScreenToolStripMenuItem.Visible = True
                TouchAllProjectScreensToolStripMenuItem.Visible = True
            End If
        End Sub

        'Protected Shadows Sub DefWndProc(ByRef m As Message)
        '    If (m.Msg = 515) Then ' WM_LBUTTONDBLCLK 
        '        Debug.Print("")
        '    Else
        '        MyBase.DefWndProc(m)
        '    End If
        'End Sub

        Private Sub treeView1_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvSolution.NodeMouseDoubleClick
            If e.Node.Tag IsNot Nothing AndAlso TypeOf e.Node.Tag Is VGDD.VGDDScreenAttr Then
                'FileSelected_FileName = Path.Combine(Path.GetDirectoryName(_ProjectFileName), CType(e.Node.Tag, VGDD.VGDDScreenAttr).FileName)
                FileSelected_FileName = CType(e.Node.Tag, VGDD.VGDDScreenAttr).FileName
                If Me.Dock = DockStyle.None Then ' TODO: Nascondere il project explorer dopo la selezione
                    'Me.Dock = Me.ShowHint ' non va
                    'Me.DockHandler.DockTo(Me.DockPanel, Me.Dock) ' non va
                End If
                RaiseEvent FileSelected(Nothing, Nothing)
                'Else
                '    ClickedNode = e.Node.Text
                '    RaiseEvent NodeDoubleClick(Nothing, Nothing)
            End If
        End Sub

        Public Property ProjectPathName As String
            Get
                Return _ProjectFileName
            End Get
            Set(ByVal value As String)
                If value IsNot Nothing Then
                    _ProjectFileName = value
                    Try
                        If tvSolution.Nodes.Count > 0 Then
                            tvSolution.Nodes(0).Name = value
                            tvSolution.Nodes(0).Text = Path.GetFileNameWithoutExtension(value)
                        End If
                    Catch ex As Exception
                    End Try
                End If
            End Set
        End Property

        Private Sub OpenAllProjectScreensToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenAllProjectScreensToolStripMenuItem.Click
            RaiseEvent OpenAllScreens(tvSolution, Nothing)
        End Sub

        Private Sub TouchAllProjectScreensToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TouchAllProjectScreensToolStripMenuItem.Click
            RaiseEvent TouchAllScreens(tvSolution, Nothing)
        End Sub

        Private Sub RemoveScreenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RemoveScreenToolStripMenuItem.Click
            RaiseEvent RemoveScreen(tvSolution.SelectedNode, Nothing)
        End Sub

        Private Sub CloseAllProjectScreensToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseAllProjectScreensToolStripMenuItem.Click
            RaiseEvent CloseAllScreens(tvSolution, Nothing)
        End Sub

        Private Sub NewScreenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewScreenToolStripMenuItem.Click
            RaiseEvent NewScreen(tvSolution, Nothing)
        End Sub

        Private Sub DuplicateScreenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DuplicateScreenToolStripMenuItem.Click
            FileSelected_FileName = CType(tvSolution.SelectedNode.Tag, VGDD.VGDDScreenAttr).FileName
            RaiseEvent DuplicateScreen(tvSolution, Nothing)
        End Sub
        Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstThumbnails.DoubleClick
            Dim strScreen As String = lstThumbnails.SelectedItems(0).Text
            If Common.aScreens.ContainsKey(strScreen.ToUpper) Then
                FileSelected_FileName = Common.aScreens(strScreen.ToUpper).FileName
                RaiseEvent FileSelected(Nothing, Nothing)
            Else
                Dim oNode As TreeNode = tvSolution.Nodes(0).Nodes("SCREENS").Nodes(strScreen)
                If oNode IsNot Nothing Then
                    FileSelected_FileName = CType(oNode.Tag, VGDD.VGDDScreenAttr).FileName
                    RaiseEvent FileSelected(Nothing, Nothing)
                End If
            End If
        End Sub

    End Class

    'Public Class NodePaths
    '    Public AbsolutePath As String
    '    Public RelativePath As String
    'End Class

End Namespace