Imports System
Imports System.Collections

'Namespace ToolboxLibrary

'  Project   : ToolboxLibrary
'  Class     : ToolboxItemCollection
' 
'  Copyright (C) 2002, Microsoft Corporation
' ------------------------------------------------------------------------------
'  <summary>
'  Strongly-typed collection of ToolboxItem objects
'  </summary>
'  <remarks></remarks>
'  <history>
'      [dineshc] 3/26/2003  Created
'  </history>
<Serializable()> _
Public Class ToolboxItemCollection
    Inherits CollectionBase

    '  <summary>
    '       Initializes a new instance of <see cref="ToolboxLibrary.VgddToolboxItemCollection"/>.
    '  </summary>
    '  <remarks></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub New()
        MyBase.New()

    End Sub

    '  <summary>
    '       Initializes a new instance of <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> based on another <see cref="ToolboxLibrary.VgddToolboxItemCollection"/>.
    '  </summary>
    '  <param name="value">
    '       A <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> from which the contents are copied
    '  </param>
    '  <remarks></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub New(ByVal value As ToolboxItemCollection)
        MyBase.New()
        Me.AddRange(value)
    End Sub

    '  <summary>
    '       Initializes a new instance of <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> containing any array of <see cref="ToolboxLibrary.VgddToolboxItem"/> objects.
    '  </summary>
    '  <param name="value">
    '       A array of <see cref="ToolboxLibrary.VgddToolboxItem"/> objects with which to intialize the collection
    '  </param>
    '  <remarks></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub New(ByVal value() As VgddToolboxItem)
        MyBase.New()
        Me.AddRange(value)
    End Sub

    '  <summary>
    '  Represents the entry at the specified index of the <see cref="ToolboxLibrary.VgddToolboxItem"/>.
    '  </summary>
    '  <param name="index">The zero-based index of the entry to locate in the collection.</param>
    '  <value>
    '  The entry at the specified index of the collection.
    '  </value>
    '  <remarks><exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the valid range of indexes for the collection.</exception></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Default Public Property Item(ByVal index As Integer) As VgddToolboxItem
        Get
            Return CType(List(index), VgddToolboxItem)
        End Get
        Set(ByVal value As VgddToolboxItem)
            List(index) = value
        End Set
    End Property

    '  <summary>
    '    Adds a <see cref="ToolboxLibrary.VgddToolboxItem"/> with the specified value to the 
    '    <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> .
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.VgddToolboxItem"/> to add.</param>
    '  <returns>
    '    The index at which the new element was inserted.
    '  </returns>
    '  <remarks><seealso cref="ToolboxLibrary.VgddToolboxItemCollection.AddRange"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Function Add(ByVal value As VgddToolboxItem) As Integer
        Return List.Add(value)
    End Function

    '  <summary>
    '  Copies the elements of an array to the end of the <see cref="ToolboxLibrary.VgddToolboxItemCollection"/>.
    '  </summary>
    '  <param name="value">
    '    An array of type <see cref="ToolboxLibrary.VgddToolboxItem"/> containing the objects to add to the collection.
    '  </param>
    '  <remarks><seealso cref="ToolboxLibrary.VgddToolboxItemCollection.Add"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Overloads Sub AddRange(ByVal value() As VgddToolboxItem)
        Dim i As Integer = 0
        Do While (i < value.Length)
            Me.Add(value(i))
            i = (i + 1)
        Loop
    End Sub

    '  <summary>
    '     
    '       Adds the contents of another <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> to the end of the collection.
    '    
    '  </summary>
    '  <param name="value">
    '    A <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> containing the objects to add to the collection.
    '  </param>
    '  <remarks><seealso cref="ToolboxLibrary.VgddToolboxItemCollection.Add"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Overloads Sub AddRange(ByVal value As ToolboxItemCollection)
        Dim i As Integer = 0
        Do While (i < value.Count)
            Me.Add(value(i))
            i = (i + 1)
        Loop
    End Sub

    '  <summary>
    '  Gets a value indicating whether the 
    '    <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> contains the specified <see cref="ToolboxLibrary.VgddToolboxItem"/>.
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.VgddToolboxItem"/> to locate.</param>
    '  <returns>
    '  <see langword="true"/> if the <see cref="ToolboxLibrary.VgddToolboxItem"/> is contained in the collection; 
    '   otherwise, <see langword="false"/>.
    '  </returns>
    '  <remarks><seealso cref="ToolboxLibrary.VgddToolboxItemCollection.IndexOf"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Function Contains(ByVal value As VgddToolboxItem) As Boolean
        Return List.Contains(value)
    End Function

    '  <summary>
    '  Copies the <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> values to a one-dimensional <see cref="System.Array"/> instance at the 
    '    specified index.
    '  </summary>
    '  <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the values copied from <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> .</param>
    '  <param name="index">The index in <paramref name="array"/> where copying begins.</param>
    '  <remarks><exception cref="System.ArgumentException"><paramref name="array"/> is multidimensional. <para>-or-</para> <para>The number of elements in the <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> is greater than the available space between <paramref name="arrayIndex"/> and the end of <paramref name="array"/>.</para></exception>
    '  <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>. </exception>
    '  <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than <paramref name="array"/>"s lowbound. </exception>
    '  <seealso cref="System.Array"/>
    '  </remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub CopyTo(ByVal array() As VgddToolboxItem, ByVal index As Integer)
        List.CopyTo(array, index)
    End Sub

    '  <summary>
    '    Returns the index of a <see cref="ToolboxLibrary.VgddToolboxItem"/> in 
    '       the <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> .
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.VgddToolboxItem"/> to locate.</param>
    '  <returns>
    '  The index of the <see cref="ToolboxLibrary.VgddToolboxItem"/> of <paramref name="value"/> in the 
    '  <see cref="ToolboxLibrary.VgddToolboxItemCollection"/>, if found; otherwise, -1.
    '  </returns>
    '  <remarks><seealso cref="ToolboxLibrary.VgddToolboxItemCollection.Contains"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Function IndexOf(ByVal value As VgddToolboxItem) As Integer
        Return List.IndexOf(value)
    End Function

    '  <summary>
    '  Inserts a <see cref="ToolboxLibrary.VgddToolboxItem"/> into the <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> at the specified index.
    '  </summary>
    '  <param name="index">The zero-based index where <paramref name="value"/> should be inserted.</param>
    '  <param name=" value">The <see cref="ToolboxLibrary.VgddToolboxItem"/> to insert.</param>
    '  <remarks><seealso cref="ToolboxLibrary.VgddToolboxItemCollection.Add"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub Insert(ByVal index As Integer, ByVal value As VgddToolboxItem)
        List.Insert(index, value)
    End Sub

    '  <summary>
    '    Returns an enumerator that can iterate through 
    '       the <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> .
    '  </summary>
    '  <returns>An enumerator for the collection</returns>
    '  <remarks><seealso cref="System.Collections.IEnumerator"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Shadows Function GetEnumerator() As ToolboxItemEnumerator
        Return New ToolboxItemEnumerator(Me)
    End Function

    '  <summary>
    '     Removes a specific <see cref="ToolboxLibrary.VgddToolboxItem"/> from the 
    '    <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> .
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.VgddToolboxItem"/> to remove from the <see cref="ToolboxLibrary.VgddToolboxItemCollection"/> .</param>
    '  <remarks><exception cref="System.ArgumentException"><paramref name="value"/> is not found in the Collection. </exception></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub Remove(ByVal value As VgddToolboxItem)
        List.Remove(value)
    End Sub

    Public Class ToolboxItemEnumerator
        Inherits Object
        Implements IEnumerator

        Private baseEnumerator As IEnumerator

        Private temp As IEnumerable

        Public Sub New(ByVal mappings As ToolboxItemCollection)
            MyBase.New()
            Me.temp = CType(mappings, IEnumerable)
            Me.baseEnumerator = temp.GetEnumerator
        End Sub

        Public ReadOnly Property Current() As VgddToolboxItem
            Get
                Return CType(baseEnumerator.Current, VgddToolboxItem)
            End Get
        End Property

        ReadOnly Property IEnumerator_Current() As Object Implements IEnumerator.Current
            Get
                Return baseEnumerator.Current
            End Get
        End Property

        Public Function MoveNext() As Boolean
            Return baseEnumerator.MoveNext
        End Function

        Function IEnumerator_MoveNext() As Boolean Implements IEnumerator.MoveNext
            Return baseEnumerator.MoveNext
        End Function

        Public Sub Reset()
            baseEnumerator.Reset()
        End Sub

        Sub IEnumerator_Reset() Implements IEnumerator.Reset
            baseEnumerator.Reset()
        End Sub
    End Class
End Class
'End Namespace