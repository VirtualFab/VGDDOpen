Imports System.ComponentModel

Public Class UiEditInteger
    Inherits Drawing.Design.UITypeEditor
    Private Shared _frmUiEditInteger As frmUIEditInteger

    Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As Drawing.Design.UITypeEditorEditStyle
        Return Drawing.Design.UITypeEditorEditStyle.DropDown
    End Function

    Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
        Static LastValue As Integer
        Dim wfes As Windows.Forms.Design.IWindowsFormsEditorService = _
            TryCast(provider.GetService(GetType(Windows.Forms.Design.IWindowsFormsEditorService)), Windows.Forms.Design.IWindowsFormsEditorService)

        If wfes IsNot Nothing Then
            If _frmUiEditInteger Is Nothing Then
                _frmUiEditInteger = New frmUiEditInteger
            End If
            _frmUiEditInteger._wfes = wfes
            _frmUiEditInteger.context = context
            _frmUiEditInteger.TrackBar1.Maximum = Int16.MaxValue
            _frmUiEditInteger.TrackBar1.Minimum = 0
            _frmUiEditInteger.NumericUpDown1.Maximum = Int16.MaxValue
            _frmUiEditInteger.NumericUpDown1.Minimum = 0
            _frmUiEditInteger.UpdateFromTrackBar = False
            _frmUiEditInteger.UpdateFromUpDown = True
            Try
                Dim oType As Type = context.Instance.GetType
                Dim aPropertyName As String() = provider.ToString.Split(" ")
                If aPropertyName.Length > 1 Then
                    Dim strPropertyName As String = aPropertyName(1)

                    Dim pi As Reflection.PropertyInfo = oType.GetProperty(strPropertyName)
                    If pi IsNot Nothing Then
                        For Each oAttribute As Attribute In Attribute.GetCustomAttributes(pi, True)
                            Dim oRange As VGDDCommon.GOLRange = TryCast(oAttribute, VGDDCommon.GOLRange)
                            If oRange IsNot Nothing Then
                                Dim oScreen As VGDD.VGDDScreen = context.Instance.Parent
                                Select Case strPropertyName
                                    Case "Top"
                                        oRange.Max = oScreen.Height - 1
                                    Case "Bottom", "Height"
                                        oRange.Max = oScreen.Height
                                    Case "Left"
                                        oRange.Max = oScreen.Width - 1
                                    Case "Right", "Width"
                                        oRange.Max = oScreen.Width
                                End Select
                                If oRange.MaxPropertyName IsNot Nothing Then
                                    Try
                                        oRange.Max = oType.GetProperty(oRange.MaxPropertyName).GetValue(context.Instance, Nothing)
                                    Catch ex As Exception
                                    End Try
                                End If
                                _frmUiEditInteger.TrackBar1.Maximum = oRange.Max
                                _frmUiEditInteger.TrackBar1.Minimum = oRange.Min
                                _frmUiEditInteger.NumericUpDown1.Maximum = oRange.Max
                                _frmUiEditInteger.NumericUpDown1.Minimum = oRange.Min
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
            End Try
            Do
                LastValue = value
                If value > _frmUiEditInteger.NumericUpDown1.Maximum Then
                    value = _frmUiEditInteger.NumericUpDown1.Maximum
                End If
                _frmUiEditInteger.NumericUpDown1.Value = CInt(value)
                wfes.DropDownControl(_frmUiEditInteger)
                value = _frmUiEditInteger.Value
                If value = LastValue Or _frmUiEditInteger.UpdateFromTrackBar Then
                    Exit Do
                End If
            Loop
            Return value
        Else
            Return MyBase.EditValue(context, provider, value)
        End If
    End Function
End Class
