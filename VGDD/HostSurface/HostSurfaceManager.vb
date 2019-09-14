Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Data
Imports System.Windows.Forms
Imports System.IO

Public Enum LoaderType
    BasicDesignerLoader = 1
    CodeDomDesignerLoader = 2
    NoLoader = 3
End Enum

Public Class HostSurfaceManager
    Inherits DesignSurfaceManager

    Public Shared ShowGrid As Boolean = False
    Public Shared SnapToGrid As Boolean = False
    Public Shared GridSize As Integer = 10
    Public Shared UseSnapLines As Boolean = True
    'Private Shared MyDesignerOptionService As New MyDesignerOptionService

    Public Sub New()
        MyBase.New()
        'If VGDDHostSurface.DesignerOptionService Is Nothing Then
        '    VGDDHostSurface.DesignerOptionService = New MyDesignerOptionService
        'End If
        Me.ServiceContainer.AddService(GetType(INameCreationService), New MyNameCreationService)
        'AddHandler ActiveDesignSurfaceChanged, AddressOf Me.HostSurfaceManager_ActiveDesignSurfaceChanged
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            Me.RemoveService(GetType(INameCreationService))
            If disposing AndAlso Me.DesignSurfaces.Count > 0 Then
                For i As Integer = 0 To Me.DesignSurfaces.Count - 1 > 0
                    Me.DesignSurfaces(i).Dispose()
                Next
            End If
        Catch ex As Exception
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Public Sub ActivateDesignerOptions()
        For Each ds As VGDDHostSurface In Me.DesignSurfaces
            Dim dsos As IDesignerOptionService = ds.GetService(GetType(DesignerOptionService))
            If dsos IsNot Nothing Then
                dsos.SetOptionValue("WindowsFormsDesigner\General", "ShowGrid", ShowGrid)
                dsos.SetOptionValue("WindowsFormsDesigner\General", "UseSnapLines", UseSnapLines)
                dsos.SetOptionValue("WindowsFormsDesigner\General", "SnapToGrid", SnapToGrid)
                dsos.SetOptionValue("WindowsFormsDesigner\General", "GridSize", New Size(GridSize, GridSize))
                ds.Initialize()
            End If
            'If dsos Is Nothing Then
            '    Me.ServiceContainer.RemoveService(GetType(DesignerOptionService), True)
            'Else
            '    Dim options As DesignerOptionService.DesignerOptionCollection = dsos.Options
            '    options.Properties.Find("", True).SetValue(options, UseSnapLines)
            '    options.Properties.Find("", True).SetValue(options, SnapToGrid)
            '    options.Properties.Find("ShowGrid", True).SetValue(options, ShowGrid)
            '    options.Properties.Find("", True).SetValue(options, )
            'End If
        Next
    End Sub

    Protected Overrides Function CreateDesignSurfaceCore(ByVal parentProvider As IServiceProvider) As DesignSurface
        Return New VGDDHostSurface(parentProvider)
    End Function

    ' <summary>
    ' Gets a new HostSurface and loads it with the appropriate type of
    ' root component. 
    ' </summary>
    Private Overloads Function GetNewHost(ByVal rootComponentType As Type) As HostControl
        Dim hostSurface As VGDDHostSurface = CType(Me.CreateDesignSurface(Me.ServiceContainer), VGDDHostSurface)
        'If (rootComponentType Is GetType(VGDD.VgddScreen)) Then
        hostSurface.BeginLoad(rootComponentType)
        'ElseIf (rootComponentType Is GetType(UserControl)) Then
        '    hostSurface.BeginLoad(GetType(UserControl))
        'ElseIf (rootComponentType Is GetType(Component)) Then
        '    hostSurface.BeginLoad(GetType(Component))
        'Else
        '    Throw New Exception(("Undefined Host Type: " + rootComponentType.ToString))
        'End If
        ActivateDesignerOptions()
        hostSurface.Initialize()
        Me.ActiveDesignSurface = hostSurface
        Return New HostControl(hostSurface)
    End Function

    ' <summary>
    ' Gets a new HostSurface and loads it with the appropriate type of
    ' root component. Uses the appropriate Loader to load the HostSurface.
    ' </summary>
    Public Overloads Function GetNewHost(ByVal rootComponentType As Type, ByVal loaderType As LoaderType) As HostControl
        If (loaderType = loaderType.NoLoader) Then
            Return GetNewHost(rootComponentType)
        End If
        Dim hostSurface As VGDDHostSurface = CType(Me.CreateDesignSurface(Me.ServiceContainer), VGDDHostSurface)
        Dim host As IDesignerHost = CType(hostSurface.GetService(GetType(IDesignerHost)), IDesignerHost)
        Select Case (loaderType)
            Case loaderType.BasicDesignerLoader
                Dim basicHostLoader As BasicHostLoader = New BasicHostLoader(rootComponentType)
                hostSurface.BeginLoad(basicHostLoader)
                hostSurface.Loader = basicHostLoader
                'Case loaderType.CodeDomDesignerLoader
                '    Dim codeDomHostLoader As CodeDomHostLoader = New CodeDomHostLoader
                '    hostSurface.BeginLoad(codeDomHostLoader)
                '    hostSurface.Loader = codeDomHostLoader
            Case Else
                Throw New Exception(("Loader is not defined: " + loaderType.ToString))
        End Select
        hostSurface.Initialize()
        ActivateDesignerOptions()
        Return New HostControl(hostSurface)
    End Function

    ' <summary>
    ' Opens an Xml file and loads it up using BasicHostLoader (inherits from
    ' BasicDesignerLoader)
    ' </summary>
    Public Overloads Function GetNewHost(ByVal fileName As String) As HostControl
        If ((fileName Is Nothing) OrElse Not File.Exists(fileName)) Then
            MessageBox.Show("FileName is incorrect: """ + fileName & """", "HostSurface Manager Error in GetNewHost")
            Return Nothing
        End If
        Dim loaderType As LoaderType = loaderType.BasicDesignerLoader
        Dim hostSurface As VGDDHostSurface = CType(Me.CreateDesignSurface(Me.ServiceContainer), VGDDHostSurface)
        Dim host As IDesignerHost = CType(hostSurface.GetService(GetType(IDesignerHost)), IDesignerHost)
        Dim basicHostLoader As BasicHostLoader
        Try
            basicHostLoader = New BasicHostLoader(fileName)
        Catch exDemo As SystemException
            Throw New SystemException(exDemo.Message)
            Exit Function
        End Try
        hostSurface.BeginLoad(BasicHostLoader)
        hostSurface.Loader = BasicHostLoader
        hostSurface.Initialize()
        ActivateDesignerOptions()
        GetNewHost = New HostControl(hostSurface)
        GetNewHost.Name = fileName
    End Function

    Public Overloads Function GetNewHost(ByRef XmlNode As Xml.XmlNode, ByVal ComponentType As System.Type) As HostControl
        Dim loaderType As LoaderType = loaderType.BasicDesignerLoader
        Dim hostSurface As VGDDHostSurface = CType(Me.CreateDesignSurface(Me.ServiceContainer), VGDDHostSurface)
        Dim host As IDesignerHost = CType(hostSurface.GetService(GetType(IDesignerHost)), IDesignerHost)
        Dim basicHostLoader As BasicHostLoader = New BasicHostLoader(XmlNode, ComponentType)
        hostSurface.BeginLoad(basicHostLoader)
        hostSurface.Loader = basicHostLoader
        ActivateDesignerOptions()
        hostSurface.Initialize()
        GetNewHost = New HostControl(hostSurface)
        If XmlNode IsNot Nothing Then GetNewHost.Name = XmlNode.Attributes("Name").Value
    End Function

    Public Sub AddService(ByVal type As Type, ByVal serviceInstance As Object)
        Me.ServiceContainer.RemoveService(type, True)
        Me.ServiceContainer.AddService(type, serviceInstance)
    End Sub

    Public Sub RemoveService(ByVal type As Type)
        Me.ServiceContainer.RemoveService(type)
    End Sub

    Private Sub HostSurfaceManager_ActiveDesignSurfaceChanged(ByVal sender As Object, ByVal e As System.ComponentModel.Design.ActiveDesignSurfaceChangedEventArgs) Handles Me.ActiveDesignSurfaceChanged
        Dim hostSurface As VGDDHostSurface = e.NewSurface
        If hostSurface IsNot Nothing Then
            ActivateDesignerOptions()
            hostSurface.Initialize()
        End If
    End Sub

    'Private Sub HostSurfaceManager_ActiveDesignSurfaceChanged(ByVal sender As Object, ByVal e As System.ComponentModel.Design.ActiveDesignSurfaceChangedEventArgs) Handles Me.ActiveDesignSurfaceChanged
    '    Debug.Print("?")
    'End Sub

    ' <summary>
    ' Uses the OutputWindow service and writes out to it.
    ' </summary>
    'Private Sub HostSurfaceManager_ActiveDesignSurfaceChanged(ByVal sender As Object, ByVal e As ActiveDesignSurfaceChangedEventArgs)
    '    Dim o As OutputWindow = CType(Me.GetService(GetType(OutputWindow)), OutputWindow)
    '    o.RichTextBox.Text = o.RichTextBox.Text + "New host added." & vbLf
    'End Sub

    'Private Sub HostSurfaceManager_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SelectionChanged
    '    'Debug.Print("HostSurfaceManager_SelectionChanged")
    'End Sub
End Class

#Region "Snapline stuff"
Public Class MyDesignerOptionService
    Inherits DesignerOptionService

    Private _MySurface As VGDDHostSurface

    Public Sub New(ByVal ds As VGDDHostSurface)
        _MySurface = ds
    End Sub

    Protected Overrides Sub PopulateOptionCollection(ByVal options As DesignerOptionCollection)
        If options.Parent Is Nothing Then
            Dim opt As Windows.Forms.Design.DesignerOptions = New Windows.Forms.Design.DesignerOptions()
            opt.UseSnapLines = HostSurfaceManager.UseSnapLines
            opt.ShowGrid = HostSurfaceManager.ShowGrid
            opt.SnapToGrid = HostSurfaceManager.SnapToGrid
            opt.GridSize = New Size(HostSurfaceManager.GridSize, HostSurfaceManager.GridSize)
            'opt.ObjectBoundSmartTagAutoShow = True
            'opt.UseSmartTags = True
            Dim wfdc As DesignerOptionCollection = Me.CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            Dim wfdgc As DesignerOptionCollection = Me.CreateOptionCollection(wfdc, "General", opt)
        End If
    End Sub
End Class
#End Region

'End Namespace
