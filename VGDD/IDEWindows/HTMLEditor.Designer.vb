Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class HTMLEditor
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

        ' <summary> 
        ' Required method for Designer support - do not modify 
        ' the contents of this method with the code editor.
        ' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HTMLEditor))
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
            Me.CustomTabControl1 = New CustomTabControl()
            Me.pnlSettings = New System.Windows.Forms.Panel()
            Me.pnlGenerate = New System.Windows.Forms.Panel()
            Me.chkForce = New System.Windows.Forms.CheckBox()
            Me.pnlUpload = New System.Windows.Forms.Panel()
            Me.txtTargetUrl = New System.Windows.Forms.TextBox()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.txtPassword = New System.Windows.Forms.TextBox()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.txtUsername = New System.Windows.Forms.TextBox()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.chkAll = New System.Windows.Forms.CheckBox()
            Me.ListView1 = New System.Windows.Forms.ListView()
            Me.columnFile = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.columnDateTime = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.columnResult = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.btnUpload = New System.Windows.Forms.Button()
            Me.cmbType = New System.Windows.Forms.ComboBox()
            Me.btnGenerate = New System.Windows.Forms.Button()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.btnChooseFolder = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.lblWebPagesFolder = New System.Windows.Forms.Label()
            Me.WebBrowser2 = New System.Windows.Forms.WebBrowser()
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
            Me.NewToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.OpenToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.SaveToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
            Me.CutToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.CopyToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.PasteToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.HelpToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
            Me.chkPreview = New System.Windows.Forms.CheckBox()
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.btnSettings = New System.Windows.Forms.Button()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.pnlSettings.SuspendLayout()
            Me.pnlGenerate.SuspendLayout()
            Me.pnlUpload.SuspendLayout()
            Me.ToolStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.SplitContainer1.Location = New System.Drawing.Point(-2, 33)
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.CustomTabControl1)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.pnlSettings)
            Me.SplitContainer1.Panel2.Controls.Add(Me.WebBrowser2)
            Me.SplitContainer1.Size = New System.Drawing.Size(999, 351)
            Me.SplitContainer1.SplitterDistance = 454
            Me.SplitContainer1.TabIndex = 1
            '
            'CustomTabControl1
            '
            Me.CustomTabControl1.DisplayStyle = TabStyle.Chrome
            '
            '
            '
            Me.CustomTabControl1.DisplayStyleProvider.BorderColor = System.Drawing.SystemColors.ControlDark
            Me.CustomTabControl1.DisplayStyleProvider.BorderColorHot = System.Drawing.SystemColors.ControlDark
            Me.CustomTabControl1.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(157, Byte), Integer), CType(CType(185, Byte), Integer))
            Me.CustomTabControl1.DisplayStyleProvider.CloserColor = System.Drawing.Color.DarkGray
            Me.CustomTabControl1.DisplayStyleProvider.CloserColorActive = System.Drawing.Color.White
            Me.CustomTabControl1.DisplayStyleProvider.FocusTrack = False
            Me.CustomTabControl1.DisplayStyleProvider.HotTrack = True
            Me.CustomTabControl1.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.CustomTabControl1.DisplayStyleProvider.Opacity = 1.0!
            Me.CustomTabControl1.DisplayStyleProvider.Overlap = 16
            Me.CustomTabControl1.DisplayStyleProvider.Padding = New System.Drawing.Point(7, 5)
            Me.CustomTabControl1.DisplayStyleProvider.Radius = 16
            Me.CustomTabControl1.DisplayStyleProvider.ShowTabCloser = True
            Me.CustomTabControl1.DisplayStyleProvider.TextColor = System.Drawing.SystemColors.ControlText
            Me.CustomTabControl1.DisplayStyleProvider.TextColorDisabled = System.Drawing.SystemColors.ControlDark
            Me.CustomTabControl1.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText
            Me.CustomTabControl1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.CustomTabControl1.HotTrack = True
            Me.CustomTabControl1.Location = New System.Drawing.Point(0, 0)
            Me.CustomTabControl1.Name = "CustomTabControl1"
            Me.CustomTabControl1.SelectedIndex = 0
            Me.CustomTabControl1.Size = New System.Drawing.Size(454, 351)
            Me.CustomTabControl1.TabIndex = 3
            '
            'pnlSettings
            '
            Me.pnlSettings.Controls.Add(Me.pnlGenerate)
            Me.pnlSettings.Controls.Add(Me.btnChooseFolder)
            Me.pnlSettings.Controls.Add(Me.Label1)
            Me.pnlSettings.Controls.Add(Me.lblWebPagesFolder)
            Me.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlSettings.Location = New System.Drawing.Point(0, 0)
            Me.pnlSettings.Name = "pnlSettings"
            Me.pnlSettings.Size = New System.Drawing.Size(541, 351)
            Me.pnlSettings.TabIndex = 1
            Me.pnlSettings.Visible = False
            '
            'pnlGenerate
            '
            Me.pnlGenerate.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlGenerate.Controls.Add(Me.chkForce)
            Me.pnlGenerate.Controls.Add(Me.pnlUpload)
            Me.pnlGenerate.Controls.Add(Me.cmbType)
            Me.pnlGenerate.Controls.Add(Me.btnGenerate)
            Me.pnlGenerate.Controls.Add(Me.Label2)
            Me.pnlGenerate.Location = New System.Drawing.Point(17, 50)
            Me.pnlGenerate.Name = "pnlGenerate"
            Me.pnlGenerate.Size = New System.Drawing.Size(513, 298)
            Me.pnlGenerate.TabIndex = 13
            Me.pnlGenerate.Visible = False
            '
            'chkForce
            '
            Me.chkForce.AutoSize = True
            Me.chkForce.Location = New System.Drawing.Point(407, 8)
            Me.chkForce.Name = "chkForce"
            Me.chkForce.Size = New System.Drawing.Size(53, 17)
            Me.chkForce.TabIndex = 13
            Me.chkForce.Text = "Force"
            Me.ToolTip1.SetToolTip(Me.chkForce, "Force re-generation of HttpPrint.h")
            Me.chkForce.UseVisualStyleBackColor = True
            '
            'pnlUpload
            '
            Me.pnlUpload.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlUpload.Controls.Add(Me.txtTargetUrl)
            Me.pnlUpload.Controls.Add(Me.Label6)
            Me.pnlUpload.Controls.Add(Me.txtPassword)
            Me.pnlUpload.Controls.Add(Me.Label5)
            Me.pnlUpload.Controls.Add(Me.txtUsername)
            Me.pnlUpload.Controls.Add(Me.Label4)
            Me.pnlUpload.Controls.Add(Me.Label3)
            Me.pnlUpload.Controls.Add(Me.chkAll)
            Me.pnlUpload.Controls.Add(Me.ListView1)
            Me.pnlUpload.Controls.Add(Me.btnUpload)
            Me.pnlUpload.Location = New System.Drawing.Point(11, 31)
            Me.pnlUpload.Name = "pnlUpload"
            Me.pnlUpload.Size = New System.Drawing.Size(499, 257)
            Me.pnlUpload.TabIndex = 12
            Me.pnlUpload.Visible = False
            '
            'txtTargetUrl
            '
            Me.txtTargetUrl.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtTargetUrl.Location = New System.Drawing.Point(73, 6)
            Me.txtTargetUrl.Name = "txtTargetUrl"
            Me.txtTargetUrl.Size = New System.Drawing.Size(263, 21)
            Me.txtTargetUrl.TabIndex = 19
            Me.txtTargetUrl.Text = "http://GUIBOARD/protect/upload.htm"
            '
            'Label6
            '
            Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(2, 9)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(65, 13)
            Me.Label6.TabIndex = 18
            Me.Label6.Text = "Target URL:"
            '
            'txtPassword
            '
            Me.txtPassword.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtPassword.Location = New System.Drawing.Point(363, 75)
            Me.txtPassword.Name = "txtPassword"
            Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
            Me.txtPassword.Size = New System.Drawing.Size(133, 21)
            Me.txtPassword.TabIndex = 17
            '
            'Label5
            '
            Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(360, 58)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(57, 13)
            Me.Label5.TabIndex = 16
            Me.Label5.Text = "Password:"
            '
            'txtUsername
            '
            Me.txtUsername.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtUsername.Location = New System.Drawing.Point(363, 26)
            Me.txtUsername.Name = "txtUsername"
            Me.txtUsername.Size = New System.Drawing.Size(133, 21)
            Me.txtUsername.TabIndex = 15
            '
            'Label4
            '
            Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(360, 9)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(60, 13)
            Me.Label4.TabIndex = 14
            Me.Label4.Text = "UserName:"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(121, 30)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(76, 13)
            Me.Label3.TabIndex = 13
            Me.Label3.Text = "Files to upload"
            '
            'chkAll
            '
            Me.chkAll.AutoSize = True
            Me.chkAll.Location = New System.Drawing.Point(4, 29)
            Me.chkAll.Name = "chkAll"
            Me.chkAll.Size = New System.Drawing.Size(68, 17)
            Me.chkAll.TabIndex = 12
            Me.chkAll.Text = "Check all"
            Me.chkAll.UseVisualStyleBackColor = True
            '
            'ListView1
            '
            Me.ListView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ListView1.CheckBoxes = True
            Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnFile, Me.columnDateTime, Me.columnResult})
            Me.ListView1.Location = New System.Drawing.Point(3, 46)
            Me.ListView1.Name = "ListView1"
            Me.ListView1.Size = New System.Drawing.Size(350, 208)
            Me.ListView1.TabIndex = 11
            Me.ListView1.UseCompatibleStateImageBehavior = False
            Me.ListView1.View = System.Windows.Forms.View.Details
            '
            'columnFile
            '
            Me.columnFile.Text = "File"
            Me.columnFile.Width = 180
            '
            'columnDateTime
            '
            Me.columnDateTime.Text = "Modified"
            Me.columnDateTime.Width = 120
            '
            'columnResult
            '
            Me.columnResult.Text = "Result"
            Me.columnResult.Width = 300
            '
            'btnUpload
            '
            Me.btnUpload.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnUpload.Image = Global.My.Resources.Resources.Upload
            Me.btnUpload.ImageAlign = System.Drawing.ContentAlignment.TopCenter
            Me.btnUpload.Location = New System.Drawing.Point(387, 155)
            Me.btnUpload.Name = "btnUpload"
            Me.btnUpload.Size = New System.Drawing.Size(80, 47)
            Me.btnUpload.TabIndex = 10
            Me.btnUpload.Text = "Upload"
            Me.btnUpload.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolTip1.SetToolTip(Me.btnUpload, "Upload to target!")
            Me.btnUpload.UseVisualStyleBackColor = True
            '
            'cmbType
            '
            Me.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbType.FormattingEnabled = True
            Me.cmbType.Items.AddRange(New Object() {"Bin", "C module", "Files  (MDD)"})
            Me.cmbType.Location = New System.Drawing.Point(100, 4)
            Me.cmbType.Name = "cmbType"
            Me.cmbType.Size = New System.Drawing.Size(231, 21)
            Me.cmbType.TabIndex = 8
            '
            'btnGenerate
            '
            Me.btnGenerate.Enabled = False
            Me.btnGenerate.Location = New System.Drawing.Point(337, 4)
            Me.btnGenerate.Name = "btnGenerate"
            Me.btnGenerate.Size = New System.Drawing.Size(63, 21)
            Me.btnGenerate.TabIndex = 6
            Me.btnGenerate.Text = "Generate"
            Me.btnGenerate.UseVisualStyleBackColor = True
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(8, 7)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(86, 13)
            Me.Label2.TabIndex = 9
            Me.Label2.Text = "Filesystem type:"
            '
            'btnChooseFolder
            '
            Me.btnChooseFolder.Location = New System.Drawing.Point(109, 13)
            Me.btnChooseFolder.Name = "btnChooseFolder"
            Me.btnChooseFolder.Size = New System.Drawing.Size(28, 19)
            Me.btnChooseFolder.TabIndex = 12
            Me.btnChooseFolder.Text = "..."
            Me.btnChooseFolder.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(14, 16)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(87, 13)
            Me.Label1.TabIndex = 10
            Me.Label1.Text = "WebPages path:"
            '
            'lblWebPagesFolder
            '
            Me.lblWebPagesFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblWebPagesFolder.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblWebPagesFolder.ForeColor = System.Drawing.Color.DarkBlue
            Me.lblWebPagesFolder.Location = New System.Drawing.Point(140, 16)
            Me.lblWebPagesFolder.Name = "lblWebPagesFolder"
            Me.lblWebPagesFolder.Size = New System.Drawing.Size(387, 19)
            Me.lblWebPagesFolder.TabIndex = 11
            Me.lblWebPagesFolder.Text = "Path"
            '
            'WebBrowser2
            '
            Me.WebBrowser2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.WebBrowser2.Location = New System.Drawing.Point(0, 0)
            Me.WebBrowser2.MinimumSize = New System.Drawing.Size(20, 20)
            Me.WebBrowser2.Name = "WebBrowser2"
            Me.WebBrowser2.Size = New System.Drawing.Size(541, 351)
            Me.WebBrowser2.TabIndex = 0
            '
            'ToolStrip1
            '
            Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripButton, Me.OpenToolStripButton, Me.SaveToolStripButton, Me.toolStripSeparator, Me.CutToolStripButton, Me.CopyToolStripButton, Me.PasteToolStripButton, Me.toolStripSeparator1, Me.HelpToolStripButton, Me.ToolStripSeparator2})
            Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
            Me.ToolStrip1.Name = "ToolStrip1"
            Me.ToolStrip1.Size = New System.Drawing.Size(999, 25)
            Me.ToolStrip1.TabIndex = 2
            Me.ToolStrip1.Text = "ToolStrip1"
            '
            'NewToolStripButton
            '
            Me.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.NewToolStripButton.Image = CType(resources.GetObject("NewToolStripButton.Image"), System.Drawing.Image)
            Me.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.NewToolStripButton.Name = "NewToolStripButton"
            Me.NewToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.NewToolStripButton.Text = "&New"
            '
            'OpenToolStripButton
            '
            Me.OpenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.OpenToolStripButton.Image = CType(resources.GetObject("OpenToolStripButton.Image"), System.Drawing.Image)
            Me.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.OpenToolStripButton.Name = "OpenToolStripButton"
            Me.OpenToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.OpenToolStripButton.Text = "&Open"
            '
            'SaveToolStripButton
            '
            Me.SaveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.SaveToolStripButton.Image = CType(resources.GetObject("SaveToolStripButton.Image"), System.Drawing.Image)
            Me.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.SaveToolStripButton.Name = "SaveToolStripButton"
            Me.SaveToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.SaveToolStripButton.Text = "&Save"
            '
            'toolStripSeparator
            '
            Me.toolStripSeparator.Name = "toolStripSeparator"
            Me.toolStripSeparator.Size = New System.Drawing.Size(6, 25)
            '
            'CutToolStripButton
            '
            Me.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.CutToolStripButton.Image = CType(resources.GetObject("CutToolStripButton.Image"), System.Drawing.Image)
            Me.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.CutToolStripButton.Name = "CutToolStripButton"
            Me.CutToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.CutToolStripButton.Text = "C&ut"
            '
            'CopyToolStripButton
            '
            Me.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.CopyToolStripButton.Image = CType(resources.GetObject("CopyToolStripButton.Image"), System.Drawing.Image)
            Me.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.CopyToolStripButton.Name = "CopyToolStripButton"
            Me.CopyToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.CopyToolStripButton.Text = "&Copy"
            '
            'PasteToolStripButton
            '
            Me.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.PasteToolStripButton.Image = CType(resources.GetObject("PasteToolStripButton.Image"), System.Drawing.Image)
            Me.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.PasteToolStripButton.Name = "PasteToolStripButton"
            Me.PasteToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.PasteToolStripButton.Text = "&Paste"
            '
            'toolStripSeparator1
            '
            Me.toolStripSeparator1.Name = "toolStripSeparator1"
            Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 25)
            '
            'HelpToolStripButton
            '
            Me.HelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.HelpToolStripButton.Image = CType(resources.GetObject("HelpToolStripButton.Image"), System.Drawing.Image)
            Me.HelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.HelpToolStripButton.Name = "HelpToolStripButton"
            Me.HelpToolStripButton.Size = New System.Drawing.Size(23, 22)
            Me.HelpToolStripButton.Text = "He&lp"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'chkPreview
            '
            Me.chkPreview.AutoSize = True
            Me.chkPreview.Location = New System.Drawing.Point(194, 5)
            Me.chkPreview.Name = "chkPreview"
            Me.chkPreview.Size = New System.Drawing.Size(64, 17)
            Me.chkPreview.TabIndex = 7
            Me.chkPreview.Text = "Preview"
            Me.chkPreview.UseVisualStyleBackColor = True
            Me.chkPreview.Visible = False
            '
            'btnSettings
            '
            Me.btnSettings.Location = New System.Drawing.Point(278, 2)
            Me.btnSettings.Name = "btnSettings"
            Me.btnSettings.Size = New System.Drawing.Size(104, 23)
            Me.btnSettings.TabIndex = 13
            Me.btnSettings.Text = "Settings/Generate"
            Me.btnSettings.UseVisualStyleBackColor = True
            '
            'HTMLEditor
            '
            Me.ClientSize = New System.Drawing.Size(999, 383)
            Me.Controls.Add(Me.btnSettings)
            Me.Controls.Add(Me.chkPreview)
            Me.Controls.Add(Me.ToolStrip1)
            Me.Controls.Add(Me.SplitContainer1)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "HTMLEditor"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            Me.TabText = "HTML Editor"
            Me.Text = "HTML Editor"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.pnlSettings.ResumeLayout(False)
            Me.pnlSettings.PerformLayout()
            Me.pnlGenerate.ResumeLayout(False)
            Me.pnlGenerate.PerformLayout()
            Me.pnlUpload.ResumeLayout(False)
            Me.pnlUpload.PerformLayout()
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents WebBrowser2 As System.Windows.Forms.WebBrowser
        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents NewToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents OpenToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents SaveToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents toolStripSeparator As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents CutToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents CopyToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents PasteToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents HelpToolStripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents chkPreview As System.Windows.Forms.CheckBox
        Friend WithEvents cmbType As System.Windows.Forms.ComboBox
        Friend WithEvents btnGenerate As System.Windows.Forms.Button
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents lblWebPagesFolder As System.Windows.Forms.Label
        Friend WithEvents btnChooseFolder As System.Windows.Forms.Button
        Friend WithEvents pnlGenerate As System.Windows.Forms.Panel
        Friend WithEvents CustomTabControl1 As CustomTabControl
        Friend WithEvents btnUpload As System.Windows.Forms.Button
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents pnlSettings As System.Windows.Forms.Panel
        Friend WithEvents pnlUpload As System.Windows.Forms.Panel
        Friend WithEvents ListView1 As System.Windows.Forms.ListView
        Friend WithEvents btnSettings As System.Windows.Forms.Button
        Friend WithEvents columnFile As System.Windows.Forms.ColumnHeader
        Friend WithEvents chkAll As System.Windows.Forms.CheckBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents columnDateTime As System.Windows.Forms.ColumnHeader
        Friend WithEvents columnResult As System.Windows.Forms.ColumnHeader
        Friend WithEvents txtPassword As System.Windows.Forms.TextBox
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents txtUsername As System.Windows.Forms.TextBox
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents txtTargetUrl As System.Windows.Forms.TextBox
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents chkForce As System.Windows.Forms.CheckBox
    End Class
End Namespace
