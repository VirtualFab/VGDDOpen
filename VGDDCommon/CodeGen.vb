Imports System.Xml
Imports System.IO
Imports System.Data
Imports System.Windows.Forms
'Imports VGDDCommon
Imports VGDDCommon.Common
Imports System.Reflection
Imports VirtualFabUtils.Utils

Namespace VGDDCommon

    Public Class CodeGen
        Public Shared FinalCode As String = ""
        Public Shared HeadersIncludes As String = ""
        Public Shared Headers As String = ""
        Public Shared CodeHead As String = "", CodeEventsHelper As String = ""
        Public Shared Code As String = "", CodeFoot As String = ""
        Public Shared ScreenUpdateCode As String = ""
        Public Shared ScreenName As String
        Public Shared ControlIdPrefix As String
        Public Shared CodeTemplate As String, CodeHeadTemplate As String, AllCodeTemplate As String, HeadersTemplate As String, HeadersIncludesTemplate As String
        Public Shared CodeUpdateTemplate As String
        Public Shared TextDeclareCodeHeadTemplate(4) As String, TextDeclareHeaderTemplate(4) As String, ConstructorTemplate As String ', MatrixDeclareTemplate(4) As String
        Public Shared WidgetsTextTemplateCode As String
        Public Shared WidgetsTextTemplateHeader As String
        Public Shared ControlEventTemplate As String, ControlHeadEventTemplate As String, ControlFootEventTemplate As String
        Public Shared StateNode As XmlNode
        Public Shared AlignmentNode As XmlNode
        Public Shared StateMentNode As XmlNode
        Public Shared NumId As Integer, NumScreens As Integer
        Public Shared _ProjectName As String
        Public Shared AllScreensEventMsgCode As String, AllScreensEventDrawCode As String, ScreenEventCode As String
        Public Shared ScreenStates As String 'ScreenStatesEnum
        Public Shared sbCodeBitmap As Text.StringBuilder
        Public Shared XmlTemplatesDoc As XmlDocument = Nothing
        Public Shared GraphicsConfigPath As String
        Public Shared GraphicsConfigDefines As String, GraphicsConfigTemplate As String
        'Public Shared WarnOutOfRangeCharsDetected As Boolean = False
        Public Shared dtFootPrint As New DataTable
        Public Shared XmlExternalTemplatesDoc As XmlDocument = Nothing
        Public Shared Errors As String

        Shared Sub New()
            'LoadCodeGenTemplates()
        End Sub

        Public Shared Function Int2ByteH(ByVal Val As Int16) As Byte
            Dim bytes() As Byte = BitConverter.GetBytes(Val)
            Return bytes(1)
        End Function

        Public Shared Function Int2ByteL(ByVal Val As Int16) As Byte
            Dim bytes() As Byte = BitConverter.GetBytes(Val)
            Return bytes(0)
        End Function

        Public Shared Function FootPrintValue(ByVal ClassName As String, ByVal FootPrintType As String) As Integer
            If dtFootPrint.Columns.Count = 0 Then Return 0
            Dim aRows() As DataRow = dtFootPrint.Select(String.Format("Module='{0}'", ClassName))
            If aRows.Length = 0 Then
                Return 0
            Else
                Return aRows(0)(FootPrintType)
            End If
        End Function

        Public Shared Sub LoadResourceAllocation(ByVal oDoc As XmlDocument)
            Dim strTable As String = CodeGen.GetTemplate(oDoc, "VGDDCodeTemplate/ProjectTemplates/ResourceAllocation")
            'Dim asm As System.Reflection.Assembly = Assembly.GetExecutingAssembly
            For Each strLine As String In strTable.Split(vbCrLf)
                strLine = strLine.Trim
                If strLine <> String.Empty Then
                    Do While strLine.Contains("  ")
                        strLine = strLine.Replace("  ", " ")
                    Loop
                    Dim aData As String() = strLine.Split(" ")
                    Select Case aData(0)
                        Case ""
                        Case "Module"
                        Case "PrimitivesLayer", "GOL"
                        Case Else
                            Dim oRow As DataRow = dtFootPrint.NewRow
                            oRow("Module") = aData(0)
                            oRow("HeapPIC24") = aData(1)
                            oRow("HeapPIC32") = aData(2)
                            oRow("HeapItem") = aData(3)
                            oRow("RAMPIC24") = aData(4)
                            oRow("RAMPIC32") = aData(5)
                            oRow("ROMPIC24") = aData(6)
                            oRow("ROMPIC32") = aData(7)
                            dtFootPrint.Rows.Add(oRow)
                    End Select
                End If
            Next
        End Sub

        Public Shared Function AddInclude(ByVal CodeIn As String, ByVal NewInclude As String) As String
            Return AddInclude(CodeIn, NewInclude, "#include")
        End Function

        Public Shared Function AddInclude(ByVal CodeIn As String, ByVal NewInclude As String, ByVal Marker As String) As String
            Dim intPos As Integer = CodeIn.LastIndexOf(Marker)
            If intPos < 0 Then
                intPos = 0
            End If
            If Not CodeIn.EndsWith(vbCrLf) Then
                CodeIn &= vbCrLf
            End If
            intPos += CodeIn.Substring(intPos).IndexOf(vbCrLf)
            Return CodeIn.Substring(0, intPos + 2) & NewInclude & CodeIn.Substring(intPos)
        End Function

        Public Shared Sub LoadCodeGenTemplates()
            Try
                If Mal.MalPath Is Nothing OrElse Mal.MalPath = String.Empty OrElse Mal.CodeGenTemplateFileName Is Nothing Then Exit Sub
                If Mal.CodeGenTemplateFileName Is Nothing Then
                    Mal.CheckMalVersion(Mal.MalPath)
                End If
                dtFootPrint = New DataTable
                dtFootPrint.Columns.Add("Module", GetType(System.String))
                dtFootPrint.Columns.Add("HeapPIC24", GetType(System.String))
                dtFootPrint.Columns.Add("HeapPIC32", GetType(System.String))
                dtFootPrint.Columns.Add("HeapItem", GetType(System.String))
                dtFootPrint.Columns.Add("RAMPIC24", GetType(System.String))
                dtFootPrint.Columns.Add("RAMPIC32", GetType(System.String))
                dtFootPrint.Columns.Add("ROMPIC24", GetType(System.String))
                dtFootPrint.Columns.Add("ROMPIC32", GetType(System.String))

                Try
                    'Dim sr As StreamReader = New StreamReader(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), _
                    '                    "CodeGenTemplates.xml"))
                    Dim sr As StreamReader
