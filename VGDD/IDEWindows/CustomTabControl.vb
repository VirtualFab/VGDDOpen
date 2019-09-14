'
' * This code is provided under the Code Project Open Licence (CPOL)
' * See http://www.codeproject.com/info/cpol10.aspx for details
' 


'Imports System
'Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
'Imports Drawing2D
'Imports Imaging
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports System.Reflection
Imports System.Windows.Forms

'Namespace System.Windows.Forms

<ToolboxBitmapAttribute(GetType(TabControl))> _
Public Class CustomTabControl
    Inherits TabControl

#Region "Construction"

    Public Sub New()

        Me.SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.Opaque Or ControlStyles.ResizeRedraw, True)

        Me._BackBuffer = New Bitmap(Me.Width, Me.Height)
        Me._BackBufferGraphics = Graphics.FromImage(Me._BackBuffer)
        Me._TabBuffer = New Bitmap(Me.Width, Me.Height)
        Me._TabBufferGraphics = Graphics.FromImage(Me._TabBuffer)

        Me.DisplayStyle = TabStyle.[Default]
    End Sub

    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        Me.OnFontChanged(EventArgs.Empty)
    End Sub

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            If Me.RightToLeftLayout Then
                cp.ExStyle = cp.ExStyle Or CustomTabNativeMethods.WS_EX_LAYOUTRTL Or CustomTabNativeMethods.WS_EX_NOINHERITLAYOUT
            End If
            Return cp
        End Get
    End Property

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
        If disposing Then
            If Me._BackImage IsNot Nothing Then
                Me._BackImage.Dispose()
            End If
            If Me._BackBufferGraphics IsNot Nothing Then
                Me._BackBufferGraphics.Dispose()
            End If
            If Me._BackBuffer IsNot Nothing Then
                Me._BackBuffer.Dispose()
            End If
            If Me._TabBufferGraphics IsNot Nothing Then
                Me._TabBufferGraphics.Dispose()
            End If
            If Me._TabBuffer IsNot Nothing Then
                Me._TabBuffer.Dispose()
            End If

            If Me._StyleProvider IsNot Nothing Then
                Me._StyleProvider.Dispose()
            End If
        End If
    End Sub

#End Region

#Region "Private variables"

    Private _BackImage As Bitmap
    Private _BackBuffer As Bitmap
    Private _BackBufferGraphics As Graphics
    Private _TabBuffer As Bitmap
    Private _TabBufferGraphics As Graphics

    Private _oldValue As Integer
    Private _dragStartPosition As Point = Point.Empty

    Private _Style As TabStyle
    Private _StyleProvider As TabStyleProvider

    Private _TabPages As List(Of TabPage)

#End Region

#Region "Public properties"

    <Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    Public Property DisplayStyleProvider() As TabStyleProvider
        Get
            If Me._StyleProvider Is Nothing Then
                Me.DisplayStyle = TabStyle.[Default]
            End If

            Return Me._StyleProvider
        End Get
        Set(ByVal value As TabStyleProvider)
            Me._StyleProvider = Value
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(TabStyle), "Default"), RefreshProperties(RefreshProperties.All)> _
    Public Property DisplayStyle() As TabStyle
        Get
            Return Me._Style
        End Get
        Set(ByVal value As TabStyle)
            If Me._Style <> Value Then
                Me._Style = Value
                Me._StyleProvider = TabStyleProvider.CreateProvider(Me)
                Me.Invalidate()
            End If
        End Set
    End Property

    <Category("Appearance"), RefreshProperties(RefreshProperties.All)> _
    Public Shadows Property Multiline() As Boolean
        Get
            Return MyBase.Multiline
        End Get
        Set(ByVal value As Boolean)
            MyBase.Multiline = Value
            Me.Invalidate()
        End Set
    End Property


    '	Hide the Padding attribute so it can not be changed
    '	We are handling this on the Style Provider
    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Shadows Property Padding() As Point
        Get
            Return Me.DisplayStyleProvider.Padding
        End Get
        Set(ByVal value As Point)
            Me.DisplayStyleProvider.Padding = Value
        End Set
    End Property

    Public Overrides Property RightToLeftLayout() As Boolean
        Get
            Return MyBase.RightToLeftLayout
        End Get
        Set(ByVal value As Boolean)
            MyBase.RightToLeftLayout = Value
            Me.UpdateStyles()
        End Set
    End Property


    '	Hide the HotTrack attribute so it can not be changed
    '	We are handling this on the Style Provider
    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Shadows Property HotTrack() As Boolean
        Get
            Return Me.DisplayStyleProvider.HotTrack
        End Get
        Set(ByVal value As Boolean)
            Me.DisplayStyleProvider.HotTrack = Value
        End Set
    End Property

    <Category("Appearance")> _
    Public Shadows Property Alignment() As TabAlignment
        Get
            Return MyBase.Alignment
        End Get
        Set(ByVal value As TabAlignment)
            MyBase.Alignment = Value
            Select Case Value
                Case TabAlignment.Top, TabAlignment.Bottom
                    Me.Multiline = False
                    Exit Select
                Case TabAlignment.Left, TabAlignment.Right
                    Me.Multiline = True
                    Exit Select

            End Select
        End Set
    End Property

    '	Hide the Appearance attribute so it can not be changed
    '	We don't want it as we are doing all the painting.
    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    <CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="value")> _
    Public Shadows Property Appearance() As TabAppearance
        Get
            Return MyBase.Appearance
        End Get
        Set(ByVal value As TabAppearance)
            '	Don't permit setting to other appearances as we are doing all the painting
            MyBase.Appearance = TabAppearance.Normal
        End Set
    End Property

    Public Overrides ReadOnly Property DisplayRectangle() As Rectangle
        Get
            '	Special processing to hide tabs
            If Me._Style = TabStyle.None Then
                Return New Rectangle(0, 0, Width, Height)
            Else
                Dim tabStripHeight As Integer = 0
                Dim itemHeight As Integer = 0

                If Me.Alignment <= TabAlignment.Bottom Then
                    itemHeight = Me.ItemSize.Height
                Else
                    itemHeight = Me.ItemSize.Width
                End If

                tabStripHeight = 5 + (itemHeight * Me.RowCount)

                Dim rect As New Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4)
                Select Case Me.Alignment
                    Case TabAlignment.Top
                        rect = New Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4)
                        Exit Select
                    Case TabAlignment.Bottom
                        rect = New Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4)
                        Exit Select
                    Case TabAlignment.Left
                        rect = New Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8)
                        Exit Select
                    Case TabAlignment.Right
                        rect = New Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8)
                        Exit Select
                End Select
                Return rect
            End If
        End Get
    End Property

    <Browsable(False)> _
    Public ReadOnly Property ActiveIndex() As Integer
        Get
            Dim hitTestInfo As New CustomTabNativeMethods.TCHITTESTINFO(Me.PointToClient(Control.MousePosition))
            Dim index As Integer = CustomTabNativeMethods.SendMessage(Me.Handle, CustomTabNativeMethods.TCM_HITTEST, IntPtr.Zero, CustomTabNativeMethods.ToIntPtr(hitTestInfo)).ToInt32()
            If index = -1 Then
                Return -1
            Else
                If Me.TabPages(index).Enabled Then
                    Return index
                Else
                    Return -1
                End If
            End If
        End Get
    End Property

    <Browsable(False)> _
    Public ReadOnly Property ActiveTab() As TabPage
        Get
            Dim activeIndex As Integer = Me.ActiveIndex
            If activeIndex > -1 Then
                Return Me.TabPages(activeIndex)
            Else
                Return Nothing
            End If
        End Get
    End Property

#End Region

