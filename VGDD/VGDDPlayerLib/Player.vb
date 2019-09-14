Imports System.Xml
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.ComponentModel.Design.Serialization
Imports System.Reflection
Imports System.Text
Imports System.Globalization
Imports System.Runtime.Serialization.Formatters.Binary
Imports VGDDCommon
Imports VGDDCommon.Common
Imports System.Collections.Generic

'Namespace VGDDPlayerLib

Public Class Player

    'Private Shared host As IDesignerLoaderHost
    Public Enum RunMode
        Standalone
        FromDesigner
    End Enum

    Public Shared Mode As RunMode = RunMode.FromDesigner
    Private Shared XmlPlayer As XmlDocument
    Private Shared WithEvents oFormPlayer As frmPlayer
    Public Shared ScreenHistory As New Collection
    Public Shared ScreenHistoryIdx As Integer
    Public Shared PackageName As String
    Public Shared IsSigned As Boolean
    Public Shared TempPath As String
    'Public Shared aAssemblies As New Collection

    Public Shared Sub PlayPackage(ByVal PackageFile As String)
        If PackageFile = String.Empty Then Exit Sub
        Common.BitmapsBinPath = Path.GetDirectoryName(PackageFile)
        Dim xmlPlayerPackage As New XmlDocument
        xmlPlayerPackage.Load(PackageFile)
        PlayPackage(xmlPlayerPackage)
    End Sub

