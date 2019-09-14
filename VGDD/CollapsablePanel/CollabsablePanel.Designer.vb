<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CollapsablePanel
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.titlePanel = New System.Windows.Forms.Panel()
        Me.lblPanelTitle = New System.Windows.Forms.Label()
        Me.togglingImage = New System.Windows.Forms.PictureBox()
        Me.titlePanel.SuspendLayout()
        CType(Me.togglingImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'titlePanel
        '
        Me.titlePanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.titlePanel.BackColor = System.Drawing.Color.Maroon
        Me.titlePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.titlePanel.Controls.Add(Me.lblPanelTitle)
        Me.titlePanel.Controls.Add(Me.togglingImage)
        Me.titlePanel.Location = New System.Drawing.Point(0, 0)
        Me.titlePanel.Name = "titlePanel"
        Me.titlePanel.Size = New System.Drawing.Size(167, 20)
        Me.titlePanel.TabIndex = 1
        '
        'lblPanelTitle
        '
        Me.lblPanelTitle.AutoSize = True
        Me.lblPanelTitle.BackColor = System.Drawing.Color.Transparent
        Me.lblPanelTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPanelTitle.Location = New System.Drawing.Point(22, 3)
        Me.lblPanelTitle.Name = "lblPanelTitle"
        Me.lblPanelTitle.Size = New System.Drawing.Size(64, 13)
        Me.lblPanelTitle.TabIndex = 1
        Me.lblPanelTitle.Text = "Panel title"
        '
        'togglingImage
        '
        Me.togglingImage.BackColor = System.Drawing.Color.Transparent
        Me.togglingImage.BackgroundImage = My.Resources.Resources.ComboBoxPlus
        Me.togglingImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.togglingImage.Location = New System.Drawing.Point(3, 3)
        Me.togglingImage.Name = "togglingImage"
        Me.togglingImage.Size = New System.Drawing.Size(14, 13)
        Me.togglingImage.TabIndex = 0
        Me.togglingImage.TabStop = False
        '
        'CollapsablePanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.titlePanel)
        Me.Name = "CollapsablePanel"
        Me.Size = New System.Drawing.Size(167, 150)
        Me.titlePanel.ResumeLayout(False)
        Me.titlePanel.PerformLayout()
        CType(Me.togglingImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents titlePanel As System.Windows.Forms.Panel
    Private WithEvents lblPanelTitle As System.Windows.Forms.Label
    Private WithEvents togglingImage As System.Windows.Forms.PictureBox

End Class
