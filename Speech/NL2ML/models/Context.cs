using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.models
{
    public class Context
    {
        public string RawString { get; set; }
        public string[][] Tags { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Tags[0].Length; i++)
            {
                sb.Append(Tags[0][i] + "/" + Tags[1][i]);
            }
            return RawString + ", " + sb.ToString();
        }
    }
}
