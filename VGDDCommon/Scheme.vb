Imports System.Xml
Imports System.ComponentModel
Imports System.Drawing

Namespace VGDDCommon

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)>
    Public Class VGDDScheme
        Implements ICustomTypeDescriptor

        Private _Name As String = "Default"
        Private _Color0 As Color
        Private _Color1 As Color
        Private _Textcolor0 As Color
        Private _Textcolor1 As Color
        Private _Embossdkcolor As Color
        Private _Embossltcolor As Color
        Private _Textcolordisabled As Color
        Private _Colordisabled As Color
        Private _Commonbkcolor As Color
        Private WithEvents _Font As VGDDFont
        Public UsedBy As New ArrayList
        Private MyPropsToRemove As String = String.Empty
        Private _CodeGenerated As Boolean
        Private _FillStyle As GFX_FILL_STYLE = GFX_FILL_STYLE.GFX_FILL_STYLE_COLOR
        Private _BackgroundType As GFX_BACKGROUND_TYPE = GFX_BACKGROUND_TYPE.GFX_BACKGROUND_COLOR
        Private _BackgroundImageName As String
        Private _BackgroundImage As VGDDImage
        Private _EmbossSize As Integer = 3
        Private _AlphaValue As Integer = 0
        Private _CommonBkLeft As Integer
        Private _CommonBkTop As Integer
        Private _Palette As VGDDPalette

        Public Event FontChanged()
        Public Event PropertiesChanged()

        Public Sub New()
            _Font = New VGDDFont '("Gentium Basic", 11.25, FontStyle.Regular)
#If Not PlayerMonolitico Then
            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    Me.RemovePropertyToShow("EmbossSize")
                    Me.RemovePropertyToShow("BackgroundImageName")
                    Me.RemovePropertyToShow("BackgroundType")
                    Me.RemovePropertyToShow("FillStyle")
                Case "MLA", "HARMONY"
                    Me.RemovePropertyToShow("GradientType")
                    Me.RemovePropertyToShow("GradientLength")
            End Select
            If Not Common.ProjectUsePalette Then
                Me.RemovePropertyToShow("Palette")
            End If
