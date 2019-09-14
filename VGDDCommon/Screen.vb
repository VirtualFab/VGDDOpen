Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common
Imports VGDDMicrochip.VGDDBase

Namespace VGDD

#If Not PlayerMonolitico Then
    <ToolboxBitmap(GetType(Button), "ScreenIco"),
    DesignerAttribute(GetType(VGDD.VGDDControlDesigner)), _
    Designer(GetType(VGDD.VGDDControlDesigner))> _
    Public Class VGDDScreen
#Else
    Public Class VGDDScreen
#End If
        'Inherits Windows.Forms.Form
        Inherits UserControl
        Implements ICustomTypeDescriptor, IVGDDEvents

        Private Shared _Instances As Integer = 0

        Private _TransparentColour As Color = Nothing
        Public _FileName As String
        Private _Overlay As Boolean = False
        Private _GolFree As Boolean = True
        Private _MasterScreens As New Collection
        Private _IsMasterScreen As Boolean
        Private _ShowMasterScreens As Boolean
        Private _MyPropsToRemove As String = ""
        Private _Group As String = ""
        Private _Events As VGDDEventsCollection
        Private _PaletteName As String

        Public Sub New()
            'Me.ControlBox = False
            'Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
            Me.BorderStyle = BorderStyle.FixedSingle
            Me.BackColor = Color.White
            RemovePropertyToShow("Locked")
            If Not Common.ProjectUsePalette Then
                RemovePropertyToShow("Palette")
            End If
            _Events = New VGDDEventsCollection
#If Not PlayerMonolitico Then
            For Each oTemplateEvent As VGDDEvent In CodeGen.GetEventsFromTemplate("Screen")
                If _Events(oTemplateEvent.Name) Is Nothing Then
                    _Events.Add(oTemplateEvent)
                Else
                    _Events(oTemplateEvent.Name).Description = oTemplateEvent.Description
                    _Events(oTemplateEvent.Name).PlayerEvent = oTemplateEvent.PlayerEvent
                End If
            Next
#End If
            If Not Common.ProjectUsePalette Then
                RemovePropertyToShow("PaletteName")
            End If

        End Sub

        Public Sub AddPropertyToShow(ByVal PropertyName As String)
            PropertyName = " " & PropertyName.Trim & " "
            If _MyPropsToRemove.Contains(PropertyName) Then
                _MyPropsToRemove = " " & _MyPropsToRemove.Replace(PropertyName, "").Trim & " "
            End If
        End Sub

        Public Sub RemovePropertyToShow(ByVal PropertyName As String)
            PropertyName = " " & PropertyName.Trim & " "
            If Not _MyPropsToRemove.Contains(PropertyName) Then
                _MyPropsToRemove = " " & _MyPropsToRemove.Trim & " " & PropertyName
            End If
        End Sub

        '<System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso Not Me.IsDisposed Then
                _Events.Clear()
                _MasterScreens.Clear()
                If Not Me.IsDisposed Then
                    Try
                        MyBase.Dispose(disposing)
                    Catch ex As Exception
                    End Try
                End If
            End If
        End Sub

        Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
            MyBase.OnPaintBackground(pevent)
            If _ShowMasterScreens AndAlso _MasterScreens IsNot Nothing AndAlso _MasterScreens.Count > 0 Then
                For Each strMasterScreen As String In _MasterScreens
                    If Common.aScreens.ContainsKey(strMasterScreen.ToUpper) Then
                        Dim oMasterScreen As VGDDScreen = Common.aScreens(strMasterScreen).Screen
                        If oMasterScreen IsNot Nothing Then
                            Using oScreenshot As Bitmap = Common.Render2Bitmap(oMasterScreen, oMasterScreen.Width, oMasterScreen.Height)
                                'oScreenshot.MakeTransparent(oMasterScreen.BackColor)
                                Try
                                    pevent.Graphics.DrawImage(oScreenshot, Point.Empty)
                                Catch ex As Exception
                                    Debug.Print("!")
                                End Try
                            End Using
                        End If
                    End If
                Next
            End If
        End Sub

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Try
                Dim g As Graphics = pevent.Graphics
                Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(Me)("Locked")
                If oProp.GetValue(Me) = False Then
                    oProp.SetValue(Me, True)
                End If

                MyBase.OnPaint(pevent)

            Catch ex As Exception

            End Try
        End Sub

        <TypeConverter(GetType(Common.PaletteOptionConverter))> _
        <Description("Name of the palette to use on all objects on this screen")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property PaletteName As String
            Get
                Return _PaletteName
            End Get
            Set(ByVal value As String)
                _PaletteName = value
            End Set
        End Property

        <Description("Is this a Master Screen?")> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property IsMasterScreen As Boolean
            Get
                Return (_IsMasterScreen)
            End Get
            Set(ByVal value As Boolean)
                _IsMasterScreen = value
                If _IsMasterScreen Then
                    Me.Overlay = True
                    RemovePropertyToShow("MasterScreens")
                    RemovePropertyToShow("ShowMasterScreens")
                    RemovePropertyToShow("Overlay")
                    RemovePropertyToShow("GolFree")
                Else
                    AddPropertyToShow("MasterScreens")
                    AddPropertyToShow("ShowMasterScreens")
                    AddPropertyToShow("Overlay")
                    AddPropertyToShow("GolFree")
                End If
            End Set
        End Property

        <Description("Show Master Screens Widgets?")> _
        <DefaultValue(True)> _
        <Category("Design")> _
        Public Property ShowMasterScreens As Boolean
            Get
                Return (_ShowMasterScreens)
            End Get
            Set(ByVal value As Boolean)
                _ShowMasterScreens = value
                Me.Invalidate()
            End Set
        End Property

