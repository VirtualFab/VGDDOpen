Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports ICSharpCode.TextEditor.Document
Imports ICSharpCode.TextEditor
Imports System.Diagnostics
Imports System.IO

Namespace SourceEditor
    Partial Public Class FindAndReplaceForm
        Inherits Form
        Public Sub New()
            InitializeComponent()
            _search = New TextEditorSearcher()
        End Sub

        Private _search As TextEditorSearcher
        Private _editor As TextEditorControl
        Private Property Editor() As TextEditorControl
            Get
                Return _editor
            End Get
            Set(ByVal value As TextEditorControl)
                _editor = value
                _search.Document = _editor.Document
                UpdateTitleBar()
            End Set
        End Property

        Private Sub UpdateTitleBar()
            Dim text As String = If(ReplaceMode, "Find & replace", "Find")
            If _editor IsNot Nothing AndAlso _editor.FileName IsNot Nothing Then
                text += " - " & Path.GetFileName(_editor.FileName)
            End If
            If _search.HasScanRegion Then
                text += " (selection only)"
            End If
            Me.Text = text
        End Sub

        Public Sub ShowFor(ByVal editor__1 As TextEditorControl, ByVal replaceMode__2 As Boolean)
            Editor = editor__1

            _search.ClearScanRegion()
            Dim sm = editor__1.ActiveTextAreaControl.SelectionManager
            If sm.HasSomethingSelected AndAlso sm.SelectionCollection.Count = 1 Then
                Dim sel = sm.SelectionCollection(0)
                If sel.StartPosition.Line = sel.EndPosition.Line Then
                    txtLookFor.Text = sm.SelectedText
                Else
                    _search.SetScanRegion(sel)
                End If
            Else
                ' Get the current word that the caret is on
                Dim caret As Caret = editor__1.ActiveTextAreaControl.Caret
                Dim start As Integer = TextUtilities.FindWordStart(editor__1.Document, caret.Offset)
                Dim endAt As Integer = TextUtilities.FindWordEnd(editor__1.Document, caret.Offset)
                txtLookFor.Text = editor__1.Document.GetText(start, endAt - start)
            End If

            ReplaceMode = replaceMode__2

            Me.Owner = DirectCast(editor__1.TopLevelControl, Form)
            Me.Show()

            txtLookFor.SelectAll()
            txtLookFor.Focus()
        End Sub

        Public Property ReplaceMode() As Boolean
            Get
                Return txtReplaceWith.Visible
            End Get
            Set(ByVal value As Boolean)
                btnReplace.Visible = InlineAssignHelper(btnReplaceAll.Visible, value)
                lblReplaceWith.Visible = InlineAssignHelper(txtReplaceWith.Visible, value)
                btnHighlightAll.Visible = Not value
                Me.AcceptButton = If(value, btnReplace, btnFindNext)
                UpdateTitleBar()
            End Set
        End Property

        Private Sub btnFindPrevious_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnFindPrevious.Click
            FindNext(False, True, "Text not found")
        End Sub
        Private Sub btnFindNext_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnFindNext.Click
            FindNext(False, False, "Text not found")
        End Sub

        Public _lastSearchWasBackward As Boolean = False
        Public _lastSearchLoopedAround As Boolean

        Public Function FindNext(ByVal viaF3 As Boolean, ByVal searchBackward As Boolean, ByVal messageIfNotFound As String) As TextRange
            If String.IsNullOrEmpty(txtLookFor.Text) Then
                MessageBox.Show("No string specified to look for!")
                Return Nothing
            End If
            _lastSearchWasBackward = searchBackward
            _search.LookFor = txtLookFor.Text
            _search.MatchCase = chkMatchCase.Checked
            _search.MatchWholeWordOnly = chkMatchWholeWord.Checked

            Dim caret = _editor.ActiveTextAreaControl.Caret
            If viaF3 AndAlso _search.HasScanRegion AndAlso Not caret.Offset.IsInRange(_search.BeginOffset, _search.EndOffset) Then
                ' user moved outside of the originally selected region
                _search.ClearScanRegion()
                UpdateTitleBar()
            End If

            Dim startFrom As Integer = caret.Offset - (If(searchBackward, 1, 0))
            Dim range As TextRange = _search.FindNext(startFrom, searchBackward, _lastSearchLoopedAround)
            If range IsNot Nothing Then
                SelectResult(range)
            ElseIf messageIfNotFound IsNot Nothing Then
                MessageBox.Show(messageIfNotFound)
            End If
            Return range
        End Function

        Private Sub SelectResult(ByVal range As TextRange)
            Dim p1 As TextLocation = _editor.Document.OffsetToPosition(range.Offset)
            Dim p2 As TextLocation = _editor.Document.OffsetToPosition(range.Offset + range.Length)
            _editor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2)
            _editor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column)
            ' Also move the caret to the end of the selection, because when the user 
            ' presses F3, the caret is where we start searching next time.
            _editor.ActiveTextAreaControl.Caret.Position = _editor.Document.OffsetToPosition(range.Offset + range.Length)
        End Sub

        Private _highlightGroups As New Dictionary(Of TextEditorControl, HighlightGroup)()

        Private Sub btnHighlightAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnHighlightAll.Click

        End Sub

        Private Sub FindAndReplaceForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
            ' Prevent dispose, as this form can be re-used
            If e.CloseReason <> CloseReason.FormOwnerClosing Then
                If Me.Owner IsNot Nothing Then
                    Me.Owner.[Select]()
                End If
                ' prevent another app from being activated instead
                e.Cancel = True
                Hide()

                ' Discard search region
                _search.ClearScanRegion()
                ' must repaint manually
                _editor.Refresh()
            End If
        End Sub



        Private Sub btnReplace_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnReplace.Click

        End Sub

        Private Sub btnReplaceAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnReplaceAll.Click
            Dim count As Integer = 0
            ' BUG FIX: if the replacement string contains the original search string
            ' (e.g. replace "red" with "very red") we must avoid looping around and
            ' replacing forever! To fix, start replacing at beginning of region (by 
            ' moving the caret) and stop as soon as we loop around.
            _editor.ActiveTextAreaControl.Caret.Position = _editor.Document.OffsetToPosition(_search.BeginOffset)

            _editor.Document.UndoStack.StartUndoGroup()
            Try
                While FindNext(False, False, Nothing) IsNot Nothing
                    If _lastSearchLoopedAround Then
                        Exit While
                    End If

                    ' Replace
                    count += 1
                    InsertText(txtReplaceWith.Text)
                End While
            Finally
                _editor.Document.UndoStack.EndUndoGroup()
            End Try
            If count = 0 Then
                MessageBox.Show("No occurrances found.")
            Else
                MessageBox.Show(String.Format("Replaced {0} occurrances.", count))
                Close()
            End If
        End Sub

        Private Sub InsertText(ByVal text As String)
            Dim textArea = _editor.ActiveTextAreaControl.TextArea
            textArea.Document.UndoStack.StartUndoGroup()
            Try
                If textArea.SelectionManager.HasSomethingSelected Then
                    textArea.Caret.Position = textArea.SelectionManager.SelectionCollection(0).StartPosition
                    textArea.SelectionManager.RemoveSelectedText()
                End If
                textArea.InsertString(text)
            Finally
                textArea.Document.UndoStack.EndUndoGroup()
            End Try
        End Sub

        Public ReadOnly Property LookFor() As String
            Get
                Return txtLookFor.Text
            End Get
        End Property
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

    End Class

    Public Class TextRange
        Inherits AbstractSegment
        Private _document As IDocument
        Public Sub New(ByVal document As IDocument, ByVal offset As Integer, ByVal length As Integer)
            _document = document
            Me.Offset = offset
            Me.Length = length
        End Sub
    End Class

    ''' <summary>This class finds occurrances of a search string in a text 
    ''' editor's IDocument... it's like Find box without a GUI.</summary>
    Public Class TextEditorSearcher
        Implements IDisposable
        Private _document As IDocument
        Public Property Document() As IDocument
            Get
                Return _document
            End Get
            Set(ByVal value As IDocument)
                If _document IsNot value Then
                    ClearScanRegion()
                    _document = value
                End If
            End Set
        End Property

        ' I would have used the TextAnchor class to represent the beginning and 
        ' end of the region to scan while automatically adjusting to changes in 
        ' the document--but for some reason it is sealed and its constructor is 
        ' internal. Instead I use a TextMarker, which is perhaps even better as 
        ' it gives me the opportunity to highlight the region. Note that all the 
        ' markers and coloring information is associated with the text document, 
        ' not the editor control, so TextEditorSearcher doesn't need a reference 
        ' to the TextEditorControl. After adding the marker to the document, we
        ' must remember to remove it when it is no longer needed.
        Private _region As TextMarker = Nothing
        ''' <summary>Sets the region to search. The region is updated 
        ''' automatically as the document changes.</summary>
        Public Sub SetScanRegion(ByVal sel As ISelection)
            SetScanRegion(sel.Offset, sel.Length)
        End Sub
        ''' <summary>Sets the region to search. The region is updated 
        ''' automatically as the document changes.</summary>
        Public Sub SetScanRegion(ByVal offset As Integer, ByVal length As Integer)
            Dim bkgColor = _document.HighlightingStrategy.GetColorFor("Default").BackgroundColor
            _region = New TextMarker(offset, length, TextMarkerType.SolidBlock, bkgColor.HalfMix(Color.FromArgb(160, 160, 160)))
            _document.MarkerStrategy.AddMarker(_region)
        End Sub
        Public ReadOnly Property HasScanRegion() As Boolean
            Get
                Return _region IsNot Nothing
            End Get
        End Property
        Public Sub ClearScanRegion()
            If _region IsNot Nothing Then
                _document.MarkerStrategy.RemoveMarker(_region)
                _region = Nothing
            End If
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            ClearScanRegion()
            GC.SuppressFinalize(Me)
        End Sub
        Protected Overrides Sub Finalize()
            Try
                Dispose()
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        ''' <summary>Begins the start offset for searching</summary>
        Public ReadOnly Property BeginOffset() As Integer
            Get
                If _region IsNot Nothing Then
                    Return _region.Offset
                Else
                    Return 0
                End If
            End Get
        End Property
        ''' <summary>Begins the end offset for searching</summary>
        Public ReadOnly Property EndOffset() As Integer
            Get
                If _region IsNot Nothing Then
                    Return _region.EndOffset
                Else
                    Return _document.TextLength
                End If
            End Get
        End Property

        Public MatchCase As Boolean

        Public MatchWholeWordOnly As Boolean

        Private _lookFor As String
        Private _lookFor2 As String
        ' uppercase in case-insensitive mode
        Public Property LookFor() As String
            Get
                Return _lookFor
            End Get
            Set(ByVal value As String)
                _lookFor = value
            End Set
        End Property

        ''' <summary>Finds next instance of LookFor, according to the search rules 
        ''' (MatchCase, MatchWholeWordOnly).</summary>
        ''' <param name="beginAtOffset">Offset in Document at which to begin the search</param>
        ''' <remarks>If there is a match at beginAtOffset precisely, it will be returned.</remarks>
        ''' <returns>Region of document that matches the search string</returns>
        Public Function FindNext(ByVal beginAtOffset As Integer, ByVal searchBackward As Boolean, ByRef loopedAround As Boolean) As TextRange
            Debug.Assert(Not String.IsNullOrEmpty(_lookFor))
            loopedAround = False

            Dim startAt As Integer = BeginOffset, endAt As Integer = EndOffset
            Dim curOffs As Integer = beginAtOffset.InRange(startAt, endAt)

            _lookFor2 = If(MatchCase, _lookFor, _lookFor.ToUpperInvariant())

            Dim result As TextRange
            If searchBackward Then
                result = FindNextIn(startAt, curOffs, True)
                If result Is Nothing Then
                    loopedAround = True
                    result = FindNextIn(curOffs, endAt, True)
                End If
            Else
                result = FindNextIn(curOffs, endAt, False)
                If result Is Nothing Then
                    loopedAround = True
                    result = FindNextIn(startAt, curOffs, False)
                End If
            End If
            Return result
        End Function

        Private Function FindNextIn(ByVal offset1 As Integer, ByVal offset2 As Integer, ByVal searchBackward As Boolean) As TextRange
            Debug.Assert(offset2 >= offset1)
            offset2 -= _lookFor.Length

            ' Make behavior decisions before starting search loop
            Dim matchFirstCh As Func(Of Char, Char, Boolean)
            Dim matchWord As Func(Of Integer, Boolean)
            If MatchCase Then
                matchFirstCh = Function(lookFor, c) (lookFor = c)
            Else
                matchFirstCh = Function(lookFor, c) (lookFor = [Char].ToUpperInvariant(c))
            End If
            If MatchWholeWordOnly Then
                matchWord = AddressOf IsWholeWordMatch
            Else
                matchWord = AddressOf IsPartWordMatch
            End If

            ' Search
            Dim lookForCh As Char = _lookFor2(0)
            If searchBackward Then
                For offset As Integer = offset2 To offset1 Step -1
                    If matchFirstCh(lookForCh, _document.GetCharAt(offset)) AndAlso matchWord(offset) Then
                        Return New TextRange(_document, offset, _lookFor.Length)
                    End If
                Next
            Else
                For offset As Integer = offset1 To offset2
                    If matchFirstCh(lookForCh, _document.GetCharAt(offset)) AndAlso matchWord(offset) Then
                        Return New TextRange(_document, offset, _lookFor.Length)
                    End If
                Next
            End If
            Return Nothing
        End Function
        Private Function IsWholeWordMatch(ByVal offset As Integer) As Boolean
            If IsWordBoundary(offset) AndAlso IsWordBoundary(offset + _lookFor.Length) Then
                Return IsPartWordMatch(offset)
            Else
                Return False
            End If
        End Function
        Private Function IsWordBoundary(ByVal offset As Integer) As Boolean
            Return offset <= 0 OrElse offset >= _document.TextLength OrElse Not IsAlphaNumeric(offset - 1) OrElse Not IsAlphaNumeric(offset)
        End Function
        Private Function IsAlphaNumeric(ByVal offset As Integer) As Boolean
            Dim c As Char = _document.GetCharAt(offset)
            Return [Char].IsLetterOrDigit(c) OrElse c = "_"c
        End Function
        Private Function IsPartWordMatch(ByVal offset As Integer) As Boolean
            Dim substr As String = _document.GetText(offset, _lookFor.Length)
            If Not MatchCase Then
                substr = substr.ToUpperInvariant()
            End If
            Return substr = _lookFor2
        End Function
    End Class

    ''' <summary>Bundles a group of markers together so that they can be cleared 
    ''' together.</summary>
    Public Class HighlightGroup
        Implements IDisposable
        Private _markers As New List(Of TextMarker)()
        Private _editor As TextEditorControl
        Private _document As IDocument
        Public Sub New(ByVal editor As TextEditorControl)
            _editor = editor
            _document = editor.Document
        End Sub
        Public Sub AddMarker(ByVal marker As TextMarker)
            _markers.Add(marker)
            _document.MarkerStrategy.AddMarker(marker)
        End Sub
        Public Sub ClearMarkers()
            For Each m As TextMarker In _markers
                _document.MarkerStrategy.RemoveMarker(m)
            Next
            _markers.Clear()
            _editor.Refresh()
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            ClearMarkers()
            GC.SuppressFinalize(Me)
        End Sub
        Protected Overrides Sub Finalize()
            Try
                Dispose()
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        Public ReadOnly Property Markers() As IList(Of TextMarker)
            Get
                Return _markers.AsReadOnly()
            End Get
        End Property
    End Class
End Namespace
