<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Eula
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Eula))
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.btnAgree = New System.Windows.Forms.Button()
        Me.btnDisagree = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WebBrowser1.Location = New System.Drawing.Point(1, 2)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Size = New System.Drawing.Size(992, 401)
        Me.WebBrowser1.TabIndex = 0
        '
        'btnAgree
        '
        Me.btnAgree.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnAgree.BackColor = System.Drawing.Color.YellowGreen
        Me.btnAgree.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnAgree.Location = New System.Drawing.Point(235, 415)
        Me.btnAgree.Name = "btnAgree"
        Me.btnAgree.Size = New System.Drawing.Size(134, 23)
        Me.btnAgree.TabIndex = 1
        Me.btnAgree.Text = "I agree"
        Me.btnAgree.UseVisualStyleBackColor = False
        '
        'btnDisagree
        '
        Me.btnDisagree.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDisagree.BackColor = System.Drawing.Color.Red
        Me.btnDisagree.DialogResult = System.Windows.Forms.DialogResult.No
        Me.btnDisagree.Location = New System.Drawing.Point(583, 415)
        Me.btnDisagree.Name = "btnDisagree"
        Me.btnDisagree.Size = New System.Drawing.Size(130, 23)
        Me.btnDisagree.TabIndex = 2
        Me.btnDisagree.Text = "I don't agree"
        Me.btnDisagree.UseVisualStyleBackColor = False
        '
        'Eula
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(994, 449)
        Me.Controls.Add(Me.btnDisagree)
        Me.Controls.Add(Me.btnAgree)
        Me.Controls.Add(Me.WebBrowser1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Eula"
        Me.Text = "End User's License Agreement"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents btnAgree As System.Windows.Forms.Button
    Friend WithEvents btnDisagree As System.Windows.Forms.Button
End Class
