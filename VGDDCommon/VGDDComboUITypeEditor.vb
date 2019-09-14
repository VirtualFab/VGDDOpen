Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Windows.Forms.Design
Imports System.Windows.Forms

Namespace VGDDCommon
    Friend Class VGDDComboUITypeEditor
        Inherits UITypeEditor

        Private edSvc As IWindowsFormsEditorService

        Public Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
            Return UITypeEditorEditStyle.DropDown
        End Function

        Public Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As System.IServiceProvider, ByVal value As Object) As Object
            edSvc = CType(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
            If edSvc IsNot Nothing Then
                Dim lb As New ListBox
                If TypeOf context.Instance Is VGDDCustomProp Then
                    If context.PropertyDescriptor.Name = "StrType" Then
                        lb.Items.AddRange({"String", "Int", "Float", "Bool", "Bitmap"})
                        Dim oCustProp As VGDDCustomProp = context.Instance
                        Select Case oCustProp.StrType
                            Case "Bool"
                                If Not Boolean.TryParse(oCustProp.Defaultvalue, Nothing) Then
                                    oCustProp.Defaultvalue = False
                                End If
                            Case "Int"
                                If Not Integer.TryParse(oCustProp.Defaultvalue, Nothing) Then
                                    oCustProp.Defaultvalue = 0
                                End If
                        End Select
                    ElseIf context.PropertyDescriptor.Name = "Category" Then
                        lb.Items.AddRange({"Appearance", "Layout", "Misc", "State"})
                    End If
                    'If lb.Items.Contains(defaultValue) Then
                    '    lb.SelectedItem = defaultValue
                    'End If
                    AddHandler lb.SelectedIndexChanged, AddressOf SelectionComplete
                    edSvc.DropDownControl(lb)
                    value = lb.SelectedItem
                End If
            End If
            Return value
        End Function

        Private Sub SelectionComplete(ByVal sender As Object, ByVal e As EventArgs)
            If edSvc IsNot Nothing Then
                edSvc.CloseDropDown()
            End If
        End Sub
    End Class
End Namespace
