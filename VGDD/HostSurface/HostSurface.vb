Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.ComponentModel.Design.DesignerOptionService

Public Class VGDDHostSurface
    Inherits DesignSurface

    Private _IsDisposed As Boolean = False
    Private _loader As BasicHostLoader
    Private _selectionService As ISelectionService

    Private _undoEngine As MyUndoEngine = Nothing
    Private _nameCreationService As MyNameCreationService = Nothing
    Private _designerSerializationService As MyDesignerSerializationService = Nothing
    Private _codeDomComponentSerializationService As CodeDomComponentSerializationService = Nothing
    Private _MenuCommandService As MenuCommandService = Nothing
    Public Shared DesignerOptionService As DesignerOptionService = Nothing
    Private _DesignerFilterService As MyDesignerFilterService

    Public Sub New()
        MyBase.New()
        Me.ServiceContainer.AddService(GetType(IMenuCommandService), New MenuCommandService(Me))
    End Sub

    Public Sub New(ByVal parentProvider As IServiceProvider)
        MyBase.New(parentProvider)

        _nameCreationService = New MyNameCreationService
        Me.ServiceContainer.RemoveService(GetType(INameCreationService), False)
        Me.ServiceContainer.AddService(GetType(INameCreationService), _nameCreationService)


        _codeDomComponentSerializationService = New CodeDomComponentSerializationService(ServiceContainer)
        Me.ServiceContainer.RemoveService(GetType(ComponentSerializationService), False)
        Me.ServiceContainer.AddService(GetType(ComponentSerializationService), _codeDomComponentSerializationService)

        _designerSerializationService = New MyDesignerSerializationService(ServiceContainer)
        Me.ServiceContainer.RemoveService(GetType(IDesignerSerializationService), False)
        Me.ServiceContainer.AddService(GetType(IDesignerSerializationService), _designerSerializationService)

        _undoEngine = New MyUndoEngine(ServiceContainer)
        _undoEngine.Enabled = False
        Me.ServiceContainer.AddService(GetType(UndoEngine), _undoEngine)

        _MenuCommandService = New MenuCommandService(Me)
        Me.ServiceContainer.RemoveService(GetType(IMenuCommandService), False)
        Me.ServiceContainer.AddService(GetType(IMenuCommandService), _MenuCommandService)

        ''Me.AddService(GetType(INameCreationService), New NameCreationService)
        ''Me.AddService(GetType(IMenuCommandService), New MenuCommandService(Me))
        ''Me.AddService(GetType(ComponentSerializationService), New CodeDomComponentSerializationService(ServiceContainer))
        ''Me.AddService(GetType(IDesignerSerializationService), New DesignerSerializationService(ServiceContainer))
        'Dim host As IDesignerHost = CType(Me.GetService(GetType(IDesignerHost)), IDesignerHost)
        'Me.AddService(GetType(UndoEngine), New MyUndoEngine(host))

        DesignerOptionService = New MyDesignerOptionService(Me) 'MyDesignerOptionService
        Me.ServiceContainer.RemoveService(GetType(DesignerOptionService), False)
        Me.ServiceContainer.AddService(GetType(DesignerOptionService), DesignerOptionService)

        Dim oldService As ITypeDescriptorFilterService = DirectCast(Me.GetService(GetType(ITypeDescriptorFilterService)), ITypeDescriptorFilterService)
        _DesignerFilterService = New MyDesignerFilterService(oldService)
        Me.ServiceContainer.RemoveService(GetType(ITypeDescriptorFilterService))
        Me.ServiceContainer.AddService(GetType(ITypeDescriptorFilterService), _DesignerFilterService)

    End Sub

    'Public Sub xxx()
    '    Me.ServiceContainer.RemoveService(GetType(DesignerOptionService), False)
    '    Me.ServiceContainer.AddService(GetType(DesignerOptionService), DesignerOptionService)
    'End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        'Static disposed As Boolean = False
        Try
            If disposing And Not _IsDisposed Then
                '_IsDisposed = True
                If _loader IsNot Nothing Then _loader.Dispose()
                _loader = Nothing

                If _selectionService IsNot Nothing Then
                    RemoveHandler _selectionService.SelectionChanged, AddressOf Me.selectionService_SelectionChanged
                End If
                Try
                    Me.ServiceContainer.RemoveService(GetType(INameCreationService), True)
                    Me.ServiceContainer.RemoveService(GetType(ComponentSerializationService), True)
                    Me.ServiceContainer.RemoveService(GetType(IDesignerSerializationService), True)
                    Me.ServiceContainer.RemoveService(GetType(UndoEngine), True)
                    Me.ServiceContainer.RemoveService(GetType(IMenuCommandService), True)
                    Me.ServiceContainer.RemoveService(GetType(DesignerOptionService), True)
                    Me.ServiceContainer.RemoveService(GetType(ToolboxService), True)
                    Me.ServiceContainer.RemoveService(GetType(ITypeDescriptorFilterService), True)
                    'Me.ServiceContainer.RemoveService(GetType(IToolboxService), True)
                    'Me.ServiceContainer.RemoveService(GetType(VGDDIDE.SolutionExplorer), True)
                    'Me.ServiceContainer.RemoveService(GetType(VGDDIDE.OutputWindow), True)
                    'Me.ServiceContainer.RemoveService(GetType(PropertyGrid), True)

                    'Me.ServiceContainer.RemoveService(GetType(IComponentChangeService), True)
                Catch ex As Exception

                End Try
                _nameCreationService = Nothing
                _codeDomComponentSerializationService = Nothing
                _designerSerializationService = Nothing
                _selectionService = Nothing
                _MenuCommandService.Dispose()
                _undoEngine.Dispose()
                _undoEngine = Nothing
                _DesignerFilterService = Nothing
                'CType(Me.View, VGDD.VGDDScreen).Dispose()
                'For Each oComponent As Component In Me.ComponentContainer.Components
                '    If TypeOf (oComponent) Is VGDD.VGDDScreen Then
                '        oComponent.Dispose()
                '        '                        CType(oComponent, VGDD.VGDDScreen).Controls.Clear()
                '        'Else
                '        '    oComponent.Dispose()
                '    End If
                'Next
                If Me.ComponentContainer.Components.Count > 0 Then
                    Me.ComponentContainer.Components(0).Dispose()
                End If
            End If
        Catch ex As Exception
            'End Try

        End Try
        _IsDisposed = True
        Try
            'If MyBase.IsLoaded Then
            'Dim host As IDesignerHost = CType(Me.GetService(GetType(IDesignerHost)), IDesignerHost)
            'If (host IsNot Nothing) Then
            If Not Me.IsDisposed Then
                MyBase.Dispose(disposing)
            End If

            'End If
        Catch ex As Exception

        End Try
    End Sub


    Public ReadOnly Property IsDisposed As Boolean
        Get
            Return Me._IsDisposed
        End Get
    End Property

    Public ReadOnly Property GetMyUndoEngine As MyUndoEngine
        Get
            Return Me._undoEngine
        End Get
    End Property

    Public Property Loader() As BasicHostLoader ' BasicDesignerLoader
        Get
            Return _loader
        End Get
        Set(ByVal value As BasicHostLoader)
            _loader = value
        End Set
    End Property

    Friend Sub Initialize()
        Dim control As Control = Nothing
        Dim host As IDesignerHost = CType(Me.GetService(GetType(IDesignerHost)), IDesignerHost)
        If (host Is Nothing OrElse host.RootComponent Is Nothing) Then
            Return
        End If
        Try

            If TypeOf host.RootComponent Is Form Then
                control = CType(Me.View, Control)
                control.BackColor = Color.White
            ElseIf TypeOf host.RootComponent Is UserControl Then
                control = CType(Me.View, Control)
                control.BackColor = Color.White
            ElseIf TypeOf host.RootComponent Is Control Then
                control = CType(Me.View, Control)
                control.BackColor = Color.FloralWhite
            Else
                Throw New Exception(("Undefined Host Type: "))
            End If

            ' Set SelectionService - SelectionChanged event handler
            _selectionService = CType(Me.ServiceContainer.GetService(GetType(ISelectionService)), ISelectionService)
            AddHandler _selectionService.SelectionChanged, AddressOf Me.selectionService_SelectionChanged

        Catch ex As Exception
            Trace.WriteLine(ex.ToString)
        End Try
    End Sub

    ' <summary>
    ' When the selection changes this sets the PropertyGrid's selected component 
    ' </summary>
    Public Sub selectionService_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim propGrid As PropertyGrid = CType(Me.GetService(GetType(PropertyGrid)), PropertyGrid)
        Dim selection() As Object = Nothing
        If _selectionService Is Nothing Then Exit Sub
        If _selectionService.SelectionCount = 0 Then
            propGrid = Nothing
        Else
            ReDim selection(_selectionService.SelectionCount - 1)
            _selectionService.GetSelectedComponents().CopyTo(selection, 0)
            Try
                propGrid.SelectedObjects = selection

            Catch ex As Exception

            End Try
        End If

    End Sub

    'Public Sub AddService(ByVal type As Type, ByVal serviceInstance As Object)
    '    Me.ServiceContainer.AddService(type, serviceInstance)
    'End Sub

    'Public Sub RemoveService(ByVal type As Type)
    '    Me.ServiceContainer.RemoveService(type)
    'End Sub

