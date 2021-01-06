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
    <ToolboxBitmap(GetType(TextEntryEx), "TextEntryEx.ico")> _
    Public Class TextEntryEx : Inherits VGDDWidget

        Private _BufferLength As Integer = 64
        Private _KeysHorizontal As Integer = 3
        Private _KeysVertical As Integer = 4
        Private _DisplayFont As String
        Private _KeysCollection As New VGDDKeyCollection
        Private _ButtonTexts As New Collections.Specialized.StringCollection
        Private _ButtonTextsAlternate As New Collections.Specialized.StringCollection
        Private _ButtonTextsShift As New Collections.Specialized.StringCollection
        Private _ButtonTextsShiftAlternate As New Collections.Specialized.StringCollection
        Private _TextBox As TextBox
        Private _TextLayout As TextEntryLayout = TextEntryLayout.RealKeyboard
        Private _KeysLayout As TextEntryKeysLayout = TextEntryKeysLayout.KeyboardLike
        Private _HorizontalSpacing As Integer
        Private _VerticalSpacing As Integer
        Private _BitmapReleasedKeyName As String
        Private _BitmapPressedKeyName As String
        Private _BitmapReleasedKey As VGDDImage
        Private _BitmapPressedKey As VGDDImage
        Private _Radius As Integer = 4
        Private _CommandKeysIdx(9) As Integer

        Private _IsNew As Boolean = True
        Private _ShiftActive As Boolean = False
        Private _AltActive As Boolean = False
        Private _CapsLockActive As Boolean = False

        Public KeyCommandsStrings As String() = {"TEEX_BKSP_COM", "TEEX_SPACE_COM", "TEEX_ENTER_COM", "TEEX_SHIFT_COM", "TEEX_ALT_COM", "TEEX_USER1", "TEEX_USER2", "TEEX_USER3", "TEEX_USER4"}

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
            RealKeyboard
        End Enum

        Public Enum TextEntryKeysLayout
            Grid
            KeyboardLike
        End Enum

        Public Sub New()
            MyBase.New()
            _Instances += 1
            MyBase.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("TextEntryEx")
            MyBase.RemovePropertyToShow("Controls")
