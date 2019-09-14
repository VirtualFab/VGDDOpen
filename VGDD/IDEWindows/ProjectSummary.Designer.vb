Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ProjectSummary
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProjectSummary))
            Me.FootPrint1 = New FootPrint()
            Me.SuspendLayout()
            '
            'FootPrint1
            '
            Me.FootPrint1.Caption = "Footprint in bytes"
            Me.FootPrint1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.FootPrint1.Location = New System.Drawing.Point(0, 0)
            Me.FootPrint1.Name = "FootPrint1"
            Me.FootPrint1.Size = New System.Drawing.Size(268, 157)
            Me.FootPrint1.TabIndex = 1
            '
            'ProjectSummary
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(268, 157)
            Me.CloseButtonVisible = False
            Me.Controls.Add(Me.FootPrint1)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "ProjectSummary"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            Me.TabText = "Summary"
            Me.Text = "Summary"
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents FootPrint1 As FootPrint

    End Class
End Namespace