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
    }
}
