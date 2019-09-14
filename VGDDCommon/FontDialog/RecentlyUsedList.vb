Imports System.Collections.Generic
Imports System.Text

''' <summary>
''' A custom collection for maintaining recently used lists of any kind. For example, recently used fonts, color etc.
''' List with limited size which is given by MaxSize. As list grows beyond MaxSize, oldest item is removed.
''' New items are added at the top of the list (at index 0), existing items move down.
''' If added item is already there in the list, it is moved to the top (at index 0).
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class RecentlyUsedList(Of T)
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="maxSize">As list grows longer than max size, oldest item will be dropped.</param>
    Public Sub New(ByVal maxSize As Integer)
        Me.m_maxSize = maxSize
    End Sub

    Private list As New List(Of T)()

    Default Public ReadOnly Property Item(ByVal i As Integer) As T
        Get
            Return list(i)
        End Get
    End Property

    Private m_maxSize As Integer

    Public ReadOnly Property MaxSize() As Integer
        Get
            Return m_maxSize
        End Get
    End Property

    Public ReadOnly Property Count() As Integer
        Get
            Return list.Count
        End Get
    End Property

    Public Sub Add(ByVal item As T)
        Dim i As Integer = list.IndexOf(item)
        If i <> -1 Then
            list.RemoveAt(i)
        End If

        If list.Count = MaxSize Then
            list.RemoveAt(list.Count - 1)
        End If

        list.Insert(0, item)
    End Sub

End Class
