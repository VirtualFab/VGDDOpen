Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Globalization
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Reflection

Namespace PropertyGridEx

    <Serializable(), XmlRootAttribute("CustomProperty")> _
    Public Class CustomProperty

#Region "Protected variables"

        ' Common properties
        Protected sName As String = ""
        Protected WithEvents oValue As Object = Nothing
        Protected bIsReadOnly As Boolean = False
        Protected _Hidden As Boolean = False
        Protected sDescription As String = ""
        Protected sCategory As String = ""
        Protected bIsPassword As Boolean = Nothing
        Protected bIsPercentage As Boolean = Nothing
        Protected bParenthesize As Boolean = Nothing

        ' Filename editor properties
        Protected sFilter As String = Nothing
        Protected eDialogType As UIFilenameEditor.FileDialogType = UIFilenameEditor.FileDialogType.LoadFileDialog
        Protected bUseFileNameEditor As Boolean = False

        ' Custom choices properties
        Protected oChoices As CustomChoices = Nothing

        ' Browsable properties
        Protected bIsBrowsable As Boolean = False
        Protected sBrowsableText = ""
        Protected eBrowsablePropertyLabel As BrowsableTypeConverter.LabelStyle = BrowsableTypeConverter.LabelStyle.lsEllipsis

        ' Dynamic properties
        Protected bRef As Boolean = False
        Protected oRef As Object = Nothing
        Protected sProp As String = ""

        ' Databinding properties
        Protected oDatasource As Object = Nothing
        Protected sDisplayMember As String = Nothing
        Protected sValueMember As String = Nothing
        Protected oSelectedValue As Object = Nothing
        Protected oSelectedItem As Object = Nothing
        Protected bIsDropdownResizable As Boolean = Nothing

        ' 3-dots button event handler        
        Protected MethodDelegate As UICustomEventEditor.OnClick

        ' Extended Attributes
        <NonSerialized()> Protected oCustomAttributes As AttributeCollection = Nothing
        Protected oTag As Object = Nothing
        Protected oDefaultValue As Object = Nothing
        Protected oDefaultType As Type = Nothing

        ' Custom Editor and Custom Type Converter
        <NonSerialized()> Protected oCustomEditor As UITypeEditor = Nothing
        <NonSerialized()> Protected oCustomTypeConverter As TypeConverter = Nothing

        ' Public Events
        Public Event OnBubbleEvent(ByVal index As Integer, ByVal value As Object)

#End Region

