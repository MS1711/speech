using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.utils
{
    interface IPlayer
    {
        int Play(string url);
        int Pause();
        int Resume();
        int Stop();
        int Mute();
        int SetVolume(int volume);
    }
}
