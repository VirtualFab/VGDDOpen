Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Serialization
'Imports System.Runtime.Serialization
'Imports System.Runtime.Serialization.Formatters.Binary
'Imports System.Runtime.Serialization.Formatters
Imports System.IO

Namespace PropertyGridEx
    <Serializable()> _
    Public Class CustomPropertyCollection
        Inherits System.Collections.CollectionBase
        Implements ICustomTypeDescriptor

        Public Event OnChange(ByVal index As Integer, ByVal value As Object)

#Region "Collection related methods"
        Public Overridable Function Add(ByVal value As CustomProperty) As Integer
            If TypeOf value.Value Is CustomPropertyCollection Then
                AddHandler value.OnBubbleEvent, AddressOf HandleBubbleEvent
            End If
            Return MyBase.List.Add(value)
        End Function

        Public Overridable Function Add(ByVal strName As String, ByVal objValue As Object, Optional ByVal boolIsReadOnly As Boolean = True, Optional ByVal strCategory As String = "", Optional ByVal strDescription As String = "", Optional ByVal boolVisible As Boolean = True) As Integer
            Dim value As New CustomProperty(strName, objValue, boolIsReadOnly, strCategory, strDescription, boolVisible)
            If TypeOf value.Value Is CustomPropertyCollection Then
                AddHandler value.OnBubbleEvent, AddressOf HandleBubbleEvent
            End If
            Return MyBase.List.Add(value)
        End Function

        Public Overridable Function Add(ByVal strName As String, ByRef objRef As Object, ByVal strProp As String, Optional ByVal boolIsReadOnly As Boolean = True, Optional ByVal strCategory As String = "", Optional ByVal strDescription As String = "", Optional ByVal boolVisible As Boolean = True) As Integer
            Dim value As New CustomProperty(strName, objRef, strProp, boolIsReadOnly, strCategory, strDescription, boolVisible)
            If TypeOf value.Value Is CustomPropertyCollection Then
                AddHandler value.OnBubbleEvent, AddressOf HandleBubbleEvent
            End If
            Return MyBase.List.Add(value)
        End Function

        Default Public Overloads ReadOnly Property Item(ByVal index As Integer) As CustomProperty
            Get
                Return DirectCast(MyBase.List.Item(index), CustomProperty)
            End Get
        End Property

        Public Overridable Sub Remove(ByVal Name As String)
            Dim CustomProp As CustomProperty
            For Each CustomProp In MyBase.List
                If CustomProp.Name = Name Then
                    MyBase.List.Remove(CustomProp)
                    Return
                End If
            Next
        End Sub

        Protected Overrides Sub OnClearComplete()
            MyBase.OnClearComplete()
            RaiseEvent OnChange(-1, Nothing)
        End Sub

        Protected Sub HandleBubbleEvent(ByVal index As Integer, ByVal value As Object)
            RaiseEvent OnChange(index, value)
        End Sub

#End Region

