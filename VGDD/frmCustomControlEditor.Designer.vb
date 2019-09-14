'Namespace VGDDCommon
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCustomWidgetEditor
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCustomWidgetEditor))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.btnSaveWidget = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnNewWidget = New System.Windows.Forms.Button()
        Me.tvCustomEdit = New System.Windows.Forms.TreeView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmbCustomWidgetName = New System.Windows.Forms.ComboBox()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.grpCodeOptions = New System.Windows.Forms.GroupBox()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.cmbCodeParams = New System.Windows.Forms.ComboBox()
        Me.grpBitmap = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblBitmapFileName = New System.Windows.Forms.Label()
        Me.btnChooseBitmap = New System.Windows.Forms.Button()
        Me.grpDefinition = New System.Windows.Forms.GroupBox()
        Me.btnNewProperty = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtPropertyName = New System.Windows.Forms.TextBox()
        Me.grpEvents = New System.Windows.Forms.GroupBox()
        Me.btnCreateEvent = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtNewEventName = New System.Windows.Forms.TextBox()
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.btnCreateAction = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtNewActionName = New System.Windows.Forms.TextBox()
        Me.grpStates = New System.Windows.Forms.GroupBox()
        Me.btnCreateState = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtNewStateName = New System.Windows.Forms.TextBox()
        Me.grpStateDetails = New System.Windows.Forms.GroupBox()
        Me.btnDeleteState = New System.Windows.Forms.Button()
        Me.grdStates = New System.Windows.Forms.DataGridView()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtStateName = New System.Windows.Forms.TextBox()
        Me.grpEventDetails = New System.Windows.Forms.GroupBox()
        Me.btnDeleteEvent = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtEventDescription = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtEventName = New System.Windows.Forms.TextBox()
        Me.grpActionDetails = New System.Windows.Forms.GroupBox()
        Me.btnDeleteAction = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtActionCode = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtActionName = New System.Windows.Forms.TextBox()
        Me.grpProperty = New System.Windows.Forms.GroupBox()
        Me.btnDeleteProperty = New System.Windows.Forms.Button()
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.rtEdit = New System.Windows.Forms.RichTextBox()
        Me.picBitmap = New System.Windows.Forms.PictureBox()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.grpCodeOptions.SuspendLayout()
        Me.grpBitmap.SuspendLayout()
        Me.grpDefinition.SuspendLayout()
        Me.grpEvents.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.grpStates.SuspendLayout()
        Me.grpStateDetails.SuspendLayout()
        CType(Me.grdStates, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpEventDetails.SuspendLayout()
        Me.grpActionDetails.SuspendLayout()
        Me.grpProperty.SuspendLayout()
        CType(Me.picBitmap, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnSaveWidget)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnDelete)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnNewWidget)
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvCustomEdit)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cmbCustomWidgetName)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer1.Size = New System.Drawing.Size(866, 410)
        Me.SplitContainer1.SplitterDistance = 288
        Me.SplitContainer1.TabIndex = 0
        '
        'btnSaveWidget
        '
        Me.btnSaveWidget.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnSaveWidget.Location = New System.Drawing.Point(101, 25)
        Me.btnSaveWidget.Name = "btnSaveWidget"
        Me.btnSaveWidget.Size = New System.Drawing.Size(89, 21)
        Me.btnSaveWidget.TabIndex = 8
        Me.btnSaveWidget.Text = "Save Widget"
        Me.btnSaveWidget.UseVisualStyleBackColor = False
        Me.btnSaveWidget.Visible = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnDelete.Location = New System.Drawing.Point(216, 25)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(60, 21)
        Me.btnDelete.TabIndex = 6
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = False
        Me.btnDelete.Visible = False
        '
        'btnNewWidget
        '
        Me.btnNewWidget.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnNewWidget.Location = New System.Drawing.Point(6, 25)
        Me.btnNewWidget.Name = "btnNewWidget"
        Me.btnNewWidget.Size = New System.Drawing.Size(60, 21)
        Me.btnNewWidget.TabIndex = 5
        Me.btnNewWidget.Text = "New"
        Me.btnNewWidget.UseVisualStyleBackColor = False
        Me.btnNewWidget.Visible = False
        '
        'tvCustomEdit
        '
        Me.tvCustomEdit.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvCustomEdit.Location = New System.Drawing.Point(0, 52)
        Me.tvCustomEdit.Name = "tvCustomEdit"
        Me.tvCustomEdit.Size = New System.Drawing.Size(282, 344)
        Me.tvCustomEdit.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Custom Widget:"
        '
        'cmbCustomWidgetName
        '
        Me.cmbCustomWidgetName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbCustomWidgetName.FormattingEnabled = True
        Me.cmbCustomWidgetName.Location = New System.Drawing.Point(90, 3)
        Me.cmbCustomWidgetName.Name = "cmbCustomWidgetName"
        Me.cmbCustomWidgetName.Size = New System.Drawing.Size(186, 21)
        Me.cmbCustomWidgetName.TabIndex = 2
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.grpCodeOptions)
        Me.SplitContainer3.Panel1.Controls.Add(Me.grpBitmap)
        Me.SplitContainer3.Panel1.Controls.Add(Me.grpDefinition)
        Me.SplitContainer3.Panel1.Controls.Add(Me.grpEvents)
        Me.SplitContainer3.Panel1.Controls.Add(Me.grpActions)
        Me.SplitContainer3.Panel1.Controls.Add(Me.grpStates)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.grpStateDetails)
        Me.SplitContainer3.Panel2.Controls.Add(Me.grpEventDetails)
        Me.SplitContainer3.Panel2.Controls.Add(Me.grpActionDetails)
        Me.SplitContainer3.Panel2.Controls.Add(Me.grpProperty)
        Me.SplitContainer3.Panel2.Controls.Add(Me.rtEdit)
        Me.SplitContainer3.Panel2.Controls.Add(Me.picBitmap)
        Me.SplitContainer3.Size = New System.Drawing.Size(574, 410)
        Me.SplitContainer3.SplitterDistance = 92
        Me.SplitContainer3.TabIndex = 0
        '
        'grpCodeOptions
        '
        Me.grpCodeOptions.Controls.Add(Me.btnInsert)
        Me.grpCodeOptions.Controls.Add(Me.cmbCodeParams)
        Me.grpCodeOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpCodeOptions.Location = New System.Drawing.Point(0, 0)
        Me.grpCodeOptions.Name = "grpCodeOptions"
        Me.grpCodeOptions.Size = New System.Drawing.Size(574, 92)
        Me.grpCodeOptions.TabIndex = 7
        Me.grpCodeOptions.TabStop = False
        Me.grpCodeOptions.Text = "Code Options"
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnInsert.Location = New System.Drawing.Point(374, 23)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(60, 21)
        Me.btnInsert.TabIndex = 2
        Me.btnInsert.Text = "Insert"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'cmbCodeParams
        '
        Me.cmbCodeParams.FormattingEnabled = True
        Me.cmbCodeParams.Location = New System.Drawing.Point(8, 23)
        Me.cmbCodeParams.Name = "cmbCodeParams"
        Me.cmbCodeParams.Size = New System.Drawing.Size(352, 21)
        Me.cmbCodeParams.TabIndex = 1
        '
        'grpBitmap
        '
        Me.grpBitmap.Controls.Add(Me.Label3)
        Me.grpBitmap.Controls.Add(Me.lblBitmapFileName)
        Me.grpBitmap.Controls.Add(Me.btnChooseBitmap)
        Me.grpBitmap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpBitmap.Location = New System.Drawing.Point(0, 0)
        Me.grpBitmap.Name = "grpBitmap"
        Me.grpBitmap.Size = New System.Drawing.Size(574, 92)
        Me.grpBitmap.TabIndex = 8
        Me.grpBitmap.TabStop = False
        Me.grpBitmap.Text = "Choose Bitmap"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 45)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(124, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Current Bitmap Filename:"
        '
        'lblBitmapFileName
        '
        Me.lblBitmapFileName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblBitmapFileName.Location = New System.Drawing.Point(139, 45)
        Me.lblBitmapFileName.Name = "lblBitmapFileName"
        Me.lblBitmapFileName.Size = New System.Drawing.Size(385, 23)
        Me.lblBitmapFileName.TabIndex = 3
        Me.lblBitmapFileName.Text = "<Filename>"
        '
        'btnChooseBitmap
        '
        Me.btnChooseBitmap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnChooseBitmap.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnChooseBitmap.Location = New System.Drawing.Point(530, 47)
        Me.btnChooseBitmap.Name = "btnChooseBitmap"
        Me.btnChooseBitmap.Size = New System.Drawing.Size(38, 21)
        Me.btnChooseBitmap.TabIndex = 2
        Me.btnChooseBitmap.Text = "..."
        Me.btnChooseBitmap.UseVisualStyleBackColor = False
        '
        'grpDefinition
        '
        Me.grpDefinition.Controls.Add(Me.btnNewProperty)
        Me.grpDefinition.Controls.Add(Me.Label2)
        Me.grpDefinition.Controls.Add(Me.txtPropertyName)
        Me.grpDefinition.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpDefinition.Location = New System.Drawing.Point(0, 0)
        Me.grpDefinition.Name = "grpDefinition"
        Me.grpDefinition.Size = New System.Drawing.Size(574, 92)
        Me.grpDefinition.TabIndex = 6
        Me.grpDefinition.TabStop = False
        Me.grpDefinition.Text = "Properties"
        '
        'btnNewProperty
        '
        Me.btnNewProperty.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnNewProperty.Enabled = False
        Me.btnNewProperty.Location = New System.Drawing.Point(259, 37)
        Me.btnNewProperty.Name = "btnNewProperty"
        Me.btnNewProperty.Size = New System.Drawing.Size(123, 21)
        Me.btnNewProperty.TabIndex = 2
        Me.btnNewProperty.Text = "Create New Property"
        Me.btnNewProperty.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 41)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Property Name:"
        '
        'txtPropertyName
        '
        Me.txtPropertyName.Location = New System.Drawing.Point(96, 38)
        Me.txtPropertyName.Name = "txtPropertyName"
        Me.txtPropertyName.Size = New System.Drawing.Size(157, 20)
        Me.txtPropertyName.TabIndex = 3
        '
        'grpEvents
        '
        Me.grpEvents.Controls.Add(Me.btnCreateEvent)
        Me.grpEvents.Controls.Add(Me.Label4)
        Me.grpEvents.Controls.Add(Me.txtNewEventName)
        Me.grpEvents.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpEvents.Location = New System.Drawing.Point(0, 0)
        Me.grpEvents.Name = "grpEvents"
        Me.grpEvents.Size = New System.Drawing.Size(574, 92)
        Me.grpEvents.TabIndex = 9
        Me.grpEvents.TabStop = False
        Me.grpEvents.Text = "Events"
        Me.grpEvents.Visible = False
        '
        'btnCreateEvent
        '
        Me.btnCreateEvent.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnCreateEvent.Enabled = False
        Me.btnCreateEvent.Location = New System.Drawing.Point(270, 37)
        Me.btnCreateEvent.Name = "btnCreateEvent"
        Me.btnCreateEvent.Size = New System.Drawing.Size(123, 21)
        Me.btnCreateEvent.TabIndex = 2
        Me.btnCreateEvent.Text = "Create New Event"
        Me.btnCreateEvent.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(10, 41)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(94, 13)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "New Event Name:"
        '
        'txtNewEventName
        '
        Me.txtNewEventName.Location = New System.Drawing.Point(107, 38)
        Me.txtNewEventName.Name = "txtNewEventName"
        Me.txtNewEventName.Size = New System.Drawing.Size(157, 20)
        Me.txtNewEventName.TabIndex = 3
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnCreateAction)
        Me.grpActions.Controls.Add(Me.Label7)
        Me.grpActions.Controls.Add(Me.txtNewActionName)
        Me.grpActions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpActions.Location = New System.Drawing.Point(0, 0)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Size = New System.Drawing.Size(574, 92)
        Me.grpActions.TabIndex = 10
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        Me.grpActions.Visible = False
        '
        'btnCreateAction
        '
        Me.btnCreateAction.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnCreateAction.Enabled = False
        Me.btnCreateAction.Location = New System.Drawing.Point(270, 37)
        Me.btnCreateAction.Name = "btnCreateAction"
        Me.btnCreateAction.Size = New System.Drawing.Size(123, 21)
        Me.btnCreateAction.TabIndex = 2
        Me.btnCreateAction.Text = "Create New Action"
        Me.btnCreateAction.UseVisualStyleBackColor = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(10, 41)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(96, 13)
        Me.Label7.TabIndex = 4
        Me.Label7.Text = "New Action Name:"
        '
        'txtNewActionName
        '
        Me.txtNewActionName.Location = New System.Drawing.Point(107, 38)
        Me.txtNewActionName.Name = "txtNewActionName"
        Me.txtNewActionName.Size = New System.Drawing.Size(157, 20)
        Me.txtNewActionName.TabIndex = 3
        '
        'grpStates
        '
        Me.grpStates.Controls.Add(Me.btnCreateState)
        Me.grpStates.Controls.Add(Me.Label13)
        Me.grpStates.Controls.Add(Me.txtNewStateName)
        Me.grpStates.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpStates.Location = New System.Drawing.Point(0, 0)
        Me.grpStates.Name = "grpStates"
        Me.grpStates.Size = New System.Drawing.Size(574, 92)
        Me.grpStates.TabIndex = 11
        Me.grpStates.TabStop = False
        Me.grpStates.Text = "States"
        Me.grpStates.Visible = False
        '
        'btnCreateState
        '
        Me.btnCreateState.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnCreateState.Enabled = False
        Me.btnCreateState.Location = New System.Drawing.Point(270, 37)
        Me.btnCreateState.Name = "btnCreateState"
        Me.btnCreateState.Size = New System.Drawing.Size(123, 21)
        Me.btnCreateState.TabIndex = 2
        Me.btnCreateState.Text = "Create New State"
        Me.btnCreateState.UseVisualStyleBackColor = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(10, 41)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(91, 13)
        Me.Label13.TabIndex = 4
        Me.Label13.Text = "New State Name:"
        '
        'txtNewStateName
        '
        Me.txtNewStateName.Location = New System.Drawing.Point(107, 38)
        Me.txtNewStateName.Name = "txtNewStateName"
        Me.txtNewStateName.Size = New System.Drawing.Size(157, 20)
        Me.txtNewStateName.TabIndex = 3
        '
        'grpStateDetails
        '
        Me.grpStateDetails.Controls.Add(Me.btnDeleteState)
        Me.grpStateDetails.Controls.Add(Me.grdStates)
        Me.grpStateDetails.Controls.Add(Me.Label11)
        Me.grpStateDetails.Controls.Add(Me.txtStateName)
        Me.grpStateDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpStateDetails.Location = New System.Drawing.Point(0, 0)
        Me.grpStateDetails.Name = "grpStateDetails"
        Me.grpStateDetails.Size = New System.Drawing.Size(574, 314)
        Me.grpStateDetails.TabIndex = 12
        Me.grpStateDetails.TabStop = False
        Me.grpStateDetails.Text = "State Details"
        Me.grpStateDetails.Visible = False
        '
        'btnDeleteState
        '
        Me.btnDeleteState.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnDeleteState.Location = New System.Drawing.Point(392, 37)
        Me.btnDeleteState.Name = "btnDeleteState"
        Me.btnDeleteState.Size = New System.Drawing.Size(60, 21)
        Me.btnDeleteState.TabIndex = 7
        Me.btnDeleteState.Text = "Delete"
        Me.btnDeleteState.UseVisualStyleBackColor = False
        '
        'grdStates
        '
        Me.grdStates.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grdStates.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.grdStates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdStates.Location = New System.Drawing.Point(22, 82)
        Me.grdStates.Name = "grdStates"
        Me.grdStates.Size = New System.Drawing.Size(546, 218)
        Me.grdStates.TabIndex = 5
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(10, 41)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(66, 13)
        Me.Label11.TabIndex = 4
        Me.Label11.Text = "State Name:"
        '
        'txtStateName
        '
        Me.txtStateName.Enabled = False
        Me.txtStateName.Location = New System.Drawing.Point(104, 38)
        Me.txtStateName.Name = "txtStateName"
        Me.txtStateName.Size = New System.Drawing.Size(236, 20)
        Me.txtStateName.TabIndex = 3
        '
        'grpEventDetails
        '
        Me.grpEventDetails.Controls.Add(Me.btnDeleteEvent)
        Me.grpEventDetails.Controls.Add(Me.Label6)
        Me.grpEventDetails.Controls.Add(Me.txtEventDescription)
        Me.grpEventDetails.Controls.Add(Me.Label5)
        Me.grpEventDetails.Controls.Add(Me.txtEventName)
        Me.grpEventDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpEventDetails.Location = New System.Drawing.Point(0, 0)
        Me.grpEventDetails.Name = "grpEventDetails"
        Me.grpEventDetails.Size = New System.Drawing.Size(574, 314)
        Me.grpEventDetails.TabIndex = 10
        Me.grpEventDetails.TabStop = False
        Me.grpEventDetails.Text = "Event Details"
        Me.grpEventDetails.Visible = False
        '
        'btnDeleteEvent
        '
        Me.btnDeleteEvent.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnDeleteEvent.Location = New System.Drawing.Point(374, 37)
        Me.btnDeleteEvent.Name = "btnDeleteEvent"
        Me.btnDeleteEvent.Size = New System.Drawing.Size(60, 21)
        Me.btnDeleteEvent.TabIndex = 7
        Me.btnDeleteEvent.Text = "Delete"
        Me.btnDeleteEvent.UseVisualStyleBackColor = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(10, 82)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(63, 13)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Description:"
        '
        'txtEventDescription
        '
        Me.txtEventDescription.Location = New System.Drawing.Point(96, 79)
        Me.txtEventDescription.Multiline = True
        Me.txtEventDescription.Name = "txtEventDescription"
        Me.txtEventDescription.Size = New System.Drawing.Size(428, 79)
        Me.txtEventDescription.TabIndex = 5
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(10, 41)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Event Name:"
        '
        'txtEventName
        '
        Me.txtEventName.Enabled = False
        Me.txtEventName.Location = New System.Drawing.Point(96, 38)
        Me.txtEventName.Name = "txtEventName"
        Me.txtEventName.Size = New System.Drawing.Size(236, 20)
        Me.txtEventName.TabIndex = 3
        '
        'grpActionDetails
        '
        Me.grpActionDetails.Controls.Add(Me.btnDeleteAction)
        Me.grpActionDetails.Controls.Add(Me.Label8)
        Me.grpActionDetails.Controls.Add(Me.txtActionCode)
        Me.grpActionDetails.Controls.Add(Me.Label9)
        Me.grpActionDetails.Controls.Add(Me.txtActionName)
        Me.grpActionDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpActionDetails.Location = New System.Drawing.Point(0, 0)
        Me.grpActionDetails.Name = "grpActionDetails"
        Me.grpActionDetails.Size = New System.Drawing.Size(574, 314)
        Me.grpActionDetails.TabIndex = 11
        Me.grpActionDetails.TabStop = False
        Me.grpActionDetails.Text = "Action Details"
        Me.grpActionDetails.Visible = False
        '
        'btnDeleteAction
        '
        Me.btnDeleteAction.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnDeleteAction.Location = New System.Drawing.Point(362, 37)
        Me.btnDeleteAction.Name = "btnDeleteAction"
        Me.btnDeleteAction.Size = New System.Drawing.Size(60, 21)
        Me.btnDeleteAction.TabIndex = 7
        Me.btnDeleteAction.Text = "Delete"
        Me.btnDeleteAction.UseVisualStyleBackColor = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(55, 79)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(35, 13)
        Me.Label8.TabIndex = 6
        Me.Label8.Text = "Code:"
        '
        'txtActionCode
        '
        Me.txtActionCode.Location = New System.Drawing.Point(96, 79)
        Me.txtActionCode.Multiline = True
        Me.txtActionCode.Name = "txtActionCode"
        Me.txtActionCode.Size = New System.Drawing.Size(428, 79)
        Me.txtActionCode.TabIndex = 5
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(19, 41)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(71, 13)
        Me.Label9.TabIndex = 4
        Me.Label9.Text = "Action Name:"
        '
        'txtActionName
        '
        Me.txtActionName.Enabled = False
        Me.txtActionName.Location = New System.Drawing.Point(96, 38)
        Me.txtActionName.Name = "txtActionName"
        Me.txtActionName.Size = New System.Drawing.Size(236, 20)
        Me.txtActionName.TabIndex = 3
        '
        'grpProperty
        '
        Me.grpProperty.Controls.Add(Me.btnDeleteProperty)
        Me.grpProperty.Controls.Add(Me.PropertyGrid1)
        Me.grpProperty.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpProperty.Location = New System.Drawing.Point(0, 0)
        Me.grpProperty.Name = "grpProperty"
        Me.grpProperty.Size = New System.Drawing.Size(574, 314)
        Me.grpProperty.TabIndex = 14
        Me.grpProperty.TabStop = False
        Me.grpProperty.Text = "Edit Property"
        '
        'btnDeleteProperty
        '
        Me.btnDeleteProperty.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnDeleteProperty.Location = New System.Drawing.Point(96, 16)
        Me.btnDeleteProperty.Name = "btnDeleteProperty"
        Me.btnDeleteProperty.Size = New System.Drawing.Size(96, 21)
        Me.btnDeleteProperty.TabIndex = 7
        Me.btnDeleteProperty.Text = "Delete Property"
        Me.btnDeleteProperty.UseVisualStyleBackColor = False
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid1.Location = New System.Drawing.Point(3, 16)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.Size = New System.Drawing.Size(568, 295)
        Me.PropertyGrid1.TabIndex = 1
        '
        'rtEdit
        '
        Me.rtEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtEdit.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtEdit.Location = New System.Drawing.Point(0, 0)
        Me.rtEdit.Name = "rtEdit"
        Me.rtEdit.Size = New System.Drawing.Size(574, 314)
        Me.rtEdit.TabIndex = 0
        Me.rtEdit.Text = ""
        '
        'picBitmap
        '
        Me.picBitmap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picBitmap.Location = New System.Drawing.Point(0, 0)
        Me.picBitmap.Name = "picBitmap"
        Me.picBitmap.Size = New System.Drawing.Size(574, 314)
        Me.picBitmap.TabIndex = 2
        Me.picBitmap.TabStop = False
        '
        'frmCustomWidgetEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(866, 410)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmCustomWidgetEditor"
        Me.Text = "Custom Widgets Editor"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.ResumeLayout(False)
        Me.grpCodeOptions.ResumeLayout(False)
        Me.grpBitmap.ResumeLayout(False)
        Me.grpBitmap.PerformLayout()
        Me.grpDefinition.ResumeLayout(False)
        Me.grpDefinition.PerformLayout()
        Me.grpEvents.ResumeLayout(False)
        Me.grpEvents.PerformLayout()
        Me.grpActions.ResumeLayout(False)
        Me.grpActions.PerformLayout()
        Me.grpStates.ResumeLayout(False)
        Me.grpStates.PerformLayout()
        Me.grpStateDetails.ResumeLayout(False)
        Me.grpStateDetails.PerformLayout()
        CType(Me.grdStates, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpEventDetails.ResumeLayout(False)
        Me.grpEventDetails.PerformLayout()
        Me.grpActionDetails.ResumeLayout(False)
        Me.grpActionDetails.PerformLayout()
        Me.grpProperty.ResumeLayout(False)
        CType(Me.picBitmap, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents tvCustomEdit As System.Windows.Forms.TreeView
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbCustomWidgetName As System.Windows.Forms.ComboBox
    Friend WithEvents rtEdit As System.Windows.Forms.RichTextBox
    Friend WithEvents PropertyGrid1 As System.Windows.Forms.PropertyGrid
    Friend WithEvents cmbCodeParams As System.Windows.Forms.ComboBox
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnNewWidget As System.Windows.Forms.Button
    Friend WithEvents btnNewProperty As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtPropertyName As System.Windows.Forms.TextBox
    Friend WithEvents grpDefinition As System.Windows.Forms.GroupBox
    Friend WithEvents grpCodeOptions As System.Windows.Forms.GroupBox
    Friend WithEvents grpBitmap As System.Windows.Forms.GroupBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblBitmapFileName As System.Windows.Forms.Label
    Friend WithEvents btnChooseBitmap As System.Windows.Forms.Button
    Friend WithEvents picBitmap As System.Windows.Forms.PictureBox
    Friend WithEvents grpEvents As System.Windows.Forms.GroupBox
    Friend WithEvents btnCreateEvent As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtNewEventName As System.Windows.Forms.TextBox
    Friend WithEvents grpEventDetails As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtEventName As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtEventDescription As System.Windows.Forms.TextBox
    Friend WithEvents grpActions As System.Windows.Forms.GroupBox
    Friend WithEvents btnCreateAction As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtNewActionName As System.Windows.Forms.TextBox
    Friend WithEvents grpActionDetails As System.Windows.Forms.GroupBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtActionCode As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtActionName As System.Windows.Forms.TextBox
    Friend WithEvents grpStateDetails As System.Windows.Forms.GroupBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtStateName As System.Windows.Forms.TextBox
    Friend WithEvents grpStates As System.Windows.Forms.GroupBox
    Friend WithEvents btnCreateState As System.Windows.Forms.Button
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtNewStateName As System.Windows.Forms.TextBox
    Friend WithEvents grpProperty As System.Windows.Forms.GroupBox
    Friend WithEvents btnSaveWidget As System.Windows.Forms.Button
    Friend WithEvents grdStates As System.Windows.Forms.DataGridView
    Friend WithEvents btnDeleteState As System.Windows.Forms.Button
    Friend WithEvents btnDeleteProperty As System.Windows.Forms.Button
    Friend WithEvents btnDeleteEvent As System.Windows.Forms.Button
    Friend WithEvents btnDeleteAction As System.Windows.Forms.Button
End Class
'End Namespace