#Region "Public methods"

        Public Sub New()
            sName = "New Property"
            oValue = New String("")
        End Sub

        Public Sub New(ByVal strName As String, ByVal objValue As Object, Optional ByVal boolIsReadOnly As Boolean = True, Optional ByVal strCategory As String = "", Optional ByVal strDescription As String = "", Optional ByVal boolVisible As Boolean = True)
            sName = strName
            oValue = objValue
            bIsReadOnly = boolIsReadOnly
            sDescription = strDescription
            sCategory = strCategory
            _Hidden = boolVisible
            If oValue IsNot Nothing Then
                oDefaultValue = oValue
                If TypeOf oValue Is CustomPropertyCollection Then
                    AddHandler CType(oValue, CustomPropertyCollection).OnChange, AddressOf OnCollectionChange
                End If
            End If
        End Sub

        Public Sub New(ByVal strName As String, ByRef objRef As Object, ByVal strProp As String, Optional ByVal boolIsReadOnly As Boolean = True, Optional ByVal strCategory As String = "", Optional ByVal strDescription As String = "", Optional ByVal boolVisible As Boolean = True)
            sName = strName
            bIsReadOnly = boolIsReadOnly
            sDescription = strDescription
            sCategory = strCategory
            _Hidden = boolVisible
            bRef = True
            oRef = objRef
            sProp = strProp
            If Value IsNot Nothing Then
                oDefaultValue = Value
            End If
        End Sub

        Public Sub New(ByVal strName As String, ByVal node As XmlNode, Optional ByVal strCategory As String = "", Optional ByVal strDescription As String = "")
            sName = strName
            sCategory = strCategory
            sDescription = strDescription
            oValue = LoadXmlNode(node, Nothing)
        End Sub

        Public Function ToXml() As String
            Dim attr As String = RTrim(" " & Me.BrowsableText)
            Dim result As String = "<" & Me.Name & attr & ">"
            If TypeOf Me.Value Is CustomPropertyCollection Then
                Dim cpc As CustomPropertyCollection = CType(Me.Value, CustomPropertyCollection)
                For Each c As CustomProperty In cpc
                    result += c.ToXml
                Next
            Else
                If Me.Value IsNot Nothing Then result += Me.Value.ToString
            End If
            result += "</" & Me.Name & ">"
            Return result
        End Function

        Protected Shared Sub CreateBrowsableProperty(ByRef p As CustomProperty, ByVal node As XmlNode, ByVal nsMgr As XmlNamespaceManager)
            Dim text As String = ""
            p.Value = LoadXmlNode(node, nsMgr)
            For Each attr As XmlAttribute In node.Attributes
                If text.Length > 0 Then text += " "
                text += String.Format("{0}=""{1}""", attr.Name, attr.Value.ToString)
            Next
            p.IsBrowsable = True
            p.BrowsableText = text

        End Sub

        Public Shared Function LoadXmlNode(ByVal node As XmlNode, ByVal nsMgr As XmlNamespaceManager) As CustomPropertyCollection
            Dim nodes As XmlNodeList = node.ChildNodes
            Dim result As New CustomPropertyCollection()

            For Each child As XmlNode In nodes
                Dim p As New CustomProperty(child.Name, Nothing, False, "")

                If child.ChildNodes.Count > 1 Then
                    ' Node has childs and must be browsable
                    CreateBrowsableProperty(p, child, nsMgr)

                ElseIf child.ChildNodes.Count = 1 Then

                    ' Node has 1 child node, checking to remove #text
                    If child.FirstChild IsNot Nothing Then
                        If child.FirstChild.Name = "#text" Then
                            p.Value = child.FirstChild.Value
                        Else
                            CreateBrowsableProperty(p, child, nsMgr)
                        End If
                    Else
                        p.Value = ""
                    End If

                Else

                    ' Node is a terminal node
                    If child.Value IsNot Nothing Then
                        p.Value = child.Value.ToString
                    Else
                        p.Value = ""
                    End If
                End If
                p.DefaultValue = p.Value
                p.Tag = child.OuterXml
                result.Add(p)
            Next
            Return result
        End Function

        Public Sub RebuildAttributes()
            If bUseFileNameEditor Then
                BuildAttributes_FilenameEditor()
            ElseIf oChoices IsNot Nothing AndAlso oChoices.Count > 0 Then
                BuildAttributes_CustomChoices()
            ElseIf oDatasource IsNot Nothing Then
                BuildAttributes_ListboxEditor()
            ElseIf bIsBrowsable Then
                BuildAttributes_BrowsableProperty()
            ElseIf MethodDelegate IsNot Nothing Then
                BuildAttributes_CustomEventProperty()
            End If
        End Sub

#End Region

