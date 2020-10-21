Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip
    <DesignerCategory("Code")> _
    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    Public MustInherit Class VGDDBase : Inherits Windows.Forms.UserControl
        Implements ICustomTypeDescriptor
        Implements IVGDDBase
        Implements ISupportInitialize

        'Private adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})

        Protected _CDeclType As TextCDeclType = TextCDeclType.ConstXcharArray
        Protected _public As Boolean = False
        Public _IsLoading As Boolean = False
        Protected _MyPropsToRemove As String = ""

        Public Event FinishedLoading As EventHandler

        Public Sub New()
            Me.RemovePropertyToShow("BackColor")
            Me.RemovePropertyToShow("Font")
            Me.RemovePropertyToShow("Bitmap")
            If Common.VGDDIsRunning Then
                Me.RemovePropertyToShow("Anchor")
            End If
            If Common.ProjectMultiLanguageTranslations = 0 Then
                Me.RemovePropertyToShow("TextStringID")
            End If
        End Sub

        Public Sub AddPropertyToShow(ByVal PropertyName As String)
            For Each strSinglePropertyName As String In PropertyName.Split(" ")
                strSinglePropertyName = " " & strSinglePropertyName.Trim & " "
                If strSinglePropertyName.Trim <> String.Empty Then
                    If _MyPropsToRemove.Contains(PropertyName) Then
                        _MyPropsToRemove = " " & _MyPropsToRemove.Replace(strSinglePropertyName, "").Trim & " "
                    End If
                End If
            Next
        End Sub

        Public Sub RemovePropertyToShow(ByVal PropertyName As String)
            For Each strSinglePropertyName As String In PropertyName.Split(" ")
                strSinglePropertyName = " " & strSinglePropertyName.Trim & " "
                If strSinglePropertyName.Trim <> String.Empty Then
                    If Not _MyPropsToRemove.Contains(strSinglePropertyName) Then
                        _MyPropsToRemove = " " & _MyPropsToRemove.Trim & " " & strSinglePropertyName & " "
                    End If
                End If
            Next
        End Sub

#Region "IVGDD MustOverrides"
#If Not PlayerMonolitico Then
        Public MustOverride Sub GetCode(ByVal ControlIdPrefix As String) Implements IVGDDBase.GetCode
#Else
        Public Sub GetCode(ByVal ControlIdPrefix As String) Implements IVGDDBase.GetCode
        End Sub
#End If
#End Region

#Region "Browsable GDDProps"

#If Not PlayerMonolitico Then
        <Description("Lock Widget in designer")> _
        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Property DesignerLocked As Boolean
            Get
                If TypeDescriptor.GetProperties(Me)("Locked") IsNot Nothing Then
                    Return TypeDescriptor.GetProperties(Me)("Locked").GetValue(Me)
                End If
                Return False
            End Get
            Set(ByVal value As Boolean)
                If TypeDescriptor.GetProperties(Me)("Locked") IsNot Nothing Then
                    TypeDescriptor.GetProperties(Me)("Locked").SetValue(Me, value)
                End If
            End Set
        End Property
#End If

        <Description("Name for this Primitive")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(False)> _
        <CustomSortedCategory("Main", 2)> _
        Public Shadows Property Name() As String
            Get
                Return MyBase.Name
            End Get
            Set(ByVal value As String)
                If Me.Text = MyBase.Name Then
                    Me.Text = value
                End If
                MyBase.Name = value
            End Set
        End Property

        <Description("Actual Widget Size on screen")>
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)>
        <CustomSortedCategory("Info", 1)>
        Public ReadOnly Property SizeOnScreen As String
            Get
                Return String.Format("{0}x{1}", MyBase.Width, MyBase.Height)
            End Get
        End Property

        <Description("Widget Type")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Info", 1)> _
        Public ReadOnly Property WidgetType() As String
            Get
                Return Me.GetType.ToString.Split(".")(1)
            End Get
        End Property

        Friend _TextStringID As Integer = 0
        <Description("String ID of the text in StringsPool (Multi Language)")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
        Public Property TextStringID As Integer
            Get
                Return _TextStringID
            End Get
            Set(ByVal value As Integer)
                SetTextStringId(value, _TextStringID, MyBase.Text)
                Me.Invalidate()
            End Set
        End Property

#If PlayerMonolitico Then
                Public Shadows Property Text() As String
