<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPlayerOptions
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPlayerOptions))
        Me.btnOk = New System.Windows.Forms.Button()
        Me.grpPlayer = New System.Windows.Forms.GroupBox()
        Me.pnlBgColor = New System.Windows.Forms.Panel()
        Me.btnSelectBgColor = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.pbBgBitmap = New System.Windows.Forms.PictureBox()
        Me.btnSelectBgBitmap = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.grpPlayer.SuspendLayout()
        CType(Me.pbBgBitmap, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.BackColor = System.Drawing.Color.DarkGreen
        Me.btnOk.ForeColor = System.Drawing.Color.Yellow
        Me.btnOk.Location = New System.Drawing.Point(408, 108)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(95, 23)
        Me.btnOk.TabIndex = 21
        Me.btnOk.Text = "Ok"
        Me.btnOk.UseVisualStyleBackColor = False
        '
        'grpPlayer
        '
        Me.grpPlayer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpPlayer.Controls.Add(Me.pnlBgColor)
        Me.grpPlayer.Controls.Add(Me.btnSelectBgColor)
        Me.grpPlayer.Controls.Add(Me.Label6)
        Me.grpPlayer.Controls.Add(Me.pbBgBitmap)
        Me.grpPlayer.Controls.Add(Me.btnSelectBgBitmap)
        Me.grpPlayer.Controls.Add(Me.Label5)
        Me.grpPlayer.Location = New System.Drawing.Point(10, 12)
        Me.grpPlayer.Name = "grpPlayer"
        Me.grpPlayer.Size = New System.Drawing.Size(502, 77)
        Me.grpPlayer.TabIndex = 26
        Me.grpPlayer.TabStop = False
        Me.grpPlayer.Text = "Player Options"
        '
        'pnlBgColor
        '
        Me.pnlBgColor.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlBgColor.Location = New System.Drawing.Point(397, 17)
        Me.pnlBgColor.Name = "pnlBgColor"
        Me.pnlBgColor.Size = New System.Drawing.Size(96, 54)
        Me.pnlBgColor.TabIndex = 36
        '
        'btnSelectBgColor
        '
        Me.btnSelectBgColor.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectBgColor.Location = New System.Drawing.Point(313, 38)
        Me.btnSelectBgColor.Name = "btnSelectBgColor"
        Me.btnSelectBgColor.Size = New System.Drawing.Size(59, 23)
        Me.btnSelectBgColor.TabIndex = 35
        Me.btnSelectBgColor.Text = "Change"
        Me.btnSelectBgColor.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(295, 22)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(95, 13)
        Me.Label6.TabIndex = 34
        Me.Label6.Text = "Background Color:"
        '
        'pbBgBitmap
        '
        Me.pbBgBitmap.Location = New System.Drawing.Point(116, 17)
        Me.pbBgBitmap.Name = "pbBgBitmap"
        Me.pbBgBitmap.Size = New System.Drawing.Size(96, 54)
        Me.pbBgBitmap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbBgBitmap.TabIndex = 33
        Me.pbBgBitmap.TabStop = False
        '
        'btnSelectBgBitmap
        '
        Me.btnSelectBgBitmap.Location = New System.Drawing.Point(25, 38)
        Me.btnSelectBgBitmap.Name = "btnSelectBgBitmap"
        Me.btnSelectBgBitmap.Size = New System.Drawing.Size(59, 23)
        Me.btnSelectBgBitmap.TabIndex = 32
        Me.btnSelectBgBitmap.Text = "Change"
        Me.btnSelectBgBitmap.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 22)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(103, 13)
        Me.Label5.TabIndex = 30
        Me.Label5.Text = "Background Bitmap:"
        '
        'frmPlayerOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(524, 145)
        Me.ControlBox = False
        Me.Controls.Add(Me.grpPlayer)
        Me.Controls.Add(Me.btnOk)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPlayerOptions"
        Me.Text = "Player Options"
        Me.grpPlayer.ResumeLayout(False)
        Me.grpPlayer.PerformLayout()
        CType(Me.pbBgBitmap, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents grpPlayer As System.Windows.Forms.GroupBox
    Friend WithEvents pnlBgColor As System.Windows.Forms.Panel
    Friend WithEvents btnSelectBgColor As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents pbBgBitmap As System.Windows.Forms.PictureBox
    Friend WithEvents btnSelectBgBitmap As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
End Class
