Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(MsgBox), "MsgBox.ico")> _
    Public Class MsgBox : Inherits VGDDWidget

        Private _BitmapNameMessage As String = String.Empty
        Protected _ImageMessage As VGDDImage
        Private _BitmapNameCaption As String = String.Empty
        Protected _ImageCaption As VGDDImage
        'Private _TextMessage As String = String.Empty
        Private _TextStringID2 As Integer = 0
        Private _TextCaption As String = String.Empty
        Private _SchemeButtons As VGDDScheme
        Private _BitmapReleasedKey As String = String.Empty
        Protected _ImageReleasedKey As VGDDImage
        Private _BitmapPressedKey As String = String.Empty
        Protected _ImagePressedKey As VGDDImage
        Private _ButtonsToDisplay As MSGBOX_BUTTONS
        Private _Radius As Integer = 12
        Private _HorizAlign As HorizAlign = HorizAlign.Left
        Private _VertAlign As VertAlign = VertAlign.Top

        Private Shared _Instances As Integer = 0

        Private intBtnTop As Int16

        Public Enum MSGBOX_BUTTONS
            BTN_OK
            BTN_YES_NO
            BTN_YES_NO_CANCEL
        End Enum

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("MsgBox")
#End If
            Me.Size = New Size(300, 150)
            CaptionText = "Caption"
            MessageText = "Message row 1" & vbCrLf & "Message row 2" & vbCrLf & "Message row 3"
            RemovePropertyToShow("Text")
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

        Public Sub InitializeButtons()
            Dim intBtnWidth As Int16, intBtnHeight As Int16, intBtnRight As Int16
            Const BTNGAP As Int16 = 20
            If _SchemeButtons Is Nothing Then
                If _Scheme Is Nothing Then Exit Sub
                _SchemeButtons = _Scheme
            End If
            If _BitmapReleasedKey = String.Empty Then
                intBtnHeight = _SchemeButtons.Font.Font.Height + BTNGAP
            Else
                intBtnHeight = _ImageReleasedKey.Bitmap.Height
            End If
            intBtnWidth = Me.Width / 5
            intBtnRight = Me.Width - BTNGAP
            intBtnTop = Me.Height - BTNGAP - intBtnHeight

            Dim oButton As VGDDMicrochip.Button
            Me.Controls.Clear()

            Select Case _ButtonsToDisplay
                Case MSGBOX_BUTTONS.BTN_OK
                    oButton = New VGDDMicrochip.Button
                    With oButton
                        .Left = intBtnRight - intBtnWidth
                        .Top = intBtnTop
                        .Width = intBtnWidth
                        .Height = intBtnHeight
                        .Radius = Radius
                        .Bitmap = _BitmapReleasedKey
                        .Text = "OK"
                        .Scheme = SchemeButtons
                    End With
                    Me.Controls.Add(oButton)
                Case MSGBOX_BUTTONS.BTN_YES_NO, MSGBOX_BUTTONS.BTN_YES_NO_CANCEL
                    If _ButtonsToDisplay = MSGBOX_BUTTONS.BTN_YES_NO_CANCEL Then
                        '"Cancel" Button
                        oButton = New VGDDMicrochip.Button
                        With oButton
                            .Left = intBtnRight - intBtnWidth
                            .Top = intBtnTop
                            .Width = intBtnWidth
                            .Height = intBtnHeight
                            .Radius = Radius
                            .Bitmap = _BitmapReleasedKey
                            .Text = "Cancel"
                            .Scheme = SchemeButtons
                        End With
                        Me.Controls.Add(oButton)
                        intBtnRight -= (intBtnWidth + BTNGAP) 'Compute next button right coordinate
                    End If

                    '"No" Button
                    oButton = New VGDDMicrochip.Button
                    With oButton
                        .Left = intBtnRight - intBtnWidth
                        .Top = intBtnTop
                        .Width = intBtnWidth
                        .Height = intBtnHeight
                        .Radius = Radius
                        .Bitmap = _BitmapReleasedKey
                        .Text = "No"
                        .Scheme = SchemeButtons
                    End With
                    Me.Controls.Add(oButton)
                    intBtnRight -= (intBtnWidth + BTNGAP) 'Compute next button right coordinate

                    '"Yes" Button
                    oButton = New VGDDMicrochip.Button
                    With oButton
                        .Left = intBtnRight - intBtnWidth
                        .Top = intBtnTop
                        .Width = intBtnWidth
                        .Height = intBtnHeight
                        .Radius = Radius
                        .Bitmap = _BitmapReleasedKey
                        .Text = "Yes"
                        .Scheme = SchemeButtons
                    End With
                    Me.Controls.Add(oButton)
            End Select
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

            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            'Me.OnPaintBackground(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle
            Dim Rad2 As Integer = _Radius

            'Impostazione Region
            Dim Mypath As GraphicsPath = New GraphicsPath
            Mypath.StartFigure()
            Mypath.AddLine(rc.Left + Rad2, rc.Top, rc.Right - Rad2, rc.Top)
            If Radius > 0 Then
                Mypath.AddArc(rc.Right - _Radius * 2, rc.Top, _Radius * 2, _Radius * 2, 270, 90)
            End If
            Mypath.AddLine(rc.Right, rc.Top + Rad2, rc.Right, rc.Bottom - Radius)
            If Radius > 0 Then
                Mypath.AddArc(rc.Right - _Radius * 2, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 0, 90)
            End If
            Mypath.AddLine(rc.Right - Rad2, rc.Bottom, rc.Left + _Radius * 2, rc.Bottom)
            If Radius > 0 Then
                Mypath.AddArc(rc.Left, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 90, 90)
            End If
            Mypath.AddLine(rc.Left, rc.Bottom - Rad2, rc.Left, rc.Top + _Radius * 2)
            If Radius > 0 Then
                Mypath.AddArc(rc.Left, rc.Top, _Radius * 2, _Radius * 2, 180, 90)
            End If
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)

            Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
            g.FillRegion(brushBackGround, Me.Region)

            'Draw MsgBox
            Dim brushPen As New SolidBrush(_Scheme.Textcolor0)
            Dim ps As Pen = New Pen(brushPen)

            Dim intCaptionHeight As Int16 = 0
            Dim faceClr1 As Color, embossDkClr As Color, embossLtClr As Color, txtColor As Color
            faceClr1 = _Scheme.Colordisabled
            embossDkClr = _Scheme.Embossdkcolor
            embossLtClr = _Scheme.Embossltcolor
            txtColor = IIf(Me.Enabled, _Scheme.Textcolor0, _Scheme.Textcolordisabled)
            ps.Width = 2
            ps.Color = embossLtClr
            g.DrawLine(ps, rc.Left + _Radius, rc.Top + 1, rc.Right - _Radius, rc.Top + 1)
            ps.Color = embossDkClr
            If Radius > 0 Then
                ps.Width = 4
                g.DrawArc(ps, rc.Right - _Radius * 2, rc.Top, _Radius * 2, _Radius * 2, 270, 90)
                ps.Width = 2
            End If
            g.DrawLine(ps, rc.Right - 1, rc.Top + _Radius - 1, rc.Right - 1, rc.Bottom - _Radius + 1)
            If Radius > 0 Then
                ps.Width = 4
                g.DrawArc(ps, rc.Right - _Radius * 2, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 0, 90)
                ps.Width = 2
            End If
            g.DrawLine(ps, rc.Right - _Radius + 1, rc.Bottom - 1, rc.Left + _Radius - 1, rc.Bottom - 1)
            ps.Color = embossLtClr
            If Radius > 0 Then
                ps.Width = 4
                g.DrawArc(ps, rc.Left, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 90, 90)
                ps.Width = 2
            End If
            g.DrawLine(ps, rc.Left + 1, rc.Bottom - _Radius + 1, rc.Left + 1, rc.Top + _Radius - 1)
            If Radius > 0 Then
                ps.Width = 4
                g.DrawArc(ps, rc.Left, rc.Top, _Radius * 2, _Radius * 2, 180, 90)
                ps.Width = 2
            End If
            Dim tx As Integer, ty As Integer

            'Draw Caption
            Dim textBrush As SolidBrush = New SolidBrush(txtColor)
            If _TextCaption <> String.Empty Then
                'Caption text
                If _ImageCaption IsNot Nothing AndAlso _ImageCaption.Bitmap IsNot Nothing Then
                    intCaptionHeight = _ImageCaption.Bitmap.Height + 4
                Else
                    intCaptionHeight = g.MeasureString(_TextCaption, _Scheme.Font.Font).Height
                End If
                brushBackGround = New SolidBrush(_Scheme.Color1)
                g.FillRectangle(brushBackGround, rc.Left, rc.Top, rc.Width, intCaptionHeight)
                tx = rc.Left + 2 + Rad2
                If _ImageCaption IsNot Nothing AndAlso _ImageCaption.Bitmap IsNot Nothing Then
                    'Caption bitmap
                    g.DrawImage(_ImageCaption.Bitmap, tx, rc.Top + 2)
                    tx += _ImageCaption.Bitmap.Width + 10
                End If
                g.DrawString(_TextCaption, _Scheme.Font.Font, textBrush, tx, rc.Top + 2)
            End If

            Dim intTextSize As SizeF
            Try
                intTextSize = g.MeasureString(Me.MessageText, _Scheme.Font.Font)
                intTextSize.Width += 2
            Catch ex As Exception
                intTextSize = New SizeF(10, 18)
            End Try

            Dim intImageWidth As Integer = 0
            'Draw message icon
            If _ImageMessage IsNot Nothing Then
                intImageWidth = _ImageMessage.Bitmap.Width
                Select Case _HorizAlign
                    Case Common.HorizAlign.Left, HorizAlign.Center
                        tx = Me.Width - intImageWidth - 20
                    Case Common.HorizAlign.Right
                        tx = 20
                End Select
                g.DrawImage(_ImageMessage.Bitmap, tx, intCaptionHeight + 20) ' (Me.Height - intCaptionHeight - _ImageMessage.Bitmap.Height) \ 2)
            End If

            'Draw Text 
            Dim format As StringFormat = New StringFormat
            Select Case _VertAlign
                Case VertAlign.Center
                    ty = (Height - intTextSize.Height - intCaptionHeight) / 2
                Case VertAlign.Bottom
                    ty = (intBtnTop - intTextSize.Height)
                Case VertAlign.Top
                    ty = intCaptionHeight + 5
            End Select
            Select Case _HorizAlign
                Case HorizAlign.Center
                    tx = (Width - intTextSize.Width) / 2 '+ 20
                    format.LineAlignment = StringAlignment.Center
                    format.Alignment = StringAlignment.Center
                Case HorizAlign.Right
                    tx = Width - intTextSize.Width - 5
                    format.LineAlignment = StringAlignment.Far
                    format.Alignment = StringAlignment.Far
                Case HorizAlign.Left
                    tx = 5
                    format.LineAlignment = StringAlignment.Near
                    format.Alignment = StringAlignment.Near
            End Select
            Try
                Dim lr As New System.Drawing.Rectangle(tx, ty, intTextSize.Width, intTextSize.Height)
                g.DrawString(MyBase.Text, _Scheme.Font.Font, textBrush, lr, format)
                'g.DrawRectangle(Pens.Black, lr)
            Catch ex As Exception
            End Try

            'If _ButtonsScheme Is Nothing Then
            '    _ButtonsScheme = _Scheme
            'End If
        End Sub

