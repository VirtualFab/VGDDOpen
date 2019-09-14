Imports System
Imports System.Xml
Imports System.Windows.Forms
Imports System.IO
Imports VGDDCommon

'Namespace ToolboxLibrary

' <summary>
' ToolboxXmlManager - Reads an XML file and populates the toolbox.
' </summary>
Public Class ToolboxXmlManager

    Private m_toolbox As VGDDIDE.CollapseToolbox = Nothing

    Public Sub New(ByVal toolbox As VGDDIDE.CollapseToolbox)
        MyBase.New()
        m_toolbox = toolbox
    End Sub

    Private ReadOnly Property Toolbox() As VGDDIDE.CollapseToolbox
        Get
            Return m_toolbox
        End Get
    End Property

    Public Function PopulateToolboxInfo() As ToolboxTabCollection
        Try
            If ((Toolbox.FilePath Is Nothing) _
                        OrElse ((Toolbox.FilePath Is "") _
                        OrElse (Toolbox.FilePath Is String.Empty))) Then
                Return PopulateToolboxTabs()
            End If
            Dim xmlDocument As XmlDocument = New XmlDocument
            xmlDocument.Load(Toolbox.FilePath)
            Return PopulateToolboxTabs(xmlDocument)
        Catch ex As Exception
            MessageBox.Show(("Error occured in reading Toolbox.xml file." & vbLf + ex.ToString))
            Return Nothing
        End Try
    End Function

    Private Overloads Function PopulateToolboxTabs() As ToolboxTabCollection
        Dim toolboxTabs As ToolboxTabCollection = New ToolboxTabCollection()
        Dim tabNames() As String = {Strings.GOLControls, Strings.GPLControls, Strings.VirtualWidgets, Strings.CustomWidgets, Strings.ExternalWidgets}
        Dim i As Integer
        For i = 0 To tabNames.Length - 1 Step i + 1
            Dim toolboxTab As ToolboxTab = New ToolboxTab()

            toolboxTab.Name = tabNames(i)
            PopulateToolboxItems(toolboxTab)
            toolboxTabs.Add(toolboxTab)
        Next

        Return toolboxTabs
    End Function

    Private Overloads Sub PopulateToolboxItems(ByVal toolboxTab As ToolboxTab)
        Try
            If toolboxTab Is Nothing Then
                Return
            End If

            Dim typeArray() As Type = Nothing

            Dim toolboxItems As ToolboxItemCollection = New ToolboxItemCollection()
            Select Case toolboxTab.Name
                Case Strings.GOLControls
#If CONFIG = "Debug" Then
                    typeArray = New Type() {GetType(VGDDMicrochip.StaticText), GetType(VGDDMicrochip.Button), GetType(VGDDMicrochip.EditBox), GetType(VGDDMicrochip.CheckBox), GetType(VGDDMicrochip.RadioButton), GetType(VGDDMicrochip.Window), GetType(VGDDMicrochip.GroupBox), GetType(VGDDMicrochip.ListBox), GetType(VGDDMicrochip.Picture), GetType(VGDDMicrochip.ProgressBar), GetType(VGDDMicrochip.RoundDial), GetType(VGDDMicrochip.Slider), GetType(VGDDMicrochip.Meter), GetType(VGDDMicrochip.TextEntry), GetType(VGDDMicrochip.ComboBox)}
#Else
                    typeArray = New Type() {GetType(VGDDMicrochip.StaticText), GetType(VGDDMicrochip.Button), GetType(VGDDMicrochip.EditBox), GetType(VGDDMicrochip.CheckBox), GetType(VGDDMicrochip.RadioButton), GetType(VGDDMicrochip.Window), GetType(VGDDMicrochip.GroupBox), GetType(VGDDMicrochip.ListBox), GetType(VGDDMicrochip.Picture), GetType(VGDDMicrochip.ProgressBar), GetType(VGDDMicrochip.RoundDial), GetType(VGDDMicrochip.Slider), GetType(VGDDMicrochip.Meter), GetType(VGDDMicrochip.TextEntry)}
#End If
                Case Strings.GPLControls
#If CONFIG = "Debug" Then
                    typeArray = New Type() {GetType(VGDDMicrochip.OutTextXY), GetType(VGDDMicrochip.Arc), GetType(VGDDMicrochip.Circle), GetType(VGDDMicrochip.Line), GetType(VGDDMicrochip.Shape), GetType(VGDDMicrochip.Rectangle), GetType(VGDDMicrochip.Gradient), GetType(VGDDMicrochip.PutImage)}
