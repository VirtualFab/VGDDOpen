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
    <ToolboxBitmap(GetType(Button), "MeterIco")> _
    Public Class Meter : Inherits VGDDWidget
        Private _MaxValue As Short = 100
        Private _MinValue As Short = 1
        Private _TitleFont As VGDDFont
        Private _ValueFont As VGDDFont
        Private _Resolution As Short = 1
        Private _Value As Short = 1
        Private _MeterType As MeterType
        Private _MeterTypeMLA4 As MeterTypeMLA4

        Private m_minDeg As Integer = 50
        Private m_maxDeg As Integer = 310
        Private degRange As Single = m_maxDeg - m_minDeg
        Private m_valueRange As Single = degRange
        Private m_tickWarningColor As Color = Color.Red
        Private m_tickColor As Color
        Private m_tickIncrement As Integer = 10
        Private degPercent As Single = 1 / (m_valueRange / degRange)
        Private m_useCustNumSize As Boolean
        Private m_custNumSize As Single

        Private _Animated As Boolean = False
        Private _AnimationDirection As Integer = 1
        Private WithEvents AniTimer As New Timer
        Friend Shared _Instances As Integer = 0

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Meter")
            Select Case VGDDCommon.Mal.FrameworkName.ToUpper
                Case "MLALEGACY"
                    _MeterType = Common.MeterType.Ring
                    Me.RemovePropertyToShow("Meter_Type")
                Case "MLA", "HARMONY"
                    Me.RemovePropertyToShow("MeterType")
            End Select
