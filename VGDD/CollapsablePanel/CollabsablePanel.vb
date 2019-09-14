'Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.ComponentModel.Design
Imports System.IO
Imports System.Reflection
Imports System.Drawing.Design

''' <summary>
''' Enumeration for defining the options for panelCommon.State
''' </summary>
Public Enum PanelStateOptions
    Collapsed
    Expanded
End Enum

''' <summary>
''' Main class for collapsible panel user control
''' </summary>
<DesignTimeVisible(True)> _
<Category("Containers")> _
<Description("Visual Studio like Collapsible Panel")> _
<Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", GetType(IDesigner))> _
Partial Public Class CollapsablePanel
    Inherits UserControl

#Region "Class members"
#Region "Variables"
    ''' <summary>
    ''' Variable for setting the user control height when control is expanded
    ''' </summary>
    Private _expandedHeight As Integer
    ''' <summary>
    ''' Variable for setting the current user controlCommon.State
    ''' </summary>
    Private _panelState As PanelStateOptions = PanelStateOptions.Expanded
    ''' <summary>
    ''' Variable for determining if the user control is currently collapsed
    ''' </summary>
    Private _isCollapsed As Boolean = False
    ''' <summary>
    ''' Determines if this user control is going to match its parent width
    ''' </summary>
    Private _fitToParent As Boolean = False
    ''' <summary>
    ''' Panel to be located beneath this panel
    ''' </summary>
    Private _nextPanel As CollapsablePanel
#End Region

#Region "Delegates and events"
    ''' <summary>
    ''' Delegate and event for informing the parent control that this user controlCommon.State has changed
    ''' </summary>
    Public Delegate Sub DelegateStateChanged()
    <Category("Collapsible Panel")> _
    Public Event StateChanged As DelegateStateChanged
#End Region