#Else
                    typeArray = New Type() {GetType(VGDDMicrochip.OutTextXY), GetType(VGDDMicrochip.Arc), GetType(VGDDMicrochip.Circle), GetType(VGDDMicrochip.Line), GetType(VGDDMicrochip.Rectangle), GetType(VGDDMicrochip.Gradient), GetType(VGDDMicrochip.PutImage)}
#End If
                Case Strings.VirtualWidgets
#If CONFIG = "Debug" Then
                    typeArray = New Type() {GetType(VGDDMicrochip.SuperGauge), GetType(VGDDMicrochip.Indicator), GetType(VGDDMicrochip.Disp7Seg), GetType(VGDDMicrochip.VuMeter), GetType(VGDDMicrochip.BarGraph), GetType(VGDDMicrochip.MsgBox), GetType(VGDDMicrochip.TextEntryEx), GetType(VGDDMicrochip.StaticTextEx)}
#Else
                    typeArray = New Type() {GetType(VGDDMicrochip.SuperGauge), GetType(VGDDMicrochip.Indicator), GetType(VGDDMicrochip.Disp7Seg), GetType(VGDDMicrochip.VuMeter), GetType(VGDDMicrochip.BarGraph), GetType(VGDDMicrochip.MsgBox), GetType(VGDDMicrochip.TextEntryEx), GetType(VGDDMicrochip.StaticTextEx)}
