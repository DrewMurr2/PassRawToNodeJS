Imports MongoDB.Bson.Serialization.Attributes
Imports MongoDB.Driver
Imports MongoDB.Bson
Imports MongoDB.Driver.Core
<BsonIgnoreExtraElements>
Public Class Settings
    Public Shared Property Client As New MongoClient()
    Public Shared Property SettingsDB As IMongoDatabase = Client.GetDatabase("Settings")
    Public Shared Property WellsDB As IMongoDatabase = Client.GetDatabase("Wells")
    Public Shared Property RawInstant As New RawInstant
    <BsonIgnoreExtraElements>
    Public Class WitsChannels
        Public Shared Property collection As IMongoCollection(Of WitsChannels) = SettingsDB.GetCollection(Of WitsChannels)("WitsChannels")
        <BsonId>
        Public Property _id As ObjectId = ObjectId.GenerateNewId()
        Public Property instant As Date = Date.UtcNow()
        Public Property Channels As New List(Of WitsChannel)
        Public Sub AddChannel(id__ As Integer, name__ As String)
            Channels.Add(New WitsChannel(id__, name__))
        End Sub

        Public Class WitsChannel
            Public Sub New(id__, name__)
                ID = id__
                Name = name__
            End Sub
            Public Property ID As Int16
            Public Property Name As String
            Public Property type As String = "number"
        End Class

        Public Sub New()
            '  _id = Date.UtcNow()
        End Sub

        Public Shared Function Latest() As WitsChannels
            Dim d As List(Of WitsChannels) = collection.Find(Function(s) s._id <> Nothing).SortByDescending(Function(s) s.instant).Limit(1).ToListAsync().Result
            If d.Count > 0 Then
                Return d(0)
            Else
                Return Nothing
            End If
        End Function


        Public Sub Save()
            collection.InsertOne(Me)
        End Sub
        Public Sub Upsert()
            collection.ReplaceOneAsync(Function(i) i._id = _id, Me, New UpdateOptions() With {.IsUpsert = True}).Wait()
        End Sub

    End Class

    Public Class WellLogInterval
        Public Shared Property WellLogIntervalcollection As IMongoCollection(Of WellLogInterval) = SettingsDB.GetCollection(Of WellLogInterval)("WellLogIntervals")
        Public Property MachineID As Long
        Public Property _id As ObjectId = ObjectId.GenerateNewId()
        Public Property WellID As Long
        Public Property WellName As String
        Public Property StartLog As Date
        Public Property StopLog As Date = New Date(3000, 1, 1) ''The year 3000
        Public Property WitsChannels As Settings.WitsChannels
        Public Property Complete As Boolean = False ''Allows for time machine

        Public Sub Save()
            WellLogIntervalcollection.InsertOne(Me)
        End Sub
        Public Sub Upsert()
            WellLogIntervalcollection.ReplaceOneAsync(Function(i) i._id = _id, Me, New UpdateOptions() With {.IsUpsert = True}).Wait()
        End Sub
        Public Shared Function Latest() As WellLogInterval
            Dim w As List(Of WellLogInterval) = WellLogIntervalcollection.Find(Function(s) (s._id <> Nothing AndAlso s.Complete = False)).SortByDescending(Function(s) s._id).Limit(1).ToListAsync().Result
            If w.Count > 0 Then
                Return w(0)
            Else
                Return Nothing
            End If
        End Function
        Public Shared Function fromTime(t As Date) As WellLogInterval
            Dim d As List(Of WellLogInterval) = WellLogIntervalcollection.Find(Function(s) (s._id <> Nothing AndAlso s.StartLog <= t AndAlso s.StopLog >= t)).SortByDescending(Function(s) s._id).Limit(1).ToListAsync().Result
            If d.Count > 0 Then
                Return d(0)
            Else
                Return Nothing
            End If
        End Function
        Public Sub MarkAsComplete()
            Complete = True

        End Sub
        'Public Function LastInterpolatedTime(Optional resolution As Integer = 1) As Date
        '    Dim l = WellBlockCollection().Find(Function(s) (s.StartTime > StartLog AndAlso s.Resolution = resolution)).SortByDescending(Function(s) s.StartTime).Limit(1).ToListAsync().Result()
        '    If l.Count = 0 Then
        '        Return Nothing
        '    Else
        '        Return l(0).StartTime
        '    End If
        'End Function
        'Public Function LastInterpolatedBlocks(Optional resolution As Integer = 1, Optional limit As Integer = 1) As List(Of Well.Block)
        '    Return WellBlockCollection().Find(Function(s) (s.StartTime > StartLog AndAlso s.Resolution = resolution)).SortByDescending(Function(s) s.StartTime).Limit(limit).ToListAsync().Result()
        'End Function

        Public Function getRawUninterpolated(limit As Integer, Optional startTime As Date = Nothing) As List(Of RawInstant)
            Dim returnList As New List(Of RawInstant)
            '   If startTime = Nothing Then startTime = LastInterpolatedTime()
            If startTime = Nothing OrElse startTime < StartLog Then startTime = StartLog

            Dim MonthHolder As Date = startTime
            Dim stopTime As Date = Now()
            If StopLog <> Nothing AndAlso StopLog < stopTime Then stopTime = StopLog
            Do While returnList.Count < limit AndAlso MonthHolder < stopTime
                returnList = RawInstant.getCollection(MonthHolder).Find(Function(s) (s.CapturedTime > startTime AndAlso s.CapturedTime < StopLog)).SortBy(Function(s) s.CapturedTime).Limit(limit).ToListAsync().Result
                MonthHolder = MonthHolder.AddMonths(1)
            Loop
            Return returnList
        End Function
        'Public Function getUninterpolatedBlocks(ResolutionFrom As Integer, ResolutionTo As Integer, limit As Integer, Optional startTime As Date = Nothing) As List(Of Well.Block)

        '    If startTime = Nothing Then startTime = LastInterpolatedTime(ResolutionTo)
        '    If startTime = Nothing OrElse startTime < StartLog Then startTime = StartLog
        '    Dim stopTime As Date = Now()
        '    If StopLog <> Nothing AndAlso StopLog < stopTime Then stopTime = StopLog
        '    Return WellBlockCollection.Find(Function(s) (s.DocType = "Block" AndAlso s.Resolution = ResolutionFrom AndAlso s.StartTime >= startTime AndAlso s.EndTime <= StopLog)).SortBy(Function(s) s.StartTime).Limit(limit).ToListAsync().Result

        'End Function
    End Class

End Class