#Region "Private methods"

        Private Sub BuildAttributes_FilenameEditor()
            Dim attrs As ArrayList = New ArrayList()
            Dim FilterAttribute As New UIFilenameEditor.FileDialogFilterAttribute(sFilter)
            Dim SaveDialogAttribute As New UIFilenameEditor.SaveFileAttribute
            Dim attrArray As Attribute()
            attrs.Add(FilterAttribute)
            If eDialogType = UIFilenameEditor.FileDialogType.SaveFileDialog Then attrs.Add(SaveDialogAttribute)
            attrArray = attrs.ToArray(GetType(Attribute))
            oCustomAttributes = New AttributeCollection(attrArray)
        End Sub

        Private Sub BuildAttributes_CustomChoices()
            If oChoices IsNot Nothing Then
                Dim list As New CustomChoices.CustomChoicesAttributeList(oChoices.Items)
                Dim attrs As ArrayList = New ArrayList()
                Dim attrArray As Attribute()
                attrs.Add(list)
                attrArray = attrs.ToArray(GetType(Attribute))
                oCustomAttributes = New AttributeCollection(attrArray)
            End If
        End Sub

        Private Sub BuildAttributes_ListboxEditor()
            If oDatasource IsNot Nothing Then
                Dim ds As New UIListboxEditor.UIListboxDatasource(oDatasource)
                Dim vm As New UIListboxEditor.UIListboxValueMember(sValueMember)
                Dim dm As New UIListboxEditor.UIListboxDisplayMember(sDisplayMember)
                Dim ddr As UIListboxEditor.UIListboxIsDropDownResizable = Nothing
                Dim attrs As ArrayList = New ArrayList()
                attrs.Add(ds)
                attrs.Add(vm)
                attrs.Add(dm)
                If bIsDropdownResizable Then
                    ddr = New UIListboxEditor.UIListboxIsDropDownResizable
                    attrs.Add(ddr)
                End If
                Dim attrArray As Attribute()
                attrArray = attrs.ToArray(GetType(Attribute))
                oCustomAttributes = New AttributeCollection(attrArray)
            End If
        End Sub

        Private Sub BuildAttributes_BrowsableProperty()
            Dim style As New BrowsableTypeConverter.BrowsableLabelStyleAttribute(eBrowsablePropertyLabel)
            Dim text As New BrowsableTypeConverter.BrowsableLabelTextAttribute(sBrowsableText)
            oCustomAttributes = New AttributeCollection(New Attribute() {style, text})
        End Sub

        Private Sub BuildAttributes_CustomEventProperty()
            Dim attr As New UICustomEventEditor.DelegateAttribute(MethodDelegate)
            oCustomAttributes = New AttributeCollection(New Attribute() {attr})
        End Sub

        Private Property DataColumn() As Object
            Get
                Dim oRow As DataRow = CType(oRef, System.Data.DataRow)
                If oRow.RowState <> DataRowState.Deleted Then
                    If oDatasource Is Nothing Then
                        Return oRow(sProp)
                    Else
                        Dim oLookupTable As DataTable = TryCast(oDatasource, DataTable)
                        If oLookupTable IsNot Nothing Then
                            Return oLookupTable.Select(sValueMember & "=" & oRow(sProp))(0).Item(sDisplayMember)
                        Else
                            Err.Raise(vbObjectError + 513, , "Bind of DataRow with a DataSource that is not a DataTable is not possible")
                            Return Nothing
                        End If
                    End If
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Object)
                Dim oRow As DataRow = CType(oRef, System.Data.DataRow)
                If oRow.RowState <> DataRowState.Deleted Then
                    If oDatasource Is Nothing Then
                        oRow(sProp) = value
                    Else
                        Dim oLookupTable As DataTable = TryCast(oDatasource, DataTable)
                        If oLookupTable IsNot Nothing Then
                            If oLookupTable.Columns(sDisplayMember).DataType.Equals(System.Type.GetType("System.String")) Then
                                oRow(sProp) = oLookupTable.Select(oLookupTable.Columns(sDisplayMember).ColumnName & " = '" & value & "'")(0).Item(sValueMember)
                            Else
                                oRow(sProp) = oLookupTable.Select(oLookupTable.Columns(sDisplayMember).ColumnName & " = " & value)(0).Item(sValueMember)
                            End If
                        Else
                            Err.Raise(vbObjectError + 514, , "Bind of DataRow with a DataSource that is not a DataTable is impossible")
                        End If
                    End If
                End If
            End Set
        End Property

        Private Sub OnCollectionChange(ByVal index As Integer, ByVal value As Object)
            RaiseEvent OnBubbleEvent(index, value)
        End Sub

#End Region

