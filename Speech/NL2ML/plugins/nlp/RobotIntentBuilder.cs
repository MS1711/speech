using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.plugins.nlp
{
    public class RobotIntentBuilder : IntentBuilder
    {
        public Intent[] GetIntents(Context context)
        {
            List<Intent> list = new List<Intent>();

            Intent intent = new Intent();
            intent.Domain = Domains.QA;
            intent.Data = context.RawString;
            list.Add(intent);

            return list.ToArray();
        }
    }
}
