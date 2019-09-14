Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Windows.Forms

Namespace VGDDCommon

    Public Class frmFontChooser
        Private WithEvents oCustomFontDialog As FontDialog

        'f.AddFontToRecentList(FontFamily.GenericSansSerif);
        'f.AddFontToRecentList(FontFamily.GenericSerif);
        Private _SelectedFont As VGDDFont
        Private _OldSelectedFontName As String

        Public Property SelectedFont As VGDDFont
            Get
                Return _SelectedFont
            End Get
            Set(ByVal value As VGDDFont)
                'If _SelectedFont IsNot Nothing Then _OldSelectedFontName = _SelectedFont.Name
                _SelectedFont = value
                PropertyGrid1.SelectedObject = _SelectedFont
            End Set
        End Property

        Private Sub frmFontChooser_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
            Try
                'If Me.Opacity = 0 Then
                '    'FadeForm.FadeIn(Me, 99)
                'End If
                If _SelectedFont Is Nothing AndAlso Common._Fonts.Count > 0 Then _SelectedFont = Common._Fonts(0)
                _OldSelectedFontName = _SelectedFont.Name
            Catch ex As Exception
            End Try
        End Sub

        'Private Sub frmFontChooser_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '    FadeForm.FadeOutAndWait(Me)
        'End Sub

        Private Sub frmFontChooser_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Common.DoFadings Then Me.Opacity = 0
            SetHelp()
            txtBinPath.Text = Common.BitmapsBinPath
            PropertyGrid1.SelectedObject = Me.SelectedFont
            UpdateFonts()
            RefreshSample()
        End Sub

#Region "Help"
        Private Sub SetHelp()
            Common.HelpProvider.SetHelpNavigator(Me, HelpNavigator.Topic)
            Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_FontChooser"
            Common.HelpProvider.SetHelpKeyword(Me, "Main")
            Common.HelpProvider.SetShowHelp(Me, True)

            Common.HelpProvider.SetHelpNavigator(Me.PropertyGrid1, HelpNavigator.Topic)
            Common.HelpProvider.SetShowHelp(Me.PropertyGrid1, True)
        End Sub

        Private Sub PropertyGrid1_HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs) Handles PropertyGrid1.HelpRequested
            Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_FontChooser"
            Common.HelpProvider.SetHelpKeyword(PropertyGrid1, PropertyGrid1.SelectedGridItem.Label)
        End Sub
