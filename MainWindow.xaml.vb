Imports System.IO
Imports System.IO.Packaging
Imports System.Printing
Imports System.Windows.Automation.Peers
Imports System.Windows.Automation.Provider
Imports System.Windows.Xps.Packaging
Imports PdfSharp

Class MainWindow
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
    Dim backspaceFlag As Boolean = False
    Dim myCulture As System.Globalization.CultureInfo = Globalization.CultureInfo.CurrentCulture

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
            MsgBox("Error reading " & file & " file! Error Message: " & ex.Message)
        End Try

    End Sub

    Public Function CheckBlacklist(attendantName As String, day As String, attendantRole As String)
        Dim data() As String
        'Create List
        Dim line As String
        Dim onBlacklist As Integer = 0

        ' Create new StreamReader instance with Using block.
        Using reader As StreamReader = New StreamReader("blacklist.txt")
            ' Read one line from file
            line = reader.ReadLine

            Do While (Not line Is Nothing)
                data = line.Split("{")
                If data(0) = attendantName Then
                    Dim data2 = data(1).Substring(0, data(1).IndexOf("}"))
                    If data2 = day Then
                        Dim data3 = data(2).Substring(0, data(2).IndexOf("}"))
                        If data3 = attendantRole Then
                            onBlacklist = 1
                        End If
                    End If
                End If
                line = reader.ReadLine
            Loop

        End Using

        If onBlacklist = 1 Then
            Return True
        Else
            Return False
        End If

    End Function


    Private Sub GenerateButton_Click(sender As Object, e As RoutedEventArgs) Handles GenerateButton.Click
        If startDateTB.Text = "" Then
            MsgBox("Please enter a start date!")
            Exit Sub
        End If
        Dim test As Date
        If Not Date.TryParseExact(startDateTB.Text.ToString(), "dd/mm/yyyy", System.Globalization.CultureInfo.CurrentCulture, Globalization.DateTimeStyles.None, test) Then
            MsgBox("Start Date is in wrong format! e.g. DD/MM/YYYY")
            Exit Sub
        End If
        frame.Content.ResetVisiblity()
        FillDateFields()
        SoundSelect()
        MicrophonesSelect()
        PlatformSelect()
        AttendantsSelect()
    End Sub

    Public Sub FillDateFields()

        Dim dates As New List(Of String)
        Dim latestdate As Date = startDateTB.Text

        dates.Add(latestdate)
        Do Until dates.Count = 7

            'If Thursday
            If Weekday(latestdate) = 5 Then
                Dim date2 As Date = NextDOW(DayOfWeek.Sunday, latestdate)
                dates.Add(date2)
                latestdate = date2
                'If Sunday
            ElseIf Weekday(latestdate) = 1 Then
                Dim date2 As Date = NextDOW(DayOfWeek.Thursday, latestdate)
                dates.Add(date2)
                latestdate = date2
            ElseIf Weekday(latestdate) < 5 Then
                Dim date2 As Date = NextDOW(DayOfWeek.Thursday, latestdate)
                dates.Add(date2)
                latestdate = date2
            Else
                Dim date2 As Date = NextDOW(DayOfWeek.Thursday, latestdate)
                dates.Add(date2)
                latestdate = date2
            End If

        Loop

        frame.Content.UpdateLabels("Date", dates)


    End Sub

    Public Function NextDOW(whDayOfWeek As DayOfWeek, Optional theDate As DateTime = Nothing) As DateTime
        'returns the next day of the week
        If theDate = Nothing Then theDate = DateTime.Now
        Dim d As DateTime = theDate.AddDays(whDayOfWeek - theDate.DayOfWeek)
        Return If(d <= theDate, d.AddDays(7), d)
    End Function

    Private Sub RandomizeFile(input As String)
        Try
            Dim data() As String = File.ReadAllLines(input & ".txt")
            RandomizeArray(data)
            IO.File.WriteAllLines(input & ".txt", data)
        Catch ex As Exception
            MsgBox("Error randomising file! Error Message: " & ex.Message)
        End Try
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

    Private Sub SoundSelect()
        RandomizeFile("sound")
        sound.Clear()
        soundcnt.Clear()
        ReadFile("sound", sound, soundcnt)

        Dim microphoneslabels = frame.Content.GetLabels("Microphones")
        Dim soundlabels = frame.Content.GetLabels("Sound")
        Dim attendantslabels = frame.Content.GetLabels("Attendants")
        Dim platformlabels = frame.Content.GetLabels("Platform")
        Dim carparklabels = frame.Content.GetLabels("CarPark")
        Dim dates = frame.Content.GetLabels("Date")
        Dim Index As Integer = soundcnt.IndexOf(soundcnt.Min)
        Dim soundfinal As New List(Of String)

        For i As Integer = 0 To 6
            Dim indi As Integer = 0

            Dim dayOfWeek As DayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
            Dim blacklist As Boolean = CheckBlacklist(sound(Index), myCulture.DateTimeFormat.GetDayName(dayOfWeek).ToUpper, "SOUND")

            If blacklist = True Then
                indi = 1
            End If

            If indi = 0 Then
                'update label
                soundfinal.Add(sound(Index))
                UpdateCount("sound", Index)
                sound.Clear()
                soundcnt.Clear()
                ReadFile("sound", sound, soundcnt)
                Index = soundcnt.IndexOf(soundcnt.Min)
                Continue For
            Else
                If Index = 5 Then
                    Index = 0
                    soundfinal.Add(sound(Index))
                    UpdateCount("sound", Index)
                    sound.Clear()
                    soundcnt.Clear()
                    ReadFile("sound", sound, soundcnt)
                    Index = soundcnt.IndexOf(soundcnt.Min)
                    Continue For
                Else
                    Index = Index + 1
                    soundfinal.Add(sound(Index))
                    UpdateCount("sound", Index)
                    sound.Clear()
                    soundcnt.Clear()
                    ReadFile("sound", sound, soundcnt)
                    Index = soundcnt.IndexOf(soundcnt.Min)
                    Continue For
                End If
            End If
            'Loop

        Next

        If soundfinal.Count > 0 Then
            frame.Content.UpdateLabels("Sound", soundfinal)
        End If

    End Sub

    Private Sub MicrophonesSelect()
        RandomizeFile("microphones")
        microphones.Clear()
        microphonescnt.Clear()
        ReadFile("microphones", microphones, microphonescnt)

        Dim microphoneslabels = frame.Content.GetLabels("Microphones")
        Dim soundlabels = frame.Content.GetLabels("Sound")
        Dim attendantslabels = frame.Content.GetLabels("Attendants")
        Dim platformlabels = frame.Content.GetLabels("Platform")
        Dim carparklabels = frame.Content.GetLabels("CarPark")
        Dim dates = frame.Content.GetLabels("Date")
        Dim Index As Integer = microphonescnt.IndexOf(microphonescnt.Min)
        Dim microphonefinal As New List(Of String)
        Dim micprev As String

        For i As Integer = 0 To 6

            Dim indi As Integer = 0