#If PlayerMonolitico Then
        Public Property MasterScreens As Collection
#Else
        <DefaultValue(""), EditorAttribute(GetType(MasterScreensPropertyEditor), GetType(System.Drawing.Design.UITypeEditor))> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
        <Description("Master Screens for this screen")> _
        <CustomSortedCategory("Main", 2)> _
        Public Property MasterScreens As Collection
#End If
            Get
                Return (_MasterScreens)
            End Get
            Set(ByVal value As Collection)
                _MasterScreens = value
                If value IsNot Nothing AndAlso value.Count > 0 Then
                    _MyPropsToRemove = " IsMasterScreen"
                    _IsMasterScreen = False
                    _ShowMasterScreens = True
                Else
                    _MyPropsToRemove = ""
                End If
            End Set
        End Property

#Region "GDDProps"

        <Description("Object Type")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        <CustomSortedCategory("Info", 1)> _
        Public ReadOnly Property ObjectType() As String
            Get
                Return "Screen"
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        <DesignOnly(True)> _
        Public Shadows Property Locked As Boolean
            Get
                Return True
            End Get
            Set(ByVal value As Boolean)
                'Debug.Print("")
            End Set
        End Property

        <Description("Event handling for this Screen")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDEventsEditorNew), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property VGDDEvents() As VGDDEventsCollection Implements IVGDDEvents.VGDDEvents
            Get
                Return _Events
            End Get
            Set(ByVal value As VGDDEventsCollection)
                _Events = value
            End Set
        End Property

        '<EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        'Public Property ScreenName() As String
        '    Get
        '        Return MyBase.Name
        '    End Get
        '    Set(ByVal value As String)
        '        MyBase.Name = value
        '    End Set
        'End Property

        <Description("Name for this Screen")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Overloads Property Name() As String Implements IVGDDEvents.Name
            Get
                Return MyBase.Name
            End Get
            Set(ByVal value As String)
                MyBase.Name = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Shadows Property ClientSize As Size
            Get
                Return MyBase.ClientSize 'New Size(MyBase.ClientSize.Width - 2, MyBase.ClientSize.Height - 2)
            End Get
            Set(ByVal value As Size)
                MyBase.ClientSize = New Size(value.Width + 2, value.Height + 2)
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Shadows Property Size As Size
            Get
                Return MyBase.Size 'New Size(MyBase.Size.Width - 2, MyBase.Height - 2)
            End Get
            Set(ByVal value As Size)
                MyBase.Size = New Size(value.Width + 2, value.Height + 2) 'value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Overloads Property Width As Integer
            Get
                Return MyBase.Width - 2
            End Get
            Set(ByVal value As Integer)
                MyBase.Width = value + 2
                Me.Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Overloads Property Height As Integer
            Get
                Return MyBase.Height - 2
            End Get
            Set(ByVal value As Integer)
                MyBase.Height = value + 2
                Me.Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Description("Group name this Screen belongs to (Optional)")> _
        <CustomSortedCategory("Main", 2)> _
        Public Property Group As String
            Get
                Return _Group
            End Get
            Set(ByVal value As String)
                _Group = value.Trim
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Info", 1)> _
        Public ReadOnly Property FileName() As String
            Get
                Return _FileName
            End Get
        End Property

