<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectMAL
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSelectMAL))
        Me.btnAddFramework = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblLog = New System.Windows.Forms.Label()
        Me.btnUseFramework = New System.Windows.Forms.Button()
        Me.txtPathMAL = New System.Windows.Forms.ComboBox()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.SuspendLayout()
        '
        'btnAddFramework
        '
        Me.btnAddFramework.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAddFramework.Location = New System.Drawing.Point(664, 162)
        Me.btnAddFramework.Name = "btnAddFramework"
        Me.btnAddFramework.Size = New System.Drawing.Size(94, 23)
        Me.btnAddFramework.TabIndex = 35
        Me.btnAddFramework.Text = "Add Framework"
        Me.btnAddFramework.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 9)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(235, 13)
        Me.Label6.TabIndex = 33
        Me.Label6.Text = "Select Framework (MLA/MLA Legacy/Harmony)"
        '
        'lblLog
        '
        Me.lblLog.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLog.Location = New System.Drawing.Point(12, 181)
        Me.lblLog.Name = "lblLog"
        Me.lblLog.Size = New System.Drawing.Size(646, 40)
        Me.lblLog.TabIndex = 36
        '
        'btnUseFramework
        '
        Me.btnUseFramework.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUseFramework.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnUseFramework.Location = New System.Drawing.Point(664, 32)
        Me.btnUseFramework.Name = "btnUseFramework"
        Me.btnUseFramework.Size = New System.Drawing.Size(94, 23)
        Me.btnUseFramework.TabIndex = 37
        Me.btnUseFramework.Text = "Use Framework"
        Me.btnUseFramework.UseVisualStyleBackColor = True
        '
        'txtPathMAL
        '
        Me.txtPathMAL.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPathMAL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple
        Me.txtPathMAL.FormattingEnabled = True
        Me.txtPathMAL.Location = New System.Drawing.Point(12, 32)
        Me.txtPathMAL.Name = "txtPathMAL"
        Me.txtPathMAL.Size = New System.Drawing.Size(646, 164)
        Me.txtPathMAL.TabIndex = 38
        '
        'btnRemove
        '
        Me.btnRemove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemove.Enabled = False
        Me.btnRemove.Location = New System.Drawing.Point(664, 96)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(94, 23)
        Me.btnRemove.TabIndex = 39
        Me.btnRemove.Text = "Remove from list"
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(12, 198)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(175, 13)
        Me.LinkLabel1.TabIndex = 40
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "Get Frameworks from Microchip site"
        '
        'frmSelectMAL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(770, 223)
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.txtPathMAL)
        Me.Controls.Add(Me.btnUseFramework)
        Me.Controls.Add(Me.lblLog)
        Me.Controls.Add(Me.btnAddFramework)
        Me.Controls.Add(Me.Label6)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmSelectMAL"
        Me.Text = "Select Framework"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnAddFramework As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblLog As System.Windows.Forms.Label
    Friend WithEvents btnUseFramework As System.Windows.Forms.Button
    Friend WithEvents txtPathMAL As System.Windows.Forms.ComboBox
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
End Class
