Public Class ReturnObject
    Public WellID As Long
    Public Instant As Date
    Public Tracks As List(Of dataPair)
    Public Class dataPair
        Public Property Name As String
        Public Property Value As Double
    End Class
End Class
