using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NLPWebService.Models;
using log4net;

namespace NLPWebService.Controllers
{
    [RoutePrefix("media")]
    public class MediaIndexerController : ApiController
    {
        private static ILog logger = LogManager.GetLogger("common");
        private static IDBHelper dbHelper = DBHelperFactory.GetInstance();
        private static MediaItemInfoCache mediaBase = MediaItemInfoCache.Instance;

        [Route("trans")]
        [HttpGet]
        public ResultInfo<string> TranslateAction(string action)
        {
            logger.Debug("in translation");
            ResultInfo<string> res = new ResultInfo<string>();
            res.Msg = dbHelper.TranslateAction(action);
            return res;
        }

        [Route("byartist")]
        [HttpGet]
        public MediaInfo GetRandomMusicByArtist(string artist)
        {
            Dictionary<string, string> dict = new Dictionary<string,string>();
            dict["Metadata.Singer"] = artist;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("bylistartist")]
        [HttpGet]
        public List<MediaInfo> GetMusicListByArtist(string artist)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Metadata.Singer"] = artist;
            List<MediaInfo> info = dbHelper.GetMediaInfoList(dict, 20);

            return info;
        }

        [Route("byarb")]
        [HttpGet]
        public List<MediaInfo> GetMediaListByGeneric(string keys, string values)
        {
            string[] keylist = keys.Split(':');
            string[] vauelist = values.Split(':');
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < keylist.Length; i++)
            {
                dict[keylist[i]] = vauelist[i];
            }

            List<MediaInfo> info = dbHelper.GetMediaInfoList(dict, 20);

            return info;
        }

        [Route("bysarb")]
        [HttpGet]
        public MediaInfo GetMediaByGeneric(string keys, string values)
        {
            string[] keylist = keys.Split(':');
            string[] vauelist = values.Split(':');
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < keylist.Length; i++)
            {
                dict[keylist[i]] = vauelist[i];
            }

            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("bygenre")]
        [HttpGet]
        public MediaInfo GetRandomMusicByGenre(string genre)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Metadata.Genre"] = genre;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("byartgenre")]
        [HttpGet]
        public MediaInfo GetRandomMusicByArtistGenre(string artist, string genre)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Metadata.Singer"] = artist;
            dict["Metadata.Genre"] = genre;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("byncate")]
        [HttpGet]
        public MediaInfo GetMediaByNameCategory(string name, string cate)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Name"] = name;
            dict["Category"] = cate;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("byname")]
        [HttpGet]
        public MediaInfo GetRandomMusicByName(string name)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Name"] = name;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("byartsong")]
        [HttpGet]
        public MediaInfo GetMusicByArtistAndSong(string artist, string song)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Name"] = song;
            dict["Metadata.Singer"] = artist;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("bylistname")]
        [HttpGet]
        public List<MediaInfo> GetMediaListByName(string name)
        {
            List<MediaInfo> list = new List<MediaInfo>();
            string[] cates = { "music", "story", "radio" };
            foreach (var item in cates)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict["Name"] = name;
                dict["Category"] = item;
                MediaInfo info = dbHelper.GetMediaInfo(dict);
                if (!string.IsNullOrEmpty(info.Name))
                {
                    list.Add(info);
                }
            }
           
            return list;
        }

        [Route("bycate")]
        [HttpGet]
        public MediaInfo GetRandomMediaByCategory(string cate)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Category"] = cate;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("byradiocate")]
        [HttpGet]
        public MediaInfo GetRandomRadioByCategory(string rcate)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Category"] = "radio";
            dict["Metadata.RadioCategory"] = rcate;
            MediaInfo info = dbHelper.GetMediaInfo(dict);

            return info;
        }

        [Route("correctartist")]
        [HttpGet]
        public CorrectedInfo GetCorrectedArtist(string artist)
        {
            CorrectedInfo info = mediaBase.GetSimilarArtist(artist);
            return info;
        }

        [Route("correctsong")]
        [HttpGet]
        public CorrectedInfo GetCorrectedSong(string song)
        {
            CorrectedInfo info = mediaBase.GetSimilarSong(song);
            return info;
        }

        [Route("dump")]
        [HttpGet]
        public string DumpArtistSongs()
        {
            Utils.Utilities.DumpLatestArtistAndSong(@"C:\workspace\nlpdictdata\artist", @"C:\workspace\nlpdictdata\songs");
            return "OK";
        }
    }
}