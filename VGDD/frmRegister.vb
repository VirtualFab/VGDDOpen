Imports System.IO

Public Class frmRegister

    Private Const ACTIVATEONLINE As String = "To activate this software please enter your license key and click on the Activate button." & vbCrLf & _
                    "Be sure to complete the activation procedure on the PC you will be using."
    Private Const ACTIVATEOFFLINE As String = "To activate this software please copy the Auth. Code here below and send via email it to the Support address." & vbCrLf & _
                    "You will be supplied with an Activation Key to be entered in the field below. Be sure to complete the activation procedure on the PC you will be using."
    Const UNREGVERS As String = "Application functionality is limited and only basic Widgets are available." & vbCrLf & _
                "Please purchase the full version using one of the links below to obtain full functionality."

    'Private Sub frmRegister_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
    '    If Me.Opacity = 0 Then
    '        VGDDCommon.FadeForm.FadeIn(Me, 99)
    '    End If
    'End Sub

    'Private Sub frmRegister_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    '    VGDDCommon.FadeForm.FadeOutAndWait(Me)
    'End Sub

    Private Sub frmRegister_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim blnCtrlKey As Boolean = Windows.Forms.Control.ModifierKeys And Keys.Control
        lblApplication.Text = "Visual Graphic Display Designer " & VGDDCommon.Common.VGDDVERSION
        lblActivated.TextAlign = ContentAlignment.MiddleCenter

#If (CONFIG = "Release" Or CONFIG = "ReleaseDebug") Or CONFIG = "Debug" Or CONFIG = "Evaluation" Then
        If LM.Checking Then
            lblActivation.Text = "Checking license..."
            lblActivation.ForeColor = Color.DarkOrange
            Do While LM.Checking
                Application.DoEvents()
                System.Threading.Thread.Sleep(1000)
            Loop
        End If

        'If MainShell._Lm.IsLicensed Or LM._LicStatus = "D" Then
        pnlPurchase.Visible = False
        pnlActivate.Dock = DockStyle.Fill
        pnlActivate.Visible = True
        btnAuthCodesFile.Visible = False

        If oMainShell._Lm.IsActivated <> MainShell.GuBru7Ad Or LM.LicStatus = "D" Or blnCtrlKey Then 'Not IsActivated
            Me.Width = 700
            Me.Height = 220
            lblActivation.Text = ACTIVATEONLINE
            lblActivation.ForeColor = Color.Black
            pnlActivate.Visible = True
            pnlActivateEnterKey.Visible = True
            'pnlActivate.Location = New Point(0, lblActivation.Height + 20)
            'pnlActivateEnterKey.BringToFront()
            pnlTransfer.Visible = False
#If CHINAMASTERS Then
            rbActivateOffline.Checked = True
            btnAuthCodesFile.Visible = True
#End If
        Else
            pnlActivate.Visible = False
            Me.Width = pnlTransfer.Width
            lblActivation.Visible = False
            pnlActivateEnterKey.Visible = False

            Me.Height = lblActivated.Top + lblActivated.Height + pnlTransfer.Height + 40
            With lblActivated
                .Visible = True
                .Anchor = AnchorStyles.None
                .Width = Me.Width
#If CONFIG = "Evaluation" Then
                lblApplication.Text &= " Evaluation Version"
                .Text = "Evaluation time-restricted version."
                pnlTransfer.Visible = False
#ElseIf CONFIG = "Debug" Then
                .Text = "Debug version"
                pnlTransfer.Visible = False
#Else
                .Text = "Application is activated. Thank you!"
#If Not CHINAMASTERS Then
                With pnlTransfer
                    '.BringToFront()
                    .Visible = True
                    .Anchor = AnchorStyles.None
                    .Top = lblActivated.Top + lblActivated.Height
                    .Left = 0
                    .Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Top + AnchorStyles.Bottom
                End With
#End If 'CHINAMASTERS
#End If 'CONFIG
                '.AutoSize = True
            End With
        End If
        'Else
        'lblActivated.Text = UNREGVERS
        'lblActivated.ForeColor = Color.Red
        'pnlPurchase.Visible = True
        'Me.Height = 460
        'End If
