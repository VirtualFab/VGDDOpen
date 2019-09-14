Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports System.IO

Namespace VGDDCommon

    'Article Text http://www.codeproject.com/KB/miscctrl/ColorPickerControl.aspx

    <Designer(GetType(ColorPickerDesigner))> _
    <DefaultEvent("ColorPicked")> _
    Public Class ColorPicker
        Implements ISupportInitialize

        'Private ValueDisplay As Rectangle = New Rectangle(12, 5, 200, 25)
        Private bmpColorBar As Bitmap = New Bitmap(186, 26)
        Private bmpImg As Bitmap = New Bitmap(200, 25)
        Private CurrColor As Color = Color.Red
        Private ptColorPaint As Point
        Private ptColor As Point = New Point(12, 49)
        Private szColor As Size = New Size(180, 20)
        Private blnMouseDnColor As Boolean = False
        Private sngXColor As Single = ptColor.X
        Private HideCursor As Cursor
        Private blnOnHold As Boolean = True
        Private pnlCustomSelected As Panel
        Private strCustomColoursFile As String = "CustomColours"
        'List of Known Colors - Done this way because I haven't found a good
        'way to get the Known Colors in color shade order yet
        Private Known_Color() As String = Split("Transparent,Black,DimGray,Gray,DarkGray,Silver,LightGray,Gainsboro," & _
            "WhiteSmoke,White,RosyBrown,IndianRed,Brown,Firebrick,LightCoral,Maroon,DarkRed,Red,Snow,MistyRose," & _
            "Salmon,Tomato,DarkSalmon,Coral,OrangeRed,LightSalmon,Sienna,SeaShell,Chocalate,SaddleBrown,SandyBrown," & _
            "PeachPuff,Peru,Linen,Bisque,DarkOrange,BurlyWood,Tan,AntiqueWhite,NavajoWhite,BlanchedAlmond,PapayaWhip," & _
            "Mocassin,Orange,Wheat,OldLace,FloralWhite,DarkGoldenrod,Cornsilk,Gold,Khaki,LemonChiffon,PaleGoldenrod," & _
            "DarkKhaki,Beige,LightGoldenrod,Olive,Yellow,LightYellow,Ivory,OliveDrab,YellowGreen,DarkOliveGreen," & _
            "GreenYellow,Chartreuse,LawnGreen,DarkSeaGreen,ForestGreen,LimeGreen,PaleGreen,DarkGreen,Green,Lime," & _
            "Honeydew,SeaGreen,MediumSeaGreen,SpringGreen,MintCream,MediumSpringGreen,MediumAquaMarine," & _
            "YellowAquaMarine,Turquoise,LightSeaGreen,MediumTurquoise,DarkSlateGray,PaleTurquoise,Teal,DarkCyan,Aqua," & _
            "Cyan,LightCyan,Azure,DarkTurquoise,CadetBlue,PowderBlue,LightBlue,DeepSkyBlue,SkyBlue,LightSkyBlue," & _
            "SteelBlue,AliceBlue,DodgerBlue,SlateGray,LightSlateGray,LightSteelBlue,CornflowerBlue,RoyalBlue," & _
            "MidnightBlue,Lavender,Navy,DarkBlue,MediumBlue,Blue,GhostWhite,SlateBlue,DarkSlateBlue,MediumSlateBlue," & _
            "MediumPurple,BlueViolet,Indigo,DarkOrchid,DarkViolet,MediumOrchid,Thistle,Plum,Violet,Purple,DarkMagenta," & _
            "Magenta,Fuchsia,Orchid,MediumVioletRed,DeepPink,HotPink,LavenderBlush,PaleVioletRed,Crimson,Pink,LightPink", ",")

        Public Event ColorPicked(ByVal sender As Object)
        Public Event ColorChanging(ByVal sender As Object)
        Private Initializing As Boolean
        Private _Palette As VGDDPalette

        Private Sub BeginInit() Implements ISupportInitialize.BeginInit
            Initializing = True
        End Sub

        Private Sub EndInit() Implements ISupportInitialize.EndInit
            Initializing = False
        End Sub

#Region "Initialize"

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            Try
                InitializeComponent()
            Catch ex As Exception

            End Try

            ' Add any initialization after the InitializeComponent() call.
            Me.SetStyle(ControlStyles.DoubleBuffer, True)
            'Me.SetStyle(ControlStyles.UserPaint, True)
            'Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            CType(Me, ISupportInitialize).BeginInit()
            Value = Color.Red
            CType(Me, ISupportInitialize).EndInit()

        End Sub

        Private Sub ColorPicker_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ptColorPaint = ptColor
            ptColorPaint.Offset(-3, -3)

            'get the blank cursor to make the it dissapear when dragging
            Dim assem As Reflection.Assembly = Me.GetType().Assembly
            Dim my_namespace As String = assem.GetName().Name.ToString()
            Dim mystream As IO.Stream
            HideCursor = Cursors.Cross
            Try
                mystream = assem.GetManifestResourceStream("VGDDCommon.HideCursor.cur")
                If mystream IsNot Nothing Then
                    HideCursor = New Cursor(mystream)
                End If

            Catch ex As Exception
            End Try

            ColorBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            AddHandler ColorBox.DrawItem, AddressOf Me.ColorBox_DrawItem
            ColorBox.Items.AddRange(Known_Color)
            ColorBox.SelectedIndex = 1

            If Common.ProjectUsePalette Then
                'tabColors.TabPages.RemoveAt(2) ' No ColourPicker
                tabColors.TabPages.RemoveAt(1) ' No Known Colours
                tabColors.TabPages.RemoveAt(0) ' No Common Colours
                tabColors.SelectedTab = TabPalette
            End If

            'Try
            '    mystream = assem.GetManifestResourceStream(my_namespace & ".Lizardsml.jpg")
            '    If mystream IsNot Nothing Then
            '        bmpImg = CType(Image.FromStream(mystream), Bitmap)
            '    End If
            'Catch ex As Exception
            '    Using hb As HatchBrush = New HatchBrush(HatchStyle.Shingle, Color.Black, Color.Transparent)
            '        Dim bmI As Graphics = Graphics.FromImage(bmpImg)
            '        bmI.FillRectangle(hb, 0, 0, 210, 30)
            '        bmI.Dispose()
            '    End Using
            'End Try

            'AutoFlyOutToolStripMenuItem.Checked = CBool(_FlyOut)

            blnOnHold = False
            DrawCustomColours()

        End Sub

