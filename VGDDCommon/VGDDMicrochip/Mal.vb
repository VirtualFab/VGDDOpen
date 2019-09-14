Imports System.IO
Imports System.Xml

Namespace VGDDMicrochip

    Public Class Mal

        Public Shared MalPath As String
        Public Shared MalVersion As String

        Public Shared Function CheckMalVersion(ByVal strPath) As String
            Dim strManifestFile As String = Path.Combine(strPath, "mal.xml")
            If Not File.Exists(strManifestFile) Then
                Return "Error: Could not find mal.xml"
            End If
            Dim oManifest As XmlDocument = New XmlDocument
            oManifest.Load(strManifestFile)
            Dim oNode As XmlNode = oManifest.DocumentElement.SelectSingleNode("INFO/VERSION/STRING")
            If oNode Is Nothing Then
                Return "Error: Could not check MAL version from mal.xml"
            End If
            If oNode.InnerText.Split("-")(0) < 2011 Or (oNode.InnerText.Split("-")(0) = 2011 And oNode.InnerText.Split("-")(1) < 10) Then
                Return "Error: MAL Version " & oNode.InnerText & " not compatible. Please update it from Microchip's website and recheck"
            End If
            MalVersion = oNode.InnerText
            Return "OK"
        End Function
    End Class

End Namespace