#Region "GDDProps"

        <Description("Bitmap name to draw for the main message")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("MsgBox")> _
        Public Overridable Property BitmapNameCaption() As String
            Get
                Return _BitmapNameCaption
            End Get
            Set(ByVal value As String)
                _BitmapNameCaption = value
                If Not Me.IsLoading Then
                    SetBitmapName(_BitmapNameCaption, _ImageCaption)
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Bitmap name to draw for the main message")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("MsgBox")> _
        Public Overridable Property BitmapNameMessage() As String
            Get
                Return _BitmapNameMessage
            End Get
            Set(ByVal value As String)
                _BitmapNameMessage = value
                If Not Me.IsLoading Then
                    SetBitmapName(_BitmapNameMessage, _ImageMessage)
                End If
                Me.Invalidate()
            End Set
        End Property

        Public Sub SetBitmapName(ByVal BitmapName As String, ByRef VGDDImage As VGDDImage)
            Dim OldImage As VGDDImage = VGDDImage
            If BitmapName = String.Empty Then
                If OldImage IsNot Nothing Then
                    OldImage.RemoveUsedBy(Me)
                End If
                VGDDImage = Nothing
            Else
                VGDDImage = GetBitmap(BitmapName)
                If VGDDImage Is Nothing OrElse VGDDImage.Bitmap Is Nothing Then
                    VGDDImage = New VGDDImage
                    VGDDImage.TransparentBitmap = VGDDImage.InvalidBitmap(Nothing, Me.Width, Me.Height)
                    VGDDImage._GraphicsPath = Nothing
                    If OldImage IsNot Nothing Then OldImage.RemoveUsedBy(Me)
                Else
                    If Not VGDDImage.AllowScaling Then
                        If OldImage Is Nothing Then
                            Me.Size = VGDDImage.OrigBitmap.Size
                        End If
                        VGDDImage.ScaleBitmap()
                    Else
                        ScaleImage()
                    End If
                    If OldImage IsNot Nothing AndAlso Not OldImage Is VGDDImage Then
                        OldImage.RemoveUsedBy(Me)
                        VGDDImage.AddUsedBy(Me)
                    End If
                    VGDDImage.AddUsedBy(Me)
                End If
            End If
            Me.Invalidate()
        End Sub

        Private Sub CheckTextChanged()
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing _
                AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION _
                AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(MyBase.Text & CaptionText & "OKYesNoCancel")
            End If
        End Sub

