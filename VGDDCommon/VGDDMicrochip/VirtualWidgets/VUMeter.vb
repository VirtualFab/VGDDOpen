Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports VGDDCommon
Imports VGDDCommon.Common
Imports System.IO
Imports VGDDMicrochip.VGDDBase

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(VuMeter), "VuMeter.ico")> _
    Public Class VuMeter : Inherits Control
        Implements ICustomTypeDescriptor
        Implements IVGDDWidget
        Implements ISupportInitialize

        Private requiresRedraw As Boolean
        Private currentValue As Int16 = 0
        Private previousValue As Int16
        Private newValue As Int16
        Private _Value As Integer
        Private _MinValue As Int16
        Private _MaxValue As Int16 = 100
        Private _AngleFrom As Int16 = 225
        Private _AngleTo As Int16 = 315
        Private _Frame As EnabledState = False
        Private _Animated As Boolean = True

        Private _BitmapName As String
        Private Shared _DefaultBitmapName As String = String.Empty
        Protected _BitmapUsePointer As Boolean = False
        Protected _VGDDImage As VGDDImage
        Protected _IsLoading As Boolean = False
        Private _PointerType As VUPointerTypes = VUPointerTypes.VU_POINTER_3D
        Private _PointerWidth As Int16 = 5
        Private _PointerCenterOffsetX As Int16 = 50
        Private _PointerCenterOffsetY As Int16 = -15
        Private _PointerLength As Int16 = 132
        Private _PointerStart As Int16 = 36
        Private _PointerLine As Common.ThickNess = Common.ThickNess.THICK_LINE
        Private _PointerSpeed As Byte = 1
        Private _PointerSpeedDecay As Byte = 1

        Private _Scheme As VGDDScheme
        Private _Hidden As Boolean = False
        Private _Events As VGDDEventsCollection
        Private _State As EnabledState = EnabledState.Enabled
        Private _CDeclType As TextCDeclType = TextCDeclType.ConstXcharArray
        Protected _MyPropsToRemove As String = ""

        Private WithEvents AniTimer As New Timer

        Private Shared _Instances As Integer = 0

        Public Enum VUPointerTypes
            VU_POINTER_NORMAL = 0
            VU_POINTER_3D
            VU_POINTER_WIREFRAME
            VU_POINTER_NEEDLE
        End Enum

        Public Sub New()
            MyBase.New()
            _Instances += 1
            InitializeComponent()
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("VUMeter")
#End If
            Me.Size = New Size(240, 150)
            Me.DoubleBuffered = True
            RemovePropertyToShow("Text")
            RemovePropertyToShow("BackColor")
            RemovePropertyToShow("Font")
            requiresRedraw = True
            If _DefaultBitmapName = String.Empty OrElse Common.GetBitmap(_DefaultBitmapName) Is Nothing Then
                _DefaultBitmapName = Common.ExtractDefaultBitmap("VuMeterDefault.png", Me.GetType.Assembly)
            Else
                Me.Size = Common.GetBitmap(_DefaultBitmapName).Size
            End If
            Common.SetBitmapName(_DefaultBitmapName, Me, _BitmapName, _VGDDImage, requiresRedraw)
        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                    If _Instances = 0 Then _DefaultBitmapName = String.Empty
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
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

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public ReadOnly Property Demolimit As Integer Implements IVGDDWidget.DemoLimit
            Get
                Return Common.DEMOCODELIMIT
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Shadows Property Text As String Implements IVGDDWidget.Text
            Get
                Return String.Empty
            End Get
            Set(ByVal value As String)

            End Set
        End Property

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

        Const BORDER As Integer = 2

        Private RectImgWidth As Int16, RectImgHeight As Int16
        Private Xcenter As Int16, Ycenter As Int16
        Private xo1, xo2, yo1, yo2 As Int16
        Private Sub CalcPar()
            RectImgWidth = Right - Left - (BORDER << 1)
            RectImgHeight = Bottom - Top - (BORDER << 1)
            Xcenter = RectImgWidth * _PointerCenterOffsetX \ 100 + BORDER
            Ycenter = RectImgHeight - RectImgHeight * _PointerCenterOffsetY \ 100 + BORDER
        End Sub

        Protected Overrides Sub OnPaintBackground(ByVal pevent As System.Windows.Forms.PaintEventArgs)
            'MyBase.OnPaintBackground(pevent)
        End Sub

        Private Sub CheckLargestArea(ByVal x, ByVal y)
            If xo1 > x Then xo1 = x - 1
            If xo2 < x Then xo2 = x + 1
            If yo1 > y Then yo1 = y - 1
            If yo2 < y Then yo2 = y + 1
        End Sub

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Dim Xn1 As Int16, Xn2 As Int16, Xn3 As Int16
            Dim Yn1 As Int16, Yn2 As Int16, Yn3 As Int16
            Static currentImage As Bitmap

            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If

            If _Scheme Is Nothing Or Me.Hidden Then Exit Sub

            'CalcPar() 'togliere

            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias

            Dim g As Graphics
            If _VGDDImage IsNot Nothing AndAlso _VGDDImage.Bitmap IsNot Nothing Then
                If (DesignMode OrElse requiresRedraw) Then
                    'Draw full Bitmap
                    'requiresRedraw = False
                    If currentImage IsNot Nothing Then
                        currentImage.Dispose()
                    End If
                    currentImage = _VGDDImage.Bitmap.Clone
                    'g = Graphics.FromImage(currentImage)
                    g = pevent.Graphics
                    g.SmoothingMode = SmoothingMode.AntiAlias
                    g.DrawImage(_VGDDImage.Bitmap, 0, 0)
                    xo1 = Right + 1
                    xo2 = -1
                    yo1 = Bottom + 1
                    yo2 = -1
                Else
                    'g = Graphics.FromImage(currentImage)
                    g = pevent.Graphics
                    g.SmoothingMode = SmoothingMode.AntiAlias
                    g.DrawImage(_VGDDImage.Bitmap, xo1, yo1, New System.Drawing.Rectangle(xo1, yo1, xo2, yo2), GraphicsUnit.Pixel)
                End If
            Else
                g = pevent.Graphics
                g.FillRectangle(Brushes.Black, Me.ClientRectangle)
            End If

            Dim ps As Pen = New Pen(_Scheme.Color1)
            If _Frame = EnabledState.Enabled Then
                ps.Width = 1
                ps.DashStyle = DashStyle.Solid
                g.DrawRectangle(ps, ps.Width >> 1, ps.Width >> 1, Me.ClientRectangle.Width - ps.Width - 1, Me.ClientRectangle.Height - ps.Width - 1)
            End If

            'Draw Pointer
            Dim degAngle As Int16 = (Me.currentValue - MinValue) * 100 / (MaxValue - MinValue)
            degAngle = _AngleFrom + (CType((_AngleTo - _AngleFrom), Int32)) * degAngle / 100

            Try
                Common.GOL.GetCirclePoint(_PointerLength, degAngle, Xn1, Yn1)
            Catch ex As Exception
                Exit Sub
            End Try
            Xn1 += Xcenter
            Yn1 += Ycenter
            CheckLargestArea(Xn1, Yn1)
            Dim Color0 As Color = IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Embossdkcolor, _Scheme.Textcolordisabled)
            Dim Color1 As Color = IIf(Me.State = Common.EnabledState.Enabled, _Scheme.Embossltcolor, _Scheme.Textcolordisabled)
            Dim PointerThickness As Byte = IIf(_PointerLine = Common.ThickNess.NORMAL_LINE, 1, 3)
            Dim DrawStep As Byte = 1 ' IIf(_PointerLine = Common.ThickNess.NORMAL_LINE, 1, 2)
            ps.Width = PointerThickness
            Select Case _PointerType
                Case PointerTypes.Normal, PointerTypes.Filled3D
                    For i As Byte = 0 To _PointerWidth >> 1 Step DrawStep
                        Common.GOL.GetCirclePoint(_PointerStart, degAngle - i, Xn2, Yn2)
                        Common.GOL.GetCirclePoint(_PointerStart, degAngle + i, Xn3, Yn3)
                        Xn2 += Xcenter
                        Yn2 += Ycenter
                        Xn3 += Xcenter
                        Yn3 += Ycenter
                        CheckLargestArea(Xn2, Yn2)
                        CheckLargestArea(Xn3, Yn3)

                        ps.Color = Color0
                        g.DrawLine(ps, Xn1, Yn1, Xn2, Yn2)
                        If _PointerType = PointerTypes.Filled3D Then
                            ps.Color = Color1
                        End If
                        g.DrawLine(ps, Xn1, Yn1, Xn3, Yn3)
                    Next

                Case PointerTypes.WireFrame
                    ps.Color = Color0
                    Common.GOL.GetCirclePoint(_PointerStart, degAngle - (_PointerWidth >> 1), Xn2, Yn2)
                    Common.GOL.GetCirclePoint(_PointerStart, degAngle + (_PointerWidth >> 1), Xn3, Yn3)
                    Xn2 += Xcenter
                    Yn2 += Ycenter
                    Xn3 += Xcenter
                    Yn3 += Ycenter
                    CheckLargestArea(Xn2, Yn2)
                    CheckLargestArea(Xn3, Yn3)
                    g.DrawLine(ps, Xn1, Yn1, Xn2, Yn2)
                    If _PointerType = PointerTypes.Filled3D Then
                        ps.Color = Color1
                    End If
                    g.DrawLine(ps, Xn2, Yn2, Xn3, Yn3)
                    g.DrawLine(ps, Xn3, Yn3, Xn1, Yn1)

                Case PointerTypes.Needle
                    Common.GOL.GetCirclePoint(_PointerStart, degAngle, Xn2, Yn2)
                    ps.Color = Color0
                    Xn2 += Xcenter
                    Yn2 += Ycenter
                    CheckLargestArea(Xn2, Yn2)
                    g.DrawLine(ps, Xn1, Yn1, Xn2, Yn2)
            End Select
            'If _VGDDImage IsNot Nothing AndAlso _VGDDImage.Bitmap IsNot Nothing Then
            '    pevent.Graphics.DrawImage(currentImage, 0, 0)
            'End If
        End Sub

        Private Sub AnimateTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles AniTimer.Tick
            Static AnimationIncrement As Integer = 1
            If Common.AnimationEnable Then
                AniTimer.Enabled = False
                If (AnimationIncrement > 0 And currentValue >= newValue) Or _
                    (AnimationIncrement < 0 And currentValue <= newValue) Then
                    newValue = _MinValue + Rnd(1) * (_MaxValue - _MinValue)
                    AnimationIncrement = (newValue * (Rnd(1) + 0.01)) Mod 5 + 1
                    If newValue < currentValue Then AnimationIncrement = AnimationIncrement * -1
                Else
                    currentValue += AnimationIncrement
                    If currentValue > _MaxValue Then currentValue = _MaxValue
                    If currentValue < _MinValue Then currentValue = _MinValue
                End If
                Me.Invalidate()
                AniTimer.Enabled = True
            End If
        End Sub

