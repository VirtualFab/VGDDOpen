Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "PictureIco")> _
    Public Class PutImage : Inherits VGDDWidgetWithBitmap

        'Private _ScaledBitmap As Bitmap
        Private _ZDrawFirst As Boolean = True
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
            'Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            'Me.SetStyle(ControlStyles.ResizeRedraw, True)
            'Me.SetStyle(ControlStyles.Opaque, False)
            'Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, False)
            'Me.Size = New Size(200, 100)
            Me.RemovePropertyToShow("Scheme")
            Me.RemovePropertyToShow("VGDDEvents")
            Me.RemovePropertyToShow("Text")
            Me.RemovePropertyToShow("TextStringID")
        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Instances As Integer
            Get
                Return _Instances
            End Get
        End Property

        'Protected Overrides ReadOnly Property CreateParams() As CreateParams
        '    Get
        '        Dim _Cp As CreateParams = MyBase.CreateParams
        '        _Cp.ExStyle = _Cp.ExStyle Or &H20
        '        Return _Cp
        '    End Get
        'End Property

        'Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
        '    MyBase.OnPaintBackground(pevent)
        'End Sub

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            'Me.OnPaintBackground(pevent)
            MyBase.OnPaint(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle
            'Dim Mypath As GraphicsPath = New GraphicsPath

            'If Me._GraphicsPath IsNot Nothing Then
            '    Mypath.AddPath(Me._GraphicsPath, False)
            'Else
            '    Mypath.AddRectangle(Me.ClientRectangle)
            'End If
            'Me.Region = New Region(Mypath)

            If _VGDDImage Is Nothing Then Exit Sub
            If _VGDDImage.TransparentBitmap IsNot Nothing AndAlso _VGDDImage._GraphicsPath IsNot Nothing Then
                Try
                    Me.Region = New Region(_VGDDImage._GraphicsPath)
                    g.DrawImage(_VGDDImage.TransparentBitmap, 0, 0, MyBase.Width, MyBase.Height)
                Catch ex As Exception
                    '_VGDDImage.ScaleBitmap(Me.Width, Me.Height, Me.Scale)
                End Try
            End If
        End Sub

#Region "GDDProps"

        <Description("Draw this primitive before (set to true) or after (set to false) the Widgets?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Category("Design")> _
        <DefaultValue(True)> _
        Property ZDrawFirst() As Boolean
            Get
                Return _ZDrawFirst
            End Get
            Set(ByVal value As Boolean)
                _ZDrawFirst = value
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = ""
            Dim MyCode As String = "", MyState As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)

            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            Dim MyBitmap As String = "", MyPointerInit As String = ""
            If Me.Bitmap = String.Empty Then
                MyBitmap = "NULL"
                If Common.CodeGenWarnEmptyBitmaps Then
                    MessageBox.Show("Warning: PutImage " & Me.Name & " has empty bitmap. Runtime errors will occur", "Warning")
                End If
                CodeGenWarnEmptyBitmaps = False
            Else
                Dim myBitmapName As String = System.IO.Path.GetFileNameWithoutExtension(Me.Bitmap)
                If Me.BitmapUsePointer Then
                    Dim strPointerName As String = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerName", 0).Trim _
                                                   .Replace("[BITMAPNAME]", myBitmapName)
                    MyBitmap = "(void *)" & strPointerName
                    MyPointerInit = CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/PointerInit", 0).Trim _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName) _
                        .Replace("[BITMAPNAME]", myBitmapName)
                    CodeGen.AddLines(CodeGen.Headers, CodeGen.GetTemplate("VGDDCodeTemplate/ProjectTemplates/BitmapsDeclare/Bitmaps", 0) _
                        .Replace("[BITMAP]", IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName) _
                        .Replace("[BITMAPNAME]", myBitmapName) _
                        .Replace("[POINTERNAME]", strPointerName))
                Else
                    MyBitmap = "(void *)&" & IIf(Common.ProjectUseBmpPrefix, "bmp", "") & myBitmapName
                End If
            End If

            MyCode = MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Me.Left).Replace("[RIGHT]", Me.Right) _
                .Replace("[TOP]", Me.Top).Replace("[BOTTOM]", Me.Bottom) _
                .Replace("[SCALE]", Me.Scale) _
                .Replace("[BITMAP]", MyBitmap) _
                .Replace("[BITMAP_POINTER_INIT]", MyPointerInit) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)
            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, CodeGen.HeadersTemplate.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            If _ZDrawFirst Then
                CodeGen.AddLines(CodeGen.Code, MyCode)
            Else
                CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCode)
            End If

        End Sub
#End If
    End Class

End Namespace
