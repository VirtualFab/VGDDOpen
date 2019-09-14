Imports System.Xml
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Design

Namespace VGDDCommon

    <Serializable()> _
    Public Class VGDDFont
        Implements ICustomTypeDescriptor

        Private _Name As String
        'Private _FileName As String
        Public _Font As Font
        'Private _Referenced As Boolean = False
        Private _Type As FontType = FontType.FLASH_VGDD
        Private _CharSet As FontCharset = FontCharset.RANGE
        Private _StartChar As Integer = 32
        Private _EndChar As Integer = 126
        Private _CharsIncluded(-1) As Char
        Private _FileName As String
        Public _BinSize As Integer = -1
        Public UsedBy As New ArrayList
        Private _ByteArray As Byte()
        Private _SmartCharset As Boolean = False
        Private MyPropsToRemove As String = "CharsIncluded SmartCharSet"
        Private _IsGOLFontDefault As Boolean = False
        Private _ToBeConverted As Boolean = True
        Private _AntiAliasing As Boolean

        Public Event FontChanged()

        Const FONTTYPESHELP As String = "FLASH_VGDD=Internal Flash Font, VGDD will handle conversion and create suitable C code" & vbCrLf & _
            "EXTERNAL_VGDD_BIN=External Flash Font, VGDD will handle conversion and create suitable bin file" & vbCrLf & _
            "FLASH=Normal Internal Flash, to be converted by the user using GRC" & vbCrLf & _
            "EXTERNAL=External Flash Font, to be converted by the user using GRC" '& vbCrLf & _
        '"BINFONT_ON_SDFAT=Font will be read from SD using FSIO.c - VGDD will take care of converting it to .bin format and copy that file to the specified path"
        Public Sub New()
            'Me.new(Nothing)
        End Sub

        Public Sub New(ByVal Font As Font)
            If Font IsNot Nothing Then
                Me.Font = Font
                Me._Name = Common.FontToString(Font)
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

        Protected Overrides Sub Finalize()
            If Font IsNot Nothing Then
                Font.Dispose()
            End If
            MyBase.Finalize()
        End Sub

        Public Overrides Function ToString() As String
            Return Common.FontToString(_Font)
        End Function

        Public Enum FontType
            FLASH_VGDD = 1
            FLASH = 2
            EXTERNAL = 3
            EXTERNAL_VGDD = 4
            'BINFONT_ON_SDFAT = 4
            EXTERNAL_VGDD_BIN = 6
        End Enum

        Public Enum FontCharset
            RANGE = 1
            SELECTION = 2
        End Enum

        'Public Sub New(ByVal FileName As String)
        '    If FileName IsNot Nothing Then
        '        If File.Exists(FileName) Then
        '            _Font = Font.FromFile(FileName).Clone
        '        End If
        '        _Name = Path.GetFileNameWithoutExtension(FileName) '.ToLower
        '    Else
        '        _Bitmap = New Bitmap(1, 1)
        '    End If
        '    _FileName = FileName
        'End Sub

