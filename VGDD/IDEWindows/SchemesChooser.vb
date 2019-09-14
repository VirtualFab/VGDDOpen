Imports VGDDCommon
Imports System.Xml
Imports System.IO
Imports VirtualFabUtils.Utils

Namespace VGDDIDE

    Public Class SchemesChooser
        Inherits WeifenLuo.WinFormsUI.Docking.DockContent

        Public Event SchemePropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs)
        Public Event ApplyScheme(ByVal SelectedSchemeName As String)
        Public Event SelectedSchemeChanged(ByVal SelectedScheme As Object)
        Public Event SelectedSchemeFontChanged(ByVal SelectedScheme As Object)
        Public Event DeleteScheme(ByVal SchemeToDelete As Object)
        Public Event NewScheme()

        Private WithEvents _SelectedScheme As VGDDScheme

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.ShowInTaskbar = False
            Me.TopLevel = False
        End Sub

        Private Sub SchemesChooser_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _SchemePropertyGrid.SendToBack()
        End Sub

        Public Sub Clear()
            cmbScheme.DataSource = Nothing
        End Sub

        Public Sub LoadSchemes(ByVal SchemeList As Dictionary(Of String, VGDDCommon.VGDDScheme), ByVal SelectedSchemeName As String)
            With cmbScheme
                '.Items.Clear()
                'For Each oScheme As VGDDScheme In _Schemes
                '    .Items.Add(oScheme)
                'Next
                .DataSource = Nothing
                Application.DoEvents()
                .DisplayMember = "Name"
                .ValueMember = "Name"
                Dim aSchemes(SchemeList.Count - 1) As VGDDCommon.VGDDScheme
                SchemeList.Values.CopyTo(aSchemes, 0)
                .DataSource = aSchemes
                .Refresh()
                If SelectedSchemeName IsNot Nothing Then
                    'For Each oScheme As VGDDScheme In cmbScheme.Items
                    '    If oScheme.Name = SelectedSchemeName Then
                    '        cmbScheme.SelectedItem = oScheme
                    '    End If
                    'Next
                    cmbScheme.Text = SelectedSchemeName
                End If
            End With
        End Sub

        Private Sub cmbScheme_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbScheme.SelectedIndexChanged
            If _SelectedScheme IsNot Nothing Then
                RemoveHandler _SelectedScheme.FontChanged, AddressOf SchemeFontChanged
                RemoveHandler _SelectedScheme.PropertiesChanged, AddressOf SchemePropertiesChanged
            End If
            If cmbScheme.SelectedIndex = -1 Then
                RaiseEvent SelectedSchemeChanged(Nothing)
                mnuDelete.Enabled = False
                _SelectedScheme = Nothing
            Else
                mnuDelete.Enabled = True
                _SelectedScheme = cmbScheme.SelectedItem
                With _SchemePropertyGrid
                    '.Text = _SelectedScheme.Name
                    .SelectedObject = _SelectedScheme
                    '.PropertySort = PropertySort.Alphabetical
                    AddHandler _SelectedScheme.FontChanged, AddressOf SchemeFontChanged
                    AddHandler _SelectedScheme.PropertiesChanged, AddressOf SchemePropertiesChanged
                End With
                RaiseEvent SelectedSchemeChanged(cmbScheme.SelectedItem)
            End If
        End Sub

        Private Sub SchemeFontChanged()
            RaiseEvent ApplyScheme(cmbScheme.Text)
        End Sub

        Private Sub SchemePropertiesChanged()
            'Dim CurrentScheme As Object = _SchemePropertyGrid.SelectedObject
            '_SchemePropertyGrid.SelectedObject = Nothing
            'Application.DoEvents()
            '_SchemePropertyGrid.SelectedObject = CurrentScheme
            _SchemePropertyGrid.Refresh()
        End Sub

        Private Sub _SchemePropertyGrid_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles _SchemePropertyGrid.PropertyValueChanged
            Select Case e.ChangedItem.Label
                Case "(Name)", "Name"
                    If Common._Schemes.ContainsKey(e.ChangedItem.Value) Then
                        MessageBox.Show("A scheme named " & e.ChangedItem.Value & " already exists!")
                        CType(_SchemePropertyGrid.SelectedObject, VGDDScheme).Name = e.OldValue
                        _SchemePropertyGrid.Refresh()
                        Exit Sub
                    End If
            End Select
            RaiseEvent SchemePropertyValueChanged(s, e)
        End Sub

        Public Property SelectedObject As Object
            Get
                Return _SchemePropertyGrid.SelectedObject
            End Get
            Set(ByVal value As Object)
                _SchemePropertyGrid.SelectedObject = value
            End Set
        End Property

        Public Property SelectedSchemeName As String
            Get
                Return cmbScheme.Text
            End Get
            Set(ByVal value As String)
                cmbScheme.Text = value
            End Set
        End Property

        Private Sub btnApplyScheme_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApplyScheme.Click
            RaiseEvent ApplyScheme(cmbScheme.Text)
        End Sub

        Private Sub mnuDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDelete.Click
            Dim oScheme As VGDDScheme = cmbScheme.SelectedItem
            If oScheme IsNot Nothing Then
                Dim UsedBy As ArrayList = oScheme.UsedBy.Clone
                Dim strWarning As String = String.Empty
                If UsedBy.Count > 0 Then
                    strWarning = "Scheme " & oScheme.Name & " is currently used by " & UsedBy.Count & " widgets in the project." & vbCrLf
                End If
                If MessageBox.Show(strWarning & "Are you sure you want to delete scheme " & CType(cmbScheme.SelectedItem, VGDDScheme).Name & "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then
                    RaiseEvent DeleteScheme(cmbScheme.SelectedItem)
                    If UsedBy.Count > 0 Then
                        oScheme = cmbScheme.Items(0)
                        Try
                            For Each oWidget As VGDDMicrochip.VGDDWidget In UsedBy
                                oWidget.Scheme = oScheme.Name
                            Next
                            MessageBox.Show(UsedBy.Count & " Widgets set to Scheme " & oScheme.Name)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End If
        End Sub

        Private Sub mnuNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuNew.Click
            RaiseEvent NewScheme()
        End Sub

        Private Sub mnuSetAsDefault_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSetAsDefault.Click
            Dim oScheme As VGDDScheme = cmbScheme.SelectedItem
            My.Settings.DefaultSchemeXML = oScheme.ToXml
            My.Settings.Save()
            MainShell.LoadDefaultScheme()
        End Sub

        Private Sub mnuExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuExport.Click
            ExportScheme(cmbScheme.SelectedItem)
        End Sub

        Private Sub mnuImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImport.Click
            ImportScheme()
        End Sub

        Private Sub _SelectedScheme_FontChanged() Handles _SelectedScheme.FontChanged
            RaiseEvent SelectedSchemeFontChanged(cmbScheme.SelectedItem)
        End Sub

        Public Sub ExportScheme(ByVal oScheme As VGDDScheme)
            Dim dlg As New SaveFileDialog
            dlg.Title = "Export Scheme"
            dlg.DefaultExt = "vsc"
            dlg.Filter = "Visual Graphics Scheme Files|*.vsc"
            dlg.FileName = oScheme.Name & ".vsc"
            If (dlg.ShowDialog = DialogResult.OK) Then
                Dim oXmlSchemeDoc As XmlDocument = New XmlDocument
                oXmlSchemeDoc.AppendChild(oXmlSchemeDoc.CreateElement("VGDDScheme"))
                oScheme.ToXml(oXmlSchemeDoc)
                Dim sw As StringWriter
                sw = New StringWriter
                Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
                xtw.Formatting = Formatting.Indented
                oXmlSchemeDoc.WriteTo(xtw)
                Try
                    WriteFile(dlg.FileName, sw.ToString, New System.Text.UnicodeEncoding)
                Catch ex As Exception
                    MessageBox.Show("Error writing to " & dlg.FileName & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Sub

        Public Sub ImportScheme()
            Dim dlg As New OpenFileDialog
            dlg.Title = "Import Scheme"
            dlg.DefaultExt = "vsc"
            dlg.Filter = "Visual Graphics Scheme Files|*.vsc"
            If (dlg.ShowDialog = DialogResult.OK) Then
                Try
                    Dim oXmlSchemeDoc As New XmlDocument
                    oXmlSchemeDoc.Load(dlg.FileName)
                    Dim oScheme As New VGDDScheme
                    For Each oNode As XmlNode In oXmlSchemeDoc.DocumentElement.ChildNodes
                        oScheme.FromXml(oNode)
                        If oScheme.Font Is Nothing Then
                            oScheme.Font = Common._Schemes("Default").Font
                        End If
                        Common._Schemes.Add(oScheme.Name, oScheme)
                    Next
                    Me.LoadSchemes(Common._Schemes, cmbScheme.SelectedValue)

                Catch ex As Exception
                    MessageBox.Show("Error reading Scheme from " & dlg.FileName & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Sub

        Private Sub _SelectedScheme_PropertiesChanged() Handles _SelectedScheme.PropertiesChanged
            mnuDelete.Enabled = True
            If _SelectedScheme.Name = "Default" Then mnuDelete.Enabled = False
        End Sub

    End Class
End Namespace
