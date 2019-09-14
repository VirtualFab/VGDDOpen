Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports VGDDCommon
Imports VGDDCommon.Common
Imports System.Xml

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(BarGraph), "BarGraph.ico")> _
    Public Class BarGraph : Inherits VGDDWidget
        Implements ICustomTypeDescriptor
        Implements IVGDDWidget

        Private currentValue As Int16 = 90
        Private previousValue As Int16
        Private newValue As Int16
        Private _Value As Integer
        Private _MinValue As Int16 = 0
        Private _MaxValue As Int16 = 100
        Private _Segments As New SegmentsCollection
        Private _Divisions As Byte = 50
        Private _ScaleNumDivisions As Byte = 10
        Private _Frame As EnabledState
        Private _Orientation As Common.Orientation
        Private _Animated As Boolean = True

        Private _Style As BarGraphStyle = BarGraphStyle.BARGRPHSTYLE_BLOCK
        Private _BarSpeed As Int16

        Private WithEvents AniTimer As New Timer

        Private Shared _Instances As Integer = 0

        Public Enum BarGraphStyle
            BARGRPHSTYLE_SOLID
            BARGRPHSTYLE_BLOCK
            BARGRPHSTYLE_WIREFRAME
        End Enum

        Public Sub New()
            MyBase.New()
            _Instances += 1
            InitializeComponent()
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("BarGraph")
#End If
            Me.Size = New Size(100, 50)
            Me.DoubleBuffered = True
            RemovePropertyToShow("Text")
            _Segments.Add(New Segment(0, 50, Color.Green))
            _Segments.Add(New Segment(51, 80, Color.Yellow))
            _Segments.Add(New Segment(81, 100, Color.Red))
        End Sub

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing And Not Me.IsDisposed Then
                    _Instances -= 1
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
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

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Demolimit As Integer
            Get
                Return Common.DEMOCODELIMIT
            End Get
        End Property

        Dim intBarRectWorH As Int32
        Dim intScaleInterval As Int16
        Dim intScaleWorH As Int32
        Dim intBarInterval As Int32
        Dim intBarValue As Int16
        Dim intBarWorH As Int16
        Const BORDER As Integer = 2

        Private Sub CalcPar()
            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle
            If _ScaleNumDivisions > 0 Then
                intScaleInterval = (_MaxValue - _MinValue) \ _ScaleNumDivisions
            End If

            Select Case _Orientation
                Case Common.Orientation.Horizontal
                    If _ScaleNumDivisions > 0 Then
                        intScaleWorH = (Convert.ToInt32(rc.Right - rc.Left) << 8) \ _ScaleNumDivisions
                    End If
                    intBarRectWorH = rc.Right - rc.Left - (BORDER << 1)
                    If _Style = BarGraphStyle.BARGRPHSTYLE_SOLID Then
                        intBarRectWorH -= (BORDER << 1)
                    End If
                    intBarRectWorH = (intBarRectWorH << 8) / (_Divisions)
                Case Common.Orientation.Vertical
                    If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _ScaleNumDivisions > 0 Then
                        intScaleWorH = (Convert.ToInt32(rc.Bottom - rc.Top - BORDER * 2 - _Scheme.Font.Font.Height) << 8) \ _ScaleNumDivisions
                    End If
                    intBarRectWorH = rc.Bottom - rc.Top - BORDER
                    If _Style = BarGraphStyle.BARGRPHSTYLE_SOLID Then
                        intBarRectWorH -= (BORDER << 1)
                    End If
                    intBarRectWorH = (intBarRectWorH << 8) / (_Divisions)
            End Select
        End Sub

        Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)

            Dim g As Graphics = pevent.Graphics
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If
            'Me.OnPaintBackground(pevent)

            Dim rc As System.Drawing.Rectangle = Me.ClientRectangle

            'Impostazione Region
            Dim Mypath As GraphicsPath = New GraphicsPath
            Mypath.StartFigure()
            Mypath.AddRectangle(rc)
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)
            If _Scheme Is Nothing Then Exit Sub
            Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
            g.FillRegion(brushBackGround, Me.Region)

            Dim ps As Pen = New Pen(_Scheme.Color1)
            ps.Width = 1
            ps.DashStyle = DashStyle.Solid
            If _Frame = EnabledState.Enabled Then
                ps.Color = _Scheme.Color1
                g.DrawRectangle(ps, ps.Width >> 1, ps.Width >> 1, Me.ClientRectangle.Width - ps.Width - 1, Me.ClientRectangle.Height - ps.Width - 1)
            End If

            Dim intTextSize As SizeF = New SizeF(0, 0)
            Dim intXorY As Int16
            If _ScaleNumDivisions > 0 Then
                'Draw Scale Texts
                Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)
                For i As Byte = 0 To _ScaleNumDivisions
                    Dim ScaleText As String = (_MinValue + i * intScaleInterval).ToString
                    intTextSize = g.MeasureString(ScaleText, _Scheme.Font.Font)
                    Select Case _Orientation
                        Case Common.Orientation.Horizontal
                            If i = 0 Then
                                intXorY = BORDER
                            ElseIf i = _ScaleNumDivisions Then
                                intXorY = rc.Right - intTextSize.Width - BORDER
                            Else
                                intXorY = (Convert.ToInt32(intScaleWorH * i) >> 8) + (BORDER << 1) - (intTextSize.Width >> 1)
                            End If
                            g.DrawString(ScaleText, _Scheme.Font.Font, textBrush, intXorY, BORDER)
                        Case Common.Orientation.Vertical
                            intXorY = rc.Bottom - (Convert.ToInt32(intScaleWorH * i) >> 8) - BORDER - intTextSize.Height
                            g.DrawString(ScaleText, _Scheme.Font.Font, textBrush, rc.Right - BORDER - intTextSize.Width, intXorY)
                    End Select
                Next
            End If

            'Draw BarGraph
            intBarInterval = (Convert.ToInt32(_MaxValue - _MinValue) << 8) \ _Divisions
            For i As Byte = 0 To _Divisions - 1
                intBarValue = (i * intBarInterval) >> 8
                Dim BarColor As Color = _Scheme.Color1
                If intBarValue > currentValue Then
                    BarColor = _Scheme.Commonbkcolor
                Else
                    For Each oSegment As Segment In _Segments
                        If intBarValue >= oSegment.ValueFrom Then
                            BarColor = oSegment.SegmentColour
                        End If
                    Next
                End If
                If (previousValue < currentValue And intBarValue >= previousValue) Or _
                    (previousValue > currentValue And intBarValue > currentValue) Then
                    Select Case _Orientation
                        Case Common.Orientation.Horizontal
                            intXorY = ((intBarRectWorH * i) >> 8) + (BORDER << 1)
                            intBarWorH = (intBarRectWorH >> 8) + 1
                            If _Style = BarGraphStyle.BARGRPHSTYLE_BLOCK Or _Style = BarGraphStyle.BARGRPHSTYLE_WIREFRAME Then
                                intBarWorH = intBarWorH >> 1
                            End If
                            If intBarWorH = 0 Then intBarWorH = 1
                            Select Case _Style
                                Case BarGraphStyle.BARGRPHSTYLE_BLOCK, BarGraphStyle.BARGRPHSTYLE_SOLID
                                    Dim BarBrush As SolidBrush = New SolidBrush(BarColor)
                                    g.FillRectangle(BarBrush, intXorY, (BORDER << 1) + intTextSize.Height, intBarWorH, rc.Height - (BORDER << 2) - intTextSize.Height)
                                Case BarGraphStyle.BARGRPHSTYLE_WIREFRAME
                                    Dim BarPen As Pen = New Pen(BarColor)
                                    g.DrawRectangle(BarPen, intXorY, BORDER + intTextSize.Height, intBarWorH, rc.Height - (BORDER << 1) - intTextSize.Height)
                            End Select
                        Case Common.Orientation.Vertical
                            intXorY = rc.Bottom - ((intBarRectWorH * i) >> 8) - (BORDER << 1) - 1
                            intBarWorH = (intBarRectWorH >> 8) + 1
                            If _Style = BarGraphStyle.BARGRPHSTYLE_BLOCK Or _Style = BarGraphStyle.BARGRPHSTYLE_WIREFRAME Then
                                intBarWorH = intBarWorH >> 1
                            Else
                                intXorY -= (BORDER << 1)
                            End If
                            If intBarWorH = 0 Then intBarWorH = 1
                            Select Case _Style
                                Case BarGraphStyle.BARGRPHSTYLE_BLOCK, BarGraphStyle.BARGRPHSTYLE_SOLID
                                    Dim BarBrush As SolidBrush = New SolidBrush(BarColor)
                                    g.FillRectangle(BarBrush, BORDER, intXorY, rc.Width - (BORDER << 1) - intTextSize.Width, intBarWorH)
                                Case BarGraphStyle.BARGRPHSTYLE_WIREFRAME
                                    Dim BarPen As Pen = New Pen(BarColor)
                                    g.DrawRectangle(BarPen, BORDER, intXorY, rc.Width - (BORDER << 1) - intTextSize.Width, intBarWorH)
                            End Select
                    End Select
                End If
            Next
        End Sub

        Private Sub AnimateTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles AniTimer.Tick
            Static AnimationIncrement As Integer = 1
            If Common.AnimationEnable Then
                AniTimer.Enabled = False
                If (AnimationIncrement > 0 And currentValue >= newValue) Or _
                    (AnimationIncrement < 0 And currentValue <= newValue) Then
                    newValue = Rnd(1) * (_MaxValue - _MinValue)
                    AnimationIncrement = newValue * (Rnd(1) + 0.1) + 1
                    If newValue < currentValue Then AnimationIncrement = AnimationIncrement * -1
                Else
                    currentValue += AnimationIncrement
                End If
                Me.Invalidate()
                AniTimer.Enabled = True
            End If
        End Sub

