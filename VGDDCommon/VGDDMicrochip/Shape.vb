Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports System.Data
Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design
Imports VGDDCommon
Imports VGDDCommon.Common
Imports System.Xml

Namespace VGDDMicrochip

    '<Designer(GetType(LineDesigner))> _
    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Button), "ShapeIco")> _
    Public Class Shape : Inherits VGDDBase
        'Implements ISupportInitialize

        Private _ShapePaths As New VGDDShapeDefinition
        Private _Color As Color = Drawing.Color.Blue
        Private _Thickness As Short
        Private _Type As LineType
        Private _ZDrawFirst As Boolean = True

        Public Sub New()
            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.Opaque, False)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, False)
            Me.Thickness = Thickness.NORMAL_LINE
            Me.RemovePropertyToShow("Top")
            Me.RemovePropertyToShow("Bottom")
            Me.RemovePropertyToShow("Left")
            Me.RemovePropertyToShow("Right")
            Me.RemovePropertyToShow("Text")
            Me.RemovePropertyToShow("TextStringID")
        End Sub

        'Protected Overrides Sub SetBoundsCore(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal specified As System.Windows.Forms.BoundsSpecified)
        '    'Debug.Print("SetBoundsCore")
        '    If width <= 10 Then width = _Thickness * 2 + 1 + 1
        '    If height <= 10 Then height = _Thickness * 2 + 1 + 1
        '    MyBase.SetBoundsCore(x, y, width, height, specified)
        'End Sub

        'Protected Overrides ReadOnly Property CreateParams() As CreateParams
        '    Get
        '        Dim _Cp As CreateParams = MyBase.CreateParams
        '        _Cp.ExStyle = _Cp.ExStyle Or &H20
        '        Return _Cp
        '    End Get
        'End Property

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

            Dim brushPen As New SolidBrush(_Color)
            Dim ps As Pen = New Pen(brushPen)
            Select Case _Type
                Case LineType.SOLID_LINE
                    ps.DashStyle = DashStyle.Solid
                Case LineType.DOTTED_LINE
                    ps.DashStyle = DashStyle.Dot
                Case LineType.DASHED_LINE
                    ps.DashStyle = DashStyle.Dash
            End Select
            ps.Width = _Thickness * 2 + 1
            For Each oPoly As VGDDShapeItem In _ShapePaths
                If oPoly.Path IsNot Nothing Then
                    g.DrawPath(ps, oPoly.Path)
                End If
            Next
        End Sub

#Region "GDDProps"
        <Description("Shape Definition")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(UiEditShapeDefinition), GetType(System.Drawing.Design.UITypeEditor))> _
        Public Overloads Property ShapePath() As VGDDShapeDefinition
            Get
                Return _ShapePaths
            End Get
            Set(ByVal value As VGDDShapeDefinition)
                _ShapePaths = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Thickness of the line")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Thickness() As ThickNess
            Get
                Return _Thickness
            End Get
            Set(ByVal value As ThickNess)
                _Thickness = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Type of the line")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property [Type]() As LineType
            Get
                Return _Type
            End Get
            Set(ByVal value As LineType)
                _Type = value
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Color for the Line")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        Public Overloads Property [Color]() As Color
#Else
        Public Overloads Property [Color]() As Color
#End If
            Get
                Return _Color
            End Get
            Set(ByVal value As Color)
                _Color = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Left X coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property X1() As Integer
            Get
                Return Me.Left
            End Get
            Set(ByVal value As Integer)
                Me.Left = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Right X coordinate of the lower-right edge")> _
         <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property X2() As Integer
            Get
                Return Me.Left + Me.Width
            End Get
            Set(ByVal value As Integer)
                If Me.IsLoading Or value >= Me.Left Then
                    Me.Width = value - Me.Left
                Else
                    Me.Width = Me.Left - value
                    Me.Left = value
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Top Y coordinate of the upper-left edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Y1() As Integer
            Get
                Return Me.Top
            End Get
            Set(ByVal value As Integer)
                If Not Me.IsLoading Then
                    Me.Height = Me.Top + Me.Height - value
                End If
                Me.Top = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Bottom Y coordinate of the lower-right edge")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Overloads Property Y2() As Integer
            Get
                Return Me.Top + Me.Height
            End Get
            Set(ByVal value As Integer)
                If Me.IsLoading Or value >= Me.Top Then
                    Me.Height = value - Me.Top
                Else
                    Me.Height = Me.Top - value
                    Me.Top = value
                End If
                Me.Invalidate()
            End Set
        End Property

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

        'Private Sub Line_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        '    Me.Location = MyBase.Location
        'End Sub

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = ""
            Dim MyCode As String = "", MyState As String = ""
            Dim MyCodeUpdate As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            MyCodeUpdate = CodeGen.CodeUpdateTemplate

            MyCode = MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[X1]", Me.X1).Replace("[Y1]", Me.Y1) _
                .Replace("[X2]", Me.X2).Replace("[Y2]", Me.Y2) _
                .Replace("[THICKNESS]", Me.Thickness.ToString) _
                .Replace("[TYPE]", Me.Type.ToString) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Shape " & Me.Name)) _
                .Replace("[COLOR_STRING]", CodeGen.Color2String(Me._Color)) _
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

            CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCodeUpdate.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[X1]", Me.X1).Replace("[Y1]", Me.Y1) _
                .Replace("[X2]", Me.X2).Replace("[Y2]", Me.Y2) _
                .Replace("[THICKNESS]", Me.Thickness.ToString) _
                .Replace("[TYPE]", Me.Type.ToString) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Shape " & Me.Name)) _
                .Replace("[COLOR_STRING]", CodeGen.Color2String(Me._Color)) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            If _ZDrawFirst Then
                CodeGen.AddLines(CodeGen.Code, MyCode)
            Else
                CodeGen.AddLines(CodeGen.ScreenUpdateCode, MyCode)
            End If
        End Sub