#End If
                Case Strings.ExternalWidgets
                    If ExternalWidgetsHandler.ExternalWidgets.Count = 0 Then
                        typeArray = Nothing
                    Else
                        ReDim typeArray(-1)
                        For Each oWidget As ExternalWidget In ExternalWidgetsHandler.ExternalWidgets.Values
                            For Each oType As Type In oWidget.Assembly.GetTypes()
                                If oType.GetMethod("GetCode") IsNot Nothing Then
                                    ReDim Preserve typeArray(typeArray.Length)
                                    typeArray(typeArray.Length - 1) = oType
                                End If
                            Next
                        Next
                    End If

                    '    typeArray = New Type() {GetType(VGDDVirtualWidgets.SuperGauge), GetType(VGDDVirtualWidgets.SegDisplay)}
                Case Strings.CustomWidgets
                    'If VGDDCustom.XmlCustomTemplatesDoc Is Nothing Then
                    VGDDCustom.LoadCustomTemplatesDoc()
                    'End If
                    For Each oCCNode As XmlNode In VGDDCustom.XmlCustomTemplatesDoc.DocumentElement.ChildNodes
                        Dim toolboxItem As VgddToolboxItem = New VgddToolboxItem()
                        toolboxItem.Type = GetType(VGDDCustom)
                        toolboxItem.Name = oCCNode.Name
                        toolboxItem.DisplayName = oCCNode.Name
                        Try
                            Dim oXmlTBBitmapNode As XmlNode = oCCNode.SelectSingleNode("Definition/ToolboxBitmap")
                            If oXmlTBBitmapNode IsNot Nothing Then
                                Dim oFilenameAttr As XmlAttribute = oXmlTBBitmapNode.Attributes("FileName")
                                If oFilenameAttr IsNot Nothing Then
                                    Dim strBitmapFile As String = Path.Combine(VGDDCustom.CustomWidgetsFolder, oFilenameAttr.Value)
                                    If File.Exists(strBitmapFile) Then
                                        toolboxItem.Bitmap = New Bitmap(strBitmapFile)
                                        'Else
                                        '    Me.OutputWindow.WriteMessage("ToolboxBitmap file specified for """ & oCCNode.Name & """ not found! File=" & strBitmapFile)
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                        toolboxItems.Add(toolboxItem)
                    Next

                    toolboxTab.ToolboxItems = toolboxItems
                    Exit Sub
            End Select
            If typeArray IsNot Nothing Then

                For i As Integer = 0 To typeArray.Length - 1
                    Dim blnTypeIsGood As Boolean = True
                    If CodeGen.XmlTemplatesDoc IsNot Nothing Then
                        Dim oControlNode As XmlNode = CodeGen.XmlTemplatesDoc.DocumentElement.SelectSingleNode(String.Format("/VGDDCodeTemplate/ControlsTemplates/{0}", typeArray(i).Name))
                        If oControlNode Is Nothing Then
                            blnTypeIsGood = False
                        End If
                    End If
                    If blnTypeIsGood Then
                        Dim toolboxItem As VgddToolboxItem = New VgddToolboxItem()

                        toolboxItem.Type = typeArray(i)
                        toolboxItem.Name = typeArray(i).Name
                        toolboxItem.DisplayName = typeArray(i).Name
                        toolboxItems.Add(toolboxItem)
                    End If
                Next

                toolboxTab.ToolboxItems = toolboxItems
            End If

        Catch ex As Exception
            Debug.Print("Cannot populate " & toolboxTab.ToString)
        End Try
    End Sub

    Private Overloads Function PopulateToolboxTabs(ByVal xmlDocument As XmlDocument) As ToolboxTabCollection
        If (xmlDocument Is Nothing) Then
            Return Nothing
        End If
        Dim toolboxNode As XmlNode = xmlDocument.FirstChild
        If (toolboxNode Is Nothing) Then
            Return Nothing
        End If
        Dim tabCollectionNode As XmlNode = toolboxNode.FirstChild
        If (tabCollectionNode Is Nothing) Then
            Return Nothing
        End If
        Dim tabsNodeList As XmlNodeList = tabCollectionNode.ChildNodes
        If (tabsNodeList Is Nothing) Then
            Return Nothing
        End If
        Dim toolboxTabs As ToolboxTabCollection = New ToolboxTabCollection
        For Each tabNode As XmlNode In tabsNodeList
            If (tabNode Is Nothing) Then
                'TODO: Warning!!! continue If
            End If
            Dim propertiesNode As XmlNode = tabNode.FirstChild
            If (propertiesNode Is Nothing) Then
                'TODO: Warning!!! continue If
            End If
            Dim nameNode As XmlNode = propertiesNode(Strings.Name)
            If (nameNode Is Nothing) Then
                'TODO: Warning!!! continue If
            End If
            Dim toolboxTab As ToolboxTab = New ToolboxTab
            toolboxTab.Name = nameNode.InnerXml.ToString
            PopulateToolboxItems(tabNode, toolboxTab)
            toolboxTabs.Add(toolboxTab)
        Next
        If (toolboxTabs.Count = 0) Then
            Return Nothing
        End If
        Return toolboxTabs
    End Function

    Private Overloads Sub PopulateToolboxItems(ByVal tabNode As XmlNode, ByVal toolboxTab As ToolboxTab)
        If (tabNode Is Nothing) Then
            Return
        End If
        Dim toolboxItemCollectionNode As XmlNode = tabNode(Strings.ToolboxItemCollection)
        If (toolboxItemCollectionNode Is Nothing) Then
            Return
        End If
        Dim toolboxItemNodeList As XmlNodeList = toolboxItemCollectionNode.ChildNodes
        If (toolboxItemNodeList Is Nothing) Then
            Return
        End If
        Dim toolboxItems As ToolboxItemCollection = New ToolboxItemCollection
        For Each toolboxItemNode As XmlNode In toolboxItemNodeList
            If (toolboxItemNode Is Nothing) Then
                'TODO: Warning!!! continue If
            End If
            Dim typeNode As XmlNode = toolboxItemNode(Strings.Type)
            If (typeNode Is Nothing) Then
                'TODO: Warning!!! continue If
            End If
            Dim found As Boolean = False
            Dim loadedAssemblies() As System.Reflection.Assembly = System.AppDomain.CurrentDomain.GetAssemblies
            Dim i As Integer = 0
            Do While ((i < loadedAssemblies.Length) _
                        AndAlso Not found)
                Dim assembly As System.Reflection.Assembly = loadedAssemblies(i)
                Dim types() As System.Type = assembly.GetTypes
                Dim j As Integer = 0
                Do While ((j < types.Length) _
                            AndAlso Not found)
                    Dim type As System.Type = types(j)
                    If (type.FullName = typeNode.InnerXml.ToString) Then
                        Dim toolboxItem As VgddToolboxItem = New VgddToolboxItem
                        toolboxItem.Type = type
                        toolboxItems.Add(toolboxItem)
                        found = True
                    End If
                    j = (j + 1)
                Loop
                i = (i + 1)
            Loop
        Next
        toolboxTab.ToolboxItems = toolboxItems
        Return
    End Sub

    Private Class Strings

        Public Const Toolbox As String = "Toolbox"
        Public Const TabCollection As String = "TabCollection"
        Public Const Tab As String = "Tab"
        Public Const Properties As String = "Properties"
        Public Const Name As String = "Name"
        Public Const ToolboxItemCollection As String = "ToolboxItemCollection"
        Public Const ToolboxItem As String = "ToolboxItem"
        Public Const Type As String = "Type"
        Public Const GOLControls As String = "GOL Widgets"
        Public Const GPLControls As String = "GPL Controls"
        Public Const GOLSchemes As String = "GOLSchemes"
        Public Const CustomWidgets As String = "Custom Widgets"
        Public Const ExternalWidgets As String = "External Widgets"
        Public Const VirtualWidgets As String = "VirtualWidgets"
    End Class
End Class
'End Namespace
