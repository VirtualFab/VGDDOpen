Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDD

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Indicator), "Indicator.ico")> _
    Public Class Indicator : Inherits Control
        Implements ICustomTypeDescriptor

        Private _Value As Integer = 1
        Private _IndicatorColour As Color = Color.Red
        Private _Frame As EnabledState
        Private _TextAlign As HorizAlign = HorizAlign.Left
        Private _Scheme As VGDDScheme
        Private _Visible As Boolean = True
        Private _Events As VGDDEventsCollection
        Private _State As EnabledState = EnabledState.Enabled
        Private _Style As IndicatorsStyle
        Private _CDeclType As TextCDeclType = TextCDeclType.ConstXcharArray
        Private _public As Boolean

        Public Enum IndicatorsStyle
            INDSTYLE_CIRCLE
            INDSTYLE_SQUARE
        End Enum

        Public Sub New()

            InitializeComponent()
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Indicator")
#End If
            Me.Size = New Size(150, 50)
        End Sub

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

            Dim brushPen As New SolidBrush(_IndicatorColour)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 2
            ps.DashStyle = DashStyle.Solid
            If _Frame = EnabledState.Enabled Then
                ps.Color = _Scheme.Embossdkcolor
                g.DrawRectangle(ps, rc.Left, rc.Top, rc.Right, rc.Bottom)
            End If

            Dim Border As Integer = 2

            'Draw Indicator
            ps.DashStyle = DashStyle.Dot
            Select Case _Style
                Case IndicatorsStyle.INDSTYLE_CIRCLE
                    If _Value Then
                        g.FillEllipse(brushPen, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2))
                    Else
                        g.DrawArc(ps, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2), 0, 360)
                    End If
                Case IndicatorsStyle.INDSTYLE_SQUARE
                    If _Value Then
                        g.FillRectangle(brushPen, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2))
                    Else
                        g.DrawRectangle(ps, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2))
                    End If
            End Select

            'Draw Text
            Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)
            Dim intTextSize As SizeF = g.MeasureString(Text, MyBase.Font)
            Select Case _TextAlign
                Case HorizAlign.Center
                    g.DrawString(Text, MyBase.Font, textBrush, Me.Height + (Me.Width - Me.Height - intTextSize.Width - 8) / 2, (Height - intTextSize.Height) / 2)
                Case HorizAlign.Right
                    g.DrawString(Text, MyBase.Font, textBrush, Width - intTextSize.Width, (Height - intTextSize.Height) / 2)
                Case Else
                    g.DrawString(Text, MyBase.Font, textBrush, Me.Height, (Height - intTextSize.Height) / 2)
            End Select
        End Sub

#Region "GDDProps"

#If PlayerMonolitico Then
        Public Shadows Property Text() As String
