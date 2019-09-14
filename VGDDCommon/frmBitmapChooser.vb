Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports ImageMagick

Namespace VGDDCommon

    Public Class frmBitmapChooser

        Public ChosenBitmap As VGDDImage
        Private TempSelBitmap As VGDDImage
        Public FileName As String = ""
        Public LastBitmapPath As String = ""

        'Private Sub frmBitmapChooser_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        '    If Me.Opacity = 0 Then
        '        Me.Focus()
        '        FadeForm.FadeIn(Me, 99)
        '    End If
        'End Sub

        Private Sub frmBitmapChooser_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
#If Not PlayerMonolitico Then
            Try
                If Me.WindowState = FormWindowState.Normal Then
                    MyCommon.Settings.BmpChooserTop = Me.Top
                    MyCommon.Settings.BmpChooserLeft = Me.Left
                    MyCommon.Settings.BmpChooserHeight = Me.Height
                    MyCommon.Settings.BmpChooserWidth = Me.Width
                End If
                MyCommon.Settings.Save()
            Catch ex As Exception
            End Try
#End If
            'FadeForm.FadeOutAndWait(Me)
        End Sub

        Private Sub BitmapChooser_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'System.Diagnostics.Debugger.Break()
            '            If Common.DoFadings Then Me.Opacity = 0
#If Not PlayerMonolitico Then
            SetHelp()
            txtBinPath.Text = Common.BitmapsBinPath
            Try
                Me.WindowState = FormWindowState.Normal
                If MyCommon.Settings.BmpChooserWidth > 0 Then
                    Me.Top = MyCommon.Settings.BmpChooserTop
                    Me.Left = MyCommon.Settings.BmpChooserLeft
                    Me.Width = MyCommon.Settings.BmpChooserWidth
                    Me.Height = MyCommon.Settings.BmpChooserHeight
                End If
                RefreshList()
                For Each oItem As ListViewItem In ListView1.Items
                    Dim oVgddBitmap As VGDDImage = oItem.Tag
                    If oVgddBitmap.Name = Me.FileName Then
                        oItem.Selected = True
                        oItem.EnsureVisible()
                        oItem.Focused = True
                        Exit For
                    End If
                Next
                ListView1.Select()

            Catch ex As Exception
            End Try
#End If
        End Sub

#Region "Help"
        Private Sub SetHelp()
            If Common.HelpProvider IsNot Nothing Then
                Common.HelpProvider.SetHelpNavigator(Me, HelpNavigator.Topic)
                Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_BitmapChooser"
                Common.HelpProvider.SetHelpKeyword(Me, "Main")
                Common.HelpProvider.SetShowHelp(Me, True)

                Common.HelpProvider.SetHelpNavigator(Me.PropertyGrid1, HelpNavigator.Topic)
                Common.HelpProvider.SetShowHelp(Me.PropertyGrid1, True)
            End If
        End Sub

        Private Sub PropertyGrid1_HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs) Handles PropertyGrid1.HelpRequested
            Common.HelpProvider.SetHelpKeyword(PropertyGrid1, PropertyGrid1.SelectedGridItem.Label)
        End Sub