#If Not PlayerMonolitico Then
        <Description("BackGround Color for the Screen")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property BackColor() As Color
#Else
        Public Overloads Property BackColor() As Color
#End If
            Get
                Return MyBase.BackColor
            End Get
            Set(ByVal value As Color)
                MyBase.BackColor = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Do a GOLFree() on creating it?" & vbCrLf & "Set it to false to keep previous screen GOL objects ""alive""." & vbCrLf & "WARNING: Heap fragmentation and/or underrun may occur when using this feature.")> _
        <DefaultValue(True)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property GolFree As Boolean
            Get
                Return (_GolFree)
            End Get
            Set(ByVal value As Boolean)
                _GolFree = value
            End Set
        End Property

        <Description("Clear Screen with <BackColor> on creating it?" & vbCrLf & "Set it to true to have an overlaying screen.")> _
        <DefaultValue(False)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property Overlay As Boolean
            Get
                Return (_Overlay)
            End Get
            Set(ByVal value As Boolean)
                _Overlay = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Transparent Colour")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <DefaultValue(GetType(Color), "Color.Empty")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property TransparentColour As Color
#Else
        Public Property TransparentColour As Color
#End If
            Get
                Return (_TransparentColour)
            End Get
            Set(ByVal value As Color)
                _TransparentColour = value
                For Each oControl As Control In Me.Controls
                    If TypeOf (oControl) Is IVGDDWidgetWithBitmap Then
                        CType(oControl, IVGDDWidgetWithBitmap).ScaleBitmap()
                    End If
                Next
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Shadows Property Location() As Point
            Get
                Return MyBase.Location
            End Get
            Set(ByVal value As Point)
                MyBase.Location = value
            End Set
        End Property

        '<EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        'Public Shadows Property DoubleBuffered() As Boolean
        '    Get
        '        Return MyBase.DoubleBuffered
        '    End Get
        '    Set(ByVal value As Boolean)
        '        MyBase.DoubleBuffered = value
        '    End Set
        'End Property

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                '#If DEBUG Then
                '                If pd.Name.ToUpper.StartsWith("DOUBLE") Then
                '                    Debug.Print("")
                '                End If
                '#End If
                If Not (Common.PROPSTOREMOVE & " MasterPagesBitmap Locked DoubleBuffered ShowInTaskbar Font" & _MyPropsToRemove & " ").Contains(" " & pd.Name & " ") Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

#End Region

