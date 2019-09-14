Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "GradientIco")> _
    Public Class Gradient : Inherits VGDDBase

        Private _Radius As Integer = 10
        Private _Color1 As Color = Drawing.Color.Blue
        Private _Color2 As Color = Drawing.Color.Red
        Private _Right, _Left, _Top, _Bottom As Integer
        Private _Length As Integer = 50
        Private _GradientType As VGDDScheme.GFX_GRADIENT_TYPE = VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOUBLE_HOR
        Private _ZDrawFirst As Boolean = True

        Private Loading As Boolean

        Public Sub New()
            Me.Height = 100
            Me.Width = 100
            Randomize()
            Me.Color1 = Color.FromArgb(Rnd(1) * 255, Rnd(1) * 255, Rnd(1) * 255)
            Me.Color2 = Color.FromArgb(Rnd(1) * 255, Rnd(1) * 255, Rnd(1) * 255)
            Loading = True
            MyBase.RemovePropertyToShow("Text Font CDeclType Public")
#If Not PlayerMonolitico Then
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLA", "HARMONY"
                    'MyBase.RemovePropertyToShow("GradientType")
                    MyBase.RemovePropertyToShow("GradientLength")
            End Select
#End If
            Common.ProjectUseGradient = True
        End Sub

        Private Sub RecalcCoords()
            _Left = MyBase.Location.X + _Radius
            _Right = MyBase.Location.X + Me.Width - _Radius - 1
            _Top = MyBase.Location.Y + _Radius
            _Bottom = MyBase.Location.Y + Me.Height - _Radius - 1
            If _Bottom - _Top <= 2 Then
                Me.Height = 3
            End If
        End Sub

        Public Overloads Property Height() As Integer
            Get
                Return MyBase.Height
            End Get
            Set(ByVal value As Integer)
                If value >= 4 Then
                    MyBase.Height = value
                    Me.Invalidate()
                End If
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
            Me.OnPaintBackground(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

            RecalcCoords()

            'Impostazione Region
            Dim p1 As GraphicsPath = Common.RoundRectanglePath(_Radius, _Radius, Me.Width, Me.Height, _Radius)
            Dim p2 As GraphicsPath = Nothing
            Dim r As New Region(p1)
            Me.Region = r

            Select Case _GradientType
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_NONE
                    Dim brushBackGround As New SolidBrush(_Color1)
                    g.FillRegion(brushBackGround, Me.Region)
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOUBLE_VER
                    Dim brushBackGround As New LinearGradientBrush(rc, _Color1, _Color2, 0)
                    Dim bl As New Blend(3)
                    bl.Positions(0) = 0.0F
                    bl.Positions(1) = 0.5F
                    bl.Positions(2) = 1.0F
                    bl.Factors(0) = 0.0F
                    bl.Factors(1) = 1.0F
                    bl.Factors(2) = 0.0F
                    brushBackGround.Blend = bl
                    g.FillRegion(brushBackGround, Me.Region)
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOUBLE_HOR
                    Dim brushBackGround As New LinearGradientBrush(rc, _Color1, _Color2, 90)
                    Dim bl As New Blend(3)
                    bl.Positions(0) = 0.0F
                    bl.Positions(1) = 0.5F
                    bl.Positions(2) = 1.0F
                    bl.Factors(0) = 0.0F
                    bl.Factors(1) = 1.0F
                    bl.Factors(2) = 0.0F
                    brushBackGround.Blend = bl
                    g.FillRegion(brushBackGround, Me.Region)
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOWN
                    Dim brushBackGround As New LinearGradientBrush(rc, _Color1, _Color2, 90)
                    g.FillRegion(brushBackGround, Me.Region)
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_LEFT
                    Dim brushBackGround As New LinearGradientBrush(rc, _Color1, _Color2, 180)
                    g.FillRegion(brushBackGround, Me.Region)
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_RIGHT
                    Dim brushBackGround As New LinearGradientBrush(rc, _Color1, _Color2, 0)
                    g.FillRegion(brushBackGround, Me.Region)
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_UP
                    Dim brushBackGround As New LinearGradientBrush(rc, _Color1, _Color2, 270)
                    g.FillRegion(brushBackGround, Me.Region)
            End Select

        End Sub


#Region "GDDProps"

#If Not PlayerMonolitico Then
        <CustomSortedCategory("Appearance", 4)> _
        <Editor(GetType(GradientTypePropertyEditor), GetType(System.Drawing.Design.UITypeEditor))> _
        <Description("Gradient type")> _
        <DefaultValue(GetType(VGDDScheme.GFX_GRADIENT_TYPE), "GRAD_NONE")> _
        Public Property GradientType() As VGDDScheme.GFX_GRADIENT_TYPE
#Else
        Public Property GradientType() As VGDDScheme.GFX_GRADIENT_TYPE
#End If
            Get
                Return _GradientType
            End Get
            Set(ByVal value As VGDDScheme.GFX_GRADIENT_TYPE)
                _GradientType = value
                If _GradientType = VGDDScheme.GFX_GRADIENT_TYPE.GRAD_NONE Then
                    MyBase.RemovePropertyToShow("GradientEndColor")
                    MyBase.RemovePropertyToShow("GradientLength")
                Else
                    MyBase.AddPropertyToShow("GradientEndColor")
                    MyBase.AddPropertyToShow("GradientLength")
                    Common.ProjectUseGradient = True
                End If
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <CustomSortedCategory("Appearance", 4)> _
        <Description("Start Colour for the Gradient")> _
        <Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Overloads Property Color1() As Color
#Else
        Public Overloads Property Color1() As Color
#End If
            Get
                Return _Color1
            End Get
            Set(ByVal value As Color)
                _Color1 = value
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <CustomSortedCategory("Appearance", 4)> _
        <Description("End Colour for the Gradient")> _
        <Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Overloads Property Color2() As Color
#Else
        Public Overloads Property Color2() As Color
#End If
            Get
                Return _Color2
            End Get
            Set(ByVal value As Color)
                _Color2 = value
                Me.Invalidate()
            End Set
        End Property

        <CustomSortedCategory("Appearance", 4)> _
        <Description("Length of the gradient transition in pixels (From 0-100%. How much of a gradient is wanted)")> _
        <DefaultValue(50)> _
        Public Property GradientLength() As Integer
            Get
                Return _Length
            End Get
            Set(ByVal value As Integer)
                _Length = value
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
        <CustomSortedCategory("Size and Position", 3)> _
         <Browsable(True)> _
        Public Shadows Property Right As Integer
            Get
                Return _Right 'MyBase.Location.X + Me.Width + _Radius2
            End Get
            Set(ByVal value As Integer)
                _Right = value
                If Not Loading Then
                    MyBase.Width = Right - _Radius * 2
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <CustomSortedCategory("Size and Position", 3)> _
        <Description("Left X coordinate of the upper-left edge")> _
        <Browsable(True)> _
        Public Shadows Property Left As Integer
            Get
                Return _Left 'MyBase.Location.X + _Radius2
            End Get
            Set(ByVal value As Integer)
                _Left = value
                If Not Loading Then
                    MyBase.Location = New Point(_Left - _Radius, MyBase.Location.Y)
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <CustomSortedCategory("Size and Position", 3)> _
        <Description("Top Y coordinate of the upper-left edge")> _
        <Browsable(True)> _
        Public Shadows Property Top As Integer
            Get
                Return _Top 'MyBase.Location.Y + _Radius2
            End Get
            Set(ByVal value As Integer)
                _Top = value
                If Not Loading Then
                    MyBase.Location = New Point(MyBase.Location.X, _Top - _Radius)
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <CustomSortedCategory("Size and Position", 3)> _
        <Description("Bottom Y coordinate of the lower-right edge")> _
        <Browsable(True)> _
        Public Shadows Property Bottom As Integer
            Get
                Return _Bottom ' MyBase.Location.Y + Me.Height - _Radius2
            End Get
            Set(ByVal value As Integer)
                _Bottom = value
                If Not Loading Then
                    Me.Height = _Bottom - _Radius * 2
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <CustomSortedCategory("Appearance", 4)> _
        <Description("Defines the radius of the circle, that draws the rounded corners. When rad = 0, the object drawn is a rectangular gradient")> _
        <DefaultValue(10)> _
        Property Radius As Integer
            Get
                Return _Radius
            End Get
            Set(ByVal value As Integer)
                _Radius = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Draw this primitive before (set to true) or after (set to false) the Widgets?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("Design")> _
        <DefaultValue(True)> _
        Property ZDrawFirst() As Boolean
            Get
                Return _ZDrawFirst
            End Get
            Set(ByVal value As Boolean)
                _ZDrawFirst = value
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
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
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            Dim FillStyle As VGDDScheme.GFX_FILL_STYLE
            Select Case _GradientType
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_NONE
                    FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_NONE
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_LEFT
                    FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_LEFT
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_RIGHT
                    FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_RIGHT
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOWN
                    FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOWN
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_UP
                    FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_UP
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOUBLE_HOR
                    FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_HOR
                Case VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOUBLE_VER
                    FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_VER
            End Select


            MyCode = MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", _Left).Replace("[TOP]", _Top).Replace("[RIGHT]", _Right).Replace("[BOTTOM]", _Bottom) _
                .Replace("[RADIUS]", _Radius) _
                .Replace("[COLOR1]", CodeGen.Color2Num(_Color1, False, "Gradient " & Me.Name & ".Color1")) _
                .Replace("[COLOR2]", CodeGen.Color2Num(_Color2, False, "Gradient " & Me.Name & ".Color2")) _
                .Replace("[LENGTH]", _Length) _
                .Replace("[DIRECTION]", _GradientType) _
                .Replace("[FILLSTYLE]", FillStyle.ToString) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, CodeGen.HeadersTemplate.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            If _ZDrawFirst Then
                CodeGen.Code &= MyCode
            Else
                CodeGen.ScreenUpdateCode &= MyCode
            End If

        End Sub
#End If

        Private Sub Arc_ClientSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ClientSizeChanged
            RecalcCoords()
        End Sub

        Private Sub Arc_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles Me.Layout
            Loading = False
        End Sub

        Private Sub Arc_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
            RecalcCoords()
        End Sub

        Private Sub Arc_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
            RecalcCoords()
        End Sub

        Private Sub Arc_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            RecalcCoords()
        End Sub

        Private Sub Gradient_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DoubleClick
            Me.Size = New Size(Me.Parent.Width - Me.Radius * 2, Me.Parent.Height - Me.Radius * 2)
        End Sub
    End Class


End Namespace