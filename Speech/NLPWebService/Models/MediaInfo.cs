using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLPWebService.Models
{
    public enum MediaCategory
    {
        Invalid,
        Music,
        Story,
        Radio
    }

    public class MediaInfo
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Url { get; set; }
        public string MetaName { get; set; }
        public MediaCategory Category { get; set; }

    }
}