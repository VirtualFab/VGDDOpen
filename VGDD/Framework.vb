Imports VGDDCommon
Imports System.IO
Imports System.Xml

Public Class Framework

    Public Event FrameworkChanged()

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If Not Me.DesignMode Then
            lblMalPath.Text = VGDDCommon.Mal.Path2FrameworkDescription
        End If
    End Sub

    Private Sub Framework_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.DesignMode Then
            If Common.ProjectPathGRC = String.Empty Then
                Common.ProjectPathGRC = Mal.ComputeGrcPath(Mal.MalPath)
            End If
            txtPathGRC.Text = Common.ProjectPathGRC
        End If
    End Sub

    Public Property Title As String
        Get
            Return GroupBox2.Text
        End Get
        Set(ByVal value As String)
            GroupBox2.Text = value
        End Set
    End Property

    Public Function CheckMal() As Boolean
        lblMalPath.Text = VGDDCommon.Mal.MalPath
        If VGDDCommon.Mal.MalPath <> String.Empty Then
            lblMalPath.Text = VGDDCommon.Mal.Path2FrameworkDescription()
            If VGDDCommon.Mal.CheckMalVersion(VGDDCommon.Mal.MalPath) = "OK" Then
                lblMalPath.ForeColor = Color.Green
                CheckMal = True
            Else
                lblMalPath.ForeColor = Color.Red
                CheckMal = False
            End If
        Else
            lblMalPath.ForeColor = Color.Red
            CheckMal = False
        End If
    End Function

    Public aConversionRules(2, 1)
    Public Sub ReadCodeGenConversionRules()
        Dim sr As StreamReader
#If CONFIG = "Debug" Then
        sr = New StreamReader(Path.GetFullPath(Path.Combine(Application.ExecutablePath, "..\..\..\..\VGDDCommon\CodeGen\CodeGenConversionRules.xml")))
#Else
        sr = New StreamReader(Common.GetResourceStream("CodeGenConversionRules.xml", Assembly.GetExecutingAssembly))
