Imports System.Windows.Forms.Design
Imports System.ComponentModel
Imports VGDDCommon
Imports System.Windows.Forms
Imports System.Drawing
Imports VGDDMicrochip

Public Class frmUIEditShapeDefinition
    Public _wfes As IWindowsFormsEditorService
    Public Event ValueChanged()
    Public context As ITypeDescriptorContext
    Private _ShapeDefinition As VGDDShapeDefinition
    Private UpdateProperty As Boolean = True
    Public UpdateFromTrackBar As Boolean = False
    Public UpdateFromUpDown As Boolean = False

    Public Sub New()
        InitializeComponent()
        'Me.TopLevel = False
    End Sub

    Private Sub frmUIEditShapeDefinition_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RenderImage()
    End Sub

    Private Sub frmUIEditShapeDefinition_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        btnPasteSVG.Enabled = False
        If Clipboard.ContainsData(DataFormats.Text.ToString) Then
            Dim strSVGXml As String = Clipboard.GetData(DataFormats.Text.ToString)
            If strSVGXml.Contains("<svg") Then
                btnPasteSVG.Enabled = True
            End If
        End If
    End Sub

    Private Sub btnPasteSVG_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPasteSVG.Click
        Dim strSVGXml As String = Clipboard.GetData(DataFormats.Text.ToString)
        If strSVGXml.Contains("<svg") Then
            Dim oNewShape As VGDDShapeDefinition = VGDDShapeDefinition.FromSVG(strSVGXml)
            For Each oItem As VGDDShapeItem In oNewShape
                Me._ShapeDefinition.Add(oItem)
            Next
            RenderImage()
        End If
    End Sub

    Private Sub RenderImage()
        If PictureBox1.Image IsNot Nothing Then
            PictureBox1.Image.Dispose()
        End If
        PictureBox1.Image = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        If _ShapeDefinition IsNot Nothing Then
            Dim g As Graphics = Graphics.FromImage(PictureBox1.Image)
            'g.DrawLine(Pens.Red, 0, 0, PictureBox1.Width, PictureBox1.Height)
            'g.DrawLine(Pens.Red, 0, PictureBox1.Height, PictureBox1.Width, 0)
            For Each oShape As VGDDShapeItem In _ShapeDefinition
                Select Case oShape.ShapeType
                    Case VGDDShapeItem.VGDDShapeTypes.Line
                        If oShape.Data IsNot Nothing Then
                            Dim aLineData() As Integer = oShape.Data
                            g.DrawLine(New Pen(oShape.Color), aLineData(0), aLineData(1), aLineData(2), aLineData(3))
                        End If
                    Case VGDDShapeItem.VGDDShapeTypes.Circle
                        If oShape.Data IsNot Nothing Then
                            Dim aLineData() As Integer = oShape.Data
                            g.DrawArc(New Pen(oShape.Color), aLineData(0), aLineData(1), aLineData(2), aLineData(3), 0, 360)
                        End If
                    Case VGDDShapeItem.VGDDShapeTypes.PolyLine
                        If oShape.Data IsNot Nothing Then
                            Dim aLineData() As Point = oShape.Data
                            g.DrawPolygon(New Pen(oShape.Color), aLineData)
                        End If
                End Select
            Next
        End If
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
    End Sub

    Public Property Value As VGDDShapeDefinition
        Get
            Return _ShapeDefinition
        End Get
        Set(ByVal value As VGDDShapeDefinition)
            _ShapeDefinition = value
            If value IsNot Nothing Then
                ListView1.Items.Clear()
                For Each oItem As VGDDShapeItem In value
                    Dim li As ListViewItem = ListView1.Items.Add(oItem.ShapeType.ToString)
                    li.SubItems.Add(oItem.Color.ToString)
                    li.SubItems.Add(oItem.Data.ToString)
                Next
            End If
        End Set
    End Property

    Dim bCancelEdit As Boolean
    Dim CurrentSB As ListViewItem.ListViewSubItem
    Dim CurrentItem As ListViewItem

    Private Sub ListView1_AfterLabelEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.LabelEditEventArgs) Handles ListView1.AfterLabelEdit
        If CurrentItem.Index >= 0 Then
            _ShapeDefinition(CurrentItem.Index).ShapeType = e.Label
        End If
    End Sub

    Private Sub LV_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ListView1.KeyDown
        ' This subroutine is for starting editing when keyboard shortcut is pressed (e.g. F2 key)
        If ListView1.SelectedItems.Count = 0 Then Exit Sub
        Select Case e.KeyCode
            Case Keys.F2    ' F2 key is pressed. Initiate editing
                e.Handled = True
                BeginEditListItem(ListView1.SelectedItems(0), 2)
        End Select
    End Sub

    Private Sub ListView1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDoubleClick
        ' This subroutine checks where the double-clicking was performed and
        ' initiates in-line editing if user double-clicked on the right subitem

        ' check where clicked
        CurrentItem = ListView1.GetItemAt(e.X, e.Y)     ' which listviewitem was clicked
        If CurrentItem Is Nothing Then Exit Sub
        CurrentSB = CurrentItem.GetSubItemAt(e.X, e.Y)  ' which subitem was clicked
        ' See which column has been clicked
        ' NOTE: This portion is important. Here you can define your own rules as to which column can be edited and which cannot.
        Dim iSubIndex As Integer = CurrentItem.SubItems.IndexOf(CurrentSB)
        Select Case iSubIndex
            Case 0, 1, 2
                ' These two columns are allowed to be edited. So continue the code
            Case Else
                ' In my example I have defined that only "Runs" and "Wickets" columns can be edited by user
                Exit Sub
        End Select

        ' Check if the first subitem is being edited
        If iSubIndex = 0 Then
            ' There's a slight coding difficulty here. If the first item is to be edited
            ' then when you get the Bounds of the first subitem, it returns the Bounds of
            ' the entire ListViewItem. Hence the Textbox looks very wierd. In order to allow
            ' editing on the first column, we use the built-in editing method.

            ListView1.LabelEdit = True
            CurrentItem.BeginEdit()     ' make sure the LabelEdit is set to True
            Exit Sub
        End If

        Dim lLeft = CurrentSB.Bounds.Left + 2
        Dim lWidth As Integer = CurrentSB.Bounds.Width
        With TextBox1
            .SetBounds(lLeft + ListView1.Left, CurrentSB.Bounds.Top + ListView1.Top, lWidth, CurrentSB.Bounds.Height)
            .Text = CurrentSB.Text
            .Show()
            .Focus()
        End With
    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        ' This subroutine closes the text box
        Select Case e.KeyChar
            Case Microsoft.VisualBasic.ChrW(Keys.Return)    ' Enter key is pressed
                bCancelEdit = False ' editing completed
                e.Handled = True
                TextBox1.Hide()
            Case Microsoft.VisualBasic.ChrW(Keys.Escape)    ' Escape key is pressed
                bCancelEdit = True  ' editing was cancelled
                e.Handled = True
                TextBox1.Hide()
        End Select
    End Sub

    Private Sub TextBox1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.LostFocus
        TextBox1.Hide()
        If bCancelEdit = False Then
            If TextBox1.Text.Trim <> "" Then
                ' NOTE: You can define your validation rules here. In my example I've set that only numbers can be entered in "Runs" and "Wickets" column
                '' Validate
                'If IsNumeric(TextBox1.Text) = False Then
                '    MsgBox("Please enter a numeric value in this field.", MsgBoxStyle.Exclamation)
                '    Exit Sub
                'End If

                ' update listviewitem
                CurrentSB.Text = TextBox1.Text ' CInt(TextBox1.Text).ToString("#,###0")

                ' save changes so that next time you load this XML document the changes are there.
                Dim iSubIndex As Integer = CurrentItem.SubItems.IndexOf(CurrentSB)
                Select Case iSubIndex
                    Case 0
                        _ShapeDefinition(CurrentItem.Index).ShapeType = CurrentItem.Text
                    Case 1
                        '_ShapeCollection(CurrentItem.Index).Color = CurrentSB.Text
                    Case 1
                        _ShapeDefinition(CurrentItem.Index).Data = CurrentSB.Text
                End Select
                'Dim szPropertyName As String
                'If iSubIndex = 2 Then
                '    szPropertyName = "Runs"
                'Else
                '    szPropertyName = "Wickets"
                'End If
                'SaveXMLChanges(CurrentItem.Text, szPropertyName, CurrentSB.Text)

                '' Recalculate last column
                'CalculateAverage()
            End If
        Else
            ' Editing was cancelled by user
            bCancelEdit = False
        End If
        ListView1.Focus()
    End Sub

    Private Sub BeginEditListItem(ByVal iTm As ListViewItem, ByVal SubItemIndex As Integer)
        ' This subroutine is for manually initiating editing instead of mouse double-clicks

        Dim pt As Point = iTm.SubItems(SubItemIndex).Bounds.Location
        Dim ee As New System.Windows.Forms.MouseEventArgs(Windows.Forms.MouseButtons.Left, 2, pt.X, pt.Y, 0)
        Call ListView1_MouseDoubleClick(ListView1, ee)
    End Sub

End Class