#Region "Properties"

    ''' <summary>
    ''' Gets or sets the value for user control height when it is expanded
    ''' </summary>
    <Description("Gets or sets the value for user control height when it is expanded")> _
    <DisplayName("Expanded Height")> _
    <Category("Collapsible Panel")> _
    <DefaultValueAttribute(0)> _
    Public Property ExpandedHeight() As Integer
        Get
            Return _expandedHeight
        End Get
        Set(ByVal value As Integer)
            If value > 0 Then
                If Me.DesignMode Then
                    If _panelState = PanelStateOptions.Expanded Then
                        'Setting Expanded Height is only allowed when user control is expanded
                        Me.SetBounds(Me.Location.X, Me.Location.Y, Me.Size.Width, titlePanel.Height + value)
                    End If
                Else
                    _expandedHeight = value
                End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets panel title"
    ''' </summary>
    <Description("Gets or sets panel title")> _
    <DisplayName("Panel Title")> _
    <Category("Collapsible Panel")> _
    Public Property PanelTitle() As String
        Get
            Return lblPanelTitle.Text
        End Get
        Set(ByVal value As String)
            lblPanelTitle.Text = value
        End Set
    End Property

    Public Property PanelTitleBackColor As Color
        Get
            Return titlePanel.BackColor
        End Get
        Set(ByVal value As Color)
            titlePanel.BackColor = value
        End Set
    End Property

    Public Property PanelTitleForeColor As Color
        Get
            Return titlePanel.ForeColor
        End Get
        Set(ByVal value As Color)
            titlePanel.ForeColor = value
        End Set
    End Property


    ''' <summary>
    ''' Gets or sets current panelCommon.State
    ''' </summary>
    <DefaultValue(GetType(PanelStateOptions), "Expanded")> _
    <Description("Gets or sets current panelCommon.State")> _
    <DisplayName("PanelCommon.State")> _
    <Category("Collapsible Panel")> _
    Public Property PanelState() As PanelStateOptions
        Get
            Return _panelState
        End Get
        Set(ByVal value As PanelStateOptions)
            _panelState = value
            _isCollapsed = (_panelState <> PanelStateOptions.Collapsed)
            ToggleState(Nothing, Nothing)
        End Set
    End Property

    ''' <summary>
    ''' If True, fits the panel to match the parent width
    ''' </summary>
    <Category("Collapsible Panel")> _
    <DesignOnly(True)> _
    <DefaultValue(False)> _
    <Description("If True, fits the panel to match the parent width")> _
    Public Property FitToParent() As Boolean
        Get
            Return _fitToParent
        End Get
        Set(ByVal value As Boolean)
            _fitToParent = value
            If _fitToParent Then
                If Me.Parent IsNot Nothing Then
                    Me.Location = New Point(0, Me.Location.Y)
                    Me.Size = New Size(Me.Parent.Size.Width, Me.Size.Height)
                    Me.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
                End If
            Else
                Me.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the panel to be located beneath this panel
    ''' </summary>
    <Category("Collapsible Panel")> _
    <Description("Gets or sets the panel to be located beneath this panel")> _
    Public Property NextPanel() As CollapsablePanel
        Get
            Return _nextPanel
        End Get
        Set(ByVal value As CollapsablePanel)
            _nextPanel = value
            MoveNextPanel()
        End Set
    End Property
#End Region

#End Region

#Region "Class constructor"
    Public Sub New()
        InitializeComponent()
        'Me.Height = Me.titlePanel.Height

        AddHandler Me.Load, New EventHandler(AddressOf CollapsablePanel_Load)
        AddHandler Me.SizeChanged, New EventHandler(AddressOf CollapsablePanel_SizeChanged)

        AddHandler Me.LocationChanged, New EventHandler(AddressOf CollapsablePanel_LocationChanged)
    End Sub


#End Region

#Region "Methods for handling user control events"

    Private Sub CollapsablePanel_Load(ByVal sender As Object, ByVal e As EventArgs)
        _isCollapsed = (_panelState = PanelStateOptions.Collapsed)

        If _isCollapsed Then
            togglingImage.Image = My.Resources.ComboBoxPlus
        Else
            togglingImage.Image = My.Resources.ComboBoxMinus
        End If
    End Sub

    ''' <summary>
    ''' We use this event to recalculate the Expanded Height when the user resizes the user control at Design mode
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CollapsablePanel_SizeChanged(ByVal sender As Object, ByVal e As EventArgs)

        If Me.DesignMode Then
            If _panelState = PanelStateOptions.Expanded Then
                _expandedHeight = Me.Height - titlePanel.Height
            Else
                'Final user can only resize the user control when it is expanded
                Me.SetBounds(Me.Location.X, Me.Location.Y, Me.Size.Width, titlePanel.Height)
            End If

            If Me.Parent IsNot Nothing Then
                If Me.Parent.Size.Width <> Me.Size.Width Then
                    FitToParent = False
                End If
            End If
        End If

        MoveNextPanel()
    End Sub

    ''' <summary>
    ''' We use this event in order to move the next panel down when this panel located is changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CollapsablePanel_LocationChanged(ByVal sender As Object, ByVal e As EventArgs)
        If Me.DesignMode Then
            If Me.Location.X > 0 Then
                FitToParent = False
            End If
        End If

        MoveNextPanel()
    End Sub


#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Changes the currentCommon.State from Collapsed to Expanded or viceversa
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToggleState(ByVal sender As Object, ByVal e As EventArgs) Handles togglingImage.Click
        If _isCollapsed Then
            ''CurrentCommon.State is Collapsed.  Expand the user control
            'Me.SetBounds(Me.Location.X, Me.Location.Y, Me.Size.Width, titlePanel.Height + _expandedHeight)
            Me.SetBounds(Me.Location.X, Me.Location.Y, Me.Size.Width, _expandedHeight - titlePanel.Height \ 2)
        Else
            ''CurrentCommon.State is Expanded.  Collapse the user control
            Me.SetBounds(Me.Location.X, Me.Location.Y, Me.Size.Width, titlePanel.Height)
        End If

        _isCollapsed = Not _isCollapsed

        ''Setting content control currentCommon.State and toggling image
        If _isCollapsed Then
            _panelState = PanelStateOptions.Collapsed
            togglingImage.Image = My.Resources.ComboBoxPlus
        Else
            _panelState = PanelStateOptions.Expanded
            togglingImage.Image = My.Resources.ComboBoxMinus
        End If

        If Not Me.DesignMode Then
            ''Fire the event to inform the parent control that theCommon.State for the user control has changed
            RaiseEvent StateChanged()
        End If
    End Sub

    ''' <summary>
    ''' Moves the next panel down (when user controlCommon.State is changed or the control is relocated)
    ''' </summary>
    Private Sub MoveNextPanel()
        If _nextPanel IsNot Nothing Then
            _nextPanel.Location = New Point(_nextPanel.Location.X, Me.Location.Y + Me.Size.Height)
        End If
    End Sub
#End Region

    Private Sub titlePanel_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles titlePanel.DoubleClick
        Me.ToggleState(Nothing, Nothing)
    End Sub
End Class
