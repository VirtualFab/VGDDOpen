<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        'Dim GaugeSegment1 As AquaGaugeVb.GaugeSegment = New AquaGaugeVb.GaugeSegment()
        'Me.AquaGaugeVb1 = New AquaGaugeVb()
        Dim GaugeSegment1 As AquaGaugeVb.GaugeSegment = New AquaGaugeVb.GaugeSegment()
        Me.AquaGaugeVb1 = New AquaGaugeVb()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'AquaGaugeVb1
        '
        Me.AquaGaugeVb1.AngleFrom = 135
        Me.AquaGaugeVb1.AngleTo = 405
        Me.AquaGaugeVb1.BackColor = System.Drawing.Color.Transparent
        Me.AquaGaugeVb1.DialColor = System.Drawing.Color.MediumBlue
        Me.AquaGaugeVb1.DialText = "VirtualFab"
        Me.AquaGaugeVb1.DialTextOffsetX = 0
        Me.AquaGaugeVb1.DialTextOffsetY = 18
        Me.AquaGaugeVb1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AquaGaugeVb1.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AquaGaugeVb1.Location = New System.Drawing.Point(0, 0)
        Me.AquaGaugeVb1.MinValue = 50
        Me.AquaGaugeVb1.Name = "AquaGaugeVb1"
        Me.AquaGaugeVb1.NoOfSubDivisions = 4
        Me.AquaGaugeVb1.NumberOfDigits = 3
        Me.AquaGaugeVb1.OffsetX = 0
        Me.AquaGaugeVb1.OffsetY = 30
        GaugeSegment1.SegmentColour = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        GaugeSegment1.ValueFrom = 50
        GaugeSegment1.ValueTo = 70
        Me.AquaGaugeVb1.Segments.Add(GaugeSegment1)
        Me.AquaGaugeVb1.SingleDigitSizeX = 7
        Me.AquaGaugeVb1.SingleDigitSizeY = 7
        Me.AquaGaugeVb1.Size = New System.Drawing.Size(542, 542)
        Me.AquaGaugeVb1.TabIndex = 0
        Me.AquaGaugeVb1.Text = "AquaGaugeVb1"
        Me.AquaGaugeVb1.Value = 50
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(0, 0)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(81, 17)
        Me.CheckBox1.TabIndex = 1
        Me.CheckBox1.Text = "CheckBox1"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(542, 531)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.AquaGaugeVb1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents AquaGaugeVb1 As AquaGaugeVb
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
End Class