#End If
            Me.Size = New Size(200, 200)
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

        Protected Overrides Sub OnPaint(ByVal pe As PaintEventArgs)
            Dim aMeterColors As Color() = {Color.Green, Color.YellowGreen, Color.Cyan, Color.Yellow, Color.Orange, Color.Red}
            Dim displayGraphics As Graphics = pe.Graphics
            Dim img As Image = New Bitmap(ClientRectangle.Width, ClientRectangle.Height)
            Dim g As Graphics = Graphics.FromImage(img)
            If MyBase.Top < 0 Then
                MyBase.Top = 0
            End If
            If MyBase.Left < 0 Then
                MyBase.Left = 0
            End If

            If _Scheme Is Nothing Then Exit Sub
            If _TitleFont Is Nothing Then _TitleFont = _Scheme.Font
            If _ValueFont Is Nothing Then _ValueFont = _Scheme.Font

            Dim szTextSize As SizeF = g.MeasureString(MyBase.Text, _TitleFont.Font)
            Const MULT As Integer = 3
            Dim rcMeter As New System.Drawing.Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Height - szTextSize.Height * MULT, ClientRectangle.Height - szTextSize.Height * MULT)
            Dim MeterOffsetX As Integer = (ClientRectangle.Width - rcMeter.Width) / 2
            Dim rcMeterOffset As New System.Drawing.Rectangle(ClientRectangle.X + MeterOffsetX, ClientRectangle.Y + MeterOffsetX, ClientRectangle.Height - szTextSize.Height * MULT, ClientRectangle.Height - szTextSize.Height * MULT)

            rcMeter.Location = New Point(MeterOffsetX, MeterOffsetX)

            'outer Ellipse
            Dim meterPath As New GraphicsPath
            'meterPath.AddEllipse(rcMeterOffset)
            'meterPath.AddRectangle(New System.Drawing.Rectangle((ClientRectangle.Width - szTextSize.Width) / 2, ClientRectangle.Height - szTextSize.Height, szTextSize.Width, szTextSize.Height))
            meterPath.AddRectangle(ClientRectangle)
            Me.Region = New System.Drawing.Region(meterPath)

            'fill path
            'Using meterBrush As Brush = New SolidBrush(_Scheme.Color0)
            '    g.FillRegion(meterBrush, Me.Region)
            'End Using

            MyBase.FillBackground(g, _Scheme.Color0, Me.Region)

            m_tickColor = _Scheme.Textcolor0

            'Rotation of tick marks
            Dim drawTo As Integer
            Dim halfWidth As Integer = rcMeter.Width \ 2
            Dim halfHeight As Integer = rcMeter.Height \ 2

            'make sure tick marks are drawn long enough
            If rcMeter.Width > rcMeter.Height Then
                drawTo = rcMeter.Width + MeterOffsetX
            Else
                drawTo = rcMeter.Height + MeterOffsetX
            End If

            If Me.MeterType = MeterType.Ring Then
                Dim aAngles() As Integer = {135, 180, 225, 270, 315, 360}
                For j As Integer = 0 To 5
                    Try
                        g.DrawArc(New Pen(aMeterColors(j), rcMeter.Width * 0.2), MeterOffsetX, 0 + MeterOffsetX, rcMeter.Width, rcMeter.Height, aAngles(j), 45)
                    Catch ex As Exception
                    End Try
                Next
            End If

            Dim i As Single = m_minDeg
            Dim matrix As Matrix
            While i < m_maxDeg
                matrix = New Matrix
                matrix.RotateAt(i, New PointF(halfWidth + MeterOffsetX, halfHeight + MeterOffsetX))
                g.Transform = matrix

                Dim intQuadrant As Integer = Math.Truncate((i - m_minDeg) / (m_maxDeg - m_minDeg) * 6)
                Select Case Me.MeterType
                    Case MeterType.Normal
                        g.DrawLine(New Pen(aMeterColors(intQuadrant), 2), halfWidth + MeterOffsetX, halfHeight + MeterOffsetX, halfWidth + MeterOffsetX, drawTo)
                    Case MeterType.Ring
                        g.DrawLine(New Pen(Color.Black, 2), halfWidth + MeterOffsetX, halfHeight + MeterOffsetX, halfWidth + MeterOffsetX, drawTo)
                End Select
                i += m_tickIncrement * degPercent
            End While

            'makes sure the last tick is marked
            Dim lastTickMtx As New Matrix()
            lastTickMtx.RotateAt(m_maxDeg, New PointF(halfWidth + MeterOffsetX, halfHeight + MeterOffsetX))
            g.Transform = lastTickMtx
            g.DrawLine(New Pen(m_tickWarningColor, 2), halfWidth + MeterOffsetX, halfHeight + MeterOffsetX, halfWidth + MeterOffsetX, drawTo)


            Try
                'inner ellipse   
                Dim rectWidth As Integer = rcMeter.Width * 0.8
                Dim rectHeight As Integer = rcMeter.Height * 0.8
                Dim upperX As Integer = (rcMeter.Width - rectWidth) / 2
                Dim upperY As Integer = (rcMeter.Height - rectHeight) / 2
                Dim innerRect As New System.Drawing.Rectangle(upperX + MeterOffsetX, upperY + MeterOffsetX, rectWidth, rectHeight)
                Using faceBrush As Brush = New SolidBrush(_Scheme.Color0)
                    'get g oriented correctly
                    Dim inEllipseMtx As New Matrix()
                    inEllipseMtx.RotateAt(0, New PointF(halfWidth + MeterOffsetX, halfHeight + MeterOffsetX))
                    g.Transform = inEllipseMtx
                    'fill ellipse
                    g.FillEllipse(faceBrush, innerRect)
                End Using

                'PaintNumber
                Dim numBrush As Brush = New SolidBrush(_Scheme.Textcolor0)
                Dim sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                'draw Number Value 
                g.DrawString(Me.Value.ToString(), Me._ValueFont.Font, numBrush, New System.Drawing.Rectangle(Me.ClientRectangle.Width * 30 \ 100, Me.ClientRectangle.Height - szTextSize.Height * 2 - Me._ValueFont.Font.Size * 1.2, Me.ClientRectangle.Width * 40 \ 100, Me.ClientRectangle.Height - szTextSize.Height), sf)

                'draw needle
                Dim needleGraph As Graphics = Graphics.FromImage(img)

                'Clip outer needle area
                upperX = CInt(Math.Truncate(rcMeter.Width * 0.1))
                upperY = CInt(Math.Truncate(rcMeter.Height * 0.1))
                rectWidth = CInt(Math.Truncate(rcMeter.Width * 0.8))
                rectHeight = CInt(Math.Truncate(rcMeter.Height * 0.8))
                Dim needleRect As New System.Drawing.Rectangle(upperX + MeterOffsetX, upperY + MeterOffsetX, rectWidth, rectHeight)
                Using needlePath As New GraphicsPath()
                    needlePath.AddEllipse(needleRect)
                    Using needleRegion As New Region(needlePath)
                        needleGraph.DrawPath(Pens.Transparent, needlePath)
                        needleGraph.Clip = needleRegion
                    End Using
                End Using

                matrix = New Matrix
                matrix.RotateAt((Me.Value * degPercent) + m_minDeg, New PointF(halfWidth + MeterOffsetX, halfHeight + MeterOffsetX))
                needleGraph.Transform = matrix
                needleGraph.DrawLine(New Pen(Color.Red, 3), halfWidth + MeterOffsetX, halfHeight + MeterOffsetX, halfWidth, drawTo)

                'clean up
                needleGraph.Dispose()

                'Draw Text
                Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)
                g.DrawString(Text, _TitleFont.Font, textBrush, (Width - szTextSize.Width) / 2, Height - szTextSize.Height * 2)

            Catch ex As Exception

            End Try

            MyBase.OnPaint(pe)
            g.Dispose()
            displayGraphics.DrawImage(img, ClientRectangle)
            img.Dispose()
        End Sub

