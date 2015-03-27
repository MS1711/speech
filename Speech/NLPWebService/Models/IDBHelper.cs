using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPWebService.Models
{
    interface IDBHelper
    {
        MediaInfo GetMediaInfo(Dictionary<string, string> query);

        List<MediaInfo> GetMediaInfoList(Dictionary<string, string> query, int max);

        string TranslateAction(string action);
    }
}