#Region "ICustomTypeDescriptor Members"

        Private Function GetProperties(ByVal attributes() As Attribute) As PropertyDescriptorCollection _
            Implements ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, attributes, True)
            Return FilterProperties(pdc)
        End Function

        Private Function GetProperties() As PropertyDescriptorCollection _
            Implements ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, True)
            Return FilterProperties(pdc)
        End Function

        Private Function GetEvents(ByVal attributes As Attribute()) As EventDescriptorCollection _
            Implements ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, attributes, True)
        End Function

        Public Function GetConverter() As TypeConverter _
            Implements ICustomTypeDescriptor.GetConverter
            Return TypeDescriptor.GetConverter(Me, True)
        End Function

        Private Function System_ComponentModel_ICustomTypeDescriptor_GetEvents() As EventDescriptorCollection _
            Implements ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, True)
        End Function

        Public Function GetComponentName() As String _
            Implements ICustomTypeDescriptor.GetComponentName
            Return TypeDescriptor.GetComponentName(Me, True)
        End Function

        <System.Diagnostics.DebuggerStepThrough()> _
        Public Function GetPropertyOwner(ByVal pd As PropertyDescriptor) As Object _
        Implements ICustomTypeDescriptor.GetPropertyOwner
            Return Me
        End Function

        Public Function GetAttributes() As AttributeCollection _
            Implements ICustomTypeDescriptor.GetAttributes
            Return TypeDescriptor.GetAttributes(Me, True)
        End Function

        Public Function GetEditor(ByVal editorBaseType As Type) As Object _
            Implements ICustomTypeDescriptor.GetEditor
            Return TypeDescriptor.GetEditor(Me, editorBaseType, True)
        End Function

        Public Function GetDefaultProperty() As PropertyDescriptor _
            Implements ICustomTypeDescriptor.GetDefaultProperty
            Return TypeDescriptor.GetDefaultProperty(Me, True)
        End Function

        Public Function GetDefaultEvent() As EventDescriptor _
            Implements ICustomTypeDescriptor.GetDefaultEvent
            Return TypeDescriptor.GetDefaultEvent(Me, True)
        End Function

        Public Function GetClassName() As String _
            Implements ICustomTypeDescriptor.GetClassName
            Return TypeDescriptor.GetClassName(Me, True)
        End Function

#End Region

        'Private Sub VGDDScreen_DockChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DockChanged
        '    Debug.Print(Me.Name & " DockChanged")
        'End Sub

        'Private Sub VGDDScreen_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        '    If Me.Opacity = 0 Then
        '        FadeForm.FadeIn(Me, 99)
        '    End If
        'End Sub

        'Private Sub VGDDScreen_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '    FadeForm.FadeOutAndWait(Me)
        'End Sub

        'Private Sub VGDDScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '    If Common.DoFadings Then Me.Opacity = 0
        'End Sub

    End Class

#If Not PlayerMonolitico Then
    Public Class MasterScreensPropertyEditor
        Inherits CustomPropertyEditorBase

        Private WithEvents myListView As New ListView 'this is the control to be used in design time DropDown editor

        Protected Overrides Function GetEditControl(ByVal PropertyName As String, ByVal CurrentValue As Object) As System.Windows.Forms.Control
            Try
                Dim aObjectMasterScreens As Collection = CurrentValue
                With myListView
                    .BorderStyle = System.Windows.Forms.BorderStyle.None
                    .CheckBoxes = True
                    .View = View.List
                End With

                Dim aProjectMasterScreens As New Collection
                For Each oScreenAttr As VGDDScreenAttr In Common.aScreens.Values
                    If oScreenAttr.Screen IsNot Nothing AndAlso oScreenAttr.Screen.IsMasterScreen Then
                        aProjectMasterScreens.Add(oScreenAttr.Screen.Name, oScreenAttr.Screen.Name)
                    End If
                Next

                Dim aCheckedObjectMasterScreens As New Collection
                If aObjectMasterScreens IsNot Nothing Then
                    For Each ObjectMasterScreenName As String In aObjectMasterScreens
                        If aProjectMasterScreens.Contains(ObjectMasterScreenName) Then
                            aCheckedObjectMasterScreens.Add(ObjectMasterScreenName, ObjectMasterScreenName)
                        End If
                    Next
                End If

                myListView.Items.Clear() 'clear previous items if any
                For Each ProjectMasterScreenName As String In aProjectMasterScreens
                    Dim oItem As New ListViewItem(ProjectMasterScreenName)
                    If aCheckedObjectMasterScreens IsNot Nothing AndAlso aCheckedObjectMasterScreens.Contains(ProjectMasterScreenName) Then
                        oItem.Checked = True
                    End If
                    myListView.Items.Add(oItem)
                Next
                'myListBox.SelectedIndex = myListBox.FindString(CurrentValue) 'Select current item based on CurrentValue of the property
                'myListBox.Height = myListBox.PreferredHeight
            Catch ex As Exception
            End Try
            Return myListView
        End Function