#If Not PlayerMonolitico Then
        <Description("Text of the message")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(UiEditTextMultiLanguage), GetType(Drawing.Design.UITypeEditor))> _
        <Category("MsgBox")> _
        Public Property MessageText() As String
#Else
        Public Property MessageText() As String
#End If
            Get
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                MyBase.Text = value
                CheckTextChanged()
                Me.Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
        Public Property TextStringID2 As Integer
            Get
                Return _TextStringID2
            End Get
            Set(ByVal value As Integer)
                MyBase.SetTextStringId(value, _TextStringID2, _TextCaption)
                CheckTextChanged()
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Text for the caption")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <AttributeTextStringIDName("TextStringID2")> _
        <Editor(GetType(UiEditTextMultiLanguage), GetType(Drawing.Design.UITypeEditor))> _
        <Category("MsgBox")> _
        Public Property CaptionText() As String
#Else
        Public Property CaptionText() As String
#End If
            Get
                Return MyBase.GetText(_TextStringID2, _TextCaption)
            End Get
            Set(ByVal value As String)
                If MyBase.SchemeObj Is Nothing Then
                    MyBase.CheckStringID(value, _TextStringID2, Common.GetScheme("").Font.Name)
                Else
                    MyBase.CheckStringID(value, _TextStringID2, MyBase.SchemeObj.Font.Name)
                End If
                MyBase.SetText(value, _TextStringID2, _TextCaption)
                CheckTextChanged()
                Me.Invalidate()
            End Set
        End Property

        <Description("Horizontal Text Alignement of the button")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Left")> _
        <Category("MsgBox")> _
        Public Property HorizAlign() As HorizAlign
            Get
                Return _HorizAlign
            End Get
            Set(ByVal value As HorizAlign)
                _HorizAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Vertical Text Alignement of the button")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(VertAlign), "Top")> _
        <Category("MsgBox")> _
        Public Property VertAlign() As VertAlign
            Get
                Return _VertAlign
            End Get
            Set(ByVal value As VertAlign)
                _VertAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Buttons to display in the MsgBox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("MsgBox")> _
        Public Property ButtonsToDisplay As MSGBOX_BUTTONS
            Get
                Return _ButtonsToDisplay
            End Get
            Set(ByVal value As MSGBOX_BUTTONS)
                _ButtonsToDisplay = value
                InitializeButtons()
                Me.Invalidate()
            End Set
        End Property

        <Description("GOL Scheme to use for the buttons (optional)")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <TypeConverter(GetType(Common.SchemesOptionConverter))> _
        <Category("MsgBox")> _
        Public Property SchemeButtons As String
            Get
                If _SchemeButtons IsNot Nothing Then
                    Return _SchemeButtons.Name
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If _SchemeButtons IsNot Nothing Then
                    _SchemeButtons.RemoveUsedBy(Me)
                    If _SchemeButtons.Font IsNot Nothing Then
                        _SchemeButtons.Font.RemoveUsedBy(Me)
                    End If
                End If
                If value Is Nothing Then
                    _SchemeButtons = Nothing
                    Exit Property
                End If
                Dim SetScheme As VGDDScheme = GetScheme(value)
                If SetScheme Is Nothing Then
                    SetScheme = GetScheme("")
                End If
                If SetScheme IsNot Nothing AndAlso (_SchemeButtons Is Nothing OrElse value <> _SchemeButtons.Name) Then
                    _SchemeButtons = SetScheme
                    SetScheme.AddUsedBy(Me)
                    SetScheme.Font.AddUsedBy(Me)
                    InitializeButtons()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Define the radius roundness for the buttons")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 256)> _
        <DefaultValue(12)> _
        <Category("MsgBox")> _
        Property Radius() As Integer
            Get
                Return _Radius
            End Get
            Set(ByVal value As Integer)
                _Radius = value
                InitializeButtons()
                Me.Invalidate()
            End Set
        End Property

