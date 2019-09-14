<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Framework
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
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnGRC = New System.Windows.Forms.Button()
        Me.btnSelectMalPath = New System.Windows.Forms.Button()
        Me.lblMalPath = New System.Windows.Forms.Label()
        Me.pnlFramework = New System.Windows.Forms.Panel()
        Me.pnlGRC = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnJavaTest = New System.Windows.Forms.Button()
        Me.btnGRCOk = New System.Windows.Forms.Button()
        Me.btnSelectPathGRC = New System.Windows.Forms.Button()
        Me.txtPathGRC = New System.Windows.Forms.TextBox()
        Me.lblLoadingTemplates = New System.Windows.Forms.Label()
        Me.GroupBox2.SuspendLayout()
        Me.pnlFramework.SuspendLayout()
        Me.pnlGRC.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lblLoadingTemplates)
        Me.GroupBox2.Controls.Add(Me.btnGRC)
        Me.GroupBox2.Controls.Add(Me.btnSelectMalPath)
        Me.GroupBox2.Controls.Add(Me.lblMalPath)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(494, 68)
        Me.GroupBox2.TabIndex = 42
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Framework"
        '
        'btnGRC
        '
        Me.btnGRC.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnGRC.BackColor = System.Drawing.Color.Honeydew
        Me.btnGRC.Location = New System.Drawing.Point(409, 32)
        Me.btnGRC.Name = "btnGRC"
        Me.btnGRC.Size = New System.Drawing.Size(74, 23)
        Me.btnGRC.TabIndex = 33
        Me.btnGRC.Text = "GRC Path"
        Me.btnGRC.UseVisualStyleBackColor = False
        '
        'btnSelectMalPath
        '
        Me.btnSelectMalPath.BackColor = System.Drawing.Color.Honeydew
        Me.btnSelectMalPath.Location = New System.Drawing.Point(8, 32)
        Me.btnSelectMalPath.Name = "btnSelectMalPath"
        Me.btnSelectMalPath.Size = New System.Drawing.Size(145, 23)
        Me.btnSelectMalPath.TabIndex = 32
        Me.btnSelectMalPath.Text = "Select Framework to use"
        Me.btnSelectMalPath.UseVisualStyleBackColor = False
        '
        'lblMalPath
        '
        Me.lblMalPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMalPath.AutoSize = True
        Me.lblMalPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMalPath.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblMalPath.Location = New System.Drawing.Point(5, 16)
        Me.lblMalPath.Name = "lblMalPath"
        Me.lblMalPath.Size = New System.Drawing.Size(479, 13)
        Me.lblMalPath.TabIndex = 31
        Me.lblMalPath.Text = "<MAL Path><MAL Path><MAL Path><MAL Path><MAL Path><MAL Path><MAL Path><MAL Path>"
        '
        'pnlFramework
        '
        Me.pnlFramework.Controls.Add(Me.GroupBox2)
        Me.pnlFramework.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlFramework.Location = New System.Drawing.Point(0, 0)
        Me.pnlFramework.Name = "pnlFramework"
        Me.pnlFramework.Size = New System.Drawing.Size(494, 68)
        Me.pnlFramework.TabIndex = 33
        '
        'pnlGRC
        '
        Me.pnlGRC.Controls.Add(Me.GroupBox1)
        Me.pnlGRC.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlGRC.Location = New System.Drawing.Point(0, 0)
        Me.pnlGRC.Name = "pnlGRC"
        Me.pnlGRC.Size = New System.Drawing.Size(494, 68)
        Me.pnlGRC.TabIndex = 43
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnJavaTest)
        Me.GroupBox1.Controls.Add(Me.btnGRCOk)
        Me.GroupBox1.Controls.Add(Me.btnSelectPathGRC)
        Me.GroupBox1.Controls.Add(Me.txtPathGRC)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(494, 68)
        Me.GroupBox1.TabIndex = 42
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Microchip's Graphics Resource Converter (GRC) Path"
        '
        'btnJavaTest
        '
        Me.btnJavaTest.Location = New System.Drawing.Point(8, 39)
        Me.btnJavaTest.Name = "btnJavaTest"
        Me.btnJavaTest.Size = New System.Drawing.Size(75, 23)
        Me.btnJavaTest.TabIndex = 34
        Me.btnJavaTest.Text = "Test"
        Me.btnJavaTest.UseVisualStyleBackColor = True
        '
        'btnGRCOk
        '
        Me.btnGRCOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnGRCOk.BackColor = System.Drawing.Color.DarkGreen
        Me.btnGRCOk.ForeColor = System.Drawing.Color.Yellow
        Me.btnGRCOk.Location = New System.Drawing.Point(408, 39)
        Me.btnGRCOk.Name = "btnGRCOk"
        Me.btnGRCOk.Size = New System.Drawing.Size(75, 23)
        Me.btnGRCOk.TabIndex = 32
        Me.btnGRCOk.Text = "OK"
        Me.btnGRCOk.UseVisualStyleBackColor = False
        '
        'btnSelectPathGRC
        '
        Me.btnSelectPathGRC.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectPathGRC.Location = New System.Drawing.Point(453, 14)
        Me.btnSelectPathGRC.Name = "btnSelectPathGRC"
        Me.btnSelectPathGRC.Size = New System.Drawing.Size(30, 23)
        Me.btnSelectPathGRC.TabIndex = 31
        Me.btnSelectPathGRC.Text = "..."
        Me.btnSelectPathGRC.UseVisualStyleBackColor = True
        '
        'txtPathGRC
        '
        Me.txtPathGRC.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPathGRC.Location = New System.Drawing.Point(8, 16)
        Me.txtPathGRC.Name = "txtPathGRC"
        Me.txtPathGRC.Size = New System.Drawing.Size(439, 20)
        Me.txtPathGRC.TabIndex = 30
        '
        'lblLoadingTemplates
        '
        Me.lblLoadingTemplates.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLoadingTemplates.AutoSize = True
        Me.lblLoadingTemplates.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoadingTemplates.ForeColor = System.Drawing.Color.Maroon
        Me.lblLoadingTemplates.Location = New System.Drawing.Point(197, 35)
        Me.lblLoadingTemplates.Name = "lblLoadingTemplates"
        Me.lblLoadingTemplates.Size = New System.Drawing.Size(154, 16)
        Me.lblLoadingTemplates.TabIndex = 34
        Me.lblLoadingTemplates.Text = "Loading Templates..."
        Me.lblLoadingTemplates.Visible = False
        '
        'Framework
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlFramework)
        Me.Controls.Add(Me.pnlGRC)
        Me.Name = "Framework"
        Me.Size = New System.Drawing.Size(494, 68)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.pnlFramework.ResumeLayout(False)
        Me.pnlGRC.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnSelectMalPath As System.Windows.Forms.Button
    Friend WithEvents lblMalPath As System.Windows.Forms.Label
    Friend WithEvents pnlFramework As System.Windows.Forms.Panel
    Friend WithEvents pnlGRC As System.Windows.Forms.Panel
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnSelectPathGRC As System.Windows.Forms.Button
    Friend WithEvents txtPathGRC As System.Windows.Forms.TextBox
    Friend WithEvents btnGRCOk As System.Windows.Forms.Button
    Friend WithEvents btnGRC As System.Windows.Forms.Button
    Friend WithEvents btnJavaTest As System.Windows.Forms.Button
    Friend WithEvents lblLoadingTemplates As System.Windows.Forms.Label

End Class
