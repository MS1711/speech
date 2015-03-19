using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.models
{
    public class MediaData
    {
        public string Url { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        //high definition url
        public string Durl { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", Name, Artist, Genre);
        }
    }
}