#If PlayerMonolitico Then
    'Private Shared Function LoadAssembly(AssemblyResourceName As String) As Assembly
    '    Dim oStream As Stream
    '    oStream = Assembly.GetExecutingAssembly.GetManifestResourceStream(AssemblyResourceName)
    '    If oStream Is Nothing Then
    '        oStream = GetResource(AssemblyResourceName)
    '    End If
    '    If oStream Is Nothing Then
    '        Return Nothing
    '    End If
    '    Dim oReader As BinaryReader = New BinaryReader(oStream)
    '    Dim NewAssembly As Assembly
    '    Dim assemblyData As [Byte]() = New [Byte](oStream.Length - 1) {}
    '    oStream.Read(assemblyData, 0, assemblyData.Length)
    '    NewAssembly = Assembly.Load(assemblyData)
    '    Return NewAssembly
    'End Function

    'Private Shared Sub LoadAssemblies()
    '    aAssemblies.Add(LoadAssembly("VirtualWidgets.dll"))
    'End Sub

    Private Shared MyEmbeddedAssemblies As New Dictionary(Of String, Assembly)
    Shared Function MyAssemblyResolveEventHandler(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly
        Dim NewAssembly As Assembly = Nothing
        'This handler is called only when the common language runtime tries to bind to the assembly and fails. 
        Dim strAssemblyName As String = args.Name.Split(",")(0)
        If strAssemblyName = "VGDD" Then strAssemblyName = "VGDDCommonEmbedded"
        If MyEmbeddedAssemblies.ContainsKey(strAssemblyName) Then
            Return MyEmbeddedAssemblies(strAssemblyName)
        End If
        Dim s As IO.Stream = Nothing
        If strAssemblyName = "VGDD" Or strAssemblyName.StartsWith("VGDDCommon") Then
            s = Assembly.GetExecutingAssembly().GetManifestResourceStream("VGDDCommonEmbedded.dll")
        Else 'If strAssemblyName = "ICSharpCode.SharpZipLib" Then
            s = Assembly.GetExecutingAssembly().GetManifestResourceStream(strAssemblyName & ".dll")
        End If
        If s IsNot Nothing Then
            Dim block(s.Length) As Byte
            s.Read(block, 0, block.Length)
            NewAssembly = Assembly.Load(block)
            'Else
            '    NewAssembly = oMainShell.GetLibraryAssembly(strAssemblyName)
        End If
        If NewAssembly IsNot Nothing Then
            MyEmbeddedAssemblies.Add(strAssemblyName, NewAssembly)
            'MessageBox.Show("Found Assembly " & strAssemblyName)
        Else
            'MessageBox.Show("Can't find Assembly " & strAssemblyName)
            Debug.Print("Can't find Assembly " & strAssemblyName)
        End If
        Return NewAssembly
    End Function
#End If

    Private Shared Function Md5Sum(ByVal Content As String, ByVal oEncoding As Encoding) As String
        Dim MD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim dataMd5 As Byte() = MD5.ComputeHash(oEncoding.GetBytes(Content))
        Dim sb As New StringBuilder()
        For j As Integer = 0 To dataMd5.Length - 1
            sb.AppendFormat("{0:x2}", dataMd5(j))
        Next
        Return sb.ToString
    End Function

    Public Shared Sub PlayPackage(ByVal oXmlPlayerPackage As XmlDocument)
#If PlayerMonolitico Then
        'LoadAssemblies()
        MyAssemblyResolveEventHandler(Nothing, New System.ResolveEventArgs("VirtualWidgets"))
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf MyAssemblyResolveEventHandler
#End If
        Dim strStartScreenName As String = ""
        PackageName = oXmlPlayerPackage.DocumentElement.Attributes("Name").Value
        IsSigned = False
        If oXmlPlayerPackage.DocumentElement.Attributes("Sign") IsNot Nothing Then
            Dim strSum As String = Md5Sum(oXmlPlayerPackage.DocumentElement.InnerXml.Replace(" ", "").Replace(vbCr, "").Replace(vbLf, "") & "VGDDPlayer", UTF8Encoding.UTF8)
            If oXmlPlayerPackage.DocumentElement.Attributes("Sign").Value = strSum.ToString Then
                IsSigned = True
            Else 'Old signing
                strSum = Md5Sum(oXmlPlayerPackage.DocumentElement.InnerXml & "VGDDPlayer", ASCIIEncoding.ASCII)
                If oXmlPlayerPackage.DocumentElement.Attributes("Sign").Value = strSum.ToString Then
                    IsSigned = True
                End If
            End If
        End If

        If System.Environment.OSVersion.Platform = PlatformID.Unix Or System.Environment.OSVersion.Platform = PlatformID.MacOSX Then
            TempPath = "/tmp/VGDDPlayer_" & PackageName
        Else
            TempPath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "VGDDPlayer_" & PackageName)
        End If
        If Directory.Exists(TempPath) Then
            Try
                Directory.Delete(TempPath, True)
                System.Threading.Thread.Sleep(100)
            Catch ex As Exception
            End Try
        End If
        If Not Directory.Exists(TempPath) Then
            Directory.CreateDirectory(TempPath)
        End If

        Dim i As Integer
        For i = 1 To 10
            If Directory.Exists(TempPath) Then Exit For
            System.Threading.Thread.Sleep(500)
        Next
        If i = 11 Then
            MessageBox.Show("Cannot create temporary directory" & vbCrLf & TempPath & vbCrLf & "Quitting...", "Error creating temp dir")
            Exit Sub
        End If

        Common._ProjectPath = TempPath
        Common.aScreens.Clear()

        Dim oXmlScreensNode As XmlNode = oXmlPlayerPackage.DocumentElement("Screens")
        If oXmlScreensNode Is Nothing Then
            Exit Sub
        End If

        oFormPlayer = New frmPlayer
        If IsSigned Then
            oFormPlayer.Text &= " - Full Version"
            oFormPlayer.ToolStripAnimation.Enabled = True
        Else
            oFormPlayer.Text &= " - Limited Version"
        End If
        oFormPlayer.Text &= " - " & Common.ProjectName
        oFormPlayer.ToolStripdrpScreens.DropDownItems.Clear()
        For Each oScrenNode As XmlNode In oXmlScreensNode.ChildNodes
            Dim strScreenFileName As String = Path.Combine(TempPath, oScrenNode.Attributes("Name").Value)

            Dim sw As New StringWriter
            Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
            xtw.Formatting = Formatting.Indented
            oScrenNode.WriteTo(xtw)
            Dim file As StreamWriter = New StreamWriter(strScreenFileName, False, New System.Text.UnicodeEncoding)
            file.Write(sw.ToString)
            file.Close()
        Next

        Dim oXmlBitmapsNode As XmlNode = oXmlPlayerPackage.DocumentElement("Bitmaps")
        If oXmlBitmapsNode IsNot Nothing Then
            For Each oBitmapNode As XmlNode In oXmlBitmapsNode.ChildNodes
                Dim strBitmapFileName As String = Path.Combine(TempPath, oBitmapNode.Attributes("FileName").Value)
                Dim bw As New BinaryWriter(File.Open(strBitmapFileName, FileMode.Create))
                bw.Write(Convert.FromBase64String(oBitmapNode.InnerText))
                bw.Flush()
                bw.Close()
            Next
        End If

        'Try
        Dim oXmlProjectNode As XmlNode = oXmlPlayerPackage.DocumentElement("VGDDProject")
        If oXmlProjectNode Is Nothing Then
            Exit Sub
        End If

        Common.LoadProject(oXmlProjectNode)
        oFormPlayer.ClientSize = New Size(Common.ProjectWidth * 1.05, Common.ProjectHeight * 1.2)
        'oFormPlayer.ClientSize = New Size(Common.ProjectWidth, Common.ProjectHeight)

        If oXmlPlayerPackage.DocumentElement.Attributes("FirstScreen") IsNot Nothing Then
            strStartScreenName = oXmlPlayerPackage.DocumentElement.Attributes("FirstScreen").Value
        Else
            strStartScreenName = Common.aScreens.Item(1).Name
        End If

        'Catch ex As Exception
        '    MessageBox.Show("Error loading project: " & ex.Message)
        'End Try
        ScreenHistory.Clear()
        ScreenHistory.Add(strStartScreenName, Path.GetFileNameWithoutExtension(strStartScreenName).ToUpper)
        ScreenHistoryIdx = 1
        OpenScreen(Common.aScreens(strStartScreenName).FileName, Nothing, False)
    End Sub

