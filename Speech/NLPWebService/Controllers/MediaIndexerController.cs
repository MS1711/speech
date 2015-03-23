using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NLPWebService.Models;

namespace NLPWebService.Controllers
{
    [RoutePrefix("media")]
    public class MediaIndexerController : ApiController
    {
        private static IDBHelper dbHelper = DBHelperFactory.GetInstance();
        private static MediaItemInfoCache mediaBase = MediaItemInfoCache.Instance;

        [Route("trans")]
        [HttpGet]
        public ResultInfo<string> TranslateAction(string action)
        {
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
    }
}