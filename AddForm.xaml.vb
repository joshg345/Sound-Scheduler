Public Class AddForm
    Public tab As String

    Private Sub AddButton_Click(sender As Object, e As RoutedEventArgs) Handles AddButton.Click
        If tab = "blacklist" Then
            Try
                If String.IsNullOrEmpty(Name.Text) Or String.IsNullOrEmpty(Day.Text) Or String.IsNullOrEmpty(Role.Text) Then
                    MsgBox("Please complete all the fields!")
                Else
                    Name.Text = Name.Text.ToUpper()
                    Day.Text = Day.Text.ToUpper()
                    Role.Text = Role.Text.ToUpper()
                    Dim file As System.IO.StreamWriter
                    file = New System.IO.StreamWriter(tab & ".txt", IO.FileMode.Append)
                    ' file = My.Computer.FileSystem.OpenTextFileWriter(tab & ".txt", True)
                    file.WriteLine(Name.Text & "{" & Day.Text & "}" & "{" & Role.Text & "}")
                    file.Close()
                    Hide()
                    Exit Sub
                End If
            Catch ex As Exception
                MsgBox("Error adding attendant to blacklist! Error Message: " & ex.Message)
            End Try
        End If
        Try
            If String.IsNullOrEmpty(Name.Text) Then
                MsgBox("Please enter the name of the attendant!")
            Else
                Name.Text = Name.Text.ToUpper()
                Dim file As System.IO.StreamWriter
                If tab = "attendantsa" Then
                    file = New System.IO.StreamWriter("attendantsElders" & ".txt", IO.FileMode.Append)
                ElseIf tab = "attendantsb" Then
                    file = New System.IO.StreamWriter("attendants" & ".txt", IO.FileMode.Append)
                Else
                    file = New System.IO.StreamWriter(tab & ".txt", IO.FileMode.Append)
                End If
                ' file = My.Computer.FileSystem.OpenTextFileWriter(tab & ".txt", True)
                file.WriteLine(Name.Text & "{0}")
                file.Close()
                Hide()
            End If
        Catch ex As Exception
            MsgBox("Error adding attendant! Error Message: " & ex.Message)
        End Try
    End Sub

    Private Sub Name_GotFocus(sender As Object, e As RoutedEventArgs) Handles Name.GotFocus
        If Name.Text = "Enter name..." Then
            Name.Text = ""
        End If
    End Sub
End Class
