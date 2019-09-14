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
    <ToolboxBitmap(GetType(Button), "ArcIco")> _
    Public Class Arc : Inherits VGDDBase

        Private _Radius1 As Integer = 10
        Private _Radius2 As Integer = 20
        Private _Octant As Byte = 255
        Private _Color As Color = Drawing.Color.Blue
        Private _Fill As Boolean = True
        Private _XR, _XL, _YT, _YB As Integer
        Private _ZDrawFirst As Boolean = True

        Public Sub New()
            Me.Height = 100
            Me.Width = 100
            Me._Radius1 = 10
            Me._Radius2 = 20
            Me._Octant = 255
            Me._Color = Drawing.Color.Blue
            Me._Fill = False
            Me.RemovePropertyToShow("Text")
            Me.RemovePropertyToShow("TextStringID")
        End Sub

        'Private WithEvents _compChServ As System.ComponentModel.Design.IComponentChangeService
        'Public Overrides Property Site() As ISite
        '    Get
        '        Return MyBase.Site
        '    End Get
        '    Set(ByVal Value As ISite)
        '        If _compChServ IsNot Nothing Then
        '            RemoveHandler _compChServ.ComponentRename, AddressOf nameChanged
        '        End If
        '        MyBase.Site = Value
        '        _compChServ = CType(GetService(GetType(System.ComponentModel.Design.IComponentChangeService)), System.ComponentModel.Design.IComponentChangeService)
        '        If _compChServ IsNot Nothing Then
        '            AddHandler _compChServ.ComponentRename, AddressOf nameChanged
        '        End If
        '    End Set
        'End Property

        'Private Sub nameChanged(ByVal sender As Object, ByVal e As System.ComponentModel.Design.ComponentRenameEventArgs)
        '    Dim strCleanName As String = Common.CleanName(e.NewName)
        '    If e.NewName <> strCleanName Then
        '        Me.Name = strCleanName
        '    End If
        'End Sub

        Private Function RoundRectanglePath(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal Radius As Integer) As GraphicsPath
            Dim w2 As Integer = w / 2
            Dim h2 As Integer = h / 2


            Dim Mypath As GraphicsPath = New GraphicsPath
            Mypath.StartFigure()
            If Radius > 0 Then
                Mypath.AddArc(x + w - Radius * 3, y - Radius, Radius * 2, Radius * 2, 270, 90)
                Mypath.AddArc(x + w - Radius * 3, y + h - Radius * 3, Radius * 2, Radius * 2, 0, 90)
                Mypath.AddArc(x - Radius, y + h - Radius * 3, Radius * 2, Radius * 2, 90, 90)
                Mypath.AddArc(x - Radius, y - Radius, Radius * 2, Radius * 2, 180, 90)
            Else
                Mypath.AddRectangle(New Drawing.Rectangle(0, 0, w, h))
            End If
            Mypath.CloseFigure()
            Return Mypath

        End Function

        Private Sub RecalcCoords()
            _XL = MyBase.Location.X + _Radius2
            _XR = MyBase.Location.X + Me.Width - _Radius2
            _YT = MyBase.Location.Y + _Radius2
            _YB = MyBase.Location.Y + Me.Height - _Radius2
        End Sub

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            Me.OnPaintBackground(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle
            'Dim Rad2 As Integer = _Radius1

            RecalcCoords()

            'Impostazione Region
            Dim w As Integer = Me.Width '_XR - _XL
            Dim w2 As Integer = CInt(w / 2)
            Dim h As Integer = Me.Height '_YB - _YT
            Dim h2 As Integer = CInt(h / 2)
            Dim rd As Integer = _Radius2 - _Radius1
            Dim rd2 As Integer = rd / 2

            Dim bp As New SolidBrush(_Color)
            Dim ps As Pen = New Pen(bp)

            ps.Width = rd

            Dim p1 As GraphicsPath = RoundRectanglePath(_Radius2, _Radius2, w, h, _Radius2)
            Dim p2 As GraphicsPath = Nothing
            Dim r As New Region(p1)
            If Not _Fill Then
                If _Radius1 = 0 Then _Radius1 = 1
                Dim r2 As Integer = _Radius2
                Do While r2 - _Radius1 < 1
                    r2 += 1
                Loop
                p2 = RoundRectanglePath(r2, r2, _
                                        w - (r2 - _Radius1) * 2, h - (r2 - _Radius1) * 2, _
                                        _Radius1)
                r.Exclude(p2)
            End If
            Me.Region = r

            'Dim Mypath As GraphicsPath = New GraphicsPath
            'Mypath.StartFigure()
            'Mypath.AddRectangle(rc)
            'Mypath.CloseFigure()
            'Me.Region = New Region(mypath)

            'Mypath = New GraphicsPath
            'Mypath.StartFigure()
            'Mypath.AddRectangle(New Drawing.Rectangle(w2 + rd, ps.Width + rd, w2 - _Radius2 - rd, ps.Width - rd))
            'Mypath.AddArc(w - _Radius2 * 2 - ps.Width, ps.Width, _Radius2 * 2, _Radius2 * 2, 270, 47)
            'Mypath.CloseFigure()
            'Me.Region.Exclude(Mypath)


            'Mypath.AddRectangle(New System.Drawing.Rectangle(rc.Left + ps.Width * 3, rc.Top + ps.Width * 3, rc.Width - ps.Width * 6, rc.Height - ps.Width * 6))
            'Mypath.AddRectangle(New Rectangle(rc.Left, rc.Top, rc.Width, ps.Width))
            'Mypath.AddRectangle(New Rectangle(rc.Right - ps.Width, rc.Top + ps.Width, ps.Width, rc.Height - ps.Width * 2))
            'Mypath.AddRectangle(New Rectangle(rc.Left, rc.Bottom - ps.Width, rc.Width, ps.Width))
            'Mypath.AddRectangle(New Rectangle(rc.Left, rc.Top + ps.Width, ps.Width, rc.Height - ps.Width * 2))

            'Dim brushBackGround As New SolidBrush(_Scheme.Color0)
            'g.FillRegion(brushBackGround, Me.Region)
            If _Fill Then
                g.FillRegion(bp, r)
            Else
                If _Octant = 255 Then
                    'g.FillRegion(New Drawing.SolidBrush(Color.FromArgb(220, Drawing.Color.DarkGreen)), New Region(p2))
                    g.FillRegion(bp, r)
                Else
                    If rd2 > 0 Then
                        If (_Octant And 1) Then
                            g.DrawLine(ps, w2, rd2, w - _Radius2 - rd2 + 1, rd2)
                            g.DrawArc(ps, w - _Radius2 * 2 - rd2, rd2, _Radius2 * 2, _Radius2 * 2, 269, 47)
                        End If
                        If (_Octant And 2) Then
                            g.DrawArc(ps, w - _Radius2 * 2 - rd2, rd2, _Radius2 * 2, _Radius2 * 2, 314, 47)
                            g.DrawLine(ps, w - rd2, _Radius2 + rd2 - 1, w - rd2, h2)
                        End If
                        If (_Octant And 4) Then
                            g.DrawLine(ps, w - rd2, h2, w - rd2, h - _Radius2 - rd2 + 1)
                            g.DrawArc(ps, w - _Radius2 * 2 - rd2, h - _Radius2 * 2 - rd2, _Radius2 * 2, _Radius2 * 2, 360, 47)
                        End If
                        If (_Octant And 8) Then
                            g.DrawArc(ps, w - _Radius2 * 2 - rd2, h - _Radius2 * 2 - rd2, _Radius2 * 2, _Radius2 * 2, 44, 48)
                            g.DrawLine(ps, w2, h - rd2, w - _Radius2 - rd2 + 1, h - rd2)
                        End If
                        If (_Octant And 16) Then
                            g.DrawLine(ps, _Radius2 + rd2, h - rd2, w2, h - rd2)
                            g.DrawArc(ps, rd2, h - _Radius2 * 2 - rd2, _Radius2 * 2, _Radius2 * 2, 89, 48)
                        End If
                        If (_Octant And 32) Then
                            g.DrawArc(ps, rd2, h - _Radius2 * 2 - rd2, _Radius2 * 2, _Radius2 * 2, 133, 48)
                            g.DrawLine(ps, rd2, h2, rd2, h - _Radius2 - rd2 + 1)
                        End If
                        If (_Octant And 64) Then
                            g.DrawLine(ps, rd2, _Radius2 + rd2, rd2, h2)
                            g.DrawArc(ps, rd2, rd2, _Radius2 * 2, _Radius2 * 2, 179, 47)
                        End If
                        If (_Octant And 128) Then
                            g.DrawArc(ps, rd2, rd2, _Radius2 * 2, _Radius2 * 2, 223, 47)
                            g.DrawLine(ps, _Radius2 + rd2, rd2, w2, rd2)
                        End If
                    End If
                End If
            End If
        End Sub

#Region "GDDProps"

        <Description("Set this to true to obtain rounded filled Rectangles")> _
        <DefaultValue(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Fill() As Boolean
            Get
                Return _Fill
            End Get
            Set(ByVal value As Boolean)
                _Fill = value
                If _Fill Then
                    MyBase.RemovePropertyToShow("Radius1")
                    MyBase.RemovePropertyToShow("Octant")
                    MyBase.RemovePropertyToShow("Type")
                    Me.SendToBack()
                Else
                    MyBase.AddPropertyToShow("Radius1")
                    MyBase.AddPropertyToShow("Octant")
                    MyBase.AddPropertyToShow("Type")
                    If _Radius2 = 0 Then
                        _Radius2 = _Radius1 + 10
                        RecalcCoords()
                    End If
                End If
                Me.Invalidate()
            End Set
        End Property

        <Description("Bitwise combination for octants to be drawn")> _
        <Browsable(True)> _
        <DefaultValue(255)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Octant() As Short
            Get
                Return _Octant
            End Get
            Set(ByVal value As Short)
                _Octant = value
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Color for the arc/bezel")> _
        <Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <CustomSortedCategory("Appearance", 4)> _
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

        <Description("Right X coordinate of the lower-right edge")> _
         <Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property XR() As Integer
            Get
                Return _XR 'MyBase.Location.X + Me.Width + _Radius2
            End Get
            Set(ByVal value As Integer)
                _XR = value
                If Not Me.IsLoading Then
                    MyBase.Width = _XR - _XL + _Radius2 * 2
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Left X coordinate of the upper-left edge")> _
        <Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property XL() As Integer
            Get
                Return _XL 'MyBase.Location.X + _Radius2
            End Get
            Set(ByVal value As Integer)
                _XL = value
                If Not Me.IsLoading Then
                    'MyBase.Location = New Point(_XL - _Radius2, MyBase.Location.Y)
                    MyBase.Left = _XL - _Radius2
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Top Y coordinate of the upper-left edge")> _
        <Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property YT() As Integer
            Get
                Return _YT 'MyBase.Location.Y + _Radius2
            End Get
            Set(ByVal value As Integer)
                _YT = value
                If Not Me.IsLoading Then
                    'MyBase.Location = New Point(MyBase.Location.X, _YT - _Radius2)
                    MyBase.Top = _YT - _Radius2
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Bottom Y coordinate of the lower-right edge")> _
        <Browsable(True)> _
        <CustomSortedCategory("Size and Position", 3)> _
        Public Property YB() As Integer
            Get
                Return _YB ' MyBase.Location.Y + Me.Height - _Radius2
            End Get
            Set(ByVal value As Integer)
                _YB = value
                If Not Me.IsLoading Then
                    Me.Height = _YB - _YT + _Radius2 * 2
                    RecalcCoords()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Define the inner radius of arc")> _
        <DefaultValue(10)> _
        <CustomSortedCategory("Appearance", 4)> _
        Property Radius1() As Integer
            Get
                Return _Radius1
            End Get
            Set(ByVal value As Integer)
                _Radius1 = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Define the outer radius of arc")> _
        <DefaultValue(20)> _
        <CustomSortedCategory("Appearance", 4)> _
        Property Radius2() As Integer
            Get
                Return _Radius2
            End Get
            Set(ByVal value As Integer)
                _Radius2 = value
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

            MyCode = MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[STATEMENT]", CodeGen.GetStatement("Fill", _Fill.ToString)) _
                .Replace("[XL]", _XL).Replace("[YT]", _YT).Replace("[XR]", _XR).Replace("[YB]", _YB) _
                .Replace("[RADIUS1]", _Radius1) _
                .Replace("[RADIUS2]", _Radius2) _
                .Replace("[OCTANT]", _Octant) _
                .Replace("[COLOR]", CodeGen.Color2Num(Me._Color, False, "Arc " & Me.Name)) _
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

            If _ZDrawFirst Then
                CodeGen.Code &= MyCode
            Else
                CodeGen.ScreenUpdateCode &= MyCode
            End If

        End Sub
#End If

        Private Sub Arc_ClientSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ClientSizeChanged
            RecalcCoords()
        End Sub

        Private Sub Arc_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
            RecalcCoords()
        End Sub

        Private Sub Arc_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
            RecalcCoords()
        End Sub

        Private Sub Arc_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            RecalcCoords()
        End Sub
    End Class


End Namespace