#End If
            Select Case Common.ProjectColourDepth
                Case 1
                    _Commonbkcolor = Color.White
                    _Color0 = Color.White
                    _Color1 = Color.Black
                    _Textcolor0 = Color.Black
                    _Textcolor1 = Color.Black
                    _Embossdkcolor = Color.Black
                    _Embossltcolor = Color.Black
                    _Textcolordisabled = Color.White
                    _Colordisabled = Color.White
                Case Else
                    If Common.DefaultSchemeXML <> String.Empty Then
                        Me.FromXml(Common.DefaultSchemeXML)
                    Else
                        _Commonbkcolor = Color.FromArgb(255, 255, 255)
                        _Color0 = Color.FromArgb(192, 255, 255)
                        _Color1 = Color.FromArgb(128, 128, 255)
                        _Textcolor0 = Color.FromArgb(24, 24, 24)
                        _Textcolor1 = Color.FromArgb(248, 252, 248)
                        _Embossdkcolor = Color.FromArgb(0, 0, 255)
                        _Embossltcolor = Color.FromArgb(192, 192, 255)
                        _Textcolordisabled = Color.FromArgb(128, 128, 128)
                        _Colordisabled = Color.FromArgb(208, 224, 240)
                    End If
            End Select
        End Sub

        Public Sub AddPropertyToShow(ByVal PropertyName As String)
            PropertyName = " " & PropertyName.Trim & " "
            If MyPropsToRemove.Contains(PropertyName) Then
                MyPropsToRemove = " " & MyPropsToRemove.Replace(PropertyName, "").Trim & " "
            End If
        End Sub

        Public Sub RemovePropertyToShow(ByVal PropertyName As String)
            PropertyName = " " & PropertyName.Trim & " "
            If Not MyPropsToRemove.Contains(PropertyName) Then
                MyPropsToRemove = " " & MyPropsToRemove.Trim & " " & PropertyName
            End If
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        <Editor(GetType(UiEditPalette), GetType(System.Drawing.Design.UITypeEditor))> _
        Property Palette As VGDDPalette
            Get
                Return _Palette
            End Get
            Set(ByVal value As VGDDPalette)
                _Palette = value
                If value IsNot Nothing Then
                    'For Each s As VGDDScheme In Common._Schemes.Values
                    '    If s.Palette IsNot Nothing AndAlso s.Palette.Name = _Palette.Name Then
                    '        s.Palette = _Palette
                    '    End If
                    'Next
                    Commonbkcolor = _Palette.GetNearestColor(Commonbkcolor)
                    Color0 = _Palette.GetNearestColor(Color0)
                    Color1 = _Palette.GetNearestColor(Color1)
                    Colordisabled = _Palette.GetNearestColor(Colordisabled)
                    Embossdkcolor = _Palette.GetNearestColor(Embossdkcolor)
                    Embossltcolor = _Palette.GetNearestColor(Embossltcolor)
                    Textcolor0 = _Palette.GetNearestColor(Textcolor0)
                    Textcolor1 = _Palette.GetNearestColor(Textcolor1)
                    Textcolordisabled = _Palette.GetNearestColor(Textcolordisabled)
                    GradientStartColor = _Palette.GetNearestColor(GradientStartColor)
                    GradientEndColor = _Palette.GetNearestColor(GradientEndColor)
                End If
            End Set
        End Property

        <Description("Horizontal starting position of the background")> _
        <DefaultValue(0)> _
        <Category("Scheme")> _
        Property CommonBkLeft() As Integer
            Get
                Return _CommonBkLeft
            End Get
            Set(ByVal value As Integer)
                _CommonBkLeft = value
            End Set
        End Property

        <Description("Vertical starting position of the background")> _
        <DefaultValue(0)> _
        <Category("Scheme")> _
        Property CommonBkTop() As Integer
            Get
                Return _CommonBkTop
            End Get
            Set(ByVal value As Integer)
                _CommonBkTop = value
            End Set
        End Property

        <Description("Alpha value used for alpha blending." & vbCrLf & _
            "When using Primitive Layer, accepted values are 0, 25, 50, 75 and 100" & vbCrLf & _
            "Unsupported values of alpha will be ignored.")> _
        <DefaultValue(0)> _
        <Category("Scheme")> _
        Property AlphaValue() As Integer
            Get
                Return _AlphaValue
            End Get
            Set(ByVal value As Integer)
                _AlphaValue = value
            End Set
        End Property

        <Description("Emboss size of the panels for 3-D effect. Set to zero if not used")> _
        <DefaultValue(3)> _
        <Category("Scheme")> _
        Property EmbossSize() As Integer
            Get
                Return _EmbossSize
            End Get
            Set(ByVal value As Integer)
                _EmbossSize = value
            End Set
        End Property

        <Description("Bitmap name to draw as background")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDBitmapFileChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("Scheme")> _
        Public Overridable Property BackgroundImageName() As String
            Get
                Return _BackgroundImageName
            End Get
            Set(ByVal value As String)
                If _BackgroundImage IsNot Nothing Then
                    _BackgroundImage.RemoveUsedBy(Me)
                End If
                _BackgroundImageName = value
                If value = String.Empty Then
                    _BackgroundImage = Nothing
                Else
                    _BackgroundImage = Common.GetBitmap(_BackgroundImageName)
                    If _BackgroundImage IsNot Nothing Then
                        _BackgroundImage.AddUsedBy(Me)
                    End If
                End If
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Background Type")> _
        <DefaultValue(GetType(GFX_BACKGROUND_TYPE), "GFX_FILL_STYLE_COLOR")> _
        <Category("Scheme")> _
        Public Property BackgroundType() As GFX_BACKGROUND_TYPE
#Else
        Public Property BackgroundType() As GFX_BACKGROUND_TYPE
