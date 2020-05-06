Imports System.Windows.Automation.Peers
Imports System.Windows.Automation.Provider

Public Class Window1
    Public Updated As Boolean = False

    Private Sub SaveButton_Click(sender As Object, e As RoutedEventArgs) Handles SaveButton.Click
        Updated = True
        Me.Close()
    End Sub

    Private Sub LabelTB_KeyDown(sender As Object, e As KeyEventArgs) Handles LabelTB.KeyDown
        If e.Key = Key.[Return] Then
            Dim peer As ButtonAutomationPeer = New ButtonAutomationPeer(SaveButton)
            Dim invokeProv As IInvokeProvider = TryCast(peer.GetPattern(PatternInterface.Invoke), IInvokeProvider)
            invokeProv.Invoke()
        End If
    End Sub
End Class