#End Region 'Initialize

#Region "Properties"

        Public Property Palette As VGDDPalette
            Get
                Return _Palette
            End Get
            Set(ByVal value As VGDDPalette)
                _Palette = value
                DrawCustomColours()
            End Set
        End Property

        Private Sub DrawCustomColours()
            If Palette IsNot Nothing AndAlso Palette.PaletteColours IsNot Nothing Then
                pnlPalette.Controls.Clear()
                For i As Integer = 0 To Palette.PaletteColours.Length - 1
                    Dim oPnlCustom As New Panel
                    With oPnlCustom
                        .Name = String.Format("pnlCustom{0:00}", i + 1)
                        .BorderStyle = BorderStyle.Fixed3D
                        .Size = New Size(18, 18)
                        .TabIndex = i
                        .Location = New Point((i Mod 8) * 22, Math.Truncate(i / 8) * 24)
                        .BackColor = Palette.PaletteColours(i)
                    End With
                    AddHandler oPnlCustom.Click, AddressOf ColorPanels_Click
                    AddHandler oPnlCustom.MouseEnter, AddressOf ColorPanels_MouseEnter
                    pnlPalette.Controls.Add(oPnlCustom)
                Next
            End If
            'If Palette IsNot Nothing AndAlso Palette.PaletteColours IsNot Nothing Then
            '    Dim i As Integer = 1
            '    For Each oColor As Color In Palette.PaletteColours
            '        Dim strPanelName = String.Format("pnlCustom{0:00}", i)
            '        Dim pnlCustom() As Control = pnlPalette.Controls.Find(strPanelName, False)
            '        If pnlCustom IsNot Nothing AndAlso pnlCustom.Length > 0 Then
            '            CType(pnlCustom(0), Panel).BackColor = oColor
            '        End If
            '        i += 1
            '    Next
            'End If
        End Sub

        Private _Value As Color
        <Category("Appearance ColorPicker")> _
        <Description("The current Color selected")> _
        Public Property Value() As Color
            Get
                Return _Value
            End Get
            Set(ByVal value As Color)
                UpdateRGB(value)
                _Value = value
                PreviewPanel.BackColor = value
                'Me.Invalidate(ValueDisplay)
                If Not Initializing Then
                    RaiseEvent ColorChanging(Me)
                End If
            End Set
        End Property

        'Enum eFlyOut
        '    Auto = -1
        '    Click = 0
        'End Enum
        'Private _FlyOut As eFlyOut = eFlyOut.Auto
        '<Category("Appearance ColorPicker")> _
        '<Description("Does the FlyOut open on MouseOver or Click")> _
        '<DefaultValue(eFlyOut.Auto)> _
        'Public Property FlyOut() As eFlyOut
        '    Get
        '        Return _FlyOut
        '    End Get
        '    Set(ByVal value As eFlyOut)
        '        _FlyOut = value
        '    End Set
        'End Property

#End Region 'Properties

#Region "Color Helpers"
        'Special thanks to Guillaume Leparmentier for his Great article
        'Manipulating colors in .NET - Part 1  http://www.codeproject.com/KB/recipes/colorspace1.aspx
        'I used some of his RGB/HSB code here with little modification

#Region "Color Structures"

        Public Structure RGB
#Region "Fields"

            Private _Red As Integer
            Private _Green As Integer
            Private _Blue As Integer

#End Region

#Region "Operators"

            Public Shared Operator =(ByVal item1 As RGB, ByVal item2 As RGB) As Boolean
                Return item1.Red = item2.Red AndAlso _
                       item1.Green = item2.Green AndAlso _
                       item1.Blue = item2.Blue
            End Operator

            Public Shared Operator <>(ByVal item1 As RGB, ByVal item2 As RGB) As Boolean
                Return item1.Red <> item2.Red OrElse _
                       item1.Green <> item2.Green OrElse _
                       item1.Blue <> item2.Blue
            End Operator

#End Region

#Region "Accessors"

            Public Property Red() As Integer
                Get
                    Return _Red
                End Get
                Set(ByVal value As Integer)
                    If value > 255 Then
                        _Red = 255
                    Else
                        If value < 0 Then
                            _Red = 0
                        Else
                            _Red = value
                        End If
                    End If
                End Set
            End Property

            Public Property Green() As Integer
                Get
                    Return _Green
                End Get
                Set(ByVal value As Integer)
                    If value > 255 Then
                        _Green = 255
                    Else
                        If value < 0 Then
                            _Green = 0
                        Else
                            _Green = value
                        End If
                    End If
                End Set
            End Property

            Public Property Blue() As Integer
                Get
                    Return _Blue
                End Get
                Set(ByVal value As Integer)
                    If value > 255 Then
                        _Blue = 255
                    Else
                        If value < 0 Then
                            _Blue = 0
                        Else
                            _Blue = value
                        End If
                    End If
                End Set
            End Property

