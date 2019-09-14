Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Drawing

<Designer("System.Windows.Forms.Design.ParentControlDesigner,System.Design", GetType(IDesigner))> _
Public Class LabeledPanel

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.ContainerControl, True)
        Me.SetStyle(ControlStyles.DoubleBuffer _
          Or ControlStyles.UserPaint _
          Or ControlStyles.AllPaintingInWmPaint, _
          True)

        ' This enables mouse support such as the Mouse Wheel

        SetStyle(ControlStyles.UserMouse, True)

        ' This will repaint the control whenever it is resized

        SetStyle(ControlStyles.ResizeRedraw, True)

        Me.UpdateStyles()
    End Sub

    Public Property Title As String
        Set(ByVal value As String)
            Me.lblHeader.Text = value
        End Set
        Get
            Return Me.lblHeader.Text
        End Get
    End Property

    Public Property TitleForeColor As Color
        Set(ByVal value As Color)
            Me.lblHeader.ForeColor = value
        End Set
        Get
            Return Me.lblHeader.ForeColor
        End Get
    End Property

    Public Property TitleBackColor As Color
        Set(ByVal value As Color)
            Me.lblHeader.BackColor = value
        End Set
        Get
            Return Me.lblHeader.BackColor
        End Get
    End Property

    Public Property TitleFont As Font
        Set(ByVal value As Font)
            Me.lblHeader.Font = value
        End Set
        Get
            Return Me.lblHeader.Font
        End Get
    End Property

    Public Property TitleAlignement As ContentAlignment
        Set(ByVal value As ContentAlignment)
            Me.lblHeader.TextAlign = value
        End Set
        Get
            Return Me.lblHeader.TextAlign
        End Get
    End Property

    Private Sub LabeledPanel_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'If Me.Parent IsNot Nothing Then
        '    Me.lblHeader.Left = Me.Parent.Padding.Left
        '    Me.lblHeader.Width = Me.Width - Me.Parent.Padding.Horizontal
        'Else
        Me.lblHeader.Left = 3
        Me.lblHeader.Width = Me.Width - 6
        'End If
    End Sub
End Class
