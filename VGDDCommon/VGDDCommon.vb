Imports System.Drawing
Imports System.IO
Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Reflection
Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports VirtualFabUtils.Utils

Namespace VGDDCommon
#If PlayerMonolitico Then
    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    Partial Public Class Common
#Else
    Partial Public Class Common
#End If
        Public Shared VGDDVERSION As String = "11.3.0"
        Public Const ASSEMBLIES_VERSION As String = "11.3.*"

#Region "Public Interfaces"

        Public Interface IVGDDBase
            Property Zorder As Integer
            Sub GetCode(ByVal ControlIdPrefix As String)
            Property Text As String
        End Interface

        Public Interface IVGDDEvents
            Property VGDDEvents As VGDDEventsCollection
            Property Name As String
        End Interface

        Public Interface IVGDDWidget : Inherits IVGDDBase, IVGDDEvents
            Property SchemeObj As VGDDScheme
            Property Scheme As String
            ReadOnly Property Instances As Integer
            ReadOnly Property HasChildWidgets As Boolean
            ReadOnly Property DemoLimit As Integer
        End Interface

        Public Interface IVGDDWidgetWithBitmap : Inherits IVGDDWidget
            Property BitmapName As String
            Sub SetBitmap(ByVal bmp As Bitmap)
            Sub ScaleBitmap()
            ReadOnly Property VGDDImage() As VGDDImage
        End Interface

        Public Interface IVGDDXmlSerialize
            Function FromXml(ByVal oNode As XmlNode) As Object
            Sub ToXml(ByRef XmlParent As XmlNode)
        End Interface


#End Region

        Public Const URL_PURCHASE_MICROCHIPDIRECT As String = "http://www.microchipdirect.com/ProductSearch.aspx?Keywords=SW500190"
        'Public Const URL_PURCHASE_MICROCHIPDIRECT As String = "http://www.microchipdirect.com/searchparts.aspx?q=vgdd"

        Public Shared VGDDIsRunning As Boolean = False
        Public Shared dtActions As DataTable
        'Public Shared _Screens As New ArrayList
        Public Shared _Schemes As New Dictionary(Of String, VGDDScheme) ' Specialized.OrderedDictionary 'ArrayList '
        Public Shared _Bitmaps As New Collection
        Public Shared _Fonts As New ArrayList ' System.Collections.Generic.Dictionary(Of String, VGDDFont)
        Public Shared BitmapsBinPath As String
        Public Shared BitmapsBinUsed As Integer
        'Public Shared CDialogCustomColours() As Color
        Public Shared ProjectWidth As Integer
        Public Shared ProjectHeight As Integer
        Public Shared ProjectColourDepth As Integer
        Public Shared ProjectUsePalette As Boolean
        Public Shared ProjectUseMultiByteChars As Boolean
        Public Shared ProjectMultiLanguageTranslations As Integer
        Public Shared ProjectStringsPoolGenerateHeader As Boolean
        Public Shared ProjectActiveLanguage As Integer
        Public Shared ProjectStringPool As New System.Collections.Generic.Dictionary(Of Integer, MultiLanguageStringSet)
        Public Shared ProjectUseGradient As Boolean = False
        Public Shared ProjectUseAlphaBlend As Boolean = False
        Public Shared ProjectCompiler As String = "C30"
        Public Shared ProjectCompilerFamily As String = "C30"
        Public Shared ProjectPathGRC As String
        Public Shared ProjectJavaCommand As String
        Public Shared ProjectFallbackGRCPath As String
        Public Shared _ProjectPath As String = Directory.GetCurrentDirectory
        Public Shared _ProjectFileName As String
        Public Shared _ProjectName As String = "New"
        Public Shared ProjectPlayerBgBitmapName As String
        Public Shared ProjectPlayerBgColour As Color
        Public Shared ProjectCopyBitmapsInVgddProjectFolder As Boolean = True
        Public Shared ProjectUseBmpPrefix As Boolean = True
        Public Shared ProjectMakeBackups As Boolean = True
        Public Shared ProjectLoading As Boolean
        Public Shared ScreenLoading As Boolean
        Public Shared ProjectSaving As Boolean
        Public Shared ProjectGraphicsConfigFileName As String
        Public Shared ProjectHardwareProfileFileName As String
        Public Shared ProjectHeapSize As String
        Public Shared ProjectLastComputedHeapSize As Integer
        Public Shared ProjectHtmlWebPagesFolder As String
        Public Shared ProjectHtmlOutputType As String
        Public Shared ProjectHtmlTargetUrl As String
        Public Shared ProjectHtmlTargetUser As String
        Public Shared ProjectHtmlTargetPassword As String
        Public Shared ProjectVGDDVersion As String
        Public Shared ProjectVGDDIsLicensed As Boolean

        Public Shared CurrentScreen As VGDD.VGDDScreen

        Public Shared aScreens As New VGDD.VGDDScreenCollection

        Public Shared AnimationEnable As Boolean = True

        Public Shared ProjectFileName_ScreensC As String
        Public Shared ProjectFileName_ScreensH As String
        Public Shared ProjectFileName_ScreenStatesH As String
        Public Shared ProjectFileName_EventsHelperH As String
        Public Shared ProjectFileName_InternalResourcesC As String
        Public Shared ProjectFileName_InternalResourcesRefC As String
        Public Shared ProjectFileName_ExternalResourcesC As String
        Public Shared ProjectFileName_BmpOnSdResourcesC As String
        Public Shared ProjectFileName_BmpOnSdResourcesRefC As String
        Public Shared ProjectFileName_InternalResourcesH As String
        Public Shared ProjectFileName_ExternalResourcesRefC As String
        Public Shared ExternalResourcesHexFileName As String
        Public Shared ProjectFileName_ExternalResourcesH As String
        Public Shared ProjectFileName_BmpOnSdResourcesH As String
        Public Shared ProjectFileName_StringsPoolC As String
        Public Shared StringsPoolSortColumn As String
        Public Shared StringsPoolMaxStringID As Integer
        Public Shared ProjectFileName_StringsPoolH As String

        Public Shared QuickCodeGen As Boolean = False

#If Not PlayerMonolitico Then
        Public Shared oGrcProjectInternal As GrcProject
        Public Shared oGrcProjectExternal As GrcProject
        Public Shared oGrcBinBmpOnSd As GrcProject
#End If
        Public Shared MplabXProjectXmlPathName As String
        Public Shared _MplabxProjectPath As String = ""
        Public Shared _CodeGenLocation As Integer = 2
        Public Shared _CodeGenDestPath As String
        Private Shared _ProjectChanged As Boolean
        Public Shared CodeHasBeenGenerated As Boolean
        Public Shared _MplabXSelectedConfig As String = String.Empty

        Public Shared DevelopmentBoardID As String = ""
        Public Shared PIMBoardID As String = ""
        Public Shared ExpansionBoardID As String = ""
        Public Shared DisplayBoardID As String = ""
        Public Shared DisplayBoardOrientation As Integer = 0

        Public Shared CodegenProgress As Integer
        Public Shared CodegenGeneratingCode As Boolean
        Public Shared CodeGenWarnEmptyBitmaps As Boolean = True

        Public Shared PlayerIsActive As Boolean = False

        Public Shared MplabXTemplatesFolder As String
        Public Shared UserTemplatesFolder As String
        Public Shared MplabXTemplateFile As String
        Public Shared XmlMplabxTemplatesDoc As XmlDocument = Nothing
        Private Shared _BuildDateTime As DateTime = #1/1/1980#
        Public Shared HelpProvider As HelpProvider
        Public Const HELPNAMESPACEBASE = "http://virtualfab.it/mediawiki/index.php/VGDD:ContextHelp"

        Public Shared IPU_MAX_COMPRESSED_BUFFER_SIZE As Integer
        Public Shared IPU_MAX_DECOMPRESSED_BUFFER_SIZE As Integer

        Public Shared WizardOptions As New Collections.Specialized.NameValueCollection

        Public Shared DoFadings As Boolean = False

        Public Shared ResourcesToConvert As Boolean

        Public Shared OsVersion As String
        Public Shared OsServicePack As String
        Public Shared OsPlatform As String

        Public Shared InsertDebugInfo As Boolean = False

        Public Shared DesignScheme As VGDDScheme = Nothing

        Public Shared oEventsFontNormal As Font, oEventsFontBold As Font

        Public Shared CodeGenLocationOptionsOk As Boolean

        Public Shared HtmlEditorSplitterDistance As Integer

        Public Shared WizardWarnings As String

        Public Shared WizardForceAddVgddFiles As Boolean = False

        Public Shared IsLicensed As Boolean = False
        Public Const DEMOCODELIMIT = 4

#If Not PlayerMonolitico Then
        Public Shared oSelectedFramework As VGDDCommon.VGDDFramework