#Region "BarGraphProps"

        <Description("Orientation of the BarGraph")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property Orientation() As Common.Orientation
            Get
                Return _Orientation
            End Get
            Set(ByVal value As Common.Orientation)
                If Not MyBase.IsLoading AndAlso value <> _Orientation Then
                    Dim w As Integer = Me.Width
                    Me.Width = Me.Height
                    Me.Height = w
                End If
                _Orientation = value
                CalcPar()
                Me.Invalidate()
            End Set
        End Property

        '<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
        '<NotifyParentProperty(True)> _
        <Description("Segments to be coloured")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Segments As SegmentsCollection
            Get
                Return _Segments
            End Get
            Set(ByVal value As SegmentsCollection)
                _Segments = value
                Me.Invalidate()
            End Set
        End Property

        <DefaultValue(50)> _
        <Description("Get or Sets the number of blocks to draw.")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(1, 255)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Divisions() As Byte
            Get
                Return _Divisions
            End Get
            Set(ByVal value As Byte)
                If value > 1 Then
                    _Divisions = value
                    CalcPar()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(5)> _
        <Description("Get or Sets the number of values to print in the scale. Set to 0 for no scale")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 255)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property ScaleNumDivisions() As Byte
            Get
                Return _ScaleNumDivisions
            End Get
            Set(ByVal value As Byte)
                If value >= 0 AndAlso value < 100 Then
                    _ScaleNumDivisions = value
                    CalcPar()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <Description("Style for the BarGraph")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(BarGraphStyle.BARGRPHSTYLE_BLOCK)> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Property Style() As BarGraphStyle
            Get
                Return _Style
            End Get
            Set(ByVal value As BarGraphStyle)
                _Style = value
                CalcPar()
                Me.Invalidate()
            End Set
        End Property

        <Description("Should the Player Animate this Widget?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(True)> _
        <CustomSortedCategory("Player", 8)> _
        Public Property Animated() As Boolean
            Get
                Return _Animated
            End Get
            Set(ByVal value As Boolean)
                _Animated = value
                If Not DesignMode Then
                    If _Animated Then
                        AniTimer.Interval = 50
                        AniTimer.Start()
                    Else
                        AniTimer.Stop()
                    End If
                End If
            End Set
        End Property

        <Description("Bar movement speed 0:realtime >8:slow")> _
        <DefaultValue(1)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(1, 128)> _
        <Category("Behavior")> _
        Public Property BarSpeed As Integer
            Get
                Return _BarSpeed
            End Get
            Set(ByVal value As Integer)
                If value >= 0 And value <= 128 Then
                    _BarSpeed = value
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(100)> _
        <Description("Initial value for the BarGraph")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Range", 5)> _
        Public Property Value() As Integer
            Get
                Return currentValue
            End Get
            Set(ByVal value As Integer)
                If value >= _MinValue AndAlso value <= _MaxValue Then
                    newValue = value
                    If DesignMode Then
                        currentValue = value
                    End If
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(0)> _
        <Description("Mininum value on the scale")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 65535)> _
        <CustomSortedCategory("Range", 5)> _
        Public Property MinValue() As Int16
            Get
                Return _MinValue
            End Get
            Set(ByVal value As Int16)
                If value < _MaxValue Then
                    _MinValue = value
                    If currentValue < _MinValue Then
                        currentValue = _MinValue
                    End If
                    CalcPar()
                    Me.Invalidate()
                End If
            End Set
        End Property

        <DefaultValue(100)> _
        <Description("Maximum value on the scale")> _
        <Editor(GetType(UiEditInteger), GetType(System.Drawing.Design.UITypeEditor))> _
        <GOLRange(0, 65535)> _
        <CustomSortedCategory("Range", 5)> _
        Public Property MaxValue() As Int16
            Get
                Return _MaxValue
            End Get
            Set(ByVal value As Int16)
                If value > _MinValue Then
                    _MaxValue = value
                    If currentValue > _MaxValue Then
                        currentValue = _MaxValue
                    End If
                    CalcPar()
                    Me.Invalidate()
                End If
            End Set
        End Property
