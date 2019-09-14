Imports System.ComponentModel
Imports System.IO
Imports System.Drawing
Imports VGDDCommon.Common
Imports ImageMagick

Namespace VGDDCommon

    Public Class VGDDImage
        Implements ICustomTypeDescriptor, System.ComponentModel.ISupportInitialize

        Private _Name As String
        Public _FileName As String
        Public _OrigBitmap As Bitmap = Nothing
        Private _TransparentBitmap As Bitmap = Nothing
        Private _ParentTransparentColour As Color = Nothing
        Public _GraphicsPath As Drawing2D.GraphicsPath
        Public _ReferencedInEventCode As Boolean = False
        Public _ForceInclude As Boolean = False
        Private _Type As PictureType = PictureType.FLASH_VGDD
        Private _SDFileName As String
        Public _BitmapSize As Integer
        Public _BitmapCompressedSize As Integer
#If Not PlayerMonolitico Then
        Private _CompressionType As VGDDCommon.GrcProject.CompressionTypes
#End If
        Public UsedBy As New ArrayList

        Private _AllowScaling As Boolean = True
        Private _InterpolationMode As Drawing2D.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        Private _OriginalColourDepth As String
        Private _ColourDepth As Integer = 0
        Private _Scale As Integer = 1
        Private ApplyingScaling As Boolean = False
        Private _MyPropsToRemove As String = "Bitmap OrigBitmap IsLoading"
        Private _ToBeConverted As Boolean = False
        Private _GroupName As String = ""
        Private _InternalResource As Boolean = False
        Private _IsLoading As Boolean = False
        Private _RotateFlip As VGDDRotation = VGDDRotation.RotateNoneFlipNone

        Const PICTURETYPESHELP As String = "FLASH=Normal Internal Flash, to be converted with Microchip's Graphics Resource Converter" & vbCrLf & _
            "FLASH_VGDD=Normal Internal Flash, VGDD will handle conversion and create suitable C code" & vbCrLf & _
            "EXTERNAL=Standard External bitmap, usually on external FLASH" & vbCrLf & _
            "BINBMP_ON_SDFAT=Bitmap will be read from SD using FSIO.c - VGDD will take care of converting it to .bin format and copy that file to the specified path"

        Public Enum VGDDRotation
            RotateNoneFlipNone = 0
            Rotate90FlipNone = 1
            Rotate180FlipNone = 2
            Rotate270FlipNone = 3
            RotateNoneFlipX = 4
            Rotate90FlipX = 5
            Rotate270FlipY = 5
            Rotate180FlipX = 6
            Rotate90FlipY = 7
        End Enum

        Public Enum PictureType
            FLASH_VGDD = 1
            FLASH = 2
            EXTERNAL = 3
            EXTERNAL_VGDD = 5
            BINBMP_ON_SDFAT = 4
            EXTERNAL_VGDD_BIN = 6
        End Enum

        Public Enum ValidColourDepths
            BPP1 = 1
            BPP4 = 4
            BPP8 = 8
            BPP16 = 16
            BPP24 = 24
        End Enum

        Public Sub New()
            'Me.new(Nothing)
        End Sub

        Public Sub New(ByVal FileName As String, ByVal LoadFrom As String)
            If LoadFrom = "FILE" AndAlso FileName IsNot Nothing Then
                Dim strName As String = Common.CleanName(Path.GetFileNameWithoutExtension(FileName))
                If File.Exists(FileName) Then
                    _FileName = FileName
                    _Name = strName
                    RefreshBitmap()
                End If
            End If
        End Sub

        Public Sub RefreshBitmap()
            Try
                If _OrigBitmap IsNot Nothing Then
                    _OrigBitmap.Dispose()
                End If
                _OrigBitmap = New Bitmap(_FileName)
                AfterLoadBitmap()
                'Dim oStream As Stream = New FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                'Me.LoadBitmap(strName, oStream)
            Catch ex As Exception
            End Try
        End Sub

        Public Sub New(ByVal BitmapName As String, ByVal oStream As Stream)
            Me.LoadBitmap(BitmapName, oStream)
        End Sub

        Public Sub Dispose()
            If UsedBy IsNot Nothing Then
                UsedBy.Clear()
            End If
            'UsedBy = Nothing

            If _GraphicsPath IsNot Nothing Then
                _GraphicsPath.Dispose()
            End If
            If _OrigBitmap IsNot Nothing Then
                _OrigBitmap.Dispose()
                _OrigBitmap = Nothing
            End If
            If _TransparentBitmap IsNot Nothing Then
                _TransparentBitmap.Dispose()
                _TransparentBitmap = Nothing
            End If
        End Sub

        Public Property IsLoading As Boolean
            Set(ByVal value As Boolean)
                _IsLoading = value
            End Set
            Get
                Return _IsLoading
            End Get
        End Property

        Public Sub LoadBitmap(ByVal BitmapName As String, ByVal oStream As Stream)
            Me.LoadBitmap(BitmapName, Bitmap.FromStream(oStream))
        End Sub

        Public Sub AfterLoadBitmap()
            Try
                _TransparentBitmap = New Bitmap(_OrigBitmap)
                'Format16bppRgb555 {135173}
                _ColourDepth = 0
                'Dim strColourDepth As String = Me.Bitmap.PixelFormat.ToString
                'If strColourDepth.IndexOf("bpp") >= 0 Then
                '    strColourDepth = strColourDepth.Substring(0, strColourDepth.IndexOf("bpp"))
                'End If
                '_ColourDepth = strColourDepth.Replace("Format", "") '.Replace("bpp", "").Replace("Argb", "").Replace("Rgb", "").Replace("Indexed", "")
                _OriginalColourDepth = _OrigBitmap.PixelFormat.ToString
                ResetColorDepth()
                CheckXP()
                'Dim s As String = ""
                'For Each oBitmap As VGDDImage In Common.Bitmaps
                '    s &= vbCrLf & "Name=" & oBitmap.Name & " - Size=" & oBitmap.BitmapSize
                'Next
                'System.Windows.Forms.MessageBox.Show("LoadBitmap3 #" & Common.Bitmaps.Count & " Name=" & BitmapName & " - Bitmaps:" & s)
            Catch ex As Exception
                'System.Windows.Forms.MessageBox.Show("LoadBitmap  " & ex.Message)
            End Try
            If _OrigBitmap Is Nothing Then
                _OrigBitmap = New Bitmap(1, 1)
            End If

        End Sub

        Public Sub LoadBitmap(ByVal BitmapName As String, ByVal Bitmap As Bitmap)
            _OrigBitmap = New Bitmap(Bitmap)
            _Name = BitmapName
            AfterLoadBitmap()
        End Sub

        Private Sub ResetColorDepth()
            If Not _AllowScaling Then
                If _FileName IsNot Nothing Then
                    Select Case Path.GetExtension(_FileName).ToLower
                        Case ".bmp", ".jpg"
                        Case Else
                            Windows.Forms.MessageBox.Show("Warning: GRC allows only pictures in BMP and JPG formats. " & vbCrLf & _
                                            "Please convert " & Path.GetFileName(_FileName) & " with an external software like GIMP before adding it to VGDD," & vbCrLf & _
                                            "or set the AllowScaling property to true to allow internal conversion if this satisfy you.", _
                                            "GRC unsupported bitmap format")
                    End Select
                End If
            End If
            If _OrigBitmap IsNot Nothing Then
                Select Case _OrigBitmap.PixelFormat
                    Case Imaging.PixelFormat.Format1bppIndexed
                        _ColourDepth = 1
