Namespace VGDDIDE
    Public Class WidgetPropertyGrid
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        Public Event PropertyValueChanged(ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs)
        Public Event PropertySortChanged()
        Public Shadows Event HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs)
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False
        End Sub

        Private Sub _ControlPropertyGrid_HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs) Handles PropertyGrid.HelpRequested
            RaiseEvent HelpRequested(sender, hlpevent)
        End Sub

        Private Sub _ControlPropertyGrid_PropertySortChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles PropertyGrid.PropertySortChanged
            RaiseEvent PropertySortChanged()
        End Sub

        Private Sub _ControlPropertyGrid_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid.PropertyValueChanged
            RaiseEvent PropertyValueChanged(e)
            PropertyGrid.Refresh()
        End Sub
    End Class
End Namespace