#Else
        <Description("Text for this Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        <Editor(GetType(UiEditTextMultiLanguage), GetType(Drawing.Design.UITypeEditor))> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
        Public Shadows Property Text() As String Implements IVGDDBase.Text
#End If
            Get
                Return GetText(_TextStringID, MyBase.Text)
            End Get
            Set(ByVal value As String)
                'Debug.Print(Me.Site.Name)
                SetText(value, _TextStringID, MyBase.Text)
                Me.Invalidate()
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
        Public Shadows Property Right() As Integer
            Get
                Return Me.Location.X + Me.Width - 1
            End Get
            Set(ByVal value As Integer)
                If Me.IsLoading Then
                    Me.Width = value - MyBase.Location.X + 1
                Else
                    Me.Left = value - Me.Width + 1
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Left X coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
        Public Shadows Property Left() As Integer
            Get
                Return MyBase.Left
            End Get
            Set(ByVal value As Integer)
                MyBase.Left = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Top Y coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
        Public Shadows Property Top() As Integer
            Get
                Return Me.Location.Y
            End Get
            Set(ByVal value As Integer)
                'Me.Location = New Point(Me.Location.X, value)
                MyBase.Top = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Bottom Y coordinate of the lower-right edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
        Public Shadows Property Bottom() As Integer
            Get
                Return Me.Location.Y + Me.Height - 1
            End Get
            Set(ByVal value As Integer)
                If Me.IsLoading Then
                    Me.Height = value - Me.Location.Y + 1
                Else
                    Me.Top = value - Me.Height + 1
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Width of the Widget. Alternative way of setting its Right property")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
        Public Shadows Property Width() As Integer
            Get
                Return MyBase.Width
            End Get
            Set(ByVal value As Integer)
                MyBase.Width = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Height of the Widget. Alternative way of setting its Bottom property")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
        Public Shadows Property Height() As Integer
            Get
                Return MyBase.Height
            End Get
            Set(ByVal value As Integer)
                MyBase.Height = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Sets the z-order of this widget (0=behind all others).")> _
        <Category("Design")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 256)> _
        Public Property Zorder() As Integer Implements IVGDDBase.Zorder
            Get
                Return MyBase.TabIndex
            End Get
            Set(ByVal value As Integer)
                MyBase.TabIndex = value
                If Not Me.IsLoading Then
                    Common.SetNewZOrder(Me.Parent)
                End If
            End Set
        End Property

        <Description("Has the Widget to be declared public when generating code?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property [Public]() As Boolean
            Get
                Return _public
            End Get
            Set(ByVal value As Boolean)
                _public = value
            End Set
        End Property

        <Description("How the text of the Widget should be generated in code")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(TextCDeclType), "ConstXcharArray")> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property CDeclType() As TextCDeclType
            Get
                Return _CDeclType
            End Get
            Set(ByVal value As TextCDeclType)
                _CDeclType = value
            End Set
        End Property
#End Region

