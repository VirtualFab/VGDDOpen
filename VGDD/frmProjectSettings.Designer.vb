<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmProjectSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmProjectSettings))
        Me.rbtColorDepth24 = New System.Windows.Forms.RadioButton()
        Me.grpColourDepth = New System.Windows.Forms.GroupBox()
        Me.rbtColorDepth2 = New System.Windows.Forms.RadioButton()
        Me.rbtColorDepth1 = New System.Windows.Forms.RadioButton()
        Me.rbtColorDepth16 = New System.Windows.Forms.RadioButton()
        Me.rbtColorDepth8 = New System.Windows.Forms.RadioButton()
        Me.rbtColorDepth4 = New System.Windows.Forms.RadioButton()
        Me.chkUseIndexedColours = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtDefaultWidth = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtDefaultHeight = New System.Windows.Forms.TextBox()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.chkUseMultiByteChars = New System.Windows.Forms.CheckBox()
        Me.rbtCompilerC30 = New System.Windows.Forms.RadioButton()
        Me.rbtCompilerC32 = New System.Windows.Forms.RadioButton()
        Me.grpCompiler = New System.Windows.Forms.GroupBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.rbtCompilerXC32 = New System.Windows.Forms.RadioButton()
        Me.rbtCompilerXC16 = New System.Windows.Forms.RadioButton()
        Me.grpSize = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.chkSwapWH = New System.Windows.Forms.CheckBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.chkExtendedModes = New System.Windows.Forms.CheckBox()
        Me.btnMigrate = New System.Windows.Forms.Button()
        Me.cmbQuick = New System.Windows.Forms.ComboBox()
        Me.btnPreferences = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.chkStringsPoolHeader = New System.Windows.Forms.CheckBox()
        Me.MultiLanguageTranslations = New System.Windows.Forms.NumericUpDown()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnStringsPool = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.ActiveLanguage = New System.Windows.Forms.NumericUpDown()
        Me.Framework1 = New Framework()
        Me.grpColourDepth.SuspendLayout()
        Me.grpCompiler.SuspendLayout()
        Me.grpSize.SuspendLayout()
        CType(Me.MultiLanguageTranslations, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.ActiveLanguage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rbtColorDepth24
        '
        Me.rbtColorDepth24.AutoSize = True
        Me.rbtColorDepth24.Location = New System.Drawing.Point(4, 134)
        Me.rbtColorDepth24.Name = "rbtColorDepth24"
        Me.rbtColorDepth24.Size = New System.Drawing.Size(113, 17)
        Me.rbtColorDepth24.TabIndex = 15
        Me.rbtColorDepth24.Text = "24 bit - 2M Colours"
        Me.rbtColorDepth24.UseVisualStyleBackColor = True
        '
        'grpColourDepth
        '
        Me.grpColourDepth.Controls.Add(Me.rbtColorDepth2)
        Me.grpColourDepth.Controls.Add(Me.rbtColorDepth1)
        Me.grpColourDepth.Controls.Add(Me.rbtColorDepth24)
        Me.grpColourDepth.Controls.Add(Me.rbtColorDepth16)
        Me.grpColourDepth.Controls.Add(Me.rbtColorDepth8)
        Me.grpColourDepth.Controls.Add(Me.rbtColorDepth4)
        Me.grpColourDepth.Location = New System.Drawing.Point(6, 131)
        Me.grpColourDepth.Name = "grpColourDepth"
        Me.grpColourDepth.Size = New System.Drawing.Size(226, 160)
        Me.grpColourDepth.TabIndex = 19
        Me.grpColourDepth.TabStop = False
        Me.grpColourDepth.Text = "Colour Depth"
        '
        'rbtColorDepth2
        '
        Me.rbtColorDepth2.AutoSize = True
        Me.rbtColorDepth2.Location = New System.Drawing.Point(4, 42)
        Me.rbtColorDepth2.Name = "rbtColorDepth2"
        Me.rbtColorDepth2.Size = New System.Drawing.Size(98, 17)
        Me.rbtColorDepth2.TabIndex = 17
        Me.rbtColorDepth2.Text = "2 bit - 4 Colours"
        Me.rbtColorDepth2.UseVisualStyleBackColor = True
        '
        'rbtColorDepth1
        '
        Me.rbtColorDepth1.AutoSize = True
        Me.rbtColorDepth1.Location = New System.Drawing.Point(4, 19)
        Me.rbtColorDepth1.Name = "rbtColorDepth1"
        Me.rbtColorDepth1.Size = New System.Drawing.Size(77, 17)
        Me.rbtColorDepth1.TabIndex = 16
        Me.rbtColorDepth1.Text = "1 bit - B/W"
        Me.rbtColorDepth1.UseVisualStyleBackColor = True
        '
        'rbtColorDepth16
        '
        Me.rbtColorDepth16.AutoSize = True
        Me.rbtColorDepth16.Checked = True
        Me.rbtColorDepth16.Location = New System.Drawing.Point(4, 111)
        Me.rbtColorDepth16.Name = "rbtColorDepth16"
        Me.rbtColorDepth16.Size = New System.Drawing.Size(116, 17)
        Me.rbtColorDepth16.TabIndex = 13
        Me.rbtColorDepth16.TabStop = True
        Me.rbtColorDepth16.Text = "16 bit - 64k Colours"
        Me.rbtColorDepth16.UseVisualStyleBackColor = True
        '
        'rbtColorDepth8
        '
        Me.rbtColorDepth8.AutoSize = True
        Me.rbtColorDepth8.Location = New System.Drawing.Point(4, 88)
        Me.rbtColorDepth8.Name = "rbtColorDepth8"
        Me.rbtColorDepth8.Size = New System.Drawing.Size(110, 17)
        Me.rbtColorDepth8.TabIndex = 12
        Me.rbtColorDepth8.Text = "8 bit - 256 Colours"
        Me.rbtColorDepth8.UseVisualStyleBackColor = True
        '
        'rbtColorDepth4
        '
        Me.rbtColorDepth4.AutoSize = True
        Me.rbtColorDepth4.Location = New System.Drawing.Point(4, 65)
        Me.rbtColorDepth4.Name = "rbtColorDepth4"
        Me.rbtColorDepth4.Size = New System.Drawing.Size(104, 17)
        Me.rbtColorDepth4.TabIndex = 11
        Me.rbtColorDepth4.Text = "4 bit - 16 Colours"
        Me.rbtColorDepth4.UseVisualStyleBackColor = True
        '
        'chkUseIndexedColours
        '
        Me.chkUseIndexedColours.AutoSize = True
        Me.chkUseIndexedColours.Location = New System.Drawing.Point(6, 297)
        Me.chkUseIndexedColours.Name = "chkUseIndexedColours"
        Me.chkUseIndexedColours.Size = New System.Drawing.Size(200, 17)
        Me.chkUseIndexedColours.TabIndex = 20
        Me.chkUseIndexedColours.Text = "Use Indexed Custom/Palette Colours"
        Me.chkUseIndexedColours.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(16, 17)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "Width"
        '
        'txtDefaultWidth
        '
        Me.txtDefaultWidth.Location = New System.Drawing.Point(6, 33)
        Me.txtDefaultWidth.Name = "txtDefaultWidth"
        Me.txtDefaultWidth.Size = New System.Drawing.Size(45, 20)
        Me.txtDefaultWidth.TabIndex = 15
        Me.txtDefaultWidth.Text = "320"
        Me.txtDefaultWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(82, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(38, 13)
        Me.Label2.TabIndex = 16
        Me.Label2.Text = "Height"
        '
        'txtDefaultHeight
        '
        Me.txtDefaultHeight.Location = New System.Drawing.Point(75, 33)
        Me.txtDefaultHeight.Name = "txtDefaultHeight"
        Me.txtDefaultHeight.Size = New System.Drawing.Size(45, 20)
        Me.txtDefaultHeight.TabIndex = 17
        Me.txtDefaultHeight.Text = "240"
        Me.txtDefaultHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.BackColor = System.Drawing.Color.DarkGreen
        Me.btnOk.ForeColor = System.Drawing.Color.Yellow
        Me.btnOk.Location = New System.Drawing.Point(413, 11)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(95, 23)
        Me.btnOk.TabIndex = 21
        Me.btnOk.Text = "Ok"
        Me.btnOk.UseVisualStyleBackColor = False
        '
        'chkUseMultiByteChars
        '
        Me.chkUseMultiByteChars.AutoSize = True
        Me.chkUseMultiByteChars.Location = New System.Drawing.Point(251, 132)
        Me.chkUseMultiByteChars.Name = "chkUseMultiByteChars"
        Me.chkUseMultiByteChars.Size = New System.Drawing.Size(121, 17)
        Me.chkUseMultiByteChars.TabIndex = 22
        Me.chkUseMultiByteChars.Text = "Use MultiByte Chars"
        Me.chkUseMultiByteChars.UseVisualStyleBackColor = True
        '
        'rbtCompilerC30
        '
        Me.rbtCompilerC30.AutoSize = True
        Me.rbtCompilerC30.Location = New System.Drawing.Point(19, 32)
        Me.rbtCompilerC30.Name = "rbtCompilerC30"
        Me.rbtCompilerC30.Size = New System.Drawing.Size(44, 17)
        Me.rbtCompilerC30.TabIndex = 23
        Me.rbtCompilerC30.Text = "C30"
        Me.rbtCompilerC30.UseVisualStyleBackColor = True
        '
        'rbtCompilerC32
        '
        Me.rbtCompilerC32.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rbtCompilerC32.AutoSize = True
        Me.rbtCompilerC32.Checked = True
        Me.rbtCompilerC32.Location = New System.Drawing.Point(162, 32)
        Me.rbtCompilerC32.Name = "rbtCompilerC32"
        Me.rbtCompilerC32.Size = New System.Drawing.Size(44, 17)
        Me.rbtCompilerC32.TabIndex = 24
        Me.rbtCompilerC32.TabStop = True
        Me.rbtCompilerC32.Text = "C32"
        Me.rbtCompilerC32.UseVisualStyleBackColor = True
        '
        'grpCompiler
        '
        Me.grpCompiler.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpCompiler.Controls.Add(Me.Label9)
        Me.grpCompiler.Controls.Add(Me.Label8)
        Me.grpCompiler.Controls.Add(Me.rbtCompilerXC32)
        Me.grpCompiler.Controls.Add(Me.rbtCompilerXC16)
        Me.grpCompiler.Controls.Add(Me.rbtCompilerC30)
        Me.grpCompiler.Controls.Add(Me.rbtCompilerC32)
        Me.grpCompiler.Location = New System.Drawing.Point(251, 47)
        Me.grpCompiler.Name = "grpCompiler"
        Me.grpCompiler.Size = New System.Drawing.Size(259, 79)
        Me.grpCompiler.TabIndex = 25
        Me.grpCompiler.TabStop = False
        Me.grpCompiler.Text = "Compiler"
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(159, 16)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(36, 13)
        Me.Label9.TabIndex = 28
        Me.Label9.Text = "PIC32"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(21, 16)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(81, 13)
        Me.Label8.TabIndex = 27
        Me.Label8.Text = "PIC24/dsPIC33"
        '
        'rbtCompilerXC32
        '
        Me.rbtCompilerXC32.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rbtCompilerXC32.AutoSize = True
        Me.rbtCompilerXC32.Location = New System.Drawing.Point(162, 55)
        Me.rbtCompilerXC32.Name = "rbtCompilerXC32"
        Me.rbtCompilerXC32.Size = New System.Drawing.Size(51, 17)
        Me.rbtCompilerXC32.TabIndex = 26
        Me.rbtCompilerXC32.Text = "XC32"
        Me.rbtCompilerXC32.UseVisualStyleBackColor = True
        '
        'rbtCompilerXC16
        '
        Me.rbtCompilerXC16.AutoSize = True
        Me.rbtCompilerXC16.Location = New System.Drawing.Point(19, 55)
        Me.rbtCompilerXC16.Name = "rbtCompilerXC16"
        Me.rbtCompilerXC16.Size = New System.Drawing.Size(51, 17)
        Me.rbtCompilerXC16.TabIndex = 25
        Me.rbtCompilerXC16.Text = "XC16"
        Me.rbtCompilerXC16.UseVisualStyleBackColor = True
        '
        'grpSize
        '
        Me.grpSize.Controls.Add(Me.Label4)
        Me.grpSize.Controls.Add(Me.chkSwapWH)
        Me.grpSize.Controls.Add(Me.Label3)
        Me.grpSize.Controls.Add(Me.chkExtendedModes)
        Me.grpSize.Controls.Add(Me.btnMigrate)
        Me.grpSize.Controls.Add(Me.cmbQuick)
        Me.grpSize.Controls.Add(Me.txtDefaultWidth)
        Me.grpSize.Controls.Add(Me.Label1)
        Me.grpSize.Controls.Add(Me.txtDefaultHeight)
        Me.grpSize.Controls.Add(Me.Label2)
        Me.grpSize.Location = New System.Drawing.Point(6, 3)
        Me.grpSize.Name = "grpSize"
        Me.grpSize.Size = New System.Drawing.Size(226, 122)
        Me.grpSize.TabIndex = 33
        Me.grpSize.TabStop = False
        Me.grpSize.Text = "Default Screen Size"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(57, 36)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(12, 13)
        Me.Label4.TabIndex = 23
        Me.Label4.Text = "x"
        '
        'chkSwapWH
        '
        Me.chkSwapWH.AutoSize = True
        Me.chkSwapWH.Location = New System.Drawing.Point(25, 56)
        Me.chkSwapWH.Name = "chkSwapWH"
        Me.chkSwapWH.Size = New System.Drawing.Size(80, 17)
        Me.chkSwapWH.TabIndex = 22
        Me.chkSwapWH.Text = "Swap W/H"
        Me.chkSwapWH.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(2, 79)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(72, 13)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "Quick setting:"
        '
        'chkExtendedModes
        '
        Me.chkExtendedModes.AutoSize = True
        Me.chkExtendedModes.Location = New System.Drawing.Point(75, 99)
        Me.chkExtendedModes.Name = "chkExtendedModes"
        Me.chkExtendedModes.Size = New System.Drawing.Size(106, 17)
        Me.chkExtendedModes.TabIndex = 21
        Me.chkExtendedModes.Text = "Extended Modes"
        Me.chkExtendedModes.UseVisualStyleBackColor = True
        '
        'btnMigrate
        '
        Me.btnMigrate.Enabled = False
        Me.btnMigrate.Location = New System.Drawing.Point(145, 30)
        Me.btnMigrate.Name = "btnMigrate"
        Me.btnMigrate.Size = New System.Drawing.Size(75, 23)
        Me.btnMigrate.TabIndex = 18
        Me.btnMigrate.Text = "Migrate"
        Me.btnMigrate.UseVisualStyleBackColor = True
        '
        'cmbQuick
        '
        Me.cmbQuick.FormattingEnabled = True
        Me.cmbQuick.Location = New System.Drawing.Point(75, 76)
        Me.cmbQuick.Name = "cmbQuick"
        Me.cmbQuick.Size = New System.Drawing.Size(146, 21)
        Me.cmbQuick.TabIndex = 19
        '
        'btnPreferences
        '
        Me.btnPreferences.Location = New System.Drawing.Point(251, 11)
        Me.btnPreferences.Name = "btnPreferences"
        Me.btnPreferences.Size = New System.Drawing.Size(121, 23)
        Me.btnPreferences.TabIndex = 34
        Me.btnPreferences.Text = "Global Preferences"
        Me.btnPreferences.UseVisualStyleBackColor = True
        '
        'chkStringsPoolHeader
        '
        Me.chkStringsPoolHeader.AutoSize = True
        Me.chkStringsPoolHeader.Location = New System.Drawing.Point(12, 65)
        Me.chkStringsPoolHeader.Name = "chkStringsPoolHeader"
        Me.chkStringsPoolHeader.Size = New System.Drawing.Size(202, 17)
        Me.chkStringsPoolHeader.TabIndex = 42
        Me.chkStringsPoolHeader.Text = "CodeGen: Create StringsPool Header"
        Me.ToolTip1.SetToolTip(Me.chkStringsPoolHeader, "Check if you want to generate header file StringsPool.h ")
        Me.chkStringsPoolHeader.UseVisualStyleBackColor = True
        '
        'MultiLanguageTranslations
        '
        Me.MultiLanguageTranslations.Location = New System.Drawing.Point(134, 14)
        Me.MultiLanguageTranslations.Name = "MultiLanguageTranslations"
        Me.MultiLanguageTranslations.Size = New System.Drawing.Size(43, 20)
        Me.MultiLanguageTranslations.TabIndex = 37
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(9, 16)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(119, 13)
        Me.Label10.TabIndex = 38
        Me.Label10.Text = "Number of Translations:"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.chkStringsPoolHeader)
        Me.GroupBox1.Controls.Add(Me.btnStringsPool)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.ActiveLanguage)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.MultiLanguageTranslations)
        Me.GroupBox1.Location = New System.Drawing.Point(251, 155)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(247, 136)
        Me.GroupBox1.TabIndex = 40
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Multilanguage Strings Pool (experimental)"
        '
        'btnStringsPool
        '
        Me.btnStringsPool.Location = New System.Drawing.Point(13, 104)
        Me.btnStringsPool.Name = "btnStringsPool"
        Me.btnStringsPool.Size = New System.Drawing.Size(108, 23)
        Me.btnStringsPool.TabIndex = 41
        Me.btnStringsPool.Text = "Edit Strings Pool"
        Me.btnStringsPool.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(9, 40)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(91, 13)
        Me.Label11.TabIndex = 40
        Me.Label11.Text = "Active Language:"
        '
        'ActiveLanguage
        '
        Me.ActiveLanguage.Location = New System.Drawing.Point(134, 38)
        Me.ActiveLanguage.Name = "ActiveLanguage"
        Me.ActiveLanguage.Size = New System.Drawing.Size(43, 20)
        Me.ActiveLanguage.TabIndex = 39
        '
        'Framework1
        '
        Me.Framework1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Framework1.Location = New System.Drawing.Point(4, 320)
        Me.Framework1.Name = "Framework1"
        Me.Framework1.Size = New System.Drawing.Size(506, 68)
        Me.Framework1.TabIndex = 41
        Me.Framework1.Title = "Framework"
        '
        'frmProjectSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(528, 396)
        Me.ControlBox = False
        Me.Controls.Add(Me.Framework1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnPreferences)
        Me.Controls.Add(Me.chkUseIndexedColours)
        Me.Controls.Add(Me.grpCompiler)
        Me.Controls.Add(Me.chkUseMultiByteChars)
        Me.Controls.Add(Me.grpColourDepth)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.grpSize)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(530, 421)
        Me.Name = "frmProjectSettings"
        Me.Text = "Project Settings"
        Me.grpColourDepth.ResumeLayout(False)
        Me.grpColourDepth.PerformLayout()
        Me.grpCompiler.ResumeLayout(False)
        Me.grpCompiler.PerformLayout()
        Me.grpSize.ResumeLayout(False)
        Me.grpSize.PerformLayout()
        CType(Me.MultiLanguageTranslations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.ActiveLanguage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rbtColorDepth24 As System.Windows.Forms.RadioButton
    Friend WithEvents grpColourDepth As System.Windows.Forms.GroupBox
    Friend WithEvents rbtColorDepth16 As System.Windows.Forms.RadioButton
    Friend WithEvents rbtColorDepth8 As System.Windows.Forms.RadioButton
    Friend WithEvents rbtColorDepth4 As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents chkUseIndexedColours As System.Windows.Forms.CheckBox
    Friend WithEvents txtDefaultWidth As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtDefaultHeight As System.Windows.Forms.TextBox
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents chkUseMultiByteChars As System.Windows.Forms.CheckBox
    Friend WithEvents rbtCompilerC30 As System.Windows.Forms.RadioButton
    Friend WithEvents rbtCompilerC32 As System.Windows.Forms.RadioButton
    Friend WithEvents grpCompiler As System.Windows.Forms.GroupBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents rbtCompilerXC32 As System.Windows.Forms.RadioButton
    Friend WithEvents rbtCompilerXC16 As System.Windows.Forms.RadioButton
    Friend WithEvents grpSize As System.Windows.Forms.GroupBox
    Friend WithEvents btnMigrate As System.Windows.Forms.Button
    Friend WithEvents rbtColorDepth2 As System.Windows.Forms.RadioButton
    Friend WithEvents rbtColorDepth1 As System.Windows.Forms.RadioButton
    Friend WithEvents btnPreferences As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cmbQuick As System.Windows.Forms.ComboBox
    Friend WithEvents chkSwapWH As System.Windows.Forms.CheckBox
    Friend WithEvents chkExtendedModes As System.Windows.Forms.CheckBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents MultiLanguageTranslations As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents ActiveLanguage As System.Windows.Forms.NumericUpDown
    Friend WithEvents btnStringsPool As System.Windows.Forms.Button
    Friend WithEvents chkStringsPoolHeader As System.Windows.Forms.CheckBox
    Friend WithEvents Framework1 As Framework
End Class
