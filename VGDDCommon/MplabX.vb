Imports System.Xml
Imports VGDDCommon
Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Text
Imports VirtualFabUtils.Utils

Public Class MplabX

    Public Shared oProjectXmlDoc As XmlDocument
    Public Shared oRootFolderNode As XmlNode
    Public Shared MalRelativePath As String
    Public Shared Log As New StringBuilder
    Public Shared OriginalProjectXml As XmlDocument
    Public Shared DevBoards As New Dictionary(Of String, DevelopmentBoard)
    Public Shared PIMBoards As New Dictionary(Of String, PIMBoard)
    Public Shared ExpBoards As New Dictionary(Of String, ExpansionBoard)
    Public Shared DisplayBoards As New Dictionary(Of String, DisplayBoard)
    Public Shared ProjectConfigs As New Collection
    Public Shared MplabXTemplateDocEl As XmlElement
    Public Shared MplabXFilesAssembly As Reflection.Assembly
    Public Shared ConfigPropertyValue As String

    Public Class DevelopmentBoard
        Public ID As String
        Public Description As String
        Public Compiler As String
        Public Enabled As Boolean
        Public Image As Bitmap
        Public CompatibleExpansionBoards As New Collection
        Public CompatibleDisplayBoards As New Collection
        Public CompatibleOptions As New Collection
        Public URL As String
        Public PartNumber As String
        Public Note As String
        Public HasPIM As Boolean
    End Class

    Public Class PIMBoard
        Public ID As String
        Public Description As String
        Public Enabled As Boolean
        Public URL As String
        Public PartNumber As String
        Public Note As String
        Public [Class] As String
    End Class

    Public Class ExpansionBoard
        Public ID As String
        Public Description As String
        Public Enabled As Boolean
        Public Image As Bitmap
        Public CompatibleDisplayBoards As New Collection
        Public CompatibleOptions As New Collection
        Public URL As String
        Public PartNumber As String
        Public Note As String
        Public [Class] As String
    End Class

    Public Class DisplayBoard
        Public ID As String
        Public Description As String
        Public Enabled As Boolean
        Public Image As Bitmap
        Public Width As Integer
        Public Height As Integer
        Public CompatibleOptions As New Collection
        Public URL As String
        Public PartNumber As String
        Public DefaultOrientation As Integer
        Public Note As String
    End Class

    'Private WithEvents oMplabxIpc As MplabxIpc

    Public Shared Sub LoadMplabxProject()
        ProjectConfigs.Clear()
        MalRelativePath = RelativePath.Evaluate(Common.MplabXProjectPath, VGDDCommon.Mal.MalPath).Replace("\", "/")
        If oMplabxIpc.IsConnected Then
            MplabXIpcStartSession()
            Exit Sub
        End If
        If Common.MplabXProjectXmlPathName Is Nothing OrElse Not FileExistsCaseSensitive(Common.MplabXProjectXmlPathName) Then Exit Sub
        Try
            Log.Append("Opening MPLAB X Project " & Common.MplabXProjectXmlPathName)
            oProjectXmlDoc = New XmlDocument
            Try
                oProjectXmlDoc.Load(Common.MplabXProjectXmlPathName)
            Catch ex As Exception
                MessageBox.Show("Error loading MPLAB X Project: " & ex.Message, "Error")
                Log.AppendLine(" ERROR")
                Exit Sub
            End Try
            Dim oAttr As XmlAttribute = oProjectXmlDoc.DocumentElement.Attributes("version")
            If oAttr Is Nothing OrElse oAttr.Value < 62 Then
                MessageBox.Show("Unsupported MPLAB X Project format. Must be Version>=62", "Error")
                Log.AppendLine(" ERROR")
            Else
                Log.AppendLine("")
            End If
            oRootFolderNode = MplabXGetSubFolder(oProjectXmlDoc.DocumentElement, "root", True)
            MplabX.OriginalProjectXml = New XmlDocument
            MplabX.OriginalProjectXml.InnerXml = oProjectXmlDoc.InnerXml

            For Each oConfNode As XmlNode In oProjectXmlDoc.DocumentElement.SelectSingleNode("confs").ChildNodes
                Dim strConfigName As String = oConfNode.Attributes("name").Value
                ProjectConfigs.Add(strConfigName)
            Next

        Catch ex As Exception
            MessageBox.Show("Cannot load MPLAB X Project file " & Common.MplabXProjectXmlPathName)
        End Try
    End Sub

    Public Shared Function LoadTemplates() As Boolean
        DevBoards.Clear()
        PIMBoards.Clear()
        ExpBoards.Clear()
        DisplayBoards.Clear()
        If Not Common.MplabXLoadAndMergeTemplates() Then
            Return False
            Exit Function
        End If
        MplabXTemplateDocEl = Common.XmlMplabxTemplatesDoc.DocumentElement
        For Each oControlNode As XmlNode In MplabXTemplateDocEl.SelectSingleNode("DevelopmentBoards").ChildNodes
            If oControlNode.NodeType = XmlNodeType.Element Then
                Dim oBoard As New DevelopmentBoard
                oBoard.Description = oControlNode.Attributes("Description").Value
                oBoard.ID = oControlNode.Attributes("ID").Value
                oBoard.Enabled = True
                If oControlNode.Attributes("Enabled") IsNot Nothing Then
                    oBoard.Enabled = oControlNode.Attributes("Enabled").Value
                End If
                If oControlNode.Attributes("URL") IsNot Nothing Then
                    oBoard.URL = oControlNode.Attributes("URL").Value
                End If
                If oControlNode.Attributes("HasPIM") IsNot Nothing Then
                    oBoard.HasPIM = oControlNode.Attributes("HasPIM").Value
                End If
                If oControlNode.Attributes("Img") IsNot Nothing Then
                    oBoard.Image = Common.BitmapFromResource(oControlNode.Attributes("Img").Value, Reflection.Assembly.GetEntryAssembly)
                    If oBoard.Image Is Nothing Then
                        Dim strImgFile As String = FindUserTemplatesFile(oControlNode.Attributes("Img").Value)
                        If strImgFile <> String.Empty Then
                            oBoard.Image = Bitmap.FromFile(strImgFile)
                        End If
                    End If
                End If
                If oControlNode.Attributes("PartNumber") IsNot Nothing Then
                    oBoard.PartNumber = oControlNode.Attributes("PartNumber").Value
                End If
                For Each oNode As XmlNode In oControlNode.SelectNodes("Note")
                    oBoard.Note &= oNode.InnerText.Trim
                Next
                For Each oNode As XmlNode In oControlNode.SelectNodes("CompatibleExpansionBoard")
                    oBoard.CompatibleExpansionBoards.Add(Nothing, oNode.Attributes("ID").Value)
                Next

                'oBoard.CompatibleOptions.Add("chkGOL", "chkGOL")
                'oBoard.CompatibleOptions.Add("chkMCC", "chkMCC")
                For Each oNode As XmlNode In oControlNode.SelectNodes("CompatibleOptions")
                    For Each oOptionNode As XmlNode In oNode.ChildNodes
                        If Not oBoard.CompatibleOptions.Contains(oOptionNode.InnerText) Then
                            oBoard.CompatibleOptions.Add(oOptionNode.InnerText, oOptionNode.InnerText)
                        End If
                    Next
                Next
                For Each oNode As XmlNode In oControlNode.SelectNodes("CompatibleDisplay")
                    oBoard.CompatibleDisplayBoards.Add(Nothing, oNode.Attributes("ID").Value)
                Next
                DevBoards.Add(oBoard.ID, oBoard)
            End If
        Next

        For Each oControlNode As XmlNode In MplabXTemplateDocEl.SelectSingleNode("PIMBoards").ChildNodes
            If oControlNode.NodeType = XmlNodeType.Element Then
                Dim oBoard As New PIMBoard
                oBoard.Description = oControlNode.Attributes("Description").Value
                oBoard.ID = oControlNode.Attributes("ID").Value
                oBoard.Enabled = True
                If oControlNode.Attributes("Enabled") IsNot Nothing Then
                    oBoard.Enabled = oControlNode.Attributes("Enabled").Value
                End If
                If oControlNode.Attributes("URL") IsNot Nothing Then
                    oBoard.URL = oControlNode.Attributes("URL").Value
                End If
                If oControlNode.Attributes("PartNumber") IsNot Nothing Then
                    oBoard.PartNumber = oControlNode.Attributes("PartNumber").Value
                End If
                If oControlNode.Attributes("Class") IsNot Nothing Then
                    oBoard.Class = oControlNode.Attributes("Class").Value
                End If
                For Each oNode As XmlNode In oControlNode.SelectNodes("Note")
                    oBoard.Note &= oNode.InnerText.Trim
                Next
                PIMBoards.Add(oBoard.ID, oBoard)
            End If
        Next

        For Each oControlNode As XmlNode In MplabXTemplateDocEl.SelectSingleNode("ExpansionBoards").ChildNodes
            If oControlNode.NodeType = XmlNodeType.Element Then
                Dim oBoard As New ExpansionBoard
                oBoard.Description = oControlNode.Attributes("Description").Value
                oBoard.ID = oControlNode.Attributes("ID").Value
                oBoard.Enabled = True
                If oControlNode.Attributes("Enabled") IsNot Nothing Then
                    oBoard.Enabled = oControlNode.Attributes("Enabled").Value
                End If
                If oControlNode.Attributes("URL") IsNot Nothing Then
                    oBoard.URL = oControlNode.Attributes("URL").Value
                End If
                Dim oImageNode As XmlNode = oControlNode.Attributes("Img")
                If oImageNode IsNot Nothing AndAlso oImageNode.Value <> String.Empty Then
                    oBoard.Image = Common.BitmapFromResource(oControlNode.Attributes("Img").Value, Reflection.Assembly.GetEntryAssembly)
                    If oBoard.Image Is Nothing Then
                        Dim strImgFile As String = FindUserTemplatesFile(oControlNode.Attributes("Img").Value)
                        If strImgFile <> String.Empty Then
                            oBoard.Image = Bitmap.FromFile(strImgFile)
                        End If
                    End If
                End If
                If oControlNode.Attributes("PartNumber") IsNot Nothing Then
                    oBoard.PartNumber = oControlNode.Attributes("PartNumber").Value
                End If
                If oControlNode.Attributes("Class") IsNot Nothing Then
                    oBoard.Class = oControlNode.Attributes("Class").Value
                End If
                For Each oNode As XmlNode In oControlNode.SelectNodes("Note")
                    oBoard.Note &= oNode.InnerText.Trim
                Next
                For Each oNode As XmlNode In oControlNode.SelectNodes("CompatibleDisplay")
                    oBoard.CompatibleDisplayBoards.Add(Nothing, oNode.Attributes("ID").Value)
                Next
                For Each oNode As XmlNode In oControlNode.SelectNodes("CompatibleOptions")
                    For Each oOptionNode As XmlNode In oNode.ChildNodes
                        If Not oBoard.CompatibleOptions.Contains(oOptionNode.InnerText) Then
                            oBoard.CompatibleOptions.Add(oOptionNode.InnerText, oOptionNode.InnerText)
                        End If
                    Next
                Next
                ExpBoards.Add(oBoard.ID, oBoard)
            End If
        Next

        For Each oControlNode As XmlNode In MplabXTemplateDocEl.SelectSingleNode("DisplayBoards").ChildNodes
            If oControlNode.NodeType = XmlNodeType.Element Then
                Dim oBoard As New DisplayBoard
                oBoard.Description = oControlNode.Attributes("Description").Value
                oBoard.ID = oControlNode.Attributes("ID").Value
                oBoard.Enabled = True
                If oControlNode.Attributes("Enabled") IsNot Nothing Then
                    oBoard.Enabled = oControlNode.Attributes("Enabled").Value
                End If
                If oControlNode.Attributes("URL") IsNot Nothing Then
                    oBoard.URL = oControlNode.Attributes("URL").Value
                End If
                If oControlNode.Attributes("Img") IsNot Nothing Then
                    oBoard.Image = Common.BitmapFromResource(oControlNode.Attributes("Img").Value, Reflection.Assembly.GetEntryAssembly)
                    If oBoard.Image Is Nothing Then
                        Dim strImgFile As String = FindUserTemplatesFile(oControlNode.Attributes("Img").Value)
                        If strImgFile <> String.Empty Then
                            oBoard.Image = Bitmap.FromFile(strImgFile)
                        End If
                    End If
                End If
                For Each oNode As XmlNode In oControlNode.SelectNodes("CompatibleOptions")
                    For Each oOptionNode As XmlNode In oNode.ChildNodes
                        If Not oBoard.CompatibleOptions.Contains(oOptionNode.InnerText) Then
                            oBoard.CompatibleOptions.Add(oOptionNode.InnerText, oOptionNode.InnerText)
                        End If
                    Next
                Next
                If oControlNode.Attributes("DefaultOrientation") IsNot Nothing Then
                    oBoard.DefaultOrientation = oControlNode.Attributes("DefaultOrientation").Value
                End If
                If oControlNode.Attributes("PartNumber") IsNot Nothing Then
                    oBoard.PartNumber = oControlNode.Attributes("PartNumber").Value
                End If
                For Each oNode As XmlNode In oControlNode.SelectNodes("Note")
                    oBoard.Note &= oNode.InnerText.Trim
                Next
                DisplayBoards.Add(oBoard.ID, oBoard)
            End If
        Next
        Return True
    End Function

    Private Shared Function FindUserTemplatesFile(ByVal strImgFile) As String
        Dim oUFi As New DirectoryInfo(Path.Combine(Common.UserTemplatesFolder, "Boards"))
        If oUFi.Exists Then
            For Each oFi As DirectoryInfo In oUFi.GetDirectories
                Dim strImgPath As String = Path.Combine(oFi.FullName, strImgFile)
                If FileExistsCaseSensitive(strImgPath) Then
                    Return strImgPath
                End If
            Next
        End If
        Return String.Empty
    End Function

    Public Shared Function CleanMplabXProject(ByVal strXml As String) As String
        Dim intPos1 As Integer
        Dim intPos2 As Integer
        For Each strTag As String In {"targetPluginBoard", "targetHeader"}
            intPos1 = 0
            intPos2 = 0
            Do While strXml.Substring(intPos2).Contains("<" & strTag & ">")
                intPos1 = strXml.IndexOf("<" & strTag & ">", intPos2)
                intPos2 = strXml.IndexOf("</" & strTag & ">", intPos2)
                Dim strTagContent As String = strXml.Substring(intPos1 + strTag.Length + 2, intPos2 - intPos1 - (strTag.Length + 2)).Replace(vbCr, "").Replace(vbLf, "").Trim
                If strTagContent = "" Then
                    'Debug.Print(strXml.Substring(intPos1 - 20, 100))
                    strXml = strXml.Substring(0, intPos1 + strTag.Length + 2) & strXml.Substring(intPos2)
                    intPos2 += 20
                End If
            Loop
        Next
        Return strXml
    End Function

    Public Shared Function SaveMplabXProject() As Boolean
        If Not Directory.Exists(Path.GetDirectoryName(Common.MplabXProjectXmlPathName)) Then
            Log.AppendLine("Folder for MPLAB X project " & Common.MplabXProjectXmlPathName & " does not exists?")
            Return False
        End If
        Dim sw As New StringWriter
        Dim xtw As New XmlTextWriter(sw)
        xtw.Formatting = Formatting.Indented
        Log.Append("Writing MPLAB X Project File " & Common.MplabXProjectXmlPathName)
        MplabX.oProjectXmlDoc.WriteTo(xtw)
        Try
            Common.MakeBackup(Common.MplabXProjectXmlPathName)
        Catch ex As Exception

        End Try
        Try
            Dim fs As FileStream = Nothing
            Do While True
                Try
                    fs = New FileStream(Common.MplabXProjectXmlPathName, FileMode.Create, FileAccess.Write, FileShare.None)
                Catch exIO As IO.IOException
                    If (MessageBox.Show("Please go to MPLAB X and CLOSE the project!", "MPLAB X Project opened!", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.Cancel) Then
                        Log.AppendLine(" ERROR")
                        Return False
                    End If
                    Continue Do
                Catch ex As Exception
                    MessageBox.Show("Cannot save project to " & Common.MplabXProjectXmlPathName & vbCrLf & ex.Message, "Error saving project", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
                Exit Do
            Loop
            fs.Close()

            Dim fw As StreamWriter = New StreamWriter(Common.MplabXProjectXmlPathName, False, New System.Text.UTF8Encoding)
            Dim strXml As String = CleanMplabXProject(sw.ToString)
            fw.Write(strXml)
            fw.Close()
        Catch ex As Exception
            MessageBox.Show("Error writing to " & Common.MplabXProjectXmlPathName & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Log.AppendLine(" ERROR")
            Return False
        End Try
        Return True
    End Function

    Public Shared Function MplabXGetSubFolder(ByVal oFolderParentNode As XmlElement, ByVal FolderName As String, ByVal FallBack As Boolean) As XmlNode
        Dim aFolders() As String = FolderName.Split("/")
        Dim oFolderNode As XmlNode = Nothing
        If oFolderParentNode Is Nothing Then Return Nothing
        For i As Integer = 0 To aFolders.Length - 1
            Dim strFolderName As String = aFolders(i).Replace(" ", "")
            oFolderNode = oFolderParentNode.SelectSingleNode(String.Format("logicalFolder[@name='{0}']", strFolderName))
            If oFolderNode Is Nothing Then
                If FallBack Then
                    Return oFolderParentNode.SelectSingleNode("//configurationDescriptor/logicalFolder")
                Else
                    Return Nothing
                End If
            End If
            oFolderParentNode = oFolderNode
        Next
        Return oFolderNode
    End Function

    Public Shared IncludeSearchPath As String = String.Empty
    Public Shared Sub MplabXAddIncludeSearch(ByVal oPathsToAdd As XmlNodeList)
        For Each oAddPathNode As XmlNode In oPathsToAdd
            Dim oOptionAttr As XmlAttribute = oAddPathNode.Attributes("Option")
            Dim oOptionFamily As XmlAttribute = oAddPathNode.Attributes("Family")
            Dim blnApply As Nullable(Of Boolean) = Nothing
            If oOptionAttr Is Nothing Then
                blnApply = True
            Else
                Dim strFamily As String = String.Empty
                If oOptionFamily IsNot Nothing Then
                    strFamily = oOptionFamily.Value
                End If
                For Each strOptionName As String In oOptionAttr.Value.Split(",")
                    blnApply = EvaluateOption(strOptionName, strFamily, blnApply)
                Next
            End If
            If blnApply Then
                Dim strPathToAdd As String = oAddPathNode.InnerText.Replace("$MAL", MplabX.MalRelativePath)
                If Not IncludeSearchPath.Contains(strPathToAdd) Then
                    IncludeSearchPath &= strPathToAdd
                    If Not IncludeSearchPath.EndsWith(";") Then
                        IncludeSearchPath &= ";"
                    End If
                End If
            End If
        Next
    End Sub

    Public Shared Function EvaluateOption(ByVal strOptionName As String, ByVal strFamily As String, ByVal blnApply As Nullable(Of Boolean)) As Nullable(Of Boolean)
        Dim strOptionCleanName As String = strOptionName
        Try
            If Not Char.IsLetter(strOptionName.Substring(0, 1)) Then
                strOptionCleanName = strOptionName.Substring(1)
            End If
            If Common.WizardOptions(strOptionCleanName) Is Nothing Then
                Common.WizardOptions.Add(strOptionCleanName, False)
            End If
            If strFamily = "PIC32" Then
                If Common.ProjectCompilerFamily <> "C32" Then
                    Return False
                End If
            ElseIf strFamily = "PIC24" Then
                If Common.ProjectCompilerFamily <> "C30" Then
                    Return False
                End If
            End If
            If strOptionName.StartsWith("!") Then
                strOptionName = strOptionName.Substring(1)
                Dim blnOptionValue As Boolean = Not (Common.WizardOptions(strOptionName) = True)
                If blnApply Is Nothing Then
                    blnApply = blnOptionValue
                Else
                    blnApply = blnApply And blnOptionValue
                End If
            ElseIf strOptionName.StartsWith("|") Then
                strOptionName = strOptionName.Substring(1)
                If blnApply Is Nothing Then
                    blnApply = Common.WizardOptions(strOptionName)
                Else
                    blnApply = blnApply Or Common.WizardOptions(strOptionName)
                End If
            Else
                If blnApply Is Nothing Then
                    blnApply = Common.WizardOptions(strOptionName)
                Else
                    blnApply = blnApply And Common.WizardOptions(strOptionName)
                End If
            End If
        Catch ex As Exception
            'MessageBox.Show(ex.Message, "EvaluateOption")
        End Try
        Return blnApply
    End Function

    Public Shared Sub MplabXAddProjectFiles(ByVal oFilesToAdd As XmlNodeList)
        For Each oAddFileNode As XmlNode In oFilesToAdd
            Dim oOptionAttr As XmlAttribute = oAddFileNode.Attributes("Option")
            Dim oOptionFamily As XmlAttribute = oAddFileNode.Attributes("Family")
            Dim blnApply As Nullable(Of Boolean) = Nothing
            If oOptionAttr Is Nothing Then
                blnApply = True
            Else
                Dim strFamily As String = String.Empty
                If oOptionFamily IsNot Nothing Then
                    strFamily = oOptionFamily.Value
                End If
                For Each strOptionName As String In oOptionAttr.Value.Split(",")
                    blnApply = EvaluateOption(strOptionName, strFamily, blnApply)
                Next
            End If
            If blnApply Then
                MplabX.AddFile(oAddFileNode)
            End If
        Next
    End Sub

    Public Shared Function MplabXEvalCondition(ByVal oTemplateNode As XmlNode) As Boolean
        Dim blnApply As Boolean = True
        Dim oConditionAttr As XmlAttribute = oTemplateNode.Attributes("Condition")
        If oConditionAttr IsNot Nothing Then
            For Each strCondition As String In oConditionAttr.Value.Split(",")
                If Not strCondition.Contains(" ") AndAlso strCondition.Contains("=") Then
                    strCondition = strCondition.Replace("=", " = ")
                End If
                Dim aCond() As String = strCondition.Trim.Split(" ")
                Dim strOp As String = aCond(1)
                Dim strCondLeft As String = ""
                Select Case aCond(0).ToUpper.Trim
                    Case "DEVBOARDID"
                        strCondLeft = Common.DevelopmentBoardID
                    Case "EXPBOARDID"
                        strCondLeft = Common.ExpansionBoardID
                    Case "DISPBOARDID"
                        strCondLeft = Common.DisplayBoardID
                    Case "EXPBOARDCLASS"
                        Dim oExpBoard As ExpansionBoard = ExpBoards(Common.ExpansionBoardID)
                        If oExpBoard IsNot Nothing Then
                            strCondLeft = oExpBoard.Class
                        Else
                            Debug.Print("?")
                        End If
                    Case "EXPBOARDID"
                        strCondLeft = Common.DisplayBoardID
                    Case "GRAPHICS_LIBRARY_VERSION"
                        strCondLeft = Mal.MalVersionNum
                        Dim GfxVersion As Integer
                        If Integer.TryParse(aCond(2).Replace("0x", ""), GfxVersion) Then
                            aCond(2) = GfxVersion.ToString
                        End If
                    Case "FRAMEWORK"
                        strCondLeft = Mal.FrameworkName.ToUpper
                    Case Else
                        MessageBox.Show(String.Format("Condition operator ""{0}"" not handled in ""{1}""", strCondLeft, oTemplateNode.Attributes("Name").Value))
                        Exit For
                End Select
                If strOp = "=" Then
                    If strCondLeft <> aCond(2).Trim Then
                        blnApply = False
                        Exit For
                    End If
                ElseIf strOp = ">" Then
                    If strCondLeft <= aCond(2).Trim Then
                        blnApply = False
                        Exit For
                    End If
                ElseIf strOp = ">=" Then
                    If strCondLeft < aCond(2).Trim Then
                        blnApply = False
                        Exit For
                    End If
                ElseIf strOp = "<" Then
                    If strCondLeft >= aCond(2).Trim Then
                        blnApply = False
                        Exit For
                    End If
                ElseIf strOp = "<=" Then
                    If strCondLeft > aCond(2).Trim Then
                        blnApply = False
                        Exit For
                    End If
                Else
                    If strCondLeft = aCond(2).Trim Then
                        blnApply = False
                        Exit For
                    End If
                End If
            Next
        End If
        Return blnApply
    End Function

    Private Shared Function MplabXInsertCode(ByRef strFileContent As String, ByRef intPos1 As Integer, ByRef intPos2 As Integer, ByVal TemplateNodes As XmlNodeList, ByVal SectionName As String, ByVal BoardType As String, ByVal BoardID As String, ByVal FileName As String, ByVal strGroupName As String, ByVal Order As Integer) As String
        MplabXInsertCode = ""
        Try
            If TemplateNodes.Count > 0 Then
                For Each oTemplateNode As XmlNode In TemplateNodes
                    Dim strOptionNameToReport As String = String.Empty
                    Dim blnApply As Nullable(Of Boolean) = Nothing
                    Dim oOptionAttr As XmlAttribute = oTemplateNode.Attributes("Option")
                    Dim oOptionFamily As XmlAttribute = oTemplateNode.Attributes("Family")
                    Dim strFamily As String = String.Empty
                    If oOptionFamily IsNot Nothing Then
                        strFamily = oOptionFamily.Value
                    End If
                    If oOptionAttr Is Nothing Then
                        If strFamily = String.Empty Then
                            blnApply = True
                        Else
                            blnApply = strFamily = Common.ProjectPicFamily
                        End If
                    Else
                        strOptionNameToReport = oOptionAttr.Value
                        For Each strOptionName As String In oOptionAttr.Value.Split(",")
                            blnApply = EvaluateOption(strOptionName, strFamily, blnApply)
                        Next
                    End If

                    If blnApply Then
                        blnApply = MplabXEvalCondition(oTemplateNode)
                    End If

                    If blnApply Then
                        Dim strTemplateText As String = ""
                        If Common.InsertDebugInfo Then
                            Dim strDebugInfo As String
                            If strGroupName <> String.Empty Then
                                If strOptionNameToReport <> String.Empty Then
                                    strDebugInfo = String.Format("// Group:{0} Option:{1}", strGroupName, strOptionNameToReport)
                                Else
                                    strDebugInfo = String.Format("// Group:{0}", strGroupName)
                                End If
                            Else
                                strDebugInfo = String.Format("// Section {1}.{2}.{0} Option:{3}", SectionName, BoardType, BoardID, strOptionNameToReport)
                            End If
                            For Each strRow As String In oTemplateNode.InnerText.Split(vbLf)
                                strRow = strRow.Replace(vbCr, "").Replace(vbTab, "    ").TrimEnd
                                If strRow <> "" AndAlso Not strRow.EndsWith("\") Then
                                    If strRow.Length < 80 Then
                                        strRow = strRow.PadRight(80)
                                    End If
                                    strTemplateText &= strRow & " " & strDebugInfo & vbCrLf
                                Else
                                    strTemplateText &= strRow & vbCrLf
                                End If
                            Next
                        Else
                            strTemplateText = oTemplateNode.InnerText
                        End If
                        Do While strTemplateText.StartsWith(vbCrLf)
                            strTemplateText = strTemplateText.Substring(2)
                        Loop
                        Do While strTemplateText.EndsWith(vbCrLf)
                            strTemplateText = strTemplateText.Substring(0, strTemplateText.Length - 2)
                        Loop
                        strTemplateText &= vbCrLf
                        If Common.InsertDebugInfo Then
                            If Order <> 99 Then
                                strTemplateText = "// BEGIN Order=" & Order & vbCrLf & strTemplateText & vbCrLf & "// END Order=" & Order & vbCrLf
                            Else
                                strTemplateText = "// BEGIN" & vbCrLf & strTemplateText & "// END" & vbCrLf
                            End If
                        End If
                        MplabXInsertCode &= String.Format("Inserting Code in Section {1}.{2}.{0} in file {3}", SectionName, BoardType, BoardID, Path.GetFileName(FileName))
                        strFileContent = strFileContent.Substring(0, intPos1) & strTemplateText & strFileContent.Substring(intPos2)
                        intPos1 += strTemplateText.Length
                        intPos2 += strTemplateText.Length '+ 2
                    End If
                Next
            End If

        Catch ex As Exception
            MplabXInsertCode = "// Error inserting code:" & ex.Message
        End Try
    End Function

    Public Shared Function ModifySkeletonFile(ByVal FileName As String, ByVal ClearSection As Boolean) As String
        Return ModifySkeletonFile(FileName, ClearSection, Nothing)
    End Function

    Public Shared Function ModifySkeletonFile(ByVal FileName As String, ByVal ClearSection As Boolean, ByVal oFixedGroupNodes As XmlNodeList) As String
        ModifySkeletonFile = ""
        Dim strFileContent As String
        Try
            Using oSr As New StreamReader(FileName)
                strFileContent = oSr.ReadToEnd.Replace(vbCr, "").Replace(vbLf, vbCrLf)
                oSr.Close()
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message & vbCrLf & "May be file has to be generated for the first time", "Unable to read " & FileName)
            Exit Function
        End Try

        Dim intPos1 As Integer = 0, intPos2 As Integer ', intTagLen As Integer
        Const TAGSTART As String = "VGDD_MPLABX_WIZARD_START_SECTION:"
        Const TAGEND As String = "VGDD_MPLABX_WIZARD_END_SECTION"
        Do While True
            intPos1 = strFileContent.IndexOf(TAGSTART, intPos1)
            If intPos1 = -1 Then Exit Do
            Dim strSectionName As String = strFileContent.Substring(intPos1 + TAGSTART.Length)
            strSectionName = strSectionName.Substring(0, strSectionName.IndexOf(vbCrLf)).Trim
            If strSectionName.Contains(" ") Then
                strSectionName = strSectionName.Substring(0, strSectionName.IndexOf(" "))
            End If
            intPos1 = strFileContent.IndexOf(vbCrLf, intPos1) + 2
            intPos2 = strFileContent.IndexOf(TAGEND, intPos1)
            If intPos2 = -1 Then Exit Do
            Do While strFileContent.Substring(intPos2, 1) <> vbLf
                intPos2 -= 1
            Loop
            intPos2 += 1
            'intTagLen = intPos2 - intPos1 - 2
            'Dim strTes As String = strFileContent.Substring(intPos1)
            'Dim strTes2 As String = strFileContent.Substring(intPos2)
            'strTes = strFileContent.Substring(intPos1)
            If ClearSection Then
                strFileContent = strFileContent.Substring(0, intPos1) & strFileContent.Substring(intPos2)
                intPos2 = strFileContent.IndexOf(TAGEND, intPos1)
                Do While strFileContent.Substring(intPos2, 1) <> vbLf
                    intPos2 -= 1
                Loop
                intPos2 += 1
                'strTes2 = strFileContent.Substring(intPos2)
            End If

            Dim aBoardTypes As New ArrayList, aBoardsID As New ArrayList
            aBoardTypes.Add("DevelopmentBoards")
            aBoardsID.Add(Common.DevelopmentBoardID)
            If Common.PIMBoardID <> "" Then
                aBoardTypes.Add("PIMBoards")
                aBoardsID.Add(Common.PIMBoardID)
            End If
            If Common.ExpansionBoardID <> "" Then
                aBoardTypes.Add("ExpansionBoards")
                aBoardsID.Add(Common.ExpansionBoardID)
            End If
            aBoardTypes.Add("DisplayBoards")
            aBoardsID.Add(Common.DisplayBoardID)
            For o As Integer = 0 To 99
                For intBt As Integer = 0 To aBoardTypes.Count - 1
                    Dim oGroupNodes As XmlNodeList = Common.XmlMplabxTemplatesDoc.DocumentElement.SelectNodes(String.Format("{0}/Board[@ID='{1}']/AddGroup", aBoardTypes(intBt), aBoardsID(intBt)))
                    'ModifySkeletonFile &= ModifySkeletonFileCore(oGroupNodes, strSectionName, aBoardTypes(intBt), aBoardsID(intBt), o, strFileContent, intPos1, intPos2, FileName)
                    Dim strBoardType As String = aBoardTypes(intBt)
                    Dim strBoardID As String = aBoardsID(intBt)
                    For intNi As Integer = -2 To oGroupNodes.Count - 1
                        Dim strSectionFilter As String = String.Format("[@Name='{0}'", strSectionName)
                        If o < 99 Then
                            strSectionFilter &= String.Format(" and @Order='{0}'", o.ToString)
                        Else
                            strSectionFilter &= " and not(@Order)"
                        End If
                        strSectionFilter &= "]"
                        'WizardLogWrite(String.Format("Checking group {0}.{1} section {2}", Common.DevelopmentBoardID, GroupNode.Attributes("Name").Value, strSectionName))
                        Dim strXPath As String
                        Select Case intNi
                            Case -2
                                strXPath = String.Format("{0}/Board[@ID='{1}']/Code/Section{2}", strBoardType, strBoardID, strSectionFilter)
                            Case -1
                                strXPath = String.Format("{0}/Board[@ID='{1}']/Code/{2}/Section{3}", strBoardType, strBoardID, Mal.FrameworkName, strSectionFilter)
                            Case Else
                                strXPath = String.Format("Groups/Group[@Name='{0}']/Code/Section{1}", oGroupNodes(intNi).Attributes("Name").Value, strSectionFilter)
                        End Select
                        Dim TemplateNodes As XmlNodeList
                        TemplateNodes = Common.XmlMplabxTemplatesDoc.DocumentElement.SelectNodes(strXPath)
                        If TemplateNodes.Count > 0 Then
                            'WizardLogWrite(Nothing, TemplateNodes.Count)
                            Dim strGroupName As String = String.Empty
                            Dim blnApply As Boolean = True
                            If intNi >= 0 Then
                                strGroupName = oGroupNodes(intNi).Attributes("Name").Value
                                blnApply = MplabXEvalCondition(oGroupNodes(intNi))
                            End If
                            If blnApply Then
                                ModifySkeletonFile &= MplabXInsertCode(strFileContent, intPos1, intPos2, TemplateNodes, strSectionName, strBoardType, strBoardID, FileName, strGroupName, o)
                            End If
                        End If
                    Next intNi
                    'If oFixedGroupNodes IsNot Nothing Then
                    '    ModifySkeletonFile &= ModifySkeletonFileCore(oFixedGroupNodes, strSectionName, aBoardTypes(intBt), aBoardsID(intBt), o, strFileContent, intPos1, intPos2, FileName)
                    'End If
                Next intBt
            Next o

            'strTes = strFileContent.Substring(intPos1)
            'strTes2 = strFileContent.Substring(intPos2)
            Dim intPosStart As Integer = strFileContent.IndexOf(TAGSTART, intPos1)
            Dim intPosEnd As Integer = strFileContent.IndexOf(TAGEND, intPos1)
            If intPosStart > 0 And intPosStart < intPosEnd Then
                intPos1 = intPosStart
            Else
                intPos1 = intPosEnd
            End If
            'intPos1 = strFileContent.IndexOf(TAGEND, intPos1)
            If intPos1 = -1 Then Exit Do
        Loop

        strFileContent = CodeGen.ReplaceProjectStrings(strFileContent)
        If strFileContent.Contains("[MPLABX_PROJECT_FOLDER]") Then
            strFileContent = strFileContent.Replace("[MPLABX_PROJECT_FOLDER]", RelativePath.Evaluate(FileName, Common._MplabxProjectPath).Replace("\", "/"))
        End If

        Common.WriteFileWithBackup(FileName, strFileContent)
        If strFileContent.Contains("//VGDD#warning") Then
            Dim aContent = strFileContent.Split(New String() {"//VGDD#warning"}, StringSplitOptions.RemoveEmptyEntries)
            For i = 1 To aContent.Length - 1
                Dim strWarning As String = aContent(i).Substring(0, aContent(i).IndexOf(vbCr)).Trim
                If strWarning.Contains(" //") Then strWarning = strWarning.Substring(0, strWarning.IndexOf(" //"))
                If strWarning.StartsWith("""") Then strWarning = strWarning.Substring(1)
                If strWarning.EndsWith("""") Then strWarning = strWarning.Substring(0, strWarning.Length - 1)
                Common.WizardWarnings &= strWarning & vbCrLf
            Next i
        End If
    End Function

    Public Shared Function AddFileCheckDestFile(ByVal oFileNode As XmlNode, ByRef strDestFile As String, ByRef strDestDir As String) As String
        Dim strFileName As String = oFileNode.InnerText.Replace("$MAL", MplabX.MalRelativePath).Replace("[COMPILER]", Common.ProjectCompiler).Replace("[PICFAMILY]", Common.ProjectPicFamily)
        strDestFile = Path.GetFileName(strFileName)
        If oFileNode.Attributes("DestFile") IsNot Nothing Then
            strDestFile = oFileNode.Attributes("DestFile").Value
        End If
        strDestDir = Common.CodeGenDestPath
        If oFileNode.Attributes("DestDir") IsNot Nothing Then
            strDestDir = Path.Combine(strDestDir, oFileNode.Attributes("DestDir").Value)
        End If
        If Not Directory.Exists(strDestDir) Then
            Try
                Directory.CreateDirectory(strDestDir)
            Catch ex As Exception
                Return String.Empty
            End Try
        End If
        Return strFileName
    End Function

    Public Shared Sub AddFile(ByVal oAddFileNode As XmlNode)
        Dim strFolderName As String = oAddFileNode.Attributes("Name").Value.Replace("[ACTIVECONFIG]", Common.MplabXSelectedConfig)
        If Mal.FrameworkName.ToUpper = "MLA" Then
            strFolderName = strFolderName.Replace("/framework/", "/frameworkMLA/")
        End If

        If strFolderName <> String.Empty Then
            MplabXCheckFolderPath(strFolderName)
        End If
        For Each ActionType As String In New String() {"AddFile", "AddVGDDFile", "RemoveFile", "EnableDisableDefine"}
            For Each oFileNode As XmlNode In oAddFileNode.SelectNodes(ActionType)
                Dim strDestFile As String = String.Empty
                Dim strDestDir As String = String.Empty
                Dim strFileName As String = AddFileCheckDestFile(oFileNode, strDestFile, strDestDir)
                Dim strDestPath As String = Path.GetFullPath(Path.Combine(strDestDir, strDestFile))

                Select Case ActionType
                    Case "AddVGDDFile"
                        If Not FileExistsCaseSensitive(strDestPath) OrElse Common.WizardForceAddVgddFiles Then
                            Log.AppendLine("Extracting VGDD file " & strFileName)
                            If Not ExtractResourceToFile(strFileName.Replace("/", "."), strDestPath, MplabX.MplabXFilesAssembly) _
                                AndAlso Not ExtractResourceToFile(strFileName.Replace("/", "."), strDestPath, Reflection.Assembly.GetEntryAssembly) Then
                                MessageBox.Show("Can't extract resource " & strFileName & " to " & strDestPath, "ExtractResourceToFile ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        End If
                        MplabXAddFileIfNotExist(strFolderName, strDestPath)
                    Case "AddFile"
                        If strFileName.Contains("$USERTEMPLATESFOLDER") Then
                            strFileName = strFileName.Replace("$USERTEMPLATESFOLDER", Common.UserTemplatesFolder)
                            Dim strFileName2 As String = Path.Combine(Common.VGDDProjectPath, Path.Combine(strDestDir, Path.GetFileName(strFileName)))
                            If Not FileExistsCaseSensitive(strFileName2) Then
                                Try
                                    File.Copy(strFileName, strFileName2)
                                Catch ex As Exception
                                    MessageBox.Show("Error copying file " & strFileName & " to " & strFileName2 & ":" & vbCrLf & ex.Message)
                                End Try
                            End If
                            MplabXAddFileIfNotExist(strFolderName, strFileName2)
                        Else
                            If oFileNode.Attributes("DestDir") IsNot Nothing Then
                                If Not FileExistsCaseSensitive(strDestPath) Then
                                    Try
                                        File.Copy(Path.GetFullPath(oFileNode.InnerText.Replace("$MAL", Mal.MalPath)), strDestPath)
                                    Catch ex As Exception
                                        MessageBox.Show("Error copying file " & strFileName & " to " & strDestPath & ":" & vbCrLf & ex.Message)
                                    End Try
                                End If
                                MplabXAddFileIfNotExist(strFolderName, strDestPath)
                            Else
                                MplabXAddFileIfNotExist(strFolderName, strFileName)
                            End If
                        End If
                        If oFileNode.Attributes("Modify") IsNot Nothing AndAlso oFileNode.Attributes("Modify").Value = "Yes" Then
                            ModifySkeletonFile(strDestPath, True)
                        End If
                    Case "RemoveFile"
                        MplabXRemoveFileIfExist(strFolderName, strFileName)
                    Case "EnableDisableDefine"
                        Dim strDefineFileName As String = oFileNode.Attributes("File").Value
                        If Not MplabxFiles.ContainsKey(strDefineFileName) Then
                            FileLoad(strDefineFileName)
                        End If
                        For Each oDefineNode As XmlNode In oFileNode.ChildNodes
                            Select Case oDefineNode.Name
                                Case "Enable"
                                    FileEnableDefine(strDefineFileName, oDefineNode.InnerText, "#define")
                                Case "Disable"
                                    FileDisableDefine(strDefineFileName, oDefineNode.InnerText, False)
                            End Select
                        Next
                        FileSave(strDefineFileName)
                End Select

                Dim oOverrideAttr As XmlAttribute = oFileNode.Attributes("Override")
                If oOverrideAttr IsNot Nothing Then
                    Dim strKeyName As String = oOverrideAttr.Value.Split("=")(0)
                    Dim strKeyValue As String = oOverrideAttr.Value.Split("=")(1)
                    MplabXOverrideConfProperty(Common.MplabXSelectedConfig, "", strFileName, strKeyName, strKeyValue)
                End If

            Next
        Next
    End Sub

    Public Shared Sub MplabXCheckFolderPath(ByVal strFolderPath As String)
        Try
            If MplabX.oProjectXmlDoc Is Nothing Then Exit Sub
            Dim oFolderParentNode As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oProjectXmlDoc.DocumentElement, "root", True)
            If oFolderParentNode Is Nothing Then Exit Sub
            Dim aFolders As String() = strFolderPath.Split("/")
            Dim oFolderNode As XmlNode
            For i As Integer = 0 To aFolders.Length - 1
                Dim strFolderName As String = aFolders(i).Replace(" ", "")
                oFolderNode = MplabX.MplabXGetSubFolder(oFolderParentNode, strFolderName, False)
                If oFolderNode Is Nothing Then
                    oFolderNode = MplabXCreateFolder(strFolderName, aFolders(i), oFolderParentNode)
                    Log.AppendLine("Created Folder " & strFolderName & " (" & aFolders(i) & ") under " & oFolderParentNode.Attributes("name").Value)
                End If
                oFolderParentNode = oFolderNode
            Next
        Catch ex As Exception
        End Try
    End Sub

    Public Shared Function MplabXCreateFolder(ByVal FolderName As String, ByVal DisplayName As String, ByVal ParentFolder As XmlNode) As XmlNode
        Try
            Dim oFolder As XmlNode = MplabX.oProjectXmlDoc.CreateElement("logicalFolder")
            Dim oAttr As XmlAttribute

            oAttr = MplabX.oProjectXmlDoc.CreateAttribute("name")
            oAttr.Value = FolderName
            oFolder.Attributes.Append(oAttr)

            oAttr = MplabX.oProjectXmlDoc.CreateAttribute("displayName")
            oAttr.Value = DisplayName
            oFolder.Attributes.Append(oAttr)

            oAttr = MplabX.oProjectXmlDoc.CreateAttribute("projectFiles")
            oAttr.Value = "true"
            oFolder.Attributes.Append(oAttr)

            ParentFolder.AppendChild(oFolder)
            Return oFolder

        Catch ex As Exception

        End Try
        Return Nothing
    End Function

    Public Class OverrideData
        Public ConfName As String
        Public SubSection As String
        Public itemPath As String
        Public PropertyName As String
        Public PropertyVal As String
    End Class
    Public Shared OverridesList As New Collection
    Public Shared Sub MplabXOverrideConfProperty(ByVal ConfName As String, ByVal SubSection As String, ByVal itemPath As String, ByVal PropertyName As String, ByVal PropertyVal As String)
        Dim oOverrideData As New OverrideData
        With oOverrideData
            .ConfName = ConfName
            .SubSection = SubSection
            .itemPath = itemPath
            .PropertyName = PropertyName
            .PropertyVal = PropertyVal
        End With
        OverridesList.Add(oOverrideData)
    End Sub

    Public Shared Sub MplabXApplyOverrides()
        For Each oOverrideData As OverrideData In OverridesList
            Try
                If oOverrideData.ConfName Is Nothing OrElse oOverrideData.ConfName = "" Then
                    oOverrideData.ConfName = "default"
                End If
                If MplabX.IpcEnabled Then
                    Dim Section As String = Common.ProjectCompilerFamily
                    Section = Section.Replace("XC32", "C32").Replace("XC16", "C30")
                    Log.AppendLine("Sending VGDD-Link command to override configuration property " & oOverrideData.PropertyName & "=" & oOverrideData.PropertyVal & " in section """ & Section & """")
                    oMplabxIpc.IpcSend("OVERRIDE_CONF_PROPERTY", oOverrideData.ConfName & "|" & Section & "|" & oOverrideData.SubSection & "|" & oOverrideData.PropertyName & "|" & oOverrideData.PropertyVal & "|" & oOverrideData.itemPath)
                    Return
                Else
                    Dim oConfNode As XmlNode = MplabX.oProjectXmlDoc.SelectSingleNode(String.Format("configurationDescriptor/confs/conf[@name='{0}']", oOverrideData.ConfName))
                    Dim Section As String = oConfNode.SelectSingleNode("toolsSet/languageToolchain").InnerText

                    Select Case Section
                        Case "XC16"
                            Section = "C30"
                        Case "XC32"
                            Section = "C32"
                    End Select
                    Dim oOverrideNode As XmlNode = oConfNode.SelectSingleNode(String.Format("item[@path='{0}']", oOverrideData.itemPath))
                    If oOverrideNode Is Nothing Then
                        Log.AppendLine("Creating Override Item " & oOverrideData.itemPath)
                        oOverrideNode = MplabX.oProjectXmlDoc.CreateElement("item")
                        Dim oAttr As XmlAttribute = MplabX.oProjectXmlDoc.CreateAttribute("path")
                        oAttr.Value = oOverrideData.itemPath
                        oOverrideNode.Attributes.Append(oAttr)

                        oAttr = MplabX.oProjectXmlDoc.CreateAttribute("ex")
                        oAttr.Value = "false"
                        oOverrideNode.Attributes.Append(oAttr)

                        oAttr = MplabX.oProjectXmlDoc.CreateAttribute("overriding")
                        oAttr.Value = "true"
                        oOverrideNode.Attributes.Append(oAttr)

                        oConfNode.AppendChild(oOverrideNode)
                    End If

                    oOverrideNode = oConfNode.SelectSingleNode(String.Format("item[@path='{0}']/{1}{2}", oOverrideData.itemPath, Section, oOverrideData.SubSection))
                    If oOverrideNode Is Nothing Then
                        Log.AppendLine("Creating Override Section " & Section & oOverrideData.SubSection)
                        oOverrideNode = MplabX.oProjectXmlDoc.CreateElement(Section & oOverrideData.SubSection)
                        oConfNode.SelectSingleNode(String.Format("item[@path='{0}']", oOverrideData.itemPath)).AppendChild(oOverrideNode)
                    End If

                    oOverrideNode = oConfNode.SelectSingleNode(String.Format("item[@path='{0}']/{1}{2}/property[@key='{3}']", oOverrideData.itemPath, Section, oOverrideData.SubSection, oOverrideData.PropertyName))
                    If oOverrideNode Is Nothing Then
                        Log.AppendLine("Overriding Project Property " & oOverrideData.PropertyName & " in section " & oOverrideData.itemPath & "-" & Section & oOverrideData.SubSection)
                        oOverrideNode = MplabX.oProjectXmlDoc.CreateElement("property")
                        Dim oAttr As XmlAttribute = MplabX.oProjectXmlDoc.CreateAttribute("key")
                        oAttr.Value = oOverrideData.PropertyName
                        oOverrideNode.Attributes.Append(oAttr)
                        oAttr = MplabX.oProjectXmlDoc.CreateAttribute("value")
                        oAttr.Value = ""
                        oOverrideNode.Attributes.Append(oAttr)
                        oConfNode.SelectSingleNode(String.Format("item[@path='{0}']/{1}{2}", oOverrideData.itemPath, Section, oOverrideData.SubSection)).AppendChild(oConfNode)
                    End If
                    If oOverrideNode.Attributes("value").Value <> oOverrideData.PropertyVal Then
                        Log.AppendLine("Overriding Project Property " & oOverrideData.PropertyName & "=" & oOverrideData.PropertyVal & " in section " & oOverrideData.ConfName & oOverrideData.SubSection)
                        oOverrideNode.Attributes("value").Value = oOverrideData.PropertyVal
                    End If
                End If
            Catch ex As Exception

            End Try

        Next
    End Sub

    Public Shared Function MplabXExcludeFile(ByVal ConfName As String, ByVal itemPath As String, ByVal strFolderName As String) As XmlNode
        Try
            If ConfName Is Nothing OrElse ConfName = "" Then
                ConfName = "default"
            End If
            If MplabX.IpcEnabled Then
                Log.AppendLine("Sending VGDD-Link command to exclude file " & itemPath & " in folder """ & strFolderName & """")
                oMplabxIpc.IpcSend("EXCLUDEFILE", ConfName & "|" & strFolderName & "|" & itemPath)
            Else
                Dim oConfNode As XmlNode = MplabX.oProjectXmlDoc.SelectSingleNode(String.Format("configurationDescriptor/confs/conf[@name='{0}']", ConfName))
                Dim Section As String = oConfNode.SelectSingleNode("toolsSet/languageToolchain").InnerText

                Select Case Section
                    Case "XC16"
                        Section = "C30"
                    Case "XC32"
                        Section = "C32"
                End Select
                MplabXExcludeFile = oConfNode.SelectSingleNode(String.Format("item[@path='{0}']", itemPath))
                If MplabXExcludeFile Is Nothing Then
                    Log.AppendLine("Creating Override Item " & itemPath)
                    MplabXExcludeFile = MplabX.oProjectXmlDoc.CreateElement("item")
                    Dim oAttr As XmlAttribute = MplabX.oProjectXmlDoc.CreateAttribute("path")
                    oAttr.Value = itemPath
                    MplabXExcludeFile.Attributes.Append(oAttr)

                    oAttr = MplabX.oProjectXmlDoc.CreateAttribute("ex")
                    oAttr.Value = "true"
                    MplabXExcludeFile.Attributes.Append(oAttr)

                    oAttr = MplabX.oProjectXmlDoc.CreateAttribute("overriding")
                    oAttr.Value = "false"
                    MplabXExcludeFile.Attributes.Append(oAttr)

                    oConfNode.AppendChild(MplabXExcludeFile)
                ElseIf MplabXExcludeFile.Attributes("ex").Value.ToLower = "false" Then
                    MplabXExcludeFile.Attributes("ex").Value = "true"
                End If
            End If

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function MplabXIncludeFile(ByVal ConfName As String, ByVal itemPath As String) As XmlNode
        If ConfName Is Nothing OrElse ConfName = "" Then
            ConfName = "default"
        End If
        If MplabX.oProjectXmlDoc Is Nothing Then
            'CodeGen.Errors &= "Cannot enable file module " & itemPath & " to MPLAB X project in config """ & ConfName & """" & vbCrLf
            Return (Nothing)
        End If
        Try
            Dim oConfNode As XmlNode = MplabX.oProjectXmlDoc.SelectSingleNode(String.Format("configurationDescriptor/confs/conf[@name='{0}']", ConfName))
            Dim Section As String = oConfNode.SelectSingleNode("toolsSet/languageToolchain").InnerText

            Select Case Section
                Case "XC16"
                    Section = "C30"
                Case "XC32"
                    Section = "C32"
            End Select
            MplabXIncludeFile = oConfNode.SelectSingleNode(String.Format("item[@path='{0}']", itemPath))
            If MplabXIncludeFile IsNot Nothing AndAlso MplabXIncludeFile.Attributes("ex").Value.ToLower = "true" Then
                MplabXIncludeFile.Attributes("ex").Value = "false"
            End If

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function MplabXRemoveFileIfExist(ByVal strFolderPath As String, ByVal strPathName As String) As Boolean
        If strFolderPath Is Nothing OrElse strPathName Is Nothing Then Return Nothing
        Dim oFolderNode As XmlNode
        If strFolderPath <> String.Empty Then
            oFolderNode = MplabX.MplabXGetSubFolder(MplabX.oRootFolderNode, strFolderPath, False)
        Else
            oFolderNode = MplabX.oRootFolderNode
        End If
        Dim oVGDDNode As XmlNode = MPLABXSearchItemInFolder(oFolderNode, Path.GetFileName(strPathName))
        If oVGDDNode IsNot Nothing Then
            oVGDDNode.ParentNode.RemoveChild(oVGDDNode)
            Return True
        End If
        Return False
    End Function

    Public Shared Function MplabXAddFileIfNotExist(ByVal strFolderPath As String, ByVal strPathName As String) As XmlNode
        If strFolderPath Is Nothing OrElse strPathName Is Nothing Then Return Nothing
        If MplabX.IpcEnabled Then
            MplabXIpcAddFile(strPathName, strFolderPath)
            Return Nothing
        Else
            MplabX.MplabXCheckFolderPath(strFolderPath)
            Dim oFolderNode As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oRootFolderNode, strFolderPath, False)
            Return MplabXAddFileIfNotExist(oFolderNode, strPathName)
        End If
    End Function

    Public Shared Function MplabXAddFileIfNotExist(ByVal oFolderNode As XmlNode, ByVal strPathName As String) As XmlNode
        Dim oVGDDNode As XmlNode = MPLABXSearchItemInFolder(oFolderNode, Path.GetFileName(strPathName))
        Dim strRelativePath As String = RelativePath.Evaluate(Common.MplabXProjectPath, strPathName).Replace("\", "/")
        If strRelativePath.StartsWith("./") Then strRelativePath = strRelativePath.Substring(2)
        If strRelativePath.Contains("/Microchip/..") Then
            strRelativePath = strRelativePath.Replace("/Microchip/..", "")
        End If
        'If strPathName.ToUpper.Contains("SSD") Then
        '    Debug.Print("")
        'End If
        If oVGDDNode Is Nothing Then
            MplabXAddFile(strRelativePath, oFolderNode)
        Else
            Dim strOldFile As String = oVGDDNode.InnerText
            If strOldFile <> strRelativePath Then
                oVGDDNode.InnerText = strRelativePath
            End If
        End If
        MplabXIncludeFile(Common.MplabXSelectedConfig, strRelativePath)
        Return oVGDDNode
    End Function

    Public Shared Function MPLABXSearchItemInFolder(ByVal oXmlFolderNode As XmlNode, ByVal PrgName As String) As XmlNode
        If oXmlFolderNode Is Nothing Then Return Nothing
        For Each oNode As XmlNode In oXmlFolderNode.ChildNodes
            Select Case oNode.Name
                Case "logicalFolder"
                    Dim oNodeSearch As XmlNode = MPLABXSearchItemInFolder(oNode, PrgName)
                    If oNodeSearch IsNot Nothing Then
                        Return oNodeSearch
                    End If
                Case "itemPath"
                    If Path.GetFileName(oNode.InnerText).ToLower = PrgName.ToLower Then
                        Return oNode
                    End If
            End Select
        Next
        Return Nothing
    End Function

    Private Shared aSearchFolders As New ArrayList, aFoundItems As New ArrayList
    Public Shared Function MPLABXGetAllItemsInFolder(ByVal oXmlFolderNode As XmlNode) As ArrayList
        Static blnSelectedConfig As Boolean = False
        If oXmlFolderNode Is Nothing Then Return Nothing
        If oXmlFolderNode.Name = "configurationDescriptor" Then
            aSearchFolders.Clear()
            aFoundItems.Clear()
        End If
        For Each oNode As XmlNode In oXmlFolderNode.ChildNodes
            Select Case oNode.Name
                Case "logicalFolder"
                    If oNode.Attributes("name").Value <> "root" Then
                        If oNode.Attributes("displayName") IsNot Nothing Then
                            aSearchFolders.Add(oNode.Attributes("displayName").Value)
                        Else
                            aSearchFolders.Add(oNode.Attributes("name").Value)
                        End If
                    End If
                    MPLABXGetAllItemsInFolder(oNode)
                Case "itemPath"
                    Dim sb As New StringBuilder
                    For i As Integer = 0 To aSearchFolders.Count - 1
                        sb.Append(aSearchFolders(i))
                        sb.Append("/")
                    Next
                    Dim strItem As String = oNode.InnerText
                    If strItem.StartsWith("../") AndAlso strItem.Contains("../framework/") Then
                        Do While strItem.StartsWith("../")
                            strItem = strItem.Substring(3)
                        Loop
                        strItem = "$MAL/" & strItem
                    End If
                    Dim strFolderPath As String = sb.ToString
                    aFoundItems.Add(strFolderPath & "|" & strItem)
                Case "confs"
                    MPLABXGetAllItemsInFolder(oNode)
                Case "conf"
                    blnSelectedConfig = (oNode.Attributes("name").Value = Common.MplabXSelectedConfig)
                    MPLABXGetAllItemsInFolder(oNode)
                Case "item"
                    If blnSelectedConfig AndAlso oNode.Attributes("ex").Value = "true" Then
                        Dim strFile As String = Path.GetFileName(oNode.Attributes("path").Value)
                        For Each strItem As String In aFoundItems
                            If strItem.Contains(strFile) Then
                                aFoundItems.Remove(strItem)
                                Exit For
                            End If
                        Next
                    End If
            End Select
        Next
        If aSearchFolders.Count > 0 Then
            aSearchFolders.RemoveAt(aSearchFolders.Count - 1)
        End If
        Return aFoundItems
    End Function

    Public Shared Function MplabXProjectSearchItem(ByVal PrgName As String) As XmlNode
        Dim oFolderNode As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oRootFolderNode, "root", True)
        Return MPLABXSearchItemInFolder(oFolderNode, PrgName)
    End Function

    Public Shared Sub MplabXIpcStartSession()
        MplabxIpcAddedFiles = New Collection
    End Sub

    Public Shared MplabxIpcAddedFiles As Collection
    Public Shared Function MplabXIpcAddFile(ByVal strPathName As String, ByVal strFolderPath As String) As Boolean
        If oMplabxIpc.IsConnected Then
            If MplabxIpcAddedFiles IsNot Nothing Then
                If MplabxIpcAddedFiles.Contains(strPathName) Then Return True
                MplabxIpcAddedFiles.Add("", strPathName)
            End If
            Try
                If Not strPathName.StartsWith("..") AndAlso Not Directory.Exists(Path.GetDirectoryName(strPathName)) Then
                    Directory.CreateDirectory(Path.GetDirectoryName(strPathName))
                End If
            Catch ex As Exception
            End Try
            Dim strRelativePath As String = RelativePath.Evaluate(Common.MplabXProjectPath, strPathName).Replace("\", "/")
            If strRelativePath.StartsWith("./") Then strRelativePath = strRelativePath.Substring(2)
            If strRelativePath.Contains("/Microchip/..") Then
                strRelativePath = strRelativePath.Replace("/Microchip/..", "")
            End If
            If strFolderPath.Contains("Header Files") Then
                oMplabxIpc.IpcSend("ADDFILE", "HEADER FILES|" & strFolderPath & "|" & strRelativePath)
                Log.AppendLine("Sending IPC command to Add File " & strRelativePath & " to folder """ & strFolderPath & """")
            Else
                oMplabxIpc.IpcSend("ADDFILE", "SOURCE FILES|" & strFolderPath & "|" & strRelativePath)
                Log.AppendLine("Sending IPC command to Add File " & strRelativePath & " to folder """ & strFolderPath & """")
            End If
            System.Threading.Thread.Sleep(100)
            'Else
            'Debug.Print("?")
        End If
        Return True
    End Function

    Public Shared Sub MplabXAddFile(ByVal FileName As String, ByVal LogicalFoderNode As XmlNode)
        If LogicalFoderNode Is Nothing Then Exit Sub
        If FileName.Contains("/Microchip/..") Then
            FileName = FileName.Replace("/Microchip/..", "")
        End If
        Dim strFolderName As String = LogicalFoderNode.Attributes("displayName").Value
        Log.AppendLine("Adding File " & FileName & " to folder """ & strFolderName & """")
        Dim oPrgNode As XmlNode = MplabX.oProjectXmlDoc.CreateElement("itemPath")
        oPrgNode.InnerText = FileName
        LogicalFoderNode.AppendChild(oPrgNode)
    End Sub

    Public Shared Sub MplabXAddFile(ByVal FileName As String, ByVal LogicalFoder As String)
        Dim oRootFolderNode As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oProjectXmlDoc.DocumentElement, "root", True)
        Dim oNode As XmlNode
        oNode = oRootFolderNode.SelectSingleNode(String.Format("logicalFolder[@name='{0}']", LogicalFoder))
        If oNode Is Nothing Then
            oNode = oRootFolderNode
        End If
        MplabXAddFile(FileName, oNode)
    End Sub

    Public Shared Sub MplabXAddLibrary(ByVal FileName As String)
        If oProjectXmlDoc Is Nothing Then Exit Sub
        FileName = FileName.Replace("/Microchip/..", "")
        Dim oLibsNode As XmlNode = oProjectXmlDoc.DocumentElement.SelectSingleNode(String.Format("confs/conf [@name='{0}']/compileType/linkerTool/linkerLibItems", Common.MplabXSelectedConfig))
        Dim blnAlreadyPresent As Boolean = False
        For Each oLibNode As XmlNode In oLibsNode.ChildNodes
            If oLibNode.InnerText.Contains(Path.GetFileName(FileName)) Then
                blnAlreadyPresent = True
                Exit For
            End If
        Next
        If Not blnAlreadyPresent Then
            Log.AppendLine("Adding Library " & FileName)
            Dim oLibNode As XmlNode = MplabX.oProjectXmlDoc.CreateElement("linkerLibFileItem")
            oLibNode.InnerText = FileName
            oLibsNode.AppendChild(oLibNode)
        End If
    End Sub

    Public Shared Function MplabXClearMakeFile() As Boolean
        For Each strFile As String In New String() {"Makefile-default.mk", "Makefile-genesis.properties", "Makefile-impl.mk", "Makefile-local-default.mk", "Makefile-variables.mk", "Package-default.bash"}
            Dim strFilePath As String = Path.Combine(Path.Combine(Common.MplabXProjectPath, "nbproject"), strFile)
            If FileExistsCaseSensitive(strFilePath) Then
                Try
                    File.Delete(strFilePath)
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "MplabXClearMakeFile Error")
                    Return False
                End Try
            End If
        Next
        Return True
    End Function

    Public Shared MplabxFiles As New Dictionary(Of String, String)
    Public Shared MplabxFilesOrig As New Dictionary(Of String, String)

    Public Shared Function FileLoad(ByVal strFile As String) As Boolean
        Dim strFilePath As String
        If IpcEnabled Then
            strFilePath = Path.GetFullPath(Path.Combine(Common.CodeGenDestPath, strFile))
            If Not FileExistsCaseSensitive(strFilePath) Then
                Return False
            End If
        Else
            Dim oFileNode As XmlNode = MplabXProjectSearchItem(strFile)
            If oFileNode Is Nothing Then Return False
            strFilePath = Path.GetFullPath(Path.Combine(Common.MplabXProjectPath, oFileNode.InnerText))
        End If
        Dim strFileContent As String = ReadFile(strFilePath)
        If Not MplabxFiles.ContainsKey(strFile) Then
            MplabxFiles.Add(strFile, strFileContent)
            MplabxFilesOrig.Add(strFile, strFileContent)
        Else
            MplabxFiles(strFile) = strFileContent
            MplabxFilesOrig(strFile) = strFileContent
        End If
        Return strFileContent <> String.Empty
    End Function

    Public Shared Function FileSave(ByVal strFile As String) As Boolean
        Try
            Dim strFilePath As String
            If IpcEnabled Then
                strFilePath = Path.GetFullPath(Path.Combine(Common.CodeGenDestPath, strFile))
                If Not FileExistsCaseSensitive(strFilePath) Then
                    Return False
                End If
            Else
                Dim oFileNode As XmlNode = MplabXProjectSearchItem(strFile)
                oFileNode = MplabXProjectSearchItem(strFile)
                strFilePath = Path.GetFullPath(Path.Combine(Common.MplabXProjectPath, oFileNode.InnerText))
            End If
            Dim strFileContent As String = MplabxFiles(strFile)
            Dim strFileContentOrig As String = MplabxFilesOrig(strFile)
            strFileContent = MplabxFiles(strFile)
            strFileContentOrig = MplabxFilesOrig(strFile)
            If strFileContent <> strFileContentOrig Then
                FileSave = Common.WriteFileWithBackup(strFilePath, strFileContent)
            End If
            FileSave = True
            MplabxFiles.Remove(strFile)
            MplabxFilesOrig.Remove(strFile)

        Catch ex As Exception
            MessageBox.Show("Cannot save MplabX project file " & strFile & ": " & ex.Message, "Mplabx.SaveFile Error")
            Return False
        End Try
    End Function

    Public Shared Function FileSetValueOfDefine(ByVal strFile As String, ByVal Define As String, ByVal value As String, ByVal Comment As String) As Boolean
        If Not MplabX.MplabxFiles.ContainsKey(strFile) Then
            CodeGen.Errors &= "Cannot set " & Define & " as " & value & ": file " & strFile & " not loaded. Run Wizard." & vbCrLf
        Else
            Dim strFileContent As String = MplabX.MplabxFiles.Item(strFile)
            If strFileContent Is Nothing Then Return False
            For Each strLine As String In strFileContent.Split(vbCrLf)
                strLine = strLine.Replace(vbCr, "").Replace(vbLf, "")
                If strLine.Contains(Define) AndAlso Not strLine.Trim.StartsWith("//") Then
                    If strLine.Trim.StartsWith("*") Then
                        Debug.Print("")
                    End If
                    MplabX.MplabxFiles.Item(strFile) = MplabX.MplabxFiles.Item(strFile).Replace(strLine, Define & " " & value & IIf(Comment IsNot Nothing, " //" & Comment, String.Empty))
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Public Shared Function FileCheckEnabledDefine(ByVal strFile As String, ByVal UseDefine As String) As Boolean
        For Each strLine As String In MplabxFiles(strFile).Split(vbCrLf)
            strLine = strLine.Replace(vbCr, "").Replace(vbLf, "")
            If strLine.Contains("#define ") AndAlso strLine.Contains(UseDefine) AndAlso Not strLine.Trim.StartsWith("//") Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Shared Function FileDisableDefine(ByVal strFile As String, ByVal UseDefine As String, ByVal blnDeleteLine As Boolean) As Boolean
        If MplabX.MplabxFiles.Count = 0 OrElse Not MplabX.MplabxFiles.ContainsKey(strFile) Then Return False
        FileDisableDefine = False
        If MplabX.FileCheckEnabledDefine(strFile, UseDefine) Then
            For Each strLine As String In MplabxFiles.Item(strFile).Split(vbCrLf)
                strLine = strLine.Replace(vbCr, "").Replace(vbLf, "")
                If strLine.Contains(UseDefine) Then
                    Dim strLineCheck As String = strLine & "//"
                    strLineCheck = strLineCheck.Substring(0, strLineCheck.IndexOf("//")).Trim
                    If strLineCheck.Contains(UseDefine) AndAlso Not strLineCheck.Trim.StartsWith("*") Then
                        If blnDeleteLine Then
                            MplabxFiles.Item(strFile) = MplabxFiles.Item(strFile).Replace(strLine & vbCrLf, "")
                        ElseIf Not strLine.Trim.StartsWith("//") Then
                            MplabxFiles.Item(strFile) = MplabxFiles.Item(strFile).Replace(strLine, "//" & strLine)
                        End If
                        FileDisableDefine = True
                    End If
                End If
            Next
        End If
        Return FileDisableDefine
    End Function

    Public Shared Function ReplaceInFile(ByVal MplabxFile As String, ByVal strSearchPattern As String, ByVal strReplace As String) As Boolean
        Dim strFileKey As String = Path.GetFileName(MplabxFile)
        If Not MplabxFiles.ContainsKey(strFileKey) Then
            MplabX.FileLoad(strFileKey)
        End If
        Try
            If MplabxFiles(strFileKey).Contains(strSearchPattern) Then
                MplabxFiles(strFileKey) = MplabxFiles(strFileKey).Replace(strSearchPattern, strReplace.Replace("<pattern>", strSearchPattern))
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Public Shared Function CommentOut(ByVal strSearchPattern As String, ByRef strFileContent As String) As Boolean
        Try
            If strFileContent.Contains(strSearchPattern) Then
                If strFileContent.Contains("// " & strSearchPattern) Then
                    Return True
                End If
                strFileContent = strFileContent.Replace(strSearchPattern, "// " & strSearchPattern)
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Public Shared Function DisableInclude(ByVal Include As String, ByRef strFileContent As String) As Boolean
        Return CommentOut(Include, strFileContent)
    End Function

    Public Shared Function FileEnableDefine(ByVal strFile As String, ByVal UseDefine As String, ByVal MarkerAdd As String) As Boolean
        'Const MARKERADD As String = "#define _GRAPHICSCONFIG_H"
        Dim MARKERUNCOMMENT As String = "#define " & UseDefine
        If MplabxFiles.Count = 0 OrElse Not MplabxFiles.ContainsKey(strFile) OrElse Not MplabxFiles.Item(strFile).Contains(MarkerAdd) Then
            Return False
        End If
        If Not MplabX.FileCheckEnabledDefine(strFile, UseDefine) Then
            If MplabxFiles.Item(strFile).Contains("//" & MARKERUNCOMMENT) Then
                MplabxFiles.Item(strFile) = MplabxFiles.Item(strFile).Replace("//" & MARKERUNCOMMENT, MARKERUNCOMMENT)
            Else
                Dim intPos As Integer = MplabxFiles.Item(strFile).LastIndexOf(MarkerAdd)
                intPos = MplabxFiles.Item(strFile).IndexOf(Environment.NewLine, intPos) + Environment.NewLine.Length
                Dim strPart1 As String = MplabxFiles.Item(strFile).Substring(0, intPos)
                Dim strPart2 As String = MplabxFiles.Item(strFile).Substring(intPos)
                If Not strPart1.EndsWith(Environment.NewLine) Then strPart1 &= Environment.NewLine
                If Not strPart2.StartsWith(Environment.NewLine) Then strPart2 = Environment.NewLine & strPart2
                MplabxFiles.Item(strFile) = strPart1 & "#define " & UseDefine & strPart2

            End If
            Return True
        Else
            Debug.Print("Already")
        End If
        Return False
    End Function

    Public Shared Function GraphicsConfigEnableDefine(ByVal UseDefine As String) As Boolean
        Select Case Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                Return MplabX.FileEnableDefine("GraphicsConfig.h", UseDefine, "#define _GRAPHICSCONFIG_H")
            Case "MLA"
                Return MplabX.FileEnableDefine("gfx_config.h", UseDefine, "#define _GRAPHICS_CONFIG_H")
            Case "HARMONY"
                Return MplabX.FileEnableDefine("system_config.h", UseDefine, "#define _SYSTEM_CONFIG_H")
        End Select
        Return False
    End Function

    Public Shared Function GraphicsConfigDisableDefine(ByVal UseDefine As String) As Boolean
        Select Case Mal.FrameworkName.ToUpper
            Case "MLALEGACY"
                Return MplabX.FileDisableDefine("GraphicsConfig.h", UseDefine, False)
            Case "MLA"
                Return MplabX.FileDisableDefine("gfx_config.h", UseDefine, False)
            Case "HARMONY"
                Return MplabX.FileDisableDefine("system_config.h", UseDefine, False)
        End Select
        Return False
    End Function

    Public Shared Sub MplabXExcludeAllFilesInFolders(ByVal strFolderName As String)
        Try
            Dim oRootFolderNode As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oProjectXmlDoc.DocumentElement, "root", True)
            'WizardLogWrite("Excluding from build all files in Folder " & strFolderName & " for config " & Common.MplabXSelectedConfig)
            Dim oFolderProjectNode As XmlNode = MplabX.MplabXGetSubFolder(oRootFolderNode, strFolderName, False)
            If oFolderProjectNode IsNot Nothing Then
                For Each oFileNode As XmlNode In oFolderProjectNode.ChildNodes
                    For Each oItemNode As XmlNode In oFileNode.ChildNodes
                        MplabX.MplabXExcludeFile(Common.MplabXSelectedConfig, oItemNode.InnerText, strFolderName)
                    Next
                Next
            End If
            'WizardLogWrite(Nothing, "OK")
        Catch ex As Exception
        End Try
    End Sub

    Public Shared Sub MplabXIncludeAllFilesInFolders(ByVal strFolderName As String)
        Try
            Dim oRootFolderNode As XmlNode = MplabX.MplabXGetSubFolder(MplabX.oProjectXmlDoc.DocumentElement, "root", True)
            'WizardLogWrite("Including in build all files in Folder " & strFolderName & " for config " & Common.MplabXSelectedConfig)
            Dim oFolderProjectNode As XmlNode = MplabX.MplabXGetSubFolder(oRootFolderNode, strFolderName, False)
            If oFolderProjectNode IsNot Nothing Then
                For Each oFileNode As XmlNode In oFolderProjectNode.ChildNodes
                    MplabX.MplabXIncludeFile(Common.MplabXSelectedConfig, oFileNode.InnerText)
                Next
            End If
            'WizardLogWrite(Nothing, "OK")
        Catch ex As Exception
        End Try
    End Sub

#Region "MplabxIPC"

    Public Shared WithEvents oMplabxIpc As MplabxIpc

    'Public Shared Function GetLocalIPv4Address() As String
    '    GetLocalIPv4Address = String.Empty
    '    Dim strHostName As String = System.Net.Dns.GetHostName()
    '    Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(strHostName)

    '    For Each ipheal As System.Net.IPAddress In iphe.AddressList
    '        If ipheal.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
    '            GetLocalIPv4Address = ipheal.ToString()
    '        End If
    '    Next
    'End Function

    Public Shared Sub IPC_Start()
        oMplabxIpc = New MplabxIpc
        oMplabxIpc.IpcStart(Common.MplabXIpcIpAddress, Common.MplabXIpcPort)
    End Sub

    Private Shared Sub oMplabxIpc_ResponseReceived(ByVal Response As String) Handles oMplabxIpc.ResponseReceived
        'If Me.InvokeRequired Then
        '    Dim d As New IpcResponseReceivedCallBack(AddressOf IpcResponseReceivedThreadSafe)
        '    Me.Invoke(d, New Object() {Response})
        'Else
        IpcResponseReceivedThreadSafe(Response)
        'End If
    End Sub

    Public Delegate Sub IpcResponseReceivedCallBack(ByVal Response As String)
    Public Shared Sub IpcResponseReceivedThreadSafe(ByVal Response As String)
        'IpcCheckConnectStatus(Response)
        'txtLog.Text &= Response & vbCrLf
        'txtLog.ScrollToCaret()
        For Each ResponseLine As String In Response.Split(vbCrLf)
            Dim aResponse As String() = ResponseLine.Split("|")
            Select Case aResponse(0).Trim
                Case "MPLABX_ACTIVECONFIG_NAME"
                    Common.MplabXSelectedConfig = aResponse(1)
                Case "MPLABX_ACTIVECONFIG_TOOLCHAINNAME"
                    Common.ProjectCompilerFamily = ""
                    Common.ProjectCompiler = aResponse(1)
                    Select Case Common.ProjectCompiler
                        Case "XC16", "C30"
                            Common.ProjectCompilerFamily = "C30"
                        Case "XC32", "C32"
                            Common.ProjectCompilerFamily = "C32"
                    End Select
                Case "MPLABX_PROJECTPATH"
                    Dim strPath As String = aResponse(1).Replace("/", Path.DirectorySeparatorChar)
                    If strPath = "null" Then
                        Common.MplabXProjectPath = String.Empty
                    End If
                    If strPath <> String.Empty Then
                        ' Fix for Linux<->Windows mixed environments
                        If Common.VGDDProjectPath <> String.Empty Then
                            Dim strVpath As String = Path.GetDirectoryName(Common.VGDDProjectPath).Replace(Path.GetPathRoot(Common.VGDDProjectPath), "")
                            Do While strVpath.Split("\").Length > 1
                                If strPath.Contains(strVpath) Then
                                    strPath = strPath.Substring(strPath.IndexOf(strVpath))
                                    strPath = Path.Combine(Path.GetPathRoot(Common.VGDDProjectPath), strPath)
                                    Exit Do
                                End If
                                strVpath = strVpath.Substring(0, strVpath.LastIndexOf("\"))
                            Loop
                        End If
                    End If
                    Common.MplabXProjectPath = strPath
                Case "MPLABX_ACTIVECONFIG_DEVICENAME"
                    'Dim strDeviceName As String = oMplabxIpc.IpcWaitForResponse()
                    'If strDeviceName.Substring(3, 2) = "32" Then
                    '    Common.ProjectCompilerFamily = "C32"
                    'ElseIf strDeviceName.Substring(3, 2) = "24" Then
                    '    Common.ProjectCompilerFamily = "C30"
                    'ElseIf strDeviceName.ToUpper.StartsWith("DSPIC") Then
                    '    Common.ProjectCompilerFamily = "C30"
                    'End If
                    'oMplabxIpc.IpcSend("GET_MPLABX_ACTIVECONFIG_GETCOMPILER", "")
                    'Dim strCompilerName As String = oMplabxIpc.IpcWaitForResponse()

                Case "GET_CONF_PROPERTY"
                    ConfigPropertyValue = aResponse(1)

            End Select
        Next
    End Sub

    Private Shared _IpcEnabled As Boolean
    Public Shared Property IpcEnabled As Boolean
        Get
            If oMplabxIpc Is Nothing Then
                _IpcEnabled = False
            End If
            Return _IpcEnabled
        End Get
        Set(ByVal value As Boolean)
            _IpcEnabled = value
        End Set
    End Property

#End Region

    Public Shared Function MplabXGetConfProperty(ByVal ConfName As String, ByVal Section As String, ByVal SubSection As String, ByVal PropertyName As String) As String
        Log.Length = 0
        If oMplabxIpc.IsConnected Then
            If Section = "" Then
                Section = Common.ProjectCompilerFamily
            End If
            Section = Section.Replace("XC32", "C32").Replace("XC16", "C30")
            ConfigPropertyValue = Nothing
            oMplabxIpc.IpcSend("GET_CONF_PROPERTY", ConfName & "|" & Section & "|" & SubSection & "|" & PropertyName)
            For i As Integer = 1 To 10
                Application.DoEvents()
                If ConfigPropertyValue IsNot Nothing Then
                    Return ConfigPropertyValue
                End If
                System.Threading.Thread.Sleep(250)
            Next
        Else
            If MplabX.oProjectXmlDoc Is Nothing Then Return Nothing
            Dim oConfNode As XmlNode = oProjectXmlDoc.SelectSingleNode(String.Format("configurationDescriptor/confs/conf[@name='{0}']", ConfName))
            If oConfNode Is Nothing Then Return Nothing
            If Section = "" Then
                Section = oConfNode.SelectSingleNode("toolsSet/languageToolchain").InnerText
                If Section = "XC32" Then
                    Section = "C32"
                ElseIf Section = "XC16" Then
                    Section = "C30"
                End If
            End If
            Dim oConfPropertyNode As XmlNode = oConfNode.SelectSingleNode(String.Format("{0}{1}/property[@key='{2}']", Section, SubSection, PropertyName))
            If oConfPropertyNode Is Nothing Then
                Return Nothing
            End If
            Return oConfPropertyNode.Attributes("value").Value
        End If
    End Function

    Public Shared Function MplabXSetConfProperty(ByVal ConfName As String, ByVal Section As String, ByVal SubSection As String, ByVal PropertyName As String, ByVal PropertyVal As String, ByRef Log As String) As XmlNode
        Log = String.Empty
        If oMplabxIpc.IsConnected Then
            If Section = "" Then
                Section = Common.ProjectCompilerFamily
            End If
            Section = Section.Replace("XC32", "C32").Replace("XC16", "C30")
            oMplabxIpc.IpcSend("SET_CONF_PROPERTY", ConfName & "|" & Section & "|" & SubSection & "|" & PropertyName & "|" & PropertyVal)
            MplabXSetConfProperty = New XmlDocument
        Else
            If oProjectXmlDoc Is Nothing Then Return Nothing
            Dim oConfNode As XmlNode = MplabX.oProjectXmlDoc.SelectSingleNode(String.Format("configurationDescriptor/confs/conf[@name='{0}']", ConfName))
            If oConfNode Is Nothing Then Return Nothing
            If Section = "" Then
                Section = oConfNode.SelectSingleNode("toolsSet/languageToolchain").InnerText
                If Section = "XC32" Then
                    Section = "C32"
                ElseIf Section = "XC16" Then
                    Section = "C30"
                End If
            End If

            MplabXSetConfProperty = oConfNode.SelectSingleNode(String.Format("{0}{1}/property[@key='{2}']", Section, SubSection, PropertyName))
            If MplabXSetConfProperty Is Nothing Then
                Log &= "Creating Project Property " & PropertyName & " in section " & ConfName & SubSection
                MplabXSetConfProperty = MplabX.oProjectXmlDoc.CreateElement("property")
                Dim oAttr As XmlAttribute = MplabX.oProjectXmlDoc.CreateAttribute("key")
                oAttr.Value = PropertyName
                MplabXSetConfProperty.Attributes.Append(oAttr)
                oAttr = MplabX.oProjectXmlDoc.CreateAttribute("value")
                oAttr.Value = ""
                MplabXSetConfProperty.Attributes.Append(oAttr)
                oConfNode = oConfNode.SelectSingleNode(Section & SubSection)
                If oConfNode Is Nothing Then
                    Dim strMsg As String = "Trying to set Project Property " & PropertyName & " in section " & ConfName & SubSection & vbCrLf & _
                        "But no Section " & Section & SubSection & " is present in Project File."
                    If Section.Contains("C30") Or Section.Contains("XC16") Then
                        strMsg &= vbCrLf & "Project appears to be created for a different MCU family (PIC32?)" & vbCrLf & "Check Wizard Hardware tab"
                    ElseIf Section.Contains("C32") Or Section.Contains("XC32") Then
                        strMsg &= vbCrLf & "Project appears to be created for a different MCU family (PIC24?)" & vbCrLf & "Check Wizard Hardware tab"
                    End If
                    strMsg &= vbCrLf & "Please enable VGDD-Link MPLAB X plugin for this modifications to work."
                    MessageBox.Show(strMsg, "Error setting MPLAB X Project Property", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Log &= "|KO!!" & vbCrLf
                    Return Nothing
                End If
                oConfNode.AppendChild(MplabXSetConfProperty)
                Log &= "OK" & vbCrLf
            Else
                Dim strCurrValue As String = MplabXSetConfProperty.Attributes("value").Value
                If strCurrValue = PropertyVal Then
                    Log &= "Project Property " & PropertyName & " already set to " & PropertyVal & " in section " & ConfName & SubSection & "|OK" & vbCrLf
                    Exit Function
                End If
                Dim intCurrValue As Integer
                If Integer.TryParse(strCurrValue, intCurrValue) Then
                    If intCurrValue >= PropertyVal Then
                        Log &= "Project Property " & PropertyName & " set to " & intCurrValue & " so skipping setting it to " & PropertyVal & " in section " & ConfName & SubSection & "|OK" & vbCrLf
                        Exit Function
                    End If
                End If
            End If
            If MplabXSetConfProperty.Attributes("value").Value <> PropertyVal Then
                Log &= "Setting Project Property " & PropertyName & "=" & PropertyVal & " in section " & ConfName & SubSection
                MplabXSetConfProperty.Attributes("value").Value = PropertyVal
                Log &= "|OK" & vbCrLf
            End If
        End If
    End Function
End Class