#ElseIf CONFIG = "Evaluation" Then
#If MASTERS Then
        lblApplication.Text &= " MASTERs Evaluation Version"
        lblActivated.Text = "MASTERs time-restricted version."
        pnlTransfer.Visible = False
#Else
        lblApplication.Text &= " Evaluation Version"
        lblActivated.Text = "Evaluation time-restricted version."
        pnlTransfer.Visible = False
#End If

#Else
        lblActivated.Text = UNREGVERS
        lblActivated.ForeColor = Color.DarkOrange
        pnlPurchase.Visible = True
        pnlActivate.Visible = False
        pnlTransfer.Visible = False
        Me.Height = lblActivated.Top + lblActivated.Height + pnlPurchase.Height + 20
        'pnlPurchase.Anchor = AnchorStyles.None
#End If
        If VGDDCommon.Common.DoFadings Then Me.Opacity = 0
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        lblBuyPayPal_Click(Nothing, Nothing)
    End Sub

    Private Sub lblBuyPayPal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblBuyPayPal.Click
        Dim strAppToRegister As String = "VGDD"
        VGDDCommon.Common.RunBrowser(REGISTERURL & "?AN=" & System.Web.HttpUtility.UrlEncode(strAppToRegister) & "&" & _
                          "VE=" & VGDDCommon.Common.VGDDVERSION & "&" & _
                          "AC=" & LM.LicFpUE)
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        lblBuyMicrochip_Click(Nothing, Nothing)
    End Sub

    Private Sub lblBuyMicrochip_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblBuyMicrochip.Click
        VGDDCommon.Common.RunBrowser(VGDDCommon.Common.URL_PURCHASE_MICROCHIPDIRECT)
    End Sub

