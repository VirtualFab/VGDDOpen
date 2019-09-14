' BitConverter
Imports System.ComponentModel
' TypeConverter
' This like tells it to use our custom type converter instead of the default.
<TypeConverter(GetType(MyColorConverter))> _
Public Class MyColor
#Region " The color channel variables w/ accessors/mutators. "
    Private _Red As Byte
    Public Property Red() As Byte
        Get
            Return _Red
        End Get
        Set(ByVal value As Byte)
            _Red = value
        End Set
    End Property

    Private _Green As Byte
    Public Property Green() As Byte
        Get
            Return _Green
        End Get
        Set(ByVal value As Byte)
            _Green = value
        End Set
    End Property

    Private _Blue As Byte
    Public Property Blue() As Byte
        Get
            Return _Blue
        End Get
        Set(ByVal value As Byte)
            _Blue = value
        End Set
    End Property
#End Region

#Region " Constructors "
    Public Sub New()
        _Red = 0
        _Green = 0
        _Blue = 0
    End Sub
    Public Sub New(ByVal red As Byte, ByVal green As Byte, ByVal blue As Byte)
        _Red = red
        _Green = green
        _Blue = blue
    End Sub
    Public Sub New(ByVal rgb As Byte())
        If rgb.Length <> 3 Then
            Throw New Exception("Array must have a length of 3.")
        End If
        _Red = rgb(0)
        _Green = rgb(1)
        _Blue = rgb(2)
    End Sub
    Public Sub New(ByVal argb As Integer)
        Dim bytes As Byte() = BitConverter.GetBytes(argb)
        _Red = bytes(2)
        _Green = bytes(1)
        _Blue = bytes(0)
    End Sub
    Public Sub New(ByVal rgb As String)
        Dim parts As String() = rgb.Split(" "c)
        If parts.Length <> 3 Then
            Throw New Exception("Array must have a length of 3.")
        End If
        _Red = Convert.ToByte(parts(0))
        _Green = Convert.ToByte(parts(1))
        _Blue = Convert.ToByte(parts(2))
    End Sub
#End Region

#Region " Methods "
    Public Shadows Function ToString() As String
        Return [String].Format("{0} {1} {2}", _Red, _Green, _Blue)


    End Function
    Public Function GetRGB() As Byte()
        Return New Byte() {_Red, _Green, _Blue}
    End Function
    Public Function GetARGB() As Integer
        Dim temp As Byte() = New Byte() {_Blue, _Green, _Red, 255}
        Return BitConverter.ToInt32(temp, 0)
    End Function
#End Region
End Class
