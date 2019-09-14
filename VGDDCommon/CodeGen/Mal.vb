Imports System.IO
Imports System.Xml
Imports VirtualFabUtils.Utils

Namespace VGDDCommon

    Public Class Mal

        Public Shared ConfiguredFrameworks As VGDDFrameworkCollection

        Private Shared _MalPath As String
        Private Shared _MalVersion As String
        Private Shared _MalVersionNum As Integer
        Private Shared _CodeGenTemplateFileName As String
        'Public Shared IsLegacyVersion As Boolean
        Private Shared _FrameworkName As String = "MLALegacy"

        Public Shared Property MalVersionNum As Integer
            Get
                Return _MalVersionNum
            End Get
            Set(ByVal value As Integer)
                _MalVersionNum = value
            End Set
        End Property

        Public Shared Property CodeGenTemplateFileName As String
            Get
                Return _CodeGenTemplateFileName
            End Get
            Set(ByVal value As String)
                If value <> _CodeGenTemplateFileName Then
                    _CodeGenTemplateFileName = value
                    CodeGen.LoadCodeGenTemplates()
                End If
            End Set
        End Property

        Public Shared Property FrameworkName As String
            Get
                Return _FrameworkName
            End Get
            Set(ByVal value As String)
                _FrameworkName = value
            End Set
        End Property

        Public Shared Property MalVersion As String
            Get
                Return _MalVersion
            End Get
            Set(ByVal value As String)
                _MalVersion = value
            End Set
        End Property

        Public Shared Property MalPath As String
            Get
                Return _MalPath
            End Get
            Set(ByVal value As String)
                _MalPath = value
            End Set
        End Property

        Public Shared Function CheckMalVersion(ByVal strPath) As String
            Dim strManifestFile As String = String.Empty
            Try
                If strPath Is Nothing Then Return "KO"
                strManifestFile = Path.Combine(strPath, "mal.xml")
                If Not File.Exists(strManifestFile) Then
                    strManifestFile = Path.Combine(strPath, "module_versions.xml")
                    If Not File.Exists(strManifestFile) Then
                        strManifestFile = Path.Combine(Path.Combine(strPath, "doc"), "module_versions.xml")
                        If Not File.Exists(strManifestFile) Then
                            strManifestFile = Path.Combine(strPath, "gfx.h")
                            If Not File.Exists(strManifestFile) Then
                                strManifestFile = Path.Combine(Path.Combine(Path.Combine(strPath, "framework"), "gfx"), "gfx.h")
                                If Not File.Exists(strManifestFile) Then
                                    Return "Error: Could not find ""mal.xml"" or ""module_versions.xml"" or ""gfx.h"""
                                End If
                            End If
                        End If
                    End If
                End If

                If strManifestFile.EndsWith("gfx.h") Then  'Harmony
                    Dim strGfxh As String = ReadFile(strManifestFile)
                    Const GFXVERSIONTAG As String = "#define GRAPHICS_LIBRARY_VERSION"
                    Dim intPos1 As Integer = strGfxh.IndexOf(GFXVERSIONTAG)
                    MalVersion = strGfxh.Substring(intPos1 + GFXVERSIONTAG.Length).TrimStart
                    MalVersion = MalVersion.Substring(0, MalVersion.IndexOf(vbLf)).Replace(vbCr, "").Trim
                    Integer.TryParse(MalVersion.Replace("0x", ""), MalVersionNum)
                    FrameworkName = "Harmony"
                    MalPath = strPath 'Path.GetFullPath(Path.Combine(Path.Combine(strPath, ".."), ".."))
                Else
                    Dim oManifest As XmlDocument = New XmlDocument
                    oManifest.Load(strManifestFile)
                    Dim oNode As XmlNode = oManifest.DocumentElement.SelectSingleNode("INFO/VERSION/STRING")
                    If oNode Is Nothing Then
                        Return "Error: Could not check MLA version from manifest file"
                    End If
                    If oNode.InnerText.Split("-")(0) < 2011 Or (oNode.InnerText.Split("-")(0) = 2011 And oNode.InnerText.Split("-")(1) < 10) Then
                        Return "Error: MLA Version " & oNode.InnerText & " not compatible. Please update it from Microchip's website and recheck"
                    End If
                    MalVersion = oNode.InnerText
                    oNode = oManifest.DocumentElement.SelectSingleNode("LIBRARIES/LIBRARY/SHORT_NAME[text()='GFX']")
                    If oNode IsNot Nothing Then
                        MalVersionNum = CInt(oNode.ParentNode.SelectSingleNode("VERSION/MAJOR").InnerText) * 100 + _
                                        CInt(oNode.ParentNode.SelectSingleNode("VERSION/MINOR").InnerText)
                    End If

                    If Mal.MalVersionNum >= 400 Then
                        'IsLegacyVersion = False
                        FrameworkName = "MLA"
                        MalPath = strPath ' Path.GetFullPath(Path.Combine(strPath, ".."))
                    Else
                        'IsLegacyVersion = True
                        FrameworkName = "MLALegacy"
                        MalPath = strPath
                    End If
                End If
                If VGDDCommon.Mal.ConfiguredFrameworks IsNot Nothing Then
                    For Each oFramework As VGDDFramework In VGDDCommon.Mal.ConfiguredFrameworks
                        If oFramework.FrameworkPath = MalPath Then
                            VGDDCommon.Common.oSelectedFramework = oFramework
                        End If
                    Next
                End If
                SetTemplateName()
                Return "OK"
            Catch ex As Exception
                Return "Error: Could not check MAL version: " & ex.Message
            End Try
        End Function

        Public Shared Sub SetTemplateName()
            Dim strPathGRC As String = String.Empty
            Select Case FrameworkName.ToUpper
                Case "HARMONY"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesHarmony.xml")
                    CodeGenTemplateFileName = "CodeGenTemplatesHarmony.xml"
                    If Common.oSelectedFramework IsNot Nothing Then
                        strPathGRC = Path.Combine(Common.oSelectedFramework.FrameworkPath, "utilities\grc\grc.jar")
                    End If
                Case "MLA"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesMLA.xml")
                    CodeGenTemplateFileName = "CodeGenTemplatesMLA.xml"
                    If Common.oSelectedFramework IsNot Nothing Then
                        strPathGRC = Path.Combine(Common.oSelectedFramework.FrameworkPath, "framework\gfx\utilities\grc\grc.jar")
                    End If
                Case "MLALEGACY"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesLegacyMLA.xml")
                    CodeGenTemplateFileName = "CodeGenTemplatesLegacyMLA.xml"
                    If Common.oSelectedFramework IsNot Nothing Then
                        strPathGRC = Path.Combine(Common.oSelectedFramework.FrameworkPath, "Graphics\bin\grc\grc.jar")
                    End If
            End Select
            If File.Exists(strPathGRC) Then VGDDCommon.Common.ProjectPathGRC = strPathGRC
        End Sub

        Public Shared Function ComputeGrcPath(ByVal strPath As String) As String
            Try
                If strPath.Contains("<") Then Return String.Empty '<No Frameworks configured>
                Select Case FrameworkName.ToUpper
                    Case "MLALEGACY"
                        Return Path.Combine(Path.Combine(Path.Combine(Path.Combine(strPath, "Graphics"), "bin"), "grc"), "grc.jar")
                    Case "MLA"
                        Return Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(strPath, "framework"), "gfx"), "utilities"), "grc"), "grc.jar")
                    Case "HARMONY"
                        ComputeGrcPath = Path.Combine(Path.Combine(Path.Combine(strPath, "utilities"), "grc"), "grc.jar")
                        If Not File.Exists(ComputeGrcPath) Then
                            ComputeGrcPath = Common.ProjectFallbackGRCPath
                        End If
                        Return ComputeGrcPath
                End Select
            Catch ex As Exception
            End Try
            Return String.Empty
        End Function

        Public Shared Function Path2FrameworkDescription() As String
            If ConfiguredFrameworks IsNot Nothing Then
                For Each oFramework As VGDDCommon.VGDDFramework In ConfiguredFrameworks
                    If oFramework.FrameworkPath = VGDDCommon.Mal.MalPath Then
                        Return oFramework.Description
                    End If
                Next
            End If
            Return "<No Frameworks configured>"
        End Function

        Public Shared Function SelectMalFromDescription(ByVal Description As String) As VGDDFramework
            SelectMalFromDescription = Nothing
            For Each f As VGDDFramework In ConfiguredFrameworks
                If f.Description = Description Then
                    SelectMalFromDescription = f
                    Exit For
                End If
            Next
            If SelectMalFromDescription IsNot Nothing Then
                Common.oSelectedFramework = SelectMalFromDescription
                CheckMalVersion(Common.oSelectedFramework.FrameworkPath)
            End If
        End Function

        Public Shared Function SelectMalFromFrameworkName(ByVal FrameworkName As String) As VGDDFramework
            If ConfiguredFrameworks Is Nothing Then Return Nothing
            SelectMalFromFrameworkName = Nothing
            For Each f As VGDDFramework In ConfiguredFrameworks
                If f.FrameworkName = FrameworkName Then
                    SelectMalFromFrameworkName = f
                    Exit For
                End If
            Next
            If SelectMalFromFrameworkName IsNot Nothing Then
                Common.oSelectedFramework = SelectMalFromFrameworkName
                CheckMalVersion(Common.oSelectedFramework.FrameworkPath)
            End If
        End Function
    End Class

End Namespace