#End If
            Get
                Return _BackgroundType
            End Get
            Set(ByVal value As GFX_BACKGROUND_TYPE)
                _BackgroundType = value
                If _BackgroundType = GFX_BACKGROUND_TYPE.GFX_BACKGROUND_IMAGE Then
                    Me.AddPropertyToShow("BackgroundImageName")
                Else
                    Me.RemovePropertyToShow("BackgroundImageName")
                    If _BackgroundType = GFX_BACKGROUND_TYPE.GFX_BACKGROUND_NONE Then
                        Me.RemovePropertyToShow("CommonBkLeft")
                        Me.RemovePropertyToShow("CommonBkTop")
                    Else
                        Me.AddPropertyToShow("CommonBkLeft")
                        Me.AddPropertyToShow("CommonBkTop")
                    End If
                End If
                RaiseEvent PropertiesChanged()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Fill Style")> _
        <DefaultValue(GetType(GFX_FILL_STYLE), "GFX_FILL_STYLE_COLOR")> _
        <Category("Scheme")> _
        Public Property FillStyle() As GFX_FILL_STYLE
#Else
        Public Property FillStyle() As GFX_FILL_STYLE
#End If
            Get
                Return _FillStyle
            End Get
            Set(ByVal value As GFX_FILL_STYLE)
                _FillStyle = value
                Select Case _FillStyle
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_NONE, _
                         GFX_FILL_STYLE.GFX_FILL_STYLE_COLOR, _
                         GFX_FILL_STYLE.GFX_FILL_STYLE_ALPHA_COLOR
                        Me.RemovePropertyToShow("GradientEndColor")
                        Me.RemovePropertyToShow("GradientStartColor")
                    Case Else
                        Me.AddPropertyToShow("GradientEndColor")
                        Me.AddPropertyToShow("GradientStartColor")
                        Common.ProjectUseGradient = True
                End Select
                Select Case _FillStyle
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_NONE
                        _gradientType = GFX_GRADIENT_TYPE.GRAD_NONE
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_HOR
                        _gradientType = GFX_GRADIENT_TYPE.GRAD_DOUBLE_HOR
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_VER
                        _gradientType = GFX_GRADIENT_TYPE.GRAD_DOUBLE_VER
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOWN
                        _gradientType = GFX_GRADIENT_TYPE.GRAD_DOWN
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_LEFT
                        _gradientType = GFX_GRADIENT_TYPE.GRAD_LEFT
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_RIGHT
                        _gradientType = GFX_GRADIENT_TYPE.GRAD_RIGHT
                    Case GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_UP
                        _gradientType = GFX_GRADIENT_TYPE.GRAD_UP
                End Select
                RaiseEvent PropertiesChanged()
            End Set
        End Property

#Region "Gradient"
        Private _gradientType As GFX_GRADIENT_TYPE = GFX_GRADIENT_TYPE.GRAD_NONE
        Private _gradientStartColor As Color = Color.LightGray
        Private _gradientEndColor As Color = Color.DarkGray
        Private _gradientLength As Integer = 50

#If Not PlayerMonolitico Then
        <Editor(GetType(GradientTypePropertyEditor), GetType(System.Drawing.Design.UITypeEditor))> _
        <Description("Gradient type")> _
        <DefaultValue(GetType(GFX_GRADIENT_TYPE), "GRAD_NONE")> _
        <Category("GradientScheme")> _
        Public Property GradientType() As GFX_GRADIENT_TYPE
#Else
        Public Property GradientType() As GFX_GRADIENT_TYPE
#End If
            Get
                Return _gradientType
            End Get
            Set(ByVal value As GFX_GRADIENT_TYPE)
                _gradientType = value
                If _gradientType = GFX_GRADIENT_TYPE.GRAD_NONE Then
                    Me.RemovePropertyToShow("GradientEndColor")
                    Me.RemovePropertyToShow("GradientStartColor")
                    Me.RemovePropertyToShow("GradientLength")
                Else
                    Me.AddPropertyToShow("GradientEndColor")
                    Me.AddPropertyToShow("GradientStartColor")
#If Not PlayerMonolitico Then
                    Select Case Mal.FrameworkName.ToUpper
                        Case "MLALEGACY"
                            Me.AddPropertyToShow("GradientLength")
                            Me.AddPropertyToShow("GradientType")
                    End Select
