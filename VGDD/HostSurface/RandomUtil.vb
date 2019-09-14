Imports System
Imports System.Diagnostics
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports System.Windows.Forms
Imports System.Text

'Namespace Host

' <summary>
' Generated random color. It is used by MyRootDesigner
' </summary>
Public Class RandomUtil

    Friend Const MaxRGBInt As Integer = 255

    Private Shared rand As Random = Nothing

    Public Sub New()
        MyBase.New()
        If (rand Is Nothing) Then
            InitializeRandoms((New Random().Next))
        End If
    End Sub

    Private Sub InitializeRandoms(ByVal seed As Integer)
        rand = New Random(seed)
    End Sub

    Public Overridable Function GetColor() As Color
        Dim bval As Byte
        Dim rval As Byte
        Dim gval As Byte
        rval = CType(GetRange(0, MaxRGBInt), Byte)
        gval = CType(GetRange(0, MaxRGBInt), Byte)
        bval = CType(GetRange(0, MaxRGBInt), Byte)
        Dim c As Color = Color.FromArgb(rval, gval, bval)
        Return c
    End Function

    Public Function GetRange(ByVal nMin As Integer, ByVal nMax As Integer) As Integer
        ' Swap max and min if min > max
        If (nMin > nMax) Then
            Dim nTemp As Integer = nMin
            nMin = nMax
            nMax = nTemp
        End If
        If nMax <> Int32.MaxValue Then
            nMax = nMax + 1
        End If
        Dim retVal As Integer = rand.Next(nMin, nMax)
        Return retVal
    End Function
End Class
'End Namespace