using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class LoadModelException : Exception
    {
        public LoadModelException(string msg) : base(msg)
        {

        }
    }
}
