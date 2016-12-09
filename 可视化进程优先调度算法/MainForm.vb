Public Class MainForm
    Private Declare Function ReleaseCapture Lib "user32" () As Integer
    Private Declare Function SendMessageA Lib "user32" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, lParam As VariantType) As Integer
    Private Const PaddingSize As Integer = 15 '元素之间间隔像素
    Private Const ProcessCount As Integer = 9 '进程总数
    Private Const Max_SystemTime As Integer = 60 '系统最大执行时间
    Dim FormSize As Size = New Size(540, 320)
    Dim TimeLineRectangle As Rectangle = New Rectangle(15, 28, 510, 108) '显示时间线的区域
    Dim CoordinateRectangle As Rectangle = New Rectangle(15, 3, 480, 90) '时间线坐标系在TimeLineRectangle内的区域
    Dim AllProcessList As New List(Of ProcessClass) '当前时刻后的所有的进程列表
    Dim WaittingProcessList As New List(Of ProcessClass) '等待运行的进程列表
    Dim ExecuteProcess As ProcessClass '正在运行的进程
    Dim DispathRectangle As Rectangle = New Rectangle(20, 170, 500, 140) '调度区域
    Dim ExecuteRectangle As Rectangle = New Rectangle(20, 175, 500, 15)
    Dim WaitRectangle As Rectangle = New Rectangle(150, 192, 370, 120)
    Dim TimeLineBitmap As Bitmap '记录时间线图像
    Dim SystemClock As Integer = -1 '系统时间
    Dim ShortestPID As Integer '当前等待队列里最短进程（即下次运行的进程）
    Dim ExecuteTime As Integer '当前执行的进程开始执行的时间

#Region "窗体事件"

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Reset()
        CreateProcessList()
    End Sub

    Private Sub MainForm_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown, SystemClockLabel.MouseDown
        If e.Y > TimeLineRectangle.Top Then Exit Sub
        ReleaseCapture()
        SendMessageA(Me.Handle, &HA1, 2, 0&)
    End Sub

    Private Sub MainForm_Click(sender As Object, e As MouseEventArgs) 'Handles Me.MouseClick
        '无法实时更新两个队列的元素，所以暂不启用
        Dim TimeLineLocation As Point = e.Location
        If TimeLineRectangle.Contains(e.Location) Then
            TimeLineLocation.Offset(-CoordinateRectangle.Left - PaddingSize, -TimeLineRectangle.Top)
            SystemClock = TimeLineLocation.X / 8
            SystemClockLabel.Text = "系统时间：" & SystemClock
            DrawUI()
        End If
    End Sub

#End Region

