Imports System.ComponentModel

Public Class UiEditPalette
    Inherits Drawing.Design.UITypeEditor

    Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As Drawing.Design.UITypeEditorEditStyle
        Return Drawing.Design.UITypeEditorEditStyle.DropDown
    End Function

    Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
        Dim wfes As Windows.Forms.Design.IWindowsFormsEditorService = _
            TryCast(provider.GetService(GetType(Windows.Forms.Design.IWindowsFormsEditorService)), Windows.Forms.Design.IWindowsFormsEditorService)

        If wfes IsNot Nothing Then
            Dim _frmUiPalette As New frmUIEditPalette
            _frmUiPalette._wfes = wfes
            _frmUiPalette.context = context
            _frmUiPalette.Palette = value
            wfes.DropDownControl(_frmUiPalette)
            Dim oNewCollection As New VGDDCommon.VGDDKeyCollection
            Return _frmUiPalette.Palette
        Else
            Return MyBase.EditValue(context, provider, value)
        End If
    End Function
End Class
