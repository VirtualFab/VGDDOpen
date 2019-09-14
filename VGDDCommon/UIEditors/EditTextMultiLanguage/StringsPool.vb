Imports System.Windows.Forms
Imports System.Data
Imports System.IO

Public Class StringsPool
    Inherits Form

    Private dtTranslations As DataTable

    Private _SelectOnly As Boolean
    Public SelectedStringID As Integer

    Public Sub ShowString(ByVal intStrId As Integer)
        SelectedStringID = intStrId
        Me.ShowDialog()
    End Sub

    Public Property SelectOnly As Boolean
        Get
            Return _SelectOnly
        End Get
        Set(ByVal value As Boolean)
            _SelectOnly = value
            pnlFunctions.Visible = Not _SelectOnly
        End Set
    End Property

    Private Sub StringsPool_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If btnSave.Visible Then
            If MessageBox.Show("You haven't saved your work yet. Exit anyway?", "Warning", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub StringsPool_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadFromPool()
    End Sub

    Private Sub LoadFromPool()
        dtTranslations = New DataTable
        With dtTranslations
            .Columns.Clear()
            .Columns.Add("STRINGID", GetType(System.Int32))
            .Columns.Add("INUSE", GetType(System.Boolean))
            .Columns.Add("FONT", GetType(System.String))
            .Columns.Add("AUTOWRAP", GetType(System.Boolean))
            .Columns.Add("DYNAMIC", GetType(System.Boolean))
            .Columns.Add("USEDBY", GetType(System.String))
            .Columns.Add("REFERENCE", GetType(System.String))
            .Columns.Add("STRINGALTID", GetType(System.String))
            Dim i As Integer
            For i = 1 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                .Columns.Add("TRANSLATION" & i.ToString, GetType(System.String))
            Next

            i = 1
            For Each oStringSet As VGDDCommon.MultiLanguageStringSet In VGDDCommon.Common.ProjectStringPool.Values
                Dim oRow As DataRow = dtTranslations.NewRow
                oRow("STRINGID") = oStringSet.StringID
                oRow("STRINGALTID") = oStringSet.StringAltID
                oRow("INUSE") = oStringSet.InUse
                oRow("AUTOWRAP") = oStringSet.AutoWrap
                oRow("DYNAMIC") = oStringSet.Dynamic
                If oStringSet.UsedBy Is Nothing Then oStringSet.UsedBy = String.Empty
                oRow("USEDBY") = oStringSet.UsedBy
                If oStringSet.FontName = String.Empty Then
                    Dim oFont As VGDDCommon.VGDDFont = VGDDCommon.Common._Fonts(0)
                    oRow("FONT") = oFont.Name
                Else
                    oRow("FONT") = oStringSet.FontName
                End If
                oRow("REFERENCE") = oStringSet.Strings(0)
                For j As Integer = 1 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                    If j < oStringSet.Strings.Length Then
                        oRow("TRANSLATION" & j.ToString) = oStringSet.Strings(j)
                    End If
                Next
                dtTranslations.Rows.Add(oRow)
                i += 1
            Next
        End With
        Dim dv As New DataView(dtTranslations)
        dv.Sort = "STRINGID ASC"
        dtTranslations = dv.ToTable()
        With dgv1
            .AllowUserToDeleteRows = False
            If SelectOnly Then .AllowUserToAddRows = False
            .AutoGenerateColumns = False
            .Columns.Clear()
            .Columns.Add("STRINGID", "String ID")
            .Columns("STRINGID").DataPropertyName = "STRINGID"
            .Columns("STRINGID").Width = 40
            Dim oInUseColumn As New DataGridViewCheckBoxColumn
            oInUseColumn.HeaderText = "In Use"
            oInUseColumn.Name = "INUSE"
            .Columns.Add(oInUseColumn)
            .Columns("INUSE").DataPropertyName = "INUSE"
            .Columns("INUSE").Width = 60
            .Columns.Add("USEDBY", "Used by screens")
            .Columns("USEDBY").DataPropertyName = "USEDBY"
            .Columns("USEDBY").Width = 200
            .Columns.Add("FONT", "Font")
            .Columns("FONT").DataPropertyName = "FONT"
            .Columns("FONT").Width = 180
            Dim oAutoWrapColumn As New DataGridViewCheckBoxColumn
            oAutoWrapColumn.HeaderText = "AutoWrap"
            oAutoWrapColumn.Name = "AUTOWRAP"
            .Columns.Add(oAutoWrapColumn)
            .Columns("AUTOWRAP").DataPropertyName = "AUTOWRAP"
            .Columns("AUTOWRAP").Width = 60
            Dim oDynamicColumn As New DataGridViewCheckBoxColumn
            oDynamicColumn.HeaderText = "Dynamic"
            oDynamicColumn.Name = "DYNAMIC"
            .Columns.Add(oDynamicColumn)
            .Columns("DYNAMIC").DataPropertyName = "DYNAMIC"
            .Columns("DYNAMIC").Width = 60
            .Columns.Add("REFERENCE", "Reference Text")
            .Columns("REFERENCE").DataPropertyName = "REFERENCE"
            .Columns("REFERENCE").Width = 200
            For i As Integer = 1 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                Dim strColName As String = "TRANSLATION" & i.ToString
                .Columns.Add(strColName, "Translation " & i.ToString)
                With .Columns(strColName)
                    .DataPropertyName = "TRANSLATION" & i.ToString
                    .Width = 200
                    .ReadOnly = True
                End With
            Next
            .Columns.Add("STRINGALTID", "String Alt ID")
            .Columns("STRINGALTID").DataPropertyName = "STRINGALTID"
            .Columns("STRINGALTID").Width = 200

            .DataSource = dtTranslations
            If VGDDCommon.Common.StringsPoolSortColumn <> String.Empty AndAlso dgv1.Columns(VGDDCommon.Common.StringsPoolSortColumn) IsNot Nothing Then
                .Sort(dgv1.Columns(VGDDCommon.Common.StringsPoolSortColumn), System.ComponentModel.ListSortDirection.Ascending)
            End If
            If SelectedStringID > 0 Then
                For Each oRow As DataGridViewRow In .Rows
                    If oRow.Cells(0).Value = SelectedStringID Then
                        .CurrentCell = oRow.Cells(0)
                        Exit For
                    End If
                Next
            End If
        End With
        btnSave.Visible = False
    End Sub

    Private intOldId As Integer = -1

    Private Sub dgv1_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles dgv1.CellBeginEdit
        If e.ColumnIndex = 0 Then
            intOldId = dgv1.SelectedCells(0).OwningRow.Cells(0).Value
        End If
    End Sub

    Private Sub DataGridView1_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgv1.CellDoubleClick
        If Not _SelectOnly AndAlso e.ColumnIndex > 3 AndAlso dgv1.SelectedCells.Count = 1 Then
            With pnlEditString
                .Dock = DockStyle.Fill
                .Visible = True
                Dim oRow As DataGridViewRow = dgv1.SelectedCells(0).OwningRow
                lblEditStringNumber.Text = "String #" & oRow.Cells(0).Value
                lblRefString.Text = oRow.Cells(6).Value.ToString.Replace(vbCrLf, "\n")
                .BringToFront()
                Dim oFont As VGDDCommon.VGDDFont = Nothing
                If Not IsDBNull(oRow.Cells(3).Value) Then
                    oFont = VGDDCommon.Common.GetFont(oRow.Cells(3).Value, Nothing)
                End If
                If oFont IsNot Nothing AndAlso oFont.Font IsNot Nothing Then
                    TextEditorControl1.Font = oFont.Font
                End If
                TextEditorControl1.Focus()
            End With
            If Not IsDBNull(dgv1.SelectedCells(0).Value) Then
                TextEditorControl1.Document.TextContent = dgv1.SelectedCells(0).Value
                'txtEditMultiLine.SelectionStart = txtEditMultiLine.Text.Length
                'txtEditMultiLine.ScrollToCaret()
            Else
                TextEditorControl1.Document.TextContent = String.Empty
            End If
        End If
    End Sub

    Public Sub ChangeStringId(ByVal OldId As Integer, ByVal NewId As Integer)
        For Each oScreenAttr As VGDD.VGDDScreenAttr In VGDDCommon.Common.aScreens.Values
            Dim oScreen As VGDD.VGDDScreen = oScreenAttr.Screen
            For Each oControl As Control In oScreen.Controls
                If TypeOf oControl Is VGDDMicrochip.VGDDWidget Then
                    Dim oWidget As VGDDMicrochip.VGDDWidget = oControl
                    If oWidget.TextStringID = OldId Then
                        VGDDCommon.Common.TouchScreen(oScreen.Name)
                        oWidget.TextStringID = NewId
                    End If
                End If
            Next
        Next
    End Sub

    Private Sub dgv1_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles dgv1.CellValidating
        If intOldId <> -1 AndAlso e.ColumnIndex = 0 Then
            Dim newID As Integer = e.FormattedValue
            If VGDDCommon.Common.ProjectStringPool.ContainsKey(newID) Then
                MessageBox.Show("String ID " & newID & " is already used!")
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub dgv1_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgv1.CellEndEdit
        If e.ColumnIndex = 0 Then
            Dim newID As Integer = dgv1.Rows(e.RowIndex).Cells(0).Value
            btnSave_Click(Nothing, Nothing)
            ChangeStringId(intOldId, newID)
            intOldId = -1
        Else
            btnSave.Visible = True
        End If
    End Sub

    Private Sub btnEditOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditOk.Click
        dgv1.SelectedCells(0).Value = TextEditorControl1.Document.TextContent
        pnlEditString.Visible = False

        btnSave_Click(Nothing, Nothing)

        'Dim oStringSet As VGDDCommon.MultiLanguageStringSet = VGDDCommon.Common.ProjectStringPool(dgv1.CurrentRow.Cells("STRINGID").Value)
        'oStringSet.Strings(dgv1.SelectedCells(0).ColumnIndex - 5) = TextEditorControl1.Document.TextContent
        'VGDDCommon.Common.ProjectChanged = True
    End Sub

    Private Sub btnNewString_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewString.Click
        Dim oRow As DataRow = dtTranslations.NewRow
        Dim newID As Integer
        If dtTranslations.Rows.Count > 0 Then
            newID = dtTranslations.Rows(dtTranslations.Rows.Count - 1)("STRINGID") + 1
        Else
            newID = 1
        End If
        oRow("STRINGID") = newID
        dtTranslations.Rows.Add(oRow)
        dgv1.CurrentCell = dgv1.Rows(dgv1.RowCount - 1).Cells(0)
        Dim oStringSet As New VGDDCommon.MultiLanguageStringSet
        oStringSet.StringID = newID
        Dim aStrings(VGDDCommon.Common.ProjectMultiLanguageTranslations) As String
        For i = 0 To VGDDCommon.Common.ProjectMultiLanguageTranslations
            aStrings(i) = String.Empty
        Next
        oStringSet.Strings = aStrings
        VGDDCommon.Common.ProjectStringPool.Add(newID, oStringSet)
        VGDDCommon.Common.ProjectChanged = True
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        VGDDCommon.Common.ProjectStringPool.Clear()
        Dim strLastFont As String = String.Empty
        For Each oRow As DataRow In dtTranslations.Rows
            Dim oStringSet As New VGDDCommon.MultiLanguageStringSet
            Dim aStrings(VGDDCommon.Common.ProjectMultiLanguageTranslations) As String
            If IsDBNull(oRow("REFERENCE")) Then oRow("REFERENCE") = String.Empty

            aStrings(0) = oRow("REFERENCE")
            For i = 1 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                If Not IsDBNull(oRow("TRANSLATION" & i.ToString)) Then
                    aStrings(i) = oRow("TRANSLATION" & i.ToString)
                End If
            Next
            oStringSet.StringID = oRow("STRINGID")
            If Not IsDBNull(oRow("STRINGALTID")) Then
                oStringSet.StringAltID = oRow("STRINGALTID")
            End If
            If Not IsDBNull(oRow("USEDBY")) Then
                oStringSet.UsedBy = oRow("USEDBY").ToString.Trim
            End If
            oStringSet.InUse = IIf(IsDBNull(oRow("INUSE")), False, oRow("INUSE"))
            oStringSet.AutoWrap = IIf(IsDBNull(oRow("AUTOWRAP")), False, oRow("AUTOWRAP"))
            oStringSet.Dynamic = IIf(IsDBNull(oRow("DYNAMIC")), False, oRow("DYNAMIC"))
            oStringSet.Strings = aStrings
            If Not IsDBNull(oRow("FONT")) Then
                oStringSet.FontName = oRow("FONT")
                strLastFont = oStringSet.FontName
            Else
                oStringSet.FontName = strLastFont
            End If
            VGDDCommon.Common.ProjectStringPool.Add(oRow("STRINGID"), oStringSet)
        Next
        'For Each oStringSet As VGDDCommon.MultiLanguageStringSet In VGDDCommon.Common.ProjectStringPool.Values
        '    If oStringSet.UsedBy <> String.Empty Then
        '        For Each strScreenName As String In oStringSet.UsedBy.Split(" ")
        '            VGDDCommon.Common.TouchScreen(strScreenName)
        '        Next
        '    End If
        'Next
        btnSave.Visible = False
        VGDDCommon.Common.ProjectChanged = True
    End Sub

    Private Sub dgv1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv1.Click
        btnRemoveString.Visible = dgv1.SelectedRows.Count > 0
    End Sub

    Private Sub dgv1_ColumnHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgv1.ColumnHeaderMouseClick
        If dgv1.Columns(e.ColumnIndex).Name <> VGDDCommon.Common.StringsPoolSortColumn Then
            dgv1.Sort(dgv1.Columns(e.ColumnIndex), System.ComponentModel.ListSortDirection.Ascending)
            VGDDCommon.Common.StringsPoolSortColumn = dgv1.Columns(e.ColumnIndex).Name
        Else
            dgv1.Sort(dgv1.Columns(e.ColumnIndex), System.ComponentModel.ListSortDirection.Descending)
            VGDDCommon.Common.StringsPoolSortColumn = String.Empty
        End If
    End Sub

    Private Sub dgv1_ColumnHeaderMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgv1.ColumnHeaderMouseDoubleClick
        If dgv1.Columns(e.ColumnIndex).Name = "INUSE" Then
            If MessageBox.Show("Do you want to reset all InUse flags?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                For Each oRow As DataGridViewRow In dgv1.Rows
                    oRow.Cells("INUSE").Value = False
                Next
                btnSave.Visible = True
                MessageBox.Show("Close and reopen the project to check which strings are actually referred by widgets")
            End If
        End If
    End Sub

    Private Sub DataGridView1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles dgv1.KeyPress
        If Not SelectOnly AndAlso Asc(e.KeyChar) = 4 AndAlso dgv1.SelectedRows.Count > 0 Then
            btnRemoveString.PerformClick()
        End If
    End Sub

    Private Sub dgv1_RowHeaderMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgv1.RowHeaderMouseDoubleClick
        'If SelectOnly Then
        SelectedStringID = dgv1.SelectedRows(0).Cells("STRINGID").Value
        Me.Close()
        'End If
    End Sub

    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        Dim dlg As New SaveFileDialog
        dlg.Title = "Export Strings Pool as CSV"
        dlg.DefaultExt = "csv"
        dlg.Filter = "Comma Separated Values Files|*.csv"
        dlg.FileName = VGDDCommon.Common.ProjectName & "StringsPool.csv"
        If (dlg.ShowDialog = DialogResult.OK) Then
            Dim strDelimiter As String = InputBox("CSV Delimiter:", "Use comma for USA, semicolon (;) for Europe", ",")
            If strDelimiter = String.Empty Then
                MessageBox.Show("Invalid delimiter")
                Exit Sub
            End If
            Using sw As New StreamWriter(dlg.FileName, False, New System.Text.UnicodeEncoding)
                Dim strLine As String = String.Format("STRING_ID{0}INUSE{0}FONT{0}AUTOWRAP{0}DYNAMIC{0}REFERENCE", strDelimiter)
                For i As Integer = 1 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                    strLine &= String.Format("{0}TRANSLATION{1}", strDelimiter, i.ToString)
                Next
                strLine &= String.Format("{0}STRING_ALTID", strDelimiter)
                sw.WriteLine(strLine)
                For Each oRow As DataRow In dtTranslations.Rows
                    strLine = String.Format("{1}{0}""{2}""{0}""{3}""{0}""{4}""{0}""{5}""{0}""{6}""", strDelimiter, oRow("STRINGID"), oRow("INUSE"), oRow("FONT"), oRow("AUTOWRAP"), oRow("DYNAMIC"), oRow("REFERENCE").ToString.Replace(vbCrLf, "\n"))
                    For i As Integer = 1 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                        strLine &= String.Format("{0}""{1}""", strDelimiter, oRow("TRANSLATION" & i.ToString).ToString.Replace(vbCrLf, "\n"))
                    Next
                    strLine &= String.Format("{0}""{1}""", strDelimiter, oRow("STRINGALTID"))
                    sw.WriteLine(strLine)
                Next
                sw.Close()
            End Using
        End If
    End Sub

    Private Sub btnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImport.Click
        Dim dlg As New OpenFileDialog
        dlg.Title = "Import Strings Pool from CSV"
        dlg.DefaultExt = "csv"
        dlg.Filter = "Comma Separated Values Files|*.csv; *.txt"
        If (dlg.ShowDialog = DialogResult.OK) Then
            Try
                VGDDCommon.Common.ProjectStringPool.Clear()
                Dim sr As New StreamReader(dlg.FileName)
                Dim strHeaders() As String
                Dim strHeaderLine As String = sr.ReadLine
                Dim strSeparator = ","
                strHeaders = strHeaderLine.Split(strSeparator)
                If strHeaders.Length < 2 Then
                    strSeparator = ";"
                    strHeaders = strHeaderLine.Split(strSeparator)
                    If strHeaders.Length < 2 Then
                        strSeparator = vbTab
                        strHeaders = strHeaderLine.Split(strSeparator)
                    End If
                End If
                If strHeaders.Length < 2 Then
                    MessageBox.Show("Cannot find a valid separator to decode CSV:" & vbCrLf & strHeaderLine)
                    Exit Sub
                End If
                Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(sr)
                    MyReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
                    MyReader.Delimiters = New String() {strSeparator}
                    Dim currentRow As String()
                    While Not MyReader.EndOfData
                        Dim aStrings(VGDDCommon.Common.ProjectMultiLanguageTranslations) As String
                        Try
                            currentRow = MyReader.ReadFields()
                            Dim oStringSet As New VGDDCommon.MultiLanguageStringSet
                            oStringSet.StringID = currentRow(0)
                            oStringSet.InUse = currentRow(1)
                            oStringSet.FontName = currentRow(2)
                            oStringSet.AutoWrap = currentRow(3)
                            oStringSet.Dynamic = currentRow(4)
                            For i = 0 To VGDDCommon.Common.ProjectMultiLanguageTranslations
                                If currentRow.Length > i + 5 Then
                                    aStrings(i) = currentRow(i + 5).Replace("\n", vbCrLf)
                                End If
                            Next
                            oStringSet.Strings = aStrings
                            If currentRow.Length > VGDDCommon.Common.ProjectMultiLanguageTranslations + 5 + 1 Then
                                oStringSet.StringAltID = currentRow(VGDDCommon.Common.ProjectMultiLanguageTranslations + 5 + 1)
                            End If
                            VGDDCommon.Common.ProjectStringPool.Add(oStringSet.StringID, oStringSet)
                        Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
                            MessageBox.Show("Line " & ex.Message & " is invalid. Skipping")
                        End Try
                    End While
                End Using
                VGDDCommon.Common.ProjectChanged = True
                LoadFromPool()
                dgv1.Refresh()

            Catch ex As Exception
                MessageBox.Show("Error importing CSV:" & ex.Message)
            End Try
        End If
    End Sub

    Private Sub btnRemoveString_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveString.Click
        If MessageBox.Show("Are sure you want to remove " & dgv1.SelectedRows.Count & " strings?", "Delete confirmation", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            For Each oRow As DataGridViewRow In dgv1.SelectedRows
                dgv1.Rows.Remove(oRow)
            Next
            btnSave.Visible = True
            btnRemoveString.Visible = False
        End If
    End Sub

    Private Sub btnRenumber_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRenumber.Click
        dgv1.Sort(dgv1.Columns(0), System.ComponentModel.ListSortDirection.Ascending)
        VGDDCommon.Common.StringsPoolSortColumn = "ID"
        Application.DoEvents()
        If MessageBox.Show("Do you want to renumber all strings IDs?", "Warning Experimental feature!", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            Dim i As Integer = 1
            For Each oRow As DataGridViewRow In dgv1.Rows
                ChangeStringId(oRow.Cells(0).Value, i)
                oRow.Cells(0).Value = i
                i += 1
            Next
            btnSave_Click(Nothing, Nothing)
        End If
    End Sub
End Class