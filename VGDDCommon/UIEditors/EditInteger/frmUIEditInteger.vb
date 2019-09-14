Imports System.Windows.Forms.Design
Imports System.ComponentModel

Public Class frmUIEditInteger
    Public _wfes As IWindowsFormsEditorService
    Public Event ValueChanged()
    Public context As ITypeDescriptorContext
    Private _Value As Integer
    Private UpdateProperty As Boolean = True
    Public UpdateFromTrackBar As Boolean = False
    Public UpdateFromUpDown As Boolean = False

    Public Sub New()
        InitializeComponent()
        Me.TopLevel = False
    End Sub

    Public Property Value As Integer
        Get
            Return _Value
        End Get
        Set(ByVal value As Integer)
            If value <> _Value Then
                _Value = value
                If UpdateFromUpDown AndAlso value <= TrackBar1.Maximum AndAlso value >= TrackBar1.Minimum Then
                    TrackBar1.Value = value
                ElseIf UpdateFromTrackBar AndAlso value <= NumericUpDown1.Maximum AndAlso value >= NumericUpDown1.Minimum Then
                    NumericUpDown1.Value = value
                End If
                If UpdateProperty AndAlso _wfes IsNot Nothing Then
                    Try
                        context.PropertyDescriptor.SetValue(context.Instance, value)
                    Catch ex As Exception
                    End Try
                End If
            End If
        End Set
    End Property

    Private Sub NumericUpDown1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NumericUpDown1.Click
        UpdateFromTrackBar = False
        UpdateFromUpDown = True
    End Sub

    Private Sub NumericUpDown1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        If UpdateFromUpDown Then
            UpdateProperty = True
            Value = NumericUpDown1.Value
        End If
    End Sub

    Private Sub TrackBar1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrackBar1.MouseUp
        UpdateFromTrackBar = False
        UpdateFromUpDown = True
        UpdateProperty = True
        _Value = -1
        Value = TrackBar1.Value
    End Sub

    Private Sub TrackBar1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TrackBar1.MouseDown
        UpdateFromTrackBar = True
        UpdateFromUpDown = False
    End Sub

    Private Sub TrackBar1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrackBar1.ValueChanged
        If UpdateFromTrackBar Then
            UpdateProperty = False
            Value = TrackBar1.Value
        End If
    End Sub
End Class