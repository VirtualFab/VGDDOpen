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
    <ToolboxBitmap(GetType(Button), "ListBoxIco")> _
    Public Class ListBox : Inherits VGDDWidget ' Windows.Forms.ListBox
        'Implements ICustomTypeDescriptor
        'Implements IVGDD

        Private adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})

        Private _TextAlign As HorizAlign = HorizAlign.Left
        Private _Items As String()
        Private _SingleSel As Boolean
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
            Me.SetStyle(ControlStyles.UserPaint, True)
            Me.BorderStyle = Windows.Forms.BorderStyle.None
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("ListBox")
#End If
            Me.Size = New Size(150, 150)
            If Common.VGDDIsRunning Then
                Me.RemovePropertyToShow("Anchor")
                Me.RemovePropertyToShow("Text")
            End If
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
            If Me.Scheme Is Nothing OrElse Me.Scheme = String.Empty Then
                If Not Common.VGDDIsRunning Then
                    'Me.Scheme = Common.CreateDesignScheme
                Else
                    Exit Sub
                End If
            End If
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            Me.OnPaintBackground(pevent)

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

            Dim brushPen As New SolidBrush(_Scheme.Embossdkcolor)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 2
            'ps.Color = _Scheme.Embossdkcolor
            g.DrawLine(ps, rc.Left, rc.Top + 1, rc.Right, rc.Top + 1)
            g.DrawLine(ps, rc.Right - 1, rc.Top + 2, rc.Right - 1, rc.Bottom - 2)
            ps.Color = _Scheme.Embossltcolor
            g.DrawLine(ps, rc.Right, rc.Bottom - 1, rc.Left, rc.Bottom - 1)
            g.DrawLine(ps, rc.Left + 1, rc.Bottom - 2, rc.Left + 1, rc.Top + 2)

            Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)
            Dim y As Integer = 0
            ps.Color = IIf(_State, _Scheme.Textcolor0, _Scheme.Colordisabled)
            ps.Width = 1
            ps.DashStyle = DashStyle.Dash
            Dim TextSize As SizeF = g.MeasureString("A", MyBase.Font)
            g.DrawRectangle(ps, 4, y + 4, Me.Width - 16, y + TextSize.Height - 4)
            'If _Items IsNot Nothing Then
            For Each strItem As String In MyBase.Text.Split(vbLf) '_Items
                TextSize = g.MeasureString(strItem, MyBase.Font)
                Select Case _TextAlign
                    Case HorizAlign.Center
                        g.DrawString(strItem, MyBase.Font, textBrush, (Width - TextSize.Width) / 2, y + 4)
                    Case HorizAlign.Right
                        g.DrawString(strItem, MyBase.Font, textBrush, Width - TextSize.Width - 8, y + 4)
                    Case Else
                        g.DrawString(strItem, MyBase.Font, textBrush, 4, y + 4)
                End Select
                y += TextSize.Height
            Next
            'End If
        End Sub

#Region "GDDProps"

        '#If PlayerMonolitico Then
        '        Public Shadows Property Text() As String
        '#Else
        Public Overloads Property Text() As String
            '#End If
            Get
                'MyBase.Text = String.Empty
                'If Me.Items IsNot Nothing Then
                '    For Each strItem As String In Me.Items
                '        MyBase.Text &= vbLf & strItem
                '    Next
                'End If
                'If MyBase.Text.StartsWith(vbLf) Then MyBase.Text = MyBase.Text.Substring(1)
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                MyBase.Text = value
                Me.Items = MyBase.Text.Split(vbLf)
                Me.Invalidate()
            End Set
        End Property

        <Description("Items collection of the ListBox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property Items() As String()
            Get
                Return _Items
            End Get
            Set(ByVal value As String())
                _Items = value
                Dim strText As String = String.Empty
                If Me.Items IsNot Nothing Then
                    For Each strItem As String In Me.Items
                        strText &= vbLf & strItem.Replace(vbCr, "").Replace(vbLf, "")
                    Next
                End If
                If strText.StartsWith(vbLf) Then strText = strText.Substring(1)
                MyBase.Text = strText
                Me.Invalidate()
            End Set
        End Property

        <Description("Single Item Selection")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Overloads Property SingleSel() As Boolean
            Get
                Return _SingleSel
            End Get
            Set(ByVal value As Boolean)
                _SingleSel = value
            End Set
        End Property

        <Description("Text Alignement of the ListBox")> _
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
            CodeGen.AddState(MyState, "SingleSel", Me.SingleSel.ToString)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    CodeGen.AddState(MyState, "HorizAlign", _TextAlign.ToString)
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _TextAlign.ToString)
            End Select
            Dim myText As String = ""
            Dim myQtext As String = ""
            If Me.Items IsNot Nothing Then
                For Each strItem As String In Me.Items
                    strItem = strItem.Replace(vbCr, "").Replace(vbLf, "")
                    myQtext &= vbLf & strItem
                    myText &= "," & strItem
                Next
            End If

            If myQtext.Length > 0 Then
                myQtext = myQtext.Substring(1)
                myText = myText.Substring(1)
            End If
            myQtext = myQtext.PadRight(GetMaxTextLength(Me.TextStringID), "_") ' DW
            myText = myText.PadRight(GetMaxTextLength(Me.TextStringID), "_") ' DW
            myQtext = CodeGen.QText(myQtext, Me._Scheme.Font, Nothing).Replace(vbLf, "\n")

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[ALIGNMENT]", MyAlignment) _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[BITMAP]", "NULL").Replace("[SCHEME]", Me.Scheme))

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
            If myText <> String.Empty Then
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

        Private Sub ListBox_StyleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.StyleChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                If _Scheme.Font.SmartCharSet Then
                    _Scheme.Font.SmartCharSetAddString(Me.Text)
                End If
            End If
        End Sub

    End Class
End Namespace
