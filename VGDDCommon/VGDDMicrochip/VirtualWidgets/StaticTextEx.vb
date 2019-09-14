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
    <ToolboxBitmap(GetType(Button), "StaticTextExIco")> _
    Public Class StaticTextEx : Inherits VGDDWidget

        Friend _Frame As EnabledState
        Private _NoPanel As EnabledState
        Friend _TextAlign As HorizAlign
        Private blnRepaint As Boolean = False

        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()

            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.SetStyle(ControlStyles.Opaque, True)

            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("StaticTextEx")
#End If
            'If _Schemes.Count > 0 Then
            '    Dim oFont As VGDDFont = CType(_Schemes("New"), VGDDScheme).Font
            '    If oFont IsNot Nothing Then
            '        Me.Height = oFont.Font.Height
            '    End If
            'End If
            Me.Height = 20
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

        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim _Cp As CreateParams = MyBase.CreateParams
                'If Not Common.PlayerIsActive Then
                _Cp.ExStyle = _Cp.ExStyle Or &H20
                'End If
                Return _Cp
            End Get
        End Property

        Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
            If blnRepaint OrElse _NoPanel = EnabledState.Disabled Then
                MyBase.OnPaintBackground(pevent)
                blnRepaint = False
            End If
        End Sub

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
                'Me.OnPaintBackground(pevent)

                Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

                ''Impostazione Region
                'If _NoPanel = EnabledState.Disabled Then
                '    'Dim Mypath As GraphicsPath = New GraphicsPath
                '    'Mypath.StartFigure()
                '    'Mypath.AddRectangle(Me.ClientRectangle)
                '    'Mypath.CloseFigure()
                '    'Me.Region = New Region(Mypath)

                '    Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
                '    g.FillRectangle(brushBackGround, Me.ClientRectangle)
                '    'Else
                '    '    'Me.Region = New Region()
                'End If

                If _NoPanel <> EnabledState.Enabled Then
                    MyBase.FillBackground(g, _Scheme.Commonbkcolor, Me.Region)
                End If

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

        Public Overloads Sub Invalidate()
            blnRepaint = True
            MyBase.Invalidate()
        End Sub

        Public Overloads Property Text As String
            Get
                Return MyBase.Text
            End Get
            Set(value As String)
                MyBase.Text = value
                Me.Invalidate()
            End Set
        End Property



#Region "GDDProps"

        <Description("Wether to enable the background panel or not")> _
        <DefaultValue(GetType(EnabledState), "Disabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Property NoPanel() As EnabledState
            Get
                Return _NoPanel
            End Get
            Set(ByVal value As EnabledState)
                _NoPanel = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Wether to enable a frame around the StaticTextEx or not")> _
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

        <Description("Text Alignement of the StaticTextEx")> _
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
            Dim MyCode As String = "", MyState As String = ""
            Dim MyClassName As String = Me.GetType.ToString

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
            CodeGen.AddState(MyState, "HorizAlign", Me.TextAlign.ToString)
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)
            CodeGen.AddState(MyState, "NoPanel", Me.NoPanel.ToString)
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
            If Not CodeGen.HeadersIncludes.Contains(CodeGen.HeadersIncludesTemplate) Then ' #include "statictext_ex.h"
                CodeGen.AddLines(CodeGen.HeadersIncludes, CodeGen.HeadersIncludesTemplate)
            End If
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            'If Me.Parent.Name = "Program_Time_Temp" AndAlso Me.Name = "StaticTextEx1" Then
            '    Debug.Print("")
            'End If

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

            Try
                'Dim myAssembly As Reflection.Assembly = System.Reflection.Assembly.GetAssembly(Me.GetType)
                For Each oFolderNode As Xml.XmlNode In CodeGen.XmlTemplatesDoc.SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Project/*", MyClassName.Split(".")(1)))
                    MplabX.AddFile(oFolderNode)
                Next
            Catch ex As Exception
            End Try
        End Sub
#End If

        Private Sub StaticTextEx_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                If _Scheme.Font.SmartCharSet Then
                    _Scheme.Font.SmartCharSetAddString(Me.Text)
                End If
            End If
        End Sub

        Private Sub StaticTextEx_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
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