#If CONFIG <> "DemoRelease" And CONFIG <> "DemoDebug" Then
    Private Sub btnActivate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnActivateOnline.Click
        If Not txtLicenseKey.Text.Trim.ToUpper = LM.HardSerial.Trim.ToUpper Then
            MessageBox.Show("Please fill in the License Key field with the Key you obtained from microchipDIRECT." & _
                            IIf(Windows.Forms.Control.ModifierKeys And Keys.Control, vbCrLf & "Expected: *" & LM.HardSerial.Trim.ToUpper & "*", ""), "Warning")
            Exit Sub
        End If
        If MessageBox.Show("Are you sure you want to activate this software on this PC?" & vbCrLf & _
                           "You won't be able to activate this copy on another PC.", "Activation confirmation", MessageBoxButtons.YesNo) = vbYes Then
            If oMainShell._Lm.ActivateLicense(Nothing, ACTIVATEURL) = "CraBrad7" Then
                oMainShell.UpdateCaption()
                Me.Close()
            End If
        End If
    End Sub

    Private Sub btnActivateOffline_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnActivateOffline.Click
        If oMainShell._Lm.ActivateLicenseOffline(txtOfflineActKey.Text.Trim) = "CraBrad7" Then
            oMainShell.UpdateCaption()
            Me.Close()
        End If
    End Sub

    Private Sub rbActivateOnline_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbActivateOnline.CheckedChanged, rbActivateOffline.CheckedChanged
        If rbActivateOnline.Checked Then
            pnlActivateOffline.Enabled = False
            pnlActivateOnline.Enabled = True
            lblActivation.Text = ACTIVATEONLINE
        Else
            pnlActivateOffline.Enabled = True
            pnlActivateOnline.Enabled = False
            lblActivation.Text = ACTIVATEOFFLINE
            txtAuthCode.Text = LM.LicFp
        End If
    End Sub

    Private Sub btnTransfer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTransfer.Click
        If MessageBox.Show("Do you confirm to deactivate License on this PC?", "License Transfer", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            Dim rc As String = oMainShell._Lm.TransferLic(ACTIVATEURL)
            If rc.StartsWith("OK") Then
                'If Not LM._HardLic.StartsWith("MD") Then
                '    MessageBox.Show("To deactivate this license please send an email to the support address asking for the transfer." & vbCrLf & _
                '                    "In the email specify, by copying and pasting it, the current AuthCode of this installation." & vbCrLf & _
                '                    "You can find it in the Help->About.")
                'Else
                MessageBox.Show(rc & vbCrLf & "License correctly deactivated on this PC. You can go on the other PC and start License Activation.")
                'End If
            Else
                MessageBox.Show("Error during transfer: " & rc, "Cannot Transfer License")
            End If
            oMainShell.UpdateCaption()
            Me.Close()
        End If
    End Sub
#End If

    Private Sub pnlPurchase_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles pnlPurchase.VisibleChanged
        If pnlPurchase.Visible Then
            pnlPurchase.Top = lblActivated.Height + lblActivated.Top + 10
            Me.Height = pnlPurchase.Top + pnlPurchase.Height + 50
            Me.Width = pnlPurchase.Width + 40
            pnlPurchase.Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Top + AnchorStyles.Bottom
        End If
    End Sub

    Const AUTHCODEFILE As String = "VGDD_AuthCodes.txt"
    Private Sub btnAuthCodesFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAuthCodesFile.Click
        Dim strPath As String = ""
        For Each di As DriveInfo In IO.DriveInfo.GetDrives()
            If (Not di.Name.StartsWith("A") And Not di.Name.StartsWith("B")) AndAlso (di.DriveType = DriveType.Removable Or DriveType.Network) Then
                If File.Exists(Path.Combine(di.Name, AUTHCODEFILE)) Then
                    strPath = di.Name
                End If
            End If
        Next
        Dim fileDialog As New OpenFileDialog()
        If strPath <> "" Then
            fileDialog.FileName = Path.Combine(strPath, AUTHCODEFILE)
        End If
        fileDialog.Filter = "VGDD_AuthCodes File|" & AUTHCODEFILE
        fileDialog.RestoreDirectory = True
        If fileDialog.ShowDialog() = DialogResult.OK Then
            Dim aAuthData As Collection = AuthCodesReadFile(fileDialog.FileName)
            Dim MyFp As String = oMainShell._Lm.oFp.Value
            Dim MyAuthData As New AuthData
            Dim MyComputerName As String = Environment.GetEnvironmentVariable("COMPUTERNAME")
            txtOfflineActKey.Text = ""
            For Each oAuthData As AuthData In aAuthData
                If oAuthData.Fp = MyFp Then
                    If MyAuthData.AuthCode <> "" Then
                        txtOfflineActKey.Text = MyAuthData.AuthCode
                        MyAuthData = oAuthData
                    End If
                End If
            Next
            If MyAuthData.Fp = "" Then
                For Each oAuthData As AuthData In aAuthData
                    If oAuthData.Computername = MyComputerName Then
                        MyAuthData = oAuthData
                    End If
                Next
            End If

            If MyAuthData.Fp <> "" Then
                If txtOfflineActKey.Text <> "" Then
                    MessageBox.Show("Activation Key successfully read from file!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("This PC data is already in selected VGDD_AuthCodes file, but no Activation Key found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                End If
                Exit Sub
            End If
            Dim oNewAuthData As New AuthData
            oNewAuthData.Fp = MyFp
            oNewAuthData.AuthCode = ""
            oNewAuthData.Computername = MyComputerName
            aAuthData.Add(oNewAuthData)
            If Not AuthCodesWriteFile(fileDialog.FileName, aAuthData) Then
                Exit Sub
            End If

            MessageBox.Show("VGDD_AuthCodes File successfully updated with info from this PC." & vbCrLf & "You can now proceed with the next PC or send the file to Virtualab.")
            'For Each di As DriveInfo In DriveInfo.GetDrives()
            '    If di.DriveType = DriveType.Removable AndAlso fileDialog.FileName.StartsWith(di.Name) Then
            '        MessageBox.Show("The selectet VGDD_AuthCodes file was on a removable drive. Now the ""Safely Remove Hardware"" dialog will be shown." & vbCrLf & "Remember to correctly eject the drive before to continue", "Eject Removable Drive", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            '        Try
            '            Shell("RunDll32.exe shell32.dll,Control_RunDLL hotplug.dll")
            '        Catch ex As Exception
            '        End Try
            '    End If
            'Next

        End If
    End Sub

    Public Function AuthCodesReadFile(ByVal strFileName As String) As Collection
        Dim sr As New StreamReader(strFileName)
        Dim AuthData As String = sr.ReadToEnd
        sr.Close()

        Dim aAuthCodes As New Collection
        For Each li As String In AuthData.Split(vbCrLf)
            Dim aCodes() As String = li.Split(",")
            Dim oAuthData As New AuthData
            If aCodes.Length > 0 Then
                oAuthData.Fp = aCodes(0).Trim
                If aCodes.Length > 1 Then
                    oAuthData.AuthCode = aCodes(1).Trim
                    If aCodes.Length > 2 Then
                        oAuthData.Computername = aCodes(2).Trim
                    End If
                End If
            End If
            If oAuthData.Fp.Trim <> "" Then
                aAuthCodes.Add(oAuthData)
            End If
        Next
        Return aAuthCodes
    End Function

    Public Function AuthCodesWriteFile(ByVal strFileName As String, ByVal aAuthData As Collection) As Boolean
        Try
            If File.Exists(strFileName) Then
                Try
                    File.Copy(strFileName, strFileName.Replace(".txt", ".bak"), True)
                Catch ex As Exception
                    MessageBox.Show("Cannot create a backup copy of VGDD_AuthCodes file. Check your disk.", "Warning")
                End Try
            End If
            Dim sw As New StreamWriter(strFileName)
            For Each oAuthData As AuthData In aAuthData
                If oAuthData.Fp.Trim <> "" Then
                    sw.WriteLine(oAuthData.Fp & "," & oAuthData.AuthCode & "," & oAuthData.Computername)
                End If
            Next
            sw.Flush()
            sw.Close()
            Return True
        Catch ex As Exception
            MessageBox.Show("Error writing  " & strFileName & ": " & ex.Message)
            Return False
        End Try
    End Function

    Public Class AuthData
        Public Fp As String = ""
        Public AuthCode As String = ""
        Public Computername As String = ""
    End Class

    Private Sub btnAuthCodeHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAuthCodeHelp.Click
        MessageBox.Show(("To activate offline this Application, follow these steps:||" & _
                         "Single PC:|" & _
                         "1) Simply copy the AuthCode here on the left and send it to virtualfab@gmail.com.|" & _
                         "2) Paste the Activation Key you received via email and you're up and running!||" & _
                         "Multiple PCs each with its own License:|" & _
                        "1a) Use an USB removable drive or a network folder|" & _
                        "1b) On the chosen drive create an empty text file named VGDD_AuthCodes.txt|" & _
                        "1c) Now click on the ""File..."" button and point to that file|" & _
                        "1d) Repeat step 1c on each PC you have to license|" & _
                        "|" & _
                        "2a) Submit the VGDD_AuthCodes.txt file to VirtualFab@gmail.com|" & _
                        "2b) On the chosen drive, replace the file with the one received back from VirtualFab (same file, but with Activation Keys)|" & _
                        "|" & _
                        "3a) On each PC repeat step a3) and the Activation Key will be loaded from the file|" & _
                        "3b) Click on the ""Activate"" button").Replace("|", vbCrLf), "Offline Activation Help")
    End Sub

    Private Sub lblBuyMicrochip_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblBuyMicrochip.MouseHover, lblBuyPayPal.MouseHover, PictureBox1.MouseHover, PictureBox2.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub lblBuyMicrochip_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblBuyMicrochip.MouseLeave, lblBuyPayPal.MouseLeave, PictureBox1.MouseLeave, PictureBox2.MouseLeave
        Me.Cursor = Cursors.Default
    End Sub
End Class