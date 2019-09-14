Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.IO
Imports System.Linq
Imports ICSharpCode.TextEditor
Imports ICSharpCode.TextEditor.Document

Namespace SourceEditor
    ''' <summary>Main form for a multi-file text editor based on 
    ''' ICSharpCode.TextEditor.TextEditorControl.</summary>
    Partial Public Class Editor
        Inherits UserControl

        Private _IsModified As Boolean
        Private WithEvents TextDocument As ICSharpCode.TextEditor.Document.IDocument

        Public Shadows Event TextChanged()

        ''' <summary>This variable holds the settings (whether to show line numbers, 
        ''' etc.) that all editor controls share.</summary>
        Private _editorSettings As ITextEditorProperties
        Private WithEvents oTextArea As TextArea

        Public Sub New()
            InitializeComponent()
            oTextArea = ActiveEditor.ActiveTextAreaControl.TextArea
        End Sub

        Private Function oTextArea_DoProcessDialogKey(keyData As Keys) As Boolean Handles oTextArea.DoProcessDialogKey
            Select Case keyData
                Case Keys.Control Or Keys.F
                    EditFind()
                    Return True
                Case Keys.Control Or Keys.R
                    EditReplace()
                    Return True
                Case Keys.Shift Or Keys.Control Or Keys.Z
                    ActiveEditor.Redo()
                    Return (True)
                Case Keys.Shift Or Keys.Control Or Keys.C
                    Dim oToggle As New ICSharpCode.TextEditor.Actions.ToggleComment
                    oToggle.Execute(oTextArea)
                    Return (True)

            End Select
        End Function

        Private Sub Editor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ActiveEditor.Dock = System.Windows.Forms.DockStyle.Fill
            ActiveEditor.IsReadOnly = False
            'AddHandler ActiveEditor.Document.DocumentChanged, New DocumentEventHandler(Function(fsender, fe)
            '                                                                               SetModifiedFlag(True)
            '                                                                               Return 0
            '                                                                           End Function)
            If _editorSettings Is Nothing Then
                _editorSettings = ActiveEditor.TextEditorProperties
            Else
                ActiveEditor.TextEditorProperties = _editorSettings
            End If
        End Sub

        Private Sub TextDocument_DocumentChanged(ByVal sender As Object, ByVal e As ICSharpCode.TextEditor.Document.DocumentEventArgs) Handles TextDocument.DocumentChanged
            SetModifiedFlag(True)
            RaiseEvent TextChanged()
        End Sub

#Region "File functions"

        Public Sub FileOpen()
            Dim openFileDialog As New OpenFileDialog
            openFileDialog.Multiselect = False
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                ' Try to open chosen file
                OpenFile(openFileDialog.FileName)
            End If
        End Sub

        Public Sub OpenFile(ByVal FileName As String)
            ' Open file(s)
            Try
                ActiveEditor.LoadFile(FileName)
                ' Modified flag is set during loading because the document 
                ' "changes" (from nothing to something). So, clear it again.
                SetModifiedFlag(False)
                TextDocument = ActiveEditor.Document
            Catch ex As Exception
                MessageBox.Show(ex.Message, ex.[GetType]().Name)
                Return
            End Try

            ' ICSharpCode.TextEditor doesn't have any built-in code folding
            ' strategies, so I've included a simple one. Apparently, the
            ' foldings are not updated automatically, so in this demo the user
            ' cannot add or remove folding regions after loading the file.
            ActiveEditor.Document.FoldingManager.FoldingStrategy = New RegionFoldingStrategy()
            ActiveEditor.Document.FoldingManager.UpdateFoldings(Nothing, Nothing)
        End Sub

        Public Function FileSave() As Boolean
            If ActiveEditor IsNot Nothing Then
                If String.IsNullOrEmpty(ActiveEditor.FileName) Then
                    Return FileSaveAs()
                Else
                    Try
                        ActiveEditor.SaveFile(ActiveEditor.FileName)
                        SetModifiedFlag(False)
                        Return True
                    Catch ex As Exception
                        MessageBox.Show(ex.Message, ex.[GetType]().Name)
                        Return False
                    End Try
                End If
            End If
            Return False
        End Function

        Private Function FileSaveAs() As Boolean
            If ActiveEditor IsNot Nothing Then
                Dim saveFileDialog As New SaveFileDialog
                saveFileDialog.FileName = ActiveEditor.FileName
                If saveFileDialog.ShowDialog() = DialogResult.OK Then
                    Try
                        ActiveEditor.SaveFile(saveFileDialog.FileName)
                        SetModifiedFlag(False)

                        ' The syntax highlighting strategy doesn't change
                        ' automatically, so do it manually.
                        ActiveEditor.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(ActiveEditor.FileName)
                        Return True
                    Catch ex As Exception
                        MessageBox.Show(ex.Message, ex.[GetType]().Name)
                    End Try
                End If
            End If
            Return False
        End Function

