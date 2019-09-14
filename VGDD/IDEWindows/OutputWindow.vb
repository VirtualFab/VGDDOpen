Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms

Namespace VGDDIDE
    Public Class OutputWindow
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent
        Public WithEvents RichTextBox As System.Windows.Forms.RichTextBox


        ' <summary> 
        ' Required designer variable.
        ' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        Public Sub New()
            MyBase.New()
            ' This call is required by the Windows.Forms Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False
        End Sub

        Public Sub WriteMessage(ByVal Message As String)
            If RichTextBox.Text.Length > 1024 Then RichTextBox.ResetText()
            RichTextBox.Text &= Message & vbLf
        End Sub

        ' <summary> 
        ' Clean up any resources being used.
        ' </summary>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If (Not (components) Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        ' <summary> 
        ' Required method for Designer support - do not modify 
        ' the contents of this method with the code editor.
        ' </summary>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OutputWindow))
            Me.RichTextBox = New System.Windows.Forms.RichTextBox()
            Me.SuspendLayout()
            '
            'RichTextBox
            '
            Me.RichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
            Me.RichTextBox.Location = New System.Drawing.Point(0, 0)
            Me.RichTextBox.Name = "RichTextBox"
            Me.RichTextBox.Size = New System.Drawing.Size(355, 186)
            Me.RichTextBox.TabIndex = 2
            Me.RichTextBox.Text = ""
            '
            'OutputWindow
            '
            Me.ClientSize = New System.Drawing.Size(355, 186)
            Me.Controls.Add(Me.RichTextBox)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "OutputWindow"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            Me.TabText = "Output"
            Me.Text = "Output"
            Me.ResumeLayout(False)

        End Sub

    End Class
End Namespace
