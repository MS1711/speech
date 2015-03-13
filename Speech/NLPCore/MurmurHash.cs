using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class MurmurHash : AbstractHashCode
    {
        public MurmurHash() { }

        protected byte[] toBytesWithoutEncoding(string str)
        {
            int len = str.Length;
            int pos = 0;
            byte[] buf = new byte[len << 1];
            for (int i = 0; i < len; i++)
            {
                char c = str[i];
                buf[pos++] = (byte)(c & 0xFF);
                buf[pos++] = (byte)(c >> 8);
            }
            return buf;
        }

        public override int hashcode(String str)
        {
            byte[] bytes = toBytesWithoutEncoding(str);
            //return hash32(bytes, bytes.Length);
            return 0;
        }


    }
}
