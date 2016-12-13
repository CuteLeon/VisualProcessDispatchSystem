Imports System.ComponentModel

Public Class MainForm
    Private Const Max_JobCount As Integer = 12 '必须与 JobColors 个数匹配
    Private Const ButtonAlphaDefault As Integer = 50
    Private Const Max_SystemTime As Integer = 60
    Private Const ButtonAlphaActive As Integer = 100
    Private Const ButtonAlphaExecute As Integer = 150
    Private Const ShadowDistance As Integer = 5
    Dim ButtonColorDefault As Color = Color.Orange
    Dim ButtonColorActive As Color = Color.Aqua
    Dim ButtonColorExecute As Color = Color.Red
    Dim Buttons() As Label
    Dim JobColors() As Color = {'伊登十二色环配色
        Color.FromArgb(227, 35, 34),
        Color.FromArgb(244, 229, 0),
        Color.FromArgb(38, 113, 178),
        Color.FromArgb(0, 142, 91),
        Color.FromArgb(241, 145, 1),
        Color.FromArgb(109, 56, 137),
        Color.FromArgb(253, 198, 11),
        Color.FromArgb(234, 98, 31),
        Color.FromArgb(196, 3, 125),
        Color.FromArgb(68, 78, 153),
        Color.FromArgb(6, 15, 187),
        Color.FromArgb(140, 187, 38)}

    Dim AllJobList As New List(Of JobClass)
    Dim WaitJobList As New List(Of JobClass)
    Dim ExecuteJob As JobClass
    Dim SystemClock As Integer = 0
    Dim NextJobSubscript As Integer
    Dim ExecuteTime As Integer
    Dim SystemRandom As Random = New Random
    Dim CoordinateRectangle As Rectangle
    Dim DispathRectangle As Rectangle
    Dim ExecuteRectangle As Rectangle
    Dim WaitRectangle As Rectangle
    Dim TimeCellWidth As Double
    Dim TimeCellHeight As Double
    Dim DispathCellWidth As Double
    Dim DispathCellHeight As Double
    Dim RecordCellWidth As Double
    Dim RecordCellHeight As Double
    Dim ResponseRatios As New List(Of Double)

    Private Structure ExecuteLog
        Dim ID As Integer
        Dim Name As String
        Dim ExecuteTime As Integer
        Dim CompleteTime As Integer
        Dim Color As Color
        Public Sub New(jID As Integer, jName As String, eTime As Integer, tLength As Integer, jColor As Color)
            ID = jID
            Name = jName
            ExecuteTime = eTime
            CompleteTime = eTime + tLength
            Color = jColor
        End Sub
    End Structure
    '记录执行记录以生成结果流程图
    Dim ExecuteLogs As New List(Of ExecuteLog)