#If Not PlayerMonolitico Then
                        _CompressionType = GrcProject.CompressionTypes.None
#End If
                    Case Imaging.PixelFormat.Format4bppIndexed
                        _ColourDepth = 4
                    Case Imaging.PixelFormat.Format8bppIndexed
                        _ColourDepth = 8
                    Case Imaging.PixelFormat.Format16bppArgb1555, Imaging.PixelFormat.Format16bppGrayScale, Imaging.PixelFormat.Format16bppRgb555, Imaging.PixelFormat.Format16bppRgb565
                        _ColourDepth = 16
#If Not PlayerMonolitico Then
                        _CompressionType = GrcProject.CompressionTypes.None
#End If
                    Case Imaging.PixelFormat.Format32bppArgb, Imaging.PixelFormat.Format32bppPArgb, Imaging.PixelFormat.Format32bppRgb
                        _ColourDepth = 32
#If Not PlayerMonolitico Then
                        _CompressionType = GrcProject.CompressionTypes.None
#End If
                    Case Else
                        _ColourDepth = 24
#If Not PlayerMonolitico Then
                        _CompressionType = GrcProject.CompressionTypes.None
#End If

                End Select
            End If
        End Sub

        Public Sub AddUsedBy(ByVal obj As Object)
            If UsedBy IsNot Nothing AndAlso Not UsedBy.Contains(obj) Then
                UsedBy.Add(obj)
            End If
        End Sub

        Public Sub RemoveUsedBy(ByVal obj As Object)
            If UsedBy IsNot Nothing AndAlso UsedBy.Contains(obj) Then
                UsedBy.Remove(obj)
            End If
        End Sub

        Public Function IsUsedByOthers(ByVal obj As Object, ByRef UsedByList As String) As Boolean
            IsUsedByOthers = False
            For Each objOth As Object In UsedBy
                If objOth IsNot Nothing AndAlso Not objOth Is obj Then
                    IsUsedByOthers = True
                    Try
                        UsedByList &= " " & objOth.parent.name & "." & objOth.name
                    Catch ex As Exception
                    End Try
                End If
            Next
            If UsedByList IsNot Nothing Then UsedByList = UsedByList.Trim
        End Function

        Public Sub AddPropertyToShow(ByVal PropertyName As String)
            PropertyName = " " & PropertyName.Trim & " "
            If _MyPropsToRemove.Contains(PropertyName) Then
                _MyPropsToRemove = " " & _MyPropsToRemove.Replace(PropertyName, "").Trim & " "
            End If
        End Sub

        Public Sub RemovePropertyToShow(ByVal PropertyName As String)
            PropertyName = " " & PropertyName.Trim & " "
            If Not _MyPropsToRemove.Contains(PropertyName) Then
                _MyPropsToRemove = " " & _MyPropsToRemove.Trim & " " & PropertyName
            End If
        End Sub

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Property ParentTransparentColour() As Color
            Get
                Return _ParentTransparentColour
            End Get
            Set(ByVal value As Color)
                Dim OldValue As Color = _ParentTransparentColour
                _ParentTransparentColour = value
                If value <> OldValue Then
                    ScaleBitmap()
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Property TransparentBitmap() As Bitmap
            Get
                Return _TransparentBitmap
            End Get
            Set(ByVal value As Bitmap)
                _TransparentBitmap = value
                BuildTransparentBitmap()
            End Set
        End Property

        Private Sub CalculateGraphicsPath()
            If _ParentTransparentColour <> Nothing AndAlso _ParentTransparentColour <> Color.Transparent Then
                Me._GraphicsPath = Common.CalculateControlGraphicsPath(TransparentBitmap, _ParentTransparentColour, 0, 0)
            Else
                Me._GraphicsPath = New Drawing2D.GraphicsPath
                Me._GraphicsPath.AddRectangle(New Rectangle(0, 0, TransparentBitmap.Width, TransparentBitmap.Height))
            End If
        End Sub

        Private Sub BuildTransparentBitmap()
            'If Me.TransparentBitmap IsNot Nothing Then
            '    Me.TransparentBitmap.Dispose()
            'End If
            Try
                If _TransparentBitmap Is Nothing Then Exit Sub
                'If _ParentTransparentColour = Nothing Then Exit Sub
                If _ParentTransparentColour = Color.Transparent Then Exit Sub
                Dim colorMap(0) As Imaging.ColorMap
                colorMap(0) = New Imaging.ColorMap()
                colorMap(0).OldColor = _TransparentBitmap.GetPixel(0, 0)
                colorMap(0).NewColor = _ParentTransparentColour
                Dim attr As New Imaging.ImageAttributes
                attr.SetRemapTable(colorMap)

                Using g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
                    Using b As New Bitmap(_TransparentBitmap.Width, _TransparentBitmap.Height, g)
                        Using g2 = Graphics.FromImage(b)
                            Dim rect As New Rectangle(0, 0, _TransparentBitmap.Width, _TransparentBitmap.Height)
                            Try
                                'g2.DrawImage(_TransparentBitmap, rect, 0, 0, _TransparentBitmap.Width, _TransparentBitmap.Height, g.PageUnit, attr)
                                g2.DrawImage(_TransparentBitmap, 0, 0, _TransparentBitmap.Width, _TransparentBitmap.Height)
                            Catch ex As Exception
                                'g2.DrawImage(_TransparentBitmap, 0, 0, _TransparentBitmap.Width, _TransparentBitmap.Height)
                            End Try
                            'b.MakeTransparent(_ParentTransparentColour)
                            _TransparentBitmap = b.Clone(rect, _TransparentBitmap.PixelFormat)
                            Return
                        End Using
                    End Using
                End Using

            Catch ex As Exception

            End Try
        End Sub

        Public Sub ScaleBitmap()
            'If _ScaledBitmap Is Nothing Then
            '    _ScaledBitmap = New Bitmap(_OrigBitmap) ' _OrigBitmap.Clone
            'End If
            'If _AllowScaling Then
            If Not Me._IsLoading Then
                If _TransparentBitmap IsNot Nothing Then
                    ScaleBitmap(_TransparentBitmap.Width, _TransparentBitmap.Height, _Scale)
                End If
            End If
        End Sub

        Private Function CheckXP() As Boolean
            If Common.OsVersion Is Nothing Then
                Common.GetOs()
            End If
            If Common.OsVersion.StartsWith("5.") Then 'XP workaround
                If _ColourDepth = ValidColourDepths.BPP4 Or _ColourDepth = ValidColourDepths.BPP8 Then
                    _AllowScaling = False
                    ResetColorDepth()
                    RemovePropertyToShow("AllowScaling")
                    RemovePropertyToShow("ColourDepth")
                    RemovePropertyToShow("InterpolationMode")
                    RemovePropertyToShow("RotateFlip")
                    Return True
                End If
            End If
            Return False
        End Function

        Public Sub ScaleBitmap(ByVal newWidth As Integer, ByVal newHeight As Integer, ByVal scale As Integer)
            Dim oPixelFormat As Drawing.Imaging.PixelFormat = Nothing
            Try
                _Scale = scale
                If _OrigBitmap Is Nothing OrElse _OrigBitmap.Width = 1 Or newWidth = 0 Or newHeight = 0 Then
                    Exit Sub
                End If
                If Not _AllowScaling Then
                    _TransparentBitmap = New Bitmap(_OrigBitmap)
                    CalculateGraphicsPath()
                    BuildTransparentBitmap()
                    Return
                End If
                '_ScaledBitmap = Me.Bitmap.Clone
                Select Case Me.ColourDepth
                    Case 1
                        oPixelFormat = Imaging.PixelFormat.Format1bppIndexed
                    Case 4
                        oPixelFormat = Imaging.PixelFormat.Format4bppIndexed
                    Case 8
                        oPixelFormat = Imaging.PixelFormat.Format8bppIndexed
                    Case 24
                        oPixelFormat = Imaging.PixelFormat.Format24bppRgb
                    Case 16
                        oPixelFormat = Imaging.PixelFormat.Format16bppRgb565
                    Case Else
                        Select Case Common.ProjectColourDepth
                            Case 1
                                oPixelFormat = Imaging.PixelFormat.Format1bppIndexed
                            Case 4
                                oPixelFormat = Imaging.PixelFormat.Format4bppIndexed
                            Case 8
                                oPixelFormat = Imaging.PixelFormat.Format8bppIndexed
                            Case 24
                                oPixelFormat = Imaging.PixelFormat.Format24bppRgb
                            Case Else
                                oPixelFormat = Imaging.PixelFormat.Format16bppRgb565
                        End Select
                End Select

                Using oNotIndexedBitmap As New Bitmap(newWidth \ scale, newHeight \ scale)
                    Dim rectF As RectangleF = New RectangleF(0, 0, oNotIndexedBitmap.Width, oNotIndexedBitmap.Height)
                    Try
                        Using gr As Graphics = Graphics.FromImage(oNotIndexedBitmap)
                            gr.InterpolationMode = _InterpolationMode 'Drawing2D.InterpolationMode.NearestNeighbor
                            gr.DrawImage(_OrigBitmap, rectF)
                        End Using
                    Catch ex As Exception
                        Windows.Forms.MessageBox.Show(String.Format("Cannot draw bitmap to {0}x{1} {2}: " & ex.Message, _
                               oNotIndexedBitmap.Width, oNotIndexedBitmap.Height, oPixelFormat.ToString), _
                               "GDI+ error: Error drawing bitmap", Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)
                        Return
                    End Try

                    Try
                        If _TransparentBitmap IsNot Nothing Then
                            _TransparentBitmap.Dispose()
                        End If
                        _TransparentBitmap = Nothing
                        '_TransparentBitmap = oNotIndexedBitmap.Clone(rectF, oPixelFormat)
                        Dim oSettings As New QuantizeSettings
                        Select Case oPixelFormat
                            Case Imaging.PixelFormat.Format1bppIndexed
                                oSettings.Colors = 2
                            Case Imaging.PixelFormat.Format4bppIndexed
                                oSettings.Colors = 16
                            Case Imaging.PixelFormat.Format8bppIndexed
                                oSettings.Colors = 256
                            Case Imaging.PixelFormat.Format24bppRgb, Imaging.PixelFormat.Format32bppArgb
                                oSettings.Colors = 16777216
                            Case Imaging.PixelFormat.Format16bppRgb565
                                oSettings.Colors = 16777216
                        End Select
                        Using image As New MagickImage(oNotIndexedBitmap)
                            image.Quantize(oSettings)
                            _TransparentBitmap = image.ToBitmap(Drawing.Imaging.ImageFormat.Bmp)
                        End Using

                'If _TransparentBitmap.PixelFormat = _OrigBitmap.PixelFormat AndAlso _OrigBitmap.Palette.Entries.Length > 0 Then
                '    _TransparentBitmap.Palette = _OrigBitmap.Palette
                'End If
            Catch ex As Exception
                Windows.Forms.MessageBox.Show(String.Format("Cannot clone bitmap to {0}x{1} {2}: " & ex.Message, _
                     oNotIndexedBitmap.Width, oNotIndexedBitmap.Height, oPixelFormat.ToString), _
                     "GDI+ error: Error cloning bitmap on " & Common.OsVersion, Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)
                Return
            End Try
                    'If UsedBy.Count > 1 Then
                    '    If Not Me.ApplyingScaling Then
                    '        Me.ApplyingScaling = True
                    '        For Each oControl As Object In UsedBy
                    '            oControl.SetSize(New Size(newWidth, newHeight))
                    '        Next
                    '        Me.ApplyingScaling = False
                    '    End If
                    'End If
                    ToBeConverted = True
                End Using

            Catch ex As Exception
                Windows.Forms.MessageBox.Show(String.Format("Cannot scale bitmap to {0}x{1} {2}: " & ex.Message, _
                                                           newWidth \ scale, newHeight \ scale, oPixelFormat.ToString), _
                                                        "GDI+ error: Error scaling bitmap", Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)
                Return
                'Finally
                'oNotIndexedBitmap.Dispose()
            End Try

            CalculateGraphicsPath()
            BuildTransparentBitmap()
            For Each obj As Object In Me.UsedBy
                If TypeOf (obj) Is IVGDDWidgetWithBitmap Then
                    Dim oWidget As IVGDDWidgetWithBitmap = obj
                    If Not oWidget.VGDDImage Is Me Then
                        oWidget.SetBitmap(_TransparentBitmap)
                    End If
                End If
            Next
        End Sub

        Public Shared Function InvalidBitmap(ByVal BitmapIn As Bitmap, ByVal newWidth As Integer, ByVal newHeight As Integer) As Bitmap
            If BitmapIn Is Nothing Then
                If newWidth <= 0 Then newWidth = 100
                If newHeight <= 0 Then newHeight = 100
                BitmapIn = New Bitmap(newWidth, newHeight)
            End If
            Dim g As Graphics = Graphics.FromImage(BitmapIn)
            g.DrawRectangle(Pens.Red, 0, 0, BitmapIn.Width - 1, BitmapIn.Height - 1)
            g.DrawLine(Pens.Red, 0, 0, BitmapIn.Width - 1, BitmapIn.Height - 1)
            g.DrawLine(Pens.Red, BitmapIn.Width - 1, 0, 0, BitmapIn.Height - 1)
            g.Dispose()
            Return BitmapIn
        End Function

        <Description("Rotate and/or Flip image before conversion" & vbCrLf & "Use this feature only if the TFT driver doesn't support DISP_ROTATION settings (i.e. Epson S1D13517)")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(VGDDRotation), "RotateNoneFlipNone")> _
        Public Property RotateFlip As VGDDRotation
            Get
                Return _RotateFlip
            End Get
            Set(ByVal value As VGDDRotation)
                _RotateFlip = value
                _ToBeConverted = True
            End Set
        End Property

        <Description("Group name this Bitmap belongs to." & vbCrLf & "For BitmapOnSD, determines the creation of Pool Arrays.")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Property GroupName As String
            Get
                Return _GroupName
            End Get
            Set(ByVal value As String)
                _GroupName = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ToBeConverted As Boolean
            Get
                Return _ToBeConverted
            End Get
            Set(ByVal value As Boolean)
                'If value = False Or Not Common.ProjectLoading Then
                _ToBeConverted = value
                'End If
            End Set
        End Property

        <Description("Interpolation algorithm to use when scaling bitmap")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(Drawing2D.InterpolationMode), "HighQualityBicubic")> _
        Public Property InterpolationMode As Drawing2D.InterpolationMode
            Get
                Return _InterpolationMode
            End Get
            Set(ByVal value As Drawing2D.InterpolationMode)
                If value <> Drawing2D.InterpolationMode.Invalid Then
                    _InterpolationMode = value
                    CheckXP()
                    ScaleBitmap()
                    _BitmapCompressedSize = 0
                    ToBeConverted = True
                End If
            End Set
        End Property

        <Description("Allow this bitmap to be scaled up/down by owner widgets?" & vbCrLf & "Setting it to true also enable internal colourdepth conversion support.")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(True)> _
        Public Property AllowScaling As Boolean
            Get
                Return _AllowScaling
            End Get
            Set(ByVal value As Boolean)
                Dim Oldvalue As Boolean = _AllowScaling
                _AllowScaling = value
                ResetColorDepth()
                CheckXP()
                If _AllowScaling Then
                    AddPropertyToShow("InterpolationMode")
                    AddPropertyToShow("ColourDepth")
                    AddPropertyToShow("RotateFlip")
                Else
                    RemovePropertyToShow("InterpolationMode")
                    RemovePropertyToShow("ColourDepth")
                    RemovePropertyToShow("RotateFlip")
                End If
                If Oldvalue <> value Then
                    ScaleBitmap()
                End If
                ToBeConverted = True
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Compression type to use. Note: IPU is available only on selected MCUs (i.e. PIC24FJ256DA210)")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(GrcProject.CompressionTypes), "None")> _
        Public Property CompressionType As GrcProject.CompressionTypes
            Get
                Return _CompressionType
            End Get
            Set(ByVal value As GrcProject.CompressionTypes)
                If value = GrcProject.CompressionTypes.RLE Then
                    If Not Common.ProjectLoading AndAlso Me.ColourDepth <> 4 AndAlso Me.ColourDepth <> 8 Then
                        Windows.Forms.MessageBox.Show("RLE Compression available only for 4bpp and 8bpp bitmaps.", "RLE compression not available", Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Exclamation)
                        Exit Property
                    End If
                End If
                _CompressionType = value
                _BitmapCompressedSize = 0
                ToBeConverted = True
            End Set
        End Property
