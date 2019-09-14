
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Partial Public Class FontDialog
    Inherits Form
    Private _SelectedFont As Font
    Public Sub New()
        InitializeComponent()
        lstFont.SelectedFontFamily = FontFamily.GenericSansSerif
        lstFont.AddSelectedFontToRecent()
        txtSize.Text = Convert.ToString(10)
    End Sub

    Public Property SelectedFont() As Font
        Get
            Return _SelectedFont
        End Get
        Set(ByVal value As Font)
            _SelectedFont = value
            lstFont.AddFontToRecent(value.FontFamily)
            lstFont.SelectedFontFamily = value.FontFamily
            txtSize.Text = value.Size.ToString()
            chbBold.Checked = value.Bold
            chbItalic.Checked = value.Italic
            chbStrikeout.Checked = value.Strikeout
        End Set
    End Property

    Public Sub AddFontToRecentList(ByVal ff As FontFamily)
        lstFont.AddFontToRecent(ff)
    End Sub

    Private Sub lstFont_SelectedFontFamilyChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lstFont.SelectedFontFamilyChanged
        UpdateSampleText()
    End Sub

    Private Sub lstSize_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lstSize.SelectedIndexChanged
        If lstSize.SelectedItem IsNot Nothing Then
            txtSize.Text = lstSize.SelectedItem.ToString()
        End If
    End Sub

    Private Sub txtSize_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSize.TextChanged
        If lstSize.Items.Contains(txtSize.Text) Then
            lstSize.SelectedItem = txtSize.Text
        Else
            lstSize.ClearSelected()
        End If

        UpdateSampleText()
    End Sub


    Private Sub txtSize_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles txtSize.KeyDown
        Select Case e.KeyData
            Case Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, _
             Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.[End], Keys.Enter, _
             Keys.Home, Keys.Back, Keys.Delete, Keys.Escape, Keys.Left, Keys.Right
                Exit Select
            Case Keys.[Decimal], DirectCast(190, Keys)
                'decimal point
                If txtSize.Text.Contains(".") Then
                    e.SuppressKeyPress = True
                    e.Handled = True
                End If
                Exit Select
            Case Else
                e.SuppressKeyPress = True
                e.Handled = True
                Exit Select
        End Select

    End Sub

    Private Sub UpdateSampleText()
        Try
            Dim size As Single = If(txtSize.Text <> "", Single.Parse(txtSize.Text), 1)
            Dim style As FontStyle = If(chbBold.Checked, FontStyle.Bold, FontStyle.Regular)
            If chbItalic.Checked Then
                style = style Or FontStyle.Italic
            End If
            If chbStrikeout.Checked Then
                style = style Or FontStyle.Strikeout
            End If
            _SelectedFont = New Font(lstFont.SelectedFontFamily, size, style)
            lblSampleText.Font = _SelectedFont
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Handles CheckedChanged event for Bold, 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub chb_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chbBold.CheckedChanged, chbItalic.CheckedChanged, chbStrikeout.CheckedChanged
        UpdateSampleText()
    End Sub

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOK.Click
        lstFont.AddSelectedFontToRecent()
    End Sub

End Class

