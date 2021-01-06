Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip
    '#If CONFIG = "Debug" Then
#If Not PlayerMonolitico Then

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(ComboBox), "ComboBox.ico")> _
    Public Class ComboBox : Inherits VGDDWidget

        Private _SelectedIndex As Integer = 1

        Private _BitmapOpen As String = ""
        Private _BitmapClose As String = ""
        Private _BitmapDown As String = ""
        Private _BitmapUp As String = ""

        Protected _VGDDImageOpen As VGDDImage
        Protected _VGDDImageClose As VGDDImage
        Protected _VGDDImageDown As VGDDImage
        Protected _VGDDImageUp As VGDDImage

        Private Shared _DefaultBitmapNameOpen As String = String.Empty
        Private Shared _DefaultBitmapNameClose As String = String.Empty
        Private Shared _DefaultBitmapNameDown As String = String.Empty
        Private Shared _DefaultBitmapNameUp As String = String.Empty

        Private _ComboOpened As Boolean = True
        Private requiresRedraw As Boolean

        Public Sub New()
            MyBase.New()
            _Instances += 1
            If Not Common.VGDDIsRunning Then
                'Me.Scheme = Common.CreateDesignScheme
            End If
            InitializeComponent()
            requiresRedraw = True

            If _DefaultBitmapNameOpen = String.Empty OrElse Common.GetBitmap(_DefaultBitmapNameOpen) Is Nothing Then
                _DefaultBitmapNameOpen = Common.ExtractDefaultBitmap("ExpandIcon.jpg", Me.GetType.Assembly)
            End If
            Common.SetBitmapName(_DefaultBitmapNameOpen, Me, _BitmapOpen, _VGDDImageOpen, requiresRedraw)

            If _DefaultBitmapNameClose = String.Empty OrElse Common.GetBitmap(_DefaultBitmapNameClose) Is Nothing Then
                _DefaultBitmapNameClose = Common.ExtractDefaultBitmap("CollapseIcon.jpg", Me.GetType.Assembly)
            End If
            Common.SetBitmapName(_DefaultBitmapNameClose, Me, _BitmapClose, _VGDDImageClose, requiresRedraw)

            If _DefaultBitmapNameUp = String.Empty OrElse Common.GetBitmap(_DefaultBitmapNameUp) Is Nothing Then
                _DefaultBitmapNameUp = Common.ExtractDefaultBitmap("combobox_arrow_up.png", Me.GetType.Assembly)
            End If
            Common.SetBitmapName(_DefaultBitmapNameUp, Me, _BitmapUp, _VGDDImageUp, requiresRedraw)

            If _DefaultBitmapNameDown = String.Empty OrElse Common.GetBitmap(_DefaultBitmapNameDown) Is Nothing Then
                _DefaultBitmapNameDown = Common.ExtractDefaultBitmap("combobox_arrow_down.png", Me.GetType.Assembly)
            End If
            Common.SetBitmapName(_DefaultBitmapNameDown, Me, _BitmapDown, _VGDDImageDown, requiresRedraw)

#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("ComboBox")
#End If

            Me.ListBox1.Items = New String() {"Item1", "Item2", "Item3", "Item4"}
            Me.ListBox1.SingleSel = False
            Me.btnOpenClose.NoPanel = True
            Me.btnSlideUp.NoPanel = True
            Me.btnSlideDown.NoPanel = True

            Me.SuspendLayout()
            Me.Size = New Size(200, 100)

            Me.RemovePropertyToShow("BackColor")
            Me.RemovePropertyToShow("Font")

            PlaceControls()
            MyBase._HasChildWidgets = True
        End Sub

        Private Sub PlaceControls()
            'If _Scheme Is Nothing Then Exit Sub
            Me.btnOpenClose.Scheme = Me.Scheme

            Dim ButtonsWidthPixel As Integer = Me.Width * _ButtonsWidth / 100
            With StaticText1
                .Left = 0
                .Width = Me.Width - ButtonsWidthPixel
                .Height = ButtonsWidthPixel
            End With
            With btnOpenClose
                .Visible = True
                .Width = ButtonsWidthPixel
                .Height = StaticText1.Height
                .Left = Me.Width - .Width
            End With
            If _ComboOpened Then
                btnOpenClose.Bitmap = _BitmapClose
                With ListBox1
                    .Left = 0
                    .Top = StaticText1.Height
                    .Width = StaticText1.Width  '+ 2
                    .Height = Me.Height - .Top
                    .Visible = True
                End With
                With btnSlideDown
                    .Visible = True
                    .Left = btnOpenClose.Left
                    .Height = ButtonsWidthPixel + 1
                    .Top = Me.Height - .Height + 1
                    .Width = ButtonsWidthPixel + 1
                End With
                With btnSlideUp
                    .Left = btnOpenClose.Left
                    .Top = ListBox1.Top
                    .Visible = True
                    .Width = ButtonsWidthPixel + 1
                    .Height = ButtonsWidthPixel + 1
                End With
                With Slider1
                    .Visible = True
                    .Left = btnOpenClose.Left
                    .Width = ButtonsWidthPixel
                    .SliderType = SliderType.ScrollBar
                    .Orientation = Common.Orientation.Vertical
                    .Top = ListBox1.Top + btnSlideUp.Height - 1
                    .Height = Me.Height - StaticText1.Height - btnSlideDown.Height - btnSlideUp.Height + 2
                    If ListBox1.Items Is Nothing Then
                        .Range = 10
                    Else
                        .Range = ListBox1.Items.Length
                    End If
                    .Pos = .Range * 0.2
                End With
            Else
                btnOpenClose.Bitmap = _BitmapOpen
                ListBox1.Visible = False
                btnSlideDown.Visible = False
                btnSlideUp.Visible = False
                Slider1.Visible = False
            End If
            With btnSlideDown
                .Bitmap = _BitmapDown
            End With
            With btnSlideUp
                .Bitmap = _BitmapUp
            End With
        End Sub

