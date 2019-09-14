Imports System
Imports System.Collections

'Namespace ToolboxLibrary

'  Project   : ToolboxLibrary
'  Class     : ToolboxTabCollection
' 
'  Copyright (C) 2002, Microsoft Corporation
' ------------------------------------------------------------------------------
'  <summary>
'  Strongly-typed collection of ToolboxTab objects
'  </summary>
'  <remarks></remarks>
'  <history>
'      [dineshc] 3/26/2003  Created
'  </history>
<Serializable()> _
Public Class ToolboxTabCollection
    Inherits CollectionBase

    '  <summary>
    '       Initializes a new instance of <see cref="ToolboxLibrary.ToolboxTabCollection"/>.
    '  </summary>
    '  <remarks></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub New()
        MyBase.New()

    End Sub

    '  <summary>
    '       Initializes a new instance of <see cref="ToolboxLibrary.ToolboxTabCollection"/> based on another <see cref="ToolboxLibrary.ToolboxTabCollection"/>.
    '  </summary>
    '  <param name="value">
    '       A <see cref="ToolboxLibrary.ToolboxTabCollection"/> from which the contents are copied
    '  </param>
    '  <remarks></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub New(ByVal value As ToolboxTabCollection)
        MyBase.New()
        Me.AddRange(value)
    End Sub

    '  <summary>
    '       Initializes a new instance of <see cref="ToolboxLibrary.ToolboxTabCollection"/> containing any array of <see cref="ToolboxLibrary.ToolboxTab"/> objects.
    '  </summary>
    '  <param name="value">
    '       A array of <see cref="ToolboxLibrary.ToolboxTab"/> objects with which to intialize the collection
    '  </param>
    '  <remarks></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub New(ByVal value() As ToolboxTab)
        MyBase.New()
        Me.AddRange(value)
    End Sub

    '  <summary>
    '  Represents the entry at the specified index of the <see cref="ToolboxLibrary.ToolboxTab"/>.
    '  </summary>
    '  <param name="index">The zero-based index of the entry to locate in the collection.</param>
    '  <value>
    '  The entry at the specified index of the collection.
    '  </value>
    '  <remarks><exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the valid range of indexes for the collection.</exception></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Default Public Property Item(ByVal index As Integer) As ToolboxTab
        Get
            Return CType(List(index), ToolboxTab)
        End Get
        Set(ByVal value As ToolboxTab)
            List(index) = value
        End Set
    End Property

    '  <summary>
    '    Adds a <see cref="ToolboxLibrary.ToolboxTab"/> with the specified value to the 
    '    <see cref="ToolboxLibrary.ToolboxTabCollection"/> .
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.ToolboxTab"/> to add.</param>
    '  <returns>
    '    The index at which the new element was inserted.
    '  </returns>
    '  <remarks><seealso cref="ToolboxLibrary.ToolboxTabCollection.AddRange"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Function Add(ByVal value As ToolboxTab) As Integer
        Return List.Add(value)
    End Function

    '  <summary>
    '  Copies the elements of an array to the end of the <see cref="ToolboxLibrary.ToolboxTabCollection"/>.
    '  </summary>
    '  <param name="value">
    '    An array of type <see cref="ToolboxLibrary.ToolboxTab"/> containing the objects to add to the collection.
    '  </param>
    '  <remarks><seealso cref="ToolboxLibrary.ToolboxTabCollection.Add"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Overloads Sub AddRange(ByVal value() As ToolboxTab)
        Dim i As Integer = 0
        Do While (i < value.Length)
            Me.Add(value(i))
            i = (i + 1)
        Loop
    End Sub

    '  <summary>
    '     
    '       Adds the contents of another <see cref="ToolboxLibrary.ToolboxTabCollection"/> to the end of the collection.
    '    
    '  </summary>
    '  <param name="value">
    '    A <see cref="ToolboxLibrary.ToolboxTabCollection"/> containing the objects to add to the collection.
    '  </param>
    '  <remarks><seealso cref="ToolboxLibrary.ToolboxTabCollection.Add"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Overloads Sub AddRange(ByVal value As ToolboxTabCollection)
        Dim i As Integer = 0
        Do While (i < value.Count)
            Me.Add(value(i))
            i = (i + 1)
        Loop
    End Sub

    '  <summary>
    '  Gets a value indicating whether the 
    '    <see cref="ToolboxLibrary.ToolboxTabCollection"/> contains the specified <see cref="ToolboxLibrary.ToolboxTab"/>.
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.ToolboxTab"/> to locate.</param>
    '  <returns>
    '  <see langword="true"/> if the <see cref="ToolboxLibrary.ToolboxTab"/> is contained in the collection; 
    '   otherwise, <see langword="false"/>.
    '  </returns>
    '  <remarks><seealso cref="ToolboxLibrary.ToolboxTabCollection.IndexOf"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Function Contains(ByVal value As ToolboxTab) As Boolean
        Return List.Contains(value)
    End Function

    '  <summary>
    '  Copies the <see cref="ToolboxLibrary.ToolboxTabCollection"/> values to a one-dimensional <see cref="System.Array"/> instance at the 
    '    specified index.
    '  </summary>
    '  <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the values copied from <see cref="ToolboxLibrary.ToolboxTabCollection"/> .</param>
    '  <param name="index">The index in <paramref name="array"/> where copying begins.</param>
    '  <remarks><exception cref="System.ArgumentException"><paramref name="array"/> is multidimensional. <para>-or-</para> <para>The number of elements in the <see cref="ToolboxLibrary.ToolboxTabCollection"/> is greater than the available space between <paramref name="arrayIndex"/> and the end of <paramref name="array"/>.</para></exception>
    '  <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>. </exception>
    '  <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than <paramref name="array"/>"s lowbound. </exception>
    '  <seealso cref="System.Array"/>
    '  </remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub CopyTo(ByVal array() As ToolboxTab, ByVal index As Integer)
        List.CopyTo(array, index)
    End Sub

    '  <summary>
    '    Returns the index of a <see cref="ToolboxLibrary.ToolboxTab"/> in 
    '       the <see cref="ToolboxLibrary.ToolboxTabCollection"/> .
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.ToolboxTab"/> to locate.</param>
    '  <returns>
    '  The index of the <see cref="ToolboxLibrary.ToolboxTab"/> of <paramref name="value"/> in the 
    '  <see cref="ToolboxLibrary.ToolboxTabCollection"/>, if found; otherwise, -1.
    '  </returns>
    '  <remarks><seealso cref="ToolboxLibrary.ToolboxTabCollection.Contains"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Function IndexOf(ByVal value As ToolboxTab) As Integer
        Return List.IndexOf(value)
    End Function

    '  <summary>
    '  Inserts a <see cref="ToolboxLibrary.ToolboxTab"/> into the <see cref="ToolboxLibrary.ToolboxTabCollection"/> at the specified index.
    '  </summary>
    '  <param name="index">The zero-based index where <paramref name="value"/> should be inserted.</param>
    '  <param name=" value">The <see cref="ToolboxLibrary.ToolboxTab"/> to insert.</param>
    '  <remarks><seealso cref="ToolboxLibrary.ToolboxTabCollection.Add"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub Insert(ByVal index As Integer, ByVal value As ToolboxTab)
        List.Insert(index, value)
    End Sub

    '  <summary>
    '    Returns an enumerator that can iterate through 
    '       the <see cref="ToolboxLibrary.ToolboxTabCollection"/> .
    '  </summary>
    '  <returns>An enumerator for the collection</returns>
    '  <remarks><seealso cref="System.Collections.IEnumerator"/></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Shadows Function GetEnumerator() As ToolboxTabEnumerator
        Return New ToolboxTabEnumerator(Me)
    End Function

    '  <summary>
    '     Removes a specific <see cref="ToolboxLibrary.ToolboxTab"/> from the 
    '    <see cref="ToolboxLibrary.ToolboxTabCollection"/> .
    '  </summary>
    '  <param name="value">The <see cref="ToolboxLibrary.ToolboxTab"/> to remove from the <see cref="ToolboxLibrary.ToolboxTabCollection"/> .</param>
    '  <remarks><exception cref="System.ArgumentException"><paramref name="value"/> is not found in the Collection. </exception></remarks>
    '  <history>
    '      [dineshc] 3/26/2003  Created
    '  </history>
    Public Sub Remove(ByVal value As ToolboxTab)
        List.Remove(value)
    End Sub

    Public Class ToolboxTabEnumerator
        Inherits Object
        Implements IEnumerator

        Private baseEnumerator As IEnumerator

        Private temp As IEnumerable

        Public Sub New(ByVal mappings As ToolboxTabCollection)
            MyBase.New()
            Me.temp = CType(mappings, IEnumerable)
            Me.baseEnumerator = temp.GetEnumerator
        End Sub

        Public ReadOnly Property Current() As ToolboxTab
            Get
                Return CType(baseEnumerator.Current, ToolboxTab)
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