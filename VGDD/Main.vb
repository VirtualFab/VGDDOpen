Imports System.IO
Imports System.Xml

Module Main
    Public Const VIRTUALFABSITE As String = "http://lm.virtualfab.it"
    Public Const CHECKUPDATEURL As String = VIRTUALFABSITE & "/lm/checkupdate.php"
    Public Const DOWNLOADURL As String = "http://virtualfab.it/mediawiki/index.php/Downloads_Page"

    Public oMainShell As MainShell
    Public LayoutFileName As String = Path.Combine(Path.GetDirectoryName(Application.CommonAppDataPath), "VGDDLayout.vwl")

    Private WithEvents oUpdateCheck As UpdateCheck

    '<System.Diagnostics.DebuggerStepThrough()> _
    Public Sub Main()
        Try
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf MyAssemblyResolveEventHandler
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            oUpdateCheck = New UpdateCheck
            oUpdateCheck.CheckNewVersion(CHECKUPDATEURL)

            System.Threading.Thread.Sleep(500)
            If (Windows.Forms.Control.ModifierKeys And Keys.Shift) OrElse Command.ToUpper.Contains("/CLEAR") Then
                If MessageBox.Show("Clear all VGDD temporary files?", "Clearing", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                    For Each strDirectory As String In Directory.GetDirectories(Application.CommonAppDataPath.Substring(0, Application.CommonAppDataPath.IndexOf("VGDD") + 4))
                        Try
                            Directory.Delete(strDirectory, True)
                        Catch ex As Exception
                        End Try
                    Next

                    For Each strDirectory As String In Directory.GetDirectories(Path.GetFullPath(Path.Combine(Path.Combine(Application.LocalUserAppDataPath, ".."), "..")))
                        Try
                            Directory.Delete(strDirectory, True)
                        Catch ex As Exception
                        End Try
                    Next

                    If File.Exists(LayoutFileName) Then
                        Try
                            File.Delete(LayoutFileName)
                        Catch ex As Exception

                        End Try
                    End If

                End If
            End If
            If Command.ToUpper.Contains("/IPC") Then
                Dim aArgs As String() = Command().Split(" ")
                For i As Integer = 0 To aArgs.Length - 1
                    If aArgs(i).ToUpper = "/IPC" Then
                        Dim strPar As String = aArgs(i + 1)
                        My.Settings.MplabXIpcIpAddress = strPar.Split(":")(0)
                        My.Settings.MplabXIpcPort = strPar.Split(":")(1)
                        My.Settings.MplabXUseIpc = True
                        My.Settings.Save()
                    End If
                Next
            End If
            oMainShell = New MainShell
            Try
                oMainShell.Show()
            Catch ex As Exception
                CatchException(ex)
                oMainShell.DockLayoutReset()
            End Try
            Application.Run(oMainShell)
        Catch ex As Exception
            CatchException(ex)
        End Try
        End
    End Sub

    Function MyAssemblyResolveEventHandler(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly
        Dim NewAssembly As Assembly
        Static MyEmbeddedAssemblies As Dictionary(Of String, Assembly)
        Try
            'This handler is called only when the common language runtime tries to bind to the assembly and fails. 
            If MyEmbeddedAssemblies Is Nothing Then
                MyEmbeddedAssemblies = New Dictionary(Of String, Assembly)
            End If
            Dim strAssemblyName As String = args.Name.Split(",")(0)
            If strAssemblyName = "System.XmlSerializers" Then Return Nothing
            If strAssemblyName = "VGDD" Then strAssemblyName = "VGDDCommonEmbedded"
            If MyEmbeddedAssemblies.ContainsKey(strAssemblyName) Then
                Return MyEmbeddedAssemblies(strAssemblyName)
            End If
            NewAssembly = LoadAssemblyFromExecutingAssembly(strAssemblyName)
            If NewAssembly Is Nothing AndAlso oMainShell IsNot Nothing Then
                NewAssembly = oMainShell.GetLibraryAssembly(strAssemblyName)
            End If
            If NewAssembly IsNot Nothing Then
                MyEmbeddedAssemblies.Add(strAssemblyName, NewAssembly)
                'MessageBox.Show("Found Assembly " & strAssemblyName)
            ElseIf "Microsoft.mshtml".IndexOf(strAssemblyName) = -1 Then
                MessageBox.Show("Can't find Assembly " & strAssemblyName)
            End If
            Return NewAssembly

        Catch ex As Exception
            MessageBox.Show("MyAssemblyResolveEventHandler error: " & ex.Message)
        End Try
        Return Nothing
    End Function

    Public Function LoadAssemblyFromExecutingAssembly(ByVal AssemblyName As String) As Assembly
        Dim s As IO.Stream = Nothing
        If AssemblyName = "VGDD" Or AssemblyName.StartsWith("VGDDCommon") Then
            s = Assembly.GetExecutingAssembly().GetManifestResourceStream("VGDDCommonEmbedded.dll")
        Else
            s = Assembly.GetExecutingAssembly().GetManifestResourceStream(AssemblyName & ".dll")
        End If
        If s IsNot Nothing Then
            Dim block(s.Length) As Byte
            s.Read(block, 0, block.Length)
            Return Assembly.Load(block)
        End If
        Return Nothing
    End Function

    Public Sub CatchException(ex As Exception)
        MessageBox.Show(ex.Message)
        Dim oXmlDocExcemptions As New XmlDocument
        Dim FileName As String = ""
        Dim strFileMessage As String = ""
        Try
            Try
                If ex.Message.Contains("Attempted to read or write protected memory") _
                    Or ex.Message.Contains("GDI") _
                    Or ex.Message.Contains("SafeNativeMethods.Gdip") _
                    Or ex.StackTrace.Contains("SafeNativeMethods.Gdip") _
                    Or ex.Message.Contains("FromHdcInternal") _
                    Or ex.Message.Contains("ResourceReader.DeserializeObject") _
                    Or ex.Message.Contains("创建窗体时出错。有关详细信息，请参阅") Then
                    If VGDDCommon.Common.OsVersion.StartsWith("5.") Then
                        If MessageBox.Show(ex.Message & vbCrLf & vbCrLf & "VGDD cannot be safely run on a Windows XP system without installing at least:" & vbCrLf &
                                            "1) Windows XP Service Pack 3" & vbCrLf &
                                            "2) Microsoft Dot.Net Framework 3.5 SP1" & vbCrLf & vbCrLf &
                                            "Do you want to open Microsoft Site to download Dot.Net Framework 3.5 SP1?",
                                            "Unhandled exception - Windows XP without SP3 detected", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.Yes Then
                            VGDDCommon.Common.RunBrowser("http://www.microsoft.com/en-us/download/details.aspx?id=22")
                        End If
                    Else
                        If MessageBox.Show(ex.Message & vbCrLf & vbCrLf & "The application encountered a system error and could not be fully functional anymore." & vbCrLf & vbCrLf &
                                            "Please close the application a start over. Also make a check-up of your system if you can since this kind of errors should not happen." & vbCrLf &
                                            "You can also try to (re)install the Microsoft Dot.Net Framework 3.5 SP1 from Microsoft site to ensure it's not corrupted." & vbCrLf & vbCrLf &
                                            "Do you want to open Microsoft Site to download Dot.Net Framework 3.5 SP1?",
                                            "System memory problem - Application became unstable", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.Yes Then
                            VGDDCommon.Common.RunBrowser("http://www.microsoft.com/en-us/download/details.aspx?id=22")
                        End If
                        Exit Sub
                    End If
                ElseIf ex.StackTrace.Contains("Cannot access a disposed object.") Then
                    Exit Sub
                ElseIf ex.StackTrace.Contains("AdornerWindow.MouseHook.MouseHookProc") Then
                    'MessageBox.Show("The application encountered a system error and must be closed." & vbCrLf & vbCrLf & _
                    '                "This is a known Microsoft Dot.Net Windows.Forms.Design issue and might be caused by a too complex" & _
                    '                "design action, such as resizing different objects at the same time." & vbCrLf & vbCrLf & _
                    '                "Please understand this is not a VGDD issue and it will be addressed only when the Dot.Net libraries will be fixed.", _
                    '                "AdornerWindow.MouseHook.MouseHookProc bug!")
                    Exit Sub
                End If

                Dim StackTraceText As String = ""
                If ex.StackTrace IsNot Nothing Then
                    StackTraceText = ex.StackTrace.ToString
                End If

                FileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.Windows.Forms.Application.CommonAppDataPath)), My.Application.Info.Title), "Exceptions.xml")

                If IO.File.Exists(FileName) Then
                    oXmlDocExcemptions.Load(FileName)
                Else
                    oXmlDocExcemptions.AppendChild(oXmlDocExcemptions.CreateElement("Exceptions"))
                End If

                Dim oException As XmlNode = oXmlDocExcemptions.CreateNode(XmlNodeType.Element, "", "Exception", "")
                Dim oNode As XmlNode

                oNode = oException.AppendChild(oXmlDocExcemptions.CreateNode(XmlNodeType.Element, "", "Date_Time", ""))
                oNode.InnerText = Date.UtcNow.ToString

                oNode = oException.AppendChild(oXmlDocExcemptions.CreateNode(XmlNodeType.Element, "", "Message", ""))
                oNode.InnerText = ex.Message

                oNode = oException.AppendChild(oXmlDocExcemptions.CreateNode(XmlNodeType.Element, "", "StackTrace", ""))
                oNode.InnerText = StackTraceText

                oXmlDocExcemptions.DocumentElement.AppendChild(oException)
                Try
                    oXmlDocExcemptions.Save(FileName)
                Catch exInternal As Exception
                End Try

            Catch exInternal As Exception
                '#If CONFIG = "Debug" Then
                MessageBox.Show("Error1 in CatchException recording: " & exInternal.Message, "Exception in exception")
                '#End If
            End Try
            strFileMessage = String.Format("Exception has been recorded to {0}", FileName)

            Dim f As New frmExceptionPanel(ex, "Unhandled exception", strFileMessage)

            Try
                f.ShowDialog()
            Finally
                f.Dispose()
            End Try

            Dim strUrl As String = AssemblyExceptionLoggingURLAttribute.GetURL(System.Reflection.Assembly.GetExecutingAssembly())
            If strUrl <> String.Empty Then
                Try
                    Dim ur As Net.HttpWebRequest = CType(Net.HttpWebRequest.Create(strUrl), Net.HttpWebRequest)
                    ur.Accept = "*/*"
                    ur.AllowAutoRedirect = True
                    ur.UserAgent = My.Application.Info.Title & " " & My.Application.Info.Version.ToString
                    ur.Timeout = 60000
                    ur.Method = "POST"
                    ur.ContentType = "application/x-www-form-urlencoded"
                    Dim encoding As New System.Text.UTF8Encoding()
                    Dim strVersion As String = My.Application.Info.Version.ToString
                    If Debugger.IsAttached Then
                        strVersion &= "-DEBUGGING"
                    End If
                    Dim myOSInfo As String = String.Format("&OS={0}&SP={1}&Platform={2}",
                                        System.Web.HttpUtility.UrlEncode(VGDDCommon.Common.OsVersion),
                                        System.Web.HttpUtility.UrlEncode(VGDDCommon.Common.OsServicePack),
                                        System.Web.HttpUtility.UrlEncode(VGDDCommon.Common.OsPlatform))
                    Dim strPost As String = "a=" & My.Application.Info.Title & "&v=" & strVersion & myOSInfo
                    Dim fp As String = "?"
                    Using sw As New StringWriter
                        Using xtw As New XmlTextWriter(sw)
                            xtw.Formatting = Formatting.Indented
                            oXmlDocExcemptions.WriteTo(xtw)
                        End Using
                        strPost &= "&e=" & System.Web.HttpUtility.UrlEncode(sw.ToString)
                    End Using

                    Dim postByteArray() As Byte = encoding.GetBytes(strPost)
                    ur.ContentLength = postByteArray.Length
                    Dim postStream As IO.Stream = ur.GetRequestStream()
                    postStream.Write(postByteArray, 0, postByteArray.Length)
                    postStream.Close()
                    Dim dr As Net.HttpWebResponse = ur.GetResponse()
                    If dr.StatusCode = Net.HttpStatusCode.OK Then
                        Debug.Print("Post exception OK")
                        If File.Exists(FileName) Then
                            File.Delete(FileName)
                        End If
                        strFileMessage = "Exception has been signalled. It will be handled in next releases of " & My.Application.Info.Title
                    Else
                        Debug.Print("Post exception KO!! " & dr.StatusCode.ToString)
                    End If
                    dr.Close()

                Catch exInternal As Exception
                    '#If CONFIG = "Debug" Then
                    MessageBox.Show("Error in CatchException posting exception: " & exInternal.Message, "Exception in exception? (only DEBUG)")
                    '#End If
                End Try

            End If

        Catch ex2 As Exception
            MessageBox.Show("Error2 in CatchException recording: " & ex2.Message, "Exception in exception")
        End Try
    End Sub
End Module
