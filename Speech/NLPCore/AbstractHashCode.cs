using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public abstract class AbstractHashCode
    {
        public abstract int hashcode(string str);
    }
}