#Region "Extension methods"

    Public Sub HideTab(ByVal page As TabPage)
        If page IsNot Nothing AndAlso Me.TabPages.Contains(page) Then
            Me.BackupTabPages()
            Me.TabPages.Remove(page)
        End If
    End Sub

    Public Sub HideTab(ByVal index As Integer)
        If Me.IsValidTabIndex(index) Then
            Me.HideTab(Me._TabPages(index))
        End If
    End Sub

    Public Sub HideTab(ByVal key As String)
        If Me.TabPages.ContainsKey(key) Then
            Me.HideTab(Me.TabPages(key))
        End If
    End Sub

    Public Sub ShowTab(ByVal page As TabPage)
        If page IsNot Nothing Then
            If Me._TabPages IsNot Nothing Then
                If Not Me.TabPages.Contains(page) AndAlso Me._TabPages.Contains(page) Then

                    '	Get insert point from backup of pages
                    Dim pageIndex As Integer = Me._TabPages.IndexOf(page)
                    If pageIndex > 0 Then
                        Dim start As Integer = pageIndex - 1

                        '	Check for presence of earlier pages in the visible tabs
                        For index As Integer = start To 0 Step -1
                            If Me.TabPages.Contains(Me._TabPages(index)) Then

                                '	Set insert point to the right of the last present tab
                                pageIndex = Me.TabPages.IndexOf(Me._TabPages(index)) + 1
                                Exit For
                            End If
                        Next
                    End If

                    '	Insert the page, or add to the end
                    If (pageIndex >= 0) AndAlso (pageIndex < Me.TabPages.Count) Then
                        Me.TabPages.Insert(pageIndex, page)
                    Else
                        Me.TabPages.Add(page)
                    End If
                End If
            Else

                '	If the page is not found at all then just add it
                If Not Me.TabPages.Contains(page) Then
                    Me.TabPages.Add(page)
                End If
            End If
        End If
    End Sub

    Public Sub ShowTab(ByVal index As Integer)
        If Me.IsValidTabIndex(index) Then
            Me.ShowTab(Me._TabPages(index))
        End If
    End Sub

    Public Sub ShowTab(ByVal key As String)
        If Me._TabPages IsNot Nothing Then
            Dim tab As TabPage = Me._TabPages.Find(Function(page As TabPage) page.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
            Me.ShowTab(tab)
        End If
    End Sub

    Private Function IsValidTabIndex(ByVal index As Integer) As Boolean
        Me.BackupTabPages()
        Return ((index >= 0) AndAlso (index < Me._TabPages.Count))
    End Function

    Private Sub BackupTabPages()
        If Me._TabPages Is Nothing Then
            Me._TabPages = New List(Of TabPage)()
            For Each page As TabPage In Me.TabPages
                Me._TabPages.Add(page)
            Next
        End If
    End Sub

#End Region

#Region "Drag 'n' Drop"

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If Me.AllowDrop Then
            Me._dragStartPosition = New Point(e.X, e.Y)
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If Me.AllowDrop Then
            Me._dragStartPosition = Point.Empty
        End If
    End Sub

    Protected Overrides Sub OnDragOver(ByVal drgevent As DragEventArgs)
        MyBase.OnDragOver(drgevent)

        If drgevent.Data.GetDataPresent(GetType(TabPage)) Then
            drgevent.Effect = DragDropEffects.Move
        Else
            drgevent.Effect = DragDropEffects.None
        End If
    End Sub

    Protected Overrides Sub OnDragDrop(ByVal drgevent As DragEventArgs)
        MyBase.OnDragDrop(drgevent)
        If drgevent.Data.GetDataPresent(GetType(TabPage)) Then
            drgevent.Effect = DragDropEffects.Move

            Dim dragTab As TabPage = DirectCast(drgevent.Data.GetData(GetType(TabPage)), TabPage)

            If Me.ActiveTab Is dragTab Then
                Return
            End If

            '	Capture insert point and adjust for removal of tab
            '	We cannot assess this after removal as differeing tab sizes will cause
            '	inaccuracies in the activeTab at insert point.
            Dim insertPoint As Integer = Me.ActiveIndex
            If dragTab.Parent.Equals(Me) AndAlso Me.TabPages.IndexOf(dragTab) < insertPoint Then
                insertPoint -= 1
            End If
            If insertPoint < 0 Then
                insertPoint = 0
            End If

            '	Remove from current position (could be another tabcontrol)
            DirectCast(dragTab.Parent, TabControl).TabPages.Remove(dragTab)

            '	Add to current position
            Me.TabPages.Insert(insertPoint, dragTab)

            '	deal with hidden tab handling?
            Me.SelectedTab = dragTab
        End If
    End Sub

    Private Sub StartDragDrop()
        If Not Me._dragStartPosition.IsEmpty Then
            Dim dragTab As TabPage = Me.SelectedTab
            If dragTab IsNot Nothing Then
                '	Test for movement greater than the drag activation trigger area
                Dim dragTestRect As New Rectangle(Me._dragStartPosition, Size.Empty)
                dragTestRect.Inflate(SystemInformation.DragSize)
                Dim pt As Point = Me.PointToClient(Control.MousePosition)
                If Not dragTestRect.Contains(pt) Then
                    Me.DoDragDrop(dragTab, DragDropEffects.All)
                    Me._dragStartPosition = Point.Empty
                End If
            End If
        End If
    End Sub

#End Region

#Region "Events"

    <Category("Action")> _
    Public Event HScroll As ScrollEventHandler
    <Category("Action")> _
    Public Event TabImageClick As EventHandler(Of TabControlEventArgs)
    <Category("Action")> _
    Public Event TabClosing As EventHandler(Of TabControlCancelEventArgs)

#End Region

#Region "Base class event processing"

    Protected Overrides Sub OnFontChanged(ByVal e As EventArgs)
        Dim hFont As IntPtr = Me.Font.ToHfont()
        CustomTabNativeMethods.SendMessage(Me.Handle, CustomTabNativeMethods.WM_SETFONT, hFont, CType(-1, IntPtr))
        CustomTabNativeMethods.SendMessage(Me.Handle, CustomTabNativeMethods.WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero)
        Me.UpdateStyles()
        If Me.Visible Then
            Me.Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        '	Recreate the buffer for manual double buffering
        If Me.Width > 0 AndAlso Me.Height > 0 Then
            If Me._BackImage IsNot Nothing Then
                Me._BackImage.Dispose()
                Me._BackImage = Nothing
            End If
            If Me._BackBufferGraphics IsNot Nothing Then
                Me._BackBufferGraphics.Dispose()
            End If
            If Me._BackBuffer IsNot Nothing Then
                Me._BackBuffer.Dispose()
            End If

            Me._BackBuffer = New Bitmap(Me.Width, Me.Height)
            Me._BackBufferGraphics = Graphics.FromImage(Me._BackBuffer)

            If Me._TabBufferGraphics IsNot Nothing Then
                Me._TabBufferGraphics.Dispose()
            End If
            If Me._TabBuffer IsNot Nothing Then
                Me._TabBuffer.Dispose()
            End If

            Me._TabBuffer = New Bitmap(Me.Width, Me.Height)
            Me._TabBufferGraphics = Graphics.FromImage(Me._TabBuffer)

            If Me._BackImage IsNot Nothing Then
                Me._BackImage.Dispose()
                Me._BackImage = Nothing

            End If
        End If
        MyBase.OnResize(e)
    End Sub

    Protected Overrides Sub OnParentBackColorChanged(ByVal e As EventArgs)
        If Me._BackImage IsNot Nothing Then
            Me._BackImage.Dispose()
            Me._BackImage = Nothing
        End If
        MyBase.OnParentBackColorChanged(e)
    End Sub

    Protected Overrides Sub OnParentBackgroundImageChanged(ByVal e As EventArgs)
        If Me._BackImage IsNot Nothing Then
            Me._BackImage.Dispose()
            Me._BackImage = Nothing
        End If
        MyBase.OnParentBackgroundImageChanged(e)
    End Sub

    Private Sub OnParentResize(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Visible Then
            Me.Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnParentChanged(ByVal e As EventArgs)
        MyBase.OnParentChanged(e)
        If Me.Parent IsNot Nothing Then
            AddHandler Me.Parent.Resize, AddressOf Me.OnParentResize
        End If
    End Sub

    Protected Overrides Sub OnSelecting(ByVal e As TabControlCancelEventArgs)
        MyBase.OnSelecting(e)

        '	Do not allow selecting of disabled tabs
        If e.Action = TabControlAction.Selecting AndAlso e.TabPage IsNot Nothing AndAlso Not e.TabPage.Enabled Then
            e.Cancel = True
        End If
    End Sub

    Protected Overrides Sub OnMove(ByVal e As EventArgs)
        If Me.Width > 0 AndAlso Me.Height > 0 Then
            If Me._BackImage IsNot Nothing Then
                Me._BackImage.Dispose()
                Me._BackImage = Nothing
            End If
        End If
        MyBase.OnMove(e)
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnControlAdded(ByVal e As ControlEventArgs)
        MyBase.OnControlAdded(e)
        If Me.Visible Then
            Me.Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnControlRemoved(ByVal e As ControlEventArgs)
        MyBase.OnControlRemoved(e)
        If Me.Visible Then
            Me.Invalidate()
        End If
    End Sub

    <UIPermission(SecurityAction.LinkDemand, Window:=UIPermissionWindow.AllWindows)> _
    Protected Overrides Function ProcessMnemonic(ByVal charCode As Char) As Boolean
        For Each page As TabPage In Me.TabPages
            If IsMnemonic(charCode, page.Text) Then
                Me.SelectedTab = page
                Return True
            End If
        Next
        Return MyBase.ProcessMnemonic(charCode)
    End Function

    Protected Overrides Sub OnSelectedIndexChanged(ByVal e As EventArgs)
        MyBase.OnSelectedIndexChanged(e)
    End Sub

    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    <DebuggerStepThrough()> _
    Protected Overrides Sub WndProc(ByRef m As Message)

        Select Case m.Msg
            Case CustomTabNativeMethods.WM_HSCROLL

                '	Raise the scroll event when the scroller is scrolled
                MyBase.WndProc(m)
                Me.OnHScroll(New ScrollEventArgs(CType(CustomTabNativeMethods.LoWord(m.WParam), ScrollEventType), _oldValue, CustomTabNativeMethods.HiWord(m.WParam), ScrollOrientation.HorizontalScroll))
                Exit Select
            Case Else
                '				case NativeMethods.WM_PAINT:
                '					
                '					//	Handle painting ourselves rather than call the base OnPaint.
                '					CustomPaint(ref m);
                '					break;

                MyBase.WndProc(m)
                Exit Select

        End Select
    End Sub

    Protected Overrides Sub OnMouseClick(ByVal e As MouseEventArgs)
        Dim index As Integer = Me.ActiveIndex

        '	If we are clicking on an image then raise the ImageClicked event before raising the standard mouse click event
        '	if there if a handler.
        If index > -1 AndAlso Me.TabImageClickEvent IsNot Nothing AndAlso (Me.TabPages(index).ImageIndex > -1 OrElse Not String.IsNullOrEmpty(Me.TabPages(index).ImageKey)) AndAlso Me.GetTabImageRect(index).Contains(Me.MousePosition) Then
            Me.OnTabImageClick(New TabControlEventArgs(Me.TabPages(index), index, TabControlAction.Selected))

            '	Fire the base event

            MyBase.OnMouseClick(e)
        ElseIf Not Me.DesignMode AndAlso index > -1 AndAlso Me._StyleProvider.ShowTabCloser AndAlso Me.GetTabCloserRect(index).Contains(Me.MousePosition) Then

            '	If we are clicking on a closer then remove the tab instead of raising the standard mouse click event
            '	But raise the tab closing event first
            Dim tab As TabPage = Me.ActiveTab
            Dim args As New TabControlCancelEventArgs(tab, index, False, TabControlAction.Deselecting)
            Me.OnTabClosing(args)

            If Not args.Cancel AndAlso tab IsNot Nothing Then
                Me.TabPages.Remove(tab)
                'tab.Dispose()
            End If
        Else
            '	Fire the base event
            MyBase.OnMouseClick(e)
        End If
    End Sub

    Protected Overridable Sub OnTabImageClick(ByVal e As TabControlEventArgs)
        RaiseEvent TabImageClick(Me, e)
    End Sub

    Protected Overridable Sub OnTabClosing(ByVal e As TabControlCancelEventArgs)
        RaiseEvent TabClosing(Me, e)
    End Sub

    Protected Overridable Sub OnHScroll(ByVal e As ScrollEventArgs)
        '	repaint the moved tabs
        Me.Invalidate()

        '	Raise the event
        RaiseEvent HScroll(Me, e)

        If e.Type = ScrollEventType.EndScroll Then
            Me._oldValue = e.NewValue
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If Me._StyleProvider.ShowTabCloser Then
            Dim tabRect As Rectangle = Me._StyleProvider.GetTabRect(Me.ActiveIndex)
            If tabRect.Contains(Me.MousePosition) Then
                Me.Invalidate()
            End If
        End If

        '	Initialise Drag Drop
        If Me.AllowDrop AndAlso e.Button = MouseButtons.Left Then
            Me.StartDragDrop()
        End If
    End Sub

#End Region

#Region "Basic drawing methods"

    '		private void CustomPaint(ref Message m){
    '			NativeMethods.PAINTSTRUCT paintStruct = new NativeMethods.PAINTSTRUCT();
    '			NativeMethods.BeginPaint(m.HWnd, ref paintStruct);
    '			using (Graphics screenGraphics = this.CreateGraphics()) {
    '				this.CustomPaint(screenGraphics);
    '			}
    '			NativeMethods.EndPaint(m.HWnd, ref paintStruct);
    '		}

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        '	We must always paint the entire area of the tab control
        'If e.ClipRectangle.Equals(Me.ClientRectangle) Then
        Me.CustomPaint(e.Graphics)
        'Else
        ''	it is less intensive to just reinvoke the paint with the whole surface available to draw on.
        'Me.Invalidate()
        'End If
    End Sub

    Private Sub CustomPaint(ByVal screenGraphics As Graphics)
        '	We render into a bitmap that is then drawn in one shot rather than using
        '	double buffering built into the control as the built in buffering
        ' 	messes up the background painting.
        '	Equally the .Net 2.0 BufferedGraphics object causes the background painting
        '	to mess up, which is why we use this .Net 1.1 buffering technique.

        '	Buffer code from Gil. Schmidt http://www.codeproject.com/KB/graphics/DoubleBuffering.aspx

        If Me.Width > 0 AndAlso Me.Height > 0 Then
            If Me._BackImage Is Nothing Then
                '	Cached Background Image
                Me._BackImage = New Bitmap(Me.Width, Me.Height)
                Dim backGraphics As Graphics = Graphics.FromImage(Me._BackImage)
                backGraphics.Clear(Color.Transparent)
                Me.PaintTransparentBackground(backGraphics, Me.ClientRectangle)
            End If

            Me._BackBufferGraphics.Clear(Color.Transparent)
            Me._BackBufferGraphics.DrawImageUnscaled(Me._BackImage, 0, 0)

            Me._TabBufferGraphics.Clear(Color.Transparent)

            If Me.TabCount > 0 Then

                '	When top or bottom and scrollable we need to clip the sides from painting the tabs.
                '	Left and right are always multiline.
                If Me.Alignment <= TabAlignment.Bottom AndAlso Not Me.Multiline Then
                    Me._TabBufferGraphics.Clip = New Region(New RectangleF(Me.ClientRectangle.X + 3, Me.ClientRectangle.Y, Me.ClientRectangle.Width - 6, Me.ClientRectangle.Height))
                End If

                '	Draw each tabpage from right to left.  We do it this way to handle
                '	the overlap correctly.
                If Me.Multiline Then
                    For row As Integer = 0 To Me.RowCount - 1
                        For index As Integer = Me.TabCount - 1 To 0 Step -1
                            If index <> Me.SelectedIndex AndAlso (Me.RowCount = 1 OrElse Me.GetTabRow(index) = row) Then
                                Me.DrawTabPage(index, Me._TabBufferGraphics)
                            End If
                        Next
                    Next
                Else
                    For index As Integer = Me.TabCount - 1 To 0 Step -1
                        If index <> Me.SelectedIndex Then
                            Me.DrawTabPage(index, Me._TabBufferGraphics)
                        End If
                    Next
                End If

                '	The selected tab must be drawn last so it appears on top.
                If Me.SelectedIndex > -1 Then
                    Me.DrawTabPage(Me.SelectedIndex, Me._TabBufferGraphics)
                End If
            End If
            Me._TabBufferGraphics.Flush()

            '	Paint the tabs on top of the background

            ' Create a new color matrix and set the alpha value to 0.5
            Dim alphaMatrix As New Imaging.ColorMatrix()
            alphaMatrix.Matrix00 = InlineAssignHelper(alphaMatrix.Matrix11, InlineAssignHelper(alphaMatrix.Matrix22, InlineAssignHelper(alphaMatrix.Matrix44, 1)))
            alphaMatrix.Matrix33 = Me._StyleProvider.Opacity

            ' Create a new image attribute object and set the color matrix to
            ' the one just created
            Using alphaAttributes As New Imaging.ImageAttributes()

                alphaAttributes.SetColorMatrix(alphaMatrix)

                ' Draw the original image with the image attributes specified
                Me._BackBufferGraphics.DrawImage(Me._TabBuffer, New Rectangle(0, 0, Me._TabBuffer.Width, Me._TabBuffer.Height), 0, 0, Me._TabBuffer.Width, Me._TabBuffer.Height, _
                 GraphicsUnit.Pixel, alphaAttributes)
            End Using

            Me._BackBufferGraphics.Flush()

            '	Now paint this to the screen


            '	We want to paint the whole tabstrip and border every time
            '	so that the hot areas update correctly, along with any overlaps

            '	paint the tabs etc.
            If Me.RightToLeftLayout Then
                screenGraphics.DrawImageUnscaled(Me._BackBuffer, -1, 0)
            Else
                screenGraphics.DrawImageUnscaled(Me._BackBuffer, 0, 0)
            End If
        End If
    End Sub

    Protected Sub PaintTransparentBackground(ByVal graphics As Graphics, ByVal clipRect As Rectangle)

        If (Me.Parent IsNot Nothing) Then

            '	Set the cliprect to be relative to the parent
            clipRect.Offset(Me.Location)

            '	Save the current state before we do anything.
            Dim state As Drawing2D.GraphicsState = graphics.Save()

            '	Set the graphicsobject to be relative to the parent
            graphics.TranslateTransform(CSng(-Me.Location.X), CSng(-Me.Location.Y))
            graphics.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed

            '	Paint the parent
            Dim e As New PaintEventArgs(graphics, clipRect)
            Try
                Me.InvokePaintBackground(Me.Parent, e)
                Me.InvokePaint(Me.Parent, e)
            Finally
                '	Restore the graphics state and the clipRect to their original locations
                graphics.Restore(state)
                clipRect.Offset(-Me.Location.X, -Me.Location.Y)
            End Try
        End If
    End Sub

    Private Sub DrawTabPage(ByVal index As Integer, ByVal graphics As Graphics)
        graphics.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed

        '	Get TabPageBorder
        Using tabPageBorderPath As Drawing2D.GraphicsPath = Me.GetTabPageBorder(index)

            '	Paint the background
            Using fillBrush As Brush = Me._StyleProvider.GetPageBackgroundBrush(index)
                graphics.FillPath(fillBrush, tabPageBorderPath)
            End Using

            If Me._Style <> TabStyle.None Then

                '	Paint the tab
                Me._StyleProvider.PaintTab(index, graphics)

                '	Draw any image
                Me.DrawTabImage(index, graphics)

                '	Draw the text

                Me.DrawTabText(index, graphics)
            End If

            '	Paint the border

            Me.DrawTabBorder(tabPageBorderPath, index, graphics)
        End Using
    End Sub

    Private Sub DrawTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal index As Integer, ByVal graphics As Graphics)
        graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        Dim borderColor As Color
        If index = Me.SelectedIndex Then
            borderColor = Me._StyleProvider.BorderColorSelected
        ElseIf Me._StyleProvider.HotTrack AndAlso index = Me.ActiveIndex Then
            borderColor = Me._StyleProvider.BorderColorHot
        Else
            borderColor = Me._StyleProvider.BorderColor
        End If

        Using borderPen As New Pen(borderColor)
            graphics.DrawPath(borderPen, path)
        End Using
    End Sub

    Private Sub DrawTabText(ByVal index As Integer, ByVal graphics As Graphics)
        graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
        Dim tabBounds As Rectangle = Me.GetTabTextRect(index)

        If Me.SelectedIndex = index Then
            Using textBrush As Brush = New SolidBrush(Me._StyleProvider.TextColorSelected)
                graphics.DrawString(Me.TabPages(index).Text, Me.Font, textBrush, tabBounds, Me.GetStringFormat())
            End Using
        Else
            If Me.TabPages(index).Enabled Then
                Using textBrush As Brush = New SolidBrush(Me._StyleProvider.TextColor)
                    graphics.DrawString(Me.TabPages(index).Text, Me.Font, textBrush, tabBounds, Me.GetStringFormat())
                End Using
            Else
                Using textBrush As Brush = New SolidBrush(Me._StyleProvider.TextColorDisabled)
                    graphics.DrawString(Me.TabPages(index).Text, Me.Font, textBrush, tabBounds, Me.GetStringFormat())
                End Using
            End If
        End If
    End Sub

    Private Sub DrawTabImage(ByVal index As Integer, ByVal graphics As Graphics)
        Dim tabImage As Image = Nothing
        If Me.TabPages(index).ImageIndex > -1 AndAlso Me.ImageList IsNot Nothing AndAlso Me.ImageList.Images.Count > Me.TabPages(index).ImageIndex Then
            tabImage = Me.ImageList.Images(Me.TabPages(index).ImageIndex)
        ElseIf (Not String.IsNullOrEmpty(Me.TabPages(index).ImageKey) AndAlso Not Me.TabPages(index).ImageKey.Equals("(none)", StringComparison.OrdinalIgnoreCase)) AndAlso Me.ImageList IsNot Nothing AndAlso Me.ImageList.Images.ContainsKey(Me.TabPages(index).ImageKey) Then
            tabImage = Me.ImageList.Images(Me.TabPages(index).ImageKey)
        End If

        If tabImage IsNot Nothing Then
            If Me.RightToLeftLayout Then
                tabImage.RotateFlip(RotateFlipType.RotateNoneFlipX)
            End If
            Dim imageRect As Rectangle = Me.GetTabImageRect(index)
            If Me.TabPages(index).Enabled Then
                graphics.DrawImage(tabImage, imageRect)
            Else
                ControlPaint.DrawImageDisabled(graphics, tabImage, imageRect.X, imageRect.Y, Color.Transparent)
            End If
        End If
    End Sub

#End Region

#Region "String formatting"

    Private Function GetStringFormat() As StringFormat
        Dim format As StringFormat = Nothing

        '	Rotate Text by 90 degrees for left and right tabs
        Select Case Me.Alignment
            Case TabAlignment.Top, TabAlignment.Bottom
                format = New StringFormat()
                Exit Select
            Case TabAlignment.Left, TabAlignment.Right
                format = New StringFormat(StringFormatFlags.DirectionVertical)
                Exit Select
        End Select
        format.Alignment = StringAlignment.Center
        format.LineAlignment = StringAlignment.Center
        If Me.FindForm() IsNot Nothing AndAlso Me.FindForm().KeyPreview Then
            format.HotkeyPrefix = Drawing.Text.HotkeyPrefix.Show
        Else
            format.HotkeyPrefix = Drawing.Text.HotkeyPrefix.Hide
        End If
        If Me.RightToLeft = RightToLeft.Yes Then
            format.FormatFlags = format.FormatFlags Or StringFormatFlags.DirectionRightToLeft
        End If
        Return format
    End Function

#End Region

#Region "Tab borders and bounds properties"

    Private Function GetTabPageBorder(ByVal index As Integer) As Drawing2D.GraphicsPath

        Dim path As New Drawing2D.GraphicsPath()
        Dim pageBounds As Rectangle = Me.GetPageBounds(index)
        Dim tabBounds As Rectangle = Me._StyleProvider.GetTabRect(index)
        Me._StyleProvider.AddTabBorder(path, tabBounds)
        Me.AddPageBorder(path, pageBounds, tabBounds)

        path.CloseFigure()
        Return path
    End Function

    Public Function GetPageBounds(ByVal index As Integer) As Rectangle
        Dim pageBounds As Rectangle = Me.TabPages(index).Bounds
        pageBounds.Width += 1
        pageBounds.Height += 1
        pageBounds.X -= 1
        pageBounds.Y -= 1

        If pageBounds.Bottom > Me.Height - 4 Then
            pageBounds.Height -= (pageBounds.Bottom - Me.Height + 4)
        End If
        Return pageBounds
    End Function

    Private Function GetTabTextRect(ByVal index As Integer) As Rectangle
        Dim textRect As New Rectangle()
        Using path As Drawing2D.GraphicsPath = Me._StyleProvider.GetTabBorder(index)
            Dim tabBounds As RectangleF = path.GetBounds()

            textRect = New Rectangle(CInt(Math.Truncate(tabBounds.X)), CInt(Math.Truncate(tabBounds.Y)), CInt(Math.Truncate(tabBounds.Width)), CInt(Math.Truncate(tabBounds.Height)))

            '	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
            Select Case Me.Alignment
                Case TabAlignment.Top
                    textRect.Y += 4
                    textRect.Height -= 6
                    Exit Select
                Case TabAlignment.Bottom
                    textRect.Y += 2
                    textRect.Height -= 6
                    Exit Select
                Case TabAlignment.Left
                    textRect.X += 4
                    textRect.Width -= 6
                    Exit Select
                Case TabAlignment.Right
                    textRect.X += 2
                    textRect.Width -= 6
                    Exit Select
            End Select

            '	If there is an image allow for it
            If Me.ImageList IsNot Nothing AndAlso (Me.TabPages(index).ImageIndex > -1 OrElse (Not String.IsNullOrEmpty(Me.TabPages(index).ImageKey) AndAlso Not Me.TabPages(index).ImageKey.Equals("(none)", StringComparison.OrdinalIgnoreCase))) Then
                Dim imageRect As Rectangle = Me.GetTabImageRect(index)
                If (Me._StyleProvider.ImageAlign And CustomTabNativeMethods.AnyLeftAlign) <> CType(0, ContentAlignment) Then
                    If Me.Alignment <= TabAlignment.Bottom Then
                        textRect.X = imageRect.Right + 4
                        textRect.Width -= (textRect.Right - CInt(Math.Truncate(tabBounds.Right)))
                    Else
                        textRect.Y = imageRect.Y + 4
                        textRect.Height -= (textRect.Bottom - CInt(Math.Truncate(tabBounds.Bottom)))
                    End If
                    '	If there is a closer allow for it
                    If Me._StyleProvider.ShowTabCloser Then
                        Dim closerRect As Rectangle = Me.GetTabCloserRect(index)
                        If Me.Alignment <= TabAlignment.Bottom Then
                            If Me.RightToLeftLayout Then
                                textRect.Width -= (closerRect.Right + 4 - textRect.X)
                                textRect.X = closerRect.Right + 4
                            Else
                                textRect.Width -= (CInt(Math.Truncate(tabBounds.Right)) - closerRect.X + 4)
                            End If
                        Else
                            If Me.RightToLeftLayout Then
                                textRect.Height -= (closerRect.Bottom + 4 - textRect.Y)
                                textRect.Y = closerRect.Bottom + 4
                            Else
                                textRect.Height -= (CInt(Math.Truncate(tabBounds.Bottom)) - closerRect.Y + 4)
                            End If
                        End If
                    End If
                ElseIf (Me._StyleProvider.ImageAlign And CustomTabNativeMethods.AnyCenterAlign) <> CType(0, ContentAlignment) Then
                    '	If there is a closer allow for it
                    If Me._StyleProvider.ShowTabCloser Then
                        Dim closerRect As Rectangle = Me.GetTabCloserRect(index)
                        If Me.Alignment <= TabAlignment.Bottom Then
                            If Me.RightToLeftLayout Then
                                textRect.Width -= (closerRect.Right + 4 - textRect.X)
                                textRect.X = closerRect.Right + 4
                            Else
                                textRect.Width -= (CInt(Math.Truncate(tabBounds.Right)) - closerRect.X + 4)
                            End If
                        Else
                            If Me.RightToLeftLayout Then
                                textRect.Height -= (closerRect.Bottom + 4 - textRect.Y)
                                textRect.Y = closerRect.Bottom + 4
                            Else
                                textRect.Height -= (CInt(Math.Truncate(tabBounds.Bottom)) - closerRect.Y + 4)
                            End If
                        End If
                    End If
                Else
                    If Me.Alignment <= TabAlignment.Bottom Then
                        textRect.Width -= (CInt(Math.Truncate(tabBounds.Right)) - imageRect.X + 4)
                    Else
                        textRect.Height -= (CInt(Math.Truncate(tabBounds.Bottom)) - imageRect.Y + 4)
                    End If
                    '	If there is a closer allow for it
                    If Me._StyleProvider.ShowTabCloser Then
                        Dim closerRect As Rectangle = Me.GetTabCloserRect(index)
                        If Me.Alignment <= TabAlignment.Bottom Then
                            If Me.RightToLeftLayout Then
                                textRect.Width -= (closerRect.Right + 4 - textRect.X)
                                textRect.X = closerRect.Right + 4
                            Else
                                textRect.Width -= (CInt(Math.Truncate(tabBounds.Right)) - closerRect.X + 4)
                            End If
                        Else
                            If Me.RightToLeftLayout Then
                                textRect.Height -= (closerRect.Bottom + 4 - textRect.Y)
                                textRect.Y = closerRect.Bottom + 4
                            Else
                                textRect.Height -= (CInt(Math.Truncate(tabBounds.Bottom)) - closerRect.Y + 4)
                            End If
                        End If
                    End If
                End If
            Else
                '	If there is a closer allow for it
                If Me._StyleProvider.ShowTabCloser Then
                    Dim closerRect As Rectangle = Me.GetTabCloserRect(index)
                    If Me.Alignment <= TabAlignment.Bottom Then
                        If Me.RightToLeftLayout Then
                            textRect.Width -= (closerRect.Right + 4 - textRect.X)
                            textRect.X = closerRect.Right + 4
                        Else
                            textRect.Width -= (CInt(Math.Truncate(tabBounds.Right)) - closerRect.X + 4)
                        End If
                    Else
                        If Me.RightToLeftLayout Then
                            textRect.Height -= (closerRect.Bottom + 4 - textRect.Y)
                            textRect.Y = closerRect.Bottom + 4
                        Else
                            textRect.Height -= (CInt(Math.Truncate(tabBounds.Bottom)) - closerRect.Y + 4)
                        End If
                    End If
                End If
            End If


            '	Ensure it fits inside the path at the centre line
            If Me.Alignment <= TabAlignment.Bottom Then
                While Not path.IsVisible(textRect.Right, textRect.Y) AndAlso textRect.Width > 0
                    textRect.Width -= 1
                End While
                While Not path.IsVisible(textRect.X, textRect.Y) AndAlso textRect.Width > 0
                    textRect.X += 1
                    textRect.Width -= 1
                End While
            Else
                While Not path.IsVisible(textRect.X, textRect.Bottom) AndAlso textRect.Height > 0
                    textRect.Height -= 1
                End While
                While Not path.IsVisible(textRect.X, textRect.Y) AndAlso textRect.Height > 0
                    textRect.Y += 1
                    textRect.Height -= 1
                End While
            End If
        End Using
        Return textRect
    End Function

    Public Function GetTabRow(ByVal index As Integer) As Integer
        '	All calculations will use this rect as the base point
        '	because the itemsize does not return the correct width.
        Dim rect As Rectangle = Me.GetTabRect(index)

        Dim row As Integer = -1

        Select Case Me.Alignment
            Case TabAlignment.Top
                row = (rect.Y - 2) \ rect.Height
                Exit Select
            Case TabAlignment.Bottom
                row = ((Me.Height - rect.Y - 2) \ rect.Height) - 1
                Exit Select
            Case TabAlignment.Left
                row = (rect.X - 2) \ rect.Width
                Exit Select
            Case TabAlignment.Right
                row = ((Me.Width - rect.X - 2) \ rect.Width) - 1
                Exit Select
        End Select
        Return row
    End Function

    Public Function GetTabPosition(ByVal index As Integer) As Point

        '	If we are not multiline then the column is the index and the row is 0.
        If Not Me.Multiline Then
            Return New Point(0, index)
        End If

        '	If there is only one row then the column is the index
        If Me.RowCount = 1 Then
            Return New Point(0, index)
        End If

        '	We are in a true multi-row scenario
        Dim row As Integer = Me.GetTabRow(index)
        Dim rect As Rectangle = Me.GetTabRect(index)
        Dim column As Integer = -1

        '	Scan from left to right along rows, skipping to next row if it is not the one we want.
        For testIndex As Integer = 0 To Me.TabCount - 1
            Dim testRect As Rectangle = Me.GetTabRect(testIndex)
            If Me.Alignment <= TabAlignment.Bottom Then
                If testRect.Y = rect.Y Then
                    column += 1
                End If
            Else
                If testRect.X = rect.X Then
                    column += 1
                End If
            End If

            If testRect.Location.Equals(rect.Location) Then
                Return New Point(row, column)
            End If
        Next

        Return New Point(0, 0)
    End Function

    Public Function IsFirstTabInRow(ByVal index As Integer) As Boolean
        If index < 0 Then
            Return False
        End If
        Dim firstTabinRow As Boolean = (index = 0)
        If Not firstTabinRow Then
            If Me.Alignment <= TabAlignment.Bottom Then
                If Me.GetTabRect(index).X = 2 Then
                    firstTabinRow = True
                End If
            Else
                If Me.GetTabRect(index).Y = 2 Then
                    firstTabinRow = True
                End If
            End If
        End If
        Return firstTabinRow
    End Function

    Private Sub AddPageBorder(ByVal path As Drawing2D.GraphicsPath, ByVal pageBounds As Rectangle, ByVal tabBounds As Rectangle)
        Select Case Me.Alignment
            Case TabAlignment.Top
                path.AddLine(tabBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Y)
                path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom)
                path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom)
                path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y)
                path.AddLine(pageBounds.X, pageBounds.Y, tabBounds.X, pageBounds.Y)
                Exit Select
            Case TabAlignment.Bottom
                path.AddLine(tabBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom)
                path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y)
                path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y)
                path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom)
                path.AddLine(pageBounds.Right, pageBounds.Bottom, tabBounds.Right, pageBounds.Bottom)
                Exit Select
            Case TabAlignment.Left
                path.AddLine(pageBounds.X, tabBounds.Y, pageBounds.X, pageBounds.Y)
                path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y)
                path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom)
                path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom)
                path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, tabBounds.Bottom)
                Exit Select
            Case TabAlignment.Right
                path.AddLine(pageBounds.Right, tabBounds.Bottom, pageBounds.Right, pageBounds.Bottom)
                path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom)
                path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y)
                path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y)
                path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, tabBounds.Y)
                Exit Select
        End Select
    End Sub

    Private Function GetTabImageRect(ByVal index As Integer) As Rectangle
        Using tabBorderPath As Drawing2D.GraphicsPath = Me._StyleProvider.GetTabBorder(index)
            Return Me.GetTabImageRect(tabBorderPath)
        End Using
    End Function
    Private Function GetTabImageRect(ByVal tabBorderPath As Drawing2D.GraphicsPath) As Rectangle
        Dim imageRect As New Rectangle()
        Dim rect As RectangleF = tabBorderPath.GetBounds()

        '	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
        Select Case Me.Alignment
            Case TabAlignment.Top
                rect.Y += 4
                rect.Height -= 6
                Exit Select
            Case TabAlignment.Bottom
                rect.Y += 2
                rect.Height -= 6
                Exit Select
            Case TabAlignment.Left
                rect.X += 4
                rect.Width -= 6
                Exit Select
            Case TabAlignment.Right
                rect.X += 2
                rect.Width -= 6
                Exit Select
        End Select

        '	Ensure image is fully visible
        If Me.Alignment <= TabAlignment.Bottom Then
            If (Me._StyleProvider.ImageAlign And CustomTabNativeMethods.AnyLeftAlign) <> CType(0, ContentAlignment) Then
                imageRect = New Rectangle(CInt(Math.Truncate(rect.X)), CInt(Math.Truncate(rect.Y)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Height)) - 16) / 2))), 16, 16)
                While Not tabBorderPath.IsVisible(imageRect.X, imageRect.Y)
                    imageRect.X += 1
                End While

                imageRect.X += 4
            ElseIf (Me._StyleProvider.ImageAlign And CustomTabNativeMethods.AnyCenterAlign) <> CType(0, ContentAlignment) Then
                imageRect = New Rectangle(CInt(Math.Truncate(rect.X)) + CInt(Math.Truncate(Math.Floor(CDbl((CInt(Math.Truncate(rect.Right)) - CInt(Math.Truncate(rect.X)) - CInt(Math.Truncate(rect.Height)) + 2) \ 2)))), CInt(Math.Truncate(rect.Y)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Height)) - 16) / 2))), 16, 16)
            Else
                imageRect = New Rectangle(CInt(Math.Truncate(rect.Right)), CInt(Math.Truncate(rect.Y)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Height)) - 16) / 2))), 16, 16)
                While Not tabBorderPath.IsVisible(imageRect.Right, imageRect.Y)
                    imageRect.X -= 1
                End While
                imageRect.X -= 4

                '	Move it in further to allow for the tab closer
                If Me._StyleProvider.ShowTabCloser AndAlso Not Me.RightToLeftLayout Then
                    imageRect.X -= 10
                End If
            End If
        Else
            If (Me._StyleProvider.ImageAlign And CustomTabNativeMethods.AnyLeftAlign) <> CType(0, ContentAlignment) Then
                imageRect = New Rectangle(CInt(Math.Truncate(rect.X)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Width)) - 16) / 2))), CInt(Math.Truncate(rect.Y)), 16, 16)
                While Not tabBorderPath.IsVisible(imageRect.X, imageRect.Y)
                    imageRect.Y += 1
                End While
                imageRect.Y += 4
            ElseIf (Me._StyleProvider.ImageAlign And CustomTabNativeMethods.AnyCenterAlign) <> CType(0, ContentAlignment) Then
                imageRect = New Rectangle(CInt(Math.Truncate(rect.X)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Width)) - 16) / 2))), CInt(Math.Truncate(rect.Y)) + CInt(Math.Truncate(Math.Floor(CDbl((CInt(Math.Truncate(rect.Bottom)) - CInt(Math.Truncate(rect.Y)) - CInt(Math.Truncate(rect.Width)) + 2) \ 2)))), 16, 16)
            Else
                imageRect = New Rectangle(CInt(Math.Truncate(rect.X)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Width)) - 16) / 2))), CInt(Math.Truncate(rect.Bottom)), 16, 16)
                While Not tabBorderPath.IsVisible(imageRect.X, imageRect.Bottom)
                    imageRect.Y -= 1
                End While
                imageRect.Y -= 4

                '	Move it in further to allow for the tab closer
                If Me._StyleProvider.ShowTabCloser AndAlso Not Me.RightToLeftLayout Then
                    imageRect.Y -= 10
                End If
            End If
        End If
        Return imageRect
    End Function

    Public Function GetTabCloserRect(ByVal index As Integer) As Rectangle
        Dim closerRect As New Rectangle()
        Using path As Drawing2D.GraphicsPath = Me._StyleProvider.GetTabBorder(index)
            Dim rect As RectangleF = path.GetBounds()

            '	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
            Select Case Me.Alignment
                Case TabAlignment.Top
                    rect.Y += 4
                    rect.Height -= 6
                    Exit Select
                Case TabAlignment.Bottom
                    rect.Y += 2
                    rect.Height -= 6
                    Exit Select
                Case TabAlignment.Left
                    rect.X += 4
                    rect.Width -= 6
                    Exit Select
                Case TabAlignment.Right
                    rect.X += 2
                    rect.Width -= 6
                    Exit Select
            End Select
            If Me.Alignment <= TabAlignment.Bottom Then
                If Me.RightToLeftLayout Then
                    closerRect = New Rectangle(CInt(Math.Truncate(rect.Left)), CInt(Math.Truncate(rect.Y)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Height)) - 6) / 2))), 6, 6)
                    While Not path.IsVisible(closerRect.Left, closerRect.Y) AndAlso closerRect.Right < Me.Width
                        closerRect.X += 1
                    End While
                    closerRect.X += 4
                Else
                    closerRect = New Rectangle(CInt(Math.Truncate(rect.Right)), CInt(Math.Truncate(rect.Y)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Height)) - 6) / 2))), 6, 6)
                    While Not path.IsVisible(closerRect.Right, closerRect.Y) AndAlso closerRect.Right > -6
                        closerRect.X -= 1
                    End While
                    closerRect.X -= 4
                End If
            Else
                If Me.RightToLeftLayout Then
                    closerRect = New Rectangle(CInt(Math.Truncate(rect.X)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Width)) - 6) / 2))), CInt(Math.Truncate(rect.Top)), 6, 6)
                    While Not path.IsVisible(closerRect.X, closerRect.Top) AndAlso closerRect.Bottom < Me.Height
                        closerRect.Y += 1
                    End While
                    closerRect.Y += 4
                Else
                    closerRect = New Rectangle(CInt(Math.Truncate(rect.X)) + CInt(Math.Truncate(Math.Floor(CDbl(CInt(Math.Truncate(rect.Width)) - 6) / 2))), CInt(Math.Truncate(rect.Bottom)), 6, 6)
                    While Not path.IsVisible(closerRect.X, closerRect.Bottom) AndAlso closerRect.Top > -6
                        closerRect.Y -= 1
                    End While
                    closerRect.Y -= 4
                End If
            End If
        End Using
        Return closerRect
    End Function

    Public Shadows ReadOnly Property MousePosition() As Point
        Get
            Dim loc As Point = Me.PointToClient(Control.MousePosition)
            If Me.RightToLeftLayout Then
                loc.X = (Me.Width - loc.X)
            End If
            Return loc
        End Get
    End Property

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

