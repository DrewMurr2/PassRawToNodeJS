using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace CSharpWorker
{
    public class SettingsCSharp
    {
        public static MongoClient Client { get; set; } = new MongoClient();
        public static IMongoDatabase SettingsDB { get; set; } = Client.GetDatabase("Settings");
        public static IMongoDatabase WellsDB { get; set; } = Client.GetDatabase("Wells");

        //public static RawInstant RawInstant { get; set; }
      


         [BsonIgnoreExtraElements]
        public class WitsChannels
        {
            public static IMongoCollection<WitsChannels> collection { get; set; } = SettingsDB.GetCollection<WitsChannels>("WitsChannels");
            public System.DateTime _id { get; set; }
            public List<WitsChannel> Channels { get; set; }
            public void AddChannel(int id__, string name__)
            {
                Channels.Add(new WitsChannel(id__, name__));
            }

            public class WitsChannel
            {
                public WitsChannel(int id__, string name__)
                {
                    ID = id__;
                    Name = name__;
                }
                public int ID { get; set; }
                public string Name { get; set; }
                public string type { get; set; }
            }

            public WitsChannels()
            {
                _id = System.DateTime.UtcNow;
            }

            public static WitsChannels Latest()
            {
                List<WitsChannels> d = collection.Find(s => s._id != null).SortByDescending(s => s._id).Limit(1).ToListAsync().Result;
                if (d.Count > 0)
                {
                    return d[0];
                }
                else
                {
                    return null;
                }
            }


            public void Save()
            {
                collection.InsertOne(this);
    
            }

        }
        public class WellLogInterval
        {
            public static IMongoCollection<WellLogInterval> WellLogIntervalcollection { get; set; }
            public System.DateTime _id = System.DateTime.UtcNow;
            public long MachineID { get; set; }
            public long WellID { get; set; }
            public string WellName { get; set; }
            public System.DateTime StartLog { get; set; }
            public System.DateTime StopLog { get; set; }
            //'The year 3000
            public SettingsCSharp.WitsChannels WitsChannels { get; set; }
            public bool Complete { get; set; }//'Allows for time machine


            public void Save()
            {
                WellLogIntervalcollection.InsertOne(this);
            }
            public static WellLogInterval Latest()
            {
                List<WellLogInterval> w = WellLogIntervalcollection.Find(s => (s._id != null && s.Complete == false)).SortByDescending(s => s._id).Limit(1).ToListAsync().Result;
                if (w.Count > 0)
                {
                    return w[0];
                }
                else
                {
                    return null;
                }
            }
            public static WellLogInterval fromTime(System.DateTime t)
            {
                List<WellLogInterval> d = WellLogIntervalcollection.Find(s => (s._id != null && s.StartLog <= t && s.StopLog >= t)).SortByDescending(s => s._id).Limit(1).ToListAsync().Result;
                if (d.Count > 0)
                {
                    return d[0];
                }
                else
                {
                    return null;
                }
            }
            public void MarkAsComplete()
            {
                Complete = true;

            }
            //Public Function LastInterpolatedTime(Optional resolution As Integer = 1) As Date
            //    Dim l = WellBlockCollection().Find(Function(s) (s.StartTime > StartLog AndAlso s.Resolution = resolution)).SortByDescending(Function(s) s.StartTime).Limit(1).ToListAsync().Result()
            //    If l.Count = 0 Then
            //        Return Nothing
            //    Else
            //        Return l(0).StartTime
            //    End If
            //End Function
            //Public Function LastInterpolatedBlocks(Optional resolution As Integer = 1, Optional limit As Integer = 1) As List(Of Well.Block)
            //    Return WellBlockCollection().Find(Function(s) (s.StartTime > StartLog AndAlso s.Resolution = resolution)).SortByDescending(Function(s) s.StartTime).Limit(limit).ToListAsync().Result()
            //End Function

            //public List<RawInstant> getRawUninterpolated(int limit, System.DateTime startTime = null)
            //{
            //    List<RawInstant> returnList = new List<RawInstant>();
            //    //   If startTime = Nothing Then startTime = LastInterpolatedTime()
            //    if (startTime == null || startTime < StartLog)
            //        startTime = StartLog;

            //    System.DateTime MonthHolder = startTime;
            //    System.DateTime stopTime = Now();
            //    if (StopLog != null && StopLog < stopTime)
            //        stopTime = StopLog;
            //    while (returnList.Count < limit && MonthHolder < stopTime)
            //    {
            //        returnList = RawInstant.getCollection(MonthHolder).Find(s => (s.CapturedTime > startTime && s.CapturedTime < StopLog)).SortBy(s => s.CapturedTime).Limit(limit).ToListAsync().Result;
            //        MonthHolder = MonthHolder.AddMonths(1);
            //    }
            //    return returnList;
            //}
        }

    }
}
