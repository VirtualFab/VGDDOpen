Imports System.Xml
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions

Public Class SVGParser

    Public Sub New(ByVal SVGDoc As XmlDocument)

    End Sub

    Public Sub SvgPath2Gdi(ByVal Data As String, ByVal translation As Point, ByVal scale As Point)
        Dim command As Char = " "c
        Dim args As String() = Nothing
        Dim initPoint As New Point()
        'Initial point of the subpath
        Dim endPoint As New Point()
        'End point of the (previous) command
        Dim currentPoint As New Point()
        'Current point of the command
        Dim point As New Point()
        'Parsing variable
        Dim path As New GraphicsPath()
        Dim points As New List(Of Point)

        Dim aFun = Regex.Split(Data, "(?=[A-Za-z])")
        For Each strFun As String In aFun
            If strFun = String.Empty Then Continue For
            command = strFun(0)
            args = Regex.Split(strFun.Remove(0, 1), "[\s,]|(?=-)")
            '.Where(Function(c) c.HasValue()).[Select](Function(c) c.ToInt()).ToArray()
            Select Case command
                Case "M"c, "m"c
                    'Open subpath
                    initPoint = New Point( _
                        (If(Char.IsUpper(command), translation.X, currentPoint.X)) + args(0) * scale.X, _
                        (If(Char.IsUpper(command), translation.Y, currentPoint.Y)) + args(1) * scale.Y _
                    )
                    endPoint = initPoint
                    currentPoint = initPoint
                    Exit Select
                Case "Z"c, "z"c
                    path.CloseFigure()
                    currentPoint = initPoint
                    'Init point becomes the current point
                    Exit Select
                Case "C"c, "c"c
                    points.Clear()
                    points.Add(endPoint)
                    Dim n As Integer = 0
                    For i As Integer = 0 To args.Length - 1 Step 2
                        point = New Point( _
                            (If(Char.IsUpper(command), translation.X, currentPoint.X)) + args(i) * scale.X, _
                            (If(Char.IsUpper(command), translation.Y, currentPoint.Y)) + args(i + 1) * scale.Y
                        )
                        points.Add(point)
                        If System.Threading.Interlocked.Increment(n) >= 3 Then
                            'Not a control point
                            currentPoint = point
                            endPoint = point
                            n = 0
                        End If
                    Next
                    path.AddBeziers(points.ToArray())
                    Exit Select
                Case "L"c, "l"c
                    points.Clear()
                    points.Add(endPoint)
                    For i As Integer = 0 To args.Length - 1 Step 2
                        point = New Point( _
                         (If(Char.IsUpper(command), translation.X, currentPoint.X)) + args(i) * scale.X, _
                         (If(Char.IsUpper(command), translation.Y, currentPoint.Y)) + args(i + 1) * scale.Y _
                        )
                        points.Add(point)
                        currentPoint = point
                    Next
                    endPoint = currentPoint
                    path.AddLines(points.ToArray())
                    Exit Select
            End Select
        Next
    End Sub
End Class