#If Not PlayerMonolitico Then

        Protected Overrides Function GetEditedValue(ByVal EditControl As System.Windows.Forms.Control, ByVal PropertyName As String, ByVal OldValue As Object) As Object
            Dim aMasterScreens As New Collection
            For i As Integer = 0 To myListView.CheckedItems.Count - 1
                aMasterScreens.Add(myListView.CheckedItems(i).Text)
            Next
            Return aMasterScreens
        End Function

        Private Sub myListBox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles myListView.Click
            'Me.CloseDropDownWindow() 'when user clicks on an item, the edit process is done.
        End Sub
#End If
    End Class
#End If

    Public Class VGDDScreenAttr
        Private _Screen As VGDDScreen
        Private _FileName As String
        Private _Name As String
        Private _EventsCode As String
        Private _EventsUpdateCode As String
        Private _Hc As Object
        Private _Shown As Boolean

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Shadows Property Shown As Boolean
            Get
                Return _Shown
            End Get
            Set(ByVal value As Boolean)
                _Shown = value
            End Set
        End Property

        Public Property Hc As Object
            Get
                Return _Hc
            End Get
            Set(ByVal value As Object)
                _Hc = value
            End Set
        End Property

        Public Property EventsCode As String
            Get
                Return _EventsCode
            End Get
            Set(ByVal value As String)
                _EventsCode = value
            End Set
        End Property

        Public Property EventsUpdateCode As String
            Get
                Return _EventsUpdateCode
            End Get
            Set(ByVal value As String)
                _EventsUpdateCode = value
            End Set
        End Property

        Public Property Screen As VGDDScreen
            Get
                Return _Screen
            End Get
            Set(ByVal value As VGDDScreen)
                _Screen = value
            End Set
        End Property

        Public Property FileName As String
            Get
                Return _FileName
            End Get
            Set(ByVal value As String)
                _FileName = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = Common.CleanName(value)
            End Set
        End Property

        Protected Overrides Sub Finalize()
            _Screen = Nothing
            _EventsCode = Nothing
            MyBase.Finalize()
        End Sub
    End Class

    Public Class VGDDScreenCollection
        Inherits Collections.Generic.Dictionary(Of String, VGDDScreenAttr)

        Public Overloads Function Add(ByVal NewScreenFileName As String, ByVal NewScreenName As String, ByRef oNewScreen As VGDDScreen, ByVal blnShow As Boolean) As VGDDScreenAttr
            Dim NewScreenAttr As VGDDScreenAttr
            If MyBase.ContainsKey(NewScreenName.ToUpper) Then
                NewScreenAttr = MyBase.Item(NewScreenName.ToUpper)
                NewScreenAttr.FileName = NewScreenFileName
                NewScreenAttr.Screen = oNewScreen
                NewScreenAttr.Shown = blnShow
            Else
                NewScreenAttr = New VGDDScreenAttr
                NewScreenAttr.Name = NewScreenName
                NewScreenAttr.FileName = NewScreenFileName
                NewScreenAttr.Screen = oNewScreen
                NewScreenAttr.Shown = blnShow
                MyBase.Add(NewScreenAttr.Name.ToUpper, NewScreenAttr)
            End If
            Return NewScreenAttr
        End Function

        Public Overloads Function Add(ByVal NewScreenFileName As String, ByVal NewScreenName As String) As VGDDScreenAttr
            Return Add(NewScreenFileName, NewScreenName, CType(Nothing, VGDDScreen), False)
        End Function

        Public Overloads Function Add(ByVal NewScreenFileName As String, ByVal blnShow As Boolean) As VGDDScreenAttr
            Dim strScreenName As String = Common.ReadScreenName(NewScreenFileName)
            Return Add(NewScreenFileName, strScreenName, CType(Nothing, VGDDScreen), blnShow)
        End Function

        Public Overloads Function Add(ByVal NewScreenFileName As String, ByRef oNewScreen As VGDDScreen) As VGDDScreenAttr
            Dim strScreenName As String = Common.ReadScreenName(NewScreenFileName)
            Return Add(NewScreenFileName, strScreenName, oNewScreen, False)
        End Function

        Default Public Overloads Property Item(ByVal ScreenName As String) As VGDDScreenAttr
            Get
                If ScreenName Is Nothing Then Return Nothing
                If MyBase.ContainsKey(ScreenName.ToUpper) Then
                    Return DirectCast(MyBase.Item(ScreenName.ToUpper), VGDDScreenAttr)
                End If
                For Each oScreenAttr As VGDDScreenAttr In MyBase.Values
                    Dim strFileName As String = System.IO.Path.GetFileNameWithoutExtension(oScreenAttr.FileName).ToUpper
                    If strFileName = ScreenName.ToUpper Or _
                        oScreenAttr.FileName.ToUpper = ScreenName.ToUpper Then
                        Return oScreenAttr
                    End If
                Next
                Return Nothing
            End Get
            Set(ByVal Value As VGDDScreenAttr)
                MyBase.Item(ScreenName.ToUpper) = Value
            End Set
        End Property

        Default Public Overloads ReadOnly Property Item(ByVal Index As Integer) As VGDDScreenAttr
            Get
                Dim i As Integer = 0
                For Each strKey As String In MyBase.Keys
                    If i = Index Then
                        Return MyBase.Item(strKey)
                    End If
                    i += 1
                Next
                Return Nothing
            End Get
        End Property

        Public Sub Rename(ByVal OldKey As String, ByVal NewKey As String)
            Dim oScreen As VGDDScreenAttr = Me(OldKey)
            If oScreen Is Nothing Then
                MessageBox.Show("Could not find " & OldKey & " screen. Please close and reopen the project.")
            ElseIf MyBase.ContainsKey(NewKey.ToUpper) Then
                MessageBox.Show("A screen named " & NewKey & " already exists. Please choose another name for screen", "Duplicate screen name")
            Else
                oScreen.Name = NewKey
                MyBase.Remove(OldKey.ToUpper)
                MyBase.Add(NewKey.ToUpper, oScreen)
            End If
        End Sub

        Public Overloads ReadOnly Property Keys() As ICollection
            Get
                Return MyBase.Keys
            End Get
        End Property

        Public Overloads ReadOnly Property Values() As ICollection
            Get
                Return MyBase.Values
            End Get
        End Property

        Public Function Contains(ByVal ScreenName As String) As Boolean
            Return MyBase.ContainsKey(ScreenName.ToUpper)
        End Function

        Public Overloads Sub Remove(ByVal ScreenName As String)
            ScreenName = ScreenName.ToUpper
            If MyBase.ContainsKey(ScreenName) Then
                MyBase.Remove(ScreenName)
            Else
                For Each oScreenAttr As VGDDScreenAttr In MyBase.Values
                    Dim strName As String = IO.Path.GetFileNameWithoutExtension(oScreenAttr.FileName).ToUpper
                    If strName = ScreenName Then
                        MyBase.Remove(oScreenAttr.Name.ToUpper)
                        Exit For
                    End If
                Next
            End If
        End Sub

    End Class