#End If
                    Common.ProjectUseGradient = True
                End If

                Select Case _gradientType
                    Case GFX_GRADIENT_TYPE.GRAD_NONE
                        '_FillStyle = GFX_FILL_STYLE.GFX_FILL_STYLE_NONE
                    Case GFX_GRADIENT_TYPE.GRAD_DOUBLE_HOR
                        _FillStyle = GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_HOR
                    Case GFX_GRADIENT_TYPE.GRAD_DOUBLE_VER
                        _FillStyle = GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOUBLE_VER
                    Case GFX_GRADIENT_TYPE.GRAD_DOWN
                        _FillStyle = GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_DOWN
                    Case GFX_GRADIENT_TYPE.GRAD_LEFT
                        _FillStyle = GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_LEFT
                    Case GFX_GRADIENT_TYPE.GRAD_RIGHT
                        _FillStyle = GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_RIGHT
                    Case GFX_GRADIENT_TYPE.GRAD_UP
                        _FillStyle = GFX_FILL_STYLE.GFX_FILL_STYLE_GRADIENT_UP
                End Select

                RaiseEvent PropertiesChanged()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Editor(GetType(VGDDCommon.MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <Description("Ending colour of gradient transition")> _
        <Category("GradientScheme")> _
        Public Property GradientEndColor() As Color
#Else
        Public Property GradientEndColor() As Color
#End If
            Get
                Return _gradientEndColor
            End Get
            Set(ByVal value As Color)
                _gradientEndColor = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Editor(GetType(VGDDCommon.MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <Description("Starting colour of gradient transition")> _
        <Category("GradientScheme")> _
        Public Property GradientStartColor() As Color
#Else
        Public Property GradientStartColor() As Color
#End If
            Get
                Return _gradientStartColor
            End Get
            Set(ByVal value As Color)
                _gradientStartColor = value
            End Set
        End Property

        <Category("GradientScheme")> _
        <Description("Length of the gradient transition in pixels (From 0-100%. How much of a gradient is wanted)")> _
        <DefaultValue(50)> _
        Public Property GradientLength() As Integer
            Get
                Return _gradientLength
            End Get
            Set(ByVal value As Integer)
                _gradientLength = value
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then
        <Description("Font to use")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDFontChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("Scheme")> _
        Public Property Font As VGDDFont
#Else
        Public Property Font As VGDDFont
#End If
            Get
                Return _Font
            End Get
            Set(ByVal value As VGDDFont)
                If _Font IsNot Nothing Then
                    _Font.RemoveUsedBy(Me)
                End If
                _Font = value
                If _Font IsNot Nothing Then
                    Common.AddFont(value, Me)
                    _Font.AddUsedBy(Me)
                End If
            End Set
        End Property

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

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property CodeGenerated As Boolean
            Get
                Return _CodeGenerated
            End Get
            Set(ByVal value As Boolean)
                _CodeGenerated = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public ReadOnly Property Referenced As Boolean
            Get
                Return Me.UsedBy.Count > 0
            End Get
        End Property

        <Description("Nome for this Scheme")> _
        <ParenthesizePropertyName(False)> _
        <Category("Scheme")> _
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = Common.CleanName(value)
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for color0")> _
        <Category("Scheme")> _
        <Editor(GetType(VGDDCommon.MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Color0() As Color
#Else
        Public Property Color0() As Color
#End If
            Get
                Return _Color0
            End Get
            Set(ByVal value As Color)
                _Color0 = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for color1")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Color1() As Color
#Else
        Public Property Color1() As Color
#End If
            Get
                Return _Color1
            End Get
            Set(ByVal value As Color)
                _Color1 = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for Textcolor0")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Textcolor0() As Color
#Else
        Public Property Textcolor0() As Color
#End If
            Get
                Return _Textcolor0
            End Get
            Set(ByVal value As Color)
                _Textcolor0 = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for Textcolor1")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Textcolor1() As Color
#Else
        Public Property Textcolor1() As Color
#End If
            Get
                Return _Textcolor1
            End Get
            Set(ByVal value As Color)
                _Textcolor1 = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for Colordisabled")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Colordisabled() As Color
#Else
        Public Property Colordisabled() As Color
#End If
            Get
                Return _Colordisabled
            End Get
            Set(ByVal value As Color)
                _Colordisabled = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for Embossdkcolor")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Embossdkcolor() As Color
#Else
        Public Property Embossdkcolor() As Color
#End If
            Get
                Return _Embossdkcolor
            End Get
            Set(ByVal value As Color)
                _Embossdkcolor = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for Embossltcolor")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Embossltcolor() As Color
#Else
        Public Property Embossltcolor() As Color
#End If
            Get
                Return _Embossltcolor
            End Get
            Set(ByVal value As Color)
                _Embossltcolor = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for Textcolordisabled")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Textcolordisabled() As Color
#Else
        Public Property Textcolordisabled() As Color
#End If
            Get
                Return _Textcolordisabled
            End Get
            Set(ByVal value As Color)
                _Textcolordisabled = value
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for Commonbkcolor")> _
        <Category("Scheme")> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Property Commonbkcolor() As Color
#Else
        Public Property Commonbkcolor() As Color
#End If
            Get
                Return _Commonbkcolor
            End Get
            Set(ByVal value As Color)
                _Commonbkcolor = value
            End Set
        End Property

        Private Function DeserializeColor(ByVal strColor As String) As Color
            Dim aColor() As String = strColor.Split(",")
            Return Color.FromArgb(255, aColor(0), aColor(1), aColor(2))
        End Function

        <System.ComponentModel.Browsable(False)> _
        Public Sub FromXml(ByRef Xml As String)
            Dim oXmlDoc As New XmlDocument
            oXmlDoc.LoadXml(Xml)
            Me.FromXml(oXmlDoc.DocumentElement)
        End Sub

        <System.ComponentModel.Browsable(False)> _
        Public Sub FromXml(ByRef XmlNode As XmlNode)
            Me.Name = XmlNode.Attributes("Name").Value.ToString

            If XmlNode.Attributes("Font") IsNot Nothing Then
                Me.Font = Common.GetFont(XmlNode.Attributes("Font").Value, Me)
            End If
            Me.Color0 = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Color0"))
            Me.Color1 = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Color1"))
            Me.Colordisabled = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Colordisabled"))
            Me.Commonbkcolor = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Commonbkcolor"))
            Me.Embossdkcolor = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Embossdkcolor"))
            Me.Embossltcolor = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Embossltcolor"))
            Me.Textcolor0 = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Textcolor0"))
            Me.Textcolor1 = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Textcolor1"))
            Me.Textcolordisabled = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "Textcolordisabled"))
            'Me.Pfont = VGDD.GetXmlNode(XmlNode, "Pfont")
            Dim strFontNode As String = Common.XmlNodeNoNull(XmlNode, "Font")

            Dim oPaletteNode As XmlNode = XmlNode.SelectSingleNode("Palette")
            If oPaletteNode IsNot Nothing Then
                _Palette = New VGDDPalette
                _Palette.FromXml(oPaletteNode)
            End If

            'Old format
            If (Me.Font Is Nothing OrElse Me.Font.Font Is Nothing) AndAlso strFontNode <> "" Then
                Dim aFont() As String = Common.XmlNodeNoNull(XmlNode, "Font").Split(",")
                If Not aFont(1).Contains(".") Then aFont(1) &= ".00"
                Dim oFont As New Font(aFont(0), Single.Parse(aFont(1).Replace(".", "") / 100),
                            CType(IIf(aFont(2).Contains("Regular"), FontStyle.Regular, 0) _
                            + IIf(aFont(2).Contains("Bold"), FontStyle.Bold, 0) _
                            + IIf(aFont(2).Contains("Italic"), FontStyle.Italic, 0), FontStyle))
                Me.Font = New VGDDFont
                Me.Font.Font = oFont
            End If

            If XmlNode.Attributes("gradientType") IsNot Nothing Then
                Me.GradientType = [Enum].Parse(GetType(VGDDScheme.GFX_GRADIENT_TYPE), XmlNode.Attributes("gradientType").Value)
                Me.GradientLength = XmlNode.Attributes("gradientLength").Value.ToString
                Me.GradientStartColor = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "gradientStartColor"))
                Me.GradientEndColor = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "gradientEndColor"))
            End If

            If XmlNode.Attributes("BackgroundType") IsNot Nothing Then
                Me.BackgroundType = [Enum].Parse(GetType(VGDDScheme.GFX_BACKGROUND_TYPE), XmlNode.Attributes("BackgroundType").Value)
            End If

            If XmlNode.Attributes("FillStyle") IsNot Nothing Then
                Me.FillStyle = [Enum].Parse(GetType(VGDDScheme.GFX_FILL_STYLE), XmlNode.Attributes("FillStyle").Value)
                Me.GradientStartColor = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "gradientStartColor"))
                Me.GradientEndColor = DeserializeColor(Common.XmlNodeNoNull(XmlNode, "gradientEndColor"))
            End If

            If XmlNode.Attributes("CommonBkLeft") IsNot Nothing Then
                Me.CommonBkLeft = XmlNode.Attributes("CommonBkLeft").Value
            End If

            If XmlNode.Attributes("CommonBkTop") IsNot Nothing Then
                Me.CommonBkTop = XmlNode.Attributes("CommonBkTop").Value
            End If

            If XmlNode.Attributes("BackgroundImageName") IsNot Nothing Then
                Me.BackgroundImageName = XmlNode.Attributes("BackgroundImageName").Value
            End If

            If XmlNode.Attributes("AlphaValue") IsNot Nothing Then
                Me.AlphaValue = XmlNode.Attributes("AlphaValue").Value
            End If

            If XmlNode.Attributes("EmbossSize") IsNot Nothing Then
                Me.EmbossSize = XmlNode.Attributes("EmbossSize").Value
            End If
        End Sub

