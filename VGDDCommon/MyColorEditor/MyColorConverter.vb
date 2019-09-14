' Type
Imports System.ComponentModel
' TypeConverter, ITypeDescriptorContext
Imports System.Globalization
' CultureInfo
Public Class MyColorConverter
    Inherits System.Drawing.ColorConverter ' TypeConverter

    Public Overrides Function GetStandardValuesSupported(ByVal context As ITypeDescriptorContext) As Boolean
        Return False
    End Function

    '' This is used, for example, by DefaultValueAttribute to convert from string to MyColor.
    'Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
    '    If value.[GetType]() Is GetType(String) Then
    '        Return New MyColor(DirectCast(value, String))
    '    End If
    '    Return MyBase.ConvertFrom(context, culture, value)
    'End Function
    '' This is used, for example, by the PropertyGrid to convert MyColor to a string.
    'Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destType As Type) As Object
    '    If (destType Is GetType(String)) AndAlso (TypeOf value Is MyColor) Then
    '        Dim color As MyColor = DirectCast(value, MyColor)
    '        Return color.ToString()
    '    End If
    '    Return MyBase.ConvertTo(context, culture, value, destType)
    'End Function
End Class