#End Region

End Class

'[SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]

'Friend NotInheritable Class NativeMethods
Public Class CustomTabNativeMethods
    Private Sub New()
    End Sub

#Region "Windows Constants"

    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WM_GETTABRECT As Integer = &H130A
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WS_EX_TRANSPARENT As Integer = &H20
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WM_SETFONT As Integer = &H30
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WM_FONTCHANGE As Integer = &H1D
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WM_HSCROLL As Integer = &H114
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const TCM_HITTEST As Integer = &H130D
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WM_PAINT As Integer = &HF
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WS_EX_LAYOUTRTL As Integer = &H400000
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Const WS_EX_NOINHERITLAYOUT As Integer = &H100000

#End Region

#Region "Content Alignment"

    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Shared ReadOnly AnyRightAlign As ContentAlignment = ContentAlignment.BottomRight Or ContentAlignment.MiddleRight Or ContentAlignment.TopRight
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Shared ReadOnly AnyLeftAlign As ContentAlignment = ContentAlignment.BottomLeft Or ContentAlignment.MiddleLeft Or ContentAlignment.TopLeft
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Shared ReadOnly AnyTopAlign As ContentAlignment = ContentAlignment.TopRight Or ContentAlignment.TopCenter Or ContentAlignment.TopLeft
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Shared ReadOnly AnyBottomAlign As ContentAlignment = ContentAlignment.BottomRight Or ContentAlignment.BottomCenter Or ContentAlignment.BottomLeft
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Shared ReadOnly AnyMiddleAlign As ContentAlignment = ContentAlignment.MiddleRight Or ContentAlignment.MiddleCenter Or ContentAlignment.MiddleLeft
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
    Public Shared ReadOnly AnyCenterAlign As ContentAlignment = ContentAlignment.BottomCenter Or ContentAlignment.MiddleCenter Or ContentAlignment.TopCenter

