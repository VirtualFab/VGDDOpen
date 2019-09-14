Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StringsPool
    Inherits Form

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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StringsPool))
        Me.dgv1 = New System.Windows.Forms.DataGridView()
        Me.StringID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Reference = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlEditString = New System.Windows.Forms.Panel()
        Me.TextEditorControl1 = New ICSharpCode.TextEditor.TextEditorControl()
        Me.lblEditStringNumber = New System.Windows.Forms.Label()
        Me.lblRefString = New System.Windows.Forms.Label()
        Me.btnEditOk = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.btnImport = New System.Windows.Forms.Button()
        Me.btnNewString = New System.Windows.Forms.Button()
        Me.pnlFunctions = New System.Windows.Forms.Panel()
        Me.btnRemoveString = New System.Windows.Forms.Button()
        Me.btnRenumber = New System.Windows.Forms.Button()
        CType(Me.dgv1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEditString.SuspendLayout()
        Me.pnlFunctions.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgv1
        '
        Me.dgv1.AllowUserToAddRows = False
        Me.dgv1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgv1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.StringID, Me.Reference})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgv1.DefaultCellStyle = DataGridViewCellStyle2
        Me.dgv1.Location = New System.Drawing.Point(0, 27)
        Me.dgv1.Name = "dgv1"
        Me.dgv1.Size = New System.Drawing.Size(900, 276)
        Me.dgv1.TabIndex = 0
        '
        'StringID
        '
        Me.StringID.Frozen = True
        Me.StringID.HeaderText = "String ID"
        Me.StringID.Name = "StringID"
        Me.StringID.ReadOnly = True
        Me.StringID.Width = 75
        '
        'Reference
        '
        Me.Reference.HeaderText = "Reference Text"
        Me.Reference.Name = "Reference"
        Me.Reference.ReadOnly = True
        Me.Reference.Width = 200
        '
        'pnlEditString
        '
        Me.pnlEditString.Controls.Add(Me.TextEditorControl1)
        Me.pnlEditString.Controls.Add(Me.lblEditStringNumber)
        Me.pnlEditString.Controls.Add(Me.lblRefString)
        Me.pnlEditString.Controls.Add(Me.btnEditOk)
        Me.pnlEditString.Location = New System.Drawing.Point(259, 90)
        Me.pnlEditString.Name = "pnlEditString"
        Me.pnlEditString.Size = New System.Drawing.Size(383, 122)
        Me.pnlEditString.TabIndex = 13
        Me.pnlEditString.Visible = False
        '
        'TextEditorControl1
        '
        Me.TextEditorControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextEditorControl1.IsReadOnly = False
        Me.TextEditorControl1.Location = New System.Drawing.Point(3, 28)
        Me.TextEditorControl1.Name = "TextEditorControl1"
        Me.TextEditorControl1.Size = New System.Drawing.Size(377, 66)
        Me.TextEditorControl1.TabIndex = 16
        Me.TextEditorControl1.Text = "TextEditorControl1"
        '
        'lblEditStringNumber
        '
        Me.lblEditStringNumber.AutoSize = True
        Me.lblEditStringNumber.Location = New System.Drawing.Point(3, 8)
        Me.lblEditStringNumber.Name = "lblEditStringNumber"
        Me.lblEditStringNumber.Size = New System.Drawing.Size(44, 13)
        Me.lblEditStringNumber.TabIndex = 14
        Me.lblEditStringNumber.Text = "String #"
        '
        'lblRefString
        '
        Me.lblRefString.AutoSize = True
        Me.lblRefString.Location = New System.Drawing.Point(99, 8)
        Me.lblRefString.Name = "lblRefString"
        Me.lblRefString.Size = New System.Drawing.Size(39, 13)
        Me.lblRefString.TabIndex = 13
        Me.lblRefString.Text = "Label1"
        '
        'btnEditOk
        '
        Me.btnEditOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnEditOk.Location = New System.Drawing.Point(316, 96)
        Me.btnEditOk.Name = "btnEditOk"
        Me.btnEditOk.Size = New System.Drawing.Size(64, 23)
        Me.btnEditOk.TabIndex = 12
        Me.btnEditOk.Text = "Ok"
        Me.btnEditOk.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.YellowGreen
        Me.btnSave.Location = New System.Drawing.Point(12, 1)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(99, 23)
        Me.btnSave.TabIndex = 14
        Me.btnSave.Text = "Save changes"
        Me.btnSave.UseVisualStyleBackColor = False
        Me.btnSave.Visible = False
        '
        'btnExport
        '
        Me.btnExport.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExport.Location = New System.Drawing.Point(353, 0)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(99, 23)
        Me.btnExport.TabIndex = 15
        Me.btnExport.Text = "Export as CSV"
        Me.btnExport.UseVisualStyleBackColor = True
        '
        'btnImport
        '
        Me.btnImport.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImport.Location = New System.Drawing.Point(458, 0)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(99, 23)
        Me.btnImport.TabIndex = 16
        Me.btnImport.Text = "Import from CSV"
        Me.btnImport.UseVisualStyleBackColor = True
        '
        'btnNewString
        '
        Me.btnNewString.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNewString.Location = New System.Drawing.Point(133, 0)
        Me.btnNewString.Name = "btnNewString"
        Me.btnNewString.Size = New System.Drawing.Size(99, 23)
        Me.btnNewString.TabIndex = 17
        Me.btnNewString.Text = "Add new string"
        Me.btnNewString.UseVisualStyleBackColor = True
        '
        'pnlFunctions
        '
        Me.pnlFunctions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlFunctions.Controls.Add(Me.btnRenumber)
        Me.pnlFunctions.Controls.Add(Me.btnRemoveString)
        Me.pnlFunctions.Controls.Add(Me.btnNewString)
        Me.pnlFunctions.Controls.Add(Me.btnExport)
        Me.pnlFunctions.Controls.Add(Me.btnImport)
        Me.pnlFunctions.Location = New System.Drawing.Point(337, 1)
        Me.pnlFunctions.Name = "pnlFunctions"
        Me.pnlFunctions.Size = New System.Drawing.Size(560, 24)
        Me.pnlFunctions.TabIndex = 18
        '
        'btnRemoveString
        '
        Me.btnRemoveString.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveString.Location = New System.Drawing.Point(238, 1)
        Me.btnRemoveString.Name = "btnRemoveString"
        Me.btnRemoveString.Size = New System.Drawing.Size(99, 23)
        Me.btnRemoveString.TabIndex = 18
        Me.btnRemoveString.Text = "Remove string"
        Me.btnRemoveString.UseVisualStyleBackColor = True
        Me.btnRemoveString.Visible = False
        '
        'btnRenumber
        '
        Me.btnRenumber.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRenumber.Location = New System.Drawing.Point(3, 1)
        Me.btnRenumber.Name = "btnRenumber"
        Me.btnRenumber.Size = New System.Drawing.Size(85, 23)
        Me.btnRenumber.TabIndex = 19
        Me.btnRenumber.Text = "Renumber IDs"
        Me.btnRenumber.UseVisualStyleBackColor = True
        '
        'StringsPool
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 303)
        Me.Controls.Add(Me.pnlFunctions)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.pnlEditString)
        Me.Controls.Add(Me.dgv1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "StringsPool"
        Me.Text = "Strings Pool"
        CType(Me.dgv1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEditString.ResumeLayout(False)
        Me.pnlEditString.PerformLayout()
        Me.pnlFunctions.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgv1 As System.Windows.Forms.DataGridView
    Friend WithEvents StringID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Reference As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents pnlEditString As System.Windows.Forms.Panel
    Friend WithEvents lblEditStringNumber As System.Windows.Forms.Label
    Friend WithEvents lblRefString As System.Windows.Forms.Label
    Friend WithEvents btnEditOk As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnExport As System.Windows.Forms.Button
    Friend WithEvents btnImport As System.Windows.Forms.Button
    Friend WithEvents btnNewString As System.Windows.Forms.Button
    Friend WithEvents pnlFunctions As System.Windows.Forms.Panel
    Friend WithEvents btnRemoveString As System.Windows.Forms.Button
    Friend WithEvents TextEditorControl1 As ICSharpCode.TextEditor.TextEditorControl
    Friend WithEvents btnRenumber As System.Windows.Forms.Button
End Class
