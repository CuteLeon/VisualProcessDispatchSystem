Public Class ProcessClass
    Public ID As Integer
    Public Name As String
    Public StartTime As Integer
    Public EndTime As Integer
    Public Color As Color
    Public ImageLocation As Point
    Public TimeLength As Integer
    Public ImageInSize As Integer

    Public Sub New(ByVal pID As Integer, ByVal pName As String, ByVal pStartTime As Integer, ByVal pEndTime As Integer)
        ID = pID
        Name = pName
        StartTime = pStartTime
        EndTime = pEndTime
        TimeLength = EndTime - StartTime
        Color = Color.FromArgb(200, VBMath.Rnd * 255, VBMath.Rnd * 255, VBMath.Rnd * 255)
    End Sub

    Public Function PrintInfo() As String
        Return String.Format("进程标识：{0}-进程名称：{1}-开始时间：{2}-结束时间：{3}-进程时长：{4}-图像半径：{5}-————————".Replace("-", vbCrLf), ID, Name, StartTime, EndTime, TimeLength, ImageInSize)
    End Function

End Class
