Imports System.Windows.Forms
Imports System.Data
Imports VGDDCommon
Imports System.ComponentModel

Namespace VGDDIDE
    Public Class EventsEditor
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent
        Implements IVGDDEventsEditor

#If Not PlayerMonolitico Then
        'Private _Events As VGDDEventsCollection

        Private LastSelectedIndex As Integer = -1
        Private strActionSelectedControlClass As String
        Private strActionSelectedControlName As String
        Private blnCodeModified As Boolean
        Private strCodeToInsert As String
        Private oSelectedEvent As VGDDEvent
        Private _EventsCollection As VGDDEventsCollection
        Private _EventsPropertyDescriptor As PropertyDescriptor
        Private WithEvents TextDocument As ICSharpCode.TextEditor.Document.IDocument
        Private Saving As Boolean
        Private strOldCode As String
        Private strOriginalCode As String
        Private WithEvents tmrUpdateCode As Timer

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.TopLevel = False
            tmrUpdateCode = New Timer
            tmrUpdateCode.Enabled = False
            tmrUpdateCode.Interval = 250
        End Sub
        'Private Sub frmEventsEditor_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        '    If Me.Opacity = 0 Then
        '        FadeForm.FadeIn(Me, 99)
        '    End If
        'End Sub

        'Private Sub frmEventsEditor_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '    FadeForm.FadeOutAndWait(Me)
        'End Sub

        Private Sub frmEventsEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            SetHelp()
            LastSelectedIndex = -1
            lblActionHelp.Text = String.Empty
            'TextEditorControl1.ActiveEditor.Document.TextContent = String.Empty
        End Sub

        Public Sub Clear()
            cmbEvents.Items.Clear()
            cmbObjects.Items.Clear()
            cmbActions.Items.Clear()
            TextEditorControl1.ActiveEditor.Document.TextContent = ""
            TextEditorControl1.ActiveEditor.Refresh()
        End Sub

#Region "Help"
        Private Sub SetHelp()
            If Me.DesignMode Then Exit Sub
            Common.HelpProvider.SetHelpNavigator(Me, HelpNavigator.Topic)
            Common.HelpProvider.HelpNamespace = Common.HELPNAMESPACEBASE & "_EventsEditor"
            Common.HelpProvider.SetHelpKeyword(Me, "Main")
            Common.HelpProvider.SetShowHelp(Me, True)

            Common.HelpProvider.SetHelpNavigator(Me.cmbActions, HelpNavigator.Topic)
            Common.HelpProvider.SetHelpKeyword(Me.cmbActions, "Actions")
            Common.HelpProvider.SetShowHelp(Me.cmbActions, True)
        End Sub

        Private Sub cmbActions_HelpRequested(ByVal sender As Object, ByVal hlpevent As System.Windows.Forms.HelpEventArgs) Handles cmbActions.HelpRequested
            Common.HelpProvider.SetHelpKeyword(cmbActions, cmbActions.Text)
        End Sub
#End Region

        Public Sub RefreshList()
            cmbEvents.Items.Clear()
            TextEditorControl1.ActiveEditor.Document.TextContent = ""
            TextEditorControl1.ActiveEditor.Refresh()
            If _EditedControl Is Nothing Then Exit Sub
            Dim oHandledEvent As VGDDEvent = Nothing
            Dim intHandledEvents As Integer
            If _EditedControl.VGDDEvents IsNot Nothing Then
                For Each oEvent As VGDDEvent In _EditedControl.VGDDEvents.List
                    cmbEvents.Items.Add(oEvent.Name)
                    If oEvent.Code <> String.Empty Then
                        oHandledEvent = oEvent
                        intHandledEvents += 1
                    End If
                Next
            End If
            LastSelectedIndex = -1
            If cmbEvents.Items.Count = 1 Then
                cmbEvents.SelectedIndex = 0
                cmbEvents_SelectedIndexChanged(Nothing, Nothing)
            ElseIf intHandledEvents = 1 Then
                cmbEvents.Text = oHandledEvent.Name
            Else
                cmbEvents.SelectedIndex = -1
                cmbEvents_SelectedIndexChanged(Nothing, Nothing)
            End If
            cmbObjects.SelectedIndex = -1
            cmbActions.SelectedIndex = -1
            blnCodeModified = False
        End Sub

