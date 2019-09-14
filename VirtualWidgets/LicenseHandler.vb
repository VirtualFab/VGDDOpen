Imports System.Reflection
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Public Class LicenseHandler
    Implements VGDDCommon.IVGDDLicenseHandler

    Private Shared MyAssembly As Assembly
    Private Shared MyAssemblyName As String
    Private Shared MyCompanyName As String

    Public Shared Sub ShowLicenseHandler()
        Dim oFrmLicenseHander As New LicenseHandler
        oFrmLicenseHander.Show()
    End Sub

    Public Sub New()
        InitializeComponent()
        MyAssembly = Assembly.GetAssembly(Me.GetType)
        MyAssemblyName = MyAssembly.GetName.Name
        MyCompanyName = MyBase.CompanyName
    End Sub

    Private Sub frmLicense_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Text = "License Handler for " & MyCompanyName & "'s " & MyAssemblyName
        If IsLicensed() Then
            lblBuy.Text = MyAssemblyName & " is registered. Thank you!"
            lblBuy.ForeColor = Color.DarkGreen
            btnExtract.Enabled = True
            pnlBuy.Visible = False
        End If
    End Sub

    Public ReadOnly Property IsLicensed() As Boolean Implements VGDDCommon.IVGDDLicenseHandler.IsLicensed
        Get
            Dim _LicKey As String = "Software\" & MyCompanyName
            Dim oKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(_LicKey, True)
            If oKey Is Nothing Then
                Return False
            End If
            Dim Lic As String = ""
            Dim strKeyValue As String = MyAssemblyName
            If oKey.GetValue(strKeyValue) IsNot Nothing Then
                Lic = oKey.GetValue(strKeyValue)
            End If
            If Lic = "" Then Return False

            Dim md5 As MD5 = MD5CryptoServiceProvider.Create()
            Dim dataMd5 As Byte() = md5.ComputeHash(Encoding.ASCII.GetBytes(MyAssemblyName & "VFW"))
            Dim sb As New StringBuilder()
            For i As Integer = 0 To dataMd5.Length - 1
                sb.AppendFormat("{0:x2}", dataMd5(i))
            Next
            Return sb.ToString() = Lic
        End Get
    End Property

    Private Sub btnEnterLicense_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnterLicense.Click
        If txtLicenseCode.Text <> "" Then
            Dim _LicKey As String = "Software\" & MyCompanyName
            Dim oKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(_LicKey, True)
            If oKey Is Nothing Then
                oKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(_LicKey)
            End If
            If oKey Is Nothing Then
                MessageBox.Show("Cannot access the Registry!", MyAssemblyName & " LicenseHandler")
                Exit Sub
            End If
            oKey.SetValue(MyAssemblyName, txtLicenseCode.Text, Microsoft.Win32.RegistryValueKind.String)
            oKey.Flush()
            oKey.Close()
            If IsLicensed() Then
                btnExtract.Enabled = True
                pnlBuy.Visible = False
                frmLicense_Load(Nothing, Nothing)
            Else
                txtLicenseCode.ForeColor = Color.Red
            End If
        End If
    End Sub

    Private Sub lblBuy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblBuy.Click
        lblBuy.Text = "Please wait, lounching browser..."
        Application.DoEvents()
        VGDDCommon.Common.RunBrowser("http://virtualfab.no-ip.org/lm/register.php?AN=" & System.Web.HttpUtility.UrlEncode(MyAssemblyName))
        System.Threading.Thread.Sleep(1000)
        lblBuy.Text = "Switch to your browser window to complete payment procedure"
    End Sub

    Private Sub btnExtract_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExtract.Click
        If VGDDCommon.Mal.MalPath = "" Then
            MessageBox.Show("To extract the library you must first define where MAL (Microchip Application Libraries) are located in your PC." & vbCrLf & _
                       "Please go in VGDD MplabX Wizard and specify MAL path and then come back here.", "MAL path not yet specified")
            Exit Sub
        End If
        Dim strVgddLibDir As String = Path.Combine(VGDDCommon.Mal.MalPath, "VGDD")
        If Not Directory.Exists(strVgddLibDir) Then
            Directory.CreateDirectory(strVgddLibDir)
        End If
        'For Each ResName As String In MyAssembly.GetManifestResourceNames
        'If ResName.EndsWith(".a") Or ResName.EndsWith(".h") Then
        'If Not VGDDCommon.Common.ExtractResourceToFile(ResName, System.IO.Path.Combine(strVgddLibDir, ResName), MyAssembly) Then
        For Each oFile As FileInfo In New DirectoryInfo(strVgddLibDir).GetFiles
            Try
                File.Delete(oFile.FullName)
            Catch ex As Exception
            End Try
        Next
        If Not VGDDCommon.Common.ExtractZippedResource("Library.zip", strVgddLibDir, MyAssembly, MyAssemblyName) Then
            MessageBox.Show("Cannot extract " & MyAssemblyName & ".Library.zip to disk!", "Error during extraction")
            Exit Sub
        End If
        'End If
        'Next
        Application.DoEvents()
        Me.Focus()
        MessageBox.Show("Libraries for PIC24 and PIC32 families successfully extracted to " & strVgddLibDir & vbCrLf & "Click on OK to open extraction folder", "Success")
        Try
            Process.Start(strVgddLibDir)
        Catch ex As Exception
            MessageBox.Show("Error opening folder " & strVgddLibDir & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class