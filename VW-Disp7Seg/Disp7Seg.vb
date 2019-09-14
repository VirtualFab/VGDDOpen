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
    <ToolboxBitmap(GetType(Disp7Seg), "Disp7Seg.ico")> _
    Public Class Disp7Seg : Inherits Control
        Implements ICustomTypeDescriptor

        Private _Value As Integer
        Private _NumDigits As Integer = 3
        Private _DotPos As Integer
        Private _Thickness As Integer = 7
        Private _Frame As EnabledState
        Private _TextAlign As HorizAlign = HorizAlign.Left
        Private _Scheme As VGDDScheme
        Private _Visible As Boolean = True
        Private _Events As VGDDEventsCollection
        Private _State As EnabledState = EnabledState.Enabled
        Private _Style As FontLed7SegStyle

        Public Enum FontLed7SegStyle
            FontLed7SegBar
            FontLed7SegPoly
        End Enum

        Private aLed7SegCoords(7, 14) As Int16
        Private FontLed7SegCurrentSizeX As UInt16, FontLed7SegCurrentSizeY As UInt16

        Public Sub New()

            InitializeComponent()
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("FontLed7Seg")
#End If
            Me.Size = New Size(150, 100)
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
            Dim num As String = Me.Value.ToString(StrDup(Me.NumDigits, "0"c))
            Dim chars As Char() = num.ToCharArray()
            For i As Integer = 1 To _NumDigits
                x = (i - 1) * w
                DrawSingleDigit(g, chars(i - 1), x, rc.Top + 4, rc.Height - 8)
                'g.FillRectangle(brushPen, x, rc.Top + 4, w - 10, _Thickness)
                'g.FillRectangle(brushPen, x + w - 10 - _Thickness, rc.Top + 4, _Thickness, rc.Height - 8)
                'g.FillRectangle(brushPen, x, rc.Bottom - 4 - _Thickness, w - 10, _Thickness)
                'g.FillRectangle(brushPen, x, rc.Top + 4, _Thickness, rc.Height - 8)
                'g.FillRectangle(brushPen, x, CInt(rc.Top - 4 + rc.Height / 2), w - 10, _Thickness)
            Next
        End Sub

        Private Sub DrawSingleDigit(ByVal g As Graphics, ByVal Digit As Char, ByVal posX As Int16, ByVal posY As Int16, ByVal height As Int16)

            Dim aLed7SegSegments() As Byte = { _
                Convert.ToByte("0000000", 2), _
                Convert.ToByte("0111111", 2), _
                Convert.ToByte("0000110", 2), _
                Convert.ToByte("1011011", 2), _
                Convert.ToByte("1001111", 2), _
                Convert.ToByte("1100110", 2), _
                Convert.ToByte("1101101", 2), _
                Convert.ToByte("1111101", 2), _
                Convert.ToByte("0000111", 2), _
                Convert.ToByte("1111111", 2), _
                Convert.ToByte("1101111", 2) _
                }

            Dim segs As Byte = Asc(Digit) - Asc("0") + 1
            If segs > 10 Then segs = 0
            For i = 0 To 7
                If (aLed7SegSegments(segs) And (1 << i)) Then
                    Select Case Me.Style
                        Case FontLed7SegStyle.FontLed7SegBar
                            'Dim brush As New SolidBrush(_Scheme.Textcolor0)
                            Dim brush As New SolidBrush(Color.FromArgb(100 + i * 20, 100 + i * 20, 100 + i * 20))
                            Dim x As Integer = posX + aLed7SegCoords(i, 0)
                            Dim y As Integer = posY + aLed7SegCoords(i, 1)
                            Dim w As Integer = aLed7SegCoords(i, 2) - aLed7SegCoords(i, 0)
                            Dim h As Integer = aLed7SegCoords(i, 3) - aLed7SegCoords(i, 1)
                            g.FillRectangle(brush, x, y, w, h)

                        Case FontLed7SegStyle.FontLed7SegPoly
                            Dim numPoints As Byte
                            numPoints = IIf(i = 6, 7, 5)
                            Dim aSegPoly(numPoints - 1) As Point
                            For j = 0 To numPoints - 1
                                aSegPoly(j) = New Point(aLed7SegCoords(i, j * 2) + posX, aLed7SegCoords(i, j * 2 + 1) + posY)
                            Next
                            Dim brushPen As New Pen(_Scheme.Textcolor0)
                            g.DrawPolygon(brushPen, aSegPoly)
                    End Select

                End If
            Next

        End Sub

        Private Sub FontLed7SegSetSize(ByVal SizeY As UInt16, ByVal SizeX As UInt16, ByVal thickness As Byte)
            FontLed7SegCurrentSizeX = SizeX
            FontLed7SegCurrentSizeY = SizeY

            Select Case (Me.Style)
                Case FontLed7SegStyle.FontLed7SegBar
                    ' Segment A
                    aLed7SegCoords(0, 0) = 0
                    aLed7SegCoords(0, 1) = 0
                    aLed7SegCoords(0, 2) = FontLed7SegCurrentSizeX + thickness
                    aLed7SegCoords(0, 3) = thickness

                    ' Segment B
                    aLed7SegCoords(1, 0) = FontLed7SegCurrentSizeX
                    aLed7SegCoords(1, 1) = 0
                    aLed7SegCoords(1, 2) = FontLed7SegCurrentSizeX + thickness
                    aLed7SegCoords(1, 3) = FontLed7SegCurrentSizeY / 2 + thickness

                    ' Segment C
                    aLed7SegCoords(2, 0) = FontLed7SegCurrentSizeX
                    aLed7SegCoords(2, 1) = FontLed7SegCurrentSizeY / 2 + thickness
                    aLed7SegCoords(2, 2) = FontLed7SegCurrentSizeX + thickness
                    aLed7SegCoords(2, 3) = FontLed7SegCurrentSizeY + thickness

                    ' Segment D
                    aLed7SegCoords(3, 0) = 0
                    aLed7SegCoords(3, 1) = FontLed7SegCurrentSizeY
                    aLed7SegCoords(3, 2) = FontLed7SegCurrentSizeX + thickness
                    aLed7SegCoords(3, 3) = FontLed7SegCurrentSizeY + thickness

                    ' Segment E
                    aLed7SegCoords(4, 0) = 0
                    aLed7SegCoords(4, 1) = FontLed7SegCurrentSizeY / 2
                    aLed7SegCoords(4, 2) = thickness
                    aLed7SegCoords(4, 3) = FontLed7SegCurrentSizeY + thickness

                    ' Segment F
                    aLed7SegCoords(5, 0) = 0
                    aLed7SegCoords(5, 1) = 0
                    aLed7SegCoords(5, 2) = thickness
                    aLed7SegCoords(5, 3) = FontLed7SegCurrentSizeY / 2 ' + thickness

                    ' Segment G
                    aLed7SegCoords(6, 0) = 0
                    aLed7SegCoords(6, 1) = FontLed7SegCurrentSizeY / 2
                    aLed7SegCoords(6, 2) = FontLed7SegCurrentSizeX + thickness
                    aLed7SegCoords(6, 3) = FontLed7SegCurrentSizeY / 2 + thickness

                Case FontLed7SegStyle.FontLed7SegPoly
                    ' Segment A
                    aLed7SegCoords(0, 0) = FontLed7SegCurrentSizeX * 2.8 / 10
                    aLed7SegCoords(0, 1) = FontLed7SegCurrentSizeY / 10
                    aLed7SegCoords(0, 2) = FontLed7SegCurrentSizeX * 10 / 10
                    aLed7SegCoords(0, 3) = FontLed7SegCurrentSizeY / 10
                    aLed7SegCoords(0, 4) = FontLed7SegCurrentSizeX * 8.8 / 10
                    aLed7SegCoords(0, 5) = FontLed7SegCurrentSizeY * 2.0 / 10
                    aLed7SegCoords(0, 6) = FontLed7SegCurrentSizeX * 3.8 / 10
                    aLed7SegCoords(0, 7) = FontLed7SegCurrentSizeY * 2.0 / 10
                    aLed7SegCoords(0, 8) = FontLed7SegCurrentSizeX * 2.8 / 10
                    aLed7SegCoords(0, 9) = FontLed7SegCurrentSizeY / 10

                    ' Segment B
                    aLed7SegCoords(1, 0) = FontLed7SegCurrentSizeX * 10 / 10
                    aLed7SegCoords(1, 1) = FontLed7SegCurrentSizeY * 1.4 / 10
                    aLed7SegCoords(1, 2) = FontLed7SegCurrentSizeX * 9.3 / 10
                    aLed7SegCoords(1, 3) = FontLed7SegCurrentSizeY * 6.8 / 10
                    aLed7SegCoords(1, 4) = FontLed7SegCurrentSizeX * 8.4 / 10
                    aLed7SegCoords(1, 5) = FontLed7SegCurrentSizeY * 6.4 / 10
                    aLed7SegCoords(1, 6) = FontLed7SegCurrentSizeX * 9.0 / 10
                    aLed7SegCoords(1, 7) = FontLed7SegCurrentSizeY * 2.2 / 10
                    aLed7SegCoords(1, 8) = FontLed7SegCurrentSizeX * 10 / 10
                    aLed7SegCoords(1, 9) = FontLed7SegCurrentSizeY * 1.4 / 10

                    ' Segment C
                    aLed7SegCoords(2, 0) = FontLed7SegCurrentSizeX * 9.2 / 10
                    aLed7SegCoords(2, 1) = FontLed7SegCurrentSizeY * 7.2 / 10
                    aLed7SegCoords(2, 2) = FontLed7SegCurrentSizeX * 8.5 / 10
                    aLed7SegCoords(2, 3) = FontLed7SegCurrentSizeY * 12.7 / 10
                    aLed7SegCoords(2, 4) = FontLed7SegCurrentSizeX * 7.7 / 10
                    aLed7SegCoords(2, 5) = FontLed7SegCurrentSizeY * 11.9 / 10
                    aLed7SegCoords(2, 6) = FontLed7SegCurrentSizeX * 8.2 / 10
                    aLed7SegCoords(2, 7) = FontLed7SegCurrentSizeY * 7.7 / 10
                    aLed7SegCoords(2, 8) = FontLed7SegCurrentSizeX * 9.2 / 10
                    aLed7SegCoords(2, 9) = FontLed7SegCurrentSizeY * 7.2 / 10

                    ' Segment D
                    aLed7SegCoords(3, 0) = FontLed7SegCurrentSizeX * 7.4 / 10
                    aLed7SegCoords(3, 1) = FontLed7SegCurrentSizeY * 12.1 / 10
                    aLed7SegCoords(3, 2) = FontLed7SegCurrentSizeX * 8.4 / 10
                    aLed7SegCoords(3, 3) = FontLed7SegCurrentSizeY * 13.0 / 10
                    aLed7SegCoords(3, 4) = FontLed7SegCurrentSizeX * 1.1 / 10
                    aLed7SegCoords(3, 5) = FontLed7SegCurrentSizeY * 13.0 / 10
                    aLed7SegCoords(3, 6) = FontLed7SegCurrentSizeX * 2.2 / 10
                    aLed7SegCoords(3, 7) = FontLed7SegCurrentSizeY * 12.1 / 10
                    aLed7SegCoords(3, 8) = FontLed7SegCurrentSizeX * 7.4 / 10
                    aLed7SegCoords(3, 9) = FontLed7SegCurrentSizeY * 12.1 / 10

                    ' Segment E
                    aLed7SegCoords(4, 0) = FontLed7SegCurrentSizeX * 2.2 / 10
                    aLed7SegCoords(4, 1) = FontLed7SegCurrentSizeY * 11.8 / 10
                    aLed7SegCoords(4, 2) = FontLed7SegCurrentSizeX * 1.0 / 10
                    aLed7SegCoords(4, 3) = FontLed7SegCurrentSizeY * 12.7 / 10
                    aLed7SegCoords(4, 4) = FontLed7SegCurrentSizeX * 1.8 / 10
                    aLed7SegCoords(4, 5) = FontLed7SegCurrentSizeY * 7.2 / 10
                    aLed7SegCoords(4, 6) = FontLed7SegCurrentSizeX * 2.8 / 10
                    aLed7SegCoords(4, 7) = FontLed7SegCurrentSizeY * 7.7 / 10
                    aLed7SegCoords(4, 8) = FontLed7SegCurrentSizeX * 2.2 / 10
                    aLed7SegCoords(4, 9) = FontLed7SegCurrentSizeY * 11.8 / 10

                    ' Segment
                    aLed7SegCoords(5, 0) = FontLed7SegCurrentSizeX * 3.0 / 10
                    aLed7SegCoords(5, 1) = FontLed7SegCurrentSizeY * 6.2 / 10
                    aLed7SegCoords(5, 2) = FontLed7SegCurrentSizeX * 1.9 / 10
                    aLed7SegCoords(5, 3) = FontLed7SegCurrentSizeY * 6.8 / 10
                    aLed7SegCoords(5, 4) = FontLed7SegCurrentSizeX * 2.7 / 10
                    aLed7SegCoords(5, 5) = FontLed7SegCurrentSizeY * 1.3 / 10
                    aLed7SegCoords(5, 6) = FontLed7SegCurrentSizeX * 3.6 / 10
                    aLed7SegCoords(5, 7) = FontLed7SegCurrentSizeY * 2.2 / 10
                    aLed7SegCoords(5, 8) = FontLed7SegCurrentSizeX * 3.0 / 10
                    aLed7SegCoords(5, 9) = FontLed7SegCurrentSizeY * 6.2 / 10

                    ' Segment G
                    aLed7SegCoords(6, 0) = FontLed7SegCurrentSizeX * 2.0 / 10
                    aLed7SegCoords(6, 1) = FontLed7SegCurrentSizeY * 7.0 / 10
                    aLed7SegCoords(6, 2) = FontLed7SegCurrentSizeX * 3.1 / 10
                    aLed7SegCoords(6, 3) = FontLed7SegCurrentSizeY * 6.5 / 10
                    aLed7SegCoords(6, 4) = FontLed7SegCurrentSizeX * 8.3 / 10
                    aLed7SegCoords(6, 5) = FontLed7SegCurrentSizeY * 6.5 / 10
                    aLed7SegCoords(6, 6) = FontLed7SegCurrentSizeX * 9.0 / 10
                    aLed7SegCoords(6, 7) = FontLed7SegCurrentSizeY * 7.0 / 10
                    aLed7SegCoords(6, 8) = FontLed7SegCurrentSizeX * 8.2 / 10
                    aLed7SegCoords(6, 9) = FontLed7SegCurrentSizeY * 7.5 / 10
                    aLed7SegCoords(6, 10) = FontLed7SegCurrentSizeX * 2.9 / 10
                    aLed7SegCoords(6, 11) = FontLed7SegCurrentSizeY * 7.5 / 10
                    aLed7SegCoords(6, 12) = FontLed7SegCurrentSizeX * 2.0 / 10
                    aLed7SegCoords(6, 13) = FontLed7SegCurrentSizeY * 7.0 / 10

            End Select
        End Sub


