Imports System.Xml
Imports System.IO
Imports VGDDCommon

'Namespace VGDDCommon

Public Class frmCustomWidgetEditor

    Private SelectedControlXmlNode As XmlNode
    Private SelectedTreeNode As TreeNode
    Private dtStates As New DataTable
    Private oCustProp As VGDDCustomProp

    Private Sub frmCustomWidgetEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        HideDetails()
        cmbCustomWidgetName.Items.Clear()
        If VGDDCustom.XmlUserCustomTemplatesDoc Is Nothing Then
            VGDDCustom.LoadUserCustomTemplatesDoc()
        End If
        RefreshcmbCustomWidgetName()
    End Sub

    Private Sub HideDetails()
        rtEdit.Visible = False
        grpProperty.Visible = False
        grpDefinition.Visible = False
        grpCodeOptions.Visible = False
        grpBitmap.Visible = False
        grpEvents.Visible = False
        grpEventDetails.Visible = False
        grpActions.Visible = False
        grpActionDetails.Visible = False
        grpStates.Visible = False
        grpStateDetails.Visible = False
        picBitmap.Visible = False
    End Sub

    Private Sub RefreshcmbCustomWidgetName()
        cmbCustomWidgetName.Items.Clear()
        If VGDDCustom.XmlUserCustomTemplatesDoc IsNot Nothing Then
            For Each oNode As XmlNode In VGDDCustom.XmlUserCustomTemplatesDoc.DocumentElement.ChildNodes
                cmbCustomWidgetName.Items.Add(oNode.Name)
            Next
        End If
    End Sub

    Private Sub RefreshTreeView()
        tvCustomEdit.Nodes.Clear()
        If VGDDCustom.XmlUserCustomTemplatesDoc IsNot Nothing AndAlso cmbCustomWidgetName.SelectedItem <> "" Then
            Dim oNode As XmlNode = VGDDCustom.XmlUserCustomTemplatesDoc.DocumentElement.SelectSingleNode(cmbCustomWidgetName.SelectedItem)
            Dim oNewTreeNode As TreeNode = tvCustomEdit.Nodes.Add(oNode.Name)
            oNewTreeNode.Tag = oNode
            AddTreeViewChildNodes(tvCustomEdit.TopNode, oNode)
        End If
    End Sub

    Private Sub AddTreeViewChildNodes(ByRef ParentTreeNode As TreeNode, ByRef oXmlNode As XmlNode)
        For Each oXmlChildNode As XmlNode In oXmlNode.ChildNodes
            Dim strNodeText As String = oXmlChildNode.Name
            Select Case strNodeText
                Case "Property"
                    strNodeText &= " " & oXmlChildNode.Attributes("Name").Value
                Case "Event"
                    strNodeText &= " " & oXmlChildNode.Attributes("Name").Value
                Case "Action"
                    strNodeText &= " " & oXmlChildNode.Attributes("Name").Value
            End Select
            Dim oNewTreeNode As TreeNode
            oNewTreeNode = ParentTreeNode.Nodes(ParentTreeNode.Text & "_" & strNodeText)
            If oNewTreeNode Is Nothing Then
                oNewTreeNode = ParentTreeNode.Nodes.Add(ParentTreeNode.Text & "_" & strNodeText, strNodeText)
            End If
            oNewTreeNode.Tag = oXmlChildNode
            If oXmlChildNode.ParentNode.Name = "VGDDCustomWidgetsTemplate" Then
                oNewTreeNode.Name = oXmlChildNode.Name
            End If
            If oXmlChildNode.ChildNodes.Count = 1 AndAlso oXmlChildNode.ChildNodes(0).NodeType = XmlNodeType.Text Then
            Else
                AddTreeViewChildNodes(oNewTreeNode, oXmlChildNode)
            End If
            'If oNewNode.Nodes.Count = 0 Then oNewNode.EnsureVisible()
        Next oXmlChildNode
    End Sub

    Private Sub tvCustomEdit_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvCustomEdit.AfterSelect
        If e.Action = TreeViewAction.ByMouse Then
            SelectedTreeNode = e.Node
            Dim oXmlCustNode As XmlNode = e.Node.Tag
            If oXmlCustNode.NodeType <> XmlNodeType.Element Then Exit Sub
            HideDetails()
            If e.Node.Text = "Definition" Then
                grpDefinition.Visible = True
            ElseIf e.Node.Text = "Events" Then
                grpEvents.Visible = True
            ElseIf e.Node.Text = "Actions" Then
                grpActions.Visible = True
            ElseIf e.Node.Text = "State" Then
                grpStates.Visible = True
            ElseIf e.Node.Text.StartsWith("Property") Then
                oCustProp = VGDDCustomTypeDescriptor.CustomPropFromXml(e.Node.Tag)
                'If oCustProp.StrType = "" Then
                '    oCustProp.StrType = "String"
                'End If
                PropertyGrid1.SelectedObject = oCustProp
                PropertyGrid1.Visible = True
                grpProperty.Text = "Edit Property " & oCustProp.Name
                grpProperty.Visible = True
            ElseIf e.Node.Text.StartsWith("Event ") Then
                txtEventDescription.Text = oXmlCustNode.Attributes("Description").Value
                txtEventName.Text = oXmlCustNode.Attributes("Name").Value
                grpEventDetails.Visible = True
            ElseIf e.Node.Text.StartsWith("Action ") Then
                txtActionCode.Text = oXmlCustNode.Attributes("Code").Value
                txtActionName.Text = oXmlCustNode.Attributes("Name").Value
                grpActionDetails.Visible = True
            ElseIf e.Node.Parent IsNot Nothing AndAlso e.Node.Parent.Text = ("State") Then
                txtStateName.Text = oXmlCustNode.Name
                txtStateName.Enabled = False
                With dtStates
                    .Columns.Clear()
                    .Rows.Clear()
                    .Columns.Add("State", GetType(System.String))
                    .Columns.Add("Value", GetType(System.String))
                    For Each oAttribute As XmlAttribute In oXmlCustNode.Attributes
                        Dim oRow As DataRow = dtStates.NewRow
                        oRow("State") = oAttribute.Name
                        oRow("Value") = oAttribute.Value
                        .Rows.Add(oRow)
                    Next
                End With
                grdStates.DataSource = dtStates
                'grdStates.Columns(1).Width = 300
                grpStateDetails.Visible = True
            ElseIf e.Node.Text = "Bitmap" Or e.Node.Text = "ToolboxBitmap" Then
                lblBitmapFileName.Text = CType(e.Node.Tag, XmlNode).Attributes("FileName").Value.ToString
                Dim strBitmapFile As String = Path.Combine(Common.UserTemplatesFolder, lblBitmapFileName.Text)
                If File.Exists(strBitmapFile) Then
                    Try
                        picBitmap.Image = Bitmap.FromStream(New FileStream(strBitmapFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    Catch ex As Exception
                        MessageBox.Show("Invalid bitmap " & strBitmapFile, "Error")
                    End Try
                Else
                    picBitmap.Image = Nothing
                End If
                grpBitmap.Visible = True
                picBitmap.Visible = True
            ElseIf e.Node.Parent IsNot Nothing AndAlso e.Node.Parent.Text = "CodeGen" Then
                grpCodeOptions.Visible = True
                Dim blnOldSaveChange As Boolean = btnSaveWidget.Visible
                rtEdit.Text = CType(e.Node.Tag, XmlNode).InnerText.Trim
                rtEdit.Visible = True
                btnSaveWidget.Visible = blnOldSaveChange
                With cmbCodeParams
                    .Items.Clear()
                    '.Text = ""
                    .DropDownStyle = ComboBoxStyle.DropDownList
                    .Items.Add("")
                    .Items.Add(New ComboBoxTextValue("ID of the Control", "[CONTROLID]"))
                    .Items.Add(New ComboBoxTextValue("ID of the Control (for indexed controls)", "[CONTROLID_NOINDEX][CONTROLID_INDEX]"))
                    .Items.Add(New ComboBoxTextValue("Next Control ID number", "[NEXT_NUMID]"))
                    Dim oNode As XmlNode = CType(e.Node.Parent.Parent.Tag, XmlNode).SelectSingleNode("Definition")
                    If oNode IsNot Nothing Then
                        For Each oXmlNode As XmlNode In oNode.ChildNodes
                            If oXmlNode.Name = "Property" AndAlso oXmlNode.Attributes("Category").Value <> "State" Then
                                .Items.Add(New ComboBoxTextValue("Property " & oXmlNode.Attributes("Name").Value, "[" & oXmlNode.Attributes("Name").Value.ToUpper & "]"))
                            End If
                        Next
                    End If
                    .Items.Add(New ComboBoxTextValue("Property STATE", "[STATE]"))
                    .Items.Add(New ComboBoxTextValue("Property SCHEME", "GOLScheme_[SCHEME]"))
                End With
            Else
                'If e.Node.Parent.Text = "CodeGen" Then
                '    rtEdit.Text = CType(e.Node.Tag, XmlNode).InnerText.Trim
                'End If

            End If
        End If
    End Sub

    Private Sub cmbCustomWidgetName_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cmbCustomWidgetName.KeyDown
        btnNewWidget.Visible = True
        btnNewWidget.Enabled = True
    End Sub

    Private Sub CheckChanges()
        If btnSaveWidget.Visible Then
            Dim ret As DialogResult = MessageBox.Show("Current Widget has been modified. Do you want to discard Changes?", "Discard changes?", MessageBoxButtons.YesNo)
            Select Case ret
                Case Windows.Forms.DialogResult.No
                    btnSaveWidget.PerformClick()
                Case Windows.Forms.DialogResult.Yes
                    If MessageBox.Show("Are you sure to discard all your hard work?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                        VGDDCustom.LoadCustomTemplatesDoc()
                    Else
                        btnSaveWidget.PerformClick()
                    End If
            End Select
            btnSaveWidget.Visible = False
        End If
    End Sub

    Private Sub frmCustomWidgetEditor_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        CheckChanges()
    End Sub

    Private Sub cmbCustomWidgetName_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCustomWidgetName.SelectionChangeCommitted
        Try
            CheckChanges()
            btnNewWidget.Visible = False
            RefreshTreeView()
            SelectedControlXmlNode = tvCustomEdit.Nodes(0).Nodes(0).Tag
            If tvCustomEdit.Nodes.Count > 0 Then
                SelectedControlXmlNode = tvCustomEdit.Nodes(0).Tag
                tvCustomEdit.Nodes(0).ExpandAll()
                tvCustomEdit.SelectedNode = tvCustomEdit.Nodes(0)
                btnDelete.Visible = True
            Else
                btnDelete.Visible = False
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click
        If cmbCodeParams.SelectedItem Is Nothing Then Exit Sub
        rtEdit.SelectedText = CType(cmbCodeParams.SelectedItem, ComboBoxTextValue).Value
        rtEdit.Focus()
    End Sub

    Private Sub btnNewWidget_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewWidget.Click
        Dim strNewWidgetName As String = Common.CleanName(cmbCustomWidgetName.Text) '.Replace(" ", "").Replace("-", "_")
        Try
            If VGDDCustom.XmlUserCustomTemplatesDoc Is Nothing Then
                VGDDCustom.LoadUserCustomTemplatesDoc()
                If VGDDCustom.XmlUserCustomTemplatesDoc Is Nothing Then
                    VGDDCustom.XmlUserCustomTemplatesDoc = New XmlDocument
                    Dim oElement As XmlElement = VGDDCustom.XmlUserCustomTemplatesDoc.CreateElement("VGDDCustomWidgetsTemplate")
                    VGDDCustom.XmlUserCustomTemplatesDoc.AppendChild(oElement)
                End If
            End If
            Dim strWidgetName As String = cmbCustomWidgetName.Text
            Dim oNodeNewWidget As XmlNode = VGDDCustom.XmlUserCustomTemplatesDoc.CreateNode(XmlNodeType.Element, strWidgetName, "")
            oNodeNewWidget.InnerXml = "<Definition>" & _
    "<Bitmap FileName=""" & strWidgetName & ".jpg"" />" & _
    "<ToolboxBitmap FileName=""" & strWidgetName & ".ico"" />" & _
    "<Property Name=""left"" Type=""Int"" Defaultvalue=""0"" Category=""Size and Position"" Eval=""Left"" Description=""Left X coordinate of the upper-left edge"" />" & _
    "<Property Name=""top"" Type=""Int"" Defaultvalue=""0"" Category=""Size and Position"" Eval=""Top"" Description=""Top Y coordinate of the upper-left edge"" />" & _
    "<Property Name=""right"" Type=""Int"" Defaultvalue=""100"" Category=""Size and Position"" Eval=""Left+Width"" Description=""Right X coordinate of the lower-right edge"" />" & _
    "<Property Name=""bottom"" Type=""Int"" Defaultvalue=""50"" Category=""Size and Position"" Eval=""Top+Height"" Description=""Bottom Y coordinate of the lower-right edge"" />" & _
    "<Property Name=""Scheme"" Type=""String"" Defaultvalue="""" Category=""Size and Position"" Description=""Colour scheme for the " & strWidgetName & """ />" & _
    "<Property Name=""Disabled"" Type=""Bool"" Defaultvalue=""False"" Category=""Appearance"" DestProperty=""STATE"" Description=""Status of the " & strWidgetName & """ />" & _
    "<Property Name=""Hidden"" Type=""Bool"" Defaultvalue=""False"" Category=""Appearance"" DestProperty=""STATE"" Description=""Visibility of the " & strWidgetName & """ />" & _
    "<Property Name=""Public"" Type=""Bool"" Defaultvalue=""False"" Category=""C Language"" Description=""Has the Widget to be declared public when generating code?"" />" & _
    "</Definition>" & _
    "<CodeGen>" & _
    "<Header>" & _
    "#define ID_[CONTROLID_NOINDEX][CONTROLID_INDEX]   [NEXT_NUMID]" & _
    "</Header>" & _
    "<GraphicsConfig>" & _
    "USE_" & strWidgetName.ToUpper & "" & _
    "</GraphicsConfig>" & _
    "<Constructor>" & _
    "" & strWidgetName.ToUpper & " *p[CONTROLID];" & _
    "</Constructor>" & _
    "<Code>" & _
    "p[CONTROLID] = " & strWidgetName & "Create(ID_[CONTROLID_NOINDEX][CONTROLID_INDEX],[LEFT],[TOP],[RIGHT],[BOTTOM],[STATE],[NUMCOLUMNS],[NUMROWS],[CELLWIDTH],[CELLHEIGHT],GOLScheme_[SCHEME]);" & _
    "</Code>" & _
    "<State>" & _
    "<Disabled True=""" & strWidgetName.ToUpper & "_DRAW|" & strWidgetName.ToUpper & "_DISABLED"" False=""" & strWidgetName.ToUpper & "_DRAW"" />" & _
    "<Hidden False=""" & strWidgetName.ToUpper & "_DRAW"" True=""" & strWidgetName.ToUpper & "_HIDE"" />" & _
    "</State>" & _
    "</CodeGen>" & _
    "<Events>" & _
    "<Event Name=""" & strWidgetName.ToUpper & "_MSG_TOUCHED"" Description=""" & strWidgetName & " touched"" />" & _
    "</Events>" & _
    "<Actions>" & _
    "<Action Name=""Use Widget ID"" Code=""ID_[CONTROLID_NOINDEX][CONTROLID_INDEX]"" />" & _
    "<Action Name=""Hide " & strWidgetName & """ Code=""SetState(GOLFindObject(ID_[CONTROLID_NOINDEX][CONTROLID_INDEX]), " & strWidgetName.ToUpper & "_HIDE);[NEWLINE]"" />" & _
    "<Action Name=""Show/Update " & strWidgetName & """ Code=""SetState(GOLFindObject(ID_[CONTROLID_NOINDEX][CONTROLID_INDEX]), " & strWidgetName.ToUpper & "_DRAW);[NEWLINE]"" />" & _
    "</Actions>"
            VGDDCustom.XmlUserCustomTemplatesDoc.DocumentElement.AppendChild(oNodeNewWidget)
            RefreshcmbCustomWidgetName()
            cmbCustomWidgetName.SelectedIndex = cmbCustomWidgetName.Items.Count - 1
            cmbCustomWidgetName_SelectionChangeCommitted(cmbCodeParams, Nothing)
            btnSaveWidget.Visible = True
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error creating Custom Widget")
        End Try
    End Sub

    'Private Sub CreateEmptyNode(ByVal Name As String)
    '    If Name Is Nothing OrElse Name = String.Empty Then Exit Sub
    '    If VGDDCustom.XmlUserCustomTemplatesDoc Is Nothing Then
    '        VGDDCustom.LoadUserCustomTemplatesDoc()
    '    End If
    '    Dim oDoc As XmlDocument = VGDDCustom.XmlUserCustomTemplatesDoc
    '    Dim oNodeNewWidget As XmlNode = oDoc.CreateNode(XmlNodeType.Element, "", Name, "")
    '    Dim oNode As XmlNode
    '    oNodeNewWidget.AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Definition", ""))
    '    oNode = oNodeNewWidget.SelectSingleNode("Definition").AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Bitmap", ""))
    '    oNode.Attributes.Append(oNode.OwnerDocument.CreateAttribute("FileName"))
    '    oNode = oNodeNewWidget.SelectSingleNode("Definition").AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "ToolboxBitmap", ""))
    '    oNode.Attributes.Append(oNode.OwnerDocument.CreateAttribute("FileName"))
    '    oNodeNewWidget.AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "CodeGen", ""))
    '    oNodeNewWidget.SelectSingleNode("CodeGen").AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Header", ""))
    '    oNodeNewWidget.SelectSingleNode("CodeGen").AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "CodeHeadComment", ""))
    '    oNodeNewWidget.SelectSingleNode("CodeGen").AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Constructor", ""))
    '    oNodeNewWidget.SelectSingleNode("CodeGen").AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Code", ""))
    '    oNodeNewWidget.SelectSingleNode("CodeGen").AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "State", ""))
    '    oNodeNewWidget.AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Events", ""))
    '    oNodeNewWidget.AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Actions", ""))
    '    oDoc.DocumentElement.AppendChild(oNodeNewWidget)
    'End Sub

    Private Function CreateEmptyProperty(ByVal PropertyName As String) As XmlNode
        Dim oDoc As XmlDocument = VGDDCustom.XmlUserCustomTemplatesDoc
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        Dim oPropertyNode As XmlNode = oXmlCustNode.AppendChild(oDoc.CreateNode(XmlNodeType.Element, "", "Property", ""))
        Dim oAttr As XmlAttribute
        oAttr = oDoc.CreateAttribute("Name")
        oAttr.Value = PropertyName
        oPropertyNode.Attributes.Append(oAttr)

        oAttr = oDoc.CreateAttribute("Type")
        oAttr.Value = String.Empty
        oPropertyNode.Attributes.Append(oAttr)

        oAttr = oDoc.CreateAttribute("Defaultvalue")
        oAttr.Value = String.Empty
        oPropertyNode.Attributes.Append(oAttr)

        oAttr = oDoc.CreateAttribute("Category")
        oAttr.Value = String.Empty
        oPropertyNode.Attributes.Append(oAttr)

        oAttr = oDoc.CreateAttribute("Eval")
        oAttr.Value = String.Empty
        oPropertyNode.Attributes.Append(oAttr)

        oAttr = oDoc.CreateAttribute("Description")
        oAttr.Value = String.Empty
        oPropertyNode.Attributes.Append(oAttr)
        Return oPropertyNode
    End Function

    Private Sub txtPropertyName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPropertyName.TextChanged
        btnNewProperty.Enabled = txtPropertyName.Text <> ""
    End Sub

    Private Sub btnNewProperty_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewProperty.Click
        Dim oPropertyXmlNode As XmlNode = CreateEmptyProperty(txtPropertyName.Text)
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        oXmlCustNode.AppendChild(oPropertyXmlNode)
        Dim oNode As TreeNode = SelectedTreeNode.Nodes.Add(SelectedTreeNode.Text & "_" & txtPropertyName.Text, "Property " & txtPropertyName.Text)
        oNode.Tag = oPropertyXmlNode

        tvCustomEdit.SelectedNode = oNode
        tvCustomEdit_AfterSelect(tvCustomEdit, New TreeViewEventArgs(tvCustomEdit.SelectedNode, TreeViewAction.ByMouse))
        tvCustomEdit.Focus()

        txtPropertyName.Text = ""
    End Sub

    Private Sub btnChooseBitmap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChooseBitmap.Click
        Dim fileDialog As New OpenFileDialog()
        fileDialog.FileName = lblBitmapFileName.Text
        fileDialog.Filter = "Graphics files|*.bmp; *.jpg; *.png; *.gif; *.ico|All Files|*.*"
        fileDialog.RestoreDirectory = False
        If fileDialog.ShowDialog() = DialogResult.OK Then
            If picBitmap.Image IsNot Nothing Then picBitmap.Image.Dispose()
            Dim strFile As String = Path.GetFileName(fileDialog.FileName)
            If SelectedTreeNode.Text = "ToolboxBitmap" Then
                Dim tb As New Bitmap(fileDialog.FileName)
                If tb.Height > 16 Then
                    Dim tbscaled As New Bitmap(CInt(16 * tb.Width / tb.Height), 16)
                    Dim g As Graphics
                    g = Graphics.FromImage(tbscaled)
                    g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    g.DrawImage(tb, 0, 0, tbscaled.Width, tbscaled.Height)
                    tb.Dispose()
                    tb = New Bitmap(tbscaled)
                End If
                Dim strIconFile As String = Path.Combine(Common.UserTemplatesFolder, Path.GetFileNameWithoutExtension(strFile) & ".ico")
                If File.Exists(strIconFile) Then
                    Try
                        File.Delete(strIconFile)
                    Catch ex As Exception
                        MessageBox.Show("Error copying " & strIconFile & vbCrLf & "Try with a different filename", "Error", MessageBoxButtons.OK)
                        Return
                    End Try
                End If
                tb.Save(strIconFile, Imaging.ImageFormat.Icon)
                strFile = Path.GetFileName(strIconFile)
            Else
                If Path.GetDirectoryName(fileDialog.FileName).ToUpper <> VGDDCustom.CustomWidgetsFolder.ToUpper Then
                    Try
                        File.Copy(fileDialog.FileName, Path.Combine(Common.UserTemplatesFolder, strFile), True)
                    Catch ex As Exception
                        MessageBox.Show(String.Format("Cannot copy {0} to {1}: {2}", fileDialog.FileName, Common.UserTemplatesFolder, ex.Message), "Error copying Bitmap", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End If
            fileDialog.Dispose()
            lblBitmapFileName.Text = strFile
            Dim bm As New Bitmap(Path.Combine(Common.UserTemplatesFolder, lblBitmapFileName.Text))
            picBitmap.Image = New Bitmap(bm)
            bm.Dispose()

            Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
            Dim oAttr As XmlAttribute = oXmlCustNode.Attributes("FileName")
            If oAttr Is Nothing Then
                oAttr = oXmlCustNode.OwnerDocument.CreateAttribute("FileName")
                oXmlCustNode.Attributes.Append(oAttr)
            End If
            If strFile <> oAttr.Value Then
                oAttr.Value = strFile
                btnSaveWidget.Visible = True
            End If
        End If
    End Sub

    Private Sub PropertyGrid1_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        Dim oAttr As XmlAttribute = oXmlCustNode.Attributes(e.ChangedItem.Label.Replace("(", "").Replace(")", ""))
        If oAttr Is Nothing Then
            'oAttr = VGDDCustom.XmlUserCustomTemplatesDoc.CreateAttribute(e.ChangedItem.Label)
            oAttr = oXmlCustNode.OwnerDocument.CreateAttribute(e.ChangedItem.Label)
            oXmlCustNode.Attributes.Append(oAttr)
        End If
        If e.ChangedItem.Value <> oAttr.Value Then
            oAttr.Value = e.ChangedItem.Value
            btnSaveWidget.Visible = True
        End If
    End Sub

    Private Sub txtEventDescription_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtEventDescription.TextChanged, txtEventName.TextChanged
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        If txtEventDescription.Text <> oXmlCustNode.Attributes("Description").Value Then
            oXmlCustNode.Attributes("Description").Value = txtEventDescription.Text
            'oXmlCustNode.Attributes("Name").Value = txtEventName.Text
            btnSaveWidget.Visible = True
        End If
    End Sub

    Private Sub txtActionCode_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtActionCode.TextChanged
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        If txtActionCode.Text <> oXmlCustNode.Attributes("Code").Value Then
            oXmlCustNode.Attributes("Code").Value = txtActionCode.Text
            'oXmlCustNode.Attributes("Name").Value = txtActionName.Text
            btnSaveWidget.Visible = True
        End If
    End Sub

    Private Sub btnSaveWidget_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveWidget.Click
        If MessageBox.Show("Are you sure to save the modified Template XML?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            VGDDCustom.SaveUserCustomTemplatesDoc()
            btnSaveWidget.Visible = False
            RefreshcmbCustomWidgetName()
            cmbCustomWidgetName.SelectedIndex = -1
        End If
    End Sub

    Private Sub rtEdit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtEdit.TextChanged
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        If oXmlCustNode.InnerText <> rtEdit.Text Then
            oXmlCustNode.InnerText = rtEdit.Text
            btnSaveWidget.Visible = True
        End If
    End Sub

    Private Sub txtNewActionName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNewActionName.TextChanged
        btnCreateAction.Enabled = txtNewActionName.Text.Trim <> ""
    End Sub

    Private Sub txtNewEventName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNewEventName.TextChanged
        btnCreateEvent.Enabled = txtNewEventName.Text.Trim <> ""
    End Sub

    Private Sub txtNewStateName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNewStateName.TextChanged
        btnCreateState.Enabled = txtNewStateName.Text.Trim <> ""
    End Sub

    Private Sub btnCreateEvent_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateEvent.Click
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        Dim oEventNode As XmlNode = oXmlCustNode.AppendChild(VGDDCustom.XmlUserCustomTemplatesDoc.CreateNode(XmlNodeType.Element, "", "Event", ""))
        Dim oAttr As XmlAttribute
        oAttr = VGDDCustom.XmlUserCustomTemplatesDoc.CreateAttribute("Name")
        oAttr.Value = txtNewEventName.Text
        oEventNode.Attributes.Append(oAttr)

        oAttr = VGDDCustom.XmlUserCustomTemplatesDoc.CreateAttribute("Description")
        oAttr.Value = String.Empty
        oEventNode.Attributes.Append(oAttr)

        AddTreeViewChildNodes(SelectedTreeNode, oXmlCustNode)

        tvCustomEdit.SelectedNode = SelectedTreeNode.Nodes(SelectedTreeNode.Text & "_Event " & txtNewEventName.Text)
        tvCustomEdit_AfterSelect(tvCustomEdit, New TreeViewEventArgs(tvCustomEdit.SelectedNode, TreeViewAction.ByMouse))
        tvCustomEdit.Focus()

        btnSaveWidget.Visible = True
        txtNewEventName.Text = ""
    End Sub

    Private Sub grdStates_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles grdStates.CellEndEdit
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        Dim oAttr As XmlAttribute
        For Each oRow As DataRow In dtStates.Rows
            Try
                oAttr = oXmlCustNode.Attributes(oRow("State"))
                If oAttr Is Nothing Then
                    oAttr = VGDDCustom.XmlUserCustomTemplatesDoc.CreateAttribute(oRow("State"))
                    oXmlCustNode.Attributes.Append(oAttr)
                End If
                oAttr.Value = oRow("Value")
            Catch ex As Exception
            End Try
        Next
        btnSaveWidget.Visible = True
    End Sub

    Private Sub grdStates_UserDeletedRow(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowCancelEventArgs) Handles grdStates.UserDeletingRow
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        Dim oAttr As XmlAttribute
        For Each oRow As DataRow In dtStates.Rows
            oAttr = oXmlCustNode.Attributes(oRow("State"))
            If oAttr.Name = e.Row.Cells("State").Value And oAttr.Value = e.Row.Cells("Value").Value Then
                oRow.Delete()
                oXmlCustNode.Attributes.Remove(oAttr)
                Exit For
            End If
        Next
        btnSaveWidget.Visible = True
    End Sub

    Private Sub btnCreateState_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateState.Click
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        Dim oStateNode As XmlNode = oXmlCustNode.AppendChild(VGDDCustom.XmlUserCustomTemplatesDoc.CreateNode(XmlNodeType.Element, "", txtNewStateName.Text, ""))

        AddTreeViewChildNodes(SelectedTreeNode, oXmlCustNode)

        tvCustomEdit.SelectedNode = SelectedTreeNode.Nodes(SelectedTreeNode.Text & "_" & txtNewStateName.Text)
        tvCustomEdit_AfterSelect(tvCustomEdit, New TreeViewEventArgs(tvCustomEdit.SelectedNode, TreeViewAction.ByMouse))
        btnSaveWidget.Visible = True
        tvCustomEdit.Focus()

        txtNewStateName.Text = ""
    End Sub

    Private Sub btnCreateAction_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateAction.Click
        Dim oXmlCustNode As XmlNode = SelectedTreeNode.Tag
        Dim oActionNode As XmlNode = oXmlCustNode.AppendChild(VGDDCustom.XmlUserCustomTemplatesDoc.CreateNode(XmlNodeType.Element, "", "Action", ""))
        Dim oAttr As XmlAttribute
        oAttr = VGDDCustom.XmlUserCustomTemplatesDoc.CreateAttribute("Name")
        oAttr.Value = txtNewActionName.Text
        oActionNode.Attributes.Append(oAttr)

        oAttr = VGDDCustom.XmlUserCustomTemplatesDoc.CreateAttribute("Code")
        oAttr.Value = String.Empty
        oActionNode.Attributes.Append(oAttr)

        AddTreeViewChildNodes(SelectedTreeNode, oXmlCustNode)

        tvCustomEdit.SelectedNode = SelectedTreeNode.Nodes(SelectedTreeNode.Text & "_Action " & txtNewActionName.Text)
        tvCustomEdit_AfterSelect(tvCustomEdit, New TreeViewEventArgs(tvCustomEdit.SelectedNode, TreeViewAction.ByMouse))
        btnSaveWidget.Visible = True
        tvCustomEdit.Focus()

        txtNewActionName.Text = ""
    End Sub

    Private Sub btnDeleteState_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteState.Click
        If MessageBox.Show("Delete State " & txtStateName.Text & "?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            DeleteNode(SelectedTreeNode)
        End If
    End Sub

    Private Sub btnDeleteProperty_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteProperty.Click
        If MessageBox.Show("Delete Property " & SelectedTreeNode.Text & "?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            DeleteNode(SelectedTreeNode)
        End If
    End Sub

    Private Sub btnDeleteEvent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteEvent.Click
        If MessageBox.Show("Delete Event " & txtEventName.Text & "?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            DeleteNode(SelectedTreeNode)
        End If
    End Sub

    Private Sub btnDeleteAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteAction.Click
        If MessageBox.Show("Delete Action " & txtEventName.Text & "?", "Confirm", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            DeleteNode(SelectedTreeNode)
        End If
    End Sub

    Private Sub DeleteNode(ByVal oTreeNode As TreeNode)
        Dim oXmlCustNode As XmlNode = oTreeNode.Tag
        If oXmlCustNode.ParentNode IsNot Nothing Then oXmlCustNode.ParentNode.RemoveChild(oXmlCustNode)
        tvCustomEdit.SelectedNode = oTreeNode.Parent
        oTreeNode.Remove()
        tvCustomEdit_AfterSelect(tvCustomEdit, New TreeViewEventArgs(tvCustomEdit.SelectedNode, TreeViewAction.ByMouse))
        btnSaveWidget.Visible = True
        tvCustomEdit.Focus()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If MessageBox.Show("Do you really want to DELETE " & cmbCustomWidgetName.Text & " custom Widget definition?", "Attention!", MessageBoxButtons.YesNo) = vbYes Then
            If MessageBox.Show("Do you want to abort this DELETE operation?", "Double check", MessageBoxButtons.YesNo) = vbNo Then
                VGDDCustom.XmlUserCustomTemplatesDoc.DocumentElement.RemoveChild(SelectedControlXmlNode)
                cmbCustomWidgetName.Text = ""
                tvCustomEdit.Nodes.Clear()
                btnDelete.Visible = False
                HideDetails()
                RefreshcmbCustomWidgetName()
            End If
        End If
    End Sub
End Class

Public Class ComboBoxTextValue
    Private strText As String
    Private strValue As String

    Public Sub New()
        strText = ""
        strValue = 0
    End Sub

    Public Sub New(ByVal Name As String, ByVal Value As String)
        strText = Name
        strValue = Value
    End Sub

    Public Property Text() As String
        Get
            Return strText
        End Get
        Set(ByVal sValue As String)
            strText = sValue
        End Set
    End Property

    Public Property Value() As String
        Get
            Return strValue
        End Get
        Set(ByVal strNewValue As String)
            strValue = strNewValue
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return strText
    End Function

End Class
'End Namespace