<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LicenseHandler
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LicenseHandler))
        Me.btnExtract = New System.Windows.Forms.Button()
        Me.pnlBuy = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnEnterLicense = New System.Windows.Forms.Button()
        Me.txtLicenseCode = New System.Windows.Forms.TextBox()
        Me.lblBuy = New System.Windows.Forms.Label()
        Me.pnlBuy.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnExtract
        '
        Me.btnExtract.Enabled = False
        Me.btnExtract.Location = New System.Drawing.Point(191, 70)
        Me.btnExtract.Name = "btnExtract"
        Me.btnExtract.Size = New System.Drawing.Size(150, 23)
        Me.btnExtract.TabIndex = 13
        Me.btnExtract.Text = "Extract Library to disk"
        Me.btnExtract.UseVisualStyleBackColor = True
        '
        'pnlBuy
        '
        Me.pnlBuy.Controls.Add(Me.Label3)
        Me.pnlBuy.Controls.Add(Me.btnEnterLicense)
        Me.pnlBuy.Controls.Add(Me.txtLicenseCode)
        Me.pnlBuy.Location = New System.Drawing.Point(0, 24)
        Me.pnlBuy.Name = "pnlBuy"
        Me.pnlBuy.Size = New System.Drawing.Size(492, 40)
        Me.pnlBuy.TabIndex = 14
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 14)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(103, 13)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Enter License Code:"
        '
        'btnEnterLicense
        '
        Me.btnEnterLicense.Location = New System.Drawing.Point(397, 9)
        Me.btnEnterLicense.Name = "btnEnterLicense"
        Me.btnEnterLicense.Size = New System.Drawing.Size(75, 23)
        Me.btnEnterLicense.TabIndex = 16
        Me.btnEnterLicense.Text = "OK"
        Me.btnEnterLicense.UseVisualStyleBackColor = True
        '
        'txtLicenseCode
        '
        Me.txtLicenseCode.Location = New System.Drawing.Point(119, 11)
        Me.txtLicenseCode.Name = "txtLicenseCode"
        Me.txtLicenseCode.Size = New System.Drawing.Size(272, 20)
        Me.txtLicenseCode.TabIndex = 15
        '
        'lblBuy
        '
        Me.lblBuy.AutoSize = True
        Me.lblBuy.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBuy.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblBuy.Location = New System.Drawing.Point(2, 5)
        Me.lblBuy.Name = "lblBuy"
        Me.lblBuy.Size = New System.Drawing.Size(111, 16)
        Me.lblBuy.TabIndex = 15
        Me.lblBuy.Text = "Buy License Now"
        '
        'LicenseHandler
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(495, 105)
        Me.Controls.Add(Me.lblBuy)
        Me.Controls.Add(Me.btnExtract)
        Me.Controls.Add(Me.pnlBuy)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "LicenseHandler"
        Me.Text = "frmLicense"
        Me.pnlBuy.ResumeLayout(False)
        Me.pnlBuy.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnExtract As System.Windows.Forms.Button
    Friend WithEvents pnlBuy As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnEnterLicense As System.Windows.Forms.Button
    Friend WithEvents txtLicenseCode As System.Windows.Forms.TextBox
    Friend WithEvents lblBuy As System.Windows.Forms.Label
End Class
