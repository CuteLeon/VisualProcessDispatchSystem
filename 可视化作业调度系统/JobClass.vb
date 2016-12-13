Public Class JobClass
    ''' <summary>
    ''' 作业ID
    ''' </summary>
    Public ID As Integer
    ''' <summary>
    ''' 作业名称
    ''' </summary>
    Public Name As String
    ''' <summary>
    ''' 作业到达时间
    ''' </summary>
    Public StartTime As Integer
    ''' <summary>
    ''' 作业结束时间
    ''' </summary>
    Public EndTime As Integer
    ''' <summary>
    ''' 作业执行时间长度
    ''' </summary>
    Public TimeLength As Integer
    ''' <summary>
    ''' 作业对应线条颜色
    ''' </summary>
    Public Color As Color
    ''' <summary>
    ''' 作业优先级
    ''' </summary>
    Public Priority As Integer
    ''' <summary>
    ''' 作业已经执行的时间片
    ''' </summary>
    Public TimeSlice As Integer

    Public Sub New(ByVal jID As Integer, ByVal jName As String, ByVal jStartTime As Integer, ByVal jEndTime As Integer, ByVal jColor As Color, ByVal jPriority As Integer)
        ID = jID
        Name = jName
        StartTime = jStartTime
        EndTime = jEndTime
        TimeLength = EndTime - StartTime
        Color = jColor
        Priority = jPriority
    End Sub
End Class