#End Region

        Private Sub PropertyGrid1_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
            Me.SelectedFont = PropertyGrid1.SelectedObject
            Select e.ChangedItem.Label
                Case "Charset"
                    If e.ChangedItem.Value = VGDDFont.FontCharset.SELECTION AndAlso Not Me.SelectedFont.SmartCharSet Then
                        For Each obj As Object In Me.SelectedFont.UsedBy
                            If obj.GetType Is GetType(VGDDScheme) Then
                                Dim oScheme As VGDDScheme = obj
                                For Each oControl As Control In oScheme.UsedBy
                                    Me.SelectedFont.SmartCharSetAddString(oControl.Text)
                                Next
                            Else
                                Me.SelectedFont.SmartCharSetAddString(obj.Text)
                            End If
                        Next
                        'If MessageBox.Show("Enable the SmartCharSet feature?", "VGDD feature", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                        '    Me.SelectedFont.SmartCharSet = True
                        'End If
                    ElseIf e.ChangedItem.Value = VGDDFont.FontCharset.RANGE Then
                        If Me.SelectedFont.StartChar = -1 Then
                            Me.SelectedFont.StartChar = 32
                            Me.SelectedFont.EndChar = 126
                        End If
                    End If
                    'Case "Font"
                    'Common._Fonts.Remove(_SelectedFont)
                    'Common._Fonts.Add(_SelectedFont)
                    '_OldSelectedFontName = _SelectedFont.Name
                Case "IsGOLFontDefault"
                    If e.ChangedItem.Value = True Then
                        For Each oVfont As VGDDFont In Common._Fonts
                            If oVfont.Name <> Me.SelectedFont.Name AndAlso oVfont.IsGOLFontDefault Then
                                oVfont.IsGOLFontDefault = False
                            End If
                        Next
                    End If
            End Select
            Common.ProjectChanged = True
            UpdateFonts()
            RefreshSample()
        End Sub

        Private Sub UpdateFonts()
            ListView1.Items.Clear()
            ImageList1.Dispose()
            ImageList1 = New ImageList
            ListView1.LargeImageList = ImageList1
            ListView1.SmallImageList = ImageList1
            pnlBinPath.Visible = False
            Dim intMaxH As Integer = 30
            For Each VFont As VGDDFont In Common._Fonts
                If VFont.Font.Size * 1.4 > intMaxH Then
                    intMaxH = VFont.Font.Size * 1.4
                    If intMaxH > 256 Then intMaxH = 256
                End If
            Next
            ImageList1.ImageSize = New Size(256, intMaxH)
            For Each VFont As VGDDFont In Common._Fonts
                Dim bm As New Bitmap(256, intMaxH, Imaging.PixelFormat.Format32bppPArgb)
                Dim g As Graphics = Graphics.FromImage(bm)
                'g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel
                g.Clear(Color.Yellow)
                Dim sf As New Drawing.StringFormat
                sf.LineAlignment = StringAlignment.Center
                sf.Alignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoWrap
                Try
                    g.DrawString(VFont.Name, VFont.Font, New SolidBrush(Color.Blue), New RectangleF(0, 0, bm.Width, bm.Height), sf)
                Catch ex As Exception
                End Try
                ImageList1.Images.Add(VFont.Name, bm)
                Dim lvi As New ListViewItem
                'lvi.Text = VFont.Name
                lvi.Text = VFont.Name
                lvi.ImageKey = VFont.Name
                lvi.Tag = VFont
                ListView1.Items.Add(lvi)
                If VFont Is Me.SelectedFont Then
                    lvi.Selected = True
                End If
                If VFont.Type = VGDDFont.FontType.EXTERNAL Then
                    pnlBinPath.Visible = True
                End If
            Next
            ListView1.ArrangeIcons(ListViewAlignment.Top)
            Dim p As New Padding(0, 0, 0, 0)
            ListView1.Margin = p
            ListView1.View = View.LargeIcon
        End Sub

        Private Sub btnChoosBinPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChoosBinPath.Click
            Dim oDlg As New FolderBrowserDialog
            With oDlg
                .RootFolder = Environment.SpecialFolder.MyComputer
                .SelectedPath = txtBinPath.Text
                SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Common.BitmapsBinPath = .SelectedPath
                    txtBinPath.Text = .SelectedPath
                End If
            End With
        End Sub

        'Private Sub btnConvert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConvert.Click
        '    Dim swOut As New StreamWriter(Path.Combine(txtBinPath.Text, Me.SelectedFont.Name & ".c"), False)
        '    Dim strBmpCodeTemplate As String = CodeGen.GetTemplate(String.Format("VGDDCodeTemplate/ProjectTemplates/FontsDeclare/{0}/Code", "FLASH_VGDD"))
        '    swOut.Write(strBmpCodeTemplate _
        '            .Replace("[HEX_LINES]", BmpConvert.FontToHex(Me.SelectedFont)) _
        '            .Replace("[FONT]", Me.SelectedFont.Name))
        '    swOut.Flush()
        '    swOut.Close()

        '    Dim strFileBin As String = Path.Combine(txtBinPath.Text, Me.SelectedFont.Name) & ".bin"
        '    BmpConvert.FontToBinFile(Me.SelectedFont, strFileBin)
        'End Sub

        'Private Sub FontDialog1_Apply(ByVal sender As Object, ByVal e As System.EventArgs)
        '    With FontDialog1
        '        Me.SelectedFont.Font = .Font
        '        UpdateFonts()
        '    End With
        'End Sub

        Private Sub ListView1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDoubleClick
            If ListView1.SelectedItems.Count > 0 Then
                Me.SelectedFont = ListView1.SelectedItems(0).Tag
                Me.Close()
            End If
        End Sub

        Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
            If ListView1.SelectedItems.Count <= 0 Then
                btnRemove.Enabled = False
                PictureBox1.Image = Nothing
            Else
                btnRemove.Enabled = True
                _SelectedFont = ListView1.SelectedItems(0).Tag
                _OldSelectedFontName = _SelectedFont.Name
                Dim aSelectedObject(ListView1.SelectedItems.Count - 1) As Object
                For i As Integer = 0 To ListView1.SelectedItems.Count - 1
                    aSelectedObject(i) = ListView1.SelectedItems(i).Tag
                Next
                PropertyGrid1.SelectedObjects = aSelectedObject 'SelectedFont
                RefreshSample()
            End If
        End Sub

        Private Sub RefreshSample()
            If Me.SelectedFont IsNot Nothing AndAlso Me.SelectedFont.Font IsNot Nothing Then
                Try
                    Dim bitmap As New Bitmap(PictureBox1.Width, PictureBox1.Height, PixelFormat.Format16bppRgb565)
                    Dim g As Graphics = Graphics.FromImage(bitmap)
                    g.FillRectangle(New SolidBrush(Drawing.Color.White), New Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height))
                    'g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
                    Dim textFormat As New StringFormat()
                    textFormat.Alignment = StringAlignment.Near
                    textFormat.LineAlignment = StringAlignment.Near
                    g.DrawString("Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat", SelectedFont.Font, New SolidBrush(Color.Black), New Point(0, 0), textFormat) ' New RectangleF(0, 0, bs.Width, bs.Height), textFormat)
                    PictureBox1.Image = bitmap
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
            Try
                oCustomFontDialog = New FontDialog
                With oCustomFontDialog
                    .SelectedFont = Me.Font
                    If SelectedFont IsNot Nothing Then
                        Try
                            .SelectedFont = SelectedFont.Font
                        Catch ex As Exception
                            .SelectedFont = Me.Font
                        End Try
                    End If
                    '.AllowSimulations = True
                    '.ShowApply = True
                    '.ShowEffects = True
                    '.AllowVerticalFonts = True
                    '.AllowVectorFonts = True
                    '.AllowScriptChange = True
                    '.FixedPitchOnly = False
                    '.FontMustExist = False
                    '.ScriptsOnly = False

                    If .ShowDialog() Then
                        Dim NewFont As New VGDDFont(.SelectedFont)
                        Common.AddFont(NewFont, Nothing)
                        UpdateFonts()
                        Dim oItem As ListViewItem = ListView1.Items(ListView1.Items.Count - 1)
                        oItem.Selected = True
                        oItem.EnsureVisible()
                    End If
                End With
            Catch ex As Exception
                MessageBox.Show(ex.Message, "New Font error")
            End Try
        End Sub

        Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.Click
            If SelectedFont IsNot Nothing Then
                Common.RemoveFont(SelectedFont)
                ListView1.SelectedIndices.Clear()
                UpdateFonts()
            End If
        End Sub

        Private Sub frmFontChooser_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
            RefreshSample()
        End Sub
    End Class

End Namespace
