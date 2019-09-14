<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmGenCode
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGenCode))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblLnkRegister = New System.Windows.Forms.Label()
        Me.pnlUnregistered = New System.Windows.Forms.Panel()
        Me.pnlGenerateOptions = New System.Windows.Forms.Panel()
        Me.btnGetItems = New System.Windows.Forms.Button()
        Me.chkQuickCodeGen = New System.Windows.Forms.CheckBox()
        Me.btnGenerateCode = New System.Windows.Forms.Button()
        Me.lblBinBitmaps = New System.Windows.Forms.Label()
        Me.btnBinBitmaps = New System.Windows.Forms.Button()
        Me.CodeGenLocation1 = New CodeGenLocation()
        Me.pnlProgressBar = New System.Windows.Forms.Panel()
        Me.FootPrint1 = New FootPrint()
        Me.btnShowSource = New System.Windows.Forms.Button()
        Me.pnlProgress = New System.Windows.Forms.Panel()
        Me.ProgressBar1 = New ProgressBarText()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SourceEditor = New SourceEditor.Editor()
        Me.pnlUnregistered.SuspendLayout()
        Me.pnlGenerateOptions.SuspendLayout()
        CType(Me.CodeGenLocation1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlProgressBar.SuspendLayout()
        Me.pnlProgress.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(850, 47)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = resources.GetString("Label1.Text")
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblLnkRegister
        '
        Me.lblLnkRegister.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLnkRegister.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLnkRegister.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblLnkRegister.Location = New System.Drawing.Point(0, 52)
        Me.lblLnkRegister.Name = "lblLnkRegister"
        Me.lblLnkRegister.Size = New System.Drawing.Size(850, 22)
        Me.lblLnkRegister.TabIndex = 2
        Me.lblLnkRegister.Text = "Register VGDD"
        Me.lblLnkRegister.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pnlUnregistered
        '
        Me.pnlUnregistered.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlUnregistered.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.pnlUnregistered.Controls.Add(Me.lblLnkRegister)
        Me.pnlUnregistered.Controls.Add(Me.Label1)
        Me.pnlUnregistered.Location = New System.Drawing.Point(0, 0)
        Me.pnlUnregistered.Name = "pnlUnregistered"
        Me.pnlUnregistered.Size = New System.Drawing.Size(855, 69)
        Me.pnlUnregistered.TabIndex = 3
        '
        'pnlGenerateOptions
        '
        Me.pnlGenerateOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlGenerateOptions.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlGenerateOptions.Controls.Add(Me.btnGetItems)
        Me.pnlGenerateOptions.Controls.Add(Me.chkQuickCodeGen)
        Me.pnlGenerateOptions.Controls.Add(Me.btnGenerateCode)
        Me.pnlGenerateOptions.Controls.Add(Me.lblBinBitmaps)
        Me.pnlGenerateOptions.Controls.Add(Me.btnBinBitmaps)
        Me.pnlGenerateOptions.Controls.Add(Me.CodeGenLocation1)
        Me.pnlGenerateOptions.Location = New System.Drawing.Point(0, 0)
        Me.pnlGenerateOptions.MinimumSize = New System.Drawing.Size(0, 160)
        Me.pnlGenerateOptions.Name = "pnlGenerateOptions"
        Me.pnlGenerateOptions.Size = New System.Drawing.Size(855, 310)
        Me.pnlGenerateOptions.TabIndex = 4
        '
        'btnGetItems
        '
        Me.btnGetItems.Location = New System.Drawing.Point(12, 215)
        Me.btnGetItems.Name = "btnGetItems"
        Me.btnGetItems.Size = New System.Drawing.Size(25, 23)
        Me.btnGetItems.TabIndex = 23
        Me.btnGetItems.Text = "G"
        Me.btnGetItems.UseVisualStyleBackColor = True
        Me.btnGetItems.Visible = False
        '
        'chkQuickCodeGen
        '
        Me.chkQuickCodeGen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkQuickCodeGen.AutoSize = True
        Me.chkQuickCodeGen.Location = New System.Drawing.Point(552, 264)
        Me.chkQuickCodeGen.Name = "chkQuickCodeGen"
        Me.chkQuickCodeGen.Size = New System.Drawing.Size(189, 17)
        Me.chkQuickCodeGen.TabIndex = 21
        Me.chkQuickCodeGen.Text = "Don't convert Graphics Resources"
        Me.chkQuickCodeGen.UseVisualStyleBackColor = True
        '
        'btnGenerateCode
        '
        Me.btnGenerateCode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnGenerateCode.BackColor = System.Drawing.Color.Honeydew
        Me.btnGenerateCode.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnGenerateCode.Location = New System.Drawing.Point(271, 255)
        Me.btnGenerateCode.Name = "btnGenerateCode"
        Me.btnGenerateCode.Size = New System.Drawing.Size(275, 32)
        Me.btnGenerateCode.TabIndex = 2
        Me.btnGenerateCode.Text = "Generate Code"
        Me.btnGenerateCode.UseVisualStyleBackColor = False
        '
        'lblBinBitmaps
        '
        Me.lblBinBitmaps.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblBinBitmaps.AutoSize = True
        Me.lblBinBitmaps.Location = New System.Drawing.Point(19, 288)
        Me.lblBinBitmaps.Name = "lblBinBitmaps"
        Me.lblBinBitmaps.Size = New System.Drawing.Size(122, 13)
        Me.lblBinBitmaps.TabIndex = 20
        Me.lblBinBitmaps.Text = "N Bitmaps in BIN format:"
        Me.lblBinBitmaps.Visible = False
        '
        'btnBinBitmaps
        '
        Me.btnBinBitmaps.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBinBitmaps.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnBinBitmaps.Location = New System.Drawing.Point(147, 283)
        Me.btnBinBitmaps.Name = "btnBinBitmaps"
        Me.btnBinBitmaps.Size = New System.Drawing.Size(95, 23)
        Me.btnBinBitmaps.TabIndex = 19
        Me.btnBinBitmaps.Text = "Open BIN folder"
        Me.btnBinBitmaps.UseVisualStyleBackColor = False
        Me.btnBinBitmaps.Visible = False
        '
        'CodeGenLocation1
        '
        Me.CodeGenLocation1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CodeGenLocation1.Location = New System.Drawing.Point(0, 0)
        Me.CodeGenLocation1.MinimumSize = New System.Drawing.Size(856, 246)
        Me.CodeGenLocation1.Name = "CodeGenLocation1"
        Me.CodeGenLocation1.oMplabxIpc = Nothing
        Me.CodeGenLocation1.Size = New System.Drawing.Size(856, 246)
        Me.CodeGenLocation1.TabIndex = 22
        '
        'pnlProgressBar
        '
        Me.pnlProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlProgressBar.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlProgressBar.Controls.Add(Me.FootPrint1)
        Me.pnlProgressBar.Controls.Add(Me.btnShowSource)
        Me.pnlProgressBar.Controls.Add(Me.pnlProgress)
        Me.pnlProgressBar.Controls.Add(Me.btnClose)
        Me.pnlProgressBar.Location = New System.Drawing.Point(0, 310)
        Me.pnlProgressBar.MinimumSize = New System.Drawing.Size(0, 128)
        Me.pnlProgressBar.Name = "pnlProgressBar"
        Me.pnlProgressBar.Size = New System.Drawing.Size(856, 131)
        Me.pnlProgressBar.TabIndex = 22
        Me.pnlProgressBar.Visible = False
        '
        'FootPrint1
        '
        Me.FootPrint1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FootPrint1.Caption = "Footprint in bytes"
        Me.FootPrint1.Location = New System.Drawing.Point(609, 1)
        Me.FootPrint1.Name = "FootPrint1"
        Me.FootPrint1.Size = New System.Drawing.Size(241, 127)
        Me.FootPrint1.TabIndex = 21
        '
        'btnShowSource
        '
        Me.btnShowSource.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnShowSource.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnShowSource.Location = New System.Drawing.Point(496, 81)
        Me.btnShowSource.Name = "btnShowSource"
        Me.btnShowSource.Size = New System.Drawing.Size(95, 23)
        Me.btnShowSource.TabIndex = 14
        Me.btnShowSource.Text = "Show Source"
        Me.btnShowSource.UseVisualStyleBackColor = False
        '
        'pnlProgress
        '
        Me.pnlProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlProgress.Controls.Add(Me.ProgressBar1)
        Me.pnlProgress.Controls.Add(Me.lblStatus)
        Me.pnlProgress.Location = New System.Drawing.Point(12, 8)
        Me.pnlProgress.Name = "pnlProgress"
        Me.pnlProgress.Size = New System.Drawing.Size(586, 41)
        Me.pnlProgress.TabIndex = 13
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ProgressBar1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.ProgressBar1.Location = New System.Drawing.Point(105, 8)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(474, 23)
        Me.ProgressBar1.TabIndex = 0
        Me.ProgressBar1.TextFormat = "{0}% Done"
        Me.ProgressBar1.UseVisualStyles = False
        Me.ProgressBar1.Value = 30
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(4, 12)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(95, 13)
        Me.lblStatus.TabIndex = 1
        Me.lblStatus.Text = "Generating code..."
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnClose.BackColor = System.Drawing.Color.Honeydew
        Me.btnClose.Location = New System.Drawing.Point(12, 81)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(94, 23)
        Me.btnClose.TabIndex = 18
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'Timer1
        '
        '
        'SourceEditor
        '
        Me.SourceEditor.AllowDrop = True
        Me.SourceEditor.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SourceEditor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SourceEditor.Font = New System.Drawing.Font("Courier New", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SourceEditor.Location = New System.Drawing.Point(0, 0)
        Me.SourceEditor.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.SourceEditor.Name = "SourceEditor"
        Me.SourceEditor.Size = New System.Drawing.Size(857, 441)
        Me.SourceEditor.TabIndex = 0
        Me.SourceEditor.Visible = False
        '
        'frmGenCode
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(857, 441)
        Me.Controls.Add(Me.pnlProgressBar)
        Me.Controls.Add(Me.pnlGenerateOptions)
        Me.Controls.Add(Me.SourceEditor)
        Me.Controls.Add(Me.pnlUnregistered)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(780, 160)
        Me.Name = "frmGenCode"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Code Generation"
        Me.pnlUnregistered.ResumeLayout(False)
        Me.pnlGenerateOptions.ResumeLayout(False)
        Me.pnlGenerateOptions.PerformLayout()
        CType(Me.CodeGenLocation1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlProgressBar.ResumeLayout(False)
        Me.pnlProgress.ResumeLayout(False)
        Me.pnlProgress.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SourceEditor As SourceEditor.Editor
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblLnkRegister As System.Windows.Forms.Label
    Friend WithEvents pnlUnregistered As System.Windows.Forms.Panel
    Friend WithEvents pnlGenerateOptions As System.Windows.Forms.Panel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents btnGenerateCode As System.Windows.Forms.Button
    Friend WithEvents pnlProgress As System.Windows.Forms.Panel
    Friend WithEvents btnShowSource As System.Windows.Forms.Button
    Friend WithEvents ProgressBar1 As ProgressBarText
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnBinBitmaps As System.Windows.Forms.Button
    Friend WithEvents lblBinBitmaps As System.Windows.Forms.Label
    Friend WithEvents FootPrint1 As FootPrint
    Friend WithEvents pnlProgressBar As System.Windows.Forms.Panel
    Friend WithEvents chkQuickCodeGen As System.Windows.Forms.CheckBox
    Friend WithEvents CodeGenLocation1 As CodeGenLocation
    Friend WithEvents btnGetItems As System.Windows.Forms.Button
End Class
