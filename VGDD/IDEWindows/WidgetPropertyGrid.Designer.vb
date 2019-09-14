Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class WidgetPropertyGrid
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WidgetPropertyGrid))
            Me.PropertyGrid = New PropertyGridEx.PropertyGridEx()
            Me.SuspendLayout()
            '
            'PropertyGrid
            '
            '
            '
            '
            Me.PropertyGrid.DocCommentDescription.AccessibleName = ""
            Me.PropertyGrid.DocCommentDescription.AutoEllipsis = True
            Me.PropertyGrid.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default
            Me.PropertyGrid.DocCommentDescription.Location = New System.Drawing.Point(3, 19)
            Me.PropertyGrid.DocCommentDescription.Name = ""
            Me.PropertyGrid.DocCommentDescription.Size = New System.Drawing.Size(381, 36)
            Me.PropertyGrid.DocCommentDescription.TabIndex = 1
            Me.PropertyGrid.DocCommentImage = Nothing
            '
            '
            '
            Me.PropertyGrid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default
            Me.PropertyGrid.DocCommentTitle.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
            Me.PropertyGrid.DocCommentTitle.Location = New System.Drawing.Point(3, 3)
            Me.PropertyGrid.DocCommentTitle.Name = ""
            Me.PropertyGrid.DocCommentTitle.Size = New System.Drawing.Size(381, 16)
            Me.PropertyGrid.DocCommentTitle.TabIndex = 0
            Me.PropertyGrid.DocCommentTitle.UseMnemonic = False
            Me.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.PropertyGrid.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me.PropertyGrid.Location = New System.Drawing.Point(0, 0)
            Me.PropertyGrid.Name = "PropertyGrid"
            Me.PropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical
            Me.PropertyGrid.ShowCustomProperties = True
            Me.PropertyGrid.Size = New System.Drawing.Size(387, 429)
            Me.PropertyGrid.TabIndex = 0
            '
            '
            '
            Me.PropertyGrid.ToolStrip.AccessibleName = "ToolBar"
            Me.PropertyGrid.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar
            Me.PropertyGrid.ToolStrip.AllowMerge = False
            Me.PropertyGrid.ToolStrip.AutoSize = False
            Me.PropertyGrid.ToolStrip.CanOverflow = False
            Me.PropertyGrid.ToolStrip.Dock = System.Windows.Forms.DockStyle.None
            Me.PropertyGrid.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.PropertyGrid.ToolStrip.Location = New System.Drawing.Point(0, 1)
            Me.PropertyGrid.ToolStrip.Name = ""
            Me.PropertyGrid.ToolStrip.Padding = New System.Windows.Forms.Padding(2, 0, 1, 0)
            Me.PropertyGrid.ToolStrip.Size = New System.Drawing.Size(387, 25)
            Me.PropertyGrid.ToolStrip.TabIndex = 1
            Me.PropertyGrid.ToolStrip.TabStop = True
            Me.PropertyGrid.ToolStrip.Text = "PropertyGridToolBar"
            '
            'WidgetPropertyGrid
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(387, 429)
            Me.CloseButtonVisible = False
            Me.Controls.Add(Me.PropertyGrid)
            Me.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "WidgetPropertyGrid"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
            Me.TabText = "Widget Properties"
            Me.Text = "Widget Properties"
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents PropertyGrid As PropertyGridEx.PropertyGridEx

    End Class
End Namespace