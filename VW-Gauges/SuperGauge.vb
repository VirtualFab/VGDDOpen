Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D
Imports System.ComponentModel.Design
Imports System.Drawing.Design
Imports VGDDCommon
'Imports VGDDCommon.Common
Imports System.Xml

Namespace VGDD

    Public Enum GaugeTypes
        Full360
        Half180Up
        Half180Down
    End Enum

    Public Enum PointerTypes
        Normal
        Filled3D
        WireFrame
        Needle
        Pie
        ColouredPie
        FloatingBit
    End Enum

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(SuperGauge), "SuperGauge.ico")> _
    Partial Public Class SuperGauge : Inherits Control
        Implements ICustomTypeDescriptor

#Region "Private Attributes"

        Private currentValue As Int16
        Private newValue As Int16
        Private BorderWidth As Byte
        Private cx As Int16, cy As Int16
        Private requiresRedraw As Boolean
        Private backgroundImg As Image
        Private RectImgX As Int16, RectImgY As Int16
        Private RectImgWidth As Int16, RectImgHeight As Int16
        Private RectImgVirtualWidth As Int16, RectImgVirtualHeight As Int16
        Private OuterAngleFrom As Int16
        Private OuterAngleTo As Int16
        Private _Animated As Boolean

        Private _GaugeType As GaugeTypes = GaugeTypes.Full360
        Private _Value As Short = 1
        Private _MinValue As Int16 = 0
        Private _MaxValue As Int16 = 100
        Private _AngleFrom As Int16 = 135
        Private _AngleTo As Int16 = 405

        Private _DialScaleFont As VGDDFont
        Private _DialScaleNumDivisions As Byte = 10
        Private _DialScaleNumSubDivisions As Byte = 4

        Private _DialText As String = "VirtualFab"
        Private _DialTextOffsetX As Int16 = 0
        Private _DialTextOffsetY As Int16 = 18

        Private _PointerSize As Int16 = 5
        Private _PointerCenterSize As Byte = 5

        Private _DigitsNumber As Byte = 3
        Private _DigitsSizeX As Byte = 6
        Private _DigitsSizeY As Byte = 7
        Private _DigitsOffsetX As Int16 = 0
        Private _DigitsOffsetY As Int16 = 30

        Private _Segments As New GaugeSegmentsCollection
        Private _PointerType As PointerTypes = PointerTypes.Filled3D
        Private _PointerLine As Common.ThickNess = Common.ThickNess.THICK_LINE

        Private _State As Common.EnabledState = Common.EnabledState.Enabled
        Private _Scheme As VGDDScheme

        Private _CDeclType As Common.TextCDeclType = Common.TextCDeclType.ConstXcharArray
        Private _public As Boolean
        'Private _Visible As Boolean = True
        Private _Events As VGDDEventsCollection

        Private WithEvents AniTimer As New Timer

        'Private oldWidth As Int16, oldHeight As Int16
        'Private rectImg As Rectangle
        'Private degRange As Single = _AngleTo - _AngleFrom
        'Private m_valueRange As Single = degRange
        'Private m_tickWarningColor As Color = Color.Red
        'Private m_tickColor As Color
        'Private m_tickIncrement As Integer = 10
        'Private degPercent As Single = 1 / (m_valueRange / degRange)
        'Private m_useCustNumSize As Boolean
        'Private m_custNumSize As Single

#End Region

        Public Sub New()
            'RectImgWidth = Me.Width - 10
            'RectImgHeight = Me.Height - 10
            'Me._DialScaleNumDivisions = 10
            'Me._DialScaleNumSubDivisions = 3
            'Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.UserPaint, True)
            'Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            'Me.BackColor = Color.Transparent
            InitializeComponent()
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Meter")
#End If

            'AddHandler Me.Resize, New EventHandler(AddressOf AquaGauge_Resize)
            Me.Size = New Size(150, 150)
            Me.requiresRedraw = True
        End Sub

