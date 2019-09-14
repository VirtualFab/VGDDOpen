Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Data
Imports System.Windows.Forms
Imports System.Diagnostics
Imports VGDDCommon

Public Class MyNameCreationService
    Implements INameCreationService

    Public Shared ContainersCount As Integer = 0

    Public Sub New()
        MyBase.New()
    End Sub

    Function INameCreationService_CreateName(ByVal container As IContainer, ByVal type As Type) As String Implements INameCreationService.CreateName
        If type Is GetType(VGDD.VGDDScreen) Then
            ContainersCount += 1
            'For Each oScreen As VGDD.VGDDScreen In Common._Screens
            '    If oScreen.Name = "Screen" & ContainersCount.ToString Then
            '        ContainersCount += 1
            '    End If
            'Next
            For Each oNode As TreeNode In oMainShell._SolutionExplorer.tvSolution.Nodes(0).Nodes("Screens").Nodes
                If oNode.Name = "Screen" & ContainersCount.ToString Then
                    ContainersCount += 1
                End If
            Next
            Return ("Screen" & ContainersCount.ToString)
        ElseIf container Is Nothing Then
            Return ""
        End If
        Dim cc As ComponentCollection = container.Components
        Dim count As Integer = 0
        For i As Integer = 0 To cc.Count - 1
            Dim comp As Component = CType(cc(i), Component)
            If (comp.GetType Is type) Then
                count += 1
            End If
        Next
        Dim strNewName As String
        For i As Integer = 0 To cc.Count - 1
            Dim comp As Component = CType(cc(i), Component)
            If (comp.GetType Is type) Then
                Dim name As String = comp.Site.Name
                strNewName = type.Name & (count + 1).ToString
                If name = strNewName Then
                    count += 1
                    i = 0
                    Continue For
                End If
            End If
        Next
        Return type.Name & (count + 1).ToString
    End Function

    Function INameCreationService_IsValidName(ByVal name As String) As Boolean Implements INameCreationService.IsValidName
        Return True
    End Function

    Sub INameCreationService_ValidateName(ByVal name As String) Implements INameCreationService.ValidateName
        Return
    End Sub
End Class
'End Namespace