LOOPSTART:

            Dim dayOfWeek As DayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
            Dim blacklist As Boolean = CheckBlacklist(microphones(Index), myCulture.DateTimeFormat.GetDayName(dayOfWeek).ToUpper, "MICROPHONES")

            If blacklist = True Then
                indi = 1
            End If

            If soundlabels(i).Content = microphones(Index) Then
                indi = 1
            End If

            If indi = 0 Then
                'update label
                micprev = microphones(Index)
                microphonefinal.Add(microphones(Index))
                UpdateCount("microphones", Index)
                microphones.Clear()
                microphonescnt.Clear()
                ReadFile("microphones", microphones, microphonescnt)
                Index = microphonescnt.IndexOf(microphonescnt.Min)
            Else
                If Index = microphones.Count - 1 Then
                    Index = 0
                    indi = 0
                    GoTo LOOPSTART
                Else
                    Index = Index + 1
                    indi = 0
                    GoTo LOOPSTART
                End If
            End If

            indi = 0

LOOPSTART2:

            dayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
            blacklist = CheckBlacklist(microphones(Index), myCulture.DateTimeFormat.GetDayName(dayOfWeek).ToUpper, "MICROPHONES")

            If blacklist = True Then
                indi = 1
            End If

            If soundlabels(i).Content = microphones(Index) Then
                indi = 1
            End If

            If micprev = microphones(Index) Then
                indi = 1
            End If

            If indi = 0 Then
                'update label
                microphonefinal.Add(microphones(Index))
                UpdateCount("microphones", Index)
                microphones.Clear()
                microphonescnt.Clear()
                ReadFile("microphones", microphones, microphonescnt)
                Index = microphonescnt.IndexOf(microphonescnt.Min)
            Else
                If Index = microphones.Count - 1 Then
                    Index = 0
                    indi = 0
                    GoTo LOOPSTART2
                Else
                    Index = Index + 1
                    indi = 0
                    GoTo LOOPSTART2
                End If
            End If
        Next

        If microphonefinal.Count > 0 Then
            frame.Content.UpdateLabels("Microphones", microphonefinal)
        End If

    End Sub

    Private Sub PlatformSelect()
        RandomizeFile("platform")
        RandomizeFile("platformsun")
        platform.Clear()
        platformcnt.Clear()
        platformSun.Clear()
        platformSuncnt.Clear()
        ReadFile("platform", platform, platformcnt)
        ReadFile("platformsun", platformSun, platformSuncnt)

        Dim microphoneslabels = frame.Content.GetLabels("Microphones")
        Dim soundlabels = frame.Content.GetLabels("Sound")
        Dim attendantslabels = frame.Content.GetLabels("Attendants")
        Dim platformlabels = frame.Content.GetLabels("Platform")
        Dim carparklabels = frame.Content.GetLabels("CarPark")
        Dim dates = frame.Content.GetLabels("Date")
        Dim Index As Integer = platformcnt.IndexOf(platformcnt.Min)
        Dim IndexSun As Integer = platformSuncnt.IndexOf(platformSuncnt.Min)
        Dim platformfinal As New List(Of String)
        Dim platformprev As String
        Dim loopind As Integer = 0

        For i As Integer = 0 To 6

            Dim indi As Integer = 0