#Region "SuperGauge Public Properties"

        <Description("Type of the Gauge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VG-Style")> _
        Public Property GaugeType() As GaugeTypes
            Get
                Return _GaugeType
            End Get
            Set(ByVal value As GaugeTypes)
                _GaugeType = value
                If Not Common.CodegenGeneratingCode AndAlso _DigitsOffsetY = 0 Then
                    Select Case _GaugeType
                        Case GaugeTypes.Full360
                            _DigitsOffsetY = 26
                            _DialTextOffsetY = 20
                            _AngleFrom = 135
                            _AngleTo = 405
                        Case GaugeTypes.Half180Up
                            If _AngleFrom < 180 Then _AngleFrom = 180
                            If _AngleTo > 360 Then _AngleTo = 360
                            Me.Height \= 2
                            _DigitsOffsetY = -10
                            _DialTextOffsetY = 20
                    End Select
                End If
                CalcCenter()
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <DefaultValue(0)> _
        <Description("Mininum value on the scale")> _
        <Category("VG-Gauge")> _
        Public Property MinValue() As Int16
            Get
                Return _MinValue
            End Get
            Set(ByVal value As Int16)
                If value < _MaxValue Then
                    _MinValue = value
                    If currentValue < _MinValue Then
                        currentValue = _MinValue
                    End If
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(100)> _
        <Description("Maximum value on the scale")> _
        <Category("VG-Gauge")> _
        Public Property MaxValue() As Int16
            Get
                Return _MaxValue
            End Get
            Set(ByVal value As Int16)
                If value > _MinValue Then
                    _MaxValue = value
                    If currentValue > _MaxValue Then
                        currentValue = _MaxValue
                    End If
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Angle to start from")> _
        <Category("VG-Style")> _
        Public Property AngleFrom As Int16
            Get
                Return _AngleFrom
            End Get
            Set(ByVal value As Int16)
                _AngleFrom = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Angle to end to")> _
        <Category("VG-Style")> _
        Public Property AngleTo As Int16
            Get
                Return _AngleTo
            End Get
            Set(ByVal value As Int16)
                _AngleTo = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <DefaultValue(0)> _
        <Description("Value where the pointer will point to.")> _
        <Category("VG-Gauge")> _
        Public Property Value() As Int16
            Get
                Return currentValue
            End Get
            Set(ByVal value As Int16)
                If value >= _MinValue AndAlso value <= _MaxValue Then
                    newValue = value
                    If DesignMode Then
                        currentValue = value
                    End If
                    Me.Refresh()
                End If
            End Set
        End Property

        <Description("Gets or Sets the Text to be displayed in the dial")> _
        <Category("VG-DialText")> _
        Public Property DialText() As String
            Get
                Return Me._DialText
            End Get
            Set(ByVal value As String)
                Me._DialText = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Text Horizontal Offset from centre (% of width)")> _
        <Category("VG-DialText")> _
        Public Property DialTextOffsetX As Int16
            Get
                Return _DialTextOffsetX
            End Get
            Set(ByVal value As Int16)
                If value < 100 Then
                    _DialTextOffsetX = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Text Vertical Offset from centre (% of width)")> _
        <Category("VG-DialText")> _
        Public Property DialTextOffsetY As Int16
            Get
                Return _DialTextOffsetY
            End Get
            Set(ByVal value As Int16)
                If value < 100 Then
                    _DialTextOffsetY = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Type of Pointer")> _
        <Category("VG-Pointer")> _
        Public Property PointerType As PointerTypes
            Get
                Return _PointerType
            End Get
            Set(ByVal value As PointerTypes)
                _PointerType = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Type of Pointer Line")> _
        <Category("VG-Pointer")> _
        Public Property PointerLine As Common.ThickNess
            Get
                Return _PointerLine
            End Get
            Set(ByVal value As Common.ThickNess)
                _PointerLine = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Size of the needle (% of width)")> _
        <Category("VG-Pointer")> _
        Public Property PointerSize As Byte
            Get
                Return _PointerSize
            End Get
            Set(ByVal value As Byte)
                _PointerSize = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Centre size (% of width) of the pointer")> _
        <Category("VG-Pointer")> _
        Public Property PointerCenterSize As Int16
            Get
                Return _PointerCenterSize
            End Get
            Set(ByVal value As Int16)
                If value < 100 Then
                    _PointerCenterSize = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(10)> _
        <Description("Get or Sets the number of Divisions in the dial scale.")> _
        <Category("VG-DialScale")> _
        Public Property DialScaleNumDivisions() As Int16
            Get
                Return Me._DialScaleNumDivisions
            End Get
            Set(ByVal value As Int16)
                If value > 1 AndAlso value < 25 Then
                    Me._DialScaleNumDivisions = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(3)> _
        <Description("Gets or Sets the number of Sub Divisions in the scale per Division.")> _
        <Category("VG-DialScale")> _
        Public Property DialScaleNumSubDivisions() As Byte
            Get
                Return Me._DialScaleNumSubDivisions
            End Get
            Set(ByVal value As Byte)
                If value >= 0 AndAlso value <= 10 Then
                    Me._DialScaleNumSubDivisions = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        '<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
        '<NotifyParentProperty(True)> _
        <Description("Segments to be coloured")> _
        <Category("VG-DialScale")> _
        Public Property Segments As GaugeSegmentsCollection
            Get
                Return _Segments
            End Get
            Set(ByVal value As GaugeSegmentsCollection)
                _Segments = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Font for the Dial Scale of the SuperGauge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDFontNameChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("VG-DialScale")> _
        Public Property DialScaleFont() As String
#Else
        Public Property DialScaleFont() As String

#End If
            Get
                If _DialScaleFont IsNot Nothing Then
                    Return _DialScaleFont.Name
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                _DialScaleFont = Common.GetFont(value, Me)
                Me.Invalidate()
            End Set
        End Property
#End Region

#Region "DigitsProps"

        <Description("Number of Digits. 0=no digits")> _
        <Category("VG-Digits")> _
        Public Property DigitsNumber As Byte
            Get
                Return _DigitsNumber
            End Get
            Set(ByVal value As Byte)
                If value >= 0 And value < 10 Then
                    _DigitsNumber = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digits Horizontal Size (% of width)")> _
        <Category("VG-Digits")> _
        Public Property DigitsSizeX As Byte
            Get
                Return _DigitsSizeX
            End Get
            Set(ByVal value As Byte)
                If value < 100 Then
                    _DigitsSizeX = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digit Vertical Size (% of width)")> _
        <Category("VG-Digits")> _
        Public Property DigitsSizeY As Byte
            Get
                Return _DigitsSizeY
            End Get
            Set(ByVal value As Byte)
                If value < 100 Then
                    _DigitsSizeY = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digits Horizontal Offset from centre (% of width)")> _
        <Category("VG-Digits")> _
        Public Property DigitsOffsetX As Int16
            Get
                Return _DigitsOffsetX
            End Get
            Set(ByVal value As Int16)
                If value < 100 Then
                    _DigitsOffsetX = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digits Vertical Offset from centre (% of width)")> _
        <Category("VG-Digits")> _
        Public Property DigitsOffsetY As Int16
            Get
                Return _DigitsOffsetY
            End Get
            Set(ByVal value As Int16)
                If value < 100 Then
                    _DigitsOffsetY = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property
#End Region

