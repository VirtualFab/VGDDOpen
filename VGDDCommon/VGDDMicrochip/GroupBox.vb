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
    <ToolboxBitmap(GetType(Button), "GroupBoxIco")> _
    Public Class GroupBox : Inherits VGDDWidget

        Private _TextAlign As HorizAlign = HorizAlign.Left
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("GroupBox")
#End If
            Me.Size = New Size(150, 150)
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
            Dim TextSize As SizeF = g.MeasureString(Text, MyBase.Font), intMezz As Integer = TextSize.Height / 2

            'Impostazione Region
            Dim Mypath As GraphicsPath = New GraphicsPath
            Mypath.StartFigure()
            Mypath.AddRectangle(rc)
            Mypath.AddRectangle(New System.Drawing.Rectangle(rc.Left + 2, rc.Top + intMezz + 6, rc.Width - 6, rc.Height - intMezz * 2 - 8))
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)
            If _Scheme Is Nothing Then Exit Sub
            Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
            g.FillRegion(brushBackGround, Me.Region)

            Dim brushPen As New SolidBrush(_Scheme.Color0)
            Dim ps As Pen = New Pen(brushPen)

            'Draw Text
            Dim textBrush As SolidBrush = New SolidBrush(IIf(_State, _Scheme.Textcolor0, _Scheme.Colordisabled))

            ps.Width = 2
            ps.Color = IIf(_State, _Scheme.Embossdkcolor, _Scheme.Colordisabled)
            'g.DrawRectangle(ps, 1, intMezz, rc.Right - 2, rc.Bottom - intMezz - 1)
            g.DrawLine(ps, 1, intMezz, 1, rc.Bottom - 1)
            g.DrawLine(ps, 1, rc.Bottom - 1, rc.Right - 2, rc.Bottom - 1)
            g.DrawLine(ps, rc.Right - 2, rc.Bottom - 1, rc.Right - 2, intMezz)
            g.DrawLine(ps, 1, intMezz, 4, intMezz)
            g.DrawLine(ps, TextSize.Width + 6, intMezz, rc.Right - 2, intMezz)

            ps.Color = _Scheme.Commonbkcolor
            Select Case _TextAlign
                Case HorizAlign.Center
                    'g.DrawLine(ps, (Width - TextSize.Width) / 2 - 6, intMezz - 4, (Width - TextSize.Width) / 2 + TextSize.Width, intMezz - 4)
                    g.DrawString(Text, MyBase.Font, textBrush, (Width - TextSize.Width - 8) / 2, 0)
                Case HorizAlign.Right
                    'g.DrawLine(ps, Width - TextSize.Width - 6 - 8, intMezz - 4, Width - 6, intMezz - 4)
                    g.DrawString(Text, MyBase.Font, textBrush, Width - TextSize.Width - 8, 0)
                Case Else
                    'g.DrawLine(ps, 6, intMezz - 4, TextSize.Width + 6, intMezz - 4)
                    g.DrawString(Text, MyBase.Font, textBrush, 4, 0)
            End Select
        End Sub

#Region "GDDProps"

        <Description("Text Alignement of the GroupBox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Left")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property TextAlign() As HorizAlign
            Get
                Return _TextAlign
            End Get
            Set(ByVal value As HorizAlign)
                _TextAlign = value
                Me.Invalidate()
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
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    CodeGen.AddState(MyState, "HorizAlign", _TextAlign.ToString)
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _TextAlign.ToString)
            End Select
            If Me.Text = String.Empty Then
                MyCodeHead = ""
            End If

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
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
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
                .Replace("[QTEXT]", myQtext)
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

        Private Sub GroupBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(Me.Text)
            End If
        End Sub

        Private Sub GroupBox_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            GroupBox_TextChanged(Nothing, Nothing)
        End Sub

    End Class

End Namespace