LOOPSTART:
            If Weekday(dates(i).Content) = 1 Then

                Dim dayOfWeek As DayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
                Dim blacklist As Boolean = CheckBlacklist(platform(Index), myCulture.DateTimeFormat.GetDayName(dayOfWeek).ToUpper, "PLATFORM")

                If blacklist = True Then
                    indi = 1
                End If

                If soundlabels(i).Content = platformSun(IndexSun) Then
                    indi = 1
                End If

                If microphoneslabels(loopind).Content = platformSun(IndexSun) Then
                    indi = 1
                End If

                If microphoneslabels(loopind + 1).Content = platformSun(IndexSun) Then
                    indi = 1
                End If

                If indi = 0 Then
                    'update label
                    platformprev = platformSun(IndexSun)
                    platformfinal.Add(platformSun(IndexSun))
                    UpdateCount("platformsun", IndexSun)
                    platformSun.Clear()
                    platformSuncnt.Clear()
                    ReadFile("platformsun", platformSun, platformSuncnt)
                    IndexSun = platformSuncnt.IndexOf(platformSuncnt.Min)
                Else
                    If IndexSun = platformSun.Count - 1 Then
                        IndexSun = 0
                        indi = 0
                        GoTo LOOPSTART
                    Else
                        IndexSun = IndexSun + 1
                        indi = 0
                        GoTo LOOPSTART
                    End If
                End If

                indi = 0
            Else

                Dim dayOfWeek2 As DayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
                Dim blacklist2 As Boolean = CheckBlacklist(platform(Index), myCulture.DateTimeFormat.GetDayName(dayOfWeek2).ToUpper, "PLATFORM")

                If blacklist2 = True Then
                    indi = 1
                End If

                If soundlabels(i).Content = platform(Index) Then
                    indi = 1
                End If

                If microphoneslabels(loopind).Content = platform(Index) Then
                    indi = 1
                End If

                If microphoneslabels(loopind + 1).Content = platform(Index) Then
                    indi = 1
                End If

                If indi = 0 Then
                    'update label
                    platformprev = platform(Index)
                    platformfinal.Add(platform(Index))
                    UpdateCount("platform", Index)
                    platform.Clear()
                    platformcnt.Clear()
                    ReadFile("platform", platform, platformcnt)
                    Index = platformcnt.IndexOf(platformcnt.Min)
                Else
                    If Index = platform.Count - 1 Then
                        Index = 0
                        indi = 0
                        GoTo LOOPSTART
                    Else
                        Index = Index + 1
                        indi = 0
                        GoTo LOOPSTART
                    End If
                End If

                indi = 0

            End If

