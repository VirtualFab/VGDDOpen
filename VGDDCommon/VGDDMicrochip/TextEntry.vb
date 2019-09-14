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
    <ToolboxBitmap(GetType(Button), "TextEntryIco")> _
    Public Class TextEntry : Inherits VGDDWidget

        Private _BufferLength As Integer = 16
        Private _KeysHorizontal As Integer = 3
        Private _KeysVertical As Integer = 4
        Private _DisplayFont As String
        Private _ButtonTexts As Collections.Specialized.StringCollection
        Private _CommandKeyEnter As String = "Enter"
        Private _CommandKeySpace As String = "_"
        Private _CommandKeyDelete As String = "Del"
        Private _TextBox As TextBox 'EditBox
        Private _TextLayout As TextEntryLayout = TextEntryLayout.Numeric
        Private _HorizAlign As HorizAlign = HorizAlign.Right
        Friend Shared _Instances As Integer = 0

        Public Enum TextEntryLayout
            UserDefined
            Numeric
            AlphaABCDEF
            AlphaQwerty
            AlphaNumericABCDEF
            AlphaNumericQwerty
            AllKeys
            Hex
        End Enum

        Public Sub New()
            MyBase.New()
            _Instances += 1
            'SetStyle(ControlStyles.Selectable, False)
            'SetStyle(ControlStyles.UserPaint, True)
            MyBase.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("TextEntry")
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Me.RemovePropertyToShow("HorizAlign")
            End Select
