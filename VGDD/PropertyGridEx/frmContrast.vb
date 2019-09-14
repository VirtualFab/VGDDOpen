Imports System.Windows.Forms.Design

Public Class frmContrast
    Public _wfes As IWindowsFormsEditorService
    Public Event ValueChanged()

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.TopLevel = False
    End Sub

    Private Sub frmContrast_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        _wfes.CloseDropDown()
    End Sub

    Private Sub NumericUpDown1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        RaiseEvent ValueChanged()
    End Sub
End Class