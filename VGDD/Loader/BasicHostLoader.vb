Imports System
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
'Imports System.Collections
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml
Imports VGDDCommon

'Namespace VGDD

' <summary>
' Inherits from BasicDesignerLoader. It can persist the HostSurface
' to an Xml file and can also parse the Xml file to re-create the
' RootComponent and all the components that it hosts.
' </summary>
Public Class BasicHostLoader
    Inherits BasicDesignerLoader
    Implements IDesignerSerializationService

    Private root As IComponent
    Private _Dirty As Boolean = True
    Public unsaved As Boolean
    Private _fileName As String
    Private host As IDesignerLoaderHost
    Private xmlDocument As XmlDocument
    Private Shared propertyAttributes() As Attribute = New Attribute() {DesignOnlyAttribute.No}
    Private rootComponentType As Type
    Private XmlNodeToLoad As XmlNode
    Public _Screen As VGDD.VGDDScreen = Nothing
    Public errors As New ArrayList

    'Public Event ComponentChanged(ByVal ce As ComponentChangedEventArgs)

    ' Empty constructor simply creates a new form.
    Public Sub New(ByVal rootComponentType As Type)
        MyBase.New()
        Me.rootComponentType = rootComponentType
        Me.Modified = True
    End Sub

    Public Sub New(ByRef XmlNode As XmlNode, ByVal rootComponentType As Type)
        MyBase.New()
        Me.rootComponentType = rootComponentType
        Me.XmlNodeToLoad = XmlNode
        Me.Modified = True
    End Sub

    'Public ReadOnly Property LoadedScreen As VGDD.VGDDScreen
    '    Get
    '        Return _Screen
    '    End Get
    'End Property

    ' <summary>
    ' This constructor takes a file name.  This file
    ' should exist on disk and consist of XML that
    ' can be read by a data set.
    ' </summary>
    Public Sub New(ByVal fileName As String)
        MyBase.New()
        If (fileName Is Nothing) Then
            Throw New ArgumentNullException("fileName")
        End If
        Me._fileName = fileName
    End Sub

    Public Property FileName As String
        Get
            Return _fileName
        End Get
        Set(ByVal value As String)
            _fileName = value
            _Dirty = True
        End Set
    End Property

    ' Called by the host when we load a document.
    Protected Overrides Sub PerformLoad(ByVal designerSerializationManager As IDesignerSerializationManager)
        Me.host = Me.LoaderHost
        If host Is Nothing Then
            Throw New ArgumentNullException("BasicHostLoader.BeginLoad: Invalid designerLoaderHost.")
        End If
        ' The loader will put error messages in here.
        Dim successful As Boolean = True
        Dim baseClassName As String
        ' If no filename was passed in, just create a form and be done with it.  If a file name
        ' was passed, read it.
        If _fileName IsNot Nothing Then
            baseClassName = ReadFile(_fileName, errors, xmlDocument)
        ElseIf XmlNodeToLoad IsNot Nothing Then
            baseClassName = ParseXml(XmlNodeToLoad, errors)
        Else
            If (rootComponentType Is GetType(VGDD.VGDDScreen)) Then
                host.CreateComponent(GetType(VGDD.VGDDScreen))
                baseClassName = "Screen1"
            ElseIf (rootComponentType Is GetType(UserControl)) Then
                host.CreateComponent(GetType(UserControl))
                baseClassName = "UserControl1"
            ElseIf (rootComponentType Is GetType(Component)) Then
                host.CreateComponent(GetType(Component))
                baseClassName = "Component1"
            Else
                Throw New Exception(("Undefined Host Type: " + rootComponentType.ToString))
            End If
        End If
        ' Now that we are done with the load work, we need to begin to listen to events.
        ' Listening to event notifications is how a designer "Loader" can also be used
        ' to save data.  If we wanted to integrate this loader with source code control,
        ' we would listen to the "ing" events as well as the "ed" events.
        Dim cs As IComponentChangeService = CType(host.GetService(GetType(IComponentChangeService)), IComponentChangeService)
        If Not cs Is Nothing Then
            AddHandler cs.ComponentChanged, AddressOf Me.OnComponentChanged
            AddHandler cs.ComponentAdded, AddressOf Me.OnComponentAddedRemoved
            AddHandler cs.ComponentRemoved, AddressOf Me.OnComponentAddedRemoved
            'AddHandler cs.ComponentRename ...
        End If
        ' Let the host know we are done loading.
        host.EndLoad(baseClassName, successful, errors)
        ' We've just loaded a document, so you can bet we need to flush changes.
        _Dirty = True
        unsaved = False
    End Sub

    ' <summary>
    ' This method is called by the designer host whenever it wants the
    ' designer loader to flush any pending changes.  Flushing changes
    ' does not mean the same thing as saving to disk.  For example,
    ' In Visual Studio, flushing changes causes new code to be generated
    ' and inserted into the text editing window.  The user can edit
    ' the new code in the editing window, but nothing has been saved
    ' to disk.  This sample adheres to this separation between flushing
    ' and saving, since a flush occurs whenever the code windows are
    ' displayed or there is a build. Neither of those items demands a save.
    ' </summary>
    Protected Overrides Sub PerformFlush(ByVal designerSerializationManager As IDesignerSerializationManager)
        ' Nothing to flush if nothing has changed.
        If Not _Dirty Then
            Return
        End If
        PerformFlushWorker()
    End Sub

    Public ReadOnly Property IsDirty As Boolean
        Get
            Return _Dirty
        End Get
    End Property

    Public Overrides Sub Dispose()
        ' Always remove attached event handlers in Dispose.
        Dim cs As IComponentChangeService = CType(host.GetService(GetType(IComponentChangeService)), IComponentChangeService)
        If Not cs Is Nothing Then
            RemoveHandler cs.ComponentChanged, AddressOf Me.OnComponentChanged
            RemoveHandler cs.ComponentAdded, AddressOf Me.OnComponentAddedRemoved
            RemoveHandler cs.ComponentRemoved, AddressOf Me.OnComponentAddedRemoved
        End If
        'Me.root.Dispose()
        'Me.root = Nothing
        'If _Screen IsNot Nothing AndAlso Not _Screen.Disposing AndAlso Not _Screen.IsDisposed Then
        '    Try
        '        _Screen.Dispose()
        '    Catch ex As Exception
        '    End Try
        'End If
        If root IsNot Nothing Then
            root.Dispose()
        End If
        _Screen = Nothing
        MyBase.Dispose()
    End Sub

    ' <summary>
    ' Simple helper method that returns true if the given type converter supports
    ' two-way conversion of the given type.
    ' </summary>
    Private Function GetConversionSupported(ByVal converter As TypeConverter, ByVal conversionType As Type) As Boolean
        Return (converter.CanConvertFrom(conversionType) AndAlso converter.CanConvertTo(conversionType))
    End Function

    Public Sub ContentIsChanged()
        _Dirty = True
        unsaved = True
    End Sub
    ' <summary>
    ' As soon as things change, we're dirty, so Flush()ing will give us a new
    ' xmlDocument and codeCompileUnit.
    ' </summary>
    Private Sub OnComponentChanged(ByVal sender As Object, ByVal ce As ComponentChangedEventArgs)
        Me.ContentIsChanged()
        'RaiseEvent ComponentChanged(ce)
    End Sub

    Private Sub OnComponentAddedRemoved(ByVal sender As Object, ByVal ce As ComponentEventArgs)
        Me.ContentIsChanged()
    End Sub

    ' <summary>
    ' This method prompts the user to see if it is OK to dispose this document.  
    ' The prompt only happens if the user has made changes.
    ' </summary>
    Friend Function PromptDispose() As Boolean
        If _Dirty OrElse unsaved Then
            Select Case (MessageBox.Show("Save changes to existing designer?", "Unsaved Changes", MessageBoxButtons.YesNoCancel))
                Case DialogResult.Yes
                    Save(False)
                Case DialogResult.Cancel
                    Return False
            End Select
        End If
        Return True
    End Function

    ' <summary>
    ' This will recussively go through all the objects in the tree and
    ' serialize them to Xml
    ' </summary>
    Public Sub PerformFlushWorker()
        Dim document As XmlDocument = New XmlDocument
        document.AppendChild(document.CreateElement("DOCUMENT_ELEMENT"))
        Dim idh As IDesignerHost = CType(Me.host.GetService(GetType(IDesignerHost)), IDesignerHost)
        root = idh.RootComponent
        Dim nametable As Hashtable = New Hashtable(idh.Container.Components.Count)
        Dim manager As IDesignerSerializationManager = CType(host.GetService(GetType(IDesignerSerializationManager)), IDesignerSerializationManager)
        document.DocumentElement.AppendChild(WriteObject(document, nametable, root))
        For Each comp As IComponent In idh.Container.Components
            If ((Not comp Is root) AndAlso Not nametable.ContainsKey(comp)) Then
                document.DocumentElement.AppendChild(WriteObject(document, nametable, comp))
            End If
        Next
        xmlDocument = document
        _Dirty = False
    End Sub