#Region "Properties"

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Property ToBeConverted As Boolean
            Get
                Return _ToBeConverted
            End Get
            Set(ByVal value As Boolean)
                _ToBeConverted = value
            End Set
        End Property

        <Description("Use this font for the #define FONTDEFAULT in GraphicsConfig.h? Note: Only one font in the project can be defined as FONTDEFAULT.")> _
        <DefaultValue(False)> _
        <Category("VGDD")> _
        Public Property IsGOLFontDefault As Boolean
            Get
                Return _IsGOLFontDefault
            End Get
            Set(ByVal value As Boolean)
                _IsGOLFontDefault = value
                _ToBeConverted = True
            End Set
        End Property

        <Description("When set to true, used characters will be detected and automatically added to the CharsIncluded property if they are not yet present in the list.")> _
        <Category("VGDD")> _
        Public Property SmartCharSet As Boolean
            Get
                Return _SmartCharset
            End Get
            Set(ByVal value As Boolean)
                _SmartCharset = value
                _ToBeConverted = True
            End Set
        End Property

        <Description("ByteArray for this Font after conversion")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        <Category("VGDD")> _
        Public Property ByteArray As Byte()
            Get
                Return _ByteArray
            End Get
            Set(ByVal value As Byte())
                '_ByteArray = New Byte(value.Length)
                'Array.Copy(value, _ByteArray, value.Length)
                _ByteArray = value
                If value IsNot Nothing Then
                    _BinSize = value.Length
                Else
                    _BinSize = 0
                End If
                _ToBeConverted = True
            End Set
        End Property

        <Description("Font Type" & vbCrLf & FONTTYPESHELP)> _
        <DefaultValue(GetType(FontType), "FLASH_VGDD")> _
        <Category("VGDD")> _
        Public Property Type As FontType
            Get
                Return (_Type)
            End Get
            Set(ByVal value As FontType)
                _Type = value
                Select Case _Type
                    Case FontType.FLASH_VGDD
                        MyPropsToRemove = MyPropsToRemove.Replace("BinFileName", "")
                    Case FontType.FLASH
                        MyPropsToRemove = MyPropsToRemove.Replace("BinFileName", "")
                    Case FontType.EXTERNAL
                        MyPropsToRemove &= "BinFileName"
                End Select
                'If _Type = PictureType.BINBMP_ON_SDFAT AndAlso _SDFileName = "" Then
                '    _SDFileName = _Name
                '    If _SDFileName.Length > 8 Then _SDFileName = _SDFileName.Substring(0, 8)
                '    _SDFileName &= ".bin"
                'End If
                _ToBeConverted = True
            End Set
        End Property

        <Description("Font FileName when Type=EXTERNAL : will be converted to bin and stored on file")> _
        <Category("VGDD")> _
        Public Property BinFileName As String
            Get
                If _Type = FontType.EXTERNAL Then
                    Return _FileName
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If _Type = FontType.EXTERNAL Then
                    _FileName = value
                End If
                _ToBeConverted = True
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)> _
        <Description("True if this Font is used by some Widget in project")> _
        <Category("VGDD")> _
        Public ReadOnly Property Referenced As Boolean
            Get
                Return Me.UsedBy.Count > 0
            End Get
        End Property

        <Description("Internal name for this Font")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <ParenthesizePropertyName(True)> _
        <Category("VGDD")> _
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = Common.CleanName(value)
                Dim oFont As VGDDFont = Common.GetFont(value, Nothing)
                If oFont IsNot Nothing Then
                    Me._Font = oFont.Font
                End If
                _ToBeConverted = True
            End Set
        End Property

        '<Description("Filename of the image on PC")> _
        '<EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        'Public Property FileName As String
        '    Get
        '        Return _FileName
        '    End Get
        '    Set(ByVal value As String)
        '        _FileName = value
        '    End Set
        'End Property

        <Description("Font Properties")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyFontEditor), GetType(System.Drawing.Design.UITypeEditor))> _
        <Category("VGDD")> _
        Public Property Font As Font
            Get
                Return _Font
            End Get
            Set(ByVal value As Font)
                _Font = value
                _Name = Common.FontToString(_Font)
                _ByteArray = Nothing
                _BinSize = -1
                _ToBeConverted = True
                RaiseEvent FontChanged()
            End Set
        End Property

        <Description("Font Charset" & vbCrLf & "RANGE (default) - Characters between <StartChar> and <EndChar> will be included in conversion" & vbCrLf & _
            "SELECTION - Only characters listed in <CharsIncluded> will be used, whether manually included by you or by VGDD, if <SmartCharSet> is used")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(FontCharset), "RANGE")> _
        <Category("VGDD")> _
        Public Property Charset As FontCharset
            Get
                Return _CharSet
            End Get
            Set(ByVal value As FontCharset)
                _CharSet = value
                If _CharSet = FontCharset.SELECTION Then
                    ReDim _CharsIncluded(-1)
                    MyPropsToRemove = MyPropsToRemove.Replace("CharsIncluded", "").Replace("SmartCharSet", "")
                    MyPropsToRemove &= "StartChar EndChar"
                ElseIf _CharSet = FontCharset.RANGE Then
                    _SmartCharset = False
                    MyPropsToRemove = MyPropsToRemove.Replace("StartChar", "").Replace("EndChar", "")
                    MyPropsToRemove &= "CharsIncluded SmartCharSet"
                End If
                _ByteArray = Nothing
                _BinSize = -1
                _ToBeConverted = True
            End Set
        End Property

        <Description("Range Start char")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(32)> _
        <Category("VGDD")> _
        Public Property StartChar As Integer
            Get
                If _CharSet = FontCharset.RANGE Then
                    Return _StartChar
                Else
                    Return -1
                End If
            End Get

            Set(ByVal value As Integer)
                _StartChar = value
                _ByteArray = Nothing
                _BinSize = -1
                _ToBeConverted = True
            End Set
        End Property

        <Description("Range End char")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(126)> _
        <Category("VGDD")> _
        Public Property EndChar As Integer
            Get
                If _CharSet = FontCharset.RANGE Then
                    Return _EndChar
                Else
                    Return -1
                End If
            End Get

            Set(ByVal value As Integer)
                _EndChar = value
                _ByteArray = Nothing
                _BinSize = -1
                _ToBeConverted = True
            End Set
        End Property

        <Description("Chars array to convert")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public Property CharsIncluded As Char()
            Get
                If Me.Charset = FontCharset.RANGE Then
                    Return Nothing
                Else ' FontCharset.SELECTION
                    If _CharsIncluded.Length > 0 AndAlso Array.IndexOf(_CharsIncluded, " "c) < 0 Then
                        Array.Resize(_CharsIncluded, _CharsIncluded.Length + 1)
                        _CharsIncluded(_CharsIncluded.Length - 1) = " "c
                        Array.Sort(_CharsIncluded)
                    End If
                    Return _CharsIncluded
                End If
            End Get
            Set(ByVal value As Char())
                _CharsIncluded = value
                _ByteArray = Nothing
                _BinSize = -1
                _ToBeConverted = True
            End Set
        End Property

        <Description("Size in bytes of the converted font. Before conversion it is -1")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("VGDD")> _
        Public ReadOnly Property BinSize As Integer
            Get
                Return _BinSize
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Description("Set to true to activate AntiAliasing feature in GRC for this Font")> _
        <Category("VGDD")> _
        Public Property AntiAliasing As Boolean
            Get
                Return _AntiAliasing
            End Get
            Set(ByVal value As Boolean)
                _AntiAliasing = value
            End Set
        End Property

