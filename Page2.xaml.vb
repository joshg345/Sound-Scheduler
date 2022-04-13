Imports System.IO
Imports System.Text.RegularExpressions

Class Page2

    Public Function GetLabels(Name As String)
        Dim labelList As New List(Of Label)
        For Each Label As Label In Page1Grid.Children.OfType(Of Label)
            If Label.Name.Contains(Name) Then
                labelList.Add(Label)
            End If
        Next
        Return labelList
    End Function

    Public Sub UpdateLabels(Name As String, list As List(Of String))
        'Dim labelList As New List(Of Label)
        Dim i As Integer = 0
        For Each Label As Label In Page1Grid.Children.OfType(Of Label)
            If Label.Name.Contains(Name) Then
                If Label.IsVisible Then
                    Label.Content = list(i)
                    If i <> list.Count - 1 Then
                        i = i + 1
                    End If
                End If
            End If
        Next

    End Sub

    Public Sub HideLabel(Name As String, number As String)
        For Each Label As Label In Page1Grid.Children.OfType(Of Label)
            If Label.Name.Contains(Name & number) Then
                Label.Visibility = Visibility.Hidden
            End If
        Next
    End Sub

    Public Sub ResetVisiblity()
        For Each Label As Label In Page1Grid.Children.OfType(Of Label)
            Label.Visibility = Visibility.Visible
        Next
    End Sub

    Private Sub Label_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Date1.MouseDown, Date2.MouseDown, Date3.MouseDown, Date4.MouseDown, Date5.MouseDown, Date6.MouseDown, Date7.MouseDown, Sound1.MouseDown, Sound2.MouseDown, Sound3.MouseDown, Sound4.MouseDown, Sound5.MouseDown, Sound6.MouseDown, Sound7.MouseDown, AVattendant1.MouseDown, AVattendant2.MouseDown, AVattendant3.MouseDown, AVattendant4.MouseDown, AVattendant5.MouseDown, AVattendant6.MouseDown, AVattendant7.MouseDown, AVoperator1.MouseDown, AVoperator2.MouseDown, AVoperator3.MouseDown, AVoperator4.MouseDown, AVoperator5.MouseDown, AVoperator6.MouseDown, AVoperator7.MouseDown, Microphones1.MouseDown, Microphones2.MouseDown, Microphones3.MouseDown, Microphones4.MouseDown, Microphones5.MouseDown, Microphones6.MouseDown, Microphones7.MouseDown, Microphones8.MouseDown, Microphones9.MouseDown, Microphones10.MouseDown, Microphones11.MouseDown, Microphones12.MouseDown, Microphones13.MouseDown, Microphones14.MouseDown, Platform1.MouseDown, Platform2.MouseDown, Platform3.MouseDown, Platform4.MouseDown, Platform5.MouseDown, Platform6.MouseDown, Platform7.MouseDown, Platform8.MouseDown, Platform9.MouseDown, Platform10.MouseDown, Platform11.MouseDown, Platform12.MouseDown, Platform13.MouseDown, Platform14.MouseDown, Attendants1.MouseDown, Attendants2.MouseDown, Attendants3.MouseDown, Attendants4.MouseDown, Attendants5.MouseDown, Attendants6.MouseDown, Attendants7.MouseDown, Attendants8.MouseDown, Attendants9.MouseDown, Attendants10.MouseDown, Attendants11.MouseDown, Attendants12.MouseDown, Attendants13.MouseDown, Attendants14.MouseDown, CarPark1.MouseDown, CarPark2.MouseDown, CarPark3.MouseDown, CarPark4.MouseDown, CarPark5.MouseDown, CarPark6.MouseDown, CarPark7.MouseDown
        Dim fileContents As New List(Of String)
        Dim fileContentscnt As New List(Of Integer)
        Dim senderLabel = DirectCast(sender, Label)
        Dim form As New Window1
        'Dim DateLabel As String = "Date" & senderLabel.Name.Substring(senderLabel.Name.Length - 1, 1)
        Dim labelName As String = Regex.Replace(senderLabel.Name, "\d+", "")
        Dim labelNumber As Integer = Num(senderLabel.Name)
        If labelName = "Date" Then
            form.LabelTB2.Visibility = Visibility.Visible
            form.LabelTB.Visibility = Visibility.Hidden
            form.LabelTB2.Text = senderLabel.Content
            form.ShowDialog()
            If form.Updated = True Then
                senderLabel.Content = form.LabelTB2.Text
            End If
        Else
            form.LabelTB2.Visibility = Visibility.Hidden
            form.LabelTB.Visibility = Visibility.Visible
            If labelName = "Attendants" Then
                If labelNumber Mod 2 > 0 Then
                    labelName = "attendantselders"
                End If
            ElseIf labelName = "CarPark" Then
                labelName = "attendants"
            Else
                For Each Label As Label In Page1Grid.Children.OfType(Of Label)
                    If Label.Name = labelName & labelNumber + 1 Then
                        If Not Label.IsVisible Then
                            labelName = labelName & "sun"
                        End If
                    End If

                Next
            End If

            labelName.ToLower()
            ReadFile(labelName, fileContents, fileContentscnt)

            For Each attendant In fileContents
                form.LabelTB.Items.Add(attendant)
            Next

            form.LabelTB.Text = senderLabel.Content
            form.ShowDialog()

            If form.Updated = True Then
                senderLabel.Content = form.LabelTB.Text
                Dim lines() As String = IO.File.ReadAllLines(labelName & ".txt")
                For i As Integer = 0 To lines.Length - 1
                    If lines(i).Contains(form.LabelTB.Text) Then
                        Dim index As Integer = lines(i).IndexOf("{")
                        Dim f As String = lines(i).Substring(index + 1, lines(i).IndexOf("}", index + 1) - index - 1)
                        Dim newChar As Integer = f + 1
                        lines(i) = lines(i).Replace(f, newChar)
                    End If
                Next
                RandomizeArray(lines)
                IO.File.WriteAllLines(labelName & ".txt", lines)
            End If
        End If

    End Sub

    Private Sub RandomizeArray(ByVal items() As String)
        Dim max_index As Integer = items.Length - 1
        Dim rnd As New Random
        For i As Integer = 0 To max_index - 1
            ' Pick an item for position i.
            Dim j As Integer = rnd.Next(i, max_index + 1)

            ' Swap them.
            Dim temp As String = items(i)
            items(i) = items(j)
            items(j) = temp
        Next i
    End Sub
    'Returns numbers from string
    Private Shared Function Num(ByVal value As String) As Integer
        Dim returnVal As String = String.Empty
        Dim collection As MatchCollection = Regex.Matches(value, "\d+")
        For Each m As Match In collection
            returnVal += m.ToString()
        Next
        Return Convert.ToInt32(returnVal)
    End Function

    Public Sub ReadFile(file As String, list As List(Of String), listcnt As List(Of Integer))
        Try
            Dim data() As String
            'Create List
            Dim line As String

            ' Create new StreamReader instance with Using block.
            Using reader As StreamReader = New StreamReader(file & ".txt")
                ' Read one line from file
                line = reader.ReadLine

                Do While (Not line Is Nothing)
                    data = line.Split("{")
                    list.Add(data(0))
                    Dim data2 = data(1).Substring(0, data(1).IndexOf("}"))
                    listcnt.Add(data2)
                    line = reader.ReadLine
                Loop

            End Using
        Catch ex As Exception
            MsgBox("Error reading " & file & " file! Error message: " & ex.Message)
        End Try

    End Sub
End Class