#End If
            If Not Me.IsLoading Then
                TextLayout = TextEntryLayout.RealKeyboard
                KeysLayout = TextEntryKeysLayout.KeyboardLike
            End If
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

        Private Sub ClearControls()
            Dim aControls(Me.Controls.Count) As Control
            Me.Controls.CopyTo(aControls, 0)
            For i As Integer = 0 To aControls.Length - 1
                Dim oButton As Button = TryCast(aControls(i), Button)
                If oButton IsNot Nothing Then
                    Try
                        RemoveHandler oButton.Click, AddressOf TeButtonClicked
                    Catch ex As Exception
                    End Try
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
                Me.BackColor = _Scheme.Color0
                ClearControls()
                If _TextBox Is Nothing Then
                    _TextBox = New TextBox
                End If
                Dim oFont As VGDDFont = Nothing
                With _TextBox
                    Try
                        oFont = GetFont(_DisplayFont, Nothing)
                    Catch ex As Exception
                    End Try
                    If oFont Is Nothing Then
                        oFont = _Fonts(0)
                    End If
                    .Font = oFont.Font
                    .Width = Me.Width
                    .ForeColor = _Scheme.Textcolor1
                    .BackColor = _Scheme.Color1
                    .Text = Me.Text
                    _BufferLength = GetMaxTextLength(Me.TextStringID) ' me.Text.Length ' DW set the bufferlenght
                    .TextAlign = HorizontalAlignment.Right
                End With
                Me.Controls.Add(_TextBox)

                Dim ButtonWidth As Integer = (Me.Right - Me.Left + 1 - (_HorizontalSpacing * (_KeysHorizontal + 1))) / _KeysHorizontal
                Dim bg As Integer = Me.Height - _TextBox.Height
                Dim ButtonHeight As Integer = (bg - _VerticalSpacing * (_KeysVertical + 1)) / _KeysVertical
                Dim buttonLeft As Integer
                Dim ButtonIndex As Integer = 0
                Dim KeysHorizontal As Integer
                For intRow As Integer = 1 To _KeysVertical
                    buttonLeft = 0
                    KeysHorizontal = _KeysHorizontal
                    If _KeysLayout = TextEntryKeysLayout.KeyboardLike AndAlso ((intRow And 1) = 0) Then
                        buttonLeft += ButtonWidth >> 1
                        KeysHorizontal -= 1
                    End If
                    For intCol As Integer = 1 To KeysHorizontal
                        buttonLeft += _HorizontalSpacing
                        Dim bw As Integer = ButtonWidth
                        Dim strButtonText As String, strButtonTextOrig As String
                        If _ButtonTexts Is Nothing OrElse ButtonIndex >= _ButtonTexts.Count Then
                            strButtonText = "?"
                        Else
                            strButtonText = _ButtonTexts(ButtonIndex)
                        End If
                        If _AltActive Then
                            If _ShiftActive AndAlso ButtonIndex < _ButtonTextsShiftAlternate.Count AndAlso _ButtonTextsShiftAlternate(ButtonIndex) <> String.Empty Then
                                strButtonText = _ButtonTextsShiftAlternate(ButtonIndex)
                            ElseIf ButtonIndex < _ButtonTextsAlternate.Count AndAlso _ButtonTextsAlternate(ButtonIndex) <> String.Empty Then
                                strButtonText = _ButtonTextsAlternate(ButtonIndex)
                            End If
                        ElseIf _ShiftActive AndAlso ButtonIndex < _ButtonTextsShift.Count AndAlso _ButtonTextsShift(ButtonIndex) <> String.Empty Then
                            strButtonText = _ButtonTextsShift(ButtonIndex)
                        End If
                        strButtonTextOrig = strButtonText
                        If _KeysLayout = TextEntryKeysLayout.KeyboardLike Then
                            Select Case strButtonText
                                Case "SH"
                                    bw *= 1.5
                                    KeysHorizontal -= 1
                                Case "OK"
                                    bw *= 1.5
                                    KeysHorizontal -= 1
                                Case "AL"
                                    strButtonText = "?123"
                                    bw *= 1.8
                                    KeysHorizontal -= 1
                                Case "DL"
                                    strButtonText = "Del"
                                    bw *= 1.5
                                Case "BK"
                                    bw *= 1.5
                                Case "SP"
                                    strButtonText = ""
                                    bw = 4 * bw + 5 * _HorizontalSpacing
                                    KeysHorizontal -= 3
                            End Select
                        End If
                        If buttonLeft + bw > Me.Width Then
                            Exit For
                        End If

                        If bw <= 0 Or ButtonHeight <= 0 Or ButtonIndex > _KeysCollection.Count - 1 Then Exit For

                        Dim oButton As New Button ' System.Windows.Forms.Button
                        Me.Controls.Add(oButton)
                        With oButton
                            ._IsLoading = True
                            .Height = ButtonHeight
                            .Width = bw
                            .Left = buttonLeft
                            .Top = (_VerticalSpacing >> 1) + _TextBox.Height + (intRow - 1) * (ButtonHeight + _VerticalSpacing)
                            .Scheme = _Scheme.Name
                            .Radius = _Radius
                            .Tag = ButtonIndex
                            Select Case strButtonText
                                Case "SH"
                                    strButtonText = ""
                                    Dim oBitmap As New Bitmap(.Width, .Height)
                                    Dim pen As New Pen(Me.SchemeObj.Textcolor0)
                                    pen.Width = 2
                                    Dim g As Graphics = Graphics.FromImage(oBitmap)
                                    DrawPolyPathInit(pen, 0, 0, .Width, .Height, 64, 30)
                                    DrawPolyPath(g, -30, 40)
                                    DrawPolyPath(g, 20, 0)
                                    DrawPolyPath(g, 0, 30)
                                    DrawPolyPath(g, 20, 0)
                                    DrawPolyPath(g, 0, -30)
                                    DrawPolyPath(g, 20, 0)
                                    DrawPolyPathClose(g)
                                    .SetBitmap(oBitmap)
                                Case "BK"
                                    strButtonText = ""
                                    Dim oBitmap As New Bitmap(.Width, .Height)
                                    Dim pen As New Pen(Me.SchemeObj.Textcolor0)
                                    pen.Width = 2
                                    Dim g As Graphics = Graphics.FromImage(oBitmap)
                                    DrawPolyPathInit(pen, 0, 0, .Width, .Height, 14, 64)
                                    DrawPolyPath(g, 24, -24)
                                    DrawPolyPath(g, 68, 0)
                                    DrawPolyPath(g, 0, 48)
                                    DrawPolyPath(g, -68, 0)
                                    DrawPolyPath(g, -24, -24)
                                    DrawPolyPathClose(g)


                                    DrawPolyPathInit(pen, 0, 0, .Width, .Height, 70, 53)
                                    DrawPolyPath(g, 22, 22)

                                    DrawPolyPathInit(pen, 0, 0, .Width, .Height, 70, 75)
                                    DrawPolyPath(g, 22, -22)
                                    .SetBitmap(oBitmap)
                                Case Else

                            End Select

                            If strButtonText <> String.Empty Then
                                .Text = strButtonText
                            End If

                            ButtonIndex += 1
                            If _BitmapReleasedKey IsNot Nothing Then
                                .Bitmap = _BitmapReleasedKeyName
                            End If
                            If _BitmapPressedKey IsNot Nothing Then

                            End If
                        End With
                        AddHandler oButton.Click, New EventHandler(AddressOf TeButtonClicked)
                        oButton._IsLoading = False
                        buttonLeft = oButton.Right
                    Next
                Next
                If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                    If _Scheme.Font.SmartCharSet Then
                        LoadStringsFromKeys()
                        Dim sb As New System.Text.StringBuilder
                        For Each strKey As String In _ButtonTexts
                            sb.Append(strKey)
                        Next
                        For Each strKey As String In _ButtonTextsAlternate
                            sb.Append(strKey)
                        Next
                        For Each strKey As String In _ButtonTextsShift
                            sb.Append(strKey)
                        Next
                        For Each strKey As String In _ButtonTextsShiftAlternate
                            sb.Append(strKey)
                        Next
                        _Scheme.Font.SmartCharSetAddString(Me.Text & sb.ToString)
                    End If
                End If
            Catch ex As Exception
            End Try
            Me.Invalidate()
        End Sub

