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
    <ToolboxBitmap(GetType(Button), "RoundDialIco")> _
    Public Class RoundDial : Inherits VGDDWidget

        Private _Max As Short = 100
        Private _Radius As Short = 30
        Private _Resolution As Short = 1
        Private _Value As Short = 1
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
            'Me.Width = 150
            'Me.Height = 150
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("RoundDial")
#End If
            Me.Size = New Size(150, 150)
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
            Mypath.AddArc(Me.ClientRectangle, 0, 360)
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)
            If _Scheme Is Nothing Then Exit Sub
            Dim brushBackGround As New SolidBrush(IIf(_State, _Scheme.Color0, _Scheme.Colordisabled))
            g.FillRegion(brushBackGround, Me.Region)

            Dim brushPen As New SolidBrush(_Scheme.Color0)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 3
            ps.Color = _Scheme.Embossltcolor
            g.DrawArc(ps, rc.Left + 2, rc.Top + 2, rc.Right - 4, rc.Bottom - 4, 135, 180)

            ps.Color = _Scheme.Embossdkcolor
            g.DrawArc(ps, rc.Left + 2, rc.Top + 2, rc.Right - 4, rc.Bottom - 4, 315, 180)

            brushPen.Color = _Scheme.Embossdkcolor
            g.FillEllipse(brushPen, CInt(rc.Right * 0.2), CInt(rc.Bottom * 0.2), CInt(Me.Width * 0.15), CInt(Me.Height * 0.15))

            ps.Width = 1
            ps.Color = _Scheme.Embossltcolor
            g.DrawArc(ps, CInt(rc.Right * 0.2), CInt(rc.Bottom * 0.2), CInt(Me.Width * 0.15), CInt(Me.Height * 0.15), 0, 360)
        End Sub

#Region "GDDProps"

        <Description("Center X coordinate of the RoundDial")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property XCenter() As Short
            Get
                Return Me.Location.X + Me.Width / 2
            End Get
            Set(ByVal value As Short)
                Me.Location = New Point(value - Me.Width / 2, MyBase.Location.Y)
                Me.Invalidate()
            End Set
        End Property

        <Description("Center Y coordinate of the RoundDial")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property YCenter() As Short
            Get
                Return Me.Location.Y + Me.Height / 2
            End Get
            Set(ByVal value As Short)
                Me.Location = New Point(MyBase.Location.X, value - Me.Height / 2)
                Me.Invalidate()
            End Set
        End Property

        <Description("Current value for the RoundDial")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Property Value() As Short
            Get
                Return _Value
            End Get
            Set(ByVal value As Short)
                _Value = value
                'Me.Invalidate()
            End Set
        End Property

        <Description("Increment/Decrement value for the RoundDial")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Property Resolution() As Short
            Get
                Return _Resolution
            End Get
            Set(ByVal value As Short)
                _Resolution = value
                'Me.Invalidate()
            End Set
        End Property

        <Description("Maximum value for the RoundDial")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Property Max() As Short
            Get
                Return _Max
            End Get
            Set(ByVal value As Short)
                _Max = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Radius for the RoundDial")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Radius() As Short
            Get
                Return _Radius
            End Get
            Set(ByVal value As Short)
                _Radius = value
                Me.Size = New Size(_Radius * 2, _Radius * 2)
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

            Dim myText As String = ""
            Me.Text = Me.Text.PadRight(GetMaxTextLength(Me.TextStringID), "_") ' DW
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[XCENTER]", Me.XCenter) _
                .Replace("[YCENTER]", Me.YCenter) _
                .Replace("[RADIUS]", Me.Radius) _
                .Replace("[MAX]", Me.Max) _
                .Replace("[STATE]", MyState) _
                .Replace("[RES]", Me.Resolution) _
                .Replace("[VALUE]", Me.Value).Replace("[SCHEME]", Me.Scheme))

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

        Private Sub RoundDial_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            Static oldWidth As Integer, oldHeight As Integer
            If Me.Width <> oldWidth Then
                Me.Height = Me.Width
            ElseIf Me.Height <> oldHeight Then
                Me.Width = Me.Height
            End If
            Me.XCenter = MyBase.Location.X + Me.Width / 2
            Me.YCenter = MyBase.Location.Y + Me.Height / 2
            Radius = Me.Width / 2
            oldWidth = Me.Width
            oldHeight = Me.Height
        End Sub

    End Class

End Namespace
