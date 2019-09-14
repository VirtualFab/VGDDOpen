Public NotInheritable Class AboutBox1

    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        UpdateLabels()
    End Sub

    Public Delegate Sub UpdateLabelsCallBack()

    Public Sub UpdateLabels()
        If Me.LabelVersion.InvokeRequired Then
            Dim d As New UpdateLabelsCallBack(AddressOf UpdateLabelsThreadSafe)
            Me.Invoke(d)
        Else
            UpdateLabelsThreadSafe()
        End If
    End Sub

    Public Sub UpdateLabelsThreadSafe()
        Dim blnCtrlKey As Boolean = Windows.Forms.Control.ModifierKeys And Keys.Control
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)

        Me.LabelProductName.Text = My.Application.Info.Trademark 'My.Application.Info.ProductName & " - " &

        Me.LabelVersion.Text = String.Format("Open Source Version {0}", VGDDCommon.Common.VGDDVERSION)
        Me.LabelVersion.ForeColor = Color.DarkGreen
        Me.LabelVersion.Text &= vbCrLf & "Build " & My.Application.Info.Version.ToString & vbCrLf &
            "Built on " & VGDDCommon.Common.BuildDateTime.ToString("yyyy/MM/dd HH:mm:ss")
        ToolStripMenuItem1.Enabled = True
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.lnkEmail.Text = "Contact virtualfab@gmail.com"
        Me.lblDescription.Text = My.Application.Info.Description
    End Sub

    Private Sub LabelCompanyName_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkEmail.LinkClicked
        Try
            Dim maileraddy As String = ("mailto:virtualfab@gmail.com?subject=VGDD;")
            System.Diagnostics.Process.Start(maileraddy)
        Catch ex As Exception
            MessageBox.Show("Cannot launch email program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'Private Sub ToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
    '    Clipboard.SetText(LM.LicFp)
    'End Sub

    Private Sub CopyInfoToClipboardToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyInfoToClipboardToolStripMenuItem.Click
        Clipboard.SetText(LabelVersion.Text)
    End Sub


    Private Sub lblForums_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblForums.LinkClicked
        VGDDCommon.Common.RunBrowser("http://vgdd.freeforums.org/index.php")
    End Sub

    Private Sub Logo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Logo.Click
        VGDDCommon.Common.RunBrowser("http://virtualfab.it/VGDD")
    End Sub

    Private Sub lnkWebSite_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkWebSite.LinkClicked
        VGDDCommon.Common.RunBrowser("http://virtualfab.it/VGDD")
    End Sub
End Class
