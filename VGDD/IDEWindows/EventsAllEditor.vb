Imports System.Xml
Imports VGDD
Imports System.ComponentModel
Imports VGDDCommon

Namespace VGDDIDE
    Public Class EventsAllEditor
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        Public Sub New()
            Me.InitializeComponent()
            Me.TextEditorControl1.ActiveEditor.Document.HighlightingStrategy() =
                    ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("MCHP_C")
            Me.TextEditorControl1.Enabled = True
        End Sub

        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EventsAllEditor))
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
            Me.SaveToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
            Me.CutToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.CopyToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.PasteToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
            Me.UndoToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.RedoToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.FindToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.ReplaceToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
            Me.HelpToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.TextEditorControl1 = New SourceEditor.Editor()
            Me.ToolStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'ToolStrip1
            '
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveToolStripButton, Me.toolStripSeparator, Me.CutToolStripButton, Me.CopyToolStripButton, Me.PasteToolStripButton, Me.ToolStripSeparator3, Me.UndoToolStripButton, Me.RedoToolStripButton, Me.toolStripSeparator1, Me.FindToolStripButton, Me.ReplaceToolStripButton, Me.ToolStripSeparator2, Me.HelpToolStripButton})
            Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
            Me.ToolStrip1.Name = "ToolStrip1"
            Me.ToolStrip1.Size = New System.Drawing.Size(835, 25)
            Me.ToolStrip1.TabIndex = 3
            Me.ToolStrip1.Text = "ToolStrip1"
            '
            'SaveToolStripButton
            '
            Me.SaveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.SaveToolStripButton.Image = CType(resources.GetObject("SaveToolStripButton.Image"), System.Drawing.Image)
            Me.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.SaveToolStripButton.Name = "SaveToolStripButton"
            Me.SaveToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.SaveToolStripButton.Text = "&Save"
            '
            'toolStripSeparator
            '
            Me.toolStripSeparator.Name = "toolStripSeparator"
            Me.toolStripSeparator.Size = New System.Drawing.Size(6, 25)
            '
            'CutToolStripButton
            '
            Me.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.CutToolStripButton.Image = CType(resources.GetObject("CutToolStripButton.Image"), System.Drawing.Image)
            Me.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.CutToolStripButton.Name = "CutToolStripButton"
            Me.CutToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.CutToolStripButton.Text = "C&ut"
            '
            'CopyToolStripButton
            '
            Me.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.CopyToolStripButton.Image = CType(resources.GetObject("CopyToolStripButton.Image"), System.Drawing.Image)
            Me.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.CopyToolStripButton.Name = "CopyToolStripButton"
            Me.CopyToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.CopyToolStripButton.Text = "&Copy"
            '
            'PasteToolStripButton
            '
            Me.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.PasteToolStripButton.Image = CType(resources.GetObject("PasteToolStripButton.Image"), System.Drawing.Image)
            Me.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.PasteToolStripButton.Name = "PasteToolStripButton"
            Me.PasteToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.PasteToolStripButton.Text = "&Paste"
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
            '
            'UndoToolStripButton
            '
            Me.UndoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.UndoToolStripButton.Image = Global.My.Resources.Resources.arrow_rotate_anticlockwisered
            Me.UndoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.UndoToolStripButton.Name = "UndoToolStripButton"
            Me.UndoToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.UndoToolStripButton.Text = "&Undo"
            Me.UndoToolStripButton.ToolTipText = "Undo"
            '
            'RedoToolStripButton
            '
            Me.RedoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.RedoToolStripButton.Image = Global.My.Resources.Resources.arrow_rotate_clockwise
            Me.RedoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.RedoToolStripButton.Name = "RedoToolStripButton"
            Me.RedoToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.RedoToolStripButton.Text = "&Redo"
            Me.RedoToolStripButton.ToolTipText = "Redo"
            '
            'toolStripSeparator1
            '
            Me.toolStripSeparator1.Name = "toolStripSeparator1"
            Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 25)
            '
            'FindToolStripButton
            '
            Me.FindToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.FindToolStripButton.Image = Global.My.Resources.Resources.Zoom
            Me.FindToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.FindToolStripButton.Name = "FindToolStripButton"
            Me.FindToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.FindToolStripButton.Text = "&Find"
            Me.FindToolStripButton.ToolTipText = "Find"
            '
            'ReplaceToolStripButton
            '
            Me.ReplaceToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ReplaceToolStripButton.Image = Global.My.Resources.Resources.Windows_Magnifier
            Me.ReplaceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ReplaceToolStripButton.Name = "ReplaceToolStripButton"
            Me.ReplaceToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.ReplaceToolStripButton.Text = "&Replace"
            Me.ReplaceToolStripButton.ToolTipText = "Replace"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'HelpToolStripButton
            '
            Me.HelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.HelpToolStripButton.Image = CType(resources.GetObject("HelpToolStripButton.Image"), System.Drawing.Image)
            Me.HelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.HelpToolStripButton.Name = "HelpToolStripButton"
            Me.HelpToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.HelpToolStripButton.Text = "He&lp"
            '
            'TextEditorControl1
            '
            Me.TextEditorControl1.AllowDrop = True
            Me.TextEditorControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextEditorControl1.Enabled = False
            Me.TextEditorControl1.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TextEditorControl1.Location = New System.Drawing.Point(3, 29)
            Me.TextEditorControl1.Margin = New System.Windows.Forms.Padding(4)
            Me.TextEditorControl1.Name = "TextEditorControl1"
            Me.TextEditorControl1.Size = New System.Drawing.Size(831, 230)
            Me.TextEditorControl1.TabIndex = 1
            '
            'EventsAllEditor
            '
            Me.ClientSize = New System.Drawing.Size(835, 262)
            Me.Controls.Add(Me.ToolStrip1)
            Me.Controls.Add(Me.TextEditorControl1)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "EventsAllEditor"
            Me.Text = "All Events Editor"
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TextEditorControl1 As SourceEditor.Editor

        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents SaveToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents toolStripSeparator As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents CutToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents CopyToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents PasteToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents HelpToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents UndoToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents RedoToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents FindToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ReplaceToolStripButton As System.Windows.Forms.ToolStripButton

        Private Sub FindToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindToolStripButton.Click
            TextEditorControl1.EditFind()
        End Sub

        Private Sub ReplaceToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplaceToolStripButton.Click
            TextEditorControl1.EditReplace()
        End Sub

        Private Sub UndoToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UndoToolStripButton.Click
            TextEditorControl1.ActiveEditor.Undo()
        End Sub

        Private Sub RedoToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RedoToolStripButton.Click
            TextEditorControl1.ActiveEditor.Redo()
        End Sub

        Private Sub PasteToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteToolStripButton.Click
            TextEditorControl1.menuEditPaste()
        End Sub

        Private Sub CopyToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripButton.Click
            TextEditorControl1.menuEditCopy()
        End Sub

        Private Sub CutToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CutToolStripButton.Click
            TextEditorControl1.EditCut()
        End Sub

        Private Sub SaveToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripButton.Click
            oMainShell.AllEventsFromXML(TextEditorControl1.ActiveEditor.Document.TextContent)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class
End Namespace