End Class


' Gestione Copia/Incolla 
Class MyDesignerSerializationService
    Implements IDesignerSerializationService
    Private serviceProvider As IServiceProvider
    Public Sub New(ByVal serviceProvider As IServiceProvider)
        Me.serviceProvider = serviceProvider
    End Sub

#Region "IDesignerSerializationService Members"
    Public Function Deserialize(ByVal serializationData As Object) As System.Collections.ICollection Implements System.ComponentModel.Design.Serialization.IDesignerSerializationService.Deserialize
        Try
            Dim serializationStore As SerializationStore = TryCast(serializationData, SerializationStore)
            If serializationStore IsNot Nothing Then
                Dim componentSerializationService As ComponentSerializationService = TryCast(serviceProvider.GetService(GetType(ComponentSerializationService)), ComponentSerializationService)
                Dim collection As ICollection = componentSerializationService.Deserialize(serializationStore)
                If VGDDCommon.Common.ProjectMultiLanguageTranslations > 0 Then
                    If collection.Count > 0 Then
                        VGDDCommon.Common.ControlCopying = True
                        For Each oControl As Control In collection
                            If TypeOf (oControl) Is VGDDMicrochip.VGDDBase Then
                                Dim oVgddBase As VGDDMicrochip.VGDDBase = oControl
                                oVgddBase.Text = oControl.Name
                            End If
                        Next
                        VGDDCommon.Common.ControlCopying = False
                    End If
                End If
                Return collection
            End If
            Return New Object(-1) {}
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function Serialize(ByVal objects As System.Collections.ICollection) As Object Implements System.ComponentModel.Design.Serialization.IDesignerSerializationService.Serialize
        Dim componentSerializationService As ComponentSerializationService = TryCast(serviceProvider.GetService(GetType(ComponentSerializationService)), ComponentSerializationService)
        Dim returnObject As SerializationStore = Nothing
        Try
            Using serializationStore As SerializationStore = componentSerializationService.CreateStore()
                For Each obj As Object In objects
                    If TypeOf obj Is Control Then
                        componentSerializationService.Serialize(serializationStore, obj)
                    End If
                Next
                returnObject = serializationStore
            End Using
        Catch ex As Exception
        End Try
        Return returnObject
    End Function