#Region "VGDDProps"

        <Description("How the text of the dial should be generated in code")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Property CDeclType() As Common.TextCDeclType
            Get
                Return _CDeclType
            End Get
            Set(ByVal value As Common.TextCDeclType)
                _CDeclType = value
            End Set
        End Property

        <Description("Should Player Animate this Widget?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD"), ParenthesizePropertyName(True)> _
        Public Property Animated() As Boolean
            Get
                Return _Animated
            End Get
            Set(ByVal value As Boolean)
                _Animated = value
                If Not DesignMode Then
                    If _Animated Then
                        AniTimer.Interval = 200
                        AniTimer.Start()
                    Else
                        AniTimer.Stop()
                    End If
                End If
            End Set
        End Property

        <Description("Event handling for this Gauge")> _
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

        <Description("Widget Type")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD"), ParenthesizePropertyName(True)> _
        Public ReadOnly Property WidgetType() As String
            Get
                Return Me.GetType.ToString.Split(".")(1)
            End Get
        End Property

        <Description("Sets the z-order of this widget (0=behind all others).")> _
        <Category("Layout")> _
        Property Zorder() As Integer
            Get
                Return MyBase.TabIndex
            End Get
            Set(ByVal value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property

        <Description("Has the object to be declared public when generating code?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Overloads Property [Public]() As Boolean
            Get
                Return _public
            End Get
            Set(ByVal value As Boolean)
                _public = value
            End Set
        End Property

        <Description("ColorVGDDScheme for the Gauge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <TypeConverter(GetType(Common.SchemesOptionConverter))> _
        <Category("VGDD")> _
        Public Property Scheme() As String
            Get
                If _Scheme IsNot Nothing Then
                    Return _Scheme.Name
                Else
                    Return String.Empty
                End If
            End Get
            Set(ByVal value As String)
                Dim SetScheme As VGDDScheme = Common.GetScheme(value, Me)
                If SetScheme IsNot Nothing Then
                    _Scheme = SetScheme
                    MyBase.Font = _Scheme.Font.Font
                    If _DialScaleFont Is Nothing Then _DialScaleFont = _Scheme.Font
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Status of the Gauge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Property State() As Common.EnabledState
            Get
                Return _State
            End Get
            Set(ByVal value As Common.EnabledState)
                _State = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Visibility of the Gauge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Overloads Property Visibility() As Boolean
            Get
                Return MyBase.Visible ' _Visible
            End Get
            Set(ByVal value As Boolean)
                '_Visible = value
                MyBase.Visible = value
                Me.Invalidate()
            End Set
        End Property

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not (" BackColor Font Text " & Common.PROPSTOREMOVE).Contains(" " & pd.Name & " ") Then
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

        <Description("Right X coordinate of the lower-right edge")> _
           <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
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
        Public Overloads Property Bottom() As Integer
            Get
                Return Me.Location.Y + Me.Height
            End Get
            Set(ByVal value As Integer)
                Me.Height = value - Me.Location.Y
                Me.Invalidate()
            End Set
        End Property

        <System.ComponentModel.Browsable(False)> _
        Public Shadows Property Width() As Integer
            Get
                Return MyBase.Width
            End Get
            Set(ByVal value As Integer)
                If value >= 136 Then
                    MyBase.Width = value
                End If
                'MyBase.Height = value
            End Set
        End Property

        <System.ComponentModel.Browsable(False)> _
        Public Shadows Property Height() As Integer
            Get
                Return MyBase.Height
            End Get
            Set(ByVal value As Integer)
                'MyBase.Width = value
                MyBase.Height = value
            End Set
        End Property
#End Region

#Region "Overriden Control methods"
        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
            Dim g As Graphics
            Dim radius As Int16
            Dim textSize As SizeF
            Dim x As Int16, y As Int16, x1 As Int16, y1 As Int16
            If _Scheme Is Nothing Or Not Me.Visibility Then Exit Sub
            If backgroundImg Is Nothing OrElse requiresRedraw OrElse DesignMode Then
                backgroundImg = New Bitmap(Me.Width, Me.Height)
                g = Graphics.FromImage(backgroundImg)
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality

                'Draw background color
                Dim backGroundBrush As Brush = New SolidBrush(_Scheme.Commonbkcolor)
                Dim outlinePen As New Pen(CType(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Textcolor1, _Scheme.Textcolordisabled), Color), RectImgWidth * 2 \ 100)

                g.FillPie(backGroundBrush, outlinePen.Width, outlinePen.Width, RectImgVirtualWidth - outlinePen.Width * 2, RectImgVirtualWidth - outlinePen.Width * 2, OuterAngleFrom, OuterAngleTo - OuterAngleFrom)

                'Draw Rim
                g.DrawArc(outlinePen, RectImgX + outlinePen.Width, RectImgY + outlinePen.Width, RectImgVirtualWidth - outlinePen.Width * 2, RectImgVirtualWidth - outlinePen.Width * 2, OuterAngleFrom, OuterAngleTo - OuterAngleFrom)

                'Draw Colored Rim
                Dim gap As Int16 = Me.Width * 11.5 / 100

                'Draw Segments
                Dim SegmentProp As Int16 = (CType((_AngleTo - _AngleFrom), Int32) << 8) / (MaxValue - MinValue)
                For Each oSegment As GaugeSegment In _Segments.List
                    If oSegment IsNot Nothing Then
                        Dim SegmentAngleFrom As Int16 = ((CType((oSegment.ValueFrom - MinValue), Int32) * SegmentProp) >> 8) + _AngleFrom
                        If SegmentAngleFrom < _AngleFrom Then
                            SegmentAngleFrom = _AngleFrom
                        End If
                        Dim SegmentAngleTo As Int16 = ((CType((oSegment.ValueTo - MinValue), Int32) * SegmentProp) >> 8) + _AngleFrom
                        If SegmentAngleTo > _AngleTo Then
                            SegmentAngleTo = _AngleTo
                        End If
                        If SegmentAngleFrom > 0 And SegmentAngleTo > 0 Then
                            g.DrawArc(New Pen(Color.FromArgb(200, IIf(Me.State = Common.EnabledState.Enabled, oSegment.SegmentColour, _Scheme.Textcolordisabled)), RectImgWidth \ 20), _
                                      RectImgX + gap, RectImgY + gap, RectImgVirtualWidth - gap * 2, RectImgVirtualHeight - gap * 2, _
                                      SegmentAngleFrom, SegmentAngleTo - SegmentAngleFrom)
                        End If
                    End If
                Next

                'Draw Calibration
                'Dim shift As Int16 = -_RectImgVirtualWidth \ 50
                Dim rulerValue As Int16 = MinValue
                Dim rulerValueIncr As Int16 = (MaxValue - MinValue) \ _DialScaleNumDivisions
                Dim currentDegAngle As Int16
                radius = (RectImgVirtualWidth >> 1) - (RectImgVirtualWidth * 10 \ 100)
                For i As Int16 = 0 To _DialScaleNumDivisions + 1
                    'degAngle = _AngleFrom + (CType((_AngleTo - _AngleFrom), Int32)) * degAngle / 100
                    currentDegAngle = _AngleFrom + ((CType((_AngleTo - _AngleFrom), Int32)) * i) \ _DialScaleNumDivisions
                    'Draw Thick Line
                    Common.GOL.GetCirclePoint(radius, currentDegAngle, x, y)
                    Common.GOL.GetCirclePoint(radius - RectImgWidth \ 20, currentDegAngle, x1, y1)
                    g.DrawLine(New Pen(_Scheme.Color0, RectImgWidth \ 50), x + cx, y + cy, x1 + cx, y1 + cy)

                    'Draw Strings
                    Dim format As New StringFormat()
                    Common.GOL.GetCirclePoint(radius + RectImgWidth \ 30, currentDegAngle, x, y)
                    Dim stringPen As Brush = New SolidBrush(CType(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Textcolor0, _Scheme.Textcolordisabled), Color))
                    'Dim f As New Font(Me.Font.FontFamily, _RectImgWidth \ 23, Me.Font.Style)
                    Dim strString As String = rulerValue.ToString
                    If _Scheme IsNot Nothing AndAlso _DialScaleFont.Font IsNot Nothing Then
                        textSize = g.MeasureString(strString, _DialScaleFont.Font)
                        g.DrawString(strString, _DialScaleFont.Font, stringPen, x + cx - (textSize.Width >> 1), y + cy - textSize.Height \ 2)
                    End If
                    rulerValue += rulerValueIncr

                    If i = _DialScaleNumDivisions Then
                        Exit For
                    End If

                    'Draw thin lines 
                    If _DialScaleNumSubDivisions > 0 Then
                        Dim subIncrDeg As Int16 = (((_AngleTo - _AngleFrom) \ _DialScaleNumDivisions) << 8) \ _DialScaleNumSubDivisions
                        Dim subDegAngle As Int16
                        Dim subDivHeigth As Int16 = RectImgWidth >> 6
                        For j As Int16 = 1 To _DialScaleNumSubDivisions
                            subDegAngle = currentDegAngle + ((subIncrDeg * j) >> 8)
                            Common.GOL.GetCirclePoint(radius, subDegAngle, x, y)
                            Common.GOL.GetCirclePoint(radius - subDivHeigth, subDegAngle, x1, y1)
                            g.DrawLine(New Pen(CType(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Color1, _Scheme.Textcolordisabled), Color), RectImgWidth \ 100), x + cx, y + cy, x1 + cx, y1 + cy)
                        Next
                    End If
                Next

                'Draw text
                If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Font IsNot Nothing Then
                    textSize = e.Graphics.MeasureString(Me._DialText, _Scheme.Font.Font)
                    g.DrawString(_DialText, _Scheme.Font.Font, New SolidBrush(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Textcolor0, _Scheme.Textcolordisabled)), _
                                          cx - textSize.Width \ 2 + (Me.Width * _DialTextOffsetX) \ 100, _
                                          cy + (Me.Height * _DialTextOffsetY) \ 100)
                End If

                requiresRedraw = False
            End If
            e.Graphics.DrawImage(backgroundImg, 0, 0, Me.Width, Me.Height)
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

            'Draws the center point
            radius = RectImgWidth * _PointerCenterSize \ 100
            Dim brush As New SolidBrush(CType(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Embossdkcolor, _Scheme.Textcolordisabled), Color))
            'e.Graphics.FillPie(brush, cx - (radius \ 2), cy - (radius \ 2), radius, radius, _OuterAngleFrom, _OuterAngleTo - _OuterAngleFrom)
            e.Graphics.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2)
            radius = RectImgWidth * _PointerCenterSize \ 200
            brush = New SolidBrush(CType(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Embossltcolor, _Scheme.Textcolordisabled), Color))
            'e.Graphics.FillPie(brush, cx - (radius \ 2), cy - (radius \ 2), radius, radius, _OuterAngleFrom, _OuterAngleTo - _OuterAngleFrom)
            e.Graphics.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2)

            'e.Graphics.DrawLine(New Pen(Color.Red, 2), cx, cy, x + cx, y + cy)

            'Draw Pointer
            radius = (RectImgWidth >> 1) - (RectImgWidth * 18 \ 100)
            Dim degAngle As Int16 = (Me.currentValue - MinValue) * 100 / (MaxValue - MinValue)
            degAngle = _AngleFrom + (CType((_AngleTo - _AngleFrom), Int32)) * degAngle / 100

            Common.GOL.GetCirclePoint(radius, degAngle, x, y)
            Dim PointerWidth As Int16 = CType(RectImgWidth, Int32) * _PointerSize \ 400
            Dim DrawStep As Int16 = RectImgWidth * 2 \ 1000 + 1
            Dim DrawRadius As Int16 = RectImgWidth * _PointerCenterSize \ 100 + 1
            Dim ColorGradient As Byte = PointerWidth / DrawStep + 1
            Dim k As Byte = 0
            Dim PointerColourDark As Color = IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Embossdkcolor, _Scheme.Textcolordisabled)
            Dim PointerColourLight As Color = IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Embossltcolor, _Scheme.Textcolordisabled)
            Dim PointerThickness As Integer = IIf(_PointerLine = Common.ThickNess.NORMAL_LINE, 1, 3)
            Select Case _PointerType
                Case PointerTypes.Normal
                    For i As Byte = 0 To PointerWidth Step DrawStep
                        Common.GOL.GetCirclePoint(DrawRadius, degAngle - i, x1, y1)
                        e.Graphics.DrawLine(New Pen(PointerColourDark, PointerThickness), x1 + cx, y1 + cy, x + cx, y + cy)
                        Common.GOL.GetCirclePoint(DrawRadius, degAngle + i, x1, y1)
                        e.Graphics.DrawLine(New Pen(PointerColourDark, PointerThickness), x1 + cx, y1 + cy, x + cx, y + cy)
                        k += 1
                    Next
                Case PointerTypes.Filled3D
                    For i As Byte = 0 To PointerWidth Step DrawStep
                        Common.GOL.GetCirclePoint(DrawRadius, degAngle - i, x1, y1)
                        e.Graphics.DrawLine(New Pen(PointerColourDark, PointerThickness), x1 + cx, y1 + cy, x + cx, y + cy)
                        Common.GOL.GetCirclePoint(DrawRadius, degAngle + i, x1, y1)
                        e.Graphics.DrawLine(New Pen(PointerColourLight, PointerThickness), x1 + cx, y1 + cy, x + cx, y + cy)
                        k += 1
                    Next
                Case PointerTypes.Needle
                    Common.GOL.GetCirclePoint(DrawRadius, degAngle, x1, y1)
                    e.Graphics.DrawLine(New Pen(PointerColourDark, PointerThickness), x1 + cx, y1 + cy, x + cx, y + cy)
                Case PointerTypes.Pie, PointerTypes.ColouredPie
                    Dim gap As Int16 = Me.Width * 26 / 100
                    Dim PieColour As Color
                    If _PointerType = PointerTypes.Pie Or _Segments.Count = 0 Then
                        PieColour = PointerColourDark
                    Else
                        For Each oSegment As GaugeSegment In _Segments
                            If oSegment.ValueFrom > Me.currentValue Then
                                Exit For
                            End If
                            PieColour = oSegment.SegmentColour
                        Next
                    End If
                    e.Graphics.DrawArc(New Pen(PieColour, Me.Width \ 4.6), _
                                        RectImgX + gap, RectImgY + gap, RectImgWidth - gap * 2, RectImgHeight - gap * 2, _
                                        _AngleFrom, degAngle - _AngleFrom)
                Case PointerTypes.WireFrame
                    Dim x2 As Int16, y2 As Int16
                    Common.GOL.GetCirclePoint(DrawRadius, degAngle - PointerWidth, x1, y1)
                    e.Graphics.DrawLine(New Pen(PointerColourDark, PointerThickness), x1 + cx, y1 + cy, x + cx, y + cy)
                    Common.GOL.GetCirclePoint(DrawRadius, degAngle + PointerWidth, x2, y2)
                    e.Graphics.DrawLine(New Pen(PointerColourDark, PointerThickness), x2 + cx, y2 + cy, x + cx, y + cy)
                    'e.Graphics.DrawLine(New Pen(PointerColourDark, 2), x1 + cx, y1 + cy, x2 + cx, y2 + cy)
                Case PointerTypes.FloatingBit
                    'Dim x2 As Int16, y2 As Int16
                    'Common.Gol.GetCirclePoint(radius * 0.8, degAngle + 2, x1, y1)
                    'Common.Gol.GetCirclePoint(radius * 0.8 - 5, degAngle - 2, x2, y2)
                    Dim gap As Int16 = radius * 0.5 'Me.Width * 11.5 / 100
                    e.Graphics.DrawArc(New Pen(PointerColourDark, Me.Width \ 20), _
                                        RectImgX + gap, RectImgY + gap, RectImgWidth - gap * 2, RectImgHeight - gap * 2, _
                                        degAngle - 2, 4)
            End Select

            'Draw Digital Value
            DrawDigits(e.Graphics, Me.currentValue)

        End Sub

