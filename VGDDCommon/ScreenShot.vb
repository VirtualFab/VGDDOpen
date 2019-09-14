Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon

Namespace VGDD

#If Not PlayerMonolitico Then
    Public Class VGDDScreenShot
        Inherits Form ' VGDDScreen

        Private _TransparentColour As Color = Nothing

        Private _ZoomFactor As Integer = 200
        Private _ScreenShot As Bitmap

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Try
                MyBase.OnPaint(pevent)

            Catch ex As Exception

            End Try
        End Sub

        Public Property ZoomFactor As Integer
            Get
                Return _ZoomFactor
            End Get
            Set(ByVal value As Integer)
                _ZoomFactor = value
                Me.Scale(New System.Drawing.SizeF(2, 2))
            End Set
        End Property

        Public Property TransparentColour As Color
            Get
                Return (_TransparentColour)
            End Get
            Set(ByVal value As Color)
                _TransparentColour = value
            End Set
        End Property
        '<System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")>
        'Private Shared Function BitBlt(ByVal hdcDest As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, _
        '    ByVal nHeight As Integer, ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As System.Int32) As Boolean
        'End Function

        'Public Sub SaveScreenshot(ByVal Filename As String)
        '    'Dim hGraphics As Graphics = Graphics.FromImage(_ScreenShot)
        '    'hGraphics.DrawRectangle(New Pen(Brushes.Black), 0, 0, _ScreenShot.Width - 1, _ScreenShot.Height - 1)
        '    'hGraphics.CopyFromScreen(Me.Left, Me.Top, 0, 0, sz)
        '    Dim g1 As Graphics = Me.CreateGraphics()
        '    Dim _ScreenShot = New Bitmap(Me.ClientRectangle.Width, Me.ClientRectangle.Height, g1)
        '    Dim g2 As Graphics = Graphics.FromImage(_ScreenShot)
        '    Dim dc1 As IntPtr = g1.GetHdc()
        '    Dim dc2 As IntPtr = g2.GetHdc()
        '    BitBlt(dc2, 0, 0, Me.ClientRectangle.Width, Me.ClientRectangle.Height, dc1, 0, 0, 13369376)
        '    g1.ReleaseHdc(dc1)
        '    g2.ReleaseHdc(dc2)
        '    _ScreenShot.Save(Filename, Imaging.ImageFormat.Png)
        'End Sub 'Capture_Click

        Private Declare Function PrintWindow Lib "user32.dll" (ByVal hwnd As IntPtr, ByVal hdcBlt As IntPtr, ByVal nFlags As UInt32) As Boolean
        'Dim WithEvents t As New Timer

        'Dim screenCapture As Bitmap
        'Dim otherForm As New Form

        Public Sub SaveScreenshot(ByVal Filename As String)
            Me.Text = "capturing, " & Me.Location.X & "," & Me.Location.Y
            _ScreenShot = New Bitmap(Me.Width, Me.Height)
            Dim g As Graphics = Graphics.FromImage(_ScreenShot)
            Dim hdc As IntPtr = g.GetHdc
            VGDDScreenShot.PrintWindow(Me.Handle, hdc, Nothing)
            g.ReleaseHdc(hdc)
            g.Flush()
            g.Dispose()
            _ScreenShot.Save(Filename, Imaging.ImageFormat.Png)
        End Sub

        'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '    ' uncomment to test capturing various hidden forms:

        '    '1- hidden off screen -> works!
        '    'Me.Location = New Point(-500, -500)

        '    '2- Minimizes - works but just takes a capture of the small minimized form...
        '    'Me.WindowState = FormWindowState.Minimized

        '    '3- big form laid over the top
        '    otherForm.Size = New Size(4000, 4000)
        '    otherForm.Show()

        '    t.Interval = 1000
        '    t.Start()
        'End Sub



        'Private Sub t_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles t.Tick
        '    CaptureScreen()
        '    If IO.File.Exists("C:\\ScreenCapBlah.bmp") Then
        '        IO.File.Delete("C:\\ScreenCapBlah.bmp")
        '    End If
        '    screenCapture.Save("c:\\ScreenCapBlah.bmp")
        '    Me.Text = ""

        '    '1 - move the form back
        '    ' Me.Location = New Point(0, 0)

        '    '2- maximize again
        '    ' Me.WindowState = FormWindowState.Maximized

        '    '3 - hide otherForm
        '    otherForm.Hide()
        '    t.Stop()
        'End Sub
    End Class
#End If

End Namespace