#Else
        <Description("Text of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
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

        <Description("Colour for the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(Color), "Color.Red")> _
        <Category("VGDD")> _
        Public Property IndicatorColour() As Color
            Get
                Return _IndicatorColour
            End Get
            Set(ByVal value As Color)
                _IndicatorColour = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Style for the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(IndicatorsStyle.INDSTYLE_CIRCLE)> _
        <Category("VGDD")> _
        Public Property Style() As IndicatorsStyle
            Get
                Return _Style
            End Get
            Set(ByVal value As IndicatorsStyle)
                _Style = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Widget Type")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD"), ParenthesizePropertyName(True)> _
        Public ReadOnly Property WidgetType() As String
            Get
                Return Me.GetType.ToString.Split(".")(1)
            End Get
        End Property

        <Description("Sets the z-order of this widget (0=behind all others).")> _
        <Category("VGDD")> _
        Property Zorder() As Integer
            Get
                Return MyBase.TabIndex
            End Get
            Set(ByVal value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property

        <Description("Event handling for this Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDEventsEditor), GetType(System.Drawing.Design.UITypeEditor))> _
        <ParenthesizePropertyName(True)> _
        <Category("VGDD")> _
        Public Property VGDDEvents() As VGDDEventsCollection
            Get
                Return _Events
            End Get
            Set(ByVal value As VGDDEventsCollection)
                _Events = value
            End Set
        End Property

        <Description("Value or variable do be displayed by the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Property Value() As Integer
            Get
                Return _Value
            End Get
            Set(ByVal value As Integer)
                _Value = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Has the object to be declared public when generating code?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property [Public]() As Boolean
            Get
                Return _public
            End Get
            Set(ByVal value As Boolean)
                _public = value
            End Set
        End Property

        <Description("How the text of the StaticText should be generated in code")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property CDeclType() As TextCDeclType
            Get
                Return _CDeclType
            End Get
            Set(ByVal value As TextCDeclType)
                _CDeclType = value
            End Set
        End Property

        <Description("ColorVGDDScheme for the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <TypeConverter(GetType(Common.SchemesOptionConverter))> _
        <Category("VGDD")> _
        Public Overloads Property Scheme() As String
            Get
                If _Scheme IsNot Nothing Then
                    Return _Scheme.Name
                Else
                    Return String.Empty
                End If
            End Get
            Set(ByVal value As String)
                Dim SetScheme As VGDDScheme = GetScheme(value, Me)
                If SetScheme IsNot Nothing Then
                    _Scheme = SetScheme
                    MyBase.Font = _Scheme.Font.Font
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
           <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("Layout")> _
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
        <Category("Layout")> _
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
        <Category("Layout")> _
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
        <Category("Layout")> _
        Public Overloads Property Bottom() As Integer
            Get
                Return Me.Location.Y + Me.Height
            End Get
            Set(ByVal value As Integer)
                Me.Height = value - Me.Location.Y
                Me.Invalidate()
            End Set
        End Property

        <Description("Wether to enable a frame around the Indicator or not")> _
        <Category("Layout")> _
        Property Frame() As EnabledState
            Get
                Return _Frame
            End Get
            Set(ByVal value As EnabledState)
                _Frame = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Status of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("Layout")> _
        Public Overloads Property State() As EnabledState
            Get
                Return _State
            End Get
            Set(ByVal value As EnabledState)
                _State = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Text Alignement of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Shadows Property TextAlign() As HorizAlign
            Get
                Return _TextAlign
            End Get
            Set(ByVal value As HorizAlign)
                _TextAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Visibility of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("Layout")> _
        Public Shadows Property Visible() As Boolean
            Get
                Return _Visible
            End Get
            Set(ByVal value As Boolean)
                _Visible = value
                Me.Invalidate()
            End Set
        End Property

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not (" BackColor Font " & PROPSTOREMOVE).Contains(" " & pd.Name & " ") Then
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

#End Region

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

#Region "VGDDCode"

#If Not PlayerMonolitico Then

        Public Sub GetCode()
            Dim MyControlId As String = CodeGen.ScreenName & "_" & Me.Name
            Dim MyControlIdNoIndex As String = CodeGen.ScreenName & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.TextDeclareTemplate(_CDeclType).Replace("[CHARMAX]", "")
            Dim MyCode As String = "", MyState As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            MyCode &= CodeGen.ConstructorTemplate
            MyCode &= CodeGen.CodeTemplate & CodeGen.AllCodeTemplate
            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)
            CodeGen.AddState(MyState, "Visible", Me.Visible.ToString)
            CodeGen.AddState(MyState, "TextAlign", Me.TextAlign.ToString)
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)

#If CONFIG = "DemoRelease" Or CONFIG = "DemoDebug" Then
            MyCode = CodeGen.Scramble(MyCode)
#End If

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            CodeGen.Code &= MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[VALUE]", Me.Value) _
                .Replace("[STYLE]", _Style.ToString) _
                .Replace("[COLOUR]", CodeGen.Color2Num(_IndicatorColour)) _
                .Replace("[COLOUR_STRING]", CodeGen.Color2String(_IndicatorColour)) _
                .Replace("[SCHEME]", Me.Scheme)
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.CodeHead &= MyCodeHead
            End If

            CodeGen.Headers &= (IIf(Me.Public, "extern " & CodeGen.ConstructorTemplate.Trim & vbCrLf, "") & _
                CodeGen.HeadersTemplate).Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)

            CodeGen.EventsToCode(MyControlId, Me.VGDDEvents)

        End Sub
#End If
#End Region

    End Class

End Namespace