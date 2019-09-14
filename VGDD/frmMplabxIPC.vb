Public Class frmMplabxIPC

    Private Sub frmMplabxIPC_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Timer1.Interval = 5000
        Timer1.Start()
    End Sub

    Private Sub oMplabxIpc_ResponseReceived(ByVal Response As String) Handles MainShell.oMplabxIpc.ResponseReceived
        If Me.InvokeRequired Then
            Dim d As New ResponseReceivedCallBack(AddressOf ResponseReceivedThreadSafe)
            Me.Invoke(d, New Object() {Response})
        Else
            ResponseReceivedThreadSafe(Response)
        End If
    End Sub

    Public Delegate Sub ResponseReceivedCallBack(ByVal Response As String)
    Public Sub ResponseReceivedThreadSafe(ByVal Response As String)
        CheckConnectStatus(Response)
        txtLog.Text &= Response.Replace(vbCr, "<CR>").Replace(vbLf, "<LF>") & vbCrLf
        txtLog.ScrollToCaret()
        Select Case Response.Split(" ")(0)
        End Select
    End Sub

    Private Sub CheckConnectStatus(ByVal Response As String)
        Static blnLastStatus As Nullable(Of Boolean) = Nothing
        If oMplabxIpc.IsConnected Then
            If blnLastStatus Is Nothing OrElse blnLastStatus = False Then
                If Response.StartsWith("VGDDipc") Then
                    lblConnected.Text = "Connected to " & Response
                Else
                    lblConnected.Text = "Connected"
                End If
                lblConnected.ForeColor = Color.DarkGreen
            End If
        Else
            If blnLastStatus Is Nothing OrElse blnLastStatus = True Then
                lblConnected.Text = "Not Connected"
                lblConnected.ForeColor = Color.DarkRed
            End If
            oMplabxIpc.IpcStart(txtIpcIpAddress.Text, txtIpcPort.Text)
        End If
        blnLastStatus = oMplabxIpc.IsConnected
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        CheckConnectStatus(String.Empty)
    End Sub

End Class