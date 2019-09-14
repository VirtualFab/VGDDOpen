Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports System.Text.RegularExpressions

Public Class Utils

#Region "Resources"
    'Public Shared myAssembly As System.Reflection.Assembly
    'Public Shared myAssemblyPath As String

    'Public Shared Sub EnsureResourceExists(ByVal strPathName As String, ByVal strResourceName As String)
    '    EnsureResourceExists(strPathName, strResourceName, False)
    'End Sub

    'Public Shared Sub EnsureResourceExists(ByVal strPathName As String, ByVal strResourceName As String, ByVal Force As Boolean)
    '    EnsureResourceExists(strPathName, strResourceName, False, System.Reflection.Assembly.GetExecutingAssembly())
    'End Sub

    Public Shared Sub EnsureResourceExists(ByVal strPathName As String, ByVal strResourceName As String, ByVal Force As Boolean, ByVal oAssembly As Assembly)
        Dim strDir As String = Path.GetDirectoryName(strPathName)
        If strDir <> "" And strDir.Substring(1, 1) <> ":" Then
            strDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), strDir)
            If Not Directory.Exists(strDir) Then
                Directory.CreateDirectory(strDir)
            End If
        End If
        strPathName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), strPathName)
        If Force Or Not File.Exists(strPathName) Then
            ExtractResourceToFile(strResourceName, strPathName, oAssembly)
        End If
    End Sub

    'Public Shared Function GetMyAssembly()
    '    If myAssembly Is Nothing Then
    '        myAssembly = System.Reflection.Assembly.GetExecutingAssembly()
    '        myAssemblyPath = myAssembly.GetName().Name().Replace(" ", "_")
    '    End If
    'End Function

    Public Shared Function GetAllResourceNames(ByVal WildCards As String, ByVal oAssembly As Assembly) As String()
        Dim AllResources(-1) As String
        For Each Resource As String In oAssembly.GetManifestResourceNames
            Dim blnExtract As Boolean = False
            If WildCards = "" Then
                blnExtract = True
            Else
                For Each strWildCard As String In WildCards.Split(",")
                    If FindFilesRegEx.Match(strWildCard, Resource) Then
                        blnExtract = True
                        Exit For
                    End If
                Next
            End If
            If blnExtract Then
                ReDim Preserve AllResources(AllResources.Length + 1)
                AllResources(AllResources.Length - 1) = Resource
            End If
        Next
        Return AllResources
    End Function

    Public Shared Function GetResourceAsString(ByVal FileName As String, ByVal Assembly As Assembly) As String
        Dim strRet As String = String.Empty
        Dim oStream As System.IO.Stream
        oStream = Assembly.GetManifestResourceStream(FileName)
        If oStream IsNot Nothing Then
            Using oReader As New StreamReader(oStream)
                strRet = oReader.ReadToEnd
            End Using
        End If
        Return strRet
    End Function

    Public Shared Function GetResource(ByVal FileName As String, ByVal Assembly As Assembly) As System.IO.Stream
        Dim oStream As System.IO.Stream
        oStream = Assembly.GetManifestResourceStream(Assembly.GetName().Name().Replace(" ", "_") & "." & FileName)
        If oStream IsNot Nothing Then Return oStream
        Return Assembly.GetManifestResourceStream(FileName)
    End Function

    'Public Shared Function GetResource(ByVal FileName As String) As System.IO.Stream
    '    GetMyAssembly()
    '    Return GetResource(FileName, myAssembly)
    'End Function

    Public Shared Function GetResourceStream(ByVal ResourceName As String, ByVal Assembly As Assembly) As Stream
        Dim oStream As Stream
        oStream = Assembly.GetManifestResourceStream(ResourceName)
        If oStream Is Nothing Then
            oStream = GetResource(ResourceName, Assembly)
        End If
        Return oStream
    End Function


    'Public Shared Function ExtractResourceToFile(ByVal strResourceName As String, ByVal strFileName As String) As Boolean
    '    GetMyAssembly()
    '    Return ExtractResourceToFile(strResourceName, strFileName, myAssembly)
    'End Function

    Public Shared Function ExtractResourceToFile(ByVal strResourceName As String, ByVal strFileName As String, ByVal Assembly As Assembly) As Boolean
        Dim oReader As BinaryReader, oStream As Stream
        oStream = Assembly.GetManifestResourceStream(strResourceName)
        If oStream Is Nothing Then
            oStream = GetResource(strResourceName, Assembly)
        End If
        If oStream Is Nothing Then
            Return False
        End If
        Try
            oReader = New BinaryReader(oStream)
        Catch ex As Exception
            'Ionic.Syslog.SyslogTraceListener.TraceErrorWithStack("ExtractResourceToFile: " & strResourceName & " risorsa non presente!", ex)
            Return False
        End Try
        'MakeBackup(strFileName)
        Try
            If File.Exists(strFileName) Then
                Try
                    File.Delete(strFileName)
                Catch ex As Exception
                End Try
            End If
            Dim oOutStream As New FileStream(strFileName, FileMode.Create)
            Dim oWriter As New BinaryWriter(oOutStream)
            Const BUFFER_SIZE As Integer = 2048

            Dim Buffer(BUFFER_SIZE) As Byte
            Dim ResLen As Integer = oReader.Read(Buffer, 0, BUFFER_SIZE)
            While ResLen > 0
                oWriter.Write(Buffer, 0, ResLen)
                ResLen = oReader.Read(Buffer, 0, BUFFER_SIZE)
            End While
            oWriter.Flush()
            oWriter.Close()
            'Trace.TraceInformation("ExtractResourceToFile: " & strResourceName & " estratto su " & strFileName)
            ExtractResourceToFile = True

        Catch ex As Exception
            'TODO: Log
            'MessageBox.Show(ex.Message & vbCrLf & "Check Project Path and change it accordingly", "Error extracting " & strFileName)
            Return False
        End Try
    End Function

    Public Shared Function FileExistsCaseSensitive(ByVal file As String) As Boolean
        Dim pathCheck As String = Path.GetDirectoryName(file)
        If Not Directory.Exists(pathCheck) Then Return False
        Dim filePart As String = Path.GetFileName(file)
        If String.IsNullOrEmpty(pathCheck) Then
            Throw New ArgumentException("The file must include a full path", file)
        End If
        Try
            Dim checkFiles As String() = Directory.GetFiles(pathCheck, filePart, SearchOption.TopDirectoryOnly)
            If checkFiles IsNot Nothing AndAlso checkFiles.Length > 0 Then
                'must be a binary compare
                Return Path.GetFileName(checkFiles(0)) = filePart
            End If

        Catch ex As Exception

        End Try
        Return False
    End Function

