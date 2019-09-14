Namespace VGDDCommon
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Public Class frmBitmapChooser
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBitmapChooser))
            Me.ListView1 = New System.Windows.Forms.ListView()
            Me.btnNew = New System.Windows.Forms.Button()
            Me.btnRemove = New System.Windows.Forms.Button()
            Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
            Me.btnExtractPalette = New System.Windows.Forms.Button()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.txtBinPath = New System.Windows.Forms.TextBox()
            Me.btnChooseBinPath = New System.Windows.Forms.Button()
            Me.pnlBinFiles = New System.Windows.Forms.Panel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.btnSaveScaledImage = New System.Windows.Forms.Button()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.pnlBinFiles.SuspendLayout()
            Me.SuspendLayout()
            '
            'ListView1
            '
            Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.ListView1.Location = New System.Drawing.Point(0, 0)
            Me.ListView1.MultiSelect = False
            Me.ListView1.Name = "ListView1"
            Me.ListView1.Size = New System.Drawing.Size(620, 379)
            Me.ListView1.TabIndex = 0
            Me.ListView1.UseCompatibleStateImageBehavior = False
            '
            'btnNew
            '
            Me.btnNew.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
            Me.btnNew.Location = New System.Drawing.Point(12, 9)
            Me.btnNew.Name = "btnNew"
            Me.btnNew.Size = New System.Drawing.Size(55, 23)
            Me.btnNew.TabIndex = 1
            Me.btnNew.Text = "Add"
            Me.btnNew.UseVisualStyleBackColor = False
            '
            'btnRemove
            '
            Me.btnRemove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnRemove.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
            Me.btnRemove.Enabled = False
            Me.btnRemove.ForeColor = System.Drawing.SystemColors.ControlText
            Me.btnRemove.Location = New System.Drawing.Point(923, 9)
            Me.btnRemove.Name = "btnRemove"
            Me.btnRemove.Size = New System.Drawing.Size(76, 23)
            Me.btnRemove.TabIndex = 2
            Me.btnRemove.Text = "Remove"
            Me.btnRemove.UseVisualStyleBackColor = False
            '
            'PropertyGrid1
            '
            Me.PropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.PropertyGrid1.Location = New System.Drawing.Point(0, 0)
            Me.PropertyGrid1.Name = "PropertyGrid1"
            Me.PropertyGrid1.Size = New System.Drawing.Size(380, 379)
            Me.PropertyGrid1.TabIndex = 3
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.SplitContainer1.Location = New System.Drawing.Point(2, 48)
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.ListView1)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.btnSaveScaledImage)
            Me.SplitContainer1.Panel2.Controls.Add(Me.btnExtractPalette)
            Me.SplitContainer1.Panel2.Controls.Add(Me.btnRefresh)
            Me.SplitContainer1.Panel2.Controls.Add(Me.PropertyGrid1)
            Me.SplitContainer1.Size = New System.Drawing.Size(1004, 379)
            Me.SplitContainer1.SplitterDistance = 620
            Me.SplitContainer1.TabIndex = 4
            '
            'btnExtractPalette
            '
            Me.btnExtractPalette.Image = CType(resources.GetObject("btnExtractPalette.Image"), System.Drawing.Image)
            Me.btnExtractPalette.Location = New System.Drawing.Point(152, 1)
            Me.btnExtractPalette.Name = "btnExtractPalette"
            Me.btnExtractPalette.Size = New System.Drawing.Size(27, 25)
            Me.btnExtractPalette.TabIndex = 7
            Me.ToolTip1.SetToolTip(Me.btnExtractPalette, "Extract Palette from bitmap")
            Me.btnExtractPalette.UseVisualStyleBackColor = True
            Me.btnExtractPalette.Visible = False
            '
            'btnRefresh
            '
            Me.btnRefresh.Image = CType(resources.GetObject("btnRefresh.Image"), System.Drawing.Image)
            Me.btnRefresh.Location = New System.Drawing.Point(86, 1)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(27, 25)
            Me.btnRefresh.TabIndex = 6
            Me.ToolTip1.SetToolTip(Me.btnRefresh, "Refresh")
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(12, 12)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(139, 13)
            Me.Label1.TabIndex = 4
            Me.Label1.Text = "Save converted .bin files to:"
            '
            'txtBinPath
            '
            Me.txtBinPath.Location = New System.Drawing.Point(157, 9)
            Me.txtBinPath.Name = "txtBinPath"
            Me.txtBinPath.Size = New System.Drawing.Size(307, 20)
            Me.txtBinPath.TabIndex = 5
            '
            'btnChooseBinPath
            '
            Me.btnChooseBinPath.Location = New System.Drawing.Point(471, 7)
            Me.btnChooseBinPath.Name = "btnChooseBinPath"
            Me.btnChooseBinPath.Size = New System.Drawing.Size(26, 23)
            Me.btnChooseBinPath.TabIndex = 6
            Me.btnChooseBinPath.Text = "..."
            Me.btnChooseBinPath.UseVisualStyleBackColor = True
            '
            'pnlBinFiles
            '
            Me.pnlBinFiles.Controls.Add(Me.btnChooseBinPath)
            Me.pnlBinFiles.Controls.Add(Me.Label1)
            Me.pnlBinFiles.Controls.Add(Me.txtBinPath)
            Me.pnlBinFiles.Location = New System.Drawing.Point(166, 2)
            Me.pnlBinFiles.Name = "pnlBinFiles"
            Me.pnlBinFiles.Size = New System.Drawing.Size(516, 40)
            Me.pnlBinFiles.TabIndex = 1
            '
            'btnClose
            '
            Me.btnClose.BackColor = System.Drawing.SystemColors.Control
            Me.btnClose.Location = New System.Drawing.Point(92, 9)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(55, 23)
            Me.btnClose.TabIndex = 5
            Me.btnClose.Text = "Close"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'btnSaveScaledImage
            '
            Me.btnSaveScaledImage.Image = CType(resources.GetObject("btnSaveScaledImage.Image"), System.Drawing.Image)
            Me.btnSaveScaledImage.Location = New System.Drawing.Point(119, 1)
            Me.btnSaveScaledImage.Name = "btnSaveScaledImage"
            Me.btnSaveScaledImage.Size = New System.Drawing.Size(27, 25)
            Me.btnSaveScaledImage.TabIndex = 8
            Me.ToolTip1.SetToolTip(Me.btnSaveScaledImage, "Save scaled image")
            Me.btnSaveScaledImage.UseVisualStyleBackColor = True
            '
            'frmBitmapChooser
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            Me.ClientSize = New System.Drawing.Size(1011, 430)
            Me.Controls.Add(Me.btnClose)
            Me.Controls.Add(Me.pnlBinFiles)
            Me.Controls.Add(Me.SplitContainer1)
            Me.Controls.Add(Me.btnRemove)
            Me.Controls.Add(Me.btnNew)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "frmBitmapChooser"
            Me.Text = "Resource Bitmap Chooser"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.pnlBinFiles.ResumeLayout(False)
            Me.pnlBinFiles.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents ListView1 As System.Windows.Forms.ListView
        Friend WithEvents btnNew As System.Windows.Forms.Button
        Friend WithEvents btnRemove As System.Windows.Forms.Button
        Friend WithEvents PropertyGrid1 As System.Windows.Forms.PropertyGrid
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents txtBinPath As System.Windows.Forms.TextBox
        Friend WithEvents btnChooseBinPath As System.Windows.Forms.Button
        Friend WithEvents pnlBinFiles As System.Windows.Forms.Panel
        Friend WithEvents btnClose As System.Windows.Forms.Button
        Friend WithEvents btnRefresh As System.Windows.Forms.Button
        Friend WithEvents btnExtractPalette As System.Windows.Forms.Button
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents btnSaveScaledImage As System.Windows.Forms.Button
    End Class
End Namespace
