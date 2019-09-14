Imports System

Public Class VgddToolboxItem
    Inherits Drawing.Design.ToolboxItem

    Private m_name As String = Nothing
    Private m_type As Type = Nothing
    Private _type As System.Type
    'Private _DisplayName As String

    Public Sub New()
        MyBase.New()
    End Sub

    Sub New(ByVal type As System.Type)
        MyBase.New(type)
        m_type = type
    End Sub

    Public Property Name() As String
        Get
            Return m_name
        End Get
        Set(ByVal value As String)
            m_name = value
        End Set
    End Property

    'Public Overloads Property DisplayName() As String
    '    Get
    '        Return _DisplayName
    '    End Get
    '    Set(ByVal value As String)
    '        _DisplayName = value
    '    End Set
    'End Property

    Public Property Type() As Type
        Get
            Return m_type
        End Get
        Set(ByVal value As Type)
            m_type = value
        End Set
    End Property

    'Protected Overrides Function CreateComponentsCore(ByVal host As System.ComponentModel.Design.IDesignerHost) As System.ComponentModel.IComponent()
    '    Return MyBase.CreateComponentsCore(host)
    'End Function

    'Public Shadows Function CreateComponents() As System.ComponentModel.IComponent()
    '    Return MyBase.CreateComponents()
    'End Function

    'Public Shadows Function CreateComponents(ByVal host As System.ComponentModel.Design.IDesignerHost) As System.ComponentModel.IComponent()
    '    Return MyBase.CreateComponents(host)
    'End Function

    'Public Shadows Function CreateComponents(ByVal host As System.ComponentModel.Design.IDesignerHost, ByVal defaultValues As System.Collections.IDictionary) As System.ComponentModel.IComponent()
    '    Return MyBase.CreateComponents(host, defaultValues)
    'End Function
End Class