#Region "PolyPath"
        Dim xPolyPathLeft As Integer, yPolyPathTop As Integer
        Dim xPolyPathSize As Integer, yPolyPathSize As Integer
        Dim xPolyPathFirst As Integer, yPolyPathFirst As Integer
        Dim xPolyPathLast As Integer, yPolyPathLast As Integer
        Dim PolyPathPen As Pen

        Public Sub DrawPolyPathInit(ByVal pen As Pen, ByVal left As Integer, ByVal top As Integer, ByVal right As Integer, ByVal bottom As Integer, ByVal xStart As Integer, ByVal yStart As Integer)
            xPolyPathLeft = left
            yPolyPathTop = top
            xPolyPathSize = right - left
            yPolyPathSize = bottom - top
            xPolyPathFirst = left + ((xPolyPathSize * xStart) >> 7)
            yPolyPathFirst = top + ((yPolyPathSize * yStart) >> 7)
            xPolyPathLast = xStart
            yPolyPathLast = yStart
            PolyPathPen = pen
        End Sub

        Public Sub DrawPolyPath(ByVal g As Graphics, ByVal xEnd As Integer, ByVal yEnd As Integer)
            Dim x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer
            x1 = xPolyPathLeft + ((xPolyPathSize * (xPolyPathLast)) >> 7)
            y1 = yPolyPathTop + ((yPolyPathSize * (yPolyPathLast)) >> 7)
            x2 = xPolyPathLeft + ((xPolyPathSize * (xPolyPathLast + xEnd)) >> 7)
            y2 = yPolyPathTop + ((yPolyPathSize * (yPolyPathLast + yEnd)) >> 7)
            g.DrawLine(PolyPathPen, x1, y1, x2, y2)
            xPolyPathLast += xEnd
            yPolyPathLast += yEnd
        End Sub

        Public Sub DrawPolyPathClose(ByVal g As Graphics)
            g.DrawLine(PolyPathPen, xPolyPathLeft + ((xPolyPathSize * (xPolyPathLast)) >> 7), yPolyPathTop + ((yPolyPathSize * (yPolyPathLast)) >> 7), xPolyPathFirst, yPolyPathFirst)
        End Sub
