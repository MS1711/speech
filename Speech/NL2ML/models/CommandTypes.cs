using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.models
{
    public enum Domains
    {
        Invalid,
        Media,
        Weather,
        SmartDevice,
    }

    public enum Actions
    {
        Invalid,
        Open,
        Close,
        Play,
        Pause,
        Stop,
        TurnUp,
        TurnDown,
    }
}