#End Region

            ' Creates an instance of a RGB structure.
            Public Sub New(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
                Me.Red = r
                Me.Green = g
                Me.Blue = b
            End Sub

#Region "Methods"

            Public Overrides Function Equals(ByVal obj As Object) As Boolean
                If (obj Is Nothing) Or (Me.GetType() IsNot obj.GetType()) Then Return False
                Return (Me = CType(obj, RGB))
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return CInt(Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode())
            End Function

#End Region

        End Structure

        Public Structure HSB

#Region "Fields"

            Private _Hue As Double
            Private _Saturation As Double
            Private _Brightness As Double

#End Region

#Region "Operators"

            Public Shared Operator =(ByVal item1 As HSB, ByVal item2 As HSB) As Boolean
                Return item1.Hue = item2.Hue AndAlso _
                       item1.Saturation = item2.Saturation AndAlso _
                       item1.Brightness = item2.Brightness
            End Operator

            Public Shared Operator <>(ByVal item1 As HSB, ByVal item2 As HSB) As Boolean
                Return item1.Hue <> item2.Hue OrElse _
                       item1.Saturation <> item2.Saturation OrElse _
                       item1.Brightness <> item2.Brightness
            End Operator

#End Region

#Region "Accessors"

            Public Property Hue() As Double
                Get
                    Return _Hue
                End Get
                Set(ByVal value As Double)
                    If value > 360 Then
                        _Hue = 360
                    Else
                        If value < 0 Then
                            _Hue = 0
                        Else
                            _Hue = value
                        End If
                    End If
                End Set
            End Property

            Public Property Saturation() As Double
                Get
                    Return _Saturation
                End Get
                Set(ByVal value As Double)
                    If value > 1 Then
                        _Saturation = 1
                    Else
                        If value < 0 Then
                            _Saturation = 0
                        Else
                            _Saturation = value
                        End If
                    End If
                End Set
            End Property

            Public Property Brightness() As Double
                Get
                    Return _Brightness
                End Get
                Set(ByVal value As Double)
                    If value > 1 Then
                        _Brightness = 1
                    Else
                        If value < 0 Then
                            _Brightness = 0
                        Else
                            _Brightness = value
                        End If
                    End If
                End Set
            End Property

#End Region

            ' Creates an instance of a HSB structure.
            Public Sub New(ByVal h As Double, ByVal s As Double, ByVal b As Double)
                Me.Hue = h
                Me.Saturation = s
                Me.Brightness = b
            End Sub

#Region "Methods"

            Public Overrides Function Equals(ByVal obj As Object) As Boolean
                If (obj Is Nothing) Or (Me.GetType() IsNot obj.GetType()) Then Return False
                Return (Me = CType(obj, HSB))
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return CInt(Hue.GetHashCode() ^ Saturation.GetHashCode() ^ Brightness.GetHashCode())
            End Function

#End Region

        End Structure

#End Region 'Color Structures

#Region "RGBtoHSB"

        ' Converts RGB to a HSB.
        Public Shared Function RGBtoHSB(ByVal red As Integer, ByVal green As Integer, ByVal blue As Integer) As HSB

            Dim h As Double = 0.0
            Dim s As Double = 0.0

            ' normalizes red-green-blue values
            Dim nRed As Double = CDbl(red) / 255.0
            Dim nGreen As Double = CDbl(green) / 255.0
            Dim nBlue As Double = CDbl(blue) / 255.0

            Dim max As Double = Math.Max(nRed, Math.Max(nGreen, nBlue))
            Dim min As Double = Math.Min(nRed, Math.Min(nGreen, nBlue))

            ' Hue
            If (max = nRed) And (nGreen >= nBlue) Then
                If (max - min = 0) Then
                    h = 0.0
                Else
                    h = 60 * (nGreen - nBlue) / (max - min)
                End If
            ElseIf (max = nRed) And (nGreen < nBlue) Then
                h = 60 * (nGreen - nBlue) / (max - min) + 360
            ElseIf (max = nGreen) Then
                h = 60 * (nBlue - nRed) / (max - min) + 120
            ElseIf (max = nBlue) Then
                h = 60 * (nRed - nGreen) / (max - min) + 240
            End If

            If h > 359 Then h = 359

            ' Saturation
            If (max = 0) Then
                s = 0.0
            Else
                s = 1.0 - (min / max)
            End If

            Return New HSB(h, s, max)

        End Function

#End Region 'RGBtoHSB

#Region "HSBtoRGB"

        ' Converts HSB to RGB.
        Public Shared Function HSBtoRGB(ByVal HSB As HSB) As RGB
            Return HSBtoRGB(HSB.Hue, HSB.Saturation, HSB.Brightness)
        End Function

        ' Converts HSB to RGB.
        Public Shared Function HSBtoRGB(ByVal h As Integer, ByVal s As Integer, ByVal b As Integer) As RGB
            Return HSBtoRGB(CDbl(h), CDbl(s / 100.0), CDbl(b / 100.0))
        End Function

        ' Converts HSB to a RGB.
        Public Shared Function HSBtoRGB(ByVal h As Double, ByVal s As Double, ByVal b As Double) As RGB

            Dim red As Double = 0.0
            Dim green As Double = 0.0
            Dim blue As Double = 0.0

            If (s = 0) Then

                red = b
                green = b
                blue = b

            Else

                ' the color wheel consists of 6 sectors. Figure out which sector you're in.
                Dim sectorPos As Double = h / 60.0
                Dim sectorNumber As Integer = CInt(Math.Floor(sectorPos))
                ' get the fractional part of the sector
                Dim fractionalSector As Double = sectorPos - sectorNumber

                ' calculate values for the three axes of the color. 
                Dim p As Double = b * (1.0 - s)
                Dim q As Double = b * (1.0 - (s * fractionalSector))
                Dim t As Double = b * (1.0 - (s * (1 - fractionalSector)))

                ' assign the fractional colors to r, g, and b based on the sector the angle is in.
                Select Case sectorNumber
                    Case 0
                        red = b
                        green = t
                        blue = p
                    Case 1
                        red = q
                        green = b
                        blue = p
                    Case 2
                        red = p
                        green = b
                        blue = t
                    Case 3
                        red = p
                        green = q
                        blue = b
                    Case 4
                        red = t
                        green = p
                        blue = b
                    Case 5
                        red = b
                        green = p
                        blue = q
                End Select

            End If

            'Return New RGB(cint((Math.Ceiling(T(0) * 255.0)), _
            '               cint((Math.Ceiling(T(1) * 255.0)), _
            '               cint((Math.Ceiling(T(2) * 255.0)))

            Return New RGB(CInt(Double.Parse(String.Format("{0:0.00}", red * 255.0))), _
                           CInt(Double.Parse(String.Format("{0:0.00}", green * 255.0))), _
                           CInt(Double.Parse(String.Format("{0:0.00}", blue * 255.0))))


        End Function

#End Region 'HSBtoRGB

#Region "HSBtoColor"

        ' Converts HSB to .Net Color.
        Public Shared Function HSBtoColor(ByVal HSB As HSB) As Color
            Return HSBtoColor(HSB.Hue, HSB.Saturation, HSB.Brightness)
        End Function

        ' Converts HSB to a .Net Color.
        Public Shared Function HSBtoColor(ByVal h As Double, ByVal s As Double, ByVal b As Double) As Color
            Dim rgb As RGB = HSBtoRGB(h, s, b)
            Return Color.FromArgb(rgb.Red, rgb.Green, rgb.Blue)
        End Function

#End Region 'HSBtoColor

#End Region 'Color Helpers

#Region "Mouse Events"

        'Private Sub ColorPicker_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown

        '    'Is the mouse clicked within one of the Color Bars or Alpha Pointer
        '    If e.Button = Windows.Forms.MouseButtons.Left Then
        '        If ValueDisplay.Contains(e.Location) Then
        '            RaiseEvent ColorPicked(Me)
        '        End If
        '    End If
        'End Sub

        'Private Sub ColorPicker_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove

        '    'Switch the cursor to Hand if it is over the Color Display Rectangle
        '    If Cursor <> HideCursor AndAlso ValueDisplay.Contains(e.Location) Then
        '        Me.Cursor = Cursors.Hand
        '    Else
        '        Me.Cursor = Cursors.Default
        '    End If

        'End Sub

        'Private Sub ColorPicker_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        '    Cursor = Cursors.Default
        'End Sub

#End Region 'Mouse Events

#Region "Drawing"

        'Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        '    MyBase.OnPaint(e)

        '    Dim g As Graphics = e.Graphics

        '    ''Draw Color Value
        '    'g.FillRectangle(New TextureBrush(bmpImg, WrapMode.Tile), ValueDisplay)

        '    ''Draw a rectangle around the ValueDisplay
        '    'g.DrawRectangle(Pens.DarkGray, ValueDisplay.Location.X - 3, _
        '    '    ValueDisplay.Location.Y - 3, ValueDisplay.Width + 5, _
        '    '    ValueDisplay.Height + 5)

        'End Sub


#End Region 'Drawing

#Region "Control Events"

        Private Sub UpdateRGB(ByVal c As Color)
            GTBarRed.Value = c.R
            GTBarGreen.Value = c.G
            GTBarBlue.Value = c.B

        End Sub

        Private Sub ColorBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ColorBox.SelectedIndexChanged
            If Not blnOnHold Then
                UpdateRGB(Color.FromName(ColorBox.Text))
                'panSwatches.Width = 6
                'panSwatches.BorderStyle = Windows.Forms.BorderStyle.None
                'panSwatches2.Visible = False
            End If
        End Sub

        Private Sub ColorPanels_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Panel4.Click, Panel7.Click, Panel8.Click, Panel9.Click, Panel10.Click, Panel11.Click, _
            Panel12.Click, Panel13.Click, Panel20.Click, Panel19.Click, Panel14.Click, Panel15.Click, Panel16.Click, _
            Panel17.Click, Panel21.Click, Panel18.Click, Panel28.Click, Panel27.Click, Panel26.Click, Panel25.Click, _
            Panel24.Click, Panel23.Click, Panel6.Click, Panel29.Click, Panel5.Click, Panel48.Click, Panel47.Click, _
            Panel46.Click, Panel45.Click, Panel44.Click, Panel43.Click, Panel42.Click, Panel41.Click, Panel40.Click, _
            Panel22.Click, Panel39.Click, Panel38.Click, Panel37.Click, Panel36.Click, Panel35.Click, Panel34.Click, _
            Panel33.Click, Panel32.Click, Panel31.Click, Panel30.Click, Panel3.Click, Panel2.Click, Panel1.Click

            Dim pnl As Panel = CType(sender, Panel)
            UpdateRGB(pnl.BackColor)
            'panSwatches.Width = 6
            'panSwatches.BorderStyle = Windows.Forms.BorderStyle.None
            'panSwatches2.Visible = False
        End Sub

        Private CurrSwatch As Panel
        Private Sub ColorPanels_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Panel5.MouseEnter, Panel7.MouseEnter, Panel8.MouseEnter, Panel9.MouseEnter, Panel3.MouseEnter, Panel11.MouseEnter, _
            Panel12.MouseEnter, Panel13.MouseEnter, Panel20.MouseEnter, Panel19.MouseEnter, Panel14.MouseEnter, Panel15.MouseEnter, _
             Panel16.MouseEnter, Panel17.MouseEnter, Panel21.MouseEnter, Panel18.MouseEnter, Panel28.MouseEnter, Panel27.MouseEnter, _
             Panel26.MouseEnter, Panel25.MouseEnter, Panel24.MouseEnter, Panel23.MouseEnter, Panel6.MouseEnter, Panel29.MouseEnter, _
             Panel22.MouseEnter, Panel48.MouseEnter, Panel47.MouseEnter, Panel46.MouseEnter, Panel45.MouseEnter, Panel44.MouseEnter, _
             Panel43.MouseEnter, Panel42.MouseEnter, Panel41.MouseEnter, Panel40.MouseEnter, Panel4.MouseEnter, Panel39.MouseEnter, _
             Panel38.MouseEnter, Panel37.MouseEnter, Panel36.MouseEnter, Panel35.MouseEnter, Panel34.MouseEnter, Panel33.MouseEnter, _
             Panel32.MouseEnter, Panel31.MouseEnter, Panel30.MouseEnter, Panel10.MouseEnter, Panel2.MouseEnter, Panel1.MouseEnter

            Dim pnl As Panel = CType(sender, Panel)
            Try
                If CurrSwatch IsNot Nothing Then
                    CurrSwatch.BorderStyle = Windows.Forms.BorderStyle.Fixed3D
                End If
            Catch ex As Exception
            End Try
            CurrSwatch = pnl
            CurrSwatch.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
        End Sub


        'Private Sub nudRGB_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        '    Handles nudGreen.ValueChanged, nudBlue.ValueChanged
        '    tbarRed.Value = CInt(nudRed.Value)
        '    tbarGreen.Value = CInt(nudGreen.Value)
        '    tbarBlue.Value = CInt(nudBlue.Value)

        '    If Not blnOnHold AndAlso Not blnMouseDnColor AndAlso Not blnMouseDnBright AndAlso Not blnMouseDnSat Then
        '        blnOnHold = True
        '        UpdateHSB(Color.FromArgb(CInt(nudRed.Value), CInt(nudGreen.Value), CInt(nudBlue.Value)))
        '        blnOnHold = False
        '    End If
        'End Sub

        'Private Sub panSwatches_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        '    Handles panSwatches.Click
        '    If _FlyOut = eFlyOut.Click And panSwatches.Width = 6 Then
        '        OpenSwatchPanel()
        '    End If
        'End Sub

        'Private Sub panSwatches_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles panSwatches.MouseEnter
        '    If _FlyOut = eFlyOut.Auto And panSwatches.Width = 6 Then
        '        OpenSwatchPanel()
        '    End If

        'End Sub


#End Region

#Region "ColorBox" 'Known colors

        Private Sub ColorBox_DrawItem(ByVal sender As Object, _
            ByVal e As DrawItemEventArgs)

            If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
                e.Graphics.FillRectangle(Brushes.CornflowerBlue, e.Bounds)
            Else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds)
            End If

            Dim cbox As ListBox = CType(sender, ListBox)
            Dim itemString As String = CStr(cbox.Items(e.Index))
            Dim MyFont As Font = New Font("Microsoft Sans Serif", 8.25)
            Dim myBrush As New SolidBrush(Color.FromName(itemString))

            'Draw a Color Swatch
            e.Graphics.FillRectangle(myBrush, e.Bounds.X + 3, e.Bounds.Y + 2, 20, e.Bounds.Height - 5)
            e.Graphics.DrawRectangle(Pens.Black, e.Bounds.X + 3, e.Bounds.Y + 2, 20, e.Bounds.Height - 5)

            ' Draw the text in the item.
            e.Graphics.DrawString(itemString, MyFont, _
                Brushes.Black, e.Bounds.X + 25, e.Bounds.Y + 1)

            ' Draw the focus rectangle around the selected item.
            e.DrawFocusRectangle()
            myBrush.Dispose()
        End Sub