#Region "Write"
    Public Function WriteObject(ByVal document As XmlDocument, ByVal nametable As IDictionary, ByVal value As Object) As XmlNode
        Dim idh As IDesignerHost = CType(Me.host.GetService(GetType(IDesignerHost)), IDesignerHost)
        Debug.Assert((Not (value) Is Nothing), "Should not invoke WriteObject with a null value")
        Dim node As XmlNode = document.CreateElement("Object")
        Dim typeAttr As XmlAttribute = document.CreateAttribute("type")
        ' typeAttr.Value = value.GetType.ToString '.AssemblyQualifiedName
        typeAttr.Value = value.GetType.AssemblyQualifiedName
        node.Attributes.Append(typeAttr)
        Dim component As IComponent = TryCast(value, IComponent)
        If ((Not component Is Nothing) _
                    AndAlso ((Not component.Site Is Nothing) _
                    AndAlso (Not component.Site.Name Is Nothing))) Then
            Dim nameAttr As XmlAttribute = document.CreateAttribute("name")
            nameAttr.Value = component.Site.Name
            node.Attributes.Append(nameAttr)
            Debug.Assert((nametable(component) = Nothing), "WriteObject should not be called more than once for the same object.  Use WriteReference instead")
            nametable(value) = component.Site.Name
        End If
        Dim isControl As Boolean = (value.GetType Is GetType(IList)) ' (value.GetType Is GetType(Control))
        If isControl Then
            Dim childAttr As XmlAttribute = document.CreateAttribute("children")
            childAttr.Value = "Controls"
            node.Attributes.Append(childAttr)
        End If
        If (Not component Is Nothing) Then
            If isControl Then
                For Each child As Control In CType(value, Control).Controls
                    If ((Not child.Site Is Nothing) _
                                AndAlso (child.Site.Container Is idh.Container)) Then
                        node.AppendChild(WriteObject(document, nametable, child))
                    End If
                Next
            End If
            ' if isControl
            Dim properties As PropertyDescriptorCollection = TypeDescriptor.GetProperties(value, propertyAttributes)
            If isControl Then
                Dim controlProp As PropertyDescriptor = properties("Controls")

                If Not controlProp Is Nothing Then
                    Dim propArray(properties.Count - 1) As PropertyDescriptor
                    Dim idx As Integer = 0

                    Dim p As PropertyDescriptor
                    For Each p In properties
                        If Not p Is controlProp Then
                            idx += 1
                            propArray(idx) = p
                        End If
                    Next

                    properties = New PropertyDescriptorCollection(propArray)
                End If
            End If
            If value.GetType Is GetType(VGDDCustom) Then
                Dim SortedProperties As PropertyDescriptorCollection = properties.Sort(New VGDDPropertyNameComparer)
                WriteProperties(document, SortedProperties, value, node, "Property")
            Else
                WriteProperties(document, properties, value, node, "Property")
            End If

            Dim events As EventDescriptorCollection = TypeDescriptor.GetEvents(value, propertyAttributes)
            Dim bindings As IEventBindingService = CType(host.GetService(GetType(IEventBindingService)), IEventBindingService)
            If (Not bindings Is Nothing) Then
                properties = bindings.GetEventProperties(events)
                WriteProperties(document, properties, value, node, "Event")
            End If
        Else
            WriteValue(document, value, node)
        End If
        Return node
    End Function

    Public Class VGDDPropertyNameComparer
        Implements System.Collections.IComparer
        Public Function Compare(ByVal Prop1 As Object, ByVal Prop2 As Object) As Integer Implements IComparer.Compare
            Dim oPropDesc1 As PropertyDescriptor = Prop1
            Dim oPropDesc2 As PropertyDescriptor = Prop2
            'If PropName1.GetType.ToString = "System.ComponentModel.ReflectPropertyDescriptor" Then
            If oPropDesc1.Name = oPropDesc2.Name Then Return 0
            If oPropDesc1.Name = "CustomWidgetType" Then
                Return -1
            End If
            'End If
            'If PropName2.GetType.ToString = "System.ComponentModel.ReflectPropertyDescriptor" Then
            If oPropDesc2.Name = "CustomWidgetType" Then
                Return 1
            End If
            'End If
            Return 0
        End Function
    End Class


    Private Sub WriteProperties(ByVal document As XmlDocument, ByVal properties As PropertyDescriptorCollection, ByVal value As Object, ByVal parent As XmlNode, ByVal elementName As String)
        For Each prop As PropertyDescriptor In properties
            'If prop.Name = "Text" Then
            '    Debug.Print("he")
            'End If

            If True Then ' prop.ShouldSerializeValue(value) Then
                Dim compName As String = parent.Name
                Dim node As XmlNode = document.CreateElement(elementName)
                Dim attr As XmlAttribute = document.CreateAttribute("name")
                attr.Value = prop.Name
                node.Attributes.Append(attr)
                Dim visibility As DesignerSerializationVisibilityAttribute = CType(prop.Attributes(GetType(DesignerSerializationVisibilityAttribute)), DesignerSerializationVisibilityAttribute)
                Select Case (visibility.Visibility)
                    Case DesignerSerializationVisibility.Visible
                        If (Not prop.IsReadOnly _
                                    AndAlso WriteValue(document, prop.GetValue(value), node)) Then
                            parent.AppendChild(node)
                        End If
                    Case DesignerSerializationVisibility.Content
                        Dim propValue As Object = prop.GetValue(value)
                        If GetType(IList).IsAssignableFrom(prop.PropertyType) Then
                            WriteCollection(document, CType(propValue, IList), node)
                        Else
                            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(propValue, propertyAttributes)
                            WriteProperties(document, props, propValue, node, elementName)
                        End If
                        If (node.ChildNodes.Count > 0) Then
                            parent.AppendChild(node)
                        End If
                End Select
            End If
        Next
    End Sub

    Private Function WriteReference(ByVal document As XmlDocument, ByVal value As IComponent) As XmlNode
        Dim idh As IDesignerHost = CType(Me.host.GetService(GetType(IDesignerHost)), IDesignerHost)
        Debug.Assert(((Not value Is Nothing) _
                        AndAlso ((Not value.Site Is Nothing) _
                        AndAlso (value.Site.Container Is idh.Container))), "Invalid component passed to WriteReference")
        Dim node As XmlNode = document.CreateElement("Reference")
        Dim attr As XmlAttribute = document.CreateAttribute("name")
        attr.Value = value.Site.Name
        node.Attributes.Append(attr)
        Return node
    End Function

    Private Function WriteValue(ByVal document As XmlDocument, ByVal value As Object, ByVal parent As XmlNode) As Boolean
        Dim idh As IDesignerHost = CType(Me.host.GetService(GetType(IDesignerHost)), IDesignerHost)
        ' For empty values, we just return.  This creates an empty node.
        If value Is Nothing Then
            Return True
        End If
        'Dim tValue As Type = value.GetType
        'Dim mi As System.Reflection.MethodInfo = tValue.GetMethod("ToXml")
        'If mi IsNot Nothing Then
        '    mi.Invoke(value, New Object() {document})
        '    Return True
        'End If
        If TypeOf (value) Is VGDDScheme Then
            CType(value, VGDDScheme).ToXml(document)
            Return True
        ElseIf TypeOf (value) Is VGDDEventsCollection Then
            CType(value, VGDDEventsCollection).ToXml(parent)
            Return True
        ElseIf TypeOf (value) Is SegmentsCollection Then ' value.GetType.ToString.Contains("GaugeSegmentsCollection") Then
            'Dim classType As Type =  oMainShell.ExternalWidgetsGetLibraryType("VGDD.GaugeSegmentsCollection", True)
            'If classType IsNot Nothing Then
            'Dim obj As Object = Activator.CreateInstance(classType, parent)
            'obj = value
            CType(value, SegmentsCollection).ToXml(parent)
            'value.ToXml(parent)
            Return True
            'End If
            'ElseIf TypeOf (value) Is VGDDFont Then
            '    CType(value, VGDDFont).ToXml(parent)
            '    Return True
        ElseIf TypeOf (value) Is Common.IVGDDXmlSerialize Then
            Dim oAttr As XmlAttribute

            oAttr = parent.OwnerDocument.CreateAttribute("VGDDType")
            oAttr.Value = value.ToString '.Split(".")(1)
            parent.Attributes.Append(oAttr)

            CType(value, Common.IVGDDXmlSerialize).ToXml(parent)
            Return True
        End If
        Dim converter As TypeConverter = TypeDescriptor.GetConverter(value)
        If GetConversionSupported(converter, GetType(String)) Then
            parent.InnerText = CStr(converter.ConvertTo(Nothing, CultureInfo.InvariantCulture, value, GetType(String)))
        ElseIf GetConversionSupported(converter, GetType(Byte())) Then
            Dim data As Byte() = CType(converter.ConvertTo(Nothing, CultureInfo.InvariantCulture, value, GetType(Byte())), Byte())
            parent.AppendChild(WriteBinary(document, data))
        ElseIf GetConversionSupported(converter, GetType(InstanceDescriptor)) Then
            Dim id As InstanceDescriptor = CType(converter.ConvertTo(Nothing, CultureInfo.InvariantCulture, value, GetType(InstanceDescriptor)), InstanceDescriptor)
            parent.AppendChild(WriteInstanceDescriptor(document, id, value))
        Else
            Dim oComponent As IComponent = TryCast(value, IComponent)
            If oComponent IsNot Nothing AndAlso Not (oComponent.Site Is Nothing) AndAlso oComponent.Site.Container Is idh.Container Then
                parent.AppendChild(WriteReference(document, CType(value, IComponent)))
            Else
                If TryCast(value, IXmlSerializable) IsNot Nothing Then
                    'Dim node As XmlNode = document.CreateElement("Reference")
                    'Dim attr As XmlAttribute = document.CreateAttribute("name")
                    ''attr.Value = value.Site.Name
                    'attr.Value = value.ToString
                    'node.Attributes.Append(attr)
                    Dim sb As New StringBuilder
                    Dim writer As XmlWriter = XmlWriter.Create(sb)
                    Dim serializer As New XmlSerializer(value.GetType)
                    serializer.Serialize(writer, value)
                    'parent.AppendChild(WriteReference(document, CType(value, IComponent)))
                    Dim d As New XmlDocument
                    d.LoadXml(sb.ToString)
                    Dim oNode As XmlNode = d.ChildNodes(1).ChildNodes(0)
                    parent.AppendChild(parent.OwnerDocument.ImportNode(oNode, True))
                ElseIf value.GetType().IsSerializable Then
                    Dim formatter As New BinaryFormatter()
                    Dim stream As New MemoryStream()

                    formatter.Serialize(stream, value)

                    Dim binaryNode As XmlNode = WriteBinary(document, stream.ToArray())

                    parent.AppendChild(binaryNode)
                Else
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Private Sub WriteCollection(ByVal document As XmlDocument, ByVal list As IList, ByVal parent As XmlNode)
        'If list.GetType.ToString.Contains("GaugeSegmentsCollection") Then
        '    Dim classType As Type = Assembly.GetExecutingAssembly().GetType("VW-Gauges.GaugeSegmentsCollection")
        '    If classType IsNot Nothing Then
        '        Dim obj As Object = Activator.CreateInstance(classType)
        '        obj.ToXml(parent)
        '    End If
        '    Return ' True
        'End If
        If list Is Nothing Then Exit Sub
        'If list.GetType.ToString.Contains("DesignerControlCollection")  Then
        '    Dim oNewList As New Collections.SortedList
        '    For Each obj As Object In list
        '        Dim z As Integer = 0
        '        If obj.zorder IsNot Nothing Then
        '            z = obj.zorder
        '        End If
        '        oNewList.Add(z, obj)
        '    Next
        '    list = oNewList.Values
        '    'list.Clear()
        '    'For Each obj As Object In oNewList.Values
        '    '    list.Add(obj)
        '    'Next
        'End If
        For Each obj As Object In list
            If obj IsNot Nothing Then
                Dim node As XmlNode = document.CreateElement("Item")
                Dim typeAttr As XmlAttribute = document.CreateAttribute("type")
                typeAttr.Value = obj.GetType.AssemblyQualifiedName
                node.Attributes.Append(typeAttr)
                WriteValue(document, obj, node)
                parent.AppendChild(node)
            End If
        Next
    End Sub

    Private Function WriteBinary(ByVal document As XmlDocument, ByVal value() As Byte) As XmlNode
        Dim node As XmlNode = document.CreateElement("Binary")
        node.InnerText = Convert.ToBase64String(value)
        Return node
    End Function

    Private Function WriteInstanceDescriptor(ByVal document As XmlDocument, ByVal desc As InstanceDescriptor, ByVal value As Object) As XmlNode
        Dim node As XmlNode = document.CreateElement("InstanceDescriptor")
        Dim formatter As BinaryFormatter = New BinaryFormatter
        Dim stream As MemoryStream = New MemoryStream
        formatter.Serialize(stream, desc.MemberInfo)
        Dim memberAttr As XmlAttribute = document.CreateAttribute("member")
        memberAttr.Value = Convert.ToBase64String(stream.ToArray)
        node.Attributes.Append(memberAttr)
        For Each arg As Object In desc.Arguments
            Dim argNode As XmlNode = document.CreateElement("Argument")
            If WriteValue(document, arg, argNode) Then
                node.AppendChild(argNode)
            End If
        Next
        If Not desc.IsComplete Then
            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(value, propertyAttributes)
            WriteProperties(document, props, value, node, "Property")
        End If
        Return node
    End Function
