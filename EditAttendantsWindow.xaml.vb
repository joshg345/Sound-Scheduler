Imports System.Collections.ObjectModel
Imports System.IO

Public Class EditAttendantsWindow
    Dim attendants As New List(Of String)
    Dim attendantscnt As New List(Of Integer)
    Dim microphones As New List(Of String)
    Dim microphonescnt As New List(Of Integer)
    Dim sound As New List(Of String)
    Dim soundcnt As New List(Of Integer)
    Dim platform As New List(Of String)
    Dim platformcnt As New List(Of Integer)
    Dim platformSun As New List(Of String)
    Dim platformSuncnt As New List(Of Integer)
    Dim attendantsElders As New List(Of String)
    Dim attendantsElderscnt As New List(Of Integer)
    Dim blacklist As New List(Of String)
    Dim blacklistday As New List(Of String)
    Dim blacklistrole As New List(Of String)
    Dim soundList As ObservableCollection(Of ListViewItemTemplate)
    Dim microphonesList As ObservableCollection(Of ListViewItemTemplate)
    Dim platformList As ObservableCollection(Of ListViewItemTemplate)
    Dim platformSunList As ObservableCollection(Of ListViewItemTemplate)
    Dim attendantsList As ObservableCollection(Of ListViewItemTemplate)
    Dim attendantsEldersList As ObservableCollection(Of ListViewItemTemplate)
    Dim blacklistList As ObservableCollection(Of BlacklistListViewTemplate)

    Private Sub EditAttendantsWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        PopulateListViews()
    End Sub

    Public Sub ReadFileBlacklist(list As List(Of String), listcnt As List(Of String), Optional listrole As List(Of String) = Nothing)
        list.Clear()
        listcnt.Clear()
        If listrole IsNot Nothing Then
            listrole.Clear()
        End If
        Dim data() As String
        'Create List
        Dim line As String

        ' Create new StreamReader instance with Using block.
        Using reader As StreamReader = New StreamReader("blacklist.txt")
            ' Read one line from file
            line = reader.ReadLine

            Do While (Not line Is Nothing)
                data = line.Split("{")
                list.Add(data(0))
                Dim data2 = data(1).Substring(0, data(1).IndexOf("}"))
                listcnt.Add(data2)
                If listrole IsNot Nothing Then
                    Dim data3 = data(2).Substring(0, data(2).IndexOf("}"))
                    listrole.Add(data3)
                End If
                line = reader.ReadLine
            Loop

        End Using

    End Sub

    Public Sub ReadFile(file As String, list As List(Of String), listcnt As List(Of Integer), Optional listrole As List(Of String) = Nothing)
        list.Clear()
        listcnt.Clear()
        If listrole IsNot Nothing Then
            listrole.Clear()
        End If
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
                If listrole IsNot Nothing Then
                    Dim data3 = data(2).Substring(0, data(2).IndexOf("}"))
                    listrole.Add(data3)
                End If
                line = reader.ReadLine
            Loop

        End Using

    End Sub

    Public Sub PopulateListViews()
        Try
            ReadFile("sound", sound, soundcnt,)
            'soundList = New ObservableCollection(Of ListViewItemTemplate)()
            soundList = New ObservableCollection(Of ListViewItemTemplate)
            For i As Integer = 0 To sound.Count - 1
                soundList.Add(New ListViewItemTemplate(sound(i), soundcnt(i)))
            Next
            SoundListView.ItemsSource = soundList
        Catch ex As Exception
            MsgBox("Error populating sound list view. Exception Message: " & ex.Message)
        End Try

        Try
            ReadFile("microphones", microphones, microphonescnt,)
            microphonesList = New ObservableCollection(Of ListViewItemTemplate)
            For i As Integer = 0 To microphones.Count - 1
                microphonesList.Add(New ListViewItemTemplate(microphones(i), microphonescnt(i)))
            Next
            MicrophonesListView.ItemsSource = microphonesList
        Catch ex As Exception
            MsgBox("Error populating microphones list view. Exception Message: " & ex.Message)
        End Try

        Try
            ReadFile("platform", platform, platformcnt,)
            platformList = New ObservableCollection(Of ListViewItemTemplate)
            For i As Integer = 0 To platform.Count - 1
                platformList.Add(New ListViewItemTemplate(platform(i), platformcnt(i)))
            Next
            PlatformListView.ItemsSource = platformList
        Catch ex As Exception
            MsgBox("Error populating platform list view. Exception Message: " & ex.Message)
        End Try

        Try
            ReadFile("platformsun", platformSun, platformSuncnt,)
            platformSunList = New ObservableCollection(Of ListViewItemTemplate)
            For i As Integer = 0 To platformSun.Count - 1
                platformSunList.Add(New ListViewItemTemplate(platformSun(i), platformSuncnt(i)))
            Next
            PlatformSunListView.ItemsSource = platformSunList
        Catch ex As Exception
            MsgBox("Error populating platformsun list view. Exception Message: " & ex.Message)
        End Try

        Try
            ReadFile("attendants", attendants, attendantscnt,)
            attendantsList = New ObservableCollection(Of ListViewItemTemplate)
            For i As Integer = 0 To attendants.Count - 1
                attendantsList.Add(New ListViewItemTemplate(attendants(i), attendantscnt(i)))
            Next
            AttendantsListView.ItemsSource = attendantsList
        Catch ex As Exception
            MsgBox("Error populating attendants list view. Exception Message: " & ex.Message)
        End Try

        Try
            ReadFile("attendantselders", attendantsElders, attendantsElderscnt,)
            attendantsEldersList = New ObservableCollection(Of ListViewItemTemplate)
            For i As Integer = 0 To attendantsElders.Count - 1
                attendantsEldersList.Add(New ListViewItemTemplate(attendantsElders(i), attendantsElderscnt(i)))
            Next
            AttendantsEldersListView.ItemsSource = attendantsEldersList
        Catch ex As Exception
            MsgBox("Error populating attendantselders list view. Exception Message: " & ex.Message)
        End Try

        Try
            ReadFileBlacklist(blacklist, blacklistday, blacklistrole)
            blacklistList = New ObservableCollection(Of BlacklistListViewTemplate)
            For i As Integer = 0 To blacklist.Count - 1
                blacklistList.Add(New BlacklistListViewTemplate(blacklist(i), blacklistday(i), blacklistrole(i)))
            Next
            BlacklistListView.ItemsSource = blacklistList
        Catch ex As Exception
            MsgBox("Error populating blacklist list view. Exception Message: " & ex.Message)
        End Try

    End Sub

    Private Sub AddButton_Click(sender As Object, e As RoutedEventArgs) Handles AddButton.Click
        Dim form As New AddForm
        Dim tab As TabItem = TryCast(FileTabControl.SelectedItem, TabItem)
        Dim tabname As String = tab.Header
        'Dim listview As ListView = tab.Content
        tabname = tabname.Replace(" ", "")
        form.tab = tabname.ToLower()
        If tabname <> "Blacklist" Then
            form.Day.Visibility = Visibility.Hidden
            form.Role.Visibility = Visibility.Hidden
        Else
            form.Day.Visibility = Visibility.Visible
            form.Role.Visibility = Visibility.Visible
        End If
        form.ShowDialog()
        If tabname <> "Blacklist" Then
            ResetCount(tabname)
        End If
        ClearListViews()
        PopulateListViews()
    End Sub

    Public Sub ClearListViews()

        For Each item In soundList.ToArray
            soundList.Remove(item)
        Next
        For Each item In microphonesList.ToArray
            microphonesList.Remove(item)
        Next
        For Each item In platformList.ToArray
            platformList.Remove(item)
        Next
        For Each item In platformSunList.ToArray
            platformSunList.Remove(item)
        Next
        For Each item In attendantsList.ToArray
            attendantsList.Remove(item)
        Next
        For Each item In attendantsEldersList.ToArray
            attendantsEldersList.Remove(item)
        Next
        For Each item In blacklist.ToArray
            blacklist.Remove(item)
        Next

    End Sub

    Private Sub RemoveButton_Click(sender As Object, e As RoutedEventArgs) Handles RemoveButton.Click
        Dim tab As TabItem = TryCast(FileTabControl.SelectedItem, TabItem)
        Dim tabname As String = tab.Header
        Dim listview As ListView = tab.Content
        If listview.SelectedItem Is Nothing Then
            MsgBox("Please select an attendant to be removed!")
        Else
            Dim filename As String
            If tabname = "Attendants B" Then
                filename = "attendants.txt"
            ElseIf tabname = "Attendants A" Then
                filename = "attendantsElders.txt"
            Else
                filename = tabname.ToLower() & ".txt"
            End If
            filename = filename.Replace(" ", "")
            For Each item In listview.SelectedItems
                If filename = "blacklist.txt" Then
                    File.WriteAllLines(filename, File.ReadAllLines(filename).Where(Function(l) l <> item.Name & "{" & item.Day & "}" & "{" & item.AttendantsRole & "}"))
                Else
                    File.WriteAllLines(filename, File.ReadAllLines(filename).Where(Function(l) l <> item.Name & "{" & item.Count & "}"))
                End If
            Next
            ClearListViews()
            PopulateListViews()
        End If
    End Sub

    Public Sub ResetCount(ByRef fileinput As String)
        If fileinput = "attendantsa" Or fileinput = "AttendantsA" Then
            fileinput = "attendantselders"
        ElseIf fileinput = "attendantsb" Or fileinput = "AttendantsB" Then
            fileinput = "attendants"
        End If

        Dim data() As String = File.ReadAllLines(fileinput & ".txt")
        For Index As Integer = 0 To data.Count - 1
            Dim i As Integer = data(Index).IndexOf("{")
            Dim f As String = data(Index).Substring(i + 1, data(Index).IndexOf("}", i + 1) - i - 1)
            Dim newChar As Integer = 0
            data(Index) = data(Index).Replace(f, newChar)
            IO.File.WriteAllLines(fileinput & ".txt", data)
        Next
        RandomizeArray(data)
    End Sub

    Private Sub ResetCountButton_Click(sender As Object, e As RoutedEventArgs) Handles ResetCountButton.Click
        Dim tab As TabItem = TryCast(FileTabControl.SelectedItem, TabItem)
        Dim tabname As String = tab.Header
        Dim result = MsgBox("You're about to reset the count for " & tabname & ". Would you like to continue?", MsgBoxStyle.YesNo)
        If result = MsgBoxResult.Yes Then
            tabname = tabname.ToLower().Replace(" ", "")
            ResetCount(tabname)
            ClearListViews()
            PopulateListViews()
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

    Private Sub BlacklistTab_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles BlacklistTab.PreviewMouseDown
        ResetCountButton.Visibility = Visibility.Hidden
    End Sub

    Private Sub PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles AttendantsEldersTab.PreviewMouseDown, PlatformTab.PreviewMouseDown, SoundTab.PreviewMouseDown, PlatformSunTab.PreviewMouseDown, MicrophonesTab.PreviewMouseDown, AttendantsTab.PreviewMouseDown
        ResetCountButton.Visibility = Visibility.Visible
    End Sub

End Class
