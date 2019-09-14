Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D
Imports System.ComponentModel.Design
Imports System.Drawing.Design

Partial Public Class AquaGaugeVb
    Inherits Control

#Region "Private Attributes"

    Private _MinValue As Int16 = 0
    Private _MaxValue As Int16 = 100
    Private currentValue As Int16
    Private newValue As Int16
    Private _NumOfDivisions As Int16
    Private _NumOfSubDivisions As Byte
    Private _DialText As String = "VirtualFab"
    Private _DialColor As Color = Color.Lavender
    Private oldWidth As Int16, oldHeight As Int16
    Private BorderWidth As Byte
    Private _AngleFrom As Int16 = 135
    Private _AngleTo As Int16 = 405
    Private requiresRedraw As Boolean
    Private backgroundImg As Image
    'Private rectImg As Rectangle
    Private _RectImgX As Int16, _RectImgY As Int16, _RectImgWidth As Int16, _RectImgHeight As Int16
    Private _Animated As Boolean
    Private WithEvents AnimateTimer As Timer
    Private _Segments As New List(Of GaugeSegment)
    Private _DialTextOffsetX As Int16 = 0
    Private _DialTextOffsetY As Int16 = 18

    Private _NumberOfDigits As Byte = 3

    Private _SingleDigitSizeX As Byte = 5
    Private _SingleDigitSizeY As Byte = 2

    Private _OffsetX As Int16 = 0
    Private _OffsetY As Int16 = 30

#End Region


#Region "GOL"
    Const COSINETABLEENTRIES As Byte = 90
    ' Cosine table used to calculate angles when rendering circular objects and  arcs  
    ' Make cosine values * 256 instead of 100 for easier math later
    Public _CosineTable As Int16() = { _
           256, 256, 256, 256, 255, 255, 255, 254, 254, 253, _
           252, 251, 250, 249, 248, 247, 246, 245, 243, 242, _
           241, 239, 237, 236, 234, 232, 230, 228, 226, 224, _
           222, 219, 217, 215, 212, 210, 207, 204, 202, 199, _
           196, 193, 190, 187, 184, 181, 178, 175, 171, 168, _
           165, 161, 158, 154, 150, 147, 143, 139, 136, 132, _
           128, 124, 120, 116, 112, 108, 104, 100, 96, 92, _
           88, 83, 79, 75, 71, 66, 62, 58, 53, 49, _
           44, 40, 36, 31, 27, 22, 18, 13, 9, 4, _
           0}
    Public Const GETSINE As Byte = 0
    Public Const GETCOSINE As Byte = 1

    Public Function GetSineCosine(ByVal degAngle As Int16, ByVal type As Byte) As Int16
        If (degAngle >= COSINETABLEENTRIES * 3) Then
            degAngle -= COSINETABLEENTRIES * 3
            GetSineCosine = (IIf(type = GETSINE, -(_CosineTable(degAngle)), (_CosineTable(COSINETABLEENTRIES - degAngle))))
        ElseIf (degAngle >= COSINETABLEENTRIES * 2) Then
            degAngle -= COSINETABLEENTRIES * 2
            GetSineCosine = IIf(type = GETSINE, -(_CosineTable((COSINETABLEENTRIES - degAngle))), -(_CosineTable(degAngle)))
        ElseIf (degAngle >= COSINETABLEENTRIES) Then
            degAngle -= COSINETABLEENTRIES
            GetSineCosine = IIf(type = GETSINE, (_CosineTable(degAngle)), -(_CosineTable(COSINETABLEENTRIES - degAngle)))
        Else
            GetSineCosine = IIf(type = GETSINE, (_CosineTable(COSINETABLEENTRIES - degAngle)), (_CosineTable(degAngle)))
        End If
    End Function

    Public Sub GetCirclePoint(ByVal radius As Int16, ByVal angle As Int16, ByRef x As Int16, ByRef y As Int16)
        'Dim radAngle As Single = GetRadian(angle)
        'Dim xg As Int16 = radius * Math.Cos(radAngle)
        'Dim yg As Int16 = radius * Math.Sin(radAngle)
        'Return

        Dim rad As UInt32
        Dim ang As Int16
        Dim temp As UInt32

        While angle < 0
            angle += 360 ' if angle is neg, convert to pos equivalent
        End While

        angle = angle Mod 360

        ang = angle Mod 45
        If ((angle \ 45) And 1) Then
            ang = 45 - ang
        End If

        rad = radius
        ' there is a shift by 8 bits here since this function assumes a shift on the calculations for accuracy
        ' and to avoid long and complex multiplications.
        temp = GetSineCosine(ang, GETCOSINE)
        x = ((temp << 8) * rad) >> 16

        temp = GetSineCosine(ang, GETSINE)
        y = ((temp << 8) * rad) >> 16

        If (((angle > 45) And (angle < 135)) Or ((angle > 225) And (angle < 315))) Then
            temp = x
            x = y
            y = temp
        End If

        If ((angle > 90) And (angle < 270)) Then
            x = -x
        End If

        If ((angle > 180) And (angle < 360)) Then
            y = -y
        End If
        'Debug.Print(String.Format("{8} {6}->{7} - {4} {5}   x={0} xg={1}   y={2} yg={3}", x, xg, y, yg, Math.Abs(x - xg), Math.Abs(y - yg), angle, ang, (angle \ 45) And 1))
    End Sub