#End Region

#Region "Code related to Edit menu"

        ''' <summary>Performs an action encapsulated in IEditAction.</summary>
        ''' <remarks>
        ''' There is an implementation of IEditAction for every action that 
        ''' the user can invoke using a shortcut key (arrow keys, Ctrl+X, etc.)
        ''' The editor control doesn't provide a public funciton to perform one
        ''' of these actions directly, so I wrote DoEditAction() based on the
        ''' code in TextArea.ExecuteDialogKey(). You can call ExecuteDialogKey
        ''' directly, but it is more fragile because it takes a Keys value (e.g.
        ''' Keys.Left) instead of the action to perform.
        ''' <para/>
        ''' Clipboard commands could also be done by calling methods in
        ''' editor.ActiveTextAreaControl.TextArea.ClipboardHandler.
        ''' </remarks>
        Private Sub DoEditAction(ByVal editor As TextEditorControl, ByVal action As ICSharpCode.TextEditor.Actions.IEditAction)
            If editor IsNot Nothing AndAlso action IsNot Nothing Then
                Dim area = editor.ActiveTextAreaControl.TextArea
                editor.BeginUpdate()
                Try
                    SyncLock editor.Document
                        action.Execute(area)
                        If area.SelectionManager.HasSomethingSelected AndAlso area.AutoClearSelection Then
                            '&& caretchanged
                            If area.Document.TextEditorProperties.DocumentSelectionMode = DocumentSelectionMode.Normal Then
                                area.SelectionManager.ClearSelection()
                            End If
                        End If
                    End SyncLock
                Finally
                    editor.EndUpdate()
                    area.Caret.UpdateCaretPosition()
                End Try
            End If
        End Sub

        Public Sub EditCut()
            If HaveSelection() Then
                DoEditAction(ActiveEditor, New ICSharpCode.TextEditor.Actions.Cut())
            End If
        End Sub
        Public Sub menuEditCopy()
            If HaveSelection() Then
                DoEditAction(ActiveEditor, New ICSharpCode.TextEditor.Actions.Copy())
            End If
        End Sub
        Public Sub menuEditPaste()
            DoEditAction(ActiveEditor, New ICSharpCode.TextEditor.Actions.Paste())
        End Sub
        Public Sub EditDelete()
            If HaveSelection() Then
                DoEditAction(ActiveEditor, New ICSharpCode.TextEditor.Actions.Delete())
            End If
        End Sub

        Public ReadOnly Property HaveSelection() As Boolean
            Get
                Return ActiveEditor IsNot Nothing AndAlso ActiveEditor.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected
            End Get
        End Property

        Private _findForm As New FindAndReplaceForm()

        Public Sub EditFind()
            If ActiveEditor Is Nothing Then
                Return
            End If
            _findForm.ShowFor(ActiveEditor, False)
        End Sub

        Public Sub EditReplace()
            If ActiveEditor Is Nothing Then
                Return
            End If
            _findForm.ShowFor(ActiveEditor, True)
        End Sub

        Public Sub FindAgain()
            _findForm.FindNext(True, False, String.Format("Search text «{0}» not found.", _findForm.LookFor))
        End Sub
        Public Sub FindAgainReverse()
            _findForm.FindNext(True, True, String.Format("Search text «{0}» not found.", _findForm.LookFor))
        End Sub

        Public Sub ToggleBookmark()
            If ActiveEditor IsNot Nothing Then
                DoEditAction(ActiveEditor, New ICSharpCode.TextEditor.Actions.ToggleBookmark())
                ActiveEditor.IsIconBarVisible = ActiveEditor.Document.BookmarkManager.Marks.Count > 0
            End If
        End Sub

        Public Sub GoToNextBookmark()
            DoEditAction(ActiveEditor, New ICSharpCode.TextEditor.Actions.GotoNextBookmark(Function(bookmark) True))
        End Sub

        Public Sub GoToPrevBookmark()
            DoEditAction(ActiveEditor, New ICSharpCode.TextEditor.Actions.GotoPrevBookmark(Function(bookmark) True))
        End Sub

#End Region

#Region "Code related to Options menu"

        Public Sub SplitTextArea()
            If ActiveEditor Is Nothing Then
                Return
            End If
            ActiveEditor.Split()
        End Sub

        Public Sub ToggleShowSpaces()
            If ActiveEditor Is Nothing Then
                Return
            End If
            ActiveEditor.ShowSpaces = InlineAssignHelper(ActiveEditor.ShowTabs, Not ActiveEditor.ShowSpaces)
        End Sub

        Public Sub ToggleShowNewlines()
            If ActiveEditor Is Nothing Then
                Return
            End If
            ActiveEditor.ShowEOLMarkers = Not ActiveEditor.ShowEOLMarkers
        End Sub

        Public Sub ToggleHighlightCurrentRow()
            If ActiveEditor Is Nothing Then
                Return
            End If
            ActiveEditor.LineViewerStyle = If(ActiveEditor.LineViewerStyle = LineViewerStyle.None, LineViewerStyle.FullRow, LineViewerStyle.None)
        End Sub

        Public Sub ToggleBracketMatchingStyle()
            If ActiveEditor Is Nothing Then
                Return
            End If
            ActiveEditor.BracketMatchingStyle = If(ActiveEditor.BracketMatchingStyle = BracketMatchingStyle.After, BracketMatchingStyle.Before, BracketMatchingStyle.After)
        End Sub

        Public Sub ToggleEnableVirtualSpace()
            If ActiveEditor Is Nothing Then
                Return
            End If
            ActiveEditor.AllowCaretBeyondEOL = Not ActiveEditor.AllowCaretBeyondEOL
        End Sub

        Public Sub ToggleShowLineNumbers()
            If ActiveEditor Is Nothing Then
                Return
            End If
            ActiveEditor.ShowLineNumbers = Not ActiveEditor.ShowLineNumbers
        End Sub

        Public Sub SetTabSize()
            If ActiveEditor IsNot Nothing Then
                Dim result As String = InputBox("Specify the desired tab width.", "Tab size", _editorSettings.TabIndent.ToString())
                Dim value As Integer
                If result IsNot Nothing AndAlso Integer.TryParse(result, value) AndAlso value.IsInRange(1, 32) Then
                    ActiveEditor.TabIndent = value
                End If
            End If
        End Sub

        Public Sub SetFont()
            If ActiveEditor IsNot Nothing Then
                Dim fontDialog As New FontDialog
                'fontDialog.ShowEffects = False
                fontDialog.Font = ActiveEditor.Font
                If fontDialog.ShowDialog(Me) = DialogResult.OK Then
                    ActiveEditor.Font = fontDialog.Font
                End If
            End If
        End Sub

#End Region

#Region "Other stuff"
        Protected Overloads Overrides Sub OnCreateControl()
            MyBase.OnCreateControl()
            If Not Me.DesignMode Then
                AddHandler Me.ParentForm.FormClosing, AddressOf ParentForm_FormClosing
            End If
        End Sub

        Private Sub ParentForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            ' Ask user to save changes
            If IsModified Then
                Dim r = MessageBox.Show(String.Format("Save changes to {0}?", If(ActiveEditor.FileName, "new file")), "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                If r = DialogResult.Cancel Then
                    e.Cancel = True
                ElseIf r = DialogResult.Yes Then
                    If Not FileSave() Then
                        e.Cancel = True
                    End If
                End If
            End If
        End Sub

        ''' <summary>Gets whether the file in the specified editor is modified.</summary>
        ''' <remarks>TextEditorControl doesn't maintain its own internal modified 
        ''' flag, so we use the '*' shown after the file name to represent the 
        ''' modified state.</remarks>
        Public ReadOnly Property IsModified As Boolean
            Get
                Return _IsModified
            End Get
        End Property

        Private Sub SetModifiedFlag(ByVal flag As Boolean)
            _IsModified = flag
        End Sub

        ''' <summary>We handle DragEnter and DragDrop so users can drop files on the editor.</summary>
        Private Sub TextEditorForm_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles Me.DragEnter
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effect = DragDropEffects.Copy
            End If
        End Sub
        Private Sub TextEditorForm_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles Me.DragDrop
            Dim list As String() = TryCast(e.Data.GetData(DataFormats.FileDrop), String())
            If list IsNot Nothing AndAlso list.Count = 1 Then
                OpenFile(list(0))
            End If
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

#End Region

    End Class

    ''' <summary>
    ''' The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
    ''' </summary>
    Public Class RegionFoldingStrategy
        Implements IFoldingStrategy

        ''' <summary>
        ''' Generates the foldings for our document.
        ''' </summary>
        ''' <param name="document">The current document.</param>
        ''' <param name="fileName">The filename of the document.</param>
        ''' <param name="parseInformation">Extra parse information, not used in this sample.</param>
        ''' <returns>A list of FoldMarkers.</returns>
        Public Function GenerateFoldMarkers(ByVal document As IDocument, ByVal fileName As String, ByVal parseInformation As Object) As List(Of FoldMarker) Implements ICSharpCode.TextEditor.Document.IFoldingStrategy.GenerateFoldMarkers
            Dim list As New List(Of FoldMarker)()

            Dim startLines As New Stack(Of Integer)()

            ' Create foldmarkers for the whole document, enumerate through every line.
            For i As Integer = 0 To document.TotalNumberOfLines - 1
                Dim seg = document.GetLineSegment(i)
                Dim offs As Integer, [end] As Integer = document.TextLength
                Dim c As Char
                offs = seg.Offset
                While offs < [end] AndAlso ((InlineAssignHelper(c, document.GetCharAt(offs))) = " "c OrElse c = ControlChars.Tab)
                    offs += 1
                End While
                If offs = [end] Then
                    Exit For
                End If
                Dim spaceCount As Integer = offs - seg.Offset

                ' now offs points to the first non-whitespace char on the line
                If document.GetCharAt(offs) = "#"c Then
                    Dim text As String = document.GetText(offs, seg.Length - spaceCount)
                    If text.StartsWith("#region") Then
                        startLines.Push(i)
                    End If
                    If text.StartsWith("#endregion") AndAlso startLines.Count > 0 Then
                        ' Add a new FoldMarker to the list.
                        Dim start As Integer = startLines.Pop()
                        list.Add(New FoldMarker(document, start, document.GetLineSegment(start).Length, i, spaceCount + "#endregion".Length))
                    End If
                End If
            Next

            Return list
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

    End Class
End Namespace

