Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Namespace VGDDCommon

    Public Class BmpConvert

        Public Shared oGrcProject As GrcProject = Nothing

#Region "Bitmaps"
        Structure BITMAP_HEADER
            Public compression As Byte  ' Compression setting 
            Public colorDepth As Byte   ' Color depth used
            Public height As Int16      ' Image height
            Public width As Int16       ' Image width
        End Structure

        Public Shared Function BitmapToBinFile(ByRef Img As VGDDImage, ByVal BinFileName As String) As Long
            BitmapToBinFile = 0
            Try
                Dim ms As MemoryStream = BitmapConvert(Img)
                BitmapToBinFile = ms.Length
                ms.Seek(0, SeekOrigin.Begin)
                Dim strPath As String = Path.GetDirectoryName(BinFileName)
                If strPath <> "" AndAlso Not Directory.Exists(strPath) Then
                    Directory.CreateDirectory(strPath)
                End If
                Dim fsOut As New FileStream(BinFileName, FileMode.Create)
                Dim swOut As New BinaryWriter(fsOut)
                For i As Long = 0 To ms.Length - 1
                    swOut.Write(CByte(ms.ReadByte))
                Next
                fsOut.Flush()
                fsOut.Close()
                swOut.Close()
                ms.Dispose()
            Catch ex As Exception

            End Try
        End Function

        Public Shared Function BitmapConvert(ByVal Img As VGDDImage) As MemoryStream
            If Common.ProjectUseGRC Then
                Throw New Exception("BitmapConvert should not be called when using GRC")
            Else
                Return BitmapConvertInternal(Img.Bitmap)
            End If
        End Function

        Public Shared Sub BitmapConvertGRC()
            If oGrcProject Is Nothing Then Exit Sub
            With oGrcProject
                Dim strOutFile As String = .ConvertToFile()
                If .GrcOutput.Trim <> "" Then
                    Windows.Forms.MessageBox.Show("Error converting bitmaps:" & vbCrLf & .GrcOutput, "GRC Bitmap conversion error")
                End If
                ParseOutputFile(.OutputFile)
            End With
            oGrcProject = Nothing
        End Sub

        Public Shared Sub ParseOutputFile(ByVal OutFile As String)
            Dim sr As New StreamReader(OutFile)
            Dim oBitmap As VGDDImage = Nothing
            Dim Line As String
            Dim blnImageFlash As Boolean = False, lngImageFlashSize As Long
            Do While Not sr.EndOfStream
                Line = sr.ReadLine
                If Line.Contains("const GFX_IMAGE_HEADER") Or Line.Contains("const IMAGE_FLASH bmp") Then
                    Dim strBitmapName As String = Line.Substring(Line.IndexOf(" bmp") + 4)
                    strBitmapName = strBitmapName.Substring(0, strBitmapName.IndexOf(" "))
                    oBitmap = VGDDCommon.Common.GetBitmap(strBitmapName)
                    If Line.Contains("const IMAGE_FLASH bmp") Then
                        blnImageFlash = True
                        lngImageFlashSize = 0
                    Else
                        For i As Integer = 1 To 6
                            sr.ReadLine()
                        Next
                        Line = sr.ReadLine.Replace(",", "").Trim
                        Integer.TryParse(Line, oBitmap._BitmapCompressedSize)
                        Line = sr.ReadLine.Replace(",", "").Trim
                        Integer.TryParse(Line, oBitmap._BitmapSize)
                        oBitmap = Nothing
                    End If
                ElseIf Line.Contains("GFX_COMPRESSED_BUFFER_SIZE") Or Line.Contains("GFX_DECOMPRESSED_BUFFER_SIZE") Then
                    Dim intVal As Integer
                    Dim strval As String = Line.Substring(Line.IndexOf("<") + 1).Replace(")", "").Trim
                    If Integer.TryParse(strval, intVal) Then
                        If Line.Contains("GFX_COMPRESSED_BUFFER_SIZE") Then
                            Common.IPU_MAX_COMPRESSED_BUFFER_SIZE = intVal
                        ElseIf Line.Contains("GFX_DECOMPRESSED_BUFFER_SIZE") Then
                            Common.IPU_MAX_DECOMPRESSED_BUFFER_SIZE = intVal
                        End If
                    End If

                ElseIf blnImageFlash Then
                    If Line.Contains("0x") Then
                        lngImageFlashSize += Line.Split(",").Length
                    ElseIf (Line.StartsWith("};") Or Line.StartsWith("asm("".section .const")) AndAlso oBitmap IsNot Nothing Then
                        oBitmap._BitmapSize = lngImageFlashSize
                        oBitmap._BitmapCompressedSize = lngImageFlashSize
                        oBitmap = Nothing
                        blnImageFlash = False
                    ElseIf Line.Contains("(FLASH_BYTE *)") Then
                        sr.ReadLine()
                    End If
                End If
            Loop
            sr.Close()
        End Sub

        Public Shared Sub BitmapAddToGRC(ByVal Img As VGDDImage)
        End Sub

        Public Shared Function BitmapConvertInternal(ByRef Bmp As Bitmap) As MemoryStream
            Dim ms As New MemoryStream()
            Dim oHeader As BITMAP_HEADER
            With oHeader
                .compression = 0
                .colorDepth = Bitmap.GetPixelFormatSize(Bmp.PixelFormat)
                .height = Bmp.Height
                .width = Bmp.Width
            End With
            If oHeader.colorDepth > 16 Then oHeader.colorDepth = 16

            ms.Write({oHeader.compression, _
                         oHeader.colorDepth, _
                          Math.Truncate(oHeader.height Mod 256), Math.Truncate(oHeader.height / 256), _
                          Math.Truncate(oHeader.width Mod 256), Math.Truncate(oHeader.width / 256)}, 0, 6)
            Select Case oHeader.colorDepth
                Case 4, 8, 16
                    For i As Integer = 0 To Bmp.Palette.Entries.Length - 1
                        Dim col As Color = Bmp.Palette.Entries(i)
                        Dim col565 As UInt16 = CodeGen.Color2Num(col)
                        ms.WriteByte(Math.Truncate(col565 Mod 256))
                        ms.WriteByte(Math.Truncate(col565 / 256))
                    Next
                    'If oHeader.colorDepth = 8 Then
                    '    ms.WriteByte(255)
                    '    ms.WriteByte(255)
                    'End If
                Case Else
                    Debug.Print("?")
            End Select

            Dim idx As Integer, idx2 As Integer
            For y As Integer = 0 To oHeader.height - 1
                Windows.Forms.Application.DoEvents()
                For x As Integer = 0 To oHeader.width - 1
                    Dim pixColor As Color = Bmp.GetPixel(x, y)
                    Select Case oHeader.colorDepth
                        Case 4
                            idx = Array.FindIndex(Bmp.Palette.Entries, Function(c) c = pixColor)
                            If idx < 0 Then idx = 0
                            If Int(x / 2) = x / 2 Then
                                idx2 = idx
                            Else
                                ms.WriteByte((idx << 4) + idx2)
                            End If
                        Case 8
                            idx = Array.FindIndex(Bmp.Palette.Entries, Function(c) c = pixColor)
                            If idx >= 0 Then
                                ms.WriteByte(idx)
                            Else
                                Debug.Print("?")
                            End If
                        Case 16
                            Dim col565 As UInt16 = CodeGen.Color2Num(pixColor)
                            ms.WriteByte(Math.Truncate(col565 Mod 256))
                            ms.WriteByte(Math.Truncate(col565 / 256))
                        Case Else
                            Debug.Print("?")
                    End Select
                Next
            Next
            ms.WriteByte(0)
            ms.WriteByte(0)
            ms.Flush()
            ms.Seek(0, SeekOrigin.Begin)
            Return ms
        End Function

        Public Shared Function BitmapToAsm(ByVal Img As VGDDImage) As String
            Dim ms As MemoryStream = BitmapConvert(Img)
            'Bmp.BitmapSize = ms.Length
            Dim sbAsm As New System.Text.StringBuilder, i As Long, j As Integer
            sbAsm.Append("asm("".byte ")
            For i = 1 To 6
                sbAsm.Append(IIf(i > 1, ",", "") & String.Format("0x{0:x2}", ms.ReadByte))
            Next
            For i = 0 To ms.Length - 6 - 1
                If i / 32 = Int(i / 32) Then
                    sbAsm.Append(""");" & vbCrLf & "asm("".byte ")
                    j = 1
                End If
                sbAsm.Append(IIf(j > 1, ",", "") & String.Format("0x{0:x2}", ms.ReadByte))
                j += 1
            Next
            sbAsm.Append(""");" & vbCrLf)
            ms.Close()
            ms.Dispose()
            Return sbAsm.ToString
        End Function

        Public Shared Function BitmapToHex(ByRef Img As VGDDImage, ByVal Asm As Boolean) As String
            If Common.ProjectUseGRC Then
                Dim ms As MemoryStream = BitmapConvert(Img)
                Dim sb As New System.Text.StringBuilder
                For i As Long = 0 To ms.Length - 1
                    sb.Append(ms.ReadByte)
                Next
                ms.Close()
                ms.Dispose()
                Dim GrcArrayC As String = sb.ToString.Replace("#include", "//#include")
                Dim intPos As Long = GrcArrayC.IndexOf("Converted Resources")
                If intPos > 0 Then
                    intPos = GrcArrayC.Substring(0, intPos).LastIndexOf("/*")
                    If intPos > 0 Then
                        GrcArrayC = GrcArrayC.Substring(intPos)
                    End If
                End If
                Return GrcArrayC
            Else
                Dim ms As MemoryStream = BitmapConvert(Img)
                Img._BitmapSize = ms.Length
                Dim sb As New System.Text.StringBuilder

                Dim i As Long
                'For i = 1 To 6
                '    sb.Append(IIf(i > 1, ",", "") & String.Format("0x{0:X2}", ms.ReadByte))
                'Next
                If Asm Then
                    sb.Append("asm("".byte ")
                End If
                For i = 0 To ms.Length - 1 'ms.Length - 6 - 1
                    If i / 32 = Int(i / 32) Then
                        If Asm Then
                            sb.Append(""");")
                            sb.Append(vbCrLf)
                            sb.Append("asm("".byte ")
                        ElseIf i > 0 Then
                            sb.Append("," & vbCrLf)
                        End If
                    ElseIf i > 0 Then
                        sb.Append(",")
                    End If
                    sb.Append(String.Format("0x{0:X2}", ms.ReadByte))
                Next
                If Asm Then
                    sb.Append(""");")
                End If
                ms.Close()
                ms.Dispose()
                Return sb.ToString
            End If
        End Function