LOOPSTART2:

            Dim dayOfWeek3 As DayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
            Dim blacklist3 As Boolean = CheckBlacklist(platform(Index), myCulture.DateTimeFormat.GetDayName(dayOfWeek3).ToUpper, "PLATFORM")

            If blacklist3 = True Then
                indi = 1
            End If

            If soundlabels(i).Content = platform(Index) Then
                indi = 1
            End If

            If microphoneslabels(loopind).Content = platform(Index) Then
                indi = 1
            End If

            If microphoneslabels(loopind + 1).Content = platform(Index) Then
                indi = 1
            End If

            If platformprev = platform(Index) Then
                indi = 1
            End If

            If Weekday(dates(i).Content) = 5 Then

                If indi = 0 Then
                    'update label
                    platformfinal.Add(platform(Index))
                    UpdateCount("platform", Index)
                    platform.Clear()
                    platformcnt.Clear()
                    ReadFile("platform", platform, platformcnt)
                    Index = platformcnt.IndexOf(platformcnt.Min)
                Else
                    If Index = platform.Count - 1 Then
                        Index = 0
                        indi = 0
                        GoTo LOOPSTART2
                    Else
                        Index = Index + 1
                        indi = 0
                        GoTo LOOPSTART2
                    End If
                End If

            Else
                frame.Content.HideLabel("Platform", loopind + 2)
            End If

            loopind = loopind + 2
        Next

        If platformfinal.Count > 0 Then
            frame.Content.UpdateLabels("Platform", platformfinal)
        End If

    End Sub

    Private Sub AttendantsSelect()
        RandomizeFile("attendants")
        RandomizeFile("attendantselders")
        attendants.Clear()
        attendantscnt.Clear()
        attendantsElders.Clear()
        attendantsElderscnt.Clear()
        ReadFile("attendants", attendants, attendantscnt)
        ReadFile("attendantselders", attendantsElders, attendantsElderscnt)

        Dim microphoneslabels = frame.Content.GetLabels("Microphones")
        Dim soundlabels = frame.Content.GetLabels("Sound")
        Dim attendantslabels = frame.Content.GetLabels("Attendants")
        Dim platformlabels = frame.Content.GetLabels("Platform")
        'Dim carparklabels = frame.Content.GetLabels("CarPark")
        Dim dates = frame.Content.GetLabels("Date")
        Dim Index As Integer = attendantscnt.IndexOf(attendantscnt.Min)
        Dim IndexElders As Integer = attendantsElderscnt.IndexOf(attendantsElderscnt.Min)
        Dim attendantsfinal As New List(Of String)
        Dim carparkfinal As New List(Of String)
        Dim attendantsprev As String
        Dim loopind As Integer = 0

        For i As Integer = 0 To 6

            Dim indi As Integer = 0

LOOPSTART:

            Dim dayOfWeek As DayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
            Dim blacklistElders As Boolean = CheckBlacklist(attendantsElders(IndexElders), myCulture.DateTimeFormat.GetDayName(dayOfWeek).ToUpper, "ATTENDANTS")

            If blacklistElders = True Then
                indi = 1
            End If

            'If Weekday(dates(i).Content) = 1 And attendantsElders(IndexElders) = "K WILSON" Then
            '    indi = 1
            'End If

            'If Weekday(dates(i).Content) = 1 And attendantsElders(IndexElders) = "C MACNEILL" Then
            '    indi = 1
            'End If

            If soundlabels(i).Content = attendantsElders(IndexElders) Then
                indi = 1
            End If

            If microphoneslabels(loopind).Content = attendantsElders(IndexElders) Then
                indi = 1
            End If

            If microphoneslabels(loopind + 1).Content = attendantsElders(IndexElders) Then
                indi = 1
            End If

            If platformlabels(loopind).Content = attendantsElders(IndexElders) Then
                indi = 1
            End If

            If platformlabels(loopind + 1).Content = attendantsElders(IndexElders) Then
                indi = 1
            End If

            If indi = 0 Then
                'update label
                attendantsprev = attendantsElders(IndexElders)
                attendantsfinal.Add(attendantsElders(IndexElders))
                UpdateCount("attendantselders", IndexElders)
                attendantsElders.Clear()
                attendantsElderscnt.Clear()
                ReadFile("attendantselders", attendantsElders, attendantsElderscnt)
                IndexElders = attendantsElderscnt.IndexOf(attendantsElderscnt.Min)
            Else
                If IndexElders = attendantsElders.Count - 1 Then
                    IndexElders = 0
                    indi = 0
                    GoTo LOOPSTART
                Else
                    IndexElders = IndexElders + 1
                    indi = 0
                    GoTo LOOPSTART
                End If
            End If

            indi = 0

