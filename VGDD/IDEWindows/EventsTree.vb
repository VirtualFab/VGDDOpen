Namespace VGDDIDE
    Public Class EventsTree
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        Public Event EventNodeClick As EventHandler
        Public Event EventNodeDoubleClick As EventHandler
        Public Event EventNodeEditAllEvents As EventHandler

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False

        End Sub

        Private Sub tvEvents_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvEvents.NodeMouseClick
            tvEvents.SelectedNode = e.Node
            RaiseEvent EventNodeClick(Nothing, Nothing)
        End Sub

        Private Sub tvEvents_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvEvents.NodeMouseDoubleClick
            RaiseEvent EventNodeDoubleClick(Nothing, Nothing)
        End Sub

        Private Sub EditAllEventsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditAllEventsToolStripMenuItem.Click
            RaiseEvent EventNodeEditAllEvents(Nothing, Nothing)
        End Sub
    End Class
End Namespace