#If Not PlayerMonolitico Then
        <System.ComponentModel.Browsable(False)> _
        Public Function ToXml() As String
            Dim oXmlDoc As New XmlDocument
            oXmlDoc.AppendChild(oXmlDoc.CreateElement("Element"))
            Me.ToXml(oXmlDoc)
            Dim sb As New System.Text.StringBuilder
            Dim XmlWriterSettings As New XmlWriterSettings()
            XmlWriterSettings.Indent = True
            XmlWriterSettings.IndentChars = "  "
            XmlWriterSettings.NewLineChars = vbCr & vbLf
            XmlWriterSettings.OmitXmlDeclaration = True
            XmlWriterSettings.NewLineHandling = NewLineHandling.Replace
            Using writer As XmlWriter = XmlWriter.Create(sb, XmlWriterSettings)
                oXmlDoc.SelectSingleNode("Element").FirstChild.WriteTo(writer)
            End Using
            Dim strXml As String = sb.ToString
            Return strXml
        End Function

        <System.ComponentModel.Browsable(False)> _
        Public Sub ToXml(ByRef XmlDoc As XmlDocument)
            If Me._Font Is Nothing OrElse Me._Font.Font Is Nothing Then Exit Sub
            Dim oSchemeElement As XmlElement = XmlDoc.CreateElement("Scheme")
            Dim oAttr As XmlAttribute = XmlDoc.CreateAttribute("Name")
            oAttr.Value = _Name
            oSchemeElement.Attributes.Append(oAttr)

            oAttr = XmlDoc.CreateAttribute("Font")
            oAttr.Value = Me.Font.Name
            oSchemeElement.Attributes.Append(oAttr)
            'Dim oFontElement As XmlElement = XmlDoc.CreateElement("Font")
            'oFontElement.InnerText = String.Format("{0},{1},{2}", Me._Font.Font.FontFamily.Name, Me._Font.Font.Size.ToString.Replace(",", "."), Me._Font.Font.Style)
            'oSchemeElement.AppendChild(oFontElement)
            'Dim oPfontElement As XmlElement = XmlDoc.CreateElement("Pfont")
            'oPfontElement.InnerText = String.Format("{0}", Me._Pfont)
            'oSchemeElement.AppendChild(oPfontElement)

            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Color0", Me.Color0))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Color1", Me.Color1))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Colordisabled", Me.Colordisabled))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Commonbkcolor", Me.Commonbkcolor))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Embossdkcolor", Me.Embossdkcolor))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Embossltcolor", Me.Embossltcolor))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Textcolor0", Me.Textcolor0))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Textcolor1", Me.Textcolor1))
            oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "Textcolordisabled", Me.Textcolordisabled))

            Select Case Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    oAttr = XmlDoc.CreateAttribute("gradientType")
                    oAttr.Value = _gradientType.ToString
                    oSchemeElement.Attributes.Append(oAttr)

                    oAttr = XmlDoc.CreateAttribute("gradientLength")
                    oAttr.Value = _gradientLength
                    oSchemeElement.Attributes.Append(oAttr)
                    oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "gradientStartColor", _gradientStartColor))
                    oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "gradientEndColor", _gradientEndColor))
                Case "MLA", "HARMONY"
                    oAttr = XmlDoc.CreateAttribute("BackgroundType")
                    oAttr.Value = _BackgroundType.ToString
                    oSchemeElement.Attributes.Append(oAttr)

                    oAttr = XmlDoc.CreateAttribute("FillStyle")
                    oAttr.Value = _FillStyle.ToString
                    oSchemeElement.Attributes.Append(oAttr)
                    oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "gradientStartColor", _gradientStartColor))
                    oSchemeElement.AppendChild(Common.ColorToXml(XmlDoc, "gradientEndColor", _gradientEndColor))

                    oAttr = XmlDoc.CreateAttribute("CommonBkLeft")
                    oAttr.Value = _CommonBkLeft
                    oSchemeElement.Attributes.Append(oAttr)

                    oAttr = XmlDoc.CreateAttribute("CommonBkTop")
                    oAttr.Value = _CommonBkTop
                    oSchemeElement.Attributes.Append(oAttr)

                    oAttr = XmlDoc.CreateAttribute("BackgroundImageName")
                    oAttr.Value = _BackgroundImageName
                    oSchemeElement.Attributes.Append(oAttr)

                    oAttr = XmlDoc.CreateAttribute("AlphaValue")
                    oAttr.Value = _AlphaValue
                    oSchemeElement.Attributes.Append(oAttr)

                    oAttr = XmlDoc.CreateAttribute("EmbossSize")
                    oAttr.Value = _EmbossSize
                    oSchemeElement.Attributes.Append(oAttr)
            End Select

            If Palette IsNot Nothing Then
                Palette.ToXml(oSchemeElement)
            End If

            XmlDoc.DocumentElement.AppendChild(oSchemeElement)
        End Sub
        'Dim oXmlNode As XmlNode = document.CreateNode(XmlNodeType.Element, "", "Scheme", "")
        'node.AppendChild(oXmlNode)
        'Dim nameAttr As XmlAttribute = document.CreateAttribute("name")
        'nameAttr.Value = oScheme.Name
        'oXmlNode.Attributes.Append(nameAttr)
        'Dim properties As PropertyDescriptorCollection = TypeDescriptor.GetProperties(oScheme)
        'For Each prop As PropertyDescriptor In properties
        '    If prop.ShouldSerializeValue(oScheme) Then
        '        Dim PropNode As XmlNode = document.CreateElement("Property")
        '        Dim attrName As XmlAttribute = document.CreateAttribute("name")
        '        attrName.Value = prop.Name
        '        PropNode.Attributes.Append(attrName)
        '        PropNode.Value = prop.GetValue(oScheme).ToString
        '        oXmlNode.AppendChild(PropNode)
        '    End If
        'Next
        'document.DocumentElement.AppendChild(oXmlNode)