#End Region

    Public Sub New()
        BorderWidth = 10
        _RectImgWidth = Me.Width - 10
        _RectImgHeight = Me.Height - 10
        Me._NumOfDivisions = 10
        Me._NumOfSubDivisions = 3
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.BackColor = Color.Transparent
        AddHandler Me.Resize, New EventHandler(AddressOf AquaGauge_Resize)
        Me.requiresRedraw = True
    End Sub

#Region "Digits"

    <Description("Number of Digits. 0=no digits")> _
    <Category("VG-Digits")> _
    Public Property NumberOfDigits As Byte
        Get
            Return _NumberOfDigits
        End Get
        Set(ByVal value As Byte)
            If value >= 0 And value < 10 Then
                _NumberOfDigits = value
                requiresRedraw = True
                Me.Invalidate()
            End If
        End Set
    End Property

    <Description("Digits Horizontal Size (% of width)")> _
    <Category("VG-Digits")> _
    Public Property SingleDigitSizeX As Byte
        Get
            Return _SingleDigitSizeX
        End Get
        Set(ByVal value As Byte)
            If value < 100 Then
                _SingleDigitSizeX = value
                requiresRedraw = True
                Me.Invalidate()
            End If
        End Set
    End Property

    <Description("Digit Vertical Size (% of width)")> _
    <Category("VG-Digits")> _
    Public Property SingleDigitSizeY As Byte
        Get
            Return _SingleDigitSizeY
        End Get
        Set(ByVal value As Byte)
            If value < 100 Then
                _SingleDigitSizeY = value
                requiresRedraw = True
                Me.Invalidate()
            End If
        End Set
    End Property

    <Description("Digits Horizontal Offset from centre (% of width)")> _
    <Category("VG-Digits")> _
    Public Property OffsetX As Int16
        Get
            Return _OffsetX
        End Get
        Set(ByVal value As Int16)
            If value < 100 Then
                _OffsetX = value
                requiresRedraw = True
                Me.Invalidate()
            End If
        End Set
    End Property

    <Description("Digits Vertical Offset from centre (% of width)")> _
    <Category("VG-Digits")> _
    Public Property OffsetY As Int16
        Get
            Return _OffsetY
        End Get
        Set(ByVal value As Int16)
            If value < 100 Then
                _OffsetY = value
                requiresRedraw = True
                Me.Invalidate()
            End If
        End Set
    End Property
#End Region

