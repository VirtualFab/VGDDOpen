<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMplabxIPC
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
        Me.btnIpcConnect = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtIpcIpAddress = New System.Windows.Forms.TextBox()
        Me.txtIpcPort = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblConnected = New System.Windows.Forms.Label()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'btnIpcConnect
        '
        Me.btnIpcConnect.Location = New System.Drawing.Point(271, 4)
        Me.btnIpcConnect.Name = "btnIpcConnect"
        Me.btnIpcConnect.Size = New System.Drawing.Size(75, 23)
        Me.btnIpcConnect.TabIndex = 0
        Me.btnIpcConnect.Text = "Connect"
        Me.btnIpcConnect.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "IP Address:"
        '
        'txtIpcIpAddress
        '
        Me.txtIpcIpAddress.Location = New System.Drawing.Point(79, 6)
        Me.txtIpcIpAddress.Name = "txtIpcIpAddress"
        Me.txtIpcIpAddress.Size = New System.Drawing.Size(73, 20)
        Me.txtIpcIpAddress.TabIndex = 2
        Me.txtIpcIpAddress.Text = "192.168.1.32"
        '
        'txtIpcPort
        '
        Me.txtIpcPort.Location = New System.Drawing.Point(202, 6)
        Me.txtIpcPort.Name = "txtIpcPort"
        Me.txtIpcPort.Size = New System.Drawing.Size(40, 20)
        Me.txtIpcPort.TabIndex = 4
        Me.txtIpcPort.Text = "4242"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(167, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(29, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Port:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(392, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Status:"
        '
        'lblConnected
        '
        Me.lblConnected.AutoSize = True
        Me.lblConnected.Location = New System.Drawing.Point(438, 9)
        Me.lblConnected.Name = "lblConnected"
        Me.lblConnected.Size = New System.Drawing.Size(79, 13)
        Me.lblConnected.TabIndex = 6
        Me.lblConnected.Text = "Not Connected"
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(15, 47)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.Size = New System.Drawing.Size(615, 81)
        Me.txtLog.TabIndex = 7
        '
        'frmMplabxIPC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(642, 262)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.lblConnected)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtIpcPort)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtIpcIpAddress)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnIpcConnect)
        Me.Name = "frmMplabxIPC"
        Me.Text = "MplabxIPC"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnIpcConnect As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtIpcIpAddress As System.Windows.Forms.TextBox
    Friend WithEvents txtIpcPort As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblConnected As System.Windows.Forms.Label
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
End Class