#End Region

        Private Sub TeButtonClicked(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim oButton As Button = sender
            Dim buttonIndex As Integer = oButton.Tag
            If _TextBox.Text = Me.Name Then _TextBox.Text = ""
            Dim blnCommand As Boolean = False
            For i As Integer = 0 To _CommandKeysIdx.Length - 1
                If _CommandKeysIdx(i) = buttonIndex + 1 Then
                    blnCommand = True
                    Select Case i + 1
                        Case 1 ' TEEX_BKSP_COM
                            If _TextBox.Text.Length > 0 Then
                                _TextBox.Text = _TextBox.Text.Substring(0, _TextBox.Text.Length - 1)
                            End If
                            Exit For
                        Case 2 ' TEEX_SPACE_COM
                            _TextBox.Text &= " "
                            Exit For
                        Case 3 ' TEEX_ENTER_COM
                            _TextBox.Text = ""
                            Exit For
                        Case 4 ' TEEX_SHIFT_COM
                            _CapsLockActive = False
                            _ShiftActive = Not _ShiftActive
                            GenKeypad()
                            Exit For
                        Case 5 ' TEEX_ALT_COM
                            _AltActive = Not _AltActive
                            GenKeypad()
                            Exit For
                    End Select
                End If
            Next
            If Not blnCommand Then
                _TextBox.Text &= oButton.Text
            End If
            Me.Invalidate()
        End Sub

        Private Sub LoadKeysFromStrings()
            If _ButtonTexts IsNot Nothing Then
                _KeysCollection.Clear()
                For i As Integer = 0 To _ButtonTexts.Count - 1
                    Dim oKey As New VGDDKey
                    oKey.Key = _ButtonTexts(i)
                    If i < _ButtonTextsShift.Count - 1 Then
                        oKey.KeyShift = _ButtonTextsShift(i)
                    End If
                    If i < _ButtonTextsAlternate.Count - 1 Then
                        oKey.KeyAlternate = _ButtonTextsAlternate(i)
                    End If
                    If i < _ButtonTextsShiftAlternate.Count - 1 Then
                        oKey.KeyShiftAlternate = _ButtonTextsShiftAlternate(i)
                    End If
                    For j = 0 To _CommandKeysIdx.Length - 1
                        If _CommandKeysIdx(j) = i + 1 Then
                            oKey.KeyCommand = KeyCommandsStrings(j)
                            Exit For
                        End If
                    Next
                    _KeysCollection.Add(oKey)
                Next
            End If
        End Sub

        Private Sub LoadStringsFromKeys()
            _ButtonTexts.Clear()
            _ButtonTextsShift.Clear()
            _ButtonTextsAlternate.Clear()
            _ButtonTextsShiftAlternate.Clear()
            For i As Integer = 0 To _CommandKeysIdx.Length - 1
                _CommandKeysIdx(i) = 0
            Next
            For i As Integer = 0 To _KeysCollection.Count - 1
                Dim oKey As VGDDKey = _KeysCollection(i)
                _ButtonTexts.Add(oKey.Key)
                _ButtonTextsShift.Add(oKey.KeyShift)
                _ButtonTextsAlternate.Add(oKey.KeyAlternate)
                _ButtonTextsShiftAlternate.Add(oKey.KeyShiftAlternate)
                If oKey.KeyCommand <> String.Empty Then
                    Select Case oKey.KeyCommand.ToUpper
                        Case "TEEX_BKSP_COM"
                            _CommandKeysIdx(0) = i + 1
                        Case "TEEX_SPACE_COM"
                            _CommandKeysIdx(1) = i + 1
                        Case "TEEX_ENTER_COM"
                            _CommandKeysIdx(2) = i + 1
                        Case "TEEX_SHIFT_COM"
                            _CommandKeysIdx(3) = i + 1
                        Case "TEEX_ALT_COM"
                            _CommandKeysIdx(4) = i + 1
                        Case "TEEX_USER1"
                            _CommandKeysIdx(5) = i + 1
                        Case "TEEX_USER2"
                            _CommandKeysIdx(6) = i + 1
                        Case "TEEX_USER3"
                            _CommandKeysIdx(7) = i + 1
                        Case "TEEX_USER4"
                            _CommandKeysIdx(8) = i + 1
                    End Select
                End If
            Next
        End Sub


#Region "GDDProps"
        Public Overloads Property Text As String
            Get
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                MyBase.Text = value
                If _TextBox IsNot Nothing Then
                    _TextBox.Text = value
                    _TextBox.Invalidate()
                End If
            End Set
        End Property

        <Description("Define the radius roundness of the buttons")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 256)> _
        <DefaultValue(0)> _
        <Category("TextEntryEx")> _
        Property Radius() As Integer
            Get
                Return _Radius
            End Get
            Set(ByVal value As Integer)
                _Radius = value
                GenKeypad()
                Me.Invalidate()
            End Set
        End Property

        <Description("Horizontal spacing between keys, in pixels")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("TextEntryEx")> _
        Public Property HorizontalSpacing As Integer
            Get
                Return _HorizontalSpacing
            End Get
            Set(ByVal value As Integer)
                _HorizontalSpacing = value
                GenKeypad()
            End Set
        End Property

        <Description("Vertical spacing between keys, in pixels")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("TextEntryEx")> _
        Public Property VerticalSpacing As Integer
            Get
                Return _VerticalSpacing
            End Get
            Set(ByVal value As Integer)
                _VerticalSpacing = value
                GenKeypad()
            End Set
        End Property

        <Description("Bitmap name to draw for the pressed button")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("TextEntryEx")> _
        Public Overridable Property BitmapPressedKey() As String
            Get
                Return _BitmapPressedKeyName
            End Get
            Set(ByVal value As String)
                _BitmapPressedKeyName = value
                If value = String.Empty Then
                    If _BitmapPressedKey IsNot Nothing Then
                        _BitmapPressedKey.RemoveUsedBy(Me)
                    End If
                    _BitmapPressedKey = Nothing
                ElseIf Not _IsLoading Then
                    'SetBitmapName(value)
                    Common.SetBitmapName(value, Me, _BitmapPressedKeyName, _BitmapPressedKey, Nothing)
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Bitmap name to draw for the released button")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("TextEntryEx")> _
        Public Overridable Property BitmapReleasedKey() As String
            Get
                Return _BitmapReleasedKeyName
            End Get
            Set(ByVal value As String)
                _BitmapReleasedKeyName = value
                If value = String.Empty Then
                    If _BitmapReleasedKey IsNot Nothing Then
                        _BitmapReleasedKey.RemoveUsedBy(Me)
                    End If
                    _BitmapReleasedKey = Nothing
                ElseIf Not _IsLoading Then
                    'SetBitmapName(value)
                    Common.SetBitmapName(value, Me, _BitmapReleasedKeyName, _BitmapReleasedKey, Nothing)
                End If
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
                    Try
                        _DisplayFont = _Schemes(value).Font.Name
                    Catch ex As Exception
                    End Try
                    'TextLayout = TextEntryLayout.RealKeyboard
                End If
                GenKeypad()

            End Set
        End Property

        <Description("Quick Default Layout keys")> _
        <Category("TextEntryEx")> _
        Public Property TextLayout As TextEntryLayout
            Get
                Return _TextLayout
            End Get
            Set(ByVal value As TextEntryLayout)
                _TextLayout = value
                If _ButtonTexts Is Nothing Then
                    _ButtonTexts = New Collections.Specialized.StringCollection
                    _ButtonTextsAlternate = New Collections.Specialized.StringCollection
                    _ButtonTextsShift = New Collections.Specialized.StringCollection
                    _ButtonTextsShiftAlternate = New Collections.Specialized.StringCollection
                End If
                If _TextLayout <> TextEntryLayout.UserDefined Then
                    _ButtonTexts.Clear()
                    _ButtonTextsAlternate.Clear()
                    _ButtonTextsShift.Clear()
                    _ButtonTextsShiftAlternate.Clear()
                End If
                Select Case _TextLayout
                    Case TextEntryLayout.UserDefined
                        Exit Select
                    Case TextEntryLayout.Numeric
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "BK", "0", "OK"})
                        _KeysHorizontal = 3
                        _KeysVertical = 4
                        _CommandKeysIdx = {10, 0, 12, 0, 0, 0, 0, 0, 0}
                        _KeysLayout = TextEntryKeysLayout.Grid
                    Case TextEntryLayout.AlphaABCDEF
                        _ButtonTexts.AddRange(New String() {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", _
                                                            "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", _
                                                            "U", "V", "W", "X", "Y", "Z", "SP", ".", "BK", "OK"})
                        _KeysHorizontal = 10
                        _KeysVertical = 3
                        _CommandKeysIdx = {29, 27, 30, 0, 0, 0, 0, 0, 0}
                        _KeysLayout = TextEntryKeysLayout.Grid
                    Case TextEntryLayout.AlphaNumericABCDEF
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", _
                                                            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", _
                                                            "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", _
                                                            "U", "V", "W", "X", "Y", "Z", "SP", ".", "BK", "OK"})
                        _KeysHorizontal = 10
                        _KeysVertical = 4
                        _CommandKeysIdx = {39, 37, 40, 0, 0, 0, 0, 0, 0}
                        _KeysLayout = TextEntryKeysLayout.Grid
                    Case TextEntryLayout.AlphaQwerty
                        _ButtonTexts.AddRange(New String() {"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", _
                                                            "A", "S", "D", "F", "G", "H", "J", "K", "L", "BK", _
                                                            "Z", "X", "C", "V", "B", "N", "M", "SP", ".", "OK"})
                        _CommandKeysIdx = {19, 27, 29, 0, 0, 0, 0, 0, 0}
                        _KeysHorizontal = 10
                        _KeysVertical = 3
                        _KeysLayout = TextEntryKeysLayout.Grid
                    Case TextEntryLayout.AlphaNumericQwerty
                        _ButtonTexts.Clear()
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", _
                                                            "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", _
                                                            "A", "S", "D", "F", "G", "H", "J", "K", "L", "BK", _
                                                            "Z", "X", "C", "V", "B", "N", "M", "SP", ".", "OK"})
                        _CommandKeysIdx = {30, 38, 40, 0, 0, 0, 0, 0, 0}
                        _KeysHorizontal = 10
                        _KeysVertical = 4
                        _KeysLayout = TextEntryKeysLayout.Grid
                    Case TextEntryLayout.AllKeys
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "BK", _
                                                            "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "+", _
                                                            "A", "S", "D", "F", "G", "H", "J", "K", "L", "-", "*", _
                                                            "Z", "X", "C", "V", "B", "N", "M", "=", "^", "_", ":", _
                                                            "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", ";", _
                                                            "a", "s", "d", "f", "g", "h", "j", "k", "l", "(", ")", _
                                                            "z", "x", "c", "v", "b", "n", "m", "%", "&", "[", "]", _
                                                            "!", """", "?", "#", "/", "$", "'", "@", "SP", ".", "OK"})
                        '_CommandKeysIdx = {BK,SP,OK,SH,AL}
                        _CommandKeysIdx = {11, 86, 88, 0, 0, 0, 0, 0, 0}
                        _KeysHorizontal = 11
                        _KeysVertical = 8
                    Case TextEntryLayout.Hex
                        _ButtonTexts.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "BK", "0", "OK"})
                        _CommandKeysIdx = {16, 0, 18, 0, 0, 0, 0, 0, 0}
                        _KeysHorizontal = 3
                        _KeysVertical = 6
                        _KeysLayout = TextEntryKeysLayout.Grid
                    Case TextEntryLayout.RealKeyboard
                        _ButtonTexts.AddRange(New String() {"q", "w", "e", "r", "t", "y", "u", "i", "o", "p", _
                                                            "a", "s", "d", "f", "g", "h", "j", "k", "l", _
                                                            "SH", "z", "x", "c", "v", "b", "n", "m", "BK", _
                                                            "AL", ",", "SP", ".", "OK"})
                        _ButtonTextsShift.AddRange(New String() {"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", _
                                                            "A", "S", "D", "F", "G", "H", "J", "K", "L", _
                                                            "", "Z", "X", "C", "V", "B", "N", "M", "", _
                                                            "", ",", "", ".", ""})
                        _ButtonTextsAlternate.AddRange(New String() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", _
                                                            "@", "#", "$", "%", "&", "*", "-", "+", "/", _
                                                            "", "(", ")", """", "'", ":", ";", "?", "", _
                                                            "", ",", "", ".", ""})
                        If Common.ProjectUseMultiByteChars Then
                            _ButtonTextsShiftAlternate.AddRange(New String() {"!", "\", "£", "|", "^", "§", "°", "{", "}", "=", _
                                                                "ç", "ä", "Ä", "ö", "Ö", "ü", "Ü", "ß", "/", _
                                                                "à", "[", "]", "è", "ì", "ò", "ù", "«", "", _
                                                                "", "€", "", ".", ""})
                        Else
                            _ButtonTextsShiftAlternate.AddRange(New String() {"!", "\", "", "|", "^", "", "", "{", "}", "=", _
                                                                "", "", "", "", "", "", "", "", "/", _
                                                                "", "[", "]", "", "", "", "", "", "", _
                                                                "", ",", "", ".", ""})
                        End If
                        '_CommandKeysIdx = {BK,SP,OK,SH,AL}
                        _CommandKeysIdx = {28, 31, 33, 20, 29, 0, 0, 0, 0}
                        _KeysHorizontal = 10
                        _KeysVertical = 4
                        _KeysLayout = TextEntryKeysLayout.KeyboardLike
                        _HorizontalSpacing = 10
                        _VerticalSpacing = 10
                        Me.Top = 0
                        Me.Left = 0
                End Select
                LoadKeysFromStrings()
                GenKeypad()
            End Set
        End Property

        <Description("Layout for the keys")> _
        <Category("TextEntryEx")> _
        Public Property KeysLayout As TextEntryKeysLayout
            Get
                Return _KeysLayout
            End Get
            Set(ByVal value As TextEntryKeysLayout)
                _KeysLayout = value
                GenKeypad()
            End Set
        End Property