#Region "窗体事件"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Buttons = New Label() {CreateJobListButton, PlayPauseButton, SpeedDownButton, SpeedUpButton}

        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.SupportsTransparentBackColor, True)
        Me.Icon = My.Resources.UnityResource.SystemICON
        CloseButton.Left = Me.Width - CloseButton.Width
        MinButton.Left = CloseButton.Left - MinButton.Width
        SettingButton.Left = MinButton.Left - SettingButton.Width
        ReplayCheckBox.Left = SettingButton.Left - ReplayCheckBox.Width - 10

        Dim LastRightPoint As Integer = 15
        '设置纯色按钮的初始颜色
        For Each InsButton As Label In Buttons
            InsButton.BackColor = Color.FromArgb(ButtonAlphaDefault, ButtonColorDefault)
            InsButton.Left = LastRightPoint
            LastRightPoint += InsButton.Width + 15
            AddHandler InsButton.MouseEnter, AddressOf ColorButton_MouseEnter
            AddHandler InsButton.MouseDown, AddressOf ColorButton_MouseDown
            AddHandler InsButton.MouseUp, AddressOf ColorButton_MouseUp
            AddHandler InsButton.MouseLeave, AddressOf ColorButton_MouseLeave
        Next
        DispathComboBox.Left = LastRightPoint

        '设置容器控件的背景颜色
        TimeLinePanel.BackColor = Color.FromArgb(50, Color.White)
        DispathPanel.BackColor = Color.FromArgb(50, Color.Gray)
        RecordPanel.BackColor = Color.FromArgb(50, Color.Gray)
        LogLabel.BackColor = Color.FromArgb(50, Color.White)

        '调整容器控件的位置和尺寸
        TimeLinePanel.Location = New Point(15, CreateJobListButton.Bottom + 15)
        TimeLinePanel.Size = New Size((Me.Width - 40) * 0.7, (Me.Height - TimeLinePanel.Top - 15) * 0.2)
        DispathPanel.Location = New Point(15, TimeLinePanel.Bottom + 10)
        DispathPanel.Size = New Size(TimeLinePanel.Width, (Me.Height - TimeLinePanel.Top - 10) * 0.6)
        RecordPanel.Location = New Point(TimeLinePanel.Left, DispathPanel.Bottom + 10)
        RecordPanel.Size = New Size(TimeLinePanel.Width, Me.Height - DispathPanel.Bottom - 25)
        LogLabel.Location = New Point(TimeLinePanel.Right + 10, TimeLinePanel.Top)
        LogLabel.Size = New Size((Me.Width - 40) * 0.3, RecordPanel.Bottom - TimeLinePanel.Top)

        CoordinateRectangle = New Rectangle(15, 25, TimeLinePanel.Width - 30, TimeLinePanel.Height - 45)
        TimeCellWidth = CoordinateRectangle.Width / Max_SystemTime
        TimeCellHeight = CoordinateRectangle.Height / Max_JobCount
        DispathRectangle = New Rectangle(15, 25, DispathPanel.Width - 30, DispathPanel.Height - 45)
        DispathCellHeight = (DispathRectangle.Height + ExecuteRectangle.Top - DispathRectangle.Top - WaitLabel.Height - 10 - Max_JobCount * 2) / (Max_JobCount + 1)
        ExecuteRectangle = New Rectangle(15, 50, DispathRectangle.Width, DispathCellHeight)
        WaitRectangle.Location = New Point(ExecuteRectangle.Left + ExecuteRectangle.Width * 0.3, ExecuteRectangle.Bottom + WaitLabel.Height + 10)
        WaitRectangle.Size = New Size(ExecuteRectangle.Right - WaitRectangle.Left, (DispathCellHeight + 2) * Max_JobCount)
        DispathCellWidth = WaitRectangle.Width / Max_SystemTime

        RecordCellWidth = RecordPanel.Width
        RecordCellHeight = RecordPanel.Height / Max_JobCount

        TimeLineLabel.Parent = TimeLinePanel
        TimeLineLabel.Image = New Bitmap(My.Resources.UnityResource.TimeLine, 29, CoordinateRectangle.Height)
        TimeLineLabel.SetBounds(CoordinateRectangle.Left - 14, CoordinateRectangle.Top, TimeLineLabel.Image.Width, TimeLineLabel.Image.Height)

        ExecuteLabel.Parent = DispathPanel
        ExecuteLabel.Location = New Point(WaitRectangle.Left, ExecuteRectangle.Top - ExecuteLabel.Height - 5)
        WaitLabel.Parent = DispathPanel
        NextJobTipLabel.Parent = DispathPanel
        WaitLabel.Location = New Point(WaitRectangle.Left, WaitRectangle.Top - WaitLabel.Height - 5)

        DispathComboBox.SelectedIndex = 1
        SystemClockLabel.Left = LogLabel.Right - SystemClockLabel.Width
        SystemClockTitle.Left = SystemClockLabel.Left - SystemClockTitle.Width
    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Me.Tag = True Then Exit Sub
        e.Cancel = True
        If MsgBox("就这样退出吗？真的不爱了？", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, "确定不留下么？") = MsgBoxResult.Ok Then Me.Tag = True : Application.Exit()
    End Sub

#End Region

#Region "标题栏按钮事件"

    Private Sub MinButton_Click(sender As Object, e As EventArgs) Handles MinButton.Click
        If SystemClockTimer.Enabled Then PlayPauseButton_Click(sender, e)
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        If MsgBox("就这样退出吗？真的不爱了？", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, "确定不留下么？") = MsgBoxResult.Ok Then Me.Tag = True : Application.Exit()
    End Sub

    Private Sub SettingButton_Click(sender As Object, e As EventArgs) Handles SettingButton.Click
        If Not AboutMe.Visible Then AboutMe.ShowDialog(Me)
    End Sub

#End Region

