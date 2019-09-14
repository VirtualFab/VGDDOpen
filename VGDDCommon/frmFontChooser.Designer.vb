Namespace VGDDCommon

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmFontChooser
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFontChooser))
            Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
            Me.ListView1 = New System.Windows.Forms.ListView()
            Me.PictureBox1 = New System.Windows.Forms.PictureBox()
            Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.pnlBinPath = New System.Windows.Forms.Panel()
            Me.btnChoosBinPath = New System.Windows.Forms.Button()
            Me.txtBinPath = New System.Windows.Forms.TextBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.btnConvert = New System.Windows.Forms.Button()
            Me.btnRemove = New System.Windows.Forms.Button()
            Me.btnNew = New System.Windows.Forms.Button()
            Me.Panel2 = New System.Windows.Forms.Panel()
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
            Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
            Me.pnlViewFonts = New System.Windows.Forms.Panel()
            Me.LabeledPanel1 = New LabeledPanel()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Panel1.SuspendLayout()
            Me.pnlBinPath.SuspendLayout()
            Me.Panel2.SuspendLayout()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.SplitContainer2.Panel1.SuspendLayout()
            Me.SplitContainer2.Panel2.SuspendLayout()
            Me.SplitContainer2.SuspendLayout()
            Me.pnlViewFonts.SuspendLayout()
            Me.LabeledPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'ImageList1
            '
            Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
            Me.ImageList1.ImageSize = New System.Drawing.Size(16, 16)
            Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
            '
            'ListView1
            '
            Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.ListView1.FullRowSelect = True
            Me.ListView1.GridLines = True
            Me.ListView1.HideSelection = False
            Me.ListView1.Location = New System.Drawing.Point(0, 0)
            Me.ListView1.Name = "ListView1"
            Me.ListView1.Size = New System.Drawing.Size(441, 367)
            Me.ListView1.TabIndex = 0
            Me.ListView1.UseCompatibleStateImageBehavior = False
            '
            'PictureBox1
            '
            Me.PictureBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PictureBox1.BackColor = System.Drawing.Color.White
            Me.PictureBox1.Location = New System.Drawing.Point(0, 3)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(724, 110)
            Me.PictureBox1.TabIndex = 0
            Me.PictureBox1.TabStop = False
            '
            'PropertyGrid1
            '
            Me.PropertyGrid1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PropertyGrid1.Location = New System.Drawing.Point(7, 20)
            Me.PropertyGrid1.Name = "PropertyGrid1"
            Me.PropertyGrid1.Size = New System.Drawing.Size(269, 251)
            Me.PropertyGrid1.TabIndex = 0
            '
            'Panel1
            '
            Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Panel1.Controls.Add(Me.pnlBinPath)
            Me.Panel1.Controls.Add(Me.btnRemove)
            Me.Panel1.Controls.Add(Me.btnNew)
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(724, 52)
            Me.Panel1.TabIndex = 1
            '
            'pnlBinPath
            '
            Me.pnlBinPath.Controls.Add(Me.btnChoosBinPath)
            Me.pnlBinPath.Controls.Add(Me.txtBinPath)
            Me.pnlBinPath.Controls.Add(Me.Label3)
            Me.pnlBinPath.Controls.Add(Me.btnConvert)
            Me.pnlBinPath.Location = New System.Drawing.Point(108, 0)
            Me.pnlBinPath.Name = "pnlBinPath"
            Me.pnlBinPath.Size = New System.Drawing.Size(449, 49)
            Me.pnlBinPath.TabIndex = 9
            '
            'btnChoosBinPath
            '
            Me.btnChoosBinPath.Location = New System.Drawing.Point(329, 14)
            Me.btnChoosBinPath.Name = "btnChoosBinPath"
            Me.btnChoosBinPath.Size = New System.Drawing.Size(24, 23)
            Me.btnChoosBinPath.TabIndex = 8
            Me.btnChoosBinPath.Text = "..."
            Me.btnChoosBinPath.UseVisualStyleBackColor = True
            '
            'txtBinPath
            '
            Me.txtBinPath.Location = New System.Drawing.Point(6, 16)
            Me.txtBinPath.Name = "txtBinPath"
            Me.txtBinPath.Size = New System.Drawing.Size(317, 20)
            Me.txtBinPath.TabIndex = 7
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(3, 0)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(139, 13)
            Me.Label3.TabIndex = 6
            Me.Label3.Text = "Save converted .bin files to:"
            '
            'btnConvert
            '
            Me.btnConvert.Location = New System.Drawing.Point(371, 14)
            Me.btnConvert.Name = "btnConvert"
            Me.btnConvert.Size = New System.Drawing.Size(75, 23)
            Me.btnConvert.TabIndex = 5
            Me.btnConvert.Text = "Convert"
            Me.btnConvert.UseVisualStyleBackColor = True
            Me.btnConvert.Visible = False
            '
            'btnRemove
            '
            Me.btnRemove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnRemove.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
            Me.btnRemove.Enabled = False
            Me.btnRemove.Location = New System.Drawing.Point(657, 12)
            Me.btnRemove.Name = "btnRemove"
            Me.btnRemove.Size = New System.Drawing.Size(55, 23)
            Me.btnRemove.TabIndex = 4
            Me.btnRemove.Text = "Remove"
            Me.btnRemove.UseVisualStyleBackColor = False
            '
            'btnNew
            '
            Me.btnNew.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
            Me.btnNew.Location = New System.Drawing.Point(12, 12)
            Me.btnNew.Name = "btnNew"
            Me.btnNew.Size = New System.Drawing.Size(55, 23)
            Me.btnNew.TabIndex = 3
            Me.btnNew.Text = "Add"
            Me.btnNew.UseVisualStyleBackColor = False
            '
            'Panel2
            '
            Me.Panel2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Panel2.Controls.Add(Me.SplitContainer1)
            Me.Panel2.Location = New System.Drawing.Point(0, 58)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(724, 484)
            Me.Panel2.TabIndex = 0
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer1.Name = "SplitContainer1"
            Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
            Me.SplitContainer1.Panel1MinSize = 280
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.PictureBox1)
            Me.SplitContainer1.Size = New System.Drawing.Size(724, 484)
            Me.SplitContainer1.SplitterDistance = 367
            Me.SplitContainer1.TabIndex = 1
            '
            'SplitContainer2
            '
            Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer2.Name = "SplitContainer2"
            '
            'SplitContainer2.Panel1
            '
            Me.SplitContainer2.Panel1.Controls.Add(Me.pnlViewFonts)
            '
            'SplitContainer2.Panel2
            '
            Me.SplitContainer2.Panel2.Controls.Add(Me.LabeledPanel1)
            Me.SplitContainer2.Size = New System.Drawing.Size(724, 367)
            Me.SplitContainer2.SplitterDistance = 441
            Me.SplitContainer2.TabIndex = 2
            '
            'pnlViewFonts
            '
            Me.pnlViewFonts.Controls.Add(Me.ListView1)
            Me.pnlViewFonts.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlViewFonts.Location = New System.Drawing.Point(0, 0)
            Me.pnlViewFonts.Name = "pnlViewFonts"
            Me.pnlViewFonts.Size = New System.Drawing.Size(441, 367)
            Me.pnlViewFonts.TabIndex = 1
            '
            'LabeledPanel1
            '
            Me.LabeledPanel1.Controls.Add(Me.PropertyGrid1)
            Me.LabeledPanel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.LabeledPanel1.Location = New System.Drawing.Point(0, 0)
            Me.LabeledPanel1.Name = "LabeledPanel1"
            Me.LabeledPanel1.Size = New System.Drawing.Size(279, 367)
            Me.LabeledPanel1.TabIndex = 1
            Me.LabeledPanel1.Title = "Font Properties"
            Me.LabeledPanel1.TitleAlignement = System.Drawing.ContentAlignment.TopLeft
            Me.LabeledPanel1.TitleBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
            Me.LabeledPanel1.TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LabeledPanel1.TitleForeColor = System.Drawing.SystemColors.ControlText
            '
            'frmFontChooser
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(724, 544)
            Me.Controls.Add(Me.Panel1)
            Me.Controls.Add(Me.Panel2)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "frmFontChooser"
            Me.Text = "Font Chooser"
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Panel1.ResumeLayout(False)
            Me.pnlBinPath.ResumeLayout(False)
            Me.pnlBinPath.PerformLayout()
            Me.Panel2.ResumeLayout(False)
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.SplitContainer2.Panel1.ResumeLayout(False)
            Me.SplitContainer2.Panel2.ResumeLayout(False)
            Me.SplitContainer2.ResumeLayout(False)
            Me.pnlViewFonts.ResumeLayout(False)
            Me.LabeledPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
        Friend WithEvents btnChoosBinPath As System.Windows.Forms.Button
        Friend WithEvents txtBinPath As System.Windows.Forms.TextBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents btnConvert As System.Windows.Forms.Button
        Friend WithEvents PropertyGrid1 As System.Windows.Forms.PropertyGrid
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents ListView1 As System.Windows.Forms.ListView
        Friend WithEvents btnRemove As System.Windows.Forms.Button
        Friend WithEvents btnNew As System.Windows.Forms.Button
        Friend WithEvents pnlBinPath As System.Windows.Forms.Panel
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents pnlViewFonts As System.Windows.Forms.Panel
        Friend WithEvents LabeledPanel1 As LabeledPanel
        Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer

    End Class

End Namespace