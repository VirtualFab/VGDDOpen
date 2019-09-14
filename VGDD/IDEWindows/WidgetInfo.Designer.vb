Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class WidgetInfo
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

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WidgetInfo))
            Me.lblWidgetHelp = New System.Windows.Forms.Label()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.pnlFootPrints = New System.Windows.Forms.Panel()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.lblRAM = New System.Windows.Forms.Label()
            Me.lblROM = New System.Windows.Forms.Label()
            Me.lblHEAP = New System.Windows.Forms.Label()
            Me.lblInstances = New System.Windows.Forms.Label()
            Me.pictSchemeHelp = New System.Windows.Forms.PictureBox()
            Me.pnlFootPrints.SuspendLayout()
            CType(Me.pictSchemeHelp, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lblWidgetHelp
            '
            Me.lblWidgetHelp.AutoSize = True
            Me.lblWidgetHelp.Font = New System.Drawing.Font("Tahoma", 8.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle))
            Me.lblWidgetHelp.ForeColor = System.Drawing.Color.Blue
            Me.lblWidgetHelp.Location = New System.Drawing.Point(6, 5)
            Me.lblWidgetHelp.Name = "lblWidgetHelp"
            Me.lblWidgetHelp.Size = New System.Drawing.Size(154, 13)
            Me.lblWidgetHelp.TabIndex = 18
            Me.lblWidgetHelp.Text = "Online Help on this Widget"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(6, 24)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(58, 13)
            Me.Label4.TabIndex = 16
            Me.Label4.Text = "Instances:"
            '
            'pnlFootPrints
            '
            Me.pnlFootPrints.Controls.Add(Me.Label2)
            Me.pnlFootPrints.Controls.Add(Me.Label3)
            Me.pnlFootPrints.Controls.Add(Me.Label5)
            Me.pnlFootPrints.Controls.Add(Me.Label7)
            Me.pnlFootPrints.Controls.Add(Me.lblRAM)
            Me.pnlFootPrints.Controls.Add(Me.lblROM)
            Me.pnlFootPrints.Controls.Add(Me.lblHEAP)
            Me.pnlFootPrints.Location = New System.Drawing.Point(3, 40)
            Me.pnlFootPrints.Name = "pnlFootPrints"
            Me.pnlFootPrints.Size = New System.Drawing.Size(194, 77)
            Me.pnlFootPrints.TabIndex = 19
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(3, 19)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(34, 13)
            Me.Label2.TabIndex = 4
            Me.Label2.Text = "ROM:"
            '
            'Label3
            '
            Me.Label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label3.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label3.Location = New System.Drawing.Point(5, 2)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(186, 13)
            Me.Label3.TabIndex = 5
            Me.Label3.Text = "Footprints in bytes"
            Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(3, 38)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(33, 13)
            Me.Label5.TabIndex = 7
            Me.Label5.Text = "RAM:"
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Location = New System.Drawing.Point(3, 57)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(96, 13)
            Me.Label7.TabIndex = 9
            Me.Label7.Text = "Heap (x instance):"
            '
            'lblRAM
            '
            Me.lblRAM.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblRAM.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me.lblRAM.Location = New System.Drawing.Point(116, 38)
            Me.lblRAM.Name = "lblRAM"
            Me.lblRAM.RightToLeft = System.Windows.Forms.RightToLeft.No
            Me.lblRAM.Size = New System.Drawing.Size(70, 13)
            Me.lblRAM.TabIndex = 8
            Me.lblRAM.Text = "0"
            Me.lblRAM.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'lblROM
            '
            Me.lblROM.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblROM.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me.lblROM.Location = New System.Drawing.Point(116, 19)
            Me.lblROM.Name = "lblROM"
            Me.lblROM.RightToLeft = System.Windows.Forms.RightToLeft.No
            Me.lblROM.Size = New System.Drawing.Size(70, 13)
            Me.lblROM.TabIndex = 6
            Me.lblROM.Text = "0"
            Me.lblROM.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'lblHEAP
            '
            Me.lblHEAP.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblHEAP.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me.lblHEAP.Location = New System.Drawing.Point(116, 57)
            Me.lblHEAP.Name = "lblHEAP"
            Me.lblHEAP.RightToLeft = System.Windows.Forms.RightToLeft.No
            Me.lblHEAP.Size = New System.Drawing.Size(70, 13)
            Me.lblHEAP.TabIndex = 10
            Me.lblHEAP.Text = "0"
            Me.lblHEAP.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'lblInstances
            '
            Me.lblInstances.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me.lblInstances.Location = New System.Drawing.Point(119, 24)
            Me.lblInstances.Name = "lblInstances"
            Me.lblInstances.Size = New System.Drawing.Size(70, 13)
            Me.lblInstances.TabIndex = 17
            Me.lblInstances.Text = "0"
            Me.lblInstances.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'pictSchemeHelp
            '
            Me.pictSchemeHelp.Location = New System.Drawing.Point(215, 24)
            Me.pictSchemeHelp.Name = "pictSchemeHelp"
            Me.pictSchemeHelp.Size = New System.Drawing.Size(281, 117)
            Me.pictSchemeHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
            Me.pictSchemeHelp.TabIndex = 15
            Me.pictSchemeHelp.TabStop = False
            '
            'WidgetInfo
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoScroll = True
            Me.AutoScrollMinSize = New System.Drawing.Size(0, 400)
            Me.BackColor = System.Drawing.Color.White
            Me.ClientSize = New System.Drawing.Size(695, 177)
            Me.CloseButtonVisible = False
            Me.Controls.Add(Me.lblWidgetHelp)
            Me.Controls.Add(Me.Label4)
            Me.Controls.Add(Me.lblInstances)
            Me.Controls.Add(Me.pictSchemeHelp)
            Me.Controls.Add(Me.pnlFootPrints)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "WidgetInfo"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            Me.ShowInTaskbar = False
            Me.TabText = "Widget Info"
            Me.Text = "Widget Info"
            Me.pnlFootPrints.ResumeLayout(False)
            Me.pnlFootPrints.PerformLayout()
            CType(Me.pictSchemeHelp, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents lblWidgetHelp As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents pnlFootPrints As System.Windows.Forms.Panel
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents lblRAM As System.Windows.Forms.Label
        Friend WithEvents lblROM As System.Windows.Forms.Label
        Friend WithEvents lblHEAP As System.Windows.Forms.Label
        Friend WithEvents lblInstances As System.Windows.Forms.Label
        Friend WithEvents pictSchemeHelp As System.Windows.Forms.PictureBox

    End Class
End Namespace