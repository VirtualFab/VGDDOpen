<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FootPrint
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
        Me.grpFootPrint = New System.Windows.Forms.GroupBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblROMFonts = New System.Windows.Forms.Label()
        Me.lblBitmaps = New System.Windows.Forms.Label()
        Me.lblROMBitmaps = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.lblRAM = New System.Windows.Forms.Label()
        Me.lblROMTotal = New System.Windows.Forms.Label()
        Me.lblHeap = New System.Windows.Forms.Label()
        Me.lblROMCode = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblROMData = New System.Windows.Forms.Label()
        Me.grpFootPrint.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpFootPrint
        '
        Me.grpFootPrint.Controls.Add(Me.Label7)
        Me.grpFootPrint.Controls.Add(Me.lblROMFonts)
        Me.grpFootPrint.Controls.Add(Me.lblBitmaps)
        Me.grpFootPrint.Controls.Add(Me.lblROMBitmaps)
        Me.grpFootPrint.Controls.Add(Me.Label15)
        Me.grpFootPrint.Controls.Add(Me.Label14)
        Me.grpFootPrint.Controls.Add(Me.lblRAM)
        Me.grpFootPrint.Controls.Add(Me.lblROMTotal)
        Me.grpFootPrint.Controls.Add(Me.lblHeap)
        Me.grpFootPrint.Controls.Add(Me.lblROMCode)
        Me.grpFootPrint.Controls.Add(Me.Label10)
        Me.grpFootPrint.Controls.Add(Me.Label9)
        Me.grpFootPrint.Controls.Add(Me.Label11)
        Me.grpFootPrint.Controls.Add(Me.Label6)
        Me.grpFootPrint.Controls.Add(Me.lblROMData)
        Me.grpFootPrint.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpFootPrint.Location = New System.Drawing.Point(0, 0)
        Me.grpFootPrint.Name = "grpFootPrint"
        Me.grpFootPrint.Size = New System.Drawing.Size(212, 127)
        Me.grpFootPrint.TabIndex = 23
        Me.grpFootPrint.TabStop = False
        Me.grpFootPrint.Text = "Footprint in bytes"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.Label7.Location = New System.Drawing.Point(38, 57)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(38, 13)
        Me.Label7.TabIndex = 29
        Me.Label7.Text = "Fonts:"
        '
        'lblROMFonts
        '
        Me.lblROMFonts.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblROMFonts.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.lblROMFonts.Location = New System.Drawing.Point(137, 57)
        Me.lblROMFonts.Name = "lblROMFonts"
        Me.lblROMFonts.Size = New System.Drawing.Size(69, 13)
        Me.lblROMFonts.TabIndex = 30
        Me.lblROMFonts.Text = "0"
        Me.lblROMFonts.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblBitmaps
        '
        Me.lblBitmaps.AutoSize = True
        Me.lblBitmaps.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.lblBitmaps.Location = New System.Drawing.Point(38, 44)
        Me.lblBitmaps.Name = "lblBitmaps"
        Me.lblBitmaps.Size = New System.Drawing.Size(48, 13)
        Me.lblBitmaps.TabIndex = 27
        Me.lblBitmaps.Text = "Bitmaps:"
        '
        'lblROMBitmaps
        '
        Me.lblROMBitmaps.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblROMBitmaps.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.lblROMBitmaps.Location = New System.Drawing.Point(137, 44)
        Me.lblROMBitmaps.Name = "lblROMBitmaps"
        Me.lblROMBitmaps.Size = New System.Drawing.Size(69, 13)
        Me.lblROMBitmaps.TabIndex = 28
        Me.lblROMBitmaps.Text = "0"
        Me.lblROMBitmaps.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.Label15.Location = New System.Drawing.Point(38, 70)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(84, 13)
        Me.Label15.TabIndex = 25
        Me.Label15.Text = "TOTAL FLASH:"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.Label14.Location = New System.Drawing.Point(38, 18)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(36, 13)
        Me.Label14.TabIndex = 24
        Me.Label14.Text = "Code:"
        '
        'lblRAM
        '
        Me.lblRAM.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblRAM.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.lblRAM.Location = New System.Drawing.Point(137, 92)
        Me.lblRAM.Name = "lblRAM"
        Me.lblRAM.Size = New System.Drawing.Size(69, 13)
        Me.lblRAM.TabIndex = 19
        Me.lblRAM.Text = "0"
        Me.lblRAM.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblROMTotal
        '
        Me.lblROMTotal.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblROMTotal.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblROMTotal.Location = New System.Drawing.Point(137, 70)
        Me.lblROMTotal.Name = "lblROMTotal"
        Me.lblROMTotal.Size = New System.Drawing.Size(69, 13)
        Me.lblROMTotal.TabIndex = 26
        Me.lblROMTotal.Text = "0"
        Me.lblROMTotal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblHeap
        '
        Me.lblHeap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblHeap.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.lblHeap.Location = New System.Drawing.Point(137, 105)
        Me.lblHeap.Name = "lblHeap"
        Me.lblHeap.Size = New System.Drawing.Size(69, 13)
        Me.lblHeap.TabIndex = 21
        Me.lblHeap.Text = "0"
        Me.lblHeap.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblROMCode
        '
        Me.lblROMCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblROMCode.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.lblROMCode.Location = New System.Drawing.Point(137, 18)
        Me.lblROMCode.Name = "lblROMCode"
        Me.lblROMCode.Size = New System.Drawing.Size(69, 13)
        Me.lblROMCode.TabIndex = 17
        Me.lblROMCode.Text = "0"
        Me.lblROMCode.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.Label10.Location = New System.Drawing.Point(10, 105)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(36, 13)
        Me.Label10.TabIndex = 20
        Me.Label10.Text = "Heap:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.Label9.Location = New System.Drawing.Point(10, 92)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(33, 13)
        Me.Label9.TabIndex = 18
        Me.Label9.Text = "RAM:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.Label11.Location = New System.Drawing.Point(38, 31)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(44, 13)
        Me.Label11.TabIndex = 22
        Me.Label11.Text = "Strings:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.Label6.Location = New System.Drawing.Point(10, 18)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(30, 13)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "ROM"
        '
        'lblROMData
        '
        Me.lblROMData.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblROMData.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.lblROMData.Location = New System.Drawing.Point(137, 31)
        Me.lblROMData.Name = "lblROMData"
        Me.lblROMData.Size = New System.Drawing.Size(69, 13)
        Me.lblROMData.TabIndex = 23
        Me.lblROMData.Text = "0"
        Me.lblROMData.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'FootPrint
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.grpFootPrint)
        Me.Name = "FootPrint"
        Me.Size = New System.Drawing.Size(212, 127)
        Me.grpFootPrint.ResumeLayout(False)
        Me.grpFootPrint.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents grpFootPrint As System.Windows.Forms.GroupBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblROMFonts As System.Windows.Forms.Label
    Friend WithEvents lblBitmaps As System.Windows.Forms.Label
    Friend WithEvents lblROMBitmaps As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents lblRAM As System.Windows.Forms.Label
    Friend WithEvents lblROMTotal As System.Windows.Forms.Label
    Friend WithEvents lblHeap As System.Windows.Forms.Label
    Friend WithEvents lblROMCode As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblROMData As System.Windows.Forms.Label

End Class
