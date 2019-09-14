Imports System.ComponentModel

Namespace VGDDMicrochip
#If Not PlayerMonolitico Then

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ComboBox
        Inherits VGDDMicrochip.VGDDWidget

        Friend Shared _Instances As Integer = 0

        'UserControl1 overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Instances As Integer
            Get
                Return _Instances
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Demolimit As Integer
            Get
                Return VGDDCommon.Common.DEMOCODELIMIT
            End Get
        End Property

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ComboBox))
            Me.Slider1 = New VGDDMicrochip.Slider()
            Me.ListBox1 = New VGDDMicrochip.ListBox()
            Me.btnOpenClose = New VGDDMicrochip.Button()
            Me.btnSlideUp = New VGDDMicrochip.Button()
            Me.btnSlideDown = New VGDDMicrochip.Button()
            Me.StaticText1 = New VGDDMicrochip.StaticText()
            Me.SuspendLayout()
            '
            'Slider1
            '
            Me.Slider1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Slider1.Animated = False
            Me.Slider1.DesignerLocked = False
            Me.Slider1.Location = New System.Drawing.Point(492, 44)
            Me.Slider1.Name = "Slider1"
            Me.Slider1.Orientation = VGDDCommon.Common.Orientation.Vertical
            Me.Slider1.Page = CType(1, Short)
            Me.Slider1.Pos = CType(30, Short)
            Me.Slider1.Range = CType(100, Short)
            Me.Slider1.Scheme = "DesignScheme"
            Me.Slider1.Size = New System.Drawing.Size(30, 263)
            Me.Slider1.SliderType = VGDDCommon.Common.SliderType.ScrollBar
            Me.Slider1.VGDDEvents = CType(resources.GetObject("Slider1.VGDDEvents"), VGDDCommon.VGDDEventsCollection)
            Me.Slider1.Zorder = 3
            '
            'ListBox1
            '
            Me.ListBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ListBox1.DesignerLocked = False
            Me.ListBox1.Location = New System.Drawing.Point(0, 21)
            Me.ListBox1.Name = "ListBox1"
            Me.ListBox1.Scheme = "DesignScheme"
            Me.ListBox1.Size = New System.Drawing.Size(491, 303)
            Me.ListBox1.VGDDEvents = CType(resources.GetObject("ListBox1.VGDDEvents"), VGDDCommon.VGDDEventsCollection)
            Me.ListBox1.Zorder = 2
            '
            'btnOpenClose
            '
            Me.btnOpenClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnOpenClose.Bitmap = "bmpbtnOpenClose"
            Me.btnOpenClose.DesignerLocked = False
            Me.btnOpenClose.Location = New System.Drawing.Point(492, 0)
            Me.btnOpenClose.Name = "btnOpenClose"
            Me.btnOpenClose.ParentTransparentColour = System.Drawing.Color.Empty
            Me.btnOpenClose.Scale = CType(1, Byte)
            Me.btnOpenClose.Scheme = "DesignScheme"
            Me.btnOpenClose.Size = New System.Drawing.Size(31, 24)
            Me.btnOpenClose.VGDDEvents = CType(resources.GetObject("btnOpenClose.VGDDEvents"), VGDDCommon.VGDDEventsCollection)
            Me.btnOpenClose.Zorder = 1
            '
            'btnSlideUp
            '
            Me.btnSlideUp.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSlideUp.Bitmap = "bmpbtnSlideUp"
            Me.btnSlideUp.DesignerLocked = False
            Me.btnSlideUp.Location = New System.Drawing.Point(492, 24)
            Me.btnSlideUp.Name = "btnSlideUp"
            Me.btnSlideUp.ParentTransparentColour = System.Drawing.Color.Empty
            Me.btnSlideUp.Scale = CType(1, Byte)
            Me.btnSlideUp.Scheme = "DesignScheme"
            Me.btnSlideUp.Size = New System.Drawing.Size(31, 20)
            Me.btnSlideUp.VGDDEvents = CType(resources.GetObject("btnSlideUp.VGDDEvents"), VGDDCommon.VGDDEventsCollection)
            Me.btnSlideUp.Zorder = 4
            '
            'btnSlideDown
            '
            Me.btnSlideDown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSlideDown.Bitmap = "bmpbtnSlideDown"
            Me.btnSlideDown.DesignerLocked = False
            Me.btnSlideDown.Location = New System.Drawing.Point(492, 306)
            Me.btnSlideDown.Name = "btnSlideDown"
            Me.btnSlideDown.ParentTransparentColour = System.Drawing.Color.Empty
            Me.btnSlideDown.Scale = CType(1, Byte)
            Me.btnSlideDown.Scheme = "DesignScheme"
            Me.btnSlideDown.Size = New System.Drawing.Size(31, 19)
            Me.btnSlideDown.VGDDEvents = CType(resources.GetObject("btnSlideDown.VGDDEvents"), VGDDCommon.VGDDEventsCollection)
            Me.btnSlideDown.Zorder = 5
            '
            'StaticText1
            '
            Me.StaticText1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.StaticText1.DesignerLocked = False
            Me.StaticText1.Location = New System.Drawing.Point(0, 0)
            Me.StaticText1.Name = "StaticText1"
            Me.StaticText1.Scheme = "DesignScheme"
            Me.StaticText1.Size = New System.Drawing.Size(492, 21)
            Me.StaticText1.VGDDEvents = CType(resources.GetObject("StaticText1.VGDDEvents"), VGDDCommon.VGDDEventsCollection)
            Me.StaticText1.Zorder = 6
            '
            'ComboBox
            '
            Me.Controls.Add(Me.StaticText1)
            Me.Controls.Add(Me.btnSlideDown)
            Me.Controls.Add(Me.btnSlideUp)
            Me.Controls.Add(Me.Slider1)
            Me.Controls.Add(Me.ListBox1)
            Me.Controls.Add(Me.btnOpenClose)
            Me.Name = "ComboBox"
            Me.Size = New System.Drawing.Size(523, 328)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents btnOpenClose As VGDDMicrochip.Button
        Friend WithEvents ListBox1 As VGDDMicrochip.ListBox
        Friend WithEvents Slider1 As VGDDMicrochip.Slider
        Friend WithEvents btnSlideUp As VGDDMicrochip.Button
        Friend WithEvents btnSlideDown As VGDDMicrochip.Button
        Friend WithEvents StaticText1 As VGDDMicrochip.StaticText

    End Class

#End If
End Namespace
