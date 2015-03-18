using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.plugins.nlp
{
    public interface IntentBuilder
    {
        Intent[] GetIntents(Context context);
    }
}
