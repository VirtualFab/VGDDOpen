Imports System.Windows.Forms.Design
Imports System.ComponentModel
Imports VGDDCommon
Imports System.Windows.Forms
Imports System.Drawing

Public Class frmUIEditTextMultiLanguage
    Public _wfes As IWindowsFormsEditorService
    Public provider As IServiceProvider
    Public context As ITypeDescriptorContext
    Public _Value As String

    Private OldText As String

    Public Sub New()
        InitializeComponent()
        Me.TopLevel = False
    End Sub

    Private Sub frmUIEditTextMultiLanguage_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim oStringSet As MultiLanguageStringSet
        Dim aStringPool() As String
        If Common.ProjectStringPool.ContainsKey(_Value) Then
            oStringSet = Common.ProjectStringPool.Item(_Value)
            OldText = oStringSet.Strings(0)
            aStringPool = oStringSet.Strings
            Dim oFont As VGDDFont = Common.GetFont(oStringSet.FontName, Nothing)
            If oFont IsNot Nothing AndAlso oFont.Font IsNot Nothing Then
                TextEditorControl1.Font = oFont.Font
            End If

            If aStringPool.Length < Common.ProjectMultiLanguageTranslations Then
                ReDim Preserve aStringPool(Common.ProjectMultiLanguageTranslations)
            End If
            For i = 1 To aStringPool.Length - 1
                Dim oItem As ListViewItem = ListView1.Items.Add(i)
                If aStringPool(i) IsNot Nothing Then
                    oItem.SubItems.Add(aStringPool(i).Replace(vbCrLf, "\n"))
                End If
            Next
        Else
            oStringSet = New MultiLanguageStringSet
            ReDim aStringPool(Common.ProjectMultiLanguageTranslations)
            oStringSet.Strings = aStringPool
            For i = 1 To 99999
                If Not Common.ProjectStringPool.ContainsKey(i) Then
                    aStringPool(0) = context.Instance.text
                    Common.ProjectStringPool.Add(i, oStringSet)
                    _Value = i
                    Exit For
                End If
            Next
            For i = 1 To Common.ProjectMultiLanguageTranslations
                Dim oItem As ListViewItem = ListView1.Items.Add(i)
                oItem.SubItems.Add(String.Empty)
            Next
        End If
        txtRefVal.Text = aStringPool(0).Replace(vbCrLf, "\n")
        lblStringNumber.Text = "String #" & _Value.ToString
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Public Property Value As String
        Get
            Return _Value
        End Get
        Set(ByVal value As String)
            _Value = value
        End Set
    End Property

    Dim bCancelEdit As Boolean
    Dim CurrentSB As ListViewItem.ListViewSubItem
    Dim CurrentItem As ListViewItem

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Me.Close()
    End Sub

    Private Sub frmUIEditTextMultiLanguage_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim oStringSet As MultiLanguageStringSet = Nothing
        If Not Common.ProjectStringPool.ContainsKey(_Value) Then
            For Each oStringSet In Common.ProjectStringPool.Values
                If oStringSet.Strings(0) = OldText Then
                    _Value = oStringSet.StringID
                    Exit For
                End If
            Next
            If Not Common.ProjectStringPool.ContainsKey(_Value) Then Exit Sub
        Else
            oStringSet = Common.ProjectStringPool.Item(_Value)
        End If
        Dim aStrings() As String = oStringSet.Strings
        aStrings(0) = txtRefVal.Text.Replace("\n", vbCrLf)
        For Each oItem As ListViewItem In ListView1.Items
            If oItem.SubItems.Count > 1 Then
                aStrings(oItem.Text) = oItem.SubItems(1).Text.Replace("\n", vbCrLf)
            End If
        Next
        Common.ProjectChanged = True
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me._wfes.CloseDropDown()
    End Sub

    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        StartEditString()
    End Sub

    Private Sub txtRefVal_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtRefVal.MouseDoubleClick
        ListView1.SelectedItems.Clear()
        StartEditString()
    End Sub

    Private Sub StartEditString()
        If ListView1.SelectedItems.Count = 1 Then
            lblEditStringNumber.Text = lblStringNumber.Text
            If ListView1.SelectedItems(0).SubItems.Count > 1 Then
                TextEditorControl1.Document.TextContent = ListView1.SelectedItems(0).SubItems(1).Text.Replace("\n", vbCrLf)
            Else
                TextEditorControl1.Document.TextContent = txtRefVal.Text.Replace("\n", vbCrLf)
            End If
        Else
            lblEditStringNumber.Text = "Ref. String"
            TextEditorControl1.Document.TextContent = txtRefVal.Text.Replace("\n", vbCrLf)
        End If
        lblRefString.Text = txtRefVal.Text.Replace(vbCrLf, "\n")
        With pnlEditString
            .Dock = DockStyle.Fill
            .Visible = True
        End With
        TextEditorControl1.Focus()
    End Sub

    Private Sub btnEditOk_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditOk.Click
        If ListView1.SelectedItems.Count = 1 Then
            If ListView1.SelectedItems(0).SubItems.Count = 1 Then
                ListView1.SelectedItems(0).SubItems.Add(TextEditorControl1.Document.TextContent.Replace(vbCrLf, "\n"))
            Else
                ListView1.SelectedItems(0).SubItems(1).Text = TextEditorControl1.Document.TextContent.Replace(vbCrLf, "\n")
            End If
        Else
            txtRefVal.Text = TextEditorControl1.Document.TextContent.Replace(vbCrLf, "\n")
        End If
        pnlEditString.Visible = False
        Common.ProjectChanged = True
    End Sub

    Private Sub lblStringNumber_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblStringNumber.Click
        Dim oSp As New StringsPool
        oSp.SelectOnly = False
        oSp.ShowString(_Value)
        If oSp.SelectedStringID > 0 Then
            _Value = oSp.SelectedStringID
            If Common.ProjectStringPool.ContainsKey(_Value) Then
                Dim aStringPool() As String = Common.ProjectStringPool.Item(_Value).Strings
                txtRefVal.Text = aStringPool(0).Replace(vbCrLf, "\n")
                For Each oItem As ListViewItem In ListView1.Items
                    If oItem.SubItems.Count > 1 Then
                        If aStringPool(oItem.Text) Is Nothing Then aStringPool(oItem.Text) = String.Empty
                        oItem.SubItems(1).Text = aStringPool(oItem.Text).Replace(vbCrLf, "\n")
                    End If
                Next
            End If
            Me.Close()
        End If
    End Sub
End Class