Imports System.Xml
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.ComponentModel.Design.Serialization
Imports System.Reflection
Imports System.Text
Imports VGDDCommon
Imports System.Globalization
Imports System.Runtime.Serialization.Formatters.Binary

Namespace VGDDPlayerLib

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

        Public Shared Sub PlayPackage(ByVal PackageFile As String)
            Common.BitmapsBinPath = Path.GetDirectoryName(PackageFile)
            Dim xmlPlayerPackage As New XmlDocument
            xmlPlayerPackage.Load(PackageFile)
            PlayPackage(xmlPlayerPackage)
        End Sub

        Public Shared Sub PlayPackage(ByVal oXmlPlayerPackage As XmlDocument)
            Dim strStartScreenName As String = ""
            PackageName = oXmlPlayerPackage.DocumentElement.Attributes("Name").Value
            IsSigned = False
            If oXmlPlayerPackage.DocumentElement.Attributes("Sign") IsNot Nothing Then
                Dim MD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
                Dim dataMd5 As Byte() = MD5.ComputeHash(Encoding.Default.GetBytes(oXmlPlayerPackage.DocumentElement.InnerXml & "VGDDPlayer"))
                Dim sb As New StringBuilder()
                For j As Integer = 0 To dataMd5.Length - 1
                    sb.AppendFormat("{0:x2}", dataMd5(j))
                Next
                If oXmlPlayerPackage.DocumentElement.Attributes("Sign").Value = sb.ToString Then
                    IsSigned = True
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
            OpenScreen(Common.aScreens(strStartScreenName).FileName, Nothing)
        End Sub

        Public Shared Sub Play(ByVal PlayerFile As String, ByVal Signed As Boolean)
            Common.BitmapsBinPath = Path.GetDirectoryName(PlayerFile)
            Dim xmlPlayer As New XmlDocument
            xmlPlayer.Load(PlayerFile)
            Play(xmlPlayer, Signed)
        End Sub

        Public Shared Sub Play(ByVal xmlPlayer As XmlDocument, ByVal Signed As Boolean)
            Dim strStartScreenName As String = ""

            IsSigned = Signed
            'Try
            Dim oPlayerNode As XmlNode = xmlPlayer.DocumentElement
            Dim ProjectFileName As String = oPlayerNode.Attributes("ProjectFileName").Value

            Dim xmlProject As New XmlDocument
            xmlProject.Load(ProjectFileName)
            Dim oProjectNode As XmlNode = xmlProject.DocumentElement

            oFormPlayer = New frmPlayer
            If IsSigned Then
                oFormPlayer.ToolStripAnimation.Enabled = True
            End If

            If strStartScreenName = "" Then
                strStartScreenName = Common.aScreens.Item(1).Name
            End If
            ScreenHistory.Clear()
            ScreenHistory.Add(strStartScreenName, Path.GetFileNameWithoutExtension(strStartScreenName).ToUpper)
            ScreenHistoryIdx = 1
            OpenScreen(Common.aScreens(strStartScreenName).FileName, Nothing)
        End Sub

        Private Shared Function OpenScreen(ByVal strScreenFileName As String, ByRef document As XmlDocument) As String
            'Try
            Dim oPropValue As Object
            Dim ScreenName As String = Path.GetFileNameWithoutExtension(strScreenFileName)
            If Not File.Exists(strScreenFileName) Then
                MessageBox.Show("Could not find Screen" & vbCrLf & strScreenFileName, "Missing Screen")
                Return ""
            End If

            oFormPlayer.Panel1.Controls.Clear()
            'oFormPlayer.SuspendLayout()

            Dim sr As StreamReader = New StreamReader(strScreenFileName)
            Dim cleandown As String = sr.ReadToEnd
            If Not cleandown.StartsWith("<") Then
                cleandown = ("<DOCUMENT_ELEMENT>" + (cleandown + "</DOCUMENT_ELEMENT>"))
            End If
            Dim doc As XmlDocument = New XmlDocument
            doc.LoadXml(cleandown)
            document = doc
            For Each oNode As XmlNode In doc.DocumentElement.ChildNodes
                Dim aPar As String() = oNode.Attributes("type").Value.ToString.Split(",")
                Dim strObjType As String = aPar(0)
                If strObjType = "VGDDMicrochip.Screen" Then
                    strObjType = "VGDD.VGDDScreen"
                End If
                Dim objType As Type = Nothing
                For Each assembly As Assembly In AppDomain.CurrentDomain.GetAssemblies
                    objType = assembly.GetType(strObjType, False, True)
                    If objType IsNot Nothing Then Exit For
                Next
                If objType IsNot Nothing Then
                    'If objType Is GetType(VGDDScreen) Then
                    'Else
                    Dim obj As Object = System.Activator.CreateInstance(objType)

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
                                        AddHandler oControl.Click, New EventHandler(AddressOf PlayerEvent)
                                    End If
                                Case "Controls"
                                Case Else
                                    Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(obj)(strPropertyName)
                                    If oProp IsNot Nothing Then
                                        If oPropNode.FirstChild IsNot Nothing Then
                                            If oPropNode.FirstChild.Name = "#text" Then
                                                oPropValue = oProp.Converter.ConvertFromInvariantString(oPropNode.InnerText)
                                            ElseIf oPropNode.FirstChild.Name = "Binary" Then
                                                Try
                                                    Dim data() As Byte = Convert.FromBase64String(oPropNode.FirstChild.InnerText)
                                                    'oPropValue = oProp.Converter.ConvertFrom(Nothing, CultureInfo.InvariantCulture, data)
                                                    Dim formatter As BinaryFormatter = New BinaryFormatter
                                                    Dim stream As MemoryStream = New MemoryStream(data)
                                                    oPropValue = formatter.Deserialize(stream)
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
                                            If oPropValue IsNot Nothing Then
                                                oProp.SetValue(obj, oPropValue)
                                            End If
                                        End If
                                    End If
                            End Select
                        End If
                    Next
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
            For Each oControl As Control In oFormPlayer.Panel1.Controls
                If oControl.GetType.ToString.Contains("Picture") Then
                    Dim oPicture As VGDDMicrochip.Picture = oControl
                    oPicture.Zorder = oPicture.Zorder 'Per il BringToFront
                End If
            Next
            With oFormPlayer
                '.ResumeLayout()
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

        Private Shared Sub PlayerEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
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
                    Dim strEventCode As String = oEvent.Code.Replace(" ", "").Replace("screenState==", "") & vbCrLf
                    Dim intPos As Integer = strEventCode.IndexOf("screenState=")
                    If intPos >= 0 Then
                        strEventCode = strEventCode.Substring(intPos + 12)
                        strEventCode = strEventCode.Substring(0, strEventCode.IndexOf(";")).Replace("CREATE_", "")
                        If Common.aScreens.Contains(strEventCode) Then
                            ScreenHistory.Add(Common.aScreens(strEventCode).Name)
                            ScreenHistoryIdx = ScreenHistory.Count
                            Dim strScreenFileName As String = System.IO.Path.Combine(Common.ProjectPath, Common.aScreens(strEventCode).FileName)
                            OpenScreen(strScreenFileName, Nothing)
                            Exit Sub
                        End If
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
        End Sub

        Private Shared Sub oFormPlayer_HistoryBack() Handles oFormPlayer.HistoryBack
            ScreenHistoryIdx -= 1
            If ScreenHistoryIdx > 0 Then
                OpenScreen(Common.aScreens(ScreenHistory(ScreenHistoryIdx).ToString).FileName, Nothing)
            Else
                ScreenHistoryIdx = 1
            End If
        End Sub

        Private Shared Sub oFormPlayer_HystoryForward() Handles oFormPlayer.HystoryForward
            ScreenHistoryIdx += 1
            If ScreenHistoryIdx <= ScreenHistory.Count Then
                OpenScreen(Common.aScreens(ScreenHistory(ScreenHistoryIdx).ToString).FileName, Nothing)
            Else
                ScreenHistoryIdx = ScreenHistory.Count
            End If
        End Sub

        Private Shared Sub oFormPlayer_UserSelectedScreen(ByVal ScreenName As String) Handles oFormPlayer.UserSelectedScreen
            Dim strScreenFileName As String = System.IO.Path.Combine(Common.ProjectPath, Common.aScreens(ScreenName).FileName)
            OpenScreen(strScreenFileName, Nothing)
        End Sub
    End Class

End Namespace