#End Region 'ColorBox

#Region "EyeDropper Events"

        Private Sub EyeDropper1_SelectedColorChanged(ByVal sender As System.Object, ByVal CurrColor As System.Drawing.Color) Handles EyeDropper1.SelectedColorChanged
            Me.Value = CurrColor
            'CloseSwatchPanel()
        End Sub

        Private Sub EyeDropper1_SelectedColorChanging(ByVal sender As Object, ByVal CurrColor As System.Drawing.Color) Handles EyeDropper1.SelectedColorChanging
            panEyedropper.BackColor = CurrColor
        End Sub

#End Region 'EyeDropper Events

        Private Sub GTBarBlue_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles GTBarRed.ValueChanged, GTBarGreen.ValueChanged, GTBarBlue.ValueChanged
            Me.Value = Color.FromArgb(CInt(GTBarRed.Value), CInt(GTBarGreen.Value), CInt(GTBarBlue.Value))
        End Sub

#Region "CustomColours"

        Private pnlCustomFosused As Panel
        Private Sub pnlCustom_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles pnlCustom01.MouseEnter

            Dim pnl As Panel = CType(sender, Panel)
            Try
                If pnlCustomFosused IsNot Nothing Then
                    pnlCustomFosused.BorderStyle = Windows.Forms.BorderStyle.Fixed3D
                End If
            Catch ex As Exception
            End Try
            pnlCustomFosused = pnl
            pnlCustomFosused.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
        End Sub

        Private Sub pnlCustom01_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles pnlCustom01.MouseUp
            TabPalette.Refresh()
            Application.DoEvents()
            pnlCustomSelected = sender
            If e.Button = MouseButtons.Left Then
                Me.Value = pnlCustomSelected.BackColor
            Else
                CustomColorHightLight()
            End If
        End Sub

        Private Sub CustomColorHightLight()
            Dim g As Graphics = TabPalette.CreateGraphics
            g.DrawRectangle(New Pen(Brushes.Red), pnlCustomSelected.Location.X - 1, pnlCustomSelected.Location.Y - 1, pnlCustomSelected.Width + 1, pnlCustomSelected.Height + 1)
        End Sub

        Private Sub pnlCustom_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles pnlCustom01.DoubleClick
            Me.Value = pnlCustomSelected.BackColor
            RaiseEvent ColorPicked(Me)
        End Sub

        Private Sub PreviewPanel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles PreviewPanel.Click
            If pnlCustomSelected IsNot Nothing AndAlso tabColors.SelectedTab Is TabPalette Then
                pnlCustomSelected.BackColor = PreviewPanel.BackColor
                Dim intPanelIndex As Integer = pnlCustomSelected.Name.Substring(9)
                If intPanelIndex > Palette.PaletteColours.Length Then ReDim Preserve Palette.PaletteColours(intPanelIndex)
                Palette.PaletteColours(intPanelIndex - 1) = PreviewPanel.BackColor
            End If
        End Sub

        Private Sub PreviewPanel_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles PreviewPanel.DoubleClick, btnApply.Click
            RaiseEvent ColorPicked(Me)
        End Sub
