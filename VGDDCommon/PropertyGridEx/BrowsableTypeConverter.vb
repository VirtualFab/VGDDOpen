Imports System.ComponentModel

Namespace PropertyGridEx
    Public Class BrowsableTypeConverter

        Inherits ExpandableObjectConverter

        Public Enum LabelStyle
            lsNormal
            lsTypeName
            lsEllipsis
            lsText
        End Enum

        Public Class BrowsableLabelStyleAttribute
            Inherits Attribute
            Private eLabelStyle As LabelStyle = LabelStyle.lsEllipsis
            Public Sub New(ByVal LabelStyle As LabelStyle)
                eLabelStyle = LabelStyle
            End Sub
            Public Property LabelStyle() As LabelStyle
                Get
                    Return eLabelStyle
                End Get
                Set(ByVal value As LabelStyle)
                    eLabelStyle = value
                End Set
            End Property
        End Class

        Public Class BrowsableLabelTextAttribute
            Inherits Attribute
            Private strText As String = ""
            Public Sub New(ByVal value As String)
                strText = value
            End Sub
            Public Property Text() As String
                Get
                    Return strText
                End Get
                Set(ByVal value As String)
                    strText = value
                End Set
            End Property
        End Class

        Public Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As System.Type) As Boolean
            Return True
        End Function

        Public Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
            Dim Style As BrowsableLabelStyleAttribute = context.PropertyDescriptor.Attributes(GetType(BrowsableLabelStyleAttribute))
            If Not Style Is Nothing Then
                Select Case Style.LabelStyle
                    Case LabelStyle.lsNormal
                        Return MyBase.ConvertTo(context, culture, value, destinationType)
                    Case LabelStyle.lsTypeName
                        Return "(" & value.GetType.Name & ")"
                    Case LabelStyle.lsEllipsis
                        Return "(...)"
                    Case LabelStyle.lsText
                        Dim text As BrowsableLabelTextAttribute = context.PropertyDescriptor.Attributes(GetType(BrowsableLabelTextAttribute))
                        If text IsNot Nothing Then
                            Return text.Text
                        End If
                End Select
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

    End Class
End Namespace
