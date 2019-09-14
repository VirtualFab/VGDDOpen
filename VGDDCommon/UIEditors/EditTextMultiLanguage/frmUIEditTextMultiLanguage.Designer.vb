<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUIEditTextMultiLanguage
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
        Me.components = New System.ComponentModel.Container()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.LangID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Translation = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnSave = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnEditOk = New System.Windows.Forms.Button()
        Me.lblStringNumber = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtRefVal = New System.Windows.Forms.TextBox()
        Me.pnlEditString = New System.Windows.Forms.Panel()
        Me.lblEditStringNumber = New System.Windows.Forms.Label()
        Me.lblRefString = New System.Windows.Forms.Label()
        Me.TextEditorControl1 = New ICSharpCode.TextEditor.TextEditorControl()
        Me.pnlEditString.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListView1
        '
        Me.ListView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.LangID, Me.Translation})
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.Location = New System.Drawing.Point(0, 41)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(466, 198)
        Me.ListView1.TabIndex = 0
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'LangID
        '
        Me.LangID.Text = "Language ID"
        Me.LangID.Width = 80
        '
        'Translation
        '
        Me.Translation.Text = "Translation"
        Me.Translation.Width = 400
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(173, 242)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(93, 23)
        Me.btnSave.TabIndex = 4
        Me.btnSave.Text = "Save"
        Me.ToolTip1.SetToolTip(Me.btnSave, "Save modifications. ESC to cancel")
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnEditOk
        '
        Me.btnEditOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnEditOk.Location = New System.Drawing.Point(316, 96)
        Me.btnEditOk.Name = "btnEditOk"
        Me.btnEditOk.Size = New System.Drawing.Size(64, 23)
        Me.btnEditOk.TabIndex = 12
        Me.btnEditOk.Text = "Ok"
        Me.ToolTip1.SetToolTip(Me.btnEditOk, "Save modifications. ESC to cancel")
        Me.btnEditOk.UseVisualStyleBackColor = True
        '
        'lblStringNumber
        '
        Me.lblStringNumber.AutoSize = True
        Me.lblStringNumber.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStringNumber.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblStringNumber.Location = New System.Drawing.Point(8, 13)
        Me.lblStringNumber.Name = "lblStringNumber"
        Me.lblStringNumber.Size = New System.Drawing.Size(44, 13)
        Me.lblStringNumber.TabIndex = 7
        Me.lblStringNumber.Text = "String #"
        Me.ToolTip1.SetToolTip(Me.lblStringNumber, "Click to select an existing string from the pool")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(107, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Reference text:"
        '
        'txtRefVal
        '
        Me.txtRefVal.Location = New System.Drawing.Point(202, 10)
        Me.txtRefVal.Name = "txtRefVal"
        Me.txtRefVal.Size = New System.Drawing.Size(252, 20)
        Me.txtRefVal.TabIndex = 10
        '
        'pnlEditString
        '
        Me.pnlEditString.Controls.Add(Me.TextEditorControl1)
        Me.pnlEditString.Controls.Add(Me.lblEditStringNumber)
        Me.pnlEditString.Controls.Add(Me.lblRefString)
        Me.pnlEditString.Controls.Add(Me.btnEditOk)
        Me.pnlEditString.Location = New System.Drawing.Point(71, 114)
        Me.pnlEditString.Name = "pnlEditString"
        Me.pnlEditString.Size = New System.Drawing.Size(383, 122)
        Me.pnlEditString.TabIndex = 12
        Me.pnlEditString.Visible = False
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
        'TextEditorControl1
        '
        Me.TextEditorControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextEditorControl1.IsReadOnly = False
        Me.TextEditorControl1.Location = New System.Drawing.Point(3, 24)
        Me.TextEditorControl1.Name = "TextEditorControl1"
        Me.TextEditorControl1.Size = New System.Drawing.Size(377, 66)
        Me.TextEditorControl1.TabIndex = 15
        Me.TextEditorControl1.Text = "TextEditorControl1"
        '
        'frmUIEditTextMultiLanguage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(466, 267)
        Me.ControlBox = False
        Me.Controls.Add(Me.pnlEditString)
        Me.Controls.Add(Me.txtRefVal)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblStringNumber)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.ListView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmUIEditTextMultiLanguage"
        Me.ShowIcon = False
        Me.Text = "MultiLanguage String Editor"
        Me.pnlEditString.ResumeLayout(False)
        Me.pnlEditString.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents Translation As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents LangID As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents lblStringNumber As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtRefVal As System.Windows.Forms.TextBox
    Friend WithEvents pnlEditString As System.Windows.Forms.Panel
    Friend WithEvents lblEditStringNumber As System.Windows.Forms.Label
    Friend WithEvents lblRefString As System.Windows.Forms.Label
    Friend WithEvents btnEditOk As System.Windows.Forms.Button
    Friend WithEvents TextEditorControl1 As ICSharpCode.TextEditor.TextEditorControl
End Class