#Region "IVGDDEventsEditor"
        Private _ParentScreenName As String
        Public Property ParentScreenName As String Implements VGDDCommon.IVGDDEventsEditor.ParentScreenName
            Get
                Return _ParentScreenName
            End Get
            Set(ByVal value As String)
                _ParentScreenName = value
                RefreshList()
            End Set
        End Property

        Private _EditedControl As Common.IVGDDEvents
        Public Sub SetEditedControl(ByVal oControl As System.Windows.Forms.Control) Implements VGDDCommon.IVGDDEventsEditor.SetEditedControl
            If Not TypeOf (oControl) Is Common.IVGDDEvents Then Exit Sub
            _EditedControl = oControl
            strOldCode = String.Empty
            If oControl Is Nothing Then
                Me.DockHandler.TabText = "Events Editor"
                _EventsCollection = Nothing
                _EventsPropertyDescriptor = Nothing
            Else
                _EventsCollection = Nothing
                _EventsPropertyDescriptor = TypeDescriptor.GetProperties(_EditedControl)("VGDDEvents")
                If _EventsPropertyDescriptor IsNot Nothing Then
                    _EventsCollection = _EventsPropertyDescriptor.GetValue(_EditedControl)
                End If
                If _EventsCollection Is Nothing Then
                    Me.DockHandler.TabText = "Events Editor"
                    _EventsPropertyDescriptor = Nothing
                Else
                    If TypeOf (_EditedControl) Is VGDD.VGDDScreen Then
                        Me.DockHandler.TabText = "Events for Screen " & _ParentScreenName
                    Else
                        Me.DockHandler.TabText = "Events for Widget " & _ParentScreenName & "." & _EditedControl.Name
                    End If
                End If
                RefreshList()
            End If
        End Sub

        Public Overloads Property DialogResult As System.Windows.Forms.DialogResult Implements VGDDCommon.IVGDDEventsEditor.DialogResult
            Get
                Return MyBase.DialogResult
            End Get
            Set(ByVal value As System.Windows.Forms.DialogResult)
                MyBase.DialogResult = value
            End Set
        End Property

        Public Sub DisplayModal() Implements VGDDCommon.IVGDDEventsEditor.DisplayModal
            If Me.DockPanel Is Nothing Then
                Me.DockPanel = oMainShell._DockPanel1
                Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
                Me.Show(Me.DockPanel)
                oMainShell._DockPanel1.ActiveAutoHideContent = Me
            ElseIf Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide Or _
                   Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide Or _
                   Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide Or _
                   Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide Then
                Me.Show(Me.DockPanel)
                oMainShell._DockPanel1.ActiveAutoHideContent = Me
                'Me.DockPanel.HoverTimeout = 4000
            Else
                Me.Show()
            End If
            Me.Focus()
            If cmbEvents.Text <> String.Empty Then
                Me.TextEditorControl1.Focus()
            Else
                cmbEvents.Focus()
            End If
        End Sub