#End If

        Public Shared ProjectTouchAllScreens As Boolean

        Public Shared MplabXIpcIpAddress As String
        Public Shared MplabXIpcPort As Integer

        Public Shared ProjectUseGol As Boolean

        Public Shared ControlAdding As Boolean
        Public Shared ControlCopying As Boolean

        Public Shared oMplabXIpcDebug As MplabXIpcDebug

        Public Shared DefaultSchemeXML As String
        Public Shared SelectedScheme As VGDDScheme

        Public Shared Function ColorToXml(ByRef XmlDoc As XmlDocument, ByRef ColorName As String, ByRef Color As Color) As XmlNode
            Dim oNodeCol As XmlElement = XmlDoc.CreateElement(ColorName)
            oNodeCol.InnerText = String.Format("{0},{1},{2}", Color.R, Color.G, Color.B)
            Return oNodeCol
        End Function

        Public Shared Function ColorFromHexString(ByVal strHexColor As String) As Color
            Try
                Return Color.FromArgb(Integer.Parse(strHexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), _
                                      Integer.Parse(strHexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), _
                                      Integer.Parse(strHexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber))
            Catch ex As Exception

            End Try
            Return Color.White
        End Function

        Public Shared Function ColorToHexString(ByVal inColor As Color) As String
            Dim strArgbColor As String = inColor.ToArgb.ToString("X")
            Return strArgbColor.Substring(2, 6)
        End Function


        'Public Shared Sub NotAvailableInDemoMode(g As Graphics)
        '    Static oFont As New Font("MicrosoftSansSerif", 6)
        '    g.DrawLine(Pens.Red, 0, 0, g.VisibleClipBounds.Width - 1, g.VisibleClipBounds.Height - 1)
        '    g.DrawLine(Pens.Red, g.VisibleClipBounds.Width - 1, 0, 0, g.VisibleClipBounds.Height - 1)
        '    g.DrawString("Not available in demo mode", oFont, New SolidBrush(Color.Black), 2, 2)
        'End Sub

        Public Shared Function ShowOpenFileDialogEx(ByVal Owner As Form, ByVal OpenFileDialog1 As OpenFileDialog) As String
            ' Gets an Input File Name from the user, works with multi-monitors
            Dim PositionForm As New Form
            Dim MyInputFile As String

            ' The FileDialog() opens in the last Form that was created.  It's buggy!  To ensure it appears in the
            ' area of the current Form, we create a new hidden PositionForm and then delete it afterwards.

            PositionForm.StartPosition = FormStartPosition.Manual
            PositionForm.Left = Owner.Left + CInt(Owner.Width / 2)
            PositionForm.Top = Owner.Top + CInt(Owner.Height / 2)
            PositionForm.Width = 0
            PositionForm.Height = 0
            PositionForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            PositionForm.Visible = False
            PositionForm.Show()

            ' Added the statement "ShowHelp = True" to workaround a problem on W8.1 machines with SkyDrive installed.
            ' It causes the "old" W7 control to be used that does not point to SkyDrive in error.
            'OpenFileDialog1.FilterIndex = 1
            'OpenFileDialog1.RestoreDirectory = True
            'OpenFileDialog1.AutoUpgradeEnabled = False
            OpenFileDialog1.ShowHelp = True
            OpenFileDialog1.SupportMultiDottedExtensions = False

            If OpenFileDialog1.ShowDialog(PositionForm) <> System.Windows.Forms.DialogResult.OK Then
                Console.WriteLine("No file was selected. Please try again!")
                PositionForm.Close()
                PositionForm.Dispose()
                OpenFileDialog1.Dispose()
                Return ""
            End If
            PositionForm.Close()
            PositionForm.Dispose()

            MyInputFile = OpenFileDialog1.FileName
            OpenFileDialog1.Dispose()
            Return MyInputFile

        End Function

        Public Shared Function AddStringToStringsPool(ByVal value As String, ByVal strFontName As String) As Integer
            Dim aStrings(Common.ProjectMultiLanguageTranslations) As String
            aStrings(0) = value
            Dim intStringID As Integer
            Dim oStringSet As MultiLanguageStringSet
            For Each oStringSet In Common.ProjectStringPool.Values
                If intStringID < oStringSet.StringID Then
                    intStringID = oStringSet.StringID
                End If
            Next
            intStringID += 1
            oStringSet = New MultiLanguageStringSet
            oStringSet.StringID = intStringID
            oStringSet.FontName = strFontName
            oStringSet.Strings = aStrings
            oStringSet.InUse = True
            Common.ProjectStringPool.Add(oStringSet.StringID, oStringSet)
            ProjectChanged = True
            Return intStringID
        End Function

        Public Shared Sub SetBitmapName(ByVal BitmapNewName As String, ByRef oControl As Control, ByRef _BitmapName As String, ByRef _VGDDImage As VGDDImage, ByRef requiresRedraw As Boolean)
            If BitmapNewName Is Nothing Then BitmapNewName = String.Empty
            _BitmapName = BitmapNewName.Trim
            Dim OldImage As VGDDImage = _VGDDImage
            If OldImage IsNot Nothing Then
                For Each oScheme As VGDDScheme In Common._Schemes.Values
                    If oScheme.BackgroundImageName = OldImage.Name Then
                        oScheme.BackgroundImageName = BitmapNewName
                    End If
                Next
            End If
            If _BitmapName = String.Empty Then
                If OldImage IsNot Nothing Then
                    OldImage.RemoveUsedBy(oControl)
                End If
                _VGDDImage = Nothing
            Else
                _VGDDImage = GetBitmap(_BitmapName)
                If _VGDDImage Is Nothing OrElse _VGDDImage.Bitmap Is Nothing Then
                    _VGDDImage = New VGDDImage
                    _VGDDImage.TransparentBitmap = VGDDImage.InvalidBitmap(Nothing, oControl.Width, oControl.Height)
                    _VGDDImage._GraphicsPath = Nothing
                    If OldImage IsNot Nothing Then OldImage.RemoveUsedBy(oControl)
                Else
                    If Not _VGDDImage.AllowScaling Then
                        If OldImage Is Nothing Then
                            oControl.Size = _VGDDImage.OrigBitmap.Size
                        End If
                        _VGDDImage.ScaleBitmap()
                    Else
                        If TypeOf (oControl) Is VGDDMicrochip.VGDDWidgetWithBitmap Then
                            _VGDDImage.ScaleBitmap(oControl.Width, oControl.Height, CType(oControl, VGDDMicrochip.VGDDWidgetWithBitmap).Scale)
                        Else
                            _VGDDImage.ScaleBitmap(oControl.Width, oControl.Height, 1)
                        End If
                    End If
                    If OldImage IsNot Nothing AndAlso Not OldImage Is _VGDDImage Then
                        OldImage.RemoveUsedBy(oControl)
                        _VGDDImage.AddUsedBy(oControl)
                    End If
                    _VGDDImage.AddUsedBy(oControl)
                End If
            End If
            requiresRedraw = True
            oControl.Invalidate()
        End Sub

        Public Shared Function MakeBackup(ByVal strPathName As String) As Boolean
            Dim strBackupDir As String = Path.Combine(Common.VGDDProjectPath, "Backup")
            Dim strPathNameBackup As String = String.Empty
            If Common.ProjectMakeBackups Then
                If Not Directory.Exists(strBackupDir) Then
                    Try
                        Directory.CreateDirectory(strBackupDir)
                    Catch ex As Exception
                        'MessageBox.Show("Cannot create Backup directory " & strBackupDir)
                        Return False
                    End Try
                End If
                For i As Integer = 1 To 1024
                    strPathNameBackup = Path.Combine(strBackupDir, Path.GetFileName(strPathName) & "." & i.ToString)
                    If Not File.Exists(strPathNameBackup) Then
                        Exit For
                    End If
                Next
                If File.Exists(strPathName) Then
                    Try
                        File.Copy(strPathName, strPathNameBackup, True)
                    Catch ex As Exception
                        Return False
                    End Try
                End If
            End If
            Return True
        End Function

        Public Shared Function WriteFileWithBackup(ByVal strPathName As String, ByVal strFileContent As String) As Boolean
            Return WriteFileWithBackup(strPathName, strFileContent, New System.Text.ASCIIEncoding)
        End Function

        Public Shared Function WriteFileWithBackup(ByVal strPathName As String, ByVal strFileContent As String, ByVal encoding As System.Text.Encoding) As Boolean
            MakeBackup(strPathName)
            Return WriteFile(strPathName, strFileContent, encoding)
        End Function

        Public Shared Function Render2Bitmap(ByRef oContainerControl As Control, ByVal Width As Integer, ByVal Height As Integer) As Bitmap
            Dim oScreenshot As New Bitmap(Width, Height)
            Dim Left As Integer = 0, Top As Integer = 0
            Using g As Graphics = Graphics.FromImage(oScreenshot)
                If TypeOf (oContainerControl) Is VGDD.VGDDScreen Then
                    Dim oScreen As VGDD.VGDDScreen = oContainerControl
                    If Not oScreen.IsMasterScreen Then
                        g.FillRectangle(New SolidBrush(oContainerControl.BackColor), 0, 0, Width, Height)
                    End If
                    If oScreen.ShowMasterScreens AndAlso oScreen.MasterScreens IsNot Nothing AndAlso oScreen.MasterScreens.Count > 0 Then
                        For Each strMasterScreen As String In oScreen.MasterScreens
                            If Common.aScreens.ContainsKey(strMasterScreen.ToUpper) Then
                                Dim oMasterScreen As VGDD.VGDDScreen = Common.aScreens(strMasterScreen).Screen
                                If oMasterScreen Is Nothing Then
                                    Return oScreenshot
                                Else
                                    Using oMasterScreenshot As Bitmap = Common.Render2Bitmap(oMasterScreen, oMasterScreen.Width, oMasterScreen.Height)
                                        'oScreenshot.MakeTransparent(oMasterScreen.BackColor)
                                        Try
                                            g.DrawImage(oMasterScreenshot, Point.Empty)
                                        Catch ex As Exception
                                        End Try
                                    End Using
                                End If
                            End If
                        Next
                    End If
                End If
                For i As Integer = oContainerControl.Controls.Count - 1 To 0 Step -1
                    Using bmp As New Bitmap(oContainerControl.Controls(i).Width, oContainerControl.Controls(i).Height, g)
                        oContainerControl.Controls(i).DrawToBitmap(bmp, oContainerControl.Controls(i).ClientRectangle)
                        'bmp.MakeTransparent(oContainerControl.BackColor)
                        If TypeOf (oContainerControl) Is VGDD.VGDDScreen Then
                            Dim oScreen As VGDD.VGDDScreen = oContainerControl
                            If oScreen.TransparentColour <> Color.Transparent Then
                                bmp.MakeTransparent(oScreen.TransparentColour)
                            End If
                        End If
                        g.TranslateTransform(oContainerControl.Controls(i).Left - Left, oContainerControl.Controls(i).Top - Top)
                        g.DrawImageUnscaled(bmp, Point.Empty)
                        g.TranslateTransform(Left - oContainerControl.Controls(i).Left, Top - oContainerControl.Controls(i).Top)
                    End Using
                Next
            End Using
            'ReverseChildrenOrder(oContainerControl)
            'oContainerControl.DrawToBitmap(oScreenshot, oContainerControl.ClientRectangle)
            'ReverseChildrenOrder(oContainerControl)
            Return oScreenshot
        End Function

        Public Shared Sub GetOs()
            Dim myOS = Environment.OSVersion
            OsVersion = myOS.VersionString.Replace("Microsoft", "").Replace("Windows", "").Replace("NT", "").Replace("Service Pack ", "SP").Trim.Replace(" ", "_")
            OsServicePack = myOS.ServicePack.Replace("Service Pack ", "SP")
            OsPlatform = myOS.Platform.ToString
        End Sub

        Public Shared Function FilesAreTheSame(ByVal filePathOne As String, ByVal filePathTwo As String) As Boolean
            If (filePathOne = filePathTwo) Then
                Return True
            End If
            Dim fileOneStream As FileStream
            Dim fileTwoStream As FileStream
            fileOneStream = New FileStream(filePathOne, FileMode.Open, FileAccess.Read)
            fileTwoStream = New FileStream(filePathTwo, FileMode.Open, FileAccess.Read)
            If (fileOneStream.Length <> fileTwoStream.Length) Then
                fileOneStream.Close()
                fileTwoStream.Close()
                Return False
            End If
            Dim fileOneHash As String = MD5FileHash(fileOneStream)
            Dim fileTwoHash As String = MD5FileHash(fileTwoStream)
            fileOneStream.Close()
            fileTwoStream.Close()
            If fileOneHash <> fileTwoHash Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Shared Function MD5FileHash(ByVal InStream As Stream) As String
            Dim sb As New System.Text.StringBuilder
            Dim theHashValue As Byte()
            InStream.Position = 0
            Try
                Try
                    Dim oCrypt As New Security.Cryptography.HMACMD5
                    theHashValue = oCrypt.ComputeHash(InStream)
                Catch
                    Try
                        Dim oCrypt As New Security.Cryptography.MD5Cng
                        theHashValue = oCrypt.ComputeHash(InStream)
                    Catch
                        Try
                            Dim oCrypt As New Security.Cryptography.SHA1Cng
                            theHashValue = oCrypt.ComputeHash(InStream)
                        Catch
                            Try
                                Dim oCrypt As New Security.Cryptography.MACTripleDES
                                theHashValue = oCrypt.ComputeHash(InStream)
                            Catch
                                Return ""
                            End Try
                        End Try
                    End Try
                End Try
                For i As Integer = 0 To UBound(theHashValue)
                    sb.Append(String.Format("{0:X2}", theHashValue(i)))
                Next
                Return sb.ToString
            Catch
                Return ""
            End Try
            Return ""
        End Function

        Public Shared Sub SetNewZOrder(ByVal oParent As Control)
            Static AlreadySettingZorder As Boolean = False
            If oParent IsNot Nothing AndAlso Not AlreadySettingZorder Then
                AlreadySettingZorder = True
                Dim aZorder(oParent.Controls.Count - 1) As Integer
                Dim aControls(oParent.Controls.Count - 1) As Control
                Dim i As Integer = 0
                For Each cnt As Control In oParent.Controls
                    aZorder(i) = cnt.TabIndex
                    aControls(i) = cnt
                    i += 1
                Next

                Dim j As Integer = oParent.Controls.Count - 1
                Dim intZMin As Integer, intZIndex As Integer
                Do While True
                    intZMin = Integer.MaxValue
                    For i = 0 To oParent.Controls.Count - 1
                        If aZorder(i) <> Integer.MaxValue AndAlso intZMin > aZorder(i) Then
                            intZMin = aZorder(i)
                            intZIndex = i
                        End If
                    Next
                    If intZMin = Integer.MaxValue Then Exit Do
                    If oParent.Controls.GetChildIndex(aControls(intZIndex)) <> j Then
                        oParent.Controls.SetChildIndex(aControls(intZIndex), j)
                    End If
                    aZorder(intZIndex) = Integer.MaxValue
                    j -= 1
                Loop
                For j = 0 To oParent.Controls.Count - 1
                    If TypeOf (aControls(j)) Is VGDDCommon.Common.IVGDDBase Then
                        Dim oVGDDBase As VGDDCommon.Common.IVGDDBase = aControls(j)
                        oVGDDBase.Zorder = aControls.Length - oParent.Controls.GetChildIndex(aControls(j))
                    End If
                Next j
                AlreadySettingZorder = False
            End If
        End Sub

        'Public Shared Function CreateDesignScheme() As String
        '    If Common.DesignScheme Is Nothing Then
        '        Common.DesignScheme = New VGDDScheme
        '        With Common.DesignScheme
        '            .Name = "DesignScheme"
        '            Select Case Common.ProjectColourDepth
        '                Case 1
        '                    .Commonbkcolor = Color.White
        '                    .Color0 = Color.Black
        '                    .Color1 = Color.Black
        '                    .Textcolor0 = Color.Black
        '                    .Textcolor1 = Color.Black
        '                    .Embossdkcolor = Color.Black
        '                    .Embossltcolor = Color.Black
        '                    .Textcolordisabled = Color.White
        '                    .Colordisabled = Color.White
        '                Case Else
        '                    .Commonbkcolor = Color.Black
        '                    .Color0 = Color.FromArgb(&H0, &H0, &H80) 'Color.DarkBlue    'Control BgColor
        '                    .Color1 = Color.FromArgb(&H0, &H0, &HF8) 'Color.Blue
        '                    .Textcolor0 = Color.FromArgb(&HF8, &H80, &H0) 'Color.Orange
        '                    .Textcolor1 = Color.FromArgb(&H0, &H0, &H0)
        '                    .Embossdkcolor = Color.FromArgb(&H18, 0, &HE0)
        '                    .Embossltcolor = Color.FromArgb(&H0, &H80, &HC0) 'Color.LightBlue
        '                    .Textcolordisabled = Color.FromArgb(&HB8, &HB8, &HB8) 'Color.Gray
        '                    .Colordisabled = Color.FromArgb(&H0, &HFC, &H0) 'lightgreen
        '            End Select
        '            .Font = New VGDDFont(New Font("Gentium Basic", 11.25, FontStyle.Regular))
        '            AddFont(.Font, Nothing)
        '        End With
        '        If Common._Schemes.ContainsKey(Common.DesignScheme.Name) Then
        '            Common._Schemes(Common.DesignScheme.Name) = Common.DesignScheme
        '        Else
        '            Common._Schemes.Add(Common.DesignScheme.Name, Common.DesignScheme)
        '        End If
        '    End If
        '    Return Common.DesignScheme.Name
        'End Function

        Public Shared Function RoundRectanglePath(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal Radius As Integer) As Drawing2D.GraphicsPath
            Dim w2 As Integer = w / 2
            Dim h2 As Integer = h / 2

            Dim Mypath As New Drawing2D.GraphicsPath
            Mypath.StartFigure()
            If Radius > 0 Then
                Mypath.AddArc(x + w - Radius * 3, y - Radius, Radius * 2, Radius * 2, 270, 90)
                Mypath.AddArc(x + w - Radius * 3, y + h - Radius * 3, Radius * 2, Radius * 2, 0, 90)
                Mypath.AddArc(x - Radius, y + h - Radius * 3, Radius * 2, Radius * 2, 90, 90)
                Mypath.AddArc(x - Radius, y - Radius, Radius * 2, Radius * 2, 180, 90)
            Else
                Mypath.AddRectangle(New Drawing.Rectangle(0, 0, w, h))
            End If
            Mypath.CloseFigure()
            Return Mypath
        End Function

#If Not PlayerMonolitico Then
        Public Shared Function ImageUniqueSDFileName(ByVal strFileName As String) As String
            Dim strNewFileName As String = strFileName
            Dim intSuffix As Integer = 0
            For i As Integer = 1 To _Bitmaps.Count
                Dim oImage As VGDDImage = _Bitmaps(i)
                If Path.GetFileNameWithoutExtension(oImage.SDFileName) = Path.GetFileNameWithoutExtension(strNewFileName) Then
                    intSuffix += 1
                    strNewFileName = Path.GetFileNameWithoutExtension(strFileName).Substring(0, Path.GetFileNameWithoutExtension(strFileName).Length - 1) & intSuffix
                    i = 1
                End If
            Next
            Return strNewFileName
        End Function

        Public Shared Function ProjectPicFamily() As String
            Select Case Common.ProjectCompiler
                Case "C30", "XC16"
                    Return "PIC24"
                Case "C32", "XC32"
                    Return "PIC32"
                Case Else
                    Return ""
            End Select
        End Function

        Public Shared Property MplabXProjectPath As String
            Get
                Return _MplabxProjectPath
            End Get
            Set(ByVal value As String)
                'If value = String.Empty Then
                '    value = Common.VGDDProjectPath
                'End If
                _MplabxProjectPath = value
                MplabXProjectXmlPathName = Path.GetFullPath(Path.Combine(Path.Combine(_MplabxProjectPath, "nbproject"), "configurations.xml"))
            End Set
        End Property

        Public Shared Property MplabXSelectedConfig As String
            Get
                Return _MplabXSelectedConfig
            End Get
            Set(ByVal value As String)
                _MplabXSelectedConfig = value
            End Set
        End Property

