Imports System.Xml

Namespace VGDDCommon

    <Serializable()> _
Public Class VGDDFramework

        Private _FrameworkName As String
        Private _FrameworkPath As String
        Private _MLAVersion As String
        Private _Description As String

        Public Sub New()

        End Sub

        'Public Sub New(ByRef oNode As XmlNode)
        '    _FrameworkName = oNode.Attributes("FrameworkName").Value
        '    _FrameworkPath = oNode.Attributes("FrameworkPath").Value
        'End Sub

        Private Sub SetDescription()
            _Description = String.Format("{0} (MLA v{1} on {2})", Me.FrameworkName, Me.MLAVersion, Me.FrameworkPath)
        End Sub

        Public Property FrameworkName As String
            Get
                Return _FrameworkName
            End Get
            Set(ByVal value As String)
                _FrameworkName = value
                SetDescription()
            End Set
        End Property

        Public Property FrameworkPath As String
            Get
                Return _FrameworkPath
            End Get
            Set(ByVal value As String)
                _FrameworkPath = value
                SetDescription()
            End Set
        End Property

        Public Property MLAVersion As String
            Get
                Return _MLAVersion
            End Get
            Set(ByVal value As String)
                _MLAVersion = value
                SetDescription()
            End Set
        End Property

        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)

            End Set
        End Property
    End Class

    <Serializable()> _
    Public Class VGDDFrameworkCollection
        Inherits CollectionBase
        Implements IList, Serialization.IXmlSerializable

        Public Sub New()

        End Sub

        Default Public Property Item(ByVal index As Integer) As VGDDFramework
            Get
                Return MyBase.List(index)
            End Get
            Set(ByVal value As VGDDFramework)
                MyBase.List(index) = value
            End Set
        End Property

        Public Sub Add(ByRef oFramework As VGDDFramework)
            MyBase.List.Add(oFramework)
        End Sub

        Public Overloads Property List()
            Get
                Return MyBase.List
            End Get
            Set(ByVal value)
                'MyBase.List = value
            End Set
        End Property

        Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements System.Xml.Serialization.IXmlSerializable.ReadXml
            If reader.MoveToContent() = XmlNodeType.Element AndAlso reader.LocalName = "VGDDFrameworkCollection" Then
                reader.ReadStartElement()
                Do While reader.MoveToContent() = XmlNodeType.Element AndAlso reader.LocalName = "Framework"
                    Dim oFramework As New VGDDFramework
                    With oFramework
                        .FrameworkName = reader.GetAttribute("FrameworkName")
                        .FrameworkPath = reader.GetAttribute("FrameworkPath")
                        .MLAVersion = reader.GetAttribute("MLAVersion")
                        reader.ReadStartElement()
                        'reader.ReadEndElement()
                    End With
                    Me.Add(oFramework)
                Loop
            End If
        End Sub

        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements System.Xml.Serialization.IXmlSerializable.WriteXml
            For Each oFramework As VGDDFramework In MyBase.List
                writer.WriteStartElement("Framework")
                writer.WriteAttributeString("FrameworkName", oFramework.FrameworkName)
                writer.WriteAttributeString("FrameworkPath", oFramework.FrameworkPath)
                writer.WriteAttributeString("MLAVersion", oFramework.MLAVersion)
                writer.WriteEndElement()
            Next
        End Sub
    End Class

End Namespace