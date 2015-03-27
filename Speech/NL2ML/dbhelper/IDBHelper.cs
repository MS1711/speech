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

        MediaData GetMediaByCategory(string name, string category);

        MediaData GetRandomMusicByArtist(string name);

        List<MediaData> GetMusicListByArtist(string artist);

        MediaData GetRandomRadioByCategory(string cate);

        MediaData GetRandomByCategory(string cate);

        MediaData GetMediaByName(string suffix);

        CorrectedInfo GetCorrectedSong(string suffix);

        CorrectedInfo GetCorrectedArtist(string suffix);

        List<MediaData> GetMediaListByName(string suffix);

        MediaData GetMusicByArtistAndSong(string artist, string song);

        List<MediaData> GetMediaListByGenericQuery(Dictionary<string, string> query);

        MediaData GetMediaByGenericQuery(Dictionary<string, string> query);
    }
}
