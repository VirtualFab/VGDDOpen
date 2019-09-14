Namespace VGDDCommon

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmColorEditor
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
            Me.HelpProvider1 = New System.Windows.Forms.HelpProvider()
            Me.ColorPicker1 = New VGDDCommon.ColorPicker()
            CType(Me.ColorPicker1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'ColorPicker1
            '
            Me.ColorPicker1.Location = New System.Drawing.Point(0, 0)
            Me.ColorPicker1.Name = "ColorPicker1"
            Me.ColorPicker1.Size = New System.Drawing.Size(406, 160)
            Me.ColorPicker1.TabIndex = 0
            Me.ColorPicker1.Value = System.Drawing.Color.Red
            '
            'frmColorEditor
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(403, 156)
            Me.Controls.Add(Me.ColorPicker1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.HelpButton = True
            Me.HelpProvider1.SetHelpNavigator(Me, System.Windows.Forms.HelpNavigator.Topic)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmColorEditor"
            Me.HelpProvider1.SetShowHelp(Me, True)
            Me.ShowIcon = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Choose Colour"
            CType(Me.ColorPicker1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents ColorPicker1 As ColorPicker
        Friend WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
    End Class
End Namespace