using NL2ML.consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.utils
{
    sealed class POSUtils
    {
        public static bool HasPOS(string[][] tags, string pos)
        {
            if (tags == null || tags.Length == 0 || tags[1].Length == 0)
            {
                return false;
            }

            for (int i = 0; i < tags[1].Length; i++)
            {
                if (tags[1][i].Equals(pos, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static string[] GetWordsByPOS(string[][] tags, string pos)
        {
            List<string> poss = new List<string>();
            if (tags == null || tags.Length == 0 || tags[1].Length == 0)
            {
                return poss.ToArray();
            }

            for (int i = 0; i < tags[1].Length; i++)
            {
                if (tags[1][i].Equals(pos, StringComparison.OrdinalIgnoreCase))
                {
                    poss.Add(tags[0][i]);
                }
            }
            return poss.ToArray();
        }

        public static bool IsNegtive(string[][] tags, string input)
        {
            string[] negAdv = POSUtils.GetWordsByPOS(tags, POSConstants.Adv);
            return false;
        }

        internal static string GetWordByPOS(string[][] tags, string p)
        {
            string[] ws = GetWordsByPOS(tags, p);
            if (ws != null && ws.Length > 0)
            {
                return ws[0];
            }

            return "";
        }
    }
}