#End If

        Public Shared Property ProjectChanged As Boolean
            Get
                Return _ProjectChanged
            End Get
            Set(ByVal value As Boolean)
                If value AndAlso aScreens.Count > 0 Then
                    If _ProjectChanged = False And Not ProjectLoading Then
                        _ProjectChanged = True
                        CodeHasBeenGenerated = False
                    End If
                Else
                    _ProjectChanged = False
                End If
            End Set
        End Property

        Public Shared Sub RunBrowser(ByVal Url As String)
            Try
                Process.Start(Url)
            Catch ex As Exception
                MessageBox.Show("Cannot launch browser: " & ex.Message, "Error starting browser", MessageBoxButtons.OK, MessageBoxIcon.Error)
                InputBox("Please copy and paste in your browser the following URL:", "Manual browser launch", Url)
            End Try
        End Sub

        Public Shared Function BuildDateTime() As DateTime
            If _BuildDateTime = #1/1/1980# Then
                Try
                    Dim filePath As String = System.Reflection.Assembly.GetCallingAssembly().Location
                    Const c_PeHeaderOffset As Integer = 60
                    Const c_LinkerTimestampOffset As Integer = 8
                    Dim b As Byte() = New Byte(2047) {}
                    Dim s As System.IO.Stream = Nothing

                    Try
                        s = New System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
                        s.Read(b, 0, 2048)
                    Finally
                        If s IsNot Nothing Then
                            s.Close()
                        End If
                    End Try

                    Dim i As Integer = System.BitConverter.ToInt32(b, c_PeHeaderOffset)
                    Dim secondsSince1970 As Integer = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset)
                    Dim dt As New DateTime(1970, 1, 1, 0, 0, 0)
                    dt = dt.AddSeconds(secondsSince1970)
                    dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours)
                    _BuildDateTime = dt
                Catch ex As Exception
                End Try
            End If
            Return _BuildDateTime
        End Function

        Public Shared ReadOnly Property VGDDProjectPath As String
            Get
                Return _ProjectPath
            End Get
            'Set(ByVal value As String)
            '    _ProjectPath = value
            '    BuildCodeFilenames()
            'End Set
        End Property

        Public Shared Function IsDirectoryWritable(ByVal DirPath As String) As Boolean
            Dim TestFileName As String = Path.Combine(DirPath, "WriteTest.txt")
            Try
                Using fstream As New FileStream(TestFileName, FileMode.Create)
                    Using writer As New StreamWriter(fstream)
                        writer.WriteLine("OK")
                    End Using
                End Using
            Catch ex As UnauthorizedAccessException
                Return False
            Catch ex As Exception
                Return False
            End Try
            Try
                If File.Exists(TestFileName) Then
                    File.Delete(TestFileName)
                End If
            Catch ex As Exception
            End Try
            Return True
        End Function

#If Not PlayerMonolitico Then
        Public Shared Function MplabXExtractTemplates() As Boolean
            Dim blnForceExtract As Boolean = False

#If CONFIG = "Debug" Then
            Common.MplabXTemplatesFolder = Path.GetFullPath(Path.Combine(Application.ExecutablePath, "..\..\..\MPLABX"))
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesLegacyMLA.xml")
                Case "MLA"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesMLA.xml")
                Case "HARMONY"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesHarmony.xml")
                Case Else
                    Return False
            End Select
#Else
            Common.MplabXTemplatesFolder = Path.GetDirectoryName(Application.CommonAppDataPath)
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesLegacyMLA.xml")
                Case "MLA"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesMLA.xml")
                Case "HARMONY"
                    Common.MplabXTemplateFile = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesHarmony.xml")
                Case Else
                    Return False
            End Select
            If Not IsDirectoryWritable(Common.MplabXTemplatesFolder) Then
                Common.MplabXTemplatesFolder = Application.LocalUserAppDataPath
                If Not IsDirectoryWritable(Common.MplabXTemplatesFolder) Then
                    MessageBox.Show("Cannot extract MPLAB X Templates to either" & vbCrLf & Path.GetDirectoryName(Application.CommonAppDataPath) & vbCrLf & "or " & Application.LocalUserAppDataPath & vbCrLf & vbCrLf & "Please modify folder permissions in order for MPLAB X Wizard to work.", "Missing Directory Write Permissions")
                    Return False
                End If
            End If

            'Dim strFlagFile As String = Path.Combine(Application.CommonAppDataPath, "TemplatesExtracted.flg")
            'If Not File.Exists(strFlagFile) Then
            'Dim file As StreamWriter = New StreamWriter(strFlagFile)
            'file.Write("OK")
            'file.Close()
            blnForceExtract = True
            'End If

            For Each oAssembly As Assembly In New Assembly() {Assembly.GetEntryAssembly, MplabX.MplabXFilesAssembly}
                For Each strTemplateFile As String In GetAllResourceNames("*.xml, *.c, *.h", oAssembly)
                    Application.DoEvents()
                    If strTemplateFile Is Nothing Then Continue For
                    If strTemplateFile = "CustomWidgetsTemplates.xml" Then
                        EnsureResourceIsOnDisk(strTemplateFile, oAssembly, Common.MplabXTemplatesFolder, False)
                    Else
                        EnsureResourceIsOnDisk(strTemplateFile, oAssembly, Common.MplabXTemplatesFolder, blnForceExtract)
                    End If
                Next
            Next
