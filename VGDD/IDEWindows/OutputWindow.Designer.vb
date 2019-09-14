Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class OutputWindow
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
            components = New System.ComponentModel.Container()
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.RichTextBox = New System.Windows.Forms.RichTextBox()
            Me.lblHeader = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'RichTextBox
            '
            Me.RichTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.RichTextBox.Location = New System.Drawing.Point(2, 15)
            Me.RichTextBox.Name = "RichTextBox"
            Me.RichTextBox.Size = New System.Drawing.Size(367, 207)
            Me.RichTextBox.TabIndex = 2
            Me.RichTextBox.Text = ""
            '
            'lblHeader
            '
            Me.lblHeader.BackColor = System.Drawing.SystemColors.ActiveCaption
            Me.lblHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblHeader.Location = New System.Drawing.Point(3, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New System.Drawing.Size(319, 17)
            Me.lblHeader.TabIndex = 3
            Me.lblHeader.Text = "Output"
            '
            'OutputWindow
            '
            Me.Controls.Add(Me.lblHeader)
            Me.Controls.Add(Me.RichTextBox)
            Me.Name = "OutputWindow"
            Me.Size = New System.Drawing.Size(371, 224)
            Me.ResumeLayout(False)

        End Sub

    End Class
End Namespace