#End Region

#Region "Read"

    ' <summary>
    ' This method is used to parse the given file.  Before calling this 
    ' method the host member variable must be setup.  This method will
    ' create a data set, read the data set from the XML stored in the
    ' file, and then walk through the data set and create components
    ' stored within it.  The data set is stored in the persistedData
    ' member variable upon return.
    ' 
    ' This method never throws exceptions.  It will set the successful
    ' ref parameter to false when there are catastrophic errors it can't
    ' resolve (like being unable to parse the XML).  All error exceptions
    ' are added to the errors array list, including minor errors.
    ' </summary>
    Private Function ReadFile(ByVal fileName As String, ByVal errors As ArrayList, ByRef document As XmlDocument) As String
        Try
            ' The main form and items in the component tray will be at the
            ' same level, so we have to create a higher super-root in order
            ' to construct our XmlDocument.
            Dim sr As StreamReader = New StreamReader(New FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            Dim cleandown As String = sr.ReadToEnd.Replace(", VGDDCommon,", ", VGDD,").Replace(", VGDDMicrochip,", ", VGDD,")
            cleandown = ("<DOCUMENT_ELEMENT>" + (cleandown + "</DOCUMENT_ELEMENT>"))
            Dim doc As XmlDocument = New XmlDocument
            doc.PreserveWhitespace = True
            doc.LoadXml(cleandown)
            document = doc
            Return ParseXml(doc.DocumentElement, errors)
        Catch exDemo As SystemException
            Throw New SystemException(exDemo.Message)
        Catch ex As Exception
            document = Nothing
            errors.Add(ex.Message)
        End Try
        Return ""
    End Function

    Private Function ParseXml(ByVal XmlNode As XmlNode, ByVal errors As ArrayList) As String
        Dim InstanceName As String = Nothing
        Dim obj As Object = Nothing


        ' Now, walk through the document's elements.
        '
        For Each node As XmlNode In XmlNode.ChildNodes
            If node.NodeType = XmlNodeType.Whitespace Then Continue For
            If node.Name.Equals("Object") Then
                obj = ReadObject(node, errors)
            Else
                'errors.Add(String.Format("Node type {0} is not allowed here.", node.Name))
            End If
            If InstanceName Is Nothing Then
                InstanceName = node.Attributes("name").Value
                Dim suffix As Integer = 0
                Dim NewInstanceName As String = InstanceName
                Do While Common.aScreens.ContainsKey(NewInstanceName.ToUpper)
                    If Common.aScreens(NewInstanceName).Screen Is Nothing Then
                        Exit Do
                    End If
                    NewInstanceName = InstanceName & Chr(suffix + Asc("a"))
                    suffix += 1
                Loop
                If NewInstanceName <> InstanceName Then
                    For i As Integer = 1 To 2
                        Dim PropDesColl As PropertyDescriptorCollection = TypeDescriptor.GetProperties(obj)
                        If PropDesColl IsNot Nothing Then
                            Dim PropDes As PropertyDescriptor = PropDesColl("Name")
                            If PropDes IsNot Nothing Then
                                PropDes.SetValue(obj, NewInstanceName)
                            End If
                        End If
                    Next i
                End If
                _Screen = obj
                '_Screen.Name = InstanceName
                '_Screen.BasePath = Path.GetDirectoryName(fileName)
            Else
                _Screen.Controls.Add(obj)
            End If
        Next
        Return InstanceName
    End Function

    Private Sub ReadEvent(ByVal childNode As XmlNode, ByVal instance As Object, ByVal errors As ArrayList)
        Dim bindings As IEventBindingService = CType(host.GetService(GetType(IEventBindingService)), IEventBindingService)
        If bindings Is Nothing Then
            errors.Add("Unable to contact event binding service so we can't bind any events")
            Return
        End If
        Dim nameAttr As XmlAttribute = childNode.Attributes("name")
        If nameAttr Is Nothing Then
            errors.Add("No event name")
            Return
        End If
        Dim methodAttr As XmlAttribute = childNode.Attributes("method")
        If (methodAttr Is Nothing _
                    OrElse ((methodAttr.Value Is Nothing) _
                    OrElse (methodAttr.Value.Length = 0))) Then
            errors.Add(String.Format("Event {0} has no method bound to it"))
            Return
        End If
        Dim evt As EventDescriptor = TypeDescriptor.GetEvents(instance)(nameAttr.Value)
        If evt Is Nothing Then
            errors.Add(String.Format("Event {0} does not exist on {1}", nameAttr.Value, instance.GetType.FullName))
            Return
        End If
        Dim prop As PropertyDescriptor = bindings.GetEventProperty(evt)
        Debug.Assert((Not prop Is Nothing), "Bad event binding service")
        Try
            prop.SetValue(instance, methodAttr.Value)
        Catch ex As Exception
            errors.Add(ex.Message)
        End Try
    End Sub

    Private Function ReadInstanceDescriptor(ByVal node As XmlNode, ByVal errors As ArrayList) As Object
        ' First, need to deserialize the member
        '
        Dim memberAttr As XmlAttribute = node.Attributes("member")
        If memberAttr Is Nothing Then
            errors.Add("No member attribute on instance descriptor")
            Return Nothing
        End If
        Dim data() As Byte = Convert.FromBase64String(memberAttr.Value)
        Dim formatter As BinaryFormatter = New BinaryFormatter
        Dim stream As MemoryStream = New MemoryStream(data)
        Dim mi As MemberInfo = CType(formatter.Deserialize(stream), MemberInfo)
        Dim args() As Object = Nothing
        ' Check to see if this member needs arguments.  If so, gather
        ' them from the XML.
        If (mi Is MethodBase.GetCurrentMethod) Then
            Dim paramInfos() As ParameterInfo = CType(mi, MethodBase).GetParameters
            args = New Object((paramInfos.Length) - 1) {}
            Dim idx As Integer = 0
            For Each child As XmlNode In node.ChildNodes
                If child.NodeType = XmlNodeType.Whitespace Then Continue For
                If child.Name.Equals("Argument") Then
                    Dim value As Object = Nothing
                    If Not ReadValue(child, TypeDescriptor.GetConverter(paramInfos(idx).ParameterType), errors, value) Then
                        Return Nothing
                    End If
                    idx += 1
                    args(idx) = value
                End If
            Next
            If (idx <> paramInfos.Length) Then
                errors.Add(String.Format("Member {0} requires {1} arguments, not {2}.", mi.Name, args.Length, idx))
                Return Nothing
            End If
        End If
        Dim id As InstanceDescriptor = New InstanceDescriptor(mi, args)
        Dim instance As Object = id.Invoke
        ' Ok, we have our object.  Now, check to see if there are any properties, and if there are, 
        ' set them.
        '
        For Each prop As XmlNode In node.ChildNodes
            If prop.NodeType = XmlNodeType.Whitespace Then Continue For
            If prop.Name.Equals("Property") Then
                ReadProperty(prop, instance, errors)
            End If
        Next
        Return instance
    End Function

    ' Reads the "Object" tags. This returns an instance of the
    ' newly created object. Returns null if there was an error.
    Public Function ReadObject(ByVal node As XmlNode, ByVal errors As ArrayList) As Object
        Dim typeAttr As XmlAttribute = node.Attributes("type")
        If typeAttr Is Nothing Then
            errors.Add("<Object> tag is missing required type attribute")
            Return Nothing
        End If
        Dim strType As String = typeAttr.Value
        'If strType.Contains("Gauge") Then
        '    'strType = strType.Replace("VGDDVirtualWidgets.SuperGauge, VirtualWidgets,", "VGDDVirtualWidgets.SuperGauge, VGDD,")
        'End If
        If strType.StartsWith("VGDD.VGDDScreen") Or strType.StartsWith("VGDDMicrochip.Screen") Then
            'strType = strType.Replace("VGDD.VGDDScreen, VGDD,", "VGDD.VGDDScreen, VGDDCommon,")
            strType = GetType(VGDD.VGDDScreen).AssemblyQualifiedName
        End If

        Dim type As Type = MainShell.VGDDGetType(strType)

        If type Is Nothing Then
            'If typeAttr.Value.ToString.Contains("VGDDMicrochip.Screen") Then    'Old format
            '    type = GetType(VGDD.VGDDScreen)
            'Else
            errors.Add(String.Format("Type {0} could not be loaded.", typeAttr.Value))
            Return Nothing
            'End If
        End If
        ' This can be null if there is no name for the object.
        '
        Dim nameAttr As XmlAttribute = node.Attributes("name")
        Dim instance As Object
        If GetType(IComponent).IsAssignableFrom(type) Then
            If nameAttr Is Nothing Then
                instance = host.CreateComponent(type)
            Else
                instance = host.CreateComponent(type, Common.CleanName(nameAttr.Value))
            End If
        Else
            instance = Activator.CreateInstance(type)
        End If

        ' Got an object, now we must process it.  Check to see if this tag
        ' offers a child collection for us to add children to.
        '
        If TypeOf (instance) Is System.ComponentModel.ISupportInitialize Then
            CType(instance, System.ComponentModel.ISupportInitialize).BeginInit()
        End If
        Dim childAttr As XmlAttribute = node.Attributes("children")
        Dim childList As IList = Nothing
        If Not childAttr Is Nothing Then
            Dim childProp As PropertyDescriptor = TypeDescriptor.GetProperties(instance)(childAttr.Value)
            If childProp Is Nothing Then
                errors.Add(String.Format("The children attribute lists {0} as the child collection but this is not a property on {1}", childAttr.Value, instance.GetType.FullName))
            Else
                childList = CType(childProp.GetValue(instance), IList)
                If childList Is Nothing Then
                    errors.Add(String.Format("The property {0} was found but did not return a valid IList", childProp.Name))
                End If
            End If
        End If
        ' Now, walk the rest of the tags under this element.
        '
        For Each childNode As XmlNode In node.ChildNodes
            If childNode.NodeType = XmlNodeType.Whitespace Then Continue For
            If childNode.Name.Equals("Object") Then
                ' Another object.  In this case, create the object, and
                ' parent it to ours using the children property.  If there
                ' is no children property, bail out now.
                'If childAttr Is Nothing Then
                '    errors.Add("Child object found but there is no children attribute")
                '    'TODO: Warning!!! continue If
                'End If
                ' no sense doing this if there was an error getting the property.  We've already reported the
                ' error above.
                'If Not childList Is Nothing Then
                Dim childInstance As Object = ReadObject(childNode, errors)
                childList.Add(childInstance)
                'End If
            ElseIf childNode.Name.Equals("Property") Then
                ' A property.  Ask the property to parse itself.
                '
                ReadProperty(childNode, instance, errors)
            ElseIf childNode.Name.Equals("Event") Then
                ' An event.  Ask the event to parse itself.
                '
                ReadEvent(childNode, instance, errors)
            End If
        Next
        If TypeOf (instance) Is System.ComponentModel.ISupportInitialize Then
            CType(instance, System.ComponentModel.ISupportInitialize).EndInit()
        End If
        Return instance
    End Function

    ' Parses the given XML node and sets the resulting property value.
    Private Sub ReadProperty(ByVal node As XmlNode, ByVal instance As Object, ByVal errors As ArrayList)
        Dim nameAttr As XmlAttribute = node.Attributes("name")
        If nameAttr Is Nothing Then
            errors.Add("Property has no name")
            Return
        End If
        Dim prop As PropertyDescriptor = TypeDescriptor.GetProperties(instance)(nameAttr.Value)
        If prop Is Nothing Then
            'errors.Add(String.Format("Property {0} does not exist on {1}", nameAttr.Value, instance.GetType.FullName))
            Return
            'ElseIf prop.Name = "MasterScreens" Then
            'Debug.Print("he")
        End If
        ' Get the type of this property.  We have three options:
        ' 1.  A normal read/write property.
        ' 2.  A "Content" property.
        ' 3.  A collection.
        '
        Dim isContent As Boolean = prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content)
        If isContent Then
            Dim value As Object = prop.GetValue(instance)
            ' Handle the case of a content property that is a collection.
            '
            If TypeOf value Is IList Then
                CType(value, IList).Clear()
                For Each child As XmlNode In node.ChildNodes
                    If child.NodeType = XmlNodeType.Whitespace Then Continue For
                    If child.Name.Equals("Item") Then
                        Dim item As Object = Nothing
                        Dim typeAttr As XmlAttribute = child.Attributes("type")
                        If typeAttr Is Nothing Then
                            errors.Add("Item has no type attribute")
                            'TODO: Warning!!! continue If
                        End If
                        Dim type As Type = System.Type.GetType(typeAttr.Value)
                        If type Is Nothing Then
                            'errors.Add(String.Format("Item type {0} could not be found.", typeAttr.Value))
                            'TODO: Warning!!! continue If
                            Return
                        End If
                        If ReadValue(child, TypeDescriptor.GetConverter(type), errors, item) Then
                            If item IsNot Nothing Then
                                Try
                                    CType(value, IList).Add(item)
                                Catch ex As Exception
                                    errors.Add(ex.Message)
                                End Try
                            End If
                        End If
                    Else
                        errors.Add(String.Format("Only Item elements are allowed in collections, not {0} elements.", child.Name))
                    End If
                Next
            Else
                ' Handle the case of a content property that consists of child properties.
                '
                For Each child As XmlNode In node.ChildNodes
                    If child.NodeType = XmlNodeType.Whitespace Then Continue For
                    If child.Name.Equals("Property") Then
                        ReadProperty(child, value, errors)
                    Else
                        'errors.Add(String.Format("Only Property elements are allowed in content properties, not {0} elements.", child.Name))
                    End If
                Next
            End If
        Else
            Dim value As Object = Nothing
            If prop.Name = "VGDDEvents" Then
                Dim oEvents As New VGDDEventsCollection(node)
                Dim oTemplateEvents As VGDDEventsCollection
                If TypeOf (instance) Is VGDD.VGDDScreen Then
                    oTemplateEvents = CodeGen.GetEventsFromTemplate("Screen")
                Else
                    oTemplateEvents = CodeGen.GetEventsFromTemplate(instance.GetType.ToString.Split(".")(1))
                End If
                If oTemplateEvents Is Nothing Then
                    oTemplateEvents = New VGDDEventsCollection
                End If
                For Each oTemplateEvent As VGDDEvent In oTemplateEvents
                    Dim i As Integer = 0
                    For Each oEvent As VGDDEvent In oEvents
                        If oEvent.Name = oTemplateEvent.LegacyName Then
                            'If oEvent.Code = String.Empty Then
                            '    oEvents.RemoveAt(i)
                            'Else
                            oEvent.Name = oTemplateEvent.Name
                            'End If
                            Exit For
                        End If
                        i += 1
                    Next
                Next
                For Each oTemplateEvent As VGDDEvent In oTemplateEvents
                    If oEvents(oTemplateEvent.Name) Is Nothing Then
                        oEvents.Add(oTemplateEvent)
                    Else
                        oEvents(oTemplateEvent.Name).Description = oTemplateEvent.Description
                        oEvents(oTemplateEvent.Name).PlayerEvent = oTemplateEvent.PlayerEvent
                    End If
                Next
                value = oEvents
            ElseIf prop.Name = "Segments" Then
                'Dim classType As Type = GetType(SegmentsCollection) 'oMainShell.ExternalWidgetsGetLibraryType("VGDD.GaugeSegmentsCollection", True)
                'If classType Is Nothing Then
                '    Return
                'Else
                '    value = Activator.CreateInstance(classType, node)
                '    prop.SetValue(instance, value)
                'End If
                value = New SegmentsCollection(node)
                Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(instance)(prop.Name)
                prop.SetValue(instance, value)
            ElseIf node.Attributes("VGDDType") IsNot Nothing Then
                Dim oType As Type = MainShell.VGDDGetType(node.Attributes("VGDDType").Value)
                If oType IsNot Nothing Then
                    Dim oPropInstance As Object = Activator.CreateInstance(oType)
                    If TypeOf (oPropInstance) Is Common.IVGDDXmlSerialize Then
                        value = CType(oPropInstance, Common.IVGDDXmlSerialize).FromXml(node)
                        'Dim oProp As PropertyDescriptor = TypeDescriptor.GetProperties(instance)(prop.Name)
                        prop.SetValue(instance, value)
                    End If
                End If
            Else
                ReadValue(node, prop.Converter, errors, value)
            End If
            If value IsNot Nothing Then
                'If instance.GetType Is GetType(VGDDCustom) Then
                '    Dim propItem As PropertyDescriptor = TypeDescriptor.GetProperties(instance)("Item")
                '    propItem.SetValue(instance, value)
                'Else
                'If prop.Name = "Location" Then
                '    Debug.Print("Location=" & value.y)
                'ElseIf prop.Name = "Size" Then
                '    Debug.Print("Size=" & value.Height)
                'ElseIf prop.Name = "Height" Then
                '    Debug.Print("Height=" & value)
                'End If
                Try
                    prop.SetValue(instance, value)
                Catch ex As Exception
                    errors.Add(ex.Message)
                End Try
                'End If
            End If
        End If
    End Sub

    ' Generic function to read an object value.  Returns true if the read
    ' succeeded.
    Private Function ReadValue(ByVal node As XmlNode, ByVal converter As TypeConverter, ByVal errors As ArrayList, ByRef value As Object) As Boolean
        Try
            For Each child As XmlNode In node.ChildNodes
                If child.NodeType = XmlNodeType.Whitespace AndAlso Not TypeOf (converter) Is System.ComponentModel.StringConverter Then
                    Continue For
                End If
                'If child.NodeType = XmlNodeType.Whitespace Then
                '    Debug.Print(node.OuterXml)
                'End If
                If (child.NodeType = XmlNodeType.Text Or (child.NodeType = XmlNodeType.Whitespace AndAlso TypeOf (converter) Is System.ComponentModel.StringConverter)) Then
                    If child.InnerText = vbCrLf & "    " Then
                        child.InnerText = ""
                    End If
                    If child.InnerText = vbCrLf & "      " Then
                        child.InnerText = ""
                    End If
                    value = converter.ConvertFromInvariantString(child.InnerText.Replace("SolidLine", "SOLID_LINE") _
                                                                 .Replace("OnePixel", "NORMAL_LINE") _
                                                                 .Replace("ThreePixels", "THICK_LINE"))
                    Return True
                ElseIf child.Name.Equals("Binary") Then
                    Dim data() As Byte = Convert.FromBase64String(child.InnerText)
                    ' Binary blob.  Now, check to see if the type converter
                    ' can convert it.  If not, use serialization.
                    '
                    If GetConversionSupported(converter, GetType(System.Byte())) Then
                        value = converter.ConvertFrom(Nothing, CultureInfo.InvariantCulture, data)
                        Return True
                    Else
                        Dim formatter As BinaryFormatter = New BinaryFormatter
                        Dim stream As MemoryStream = New MemoryStream(data)
                        value = formatter.Deserialize(stream)
                        Return True
                    End If
                ElseIf child.Name.Equals("InstanceDescriptor") Then
                    value = ReadInstanceDescriptor(child, errors)
                    Return (Not (value) Is Nothing)
                Else
                    'errors.Add(String.Format("Unexpected element type {0}", child.Name))
                    value = Nothing
                    Return False
                End If
            Next
            ' If we get here, it is because there were no nodes.  No nodes and no inner
            ' text is how we signify null.
            '
            value = Nothing
            Return True
        Catch ex As Exception
            errors.Add(ex.Message)
            value = Nothing
            Return False
        End Try
    End Function
