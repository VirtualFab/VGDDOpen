Imports System.Xml
Imports System.ComponentModel
Imports System.Drawing
Imports VirtualFabUtils.Utils

Namespace VGDDCommon

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)>
    Public Class VGDDPalette

        Private _Name As String = "Default"
        Private _PaletteColours() As Color
        Private _PaletteFile As String

        Public Sub New()
        End Sub

        <Description("The Palette Colour array")> _
        Public Property PaletteColours() As Color()
            Get
                Return _PaletteColours
            End Get
            Set(ByVal value As Color())
                _PaletteColours = value
            End Set
        End Property

        <Description("Source/Destination File for the Palette ")> _
        Public Property PaletteFile() As String
            Get
                Return _PaletteFile
            End Get
            Set(ByVal value As String)
                _PaletteFile = value
            End Set
        End Property

        <Description("Name for this Palette")> _
        <ParenthesizePropertyName(False)> _
        <Category("Scheme")> _
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = Common.CleanName(value)
            End Set
        End Property

        <System.ComponentModel.Browsable(False)> _
        Public Sub FromXml(ByRef Xml As String)
            Dim oXmlDoc As New XmlDocument
            oXmlDoc.LoadXml(Xml)
            Me.FromXml(oXmlDoc.DocumentElement)
        End Sub

        <System.ComponentModel.Browsable(False)> _
        Public Sub FromXml(ByRef XmlNode As XmlNode)
            Me.Name = XmlNode.Attributes("Name").Value.ToString
            If XmlNode.Attributes("PaletteFile") IsNot Nothing Then
                Me.PaletteFile = XmlNode.Attributes("PaletteFile").Value
            End If
            Dim LoadPaletteColours(1) As Color
            For Each node As XmlNode In XmlNode.ChildNodes
                If node.Name.Equals("Colour") Then
                    Dim aRGB() As String = node.InnerText.Split(",")
                    Dim c As Color = Color.FromArgb(aRGB(0), aRGB(1), aRGB(2))
                    Dim index As Integer = node.Attributes("Index").Value
                    If LoadPaletteColours.Length <= index Then
                        ReDim Preserve LoadPaletteColours(index)
                    End If
                    LoadPaletteColours(index) = c
                End If
            Next
            _PaletteColours = LoadPaletteColours
        End Sub

