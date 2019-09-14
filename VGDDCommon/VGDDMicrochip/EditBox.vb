Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common
Imports System.Drawing.Design

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "StaticTextIco")> _
    Public Class EditBox : Inherits Statictext

        Public Sub New()
            MyBase.New()
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("EditBox")
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Me.RemovePropertyToShow("VertAlign")
            End Select
#End If
            Me.Height = 20
            MyBase.Frame = EnabledState.Enabled
            MyBase.CDeclType = TextCDeclType.RamXcharArray
            RemovePropertyToShow("Frame")
            RemovePropertyToShow("CDeclType")
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
            Try
                Dim g As Graphics = pevent.Graphics
                If MyBase.Top < 0 Then
                    MyBase.Top = 0
                End If
                If MyBase.Left < 0 Then
                    MyBase.Left = 0
                End If

                Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

                ''Impostazione Region
                'Dim Mypath As GraphicsPath = New GraphicsPath
                'Mypath.StartFigure()
                'Mypath.AddRectangle(Me.ClientRectangle)
                'Mypath.CloseFigure()
                'Me.Region = New Region(Mypath)

                'Dim brushBackGround As New SolidBrush(_Scheme.Color0)
                'g.FillRegion(brushBackGround, Me.Region)

                MyBase.FillBackground(g, _Scheme.Color0, Me.Region)

                Dim brushPen As New SolidBrush(_Scheme.Color0)
                Dim ps As Pen = New Pen(brushPen)

                ps.Width = 2

                ps.Color = _Scheme.Embossdkcolor
                g.DrawLine(ps, rc.Left, rc.Top + 1, rc.Right, rc.Top + 1)
                ps.Color = _Scheme.Embossltcolor
                g.DrawLine(ps, rc.Right - 1, rc.Top - 1, rc.Right - 1, rc.Bottom + 1)
                g.DrawLine(ps, rc.Right + 1, rc.Bottom - 1, rc.Left - 1, rc.Bottom - 1)
                ps.Color = _Scheme.Embossdkcolor
                g.DrawLine(ps, rc.Left + 1, rc.Bottom + 1, rc.Left + 1, rc.Top - 1)

                'Draw Text
                Dim textBrush As SolidBrush = New SolidBrush(IIf(_State, _Scheme.Textcolor0, _Scheme.Colordisabled))
                'Dim intTextSize As SizeF = New Size(0, 0)
                'Try
                '    intTextSize = g.MeasureString(Text, MyBase.Font)

                'Catch ex As Exception
                '    intTextSize = New Size(16, 16)
                'End Try
                Dim drawFormat As New StringFormat
                drawFormat.FormatFlags = StringFormatFlags.NoWrap Or StringFormatFlags.NoClip 'StringFormatFlags.FitBlackBox '
                Select Case MyBase.TextAlign
                    Case HorizAlign.Center
                        drawFormat.Alignment = StringAlignment.Center
                        'drawFormat.LineAlignment = StringAlignment.Center
                        g.DrawString(Text, MyBase.Font, textBrush, rc, drawFormat)
                    Case HorizAlign.Right
                        drawFormat.Alignment = StringAlignment.Far
                        g.DrawString(Text, MyBase.Font, textBrush, New Drawing.Rectangle(rc.Left + 4, rc.Top, rc.Width - 8, rc.Height), drawFormat)
                    Case Else
                        'g.DrawString(Text, MyBase.Font, textBrush, -intTextSize.Height / 10, intTextSize.Height / 20)
                        g.DrawString(Text, MyBase.Font, textBrush, -MyBase.Font.Size \ 6 + 4, rc.Top, drawFormat)
                End Select

            Catch ex As Exception

            End Try
        End Sub

#Region "GDDProps"

        Private _CharMax As Integer = 16
        <Description("Maximum characters number accepted by the EditBox")> _
        <CustomSortedCategory("CodeGen", 6)> _
        Property Charmax() As Integer
            Get
                Return _CharMax
            End Get
            Set(ByVal value As Integer)
                _CharMax = value
                Me.Invalidate()
            End Set
        End Property

        Private _Caret As Boolean = False
        <Description("Show caret?")> _
        <CustomSortedCategory("Appearance", 4)> _
        Property Caret() As Boolean
            Get
                Return _Caret
            End Get
            Set(ByVal value As Boolean)
                _Caret = value
            End Set
        End Property
#End Region

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.TextDeclareCodeHeadTemplate(_CDeclType).Replace("[CHARMAX]", _CharMax + 1)
            Dim MyCode As String = String.Empty, MyState As String = String.Empty, MyAlignment As String = String.Empty
            Dim MyEventCode As String = ""

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
                    CodeGen.AddState(MyState, "TextAlign", Me.TextAlign.ToString)
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", Me.TextAlign.ToString)
                    CodeGen.AddAlignment(MyAlignment, "Vertical", Me.VertAlign.ToString)
            End Select
            CodeGen.AddState(MyState, "Caret", _Caret)
            'If Me.Text = String.Empty Then
            '    MyCodeHead = ""
            'End If

            Dim myText As String = ""
            Dim myQtext As String = Me.Text
            If myQtext <> String.Empty Then
                myQtext = myQtext.Substring(0, Math.Min(myQtext.Length, _CharMax - 1))
            End If
            myQtext = CodeGen.QText(myQtext, Me._Scheme.Font, myText)

            If Common.ProjectMultiLanguageTranslations > 0 AndAlso MyBase.TextStringID < 0 Then
                CodeGen.Errors &= MyControlId & " has empty text ID" & vbCrLf
            End If
            If Me.Text.Length > _CharMax Then
                CodeGen.Errors &= String.Format("{0} has text legth={1} but MAXLEN={2}. Truncating text.", MyControlId, Me.Text.Length, _CharMax) & vbCrLf
            End If

            CodeGen.AddLines(CodeGen.Code, MyCode _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[CHARMAX]", _CharMax) _
                .Replace("[SCHEME]", Me.Scheme) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[ALIGNMENT]", MyAlignment) _
                .Replace("[SCHEME]", Me.Scheme) _
                .Replace("[CHARMAX]", Charmax))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[CHARMAX]", _CharMax) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            Dim MyHeaders As String = String.Empty
            If Me.Public Then
                CodeGen.AddLines(MyHeaders, "extern " & CodeGen.ConstructorTemplate.Trim)
            End If
            CodeGen.AddLines(MyHeaders, CodeGen.MyHeader(_CDeclType))
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

        Private Sub EditBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(Me.Text)
            End If
        End Sub

        Private Sub EditBox_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            EditBox_TextChanged(Nothing, Nothing)
        End Sub

        Private Sub EditBox_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
            If Not Me.IsLoading Then
                Try
                    Me.Height = Me.Font.Height
                    Application.DoEvents()
                Catch ex As Exception

                End Try
            End If
        End Sub
    End Class

End Namespace