#End Region
End Class

Public Class MyUndoEngine
    Inherits UndoEngine

    Private undoStack As New Stack(Of UndoEngine.UndoUnit)()
    Private redoStack As New Stack(Of UndoEngine.UndoUnit)()

    Public Sub New(ByVal provider As IServiceProvider)
        MyBase.New(provider)
        ' When created, UndoEngine is enabled
        Me.Enabled = True
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            undoStack.Clear()
            redoStack.Clear()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Public ReadOnly Property EnableUndo() As Boolean
        Get
            Return undoStack.Count > 0
        End Get
    End Property

    Public ReadOnly Property EnableRedo() As Boolean
        Get
            Return redoStack.Count > 0
        End Get
    End Property

    Public Sub Undo()
        If undoStack.Count > 0 Then
            Try
                Dim unit As UndoEngine.UndoUnit = undoStack.Pop()
                unit.Undo()
                redoStack.Push(unit)
            Catch ex As Exception
            End Try
        Else
        End If
    End Sub

    Public Sub Redo()
        If redoStack.Count > 0 Then
            Try
                Dim unit As UndoEngine.UndoUnit = redoStack.Pop()
                unit.Undo()
                undoStack.Push(unit)
            Catch ex As Exception
            End Try
        Else
        End If
    End Sub

    Protected Overrides Sub AddUndoUnit(ByVal unit As UndoEngine.UndoUnit)
        Try
            If Not unit.ToString.EndsWith("Locked") Then
                undoStack.Push(unit)
            End If
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

    'Private Sub MyUndoEngine_Undoing(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Undoing
    '    Debug.Print("?")
    'End Sub

    'Private Sub MyUndoEngine_Undone(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Undone
    '    Debug.Print("?")
    'End Sub

