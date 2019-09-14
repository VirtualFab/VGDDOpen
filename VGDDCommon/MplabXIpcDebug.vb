Public Class MplabXIpcDebug

    Public Delegate Sub MplabXIpcDebugWriteCallBack(ByVal strMessage As String)
    Public Sub MplabXIpcDebugWrite(ByVal strMessage As String)
        If Me.InvokeRequired Then
            Dim d As New MplabXIpcDebugWriteCallBack(AddressOf MplabXIpcDebugWriteThreadSafe)
            Me.Invoke(d, New Object() {strMessage})
        Else
            MplabXIpcDebugWriteThreadSafe(strMessage)
        End If
    End Sub

    Private Sub MplabXIpcDebugWriteThreadSafe(ByVal strMessage As String)
        txtDebugLog.AppendText(strMessage & vbCrLf)
    End Sub
End Class