#Region "功能函数"

    Private Sub DrawUI()
        Dim UnityBitmap As Bitmap = New Bitmap(My.Resources.UnityResource.FormBGI, FormSize)
        Dim UnityGraphics As Graphics = Graphics.FromImage(UnityBitmap)
        CreateProcessLabel.DrawToBitmap(UnityBitmap, New Rectangle(CreateProcessLabel.Location, CreateProcessLabel.Size))
        CloseLabel.DrawToBitmap(UnityBitmap, New Rectangle(CloseLabel.Location, CloseLabel.Size))
        PlayPauseLabel.DrawToBitmap(UnityBitmap, New Rectangle(PlayPauseLabel.Location, PlayPauseLabel.Size))
        InfoLabel.DrawToBitmap(UnityBitmap, New Rectangle(InfoLabel.Location, InfoLabel.Size))
        SystemClockLabel.DrawToBitmap(UnityBitmap, New Rectangle(SystemClockLabel.Location, SystemClockLabel.Size))
        UnityGraphics.DrawImage(TimeLineBitmap, TimeLineRectangle.Location)
        If SystemClock > -1 Then UnityGraphics.DrawImage(My.Resources.UnityResource.TimeLine, TimeLineRectangle.Left + CoordinateRectangle.Left - 15 + IIf(SystemClock > 60, 60, SystemClock) * 8, TimeLineRectangle.Top)
        UnityGraphics.DrawString("进程调度算法-短进程优先", Me.Font, Brushes.DarkGoldenrod, 30, 10)
        UnityGraphics.DrawString("正在等待的进程队列：", Me.Font, Brushes.DodgerBlue, WaitRectangle.Left - 120, WaitRectangle.Top)
        If Not (ExecuteProcess Is Nothing) Then
            If ExecuteProcessInWait() Then
                InfoLabel.Text = "开始执行： " & ExecuteProcess.Name
                UnityGraphics.DrawString("正在执行：" & ExecuteProcess.Name, Me.Font, Brushes.Black, 201, DispathRectangle.Top - 9)
                UnityGraphics.DrawString("正在执行：" & ExecuteProcess.Name, Me.Font, New SolidBrush(ExecuteProcess.Color), 200, DispathRectangle.Top - 10)
                ExecuteRectangle.Location = New Point(WaitRectangle.Left - WaitRectangle.Width * (SystemClock - ExecuteTime) / 60, ExecuteRectangle.Top)
                UnityGraphics.FillRectangle(New SolidBrush(ExecuteProcess.Color), ExecuteRectangle)
                UnityGraphics.DrawRectangle(Pens.Red, ExecuteRectangle)
            End If
        End If
        For Index As Integer = 0 To WaittingProcessList.Count - 1
            UnityGraphics.FillRectangle(New SolidBrush(WaittingProcessList(Index).Color), New Rectangle(WaitRectangle.Left, WaitRectangle.Top + Index * 15, WaitRectangle.Width * WaittingProcessList(Index).TimeLength / 60, 15))
            UnityGraphics.DrawString(WaittingProcessList(Index).Name & IIf(ShortestPID = Index, " (当前等待队列里最短进程)", vbNullString), Me.Font, Brushes.Black, WaitRectangle.Left + 1, WaitRectangle.Top + Index * 15 + 1)
            UnityGraphics.DrawString(WaittingProcessList(Index).Name & IIf(ShortestPID = Index, " (当前等待队列里最短进程)", vbNullString), Me.Font, Brushes.Aqua, WaitRectangle.Left, WaitRectangle.Top + Index * 15)
        Next
        UnityGraphics.DrawLine(Pens.Red, WaitRectangle.Left, ExecuteRectangle.Top, WaitRectangle.Left, WaitRectangle.Bottom)
        DrawImage(Me, UnityBitmap)
    End Sub

    Private Function DrawTimeLine()
        Dim Index As Integer
        Dim TimeLineBitmap As Bitmap = New Bitmap(TimeLineRectangle.Width, TimeLineRectangle.Height)
        Dim TimeLineGraphics As Graphics = Graphics.FromImage(TimeLineBitmap)
        Dim TempPen As Pen = New Pen(Color.FromArgb(150, Color.MediumSpringGreen), 2)
        TimeLineGraphics.DrawLine(TempPen, CoordinateRectangle.Left, CoordinateRectangle.Top, CoordinateRectangle.Left, CoordinateRectangle.Bottom)
        TimeLineGraphics.DrawLine(TempPen, CoordinateRectangle.Left, CoordinateRectangle.Bottom, CoordinateRectangle.Right, CoordinateRectangle.Bottom)
        TempPen = New Pen(Color.FromArgb(100, Color.White), 1)
        For Index = 0 To 60 Step 5
            TimeLineGraphics.DrawString(Index, Me.Font, Brushes.Yellow, PaddingSize + 8 * Index - 4, CoordinateRectangle.Bottom + 2)
            TimeLineGraphics.DrawLine(TempPen, CoordinateRectangle.Left + 8 * Index - 1, CoordinateRectangle.Top, CoordinateRectangle.Left + 8 * Index - 1, CoordinateRectangle.Bottom)
        Next
        For Index = 0 To AllProcessList.Count - 1
            TimeLineGraphics.DrawString("PID-" & Index, Me.Font, New SolidBrush(AllProcessList(Index).Color), CInt(CoordinateRectangle.Left + CoordinateRectangle.Width * (AllProcessList(Index).StartTime / Max_SystemTime)), CoordinateRectangle.Top + Index * 10 + 5)
            TimeLineGraphics.FillEllipse(New SolidBrush(AllProcessList(Index).Color), CInt(CoordinateRectangle.Left + CoordinateRectangle.Width * (AllProcessList(Index).StartTime / Max_SystemTime)) - 4, CoordinateRectangle.Top + Index * 10 + 2, 5, 5)
            TimeLineGraphics.DrawLine(New Pen(AllProcessList(Index).Color, 2), CInt(CoordinateRectangle.Left + CoordinateRectangle.Width * (AllProcessList(Index).StartTime / Max_SystemTime)),
                                      CoordinateRectangle.Top + Index * 10 + 5, CInt(CoordinateRectangle.Left + CoordinateRectangle.Width * (AllProcessList(Index).EndTime / Max_SystemTime)), CoordinateRectangle.Top + Index * 10 + 5)
        Next
        Return TimeLineBitmap
    End Function

    Protected Overloads Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            If Not DesignMode Then
                Dim cp As CreateParams = MyBase.CreateParams
                cp.ExStyle = cp.ExStyle Or WS_EX_LAYERED
                Return cp
            Else
                Return MyBase.CreateParams
            End If
        End Get
    End Property

    Private Sub Reset()
        SystemClock = 0
        ShortestPID = -1
        ExecuteTime = -1
        ExecuteProcess = Nothing
        SystemClockLabel.Text = "系统时间：0"
    End Sub

    Private Sub CreateProcessList()
        '刷新进程列表
        AllProcessList.Clear()
        WaittingProcessList.Clear()
        AllProcessList = New List(Of ProcessClass)
        WaittingProcessList = New List(Of ProcessClass)
        For Index As Integer = 0 To ProcessCount - 1
            Dim ProcessStartTime As Integer = VBMath.Rnd * (Max_SystemTime - 1)
            Dim ProcessEndTime As Integer = ProcessStartTime + 1 + (Max_SystemTime - ProcessStartTime - 1) * VBMath.Rnd
            Dim ProcessDemo As ProcessClass = New ProcessClass(Index, "进程-" & Index, ProcessStartTime, ProcessEndTime)
            AllProcessList.Add(ProcessDemo)
        Next
        TimeLineBitmap = DrawTimeLine()
        InfoLabel.Text = "已创建随机的进程列表！"
        DrawUI()
        GC.Collect()
    End Sub

    Private Sub WaitProcessInAll()
        '在所有进程列表里找到达的进程
        Dim PIDs As String = vbNullString
        Dim Index As Integer = 0, ListCount As Integer = AllProcessList.Count
        Do While Index < ListCount
            If AllProcessList(Index).StartTime = SystemClock Then
                PIDs &= AllProcessList(Index).ID & " & "
                If ExecuteProcess Is Nothing Then
                    ExecuteTime = SystemClock
                    ExecuteProcess = AllProcessList(Index)
                    ExecuteRectangle.Size = New Size(WaitRectangle.Width * ExecuteProcess.TimeLength / 60, ExecuteRectangle.Height)
                    ShortestPID = -1
                Else
                    WaittingProcessList.Add(AllProcessList(Index))
                    ShortestPID = GetShortestProcess()
                End If
                AllProcessList.RemoveAt(Index)
                ListCount -= 1
            Else
                Index += 1
            End If
        Loop
        If PIDs IsNot Nothing Then
            InfoLabel.Text = "进程 " + PIDs.Remove(PIDs.Length - 3) + " 加入等待执行队列！"
        End If
    End Sub

    Private Function ExecuteProcessInWait() As Boolean
        '从等待队列寻找需要被执行的进程
        If SystemClock >= ExecuteTime + ExecuteProcess.TimeLength Then
            If WaittingProcessList.Count > 0 Then
                ExecuteProcess = WaittingProcessList(ShortestPID)
                WaittingProcessList.RemoveAt(ShortestPID)
                ShortestPID = GetShortestProcess()
                ExecuteTime = SystemClock
                ExecuteRectangle.Size = New Size(WaitRectangle.Width * (ExecuteProcess.EndTime - ExecuteProcess.StartTime) / 60, ExecuteRectangle.Height)
            Else
                If AllProcessList.Count = 0 Then
                    InfoLabel.Text = "所有进程已经完成，正在生成新的进程队列！"
                    Reset()
                    CreateProcessList()
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Private Function GetShortestProcess() As Integer
        ' 从等待进程列表里找到最短时间的进程ID
        Dim PID As Integer = 0
        For Index As Integer = 1 To WaittingProcessList.Count - 1
            If (WaittingProcessList(Index).TimeLength < WaittingProcessList(PID).TimeLength) Then
                PID = Index
            End If
        Next
        Return PID
    End Function
