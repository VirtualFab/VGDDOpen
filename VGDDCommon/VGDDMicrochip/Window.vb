Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports System.IO
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "WindowIco")> _
    Public Class Window : Inherits VGDDWidgetWithBitmap
        Private _TitleAlign As HorizAlign
        Private _VertAlign As VertAlign = VertAlign.Center
        Private Docked As Boolean = False
        Public Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
            Me.Width = 0
            Me.Height = 42
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Window")
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Me.RemovePropertyToShow("VertAlign")
            End Select
#End If
            MyBase.AddPropertyToShow("Bitmap")
            MyBase._BitmapNeeded = False
            MyBase._VGDDImage = Nothing
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
            Me.OnPaintBackground(pevent)

            Dim oLockedProp As PropertyDescriptor = TypeDescriptor.GetProperties(Me)("Locked")
            If oLockedProp IsNot Nothing Then oLockedProp.SetValue(Me, True)

            If Not Docked OrElse Me.Top <> 0 OrElse Me.Left <> 0 OrElse Me.Width <> Me.Parent.Width Then
                Docked = True
                Me.Top = 0
                Me.Left = 0
                Me.Height = 42
                Me.Width = Me.Parent.Width
            End If
            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

            'Impostazione Region
            Dim Mypath As GraphicsPath = New GraphicsPath
            Mypath.StartFigure()
            Mypath.AddRectangle(Me.ClientRectangle)
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)
            If _Scheme Is Nothing Then Exit Sub
            Dim brushBackGround As New SolidBrush(_Scheme.Color0)
            g.FillRegion(brushBackGround, Me.Region)

            If _VGDDImage IsNot Nothing AndAlso _VGDDImage.Bitmap IsNot Nothing Then
                'Dim b As Image = VGDDCommonGetBipmap(_Bitmap)
                g.DrawImage(_VGDDImage.Bitmap, 2, 0) ', _VGDDImage.Bitmap.Width, Me.Height)
            End If

            Dim brushPen As New SolidBrush(_Scheme.Embossdkcolor)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 4
            ps.Color = _Scheme.Embossltcolor
            g.DrawLine(ps, 1, 1, Me.Width - 1, 1)
            g.DrawLine(ps, Me.Width - 1, 1, Me.Width - 1, Me.Height - 1)

            ps.Color = _Scheme.Embossdkcolor
            g.DrawLine(ps, 1, 1, 1, Me.Height - 1)
            g.DrawLine(ps, 1, Me.Height - 1, Me.Width - 1, Me.Height - 1)

            Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)

            Dim intTextSize As SizeF
            Try
                intTextSize = g.MeasureString(Text, MyBase.Font)
            Catch ex As Exception
                intTextSize = New SizeF(10, 18)
            End Try

            Select Case _TitleAlign
                Case HorizAlign.Center
                    'g.DrawString(Text, MyBase.Font, textBrush, _Image.Width + (Me.Width - _Image.Width - intTextSize.Width) / 2 + 2, (Height - intTextSize.Height) / 2)
                    g.DrawString(Text, MyBase.Font, textBrush, (Me.Width - intTextSize.Width) / 2 + 2, (Height - intTextSize.Height) / 2)
                Case Else
                    If _VGDDImage IsNot Nothing Then
                        g.DrawString(Text, MyBase.Font, textBrush, _VGDDImage.Bitmap.Width + 2, (Height - intTextSize.Height) / 2)
                    Else
                        g.DrawString(Text, MyBase.Font, textBrush, 2, (Height - intTextSize.Height) / 2)
                    End If
            End Select
        End Sub

#Region "GDDProps"

        <Description("Bitmap name to draw as window header")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        Public Overrides Property Bitmap() As String
            Get
                If _VGDDImage IsNot Nothing Then
                    Return _VGDDImage.Name
                Else
                    Return (String.Empty)
                End If
            End Get
            Set(ByVal value As String)
                Dim OldImage As VGDDImage = _VGDDImage
                If value = String.Empty Then
                    '_VGDDImage.TransparentBitmap = VGDDImage.InvalidBitmap(Nothing, Me.Width, Me.Height)
                    '_VGDDImage._GraphicsPath = New GraphicsPath
                    '_VGDDImage._GraphicsPath.AddRectangle(Me.ClientRectangle)
                    _VGDDImage = Nothing
                    If OldImage IsNot Nothing Then OldImage.RemoveUsedBy(Me)
                Else
                    _VGDDImage = GetBitmap(value)
                    If _VGDDImage Is Nothing OrElse _VGDDImage.Bitmap Is Nothing Then
                        _VGDDImage = Nothing
                        '_VGDDImage.TransparentBitmap = Nothing
                        If OldImage IsNot Nothing Then OldImage.RemoveUsedBy(Me)
                    Else
                        If _VGDDImage.AllowScaling Then
                            _VGDDImage.ScaleBitmap(MyBase.Height - 2, MyBase.Height - 2, 1)
                        End If
                        If OldImage IsNot Nothing AndAlso Not OldImage Is _VGDDImage Then
                            OldImage.RemoveUsedBy(Me)
                            _VGDDImage.AddUsedBy(Me)
                        End If
                        _VGDDImage.AddUsedBy(Me)
                    End If
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Text Alignement of the Window Title")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Left")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property TitleAlign() As HorizAlign
            Get
                Return _TitleAlign
            End Get
            Set(ByVal value As HorizAlign)
                _TitleAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Vertical Text Alignement of the Window Title")> _
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

            'If _public Then
            '    MyCodeHead &= vbCrLf & CodeGen.ConstructorTemplate.Trim
            'Else
            CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            'End If
            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", IIf(_State = EnabledState.Enabled, True, False))
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    CodeGen.AddState(MyState, "TitleAlign", _TitleAlign.ToString)
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _TitleAlign.ToString)
                    CodeGen.AddAlignment(MyAlignment, "Vertical", _VertAlign.ToString)
            End Select

            If Me.Text = String.Empty Then
                MyCodeHead = ""
            End If

            Dim myText As String = ""
            Me.Text = Me.Text.PadRight(GetMaxTextLength(Me.TextStringID), "_") ' DW
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            Dim MyBitmap As String = "", MyPointerInit As String = ""
            If Me.Bitmap = String.Empty Then
                MyBitmap = "NULL"
            Else
                Dim myBitmapName As String = System.IO.Path.GetFileNameWithoutExtension(Me.Bitmap)
                If Me.BitmapUsePointer Then
                    Dim strPointerName As String = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerName", 0).Trim _
                                                   .Replace("[BITMAPNAME]", myBitmapName)
                    MyBitmap = "(void *)" & strPointerName
                    MyPointerInit = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerInit", 0).Trim _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName) _
                        .Replace("[BITMAPNAME]", myBitmapName)
                    CodeGen.AddLines(CodeGen.Headers, CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/Bitmaps", 0) _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[BITMAPNAME]", myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName))
                Else
                    MyBitmap = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName
                End If
            End If

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
                .Replace("[BITMAP]", MyBitmap) _
                .Replace("[BITMAP_POINTER_INIT]", MyPointerInit) _
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

        Private Sub Window_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                If _Scheme.Font.SmartCharSet Then
                    _Scheme.Font.SmartCharSetAddString(Me.Text)
                End If
            End If
        End Sub

    End Class

End Namespace