#If CONFIG = "Debug" Then
                    Dim strLocalFile As String = Path.GetFullPath(Path.Combine(Application.ExecutablePath, "..\..\..\..\VGDDCommon\CodeGen\" & Mal.CodeGenTemplateFileName))
                    If File.Exists(strLocalFile) Then
                        sr = New StreamReader(strLocalFile)
                    Else
                        sr = New StreamReader(Common.GetResourceStream(Mal.CodeGenTemplateFileName, System.Reflection.Assembly.GetExecutingAssembly()))
                    End If

#Else
                    sr = New StreamReader(Common.GetResourceStream(Mal.CodeGenTemplateFileName, Assembly.GetExecutingAssembly))
#End If
                    Dim xml As String = sr.ReadToEnd
                    sr.Close()
                    XmlTemplatesDoc = Nothing
                    XmlTemplatesDoc = New XmlDocument
                    XmlTemplatesDoc.PreserveWhitespace = False
                    XmlTemplatesDoc.LoadXml(xml)

                Catch ex As Exception
                    MessageBox.Show("Error accessing CodeGen Templates: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End Try

                Try
                    LoadResourceAllocation(XmlTemplatesDoc)
                Catch ex As Exception
                    MessageBox.Show("Error Loading Resource Allocation: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

                If ExternalWidgetsHandler.ExternalWidgets.Count > 0 Then
                    For Each oExW As ExternalWidget In ExternalWidgetsHandler.ExternalWidgets.Values
                        Try
                            Dim oStream As Stream = VirtualFabUtils.Utils.GetResourceStream("CodeGenTemplates.xml", oExW.Assembly)
                            If oStream IsNot Nothing Then
                                Dim sr As StreamReader = New StreamReader(oStream)
                                Dim xml As String = sr.ReadToEnd
                                sr.Close()
                                Dim XmlWidgetTemplateDoc As New XmlDocument
                                XmlWidgetTemplateDoc.LoadXml(xml)
                                For Each oNode As XmlNode In XmlWidgetTemplateDoc.SelectNodes("VGDDCodeTemplate/ControlsTemplates/*")
                                    Dim oMainNode As XmlNode = XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/ControlsTemplates/" & oNode.Name)
                                    If oMainNode Is Nothing Then
                                        'oMainNode = oNode.CloneNode(True)
                                        oMainNode = XmlTemplatesDoc.ImportNode(oNode, True)
                                        XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/ControlsTemplates").AppendChild(oMainNode)
                                    Else
                                        XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/ControlsTemplates").ReplaceChild(oNode, oMainNode)
                                    End If
                                Next
                                LoadResourceAllocation(XmlWidgetTemplateDoc)
                            End If
                        Catch ex As Exception
                            MessageBox.Show("Error Loading External Widget " & oExW.Name & ": " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    Next
                End If

                Common.dtActions = New DataTable
                With Common.dtActions
                    .Columns.Add("ControlType", GetType(System.String))
                    .Columns.Add("ActionName", GetType(System.String))
                    .Columns.Add("ActionCode", GetType(System.String))
                    .Columns.Add("ActionHelp", GetType(System.String))
                End With
                Try
                    For Each oControlNode As XmlNode In XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/ControlsTemplates").ChildNodes
                        If Not oControlNode.NodeType = XmlNodeType.Whitespace Then
                            Dim oActionNodes As XmlNode = XmlTemplatesDoc.SelectSingleNode(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Actions", oControlNode.Name))
                            If oActionNodes IsNot Nothing Then
                                For Each oActionNode As XmlNode In oActionNodes.ChildNodes
                                    If oActionNode.NodeType = XmlNodeType.Text Or oActionNode.NodeType = XmlNodeType.CDATA Or oActionNode.NodeType = XmlNodeType.Element Then
                                        Dim oRow As DataRow = Common.dtActions.NewRow
                                        oRow("ControlType") = oControlNode.Name
                                        oRow("ActionName") = oActionNode.Attributes("Name").Value
                                        oRow("ActionCode") = oActionNode.Attributes("Code").Value
                                        If oActionNode.Attributes("Help") IsNot Nothing Then
                                            oRow("ActionHelp") = oActionNode.Attributes("Help").Value
                                        End If
                                        Common.dtActions.Rows.Add(oRow)
                                        'Else
                                        '    Debug.Print(oActionNode.NodeType.ToString)
                                    End If
                                Next
                            End If
                        End If
                    Next
                Catch ex As Exception
                    MessageBox.Show("Error reading CodeGen template: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

                Try
                    If VGDDCustom.XmlCustomTemplatesDoc Is Nothing Then
                        VGDDCustom.LoadCustomTemplatesDoc()
                    End If
                    If VGDDCustom.XmlCustomTemplatesDoc IsNot Nothing Then
                        For Each oControlNode As XmlNode In VGDDCustom.XmlCustomTemplatesDoc.DocumentElement.ChildNodes
                            Dim oActionNodes As XmlNode = oControlNode.SelectSingleNode("Actions")
                            If oActionNodes IsNot Nothing Then
                                For Each oActionNode As XmlNode In oActionNodes.ChildNodes
                                    If Not oActionNode.NodeType = XmlNodeType.Whitespace Then
                                        Dim oRow As DataRow = Common.dtActions.NewRow
                                        oRow("ControlType") = oControlNode.Name
                                        oRow("ActionName") = oActionNode.Attributes("Name").Value
                                        oRow("ActionCode") = oActionNode.Attributes("Code").Value
                                        Common.dtActions.Rows.Add(oRow)
                                    End If
                                Next
                            End If
                        Next
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error reading Custom template: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            Catch ex As Exception
                MessageBox.Show("Error loading templates for " & Mal.CodeGenTemplateFileName & ": " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

#If Not PlayerMonolitico Then

        ''#If CONFIG = "DemoRelease" Or CONFIG = "DemoDebug" Then
        'Public Shared Function Scramble(ByVal strCode As String) As String
        '    Dim aCodeLines As String() = strCode.Split(vbCrLf)
        '    Dim sb As New Text.StringBuilder
        '    Dim intLine As Integer = 0
        '    Dim intLineStart As Integer = aCodeLines.Length * 10 / 100
        '    Dim intLineEnd As Integer = aCodeLines.Length * 90 / 100
        '    Scramble = ""

        '    For Each CodeRow As String In aCodeLines
        '        intLine += 1
        '        If CodeRow.Trim.Length > 0 AndAlso intLine >= intLineStart AndAlso intLine <= intLineEnd AndAlso Rnd(1) < 0.5 Then
        '            Dim intPosStart = CodeRow.Length * 50 / 100
        '            Dim intPosEnd = CodeRow.Length * 75 / 100
        '            CodeRow = CodeRow.Substring(0, intPosStart) & "..." & CodeRow.Substring(intPosEnd)
        '        End If
        '        sb.Append(CodeRow)
        '    Next
        '    Return sb.ToString
        'End Function
        ''#End If

        Public Shared Function GetEventsFromTemplate(ByRef oNode As XmlNode) As VGDDEventsCollection
            If oNode Is Nothing Then Return Nothing
            Dim intNumEvents As Integer
            For Each oEventNode As XmlNode In oNode.ChildNodes
                If oEventNode.GetType.ToString <> "System.Xml.XmlWhitespace" Then
                    intNumEvents += 1
                End If
            Next
            Dim Events As New VGDDEventsCollection
            Dim i As Integer = 0
            For Each oEventNode As XmlNode In oNode.ChildNodes
                If oEventNode.GetType.ToString <> "System.Xml.XmlWhitespace" AndAlso oEventNode.Attributes IsNot Nothing AndAlso oEventNode.Attributes("Name") IsNot Nothing Then
                    Dim oEvent As New VGDDEvent
                    oEvent.Name = oEventNode.Attributes("Name").Value
                    If oEventNode.Attributes("LegacyName") IsNot Nothing Then
                        oEvent.LegacyName = oEventNode.Attributes("LegacyName").Value
                    End If
                    oEvent.Description = oEventNode.Attributes("Description").Value
                    If oEventNode.Attributes("PlayerEvent") IsNot Nothing Then
                        oEvent.PlayerEvent = oEventNode.Attributes("PlayerEvent").Value
                    End If
                    Events.Add(oEvent)
                    i += 1
                End If
            Next
            Return Events
        End Function

        Public Shared Sub RenameEvents(ByVal objWithEvents As IVGDDEvents)
            Dim oTemplateEvents As VGDDEventsCollection
            If TypeOf (objWithEvents) Is VGDD.VGDDScreen Then
                oTemplateEvents = CodeGen.GetEventsFromTemplate("Screen")
            Else
                oTemplateEvents = CodeGen.GetEventsFromTemplate(objWithEvents.GetType.ToString.Split(".")(1))
            End If
            If oTemplateEvents Is Nothing Then
                oTemplateEvents = New VGDDEventsCollection
            End If
            For Each oEvent As VGDDEvent In objWithEvents.VGDDEvents
                For Each oTemplateEvent As VGDDEvent In oTemplateEvents
                    If oTemplateEvent.LegacyName = oEvent.Name Then
                        oEvent.Name = oTemplateEvent.Name
                        Exit For
                    End If
                Next
            Next
        End Sub


        Public Shared Function GetEventsFromTemplate(ByRef ControlName As String) As VGDDEventsCollection
            If XmlTemplatesDoc IsNot Nothing Then
                Try
                    Dim oNode As XmlNode = XmlTemplatesDoc.SelectSingleNode(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Events", ControlName))
                    Return CodeGen.GetEventsFromTemplate(oNode)
                Catch ex As Exception
                End Try
            End If
            Return New VGDDEventsCollection
        End Function

        Public Shared Sub Clear(ByVal ProjectName As String)
            Try
                NumId = 0
                NumScreens = 0
                StateNode = Nothing
                AlignmentNode = Nothing
                StateMentNode = Nothing
                _ProjectName = Common.ProjectName
                CodeHead = String.Empty
                CodeEventsHelper = String.Empty
                Code = String.Empty
                CodeFoot = String.Empty
                AllScreensEventMsgCode = String.Empty
                AllScreensEventDrawCode = String.Empty
                Headers = String.Empty
                HeadersIncludes = String.Empty
                ScreenStates = String.Empty
                sbCodeBitmap = New Text.StringBuilder
                GraphicsConfigDefines = String.Empty
                Errors = String.Empty

                TextDeclareCodeHeadTemplate(0) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/ConstXcharArray/CodeHead", 0)
                TextDeclareCodeHeadTemplate(1) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/RamXcharArray/CodeHead", 0)
                TextDeclareCodeHeadTemplate(2) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/ExternXcharPointer/CodeHead", 0)
                TextDeclareCodeHeadTemplate(3) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/ExternRamXCharArray/CodeHead", 0)
                TextDeclareHeaderTemplate(0) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/ConstXcharArray/Header", 0)
                TextDeclareHeaderTemplate(1) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/RamXcharArray/Header", 0)
                TextDeclareHeaderTemplate(2) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/ExternXcharPointer/Header", 0)
                TextDeclareHeaderTemplate(3) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/TextDeclare/ExternRamXCharArray/Header", 0)
                'MatrixDeclareTemplate(0) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/MatrixTextDeclare/ConstXcharArray", 0)
                'MatrixDeclareTemplate(1) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/MatrixTextDeclare/RamXcharArray", 0)
                'MatrixDeclareTemplate(2) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/MatrixTextDeclare/ExternXcharPointer", 0)
                'MatrixDeclareTemplate(3) = GetTemplate("VGDDCodeTemplate/ProjectTemplates/MatrixTextDeclare/ExternRamXCharArray", 0)
                ControlHeadEventTemplate = GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/ControlHead", 1)
                ControlEventTemplate = GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/Event", 1)
                ControlFootEventTemplate = GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/ControlFoot", 1)
                If Common.ProjectMultiLanguageTranslations > 0 Then
                    WidgetsTextTemplateCode = GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/WidgetsText/Translations/Code", 0)
                    WidgetsTextTemplateHeader = GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/WidgetsText/Translations/Header", 0)
                Else
                    WidgetsTextTemplateCode = GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/WidgetsText/NoTranslations/Code", 0)
                    WidgetsTextTemplateHeader = GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/WidgetsText/NoTranslations/Header", 0)
                End If
                Do While WidgetsTextTemplateCode.StartsWith(vbCrLf)
                    WidgetsTextTemplateCode = WidgetsTextTemplateCode.Substring(2)
                Loop
                Do While WidgetsTextTemplateCode.EndsWith(vbCrLf)
                    WidgetsTextTemplateCode = WidgetsTextTemplateCode.Substring(0, WidgetsTextTemplateCode.Length - 2)
                Loop
                Dim oCodeGenStartNodes As XmlNode = XmlTemplatesDoc.SelectSingleNode("VGDDCodeTemplate/ProjectTemplates/CodeGenStart")
                If oCodeGenStartNodes IsNot Nothing AndAlso oCodeGenStartNodes.HasChildNodes Then
                    For Each oNode As XmlNode In oCodeGenStartNodes.ChildNodes
                        Select Case oNode.Name
                            Case "ExcludeAllFilesInFolder"
                                Dim strFolder As String = oNode.Attributes("Folder").Value
                                MplabX.MplabXExcludeAllFilesInFolders(strFolder)
                            Case "IncludeFile"
                                Dim strFolder As String = oNode.Attributes("Folder").Value
                                Dim strFile As String = oNode.Attributes("File").Value
                                MplabX.MplabXIncludeFile(Common.MplabXSelectedConfig, strFile)
                        End Select
                    Next
                End If

                Common.ProjectUseAlphaBlend = False
                For Each oScheme As VGDDScheme In Common._Schemes.Values
                    If oScheme.Name = "DesignScheme" Then Continue For
                    Common.SelectedScheme = oScheme
                    If oScheme.Font.Name IsNot Nothing Then
                        Dim strBackgroundBitmapStatement As String
                        If oScheme.BackgroundImageName = String.Empty OrElse oScheme.BackgroundType <> VGDDScheme.GFX_BACKGROUND_TYPE.GFX_BACKGROUND_IMAGE Then
                            strBackgroundBitmapStatement = "NULL"
                        Else
                            strBackgroundBitmapStatement = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & oScheme.BackgroundImageName
                        End If
                        CodeHead &= GetTemplate("VGDDCodeTemplate/ProjectTemplates/CreateSchemes/CodeHead", 0) _
                            .Replace("[SCHEME_NAME]", oScheme.Name)
                        Code &= GetTemplate("VGDDCodeTemplate/ProjectTemplates/CreateSchemes/Code", -1, -1) _
                            .Replace("[SCHEME_NAME]", oScheme.Name) _
                            .Replace("[COLOR0]", Color2Num(oScheme.Color0, False, "Scheme " & oScheme.Name & ".Color0")).Replace("[COLOR0_STRING]", Color2String(oScheme.Color0)) _
                            .Replace("[COLOR1]", Color2Num(oScheme.Color1, False, "Scheme " & oScheme.Name & ".Color1")).Replace("[COLOR1_STRING]", Color2String(oScheme.Color1)) _
                            .Replace("[COLORDISABLED]", Color2Num(oScheme.Colordisabled, False, "Scheme " & oScheme.Name & ".Colordisabled")).Replace("[COLORDISABLED_STRING]", Color2String(oScheme.Colordisabled)) _
                            .Replace("[COMMONBKCOLOR]", Color2Num(oScheme.Commonbkcolor, False, "Scheme " & oScheme.Name & ".Commonbkcolor")).Replace("[COMMONBKCOLOR_STRING]", Color2String(oScheme.Commonbkcolor)) _
                            .Replace("[EMBOSSDKCOLOR]", Color2Num(oScheme.Embossdkcolor, False, "Scheme " & oScheme.Name & ".Embossdkcolor")).Replace("[EMBOSSDKCOLOR_STRING]", Color2String(oScheme.Embossdkcolor)) _
                            .Replace("[EMBOSSLTCOLOR]", Color2Num(oScheme.Embossltcolor, False, "Scheme " & oScheme.Name & ".Embossltcolor")).Replace("[EMBOSSLTCOLOR_STRING]", Color2String(oScheme.Embossltcolor)) _
                            .Replace("[TEXTCOLOR0]", Color2Num(oScheme.Textcolor0, False, "Scheme " & oScheme.Name & ".Textcolor0")).Replace("[TEXTCOLOR0_STRING]", Color2String(oScheme.Textcolor0)) _
                            .Replace("[TEXTCOLOR1]", Color2Num(oScheme.Textcolor1, False, "Scheme " & oScheme.Name & ".Textcolor1")).Replace("[TEXTCOLOR1_STRING]", Color2String(oScheme.Textcolor1)) _
                            .Replace("[TEXTCOLORDISABLED]", Color2Num(oScheme.Textcolordisabled, False, "Scheme " & oScheme.Name & ".Textcolordisabled")).Replace("[TEXTCOLORDISABLED_STRING]", Color2String(oScheme.Textcolordisabled)) _
                            .Replace("[FONT_NAME]", oScheme.Font.Name.Replace("-", "_")) _
                            .Replace("[GRADIENTTYPE]", oScheme.GradientType.ToString) _
                            .Replace("[GRADIENTSTARTCOLOR]", Color2Num(oScheme.GradientStartColor, False, "Scheme " & oScheme.Name & ".GradientStartColor")).Replace("[GRADIENTSTARTCOLOR_STRING]", Color2String(oScheme.GradientStartColor)) _
                            .Replace("[GRADIENTENDCOLOR]", Color2Num(oScheme.GradientEndColor, False, "Scheme " & oScheme.Name & ".GradientEndColor")).Replace("[GRADIENTENDCOLOR_STRING]", Color2String(oScheme.GradientEndColor)) _
                            .Replace("[GRADIENTLENGTH]", oScheme.GradientLength) _
                            .Replace("[FILLSTYLE]", oScheme.FillStyle.ToString) _
                            .Replace("[ALPHAVALUE]", oScheme.AlphaValue) _
                            .Replace("[EMBOSS_SIZE]", oScheme.EmbossSize) _
                            .Replace("[COMMONBKLEFT]", oScheme.CommonBkLeft) _
                            .Replace("[COMMONBKTOP]", oScheme.CommonBkTop) _
                            .Replace("[COMMONBKTYPE]", oScheme.BackgroundType.ToString) _
                            .Replace("[COMMONBKIMAGE]", strBackgroundBitmapStatement)
                        Headers &= GetTemplate("VGDDCodeTemplate/ProjectTemplates/CreateSchemes/Header", 0) _
                            .Replace("[SCHEME_NAME]", oScheme.Name)
                        If oScheme.FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_ALPHA_COLOR Then 'Or oScheme.BackgroundType = VGDDScheme.GFX_BACKGROUND_TYPE.GFX_BACKGROUND_IMAGE
                            Common.ProjectUseAlphaBlend = True
                        End If
                    End If
                Next

                For Each oScreenAttr As VGDD.VGDDScreenAttr In aScreens.Values
                    oScreenAttr.EventsCode = ""
                    CodeGen.CheckScreenTexts(oScreenAttr.Screen)
                Next
                FootPrint.Clear()
                FootPrint.Heap = Common._Schemes.Count * 32
                IPU_MAX_COMPRESSED_BUFFER_SIZE = 0
                IPU_MAX_DECOMPRESSED_BUFFER_SIZE = 0


            Catch ex As Exception
                MessageBox.Show("Cannot clear environment. Please exit and re-launch the application to avoid further issues.", "Problems cleaning environment", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try

        End Sub

        Public Shared Function UInt162Hex(ByVal Val As UInt16) As String
            Return String.Format("0x{0:x2}{1:x2}", CType((Val >> 8) And &HFF, Byte), CType(Val And &HFF, Byte))
        End Function

        Public Shared Function StringPoolIndex(ByVal StringID As Integer) As Integer
            Return StringID - 1
            'Dim intIndex As Integer = 0
            'For Each oStringSet As VGDDCommon.MultiLanguageStringSet In VGDDCommon.Common.ProjectStringPool.Values
            '    If oStringSet.StringID = StringID Then
            '        Return intIndex
            '    End If
            '    intIndex += 1
            'Next
            'Return -1
        End Function

        Public Shared Function QText(ByVal strIn As String, ByVal Font As VGDDFont, ByRef MyText As String) As String
            Return QText(strIn, Font, MyText, False)
        End Function

        Public Shared Function QText(ByVal strIn As String, ByVal Font As VGDDFont, ByRef MyText As String, ByVal blnNoFontFilter As Boolean) As String
            Dim intConversionType As Integer = 1

            If strIn Is Nothing Then
                MyText = "NULL"
                Return IIf(Common.ProjectUseMultiByteChars, "L", "") & "{0}"
            End If
            If Font Is Nothing Then
                'strIn = "NULL FONT!"
                Font = Common._Fonts(0)
            End If

            FootPrint.ROMStrings += (strIn.Length + 1) * IIf(Common.ProjectUseMultiByteChars, 2, 1)

            If Font.Charset = VGDDFont.FontCharset.SELECTION And Not blnNoFontFilter Then
                Dim strHexFormat As String
                strIn = strIn.Replace("\n", vbLf) '.Replace("|", vbLf)
                If Common.ProjectUseMultiByteChars Then
                    strHexFormat = "0x{0:X4}"
                Else
                    strHexFormat = "0x{0:X2}"
                End If
                QText = ""
                If strIn <> String.Empty Then
                    For Each c As Char In strIn
                        Dim idx As Integer
                        If AscW(c) < 32 Or AscW(c) = 123 Or AscW(c) = 125 Then 'CR LF ...
                            QText &= String.Format(strHexFormat, AscW(c)) & ","
                        Else
                            idx = Array.IndexOf(Font.CharsIncluded, c) '+ 1
                            If idx >= 0 Then
                                QText &= String.Format(strHexFormat, idx + 32) & "," '+ Asc(Font.CharsIncluded(0)) - 1
                            End If
                        End If
                    Next
                End If
                MyText = strIn.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, "\n")
                Return "{" & QText & String.Format(strHexFormat, 0) & "}"
            Else 'VGDDFont.FontCharset.RANGE
                Dim strOutOfRangeChars As String = String.Empty
                Dim intMaxRange As Integer = 0
                Dim strAscii As String = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(strIn))
                If strIn <> strAscii Then ' Or strIn.Contains("{") Or strIn.Contains("}") Then
                    For j = 0 To strIn.Length - 1
                        Dim intAsc As Integer = Asc(strIn.Substring(j, 1))
                        If intAsc = 63 OrElse (intAsc <> 13 AndAlso intAsc <> 10 AndAlso (intAsc > Font.EndChar OrElse intAsc < Font.StartChar)) Then
                            strOutOfRangeChars &= strIn.Substring(j, 1) & "(" & intAsc.ToString & ") "
                            If intMaxRange < intAsc Then intMaxRange = intAsc
                            'Exit For
                        End If
                    Next j
                    If strOutOfRangeChars <> String.Empty Then
                        Dim strWarning As String = String.Format("Warning: the string {0} with Font {1} contains the following out-of-range chars:" &
                                        vbCrLf & strOutOfRangeChars & vbCrLf & "Highest char code: {2} (0x{2:X4})", strIn, Font.Name, intMaxRange) & vbCrLf
                        If Not Errors.Contains(strWarning) Then
                            Errors &= strWarning
                        End If
                    End If
                    QText = ""
                    MyText = strIn.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, "\n")
                    If strIn <> String.Empty Then
                        Dim UnicodeBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(strIn)
                        For i As Integer = 0 To UnicodeBytes.Length - 1 Step 2
                            If UnicodeBytes(i + 1) = 0 And UnicodeBytes(i) < 128 And UnicodeBytes(i) <> 123 And UnicodeBytes(i) <> 125 Then
                                Dim c As String = Chr(UnicodeBytes(i))
                                Select Case c
                                    Case "'"
                                        c = "\'"
                                    Case "\"
                                        c = "\\"
                                    Case vbCr
                                        c = "\n"
                                        i += 2
                                End Select
                                QText &= String.Format("'{0}'", c) & ","
                            Else
                                QText &= String.Format("0x{0:X2}{1:X2}", UnicodeBytes(i + 1), UnicodeBytes(i)) & ","
                            End If
                        Next
                    End If
                    Return "{" & QText & "0x0000}"
                End If
                'No Unicode Chars
                MyText = strIn.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, "\n")
                Select Case Common.ProjectCompilerFamily
                    Case "C30"
                        Return IIf(Common.ProjectUseMultiByteChars, "L", "") & """" & MyText & """"
                    Case "C32"
                        If Common.ProjectUseMultiByteChars Then
                            Return TextAsArray(strIn)
                        Else
                            Return """" & MyText & """"
                        End If
                End Select
                Return """" & MyText & """"
            End If
        End Function

        Public Shared Function QtextBinary(ByVal strArrayText As String, ByVal strTranslations As String, ByVal strVisibleText As String, ByVal i As Integer, ByVal stringLen As Integer, ByVal strComma As String) As String
            If strArrayText.StartsWith("{") Then
                strTranslations &= CodeGen.GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/TranslationsTemplate/MultiChar", 0, 0).Replace(vbCrLf, "")
            Else
                strTranslations &= CodeGen.GetTemplate("VGDDCodeTemplate/StringsPoolTemplate/TranslationsTemplate/NoMultiChar", 0, 0).Replace(vbCrLf, "")
            End If
            Return strTranslations.Replace("[STRING]", strArrayText) _
                    .Replace("[COMMA]", strComma) _
                    .Replace("[INDENT]", IIf(i > 0, "[TAB]  ", "")) _
                    .Replace("[QTEXT]", strVisibleText) _
                    .Replace("[STRINGLEN]", stringLen) _
                    .Replace("[STRINGLEN+1]", stringLen + 1)
        End Function

        Public Shared Function TextAsArray(ByVal strIn As String) As String
            Dim strArray As String = String.Empty
            Dim UnicodeBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(strIn)
            'For Each c As Byte In UnicodeBytes
            For i As Integer = 0 To UnicodeBytes.Length - 1 Step 2
                If UnicodeBytes(i + 1) = 0 And UnicodeBytes(i) < 128 Then
                    Dim c As String = Chr(UnicodeBytes(i))
                    Select Case c
                        Case "'"
                            c = "\'"
                        Case "\"
                            c = "\\"
                        Case vbCr
                            c = "\n"
                            i += 2
                    End Select
                    strArray &= "'" & c & "',"
                Else
                    strArray &= String.Format("0x{0:X2}{1:X2},", UnicodeBytes(i + 1), UnicodeBytes(i))
                End If
            Next
            Return "{" & strArray & "0x0000}"
        End Function

        Public Shared Sub EventsInsertScreenCode(ByVal oWidget As Control, ByVal oScreen As VGDD.VGDDScreen, ByVal ControlIdNoIndex As String, ByVal ControlIdIndex As String)
            For Each oScreenEventNode As Xml.XmlNode In CodeGen.XmlTemplatesDoc. _
                SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/ScreenCode", oWidget.GetType.ToString.Split(".")(1)))
                Dim oAttribute As Xml.XmlAttribute = oScreenEventNode.Attributes("Event")
                If oAttribute IsNot Nothing Then
                    Dim strEventName As String = oAttribute.Value
                    Dim oScreenEvent As VGDDEvent = oScreen.VGDDEvents(strEventName)
                    If oScreenEvent Is Nothing Then
                        oScreenEvent = New VGDDEvent
                        oScreenEvent.Name = strEventName
                    End If
                    Dim strEventCode As String = oScreenEventNode.InnerText.Replace("[CONTROLID_NOINDEX]", ControlIdNoIndex) _
                                    .Replace("[CONTROLID_INDEX]", ControlIdIndex)
                    If Not oScreenEvent.Code.Contains(strEventCode) Then
                        oScreenEvent.Code &= vbCrLf & strEventCode
                        oScreenEvent.Handled = True
                        If oScreen.VGDDEvents(strEventName) Is Nothing Then
                            oScreen.VGDDEvents.Add(oScreenEvent)
                        End If
                    End If
                End If
            Next
        End Sub

        Public Shared Sub EventsToCode(ByVal ControlID As String, ByRef oWidget As IVGDDWidget)
            Dim ControlIdNoIndex As String = ControlID.Split("[")(0)
            Dim ControlIdIndex As String = "", ControlIdIndexPar As String = ""
            Dim ControlEventCode As String = "", ControlHeadEventCode As String = "", ControlFootEventCode As String = ""

            If oWidget.VGDDEvents Is Nothing Then Exit Sub
            If ControlID <> ControlIdNoIndex Then
                ControlIdIndexPar = ControlID.Substring(ControlIdNoIndex.Length)
                ControlIdIndex = ControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            For Each oEvent As VGDDEvent In oWidget.VGDDEvents
                If oEvent.Handled Then
                    If ControlHeadEventCode = "" Then
                        ControlHeadEventCode = CodeGen.ControlHeadEventTemplate.Replace("[CONTROLID]", ControlID) _
                            .Replace("[CONTROLID_NOINDEX]", ControlIdNoIndex) _
                            .Replace("[CONTROLID_INDEX]", ControlIdIndex) _
                            .Replace("[CONTROLID_INDEXPAR]", ControlIdIndexPar)
                    End If
                    Dim strEventCode As String = EventCodeExpandMacros(oEvent, ControlID, oWidget)
                    Do While strEventCode.EndsWith(vbCrLf)
                        strEventCode = strEventCode.Substring(0, strEventCode.Length - 2)
                    Loop
                    Dim strLine As String = ControlEventTemplate.Substring(0, ControlEventTemplate.IndexOf("[CONTROLEVENTCODE]"))
                    strLine = strLine.Substring(strLine.LastIndexOf(vbCrLf) + 2)
                    Dim intIndent As Integer = strLine.Length
                    Dim aEventCode() As String = strEventCode.Split(vbCr)
                    For i As Integer = 1 To aEventCode.Length - 1
                        aEventCode(i) = Space(intIndent) & aEventCode(i).Replace(vbLf, "")
                    Next
                    strEventCode = String.Join(vbCrLf, aEventCode)

                    ControlEventCode &= ControlEventTemplate.Replace("[CONTROLID]", ControlID) _
                       .Replace("[EVENTMSG]", oEvent.Name) _
                       .Replace("[CONTROLEVENTCODE]", strEventCode)
                End If
            Next
            If ControlHeadEventCode <> "" Then
                ControlFootEventCode = CodeGen.ControlFootEventTemplate.Replace("[CONTROLID]", ControlID)
                ScreenEventCode &= ControlHeadEventCode & ControlEventCode & ControlFootEventCode
            End If
        End Sub

        Public Shared Function ResourcesToCode() As Boolean
            Dim oBitmap As VGDDImage = Nothing
            Dim GraphicsModel As GrcProject.GraphicsModels
            Select Case Common.ProjectColourDepth
                Case 24
                    GraphicsModel = GrcProject.GraphicsModels.BPP24
                Case Else
                    GraphicsModel = GrcProject.GraphicsModels.BPP16
            End Select
            Common.oGrcProjectInternal = New GrcProject(Common.ProjectCompiler, GrcProject.OutputFormats.CArray, GraphicsModel)
            Common.oGrcProjectExternal = New GrcProject(Common.ProjectCompiler, GrcProject.OutputFormats.Hex, GraphicsModel)
            Common.oGrcBinBmpOnSd = New GrcProject(Common.ProjectCompiler, GrcProject.OutputFormats.Binary, GraphicsModel)
            Common.BitmapsBinUsed = 0
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    MplabX.GraphicsConfigDisableDefine("USE_BITMAP_FLASH")
                    MplabX.GraphicsConfigDisableDefine("USE_BITMAP_EXTERNAL")
                    MplabX.GraphicsConfigDisableDefine("USE_FONT_FLASH")
                    MplabX.GraphicsConfigDisableDefine("USE_FONT_EXTERNAL")
                    MplabX.GraphicsConfigDisableDefine("USE_COMP_RLE")
                    MplabX.GraphicsConfigDisableDefine("USE_COMP_IPU")
                Case "MLA"
                Case "HARMONY"
            End Select

            Dim strTempPath As String = Path.Combine(Common.VGDDProjectPath, "GRC_Input")
            If Not Directory.Exists(strTempPath) Then
                Try
                    Directory.CreateDirectory(strTempPath)
                Catch ex As Exception
                    strTempPath = Environment.GetEnvironmentVariable("TEMP")
                End Try
            End If
            For Each oBitmap In _Bitmaps
                Common.CodegenProgress += 1
                Application.DoEvents()
                If oBitmap.Bitmap Is Nothing Then
                    Throw New Exception("Bitmap " & oBitmap.Name & " is not defined!")
                Else
                    If oBitmap.Referenced Then
                        Select Case oBitmap.CompressionType
                            Case GrcProject.CompressionTypes.RLE
                                Select Case Mal.FrameworkName.ToUpper
                                    Case "MLALEGACY"
                                        MplabX.GraphicsConfigEnableDefine("USE_COMP_RLE")
                                    Case "MLA"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_RLE_DECODE_DISABLE")
                                    Case "HARMONY"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_RLE_DECODE_DISABLE")
                                End Select

                            Case GrcProject.CompressionTypes.IPU
                                MplabX.GraphicsConfigEnableDefine("USE_COMP_IPU")
                            Case GrcProject.CompressionTypes.None
                        End Select

                        Dim strTempFilename As String = Path.Combine(strTempPath, oBitmap.Name) & ".bmp"
                        If oBitmap.AllowScaling Then
                            If oBitmap.TransparentBitmap.PixelFormat.ToString.Contains("32bpp") Then
                                Debug.Print("!")
                            End If
                            If (oBitmap.ToBeConverted AndAlso Not Common.QuickCodeGen) Or Not FileExistsCaseSensitive(strTempFilename) Then
                                Dim strResult As String = oBitmap.Save(strTempFilename, Drawing.Imaging.ImageFormat.Bmp)
                                If strResult <> "OK" Then
                                    MessageBox.Show(String.Format("Error writing {0}. Please ensure the file is not in use and/or that the folder is writable." & vbCrLf & "Error: " & strResult, strTempFilename), "Write error on GRC_Input path")
                                End If
                            End If
                        Else
                            strTempFilename = oBitmap.FileName
                        End If

                        Dim oGrcResource As New GrcResource
                        With oGrcResource
                            .InputFile = strTempFilename
                            .ResourceType = GrcProject.ResourceTypes.Bitmap
                            .ResultingLabel = IIf(Common.ProjectUseBmpPrefix, "bmp", "") & oBitmap.Name
                            .CompressionType = oBitmap.CompressionType
                        End With
                        Select Case oBitmap.Type
                            Case VGDDImage.PictureType.FLASH_VGDD, VGDDImage.PictureType.FLASH
                                Select Case Mal.FrameworkName.ToUpper
                                    Case "MLALEGACY"
                                        MplabX.GraphicsConfigEnableDefine("USE_BITMAP_FLASH")
                                    Case "MLA"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_IMAGE_FLASH_DISABLE")
                                    Case "HARMONY"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_IMAGE_FLASH_DISABLE")
                                End Select
                            Case VGDDImage.PictureType.EXTERNAL_VGDD, VGDDImage.PictureType.EXTERNAL_VGDD_BIN, VGDDImage.PictureType.EXTERNAL
                                Select Case Mal.FrameworkName.ToUpper
                                    Case "MLALEGACY"
                                        MplabX.GraphicsConfigEnableDefine("USE_BITMAP_EXTERNAL")
                                    Case "MLA"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_IMAGE_EXTERNAL_DISABLE")
                                    Case "HARMONY"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_IMAGE_EXTERNAL_DISABLE")
                                End Select
                        End Select

                        Select Case oBitmap.Type
                            Case VGDDImage.PictureType.FLASH_VGDD
                                oGrcResource.OutputFormat = GrcProject.OutputFormats.CArray
                                Common.oGrcProjectInternal.AddResourceToProject(oGrcResource)
                            Case VGDDImage.PictureType.EXTERNAL_VGDD
                                oGrcResource.OutputFormat = GrcProject.OutputFormats.Hex
                                Common.oGrcProjectExternal.AddResourceToProject(oGrcResource)
                            Case VGDDImage.PictureType.EXTERNAL_VGDD_BIN
                                oGrcResource.OutputFormat = GrcProject.OutputFormats.Binary
                                Common.oGrcProjectExternal.OutputFormat = GrcProject.OutputFormats.Binary
                                Common.oGrcProjectExternal.AddResourceToProject(oGrcResource)
                            Case VGDDImage.PictureType.BINBMP_ON_SDFAT
                                If oBitmap.SDFileName IsNot Nothing Then
                                    Common.BitmapsBinUsed += 1
                                    oGrcResource.OutputFormat = GrcProject.OutputFormats.Binary
                                    Common.oGrcBinBmpOnSd.AddResourceToProject(oGrcResource)
                                    Dim strBinFileName As String = Path.Combine(Common.BitmapsBinPath, oBitmap.SDFileName)
                                    'Dim blnConvertSD As Boolean = False
                                    'If Not File.Exists(strBinFileName) Then
                                    '    blnConvertSD = True
                                    'ElseIf File.GetLastWriteTime(oBitmap.FileName) > File.GetLastWriteTime(oBitmap.SDFileName) Then
                                    '    blnConvertSD = True
                                    'End If
                                    If (oBitmap.ToBeConverted AndAlso Not Common.QuickCodeGen) OrElse Not File.Exists(strBinFileName) Then 'blnConvertSD Then
                                        oBitmap._BitmapSize = oGrcResource.ConvertSingleResource(strBinFileName, IIf(Common.ProjectCompilerFamily = "C30", GrcProject.BuildTypes.C30, GrcProject.BuildTypes.C32))
                                        'FootPrint.BitmapsFlash += oBitmap.BitmapSize
                                        Dim strFileToDelete As String
                                        strFileToDelete = Path.Combine(Common.BitmapsBinPath, Path.GetFileNameWithoutExtension(oBitmap.SDFileName) & ".c")
                                        If File.Exists(strFileToDelete) Then File.Delete(strFileToDelete)
                                        strFileToDelete = Path.Combine(Common.BitmapsBinPath, Path.GetFileNameWithoutExtension(oBitmap.SDFileName) & ".h")
                                        If File.Exists(strFileToDelete) Then File.Delete(strFileToDelete)
                                    End If
                                End If
                                'Common.BitmapOnSdHeaders &= String.Format("const IMAGE_ON_SD {0} = {{ ({3} *) &{1}, ""{2}"" }};", _
                                '                                         IIf(Common.ProjectUseBmpPrefix, "bmp", "") & oBitmap.Name, _
                                '                                         IIf(Common.ProjectUseBmpPrefix, "bmp", "") & oBitmap.Name, _
                                '                                         oBitmap.SDFileName, _
                                '                                         IIf(oBitmap.CompressionType = GrcProject.CompressionTypes.None, "IMAGE_EXTERNAL", "GFX_IMAGE_HEADER")) & vbCrLf
                        End Select
                    End If
                End If
            Next

            For Each VFont As VGDDFont In Common._Fonts
                Common.CodegenProgress += 1
                Application.DoEvents()
                Try
                    Dim strFontDef As String = "Charset: " & VFont.Charset.ToString
                    Select Case VFont.Charset
                        Case VGDDFont.FontCharset.RANGE
                            If VFont.StartChar < 0 Or VFont.EndChar < 0 Then
                                MessageBox.Show("Font " & VFont.Name & " is defined as Charset.RANGE but range is not valid: from " & VFont.StartChar & " to " & VFont.EndChar & vbCrLf & "Correct and restart CodeGen", "Wrong Font definition")
                                Continue For
                            End If
                            strFontDef &= " from " & VFont.StartChar & "(" & ChrW(VFont.StartChar) & ") to " & VFont.EndChar & "(" & ChrW(VFont.EndChar) & ")"
                        Case VGDDFont.FontCharset.SELECTION
                            Dim aFontSelectionTable As String = String.Empty
                            Array.Sort(VFont.CharsIncluded)
                            strFontDef &= " CharsIncluded: """
                            'If VFont.CharsIncluded IsNot Nothing Then
                            For Each c As Char In VFont.CharsIncluded
                                strFontDef &= c
                                aFontSelectionTable &= "," & String.Format(IIf(Common.ProjectUseMultiByteChars, "0x{0:X4}", "0x{0:X2}"), AscW(c))
                            Next
                            If aFontSelectionTable <> String.Empty Then aFontSelectionTable = aFontSelectionTable.Substring(1)
                            CodeHead &= GetTemplate("VGDDCodeTemplate/ProjectTemplates/FontsDeclare/FontFilter/CodeHead", 0) _
                                .Replace("[FILTERTABLE]", aFontSelectionTable) _
                                .Replace("[FILTERTABLE_SIZE]", VFont.CharsIncluded.Length) _
                                .Replace("[FILTERTABLE_CHARS]", strFontDef.Substring(strFontDef.IndexOf("""") + 1)) _
                                .Replace("[FONT]", VFont.Name)
                            Headers &= GetTemplate("VGDDCodeTemplate/ProjectTemplates/FontsDeclare/FontFilter/Header", 0) _
                                .Replace("[FILTERTABLE_SIZE]", VFont.CharsIncluded.Length) _
                                .Replace("[FONT]", VFont.Name)
                            strFontDef &= """ (LEN=" & VFont.CharsIncluded.Length & ")"
                            'End If
                    End Select

                    If VFont.Referenced Then
                        Dim oGrcResource As New GrcResource
                        With oGrcResource
                            .ResourceType = GrcProject.ResourceTypes.Font
                            .ResourceSubType = GrcProject.ResourceSubTypes.Installed
                            .ResultingLabel = VFont.Name
                            .VFont = VFont
                        End With
                        Select VFont.Type
                            Case VGDDFont.FontType.FLASH_VGDD, VGDDFont.FontType.FLASH
                                Select Case Mal.FrameworkName.ToUpper
                                    Case "MLALEGACY"
                                        MplabX.GraphicsConfigEnableDefine("USE_FONT_FLASH")
                                    Case "MLA"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_FONT_FLASH_DISABLE")
                                    Case "HARMONY"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_FONT_FLASH_DISABLE")
                                End Select
                            Case VGDDFont.FontType.EXTERNAL_VGDD, VGDDFont.FontType.EXTERNAL, VGDDFont.FontType.EXTERNAL_VGDD_BIN
                                Select Case Mal.FrameworkName.ToUpper
                                    Case "MLALEGACY"
                                        MplabX.GraphicsConfigEnableDefine("USE_FONT_EXTERNAL")
                                    Case "MLA"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_FONT_EXTERNAL_DISABLE")
                                    Case "HARMONY"
                                        MplabX.GraphicsConfigDisableDefine("GFX_CONFIG_FONT_EXTERNAL_DISABLE")
                                End Select
                        End Select

                        Select Case VFont.Type
                            Case VGDDFont.FontType.FLASH_VGDD
                                Common.oGrcProjectInternal.AddResourceToProject(oGrcResource)
                            Case VGDDFont.FontType.EXTERNAL_VGDD
                                oGrcResource.OutputFormat = GrcProject.OutputFormats.Hex
                                Common.oGrcProjectExternal.AddResourceToProject(oGrcResource)
                            Case VGDDFont.FontType.EXTERNAL_VGDD_BIN
                                oGrcResource.OutputFormat = GrcProject.OutputFormats.Binary
                                Common.oGrcProjectExternal.OutputFormat = GrcProject.OutputFormats.Binary
                                Common.oGrcProjectExternal.AddResourceToProject(oGrcResource)
                        End Select
                    End If
                Catch ex As Exception
                    Throw New Exception(String.Format("ResourcesToCode - Error converting Font {0} - type {1}" & vbCrLf & ex.Message, VFont.Name, VFont.Type.ToString))
                End Try
            Next

            If Common.ProjectUsePalette AndAlso Not Common.QuickCodeGen Then
                For Each oScheme As VGDDScheme In Common._Schemes.Values
                    If oScheme.Palette IsNot Nothing AndAlso oScheme.Palette.PaletteFile <> String.Empty Then
                        Dim strPalettePath As String = Path.GetFullPath(Path.Combine(Common.MplabXProjectPath, oScheme.Palette.PaletteFile))
                        If Not File.Exists(strPalettePath) Then
                            Throw New Exception(String.Format("ResourcesToCode - Palette file {0} in scheme {1} does not exist", strPalettePath, oScheme.Name))
                        End If
                        Dim oGrcResource As New GrcResource
                        With oGrcResource
                            .InputFile = strPalettePath
                            .ResourceType = GrcProject.ResourceTypes.Palette
                            .ResultingLabel = "palette_" & oScheme.Palette.Name
                            .CompressionType = GrcProject.CompressionTypes.None
                        End With
                        oGrcResource.OutputFormat = GrcProject.OutputFormats.CArray
                        Common.oGrcProjectInternal.AddResourceToProject(oGrcResource)
                    End If
                Next
            End If

            If Not Common.QuickCodeGen Then
                If Common.oGrcProjectInternal.ResourcesCount > 0 Then
                    Common.oGrcProjectInternal.WriteProject(Path.Combine(Common.VGDDProjectPath, "VGDD_GRC_" & Common.ProjectName & "_Internal.xml"))
                    If Not Common.oGrcProjectInternal.RunProject(Common.ProjectFileName_InternalResourcesC) Then
                        Return False
                    End If
                End If

                If Common.oGrcProjectExternal.ResourcesCount > 0 Then
                    Common.oGrcProjectExternal.WriteProject(Path.Combine(Common.VGDDProjectPath, "VGDD_GRC_" & Common.ProjectName & "_External.xml"))
                    If Not Common.oGrcProjectExternal.RunProject(Common.ProjectFileName_ExternalResourcesC.Replace(".c", IIf(Common.oGrcProjectExternal.OutputFormat = GrcProject.OutputFormats.Binary, ".bin", ".hex"))) Then
                        Return False
                    End If
                End If

                If Common.oGrcBinBmpOnSd.ResourcesCount > 0 Then
                    Dim strBinFile As String = Common.ProjectFileName_BmpOnSdResourcesC.Replace(".c", ".bin")
                    Common.oGrcBinBmpOnSd.WriteProject(Path.Combine(Common.VGDDProjectPath, "VGDD_GRC_" & Common.ProjectName & "_BinBmpOnSd.xml"))
                    If Not Common.oGrcBinBmpOnSd.RunProject(strBinFile) Then
                        Return False
                    End If
                    If File.Exists(strBinFile) Then File.Delete(strBinFile)

                    Dim strCodeBmpOnSdResources As String = ReadFile(Common.ProjectFileName_BmpOnSdResourcesC)
                    strCodeBmpOnSdResources = strCodeBmpOnSdResources.Replace("IMAGE_EXTERNAL", "IMAGE_ON_SD")
                    strCodeBmpOnSdResources = strCodeBmpOnSdResources.Replace("GFX_IMAGE_HEADER", "IMAGE_ON_SD")
                    Dim sb As New Text.StringBuilder
                    Dim strBitmapName As String = ""

                    Dim aLines As String() = strCodeBmpOnSdResources.Split(vbLf)
                    For i As Integer = 0 To aLines.Length - 1
                        Dim li As String = aLines(i).Replace(vbCr, "")
                        Dim intPos As Integer = li.IndexOf("IMAGE_ON_SD")
                        If intPos > 0 Then
                            strBitmapName = li.Substring(intPos + 12).Trim
                            strBitmapName = strBitmapName.Substring(0, strBitmapName.IndexOf(" "))
                            If Common.ProjectUseBmpPrefix Then
                                strBitmapName = strBitmapName.Substring(3)
                            End If
                            For Each oBitmap In Common.Bitmaps
                                If oBitmap.Name = strBitmapName Then
                                    strBitmapName = oBitmap.SDFileName
                                    Exit For
                                End If
                            Next
                        End If
                        If li.Contains("};") Then
                            If strBitmapName <> "" Then
                                Select Case oBitmap.CompressionType
                                    Case GrcProject.CompressionTypes.None
                                        sb.Append("    ,0" & vbCrLf & _
                                                "    ,0" & vbCrLf & _
                                                "    ,0" & vbCrLf & _
                                                "    ,0" & vbCrLf & _
                                                "    ,0" & vbCrLf)
                                    Case GrcProject.CompressionTypes.RLE
                                    Case GrcProject.CompressionTypes.IPU
                                End Select
                                sb.AppendLine("    ,""" & strBitmapName & """")
                                strBitmapName = ""
                            End If
                            sb.AppendLine(li & vbCrLf)
                            'If li.Contains("COMP_RLE") Then
                            '    i += 7
                            'Else
                            '    i += 2
                            'End If
                        Else
                            sb.AppendLine(li)
                        End If
                    Next i

                    Try
                        Common.WriteFileWithBackup(Common.ProjectFileName_BmpOnSdResourcesC, sb.ToString)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message, "CodeGen.ResourcesToCode")
                    End Try
                End If
            End If

            Dim BitmapArrays As New Dictionary(Of String, String)
            For Each oBitmap In _Bitmaps
                If oBitmap.CompressionType = GrcProject.CompressionTypes.None Then
                    FootPrint.BitmapsFlash += oBitmap.BitmapSize
                Else
                    FootPrint.BitmapsFlash += oBitmap.BitmapCompressedSize
                End If
                If oBitmap.UsedBy.Count > 0 Then
                    If oBitmap.GroupName <> String.Empty Then
                        If Not BitmapArrays.ContainsKey(oBitmap.GroupName) Then
                            BitmapArrays.Add(oBitmap.GroupName, GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/BitmapsGroup/GroupStart", 0) _
                                             .Replace("[GROUPNAME]", oBitmap.GroupName))
                        Else
                            BitmapArrays.Item(oBitmap.GroupName) &= ","
                        End If
                        BitmapArrays.Item(oBitmap.GroupName) &= GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/BitmapsGroup/GroupItem", 1) _
                                                          .Replace("[BITMAPNAME]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & oBitmap.Name)
                    End If
                End If
            Next

            For Each strPictureGroup As String In BitmapArrays.Keys
                CodeHead &= BitmapArrays(strPictureGroup) & vbCrLf & GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/BitmapsGroup/GroupEnd", 0)
                Headers &= GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/BitmapsGroup/GroupHeader", 0) _
                    .Replace("[GROUPNAME]", strPictureGroup) _
                    .Replace("[GROUPNAME_UPPER]", strPictureGroup.ToUpper) _
                    .Replace("[GROUP_BITMAP_COUNT]", BitmapArrays(strPictureGroup).Split(vbCrLf).Length - 3)
            Next

            If Mal.FrameworkName.ToUpper <> "HARMONY" Then
                Dim strDefaultFont As String = ""
                For Each VFont As VGDDFont In Common._Fonts
                    FootPrint.FontFlash += VFont.BinSize
                    If VFont.IsGOLFontDefault Then
                        If strDefaultFont <> "" Then
                            VFont.IsGOLFontDefault = False
                        Else
                            strDefaultFont = VFont.Name
                        End If
                    End If
                Next
                If strDefaultFont = "" Then
                    Dim VFont As VGDDFont = _Fonts(0)
                    VFont.IsGOLFontDefault = True
                    strDefaultFont = VFont.Name
                    MplabX.FileDisableDefine("GraphicsConfig.h", "FONTDEFAULT", True)
                    MplabX.GraphicsConfigEnableDefine("FONTDEFAULT " & VFont.Name)
                    ProjectChanged = True
                    MessageBox.Show("None of the used fonts is marked as GOL DefaultFont." & vbCrLf & _
                                    strDefaultFont & " has been elected as GOL default font." & vbCrLf & _
                                    "If you wish a different font to be used, please set its IsGOLDefaultFont to true and restart CodeGen", "Warning - No GOL Default Font defined")
                End If
            End If
            For Each VFont As VGDDFont In Common._Fonts
                If VFont.IsGOLFontDefault Then
                    Select Case Mal.FrameworkName.ToUpper
                        Case "MLALEGACY"
                            MplabX.FileDisableDefine("GraphicsConfig.h", "FONTDEFAULT", True)
                            MplabX.GraphicsConfigEnableDefine("FONTDEFAULT " & VFont.Name)
                        Case "MLA"
                            MplabX.FileDisableDefine("system_config.h", "DRV_TOUCHSCREEN_FONT", True)
                            MplabX.FileEnableDefine("system_config.h", "DRV_TOUCHSCREEN_FONT " & VFont.Name, "#define __SYSTEM_CONFIG_H")
                            'Case "HARMONY"
                            '    MplabX.FileDisableDefine("system_config.h", "GOLFontDefault", True)
                            '    MplabX.FileEnableDefine("system_config.h", "GOLFontDefault " & VFont.Name, "#define _SYSTEM_CONFIG_H")
                    End Select
                End If
            Next
            If Mal.MalVersionNum < 306 Then
                MplabX.FileDisableDefine("GraphicsConfig.h", "GOLFontDefault", True)
                MplabX.GraphicsConfigEnableDefine("GOLFontDefault FONTDEFAULT")
            End If

            For Each oBitmap In _Bitmaps
                If oBitmap.ToBeConverted Then
                    oBitmap.ToBeConverted = False
                End If
            Next

            For Each VFont As VGDDFont In Common._Fonts
                If VFont.ToBeConverted Then
                    VFont.ToBeConverted = False
                End If
            Next

            Return True
        End Function

        Public Shared Function CheckScreenTexts(ByVal oScreen As VGDD.VGDDScreen) As String
            If oScreen Is Nothing Then Return String.Empty
            Dim strScreensText As String = ""
            For Each obj As Object In OrderedControlArray(oScreen.Controls)
                If TypeOf (obj) Is VGDDMicrochip.VGDDWidget Then
                    Dim oWidget As VGDDMicrochip.VGDDWidget = obj
                    If oWidget.SchemeObj IsNot Nothing AndAlso oWidget.SchemeObj.Font IsNot Nothing Then
                        Dim strAllChars As String = oWidget.AllChars
                        strScreensText &= strAllChars & vbCrLf
                        oWidget.SchemeObj.Font.SmartCharSetAddString(strAllChars)
                    End If
                End If
            Next
            Return strScreensText
        End Function

        Public Shared Sub ControlToCode(ByVal oControl As Control)
            Try
                NumId += 1

                Dim WidgetType As Type = oControl.GetType
                Dim strWidgetType As String = WidgetType.ToString.Replace("VGDDMicrochip.", "").Replace("VGDD.", "").Replace("VGDDCommon.", "").Replace("VGDDVirtualWidgets.", "")
                Dim WidgetAssembly As Assembly = WidgetType.Assembly
                Dim WidgetAssemblyName As String = WidgetAssembly.GetName.Name
                If Not WidgetAssemblyName = "VGDDCommonEmbedded" AndAlso strWidgetType <> "VGDDCustom" Then
                    XmlExternalTemplatesDoc = New XmlDocument
                    Dim sr As StreamReader
                    Try
#If CONFIG = "Debug" Then
                        If WidgetAssemblyName = "VirtualWidgets" Then
                            sr = New StreamReader(Path.GetFullPath(Path.Combine(Application.ExecutablePath, "..\..\..\..\VirtualWidgets\CodeGenTemplates.xml")))
                        Else
                            sr = New StreamReader(VirtualFabUtils.Utils.GetResourceStream("CodeGenTemplates.xml", WidgetAssembly))
                        End If
#Else
                        sr = New StreamReader(VirtualFabUtils.Utils.GetResourceStream("CodeGenTemplates.xml", WidgetAssembly))
#End If
                        Dim xml As String = sr.ReadToEnd
                        sr.Close()
                        XmlExternalTemplatesDoc.PreserveWhitespace = True
                        XmlExternalTemplatesDoc.LoadXml(xml)

                    Catch ex As Exception
                        MessageBox.Show("Error accessing CodeGenTemplates.xml in " & WidgetAssemblyName & " library", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End Try
                    CodeTemplate = GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Code", strWidgetType), 0, 1)
                    CodeHeadTemplate = GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/CodeHead", strWidgetType), 0, 0)
                    HeadersTemplate = GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Header", strWidgetType), 0, 0)
                    HeadersIncludesTemplate = GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/HeadersIncludes", strWidgetType), 0, 0)
                    ConstructorTemplate = GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Constructor", strWidgetType), 0, 0)
                    CodeUpdateTemplate = GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/CodeUpdate", strWidgetType)).Trim
                    GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/State", strWidgetType), StateNode, 0, 0)
                    GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Alignment", strWidgetType), AlignmentNode, 0, 0)
                    GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Statement", strWidgetType), StateMentNode, 0, 0)
                    GraphicsConfigTemplate = GetTemplate(XmlExternalTemplatesDoc, String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/GraphicsConfig", strWidgetType))

                    If Not Common.ProjectUseGol Then
                        Dim oGolNode As XmlNode = XmlExternalTemplatesDoc.SelectSingleNode(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/GOL", strWidgetType))
                        If oGolNode IsNot Nothing AndAlso oGolNode.InnerText.ToUpper = "YES" Then
                            Common.ProjectUseGol = True
                        End If
                    End If

                Else
                    Select Case strWidgetType
                        Case "VGDDCustom"
                            Dim CustomType As String = CType(oControl, VGDDCustom).CustomWidgetType
                            CodeTemplate = VGDDCustom.CustomGetTemplate(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/Code", CustomType), 1, 1)
                            CodeHeadTemplate = VGDDCustom.CustomGetTemplate(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/CodeHead", CustomType), 0, 0)
                            HeadersTemplate = VGDDCustom.CustomGetTemplate(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/Header", CustomType), 0, 0)
                            HeadersIncludesTemplate = VGDDCustom.CustomGetTemplate(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/HeadersIncludes", CustomType), 0, 0)
                            ConstructorTemplate = VGDDCustom.CustomGetTemplate(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/Constructor", CustomType), 0, 0)
                            CodeUpdateTemplate = VGDDCustom.CustomGetTemplate(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/CodeUpdate", CustomType), 1, 1)
                            StateNode = VGDDCustom.CustomGetTemplateNode(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/State", CustomType))
                            AlignmentNode = VGDDCustom.CustomGetTemplateNode(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/Alignmen", CustomType))
                            StateMentNode = VGDDCustom.CustomGetTemplateNode(String.Format("VGDDCustomWidgetsTemplate/{0}/CodeGen/Statement", CustomType))

                            If Not Common.ProjectUseGol Then
                                Dim oGolNode As XmlNode = VGDDCustom.CustomGetTemplateNode(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/GOL", strWidgetType))
                                If oGolNode IsNot Nothing AndAlso oGolNode.InnerText.ToUpper = "YES" Then
                                    Common.ProjectUseGol = True
                                End If
                            End If
                        Case Else ' VGDDMicrochip.*
                            CodeTemplate = GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Code", strWidgetType), 1, 1)
                            CodeHeadTemplate = GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/CodeHead", strWidgetType), 0)
                            HeadersTemplate = GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Header", strWidgetType))
                            HeadersIncludesTemplate = GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/HeadersIncludes", strWidgetType))
                            ConstructorTemplate = GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Constructor", strWidgetType, 1))
                            CodeUpdateTemplate = GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/CodeUpdate", strWidgetType), 1, 1)
                            GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/State", strWidgetType), StateNode, 0)
                            GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Alignment", strWidgetType), AlignmentNode, 0)
                            GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Statement", strWidgetType), StateMentNode, 0)
                            GraphicsConfigTemplate = GetTemplate(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/GraphicsConfig", strWidgetType))

                            If Not Common.ProjectUseGol Then
                                Dim oGolNode As XmlNode = XmlTemplatesDoc.SelectSingleNode(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/GOL", strWidgetType))
                                If oGolNode IsNot Nothing AndAlso oGolNode.InnerText.ToUpper = "YES" Then
                                    Common.ProjectUseGol = True
                                End If
                            End If
                    End Select
                End If

                Dim oGetCode As IVGDDBase = oControl
                Try
                    oGetCode.GetCode(CodeGen.ControlIdPrefix)
                Catch ex As Exception
                    MessageBox.Show("Error getting code for " & oControl.Name & " in " & oControl.Parent.Name & ": " & ex.Message, "Codegen error")
                End Try

                If TypeOf (oControl) Is IVGDDWidget AndAlso CType(oControl, IVGDDWidget).HasChildWidgets AndAlso oControl.Controls.Count > 0 Then
                    Dim strControlPrefix As String = CodeGen.ControlIdPrefix
                    CodeGen.ControlIdPrefix &= "_" & oControl.Site.Name
                    For Each oSubControl As Control In OrderedControlArray(oControl.Controls)
                        ControlToCode(oSubControl)
                    Next
                    CodeGen.ControlIdPrefix = strControlPrefix
                End If

                MplabX.MplabXAddProjectFiles(XmlTemplatesDoc.SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Project/Folder", strWidgetType)))

            Catch ex As Exception
            End Try
        End Sub

        Public Shared Function MyCodeHead(ByVal CDeclType As TextCDeclType) As String
            If Common.ProjectMultiLanguageTranslations > 0 Then
                Select Case CDeclType
                    Case TextCDeclType.RamXcharArray
                        Return CodeGen.TextDeclareCodeHeadTemplate(CDeclType).Replace("[CHARMAX]", "")
                    Case Else
                        Return ""
                End Select
            Else
                Return CodeGen.TextDeclareCodeHeadTemplate(CDeclType).Replace("[CHARMAX]", "")
            End If
        End Function

        Public Shared Function MyHeader(ByVal CDeclType As TextCDeclType) As String
            If Common.ProjectMultiLanguageTranslations > 0 Then
                If CDeclType = TextCDeclType.ConstXcharArray Then
                    Return CodeGen.WidgetsTextTemplateHeader
                Else
                    Return String.Empty
                End If
            Else
                Return String.Empty
            End If
        End Function

        Public Shared Sub ScreenToCode(ByVal oScreenAttr As VGDD.VGDDScreenAttr)
            Dim oScreen As VGDD.VGDDScreen = oScreenAttr.Screen
            Dim x As Integer = oScreen.Controls.Count
            Common.CodegenProgress += 1
            Application.DoEvents()
            If oScreen Is Nothing Then Exit Sub
            ScreenName = oScreenAttr.Name
            NumScreens += 1

            ScreenStates &= vbCrLf & "CREATE_" & ScreenName.ToUpper & "," & vbCrLf & _
                         "UPDATE_" & ScreenName.ToUpper & "," & vbCrLf & _
                         "DISPLAY_" & ScreenName.ToUpper & "," & vbCrLf & _
                         "EDITED_" & ScreenName.ToUpper & "," & vbCrLf
            Dim strCodeTemplate As String
            Dim strMasterScreens As String = ""
            If oScreen.MasterScreens IsNot Nothing AndAlso oScreen.MasterScreens.Count > 0 Then
                strCodeTemplate = GetTemplate("VGDDCodeTemplate/ScreenTemplates/Create/Code/NormalWithMasterScreen", 0, 1)
                Dim strCreateMasterScreensTemplate As String = GetTemplate("VGDDCodeTemplate/ScreenTemplates/Create/Code/CreateMasterScreens", 0, 1)
                For Each strMasterScreen As String In oScreen.MasterScreens
                    strMasterScreens &= strCreateMasterScreensTemplate.Replace("[MASTERSCREEN_NAME]", strMasterScreen)
                Next
            ElseIf oScreen.IsMasterScreen Then
                strCodeTemplate = GetTemplate("VGDDCodeTemplate/ScreenTemplates/Create/Code/MasterScreen", 0, 1)
            ElseIf oScreen.Overlay Then
                strCodeTemplate = GetTemplate("VGDDCodeTemplate/ScreenTemplates/Create/Code/Overlay", 0, 1)
            Else
                strCodeTemplate = GetTemplate("VGDDCodeTemplate/ScreenTemplates/Create/Code/Normal", 0, 1)
            End If

            Dim strSetPaletteCode As String = String.Empty
            If Common.ProjectUsePalette AndAlso oScreen.PaletteName IsNot Nothing AndAlso oScreen.PaletteName <> String.Empty Then
                strSetPaletteCode = GetTemplate("VGDDCodeTemplate/ProjectTemplates/SetPalette", 1).Replace("[PALETTE]", "palette_" & oScreen.PaletteName)
            End If
            strCodeTemplate = RemoveEmptyLines(strCodeTemplate.Replace("[SETPALETTE]", strSetPaletteCode) _
                .Replace("[SCREEN_NAME]", ScreenName) _
                .Replace("[CREATE_MASTERSCREENS]", strMasterScreens) _
                .Replace("[SCREEN_BACKCOLOR]", Color2Num(oScreen.BackColor, False, "Screen " & oScreen.Name & ".BackColor")) _
                .Replace("[SCREEN_BACKCOLOR_STRING]", Color2String(oScreen.BackColor)) _
                .Replace("[TRANSPARENT_COLOUR]", Color2Num(System.Drawing.Color.FromArgb(0, oScreen.TransparentColour), oScreen.TransparentColour = Nothing, "Screen " & oScreen.Name & ".TransparentColour")) _
                .Replace("[TRANSPARENT_COLOUR_STRING]", Color2String(System.Drawing.Color.FromArgb(0, oScreen.TransparentColour))) _
                .Replace("[GOLFREE]", IIf(oScreen.GolFree And Not oScreen.IsMasterScreen, "[GOLFREE]", "")))
            Code &= strCodeTemplate

            Headers &= GetTemplate("VGDDCodeTemplate/ScreenTemplates/Create/Header", 0) _
                .Replace("[SCREEN_NAME]", ScreenName)

            'For Each oScheme As VGDDScheme In Common._Schemes
            '    oScheme._Referenced = False
            'Next

            For Each oScheme As VGDDScheme In Common._Schemes.Values
                oScheme.CodeGenerated = False
                If Common.ProjectUsePalette Then
                    If oScheme.Palette IsNot Nothing AndAlso oScheme.Palette.Name = oScreen.PaletteName Then
                        Common.SelectedScheme = oScheme
                    End If
                End If
            Next


            For Each oControl As Object In oScreen.Controls
                Try
                    'If TypeOf oControl Is Common.IVGDD Then
                    '    Dim strSchemeName As String = CType(oControl, Common.IVGDD).Scheme
                    '    For Each oScheme As VGDDScheme In Common._Schemes
                    '        If oScheme.Name = strSchemeName Then
                    '            oScheme._Referenced = True
                    '        End If
                    '    Next
                    'End If
                    Dim oType As Type = oControl.GetType
                    Dim strWidgetType As String = oType.ToString.Split(".")(1)
                    If strWidgetType = "VGDDCustom" Then
                        strWidgetType = CType(oControl, VGDDCustom).Type
                    End If
                    If Not FootPrint.Widgets.ContainsKey(strWidgetType) Then
                        FootPrint.Widgets.Add(strWidgetType, 0)
                    End If
                    If Not FootPrint.ProjectWidgets.ContainsKey(strWidgetType) Then
                        FootPrint.ProjectWidgets.Add(strWidgetType, 0)
                        FootPrint.ROMCode += FootPrintValue(strWidgetType, "ROM" & Common.ProjectPicFamily)
                        FootPrint.RAM += FootPrintValue(strWidgetType, "RAM" & Common.ProjectPicFamily)
                    End If
                    If TypeOf oControl Is IVGDDWidget Then
                        Dim oWidget As IVGDDWidget = oControl
                        'FootPrintHeap += oControl.FootPrintHEAP * oControl.Instances
                        FootPrint.Widgets.Item(strWidgetType) += FootPrintValue(strWidgetType, "HEAP" & Common.ProjectPicFamily)
                        If oWidget.SchemeObj IsNot Nothing AndAlso Not oWidget.SchemeObj.CodeGenerated Then
                            Dim strSchemeCode As String = GetTemplate("VGDDCodeTemplate/ScreenTemplates/CreateSchemes/Code", 1).Replace("[SCHEME_NAME]", oWidget.Scheme)
                            Code &= strSchemeCode
                            oWidget.SchemeObj.CodeGenerated = True
                        End If
                    End If
                    Dim ControlId As String = oScreen.Name & "_" & oControl.Name
                    Dim ControlIdNoIndex As String = oScreen.Name & "_" & oControl.Name.Split("[")(0)
                    Dim ControlIdIndex As String = "", ControlIdIndexPar As String = ""
                    If ControlId <> ControlIdNoIndex Then
                        ControlIdIndexPar = ControlId.Substring(ControlIdNoIndex.Length)
                        ControlIdIndex = ControlIdIndexPar.Replace("[", "").Replace("]", "")
                    End If
                    EventsInsertScreenCode(oControl, oScreen, ControlIdNoIndex, ControlIdIndex)

                Catch ex As Exception
                End Try
            Next

            AllCodeTemplate = GetTemplate("VGDDCodeTemplate/ControlsTemplates/All/Code", 2)
            ScreenEventCode = ""
            ScreenUpdateCode = ""

            CodeGen.ControlIdPrefix = oScreen.Name
            For Each oControl As Object In OrderedControlArray(oScreen.Controls)
                ControlToCode(oControl)
            Next
            Code &= GetTemplate("VGDDCodeTemplate/ScreenTemplates/ClosingBlock/Code") _
                .Replace("[SCREEN_NAME]", ScreenName)
            Headers &= GetTemplate("VGDDCodeTemplate/ScreenTemplates/ClosingBlock/Header") _
                .Replace("[SCREEN_NAME]", ScreenName)
            'If ScreenEventCode <> "" Or ScreenUpdateCode <> "" Then
            If oScreen.IsMasterScreen Then
                oScreenAttr.EventsCode &= ScreenEventCode.Replace("[SCREEN_NAME]", ScreenName) _
                   .Replace("[SCREEN_UPPERNAME]", ScreenName.ToUpper)
                oScreenAttr.EventsUpdateCode &= ScreenUpdateCode.Replace("[SCREEN_NAME]", ScreenName) _
                   .Replace("[SCREEN_UPPERNAME]", ScreenName.ToUpper)
            Else
                'If oScreen.MasterScreens IsNot Nothing AndAlso oScreen.MasterScreens.Count > 0 Then
                '    For Each strMasterScreen As String In oScreen.MasterScreens
                '        If Common.aScreens.ContainsKey(strMasterScreen.ToUpper) Then
                '            ScreenUpdateCode &= aScreens(strMasterScreen).EventsUpdateCode
                '        End If
                '    Next
                'End If
                AllScreensEventMsgCode &= (GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/ScreenHead", 0) & _
                   ScreenEventCode & _
                   GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/ScreenFoot", 0)) _
                   .Replace("[SCREEN_NAME]", ScreenName) _
                   .Replace("[SCREEN_UPPERNAME]", ScreenName.ToUpper) _
                   .Replace("&bmp", IIf(Common.ProjectUseBmpPrefix, "&bmp", "&"))
                Dim strScreenEventDrawCode As String = GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/DrawCallBack/Code/ScreenHead", 0) _
                    .Replace("[SCREEN_NAME]", ScreenName) _
                    .Replace("[SCREEN_UPPERNAME]", ScreenName.ToUpper) _
                    .Replace("&bmp", IIf(Common.ProjectUseBmpPrefix, "&bmp", "&"))
                If oScreen.VGDDEvents IsNot Nothing Then
                    strScreenEventDrawCode = strScreenEventDrawCode _
                        .Replace("[SCREEN_BEFORE_CREATE]", EventCodeExpandMacros(oScreen.VGDDEvents("SCREEN_BEFORE_CREATE"), "ID" & oScreen.Name, oScreen)) _
                        .Replace("[SCREEN_AFTER_CREATE]", EventCodeExpandMacros(oScreen.VGDDEvents("SCREEN_AFTER_CREATE"), "ID" & oScreen.Name, oScreen)) _
                        .Replace("[SCREEN_UPDATE_CODE]", EventCodeExpandMacros(oScreen.VGDDEvents("SCREEN_UPDATE"), "ID" & oScreen.Name, oScreen) & ScreenUpdateCode) _
                        .Replace("[SCREEN_DISPLAY_CODE]", EventCodeExpandMacros(oScreen.VGDDEvents("SCREEN_DISPLAY"), "ID" & oScreen.Name, oScreen))

                Else
                    strScreenEventDrawCode = strScreenEventDrawCode _
                        .Replace("[SCREEN_BEFORE_CREATE]", "") _
                        .Replace("[SCREEN_AFTER_CREATE]", "") _
                        .Replace("[SCREEN_UPDATE_CODE]", "") _
                        .Replace("[SCREEN_DISPLAY_CODE]", "")
                End If
                AllScreensEventDrawCode &= RemoveEmptyLines(strScreenEventDrawCode)
            End If
            'End If

            If GraphicsConfigTemplate <> "" AndAlso Not GraphicsConfigDefines.Contains(GraphicsConfigTemplate) Then
                'GraphicsConfigDefines &= GraphicsConfigTemplate
            End If

            '#If CONFIG = "DemoRelease" Or CONFIG = "DemoDebug" Then
            '            Headers = CodeGen.Scramble(Headers)
            '            AllScreensEventMsgCode = CodeGen.Scramble(AllScreensEventMsgCode)
            '            AllScreensEventDrawCode = CodeGen.Scramble(AllScreensEventDrawCode)
            '#End If

        End Sub

        Public Shared Function EventCodeExpandMacros(ByVal oEvent As VGDDEvent, ByVal ControlID As String, ByVal oObj As Object) As String
            If oEvent Is Nothing Then Return String.Empty
            Dim strEventCode As String = oEvent.Code.Replace("$widget_ID", "ID_" & ControlID)
            Dim strState(4) As String
            strState(0) = String.Empty
            strState(1) = String.Empty
            strState(2) = String.Empty
            strState(3) = String.Empty
            If TypeOf (oObj) Is VGDDMicrochip.VGDDWidget Then
                Try
                    Dim oWidget As VGDDMicrochip.VGDDWidget = oObj
                    strState(0) = "CREATE_" & oWidget.Parent.Name.ToUpper
                    strState(1) = "UPDATE_" & oWidget.Parent.Name.ToUpper
                    strState(2) = "DISPLAY_" & oWidget.Parent.Name.ToUpper
                    strState(3) = "EDITED_" & oWidget.Parent.Name.ToUpper
                Catch ex As Exception
                End Try
            ElseIf TypeOf (oObj) Is VGDD.VGDDScreen Then
                Try
                    Dim oScreen As VGDD.VGDDScreen = oObj
                    strState(0) = "CREATE_" & oScreen.Name.ToUpper
                    strState(1) = "UPDATE_" & oScreen.Name.ToUpper
                    strState(2) = "DISPLAY_" & oScreen.Name.ToUpper
                    strState(3) = "EDITED_" & oScreen.Name.ToUpper
                Catch ex As Exception
                End Try
            End If
            strEventCode = strEventCode.Replace("$CREATE_STATE", strState(0)) _
                                .Replace("$UPDATE_STATE", strState(1)) _
                                .Replace("$DISPLAY_STATE", strState(2)) _
                                .Replace("$EDITED_STATE", strState(3))
            Return strEventCode
        End Function

        Public Shared Function RemoveEmptyLines(ByVal TextIn As String) As String
            Dim sb As New Text.StringBuilder
            For Each riga As String In TextIn.Replace(vbCr, String.Empty).Split(vbLf)
                If riga.Trim <> String.Empty Then
                    sb.AppendLine(riga)
                End If
            Next
            Return sb.ToString
        End Function

        'Public Shared Sub AddFilesToProject(ByVal oNodes As XmlNodeList)
        '    AddFilesToProject(oNodes, Nothing)
        'End Sub

        'Public Shared Sub AddFilesToProject(ByVal oNodes As XmlNodeList, ByVal WidgetAssembly As Assembly)
        '    If Common.MplabXProjectPath Is Nothing Then Exit Sub
        '    For Each oFolderNode As XmlNode In oNodes
        '        If oFolderNode.Name = "Folder" Then
        '            Dim strFolderPath As String = oFolderNode.Attributes("Name").Value
        '            For Each oFileNode As XmlNode In oFolderNode.ChildNodes
        '                Select Case oFileNode.NodeType
        '                    Case XmlNodeType.Whitespace, XmlNodeType.Whitespace, XmlNodeType.Comment
        '                    Case Else
        '                        Select Case oFileNode.Name
        '                            Case "AddFile", "AddVGDDFile"
        '                                Dim strDestFile As String = String.Empty
        '                                Dim strDestDir As String = String.Empty
        '                                Dim strFileName As String = MplabX.AddFileCheckDestFile(oFileNode, strDestFile, strDestDir)
        '                                Dim strDestPathName As String
        '                                If strDestFile = String.Empty Then
        '                                    strDestPathName = Path.Combine(strDestDir, strFileName)
        '                                Else
        '                                    strDestPathName = Path.Combine(strDestDir, strDestFile)
        '                                End If
        '                                MplabX.MplabXAddFileIfNotExist(strFolderPath, strDestPathName)
        '                                If oFileNode.Name = "AddVGDDFile" AndAlso WidgetAssembly IsNot Nothing Then
        '                                    If Not File.Exists(strDestPathName) Then
        '                                        If Not Common.ExtractResourceToFile(strFileName, strDestPathName, WidgetAssembly) Then
        '                                            Debug.Print("")
        '                                        End If
        '                                    End If
        '                                End If
        '                            Case "AddLibrary"
        '                                MplabX.MplabXAddLibrary(oFolderNode.InnerText.Replace("$MAL", MplabX.MalRelativePath).Replace("[COMPILER]", Common.ProjectCompiler).Replace("[PICFAMILY]", Common.ProjectPicFamily).Replace("[MULTIBYTECHARS]", IIf(Common.ProjectUseMultiByteChars, "MultiByte", "")))
        '                        End Select
        '                End Select
        '            Next
        '        End If
        '    Next
        'End Sub

        Private Shared Function OrderedControlArray(ByVal ControlsIn As Control.ControlCollection) As Control()
            Dim ControlsOut(ControlsIn.Count - 1) As Control

            ControlsIn.CopyTo(ControlsOut, 0)
            Array.Sort(ControlsOut, New TabIndexComparer)
            Return ControlsOut
        End Function

        Class TabIndexComparer
            Implements System.Collections.IComparer

            Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
                Dim oControl1 As Control = x
                Dim oControl2 As Control = y
                Return oControl1.TabIndex < oControl2.TabIndex
            End Function
        End Class

        Public Shared Sub ToCodeClose()
            Try
                Dim AllMasterScreensEventCode As String = ""
                CodeFoot &= ReplaceProjectStrings(GetTemplate("VGDDCodeTemplate/ClosingBlock/Code"))
                CodeHead = ReplaceProjectStrings(GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/CodeHead")) _
                        & vbCrLf & CodeHead

                For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                    If oScreenAttr.Screen IsNot Nothing AndAlso oScreenAttr.Screen.IsMasterScreen Then
                        AllMasterScreensEventCode &= oScreenAttr.EventsCode
                    End If
                Next
                If AllMasterScreensEventCode <> "" Then
                    AllMasterScreensEventCode = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/MasterScreenHead", 0) & _
                        AllMasterScreensEventCode & _
                        CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/MasterScreenFoot", 0)
                End If

                If Common.ProjectUseGol Then
                    AllScreensEventMsgCode = _
                        (GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/Head", 0) & _
                        AllScreensEventMsgCode & _
                        GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/NormalScreensFoot", 0) & _
                        AllMasterScreensEventCode & _
                        GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Code/Foot", 0) & _
                        GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/DrawCallBack/Code/Head", 0) & _
                        AllScreensEventDrawCode & _
                        ReplaceProjectStrings(GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/DrawCallBack/Code/Foot", 0)))

                    Headers = GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/MsgCallBack/Header", 0) & _
                              GetTemplate("VGDDCodeTemplate/ProjectTemplates/EventHandling/DrawCallBack/Header", 0) & _
                        Headers
                Else
                    AllScreensEventMsgCode = String.Empty
                End If
                CodeHead = ReplaceProjectStrings(GetTemplate("VGDDCodeTemplate/ProjectTemplates/CodeHead", 0)) _
                    & CodeHead
                '.Replace("[VGDDVERSION]", VGDD.VERSION) _
                Headers = ReplaceProjectStrings(GetTemplate("VGDDCodeTemplate/ProjectTemplates/Header", 0) & _
                        Headers & _
                        GetTemplate("VGDDCodeTemplate/ClosingBlock/Header"))

            Catch ex As Exception
            End Try
        End Sub
#End If

        Public Shared Function ReplaceProjectStrings(ByVal strContent As String) As String
            strContent = strContent _
                .Replace("[PROJECT_NAME]", Common.ProjectName) _
                .Replace("[PROJECT_CLEAN_NAME]", Common.CleanName(Common.ProjectName)) _
                .Replace("[DISP_ORIENTATION]", Common.DisplayBoardOrientation) _
                .Replace("[COLOUR_DEPTH]", Common.ProjectColourDepth) _
                .Replace("[PROCESSOR]", Common.ProjectPicFamily) _
                .Replace("[COMPILER]", Common.ProjectCompiler) _
                .Replace("[DEVBOARD]", Common.DevelopmentBoardID) _
                .Replace("[EXPANSIONBOARD]", Common.ExpansionBoardID) _
                .Replace("[DISPLAYBOARD]", Common.DisplayBoardID) _
                .Replace("[VGDDVERSION]", VGDDCommon.Common.VGDDVERSION) _
                .Replace("[COPYRIGHT]", My.Application.Info.Copyright) _
                .Replace("[PROJECT_USE_MULTIBYTECHAR]", IIf(Common.ProjectUseMultiByteChars, "", "//") & "#define USE_MULTIBYTECHAR") _
                .Replace("[MAINSCREEN]", Common.aScreens(0).Name) _
                .Replace("[VGDD_NUM_SCREENS]", NumScreens) _
                .Replace("[VGDD_LAST_ID]", NumId) _
                .Replace("[FIRST_SCREEN_IN_PROJECT]", "CREATE_" & Common.aScreens(0).Name.ToUpper) _
                .Replace("[NUMSTRINGS]", Common.StringsPoolMaxStringID + 1) _
                .Replace("[NUMTRANSLATIONS]", Common.ProjectMultiLanguageTranslations + 1) _
                .Replace("[PROJECTFILENAME_SCREENSC]", Path.GetFileName(Common.ProjectFileName_ScreensC)) _
                .Replace("[PROJECTFILENAME_SCREENSH]", Path.GetFileName(Common.ProjectFileName_ScreensH)) _
                .Replace("[PROJECTFILENAME_HELPERH]", Path.GetFileName(Common.ProjectFileName_EventsHelperH))

            'If strContent.Contains("[MPLABX_PROJECT_FOLDER]") Then
            '    Debug.Print("")
            'End If

            Return strContent
        End Function

        Public Shared Sub AddLines(ByRef StringIn As String, ByVal strLine As String)
            If strLine.Trim <> String.Empty Then
                If Not StringIn.EndsWith(vbCrLf) AndAlso Not strLine.StartsWith(vbCrLf) Then
                    StringIn &= vbCrLf
                End If
                StringIn &= strLine
            End If
        End Sub

        Public Shared Function GetTemplate(ByVal xPath As String) As String
            Return GetTemplate(XmlTemplatesDoc, xPath, Nothing, 0, 0)
        End Function

        Public Shared Function GetTemplate(ByVal xPath As String, ByRef oNode As XmlNode, ByVal NumTabs As Integer) As String
            Return GetTemplate(XmlTemplatesDoc, xPath, oNode, NumTabs, NumTabs)
        End Function

        Public Shared Function GetTemplate(ByVal xPath As String, ByRef oNode As XmlNode, ByVal NumTabsFirstLine As Integer, ByVal NumTabs As Integer) As String
            Return GetTemplate(XmlTemplatesDoc, xPath, oNode, NumTabsFirstLine, NumTabs)
        End Function

        Public Shared Function GetTemplate(ByVal xPath As String, ByVal NumTabsFirstLine As Integer, ByVal NumTabs As Integer) As String
            Return GetTemplate(XmlTemplatesDoc, xPath, Nothing, NumTabsFirstLine, NumTabs)
        End Function

        Public Shared Function GetTemplate(ByVal xPath As String, ByVal NumTabs As Integer) As String
            Return GetTemplate(XmlTemplatesDoc, xPath, Nothing, NumTabs, NumTabs)
        End Function

        Public Shared Function GetTemplate(ByRef xDoc As XmlNode, ByVal xPath As String) As String
            Return GetTemplate(xDoc, xPath, Nothing, 0, 0)
        End Function

        Public Shared Function GetTemplate(ByRef xDoc As XmlNode, ByVal xPath As String, ByVal NumTabsFirstLine As Integer, ByVal NumTabs As Integer) As String
            Return GetTemplate(xDoc, xPath, Nothing, NumTabsFirstLine, NumTabs)
        End Function

        Public Shared Function GetTemplate(ByRef xDoc As XmlNode, ByVal xPath As String, ByRef oNode As XmlNode, ByVal NumTabsFirstLine As Integer, ByVal NumTabs As Integer) As String
            If xDoc IsNot Nothing Then oNode = xDoc.SelectSingleNode(xPath)
            Dim Template As String = ""
            Dim NumRiga As Integer = 0

            If oNode Is Nothing Then Return String.Empty
            Dim OrgTemplate As String = oNode.InnerText()
            'If OrgTemplate.StartsWith(vbCrLf) Then OrgTemplate = OrgTemplate.Substring(2)
            If oNode.InnerXml.Trim.StartsWith("<![CDATA[") Then
                Return OrgTemplate
            End If
            For Each Riga As String In OrgTemplate.Trim.Split(vbLf)
                NumRiga += 1
                If NumTabsFirstLine = -1 Then
                    Template &= vbCrLf & Riga.Replace(vbCr, "").Replace(vbLf, "")
                Else
                    Template &= vbCrLf & Space(IIf(NumRiga = 1, NumTabsFirstLine, NumTabs) * 4) & Riga.Replace(vbCr, "").Replace(vbLf, "").Trim
                End If
            Next
            Return Template '.Substring(2).TrimEnd '& vbCrLf
        End Function

        Public Shared Function ColorIndex(ByVal PaletteColours As Drawing.Color(), ByVal col As Drawing.Color) As Integer
            For i As Integer = 0 To PaletteColours.Length - 1
                If col.R = PaletteColours(i).R AndAlso col.G = PaletteColours(i).G AndAlso col.B = PaletteColours(i).B Then
                    Return i
                End If
            Next
            Return -1
        End Function

        Public Shared Function Color2Num(ByVal col As System.Drawing.Color) As Long
            Return Color2Num(col, False, String.Empty)
        End Function
        Public Shared Function Color2Num(ByVal col As System.Drawing.Color, ByVal ignoreErrors As Boolean, ByVal ObjectDescription As String) As Long
            If Common.ProjectUsePalette AndAlso Common.SelectedScheme.Palette IsNot Nothing Then
                Color2Num = ColorIndex(Common.SelectedScheme.Palette.PaletteColours, col)
                If Color2Num >= 0 Then
                    Exit Function
                Else
                    Dim simCol As Drawing.Color = Common.SelectedScheme.Palette.GetNearestColor(col)
                    If Not ignoreErrors Then
                        Dim strError As String = String.Format("Warning: Color {0}, {1}, {2} not found in palette {3} in {4}, using nearest color {5}, {6}, {7}", col.R, col.G, col.B, Common.SelectedScheme.Palette.Name, ObjectDescription, simCol.R, simCol.G, simCol.B)
                        If Not CodeGen.Errors.Contains(strError) Then
                            CodeGen.Errors &= strError & vbCrLf
                        End If
                    End If
                    Color2Num = ColorIndex(Common.SelectedScheme.Palette.PaletteColours, simCol)
                    Exit Function
                End If
            End If

            Select Case Common.ProjectColourDepth
                Case 1
                    If col.R = 0 AndAlso col.G = 0 AndAlso col.B = 0 Then
                        Return 0
                    Else
                        Return 1
                    End If
                Case 2
                    Return Color2RGB110(col)
                Case 4
                    Return Color2RGB121(col)
                Case 8
                    Return Color2RGB332(col)
                Case 16
                    Return Color2RGB565(col)
                Case 18
                    Return Color2RGB666(col)
                Case Else
                    Return Color2RGB888(col)
            End Select

        End Function

        Public Shared Function Color2String(ByVal col As System.Drawing.Color) As String
            Color2String = col.ToString.Replace("A=255, ", "")
            If Common.ProjectUsePalette Then
                'Select Case Common.ProjectColourDepth
                '    Case 4, 8
                '        If Array.IndexOf(Common.CurrentScreen.CustomColors, col) < 0 Then
                '            Color2String &= " WARNING: not found in custom colours table! Using 0 but FIXIT"
                '        End If
                'End Select
            End If
        End Function

        Public Shared Function Color2RGB110(ByVal col As System.Drawing.Color) As Byte
            Dim redB As Byte = col.R, red As UInt16
            Dim greenB As Byte = col.G, green As UInt16
            red = redB >> 7
            green = greenB >> 6
            Return (red << 1) + green
        End Function

        Public Shared Function Color2RGB121(ByVal col As System.Drawing.Color) As Byte
            Dim redB As Byte = col.R, red As UInt16
            Dim greenB As Byte = col.G, green As UInt16
            Dim blueB As Byte = col.B, blue As UInt16
            '((value & 0xE000) >> 8) | ((value & 0x0700) >> 6) | ((value & 0x0018) >> 3);
            red = redB >> 7
            green = greenB >> 6
            blue = blueB >> 7
            Return (red << 3) + (green << 1) + (blue)
        End Function

        Public Shared Function Color2RGB332(ByVal col As System.Drawing.Color) As Byte
            Dim redB As Byte = col.R, red As UInt16
            Dim greenB As Byte = col.G, green As UInt16
            Dim blueB As Byte = col.B, blue As UInt16
            '((value & 0xE000) >> 8) | ((value & 0x0700) >> 6) | ((value & 0x0018) >> 3);
            red = redB >> 5
            green = greenB >> 5
            blue = blueB >> 6
            Return (red << 5) + (green << 2) + (blue)
        End Function

        Public Shared Function Color2RGB565(ByVal col As System.Drawing.Color) As UInt16
            Dim redB As Byte = col.R, red As UInt16
            Dim greenB As Byte = col.G, green As UInt16
            Dim blueB As Byte = col.B, blue As UInt16
            red = redB >> 3
            green = greenB >> 2
            blue = blueB >> 3
            Return (red << 11) + (green << 5) + (blue)
        End Function

        Public Shared Function ColorFromRGB565(ByVal c16 As UInt16) As System.Drawing.Color
            Dim redB As Integer = (c16 And (31 << 11)) >> 8
            Dim greenB As Integer = (c16 And (63 << 5)) >> 3
            Dim blueB As Integer = (c16 And 31) << 3
            Return System.Drawing.Color.FromArgb(redB, greenB, blueB)
        End Function

        Public Shared Function Color2RGB666(ByVal col As System.Drawing.Color) As UInt16
            Dim redB As Byte = col.R, red As UInt16
            Dim greenB As Byte = col.G, green As UInt16
            Dim blueB As Byte = col.B, blue As UInt16
            red = redB >> 2
            green = greenB >> 2
            blue = blueB >> 2
            Return (red << 12) + (green << 6) + (blue)
        End Function

        Public Shared Function Color2RGB888(ByVal col As System.Drawing.Color) As UInt16
            Dim redB As Byte = col.R, red As UInt16
            Dim greenB As Byte = col.G, green As UInt16
            Dim blueB As Byte = col.B, blue As UInt16
            Return (red << 16) + (green << 8) + (blue)
        End Function

        Public Shared Sub AddState(ByRef MyState As String, ByVal PropertyName As String, ByVal PropertyValue As String)
            If StateNode IsNot Nothing Then
                Dim oNode As XmlNode = StateNode.SelectSingleNode(PropertyName)
                Dim oAttr As XmlAttribute, strAttrValue As String
                If oNode IsNot Nothing Then
                    oAttr = oNode.Attributes(PropertyValue)
                    If oAttr IsNot Nothing Then
                        strAttrValue = oAttr.Value
                        If Not MyState.Contains(strAttrValue) Then
                            MyState &= "|" & strAttrValue
                        End If
                    End If
                    If MyState.StartsWith("|") Then MyState = MyState.Substring(1)
                End If
            End If
        End Sub

        Public Shared Sub AddAlignment(ByRef MyAlignment As String, ByVal PropertyName As String, ByVal PropertyValue As String)
            If AlignmentNode IsNot Nothing Then
                Dim oNode As XmlNode = AlignmentNode.SelectSingleNode(PropertyName)
                Dim oAttr As XmlAttribute, strAttrValue As String
                If oNode IsNot Nothing Then
                    oAttr = oNode.Attributes(PropertyValue)
                    If oAttr IsNot Nothing Then
                        strAttrValue = oAttr.Value
                        If Not MyAlignment.Contains(strAttrValue) Then
                            MyAlignment &= "|" & strAttrValue
                        End If
                    End If
                    If MyAlignment.StartsWith("|") Then MyAlignment = MyAlignment.Substring(1)
                End If
            End If
        End Sub

        Public Shared Function GetStatement(ByVal PropertyName As String, ByVal PropertyValue As String) As String
            Dim oNode As XmlNode = StateMentNode.SelectSingleNode(PropertyName)
            Dim oAttr As XmlAttribute
            If oNode IsNot Nothing Then
                oAttr = oNode.Attributes(PropertyValue)
                If oAttr IsNot Nothing Then
                    Return oAttr.Value
                End If
            End If
            Return String.Empty
        End Function

        Public Shared Function BitmapStatement(ByVal oWidget As VGDDMicrochip.VGDDWidgetWithBitmap, ByVal BitmapName As String, ByRef MyPointerInit As String) As String
            BitmapStatement = String.Empty
            MyPointerInit = String.Empty
            If BitmapName Is Nothing OrElse BitmapName.Trim = String.Empty Then
                BitmapStatement = "NULL"
            Else
                Dim myBitmapName As String = System.IO.Path.GetFileNameWithoutExtension(BitmapName)
                If oWidget.BitmapUsePointer Then
                    Dim strPointerName As String = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerName", 0).Trim _
                                                   .Replace("[BITMAPNAME]", myBitmapName)
                    BitmapStatement = "(void *)" & strPointerName
                    MyPointerInit = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerInit", 0).Trim _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName) _
                        .Replace("[BITMAPNAME]", myBitmapName)
                    CodeGen.AddLines(CodeGen.Headers, CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/Bitmaps", 0) _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[BITMAPNAME]", myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName))
                Else
                    BitmapStatement = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName
                End If
            End If
        End Function

        Public Shared Function LineTypeMLA4(ByVal Thickenss As Common.ThickNess, ByVal LineType As Common.LineType) As String
            Select Case Thickenss
                Case ThickNess.NORMAL_LINE
                    Select Case LineType
                        Case LineType.SOLID_LINE
                            Return "GFX_LINE_STYLE_THIN_SOLID"
                        Case LineType.DASHED_LINE
                            Return "GFX_LINE_STYLE_THIN_DASHED"
                        Case LineType.DOTTED_LINE
                            Return "GFX_LINE_STYLE_THIN_DOTTED"
                    End Select
                Case ThickNess.THICK_LINE
                    Select Case LineType
                        Case LineType.SOLID_LINE
                            Return "GFX_LINE_STYLE_THICK_SOLID"
                        Case LineType.DASHED_LINE
                            Return "GFX_LINE_STYLE_THICK_DASHED"
                        Case LineType.DOTTED_LINE
                            Return "GFX_LINE_STYLE_THICK_DOTTED"
                    End Select
            End Select
            Return ""
        End Function

        Public Shared Function ProjectUseGOL() As Boolean
            Common.ProjectUseGol = False
            For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                If oScreenAttr.Screen Is Nothing Then Continue For
                Dim oScreen As VGDD.VGDDScreen = oScreenAttr.Screen
                For Each oControl As Object In oScreen.Controls
                    If TypeOf oControl Is IVGDDWidget Then
                        Dim oType As Type = oControl.GetType
                        Dim strWidgetType As String = oType.ToString.Split(".")(1)
                        Dim oGolNode As XmlNode = XmlTemplatesDoc.SelectSingleNode(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/GOL", strWidgetType))
                        If oGolNode IsNot Nothing AndAlso oGolNode.InnerText.ToUpper = "YES" Then
                            Common.ProjectUseGol = True
                            Return True
                        End If
                    End If
                Next
            Next
            Return False
        End Function

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class
End Namespace
