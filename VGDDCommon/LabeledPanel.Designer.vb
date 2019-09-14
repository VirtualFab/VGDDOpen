<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LabeledPanel
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
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblHeader
        '
        Me.lblHeader.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblHeader.Location = New System.Drawing.Point(0, 0)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(211, 17)
        Me.lblHeader.TabIndex = 2
        Me.lblHeader.Text = "Output"
        '
        'LabeledPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblHeader)
        Me.Name = "LabeledPanel"
        Me.Size = New System.Drawing.Size(211, 224)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents lblHeader As System.Windows.Forms.Label

End Class
