using NL2ML.dbhelper;
using NL2ML.mediaProvider;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.utils
{
    class MediaInfoHelper
    {
        private static MediaInfoHelper instance = new MediaInfoHelper();
        private IMediaContentProvider provider;
        private IDBHelper dbHelper;
        private MediaInfoHelper()
        {
            provider = MediaDataProviderFactory.GetProvider();
            dbHelper = DBHelperFactory.GetInstance();
        }

        internal static MediaInfoHelper Instance
        {
            get { return MediaInfoHelper.instance; }
        }

        public MediaData GetMusicOnline(string name, string artist)
        {
            return provider.GetMusic(name, artist);
        }

        public MediaData GetMusicByGenre(string genre)
        {
            int maxRetry = 10;
            while(maxRetry > 0)
            {
                MediaData data = dbHelper.GetRandomMusicByGenre(genre);
                if (!string.IsNullOrEmpty(data.Name))
                {
                    MediaData t = provider.GetMusic(data.Name, "");
                    if (t != null)
                    {
                        return t;
                    }
                }
                maxRetry--;
            }
            

            return null;
        }

        internal MediaData GetMusicByArtistGenre(string artist, string genre)
        {
            MediaData data = dbHelper.GetRandomMusicByGenre(artist, genre);
            if (!string.IsNullOrEmpty(data.Name))
            {
                return provider.GetMusic(data.Name, data.Artist);
            }

            return null;
        }

        internal MediaData GetMusicByName(string songName)
        {
            MediaData data = dbHelper.GetMediaByCategory(songName, "music");
            if (!string.IsNullOrEmpty(data.Name))
            {
                return provider.GetMusic(data.Name, data.Artist);
            }

            return null;
        }

        internal MediaData GetMusicByGenre(string genre, string songName)
        {
            if (!string.IsNullOrEmpty(songName))
            {
                return provider.GetMusic(songName, "");
            }
            else
            {
                return GetMusicByGenre(genre);
            }
        }

        internal MediaData GetRandomMediaByCategory(string category)
        {
            int maxRetry = 10;
            while (maxRetry > 0)
            {
                MediaData data = dbHelper.GetRandomByCategory(category);
                if (data != null && !string.IsNullOrEmpty(data.Name))
                {
                    if (data.Category == MediaCategory.Music)
                    {
                        MediaData t = provider.GetMusic(data.Name, "");
                        if (t != null)
                        {
                            return t;
                        }
                    }
                    else
                    {
                        return data;
                    }
                    
                }
                maxRetry--;
            }
            return null;
        }

        internal MediaData GetMediaByCategory(string name, string category)
        {
            return dbHelper.GetMediaByCategory(name, category);
        }

        internal MediaData GetRandomMediaByArtist(string name)
        {
            int maxRetry = 10;
            while (maxRetry > 0)
            {
                MediaData item = dbHelper.GetRandomMusicByArtist(name);
                MediaData t = provider.GetMusic(item.Name, item.Artist);
                if (t != null)
                {
                    return t;
                }

            }

            return null;
        }

        internal MediaData GetRandomRadioByCategory(string cate)
        {
            return dbHelper.GetRandomRadioByCategory(cate);
        }

        internal MediaData GetMediaByName(string suffix)
        {
            MediaData data = dbHelper.GetMediaByName(suffix);
            if (data == null || data.Category == MediaCategory.Music)
            {
                data = provider.GetMusic(suffix, "");
            }
            
            return data;
        }

        internal List<MediaData> GetMediaListByName(string suffix)
        {
            List<MediaData> data = dbHelper.GetMediaListByName(suffix);
            foreach (var item in data)
            {
                if (item == null || item.Category == MediaCategory.Music)
                {
                    MediaData d = provider.GetMusic(suffix, "");
                    if (!string.IsNullOrEmpty(d.Url))
                    {
                        item.Url = d.Url;
                    }
                    if (!string.IsNullOrEmpty(d.Durl))
                    {
                        item.Durl = d.Durl;
                    }
                }
            }
            
            return data;
        }

        internal List<MediaData> GetMediaListByGenericQuery(Dictionary<string, string> query)
        {
            List<MediaData> data = new List<MediaData>();
            string mode = "S";
            if (query.ContainsKey("Mode"))
            {
                mode = query["Mode"];
                query.Remove("Mode");
            }
            
            if (mode.Equals("S"))
            {
                query.Remove("Mode");
                MediaData t = dbHelper.GetMediaByGenericQuery(query);
                data.Add(t);
            }
            else if (mode.Equals("M"))
            {
                data.AddRange(dbHelper.GetMediaListByGenericQuery(query));
            }

            foreach (var item in data)
            {
                if (item == null || item.Category == MediaCategory.Music)
                {
                    if (string.IsNullOrEmpty(item.Url) && string.IsNullOrEmpty(item.Durl))
                    {
                        MediaData d = provider.GetMusic(item.Name, item.Artist);
                        if (d == null)
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(d.Url))
                        {
                            item.Url = d.Url;
                        }
                        if (!string.IsNullOrEmpty(d.Durl))
                        {
                            item.Durl = d.Durl;
                        }
                    }
                    
                }
            }

            return data;
        }

        internal MediaData GetMediaByGenericQuery(Dictionary<string, string> query)
        {
            MediaData item = dbHelper.GetMediaByGenericQuery(query);

            if (item == null || item.Category == MediaCategory.Music)
            {
                MediaData d = provider.GetMusic(item.Name, item.Artist);
                if (!string.IsNullOrEmpty(d.Url))
                {
                    item.Url = d.Url;
                }
                if (!string.IsNullOrEmpty(d.Durl))
                {
                    item.Durl = d.Durl;
                }
            }


            return item;
        }

        internal CorrectedInfo GetSimilarSong(string suffix)
        {
            return dbHelper.GetCorrectedSong(suffix);
        }

        internal CorrectedInfo GetSimilarArtist(string suffix)
        {
            return dbHelper.GetCorrectedArtist(suffix);
        }

        internal MediaData GetMusicByArtistAndSong(string artist, string song)
        {
            MediaData data = dbHelper.GetMusicByArtistAndSong(artist, song);
            if (data == null || data.Category == MediaCategory.Music)
            {
                data = provider.GetMusic(data.Name, data.Artist);
            }

            return data;
        }
    }
}