#End Region

#Region "User32.dll"

    '        [DllImport("user32.dll"), SecurityPermission(SecurityAction.Demand)]
    '		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        '	This Method replaces the User32 method SendMessage, but will only work for sending
        '	messages to Managed controls.
        Dim control__1 As Control = Control.FromHandle(hWnd)
        If control__1 Is Nothing Then
            Return IntPtr.Zero
        End If

        Dim message As New Message()
        message.HWnd = hWnd
        message.LParam = lParam
        message.WParam = wParam
        message.Msg = msg

        Dim wproc As MethodInfo = control__1.[GetType]().GetMethod("WndProc", BindingFlags.NonPublic Or BindingFlags.InvokeMethod Or BindingFlags.FlattenHierarchy Or BindingFlags.IgnoreCase Or BindingFlags.Instance)

        Dim args As Object() = New Object() {message}
        wproc.Invoke(control__1, args)

        Return CType(args(0), Message).Result
    End Function


    '		[DllImport("user32.dll")]
    '		public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT paintStruct);
    '		
    '		[DllImport("user32.dll")]
    '		[return: MarshalAs(UnmanagedType.Bool)]
    '		public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT paintStruct);
    '
#End Region

#Region "Misc Functions"

    Public Shared Function LoWord(ByVal dWord As IntPtr) As Integer
        Return dWord.ToInt32() And &HFFFF
    End Function

    Public Shared Function HiWord(ByVal dWord As IntPtr) As Integer
        If (dWord.ToInt32() And &H80000000UI) = &H80000000UI Then
            Return (dWord.ToInt32() >> 16)
        Else
            Return (dWord.ToInt32() >> 16) And &HFFFF
        End If
    End Function

    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")> _
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")> _
    Public Shared Function ToIntPtr(ByVal [structure] As Object) As IntPtr
        Dim lparam As IntPtr = IntPtr.Zero
        lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf([structure]))
        Marshal.StructureToPtr([structure], lparam, False)
        Return lparam
    End Function


#End Region