#Region "IVGDD Stubs"
        Public ReadOnly Property HasChildWidgets As Boolean Implements IVGDDWidget.HasChildWidgets
            Get
                Return False
            End Get
        End Property
#End Region

#Region "VuMeterProps"

        <Description("Type of Pointer")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(VUPointerTypes.VU_POINTER_3D)> _
        <Category("VUMeter")> _
        Public Property PointerType As VUPointerTypes
            Get
                Return _PointerType
            End Get
            Set(ByVal value As VUPointerTypes)
                _PointerType = value
                If _PointerType = VUPointerTypes.VU_POINTER_NEEDLE Then
                    RemovePropertyToShow("PointerWidth")
                Else
                    AddPropertyToShow("PointerWidth")
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Width of the needle (pixels)")> _
        <DefaultValue(10)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(1, 255)> _
        <Category("VUMeter")> _
        Public Property PointerWidth As Integer
            Get
                Return _PointerWidth
            End Get
            Set(ByVal value As Integer)
                If value > 0 Then
                    _PointerWidth = value
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Pointer center X offset point (% of height). Use Negative values to put it outside the left edge.")> _
        <DefaultValue(50)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(-200, 100)> _
        <Category("VUMeter")> _
        Public Property PointerCenterOffsetX As Integer
            Get
                Return _PointerCenterOffsetX
            End Get
            Set(ByVal value As Integer)
                If value >= -200 And value <= 100 Then
                    _PointerCenterOffsetX = value
                    CalcPar()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Pointer center Y offset point (% of height). Use Negative values to put it below bottom edge.")> _
        <DefaultValue(-15)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(-200, 100)> _
        <Category("VUMeter")> _
        Public Property PointerCenterOffsetY As Integer
            Get
                Return _PointerCenterOffsetY
            End Get
            Set(ByVal value As Integer)
                If value >= -200 And value <= 100 Then
                    _PointerCenterOffsetY = value
                    CalcPar()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Pointer movement speed 0:realtime >8:slow like analog")> _
        <DefaultValue(1)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 128)> _
        <Category("VUMeter")> _
        Public Property PointerSpeed As Integer
            Get
                Return _PointerSpeed
            End Get
            Set(ByVal value As Integer)
                If value >= 0 And value <= 128 Then
                    _PointerSpeed = value
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Pointer movement speed in decay 0:realtime >8:slow like analog")> _
        <DefaultValue(1)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 128)> _
        <Category("VUMeter")> _
        Public Property PointerSpeedDecay As Integer
            Get
                Return _PointerSpeedDecay
            End Get
            Set(ByVal value As Integer)
                If value >= 0 And value <= 128 Then
                    _PointerSpeedDecay = value
                    Me.Invalidate()
                End If
            End Set
        End Property
        <Description("Pointer length (pixels)")> _
        <DefaultValue(132)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 1024)> _
        <Category("VUMeter")> _
        Public Property PointerLength As Integer
            Get
                Return _PointerLength
            End Get
            Set(ByVal value As Integer)
                _PointerLength = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Pointer start from center (pixels)")> _
        <DefaultValue(36)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 1024)> _
        <Category("VUMeter")> _
        Public Property PointerStart As Integer
            Get
                Return _PointerStart
            End Get
            Set(ByVal value As Integer)
                _PointerStart = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Type of Pointer Line")> _
        <DefaultValue(Common.ThickNess.THICK_LINE)> _
        <Category("VUMeter")> _
        Public Property PointerLine As Common.ThickNess
            Get
                Return _PointerLine
            End Get
            Set(ByVal value As Common.ThickNess)
                _PointerLine = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Angle to start from")> _
        <DefaultValue(225)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 360)> _
        <Category("VUMeter")> _
        Public Property AngleFrom As Integer
            Get
                Return _AngleFrom
            End Get
            Set(ByVal value As Integer)
                _AngleFrom = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Angle to end to")> _
        <DefaultValue(315)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 360)> _
        <Category("VUMeter")> _
        Public Property AngleTo As Integer
            Get
                Return _AngleTo
            End Get
            Set(ByVal value As Integer)
                _AngleTo = value
                Me.Invalidate()
            End Set
        End Property

        <DefaultValue(0)> _
        <Description("Initial value for the VUMeter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 1024, "MaxValue")> _
        <Category("VUMeter")> _
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
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(0)> _
        <Description("Mininum value on the scale")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("VUMeter")> _
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
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(100)> _
        <Description("Maximum value on the scale")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("VUMeter")> _
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
                    Me.Invalidate()
                End If
            End Set
        End Property
#End Region

#Region "GDDProps"

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
                        AniTimer.Interval = 50
                        AniTimer.Start()
                    Else
                        AniTimer.Stop()
                    End If
                End If
            End Set
        End Property

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
        Public Overridable Property Bitmap() As String
            Get
                Return _BitmapName
            End Get
            Set(ByVal value As String)
                _BitmapName = value
                If value = String.Empty Then
                    If _VGDDImage IsNot Nothing Then
                        _VGDDImage.RemoveUsedBy(Me)
                    End If
                    _VGDDImage = Nothing
                ElseIf Not _IsLoading Then
                    'SetBitmapName(value)
                    Common.SetBitmapName(value, Me, _BitmapName, _VGDDImage, requiresRedraw)
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Widget Type")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Info", 1)> _
        Public ReadOnly Property WidgetType() As String
            Get
                Return Me.GetType.ToString.Split(".")(1)
            End Get
        End Property

        <Description("Sets the z-order of this widget (0=behind all others).")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 256)> _
        <Category("Design")> _
        Property Zorder() As Integer Implements IVGDDWidget.Zorder
            Get
                Return MyBase.TabIndex
            End Get
            Set(ByVal value As Integer)
                MyBase.TabIndex = value
                If Me.Parent IsNot Nothing Then
                    Try
                        Me.Parent.Controls.SetChildIndex(Me, Me.Parent.Controls.Count - value)
                    Catch ex As Exception
                    End Try
                End If
                MyBase.UpdateZOrder()
            End Set
        End Property

        <Description("Event handling for this VUMeter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDEventsEditorNew), GetType(System.Drawing.Design.UITypeEditor))> _
        <ParenthesizePropertyName(True)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Property VGDDEvents() As VGDDEventsCollection Implements IVGDDWidget.VGDDEvents
            Get
                Return _Events
            End Get
            Set(ByVal value As VGDDEventsCollection)
                _Events = value
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

        <Description("VUMeter Widgets are declared PUBLIC by default to handle animation in DrawCallback")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Overloads ReadOnly Property [Public]() As Boolean
            Get
                Return True
            End Get
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

        <Description("Color Scheme for the VUMeter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <TypeConverter(GetType(Common.SchemesOptionConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Scheme As String Implements IVGDDWidget.Scheme
            Get
                If _Scheme IsNot Nothing Then
                    Return _Scheme.Name
                Else
                    Return String.Empty
                End If
            End Get
            Set(ByVal value As String)
                If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing Then
                    _Scheme.Font.RemoveUsedBy(Me)
                End If
                Dim SetScheme As VGDDScheme = GetScheme(value)
                If SetScheme IsNot Nothing Then
                    _Scheme = SetScheme
                    MyBase.Font = _Scheme.Font.Font
                    _Scheme.AddUsedBy(Me)
                    _Scheme.Font.AddUsedBy(Me)
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 4096)> _
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

        <Description("Wether to enable a frame around the VUMeter or not")> _
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

        <Description("Status of the VUMeter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(EnabledState), "Enabled")> _
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

        <Description("Visibility of the VUMeter")> _
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

#Region "ISupportInitialize"
        Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
            _IsLoading = True
        End Sub

        Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
            _IsLoading = False
            If _BitmapName IsNot Nothing Then
                'SetBitmapName(_BitmapName)
                Common.SetBitmapName(_BitmapName, Me, _BitmapName, _VGDDImage, requiresRedraw)
            End If
        End Sub
#End Region

#Region "ICustomTypeDescriptor Members"

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not ((PROPSTOREMOVE & _MyPropsToRemove)).Contains(" " & pd.Name & " ") Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

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

        Public Sub GetCode(ByVal ControlIdPrefix As String) Implements IVGDDWidget.GetCode
#If Not PlayerMonolitico Then
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String
            Dim MyCode As String = "", MyState As String = ""

            Dim MyClassName As String = Me.GetType.ToString
            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            MyCodeHead = CodeGen.ConstructorTemplate.Trim & vbCrLf

            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)
            CodeGen.AddState(MyState, "PointerLine", Me.PointerLine.ToString)

            'For Each oScreenEventNode As Xml.XmlNode In CodeGen.XmlTemplatesDoc. _
            '    SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/ScreenCode", Me.WidgetType))
            '    Dim oAttribute As Xml.XmlAttribute = oScreenEventNode.Attributes("Event")
            '    If oAttribute IsNot Nothing Then
            '        Dim oScreen As VGDDScreen = Me.Parent
            '        Dim strEventName As String = oAttribute.Value
            '        Dim oScreenEvent As VGDDEvent = oScreen.VGDDEvents(strEventName)
            '        If oScreenEvent Is Nothing Then
            '            oScreenEvent = New VGDDEvent
            '            oScreenEvent.Name = strEventName
            '        End If
            '        Dim strEventCode As String = oScreenEventNode.InnerText.Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
            '                        .Replace("[CONTROLID_INDEX]", MyControlIdIndex)
            '        If Not oScreenEvent.Code.Contains(strEventCode) Then
            '            oScreenEvent.Code &= vbCrLf & strEventCode
            '            oScreenEvent.Handled = True
            '            If oScreen.VGDDEvents(strEventName) Is Nothing Then
            '                oScreen.VGDDEvents.Add(oScreenEvent)
            '            End If
            '        End If
            '    End If
            'Next

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            Dim strSegmentsArray As String = ""
            If strSegmentsArray <> "" Then strSegmentsArray = strSegmentsArray.Substring(1)

            Dim MyBitmap As String = "", MyPointerInit As String = ""
            If Me.Bitmap = String.Empty Then
                MyBitmap = "NULL"
            Else
                Dim myBitmapName As String = System.IO.Path.GetFileNameWithoutExtension(Me.Bitmap)
                If Me.BitmapUsePointer Then
                    Dim strPointerName As String = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerName", 0).Trim _
                                                   .Replace("[BITMAPNAME]", myBitmapName)
                    MyBitmap = "(void *)" & strPointerName
                    MyPointerInit = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerInit", 0).Trim _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName) _
                        .Replace("[BITMAPNAME]", myBitmapName)
                    CodeGen.AddLines(CodeGen.Headers, CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/Bitmaps", 0) _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[BITMAPNAME]", myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName))
                Else
                    MyBitmap = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName
                End If
            End If

            Dim MyParameters() As Byte = {0, _
                                        _Value \ 256, _
                                        _Value Mod 256, _
                                        CodeGen.Int2ByteH(_MinValue), _
                                        CodeGen.Int2ByteL(_MinValue), _
                                        CodeGen.Int2ByteH(_MaxValue), _
                                        CodeGen.Int2ByteL(_MaxValue), _
                                        _PointerType, _
                                        _AngleFrom \ 256, _
                                        _AngleFrom Mod 256, _
                                        _AngleTo \ 256, _
                                        _AngleTo Mod 256, _
                                        CodeGen.Int2ByteH(_PointerCenterOffsetX), _
                                        CodeGen.Int2ByteL(_PointerCenterOffsetX), _
                                        CodeGen.Int2ByteH(_PointerCenterOffsetY), _
                                        CodeGen.Int2ByteL(_PointerCenterOffsetY), _
                                        _PointerLength \ 256, _
                                        _PointerLength Mod 256, _
                                        _PointerWidth, _
                                        _PointerSpeed, _
                                        _PointerSpeedDecay, _
                                        _PointerStart \ 256, _
                                        _PointerStart Mod 256, _
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
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[PARAMETERS]", strMyParameters) _
                .Replace("[BITMAP]", MyBitmap) _
                .Replace("[BITMAP_POINTER_INIT]", MyPointerInit) _
                .Replace("[SCHEME]", _Scheme.Name) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                )
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.HeadersIncludes.Contains(CodeGen.HeadersIncludesTemplate) Then ' #include "vumeter.h"
                CodeGen.AddLines(CodeGen.HeadersIncludes, CodeGen.HeadersIncludesTemplate)
            End If
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If


            CodeGen.AddLines(CodeGen.Headers, (IIf(CodeGen.Headers.Contains(CodeGen.CodeHeadTemplate), "", CodeGen.CodeHeadTemplate & vbCrLf) & _
                "extern " & CodeGen.ConstructorTemplate.Trim & vbCrLf & _
                CodeGen.TextDeclareHeaderTemplate(_CDeclType) & CodeGen.HeadersTemplate) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, CType(Me, IVGDDWidget))

            Try
                'Dim myAssembly As Reflection.Assembly = System.Reflection.Assembly.GetAssembly(Me.GetType)
                For Each oFolderNode As Xml.XmlNode In CodeGen.XmlTemplatesDoc.SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Project/*", MyClassName.Split(".")(1)))
                    MplabX.AddFile(oFolderNode)
                Next
            Catch ex As Exception
            End Try
#End If

        End Sub
#End Region

        Private Sub VUMeter_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            'SetBitmapName(_BitmapName)
            Common.SetBitmapName(_BitmapName, Me, _BitmapName, _VGDDImage, requiresRedraw)
            CalcPar()
        End Sub
    End Class

End Namespace

