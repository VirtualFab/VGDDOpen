'Namespace VGDDCommon
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMplabXTemplatesEditor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMplabXTemplatesEditor))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.btnReload = New System.Windows.Forms.Button()
        Me.btnSaveTemplate = New System.Windows.Forms.Button()
        Me.tvTemplates = New System.Windows.Forms.TreeView()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.rtEdit = New System.Windows.Forms.RichTextBox()
        Me.picBitmap = New System.Windows.Forms.PictureBox()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.picBitmap, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnReload)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnSaveTemplate)
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvTemplates)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer1.Size = New System.Drawing.Size(866, 410)
        Me.SplitContainer1.SplitterDistance = 288
        Me.SplitContainer1.TabIndex = 0
        '
        'btnReload
        '
        Me.btnReload.Image = Global.My.Resources.Resources.reload
        Me.btnReload.Location = New System.Drawing.Point(203, 24)
        Me.btnReload.Name = "btnReload"
        Me.btnReload.Size = New System.Drawing.Size(26, 23)
        Me.btnReload.TabIndex = 9
        Me.btnReload.UseVisualStyleBackColor = True
        '
        'btnSaveTemplate
        '
        Me.btnSaveTemplate.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnSaveTemplate.Location = New System.Drawing.Point(235, 25)
        Me.btnSaveTemplate.Name = "btnSaveTemplate"
        Me.btnSaveTemplate.Size = New System.Drawing.Size(47, 21)
        Me.btnSaveTemplate.TabIndex = 8
        Me.btnSaveTemplate.Text = "Save"
        Me.btnSaveTemplate.UseVisualStyleBackColor = False
        Me.btnSaveTemplate.Visible = False
        '
        'tvTemplates
        '
        Me.tvTemplates.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvTemplates.Location = New System.Drawing.Point(0, 52)
        Me.tvTemplates.Name = "tvTemplates"
        Me.tvTemplates.Size = New System.Drawing.Size(282, 344)
        Me.tvTemplates.TabIndex = 4
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.rtEdit)
        Me.SplitContainer3.Panel2.Controls.Add(Me.picBitmap)
        Me.SplitContainer3.Size = New System.Drawing.Size(574, 410)
        Me.SplitContainer3.SplitterDistance = 92
        Me.SplitContainer3.TabIndex = 0
        '
        'rtEdit
        '
        Me.rtEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtEdit.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtEdit.Location = New System.Drawing.Point(0, 0)
        Me.rtEdit.Name = "rtEdit"
        Me.rtEdit.Size = New System.Drawing.Size(574, 314)
        Me.rtEdit.TabIndex = 0
        Me.rtEdit.Text = ""
        '
        'picBitmap
        '
        Me.picBitmap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picBitmap.Location = New System.Drawing.Point(0, 0)
        Me.picBitmap.Name = "picBitmap"
        Me.picBitmap.Size = New System.Drawing.Size(574, 314)
        Me.picBitmap.TabIndex = 2
        Me.picBitmap.TabStop = False
        '
        'frmMplabXTemplatesEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(866, 410)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMplabXTemplatesEditor"
        Me.Text = "MPLAB X Wizard Templates Editor"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.picBitmap, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents tvTemplates As System.Windows.Forms.TreeView
    Friend WithEvents rtEdit As System.Windows.Forms.RichTextBox
    Friend WithEvents picBitmap As System.Windows.Forms.PictureBox
    Friend WithEvents btnSaveTemplate As System.Windows.Forms.Button
    Friend WithEvents btnReload As System.Windows.Forms.Button
End Class
'End Namespace