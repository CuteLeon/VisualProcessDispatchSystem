Public Class JobClass
    Public ID As Integer
    Public Name As String
    Public StartTime As Integer
    Public EndTime As Integer
    Public Color As Color
    Public ImageLocation As Point
    Public TimeLength As Integer

    Public Sub New(ByVal jID As Integer, ByVal jName As String, ByVal jStartTime As Integer, ByVal jEndTime As Integer)
        ID = jID
        Name = jName
        StartTime = jStartTime
        EndTime = jEndTime
        TimeLength = EndTime - StartTime
        Color = Color.FromArgb(200, VBMath.Rnd * 255, VBMath.Rnd * 255, VBMath.Rnd * 255)
    End Sub

    Public Function PrintInfo() As String
        Return String.Format("进程标识：{0}-进程名称：{1}-开始时间：{2}-结束时间：{3}-进程时长：{4}-————————".Replace("-", vbCrLf), ID, Name, StartTime, EndTime, TimeLength)
    End Function

End Class
