using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.models
{
    public abstract class Reader
    {
        public void remove()
        {
            throw new Exception("This Iterator<Instance> does not support remove().");
        }

        public abstract bool hasNext();

        public abstract Instance next();
    }
}