#If Not PlayerMonolitico Then
    <Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name:="FullTrust")>
    Public Class VGDDControlDesigner
        Inherits System.Windows.Forms.Design.ControlDesigner

        Public Sub New()
            MyBase.new()
        End Sub

        Private _buttonRect As New Rectangle(10, 10, 100, 100)
        Protected Overrides Sub OnPaintAdornments(ByVal pe As PaintEventArgs)
            MyBase.OnPaintAdornments(pe)
            ButtonRenderer.DrawButton(pe.Graphics, _buttonRect, "Text", Me.Control.Font, True,
                System.Windows.Forms.VisualStyles.PushButtonState.Normal)
        End Sub
        Protected Overrides Sub OnMouseHover()
            MyBase.OnMouseHover()
        End Sub

        Protected Overrides Sub OnMouseDragBegin(ByVal x As Integer, ByVal y As Integer)
            MyBase.OnMouseDragBegin(x, y)
        End Sub

        Protected Overrides Sub PreFilterProperties(ByVal properties As System.Collections.IDictionary)
            MyBase.PreFilterProperties(properties)
            properties.Remove("DoubleBuffered")
        End Sub

        Public Overrides ReadOnly Property ParticipatesWithSnapLines As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Sub InitializeNewComponent(ByVal defaultValues As System.Collections.IDictionary)

        End Sub

        Public Overrides Sub InitializeExistingComponent(ByVal defaultValues As System.Collections.IDictionary)

        End Sub

    End Class
#End If

End Namespace