#Region "Non browsable GDDProps"

        <Browsable(False)> _
        Public Shadows Property Location() As Point
            Get
                Return MyBase.Location
            End Get
            Set(ByVal value As Point)
                If Me.Parent Is Nothing OrElse _
                    (value.X >= 0 AndAlso value.X <= Me.Parent.Width - Me.Width _
                    AndAlso value.Y >= 0 AndAlso value.Y <> Me.Parent.Height - Me.Height) Then
                    MyBase.Location = value
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IsLoading As Boolean
            Get
                Return _IsLoading
            End Get
        End Property

        <Browsable(False)> _
        Public Shadows Property Size() As Size
            Get
                Return MyBase.Size
            End Get
            Set(ByVal value As Size)
                MyBase.Size = value
                Me.Invalidate()
            End Set
        End Property

        Friend Sub SetTextStringId(ByVal value As Integer, ByRef byRefTextStringID As Integer, ByRef byRefText As String)
            If Not Me.IsLoading AndAlso Me._CDeclType = TextCDeclType.ConstXcharArray AndAlso Common.ProjectMultiLanguageTranslations > 0 Then
                StringsPoolSetUsedBy(byRefTextStringID, StringsPoolSetUsedByAction.Remove)
            End If
            byRefTextStringID = value
            If byRefTextStringID = -1 Then
                byRefTextStringID = 0
            End If
            If Not Me.IsLoading AndAlso Me._CDeclType = TextCDeclType.ConstXcharArray AndAlso Common.ProjectMultiLanguageTranslations > 0 Then
                If byRefTextStringID = 0 OrElse Not Common.ProjectStringPool.ContainsKey(byRefTextStringID) Then
                    If TypeOf (Me) Is VGDDWidget Then
                        CheckStringID(byRefText, byRefTextStringID, CType(Me, VGDDWidget).SchemeObj.Font.Name)
                    Else
                        CheckStringID(byRefText, byRefTextStringID, String.Empty)
                    End If
                End If
                If Common.ProjectStringPool.ContainsKey(byRefTextStringID) Then
                    Dim oStringSet As MultiLanguageStringSet = Common.ProjectStringPool(byRefTextStringID)
                    oStringSet.InUse = True
                    'If Not Me.IsLoading Then
                    StringsPoolSetUsedBy(byRefTextStringID, StringsPoolSetUsedByAction.Add)
                    'End If
                    If byRefText <> oStringSet.Strings(0) Then
                        byRefText = oStringSet.Strings(0)
                    End If
                End If
            End If
        End Sub

        Friend Sub CheckStringID(ByVal Text As String, ByRef StringID As Integer, ByVal strFontName As String)
            If Not Me.IsLoading AndAlso Me._CDeclType = TextCDeclType.ConstXcharArray AndAlso Text IsNot Nothing Then
                'If Common.ControlAdding Then
                '    StringID = AddStringToStringsPool(Text, strFontName)
                '    Return
                'End If
                StringsPoolSetUsedBy(StringID, StringsPoolSetUsedByAction.Remove)
                For Each oStringSet As MultiLanguageStringSet In VGDDCommon.Common.ProjectStringPool.Values
                    If oStringSet.Strings(0) = Text AndAlso (strFontName = String.Empty OrElse oStringSet.FontName = String.Empty OrElse oStringSet.FontName = strFontName) Then
                        If StringID <> oStringSet.StringID Then
                            StringID = oStringSet.StringID
                        End If
                        oStringSet.InUse = True
                        If oStringSet.FontName = String.Empty AndAlso strFontName <> String.Empty Then
                            oStringSet.FontName = strFontName
                        End If
                        StringsPoolSetUsedBy(StringID, StringsPoolSetUsedByAction.Add)
                        Return
                    End If
                Next
                If Text <> String.Empty AndAlso strFontName <> String.Empty Then
                    StringID = AddStringToStringsPool(Text, strFontName)
                End If
            End If
        End Sub

        Friend Enum StringsPoolSetUsedByAction
            Add
            Remove
        End Enum

        Friend Sub StringsPoolSetUsedBy(ByVal intStringId As Integer, ByVal Action As StringsPoolSetUsedByAction)
            If Me.Parent IsNot Nothing AndAlso Me.Parent.Site IsNot Nothing AndAlso _
                intStringId > 0 AndAlso Common.ProjectStringPool.ContainsKey(intStringId) Then
                Dim oStringSet As MultiLanguageStringSet = Common.ProjectStringPool(intStringId)
                Dim strParent As String = String.Empty
                If TypeOf (Me.Parent) Is VGDD.VGDDScreen Then
                    strParent = Me.Parent.Site.Name
                ElseIf Me.Parent.Parent IsNot Nothing _
                    AndAlso TypeOf (Me.Parent.Parent) Is VGDD.VGDDScreen Then
                    strParent = Me.Parent.Parent.Site.Name
                Else
                    Debug.Print("?")
                End If
                If strParent <> String.Empty Then
                    Select Case Action
                        Case StringsPoolSetUsedByAction.Add
                            If oStringSet.UsedBy Is Nothing OrElse Not (oStringSet.UsedBy & " ").Contains(strParent & " ") Then
                                oStringSet.UsedBy &= " " & strParent
                            End If
                        Case StringsPoolSetUsedByAction.Remove
                            If oStringSet.UsedBy IsNot Nothing AndAlso (oStringSet.UsedBy & " ").Contains(strParent & " ") Then
                                oStringSet.UsedBy = (oStringSet.UsedBy & " ").Replace(strParent & " ", "")
                            End If
                    End Select
                End If
            End If
        End Sub

        ' DW Find maximum length of the MultiLanguage string
        Friend Function GetMaxTextLength(ByRef TextStringID As Integer) As Integer
            If Common.ProjectStringPool.ContainsKey(TextStringID) Then
                For j As Integer = 0 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                    If Common.ProjectStringPool(TextStringID).Strings(j) <> String.Empty Then
                        GetMaxTextLength = Math.Max(Common.ProjectStringPool(TextStringID).Strings(j).Length, GetMaxTextLength)
                    End If
                Next
            End If

            Return GetMaxTextLength
        End Function
        Friend Function GetText(ByRef TextStringID As Integer, ByRef Text As String) As String
            If Not Me.IsLoading AndAlso Me._CDeclType = TextCDeclType.ConstXcharArray AndAlso Common.ProjectMultiLanguageTranslations > 0 Then
                'If TextStringID <= 0 OrElse Not Common.ProjectStringPool.ContainsKey(TextStringID) Then
                '    If TypeOf (Me) Is VGDDWidget AndAlso CType(Me, VGDDWidget).SchemeObj IsNot Nothing Then
                '        CheckStringID(MyBase.Text, TextStringID, CType(Me, VGDDWidget).SchemeObj.Font.Name)
                '    Else
                '        CheckStringID(MyBase.Text, TextStringID, String.Empty)
                '    End If
                'End If
                If Common.ProjectStringPool.ContainsKey(TextStringID) Then
                    Text = Common.ProjectStringPool(TextStringID).Strings(Common.ProjectActiveLanguage)
                    'If Text = String.Empty AndAlso Common.ProjectStringPool(TextStringID).Strings(0) <> String.Empty Then
                    '    Text = Common.ProjectStringPool(TextStringID).Strings(0)
                    'End If
                End If
            End If
            Return Text
        End Function

        Friend Sub SetText(ByVal value As String, ByRef ByRefTextStringID As Integer, ByRef ByRefText As String)
            If value Is Nothing Then value = String.Empty
            ByRefText = value
            If Not Me.IsLoading AndAlso Me._CDeclType = TextCDeclType.ConstXcharArray AndAlso Common.ProjectMultiLanguageTranslations > 0 Then
                'CheckStringID(value, ByRefTextStringID)
                If Common.ControlCopying AndAlso value <> String.Empty AndAlso TypeOf (Me) Is VGDDWidget Then
                    ByRefTextStringID = AddStringToStringsPool(value, CType(Me, VGDDWidget).SchemeObj.Font.Name)
                Else
                    If ByRefTextStringID > 0 AndAlso Common.ProjectStringPool.ContainsKey(ByRefTextStringID) Then
                        Dim oStringSet As MultiLanguageStringSet = Common.ProjectStringPool(ByRefTextStringID)
                        If (oStringSet.UsedBy Is Nothing OrElse (oStringSet.UsedBy.Trim.Split(" ").Length = 1) AndAlso value <> oStringSet.Strings(Common.ProjectActiveLanguage)) Then
                            oStringSet.Strings(Common.ProjectActiveLanguage) = value
                        ElseIf value <> oStringSet.Strings(Common.ProjectActiveLanguage) Then
                            'String is referenced also by other widgets so create new one
                            ByRefTextStringID = AddStringToStringsPool(value, oStringSet.FontName)
                            oStringSet = Common.ProjectStringPool(ByRefTextStringID)
                            If Me.Parent.Parent IsNot Nothing AndAlso TypeOf (Me.Parent.Parent) Is VGDD.VGDDScreen Then
                                oStringSet.UsedBy = Me.Parent.Parent.Name
                            End If
                        End If
                        'If Text <> oStringSet.Strings(0) Then
                        '    Debug.Print("?")
                        'End If
                        'Else
                        '    If TextStringID <= 0 AndAlso Not Me.IsLoading Then
                        '        Me.TextStringID = Common.AddStringToStringsPool(value)
                        '        Common.ProjectChanged = True
                        '    End If
                    End If
                End If
                'If TextStringID > 0 AndAlso Common.ProjectStringPool.ContainsKey(TextStringID) Then
                'End If
            End If
        End Sub

