using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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


        public MediaData GetRandomMusicByGenre(string artist, string genre)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument("Metadata.Genre", genre);
            query.Add("Metadata.Singer", artist);

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


        public MediaData GetRandomMusic()
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument("Category", "music");

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


        public void LoadAllArtist(MediaItemInfoCache artistList)
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
                    if (meta.ToBsonDocument().Contains("Singer"))
                    {
                        string singer = meta["Singer"].ToString();
                        if (!string.IsNullOrEmpty(singer))
                        {
                            artistList.AddArtist(singer.Trim());
                        }            
                    }

                }
            }
        }

        public void LoadAllSong(MediaItemInfoCache mediaItemInfoCache)
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
                        string singer = meta["Name"].ToString();
                        if (!string.IsNullOrEmpty(singer))
                        {
                            mediaItemInfoCache.AddSong(singer.Trim());
                        }
                    }

                }
            }
        }


        public MediaData GetMediaByCategory(string name, string category)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument("Category", category);
            query.Add("Name", name);

            BsonDocument doc = coll.FindOne(query);
            if (doc != null)
            {
                MediaData data = new MediaData();
                data.Url = doc["URL"].ToString();
                return data; 
            }


            return null;
        }


        public MediaData GetRandomMusicByArtist(string name)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument("Metadata.Singer", name);
            query.Add("Category", "music");

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


        public MediaData GetRandomRadioByCategory(string cate)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            QueryDocument query = new QueryDocument("Metadata.RadioCategory", cate);
            query.Add("Category", "radio");

            long count = coll.Count(query);
            if (count == 0)
            {
                return null;
            }
            var cursor = coll.Find(query);
            cursor.Skip = rand.Next((int)(count - 1));
            cursor.Limit = 1;
            foreach (var doc in cursor)
            {
                BsonValue meta = doc["Metadata"];
                if (meta != null)
                {
                    MediaData data = new MediaData();
                    data.Url = doc["URL"].ToString();

                    return data;
                }
            }


            return null;
        }
    }
}