#End Region

#Region "VGDDCode"

#If Not PlayerMonolitico Then
        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = String.Empty
            Dim MyCodeHead1 As String = String.Empty
            Dim MyCodeHead2 As String = String.Empty

            Dim MyCode As String = "", MyState As String = ""

            Dim MyClassName As String = Me.GetType.ToString

            If Common.ProjectMultiLanguageTranslations > 0 Then
                If MyBase.TextStringID < 0 Then
                    CodeGen.Errors &= MyControlId & " has empty text ID" & vbCrLf
                End If
            Else
                MyCodeHead1 = CodeGen.TextDeclareCodeHeadTemplate(_CDeclType).Replace("[CHARMAX]", "")
                MyCodeHead2 = CodeGen.TextDeclareCodeHeadTemplate(_CDeclType).Replace("[CHARMAX]", "") _
                                        .Replace("_Text", "_CaptionText") _
                                        .Replace("[TEXT]", "[TEXT2]") _
                                        .Replace("[QTEXT]", "[QTEXT2]")
            End If
            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            If Me.MessageText = String.Empty Then
                MyCode = MyCode.Replace("(XCHAR*)[CONTROLID]_Text", "NULL")
                MyCodeHead1 = ""
            End If
            If Me.CaptionText = String.Empty Then
                MyCode = MyCode.Replace("(XCHAR*)[CONTROLID]_CaptionText", "NULL")
                MyCodeHead2 = ""
            End If
            CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            CodeGen.AddLines(MyCodeHead, MyCodeHead1)
            CodeGen.AddLines(MyCodeHead, MyCodeHead2)

            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "HorizAlign", Me.HorizAlign.ToString)
            CodeGen.AddState(MyState, "VertAlign", Me.VertAlign.ToString)

            Dim myText As String = ""
            MyBase.Text = Me.Text.PadRight(GetMaxTextLength(MyBase.TextStringID), "_") ' DW
            Dim myQtext As String = CodeGen.QText(MyBase.Text, _Scheme.Font, myText)
            Dim myCaption As String = ""
            Dim myQCaption As String = CodeGen.QText(_TextCaption, _Scheme.Font, myCaption)

            If _SchemeButtons Is Nothing Then
                _SchemeButtons = _Scheme
            End If

            CodeGen.AddLines(CodeGen.Code, MyCode _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[CAPTIONTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode.Replace("_Text", "_CaptionText"))) _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[RADIUS]", _Radius) _
                .Replace("[BUTTONSTODISPLAY]", _ButtonsToDisplay.ToString) _
                .Replace("[BITMAP]", IIf(_BitmapNameMessage = String.Empty, "NULL", "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & _BitmapNameMessage)) _
                .Replace("[CAPTIONBITMAP]", IIf(_BitmapNameCaption = String.Empty, "NULL", "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & _BitmapNameCaption)) _
                .Replace("[RELEASEDKEY_BITMAP]", IIf(_BitmapReleasedKey = String.Empty, "NULL", "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & _BitmapReleasedKey)) _
                .Replace("[PRESSEDKEY_BITMAP]", IIf(_BitmapPressedKey = String.Empty, "NULL", "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & _BitmapPressedKey)) _
                .Replace("[SCHEME]", _Scheme.Name) _
                .Replace("[BUTTONS_SCHEME]", IIf(_SchemeButtons Is Nothing, "NULL", _SchemeButtons.Name)) _
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
                .Replace("[CAPTIONTEXT]", myCaption) _
                .Replace("[QTEXT2]", myQCaption)
            If Not CodeGen.HeadersIncludes.Contains(CodeGen.HeadersIncludesTemplate) Then ' #include "msgbox.h"
                CodeGen.AddLines(CodeGen.HeadersIncludes, CodeGen.HeadersIncludesTemplate)
            End If
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
            If Me._TextCaption <> String.Empty Then
                CodeGen.AddLines(MyHeaders, CodeGen.MyHeader(_CDeclType).Replace("_Text", "_CaptionText").Replace("[STRINGID]", "[STRINGID2]"))
            End If
            CodeGen.AddLines(MyHeaders, CodeGen.TextDeclareHeaderTemplate(_CDeclType))
            CodeGen.AddLines(MyHeaders, CodeGen.HeadersTemplate)
            CodeGen.AddLines(CodeGen.Headers, MyHeaders _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(_TextStringID)) _
                .Replace("[STRINGID2]", CodeGen.StringPoolIndex(_TextStringID2)) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[TEXT2]", myCaption) _
                .Replace("[QTEXT2]", myQCaption) _
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
#End Region

        Private Sub MsgBox_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            SetBitmapName(_BitmapNameMessage, _ImageMessage)
            'CheckStringID(_TextCaption, _TextStringID2)
        End Sub

        Private Sub MsgBox_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            MyBase.StringsPoolSetUsedBy(_TextStringID2, StringsPoolSetUsedByAction.Add)
        End Sub

        Private Sub MsgBox_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            InitializeButtons()
            ScaleImage()
        End Sub

        Private Sub ScaleImage()
            If _ImageMessage Is Nothing Then Exit Sub
            If Me.Width < Me.Height Then
                _ImageMessage.ScaleBitmap(Me.Width \ 4, Me.Width \ 4, 1)
            Else
                _ImageMessage.ScaleBitmap(Me.Height \ 4, Me.Height \ 4, 1)
            End If
        End Sub
    End Class

End Namespace