#End Region

        Private Sub RefreshList()
            Try
                pnlBinFiles.Visible = False
                With Me.ListView1
                    .Items.Clear()
                    .MultiSelect = True
                    .LargeImageList = New ImageList
                    .LargeImageList.ImageSize = New Drawing.Size(64, 64)
                    .LargeImageList.ColorDepth = ColorDepth.Depth32Bit
                    .View = View.LargeIcon
                    .HideSelection = False
                    .FullRowSelect = True
                    For i As Integer = 1 To 2
                        For Each oVgddBitmap As VGDDImage In Common._Bitmaps
                            Dim strGroup As String = String.Empty
                            If i = 1 Then
                                If oVgddBitmap.Referenced Then
                                    strGroup = oVgddBitmap.Type.ToString
                                End If
                            Else
                                If Not oVgddBitmap.Referenced Then
                                    strGroup = "NOT IN USE"
                                End If
                            End If
                            If strGroup = String.Empty Then Continue For
                            If .Groups(strGroup) Is Nothing Then
                                Dim strGroupType As String = strGroup
                                If strGroup <> "NOT IN USE" Then strGroupType = "Type: " & strGroupType
                                .Groups.Add(New ListViewGroup(strGroup, strGroupType))
                            End If
                            If oVgddBitmap.Bitmap IsNot Nothing Then
                                Try
                                    Dim offsetX As Integer, offsetY As Integer, scaledWidth As Integer, scaledHeight As Integer
                                    Dim scaleRatio As Single
                                    If oVgddBitmap.Bitmap.Width < .LargeImageList.ImageSize.Width And _
                                        oVgddBitmap.Bitmap.Height < .LargeImageList.ImageSize.Height Then
                                        scaledHeight = oVgddBitmap.Bitmap.Height
                                        scaledWidth = oVgddBitmap.Bitmap.Width
                                    ElseIf oVgddBitmap.Bitmap.Width > oVgddBitmap.Bitmap.Height Then
                                        scaleRatio = oVgddBitmap.Bitmap.Height / oVgddBitmap.Bitmap.Width
                                        scaledWidth = .LargeImageList.ImageSize.Width
                                        scaledHeight = .LargeImageList.ImageSize.Height * scaleRatio
                                    Else
                                        scaleRatio = oVgddBitmap.Bitmap.Width / oVgddBitmap.Bitmap.Height
                                        scaledHeight = .LargeImageList.ImageSize.Height
                                        scaledWidth = .LargeImageList.ImageSize.Width * scaleRatio
                                    End If
                                    offsetX = (.LargeImageList.ImageSize.Width - scaledWidth) / 2
                                    offsetY = (.LargeImageList.ImageSize.Height - scaledHeight) / 2
                                    'Using bmp As New Drawing.Bitmap(.LargeImageList.ImageSize.Width, .LargeImageList.ImageSize.Height)
                                    'Using g As Drawing.Graphics = Drawing.Graphics.FromImage(bmp)
                                    '    g.DrawImage(oVgddBitmap.Bitmap, offsetX, offsetY, scaledWidth, scaledHeight)
                                    '    .LargeImageList.Images.Add(oVgddBitmap.Name, bmp)
                                    'End Using
                                    ' Create empty image
                                    'Using image As New MagickImage(oVgddBitmap.Bitmap)
                                    '    Dim oG As New MagickGeometry(offsetX, offsetY, scaledWidth, scaledHeight)
                                    '    oG.IgnoreAspectRatio = True
                                    '    'image.Resize(.LargeImageList.ImageSize.Width, .LargeImageList.ImageSize.Height)
                                    '    image.Resize(oG)
                                    '    .LargeImageList.Images.Add(oVgddBitmap.Name, image.ToBitmap(Drawing.Imaging.ImageFormat.Bmp))
                                    'End Using
                                    Using image As New MagickImage(New MagickColor("White"), .LargeImageList.ImageSize.Width, .LargeImageList.ImageSize.Height)
                                        'image.Resize(.LargeImageList.ImageSize.Width, .LargeImageList.ImageSize.Height)
                                        Using scaledImage As New MagickImage(oVgddBitmap.Bitmap)
                                            scaledImage.Resize(scaledWidth, scaledHeight)
                                            image.Composite(scaledImage, offsetX, offsetY, CompositeOperator.Over)
                                            .LargeImageList.Images.Add(oVgddBitmap.Name, image.ToBitmap(Drawing.Imaging.ImageFormat.Bmp))
                                        End Using
                                    End Using
                                    'End Using
                                    '.LargeImageList.Images.Add(oVgddBitmap.Name, oVgddBitmap.Bitmap)
                                    Dim oItem As New ListViewItem(String.Format("{0} {1}x{2}", oVgddBitmap.Name, _
                                                                                oVgddBitmap.Bitmap.Width, oVgddBitmap.Bitmap.Height), _
                                                                                oVgddBitmap.Name, .Groups(strGroup))

                                    oItem.Tag = oVgddBitmap
                                    .Items.Add(oItem)
                                    If oVgddBitmap.FileName = Me.FileName Then
                                        TempSelBitmap = oVgddBitmap
                                    End If
                                    'oItem.SubItems.Add(oVgddBitmap.Type.ToString)
                                    'oItem.SubItems.Add(oVgddBitmap.SDFileName)
                                    'oItem.ImageKey = oVgddBitmap.Name
                                Catch ex As Exception

                                End Try

                                If oVgddBitmap.Type = VGDDImage.PictureType.BINBMP_ON_SDFAT Or oVgddBitmap.Type = VGDDImage.PictureType.EXTERNAL Then
                                    pnlBinFiles.Visible = True
                                End If
                            End If
                        Next
                        .Sorting = SortOrder.Ascending
                        .Sort()
                    Next
                End With

            Catch ex As Exception

            End Try
        End Sub

        Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
            If ListView1.SelectedIndices.Count > 0 Then
                Me.ChosenBitmap = ListView1.Items((ListView1.SelectedIndices(0))).Tag
                'Me.ChosenBitmap._Referenced = True
                Me.Close()
            End If
        End Sub

        Private Sub ListView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
            Try
                If ListView1.SelectedIndices.Count > 0 Then
                    btnRemove.Enabled = True
                    If ListView1.SelectedIndices.Count = 1 Then
                        TempSelBitmap = ListView1.Items((ListView1.SelectedIndices(0))).Tag
                        PropertyGrid1.SelectedObject = TempSelBitmap
                    Else
                        Dim TempSelObjs(ListView1.SelectedIndices.Count - 1) As Object
                        Dim i As Integer = 0
                        For Each oItem As ListViewItem In ListView1.SelectedItems
                            TempSelObjs(i) = oItem.Tag
                            i += 1
                        Next
                        PropertyGrid1.SelectedObjects = TempSelObjs
                        TempSelBitmap = Nothing
                        'btnRemove.Enabled = False
                    End If
                Else
                    PropertyGrid1.SelectedObject = Nothing
                    TempSelBitmap = Nothing
                    btnRemove.Enabled = False
                End If

            Catch ex As Exception

            End Try
            CheckVisibleButtons()
        End Sub

        Private Sub btnNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNew.Click
            Dim fileDialog As New OpenFileDialog()
            With fileDialog
                .FileName = FileName
                'If Common.ProjectUseGRC Then
                '    .Filter = "Bitmap files|*.bmp|All Files|*.*"
                'Else
                .Filter = "Graphics files|*.bmp; *.jpg; *.png; *.gif|All Files|*.*"
                'End If
                .RestoreDirectory = False
                .Multiselect = True
                If LastBitmapPath <> "" Then
                    .InitialDirectory = LastBitmapPath
                End If
            End With
            If fileDialog.ShowDialog() = DialogResult.OK Then
                For Each strFileNameDialog As String In fileDialog.FileNames
                    Dim strFileName As String = Path.GetFileName(strFileNameDialog)
                    Dim strFileNameClean As String = Common.CleanName(Path.GetFileNameWithoutExtension(strFileName))
                    If Common.Bitmaps.Contains(strFileNameClean) Then
                        Dim i = 1
                        Do While Common.Bitmaps.Contains(strFileNameClean & i)
                            i += 1
                        Loop
                        strFileNameClean &= i
                        'MessageBox.Show(String.Format("A bitmap with name {0} already exists in current project. Please remove the old bitmap or externally rename the new one.", strFileNameClean), "Bitmap already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        'Exit Sub
                    End If
                    If Common.ProjectCopyBitmapsInVgddProjectFolder Then
                        Static blnCopyBitmapsWarning As Boolean
                        If Common.VGDDProjectPath = "" Then
                            If Not blnCopyBitmapsWarning Then
                                blnCopyBitmapsWarning = True
                                MessageBox.Show("Option ""Copy Bitmap to project folder"" is set but the project has not been saved yet." & vbCrLf & "If you want this option to work as expected, save project prior to start adding bitmaps.", "Project folder not yet defined", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            End If
                        Else
                            Dim strNewFileName As String = Path.Combine(Common.VGDDProjectPath, Path.GetFileName(strFileNameDialog))
                            If strNewFileName <> strFileNameDialog Then
                                If File.Exists(strNewFileName) AndAlso Not Common.FilesAreTheSame(strNewFileName, strFileNameDialog) Then
                                    If MessageBox.Show("File " & Path.GetFileName(strFileNameDialog) & " already exists in VGDD Project Path. Overwrite?", "Overwrite confirmation", MessageBoxButtons.OKCancel) = vbCancel Then
                                        Exit Sub
                                    End If
                                End If
                                Try
                                    File.Copy(strFileNameDialog, strNewFileName, True)
                                Catch ex As Exception
                                    MessageBox.Show("Cannot copy file " & Path.GetFileName(strFileNameDialog) & " to VGDD Project Path:" & vbCrLf & ex.Message, "Copy error")
                                    Exit Sub
                                End Try
                                strFileNameDialog = strNewFileName
                            End If
                        End If
                    End If
                    LastBitmapPath = Path.GetDirectoryName(strFileNameDialog)
                    'If Path.GetFileNameWithoutExtension(strFileNameOk).Length > 8 Then
                    '    strFileNameOk = strFileNameOk.Substring(0, 8) & Path.GetExtension(strFileName)
                    'End If
                    'strFileNameOk &= Path.GetExtension(strFileName)
                    'If strFileName <> strFileNameOk Then
                    '    If MessageBox.Show(String.Format("Filename ""{0}"" isn't suitable for VGDD naming conventions. Copy it to ""{1}""?", strFileName, strFileNameOk), "Rename Picture file", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then
                    '        strFileNameOk = Path.Combine(Path.GetDirectoryName(strFileNameDialog), strFileNameOk)
                    '        If File.Exists(strFileNameOk) Then
                    '            Try
                    '                File.Delete(strFileNameOk)
                    '            Catch ex As Exception
                    '                MessageBox.Show("Can't delete " & strFileNameOk & ". Please manually rename it before using it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    '                Exit Sub
                    '            End Try
                    '        End If
                    '        Try
                    '            File.Copy(strFileNameDialog, strFileNameOk)
                    '        Catch ex As Exception
                    '            MessageBox.Show("Cannot copy file " & Path.GetFileName(strFileNameDialog) & " to new name " & Path.GetFileName(strFileNameOk) & vbCrLf & ex.Message, "Copy error")
                    '            Exit Sub
                    '        End Try
                    '    Else
                    '        strFileNameOk = strFileNameDialog
                    '    End If
                    'Else
                    strFileName = Path.Combine(Path.GetDirectoryName(strFileNameDialog), strFileName)
                    'End If
                    Common.AddBitmap(strFileNameClean, strFileName, Nothing)
                    'End If
                    RefreshList()
                    For Each oItem As ListViewItem In ListView1.Items
                        If CType(oItem.Tag, VGDDImage).FileName = strFileName Then
                            oItem.Selected = True
                            oItem.EnsureVisible()
                            Exit For
                        End If
                    Next
                Next
                fileDialog.Dispose()
            End If
        End Sub

        Private Sub CheckVisibleButtons()
            btnExtractPalette.Visible = False
            If TempSelBitmap IsNot Nothing AndAlso TempSelBitmap.AllowScaling = False AndAlso (TempSelBitmap.ColourDepth = 4 Or TempSelBitmap.ColourDepth = 8) Then
                btnExtractPalette.Visible = True
            End If
        End Sub

        Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
            For Each oSelItem As ListViewItem In ListView1.SelectedItems
                Common.RemoveBitmap(CType(oSelItem.Tag, VGDDImage).Name)
            Next
            Common.ProjectChanged = True
            RefreshList()
        End Sub

#If Not PlayerMonolitico Then

        Private Sub PropertyGrid1_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
            Select Case e.ChangedItem.Label
                Case "AllowScaling"
                    If e.ChangedItem.Value = False Then
                        For Each oVGDDImage As VGDDImage In PropertyGrid1.SelectedObjects
                            Try
                                oVGDDImage.TransparentBitmap = oVGDDImage._OrigBitmap
                                oVGDDImage.Size = oVGDDImage._OrigBitmap.Size
                            Catch ex As Exception
                            End Try
                        Next
                    End If
                    btnSaveScaledImage.Visible = e.ChangedItem.Value
                Case "Type"
                    For Each oVGDDImage As VGDDImage In PropertyGrid1.SelectedObjects
                        If oVGDDImage.Type = VGDDImage.PictureType.BINBMP_ON_SDFAT Then
                            oVGDDImage.CheckSDFilename()
                        End If
                    Next
            End Select

            Select Case e.ChangedItem.Label
                Case "Type", "ColourDepth", "AllowScaling", "Size", "ForceInclude", "InterpolationMode"
                    'If e.ChangedItem.Value.ToString.Contains("BINBMP") Then
                    '    Dim binFileName As String = Path.Combine(txtBinPath.Text, Path.GetFileNameWithoutExtension(TempSelBitmap.FileName)) & ".bin"
                    '    BmpConvert.BitmapToBinFile(TempSelBitmap, binFileName)
                    'End If
                    PropertyGrid1.Refresh()
                    RefreshList()
            End Select
            Common.ProjectChanged = True
            CheckVisibleButtons()
        End Sub

#End If

        Private Sub btnChooseBinPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChooseBinPath.Click
            Dim oDlg As New FolderBrowserDialog
            With oDlg
                If txtBinPath.Text <> "" Then
                    .SelectedPath = txtBinPath.Text
                    SendKeys.Send("{TAB}{TAB}{RIGHT}") 'Trick to focus current folder
                End If
                .Description = "Select destination folder for .bin bitmap files"
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Common.BitmapsBinPath = .SelectedPath
                    txtBinPath.Text = .SelectedPath
                End If
            End With
        End Sub

        Private Sub ColorPicker()
            Dim BMP As New Drawing.Bitmap(1, 1)
            Dim GFX As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(BMP)
            GFX.CopyFromScreen(New Drawing.Point(MousePosition.X, MousePosition.Y), New Drawing.Point(0, 0), BMP.Size)
            Dim Pixel As Drawing.Color = BMP.GetPixel(0, 0)
            'CPpanel.BackColor = Pixel
            'Redtxt.Text = Pixel.R
            'Greentxt.Text = Pixel.G
            'Bluetxt.Text = Pixel.B

        End Sub

        Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
            For Each oVGDDImage As VGDDImage In PropertyGrid1.SelectedObjects
                oVGDDImage.RefreshBitmap()
            Next
        End Sub

        Private Sub btnExtractPalette_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExtractPalette.Click
            'If ListView1.SelectedIndices.Count = 0 Then Exit Sub
            'TempSelBitmap = ListView1.Items((ListView1.SelectedIndices(0))).Tag
            If TempSelBitmap Is Nothing Then Exit Sub
            Dim aColors(TempSelBitmap.OrigBitmap.Palette.Entries.Length - 1) As Color
            Dim i As Integer = 0
            For Each c As Color In TempSelBitmap.OrigBitmap.Palette.Entries
                aColors(i) = c
                i += 1
            Next

            Dim oPalette As New VGDDPalette
            With oPalette
                .Name = TempSelBitmap.Name '& "_palette"
                .PaletteColours = aColors
            End With

            Dim dlg As New SaveFileDialog
            dlg.Title = "Save bitmap's Palette"
            dlg.DefaultExt = "gpl"
            dlg.Filter = "GIMP Palette files|*.gpl"
            dlg.FileName = oPalette.Name & ".gpl"
            If (dlg.ShowDialog = DialogResult.OK) Then
                Try
                    oPalette.SaveAsGpl(dlg.FileName)
                Catch ex As Exception
                    Windows.Forms.MessageBox.Show("Unable to extract palette from bitmap: " & ex.Message)
                End Try
            End If
        End Sub

        Private Sub btnSaveScaledImage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveScaledImage.Click
            Dim oVgddImage As VGDDImage
            Try
                oVgddImage = PropertyGrid1.SelectedObject
            Catch ex As Exception
                Exit Sub
            End Try
            If oVgddImage Is Nothing Then Exit Sub
            Dim dlg As New SaveFileDialog
            dlg.Title = "Save scaled/transformed bitmap"
            dlg.Filter = "Microsoft Bitmap|*.bmp"
            dlg.FileName = oVgddImage.Name & ".bmp"
            dlg.DefaultExt = "bmp"
            dlg.AddExtension = True
            If Not dlg.ShowDialog = DialogResult.OK Then
                Exit Sub
            End If
            Dim strRet As String = oVgddImage.Save(dlg.FileName, Drawing.Imaging.ImageFormat.Bmp)
            If strRet <> "OK" Then
                MessageBox.Show("Error saving " & dlg.FileName & ":" & vbCrLf & strRet)
            End If
        End Sub
    End Class

End Namespace
