Module Module1

    Sub Main()
        Dim Wits = getWits()
        Dim WellID = getWell_id()
        Dim t = getLastRetrievedTime()
        Dim rawInstants = getRawUnsent(t)
        Dim consolidatedRawInstants = consolidateRaw(t, rawInstants)
    End Sub
    Public Function getWits() As Settings.WitsChannels

    End Function
    Public Function getWell_id() As Long

    End Function
    Public Function getLastRetrievedTime() As Date
        Return Nothing
    End Function
    Public Function getRawUnsent(t As Date) As List(Of RawInstant)
        Return Nothing
    End Function
    Public Function consolidateRaw(t As Date, raw As List(Of RawInstant)) As List(Of RawInstant)

        ''This makes sure they are all rounded to whole seconds and that the time does match an existing point
        For Each r As RawInstant In raw
            r.CapturedTime = r.CapturedTime.Truncate(TimeSpan.FromSeconds(1))
            If r.CapturedTime = t Then r.CapturedTime = r.CapturedTime.AddSeconds(1)
        Next

        ''This merges those with the same time
        For Each r As RawInstant In raw
            For Each r2 As RawInstant In raw
                If r.CapturedTime = r2.CapturedTime Then mergeTwoInstants(r, r2)
            Next
        Next

        ''Gets rid of the duplicates that were merged
        Dim newRawList As New List(Of RawInstant)
        For Each r As RawInstant In raw
            If r.CapturedTime <> Nothing Then newRawList.Add(r)
        Next

        Return newRawList
    End Function
    Public Function mergeTwoInstants(r As RawInstant, r2 As RawInstant) As RawInstant
        r.data.AddRange(r2.data) ''Adds the two ranges
        r2.CapturedTime = Nothing ''Sets the extra captured time to nothing so that it can be removed later
        For i = r.data.Count - 1 To 1 ''Eliminates duplicates
            For j = i - 1 To 0
                If r.data(i).id = r.data(j).id Then r.data.Remove(r.data(i))
            Next
        Next
        Return r
    End Function
    <System.Runtime.CompilerServices.Extension>
    Public Function Truncate(dateTime As DateTime, timeSpan__1 As TimeSpan) As DateTime
        If timeSpan__1 = TimeSpan.Zero Then
            Return dateTime
        End If
        ' Or could throw an ArgumentException
        Return dateTime.AddTicks(-(dateTime.Ticks Mod timeSpan__1.Ticks))
    End Function
End Module
