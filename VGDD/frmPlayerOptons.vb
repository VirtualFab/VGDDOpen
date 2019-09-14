Imports System.IO
Imports System.Xml
Imports VGDDCommon

Public Class frmPlayerOptions
    Private Sub frmPlayerOptions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        pbBgBitmap.Image = Nothing
        If Common.ProjectPlayerBgBitmapName <> "" Then
            Dim oBgBitmap As VGDDImage = Common.GetBitmap(Common.ProjectPlayerBgBitmapName)
            If oBgBitmap IsNot Nothing Then pbBgBitmap.Image = oBgBitmap.Bitmap
        End If
        pnlBgColor.BackColor = Common.ProjectPlayerBgColour
    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub

    Private Sub btnSelectBgBitmap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectBgBitmap.Click
        Dim oBitmapChooser As New frmBitmapChooser
        oBitmapChooser.ShowDialog()
        If oBitmapChooser.ChosenBitmap IsNot Nothing Then
            Common.ProjectPlayerBgBitmapName = oBitmapChooser.ChosenBitmap.Name
            pbBgBitmap.Image = oBitmapChooser.ChosenBitmap.Bitmap
            oMainShell.ProjectChanged()
        End If
    End Sub

    Private Sub btnSelectBgColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectBgColor.Click
        Dim oColorPicker As New frmColorEditor
        oColorPicker.ShowDialog()
        Common.ProjectPlayerBgColour = oColorPicker.SelectedColor
        pnlBgColor.BackColor = Common.ProjectPlayerBgColour
        oMainShell.ProjectChanged()
    End Sub
End Class