#End Region

#Region "Zip"
    Public Shared Function ExtractZippedResource(ByVal ResourceName As String, ByVal ExtractDir As String, ByVal Assembly As Assembly, ByVal Password As String) As Boolean
        Dim oStream As Stream
        oStream = Assembly.GetManifestResourceStream(ResourceName)
        If oStream Is Nothing Then
            oStream = GetResource(ResourceName, Assembly)
        End If
        If oStream Is Nothing Then Return False
        Return ZipExtract(oStream, ExtractDir, Password)
    End Function

    Public Shared Function ZipExtract(ByVal zipFilename As String, ByVal ExtractDir As String, ByVal Password As String) As Boolean
        Dim oStream As Stream = New FileStream(zipFilename, FileMode.Open, FileAccess.Read)
        Return ZipExtract(oStream, ExtractDir, Password)
    End Function

    Public Shared Function ZipExtract(ByVal stream As Stream, ByVal ExtractDir As String, ByVal Password As String) As Boolean
        Dim MyZipInputStream As ICSharpCode.SharpZipLib.Zip.ZipInputStream = Nothing
        Dim MyFileStream As FileStream = Nothing
        Try
            MyZipInputStream = New ICSharpCode.SharpZipLib.Zip.ZipInputStream(stream)
            If Password IsNot Nothing Then
                MyZipInputStream.Password = Password
            End If
            Dim MyZipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry = MyZipInputStream.GetNextEntry
            Directory.CreateDirectory(ExtractDir)
            While Not MyZipEntry Is Nothing
                If (MyZipEntry.IsDirectory) Then
                    Directory.CreateDirectory(Path.Combine(ExtractDir, MyZipEntry.Name))
                Else
                    If Not Directory.Exists(Path.Combine(ExtractDir, Path.GetDirectoryName(MyZipEntry.Name))) Then
                        Directory.CreateDirectory(Path.Combine(ExtractDir, Path.GetDirectoryName(MyZipEntry.Name)))
                    End If
                    MyFileStream = New FileStream(Path.Combine(ExtractDir, MyZipEntry.Name), FileMode.OpenOrCreate, FileAccess.Write)
                    Dim count As Integer
                    Dim buffer(4096) As Byte
                    count = MyZipInputStream.Read(buffer, 0, 4096)
                    While count > 0
                        MyFileStream.Write(buffer, 0, count)
                        count = MyZipInputStream.Read(buffer, 0, 4096)
                    End While
                    MyFileStream.Close()
                End If
                MyZipEntry = MyZipInputStream.GetNextEntry
            End While
        Catch ex As Exception
            Return False
        Finally
            If Not (MyZipInputStream Is Nothing) Then MyZipInputStream.Close()
            If Not (MyFileStream Is Nothing) Then MyFileStream.Close()
        End Try
        Return True
    End Function