#If PlayerMonolitico Then
        Public Property Keys As VGDDKeyCollection
#Else
        <Description("Texts for the key Buttons")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("TextEntryEx")> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
        <Editor(GetType(UiEditKeyCollection), GetType(System.Drawing.Design.UITypeEditor))> _
        Public Property Keys As VGDDKeyCollection
#End If
            Get
                Return _KeysCollection
            End Get
            Set(ByVal value As VGDDKeyCollection)
                _KeysCollection = value
                If Not Me.IsLoading Then
                    _TextLayout = TextEntryLayout.UserDefined
                End If
                LoadStringsFromKeys()
                GenKeypad()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ButtonTexts As Collections.Specialized.StringCollection
            Get
                Return _ButtonTexts
            End Get
            Set(ByVal value As Collections.Specialized.StringCollection)
                _ButtonTexts = value
                LoadKeysFromStrings()
                If Not Me.IsLoading Then
                    _TextLayout = TextEntryLayout.UserDefined
                End If
                GenKeypad()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ButtonTextsShift As Collections.Specialized.StringCollection
            Get
                Return _ButtonTextsShift
            End Get
            Set(ByVal value As Collections.Specialized.StringCollection)
                _ButtonTextsShift = value
                LoadKeysFromStrings()
                If Not Me.IsLoading Then
                    _TextLayout = TextEntryLayout.UserDefined
                End If
                GenKeypad()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ButtonTextsAlternate As Collections.Specialized.StringCollection
            Get
                Return _ButtonTextsAlternate
            End Get
            Set(ByVal value As Collections.Specialized.StringCollection)
                _ButtonTextsAlternate = value
                LoadKeysFromStrings()
                If Not Me.IsLoading Then
                    _TextLayout = TextEntryLayout.UserDefined
                End If
                GenKeypad()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ButtonTextsShiftAlternate As Collections.Specialized.StringCollection
            Get
                Return _ButtonTextsShiftAlternate
            End Get
            Set(ByVal value As Collections.Specialized.StringCollection)
                _ButtonTextsShiftAlternate = value
                LoadKeysFromStrings()
                If Not Me.IsLoading Then
                    _TextLayout = TextEntryLayout.UserDefined
                End If
                GenKeypad()
            End Set
        End Property

        <Description("Number of Vertical Keys")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("TextEntryEx")> _
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
        <Category("TextEntryEx")> _
        Public Property KeysHorizontal As Integer
            Get
                Return _KeysHorizontal
            End Get
            Set(ByVal value As Integer)
                _KeysHorizontal = value
                GenKeypad()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Font for the display ot this TextEntryEx")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDFontNameChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("TextEntryEx")> _
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
        <Category("TextEntryEx")> _
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

