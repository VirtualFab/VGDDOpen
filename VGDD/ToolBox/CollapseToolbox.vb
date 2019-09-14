Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing.Design
Imports System.Windows.Forms.Design
Imports System.IO
Imports System.Reflection
Imports VGDDCommon

Namespace VGDDIDE
    ' <summary>
    ' Toolbox - it implements the IToolboxService to enable
    ' dragging toolbox items onto the host
    ' </summary>
    Public Class CollapseToolbox
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent
        'Inherits System.Windows.Forms.UserControl
        Implements IToolboxService

        ' <summary>
        ' Required designer variable.
        ' </summary>
        Private components As System.ComponentModel.Container = Nothing
        Private m_ToolboxTabCollection As ToolboxTabCollection = Nothing
        Private m_filePath As String = Nothing
        Private m_tabPageArray() As Button = Nothing
        Private selectedIndex As Integer = 0
        Private m_designerHost As IDesignerHost = Nothing
        Private m_toolsListBox As New ArrayList
        Private m_Panels As New Collection
        Private pointer As VgddToolboxItem
        'Private SelectedPanel As Integer = 0
        Private SelectedListbox As ListBox
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Public LastToolBoxItemSelected As VgddToolboxItem

        Public Sub New()
            MyBase.New()
            ' This call is required by the Windows.Forms Form Designer.
            InitializeComponent()
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False

            pointer = New VgddToolboxItem
            pointer.DisplayName = "<Pointer>"
            pointer.Bitmap = My.Resources.POINT13.ToBitmap
            'pointer.Bitmap = New System.Drawing.Bitmap(16, 16)
        End Sub

        Public Sub Clear()

        End Sub

        Public Sub AddPanel(ByVal oTab As ToolboxTab)
            Try
                If oTab.ToolboxItems IsNot Nothing Then
                    Dim oListbox As New ListBox
                    oListbox.Items.Add(pointer)
                    oListbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
                    ' oListbox.BackColor = Color.Green
                    oListbox.ItemHeight = 18
                    'oListbox.Dock = DockStyle.Fill
                    Dim numItems As Integer
                    For Each toolboxItem As VgddToolboxItem In oTab.ToolboxItems
                        Dim type As Type = toolboxItem.Type
                        Dim tbi As VgddToolboxItem = New VgddToolboxItem(type)
                        Dim blnGetDefaultIcon As Boolean = False
                        If type.ToString.Contains("VGDDCustom") Then
                            If toolboxItem.Bitmap IsNot Nothing Then
                                tbi.Bitmap = toolboxItem.Bitmap
                            Else
                                blnGetDefaultIcon = True
                            End If
                        Else
                            blnGetDefaultIcon = True
                        End If
                        If blnGetDefaultIcon Then
                            Dim tba As System.Drawing.ToolboxBitmapAttribute = CType(TypeDescriptor.GetAttributes(type)(GetType(System.Drawing.ToolboxBitmapAttribute)), System.Drawing.ToolboxBitmapAttribute)
                            If (Not (tba) Is Nothing) Then
                                tbi.Bitmap = CType(tba.GetImage(type), System.Drawing.Bitmap)
                            End If
                        End If
                        tbi.DisplayName = toolboxItem.DisplayName
                        oListbox.Items.Add(tbi)
                        numItems += 1
                    Next
                    Dim oPanel As New CollapsablePanel
                    oPanel.PanelTitle = oTab.Name
                    oPanel.BackColor = Me.BackColor
                    oPanel.PanelTitleBackColor = Color.Maroon
                    oPanel.PanelTitleForeColor = Color.White
                    oPanel.ExpandedHeight = oListbox.ItemHeight * (oListbox.Items.Count + 1) + 20

                    'If m_toolsListBox.Count = 0 Then
                    oPanel.PanelState = PanelStateOptions.Expanded
                    'Else
                    'oPanel.PanelState = PanelStateOptions.Collapsed
                    'End If
                    m_toolsListBox.Add(oListbox)
                    AddHandler oListbox.DrawItem, AddressOf Me.list_DrawItem
                    AddHandler oListbox.KeyDown, AddressOf Me.list_KeyDown
                    AddHandler oListbox.MouseDown, AddressOf Me.list_MouseDown
                    oPanel.Controls.Add(oListbox)
                    'oPanel.Top = Me.Controls.Count * 100
                    AddHandler oPanel.StateChanged, AddressOf Me.Panel_StateChanged
                    Me.Panel1.Controls.Add(oPanel)
                    PanelReorg()
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Sub PanelReorg()
            Dim y As Integer = 0
            For i As Integer = 0 To Me.Panel1.Controls.Count - 1
                Dim oPanel As CollapsablePanel = Me.Panel1.Controls(i)
                oPanel.Top = y
                oPanel.Width = Me.ClientSize.Width - 18
                If oPanel.PanelState = PanelStateOptions.Collapsed Then
                    y += 20 '+ 40
                Else
                    y += oPanel.Height
                End If
                Dim oListbox As ListBox = oPanel.Controls(1)
                oListbox.Top = oPanel.Controls(0).Height
                oListbox.Height = oPanel.Height - oListbox.ItemHeight  '- oListbox.Top
                oListbox.Width = oPanel.Width
            Next
        End Sub

        Public Property DesignerHost() As IDesignerHost
            Get
                Return m_designerHost
            End Get
            Set(ByVal value As IDesignerHost)
                m_designerHost = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property Tabs() As ToolboxTabCollection
            Get
                Return m_ToolboxTabCollection
            End Get
            Set(ByVal value As ToolboxTabCollection)
                m_ToolboxTabCollection = value
                Me.Panel1.Controls.Clear()
                For i As Integer = 0 To m_ToolboxTabCollection.Count - 1
                    Me.AddPanel(m_ToolboxTabCollection(i))
                Next
            End Set
        End Property

        Public Property FilePath() As String
            Get
                Return m_filePath
            End Get
            Set(ByVal value As String)
                m_filePath = value
                InitializeToolbox()
            End Set
        End Property

        'Friend Property TabPageArray() As Button()
        '    Get
        '        Return m_tabPageArray
        '    End Get
        '    Set(ByVal value As Button())
        '        m_tabPageArray = value
        '    End Set
        'End Property

        'Friend Property ToolsListBox() As ListBox
        '    Get
        '        Return m_toolsListBox
        '    End Get
        '    Set(ByVal value As ListBox)
        '        m_toolsListBox = value
        '    End Set
        'End Property

        Public ReadOnly Property CategoryNames() As CategoryNameCollection Implements IToolboxService.CategoryNames
            Get
                Return Nothing
            End Get
        End Property

        Public Property SelectedCategory() As String Implements IToolboxService.SelectedCategory
            Get
                Return Nothing
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        ' <summary>
        ' Clean up any resources being used.
        ' </summary>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
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
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.SuspendLayout()
            '
            'Panel1
            '
            Me.Panel1.AutoScroll = True
            Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(165, 497)
            Me.Panel1.TabIndex = 0
            '
            'CollapseToolbox
            '
            Me.ClientSize = New System.Drawing.Size(165, 497)
            Me.Controls.Add(Me.Panel1)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "CollapseToolbox"
            Me.ResumeLayout(False)

        End Sub

        Public Sub InitializeToolbox()
            Try
                Dim toolboxXmlManager As ToolboxXmlManager = New ToolboxXmlManager(Me)
                Tabs = toolboxXmlManager.PopulateToolboxInfo
            Catch ex As Exception

            End Try

            'Dim toolboxUIManagerVS As ToolboxUIManagerVS = New ToolboxUIManagerVS(Me)
            'toolboxUIManagerVS.FillToolbox()
            'AddEventHandlers()
            'PrintToolbox()
        End Sub

        'Private Sub PrintToolbox()
        '    Try
        '        Dim i As Integer = 0
        '        Do While (i < Tabs.Count)
        '            Console.WriteLine(Tabs(i).Name)
        '            Dim j As Integer = 0
        '            Do While (j < Tabs(i).VgddToolboxItems.Count)
        '                Console.WriteLine(("" & vbTab + Tabs(i).VgddToolboxItems(j).Type.ToString))
        '                j = (j + 1)
        '            Loop
        '            Console.WriteLine(" ")
        '            i = (i + 1)
        '        Loop
        '    Catch ex As Exception
        '        MessageBox.Show(ex.ToString)
        '    End Try
        'End Sub

        'Private Sub AddEventHandlers()
        '    AddHandler ToolsListBox.KeyDown, AddressOf Me.list_KeyDown
        '    AddHandler ToolsListBox.MouseDown, AddressOf Me.list_MouseDown
        '    AddHandler ToolsListBox.DrawItem, AddressOf Me.list_DrawItem
        'End Sub


        Private Sub Panel_StateChanged() '(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs)
            PanelReorg()
        End Sub

        Private Sub list_DrawItem(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs)
            Try
                Dim lbSender As ListBox = CType(sender, ListBox)
                If (lbSender Is Nothing) Then
                    Return
                End If
                ' If this tool is the currently selected tool, draw it with a highlight.
                If (selectedIndex = e.Index) Then
                    e.Graphics.FillRectangle(Brushes.LightSlateGray, e.Bounds)
                End If
                Dim tbi As VgddToolboxItem = CType(lbSender.Items(e.Index), VgddToolboxItem)
                Dim BitmapBounds As Rectangle = New Rectangle(e.Bounds.Location.X, (e.Bounds.Location.Y _
                                + ((e.Bounds.Height / 2) _
                                - (tbi.Bitmap.Height / 2))), tbi.Bitmap.Width, tbi.Bitmap.Height)
                Dim StringBounds As Rectangle = New Rectangle((e.Bounds.Location.X _
                                + (BitmapBounds.Width + 5)), e.Bounds.Location.Y, (e.Bounds.Width - BitmapBounds.Width), e.Bounds.Height)
                Dim format As StringFormat = New StringFormat
                format.LineAlignment = StringAlignment.Center
                format.Alignment = StringAlignment.Near
                e.Graphics.DrawImage(tbi.Bitmap, BitmapBounds)
                'e.Graphics.DrawString(tbi.DisplayName, New Font("Tahoma", 11, FontStyle.Regular, GraphicsUnit.World), Brushes.Black, StringBounds, format)
                e.Graphics.DrawString(tbi.DisplayName, Me.Font, Brushes.Black, StringBounds, format)
            Catch ex As Exception
                ex.ToString()
            End Try
        End Sub

        Private Sub list_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            Try
                If Me.DesignerHost Is Nothing Then Exit Sub
                Dim lbSender As ListBox = CType(sender, ListBox)
                SelectedListbox = lbSender
                Dim lastSelectedBounds As Rectangle = lbSender.GetItemRectangle(0)
                Try
                    If selectedIndex >= 0 Then
                        Try
                            lastSelectedBounds = lbSender.GetItemRectangle(selectedIndex)
                        Catch ex As Exception
                        End Try
                    End If
                Catch ex As Exception
                    ex.ToString()
                End Try
                selectedIndex = lbSender.IndexFromPoint(e.X, e.Y)
                ' change our selection
                lbSender.SelectedIndex = selectedIndex
                lbSender.Invalidate(lastSelectedBounds)
                ' clear highlight from last selection
                If selectedIndex >= 0 Then
                    lbSender.Invalidate(lbSender.GetItemRectangle(selectedIndex))
                    LastToolBoxItemSelected = lbSender.Items(selectedIndex)
                    ' highlight new one
                    If (selectedIndex <> 0) Then
                        If (e.Clicks = 2) Then
                            Dim idh As IDesignerHost = CType(Me.DesignerHost.GetService(GetType(IDesignerHost)), IDesignerHost)
                            Dim tbu As IToolboxUser = CType(idh.GetDesigner(CType(idh.RootComponent, IComponent)), IToolboxUser)
                            If (Not (tbu) Is Nothing) Then
                                Try
                                    If tbu.GetToolSupported(LastToolBoxItemSelected) Then
                                        'For Each o As Windows.Forms.Design.Behavior.Adorner In MainShell._CurrentHost._BehaviorService.Adorners
                                        '    o.Enabled = False
                                        'Next
                                        Common.ControlAdding = True
                                        tbu.ToolPicked(LastToolBoxItemSelected)
                                        Common.ControlAdding = False
                                    End If
                                Catch ex As Exception
                                End Try
                            End If
                        ElseIf (e.Clicks < 2) Then
                            'Dim tbi As VgddToolboxItem = CType(lbSender.Items(selectedIndex), VgddToolboxItem)
                            Dim tbs As IToolboxService = Me
                            ' The IToolboxService serializes ToolboxItems by packaging them in DataObjects.
                            Dim d As DataObject = CType(tbs.SerializeToolboxItem(LastToolBoxItemSelected), DataObject)
                            Try
                                Common.ControlAdding = True
                                lbSender.DoDragDrop(d, DragDropEffects.Copy)
                                Common.ControlAdding = False
                            Catch ex As Exception
                                MessageBox.Show(ex.Message, "Error adding Widget", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Try
                        End If
                    End If
                End If
            Catch ex As Exception
                ex.ToString()
            End Try
        End Sub

        Private Sub list_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
            Try
                Dim lbSender As ListBox = CType(sender, ListBox)
                Dim lastSelectedBounds As Rectangle = lbSender.GetItemRectangle(0)
                Try
                    lastSelectedBounds = lbSender.GetItemRectangle(selectedIndex)
                Catch ex As Exception
                    ex.ToString()
                End Try
                Select Case (e.KeyCode)
                    Case Keys.Up
                        If (selectedIndex > 0) Then
                            selectedIndex = (selectedIndex - 1)
                            ' change selection
                            lbSender.SelectedIndex = selectedIndex
                            lbSender.Invalidate(lastSelectedBounds)
                            ' clear old highlight
                            lbSender.Invalidate(lbSender.GetItemRectangle(selectedIndex))
                            ' add new one
                        End If
                    Case Keys.Down
                        If (selectedIndex + 1) < lbSender.Items.Count Then
                            selectedIndex = (selectedIndex + 1)
                            ' change selection
                            lbSender.SelectedIndex = selectedIndex
                            lbSender.Invalidate(lastSelectedBounds)
                            ' clear old highlight
                            lbSender.Invalidate(lbSender.GetItemRectangle(selectedIndex))
                            ' add new one
                        End If
                    Case Keys.Enter
                        If (DesignerHost Is Nothing) Then
                            MessageBox.Show("idh Null")
                        End If
                        Dim tbu As IToolboxUser = CType(DesignerHost.GetDesigner(CType(DesignerHost.RootComponent, IComponent)), IToolboxUser)
                        If (Not (tbu) Is Nothing) Then
                            ' Enter means place the tool with default location and default size.
                            tbu.ToolPicked(CType(lbSender.Items(selectedIndex), VgddToolboxItem))
                            lbSender.Invalidate(lastSelectedBounds)
                            ' clear old highlight
                            lbSender.Invalidate(lbSender.GetItemRectangle(selectedIndex))
                            ' add new one
                        End If
                    Case Else
                        Console.WriteLine("Error: Not able to add")
                        Exit Select
                End Select
                ' switch
            Catch ex As Exception
                MessageBox.Show(ex.ToString, "CollapseToolbox Error")
            End Try
        End Sub

        ' We only implement what is really essential for ToolboxService
        Public Overloads Function GetSelectedToolboxItem(ByVal host As IDesignerHost) As ToolboxItem Implements IToolboxService.GetSelectedToolboxItem
            Dim list As ListBox = SelectedListbox 'm_toolsListBox(SelectedPanel)
            If list Is Nothing Then Return Nothing
            Dim tbi As VgddToolboxItem = CType(list.Items(selectedIndex), VgddToolboxItem)
            If (Not tbi.DisplayName Is "<Pointer>") Then
                Return tbi
            Else
                Return Nothing
            End If
        End Function

        Public Overloads Function GetSelectedToolboxItem() As ToolboxItem Implements IToolboxService.GetSelectedToolboxItem
            Return Me.GetSelectedToolboxItem(Nothing)
        End Function

        Public Overloads Sub AddToolboxItem(ByVal toolboxItem As ToolboxItem, ByVal category As String) Implements IToolboxService.AddToolboxItem

        End Sub

        Public Overloads Sub AddToolboxItem(ByVal toolboxItem As ToolboxItem) Implements IToolboxService.AddToolboxItem

        End Sub

        Public Overloads Function IsToolboxItem(ByVal serializedObject As Object, ByVal host As IDesignerHost) As Boolean Implements IToolboxService.IsToolboxItem
            Return False
        End Function

        Public Overloads Function IsToolboxItem(ByVal serializedObject As Object) As Boolean Implements IToolboxService.IsToolboxItem
            Return False
        End Function

        Public Sub SetSelectedToolboxItem(ByVal toolboxItem As ToolboxItem) Implements IToolboxService.SetSelectedToolboxItem

        End Sub

        Public Sub SelectedToolboxItemUsed() Implements IToolboxService.SelectedToolboxItemUsed
            Dim list As ListBox = SelectedListbox 'm_toolsListBox(SelectedPanel)
            list.Invalidate(list.GetItemRectangle(selectedIndex))
            selectedIndex = 0
            list.SelectedIndex = 0
            list.Invalidate(list.GetItemRectangle(selectedIndex))
        End Sub

        Sub IToolboxService_Refresh() Implements IToolboxService.Refresh

        End Sub

        Public Overloads Sub AddLinkedToolboxItem(ByVal toolboxItem As ToolboxItem, ByVal host As IDesignerHost) Implements IToolboxService.AddLinkedToolboxItem

        End Sub

        Public Overloads Sub AddLinkedToolboxItem(ByVal toolboxItem As ToolboxItem, ByVal category As String, ByVal host As IDesignerHost) Implements IToolboxService.AddLinkedToolboxItem

        End Sub

        Public Overloads Function IsSupported(ByVal serializedObject As Object, ByVal filterAttributes As ICollection) As Boolean Implements IToolboxService.IsSupported
            Return False
        End Function

        Public Overloads Function IsSupported(ByVal serializedObject As Object, ByVal host As IDesignerHost) As Boolean Implements IToolboxService.IsSupported
            Return False
        End Function

        Public Overloads Function DeserializeToolboxItem(ByVal serializedObject As Object, ByVal host As IDesignerHost) As ToolboxItem Implements IToolboxService.DeserializeToolboxItem
            Return CType(CType(serializedObject, DataObject).GetData(GetType(VgddToolboxItem)), VgddToolboxItem)
        End Function

        Public Overloads Function DeserializeToolboxItem(ByVal serializedObject As Object) As ToolboxItem Implements IToolboxService.DeserializeToolboxItem
            Return Me.DeserializeToolboxItem(serializedObject, Me.DesignerHost)
        End Function

        Public Function GetToolboxItems() As System.Drawing.Design.ToolboxItemCollection Implements System.Drawing.Design.IToolboxService.GetToolboxItems
            Return Nothing
        End Function

        Public Function GetToolboxItems(ByVal category As String) As System.Drawing.Design.ToolboxItemCollection Implements System.Drawing.Design.IToolboxService.GetToolboxItems
            Return Nothing
        End Function

        Public Function GetToolboxItems(ByVal category As String, ByVal host As System.ComponentModel.Design.IDesignerHost) As System.Drawing.Design.ToolboxItemCollection Implements System.Drawing.Design.IToolboxService.GetToolboxItems
            Return Nothing
        End Function

        Public Function GetToolboxItems(ByVal host As System.ComponentModel.Design.IDesignerHost) As System.Drawing.Design.ToolboxItemCollection Implements System.Drawing.Design.IToolboxService.GetToolboxItems
            Return Nothing
        End Function

        Public Overloads Sub AddCreator(ByVal creator As ToolboxItemCreatorCallback, ByVal format As String, ByVal host As IDesignerHost) Implements IToolboxService.AddCreator

        End Sub

        Public Overloads Sub AddCreator(ByVal creator As ToolboxItemCreatorCallback, ByVal format As String) Implements IToolboxService.AddCreator

        End Sub

        Public Function SetCursor() As Boolean Implements IToolboxService.SetCursor
            Return False
        End Function

        Public Overloads Sub RemoveToolboxItem(ByVal toolboxItem As ToolboxItem, ByVal category As String) Implements IToolboxService.RemoveToolboxItem

        End Sub

        Public Overloads Sub RemoveToolboxItem(ByVal toolboxItem As ToolboxItem) Implements IToolboxService.RemoveToolboxItem

        End Sub

        Public Function SerializeToolboxItem(ByVal toolboxItem As ToolboxItem) As Object Implements IToolboxService.SerializeToolboxItem
            Return New DataObject(toolboxItem)
        End Function

        Public Overloads Sub RemoveCreator(ByVal format As String, ByVal host As IDesignerHost) Implements IToolboxService.RemoveCreator

        End Sub

        Public Overloads Sub RemoveCreator(ByVal format As String) Implements IToolboxService.RemoveCreator

        End Sub

        Private Sub CollapseToolbox_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitializeToolbox()
        End Sub

        Private Sub CollapseToolbox_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            PanelReorg()
        End Sub

    End Class
End Namespace