Public Class AboutMe
    ''' <summary>
    ''' 鼠标拖动相关
    ''' </summary>
    Private Declare Function ReleaseCapture Lib "user32" () As Integer
    ''' <summary>
    ''' 鼠标拖动相关
    ''' </summary>
    Private Declare Function SendMessageA Lib "user32" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, lParam As VariantType) As Integer

#Region "标签动态效果"

    Private Sub Label_MouseEnter(sender As Label, e As EventArgs) Handles CloseButton.MouseEnter, LinkLabel.MouseEnter
        sender.ForeColor = Color.Red
    End Sub

    Private Sub Label_MouseLeave(sender As Label, e As EventArgs) Handles CloseButton.MouseLeave, LinkLabel.MouseLeave
        sender.ForeColor = Color.White
    End Sub
#End Region

#Region "窗体和控件"
    Private Sub MoveWindow(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        ' 鼠标拖动
        ReleaseCapture()
        SendMessageA(Me.Handle, &HA1, 2, 0&)
    End Sub

    Private Sub Label_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub

    Private Sub LinkLabel_Click(sender As Label, e As EventArgs) Handles LinkLabel.Click
        Process.Start(sender.Text)
    End Sub

    Private Sub AboutMe_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.UnityResource.SystemICON
    End Sub

#End Region

End Class