#End If

        <Description("Get image Bytes()")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public ReadOnly Property GetBytes() As Byte()
            Get
                Try
                    Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(Bitmap))
                    Return converter.ConvertTo(_TransparentBitmap, GetType(Byte()))
                Catch ex As Exception
                End Try
                Return Nothing
            End Get
        End Property

        <Description("Picture Type" & vbCrLf & PICTURETYPESHELP)> _
        Public Property Type As PictureType
            Get
                Return (_Type)
            End Get
            Set(ByVal value As PictureType)
                _Type = value
                If _Type = PictureType.BINBMP_ON_SDFAT Then
                    AddPropertyToShow("SDFileName")
                Else
                    RemovePropertyToShow("SDFileName")
                End If
                ToBeConverted = True
            End Set
        End Property

        Public Sub CheckSDFilename()
#If Not PlayerMonolitico Then
            If _SDFileName = "" AndAlso _Name IsNot Nothing Then
                Dim strNewName As String = _Name
                If strNewName.Length > 8 Then strNewName = strNewName.Substring(0, 8)
                _SDFileName = Common.ImageUniqueSDFileName(strNewName) & ".bin"
            End If
#End If
        End Sub


        <Description("Picture FileName when converted to bin and stored on SD (Type=BINBMP_ON_SDFAT)")> _
        Public Property SDFileName As String
            Get
                If _Type = PictureType.BINBMP_ON_SDFAT Then
                    Return _SDFileName
                Else
                    Return "" '"<Valid only for Type=""BINBMP_ON_SDFAT"">"
                End If
            End Get
            Set(ByVal value As String)
                If _Type = PictureType.BINBMP_ON_SDFAT Then
                    _SDFileName = value
                    If Not Me._IsLoading Then
                        CheckSDFilename()
                    End If
                End If
                ToBeConverted = True
            End Set
        End Property

        <Description("Size of the original bitmap")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)>
        Public ReadOnly Property OriginalSize As Size
            Get
                If _OrigBitmap IsNot Nothing Then
                    Try
                        Return _OrigBitmap.Size
                    Catch
                        If FileName IsNot Nothing Then
                            _OrigBitmap.Dispose()
                            _OrigBitmap = New Bitmap(FileName)
                            Return _OrigBitmap.Size
                        End If
                        Return New Size(0, 0)
                    End Try
                    Try
                        Return _TransparentBitmap.Size
                    Catch
                        Return New Size(0, 0)
                    End Try
                Else
                    Return New Size(0, 0)
                End If
            End Get
        End Property

        <Description("Colour depth of the original bitmap")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)>
        Public ReadOnly Property OriginalColourDepth As String
            Get
                Return _OriginalColourDepth
            End Get
        End Property

        <Description("Desired Colour depth of the converted/scaled bitmap")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(ValidColourDepths), "BPP16")> _
        Public Property ColourDepth As ValidColourDepths
            Get
                Return _ColourDepth
            End Get
            Set(ByVal value As ValidColourDepths)
                Dim OldValue As ValidColourDepths = _ColourDepth
                _ColourDepth = value
                CheckXP()
                If _AllowScaling And value <> OldValue Then
                    ScaleBitmap()
                End If
