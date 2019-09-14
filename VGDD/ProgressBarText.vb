Public Class ProgressBarText
    Inherits System.Windows.Forms.ProgressBar

    Private _TextFormat = "{0}% Done"
    'Private BackDrawn As Boolean = False
    Private _UseVisualStyles As Boolean = True

    Sub New()
        MyBase.New()
        Me.SetStyle(Windows.Forms.ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(Windows.Forms.ControlStyles.UserPaint, True)
        Me.SetStyle(Windows.Forms.ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(Windows.Forms.ControlStyles.ResizeRedraw, True)

        'Me.BarStyle = BrushBarStyle.Solid
        'Me.BackColor = System.Drawing.SystemColors.Control
        'Me.ForeColor = System.Drawing.SystemColors.Highlight
        ''Me.Font = MyBase.Font
        'Me.FontColor = System.Drawing.SystemColors.WindowText
    End Sub

    Public Property UseVisualStyles As Boolean
        Get
            Return _UseVisualStyles
        End Get
        Set(ByVal value As Boolean)
            _UseVisualStyles = value
        End Set
    End Property

    Public Property TextFormat As String
        Set(value As String)
            _TextFormat = value
        End Set
        Get
            Return _TextFormat
        End Get
    End Property

    'Public Shadows Property Style
    '    Get
    '        Return MyBase.Style
    '    End Get
    '    Set(ByVal value)
    '        MyBase.Style = value
    '        If value = ProgressBarStyle.Marquee Then
    '            Me.SetStyle(Windows.Forms.ControlStyles.AllPaintingInWmPaint, False)
    '            Me.SetStyle(Windows.Forms.ControlStyles.UserPaint, False)
    '        End If
    '    End Set
    'End Property

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed

        'If Not BackDrawn Then
        '    BackDrawn = True
        Using backBrush As New SolidBrush(Me.BackColor)
            e.Graphics.FillRectangle(backBrush, Me.ClientRectangle)
        End Using
        'End If

        Dim valueLen As Integer = Me.Value - Me.Minimum
        valueLen *= (Me.Size.Width - 2 * MyBase.Margin.All)
        valueLen /= (Me.Maximum - Me.Minimum)
        Dim rect As New Rectangle(Me.ClientRectangle.X + MyBase.Margin.All, Me.ClientRectangle.Y + MyBase.Margin.All, valueLen, Me.ClientRectangle.Height - 2 * MyBase.Margin.All)

        Select Case Me.style
            Case ProgressBarStyle.Continuous, ProgressBarStyle.Blocks
                If _UseVisualStyles Then
                    ProgressBarRenderer.DrawHorizontalChunks(e.Graphics, rect)
                Else
                    Using foreBrush As New SolidBrush(Me.ForeColor)
                        e.Graphics.FillRectangle(foreBrush, rect)
                    End Using
                End If
            Case ProgressBarStyle.Marquee
                Using foreBrush As New SolidBrush(Me.ForeColor)
                    e.Graphics.FillRectangle(foreBrush, rect)
                End Using
        End Select
        TextRenderer.DrawText(e.Graphics, String.Format(_TextFormat, ((MyBase.Value * 100) \ MyBase.Maximum).ToString()), Control.DefaultFont, Me.ClientRectangle, Color.Black, Color.Empty, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        'e.Graphics.DrawString(String.Format(_TextFormat, ((MyBase.Value * 100) \ MyBase.Maximum).ToString()), New Font("Verdana", 8, FontStyle.Regular), New SolidBrush(Color.Black), MyBase.ClientRectangle, StringFormat.GenericTypographic)
    End Sub
End Class
