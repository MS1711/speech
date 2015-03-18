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

        void LoadAllMediaInfo(ICollection<MediaData> set);
    }
}
