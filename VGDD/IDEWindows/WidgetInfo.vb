Namespace VGDDIDE
    Public Class WidgetInfo
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        Public Shadows Event HelpRequested()

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False

        End Sub

        Private Sub lblWidgetHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblWidgetHelp.Click
            RaiseEvent HelpRequested()
        End Sub
    End Class
End Namespace