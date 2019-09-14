Imports VGDDCommon
Imports System.IO

Public Class CodeGenLocation
    Implements System.ComponentModel.ISupportInitialize

    Public Event ProjectModified As EventHandler
    Public Event OptionsChanged As EventHandler
    Public Event BadChoices As EventHandler

    Public WithEvents oMplabxIpc As MplabxIpc

    Private IsLoading As Boolean = False

    Public Sub New()
        IsLoading = True
        InitializeComponent()
        IsLoading = False
    End Sub

    Private Sub CodeGenLocation_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.DesignMode Then
            Me.BeginInit()
            lblVGDDToolChain.Text = String.Empty
            lblMplabXConfig.Text = String.Empty
            lblMplabXToolChain.Text = String.Empty
            txtIpcIpAddress.Text = Common.MplabXIpcIpAddress
            oMplabxIpc = MplabX.oMplabxIpc
            chkUseIPC.Checked = MplabX.IpcEnabled
            Me.EndInit()
            CheckMplabProject()
            'If oMplabxIpc.IsConnected Then
            '    DisplayCurrentMplabProjectSettings()
            'Else
            '    CheckMplabProject()
            'End If
        End If
    End Sub

    Public Delegate Sub CheckMplabProjectCallBack()
    Public Sub CheckMplabProject()
        If Me.InvokeRequired Then
            Dim d As New CheckMplabProjectCallBack(AddressOf CheckMplabProjectThreadSafe)
            Me.Invoke(d) ', New Object() {Connected})
        Else
            CheckMplabProjectThreadSafe()
        End If
    End Sub

    Private Sub CheckMplabProjectThreadSafe()
        If chkUseIPC.Checked AndAlso oMplabxIpc.IsConnected Then 'TODO: Common.MplabXProjectPath = String.Empty AndAlso
            lblMplabXProject.Text = "None"
            lblMplabXConfig.Text = ""
            lblMplabXToolChain.Text = ""
            Common.MplabXSelectedConfig = String.Empty
            oMplabxIpc.IpcSend("GET_MPLABX_ACTIVECONFIG_NAME", "")
            oMplabxIpc.IpcSend("GET_MPLABX_ACTIVECONFIG_TOOLCHAINNAME", "")
            'oMplabxIpc.IpcSend("GET_MPLABX_ACTIVECONFIG_DEVICENAME", "")
            oMplabxIpc.IpcSend("GET_MPLABX_PROJECTPATH", "")
            For i As Integer = 1 To 10
                If Common.MplabXSelectedConfig <> String.Empty Then
                    Exit For
                End If
                Application.DoEvents()
                System.Threading.Thread.Sleep(50)
            Next
        End If
        CheckCodeGenPath()
    End Sub

    Public Delegate Sub DisplayCurrentMplabProjectSettingsCallBack()
    Public Sub DisplayCurrentMplabProjectSettings()
        If Me.InvokeRequired Then
            Dim d As New DisplayCurrentMplabProjectSettingsCallBack(AddressOf DisplayCurrentMplabProjectSettingsThreadSafe)
            Me.Invoke(d) ', New Object() {Connected})
        Else
            DisplayCurrentMplabProjectSettingsThreadSafe()
        End If
    End Sub

    Private Sub DisplayCurrentMplabProjectSettingsThreadSafe()
        btnChangeMplabXProject.Visible = Not oMplabxIpc.IsConnected
        With lblMplabXIpcConnectionStatus
            If oMplabxIpc.IsConnected Then
                .Text = "Connected to " & oMplabxIpc.ConnectedTo
                .ForeColor = Color.DarkGreen
                pnlIpcConnect.Enabled = False
                chkUseIPC.Enabled = True
                'chkUseIPC.Checked = True
            Else
                .Text = "Disconnected"
                .ForeColor = Color.Red
                pnlIpcConnect.Enabled = True
                'oMplabxIpc = Nothing
            End If
        End With
        'End If
        lblMplabXProject.Text = Common.MplabXProjectPath
        lblMplabXConfig.Text = Common.MplabXSelectedConfig
        lblMplabXToolChain.Text = Common.ProjectCompiler
        If Common.MplabXProjectPath = String.Empty Then
            If chkUseIPC.Checked Then
                lblMplabXProject.Text = "None - Open/Select project in MPLAB X please!"
            Else
                lblMplabXProject.Text = ""
            End If
            lblMplabXProject.ForeColor = Color.Red
        Else
            MplabX.LoadMplabxProject()
            lblMplabXProject.ForeColor = Color.Green
            'lblMplabXConfig.ForeColor = Color.Green
            'lblMplabXToolChain.ForeColor = Color.Green
        End If
        CheckRbWhere()
    End Sub

    Private Sub lblCodeGenPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblCodeGenPath.Click
        If Not Directory.Exists(Common.CodeGenDestPath) Then
            If MessageBox.Show("Destination path " & Common.CodeGenDestPath & " doesn't exist. Create folder?", "Create destination folder", MessageBoxButtons.YesNo) = vbNo Then
                Exit Sub
            End If
            Try
                Directory.CreateDirectory(Common.CodeGenDestPath)
            Catch ex As Exception
                MessageBox.Show("Cannot create folder " & Common.CodeGenDestPath & ": " & ex.Message, "Destination folder creation failed")
                Exit Sub
            End Try
        End If
        Try
            System.Diagnostics.Process.Start(Common.CodeGenDestPath)
        Catch ex As Exception
            MessageBox.Show("Error running command " & vbCrLf & Common.CodeGenDestPath & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub CheckCodeGenPath()
        lblWarning.Visible = False
        btnVGDDSettings.Visible = False
        'If Common.MplabXProjectPath <> String.Empty Then

        'End If
        lblMplabXProject.Text = Common.MplabXProjectPath
        lblCodeGenPath.Text = Common.CodeGenDestPath
        If Directory.Exists(Common.CodeGenDestPath) Then
            lblCodeGenPath.ForeColor = Color.DarkGreen
        Else
            lblCodeGenPath.ForeColor = Color.Black
        End If

        lblVGDDToolChain.Text = Common.ProjectCompiler
        lblMplabXProject.ForeColor = Color.Red
        'lblMplabXConfig.ForeColor = Color.Red
        If MplabX.IpcEnabled Then
            'Not chkUseIPC.Checked OrElse Not oMplabxIpc.IsConnected Then
            If Common.MplabXProjectPath <> String.Empty Then
                If Not Directory.Exists(Common.MplabXProjectPath) Then
                    lblMplabXProject.ForeColor = Color.Red
                Else
                    lblMplabXProject.ForeColor = Color.Green
                End If
            End If

        ElseIf MplabX.oProjectXmlDoc IsNot Nothing Then
            Dim oToolChainNode As Xml.XmlNode = MplabX.oProjectXmlDoc.SelectSingleNode(String.Format("/configurationDescriptor/confs/conf[@name='{0}']/toolsSet/languageToolchain", Common.MplabXSelectedConfig))
            If oToolChainNode IsNot Nothing Then
                lblMplabXConfig.ForeColor = Color.Green
                lblMplabXToolChain.Text = oToolChainNode.InnerText
                lblWarning.Text = String.Format("Warning: selected MPLAB X config ({0}) / VGDD Settings ({1}) mismatch", oToolChainNode.InnerText, Common.ProjectCompiler)
                Select Case oToolChainNode.InnerText
                    Case "XC16", "C30"
                        If Common.ProjectCompilerFamily <> "C30" Then
                            lblMplabXConfig.ForeColor = Color.Red
                            lblWarning.Visible = True
                        End If
                    Case "XC32", "C32"
                        If Common.ProjectCompilerFamily <> "C32" Then
                            lblMplabXConfig.ForeColor = Color.Red
                            lblWarning.Visible = True
                        End If
                End Select
            Else
                lblMplabXConfig.ForeColor = Color.Red
                lblWarning.Visible = True
                lblWarning.Text = String.Format("Warning: selected MPLAB X config ({0}) does not exists in MPLAB X project", Common.MplabXSelectedConfig)
                RaiseEvent BadChoices(Nothing, Nothing)
                Exit Sub
            End If
        End If
        If lblMplabXConfig.ForeColor = Color.Red Then
            btnVGDDSettings.Visible = True
            RaiseEvent BadChoices(Nothing, Nothing)
        End If
    End Sub

    Private Sub btnVGDDSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVGDDSettings.Click
        Dim oProjectSettings As New frmProjectSettings
        oProjectSettings.ShowDialog()
        CheckCodeGenPath()
    End Sub

    Private Sub btnDestPathOther_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDestPathOther.Click
        Dim oDlg As New FolderBrowserDialog
        With oDlg
            If Directory.Exists(Common.CodeGenDestPath) Then
                .SelectedPath = Common.CodeGenDestPath
            Else
                .SelectedPath = Common.VGDDProjectPath
            End If
            SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
            .Description = "Select destination folder for generated code"
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                txtCodeGenPathOther.Text = RelativePath.Evaluate(Common.VGDDProjectPath, .SelectedPath)
            End If
        End With
    End Sub

    Private Sub txtCodeGenPathOther_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodeGenPathOther.TextChanged
        If txtCodeGenPathOther.Text <> "" Then
            Common.CodeGenLocationOptionsOk = False
            Common.CodeGenDestPath = Path.GetFullPath(Path.Combine(Common.VGDDProjectPath, txtCodeGenPathOther.Text))
            If Directory.Exists(Common.CodeGenDestPath) Then
                CheckRbWhere() 'Recalculate path
                Common.CodeGenLocationOptionsOk = True
                'If Path.IsPathRooted(txtCodeGenPathOther.Text) Then
                '    Common.CodeGenDestPath = txtCodeGenPathOther.Text
                'Else
                '    Common.CodeGenDestPath = Path.GetFullPath(Path.Combine(Common.ProjectPath, txtCodeGenPathOther.Text))
                'End If
            End If
        End If
        CheckCodeGenPath()
        If Not Me.IsLoading Then
            RaiseEvent OptionsChanged(Nothing, Nothing)
        End If
        txtCodeGenPathOther.Focus()
    End Sub

    Private Sub btnChangeMplabXProject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChangeMplabXProject.Click
        If frmMplabXSelectProject.ShowDialog = DialogResult.OK Then
            CheckCodeGenPath()
            CheckRbWhere()
            If Not Me.IsLoading Then
                RaiseEvent OptionsChanged(Nothing, Nothing)
            End If
        End If
    End Sub

    Private Sub rbWhere_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles rbWhereMplabxFolder.CheckedChanged, _
                    rbWhereVgddFolder.CheckedChanged, _
                    rbWhereMplabxParentFolder.CheckedChanged, _
                    rbWhereOtherFolder.CheckedChanged, _
                    rbWhereMplabxConfigFolder.CheckedChanged
        If Not Me.IsLoading Then
            CheckRbWhere()
            Common.ProjectChanged = True
            RaiseEvent OptionsChanged(Nothing, Nothing)
        End If
    End Sub

    Private Sub CheckRbWhere()
        txtCodeGenPathOther.Enabled = False
        Common.CodeGenLocationOptionsOk = False
        If rbWhereMplabxFolder.Checked Then
            'grpMplabX.Enabled = True
            If Common.MplabxCheckProjectPath(Common.MplabXProjectPath) Then
                lblMplabXProject.ForeColor = Color.Green
                Common.CodeGenLocation = 1
                Common.CodeGenLocationOptionsOk = True
            Else
                lblMplabXProject.ForeColor = Color.Red
            End If
        ElseIf rbWhereVgddFolder.Checked Then
            'grpMplabX.Enabled = False
            Common.CodeGenLocationOptionsOk = True
            Common.CodeGenLocation = 2
        ElseIf rbWhereMplabxParentFolder.Checked Then
            'grpMplabX.Enabled = True
            If Common.MplabxCheckProjectPath(Common.MplabXProjectPath) Then
                lblMplabXProject.ForeColor = Color.Green
                Common.CodeGenLocationOptionsOk = True
                Common.CodeGenLocation = 3
            Else
                lblMplabXProject.ForeColor = Color.Red
            End If
        ElseIf rbWhereOtherFolder.Checked Then
            'grpMplabX.Enabled = False
            txtCodeGenPathOther.Enabled = True
            If Common.VGDDProjectPath <> String.Empty Then
                Common.CodeGenDestPath = Path.GetFullPath(Path.Combine(Common.VGDDProjectPath, txtCodeGenPathOther.Text))
            End If
            If txtCodeGenPathOther.Text = "" Then
                txtCodeGenPathOther.Focus()
            Else
                Common.CodeGenLocationOptionsOk = True
            End If
            Common.CodeGenLocation = 4
        ElseIf rbWhereMplabxConfigFolder.Checked Then
            Common.CodeGenDestPath = Path.GetFullPath(Path.Combine(Path.Combine(Path.Combine(Path.Combine( _
                    Common.MplabXProjectPath, ".."), _
                    "src"), _
                    "system_config"), _
                    Common.MplabXSelectedConfig))
            Common.CodeGenLocationOptionsOk = True
            Common.CodeGenLocation = 5
        End If
        CheckCodeGenPath()
    End Sub

    Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
        Me.IsLoading = True
        If Common.MplabXProjectPath <> "" Then lblMplabXProject.Text = Common.MplabXProjectPath
        If Common.MplabXProjectPath <> "" Then
            Select Case Common.CodeGenLocation
                Case 1
                    rbWhereMplabxFolder.Checked = True
                Case 2
                    rbWhereVgddFolder.Checked = True
                Case 3
                    rbWhereMplabxParentFolder.Checked = True
                Case 4
                    txtCodeGenPathOther.Text = RelativePath.Evaluate(Common.VGDDProjectPath, Common.CodeGenDestPath)
                    rbWhereOtherFolder.Checked = True
                Case 5
                    rbWhereMplabxConfigFolder.Checked = True
            End Select
        Else
            rbWhereVgddFolder.Checked = True
        End If
        CheckRbWhere()
    End Sub

    Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
        Me.IsLoading = False
    End Sub

    Private Sub Framework1_FrameworkChanged()
        RaiseEvent OptionsChanged(Nothing, Nothing)
    End Sub

    Private Sub chkUseIPC_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkUseIPC.CheckedChanged
        If chkUseIPC.Checked Then
            grpMplabX.Text = "MPLAB X Project - Data from VGDD-Link plug-in"
            If IsLoading Then Exit Sub
            pnlIpcConnect.Enabled = False
            My.Settings.MplabXIpcIpAddress = Common.MplabXIpcIpAddress
            My.Settings.MplabXIpcPort = Common.MplabXIpcPort
            My.Settings.Save()
            lblMplabXIpcConnectionStatus.Text = "Not connected"
            If oMplabxIpc IsNot Nothing Then
                IsLoading = True
                oMplabxIpc.IpcStop()
                IsLoading = False
            End If
            'oMplabxIpc = New MplabxIpc
            oMplabxIpc.IpcStart(Common.MplabXIpcIpAddress, Common.MplabXIpcPort)
            For i As Integer = 0 To 50
                If oMplabxIpc.IsConnected Then
                    Exit For
                End If
                System.Threading.Thread.Sleep(100)
            Next
            If My.Computer.Keyboard.ShiftKeyDown Then
                If Common.oMplabXIpcDebug IsNot Nothing Then
                    Common.oMplabXIpcDebug.Close()
                    Common.oMplabXIpcDebug = Nothing
                End If
                Common.oMplabXIpcDebug = New MplabXIpcDebug
                Common.oMplabXIpcDebug.Show()
            End If
        Else
            grpMplabX.Text = "MPLAB X Project - Manual definitions"
            If IsLoading Then Exit Sub
            If oMplabxIpc IsNot Nothing Then oMplabxIpc.IpcStop()
            pnlIpcConnect.Enabled = True
            chkUseIPC.Enabled = False
        End If
        MplabX.IpcEnabled = chkUseIPC.Checked
        My.Settings.MplabXUseIpc = MplabX.IpcEnabled
    End Sub

    Private Sub btnMplabXIpcConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMplabXIpcConnect.Click
        If chkUseIPC.Checked Then
            chkUseIPC_CheckedChanged(Nothing, Nothing)
        Else
            chkUseIPC.Checked = True
        End If
    End Sub

    Private Sub txtIpcIpAddress_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtIpcIpAddress.TextChanged
        Common.MplabXIpcIpAddress = txtIpcIpAddress.Text
    End Sub

    Public Delegate Sub oMplabxIpc_ConnectionStatusChangedCallBack(ByVal Connected As Boolean)
    Public Sub oMplabxIpc_ConnectionStatusChanged(ByVal Connected As Boolean) Handles oMplabxIpc.ConnectionStatusChanged
        If Me.InvokeRequired Then
            Dim d As New oMplabxIpc_ConnectionStatusChangedCallBack(AddressOf oMplabxIpc_ConnectionStatusChangedThreadSafe)
            Me.Invoke(d, New Object() {Connected})
        Else
            CheckMplabProjectThreadSafe()
        End If
    End Sub

    Private Sub oMplabxIpc_ConnectionStatusChangedThreadSafe(ByVal Connected As Boolean)
        pnlIpcConnect.Enabled = Not Connected
        'If Not Connected Then
        '    If Not IsLoading Then
        '        'chkUseIPC.Checked = False
        '    End If
        'Else
        'End If
        CheckMplabProject()
        DisplayCurrentMplabProjectSettings()
    End Sub

    Private Sub oMplabxIpc_ResponseReceived(ByVal Response As String) Handles oMplabxIpc.ResponseReceived
        If Response <> String.Empty AndAlso Not Response.Contains("ADDFILE") Then
            DisplayCurrentMplabProjectSettings()
        End If
    End Sub
End Class