#End Region

#Region "Private methods"

        ' Displays the given number in the 7-Segement format.
        Private Sub DrawDigits(ByVal g As Graphics, ByVal number As Int16)
            Try
                Dim num As String = number.ToString(StrDup(_DigitsNumber, "0"c))
                Dim shift As Int16 = 0
                If number < 0 Then
                    shift -= RectImgWidth \ 17
                End If
                Dim drawDPS As Boolean = False
                Dim chars As Char() = num.ToCharArray()
                Dim DigitWidth As Int16 = (Me.Width * _DigitsSizeX) \ 100
                Dim DigitHeight As Int16 = (Me.Height * _DigitsSizeY) \ 100
                Dim OrigX As Int16 = Me.Width * _DigitsOffsetX \ 100 + ((Me.Width - (DigitWidth * 1.1 * _DigitsNumber)) >> 1)
                Dim OrigY As Int16 = cy + Me.Height * _DigitsOffsetY \ 100
                For i As Byte = 0 To chars.Length - 1
                    Dim c As Char = chars(i)
                    If i < chars.Length - 1 AndAlso chars(i + 1) = "."c Then
                        drawDPS = True
                    Else
                        drawDPS = False
                    End If
                    If c <> "."c Then
                        If c = "-"c Then
                            DrawSingleDigit(g, -1, OrigX + shift, OrigY, drawDPS, DigitHeight)
                        Else
                            DrawSingleDigit(g, Int16.Parse(c.ToString()), OrigX + shift, OrigY, drawDPS, DigitHeight)
                        End If
                        shift += DigitWidth * 1.1  ' 15 * Me.Width \ 220
                    Else
                        shift += DigitWidth * 1.1  '2 * Me.Width \ 220
                    End If
                Next
            Catch generatedExceptionName As Exception
            End Try
        End Sub

        Private Sub DrawSingleDigit(ByVal g As Graphics, ByVal number As Int16, ByVal posX As Int16, ByVal posY As Int16, ByVal dp As Boolean, ByVal height As Int16)
            Dim width As Int16
            width = 10.0F * height / 13

            Dim outline As New Pen(Color.FromArgb(40, Me._Scheme.Commonbkcolor))
            Dim fillPen As New Pen(CType(IIf(Me.State = Common.EnabledState.Enabled, Me._Scheme.Textcolor1, Me._Scheme.Textcolordisabled), Color))

            '#Region "Form Polygon Points"
            'Segment A
            Dim segmentA As Point() = New Point(3) {}
            segmentA(0) = New Point(posX + width * 2.8F \ 10, posY + height * 1.0F \ 10)
            segmentA(1) = New Point(posX + width * 10 \ 10, posY + height * 1.0F \ 10)
            segmentA(2) = New Point(posX + width * 8.8F \ 10, posY + height * 2.0F \ 10)
            segmentA(3) = New Point(posX + width * 3.8F \ 10, posY + height * 2.0F \ 10)

            'Segment B
            Dim segmentB As Point() = New Point(3) {}
            segmentB(0) = New Point(posX + width * 10 \ 10, posY + height * 1.4F \ 10)
            segmentB(1) = New Point(posX + width * 9.3F \ 10, posY + height * 6.8F \ 10)
            segmentB(2) = New Point(posX + width * 8.4F \ 10, posY + height * 6.4F \ 10)
            segmentB(3) = New Point(posX + width * 9.0F \ 10, posY + height * 2.2F \ 10)

            'Segment C
            Dim segmentC As Point() = New Point(3) {}
            segmentC(0) = New Point(posX + width * 9.2F \ 10, posY + height * 7.2F \ 10)
            segmentC(1) = New Point(posX + width * 8.7F \ 10, posY + height * 12.7F \ 10)
            segmentC(2) = New Point(posX + width * 7.6F \ 10, posY + height * 11.9F \ 10)
            segmentC(3) = New Point(posX + width * 8.2F \ 10, posY + height * 7.7F \ 10)

            'Segment D
            Dim segmentD As Point() = New Point(3) {}
            segmentD(0) = New Point(posX + width * 7.4F \ 10, posY + height * 12.1F \ 10)
            segmentD(1) = New Point(posX + width * 8.4F \ 10, posY + height * 13.0F \ 10)
            segmentD(2) = New Point(posX + width * 1.3F \ 10, posY + height * 13.0F \ 10)
            segmentD(3) = New Point(posX + width * 2.2F \ 10, posY + height * 12.1F \ 10)

            'Segment E
            Dim segmentE As Point() = New Point(3) {}
            segmentE(0) = New Point(posX + width * 2.2F \ 10, posY + height * 11.8F \ 10)
            segmentE(1) = New Point(posX + width * 1.0F \ 10, posY + height * 12.7F \ 10)
            segmentE(2) = New Point(posX + width * 1.7F \ 10, posY + height * 7.2F \ 10)
            segmentE(3) = New Point(posX + width * 2.8F \ 10, posY + height * 7.7F \ 10)

            'Segment F
            Dim segmentF As Point() = New Point(3) {}
            segmentF(0) = New Point(posX + width * 3.0F \ 10, posY + height * 6.4F \ 10)
            segmentF(1) = New Point(posX + width * 1.8F \ 10, posY + height * 6.8F \ 10)
            segmentF(2) = New Point(posX + width * 2.6F \ 10, posY + height * 1.3F \ 10)
            segmentF(3) = New Point(posX + width * 3.6F \ 10, posY + height * 2.2F \ 10)

            'Segment G
            Dim segmentG As Point() = New Point(5) {}
            segmentG(0) = New Point(posX + width * 2.0F \ 10, posY + height * 7.0F \ 10)
            segmentG(1) = New Point(posX + width * 3.1F \ 10, posY + height * 6.5F \ 10)
            segmentG(2) = New Point(posX + width * 8.3F \ 10, posY + height * 6.5F \ 10)
            segmentG(3) = New Point(posX + width * 9.0F \ 10, posY + height * 7.0F \ 10)
            segmentG(4) = New Point(posX + width * 8.2F \ 10, posY + height * 7.5F \ 10)
            segmentG(5) = New Point(posX + width * 2.9F \ 10, posY + height * 7.5F \ 10)

            'Segment DP
            '#End Region

            '#Region "Draw Segments Outline"
            g.FillPolygon(outline.Brush, segmentA)
            g.FillPolygon(outline.Brush, segmentB)
            g.FillPolygon(outline.Brush, segmentC)
            g.FillPolygon(outline.Brush, segmentD)
            g.FillPolygon(outline.Brush, segmentE)
            g.FillPolygon(outline.Brush, segmentF)
            g.FillPolygon(outline.Brush, segmentG)
            '#End Region

            '#Region "Fill Segments"
            'Fill SegmentA
            If IsNumberAvailable(number, 0, 2, 3, 5, 6, 7, 8, 9) Then
                g.FillPolygon(fillPen.Brush, segmentA)
            End If

            'Fill SegmentB
            If IsNumberAvailable(number, 0, 1, 2, 3, 4, 7, 8, 9) Then
                g.FillPolygon(fillPen.Brush, segmentB)
            End If

            'Fill SegmentC
            If IsNumberAvailable(number, 0, 1, 3, 4, 5, 6, 7, 8, 9) Then
                g.FillPolygon(fillPen.Brush, segmentC)
            End If

            'Fill SegmentD
            If IsNumberAvailable(number, 0, 2, 3, 5, 6, 8, 9) Then
                g.FillPolygon(fillPen.Brush, segmentD)
            End If

            'Fill SegmentE
            If IsNumberAvailable(number, 0, 2, 6, 8) Then
                g.FillPolygon(fillPen.Brush, segmentE)
            End If

            'Fill SegmentF
            If IsNumberAvailable(number, 0, 4, 5, 6, 7, 8, 9) Then
                g.FillPolygon(fillPen.Brush, segmentF)
            End If

            'Fill SegmentG
            If IsNumberAvailable(number, 2, 3, 4, 5, 6, 8, 9, -1) Then
                g.FillPolygon(fillPen.Brush, segmentG)
            End If
            '#End Region

            'Draw decimal point
            If dp Then
                g.FillEllipse(fillPen.Brush, New RectangleF(posX + width * 10.0F \ 10, posY + height * 12.0F \ 10, width / 7, width / 7))
            End If
        End Sub

        ''' <summary>
        ''' Returns true if a given number is available in the given list.
        ''' </summary>
        ''' <param name="number"></param>
        ''' <param name="listOfNumbers"></param>
        ''' <returns></returns>

        Private Function IsNumberAvailable(ByVal number As Int16, ByVal ParamArray listOfNumbers As Int16()) As Boolean
            If listOfNumbers.Length > 0 Then
                For Each i As Int16 In listOfNumbers
                    If i = number Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        Private Sub CalcCenter()
            BorderWidth = 0 'Me.Width \ 50
            Select Case _GaugeType
                Case GaugeTypes.Full360
                    If Me.Width <> Me.Height Then Me.Width = Me.Height
                    cx = Me.Width \ 2
                    cy = Me.Height \ 2
                    OuterAngleFrom = 0
                    OuterAngleTo = 360
                    RectImgVirtualWidth = Me.Width - BorderWidth * 2
                    RectImgVirtualHeight = Me.Height - BorderWidth * 2
                Case GaugeTypes.Half180Up
                    If Me.Width <> Me.Height * 2 Then Me.Width = Me.Height * 2
                    cx = Me.Width \ 2
                    cy = Me.Height - BorderWidth * 2
                    OuterAngleFrom = 180
                    OuterAngleTo = 360
                    RectImgVirtualWidth = Me.Width - BorderWidth * 2
                    RectImgVirtualHeight = Me.Height * 2 - BorderWidth * 2
                Case GaugeTypes.Half180Down
                    If Me.Width <> Me.Height * 2 Then Me.Width = Me.Height * 2
                    cx = Me.Width \ 2
                    cy = BorderWidth * 2
                    OuterAngleFrom = 0
                    OuterAngleTo = 180
                    RectImgVirtualWidth = Me.Width - BorderWidth * 2
                    RectImgVirtualHeight = Me.Height * 2 - BorderWidth * 2
            End Select
            RectImgWidth = Me.Width - BorderWidth * 2
            RectImgHeight = Me.Height - BorderWidth * 2
            RectImgX = BorderWidth
            RectImgY = BorderWidth
            Me.Invalidate()
            requiresRedraw = True
        End Sub

        Private Sub SuperGauge_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            If Me.Width < 136 Then
                Me.Width = 136
            End If
            CalcCenter()
        End Sub

        Public Overrides Sub Refresh()
            requiresRedraw = True
            MyBase.Refresh()
            Me.Invalidate()
        End Sub

