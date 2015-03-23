using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.models
{
    public enum MediaCategory
    {
        Invalid,
        Music,
        Story,
        Radio
    }

    public class MediaData
    {
        public string Url { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string MetaName { get; set; }
        //high definition url
        public string Durl { get; set; }
        public MediaCategory Category { get; set; }

        public bool IsValid() 
        {
            return Url != null || Durl != null;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", Name, Artist, Genre);
        }
    }

    public class CorrectedInfo
    {
        public float Score { get; set; }
        public string Item { get; set; }
        public string Artist { get; set; }
        public string SongName { get; set; }
        public MediaCategory Category { get; set; }
    }
}
