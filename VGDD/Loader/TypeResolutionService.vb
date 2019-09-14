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
Imports System.CodeDom.Compiler
Imports System.CodeDom
Imports Microsoft.CSharp
Imports Microsoft.VisualBasic

'Namespace Loader

' <summary>
' This service resolved the types and is required when using the
' CodeDomHostLoader
' </summary>
Public Class TypeResolutionService
    Implements ITypeResolutionService

    Private ht As Hashtable = New Hashtable

    Public Sub New()
        MyBase.New()

    End Sub

    Public Overloads Function GetAssembly(ByVal name As System.Reflection.AssemblyName) As System.Reflection.Assembly Implements ITypeResolutionService.GetAssembly
        Return GetAssembly(name, True)
    End Function

    Public Overloads Function GetAssembly(ByVal name As System.Reflection.AssemblyName, ByVal throwOnErrors As Boolean) As System.Reflection.Assembly Implements ITypeResolutionService.GetAssembly
        Return Assembly.GetAssembly(GetType(Form))
    End Function

    Public Function GetPathOfAssembly(ByVal name As System.Reflection.AssemblyName) As String Implements ITypeResolutionService.GetPathOfAssembly
        Return Nothing
    End Function

    Public Overloads Function GetThisType(ByVal name As String) As Type Implements ITypeResolutionService.GetType
        Return Me.GetThisType(name, True)
    End Function

    Public Overloads Function GetThisType(ByVal name As String, ByVal throwOnError As Boolean) As Type Implements ITypeResolutionService.GetType
        Dim xType As Type = Me.GetThisType(name, throwOnError, False)
        Return xType
    End Function

    ' <summary>
    ' This method is called when dropping controls from the toolbox
    ' to the host that is loaded using CodeDomHostLoader. For
    ' simplicity we just go through System.Windows.Forms assembly
    ' </summary>
    'Public Overloads Function GetThisType(ByVal name As String, ByVal throwOnError As Boolean, ByVal ignoreCase As Boolean) As Type Implements ITypeResolutionService.GetType
    '    If ht.ContainsKey(name) Then
    '        Return CType(ht(name), Type)
    '    End If
    '    Dim winForms As Assembly = Assembly.GetAssembly(GetType(VGDD.Button))
    '    Dim types() As Type = winForms.GetTypes
    '    Dim typeName As String = String.Empty
    '    For Each type As Type In types
    '        typeName = ("VGDD." + type.Name.ToLower)
    '        If (typeName = name.ToLower) Then
    '            ht(name) = type
    '            Return type
    '        End If
    '    Next
    '    Return Type.GetType(name)
    'End Function
    Public Overloads Function GetThisType(ByVal name As String, ByVal throwOnError As Boolean, ByVal ignoreCase As Boolean) As Type Implements ITypeResolutionService.GetType
        ' try to use Type.GetType, that's fast
        Dim returnType As Type = Type.[GetType](name, False, ignoreCase)
        ' in case GetType is asked for a form, give back rootComponentTyp
        ' this enables HostLoader to load other types than Form
        'If name = "System.Windows.Forms.Form" Then
        '    returnType = Me.rootComponentTyp
        'End If
        If returnType IsNot Nothing Then
            Return returnType
        End If
        ' look up the dictionary  for cached types
        If ht.ContainsKey(name) Then
            Return CType(ht(name), Type)
        End If
        ' in case the type can't be resolved so far,
        ' look for the type in any referenced assembly
        Dim assemblyNames As AssemblyName() = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
        For Each an As AssemblyName In assemblyNames
            Dim a As Assembly = Assembly.Load(an)
            Dim types As Type() = a.GetExportedTypes()
            For Each type As Type In types
                If type.FullName = name Then
                    ht(name) = type
                    Return type
                End If
            Next
        Next
        If throwOnError Then
            Throw New ArgumentException("Can't determine type of " & name & ". Check references.")
        End If
        Return Nothing
    End Function


    Public Sub ReferenceAssembly(ByVal name As System.Reflection.AssemblyName) Implements ITypeResolutionService.ReferenceAssembly

    End Sub
End Class
'End Namespace
' namespace