Imports System.Windows.Forms.Design
Imports System.ComponentModel
Imports VGDDCommon
Imports System.Windows.Forms
Imports System.Drawing
Imports VirtualFabUtils.Utils

Public Class frmUIEditPalette
    Public _wfes As IWindowsFormsEditorService
    Public context As ITypeDescriptorContext
    Private _Palette As VGDDPalette

    Public Sub New()
        InitializeComponent()
        Me.TopLevel = False
    End Sub

    Public Property Palette As VGDDPalette
        Get
            Return _Palette
        End Get
        Set(ByVal value As VGDDPalette)
            _Palette = value
            If _Palette IsNot Nothing Then
                lblPaletteName.Text = _Palette.Name
                btnOptimize.Enabled = _Palette.CanBeOptimisedForRGB565
                Dim oScheme As VGDDScheme = Common.SelectedScheme
                oScheme.Palette = value
                Common.ProjectChanged = True
                Common.ResourcesToConvert = True
            End If
        End Set
    End Property

    Private Sub frmUIEditPalette_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Debug.Print(Me.DialogResult)
    End Sub

    Private Sub frmUIEditPalette_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnPaletteLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPaletteLoad.Click
        Dim dlg As OpenFileDialog = New OpenFileDialog
        dlg.Title = "Load Palette"
        dlg.DefaultExt = "gpl"
        dlg.Filter = "GIMP Palette files|*.gpl"
        dlg.RestoreDirectory = True
        If (dlg.ShowDialog = DialogResult.OK) Then
            _Palette = New VGDDPalette
            _Palette.PaletteFile = dlg.FileName
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
            'Dim strPanelName = String.Format("pnlCustom{0:00}", i)
            'Dim pnlCustom As Panel = pnlPalette.Controls.Find(strPanelName, False)(0)
            'pnlCustom.BackColor = c

            _Palette.Name = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName)

            btnOptimize.Enabled = _Palette.CanBeOptimisedForRGB565
        End If
        'Me.DialogResult = Windows.Forms.DialogResult.OK
        'Me._wfes.CloseDropDown()
    End Sub

    Private Sub btnPaletteExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPaletteExport.Click
        Dim dlg As SaveFileDialog = New SaveFileDialog
        dlg.Title = "Export Palette"
        dlg.DefaultExt = "gpl"
        dlg.Filter = "GIMP Palette|*.gpl"
        dlg.FileName = _Palette.PaletteFile
        If (dlg.ShowDialog = DialogResult.OK) Then
            _Palette.PaletteFile = dlg.FileName
            Dim strPalette As String = String.Format("GIMP Palette" & vbCrLf & _
                "Name: [{0}]" & vbCrLf & _
                "Columns: {1}" & vbCrLf & _
                "#" & vbCrLf, _Palette.Name, _Palette.PaletteColours.Length)
            For i As Integer = 0 To _Palette.PaletteColours.Length - 1
                Dim c As Color = _Palette.PaletteColours(i)
                strPalette &= String.Format(" {0,3:##0} {1,3:##0} {2,3:##0}" & vbTab & "Index {3}", c.R, c.G, c.B, i) & vbCrLf
            Next
            'Dim doc As XmlDocument = New XmlDocument
            'Dim node As XmlNode = doc.AppendChild(doc.CreateElement("CustomColours"))
            'For i As Integer = 1 To 16
            '    Dim strPanelName = String.Format("pnlCustom{0:00}", i)
            '    Dim pnlCustom As Panel = TabPageCustomColor.Controls.Find(strPanelName, False)(0)
            '    Dim oNodeCustomColour As XmlNode = doc.CreateNode(XmlNodeType.Element, "", "CustomColour", "")
            '    node.AppendChild(oNodeCustomColour)
            '    Dim nameAttr As XmlAttribute
            '    nameAttr = doc.CreateAttribute("Value")
            '    Dim PanelColour As Color = pnlCustom.BackColor
            '    nameAttr.Value = String.Format("{0}, {1}, {2}", PanelColour.R, PanelColour.G, PanelColour.B)
            '    oNodeCustomColour.Attributes.Append(nameAttr)
            '    doc.DocumentElement.AppendChild(oNodeCustomColour)
            'Next
            'Dim sw As StringWriter
            'sw = New StringWriter
            'Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
            'xtw.Formatting = Formatting.Indented
            'doc.WriteTo(xtw)
            Try
                'Dim file As StreamWriter = New StreamWriter(strCustomColoursFile)
                'file.Write(sw.ToString)
                'file.Close()
                WriteFile(_Palette.PaletteFile, strPalette)

            Catch ex As Exception
                MessageBox.Show("Error writing to " & _Palette.PaletteFile & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub btnOptimize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOptimize.Click
        _Palette.OptimiseForRGB565()
        btnOptimize.Enabled = _Palette.CanBeOptimisedForRGB565
    End Sub

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me._wfes.CloseDropDown()
    End Sub

End Class