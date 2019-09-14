Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Xml

Namespace VGDDCommon

    Public Class GrcProject
        Implements System.IDisposable

#Region "Public Enums"
        Public Enum ResourceTypes
            Bitmap
            Font
            Palette
        End Enum

        Public Enum ResourceSubTypes
            Installed
            TtfFile
            RasterFile
        End Enum

        Public Enum OutputFormats
            CArray
            Hex
            Binary
        End Enum

        Public Enum BuildTypes
            C30
            C32
        End Enum

        Public Enum CompressionTypes
            None
            IPU
            RLE
        End Enum

        Public Enum GraphicsModels
            BPP16
            BPP24
        End Enum
#End Region

#If Not PlayerMonolitico Then

        'Public GrcOutput As String = ""
        'Private Cl As String = ""

        'Private OutFile As String = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "VGDD_GRC_Wrapper.bin")
        'Private FilterFile As String = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "VGDD_GRC_Filter.txt")

        Private _Resources As New Collection
        Private _Compiler As String
        Private _GraphicsModel As GraphicsModels

        Private GrcXmlProject As XmlDocument
        Private GrcXmlProjectFileName As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal Compiler As String, ByVal OutputFormat As GrcProject.OutputFormats, ByVal GraphicsModel As GraphicsModels)
            'Dim BuildType As BuildTypes
            'Select Case Compiler
            '    Case "C30", "XC16"
            '        BuildType = GrcProject.BuildTypes.C30
            '    Case "C32", "XC32"
            '        BuildType = GrcProject.BuildTypes.C32
            'End Select

            GrcXmlProject = New XmlDocument
            Dim DocEl As XmlElement = GrcXmlProject.CreateNode(XmlNodeType.Element, "Graphics_Resource_Converter", "")
            Dim ConfigNode As XmlNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "Configuration", "")
            DocEl.AppendChild(ConfigNode)
            ConfigNode.AppendChild(GrcXmlProject.CreateNode(XmlNodeType.Element, "major", ""))
            ConfigNode.AppendChild(GrcXmlProject.CreateNode(XmlNodeType.Element, "minor", ""))
            ConfigNode.AppendChild(GrcXmlProject.CreateNode(XmlNodeType.Element, "compiler", ""))
            ConfigNode.AppendChild(GrcXmlProject.CreateNode(XmlNodeType.Element, "format", ""))
            ConfigNode.AppendChild(GrcXmlProject.CreateNode(XmlNodeType.Element, "graphics_module", ""))
            ConfigNode.AppendChild(GrcXmlProject.CreateNode(XmlNodeType.Element, "bitmap_padding_type", ""))
            ConfigNode.AppendChild(GrcXmlProject.CreateNode(XmlNodeType.Element, "graphics_model", ""))
            _Compiler = Compiler
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    ConfigNode.SelectSingleNode("major").InnerText = "3"
                    ConfigNode.SelectSingleNode("minor").InnerText = "29"
                Case "MLA", "HARMONY"
                    ConfigNode.SelectSingleNode("major").InnerText = "4"
                    ConfigNode.SelectSingleNode("minor").InnerText = "00"
            End Select
            Select Case Compiler
                Case "C30", "XC16"
                    ConfigNode.SelectSingleNode("compiler").InnerText = "C30/XC16"
                Case "C32", "XC32"
                    ConfigNode.SelectSingleNode("compiler").InnerText = "C32/XC32"
            End Select
            Select Case OutputFormat
                Case OutputFormats.CArray
                    ConfigNode.SelectSingleNode("format").InnerText = "CArray"
                Case OutputFormats.Hex
                    ConfigNode.SelectSingleNode("format").InnerText = "HEX"
                Case OutputFormats.Binary
                    ConfigNode.SelectSingleNode("format").InnerText = "BIN"
            End Select
            ConfigNode.SelectSingleNode("graphics_module").InnerText = IIf(Common.ProjectUsePalette, "true", "false")
            ConfigNode.SelectSingleNode("bitmap_padding_type").InnerText = "padding"
            Select Case GraphicsModel
                Case GraphicsModels.BPP16
                    ConfigNode.SelectSingleNode("graphics_model").InnerText = "16bpp"
                Case GraphicsModels.BPP24
                    ConfigNode.SelectSingleNode("graphics_model").InnerText = "24bpp"
            End Select

            GrcXmlProject.AppendChild(DocEl)
            _Resources.Clear()
        End Sub

        Private _LastOutputFile As String
        Public ReadOnly Property LastOutputFile As String
            Get
                Return _LastOutputFile
            End Get
        End Property

        Public Property OutputFormat As GrcProject.OutputFormats
            Set(ByVal value As GrcProject.OutputFormats)
                Dim oNode As XmlNode = GrcXmlProject.DocumentElement.SelectSingleNode("Configuration/format")
                Select Case value
                    Case OutputFormats.CArray
                        oNode.InnerText = "CArray"
                    Case OutputFormats.Hex
                        oNode.InnerText = "HEX"
                    Case OutputFormats.Binary
                        oNode.InnerText = "BIN"
                End Select

            End Set
            Get
                Select Case GrcXmlProject.DocumentElement.SelectSingleNode("Configuration/format").InnerText
                    Case "CArray"
                        Return OutputFormats.CArray
                    Case "HEX"
                        Return OutputFormats.Hex
                    Case "BIN"
                        Return OutputFormats.Binary
                End Select
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property ResourcesCount As Integer
            Get
                Return _Resources.Count
            End Get
        End Property

        Public ReadOnly Property BitmapsCount As Integer
            Get
                BitmapsCount = 0
                For Each oGrcResource As GrcResource In _Resources
                    If oGrcResource.ResourceType = ResourceTypes.Bitmap Then
                        BitmapsCount += 1
                    End If
                Next
                Return BitmapsCount
            End Get
        End Property

        Public ReadOnly Property FontInstalledCount As Integer
            Get
                FontInstalledCount = 0
                For Each oGrcResource As GrcResource In _Resources
                    If oGrcResource.ResourceType = ResourceTypes.Font AndAlso oGrcResource.ResourceSubType = ResourceSubTypes.Installed Then
                        FontInstalledCount += 1
                    End If
                Next
                Return FontInstalledCount
            End Get
        End Property

        Public ReadOnly Property FontFileCount As Integer
            Get
                FontFileCount = 0
                For Each oGrcResource As GrcResource In _Resources
                    If oGrcResource.ResourceType = ResourceTypes.Font AndAlso oGrcResource.ResourceSubType = ResourceSubTypes.Installed Then
                        FontFileCount += 1
                    End If
                Next
                Return FontFileCount
            End Get
        End Property

        Public ReadOnly Property PaletteCount As Integer
            Get
                PaletteCount = 0
                For Each oGrcResource As GrcResource In _Resources
                    If oGrcResource.ResourceType = ResourceTypes.Palette Then
                        PaletteCount += 1
                    End If
                Next
                Return PaletteCount
            End Get
        End Property

        Public Function AscentDescent(ByVal oFont As Font) As Integer
            Dim ascent As Integer = oFont.FontFamily.GetCellAscent(oFont.Style)
            Dim ascentPixel As Integer = Math.Round(oFont.Size * ascent / oFont.FontFamily.GetEmHeight(oFont.Style))

            Dim descent As Integer = oFont.FontFamily.GetCellDescent(oFont.Style)
            Dim descentPixel As Integer = Math.Round(oFont.Size * descent / oFont.FontFamily.GetEmHeight(oFont.Style))
            Return ascentPixel + descentPixel
        End Function

        Public Function WriteProject(ByVal strFileName As String) As Boolean

            If Me.BitmapsCount > 0 Then
                Dim oBitmapsNode As XmlNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "Bitmaps", "")
                For Each oGrcResource As GrcResource In _Resources
                    If oGrcResource.ResourceType = ResourceTypes.Bitmap Then
                        Dim oBitmapNode As XmlNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "Bitmap_Resource", "")
                        Dim oNode As XmlNode

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "label", "")
                        oNode.InnerText = oGrcResource.ResultingLabel
                        oBitmapNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "path", "")
                        oNode.InnerText = oGrcResource.InputFile
                        oBitmapNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "compression", "")
                        oNode.InnerText = oGrcResource.CompressionType.ToString
                        oBitmapNode.AppendChild(oNode)

                        oBitmapsNode.AppendChild(oBitmapNode)
                    End If
                Next
                GrcXmlProject.DocumentElement.AppendChild(oBitmapsNode)
            End If

            If Me.FontFileCount > 0 Then
                Dim oFontsNode As XmlNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "Fonts", "")
                For Each oGrcResource As GrcResource In _Resources
                    If oGrcResource.ResourceType = ResourceTypes.Font Then
                        Dim oFontNode As XmlNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "Font_Resource", "")
                        Dim oNode As XmlNode

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "label", "")
                        oNode.InnerText = oGrcResource.ResultingLabel
                        oFontNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "name", "")
                        oNode.InnerText = oGrcResource.VFont.Font.Name
                        oFontNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "size", "")
                        'Dim GrcFontSize As Integer = AscentDescent(oGrcResource.VFont.Font)
                        oNode.InnerText = Math.Round(oGrcResource.VFont.Font.Size * 1.34) '
                        oFontNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "height", "")
                        'Dim oGrcFont As Font = New Font(oGrcResource.VFont.Font.FontFamily, GrcFontSize, oGrcResource.VFont.Font.Style)
                        oNode.InnerText = Math.Round(AscentDescent(oGrcResource.VFont.Font) * 1.34) '
                        oFontNode.AppendChild(oNode)

                        If oGrcResource.VFont.Font.Bold Then
                            oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "bold", "")
                            oNode.InnerText = "true"
                            oFontNode.AppendChild(oNode)
                        End If

                        If oGrcResource.VFont.Font.Italic Then
                            oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "italic", "")
                            oNode.InnerText = "true"
                            oFontNode.AppendChild(oNode)
                        End If

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "anti_aliasing", "")
                        If oGrcResource.VFont.AntiAliasing Then
                            oNode.InnerText = "2 bit per pixel"
                        Else
                            oNode.InnerText = "1 bit per pixel"
                        End If
                        oFontNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "first_character", "")
                        oNode.InnerText = oGrcResource.VFont.StartChar
                        oFontNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "last_character", "")
                        oNode.InnerText = oGrcResource.VFont.EndChar
                        oFontNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "font_filter", "")
                        Select Case oGrcResource.VFont.Charset
                            Case VGDDFont.FontCharset.RANGE
                                oNode.InnerText = "None"
                            Case VGDDFont.FontCharset.SELECTION
                                Dim strInclude As String = ""
                                If oGrcResource.VFont.CharsIncluded.Length = 0 Then
                                    If oGrcResource.VFont.SmartCharSet Then
                                        MessageBox.Show("Warning: Font " & oGrcResource.VFont.Name & " has been declared FontCharset.SELECTION and SmartCharSet has been just set. Please close the project and reopen it to update character list", "Empty font")
                                    Else
                                        MessageBox.Show("Warning: Font " & oGrcResource.VFont.Name & " has been declared FontCharset.SELECTION but its CharsIncluded character list is empty", "Empty font")
                                    End If
                                End If
                                Array.Sort(oGrcResource.VFont.CharsIncluded)
                                For Each c As Char In oGrcResource.VFont.CharsIncluded
                                    strInclude &= c
                                Next

                                Dim FilterFile As String = Path.Combine(Common.VGDDProjectPath, oGrcResource.VFont.Name & "_Filter.txt")
                                Dim oFilterFile As New StreamWriter(FilterFile, False, System.Text.UnicodeEncoding.Unicode)
                                oFilterFile.WriteLine("include:    " & strInclude)
                                oFilterFile.Flush()
                                oFilterFile.Close()

                                oNode.InnerText = FilterFile
                        End Select
                        oFontNode.AppendChild(oNode)

                        oFontsNode.AppendChild(oFontNode)
                    End If
                Next
                GrcXmlProject.DocumentElement.AppendChild(oFontsNode)
            End If

            If Me.PaletteCount > 0 Then
                Dim oPalettesNode As XmlNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "Palette", "")
                For Each oGrcResource As GrcResource In _Resources
                    If oGrcResource.ResourceType = ResourceTypes.Palette Then
                        Dim oPaletteNode As XmlNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "Palette_Resource", "")
                        Dim oNode As XmlNode

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "label", "")
                        oNode.InnerText = oGrcResource.ResultingLabel
                        oPaletteNode.AppendChild(oNode)

                        oNode = GrcXmlProject.CreateNode(XmlNodeType.Element, "path", "")
                        oNode.InnerText = oGrcResource.InputFile
                        oPaletteNode.AppendChild(oNode)

                        oPalettesNode.AppendChild(oPaletteNode)
                    End If
                Next
                GrcXmlProject.DocumentElement.AppendChild(oPalettesNode)
            End If

            Dim sw As New StringWriter
            Dim xtw As New XmlTextWriter(sw)
            xtw.Formatting = Formatting.Indented
            GrcXmlProject.WriteTo(xtw)
            Try
                Dim fw As StreamWriter = New StreamWriter(strFileName, False, New System.Text.UTF8Encoding)
                Dim strXml As String = sw.ToString
                fw.Write(strXml)
                fw.Close()
            Catch ex As Exception
                MessageBox.Show("Error writing to " & strFileName & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
            GrcXmlProjectFileName = strFileName
            Return True
        End Function

        Public Sub AddResourceToProject(ByVal oResource As GrcResource)
            _Resources.Add(oResource)
        End Sub

        Public Function RunProject(ByVal strOutFile As String) As Boolean
            _LastOutputFile = strOutFile
            Dim strResult As String = String.Empty
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    strResult = RunGrc(" -XML """ & GrcXmlProjectFileName & """ -O """ & strOutFile & """ -B " & _Compiler).Trim
                Case "MLA", "HARMONY"
                    strResult = RunGrc(" -XML """ & GrcXmlProjectFileName & """ -O """ & strOutFile & """").Trim
            End Select
            If strResult = "" Then
                If Mal.FrameworkName.ToUpper = "MLALEGACY" Then
                    ParseOutputFile(strOutFile)
                Else
                    ParseOutputFile(Common.ProjectFileName_InternalResourcesRefC)
                    ParseOutputFile(Common.ProjectFileName_InternalResourcesC)
                End If
                Return True
            Else
                MessageBox.Show("GRC RunProject ERROR: " & strResult, "GRC error")
                Return False
            End If
        End Function

        Public Shared Sub ParseOutputFile(ByVal OutFile As String)
            If Not File.Exists(OutFile) Then Exit Sub
            Dim sr As New StreamReader(OutFile)
            Dim oBitmap As VGDDImage = Nothing
            Dim oVFont As VGDDFont = Nothing
            Dim Line As String
            Dim blnImageFlash As Boolean = False, lngImageFlashSize As Long
            Dim blnFontFlash As Boolean = False, lngFontFlashSize As Long
            Dim strBitmapName As String
            Do While Not sr.EndOfStream
                Line = sr.ReadLine
                If Line.Contains("GFX_RESOURCE_HDR") Then
                    strBitmapName = Line.Split(" ")(2).Trim
                    If Common.ProjectUseBmpPrefix Then
                        strBitmapName = strBitmapName.Substring(3)
                    End If
                    oBitmap = VGDDCommon.Common.GetBitmap(strBitmapName)
                    If oBitmap Is Nothing Then Continue Do
                    For i As Integer = 1 To 6
                        sr.ReadLine()
                    Next
                    Line = sr.ReadLine.Replace(",", "").Trim
                    Integer.TryParse(Line.Split("=")(1), oBitmap._BitmapCompressedSize)
                    Line = sr.ReadLine.Replace(",", "").Trim
                    Integer.TryParse(Line.Split("=")(1), oBitmap._BitmapSize)
                    oBitmap = Nothing
                ElseIf Line.Contains("const GFX_IMAGE_HEADER") Or Line.Contains("const IMAGE_FLASH ") Or Line.Contains("GFX_RESOURCE_HDR") Then
                    strBitmapName = Line.Split(" ")(2).Trim
                    If Common.ProjectUseBmpPrefix Then
                        strBitmapName = strBitmapName.Substring(3)
                    End If
                    oBitmap = VGDDCommon.Common.GetBitmap(strBitmapName)
                    If Line.Contains("const IMAGE_FLASH ") Then 'Uncompressed
                        blnImageFlash = True
                        lngImageFlashSize = 0
                        For i As Integer = 1 To 5
                            sr.ReadLine()
                        Next
                    Else 'RLE, IPU
                        For i As Integer = 1 To 6
                            sr.ReadLine()
                        Next
                        Line = sr.ReadLine.Replace(",", "").Trim
                        Integer.TryParse(Line, oBitmap._BitmapCompressedSize)
                        Line = sr.ReadLine.Replace(",", "").Trim
                        Integer.TryParse(Line, oBitmap._BitmapSize)
                        oBitmap = Nothing
                    End If
                ElseIf Line.Contains("GFX_COMPRESSED_BUFFER_SIZE") Or Line.Contains("GFX_DECOMPRESSED_BUFFER_SIZE") Then
                    Dim intVal As Integer
                    Dim strval As String = Line.Substring(Line.IndexOf("<") + 1).Replace(")", "").Trim
                    If Integer.TryParse(strval, intVal) Then
                        If Line.Contains("GFX_COMPRESSED_BUFFER_SIZE") Then
                            Common.IPU_MAX_COMPRESSED_BUFFER_SIZE = intVal
                        ElseIf Line.Contains("GFX_DECOMPRESSED_BUFFER_SIZE") Then
                            Common.IPU_MAX_DECOMPRESSED_BUFFER_SIZE = intVal
                        End If
                    End If

                ElseIf Line.Contains("const FONT_FLASH ") Then 'LegacyMLA
                    Dim strFontName As String = Line.Split(" ")(2).Trim
                    oVFont = Common.GetFont(strFontName, Nothing)
                    blnFontFlash = True
                    For i As Integer = 1 To 10
                        sr.ReadLine()
                    Next
                ElseIf Line.StartsWith("GRC_FONT_SECTION") Then 'MLA/Harmony
                    blnFontFlash = True
                ElseIf Line.StartsWith("GRC_CONST_SECTION") Then 'MLA/Harmony
                    blnFontFlash = False
                    If oVFont IsNot Nothing Then
                        oVFont._BinSize = lngFontFlashSize
                    End If
                    oVFont = Nothing
                    lngFontFlashSize = 0
                ElseIf blnFontFlash AndAlso Line.StartsWith(".align 2") Then
                    Line = sr.ReadLine
                    Dim strFontName As String = Line.Split(" ")(0).Trim
                    Do While strFontName.StartsWith("_")
                        strFontName = strFontName.Substring(1)
                    Loop
                    If strFontName.EndsWith(":") Then strFontName = strFontName.Substring(0, strFontName.Length - 1)
                    oVFont = Common.GetFont(strFontName, Nothing)
                    For i As Integer = 1 To 6
                        sr.ReadLine()
                    Next
                ElseIf blnImageFlash Or blnFontFlash Then
                    If Line.Contains("0x") Then
                        Dim BytesInLine As Integer = Line.Split(",").Length
                        If blnImageFlash Then
                            lngImageFlashSize += BytesInLine
                        Else
                            lngFontFlashSize += BytesInLine
                        End If
                    ElseIf (Line.StartsWith("};") Or Line.StartsWith("asm("".section .const")) Then
                        If oBitmap IsNot Nothing Then
                            oBitmap._BitmapSize = lngImageFlashSize
                            oBitmap._BitmapCompressedSize = lngImageFlashSize
                            oBitmap = Nothing
                            blnImageFlash = False
                        ElseIf oVFont IsNot Nothing Then
                            oVFont._BinSize = lngFontFlashSize
                            oVFont = Nothing
                            blnFontFlash = False
                        End If
                    End If
                ElseIf Line.Contains("(FLASH_BYTE *)") Then
                    sr.ReadLine()
                End If
            Loop
            sr.Close()
        End Sub

        Public Function RunGrc(ByVal Cl As String) As String
            Dim blnTest As Boolean = False
            Dim strOut As String = ""
            Try
                If Common.ProjectJavaCommand Is Nothing Then
                    Throw New Exception("Please define Java command to run GRC in Project Settings")
                End If
                If Not File.Exists(Common.ProjectPathGRC) Then
                    Common.ProjectPathGRC = Common.ProjectFallbackGRCPath
                End If
                If Not File.Exists(Common.ProjectPathGRC) Then
                    Throw New Exception("Please define fallback path for GRC - Graphics Resource Converter")
                End If
                If Cl = "TEST" Then
                    Cl = ""
                    blnTest = True
                ElseIf Not Cl.StartsWith(" ") Then
                    Cl = " " & Cl
                End If
                Dim myProcess As Process = New Process()
                Try
                    If Common.ProjectJavaCommand.StartsWith("""") Then
                        myProcess.StartInfo.FileName = Common.ProjectJavaCommand.Substring(1, Common.ProjectJavaCommand.Substring(1).IndexOf(""""))
                        myProcess.StartInfo.Arguments = Common.ProjectJavaCommand.Substring(Common.ProjectJavaCommand.Substring(1).IndexOf("""") + 2).Trim & " """ & Common.ProjectPathGRC & """" & Cl
                    ElseIf Common.ProjectJavaCommand.EndsWith("-jar") Then
                        myProcess.StartInfo.FileName = Common.ProjectJavaCommand.Substring(0, Common.ProjectJavaCommand.IndexOf("-jar") - 1)
                        myProcess.StartInfo.Arguments = Common.ProjectJavaCommand.Substring(Common.ProjectJavaCommand.IndexOf("-jar")) & " """ & Common.ProjectPathGRC & """" & Cl
                    Else
                        myProcess.StartInfo.FileName = Common.ProjectJavaCommand.Substring(0, Common.ProjectJavaCommand.IndexOf(" "))
                        myProcess.StartInfo.Arguments = Common.ProjectJavaCommand.Substring(Common.ProjectJavaCommand.IndexOf(" ") + 1) & " """ & Common.ProjectPathGRC & """" & Cl
                    End If
                    myProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(Common.ProjectPathGRC)
                    myProcess.StartInfo.UseShellExecute = False
                    myProcess.StartInfo.CreateNoWindow = True
                    myProcess.StartInfo.RedirectStandardInput = True
                    myProcess.StartInfo.RedirectStandardOutput = True
                    myProcess.StartInfo.RedirectStandardError = True
                    myProcess.Start()

                Catch ex As Exception
                    If blnTest Then
                        Return "KO - " & ex.Message
                    End If
                    MessageBox.Show("Error launching GRC." & vbCrLf & "Commandline: " & myProcess.StartInfo.FileName & " " & myProcess.StartInfo.Arguments & vbCrLf & vbCrLf & ex.Message, "GRC Wrapper error")
                    Throw New Exception("Please check your Java installation and GRC - Graphics Resource Converter path")
                End Try
                If blnTest Then
                    myProcess.Kill()
                    Return "OK"
                End If
                Do While Not myProcess.HasExited
                    System.Threading.Thread.Sleep(250)
                    Application.DoEvents()
                Loop
                Dim oSrStdOut As StreamReader = myProcess.StandardOutput
                strOut = oSrStdOut.ReadToEnd() & vbCrLf
                oSrStdOut.Close()

                Dim oSrStdErr As StreamReader = myProcess.StandardError
                strOut &= oSrStdErr.ReadToEnd()
                oSrStdErr.Close()

                If Not myProcess.HasExited Then
                    myProcess.Kill()
                End If
                myProcess.Close()

            Catch ex As Exception

            End Try
            Dim sw As New StringWriter
            For Each l As String In strOut.Split(vbCrLf)
                If Not l.Contains("Warning:") Then
                    sw.WriteLine(l)
                End If
            Next
            Return sw.ToString
        End Function

#End If
#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        'Public Function StreamToMemory(ByVal path As String) As MemoryStream
        '    Dim input As FileStream
        '    Dim output As MemoryStream

        '    If Not File.Exists(path) Then Return Nothing
        '    input = New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
        '    Dim buffer(1023) As Byte
        '    Dim count As Integer = 1024
        '    If input.CanSeek Then
        '        output = New MemoryStream(input.Length)
        '    Else
        '        output = New MemoryStream
        '    End If
        '    Do
        '        count = input.Read(buffer, 0, count)
        '        If count = 0 Then Exit Do
        '        output.Write(buffer, 0, count)
        '    Loop
        '    output.Position = 0
        '    input.Close()
        '    Return output
        'End Function
    End Class

    Public Class GrcResource
        Private _ResourceType As GrcProject.ResourceTypes
        Private _ResourceSubType As GrcProject.ResourceSubTypes
        Private _OutputFormat As GrcProject.OutputFormats
        Private _InputFile As String
        Private _ResultingLabel As String
        Private _VFont As VGDDFont
        Private _CompressionType As GrcProject.CompressionTypes = GrcProject.CompressionTypes.None

#Region "Public Properties"

        Public Property CompressionType As GrcProject.CompressionTypes
            Get
                Return _CompressionType
            End Get
            Set(ByVal value As GrcProject.CompressionTypes)
                _CompressionType = value
            End Set
        End Property

        Public Property ResourceType As GrcProject.ResourceTypes
            Get
                Return _ResourceType
            End Get
            Set(ByVal value As GrcProject.ResourceTypes)
                _ResourceType = value
            End Set
        End Property

        Public Property ResourceSubType As GrcProject.ResourceSubTypes
            Get
                Return _ResourceSubType
            End Get
            Set(ByVal value As GrcProject.ResourceSubTypes)
                _ResourceSubType = value
            End Set
        End Property

        Public Property OutputFormat As GrcProject.OutputFormats
            Get
                Return _OutputFormat
            End Get
            Set(ByVal value As GrcProject.OutputFormats)
                _OutputFormat = value
            End Set
        End Property

        Public Property InputFile As String
            Get
                Return _InputFile
            End Get
            Set(ByVal value As String)
                _InputFile = value
            End Set
        End Property

        Public Property ResultingLabel As String
            Get
                Return _ResultingLabel
            End Get
            Set(ByVal value As String)
                _ResultingLabel = value
            End Set
        End Property

        Public Property VFont As VGDDFont
            Get
                Return _VFont
            End Get
            Set(ByVal value As VGDDFont)
                _VFont = value
            End Set
        End Property
#End Region

        Public Function ConvertSingleResource(ByVal OutputFile As String, ByVal BuildType As GrcProject.BuildTypes) As Integer
            Dim Cl As String = ""
            Select Case Me.ResourceType
                Case GrcProject.ResourceTypes.Font
                    Cl = " -T F"
                    Select Case Me.ResourceSubType
                        Case GrcProject.ResourceSubTypes.Installed
                            Cl &= " -U I"
                            Cl &= " -N """ & _VFont.Font.Name & """"
                        Case GrcProject.ResourceSubTypes.RasterFile
                            Cl &= " -U R"
                        Case GrcProject.ResourceSubTypes.TtfFile
                            Cl &= " -U T"
                    End Select
                    If _VFont.Font.Bold Then
                        Cl &= " -D"
                    End If
                    If _VFont.Font.Italic Then
                        Cl &= " -A"
                    End If
                    Cl &= " -Z " & (Math.Round(_VFont.Font.Size, 0) - 1) '* 1.4
                    Select Case Me.OutputFormat
                        Case GrcProject.OutputFormats.CArray
                        Case GrcProject.OutputFormats.Hex
                        Case GrcProject.OutputFormats.Binary
                            Cl &= " -F B"
                    End Select
                    Select Case _VFont.Charset
                        Case VGDDFont.FontCharset.RANGE
                            Cl &= " -S """ & Char.ConvertFromUtf32(IIf(_VFont.StartChar > 0, _VFont.StartChar, 32)) & """ -E """ & Char.ConvertFromUtf32(IIf(_VFont.EndChar > 0, _VFont.EndChar, 127)) & """"
                        Case VGDDFont.FontCharset.SELECTION
                            Dim strInclude As String = ""
                            If _VFont.CharsIncluded.Length = 0 Then
                                If _VFont.SmartCharSet Then
                                    MessageBox.Show("Warning: Font " & _VFont.Name & " has been declared FontCharset.SELECTION and SmartCharSet has been just set. Please close the project and reopen it to update character list", "Empty font")
                                Else
                                    MessageBox.Show("Warning: Font " & _VFont.Name & " has been declared FontCharset.SELECTION but its CharsIncluded character list is empty", "Empty font")
                                End If
                            End If
                            Array.Sort(_VFont.CharsIncluded)
                            For Each c As Char In _VFont.CharsIncluded
                                strInclude &= c
                            Next

                            Dim FilterFile As String = Path.Combine(Common.VGDDProjectPath, _VFont.Name & "_Filter.txt")
                            Dim sw As New StreamWriter(FilterFile, False, System.Text.UnicodeEncoding.Unicode)
                            sw.WriteLine("include:    " & strInclude)
                            sw.Flush()
                            sw.Close()
                            Cl &= " -R """ & FilterFile & """"
                    End Select
                    Cl &= " -B " & BuildType.ToString
                    Cl &= " -O """ & OutputFile & """"

                Case GrcProject.ResourceTypes.Bitmap
                    Cl = " -I """ & Me.InputFile & """"
                    Cl &= " -T I"
                    Select Case Me.CompressionType
                        Case GrcProject.CompressionTypes.None
                        Case GrcProject.CompressionTypes.IPU
                            Cl &= " -P IPU"
                        Case GrcProject.CompressionTypes.RLE
                            Cl &= " -P RLE"
                    End Select
                    Select Case Me.OutputFormat
                        Case GrcProject.OutputFormats.CArray
                            Cl &= " -F C"
                        Case GrcProject.OutputFormats.Hex
                            Cl &= " -F H"
                        Case GrcProject.OutputFormats.Binary
                            Cl &= " -F B"
                    End Select
                    Cl &= " -B " & BuildType.ToString
                    If Me.ResultingLabel <> "" Then
                        Cl &= " -X " & Me.ResultingLabel
                    End If
                    Cl &= " -O """ & OutputFile & """ "
            End Select

            If File.Exists(OutputFile) Then
                File.Delete(OutputFile)
            End If
            Dim GrcRawOutput As String
            Using oGrc As New GrcProject
                GrcRawOutput = oGrc.RunGrc(Cl.Trim())
            End Using

            Dim GrcOutput As String = ""
            For Each li As String In GrcRawOutput.Split(vbLf)
                If Not li.StartsWith("Line:") And Not li.StartsWith("characters:") Then
                    GrcOutput &= li.Replace(vbCr, "") & vbCrLf
                End If
            Next
            If GrcOutput.Trim <> "" Then
                GrcOutput &= vbCrLf & "CommandLine: " & Cl & vbCrLf
            End If
            If File.Exists(OutputFile) Then
                Return New FileInfo(OutputFile).Length
            Else
                Return -1
            End If
        End Function

    End Class
End Namespace