#If Not PlayerMonolitico Then

    Public Shared Sub Play(ByVal PlayerFile As String, ByVal Signed As Boolean)
        Common.BitmapsBinPath = Path.GetDirectoryName(PlayerFile)
        Dim xmlPlayer As New XmlDocument
        xmlPlayer.Load(PlayerFile)
        Play(xmlPlayer, Signed)
    End Sub

    Public Shared Sub Play(ByVal xmlPlayer As XmlDocument, ByVal Signed As Boolean)
        Dim strStartScreenName As String = ""

        IsSigned = Signed
        Try
            Dim oPlayerNode As XmlNode = xmlPlayer.DocumentElement
            Dim ProjectFileName As String = oPlayerNode.Attributes("ProjectFileName").Value

            Dim xmlProject As New XmlDocument
            xmlProject.Load(ProjectFileName)
            Dim oProjectNode As XmlNode = xmlProject.DocumentElement

            oFormPlayer = New frmPlayer
            oFormPlayer.ClientSize = New Size(Common.ProjectWidth * 1.05, Common.ProjectHeight * 1.2)
            If IsSigned Then
                oFormPlayer.ToolStripAnimation.Enabled = True
            End If

            If strStartScreenName = "" Then
                strStartScreenName = Common.aScreens.Item(0).Name
            End If
            ScreenHistory.Clear()
            ScreenHistory.Add(strStartScreenName, Path.GetFileNameWithoutExtension(strStartScreenName).ToUpper)
            ScreenHistoryIdx = 1
            'oFormPlayer.SuspendLayout()
            OpenScreen(Common.aScreens(strStartScreenName).FileName, Nothing, False)
            'oFormPlayer.ResumeLayout()

        Catch ex As Exception
            MessageBox.Show("Cannot Play this GUI: " & ex.Message & vbCrLf & "Please review your VGDD project and retry.", "Player error")
        End Try
    End Sub