#End Region

        Private Sub AnimateTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles AniTimer.Tick
            AniTimer.Enabled = False
            If currentValue < newValue Then
                currentValue += 1
                'Me.Refresh()
                Me.Invalidate()
            ElseIf currentValue > newValue Then
                currentValue -= 1
                'Me.Refresh()
                Me.Invalidate()
            End If
            If Me.currentValue = newValue Then
                If Me.currentValue = _MaxValue Then
                    Me.newValue = _MinValue
                ElseIf Me.currentValue = _MinValue Then
                    Me.newValue = _MaxValue
                End If
            End If
            AniTimer.Enabled = True
        End Sub

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

        Private Function System_ComponentModel_ICustomTypeDescriptor_GetEvents() As EventDescriptorCollection Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, True)
        End Function

        Public Function GetComponentName() As String Implements ICustomTypeDescriptor.GetComponentName
            Return TypeDescriptor.GetComponentName(Me, True)
        End Function

        Public Function GetPropertyOwner(ByVal pd As PropertyDescriptor) As Object Implements ICustomTypeDescriptor.GetPropertyOwner
            Return Me
        End Function

        Public Function GetAttributes() As AttributeCollection Implements ICustomTypeDescriptor.GetAttributes
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

        Public Sub GetText(ByRef ReturnedText As String)
            ReturnedText = CodeGen.QText(Me.DialText, Me._Scheme.Font, Nothing)
        End Sub

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

            If _public Then
                MyCodeHead &= CodeGen.ConstructorTemplate.Trim & vbCrLf
            Else
                MyCode &= CodeGen.ConstructorTemplate
            End If
            MyCodeHead &= CodeGen.CodeHeadTemplate

            MyCode &= CodeGen.CodeTemplate & CodeGen.AllCodeTemplate
            CodeGen.AddState(MyState, "Enabled", Me.State.ToString)
            CodeGen.AddState(MyState, "Visible", Me.Visible.ToString)
            CodeGen.AddState(MyState, "PointerType", Me.PointerType.ToString)
            CodeGen.AddState(MyState, "PointerLine", Me.PointerLine.ToString)
            CodeGen.AddState(MyState, "GaugeTypes", Me.GaugeType.ToString)

