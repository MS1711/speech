using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class Sentenizer
    {
        private static char[] puncs = new char[] { '。', '？', '！', '；' };

        /**
         * 根据标点符号进行断句
         * @param puncs
         */

        public static void setPuncs(char[] puncs)
        {
            Sentenizer.puncs = puncs;
        }

        public static string[] split(string sent)
        {
            List<int> plist = new List<int>();
            int p = 0;
            for (int i = 0; i < puncs.Length; i++)
            {
                p = sent.IndexOf(puncs[i]);
                while (p != -1)
                {
                    plist.Add(p);
                    p = sent.IndexOf(puncs[i], p + 1);
                }
            }
            plist.Sort();
            if (plist.Count != 0)
            {
                p = plist[plist.Count - 1];
                if (p < sent.Length - 1)
                    plist.Add(sent.Length - 1);
            }
            else
            {
                plist.Add(sent.Length - 1);
            }

            string[] ret = new string[plist.Count];
            p = 0;
            for (int i = 0; i < plist.Count; i++)
            {
                ret[i] = sent.Substring(p, plist[i] + 1 - p);
                p = plist[i] + 1;
            }
            plist.Clear();

            return ret;
        }
    }
}
