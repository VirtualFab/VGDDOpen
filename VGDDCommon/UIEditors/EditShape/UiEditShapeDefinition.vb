Imports System.ComponentModel

Public Class UiEditShapeDefinition
    Inherits Drawing.Design.UITypeEditor

    Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As Drawing.Design.UITypeEditorEditStyle
        Return Drawing.Design.UITypeEditorEditStyle.DropDown
    End Function

    Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
        Dim wfes As Windows.Forms.Design.IWindowsFormsEditorService = _
            TryCast(provider.GetService(GetType(Windows.Forms.Design.IWindowsFormsEditorService)), Windows.Forms.Design.IWindowsFormsEditorService)

        If wfes IsNot Nothing Then
            Dim _frmUiEditShapeCollection As New frmUIEditShapeDefinition
            _frmUiEditShapeCollection._wfes = wfes
            _frmUiEditShapeCollection.context = context
            _frmUiEditShapeCollection.Value = value
            If wfes.ShowDialog(_frmUiEditShapeCollection) = Windows.Forms.DialogResult.OK Then
                Dim oNewCollection As New VGDDMicrochip.VGDDShapeDefinition
                For Each oItem As VGDDMicrochip.VGDDShapeItem In _frmUiEditShapeCollection.Value
                    oNewCollection.Add(oItem)
                Next
                Return oNewCollection
            Else
                Return value
            End If
        Else
            Return MyBase.EditValue(context, provider, value)
        End If
    End Function
End Class
