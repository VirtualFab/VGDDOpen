Namespace VGDDCommon
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmMagnify
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMagnify))
            Me.TrackBar1 = New System.Windows.Forms.TrackBar()
            Me.lblZoom = New System.Windows.Forms.Label()
            Me.btnLock = New System.Windows.Forms.Button()
            CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'TrackBar1
            '
            Me.TrackBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TrackBar1.Location = New System.Drawing.Point(56, -5)
            Me.TrackBar1.Minimum = 1
            Me.TrackBar1.Name = "TrackBar1"
            Me.TrackBar1.Size = New System.Drawing.Size(204, 42)
            Me.TrackBar1.TabIndex = 0
            Me.TrackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft
            Me.TrackBar1.Value = 1
            '
            'lblZoom
            '
            Me.lblZoom.AutoSize = True
            Me.lblZoom.Location = New System.Drawing.Point(-3, 9)
            Me.lblZoom.Name = "lblZoom"
            Me.lblZoom.Size = New System.Drawing.Size(53, 13)
            Me.lblZoom.TabIndex = 2
            Me.lblZoom.Text = "Zoom: 1X"
            '
            'btnLock
            '
            Me.btnLock.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnLock.Image = Global.My.Resources.Resources.lock_open
            Me.btnLock.Location = New System.Drawing.Point(266, 0)
            Me.btnLock.Name = "btnLock"
            Me.btnLock.Size = New System.Drawing.Size(32, 37)
            Me.btnLock.TabIndex = 3
            Me.btnLock.UseVisualStyleBackColor = True
            '
            'frmMagnify
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(298, 268)
            Me.ControlBox = False
            Me.Controls.Add(Me.btnLock)
            Me.Controls.Add(Me.lblZoom)
            Me.Controls.Add(Me.TrackBar1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmMagnify"
            Me.Text = "Magnify Window"
            CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
        Friend WithEvents lblZoom As System.Windows.Forms.Label
        Friend WithEvents btnLock As System.Windows.Forms.Button
    End Class
End Namespace
