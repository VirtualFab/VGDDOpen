Imports VGDDCommon
Imports System.IO

Partial Public Class frmGenCode

    Public Event Register As EventHandler
    Public Event GenerateCode As EventHandler
    Public Event ProjectModified As EventHandler

    Dim blnSettingSelection As Boolean = False
    Private blnLoading As Boolean = False
    Private WithEvents _SourceDocument As ICSharpCode.TextEditor.Document.SelectionManager

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    'Private Sub frmGenCode_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
    '    If Me.Opacity = 0 Then
    '        FadeForm.FadeIn(Me, 99)
    '    End If
    'End Sub

    'Private Sub frmGenCode_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    '    FadeForm.FadeOutAndWait(Me)
    'End Sub

    Private Sub frmGenCode_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Common.ProjectLoading = True

#If CONFIG = "Debug" Then
        btnGetItems.Visible = True
#End If

        chkQuickCodeGen.Checked = Common.QuickCodeGen
        Common.ProjectLoading = False
        btnShowSource.Enabled = False
        pnlProgress.Visible = False
        'FootPrint1.Visible = False
        If Common.DoFadings Then Me.Opacity = 0
        For Each oBitmap As VGDDImage In Common._Bitmaps
            If oBitmap.ToBeConverted AndAlso oBitmap.Referenced Then
                Common.ResourcesToConvert = True
            End If
        Next
        For Each VFont As VGDDFont In Common._Fonts
            If VFont.ToBeConverted Then
                Common.ResourcesToConvert = True
            End If
        Next
        chkQuickCodeGen.Checked = Not Common.ResourcesToConvert
        CodeGenLocation1.Framework1.CheckMal()
    End Sub

    Public Sub LoadText(ByVal strText As String)
        Me.WindowState = FormWindowState.Maximized
        With Me.SourceEditor
            .ActiveEditor.Document.TextContent = strText
            .ActiveEditor.Document.HighlightingStrategy() =
                                ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("MCHP_C")
            .Visible = True
            .BringToFront()
            .ActiveEditor.IsReadOnly = True
            _SourceDocument = .ActiveEditor.ActiveTextAreaControl.TextArea.SelectionManager
        End With
    End Sub

    Private Sub _SourceDocument_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SourceDocument.SelectionChanged
        If pnlUnregistered.Visible AndAlso Not blnLoading And Not blnSettingSelection Then
            blnSettingSelection = True
            _SourceDocument.ClearSelection()
            blnSettingSelection = False
        End If
    End Sub

    Private Sub lblLnkRegister_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblLnkRegister.Click
        RaiseEvent Register(Nothing, Nothing)
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If Common.CodegenProgress <= Me.ProgressBar1.Maximum Then
            Me.ProgressBar1.Value = Common.CodegenProgress
        End If
        ShowFootPrint()
    End Sub

    Public Sub ShowFootPrint()
        FootPrint1.UpdatePanel()
    End Sub

    Private Sub btnGenerateCode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateCode.Click
        Try
            btnGenerateCode.Enabled = False
            btnClose.Enabled = False
            Me.SuspendLayout()
            Me.Height += pnlProgressBar.Height ' + Me.DefaultMargin.Vertical * 2
            pnlProgressBar.Visible = True
            pnlProgressBar.Top = pnlGenerateOptions.Top + pnlGenerateOptions.Height ' lblBinBitmaps.Top + lblBinBitmaps.Height + 8 ' +  - Me.DefaultMargin.Vertical * 2
            Me.ResumeLayout()
            Application.DoEvents()
            RaiseEvent GenerateCode(Nothing, Nothing)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "btnGenerateCode_Click")
        End Try
    End Sub

    Private Sub btnShowSource_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowSource.Click
        Me.LoadText(CodeGen.FinalCode)
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnBinBitmaps_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBinBitmaps.Click
        Try
            System.Diagnostics.Process.Start(Common.BitmapsBinPath)
        Catch ex As Exception
            MessageBox.Show("Error opening folder " & Common.BitmapsBinPath & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub chkQuickGen_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkQuickCodeGen.CheckedChanged
        Common.QuickCodeGen = chkQuickCodeGen.Checked
    End Sub

    Private Sub CodeGenLocation1_OptionsChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CodeGenLocation1.OptionsChanged
        btnGenerateCode.Enabled = Common.CodeGenLocationOptionsOk
        Common.QuickCodeGen = False
        chkQuickCodeGen.Checked = False
    End Sub

    Private Sub btnGetItems_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetItems.Click
        Dim cItems As ArrayList = MplabX.MPLABXGetAllItemsInFolder(MplabX.oProjectXmlDoc.DocumentElement)
        Dim strLastFolderName As String = String.Empty
        Dim sb As New System.Text.StringBuilder
        For Each strKey As String In cItems
            Dim strFolderName As String = strKey.Split("|")(0)
            Dim strFile As String = strKey.Split("|")(1)
            If strLastFolderName <> strFolderName Then
                If strLastFolderName <> String.Empty Then
                    sb.Append("</Folder>")
                    sb.Append(vbCrLf)
                End If
                If strFolderName.EndsWith("/") Then strFolderName = strFolderName.Substring(0, strFolderName.Length - 1)
                strLastFolderName = strFolderName
                sb.Append(String.Format("<Folder Name=""{0}"">", strFolderName))
                sb.Append(vbCrLf)
            End If
            sb.Append(String.Format("    <AddFile>{0}</AddFile>", strFile))
            sb.Append(vbCrLf)
        Next
        sb.Append("</Folder>")
        Dim strFileName As String = Path.Combine(Path.GetDirectoryName(Common.ProjectPathName), "MplabXItems.txt")
        Dim sw As New StreamWriter(strFileName)
        sw.Write(sb.ToString)
        sw.Flush()
        sw.Close()
        Process.Start("notepad.exe", strFileName)
    End Sub

End Class