#Region "VGDDCode"

#If Not PlayerMonolitico Then

        Private Function GetKeys(ByVal oKeySet As Collections.Specialized.StringCollection) As String
            Dim strKeys As String = String.Empty
            Dim intIdx As Integer = 0
            Do While oKeySet.Count > 0 AndAlso oKeySet(oKeySet.Count - 1) = String.Empty
                oKeySet.RemoveAt(oKeySet.Count - 1)
            Loop
            For Each strButtonText As String In oKeySet
                If strButtonText Is Nothing Then
                    strButtonText = String.Empty
                End If
                If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                    If _Scheme.Font.SmartCharSet Then
                        _Scheme.Font.SmartCharSetAddString(strButtonText)
                    End If
                End If
                strKeys &= "    ," & CodeGen.QtextBinary(CodeGen.QText(strButtonText, _Scheme.Font, Nothing), String.Empty, " Button """ & strButtonText & """", intIdx, strButtonText.Length, "").TrimStart
                'If Common.ProjectUseMultiByteChars Then
                '    strKeys &= "," & CodeGen.QText(strButtonText, _Scheme.Font, Nothing) & " // Button """ & strButtonText & """" & vbCrLf & "    "
                'Else
                '    strKeys &= "," & CodeGen.QText(strButtonText, _Scheme.Font, Nothing)
                'End If
                intIdx += 1
            Next
            If strKeys.TrimStart.StartsWith(",") Then strKeys = strKeys.TrimStart.Substring(1)
            strKeys = strKeys.Replace("[NEWLINE]", vbCrLf) _
                            .Replace("[TAB]", "    ")
            Return strKeys
        End Function

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.TextDeclareCodeHeadTemplate(TextCDeclType.RamXcharArray).Replace("[CHARMAX]", "") ' DW do not generate _BufferLength sized array, use empty (str[]) insted
            '  Dim MyCodeHead As String = CodeGen.TextDeclareCodeHeadTemplate(TextCDeclType.RamXcharArray).Replace("[CHARMAX]", _BufferLength + 1) 
            Dim MyCode As String = "", MyState As String = ""
            Dim MyClassName As String = Me.GetType.ToString

            'Dim strPath As String = String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/CodeSub1", MyClassName.Split(".")(1))
            'MyCommandKeyTemplate = Space(4) & CodeGen.XmlExternalTemplatesDoc.SelectSingleNode(strPath).InnerText.Trim
            'MyCommandKeyCode = String.Empty

            Dim strKeysAlternate As String = GetKeys(_ButtonTextsAlternate)
            Dim strKeysShift As String = GetKeys(_ButtonTextsShift)
            Dim strKeysShiftAlternate As String = GetKeys(_ButtonTextsShiftAlternate)
            Dim strKeys As String = GetKeys(_ButtonTexts)

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
            CodeGen.AddState(MyState, "KeysLayout", Me.KeysLayout.ToString)

            Dim myText As String = ""
            Me.Text = Me.Text.PadRight(GetMaxTextLength(Me.TextStringID), "_") ' DW
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            If _DisplayFont = "" Then
                MessageBox.Show("TextEntryEx " & Me.Name & " on screen " & Me.Parent.Name & " is missing the mandatory DisplayFont property." & vbCrLf & "Compilation errors will happen if you don't fix this.", "Warning")
            End If

            Dim MyBitmapPressed As String = "NULL"
            If Me.BitmapPressedKey <> String.Empty Then
                MyBitmapPressed = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & Me.BitmapPressedKey
            End If

            Dim MyBitmapReleased As String = "NULL"
            If Me.BitmapReleasedKey <> String.Empty Then
                MyBitmapReleased = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & Me.BitmapReleasedKey
            End If

            Dim MyCommandKeyCode As String = String.Empty
            For i As Integer = 0 To _CommandKeysIdx.Length - 1
                MyCommandKeyCode &= ", " & _CommandKeysIdx(i)
            Next
            MyCommandKeyCode = "{ " & MyCommandKeyCode.Substring(2) & " }"

            _BufferLength = _TextBox.Text.Length  ' DW bufflength does not respect the string length => generated code array ist too small eg: GFX_XCHAR Password_TextEntryEx1_Text[11] = {0x005F,0x005F,0x005F,0x005F,0x005F,0x005F,0x005F,0x005F,0x005F,0x005F,0x005F,0x005F,0x0000}; // ____________

            Dim MyParameters() As Byte = {0,
                            CodeGen.Int2ByteH(_Radius), CodeGen.Int2ByteL(_Radius),
                            CodeGen.Int2ByteH(_ButtonTexts.Count), CodeGen.Int2ByteL(_ButtonTexts.Count),
                            CodeGen.Int2ByteH(_ButtonTextsAlternate.Count), CodeGen.Int2ByteL(_ButtonTextsAlternate.Count),
                            CodeGen.Int2ByteH(_ButtonTextsShift.Count), CodeGen.Int2ByteL(_ButtonTextsShift.Count),
                            CodeGen.Int2ByteH(_ButtonTextsShiftAlternate.Count), CodeGen.Int2ByteL(_ButtonTextsShiftAlternate.Count),
                            CodeGen.Int2ByteH(_KeysHorizontal), CodeGen.Int2ByteL(_KeysHorizontal),
                            CodeGen.Int2ByteH(_KeysVertical), CodeGen.Int2ByteL(_KeysVertical),
                            CodeGen.Int2ByteH(_BufferLength), CodeGen.Int2ByteL(_BufferLength),
                            CodeGen.Int2ByteH(_VerticalSpacing), CodeGen.Int2ByteL(_VerticalSpacing),
                            CodeGen.Int2ByteH(_HorizontalSpacing), CodeGen.Int2ByteL(_HorizontalSpacing),
                            0}
            MyParameters(0) = MyParameters.Length - 1
            Dim cs As Byte = 0
            Dim strMyParameters As String = String.Empty
            For i As Integer = 0 To MyParameters.Length - 1
                If i = MyParameters(0) Then
                    MyParameters(i) = cs
                Else
                    cs = cs Xor MyParameters(i)
                End If
                strMyParameters &= String.Format(",0x{0:x2}", MyParameters(i))
            Next
            strMyParameters = "(unsigned char []){" & strMyParameters.Substring(1) & "}"

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[PARAMETERS]", strMyParameters) _
                .Replace("[COMMANDKEYS]", MyCommandKeyCode) _
                .Replace("[DISPFONT]", _DisplayFont) _
                .Replace("[BITMAPPRESSED]", MyBitmapPressed) _
                .Replace("[BITMAPRELEASED]", MyBitmapReleased) _
                .Replace("[SCHEME]", Me.Scheme))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[KEYS]", strKeys) _
                .Replace("[KEYSALTERNATE]", strKeysAlternate) _
                .Replace("[KEYSSHIFT]", strKeysShift) _
                .Replace("[KEYSSHIFTALTERNATE]", strKeysShiftAlternate) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId) '_
            If Not CodeGen.HeadersIncludes.Contains(CodeGen.HeadersIncludesTemplate) Then ' #include "TextEntry.h"
                CodeGen.AddLines(CodeGen.HeadersIncludes, CodeGen.HeadersIncludesTemplate)
            End If
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

        Private Sub TextEntryEx_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                If _Scheme.Font.SmartCharSet Then
                    _Scheme.Font.SmartCharSetAddString(Me.Text)
                End If
            End If
            Me.Invalidate()
        End Sub

        Private Sub TextEntry_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            _IsNew = False
            LoadKeysFromStrings()
            'GenKeypad()
        End Sub

        Private Sub TextEntryEx_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            If Me.Parent IsNot Nothing Then
                GenKeypad()
            End If
        End Sub

        Private Sub TextEntryEx_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
            If _IsNew Then
                _IsNew = False
                If Me.Parent IsNot Nothing Then
                    Me.Width = Me.Parent.Width - 20
                    Me.Height = Me.Parent.Height - 20
                    Me.Left = 10
                    Me.Top = 10
                End If
            End If
        End Sub

        Private Sub TextEntry_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            If _IsNew Then
                If Not _Schemes.ContainsKey("TextEntryEx") Then
                    Dim oScheme As New VGDDScheme
                    With oScheme
                        .Color0 = Color.FromArgb(33, 36, 33)
                        .Color1 = Color.FromArgb(255, 255, 255)
                        .Colordisabled = Color.FromArgb(208, 224, 240)
                        .Commonbkcolor = Color.FromArgb(192, 0, 192)
                        .Embossdkcolor = Color.FromArgb(128, 128, 128)
                        .Embossltcolor = Color.FromArgb(224, 224, 224)
                        .Textcolor0 = Color.FromArgb(255, 255, 255)
                        .Textcolor1 = Color.FromArgb(0, 0, 0)
                        .Textcolordisabled = Color.FromArgb(33, 36, 33)
                        .GradientStartColor = Color.FromArgb(111, 110, 111)
                        .GradientEndColor = Color.FromArgb(75, 77, 75)
                        .GradientType = VGDDScheme.GFX_GRADIENT_TYPE.GRAD_DOWN
                        .Name = "TextEntryEx"
                        .Font = _Fonts(0)
                        MessageBox.Show("TextEntryEx scheme has been added to your schemes", "Info")
                    End With
                    _Schemes.Add(oScheme.Name, oScheme)
                End If
                Me.Scheme = "TextEntryEx"
            End If
            GenKeypad()
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class

End Namespace
