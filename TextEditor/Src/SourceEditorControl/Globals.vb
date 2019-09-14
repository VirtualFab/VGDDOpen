Imports System.Collections.Generic
Imports System.Text
Imports System.Diagnostics
Imports System.Drawing

Namespace SourceEditor
    Public Module Globals
        <System.Runtime.CompilerServices.Extension()> _
        Public Function InRange(ByVal x As Integer, ByVal lo As Integer, ByVal hi As Integer) As Integer
            Debug.Assert(lo <= hi)
            Return If(x < lo, lo, (If(x > hi, hi, x)))
        End Function

        <System.Runtime.CompilerServices.Extension()> _
        Public Function IsInRange(ByVal x As Integer, ByVal lo As Integer, ByVal hi As Integer) As Boolean
            Return x >= lo AndAlso x <= hi
        End Function

        <System.Runtime.CompilerServices.Extension()> _
        Public Function HalfMix(ByVal one As Color, ByVal two As Color) As Color
            Return Color.FromArgb((one.A + two.A) >> 1, (one.R + two.R) >> 1, (one.G + two.G) >> 1, (one.B + two.B) >> 1)
        End Function
    End Module
End Namespace
