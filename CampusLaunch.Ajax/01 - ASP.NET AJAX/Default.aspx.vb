Imports System.Collections.Generic
Imports System.Linq

Partial Class Demo_01_ASPNET_Default
    Inherits System.Web.UI.Page

    Private launchEvents As New List(Of CampusLaunchEvent)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Daten erzeugen
        launchEvents.Add(New CampusLaunchEvent With {.City = "Hamburg", .EventDate = New Date(2008, 4, 3)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Dresden", .EventDate = New Date(2008, 4, 4)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Duisburg-Essen", .EventDate = New Date(2008, 4, 7)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Berlin", .EventDate = New Date(2008, 4, 8)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Köln", .EventDate = New Date(2008, 4, 11)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Leipzig", .EventDate = New Date(2008, 4, 11)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "München", .EventDate = New Date(2008, 4, 15)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Karlsruhe", .EventDate = New Date(2008, 4, 18)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Freiburg", .EventDate = New Date(2008, 4, 21)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Bochum", .EventDate = New Date(2008, 4, 24)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Wuppertal", .EventDate = New Date(2008, 4, 24)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Darmstadt", .EventDate = New Date(2008, 4, 28)})
        launchEvents.Add(New CampusLaunchEvent With {.City = "Aachen", .EventDate = New Date(2008, 4, 29)})
    End Sub

    Protected Sub EventCalendar_DayRender(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DayRenderEventArgs) Handles EventCalendar.DayRender
        ' Event für den aktuellen Tag suchen ...
        Dim launchEvent = (From x In launchEvents Where x.EventDate = e.Day.Date).FirstOrDefault

        If launchEvent IsNot Nothing Then
            ' Event gefunden --> Fett markieren
            e.Cell.Font.Bold = True
        End If
    End Sub

    Protected Sub EventCalendar_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles EventCalendar.SelectionChanged
        Dim selectedDate As Date
        Dim foundEvents As List(Of CampusLaunchEvent)

        ' Events für den ausgewählten Tag finden
        selectedDate = EventCalendar.SelectedDate
        foundEvents = (From x In launchEvents Where x.EventDate = selectedDate).ToList

        ' Ergebnisse anzeigen
        EventLabel.Text = String.Format("{0} Events gefunden", foundEvents.Count)
        EventLabel.Visible = True

        EventRepeater.DataSource = foundEvents
        EventRepeater.DataBind()

        Threading.Thread.Sleep(TimeSpan.FromSeconds(5))
    End Sub
End Class