#End If

        Public Enum GFX_GRADIENT_TYPE
            GRAD_NONE = 0
            GRAD_DOWN
            GRAD_RIGHT
            GRAD_UP
            GRAD_LEFT
            GRAD_DOUBLE_VER
            GRAD_DOUBLE_HOR
        End Enum

        Public Enum GFX_FILL_STYLE
            GFX_FILL_STYLE_NONE  'no fill
            GFX_FILL_STYLE_COLOR 'color fill
            GFX_FILL_STYLE_ALPHA_COLOR 'color fill with alpha blending
            GFX_FILL_STYLE_GRADIENT_DOWN 'gradient, vertical-down direction
            GFX_FILL_STYLE_GRADIENT_UP 'gradient, vertical-up direction
            GFX_FILL_STYLE_GRADIENT_RIGHT 'gradient, horizontal-right direction
            GFX_FILL_STYLE_GRADIENT_LEFT 'gradient, horizontal-left direction
            GFX_FILL_STYLE_GRADIENT_DOUBLE_VER 'gradient, vertical-up/down direction
            GFX_FILL_STYLE_GRADIENT_DOUBLE_HOR 'gradient, horizontal-left/right direction
        End Enum

        Public Enum GFX_BACKGROUND_TYPE
            GFX_BACKGROUND_NONE '// No background information set.
            GFX_BACKGROUND_COLOR '// Background is set to a color.
            GFX_BACKGROUND_IMAGE '// Background is set to an image.
            GFX_BACKGROUND_DISPLAY_BUFFER '// Background is set to the current content of the display buffer. This requires support of GFX_PixelArrayGet().    
        End Enum
