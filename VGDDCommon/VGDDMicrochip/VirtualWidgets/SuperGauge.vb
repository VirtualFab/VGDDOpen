Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D
Imports System.ComponentModel.Design
Imports System.Drawing.Design
Imports VGDDCommon
'Imports VGDDCommon.Common
Imports System.Xml
Imports VGDDCommon.Common
Imports System.Xml.Serialization

Namespace VGDDMicrochip

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
    Partial Public Class SuperGauge : Inherits VGDDWidget

        Private Shared _Instances As Integer = 0

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
        Private _Animated As Boolean = True
        Private _NoPanel As EnabledState

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

        Private _Segments As New SegmentsCollection
        Private _PointerType As PointerTypes = PointerTypes.Filled3D
        Private _PointerLine As Common.ThickNess = Common.ThickNess.THICK_LINE

        Private WithEvents AniTimer As New Timer

#End Region

        Public Sub New()
            MyBase.New()
            _Instances += 1
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.UserPaint, True)
            InitializeComponent()
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Meter")
#End If

            Me.Size = New Size(150, 150)
            Me.requiresRedraw = True
            _Segments.Add(New Segment(0, 50, Color.Green))
            _Segments.Add(New Segment(51, 80, Color.Yellow))
            _Segments.Add(New Segment(81, 100, Color.Red))
            CalcCenter()

        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Instances As Integer
            Get
                Return _Instances
            End Get
        End Property


#Region "SuperGauge Public Properties"

        <Description("Wether to enable the background panel or not")> _
        <DefaultValue(GetType(EnabledState), "Disabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Property NoPanel() As EnabledState
            Get
                Return _NoPanel
            End Get
            Set(ByVal value As EnabledState)
                _NoPanel = value
                Me.Invalidate()
            End Set
        End Property

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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("VG-Gauge")> _
        Public Property MinValue() As Integer
            Get
                Return _MinValue
            End Get
            Set(ByVal value As Integer)
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("VG-Gauge")> _
        Public Property MaxValue() As Integer
            Get
                Return _MaxValue
            End Get
            Set(ByVal value As Integer)
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 360)> _
        <Category("VG-Style")> _
        Public Property AngleFrom As Integer
            Get
                Return _AngleFrom
            End Get
            Set(ByVal value As Integer)
                _AngleFrom = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Angle to end to")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 360)> _
        <Category("VG-Style")> _
        Public Property AngleTo As Integer
            Get
                Return _AngleTo
            End Get
            Set(ByVal value As Integer)
                _AngleTo = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <DefaultValue(0)> _
        <Description("Value where the pointer will point to.")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("VG-Gauge")> _
        Public Property Value() As Integer
            Get
                Return currentValue
            End Get
            Set(ByVal value As Integer)
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
                MyBase.Text = value
                Me._DialText = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Text Horizontal Offset from centre (% of width)")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-DialText")> _
        Public Property DialTextOffsetX As Integer
            Get
                Return _DialTextOffsetX
            End Get
            Set(ByVal value As Integer)
                If value < 100 Then
                    _DialTextOffsetX = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Text Vertical Offset from centre (% of width)")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-DialText")> _
        Public Property DialTextOffsetY As Integer
            Get
                Return _DialTextOffsetY
            End Get
            Set(ByVal value As Integer)
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-Pointer")> _
        Public Property PointerSize As Integer
            Get
                Return _PointerSize
            End Get
            Set(ByVal value As Integer)
                _PointerSize = value
                requiresRedraw = True
                Me.Invalidate()
            End Set
        End Property

        <Description("Centre size (% of width) of the pointer")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-Pointer")> _
        Public Property PointerCenterSize As Integer
            Get
                Return _PointerCenterSize
            End Get
            Set(ByVal value As Integer)
                If value < 100 Then
                    _PointerCenterSize = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(10)> _
        <Description("Get or Sets the number of Divisions in the dial scale.")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 200)> _
        <Category("VG-DialScale")> _
        Public Property DialScaleNumDivisions() As Integer
            Get
                Return Me._DialScaleNumDivisions
            End Get
            Set(ByVal value As Integer)
                If value >= 0 AndAlso value < 50 Then
                    Me._DialScaleNumDivisions = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(3)> _
        <Description("Gets or Sets the number of Sub Divisions in the scale per Division.")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 200)> _
        <Category("VG-DialScale")> _
        Public Property DialScaleNumSubDivisions() As Integer
            Get
                Return Me._DialScaleNumSubDivisions
            End Get
            Set(ByVal value As Integer)
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
        Public Property Segments As SegmentsCollection
            Get
                Return _Segments
            End Get
            Set(ByVal value As SegmentsCollection)
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 20)> _
        <Category("VG-Digits")> _
        Public Property DigitsNumber As Integer
            Get
                Return _DigitsNumber
            End Get
            Set(ByVal value As Integer)
                If value >= 0 And value < 10 Then
                    _DigitsNumber = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digits Horizontal Size (% of width)")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-Digits")> _
        Public Property DigitsSizeX As Integer
            Get
                Return _DigitsSizeX
            End Get
            Set(ByVal value As Integer)
                If value < 100 Then
                    _DigitsSizeX = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digit Vertical Size (% of width)")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-Digits")> _
        Public Property DigitsSizeY As Integer
            Get
                Return _DigitsSizeY
            End Get
            Set(ByVal value As Integer)
                If value < 100 Then
                    _DigitsSizeY = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digits Horizontal Offset from centre (% of width)")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-Digits")> _
        Public Property DigitsOffsetX As Integer
            Get
                Return _DigitsOffsetX
            End Get
            Set(ByVal value As Integer)
                If value < 100 Then
                    _DigitsOffsetX = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Digits Vertical Offset from centre (% of width)")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 100)> _
        <Category("VG-Digits")> _
        Public Property DigitsOffsetY As Integer
            Get
                Return _DigitsOffsetY
            End Get
            Set(ByVal value As Integer)
                If value < 100 Then
                    _DigitsOffsetY = value
                    requiresRedraw = True
                    Me.Invalidate()
                End If
            End Set
        End Property
