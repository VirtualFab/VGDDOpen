Public Class frmMessageBoxWithCheckBox
End Class

Public Class MessageBoxWithCheckBox
    Private Shared MyInstance As frmMessageBoxWithCheckBox
    Public Overloads Shared Function Show(ByVal text As String) As System.Windows.Forms.DialogResult
        Return Show(text, "", Windows.Forms.MessageBoxButtons.OK, Nothing, Nothing)
    End Function

    Public Overloads Shared Function Show(ByVal text As String, ByVal caption As String) As System.Windows.Forms.DialogResult
        Return Show(text, caption, Windows.Forms.MessageBoxButtons.OK, Nothing, Nothing)
    End Function

    Public Overloads Shared Function Show(ByVal text As String, ByVal caption As String, buttons As Windows.Forms.MessageBoxButtons, ByVal OptionText As String, ByRef OptionChecked As Nullable(Of Boolean)) As System.Windows.Forms.DialogResult
        If MyInstance Is Nothing Then
            MyInstance = New frmMessageBoxWithCheckBox
        End If
        MyInstance.lblMsg.Text = text
        MyInstance.Text = caption
        Select Case buttons
            Case Windows.Forms.MessageBoxButtons.OK
                MyInstance.btnYes.Visible = False
                MyInstance.btnNo.Visible = False
                MyInstance.btnOk.Visible = True
            Case Windows.Forms.MessageBoxButtons.YesNo
                MyInstance.btnYes.Visible = True
                MyInstance.btnNo.Visible = True
                MyInstance.btnOk.Visible = False
            Case Else
                Throw New Exception("MessageBoxWithCheckBox unhandled buttons")
        End Select
        If OptionText Is Nothing Then
            MyInstance.chkOption.Visible = False
        Else
            MyInstance.chkOption.Text = OptionText
            MyInstance.chkOption.Checked = IIf(OptionChecked Is Nothing, False, OptionChecked)
            MyInstance.chkOption.Visible = True
        End If
        Show = MyInstance.ShowDialog
        OptionChecked = MyInstance.chkOption.Checked
    End Function
End Class
