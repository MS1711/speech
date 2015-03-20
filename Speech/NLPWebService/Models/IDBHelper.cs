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

        string TranslateAction(string action);
    }
}