#If CONFIG = "DemoRelease" Or CONFIG = "DemoDebug" Then
            MyCode = CodeGen.Scramble(MyCode)
#End If

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.DialText, Me._Scheme.Font, myText)

            Dim strSegmentsArray As String = ""
            For Each oSegment As GaugeSegment In Segments
                strSegmentsArray &= String.Format(", {0}, {1}, {2}  ", oSegment.ValueFrom, oSegment.ValueTo, CodeGen.UInt162Hex(CodeGen.Color2Num(oSegment.SegmentColour)))
            Next
            If strSegmentsArray <> "" Then strSegmentsArray = strSegmentsArray.Substring(1)

            CodeGen.Code &= MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[MAXVALUE]", Me.MaxValue) _
                .Replace("[MINVALUE]", Me.MinValue) _
                .Replace("[GAUGETYPE]", Me.GaugeType) _
                .Replace("[POINTERTYPE]", Me.PointerType) _
                .Replace("[ANGLEFROM]", Me.AngleFrom) _
                .Replace("[ANGLETO]", Me.AngleTo) _
                .Replace("[DIALSCALENUMDIVISIONS]", Me.DialScaleNumDivisions) _
                .Replace("[DIALSCALENUMSUBDIVISIONS]", Me.DialScaleNumSubDivisions) _
                .Replace("[DIALTEXTOFFSETX]", Me.DialTextOffsetX) _
                .Replace("[DIALTEXTOFFSETY]", Me.DialTextOffsetY) _
                .Replace("[POINTERSIZE]", Me.PointerSize) _
                .Replace("[POINTERCENTERSIZE]", Me.PointerCenterSize) _
                .Replace("[DIGITSNUMBER]", Me.DigitsNumber) _
                .Replace("[DIGITSSIZEX]", Me.DigitsSizeX) _
                .Replace("[DIGITSSIZEY]", Me.DigitsSizeY) _
                .Replace("[DIGITSOFFSETX]", Me.DigitsOffsetX) _
                .Replace("[DIGITSOFFSETY]", Me.DigitsOffsetY) _
                .Replace("[DIALSCALEFONT]", Me.DialScaleFont) _
                .Replace("[SEGMENTSCOUNT]", _Segments.Count) _
                .Replace("[SEGMENTSARRAY]", strSegmentsArray) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[VALUE]", Me.Value).Replace("[SCHEME]", Me.Scheme)
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[SEGMENTSCOUNT]", _Segments.Count) _
                .Replace("[SEGMENTSARRAY]", strSegmentsArray) _
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

        Private Sub Meter_QueryContinueDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.QueryContinueDragEventArgs) Handles Me.QueryContinueDrag
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                If _Scheme.Font.SmartCharSet Then
                    _Scheme.Font.SmartCharSetAddString(Me.DialText)
                End If
            End If
        End Sub
