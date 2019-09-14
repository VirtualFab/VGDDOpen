<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AboutBox1
    'Inherits System.Windows.Forms.Form
    Inherits System.Windows.Forms.UserControl

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AboutBox1))
        Me.lnkEmail = New System.Windows.Forms.LinkLabel()
        Me.LabelProductName = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.LabelVersion = New System.Windows.Forms.Label()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.LabelCopyright = New System.Windows.Forms.Label()
        Me.Logo = New System.Windows.Forms.PictureBox()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.lblForums = New System.Windows.Forms.LinkLabel()
        Me.lnkWebSite = New System.Windows.Forms.LinkLabel()
        Me.CopyInfoToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.Logo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lnkEmail
        '
        Me.lnkEmail.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lnkEmail.BackColor = System.Drawing.Color.Transparent
        Me.lnkEmail.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lnkEmail.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.lnkEmail.LinkColor = System.Drawing.SystemColors.HotTrack
        Me.lnkEmail.Location = New System.Drawing.Point(291, 392)
        Me.lnkEmail.Margin = New System.Windows.Forms.Padding(3)
        Me.lnkEmail.Name = "lnkEmail"
        Me.lnkEmail.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.lnkEmail.Size = New System.Drawing.Size(212, 16)
        Me.lnkEmail.TabIndex = 6
        Me.lnkEmail.TabStop = True
        Me.lnkEmail.Text = "email"
        Me.lnkEmail.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'LabelProductName
        '
        Me.LabelProductName.BackColor = System.Drawing.Color.Transparent
        Me.LabelProductName.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelProductName.Location = New System.Drawing.Point(0, 99)
        Me.LabelProductName.Name = "LabelProductName"
        Me.LabelProductName.Size = New System.Drawing.Size(506, 51)
        Me.LabelProductName.TabIndex = 7
        Me.LabelProductName.Text = "ProductName"
        Me.LabelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(0, 5)
        Me.PictureBox1.MinimumSize = New System.Drawing.Size(360, 66)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(506, 93)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 8
        Me.PictureBox1.TabStop = False
        '
        'LabelVersion
        '
        Me.LabelVersion.BackColor = System.Drawing.Color.Transparent
        Me.LabelVersion.ContextMenuStrip = Me.ContextMenuStrip1
        Me.LabelVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelVersion.Location = New System.Drawing.Point(0, 179)
        Me.LabelVersion.Name = "LabelVersion"
        Me.LabelVersion.Size = New System.Drawing.Size(506, 70)
        Me.LabelVersion.TabIndex = 9
        Me.LabelVersion.Text = "Version Line1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Version Line2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Version Line3" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Version Line4"
        Me.LabelVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.BackColor = System.Drawing.Color.Transparent
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyInfoToClipboardToolStripMenuItem, Me.ToolStripMenuItem1})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(227, 48)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.BackColor = System.Drawing.Color.Transparent
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(226, 22)
        Me.ToolStripMenuItem1.Text = "Copy AuthCode to clipboard"
        '
        'LabelCopyright
        '
        Me.LabelCopyright.BackColor = System.Drawing.Color.Transparent
        Me.LabelCopyright.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelCopyright.Location = New System.Drawing.Point(100, 381)
        Me.LabelCopyright.Name = "LabelCopyright"
        Me.LabelCopyright.Size = New System.Drawing.Size(306, 24)
        Me.LabelCopyright.TabIndex = 10
        Me.LabelCopyright.Text = "Copyright"
        Me.LabelCopyright.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Logo
        '
        Me.Logo.BackColor = System.Drawing.Color.Transparent
        Me.Logo.Image = Global.My.Resources.Resources.g12
        Me.Logo.Location = New System.Drawing.Point(196, 252)
        Me.Logo.Name = "Logo"
        Me.Logo.Size = New System.Drawing.Size(114, 126)
        Me.Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.Logo.TabIndex = 11
        Me.Logo.TabStop = False
        '
        'lblDescription
        '
        Me.lblDescription.BackColor = System.Drawing.Color.Transparent
        Me.lblDescription.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDescription.Location = New System.Drawing.Point(0, 155)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(506, 24)
        Me.lblDescription.TabIndex = 13
        Me.lblDescription.Text = "Description"
        Me.lblDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblForums
        '
        Me.lblForums.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblForums.BackColor = System.Drawing.Color.Transparent
        Me.lblForums.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblForums.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.lblForums.LinkColor = System.Drawing.SystemColors.HotTrack
        Me.lblForums.Location = New System.Drawing.Point(443, 360)
        Me.lblForums.Name = "lblForums"
        Me.lblForums.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.lblForums.Size = New System.Drawing.Size(60, 16)
        Me.lblForums.TabIndex = 14
        Me.lblForums.TabStop = True
        Me.lblForums.Text = "Forum"
        Me.lblForums.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lnkWebSite
        '
        Me.lnkWebSite.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lnkWebSite.BackColor = System.Drawing.Color.Transparent
        Me.lnkWebSite.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lnkWebSite.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.lnkWebSite.LinkColor = System.Drawing.SystemColors.HotTrack
        Me.lnkWebSite.Location = New System.Drawing.Point(436, 376)
        Me.lnkWebSite.Name = "lnkWebSite"
        Me.lnkWebSite.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.lnkWebSite.Size = New System.Drawing.Size(67, 16)
        Me.lnkWebSite.TabIndex = 15
        Me.lnkWebSite.TabStop = True
        Me.lnkWebSite.Text = "Web site"
        Me.lnkWebSite.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'CopyInfoToClipboardToolStripMenuItem
        '
        Me.CopyInfoToClipboardToolStripMenuItem.Name = "CopyInfoToClipboardToolStripMenuItem"
        Me.CopyInfoToClipboardToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.CopyInfoToClipboardToolStripMenuItem.Text = "Copy info to clipboard"
        '
        'AboutBox1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.LabelProductName)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lblForums)
        Me.Controls.Add(Me.lnkWebSite)
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.Logo)
        Me.Controls.Add(Me.LabelVersion)
        Me.Controls.Add(Me.lnkEmail)
        Me.Controls.Add(Me.LabelCopyright)
        Me.MinimumSize = New System.Drawing.Size(506, 351)
        Me.Name = "AboutBox1"
        Me.Size = New System.Drawing.Size(506, 408)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.Logo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lnkEmail As System.Windows.Forms.LinkLabel
    Friend WithEvents LabelProductName As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents LabelVersion As System.Windows.Forms.Label
    Friend WithEvents LabelCopyright As System.Windows.Forms.Label
    Friend WithEvents Logo As System.Windows.Forms.PictureBox
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents lblForums As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkWebSite As System.Windows.Forms.LinkLabel
    Friend WithEvents CopyInfoToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