#End Region

#Region "Fonts"
        'http://ww1.microchip.com/downloads/en/AppNotes/01182b.pdf

        Public Shared Function BitmapRGBTo1BPP(ByRef bmIn As Bitmap, ByVal Pixel As Boolean) As Bitmap
            Dim bmdIn As BitmapData = bmIn.LockBits(New System.Drawing.Rectangle(0, 0, bmIn.Width, bmIn.Height), ImageLockMode.ReadOnly, bmIn.PixelFormat)
            Dim bmOut As New Bitmap(bmIn.Width, bmIn.Height, PixelFormat.Format1bppIndexed)
            Dim bmdOut As BitmapData = bmOut.LockBits(New System.Drawing.Rectangle(0, 0, bmOut.Width, bmOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed)
            Dim x As Integer, y As Integer
            For y = 0 To bmIn.Height - 1
                For x = 0 To bmIn.Width - 1
                    'generate the address of the colour pixel
                    Dim index As Integer = y * bmdIn.Stride + (x * 4)
                    'check its brightness
                    If Color.FromArgb(Marshal.ReadByte(bmdIn.Scan0, index + 2), Marshal.ReadByte(bmdIn.Scan0, index + 1), Marshal.ReadByte(bmdIn.Scan0, index)).GetBrightness() > 0.7F Then
                        Dim idx As Integer = y * bmdOut.Stride + (x >> 3)
                        Dim p As Byte = Marshal.ReadByte(bmdOut.Scan0, idx)
                        Dim mask As Byte = CByte(&H80 >> (x And &H7))
                        If Pixel Then
                            p = p Or mask
                        Else
                            p = p And CByte(mask Xor &HFF)
                        End If
                        Marshal.WriteByte(bmdOut.Scan0, idx, p)
                    End If
                Next
            Next
            bmOut.UnlockBits(bmdOut)
            bmIn.UnlockBits(bmdIn)
            Return bmOut
        End Function

        Public Shared FontMinX As Integer, FontMinY As Integer, FontMaxX As Integer, FontMaxY As Integer

        Public Shared Function FontCharToBitmap(ByVal fnt As Font, ByVal charToDraw As Char, ByVal color As Color, ByVal bWidth As Integer, ByVal bHeight As Integer) As Bitmap
            Dim blnFindMax As Boolean = False
            'Dim bitmapWidth As Integer = fnt.Height, bitmapHeight As Integer = fnt.Height
            Dim gt As Graphics = Graphics.FromImage(New Bitmap(1, 1))
            Dim textFormat As New StringFormat()

            textFormat.Alignment = StringAlignment.Near
            textFormat.LineAlignment = StringAlignment.Near

            Dim bs As SizeF = gt.MeasureString(charToDraw, fnt, Integer.MaxValue, textFormat)

            'Dim bitmap As New Bitmap(bs.Width, bs.Height, PixelFormat.Format16bppRgb565)
            Dim bitmap As New Bitmap(bWidth, bHeight, PixelFormat.Format32bppPArgb)
            'bitmap.SetResolution(300, 300)
            Dim g As Graphics = Graphics.FromImage(bitmap)

            g.FillRectangle(New SolidBrush(Drawing.Color.White), New Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height))
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.TextRenderingHint = Text.TextRenderingHint.AntiAlias

            If color = color.Transparent Then
                color = Drawing.Color.Black
                blnFindMax = True
            End If
            g.DrawString(charToDraw.ToString, fnt, New SolidBrush(color), New RectangleF(0, 0, bWidth - 1, bHeight - 1), textFormat)
            'g.DrawString(charToDraw.ToString, fnt, New SolidBrush(color), New RectangleF(0, 0, bs.Width, bs.Height), textFormat)
            Dim blnFoundPixel As Boolean = False

            Dim bmpBW As Bitmap = BitmapRGBTo1BPP(bitmap, True)
            Dim bmFinal As Bitmap = Nothing

            Dim StartX As Integer, MaxX As Integer, StartY As Integer, MaxY As Integer
            Dim WhitePixel As Color = color.FromArgb(255, 255, 255, 255)
            If charToDraw <> Chr(9) And charToDraw <> Chr(32) Then
                blnFoundPixel = False
                For StartX = 0 To bmpBW.Width - 1
                    For y As Integer = 0 To bmpBW.Height - 1
                        If bmpBW.GetPixel(StartX, y) <> WhitePixel Then '.ToString <> "Color [A=255, R=255, G=255, B=255]" Then
                            blnFoundPixel = True
                            Exit For
                        End If
                    Next
                    If blnFoundPixel Then Exit For
                Next
                If StartX = bmpBW.Width Then
                    StartX = 0
                Else
                    StartX -= bmpBW.Width / 16
                    If StartX < 0 Then StartX = 0
                End If

                blnFoundPixel = False
                For MaxX = bmpBW.Width - 1 To 0 Step -1
                    For y As Integer = 0 To bmpBW.Height - 1
                        If bmpBW.GetPixel(MaxX, y) <> WhitePixel Then '.ToString <> "Color [A=255, R=255, G=255, B=255]" Then
                            blnFoundPixel = True
                            Exit For
                        End If
                    Next
                    If blnFoundPixel Then Exit For
                Next
                If MaxX = -1 Then
                    MaxX = FontMaxX
                    'Else
                    '    MaxX += 2
                End If
            End If

            If blnFindMax Then
                blnFoundPixel = False
                For StartY = 0 To bmpBW.Height - 1
                    For x As Integer = 0 To bmpBW.Width - 1
                        If bmpBW.GetPixel(x, StartY) <> WhitePixel Then '.ToString <> "Color [A=255, R=255, G=255, B=255]" Then
                            blnFoundPixel = True
                            Exit For
                        End If
                    Next
                    If blnFoundPixel Then Exit For
                Next
                'If StartY = bmpBW.Height Then
                '    StartY = 0
                'Else
                '    StartY -= 2
                '    If StartY < 0 Then StartY = 0
                'End If

                blnFoundPixel = False
                For MaxY = bmpBW.Height - 1 To 0 Step -1
                    For x As Integer = 0 To bmpBW.Width - 1
                        If bmpBW.GetPixel(x, MaxY) <> WhitePixel Then '.ToString <> "Color [A=255, R=255, G=255, B=255]" Then
                            blnFoundPixel = True
                            Exit For
                        End If
                    Next
                    If blnFoundPixel Then Exit For
                Next
                'If MaxY = -1 Then MaxY = 1
                'MaxY += 3

                If MaxX > FontMaxX Then
                    FontMaxX = MaxX
                End If
                If MaxY > FontMaxY Then
                    FontMaxY = MaxY
                End If
                If StartX < FontMinX Then
                    FontMinX = StartX
                End If
                If StartY < FontMinY Then
                    FontMinY = StartY
                End If
            Else
                'g.DrawRectangle(New Pen(Drawing.Color.Black, 1), New System.Drawing.Rectangle(StartX, 0, MaxX - StartX, MaxY + 1))
                If charToDraw = Chr(32) Then
                    MaxX = bmpBW.Width / 4
                    StartX = 0
                ElseIf MaxX = -1 Then
                    StartX = 0
                    MaxX = FontMaxX
                End If
                Dim cropWidth As Integer = MaxX - StartX + 1
                Dim cropHeight = FontMaxY - FontMinY + 1
                If charToDraw = Chr(32) Then
                    cropWidth = bmpBW.Width / 4
                End If
                'StartX = FontMinX
                'StartY = (bHeigth - cropHeight) / 2
                'If StartX + cropWidth > bmpBW.Width Then cropWidth = bmpBW.Width - StartX
                'If FontMinY + cropHeight > bmpBW.Height Then cropHeight = bmpBW.Height - FontMinY
                Dim rect As New System.Drawing.Rectangle(StartX, FontMinY, cropWidth, cropHeight)
                bmFinal = New Bitmap(bmpBW, New Size(cropWidth, cropHeight))
                bmFinal = bmpBW.Clone(rect, bmpBW.PixelFormat)
            End If
            bmpBW.Dispose()

            Return bmFinal
        End Function

        Public Shared Function FontToHex(ByRef font As VGDDFont) As String
            Dim ms As MemoryStream = FontConvert(font)
            If ms Is Nothing Then Return ""
            Dim sbHex As New System.Text.StringBuilder, i As Long
            For i = 0 To ms.Length - 1
                If i > 0 AndAlso i / 33 = Int(i / 33) Then
                    sbHex.Append(vbCrLf)
                End If
                sbHex.Append(IIf(i = 0, " ", ",") & String.Format("0x{0:X2}", ms.ReadByte))
            Next
            ms.Close()
            ms.Dispose()
            Return sbHex.ToString
        End Function

        Public Shared Sub FontToBinFile(ByRef font As VGDDFont, ByVal Filename As String)
            If Filename = "" Then Exit Sub
            Dim fsOut As New FileStream(Filename, FileMode.Create)
            Dim swOut As New BinaryWriter(fsOut)
            Dim ms As MemoryStream = FontConvert(font)
            ms.Seek(0, SeekOrigin.Begin)
            For i As Long = 0 To ms.Length - 1
                swOut.Write(CByte(ms.ReadByte))
            Next
            fsOut.Flush()
            fsOut.Close()
            swOut.Close()
            ms.Dispose()
        End Sub

        Public Shared Function FontConvert(ByRef VFont As VGDDFont) As MemoryStream
            If Common.ProjectUseGRC Then
                Return FontConvertGRC(VFont)
            Else
                Return FontConvertInternal(VFont)
            End If
        End Function

        Public Shared Function FontConvertGRC(ByRef font As VGDDFont) As MemoryStream
            If font.ByteArray Is Nothing Then
                Using oGrc As New GrcProject
                    With oGrc
                        .ResourceType = GrcProject.ResourceTypes.Font
                        .ResourceSubType = GrcProject.ResourceSubTypes.Installed
                        .Font = font
                        Select Case Common.ProjectCompiler
                            Case "C30", "XC16"
                                .BuildType = GrcProject.BuildTypes.C30
                            Case "C32", "XC32"
                                .BuildType = GrcProject.BuildTypes.C32
                        End Select
                        .OutputFormat = GrcProject.OutputFormats.Binary
                        .AddResource()
                        FontConvertGRC = .Convert()
                        If .GrcOutput.Trim <> "" Then
                            Windows.Forms.MessageBox.Show("Error converting " & font.Name & ":" & vbCrLf & .GrcOutput, "GRC Font conversion error")
                        Else
                            Dim ByteArrayBuffer(FontConvertGRC.Length - 1) As Byte
                            FontConvertGRC.Read(ByteArrayBuffer, 0, FontConvertGRC.Length)
                            FontConvertGRC.Position = 0
                            font.ByteArray = ByteArrayBuffer
                        End If
                    End With
                End Using
            Else
                FontConvertGRC = New MemoryStream(font.ByteArray)
            End If
        End Function

        Public Shared Function FontConvertInternal(ByRef VFont As VGDDFont) As MemoryStream
            Dim FontHeigth As Integer = VFont.Font.SizeInPoints
            Dim CharsToConvert As Char() = Nothing
            Dim ms As New MemoryStream()
            Dim FontMaxWidth As Integer, FontMaxHeigth As Integer
            FontMinX = Integer.MaxValue
            FontMinY = Integer.MaxValue
            FontMaxX = 0
            FontMaxY = 0

            For i As Integer = 1 To 8
                ms.WriteByte(0)
            Next
            Select Case VFont.Charset
                Case VGDDFont.FontCharset.RANGE
                    ReDim CharsToConvert(VFont.EndChar - VFont.StartChar + 1)
                    For i As Integer = 0 To CharsToConvert.Length - 1
                        CharsToConvert(i) = Chr(VFont.StartChar + i)
                    Next
                Case VGDDFont.FontCharset.SELECTION
                    CharsToConvert = VFont.CharsIncluded
            End Select

            Dim aGlyph(4 * (CharsToConvert.Length)) As Byte
            ms.Write(aGlyph, 0, aGlyph.Length - 1)
            Dim intOffset As Int32 = 8 + aGlyph.Length - 1
            Dim iByte As Byte = 0, bmpData As Byte = 0

            For i As Integer = 0 To CharsToConvert.Length - 1
                Dim bm As Bitmap = FontCharToBitmap(VFont.Font, CharsToConvert(i), Color.Transparent, FontHeigth * 2, FontHeigth * 2)
            Next i

            FontMaxWidth = FontMaxX - FontMinX + 1
            FontMaxHeigth = FontMaxY - FontMinY + 1
            For i As Integer = 0 To CharsToConvert.Length - 1
                Dim bm As Bitmap = FontCharToBitmap(VFont.Font, CharsToConvert(i), Color.Black, FontHeigth * 2, FontHeigth * 2)
                aGlyph(i * 4) = bm.Width
                Dim MSB As Int16 = (intOffset >> 8)
                Dim LSB As Byte = (intOffset - MSB * 256)
                aGlyph(i * 4 + 1) = LSB
                aGlyph(i * 4 + 2) = LOBYTE(MSB)
                aGlyph(i * 4 + 3) = HIBYTE(MSB)
                For y As Integer = 0 To bm.Height - 1
                    iByte = 0
                    For x As Integer = 0 To bm.Width - 1
                        Dim pixColor As Color = bm.GetPixel(x, y)
                        bmpData = bmpData >> 1
                        If pixColor.ToString <> "Color [A=255, R=255, G=255, B=255]" Then
                            bmpData = CByte(bmpData Or &H80)
                        End If
                        iByte += 1
                        If iByte = 8 Then
                            ms.WriteByte(bmpData)
                            bmpData = 0
                            iByte = 0
                            intOffset += 1
                        End If
                    Next
                    If iByte <> 0 Then
                        For x As Byte = iByte To 8
                            bmpData = bmpData >> 1
                        Next
                        ms.WriteByte(bmpData)
                        bmpData = 0
                        iByte = 0
                        intOffset += 1
                    End If
                Next
                bm.Dispose()
            Next
            ms.Seek(0, SeekOrigin.Begin)
            ' FONT_HEADER
            ms.WriteByte(0)          ' fontID User assigned value
            ms.WriteByte(0)          ' BYTE res1        : 4;    // Reserved for future use (must be set to 0).
            '                          BYTE orientation : 2;    // Orientation of the character glyphs (0,90,180,270 degrees) 00 - Normal 01 - Characters rotated 270 degrees clockwise 10 - Characters rotated 180 degrees 11 - Characters rotated 90 degrees clockwise
            '                          BYTE res2        : 2;    // Reserved for future use (must be set to 0).
            Select Case VFont.Charset
                Case VGDDFont.FontCharset.RANGE
                    ms.WriteByte(LOBYTE(VFont.StartChar))  ' WORD firstChar;          // Character code of first character (e.g. 32).
                    ms.WriteByte(HIBYTE(VFont.StartChar))
                    ms.WriteByte(LOBYTE(VFont.EndChar))    ' WORD lastChar;           // Character code of last character in font (e.g. 3006).
                    ms.WriteByte(HIBYTE(VFont.EndChar))
                Case VGDDFont.FontCharset.SELECTION
                    ms.WriteByte(LOBYTE(Asc(VFont.CharsIncluded(0))))
                    ms.WriteByte(HIBYTE(Asc(VFont.CharsIncluded(0))))
                    ms.WriteByte(LOBYTE(Asc(VFont.CharsIncluded(0)) + VFont.CharsIncluded.Length - 1))
                    ms.WriteByte(HIBYTE(Asc(VFont.CharsIncluded(0)) + VFont.CharsIncluded.Length - 1))
            End Select
            ms.WriteByte(FontMaxHeigth) ' BYTE height;             // Font characters height in pixels.
            ms.WriteByte(0)          ' BYTE reserved;           // Reserved for future use (must be set to 0).

            ms.Write(aGlyph, 0, aGlyph.Length - 1)
            ms.Flush()
            ms.Seek(0, SeekOrigin.Begin)
            VFont._BinSize = ms.Length
            Return (ms)
        End Function
#End Region

        Public Shared Function LOBYTE(ByVal val As Int16) As Byte
            Return Convert.ToByte(val And &HFF)
        End Function

        Public Shared Function HIBYTE(ByVal val As Int16) As Byte
            Return Convert.ToByte((val And &HFF00) / &H100)
        End Function
    End Class

End Namespace