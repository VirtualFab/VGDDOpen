<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CodeGenLocation
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
        Me.grpMplabX = New System.Windows.Forms.GroupBox()
        Me.lblMplabXToolChain = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblMplabXConfig = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnChangeMplabXProject = New System.Windows.Forms.Button()
        Me.lblMplabXProject = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblCodeGenPath = New System.Windows.Forms.Label()
        Me.grpWhere = New System.Windows.Forms.GroupBox()
        Me.rbWhereMplabxConfigFolder = New System.Windows.Forms.RadioButton()
        Me.btnDestPathOther = New System.Windows.Forms.Button()
        Me.txtCodeGenPathOther = New System.Windows.Forms.TextBox()
        Me.rbWhereOtherFolder = New System.Windows.Forms.RadioButton()
        Me.rbWhereMplabxFolder = New System.Windows.Forms.RadioButton()
        Me.rbWhereMplabxParentFolder = New System.Windows.Forms.RadioButton()
        Me.rbWhereVgddFolder = New System.Windows.Forms.RadioButton()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblVGDDToolChain = New System.Windows.Forms.Label()
        Me.lblWarning = New System.Windows.Forms.Label()
        Me.btnVGDDSettings = New System.Windows.Forms.Button()
        Me.Framework1 = New Framework()
        Me.grpMplabXIpcPlugin = New System.Windows.Forms.GroupBox()
        Me.chkUseIPC = New System.Windows.Forms.CheckBox()
        Me.pnlIpcConnect = New System.Windows.Forms.Panel()
        Me.btnMplabXIpcConnect = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.txtIpcIpAddress = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblMplabXIpcConnectionStatus = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.grpMplabX.SuspendLayout()
        Me.grpWhere.SuspendLayout()
        Me.grpMplabXIpcPlugin.SuspendLayout()
        Me.pnlIpcConnect.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpMplabX
        '
        Me.grpMplabX.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpMplabX.Controls.Add(Me.lblMplabXToolChain)
        Me.grpMplabX.Controls.Add(Me.Label1)
        Me.grpMplabX.Controls.Add(Me.lblMplabXConfig)
        Me.grpMplabX.Controls.Add(Me.Label5)
        Me.grpMplabX.Controls.Add(Me.btnChangeMplabXProject)
        Me.grpMplabX.Controls.Add(Me.lblMplabXProject)
        Me.grpMplabX.Controls.Add(Me.Label2)
        Me.grpMplabX.Location = New System.Drawing.Point(293, 65)
        Me.grpMplabX.Name = "grpMplabX"
        Me.grpMplabX.Size = New System.Drawing.Size(560, 69)
        Me.grpMplabX.TabIndex = 21
        Me.grpMplabX.TabStop = False
        Me.grpMplabX.Text = "MPLAB X Project - Manual definitions"
        '
        'lblMplabXToolChain
        '
        Me.lblMplabXToolChain.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMplabXToolChain.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMplabXToolChain.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblMplabXToolChain.Location = New System.Drawing.Point(112, 52)
        Me.lblMplabXToolChain.Name = "lblMplabXToolChain"
        Me.lblMplabXToolChain.Size = New System.Drawing.Size(374, 13)
        Me.lblMplabXToolChain.TabIndex = 9
        Me.lblMplabXToolChain.Text = "<ToolChain>"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 52)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(108, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Language Toolchain:"
        '
        'lblMplabXConfig
        '
        Me.lblMplabXConfig.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMplabXConfig.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMplabXConfig.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblMplabXConfig.Location = New System.Drawing.Point(98, 35)
        Me.lblMplabXConfig.Name = "lblMplabXConfig"
        Me.lblMplabXConfig.Size = New System.Drawing.Size(388, 17)
        Me.lblMplabXConfig.TabIndex = 7
        Me.lblMplabXConfig.Text = "default"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 35)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(85, 13)
        Me.Label5.TabIndex = 6
        Me.Label5.Text = "Selected Config:"
        '
        'btnChangeMplabXProject
        '
        Me.btnChangeMplabXProject.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnChangeMplabXProject.BackColor = System.Drawing.SystemColors.Control
        Me.btnChangeMplabXProject.Location = New System.Drawing.Point(492, 40)
        Me.btnChangeMplabXProject.Name = "btnChangeMplabXProject"
        Me.btnChangeMplabXProject.Size = New System.Drawing.Size(62, 23)
        Me.btnChangeMplabXProject.TabIndex = 11
        Me.btnChangeMplabXProject.Text = "Change"
        Me.btnChangeMplabXProject.UseVisualStyleBackColor = False
        '
        'lblMplabXProject
        '
        Me.lblMplabXProject.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMplabXProject.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMplabXProject.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblMplabXProject.Location = New System.Drawing.Point(142, 16)
        Me.lblMplabXProject.Name = "lblMplabXProject"
        Me.lblMplabXProject.Size = New System.Drawing.Size(407, 16)
        Me.lblMplabXProject.TabIndex = 1
        Me.lblMplabXProject.Text = "<Select MPLAB X Project>"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(137, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Selected MPLAB X Project:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 138)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(129, 13)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "Code will be generated in:"
        '
        'lblCodeGenPath
        '
        Me.lblCodeGenPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCodeGenPath.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblCodeGenPath.Location = New System.Drawing.Point(3, 154)
        Me.lblCodeGenPath.Name = "lblCodeGenPath"
        Me.lblCodeGenPath.Size = New System.Drawing.Size(284, 88)
        Me.lblCodeGenPath.TabIndex = 19
        Me.lblCodeGenPath.Text = "<CodeGenPath>"
        '
        'grpWhere
        '
        Me.grpWhere.Controls.Add(Me.rbWhereMplabxConfigFolder)
        Me.grpWhere.Controls.Add(Me.btnDestPathOther)
        Me.grpWhere.Controls.Add(Me.txtCodeGenPathOther)
        Me.grpWhere.Controls.Add(Me.rbWhereOtherFolder)
        Me.grpWhere.Controls.Add(Me.rbWhereMplabxFolder)
        Me.grpWhere.Controls.Add(Me.rbWhereMplabxParentFolder)
        Me.grpWhere.Controls.Add(Me.rbWhereVgddFolder)
        Me.grpWhere.Location = New System.Drawing.Point(0, 0)
        Me.grpWhere.Name = "grpWhere"
        Me.grpWhere.Size = New System.Drawing.Size(287, 134)
        Me.grpWhere.TabIndex = 18
        Me.grpWhere.TabStop = False
        Me.grpWhere.Text = "Generate Source Files"
        '
        'rbWhereMplabxConfigFolder
        '
        Me.rbWhereMplabxConfigFolder.AutoSize = True
        Me.rbWhereMplabxConfigFolder.CausesValidation = False
        Me.rbWhereMplabxConfigFolder.Checked = True
        Me.rbWhereMplabxConfigFolder.Location = New System.Drawing.Point(6, 13)
        Me.rbWhereMplabxConfigFolder.Name = "rbWhereMplabxConfigFolder"
        Me.rbWhereMplabxConfigFolder.Size = New System.Drawing.Size(248, 17)
        Me.rbWhereMplabxConfigFolder.TabIndex = 0
        Me.rbWhereMplabxConfigFolder.TabStop = True
        Me.rbWhereMplabxConfigFolder.Text = "In MPLAB X selected config Folder (suggested)"
        Me.rbWhereMplabxConfigFolder.UseVisualStyleBackColor = True
        '
        'btnDestPathOther
        '
        Me.btnDestPathOther.Location = New System.Drawing.Point(247, 101)
        Me.btnDestPathOther.Name = "btnDestPathOther"
        Me.btnDestPathOther.Size = New System.Drawing.Size(34, 23)
        Me.btnDestPathOther.TabIndex = 6
        Me.btnDestPathOther.Text = "..."
        Me.btnDestPathOther.UseVisualStyleBackColor = True
        '
        'txtCodeGenPathOther
        '
        Me.txtCodeGenPathOther.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCodeGenPathOther.Location = New System.Drawing.Point(54, 103)
        Me.txtCodeGenPathOther.Name = "txtCodeGenPathOther"
        Me.txtCodeGenPathOther.ReadOnly = True
        Me.txtCodeGenPathOther.Size = New System.Drawing.Size(187, 20)
        Me.txtCodeGenPathOther.TabIndex = 5
        '
        'rbWhereOtherFolder
        '
        Me.rbWhereOtherFolder.AutoSize = True
        Me.rbWhereOtherFolder.CausesValidation = False
        Me.rbWhereOtherFolder.Location = New System.Drawing.Point(6, 105)
        Me.rbWhereOtherFolder.Name = "rbWhereOtherFolder"
        Me.rbWhereOtherFolder.Size = New System.Drawing.Size(54, 17)
        Me.rbWhereOtherFolder.TabIndex = 4
        Me.rbWhereOtherFolder.Text = "Other:"
        Me.rbWhereOtherFolder.UseVisualStyleBackColor = True
        '
        'rbWhereMplabxFolder
        '
        Me.rbWhereMplabxFolder.AutoSize = True
        Me.rbWhereMplabxFolder.CausesValidation = False
        Me.rbWhereMplabxFolder.Location = New System.Drawing.Point(6, 82)
        Me.rbWhereMplabxFolder.Name = "rbWhereMplabxFolder"
        Me.rbWhereMplabxFolder.Size = New System.Drawing.Size(158, 17)
        Me.rbWhereMplabxFolder.TabIndex = 3
        Me.rbWhereMplabxFolder.Text = "In MPLAB X Project's Folder"
        Me.rbWhereMplabxFolder.UseVisualStyleBackColor = True
        '
        'rbWhereMplabxParentFolder
        '
        Me.rbWhereMplabxParentFolder.AutoSize = True
        Me.rbWhereMplabxParentFolder.CausesValidation = False
        Me.rbWhereMplabxParentFolder.Location = New System.Drawing.Point(6, 36)
        Me.rbWhereMplabxParentFolder.Name = "rbWhereMplabxParentFolder"
        Me.rbWhereMplabxParentFolder.Size = New System.Drawing.Size(192, 17)
        Me.rbWhereMplabxParentFolder.TabIndex = 1
        Me.rbWhereMplabxParentFolder.Text = "In MPLAB X Project's Parent Folder"
        Me.rbWhereMplabxParentFolder.UseVisualStyleBackColor = True
        '
        'rbWhereVgddFolder
        '
        Me.rbWhereVgddFolder.AutoSize = True
        Me.rbWhereVgddFolder.CausesValidation = False
        Me.rbWhereVgddFolder.Location = New System.Drawing.Point(6, 59)
        Me.rbWhereVgddFolder.Name = "rbWhereVgddFolder"
        Me.rbWhereVgddFolder.Size = New System.Drawing.Size(143, 17)
        Me.rbWhereVgddFolder.TabIndex = 2
        Me.rbWhereVgddFolder.Text = "In VGDD Project's Folder"
        Me.rbWhereVgddFolder.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(300, 137)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(131, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "VGDD Language settings:"
        '
        'lblVGDDToolChain
        '
        Me.lblVGDDToolChain.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblVGDDToolChain.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVGDDToolChain.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblVGDDToolChain.Location = New System.Drawing.Point(437, 137)
        Me.lblVGDDToolChain.Name = "lblVGDDToolChain"
        Me.lblVGDDToolChain.Size = New System.Drawing.Size(148, 13)
        Me.lblVGDDToolChain.TabIndex = 10
        Me.lblVGDDToolChain.Text = "<VGDDToolChain>"
        '
        'lblWarning
        '
        Me.lblWarning.AutoSize = True
        Me.lblWarning.ForeColor = System.Drawing.Color.Red
        Me.lblWarning.Location = New System.Drawing.Point(300, 154)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(306, 13)
        Me.lblWarning.TabIndex = 22
        Me.lblWarning.Text = "Selected MPLAB X config ({0}) / VGDD Settings ({1}) mismatch"
        Me.lblWarning.Visible = False
        '
        'btnVGDDSettings
        '
        Me.btnVGDDSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnVGDDSettings.BackColor = System.Drawing.SystemColors.Control
        Me.btnVGDDSettings.Image = Global.My.Resources.Resources.Tools
        Me.btnVGDDSettings.Location = New System.Drawing.Point(796, 138)
        Me.btnVGDDSettings.Name = "btnVGDDSettings"
        Me.btnVGDDSettings.Size = New System.Drawing.Size(38, 34)
        Me.btnVGDDSettings.TabIndex = 10
        Me.btnVGDDSettings.UseVisualStyleBackColor = False
        Me.btnVGDDSettings.Visible = False
        '
        'Framework1
        '
        Me.Framework1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Framework1.Location = New System.Drawing.Point(293, 174)
        Me.Framework1.Name = "Framework1"
        Me.Framework1.Size = New System.Drawing.Size(560, 68)
        Me.Framework1.TabIndex = 13
        Me.Framework1.Title = "Framework"
        '
        'grpMplabXIpcPlugin
        '
        Me.grpMplabXIpcPlugin.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpMplabXIpcPlugin.Controls.Add(Me.chkUseIPC)
        Me.grpMplabXIpcPlugin.Controls.Add(Me.pnlIpcConnect)
        Me.grpMplabXIpcPlugin.Controls.Add(Me.lblMplabXIpcConnectionStatus)
        Me.grpMplabXIpcPlugin.Controls.Add(Me.Label8)
        Me.grpMplabXIpcPlugin.Location = New System.Drawing.Point(294, 4)
        Me.grpMplabXIpcPlugin.Name = "grpMplabXIpcPlugin"
        Me.grpMplabXIpcPlugin.Size = New System.Drawing.Size(559, 58)
        Me.grpMplabXIpcPlugin.TabIndex = 24
        Me.grpMplabXIpcPlugin.TabStop = False
        Me.grpMplabXIpcPlugin.Text = "MPLAB X VGDD-Link Plug-in"
        '
        'chkUseIPC
        '
        Me.chkUseIPC.AutoSize = True
        Me.chkUseIPC.Location = New System.Drawing.Point(9, 14)
        Me.chkUseIPC.Name = "chkUseIPC"
        Me.chkUseIPC.Size = New System.Drawing.Size(296, 17)
        Me.chkUseIPC.TabIndex = 7
        Me.chkUseIPC.Text = "Connect to MPLAB X IDE running on PC with IP address:"
        Me.chkUseIPC.UseVisualStyleBackColor = True
        '
        'pnlIpcConnect
        '
        Me.pnlIpcConnect.Controls.Add(Me.btnMplabXIpcConnect)
        Me.pnlIpcConnect.Controls.Add(Me.TextBox1)
        Me.pnlIpcConnect.Controls.Add(Me.txtIpcIpAddress)
        Me.pnlIpcConnect.Controls.Add(Me.Label7)
        Me.pnlIpcConnect.Location = New System.Drawing.Point(305, 10)
        Me.pnlIpcConnect.Name = "pnlIpcConnect"
        Me.pnlIpcConnect.Size = New System.Drawing.Size(252, 23)
        Me.pnlIpcConnect.TabIndex = 16
        '
        'btnMplabXIpcConnect
        '
        Me.btnMplabXIpcConnect.BackColor = System.Drawing.SystemColors.Control
        Me.btnMplabXIpcConnect.Location = New System.Drawing.Point(179, -1)
        Me.btnMplabXIpcConnect.Name = "btnMplabXIpcConnect"
        Me.btnMplabXIpcConnect.Size = New System.Drawing.Size(62, 23)
        Me.btnMplabXIpcConnect.TabIndex = 10
        Me.btnMplabXIpcConnect.Text = "Connect"
        Me.btnMplabXIpcConnect.UseVisualStyleBackColor = False
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(123, 1)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(50, 20)
        Me.TextBox1.TabIndex = 9
        Me.TextBox1.Text = "4242"
        '
        'txtIpcIpAddress
        '
        Me.txtIpcIpAddress.Location = New System.Drawing.Point(2, 1)
        Me.txtIpcIpAddress.Name = "txtIpcIpAddress"
        Me.txtIpcIpAddress.Size = New System.Drawing.Size(80, 20)
        Me.txtIpcIpAddress.TabIndex = 8
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(88, 4)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(29, 13)
        Me.Label7.TabIndex = 10
        Me.Label7.Text = "Port:"
        '
        'lblMplabXIpcConnectionStatus
        '
        Me.lblMplabXIpcConnectionStatus.AutoSize = True
        Me.lblMplabXIpcConnectionStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMplabXIpcConnectionStatus.ForeColor = System.Drawing.Color.Red
        Me.lblMplabXIpcConnectionStatus.Location = New System.Drawing.Point(141, 34)
        Me.lblMplabXIpcConnectionStatus.Name = "lblMplabXIpcConnectionStatus"
        Me.lblMplabXIpcConnectionStatus.Size = New System.Drawing.Size(92, 13)
        Me.lblMplabXIpcConnectionStatus.TabIndex = 15
        Me.lblMplabXIpcConnectionStatus.Text = "Not Connected"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(6, 34)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(129, 13)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "Plug-in connection status:"
        '
        'CodeGenLocation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.grpMplabXIpcPlugin)
        Me.Controls.Add(Me.Framework1)
        Me.Controls.Add(Me.btnVGDDSettings)
        Me.Controls.Add(Me.lblWarning)
        Me.Controls.Add(Me.lblVGDDToolChain)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.grpMplabX)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblCodeGenPath)
        Me.Controls.Add(Me.grpWhere)
        Me.MinimumSize = New System.Drawing.Size(856, 277)
        Me.Name = "CodeGenLocation"
        Me.Size = New System.Drawing.Size(856, 277)
        Me.grpMplabX.ResumeLayout(False)
        Me.grpMplabX.PerformLayout()
        Me.grpWhere.ResumeLayout(False)
        Me.grpWhere.PerformLayout()
        Me.grpMplabXIpcPlugin.ResumeLayout(False)
        Me.grpMplabXIpcPlugin.PerformLayout()
        Me.pnlIpcConnect.ResumeLayout(False)
        Me.pnlIpcConnect.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents grpMplabX As System.Windows.Forms.GroupBox
    Friend WithEvents btnChangeMplabXProject As System.Windows.Forms.Button
    Friend WithEvents lblMplabXProject As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblCodeGenPath As System.Windows.Forms.Label
    Friend WithEvents grpWhere As System.Windows.Forms.GroupBox
    Friend WithEvents txtCodeGenPathOther As System.Windows.Forms.TextBox
    Friend WithEvents rbWhereOtherFolder As System.Windows.Forms.RadioButton
    Friend WithEvents rbWhereMplabxFolder As System.Windows.Forms.RadioButton
    Friend WithEvents rbWhereMplabxParentFolder As System.Windows.Forms.RadioButton
    Friend WithEvents rbWhereVgddFolder As System.Windows.Forms.RadioButton
    Friend WithEvents lblMplabXConfig As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblMplabXToolChain As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblVGDDToolChain As System.Windows.Forms.Label
    Friend WithEvents lblWarning As System.Windows.Forms.Label
    Friend WithEvents btnVGDDSettings As System.Windows.Forms.Button
    Friend WithEvents Framework1 As Framework
    Friend WithEvents grpMplabXIpcPlugin As System.Windows.Forms.GroupBox
    Friend WithEvents lblMplabXIpcConnectionStatus As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtIpcIpAddress As System.Windows.Forms.TextBox
    Friend WithEvents btnMplabXIpcConnect As System.Windows.Forms.Button
    Friend WithEvents pnlIpcConnect As System.Windows.Forms.Panel
    Friend WithEvents chkUseIPC As System.Windows.Forms.CheckBox
    Friend WithEvents btnDestPathOther As System.Windows.Forms.Button
    Friend WithEvents rbWhereMplabxConfigFolder As System.Windows.Forms.RadioButton

End Class
