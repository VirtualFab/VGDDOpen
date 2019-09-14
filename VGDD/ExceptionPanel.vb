Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports System.Xml
Imports System.IO

Public Class frmExceptionPanel

    Private InBoundException As Exception
    Private Header As String
    Private Message As String

    Public Sub New(ByVal sender As Exception, ByVal Header As String, ByVal Message As String)
        InitializeComponent()
        Me.InBoundException = sender
        Me.Header = Header
        Me.Message = Message
    End Sub

    Private Sub frmAlert_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim TimeStamp As String = String.Format(Date.Now.ToString, "MM/dd/yyyy hh:mm:ss")

        Dim ExceptionMessage = Me.Header & ": " & InBoundException.Message & vbCrLf & Me.Message
        For Each l As String In Regex.Split(StackTracer.EnhancedStackTrace(InBoundException), "{sep}")
            ExceptionMessage &= vbCrLf & l
        Next
        TextBox1.Text = ExceptionMessage
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Close()
    End Sub

    Private Sub cmdCopyTextToClipboard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCopyTextToClipboard.Click
        Windows.Forms.Clipboard.SetText(TextBox1.Text)
    End Sub
End Class

Public Class StackTracer
    Public Shared Function StackFrameToString(ByVal ApplicationStack As StackFrame) As String

        Dim LineBreak As String = "{sep}"
        Dim sb As New System.Text.StringBuilder
        Dim ParamIterator As Integer

        Dim Discard As Boolean
        Dim mi As Reflection.MemberInfo = ApplicationStack.GetMethod

        With sb
            .Append(mi.DeclaringType.Namespace)
            .Append(".")
            .Append(mi.DeclaringType.Name)
            .Append(".")
            .Append(mi.Name)

            Dim Parameters() As Reflection.ParameterInfo = ApplicationStack.GetMethod.GetParameters()
            Dim Param As Reflection.ParameterInfo

            .Append("(")

            ParamIterator = 0

            For Each Param In Parameters
                ParamIterator += 1
                If ParamIterator > 1 Then .Append(", ")
                .Append(Param.Name)
                .Append(" As ")
                .Append(Param.ParameterType.Name)
            Next

            .Append(")")
            .Append(LineBreak)

            If ApplicationStack.GetFileName Is Nothing OrElse ApplicationStack.GetFileName.Length = 0 Then
                .Append(System.IO.Path.GetFileName(Helper.ParentAssembly.CodeBase))
                .Append(": N ")
                .Append(String.Format("{0:#00000}", ApplicationStack.GetNativeOffset))
                Discard = True
            Else
                .Append(System.IO.Path.GetFileName(ApplicationStack.GetFileName))
                .Append(": line ")
                .Append(String.Format("{0:#0000}", ApplicationStack.GetFileLineNumber))
                .Append(", col ")
                .Append(String.Format("{0:#00}", ApplicationStack.GetFileColumnNumber))
                Discard = False
            End If

            .Append(LineBreak)

        End With

        If Discard Then
            Return ""
        Else
            mDetails = sb.ToString
            Return sb.ToString
        End If

    End Function

    Public Overloads Shared Function EnhancedStackTrace(ByVal ST As StackTrace, Optional ByVal SkipClassName As String = "", Optional ByVal UseReturns As Boolean = False) As String

        Dim sb As New System.Text.StringBuilder
        sb.AppendLine("")

        For Frame As Integer = 0 To ST.FrameCount - 1
            Dim sf As StackFrame = ST.GetFrame(Frame)
            Dim mi As Reflection.MemberInfo = sf.GetMethod
            Dim Results As String

            If SkipClassName <> "" AndAlso mi.DeclaringType.Name.IndexOf(SkipClassName) > -1 Then
            Else
                Results = StackFrameToString(sf)
                If Results.Length > 0 Then
                    sb.Append(Results)
                End If
            End If
        Next

        Return sb.ToString

    End Function

    Public Overloads Shared Function EnhancedStackTrace(ByVal Ex As Exception) As String
        Dim ST As New StackTrace(Ex, True)
        Return EnhancedStackTrace(ST, String.Empty)
    End Function

    Public Overloads Function EnhancedStackTrace() As String
        Dim ST As New StackTrace(True)
        Return EnhancedStackTrace(ST, "ExceptionManager")
    End Function

    Private Shared mDetails As String

    Public Shared ReadOnly Property Details() As String
        Get
            Return mDetails
        End Get
    End Property

End Class

Module Helper
    Private _mParentAssembly As System.Reflection.Assembly = Nothing

    Function ParentAssembly() As System.Reflection.Assembly
        If _mParentAssembly Is Nothing Then
            If System.Reflection.Assembly.GetEntryAssembly Is Nothing Then
                _mParentAssembly = System.Reflection.Assembly.GetCallingAssembly
            Else
                _mParentAssembly = System.Reflection.Assembly.GetEntryAssembly
            End If
        End If
        Return _mParentAssembly
    End Function

    Private Function GetApplicationPath() As String

        Dim Path As String = ""

        If ParentAssembly.CodeBase.StartsWith("http://") Then
            Path = "c:\" + System.Text.RegularExpressions.Regex.Replace(ParentAssembly.CodeBase(), "[\/\\\:\*\?\""\<\>\|]", "_") + "."
        Else
            Dim P As String = System.AppDomain.CurrentDomain.BaseDirectory + System.AppDomain.CurrentDomain.FriendlyName
            If P.Contains(".vshost.") Then
                Path = P.Replace(".vshost.", ".")
            Else
                Path = P
            End If
        End If

        Return IO.Path.GetDirectoryName(Path)
    End Function
End Module

Public Class AssemblyExceptionLoggingURLAttribute
    Inherits System.Attribute

    Private _ExceptionLoggingURL As String
    Public ReadOnly Property AssemblyExceptionLoggingURL() As String
        Get
            Return _ExceptionLoggingURL
        End Get
    End Property

    Public Sub New(ByVal Value As String)
        MyBase.New()
        _ExceptionLoggingURL = Value
    End Sub

    Public Shared Function GetURL(ByVal TheAssembly As Reflection.Assembly) As String
        Dim attr As Attribute
        For Each attr In Attribute.GetCustomAttributes(TheAssembly)
            If TypeOf attr Is AssemblyExceptionLoggingURLAttribute Then
                Return DirectCast(attr, AssemblyExceptionLoggingURLAttribute).AssemblyExceptionLoggingURL
            End If
        Next attr
        Return ""
    End Function
End Class


