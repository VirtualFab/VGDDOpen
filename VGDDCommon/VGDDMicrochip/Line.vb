Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip

    '<Designer(GetType(LineDesigner))> _
    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "LineIco")> _
    Public Class Line : Inherits VGDDBase
        'Implements ISupportInitialize

        Private _Color As Color = Drawing.Color.Blue
        Private _Thickness As Short
        Private _Type As LineType
        Private _Swap As Boolean = False
        Private _ZDrawFirst As Boolean = True

        'Private _X1 As Integer = -1
        'Private _X2 As Integer = -1
        'Private _Y1 As Integer = -1
        'Private _Y2 As Integer = -1

        Public Sub New()
            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.Opaque, False)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, False)
            Me.Thickness = Thickness.NORMAL_LINE
            Me.RemovePropertyToShow("Top")
            Me.RemovePropertyToShow("Bottom")
            Me.RemovePropertyToShow("Left")
            Me.RemovePropertyToShow("Right")
            Me.RemovePropertyToShow("Text")
            Me.RemovePropertyToShow("TextStringID")
        End Sub

        Protected Overrides Sub SetBoundsCore(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal specified As System.Windows.Forms.BoundsSpecified)
            'Debug.Print("SetBoundsCore")
            If width <= 10 Then width = _Thickness * 2 + 1 + 1
            If height <= 10 Then height = _Thickness * 2 + 1 + 1
            MyBase.SetBoundsCore(x, y, width, height, specified)
        End Sub

        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim _Cp As CreateParams = MyBase.CreateParams
                _Cp.ExStyle = _Cp.ExStyle Or &H20
                Return _Cp
            End Get
        End Property

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

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle 'New Drawing.Rectangle(_X1, _Y1, _X2 - _X1, _Y2 - _Y1) '

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
            ps.Width = _Thickness * 2 + 1

            'Dim intPxThickness As Integer = _Thickness * 2 + 1
            Dim l, r, t, b As Integer
            t = rc.Top
            b = rc.Bottom
            If _Swap Then
                l = rc.Right
                r = rc.Left
                'intPxThickness = intPxThickness * 2
            Else
                l = rc.Left
                r = rc.Right
            End If

            'Dim Mypath As GraphicsPath = New GraphicsPath
            'Mypath.StartFigure()
            ''Mypath.AddRectangle(rc)
            'If _Swap Then
            '    Mypath.AddPolygon({ _
            '                      New Point(l - intPxThickness, t), _
            '                      New Point(r, b - intPxThickness), _
            '                      New Point(r + intPxThickness, b), _
            '                      New Point(l, t + intPxThickness)
            '                       })
            'Else
            '    Mypath.AddPolygon({ _
            '                      New Point(l + intPxThickness, t), _
            '                      New Point(r, b - intPxThickness), _
            '                      New Point(r - intPxThickness, b), _
            '                      New Point(l, t + intPxThickness)
            '                       })
            'End If
            'Mypath.CloseFigure()
            'Me.Region = New Region(Mypath)

            'g.FillPolygon(brushPen, { _
            '                  New Point(rc.Left + _Thickness, rc.Top), _
            '                  New Point(rc.Right, rc.Bottom - _Thickness), _
            '                  New Point(rc.Right - _Thickness, rc.Bottom), _
            '                  New Point(rc.Left, rc.Top + _Thickness)
            '                   })
            Dim lx1 As Integer = l
            Dim ly1 As Integer = t
            Dim lx2 As Integer = r
            Dim ly2 As Integer = b
            If Math.Abs(lx2 - lx1) < 10 Then ' Vertical line
                lx1 = 0
                lx2 = lx1
                'ly1 += 1
                'ly2 -= ps.Width
            ElseIf Math.Abs(ly2 - ly1) < 10 Then 'Horizontal line
                ly1 = 0
                ly2 = ly1
                'lx1 += 1
                'lx2 -= ps.Width
            End If
            g.DrawLine(ps, lx1, ly1, lx2, ly2)
            'Dim r As New System.Drawing.Rectangle(0, rc.Height, 5, 5)
            'ControlPaint.DrawGrabHandle(g, r, True, True)
        End Sub


