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
    <ToolboxBitmap(GetType(Button), "SliderIco")> _
    Public Class Slider : Inherits VGDDWidget

        Private _Range As Short = 100
        Private _Pos As Short = 30
        Private _Orientation As Common.Orientation
        Private _Page As Short = 1
        Private _SliderType As SliderType

        Private _Animated As Boolean = False
        Private _AnimationDirection As Integer = 1
        Private WithEvents AniTimer As New Timer
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Slider")
#End If
            Me.Size = New Size(150, 20)
            _Orientation = Common.Orientation.Horizontal
            _SliderType = SliderType.Slider
            Me.RemovePropertyToShow("Text")
        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
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

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Demolimit As Integer
            Get
                Return Common.DEMOCODELIMIT
            End Get
        End Property

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            If Me.Scheme Is Nothing OrElse Me.Scheme = String.Empty Then
                If Not Common.VGDDIsRunning Then
                    'Me.Scheme = Common.CreateDesignScheme
                Else
                    Exit Sub
                End If
            End If
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

            'Dim brushBackGround As New SolidBrush(IIf(_State, _Scheme.Color0, _Scheme.Colordisabled))
            'g.FillRegion(brushBackGround, Me.Region)

            MyBase.FillBackground(g, IIf(_State, _Scheme.Color0, _Scheme.Colordisabled), Me.Region)

            Dim brushPen As New SolidBrush(_Scheme.Color0)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 2
            ps.Color = _Scheme.Embossdkcolor
            'g.DrawRectangle(ps, rc.Left, rc.Top, rc.Right, rc.Bottom)
            g.DrawLine(ps, rc.Right - 1, rc.Top, rc.Right - 1, rc.Bottom)
            g.DrawLine(ps, rc.Right - 1, rc.Bottom - 1, rc.Left + 2, rc.Bottom - 1)

            ps.Color = _Scheme.Embossltcolor
            g.DrawLine(ps, rc.Left + 1, rc.Bottom, rc.Left + 1, rc.Top)
            g.DrawLine(ps, rc.Left + 2, rc.Top + 1, rc.Right - 2, rc.Top + 1)

            brushPen.Color = IIf(_State, _Scheme.Color0, _Scheme.Colordisabled)
            Dim intMezzH As Integer = rc.Left + Me.Width / 2
            Dim intMezzV As Integer = rc.Top + Me.Height / 2
            Dim CursorW As Integer = IIf(Me.SliderType = SliderType.Slider, 6, 3)
            Select Case _Orientation
                Case Common.Orientation.Horizontal
                    If Me.SliderType = SliderType.Slider Then
                        ps.Color = _Scheme.Embossdkcolor
                        g.DrawLine(ps, rc.Left + 10, intMezzV, rc.Right - 10, intMezzV)
                        ps.Color = _Scheme.Embossltcolor
                        g.DrawLine(ps, rc.Left + 10, intMezzV + 1, rc.Right - 10, intMezzV + 1)
                    End If
                    ps.Color = _Scheme.Embossdkcolor
                    Dim CursorPosX As Integer = CursorW * 2 + Me.Pos / Me.Range * (Me.Width - CursorW * 4)
                    g.DrawRectangle(ps, CursorPosX - CursorW, rc.Top + 6, CursorW * 2, rc.Bottom - 12)
                    g.FillRectangle(brushPen, CursorPosX - CursorW + 1, rc.Top + 7, (CursorW - 1) * 2, rc.Bottom - 14)
                Case Common.Orientation.Vertical
                    If Me.SliderType = SliderType.Slider Then
                        ps.Color = _Scheme.Embossdkcolor
                        g.DrawLine(ps, intMezzH, rc.Top + 10, intMezzH, rc.Bottom - 10)
                        ps.Color = _Scheme.Embossltcolor
                        g.DrawLine(ps, intMezzH + 1, rc.Top + 10, intMezzH + 1, rc.Bottom - 10)
                    End If
                    ps.Color = _Scheme.Embossdkcolor
                    Dim CursorPosY As Integer = Me.Height - (CursorW * 3 + Me.Pos / IIf(Me.Range > 0, Me.Range, 1) * (Me.Height - CursorW * 6))
                    g.DrawRectangle(ps, rc.Left + 6, CursorPosY - CursorW, rc.Right - 12, CInt(CursorW * 2))
                    g.FillRectangle(brushPen, rc.Left + 7, CursorPosY - CursorW + 1, rc.Right - 14, CInt((CursorW - 1) * 2) + 1)
            End Select

        End Sub

#Region "GDDProps"

        <Description("Should Player Animate this Widget?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
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

        <Description("Orientation of the Slider")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Orientation() As Common.Orientation
            Get
                Return _Orientation
            End Get
            Set(ByVal value As Common.Orientation)
                If Not Me.IsLoading AndAlso value <> _Orientation Then
                    Dim w As Integer = Me.Width
                    Me.Width = Me.Height
                    Me.Height = w
                End If
                _Orientation = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Incremental change of the Slider")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Overloads Property Page() As Short
            Get
                Return _Page
            End Get
            Set(ByVal value As Short)
                _Page = value
            End Set
        End Property

        <Description("Type of the Slider")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Overloads Property SliderType() As SliderType
            Get
                Return _SliderType
            End Get
            Set(ByVal value As SliderType)
                _SliderType = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Maximum value for the Slider")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Overloads Property Range() As Short
            Get
                Return _Range
            End Get
            Set(ByVal value As Short)
                _Range = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Default initial value for the Slider")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Overloads Property Pos() As Short
            Get
                Return _Pos
            End Get
            Set(ByVal value As Short)
                _Pos = value
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

            If _public Then
                CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            Else
                CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            End If
            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", IIf(_State = EnabledState.Enabled, True, False))
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "SliderType", Me.SliderType.ToString)
            CodeGen.AddState(MyState, "Orientation", _Orientation.ToString)

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[RANGE]", Me.Range) _
                .Replace("[PAGE]", Me.Page) _
                .Replace("[POS]", Me.Pos) _
                .Replace("[SCHEME]", Me.Scheme))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, (IIf(Me.Public, vbCrLf & "extern " & CodeGen.ConstructorTemplate.Trim, "") & CodeGen.HeadersTemplate) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, Me)

        End Sub
#End If

        Private Sub AniTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles AniTimer.Tick
            If Common.AnimationEnable Then
                Me._Pos += Rnd(1) * Me._Range / 10 * _AnimationDirection
                If Me._Pos > Me._Range Then
                    Me._Pos = Me._Range
                    _AnimationDirection *= -1
                ElseIf Me._Pos < 0 Then
                    Me._Pos = 0
                    _AnimationDirection *= -1
                End If
                Me.Invalidate()
            End If
        End Sub

    End Class

End Namespace