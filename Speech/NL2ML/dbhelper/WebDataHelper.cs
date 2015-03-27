using log4net;
using Newtonsoft.Json;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NL2ML.dbhelper
{
    public class WebDataHelper : IDBHelper
    {
        private static ILog logger = LogManager.GetLogger("common");

        class ResponseData<T>
        {
            public T Msg { get; set; }
            public int Code { get; set; }
        }

        private static string BaseUrl = "http://ms1711-test01.chinacloudapp.cn/media/{0}?";

        private T GetResponse<T>(string url)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
                // Display the status.
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
                {
                    // Read the content.
                    string txt = reader.ReadToEnd();
                    T t = JsonConvert.DeserializeObject<T>(txt);
                    if (t != null)
                    {
                        logger.Debug("get data from media service: " + t);
                    }
                    return t;
                }
            }
        }

        public string TranslateCommand(string input)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "trans", "action", HttpUtility.UrlEncode(input, Encoding.UTF8));
            ResponseData<string> res = GetResponse<ResponseData<string>>(url);
            return res.Msg;
        }

        public models.MediaData GetRandomMusicByGenre(string genre)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "bygenre", "genre", HttpUtility.UrlEncode(genre, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }

        public models.MediaData GetRandomMusicByGenre(string artist, string genre)
        {
            string url = string.Format(BaseUrl + "{1}={2}&{3}={4}", "byartgenre", 
                "artist", HttpUtility.UrlEncode(artist, Encoding.UTF8),
                "genre", HttpUtility.UrlEncode(genre, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }

        public models.MediaData GetMediaByCategory(string name, string category)
        {
            string url = string.Format(BaseUrl + "{1}={2}&{3}={4}", "byncate",
                "name", HttpUtility.UrlEncode(name, Encoding.UTF8),
                "cate", HttpUtility.UrlEncode(category, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }

        public models.MediaData GetRandomMusicByArtist(string name)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "byartist",
                "artist", HttpUtility.UrlEncode(name, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }

        public models.MediaData GetRandomRadioByCategory(string cate)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "byradiocate",
                "rcate", HttpUtility.UrlEncode(cate, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }

        public models.MediaData GetMediaByName(string suffix)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "byname",
                "name", HttpUtility.UrlEncode(suffix, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }


        public MediaData GetRandomByCategory(string cate)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "bycate",
                "cate", HttpUtility.UrlEncode(cate, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }


        public CorrectedInfo GetCorrectedSong(string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
            {
                return new CorrectedInfo();
            }
            string url = string.Format(BaseUrl + "{1}={2}", "correctsong",
                "song", HttpUtility.UrlEncode(suffix, Encoding.UTF8));
            CorrectedInfo res = GetResponse<CorrectedInfo>(url);
            return res;
        }


        public CorrectedInfo GetCorrectedArtist(string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
            {
                return new CorrectedInfo();
            }

            string url = string.Format(BaseUrl + "{1}={2}", "correctartist",
                "artist", HttpUtility.UrlEncode(suffix, Encoding.UTF8));
            CorrectedInfo res = GetResponse<CorrectedInfo>(url);
            return res;
        }


        public List<MediaData> GetMediaListByName(string suffix)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "bylistname",
                "name", HttpUtility.UrlEncode(suffix, Encoding.UTF8));
            List<MediaData> res = GetResponse<List<MediaData>>(url);
            return res;
        }


        public MediaData GetMusicByArtistAndSong(string artist, string song)
        {
            string url = string.Format(BaseUrl + "{1}={2}&{3}={4}", "byartsong",
                "artist", HttpUtility.UrlEncode(song, Encoding.UTF8),
                "song", HttpUtility.UrlEncode(artist, Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }


        public List<MediaData> GetMusicListByArtist(string artist)
        {
            string url = string.Format(BaseUrl + "{1}={2}", "bylistartist",
                "artist", HttpUtility.UrlEncode(artist, Encoding.UTF8));
            List<MediaData> res = GetResponse<List<MediaData>>(url);
            return res;
        }


        public List<MediaData> GetMediaListByGenericQuery(Dictionary<string, string> query)
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (var item in query.Keys)
	        {
		        keys.Add(item);
                values.Add(query[item]);
	        }
            string url = string.Format(BaseUrl + "{1}={2}&{3}={4}", "byarb",
                "keys", HttpUtility.UrlEncode(string.Join(":", keys.ToArray()), Encoding.UTF8),
                "values", HttpUtility.UrlEncode(string.Join(":", values.ToArray()), Encoding.UTF8));
            List<MediaData> res = GetResponse<List<MediaData>>(url);
            return res;
        }

        public MediaData GetMediaByGenericQuery(Dictionary<string, string> query)
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (var item in query.Keys)
            {
                keys.Add(item);
                values.Add(query[item]);
            }
            string url = string.Format(BaseUrl + "{1}={2}&{3}={4}", "bysarb",
                "keys", HttpUtility.UrlEncode(string.Join(":", keys.ToArray()), Encoding.UTF8),
                "values", HttpUtility.UrlEncode(string.Join(":", values.ToArray()), Encoding.UTF8));
            MediaData res = GetResponse<MediaData>(url);
            return res;
        }
    }
}
