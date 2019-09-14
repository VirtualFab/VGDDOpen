Imports System.ComponentModel.Design.Serialization
Imports VGDDCommon

Public Class frmExternalWidgets

    Private Sub frmExternalWidgets_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RefreshList()
    End Sub

    Private Sub RefreshList()
        ToolStripBtnUnloadLib.Enabled = False
        Application.DoEvents()
        'ImageList1.Images.Clear()
        ListView1.Items.Clear()
        For Each oExtWidget As ExternalWidget In ExternalWidgetsHandler.ExternalWidgets.Values
            Dim oItem As New ListViewItem(oExtWidget.Name)
            oItem.Tag = oExtWidget
            oItem.SubItems.Add(oExtWidget.Author)
            oItem.SubItems.Add(oExtWidget.Loaded)
            oItem.SubItems.Add(oExtWidget.IsLicensed)
            ListView1.Items.Add(oItem)
        Next
        ListView1.SelectedIndices.Clear()
    End Sub

    Private Sub ToolStripBtnNewLib_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripBtnNewLib.Click
        Dim fileDialog As New OpenFileDialog()
        fileDialog.Filter = "VGDD External Widgets Library|*.dll"
        fileDialog.Multiselect = False
        fileDialog.RestoreDirectory = True
        If fileDialog.ShowDialog() = DialogResult.OK Then
            If oMainShell.oExternalWidgetsHandler.LoadAssembly(fileDialog.FileName) Then
                ExternalWidgetsHandler.CollectionXmlDoc.Save(ExternalWidgetsHandler.CollectionXmlPath)
                ToolStripReload.PerformClick()
            End If
        End If
    End Sub

    Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
        ToolStripBtnUnloadLib.Enabled = False
        ListViewImages.Items.Clear()
        If ListView1.SelectedItems.Count > 0 Then
            ToolStripBtnUnloadLib.Enabled = True
            Dim oEW As ExternalWidget = ListView1.SelectedItems(0).Tag
            ImageList1 = oEW.ImageList
            Dim imgIdx As Integer = 0
            For Each oBitmap As Bitmap In ImageList1.Images
                ListViewImages.Items.Add("", imgIdx)
                imgIdx += 1
            Next
            ListViewImages.LargeImageList = ImageList1
            ListViewImages.View = View.LargeIcon
            ToolStripBtnUnloadLib.Enabled = True
            'ToolStripEnterLicense.Enabled = Not oEW.IsLicensed
        End If
    End Sub

    Private Sub ToolStripReload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripReload.Click
        'ExternalWidgetsHandler.ExternalWidgets.Clear()
        'MainShell.LoadExternalWidgets()
        RefreshList()
    End Sub

    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        ToolStripEnterLicense.PerformClick()
    End Sub

    Private Sub ToolStripEnterLicense_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripEnterLicense.Click
        If ListView1.SelectedItems.Count = 0 Then Exit Sub
        Dim oEW As ExternalWidget = ListView1.SelectedItems(0).Tag
        Dim oLicenseHndlerType As Type = oEW.Assembly.GetType("LicenseHandler")
        If oLicenseHndlerType IsNot Nothing Then
            Dim mi As System.Reflection.MethodInfo = oLicenseHndlerType.GetMethod("ShowLicenseHandler")
            mi.Invoke(Nothing, Nothing)
            'Dim id As InstanceDescriptor = New InstanceDescriptor(mi, Nothing)
            'Dim instance As Object = id.Invoke
            'If instance.GetMethod("GetCode") IsNot Nothing Then

            'End If
        End If
        'Dim strCode As String = InputBox("License AuthCode:", "Enter License AuthCode")
        'If strcode <> "" Then
        '    oEW.SetAuthCode(strCode)
        '    MainShell.oExternalWidgetsHandler.SaveAll()
        '    ToolStripReload.PerformClick()
        'End If
    End Sub

    Private Sub ToolStripBtnUnloadLib_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripBtnUnloadLib.Click
        Dim oEW As ExternalWidget = ListView1.SelectedItems(0).Tag
        If MessageBox.Show("Do you really want to unload " & oEW.Name & " library? (Runtime errors could occour if you do so)", "Unload library", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            oMainShell.oExternalWidgetsHandler.UnLoadAssembly(oEW.Name)
            ToolStripReload.PerformClick()
        End If
    End Sub
End Class