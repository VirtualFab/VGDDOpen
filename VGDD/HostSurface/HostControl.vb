Imports System
'Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Diagnostics

Public Class HostControl
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent
    'Inherits System.Windows.Forms.UserControl

    ' <summary>
    ' Required designer variable.
    ' </summary>
    Private _hostSurface As VGDDHostSurface
    'Private _Loader As BasicHostLoader
    Public _BehaviorService As Windows.Forms.Design.Behavior.BehaviorService
    Private _MyBehavior As MyBehavior
    Public Shared Instances As Integer = 0

    Public Sub New(ByVal hostSurface As VGDDHostSurface)
        MyBase.New()
        ' This call is required by the Windows.Forms Form Designer.
        InitializeComponent()
        InitializeHost(hostSurface)
        Instances += 1
    End Sub

    Public ReadOnly Property Loader() As BasicHostLoader
        Get
            If _hostSurface IsNot Nothing AndAlso _hostSurface.Loader IsNot Nothing Then
                Return _hostSurface.Loader
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property HostSurface() As VGDDHostSurface
        Get
            Return _hostSurface
        End Get
    End Property

    Public ReadOnly Property DesignerHost() As IDesignerHost
        Get
            If _hostSurface IsNot Nothing Then
                Return CType(_hostSurface.GetService(GetType(IDesignerHost)), IDesignerHost)
            Else
                Return Nothing
            End If
        End Get
    End Property

    ' <summary>
    ' Clean up any resources being used.
    ' </summary>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing And Not IsDisposed Then
            If _hostSurface IsNot Nothing AndAlso Not _hostSurface.IsDisposed Then
                Try
                    Instances -= 1
                    _hostSurface.Dispose()
                    _hostSurface = Nothing

                    If _MyBehavior IsNot Nothing Then
                        _BehaviorService.PopBehavior(_MyBehavior)
                        _MyBehavior = Nothing
                        _BehaviorService.Dispose()
                    End If
                Catch ex As Exception
                End Try
            End If
        End If

        Try
            MyBase.Dispose(disposing)
        Catch ex As Exception
        End Try
    End Sub

    ' <summary>
    ' Required method for Designer support - do not modify 
    ' the contents of this method with the code editor.
    ' </summary>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'HostControl
        '
        Me.ClientSize = New System.Drawing.Size(252, 186)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "HostControl"
        Me.ResumeLayout(False)

    End Sub

    Public ReadOnly Property Screen As VGDD.VGDDScreen
        Get
            If _hostSurface IsNot Nothing AndAlso Not _hostSurface.IsDisposed Then
                If _hostSurface.Loader IsNot Nothing Then
                    If _hostSurface.Loader._Screen IsNot Nothing Then
                        Return _hostSurface.Loader._Screen 'Loaded
                    Else
                        Return _hostSurface.ComponentContainer.Components(0) 'Newly created
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property


    Friend Sub InitializeHost(ByVal hostSurface As VGDDHostSurface)
        Try
            If hostSurface Is Nothing Then
                Return
            End If
            _hostSurface = hostSurface
            Dim control As Control
            Try
                control = CType(_hostSurface.View, Control)
            Catch ex As SystemException
                Throw New SystemException(ex.Message)
                Exit Sub
            End Try
            control.Parent = Me
            control.Dock = DockStyle.Fill
            control.Visible = True

            _BehaviorService = _hostSurface.GetService(GetType(Windows.Forms.Design.Behavior.BehaviorService))
            _MyBehavior = New MyBehavior
            _BehaviorService.PushBehavior(_MyBehavior)
            '_BehaviorService.PushCaptureBehavior(_MyBehavior)
            'For Each o As Windows.Forms.Design.Behavior.Adorner In _BehaviorService.Adorners
            '    o.Enabled = False
            'Next
            'Do While _BehaviorService.CurrentBehavior IsNot Nothing
            '    _BehaviorService.PopBehavior(_BehaviorService.CurrentBehavior)
            'Loop

        Catch ex As Exception
            Throw New SystemException(ex.Message)
        End Try
    End Sub

    Private Sub HostControl_DockChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DockChanged
        'If Not VGDDCommon.Common.ProjectLoading Then
        '    VGDDCommon.Common.ProjectChanged = True
        'End If
    End Sub

    Private Sub HostControl_DockStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DockStateChanged
        'If Not VGDDCommon.Common.ProjectLoading Then
        '    VGDDCommon.Common.ProjectChanged = True
        'End If
    End Sub

    Private Sub HostControl_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Debug.Print("")
    End Sub

    Private Sub HostControl_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
        Try
            If Me.Parent Is Nothing OrElse Me.Parent.Parent Is Nothing Then Exit Sub
            If TypeOf (Me.Parent.Parent) Is WeifenLuo.WinFormsUI.Docking.FloatWindow Then
                Dim oFloatWindow As WeifenLuo.WinFormsUI.Docking.FloatWindow = Me.Parent.Parent
                With oFloatWindow
                    .FormBorderStyle = FormBorderStyle.FixedToolWindow
                    .Width = Me.Screen.Width + Me.Screen.Left * 3 '(Me.Screen.Width - Me.Screen.ClientSize.Width)
                    .Height = Me.Screen.Height + Me.Screen.Top * 4 ' + (.Height - .ClientSize.Height)
                End With
            End If
        Catch ex As Exception
        End Try
        'If Not VGDDCommon.Common.ProjectLoading Then
        '    VGDDCommon.Common.ProjectChanged = True
        'End If
    End Sub

    'Public Delegate Function GetPersistingStringCallBack() As String
    Public Function GetPersistingStringCallBack() As String
        Return "HostControl," & Me.Name
    End Function

End Class

Public Class MyBehavior
    Inherits Windows.Forms.Design.Behavior.Behavior

    Public Overrides Function OnMouseUp(ByVal g As Windows.Forms.Design.Behavior.Glyph, ByVal button As MouseButtons) As Boolean
        Try
            If button = MouseButtons.Left Then
                If oMainShell._CurrentHost IsNot Nothing AndAlso oMainShell.MagnifyLockStatus = VGDDCommon.MagnifyStatuses.Locking Then
                    oMainShell._CurrentHost.Cursor = Cursors.Default
                    oMainShell.MagnifyLockStatus = VGDDCommon.MagnifyStatuses.Locked
                    oMainShell.MagnyfyPoint = Cursor.Position
                End If
            End If

        Catch ex As Exception
            'Debug.Print("OnMouseUp")
        End Try
        Return False
    End Function

    'Public Overrides Function OnMouseMove(ByVal g As System.Windows.Forms.Design.Behavior.Glyph, ByVal button As System.Windows.Forms.MouseButtons, ByVal mouseLoc As System.Drawing.Point) As Boolean
    '    Return False
    'End Function

    'Public Overrides Function OnMouseDoubleClick(ByVal g As Windows.Forms.Design.Behavior.Glyph, ByVal button As MouseButtons, ByVal mouseloc As Point) As Boolean
    '    Return False
    'End Function

End Class