#End Region

#Region "Files"

    Public Shared Function ReadFile(ByVal strFileName As String) As String
        ReadFile = String.Empty ' "VGDD Wizard: ***File " & strFileName & " does not exist***"
        If Not File.Exists(strFileName) Then
            Exit Function
        End If
        Try
            Dim sr As New StreamReader(strFileName, True)
            ReadFile = sr.ReadToEnd
            sr.Close()
        Catch ex As Exception
        End Try
        Return ReadFile
    End Function

    Public Shared Function WriteFile(ByVal strPathName As String, ByVal strFileContent As String) As Boolean
        Return WriteFile(strPathName, strFileContent, New System.Text.ASCIIEncoding)
    End Function

    Public Shared Function WriteFile(ByVal strPathName As String, ByVal strFileContent As String, ByVal encoding As System.Text.Encoding) As Boolean
        If strPathName = String.Empty Then Return False
        If File.Exists(strPathName) Then
            Try
                File.Delete(strPathName) 'Case sensitive patch
            Catch ex As Exception
            End Try
        End If
        If encoding Is System.Text.Encoding.UTF8 Then
            encoding = New System.Text.UTF8Encoding(False)
        End If
        Try
            Dim strDir As String = Path.GetDirectoryName(strPathName)
            If strDir <> String.Empty AndAlso Not Directory.Exists(strDir) Then
                Directory.CreateDirectory(strDir)
            End If
            Dim sw As New StreamWriter(strPathName, False, encoding)
            sw.Write(strFileContent)
            If Not strFileContent.EndsWith(vbCrLf) Then
                sw.Write(vbCrLf)
            End If
            sw.Flush()
            sw.Close()
            Return True
        Catch ex As Exception
            MessageBox.Show("Error writing to " & strPathName & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return False
    End Function

#End Region
End Class

#Region "FindFilesRegEx"

Public Class FindFilesRegEx

    Public Shared Function Match(ByVal pattern As String, ByVal name As String) As Boolean
        Dim regex As Regex = FindFilesPatternToRegex(pattern)
        Return regex.IsMatch(name)
    End Function

    Private Shared HasQuestionMarkRegEx As New Regex("\?", RegexOptions.Compiled)
    Private Shared IlegalCharactersRegex As New Regex("[" & "\/:<>|" & """]", RegexOptions.Compiled)
    Private Shared CatchExtentionRegex As New Regex("^\s*.+\.([^\.]+)\s*$", RegexOptions.Compiled)
    Private Shared NonDotCharacters As String = "[^.]*"
    Public Shared Function FindFilesPatternToRegex(ByVal pattern As String) As Regex
        If pattern Is Nothing Then
            Throw New ArgumentNullException()
        End If
        pattern = pattern.Trim()
        If pattern.Length = 0 Then
            Throw New ArgumentException("Pattern is empty.")
        End If
        If IlegalCharactersRegex.IsMatch(pattern) Then
            Throw New ArgumentException("Patterns contains ilegal characters.")
        End If
        Dim hasExtension As Boolean = CatchExtentionRegex.IsMatch(pattern)
        Dim matchExact As Boolean = False
        If HasQuestionMarkRegEx.IsMatch(pattern) Then
            matchExact = True
        ElseIf hasExtension Then
            matchExact = CatchExtentionRegex.Match(pattern).Groups(1).Length <> 3
        End If
        Dim regexString As String = Regex.Escape(pattern)
        regexString = "^" & Regex.Replace(regexString, "\\\*", ".*")
        regexString = Regex.Replace(regexString, "\\\?", ".")
        If Not matchExact AndAlso hasExtension Then
            regexString += NonDotCharacters
        End If
        regexString += "$"
        Dim regex__1 As New Regex(regexString, RegexOptions.Compiled Or RegexOptions.IgnoreCase)
        Return regex__1
    End Function

End Class
#End Region

