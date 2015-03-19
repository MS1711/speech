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

        public MediaData GetMusic(string name, string artist)
        {
            return provider.GetMusic(name, artist);
        }

        public MediaData GetMusicByGenre(string genre)
        {
            MediaData data = dbHelper.GetRandomMusicByGenre(genre);
            if (!string.IsNullOrEmpty(data.Name))
            {
                return provider.GetMusic(data.Name, data.Artist);
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
            return provider.GetMusic(songName, "");
        }

        internal MediaData GetRandomMusic()
        {
            MediaData data = dbHelper.GetRandomMusic();
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
            MediaData data = dbHelper.GetRandomMusic();
            if (!string.IsNullOrEmpty(data.Name))
            {
                return provider.GetMusic(data.Name, data.Artist);
            }

            return null;
        }

        internal MediaData GetMediaByCategory(string name, string category)
        {
            return dbHelper.GetMediaByCategory(name, category);
        }

        internal MediaData GetRandomMediaByArtist(string name)
        {
            MediaData data = dbHelper.GetRandomMusicByArtist(name);
            if (!string.IsNullOrEmpty(data.Name))
            {
                return provider.GetMusic(data.Name, data.Artist);
            }

            return null;
        }

        internal MediaData GetRandomRadioByCategory(string cate)
        {
            return dbHelper.GetRandomRadioByCategory(cate);
        }
    }
}