#End Region

        Private Sub tabColors_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabColors.SelectedIndexChanged
            If tabColors.SelectedTab Is TabPalette Then
                DrawCustomColours()
                If pnlCustomSelected IsNot Nothing Then
                    CustomColorHightLight()
                End If
            End If
        End Sub

    End Class

#Region "ColorPickerDesigner"

    Public Class ColorPickerDesigner
        Inherits System.Windows.Forms.Design.ControlDesigner

        Public Overrides ReadOnly Property SelectionRules() As System.Windows.Forms.Design.SelectionRules
            Get
                Return Windows.Forms.Design.SelectionRules.Visible Or Windows.Forms.Design.SelectionRules.Moveable
            End Get
        End Property


#Region "ActionLists"

        Private _Lists As DesignerActionListCollection

        Public Overrides ReadOnly Property ActionLists() As System.ComponentModel.Design.DesignerActionListCollection
            Get
                If _Lists Is Nothing Then
                    _Lists = New DesignerActionListCollection
                    _Lists.Add(New ColorPickerActionList(Me.Component))
                End If
                Return _Lists
            End Get
        End Property

#End Region 'ActionLists

    End Class

#Region "ColorPickerActionList"

    Public Class ColorPickerActionList
        Inherits DesignerActionList

        Private _ColorPickerSelector As ColorPicker
        Private _DesignerService As DesignerActionUIService = Nothing

        Public Sub New(ByVal component As IComponent)
            MyBase.New(component)

            ' Save a reference to the control we are designing.
            _ColorPickerSelector = DirectCast(component, ColorPicker)

            ' Save a reference to the DesignerActionUIService
            _DesignerService = _
                CType(GetService(GetType(DesignerActionUIService)),  _
                DesignerActionUIService)

            'Makes the Smart Tags open automatically 
            Me.AutoShow = True
        End Sub