#Region "GDDProps"

        <Description("Swap Line Inclination?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Swap() As Boolean
            Get
                Return _Swap
            End Get
            Set(ByVal value As Boolean)
                _Swap = value
                Me.RecreateHandle()
                Me.Invalidate()
            End Set
        End Property

        <Description("Thickness of the line")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Thickness() As ThickNess
            Get
                Return _Thickness
            End Get
            Set(ByVal value As ThickNess)
                _Thickness = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Type of the line")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property [Type]() As LineType
            Get
                Return _Type
            End Get
            Set(ByVal value As LineType)
                _Type = value
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Color for the Line")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
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

        <Description("Left X coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property X1() As Integer
            Get
                If _Swap Then
                    If Me.Width < 10 Then
                        Return Me.Left
                    Else
                        Return Me.Left + Me.Width
                    End If
                Else
                    Return Me.Left
                End If
            End Get
            Set(ByVal value As Integer)
                Me.Left = value
                'If Me.Width <= 10 Then Me.Width = 0
                Me.Invalidate()
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
         <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property X2() As Integer
            Get
                If _Swap Then
                    Return Me.Left
                Else
                    If Me.Width < 10 Then
                        Return Me.Left
                    Else
                        Return Me.Left + Me.Width
                    End If
                End If
            End Get
            Set(ByVal value As Integer)
                If Me.IsLoading Or value >= Me.Left Then
                    Me.Width = value - Me.Left
                Else
                    Me.Width = Me.Left - value
                    Me.Left = value
                End If
                'If Me.Width <= 10 Then Me.Width = 0
                Me.Invalidate()
            End Set
        End Property

        <Description("Top Y coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Y1() As Integer
            Get
                Return Me.Top
            End Get
            Set(ByVal value As Integer)
                If Not Me.IsLoading Then
                    Me.Height = Me.Top + Me.Height - value
                End If
                Me.Top = value
                'If Me.Height <= 10 Then Me.Height = 0
                Me.Invalidate()
            End Set
        End Property

        <Description("Bottom Y coordinate of the lower-right edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Y2() As Integer
            Get
                If Me.Height < 10 Then
                    Return Me.Top
                Else
                    Return Me.Top + Me.Height
                End If
            End Get
            Set(ByVal value As Integer)
                If Me.IsLoading Or value >= Me.Top Then
                    Me.Height = value - Me.Top
                Else
                    Me.Height = Me.Top - value
                    Me.Top = value
                End If
                'If Me.Height <= 10 Then Me.Height = 0
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

        Private Sub Line_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
            Me.Location = MyBase.Location
        End Sub

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
                .Replace("[X1]", Me.X1).Replace("[Y1]", Me.Y1) _
                .Replace("[X2]", Me.X2).Replace("[Y2]", Me.Y2) _
                .Replace("[THICKNESS]", Me.Thickness.ToString) _
                .Replace("[TYPE]", Me.Type.ToString) _
                .Replace("[LINETYPE]", CodeGen.LineTypeMLA4(_Thickness, _Type)) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Line " & Me.Name)) _
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
                .Replace("[X1]", Me.X1).Replace("[Y1]", Me.Y1) _
                .Replace("[X2]", Me.X2).Replace("[Y2]", Me.Y2) _
                .Replace("[THICKNESS]", Me.Thickness.ToString) _
                .Replace("[TYPE]", Me.Type.ToString) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Line " & Me.Name)) _
                .Replace("[COLOR_STRING]", CodeGen.Color2String(Me._Color)) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            If _ZDrawFirst Then
                CodeGen.AddLines(CodeGen.Code, MyCode)
            Else
                CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCode)
            End If
        End Sub
#End If

        'Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit

        'End Sub

        'Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit

        'End Sub
    End Class

    '#Region " Snapline stuff "

    'Class MyDesignerOptionService
    '    Inherits DesignerOptionService

    '    Public Sub New()
    '    End Sub

    '    '/ <summary>
    '    '/ Populates an option collection with the settings for the DesignSurface.
    '    '/ </summary>
    '    '/ <param name="options"></param>

    '    Protected Overrides Sub PopulateOptionCollection(ByVal options As DesignerOptionCollection)
    '        If options.Parent Is Nothing Then
    '            Dim opt As Windows.Forms.Design.DesignerOptions = New Windows.Forms.Design.DesignerOptions()
    '            'opt.UseSmartTags = True
    '            opt.UseSnapLines = False
    '            opt.SnapToGrid = True
    '            opt.ShowGrid = True
    '            'opt.ObjectBoundSmartTagAutoShow = True
    '            Dim wfdc As DesignerOptionCollection = Me.CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
    '            Dim wfdgc As DesignerOptionCollection = Me.CreateOptionCollection(wfdc, "General", opt)
    '        End If
    '    End Sub
    'End Class
    '#End Region

    'Friend Class LineDesigner
    '    Inherits ControlDesigner
    '    Private Shared ReadOnly AdornmentDimensions As Size = New Size(7, 7)
    '    Private behaviorSvc As Behavior.BehaviorService = Nothing

    '    Public Sub New()
    '        'Stop
    '    End Sub

    '    Public Overrides Sub Initialize(ByVal component As IComponent)
    '        MyBase.Initialize(component)

    '        Me.behaviorSvc = GetService(GetType(Behavior.BehaviorService))
    '        For Each b In Me.behaviorSvc.Adorners
    '            b.Enabled = False
    '            'For Each g As System.Windows.Forms.Design.Behavior.Glyph In b.Glyphs

    '            'Next

    '        Next
    '    End Sub

    '    Public Overrides Sub InitializeExistingComponent(ByVal defaultValues As System.Collections.IDictionary)
    '        MyBase.InitializeExistingComponent(defaultValues)
    '        For Each b In Me.behaviorSvc.Adorners
    '            b.Enabled = False
    '        Next
    '    End Sub

    '    Protected Overridable ReadOnly Property Line() As Line
    '        Get
    '            Return CType(Component, Line)
    '        End Get
    '    End Property

    '    Protected Overrides Sub OnPaintAdornments(ByVal e As System.Windows.Forms.PaintEventArgs)
    '        ' The line designer draws a grab on each end of the line.
    '        '
    '        Dim grabCenter As Integer = AdornmentDimensions.Width / 2

    '        Dim startRect As New System.Drawing.Rectangle(New Point(Line.X1, Line.Y1), AdornmentDimensions)
    '        Dim endRect As New System.Drawing.Rectangle(New Point(Line.X2, Line.Y2), AdornmentDimensions)
    '        startRect.Offset(-grabCenter, -grabCenter)
    '        endRect.Offset(-grabCenter, -grabCenter)

    '        ControlPaint.DrawGrabHandle(e.Graphics, startRect, True, True)
    '        ControlPaint.DrawGrabHandle(e.Graphics, endRect, True, True)
    '    End Sub
    'End Class
End Namespace
