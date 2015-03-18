using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.plugins.regex
{
    public class MediaRegexParser : IRegexParser
    {
        public string[][] Pattern
        {
            get
            {
                return new string[][]{
                    new string[]{"1", @"[想|要]听(?<author>\w+)的(?<song>\w+)"},
                    new string[]{"2", @""}
                };
            }
        }

        public Intent Process(int matchId, Dictionary<string, string> keyWords)
        {
            throw new NotImplementedException();
        }

        public Intent SubProcess1(Dictionary<string, string> keyWords)
        {
            throw new NotImplementedException();
        }

        public Intent SubProcess2(Dictionary<string, string> keyWords)
        {
            throw new NotImplementedException();
        }
    }
}
