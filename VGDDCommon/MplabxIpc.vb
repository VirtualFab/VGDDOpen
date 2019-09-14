Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Text
Imports System.Net.NetworkInformation
Imports System.Windows.Forms

Public Class MplabxIpc
    Implements IDisposable

    Private oTcpClient As TcpClient
    Private ServerStream As NetworkStream
    Private threadConnect As Thread
    Private threadReceive As Thread
    Private _IsConnected As Boolean
    Private _ServerAddress As String
    Private _ServerPort As Integer
    Private _sbLog As New StringBuilder

    Public Event ResponseReceived(ByVal Response As String)
    Public Event ConnectionStatusChanged(ByVal Connected As Boolean)

    Private WithEvents TimerCheckMplabXIpc As New Windows.Forms.Timer
    Private _LastResponse As String = String.Empty
    Public ConnectedTo As String
    Private blnLastStatus As Nullable(Of Boolean) = Nothing

    Public Sub IpcStop()
        If oTcpClient Is Nothing Then Exit Sub
        If TimerCheckMplabXIpc IsNot Nothing Then
            TimerCheckMplabXIpc.Stop()
            TimerCheckMplabXIpc = Nothing
        End If
        'If threadReceive IsNot Nothing Then
        '    threadReceive.Join(500)
        'End If
        'threadReceive.Abort()
        'threadReceive = Nothing
        If ServerStream IsNot Nothing Then ServerStream.Close()
        ServerStream = Nothing
        oTcpClient.Close()
        'Client = Nothing
        _IsConnected = False
        Thread.Sleep(500)
        RaiseEvent ConnectionStatusChanged(False)
    End Sub

    Public Sub IpcStart(ByVal strAddress As String, ByVal intPort As Integer)
        Try
            _ServerAddress = strAddress
            _ServerPort = intPort
            TimerCheckMplabXIpc = New Windows.Forms.Timer
            TimerCheckMplabXIpc.Interval = 100
            TimerCheckMplabXIpc.Start()
            IpcTryConnectThreaded()
        Catch ex As Exception
            MessageBox.Show(ex.Message & vbCrLf & vbCrLf & _
                            "Please Review Client Address", _
                            "Error Sending Message", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private _TryingConnect As Boolean
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub IpcTryConnect()
        Try
            If oTcpClient IsNot Nothing Then
                oTcpClient.Close()
                oTcpClient = Nothing
            End If
            oTcpClient = New TcpClient(AddressFamily.InterNetwork) 'IPV4
            _TryingConnect = True
            oTcpClient.Connect(_ServerAddress, _ServerPort)
            ServerStream = oTcpClient.GetStream()
            threadReceive = New Thread(New ThreadStart(AddressOf IpcReceive))
            threadReceive.Start()
            'Thread.Sleep(250)
            IpcSend("HELLO", "VGDD version " & VGDDCommon.Common.VGDDVERSION)
        Catch socketEx As SocketException
            If blnLastStatus Is Nothing OrElse blnLastStatus = True Then
                RaiseEvent ConnectionStatusChanged(False)
            End If
            blnLastStatus = False
            If socketEx.Message.Contains("refused") AndAlso TimerCheckMplabXIpc IsNot Nothing AndAlso TimerCheckMplabXIpc.Interval < 60000 Then
                TimerCheckMplabXIpc.Interval = TimerCheckMplabXIpc.Interval * 1.2
            End If
        Catch ex As Exception
            'MessageBox.Show(ex.Message, "Error connecting to server", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        _TryingConnect = False
    End Sub

    Private Sub IpcTryConnectThreaded()
        If _TryingConnect Then Exit Sub
        If threadConnect IsNot Nothing Then
            'If strAddress = _ServerAddress And intPort = _ServerPort Then
            '    Exit Sub
            'End If
            threadConnect.Abort()
            'threadConnect.Join(500)
            threadConnect = Nothing
        End If
        threadConnect = New Thread(New ThreadStart(AddressOf IpcTryConnect))
        threadConnect.Start()
    End Sub

    Private Sub TimerCheckMplabXIpc_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TimerCheckMplabXIpc.Tick
        TimerCheckMplabXIpc.Enabled = False
        If TimerCheckMplabXIpc.Interval < 5000 Then TimerCheckMplabXIpc.Interval = 5000
        Try
            If oTcpClient Is Nothing OrElse oTcpClient.Client Is Nothing OrElse Not oTcpClient.Connected Then
                IpcTryConnectThreaded()
            Else
                If Not IsConnected Then
                    oTcpClient.Close()
                    If blnLastStatus Is Nothing OrElse blnLastStatus = True Then
                        RaiseEvent ConnectionStatusChanged(False)
                    End If
                    IpcTryConnectThreaded()
                Else
                    If blnLastStatus Is Nothing OrElse blnLastStatus = False Then
                        If _LastResponse IsNot Nothing AndAlso _LastResponse.StartsWith("VGDDipc") Then
                            RaiseEvent ConnectionStatusChanged(True)
                        End If
                    End If
                End If
                blnLastStatus = IsConnected
            End If
        Catch ex As Exception
        End Try
        TimerCheckMplabXIpc.Enabled = True
    End Sub

    Public Sub IpcSend(ByVal strCommand As String, ByVal strPayload As String)
        Dim strMessage As String = strCommand
        Dim bMessage As Byte()
        If strPayload <> String.Empty Then strMessage &= "|" & strPayload
        bMessage = Encoding.ASCII.GetBytes(strMessage & vbLf)
        ServerStream.Write(bMessage, 0, bMessage.Length)
        ServerStream.Flush()
        If VGDDCommon.Common.oMplabXIpcDebug IsNot Nothing Then
            VGDDCommon.Common.oMplabXIpcDebug.MplabXIpcDebugWrite("Sent: " & strMessage)
        End If
    End Sub

    Public ReadOnly Property Log As String
        Get
            If _sbLog.Length > 100000 Then
                _sbLog.Remove(0, 90000)
            End If
            Return _sbLog.ToString
        End Get
    End Property

    'Public Function IpcWaitForResponse() As String
    '    _LastResponse = String.Empty
    '    Dim i As Integer
    '    For i = 1 To 20
    '        System.Threading.Thread.Sleep(100)
    '        Application.DoEvents()
    '        If _LastResponse.Replace(vbCr, "").Replace(vbLf, "") <> String.Empty Then Exit For
    '    Next
    '    If i = 21 Then Return String.Empty
    '    Return _LastResponse.Replace(vbCr, "").Replace(vbLf, "")
    'End Function

    Private Sub IpcReceive()
        Try
            Dim ReceivedData As String
            Dim inBytes(4096) As Byte
            Do While oTcpClient IsNot Nothing AndAlso oTcpClient.Connected
                If ServerStream.DataAvailable Then
                    'System.Threading.Thread.Sleep(250)
                    Dim intReadBytes As Integer = ServerStream.Read(inBytes, 0, 4096)
                    ReceivedData = Encoding.ASCII.GetString(inBytes).Substring(0, intReadBytes).Trim.Replace(vbLf, vbCrLf)
                    _LastResponse = ReceivedData
                    Dim strRec As String = ReceivedData.Replace(vbCr, "<cr>").Replace(vbLf, "<lf>")
                    Debug.Print("REC:" & strRec)
                    Dim aReceivedData As String() = ReceivedData.Split("|")
                    Select Case aReceivedData(0)
                        Case "WELCOME"
                            ConnectedTo = aReceivedData(1)
                            RaiseEvent ConnectionStatusChanged(True)
                            _IsConnected = True
                        Case "CHECK_LINK"
                            IpcSend("LINK_OK", "")
                    End Select
                    'If Not ReceivedData.StartsWith("KA") AndAlso ReceivedData <> String.Empty Then
                    _sbLog.Append(strRec & vbCrLf)
                    RaiseEvent ResponseReceived(ReceivedData)
                    'End If
                    If VGDDCommon.Common.oMplabXIpcDebug IsNot Nothing Then
                        VGDDCommon.Common.oMplabXIpcDebug.MplabXIpcDebugWrite("Received: " & ReceivedData)
                    End If
                Else
                    Thread.Sleep(100)
                End If
            Loop
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

    Public ReadOnly Property IsConnected As Boolean
        Get
            If oTcpClient Is Nothing OrElse oTcpClient.Client Is Nothing Then Return False
            If oTcpClient.Connected Then
                Dim ipProperties As IPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties()
                _IsConnected = False
                For Each x As TcpConnectionInformation In ipProperties.GetActiveTcpConnections()
                    If x.LocalEndPoint.Equals(oTcpClient.Client.LocalEndPoint) _
                        AndAlso x.RemoteEndPoint.Equals(oTcpClient.Client.RemoteEndPoint) _
                        AndAlso x.State = TcpState.Established Then
                        _IsConnected = True
                        Exit For
                    End If
                Next
                Return _IsConnected
            Else
                Return False
            End If
        End Get
    End Property

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If oTcpClient IsNot Nothing Then
                    oTcpClient.Close()
                    oTcpClient = Nothing
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
