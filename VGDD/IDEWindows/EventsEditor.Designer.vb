Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Public Class EventsEditor
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EventsEditor))
            Me.TextEditorControl1 = New SourceEditor.Editor()
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
            Me.pnlAction = New System.Windows.Forms.Panel()
            Me.txtActionUserCode = New System.Windows.Forms.TextBox()
            Me.lblActionText = New System.Windows.Forms.Label()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.lblActionHelp = New System.Windows.Forms.Label()
            Me.pnlBitmaps = New System.Windows.Forms.Panel()
            Me.btnNewBitmap = New System.Windows.Forms.Button()
            Me.cmbBitmap = New System.Windows.Forms.ComboBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.cmbEvents = New System.Windows.Forms.ComboBox()
            Me.cmbObjects = New System.Windows.Forms.ComboBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.btnActionInsertCode = New System.Windows.Forms.Button()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.cmbActions = New System.Windows.Forms.ComboBox()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.pnlAction.SuspendLayout()
            Me.pnlBitmaps.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'TextEditorControl1
            '
            Me.TextEditorControl1.AllowDrop = True
            Me.TextEditorControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextEditorControl1.Enabled = False
            Me.TextEditorControl1.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TextEditorControl1.Location = New System.Drawing.Point(3, 16)
            Me.TextEditorControl1.Margin = New System.Windows.Forms.Padding(4)
            Me.TextEditorControl1.Name = "TextEditorControl1"
            Me.TextEditorControl1.Size = New System.Drawing.Size(632, 166)
            Me.TextEditorControl1.TabIndex = 1
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.pnlAction)
            Me.SplitContainer1.Panel1.Controls.Add(Me.btnCancel)
            Me.SplitContainer1.Panel1.Controls.Add(Me.lblActionHelp)
            Me.SplitContainer1.Panel1.Controls.Add(Me.pnlBitmaps)
            Me.SplitContainer1.Panel1.Controls.Add(Me.Label4)
            Me.SplitContainer1.Panel1.Controls.Add(Me.cmbEvents)
            Me.SplitContainer1.Panel1.Controls.Add(Me.cmbObjects)
            Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
            Me.SplitContainer1.Panel1.Controls.Add(Me.btnActionInsertCode)
            Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
            Me.SplitContainer1.Panel1.Controls.Add(Me.cmbActions)
            Me.SplitContainer1.Panel1MinSize = 200
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.GroupBox1)
            Me.SplitContainer1.Panel2MinSize = 100
            Me.SplitContainer1.Size = New System.Drawing.Size(853, 182)
            Me.SplitContainer1.SplitterDistance = 211
            Me.SplitContainer1.TabIndex = 2
            '
            'pnlAction
            '
            Me.pnlAction.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlAction.Controls.Add(Me.txtActionUserCode)
            Me.pnlAction.Controls.Add(Me.lblActionText)
            Me.pnlAction.Location = New System.Drawing.Point(6, 127)
            Me.pnlAction.Name = "pnlAction"
            Me.pnlAction.Size = New System.Drawing.Size(202, 48)
            Me.pnlAction.TabIndex = 26
            Me.pnlAction.Visible = False
            '
            'txtActionUserCode
            '
            Me.txtActionUserCode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtActionUserCode.Location = New System.Drawing.Point(6, 22)
            Me.txtActionUserCode.Name = "txtActionUserCode"
            Me.txtActionUserCode.Size = New System.Drawing.Size(194, 20)
            Me.txtActionUserCode.TabIndex = 26
            '
            'lblActionText
            '
            Me.lblActionText.AutoSize = True
            Me.lblActionText.Location = New System.Drawing.Point(3, 6)
            Me.lblActionText.Name = "lblActionText"
            Me.lblActionText.Size = New System.Drawing.Size(58, 13)
            Me.lblActionText.TabIndex = 24
            Me.lblActionText.Text = "ActionText"
            '
            'btnCancel
            '
            Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCancel.BackColor = System.Drawing.Color.OrangeRed
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(154, 6)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(53, 23)
            Me.btnCancel.TabIndex = 21
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = False
            Me.btnCancel.Visible = False
            '
            'lblActionHelp
            '
            Me.lblActionHelp.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblActionHelp.ForeColor = System.Drawing.Color.OliveDrab
            Me.lblActionHelp.Location = New System.Drawing.Point(8, 110)
            Me.lblActionHelp.Name = "lblActionHelp"
            Me.lblActionHelp.Size = New System.Drawing.Size(199, 42)
            Me.lblActionHelp.TabIndex = 22
            Me.lblActionHelp.Text = "lblActionHelp"
            '
            'pnlBitmaps
            '
            Me.pnlBitmaps.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlBitmaps.Controls.Add(Me.btnNewBitmap)
            Me.pnlBitmaps.Controls.Add(Me.cmbBitmap)
            Me.pnlBitmaps.Controls.Add(Me.Label3)
            Me.pnlBitmaps.Location = New System.Drawing.Point(7, 146)
            Me.pnlBitmaps.Name = "pnlBitmaps"
            Me.pnlBitmaps.Size = New System.Drawing.Size(202, 27)
            Me.pnlBitmaps.TabIndex = 25
            Me.pnlBitmaps.Visible = False
            '
            'btnNewBitmap
            '
            Me.btnNewBitmap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnNewBitmap.Location = New System.Drawing.Point(158, 1)
            Me.btnNewBitmap.Name = "btnNewBitmap"
            Me.btnNewBitmap.Size = New System.Drawing.Size(41, 23)
            Me.btnNewBitmap.TabIndex = 25
            Me.btnNewBitmap.Text = "New"
            Me.btnNewBitmap.UseVisualStyleBackColor = True
            '
            'cmbBitmap
            '
            Me.cmbBitmap.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbBitmap.FormattingEnabled = True
            Me.cmbBitmap.Location = New System.Drawing.Point(51, 3)
            Me.cmbBitmap.Name = "cmbBitmap"
            Me.cmbBitmap.Size = New System.Drawing.Size(93, 21)
            Me.cmbBitmap.TabIndex = 23
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(3, 6)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(42, 13)
            Me.Label3.TabIndex = 24
            Me.Label3.Text = "Bitmap:"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(14, 37)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(38, 13)
            Me.Label4.TabIndex = 4
            Me.Label4.Text = "Event:"
            '
            'cmbEvents
            '
            Me.cmbEvents.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbEvents.FormattingEnabled = True
            Me.cmbEvents.Location = New System.Drawing.Point(58, 34)
            Me.cmbEvents.Name = "cmbEvents"
            Me.cmbEvents.Size = New System.Drawing.Size(143, 21)
            Me.cmbEvents.TabIndex = 1
            '
            'cmbObjects
            '
            Me.cmbObjects.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbObjects.FormattingEnabled = True
            Me.cmbObjects.Location = New System.Drawing.Point(58, 60)
            Me.cmbObjects.Name = "cmbObjects"
            Me.cmbObjects.Size = New System.Drawing.Size(143, 21)
            Me.cmbObjects.TabIndex = 2
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(6, 63)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(46, 13)
            Me.Label1.TabIndex = 3
            Me.Label1.Text = "Objects:"
            '
            'btnActionInsertCode
            '
            Me.btnActionInsertCode.BackColor = System.Drawing.Color.LemonChiffon
            Me.btnActionInsertCode.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.btnActionInsertCode.ForeColor = System.Drawing.Color.Chocolate
            Me.btnActionInsertCode.Location = New System.Drawing.Point(12, 6)
            Me.btnActionInsertCode.Name = "btnActionInsertCode"
            Me.btnActionInsertCode.Size = New System.Drawing.Size(88, 23)
            Me.btnActionInsertCode.TabIndex = 1
            Me.btnActionInsertCode.Text = "Insert Code"
            Me.btnActionInsertCode.UseVisualStyleBackColor = False
            Me.btnActionInsertCode.Visible = False
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(7, 89)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(45, 13)
            Me.Label2.TabIndex = 5
            Me.Label2.Text = "Actions:"
            '
            'cmbActions
            '
            Me.cmbActions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbActions.FormattingEnabled = True
            Me.cmbActions.Location = New System.Drawing.Point(58, 86)
            Me.cmbActions.Name = "cmbActions"
            Me.cmbActions.Size = New System.Drawing.Size(143, 21)
            Me.cmbActions.TabIndex = 4
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.TextEditorControl1)
            Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(638, 182)
            Me.GroupBox1.TabIndex = 2
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Event Code"
            '
            'EventsEditor
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(853, 182)
            Me.Controls.Add(Me.SplitContainer1)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MinimumSize = New System.Drawing.Size(430, 0)
            Me.Name = "EventsEditor"
            Me.Text = "Events Editor"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel1.PerformLayout()
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.pnlAction.ResumeLayout(False)
            Me.pnlAction.PerformLayout()
            Me.pnlBitmaps.ResumeLayout(False)
            Me.pnlBitmaps.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TextEditorControl1 As SourceEditor.Editor 'System.Windows.Forms.RichTextBox
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents cmbActions As System.Windows.Forms.ComboBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents cmbObjects As System.Windows.Forms.ComboBox
        Friend WithEvents btnActionInsertCode As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents lblActionHelp As System.Windows.Forms.Label
        Friend WithEvents btnNewBitmap As System.Windows.Forms.Button
        Friend WithEvents pnlBitmaps As System.Windows.Forms.Panel
        Friend WithEvents cmbBitmap As System.Windows.Forms.ComboBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents cmbEvents As System.Windows.Forms.ComboBox
        Friend WithEvents pnlAction As System.Windows.Forms.Panel
        Friend WithEvents txtActionUserCode As System.Windows.Forms.TextBox
        Friend WithEvents lblActionText As System.Windows.Forms.Label
    End Class
End Namespace
