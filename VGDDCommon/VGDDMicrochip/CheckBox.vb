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
    <ToolboxBitmap(GetType(Button), "CheckBoxIco")> _
    Public Class CheckBox : Inherits VGDDWidget

        Private _HorizAlign As HorizAlign = HorizAlign.Left
        Private _VertAlign As VertAlign = VertAlign.Center
        Private _Checked As Boolean
        Friend Shared _Instances As Integer = 0

        Public Event CheckedChanged As EventHandler

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("CheckBox")
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Me.RemovePropertyToShow("VertAlign")
                    Me.RemovePropertyToShow("HorizAlign")
            End Select
#End If
            Me.Size = New Size(150, 25)
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
            g.DrawRectangle(ps, rc.Left, rc.Top, rc.Left + rc.Height, rc.Bottom)
            g.DrawLine(ps, rc.Left + rc.Height - 2, rc.Top + 2, rc.Left + rc.Height - 2, rc.Bottom - 2)
            g.DrawLine(ps, rc.Left + rc.Height - 2, rc.Bottom - 2, rc.Left + 2, rc.Bottom - 2)

            ps.Color = _Scheme.Embossdkcolor
            g.DrawLine(ps, rc.Left + 2, rc.Bottom - 2, rc.Left + 2, rc.Top + 2)
            g.DrawLine(ps, rc.Left + 2, rc.Top + 2, rc.Left + rc.Height - 2, rc.Top + 2)

            'Draw Text
            Dim textBrush As SolidBrush = New SolidBrush(IIf(_State, _Scheme.Textcolor0, _Scheme.Colordisabled))
            Dim intTextSize As SizeF = g.MeasureString(Text, MyBase.Font)
            g.DrawString(Text, MyBase.Font, textBrush, Height + 2, (Height - intTextSize.Height) / 2)
            If Not Checked Then
                textBrush.Color = IIf(_State, _Scheme.Color0, _Scheme.Colordisabled)
            End If
            g.FillRectangle(textBrush, rc.Left + 3, rc.Top + 3, rc.Left + rc.Height - 6, rc.Bottom - 6)
        End Sub

#Region "GDDProps"

        <Description("Horizontal Text Alignement of the Checkbox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Left")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property HorizAlign() As HorizAlign
            Get
                Return _HorizAlign
            End Get
            Set(ByVal value As HorizAlign)
                _HorizAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Vertical Text Alignement of the Checkbox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(VertAlign), "Center")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property VertAlign() As VertAlign
            Get
                Return _VertAlign
            End Get
            Set(ByVal value As VertAlign)
                _VertAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Checkbox Checked?")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Checked As Boolean
            Get
                Return _Checked
            End Get
            Set(ByVal value As Boolean)
                _Checked = value
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.MyCodeHead(_CDeclType)
            Dim MyCode As String = String.Empty, MyState As String = String.Empty, MyAlignment As String = String.Empty

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
            CodeGen.AddState(MyState, "Checked", Me.Checked.ToString)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _HorizAlign.ToString)
                    CodeGen.AddAlignment(MyAlignment, "Vertical", _VertAlign.ToString)
            End Select

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            If Common.ProjectMultiLanguageTranslations > 0 AndAlso MyBase.TextStringID < 0 Then
                CodeGen.Errors &= MyControlId & " has empty text ID" & vbCrLf
            End If

            CodeGen.AddLines(CodeGen.Code, MyCode _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[ALIGNMENT]", MyAlignment) _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[SCHEME]", Me.Scheme) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            Dim MyHeaders As String = String.Empty
            If Me.Public Then
                CodeGen.AddLines(MyHeaders, "extern " & CodeGen.ConstructorTemplate.Trim)
            End If
            If Me.Text <> String.Empty Then
                CodeGen.AddLines(MyHeaders, CodeGen.MyHeader(_CDeclType))
            End If
            CodeGen.AddLines(MyHeaders, CodeGen.TextDeclareHeaderTemplate(_CDeclType))
            CodeGen.AddLines(MyHeaders, CodeGen.HeadersTemplate)
            CodeGen.AddLines(CodeGen.Headers, MyHeaders _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
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

        Private Sub CheckBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(Me.Text)
            End If
        End Sub

        Private Sub CheckBox_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            CheckBox_TextChanged(Nothing, Nothing)
        End Sub

        Private Sub CheckBox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click
            RaiseEvent CheckedChanged(sender, e)
        End Sub
    End Class
End Namespace
