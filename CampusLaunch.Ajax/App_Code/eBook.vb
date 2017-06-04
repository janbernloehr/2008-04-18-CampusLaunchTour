Option Strict On

Imports System.ComponentModel

Public Class eBook

    Private _Title As String
    <DataObjectField(True)> _
    Public Property Title() As String
        Get
            Return _Title
        End Get
        Set(ByVal value As String)
            _Title = value
        End Set
    End Property

    Private _ThumbnailUrl As String
    <DataObjectField(False)> _
    Public Property ThumbnailUrl() As String
        Get
            Return _ThumbnailUrl
        End Get
        Set(ByVal value As String)
            _ThumbnailUrl = value
        End Set
    End Property

    Private _BookUrl As String
    <DataObjectField(False)> _
    Public Property BookUrl() As String
        Get
            Return _BookUrl
        End Get
        Set(ByVal value As String)
            _BookUrl = value
        End Set
    End Property

End Class
