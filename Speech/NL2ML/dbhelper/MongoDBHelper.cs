using MongoDB.Bson;
using MongoDB.Driver;
using NL2ML.consts;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.dbhelper
{
    public class MongoDBHelper : IDBHelper
    {
        private static Random rand = new Random();

        public string TranslateCommand(string input)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableCommandCollection);

            QueryDocument query = new QueryDocument("Name", input);
            BsonDocument doc = coll.FindOne(query);
            if (doc != null)
            {
                return doc["Command"].ToString();
            }

            return "";
        }


        public MediaData GetRandomMusicByGenre(string genre)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument("Metadata.Genre", genre);
            long count = coll.Count(query);
            var cursor = coll.Find(query);
            cursor.Skip = rand.Next((int)(count - 1));
            cursor.Limit = 1;
            foreach (var doc in cursor)
            {
                BsonValue meta = doc["Metadata"];
                if (meta != null)
                {
                    MediaData data = new MediaData();
                    data.Name = meta["Name"].ToString();
                    data.Artist = meta["Singer"].ToString();

                    return data;
                }
            }


            return null;
        }


        public void LoadAllMediaInfo(ICollection<MediaData> set)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            var cursor = coll.FindAll();
            foreach (var doc in cursor)
            {
                BsonValue meta = doc["Metadata"];
                if (meta != null)
                {
                    if (meta.ToBsonDocument().Contains("Name"))
                    {
                        string name = meta["Name"].ToString();

                        if (!string.IsNullOrEmpty(name))
                        {
                            MediaData data = new MediaData();
                            data.Name = name;
                            if (meta.ToBsonDocument().Contains("Singer"))
                            {
                                string singer = meta["Singer"].ToString();
                                data.Artist = singer;
                            }
                            set.Add(data);
                        }
                    }
                    
                }
            }
        }
    }
}
