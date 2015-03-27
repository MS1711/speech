using MongoDB.Bson;
using MongoDB.Driver;
using NLPWebService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPWebService.Utils
{
    sealed class Utilities
    {
        public static float Leven(string value1, string value2)
        {
            int len1 = value1.Length;
            int len2 = value2.Length;
            int[,] dif = new int[len1 + 1, len2 + 1];
            for (int a = 0; a <= len1; a++)
            {
                dif[a, 0] = a;
            }
            for (int a = 0; a <= len2; a++)
            {
                dif[0, a] = a;
            }
            int temp = 0;
            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (value1[i - 1] == value2[j - 1])
                    { temp = 0; }
                    else
                    {
                        temp = 1;
                    }
                    dif[i, j] = Min(dif[i - 1, j - 1] + temp, dif[i, j - 1] + 1,
                        dif[i - 1, j] + 1);
                }
            }

            float similarity = 1 - (float)dif[len1, len2] / Math.Max(len1, len2);
            return similarity;
        }

        public static int Min(int a, int b, int c)
        {
            int i = a < b ? a : b;
            return i = i < c ? i : c;
        }

        public static string PrintList<T>(IList<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item.ToString() + " ");
            }

            return sb.ToString();
        }

        public static void DumpLatestArtistAndSong(string artistpath, string songpath)
        {
            DumpLatestArtist(artistpath);
            DumpLatestSong(songpath);
        }

        private static void DumpLatestSong(string songpath)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            using (StreamWriter sw = new StreamWriter(songpath, false, Encoding.UTF8))
            {
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
                                sw.WriteLine(singer.Trim());
                            }
                        }

                    }
                }
            }
        }

        private static void DumpLatestArtist(string artistpath)
        {
            MongoClient client = new MongoClient(MongoDBConstants.ConnString); // connect to localhost
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(MongoDBConstants.DBName);
            MongoCollection<BsonDocument> coll = db.GetCollection(MongoDBConstants.TableMediaCollection);

            using (StreamWriter sw = new StreamWriter(artistpath, false, Encoding.UTF8))
            {
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
                                sw.WriteLine(singer.Trim());
                            }
                        }

                    }
                }
            }
            
        }


    }
}