#End If

    End Class

    Public Class VGDDShapeItem
        Public Enum VGDDShapeTypes
            Line
            Circle
            PolyLine
        End Enum

        Private _ShapeType As VGDDShapeTypes
        Public Property ShapeType As VGDDShapeTypes
            Get
                Return _ShapeType
            End Get
            Set(ByVal value As VGDDShapeTypes)
                _ShapeType = value
            End Set
        End Property

        Private _Data As Object
        Public Property Data As Object
            Get
                Return _Data
            End Get
            Set(ByVal value As Object)
                _Data = value
            End Set
        End Property

        Private _Color As Color
        Public Property Color As Color
            Get
                Return _Color
            End Get
            Set(ByVal value As Color)
                _Color = value
            End Set
        End Property

        Private _Path As GraphicsPath
        Public Property Path As GraphicsPath
            Get
                Return _Path
            End Get
            Set(ByVal value As GraphicsPath)
                _Path = value
            End Set
        End Property
    End Class

    Public Class VGDDShapeDefinition
        Inherits CollectionBase
        Implements IList

        Public Sub New()

        End Sub

        Public Shared Function FromSVG(ByVal strSVG As String) As VGDDShapeDefinition
            Dim oShape As New VGDDShapeDefinition
            Dim oSVGDoc As New XmlDocument
            oSVGDoc.LoadXml(strSVG)
            Dim oSVGPaths As XmlNodeList = oSVGDoc.DocumentElement.GetElementsByTagName("path")
            For Each oSVGPath As XmlNode In oSVGPaths
                Dim strPath As String = oSVGPath.Attributes("d").Value
                Dim oPoint As Point, oPointsCollection As New Collection
                Dim oLastPoint As Point = New Point(0, 0)
                For Each strPointData As String In strPath.Split(" ")
                    Select Case strPointData
                        Case "m"
                        Case "z"
                            oPointsCollection.Add(oLastPoint)
                        Case Else
                            If strPointData.Contains(",") Then
                                oPoint = New Point(Single.Parse(strPointData.Split(",")(0), Globalization.CultureInfo.InvariantCulture), _
                                                   Single.Parse(strPointData.Split(",")(1), Globalization.CultureInfo.InvariantCulture))
                                oPoint.X += oLastPoint.X
                                oPoint.Y += oLastPoint.Y
                                oPointsCollection.Add(oPoint)
                                oLastPoint = oPoint
                            End If
                    End Select
                Next
                Dim aPoints(oPointsCollection.Count - 1) As Point
                For i As Integer = 0 To oPointsCollection.Count - 1
                    aPoints(i) = oPointsCollection(i + 1)
                Next
                Dim oItem As New VGDDShapeItem
                oItem.Color = Color.Black
                oItem.ShapeType = VGDDShapeItem.VGDDShapeTypes.PolyLine
                oItem.Data = aPoints
                oShape.Add(oItem)
            Next
            Return oShape
        End Function

        Default Public ReadOnly Property Item(ByVal index As Integer) As VGDDShapeItem
            Get
                Return MyBase.List(index)
            End Get
        End Property

        Public Sub Add(ByRef oKey As VGDDShapeItem)
            MyBase.List.Add(oKey)
        End Sub

        Public Overloads ReadOnly Property List()
            Get
                Return MyBase.List
            End Get
        End Property

    End Class


End Namespace