End Class

Public Class MyDesignerFilterService
    Implements System.ComponentModel.Design.ITypeDescriptorFilterService
    Public oldService As ITypeDescriptorFilterService = Nothing

    Public Sub New()
    End Sub

    Public Sub New(ByVal oldService_ As ITypeDescriptorFilterService)
        ' Stores any previous ITypeDescriptorFilterService to implement service chaining.
        Me.oldService = oldService_
    End Sub

    Public Function FilterAttributes(ByVal component As System.ComponentModel.IComponent, ByVal attributes As System.Collections.IDictionary) As Boolean Implements System.ComponentModel.Design.ITypeDescriptorFilterService.FilterAttributes
        If oldService IsNot Nothing Then
            oldService.FilterAttributes(component, attributes)
            'If TypeOf component Is VGDD.VGDDScreen Then
            '    'properties.Add.Attributes(GetType(BrowsableAttribute)) = False
            '    Debug.Print("?")
            'End If
        End If
        Return True
    End Function

    Public Function FilterEvents(ByVal component As System.ComponentModel.IComponent, ByVal events As System.Collections.IDictionary) As Boolean Implements System.ComponentModel.Design.ITypeDescriptorFilterService.FilterEvents
        If oldService IsNot Nothing Then
            oldService.FilterEvents(component, events)
        End If
        Return True
    End Function

    Public Function FilterProperties(ByVal component As System.ComponentModel.IComponent, ByVal properties As System.Collections.IDictionary) As Boolean Implements System.ComponentModel.Design.ITypeDescriptorFilterService.FilterProperties
        If oldService IsNot Nothing Then
            oldService.FilterProperties(component, properties)
        End If

        If TypeOf component Is VGDD.VGDDScreen Then
            RemoveProperty(properties, "DoubleBuffered")
            RemoveProperty(properties, "ShowInTaskbar")
            RemoveProperty(properties, "Text")
            'RemoveProperty(properties, "Size")
            'RemoveProperty(properties, "Locked")
            'Dim x As PropertyDescriptor = properties("Locked")
            'Dim y As AttributeCollection = x.Attributes
            'Dim z As BrowsableAttribute = y(GetType(BrowsableAttribute))
        End If
        Return True
    End Function

    Private Sub RemoveProperty(ByRef properties As System.Collections.IDictionary, ByVal PropertyName As String)
        Dim pdText As PropertyDescriptor = TryCast(properties(PropertyName), PropertyDescriptor)
        If pdText IsNot Nothing Then
            properties.Remove(PropertyName)
        End If
    End Sub
End Class
'End Namespace
