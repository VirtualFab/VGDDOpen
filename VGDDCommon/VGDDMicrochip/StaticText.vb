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
    Public Class StaticText : Inherits VGDDWidget

        Private _Frame As EnabledState
        Private _TextAlign As HorizAlign '= HorizAlign.Left
        Private _VertAlign As VertAlign = VertAlign.Top
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1

            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.SetStyle(ControlStyles.Opaque, True)

#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("StaticText")
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Me.RemovePropertyToShow("VertAlign")
            End Select
#End If
            Me.Height = 20
        End Sub

        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim _Cp As CreateParams = MyBase.CreateParams
                'If Not Common.PlayerIsActive Then
                _Cp.ExStyle = _Cp.ExStyle Or &H20
                'End If
                Return _Cp
            End Get
        End Property

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
            If Me.Scheme Is Nothing OrElse Me.Scheme = String.Empty Then
                If Not Common.VGDDIsRunning Then
                    'Me.Scheme = Common.CreateDesignScheme
                Else
                    Exit Sub
                End If
            End If
            If _Hidden Then Exit Sub
            Try

                'MyBase.OnPaintBackground(pevent)

                Dim g As Graphics = pevent.Graphics
                If MyBase.Top < 0 Then
                    MyBase.Top = 0
                End If
                If MyBase.Left < 0 Then
                    MyBase.Left = 0
                End If
                'Me.OnPaintBackground(pevent)

                Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

                ''Impostazione Region
                'Dim Mypath As GraphicsPath = New GraphicsPath
                'Mypath.StartFigure()
                'Mypath.AddRectangle(Me.ClientRectangle)
                'Mypath.CloseFigure()
                'Me.Region = New Region(Mypath)

                'Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
                'g.FillRegion(brushBackGround, Me.Region)

                MyBase.FillBackground(g, _Scheme.Commonbkcolor, Me.Region)

                Dim brushPen As New SolidBrush(_Scheme.Color0)
                If _Frame = EnabledState.Enabled Then
                    Dim ps As Pen = New Pen(brushPen)
                    ps.Width = 2
                    ps.Color = IIf(_State, _Scheme.Embossdkcolor, _Scheme.Colordisabled)
                    g.DrawRectangle(ps, rc.Left, rc.Top, rc.Right, rc.Bottom)
                End If

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
                Select Case _TextAlign
                    Case HorizAlign.Center
                        drawFormat.Alignment = StringAlignment.Center
                        'drawFormat.LineAlignment = StringAlignment.Center
                        g.DrawString(Text, MyBase.Font, textBrush, rc, drawFormat)
                    Case HorizAlign.Right
                        drawFormat.Alignment = StringAlignment.Far
                        g.DrawString(Text, MyBase.Font, textBrush, New Drawing.Rectangle(-MyBase.Font.Size \ 6, rc.Top, rc.Width + MyBase.Font.Size \ 3, rc.Height), drawFormat)
                    Case Else
                        'g.DrawString(Text, MyBase.Font, textBrush, -intTextSize.Height / 10, intTextSize.Height / 20)
                        g.DrawString(Text, MyBase.Font, textBrush, -MyBase.Font.Size \ 6, rc.Top, drawFormat)
                End Select

            Catch ex As Exception
                MyBase.Font = New Font("Arial", 10)
            End Try
        End Sub

#Region "GDDProps"

        <Description("Wether to enable a frame around the StaticText or not")> _
        <DefaultValue(GetType(EnabledState), "Disabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Property Frame() As EnabledState
            Get
                Return _Frame
            End Get
            Set(ByVal value As EnabledState)
                _Frame = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Text Alignement of the StaticText")> _
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

        <Description("Vertical Text Alignement of the StaticText")> _
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
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    CodeGen.AddState(MyState, "HorizAlign", _TextAlign.ToString)
                    CodeGen.AddState(MyState, "VertAlign", _VertAlign.ToString)
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _TextAlign.ToString)
                    CodeGen.AddAlignment(MyAlignment, "Vertical", _VertAlign.ToString)
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

        Private Sub StaticText_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(Me.Text)
            End If
        End Sub

        Private Sub StaticText_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            StaticText_TextChanged(Nothing, Nothing)
        End Sub

        Private Sub StaticText_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
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
