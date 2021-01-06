Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MainSplash
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

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
            Me.pnlSplash = New System.Windows.Forms.Panel()
            Me.lnkSplashNewProject = New System.Windows.Forms.Label()
            Me.btnDemoHelp = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.lnkSplashOpenDemo = New System.Windows.Forms.Label()
            Me.lnkSplashOpenProject = New System.Windows.Forms.Label()
            Me.lstRecent = New System.Windows.Forms.ListBox()
            Me.AboutBox11 = New AboutBox1()
            Me.pnlSplash.SuspendLayout()
            Me.SuspendLayout()
            '
            'pnlSplash
            '
            Me.pnlSplash.BackColor = System.Drawing.SystemColors.ControlLightLight
            Me.pnlSplash.Controls.Add(Me.AboutBox11)
            Me.pnlSplash.Controls.Add(Me.lnkSplashNewProject)
            Me.pnlSplash.Controls.Add(Me.btnDemoHelp)
            Me.pnlSplash.Controls.Add(Me.Label1)
            Me.pnlSplash.Controls.Add(Me.lnkSplashOpenDemo)
            Me.pnlSplash.Controls.Add(Me.lnkSplashOpenProject)
            Me.pnlSplash.Controls.Add(Me.lstRecent)
            Me.pnlSplash.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlSplash.Location = New System.Drawing.Point(0, 0)
            Me.pnlSplash.Name = "pnlSplash"
            Me.pnlSplash.Size = New System.Drawing.Size(1041, 445)
            Me.pnlSplash.TabIndex = 2
            '
            'lnkSplashNewProject
            '
            Me.lnkSplashNewProject.AutoSize = True
            Me.lnkSplashNewProject.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lnkSplashNewProject.ForeColor = System.Drawing.SystemColors.HotTrack
            Me.lnkSplashNewProject.Location = New System.Drawing.Point(20, 16)
            Me.lnkSplashNewProject.Name = "lnkSplashNewProject"
            Me.lnkSplashNewProject.Size = New System.Drawing.Size(77, 16)
            Me.lnkSplashNewProject.TabIndex = 10
            Me.lnkSplashNewProject.Text = "New Project"
            '
            'btnDemoHelp
            '
            Me.btnDemoHelp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.btnDemoHelp.FlatAppearance.BorderSize = 0
            Me.btnDemoHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnDemoHelp.Image = Global.My.Resources.Resources.help
            Me.btnDemoHelp.Location = New System.Drawing.Point(145, 68)
            Me.btnDemoHelp.Name = "btnDemoHelp"
            Me.btnDemoHelp.Size = New System.Drawing.Size(35, 37)
            Me.btnDemoHelp.TabIndex = 16
            Me.btnDemoHelp.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label1.ForeColor = System.Drawing.SystemColors.HotTrack
            Me.Label1.Location = New System.Drawing.Point(20, 110)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(102, 16)
            Me.Label1.TabIndex = 15
            Me.Label1.Text = "Recent Projects:"
            '
            'lnkSplashOpenDemo
            '
            Me.lnkSplashOpenDemo.AutoSize = True
            Me.lnkSplashOpenDemo.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lnkSplashOpenDemo.ForeColor = System.Drawing.SystemColors.HotTrack
            Me.lnkSplashOpenDemo.Location = New System.Drawing.Point(20, 78)
            Me.lnkSplashOpenDemo.Name = "lnkSplashOpenDemo"
            Me.lnkSplashOpenDemo.Size = New System.Drawing.Size(119, 16)
            Me.lnkSplashOpenDemo.TabIndex = 12
            Me.lnkSplashOpenDemo.Text = "Open Demo Project"
            '
            'lnkSplashOpenProject
            '
            Me.lnkSplashOpenProject.AutoSize = True
            Me.lnkSplashOpenProject.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lnkSplashOpenProject.ForeColor = System.Drawing.SystemColors.HotTrack
            Me.lnkSplashOpenProject.Location = New System.Drawing.Point(20, 46)
            Me.lnkSplashOpenProject.Name = "lnkSplashOpenProject"
            Me.lnkSplashOpenProject.Size = New System.Drawing.Size(129, 16)
            Me.lnkSplashOpenProject.TabIndex = 11
            Me.lnkSplashOpenProject.Text = "Open existing Project"
            '
            'lstRecent
            '
            Me.lstRecent.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lstRecent.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.lstRecent.Enabled = True
            Me.lstRecent.FormattingEnabled = True
            Me.lstRecent.Location = New System.Drawing.Point(23, 129)
            Me.lstRecent.Name = "lstRecent"
            Me.lstRecent.Size = New System.Drawing.Size(1015, 156)
            Me.lstRecent.TabIndex = 14
            '
            'AboutBox11
            '
            Me.AboutBox11.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.AboutBox11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.AboutBox11.BackColor = System.Drawing.Color.White
            Me.AboutBox11.Location = New System.Drawing.Point(532, 0)
            Me.AboutBox11.MinimumSize = New System.Drawing.Size(506, 351)
            Me.AboutBox11.Name = "AboutBox11"
            Me.AboutBox11.Size = New System.Drawing.Size(506, 442)
            Me.AboutBox11.TabIndex = 17
            '
            'MainSplash
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(1041, 445)
            Me.Controls.Add(Me.pnlSplash)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "MainSplash"
            Me.TabText = "Start"
            Me.Text = "Start"
            Me.pnlSplash.ResumeLayout(False)
            Me.pnlSplash.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents pnlSplash As System.Windows.Forms.Panel
        Friend WithEvents lnkSplashNewProject As System.Windows.Forms.Label
        Friend WithEvents btnDemoHelp As System.Windows.Forms.Button
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents lstRecent As System.Windows.Forms.ListBox
        Friend WithEvents lnkSplashOpenDemo As System.Windows.Forms.Label
        Friend WithEvents lnkSplashOpenProject As System.Windows.Forms.Label
        Friend WithEvents AboutBox11 As AboutBox1

    End Class
End Namespace