#End If
            _ButtonTexts = New Collections.Specialized.StringCollection
        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                End If
                ClearControls()
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

        Private Sub ClearControls()
            Dim aControls(Me.Controls.Count) As Control
            Me.Controls.CopyTo(aControls, 0)
            For Each oControl As Control In aControls
                Dim oButton As Button = TryCast(oControl, Button)
                If oButton IsNot Nothing Then
                    RemoveHandler oButton.Click, AddressOf TeButtonClicked
                    oButton.Dispose()
                End If
            Next
            Me.Controls.Clear()
        End Sub

        Private Sub GenKeypad()
            If Me.IsLoading Then Exit Sub
            Try
                If _Scheme Is Nothing Then Exit Sub
                '    If _Schemes.Count = 0 Then Exit Sub
                '    _Scheme = _Schemes(0)
                'End If
                ClearControls()
                _TextBox = New TextBox 'EditBox
                Dim oFont As VGDDFont = Nothing
                With _TextBox
                    Try
                        oFont = GetFont(_DisplayFont, Nothing)
                        Debug.Print("")
                    Catch ex As Exception
                    End Try
                    If oFont Is Nothing Then
                        oFont = _Fonts(0)
                    End If
                    '.Dock = DockStyle.None
                    '.Anchor = AnchorStyles.None
                    .Font = oFont.Font
                    '.Size = New Size(Me.Width, 50)
                    '.Height = oFont.Font.Height + 20
                    .Width = Me.Width
                    .ForeColor = _Scheme.Textcolor1
                    .BackColor = _Scheme.Color1
                    .Text = Me.Text
                    .TextAlign = HorizontalAlignment.Right
                    .Invalidate()
                End With
                Dim bg As Integer = Me.Height - _TextBox.Height
                Me.Controls.Add(_TextBox)
                For i As Integer = 1 To _KeysVertical
                    For j As Integer = 1 To _KeysHorizontal
                        Dim oButton As New Button ' System.Windows.Forms.Button
                        With oButton
                            .Height = bg / _KeysVertical
                            .Width = Me.Width / _KeysHorizontal
                            .Left = (j - 1) * Me.Width / _KeysHorizontal
                            .Top = _TextBox.Height + (i - 1) * bg / _KeysVertical
                            .Scheme = _Scheme.Name
                            '.Font = _Scheme.Font.Font
                            '.ForeColor = _sc
                            Dim bi As Integer = (i - 1) * _KeysHorizontal + j
                            If _ButtonTexts Is Nothing OrElse bi > _ButtonTexts.Count Then
                                .Text = "?"
                            Else
                                .Text = _ButtonTexts(bi - 1)
                            End If
                        End With
                        AddHandler oButton.Click, New EventHandler(AddressOf TeButtonClicked)
                        Me.Controls.Add(oButton)
                    Next
                Next
            Catch ex As Exception
            End Try
            Me.Invalidate()
        End Sub

        Private Sub TeButtonClicked(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim oButton As Button = sender
            If _TextBox.Text = Me.Name Then _TextBox.Text = ""
            If oButton.Text = _CommandKeyDelete AndAlso _TextBox.Text.Length > 0 Then
                _TextBox.Text = _TextBox.Text.Substring(0, _TextBox.Text.Length - 1)
            ElseIf oButton.Text = _CommandKeySpace Then
                _TextBox.Text &= " "
            ElseIf oButton.Text = _CommandKeyEnter Then
                _TextBox.Text = ""
            Else
                _TextBox.Text &= oButton.Text
            End If
            Me.Invalidate()
        End Sub

#Region "GDDProps"

        <Description("Horizontal Text Alignement of the TextEntry")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Center")> _
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

        Public Overrides Property Scheme As String
            Get
                Return MyBase.Scheme
            End Get
            Set(ByVal value As String)
                MyBase.Scheme = value
                If _DisplayFont Is Nothing Then
                    If GetScheme(value) IsNot Nothing Then
                        _DisplayFont = GetScheme(value).Font.Name
                    End If
                    TextLayout = TextEntryLayout.Numeric
                End If
            End Set
        End Property

        <Description("Quick Default Layout keys")> _
        <CustomSortedCategory("Main", 2)> _
        Public Property TextLayout As TextEntryLayout
            Get
                Return _TextLayout
            End Get
            Set(ByVal value As TextEntryLayout)
                _TextLayout = value
                If Me.IsLoading Then Exit Property
                Select Case _TextLayout
                    Case TextEntryLayout.UserDefined
                        Exit Select
                    Case TextEntryLayout.Numeric
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Del", "Enter"})
                        _KeysHorizontal = 4
                        _KeysVertical = 3
                        _CommandKeyEnter = "Enter"
                        _CommandKeySpace = ""
                        _CommandKeyDelete = "Del"
                    Case TextEntryLayout.AlphaABCDEF
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "spc", ".", "Del", "OK"})
                        _KeysHorizontal = 10
                        _KeysVertical = 3
                        _CommandKeyEnter = "OK"
                        _CommandKeySpace = "spc"
                        _CommandKeyDelete = "Del"
                    Case TextEntryLayout.AlphaNumericABCDEF
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", _
                                                            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", _
                                                            "spc", ".", "Del", "OK"})
                        _KeysHorizontal = 10
                        _KeysVertical = 4
                        _CommandKeyEnter = "OK"
                        _CommandKeySpace = "spc"
                        _CommandKeyDelete = "Del"
                    Case TextEntryLayout.AlphaQwerty
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Del", "Z", "X", "C", "V", "B", "N", "M", "spc", ".", "OK"})
                        _CommandKeyEnter = "OK"
                        _CommandKeySpace = "spc"
                        _CommandKeyDelete = "Del"
                        _KeysHorizontal = 10
                        _KeysVertical = 3
                    Case TextEntryLayout.AlphaNumericQwerty
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", _
                                                            "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Del", "Z", "X", "C", "V", "B", "N", "M", _
                                                            "spc", ".", "OK"})
                        _CommandKeyEnter = "OK"
                        _CommandKeySpace = "spc"
                        _CommandKeyDelete = "Del"
                        _KeysHorizontal = 10
                        _KeysVertical = 4
                    Case TextEntryLayout.AllKeys
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Del", _
                                                            "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "+", _
                                                            "A", "S", "D", "F", "G", "H", "J", "K", "L", "-", "*", _
                                                            "Z", "X", "C", "V", "B", "N", "M", "=", "^", "_", ":", _
                                                            "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", ";", _
                                                            "a", "s", "d", "f", "g", "h", "j", "k", "l", "(", ")", _
                                                            "z", "x", "c", "v", "b", "n", "m", "%", "&", "[", "]", _
                                                            "!", """", "?", "#", "/", "$", "'", "@", "spc", ".", "OK"})
                        _CommandKeyEnter = "OK"
                        _CommandKeySpace = "spc"
                        _CommandKeyDelete = "Del"
                        _KeysHorizontal = 11
                        _KeysVertical = 8
                    Case TextEntryLayout.Hex
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "A", "B", "C", "D", "E", "F", "Del", "OK"})
                        _CommandKeyEnter = "OK"
                        _CommandKeyDelete = "Del"
                        _KeysHorizontal = 3
                        _KeysVertical = 6
                End Select
                GenKeypad()
            End Set
        End Property

        '<EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        '<DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        'Public Shadows ReadOnly Property Controls As ControlCollection
        '    Get
        '        Return MyBase.Controls
        '    End Get
        'End Property

        '<Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", GetType(System.Drawing.Design.UITypeEditor))> _
        '<Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        '"System.Drawing.Design.UITypeEditor, System.Drawing, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")> _

        <Description("Texts for the key Buttons")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
        <Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", GetType(System.Drawing.Design.UITypeEditor))> _
        Public Property ButtonTexts As Collections.Specialized.StringCollection
            Get
                Return _ButtonTexts
            End Get
            Set(ByVal value As Collections.Specialized.StringCollection)
                _ButtonTexts = value
                _TextLayout = TextEntryLayout.UserDefined
                GenKeypad()
            End Set
        End Property

        <Description("Number of Vertical Keys")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property KeysVertical As Integer
            Get
                Return _KeysVertical
            End Get
            Set(ByVal value As Integer)
                _KeysVertical = value
                GenKeypad()
            End Set
        End Property

        <Description("Number of Horizontal Keys")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property KeysHorizontal As Integer
            Get
                Return _KeysHorizontal
            End Get
            Set(ByVal value As Integer)
                _KeysHorizontal = value
                GenKeypad()
            End Set
        End Property

        <Description("Key string of the key to be associated to TE_ENTER_COM command")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property CommandKeyEnter As String
            Get
                Return _CommandKeyEnter
            End Get
            Set(ByVal value As String)
                _CommandKeyEnter = value
            End Set
        End Property

        <Description("Key string of the key to be associated to TE_DELETE_COM command")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property CommandKeyDelete As String
            Get
                Return _CommandKeyDelete
            End Get
            Set(ByVal value As String)
                _CommandKeyDelete = value
            End Set
        End Property

        <Description("Key string of the key to be associated to TE_SPACE_COM command")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property CommandKeySpace As String
            Get
                Return _CommandKeySpace
            End Get
            Set(ByVal value As String)
                _CommandKeySpace = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Font for the display ot this TextEntry")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDFontNameChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property DisplayFont() As String
#Else
        Public Property DisplayFont() As String
#End If
            Get
                Return _DisplayFont
            End Get
            Set(ByVal value As String)
                _DisplayFont = value
                GenKeypad()
            End Set
        End Property

        <Description("Length of the buffer")> _
        <CustomSortedCategory("CodeGen", 6)> _
        Property BufferLength() As Integer
            Get
                Return _BufferLength
            End Get
            Set(ByVal value As Integer)
                _BufferLength = value
                Me.Invalidate()
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.TextDeclareCodeHeadTemplate(TextCDeclType.RamXcharArray).Replace("[CHARMAX]", _BufferLength + 1)
            Dim MyCode As String = String.Empty, MyState As String = String.Empty, MyAlignment As String = String.Empty

            Dim strKeys As String = "", intKeyIdxEnter As Integer = -1, intKeyIdxDelete As Integer = -1, intKeyIdxSpace As Integer = -1
            Dim intIdx As Integer = 0
            If _ButtonTexts IsNot Nothing Then
                For Each strButtonText As String In _ButtonTexts
                    'If Common.ProjectUseMultiByteChars Then
                    'strKeys &= ",(XCHAR[])" & CodeGen.QText(strButtonText, _Scheme.Font, Nothing) & " // Button """ & strButtonText & """" & vbCrLf & "    "
                    strKeys &= "," & CodeGen.QtextBinary(CodeGen.QText(strButtonText, _Scheme.Font, Nothing), String.Empty, " Button """ & strButtonText & """", intIdx, strButtonText.Length, "")
                    'Else
                    'strKeys &= "," & CodeGen.QText(strButtonText, _Scheme.Font, Nothing)
                    'End If
                    If strButtonText = _CommandKeyEnter Then
                        intKeyIdxEnter = intIdx
                    ElseIf strButtonText = _CommandKeyDelete Then
                        intKeyIdxDelete = intIdx
                    ElseIf strButtonText = _CommandKeySpace Then
                        intKeyIdxSpace = intIdx
                    End If
                    intIdx += 1
                Next
                If strKeys.StartsWith(",") Then strKeys = strKeys.Substring(1)
                strKeys = strKeys.Replace("[NEWLINE]", vbCrLf) _
                                .Replace("[TAB]", "    ")
            End If

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

            CodeGen.AddLines(MyCodeHead, CodeGen.CodeHeadTemplate)

            CodeGen.AddState(MyState, "Enabled", IIf(_State = EnabledState.Enabled, True, False))
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _HorizAlign.ToString)
            End Select

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            If _DisplayFont = "" Then
                MessageBox.Show("TextEntry " & Me.Name & " on screen " & Me.Parent.Name & " is missing the mandatory DisplayFont property." & vbCrLf & "Compilation errors will happen if you don't fix this.", "Warning")
            End If
            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[HORKEYS]", _KeysHorizontal) _
                .Replace("[VERKEYS]", _KeysVertical) _
                .Replace("[ENTERKEYIDX]", intKeyIdxEnter) _
                .Replace("[DELETEKEYIDX]", intKeyIdxDelete) _
                .Replace("[SPACEKEYIDX]", intKeyIdxSpace) _
                .Replace("[BUFFERLEN]", _BufferLength) _
                .Replace("[DISPFONT]", _DisplayFont) _
                .Replace("[STATE]", MyState) _
                .Replace("[ALIGNMENT]", MyAlignment) _
                .Replace("[SCHEME]", Me.Scheme))
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[KEYS]", strKeys) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId) '_
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, (IIf(Me.Public, vbCrLf & "extern " & CodeGen.ConstructorTemplate.Trim, "") & _
                                CodeGen.TextDeclareHeaderTemplate(_CDeclType) & CodeGen.HeadersTemplate) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, Me)
        End Sub

        Private Sub TextEntry_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(Me.Text)
            End If
            Me.Invalidate()
        End Sub
#End If

        Private Sub TextEntry_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            TextEntry_TextChanged(Nothing, Nothing)
            GenKeypad()
        End Sub

        Private Sub TextEntry_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            GenKeypad()
        End Sub

        Private Sub TextEntry_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            If Common.ProjectMultiLanguageTranslations > 0 AndAlso Me.Parent IsNot Nothing Then
                For Each oControl As Object In Me.Controls
                    If TypeOf (oControl) Is VGDDBase Then
                        MyBase.StringsPoolSetUsedBy(CType(oControl, VGDDBase).TextStringID, StringsPoolSetUsedByAction.Add)
                    End If
                Next
            End If
        End Sub

    End Class

End Namespace
