Partial Class FontList
    Inherits System.Windows.Forms.UserControl
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

#Region "Component Designer generated code"

    ''' <summary> 
    ''' Required method for Designer support - do not modify 
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.lstFont = New System.Windows.Forms.ListBox()
        Me.txtFont = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        ' 
        ' lstFont
        ' 
        Me.lstFont.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        Me.lstFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstFont.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.lstFont.ItemHeight = 20
        Me.lstFont.Location = New System.Drawing.Point(0, 21)
        Me.lstFont.Name = "lstFont"
        Me.lstFont.Size = New System.Drawing.Size(220, 282)
        Me.lstFont.TabIndex = 0
        ' 
        ' txtFont
        ' 
        Me.txtFont.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        Me.txtFont.Location = New System.Drawing.Point(0, 0)
        Me.txtFont.Name = "txtFont"
        Me.txtFont.Size = New System.Drawing.Size(220, 20)
        Me.txtFont.TabIndex = 1
        ' 
        ' FontList
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.txtFont)
        Me.Controls.Add(Me.lstFont)
        Me.Name = "FontList"
        Me.Size = New System.Drawing.Size(220, 307)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private WithEvents lstFont As System.Windows.Forms.ListBox
    Private WithEvents txtFont As System.Windows.Forms.TextBox
End Class