#Region "Windows Structures and Enums"

    <Flags()> _
    Public Enum TCHITTESTFLAGS
        TCHT_NOWHERE = 1
        TCHT_ONITEMICON = 2
        TCHT_ONITEMLABEL = 4
        TCHT_ONITEM = TCHT_ONITEMICON Or TCHT_ONITEMLABEL
    End Enum



    <StructLayout(LayoutKind.Sequential)> _
    Public Structure TCHITTESTINFO

        Public Sub New(ByVal location As Point)
            pt = location
            flags = TCHITTESTFLAGS.TCHT_ONITEM
        End Sub

        Public pt As Point
        Public flags As TCHITTESTFLAGS
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=4)> _
    Public Structure PAINTSTRUCT
        Public hdc As IntPtr
        Public fErase As Integer
        Public rcPaint As RECT
        Public fRestore As Integer
        Public fIncUpdate As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)> _
        Public rgbReserved As Byte()
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Class RECT 'Structure 
        Public left As Integer
        Public top As Integer
        Public right As Integer
        Public bottom As Integer

        Public Sub New(ByVal left As Integer, ByVal top As Integer, ByVal right As Integer, ByVal bottom As Integer)
            Me.left = left
            Me.top = top
            Me.right = right
            Me.bottom = bottom
        End Sub

        Public Sub New(ByVal r As Rectangle)
            Me.left = r.Left
            Me.top = r.Top
            Me.right = r.Right
            Me.bottom = r.Bottom
        End Sub

        Public Shared Function FromXYWH(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As RECT
            Return New RECT(x, y, x + width, y + height)
        End Function

        Public Shared Function FromIntPtr(ByVal ptr As IntPtr) As RECT
            Dim rect As RECT = CType(Marshal.PtrToStructure(ptr, GetType(RECT)), RECT)
            Return rect
        End Function

        Public ReadOnly Property Size() As Size
            Get
                Return New Size(Me.right - Me.left, Me.bottom - Me.top)
            End Get
        End Property
    End Class


#End Region

End Class

Public Enum TabStyle
    None = 0
    [Default] = 1
    VisualStudio = 2
    Rounded = 3
    Angled = 4
    Chrome = 5
    IE8 = 6
    VS2010 = 7
End Enum

<ToolboxItemAttribute(False)> _
Public MustInherit Class TabStyleProvider
    Inherits Component
#Region "Constructor"

    Protected Sub New(ByVal tabControl As CustomTabControl)
        Me._TabControl = tabControl

        Me._BorderColor = Color.Empty
        Me._BorderColorSelected = Color.Empty
        Me._FocusColor = Color.Orange

        If Me._TabControl.RightToLeftLayout Then
            Me._ImageAlign = ContentAlignment.MiddleRight
        Else
            Me._ImageAlign = ContentAlignment.MiddleLeft
        End If

        Me.HotTrack = True

        '	Must set after the _Overlap as this is used in the calculations of the actual padding
        Me.Padding = New Point(6, 3)
    End Sub

#End Region

#Region "Factory Methods"

    Public Shared Function CreateProvider(ByVal tabControl As CustomTabControl) As TabStyleProvider
        Dim provider As TabStyleProvider

        '	Depending on the display style of the tabControl generate an appropriate provider.
        Select Case tabControl.DisplayStyle
            Case TabStyle.None
                provider = New TabStyleNoneProvider(tabControl)
                Exit Select

            Case TabStyle.[Default]
                provider = New TabStyleDefaultProvider(tabControl)
                Exit Select

            Case TabStyle.Angled
                provider = New TabStyleAngledProvider(tabControl)
                Exit Select

            Case TabStyle.Rounded
                provider = New TabStyleRoundedProvider(tabControl)
                Exit Select

            Case TabStyle.VisualStudio
                provider = New TabStyleVisualStudioProvider(tabControl)
                Exit Select

            Case TabStyle.Chrome
                provider = New TabStyleChromeProvider(tabControl)
                Exit Select

            Case TabStyle.IE8
                provider = New TabStyleIE8Provider(tabControl)
                Exit Select

            Case TabStyle.VS2010
                provider = New TabStyleVS2010Provider(tabControl)
                Exit Select
            Case Else

                provider = New TabStyleDefaultProvider(tabControl)
                Exit Select
        End Select

        provider._Style = tabControl.DisplayStyle
        Return provider
    End Function

#End Region

#Region "Protected variables"

    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _TabControl As CustomTabControl

    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _Padding As Point
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _HotTrack As Boolean
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _Style As TabStyle = TabStyle.[Default]


    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _ImageAlign As ContentAlignment
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _Radius As Integer = 1
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _Overlap As Integer
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _FocusTrack As Boolean
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _Opacity As Single = 1
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _ShowTabCloser As Boolean

    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _BorderColorSelected As Color = Color.Empty
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _BorderColor As Color = Color.Empty
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _BorderColorHot As Color = Color.Empty

    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _CloserColorActive As Color = Color.Black
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _CloserColor As Color = Color.DarkGray

    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _FocusColor As Color = Color.Empty

    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _TextColor As Color = Color.Empty
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _TextColorSelected As Color = Color.Empty
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")> _
    Protected _TextColorDisabled As Color = Color.Empty

#End Region

#Region "overridable Methods"

    Public MustOverride Sub AddTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal tabBounds As Rectangle)

    Public Overridable Function GetTabRect(ByVal index As Integer) As Rectangle

        If index < 0 Then
            Return New Rectangle()
        End If
        Dim tabBounds As Rectangle = Me._TabControl.GetTabRect(index)
        If Me._TabControl.RightToLeftLayout Then
            tabBounds.X = Me._TabControl.Width - tabBounds.Right
        End If
        Dim firstTabinRow As Boolean = Me._TabControl.IsFirstTabInRow(index)

        '	Expand to overlap the tabpage
        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                tabBounds.Height += 2
                Exit Select
            Case TabAlignment.Bottom
                tabBounds.Height += 2
                tabBounds.Y -= 2
                Exit Select
            Case TabAlignment.Left
                tabBounds.Width += 2
                Exit Select
            Case TabAlignment.Right
                tabBounds.X -= 2
                tabBounds.Width += 2
                Exit Select
        End Select


        '	Greate Overlap unless first tab in the row to align with tabpage
        If (Not firstTabinRow OrElse Me._TabControl.RightToLeftLayout) AndAlso Me._Overlap > 0 Then
            If Me._TabControl.Alignment <= TabAlignment.Bottom Then
                tabBounds.X -= Me._Overlap
                tabBounds.Width += Me._Overlap
            Else
                tabBounds.Y -= Me._Overlap
                tabBounds.Height += Me._Overlap
            End If
        End If

        '	Adjust first tab in the row to align with tabpage
        Me.EnsureFirstTabIsInView(tabBounds, index)

        Return tabBounds
    End Function


    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId:="0#")> _
    Protected Overridable Sub EnsureFirstTabIsInView(ByRef tabBounds As Rectangle, ByVal index As Integer)
        '	Adjust first tab in the row to align with tabpage
        '	Make sure we only reposition visible tabs, as we may have scrolled out of view.

        Dim firstTabinRow As Boolean = Me._TabControl.IsFirstTabInRow(index)

        If firstTabinRow Then
            If Me._TabControl.Alignment <= TabAlignment.Bottom Then
                If Me._TabControl.RightToLeftLayout Then
                    If tabBounds.Left < Me._TabControl.Right Then
                        Dim tabPageRight As Integer = Me._TabControl.GetPageBounds(index).Right
                        If tabBounds.Right > tabPageRight Then
                            tabBounds.Width -= (tabBounds.Right - tabPageRight)
                        End If
                    End If
                Else
                    If tabBounds.Right > 0 Then
                        Dim tabPageX As Integer = Me._TabControl.GetPageBounds(index).X
                        If tabBounds.X < tabPageX Then
                            tabBounds.Width -= (tabPageX - tabBounds.X)
                            tabBounds.X = tabPageX
                        End If
                    End If
                End If
            Else
                If Me._TabControl.RightToLeftLayout Then
                    If tabBounds.Top < Me._TabControl.Bottom Then
                        Dim tabPageBottom As Integer = Me._TabControl.GetPageBounds(index).Bottom
                        If tabBounds.Bottom > tabPageBottom Then
                            tabBounds.Height -= (tabBounds.Bottom - tabPageBottom)
                        End If
                    End If
                Else
                    If tabBounds.Bottom > 0 Then
                        Dim tabPageY As Integer = Me._TabControl.GetPageBounds(index).Location.Y
                        If tabBounds.Y < tabPageY Then
                            tabBounds.Height -= (tabPageY - tabBounds.Y)
                            tabBounds.Y = tabPageY
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Protected Overridable Function GetTabBackgroundBrush(ByVal index As Integer) As Brush
        Dim fillBrush As Drawing2D.LinearGradientBrush = Nothing

        '	Capture the colours dependant on selection state of the tab
        Dim dark As Color = Color.FromArgb(207, 207, 207)
        Dim light As Color = Color.FromArgb(242, 242, 242)

        If Me._TabControl.SelectedIndex = index Then
            dark = SystemColors.ControlLight
            light = SystemColors.Window
        ElseIf Not Me._TabControl.TabPages(index).Enabled Then
            light = dark
        ElseIf Me._HotTrack AndAlso index = Me._TabControl.ActiveIndex Then
            '	Enable hot tracking
            light = Color.FromArgb(234, 246, 253)
            dark = Color.FromArgb(167, 217, 245)
        End If

        '	Get the correctly aligned gradient
        Dim tabBounds As Rectangle = Me.GetTabRect(index)
        tabBounds.Inflate(3, 3)
        tabBounds.X -= 1
        tabBounds.Y -= 1
        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                If Me._TabControl.SelectedIndex = index Then
                    dark = light
                End If
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, light, dark, Drawing2D.LinearGradientMode.Vertical)
                Exit Select
            Case TabAlignment.Bottom
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, light, dark, Drawing2D.LinearGradientMode.Vertical)
                Exit Select
            Case TabAlignment.Left
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, dark, light, Drawing2D.LinearGradientMode.Horizontal)
                Exit Select
            Case TabAlignment.Right
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, light, dark, Drawing2D.LinearGradientMode.Horizontal)
                Exit Select
        End Select

        '	Add the blend
        fillBrush.Blend = Me.GetBackgroundBlend()

        Return fillBrush
    End Function

#End Region

#Region "Base Properties"

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property DisplayStyle() As TabStyle
        Get
            Return Me._Style
        End Get
        Set(ByVal value As TabStyle)
            Me._Style = value
        End Set
    End Property

    <Category("Appearance")> _
    Public Property ImageAlign() As ContentAlignment
        Get
            Return Me._ImageAlign
        End Get
        Set(ByVal value As ContentAlignment)
            Me._ImageAlign = value
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance")> _
    Public Property Padding() As Point
        Get
            Return Me._Padding
        End Get
        Set(ByVal value As Point)
            Me._Padding = value
            '	This line will trigger the handle to recreate, therefore invalidating the control
            If Me._ShowTabCloser Then
                If value.X + CInt(Me._Radius \ 2) < -6 Then
                    DirectCast(Me._TabControl, TabControl).Padding = New Point(0, value.Y)
                Else
                    DirectCast(Me._TabControl, TabControl).Padding = New Point(value.X + CInt(Me._Radius \ 2) + 6, value.Y)
                End If
            Else
                If value.X + CInt(Me._Radius \ 2) < 1 Then
                    DirectCast(Me._TabControl, TabControl).Padding = New Point(0, value.Y)
                Else
                    DirectCast(Me._TabControl, TabControl).Padding = New Point(value.X + CInt(Me._Radius \ 2) - 1, value.Y)
                End If
            End If
        End Set
    End Property


    <Category("Appearance"), DefaultValue(1), Browsable(True)> _
    Public Property Radius() As Integer
        Get
            Return Me._Radius
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then
                Throw New ArgumentException("The radius must be greater than 1", "value")
            End If
            Me._Radius = value
            '	Adjust padding
            Me.Padding = Me._Padding
        End Set
    End Property

    <Category("Appearance")> _
    Public Property Overlap() As Integer
        Get
            Return Me._Overlap
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then
                Throw New ArgumentException("The tabs cannot have a negative overlap", "value")
            End If

            Me._Overlap = value
        End Set
    End Property


    <Category("Appearance")> _
    Public Property FocusTrack() As Boolean
        Get
            Return Me._FocusTrack
        End Get
        Set(ByVal value As Boolean)
            Me._FocusTrack = value
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance")> _
    Public Property HotTrack() As Boolean
        Get
            Return Me._HotTrack
        End Get
        Set(ByVal value As Boolean)
            Me._HotTrack = value
            DirectCast(Me._TabControl, TabControl).HotTrack = value
        End Set
    End Property

    <Category("Appearance")> _
    Public Property ShowTabCloser() As Boolean
        Get
            Return Me._ShowTabCloser
        End Get
        Set(ByVal value As Boolean)
            Me._ShowTabCloser = value
            '	Adjust padding
            Me.Padding = Me._Padding
        End Set
    End Property

    <Category("Appearance")> _
    Public Property Opacity() As Single
        Get
            Return Me._Opacity
        End Get
        Set(ByVal value As Single)
            If value < 0 Then
                Throw New ArgumentException("The opacity must be between 0 and 1", "value")
            End If
            If value > 1 Then
                Throw New ArgumentException("The opacity must be between 0 and 1", "value")
            End If
            Me._Opacity = value
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "")> _
    Public Property BorderColorSelected() As Color
        Get
            If Me._BorderColorSelected.IsEmpty Then
                Return ThemedColors.ToolBorder
            Else
                Return Me._BorderColorSelected
            End If
        End Get
        Set(ByVal value As Color)
            If value.Equals(ThemedColors.ToolBorder) Then
                Me._BorderColorSelected = Color.Empty
            Else
                Me._BorderColorSelected = value
            End If
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "")> _
    Public Property BorderColorHot() As Color
        Get
            If Me._BorderColorHot.IsEmpty Then
                Return SystemColors.ControlDark
            Else
                Return Me._BorderColorHot
            End If
        End Get
        Set(ByVal value As Color)
            If value.Equals(SystemColors.ControlDark) Then
                Me._BorderColorHot = Color.Empty
            Else
                Me._BorderColorHot = value
            End If
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "")> _
    Public Property BorderColor() As Color
        Get
            If Me._BorderColor.IsEmpty Then
                Return SystemColors.ControlDark
            Else
                Return Me._BorderColor
            End If
        End Get
        Set(ByVal value As Color)
            If value.Equals(SystemColors.ControlDark) Then
                Me._BorderColor = Color.Empty
            Else
                Me._BorderColor = value
            End If
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "")> _
    Public Property TextColor() As Color
        Get
            If Me._TextColor.IsEmpty Then
                Return SystemColors.ControlText
            Else
                Return Me._TextColor
            End If
        End Get
        Set(ByVal value As Color)
            If value.Equals(SystemColors.ControlText) Then
                Me._TextColor = Color.Empty
            Else
                Me._TextColor = value
            End If
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "")> _
    Public Property TextColorSelected() As Color
        Get
            If Me._TextColorSelected.IsEmpty Then
                Return SystemColors.ControlText
            Else
                Return Me._TextColorSelected
            End If
        End Get
        Set(ByVal value As Color)
            If value.Equals(SystemColors.ControlText) Then
                Me._TextColorSelected = Color.Empty
            Else
                Me._TextColorSelected = value
            End If
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "")> _
    Public Property TextColorDisabled() As Color
        Get
            If Me._TextColor.IsEmpty Then
                Return SystemColors.ControlDark
            Else
                Return Me._TextColorDisabled
            End If
        End Get
        Set(ByVal value As Color)
            If value.Equals(SystemColors.ControlDark) Then
                Me._TextColorDisabled = Color.Empty
            Else
                Me._TextColorDisabled = value
            End If
            Me._TabControl.Invalidate()
        End Set
    End Property


    <Category("Appearance"), DefaultValue(GetType(Color), "Orange")> _
    Public Property FocusColor() As Color
        Get
            Return Me._FocusColor
        End Get
        Set(ByVal value As Color)
            Me._FocusColor = value
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "Black")> _
    Public Property CloserColorActive() As Color
        Get
            Return Me._CloserColorActive
        End Get
        Set(ByVal value As Color)
            Me._CloserColorActive = value
            Me._TabControl.Invalidate()
        End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "DarkGrey")> _
    Public Property CloserColor() As Color
        Get
            Return Me._CloserColor
        End Get
        Set(ByVal value As Color)
            Me._CloserColor = value
            Me._TabControl.Invalidate()
        End Set
    End Property