#End Region

#Region "ICustomTypeDescriptor Members"

        Public Class CustomSortedCategoryAttribute
            Inherits CategoryAttribute
            Private Const NonPrintableChar As Char = vbTab
            Private Const totalCategories As UShort = 8
            Public Sub New(ByVal category As String, ByVal categoryPos As UShort)
                MyBase.New(category.PadLeft(category.Length + (totalCategories - categoryPos), CustomSortedCategoryAttribute.NonPrintableChar))
            End Sub
        End Class

        Public Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not (PROPSTOREMOVE & _MyPropsToRemove).Contains(" " & pd.Name & " ") _
                    AndAlso Not adjustedProps.Contains(pd) Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

        Public Function GetProperties() As System.ComponentModel.PropertyDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, True)
            Return FilterProperties(pdc)
        End Function

        Public Function GetProperties(ByVal attributes() As System.Attribute) As System.ComponentModel.PropertyDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, attributes, True)
            'pdc = FilterProperties(pdc)
            'Debug.Print("------------------")
            'For Each pd As PropertyDescriptor In pdc
            '    Debug.Print(pd.Name)
            'Next
            'Debug.Print("------------------")
            'Return pdc
            Return FilterProperties(pdc)
        End Function

        Public Function GetEvents(ByVal attributes() As System.Attribute) As System.ComponentModel.EventDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, attributes, True)
        End Function

        Public Function GetEvents() As System.ComponentModel.EventDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, True)
        End Function

        Public Function GetConverter() As System.ComponentModel.TypeConverter _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetConverter
            Return TypeDescriptor.GetConverter(Me, True)
        End Function

        Public Function GetComponentName() As String _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetComponentName
            Return TypeDescriptor.GetComponentName(Me, True)
        End Function

        Public Function GetPropertyOwner(ByVal pd As System.ComponentModel.PropertyDescriptor) As Object _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner
            Return Me
        End Function

        Public Function GetAttributes() As System.ComponentModel.AttributeCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetAttributes
            Return TypeDescriptor.GetAttributes(Me, True)
        End Function

        Public Function GetEditor(ByVal editorBaseType As System.Type) As Object _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetEditor
            Return TypeDescriptor.GetEditor(Me, editorBaseType, True)
        End Function

        Public Function GetDefaultProperty() As System.ComponentModel.PropertyDescriptor _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty
            Return TypeDescriptor.GetDefaultProperty(Me, True)
        End Function

        Public Function GetDefaultEvent() As System.ComponentModel.EventDescriptor _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent
            Return TypeDescriptor.GetDefaultEvent(Me, True)
        End Function

        Public Function GetClassName() As String _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetClassName
            Return TypeDescriptor.GetClassName(Me, True)
        End Function

