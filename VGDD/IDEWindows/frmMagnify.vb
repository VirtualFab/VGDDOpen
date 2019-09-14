Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports System.IO

Namespace VGDDCommon
    Public Class frmMagnify
        Private bmpScreenShot As Bitmap
        Private MouseMoveStart As Point
        Dim MagnifyDeltaX As Integer
        Dim MagnifyDeltaY As Integer
        Public WithEvents tmrMagnify As New Timer

        Public Sub New()
            InitializeComponent()
            Me.SetStyle(ControlStyles.DoubleBuffer, True)
            Me.SetStyle(ControlStyles.UserPaint, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.DoubleBuffered = True
            Me.TopMost = True
            tmrMagnify.Interval = 100
            tmrMagnify.Enabled = True
        End Sub

        'Private Sub frmMagnify_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        '    If Me.Opacity = 0 Then
        '        FadeForm.FadeIn(Me, 99)
        '    End If
        'End Sub

        Private Sub frmMagnify_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            My.Settings.MagnifyWindowLocation = Me.Location
            My.Settings.MagnifyWindowSize = Me.Size
            My.Settings.MagnifyWindowZoomFactor = Me.TrackBar1.Value
            'FadeForm.FadeOutAndWait(Me)
        End Sub

        Private Sub Form_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.Location = My.Settings.MagnifyWindowLocation
            Me.Size = My.Settings.MagnifyWindowSize
            Me.TrackBar1.Value = My.Settings.MagnifyWindowZoomFactor
            oMainShell.MagnifyLockStatus = oMainShell.MagnifyLockStatus
            RecalcScreenShotSize()
            If Common.DoFadings Then Me.Opacity = 0
        End Sub

        Sub RecalcScreenShotSize()
            If bmpScreenShot IsNot Nothing Then
                bmpScreenShot.Dispose()
            End If
            bmpScreenShot = New Bitmap(CInt(Me.Width / TrackBar1.Value), CInt(Me.Height / TrackBar1.Value))
        End Sub

        Sub GetScreenShot()
            Try
                Dim scrPt As Point
                If oMainShell.MagnifyLockStatus = MagnifyStatuses.Locked Then
                    scrPt = oMainShell.MagnyfyPoint
                    scrPt.X -= MagnifyDeltaX
                    scrPt.Y -= MagnifyDeltaY
                Else
                    scrPt = Control.MousePosition
                End If

                scrPt.X = CInt(scrPt.X - bmpScreenShot.Width / 2)
                scrPt.Y = CInt(scrPt.Y - bmpScreenShot.Height / 2)

                Using g As Graphics = Graphics.FromImage(bmpScreenShot)
                    g.CopyFromScreen(scrPt, New Point(0, 0), bmpScreenShot.Size)
                End Using

                Me.Refresh()
            Catch ex As Exception

            End Try
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
            MyBase.OnPaint(e)
            If bmpScreenShot IsNot Nothing Then
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor
                e.Graphics.SmoothingMode = SmoothingMode.None
                Try
                    e.Graphics.DrawImage(bmpScreenShot, 0, TrackBar1.Height, Me.Width, Me.Height - TrackBar1.Height)
                Catch ex As Exception
                End Try
            End If
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        End Sub

        Private Sub tmrMagnify_Tick(sender As Object, e As System.EventArgs) Handles tmrMagnify.Tick
            GetScreenShot()
            Me.Invalidate()
        End Sub

        Private Sub TrackBar1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrackBar1.ValueChanged
            RecalcScreenShotSize()
            lblZoom.Text = "Zoom: " & TrackBar1.Value & "X"
            oMainShell.Focus()
        End Sub

        Private Sub btnLock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLock.Click
            If oMainShell._CurrentHost IsNot Nothing Then
                Select Case oMainShell.MagnifyLockStatus
                    Case MagnifyStatuses.Unlocked
                        oMainShell.MagnifyLockStatus = MagnifyStatuses.Locking
                    Case MagnifyStatuses.Locked
                        oMainShell.MagnifyLockStatus = MagnifyStatuses.Unlocked
                    Case MagnifyStatuses.Locking
                        oMainShell.MagnifyLockStatus = MagnifyStatuses.Unlocked
                End Select
            End If
        End Sub

        Private Sub frmMagnify_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
            If oMainShell.MagnifyLockStatus = MagnifyStatuses.Locked Then
                MouseMoveStart = e.Location
                MagnifyDeltaX = 0
                MagnifyDeltaY = 0
                Me.Cursor = Cursors.Hand
            End If
        End Sub

        Private Sub frmMagnify_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
            If oMainShell.MagnifyLockStatus = MagnifyStatuses.Locked Then
                oMainShell.MagnyfyPoint.X -= MagnifyDeltaX
                oMainShell.MagnyfyPoint.Y -= MagnifyDeltaY
                MagnifyDeltaX = 0
                MagnifyDeltaY = 0
                Me.Cursor = Cursors.Default
                oMainShell.Focus()
            End If
        End Sub

        Private Sub frmMagnify_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
            If e.Button = MouseButtons.Left AndAlso oMainShell.MagnifyLockStatus = MagnifyStatuses.Locked Then
                MagnifyDeltaX = (e.Location.X - MouseMoveStart.X)
                If oMainShell.MagnyfyPoint.X - MagnifyDeltaX - bmpScreenShot.Width / 2 < 0 Then
                    MagnifyDeltaX = oMainShell.MagnyfyPoint.X - bmpScreenShot.Width / 2
                ElseIf oMainShell.MagnyfyPoint.X - MagnifyDeltaX + bmpScreenShot.Width / 2 > Screen.PrimaryScreen.Bounds.Width Then
                    MagnifyDeltaX = oMainShell.MagnyfyPoint.X + bmpScreenShot.Width / 2 - Screen.PrimaryScreen.Bounds.Width
                End If
                MagnifyDeltaY = (e.Location.Y - MouseMoveStart.Y)
                If oMainShell.MagnyfyPoint.Y - MagnifyDeltaY - bmpScreenShot.Height / 2 < 0 Then
                    MagnifyDeltaY = oMainShell.MagnyfyPoint.Y - bmpScreenShot.Height / 2
                ElseIf oMainShell.MagnyfyPoint.Y - MagnifyDeltaY + bmpScreenShot.Height / 2 > Screen.PrimaryScreen.Bounds.Height Then
                    MagnifyDeltaY = oMainShell.MagnyfyPoint.Y + bmpScreenShot.Height / 2 - Screen.PrimaryScreen.Bounds.Height
                End If
            End If
        End Sub

    End Class

    Public Enum MagnifyStatuses
        Locked
        Locking
        Unlocked
    End Enum
End Namespace