#Region "Public properties"

        <Category("Appearance"), _
         DisplayName("Name"), _
         DescriptionAttribute("Display Name of the CustomProperty."), _
         ParenthesizePropertyName(True), _
         XmlElementAttribute("Name")> _
        Public Property Name() As String
            Get
                Return sName
            End Get
            Set(ByVal value As String)
                sName = VGDDCommon.Common.CleanName(value)
            End Set
        End Property

        <Category("Appearance"), _
         DisplayName("ReadOnly"), _
         DescriptionAttribute("Set read only attribute of the CustomProperty."), _
         XmlElementAttribute("ReadOnly")> _
        Public Property IsReadOnly() As Boolean
            Get
                Return bIsReadOnly
            End Get
            Set(ByVal value As Boolean)
                bIsReadOnly = value
            End Set
        End Property

        <DescriptionAttribute("Set visibility attribute of the CustomProperty.")> _
        <DefaultValue(False)> _
        <Category("Appearance")> _
        Public Property Hidden() As Boolean
            Get
                Return _Hidden
            End Get
            Set(ByVal value As Boolean)
                _Hidden = value
            End Set
        End Property

        <Category("Appearance"), _
         DescriptionAttribute("Represent the Value of the CustomProperty.")> _
        Public Property Value() As Object
            Get
                If bRef Then
                    If TryCast(oRef, DataRow) IsNot Nothing Then
                        Return Me.DataColumn
                    Else
                        Return CallByName(oRef, sProp, CallType.Get)
                    End If
                Else
                    Return oValue
                End If
            End Get
            Set(ByVal value As Object)
                If bRef Then
                    If TryCast(oRef, DataRow) IsNot Nothing Then
                        Me.DataColumn = value
                    Else
                        CallByName(oRef, sProp, CallType.Set, value)
                    End If
                Else
                    oValue = value
                End If
                If TypeOf oValue Is CustomPropertyCollection Then
                    AddHandler CType(oValue, CustomPropertyCollection).OnChange, AddressOf OnCollectionChange
                End If
            End Set
        End Property

        <Category("Appearance"), _
         DescriptionAttribute("Set description associated with the CustomProperty.")> _
        Public Property Description() As String
            Get
                Return sDescription
            End Get
            Set(ByVal value As String)
                sDescription = value
            End Set
        End Property

        <Category("Appearance"), Browsable(False), _
         DescriptionAttribute("Set category associated with the CustomProperty.")> _
        Public Property Category() As String
            Get
                Return sCategory
            End Get
            Set(ByVal value As String)
                sCategory = value
            End Set
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public ReadOnly Property Type() As System.Type
            Get
                If oValue IsNot Nothing Then
                    Return oValue.GetType
                Else
                    If oDefaultType IsNot Nothing Then
                        Return oDefaultType
                    Else
                        If oDefaultValue IsNot Nothing Then
                            Return oDefaultValue.GetType
                        Else
                            Return Nothing
                        End If
                    End If
                End If
            End Get
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public Property Attributes() As AttributeCollection
            Get
                Return oCustomAttributes
            End Get
            Set(ByVal value As AttributeCollection)
                oCustomAttributes = value
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Indicates if the property is browsable or not."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property IsBrowsable() As Boolean
            Get
                Return bIsBrowsable
            End Get
            Set(ByVal value As Boolean)
                bIsBrowsable = value
                If value = True Then
                    BuildAttributes_BrowsableProperty()
                End If
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Specify the label shown when the property is browsable"), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property BrowsableText() As String
            Get
                Return sBrowsableText
            End Get
            Set(ByVal value As String)
                sBrowsableText = value
                eBrowsablePropertyLabel = BrowsableTypeConverter.LabelStyle.lsText
                BuildAttributes_BrowsableProperty()
            End Set
        End Property

        <Category("Appearance"), _
         DisplayName("Parenthesize"), _
         DescriptionAttribute("Indicates whether the name of the associated property is displayed with parentheses in the Properties window."), _
         DefaultValue(False), _
         XmlElementAttribute("Parenthesize"), Browsable(False)> _
        Public Property Parenthesize() As Boolean
            Get
                Return bParenthesize
            End Get
            Set(ByVal value As Boolean)
                bParenthesize = value
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Indicates the style of the label when a property is browsable."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property BrowsableLabelStyle() As BrowsableTypeConverter.LabelStyle
            Get
                Return eBrowsablePropertyLabel
            End Get
            Set(ByVal value As BrowsableTypeConverter.LabelStyle)
                Dim Update As Boolean = False
                If value <> eBrowsablePropertyLabel Then Update = True
                eBrowsablePropertyLabel = value
                If Update Then
                    Dim style As New BrowsableTypeConverter.BrowsableLabelStyleAttribute(value)
                    oCustomAttributes = New AttributeCollection(New Attribute() {style})
                End If
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Indicates if the property is masked or not."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property IsPassword() As Boolean
            Get
                Return bIsPassword
            End Get
            Set(ByVal value As Boolean)
                bIsPassword = value
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Indicates if the property represents a value in percentage."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property IsPercentage() As Boolean
            Get
                Return bIsPercentage
            End Get
            Set(ByVal value As Boolean)
                bIsPercentage = value
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Indicates if the property uses a FileNameEditor converter."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property UseFileNameEditor() As Boolean
            Get
                Return bUseFileNameEditor
            End Get
            Set(ByVal value As Boolean)
                bUseFileNameEditor = value
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Apply a filter to FileNameEditor converter."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property FileNameFilter() As String
            Get
                Return sFilter
            End Get
            Set(ByVal value As String)
                Dim UpdateAttributes As Boolean = False
                If value <> sFilter Then UpdateAttributes = True
                sFilter = value
                If UpdateAttributes Then BuildAttributes_FilenameEditor()
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("DialogType of the FileNameEditor."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property FileNameDialogType() As UIFilenameEditor.FileDialogType
            Get
                Return eDialogType
            End Get
            Set(ByVal value As UIFilenameEditor.FileDialogType)
                Dim UpdateAttributes As Boolean = False
                If value <> eDialogType Then UpdateAttributes = True
                eDialogType = value
                If UpdateAttributes Then BuildAttributes_FilenameEditor()
            End Set
        End Property

        <Category("Behavior"), _
         DescriptionAttribute("Custom Choices list."), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property Choices() As CustomChoices
            Get
                Return oChoices
            End Get
            Set(ByVal value As CustomChoices)
                oChoices = value
                BuildAttributes_CustomChoices()
            End Set
        End Property

        <Category("Databinding"), _
         XmlIgnore(), Browsable(False)> _
        Public Property Datasource() As Object
            Get
                Return oDatasource
            End Get
            Set(ByVal value As Object)
                oDatasource = value
                BuildAttributes_ListboxEditor()
            End Set
        End Property

        <Category("Databinding"), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property ValueMember() As String
            Get
                Return sValueMember
            End Get
            Set(ByVal value As String)
                sValueMember = value
                BuildAttributes_ListboxEditor()
            End Set
        End Property

        <Category("Databinding"), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property DisplayMember() As String
            Get
                Return sDisplayMember
            End Get
            Set(ByVal value As String)
                sDisplayMember = value
                BuildAttributes_ListboxEditor()
            End Set
        End Property

        <Category("Databinding"), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property SelectedValue() As Object
            Get
                Return oSelectedValue
            End Get
            Set(ByVal value As Object)
                oSelectedValue = value
            End Set
        End Property

        <Category("Databinding"), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property SelectedItem() As Object
            Get
                Return oSelectedItem
            End Get
            Set(ByVal value As Object)
                oSelectedItem = value
            End Set
        End Property

        <Category("Databinding"), _
         XmlElementAttribute(IsNullable:=False), Browsable(False)> _
        Public Property IsDropdownResizable() As Boolean
            Get
                Return bIsDropdownResizable
            End Get
            Set(ByVal value As Boolean)
                bIsDropdownResizable = value
                BuildAttributes_ListboxEditor()
            End Set
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public Property CustomEditor() As UITypeEditor
            Get
                Return oCustomEditor
            End Get
            Set(ByVal value As UITypeEditor)
                oCustomEditor = value
            End Set
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public Property CustomTypeConverter() As TypeConverter
            Get
                Return oCustomTypeConverter
            End Get
            Set(ByVal value As TypeConverter)
                oCustomTypeConverter = value
            End Set
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public Property Tag() As Object
            Get
                Return oTag
            End Get
            Set(ByVal value As Object)
                oTag = value
            End Set
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public Property DefaultValue() As Object
            Get
                Return oDefaultValue
            End Get
            Set(ByVal value As Object)
                oDefaultValue = value
            End Set
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public Property DefaultType() As Type
            Get
                Return oDefaultType
            End Get
            Set(ByVal value As Type)
                oDefaultType = value
            End Set
        End Property

        <XmlIgnore(), Browsable(False)> _
        Public Property OnClick() As UICustomEventEditor.OnClick
            Get
                Return MethodDelegate
            End Get
            Set(ByVal value As UICustomEventEditor.OnClick)
                MethodDelegate = value
                BuildAttributes_CustomEventProperty()
            End Set
        End Property

        Public Property HasChildNodes() As Boolean
            Get
                Return TypeOf Value Is CustomPropertyCollection
            End Get
            Set(ByVal value As Boolean)
                If value = True Then
                    Me.Value = New CustomPropertyCollection
                    AddHandler CType(oValue, CustomPropertyCollection).OnChange, AddressOf OnCollectionChange
                End If
            End Set
        End Property

#End Region

#Region "CustomPropertyDescriptor"
        Public Class CustomPropertyDescriptor
            Inherits PropertyDescriptor

            Protected oCustomProperty As CustomProperty

            Public Sub New(ByVal myProperty As CustomProperty, ByVal attrs() As Attribute)
                MyBase.New(myProperty.Name, attrs)
                If myProperty Is Nothing Then
                    oCustomProperty = Nothing
                Else : oCustomProperty = myProperty
                End If
            End Sub

            Public Overrides Function CanResetValue(ByVal component As Object) As Boolean
                If oCustomProperty.DefaultValue IsNot Nothing OrElse oCustomProperty.DefaultType IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Overrides ReadOnly Property ComponentType() As System.Type
                Get
                    Return Me.GetType
                End Get
            End Property

            Public Overrides Function GetValue(ByVal component As Object) As Object
                Return oCustomProperty.Value
            End Function

            Public Overrides ReadOnly Property IsReadOnly() As Boolean
                Get
                    Return oCustomProperty.IsReadOnly
                End Get
            End Property

            Public Overrides ReadOnly Property PropertyType() As System.Type
                Get
                    Return oCustomProperty.Type
                End Get
            End Property

            Public Overrides Sub ResetValue(ByVal component As Object)
                oCustomProperty.Value = oCustomProperty.DefaultValue
                Me.OnValueChanged(component, EventArgs.Empty)
            End Sub

            Public Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
                oCustomProperty.Value = value
                Me.OnValueChanged(component, EventArgs.Empty)
            End Sub

            Public Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
                Dim oValue As Object = oCustomProperty.Value
                If oCustomProperty.DefaultValue IsNot Nothing AndAlso _
                   oValue IsNot Nothing Then
                    Return Not oValue.Equals(oCustomProperty.DefaultValue)
                Else
                    Return False
                End If
            End Function

            Public Overrides ReadOnly Property Description() As String
                Get
                    Return oCustomProperty.Description
                End Get
            End Property

            Public Overrides ReadOnly Property Category() As String
                Get
                    Return oCustomProperty.Category
                End Get
            End Property

            Public Overrides ReadOnly Property DisplayName() As String
                Get
                    Return oCustomProperty.Name
                End Get
            End Property

            Public Overrides ReadOnly Property IsBrowsable() As Boolean
                Get
                    Return oCustomProperty.IsBrowsable
                End Get
            End Property

            Public ReadOnly Property CustomProperty()
                Get
                    Return oCustomProperty
                End Get
            End Property

        End Class
#End Region

    End Class

End Namespace