#Region "图像按钮动态效果"

    Private Sub ControlButton_MouseDown(sender As Label, e As MouseEventArgs) Handles MinButton.MouseDown, CloseButton.MouseDown, SettingButton.MouseDown
        sender.Image = My.Resources.UnityResource.ResourceManager.GetObject(sender.Tag & "_2")
    End Sub

    Private Sub ControlButton_MouseEnter(sender As Label, e As EventArgs) Handles MinButton.MouseEnter, CloseButton.MouseEnter, SettingButton.MouseEnter
        sender.Image = My.Resources.UnityResource.ResourceManager.GetObject(sender.Tag & "_1")
    End Sub

    Private Sub ControlButton_MouseLeave(sender As Label, e As EventArgs) Handles MinButton.MouseLeave, CloseButton.MouseLeave, SettingButton.MouseLeave
        sender.Image = My.Resources.UnityResource.ResourceManager.GetObject(sender.Tag & "_0")
    End Sub

    Private Sub ControlButton_MouseUp(sender As Label, e As MouseEventArgs) Handles MinButton.MouseUp, CloseButton.MouseUp, SettingButton.MouseUp
        sender.Image = My.Resources.UnityResource.ResourceManager.GetObject(sender.Tag & "_1")
    End Sub

#End Region

#Region "纯色按钮动态效果"

    Private Sub ColorButton_MouseDown(sender As Label, e As MouseEventArgs)
        sender.BackColor = Color.FromArgb(ButtonAlphaExecute, ButtonColorExecute)
    End Sub

    Private Sub ColorButton_MouseEnter(sender As Label, e As EventArgs)
        sender.BackColor = Color.FromArgb(ButtonAlphaActive, ButtonColorActive)
    End Sub

    Private Sub ColorButton_MouseLeave(sender As Label, e As EventArgs)
        sender.BackColor = Color.FromArgb(ButtonAlphaDefault, ButtonColorDefault)
    End Sub

    Private Sub ColorButton_MouseUp(sender As Label, e As MouseEventArgs)
        sender.BackColor = Color.FromArgb(ButtonAlphaActive, ButtonColorActive)
    End Sub

#End Region

