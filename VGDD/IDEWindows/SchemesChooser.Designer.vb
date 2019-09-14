Namespace VGDDIDE
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class SchemesChooser
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SchemesChooser))
            Me.btnApplyScheme = New System.Windows.Forms.Button()
            Me.cmbScheme = New System.Windows.Forms.ComboBox()
            Me._SchemePropertyGrid = New System.Windows.Forms.PropertyGrid()
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
            Me.mnuAction = New System.Windows.Forms.ToolStripMenuItem()
            Me.mnuNew = New System.Windows.Forms.ToolStripMenuItem()
            Me.mnuDelete = New System.Windows.Forms.ToolStripMenuItem()
            Me.mnuSetAsDefault = New System.Windows.Forms.ToolStripMenuItem()
            Me.mnuExport = New System.Windows.Forms.ToolStripMenuItem()
            Me.mnuImport = New System.Windows.Forms.ToolStripMenuItem()
            Me.MenuStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'btnApplyScheme
            '
            Me.btnApplyScheme.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnApplyScheme.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.btnApplyScheme.ForeColor = System.Drawing.Color.Green
            Me.btnApplyScheme.Location = New System.Drawing.Point(271, 0)
            Me.btnApplyScheme.Name = "btnApplyScheme"
            Me.btnApplyScheme.Size = New System.Drawing.Size(72, 21)
            Me.btnApplyScheme.TabIndex = 9
            Me.btnApplyScheme.Text = "Apply"
            Me.ToolTip1.SetToolTip(Me.btnApplyScheme, "Apply this scheme to the selected object(s)")
            Me.btnApplyScheme.UseVisualStyleBackColor = True
            '
            'cmbScheme
            '
            Me.cmbScheme.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbScheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbScheme.FormattingEnabled = True
            Me.cmbScheme.Location = New System.Drawing.Point(0, 0)
            Me.cmbScheme.Name = "cmbScheme"
            Me.cmbScheme.Size = New System.Drawing.Size(262, 21)
            Me.cmbScheme.TabIndex = 6
            Me.ToolTip1.SetToolTip(Me.cmbScheme, "Select the scheme to modify")
            '
            '_SchemePropertyGrid
            '
            Me._SchemePropertyGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me._SchemePropertyGrid.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me._SchemePropertyGrid.Location = New System.Drawing.Point(0, 24)
            Me._SchemePropertyGrid.Name = "_SchemePropertyGrid"
            Me._SchemePropertyGrid.Size = New System.Drawing.Size(343, 275)
            Me._SchemePropertyGrid.TabIndex = 10
            '
            'MenuStrip1
            '
            Me.MenuStrip1.BackColor = System.Drawing.SystemColors.Control
            Me.MenuStrip1.Dock = System.Windows.Forms.DockStyle.None
            Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAction})
            Me.MenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table
            Me.MenuStrip1.Location = New System.Drawing.Point(83, 24)
            Me.MenuStrip1.Name = "MenuStrip1"
            Me.MenuStrip1.ShowItemToolTips = True
            Me.MenuStrip1.Size = New System.Drawing.Size(98, 42)
            Me.MenuStrip1.TabIndex = 12
            Me.MenuStrip1.Text = "MenuStrip1"
            '
            'mnuAction
            '
            Me.mnuAction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.mnuAction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuNew, Me.mnuDelete, Me.mnuSetAsDefault, Me.mnuExport, Me.mnuImport})
            Me.mnuAction.Name = "mnuAction"
            Me.mnuAction.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded
            Me.mnuAction.Size = New System.Drawing.Size(54, 19)
            Me.mnuAction.Text = "Action"
            Me.mnuAction.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.mnuAction.ToolTipText = "Select actions on current Scheme"
            '
            'mnuNew
            '
            Me.mnuNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.mnuNew.Name = "mnuNew"
            Me.mnuNew.Size = New System.Drawing.Size(152, 22)
            Me.mnuNew.Text = "New"
            Me.mnuNew.ToolTipText = "Creates a new Scheme based on selected one"
            '
            'mnuDelete
            '
            Me.mnuDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.mnuDelete.Name = "mnuDelete"
            Me.mnuDelete.Size = New System.Drawing.Size(152, 22)
            Me.mnuDelete.Text = "Delete"
            Me.mnuDelete.ToolTipText = "Deletes the selected Scheme"
            '
            'mnuSetAsDefault
            '
            Me.mnuSetAsDefault.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.mnuSetAsDefault.Name = "mnuSetAsDefault"
            Me.mnuSetAsDefault.Size = New System.Drawing.Size(152, 22)
            Me.mnuSetAsDefault.Text = "Set as Default"
            Me.mnuSetAsDefault.ToolTipText = "Sets the selected Scheme as the default for future projects"
            '
            'mnuExport
            '
            Me.mnuExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.mnuExport.Name = "mnuExport"
            Me.mnuExport.Size = New System.Drawing.Size(152, 22)
            Me.mnuExport.Text = "Export"
            Me.mnuExport.ToolTipText = "Exports the selected Scheme to disk"
            '
            'mnuImport
            '
            Me.mnuImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.mnuImport.Name = "mnuImport"
            Me.mnuImport.Size = New System.Drawing.Size(152, 22)
            Me.mnuImport.Text = "Import"
            Me.mnuImport.ToolTipText = "Imports a saved Scheme from disk"
            '
            'SchemesChooser
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(346, 300)
            Me.CloseButtonVisible = False
            Me.Controls.Add(Me.MenuStrip1)
            Me.Controls.Add(Me.btnApplyScheme)
            Me.Controls.Add(Me.cmbScheme)
            Me.Controls.Add(Me._SchemePropertyGrid)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MainMenuStrip = Me.MenuStrip1
            Me.MinimumSize = New System.Drawing.Size(240, 134)
            Me.Name = "SchemesChooser"
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
            Me.TabText = "Schemes"
            Me.Text = "Schemes"
            Me.MenuStrip1.ResumeLayout(False)
            Me.MenuStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents btnApplyScheme As System.Windows.Forms.Button
        Friend WithEvents cmbScheme As System.Windows.Forms.ComboBox
        Friend WithEvents _SchemePropertyGrid As System.Windows.Forms.PropertyGrid
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
        Friend WithEvents mnuNew As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuDelete As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuSetAsDefault As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuExport As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuImport As System.Windows.Forms.ToolStripMenuItem
        Public WithEvents mnuAction As System.Windows.Forms.ToolStripMenuItem

    End Class
End Namespace