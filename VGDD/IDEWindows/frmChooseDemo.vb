Public Class frmChooseDemo

    Private Sub cmbDemos_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbDemos.SelectedIndexChanged
        btnExtract.Enabled = cmbDemos.SelectedIndex >= 0
    End Sub

    Private Sub btnExtract_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExtract.Click
        Me.Close()
    End Sub
End Class