#End If
        Dim xml As String = sr.ReadToEnd
        sr.Close()
        Dim XmlConversionRules As New XmlDocument
        XmlConversionRules.PreserveWhitespace = False
        XmlConversionRules.LoadXml(xml)
        Dim i As Integer = 0
        For Each strRow As String In XmlConversionRules.SelectSingleNode("/ConversionRules").InnerText.Split(vbCrLf)
            If strRow.Trim <> String.Empty Then
                ReDim Preserve aConversionRules(2, i + 1)
                aConversionRules(0, i) = strRow.Substring(0, 28).Trim
                aConversionRules(1, i) = strRow.Substring(29).Trim
                i += 1
            End If
        Next
    End Sub

    Private Sub btnSelectMalPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectMalPath.Click
        Dim strOldMal As String = VGDDCommon.Mal.FrameworkName
        frmSelectMAL.ShowDialog()
        If CheckMal() Then
            Me.ParentForm.Enabled = False
            Me.ParentForm.Cursor = Cursors.WaitCursor
            lblLoadingTemplates.ForeColor = Color.Maroon
            lblLoadingTemplates.Visible = True
            Application.DoEvents()
            Common.MplabXExtractTemplates()
            CodeGen.LoadCodeGenTemplates()
            oMainShell._Toolbox.InitializeToolbox()
            RaiseEvent FrameworkChanged()

            If strOldMal <> VGDDCommon.Mal.FrameworkName AndAlso Common.ProjectFileName <> String.Empty Then
                If MessageBox.Show("Convert existing event code from " & strOldMal & " to " & VGDDCommon.Mal.FrameworkName & "?", "Code conversion", MessageBoxButtons.YesNo) = vbYes Then
                    Dim strOldCode As String = oMainShell.AllEventsToXml(Nothing)
                    ReadCodeGenConversionRules()
                    For i As Integer = 0 To aConversionRules.GetUpperBound(1) - 1
                        If aConversionRules(0, i) IsNot Nothing AndAlso aConversionRules(1, i) IsNot Nothing Then
                            If strOldMal = "MLALegacy" Then
                                If strOldCode.Contains(aConversionRules(0, i)) Then
                                    strOldCode = strOldCode.Replace(aConversionRules(0, i), aConversionRules(1, i))
                                End If
                            Else
                                If strOldCode.Contains(aConversionRules(1, i)) Then
                                    strOldCode = strOldCode.Replace(aConversionRules(1, i), aConversionRules(0, i))
                                End If
                            End If
                        End If
                    Next
                    oMainShell.AllEventsFromXML(strOldCode)
                End If
            End If

            lblLoadingTemplates.Visible = False
            Me.ParentForm.Enabled = True
            Me.ParentForm.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub lblMalPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblMalPath.Click
        Try
            System.Diagnostics.Process.Start(Mal.MalPath)
        Catch ex As Exception
            MessageBox.Show("Error running command " & vbCrLf & lblMalPath.Text & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub lblMalPath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblMalPath.TextChanged
        If Not Me.DesignMode AndAlso lblMalPath.Text <> "" AndAlso Not lblMalPath.Text.Contains("<MAL Path>") Then
            Try
                CheckJavaPaths()
                If txtPathGRC.ForeColor = Color.Red Then
                    txtPathGRC.Text = Mal.ComputeGrcPath(lblMalPath.Text)
                    CheckJavaPaths()
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub CheckJavaPaths()
        If File.Exists(txtPathGRC.Text) Then
            txtPathGRC.ForeColor = Color.Green
            btnJavaTest.Enabled = True
        Else
            txtPathGRC.ForeColor = Color.Red
            btnJavaTest.Enabled = False
        End If
    End Sub

    Private Sub btnGRCOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGRCOk.Click
        pnlGRC.Visible = False
        pnlFramework.Visible = True
        Common.ProjectPathGRC = txtPathGRC.Text
    End Sub

    Private Sub btnGRC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGRC.Click
        txtPathGRC.Text = Common.ProjectPathGRC
        pnlGRC.Visible = True
        pnlFramework.Visible = False
    End Sub

    Private Sub txtPathGRC_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPathGRC.TextChanged
        CheckJavaPaths()
    End Sub

    Private Sub btnJavaTest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnJavaTest.Click
        Common.ProjectPathGRC = txtPathGRC.Text
        If JavaTest() Then
            MessageBox.Show("GRC integration test PASSED.", "GRC and Java JRE configuration test")
        ElseIf JavaError <> "RETRY" Then
            MessageBox.Show("GRC integration test ERROR: " & JavaError, "GRC and Java JRE configuration test")
        End If
    End Sub

    Private JavaError As String = ""
    Private Function JavaTest() As Boolean
        If Me.txtPathGRC.Text = "" Then Return False
        Using oGrc As New GrcProject
            With oGrc
                JavaError = .RunGrc("TEST")
                If JavaError = "OK" Then
                    Return True
                Else
                    MessageBox.Show("GRC Test Error: " & JavaError & vbCrLf & "Please check Java Command in Global Preferences and GRC path", "GRC Test NOT passed")
                    'Dim strKey As String = "jarfile\shell\open\command"
                    'Try
                    '    Dim oKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(strKey, False)
                    '    If oKey.GetValue("") IsNot Nothing Then '"C:\Program Files (x86)\Java\jre7\bin\javaw.exe" -jar "%1" %*
                    '        My.Settings.JavaCommand = oKey.GetValue("").replace("""%1"" %*", "").replace("javaw.exe", "java.exe")
                    '        MessageBox.Show("Java Command has been detected from registry. Please check it and run TEST again to verify it is working")
                    '        JavaError = "RETRY"
                    '    End If
                    'Catch ex As Exception
                    'End Try
                End If
                Return False
            End With
        End Using
    End Function

    Private Sub btnSelectPathGRC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectPathGRC.Click
        Dim fileDialog As New OpenFileDialog()
        fileDialog.FileName = txtPathGRC.Text
        fileDialog.Filter = "Microchip's GRC jar file|grc.jar|All Files|*.*"
        fileDialog.RestoreDirectory = True
        If fileDialog.ShowDialog() = DialogResult.OK Then
            txtPathGRC.Text = fileDialog.FileName
            'CheckGRC()
        End If
    End Sub
End Class