#End Region

#Region "GDDProps"
 
        <Description("Has the object to be declared public when generating code?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(True)> _
        <CustomSortedCategory("CodeGen", 6)> _
        Public Overloads ReadOnly Property [Public]() As Boolean
            Get
                Return True
            End Get
        End Property

        <Description("Wether to enable a frame around the BarGraph or not")> _
        <DefaultValue(GetType(EnabledState), "Disabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Property Frame() As EnabledState
            Get
                Return _Frame
            End Get
            Set(ByVal value As EnabledState)
                _Frame = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Status of the BarGraph")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(EnabledState), "Enabled")> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property State() As EnabledState
            Get
                Return _State
            End Get
            Set(ByVal value As EnabledState)
                _State = value
                Me.Invalidate()
            End Set
        End Property

#End Region

#Region "VGDDCode"

#If Not PlayerMonolitico Then
        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = String.Empty
            Dim MyCode As String = "", MyState As String = ""

            Dim MyClassName As String = Me.GetType.ToString

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            CodeGen.AddLines(MyCodeHead, CodeGen.CodeHeadTemplate)

            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)
            CodeGen.AddState(MyState, "Orientation", Me.Orientation.ToString)

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            Dim strSegmentsArray As String = ""
            For Each oSegment As Segment In _Segments
                strSegmentsArray &= String.Format(",{{ {0}, {1}, {2} }} ", oSegment.ValueFrom, oSegment.ValueTo, CodeGen.UInt162Hex(CodeGen.Color2Num(oSegment.SegmentColour, False, "BarGraph " & Me.Name)))
            Next
            If strSegmentsArray <> "" Then strSegmentsArray = strSegmentsArray.Substring(1)

            Dim SegmentsCount As Integer = 0
            If _Segments IsNot Nothing Then SegmentsCount = _Segments.Count

            Dim MyParameters(13) As Byte
            MyParameters(0) = MyParameters.Length - 1
            MyParameters(1) = Me.Value \ 256
            MyParameters(2) = Me.Value Mod 256
            MyParameters(3) = _MinValue \ 256
            MyParameters(4) = _MinValue Mod 256
            MyParameters(5) = _MaxValue \ 256
            MyParameters(6) = _MaxValue Mod 256
            MyParameters(7) = _BarSpeed
            MyParameters(8) = _Style
            MyParameters(9) = _Divisions \ 256
            MyParameters(10) = _Divisions Mod 256
            MyParameters(11) = _ScaleNumDivisions \ 256
            MyParameters(12) = _ScaleNumDivisions Mod 256
            Dim cs As Byte = 0
            Dim strMyParameters As String = String.Empty
            For i As Integer = 0 To MyParameters.Length - 1
                If i = MyParameters.Length - 1 Then
                    MyParameters(i) = cs
                Else
                    cs = cs Xor MyParameters(i)
                End If
                strMyParameters &= String.Format(",0x{0:x2}", MyParameters(i))
            Next
            strMyParameters = "(unsigned char []){" & strMyParameters.Substring(1) & "}"

            CodeGen.AddLines(CodeGen.Code, MyCode.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[PARAMETERS]", strMyParameters) _
                .Replace("[SEGMENTSCOUNT]", SegmentsCount) _
                .Replace("[SEGMENTSARRAY]", strSegmentsArray) _
                .Replace("[SCHEME]", Me.Scheme))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[SEGMENTSARRAY]", strSegmentsArray) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.HeadersIncludes.Contains(CodeGen.HeadersIncludesTemplate) Then ' #include "bargraph.h"
                CodeGen.AddLines(CodeGen.HeadersIncludes, CodeGen.HeadersIncludesTemplate)
            End If
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            CodeGen.AddLines(CodeGen.Headers, (CodeGen.TextDeclareHeaderTemplate(_CDeclType) & CodeGen.HeadersTemplate) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, CType(Me, IVGDDWidget))

            Try
                'Dim myAssembly As Reflection.Assembly = System.Reflection.Assembly.GetAssembly(Me.GetType)
                For Each oFolderNode As XmlNode In CodeGen.XmlTemplatesDoc.SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Project/*", MyClassName.Split(".")(1)))
                    MplabX.AddFile(oFolderNode)
                Next
            Catch ex As Exception
            End Try

        End Sub
#End If
#End Region

        'Private Sub BarGraph_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
        '    If Me.Parent IsNot Nothing AndAlso Me.Width = 100 Then
        '        If _Orientation = Common.Orientation.Horizontal Then
        '            Me.Left = 0
        '            Me.Width = Me.Parent.Width
        '        Else
        '            Me.Top = 0
        '            Me.Height = Me.Parent.Height
        '        End If
        '        Me.Invalidate()
        '    End If
        'End Sub

        Private Sub BarGraph_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            CalcPar()
        End Sub

    End Class

End Namespace