#End Region

    ' This method writes out the contents of our designer in XML.
    ' It writes out the contents of xmlDocument.
    Public Function GetCode() As String
        Flush()
        Dim sw As StringWriter
        sw = New StringWriter
        Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
        xtw.Formatting = Formatting.Indented
        xmlDocument.WriteTo(xtw)
        Dim cleanup As String = sw.ToString.Replace("<DOCUMENT_ELEMENT>", "")
        cleanup = cleanup.Replace("</DOCUMENT_ELEMENT>", "")
        sw.Close()
        Return cleanup
    End Function

    Public Overloads Function Save(ByVal Name As String) As Boolean
        If unsaved Then
            Return Save(Name, False, False)
        End If
        Return True
    End Function

    Public Overloads Function Save(ByVal Name As String, ByVal forceFilePrompt As Boolean) As Boolean
        Return Save(Name, forceFilePrompt, False)
    End Function


    ' <summary>
    ' Save the currentCommon.State of the loader. If the user loaded the file
    ' or saved once before, then he doesn't need to select a file again.
    ' Unless this is being called as a result of "Save As..." being clicked,
    ' in which case forceFilePrompt will be true.
    ' </summary>
    Public Overloads Function Save(ByVal Name As String, ByVal forceFilePrompt As Boolean, ByVal blnRenameScreen As Boolean) As Boolean
        Dim strOldName As String = String.Empty
        Try
            If _Screen IsNot Nothing Then
                strOldName = _Screen.Site.Name
            End If
            'Dim filterIndex As Integer = 1
            If _fileName Is Nothing OrElse forceFilePrompt Then
                Dim dlg As SaveFileDialog = New SaveFileDialog
                dlg.Title = "Save Screen " & Name
                dlg.AddExtension = True
                dlg.DefaultExt = "vds"
                dlg.Filter = "Visual Graphics Display Screen Files|*.vds"
                If Not dlg.FileName.EndsWith(".vds") Then
                    dlg.FileName = Name & ".vds"
                End If
                dlg.InitialDirectory = Common.VGDDProjectPath
                dlg.RestoreDirectory = True
                If (dlg.ShowDialog = DialogResult.OK) Then
                    _fileName = dlg.FileName
                    'filterIndex = dlg.FilterIndex
                End If
            End If
            If blnRenameScreen AndAlso _Screen IsNot Nothing Then
                _Screen.Site.Name = Path.GetFileNameWithoutExtension(_fileName)
                _Dirty = True
            End If
            Flush()
            If _fileName IsNot Nothing Then
                'Select Case (filterIndex)
                '    Case 1
                ' Write out our xmlDocument to a file.
                Dim sw As New StringWriter
                Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
                xtw.Formatting = Formatting.Indented
                xmlDocument.WriteTo(xtw)
                ' Get rid of our artificial super-root before we save out
                ' the XML.
                '
                Dim cleanup As String = sw.ToString.Replace("<DOCUMENT_ELEMENT>", "")
                cleanup = cleanup.Replace("</DOCUMENT_ELEMENT>", "")
                xtw.Close()
                Common.WriteFileWithBackup(_fileName, cleanup, New System.Text.UnicodeEncoding)
                unsaved = False
            End If
        Catch exIO As IOException
            Dim rc As DialogResult = MessageBox.Show("Unable to save to " & _fileName & ". Do you want to save the screen with another filename?", "Error saving screen", MessageBoxButtons.YesNoCancel)
            Select Case rc
                Case DialogResult.Cancel
                    Return False
                Case DialogResult.Yes
                    Return Save(Name, True)
                Case DialogResult.No
                    Return True
            End Select
        Catch ex As Exception
            MessageBox.Show(("Error during save: " + ex.ToString))
            Return False
        Finally
            If blnRenameScreen AndAlso _Screen IsNot Nothing Then
                _Screen.Site.Name = strOldName
            End If
        End Try
        Return True
    End Function

    Public Function Deserialize(ByVal serializationData As Object) As System.Collections.ICollection Implements System.ComponentModel.Design.Serialization.IDesignerSerializationService.Deserialize
        Dim serializationSvc As ComponentSerializationService = GetService(GetType(ComponentSerializationService))
        Dim designerHost As IDesignerHost = GetService(GetType(IDesignerHost))
        If Not TypeOf serializationData Is SerializationStore Then Throw New ArgumentException("Deserialize: bad serialization store.")
        If serializationSvc Is Nothing Then Throw New InvalidOperationException("Deserialize: unable to get a reference to ComponentSerializationService.")
        If designerHost Is Nothing Then Throw New InvalidOperationException("Deserialize: unable to get a reference to IDesignerHost.")
        Return serializationSvc.Deserialize(DirectCast(serializationData, SerializationStore), designerHost.Container)
    End Function

    Public Function Serialize(ByVal objects As System.Collections.ICollection) As Object Implements System.ComponentModel.Design.Serialization.IDesignerSerializationService.Serialize
        Dim serializationSvc As ComponentSerializationService = GetService(GetType(ComponentSerializationService))
        Dim store As SerializationStore
        If serializationSvc Is Nothing Then Throw New InvalidOperationException("Serialize: unable to get a reference to ComponentSerializationService.")
        store = serializationSvc.CreateStore
        If objects Is Nothing Then
            For Each obj As Object In objects
                serializationSvc.Serialize(store, obj)
            Next
        End If
        store.Close()
        Return store
    End Function
End Class
'End Namespace

