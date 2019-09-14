Imports System.ComponentModel

Public Class UiEditKeyCollection
    Inherits Drawing.Design.UITypeEditor

    Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As Drawing.Design.UITypeEditorEditStyle
        Return Drawing.Design.UITypeEditorEditStyle.DropDown
    End Function

    Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
        Dim wfes As Windows.Forms.Design.IWindowsFormsEditorService = _
            TryCast(provider.GetService(GetType(Windows.Forms.Design.IWindowsFormsEditorService)), Windows.Forms.Design.IWindowsFormsEditorService)

        If wfes IsNot Nothing Then
            Dim _frmUiEditKey As New frmUIEditKeyCollection
            _frmUiEditKey._wfes = wfes
            _frmUiEditKey.context = context
            _frmUiEditKey.Value = value
            wfes.DropDownControl(_frmUiEditKey)
            Dim oNewCollection As New VGDDCommon.VGDDKeyCollection
            For Each oKey As VGDDCommon.VGDDKey In _frmUiEditKey.Value
                oNewCollection.Add(oKey)
            Next
            Return oNewCollection
        Else
            Return MyBase.EditValue(context, provider, value)
        End If
    End Function
End Class