#End Region

#Region "VGDDProps"

        <Description("Should the Player Animate this Widget?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(True)> _
        <CustomSortedCategory("Player", 8)> _
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

        <Description("Has the object to be declared public when generating code?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Overloads ReadOnly Property [Public]() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overloads Property Scheme() As String
            Get
                Return MyBase.Scheme
            End Get
            Set(ByVal value As String)
                MyBase.Scheme = value
                If _DialScaleFont Is Nothing Then _DialScaleFont = _Scheme.Font
                Me.Invalidate()
            End Set
        End Property

        <Description("Visibility of the Gauge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Hidden() As Boolean
            Get
                Return _Hidden
            End Get
            Set(ByVal value As Boolean)
                _Hidden = value
                Me.Invalidate()
            End Set
        End Property

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
                If Me.Left < 0 Then Me.Left = 0
                If Me.Top < 0 Then Me.Top = 0
                If value.Width > value.Height Then value.Height = value.Width
                If value.Height > value.Width Then value.Width = value.Height
                MyBase.Size = value
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
        Public Shadows Property Width() As Integer
            Get
                Return MyBase.Width
            End Get
            Set(ByVal value As Integer)
                If Me.Left < 0 Then Me.Left = 0
                If Me.Top < 0 Then Me.Top = 0
                'If value >= 136 Then
                MyBase.Width = value
                'End If
                'MyBase.Height = value
            End Set
        End Property

        <System.ComponentModel.Browsable(False)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
            Try
                Dim g As Graphics
                Dim radius As Int16
                Dim textSize As SizeF
                Dim x As Int16, y As Int16, x1 As Int16, y1 As Int16
                If _Scheme Is Nothing Or Me.Hidden Then Exit Sub
                If backgroundImg Is Nothing OrElse requiresRedraw OrElse DesignMode Then
                    backgroundImg = New Bitmap(Me.Width, Me.Height)
                    g = Graphics.FromImage(backgroundImg)
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality

                    Dim backGroundBrush As Brush = New SolidBrush(_Scheme.Commonbkcolor)
                    Dim outlinePen As New Pen(CType(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Color1, _Scheme.Textcolordisabled), Color), RectImgWidth * 2 \ 100)

                    If _NoPanel = EnabledState.Disabled Then
                        'Draw background color
                        g.FillPie(backGroundBrush, outlinePen.Width, outlinePen.Width, RectImgVirtualWidth - outlinePen.Width * 2, RectImgVirtualWidth - outlinePen.Width * 2, OuterAngleFrom, OuterAngleTo - OuterAngleFrom)
                    End If
                    'Draw Rim
                    g.DrawArc(outlinePen, RectImgX + outlinePen.Width, RectImgY + outlinePen.Width, RectImgVirtualWidth - outlinePen.Width * 2, RectImgVirtualWidth - outlinePen.Width * 2, OuterAngleFrom, OuterAngleTo - OuterAngleFrom)

                    'Draw Colored Rim
                    Dim gap As Int16 = Me.Width * 11.5 / 100

                    'Draw Segments
                    Dim SegmentProp As Int16 = (CType((_AngleTo - _AngleFrom), Int32) << 8) / (MaxValue - MinValue)
                    If _Segments IsNot Nothing Then
                        For Each oSegment As Segment In _Segments.List
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
                                    Try
                                        g.DrawArc(New Pen(Color.FromArgb(200, IIf(Me.State = Common.EnabledState.Enabled, oSegment.SegmentColour, _Scheme.Textcolordisabled)), RectImgWidth \ 20), _
                                                  RectImgX + gap, RectImgY + gap, RectImgVirtualWidth - gap * 2, RectImgVirtualHeight - gap * 2, _
                                                  SegmentAngleFrom, SegmentAngleTo - SegmentAngleFrom)
                                    Catch ex As Exception
                                    End Try
                                End If
                            End If
                        Next
                    End If

                    If _DialScaleNumDivisions > 0 Then
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
                            If _Scheme IsNot Nothing AndAlso _DialScaleFont IsNot Nothing AndAlso _DialScaleFont.Font IsNot Nothing Then
                                Try
                                    textSize = g.MeasureString(strString, _DialScaleFont.Font)
                                Catch ex As Exception
                                    textSize = New SizeF(10, 18)
                                End Try
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
                                Dim subDivHeight As Int16 = RectImgWidth >> 6
                                For j As Int16 = 1 To _DialScaleNumSubDivisions
                                    subDegAngle = currentDegAngle + ((subIncrDeg * j) >> 8)
                                    Common.GOL.GetCirclePoint(radius, subDegAngle, x, y)
                                    Common.GOL.GetCirclePoint(radius - subDivHeight, subDegAngle, x1, y1)
                                    g.DrawLine(New Pen(CType(IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Color1, _Scheme.Textcolordisabled), Color), RectImgWidth \ 100), x + cx, y + cy, x1 + cx, y1 + cy)
                                Next
                            End If
                        Next
                    End If

                    'Draw text
                    If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Font IsNot Nothing Then
                        Try
                            textSize = g.MeasureString(Me._DialText, _Scheme.Font.Font)
                        Catch ex As Exception
                            textSize = New SizeF(10, 18)
                        End Try
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
                        If _PointerType = PointerTypes.Pie Or (_Segments Is Nothing OrElse _Segments.Count = 0) Then
                            PieColour = PointerColourDark
                        ElseIf _Segments IsNot Nothing Then
                            For Each oSegment As Segment In _Segments
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
                If _DigitsNumber > 0 Then
                    DrawDigits(e.Graphics, Me.currentValue)
                End If

            Catch ex As Exception

            End Try

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
                Dim DigitWidth As Int16 = (Me.RectImgHeight * _DigitsSizeX) \ 100
                Dim DigitHeight As Int16 = (Me.RectImgHeight * _DigitsSizeY) \ 100
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
                        shift += DigitWidth * 1  ' 15 * Me.Width \ 220
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
            'If Me.Width < 136 Then
            '    Me.Width = 136
            'End If
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

#Region "VGDDCode"

#If Not PlayerMonolitico Then
        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.MyCodeHead(_CDeclType)
            Dim MyCode As String = "", MyState As String = ""

            Dim MyClassName As String = Me.GetType.ToString

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            CodeGen.AddLines(MyCodeHead, CodeGen.CodeHeadTemplate)

            MyCode &= CodeGen.CodeTemplate & CodeGen.AllCodeTemplate.Trim
            CodeGen.AddState(MyState, "Enabled", Me.State.ToString)
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "PointerType", Me.PointerType.ToString)
            CodeGen.AddState(MyState, "PointerLine", Me.PointerLine.ToString)
            CodeGen.AddState(MyState, "GaugeTypes", Me.GaugeType.ToString)
            CodeGen.AddState(MyState, "NoPanel", Me.NoPanel.ToString)

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.DialText, Me._Scheme.Font, myText)

            Dim strSegmentsArray As String = ""
            For Each oSegment As Segment In Segments
                strSegmentsArray &= String.Format(", {0}, {1}, {2}  ", oSegment.ValueFrom, oSegment.ValueTo, CodeGen.UInt162Hex(CodeGen.Color2Num(oSegment.SegmentColour, False, "SuperGauge " & Me.Name)))
            Next
            If strSegmentsArray <> "" Then strSegmentsArray = strSegmentsArray.Substring(1)

            Dim SegmentsCount As Integer = 0
            If _Segments IsNot Nothing Then SegmentsCount = _Segments.Count

            Dim MyParameters() As Byte = {0,
                                          Me.Value \ 256, Me.Value Mod 256, _
                                          _MinValue \ 256, _MinValue Mod 256,
                                          _MaxValue \ 256, _MaxValue Mod 256, _
                                          _GaugeType, _PointerType, _
                                          _AngleFrom \ 256, _AngleFrom Mod 256, _
                                          _AngleTo \ 256, _AngleTo Mod 256,
                                          _DialScaleNumDivisions, _DialScaleNumSubDivisions, _
                                          _DialTextOffsetX \ 256, _DialTextOffsetX Mod 256, _
                                          _DialTextOffsetY \ 256, _DialTextOffsetY Mod 256, _
                                          _PointerSize \ 256, _PointerSize Mod 256, _
                                          _PointerCenterSize, _DigitsNumber,
                                          _DigitsSizeX, _DigitsSizeY, _
                                          _DigitsOffsetX \ 256, _DigitsOffsetX Mod 256, _
                                          _DigitsOffsetY \ 256, _DigitsOffsetY Mod 256, _
                                          0}
            MyParameters(0) = MyParameters.Length - 1

            Dim cs As Byte = 0
            Dim strMyParameters As String = String.Empty
            For i As Integer = 0 To MyParameters.Length - 1
                If i = MyParameters(0) Then
                    MyParameters(i) = cs
                Else
                    cs = cs Xor MyParameters(i)
                End If
                strMyParameters &= String.Format(",0x{0:x2}", MyParameters(i))
            Next
            strMyParameters = "(unsigned char []){" & strMyParameters.Substring(1) & "}"

            CodeGen.AddLines(CodeGen.Code, MyCode _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[PARAMETERS]", strMyParameters) _
                .Replace("[DIALSCALEFONT]", Me.DialScaleFont) _
                .Replace("[SEGMENTSCOUNT]", SegmentsCount) _
                .Replace("[SEGMENTSARRAY]", strSegmentsArray) _
                .Replace("[WIDGETTEXT]", IIf(myText = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[VALUE]", Me.Value).Replace("[SCHEME]", Me.Scheme) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                )
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[SEGMENTSCOUNT]", SegmentsCount) _
                .Replace("[SEGMENTSARRAY]", strSegmentsArray) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            'If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
            '    CodeGen.CodeHead &= MyCodeHead
            'End If
            If Not CodeGen.HeadersIncludes.Contains(CodeGen.HeadersIncludesTemplate) Then ' #include "supergauge.h"
                CodeGen.AddLines(CodeGen.HeadersIncludes, CodeGen.HeadersIncludesTemplate)
            End If
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            Dim MyHeaders As String = String.Empty
            'If Me.Public Then
            '    CodeGen.AddLines(MyHeaders, "extern " & CodeGen.ConstructorTemplate.Trim)
            'End If
            If Me.DialText <> String.Empty Then
                CodeGen.AddLines(MyHeaders, CodeGen.MyHeader(_CDeclType))
            End If
            CodeGen.AddLines(MyHeaders, CodeGen.TextDeclareHeaderTemplate(_CDeclType))
            CodeGen.AddLines(MyHeaders, CodeGen.HeadersTemplate)

            CodeGen.AddLines(CodeGen.Headers, MyHeaders _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, Me)

            Try
                'Dim myAssembly As Reflection.Assembly = System.Reflection.Assembly.GetAssembly(Me.GetType)
                For Each oFolderNode As Xml.XmlNode In CodeGen.XmlTemplatesDoc.SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Project/*", MyClassName.Split(".")(1)))
                    MplabX.AddFile(oFolderNode)
                Next
            Catch ex As Exception
            End Try
        End Sub
#End If


        Private Sub SuperGauge_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                If _Scheme.Font.SmartCharSet Then
                    _Scheme.Font.SmartCharSetAddString(Me.DialText)
                End If
            End If
        End Sub
#End Region

    End Class


End Namespace