#End Region

        Public Sub FromXml(ByVal XmlNode As XmlNode)
            Dim strFont As String = Common.XmlNodeNoNull(XmlNode, "Font") 'Old format
            If strFont = "" Then
                If XmlNode.Attributes("Font") IsNot Nothing Then
                    strFont = XmlNode.Attributes("Font").Value
                End If
            End If
            If strFont <> "" Then
                Dim aFont() As String = strFont.Split(",")
                If Not aFont(1).Contains(".") Then aFont(1) &= ".00"
                Dim strFontStyles As String = ""
                For i As Integer = 2 To aFont.Length - 1
                    strFontStyles &= aFont(i)
                Next
                Dim oFont As New Font(aFont(0), Single.Parse(aFont(1).Replace(".", "")) / 100, _
                            CType(IIf(aFont(2).Contains("Regular"), FontStyle.Regular, 0) _
                            + IIf(strFontStyles.Contains("Bold"), FontStyle.Bold, 0) _
                            + IIf(strFontStyles.Contains("Italic"), FontStyle.Italic, 0), FontStyle))
                If oFont Is Nothing Then
                    Debug.Print("")
                End If
                Me.Font = oFont
            End If

            Dim oAttr As XmlAttribute
            oAttr = XmlNode.Attributes("FileName")
            If oAttr IsNot Nothing Then
                Me.BinFileName = oAttr.Value.ToString
            End If
            oAttr = XmlNode.Attributes("Name")
            If oAttr IsNot Nothing Then
                Me.Name = oAttr.Value.ToString
            End If
            oAttr = XmlNode.Attributes("Type")
            If oAttr IsNot Nothing Then
                Dim converter As TypeConverter = TypeDescriptor.GetConverter(Me.Type)
                Me.Type = converter.ConvertFrom(oAttr.Value)
            End If
            oAttr = XmlNode.Attributes("SmartCharset")
            If oAttr IsNot Nothing Then
                Me.SmartCharSet = oAttr.Value.ToString
            End If
            oAttr = XmlNode.Attributes("StartChar")
            If oAttr IsNot Nothing Then
                Me.StartChar = oAttr.Value.ToString
            End If
            oAttr = XmlNode.Attributes("EndChar")
            If oAttr IsNot Nothing Then
                Me.EndChar = oAttr.Value.ToString
            End If
            oAttr = XmlNode.Attributes("Charset")
            If oAttr IsNot Nothing Then
                Dim converter As TypeConverter = TypeDescriptor.GetConverter(Me.Charset)
                Me.Charset = converter.ConvertFrom(oAttr.Value)
            End If
            oAttr = XmlNode.Attributes("CharsIncluded")
            If oAttr IsNot Nothing Then
                Me.CharsIncluded = oAttr.Value.ToString
            End If
            oAttr = XmlNode.Attributes("IsGOLFontDefault")
            If oAttr IsNot Nothing Then
                _IsGOLFontDefault = oAttr.Value.ToString
            End If
        End Sub

        <System.ComponentModel.Browsable(False)> _
        Public Sub ToXml(ByRef oNode As XmlNode)
            Dim XmlDoc As XmlDocument = oNode.OwnerDocument
            Dim oAttr As XmlAttribute
            oAttr = XmlDoc.CreateAttribute("Name")
            oAttr.Value = Me.Name
            oNode.Attributes.Append(oAttr)
            If oNode.Name = "VGDDFont" Then

                oAttr = XmlDoc.CreateAttribute("Type")
                oAttr.Value = Me.Type.ToString
                oNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("Charset")
                oAttr.Value = Me.Charset.ToString
                oNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("SmartCharset")
                oAttr.Value = Me.SmartCharSet.ToString
                oNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("StartChar")
                oAttr.Value = Me.StartChar
                oNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("EndChar")
                oAttr.Value = Me.EndChar
                oNode.Attributes.Append(oAttr)

                If Me.CharsIncluded IsNot Nothing AndAlso Me.CharsIncluded.Length > 0 Then
                    Array.Sort(Me.CharsIncluded)
                    oAttr = XmlDoc.CreateAttribute("CharsIncluded")
                    oAttr.Value = Me.CharsIncluded
                    oNode.Attributes.Append(oAttr)
                End If

                If Me.BinFileName <> "" Then
                    oAttr = XmlDoc.CreateAttribute("FileName")
                    oAttr.Value = Me.BinFileName
                    oNode.Attributes.Append(oAttr)
                End If

                oAttr = XmlDoc.CreateAttribute("Font")
                oAttr.Value = String.Format("{0},{1},{2}", Me.Font.FontFamily.Name, String.Format("{0:#.#0}", Me.Font.Size).Replace(",", "."), Me.Font.Style)
                oNode.Attributes.Append(oAttr)

                oAttr = XmlDoc.CreateAttribute("IsGOLFontDefault")
                oAttr.Value = _IsGOLFontDefault
                oNode.Attributes.Append(oAttr)

            End If

        End Sub

        <System.ComponentModel.Browsable(False)> _
        Public Sub ToXml(ByRef XmlDoc As XmlDocument)
            Dim oVGDDFontElement As XmlElement = XmlDoc.CreateElement("VGDDFont")
            Me.ToXml(oVGDDFontElement)
            XmlDoc.DocumentElement.AppendChild(oVGDDFontElement)
        End Sub

        Public Function SmartCharSetAddString(ByVal s As String) As Integer
            SmartCharSetAddString = 0
            If s Is Nothing Then Exit Function
            If _CharsIncluded Is Nothing Then ReDim _CharsIncluded(-1)
            For Each c As Char In s.Replace(vbCr, "").Replace(vbLf, "")
                If Array.IndexOf(_CharsIncluded, c) = -1 Then
                    Array.Resize(_CharsIncluded, _CharsIncluded.Length + 1)
                    _CharsIncluded(_CharsIncluded.Length - 1) = c
                    SmartCharSetAddString += 1
                End If
            Next
            If SmartCharSetAddString > 0 Then
                Array.Sort(_CharsIncluded)
                _ToBeConverted = True
            End If
        End Function

        'Public Function Clone() As VGDDFont
        '    Dim newVgddFont As New VGDDFont
        '    With newVgddFont
        '        .Font = Me.Font
        '        .BinFileName = Me.BinFileName
        '        ._BinSize = Me.BinSize
        '        .ByteArray = Me.ByteArray
        '        .Charset = Me.Charset
        '        .CharsIncluded = Me.CharsIncluded
        '        .EndChar = Me.EndChar
        '        .Font = Me.Font.Clone
        '        .Name = Me.Name
        '        .Referenced = Me.Referenced
        '        .SmartCharSet = Me.SmartCharSet
        '        .StartChar = Me.StartChar
        '        .Type = Me.Type
        '    End With
        '    Return newVgddFont
        'End Function

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
    End Class


    Public Class MyFontEditor
        Inherits UITypeEditor
        Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As UITypeEditorEditStyle
            Return UITypeEditorEditStyle.Modal
        End Function

        Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
            Dim dlg As New FontDialog()
            dlg = New FontDialog()

            Dim font As Font = TryCast(value, Font)
            If font IsNot Nothing Then
                dlg.SelectedFont = font
            End If

            If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Return dlg.SelectedFont
            End If

            Return MyBase.EditValue(context, provider, value)
        End Function
    End Class
End Namespace