#End Region

        Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
            _IsLoading = True
        End Sub

        Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
            _IsLoading = False
            If Common.ProjectMultiLanguageTranslations > 0 Then
                'Debug.Print(Me.Site.Name)
                If TextStringID <= 0 OrElse Not Common.ProjectStringPool.ContainsKey(TextStringID) Then
                    If TypeOf (Me) Is VGDDWidget AndAlso CType(Me, VGDDWidget).SchemeObj IsNot Nothing Then
                        CheckStringID(MyBase.Text, TextStringID, CType(Me, VGDDWidget).SchemeObj.Font.Name)
                    Else
                        CheckStringID(MyBase.Text, TextStringID, String.Empty)
                    End If
                End If
                SetTextStringId(_TextStringID, _TextStringID, MyBase.Text)
                'If Common.ProjectStringPool.ContainsKey(TextStringID) Then
                '    Dim oStringSet As MultiLanguageStringSet = Common.ProjectStringPool(TextStringID)
                '    oStringSet.InUse = True
                '    StringsPoolSetUsedBy(TextStringID, StringsPoolSetUsedByAction.Add)
                '    Text = oStringSet.Strings(0)
                'End If
            End If
            RaiseEvent FinishedLoading(Me, New EventArgs)
        End Sub

        Private Sub VGDDBase_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            If Common.ProjectMultiLanguageTranslations > 0 AndAlso Me.Parent IsNot Nothing Then
                StringsPoolSetUsedBy(Me.TextStringID, StringsPoolSetUsedByAction.Add)
                For Each oControl As Object In Me.Controls
                    If TypeOf (oControl) Is VGDDBase Then
                        StringsPoolSetUsedBy(CType(oControl, VGDDBase).TextStringID, StringsPoolSetUsedByAction.Add)
                    End If
                Next
            End If
        End Sub
    End Class

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    Public MustInherit Class VGDDWidget : Inherits VGDDBase
        Implements IVGDDWidget

        Public Event SchemeChanged As EventHandler
        Public Event StateChanged As EventHandler

        Protected _Scheme As VGDDScheme
        Protected _Events As VGDDEventsCollection
        Protected _State As EnabledState = EnabledState.Enabled
        Protected _Hidden As Boolean = False

        Protected _HasChildWidgets As Boolean = False

        Public Sub New()
            Me.RemovePropertyToShow("BackColor")
            Me.RemovePropertyToShow("Font")
            Me.RemovePropertyToShow("Bitmap")
            'Me.RemovePropertyToShow("TextStringID")
            If Common.VGDDIsRunning Then
                Me.RemovePropertyToShow("Anchor")
            End If
        End Sub

        '<System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    If _Scheme IsNot Nothing Then
                        _Scheme.RemoveUsedBy(Me)
                        If _Scheme.Font IsNot Nothing Then
                            _Scheme.Font.RemoveUsedBy(Me)
                        End If
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Friend Sub FillBackground(ByRef g As Graphics, ByRef col As Color, ByRef r As Region)
            Dim bounds As New System.Drawing.Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
            Select Case _Scheme.BackgroundType
                Case VGDDScheme.GFX_BACKGROUND_TYPE.GFX_BACKGROUND_COLOR,
                     VGDDScheme.GFX_BACKGROUND_TYPE.GFX_BACKGROUND_NONE
                    Select Case _Scheme.FillStyle
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_NONE
                            Dim oParent As Control = Me.Parent
                            If oParent IsNot Nothing Then
                                Dim brushBackGround As New SolidBrush(oParent.BackColor)
                                If r IsNot Nothing Then
                                    g.FillRegion(brushBackGround, r)
                                Else
                                    'g.DrawRectangle(New Pen(oParent.BackColor), bounds)
                                End If
                            End If
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_COLOR
                            Dim brushBackGround As New SolidBrush(col)
                            If r IsNot Nothing Then
                                g.FillRegion(brushBackGround, r)
                            Else
                                g.FillRectangle(brushBackGround, bounds)
                            End If
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_VER
                            Dim brushBackGround As New LinearGradientBrush(g.VisibleClipBounds, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 90)
                            Dim bl As New Blend(3)
                            bl.Positions(0) = 0.0F
                            bl.Positions(1) = 0.5F
                            bl.Positions(2) = 1.0F
                            bl.Factors(0) = 0.0F
                            bl.Factors(1) = 1.0F
                            bl.Factors(2) = 0.0F
                            brushBackGround.Blend = bl
                            If r IsNot Nothing Then
                                g.FillRegion(brushBackGround, r)
                            Else
                                g.FillRectangle(brushBackGround, bounds)
                            End If
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_HOR
                            Dim brushBackGround As New LinearGradientBrush(g.VisibleClipBounds, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 0)
                            Dim bl As New Blend(3)
                            bl.Positions(0) = 0.0F
                            bl.Positions(1) = 0.5F
                            bl.Positions(2) = 1.0F
                            bl.Factors(0) = 0.0F
                            bl.Factors(1) = 1.0F
                            bl.Factors(2) = 0.0F
                            brushBackGround.Blend = bl
                            If r IsNot Nothing Then
                                g.FillRegion(brushBackGround, r)
                            Else
                                g.FillRectangle(brushBackGround, bounds)
                            End If
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOWN
                            Dim brushBackGround As New LinearGradientBrush(g.VisibleClipBounds, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 90)
                            If r IsNot Nothing Then
                                g.FillRegion(brushBackGround, r)
                            Else
                                g.FillRectangle(brushBackGround, bounds)
                            End If
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_LEFT
                            Dim brushBackGround As New LinearGradientBrush(g.VisibleClipBounds, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 180)
                            If r IsNot Nothing Then
                                g.FillRegion(brushBackGround, r)
                            Else
                                g.FillRectangle(brushBackGround, bounds)
                            End If
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_RIGHT
                            Dim brushBackGround As New LinearGradientBrush(g.VisibleClipBounds, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 0)
                            If r IsNot Nothing Then
                                g.FillRegion(brushBackGround, r)
                            Else
                                g.FillRectangle(brushBackGround, bounds)
                            End If
                        Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_UP
                            Dim brushBackGround As New LinearGradientBrush(g.VisibleClipBounds, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 90)
                            If r IsNot Nothing Then
                                g.FillRegion(brushBackGround, r)
                            Else
                                g.FillRectangle(brushBackGround, bounds)
                            End If
                    End Select
                Case VGDDScheme.GFX_BACKGROUND_TYPE.GFX_BACKGROUND_IMAGE
                Case Else
            End Select
        End Sub

#If Not PlayerMonolitico Then
        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
        End Sub
#End If

