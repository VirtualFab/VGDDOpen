Imports System.ComponentModel

Public Class UiEditTextMultiLanguage
    Inherits Drawing.Design.UITypeEditor

    Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As Drawing.Design.UITypeEditorEditStyle
        Return Drawing.Design.UITypeEditorEditStyle.DropDown
    End Function

    Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
        Dim wfes As Windows.Forms.Design.IWindowsFormsEditorService = _
            TryCast(provider.GetService(GetType(Windows.Forms.Design.IWindowsFormsEditorService)), Windows.Forms.Design.IWindowsFormsEditorService)
        Dim oWidget As VGDDMicrochip.VGDDBase = TryCast(context.Instance, VGDDMicrochip.VGDDBase)
        If wfes Is Nothing OrElse VGDDCommon.Common.ProjectMultiLanguageTranslations = 0 OrElse oWidget Is Nothing Then
            Dim oEdit As New System.ComponentModel.Design.MultilineStringEditor
            Return oEdit.EditValue(context, provider, value)
            'Return MyBase.EditValue(context, provider, value)
        Else
            Dim TextStringIDName As String = "TextStringID"
            For Each oAttribute As Attribute In context.PropertyDescriptor.Attributes
                If TypeOf (oAttribute) Is VGDDCommon.AttributeTextStringIDName Then
                    TextStringIDName = CType(oAttribute, VGDDCommon.AttributeTextStringIDName).TextStringIdName
                End If
            Next
            Dim TextStringID As Integer
            If TextStringIDName = "TextStringID" Then
                TextStringID = oWidget.TextStringID
            Else
                TextStringID = TypeDescriptor.GetProperties(context.Instance)(TextStringIDName).GetValue(context.Instance)
            End If
            If TextStringID = 0 Then
                For i As Integer = 1 To VGDDCommon.Common.ProjectStringPool.Count
                    Dim oStringSet As VGDDCommon.MultiLanguageStringSet = VGDDCommon.Common.ProjectStringPool(i)
                    If oStringSet.Strings(0) = oWidget.Text Then
                        If TextStringIDName = "TextStringID" Then
                            oWidget.TextStringID = i
                        Else
                            TypeDescriptor.GetProperties(context.Instance)(TextStringIDName).SetValue(context.Instance, i)
                        End If
                        Exit For
                    End If
                Next
            End If
            Dim _frmUiEditKey As New frmUIEditTextMultiLanguage
            _frmUiEditKey._wfes = wfes
            _frmUiEditKey.provider = provider
            _frmUiEditKey.context = context
            _frmUiEditKey.Value = TextStringID
            'If IsNumeric(value) Then
            '    _frmUiEditKey.Value = value
            'End If
            wfes.DropDownControl(_frmUiEditKey)
            If TextStringIDName = "TextStringID" Then
                oWidget.TextStringID = _frmUiEditKey.Value
            Else
                TypeDescriptor.GetProperties(context.Instance)(TextStringIDName).SetValue(context.Instance, CInt(_frmUiEditKey.Value))
            End If
            If VGDDCommon.Common.ProjectStringPool.ContainsKey(_frmUiEditKey.Value) Then
                Dim oStringSet As VGDDCommon.MultiLanguageStringSet = VGDDCommon.Common.ProjectStringPool(_frmUiEditKey.Value)
                'If oWidget IsNot Nothing Then
                '    oStringSet.FontName = oWidget.SchemeObj.Font.Name
                'End If
                Dim strNew As String = oStringSet.Strings(0).ToString
                Return strNew
            Else
                Return _frmUiEditKey.Value
            End If
        End If
    End Function
End Class