#Region "VGDDProps"
    '<NotifyParentProperty(True)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    <Description("Segments to be coloured")> _
    <Category("VG-Style")> _
    Public Property Segments As List(Of GaugeSegment)
        Get
            Return _Segments
        End Get
        Set(ByVal value As List(Of GaugeSegment))
            _Segments = value
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
#End Region

#Region "Public Properties"

    <DefaultValue(False)> _
    <Description("Should the Widget be animated")> _
    <Category("VG")> _
    Public Property Animated() As Boolean
        Get
            Return _Animated
        End Get
        Set(ByVal value As Boolean)
            _Animated = value
            If _Animated Then
                AnimateTimer = New Timer
                AnimateTimer.Interval = 5
                AnimateTimer.Enabled = True
            ElseIf AnimateTimer IsNot Nothing Then
                AnimateTimer.Stop()
                AnimateTimer.Dispose()
                AnimateTimer = Nothing
            End If
        End Set
    End Property

    ''' <summary>
    ''' Mininum value on the scale
    ''' </summary>
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

    ''' <summary>
    ''' Maximum value on the scale
    ''' </summary>
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

    ''' <summary>
    ''' Value where the pointer will point to.
    ''' </summary>
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
                Me.Refresh()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Background color of the dial
    ''' </summary>
    <Description("Background color of the dial")> _
    <Category("VG-Style")> _
    Public Property DialColor() As Color
        Get
            Return _DialColor
        End Get
        Set(ByVal value As Color)
            _DialColor = value
            requiresRedraw = True
            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Get or Sets the number of Divisions in the dial scale.
    ''' </summary>
    <DefaultValue(10)> _
    <Description("Get or Sets the number of Divisions in the dial scale.")> _
    <Category("VG-Style")> _
    Public Property NoOfDivisions() As Int16
        Get
            Return Me._NumOfDivisions
        End Get
        Set(ByVal value As Int16)
            If value > 1 AndAlso value < 25 Then
                Me._NumOfDivisions = value
                requiresRedraw = True
                Me.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or Sets the number of Sub Divisions in the scale per Division.
    ''' </summary>
    <DefaultValue(3)> _
    <Description("Gets or Sets the number of Sub Divisions in the scale per Division.")> _
    <Category("VG-Style")> _
    Public Property NoOfSubDivisions() As Byte
        Get
            Return Me._NumOfSubDivisions
        End Get
        Set(ByVal value As Byte)
            If value > 0 AndAlso value <= 10 Then
                Me._NumOfSubDivisions = value
                requiresRedraw = True
                Me.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or Sets the Text to be displayed in the dial
    ''' </summary>
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

#End Region

#Region "Overriden Control methods"
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim g As Graphics
        Dim cx As Int16 = Me.Width \ 2
        Dim cy As Int16 = Me.Height \ 2
        Dim radius As Int16
        Dim textSize As SizeF
        Dim x As Int16, y As Int16, x1 As Int16, y1 As Int16
        If backgroundImg Is Nothing OrElse requiresRedraw Then
            backgroundImg = New Bitmap(Me.Width, Me.Height)
            g = Graphics.FromImage(backgroundImg)
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
            _RectImgWidth = Me.Width - BorderWidth * 2
            _RectImgHeight = Me.Height - BorderWidth * 2
            _RectImgX = BorderWidth
            _RectImgY = BorderWidth

            'Draw background color
            Dim backGroundBrush As Brush = New SolidBrush(Color.FromArgb(120, _DialColor))
            g.FillEllipse(backGroundBrush, BorderWidth, BorderWidth, _RectImgWidth, _RectImgHeight)

            'Draw Rim
            Dim outlinePen As New Pen(Color.SlateGray, _RectImgWidth * 2 \ 100)
            g.DrawEllipse(outlinePen, _RectImgX, _RectImgY, _RectImgWidth, _RectImgHeight)

            ''Draw Colored Rim
            Dim gap As Int16 = Me.Width * 11.5 / 100

            'Draw Segments
            Dim SegmentProp As Int16 = (CType((_AngleTo - _AngleFrom), Int32) << 8) / (MaxValue - MinValue)
            For Each oSegment As GaugeSegment In _Segments
                Dim SegmentAngleFrom As Int16 = (((oSegment.ValueFrom - MinValue) * SegmentProp) >> 8) + _AngleFrom
                If SegmentAngleFrom < _AngleFrom Then
                    SegmentAngleFrom = _AngleFrom
                End If
                Dim SegmentAngleTo As Int16 = (((oSegment.ValueTo - MinValue) * SegmentProp) >> 8) + _AngleFrom
                If SegmentAngleTo > _AngleTo Then
                    SegmentAngleTo = _AngleTo
                End If
                If SegmentAngleFrom > 0 And SegmentAngleTo > 0 Then
                    g.DrawArc(New Pen(Color.FromArgb(200, oSegment.SegmentColour), Me.Width \ 50), _
                              _RectImgX + gap, _RectImgY + gap, _RectImgWidth - gap * 2, _RectImgHeight - gap * 2, _
                              SegmentAngleFrom, SegmentAngleTo - SegmentAngleFrom)
                End If
            Next

            'Draw Calibration
            Dim shift As Int16 = Me.Width \ 40
            Dim incrDeg As Int16 = ((_AngleTo - _AngleFrom) / _NumOfDivisions)
            Dim rulerValue As Int16 = MinValue
            Dim rulerValueIncr As Int16 = (MaxValue - MinValue) \ _NumOfDivisions
            Dim currentDegAngle As Int16 = _AngleFrom
            radius = (_RectImgWidth >> 1) - (_RectImgWidth * 10 \ 100)
            For i As Int16 = 0 To _NumOfDivisions + 1
                'Draw Thick Line
                GetCirclePoint(radius, currentDegAngle, x, y)
                GetCirclePoint(radius - _RectImgWidth \ 20, currentDegAngle, x1, y1)
                g.DrawLine(New Pen(Color.Black, _RectImgWidth \ 50), x + cx, y + cy, x1 + cx, y1 + cy)

                'Draw Strings
                Dim format As New StringFormat()
                GetCirclePoint(radius + _RectImgWidth \ 20, currentDegAngle, x, y)
                Dim stringPen As Brush = New SolidBrush(Me.ForeColor)
                Dim f As New Font(Me.Font.FontFamily, _RectImgWidth \ 23, Me.Font.Style)
                Dim strString As String = rulerValue.ToString
                textSize = g.MeasureString(strString, f)
                g.DrawString(strString, f, stringPen, x + cx - (textSize.Width >> 1), y + cy - shift)
                rulerValue += rulerValueIncr

                If i = _NumOfDivisions Then
                    Exit For
                End If

                'Draw thin lines 
                Dim subIncrDeg As Int16 = (incrDeg << 8) \ _NumOfSubDivisions
                Dim subDegAngle As Int16
                Dim subDivHeigth As Int16 = _RectImgWidth >> 6
                For j As Int16 = 1 To _NumOfSubDivisions
                    subDegAngle = currentDegAngle + ((subIncrDeg * j) >> 8)
                    GetCirclePoint(radius, subDegAngle, x, y)
                    GetCirclePoint(radius - subDivHeigth, subDegAngle, x1, y1)
                    g.DrawLine(New Pen(Color.Black, _RectImgWidth \ 100), x + cx, y + cy, x1 + cx, y1 + cy)
                Next
                currentDegAngle += incrDeg
            Next


            requiresRedraw = False
        End If
        e.Graphics.DrawImage(backgroundImg, _RectImgX, _RectImgY, _RectImgWidth, _RectImgHeight)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        'Draws the center point
        Dim CenterDia As Int16 = Me.Width \ 5
        Dim brush As New SolidBrush(Color.Black)
        e.Graphics.FillEllipse(brush, cx - (CenterDia \ 2), cy - (CenterDia \ 2), CenterDia, CenterDia)
        CenterDia = Me.Width \ 7
        brush = New SolidBrush(Color.SlateGray)
        e.Graphics.FillEllipse(brush, cx - (CenterDia \ 2), cy - (CenterDia \ 2), CenterDia, CenterDia)

        'Draw Pointer
        'radius = (Me.Width >> 1) - (Me.Width * 18 \ 100)
        radius = (_RectImgWidth >> 1) - (_RectImgWidth * 18 \ 100)
        Dim degAngle As Int16 = (Me.currentValue - MinValue) * 100 / (MaxValue - MinValue)
        degAngle = _AngleFrom + ((_AngleTo - _AngleFrom) * degAngle) / 100

        GetCirclePoint(radius, degAngle, x, y)
        Dim DrawWidth As Int16 = Me.Width * 5 \ 100
        Dim DrawStep As Int16 = Me.Width * 2 \ 1000 + 1
        Dim DrawRadius As Int16 = Me.Width * 10 \ 100
        For i As Byte = 0 To DrawWidth Step DrawStep
            Dim cColor1 As Int16 = 60 - i * 2
            Dim cColor2 As Int16 = 120 - i * 2
            If cColor1 < 0 Then cColor1 = 0
            If cColor2 < 0 Then cColor2 = 0
            GetCirclePoint(DrawRadius, degAngle - i, x1, y1)
            e.Graphics.DrawLine(New Pen(Color.FromArgb(cColor1, cColor1, cColor1), 2), x1 + cx, y1 + cy, x + cx, y + cy)
            GetCirclePoint(DrawRadius, degAngle + i, x1, y1)
            e.Graphics.DrawLine(New Pen(Color.FromArgb(cColor2, cColor2, cColor2), 2), x1 + cx, y1 + cy, x + cx, y + cy)
        Next

        'Draw Digital Value
        DisplayNumber(e.Graphics, Me.currentValue)

        'Draw text
        textSize = e.Graphics.MeasureString(Me._DialText, Me.Font)
        e.Graphics.DrawString(_DialText, Me.Font, New SolidBrush(Me.ForeColor), Me.Width \ 2 - textSize.Width \ 2 + (Me.Width * _DialTextOffsetX) \ 100, Me.Height \ 2 - textSize.Height \ 2 + (Me.Height * _DialTextOffsetY) \ 100)
    End Sub

#End Region

#Region "Private methods"

    ' Displays the given number in the 7-Segement format.
    Private Sub DisplayNumber(ByVal g As Graphics, ByVal number As Int16)
        Try
            Dim num As String = number.ToString(StrDup(_NumberOfDigits, "0"c))
            Dim shift As Int16 = 0
            If number < 0 Then
                shift -= _RectImgWidth \ 17
            End If
            Dim drawDPS As Boolean = False
            Dim chars As Char() = num.ToCharArray()
            Dim DigitWidth As Int16 = (Me.Width * _SingleDigitSizeX) \ 100
            Dim DigitHeight As Int16 = (Me.Height * _SingleDigitSizeY) \ 100
            Dim OrigX As Int16 = Me.Width * _OffsetX \ 100 + ((Me.Width - DigitWidth * _NumberOfDigits) >> 1)
            Dim OrigY As Int16 = Me.Height * _OffsetY \ 100 + ((Me.Height - DigitHeight) >> 1)
            For i As Byte = 0 To chars.Length - 1
                Dim c As Char = chars(i)
                If i < chars.Length - 1 AndAlso chars(i + 1) = "."c Then
                    drawDPS = True
                Else
                    drawDPS = False
                End If
                If c <> "."c Then
                    If c = "-"c Then
                        DrawDigit(g, -1, OrigX + shift, OrigY, drawDPS, DigitHeight)
                    Else
                        DrawDigit(g, Int16.Parse(c.ToString()), OrigX + shift, OrigY, drawDPS, DigitHeight)
                    End If
                    shift += DigitWidth  ' 15 * Me.Width \ 220
                Else
                    shift += DigitWidth  '2 * Me.Width \ 220
                End If
            Next
        Catch generatedExceptionName As Exception
        End Try
    End Sub

    Private Sub DrawDigit(ByVal g As Graphics, ByVal number As Int16, ByVal posX As Int16, ByVal posY As Int16, ByVal dp As Boolean, ByVal height As Int16)
        Dim width As Int16
        width = 10.0F * height / 13

        Dim outline As New Pen(Color.FromArgb(40, Me._DialColor))
        Dim fillPen As New Pen(Color.Black)

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

    ''' <summary>
    ''' Restricts the size to make sure the height and width are always same.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub AquaGauge_Resize(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Width < 136 Then
            Me.Width = 136
        End If
        If oldWidth <> Me.Width Then
            Me.Height = Me.Width
            oldHeight = Me.Width
        End If
        If oldHeight <> Me.Height Then
            Me.Width = Me.Height
            oldWidth = Me.Width
        End If
        requiresRedraw = True
    End Sub

#End Region

    Private Sub AnimateTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles AnimateTimer.Tick
        AnimateTimer.Enabled = False
        If currentValue < newValue Then
            currentValue += 1
            Me.Refresh()
        ElseIf currentValue > newValue Then
            currentValue -= 1
            Me.Refresh()
        End If
        If Me.currentValue = newValue Then
            If Me.currentValue = _MaxValue Then
                Me.newValue = _MinValue
            ElseIf Me.currentValue = _MinValue Then
                Me.newValue = _MaxValue
            End If
        End If
        AnimateTimer.Enabled = True
    End Sub

    'Private Function GetCos(degAngle) As Single
    '    Dim Cos1 As Single, Cos2 As Single
    '    Dim radAngle As Single = GetRadian(degAngle)
    '    Cos1 = Math.Cos(radAngle)
    '    Dim Cos2Int32 As Int32 = GetSineCosine(degAngle, GETCOSINE)
    '    Cos2Int32 = Cos2Int32 >> 8
    '    Cos2 = Cos2Int32
    '    Return Cos2
    'End Function

    'Private Function GetSin(degAngle) As Single
    '    Dim Sin1 As Single, Sin2 As Single
    '    Dim radAngle As Single = GetRadian(degAngle)
    '    Sin1 = Math.Sin(radAngle)
    '    Dim Sin2Int32 As Int32 = (GetSineCosine(degAngle, GETSINE) << 8)
    '    Sin2 = Sin2Int32
    '    Return Sin2
    'End Function


    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    '<TypeConverter(GetType(CaptionConverter))> _
    '<Editor(GetType(System.ComponentModel.Design.ObjectSelectorEditor), GetType(System.Drawing.Design.UITypeEditor))> _
    Public Class GaugeSegment
        Private _ValueFrom As Int16
        Private _ValueTo As Int16
        Private _SegmentColour As Color

        Public Property ValueFrom As Int16
            Get
                Return _ValueFrom
            End Get
            Set(ByVal value As Int16)
                _ValueFrom = value
            End Set
        End Property

        Public Property ValueTo As Int16
            Get
                Return _ValueTo
            End Get
            Set(ByVal value As Int16)
                _ValueTo = value
            End Set
        End Property

        Public Property SegmentColour As Color
            Get
                Return _SegmentColour
            End Get
            Set(ByVal value As Color)
                _SegmentColour = value
            End Set
        End Property

    End Class

    '' This is a special type converter which will be associated with the Caption class.
    '' It converts a Caption object to string representation for use in a property grid.
    'Friend Class CaptionConverter
    '    Inherits ExpandableObjectConverter

    '    Public Overloads Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destType As Type) As Object
    '        'Return "(Collection)"
    '        If destType Is GetType(String) AndAlso TypeOf value Is GaugeSegment Then
    '            ' Cast the value to an Caption type
    '            Dim emp As GaugeSegment = DirectCast(value, GaugeSegment)
    '            ' Return the text for display.
    '            Return emp.SegmentColour
    '        End If
    '        Return MyBase.ConvertTo(context, culture, value, destType)
    '    End Function
    'End Class


    'Public Class CaptionCollectionPropertyDescriptor
    '    Inherits PropertyDescriptor
    '    Private collection As CaptionCollection = Nothing
    '    Private index As Integer = -1

    '    Public Sub New(ByVal coll As CaptionCollection, ByVal idx As Integer)
    '        MyBase.New("#" & idx.ToString(), Nothing)
    '        Me.collection = coll
    '        Me.index = idx
    '    End Sub

    '    Public Overloads Overrides ReadOnly Property Attributes() As AttributeCollection
    '        Get
    '            Return New AttributeCollection(Nothing)
    '        End Get
    '    End Property

    '    Public Overloads Overrides Function CanResetValue(ByVal component As Object) As Boolean
    '        Return True
    '    End Function

    '    Public Overloads Overrides ReadOnly Property ComponentType() As Type
    '        Get
    '            Return Me.collection.[GetType]()
    '        End Get
    '    End Property

    '    Public Overloads Overrides ReadOnly Property DisplayName() As String
    '        Get
    '            Return "Caption" + (index + 1).ToString()
    '        End Get
    '    End Property

    '    Public Overloads Overrides ReadOnly Property Description() As String
    '        Get
    '            Return "Caption" + (index + 1).ToString()
    '        End Get
    '    End Property

    '    Public Overloads Overrides Function GetValue(ByVal component As Object) As Object
    '        Return Me.collection(index)
    '    End Function

    '    Public Overloads Overrides ReadOnly Property IsReadOnly() As Boolean
    '        Get
    '            Return False
    '        End Get
    '    End Property

    '    Public Overloads Overrides ReadOnly Property Name() As String
    '        Get
    '            Return "#" & index.ToString()
    '        End Get
    '    End Property

    '    Public Overloads Overrides ReadOnly Property PropertyType() As Type
    '        Get
    '            Return Me.collection(index).[GetType]()
    '        End Get
    '    End Property

    '    Public Overloads Overrides Sub ResetValue(ByVal component As Object)
    '    End Sub

    '    Public Overloads Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
    '        Return True
    '    End Function

    '    Public Overloads Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
    '        ' this.collection[index] = value;
    '    End Sub
    'End Class
End Class

'Public Class GenericCustomTypeConverter
'    Inherits ExpandableObjectConverter

'    Public Overloads Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
'        'If (destinationType Is GetType(DigitsProp)) Then
'        Return True
'        'End If
'        'Return MyBase.CanConvertFrom(context, destinationType)
'    End Function

'    'Public Overloads Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
'    '    If (destinationType Is GetType(System.String) AndAlso TypeOf value Is DigitsProp) Then
'    '        Dim so As DigitsProp = CType(value, DigitsProp)
'    '        Return "# digits: " & so.Number & _
'    '               ", SizeX: " & so.SizeX & _
'    '               ", SizeY: " & so.SizeY & _
'    '               ", OffsetX: " & so.OffsetX & _
'    '               ", OffsetY: " & so.OffsetY
'    '    End If
'    '    Return MyBase.ConvertTo(context, culture, value, destinationType)
'    'End Function

'    'Public Overloads Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object) As Object
'    '    If (TypeOf value Is String) Then
'    '        Try
'    '            Dim tokens() As String = CStr(value).Split(",")
'    '            If tokens.Length = 4 Then
'    '                Dim so As New DigitsProp
'    '                so.Number = tokens(0).Substring(":")
'    '                so.SizeX = tokens(1).Substring(":")
'    '                so.SizeY = tokens(2).Substring(":")
'    '                so.OffsetX = tokens(3).Substring(":")
'    '                so.OffsetY = tokens(4).Substring(":")
'    '                Return so
'    '            End If
'    '        Catch
'    '            Throw New ArgumentException("Can not convert '" & CStr(value) & "' to type DigitsProp")
'    '        End Try
'    '    End If
'    '    Return MyBase.ConvertFrom(context, culture, value)
'    'End Function
'End Class