#Region "ICustomTypeDescriptor Members"

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
            Implements ICustomTypeDescriptor.GetEvents
            Return TypeDescriptor.GetEvents(Me, True)
        End Function

        Public Function GetComponentName() As String _
            Implements ICustomTypeDescriptor.GetComponentName
            Return TypeDescriptor.GetComponentName(Me, True)
        End Function

        <System.Diagnostics.DebuggerStepThrough()> _
        Public Function GetPropertyOwner(ByVal pd As PropertyDescriptor) As Object _
        Implements ICustomTypeDescriptor.GetPropertyOwner
            Return Me
        End Function

        Public Function GetAttributes() As AttributeCollection _
            Implements ICustomTypeDescriptor.GetAttributes
            Return TypeDescriptor.GetAttributes(Me, True)
        End Function

        Private Function System_ComponentModel_ICustomTypeDescriptor_GetProperties() As PropertyDescriptorCollection _
            Implements ICustomTypeDescriptor.GetProperties
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

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection
            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not (Common.PROPSTOREMOVE & MyPropsToRemove & " ").Contains(" " & pd.Name & " ") Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

#End Region

        Private Sub _Font_FontChanged() Handles _Font.FontChanged
            RaiseEvent FontChanged()
        End Sub
    End Class


#If Not PlayerMonolitico Then
    Public Class GradientTypePropertyEditor
        Inherits CustomPropertyEditorBase

        Private WithEvents myListBox As New Windows.Forms.ListBox

        Protected Overrides Function GetEditControl(ByVal PropertyName As String, ByVal CurrentValue As Object) As System.Windows.Forms.Control
            With myListBox
                .BorderStyle = System.Windows.Forms.BorderStyle.None
                .Items.Clear()
                For Each GradientType As VGDDScheme.GFX_GRADIENT_TYPE In [Enum].GetValues(GetType(VGDDScheme.GFX_GRADIENT_TYPE))
                    .Items.Add(GradientType.ToString)
                    If CurrentValue IsNot Nothing AndAlso CurrentValue.ToString = GradientType.ToString Then
                        myListBox.SelectedIndex = myListBox.Items.Count - 1
                    End If
                Next
            End With
            Return myListBox
        End Function

        Protected Overrides Function GetEditedValue(ByVal EditControl As System.Windows.Forms.Control, ByVal PropertyName As String, ByVal OldValue As Object) As Object
            For Each GradientType As VGDDScheme.GFX_GRADIENT_TYPE In [Enum].GetValues(GetType(VGDDScheme.GFX_GRADIENT_TYPE))
                If myListBox.SelectedItem = GradientType.ToString Then
                    Return GradientType
                End If
            Next
            Return Nothing
        End Function

        Private Sub myListBox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles myListBox.Click
            Me.CloseDropDownWindow()
        End Sub
    End Class
#End If

End Namespace
