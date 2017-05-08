Imports MongoDB.Driver
Imports MongoDB.Bson
Imports MongoDB.Bson.Serialization.Attributes
Imports CSharpWorker
Public Class Wells
    <BsonId>
    Public Property _id As String = Naming.newName()
    Public Property Name As String = "test well"
    Shared Client As New MongoClient
    Shared WellsDB As IMongoDatabase = Client.GetDatabase("Wells")
    Shared Collection As IMongoCollection(Of Wells) = WellsDB.GetCollection(Of Wells)("Wells")
    Public Sub Upsert()
        Collection.ReplaceOneAsync(Function(i) i._id Is _id, Me, New UpdateOptions() With {.IsUpsert = True}).Wait()
    End Sub
    Public Sub Save()
        Collection.InsertOne(Me)
    End Sub

    Public Shared Function get_1s_Collection(id As Long) As IMongoCollection(Of CheckWellTime)
        Return WellsDB.GetCollection(Of CheckWellTime)("Well" & id & "1s")
    End Function
    Public Class CheckWellTime
        Public Property Instant As Date
        Public Shared Function Latest(id As String) As Date
            Dim d As Date
            Dim cwtList As List(Of CheckWellTime)
            cwtList = Wells.get_1s_Collection(id).Find(Function(s) s.Instant <> Nothing).SortByDescending(Function(s) s.Instant).Limit(1).ToListAsync().Result
            If cwtList.Count > 0 Then
                Return cwtList(0).Instant
            Else
                Return Nothing
            End If
        End Function
    End Class
End Class


