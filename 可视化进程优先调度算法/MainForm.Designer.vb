<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.SystemClockTimer = New System.Windows.Forms.Timer(Me.components)
        Me.CreateProcessLabel = New System.Windows.Forms.Label()
        Me.PlayPauseLabel = New System.Windows.Forms.Label()
        Me.CloseLabel = New System.Windows.Forms.Label()
        Me.InfoLabel = New System.Windows.Forms.Label()
        Me.SystemClockLabel = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'SystemClockTimer
        '
        Me.SystemClockTimer.Interval = 500
        '
        'CreateProcessLabel
        '
        Me.CreateProcessLabel.AutoSize = True
        Me.CreateProcessLabel.BackColor = System.Drawing.Color.Transparent
        Me.CreateProcessLabel.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.CreateProcessLabel.ForeColor = System.Drawing.Color.BlueViolet
        Me.CreateProcessLabel.Location = New System.Drawing.Point(386, 5)
        Me.CreateProcessLabel.Name = "CreateProcessLabel"
        Me.CreateProcessLabel.Size = New System.Drawing.Size(80, 17)
        Me.CreateProcessLabel.TabIndex = 0
        Me.CreateProcessLabel.Text = "重新生成进程"
        Me.CreateProcessLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PlayPauseLabel
        '
        Me.PlayPauseLabel.AutoSize = True
        Me.PlayPauseLabel.BackColor = System.Drawing.Color.Transparent
        Me.PlayPauseLabel.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.PlayPauseLabel.ForeColor = System.Drawing.Color.BlueViolet
        Me.PlayPauseLabel.Location = New System.Drawing.Point(324, 5)
        Me.PlayPauseLabel.Name = "PlayPauseLabel"
        Me.PlayPauseLabel.Size = New System.Drawing.Size(56, 17)
        Me.PlayPauseLabel.TabIndex = 1
        Me.PlayPauseLabel.Text = "开始播放"
        Me.PlayPauseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CloseLabel
        '
        Me.CloseLabel.AutoSize = True
        Me.CloseLabel.BackColor = System.Drawing.Color.Transparent
        Me.CloseLabel.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.CloseLabel.ForeColor = System.Drawing.Color.BlueViolet
        Me.CloseLabel.Location = New System.Drawing.Point(472, 5)
        Me.CloseLabel.Name = "CloseLabel"
        Me.CloseLabel.Size = New System.Drawing.Size(32, 17)
        Me.CloseLabel.TabIndex = 2
        Me.CloseLabel.Text = "关闭"
        Me.CloseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'InfoLabel
        '
        Me.InfoLabel.AutoSize = True
        Me.InfoLabel.BackColor = System.Drawing.Color.Transparent
        Me.InfoLabel.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.InfoLabel.ForeColor = System.Drawing.Color.DodgerBlue
        Me.InfoLabel.Location = New System.Drawing.Point(16, 143)
        Me.InfoLabel.Name = "InfoLabel"
        Me.InfoLabel.Size = New System.Drawing.Size(163, 20)
        Me.InfoLabel.TabIndex = 3
        Me.InfoLabel.Text = "已创建随机的进程列表！"
        '
        'SystemClockLabel
        '
        Me.SystemClockLabel.AutoSize = True
        Me.SystemClockLabel.BackColor = System.Drawing.Color.Transparent
        Me.SystemClockLabel.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.SystemClockLabel.ForeColor = System.Drawing.Color.Red
        Me.SystemClockLabel.Location = New System.Drawing.Point(206, 3)
        Me.SystemClockLabel.Name = "SystemClockLabel"
        Me.SystemClockLabel.Size = New System.Drawing.Size(87, 20)
        Me.SystemClockLabel.TabIndex = 4
        Me.SystemClockLabel.Text = "系统时间：0"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.OldLace
        Me.BackgroundImage = Global.可视化进程优先调度算法.My.Resources.UnityResource.FormBGI
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(540, 320)
        Me.Controls.Add(Me.SystemClockLabel)
        Me.Controls.Add(Me.InfoLabel)
        Me.Controls.Add(Me.CloseLabel)
        Me.Controls.Add(Me.PlayPauseLabel)
        Me.Controls.Add(Me.CreateProcessLabel)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "可视化进程优先调度算法"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SystemClockTimer As System.Windows.Forms.Timer
    Friend WithEvents CreateProcessLabel As Label
    Friend WithEvents PlayPauseLabel As Label
    Friend WithEvents CloseLabel As Label
    Friend WithEvents InfoLabel As Label
    Friend WithEvents SystemClockLabel As Label
End Class
