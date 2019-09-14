Imports System.Reflection
Imports System.Xml
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Net
Imports VirtualFabUtils.Utils

Public Class frmMhcPatcher

    Const VERSION As String = "0.2 Beta"

    Enum TVIMG
        IMG_OK = 0
        IMG_NOT_OK = 1
        IMG_ERROR = 2
    End Enum

    Private myAssembly As Assembly
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        myAssembly = System.Reflection.Assembly.GetExecutingAssembly()
        Try
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf MyAssemblyResolveEventHandler
        Catch ex As Exception

        End Try

    End Sub

    Private DownloadPath As String
    Private MainRepositoryPath As String
    Private HarmonyVersion As String
    Private HarmonyPath As String

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'HarmonyPath = "D:\pic\Software\Pic\harmony\v1_08org"
        'HarmonyPath = "D:\pic\Software\Pic\harmony\latest"
        Dim strNameSpace As String = Assembly.GetExecutingAssembly().ToString.Split(",")(0).Trim
        DownloadPath = Path.Combine(Path.GetDirectoryName(Application.CommonAppDataPath), "Repositories")
        If Not Directory.Exists(DownloadPath) Then Directory.CreateDirectory(DownloadPath)
        MainRepositoryPath = Path.Combine(DownloadPath, "MainMhcPatcherRepository")
        If Not Directory.Exists(MainRepositoryPath) Then
            If Not ExtractZippedResource(strNameSpace & ".MhcPatcherPatches.zip", DownloadPath, System.Reflection.Assembly.GetExecutingAssembly(), Nothing) Then
                MessageBox.Show("Could not extract MhcPatcherPatches.zip to " & DownloadPath)
                Exit Sub
            End If
        End If
        RefreshRepository()

        HarmonyPath = My.Settings.LatestHarmonyFolder
        ReadHarmonyVersion()
        drpRepository_SelectedIndexChanged(Nothing, Nothing)
        Me.Text &= " version " & VERSION
        If Not My.Settings.EulaShown Then
            If Eula.ShowDialog() = Windows.Forms.DialogResult.No Then
                End
                Exit Sub
            End If
            My.Settings.EulaShown = True
            My.Settings.Save()
        End If
    End Sub

    Public Sub RefreshRepository()
        drpRepository.Items.Clear()
        drpRepository.Items.Add(String.Empty)
        For Each strDir As String In Directory.GetDirectories(DownloadPath)
            If strDir <> "main" Then
                drpRepository.Items.Add(Path.GetFileName(strDir))
            End If
        Next
    End Sub

    Private Sub drpRepository_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpRepository.SelectedIndexChanged
        pnlHarmony.Visible = (drpRepository.SelectedItem <> String.Empty) AndAlso HarmonyPath <> String.Empty AndAlso Directory.Exists(HarmonyPath)
        If pnlHarmony.Visible Then
            btnCheckPatches.PerformClick()
        End If
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        Try
            Me.Cursor = Cursors.WaitCursor
            If Directory.Exists(MainRepositoryPath) Then
                Directory.Delete(MainRepositoryPath, True)
            End If
            If Not Directory.Exists(MainRepositoryPath) Then
                Directory.CreateDirectory(MainRepositoryPath)
            End If
            Dim strNewZipFile As String = Path.Combine(DownloadPath, "MhcPatcherPatches.zip")
            Using client = New WebClient()
                'client.Credentials = New NetworkCredential(username, password)
                client.DownloadFile("http://www.virtualfab.it/MhcPatcher.php", strNewZipFile)
            End Using
            If ZipExtract(strNewZipFile, DownloadPath, "") Then
                RefreshRepository()
                drpRepository.SelectedItem = ""
            Else
                Me.Cursor = Cursors.Default
                MessageBox.Show("Cannot extract downloaded Zip to " & DownloadPath)
            End If
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            Dim strMsg As String = ex.Message
            If ex.InnerException IsNot Nothing Then
                strMsg &= ": " & ex.InnerException.Message
            End If
            MessageBox.Show("Error updating patches from the internet:" & strMsg)
        End Try
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub btnOpenDownloadFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenDownloadFolder.Click
        Process.Start(DownloadPath)
    End Sub

    Private Sub lblMalPath_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblMalPath.Click
        Process.Start(HarmonyPath)
    End Sub

    Private Sub btnSelectHarmonyPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectHarmonyPath.Click
        Dim folderDialog As New FolderBrowserDialog
        folderDialog.ShowNewFolderButton = False
        folderDialog.Description = "Select Harmony folder"
        If HarmonyPath <> String.Empty Then
            folderDialog.SelectedPath = HarmonyPath
        ElseIf My.Settings.LatestHarmonyFolder <> String.Empty Then
            folderDialog.SelectedPath = My.Settings.LatestHarmonyFolder
        End If
        SendKeys.Send("{TAB}{TAB}{RIGHT}")
        If folderDialog.ShowDialog() = DialogResult.OK Then
            HarmonyVersion = String.Empty
            HarmonyPath = folderDialog.SelectedPath
            ReadHarmonyVersion()
            If HarmonyVersion <> String.Empty Then
                My.Settings.LatestHarmonyFolder = HarmonyPath
                My.Settings.Save()
                drpRepository_SelectedIndexChanged(Nothing, Nothing)
            Else
                HarmonyPath = My.Settings.LatestHarmonyFolder
                MessageBox.Show("Selected path does not contain a valid Harmony framework")
            End If
        End If
    End Sub

    Private Sub ReadHarmonyVersion()
        Dim strHarmonyVersionFile As String = Path.Combine(Path.Combine(HarmonyPath, "config"), "harmony.hconfig")
        If File.Exists(strHarmonyVersionFile) Then
            HarmonyVersion = ReadFile(strHarmonyVersionFile)
            HarmonyVersion = HarmonyVersion.Substring(HarmonyVersion.IndexOf("default """) + 9)
            HarmonyVersion = HarmonyVersion.Substring(0, HarmonyVersion.IndexOf(""""))
            lblHarmonyVersion.Text = String.Format("Selected Harmony path (version {0}):", HarmonyVersion)
            lblMalPath.Text = HarmonyPath
        End If
    End Sub

    Private Sub btnCheckPatches_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCheckPatches.Click
        CheckApplyPatches(False, False)
    End Sub

    Private Sub btnApplyPatches_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyPatches.Click
        CheckApplyPatches(True, False)
    End Sub

    Private Sub btnRestoreOriginals_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRestoreOriginals.Click
        CheckApplyPatches(False, True)
    End Sub

    Private Sub CheckApplyPatches(ByVal blnApplyPatches As Boolean, ByVal blnRestoreOriginals As Boolean)
        TreeView1.Nodes.Clear()
        TreeView1.ImageList = ImageList1
        For Each strPatchFile As String In Directory.GetFiles(Path.Combine(DownloadPath, drpRepository.SelectedItem), "*.mhcp", SearchOption.AllDirectories)
            Application.DoEvents()
            Dim strPatchXml As String = ReadFile(strPatchFile)
            Dim oPatchXmlDoc As New XmlDocument
            Try
                oPatchXmlDoc.LoadXml(strPatchXml)
                If oPatchXmlDoc.SelectSingleNode("/MHCPatcher") Is Nothing Then Continue For
                Dim oTvNodePatchFile As New TreeNode(Path.GetFileName(strPatchFile) & " - " &
                                                     oPatchXmlDoc.SelectSingleNode("/MHCPatcher/Description").InnerText.Trim)
                oTvNodePatchFile.NodeFont = New Font(TreeView1.Font.FontFamily, TreeView1.Font.Size * 1.2)
                oTvNodePatchFile.Text = oTvNodePatchFile.Text
                If oPatchXmlDoc.SelectSingleNode("/MHCPatcher/Notes") IsNot Nothing Then
                    Dim strNotes As String = oPatchXmlDoc.SelectSingleNode("/MHCPatcher/Notes").InnerText.Trim
                    If oTvNodePatchFile.ToolTipText.StartsWith(vbCrLf) Then
                        oTvNodePatchFile.ToolTipText = oTvNodePatchFile.ToolTipText.Substring(2).Trim
                    End If
                    oTvNodePatchFile.ToolTipText = String.Empty
                    For Each strLine In strNotes.Split(vbLf)
                        oTvNodePatchFile.ToolTipText &= strLine.Replace(vbCr, "").Trim & vbCrLf
                    Next
                End If
                For Each oTargetNode As XmlNode In oPatchXmlDoc.SelectNodes("/MHCPatcher/Target")
                    Dim strTargetFile As String = Path.Combine(HarmonyPath, oTargetNode.Attributes("file").Value)
                    If blnRestoreOriginals Then
                        If File.Exists(strTargetFile & ".org") Then
                            File.Delete(strTargetFile)
                            FileSystem.Rename(strTargetFile & ".org", strTargetFile)
                        End If
                    End If
                    Dim oTvTargetNode As New TreeNode("Target: " & strTargetFile)
                    oTvNodePatchFile.Nodes.Add(oTvTargetNode)
                    Dim strFileContent As String = String.Empty
                    If File.Exists(strTargetFile) Then
                        strFileContent = ReadFile(strTargetFile).Replace(vbCrLf, vbLf)
                    Else
                        oTvTargetNode.ImageIndex = TVIMG.IMG_NOT_OK
                        oTvTargetNode.SelectedImageIndex = TVIMG.IMG_NOT_OK
                        oTvTargetNode.Parent.ImageIndex = TVIMG.IMG_NOT_OK
                        oTvTargetNode.Parent.SelectedImageIndex = TVIMG.IMG_NOT_OK
                    End If
                    Dim strFileContentOriginal As String = strFileContent

                    For Each oPatchNode As XmlNode In oTargetNode.SelectNodes("Patch")
                        Dim strPatchName As String = String.Empty
                        Dim strPatchSearchRegEx As String = String.Empty
                        Dim strPatchBlock As String = String.Empty
                        Dim strPatch As String = oPatchNode.InnerText.Replace(vbCrLf, vbLf)
                        If strPatch.StartsWith(vbLf) Then
                            strPatch = strPatch.Substring(1)
                        End If
                        Dim strPatchCheck As String = strPatch
                        If oPatchNode.Attributes("SearchRegEx") IsNot Nothing Then
                            strPatchSearchRegEx = oPatchNode.Attributes("SearchRegEx").Value
                        End If
                        If oPatchNode.Attributes("Block") IsNot Nothing Then
                            strPatchBlock = oPatchNode.Attributes("Block").Value
                            strPatchName = oPatchNode.Attributes("Block").Value
                        End If
                        If oPatchNode.Attributes("Check") IsNot Nothing Then
                            strPatchName = oPatchNode.Attributes("Check").Value
                            strPatchCheck = oPatchNode.Attributes("Check").Value
                        End If
                        Dim oTvPatchNode As New TreeNode("Patch " & strPatchName)
                        oTvTargetNode.Nodes.Add(oTvPatchNode)
                        With oTvPatchNode
                            .ImageIndex = TVIMG.IMG_NOT_OK
                            .SelectedImageIndex = TVIMG.IMG_NOT_OK
                            If strFileContent <> String.Empty Then
                                If strPatchCheck <> String.Empty Then
                                    If strFileContent.Contains(strPatchCheck.Replace("\n", vbLf)) Then
                                        .ImageIndex = TVIMG.IMG_OK
                                        .SelectedImageIndex = TVIMG.IMG_OK
                                    End If
                                ElseIf strFileContent.Contains(strPatch) Then
                                    .ImageIndex = TVIMG.IMG_OK
                                    .SelectedImageIndex = TVIMG.IMG_OK
                                End If
                            End If
                            If .ImageIndex = TVIMG.IMG_NOT_OK Then
                                .EnsureVisible()
                                If blnApplyPatches And strFileContent <> String.Empty Then
                                    Try
                                        strFileContent = ApplyPatch(oPatchNode, strFileContent)
                                        .ImageIndex = TVIMG.IMG_OK
                                        .SelectedImageIndex = TVIMG.IMG_OK
                                    Catch ex As Exception
                                        AddErrorNode(oTvPatchNode, ex.Message)
                                    End Try
                                Else
                                    .Parent.ImageIndex = TVIMG.IMG_NOT_OK
                                    .Parent.SelectedImageIndex = TVIMG.IMG_NOT_OK
                                    .Parent.Parent.ImageIndex = TVIMG.IMG_NOT_OK
                                    .Parent.Parent.SelectedImageIndex = TVIMG.IMG_NOT_OK
                                    .Parent.Expand()
                                    .Parent.Parent.Expand()
                                    .EnsureVisible()
                                End If
                            End If
                        End With
                    Next
                    If strFileContent <> strFileContentOriginal Then
                        If Not strTargetFile.Contains("MhcPatcher") AndAlso Not File.Exists(strTargetFile & ".org") Then
                            File.Copy(strTargetFile, strTargetFile & ".org")
                        End If
                        WriteFile(strTargetFile, strFileContent.Replace(vbLf, vbCrLf))
                    End If
                    'oTvNodePatchFile.ExpandAll()
                Next

                For Each oAddFileNode As XmlNode In oPatchXmlDoc.SelectNodes("/MHCPatcher/AddFile")
                    Dim strTargetFile As String = oAddFileNode.Attributes("Target").Value
                    If Path.IsPathRooted(strTargetFile) Then
                        strTargetFile = strTargetFile.TrimStart(Path.DirectorySeparatorChar)
                        strTargetFile = strTargetFile.TrimStart(Path.AltDirectorySeparatorChar)
                    End If
                    strTargetFile = Path.Combine(HarmonyPath, strTargetFile)
                    Dim oTvAddFileNode As New TreeNode("AddFile " & oAddFileNode.Attributes("Target").Value)
                    oTvNodePatchFile.Nodes.Add(oTvAddFileNode)
                    With oTvAddFileNode
                        If File.Exists(strTargetFile) Then
                            If blnRestoreOriginals Then
                                File.Delete(strTargetFile)
                                .ImageIndex = TVIMG.IMG_NOT_OK
                                .SelectedImageIndex = TVIMG.IMG_NOT_OK
                                .Parent.ImageIndex = TVIMG.IMG_NOT_OK
                                .Parent.SelectedImageIndex = TVIMG.IMG_NOT_OK
                            Else
                                .ImageIndex = TVIMG.IMG_OK
                                .SelectedImageIndex = TVIMG.IMG_OK
                            End If
                        Else
                            If blnApplyPatches Then
                                Try
                                    Dim strSourceFile As String = oAddFileNode.Attributes("Source").Value
                                    Dim strDestFile As String = oAddFileNode.Attributes("Target").Value
                                    If Path.IsPathRooted(strDestFile) Then
                                        strDestFile = strDestFile.TrimStart(Path.DirectorySeparatorChar)
                                        strDestFile = strDestFile.TrimStart(Path.AltDirectorySeparatorChar)
                                    End If
                                    strDestFile = Path.Combine(HarmonyPath, strDestFile)
                                    Directory.CreateDirectory(Path.GetDirectoryName(strDestFile))
                                    Try
                                        File.Copy(Path.Combine(Path.GetDirectoryName(strPatchFile), strSourceFile), strDestFile)
                                        .ImageIndex = TVIMG.IMG_OK
                                        .SelectedImageIndex = TVIMG.IMG_OK

                                    Catch ex1 As Exception
                                        AddErrorNode(oTvAddFileNode, ex1.Message)

                                    End Try
                                Catch ex2 As Exception
                                    AddErrorNode(oTvAddFileNode, ex2.Message)
                                End Try
                            Else
                                .ImageIndex = TVIMG.IMG_NOT_OK
                                .SelectedImageIndex = TVIMG.IMG_NOT_OK
                                .Parent.ImageIndex = TVIMG.IMG_NOT_OK
                                .Parent.SelectedImageIndex = TVIMG.IMG_NOT_OK
                                .Parent.Expand()
                                .EnsureVisible()
                            End If
                        End If
                    End With
                Next
                TreeView1.Nodes.Add(oTvNodePatchFile)
                oTvNodePatchFile.Text = oTvNodePatchFile.Text

            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub AddErrorNode(ByRef oTvNode As TreeNode, ByVal message As String)
        Dim oTvNodeError As TreeNode = oTvNode.Nodes.Add(message)
        With oTvNodeError
            .ImageIndex = TVIMG.IMG_ERROR
            .Parent.Expand()
            .Parent.Parent.Expand()
            If .Parent.Parent.Parent IsNot Nothing Then
                .Parent.Parent.Parent.Expand()
            End If
            .SelectedImageIndex = TVIMG.IMG_ERROR
            .EnsureVisible()
        End With
        oTvNode.ImageIndex = TVIMG.IMG_NOT_OK
        oTvNode.SelectedImageIndex = TVIMG.IMG_NOT_OK
        oTvNode.Parent.ImageIndex = TVIMG.IMG_NOT_OK
        oTvNode.Parent.SelectedImageIndex = TVIMG.IMG_NOT_OK
    End Sub

    Public Function ApplyPatch(ByVal oPatchNode As XmlNode, ByVal strFileContent As String) As String
        Dim intInsertPoint As Integer = -1
        Dim intMatchRegexStartAt As Integer = 0
        Dim matchBlock As Match = Nothing
        Dim intRegExIndex = 0
        Dim intRegExLength = 0
        If oPatchNode.Attributes("Action") Is Nothing Then
            Throw New Exception("Malformed patch: please specify Action=""{ Append | InsertBefore | InsertAfter }""")
        End If
        If oPatchNode.Attributes("Block") IsNot Nothing Then
            Dim strBlock As String = oPatchNode.Attributes("Block").Value
            Dim strBlockRegEx As String = ".*" & strBlock & "((?:\n.+)+)"
            Dim regex As Regex = New Regex(strBlockRegEx, RegexOptions.Multiline)
            matchBlock = regex.Match(strFileContent)
            If matchBlock.Success Then
                intMatchRegexStartAt = matchBlock.Index
            Else
                Throw New Exception("Block """ & strBlockRegEx & """ not found")
            End If
        End If
        If oPatchNode.Attributes("SearchRegEx") IsNot Nothing Then
            Dim strRegEx As String = oPatchNode.Attributes("SearchRegEx").Value
            Dim regex As Regex = New Regex(strRegEx)
            Dim match As Match = regex.Match(strFileContent, intMatchRegexStartAt)
            If match.Success Then
                intRegExIndex = match.Index
                intRegExLength = match.Length
            Else
                Throw New Exception("SearchRegEx """ & strRegEx & """ not found")
            End If
        End If
        Dim strPatch As String = oPatchNode.InnerText.Replace(vbCrLf, vbLf)
        If strPatch.StartsWith(vbLf) Then
            strPatch = strPatch.Substring(1)
        End If
        Select Case oPatchNode.Attributes("Action").Value.ToUpper
            Case "APPEND"
                If matchBlock Is Nothing Then
                    Throw New Exception("action=""Append"" can only be used in conjunction with Block=""<search expression>""")
                End If
                If intRegExIndex = 0 Then
                    intInsertPoint = matchBlock.Index + matchBlock.Length
                Else
                    intInsertPoint = intRegExIndex + intRegExLength
                End If
                strPatch = vbLf & strPatch
            Case "INSERTBEFORE"
                intInsertPoint = intRegExIndex
            Case "INSERTAFTER"
                intInsertPoint = intRegExIndex + intRegExLength
            Case "REPLACE"
                intInsertPoint = intRegExIndex
                strFileContent = strFileContent.Substring(0, intInsertPoint) & strFileContent.Substring(intInsertPoint + intRegExLength)
            Case Else
                Throw New Exception(String.Format("action=""{0}"" unknown", oPatchNode.Attributes("Action").Value))
        End Select
        If intInsertPoint = -1 Then
            Throw New Exception("Malformed patch: please specify at least block=""<block searchstring>"" or SearchRegEx=""<searchstring>""")
        End If
        Dim strFileContentPatched As String = _
                strFileContent.Substring(0, intInsertPoint) & _
                strPatch & _
                strFileContent.Substring(intInsertPoint)
        Return strFileContentPatched
    End Function

    Function MyAssemblyResolveEventHandler(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly
        Dim NewAssembly As Assembly
        Try
            'This handler is called only when the common language runtime tries to bind to the assembly and fails. 
            Static MyEmbeddedAssemblies As New Dictionary(Of String, Assembly)
            Dim strAssemblyName As String = args.Name.Split(",")(0)
            If MyEmbeddedAssemblies.ContainsKey(strAssemblyName) Then
                Return MyEmbeddedAssemblies(strAssemblyName)
            End If
            NewAssembly = LoadAssemblyFromExecutingAssembly(strAssemblyName)
            If NewAssembly IsNot Nothing Then
                MyEmbeddedAssemblies.Add(strAssemblyName, NewAssembly)
            Else
                If strAssemblyName <> "System.XmlSerializers" Then
                    MessageBox.Show("Can't find Assembly " & strAssemblyName)
                End If
            End If
            Return NewAssembly

        Catch ex As Exception
            MessageBox.Show("MyAssemblyResolveEventHandler error: " & ex.Message)
        End Try
        Return Nothing
    End Function

    Public Function LoadAssemblyFromExecutingAssembly(ByVal AssemblyName As String) As Assembly
        Dim s As IO.Stream = Nothing
        Dim strNameSpace As String = Assembly.GetExecutingAssembly().ToString.Split(",")(0).Trim
        s = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("{0}.{1}.dll", strNameSpace, AssemblyName))
        If s IsNot Nothing Then
            Dim block(s.Length) As Byte
            s.Read(block, 0, block.Length)
            Return Assembly.Load(block)
        End If
        Return Nothing
    End Function
End Class
