Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Windows.Forms
Imports System.Windows.Forms.Design
Imports System.Collections.Specialized
Imports System.Drawing.Drawing2D
Imports System.ComponentModel.Design
Imports VGDDCommon

Namespace VGDDCommon

    ' IWindowsFormsEditorService
    Public Class MyColorEditor
        Inherits System.Drawing.Design.ColorEditor ' UITypeEditor

        Private service As IWindowsFormsEditorService
        Private WithEvents oColorPicker As ColorPicker
        Private PickedColor As Color = Nothing

        Public Sub New()
            oColorPicker = New ColorPicker
            If Common.SelectedScheme IsNot Nothing Then
                oColorPicker.Palette = Common.SelectedScheme.Palette
            End If
        End Sub

        Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As UITypeEditorEditStyle
            ' This tells it to show the [...] button which is clickable firing off EditValue below.
            Return UITypeEditorEditStyle.Modal
        End Function

        Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
            If provider IsNot Nothing Then
                service = DirectCast(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
            End If
            If service IsNot Nothing Then
                oColorPicker.Value = value
                If Common.SelectedScheme.Palette IsNot Nothing Then
                    oColorPicker.Palette = Common.SelectedScheme.Palette
                End If
                'oColorDialog.ColourDepth = Common.ProjectColourDepth
                service.DropDownControl(oColorPicker)
                If PickedColor <> Nothing Then
                    value = oColorPicker.Value
                End If
            End If
            Return value
        End Function

        Private Sub oColorPicker_ColorPicked(ByVal sender As Object) Handles oColorPicker.ColorPicked
            PickedColor = oColorPicker.Value
            service.CloseDropDown()
            Common.ProjectChanged = True
        End Sub

        'Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
        '    If provider IsNot Nothing Then
        '        Dim service As IWindowsFormsEditorService = DirectCast(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
        '        If service Is Nothing Then
        '            Return value
        '        End If

        '        If _colorUI Is Nothing Then
        '            _colorUI = New ColorPicker(Me)
        '        End If

        '        _colorUI.Start(service, value)
        '        service.DropDownControl(_colorUI.Control)
        '        If (_colorUI.Value IsNot Nothing) AndAlso (DirectCast(_colorUI.Value, Color) <> Color.Empty) Then
        '            value = _colorUI.Value
        '        End If
        '        _colorUI.[End]()
        '    End If
        '    Return value
        'End Function

    End Class

End Namespace
