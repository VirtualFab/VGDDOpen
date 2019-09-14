<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUIEditPalette
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
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnPaletteLoad = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.lblPaletteName = New System.Windows.Forms.Label()
        Me.btnOptimize = New System.Windows.Forms.Button()
        Me.btnPaletteExport = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnPaletteLoad
        '
        Me.btnPaletteLoad.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPaletteLoad.Location = New System.Drawing.Point(10, 36)
        Me.btnPaletteLoad.Margin = New System.Windows.Forms.Padding(0)
        Me.btnPaletteLoad.Name = "btnPaletteLoad"
        Me.btnPaletteLoad.Size = New System.Drawing.Size(152, 20)
        Me.btnPaletteLoad.TabIndex = 22
        Me.btnPaletteLoad.Text = "Load Palette from file"
        Me.btnPaletteLoad.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnPaletteLoad.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(10, 146)
        Me.btnOK.Margin = New System.Windows.Forms.Padding(0)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(152, 20)
        Me.btnOK.TabIndex = 23
        Me.btnOK.Text = "OK"
        Me.btnOK.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'lblPaletteName
        '
        Me.lblPaletteName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPaletteName.Location = New System.Drawing.Point(12, 9)
        Me.lblPaletteName.Name = "lblPaletteName"
        Me.lblPaletteName.Size = New System.Drawing.Size(150, 18)
        Me.lblPaletteName.TabIndex = 24
        Me.lblPaletteName.Text = "Palette Name"
        Me.lblPaletteName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnOptimize
        '
        Me.btnOptimize.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOptimize.Location = New System.Drawing.Point(10, 67)
        Me.btnOptimize.Margin = New System.Windows.Forms.Padding(0)
        Me.btnOptimize.Name = "btnOptimize"
        Me.btnOptimize.Size = New System.Drawing.Size(152, 20)
        Me.btnOptimize.TabIndex = 25
        Me.btnOptimize.Text = "Optimize Palette for RGB565"
        Me.btnOptimize.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnOptimize.UseVisualStyleBackColor = True
        '
        'btnPaletteExport
        '
        Me.btnPaletteExport.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPaletteExport.Location = New System.Drawing.Point(10, 98)
        Me.btnPaletteExport.Margin = New System.Windows.Forms.Padding(0)
        Me.btnPaletteExport.Name = "btnPaletteExport"
        Me.btnPaletteExport.Size = New System.Drawing.Size(152, 20)
        Me.btnPaletteExport.TabIndex = 26
        Me.btnPaletteExport.Text = "Export Palette to file"
        Me.btnPaletteExport.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnPaletteExport.UseVisualStyleBackColor = True
        '
        'frmUIEditPalette
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(179, 175)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnPaletteExport)
        Me.Controls.Add(Me.btnOptimize)
        Me.Controls.Add(Me.lblPaletteName)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnPaletteLoad)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmUIEditPalette"
        Me.ShowIcon = False
        Me.Text = "Edit Palette"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnPaletteLoad As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents lblPaletteName As System.Windows.Forms.Label
    Friend WithEvents btnOptimize As System.Windows.Forms.Button
    Friend WithEvents btnPaletteExport As System.Windows.Forms.Button
End Class