#Region "Smart Tag Items"

#Region "Properties"

        Public Property Value() As Color
            Get
                Return _ColorPickerSelector.Value
            End Get
            Set(ByVal value As Color)
                SetControlProperty("Value", value)
            End Set
        End Property

        Public Property BackColor() As Color
            Get
                Return _ColorPickerSelector.BackColor
            End Get
            Set(ByVal value As Color)
                SetControlProperty("BackColor", value)
            End Set
        End Property

        'Public Property FlyOut() As ColorPicker.eFlyOut
        '    Get
        '        Return _ColorPickerSelector.FlyOut
        '    End Get
        '    Set(ByVal value As ColorPicker.eFlyOut)
        '        SetControlProperty("FlyOut", value)
        '    End Set
        'End Property

#End Region 'Properties

        ' Set a control property. This method makes Undo/Redo
        ' work properly and marks the form as modified in the IDE.
        Private Sub SetControlProperty(ByVal property_name As String, ByVal value As Object)
            TypeDescriptor.GetProperties(_ColorPickerSelector) _
                (property_name).SetValue(_ColorPickerSelector, value)
        End Sub

#End Region ' Smart Tag Items

        ' Return the smart tag action items.
        Public Overrides Function GetSortedActionItems() As System.ComponentModel.Design.DesignerActionItemCollection
            Dim items As New DesignerActionItemCollection()

            items.Add( _
                New DesignerActionPropertyItem( _
                    "HideRGB", _
                    "Hide RGB Panel", _
                    "Color Picker", _
                    "Show or Hide RGB Panel"))
            items.Add( _
                New DesignerActionPropertyItem( _
                    "FlyOut", _
                    "Auto Color Flyout", _
                    "Color Picker", _
                    "Show FlyOut Automatically or with a MouseClick"))
            items.Add( _
                New DesignerActionPropertyItem( _
                    "BackColor", _
                    "Background Color", _
                    "Color Picker", _
                    "The Background Color of the Control"))

            items.Add( _
                New DesignerActionHeaderItem("Value"))
            items.Add( _
                New DesignerActionPropertyItem( _
                    "Value", _
                    "Current Value", _
                    "Selected Color Value", _
                    "The color currently selected"))

            'Add Text Item 
            items.Add( _
                New DesignerActionTextItem( _
                    Space(28) & "Gonzo Diver", _
                    " "))
            Return items
        End Function

    End Class

