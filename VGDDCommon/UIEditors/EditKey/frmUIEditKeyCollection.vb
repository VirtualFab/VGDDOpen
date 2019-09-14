Imports System.Windows.Forms.Design
Imports System.ComponentModel
Imports VGDDCommon
Imports System.Windows.Forms
Imports System.Drawing

Public Class frmUIEditKeyCollection
    Public _wfes As IWindowsFormsEditorService
    Public context As ITypeDescriptorContext
    Private _KeysCollection As VGDDKeyCollection

    Public Sub New()
        InitializeComponent()
        Me.TopLevel = False
    End Sub

    Public Property Value As VGDDKeyCollection
        Get
            Return _KeysCollection
        End Get
        Set(ByVal value As VGDDKeyCollection)
            _KeysCollection = value
            If value IsNot Nothing Then
                LoadListview()
            End If
        End Set
    End Property

    Private Sub LoadListview()
        ListView1.Items.Clear()
        Dim i As Integer = 1
        For Each oKey As VGDDKey In Value
            Dim li As ListViewItem = ListView1.Items.Add(i)
            li.SubItems.Add(oKey.Key)
            li.SubItems.Add(oKey.KeyShift)
            li.SubItems.Add(oKey.KeyAlternate)
            li.SubItems.Add(oKey.KeyShiftAlternate)
            li.SubItems.Add(oKey.KeyCommand)
            i += 1
        Next
    End Sub

    Dim bCancelEdit As Boolean
    Dim CurrentSB As ListViewItem.ListViewSubItem
    Dim CurrentItem As ListViewItem

    'Private Sub ListView1_AfterLabelEdit(sender As Object, e As System.Windows.Forms.LabelEditEventArgs) Handles ListView1.AfterLabelEdit
    '    If CurrentItem.Index >= 0 Then
    '        _KeysCollection(CurrentItem.Index).Key = e.Label
    '    End If
    'End Sub

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
            Case 0, 1, 2, 3, 4, 5
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

            'ListView1.LabelEdit = True
            'CurrentItem.BeginEdit()     ' make sure the LabelEdit is set to True
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
            'If TextBox1.Text.Trim <> "" Then
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
                Case 1
                    _KeysCollection(CurrentItem.Index).Key = CurrentSB.Text
                Case 2
                    _KeysCollection(CurrentItem.Index).KeyShift = CurrentSB.Text
                Case 3
                    _KeysCollection(CurrentItem.Index).KeyAlternate = CurrentSB.Text
                Case 4
                    _KeysCollection(CurrentItem.Index).KeyShiftAlternate = CurrentSB.Text
                Case 5
                    _KeysCollection(CurrentItem.Index).KeyCommand = CurrentSB.Text
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
            'End If
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

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If TextBox1.Visible Then
            TextBox1_LostFocus(Nothing, Nothing)
        End If
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me._wfes.CloseDropDown()
    End Sub

    Private Sub frmUIEditKeyCollection_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Debug.Print(Me.DialogResult)
    End Sub

    Private Sub frmUIEditKeyCollection_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub ListView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
        btnRemove.Enabled = True
        btnMoveUp.Enabled = False
        btnMoveDown.Enabled = False
        If ListView1.SelectedIndices.Count > 0 Then
            btnRemove.Enabled = True
            If ListView1.SelectedIndices(0) > 0 Then
                btnMoveUp.Enabled = True
            End If
            If ListView1.SelectedIndices(0) < ListView1.Items.Count Then
                btnMoveDown.Enabled = True
            End If
        End If
    End Sub

    Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        For i As Integer = 0 To _KeysCollection.Count - 1
            Dim oKey As VGDDKey = _KeysCollection(i)
            If oKey.Key = ListView1.SelectedItems(0).Text Then
                _KeysCollection.RemoveAt(i)
                Exit For
            End If
        Next
        ListView1.Items.Remove(ListView1.SelectedItems(0))
    End Sub

    Private Sub btnMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMoveUp.Click, btnMoveDown.Click
        If ListView1.SelectedItems.Count = 0 Then Exit Sub
        Dim oItem As ListViewItem = ListView1.SelectedItems(0)
        Dim strSelected As String = oItem.SubItems(1).Text
        Dim Index As Integer = oItem.Index
        If Index >= 0 And Index <= ListView1.Items.Count Then
            ListView1.Items.Remove(oItem)
            If sender Is btnMoveUp Then
                Index -= 1
            Else
                Index += 1
            End If
            ListView1.Items.Insert(Index, oItem)
            For Index = 0 To ListView1.Items.Count - 1
                ListView1.Items(Index).Text = Index + 1
            Next
            _KeysCollection.Clear()
            For i As Integer = 0 To ListView1.Items.Count - 1
                oItem = ListView1.Items(i)
                Dim oKey As New VGDDKey
                With oKey
                    .Key = oItem.SubItems(1).Text
                    .KeyShift = oItem.SubItems(2).Text
                    .KeyAlternate = oItem.SubItems(3).Text
                    .KeyShiftAlternate = oItem.SubItems(4).Text
                    .KeyCommand = oItem.SubItems(5).Text
                End With
                _KeysCollection.Add(oKey)
            Next
            LoadListview()
            For Each oItem In ListView1.Items
                If oItem.SubItems(1).Text = strSelected Then
                    oItem.Selected = True
                    Exit Sub
                End If
            Next
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim oKey As New VGDDKey
        With oKey
            .Key = ""
            .KeyShift = ""
            .KeyShiftAlternate = ""
            .KeyAlternate = ""
            .KeyCommand = ""
        End With
        _KeysCollection.Add(oKey)
        LoadListview()
    End Sub
End Class