#End Region

        Private Sub cmbEvents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbEvents.SelectedIndexChanged
            RefreshEventCode()
        End Sub

        Public Sub RefreshEventCode()
            LastSelectedIndex = cmbEvents.SelectedIndex
            cmbObjects.Items.Clear()
            btnCancel.Visible = False
            strOldCode = String.Empty

            If cmbEvents.SelectedIndex >= 0 Then
                oSelectedEvent = _EditedControl.VGDDEvents(cmbEvents.Text)
                strOldCode = oSelectedEvent.Code
                TextDocument = Me.TextEditorControl1.ActiveEditor.Document
                TextDocument.TextContent = oSelectedEvent.Code
                TextDocument.HighlightingStrategy() =
                                    ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("MCHP_C")
                Me.TextEditorControl1.Enabled = True
                strOriginalCode = oSelectedEvent.Code
                cmbObjects.Items.Add("")
                For Each oScreenAttr As VGDD.VGDDScreenAttr In Common.aScreens.Values
                    cmbObjects.Items.Add(oScreenAttr.Name & " (SCREEN)")
                Next
                For Each oControl As Control In Common.CurrentScreen.Controls
                    Dim strType As String = oControl.GetType.ToString.Split(".")(1).ToUpper
                    If strType = "VGDDCUSTOM" Then
                        Dim oCustomControl As VGDDCustom = oControl
                        If oCustomControl.CustomWidgetType IsNot Nothing Then strType = oCustomControl.CustomWidgetType.ToUpper
                    End If
                    cmbObjects.Items.Add(oControl.Name & " (" & strType & ")" & IIf(oControl Is _EditedControl, " This Widget", ""))
                Next
                cmbObjects.Enabled = True
                cmbActions.Enabled = True
            Else
                Me.TextEditorControl1.ActiveEditor.Document.TextContent = ""
                TextEditorControl1.Enabled = False
                cmbObjects.Enabled = False
                cmbActions.Enabled = False
                btnActionInsertCode.Visible = False
            End If
            Me.TextEditorControl1.ActiveEditor.Refresh()

        End Sub

        Private Sub cmbObjects_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbObjects.SelectedValueChanged
            If cmbObjects.SelectedItem IsNot Nothing Then
                Dim astrControl() As String = cmbObjects.SelectedItem.ToString.Split("(")
                If astrControl.Length > 1 Then
                    strActionSelectedControlClass = astrControl(1).Substring(0, astrControl(1).IndexOf(")")).ToUpper
                    strActionSelectedControlName = astrControl(0).Trim
                    cmbActions.Items.Clear()
                    cmbActions.Items.Add("")
                    If Common.dtActions IsNot Nothing Then
                        For Each oRow As DataRow In Common.dtActions.Select("ControlType='" & strActionSelectedControlClass & "'")
                            cmbActions.Items.Add(oRow("ActionName"))
                        Next
                        cmbActions.SelectedIndex = 0
                    End If
                End If
            Else
                strActionSelectedControlClass = ""
            End If
            btnActionInsertCode.Visible = False
        End Sub

        Private Sub cmbActions_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbActions.SelectedValueChanged
            btnActionInsertCode.Visible = (cmbObjects.SelectedItem IsNot Nothing And cmbActions.SelectedItem IsNot Nothing)
        End Sub

        Private Sub btnActionInsertCode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActionInsertCode.Click
            If cmbActions.SelectedItem IsNot Nothing Then
                Dim aRows As DataRow() = Common.dtActions.Select(String.Format( _
                        "ControlType='{0}' AND ActionName='{1}'", strActionSelectedControlClass, cmbActions.SelectedItem.ToString.Replace("'", "''")))
                If aRows.Length > 0 Then
                    Dim strActualCode As String = strCodeToInsert
                    If strActualCode.Contains("$$") Then
                        strActualCode = strActualCode.Replace("$$", txtActionUserCode.Text)
                    End If
                    TextEditorControl1.ActiveEditor.ActiveTextAreaControl.TextArea.InsertString(strActualCode)
                    blnCodeModified = True
                    TextEditorControl1.Focus()
                    If pnlBitmaps.Visible AndAlso cmbBitmap.SelectedItem IsNot Nothing Then
                        Dim bmp As VGDDImage = Common.Bitmaps(cmbBitmap.SelectedItem)
                        bmp._ReferencedInEventCode = True
                    End If
                End If
            End If
        End Sub

        Private Sub cmbActions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbActions.SelectedIndexChanged
            lblActionHelp.Text = String.Empty
            pnlBitmaps.Visible = False
            pnlAction.Visible = False
            strCodeToInsert = String.Empty
            If cmbActions.SelectedIndex <> -1 Then
                GetCodeToInsert()
            End If
            If strCodeToInsert IsNot Nothing AndAlso strCodeToInsert.Contains("%BITMAP%") Then
                PopulateComboBitmaps()
                pnlBitmaps.Visible = True
            End If
        End Sub

        Private Sub GetCodeToInsert()
            Try
                Dim aRows As DataRow() = Common.dtActions.Select(String.Format( _
                        "ControlType='{0}' AND ActionName='{1}'", strActionSelectedControlClass, cmbActions.SelectedItem.ToString.Replace("'", "''")))
                If aRows.Length > 0 Then
                    If Not IsDBNull(aRows(0)("ActionHelp")) Then
                        lblActionHelp.Text = aRows(0)("ActionHelp")
                    End If
                    strCodeToInsert = aRows(0)("ActionCode").ToString
                    Dim aStrCode As String() = strCodeToInsert.Split("$")
                    If aStrCode.Length > 1 Then
                        pnlAction.Visible = True
                        If aStrCode(1).Contains("|") Then
                            lblActionText.Text = aStrCode(1).Split("|")(0)
                            txtActionUserCode.Text = aStrCode(1).Split("|")(1)
                        Else
                            lblActionText.Text = aStrCode(1)
                        End If
                        strCodeToInsert = strCodeToInsert.Replace(aStrCode(1), "")
                    End If
                    If cmbObjects.SelectedItem IsNot Nothing AndAlso cmbObjects.SelectedItem.ToString.Split("(")(0).Trim = _EditedControl.Name Then
                        strCodeToInsert = strCodeToInsert.Replace("GOLFindObject(ID_[CONTROLID_NOINDEX][CONTROLID_INDEX])", "pObj")
                    End If
                    strCodeToInsert = strCodeToInsert.Replace("[CONTROLID]", ParentScreenName & "_" & strActionSelectedControlName) _
                        .Replace("[CONTROLID_NOINDEX]", ParentScreenName & "_" & strActionSelectedControlName) _
                        .Replace("[CONTROLID_INDEX]", "") _
                        .Replace("[SCREEN_NAME]", strActionSelectedControlName) _
                        .Replace("[SCREEN_UPPERNAME]", strActionSelectedControlName.ToUpper) _
                        .Replace("[NEWLINE]", vbCrLf)
                End If
            Catch ex As Exception
            End Try
        End Sub

        Private Sub PopulateComboBitmaps()
            cmbBitmap.Items.Clear()
            For Each bmp As VGDDImage In Common.Bitmaps
                cmbBitmap.Items.Add(bmp.Name)
            Next
            cmbBitmap.Text = ""
        End Sub

        Private Sub cmbBitmap_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbBitmap.SelectedIndexChanged
            GetCodeToInsert()
            If strCodeToInsert.Contains("//") Then
                strCodeToInsert = strCodeToInsert.Substring(0, strCodeToInsert.IndexOf("//")) & vbCrLf 'TODO: Multiline?
            End If
            strCodeToInsert = strCodeToInsert.Replace("%BITMAP%", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & cmbBitmap.SelectedItem)
        End Sub

        Private Sub btnNewBitmap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewBitmap.Click
            Dim oBitmapChooser As New frmBitmapChooser
            oBitmapChooser.ShowDialog()
            PopulateComboBitmaps()
            If oBitmapChooser.ChosenBitmap IsNot Nothing Then
                cmbBitmap.SelectedItem = oBitmapChooser.ChosenBitmap.Name
            End If
        End Sub
