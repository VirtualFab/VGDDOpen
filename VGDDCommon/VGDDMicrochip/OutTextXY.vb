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
    <ToolboxBitmap(GetType(Button), "OutTextXYIco")> _
    Public Class OutTextXY : Inherits VGDDBase

        Private _Color As Color = Drawing.Color.Black
        Private _Font As VGDDCommon.VGDDFont
        Private _FontName As String
        Private _ZDrawFirst As Boolean = True

        Public Sub New()
            Me.RemovePropertyToShow("VGDDEvents")
            Me.AddPropertyToShow("Font")
            Me.AddPropertyToShow("Text")
            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.Opaque, False)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, False)
            'Me.DoubleBuffered = True
            'Me.BackColor = Drawing.Color.Transparent
            If _Schemes.Count > 0 Then
                Dim oFont As Font = GetScheme("").Font.Font
                If oFont IsNot Nothing Then
                    Me.Font = Common.FontToString(oFont)
                    Me.Height = oFont.Height
                End If
            End If

            'AddHandler Me.PropertyChanged, AddressOf OnPropertyChanged
            'RaiseEvent PropertyChanged(False)

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

        Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
            'If Me.Parent.GetType Is GetType(PlayerPanel) Then
            '    MyBase.OnPaintBackground(pevent)
            'End If
        End Sub

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If

            Me.OnPaintBackground(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle
            'Dim TextSize As SizeF = g.MeasureString(Text, MyBase.Font)

            'If TextSize.Width > MyBase.Width Then
            '    Me.Width = TextSize.Width + 2
            'End If
            'If TextSize.Height > MyBase.Height Then
            '    Me.Height = TextSize.Height + 2
            'End If

            Dim textBrush As SolidBrush = New SolidBrush(_Color)
            'Dim textBrush As SolidBrush = New SolidBrush(Color.FromArgb(250, _Color))

            MyBase.OnPaint(pevent)

            'g.CompositingMode = CompositingMode.SourceOver
            'g.CompositingQuality = CompositingQuality.GammaCorrected
            'g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            'Me.BackColor = Drawing.Color.Transparent
            'Me.Parent.Invalidate(rc)

            'g.SmoothingMode = SmoothingMode.HighQuality
            'g.InterpolationMode = InterpolationMode.HighQualityBicubic
            'If _Transparent Then
            'Dim Mypath As GraphicsPath = New GraphicsPath
            'Mypath.StartFigure()
            ''Mypath.AddRectangle(Me.ClientRectangle)
            ''Mypath.AddRectangle(New System.Drawing.Rectangle(0, 0, TextSize.Width, TextSize.Height))
            'Mypath.AddString(Me.Text, MyBase.Font.FontFamily, MyBase.Font.Style, MyBase.Font.Size + 4, New Drawing.Point(0, 0), StringFormat.GenericDefault)
            ''TextPen.Width = 2
            ''Mypath.Widen(TextPen)
            'Mypath.CloseFigure()
            'Me.Region = New Region(Mypath)
            'g.FillRegion(textBrush, Me.Region)
            'Else
            'Me.Region = New Region(rc)
            'g.FillRegion(Brushes.Transparent, Me.Region)

            'g.SmoothingMode = SmoothingMode.HighQuality
            'g.InterpolationMode = InterpolationMode.HighQualityBicubic

            'g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel

            Dim drawFormat As New StringFormat
            drawFormat.FormatFlags = StringFormatFlags.NoWrap Or StringFormatFlags.NoClip 'StringFormatFlags.FitBlackBox '
            Dim TextPen As New Pen(textBrush)
            TextPen.Width = 1
            Try
                g.DrawString(Text, MyBase.Font, textBrush, -MyBase.Font.Size \ 6, 0, drawFormat)
            Catch ex As Exception
            End Try


            'End If

            'Draw Text
        End Sub

        'Protected Event PropertyChanged(ByVal _RecreateHandle As Boolean)
        'Private Sub OnPropertyChanged(ByVal _RecreateHandle As Boolean)
        '    If (_RecreateHandle = True) Then Me.RecreateHandle()
        '    Me.Invalidate()
        'End Sub

#Region "GDDProps"

        '<EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        'Public Overloads Property TextStringID As Integer
        '    Get
        '        Return MyBase.TextStringID
        '    End Get
        '    Set(ByVal value As Integer)
        '        MyBase.TextStringID = value
        '        If value = 0 Then
        '            Me.AddPropertyToShow("CDeclType")
        '        Else
        '            Me.RemovePropertyToShow("CDeclType")
        '        End If
        '        If _Font IsNot Nothing AndAlso _Font.SmartCharSet Then _Font.ToBeConverted = True
        '        Me.Invalidate()
        '    End Set
        'End Property

        <Description("Draw this primitive before (set to true) or after (set to false) the Widgets?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("Design")> _
        <DefaultValue(True)> _
        Property ZDrawFirst() As Boolean
            Get
                Return _ZDrawFirst
            End Get
            Set(ByVal value As Boolean)
                _ZDrawFirst = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Color for the OutTextTextXY")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Color() As Color
#Else
        Public Overloads Property Color() As Color
#End If
            Get
                Return _Color
            End Get
            Set(ByVal value As Color)
                _Color = value
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Font for the OutTextTextXY")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDFontNameChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property Font() As String
#Else
        Public Shadows Property Font() As String
#End If
            Get
                If _Font IsNot Nothing Then
                    Return _FontName
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                _Font = GetFont(value, Me)
                If _Font Is Nothing Then _Font = _Fonts(0)
                Try
                    MyBase.Font = _Font.Font ' New Font(_Font.Font.FontFamily, CInt(_Font.Font.Size), _Font.Font.Style) '_Font.Font.Clone * 0.98
                Catch ex As Exception
                    _Font = _Fonts(0)
                    MyBase.Font = _Font.Font 'New Font(_Font.Font.FontFamily, CInt(_Font.Font.Size), _Font.Font.Style) '_Font.Font.Clone * 0.98
                End Try
                _FontName = _Font.Name
                _Font.AddUsedBy(Me)
                VGDDWidget_FinishedLoading(Nothing, Nothing)
                Me.Invalidate()
                'Me.RecreateHandle()
            End Set
        End Property

        <CustomSortedCategory("Size and Position", 3)> _
        <Description("Left X coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property X() As Integer
            Get
                Return MyBase.Left
            End Get
            Set(ByVal value As Integer)
                MyBase.Left = value
                Me.Invalidate()
                'Me.RecreateHandle()
            End Set
        End Property

        <CustomSortedCategory("Size and Position", 3)> _
        <Description("Top Y coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Y() As Integer
            Get
                Return Me.Location.Y
            End Get
            Set(ByVal value As Integer)
                'Me.Location = New Point(Me.Location.X, value)
                MyBase.Top = value
                Me.Invalidate()
                'Me.RecreateHandle()
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
            If Me.Text = String.Empty Then
                MyCodeHead = ""
            End If

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Font, myText)

            MyCode = MyCode _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", Me.TextStringID - 1) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[TEXTLENGTH]", myText.Length) _
                .Replace("[COLOR]", CodeGen.Color2Num(_Color, False, "OutTextXY " & Me.Name)) _
                .Replace("[COLOR_STRING]", _Color.ToString.Replace("A=255, ", "")) _
                .Replace("[FONT]", Me._Font.Name) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar)
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
            If Me.Text <> String.Empty Then
                CodeGen.AddLines(MyHeaders, CodeGen.MyHeader(_CDeclType))
            End If
            CodeGen.AddLines(MyHeaders, CodeGen.TextDeclareHeaderTemplate(_CDeclType))
            CodeGen.AddLines(MyHeaders, CodeGen.HeadersTemplate)
            CodeGen.AddLines(CodeGen.Headers, MyHeaders _
                .Replace("[STRINGID]", Me.TextStringID - 1) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            If _ZDrawFirst Then
                CodeGen.AddLines(CodeGen.Code, MyCode)
            Else
                CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCode)
            End If

        End Sub
#End If

        Private Sub OutTextXY_Invalidated(ByVal sender As Object, ByVal e As System.Windows.Forms.InvalidateEventArgs) Handles Me.Invalidated
            'Me.RecreateHandle()
        End Sub

        Private Sub OutTextXY_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Font IsNot Nothing AndAlso _Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Font.SmartCharSet Then
                _Font.SmartCharSetAddString(Me.Text)
            End If
            If Not Me.IsLoading Then
                Me.RecreateHandle()
            End If
            If MyBase._TextStringID = 0 Then
                Me.AddPropertyToShow("CDeclType")
            Else
                Me.RemovePropertyToShow("CDeclType")
            End If
            If _Font IsNot Nothing Then
                If _Font.SmartCharSet Then _Font.ToBeConverted = True
                If Common.ProjectMultiLanguageTranslations > 0 Then
                    Dim oStringSet As MultiLanguageStringSet = VGDDCommon.Common.ProjectStringPool(_TextStringID)
                    If oStringSet.AutoWrap Then
                        oStringSet.AutoWrapStrings(_Font.Font, Me.Width)
                    End If
                End If
            End If
        End Sub

        Private Sub VGDDWidget_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            If _Font IsNot Nothing AndAlso _Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Font.SmartCharSet Then
                _Font.SmartCharSetAddString(Me.Text)
            End If
            If Common.ProjectMultiLanguageTranslations > 0 AndAlso MyBase.Text IsNot Nothing Then
                CheckStringID(MyBase.Text, _TextStringID, _Font.Name)
                Dim oStringSet As MultiLanguageStringSet = VGDDCommon.Common.ProjectStringPool(_TextStringID)
                If oStringSet.AutoWrap Then
                    oStringSet.AutoWrapStrings(_Font.Font, Me.Width)
                End If
            End If
        End Sub

    End Class
End Namespace