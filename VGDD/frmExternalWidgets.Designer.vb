<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmExternalWidgets
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmExternalWidgets))
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.Library = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Author = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Loaded = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtnNewLib = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnUnloadLib = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripReload = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripEnterLicense = New System.Windows.Forms.ToolStripButton()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ListViewImages = New System.Windows.Forms.ListView()
        Me.Licensed = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ToolStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListView1
        '
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Library, Me.Author, Me.Loaded, Me.Licensed})
        Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView1.Location = New System.Drawing.Point(0, 0)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(527, 249)
        Me.ListView1.TabIndex = 0
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'Library
        '
        Me.Library.Text = "Library Name"
        Me.Library.Width = 200
        '
        'Author
        '
        Me.Author.Text = "Author"
        Me.Author.Width = 120
        '
        'Loaded
        '
        Me.Loaded.Text = "Loaded"
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.ImageList1.ImageSize = New System.Drawing.Size(16, 16)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtnNewLib, Me.ToolStripBtnUnloadLib, Me.ToolStripSeparator2, Me.ToolStripReload, Me.ToolStripSeparator1, Me.ToolStripEnterLicense})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(869, 25)
        Me.ToolStrip1.TabIndex = 3
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtnNewLib
        '
        Me.ToolStripBtnNewLib.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnNewLib.Image = CType(resources.GetObject("ToolStripBtnNewLib.Image"), System.Drawing.Image)
        Me.ToolStripBtnNewLib.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnNewLib.Name = "ToolStripBtnNewLib"
        Me.ToolStripBtnNewLib.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnNewLib.Text = "ToolStripButton1"
        Me.ToolStripBtnNewLib.ToolTipText = "Load new Library"
        '
        'ToolStripBtnUnloadLib
        '
        Me.ToolStripBtnUnloadLib.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnUnloadLib.Enabled = False
        Me.ToolStripBtnUnloadLib.Image = Global.My.Resources.Resources.minus
        Me.ToolStripBtnUnloadLib.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnUnloadLib.Name = "ToolStripBtnUnloadLib"
        Me.ToolStripBtnUnloadLib.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnUnloadLib.Text = "ToolStripButton1"
        Me.ToolStripBtnUnloadLib.ToolTipText = "Unload Library"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripReload
        '
        Me.ToolStripReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripReload.Image = Global.My.Resources.Resources.refresh
        Me.ToolStripReload.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripReload.Name = "ToolStripReload"
        Me.ToolStripReload.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripReload.Text = "Reload Libraries"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripEnterLicense
        '
        Me.ToolStripEnterLicense.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripEnterLicense.Image = Global.My.Resources.Resources.librarybookmarked
        Me.ToolStripEnterLicense.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripEnterLicense.Name = "ToolStripEnterLicense"
        Me.ToolStripEnterLicense.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripEnterLicense.Text = "ToolStripButton1"
        Me.ToolStripEnterLicense.ToolTipText = "Enter License Code"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 28)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.ListView1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.ListViewImages)
        Me.SplitContainer1.Size = New System.Drawing.Size(869, 249)
        Me.SplitContainer1.SplitterDistance = 527
        Me.SplitContainer1.TabIndex = 5
        '
        'ListViewImages
        '
        Me.ListViewImages.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListViewImages.Location = New System.Drawing.Point(0, 0)
        Me.ListViewImages.Name = "ListViewImages"
        Me.ListViewImages.Size = New System.Drawing.Size(338, 249)
        Me.ListViewImages.TabIndex = 0
        Me.ListViewImages.UseCompatibleStateImageBehavior = False
        '
        'Licensed
        '
        Me.Licensed.Text = "Licensed"
        '
        'frmExternalWidgets
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(869, 273)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmExternalWidgets"
        Me.Text = "External Widgets Management"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents Library As System.Windows.Forms.ColumnHeader
    Friend WithEvents Author As System.Windows.Forms.ColumnHeader
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripBtnNewLib As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripBtnUnloadLib As System.Windows.Forms.ToolStripButton
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents ListViewImages As System.Windows.Forms.ListView
    Friend WithEvents Loaded As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolStripReload As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripEnterLicense As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Licensed As System.Windows.Forms.ColumnHeader
End Class