#If Not PlayerMonolitico Then
                If _CompressionType = GrcProject.CompressionTypes.RLE AndAlso (value <> ValidColourDepths.BPP4 And value <> ValidColourDepths.BPP8) Then
                    Me.CompressionType = GrcProject.CompressionTypes.None
                End If
#End If
                _BitmapCompressedSize = 0
                ToBeConverted = True
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)> _
        <Description("Uncompressed Bitmap size in bytes.")> _
        Public ReadOnly Property BitmapSize As Integer
            Get
                Return _BitmapSize
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)> _
        <Description("Bitmap size in bytes after compression (if applicable).")> _
        Public ReadOnly Property BitmapCompressedSize As Integer
            Get
                Return _BitmapCompressedSize
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)> _
        <Description("True is this Bitmap is used by some Widget in project or referenced in Event Code.")> _
        Public ReadOnly Property Referenced As Boolean
            Get
                'Return _Referenced
                If Me.UsedBy Is Nothing Then Return False
                Return Me.UsedBy.Count > 0 Or _ReferencedInEventCode Or _ForceInclude
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <Description("Set to True is you want this bitmap to be included in conversion regardless of its usage.")> _
        Public Property ForceInclude As Boolean
            Get
                Return _ForceInclude
            End Get
            Set(ByVal value As Boolean)
                _ForceInclude = value
                If _ForceInclude Then
                    ToBeConverted = True
                End If
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)> _
        <Description("True is this Bitmap is referenced by some Widget Event Code.")> _
        Public ReadOnly Property ReferencedInEventCode As Boolean
            Get
                Return _ReferencedInEventCode
            End Get
        End Property

        <Description("Internal name for this bitmap")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = Common.CleanName(value)
                ToBeConverted = True
            End Set
        End Property

        <Description("Filename of the image on PC")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)> _
        Public ReadOnly Property FileName As String
            Get
                Return _FileName
            End Get
        End Property

        <Description("Bitmap for this VGDDImage - Scaled version")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public ReadOnly Property Bitmap As Bitmap
            Get
                Return _TransparentBitmap
            End Get
        End Property

        <Description("Original Bitmap for this VGDDImage")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Property OrigBitmap As Bitmap
            Get
                Return _OrigBitmap
            End Get
            Set(ByVal value As Bitmap)
                _OrigBitmap = value
                ScaleBitmap()
                ToBeConverted = True
            End Set
        End Property

        <Description("Scaled Bitmap size")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Property Size() As Size
            Get
                Try
                    Return _TransparentBitmap.Size
                Catch ex As Exception
                    Return _OrigBitmap.Size
                End Try
            End Get
            Set(ByVal value As Size)
                If value <> _TransparentBitmap.Size Then
                    ScaleBitmap(value.Width, value.Height, _Scale)
                    ToBeConverted = True
                End If
            End Set
        End Property

        Public Function Save(ByVal strFileName As String, ByVal format As Drawing.Imaging.ImageFormat) As String
            Using oBitmapToConvert As New ImageMagick.MagickImage(_TransparentBitmap)
                Try
                    Select Case _RotateFlip
                        Case VGDDImage.VGDDRotation.RotateNoneFlipNone
                        Case VGDDImage.VGDDRotation.Rotate90FlipNone
                            oBitmapToConvert.Rotate(90)
                        Case VGDDImage.VGDDRotation.Rotate180FlipNone
                            oBitmapToConvert.Rotate(180)
                        Case VGDDImage.VGDDRotation.Rotate270FlipNone
                            oBitmapToConvert.Rotate(270)
                        Case VGDDImage.VGDDRotation.Rotate90FlipX
                            oBitmapToConvert.Rotate(90)
                            oBitmapToConvert.Flop()
                        Case VGDDImage.VGDDRotation.Rotate90FlipY
                            oBitmapToConvert.Rotate(90)
                            oBitmapToConvert.Flip()
                        Case VGDDImage.VGDDRotation.Rotate180FlipX
                            oBitmapToConvert.Rotate(270)
                            oBitmapToConvert.Flop()
                        Case VGDDImage.VGDDRotation.Rotate270FlipY
                            oBitmapToConvert.Rotate(270)
                            oBitmapToConvert.Flip()
                        Case VGDDImage.VGDDRotation.RotateNoneFlipX
                            oBitmapToConvert.Flop()
                    End Select
                Catch ex As Exception
                End Try

                Dim blnToSave As Boolean = True
                Dim oSettings As New ImageMagick.QuantizeSettings
                Select Case _ColourDepth
                    Case VGDDImage.ValidColourDepths.BPP1
                        oSettings.Colors = 2
                        oBitmapToConvert.ColorType = ImageMagick.ColorType.TrueColor
                        oBitmapToConvert.Quantize(oSettings)
                    Case VGDDImage.ValidColourDepths.BPP4
                        oSettings.Colors = 16
                        oBitmapToConvert.ColorType = ImageMagick.ColorType.Palette
                        oBitmapToConvert.Quantize(oSettings)
                    Case VGDDImage.ValidColourDepths.BPP8 'Bug ImageMagick
                        oSettings.Colors = 256
                        oBitmapToConvert.Quantize(oSettings)
                        oBitmapToConvert.ColorType = ImageMagick.ColorType.Palette
                        Dim oGdiBitmap As System.Drawing.Bitmap
                        oGdiBitmap = oBitmapToConvert.ToBitmap(Drawing.Imaging.ImageFormat.Bmp)
                        Dim oGdiBitmap2 As System.Drawing.Bitmap
                        oGdiBitmap2 = oGdiBitmap.Clone(New Drawing.Rectangle(0, 0, oGdiBitmap.Width, oGdiBitmap.Height), Drawing.Imaging.PixelFormat.Format8bppIndexed)
                        oGdiBitmap2.Save(strFileName, format)
                        blnToSave = False
                        oGdiBitmap.Dispose()
                        oGdiBitmap2.Dispose()
                        Return "OK"
                    Case VGDDImage.ValidColourDepths.BPP16, _
                         VGDDImage.ValidColourDepths.BPP24
                        oSettings.Colors = 16777216
                        oBitmapToConvert.ColorType = ImageMagick.ColorType.TrueColor
                End Select
                'oBitmapToConvert.Format = ImageMagick.MagickFormat.Bmp
                If blnToSave Then
                    Try
                        oBitmapToConvert.Write("BMP3:" & strFileName)
                    Catch ex As Exception
                        'MessageBox.Show(String.Format("Error writing {0}. Please ensure the file is not in use and/or that the folder is writable.", strTempFilename), "Write error on GRC_Input path")
                        Return ex.Message
                    End Try
                End If
            End Using
            Return "OK"
        End Function