#End Region 'DDPActionList

#End Region 'ColorPickerDesigner

#Region "EyeDropper"

    <ToolboxItem(True), ToolboxBitmap(GetType(EyeDropper), "ColorPickerLib.EyeDropper.bmp")> _
    <DefaultEvent("SelectedColorChanged")> _
    Public Class EyeDropper
        Inherits UserControl

        Public Event SelectedColorChanging(ByVal sender As Object, ByVal CurrColor As Color)
        Public Event SelectedColorChanged(ByVal sender As Object, ByVal CurrColor As Color)

        Private szDownSize As Size = New Size(23, 23)
        Private bmpScreenShot As Bitmap
        Private DropperCursor As Cursor
        Private sngZoom As Single = 5
        Private blnGettingPixelColor As Boolean = False
        Private szZoomWindowSize As Size = New Size(100, 100)
        Private bmpButtonImage As Bitmap

#Region "Initialize"

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            'InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.SetStyle(ControlStyles.DoubleBuffer, True)
            Me.SetStyle(ControlStyles.UserPaint, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)

            Me.DoubleBuffered = True
            Me.Size = szDownSize

        End Sub

        Private Sub ColorPicker_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            RecalcScreenShotSize()
            'get the blank cursor to make the it dissapear when dragging
            Dim assem As Reflection.Assembly = Me.GetType().Assembly
            Dim my_namespace As String = assem.GetName().Name.ToString()
            Dim mystream As IO.Stream
            Try
                mystream = assem.GetManifestResourceStream("Dropper.cur")
                DropperCursor = New Cursor(mystream)

            Catch ex As Exception
                DropperCursor = Cursors.Cross
            End Try
            DrawDropper(Color.Blue)
        End Sub

#End Region 'Initialize

#Region "Properties"

        Private _SelectedColor As Color
        <Category("Appearance EyeDropper")> _
        <Description("Current Color at the Cursor Location")> _
        Public Property SelectedColor() As Color
            Get
                Return _SelectedColor
            End Get
            Set(ByVal value As Color)
                If _SelectedColor <> value Then
                    _SelectedColor = value
                    RaiseEvent SelectedColorChanging(Me, _SelectedColor)
                End If
            End Set
        End Property

        Private _BorderColor As Color = Color.Blue
        <Category("Appearance EyeDropper")> _
        <Description("Color of the Border")> _
        Public Property BorderColor() As Color
            Get
                Return _BorderColor
            End Get
            Set(ByVal value As Color)
                _BorderColor = value
                Me.Invalidate()
            End Set
        End Property

        Private _ButtonColor As Color = Me.BackColor
        <Category("Appearance EyeDropper")> _
        <Description("Background Color for the Button")> _
        Public Property ButtonColor() As Color
            Get
                Return _ButtonColor
            End Get
            Set(ByVal value As Color)
                _ButtonColor = value
                Me.Invalidate()
            End Set
        End Property

#End Region 'Properties

#Region "Mouse Events"

        Private Sub EyeDropScreen_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Cursor = DropperCursor
                blnGettingPixelColor = True
                Me.Size = szZoomWindowSize
                Me.BringToFront()
                Invalidate()
            End If
        End Sub

        Private Sub EyeDropScreen_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
            If e.Button = Windows.Forms.MouseButtons.Left Then
                GetScreenShot()
            End If
        End Sub

        Private Sub EyeDropScreen_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
            Cursor = Cursors.Default
            blnGettingPixelColor = False
            Me.Size = szDownSize
            DrawDropper(_SelectedColor)

            RaiseEvent SelectedColorChanged(Me, _SelectedColor)

            Invalidate()
        End Sub

#End Region 'Mouse Events

