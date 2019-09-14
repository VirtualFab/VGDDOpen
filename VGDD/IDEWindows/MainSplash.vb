Namespace VGDDIDE
    Public Class MainSplash
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        Public Event NewProject As EventHandler
        Public Event OpenProject As EventHandler
        Public Event OpenDemo As EventHandler
        Public Event OpenRecent As EventHandler

        Public SelectedDemo As String

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False
        End Sub

        Private Sub lstRecent_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstRecent.DoubleClick
            If lstRecent.SelectedItem <> "" Then
                RaiseEvent OpenRecent(Nothing, Nothing)
            End If
        End Sub

        Private Sub lnkSplashNewProject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkSplashNewProject.Click
            RaiseEvent NewProject(Nothing, Nothing)
        End Sub

        Private Sub lnkSplashOpenProject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkSplashOpenProject.Click
            RaiseEvent OpenProject(Nothing, Nothing)
        End Sub

        Private Sub lnkSplashOpenDemo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkSplashOpenDemo.Click
            SelectedDemo = String.Empty
            If frmChooseDemo.ShowDialog() = DialogResult.OK Then
                SelectedDemo = frmChooseDemo.cmbDemos.SelectedItem
                RaiseEvent OpenDemo(Nothing, Nothing)
            End If
        End Sub

        Private Sub MainSplash_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            'AboutBox11.Top = (Me.Height - Me.Top - AboutBox11.Height) / 2
        End Sub
    End Class
End Namespace