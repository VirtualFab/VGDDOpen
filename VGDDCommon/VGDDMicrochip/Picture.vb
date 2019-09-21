Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "PictureIco")> _
    Public Class Picture : Inherits VGDDWidgetWithBitmap

        Private _Frame As EnabledState
        Private _ShowWhenHidden As Boolean = True
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Picture")
#End If
            MyBase.Size = New Size(150, 150)
            If Common.VGDDIsRunning Then
                Me.RemovePropertyToShow("Text")
                'Me.RemovePropertyToShow("Hidden")
                MyBase.AddPropertyToShow("Bitmap")
            End If
        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                    If Me.Bitmap IsNot Nothing AndAlso Common.GetBitmap(Me.Bitmap) IsNot Nothing Then
                        Common.GetBitmap(Me.Bitmap).RemoveUsedBy(Me)
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Instances As Integer
            Get
                Return _Instances
            End Get
        End Property
        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            'Me.OnPaintBackground(pevent)

            'Dim Mypath As GraphicsPath = New GraphicsPath
            'Mypath.AddRectangle(New Drawing.Rectangle(0, 0, Me.Width, Me.Height))
            'Me.Region = New Region(Mypath)
            'g.DrawRectangle(Pens.Red, 0, 0, Me.Width - 1, Me.Height - 1)
            'Exit Sub

            If _Hidden And Not _ShowWhenHidden Then
                Exit Sub
            End If

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

            'Impostazione Region

            Dim Mypath As GraphicsPath = New GraphicsPath
            If _VGDDImage Is Nothing Then Exit Sub
            If _VGDDImage._GraphicsPath IsNot Nothing Then
                Mypath.AddPath(_VGDDImage._GraphicsPath, False)
                Me.Region = New Region(Mypath)
            Else
                Mypath.AddRectangle(Me.ClientRectangle)
            End If

            'Mypath = New GraphicsPath
            'Mypath.StartFigure()
            'Mypath.AddRectangle(Me.ClientRectangle)
            'Mypath.AddRectangle(New Drawing.Rectangle(rc.Left + 2, rc.Top + 2, rc.Width - 4, rc.Height - 4))
            'Mypath.CloseFigure()
            'Me.Region.Union(Mypath)

            'Me.Region = New Region(Me._GraphicsPath)
            If _Scheme Is Nothing Then Exit Sub
            If _Hidden Then
                If _VGDDImage.TransparentBitmap IsNot Nothing Then
                    Dim cm As New Imaging.ColorMatrix
                    cm.Matrix33 = 0.2
                    Dim ia As New Imaging.ImageAttributes
                    ia.SetColorMatrix(cm)
                    Try
                        g.DrawImage(_VGDDImage.TransparentBitmap, rc, 0, 0, _VGDDImage.TransparentBitmap.Width, _VGDDImage.TransparentBitmap.Height, GraphicsUnit.Pixel, ia)
                    Catch ex As Exception
                    End Try
                Else
                    Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
                    g.FillRegion(brushBackGround, Me.Region)
                End If
                Exit Sub
            End If

            Try
                If _VGDDImage IsNot Nothing AndAlso _VGDDImage.TransparentBitmap IsNot Nothing Then
                    'g.DrawImage(_VGDDImage.TransparentBitmap, 0, 0, _VGDDImage.TransparentBitmap.Width - 1, _VGDDImage.TransparentBitmap.Height - 1)
                    g.DrawImage(_VGDDImage.TransparentBitmap, 0, 0)
                End If
            Catch ex As Exception
                '_VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
            End Try

            If _Frame = EnabledState.Enabled Then
                Dim brushPen As New SolidBrush(_Scheme.Color1)
                Dim ps As Pen = New Pen(brushPen)
                ps.Color = _Scheme.Embossdkcolor
                'g.DrawRectangle(ps, rc.Left + 1, rc.Top + 1, rc.Right - 2, rc.Bottom - 2)
                g.DrawRectangle(ps, 0, 0, rc.Width - 1, rc.Height - 1)
                'g.DrawRectangle(ps, rc)
            End If
        End Sub

#Region "IVGDD Stubs"


#End Region

#Region "GDDProps"

        <Description("Show a placeholder in designer when Picture is hidden?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(True)> _
        <Category("Design")> _
        Public Overloads Property ShowWhenHidden() As Boolean
            Get
                Return _ShowWhenHidden
            End Get
            Set(ByVal value As Boolean)
                _ShowWhenHidden = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Whether to enable a frame around the Picture or not")> _
        <DefaultValue(GetType(EnabledState), "Disabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Frame() As EnabledState
            Get
                Return _Frame
            End Get
            Set(ByVal value As EnabledState)
                _Frame = value
                Me.Invalidate()
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then
        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = ""
            Dim MyCode As String = "", MyState As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            If _public Then
                CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            Else
                CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            End If
            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", IIf(_State = EnabledState.Enabled, True, False))
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)

            Dim MyBitmap As String = "", MyPointerInit As String = ""
            If Me.Bitmap = String.Empty Then
                MyBitmap = "NULL"
                If Common.CodeGenWarnEmptyBitmaps Then
                    MessageBox.Show("Warning: Picture " & Me.Name & " on screen " & Me.Parent.Name & " has empty bitmap. Runtime errors will occur", "Warning")
                End If
                CodeGenWarnEmptyBitmaps = False
            Else
                Dim myBitmapName As String = System.IO.Path.GetFileNameWithoutExtension(Me.Bitmap)
                If Me.BitmapUsePointer Then
                    Dim strPointerName As String = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerName", 0).Trim _
                                                   .Replace("[BITMAPNAME]", myBitmapName)
                    MyBitmap = "(void *)" & strPointerName
                    MyPointerInit = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerInit", 0).Trim _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName) _
                        .Replace("[BITMAPNAME]", myBitmapName)
                    CodeGen.AddLines(CodeGen.Headers, CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/Bitmaps", 0) _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[BITMAPNAME]", myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName))
                Else
                    MyBitmap = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName
                End If
            End If
            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[SCALE]", Me.Scale) _
                .Replace("[BITMAP]", MyBitmap) _
                .Replace("[BITMAP_POINTER_INIT]", MyPointerInit) _
                .Replace("[SCHEME]", Me.Scheme))
            CodeGen.AddLines(MyCodeHead, CodeGen.CodeHeadTemplate)
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, (IIf(Me.Public, vbCrLf & "extern " & CodeGen.ConstructorTemplate.Trim, "") & CodeGen.HeadersTemplate) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, Me)
        End Sub
#End If

    End Class

End Namespace
