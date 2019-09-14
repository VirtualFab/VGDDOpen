Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.IO
Imports System.Web
Imports VGDDCommon
Imports System.Text.RegularExpressions
Imports VirtualFabUtils.Utils

Namespace VGDDIDE
    Public Class HTMLEditor
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        Private WithEvents PreviewTimer As New Timer
        Private WebPagesRoot As String

        Public Sub New()
            MyBase.New()
            ' This call is required by the Windows.Forms Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False
            chkPreview.Checked = True
        End Sub

        Public Sub Clear()
            btnSettings.Enabled = False
            CustomTabControl1.TabPages.Clear()
            WebBrowser2.DocumentText = String.Empty
        End Sub

        Private Sub HTMLEditor_DockStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DockStateChanged
            SetHtmlEnviron()
        End Sub

        Private Sub HTMLEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.WebBrowser2.ScriptErrorsSuppressed = True
            '            TextEditorControl1.ActiveEditor.Document.TextContent = String.Empty
            SaveToolStripButton.Enabled = False
            chkPreview.Visible = True
        End Sub

        Private Sub SetHtmlEnviron()
            If Common.ProjectHtmlOutputType <> String.Empty Then
                cmbType.Text = Common.ProjectHtmlOutputType
            End If
            lblWebPagesFolder.Text = Common.ProjectHtmlWebPagesFolder
            pnlGenerate.Visible = False
            If Common.ProjectHtmlWebPagesFolder <> String.Empty Then
                WebPagesRoot = Path.GetFullPath(Path.Combine(Common.VGDDProjectPath, Common.ProjectHtmlWebPagesFolder))
                pnlGenerate.Visible = True
                txtTargetUrl.Text = Common.ProjectHtmlTargetUrl
                txtUsername.Text = Common.ProjectHtmlTargetUser
                txtPassword.Text = Common.ProjectHtmlTargetPassword
            End If
        End Sub

        Private EditedFileName As String = ""

        Private Sub CreateTabForFile(ByVal strFilename As String)
            If strFilename <> String.Empty Then
                EditedFileName = strFilename
            Else
                EditedFileName = "New file"
            End If
            Dim oTabPage As New TabPage
            Dim oEditor As New SourceEditor.Editor
            With oEditor
                '.AllowDrop = True
                .Dock = System.Windows.Forms.DockStyle.Fill
                .Location = New System.Drawing.Point(3, 3)
            End With
            oTabPage.Tag = strFilename
            oTabPage.Text = Path.GetFileName(EditedFileName)
            oTabPage.Enabled = True
            oTabPage.Controls.Add(oEditor)
            CustomTabControl1.TabPages.Add(oTabPage)
            CustomTabControl1.SelectedTab = oTabPage
            If strFilename <> String.Empty Then
                oEditor.OpenFile(Path.GetFullPath(EditedFileName))
            End If
            AddHandler oEditor.TextChanged, AddressOf HTMLEditor_TextChanged
            Select Case Path.GetExtension(EditedFileName)
                Case ".js"
                    oEditor.ActiveEditor.Document.HighlightingStrategy() =
                        ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("JavaScript")
                Case ".css"
                    oEditor.ActiveEditor.Document.HighlightingStrategy() =
                        ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("CSS")
                Case Else
                    oEditor.ActiveEditor.Document.HighlightingStrategy() =
                        ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("MCHP_HTML")
            End Select
            SetHtmlEnviron()
            SaveToolStripButton.Enabled = False
            WebBrowser2.Visible = True
            pnlSettings.Visible = False
            PreviewTimer.Interval = 10
            PreviewTimer.Enabled = True
        End Sub

        Private Sub NewToolStripButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewToolStripButton.Click
            If WebPagesRoot Is Nothing Then
                MessageBox.Show("Please use Settings to define WebPages folder.")
                Exit Sub
            End If
            CreateTabForFile(String.Empty)
        End Sub

        Private Sub OpenToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripButton.Click
            If WebPagesRoot Is Nothing Then
                MessageBox.Show("Please use Settings to define WebPages folder.")
                Exit Sub
            End If
            'If SaveToolStripButton.Enabled Then
            '    If MessageBox.Show("Save changes to current file?", "Warning", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            '        SaveToolStripButton_Click(Nothing, Nothing)
            '        Application.DoEvents()
            '    End If
            'End If
            Dim dlg As OpenFileDialog = New OpenFileDialog
            dlg.DefaultExt = "htm"
            dlg.Filter = "MCHP HTML files|*.htm; *.inc; *.css; *.cgi; *.js; *.xml;|All Files|*.*"
            dlg.RestoreDirectory = False
            If WebPagesRoot <> "" Then dlg.InitialDirectory = WebPagesRoot
            If (dlg.ShowDialog = Windows.Forms.DialogResult.OK) Then
                CreateTabForFile(dlg.FileName)
            End If
        End Sub

        Private Sub PreView()
            If CustomTabControl1.SelectedTab Is Nothing Then Exit Sub
            Dim oEditor As SourceEditor.Editor = CustomTabControl1.SelectedTab.Controls(0)
            Dim strHtml As String = oEditor.ActiveEditor.Document.TextContent
            PreviewTimer.Enabled = False
            If strHtml Is Nothing Then Exit Sub
            Dim intPos1 As Integer, intPos2 As Integer
            Do While True
                intPos1 = strHtml.IndexOf("~inc:")
                If intPos1 < 0 Then Exit Do
                intPos2 = strHtml.IndexOf("~", intPos1 + 5)
                Dim strInclFile As String = strHtml.Substring(intPos1 + 5, intPos2 - intPos1 - 5)
                Dim strInclHtml As String
                Try
                    strInclHtml = ReadFile(Path.Combine(WebPagesRoot, strInclFile))
                Catch ex As Exception
                    strInclHtml = "***Cannot read " & Path.Combine(WebPagesRoot, strInclFile) & "***"
                End Try
                strHtml = strHtml.Replace("~inc:" & strInclFile & "~", strInclHtml)
            Loop
            Try
                For Each strRegEx As String In New String() {"(?m)\s*(?i)href\s*=\s*(""([^""]*)""|'([^']*'))", _
                                                             "(?m)\s*(?i)\s*src\s*=\s*(""([^""]*)""|'([^']*'))"}
                    Dim matches As MatchCollection = Regex.Matches(strHtml, strRegEx)
                    For Each m As Match In matches
                        'For Each g As Group In m.Groups
                        '    Debug.Print(g.Value)
                        'Next
                        'For Each c As Capture In m.Captures
                        'Console.WriteLine("Index={0}, Value={1}", c.Index, c.Value)
                        Dim strHref As String = m.Groups(2).Value
                        Dim strHref2 As String = strHref
                        strHref2 = Path.Combine(WebPagesRoot, strHref)
                        strHtml = strHtml.Replace(strHref, strHref2)
                        'Next
                    Next
                Next
            Catch ex As Exception
                MessageBox.Show(ex.Message, "HTMLEditor.Preview")
            End Try
            Me.WebBrowser2.DocumentText = strHtml.Replace(" href=""/", " href=""" & WebPagesRoot & "\") _
                .Replace(" src=""/", " src=""" & WebPagesRoot & "\")
        End Sub

        Private Sub PreviewTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles PreviewTimer.Tick
            PreviewTimer.Enabled = False
            If chkPreview.Checked Then
                Select Case Path.GetExtension(EditedFileName)
                    Case ".js", ".css", ".xml"
                        Me.WebBrowser2.DocumentText = String.Empty
                    Case Else
                        PreView()
                End Select
            End If
        End Sub

        Private Sub HTMLEditor_TextChanged()
            PreviewTimer.Enabled = False
            PreviewTimer.Interval = 1000
            PreviewTimer.Enabled = True
            SaveToolStripButton.Enabled = True
        End Sub

        Private Sub WebBrowser1_DocumentCompleted(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs)
            PreView()
        End Sub

        Private Sub WebBrowser2_Navigating(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles WebBrowser2.Navigating
            'Dim wb As WebBrowser = sender
            'Dim ScriptElements As HtmlElementCollection = wb.Document.GetElementsByTagName("script")
            'If ScriptElements.Count > 0 Then
            '    For Each scriptElement As HtmlElement In ScriptElements
            '        scriptElement.OuterHtml = ""
            '    Next
            'End If
        End Sub

        Private Sub SaveToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripButton.Click
            Dim oEditor As SourceEditor.Editor = CustomTabControl1.SelectedTab.Controls(0)
            oEditor.FileSave()
            SaveToolStripButton.Enabled = False
        End Sub

        Private Sub btnGenerate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerate.Click
            Dim strDestWebagesPath As String = Path.GetFullPath(Path.Combine(Common.VGDDProjectPath, Common.ProjectHtmlWebPagesFolder))
            If File.Exists(Path.Combine(strDestWebagesPath, "FileRcrd.bin")) Then File.Delete(Path.Combine(strDestWebagesPath, "FileRcrd.bin"))
            If File.Exists(Path.Combine(strDestWebagesPath, "DynRcrd.bin")) Then File.Delete(Path.Combine(strDestWebagesPath, "DynRcrd.bin"))

            Dim strCommand As String = ""
            Dim strOutFileName As String = ""
            Select Case cmbType.Text.Replace(" ", "")
                Case "Bin"
                    strCommand &= "/b"
                    strOutFileName = "WebPages.bin"
                Case "Files(MDD)"
                    strCommand &= "/b"
                    strOutFileName = "WebPages.bin"
                Case "Cmodule"
                    strCommand &= "/c"
                    strOutFileName = "MPFSImg2.c"
                Case Else
                    Exit Sub
            End Select
            If chkForce.Checked AndAlso File.Exists(Path.Combine(Common.VGDDProjectPath, "HTTPPrint.idx")) Then
                Try
                    File.Delete(Path.Combine(Common.VGDDProjectPath, "HTTPPrint.idx"))
                Catch ex As Exception
                    MessageBox.Show("Cannot delete HTTPPrint.idx: " & ex.Message, "Warning")
                End Try
            End If
            Try
                Dim oBuilder As New Microchip.MPFS2Builder(Common.CodeGenDestPath, strOutFileName)
                oBuilder.DynamicTypes = "*.htm, *.html, *.cgi, *.xml"
                oBuilder.NonGZipTypes = "*.inc, snmp.bib"
                oBuilder.AddDirectory(strDestWebagesPath, "")
                Dim generationResult As Boolean
                Dim myLog As New List(Of String)
                Select Case cmbType.Text.Replace(" ", "")
                    Case "Bin"
                        generationResult = oBuilder.Generate(Microchip.MPFSOutputFormat.BIN)
                        myLog = oBuilder.Log
                    Case "Files(MDD)"
                        generationResult = oBuilder.Generate(Microchip.MPFSOutputFormat.MDD)
                        myLog = oBuilder.Log
                        File.Copy(Path.Combine(Common.CodeGenDestPath, "FileRcrd.bin"), Path.Combine(strDestWebagesPath, "FileRcrd.bin"), True)
                        File.Copy(Path.Combine(Common.CodeGenDestPath, "DynRcrd.bin"), Path.Combine(strDestWebagesPath, "DynRcrd.bin"), True)
                        myLog.Add(" FileRcrd.bin: NOLOG" & vbCrLf)
                        myLog.Add(" DynRcrd.bin: NOLOG" & vbCrLf)
                    Case "Cmodule"
                        generationResult = oBuilder.Generate(Microchip.MPFSOutputFormat.C32)
                        myLog = oBuilder.Log
                End Select
                Dim DynVars As ArrayList = oBuilder.VarList
                Dim strOut As String = String.Empty
                ListView1.Items.Clear()
                Dim intChecks As Integer
                For Each s As String In myLog
                    If generationResult AndAlso s.StartsWith(" ") Then
                        Dim strFile As String = Path.GetFullPath(Path.Combine(strDestWebagesPath, s.Split(":")(0).Trim))
                        Dim oFi As New FileInfo(strFile)
                        Dim strDir As String = strDestWebagesPath
                        strDir = "/" & Path.Combine(Path.GetFileName(Common.ProjectHtmlWebPagesFolder), RelativePath.Evaluate(strDir, Path.GetDirectoryName(strFile))).Replace("\", "/")
                        'strDir= Path.Combine(Common.ProjectHtmlWebPagesFolder, )
                        Dim oItem As New ListViewItem(Path.Combine(strDir, Path.GetFileName(strFile)).Replace("\", "/"))
                        oItem.Checked = False
                        oItem.SubItems.Add(oFi.LastWriteTime.ToString("yyyy/MM/dd hh:mm:ss"))
                        oItem.SubItems.Add(String.Empty)
                        oItem.Tag = strFile
                        ListView1.Items.Add(oItem)
                        If (oFi.Attributes And FileAttributes.Archive) = FileAttributes.Archive Then
                            oItem.Checked = True
                            intChecks += 1
                            Try
                                File.SetAttributes(strFile, oFi.Attributes - FileAttributes.Archive)
                                oFi.Refresh()
                            Catch ex As Exception
                            End Try
                        End If
                    End If
                    If Not s.Contains("NOLOG") Then strOut &= s & vbCrLf
                Next
                If generationResult Then
                    Select Case cmbType.Text.Replace(" ", "")
                        Case "Bin"
                        Case "Files(MDD)"
                            SortListViewByDateTime()
                            pnlUpload.Visible = True
                        Case "Cmodule"
                            strOut &= Path.Combine(Common.CodeGenDestPath, "MPFSImg2.c") & " generated!"
                    End Select
                    If Not oBuilder.IndexUpdated Then
                        MessageBox.Show(strOut, "Generation successful", MessageBoxButtons.OK)
                    Else
                        MessageBox.Show(strOut & vbCrLf & "The dynamic variables in your web pages have changed!" & vbCrLf & _
                                        "Remember to recompile your MPLAB project before continuing" & vbCrLf & _
                                        "to ensure that the project is in sync.", _
                                        "MPFS2 Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    MessageBox.Show(strOut, "Errors during MPFS2 run")
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "MPFS2Builder")
            End Try
            btnUpload.Text = "Upload"
            btnUpload.Enabled = True
        End Sub

        Private Sub SortListViewByDateTime()
            Dim itemsMoved As Boolean
            Dim relocate As New ArrayList
            Dim dData1 As Date, dData2 As Date
            Do
                itemsMoved = False
                For index As Integer = 0 To (Me.ListView1.Items.Count - 2)
                    Dim oItem1 As ListViewItem = Me.ListView1.Items.Item(index)
                    Dim oItem2 As ListViewItem = Me.ListView1.Items.Item(index + 1)
                    If Date.TryParse(oItem1.SubItems(1).Text, dData1) AndAlso Date.TryParse(oItem2.SubItems(1).Text, dData2) _
                          AndAlso dData1 < dData2 Then
                        relocate.Clear()
                        'relocate.Add(oItem1.Text)
                        relocate.Add(oItem1.Checked)
                        relocate.Add(oItem1.Tag)
                        For Each oSubItem As ListViewItem.ListViewSubItem In oItem1.SubItems
                            relocate.Add(oSubItem.Text)
                        Next
                        oItem1.Remove()
                        Dim newItem As ListViewItem = ListView1.Items.Add(relocate(2))
                        newItem.Checked = Boolean.Parse(relocate(0))
                        newItem.Tag = relocate(1)
                        For i As Integer = 3 To relocate.Count - 1
                            newItem.SubItems.Add(relocate(i))
                        Next
                        itemsMoved = True
                    End If
                Next index
            Loop Until itemsMoved = False
        End Sub

        Private Sub chkPreview_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPreview.CheckedChanged
            If chkPreview.Checked Then
                PreviewTimer.Interval = 10
                PreviewTimer.Enabled = True
            Else
                Me.WebBrowser2.DocumentText = ""
            End If
        End Sub

        Private Sub btnChooseFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChooseFolder.Click
            Dim oFolderDialog As New FolderBrowserDialog()
            Dim strDir As String = String.Empty
            If Common.VGDDProjectPath <> String.Empty Then
                strDir = Path.GetFullPath((Path.Combine(Common.VGDDProjectPath, Common.ProjectHtmlWebPagesFolder)))
            End If
            If Not Directory.Exists(strDir) Then
                Common.ProjectHtmlWebPagesFolder = Common.VGDDProjectPath
                strDir = Common.ProjectHtmlWebPagesFolder
            End If
            oFolderDialog.SelectedPath = strDir
            SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
            If oFolderDialog.ShowDialog() = DialogResult.OK Then
                Common.ProjectChanged = True
                Common.ProjectHtmlWebPagesFolder = RelativePath.Evaluate(Common.VGDDProjectPath, oFolderDialog.SelectedPath)
                SetHtmlEnviron()
            End If
        End Sub

        Private Sub lblWebPagesFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblWebPagesFolder.Click
            Try
                Process.Start(Path.Combine(Common.VGDDProjectPath, Common.ProjectHtmlWebPagesFolder))
            Catch ex As Exception
                MessageBox.Show("Error opening folder " & lblWebPagesFolder.Text & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub CustomTabControl1_TabClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles CustomTabControl1.TabClosing
            Dim oEditor As SourceEditor.Editor = CustomTabControl1.SelectedTab.Controls(0)
            If oEditor.IsModified Then
                Dim r = MessageBox.Show(String.Format("Save changes to {0}?", If(oEditor.ActiveEditor.FileName, "new file")), "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                If r = DialogResult.Cancel Then
                    e.Cancel = True
                ElseIf r = DialogResult.Yes Then
                    If Not oEditor.FileSave() Then
                        e.Cancel = True
                    End If
                End If
            End If
        End Sub

        Private Sub CustomTabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomTabControl1.SelectedIndexChanged
            If CustomTabControl1.SelectedTab IsNot Nothing Then
                Dim oEditor As SourceEditor.Editor = CustomTabControl1.SelectedTab.Controls(0)
                Me.SaveToolStripButton.Enabled = oEditor.IsModified
                EditedFileName = CustomTabControl1.SelectedTab.Tag
                PreviewTimer.Interval = 10
                PreviewTimer.Enabled = True
            Else
                SaveToolStripButton.Enabled = False
            End If
        End Sub

        Private Sub btnUpload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpload.Click
            'Dim headers As New Specialized.NameValueCollection()
            'headers.Add("Cookie", "name=value;")
            'headers.Add("Referer", "http://google.com")
            If btnUpload.Text = "Upload" Then
                btnUpload.Text = "Stop"
                For Each oItem As ListViewItem In ListView1.Items
                    oItem.SubItems(2).Text = ""
                    oItem.ForeColor = Color.Black
                Next
                Application.DoEvents()
                For Each oItem As ListViewItem In ListView1.Items
                    If oItem.Checked Then
                        oItem.SubItems(2).Text = "Uploading..."
                        oItem.EnsureVisible()
                        Application.DoEvents()
                        Dim strFile As String = oItem.Text
                        Dim strDir As String = Path.GetDirectoryName(strFile) '"/" & Path.Combine(Common.ProjectHtmlWebPagesFolder, RelativePath.Evaluate(Path.Combine(Common.ProjectPath, Common.ProjectHtmlWebPagesFolder), Path.GetDirectoryName(strFile)))
                        strFile = Path.Combine(Path.Combine(Common.VGDDProjectPath, Common.ProjectHtmlWebPagesFolder), strFile)
                        Dim nvc As New Specialized.NameValueCollection()
                        nvc.Add("upddir", strDir.Replace("\", "/"))
                        Dim strResult As String = HttpUploadFile(Common.ProjectHtmlTargetUrl, oItem.Tag, "updfile", "application/octet-stream", nvc, txtUsername.Text, txtPassword.Text) ', headers)
                        oItem.SubItems(2).Text = strResult
                        If strResult = "OK" Then
                            oItem.Checked = False
                            oItem.ForeColor = Color.Green
                        Else
                            oItem.ForeColor = Color.Red
                        End If
                        Application.DoEvents()
                        Threading.Thread.Sleep(500)
                    End If
                    If Not btnUpload.Enabled Then Exit For
                Next
                btnUpload.Text = "Upload"
                btnUpload.Enabled = True
            Else
                btnUpload.Enabled = False
            End If
        End Sub

        Private Sub cmbType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbType.SelectedIndexChanged
            pnlUpload.Visible = False
            btnGenerate.Enabled = cmbType.Text <> String.Empty
        End Sub

        Private Function HttpUploadFile( _
            ByVal uri As String, _
            ByVal filePath As String, _
            ByVal fileParameterName As String, _
            ByVal contentType As String, _
            ByVal otherParameters As Specialized.NameValueCollection, ByVal strUserName As String, ByVal strPassword As String) As String

            Dim boundary As String = "---------------------------" & DateTime.Now.Ticks.ToString("x")
            Dim newLine As String = System.Environment.NewLine
            Dim boundaryBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(newLine & "--" & boundary & newLine)
            Dim request As Net.HttpWebRequest = Net.WebRequest.Create(uri)

            request.ContentType = "multipart/form-data; boundary=" & boundary
            request.Method = "POST"
            request.KeepAlive = False
            request.Timeout = 10000
            request.SendChunked = True
            'request.ContentLength = request.ContentType.Length + otherParameters.Keys.Count * (boundaryBytes.Length) + FileLen(filePath)
            If strUserName <> String.Empty Then
                request.Credentials = New Net.NetworkCredential(strUserName, strPassword)
                '                Net.CredentialCache.DefaultCredentials
            End If
            'Dim oTs As New FileStream("c:\temp\upload.txt", FileMode.Create)
            Try
                Using requestStream As IO.Stream = request.GetRequestStream()
                    Dim formDataTemplate As String = "Content-Disposition: form-data; name=""{0}""{1}{1}{2}"
                    For Each key As String In otherParameters.Keys
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                        'oTs.Write(boundaryBytes, 0, boundaryBytes.Length)
                        Dim formItem As String = String.Format(formDataTemplate, key, newLine, otherParameters(key))
                        Dim formItemBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(formItem)
                        requestStream.Write(formItemBytes, 0, formItemBytes.Length)
                        'oTs.Write(formItemBytes, 0, formItemBytes.Length)
                    Next (key)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                    'oTs.Write(boundaryBytes, 0, boundaryBytes.Length)
                    Dim headerTemplate As String = "Content-Disposition: form-data; name=""{0}""; filename=""{1}""{2}Content-Type: {3}{2}{2}"
                    Dim header As String = String.Format(headerTemplate, fileParameterName, Path.GetFileName(filePath), newLine, contentType)
                    Dim headerBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(header)
                    requestStream.Write(headerBytes, 0, headerBytes.Length)
                    'oTs.Write(headerBytes, 0, headerBytes.Length)

                    Using fileStream As New IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read)
                        Dim buffer(4096) As Byte
                        Dim bytesRead As Int32 = fileStream.Read(buffer, 0, buffer.Length)
                        Do While (bytesRead > 0)
                            requestStream.Write(buffer, 0, bytesRead)
                            'oTs.Write(buffer, 0, bytesRead)
                            bytesRead = fileStream.Read(buffer, 0, buffer.Length)
                        Loop
                    End Using
                    Dim trailer As Byte() = System.Text.Encoding.ASCII.GetBytes(newLine & "--" + boundary + "--" & newLine)
                    requestStream.Write(trailer, 0, trailer.Length)
                    'oTs.Write(trailer, 0, trailer.Length)
                    requestStream.Flush()
                    'oTs.Flush()
                    requestStream.Close()
                    'oTs.Close()
                End Using
                Dim response As Net.WebResponse = Nothing
                Try
                    response = request.GetResponse()
                    Using responseStream As IO.Stream = response.GetResponseStream()
                        Using responseReader As New IO.StreamReader(responseStream)
                            Dim responseText = responseReader.ReadToEnd()
                            Diagnostics.Debug.Write(responseText)
                        End Using
                    End Using
                    HttpUploadFile = CType(response, Net.HttpWebResponse).StatusDescription
                Catch exception As Net.WebException
                    response = exception.Response
                    HttpUploadFile = exception.Message
                    If (response IsNot Nothing) Then
                        Using reader As New IO.StreamReader(response.GetResponseStream())
                            Dim responseText = reader.ReadToEnd()
                            HttpUploadFile &= responseText
                            'Diagnostics.Debug.Write(responseText)
                        End Using
                        response.Close()
                    End If
                Finally
                    request = Nothing
                    'oTs = Nothing
                End Try

            Catch ex As Exception
                HttpUploadFile = ex.Message
            End Try

        End Function

        Private Sub lblWebPagesFolder_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblWebPagesFolder.TextChanged
            If Common.ProjectHtmlWebPagesFolder <> String.Empty Then
                pnlGenerate.Visible = True
            End If
        End Sub

        Private Sub btnSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSettings.Click
            WebBrowser2.Visible = Not WebBrowser2.Visible
            pnlSettings.Visible = Not pnlSettings.Visible
        End Sub

        Private Sub chkAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAll.CheckedChanged
            For Each oItem As ListViewItem In ListView1.Items
                oItem.Checked = chkAll.Checked
            Next
            If chkAll.Checked Then
                chkAll.Text = "Uncheck all"
            Else
                chkAll.Text = "Check all"
            End If
        End Sub

        Private Sub txtTargetUrl_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTargetUrl.TextChanged, txtUsername.TextChanged, txtPassword.TextChanged
            If txtTargetUrl.Text <> String.Empty Then
                Common.ProjectHtmlTargetUrl = txtTargetUrl.Text
                If txtUsername.Text <> String.Empty Then Common.ProjectHtmlTargetUser = txtUsername.Text
                If txtPassword.Text <> String.Empty Then Common.ProjectHtmlTargetPassword = txtPassword.Text
                Common.ProjectChanged = True
            End If
        End Sub
    End Class
End Namespace
