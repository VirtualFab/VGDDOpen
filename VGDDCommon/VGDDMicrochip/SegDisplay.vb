Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common
Imports VGDDMicrochip.VGDDBase

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "SegDisplayIco")> _
    Public Class SegDisplay : Inherits Windows.Forms.Control
        Implements ICustomTypeDescriptor
        Implements IVGDDWidget

        Private _Value As String
        Private _NumDigits As Integer = 5
        Private _DotPos As Integer
        Private _Thickness As Integer = 3
        Private _Frame As EnabledState
        Private _TextAlign As HorizAlign = HorizAlign.Left
        Private _Scheme As VGDDScheme
        Private _Visible As Boolean = True
        Private _Events As VGDDEventsCollection
        Private _State As EnabledState = EnabledState.Enabled

        Private Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1

#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("SegDisplay")
#End If
        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public ReadOnly Property Instances As Integer Implements IVGDDWidget.Instances
            Get
                Return _Instances
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)>
        Public Shadows Property Text As String Implements IVGDDWidget.Text
            Get
                Return String.Empty
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            'Me.OnPaintBackground(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

            'Impostazione Region
            Dim Mypath As GraphicsPath = New GraphicsPath
            Mypath.StartFigure()
            Mypath.AddRectangle(Me.ClientRectangle)
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)
            If _Scheme Is Nothing Then Exit Sub
            Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
            g.FillRegion(brushBackGround, Me.Region)

            Dim brushPen As New SolidBrush(_Scheme.Color0)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 2
            If _Frame = EnabledState.Enabled Then
                ps.Color = _Scheme.Embossdkcolor
                g.DrawRectangle(ps, rc.Left, rc.Top, rc.Right, rc.Bottom)
            End If

            'Draw Text
            brushPen.Color = _Scheme.Textcolor0
            Dim x As Integer, w As Integer = rc.Width / _NumDigits
            For i As Integer = 1 To _NumDigits
                x = (i - 1) * w
                g.FillRectangle(brushPen, x, rc.Top + 4, w - 10, _Thickness)
                g.FillRectangle(brushPen, x + w - 10 - _Thickness, rc.Top + 4, _Thickness, rc.Height - 8)
                g.FillRectangle(brushPen, x, rc.Bottom - 4 - _Thickness, w - 10, _Thickness)
                g.FillRectangle(brushPen, x, rc.Top + 4, _Thickness, rc.Height - 8)
                g.FillRectangle(brushPen, x, CInt(rc.Top - 4 + rc.Height / 2), w - 10, _Thickness)
            Next
        End Sub

#Region "IVGDD Stubs"

        Public ReadOnly Property HasChildWidgets As Boolean Implements IVGDDWidget.HasChildWidgets
            Get
                Return False
            End Get
        End Property

#End Region

