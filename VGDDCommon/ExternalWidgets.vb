Imports System.Reflection
Imports System.Xml
Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing

Namespace VGDDCommon

    Public Interface IVGDDLicenseHandler
        ReadOnly Property IsLicensed As Boolean
    End Interface

    Public Class ExternalWidgetsHandler

        Public Shared ExternalWidgets As New Dictionary(Of String, ExternalWidget)
        Public Shared CollectionXmlDoc As New XmlDocument
        Public Shared CollectionXmlPath As String _
            = Path.Combine(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Application.CommonAppDataPath)), "VGDD"), "ExternalWidgets.xml")

        Public Event Changed()

        Public Function UnLoadAssembly(ByVal AssemblyName As String) As Boolean
            If ExternalWidgets.ContainsKey(AssemblyName) Then
                ExternalWidgets.Remove(AssemblyName)
                Dim oNode As XmlNode = CollectionXmlDoc.DocumentElement.SelectSingleNode(String.Format("Assembly[@Name='{0}']", AssemblyName))
                CollectionXmlDoc.DocumentElement.RemoveChild(oNode)
                CollectionXmlDoc.Save(CollectionXmlPath)
                Return True
            End If
            Return False
        End Function

        Public Function LoadAssembly(ByVal strFileName As String) As Boolean
            Dim blnOk As Boolean = False
            Dim NewAssembly As Assembly
            If Not File.Exists(strFileName) Then Return False
            Try
                NewAssembly = Assembly.LoadFile(strFileName)
                Return LoadAssembly(NewAssembly)
            Catch ex As Exception
                MessageBox.Show("Error loading Assembly " & strFileName & ". It must be a Dot.Net VGDDWidgets Library", "Error")
                Return False
            End Try
        End Function

        Public Function LoadAssembly(ByVal NewAssembly As Assembly) As Boolean
            Dim blnOk As Boolean = False
            Try
                For Each oType As Type In NewAssembly.GetTypes()
                    If oType.GetMethod("GetCode") IsNot Nothing Then
                        blnOk = True
                        Exit For
                    End If
                Next
            Catch ex As Exception
                MessageBox.Show("Error loading Assembly " & NewAssembly.FullName & ". It must be a Dot.Net VGDDWidgets Library", "Error")
                Return False
            End Try
            If Not blnOk Then
                MessageBox.Show(NewAssembly.FullName & " does not appear to be a valid VGDD Widgets Library", "Unknown Assembly")
                Return False
            End If
            AppDomain.CurrentDomain.DefineDynamicAssembly(NewAssembly.GetName, Emit.AssemblyBuilderAccess.Run)
            Dim strAssemblyName As String = NewAssembly.GetName.Name
            If Not ExternalWidgets.ContainsKey(strAssemblyName) Then
                Dim oNewExtWidget As New ExternalWidget
                With oNewExtWidget
                    .Name = NewAssembly.FullName 'Path.GetFileNameWithoutExtension(strFileName)
                    .Assembly = NewAssembly
                    '.AssemblyFileName = strFileName
                    .Author = NewAssembly.GetCustomAttributes(GetType(System.Reflection.AssemblyCompanyAttribute), False)(0).Company.ToString
                    .Loaded = True

                    Dim il As New ImageList
                    il.ImageSize = New Size(128, 128)
                    For Each strResourceName As String In NewAssembly.GetManifestResourceNames
                        If strResourceName.EndsWith(".bmp") _
                        Or strResourceName.EndsWith(".jpg") _
                        Or strResourceName.EndsWith(".png") _
                            Then
                            Dim bNew As New Bitmap(NewAssembly.GetManifestResourceStream(strResourceName))
                            Dim bList As New Bitmap(il.ImageSize.Width, il.ImageSize.Height)
                            Dim g As Graphics = Graphics.FromImage(bList)
                            If bNew.Width > bNew.Height Then
                                g.DrawImage(bNew, 0, 0, bList.Width, bList.Height * bNew.Height \ bNew.Width)
                            Else
                                g.DrawImage(bNew, (bNew.Height - bNew.Width) \ 2, 0, bList.Width * bNew.Width \ bNew.Height, bList.Height)
                            End If
                            'Dim bDest As New Bitmap(.ImageList.ImageSize.Width, .ImageList.ImageSize.Width )
                            'Dim g As Graphics = Graphics.FromImage(bDest)
                            il.Images.Add(bList)
                            g.Dispose()
                        End If
                    Next
                    .ImageList = il
                End With

                ExternalWidgets.Add(strAssemblyName, oNewExtWidget)

                Dim oNode As XmlNode = CollectionXmlDoc.DocumentElement.SelectSingleNode(String.Format("Assembly[@Name='{0}']", oNewExtWidget.Name))
                Dim oNewNode = oNewExtWidget.ToXml(CollectionXmlDoc)
                If oNode Is Nothing Then
                    CollectionXmlDoc.DocumentElement.AppendChild(oNewNode)
                Else
                    oNode = oNewNode
                End If
            End If
            Return True
        End Function

        Private Sub ExternalWidgetsUnload(ByVal sender As Object, ByVal e As EventArgs)
            Dim oMenuItem As MenuItem = sender
            Dim strAssemblyName As String = oMenuItem.Text
            If MessageBox.Show("Do you want to unload the " & strAssemblyName & " External Widgets Library?", "Unload Library", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                Dim oNode As XmlNode = CollectionXmlDoc.DocumentElement.SelectSingleNode(String.Format("Assembly[@AssemblyName='{0}']", strAssemblyName))
                If oNode IsNot Nothing Then
                    CollectionXmlDoc.DocumentElement.RemoveChild(oNode)
                    CollectionXmlDoc.Save(CollectionXmlPath)
                    ExternalWidgets.Remove(strAssemblyName)
                End If
                LoadAll()
                If ExternalWidgets.Count = 0 Then
                    RaiseEvent Changed()
                End If
                oMenuItem.Parent.MenuItems.Remove(oMenuItem)
            End If
        End Sub

        Public Sub SaveAll()
            Dim blnChanged As Boolean = False
            For Each oExtWidget As ExternalWidget In ExternalWidgets.Values
                Dim oNode As XmlNode = CollectionXmlDoc.DocumentElement.SelectSingleNode(String.Format("Assembly[@Name='{0}']", oExtWidget.Name))
                Dim oNewNode = oExtWidget.ToXml(CollectionXmlDoc)
                If oNode Is Nothing Then
                    CollectionXmlDoc.DocumentElement.AppendChild(oNewNode)
                    blnChanged = True
                Else
                    oNode.ParentNode.ReplaceChild(oNewNode, oNode)
                End If
            Next
            CollectionXmlDoc.Save(CollectionXmlPath)
            If blnChanged Then
                RaiseEvent Changed()
            End If
        End Sub

        Public Sub LoadAll()
            If File.Exists(CollectionXmlPath) Then
                CollectionXmlDoc.Load(CollectionXmlPath)
            End If
            If (CollectionXmlDoc.DocumentElement) Is Nothing Then
                Dim oRootNode As XmlNode = CollectionXmlDoc.CreateElement("ExternalWidgetsAssemblies")
                CollectionXmlDoc.AppendChild(oRootNode)
            End If
            For Each oNode As XmlNode In CollectionXmlDoc.DocumentElement.ChildNodes
                If oNode.Attributes("FileName") IsNot Nothing Then
                    Dim strAssemblyname As String = oNode.Attributes("FileName").Value
                    If LoadAssembly(strAssemblyname) Then
                        Dim oXW As ExternalWidget = ExternalWidgets(oNode.Attributes("Name").Value)
                        'If oNode.Attributes("AuthCode") IsNot Nothing Then
                        '    oXW.SetAuthCode(oNode.Attributes("AuthCode").Value)
                        'End If
                    End If
                End If
            Next
            If ExternalWidgets.Count > 0 Then
                VGDDCommon.CodeGen.LoadCodeGenTemplates()
            End If
            RaiseEvent Changed()
        End Sub
    End Class

    Public Class ExternalWidget
        Private _Name As String
        Private _AssemblyFileName As String
        Private _Assembly As Assembly
        Private _Image As ImageList
        Private _Loaded As Boolean
        Private _Author As String

        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        Public Property AssemblyFileName As String
            Get
                Return _AssemblyFileName
            End Get
            Set(ByVal value As String)
                _AssemblyFileName = value
            End Set
        End Property

        Public Property Assembly As Assembly
            Get
                Return _Assembly
            End Get
            Set(ByVal value As Assembly)
                _Assembly = value
            End Set
        End Property

        Public ReadOnly Property IsLicensed As Boolean
            Get
                Dim LicenseHandlerType As Type = _Assembly.GetType("LicenseHandler")
                If LicenseHandlerType IsNot Nothing Then
                    Dim Ilh As Type = LicenseHandlerType.GetInterface("IVGDDLicenseHandler")
                    If Ilh IsNot Nothing Then
                        Dim instance As IVGDDLicenseHandler = Activator.CreateInstance(LicenseHandlerType)
                        Return instance.IsLicensed
                    End If
                End If
                Return True
            End Get
        End Property

        Public Property ImageList As ImageList
            Get
                Return _Image
            End Get
            Set(ByVal value As ImageList)
                _Image = value
            End Set
        End Property

        Public Property Loaded As Boolean
            Get
                Return _Loaded
            End Get
            Set(ByVal value As Boolean)
                _Loaded = value
            End Set
        End Property

        Public Property Author As String
            Get
                Return _Author
            End Get
            Set(ByVal value As String)
                _Author = value
            End Set
        End Property

        Public Function ToXml(ByVal OwnerDoc As XmlDocument) As XmlNode
            Dim oNode As XmlNode = OwnerDoc.CreateElement("Assembly")
            Dim oAttr As XmlAttribute

            oAttr = OwnerDoc.CreateAttribute("Name")
            oAttr.Value = Me.Name
            oNode.Attributes.Append(oAttr)

            'oAttr = OwnerDoc.CreateAttribute("AssemblyName")
            'oAttr.Value = Me.Assembly.GetName.Name
            'oNode.Attributes.Append(oAttr)

            oAttr = OwnerDoc.CreateAttribute("FileName")
            oAttr.Value = Me.AssemblyFileName
            oNode.Attributes.Append(oAttr)

            'oAttr = OwnerDoc.CreateAttribute("AuthCode")
            'oAttr.Value = Me._AuthCode
            'oNode.Attributes.Append(oAttr)

            Return oNode
        End Function

    End Class

End Namespace