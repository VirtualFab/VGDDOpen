Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Collections
Imports VGDDCommon
Imports VGDDCommon.Common

Namespace VGDDMicrochip

    <System.Reflection.ObfuscationAttribute(Feature:="renaming", exclude:=True)> _
    <ToolboxBitmap(GetType(Indicator), "Indicator.ico")> _
    Public Class Indicator : Inherits VGDDWidget

        Private _Value As Integer = 1
        Private _IndicatorColour As Color = Color.Red
        Private _Frame As EnabledState
        Private _TextAlign As HorizAlign = HorizAlign.Left
        Private _Style As IndicatorsStyle

        Private Shared _Instances As Integer = 0

        Public Enum IndicatorsStyle
            INDSTYLE_CIRCLE
            INDSTYLE_SQUARE
        End Enum

        Public Sub New()
            MyBase.New()
            _Instances += 1
#If Not PlayerMonolitico Then
            Me.VGDDEvents = CodeGen.GetEventsFromTemplate("Indicator")
#End If
            Me.Size = New Size(150, 25)
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

        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Overrides ReadOnly Property Demolimit As Integer
            Get
                Return Common.DEMOCODELIMIT
            End Get
        End Property

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
            Mypath.AddRectangle(Me.ClientRectangle)
            Mypath.CloseFigure()
            Me.Region = New Region(Mypath)
            If _Scheme Is Nothing Then Exit Sub
            Dim brushBackGround As New SolidBrush(_Scheme.Commonbkcolor)
            g.FillRegion(brushBackGround, Me.Region)

            Dim brushPen As New SolidBrush(_IndicatorColour)
            Dim ps As Pen = New Pen(brushPen)

            ps.Width = 2
            ps.DashStyle = DashStyle.Solid
            If _Frame = EnabledState.Enabled Then
                ps.Color = _Scheme.Color1
                g.DrawRectangle(ps, rc.Left, rc.Top, rc.Right, rc.Bottom)
            End If

            Dim Border As Integer = 2

            'Draw Indicator
            ps.DashStyle = DashStyle.Dot
            Select Case _Style
                Case IndicatorsStyle.INDSTYLE_CIRCLE
                    If _Value Then
                        g.FillEllipse(brushPen, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2))
                    Else
                        g.DrawArc(ps, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2), 0, 360)
                    End If
                Case IndicatorsStyle.INDSTYLE_SQUARE
                    If _Value Then
                        g.FillRectangle(brushPen, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2))
                    Else
                        g.DrawRectangle(ps, CInt(rc.Left + Border), CInt(rc.Top + Border), CInt(Me.Height - Border * 2), CInt(Me.Height - Border * 2))
                    End If
            End Select

            'Draw Text
            Dim textBrush As SolidBrush = New SolidBrush(_Scheme.Textcolor0)
            Dim intTextSize As SizeF = g.MeasureString(Text, MyBase.Font)
            Select Case _TextAlign
                Case HorizAlign.Center
                    g.DrawString(Text, MyBase.Font, textBrush, Me.Height + (Me.Width - Me.Height - intTextSize.Width - 8) / 2, (Height - intTextSize.Height) / 2)
                Case HorizAlign.Right
                    g.DrawString(Text, MyBase.Font, textBrush, Width - intTextSize.Width, (Height - intTextSize.Height) / 2)
                Case Else
                    g.DrawString(Text, MyBase.Font, textBrush, Me.Height, (Height - intTextSize.Height) / 2)
            End Select
        End Sub

#Region "GDDProps"

#If PlayerMonolitico Then
        Public Shadows Property Text() As String
