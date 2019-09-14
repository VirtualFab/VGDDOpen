Imports System.Windows.Forms
Imports VGDDCommon
Imports System.Drawing

Public Class frmLoadPalette

    Private _Palette As VGDDPalette

    Public Sub New(ByRef Palette As VGDDPalette)
        InitializeComponent()
        _Palette = Palette
    End Sub

    Private Sub btnLoadFromFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadFromFile.Click
        Dim dlg As OpenFileDialog = New OpenFileDialog
        dlg.Title = "Load Palette"
        dlg.DefaultExt = "gpl"
        dlg.Filter = "GIMP Palette files|*.gpl"
        dlg.RestoreDirectory = True
        If (dlg.ShowDialog = DialogResult.OK) Then
            _Palette.PaletteFile = dlg.FileName
            _Palette.Name = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName)
            'If Path.GetExtension(strCustomColoursFile) = "txt" Then
            Try
                _Palette.LoadFromGpl()
            Catch ex As Exception
                Windows.Forms.MessageBox.Show("Unable to load palette from file: " & ex.Message)
            End Try
            'Dim i As Integer = 0
            'Dim aFileRows() As String = Common.ReadFile(_Palette.PaletteFile).Split(vbLf)
            'ReDim _Palette.PaletteColours(aFileRows.Length - 1)
            'For Each strColorRow As String In aFileRows
            '    Dim c As Color = ColorTranslator.FromHtml(strColorRow)
            '    _Palette.PaletteColours(i) = c
            '    i += 1
            'Next
            'ReDim Preserve _Palette.PaletteColours(i - 1)
            'Else
            '    Dim doc As XmlDocument = New XmlDocument
            '    doc.Load(strCustomColoursFile)
            '    Dim oCustomColorNode As XmlNode = doc.DocumentElement
            '    Dim i As Integer = 1
            '    For Each node As XmlNode In doc.DocumentElement.ChildNodes
            '        If node.Name.Equals("CustomColour") Then
            '            Dim aRGB() As String = node.Attributes("Value").Value.ToString.Split(",")
            '            Dim c As Color = Color.FromArgb(aRGB(0), aRGB(1), aRGB(2))
            '            Dim strPanelName = String.Format("pnlCustom{0:00}", i)
            '            Dim pnlCustom As Panel = TabPageCustomColor.Controls.Find(strPanelName, False)(0)
            '            pnlCustom.BackColor = c
            '            _CustomColours(i - 1) = c
            '            i += 1
            '        End If
            '    Next
            'End If
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

End Class