#Region "Browsable GDDProps"

        <Description("Status of this Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(EnabledState), "Enabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property State() As EnabledState
            Get
                Return _State
            End Get
            Set(ByVal value As EnabledState)
                _State = value
                RaiseEvent StateChanged(Me, New EventArgs)
                Me.Invalidate()
            End Set
        End Property

        <Description("Name for this Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(False)> _
        <CustomSortedCategory("Main", 2)> _
        Public Shadows Property Name() As String Implements IVGDDWidget.Name
            Get
                Return MyBase.Name
            End Get
            Set(ByVal value As String)
                MyBase.Name = value
            End Set
        End Property

        <Description("Event handling for this Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDEventsEditorNew), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property VGDDEvents() As VGDDEventsCollection Implements IVGDDWidget.VGDDEvents
            Get
                Return _Events
            End Get
            Set(ByVal value As VGDDEventsCollection)
                _Events = value
            End Set
        End Property

        Public Overloads Property TextStringID As Integer
            Get
                Return MyBase.TextStringID
            End Get
            Set(ByVal value As Integer)
                MyBase.TextStringID = value
                If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.SmartCharSet Then _Scheme.Font.ToBeConverted = True
                Me.Invalidate()
            End Set
        End Property

#If PlayerMonolitico Then
        Public Shadows Property Text() As String
#Else
        <CustomSortedCategory("Main", 2)> _
        Public Shadows Property Text() As String
#End If
            Get
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                MyBase.Text = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Color Scheme for this Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <TypeConverter(GetType(Common.SchemesOptionConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overridable Property Scheme As String Implements IVGDDWidget.Scheme
            Get
                If _Scheme IsNot Nothing Then
                    Return _Scheme.Name
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If _Scheme IsNot Nothing Then
                    _Scheme.RemoveUsedBy(Me)
                    If _Scheme.Font IsNot Nothing Then
                        _Scheme.Font.RemoveUsedBy(Me)
                    End If
                End If
                If value Is Nothing Then
                    _Scheme = Nothing
                    Exit Property
                End If
                Dim SetScheme As VGDDScheme = GetScheme(value)
                If SetScheme Is Nothing Then
                    SetScheme = GetScheme("")
                End If
                If SetScheme IsNot Nothing AndAlso (_Scheme Is Nothing OrElse value <> _Scheme.Name) Then
                    Me.SchemeObj = SetScheme
                    MyBase.Font = _Scheme.Font.Font ' New Font(_Scheme.Font.Font.FontFamily, CInt(_Scheme.Font.Font.Size), _Scheme.Font.Font.Style) '* 0.98
                    SetScheme.AddUsedBy(Me)
                    SetScheme.Font.AddUsedBy(Me)
                    For Each oControl As Control In Me.Controls
                        If TypeOf (oControl) Is IVGDDWidget Then
                            CType(oControl, IVGDDWidget).Scheme = value
                        End If
                    Next
                    If Common.ProjectMultiLanguageTranslations > 0 AndAlso Not Me.IsLoading Then
                        If VGDDCommon.Common.ProjectStringPool.ContainsKey(_TextStringID) Then
                            VGDDCommon.Common.ProjectStringPool(_TextStringID).FontName = _Scheme.Font.Name
                        End If
                    End If
                    Me.Invalidate()
                    RaiseEvent SchemeChanged(Me, New EventArgs)
                End If
            End Set
        End Property

        <Description("Visibility of the Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property Hidden() As Boolean
            Get
                Return _Hidden
            End Get
            Set(ByVal value As Boolean)
                _Hidden = value
                Me.Invalidate()
            End Set
        End Property

#End Region

#Region "Non Browsable GDDProps"

#If PlayerMonolitico Then
        Public Shadows ReadOnly Property AllChars() As String
#Else
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property AllChars() As String
#End If
            Get
                If Common.ProjectMultiLanguageTranslations > 0 Then
                    AllChars = String.Empty
                    Dim CurrentLanguageSave As Integer = Common.ProjectActiveLanguage
                    For i = 0 To Common.ProjectMultiLanguageTranslations
                        Common.ProjectActiveLanguage = i
                        AllChars &= Me.Text
                    Next
                    Common.ProjectActiveLanguage = CurrentLanguageSave
                Else
                    AllChars = Me.Text
                End If
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property SchemeObj As VGDDScheme Implements IVGDDWidget.SchemeObj
            Get
                Return _Scheme
            End Get
            Set(ByVal value As VGDDScheme)
                If _TextStringID > 0 AndAlso Common.ProjectMultiLanguageTranslations > 0 AndAlso VGDDCommon.Common.ProjectStringPool.ContainsKey(_TextStringID) Then
                    Dim oStringSet As MultiLanguageStringSet = VGDDCommon.Common.ProjectStringPool(_TextStringID)
                    oStringSet.FontName = value.Font.Name
                End If
                _Scheme = value
                CheckStringID(Me.Text, Me.TextStringID, Me.SchemeObj.Font.Name)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public ReadOnly Property HasChildWidgets() As Boolean Implements IVGDDWidget.HasChildWidgets
            Get
                Return _HasChildWidgets
            End Get
        End Property

#End Region

#Region "MustOverride"
        Public MustOverride ReadOnly Property Instances As Integer Implements IVGDDWidget.Instances
#End Region

        Private Sub VGDDWidget_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            If Common.ProjectMultiLanguageTranslations > 0 AndAlso Not _MyPropsToRemove.Contains("Text") Then
                'Dim oldid As Integer = _TextStringID
                'CheckStringID(MyBase._Text, _TextStringID, Me.SchemeObj.Font.Name)
                'If _TextStringID <> oldid Then
                '    Debug.Print("£")
                'End If
                If _TextStringID > 0 AndAlso VGDDCommon.Common.ProjectStringPool.ContainsKey(_TextStringID) Then
                    Dim oStringSet As MultiLanguageStringSet = VGDDCommon.Common.ProjectStringPool(_TextStringID)
                    'If MyBase.Text <> oStringSet.Strings(0) AndAlso Not Common.PlayerIsActive Then
                    '    Me.Text = oStringSet.Strings(0)
                    'End If
                    If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso oStringSet.FontName <> _Scheme.Font.Name Then
                        oStringSet.FontName = _Scheme.Font.Name
                    End If
                    If oStringSet.AutoWrap Then
                        oStringSet.AutoWrapStrings(Me.SchemeObj.Font.Font, Me.Width)
                    End If
                ElseIf Me.TextStringID <> _TextStringID Then
                    Me.TextStringID = _TextStringID
                End If
            End If
        End Sub

        Private Sub VGDDWidget_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            StringsPoolSetUsedBy(_TextStringID, StringsPoolSetUsedByAction.Add)
        End Sub

        Private Sub VGDDWidget_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            If Not Me.IsLoading AndAlso Common.ProjectMultiLanguageTranslations > 0 AndAlso _TextStringID > 0 AndAlso _
                Common.ProjectStringPool.ContainsKey(_TextStringID) Then
                Dim oStringSet As MultiLanguageStringSet = Common.ProjectStringPool(_TextStringID)
                If oStringSet.AutoWrap Then
                    oStringSet.AutoWrapStrings(Me.SchemeObj.Font.Font, Me.Width)
                End If
            End If
        End Sub
    End Class

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    Public MustInherit Class VGDDWidgetWithBitmap : Inherits VGDDWidget
        Implements IVGDDWidgetWithBitmap

        Protected _BitmapName As String
        Protected _VGDDImage As VGDDImage
        Private _Scale As Byte = 1
        Protected _ShrinkSizeToBitmapSize As Boolean = True
        Protected _BitmapNeeded As Boolean = True
        Protected _BitmapUsePointer As Boolean = False

        Public Sub New()
            Me.AddPropertyToShow("Bitmap")
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    If _VGDDImage IsNot Nothing Then
                        _VGDDImage.RemoveUsedBy(Me)
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#If Not PlayerMonolitico Then
        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
        End Sub
#End If

        Public Sub ScaleBitmap() Implements IVGDDWidgetWithBitmap.ScaleBitmap
            If _VGDDImage IsNot Nothing Then
                If _VGDDImage.AllowScaling Then
                    _VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
                End If
            End If
        End Sub

#Region "Browsable GDDProps"

        <Description("Should a (modificable) pointer be used in generated code when using the bitmap?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(Boolean), "False")> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property BitmapUsePointer() As Boolean
            Get
                Return _BitmapUsePointer
            End Get
            Set(ByVal value As Boolean)
                _BitmapUsePointer = value
            End Set
        End Property

        <Description("Bitmap name to draw")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overridable Property Bitmap() As String Implements IVGDDWidgetWithBitmap.BitmapName
            Get
                'If _VGDDImage IsNot Nothing Then
                'Return _VGDDImage.Name
                Return _BitmapName
                'Else
                'Return (String.Empty)
                'End If
            End Get
            Set(ByVal value As String)
                _BitmapName = value
                If value = String.Empty Then
                    If _VGDDImage IsNot Nothing Then
                        _VGDDImage.RemoveUsedBy(Me)
                    End If
                    _VGDDImage = Nothing
                ElseIf Not Me.IsLoading Then
                    Common.SetBitmapName(value, Me, _BitmapName, _VGDDImage, Nothing)
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Scale Factor for the bitmap. Valid values are 1 and 2")>
        <DefaultValue(1)> _
        <CustomSortedCategory("Appearance", 4)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(1, 2)> _
        Public Shadows Property Scale() As Byte
            Get
                Return _Scale
            End Get
            Set(ByVal value As Byte)
                Dim OldValue As Byte = _Scale
                If value = 1 Or value = 2 Then
                    _Scale = value
                    If Not Me.IsLoading AndAlso OldValue <> value AndAlso _VGDDImage IsNot Nothing Then
                        _VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
                        Me.Invalidate()
                    End If
                End If
            End Set
        End Property
#End Region

#Region "Non-Browsable GDDProps"

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property BitmapNeeded As Boolean
            Get
                Return _BitmapNeeded
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property VGDDImage() As VGDDImage Implements IVGDDWidgetWithBitmap.VGDDImage
            Get
                Return _VGDDImage
            End Get
        End Property

        Public Sub SetBitmap(ByVal bmp As System.Drawing.Bitmap) Implements IVGDDWidgetWithBitmap.SetBitmap
            If _VGDDImage Is Nothing Then _VGDDImage = New VGDDImage
            _VGDDImage.TransparentBitmap = bmp
            Me.Invalidate()
        End Sub

        'Public Sub SetAndScaleBitmap(ByVal bmp As System.Drawing.Bitmap)
        '    SetAndScaleBitmap(bmp, Me.Size)
        'End Sub

        'Public Sub SetAndScaleBitmap(ByVal bmp As System.Drawing.Bitmap, ByVal NewSize As Size)
        '    If _VGDDImage Is Nothing Then
        '        _VGDDImage = New VGDDImage
        '    ElseIf _VGDDImage.TransparentBitmap IsNot Nothing Then
        '        _VGDDImage.TransparentBitmap.Dispose()
        '    End If
        '    _VGDDImage.AllowScaling = False
        '    _VGDDImage.TransparentBitmap = New Bitmap(NewSize.Width, NewSize.Height)
        '    Using gr As Graphics = Graphics.FromImage(_VGDDImage.TransparentBitmap)
        '        gr.DrawImage(bmp, (Me.Width - NewSize.Width) \ 2, (Me.Height - NewSize.Height) \ 2, NewSize.Width, NewSize.Height)
        '    End Using
        '    Me.Invalidate()
        'End Sub

        'Public Sub SetBitmapName(ByVal BitmapName As String, ByRef Image2Set As VGDDImage, ByRef Var2Set As String)
        '    Var2set = BitmapName
        '    Dim OldImage As VGDDImage = Image2Set
        '    If Var2set = String.Empty Then
        '        If OldImage IsNot Nothing Then
        '            OldImage.RemoveUsedBy(Me)
        '        End If
        '        Image2Set = Nothing
        '    Else
        '        Image2Set = GetBitmap(Var2Set)
        '        If Image2Set Is Nothing OrElse Image2Set.Bitmap Is Nothing Then
        '            Image2Set = New VGDDImage
        '            Image2Set.TransparentBitmap = VGDDImage.InvalidBitmap(Nothing, Me.Width, Me.Height)
        '            Image2Set._GraphicsPath = Nothing
        '            If OldImage IsNot Nothing Then OldImage.RemoveUsedBy(Me)
        '        Else
        '            VGDDWidgetWithBitmap_ParentChanged(Nothing, Nothing)
        '            If Not Image2Set.AllowScaling Then
        '                If OldImage Is Nothing Then
        '                    Me.Size = Image2Set.OrigBitmap.Size
        '                End If
        '                Image2Set.ScaleBitmap()
        '            Else
        '                Image2Set.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
        '            End If
        '            If OldImage IsNot Nothing AndAlso Not OldImage Is Image2Set Then
        '                OldImage.RemoveUsedBy(Me)
        '            End If
        '            Image2Set.AddUsedBy(Me)
        '        End If
        '    End If
        '    Me.Invalidate()
        'End Sub

        <Description("Parent's transparent colour")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Property ParentTransparentColour() As Color
            Get
                If _VGDDImage IsNot Nothing Then
                    Return _VGDDImage.ParentTransparentColour
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Color)
                If _VGDDImage IsNot Nothing Then
                    Dim OldValue As Color = _VGDDImage.ParentTransparentColour
                    _VGDDImage.ParentTransparentColour = value
                    If Not Me.IsLoading AndAlso value <> OldValue Then
                        _VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
                        Me.Invalidate()
                    End If
                End If
            End Set
        End Property

        Public Sub SetSize(ByVal newSize As Size)
            MyBase.Size = newSize 'New Size(newSize.Width / _Scale, newSize.Height / _Scale)
            If _VGDDImage IsNot Nothing Then
                _VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
            End If
            Me.Invalidate()
        End Sub
#End Region

        Private Sub VGDDWidgetWithBitmap_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            If _BitmapName IsNot Nothing Then
                _VGDDImage = New VGDDImage
                Common.SetBitmapName(_BitmapName, Me, _BitmapName, _VGDDImage, Nothing)
                '_IsLoading = False
            End If
        End Sub

        Private Sub VGDDWidgetWithBitmap_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            If Me.Parent IsNot Nothing Then
                If TypeOf (Me.Parent) Is VGDD.VGDDScreen Then
                    Me.ParentTransparentColour = CType(Me.Parent, VGDD.VGDDScreen).TransparentColour
#If PlayerMonolitico Then
                ElseIf TypeOf (Me.Parent) Is PlayerPanel Then
                    Me.ParentTransparentColour = CType(Me.Parent, PlayerPanel).TransparentColour
#End If
                End If
            End If
        End Sub

        Protected Overridable Sub VGDDWidgetWithBitmap_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            If Bitmap = String.Empty AndAlso _BitmapNeeded Then
                If _VGDDImage IsNot Nothing Then _VGDDImage.RemoveUsedBy(Me)
                _VGDDImage = New VGDDImage
                _VGDDImage.TransparentBitmap = VGDDImage.InvalidBitmap(Nothing, Me.Width, Me.Height)
                _VGDDImage._GraphicsPath = New GraphicsPath
                _VGDDImage._GraphicsPath.AddRectangle(Me.ClientRectangle)
            ElseIf _VGDDImage IsNot Nothing Then
                If _VGDDImage.AllowScaling Then
                    _VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
                    'BuildTransparentBitmap(_VGDDImage.Bitmap)
                    Me.Invalidate()
                ElseIf _VGDDImage.Bitmap IsNot Nothing Then 'AndAlso MyBase.Size <> _VGDDImage.Bitmap.Size Then
                    If Not TypeOf (Me) Is VGDDMicrochip.Button Then
                        MyBase.Size = _VGDDImage.Bitmap.Size
                    End If
                    If _VGDDImage.TransparentBitmap.Size <> _VGDDImage.OrigBitmap.Size Then
                        _VGDDImage.TransparentBitmap = New Bitmap(_VGDDImage.OrigBitmap)
                    End If
                End If
            End If
        End Sub
    End Class

End Namespace