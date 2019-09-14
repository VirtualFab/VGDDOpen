<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMhcPatcher
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMhcPatcher))
        Me.btnSelectHarmonyPath = New System.Windows.Forms.Button()
        Me.lblMalPath = New System.Windows.Forms.Label()
        Me.lblHarmonyVersion = New System.Windows.Forms.Label()
        Me.btnCheckPatches = New System.Windows.Forms.Button()
        Me.pnlHarmony = New System.Windows.Forms.Panel()
        Me.btnRestoreOriginals = New System.Windows.Forms.Button()
        Me.btnApplyPatches = New System.Windows.Forms.Button()
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.drpRepository = New System.Windows.Forms.ComboBox()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnOpenDownloadFolder = New System.Windows.Forms.Button()
        Me.pnlHarmony.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSelectHarmonyPath
        '
        Me.btnSelectHarmonyPath.BackColor = System.Drawing.Color.Honeydew
        Me.btnSelectHarmonyPath.Location = New System.Drawing.Point(9, 38)
        Me.btnSelectHarmonyPath.Name = "btnSelectHarmonyPath"
        Me.btnSelectHarmonyPath.Size = New System.Drawing.Size(186, 23)
        Me.btnSelectHarmonyPath.TabIndex = 34
        Me.btnSelectHarmonyPath.Text = "Harmony folder to be patched"
        Me.btnSelectHarmonyPath.UseVisualStyleBackColor = False
        '
        'lblMalPath
        '
        Me.lblMalPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMalPath.AutoSize = True
        Me.lblMalPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMalPath.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblMalPath.Location = New System.Drawing.Point(9, 22)
        Me.lblMalPath.Name = "lblMalPath"
        Me.lblMalPath.Size = New System.Drawing.Size(0, 13)
        Me.lblMalPath.TabIndex = 33
        '
        'lblHarmonyVersion
        '
        Me.lblHarmonyVersion.AutoSize = True
        Me.lblHarmonyVersion.Location = New System.Drawing.Point(12, 9)
        Me.lblHarmonyVersion.Name = "lblHarmonyVersion"
        Me.lblHarmonyVersion.Size = New System.Drawing.Size(121, 13)
        Me.lblHarmonyVersion.TabIndex = 35
        Me.lblHarmonyVersion.Text = "Selected Harmony path:"
        '
        'btnCheckPatches
        '
        Me.btnCheckPatches.BackColor = System.Drawing.Color.Honeydew
        Me.btnCheckPatches.Location = New System.Drawing.Point(3, 14)
        Me.btnCheckPatches.Name = "btnCheckPatches"
        Me.btnCheckPatches.Size = New System.Drawing.Size(186, 23)
        Me.btnCheckPatches.TabIndex = 36
        Me.btnCheckPatches.Text = "Check patches"
        Me.btnCheckPatches.UseVisualStyleBackColor = False
        '
        'pnlHarmony
        '
        Me.pnlHarmony.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlHarmony.Controls.Add(Me.btnRestoreOriginals)
        Me.pnlHarmony.Controls.Add(Me.btnApplyPatches)
        Me.pnlHarmony.Controls.Add(Me.TreeView1)
        Me.pnlHarmony.Controls.Add(Me.btnCheckPatches)
        Me.pnlHarmony.Location = New System.Drawing.Point(6, 67)
        Me.pnlHarmony.Name = "pnlHarmony"
        Me.pnlHarmony.Size = New System.Drawing.Size(839, 371)
        Me.pnlHarmony.TabIndex = 37
        Me.pnlHarmony.Visible = False
        '
        'btnRestoreOriginals
        '
        Me.btnRestoreOriginals.BackColor = System.Drawing.Color.Honeydew
        Me.btnRestoreOriginals.Location = New System.Drawing.Point(3, 100)
        Me.btnRestoreOriginals.Name = "btnRestoreOriginals"
        Me.btnRestoreOriginals.Size = New System.Drawing.Size(186, 23)
        Me.btnRestoreOriginals.TabIndex = 39
        Me.btnRestoreOriginals.Text = "Restore originals"
        Me.btnRestoreOriginals.UseVisualStyleBackColor = False
        '
        'btnApplyPatches
        '
        Me.btnApplyPatches.BackColor = System.Drawing.Color.Honeydew
        Me.btnApplyPatches.Location = New System.Drawing.Point(3, 57)
        Me.btnApplyPatches.Name = "btnApplyPatches"
        Me.btnApplyPatches.Size = New System.Drawing.Size(186, 23)
        Me.btnApplyPatches.TabIndex = 38
        Me.btnApplyPatches.Text = "Apply patches"
        Me.btnApplyPatches.UseVisualStyleBackColor = False
        '
        'TreeView1
        '
        Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeView1.Location = New System.Drawing.Point(195, 3)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(644, 368)
        Me.TreeView1.TabIndex = 37
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(212, 41)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(60, 13)
        Me.Label2.TabIndex = 41
        Me.Label2.Text = "Repository:"
        '
        'drpRepository
        '
        Me.drpRepository.FormattingEnabled = True
        Me.drpRepository.Location = New System.Drawing.Point(278, 38)
        Me.drpRepository.Name = "drpRepository"
        Me.drpRepository.Size = New System.Drawing.Size(239, 21)
        Me.drpRepository.TabIndex = 40
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "checked.png")
        Me.ImageList1.Images.SetKeyName(1, "unchecked.png")
        Me.ImageList1.Images.SetKeyName(2, "error.png")
        '
        'btnUpdate
        '
        Me.btnUpdate.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnUpdate.Image = CType(resources.GetObject("btnUpdate.Image"), System.Drawing.Image)
        Me.btnUpdate.Location = New System.Drawing.Point(525, 28)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(38, 37)
        Me.btnUpdate.TabIndex = 42
        Me.ToolTip1.SetToolTip(Me.btnUpdate, "Get latest patch definitions from the internet")
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnOpenDownloadFolder
        '
        Me.btnOpenDownloadFolder.Image = CType(resources.GetObject("btnOpenDownloadFolder.Image"), System.Drawing.Image)
        Me.btnOpenDownloadFolder.Location = New System.Drawing.Point(566, 28)
        Me.btnOpenDownloadFolder.Name = "btnOpenDownloadFolder"
        Me.btnOpenDownloadFolder.Size = New System.Drawing.Size(38, 37)
        Me.btnOpenDownloadFolder.TabIndex = 43
        Me.ToolTip1.SetToolTip(Me.btnOpenDownloadFolder, "Open Download Folder")
        Me.btnOpenDownloadFolder.UseVisualStyleBackColor = True
        '
        'frmMhcPatcher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(860, 454)
        Me.Controls.Add(Me.btnOpenDownloadFolder)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.drpRepository)
        Me.Controls.Add(Me.pnlHarmony)
        Me.Controls.Add(Me.lblHarmonyVersion)
        Me.Controls.Add(Me.btnSelectHarmonyPath)
        Me.Controls.Add(Me.lblMalPath)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMhcPatcher"
        Me.Text = "MHC Patcher by VirtualFab"
        Me.pnlHarmony.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSelectHarmonyPath As System.Windows.Forms.Button
    Friend WithEvents lblMalPath As System.Windows.Forms.Label
    Friend WithEvents lblHarmonyVersion As System.Windows.Forms.Label
    Friend WithEvents btnCheckPatches As System.Windows.Forms.Button
    Friend WithEvents pnlHarmony As System.Windows.Forms.Panel
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents btnApplyPatches As System.Windows.Forms.Button
    Friend WithEvents btnRestoreOriginals As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents drpRepository As System.Windows.Forms.ComboBox
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnOpenDownloadFolder As System.Windows.Forms.Button

End Class
