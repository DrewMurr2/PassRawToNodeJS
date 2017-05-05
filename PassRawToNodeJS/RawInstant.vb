Imports MongoDB.Driver
Imports MongoDB.Bson
Imports MongoDB.Bson.Serialization.Attributes

<BsonIgnoreExtraElements>
    Public Class RawInstant
    Dim Client As New MongoClient
    Public Property CapturedTime As Date

    Public Property data As New List(Of dataPair)
    Dim WitsChannels As Settings.WitsChannels
    Private Function returnWitsChannel(id As Integer) As Settings.WitsChannels.WitsChannel
            If WitsChannels IsNot Nothing Then
                For Each wtsChnl As Settings.WitsChannels.WitsChannel In WitsChannels.Channels
                    If wtsChnl.ID = id Then Return wtsChnl
                Next
            End If
            Return Nothing
        End Function
        Public Sub add(i, v)
            data.Add(New dataPair() With {.id = i, .val = v, .witsChannel = returnWitsChannel(i)})
        End Sub
        Public Class dataPair
            Public Property id As Integer
            Public Property val As String
            Public Property witsChannel As Settings.WitsChannels.WitsChannel
        End Class

        Public Function getCollection(Optional ct As Date = Nothing) As IMongoCollection(Of RawInstant)
            If ct = Nothing Then ct = CapturedTime
            Return Client.GetDatabase("Raw").GetCollection(Of RawInstant)("RawInstants_" & ct.Month() & "_" & ct.Year()) ' Establish Mongo Collection this naming convention is RawInstants_Mo_Year
        End Function
        Public Function getMore(ts As TimeSpan) As List(Of RawInstant)
            Dim l As List(Of RawInstant) = getCollection.Find(Function(s) s.CapturedTime > CapturedTime AndAlso s.CapturedTime <= CapturedTime + ts).ToListAsync.Result
            If l Is Nothing Then l = New List(Of RawInstant)
            Return l
        End Function
        Public Sub Save()
            getCollection.InsertOne(Me)
        End Sub
End Class