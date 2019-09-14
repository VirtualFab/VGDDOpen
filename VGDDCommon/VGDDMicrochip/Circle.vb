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
    <ToolboxBitmap(GetType(Button), "CircleIco")> _
    Public Class Circle : Inherits VGDDBase

        Private _Radius As Integer = 10
        Private _Color As Color = Drawing.Color.Blue
        Private _Thickness As Short
        Private _Fill As Boolean
        Private _Type As LineType
        Private _X As Integer
        Private _Y As Integer
        Private _ZDrawFirst As Boolean = True

        Public Sub New()
            Me.Thickness = Thickness.NORMAL_LINE
            Me.Size = New Size(100, 100)
            Me.RemovePropertyToShow("Text")
            Me.RemovePropertyToShow("TextStringID")
        End Sub

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
            Dim intPxThickness As Integer = (_Thickness + 1) '* 2 + 1

            Dim brushPen As New SolidBrush(_Color)
            Dim ps As Pen = New Pen(brushPen)
            Select Case _Type
                Case LineType.SOLID_LINE
                    ps.DashStyle = DashStyle.Solid
                Case LineType.DOTTED_LINE
                    ps.DashStyle = DashStyle.Dot
                Case LineType.DASHED_LINE
                    ps.DashStyle = DashStyle.Dash
            End Select
            ps.Width = intPxThickness + 2

            Dim Mypath As GraphicsPath = New GraphicsPath
            Mypath.StartFigure()
            'Mypath.AddRectangle(rc)
            Mypath.AddEllipse(rc.Left, rc.Top, rc.Width, rc.Height)
            If Not _Fill Then
                Mypath.AddEllipse(rc.Left + intPxThickness, rc.Top + intPxThickness, rc.Width - intPxThickness * 2, rc.Height - intPxThickness * 2)
            End If
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)

            'Mypath = New GraphicsPath
            'Mypath.StartFigure()
            'Mypath.CloseFigure()
            'Me.Region.Exclude(Mypath)

            'Dim brushBackGround As New SolidBrush(_Scheme.Color0)
            'g.FillRegion(brushBackGround, Me.Region)
            If _Fill Then
                g.FillEllipse(brushPen, rc.Left, rc.Top, rc.Width, rc.Height)
            Else
                g.DrawArc(ps, rc.Left, rc.Top, rc.Width, rc.Height, 0, 360)
            End If
        End Sub

#Region "GDDProps"

        <Description("Thickness of line for the circle")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Thickness() As ThickNess
            Get
                Return _Thickness
            End Get
            Set(ByVal value As ThickNess)
                _Thickness = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Type of line for the circle")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property [Type]() As LineType
            Get
                Return _Type
            End Get
            Set(ByVal value As LineType)
                _Type = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Should the circle be filled")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Fill() As Boolean
            Get
                Return _Fill
            End Get
            Set(ByVal value As Boolean)
                _Fill = value
                If _Fill Then
                    MyBase.RemovePropertyToShow("Type")
                    MyBase.RemovePropertyToShow("Thickness")
                Else
                    MyBase.AddPropertyToShow("Type")
                    MyBase.AddPropertyToShow("Thickness")
                End If
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Color for the circle")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property [Color]() As Color
#Else
        Public  Property [Color]() As Color
#End If
            Get
                Return _Color
            End Get
            Set(ByVal value As Color)
                _Color = value
                Me.Invalidate()
            End Set
        End Property

        <Description("X coordinate of the centre of circle")> _
         <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property X() As Integer
            Get
                Return _X 'MyBase.Location.X + MyBase.Width - _Radius
            End Get
            Set(ByVal value As Integer)
                _X = value
                If Not Me.IsLoading Then
                    MyBase.Left = _X - _Radius
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Y coordinate of the centre of circle")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property Y() As Integer
            Get
                Return _Y 'MyBase.Location.Y + _Radius
            End Get
            Set(ByVal value As Integer)
                'Me.Location = New Point(Me.Location.X, value)
                _Y = value
                If Not Me.IsLoading Then
                    MyBase.Top = _Y - _Radius
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Define the radius of circle")> _
        <CustomSortedCategory("Size and Position", 3)> _
        Property Radius() As Integer
            Get
                Return _Radius
            End Get
            Set(ByVal value As Integer)
                _Radius = value
                Me.Size = New Size((_Radius + _Thickness) * 2, (_Radius + _Thickness) * 2)
                Me.Invalidate()
            End Set
        End Property

#End Region

        Private Sub Circle_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
            RecalcCoords()
        End Sub

        Private Sub Circle_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            RecalcCoords()
        End Sub

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Shadows Property Location() As Point
            Get
                Return MyBase.Location
            End Get
            Set(ByVal value As Point)
                MyBase.Location = value
                RecalcCoords()
            End Set
        End Property

        Private Sub RecalcCoords()
            'If MyBase.Width > MyBase.Height Then
            _Radius = (MyBase.Width - _Thickness * 2) / 2
            'Else
            '_Radius = (MyBase.Height - _Thickness * 2) / 2
            'End If
            _X = MyBase.Location.X + _Radius
            _Y = MyBase.Location.Y + _Radius
            Me.Invalidate()
        End Sub

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Shadows Property Width() As Integer
            Get
                Return MyBase.Width
            End Get
            Set(ByVal value As Integer)
                MyBase.Width = value
                MyBase.Height = value
                RecalcCoords()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Shadows Property Height() As Integer
            Get
                Return MyBase.Height
            End Get
            Set(ByVal value As Integer)
                MyBase.Width = value
                MyBase.Height = value
                RecalcCoords()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Shadows Property Size() As Size
            Get
                Return MyBase.Size
            End Get
            Set(ByVal value As Size)
                If MyBase.Width <> value.Width Then
                    MyBase.Width = value.Width
                    MyBase.Height = value.Width
                    RecalcCoords()
                End If
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

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = ""
            Dim MyCode As String = "", MyState As String = ""
            Dim MyCodeUpdate As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)

            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            MyCodeUpdate = CodeGen.CodeUpdateTemplate

            MyCode = MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[X]", Me.X).Replace("[Y]", Me.Y) _
                .Replace("[RADIUS]", _Radius) _
                .Replace("[STATEMENT]", CodeGen.GetStatement("Fill", _Fill.ToString)) _
                .Replace("[THICKNESS]", Me.Thickness.ToString) _
                .Replace("[TYPE]", Me.Type.ToString) _
                .Replace("[LINETYPE]", CodeGen.LineTypeMLA4(_Thickness, _Type)) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Circle " & Me.Name)) _
                .Replace("[COLOR_STRING]", CodeGen.Color2String(Me._Color)) _
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

            CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCodeUpdate.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[X]", Me.X).Replace("[Y]", Me.Y) _
                .Replace("[RADIUS]", _Radius) _
                .Replace("[STATEMENT]", CodeGen.GetStatement("Fill", _Fill.ToString)) _
                .Replace("[THICKNESS]", Me.Thickness.ToString) _
                .Replace("[TYPE]", Me.Type.ToString) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Circle " & Me.Name)) _
                .Replace("[COLOR_STRING]", CodeGen.Color2String(Me._Color)) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            If _ZDrawFirst Then
                CodeGen.AddLines(CodeGen.Code, MyCode)
            Else
                CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCode)
            End If


        End Sub

#End If
    End Class
End Namespace
