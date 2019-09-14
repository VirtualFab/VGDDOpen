Imports System.IO

Public Class RelativePath
    Public Shared Function Evaluate(ByVal mainDirPath As String, ByVal absoluteFilePath As String) As String
        If absoluteFilePath Is Nothing Then Return Nothing
        Dim firstPathParts As String() = mainDirPath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar)
        Dim secondPathParts As String() = absoluteFilePath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar)
        Dim sameCounter As Integer = 0
        For i As Integer = 0 To Math.Min(firstPathParts.Length, secondPathParts.Length) - 1
            If Not firstPathParts(i).ToLower().Equals(secondPathParts(i).ToLower()) Then
                Exit For
            End If
            sameCounter += 1
        Next
        If sameCounter = 0 Then
            Return absoluteFilePath
        End If
        Dim newPath As String = [String].Empty
        For i As Integer = sameCounter To firstPathParts.Length - 1
            If i > sameCounter Then
                newPath += Path.DirectorySeparatorChar
            End If
            newPath += ".."
        Next
        'If newPath.Length = 0 Then
        '    newPath = "."
        'End If
        For i As Integer = sameCounter To secondPathParts.Length - 1
            newPath += Path.DirectorySeparatorChar
            newPath += secondPathParts(i)
        Next
        If newPath.StartsWith(Path.DirectorySeparatorChar) Then
            newPath = newPath.Substring(1)
        End If
        Return newPath
    End Function
End Class
