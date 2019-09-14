Imports System.Windows.Forms

Public Class FootPrint

    Public Shared ROMCode As Integer
    Public Shared ROMStrings As Integer
    Public Shared RAM As Integer
    Public Shared Heap As Integer
    Public Shared BitmapsFlash As Integer
    Public Shared FontFlash As Integer
    Public Shared Widgets As New Collections.Generic.Dictionary(Of String, Integer)
    Public Shared ProjectWidgets As New Collections.Generic.Dictionary(Of String, Integer)

    Public Property Caption As String
        Get
            Return grpFootPrint.Text
        End Get
        Set(ByVal value As String)
            grpFootPrint.Text = value
        End Set
    End Property

    Public Shared Sub Clear()
        ROMCode = 0
        ROMStrings = 0
        RAM = 0
        Heap = 0
        BitmapsFlash = 0
        FontFlash = 0
        Widgets.Clear()
        ProjectWidgets.Clear()
    End Sub

    Public Sub UpdatePanel()
        lblROMCode.Text = ROMCode.ToString("###,##0")
        lblROMData.Text = ROMStrings.ToString("###,##0")
        lblROMBitmaps.Text = BitmapsFlash.ToString("###,##0")
        lblROMFonts.Text = FontFlash.ToString("###,##0")
        lblROMTotal.Text = (ROMCode + _
                            ROMStrings + _
                            BitmapsFlash + _
                            FontFlash).ToString("###,##0")
        lblRAM.Text = RAM.ToString("###,##0")
        lblHeap.Text = Heap.ToString("###,##0")
        Application.DoEvents()
    End Sub

End Class