#Region "Implements ICustomTypeDescriptor"
        Public Function GetAttributes() As System.ComponentModel.AttributeCollection Implements System.ComponentModel.ICustomTypeDescriptor.GetAttributes
            Return TypeDescriptor.GetAttributes(Me, True)
        End Function

        Public Function GetClassName() As String Implements System.ComponentModel.ICustomTypeDescriptor.GetClassName
            Return TypeDescriptor.GetClassName(Me, True)
        End Function

        Public Function GetComponentName() As String Implements System.ComponentModel.ICustomTypeDescriptor.GetComponentName
            Return TypeDescriptor.GetComponentName(Me, True)
        End Function

        Public Function GetConverter() As System.ComponentModel.TypeConverter Implements System.ComponentModel.ICustomTypeDescriptor.GetConverter
            Return TypeDescriptor.GetConverter(Me, True)
        End Function

        Public Function GetDefaultEvent() As System.ComponentModel.EventDescriptor Implements System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent
            Return TypeDescriptor.GetDefaultEvent(Me, True)
        End Function

        Public Function GetDefaultProperty() As System.ComponentModel.PropertyDescriptor Implements System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty
            Return TypeDescriptor.GetDefaultProperty(Me, True)
        End Function

        Public Function GetEditor(ByVal editorBaseType As System.Type) As Object Implements System.ComponentModel.ICustomTypeDescriptor.GetEditor
            Return TypeDescriptor.GetEditor(Me, editorBaseType, True)
        End Function

        Public Function GetEvents() As System.ComponentModel.EventDescriptorCollection Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, True)
        End Function

        Public Function GetEvents(ByVal attributes() As System.Attribute) As System.ComponentModel.EventDescriptorCollection Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, attributes, True)
        End Function

        Public Function GetProperties() As System.ComponentModel.PropertyDescriptorCollection Implements System.ComponentModel.ICustomTypeDescriptor.GetProperties
            Return TypeDescriptor.GetProperties(Me, True)
        End Function

        Public Function GetProperties(ByVal attributes() As System.Attribute) As System.ComponentModel.PropertyDescriptorCollection Implements System.ComponentModel.ICustomTypeDescriptor.GetProperties

            Dim Properties As New PropertyDescriptorCollection(Nothing)
            Dim CustomProp As CustomProperty
            For Each CustomProp In MyBase.List
                If CustomProp.Hidden Then
                    Dim attrs As ArrayList = New ArrayList()

                    ' Expandable Object Converter
                    If CustomProp.IsBrowsable Then
                        attrs.Add(New TypeConverterAttribute(GetType(BrowsableTypeConverter)))
                    End If

                    ' The Filename Editor
                    If CustomProp.UseFileNameEditor = True Then
                        attrs.Add(New EditorAttribute(GetType(UIFilenameEditor), GetType(UITypeEditor)))
                    End If

                    ' Custom Choices Type Converter
                    If CustomProp.Choices IsNot Nothing AndAlso CustomProp.Choices.Count > 0 Then
                        attrs.Add(New TypeConverterAttribute(GetType(CustomChoices.CustomChoicesTypeConverter)))
                    End If

                    ' Password Property
                    If CustomProp.IsPassword Then
                        attrs.Add(New PasswordPropertyTextAttribute(True))
                    End If

                    ' Parenthesize Property
                    If CustomProp.Parenthesize Then
                        attrs.Add(New ParenthesizePropertyNameAttribute(True))
                    End If

                    ' Datasource
                    If CustomProp.Datasource IsNot Nothing Then
                        attrs.Add(New EditorAttribute(GetType(UIListboxEditor), GetType(UITypeEditor)))
                    End If

                    ' Custom Editor
                    If CustomProp.CustomEditor IsNot Nothing Then
                        attrs.Add(New EditorAttribute(CustomProp.CustomEditor.GetType, GetType(UITypeEditor)))
                    End If

                    ' Custom Type Converter
                    If CustomProp.CustomTypeConverter IsNot Nothing Then
                        attrs.Add(New TypeConverterAttribute(CustomProp.CustomTypeConverter.GetType))
                    End If

                    ' Is Percentage
                    If CustomProp.IsPercentage Then
                        attrs.Add(New TypeConverterAttribute(GetType(OpacityConverter)))
                    End If

                    ' 3-dots button event delegate
                    If CustomProp.OnClick IsNot Nothing Then
                        attrs.Add(New EditorAttribute(GetType(UICustomEventEditor), GetType(UITypeEditor)))
                    End If

                    ' Default value attribute
                    If CustomProp.DefaultType IsNot Nothing Then
                        ' Default type attribute
                        If CustomProp.DefaultType IsNot Nothing Then
                            attrs.Add(New DefaultValueAttribute(CustomProp.DefaultType, CustomProp.DefaultValue))
                        End If
                    Else
                        If CustomProp.Value IsNot Nothing Then
                            attrs.Add(New DefaultValueAttribute(CustomProp.Type, CustomProp.Value.ToString))
                        End If
                    End If

                    ' Extra Attributes
                    If CustomProp.Attributes IsNot Nothing Then
                        attrs.AddRange(CustomProp.Attributes)
                    End If

                    ' Add my own attributes
                    Dim attrArray As Attribute() = attrs.ToArray(GetType(Attribute))
                    Properties.Add(New CustomProperty.CustomPropertyDescriptor(CustomProp, attrArray))
                End If
            Next
            Return Properties
        End Function


        Public Function GetPropertyOwner(ByVal pd As System.ComponentModel.PropertyDescriptor) As Object Implements System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner
            Return Me
        End Function

#End Region

#Region "Serialize & Deserialize related methods"

        Public Sub SaveXml(ByVal filename As String)
            Dim serializer As New XmlSerializer(GetType(CustomPropertyCollection))
            Dim writer As New FileStream(filename, FileMode.Create)
            Try
                serializer.Serialize(writer, Me)
            Catch ex As Exception
                MsgBox(ex.InnerException.Message)
            End Try
            writer.Close()
        End Sub

        Public Function LoadXml(ByVal filename As String, Optional ByVal UpdateDefaultValue As Boolean = False) As Boolean
            Try
                Dim serializer As New XmlSerializer(GetType(CustomPropertyCollection))
                Dim reader As New FileStream(filename, FileMode.Open)

                Dim cpc As CustomPropertyCollection = CType(serializer.Deserialize(reader), CustomPropertyCollection)
                For Each customprop As CustomProperty In cpc
                    If UpdateDefaultValue Then customprop.DefaultValue = customprop.Value
                    customprop.RebuildAttributes()
                    Me.Add(customprop)
                Next
                cpc = Nothing
                reader.Close()
                Return True

            Catch exIO As System.IO.FileNotFoundException
                Return False

            Catch ex As Exception
                MsgBox(ex.InnerException, MsgBoxStyle.Critical, ex.Message)
                Return False
            End Try

        End Function

        'Public Sub SaveBinary(ByVal filename As String)
        '    Dim stream As Stream = File.Create(filename)
        '    Dim serializer As New BinaryFormatter
        '    Try
        '        serializer.Serialize(stream, Me)
        '    Catch ex As Exception
        '        MsgBox(ex.InnerException.Message)
        '    End Try
        '    stream.Close()
        'End Sub

        'Public Function LoadBinary(ByVal filename As String) As Boolean
        '    Try
        '        Dim stream As Stream = File.Open(filename, FileMode.Open)
        '        Dim formatter As New BinaryFormatter()
        '        If stream.Length > 0 Then
        '            Dim cpc As CustomPropertyCollection = CType(formatter.Deserialize(stream), CustomPropertyCollection)
        '            For Each customprop As CustomProperty In cpc
        '                customprop.RebuildAttributes()
        '                Me.Add(customprop)
        '            Next
        '            cpc = Nothing
        '            stream.Close()
        '            Return True
        '        Else
        '            stream.Close()
        '            Return False
        '        End If

        '    Catch ex As Exception
        '        Return False
        '    End Try
        'End Function

#End Region

    End Class
End Namespace
