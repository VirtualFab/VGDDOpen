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
    <ToolboxBitmap(GetType(Button), "RadioButtonIco")> _
    Public Class RadioButton : Inherits VGDDWidget

        Private _Checked As Boolean
        Private _HorizAlign As HorizAlign = HorizAlign.Left
        Private _VertAlign As VertAlign = VertAlign.Center
        Private _FirstInGroup As Boolean = False
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("RadioButton")
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

            ps.Width = 3
            ps.Color = _Scheme.Embossdkcolor
            'g.DrawEllipse(ps, rc.Left, rc.Top, rc.Left + rc.Height, rc.Bottom)
            g.DrawArc(ps, rc.Left + 1, rc.Top + 1, rc.Height - 2, rc.Height - 2, 135, 180)
            'g.DrawLine(ps, rc.Left + rc.Height - 2, rc.Bottom - 2, rc.Left + 2, rc.Bottom - 2)

            ps.Color = _Scheme.Embossltcolor
            g.DrawArc(ps, rc.Left + 1, rc.Top + 1, rc.Height - 2, rc.Height - 2, 315, 180)
            'g.DrawLine(ps, rc.Left + 2, rc.Top + 2, rc.Left + rc.Height - 2, rc.Top + 2)

            'Draw Text
            Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)
            Dim intTextSize As SizeF = g.MeasureString(Text, MyBase.Font)
            g.DrawString(Text, MyBase.Font, textBrush, Height + 2, (Height - intTextSize.Height) / 2)
            textBrush.Color = _Scheme.Color0
            g.FillEllipse(textBrush, rc.Left + 2, rc.Top + 2, rc.Left + rc.Height - 4, rc.Bottom - 4)
            If Checked Then
                textBrush.Color = _Scheme.Textcolor0
                Dim rcChecked As New Drawing.Rectangle(rc.Left + rc.Height \ 4, rc.Top + rc.Height \ 4, rc.Left + rc.Height - rc.Height \ 2, rc.Bottom - rc.Height \ 2)
                g.FillEllipse(textBrush, rcChecked)
            End If
        End Sub

#Region "GDDProps"

        <Description("Horizontal Text Alignement of the RadioButton")> _
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

        <Description("Vertical Text Alignement of the RadioButton")> _
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

        <Description("Checked initial state for the RadioButton")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Checked() As Boolean
            Get
                Return _Checked
            End Get
            Set(ByVal value As Boolean)
                _Checked = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Set to true to create a new RadioButton Group")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Main", 2)> _
        Public Overloads Property FirstInGroup() As Boolean
            Get
                Return _FirstInGroup
            End Get
            Set(ByVal value As Boolean)
                _FirstInGroup = value
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
            CodeGen.AddState(MyState, "FirstInGroup", Me.FirstInGroup.ToString)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _HorizAlign.ToString)
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
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[STATE]", MyState) _
                .Replace("[ALIGNMENT]", MyAlignment) _
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

        Private Sub RadioButton_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            If Common.ProjectLoading OrElse Me.Parent Is Nothing Then Exit Sub
            _FirstInGroup = True
            For Each oControl As Control In Me.Parent.Controls
                If TypeOf (oControl) Is RadioButton AndAlso Not oControl Is Me Then
                    Dim oRadioButton As RadioButton = oControl
                    If oRadioButton.FirstInGroup Then
                        _FirstInGroup = False
                        Exit Sub
                    End If
                End If
            Next
        End Sub

        Private Sub RadioButton_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
             If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(Me.Text)
            End If
        End Sub

        Private Sub RadioButton_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            RadioButton_TextChanged(Nothing, Nothing)
        End Sub

    End Class

End Namespace