#If Not PlayerMonolitico Then
        <System.ComponentModel.Browsable(False)> _
        Public Function ToXml() As String
            Dim oXmlDoc As New XmlDocument
            Dim oElement As XmlElement = oXmlDoc.AppendChild(oXmlDoc.CreateElement("Element"))
            Me.ToXml(oElement)
            Dim sb As New System.Text.StringBuilder
            Dim XmlWriterSettings As New XmlWriterSettings()
            XmlWriterSettings.Indent = True
            XmlWriterSettings.IndentChars = "  "
            XmlWriterSettings.NewLineChars = vbCr & vbLf
            XmlWriterSettings.OmitXmlDeclaration = True
            XmlWriterSettings.NewLineHandling = NewLineHandling.Replace
            Using writer As XmlWriter = XmlWriter.Create(sb, XmlWriterSettings)
                oXmlDoc.SelectSingleNode("Element").FirstChild.WriteTo(writer)
            End Using
            Dim strXml As String = sb.ToString
            Return strXml
        End Function

        <System.ComponentModel.Browsable(False)> _
        Public Sub ToXml(ByRef oParentElement As XmlElement)
            Dim XmlDoc As XmlDocument = oParentElement.OwnerDocument
            Dim oPaletteElement As XmlElement = XmlDoc.CreateElement("Palette")
            Dim oAttr As XmlAttribute = oParentElement.OwnerDocument.CreateAttribute("Name")
            oAttr.Value = _Name
            oPaletteElement.Attributes.Append(oAttr)

            oAttr = XmlDoc.CreateAttribute("PaletteFile")
            oAttr.Value = RelativePath.Evaluate(Common.MplabXProjectPath, Me.PaletteFile).Replace("\", "/")

            oPaletteElement.Attributes.Append(oAttr)

            'Dim oPaletteColours As XmlNode = XmlDoc.CreateElement("PaletteColours")
            For i As Integer = 0 To _PaletteColours.Length - 1
                Dim oNode As XmlNode = Common.ColorToXml(XmlDoc, "Colour", _PaletteColours(i))
                'XmlDoc.CreateElement("PaletteColour")

                oAttr = XmlDoc.CreateAttribute("Index")
                oAttr.Value = i
                oNode.Attributes.Append(oAttr)

                'oPaletteColours.AppendChild(oNode)
                oPaletteElement.AppendChild(oNode)
            Next
            oParentElement.AppendChild(oPaletteElement)
        End Sub

        Public Sub SaveAsGpl(ByVal FileName As String)
            Dim strPalette As String = String.Format("GIMP Palette" & vbCrLf & _
                    "Name: [{0}]" & vbCrLf & _
                    "Columns: 16" & vbCrLf & _
                    "#" & vbCrLf, Me.Name)
            For i As Integer = 0 To _PaletteColours.Length - 1
                Dim c As Color = _PaletteColours(i)
                strPalette &= String.Format(" {0,3:##0} {1,3:##0} {2,3:##0}{3}Index {4:##0}", c.R, c.G, c.B, vbTab, i) & vbCrLf
            Next
            WriteFile(FileName, strPalette)
        End Sub

        Public Sub LoadFromGpl()
            Dim i As Integer = 0
            Dim alColors As New ArrayList
            For Each strRow As String In ReadFile(_PaletteFile).Split(vbLf)
                strRow = strRow.Trim
                If strRow.StartsWith("GIMP Palette") Then
                ElseIf strRow.StartsWith("Name:") Then
                ElseIf strRow.StartsWith("Columns:") Then
                ElseIf strRow.Contains("Index ") Then
                    strRow = strRow.Trim.Replace(vbTab, " ")
                    While strRow.Contains("  ")
                        strRow = strRow.Replace("  ", " ")
                    End While
                    Dim aColorsStr As String() = strRow.Split(" ")
                    Dim c As Color = Color.FromArgb(aColorsStr(0), aColorsStr(1), aColorsStr(2))
                    alColors.Add(c)
                    i += 1
                End If
            Next
            Dim aColors(i - 1) As Color
            For j As Integer = 0 To i - 1
                aColors(j) = alColors(j)
            Next
            _PaletteColours = aColors

        End Sub
#End If

        Public Function CanBeOptimisedForRGB565() As Boolean
            CanBeOptimisedForRGB565 = False
            For i As Integer = 0 To _PaletteColours.Length - 1
                Dim c565 As UInt16 = CodeGen.Color2RGB565(_PaletteColours(i))
                Dim newColor As Color = CodeGen.ColorFromRGB565(c565)
                If newColor <> _PaletteColours(i) Then
                    CanBeOptimisedForRGB565 = True
                    Exit Function
                End If
            Next
        End Function

        Public Sub OptimiseForRGB565()
            For i As Integer = 0 To _PaletteColours.Length - 1
                _PaletteColours(i) = CodeGen.ColorFromRGB565(CodeGen.Color2RGB565(_PaletteColours(i)))
            Next
        End Sub

        Public Function GetNearestColor(ByVal inColor As Color) As Color
            Dim dbl_input_red As Double = Convert.ToDouble(inColor.R)
            Dim dbl_input_green As Double = Convert.ToDouble(inColor.G)
            Dim dbl_input_blue As Double = Convert.ToDouble(inColor.B)
            Dim minDistance As Double = 500.0
            Dim tempDistance As Double
            Dim nearest_color As Color = Color.Empty
            For Each c As Color In _PaletteColours
                ' compute the Euclidean distance between the two colors
                ' note, that the alpha-component is not used in this example
                tempDistance = Math.Sqrt(Math.Pow(Convert.ToDouble(c.R) - dbl_input_red, 2.0) + Math.Pow(Convert.ToDouble(c.G) - dbl_input_green, 2.0) + Math.Pow(Convert.ToDouble(c.B) - dbl_input_blue, 2.0))
                ' explore the result and store the nearest color
                If tempDistance < minDistance Then
                    minDistance = tempDistance
                    nearest_color = c
                End If
            Next
            Return nearest_color
        End Function
    End Class

End Namespace