#Region "GDDProps"

        <Description("Style for the FontLed7Seg")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Property Style() As FontLed7SegStyle
            Get
                Return _Style
            End Get
            Set(ByVal value As FontLed7SegStyle)
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

        <Description("Event handling for this FontLed7Seg")> _
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

        <Description("Value or variable do be displayed by the FontLed7Seg")> _
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

        <Description("Number of digits to display")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
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
        <Category("VGDD")> _
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
        <Category("VGDD")> _
        Public Property Thickness() As Integer
            Get
                Return _Thickness
            End Get
            Set(ByVal value As Integer)
                _Thickness = value
                Me.Invalidate()
            End Set
        End Property

        <Description("ColorVGDDScheme for the FontLed7Seg")> _
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

        <Description("Wether to enable a frame around the FontLed7Seg or not")> _
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

        <Description("Status of the FontLed7Seg")> _
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

        '<Description("Text Alignement of the FontLed7Seg")> _
        '<EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        '<Category("VGDD")> _
        'Public Shadows Property TextAlign() As HorizAlign
        '    Get
        '        Return _TextAlign
        '    End Get
        '    Set(ByVal value As HorizAlign)
        '        _TextAlign = value
        '        Me.Invalidate()
        '    End Set
        'End Property

        <Description("Visibility of the FontLed7Seg")> _
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
                If Not (" Text BackColor Font " & PROPSTOREMOVE).Contains(" " & pd.Name & " ") Then
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
            Dim MyCodeHead As String = ""
            Dim MyCode As String = "", MyState As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            MyCode &= CodeGen.ConstructorTemplate
            MyCode &= CodeGen.CodeTemplate & CodeGen.AllCodeTemplate
            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)
            CodeGen.AddState(MyState, "Visible", Me.Visible.ToString)
            'CodeGen.AddState(MyState, "HorizAlign", Me.TextAlign.ToString)
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
                .Replace("[STATE]", MyState).Replace("[VALUE]", Me.Value) _
                .Replace("[NUMDIGITS]", Me._NumDigits) _
                .Replace("[DOTPOS]", Me._DotPos) _
                .Replace("[THICKNESS]", Me._Thickness) _
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

            CodeGen.Headers &= CodeGen.HeadersTemplate.Replace("[CONTROLID]", MyControlId) _
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

        Private Sub FontLed7Seg_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
            Try
                FontLed7SegSetSize(Me.Height * 0.75 - Thickness * 2, (Me.Width \ (Me.NumDigits + 1)) - Thickness * 2, Me.Thickness)

            Catch ex As Exception

            End Try
        End Sub
    End Class

End Namespace