#Region "功能函数"

    ''' <summary>
    ''' 重置系统配置
    ''' </summary>
    Private Sub ResetSystem()
        SystemClock = 0
        NextJobSubscript = -1
        ExecuteTime = -1
        ExecuteJob = Nothing
        SystemClockLabel.Text = "0"
        SystemClockTimer.Stop()
        PlayPauseButton.Text = "播放   "
        PlayPauseButton.Image = My.Resources.UnityResource.Play
        DispathPanel.Image = Nothing
        TimeLineLabel.Hide()
        ExecuteLabel.Hide()
        WaitLabel.Hide()
        NextJobTipLabel.Hide()
        TimeLineLabel.Location = New Point(CoordinateRectangle.Left - 14, CoordinateRectangle.Top - 1)
        LogLabel.TextBox.Text = "重置系统配置！" & vbCrLf
    End Sub

    ''' <summary>
    ''' 重新生成作业列表
    ''' </summary>
    Private Sub CreateJobsList()
        AllJobList.Clear()
        WaitJobList.Clear()
        ExecuteLogs.Clear()
        LogLabel.TextBox.Text &= "开始重新生成作业队列..." & vbCrLf
        For Index As Integer = 0 To Max_JobCount - 1
            Dim JobStartTime As Integer = SystemRandom.Next(Max_SystemTime)
            Dim JobEndTime As Integer = JobStartTime + 1 + SystemRandom.Next(Max_SystemTime - JobStartTime)
            Dim InsJob As JobClass = New JobClass(Index, "作业-" & Index, JobStartTime, JobEndTime, JobColors(Index), SystemRandom.Next(Max_JobCount))
            AllJobList.Add(InsJob)
            LogLabel.TextBox.Text &= String.Format("    生成作业：{0} / 时间：{1}-{2}", InsJob.Name, InsJob.StartTime, InsJob.EndTime) & vbCrLf
        Next
        LogLabel.TextBox.Text &= "生成作业队列完毕！" & vbCrLf
        LogLabel.TextBox.Text &= "================" & vbCrLf
    End Sub

    ''' <summary>
    ''' 绘制时间线区域图像
    ''' </summary>
    Private Function CreateTimeLineImage() As Image
        Dim Index As Integer
        Dim TimeLineImage As Bitmap = New Bitmap(TimeLinePanel.Width, TimeLinePanel.Height)
        Dim TimeLineGraphics As Graphics = Graphics.FromImage(TimeLineImage)
        Dim TempPen As Pen = New Pen(Color.FromArgb(150, Color.MediumSpringGreen), 3)
        'TimeLineGraphics.FillRectangle(Brushes.Yellow, CoordinateRectangle)
        TimeLineGraphics.DrawLine(TempPen, CoordinateRectangle.Left, CoordinateRectangle.Top + 1, CoordinateRectangle.Left, CoordinateRectangle.Bottom + 1)
        TimeLineGraphics.DrawLine(TempPen, CoordinateRectangle.Left - 1, CoordinateRectangle.Bottom + 1, CoordinateRectangle.Right + 1, CoordinateRectangle.Bottom + 1)
        TempPen = New Pen(Color.FromArgb(100, Color.White), 1)
        For Index = 0 To Max_SystemTime Step 5
            TimeLineGraphics.DrawLine(TempPen, CInt(CoordinateRectangle.Left + TimeCellWidth * Index), CoordinateRectangle.Top, CInt(CoordinateRectangle.Left + TimeCellWidth * Index), CoordinateRectangle.Bottom)
            TimeLineGraphics.DrawString(Index, Me.Font, Brushes.Yellow, CoordinateRectangle.Left + TimeCellWidth * Index - 4, CoordinateRectangle.Bottom + 2)
        Next
        For Index = 0 To Max_JobCount - 1
            TempPen = New Pen(AllJobList(Index).Color, 2)
            TimeLineGraphics.DrawLine(TempPen, CInt(CoordinateRectangle.Left + CoordinateRectangle.Width * (AllJobList(Index).StartTime / Max_SystemTime)), CInt(CoordinateRectangle.Top + Index * TimeCellHeight + 5), CInt(CoordinateRectangle.Left + CoordinateRectangle.Width * (AllJobList(Index).EndTime / Max_SystemTime)), CInt(CoordinateRectangle.Top + Index * TimeCellHeight + 5))
            TimeLineGraphics.FillEllipse(New SolidBrush(AllJobList(Index).Color), CInt(CoordinateRectangle.Left + CoordinateRectangle.Width * (AllJobList(Index).StartTime / Max_SystemTime)) - 3, CInt(CoordinateRectangle.Top + Index * TimeCellHeight + 2), 5, 5)
            TimeLineGraphics.DrawString("JID-" & Index, Me.Font, New SolidBrush(AllJobList(Index).Color), CInt(CoordinateRectangle.Left + TimeCellWidth * AllJobList(Index).StartTime - 10), CoordinateRectangle.Top + Index * TimeCellHeight - 7)
        Next
        Return TimeLineImage
    End Function

    ''' <summary>
    ''' 绘制调度区图像
    ''' </summary>
    Private Function CreateDispathImage() As Image
        On Error Resume Next
        Dim DispathImage As Bitmap = New Bitmap(DispathPanel.Width, DispathPanel.Height)
        Dim DispathGraphics As Graphics = Graphics.FromImage(DispathImage)
        Dim WaitJobPoint As Point
        Dim WaitJobSize As Size
        Dim TempPen As Pen
        'DispathGraphics.FillRectangle(Brushes.Goldenrod, ExecuteRectangle)
        'DispathGraphics.DrawRectangle(Pens.Red, ExecuteRectangle)
        'DispathGraphics.FillRectangle(Brushes.AliceBlue, WaitRectangle)
        'DispathGraphics.DrawRectangle(Pens.Red, WaitRectangle)

        DispathGraphics.DrawLine(Pens.Red, WaitRectangle.Left + ShadowDistance, ExecuteRectangle.Top + ShadowDistance, WaitRectangle.Left + ShadowDistance, WaitRectangle.Bottom + ShadowDistance)
        DispathGraphics.DrawLine(Pens.Red, WaitRectangle.Left, ExecuteRectangle.Top, WaitRectangle.Left + ShadowDistance, ExecuteRectangle.Top + ShadowDistance)

        If WaitJobList.Count > 0 Then
            If Not WaitLabel.Visible Then WaitLabel.Show()
            If Not NextJobTipLabel.Visible Then NextJobTipLabel.Show()
            Dim InsWaitJob As JobClass
            For Index As Integer = 0 To WaitJobList.Count - 1
                InsWaitJob = WaitJobList(Index)
                WaitJobPoint = New Point(WaitRectangle.Left, WaitRectangle.Top + Index * (DispathCellHeight + 2))
                WaitJobSize = New Size(WaitRectangle.Width * InsWaitJob.TimeLength / Max_SystemTime, DispathCellHeight)

                TempPen = New Pen(InsWaitJob.Color, 1)
                DispathGraphics.DrawLine(TempPen, WaitJobPoint.X, WaitJobPoint.Y + WaitJobSize.Height, WaitJobPoint.X + ShadowDistance, WaitJobPoint.Y + WaitJobSize.Height + ShadowDistance)
                DispathGraphics.DrawLine(TempPen, WaitJobPoint.X + WaitJobSize.Width, WaitJobPoint.Y, WaitJobPoint.X + WaitJobSize.Width + ShadowDistance, WaitJobPoint.Y + ShadowDistance)
                DispathGraphics.DrawLine(TempPen, WaitJobPoint.X + WaitJobSize.Width, WaitJobPoint.Y + WaitJobSize.Height, WaitJobPoint.X + WaitJobSize.Width + ShadowDistance, WaitJobPoint.Y + WaitJobSize.Height + ShadowDistance)

                DispathGraphics.DrawLine(TempPen, WaitJobPoint.X + ShadowDistance, WaitJobPoint.Y + ShadowDistance, WaitJobPoint.X + ShadowDistance, WaitJobPoint.Y + WaitJobSize.Height + ShadowDistance)
                DispathGraphics.DrawLine(TempPen, WaitJobPoint.X + ShadowDistance, WaitJobPoint.Y + ShadowDistance, WaitJobPoint.X + WaitJobSize.Width + ShadowDistance, WaitJobPoint.Y + ShadowDistance)
                DispathGraphics.DrawLine(TempPen, WaitJobPoint.X + WaitJobSize.Width + ShadowDistance, WaitJobPoint.Y + ShadowDistance, WaitJobPoint.X + WaitJobSize.Width + ShadowDistance, WaitJobPoint.Y + WaitJobSize.Height + ShadowDistance)
                DispathGraphics.DrawLine(TempPen, WaitJobPoint.X + ShadowDistance, WaitJobPoint.Y + WaitJobSize.Height + ShadowDistance, WaitJobPoint.X + WaitJobSize.Width + ShadowDistance, WaitJobPoint.Y + WaitJobSize.Height + ShadowDistance)

                DispathGraphics.FillRectangle(New SolidBrush(InsWaitJob.Color), New Rectangle(WaitJobPoint, WaitJobSize))
                If (NextJobSubscript = Index) Then
                    NextJobTipLabel.Location = New Point(WaitJobPoint.X - NextJobTipLabel.Width, WaitJobPoint.Y)
                End If
                WaitJobPoint.Offset(2, 2)
                Select Case DispathComboBox.SelectedIndex
                    Case 0
                        DispathGraphics.DrawString(InsWaitJob.Name & " / 到达顺序：" & Index, Me.Font, Brushes.White, WaitJobPoint)
                    Case 1
                        DispathGraphics.DrawString(InsWaitJob.Name & " / 作业长度：" & InsWaitJob.TimeLength, Me.Font, Brushes.White, WaitJobPoint)
                    Case 2
                        If ResponseRatios.Count > Index Then DispathGraphics.DrawString(InsWaitJob.Name & " / 响应比：" & ResponseRatios(Index), Me.Font, Brushes.White, WaitJobPoint)
                    Case 3
                        DispathGraphics.DrawString(InsWaitJob.Name & " / 优先数：" & InsWaitJob.Priority, Me.Font, Brushes.White, WaitJobPoint)
                    Case 4
                    Case 5
                End Select
            Next
        Else
            If WaitLabel.Visible Then WaitLabel.Hide()
            If NextJobTipLabel.Visible Then NextJobTipLabel.Hide()
        End If

        If Not (IsNothing(ExecuteJob)) Then
            If Not ExecuteLabel.Visible Then
                ExecuteLabel.Show()
            End If
            ExecuteLabel.Text = "正在执行：" & ExecuteJob.Name
            TempPen = New Pen(ExecuteJob.Color, 1)
            ExecuteRectangle.Location = New Point(WaitRectangle.Left - WaitRectangle.Width * (SystemClock - ExecuteTime) / Max_SystemTime, ExecuteRectangle.Top)

            DispathGraphics.DrawLine(TempPen, ExecuteRectangle.Left, ExecuteRectangle.Top + ExecuteRectangle.Height, ExecuteRectangle.Left + ShadowDistance, ExecuteRectangle.Top + ExecuteRectangle.Height + ShadowDistance)
            DispathGraphics.DrawLine(TempPen, ExecuteRectangle.Left + ExecuteRectangle.Width, ExecuteRectangle.Top, ExecuteRectangle.Left + ExecuteRectangle.Width + ShadowDistance, ExecuteRectangle.Top + ShadowDistance)
            DispathGraphics.DrawLine(TempPen, ExecuteRectangle.Left + ExecuteRectangle.Width, ExecuteRectangle.Top + ExecuteRectangle.Height, ExecuteRectangle.Left + ExecuteRectangle.Width + ShadowDistance, ExecuteRectangle.Top + ExecuteRectangle.Height + ShadowDistance)

            DispathGraphics.DrawLine(TempPen, ExecuteRectangle.Left + ShadowDistance, ExecuteRectangle.Top + ShadowDistance, ExecuteRectangle.Left + ShadowDistance, ExecuteRectangle.Top + ExecuteRectangle.Height + ShadowDistance)
            DispathGraphics.DrawLine(TempPen, ExecuteRectangle.Left + ShadowDistance, ExecuteRectangle.Top + ShadowDistance, ExecuteRectangle.Left + ExecuteRectangle.Width + ShadowDistance, ExecuteRectangle.Top + ShadowDistance)
            DispathGraphics.DrawLine(TempPen, ExecuteRectangle.Left + ExecuteRectangle.Width + ShadowDistance, ExecuteRectangle.Top + ShadowDistance, ExecuteRectangle.Left + ExecuteRectangle.Width + ShadowDistance, ExecuteRectangle.Top + ExecuteRectangle.Height + ShadowDistance)
            DispathGraphics.DrawLine(TempPen, ExecuteRectangle.Left + ShadowDistance, ExecuteRectangle.Top + ExecuteRectangle.Height + ShadowDistance, ExecuteRectangle.Left + ExecuteRectangle.Width + ShadowDistance, ExecuteRectangle.Top + ExecuteRectangle.Height + ShadowDistance)

            DispathGraphics.FillRectangle(New SolidBrush(ExecuteJob.Color), ExecuteRectangle)
            DispathGraphics.DrawRectangle(Pens.Red, ExecuteRectangle)
        Else
            ExecuteLabel.Hide()
        End If

        DispathGraphics.DrawLine(Pens.Red, WaitRectangle.Left, ExecuteRectangle.Top, WaitRectangle.Left, WaitRectangle.Bottom)
        DispathGraphics.DrawLine(Pens.Red, WaitRectangle.Left, WaitRectangle.Bottom, WaitRectangle.Left + ShadowDistance, WaitRectangle.Bottom + ShadowDistance)

        Return DispathImage
    End Function

    ''' <summary>
    ''' 检查作业到达
    ''' </summary>
    Private Sub CheckJobArrive()
        Dim JIDs As String = vbNullString
        Dim Index As Integer = 0, ListCount As Integer = AllJobList.Count
        Do While Index < ListCount
            If AllJobList(Index).StartTime = SystemClock Then
                JIDs &= AllJobList(Index).ID & " 和 "

                WaitJobList.Add(AllJobList(Index))

                AllJobList.RemoveAt(Index)
                ListCount -= 1
            Else
                Index += 1
            End If
        Loop
        If JIDs <> vbNullString Then
            LogLabel.TextBox.Text &= String.Format("系统时间：{0}  ||  作业 {1} 加入等待执行队列！", SystemClock, JIDs.Remove(JIDs.Length - 3)) & vbCrLf
        End If
    End Sub

    ''' <summary>
    ''' 检查作业执行结束
    ''' </summary>
    Private Sub CheckJobCompelet()
        If (IsNothing(ExecuteJob)) Then
            If WaitJobList.Count > 0 Then
                ExecuteJob = WaitJobList(NextJobSubscript)
                Dim ExecuteLog As ExecuteLog = New ExecuteLog(ExecuteJob.ID, ExecuteJob.Name, SystemClock, ExecuteJob.TimeLength, ExecuteJob.Color)
                ExecuteLogs.Add(ExecuteLog)
                LogLabel.TextBox.Text &= String.Format("系统时间：{0}  ||  开始执行 {1}！", SystemClock, ExecuteJob.Name) & vbCrLf
                WaitJobList.RemoveAt(NextJobSubscript)
                ExecuteTime = SystemClock
                ExecuteRectangle.Size = New Size(WaitRectangle.Width * (ExecuteJob.EndTime - ExecuteJob.StartTime) / Max_SystemTime, ExecuteRectangle.Height)
            End If
        Else
            If SystemClock >= ExecuteTime + ExecuteJob.TimeLength Then
                If WaitJobList.Count > 0 Then
                    LogLabel.TextBox.Text &= String.Format("系统时间：{0}  ||  {1} 执行完毕！开始执行 {2}！", SystemClock, ExecuteJob.Name, WaitJobList(NextJobSubscript).Name) & vbCrLf
                    ExecuteJob = WaitJobList(NextJobSubscript)
                    Dim ExecuteLog As ExecuteLog = New ExecuteLog(ExecuteJob.ID, ExecuteJob.Name, SystemClock, ExecuteJob.TimeLength, ExecuteJob.Color)
                    ExecuteLogs.Add(ExecuteLog)
                    WaitJobList.RemoveAt(NextJobSubscript)
                    ExecuteTime = SystemClock
                    ExecuteRectangle.Size = New Size(WaitRectangle.Width * (ExecuteJob.EndTime - ExecuteJob.StartTime) / Max_SystemTime, ExecuteRectangle.Height)
                Else
                    If AllJobList.Count = 0 Then
                        If ReplayCheckBox.Checked Then
                            SystemClock = 0
                            NextJobSubscript = -1
                            ExecuteTime = -1
                            SystemClockLabel.Text = "0"
                            DispathPanel.Image = Nothing
                            TimeLineLabel.Location = New Point(CoordinateRectangle.Left - 14, CoordinateRectangle.Top - 1)
                            ExecuteLabel.Hide()
                            WaitLabel.Hide()
                            NextJobTipLabel.Hide()
                            LogLabel.TextBox.Text = "重置系统配置！" & vbCrLf
                            CreateJobsList()
                            TimeLinePanel.Image = CreateTimeLineImage()
                            ExecuteFunction()
                        Else
                            LogLabel.TextBox.Text &= String.Format("系统时间：{0}  ||  所有作业已完成！", SystemClock) & vbCrLf
                            SystemClockTimer.Stop()
                            PlayPauseButton.Text = "播放   "
                            PlayPauseButton.Image = My.Resources.UnityResource.Play
                            MsgBox("年轻的樵夫呦！-貌似队列里所有的作业都已经执行完毕了呢！-快来重置生成新的作业队列吧！".Replace("-", vbCrLf), MsgBoxStyle.Information, "Leon：)")
                        End If
                    End If
                    ExecuteJob = Nothing
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' 返回等待队列里下次要执行的作业下标
    ''' </summary>
    Private Function GetNextJobSubscript() As Integer
        Dim JobSubscript As Integer = 0
        Select Case DispathComboBox.SelectedIndex
            Case 0
                '先来先服务-FCFS
                Return JobSubscript
            Case 1
                '短作业优先-SJF
                For Index As Integer = 1 To WaitJobList.Count - 1
                    If (WaitJobList(Index).TimeLength < WaitJobList(JobSubscript).TimeLength) Then JobSubscript = Index
                Next
                LogLabel.TextBox.Text &= String.Format("系统时间：{0}  ||  找到最短作业：{1}", SystemClock, WaitJobList(JobSubscript).Name) & vbCrLf
                Return JobSubscript
            Case 2
                '最高响应比优先-HRN（响应比 = 1+ 等待时间/执行时间）
                ResponseRatios.Clear()
                Dim Index As Integer
                For Index = 0 To WaitJobList.Count - 1
                    ResponseRatios.Add(Math.Round((SystemClock - WaitJobList(Index).StartTime + WaitJobList(Index).TimeLength) / WaitJobList(Index).TimeLength, 2))
                Next
                Index = ResponseRatios.IndexOf(ResponseRatios.Max)
                LogLabel.TextBox.Text &= String.Format("系统时间：{0}  ||  找到最高响应比作业：{1}", SystemClock, WaitJobList(Index).Name) & vbCrLf
                Return (Index)
            Case 3
                '优先数调度-HPF
                For Index As Integer = 1 To WaitJobList.Count - 1
                    If (WaitJobList(Index).Priority > WaitJobList(JobSubscript).Priority) Then JobSubscript = Index
                Next
                LogLabel.TextBox.Text &= String.Format("系统时间：{0}  ||  找到最高优先数作业：{1}", SystemClock, WaitJobList(JobSubscript).Name) & vbCrLf
                Return JobSubscript
            Case 4
                '时间片轮转 = RR
            Case 5
                '多级反馈队列
        End Select
        Return 0
    End Function

    ''' <summary>
    ''' 系统时钟值守执行
    ''' </summary>
    Private Sub ExecuteFunction()
        CheckJobArrive() '检查任务到达
        If WaitJobList.Count > 0 Then NextJobSubscript = GetNextJobSubscript()
        TimeLineLabel.Left = Math.Min(CoordinateRectangle.Right - 14, CInt(CoordinateRectangle.Left + TimeCellWidth * SystemClock - 14))
        DispathPanel.Image = CreateDispathImage() '刷新调度区域图像
        RecordPanel.Image = CreateRecordImage() '刷新图像日志记录
        CheckJobCompelet() '检查任务结束
        If WaitJobList.Count > 0 Then NextJobSubscript = GetNextJobSubscript()
    End Sub

    ''' <summary>
    ''' 绘制值守
    ''' </summary>
    Private Function CreateRecordImage() As Bitmap
        On Error Resume Next
        Dim RecordImage As Bitmap = New Bitmap(RecordPanel.Width, RecordPanel.Height)
        Dim RecordGraphics As Graphics = Graphics.FromImage(RecordImage)
        Dim InsExecuteLog As ExecuteLog
        RecordCellWidth = RecordPanel.Width / SystemClock
        'RecordGraphics.FillRectangle(Brushes.Red, New Rectangle(0, 0, RecordCellWidth, RecordCellHeight))
        For Index As Integer = 0 To ExecuteLogs.Count - 1
            InsExecuteLog = ExecuteLogs(Index)
            RecordGraphics.DrawLine(New Pen(Color.FromArgb(50, Color.White), 1), CInt(InsExecuteLog.ExecuteTime * RecordCellWidth), 0, CInt(InsExecuteLog.ExecuteTime * RecordCellWidth), RecordPanel.Height)
            RecordGraphics.DrawLine(New Pen(InsExecuteLog.Color, 2),
                CInt(InsExecuteLog.ExecuteTime * RecordCellWidth), CInt(InsExecuteLog.ID * RecordCellHeight + 4),
                CInt(InsExecuteLog.CompleteTime * RecordCellWidth), CInt(InsExecuteLog.ID * RecordCellHeight + 4))
            RecordGraphics.DrawString(InsExecuteLog.Name, Me.Font, New SolidBrush(InsExecuteLog.Color), CInt(InsExecuteLog.ExecuteTime * RecordCellWidth), CInt(InsExecuteLog.ID * RecordCellHeight - 10))
            RecordGraphics.DrawString(InsExecuteLog.ExecuteTime, Me.Font, Brushes.White, CInt(InsExecuteLog.ExecuteTime * RecordCellWidth), RecordPanel.Height - 13)
        Next
        Return RecordImage
    End Function

#End Region

#Region "纯色按钮事件"

    Private Sub SpeedUpButton_Click(sender As Object, e As EventArgs) Handles SpeedUpButton.Click
        SystemClockTimer.Interval = Math.Max(100, SystemClockTimer.Interval - 100)
    End Sub

    Private Sub SpeedDownButton_Click(sender As Object, e As EventArgs) Handles SpeedDownButton.Click
        SystemClockTimer.Interval = Math.Min(3000, SystemClockTimer.Interval + 100)
    End Sub

    Private Sub CreateJobListButton_Click(sender As Object, e As EventArgs) Handles CreateJobListButton.Click
        ResetSystem()
        CreateJobsList()
        TimeLinePanel.Image = CreateTimeLineImage()
        GC.Collect()
    End Sub

    Private Sub PlayPauseButton_Click(sender As Object, e As EventArgs) Handles PlayPauseButton.Click
        If PlayPauseButton.Text = "播放   " Then
            If (AllJobList.Count = 0 AndAlso WaitJobList.Count = 0 AndAlso IsNothing(ExecuteJob)) Then Exit Sub '所有作业完成后不会继续播放
            PlayPauseButton.Text = "停止   "
            PlayPauseButton.Image = My.Resources.UnityResource.Pause
            ExecuteFunction()
            SystemClockTimer.Start()
            If Not TimeLineLabel.Visible Then TimeLineLabel.Show()
        Else
            PlayPauseButton.Text = "播放   "
            PlayPauseButton.Image = My.Resources.UnityResource.Play
            SystemClockTimer.Stop()
        End If
    End Sub

#End Region

#Region "其他控件"

    Private Sub SystemClockTimer_Tick(sender As Object, e As EventArgs) Handles SystemClockTimer.Tick
        SystemClock += 1
        SystemClockLabel.Text = SystemClock

        ExecuteFunction()

        GC.Collect()
    End Sub

    Private Sub DispathComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DispathComboBox.SelectedIndexChanged
        If WaitJobList.Count > 0 Then NextJobSubscript = GetNextJobSubscript()
        DispathPanel.Image = CreateDispathImage()
    End Sub

#End Region

End Class