#Region "GDDProps"

        <Description("Widget Type")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public ReadOnly Property WidgetType() As String
            Get
                Return Me.GetType.ToString.Split(".")(1)
            End Get
        End Property

        <Description("Sets the z-order of this widget (0=behind all others).")> _
        <Category("Design")> _
        Property Zorder() As Integer Implements IVGDDWidget.Zorder
            Get
                Return MyBase.TabIndex
            End Get
            Set(ByVal value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property

        <Description("Name for this Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Shadows Property Name() As String Implements IVGDDWidget.Name
            Get
                Return MyBase.Name
            End Get
            Set(ByVal value As String)
                MyBase.Name = value
            End Set
        End Property

        '#If Not PlayerMonolitico Then
        <Description("Event handling for this SegDisplay")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDEventsEditorNew), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("CodeGen", 6)> _
        <ParenthesizePropertyName(True)> _
        Public Property VGDDEvents() As VGDDEventsCollection Implements IVGDDWidget.VGDDEvents
            Get
                Return _Events
            End Get
            Set(ByVal value As VGDDEventsCollection)
                _Events = value
            End Set
        End Property
        '#End If

        <Description("Value or variable do be displayed by the SegDisplay")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Property Value() As String
            Get
                Return _Value
            End Get
            Set(ByVal value As String)
                _Value = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Number of digits to display")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property NumDigits() As Integer
            Get
                Return _NumDigits
            End Get
            Set(ByVal value As Integer)
                _NumDigits = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Position of the decimal point")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property DotPos() As Integer
            Get
                Return _DotPos
            End Get
            Set(ByVal value As Integer)
                _DotPos = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Thickness of the segments in pixels")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Thickness() As Integer
            Get
                Return _Thickness
            End Get
            Set(ByVal value As Integer)
                _Thickness = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Color Scheme for the SegDisplay")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <TypeConverter(GetType(Common.SchemesOptionConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Scheme() As String Implements IVGDDWidget.Scheme
            Get
                If _Scheme IsNot Nothing Then
                    Return _Scheme.Name
                Else
                    Return String.Empty
                End If
            End Get
            Set(ByVal value As String)
                Dim SetScheme As VGDDScheme = GetScheme(value)
                If SetScheme IsNot Nothing Then
                    _Scheme = SetScheme
                    MyBase.Font = _Scheme.Font.Font
                    Me.Invalidate()
                End If
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property SchemeObj As VGDDScheme Implements IVGDDWidget.SchemeObj
            Get
                Return _Scheme
            End Get
            Set(ByVal value As VGDDScheme)
                _Scheme = value
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
           <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Overloads Property Right() As Integer
            Get
                Return Me.Location.X + Me.Width
            End Get
            Set(ByVal value As Integer)
                Me.Width = value - MyBase.Location.X
                Me.Invalidate()
            End Set
        End Property

        <Description("Left X coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Overloads Property Left() As Integer
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
        Public Overloads Property Top() As Integer
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
        Public Overloads Property Bottom() As Integer
            Get
                Return Me.Location.Y + Me.Height
            End Get
            Set(ByVal value As Integer)
                Me.Height = value - Me.Location.Y
                Me.Invalidate()
            End Set
        End Property

        <Description("Wether to enable a frame around the SegDisplay or not")> _
        <DefaultValue(GetType(EnabledState), "Disabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Property Frame() As EnabledState
            Get
                Return _Frame
            End Get
            Set(ByVal value As EnabledState)
                _Frame = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Status of the SegDisplay")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property State() As EnabledState
            Get
                Return _State
            End Get
            Set(ByVal value As EnabledState)
                _State = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Text Alignement of the SegDisplay")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Left")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property TextAlign() As HorizAlign
            Get
                Return _TextAlign
            End Get
            Set(ByVal value As HorizAlign)
                _TextAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Visibility of the SegDisplay")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property Visible() As Boolean
            Get
                Return _Visible
            End Get
            Set(ByVal value As Boolean)
                _Visible = value
                Me.Invalidate()
            End Set
        End Property

#End Region

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not (" Text Font " & PROPSTOREMOVE).Contains(" " & pd.Name & " ") Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

        <System.ComponentModel.Browsable(False)> _
        Public Shadows Property Location() As Point
            Get
                Return MyBase.Location
            End Get
            Set(ByVal value As Point)
                MyBase.Location = value
            End Set
        End Property

        <System.ComponentModel.Browsable(False)> _
        Public Shadows Property Size() As Size
            Get
                Return MyBase.Size
            End Get
            Set(ByVal value As Size)
                MyBase.Size = value
            End Set
        End Property

#Region "ICustomTypeDescriptor Members"

        Private Function GetProperties(ByVal attributes() As Attribute) As PropertyDescriptorCollection _
            Implements ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, attributes, True)
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
            Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, True)
        End Function

        Public Function GetComponentName() As String _
            Implements ICustomTypeDescriptor.GetComponentName
            Return TypeDescriptor.GetComponentName(Me, True)
        End Function

        Public Function GetPropertyOwner(ByVal pd As PropertyDescriptor) As Object _
            Implements ICustomTypeDescriptor.GetPropertyOwner
            Return Me
        End Function

        Public Function GetAttributes() As AttributeCollection _
            Implements ICustomTypeDescriptor.GetAttributes
            Return TypeDescriptor.GetAttributes(Me, True)
        End Function

        Private Function System_ComponentModel_ICustomTypeDescriptor_GetProperties() As PropertyDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, True)
            Return FilterProperties(pdc)
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

        Public Sub GetCode(ByVal ControlIdPrefix As String) Implements IVGDDWidget.GetCode
#If Not PlayerMonolitico Then
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = ""
            Dim MyCode As String = "", MyState As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate)

            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)
            CodeGen.AddState(MyState, "Visible", Me.Visible.ToString)
            CodeGen.AddState(MyState, "HorizAlign", Me.TextAlign.ToString)
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState).Replace("[VALUE]", Me.Value) _
                .Replace("[NUMDIGITS]", Me._NumDigits) _
                .Replace("[DOTPOS]", Me._DotPos) _
                .Replace("[THICKNESS]", Me._Thickness) _
                .Replace("[SCHEME]", Me.Scheme))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, CodeGen.HeadersTemplate.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, CType(Me, IVGDDWidget))

#End If
        End Sub
    End Class

End Namespace