#End Region
    End Class

    <Serializable()> _
    Public Class GaugeSegment

        Private _ValueFrom As Int16
        Private _ValueTo As Int16
        Private _SegmentColour As Color

        Public Sub New()

        End Sub

        Public Sub New(ByRef oSegmentNode As XmlNode)
            _ValueFrom = oSegmentNode.Attributes("ValueFrom").Value
            _ValueTo = oSegmentNode.Attributes("ValueTo").Value
            _SegmentColour = Color.FromArgb(oSegmentNode.Attributes("SegmentColour").Value)
        End Sub

        '<XmlElement("ValueFrom")> _
        Public Property ValueFrom As Int16
            Get
                Return _ValueFrom
            End Get
            Set(ByVal value As Int16)
                _ValueFrom = value
            End Set
        End Property

        '<XmlElement("ValueTo")> _
        Public Property ValueTo As Int16
            Get
                Return _ValueTo
            End Get
            Set(ByVal value As Int16)
                _ValueTo = value
            End Set
        End Property

        '<XmlElement("SegmentColour")> _
        Public Property SegmentColour As Color
            Get
                Return _SegmentColour
            End Get
            Set(ByVal value As Color)
                _SegmentColour = value
            End Set
        End Property

    End Class

    '<System.Xml.Serialization.XmlRoot("VGDDEventsCollection")> _
    <Serializable()> _
    Public Class GaugeSegmentsCollection
        Inherits CollectionBase
        Implements IList

        Public Sub New()

        End Sub

        Public Sub New(ByRef XmlParent As XmlNode)
            For Each oSegmentNode As XmlElement In XmlParent.ChildNodes
                Dim oSegment As New GaugeSegment
                With oSegment
                    If oSegmentNode.Attributes("ValueFrom") IsNot Nothing Then
                        .ValueFrom = oSegmentNode.Attributes("ValueFrom").Value
                    End If
                    If oSegmentNode.Attributes("ValueTo") IsNot Nothing Then
                        .ValueTo = oSegmentNode.Attributes("ValueTo").Value
                    End If
                    If oSegmentNode.Attributes("SegmentColour") IsNot Nothing Then
                        .SegmentColour = Color.FromArgb(oSegmentNode.Attributes("SegmentColour").Value)
                    End If
                End With
                Me.Add(oSegment)
            Next
        End Sub

        Default Public ReadOnly Property Item(ByVal index As Integer) As GaugeSegment
            Get
                Return MyBase.List(index)
            End Get
        End Property

        Public Sub Add(ByRef oSegment As GaugeSegment)
            MyBase.List.Add(oSegment)
        End Sub

        Public Overloads ReadOnly Property List()
            Get
                Return MyBase.List
            End Get
        End Property

        Public Sub ToXml(ByRef XmlParent As XmlNode)
            Dim XmlDoc As XmlDocument = XmlParent.OwnerDocument
            For Each oSegment As GaugeSegment In MyBase.List
                Dim oEventNode As XmlElement = XmlDoc.CreateElement("Segment")
                Dim oAttr As XmlAttribute

                oAttr = XmlDoc.CreateAttribute("ValueFrom")
                oAttr.Value = oSegment.ValueFrom
                oEventNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("ValueTo")
                oAttr.Value = oSegment.ValueTo
                oEventNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("SegmentColour")
                oAttr.Value = oSegment.SegmentColour.ToArgb
                oEventNode.Attributes.Append(oAttr)

                XmlParent.AppendChild(oEventNode)
            Next
        End Sub
    End Class

End Namespace