#Else
        <Description("Text of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        Public Shadows Property Text() As String
#End If
            Get
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                MyBase.Text = value
                Me.Invalidate()
            End Set
        End Property

#If Not PlayerMonolitico Then
        <Description("Colour for the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <Editor(GetType(MyColorEditor), GetType(System.Drawing.Design.UITypeEditor)), TypeConverter(GetType(MyColorConverter))> _
        <DefaultValue(GetType(Color), "Color.Red")> _
        <VGDDBase.CustomSortedCategory("Appearance", 4)> _
        Public Property IndicatorColour() As Color
#Else
        Public Property IndicatorColour() As Color
#End If
            Get
                Return _IndicatorColour
            End Get
            Set(ByVal value As Color)
                _IndicatorColour = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Style for the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(IndicatorsStyle.INDSTYLE_CIRCLE)> _
        <VGDDBase.CustomSortedCategory("Appearance", 4)> _
        Public Property Style() As IndicatorsStyle
            Get
                Return _Style
            End Get
            Set(ByVal value As IndicatorsStyle)
                _Style = value
                Me.Invalidate()
            End Set
        End Property

        <Description("1=Filled 0=Dotted Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <VGDDBase.CustomSortedCategory("Appearance", 4)> _
        Public Property Value() As Integer
            Get
                Return _Value
            End Get
            Set(ByVal value As Integer)
                _Value = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Wether to enable a frame around the Indicator or not")> _
        <DefaultValue(GetType(EnabledState), "Disabled")> _
        <VGDDBase.CustomSortedCategory("Appearance", 4)> _
        Property Frame() As EnabledState
            Get
                Return _Frame
            End Get
            Set(ByVal value As EnabledState)
                _Frame = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Status of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(EnabledState), "Enabled")> _
        <VGDDBase.CustomSortedCategory("Appearance", 4)> _
        Public Overloads Property State() As EnabledState
            Get
                Return _State
            End Get
            Set(ByVal value As EnabledState)
                _State = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Text Alignement of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(GetType(HorizAlign), "Left")> _
        <VGDDBase.CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property TextAlign() As HorizAlign
            Get
                Return _TextAlign
            End Get
            Set(ByVal value As HorizAlign)
                _TextAlign = value
                Me.Invalidate()
            End Set
        End Property

        <Description("Visibility of the Indicator")> _
        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True)> _
        <DefaultValue(False)> _
        <VGDDBase.CustomSortedCategory("Appearance", 4)> _
        Public Shadows Property Hidden() As Boolean
            Get
                Return _Hidden
            End Get
            Set(ByVal value As Boolean)
                _Hidden = value
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
            Dim MyCodeHead As String = CodeGen.MyCodeHead(_CDeclType)
            Dim MyCode As String = "", MyState As String = ""

            Dim MyClassName As String = Me.GetType.ToString

            If MyControlId <> MyControlIdNoIndex Then
                MyControlIdIndexPar = MyControlId.Substring(MyControlIdNoIndex.Length)
                MyControlIdIndex = MyControlIdIndexPar.Replace("[", "").Replace("]", "")
            End If

            If _public Then
                CodeGen.AddLines(MyCodeHead, CodeGen.ConstructorTemplate.Trim)
            Else
                CodeGen.AddLines(MyCode, CodeGen.ConstructorTemplate)
            End If
            CodeGen.AddLines(MyCodeHead, CodeGen.CodeHeadTemplate)

            CodeGen.AddLines(MyCode, CodeGen.CodeTemplate)
            CodeGen.AddLines(MyCode, CodeGen.AllCodeTemplate.Trim)

            CodeGen.AddState(MyState, "Enabled", Me.Enabled.ToString)
            CodeGen.AddState(MyState, "Hidden", Me.Hidden.ToString)
            CodeGen.AddState(MyState, "TextAlign", Me.TextAlign.ToString)
            CodeGen.AddState(MyState, "Frame", Me.Frame.ToString)
            If Me.Text = String.Empty Then
                MyCodeHead = ""
            End If

            Dim myText As String = ""
            Dim myQtext As String = CodeGen.QText(Me.Text, Me._Scheme.Font, myText)

            CodeGen.AddLines(CodeGen.Code, MyCode _
                .Replace("[LEFT]", Left).Replace("[TOP]", Top).Replace("[RIGHT]", Right).Replace("[BOTTOM]", Bottom) _
                .Replace("[STATE]", MyState) _
                .Replace("[WIDGETTEXT]", IIf(Me.Text = String.Empty, """""", CodeGen.WidgetsTextTemplateCode)) _
                .Replace("[STRINGID]", _TextStringID - 1) _
                .Replace("[VALUE]", Me.Value) _
                .Replace("[STYLE]", _Style.ToString) _
                .Replace("[COLOUR]", CodeGen.Color2Num(_IndicatorColour, False, "Indicator " & Me.Name)) _
                .Replace("[COLOUR_STRING]", CodeGen.Color2String(_IndicatorColour)) _
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
            If Not CodeGen.HeadersIncludes.Contains(CodeGen.HeadersIncludesTemplate) Then ' #include "indicator.h"
                CodeGen.AddLines(CodeGen.HeadersIncludes, CodeGen.HeadersIncludesTemplate)
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

            Try
                'Dim myAssembly As Reflection.Assembly = System.Reflection.Assembly.GetAssembly(Me.GetType)
                For Each oFolderNode As Xml.XmlNode In CodeGen.XmlTemplatesDoc.SelectNodes(String.Format("VGDDCodeTemplate/ControlsTemplates/{0}/Project/*", MyClassName.Split(".")(1)))
                    MplabX.AddFile(oFolderNode)
                Next
            Catch ex As Exception
            End Try
        End Sub
#End If
#End Region

    End Class

End Namespace