LOOPSTART2:

            dayOfWeek = myCulture.Calendar.GetDayOfWeek(dates(i).Content)
            Dim blacklist As Boolean = CheckBlacklist(attendants(Index), myCulture.DateTimeFormat.GetDayName(dayOfWeek).ToUpper, "ATTENDANTS")

            If blacklist = True Then
                indi = 1
            End If

            If soundlabels(i).Content = attendants(Index) Then
                indi = 1
            End If

            If microphoneslabels(loopind).Content = attendants(Index) Then
                indi = 1
            End If

            If microphoneslabels(loopind + 1).Content = attendants(Index) Then
                indi = 1
            End If

            If platformlabels(loopind).Content = attendants(Index) Then
                indi = 1
            End If

            If platformlabels(loopind + 1).Content = attendants(Index) Then
                indi = 1
            End If

            If attendantsprev = attendants(Index) Then
                indi = 1
            End If

            If indi = 0 Then
                'update label
                attendantsfinal.Add(attendants(Index))
                carparkfinal.Add(attendants(Index))
                UpdateCount("attendants", Index)
                attendants.Clear()
                attendantscnt.Clear()
                ReadFile("attendants", attendants, attendantscnt)
                Index = attendantscnt.IndexOf(attendantscnt.Min)
            Else
                If Index = attendants.Count - 1 Then
                    Index = 0
                    indi = 0
                    GoTo LOOPSTART2
                Else
                    Index = Index + 1
                    indi = 0
                    GoTo LOOPSTART2
                End If
            End If

            loopind = loopind + 2
        Next

        If attendantsfinal.Count > 0 Then
            frame.Content.UpdateLabels("Attendants", attendantsfinal)
        End If

        If carparkfinal.Count > 0 Then
            frame.Content.UpdateLabels("CarPark", carparkfinal)
        End If

    End Sub

    Private Sub UpdateCount(input As String, Index As Integer)
        Try
            Dim data() As String = File.ReadAllLines(input & ".txt")
            Dim i As Integer = data(Index).IndexOf("{")
            Dim f As String = data(Index).Substring(i + 1, data(Index).IndexOf("}", i + 1) - i - 1)
            Dim newChar As Integer = f + 1
            data(Index) = data(Index).Replace(f, newChar)
            RandomizeArray(data)
            IO.File.WriteAllLines(input & ".txt", data)
        Catch ex As Exception
            MsgBox("Error trying to update count! Error message: " & ex.Message)
        End Try
    End Sub

    Private Shared Sub SaveUsingEncoder(ByVal fileName As String, ByVal UIElement As FrameworkElement, ByVal encoder As BitmapEncoder)
        Dim height As Integer = CInt(UIElement.ActualHeight)
        Dim width As Integer = CInt(UIElement.ActualWidth)
        UIElement.Measure(New System.Windows.Size(width, height))
        UIElement.Arrange(New Rect(0, 0, width, height))
        Dim bitmap As RenderTargetBitmap = New RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32)
        bitmap.Render(UIElement)
        SaveUsingBitmapTargetRenderer(fileName, bitmap, encoder)
    End Sub

    Private Shared Sub SaveUsingBitmapTargetRenderer(ByVal fileName As String, ByVal renderTargetBitmap As RenderTargetBitmap, ByVal bitmapEncoder As BitmapEncoder)
        Dim frame As BitmapFrame = BitmapFrame.Create(renderTargetBitmap)
        bitmapEncoder.Frames.Add(frame)

        Using stream = File.Create(fileName)
            bitmapEncoder.Save(stream)
        End Using
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs) Handles SaveImageButton.Click
        'SaveUsingEncoder("soundschedule.png", frame, New JpegBitmapEncoder())
        'MsgBox("Image saved!")
        Try
            Dim lMemoryStream As MemoryStream = New MemoryStream()
            Dim package As Package = Package.Open(lMemoryStream, FileMode.Create)
            Dim doc As XpsDocument = New XpsDocument(package)
            Dim writer = XpsDocument.CreateXpsDocumentWriter(doc)
            writer.Write(frame)
            doc.Close()
            package.Close()
            Dim pdfXpsDoc = Xps.XpsModel.XpsDocument.Open(lMemoryStream)
            Xps.XpsConverter.Convert(pdfXpsDoc, "sound-schedule.pdf", 0)
            MsgBox("PDF file saved!")
        Catch ex As Exception
            MsgBox("Error trying to export to PDF. Error message: " & ex.Message)
        End Try
    End Sub

    Private Sub PrintButton_Click(sender As Object, e As RoutedEventArgs) Handles PrintButton.Click
        Try
            Dim printdlg As New PrintDialog
            printdlg.PrintTicket.PageOrientation = Printing.PageOrientation.Landscape
            Hide()
            frame.RenderTransform = New ScaleTransform(1.25, 1.25)
            If printdlg.ShowDialog = True Then printdlg.PrintVisual(frame, "")
            Show()
            frame.RenderTransform = New ScaleTransform(1, 1)
        Catch ex As Exception
            MsgBox("Error trying to print the schedule. Error message: " & ex.Message)
        End Try

    End Sub

    Private Sub StartDateTB_KeyDown(sender As Object, e As KeyEventArgs) Handles startDateTB.KeyDown
        If e.Key = Key.[Return] Then
            Dim peer As ButtonAutomationPeer = New ButtonAutomationPeer(GenerateButton)
            Dim invokeProv As IInvokeProvider = TryCast(peer.GetPattern(PatternInterface.Invoke), IInvokeProvider)
            invokeProv.Invoke()
        End If
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim dpiX As Double, dpiY As Double
        Dim source = PresentationSource.FromVisual(Me)

        If Not source Is Nothing Then
            dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11
            dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22
            If dpiX > 96 And dpiY > 96 Then
                Me.WindowState = WindowState.Maximized
            Else
                'Dim transform As New ScaleTransform
                'transform.ScaleX = 1.25
                'transform.ScaleX = 1.25
                'frame.RenderTransform = transform
            End If
        End If

        If Not File.Exists("sound.txt") Then
            'MsgBox("sound.txt file cannot be found! Please move sound.txt to the directory of the .exe file!")
            'Me.Close()
            File.Create("sound.txt").Dispose()
        End If

        If Not File.Exists("microphones.txt") Then
            'MsgBox("sicrophones.txt file cannot be found! Please move microphones.txt to the directory of the .exe file!")
            'Me.Close()
            File.Create("microphones.txt").Dispose()
        End If

        If Not File.Exists("platform.txt") Then
            'MsgBox("platform.txt file cannot be found! Please move platform.txt to the directory of the .exe file!")
            'Me.Close()
            File.Create("platform.txt").Dispose()
        End If

        If Not File.Exists("platformsun.txt") Then
            'MsgBox("platformsun.txt file cannot be found! Please move platformsun.txt to the directory of the .exe file!")
            'Me.Close()
            File.Create("platformsun.txt").Dispose()
        End If

        If Not File.Exists("attendants.txt") Then
            'MsgBox("attendants.txt file cannot be found! Please move attendants.txt to the directory of the .exe file!")
            'Me.Close()
            File.Create("attendants.txt").Dispose()
        End If

        If Not File.Exists("attendantselders.txt") Then
            'MsgBox("attendantselders.txt file cannot be found! Please move attendantselders.txt to the directory of the .exe file!")
            'Me.Close()
            File.Create("attendantselders.txt").Dispose()
        End If

        If Not File.Exists("blacklist.txt") Then
            'MsgBox("blacklist.txt file cannot be found! Please move blacklist.txt to the directory of the .exe file!")
            File.Create("blacklist.txt").Dispose()
            'Me.Close()
        End If

    End Sub

    Private Sub EditAttendantsClick(sender As Object, e As RoutedEventArgs)
        Dim form As New EditAttendantsWindow
        form.ShowDialog()
    End Sub

    Private Sub startDateTB_TextChanged(sender As Object, e As TextChangedEventArgs) Handles startDateTB.TextChanged
        If backspaceFlag Then
            backspaceFlag = False
            Exit Sub
        End If
        Select Case startDateTB.Text.Length
            Case = 2
                startDateTB.Text = startDateTB.Text + "/"
                startDateTB.Select(startDateTB.Text.Length, 0)
            Case = 5
                startDateTB.Text = startDateTB.Text + "/"
                startDateTB.Select(startDateTB.Text.Length, 0)
        End Select
    End Sub

    Private Sub startDateTB_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles startDateTB.PreviewKeyDown
        If e.Key = Key.Back Then
            backspaceFlag = True
        End If
    End Sub
End Class

