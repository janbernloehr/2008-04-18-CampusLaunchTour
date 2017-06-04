Option Strict On

Public Class CampusLaunchEvent
    Private _City As String
    Public Property City() As String
        Get
            Return _City
        End Get
        Set(ByVal value As String)
            _City = value
        End Set
    End Property

    Private _EventDate As Date
    Public Property EventDate() As Date
        Get
            Return _EventDate
        End Get
        Set(ByVal value As Date)
            _EventDate = value
        End Set
    End Property
End Class