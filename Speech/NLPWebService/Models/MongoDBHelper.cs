using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLPWebService.Models
{
    public class MongoDBHelper : IDBHelper
    {
        private static Random rand = new Random();

        public MediaInfo GetMediaInfo(Dictionary<string, string> items)
        {
            MediaInfo info = new MediaInfo();

            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument(items);
            query.Add("URL", new BsonDocument("$ne", new BsonString("null")));

            long count = coll.Count(query);
            if (count == 0)
            {
                return info;
            }
            var cursor = coll.Find(query);
            cursor.Skip = rand.Next((int)(count - 1));
            cursor.Limit = 1;
            foreach (var doc in cursor)
            {
                BsonValue meta = doc["Metadata"];
                if (doc.Contains("URL"))
                {
                    info.Url = doc["URL"].ToString();
                }
                if (doc.Contains("Name"))
                {
                    info.Name = doc["Name"].ToString();
                }
                if (doc.Contains("Category"))
                {
                    BsonValue v = doc["Category"];
                    if (v != null)
                    {
                        MediaCategory catg = MediaCategory.Invalid;
                        switch (v.ToString())
                        {
                            case "music":
                                {
                                    catg = MediaCategory.Music;
                                    break;
                                }
                            case "story":
                                {
                                    catg = MediaCategory.Story;
                                    break;
                                }
                            case "radio":
                                {
                                    catg = MediaCategory.Radio;
                                    break;
                                }
                        }
                        info.Category = catg;
                    }
                    
                }
                if (meta != null)
                {
                    if (meta.ToBsonDocument().Contains("Singer"))
                    {
                        info.Artist = meta["Singer"].ToString();
                    }
                    if (meta.ToBsonDocument().Contains("Name"))
                    {
                        info.MetaName = meta["Name"].ToString();
                    }
                    break;
                }
            }

            return info;
        }


        public string TranslateAction(string action)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableCommandCollection);

            QueryDocument query = new QueryDocument("Name", action);
            BsonDocument doc = coll.FindOne(query);
            if (doc != null)
            {
                return doc["Command"].ToString();
            }

            return "";
        }


        public List<MediaInfo> GetMediaInfoList(Dictionary<string, string> items, int max)
        {
            List<MediaInfo> list = new List<MediaInfo>();
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument(items);
            query.Add("URL", new BsonDocument("$ne", new BsonString("null")));

            long count = coll.Count(query);
            if (count == 0)
            {
                return list;
            }


            var cursor = coll.Find(query);
            //cursor.Skip = rand.Next((int)(count - 1));
            //cursor.Limit = 1;
            foreach (var doc in cursor)
            {
                if (max <= 0)
                {
                    break;
                }

                max--;
                MediaInfo info = new MediaInfo();
                BsonValue meta = doc["Metadata"];
                if (doc.Contains("URL"))
                {
                    info.Url = doc["URL"].ToString();
                }
                if (doc.Contains("Name"))
                {
                    info.Name = doc["Name"].ToString();
                }
                if (doc.Contains("Category"))
                {
                    BsonValue v = doc["Category"];
                    if (v != null)
                    {
                        MediaCategory catg = MediaCategory.Invalid;
                        switch (v.ToString())
                        {
                            case "music":
                                {
                                    catg = MediaCategory.Music;
                                    break;
                                }
                            case "story":
                                {
                                    catg = MediaCategory.Story;
                                    break;
                                }
                            case "radio":
                                {
                                    catg = MediaCategory.Radio;
                                    break;
                                }
                        }
                        info.Category = catg;
                    }

                }
                if (meta != null)
                {
                    if (meta.ToBsonDocument().Contains("Singer"))
                    {
                        info.Artist = meta["Singer"].ToString();
                    }
                    if (meta.ToBsonDocument().Contains("Name"))
                    {
                        info.MetaName = meta["Name"].ToString();
                    }
                    list.Add(info);
                }
            }

            return list;
        }
    }
}