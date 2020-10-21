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
    <ToolboxBitmap(GetType(Button), "ProgressBarIco")> _
    Public Class ProgressBar : Inherits VGDDWidget

        Private _Range As Integer = 100
        Private _Pos As Integer = 30
        Private _Orientation As Common.Orientation = Common.Orientation.Horizontal
        Private _ShowPercentage As Boolean = True

        Private _Animated As Boolean = False
        Private _AnimationDirection As Integer = 1
        Private WithEvents AniTimer As New Timer
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("ProgressBar")
#End If
            Me.Size = New Size(150, 20)
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

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
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
            If _Scheme Is Nothing Then Exit Sub

            'Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
            'g.FillRegion(brushBackGround, Me.Region)

            MyBase.FillBackground(g, _Scheme.Commonbkcolor, Me.Region)

            Dim brushPen As New SolidBrush(_Scheme.Color0)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 2
            ps.Color = _Scheme.Embossltcolor
            g.DrawRectangle(ps, rc.Left, rc.Top, rc.Right, rc.Bottom)
            g.DrawLine(ps, rc.Right - 2, rc.Top + 2, rc.Right - 2, rc.Bottom - 2)
            g.DrawLine(ps, rc.Right - 2, rc.Bottom - 2, rc.Left + 2, rc.Bottom - 2)

            ps.Color = _Scheme.Embossdkcolor
            g.DrawLine(ps, rc.Left + 2, rc.Bottom - 2, rc.Left + 2, rc.Top + 2)
            g.DrawLine(ps, rc.Left + 2, rc.Top + 2, rc.Right - 2, rc.Top + 2)

            Select Case Me.Orientation
                Case Common.Orientation.Horizontal
                    brushPen.Color = _Scheme.Color1
                    g.FillRectangle(brushPen, rc.Left + 4, rc.Top + 4, CInt((rc.Right - 8) * _Pos / _Range), rc.Bottom - 8)
                Case Common.Orientation.Vertical
                    brushPen.Color = _Scheme.Color1
                    g.FillRectangle(brushPen, rc.Left + 4, rc.Bottom - CInt((Height - 4) * _Pos / _Range), rc.Right - 8, CInt((Height - 4) * _Pos / _Range) - 4)
            End Select

            If _ShowPercentage Then
                'Draw Text
                Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)
                Dim strText As String = String.Format("{0}%", _Pos)
                Dim TextSize As SizeF = g.MeasureString(strText, MyBase.Font)
                g.DrawString(strText, MyBase.Font, textBrush, (Width - TextSize.Width) / 2 + 4, (Height - TextSize.Height) / 2)
            End If
        End Sub

#Region "GDDProps"

        <Description("Should the percentage be shown?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property ShowPercentage() As Boolean
            Get
                Return _ShowPercentage
            End Get
            Set(ByVal value As Boolean)
                _ShowPercentage = value
                Me.Invalidate()
            End Set
        End Property

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

        <Description("Orientation of the ProgressBar")> _
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

        <Description("Maximum value for the ProgressBar")> _
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

        <Description("Default initial value for the ProgressBar")> _
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
            CodeGen.AddState(MyState, "Orientation", Me.Orientation.ToString)
            CodeGen.AddState(MyState, "ShowPercentage", Me.ShowPercentage.ToString)

            Dim myText As String = ""
            Me.Text = Me.Text.PadRight(GetMaxTextLength(Me.TextStringID), "_") ' DW
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[POS]", Me.Pos) _
                .Replace("[RANGE]", Me.Range) _
                .Replace("[SCHEME]", Me.Scheme))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, (IIf(Me.Public, vbCrLf & "extern " & CodeGen.ConstructorTemplate.Trim, "") & CodeGen.HeadersTemplate) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
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