#Region "GDDProps"

        Private _ButtonsWidth As Integer = 10
        <Description("Percentage Width for the Button's Bitmaps of this ComboBox. Range 1-99")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        <DefaultValue(10)> _
        Public Property ButtonsWidth As Integer
            Get
                Return _ButtonsWidth
            End Get
            Set(ByVal value As Integer)
                If value >= 1 And value <= 99 Then
                    _ButtonsWidth = value
                    PlaceControls()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Bitmap for the Open Button of the ComboBox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property ButtonOpenBitmap As String
            Get
                Return _BitmapOpen
            End Get
            Set(ByVal value As String)
                _BitmapOpen = value
                If Opened Then
                    btnOpenClose.Bitmap = _BitmapClose
                Else
                    btnOpenClose.Bitmap = _BitmapOpen
                End If
                PlaceControls()
                Me.Invalidate()
            End Set
        End Property

        <Description("Bitmap for the Close Button of the ComboBox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property ButtonCloseBitmap As String
            Get
                Return _BitmapClose
            End Get
            Set(ByVal value As String)
                _BitmapClose = value
                If Opened Then
                    btnOpenClose.Bitmap = _BitmapClose
                Else
                    btnOpenClose.Bitmap = _BitmapOpen
                End If
                PlaceControls()
                Me.Invalidate()
            End Set
        End Property

        <Description("Bitmap for the Down Button of the ScrollBar")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property ButtonDownBitmap As String
            Get
                Return _BitmapDown
            End Get
            Set(ByVal value As String)
                _BitmapDown = value
                If value <> String.Empty Then
                    btnSlideDown.Bitmap = value
                End If
                PlaceControls()
                Me.Invalidate()
            End Set
        End Property

        <Description("Bitmap for the Up Button of the ScrollBar")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property ButtonUpBitmap As String
            Get
                Return _BitmapUp
            End Get
            Set(ByVal value As String)
                _BitmapUp = value
                If value <> String.Empty Then
                    btnSlideUp.Bitmap = value
                End If
                PlaceControls()
                Me.Invalidate()
            End Set
        End Property

        <Description("Initial Open state of the ComboBox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property Opened As Boolean
            Get
                Return _ComboOpened
            End Get
            Set(ByVal value As Boolean)
                _ComboOpened = value
                PlaceControls()
                Me.Invalidate()
            End Set
        End Property

        <Description("Items collection of the ComboBox")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Shadows Property Items() As String()
            Get
                Return ListBox1.Items
            End Get
            Set(ByVal value As String())
                ListBox1.Items = value
                Me.Invalidate()
            End Set
        End Property

        '<Description("Text of the ComboBox")> _
        '<EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        'Public Shadows Property text As String
        '    Get
        '        Return MyBase.Text
        '    End Get
        '    Set(ByVal value As String)
        '        StaticText1.Text = value
        '        Me.Invalidate()
        '    End Set
        'End Property

#End Region

#Region "VGDDCode"

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Site.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Site.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.MyCodeHead(_CDeclType)
            Dim MyCode As String = "", MyState As String = ""

            Dim MyClassName As String = Me.GetType.ToString

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If


            If _public Then
                CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            Else
                CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            End If
            CodeGen.AddLines(MyCodeHead, CodeGen.CodeHeadTemplate)

            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)

            Dim myText As String = ""
            Me.Text = Me.Text.PadRight(GetMaxTextLength(Me.TextStringID), "_") ' DW
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[SCHEME]", Me.Scheme))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, (IIf(Me.Public, "extern " & CodeGen.ConstructorTemplate.Trim & vbCrLf, "") & _
                CodeGen.TextDeclareHeaderTemplate(_CDeclType) & CodeGen.HeadersTemplate) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            'CodeGen.EventsToCode(MyControlId, Me.VGDDEvents)

            Try
                'Dim myAssembly As Reflection.Assembly = System.Reflection.Assembly.GetAssembly(Me.GetType)
                For Each oFolderNode As Xml.XmlNode In CodeGen.XmlTemplatesDoc.SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Project/*", MyClassName.Split(".")(1)))
                    MplabX.AddFile(oFolderNode)
                Next
            Catch ex As Exception
            End Try

        End Sub
#End If
#End Region

        Private Sub ComboBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            StaticText1.Text = MyBase.Text
            Me.Invalidate()
        End Sub

        Private Sub ComboBox_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
            'StaticText1.Name = Me.Site.Name & "StaticText1"
            'btnOpenClose.Name = Me.Site.Name & "ButtonOpenClose"
            'btnSlideDown.Name = Me.Site.Name & "ButtonSlideDown"
            'btnSlideUp.Name = Me.Site.Name & "ButtonSlideUp"
            'ListBox1.Name = Me.Site.Name & "ListBox1"
        End Sub

        Private Sub ComboBox_StateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.StateChanged
            ListBox1.State = Me.State
            btnOpenClose.State = Me.State
            btnSlideDown.State = Me.State
            btnSlideUp.State = Me.State
            Slider1.State = Me.State
            StaticText1.State = Me.State
        End Sub

        Private Sub ComboBox_SchemeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SchemeChanged
            Me.Invalidate()
        End Sub

        'Private Sub btnOpenClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOpenClose.Click
        '    _ComboOpened = Not _ComboOpened
        '    PlaceControls()
        'End Sub

        Private Sub ComboBox_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            PlaceControls()
        End Sub
    End Class
    '#End If
#End If
End Namespace