#End If
            Return True
        End Function

        Public Shared Sub MergeTemplate(ByVal strTemplateFile As String)
            If strTemplateFile Is Nothing Then Exit Sub
            Dim xml As String
            Dim oXmlBoardDoc As New XmlDocument
            Try
                xml = ReadFile(Path.Combine(Common.MplabXTemplatesFolder, strTemplateFile))
                oXmlBoardDoc.LoadXml(xml)
                Dim oBoardNode As XmlNode = oXmlBoardDoc.DocumentElement
                If oBoardNode.Attributes("Type") IsNot Nothing AndAlso oBoardNode.Attributes("ID") IsNot Nothing Then
                    Dim strID As String = oBoardNode.Attributes("ID").Value
                    Dim strType As String = oBoardNode.Attributes("Type").Value
                    Dim oTemplateBoardNode As XmlNode = Common.XmlMplabxTemplatesDoc.DocumentElement.SelectSingleNode(String.Format("{0}Boards/Board[@ID='{1}']", strType, strID))
                    If oTemplateBoardNode Is Nothing Then
                        oTemplateBoardNode = XmlMplabxTemplatesDoc.ImportNode(oBoardNode, True)
                        Common.XmlMplabxTemplatesDoc.DocumentElement.SelectSingleNode(String.Format("{0}Boards", strType)).AppendChild(oTemplateBoardNode)
                    Else
                        oTemplateBoardNode.InnerXml = oBoardNode.InnerXml
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show("Error loading MPLAB X Template " & strTemplateFile & " - " & ex.Message, "Error loading Template XML", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End Try
        End Sub

        Public Shared Function MplabXLoadAndMergeTemplates() As Boolean
            Dim xml As String
            Try
                Try

                    xml = ReadFile(Common.MplabXTemplateFile).Replace(vbTab, "    ")

                    Common.XmlMplabxTemplatesDoc = Nothing
                    Common.XmlMplabxTemplatesDoc = New XmlDocument
                    Common.XmlMplabxTemplatesDoc.PreserveWhitespace = False
                    Common.XmlMplabxTemplatesDoc.LoadXml(xml)

                    'Catch ex As Exception
                    '    MessageBox.Show("Error reading MplabXTemplates.xml" & vbCrLf & "Can't run Wizard!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    '    Return
                    'End Try

                Catch ex As Exception
                    MessageBox.Show(String.Format("Error loading MPLAB X template {0}. May be a temporary files cleanup can solve the issue." & vbCrLf & ex.Message, Common.MplabXTemplateFile), "MplabXLoadAndMergeTemplates Error 1")
                End Try

#If CONFIG = "Debug" Then
                Dim strBoardsPath As String = Path.Combine(Common.MplabXTemplatesFolder, "Boards")
                Dim aDirs As String() = Directory.GetDirectories(strBoardsPath)
                ReDim Preserve aDirs(aDirs.Length)
                aDirs(aDirs.Length - 1) = strBoardsPath 'Path.Combine(Common.MplabXTemplatesFolder, "Boards")
                For Each strDir As String In aDirs
                    For Each strTemplateFile As String In Directory.GetFiles(strDir, "Boards.*")
                        MergeTemplate(strTemplateFile)
                    Next
                Next
#Else
                Try
                    For Each strTemplateFile As String In Directory.GetFiles(Common.MplabXTemplatesFolder, "Boards.*")
                        MergeTemplate(strTemplateFile)
                    Next
                Catch ex As Exception
                    MessageBox.Show(String.Format("Error merging templates from folder {0}" & vbCrLf & ex.Message, Common.MplabXTemplatesFolder), "MplabXLoadAndMergeTemplates Error 2")
                End Try
#End If
                Try
                    Dim strUserBoardsDir As String = Path.Combine(Common.UserTemplatesFolder, "Boards")
                    If Directory.Exists(strUserBoardsDir) Then
                        For Each strDir As String In Directory.GetDirectories(strUserBoardsDir)
                            For Each strTemplateFile As String In Directory.GetFiles(strDir, "*.xml")
                                MergeTemplate(strTemplateFile)
                            Next
                        Next
                    End If
                Catch ex As Exception
                    MessageBox.Show(String.Format("Error merging custom templates from folder {0}" & vbCrLf & ex.Message, Common.UserTemplatesFolder), "MplabXLoadAndMergeTemplates Error 3")
                End Try

                Try
                    For Each oNode As XmlNode In Common.XmlMplabxTemplatesDoc.SelectNodes("/MplabXWizard/Option[@Disabled='true']")
                        Common.XmlMplabxTemplatesDoc.DocumentElement.RemoveChild(oNode)
                    Next
                Catch ex As Exception
                End Try

                Dim sw As New StringWriter
                Dim xtw As New XmlTextWriter(sw)
                xtw.Formatting = Formatting.Indented
                Common.XmlMplabxTemplatesDoc.WriteTo(xtw)
                Dim strMplabXTemplatesLast As String = String.Empty
                Try
                    strMplabXTemplatesLast = Path.Combine(Common.MplabXTemplatesFolder, "MplabXTemplatesLast.xml")
                    Dim file As StreamWriter = New StreamWriter(strMplabXTemplatesLast, False, New System.Text.UTF8Encoding)
                    file.Write(sw.ToString)
                    file.Close()
                Catch ex As Exception
                    MessageBox.Show(String.Format("Error writing merged temtemplate {0}" & vbCrLf & ex.Message, strMplabXTemplatesLast), "MplabXLoadAndMergeTemplates Error 4")
                End Try

            Catch ex As Exception
                MessageBox.Show("Error loading and merging MPLAB X templates:" & vbCrLf & ex.Message, "Error G")
            End Try

            Return True
        End Function

        Public Shared Sub EnsureResourceIsOnDisk(ByVal ResourceName As String, ByVal oAssembly As Assembly, ByVal ResourcePath As String, ByVal blnForceExtract As Boolean)
            Dim ResourceFilePath As String = Path.Combine(ResourcePath, ResourceName)
            Dim Buffer As String
            Dim blnExtractResource As Boolean

#If CONFIG = "DemoRelease" Then
            blnForceExtract = True
#End If

            If blnForceExtract OrElse Not System.IO.File.Exists(ResourceFilePath) Then
                blnExtractResource = True
                'ElseIf (System.IO.File.GetAttributes(ResourceFilePath) And FileAttributes.Archive) <> FileAttributes.Archive Then
                '    blnExtractResource = True
            End If
            If blnExtractResource Then
                Application.DoEvents()
                'MakeBackup(ResourceFilePath)
                Try
                    Dim s As Stream = VirtualFabUtils.Utils.GetResourceStream(ResourceName, oAssembly)
                    If s IsNot Nothing Then
                        Dim sr As StreamReader
                        sr = New StreamReader(s)
                        Buffer = sr.ReadToEnd
                        Dim file As StreamWriter = New StreamWriter(ResourceFilePath, False, New System.Text.UnicodeEncoding)
                        file.Write(Buffer)
                        file.Close()
                    End If
                Catch ex As Exception
                    'TODO: Log
                    'MessageBox.Show("Error extracting resource file " & ResourceName & " to the Path " & ResourcePath & ":" & vbCrLf & ex.Message & vbCrLf & "Please ensure the folder is writable", "File system error")
                End Try
                'Dim CurrentAttributes As Integer = System.IO.File.GetAttributes(ResourceFilePath)
                'If CurrentAttributes And FileAttributes.Archive Then
                '    System.IO.File.SetAttributes(ResourceFilePath, CurrentAttributes - FileAttributes.Archive)
                'End If
            End If
        End Sub

        Public Shared Property CodeGenLocation As Integer
            Get
                Return _CodeGenLocation
            End Get
            Set(ByVal value As Integer)
                _CodeGenLocation = value
                Select Case value
                    Case 1
                        Common.CodeGenDestPath = Common.MplabXProjectPath
                    Case 2
                        Common.CodeGenDestPath = Common.VGDDProjectPath
                    Case 3
                        If Common.MplabXProjectPath <> String.Empty Then
                            Dim aPath() As String = Common.MplabXProjectPath.Split(Path.DirectorySeparatorChar)
                            aPath(aPath.Length - 1) = ""
                            Common.CodeGenDestPath = Path.GetFullPath(String.Join(Path.DirectorySeparatorChar, aPath))
                        Else
                            Common.CodeGenDestPath = Common.VGDDProjectPath
                        End If
                    Case 4
                        'BuildCodeFilenames()
                End Select
                If value <> _CodeGenLocation AndAlso Not ProjectLoading Then
                    For Each oBitmap As VGDDImage In Bitmaps
                        If Not oBitmap.ToBeConverted Then
                            oBitmap.ToBeConverted = True
                        End If
                    Next
                    For Each oFont As VGDDFont In _Fonts
                        If Not oFont.ToBeConverted Then
                            oFont.ToBeConverted = True
                        End If
                    Next
                End If
            End Set
        End Property

        Public Shared Property CodeGenDestPath As String
            Get
                Return _CodeGenDestPath
            End Get
            Set(ByVal value As String)
                _CodeGenDestPath = value
                BuildCodeFilenames()
            End Set
        End Property

        Public Shared Function MplabxCheckProjectPath(ByVal strPath As String) As Boolean
            If strPath <> "" Then
                Try
                    strPath = Path.GetFullPath(strPath)
                    Dim MplabXProjectXmlPathName As String = Path.Combine(Path.Combine(strPath, "nbproject"), "configurations.xml")
                    If Not Directory.Exists(Path.Combine(strPath, "nbproject")) OrElse Not File.Exists(MplabXProjectXmlPathName) Then
                        Common.MplabXProjectPath = ""
                        'MessageBox.Show("This folder doesn't appear to contain a valid MPLABX project", "Wizard exiting")
                        Return False
                    End If
                    If Common.MplabXProjectPath = String.Empty OrElse Path.GetFullPath(Common.MplabXProjectPath) <> strPath Then
                        If Common.MplabXProjectPath <> String.Empty Then
                            Common.ProjectChanged = True
                        End If
                        Common.MplabXProjectPath = strPath
                    End If
                    Return True

                Catch ex As Exception
                    Return False
                End Try
            Else
                Return False
            End If
        End Function

#End If

        Public Shared Property ProjectPathName As String
            Get
                If _ProjectPath IsNot Nothing AndAlso _ProjectFileName IsNot Nothing Then
                    Return Path.Combine(_ProjectPath, _ProjectFileName)
                Else
                    Return Directory.GetCurrentDirectory
                End If
            End Get
            Set(ByVal value As String)
                If _ProjectFileName <> Path.GetFileName(value) Or _
                    _ProjectName <> Path.GetFileNameWithoutExtension(value).Replace("-", "_") Or _
                    _ProjectPath <> Path.GetDirectoryName(value) Then
                    ProjectChanged = True
                End If
                If value IsNot Nothing AndAlso value <> String.Empty Then
                    _ProjectFileName = Path.GetFileName(value)
                    _ProjectName = Path.GetFileNameWithoutExtension(value).Replace("-", "_")
                    _ProjectPath = Path.GetDirectoryName(value)
#If Not PlayerMonolitico Then
                    BuildCodeFilenames()
#End If
                End If
            End Set
        End Property

        Public Shared ReadOnly Property ProjectFileName As String
            Get
                Return _ProjectFileName
            End Get
            'Set(ByVal value As String)
            '    _ProjectFileName = value
            '    _ProjectName = Path.GetFileNameWithoutExtension(value).Replace("-", "_")
            '    '_ProjectPath = Path.GetDirectoryName(value)
            '    BuildCodeFilenames()
            'End Set
        End Property

        Public Shared ReadOnly Property ProjectName As String
            Get
                Select Case Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        Return _ProjectName
                    Case Else
                        Return _ProjectName.ToLower
                End Select
            End Get
        End Property

#If Not PlayerMonolitico Then
        Public Shared Sub BuildCodeFilenames()
            If _CodeGenDestPath Is Nothing Then
                _CodeGenDestPath = VGDDProjectPath
            End If
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    ProjectFileName_ScreensC = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_Screens.c")
                    ProjectFileName_ScreensH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_Screens.h")
                    ProjectFileName_ScreenStatesH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_ScreenStates.h")
                    ProjectFileName_EventsHelperH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_EventsHelper.h")
                    ProjectFileName_InternalResourcesC = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_Resources.c")
                    ProjectFileName_InternalResourcesRefC = String.Empty
                    ProjectFileName_ExternalResourcesC = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_ExternalResources.c")
                    ProjectFileName_BmpOnSdResourcesC = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_BmpOnSdResources.c")
                    ProjectFileName_BmpOnSdResourcesRefC = String.Empty
                    ProjectFileName_InternalResourcesH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_Resources.h")
                    ProjectFileName_ExternalResourcesRefC = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_ExternalResources.c")
                    ProjectFileName_ExternalResourcesH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_ExternalResources.h")
                    ProjectFileName_BmpOnSdResourcesH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_BmpOnSdResources.h")
                Case "MLA"
                    ProjectFileName_ScreensC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_screens.c")
                    ProjectFileName_ScreensH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_screens.h")
                    ProjectFileName_ScreenStatesH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_screen_states.h")
                    ProjectFileName_EventsHelperH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_events_helper.h")

                    ProjectFileName_InternalResourcesC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_internal_resource.S")
                    ProjectFileName_InternalResourcesRefC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_internal_resource_reference.c")
                    ProjectFileName_ExternalResourcesC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_external_resource.S")
                    ProjectFileName_BmpOnSdResourcesC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_bmp_on_sd_resource.S")
                    ProjectFileName_BmpOnSdResourcesRefC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_bmp_on_sd_resource_reference.c")
                    ProjectFileName_InternalResourcesH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_internal_resource.h")
                    ProjectFileName_ExternalResourcesRefC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_external_resource_reference.c")
                    ProjectFileName_ExternalResourcesH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_external_resource.h")
                    ProjectFileName_BmpOnSdResourcesH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_BmpOnSdResources.h")
                Case "HARMONY"
                    'If MplabXSelectedConfig = String.Empty Then
                    '    'MessageBox.Show("MHC Layout active but no MPLAB X configuration selected. Please connect to VGDD-Link plugin")
                    '    Exit Sub
                    'End If
                    'VGDDTest-MHC/firmware/src/system_config/pic32mx_usb_sk2_lcc_pictail_wqvga/gfx_hgc_definitions.c
                    ProjectFileName_ScreensC = Path.Combine(_CodeGenDestPath, "gfx_vgdd_definitions.c")
                    ProjectFileName_ScreensH = Path.Combine(_CodeGenDestPath, "gfx_vgdd_definitions.h")
                    ProjectFileName_EventsHelperH = ProjectFileName_ScreensH
                    'Else
                    'ProjectFileName_ScreensC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_screens.c")
                    'ProjectFileName_ScreensH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_screens.h")
                    'ProjectFileName_EventsHelperH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_events_helper.h")
                    'End If
                    ProjectFileName_ScreenStatesH = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_screen_states.h")

                    ProjectFileName_InternalResourcesC = Path.Combine(_CodeGenDestPath, "internal_resource.S")
                    ProjectFileName_InternalResourcesRefC = Path.Combine(_CodeGenDestPath, "internal_resource_reference.c")
                    ProjectFileName_ExternalResourcesC = Path.Combine(_CodeGenDestPath, "external_resource.S")
                    ProjectFileName_BmpOnSdResourcesC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_bmp_on_sd_resource.S")
                    ProjectFileName_BmpOnSdResourcesRefC = Path.Combine(_CodeGenDestPath, "vgdd_" & _ProjectName.ToLower & "_bmp_on_sd_resource_reference.c")
                    ProjectFileName_InternalResourcesH = Path.Combine(_CodeGenDestPath, "internal_resource.h")
                    ProjectFileName_ExternalResourcesRefC = Path.Combine(_CodeGenDestPath, "external_resource_reference.c")
                    ProjectFileName_ExternalResourcesH = Path.Combine(_CodeGenDestPath, "external_resource.h")
                    ProjectFileName_BmpOnSdResourcesH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_BmpOnSdResources.h")
            End Select
            ProjectFileName_StringsPoolC = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_StringsPool.c")
            ProjectFileName_StringsPoolH = Path.Combine(_CodeGenDestPath, "VGDD_" & _ProjectName & "_StringsPool.h")
        End Sub
#End If
        Public Shared Function CleanName(ByVal NameIn As String) As String
            Dim strNameOut As String = ""
            If NameIn IsNot Nothing Then
                For Each c As Char In NameIn
                    If c = "-"c Or c = " "c Then c = "_"c
                    If "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_[]".IndexOf(c) >= 0 Then
                        strNameOut &= c
                    End If
                Next
                If strNameOut = "" Then
                    strNameOut = String.Format("NoValidCharsInName{0}", CInt(Rnd(1) * 1000))
                End If
                If "0123456789".Contains(strNameOut.Substring(0, 1)) Then
                    strNameOut = "_" & strNameOut
                End If
            End If
            Return strNameOut
        End Function

        Public Shared Event BitmapAdded(ByVal vImage As VGDDImage)
        Public Shared Event BitmapRemoved(ByVal vImage As VGDDImage)
        Public Shared Event FontAdded(ByVal Font As VGDDFont)
        Public Shared Event FontRemoving(ByVal Font As VGDDFont, ByRef Cancel As Boolean)

#Region "Project Handling"

        Public Shared Function ReadScreenName(ByVal FileName As String) As String
            If File.Exists(FileName) Then
                Try
                    Dim sr As StreamReader = New StreamReader(New FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    Dim cleandown As String = sr.ReadToEnd.Replace(", VGDDCommon,", ", VGDD,").Replace(", VGDDMicrochip,", ", VGDD,")
                    cleandown = ("<DOCUMENT_ELEMENT>" + (cleandown + "</DOCUMENT_ELEMENT>"))
                    Dim oXmlScreenDoc As New XmlDocument
                    oXmlScreenDoc.LoadXml(cleandown)
                    Dim oRootNode As XmlNode = oXmlScreenDoc.DocumentElement.SelectSingleNode("Screen")
                    If oRootNode Is Nothing Then
                        oRootNode = oXmlScreenDoc.DocumentElement
                    End If
                    For Each oNode As XmlNode In oRootNode.ChildNodes
                        If oNode.Name = "Object" Then
                            If oNode.Attributes("type") IsNot Nothing AndAlso oNode.Attributes("type").Value.Contains("VGDDScreen") Then
                                Return oNode.Attributes("name").Value
                            End If
                        End If
                    Next
                    Return Nothing

                Catch ex As Exception

                End Try
            Else
                Return Path.GetFileNameWithoutExtension(FileName)
            End If
            Return Nothing
        End Function

        Public Shared Function LoadProject(ByRef oProjectNode As XmlElement) As String
            Dim blnAskFramework As Boolean = False
            aScreens.Clear()
            Mal.FrameworkName = String.Empty
            Mal.MalPath = String.Empty
            If oProjectNode.Attributes("ColourDepth") IsNot Nothing Then
                Common.ProjectColourDepth = oProjectNode.Attributes("ColourDepth").Value
                If Common.ProjectColourDepth = 0 Then
                    Common.ProjectColourDepth = 16
                End If
            End If
            If oProjectNode.Attributes("Width") IsNot Nothing Then
                Common.ProjectWidth = oProjectNode.Attributes("Width").Value
            End If
            If oProjectNode.Attributes("Height") IsNot Nothing Then
                Common.ProjectHeight = oProjectNode.Attributes("Height").Value
            End If
            If oProjectNode.Attributes("UseIndexedCustomColours") IsNot Nothing Then
                Common.ProjectUsePalette = oProjectNode.Attributes("UseIndexedCustomColours").Value
            End If
            If oProjectNode.Attributes("UseMultiByteChars") IsNot Nothing Then
                Common.ProjectUseMultiByteChars = oProjectNode.Attributes("UseMultiByteChars").Value
            End If
            If oProjectNode.Attributes("MultiLanguageTranslations") IsNot Nothing Then
                Common.ProjectMultiLanguageTranslations = oProjectNode.Attributes("MultiLanguageTranslations").Value
            End If
            If oProjectNode.Attributes("ActiveLanguage") IsNot Nothing Then
                Common.ProjectActiveLanguage = oProjectNode.Attributes("ActiveLanguage").Value
            End If
            If oProjectNode.Attributes("ProjectStringsPoolGenerateHeader") IsNot Nothing Then
                Common.ProjectStringsPoolGenerateHeader = oProjectNode.Attributes("ProjectStringsPoolGenerateHeader").Value
            End If
            If oProjectNode.Attributes("Compiler") IsNot Nothing AndAlso oProjectNode.Attributes("Compiler").Value <> "" Then
                Common.ProjectCompiler = oProjectNode.Attributes("Compiler").Value
                Select Case Common.ProjectCompiler
                    Case "C30", "XC16"
                        Common.ProjectCompilerFamily = "C30"
                    Case "C32", "XC32"
                        Common.ProjectCompilerFamily = "C32"
                End Select

            End If
            If oProjectNode.Attributes("PlayerBgBitmap") IsNot Nothing AndAlso oProjectNode.Attributes("PlayerBgBitmap").Value <> "" Then
                Common.ProjectPlayerBgBitmapName = oProjectNode.Attributes("PlayerBgBitmap").Value
            End If
            If oProjectNode.Attributes("PlayerBgColour") IsNot Nothing AndAlso oProjectNode.Attributes("PlayerBgColour").Value <> "" Then
                Dim aRGB() As String = oProjectNode.Attributes("PlayerBgColour").Value.ToString.Split(",")
                If aRGB.Length = 3 Then
                    Common.ProjectPlayerBgColour = Color.FromArgb(aRGB(0), aRGB(1), aRGB(2))
                End If
            End If
#If Not PlayerMonolitico Then
            If oProjectNode.Attributes("MplabXProjectPath") IsNot Nothing AndAlso oProjectNode.Attributes("MplabXProjectPath").Value <> "" Then
                Common.MplabXProjectPath = Path.GetFullPath(Path.Combine(Common.VGDDProjectPath, oProjectNode.Attributes("MplabXProjectPath").Value))
                If Not Directory.Exists(Common.MplabXProjectPath) Then
                    Common.MplabXProjectPath = Path.Combine(Common.VGDDProjectPath, Common.MplabXProjectPath)
                End If
            End If
            If oProjectNode.Attributes("MplabXSelectedConfig") IsNot Nothing AndAlso oProjectNode.Attributes("MplabXSelectedConfig").Value <> "" Then
                Common.MplabXSelectedConfig = oProjectNode.Attributes("MplabXSelectedConfig").Value
            End If
            If oProjectNode.Attributes("FrameworkName") IsNot Nothing AndAlso oProjectNode.Attributes("FrameworkName").Value <> "" AndAlso Mal.FrameworkName = String.Empty Then
                Mal.FrameworkName = oProjectNode.Attributes("FrameworkName").Value
                If Mal.MalPath = String.Empty Then
                    Mal.SelectMalFromFrameworkName(Mal.FrameworkName)
                End If
                blnAskFramework = False
            End If
            If oProjectNode.Attributes("ProjectPathGRC") IsNot Nothing AndAlso oProjectNode.Attributes("ProjectPathGRC").Value <> "" Then
                Common.ProjectPathGRC = oProjectNode.Attributes("ProjectPathGRC").Value
            End If
            If oProjectNode.Attributes("MALPath") IsNot Nothing AndAlso oProjectNode.Attributes("MALPath").Value <> "" Then
                Mal.MalPath = oProjectNode.Attributes("MALPath").Value
                If Not Directory.Exists(Mal.MalPath) Or Mal.MalPath.StartsWith("..") Then
                    Mal.MalPath = Path.GetFullPath(Path.Combine(VGDDProjectPath, Mal.MalPath))
                End If
                Mal.CheckMalVersion(Mal.MalPath)
                If Mal.FrameworkName = String.Empty Then
                    blnAskFramework = True
                End If
            Else
                blnAskFramework = True
            End If
            If oProjectNode.Attributes("CodeGenLocation") IsNot Nothing AndAlso oProjectNode.Attributes("CodeGenLocation").Value <> "" Then
                Common.CodeGenLocation = oProjectNode.Attributes("CodeGenLocation").Value
            End If
            If oProjectNode.Attributes("CodeGenDestPath") IsNot Nothing AndAlso oProjectNode.Attributes("CodeGenDestPath").Value <> "" Then
                Common.CodeGenDestPath = Path.GetFullPath(Path.Combine(Common.VGDDProjectPath, oProjectNode.Attributes("CodeGenDestPath").Value))
            End If
#End If
            If oProjectNode.Attributes("CopyBitmapsInVgddProjectFolder") IsNot Nothing AndAlso oProjectNode.Attributes("CopyBitmapsInVgddProjectFolder").Value <> "" Then
                Common.ProjectCopyBitmapsInVgddProjectFolder = oProjectNode.Attributes("CopyBitmapsInVgddProjectFolder").Value
            End If
            If oProjectNode.Attributes("UseBmpPrefix") IsNot Nothing AndAlso oProjectNode.Attributes("UseBmpPrefix").Value <> "" Then
                Common.ProjectUseBmpPrefix = oProjectNode.Attributes("UseBmpPrefix").Value
            End If
            If oProjectNode.Attributes("BitmapsBinPath") IsNot Nothing AndAlso oProjectNode.Attributes("BitmapsBinPath").Value <> "" Then
                Common.BitmapsBinPath = oProjectNode.Attributes("BitmapsBinPath").Value
            End If
            If oProjectNode.Attributes("HeapSize") IsNot Nothing AndAlso oProjectNode.Attributes("HeapSize").Value <> "" Then
                Common.ProjectHeapSize = oProjectNode.Attributes("HeapSize").Value
            End If
            If oProjectNode.Attributes("LastComputedHeapSize") IsNot Nothing AndAlso oProjectNode.Attributes("LastComputedHeapSize").Value <> "" Then
                Common.ProjectLastComputedHeapSize = oProjectNode.Attributes("LastComputedHeapSize").Value
            End If
            If oProjectNode.Attributes("VGDDVersion") IsNot Nothing AndAlso oProjectNode.Attributes("VGDDVersion").Value <> "" Then
                Common.ProjectVGDDVersion = oProjectNode.Attributes("VGDDVersion").Value
            End If
            If oProjectNode.Attributes("VGDDLicensed") IsNot Nothing AndAlso oProjectNode.Attributes("VGDDLicensed").Value <> "" Then
#If CONFIG = "DemoRelease" Or CONFIG = "DemoDebug" Then
                MessageBox.Show("You are opening a project that has been created with a Licensed (Full) VGDD" & vbCrLf & _
                                "but the running VGDD is a DEMO version and the project will break if you try to modify it." & vbCrLf & vbCrLf & _
                                "Please purchase additional VGDD licenses if you need to work on different PCs", "Warning - DEMO version")
#End If
                Common.ProjectVGDDIsLicensed = oProjectNode.Attributes("VGDDLicensed").Value
            End If

            Dim oAttr As XmlAttribute
            For Each node As XmlNode In oProjectNode.ChildNodes
                If node.NodeType = XmlNodeType.Whitespace Then Continue For
                Application.DoEvents()
                If node.Name.Equals("Screen") Then
                    Dim strScreenFilename As String = node.Attributes("FileName").Value.ToString
                    strScreenFilename = Path.Combine(VGDDProjectPath, strScreenFilename)
                    If Not File.Exists(strScreenFilename) AndAlso File.Exists(strScreenFilename & ".vds") Then
                        strScreenFilename &= ".vds"
                    End If

                    Dim blnShown As Boolean
                    oAttr = node.Attributes("Shown")
                    If oAttr IsNot Nothing Then
                        blnShown = oAttr.Value
                    Else
                        blnShown = True
                    End If

                    aScreens.Add(strScreenFilename, blnShown)
                ElseIf node.Name.Equals("Scheme") Then
                    Dim oScheme As New VGDDScheme
                    oScheme.FromXml(node)
                    Common._Schemes.Add(oScheme.Name, oScheme)
                    'Common.AddFont(oScheme.Font, Nothing)
                ElseIf node.Name.Equals("Bitmap") Then
                    Dim strBitmapFileName As String = node.Attributes("FileName").Value.ToString
                    Do While strBitmapFileName.StartsWith(".\")
                        strBitmapFileName = strBitmapFileName.Substring(2)
                    Loop
                    If Not File.Exists(strBitmapFileName) Then
                        strBitmapFileName = Path.Combine(VGDDProjectPath, strBitmapFileName)
                    End If
                    Dim oImage As VGDDImage
                    Dim strBitmapName As String = Common.CleanName(Path.GetFileNameWithoutExtension(strBitmapFileName))

                    oAttr = node.Attributes("Name")
                    If oAttr IsNot Nothing Then
                        strBitmapName = oAttr.Value
                    End If

                    oAttr = node.Attributes("Type")
                    If oAttr Is Nothing Then
                        oImage = Common.AddBitmap(strBitmapName, strBitmapFileName, Nothing)
                    Else
                        oImage = Common.AddBitmap(strBitmapName, strBitmapFileName, Nothing, node.Attributes("Type").Value.ToString)
                    End If

                    If oImage IsNot Nothing Then
                        oImage.IsLoading = True

                        oAttr = node.Attributes("SDFileName")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.SDFileName = oAttr.Value
                            Catch ex As Exception
                            End Try
                        End If

                        oAttr = node.Attributes("AllowScaling")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.AllowScaling = oAttr.Value
                            Catch ex As Exception
                            End Try
                        End If
                        oAttr = node.Attributes("Size")
                        If oAttr IsNot Nothing Then
                            Try
                                Dim intWidth As Integer, intHeight As Integer
                                Dim strValue As String = oAttr.Value.Replace("{", "").Replace("}", "")
                                intWidth = strValue.Split(",")(0).Split("=")(1)
                                intHeight = strValue.Split(",")(1).Split("=")(1)
                                oImage.Size = New Size(intWidth, intHeight)
                            Catch ex As Exception
                            End Try
                        End If

                        oAttr = node.Attributes("ColourDepth")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.ColourDepth = System.Enum.Parse(GetType(VGDDImage.ValidColourDepths), oAttr.Value, True)
                            Catch ex As Exception
                            End Try
                        End If

#If Not PlayerMonolitico Then
                        oAttr = node.Attributes("CompressionType")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.CompressionType = System.Enum.Parse(GetType(GrcProject.CompressionTypes), oAttr.Value, True)
                            Catch ex As Exception
                            End Try
                        End If
#End If

                        oAttr = node.Attributes("InterpolationMode")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.InterpolationMode = System.Enum.Parse(GetType(Drawing2D.InterpolationMode), oAttr.Value, True)
                            Catch ex As Exception
                            End Try
                        End If

                        oAttr = node.Attributes("ForceInclude")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.ForceInclude = oAttr.Value
                            Catch ex As Exception
                            End Try
                        End If

                        oAttr = node.Attributes("GroupName")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.GroupName = oAttr.Value
                            Catch ex As Exception
                            End Try
                        End If

                        oAttr = node.Attributes("RotateFlip")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage.RotateFlip = oAttr.Value
                            Catch ex As Exception
                            End Try
                        End If

                        oAttr = node.Attributes("BitmapSize")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage._BitmapSize = oAttr.Value
                            Catch ex As Exception
                            End Try
                        End If

                        oAttr = node.Attributes("BitmapCompressedSize")
                        If oAttr IsNot Nothing Then
                            Try
                                oImage._BitmapCompressedSize = oAttr.Value
                            Catch ex As Exception
                            End Try
                        End If

                        oImage.IsLoading = False
                    End If
                ElseIf node.Name.Equals("VGDDFont") Then
                    Dim oFont As New VGDDFont
                    oFont.FromXml(node)
                    Common.AddFont(oFont, Nothing)
                ElseIf node.Name.Equals("BitmapsBinPath") Then
                    Common.BitmapsBinPath = Path.Combine(VGDDProjectPath, node.Attributes("Path").Value.ToString)
                ElseIf node.Name.Equals("CustomColours") Then
                    'Try
                    '    Array.Resize(Common.CDialogCustomColours, 2 ^ Common.ProjectColourDepth)
                    '    Dim i As Integer = 0
                    '    For Each oNodeCustomColour As XmlNode In node
                    '        If oNodeCustomColour.NodeType = XmlNodeType.Whitespace Then Continue For
                    '        Dim aRGB() As String = oNodeCustomColour.Attributes("Value").Value.ToString.Split(",")
                    '        If aRGB.Length = 3 Then
                    '            Dim c As Color = Color.FromArgb(aRGB(0), aRGB(1), aRGB(2))
                    '            Common.CDialogCustomColours(i) = c
                    '            i += 1
                    '        End If
                    '    Next
                    'Catch ex As Exception
                    'End Try
                ElseIf node.Name.Equals("Wizard") Then
                    Common.WizardOptions.Clear()
                    For Each WizardNode As XmlNode In node.ChildNodes
                        If WizardNode.NodeType = XmlNodeType.Whitespace Then Continue For
                        Select Case WizardNode.Name
                            Case "DevBoardID"
                                Common.DevelopmentBoardID = WizardNode.InnerText
                            Case "PIMBoardID"
                                Common.PIMBoardID = WizardNode.InnerText
                            Case "ExpansionBoardID"
                                Common.ExpansionBoardID = WizardNode.InnerText
                            Case "DisplayBoardID"
                                Common.DisplayBoardID = WizardNode.InnerText
                            Case "DisplayBoardOrientation"
                                Common.DisplayBoardOrientation = WizardNode.InnerText
                            Case "Option"
                                Dim blnChecked As Boolean
                                Boolean.TryParse(WizardNode.Attributes("Checked").Value, blnChecked)
                                Common.WizardOptions.Add(WizardNode.Attributes("Name").Value, blnChecked)
                        End Select
                    Next
                ElseIf node.Name.Equals("HTMLEditor") Then
                    oAttr = node.Attributes("SpitterDistance")
                    If oAttr IsNot Nothing Then
                        Try
                            Common.HtmlEditorSplitterDistance = oAttr.Value
                        Catch ex As Exception
                        End Try
                    End If

                    oAttr = node.Attributes("WebPagesFolder")
                    If oAttr IsNot Nothing Then
                        Try
                            Common.ProjectHtmlWebPagesFolder = oAttr.Value
                        Catch ex As Exception
                        End Try
                    End If

                    oAttr = node.Attributes("OutputType")
                    If oAttr IsNot Nothing Then
                        Try
                            Common.ProjectHtmlOutputType = oAttr.Value
                        Catch ex As Exception
                        End Try
                    End If

                    oAttr = node.Attributes("TargetURL")
                    If oAttr IsNot Nothing Then
                        Try
                            Common.ProjectHtmlTargetUrl = oAttr.Value
                        Catch ex As Exception
                        End Try
                    End If

                    oAttr = node.Attributes("TargetUser")
                    If oAttr IsNot Nothing Then
                        Try
                            Common.ProjectHtmlTargetUser = oAttr.Value
                        Catch ex As Exception
                        End Try
                    End If

                    oAttr = node.Attributes("TargetPassword")
                    If oAttr IsNot Nothing Then
                        Try
                            Common.ProjectHtmlTargetPassword = oAttr.Value
                        Catch ex As Exception
                        End Try
                    End If

                    oAttr = node.Attributes("TouchAllScreens")
                    If oAttr IsNot Nothing Then
                        Try
                            Common.ProjectTouchAllScreens = oAttr.Value
                        Catch ex As Exception
                        End Try
                    End If

                ElseIf node.Name.Equals("MultiLanguageTranslations") Then
                    oAttr = node.Attributes("Languages")
                    If oAttr IsNot Nothing Then
                        Common.ProjectMultiLanguageTranslations = oAttr.Value
                    End If
                    oAttr = node.Attributes("SortColumn")
                    If oAttr IsNot Nothing Then
                        Common.StringsPoolSortColumn = oAttr.Value
                    End If

                    For Each oStringNode As XmlNode In node.ChildNodes
                        If oStringNode.NodeType = XmlNodeType.Whitespace Then Continue For
                        Dim aStrings(Common.ProjectMultiLanguageTranslations) As String
                        aStrings(0) = oStringNode.SelectSingleNode("Value").InnerText
                        If aStrings(0).StartsWith(vbCrLf) AndAlso aStrings(0).Substring(1).Trim = String.Empty Then
                            aStrings(0) = String.Empty
                        End If
                        Try
                            For Each oTransNode As XmlNode In oStringNode.SelectNodes("Translation")
                                If oTransNode.NodeType = XmlNodeType.Whitespace Then Continue For
                                Dim strString As String = oTransNode.InnerText
                                If strString.StartsWith(vbCrLf) AndAlso strString.Substring(1).Trim = String.Empty Then
                                    strString = String.Empty
                                End If
                                aStrings(oTransNode.Attributes("LangID").Value) = strString
                            Next
                        Catch ex As Exception
                        End Try
                        Dim oStringSet As New MultiLanguageStringSet
                        oStringSet.Strings = aStrings
                        oStringSet.StringID = oStringNode.Attributes("ID").Value
                        If oStringNode.Attributes("FontName") IsNot Nothing Then
                            oStringSet.FontName = oStringNode.Attributes("FontName").Value
                        End If
                        If oStringNode.Attributes("InUse") IsNot Nothing Then
                            oStringSet.InUse = oStringNode.Attributes("InUse").Value
                        End If
                        If oStringNode.Attributes("AutoWrap") IsNot Nothing Then
                            Boolean.TryParse(oStringNode.Attributes("AutoWrap").Value, oStringSet.AutoWrap)
                        End If
                        If oStringNode.Attributes("Dynamic") IsNot Nothing Then
                            Boolean.TryParse(oStringNode.Attributes("Dynamic").Value, oStringSet.Dynamic)
                        End If
                        If oStringNode.Attributes("AltID") IsNot Nothing Then
                            oStringSet.StringAltID = oStringNode.Attributes("AltID").Value
                        End If
                        If Not Common.ProjectStringPool.ContainsKey(oStringSet.StringID) Then
                            Common.ProjectStringPool.Add(oStringSet.StringID, oStringSet)
                        End If
                    Next

                ElseIf node.Name.Equals("WindowsLayout") Then

                Else
                    'TODO: Log
                    'MessageBox.Show(String.Format("Node type {0} is not allowed here.", node.Name))
                End If
            Next
            If blnAskFramework Then
                Return "ASKFRAMEWORK"
            Else
                Return String.Empty
            End If
        End Function

#End Region

#Region "Bitmaps"

        Public Shared ReadOnly Property Bitmaps As Collection
            Get
                Return _Bitmaps
            End Get
        End Property

        Public Shared Function GetBitmap(ByVal Name As String) As VGDDImage
            If Name = "" Then Return Nothing
            If _Bitmaps IsNot Nothing Then
                For Each b As VGDDImage In _Bitmaps
                    If b.Name = Name Then
                        Return b
                    End If
                Next
            End If
            Return Nothing
        End Function

        Public Shared Sub ClearBitmaps()
            For Each b As VGDDImage In _Bitmaps
                b.Dispose()
            Next
            _Bitmaps.Clear()
        End Sub

        Public Shared Function AddBitmap(ByVal BitmapName As String, ByVal FileName As String, ByRef Bitmap As Bitmap) As VGDDImage
            Return AddBitmap(BitmapName, FileName, Bitmap, "FLASH_VGDD")
        End Function

        Public Shared Function AddBitmap(ByVal BitmapName As String, ByVal FileName As String, ByRef Bitmap As Bitmap, ByVal Type_ As String) As VGDDImage
            'Dim BitmapName As String = Common.CleanName(Path.GetFileNameWithoutExtension(FileName))
            Dim bNew As VGDDImage = Nothing
            Try
                bNew = New VGDDImage(FileName, "FILE")
            Catch ex As Exception
            End Try
            If bNew Is Nothing OrElse bNew.Bitmap Is Nothing Then
                Return Nothing
            End If
            Select Case Type_
                Case "FLASH_VGDD"
                    bNew.Type = VGDDImage.PictureType.FLASH_VGDD
                Case "EXTERNAL"
                    bNew.Type = VGDDImage.PictureType.EXTERNAL
                Case "EXTERNAL_VGDD_BIN"
                    bNew.Type = VGDDImage.PictureType.EXTERNAL_VGDD_BIN
                Case "EXTERNAL_VGDD"
                    bNew.Type = VGDDImage.PictureType.EXTERNAL_VGDD
                Case "BINBMP_ON_SDFAT"
                    bNew.Type = VGDDImage.PictureType.BINBMP_ON_SDFAT
                Case Else
                    bNew.Type = VGDDImage.PictureType.FLASH
            End Select
            Bitmap = bNew.Bitmap
            For Each b As VGDDImage In _Bitmaps
                If b.Name = BitmapName AndAlso b.Bitmap IsNot Nothing Then
                    b.OrigBitmap = New Bitmap(bNew.Bitmap)
                    b._FileName = FileName
                    b.Type = bNew.Type
                    RaiseEvent BitmapAdded(bNew)
                    Return b
                End If
            Next
            bNew.Name = BitmapName
            _Bitmaps.Add(bNew, BitmapName)
            RaiseEvent BitmapAdded(bNew)
            Return bNew
        End Function

        Public Shared Function RemoveBitmap(ByVal BitmapName As String) As Boolean
            If _Bitmaps.Count < 1 Then Return False
            'Dim BitmapName As String = Common.CleanName(Path.GetFileNameWithoutExtension(BitmapName))
            RemoveBitmap = False
            If _Bitmaps.Contains(BitmapName) Then
                Dim b As VGDDImage = _Bitmaps(BitmapName)
                'Dim i As Integer = 1
                'Do
                '    b = _Bitmaps(i)
                '    If b.Name = BitmapName Then
                If b.Referenced Then
                    If MessageBox.Show("Are you sure you want to remove this bitmap? It is currently in use in your project.", "Confirm", MessageBoxButtons.YesNo) = vbNo Then
                        Exit Function
                    End If
                End If
                Try
                    RaiseEvent BitmapRemoved(b)
                    b.OrigBitmap = Nothing
                    _Bitmaps.Remove(b.Name)
                    RemoveBitmap = True
                Catch ex As Exception
                    Exit Function
                End Try
                'Continue Do
                '    End If
                'i += 1
                'Loop While i <= _Bitmaps.Count
            End If
        End Function

        Public Shared Function CalculateControlGraphicsPath(ByVal bitmap As Bitmap, ByVal TransparentColour As Color, ByVal offsetX As Integer, ByVal offsetY As Integer) As System.Drawing.Drawing2D.GraphicsPath
            If bitmap Is Nothing Then Return Nothing
            Dim graphicsPath As New System.Drawing.Drawing2D.GraphicsPath
            Dim PixelColour As Long, TransColor As Long = TransparentColour.ToArgb
            If TransparentColour = Nothing Then TransparentColour = bitmap.GetPixel(0, 0)
            Dim colOpaquePixel As Integer = 0
            For row As Integer = 0 To bitmap.Height - 1
                colOpaquePixel = 0
                For col As Integer = 0 To bitmap.Width - 1
                    PixelColour = bitmap.GetPixel(col, row).ToArgb
                    If PixelColour <> TransColor Then
                        colOpaquePixel = col
                        Dim colNext As Integer = col
                        For colNext = colOpaquePixel To bitmap.Width - 2
                            If bitmap.GetPixel(colNext, row).ToArgb = TransColor Then
                                Exit For
                            End If
                        Next
                        graphicsPath.AddRectangle(New Rectangle(colOpaquePixel + offsetX, row + offsetY, colNext - colOpaquePixel + 1, 1))
                        col = colNext ' - 1
                    Else
                        'Debug.Print("gu")
                    End If
                Next
            Next
            Return graphicsPath
        End Function

#End Region

        Public Shared Function ExtractDefaultBitmap(ByVal strBitmapFileName As String, ByVal Assembly As Assembly) As String
            Dim strBitmapName As String = Path.GetFileNameWithoutExtension(strBitmapFileName)
            Dim strBitmapPathName As String = Path.Combine(Common.VGDDProjectPath, strBitmapFileName)
            If strBitmapPathName = strBitmapFileName Then
                strBitmapPathName = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), strBitmapFileName)
            End If
            If Not File.Exists(strBitmapPathName) Then
                If Not ExtractResourceToFile(strBitmapFileName, strBitmapPathName, Assembly) Then
                    MessageBox.Show("Cannot extract " & strBitmapFileName & ". Please choose a different project path", "Error extracting default VuMeter bitmap")
                    Return String.Empty
                End If
            End If
            If Common.GetBitmap(strBitmapName) Is Nothing Then
                Common.AddBitmap(strBitmapName, strBitmapPathName, Nothing)
                'MessageBox.Show("""VuMeterDefault"" bitmap has been automatically added to your VGDD project.", "Warning")
            End If
            Return strBitmapName
        End Function

        Public Shared Function BitmapFromResource(ByVal ResourceName As String, ByVal Assembly As Assembly) As Bitmap
            Dim oStream As Stream = VirtualFabUtils.Utils.GetResourceStream(ResourceName, Assembly)
            If oStream Is Nothing Then
                Return Nothing
            End If
            Return New Bitmap(oStream)
        End Function

        Public Shared Function GetResourceStream(ByVal ResourceName As String, ByVal oAssembly As Assembly) As Stream
            Dim oStream As Stream
            oStream = VirtualFabUtils.Utils.GetResourceStream(ResourceName, oAssembly)
#If Not PlayerMonolitico Then
            If oStream Is Nothing Then
                oStream = VirtualFabUtils.Utils.GetResourceStream(ResourceName, MplabX.MplabXFilesAssembly)
            End If
#End If
            Return oStream
        End Function

#Region "Fonts"
        Public Shared Function GetFont(ByVal FontName As String, ByVal UsedBy As Object) As VGDDFont
            GetFont = Nothing
            For Each oFont As VGDDFont In _Fonts
                If FontName = oFont.Name _
                    OrElse FontName = FontToString(oFont.Font) Then
                    GetFont = oFont
                    If UsedBy IsNot Nothing AndAlso Not GetFont.UsedBy.Contains(UsedBy) Then
                        GetFont.UsedBy.Add(UsedBy)
                    End If
                    Exit Function
                End If
            Next
        End Function

        Public Shared Sub AddFont(ByVal NewFont As VGDDFont, ByVal UsedBy As Object)
            If NewFont Is Nothing Then Return
            If NewFont.Name = "" Then NewFont.Name = FontToString(NewFont.Font)
            Dim oFont As VGDDFont = GetFont(NewFont.Name, Nothing)
            If oFont Is Nothing Then
                'If _Fonts.Count = 0 Then
                '    NewFont.IsGOLFontDefault = True
                'End If
                _Fonts.Add(NewFont)
                RaiseEvent FontAdded(NewFont)
                oFont = NewFont
            Else
                oFont = NewFont
            End If
            If UsedBy IsNot Nothing AndAlso Not oFont.UsedBy.Contains(UsedBy) Then
                oFont.UsedBy.Add(UsedBy)
            End If
        End Sub

        Public Shared Function RemoveFont(ByVal FontToRemove As VGDDFont) As Boolean
            If _Fonts.Contains(FontToRemove) Then
                Dim Cancel As Boolean = False
                RaiseEvent FontRemoving(FontToRemove, Cancel)
                If Cancel = False Then
                    _Fonts.Remove(FontToRemove)
                    Return True
                End If
            End If
            Return False
        End Function

        Public Shared Function FontToString(ByVal Font As Font) As String
            If Font Is Nothing Then Return String.Empty
            Dim f As String = String.Format("{0}{1}{2}", Font.FontFamily.Name, Font.Style.ToString.Replace(",", ""), Math.Round(Font.Size, 0).ToString) '.Replace(",", "").Replace(".", "")).Replace(" ", "")
            FontToString = ""
            For Each c As Char In f
                If Char.IsLetterOrDigit(c) Then
                    FontToString &= c
                End If
            Next
        End Function
#End Region

#Region "Schemes"
        Public Shared Function NewScheme(ByVal FromScheme As VGDDScheme) As VGDDScheme
            Dim oScheme As VGDDScheme
            Dim i As Integer = 0
            Dim strName As String = ""
            Do While True
                If i = 0 Then
                    strName = "Default"
                Else
                    strName = "Scheme" & i
                End If
                If Not _Schemes.ContainsKey(strName) Then Exit Do
                'For Each oScheme In _Schemes.Values
                '    If oScheme.Name = strName Then
                '        strName = ""
                '        Exit For
                '    End If
                'Next
                'If strName <> "" Then Exit Do
                i += 1
            Loop
            If FromScheme IsNot Nothing Then
                oScheme = New VGDDScheme
                oScheme.Name = strName
                With oScheme
                    .Color0 = FromScheme.Color0
                    .Color1 = FromScheme.Color1
                    .Colordisabled = FromScheme.Colordisabled
                    .Commonbkcolor = FromScheme.Commonbkcolor
                    .Embossdkcolor = FromScheme.Embossdkcolor
                    .Embossltcolor = FromScheme.Embossltcolor
                    .Textcolor0 = FromScheme.Textcolor0
                    .Textcolor1 = FromScheme.Textcolor1
                    .Textcolordisabled = FromScheme.Textcolordisabled
                    .Font = FromScheme.Font
                End With
                _Schemes.Add(oScheme.Name, oScheme)
            Else
                oScheme = CreateScheme(strName)
            End If
            Return oScheme
        End Function

        Public Shared Function CreateScheme(ByVal Name As String) As VGDDScheme
            Dim NewScheme As New VGDDScheme
            With NewScheme
                .Name = Name
                '.Commonbkcolor = Color.Black
                '.Color0 = Color.FromArgb(&H0, &H0, &H80) 'Color.DarkBlue    'Control BgColor
                '.Color1 = Color.FromArgb(&H0, &H0, &HF8) 'Color.Blue
                '.Textcolor0 = Color.FromArgb(&HF8, &H80, &H0) 'Color.Orange
                '.Textcolor1 = Color.FromArgb(&H0, &H0, &H0)
                '.Embossdkcolor = Color.FromArgb(&H18, 0, &HE0)
                '.Embossltcolor = Color.FromArgb(&H0, &H80, &HC0) 'Color.LightBlue
                '.Textcolordisabled = Color.FromArgb(&HB8, &HB8, &HB8) 'Color.Gray
                '.Colordisabled = Color.FromArgb(&H0, &HFC, &H0) 'lightgreen
                .Font = New VGDDFont(New Font("Gentium Basic", 11.25, FontStyle.Regular))
                AddFont(.Font, Nothing)
            End With
            _Schemes.Add(NewScheme.Name, NewScheme)
            Return NewScheme
        End Function

        Public Shared Function GetScheme(ByVal Name As String) As VGDDScheme
            If Name = String.Empty Then
                For Each oScheme As VGDDScheme In _Schemes.Values
                    Return oScheme
                    Exit Function
                Next
            End If
            'For Each oScheme As VGDDScheme In _Schemes.Values
            '    If oScheme.Name = Name Then
            '        If Not oScheme.Font.UsedBy.Contains(caller) Then
            '            oScheme.Font.UsedBy.Add(caller)
            '        End If
            '        Return oScheme
            '    End If
            'Next
            'If _Schemes.Count > 0 Then Return _Schemes(0)
            'Dim RetScheme As VGDDScheme = Nothing ' CreateScheme(Name)
            'Return RetScheme
            If _Schemes.ContainsKey(Name) Then
                Return _Schemes(Name)
            ElseIf _Schemes.ContainsKey("Default") Then
                Return _Schemes("Default")
            ElseIf _Schemes.ContainsKey("DesignScheme") Then
                Return _Schemes("DesignScheme")
            End If
            Return Nothing
        End Function

        Public Class SchemesOptionConverter
            Inherits StringConverter

            Public Overloads Overrides Function GetStandardValuesSupported( _
                ByVal context As ITypeDescriptorContext) As Boolean
                Return True 'True tells the propertygrid to display a combobox
            End Function
            Public Overloads Overrides Function GetStandardValuesExclusive( _
                ByVal context As ITypeDescriptorContext) As Boolean
                Return True 'True makes the combobox select only. 
                'False allows free text entry.
            End Function
            Public Overloads Overrides Function GetStandardValues( _
            ByVal context As ITypeDescriptorContext) As StandardValuesCollection
                Dim aSchemeNames(_Schemes.Count - 1) As String
                Dim i As Integer = 0
                For Each oScheme As VGDDScheme In _Schemes.Values
                    aSchemeNames(i) = oScheme.Name
                    i += 1
                Next
                Return New StandardValuesCollection(aSchemeNames)
            End Function
        End Class

        Public Class PaletteOptionConverter
            Inherits StringConverter

            Public Overloads Overrides Function GetStandardValuesSupported(ByVal context As ITypeDescriptorContext) As Boolean
                Return True 'True tells the propertygrid to display a combobox
            End Function
            Public Overloads Overrides Function GetStandardValuesExclusive(ByVal context As ITypeDescriptorContext) As Boolean
                Return True 'True makes the combobox select only. 
                'False allows free text entry.
            End Function
            Public Overloads Overrides Function GetStandardValues(ByVal context As ITypeDescriptorContext) As StandardValuesCollection
                Dim colPaletteNames As New Collection
                For Each oScheme As VGDDScheme In Common._Schemes.Values
                    If oScheme.Palette IsNot Nothing Then
                        colPaletteNames.Add(oScheme.Palette.Name)
                    End If
                Next
                Return New StandardValuesCollection(colPaletteNames)
            End Function
        End Class
#End Region

#Region "utils"
        Public Shared Function TestJava(ByVal strJavaCommand As String) As String
            Dim myProcess As Process = New Process()
            Dim strOut As String = String.Empty
            Try
                myProcess.StartInfo.FileName = strJavaCommand
                myProcess.StartInfo.Arguments = "-version"
                myProcess.StartInfo.UseShellExecute = False
                myProcess.StartInfo.CreateNoWindow = True
                myProcess.StartInfo.RedirectStandardInput = True
                myProcess.StartInfo.RedirectStandardOutput = True
                myProcess.StartInfo.RedirectStandardError = True
                myProcess.Start()
                Do While Not myProcess.HasExited
                    System.Threading.Thread.Sleep(250)
                    Application.DoEvents()
                Loop
                Dim oSrStdOut As StreamReader = myProcess.StandardOutput
                strOut = oSrStdOut.ReadToEnd() & vbCrLf
                oSrStdOut.Close()

                Dim oSrStdErr As StreamReader = myProcess.StandardError
                strOut &= oSrStdErr.ReadToEnd()
                oSrStdErr.Close()
                If Not myProcess.HasExited Then
                    myProcess.Kill()
                End If
                myProcess.Close()
                Dim strJavaVersion As String = String.Empty
                If strOut.ToLower.Contains("java version ") Then
                    strJavaVersion = strOut.Substring(strOut.IndexOf("java version") + 12).Trim
                    strJavaVersion = strJavaVersion.Substring(0, strJavaVersion.IndexOf(vbCr) - 1).Replace("""", "").Trim
                End If
                If strJavaVersion <> String.Empty Then
                    TestJava = "OK: Java Version " & strJavaVersion & " found"
                Else
                    TestJava = "KO: No Java found: " & strOut
                End If

            Catch ex As Exception
                TestJava = "KO: Error looking for Java: " & ex.Message
            End Try
        End Function

        Public Shared Function XmlNodeNoNull(ByRef XmlNode As XmlNode, ByVal xPath As String) As String
            Dim oNode As XmlNode = XmlNode.SelectSingleNode(xPath)
            If oNode Is Nothing Then
                Return String.Empty
            Else
                Return oNode.InnerText()
            End If
        End Function

        Public Shared Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not PROPSTOREMOVE.Contains(" " & pd.Name & " ") Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

        Public Const PROPSTOREMOVE As String = " HasChildWidgets Anchor ContextMenu FootPrintHEAP FootPrintRAM FootPrintROM Instances Demolimit " & _
        "Visible TabIndex ApplicationSettings DataBindings AccessibleDescription AccessibleName " & _
        "AccessibilityObject AccessibleRole AllowDrop AutoSize AutoSizeMode " & _
        "AutoEllipsis AutoScrollOffset BackgroundImage BackgroundImageLayout Cursor ContextMenuStrip " & _
        "CausesValidation Capture DialogResult DisplayRectangle Dock Enabled FlatAppearance FlatStyle " & _
        "ForeColor GenerateMember ImageAlign ImageIndex ImageKey ImageList Margin MaximumSize " & _
        "MinimumSize Modifiers Region Tag Padding RightToLeft TabStop TextImageRelation " & _
        "UseCompatibleTextRendering UseMnemonic UseVisualStyleBackColor UseWaitCursor " & _
        "ShortcutsEnabled ScrollBars ReadOnly PasswordChar Multiline MaxLength Lines ImeMode HideSelection " & _
        "CharacterCasing BorderStyle AutoCompleteSource AutoCompleteMode AutoCompleteCustomSource AcceptsTab AcceptsReturn " & _
        "WordWrap UseSystemPasswordChar Appearance AutoCheck CheckAlign CheckState ThreeState " & _
        "AcceptButton AutoScaleMode AutoScaleBaseSize AutoScroll AutoScrollMargin AutoScrollMinSize AutoValidate CancelButton " & _
        "ControlBox DoubleBuffered FormBorderStyle HelpButton Icon IsMdiContainer KeyPreview MainMenuStrip " & _
        "MaximizeBox MinimizeBox Opacity RightToLeftLayout ShowIcon ShowInTaskBar SizeGripStyle StartPosition " & _
        "TopMost TransparencyKey WindowState Culture ColumnWidth DataSource DisplayMember DrawMode FormatString FormattingEnabled " & _
        "HorizontalExtent HorizontalScrollbar ItemHeight IntegralHeight MultiColumn ScrollAlwaysVisible SelectionMode Sorted UseTabStops ValueMember "

        Public Shared Function Reindent(ByRef strIn As String, ByVal InitialLevel As Integer) As String
            Dim strOut As String = ""
            Try
                Dim intLevel As Integer = InitialLevel
                Dim blnFirstSwitch As Boolean = False, aSwitchLevels(40) As Integer
                Dim blnComment As Boolean, blnInlineComment As Boolean

                strIn = strIn.Replace(vbCrLf, vbLf).Replace(vbLf, vbCrLf)
                For Each strLine As String In strIn.Split(vbCrLf)
                    aSwitchLevels(intLevel + 1) = 0
                    blnInlineComment = False
                    If strLine.Trim.StartsWith("//") Then
                        blnInlineComment = True
                    End If
                    If strLine.Contains("/*") Then
                        blnComment = True
                    End If
                    If strLine.Contains("*/") Then
                        blnComment = False
                    End If
                    If Not blnComment And Not blnInlineComment Then
                        If strLine.Contains("}") Then
                            If intLevel > 0 Then intLevel -= 1
                            If aSwitchLevels(intLevel) = 1 Then
                                If intLevel > 0 Then intLevel -= 1
                            End If
                        ElseIf strLine.Contains("switch") Then
                            blnFirstSwitch = True
                            aSwitchLevels(intLevel + 1) = 1
                        ElseIf strLine.Trim.EndsWith(":") Then
                            If Not blnFirstSwitch Then
                                If intLevel > 0 Then intLevel -= 1
                            End If
                            blnFirstSwitch = False
                        End If
                    End If

                    strOut &= New String(" ", intLevel * 4) & strLine.Trim & vbCrLf

                    If Not blnComment And Not blnInlineComment Then
                        If (strLine.Contains("{") AndAlso Not strLine.Contains("}")) Or strLine.Trim.EndsWith(":") Then
                            intLevel += 1
                        End If
                    End If
                Next
                Do While strOut.EndsWith(vbCrLf)
                    strOut = strOut.Substring(0, strOut.Length - 2)
                Loop

            Catch ex As Exception
                strOut = strIn
            End Try
            Return strOut
        End Function

#End Region

#Region "GOL"
        Public Class GOL
            Const COSINETABLEENTRIES As Byte = 90
            ' Cosine table used to calculate angles when rendering circular objects and  arcs  
            ' Make cosine values * 256 instead of 100 for easier math later
            Public Shared _CosineTable As Int16() = { _
                   256, 256, 256, 256, 255, 255, 255, 254, 254, 253, _
                   252, 251, 250, 249, 248, 247, 246, 245, 243, 242, _
                   241, 239, 237, 236, 234, 232, 230, 228, 226, 224, _
                   222, 219, 217, 215, 212, 210, 207, 204, 202, 199, _
                   196, 193, 190, 187, 184, 181, 178, 175, 171, 168, _
                   165, 161, 158, 154, 150, 147, 143, 139, 136, 132, _
                   128, 124, 120, 116, 112, 108, 104, 100, 96, 92, _
                   88, 83, 79, 75, 71, 66, 62, 58, 53, 49, _
                   44, 40, 36, 31, 27, 22, 18, 13, 9, 4, _
                   0}
            Public Const GETSINE As Byte = 0
            Public Const GETCOSINE As Byte = 1

            Public Shared Function GetSineCosine(ByVal degAngle As Int16, ByVal type As Byte) As Int16
                If (degAngle >= COSINETABLEENTRIES * 3) Then
                    degAngle -= COSINETABLEENTRIES * 3
                    GetSineCosine = (IIf(type = GETSINE, -(_CosineTable(degAngle)), (_CosineTable(COSINETABLEENTRIES - degAngle))))
                ElseIf (degAngle >= COSINETABLEENTRIES * 2) Then
                    degAngle -= COSINETABLEENTRIES * 2
                    GetSineCosine = IIf(type = GETSINE, -(_CosineTable((COSINETABLEENTRIES - degAngle))), -(_CosineTable(degAngle)))
                ElseIf (degAngle >= COSINETABLEENTRIES) Then
                    degAngle -= COSINETABLEENTRIES
                    GetSineCosine = IIf(type = GETSINE, (_CosineTable(degAngle)), -(_CosineTable(COSINETABLEENTRIES - degAngle)))
                Else
                    GetSineCosine = IIf(type = GETSINE, (_CosineTable(COSINETABLEENTRIES - degAngle)), (_CosineTable(degAngle)))
                End If
            End Function

            Public Shared Sub GetCirclePoint(ByVal radius As Int16, ByVal angle As Int16, ByRef x As Int16, ByRef y As Int16)
                'Dim radAngle As Single = GetRadian(angle)
                'Dim xg As Int16 = radius * Math.Cos(radAngle)
                'Dim yg As Int16 = radius * Math.Sin(radAngle)
                'Return

                Dim rad As UInt32
                Dim ang As Int16
                Dim temp As UInt32

                While angle < 0
                    angle += 360 ' if angle is neg, convert to pos equivalent
                End While

                angle = angle Mod 360

                ang = angle Mod 45
                If ((angle \ 45) And 1) Then
                    ang = 45 - ang
                End If

                rad = radius
                ' there is a shift by 8 bits here since this function assumes a shift on the calculations for accuracy
                ' and to avoid long and complex multiplications.
                temp = GetSineCosine(ang, GETCOSINE)
                x = ((temp << 8) * rad) >> 16

                temp = GetSineCosine(ang, GETSINE)
                y = ((temp << 8) * rad) >> 16

                If (((angle > 45) And (angle < 135)) Or ((angle > 225) And (angle < 315))) Then
                    temp = x
                    x = y
                    y = temp
                End If

                If ((angle > 90) And (angle < 270)) Then
                    x = -x
                End If

                If ((angle > 180) And (angle < 360)) Then
                    y = -y
                End If
                'Debug.Print(String.Format("{8} {6}->{7} - {4} {5}   x={0} xg={1}   y={2} yg={3}", x, xg, y, yg, Math.Abs(x - xg), Math.Abs(y - yg), angle, ang, (angle \ 45) And 1))
            End Sub
        End Class
#End Region

#Region "Enums"

        Public Enum TextCDeclType
            ConstXcharArray = 0
            RamXcharArray = 1
            ExternXcharPointer = 2
            ExternRamXCharArray = 3
        End Enum

        Public Enum MatrixCDeclType
            ConstXcharArray = 0
            RamXcharArray = 1
            ExternXcharPointer = 2
            ExternRamXCharArray = 3
        End Enum

        Public Enum EditTextCDeclType
            RamXcharArray = 1
            ExternXcharPointer = 2
            ExternRamXCharArray = 3
            NULL = 4
        End Enum

        Public Enum EnabledState
            Enabled = 1
            Disabled = 0
        End Enum

        Public Enum HorizAlign
            Left
            Right
            Center
        End Enum

        Public Enum VertAlign
            Top
            Bottom
            Center
        End Enum

        Public Enum Orientation
            Horizontal
            Vertical
        End Enum

        Public Enum SliderType
            Slider
            ScrollBar
        End Enum

        Public Enum MeterType
            Normal
            Ring
        End Enum

        Public Enum MeterTypeMLA4
            GFX_GOL_METER_WHOLE_TYPE
            GFX_GOL_METER_HALF_TYPE
            GFX_GOL_METER_QUARTER_TYPE
        End Enum

        Public Enum LineType
            SOLID_LINE = 0
            DOTTED_LINE = 1
            DASHED_LINE = 4
        End Enum

        Public Enum ThickNess
            NORMAL_LINE = 0
            THICK_LINE = 1
        End Enum

#End Region

        Public Shared Sub TouchScreen(ByVal ScreenName As String)
            If Common.aScreens.ContainsKey(ScreenName.ToUpper) Then
                Dim oScreenAttr As VGDD.VGDDScreenAttr = Common.aScreens(ScreenName.ToUpper)
                Dim hc As Object = oScreenAttr.Hc
                If hc IsNot Nothing Then
                    Try
                        If Not hc.Text.EndsWith("*") Then
                            hc.Text &= "*"
                        End If
                        hc.Loader.ContentIsChanged()
                        Common.ProjectChanged = True
                        If Not Common.ProjectLoading Then
                            hc.show()
                            hc.Focus()
                        End If
                        Application.DoEvents()
                    Catch ex As Exception
                    End Try
                End If
            End If
        End Sub

    End Class


#Region "BitmapFileChooser"

    Public Class VGDDBitmapFileChooser
        Inherits System.Drawing.Design.UITypeEditor

        Public Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
            Dim oBitmapChooser As New frmBitmapChooser
            With oBitmapChooser
                .FileName = value
                If .txtBinPath.Text = "" Then .txtBinPath.Text = Common.BitmapsBinPath
                .ShowDialog()
                If .ChosenBitmap IsNot Nothing Then
                    Return .ChosenBitmap.Name
                End If
            End With
            Return MyBase.EditValue(context, provider, value)
        End Function

        Public Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
            Return System.Drawing.Design.UITypeEditorEditStyle.Modal
        End Function
    End Class
#End Region

#Region "FontChooser"

#If Not PlayerMonolitico Then

    Public Class VGDDFontChooser
        Inherits System.Drawing.Design.UITypeEditor
        Public Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
            Dim oFontChooser As New frmFontChooser
            With oFontChooser
                .SelectedFont = value
                .ShowDialog()
                If .SelectedFont IsNot Nothing Then
                    Return .SelectedFont
                End If
            End With
            Return MyBase.EditValue(context, provider, value)
        End Function

        Public Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
            Return System.Drawing.Design.UITypeEditorEditStyle.Modal
        End Function
    End Class

    Public Class VGDDFontNameChooser
        Inherits System.Drawing.Design.UITypeEditor
        Public Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
            Dim oFontChooser As New frmFontChooser
            With oFontChooser
                .SelectedFont = Common.GetFont(value, context.Instance)
                .ShowDialog()
                If .SelectedFont IsNot Nothing Then
                    Return .SelectedFont.Name
                End If
            End With
            Return MyBase.EditValue(context, provider, value)
        End Function

        Public Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
            Return System.Drawing.Design.UITypeEditorEditStyle.Modal
        End Function
    End Class
#End If

#End Region


    <Serializable()> _
    Public Class Segment

        Private _ValueFrom As Int16
        Private _ValueTo As Int16
        Private _SegmentColour As Color

        Public Sub New()

        End Sub

        Public Sub New(ByRef oSegmentNode As XmlNode)
            _ValueFrom = oSegmentNode.Attributes("ValueFrom").Value
            _ValueTo = oSegmentNode.Attributes("ValueTo").Value
            _SegmentColour = Color.FromArgb(oSegmentNode.Attributes("SegmentColour").Value)
        End Sub

        Public Sub New(ByVal ValueFrom As Integer, ByVal ValueTo As Integer, ByVal SegmentColour As Color)
            _ValueFrom = ValueFrom
            _ValueTo = ValueTo
            _SegmentColour = SegmentColour
        End Sub

        '<XmlElement("ValueFrom")> _
        Public Property ValueFrom As Int16
            Get
                Return _ValueFrom
            End Get
            Set(ByVal value As Int16)
                _ValueFrom = value
            End Set
        End Property

        '<XmlElement("ValueTo")> _
        Public Property ValueTo As Int16
            Get
                Return _ValueTo
            End Get
            Set(ByVal value As Int16)
                _ValueTo = value
            End Set
        End Property

        '<XmlElement("SegmentColour")> _
        Public Property SegmentColour As Color
            Get
                Return _SegmentColour
            End Get
            Set(ByVal value As Color)
                _SegmentColour = value
            End Set
        End Property

    End Class

    '<System.Xml.Serialization.XmlRoot("VGDDEventsCollection")> _
    <Serializable()> _
    Public Class SegmentsCollection
        Inherits CollectionBase
        Implements IList ', IXmlSerializable

        Public Sub New()

        End Sub

        Public Sub New(ByRef XmlParent As XmlNode)
            For Each oSegmentNode As XmlNode In XmlParent.ChildNodes
                If oSegmentNode.NodeType = XmlNodeType.Whitespace Then Continue For
                Dim oSegment As New Segment
                With oSegment
                    If oSegmentNode.Attributes("ValueFrom") IsNot Nothing Then
                        .ValueFrom = oSegmentNode.Attributes("ValueFrom").Value
                    End If
                    If oSegmentNode.Attributes("ValueTo") IsNot Nothing Then
                        .ValueTo = oSegmentNode.Attributes("ValueTo").Value
                    End If
                    If oSegmentNode.Attributes("SegmentColour") IsNot Nothing Then
                        .SegmentColour = Color.FromArgb(oSegmentNode.Attributes("SegmentColour").Value)
                    End If
                End With
                Me.Add(oSegment)
            Next
        End Sub

        Default Public ReadOnly Property Item(ByVal index As Integer) As Segment
            Get
                Return MyBase.List(index)
            End Get
        End Property

        Public Sub Add(ByRef oSegment As Segment)
            MyBase.List.Add(oSegment)
        End Sub

        Public Overloads ReadOnly Property List()
            Get
                Return MyBase.List
            End Get
        End Property

        Public Sub ToXml(ByRef XmlParent As XmlNode)
            Dim XmlDoc As XmlDocument = XmlParent.OwnerDocument
            For Each oSegment As Segment In MyBase.List
                Dim oEventNode As XmlElement = XmlDoc.CreateElement("Segment")
                Dim oAttr As XmlAttribute

                oAttr = XmlDoc.CreateAttribute("ValueFrom")
                oAttr.Value = oSegment.ValueFrom
                oEventNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("ValueTo")
                oAttr.Value = oSegment.ValueTo
                oEventNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("SegmentColour")
                oAttr.Value = oSegment.SegmentColour.ToArgb
                oEventNode.Attributes.Append(oAttr)

                XmlParent.AppendChild(oEventNode)
            Next
        End Sub

        'Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
        '    Return Nothing
        'End Function

        'Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements System.Xml.Serialization.IXmlSerializable.ReadXml

        'End Sub

        'Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements System.Xml.Serialization.IXmlSerializable.WriteXml
        '    With writer
        '        .WriteStartElement("Object")
        '        .WriteAttributeString("type", Me.GetType.AssemblyQualifiedName)
        '        For Each oSegment As GaugeSegment In MyBase.List
        '            .WriteStartElement("Segment")
        '            .WriteAttributeString("ValueFrom", oSegment.ValueFrom)
        '            .WriteAttributeString("ValueTo", oSegment.ValueTo)
        '            .WriteAttributeString("SegmentColour", oSegment.SegmentColour.ToArgb)
        '            .WriteEndElement()
        '        Next
        '        .WriteEndElement()
        '    End With
        'End Sub
    End Class

    <AttributeUsage(AttributeTargets.All)>
    Public Class AttributeTextStringIDName
        Inherits Attribute
        ' Private fields. 
        Private _TextStringIdName As String = "TextStringIDName"

        Public Sub New(ByVal ID As String)
            Me._TextStringIdName = ID
        End Sub

        Public Overridable Property TextStringIdName() As String
            Get
                Return _TextStringIdName
            End Get
            Set(ByVal value As String)
                _TextStringIdName = value
            End Set
        End Property
    End Class

    <AttributeUsage(AttributeTargets.All)>
    Public Class GOLRange
        Inherits Attribute
        ' Private fields. 
        Private _Min As Integer
        Private _Max As Integer
        Private _MaxPropertyName As String

        Public Sub New(ByVal Min As Integer, ByVal Max As Integer)
            Me._Min = Min
            Me._Max = Max
        End Sub

        Public Sub New(ByVal Min As Integer, ByVal Max As Integer, ByVal MaxPropertyName As String)
            Me._Min = Min
            Me._Max = Max
            Me._MaxPropertyName = MaxPropertyName
        End Sub

        Public Overridable Property Min() As Integer
            Get
                Return _Min
            End Get
            Set(ByVal value As Integer)
                _Min = value
            End Set
        End Property

        Public Overridable Property Max() As Integer
            Get
                Return _Max
            End Get
            Set(ByVal value As Integer)
                _Max = value
            End Set
        End Property

        Public Overridable Property MaxPropertyName() As String
            Get
                Return _MaxPropertyName
            End Get
            Set(ByVal value As String)
                _MaxPropertyName = value
            End Set
        End Property
    End Class

    Public Class VGDDKey
        Private _Key As String
        Private _KeyAlternate As String
        Private _KeyShift As String
        Private _KeyShiftAlternate As String
        Private _KeyCommand As String = String.Empty

        Public Property Key As String
            Get
                Return _Key
            End Get
            Set(ByVal value As String)
                _Key = value
            End Set
        End Property

        Public Property KeyAlternate As String
            Get
                Return _KeyAlternate
            End Get
            Set(ByVal value As String)
                _KeyAlternate = value
            End Set
        End Property

        Public Property KeyShift As String
            Get
                Return _KeyShift
            End Get
            Set(ByVal value As String)
                _KeyShift = value
            End Set
        End Property

        Public Property KeyShiftAlternate As String
            Get
                Return _KeyShiftAlternate
            End Get
            Set(ByVal value As String)
                _KeyShiftAlternate = value
            End Set
        End Property

        Public Property KeyCommand As String
            Get
                Return _KeyCommand
            End Get
            Set(ByVal value As String)
                _KeyCommand = value
            End Set
        End Property

    End Class

    Public Class VGDDKeyCollection
        Inherits CollectionBase
        Implements IList, Common.IVGDDXmlSerialize

        Public Sub New()

        End Sub

        Default Public ReadOnly Property Item(ByVal index As Integer) As VGDDKey
            Get
                If index <= MyBase.Count Then
                    Return MyBase.List(index)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Sub Add(ByRef oKey As VGDDKey)
            MyBase.List.Add(oKey)
        End Sub

        Public Overloads ReadOnly Property List()
            Get
                Return MyBase.List
            End Get
        End Property

        Public Sub ToXml(ByRef XmlParent As System.Xml.XmlNode) Implements Common.IVGDDXmlSerialize.ToXml
            Dim XmlDoc As XmlDocument = XmlParent.OwnerDocument
            For Each oKey As VGDDKey In MyBase.List
                Dim oKeyNode As XmlElement = XmlDoc.CreateElement("Key")
                Dim oAttr As XmlAttribute

                oAttr = XmlDoc.CreateAttribute("KeyText")
                oAttr.Value = oKey.Key
                oKeyNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("KeyCommand")
                oAttr.Value = oKey.KeyCommand
                oKeyNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("KeyAlternate")
                oAttr.Value = oKey.KeyAlternate
                oKeyNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("KeyShift")
                oAttr.Value = oKey.KeyShift
                oKeyNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("KeyShiftAlternate")
                oAttr.Value = oKey.KeyShiftAlternate
                oKeyNode.Attributes.Append(oAttr)

                XmlParent.AppendChild(oKeyNode)
            Next

        End Sub

        Public Function FromXml(ByVal oNode As System.Xml.XmlNode) As Object Implements Common.IVGDDXmlSerialize.FromXml
            For Each oKeyNode As XmlNode In oNode.ChildNodes
                If oKeyNode.NodeType = XmlNodeType.Whitespace Then Continue For
                Dim oKey As New VGDDKey
                With oKey
                    If oKeyNode.Attributes("KeyText") IsNot Nothing Then
                        .Key = oKeyNode.Attributes("KeyText").Value
                    End If
                    If oKeyNode.Attributes("KeyCommand") IsNot Nothing Then
                        .KeyCommand = oKeyNode.Attributes("KeyCommand").Value
                    End If
                    If oKeyNode.Attributes("KeyAlternate") IsNot Nothing Then
                        .KeyAlternate = oKeyNode.Attributes("KeyAlternate").Value
                    End If
                    If oKeyNode.Attributes("KeyShift") IsNot Nothing Then
                        .KeyShift = oKeyNode.Attributes("KeyShift").Value
                    End If
                    If oKeyNode.Attributes("KeyShiftAlternate") IsNot Nothing Then
                        .KeyShiftAlternate = oKeyNode.Attributes("KeyShiftAlternate").Value
                    End If
                End With
                Me.Add(oKey)
            Next
            Return Me
        End Function
    End Class

    Public Class MultiLanguageStringSet
        Private _StringID As Integer
        Private _Strings() As String
        Private _FontName As String
        Private _InUse As Boolean = False
        Private _AutoWrap As Boolean = False
        Private _Dynamic As Boolean = False
        Private _UsedBy As String = String.Empty
        Private _StringAltID As String

        Public Sub AutoWrapStrings(ByVal oFont As Font, ByVal lineWidth As Integer)
            Using image = New Bitmap(1, 1)
                Using g = Graphics.FromImage(image)
                    'g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
                    For i As Integer = 0 To _Strings.Length - 1
                        Dim strPool As String = _Strings(i)
                        If strPool Is Nothing Then strPool = String.Empty
                        Do While strPool.Contains(" " & vbCrLf)
                            strPool = strPool.Replace(" " & vbCrLf, vbCrLf)
                        Loop
                        Do While strPool.Contains(vbCrLf & " ")
                            strPool = strPool.Replace(vbCrLf & " ", vbCrLf)
                        Loop
                        strPool = strPool.Replace(vbCrLf, " ")
                        Const SPACE As String = " "
                        Dim words As String() = strPool.Split(New String() {SPACE}, StringSplitOptions.None)
                        Dim spaceWidth As Single = g.MeasureString(SPACE, oFont).Width ', Integer.MaxValue, StringFormat.GenericTypographic).Width
                        Dim spaceLeft As Single = lineWidth, wordWidth As Single
                        strPool = String.Empty
                        For Each word As String In words
                            wordWidth = g.MeasureString(word, oFont, Integer.MaxValue, StringFormat.GenericTypographic).Width
                            If wordWidth + spaceWidth > spaceLeft Then
                                If strPool.EndsWith(SPACE) Then strPool = strPool.Substring(0, strPool.Length - 1)
                                strPool &= vbCrLf
                                spaceLeft = lineWidth - wordWidth
                            Else
                                spaceLeft -= (wordWidth + spaceWidth)
                            End If
                            strPool &= word & SPACE
                        Next
                        _Strings(i) = strPool.Trim
                    Next
                End Using
            End Using
        End Sub

        Public Property StringID As Integer
            Get
                Return _StringID
            End Get
            Set(ByVal value As Integer)
                _StringID = value
            End Set
        End Property

        Public Property StringAltID As String
            Get
                Return _StringAltID
            End Get
            Set(ByVal value As String)
                _StringAltID = value
            End Set
        End Property

        Public Property AutoWrap As Boolean
            Get
                Return _AutoWrap
            End Get
            Set(ByVal value As Boolean)
                _AutoWrap = value
            End Set
        End Property

        Public Property Dynamic As Boolean
            Get
                Return _Dynamic
            End Get
            Set(ByVal value As Boolean)
                _Dynamic = value
            End Set
        End Property

        Public Property InUse As Boolean
            Get
                Return _InUse
            End Get
            Set(ByVal value As Boolean)
                _InUse = value
            End Set
        End Property

        Public Property Strings As String()
            Get
                Return _Strings
            End Get
            Set(ByVal value As String())
                _Strings = value
            End Set
        End Property

        Public Property FontName As String
            Get
                Return _FontName
            End Get
            Set(ByVal value As String)
                _FontName = value
            End Set
        End Property

        Public Property UsedBy As String
            Get
                Return _UsedBy.Trim
            End Get
            Set(ByVal value As String)
                _UsedBy = value.Trim
            End Set
        End Property
    End Class
End Namespace
