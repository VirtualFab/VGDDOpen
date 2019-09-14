Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports System.IO
Imports System.Xml
Imports System.Drawing.Design
Imports VGDDCommon
Imports VGDDCommon.Common
Imports System.Collections.Generic
Imports VGDDMicrochip.VGDDBase
Imports VirtualFabUtils.Utils

Namespace VGDDCommon

    <ToolboxBitmap(GetType(Button), "CustomIco")> _
    Public Class VGDDCustom : Inherits Windows.Forms.Control
        Implements ICustomTypeDescriptor
        Implements IVGDDWidget
        Implements ISupportInitialize

        'Public Shared CustomWidgetsFolder As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "CustomWidgets")
        Public Shared CustomWidgetsFolder As String
        Public Shared XmlCustomTemplatesDoc As XmlDocument = Nothing
        Public Shared XmlUserCustomTemplatesDoc As XmlDocument = Nothing
        Public Shared UsedCustomProps As New Dictionary(Of String, Object) ' New System.Collections.Specialized.OrderedDictionary
        Public PROPSTOREMOVECUSTOM As String = " Text TextAlign Font HasChildWidgets SchemeObj Anchor " & PROPSTOREMOVE

        Private _Color As Color = Drawing.Color.Blue
        Private _CustomWidgetType As String
        Private _X1 As Integer = 0
        Private _X2 As Integer = 20
        Private _Y1 As Integer = 0
        Private _Y2 As Integer = 100
        Private _Events As VGDDEventsCollection
        Public _CustomProperties As Hashtable = Nothing ' New Dictionary(Of String, Object)
        Private _Image As Image
        Private _InstancesSet As Boolean = False

        Private Shared _InstancesColl As New Collections.Generic.Dictionary(Of String, Integer)

        Public Sub New()
            MyBase.new()
            InitializeComponent()
        End Sub

        '<System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _InstancesColl(_CustomWidgetType) -= 1
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public ReadOnly Property Demolimit As Integer Implements IVGDDWidget.DemoLimit
            Get
                Return Common.DEMOCODELIMIT
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Shadows Property Text As String Implements IVGDDWidget.Text
            Get
                Return String.Empty
            End Get
            Set(ByVal value As String)
                ' TODO: implement set text
            End Set
        End Property

        Public Shared Sub LoadUserCustomTemplatesDoc()
            Try
                If Common.UserTemplatesFolder Is Nothing OrElse Common.UserTemplatesFolder = String.Empty Then Exit Sub
                Dim UserCustomWidgetsFile As String = Path.Combine(Common.UserTemplatesFolder, "UserCustomWidgetsTemplates.xml")
                XmlUserCustomTemplatesDoc = New XmlDocument
                If File.Exists(UserCustomWidgetsFile) Then
                    Try
                        Dim sr As StreamReader = New StreamReader(New FileStream(UserCustomWidgetsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        XmlUserCustomTemplatesDoc.Load(sr)
                        sr.Close()
                    Catch ex As Exception
                        MessageBox.Show("Error loading file " & UserCustomWidgetsFile & vbCrLf & ex.Message, "Error1 loading user Custom Widget Template")
                    End Try
                Else
                    XmlUserCustomTemplatesDoc.AppendChild(XmlUserCustomTemplatesDoc.CreateNode(XmlNodeType.Element, "UserCustomWidgets", String.Empty))
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error2 loading user Custom Widget Template")
            End Try

        End Sub

        Public Shared Sub LoadCustomTemplatesDoc()
            'TODO: Gestire i templates per il player
            Try
                CustomWidgetsFolder = Path.Combine(Path.GetDirectoryName(Application.CommonAppDataPath), "CustomWidgets")
                If Not Directory.Exists(CustomWidgetsFolder) Then
                    Directory.CreateDirectory(CustomWidgetsFolder)
                End If
                Dim strResName As String = "CustomWidgetsTemplates.xml"
                EnsureResourceExists(Path.Combine(CustomWidgetsFolder, strResName), strResName, True, System.Reflection.Assembly.GetExecutingAssembly())
                For Each strResName In {"alarm.ico", "alarm.png", "DigitalMeter.png", "DigitalMeter.ico", "Grid.ico", "Grid.jpg"}
                    EnsureResourceExists(Path.Combine(CustomWidgetsFolder, strResName), strResName, False, System.Reflection.Assembly.GetExecutingAssembly())
                Next
                'For Each strFileName As String In Directory.GetFiles(CustomWidgetsFolder)
                '    If Not File.Exists(Path.Combine(UserCustomWidgetsFolder, Path.GetFileName(strFileName))) Then
                '        File.Copy(strFileName, Path.Combine(UserCustomWidgetsFolder, Path.GetFileName(strFileName)))
                '    End If
                'Next
                Dim sr As StreamReader = New StreamReader(New FileStream(Path.Combine(CustomWidgetsFolder, "CustomWidgetsTemplates.xml"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                VGDDCustom.XmlCustomTemplatesDoc = New XmlDocument
                VGDDCustom.XmlCustomTemplatesDoc.Load(sr)
                sr.Close()

                LoadUserCustomTemplatesDoc()

                If XmlUserCustomTemplatesDoc IsNot Nothing Then
                    For Each oUserWidgetNode As XmlNode In XmlUserCustomTemplatesDoc.DocumentElement.ChildNodes
                        Dim oNodeNewWidget As XmlNode = VGDDCustom.XmlCustomTemplatesDoc.ImportNode(oUserWidgetNode, True)
                        'oNodeNewWidget.InnerXml = oUserWidgetNode.InnerXml
                        VGDDCustom.XmlCustomTemplatesDoc.DocumentElement.AppendChild(oNodeNewWidget)
                    Next
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Shared Sub SaveUserCustomTemplatesDoc()
            Dim sw As New StringWriter
            Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
            xtw.Formatting = Formatting.Indented
            XmlUserCustomTemplatesDoc.WriteTo(xtw)
            sw.Close()

            Dim UserCustomWidgetsFile As String = Path.Combine(Common.UserTemplatesFolder, "UserCustomWidgetsTemplates.xml")
            Dim file As StreamWriter = New StreamWriter(UserCustomWidgetsFile)
            file.Write(sw.ToString)
            file.Close()
        End Sub

        Public Shared Function CustomGetTemplate(ByVal xPath As String, ByVal NumTabsFirstLine As Integer, ByVal NumTabs As Integer) As String
            Dim oNode As XmlNode = Nothing
            Dim Template As String = ""
            Dim NumRiga As Integer = 0

            If XmlCustomTemplatesDoc IsNot Nothing Then oNode = XmlCustomTemplatesDoc.SelectSingleNode(xPath)
            If oNode Is Nothing Then Return String.Empty
            Dim OrgTemplate As String = oNode.InnerText()
            If OrgTemplate.StartsWith(vbCrLf) Then OrgTemplate = OrgTemplate.Substring(2)
            For Each Riga As String In OrgTemplate.Trim.Split(vbLf)
                NumRiga += 1
                Template &= vbCrLf & Space(IIf(NumRiga = 1, NumTabsFirstLine, NumTabs) * 4) & Riga.Replace(vbCr, "").Replace(vbLf, "").Trim
            Next
            Return Template.Substring(2).TrimEnd & vbCrLf
        End Function

        Public Shared Function CustomGetTemplateNode(ByVal xPath As String) As XmlNode
            Dim oNode As XmlNode = Nothing
            If XmlCustomTemplatesDoc IsNot Nothing Then oNode = XmlCustomTemplatesDoc.SelectSingleNode(xPath)
            If oNode Is Nothing Then Return Nothing
            Return oNode
        End Function

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            Me.OnPaintBackground(pevent)

            'Dim Mypath As GraphicsPath = New GraphicsPath
            'Mypath.StartFigure()
            'Mypath.AddRectangle(Me.ClientRectangle)
            'Mypath.CloseFigure()
            'Me.Region = New Region(Mypath)
            Me.Region = Nothing
            If Me._Image IsNot Nothing Then
                g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                g.DrawImage(Me._Image, 0, 0, Me.ClientRectangle.Width, Me.ClientRectangle.Height)
            Else
                Dim b As New Bitmap(Me.ClientRectangle.Width, Me.ClientRectangle.Height)
                Dim g2 As Graphics = Graphics.FromImage(b)
                g2.DrawLine(Pens.Red, 0, 0, b.Width, b.Height)
                g2.DrawLine(Pens.Red, b.Width, 0, 0, b.Height)
                g2.DrawString(Me.Name, MyBase.Font, Brushes.Blue, 0, 0)
                g.DrawImage(b, 0, 0)
                g2.Dispose()
            End If
        End Sub

        'Public Sub SetType(ByVal strType As String)
        '    Me._CustomWidgetType = strType
        'End Sub

#Region "IVGDD Stubs"

        Public Property Zorder As Integer Implements IVGDDWidget.Zorder

        Public Property SchemeObj As VGDDScheme Implements IVGDDWidget.SchemeObj
            Get
                If Me.Item("Scheme") IsNot Nothing Then
                    Dim oScheme As VGDDScheme = Common.GetScheme(Me.Item("Scheme"))
                    Return oScheme
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As VGDDScheme)
                Me.Item("Scheme") = value.Name
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public ReadOnly Property Instances As Integer Implements IVGDDWidget.Instances
            Get
                If _CustomWidgetType IsNot Nothing AndAlso _InstancesColl.ContainsKey(_CustomWidgetType) Then
                    Return _InstancesColl(_CustomWidgetType)
                Else
                    Return 0
                End If
            End Get
        End Property

        Public ReadOnly Property HasChildWidgets As Boolean Implements IVGDDWidget.HasChildWidgets
            Get
                Return False
            End Get
        End Property

#End Region

#Region "VGDDCustomProps"

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(False)> _
        Public Property Image() As Image
            Get
                Return _Image
            End Get
            Set(ByVal value As Image)
                _Image = value
            End Set
        End Property

        <Description("Type of Custom Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Overloads ReadOnly Property [Type]() As String
            Get
                Return _CustomWidgetType
            End Get
        End Property

        <Description("Name of the Custom Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Shadows Property Name() As String Implements IVGDDWidget.Name
            Get
                Return MyBase.Name
            End Get
            Set(ByVal value As String)
                Try
                    MyBase.Name = value
                    MyBase.Site.Name = value
                    Me.Invalidate()

                Catch ex As Exception

                End Try
            End Set
        End Property

        <Description("Type of Custom Widget")> _
        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        <CustomSortedCategory("Main", 2)> _
        Public Property CustomWidgetType() As String
            Get
                Return _CustomWidgetType
            End Get
            Set(ByVal value As String)
                _CustomWidgetType = value
                If Not _InstancesSet Then
                    _InstancesSet = True
                    If Not _InstancesColl.ContainsKey(_CustomWidgetType) Then
                        _InstancesColl.Add(_CustomWidgetType, 1)
                    Else
                        _InstancesColl(_CustomWidgetType) += 1
                    End If
                End If
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Advanced), Browsable(True)> _
        Default Public Property Item(ByVal fieldName As String) As Object
            Get
                Dim value As Object = Nothing
                If _CustomProperties IsNot Nothing AndAlso _CustomProperties.Contains(fieldName.ToUpper) Then
                    value = _CustomProperties(fieldName.ToUpper)
                End If
                Return value
            End Get
            Set(ByVal value As Object)
                If _CustomProperties IsNot Nothing Then
                    If Not _CustomProperties.Contains(fieldName.ToUpper) Then
                        _CustomProperties.Add(fieldName.ToUpper, value)
                    Else
                        _CustomProperties(fieldName.ToUpper) = value
                    End If
                    If Not _IsLoading Then
                        Select Case fieldName.ToUpper
                            Case "LEFT"
                                Me.Left = value
                            Case "RIGHT"
                                Me.Width = value - Me.Left
                            Case "TOP"
                                Me.Top = value
                            Case "BOTTOM"
                                Me.Height = value - Me.Top
                        End Select
                    End If
                    Me.Invalidate()
                End If
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Event handling for this Custom Widget")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDEventsEditorNew), GetType(System.Drawing.Design.UITypeEditor))> _
        <ParenthesizePropertyName(True)> _
        Public Property VGDDEvents() As VGDDEventsCollection Implements IVGDDWidget.VGDDEvents
            Get
                If _Events Is Nothing AndAlso XmlCustomTemplatesDoc IsNot Nothing AndAlso _CustomWidgetType IsNot Nothing Then
                    Dim oNode As XmlNode = XmlCustomTemplatesDoc.SelectSingleNode(String.Format("VGDDCustomWidgetsTemplate/{0}/Events", _CustomWidgetType))
                    If oNode IsNot Nothing Then
                        _Events = CodeGen.GetEventsFromTemplate(oNode)
                    End If
                End If
                Return _Events
            End Get
            Set(ByVal value As VGDDEventsCollection)
                _Events = value
            End Set
        End Property
#Else
        Public Property VGDDEvents() As VGDDEventsCollection Implements IVGDDWidget.VGDDEvents
            Get
                Return Nothing
            End Get
            Set(ByVal value As VGDDEventsCollection)
            End Set
        End Property
#End If

        Private Function FilterProperties(ByVal pdc As PropertyDescriptorCollection) As PropertyDescriptorCollection

            Dim adjustedProps As New PropertyDescriptorCollection(New PropertyDescriptor() {})
            For Each pd As PropertyDescriptor In pdc
                If Not PROPSTOREMOVECUSTOM.Contains(" " & pd.Name & " ") Then
                    adjustedProps.Add(pd)
                End If
            Next
            Return adjustedProps
        End Function

#End Region

#Region "VGDDProps"

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overloads Property Location As Point
            Get
                Return MyBase.Location
            End Get
            Set(ByVal value As Point)
                MyBase.Location = value
                Me.Invalidate()
                'CheckEval("Left")
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Shadows Property Size As Size
            Get
                Return MyBase.Size
            End Get
            Set(ByVal value As Size)
                MyBase.Size = value
                Me.Invalidate()
            End Set
        End Property

        Private Sub VGDDCustom_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
            If Not _IsLoading Then
                CheckEval("Left,Right,Top,Bottom")
            End If
        End Sub

        Private Sub VGDDCustom_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            If Not _IsLoading Then
                CheckEval("Left,Right,Top,Bottom")
            End If
        End Sub

#End Region

        Private Sub CheckEval(ByVal PropsToCheck As String)
            Dim mEvaluator As New Evaluator
            Dim cTd As New VGDDCustomTypeDescriptor(TypeDescriptor.GetProvider(GetType(VGDDCustom)).GetTypeDescriptor(Me), Me)
            Dim oPropBag As VGDDCustomPropBag = cTd.GetCustomPropertyBag(Me._CustomWidgetType)
            If oPropBag IsNot Nothing Then
                Dim oPropColl As System.Collections.Hashtable = oPropBag.CustomPropsCollection.Clone
                For Each oProp As VGDDCustomProp In oPropColl.Values
                    For Each PropToCheck As String In PropsToCheck.Split(",")
                        If oProp IsNot Nothing AndAlso oProp.Eval IsNot Nothing AndAlso oProp.Eval.Contains(PropToCheck) Then
                            Dim strExpr As String = oProp.Eval _
                                                  .Replace("Left", Me.Left).Replace("left", Me.Left) _
                                                .Replace("Top", Me.Top).Replace("top", Me.Top) _
                                                .Replace("Width", Me.Width).Replace("width", Me.Width) _
                                                .Replace("Height", Me.Height).Replace("height", Me.Height)

                            Dim res As Integer = CInt(mEvaluator.Eval(strExpr))
                            Me.Item(oProp.Name) = res
                        End If
                    Next
                Next
            End If
        End Sub

#Region "ICustomTypeDescriptor Members"

        Private Function GetProperties(ByVal attributes() As Attribute) As PropertyDescriptorCollection _
            Implements ICustomTypeDescriptor.GetProperties
            Try
                Dim cTd As New VGDDCustomTypeDescriptor(TypeDescriptor.GetProvider(GetType(VGDDCustom)).GetTypeDescriptor(Me), Me)
                Dim pdc As PropertyDescriptorCollection = cTd.GetProperties(attributes)
                Return FilterProperties(pdc)

            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Private Function GetProperties() As PropertyDescriptorCollection _
            Implements ICustomTypeDescriptor.GetProperties
            Return GetProperties(Nothing)
        End Function

        Private Function GetEvents(ByVal attributes As Attribute()) As EventDescriptorCollection _
            Implements ICustomTypeDescriptor.GetEvents

            Dim cTd As New VGDDCustomTypeDescriptor(TypeDescriptor.GetProvider(GetType(VGDDCustom)).GetTypeDescriptor(Me), Me)
            Return cTd.GetEvents(attributes)
        End Function

        Public Function GetConverter() As TypeConverter _
            Implements ICustomTypeDescriptor.GetConverter
            Try
                Dim cTd As New VGDDCustomTypeDescriptor(TypeDescriptor.GetProvider(GetType(VGDDCustom)).GetTypeDescriptor(Me), Me)
                Return cTd.GetConverter

            Catch ex As Exception

            End Try
            Return Nothing
        End Function

        Private Function System_ComponentModel_ICustomTypeDescriptor_GetEvents() As EventDescriptorCollection _
            Implements System.ComponentModel.ICustomTypeDescriptor.GetEvents

            Dim cTd As New VGDDCustomTypeDescriptor(TypeDescriptor.GetProvider(GetType(VGDDCustom)).GetTypeDescriptor(Me), Me)
            Return cTd.GetEvents()
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
            Try
                Dim cTd As New VGDDCustomTypeDescriptor(TypeDescriptor.GetProvider(GetType(VGDDCustom)).GetTypeDescriptor(Me), Me)
                Return cTd.GetAttributes
            Catch ex As Exception
            End Try
            Return Nothing
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
#If Not PlayerMonolitico Then

        Public Sub GetCode(ByVal ControlIdPrefix As String) Implements IVGDDWidget.GetCode
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = ""
            Dim MyCode As String = ""
            Dim MyState As New Hashtable
            Dim oPropBag As VGDDCustomPropBag = Nothing

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            Try
                oPropBag = VGDDCustom.UsedCustomProps(Me.Type)
                For Each oProp As VGDDCustomProp In oPropBag.CustomPropsCollection.Values
                    If oProp.DestProperty <> "" Then
                        If Not MyState.ContainsKey(oProp.DestProperty.ToUpper) Then
                            MyState.Add(oProp.DestProperty.ToUpper, "")
                        End If
                        CodeGen.AddState(MyState(oProp.DestProperty.ToUpper), oProp.Name, Me.Item(oProp.Name).ToString)
                    End If
                Next
            Catch ex As Exception
                Debug.Print("?")
            End Try

            Dim MyPublic As Boolean
            Boolean.TryParse(_CustomProperties("PUBLIC"), MyPublic)
            If MyPublic Then
                MyCodeHead &= CodeGen.ConstructorTemplate.Trim
            Else
                MyCode &= CodeGen.ConstructorTemplate
            End If

            MyCode &= CodeGen.CodeTemplate & CodeGen.AllCodeTemplate.Trim

            MyCode = MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color)) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)
            Do While True
                Dim lngPos As Integer = MyCode.IndexOf("[")
                If lngPos < 0 Then
                    Exit Do
                End If
                Dim strPropname As String = MyCode.Substring(lngPos + 1)
                strPropname = strPropname.Substring(0, strPropname.IndexOf("]"))
                Dim value As Object = ""
                If _CustomProperties.Contains(strPropname.ToUpper) Then
                    value = _CustomProperties(strPropname.ToUpper)
                    'ElseIf Me.GetProperties.Item(strPropname) IsNot Nothing Then
                    '    Dim oPropDesc As PropertyDescriptor = Me.GetProperties.Item(strPropname)
                    '    value = oPropDesc.GetValue(Me)
                ElseIf MyState.ContainsKey(strPropname.ToUpper) Then
                    value = MyState(strPropname.ToUpper)
                Else
                    Debug.Print("Unhandled property " & strPropname)
                End If
                If TypeOf (value) Is System.Single Then
                    MyCode = MyCode.Replace("[" & strPropname & "]", Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture))
                ElseIf TypeOf (value) Is System.Boolean Then
                    MyCode = MyCode.Replace("[" & strPropname & "]", value.ToString.ToUpper)
                ElseIf strPropname = "BITMAP" Then
                    If value = "" Then
                        MyCode = MyCode.Replace("[" & strPropname & "]", "NULL")
                    Else
                        MyCode = MyCode.Replace("[" & strPropname & "]", "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & value.ToString)
                    End If
                Else
                    If oPropBag.CustomPropsCollection.ContainsKey(strPropname) Then
                        If CType(oPropBag.CustomPropsCollection(strPropname), VGDDCustomProp).StrType = "Bool" Then
                            MyCode = MyCode.Replace("[" & strPropname & "]", value.ToString.ToUpper)
                        Else
                            MyCode = MyCode.Replace("[" & strPropname & "]", value)
                        End If
                    Else
                        MyCode = MyCode.Replace("[" & strPropname & "]", value)
                    End If
                End If
            Loop
            CodeGen.Code &= vbCrLf & MyCode
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.CodeHead &= vbCrLf & MyCodeHead
            End If

            'CodeGen.Headers &= vbCrLf & CodeGen.HeadersTemplate _
            CodeGen.Headers &= (IIf(MyPublic, vbCrLf & "extern " & CodeGen.ConstructorTemplate.Trim, "") & vbCrLf & _
                               CodeGen.HeadersTemplate) _
            .Replace("[CONTROLID]", MyControlId) _
            .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
            .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
            .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
            .Replace("[NEXT_NUMID]", CodeGen.NumId)

            CodeGen.EventsToCode(MyControlId, CType(Me, IVGDDWidget))

        End Sub
#Else
        Public Sub GetCode(ByVal ControlIdPrefix As String) Implements IVGDDWidget.GetCode
        End Sub
#End If

        Public Property Scheme As String Implements Common.IVGDDWidget.Scheme
            Get
                Return Me.Item("Scheme")
            End Get
            Set(ByVal value As String)
                Me.Item("Scheme") = value
            End Set
        End Property

        Private _IsLoading As Boolean
        Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
            _IsLoading = True
        End Sub

        Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
            _IsLoading = False
            CheckEval("Left,Right,Top,Bottom")
        End Sub
    End Class

    Public Class VGDDCustomProp
        Private _PropName As String
        Private _PropStrType As String
        Private _PropType As Type
        Private _PropDescr As String
        Private _PropCategory As String
        Private _Eval As String
        Private _DefaultValue As String
        Private _DestProperty As String

        <Description("Name for this property")> _
        <ParenthesizePropertyName(True)> _
        Public ReadOnly Property Name As String
            Get
                Return _PropName
            End Get
        End Property

        <EditorBrowsable(True), Browsable(False)> _
        Public Property PropertyName As String
            Get
                Return _PropName
            End Get
            Set(ByVal value As String)
                _PropName = value
            End Set
        End Property

        <Description("Formula to be calculated for this property" & vbCrLf & "i.e.: for the ""Bottom"" property the eval sould be ""Top + Height""")> _
        Public Property Eval As String
            Get
                Return _Eval
            End Get
            Set(ByVal value As String)
                _Eval = value
            End Set
        End Property

        '<Editor(GetType(VGDDEventsEditor), GetType(System.Drawing.Design.UITypeEditor))> _
        <Description("Data Type for this property")> _
        <DisplayName("Type")> _
        <Editor(GetType(VGDDComboUITypeEditor), GetType(UITypeEditor))> _
        Public Property StrType As String
            Get
                Return _PropStrType
            End Get
            Set(ByVal value As String)
                _PropStrType = value
                If value Is Nothing Then Exit Property
                Select Case value.ToUpper
                    Case "INT"
                        Me.Type = GetType(System.Int32)
                        If Defaultvalue IsNot Nothing Then
                            If Defaultvalue.ToString = String.Empty Then
                                Defaultvalue = 0
                            Else
                                Integer.TryParse(Defaultvalue, Me.Defaultvalue)
                            End If
                        End If
                    Case "FLOAT"
                        Me.Type = GetType(System.Single)
                        If Defaultvalue IsNot Nothing Then
                            Single.TryParse(Defaultvalue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, Me.Defaultvalue)
                        End If
                    Case "BOOL"
                        Me.Type = GetType(System.Boolean)
                        If Defaultvalue IsNot Nothing AndAlso Defaultvalue <> "" Then
                            Try
                                Boolean.TryParse(Defaultvalue, Me.Defaultvalue)
                            Catch ex As Exception
                            End Try
                        End If
                    Case "HORIZALIGN"
                        Me.Type = GetType(HorizAlign)
                        If Defaultvalue IsNot Nothing Then
                            Me.Defaultvalue = Defaultvalue
                        Else
                            Me.Defaultvalue = Common.HorizAlign.Left
                        End If
                        'Case "STATE"
                        '    oCustProp.Type = GetType(Common.State)
                        '    If DefaultValue IsNot Nothing Then
                        '        Integer.TryParse(DefaultValue, oCustProp.DefaultValue)
                        '    End If
                    Case Else
                        Me.Type = GetType(System.String)
                        If Defaultvalue IsNot Nothing Then
                            Me.Defaultvalue = Defaultvalue.ToString
                        End If
                End Select
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Property Type As Type
            Get
                Return _PropType
            End Get
            Set(ByVal value As Type)
                _PropType = value
            End Set
        End Property

        <Description("Description of this Property")> _
        Public Property Description As String
            Get
                Return _PropDescr
            End Get
            Set(ByVal value As String)
                _PropDescr = value
            End Set
        End Property

        <Description("Category for this Property")> _
        <Editor(GetType(VGDDComboUITypeEditor), GetType(UITypeEditor))> _
        Public Property Category As String
            Get
                Return _PropCategory
            End Get
            Set(ByVal value As String)
                _PropCategory = value
            End Set
        End Property

        <Description("Default value for this Property")> _
        Public Property Defaultvalue As String
            Get
                Return _DefaultValue
            End Get
            Set(ByVal value As String)
                _DefaultValue = value
            End Set
        End Property

        <Description("Effective destination Property" & vbCrLf & "If this property should be used to determine the value of another property, fill this field with the name of that property." & vbCrLf & "i.e.: the STATE property is the destination for both ""Hidden"" and ""Disabled"" base properties")> _
        Public Property DestProperty As String
            Get
                Return _DestProperty
            End Get
            Set(ByVal value As String)
                _DestProperty = value
            End Set
        End Property
    End Class

    Public Class VGDDCustomPropBag
        Private _ToolboxBitmap As Image
        Private _Bitmap As Image
        Public CustomPropsCollection As New System.Collections.Hashtable '.Specialized.OrderedDictionary

        Public Sub SetProperty(ByVal PropertyName As String, ByVal Prop As VGDDCustomProp)
            If CustomPropsCollection.Contains(PropertyName) Then
                CustomPropsCollection(PropertyName) = Prop
            Else
                CustomPropsCollection.Add(PropertyName, Prop)
            End If
        End Sub

        Public Function GetProperty(ByVal PropertyName As String) As VGDDCustomProp
            If CustomPropsCollection.Contains(PropertyName) Then
                Return CustomPropsCollection(PropertyName)
            Else
                Return Nothing
            End If
        End Function

        Public Property ToolboxBitmap As Image
            Get
                Return _ToolboxBitmap
            End Get
            Set(ByVal value As Image)
                _ToolboxBitmap = value
            End Set
        End Property

        Public Property Bitmap As Image
            Get
                Return _Bitmap
            End Get
            Set(ByVal value As Image)
                _Bitmap = value
            End Set
        End Property
    End Class

    Public Class VGDDCustomTypeDescriptionProvider
        Inherits TypeDescriptionProvider
        Private Shared _defaultTypeProvider As TypeDescriptionProvider = TypeDescriptor.GetProvider(GetType(VGDDCustom))

        Public Sub New()
            MyBase.New(_defaultTypeProvider)
        End Sub

        Public Overrides Function GetTypeDescriptor(ByVal objectType As System.Type, ByVal instance As Object) As ICustomTypeDescriptor
            Dim defaultDescriptor As ICustomTypeDescriptor = MyBase.GetTypeDescriptor(objectType, instance)

            If instance IsNot Nothing Then
                Return New VGDDCustomTypeDescriptor(defaultDescriptor, instance)
            Else
                Return defaultDescriptor
            End If
        End Function
    End Class

    Public Class VGDDCustomTypeDescriptor
        Inherits CustomTypeDescriptor

        Private _instance As VGDDCustom

        Public Sub New(ByVal parent As ICustomTypeDescriptor, ByVal instance As Object)
            MyBase.New(parent)
            If VGDDCustom.XmlCustomTemplatesDoc Is Nothing Then Exit Sub
            _instance = instance
            If _instance.CustomWidgetType Is Nothing Then Exit Sub
            Dim oPropBag As VGDDCustomPropBag
            If VGDDCustom.UsedCustomProps.ContainsKey(_instance.CustomWidgetType) Then
                oPropBag = VGDDCustom.UsedCustomProps(_instance.CustomWidgetType)
            Else
                oPropBag = New VGDDCustomPropBag
                If VGDDCustom.XmlCustomTemplatesDoc Is Nothing Then
                    VGDDCustom.LoadCustomTemplatesDoc()
                End If
                Dim oNodeDef As XmlNode = VGDDCustom.XmlCustomTemplatesDoc.SelectSingleNode(String.Format("VGDDCustomWidgetsTemplate/{0}/Definition", _instance.CustomWidgetType))
                If oNodeDef IsNot Nothing Then
                    For Each oNode As XmlNode In oNodeDef.ChildNodes
                        Select Case oNode.Name
                            Case "Bitmap"
                                Dim strFileName As String = Path.Combine(VGDDCustom.CustomWidgetsFolder, oNode.Attributes("FileName").Value.ToString)
                                If Not File.Exists(strFileName) Then
                                    strFileName = Path.Combine(Common.UserTemplatesFolder, oNode.Attributes("FileName").Value.ToString)
                                End If
                                If File.Exists(strFileName) Then
                                    oPropBag.Bitmap = New Bitmap(strFileName)
                                End If
                            Case "ToolboxBitmap"
                                Dim strFileName As String = Path.Combine(VGDDCustom.CustomWidgetsFolder, oNode.Attributes("FileName").Value.ToString)
                                If Not File.Exists(strFileName) Then
                                    strFileName = Path.Combine(Common.UserTemplatesFolder, oNode.Attributes("FileName").Value.ToString)
                                End If
                                If File.Exists(strFileName) Then
                                    oPropBag.ToolboxBitmap = New Bitmap(strFileName)
                                End If
                            Case "Property"
                                Dim oCustProp As VGDDCustomProp = CustomPropFromXml(oNode)
                                oPropBag.SetProperty(oCustProp.Name.ToUpper, oCustProp)
                                If _instance.PROPSTOREMOVECUSTOM.Contains(" " & oCustProp.Name & " ") Then
                                    _instance.PROPSTOREMOVECUSTOM = _instance.PROPSTOREMOVECUSTOM.Replace(" " & oCustProp.Name & " ", " ")
                                End If
                        End Select
                    Next
                    If oPropBag.ToolboxBitmap Is Nothing AndAlso oPropBag.Bitmap IsNot Nothing Then
                        Dim b As New Bitmap(72, 72)
                        Dim g As Graphics = Graphics.FromImage(b)
                        g.DrawImage(oPropBag.Bitmap, 0, 0, b.Width, b.Height)
                        g.Dispose()
                        oPropBag.ToolboxBitmap = b
                    End If
                    VGDDCustom.UsedCustomProps.Add(_instance.CustomWidgetType, oPropBag)
                End If
            End If
            _instance.Image = oPropBag.Bitmap
            If _instance._CustomProperties Is Nothing Then
                _instance._CustomProperties = New Hashtable
                For Each oProp As VGDDCustomProp In oPropBag.CustomPropsCollection.Values
                    _instance._CustomProperties.Add(oProp.Name.ToUpper, oProp.Defaultvalue)
                Next
            End If
        End Sub

        Public Shared Function CustomPropFromXml(ByVal oNode As XmlNode) As VGDDCustomProp
            Dim oCustProp As New VGDDCustomProp
            Dim DefaultValue As Object = Nothing
            Try
                oCustProp.PropertyName = oNode.Attributes("Name").Value.ToString
                oCustProp.Description = oNode.Attributes("Description").Value.ToString
                If oNode.Attributes("Category") IsNot Nothing Then
                    oCustProp.Category = oNode.Attributes("Category").Value.ToString
                End If
                If oNode.Attributes("DestProperty") IsNot Nothing Then
                    oCustProp.DestProperty = oNode.Attributes("DestProperty").Value.ToString
                End If
                If oNode.Attributes("Eval") IsNot Nothing Then
                    oCustProp.Eval = oNode.Attributes("Eval").Value.ToString
                End If
                If oNode.Attributes("Defaultvalue") IsNot Nothing Then
                    oCustProp.Defaultvalue = oNode.Attributes("Defaultvalue").Value
                End If
                If oNode.Attributes("Type") IsNot Nothing Then
                    oCustProp.StrType = oNode.Attributes("Type").Value
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error reading property value from XML")
            End Try
            Return oCustProp
        End Function

        Public Function GetCustomPropertyBag(ByVal CustomType As String) As VGDDCustomPropBag
            If CustomType IsNot Nothing Then
                If VGDDCustom.UsedCustomProps.ContainsKey(CustomType) Then
                    Return VGDDCustom.UsedCustomProps(CustomType)
                End If
            End If
            Return Nothing
        End Function

        Public Overrides Function GetProperties(ByVal attributes() As System.Attribute) As System.ComponentModel.PropertyDescriptorCollection
            Dim f As VGDDCustom
            Dim propArr() As PropertyDescriptor
            Dim propCollection As PropertyDescriptorCollection
            ReDim propArr(MyBase.GetProperties.Count - 1)
            MyBase.GetProperties().CopyTo(propArr, 0)
            propCollection = New PropertyDescriptorCollection(propArr)
            With (propCollection)
                If TypeOf (_instance) Is VGDDCustom AndAlso _instance.Type IsNot Nothing Then
                    f = DirectCast(_instance, VGDDCustom)
                    If VGDDCustom.UsedCustomProps.ContainsKey(_instance.Type) Then
                        Dim oCustomPropBag As VGDDCustomPropBag = VGDDCustom.UsedCustomProps(_instance.Type)
                        For Each oProp As VGDDCustomProp In oCustomPropBag.CustomPropsCollection.Values
                            Dim oPropDesc As PropertyDescriptor = New VGDDCustomPropertyDescriptor(oProp, attributes)
                            .Add(oPropDesc)
                        Next
                    End If
                End If
            End With
            Return propCollection
        End Function

        Public Overrides Function GetProperties() As System.ComponentModel.PropertyDescriptorCollection
            Return GetProperties(Nothing)
        End Function
    End Class

    Public Class VGDDCustomPropertyDescriptor
        Inherits PropertyDescriptor

        Private _name As String
        Private _type As Type
        Private _Description As String
        Private _Category As String
        Private _DefaultValue As Object

        Public Overrides Function GetEditor(ByVal editorBaseType As System.Type) As Object
            Select Case Me._name
                Case "Bitmap"
                    Return New VGDDBitmapFileChooser
                Case Else
                    Return MyBase.GetEditor(editorBaseType)
            End Select
        End Function

        Public Sub New(ByVal oProp As VGDDCustomProp, ByVal attributesArr() As Attribute)
            MyBase.New(oProp.Name, attributesArr)
            _name = oProp.Name
            _type = oProp.Type
            _Description = oProp.Description
            _Category = oProp.Category
            _DefaultValue = oProp.Defaultvalue
        End Sub

        Public Overrides ReadOnly Property Category() As String
            Get
                Return _Category
            End Get
        End Property

        Public Overrides ReadOnly Property Description() As String
            Get
                Return _Description
            End Get
        End Property

        Public Overrides Function CanResetValue(ByVal component As Object) As Boolean
            Return False
        End Function

        Public Overrides ReadOnly Property ComponentType() As System.Type
            Get
                Return GetType(VGDDCustom)
            End Get
        End Property

        Public Overrides Function GetValue(ByVal component As Object) As Object
            Dim f As VGDDCustom
            f = DirectCast(component, VGDDCustom)
            If f(_name) IsNot Nothing Then
                Return f(_name)
            Else
                Return _DefaultValue
            End If
        End Function

        Public Overrides ReadOnly Property IsReadOnly() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property PropertyType() As System.Type
            Get
                Return _type
            End Get
        End Property

        Public Overrides Sub ResetValue(ByVal component As Object)
            Throw New NotImplementedException("This is not supported by me")
        End Sub

        Public Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
            Dim f As VGDDCustom
            f = DirectCast(component, VGDDCustom)
            f.Item(_name) = value
        End Sub

        Public Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
            Return True
        End Function
    End Class
End Namespace

