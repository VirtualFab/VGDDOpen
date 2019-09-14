Namespace SourceEditor
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class FindAndReplaceForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.chkMatchCase = New System.Windows.Forms.CheckBox()
            Me.chkMatchWholeWord = New System.Windows.Forms.CheckBox()
            Me.btnReplaceAll = New System.Windows.Forms.Button()
            Me.btnReplace = New System.Windows.Forms.Button()
            Me.btnHighlightAll = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.btnFindPrevious = New System.Windows.Forms.Button()
            Me.btnFindNext = New System.Windows.Forms.Button()
            Me.txtReplaceWith = New System.Windows.Forms.TextBox()
            Me.txtLookFor = New System.Windows.Forms.TextBox()
            Me.lblReplaceWith = New System.Windows.Forms.Label()
            Me.label1 = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'chkMatchCase
            '
            Me.chkMatchCase.AutoSize = True
            Me.chkMatchCase.Location = New System.Drawing.Point(84, 57)
            Me.chkMatchCase.Name = "chkMatchCase"
            Me.chkMatchCase.Size = New System.Drawing.Size(82, 17)
            Me.chkMatchCase.TabIndex = 14
            Me.chkMatchCase.Text = "Match &case"
            Me.chkMatchCase.UseVisualStyleBackColor = True
            '
            'chkMatchWholeWord
            '
            Me.chkMatchWholeWord.AutoSize = True
            Me.chkMatchWholeWord.Location = New System.Drawing.Point(172, 57)
            Me.chkMatchWholeWord.Name = "chkMatchWholeWord"
            Me.chkMatchWholeWord.Size = New System.Drawing.Size(113, 17)
            Me.chkMatchWholeWord.TabIndex = 15
            Me.chkMatchWholeWord.Text = "Match &whole word"
            Me.chkMatchWholeWord.UseVisualStyleBackColor = True
            '
            'btnReplaceAll
            '
            Me.btnReplaceAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnReplaceAll.Location = New System.Drawing.Point(171, 117)
            Me.btnReplaceAll.Name = "btnReplaceAll"
            Me.btnReplaceAll.Size = New System.Drawing.Size(75, 23)
            Me.btnReplaceAll.TabIndex = 21
            Me.btnReplaceAll.Text = "Replace &All"
            Me.btnReplaceAll.UseVisualStyleBackColor = True
            '
            'btnReplace
            '
            Me.btnReplace.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnReplace.Location = New System.Drawing.Point(90, 117)
            Me.btnReplace.Name = "btnReplace"
            Me.btnReplace.Size = New System.Drawing.Size(75, 23)
            Me.btnReplace.TabIndex = 19
            Me.btnReplace.Text = "&Replace"
            Me.btnReplace.UseVisualStyleBackColor = True
            '
            'btnHighlightAll
            '
            Me.btnHighlightAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnHighlightAll.Location = New System.Drawing.Point(110, 117)
            Me.btnHighlightAll.Name = "btnHighlightAll"
            Me.btnHighlightAll.Size = New System.Drawing.Size(136, 23)
            Me.btnHighlightAll.TabIndex = 20
            Me.btnHighlightAll.Text = "Find && highlight &all"
            Me.btnHighlightAll.UseVisualStyleBackColor = True
            Me.btnHighlightAll.Visible = False
            '
            'btnCancel
            '
            Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(252, 117)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 16
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'btnFindPrevious
            '
            Me.btnFindPrevious.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnFindPrevious.Location = New System.Drawing.Point(162, 88)
            Me.btnFindPrevious.Name = "btnFindPrevious"
            Me.btnFindPrevious.Size = New System.Drawing.Size(84, 23)
            Me.btnFindPrevious.TabIndex = 18
            Me.btnFindPrevious.Text = "Find pre&vious"
            Me.btnFindPrevious.UseVisualStyleBackColor = True
            '
            'btnFindNext
            '
            Me.btnFindNext.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnFindNext.Location = New System.Drawing.Point(252, 88)
            Me.btnFindNext.Name = "btnFindNext"
            Me.btnFindNext.Size = New System.Drawing.Size(75, 23)
            Me.btnFindNext.TabIndex = 17
            Me.btnFindNext.Text = "&Find next"
            Me.btnFindNext.UseVisualStyleBackColor = True
            '
            'txtReplaceWith
            '
            Me.txtReplaceWith.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtReplaceWith.Location = New System.Drawing.Point(84, 31)
            Me.txtReplaceWith.Name = "txtReplaceWith"
            Me.txtReplaceWith.Size = New System.Drawing.Size(243, 20)
            Me.txtReplaceWith.TabIndex = 13
            '
            'txtLookFor
            '
            Me.txtLookFor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtLookFor.Location = New System.Drawing.Point(84, 5)
            Me.txtLookFor.Name = "txtLookFor"
            Me.txtLookFor.Size = New System.Drawing.Size(243, 20)
            Me.txtLookFor.TabIndex = 11
            '
            'lblReplaceWith
            '
            Me.lblReplaceWith.AutoSize = True
            Me.lblReplaceWith.Location = New System.Drawing.Point(6, 34)
            Me.lblReplaceWith.Name = "lblReplaceWith"
            Me.lblReplaceWith.Size = New System.Drawing.Size(72, 13)
            Me.lblReplaceWith.TabIndex = 12
            Me.lblReplaceWith.Text = "Re&place with:"
            '
            'label1
            '
            Me.label1.AutoSize = True
            Me.label1.Location = New System.Drawing.Point(6, 8)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(56, 13)
            Me.label1.TabIndex = 10
            Me.label1.Text = "Fi&nd what:"
            '
            'FindAndReplaceForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(334, 145)
            Me.Controls.Add(Me.chkMatchCase)
            Me.Controls.Add(Me.chkMatchWholeWord)
            Me.Controls.Add(Me.btnReplaceAll)
            Me.Controls.Add(Me.btnReplace)
            Me.Controls.Add(Me.btnHighlightAll)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnFindPrevious)
            Me.Controls.Add(Me.btnFindNext)
            Me.Controls.Add(Me.txtReplaceWith)
            Me.Controls.Add(Me.txtLookFor)
            Me.Controls.Add(Me.lblReplaceWith)
            Me.Controls.Add(Me.label1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "FindAndReplaceForm"
            Me.ShowIcon = False
            Me.Text = "Find and replace"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents chkMatchCase As System.Windows.Forms.CheckBox
        Private WithEvents chkMatchWholeWord As System.Windows.Forms.CheckBox
        Private WithEvents btnReplaceAll As System.Windows.Forms.Button
        Private WithEvents btnReplace As System.Windows.Forms.Button
        Private WithEvents btnHighlightAll As System.Windows.Forms.Button
        Private WithEvents btnCancel As System.Windows.Forms.Button
        Private WithEvents btnFindPrevious As System.Windows.Forms.Button
        Private WithEvents btnFindNext As System.Windows.Forms.Button
        Private WithEvents txtReplaceWith As System.Windows.Forms.TextBox
        Private WithEvents txtLookFor As System.Windows.Forms.TextBox
        Private WithEvents lblReplaceWith As System.Windows.Forms.Label
        Private WithEvents label1 As System.Windows.Forms.Label
    End Class
End Namespace