#End Region

#Region "Painting"

    Public Sub PaintTab(ByVal index As Integer, ByVal graphics As Graphics)
        Using tabpath As Drawing2D.GraphicsPath = Me.GetTabBorder(index)
            Using fillBrush As Brush = Me.GetTabBackgroundBrush(index)
                '	Paint the background
                graphics.FillPath(fillBrush, tabpath)

                '	Paint a focus indication
                If Me._TabControl.Focused Then
                    Me.DrawTabFocusIndicator(tabpath, index, graphics)
                End If

                '	Paint the closer

                Me.DrawTabCloser(index, graphics)
            End Using
        End Using
    End Sub

    Protected Overridable Sub DrawTabCloser(ByVal index As Integer, ByVal graphics As Graphics)
        If Me._ShowTabCloser Then
            Dim closerRect As Rectangle = Me._TabControl.GetTabCloserRect(index)
            graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Using closerPath As Drawing2D.GraphicsPath = TabStyleProvider.GetCloserPath(closerRect)
                If closerRect.Contains(Me._TabControl.MousePosition) Then
                    Using closerPen As New Pen(Me._CloserColorActive)
                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                Else
                    Using closerPen As New Pen(Me._CloserColor)
                        graphics.DrawPath(closerPen, closerPath)
                    End Using

                End If
            End Using
        End If
    End Sub

    Protected Shared Function GetCloserPath(ByVal closerRect As Rectangle) As Drawing2D.GraphicsPath
        Dim closerPath As New Drawing2D.GraphicsPath()
        closerPath.AddLine(closerRect.X, closerRect.Y, closerRect.Right, closerRect.Bottom)
        closerPath.CloseFigure()
        closerPath.AddLine(closerRect.Right, closerRect.Y, closerRect.X, closerRect.Bottom)
        closerPath.CloseFigure()

        Return closerPath
    End Function

    Private Sub DrawTabFocusIndicator(ByVal tabpath As Drawing2D.GraphicsPath, ByVal index As Integer, ByVal graphics As Graphics)
        If Me._FocusTrack AndAlso Me._TabControl.Focused AndAlso index = Me._TabControl.SelectedIndex Then
            Dim focusBrush As Brush = Nothing
            Dim pathRect As RectangleF = tabpath.GetBounds()
            Dim focusRect As Rectangle = Rectangle.Empty
            Select Case Me._TabControl.Alignment
                Case TabAlignment.Top
                    focusRect = New Rectangle(CInt(Math.Truncate(pathRect.X)), CInt(Math.Truncate(pathRect.Y)), CInt(Math.Truncate(pathRect.Width)), 4)
                    focusBrush = New Drawing2D.LinearGradientBrush(focusRect, Me._FocusColor, SystemColors.Window, Drawing2D.LinearGradientMode.Vertical)
                    Exit Select
                Case TabAlignment.Bottom
                    focusRect = New Rectangle(CInt(Math.Truncate(pathRect.X)), CInt(Math.Truncate(pathRect.Bottom)) - 4, CInt(Math.Truncate(pathRect.Width)), 4)
                    focusBrush = New Drawing2D.LinearGradientBrush(focusRect, SystemColors.ControlLight, Me._FocusColor, Drawing2D.LinearGradientMode.Vertical)
                    Exit Select
                Case TabAlignment.Left
                    focusRect = New Rectangle(CInt(Math.Truncate(pathRect.X)), CInt(Math.Truncate(pathRect.Y)), 4, CInt(Math.Truncate(pathRect.Height)))
                    focusBrush = New Drawing2D.LinearGradientBrush(focusRect, Me._FocusColor, SystemColors.ControlLight, Drawing2D.LinearGradientMode.Horizontal)
                    Exit Select
                Case TabAlignment.Right
                    focusRect = New Rectangle(CInt(Math.Truncate(pathRect.Right)) - 4, CInt(Math.Truncate(pathRect.Y)), 4, CInt(Math.Truncate(pathRect.Height)))
                    focusBrush = New Drawing2D.LinearGradientBrush(focusRect, SystemColors.ControlLight, Me._FocusColor, Drawing2D.LinearGradientMode.Horizontal)
                    Exit Select
            End Select

            '	Ensure the focus stip does not go outside the tab
            Dim focusRegion As New Region(focusRect)
            focusRegion.Intersect(tabpath)
            graphics.FillRegion(focusBrush, focusRegion)
            focusRegion.Dispose()
            focusBrush.Dispose()
        End If
    End Sub

#End Region

#Region "Background brushes"

    Private Function GetBackgroundBlend() As Drawing2D.Blend
        Dim relativeIntensities As Single() = New Single() {0.0F, 0.7F, 1.0F}
        Dim relativePositions As Single() = New Single() {0.0F, 0.6F, 1.0F}

        '	Glass look to top aligned tabs
        If Me._TabControl.Alignment = TabAlignment.Top Then
            relativeIntensities = New Single() {0.0F, 0.5F, 1.0F, 1.0F}
            relativePositions = New Single() {0.0F, 0.5F, 0.51F, 1.0F}
        End If

        Dim blend As New Drawing2D.Blend()
        blend.Factors = relativeIntensities
        blend.Positions = relativePositions

        Return blend
    End Function

    Public Overridable Function GetPageBackgroundBrush(ByVal index As Integer) As Brush

        '	Capture the colours dependant on selection state of the tab
        Dim light As Color = Color.FromArgb(242, 242, 242)
        If Me._TabControl.Alignment = TabAlignment.Top Then
            light = Color.FromArgb(207, 207, 207)
        End If

        If Me._TabControl.SelectedIndex = index Then
            light = SystemColors.Window
        ElseIf Not Me._TabControl.TabPages(index).Enabled Then
            light = Color.FromArgb(207, 207, 207)
        ElseIf Me._HotTrack AndAlso index = Me._TabControl.ActiveIndex Then
            '	Enable hot tracking
            light = Color.FromArgb(234, 246, 253)
        End If

        Return New SolidBrush(light)
    End Function

#End Region

#Region "Tab border and rect"

    Public Function GetTabBorder(ByVal index As Integer) As Drawing2D.GraphicsPath

        Dim path As New Drawing2D.GraphicsPath()
        Dim tabBounds As Rectangle = Me.GetTabRect(index)

        Me.AddTabBorder(path, tabBounds)

        path.CloseFigure()
        Return path
    End Function

#End Region

End Class

Friend NotInheritable Class ThemedColors

#Region "    Variables and Constants "

    Private Const NormalColor As String = "NormalColor"
    Private Const HomeStead As String = "HomeStead"
    Private Const Metallic As String = "Metallic"
    Private Const NoTheme As String = "NoTheme"

    Private Shared _toolBorder As Color()
#End Region

#Region "    Properties "

    <CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
    Public Shared ReadOnly Property CurrentThemeIndex() As ColorScheme
        Get
            Return ThemedColors.GetCurrentThemeIndex()
        End Get
    End Property

    <CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
    Public Shared ReadOnly Property ToolBorder() As Color
        Get
            Return ThemedColors._toolBorder(CInt(ThemedColors.CurrentThemeIndex))
        End Get
    End Property

#End Region

#Region "    Constructors "

    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")> _
    Shared Sub New()
        ThemedColors._toolBorder = New Color() {Color.FromArgb(127, 157, 185), Color.FromArgb(164, 185, 127), Color.FromArgb(165, 172, 178), Color.FromArgb(132, 130, 132)}
    End Sub

    Private Sub New()
    End Sub

#End Region

    Private Shared Function GetCurrentThemeIndex() As ColorScheme
        Dim theme As ColorScheme = ColorScheme.NoTheme

        If VisualStyles.VisualStyleInformation.IsSupportedByOS AndAlso VisualStyles.VisualStyleInformation.IsEnabledByUser AndAlso Application.RenderWithVisualStyles Then


            Select Case VisualStyles.VisualStyleInformation.ColorScheme
                Case NormalColor
                    theme = ColorScheme.NormalColor
                    Exit Select
                Case HomeStead
                    theme = ColorScheme.HomeStead
                    Exit Select
                Case Metallic
                    theme = ColorScheme.Metallic
                    Exit Select
                Case Else
                    theme = ColorScheme.NoTheme
                    Exit Select
            End Select
        End If

        Return theme
    End Function

    Public Enum ColorScheme
        NormalColor = 0
        HomeStead = 1
        Metallic = 2
        NoTheme = 3
    End Enum

End Class

<ToolboxItem(False)> _
Public Class TabStyleAngledProvider
    Inherits TabStyleProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
        Me._ImageAlign = ContentAlignment.MiddleRight
        Me._Overlap = 7
        Me._Radius = 10

        '	Must set after the _Radius as this is used in the calculations of the actual padding

        Me.Padding = New Point(10, 3)
    End Sub

    Public Overrides Sub AddTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal tabBounds As Rectangle)
        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X + Me._Radius - 2, tabBounds.Y + 2)
                path.AddLine(tabBounds.X + Me._Radius, tabBounds.Y, tabBounds.Right - Me._Radius, tabBounds.Y)
                path.AddLine(tabBounds.Right - Me._Radius + 2, tabBounds.Y + 2, tabBounds.Right, tabBounds.Bottom)
                Exit Select
            Case TabAlignment.Bottom
                path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right - Me._Radius + 2, tabBounds.Bottom - 2)
                path.AddLine(tabBounds.Right - Me._Radius, tabBounds.Bottom, tabBounds.X + Me._Radius, tabBounds.Bottom)
                path.AddLine(tabBounds.X + Me._Radius - 2, tabBounds.Bottom - 2, tabBounds.X, tabBounds.Y)
                Exit Select
            Case TabAlignment.Left
                path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X + 2, tabBounds.Bottom - Me._Radius + 2)
                path.AddLine(tabBounds.X, tabBounds.Bottom - Me._Radius, tabBounds.X, tabBounds.Y + Me._Radius)
                path.AddLine(tabBounds.X + 2, tabBounds.Y + Me._Radius - 2, tabBounds.Right, tabBounds.Y)
                Exit Select
            Case TabAlignment.Right
                path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right - 2, tabBounds.Y + Me._Radius - 2)
                path.AddLine(tabBounds.Right, tabBounds.Y + Me._Radius, tabBounds.Right, tabBounds.Bottom - Me._Radius)
                path.AddLine(tabBounds.Right - 2, tabBounds.Bottom - Me._Radius + 2, tabBounds.X, tabBounds.Bottom)
                Exit Select
        End Select
    End Sub

End Class

