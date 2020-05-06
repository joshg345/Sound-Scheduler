Public Class ListViewItemTemplate
    Private v1 As String
    Private v2 As Integer

    Public Sub New(v1 As String, v2 As Integer)
        Count = v2
        Name = v1
    End Sub

    Public Property Name As String
    Public Property Count As String
End Class
