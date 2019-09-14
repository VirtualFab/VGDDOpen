<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPreferences
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPreferences))
        Me.btnOk = New System.Windows.Forms.Button()
        Me.chkCopyBitmaps = New System.Windows.Forms.CheckBox()
        Me.chkBmpPrefix = New System.Windows.Forms.CheckBox()
        Me.btnUserTemplates = New System.Windows.Forms.Button()
        Me.lblUserTemplatesFolder = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.chkMakeBackups = New System.Windows.Forms.CheckBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtJavaCommand = New System.Windows.Forms.TextBox()
        Me.btnResetJavaCommand = New System.Windows.Forms.Button()
        Me.btnGetJava = New System.Windows.Forms.Button()
        Me.txtFallbackGRCPath = New System.Windows.Forms.TextBox()
        Me.btnSelectPathJava = New System.Windows.Forms.Button()
        Me.lblJavaLog = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnTestJava = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.Framework1 = New Framework()
        Me.grpFallbackGRC = New System.Windows.Forms.GroupBox()
        Me.btnSelectFallbackGRCPath = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.grpFallbackGRC.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.BackColor = System.Drawing.Color.DarkGreen
        Me.btnOk.ForeColor = System.Drawing.Color.Yellow
        Me.btnOk.Location = New System.Drawing.Point(485, 337)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(103, 23)
        Me.btnOk.TabIndex = 21
        Me.btnOk.Text = "Save"
        Me.ToolTip1.SetToolTip(Me.btnOk, "Click to save preferences")
        Me.btnOk.UseVisualStyleBackColor = False
        '
        'chkCopyBitmaps
        '
        Me.chkCopyBitmaps.AutoSize = True
        Me.chkCopyBitmaps.Location = New System.Drawing.Point(12, 132)
        Me.chkCopyBitmaps.Name = "chkCopyBitmaps"
        Me.chkCopyBitmaps.Size = New System.Drawing.Size(323, 17)
        Me.chkCopyBitmaps.TabIndex = 29
        Me.chkCopyBitmaps.Text = "BitmapChooser - Copy selected bitmaps to VGDD Project folder"
        Me.ToolTip1.SetToolTip(Me.chkCopyBitmaps, "Enable if you want your project to be portable: bitmaps will be copied from the s" & _
                "ource folder.")
        Me.chkCopyBitmaps.UseVisualStyleBackColor = True
        '
        'chkBmpPrefix
        '
        Me.chkBmpPrefix.AutoSize = True
        Me.chkBmpPrefix.Location = New System.Drawing.Point(12, 155)
        Me.chkBmpPrefix.Name = "chkBmpPrefix"
        Me.chkBmpPrefix.Size = New System.Drawing.Size(226, 17)
        Me.chkBmpPrefix.TabIndex = 36
        Me.chkBmpPrefix.Text = "Use ""bmp"" prefix for bitmaps object names"
        Me.ToolTip1.SetToolTip(Me.chkBmpPrefix, "Check if you want bitmaps internal names to be prefixed with ""bmp"" for a better m" & _
                "nemonic naming")
        Me.chkBmpPrefix.UseVisualStyleBackColor = True
        '
        'btnUserTemplates
        '
        Me.btnUserTemplates.BackColor = System.Drawing.Color.Honeydew
        Me.btnUserTemplates.Location = New System.Drawing.Point(131, 305)
        Me.btnUserTemplates.Name = "btnUserTemplates"
        Me.btnUserTemplates.Size = New System.Drawing.Size(62, 23)
        Me.btnUserTemplates.TabIndex = 39
        Me.btnUserTemplates.Text = "Select"
        Me.btnUserTemplates.UseVisualStyleBackColor = False
        '
        'lblUserTemplatesFolder
        '
        Me.lblUserTemplatesFolder.AutoSize = True
        Me.lblUserTemplatesFolder.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUserTemplatesFolder.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblUserTemplatesFolder.Location = New System.Drawing.Point(207, 310)
        Me.lblUserTemplatesFolder.Name = "lblUserTemplatesFolder"
        Me.lblUserTemplatesFolder.Size = New System.Drawing.Size(125, 13)
        Me.lblUserTemplatesFolder.TabIndex = 38
        Me.lblUserTemplatesFolder.Text = "<User Templates Folder>"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 310)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(116, 13)
        Me.Label2.TabIndex = 37
        Me.Label2.Text = "User Templates Folder:"
        '
        'chkMakeBackups
        '
        Me.chkMakeBackups.AutoSize = True
        Me.chkMakeBackups.Location = New System.Drawing.Point(12, 178)
        Me.chkMakeBackups.Name = "chkMakeBackups"
        Me.chkMakeBackups.Size = New System.Drawing.Size(190, 17)
        Me.chkMakeBackups.TabIndex = 40
        Me.chkMakeBackups.Text = "Make backups of the modified files"
        Me.ToolTip1.SetToolTip(Me.chkMakeBackups, "If checked, any file modified by VGDD will be copied inside ""Backup"" folder prior" & _
                " modifying it. Handy if you don't commit your project frequently")
        Me.chkMakeBackups.UseVisualStyleBackColor = True
        '
        'txtJavaCommand
        '
        Me.txtJavaCommand.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtJavaCommand.Location = New System.Drawing.Point(7, 19)
        Me.txtJavaCommand.Name = "txtJavaCommand"
        Me.txtJavaCommand.Size = New System.Drawing.Size(534, 20)
        Me.txtJavaCommand.TabIndex = 31
        Me.txtJavaCommand.Text = "java -jar"
        Me.ToolTip1.SetToolTip(Me.txtJavaCommand, "Java commandline for your system., needed to run GRC")
        '
        'btnResetJavaCommand
        '
        Me.btnResetJavaCommand.BackColor = System.Drawing.Color.Honeydew
        Me.btnResetJavaCommand.Location = New System.Drawing.Point(514, 40)
        Me.btnResetJavaCommand.Name = "btnResetJavaCommand"
        Me.btnResetJavaCommand.Size = New System.Drawing.Size(62, 23)
        Me.btnResetJavaCommand.TabIndex = 44
        Me.btnResetJavaCommand.Text = "Default"
        Me.ToolTip1.SetToolTip(Me.btnResetJavaCommand, "Resets Java command to the default ""Java.exe -jar""")
        Me.btnResetJavaCommand.UseVisualStyleBackColor = False
        Me.btnResetJavaCommand.Visible = False
        '
        'btnGetJava
        '
        Me.btnGetJava.BackColor = System.Drawing.Color.Honeydew
        Me.btnGetJava.Location = New System.Drawing.Point(514, 69)
        Me.btnGetJava.Name = "btnGetJava"
        Me.btnGetJava.Size = New System.Drawing.Size(62, 23)
        Me.btnGetJava.TabIndex = 45
        Me.btnGetJava.Text = "Get Java"
        Me.ToolTip1.SetToolTip(Me.btnGetJava, "Starts Java Download in the browser. You'll have  to restart VGDD after having in" & _
                "stalled Java")
        Me.btnGetJava.UseVisualStyleBackColor = False
        Me.btnGetJava.Visible = False
        '
        'txtFallbackGRCPath
        '
        Me.txtFallbackGRCPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFallbackGRCPath.Location = New System.Drawing.Point(7, 19)
        Me.txtFallbackGRCPath.Name = "txtFallbackGRCPath"
        Me.txtFallbackGRCPath.Size = New System.Drawing.Size(534, 20)
        Me.txtFallbackGRCPath.TabIndex = 31
        Me.txtFallbackGRCPath.Text = "grc.jar"
        Me.ToolTip1.SetToolTip(Me.txtFallbackGRCPath, "Java commandline for your system., needed to run GRC")
        '
        'btnSelectPathJava
        '
        Me.btnSelectPathJava.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectPathJava.Location = New System.Drawing.Point(547, 17)
        Me.btnSelectPathJava.Name = "btnSelectPathJava"
        Me.btnSelectPathJava.Size = New System.Drawing.Size(30, 23)
        Me.btnSelectPathJava.TabIndex = 32
        Me.btnSelectPathJava.Text = "..."
        Me.btnSelectPathJava.UseVisualStyleBackColor = True
        '
        'lblJavaLog
        '
        Me.lblJavaLog.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblJavaLog.Location = New System.Drawing.Point(88, 43)
        Me.lblJavaLog.Name = "lblJavaLog"
        Me.lblJavaLog.Size = New System.Drawing.Size(418, 52)
        Me.lblJavaLog.TabIndex = 34
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.btnGetJava)
        Me.GroupBox1.Controls.Add(Me.btnResetJavaCommand)
        Me.GroupBox1.Controls.Add(Me.btnTestJava)
        Me.GroupBox1.Controls.Add(Me.lblJavaLog)
        Me.GroupBox1.Controls.Add(Me.txtJavaCommand)
        Me.GroupBox1.Controls.Add(Me.btnSelectPathJava)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 201)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(583, 98)
        Me.GroupBox1.TabIndex = 35
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Java command"
        '
        'btnTestJava
        '
        Me.btnTestJava.BackColor = System.Drawing.Color.Honeydew
        Me.btnTestJava.Location = New System.Drawing.Point(7, 40)
        Me.btnTestJava.Name = "btnTestJava"
        Me.btnTestJava.Size = New System.Drawing.Size(75, 23)
        Me.btnTestJava.TabIndex = 35
        Me.btnTestJava.Text = "Test"
        Me.btnTestJava.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(12, 337)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(103, 23)
        Me.btnCancel.TabIndex = 43
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'Framework1
        '
        Me.Framework1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Framework1.Location = New System.Drawing.Point(9, 11)
        Me.Framework1.Name = "Framework1"
        Me.Framework1.Size = New System.Drawing.Size(583, 68)
        Me.Framework1.TabIndex = 42
        Me.Framework1.Title = "Default Framework"
        '
        'grpFallbackGRC
        '
        Me.grpFallbackGRC.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpFallbackGRC.Controls.Add(Me.txtFallbackGRCPath)
        Me.grpFallbackGRC.Controls.Add(Me.btnSelectFallbackGRCPath)
        Me.grpFallbackGRC.Location = New System.Drawing.Point(9, 82)
        Me.grpFallbackGRC.Name = "grpFallbackGRC"
        Me.grpFallbackGRC.Size = New System.Drawing.Size(583, 48)
        Me.grpFallbackGRC.TabIndex = 44
        Me.grpFallbackGRC.TabStop = False
        Me.grpFallbackGRC.Text = "Fallback GRC path"
        '
        'Button4
        '
        Me.btnSelectFallbackGRCPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectFallbackGRCPath.Location = New System.Drawing.Point(547, 17)
        Me.btnSelectFallbackGRCPath.Name = "Button4"
        Me.btnSelectFallbackGRCPath.Size = New System.Drawing.Size(30, 23)
        Me.btnSelectFallbackGRCPath.TabIndex = 32
        Me.btnSelectFallbackGRCPath.Text = "..."
        Me.btnSelectFallbackGRCPath.UseVisualStyleBackColor = True
        '
        'frmPreferences
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(601, 368)
        Me.ControlBox = False
        Me.Controls.Add(Me.grpFallbackGRC)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.Framework1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.chkMakeBackups)
        Me.Controls.Add(Me.btnUserTemplates)
        Me.Controls.Add(Me.lblUserTemplatesFolder)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.chkBmpPrefix)
        Me.Controls.Add(Me.chkCopyBitmaps)
        Me.Controls.Add(Me.btnOk)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPreferences"
        Me.Text = "Global Preferences"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.grpFallbackGRC.ResumeLayout(False)
        Me.grpFallbackGRC.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents chkCopyBitmaps As System.Windows.Forms.CheckBox
    Friend WithEvents chkBmpPrefix As System.Windows.Forms.CheckBox
    Friend WithEvents btnUserTemplates As System.Windows.Forms.Button
    Friend WithEvents lblUserTemplatesFolder As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents chkMakeBackups As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents txtJavaCommand As System.Windows.Forms.TextBox
    Friend WithEvents btnSelectPathJava As System.Windows.Forms.Button
    Friend WithEvents lblJavaLog As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Framework1 As Framework
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnTestJava As System.Windows.Forms.Button
    Friend WithEvents btnResetJavaCommand As System.Windows.Forms.Button
    Friend WithEvents btnGetJava As System.Windows.Forms.Button
    Friend WithEvents grpFallbackGRC As System.Windows.Forms.GroupBox
    Friend WithEvents txtFallbackGRCPath As System.Windows.Forms.TextBox
    Friend WithEvents btnSelectFallbackGRCPath As System.Windows.Forms.Button
End Class