#End If

        Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            If MessageBox.Show("Do you want to restore this event's code to its previous contents, before this last editing?", "Cancel editing?", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                Me.TextEditorControl1.ActiveEditor.Document.TextContent = strOriginalCode
                btnCancel.Visible = False
            End If
        End Sub

        Private Sub TextDocument_UpdateCommited(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextDocument.UpdateCommited
            tmrUpdateCode.Enabled = True
        End Sub

        Private Sub tmrUpdateCode_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrUpdateCode.Tick
            tmrUpdateCode.Enabled = False
            Try
                If oSelectedEvent Is Nothing OrElse cmbEvents.Text = String.Empty OrElse Common.aScreens.Count = 0 Then Exit Sub
                Dim strCode As String = TextEditorControl1.ActiveEditor.Document.TextContent
                If strOldCode <> strCode Then
                    Dim oEvent As VGDDEvent = _EventsCollection(cmbEvents.Text)
                    If oEvent IsNot Nothing Then
                        oEvent.Code = strCode
                        oEvent.Handled = (strCode.Trim <> String.Empty)
                        _EventsPropertyDescriptor.SetValue(_EditedControl, _EventsCollection)
                        If strOldCode = String.Empty Then
                            oMainShell.PopulateControlEvents(_EditedControl, True)
                        End If
                        strOldCode = strCode
                        oMainShell.ScreenChanged()
                        btnCancel.Visible = True
                    End If
                End If

            Catch ex As Exception

            End Try
        End Sub

    End Class
End Namespace