<ToolboxItem(False)> _
Public Class TabStyleChromeProvider
    Inherits TabStyleProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
        Me._Overlap = 16
        Me._Radius = 16
        Me._ShowTabCloser = True
        Me._CloserColorActive = Color.White

        '	Must set after the _Radius as this is used in the calculations of the actual padding
        Me.Padding = New Point(7, 5)
    End Sub

    Public Overrides Sub AddTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal tabBounds As Rectangle)

        Dim spread As Integer
        Dim eigth As Integer
        Dim sixth As Integer
        Dim quarter As Integer

        If Me._TabControl.Alignment <= TabAlignment.Bottom Then
            spread = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Height) * 2 / 3)))
            eigth = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Height) * 1 / 8)))
            sixth = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Height) * 1 / 6)))
            quarter = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Height) * 1 / 4)))
        Else
            spread = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Width) * 2 / 3)))
            eigth = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Width) * 1 / 8)))
            sixth = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Width) * 1 / 6)))
            quarter = CInt(Math.Truncate(Math.Floor(CDec(tabBounds.Width) * 1 / 4)))
        End If

        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top

                path.AddCurve(New Point() {New Point(tabBounds.X, tabBounds.Bottom), New Point(tabBounds.X + sixth, tabBounds.Bottom - eigth), New Point(tabBounds.X + spread - quarter, tabBounds.Y + eigth), New Point(tabBounds.X + spread, tabBounds.Y)})
                path.AddLine(tabBounds.X + spread, tabBounds.Y, tabBounds.Right - spread, tabBounds.Y)
                path.AddCurve(New Point() {New Point(tabBounds.Right - spread, tabBounds.Y), New Point(tabBounds.Right - spread + quarter, tabBounds.Y + eigth), New Point(tabBounds.Right - sixth, tabBounds.Bottom - eigth), New Point(tabBounds.Right, tabBounds.Bottom)})
                Exit Select
            Case TabAlignment.Bottom
                path.AddCurve(New Point() {New Point(tabBounds.Right, tabBounds.Y), New Point(tabBounds.Right - sixth, tabBounds.Y + eigth), New Point(tabBounds.Right - spread + quarter, tabBounds.Bottom - eigth), New Point(tabBounds.Right - spread, tabBounds.Bottom)})
                path.AddLine(tabBounds.Right - spread, tabBounds.Bottom, tabBounds.X + spread, tabBounds.Bottom)
                path.AddCurve(New Point() {New Point(tabBounds.X + spread, tabBounds.Bottom), New Point(tabBounds.X + spread - quarter, tabBounds.Bottom - eigth), New Point(tabBounds.X + sixth, tabBounds.Y + eigth), New Point(tabBounds.X, tabBounds.Y)})
                Exit Select
            Case TabAlignment.Left
                path.AddCurve(New Point() {New Point(tabBounds.Right, tabBounds.Bottom), New Point(tabBounds.Right - eigth, tabBounds.Bottom - sixth), New Point(tabBounds.X + eigth, tabBounds.Bottom - spread + quarter), New Point(tabBounds.X, tabBounds.Bottom - spread)})
                path.AddLine(tabBounds.X, tabBounds.Bottom - spread, tabBounds.X, tabBounds.Y + spread)
                path.AddCurve(New Point() {New Point(tabBounds.X, tabBounds.Y + spread), New Point(tabBounds.X + eigth, tabBounds.Y + spread - quarter), New Point(tabBounds.Right - eigth, tabBounds.Y + sixth), New Point(tabBounds.Right, tabBounds.Y)})

                Exit Select
            Case TabAlignment.Right
                path.AddCurve(New Point() {New Point(tabBounds.X, tabBounds.Y), New Point(tabBounds.X + eigth, tabBounds.Y + sixth), New Point(tabBounds.Right - eigth, tabBounds.Y + spread - quarter), New Point(tabBounds.Right, tabBounds.Y + spread)})
                path.AddLine(tabBounds.Right, tabBounds.Y + spread, tabBounds.Right, tabBounds.Bottom - spread)
                path.AddCurve(New Point() {New Point(tabBounds.Right, tabBounds.Bottom - spread), New Point(tabBounds.Right - eigth, tabBounds.Bottom - spread + quarter), New Point(tabBounds.X + eigth, tabBounds.Bottom - sixth), New Point(tabBounds.X, tabBounds.Bottom)})
                Exit Select
        End Select
    End Sub

    Protected Overrides Sub DrawTabCloser(ByVal index As Integer, ByVal graphics As Graphics)
        If Me._ShowTabCloser Then
            Dim closerRect As Rectangle = Me._TabControl.GetTabCloserRect(index)
            graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            If closerRect.Contains(Me._TabControl.MousePosition) Then
                Using closerPath As Drawing2D.GraphicsPath = GetCloserButtonPath(closerRect)
                    Using closerBrush As New SolidBrush(Color.FromArgb(193, 53, 53))
                        graphics.FillPath(closerBrush, closerPath)
                    End Using
                End Using
                Using closerPath As Drawing2D.GraphicsPath = GetCloserPath(closerRect)
                    Using closerPen As New Pen(Me._CloserColorActive)
                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                End Using
            Else
                Using closerPath As Drawing2D.GraphicsPath = GetCloserPath(closerRect)
                    Using closerPen As New Pen(Me._CloserColor)
                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                End Using


            End If
        End If
    End Sub

    Private Shared Function GetCloserButtonPath(ByVal closerRect As Rectangle) As Drawing2D.GraphicsPath
        Dim closerPath As New Drawing2D.GraphicsPath()
        closerPath.AddEllipse(New Rectangle(closerRect.X - 2, closerRect.Y - 2, closerRect.Width + 4, closerRect.Height + 4))
        closerPath.CloseFigure()
        Return closerPath
    End Function
End Class

<ToolboxItem(False)> _
Public Class TabStyleDefaultProvider
    Inherits TabStyleProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
        Me._FocusTrack = True
        Me._Radius = 2
    End Sub

    Public Overrides Sub AddTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal tabBounds As Rectangle)
        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y)
                path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y)
                path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom)
                Exit Select
            Case TabAlignment.Bottom
                path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom)
                path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom)
                path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y)
                Exit Select
            Case TabAlignment.Left
                path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom)
                path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y)
                path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y)
                Exit Select
            Case TabAlignment.Right
                path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y)
                path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom)
                path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom)
                Exit Select
        End Select
    End Sub

    Public Overrides Function GetTabRect(ByVal index As Integer) As Rectangle
        If index < 0 Then
            Return New Rectangle()
        End If

        Dim tabBounds As Rectangle = MyBase.GetTabRect(index)
        Dim firstTabinRow As Boolean = Me._TabControl.IsFirstTabInRow(index)

        '	Make non-SelectedTabs smaller and selected tab bigger
        If index <> Me._TabControl.SelectedIndex Then
            Select Case Me._TabControl.Alignment
                Case TabAlignment.Top
                    tabBounds.Y += 1
                    tabBounds.Height -= 1
                    Exit Select
                Case TabAlignment.Bottom
                    tabBounds.Height -= 1
                    Exit Select
                Case TabAlignment.Left
                    tabBounds.X += 1
                    tabBounds.Width -= 1
                    Exit Select
                Case TabAlignment.Right
                    tabBounds.Width -= 1
                    Exit Select
            End Select
        Else
            Select Case Me._TabControl.Alignment
                Case TabAlignment.Top
                    If tabBounds.Y > 0 Then
                        tabBounds.Y -= 1
                        tabBounds.Height += 1
                    End If

                    If firstTabinRow Then
                        tabBounds.Width += 1
                    Else
                        tabBounds.X -= 1
                        tabBounds.Width += 2
                    End If
                    Exit Select
                Case TabAlignment.Bottom
                    If tabBounds.Bottom < Me._TabControl.Bottom Then
                        tabBounds.Height += 1
                    End If
                    If firstTabinRow Then
                        tabBounds.Width += 1
                    Else
                        tabBounds.X -= 1
                        tabBounds.Width += 2
                    End If
                    Exit Select
                Case TabAlignment.Left
                    If tabBounds.X > 0 Then
                        tabBounds.X -= 1
                        tabBounds.Width += 1
                    End If

                    If firstTabinRow Then
                        tabBounds.Height += 1
                    Else
                        tabBounds.Y -= 1
                        tabBounds.Height += 2
                    End If
                    Exit Select
                Case TabAlignment.Right
                    If tabBounds.Right < Me._TabControl.Right Then
                        tabBounds.Width += 1
                    End If
                    If firstTabinRow Then
                        tabBounds.Height += 1
                    Else
                        tabBounds.Y -= 1
                        tabBounds.Height += 2
                    End If
                    Exit Select
            End Select
        End If

        '	Adjust first tab in the row to align with tabpage
        Me.EnsureFirstTabIsInView(tabBounds, index)

        Return tabBounds
    End Function
End Class

<ToolboxItem(False)> _
Public Class TabStyleIE8Provider
    Inherits TabStyleRoundedProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
        Me._Radius = 3
        Me._ShowTabCloser = True
        Me._CloserColorActive = Color.Red

        '	Must set after the _Radius as this is used in the calculations of the actual padding

        Me.Padding = New Point(6, 5)
    End Sub

    Public Overrides Function GetTabRect(ByVal index As Integer) As Rectangle
        If index < 0 Then
            Return New Rectangle()
        End If
        Dim tabBounds As Rectangle = MyBase.GetTabRect(index)
        Dim firstTabinRow As Boolean = Me._TabControl.IsFirstTabInRow(index)

        '	Make non-SelectedTabs smaller and selected tab bigger
        If index <> Me._TabControl.SelectedIndex Then
            Select Case Me._TabControl.Alignment
                Case TabAlignment.Top
                    tabBounds.Y += 1
                    tabBounds.Height -= 1
                    Exit Select
                Case TabAlignment.Bottom
                    tabBounds.Height -= 1
                    Exit Select
                Case TabAlignment.Left
                    tabBounds.X += 1
                    tabBounds.Width -= 1
                    Exit Select
                Case TabAlignment.Right
                    tabBounds.Width -= 1
                    Exit Select
            End Select
        Else
            Select Case Me._TabControl.Alignment
                Case TabAlignment.Top
                    tabBounds.Y -= 1
                    tabBounds.Height += 1

                    If firstTabinRow Then
                        tabBounds.Width += 1
                    Else
                        tabBounds.X -= 1
                        tabBounds.Width += 2
                    End If
                    Exit Select
                Case TabAlignment.Bottom
                    tabBounds.Height += 1

                    If firstTabinRow Then
                        tabBounds.Width += 1
                    Else
                        tabBounds.X -= 1
                        tabBounds.Width += 2
                    End If
                    Exit Select
                Case TabAlignment.Left
                    tabBounds.X -= 1
                    tabBounds.Width += 1

                    If firstTabinRow Then
                        tabBounds.Height += 1
                    Else
                        tabBounds.Y -= 1
                        tabBounds.Height += 2
                    End If
                    Exit Select
                Case TabAlignment.Right
                    tabBounds.Width += 1
                    If firstTabinRow Then
                        tabBounds.Height += 1
                    Else
                        tabBounds.Y -= 1
                        tabBounds.Height += 2
                    End If
                    Exit Select
            End Select
        End If

        '	Adjust first tab in the row to align with tabpage
        Me.EnsureFirstTabIsInView(tabBounds, index)
        Return tabBounds
    End Function

    Protected Overrides Function GetTabBackgroundBrush(ByVal index As Integer) As Brush
        Dim fillBrush As Drawing2D.LinearGradientBrush = Nothing

        '	Capture the colours dependant on selection state of the tab
        Dim dark As Color = Color.FromArgb(227, 238, 251)
        Dim light As Color = Color.FromArgb(227, 238, 251)

        If Me._TabControl.SelectedIndex = index Then
            dark = Color.FromArgb(196, 222, 251)
            light = SystemColors.Window
        ElseIf Not Me._TabControl.TabPages(index).Enabled Then
            light = dark
        ElseIf Me.HotTrack AndAlso index = Me._TabControl.ActiveIndex Then
            '	Enable hot tracking
            light = SystemColors.Window
            dark = Color.FromArgb(166, 203, 248)
        End If

        '	Get the correctly aligned gradient
        Dim tabBounds As Rectangle = Me.GetTabRect(index)
        tabBounds.Inflate(3, 3)
        tabBounds.X -= 1
        tabBounds.Y -= 1
        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, dark, light, Drawing2D.LinearGradientMode.Vertical)
                Exit Select
            Case TabAlignment.Bottom
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, light, dark, Drawing2D.LinearGradientMode.Vertical)
                Exit Select
            Case TabAlignment.Left
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, dark, light, Drawing2D.LinearGradientMode.Horizontal)
                Exit Select
            Case TabAlignment.Right
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, light, dark, Drawing2D.LinearGradientMode.Horizontal)
                Exit Select
        End Select

        '	Add the blend
        fillBrush.Blend = Me.GetBackgroundBlend(index)

        Return fillBrush
    End Function

    Private Overloads Function GetBackgroundBlend(ByVal index As Integer) As Drawing2D.Blend
        Dim relativeIntensities As Single() = New Single() {0.0F, 0.7F, 1.0F}
        Dim relativePositions As Single() = New Single() {0.0F, 0.8F, 1.0F}

        If Me._TabControl.SelectedIndex <> index Then
            relativeIntensities = New Single() {0.0F, 0.3F, 1.0F}
            relativePositions = New Single() {0.0F, 0.2F, 1.0F}
        End If

        Dim blend As New Drawing2D.Blend()
        blend.Factors = relativeIntensities
        blend.Positions = relativePositions

        Return blend
    End Function

    Protected Overrides Sub DrawTabCloser(ByVal index As Integer, ByVal graphics As Graphics)
        If Me._ShowTabCloser Then
            Dim closerRect As Rectangle = Me._TabControl.GetTabCloserRect(index)
            graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            If closerRect.Contains(Me._TabControl.MousePosition) Then
                Using closerPath As Drawing2D.GraphicsPath = GetCloserButtonPath(closerRect)
                    graphics.FillPath(Brushes.White, closerPath)
                    Using closerPen As New Pen(Me.BorderColor)
                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                End Using
                Using closerPath As Drawing2D.GraphicsPath = GetCloserPath(closerRect)
                    Using closerPen As New Pen(Me._CloserColorActive)
                        closerPen.Width = 2
                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                End Using
            Else
                Using closerPath As Drawing2D.GraphicsPath = GetCloserPath(closerRect)
                    Using closerPen As New Pen(Me._CloserColor)
                        closerPen.Width = 2

                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                End Using

            End If
        End If
    End Sub

    Private Shared Function GetCloserButtonPath(ByVal closerRect As Rectangle) As Drawing2D.GraphicsPath
        Dim closerPath As New Drawing2D.GraphicsPath()
        closerPath.AddLine(closerRect.X - 1, closerRect.Y - 2, closerRect.Right + 1, closerRect.Y - 2)
        closerPath.AddLine(closerRect.Right + 2, closerRect.Y - 1, closerRect.Right + 2, closerRect.Bottom + 1)
        closerPath.AddLine(closerRect.Right + 1, closerRect.Bottom + 2, closerRect.X - 1, closerRect.Bottom + 2)
        closerPath.AddLine(closerRect.X - 2, closerRect.Bottom + 1, closerRect.X - 2, closerRect.Y - 1)
        closerPath.CloseFigure()
        Return closerPath
    End Function

    Public Overrides Function GetPageBackgroundBrush(ByVal index As Integer) As Brush

        '	Capture the colours dependant on selection state of the tab
        Dim light As Color = Color.FromArgb(227, 238, 251)

        If Me._TabControl.SelectedIndex = index Then
            light = SystemColors.Window
        ElseIf Not Me._TabControl.TabPages(index).Enabled Then
            light = Color.FromArgb(207, 207, 207)
        ElseIf Me._HotTrack AndAlso index = Me._TabControl.ActiveIndex Then
            '	Enable hot tracking
            light = Color.FromArgb(234, 246, 253)
        End If

        Return New SolidBrush(light)
    End Function