#End Region

#Region "控件事件"

    Private Sub CreateProcessLabel_Click(sender As Object, e As EventArgs) Handles CreateProcessLabel.Click
        Reset()
        CreateProcessList()
    End Sub

    Private Sub CloseLabel_Click(sender As Object, e As EventArgs) Handles CloseLabel.Click
        Me.Close()
    End Sub

    Private Sub Label_MouseEnter(sender As Object, e As EventArgs) Handles CreateProcessLabel.MouseEnter, CloseLabel.MouseEnter, PlayPauseLabel.MouseEnter
        CType(sender, Label).ForeColor = Color.Red
        DrawUI()
    End Sub

    Private Sub Label_MouseLeave(sender As Object, e As EventArgs) Handles CreateProcessLabel.MouseLeave, CloseLabel.MouseLeave, PlayPauseLabel.MouseLeave
        CType(sender, Label).ForeColor = Color.BlueViolet
        DrawUI()
    End Sub

    Private Sub PlayPauseLabel_Click(sender As Object, e As EventArgs) Handles PlayPauseLabel.Click
        SystemClockTimer.Enabled = Not SystemClockTimer.Enabled
        PlayPauseLabel.Text = IIf(SystemClockTimer.Enabled, "暂停"， "开始播放")
        DrawUI()
    End Sub

    Private Sub SystemClock_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SystemClockTimer.Tick
        SystemClock += 1
        SystemClockLabel.Text = "系统时间：" & SystemClock
        WaitProcessInAll()
        DrawUI()
        GC.Collect()
    End Sub
#End Region

End Class
