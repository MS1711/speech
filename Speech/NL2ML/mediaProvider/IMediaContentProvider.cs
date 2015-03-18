using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.mediaProvider
{
    interface IMediaContentProvider
    {
        MediaData GetMusic(string name, string artist);

        MediaData GetMusicByGenre(string genre);
    }
}