End Class

<ToolboxItem(False)> _
Public Class TabStyleNoneProvider
    Inherits TabStyleProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
    End Sub

    Public Overrides Sub AddTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal tabBounds As Rectangle)
    End Sub
End Class

<ToolboxItem(False)> _
Public Class TabStyleRoundedProvider
    Inherits TabStyleProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
        Me._Radius = 10
        '	Must set after the _Radius as this is used in the calculations of the actual padding
        Me.Padding = New Point(6, 3)
    End Sub

    Public Overrides Sub AddTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal tabBounds As Rectangle)

        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y + Me._Radius)
                path.AddArc(tabBounds.X, tabBounds.Y, Me._Radius * 2, Me._Radius * 2, 180, 90)
                path.AddLine(tabBounds.X + Me._Radius, tabBounds.Y, tabBounds.Right - Me._Radius, tabBounds.Y)
                path.AddArc(tabBounds.Right - Me._Radius * 2, tabBounds.Y, Me._Radius * 2, Me._Radius * 2, 270, 90)
                path.AddLine(tabBounds.Right, tabBounds.Y + Me._Radius, tabBounds.Right, tabBounds.Bottom)
                Exit Select
            Case TabAlignment.Bottom
                path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom - Me._Radius)
                path.AddArc(tabBounds.Right - Me._Radius * 2, tabBounds.Bottom - Me._Radius * 2, Me._Radius * 2, Me._Radius * 2, 0, 90)
                path.AddLine(tabBounds.Right - Me._Radius, tabBounds.Bottom, tabBounds.X + Me._Radius, tabBounds.Bottom)
                path.AddArc(tabBounds.X, tabBounds.Bottom - Me._Radius * 2, Me._Radius * 2, Me._Radius * 2, 90, 90)
                path.AddLine(tabBounds.X, tabBounds.Bottom - Me._Radius, tabBounds.X, tabBounds.Y)
                Exit Select
            Case TabAlignment.Left
                path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X + Me._Radius, tabBounds.Bottom)
                path.AddArc(tabBounds.X, tabBounds.Bottom - Me._Radius * 2, Me._Radius * 2, Me._Radius * 2, 90, 90)
                path.AddLine(tabBounds.X, tabBounds.Bottom - Me._Radius, tabBounds.X, tabBounds.Y + Me._Radius)
                path.AddArc(tabBounds.X, tabBounds.Y, Me._Radius * 2, Me._Radius * 2, 180, 90)
                path.AddLine(tabBounds.X + Me._Radius, tabBounds.Y, tabBounds.Right, tabBounds.Y)
                Exit Select
            Case TabAlignment.Right
                path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right - Me._Radius, tabBounds.Y)
                path.AddArc(tabBounds.Right - Me._Radius * 2, tabBounds.Y, Me._Radius * 2, Me._Radius * 2, 270, 90)
                path.AddLine(tabBounds.Right, tabBounds.Y + Me._Radius, tabBounds.Right, tabBounds.Bottom - Me._Radius)
                path.AddArc(tabBounds.Right - Me._Radius * 2, tabBounds.Bottom - Me._Radius * 2, Me._Radius * 2, Me._Radius * 2, 0, 90)
                path.AddLine(tabBounds.Right - Me._Radius, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom)
                Exit Select
        End Select
    End Sub
End Class

<ToolboxItem(False)> _
Public Class TabStyleVisualStudioProvider
    Inherits TabStyleProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
        Me._ImageAlign = ContentAlignment.MiddleRight
        Me._Overlap = 7

        '	Must set after the _Radius as this is used in the calculations of the actual padding
        Me.Padding = New Point(14, 1)
    End Sub

    Public Overrides Sub AddTabBorder(ByVal path As Drawing2D.GraphicsPath, ByVal tabBounds As Rectangle)
        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X + tabBounds.Height - 4, tabBounds.Y + 2)
                path.AddLine(tabBounds.X + tabBounds.Height, tabBounds.Y, tabBounds.Right - 3, tabBounds.Y)
                path.AddArc(tabBounds.Right - 6, tabBounds.Y, 6, 6, 270, 90)
                path.AddLine(tabBounds.Right, tabBounds.Y + 3, tabBounds.Right, tabBounds.Bottom)
                Exit Select
            Case TabAlignment.Bottom
                path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom - 3)
                path.AddArc(tabBounds.Right - 6, tabBounds.Bottom - 6, 6, 6, 0, 90)
                path.AddLine(tabBounds.Right - 3, tabBounds.Bottom, tabBounds.X + tabBounds.Height, tabBounds.Bottom)
                path.AddLine(tabBounds.X + tabBounds.Height - 4, tabBounds.Bottom - 2, tabBounds.X, tabBounds.Y)
                Exit Select
            Case TabAlignment.Left
                path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X + 3, tabBounds.Bottom)
                path.AddArc(tabBounds.X, tabBounds.Bottom - 6, 6, 6, 90, 90)
                path.AddLine(tabBounds.X, tabBounds.Bottom - 3, tabBounds.X, tabBounds.Y + tabBounds.Width)
                path.AddLine(tabBounds.X + 2, tabBounds.Y + tabBounds.Width - 4, tabBounds.Right, tabBounds.Y)
                Exit Select
            Case TabAlignment.Right
                path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right - 2, tabBounds.Y + tabBounds.Width - 4)
                path.AddLine(tabBounds.Right, tabBounds.Y + tabBounds.Width, tabBounds.Right, tabBounds.Bottom - 3)
                path.AddArc(tabBounds.Right - 6, tabBounds.Bottom - 6, 6, 6, 0, 90)
                path.AddLine(tabBounds.Right - 3, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom)
                Exit Select
        End Select
    End Sub

End Class

<ToolboxItem(False)> _
Public Class TabStyleVS2010Provider
    Inherits TabStyleRoundedProvider
    Public Sub New(ByVal tabControl As CustomTabControl)
        MyBase.New(tabControl)
        Me._Radius = 3
        Me._ShowTabCloser = True
        Me._CloserColorActive = Color.Black
        Me._CloserColor = Color.FromArgb(117, 99, 61)
        Me._TextColor = Color.White
        Me._TextColorDisabled = Color.WhiteSmoke
        Me._BorderColor = Color.Transparent
        Me._BorderColorHot = Color.FromArgb(155, 167, 183)

        '	Must set after the _Radius as this is used in the calculations of the actual padding

        Me.Padding = New Point(6, 5)
    End Sub

    Protected Overrides Function GetTabBackgroundBrush(ByVal index As Integer) As Brush
        Dim fillBrush As Drawing2D.LinearGradientBrush = Nothing

        '	Capture the colours dependant on selection state of the tab
        Dim dark As Color = Color.Transparent
        Dim light As Color = Color.Transparent

        If Me._TabControl.SelectedIndex = index Then
            dark = Color.FromArgb(229, 195, 101)
            light = SystemColors.Window
        ElseIf Not Me._TabControl.TabPages(index).Enabled Then
            light = dark
        ElseIf Me.HotTrack AndAlso index = Me._TabControl.ActiveIndex Then
            '	Enable hot tracking
            dark = Color.FromArgb(108, 116, 118)
            light = dark
        End If

        '	Get the correctly aligned gradient
        Dim tabBounds As Rectangle = Me.GetTabRect(index)
        tabBounds.Inflate(3, 3)
        tabBounds.X -= 1
        tabBounds.Y -= 1
        Select Case Me._TabControl.Alignment
            Case TabAlignment.Top
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, light, dark, Drawing2D.LinearGradientMode.Vertical)
                Exit Select
            Case TabAlignment.Bottom
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, dark, light, Drawing2D.LinearGradientMode.Vertical)
                Exit Select
            Case TabAlignment.Left
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, light, dark, Drawing2D.LinearGradientMode.Horizontal)
                Exit Select
            Case TabAlignment.Right
                fillBrush = New Drawing2D.LinearGradientBrush(tabBounds, dark, light, Drawing2D.LinearGradientMode.Horizontal)
                Exit Select
        End Select

        '	Add the blend
        fillBrush.Blend = GetBackgroundBlend()

        Return fillBrush
    End Function

    Private Overloads Shared Function GetBackgroundBlend() As Drawing2D.Blend
        Dim relativeIntensities As Single() = New Single() {0.0F, 0.5F, 1.0F, 1.0F}
        Dim relativePositions As Single() = New Single() {0.0F, 0.5F, 0.51F, 1.0F}


        Dim blend As New Drawing2D.Blend()
        blend.Factors = relativeIntensities
        blend.Positions = relativePositions

        Return blend
    End Function

    Public Overrides Function GetPageBackgroundBrush(ByVal index As Integer) As Brush

        '	Capture the colours dependant on selection state of the tab
        Dim light As Color = Color.Transparent

        If Me._TabControl.SelectedIndex = index Then
            light = Color.FromArgb(229, 195, 101)
        ElseIf Not Me._TabControl.TabPages(index).Enabled Then
            light = Color.Transparent
        ElseIf Me._HotTrack AndAlso index = Me._TabControl.ActiveIndex Then
            '	Enable hot tracking
            light = Color.Transparent
        End If

        Return New SolidBrush(light)
    End Function

    Protected Overrides Sub DrawTabCloser(ByVal index As Integer, ByVal graphics As Graphics)
        If Me._ShowTabCloser Then
            Dim closerRect As Rectangle = Me._TabControl.GetTabCloserRect(index)
            graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            If closerRect.Contains(Me._TabControl.MousePosition) Then
                Using closerPath As Drawing2D.GraphicsPath = GetCloserButtonPath(closerRect)
                    graphics.FillPath(Brushes.White, closerPath)
                    Using closerPen As New Pen(Color.FromArgb(229, 195, 101))
                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                End Using
                Using closerPath As Drawing2D.GraphicsPath = GetCloserPath(closerRect)
                    Using closerPen As New Pen(Me._CloserColorActive)
                        closerPen.Width = 2
                        graphics.DrawPath(closerPen, closerPath)
                    End Using
                End Using
            Else
                If index = Me._TabControl.SelectedIndex Then
                    Using closerPath As Drawing2D.GraphicsPath = GetCloserPath(closerRect)
                        Using closerPen As New Pen(Me._CloserColor)
                            closerPen.Width = 2
                            graphics.DrawPath(closerPen, closerPath)
                        End Using
                    End Using
                ElseIf index = Me._TabControl.ActiveIndex Then
                    Using closerPath As Drawing2D.GraphicsPath = GetCloserPath(closerRect)
                        Using closerPen As New Pen(Color.FromArgb(155, 167, 183))
                            closerPen.Width = 2
                            graphics.DrawPath(closerPen, closerPath)
                        End Using
                    End Using
                End If

            End If
        End If
    End Sub

    Private Shared Function GetCloserButtonPath(ByVal closerRect As Rectangle) As Drawing2D.GraphicsPath
        Dim closerPath As New Drawing2D.GraphicsPath()
        closerPath.AddLine(closerRect.X - 1, closerRect.Y - 2, closerRect.Right + 1, closerRect.Y - 2)
        closerPath.AddLine(closerRect.Right + 2, closerRect.Y - 1, closerRect.Right + 2, closerRect.Bottom + 1)
        closerPath.AddLine(closerRect.Right + 1, closerRect.Bottom + 2, closerRect.X - 1, closerRect.Bottom + 2)
        closerPath.AddLine(closerRect.X - 2, closerRect.Bottom + 1, closerRect.X - 2, closerRect.Y - 1)
        closerPath.CloseFigure()
        Return closerPath
    End Function

End Class

'End Namespace