<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmChooseDemo
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
        Me.cmbDemos = New System.Windows.Forms.ComboBox()
        Me.btnExtract = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmbDemos
        '
        Me.cmbDemos.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbDemos.FormattingEnabled = True
        Me.cmbDemos.Items.AddRange(New Object() {"Demo1-MEB", "Demo2-Web-EthernetStarterKit"})
        Me.cmbDemos.Location = New System.Drawing.Point(3, 12)
        Me.cmbDemos.Name = "cmbDemos"
        Me.cmbDemos.Size = New System.Drawing.Size(230, 21)
        Me.cmbDemos.TabIndex = 0
        '
        'btnExtract
        '
        Me.btnExtract.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExtract.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnExtract.Enabled = False
        Me.btnExtract.Location = New System.Drawing.Point(239, 12)
        Me.btnExtract.Name = "btnExtract"
        Me.btnExtract.Size = New System.Drawing.Size(75, 23)
        Me.btnExtract.TabIndex = 1
        Me.btnExtract.Text = "Extract"
        Me.btnExtract.UseVisualStyleBackColor = True
        '
        'frmChooseDemo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(323, 46)
        Me.Controls.Add(Me.btnExtract)
        Me.Controls.Add(Me.cmbDemos)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmChooseDemo"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Choose Demo to extract"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cmbDemos As System.Windows.Forms.ComboBox
    Friend WithEvents btnExtract As System.Windows.Forms.Button
End Class
