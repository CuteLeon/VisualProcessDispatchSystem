Public Class JobClass
    Public ID As Integer
    Public Name As String
    Public StartTime As Integer
    Public EndTime As Integer
    Public TimeLength As Integer
    Public Color As Color
    Public Sub New(ByVal jID As Integer, ByVal jName As String, ByVal jStartTime As Integer, ByVal jEndTime As Integer, ByVal jColor As Color)
        ID = jID
        Name = jName
        StartTime = jStartTime
        EndTime = jEndTime
        TimeLength = EndTime - StartTime
        Color = jColor
    End Sub
End Class