#Region "GDDProps"

        <Description("Type of the Meter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Overloads Property MeterType() As MeterType
            Get
                Return _MeterType
            End Get
            Set(ByVal value As MeterType)
                _MeterType = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Type of the Meter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Main", 2)> _
        Public Overloads Property Meter_Type() As MeterTypeMLA4
            Get
                Return _MeterTypeMLA4
            End Get
            Set(ByVal value As MeterTypeMLA4)
                _MeterTypeMLA4 = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Should Player Animate this Widget?")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Player", 8)> _
        Public Property Animated() As Boolean
            Get
                Return _Animated
            End Get
            Set(ByVal value As Boolean)
                _Animated = value
                If Not DesignMode Then
                    If _Animated Then
                        AniTimer.Interval = 200
                        AniTimer.Start()
                    Else
                        AniTimer.Stop()
                    End If
                End If
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Title Font of the Meter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDFontNameChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property TitleFont() As String
#Else
        Public Overloads Property TitleFont() As String
#End If
            Get
                If _TitleFont IsNot Nothing Then
                    Return _TitleFont.Name
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                _TitleFont = GetFont(value, Me)
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Value Font of the Meter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(VGDDFontNameChooser), GetType(System.Drawing.Design.UITypeEditor))> _
        <CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property ValueFont() As String
#Else
        Public Overloads Property ValueFont() As String

