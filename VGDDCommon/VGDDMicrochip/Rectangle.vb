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
    <ToolboxBitmap(GetType(Button), "RectangleIco")> _
    Public Class Rectangle : Inherits VGDDBase

        Private _Color As Color = Drawing.Color.Blue
        Private _Thickness As ThickNess = Thickness.NORMAL_LINE
        Private _Type As LineType
        Private _Fill As Boolean
        Private FirstTime As Boolean = True
        Private _ZDrawFirst As Boolean = True

        Public Sub New()
            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.Opaque, False)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, False)
            Me.Size = New Size(200, 100)
            Me.RemovePropertyToShow("Text")
            Me.RemovePropertyToShow("TextStringID")
        End Sub

        'Protected Overrides ReadOnly Property CreateParams() As CreateParams
        '    Get
        '        Dim _Cp As CreateParams = MyBase.CreateParams
        '        _Cp.ExStyle = _Cp.ExStyle Or &H20
        '        Return _Cp
        '    End Get
        'End Property

        Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
            MyBase.OnPaintBackground(pevent)
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
            MyBase.OnPaint(pevent)

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
            If _Fill Then
                Mypath.AddRectangle(rc)
            Else
                Mypath.AddRectangle(New Drawing.Rectangle(rc.Left, rc.Top, rc.Width, intPxThickness))
                Mypath.AddRectangle(New Drawing.Rectangle(rc.Right - intPxThickness, rc.Top + intPxThickness, intPxThickness, rc.Height - intPxThickness))
                Mypath.AddRectangle(New Drawing.Rectangle(rc.Left + intPxThickness, rc.Bottom - intPxThickness, rc.Width - intPxThickness * 2, intPxThickness))
                Mypath.AddRectangle(New Drawing.Rectangle(rc.Left, rc.Top + intPxThickness, intPxThickness, rc.Height))
            End If
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)

            'Dim brushBackGround As New SolidBrush(_Scheme.Color0)
            'g.FillRegion(brushBackGround, Me.Region)
            If _Fill Then
                g.FillRectangle(brushPen, CInt(rc.Left), CInt(rc.Top), CInt(Me.Width), CInt(Me.Height))
            Else
                'g.DrawRectangle(ps, CInt(rc.Left), CInt(rc.Top), Me.Width - intPxThickness, Me.Height - intPxThickness)
                g.DrawRectangle(ps, rc)
            End If

            'If FirstTime Then
            '    Me.SendToBack()
            '    FirstTime = False
            'End If
        End Sub

#Region "GDDProps"

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

        <Description("Thickness of the Rectangle")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(ThickNess), "NORMAL_LINE")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Thickness() As ThickNess
            Get
                Return _Thickness
            End Get
            Set(ByVal value As ThickNess)
                _Thickness = value
                'Me.RecreateHandle()
                Me.Invalidate()
            End Set
        End Property

        <Description("Line type for the Rectangle")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(LineType), "SOLID_LINE")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property [Type]() As LineType
            Get
                Return _Type
            End Get
            Set(ByVal value As LineType)
                _Type = value
                'Me.RecreateHandle()
                Me.Invalidate()
            End Set
        End Property

        <Description("Should the Rectangle be filled")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Fill() As Boolean
            Get
                Return _Fill
            End Get
            Set(ByVal value As Boolean)
                _Fill = value
                If _Fill Then
                    MyBase.RemovePropertyToShow("Type")
                    MyBase.RemovePropertyToShow("Thickness")
                    Me.SendToBack()
                Else
                    MyBase.AddPropertyToShow("Type")
                    MyBase.AddPropertyToShow("Thickness")
                End If
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Color for the Rectangle")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property [Color]() As Color
#Else
        Public Overloads Property [Color]() As Color
#End If
            Get
                Return _Color
            End Get
            Set(ByVal value As Color)
                _Color = value
                Me.Invalidate()
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

            MyCode = MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[STATEMENT]", CodeGen.GetStatement("Fill", _Fill.ToString)) _
                .Replace("[LEFT]", Me.Left).Replace("[RIGHT]", Me.Right) _
                .Replace("[TOP]", Me.Top).Replace("[BOTTOM]", Me.Bottom) _
                .Replace("[LINETYPE]", CodeGen.LineTypeMLA4(_Thickness, _Type)) _
                .Replace("[THICKNESS]", Me.Thickness.ToString) _
                .Replace("[TYPE]", Me.Type.ToString) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Rectangle " & Me.Name)) _
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

            If _ZDrawFirst Then
                CodeGen.AddLines(CodeGen.Code, MyCode)
            Else
                CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCode)
            End If
        End Sub
#End If
    End Class

End Namespace
