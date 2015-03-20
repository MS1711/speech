using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.dbhelper
{
    interface IDBHelper
    {
        string TranslateCommand(string input);

        MediaData GetRandomMusicByGenre(string genre);

        MediaData GetRandomMusicByGenre(string artist, string genre);

        void LoadAllArtist(MediaItemInfoCache set);

        void LoadAllSong(MediaItemInfoCache mediaItemInfoCache);

        MediaData GetMediaByCategory(string name, string category);

        MediaData GetRandomMusicByArtist(string name);

        MediaData GetRandomRadioByCategory(string cate);

        MediaData GetRandomByCategory(string cate);

        MediaData GetMediaByName(string suffix);
    }
}
