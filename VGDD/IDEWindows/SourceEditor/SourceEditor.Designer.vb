Namespace SourceEditor
    Partial Class Editor
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
            Me.ActiveEditor = New ICSharpCode.TextEditor.TextEditorControl()
            Me.SuspendLayout()
            '
            'ActiveEditor
            '
            Me.ActiveEditor.Dock = System.Windows.Forms.DockStyle.Fill
            Me.ActiveEditor.IsReadOnly = False
            Me.ActiveEditor.Location = New System.Drawing.Point(0, 0)
            Me.ActiveEditor.Name = "ActiveEditor"
            Me.ActiveEditor.Size = New System.Drawing.Size(364, 277)
            Me.ActiveEditor.TabIndex = 4
            Me.ActiveEditor.Text = "TextEditorControl1"
            '
            'Editor
            '
            Me.AllowDrop = True
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.ActiveEditor)
            Me.Name = "Editor"
            Me.Size = New System.Drawing.Size(364, 277)
            Me.ResumeLayout(False)

        End Sub

#End Region

        Friend WithEvents ActiveEditor As ICSharpCode.TextEditor.TextEditorControl
    End Class
End Namespace

