
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Partial Public Class FontList
    Inherits UserControl

    Public Event SelectedFontFamilyChanged As EventHandler
    Public RecentlyUsed As New RecentlyUsedList(Of Font)(5)
    Private lastSelectedIndex As Integer = -1

    Public Sub New()
        InitializeComponent()


        lstFont.Items.Add("Section")
        'section entry for Recently Used
        lstFont.Items.Add("Section")
        'section entry for All Fonts
        For Each f As FontFamily In FontFamily.Families
            Try
                If f.Name IsNot Nothing OrElse f.Name <> "" Then
                    If f.IsStyleAvailable(FontStyle.Regular) Then
                        lstFont.Items.Add(New Font(f, 12))
                    End If
                End If
            Catch

            End Try


        Next
    End Sub

    Private Const RecentlyUsedSectionIndex As Integer = 0
    Private ReadOnly Property AllFontsSectionIndex() As Integer
        Get
            Return RecentlyUsed.Count + 1
        End Get
    End Property

    Private ReadOnly Property AllFontsStartIndex() As Integer
        Get
            Return RecentlyUsed.Count + 2
        End Get
    End Property



    Public Property SelectedFontFamily() As FontFamily
        Get
            If lstFont.SelectedItem IsNot Nothing Then
                Return DirectCast(lstFont.SelectedItem, Font).FontFamily
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As FontFamily)
            If value Is Nothing Then
                lstFont.ClearSelected()
            Else
                lstFont.SelectedIndex = IndexOf(value)

            End If
        End Set
    End Property

    Public Function IndexOf(ByVal ff As FontFamily) As Integer
        For i As Integer = 1 To lstFont.Items.Count - 1
            Try
                If TypeOf lstFont.Items(i) Is System.String AndAlso lstFont.Items(i) = "Section" Then Continue For
                Dim f As Font = DirectCast(lstFont.Items(i), Font)
                If f.FontFamily.Name = ff.Name Then
                    Return i
                End If
            Catch
            End Try
        Next
        Return -1
    End Function

    Public Sub AddSelectedFontToRecent()
        If lstFont.SelectedIndex < 1 Then
            Return
        End If

        lstFont.SuspendLayout()

        Dim tmpCount As Integer = RecentlyUsed.Count

        RecentlyUsed.Add(DirectCast(lstFont.SelectedItem, Font))

        For i As Integer = 1 To tmpCount
            lstFont.Items.RemoveAt(1)
        Next

        For i As Integer = 0 To RecentlyUsed.Count - 1
            lstFont.Items.Insert(i + 1, RecentlyUsed(i))
        Next

        lstFont.SelectedIndex = 1

        lstFont.ResumeLayout()

    End Sub

    Public Sub AddFontToRecent(ByVal ff As FontFamily)
        lstFont.SuspendLayout()

        For i As Integer = 1 To RecentlyUsed.Count
            lstFont.Items.RemoveAt(1)
        Next

        RecentlyUsed.Add(DirectCast(lstFont.Items(IndexOf(ff)), Font))

        For i As Integer = 0 To RecentlyUsed.Count - 1
            lstFont.Items.Insert(i + 1, RecentlyUsed(i))
        Next

        'lstFont.SelectedIndex = 1;

        lstFont.ResumeLayout()

    End Sub


    Private Sub lstFont_DrawItem(ByVal sender As Object, ByVal e As DrawItemEventArgs) Handles lstFont.DrawItem
        Try
            If e.Index = 0 Then
                e.Graphics.FillRectangle(Brushes.AliceBlue, e.Bounds)
                Dim font As New Font(DefaultFont, FontStyle.Bold Or FontStyle.Italic)
                e.Graphics.DrawString("Recently Used", font, Brushes.Black, e.Bounds.X + 10, e.Bounds.Y + 3, StringFormat.GenericDefault)
            ElseIf e.Index = AllFontsStartIndex - 1 Then
                e.Graphics.FillRectangle(Brushes.AliceBlue, e.Bounds)
                Dim font As New Font(DefaultFont, FontStyle.Bold Or FontStyle.Italic)
                e.Graphics.DrawString("All Fonts", font, Brushes.Black, e.Bounds.X + 10, e.Bounds.Y + 3, StringFormat.GenericDefault)
            ElseIf Not TypeOf lstFont.Items(e.Index) Is System.String Then
                ' Draw the background of the ListBox control for each item.
                e.DrawBackground()

                Dim font As Font = DirectCast(lstFont.Items(e.Index), Font)
                e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds, StringFormat.GenericDefault)

                ' If the ListBox has focus, draw a focus rectangle around the selected item.
                e.DrawFocusRectangle()
            End If
        Catch
        End Try
    End Sub

    Private Sub lstFont_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lstFont.SelectedIndexChanged
        If lstFont.SelectedIndex = RecentlyUsedSectionIndex OrElse lstFont.SelectedIndex = AllFontsSectionIndex Then
            lstFont.SelectedIndex = lastSelectedIndex
        ElseIf lstFont.SelectedItem IsNot Nothing Then
            If Not txtFont.Focused Then
                Dim f As Font = DirectCast(lstFont.SelectedItem, Font)
                txtFont.Text = f.Name
            End If
            RaiseEvent SelectedFontFamilyChanged(lstFont, New EventArgs())
            lastSelectedIndex = lstFont.SelectedIndex
        End If
    End Sub

    Private Sub txtFont_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtFont.TextChanged
        If Not txtFont.Focused Then
            Return
        End If

        For i As Integer = AllFontsStartIndex To lstFont.Items.Count - 1
            Try
                Dim str As String = DirectCast(lstFont.Items(i), Font).Name
                If str.StartsWith(txtFont.Text, True, Nothing) Then
                    lstFont.SelectedIndex = i

                    Const WM_VSCROLL As UInteger = &H115
                    Const SB_THUMBPOSITION As UInteger = 4

                    Dim b As UInteger = (CUInt(lstFont.SelectedIndex) << 16) Or (SB_THUMBPOSITION And &HFFFF)
                    SendMessage(lstFont.Handle, WM_VSCROLL, b, 0)

                    Return
                End If
            Catch
            End Try
        Next
    End Sub

    Private Sub txtFont_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles txtFont.MouseClick
        txtFont.SelectAll()
    End Sub


    <DllImport("user32.dll")> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As UInteger, ByVal lParam As UInteger) As IntPtr
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub lstFont_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        ' if you type alphanumeric characters while focus is on ListBox, it shifts the focus to TextBox.
        If [Char].IsLetterOrDigit(ChrW(e.KeyValue)) Then
            txtFont.Focus()
            txtFont.Text = ChrW(e.KeyValue).ToString()
            txtFont.SelectionStart = 1
        End If


        ' allows to move between sections using arrow keys
        Select Case e.KeyCode
            Case Keys.Left, Keys.Up
                If lstFont.SelectedIndex = AllFontsSectionIndex + 1 Then
                    lstFont.SelectedIndex = lstFont.SelectedIndex - 2
                    e.SuppressKeyPress = True
                End If
                Exit Select
            Case Keys.Down, Keys.Right
                If lstFont.SelectedIndex = AllFontsSectionIndex - 1 Then
                    lstFont.SelectedIndex = lstFont.SelectedIndex + 2
                    e.SuppressKeyPress = True
                End If
                Exit Select

        End Select
    End Sub

    ' ensures that focus is lstFont control whenever the form is loaded
    Private Sub FontList_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Me.ActiveControl = lstFont
    End Sub


End Class

