<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPlayer
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
    '<System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPlayer))
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBack = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripForward = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripdrpScreens = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripHelp = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripAnimation = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSnapshot = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripdrpLanguage = New System.Windows.Forms.ToolStripDropDownButton()
        Me.Panel1 = New PlayerPanel()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.BackColor = System.Drawing.Color.White
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBack, Me.ToolStripForward, Me.ToolStripSeparator3, Me.ToolStripdrpScreens, Me.ToolStripSeparator2, Me.ToolStripHelp, Me.ToolStripAnimation, Me.ToolStripSnapshot, Me.ToolStripdrpLanguage})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 286)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(477, 39)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBack
        '
        Me.ToolStripBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBack.Image = CType(resources.GetObject("ToolStripBack.Image"), System.Drawing.Image)
        Me.ToolStripBack.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBack.Name = "ToolStripBack"
        Me.ToolStripBack.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripBack.Text = "History Back"
        '
        'ToolStripForward
        '
        Me.ToolStripForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripForward.Image = CType(resources.GetObject("ToolStripForward.Image"), System.Drawing.Image)
        Me.ToolStripForward.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripForward.Name = "ToolStripForward"
        Me.ToolStripForward.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripForward.Text = "History Forward"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripdrpScreens
        '
        Me.ToolStripdrpScreens.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripdrpScreens.Image = CType(resources.GetObject("ToolStripdrpScreens.Image"), System.Drawing.Image)
        Me.ToolStripdrpScreens.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripdrpScreens.Name = "ToolStripdrpScreens"
        Me.ToolStripdrpScreens.Size = New System.Drawing.Size(45, 36)
        Me.ToolStripdrpScreens.Text = "Select Screen"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 39)
        '
        'ToolStripHelp
        '
        Me.ToolStripHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripHelp.Image = CType(resources.GetObject("ToolStripHelp.Image"), System.Drawing.Image)
        Me.ToolStripHelp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripHelp.Name = "ToolStripHelp"
        Me.ToolStripHelp.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripHelp.Text = "ToolStripButton1"
        '
        'ToolStripAnimation
        '
        Me.ToolStripAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripAnimation.Enabled = False
        Me.ToolStripAnimation.Image = CType(resources.GetObject("ToolStripAnimation.Image"), System.Drawing.Image)
        Me.ToolStripAnimation.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripAnimation.Name = "ToolStripAnimation"
        Me.ToolStripAnimation.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripAnimation.Text = "Enable/Disable animation"
        '
        'ToolStripSnapshot
        '
        Me.ToolStripSnapshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSnapshot.Image = CType(resources.GetObject("ToolStripSnapshot.Image"), System.Drawing.Image)
        Me.ToolStripSnapshot.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSnapshot.Name = "ToolStripSnapshot"
        Me.ToolStripSnapshot.Size = New System.Drawing.Size(36, 36)
        Me.ToolStripSnapshot.Text = "Take a snapshot"
        '
        'ToolStripdrpLanguage
        '
        Me.ToolStripdrpLanguage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripdrpLanguage.Image = CType(resources.GetObject("ToolStripdrpLanguage.Image"), System.Drawing.Image)
        Me.ToolStripdrpLanguage.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripdrpLanguage.Name = "ToolStripdrpLanguage"
        Me.ToolStripdrpLanguage.Size = New System.Drawing.Size(45, 36)
        Me.ToolStripdrpLanguage.Text = "Choose Language"
        '
        'Panel1
        '
        Me.Panel1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Panel1.Location = New System.Drawing.Point(41, 21)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(389, 255)
        Me.Panel1.TabIndex = 1
        Me.Panel1.TransparentColour = System.Drawing.Color.Empty
        '
        'frmPlayer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(477, 325)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmPlayer"
        Me.Text = "VGDD Player 2.3"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripAnimation As System.Windows.Forms.ToolStripButton
    Friend WithEvents Panel1 As PlayerPanel
    Friend WithEvents ToolStripBack As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripForward As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripdrpScreens As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripHelp As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSnapshot As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripdrpLanguage As System.Windows.Forms.ToolStripDropDownButton
End Class