#Region "ICustomTypeDescriptor Members"

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not (_MyPropsToRemove & " " & PROPSTOREMOVE).Contains(" " & pd.Name & " ") Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

        Private Function GetProperties(ByVal attributes() As Attribute) As PropertyDescriptorCollection _
            Implements ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, attributes, True)
            Return FilterProperties(pdc)
        End Function

        Private Function GetEvents(ByVal attributes As Attribute()) As EventDescriptorCollection _
            Implements ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, attributes, True)
        End Function

        Public Function GetConverter() As TypeConverter _
            Implements ICustomTypeDescriptor.GetConverter
            Return TypeDescriptor.GetConverter(Me, True)
        End Function

        Private Function System_ComponentModel_ICustomTypeDescriptor_GetEvents() As EventDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, True)
        End Function

        Public Function GetComponentName() As String _
            Implements ICustomTypeDescriptor.GetComponentName
            Return TypeDescriptor.GetComponentName(Me, True)
        End Function

        Public Function GetPropertyOwner(ByVal pd As PropertyDescriptor) As Object _
            Implements ICustomTypeDescriptor.GetPropertyOwner
            Return Me
        End Function

        Public Function GetAttributes() As AttributeCollection _
            Implements ICustomTypeDescriptor.GetAttributes
            Return TypeDescriptor.GetAttributes(Me, True)
        End Function

        Private Function System_ComponentModel_ICustomTypeDescriptor_GetProperties() As PropertyDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetProperties
            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me, True)
            Return FilterProperties(pdc)
        End Function

        Public Function GetEditor(ByVal editorBaseType As Type) As Object _
            Implements ICustomTypeDescriptor.GetEditor
            Return TypeDescriptor.GetEditor(Me, editorBaseType, True)
        End Function

        Public Function GetDefaultProperty() As PropertyDescriptor _
            Implements ICustomTypeDescriptor.GetDefaultProperty
            Return TypeDescriptor.GetDefaultProperty(Me, True)
        End Function

        Public Function GetDefaultEvent() As EventDescriptor _
            Implements ICustomTypeDescriptor.GetDefaultEvent
            Return TypeDescriptor.GetDefaultEvent(Me, True)
        End Function

        Public Function GetClassName() As String _
            Implements ICustomTypeDescriptor.GetClassName
            Return TypeDescriptor.GetClassName(Me, True)
        End Function

#End Region

        Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
            Me._IsLoading = True
        End Sub

        Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
            Me._IsLoading = False
            ScaleBitmap()
        End Sub
    End Class

End Namespace