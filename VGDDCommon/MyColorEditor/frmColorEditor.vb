Namespace VGDDCommon
    Public Class frmColorEditor

        Public ColourDepth As Integer = 16
        Public SelectedColor As System.Drawing.Color

        Private Sub ColorPicker1_ColorPicked(ByVal sender As Object) Handles ColorPicker1.ColorPicked
            Me.SelectedColor = ColorPicker1.Value
            'Common.CurrentScreen.CustomColors = Me.ColorPicker1.CustomColours
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub frmColorEditor_HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs) Handles Me.HelpRequested
            Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_ColorChooser"
            Common.HelpProvider.SetHelpKeyword(Me, ColorPicker1.tabColors.SelectedTab.Text)
        End Sub

        Private Sub frmColorEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.HelpProvider1 = Common.HelpProvider
            For Each oScheme As VGDDScheme In Common._Schemes.Values
                If oScheme.Palette IsNot Nothing AndAlso oScheme.Palette.Name = Common.CurrentScreen.PaletteName Then
                    Me.ColorPicker1.Palette = oScheme.Palette
                    Exit For
                End If
            Next
        End Sub

    End Class
End Namespace
