Imports System.Xml
Imports System.IO
Imports VGDDCommon

'Namespace VGDDCommon

Public Class frmMplabXTemplatesEditor

    Private SelectedControlXmlNode As XmlNode
    Private SelectedTreeNode As TreeNode
    Private dtStates As New DataTable
    Private oCustProp As VGDDCustomProp

    Private Sub frmCustomWidgetEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Common.MplabXLoadAndMergeTemplates() Then
            Me.Close()
            Exit Sub
        End If
        RefreshTreeView()
    End Sub

    Private Sub btnReload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReload.Click
        RefreshTreeView()
    End Sub

    Private Sub btnSaveTemplate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveTemplate.Click
        Dim sw As New StringWriter
        Dim xtw As New XmlTextWriter(sw)
        xtw.Formatting = Formatting.Indented
        Common.XmlMplabxTemplatesDoc.WriteTo(xtw)
        If Common.WriteFileWithBackup(Common.MplabXTemplateFile, sw.ToString, New System.Text.UTF8Encoding) Then
            btnSaveTemplate.Visible = False
        End If
    End Sub

    Private Sub RefreshTreeView()
        tvTemplates.Nodes.Clear()
        Dim oNode As XmlNode = Common.XmlMplabxTemplatesDoc.DocumentElement
        Dim oNewTreeNode As TreeNode = tvTemplates.Nodes.Add(oNode.Name)
        oNewTreeNode.Tag = oNode
        AddTreeViewChildNodes(tvTemplates.TopNode, oNode)
        tvTemplates.ExpandAll()
    End Sub

    Private Sub AddTreeViewChildNodes(ByRef ParentTreeNode As TreeNode, ByRef oXmlNode As XmlNode)
        For Each oXmlChildNode As XmlNode In oXmlNode.ChildNodes
            'If oXmlChildNode.NodeType <> XmlNodeType.Whitespace Then
            Dim blnUseNode As Boolean = True
            Dim strNodeText As String = oXmlChildNode.Name
            Select Case strNodeText
                Case "Board"
                    strNodeText &= " - " & oXmlChildNode.Attributes("Description").Value
                Case "Section", "Folder"
                    strNodeText &= " - " & oXmlChildNode.Attributes("Name").Value
                Case "Option", "AddFile", "AddVGDDFile", "#cdata-section", "#comment", "#whitespace"
                    blnUseNode = False
            End Select
            If blnUseNode Then
                Dim oNewTreeNode As TreeNode
                oNewTreeNode = ParentTreeNode.Nodes(ParentTreeNode.Text & "_" & strNodeText)
                If oNewTreeNode Is Nothing Then
                    oNewTreeNode = ParentTreeNode.Nodes.Add(ParentTreeNode.Text & "_" & strNodeText, strNodeText)
                End If
                oNewTreeNode.Tag = oXmlChildNode
                'If oXmlChildNode.ParentNode.Name = "MplabXWizard" Then
                '    oNewTreeNode.Name = oXmlChildNode.Name
                'End If
                If oXmlChildNode.ChildNodes.Count = 1 AndAlso oXmlChildNode.ChildNodes(0).NodeType = XmlNodeType.Text Then
                Else
                    AddTreeViewChildNodes(oNewTreeNode, oXmlChildNode)
                End If
                'If oNewNode.Nodes.Count = 0 Then oNewNode.EnsureVisible()
            End If
            'End If
        Next oXmlChildNode
    End Sub

    Private Sub tvCustomEdit_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvTemplates.AfterSelect
        If e.Action = TreeViewAction.ByMouse Then
            SelectedTreeNode = e.Node
            Dim oXmlTemplateNode As XmlNode = e.Node.Tag
            Dim blnOldSaveChange As Boolean = btnSaveTemplate.Visible
            'If e.Node.Parent IsNot Nothing AndAlso e.Node.Parent.Text = "Code" Then
            '    rtEdit.Text = CType(e.Node.Tag, XmlNode).InnerText
            'Else
            rtEdit.Text = CType(e.Node.Tag, XmlNode).InnerXml.Replace("><", ">" & vbCrLf & "<")
            'End If
            rtEdit.Visible = True
            btnSaveTemplate.Visible = blnOldSaveChange
        End If
    End Sub

    Private Sub CheckChanges()
        If btnSaveTemplate.Visible Then
            Dim ret As DialogResult = MessageBox.Show("Current Templates has been modified. Do you want to discard Changes?", "Discard changes?", MessageBoxButtons.YesNo)
            Select Case ret
                Case Windows.Forms.DialogResult.No
                    btnSaveTemplate.PerformClick()
                Case Windows.Forms.DialogResult.Yes
                    If MessageBox.Show("Are you sure to discard all your hard work?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                        VGDDCustom.LoadCustomTemplatesDoc()
                    Else
                        btnSaveTemplate.PerformClick()
                    End If
            End Select
            btnSaveTemplate.Visible = False
        End If
    End Sub

    Private Sub Form_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        CheckChanges()
    End Sub

    Private Sub DeleteNode(ByVal oTreeNode As TreeNode)
        Dim oXmlCustNode As XmlNode = oTreeNode.Tag
        If oXmlCustNode.ParentNode IsNot Nothing Then oXmlCustNode.ParentNode.RemoveChild(oXmlCustNode)
        tvTemplates.SelectedNode = oTreeNode.Parent
        oTreeNode.Remove()
        tvCustomEdit_AfterSelect(tvTemplates, New TreeViewEventArgs(tvTemplates.SelectedNode, TreeViewAction.ByMouse))
        btnSaveTemplate.Visible = True
        tvTemplates.Focus()
    End Sub

    Private Sub tvTemplates_BeforeSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles tvTemplates.BeforeSelect
        '    If btnSaveTemplate.Visible Then
        '        Dim ret As DialogResult = MessageBox.Show("Save changes?", "Save changes?", MessageBoxButtons.YesNoCancel)
        '        Select Case ret
        '            Case Windows.Forms.DialogResult.Yes
        '                btnSaveTemplate.PerformClick()
        '            Case Windows.Forms.DialogResult.No
        '            Case Windows.Forms.DialogResult.Cancel
        '                e.Cancel = True
        '                Exit Sub
        '        End Select
        '        btnSaveTemplate.Visible = False
        '    End If
        If SelectedTreeNode IsNot Nothing Then
            Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
            'If SelectedTreeNode.Parent IsNot Nothing AndAlso SelectedTreeNode.Parent.Text = "Code" Then
            '    If oXmlCustNode.InnerText <> rtEdit.Text Then
            '        oXmlCustNode.InnerText = rtEdit.Text
            '        btnSaveTemplate.Visible = True
            '    End If
            'Else
            If oXmlCustNode.InnerXml <> rtEdit.Text.Replace(vbLf, vbCrLf) Then
                Try
                    oXmlCustNode.InnerXml = rtEdit.Text.Replace(vbLf, vbCrLf)
                    btnSaveTemplate.Visible = True
                Catch ex As Exception
                    Debug.Print(ex.Message)
                End Try
            End If
        End If
        'End If
    End Sub

End Class

'End Namespace
