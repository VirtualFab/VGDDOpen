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
    <ToolboxBitmap(GetType(Button), "ButtonIco")> _
    Public Class Button : Inherits VGDDWidgetWithBitmap

        Private _Radius As Integer = 0
        Private _HorizAlign As HorizAlign = HorizAlign.Center
        Private _VertAlign As VertAlign = VertAlign.Center
        Private _Pressed As Boolean = False
        Private _HasFocus As Boolean = False
        Private _NoPanel As Boolean = False
        Private _TwoTone As Boolean = False
        Private _Toggle As Boolean = False
        Private _BitmapNamePressed As String
        Protected _VGDDImagePressed As VGDDImage

        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Button")
            If Common.VGDDIsRunning Then
                'MyBase.AddPropertyToShow("Bitmap")
                Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                    Case "MLALEGACY"
                        Me.RemovePropertyToShow("BitmapPressed")
                End Select
            End If
#End If
            'MyBase._ShrinkSizeToBitmapSize = False
            MyBase._BitmapNeeded = False
            Me.Size = New Size(100, 25)
        End Sub

        '<System.Diagnostics.DebuggerNonUserCode()> _
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

        Protected Overrides Sub OnPaintBackGround(ByVal pevent As PaintEventArgs)
            If _NoPanel Then
                MyBase.OnPaintBackground(pevent)
            End If
        End Sub

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            If Me.Scheme Is Nothing OrElse Me.Scheme = String.Empty Then
                If Not Common.VGDDIsRunning Then
                    'Me.Scheme = Common.CreateDesignScheme
                Else
                    Exit Sub
                End If
            End If
            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            'Me.OnPaintBackground(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle
            Dim Rad2 As Integer = _Radius

            Dim BitmapX As Short
            Dim BitmapY As Short
            If _VGDDImage IsNot Nothing AndAlso _VGDDImage.Bitmap IsNot Nothing Then
                BitmapX = (Me.Width - _VGDDImage.Bitmap.Width) / 2 '- 6
                BitmapY = (Me.Height - _VGDDImage.Bitmap.Height) / 2 '- 6
                If BitmapX < 0 Then BitmapX = 0
                If BitmapY < 0 Then BitmapY = 0
            End If

            Dim faceClr1 As Color, faceClr2 As Color, embossDkClr As Color, embossLtClr As Color, txtColor As Color

            If Me.State = EnabledState.Enabled Then
                If Me._TwoTone Then
                    embossDkClr = _Scheme.Color0
                    embossLtClr = _Scheme.Color0
                    txtColor = _Scheme.Textcolor1
                    If Me._Pressed Then
                        faceClr1 = _Scheme.Embossdkcolor
                        faceClr2 = _Scheme.Embossltcolor
                    Else
                        faceClr1 = _Scheme.Embossltcolor
                        faceClr2 = _Scheme.Embossdkcolor
                    End If
                Else
                    If Me._Pressed Then
                        faceClr1 = _Scheme.Color1
                        embossDkClr = _Scheme.Embossltcolor
                        embossLtClr = _Scheme.Embossdkcolor
                        txtColor = _Scheme.Textcolor1
                    Else
                        faceClr1 = _Scheme.Color0
                        embossDkClr = _Scheme.Embossdkcolor
                        embossLtClr = _Scheme.Embossltcolor
                        txtColor = _Scheme.Textcolor0
                    End If
                End If
            Else
                faceClr1 = _Scheme.Colordisabled
                embossDkClr = _Scheme.Embossdkcolor
                embossLtClr = _Scheme.Embossltcolor
                txtColor = _Scheme.Textcolordisabled
            End If

            Dim intTextSize As SizeF
            Try
                intTextSize = g.MeasureString(Me.Text, MyBase.Font)
            Catch ex As Exception
                MyBase.Font = New Font("Arial", 10)
                intTextSize = New SizeF(10, 18)
            End Try
            Dim tx As Integer, ty As Integer
            Select Case _VertAlign
                Case VertAlign.Center
                    ty = (Height - intTextSize.Height) / 2
                Case VertAlign.Bottom
                    ty = (Height - intTextSize.Height) + 1
                Case VertAlign.Top
                    ty = 2
            End Select
            Select Case _HorizAlign
                Case HorizAlign.Center
                    tx = (Width - intTextSize.Width) / 2
                Case HorizAlign.Right
                    tx = Width - intTextSize.Width
                Case HorizAlign.Left
                    tx = 0
            End Select
            If _NoPanel Then
                If _VGDDImage IsNot Nothing AndAlso _VGDDImage.Bitmap IsNot Nothing AndAlso _VGDDImage._GraphicsPath IsNot Nothing Then
                    'If _Scheme.BackgroundType = VGDDScheme.GFX_BACKGROUND_TYPE.GFX_BACKGROUND_IMAGE Then
                    'Dim backBmp As Bitmap = CType(Common.Bitmaps(_Scheme.BackgroundImageName), VGDDImage).Bitmap
                    'Dim backBmpPart As New Bitmap(Me.Width, Me.Height)
                    'Using gr As Graphics = Graphics.FromImage(backBmpPart)
                    '    'gr.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                    'Dim srcRect As New Drawing.Rectangle(_Scheme.CommonBkLeft - Me.Left, _Scheme.CommonBkTop - Me.Top, Me.Width - 1, Me.Height - 1)
                    '    gr.DrawImage(backBmp, 0, 0, srcRect, GraphicsUnit.Pixel)
                    '    gr.DrawRectangle(Pens.Red, 0, 0, Me.Width - 1, Me.Height - 1)
                    'End Using
                    'Me.BackgroundImage = backBmpPart
                    'g.DrawImage(backBmp, 0, 0, srcRect, GraphicsUnit.Pixel)
                    'End If
                    'Me.Region = New Region(New Drawing.Rectangle(BitmapX, BitmapY, _VGDDImage.TransparentBitmap.Width, _VGDDImage.TransparentBitmap.Height))
                    'Dim p As New GraphicsPath
                    'p.AddRectangle(rc)
                    'p.CloseFigure()
                    'Me.Region = New Region(p)
                    'Me.Region = New Region(rc)
                    'Me.Region.Intersect(_VGDDImage._GraphicsPath)
                    'Me.Region.
                    'Me.Region = New Region(_VGDDImage._GraphicsPath)
                    'Me.Region = New Region(Common.CalculateControlGraphicsPath(_VGDDImage.Bitmap, _VGDDImage.ParentTransparentColour, BitmapX, BitmapY))
                    Dim rcText As New Drawing.Rectangle(tx, ty, intTextSize.Width, intTextSize.Height)
                    'Me.Region.Union(rcText)
                    'Me.Region = New Region(rcText)
                    Dim gp As GraphicsPath = Common.CalculateControlGraphicsPath(_VGDDImage.Bitmap, _VGDDImage.ParentTransparentColour, BitmapX, BitmapY)
                    Select Case _VertAlign
                        Case VertAlign.Center
                            Me.Region = New Region(gp)
                            Me.Region.Union(rcText)
                        Case VertAlign.Bottom, VertAlign.Top
                            gp.AddRectangle(rcText)
                            Me.Region = New Region(gp)
                    End Select

                    'g.FillRegion(Brushes.Red, Me.Region)
                    'Return
                Else
                    Me.Region = New Region(rc)
                    'Exit Sub 
                    Dim brushBackGround As SolidBrush
                    If TypeOf (Me.Parent) Is VGDD.VGDDScreen Then
                        brushBackGround = New SolidBrush(CType(Me.Parent, VGDD.VGDDScreen).BackColor)
                    Else
                        brushBackGround = New SolidBrush(_Scheme.Commonbkcolor)
                    End If
                    g.FillRegion(brushBackGround, Me.Region)
                End If
            Else
                'Impostazione Region
                If Me._TwoTone Then
                    Dim Mypath1 As New GraphicsPath
                    Mypath1.AddLine(rc.Left + Rad2, rc.Top, rc.Right - Rad2, rc.Top)
                    If Radius > 0 Then
                        Mypath1.AddArc(rc.Right - _Radius * 2, rc.Top, _Radius * 2, _Radius * 2, 270, 90)
                    End If
                    Mypath1.AddLine(rc.Right, rc.Top + Rad2, rc.Right, rc.Bottom \ 2)
                    Mypath1.AddLine(rc.Right, rc.Bottom \ 2, rc.Left, rc.Bottom \ 2)
                    Mypath1.AddLine(rc.Left, rc.Bottom \ 2, rc.Left, rc.Top + _Radius * 2)
                    If Radius > 0 Then
                        Mypath1.AddArc(rc.Left, rc.Top, _Radius * 2, _Radius * 2, 180, 90)
                    End If

                    Dim Mypath2 As New GraphicsPath
                    Mypath2.AddLine(rc.Right, rc.Bottom \ 2, rc.Right, rc.Bottom - Radius)
                    If Radius > 0 Then
                        Mypath2.AddArc(rc.Right - _Radius * 2, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 0, 90)
                    End If
                    Mypath2.AddLine(rc.Right - Rad2, rc.Bottom, rc.Left + _Radius * 2, rc.Bottom)
                    If Radius > 0 Then
                        Mypath2.AddArc(rc.Left, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 90, 90)
                    End If
                    Mypath2.AddLine(rc.Left, rc.Bottom - Rad2, rc.Left, rc.Bottom \ 2)
                    Mypath2.AddLine(rc.Left, rc.Bottom \ 2, rc.Right, rc.Bottom \ 2)

                    Dim MyPathFull As New GraphicsPath
                    MyPathFull.AddPath(Mypath1, False)
                    MyPathFull.AddPath(Mypath2, False)
                    Me.Region = New Region(MyPathFull)

                    Dim r1 As New Region(Mypath1)
                    Dim r2 As New Region(Mypath2)

                    Dim brushBackGround1 As New SolidBrush(faceClr1)
                    g.FillRegion(brushBackGround1, r1)
                    Dim brushBackGround2 As New SolidBrush(faceClr2)
                    g.FillRegion(brushBackGround2, r2)
                Else
                    Dim Mypath As GraphicsPath = New GraphicsPath
                    Mypath.StartFigure()
                    Mypath.AddLine(rc.Left + Rad2, rc.Top, rc.Right - Rad2, rc.Top)
                    If Radius > 0 Then
                        Mypath.AddArc(rc.Right - _Radius * 2, rc.Top, _Radius * 2, _Radius * 2, 270, 90)
                    End If
                    Mypath.AddLine(rc.Right, rc.Top + Rad2, rc.Right, rc.Bottom - Radius)
                    If Radius > 0 Then
                        Mypath.AddArc(rc.Right - _Radius * 2, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 0, 90)
                    End If
                    Mypath.AddLine(rc.Right - Rad2, rc.Bottom, rc.Left + _Radius * 2, rc.Bottom)
                    If Radius > 0 Then
                        Mypath.AddArc(rc.Left, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 90, 90)
                    End If
                    Mypath.AddLine(rc.Left, rc.Bottom - Rad2, rc.Left, rc.Top + _Radius * 2)
                    If Radius > 0 Then
                        Mypath.AddArc(rc.Left, rc.Top, _Radius * 2, _Radius * 2, 180, 90)
                    End If
                    Mypath.CloseFigure()
                    Me.Region = New Region(Mypath)

                    'Dim brushBackGround As New SolidBrush(_Scheme.Color0)
                    'g.FillRegion(brushBackGround, Me.Region)
                    '    End If
                    'End If

                    If MyBase._State = EnabledState.Disabled And _Scheme.FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_COLOR Then
                        Dim brushBackGround As New SolidBrush(faceClr1)
                        g.FillRegion(brushBackGround, Me.Region)
                    Else
                        'Select Case _Scheme.FillStyle
                        '    Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_COLOR
                        '        Dim brushBackGround As New SolidBrush(faceClr1)   'Control BgColor
                        '        g.FillRegion(brushBackGround, Me.Region)
                        '    Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_VER
                        '        Dim Rect1 As New Drawing.Rectangle(rc.X, rc.Y, rc.Width \ 2, rc.Height)
                        '        Dim Rect2 As New Drawing.Rectangle(rc.X + rc.Width \ 2, rc.Y, rc.Width \ 2, rc.Height)
                        '        Dim brushBackGround As New LinearGradientBrush(Rect1, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 0)
                        '        Dim brushBackGround2 As New LinearGradientBrush(Rect2, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 180)
                        '        g.FillRectangle(brushBackGround, Rect1)
                        '        g.FillRectangle(brushBackGround2, Rect2)
                        '    Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_HOR
                        '        Dim Rect1 As New Drawing.Rectangle(rc.X, rc.Y, rc.Width, rc.Height \ 2)
                        '        Dim Rect2 As New Drawing.Rectangle(rc.X, rc.Y + rc.Height \ 2, rc.Width, rc.Height \ 2)
                        '        Dim brushBackGround As New LinearGradientBrush(Rect1, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 90)
                        '        Dim brushBackGround2 As New LinearGradientBrush(Rect2, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 270)
                        '        g.FillRectangle(brushBackGround, Rect1)
                        '        g.FillRectangle(brushBackGround2, Rect2)
                        '    Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOWN
                        '        Dim brushBackGround As New LinearGradientBrush(rc, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 90)
                        '        g.FillRegion(brushBackGround, Me.Region)
                        '    Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_LEFT
                        '        Dim brushBackGround As New LinearGradientBrush(rc, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 180)
                        '        g.FillRegion(brushBackGround, Me.Region)
                        '    Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_RIGHT
                        '        Dim brushBackGround As New LinearGradientBrush(rc, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 0)
                        '        g.FillRegion(brushBackGround, Me.Region)
                        '    Case VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_UP
                        '        Dim brushBackGround As New LinearGradientBrush(rc, _Scheme.GradientStartColor, _Scheme.GradientEndColor, 270)
                        '        g.FillRegion(brushBackGround, Me.Region)
                        'End Select

                        MyBase.FillBackground(g, faceClr1, Me.Region)

                    End If
                End If
            End If

            If Not _NoPanel And _Scheme.EmbossSize > 0 Then
                Dim brushPen As New SolidBrush(_Scheme.Textcolor0)
                Dim ps As Pen = New Pen(brushPen)

                ps.Width = _Scheme.EmbossSize
                ps.Color = embossLtClr
                g.DrawLine(ps, rc.Left + _Radius, rc.Top + 1, rc.Right - _Radius, rc.Top + 1)
                ps.Color = embossDkClr
                If Radius > 0 Then
                    ps.Width = _Scheme.EmbossSize * 2
                    g.DrawArc(ps, rc.Right - _Radius * 2, rc.Top, _Radius * 2, _Radius * 2, 270, 90)
                    ps.Width = _Scheme.EmbossSize
                End If
                g.DrawLine(ps, rc.Right - 1, rc.Top + _Radius - 1, rc.Right - 1, rc.Bottom - _Radius + 1)
                If Radius > 0 Then
                    ps.Width = _Scheme.EmbossSize * 2
                    g.DrawArc(ps, rc.Right - _Radius * 2, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 0, 90)
                    ps.Width = _Scheme.EmbossSize
                End If
                g.DrawLine(ps, rc.Right - _Radius + 1, rc.Bottom - 1, rc.Left + _Radius - 1, rc.Bottom - 1)
                ps.Color = embossLtClr
                If Radius > 0 Then
                    ps.Width = _Scheme.EmbossSize * 2
                    g.DrawArc(ps, rc.Left, rc.Bottom - _Radius * 2, _Radius * 2, _Radius * 2, 90, 90)
                    ps.Width = _Scheme.EmbossSize
                End If
                g.DrawLine(ps, rc.Left + 1, rc.Bottom - _Radius + 1, rc.Left + 1, rc.Top + _Radius - 1)
                If Radius > 0 Then
                    ps.Width = _Scheme.EmbossSize * 2
                    g.DrawArc(ps, rc.Left, rc.Top, _Radius * 2, _Radius * 2, 180, 90)
                    ps.Width = _Scheme.EmbossSize
                End If
            End If
            If _VGDDImage IsNot Nothing AndAlso _VGDDImage.Bitmap IsNot Nothing Then
                If _Scheme.FillStyle = VGDDScheme.GFX_FILL_STYLE.GFX_FILL_STYLE_ALPHA_COLOR Then
                    Dim ptsArray()() As Single = {
                        New Single() {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                        New Single() {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                        New Single() {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                        New Single() {0.0F, 0.0F, 0.0F, 0.5F, 0.0F},
                        New Single() {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                        }
                    Dim ClrMatrix As New Imaging.ColorMatrix(ptsArray)
                    ClrMatrix.Matrix33 = _Scheme.AlphaValue / 100
                    Dim ImgAttr As New Imaging.ImageAttributes
                    ImgAttr.SetColorMatrix(ClrMatrix, Imaging.ColorMatrixFlag.Default, Imaging.ColorAdjustType.Bitmap)
                    Dim srcRect As New Drawing.Rectangle(0, 0, _VGDDImage.Bitmap.Width, _VGDDImage.Bitmap.Height)
                    Dim dstRect As New Drawing.Rectangle(BitmapX, BitmapY, Me.Width - BitmapX, Me.Height - BitmapY)
                    g.DrawImage(_VGDDImage.Bitmap, dstRect, 0, 0, _VGDDImage.Bitmap.Width, _VGDDImage.Bitmap.Height, GraphicsUnit.Pixel, ImgAttr)
                Else
                    g.DrawImage(_VGDDImage.Bitmap, BitmapX, BitmapY)
                End If
            End If

            'Draw Text 
            Dim textBrush As SolidBrush = New SolidBrush(txtColor)

            Try
                g.DrawString(Me.Text, MyBase.Font, textBrush, tx, ty)
            Catch ex As Exception
            End Try
        End Sub

#Region "GDDProps"

        <Description("Bitmap name to draw when button is pressed")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overridable Property BitmapPressed() As String
            Get
                Return _BitmapNamePressed
            End Get
            Set(ByVal value As String)
                _BitmapNamePressed = value
                If value = String.Empty Then
                    If _VGDDImagePressed IsNot Nothing Then
                        _VGDDImagePressed.RemoveUsedBy(Me)
                    End If
                    _VGDDImagePressed = Nothing
                ElseIf Not Me.IsLoading Then
                    Common.SetBitmapName(value, Me, _BitmapNamePressed, _VGDDImagePressed, Nothing)
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Two-tone Button?")> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property TwoTone As Boolean
            Get
                Return _TwoTone
            End Get
            Set(ByVal value As Boolean)
                _TwoTone = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Should the Button panel be drawn? Set to false for faster drawing if using a bitmap larger than the button.")> _
        <CustomSortedCategory("Appearance", 4)> _
        <DefaultValue(False)> _
        Public Property NoPanel As Boolean
            Get
                Return _NoPanel
            End Get
            Set(ByVal value As Boolean)
                _NoPanel = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Focused initial state")> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property HasFocus As Boolean
            Get
                Return _HasFocus
            End Get
            Set(ByVal value As Boolean)
                _HasFocus = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Set Toggle behaviour for this button")> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Toggle As Boolean
            Get
                Return _Toggle
            End Get
            Set(ByVal value As Boolean)
                _Toggle = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Pressed initial state")> _
        <DefaultValue(False)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Pressed As Boolean
            Get
                Return _Pressed
            End Get
            Set(ByVal value As Boolean)
                _Pressed = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Define the radius roundness of the button")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 256)> _
        <DefaultValue(0)> _
        <CustomSortedCategory("Appearance", 4)> _
        Property Radius() As Integer
            Get
                Return _Radius
            End Get
            Set(ByVal value As Integer)
                _Radius = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Horizontal Text Alignement of the button")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Center")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property HorizAlign() As HorizAlign
            Get
                Return _HorizAlign
            End Get
            Set(ByVal value As HorizAlign)
                _HorizAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Vertical Text Alignement of the button")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(VertAlign), "Center")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property VertAlign() As VertAlign
            Get
                Return _VertAlign
            End Get
            Set(ByVal value As VertAlign)
                _VertAlign = value
                Me.Invalidate()
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.MyCodeHead(_CDeclType)
            Dim MyCode As String = String.Empty, MyState As String = String.Empty, MyAlignment As String = String.Empty
            Dim MyEventCode As String = ""

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

            If Me.Text = String.Empty Then
                MyCodeHead = ""
            End If
            CodeGen.AddState(MyState, "Enabled", IIf(_State = EnabledState.Enabled, True, False))
            CodeGen.AddState(MyState, "Hidden", _Hidden.ToString)
            CodeGen.AddState(MyState, "NoPanel", _NoPanel.ToString)
            CodeGen.AddState(MyState, "Pressed", _Pressed.ToString)
            CodeGen.AddState(MyState, "TwoTone", _TwoTone.ToString)
            CodeGen.AddState(MyState, "HasFocus", _HasFocus.ToString)
            CodeGen.AddState(MyState, "Toggle", _Toggle.ToString)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    CodeGen.AddState(MyState, "HorizAlign", _HorizAlign.ToString)
                    CodeGen.AddState(MyState, "VertAlign", _VertAlign.ToString)
                Case "MLA", "HARMONY"
                    CodeGen.AddAlignment(MyAlignment, "Horizontal", _HorizAlign.ToString)
                    CodeGen.AddAlignment(MyAlignment, "Vertical", _VertAlign.ToString)
            End Select

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            Dim MyBitmapReleased As String = String.Empty, MyPointerInitReleased As String = String.Empty
            Dim MyBitmapPressed As String = String.Empty, MyPointerInitPressed As String = String.Empty
            MyBitmapReleased = CodeGen.BitmapStatement(Me, Me.Bitmap, MyPointerInitReleased)
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLA", "HARMONY"
                    MyBitmapPressed = CodeGen.BitmapStatement(Me, Me.BitmapPressed, MyPointerInitPressed)
                    CodeGen.AddLines(MyPointerInitReleased, MyPointerInitPressed)
            End Select

            If Common.ProjectMultiLanguageTranslations > 0 AndAlso MyBase.TextStringID < 0 Then
                CodeGen.Errors &= MyControlId & " has empty text ID" & vbCrLf
            End If

            CodeGen.AddLines(CodeGen.Code, MyCode _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[RADIUS]", Radius) _
                .Replace("[STATE]", MyState) _
                .Replace("[ALIGNMENT]", MyAlignment) _
                .Replace("[BITMAP_POINTER_INIT]", MyPointerInitReleased) _
                .Replace("[BITMAP]", MyBitmapReleased) _
                .Replace("[RELEASEDBITMAP]", MyBitmapReleased) _
                .Replace("[PRESSEDBITMAP]", MyBitmapPressed) _
                .Replace("[SCHEME]", Me.Scheme) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)

            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            Dim MyHeaders As String = String.Empty
            If Me.Public Then
                CodeGen.AddLines(MyHeaders, "extern " & CodeGen.ConstructorTemplate.Trim)
            End If
            If Me.Text <> String.Empty Then
                CodeGen.AddLines(MyHeaders, CodeGen.MyHeader(_CDeclType))
            End If
            CodeGen.AddLines(MyHeaders, CodeGen.TextDeclareHeaderTemplate(_CDeclType))
            CodeGen.AddLines(MyHeaders, CodeGen.HeadersTemplate)
            CodeGen.AddLines(CodeGen.Headers, MyHeaders _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, Me)
        End Sub
#End If

        Private Sub PlayerInvalidate()
            Dim oControls As ControlCollection = Nothing
            If Me.Parent IsNot Nothing Then
                oControls = Me.Parent.Controls
            End If
            If oControls IsNot Nothing Then
                For Each oControl As Control In oControls
                    oControl.Invalidate()
                Next
            Else
                Me.Invalidate()
            End If
        End Sub

        Private Sub Button_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
            Me._Pressed = True
            PlayerInvalidate()
        End Sub

        Private Sub Button_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
            Me._Pressed = False
            PlayerInvalidate()
        End Sub

        Private Sub Button_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION AndAlso _Scheme.Font.SmartCharSet Then
                _Scheme.Font.SmartCharSetAddString(Me.Text)
            End If
        End Sub

        Private Sub Button_FinishedLoading(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FinishedLoading
            Button_TextChanged(Nothing, Nothing)
            If _BitmapNamePressed IsNot Nothing Then
                _VGDDImagePressed = New VGDDImage
                Common.SetBitmapName(_BitmapNamePressed, Me, _BitmapNamePressed, _VGDDImagePressed, Nothing)
            End If
        End Sub

        'Protected Overrides Sub VGDDWidgetWithBitmap_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
        '    If _VGDDImage IsNot Nothing Then
        '        If _VGDDImage.AllowScaling Then
        '            _VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
        '            'BuildTransparentBitmap(_VGDDImage.Bitmap)
        '            Me.Invalidate()
        '        ElseIf _VGDDImage.Bitmap IsNot Nothing Then 'AndAlso MyBase.Size <> _VGDDImage.Bitmap.Size Then
        '            '    MyBase.Size = _VGDDImage.Bitmap.Size
        '        End If
        '    End If
        'End Sub

    End Class

End Namespace