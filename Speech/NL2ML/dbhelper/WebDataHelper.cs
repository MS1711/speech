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
        class ResponseData<T>
        {
            public T Msg { get; set; }
            public int Code { get; set; }
        }

        private static string BaseUrl = "http://localhost/media/{0}?";

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

        public void LoadAllArtist(MediaItemInfoCache set)
        {
            throw new NotImplementedException();
        }

        public void LoadAllSong(MediaItemInfoCache mediaItemInfoCache)
        {
            throw new NotImplementedException();
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
    }
}
