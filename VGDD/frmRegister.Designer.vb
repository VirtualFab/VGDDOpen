<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRegister
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRegister))
        Me.lblApplication = New System.Windows.Forms.Label()
        Me.lblActivated = New System.Windows.Forms.Label()
        Me.pnlPurchase = New System.Windows.Forms.Panel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblBuyMicrochip = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblBuyPayPal = New System.Windows.Forms.Label()
        Me.pnlActivate = New System.Windows.Forms.Panel()
        Me.lblActivation = New System.Windows.Forms.Label()
        Me.pnlActivateEnterKey = New System.Windows.Forms.Panel()
        Me.pnlActivateOnline = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtLicenseKey = New System.Windows.Forms.TextBox()
        Me.btnActivateOnline = New System.Windows.Forms.Button()
        Me.pnlActivateOffline = New System.Windows.Forms.Panel()
        Me.btnAuthCodeHelp = New System.Windows.Forms.Button()
        Me.btnAuthCodesFile = New System.Windows.Forms.Button()
        Me.btnActivateOffline = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtAuthCode = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtOfflineActKey = New System.Windows.Forms.TextBox()
        Me.rbActivateOffline = New System.Windows.Forms.RadioButton()
        Me.rbActivateOnline = New System.Windows.Forms.RadioButton()
        Me.pnlTransfer = New System.Windows.Forms.Panel()
        Me.btnTransfer = New System.Windows.Forms.Button()
        Me.lblTransfer = New System.Windows.Forms.Label()
        Me.pnlPurchase.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlActivate.SuspendLayout()
        Me.pnlActivateEnterKey.SuspendLayout()
        Me.pnlActivateOnline.SuspendLayout()
        Me.pnlActivateOffline.SuspendLayout()
        Me.pnlTransfer.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblApplication
        '
        Me.lblApplication.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblApplication.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblApplication.Location = New System.Drawing.Point(12, 6)
        Me.lblApplication.Name = "lblApplication"
        Me.lblApplication.Size = New System.Drawing.Size(829, 16)
        Me.lblApplication.TabIndex = 0
        Me.lblApplication.Text = "Application name"
        Me.lblApplication.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblActivated
        '
        Me.lblActivated.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblActivated.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblActivated.ForeColor = System.Drawing.Color.Green
        Me.lblActivated.Location = New System.Drawing.Point(14, 22)
        Me.lblActivated.Name = "lblActivated"
        Me.lblActivated.Size = New System.Drawing.Size(826, 64)
        Me.lblActivated.TabIndex = 1
        Me.lblActivated.Text = "This appliction is activated!"
        Me.lblActivated.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pnlPurchase
        '
        Me.pnlPurchase.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlPurchase.Controls.Add(Me.PictureBox2)
        Me.pnlPurchase.Controls.Add(Me.PictureBox1)
        Me.pnlPurchase.Controls.Add(Me.lblApplication)
        Me.pnlPurchase.Controls.Add(Me.lblBuyMicrochip)
        Me.pnlPurchase.Controls.Add(Me.Label1)
        Me.pnlPurchase.Controls.Add(Me.Label6)
        Me.pnlPurchase.Controls.Add(Me.lblBuyPayPal)
        Me.pnlPurchase.Location = New System.Drawing.Point(0, 0)
        Me.pnlPurchase.Name = "pnlPurchase"
        Me.pnlPurchase.Size = New System.Drawing.Size(851, 312)
        Me.pnlPurchase.TabIndex = 5
        '
        'PictureBox2
        '
        Me.PictureBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox2.Image = Global.My.Resources.Resources.MicrochipDirectLogo
        Me.PictureBox2.Location = New System.Drawing.Point(568, 48)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(186, 46)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 22
        Me.PictureBox2.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.My.Resources.Resources.paypal__secure
        Me.PictureBox1.Location = New System.Drawing.Point(117, 46)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(163, 95)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 21
        Me.PictureBox1.TabStop = False
        '
        'lblBuyMicrochip
        '
        Me.lblBuyMicrochip.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblBuyMicrochip.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblBuyMicrochip.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBuyMicrochip.ForeColor = System.Drawing.Color.Green
        Me.lblBuyMicrochip.Location = New System.Drawing.Point(464, 22)
        Me.lblBuyMicrochip.Name = "lblBuyMicrochip"
        Me.lblBuyMicrochip.Size = New System.Drawing.Size(376, 21)
        Me.lblBuyMicrochip.TabIndex = 20
        Me.lblBuyMicrochip.Text = "Buy VGDD Full Version on microchipDIRECT"
        Me.lblBuyMicrochip.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label1
        '
        Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(5, 277)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(836, 20)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Buying VGDD now grants all future releases of the software, which will be downloa" & _
            "dable free of charge."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(2, 161)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(828, 91)
        Me.Label6.TabIndex = 7
        Me.Label6.Text = resources.GetString("Label6.Text")
        '
        'lblBuyPayPal
        '
        Me.lblBuyPayPal.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblBuyPayPal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBuyPayPal.ForeColor = System.Drawing.Color.Green
        Me.lblBuyPayPal.Location = New System.Drawing.Point(12, 22)
        Me.lblBuyPayPal.Name = "lblBuyPayPal"
        Me.lblBuyPayPal.Size = New System.Drawing.Size(379, 21)
        Me.lblBuyPayPal.TabIndex = 5
        Me.lblBuyPayPal.Text = "Buy VGDD Full Version using PayPal"
        Me.lblBuyPayPal.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pnlActivate
        '
        Me.pnlActivate.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlActivate.Controls.Add(Me.lblActivation)
        Me.pnlActivate.Controls.Add(Me.pnlActivateEnterKey)
        Me.pnlActivate.Location = New System.Drawing.Point(5, 334)
        Me.pnlActivate.Name = "pnlActivate"
        Me.pnlActivate.Size = New System.Drawing.Size(846, 196)
        Me.pnlActivate.TabIndex = 6
        '
        'lblActivation
        '
        Me.lblActivation.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblActivation.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblActivation.ForeColor = System.Drawing.Color.Green
        Me.lblActivation.Location = New System.Drawing.Point(12, 6)
        Me.lblActivation.Name = "lblActivation"
        Me.lblActivation.Size = New System.Drawing.Size(823, 35)
        Me.lblActivation.TabIndex = 16
        Me.lblActivation.Text = "This appliction is activated!"
        '
        'pnlActivateEnterKey
        '
        Me.pnlActivateEnterKey.Controls.Add(Me.pnlActivateOnline)
        Me.pnlActivateEnterKey.Controls.Add(Me.pnlActivateOffline)
        Me.pnlActivateEnterKey.Controls.Add(Me.rbActivateOffline)
        Me.pnlActivateEnterKey.Controls.Add(Me.rbActivateOnline)
        Me.pnlActivateEnterKey.Location = New System.Drawing.Point(3, 47)
        Me.pnlActivateEnterKey.Name = "pnlActivateEnterKey"
        Me.pnlActivateEnterKey.Size = New System.Drawing.Size(755, 160)
        Me.pnlActivateEnterKey.TabIndex = 15
        Me.pnlActivateEnterKey.Visible = False
        '
        'pnlActivateOnline
        '
        Me.pnlActivateOnline.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlActivateOnline.Controls.Add(Me.Label3)
        Me.pnlActivateOnline.Controls.Add(Me.txtLicenseKey)
        Me.pnlActivateOnline.Controls.Add(Me.btnActivateOnline)
        Me.pnlActivateOnline.Location = New System.Drawing.Point(3, 22)
        Me.pnlActivateOnline.Name = "pnlActivateOnline"
        Me.pnlActivateOnline.Size = New System.Drawing.Size(684, 37)
        Me.pnlActivateOnline.TabIndex = 13
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 12)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(96, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Enter License Key:"
        '
        'txtLicenseKey
        '
        Me.txtLicenseKey.Location = New System.Drawing.Point(118, 6)
        Me.txtLicenseKey.Multiline = True
        Me.txtLicenseKey.Name = "txtLicenseKey"
        Me.txtLicenseKey.Size = New System.Drawing.Size(427, 22)
        Me.txtLicenseKey.TabIndex = 12
        '
        'btnActivateOnline
        '
        Me.btnActivateOnline.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnActivateOnline.Location = New System.Drawing.Point(560, 6)
        Me.btnActivateOnline.Name = "btnActivateOnline"
        Me.btnActivateOnline.Size = New System.Drawing.Size(111, 23)
        Me.btnActivateOnline.TabIndex = 10
        Me.btnActivateOnline.Text = "Activate"
        Me.btnActivateOnline.UseVisualStyleBackColor = True
        '
        'pnlActivateOffline
        '
        Me.pnlActivateOffline.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlActivateOffline.Controls.Add(Me.btnAuthCodeHelp)
        Me.pnlActivateOffline.Controls.Add(Me.btnAuthCodesFile)
        Me.pnlActivateOffline.Controls.Add(Me.btnActivateOffline)
        Me.pnlActivateOffline.Controls.Add(Me.Label4)
        Me.pnlActivateOffline.Controls.Add(Me.txtAuthCode)
        Me.pnlActivateOffline.Controls.Add(Me.Label2)
        Me.pnlActivateOffline.Controls.Add(Me.txtOfflineActKey)
        Me.pnlActivateOffline.Enabled = False
        Me.pnlActivateOffline.Location = New System.Drawing.Point(3, 66)
        Me.pnlActivateOffline.Name = "pnlActivateOffline"
        Me.pnlActivateOffline.Size = New System.Drawing.Size(684, 74)
        Me.pnlActivateOffline.TabIndex = 14
        '
        'btnAuthCodeHelp
        '
        Me.btnAuthCodeHelp.Image = Global.My.Resources.Resources.helpround
        Me.btnAuthCodeHelp.Location = New System.Drawing.Point(649, 8)
        Me.btnAuthCodeHelp.Name = "btnAuthCodeHelp"
        Me.btnAuthCodeHelp.Size = New System.Drawing.Size(22, 23)
        Me.btnAuthCodeHelp.TabIndex = 15
        Me.btnAuthCodeHelp.UseVisualStyleBackColor = True
        '
        'btnAuthCodesFile
        '
        Me.btnAuthCodesFile.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnAuthCodesFile.Location = New System.Drawing.Point(560, 8)
        Me.btnAuthCodesFile.Name = "btnAuthCodesFile"
        Me.btnAuthCodesFile.Size = New System.Drawing.Size(83, 23)
        Me.btnAuthCodesFile.TabIndex = 14
        Me.btnAuthCodesFile.Text = "File..."
        Me.btnAuthCodesFile.UseVisualStyleBackColor = True
        '
        'btnActivateOffline
        '
        Me.btnActivateOffline.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnActivateOffline.Location = New System.Drawing.Point(560, 36)
        Me.btnActivateOffline.Name = "btnActivateOffline"
        Me.btnActivateOffline.Size = New System.Drawing.Size(111, 23)
        Me.btnActivateOffline.TabIndex = 13
        Me.btnActivateOffline.Text = "Activate"
        Me.btnActivateOffline.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 12)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(63, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Auth. Code:"
        '
        'txtAuthCode
        '
        Me.txtAuthCode.Location = New System.Drawing.Point(118, 9)
        Me.txtAuthCode.Multiline = True
        Me.txtAuthCode.Name = "txtAuthCode"
        Me.txtAuthCode.ReadOnly = True
        Me.txtAuthCode.Size = New System.Drawing.Size(427, 22)
        Me.txtAuthCode.TabIndex = 11
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 40)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(106, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Enter Activation Key:"
        '
        'txtOfflineActKey
        '
        Me.txtOfflineActKey.Location = New System.Drawing.Point(118, 37)
        Me.txtOfflineActKey.Multiline = True
        Me.txtOfflineActKey.Name = "txtOfflineActKey"
        Me.txtOfflineActKey.Size = New System.Drawing.Size(427, 22)
        Me.txtOfflineActKey.TabIndex = 9
        '
        'rbActivateOffline
        '
        Me.rbActivateOffline.AutoSize = True
        Me.rbActivateOffline.Location = New System.Drawing.Point(319, 4)
        Me.rbActivateOffline.Name = "rbActivateOffline"
        Me.rbActivateOffline.Size = New System.Drawing.Size(193, 17)
        Me.rbActivateOffline.TabIndex = 16
        Me.rbActivateOffline.Text = "No Internet access: Activate Offline"
        Me.rbActivateOffline.UseVisualStyleBackColor = True
        '
        'rbActivateOnline
        '
        Me.rbActivateOnline.AutoSize = True
        Me.rbActivateOnline.Checked = True
        Me.rbActivateOnline.Location = New System.Drawing.Point(39, 4)
        Me.rbActivateOnline.Name = "rbActivateOnline"
        Me.rbActivateOnline.Size = New System.Drawing.Size(148, 17)
        Me.rbActivateOnline.TabIndex = 15
        Me.rbActivateOnline.TabStop = True
        Me.rbActivateOnline.Text = "Activate Online (preferred)"
        Me.rbActivateOnline.UseVisualStyleBackColor = True
        '
        'pnlTransfer
        '
        Me.pnlTransfer.Controls.Add(Me.btnTransfer)
        Me.pnlTransfer.Controls.Add(Me.lblTransfer)
        Me.pnlTransfer.Location = New System.Drawing.Point(5, 564)
        Me.pnlTransfer.Name = "pnlTransfer"
        Me.pnlTransfer.Size = New System.Drawing.Size(758, 73)
        Me.pnlTransfer.TabIndex = 7
        Me.pnlTransfer.Visible = False
        '
        'btnTransfer
        '
        Me.btnTransfer.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnTransfer.Location = New System.Drawing.Point(308, 38)
        Me.btnTransfer.Name = "btnTransfer"
        Me.btnTransfer.Size = New System.Drawing.Size(136, 23)
        Me.btnTransfer.TabIndex = 18
        Me.btnTransfer.Text = "Transfer License"
        Me.btnTransfer.UseVisualStyleBackColor = True
        '
        'lblTransfer
        '
        Me.lblTransfer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTransfer.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTransfer.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblTransfer.Location = New System.Drawing.Point(10, 10)
        Me.lblTransfer.Name = "lblTransfer"
        Me.lblTransfer.Size = New System.Drawing.Size(742, 25)
        Me.lblTransfer.TabIndex = 17
        Me.lblTransfer.Text = "You can transfer the License onto another PC (for at max. 3 times)"
        Me.lblTransfer.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'frmRegister
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(852, 749)
        Me.Controls.Add(Me.pnlActivate)
        Me.Controls.Add(Me.pnlTransfer)
        Me.Controls.Add(Me.pnlPurchase)
        Me.Controls.Add(Me.lblActivated)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmRegister"
        Me.Text = "License"
        Me.pnlPurchase.ResumeLayout(False)
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlActivate.ResumeLayout(False)
        Me.pnlActivateEnterKey.ResumeLayout(False)
        Me.pnlActivateEnterKey.PerformLayout()
        Me.pnlActivateOnline.ResumeLayout(False)
        Me.pnlActivateOnline.PerformLayout()
        Me.pnlActivateOffline.ResumeLayout(False)
        Me.pnlActivateOffline.PerformLayout()
        Me.pnlTransfer.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblApplication As System.Windows.Forms.Label
    Friend WithEvents lblActivated As System.Windows.Forms.Label
    Friend WithEvents pnlPurchase As System.Windows.Forms.Panel
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblBuyPayPal As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents pnlActivate As System.Windows.Forms.Panel
    Friend WithEvents pnlActivateEnterKey As System.Windows.Forms.Panel
    Friend WithEvents txtLicenseKey As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtOfflineActKey As System.Windows.Forms.TextBox
    Friend WithEvents btnActivateOnline As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblActivation As System.Windows.Forms.Label
    Friend WithEvents pnlActivateOnline As System.Windows.Forms.Panel
    Friend WithEvents pnlActivateOffline As System.Windows.Forms.Panel
    Friend WithEvents btnActivateOffline As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtAuthCode As System.Windows.Forms.TextBox
    Friend WithEvents rbActivateOffline As System.Windows.Forms.RadioButton
    Friend WithEvents rbActivateOnline As System.Windows.Forms.RadioButton
    Friend WithEvents pnlTransfer As System.Windows.Forms.Panel
    Friend WithEvents btnTransfer As System.Windows.Forms.Button
    Friend WithEvents lblTransfer As System.Windows.Forms.Label
    Friend WithEvents btnAuthCodesFile As System.Windows.Forms.Button
    Friend WithEvents btnAuthCodeHelp As System.Windows.Forms.Button
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblBuyMicrochip As System.Windows.Forms.Label
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
End Class