#End If
            Get
                If _ValueFont IsNot Nothing Then
                    Return _ValueFont.Name
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                _ValueFont = GetFont(value, Me)
                Me.Invalidate()
            End Set
        End Property

        <Description("Current value for the Meter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Overloads Property Value() As Short
            Get
                Return _Value
            End Get
            Set(ByVal value As Short)
                _Value = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Minimum value for the Meter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Overloads Property MinValue() As Short
            Get
                Return _MinValue
            End Get
            Set(ByVal value As Short)
                _MinValue = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Maximum value for the Meter")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <CustomSortedCategory("Range", 5)> _
        Public Overloads Property MaxValue() As Short
            Get
                Return _MaxValue
            End Get
            Set(ByVal value As Short)
                _MaxValue = value
                Me.Invalidate()
            End Set
        End Property

#End Region

#If Not PlayerMonolitico Then

        Public Overrides Sub GetCode(ByVal ControlIdPrefix As String)
            Dim MyControlId As String = ControlIdPrefix & "_" & Me.Name
            Dim MyControlIdNoIndex As String = ControlIdPrefix & "_" & Me.Name.Split("[")(0)
            Dim MyControlIdIndex As String = "", MyControlIdIndexPar As String = ""
            Dim MyCodeHead As String = CodeGen.MyCodeHead(_CDeclType)
            Dim MyCode As String = "", MyState As String = ""

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            If _public Then
                CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            Else
                CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            End If
            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", IIf(_State = EnabledState.Enabled, True, False))
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "MeterType", Me.MeterType.ToString)

            If Me.Text = String.Empty Then
                MyCodeHead = ""
            End If

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            If Common.ProjectMultiLanguageTranslations > 0 AndAlso MyBase.TextStringID < 0 Then
                CodeGen.Errors &= MyControlId & " has empty text ID" & vbCrLf
            End If

            CodeGen.AddLines(CodeGen.Code, MyCode _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[MINVALUE]", Me.MinValue) _
                .Replace("[MAXVALUE]", Me.MaxValue) _
                .Replace("[TITLEFONT]", Me._TitleFont.Name) _
                .Replace("[VALUEFONT]", Me._ValueFont.Name) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[TYPE]", _MeterTypeMLA4.ToString) _
                .Replace("[VALUE]", Me.Value) _
                .Replace("[SCHEME]", Me.Scheme) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar))

            MyCodeHead = MyCodeHead.Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext)
            If Not CodeGen.CodeHead.Contains(MyCodeHead) Then
                CodeGen.AddLines(CodeGen.CodeHead, MyCodeHead)
            End If

            Dim MyHeaders As String = String.Empty
            If Me.Public Then
                CodeGen.AddLines(MyHeaders, "extern " & CodeGen.ConstructorTemplate.Trim)
            End If
            If Me.Text <> String.Empty Then
                CodeGen.AddLines(MyHeaders, CodeGen.MyHeader(_CDeclType))
            End If
            CodeGen.AddLines(MyHeaders, CodeGen.TextDeclareHeaderTemplate(_CDeclType))
            CodeGen.AddLines(MyHeaders, CodeGen.HeadersTemplate)
            CodeGen.AddLines(CodeGen.Headers, MyHeaders _
                .Replace("[STRINGID]", CodeGen.StringPoolIndex(MyBase.TextStringID)) _
                .Replace("[CONTROLID]", MyControlId) _
                .Replace("[CONTROLID_NOINDEX]", MyControlIdNoIndex) _
                .Replace("[CONTROLID_INDEX]", MyControlIdIndex) _
                .Replace("[CONTROLID_INDEXPAR]", MyControlIdIndexPar) _
                .Replace("[TEXT]", myText) _
                .Replace("[QTEXT]", myQtext) _
                .Replace("[NEXT_NUMID]", CodeGen.NumId))

            CodeGen.EventsToCode(MyControlId, Me)
        End Sub

#End If

        Private Sub AniTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles AniTimer.Tick
            If Common.AnimationEnable Then
                Me._Value += Rnd(1) * (Me._MaxValue - Me._MinValue) / 10 * _AnimationDirection
                If Me._Value > Me._MaxValue Then
                    Me._Value = Me._MaxValue
                    _AnimationDirection *= -1
                ElseIf Me._Value < _MinValue Then
                    Me._Value = _MinValue
                    _AnimationDirection *= -1
                End If
                Me.Invalidate()
            End If
        End Sub

        Private Sub Meter_QueryContinueDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.QueryContinueDragEventArgs) Handles Me.QueryContinueDrag
            If _Scheme IsNot Nothing AndAlso _Scheme.Font IsNot Nothing AndAlso _Scheme.Font.Charset = VGDDFont.FontCharset.SELECTION Then
                If _Scheme.Font.SmartCharSet Then
                    _Scheme.Font.SmartCharSetAddString(Me.Text)
                End If
            End If
        End Sub

    End Class

End Namespace
