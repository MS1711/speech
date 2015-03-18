using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.plugins.regex
{
    public interface IRegexParser
    {
        string[][] Pattern { get; }
        Intent Process(int matchId, Dictionary<string, string> keyWords);
    }
}
