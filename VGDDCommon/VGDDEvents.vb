Imports System.Xml
Imports System.Windows.Forms

Namespace VGDDCommon

    Public Interface IVGDDEventsEditor
        Property ParentScreenName As String
        Sub SetEditedControl(ByVal oControl As Control)
        Property DialogResult As DialogResult
        Sub DisplayModal()
    End Interface

    Public Class VGDDEventsEditorNew
        Inherits System.Drawing.Design.UITypeEditor

        Private Shared _EditForm As Windows.Forms.Form
        Public Shared Property EditForm As System.Windows.Forms.Form
            Get
                Return _EditForm
            End Get
            Set(ByVal value As System.Windows.Forms.Form)
                _EditForm = value
            End Set
        End Property

        Public Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
#If Not PlayerMonolitico Then
            If TypeOf _EditForm Is IVGDDEventsEditor AndAlso Not TypeOf context.Instance Is Object() Then
                Dim EventEditorForm As IVGDDEventsEditor = _EditForm
                EventEditorForm.ParentScreenName = Common.CurrentScreen.Name
                EventEditorForm.SetEditedControl(context.Instance)
                EventEditorForm.DialogResult = DialogResult.Retry
                EventEditorForm.DisplayModal()
                'Do While EventEditorForm.DialogResult = DialogResult.Retry
                '    Application.DoEvents()
                'Loop
                'If EventEditorForm.DialogResult = DialogResult.OK Then
                Return value
                'End If
            End If
            Return Nothing
            'Return MyBase.EditValue(context, provider, value)
#Else
            Return Nothing
#End If
        End Function

        Public Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
            Return System.Drawing.Design.UITypeEditorEditStyle.Modal
        End Function
    End Class

    <Serializable(), System.Xml.Serialization.XmlRoot("VGDDEventsCollection")> _
    Public Class VGDDEventsCollection
        Inherits CollectionBase
        Implements ICloneable

        'Private _ParentScreen As VGDDMicrochip.Screen

#Region " ICloneable Implementation "
        Private Function IClone() As Object Implements ICloneable.Clone
            'Implemented in Public Function Clone()
            Return Nothing
        End Function

        Public Function Clone() As VGDDEventsCollection
            Dim collection As New VGDDEventsCollection()
            For Each item As VGDDEvent In Me.InnerList
                collection.InnerList.Add(item)
            Next
            Return collection
        End Function
#End Region

        Public Sub New()

        End Sub

        Public Sub New(ByRef XmlParent As XmlNode)
            Try
                For Each oEventNode As XmlNode In XmlParent.ChildNodes
                    If oEventNode.NodeType = XmlNodeType.Whitespace Then Continue For
                    Dim oEvent As New VGDDEvent
                    With oEvent
                        .Name = oEventNode.Attributes("Name").Value
                        .Handled = oEventNode.Attributes("Handled").Value
                        .Code = oEventNode.InnerText
                        If .Code.StartsWith(vbCrLf) Then .Code = .Code.Substring(2)
                        If .Code.EndsWith(vbCrLf) Then .Code = .Code.Substring(0, .Code.Length - 2)
                        If .Code.Trim <> "" Then
                            For Each bmp As VGDDImage In Common.Bitmaps
                                If Not bmp.Referenced AndAlso .Code.Contains(IIf(Common.ProjectUseBmpPrefix, "bmp", "") & bmp.Name) Then
                                    bmp._ReferencedInEventCode = True
                                End If
                            Next
                        End If

                        .Description = oEventNode.Attributes("Description").Value
                        If oEventNode.Attributes("PlayerEvent") IsNot Nothing Then
                            .PlayerEvent = oEventNode.Attributes("PlayerEvent").Value
                        End If
                    End With
                    Dim oDuplicateEvent As VGDDEvent = Me.VGDDEvent(oEvent.Name)
                    If oDuplicateEvent Is Nothing Then
                        Me.Add(oEvent)
                    Else
                        If oEvent.Code <> String.Empty And oDuplicateEvent.Code = String.Empty Then
                            oDuplicateEvent.Code = oEvent.Code
                        End If
                    End If
                Next

            Catch ex As Exception

            End Try
        End Sub

        Default Public ReadOnly Property VGDDEvent(ByVal Name As String) As VGDDEvent
            Get
                For Each oEvent As VGDDEvent In MyBase.List
                    If oEvent.Name = Name Then
                        Return oEvent
                    End If
                Next
                Return Nothing
            End Get
        End Property

        'Public Property ParentScreen As VGDDMicrochip.Screen
        '    Get
        '        Return _ParentScreen
        '    End Get
        '    Set(ByVal value As VGDDMicrochip.Screen)
        '        _ParentScreen = value
        '    End Set
        'End Property

        Public Sub Add(ByRef oEvent As VGDDEvent)
            MyBase.List.Add(oEvent)
            Common.ProjectChanged = True
        End Sub

        Public Overloads ReadOnly Property List()
            Get
                Return MyBase.List
            End Get
        End Property

        Public Sub ToXml(ByRef XmlParent As XmlNode)
            Dim XmlDoc As XmlDocument = XmlParent.OwnerDocument
            For Each oEvent As VGDDEvent In MyBase.List
                Dim oEventNode As XmlElement = XmlDoc.CreateElement("Event")
                Dim oAttr As XmlAttribute = XmlDoc.CreateAttribute("Name")
                oAttr.Value = oEvent.Name
                oEventNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("Handled")
                oAttr.Value = oEvent.Handled
                oEventNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("Description")
                oAttr.Value = oEvent.Description
                oEventNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("PlayerEvent")
                oAttr.Value = oEvent.PlayerEvent
                oEventNode.Attributes.Append(oAttr)

                If oEvent.Code.Length > 0 Then
                    oEventNode.InnerText = vbCrLf & oEvent.Code.Replace(vbCrLf, vbLf).Replace(vbLf, vbCrLf) & vbCrLf
                End If
                XmlParent.AppendChild(oEventNode)
            Next
        End Sub
    End Class

    <Serializable(), System.Xml.Serialization.XmlRoot("VGDDEvent")> _
    Public Class VGDDEvent
        Private _Name As String
        Private _LegacyName As String
        Private _Description As String
        Private _Handled As Boolean = False
        Private _Code As String = ""
        Private _PlayerEvent As String = ""

        Public Sub New()
            _Name = ""
        End Sub
        <System.Xml.Serialization.XmlElement("Property_Name")> _
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
        Public Property LegacyName As String
            Get
                Return _LegacyName
            End Get
            Set(ByVal value As String)
                _LegacyName = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property
        Public Property Handled As Boolean
            Get
                Return _Handled
            End Get
            Set(ByVal value As Boolean)
                _Handled = value
            End Set
        End Property
        Public Property Code As String
            Get
                Return _Code
            End Get
            Set(ByVal value As String)
                _Code = value
            End Set
        End Property
        Public Property PlayerEvent As String
            Get
                Return _PlayerEvent
            End Get
            Set(ByVal value As String)
                _PlayerEvent = value
            End Set
        End Property
    End Class

End Namespace
