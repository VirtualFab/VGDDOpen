Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EventsTree
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        'UserControl overrides dispose to clean up the component list.
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
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EventsTree))
            Me.tvEvents = New System.Windows.Forms.TreeView()
            Me.EventsImageList1 = New System.Windows.Forms.ImageList(Me.components)
            Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.EditAllEventsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ContextMenuStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'tvEvents
            '
            Me.tvEvents.ContextMenuStrip = Me.ContextMenuStrip1
            Me.tvEvents.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tvEvents.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.tvEvents.ImageIndex = 0
            Me.tvEvents.ImageList = Me.EventsImageList1
            Me.tvEvents.Location = New System.Drawing.Point(0, 0)
            Me.tvEvents.Name = "tvEvents"
            Me.tvEvents.SelectedImageIndex = 0
            Me.tvEvents.ShowNodeToolTips = True
            Me.tvEvents.Size = New System.Drawing.Size(427, 358)
            Me.tvEvents.TabIndex = 0
            '
            'EventsImageList1
            '
            Me.EventsImageList1.ImageStream = CType(resources.GetObject("EventsImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
            Me.EventsImageList1.TransparentColor = System.Drawing.Color.Transparent
            Me.EventsImageList1.Images.SetKeyName(0, "monitor.ico")
            Me.EventsImageList1.Images.SetKeyName(1, "ui-toolbar--arrow.ico")
            Me.EventsImageList1.Images.SetKeyName(2, "lightning.ico")
            Me.EventsImageList1.Images.SetKeyName(3, "lightning--arrow.ico")
            '
            'ContextMenuStrip1
            '
            Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditAllEventsToolStripMenuItem})
            Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
            Me.ContextMenuStrip1.Size = New System.Drawing.Size(153, 48)
            '
            'EditAllEventsToolStripMenuItem
            '
            Me.EditAllEventsToolStripMenuItem.Name = "EditAllEventsToolStripMenuItem"
            Me.EditAllEventsToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.EditAllEventsToolStripMenuItem.Text = "Edit all Events"
            '
            'EventsTree
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(427, 358)
            Me.CloseButtonVisible = False
            Me.Controls.Add(Me.tvEvents)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "EventsTree"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
            Me.TabText = "Events"
            Me.Text = "Events"
            Me.ContextMenuStrip1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents tvEvents As System.Windows.Forms.TreeView
        Friend WithEvents EventsImageList1 As System.Windows.Forms.ImageList
        Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents EditAllEventsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    End Class
End Namespace