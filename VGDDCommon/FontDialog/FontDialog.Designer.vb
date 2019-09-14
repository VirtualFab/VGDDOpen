Partial Class FontDialog
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.label1 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.lstSize = New System.Windows.Forms.ListBox()
        Me.txtSize = New System.Windows.Forms.TextBox()
        Me.chbBold = New System.Windows.Forms.CheckBox()
        Me.chbItalic = New System.Windows.Forms.CheckBox()
        Me.groupBox1 = New System.Windows.Forms.GroupBox()
        Me.chbStrikeout = New System.Windows.Forms.CheckBox()
        Me.groupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblSampleText = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.lstFont = New FontList()
        Me.groupBox1.SuspendLayout()
        Me.groupBox2.SuspendLayout()
        Me.SuspendLayout()
        ' 
        ' label1
        ' 
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(13, 13)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(31, 13)
        Me.label1.TabIndex = 0
        Me.label1.Text = "Font:"
        ' 
        ' label2
        ' 
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(187, 13)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(30, 13)
        Me.label2.TabIndex = 3
        Me.label2.Text = "Size:"
        ' 
        ' lstSize
        ' 
        Me.lstSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstSize.FormattingEnabled = True
        Me.lstSize.Items.AddRange(New Object() {"8", "9", "10", "11", "12", "14", _
         "16", "18", "20", "22", "24", "26", _
         "28", "36", "48", "72"})
        Me.lstSize.Location = New System.Drawing.Point(190, 50)
        Me.lstSize.Name = "lstSize"
        Me.lstSize.Size = New System.Drawing.Size(100, 119)
        Me.lstSize.TabIndex = 4
        ' 
        ' txtSize
        ' 
        Me.txtSize.Location = New System.Drawing.Point(190, 30)
        Me.txtSize.Name = "txtSize"
        Me.txtSize.Size = New System.Drawing.Size(100, 20)
        Me.txtSize.TabIndex = 5
        ' 
        ' chbBold
        ' 
        Me.chbBold.AutoSize = True
        Me.chbBold.Location = New System.Drawing.Point(21, 27)
        Me.chbBold.Name = "chbBold"
        Me.chbBold.Size = New System.Drawing.Size(47, 17)
        Me.chbBold.TabIndex = 6
        Me.chbBold.Text = "Bold"
        Me.chbBold.UseVisualStyleBackColor = True
        ' 
        ' chbItalic
        ' 
        Me.chbItalic.AutoSize = True
        Me.chbItalic.Location = New System.Drawing.Point(21, 50)
        Me.chbItalic.Name = "chbItalic"
        Me.chbItalic.Size = New System.Drawing.Size(48, 17)
        Me.chbItalic.TabIndex = 7
        Me.chbItalic.Text = "Italic"
        Me.chbItalic.UseVisualStyleBackColor = True
        ' 
        ' groupBox1
        ' 
        Me.groupBox1.Controls.Add(Me.chbStrikeout)
        Me.groupBox1.Controls.Add(Me.chbBold)
        Me.groupBox1.Controls.Add(Me.chbItalic)
        Me.groupBox1.Location = New System.Drawing.Point(312, 13)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(113, 158)
        Me.groupBox1.TabIndex = 8
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Font Style"
        ' 
        ' chbStrikeout
        ' 
        Me.chbStrikeout.AutoSize = True
        Me.chbStrikeout.Location = New System.Drawing.Point(21, 73)
        Me.chbStrikeout.Name = "chbStrikeout"
        Me.chbStrikeout.Size = New System.Drawing.Size(68, 17)
        Me.chbStrikeout.TabIndex = 8
        Me.chbStrikeout.Text = "Strikeout"
        Me.chbStrikeout.UseVisualStyleBackColor = True
        ' 
        ' groupBox2
        ' 
        Me.groupBox2.Controls.Add(Me.lblSampleText)
        Me.groupBox2.Location = New System.Drawing.Point(190, 177)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(235, 140)
        Me.groupBox2.TabIndex = 9
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Sample Text"
        ' 
        ' lblSampleText
        ' 
        Me.lblSampleText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSampleText.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
        Me.lblSampleText.Location = New System.Drawing.Point(3, 16)
        Me.lblSampleText.Name = "lblSampleText"
        Me.lblSampleText.Size = New System.Drawing.Size(229, 121)
        Me.lblSampleText.TabIndex = 0
        Me.lblSampleText.Text = "AaBbCcXxYyZz"
        Me.lblSampleText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        ' 
        ' btnCancel
        ' 
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(350, 327)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 10
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        ' 
        ' btnOK
        ' 
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(269, 327)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 11
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        ' 
        ' lstFont
        ' 
        Me.lstFont.Location = New System.Drawing.Point(16, 30)
        Me.lstFont.Name = "lstFont"
        Me.lstFont.SelectedFontFamily = Nothing
        Me.lstFont.Size = New System.Drawing.Size(150, 289)
        Me.lstFont.TabIndex = 1
        ' 
        ' FontDialog
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(450, 362)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.groupBox2)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.txtSize)
        Me.Controls.Add(Me.lstSize)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.lstFont)
        Me.Controls.Add(Me.label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "FontDialog"
        Me.Text = "Font"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.groupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private label1 As System.Windows.Forms.Label
    Private WithEvents lstFont As FontList
    Private label2 As System.Windows.Forms.Label
    Private WithEvents lstSize As System.Windows.Forms.ListBox
    Private WithEvents txtSize As System.Windows.Forms.TextBox
    Private WithEvents chbBold As System.Windows.Forms.CheckBox
    Private WithEvents chbItalic As System.Windows.Forms.CheckBox
    Private groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents chbStrikeout As System.Windows.Forms.CheckBox
    Private groupBox2 As System.Windows.Forms.GroupBox
    Private lblSampleText As System.Windows.Forms.Label
    Private btnCancel As System.Windows.Forms.Button
    Private WithEvents btnOK As System.Windows.Forms.Button
End Class