#Region "Screen Capture"

        Sub RecalcScreenShotSize()
            If bmpScreenShot IsNot Nothing Then
                bmpScreenShot.Dispose()
            End If
            bmpScreenShot = New Bitmap(CInt(szZoomWindowSize.Width / sngZoom), CInt(szZoomWindowSize.Height / sngZoom))
        End Sub

        Sub GetScreenShot()
            Dim scrPt As Point = Control.MousePosition
            scrPt.X = CInt(scrPt.X - bmpScreenShot.Width / 2)
            scrPt.Y = CInt(scrPt.Y - bmpScreenShot.Height / 2)

            Using g As Graphics = Graphics.FromImage(bmpScreenShot)
                g.CopyFromScreen(scrPt, New Point(0, 0), bmpScreenShot.Size)
            End Using

            Me.SelectedColor = bmpScreenShot.GetPixel(CInt(bmpScreenShot.Size.Width / 2), CInt(bmpScreenShot.Size.Height / 2))

            Refresh()
        End Sub

#End Region 'Screen Capture

#Region "Painting"

        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
            MyBase.OnPaint(e)
            Dim rr As New Rectangle
            Dim crns As Integer = 0
            If bmpScreenShot IsNot Nothing Then
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor
                e.Graphics.SmoothingMode = SmoothingMode.None
                Try

                    If blnGettingPixelColor Then
                        crns = 0
                        rr.Size = szZoomWindowSize
                        Dim centerrect As Rectangle = New Rectangle(CInt(CSng((szZoomWindowSize.Width / 2) - (sngZoom / 2) + 1)), _
                            CInt(CSng((szZoomWindowSize.Height / 2) - (sngZoom / 2) + 1)), CInt(sngZoom - 1), CInt(sngZoom - 1))
                        e.Graphics.DrawImage(bmpScreenShot, 0, 0, szZoomWindowSize.Width, szZoomWindowSize.Height)
                        e.Graphics.DrawRectangle(Pens.Black, centerrect)
                    Else
                        crns = 3
                        rr.Size = szDownSize
                        e.Graphics.FillRectangle(New SolidBrush(Me.BackColor), New Rectangle(New Point(0, 0), szDownSize))
                        e.Graphics.FillPath(New SolidBrush(Me._ButtonColor), GetRectPath(rr, crns))
                        e.Graphics.DrawImage(bmpButtonImage, 3, 3)
                    End If

                Catch ex As Exception

                End Try
            End If
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            e.Graphics.DrawPath(New Pen(_BorderColor), GetRectPath(rr, crns))
        End Sub

        Private Sub DrawDropper(ByVal TipColor As Color)
            bmpButtonImage = New Bitmap(16, 16)
            Using g As Graphics = Graphics.FromImage(bmpButtonImage)
                g.SmoothingMode = SmoothingMode.AntiAlias
                'Tube Border
                g.DrawLine(Pens.Gray, 0, 15, 0, 13)
                g.DrawLine(Pens.Black, 0, 13, 7, 6)
                g.DrawLine(Pens.Gray, 0, 15, 2, 15)
                g.DrawLine(Pens.Black, 2, 15, 9, 8)

                'Tube Fill
                g.DrawLine(Pens.White, 1, 13, 8, 6)
                g.DrawLine(Pens.Silver, 1, 14, 8, 7)
                g.DrawLine(Pens.Silver, 2, 14, 9, 7)
                g.DrawLine(New Pen(TipColor), 1, 13, 4, 10)
                g.DrawLine(New Pen(TipColor), 1, 14, 5, 10)
                g.DrawLine(New Pen(TipColor), 2, 14, 6, 10)

                'Bulb Hilt
                g.DrawLine(Pens.Black, 6, 3, 12, 9)
                g.DrawLine(Pens.Black, 7, 3, 12, 8)

                'Bulb Border
                g.DrawLine(Pens.Black, 9, 3, 12, 0)
                g.DrawLine(Pens.Black, 12, 0, 14, 0)
                g.DrawLine(Pens.Black, 15, 1, 15, 3)
                g.DrawLine(Pens.Black, 12, 6, 15, 3)

                'Bulb Fill
                g.DrawLine(Pens.Gray, 9, 4, 12, 1)
                g.DrawLine(Pens.White, 10, 4, 13, 1)
                g.DrawLine(Pens.DimGray, 10, 5, 14, 1)
                g.DrawLine(Pens.DimGray, 11, 5, 14, 2)
                g.DrawLine(Pens.Black, 11, 6, 14, 3)
            End Using
        End Sub

        Public Function GetRectPath(ByVal BaseRect As RectangleF, ByVal CornerRadius As Integer) As GraphicsPath

            Dim BorderRect As Rectangle = New Rectangle(0, 0, CInt(BaseRect.Width - 2), CInt(BaseRect.Height - 2))

            Dim ArcRect As RectangleF
            Dim MyPath As New GraphicsPath()
            If CornerRadius = 0 Then
                MyPath.AddRectangle(BorderRect)
            Else
                With MyPath
                    ArcRect = New RectangleF(BorderRect.Location, _
                        New SizeF(CornerRadius * 2, CornerRadius * 2))
                    ' top left arc
                    .AddArc(ArcRect, 180, 90)

                    ' top right arc
                    ArcRect.X = BorderRect.Right - (CornerRadius * 2)
                    .AddArc(ArcRect, 270, 90)

                    ' bottom right arc
                    ArcRect.Y = BorderRect.Bottom - (CornerRadius * 2)
                    .AddArc(ArcRect, 0, 90)

                    ' bottom left arc
                    ArcRect.X = BorderRect.Left
                    .AddArc(ArcRect, 90, 90)

                    .CloseFigure()
                End With
            End If

            Return MyPath

        End Function

#End Region 'Painting 

    End Class

#End Region 'EyeDropper
End Namespace
