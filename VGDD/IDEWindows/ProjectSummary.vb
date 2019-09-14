Namespace VGDDIDE
    Public Class ProjectSummary
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent ' System.Windows.Forms.UserControl

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False
        End Sub
    End Class
End Namespace