#End If

    Private Shared Function OpenScreen(ByVal strScreenFileName As String, ByRef document As XmlDocument, ByVal Overlay As Boolean) As String
        'Try
        Common.PlayerIsActive = True
        Common.ProjectLoading = True

        Dim oPropValue As Object
        Dim ScreenName As String = Path.GetFileNameWithoutExtension(strScreenFileName)
        If Not File.Exists(strScreenFileName) Then
            MessageBox.Show("Could not find Screen" & vbCrLf & strScreenFileName, "Missing Screen")
            Return ""
        End If

        Dim sr As StreamReader = New StreamReader(New FileStream(strScreenFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        Dim cleandown As String = sr.ReadToEnd
        If Not cleandown.StartsWith("<") Then
            cleandown = ("<DOCUMENT_ELEMENT>" + (cleandown + "</DOCUMENT_ELEMENT>"))
        End If
        Dim doc As XmlDocument = New XmlDocument
        doc.LoadXml(cleandown)
        Dim strErrors As String = String.Empty
        document = doc

        If Not Overlay Then
            oFormPlayer.Panel1.Controls.Clear()
            'oFormPlayer.Panel1.SuspendLayout()
        End If

        For Each oNode As XmlNode In doc.DocumentElement.ChildNodes
            Dim aPar As String() = oNode.Attributes("type").Value.ToString.Split(",")
            Dim strObjType As String = aPar(0)
            If strObjType = "VGDDMicrochip.Screen" Then
                strObjType = "VGDD.VGDDScreen"
            End If
            Dim objType As Type = Nothing
            For Each assembly As Assembly In AppDomain.CurrentDomain.GetAssemblies
                objType = assembly.GetType(strObjType, False, True)
                If objType IsNot Nothing Then
                    Exit For
                End If
            Next
#If PlayerMonolitico Then
            If objType Is Nothing Then
                For Each Assembly As Assembly In MyEmbeddedAssemblies.Values
                    Debug.Print(Assembly.GetTypes.ToString)
                    objType = Assembly.GetType(strObjType, False, True)
                    If objType IsNot Nothing Then
                        Exit For
                    End If
                Next
            End If
#End If
            If objType Is Nothing Then
                strErrors &= "Could not load Widget type " & strObjType & vbCrLf
            Else
                Dim obj As Object = System.Activator.CreateInstance(objType)
                'If objType Is GetType(VGDD.VGDDScreen) Then
                '    Dim strMasterScreenName As String = CType(obj, VGDD.VGDDScreen).MasterScreen
                '    If strMasterScreenName <> "" Then
                '        Dim strMasterScreenFileName As String = System.IO.Path.Combine(Common.ProjectPath, Common.aScreens(strMasterScreenName).FileName)
                '        OpenScreen(strMasterScreenFileName, Nothing)
                '    End If
                'End If

                If TypeOf (obj) Is System.ComponentModel.ISupportInitialize Then
                    CType(obj, System.ComponentModel.ISupportInitialize).BeginInit()
                End If

                'Dim oPropInfo As PropertyInfo() = objType.GetProperties
                For Each oPropNode As XmlNode In oNode.ChildNodes
                    If oPropNode.Name = "Property" Then
                        Dim strPropertyName As String = oPropNode.Attributes("name").Value
                        oPropValue = Nothing
                        Select Case strPropertyName
                            Case "VGDDEvents"
                                Dim oEventCollection As New VGDDEventsCollection(oPropNode)
                                If oEventCollection.Count > 0 Then
                                    Dim oControl As Control = obj
                                    oControl.Tag = oEventCollection
                                    For Each oEvent As VGDDEvent In oEventCollection
                                        Select Case oEvent.PlayerEvent
                                            Case "MOUSE_DOWN"
                                                AddHandler oControl.MouseDown, New MouseEventHandler(AddressOf PlayerEvent_MouseDown)
                                            Case "MOUSE_UP"
                                                AddHandler oControl.MouseUp, New MouseEventHandler(AddressOf PlayerEvent_MouseUp)
                                            Case "MOUSE_LEAVE"
                                                'AddHandler oControl.MouseLeave, New EventHandler(AddressOf PlayerEvent_MouseLeave)
                                            Case "CHECKED", "UNCHECKED"
                                                AddHandler CType(oControl, VGDDMicrochip.CheckBox).CheckedChanged, New EventHandler(AddressOf PlayerEvent_CheckedChange)
                                            Case ""
                                            Case Else
                                                Debug.Print("Unhandled")
                                        End Select
                                    Next
                                End If
                            Case "Segments"
                                Dim value As Object = Nothing
                                Dim classType As Type = GetType(SegmentsCollection)
                                If classType IsNot Nothing Then
                                    oPropValue = Activator.CreateInstance(classType, oPropNode)
                                    Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(obj)(strPropertyName)
                                    oProp.SetValue(obj, oPropValue)
                                End If
                                'oPropValue = New VGDD.GaugeSegmentsCollection(oPropNode)
                                'Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(obj)(strPropertyName)
                                'Try
                                '    oProp.SetValue(obj, oPropValue)
                                'Catch ex As Exception
                                'End Try
                            Case "Controls"
                            Case "MasterScreens"
                                'Dim strMasterScreenNames As String() = oPropNode.InnerText.Split(",")
                                Dim aMasterScreens As New Collection
                                For Each child As XmlNode In oPropNode.ChildNodes
                                    If child.Name.Equals("Item") Then
                                        Dim strMasterScreenName As String = child.InnerText
                                        aMasterScreens.Add(strMasterScreenName)
                                        If Common.aScreens.Contains(strMasterScreenName) Then
                                            Dim strMasterScreenFileName As String = System.IO.Path.Combine(Common.VGDDProjectPath, Common.aScreens(strMasterScreenName).FileName)
                                            OpenScreen(strMasterScreenFileName, Nothing, True)
                                        End If
                                    End If
                                Next
                                CType(obj, VGDD.VGDDScreen).MasterScreens = aMasterScreens
                            Case "MasterScreenBitmap", "ScreenShot"
                            Case "TransparentColour"
                                Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(obj)(strPropertyName)
                                If oProp IsNot Nothing Then
                                    oFormPlayer.Panel1.TransparentColour = oProp.Converter.ConvertFromInvariantString(oPropNode.InnerText)
                                End If
                            Case Else
                                Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(obj)(strPropertyName)
                                If oProp IsNot Nothing Then
                                    If oPropNode.FirstChild IsNot Nothing Then
                                        If oPropNode.FirstChild.Name = "#text" Then
                                            Try
                                                oPropValue = oProp.Converter.ConvertFromInvariantString(oPropNode.InnerText.Replace("OnePixel", "NORMAL_LINE").Replace("ThreePixels", "THICK_LINE"))
                                            Catch ex As Exception
                                                MessageBox.Show(ex.Message & vbCrLf & "Object " & obj.ToString & "Error setting property " & strPropertyName & " to value " & oPropNode.InnerText)
                                            End Try
                                        ElseIf oPropNode.FirstChild.Name = "Binary" Then
                                            Try
                                                Dim data() As Byte = Convert.FromBase64String(oPropNode.FirstChild.InnerText)
                                                'oPropValue = oProp.Converter.ConvertFrom(Nothing, CultureInfo.InvariantCulture, data)
                                                Dim formatter As BinaryFormatter = New BinaryFormatter
                                                Dim stream As MemoryStream = New MemoryStream(data)
                                                oPropValue = formatter.Deserialize(stream)
                                            Catch ex As Exception
                                            End Try
                                        ElseIf oPropNode.FirstChild.Name = "Item" Then
                                            Try
                                                If oProp.Converter.CanConvertFrom(GetType(XmlNode)) Then
                                                    oPropValue = oProp.Converter.ConvertFrom(oPropNode)
                                                Else
                                                    Dim childList As IList = oProp.GetValue(obj)
                                                    childList.Clear()
                                                    For Each oChildNode As XmlNode In oPropNode.ChildNodes
                                                        childList.Add(oChildNode.InnerText)
                                                    Next
                                                    oPropValue = childList
                                                End If
                                            Catch ex As Exception
                                            End Try
                                        Else
                                            Debug.Print("?")
                                        End If
                                    End If
                                    If strObjType.Contains("VGDDCustom") Then
                                        Try
                                            Dim oCustom As VGDDCustom = obj
                                            'Dim cTd As New VGDDCustomTypeDescriptor(TypeDescriptor.GetProvider(GetType(VGDDCustom)).GetTypeDescriptor(oCustom), oCustom)
                                            If oCustom._CustomProperties Is Nothing OrElse Not oCustom._CustomProperties.Contains(strPropertyName.ToUpper) Then
                                                If oProp IsNot Nothing Then
                                                    oProp.SetValue(obj, oPropValue)
                                                End If
                                            Else
                                                obj.item(strPropertyName) = oPropValue
                                            End If
                                        Catch ex As Exception
                                        End Try
                                    Else
                                        Try
                                            If oPropValue IsNot Nothing Then
                                                oProp.SetValue(obj, oPropValue)
                                            End If
                                        Catch ex As Exception
                                            'MessageBox.Show(ex.Message)
                                        End Try
                                    End If
                                End If
                        End Select
                    End If
                Next

                If TypeOf (obj) Is System.ComponentModel.ISupportInitialize Then
                    CType(obj, System.ComponentModel.ISupportInitialize).EndInit()
                End If

                If objType Is GetType(VGDD.VGDDScreen) Then
                    oFormPlayer.CurrentScreen = obj
                    If oFormPlayer.CurrentScreen.Overlay Then
                        oFormPlayer.Panel1.BackgroundImage = oFormPlayer.bmpLastScreen
                        oFormPlayer.Panel1.BackgroundImageLayout = ImageLayout.None
                    Else
                        oFormPlayer.Panel1.BackgroundImage = Nothing
                        oFormPlayer.Panel1.BackColor = oFormPlayer.CurrentScreen.BackColor
                    End If
                    'oFormPlayer.ResizeMe()
                Else
                    oFormPlayer.Panel1.Controls.Add(obj)
                End If
            End If
        Next
        If strErrors <> String.Empty Then
            MessageBox.Show(strErrors, "Errors loading screen")
        End If
        Common.ProjectLoading = False
        'For Each obj As Object In oFormPlayer.Panel1.Controls
        '    If TypeOf obj Is IVGDDBase Then
        '        CType(obj, IVGDDBase).Zorder = CType(obj, IVGDDBase).Zorder
        '    End If
        'Next
        With oFormPlayer
            If Not Overlay Then
                Common.SetNewZOrder(oFormPlayer.Panel1)
                ''oFormPlayer.Panel1.Refresh()
                '.Panel1.ResumeLayout()
            End If
            .Show()
            .Activate()
            .BringToFront()
            .Focus()
            Application.DoEvents()
            .CheckHistory()
        End With

        'Catch ex As Exception
        '    MessageBox.Show(ex.Message, "Error loading screen " & strScreenFileName)
        'End Try

        Return ""
    End Function

    Private Shared Sub PlayerEvent_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs)
        PlayerEvent(sender, "MOUSE_DOWN")
    End Sub

    Private Shared Sub PlayerEvent_MouseUp(ByVal sender As System.Object, ByVal e As System.EventArgs)
        PlayerEvent(sender, "MOUSE_UP")
    End Sub

    Private Shared Sub PlayerEvent_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'If MouseButtons.Left Then
        '    PlayerEvent(sender, "MOUSE_LEAVE")
        'End If
    End Sub

    Private Shared Sub PlayerEvent_CheckedChange(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim oChk As VGDDMicrochip.CheckBox = sender
            If oChk IsNot Nothing Then
                If oChk.Checked Then
                    PlayerEvent(sender, "CHECKED")
                Else
                    PlayerEvent(sender, "UNCHECKED")
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Sub PlayerEvent(ByVal sender As System.Object, ByVal EventType As String)
        Dim strObjId As String, strLine As String
        If Not IsSigned Then
            Static blnShowedLimitedWarning As Boolean = False
            If Not blnShowedLimitedWarning Then
                Dim oFrmLimited As New frmLimited
                blnShowedLimitedWarning = (oFrmLimited.ShowDialog = DialogResult.Cancel)
            End If
            Exit Sub
        End If
        Dim oControl As Control = sender
        'MessageBox.Show("Click to " & oControl.Tag)
        Dim oEventCollection As VGDDEventsCollection = sender.tag
        If oEventCollection IsNot Nothing Then
            For Each oEvent As VGDDEvent In oEventCollection
                If oEvent.PlayerEvent = EventType Then
                    Dim strEventCode As String = oEvent.Code & vbCrLf
                    For Each strLineOrig As String In strEventCode.Split(vbCrLf)
                        Try
                            'Dim strEventCode As String = oEvent.Code.Replace(" ", "").Replace("screenState==", "") & vbCrLf
                            Dim intPos As Integer
                            intPos = strLineOrig.IndexOf("screenState=")
                            If intPos >= 0 And Not strLineOrig.Contains("screenState==") Then
                                strLine = strLineOrig.Substring(intPos + 12).Trim
                                strLine = strLine.Substring(0, strLine.IndexOf(";")).Replace("CREATE_", "").Trim
                                If Common.aScreens.Contains(strLine) Then
                                    ScreenHistory.Add(Common.aScreens(strLine).Name)
                                    ScreenHistoryIdx = ScreenHistory.Count
                                    Dim strScreenFileName As String = System.IO.Path.Combine(Common.VGDDProjectPath, Common.aScreens(strLine).FileName)
                                    'oFormPlayer.SuspendLayout()
                                    OpenScreen(strScreenFileName, Nothing, False)
                                    'oFormPlayer.ResumeLayout()
                                    Exit Sub
                                End If
                            End If

                            If strLineOrig.Contains("SetText(") Or strLineOrig.Contains("SetBitmap(") Then
                                strObjId = ""
                                Dim strNewValue As String = ""
                                intPos = strLineOrig.IndexOf("GOLFindObject(")
                                If intPos > 0 Then
                                    strLine = strLineOrig.Substring(intPos + 14)
                                    strObjId = strLine.Split(",")(0)
                                    strObjId = strObjId.Substring(strObjId.LastIndexOf("_") + 1)
                                    strObjId = strObjId.Substring(0, strObjId.IndexOf(")"))
                                    strNewValue = strLine.Split(",")(1).Trim
                                ElseIf strLineOrig.Contains("pObj") Then
                                    strObjId = oControl.Name
                                    strNewValue = strLineOrig.Split(",")(1).Trim
                                End If
                                If strObjId <> "" Then
                                    If strNewValue.StartsWith("(") Then
                                        strNewValue = strNewValue.Substring(strNewValue.IndexOf(")") + 1).Trim
                                    End If
                                    Dim oControlToUpdate() As Control = oFormPlayer.Panel1.Controls.Find(strObjId, False)
                                    If oControlToUpdate.Length > 0 Then
                                        If strLineOrig.Contains("SetText(") Then
                                            If strNewValue.StartsWith("""") Then strNewValue = strNewValue.Substring(1)
                                            If strNewValue.Contains("""") Then
                                                strNewValue = strNewValue.Substring(0, strNewValue.IndexOf(""""))
                                            ElseIf strNewValue.Contains(")") Then
                                                strNewValue = strNewValue.Substring(0, strNewValue.IndexOf(")"))
                                            End If
                                            oControlToUpdate(0).Text = strNewValue
                                        ElseIf strLineOrig.Contains("SetBitmap(") Then
                                            If strNewValue.StartsWith("&bmp") Then
                                                strNewValue = strNewValue.Substring(4)
                                            ElseIf strNewValue.StartsWith("&") Then
                                                strNewValue = strNewValue.Substring(1)
                                            End If
                                            If strNewValue.Contains(")") Then strNewValue = strNewValue.Substring(0, strNewValue.IndexOf(")"))
                                            If TypeOf oControlToUpdate(0) Is IVGDDWidgetWithBitmap Then
                                                CType(oControlToUpdate(0), IVGDDWidgetWithBitmap).BitmapName = strNewValue
                                            Else
                                                Debug.Print("Not handled!")
                                            End If
                                        End If
                                        oControlToUpdate(0).Refresh()
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Next
                End If
            Next
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        'oFormPlayer.Dispose()
    End Sub

    Private Shared Sub oFormPlayer_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles oFormPlayer.FormClosed
        If Mode = RunMode.Standalone Then
            Application.Exit()
        End If
        Common.PlayerIsActive = False
    End Sub

    Private Shared Sub oFormPlayer_HistoryBack() Handles oFormPlayer.HistoryBack
        ScreenHistoryIdx -= 1
        If ScreenHistoryIdx > 0 Then
            OpenScreen(Common.aScreens(ScreenHistory(ScreenHistoryIdx).ToString).FileName, Nothing, False)
        Else
            ScreenHistoryIdx = 1
        End If
    End Sub

    Private Shared Sub oFormPlayer_HystoryForward() Handles oFormPlayer.HystoryForward
        ScreenHistoryIdx += 1
        If ScreenHistoryIdx <= ScreenHistory.Count Then
            OpenScreen(Common.aScreens(ScreenHistory(ScreenHistoryIdx).ToString).FileName, Nothing, False)
        Else
            ScreenHistoryIdx = ScreenHistory.Count
        End If
    End Sub

    Private Shared Sub oFormPlayer_UserSelectedScreen() Handles oFormPlayer.UserSelectedScreen
        Try
            Dim strScreenFileName As String = System.IO.Path.Combine(Common.VGDDProjectPath, Common.aScreens(oFormPlayer.UserSelectedScreen_Screen).FileName)
            OpenScreen(strScreenFileName, Nothing, False)
        Catch ex As Exception

        End Try
    End Sub
End Class

Public Class PlayerPanel
    Inherits Panel
    Private _TransparentColour As Color

    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.Opaque, False)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, False)
    End Sub

    Public Property TransparentColour As Color
        Get
            Return _TransparentColour
        End Get
        Set(value As Color)
            _TransparentColour = value
        End Set
    End Property
End Class

'End Namespace