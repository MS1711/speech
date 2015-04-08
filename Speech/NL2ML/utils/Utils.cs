using NL2ML.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NL2ML.utils
{
    public sealed class Utils
    {
        public static float Leven(string value1, string value2)
        {
            int len1 = value1.Length;
            int len2 = value2.Length;
            int[,] dif = new int[len1 + 1, len2 + 1];
            for (int a = 0; a <= len1; a++)
            {
                dif[a, 0] = a;
            }
            for (int a = 0; a <= len2; a++)
            {
                dif[0, a] = a;
            }
            int temp = 0;
            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (value1[i - 1] == value2[j - 1])
                    { temp = 0; }
                    else
                    {
                        temp = 1;
                    }
                    dif[i, j] = Min(dif[i - 1, j - 1] + temp, dif[i, j - 1] + 1,
                        dif[i - 1, j] + 1);
                }
            }

            float similarity = 1 - (float)dif[len1, len2] / Math.Max(len1, len2);
            return similarity;
        }

        public static int Min(int a, int b, int c)
        {
            int i = a < b ? a : b;
            return i = i < c ? i : c;
        }

        public static string PrintList<T>(IList<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item.ToString() + " ");
            }

            return sb.ToString();
        }

        public static bool IsVagueMusic(string word)
        {
            string[] cate = new string[] { "歌", "歌曲", "音乐" };
            return (from c in cate where c.Equals(word, StringComparison.OrdinalIgnoreCase) select c).Count() > 0;
        }

        public static bool IsVagueStory(string word)
        {
            string[] cate = new string[] { "故事", "童话", "儿童故事" };
            return (from c in cate where c.Equals(word, StringComparison.OrdinalIgnoreCase) select c).Count() > 0;
        }

        public static bool IsVagueRadio(string word)
        {
            string[] cate = new string[] { "广播", "调频", "调幅", "电台", "广播电台" };
            return (from c in cate where c.Equals(word, StringComparison.OrdinalIgnoreCase) select c).Count() > 0;
        }

        public static int[] LocatePOS(Context context, string POS)
        {
            List<int> loc = new List<int>();
            string[][] tags = context.Tags;
            for (int i = 0; i < tags[0].Length; i++)
            {
                if (tags[1][i].Equals(POS))
                {
                    loc.Add(i);
                }
            }

            return loc.ToArray();
        }

        public static Context SubContext(Context context, int start, int end)
        {
            Context cnt = new Context();
            cnt.RawString = "";
            cnt.Tags = null;

            if (start >= context.Tags[0].Length)
            {
                return cnt;
            }

            if (end > context.Tags[0].Length)
            {
                end = context.Tags[0].Length;
            }

            List<List<string>> list = new List<List<string>>();
            list.Add(new List<string>());
            list.Add(new List<string>());

            string[][] tags = context.Tags;
            for (int i = start; i < end; i++)
            {
                list[0].Add(tags[0][i]);
                list[1].Add(tags[1][i]);
            }

            string[][] cpy = new string[2][];
            cpy[0] = list[0].ToArray();
            cpy[1] = list[1].ToArray();

            cnt.Tags = cpy;
            cnt.RawString = string.Join("", cpy[0]);

            return cnt;
        }

        public static string FixSongName(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            StringBuilder sb = new StringBuilder();
            bool start = false;
            for (int i = 0; i < name.Length; i++)
            {
                if (Char.IsLetterOrDigit(name[i]))
                {
                    start = true;
                    sb.Append(name[i]);
                }
                else
                {
                    if (start)
                    {
                        break;
                    }
                }
            }

            return sb.ToString();
        }

        public static string GetTom61InnerAudioLink(string url)
        {
            return url;
            //if (string.IsNullOrEmpty(url))
            //{
            //    return "";
            //}

            //// Create a request for the URL. 
            //WebRequest request = WebRequest.Create(url);
            //// Get the response.
            //using (WebResponse response = request.GetResponse())
            //{
            //    // Display the status.
            //    // Get the stream containing content returned by the server.
            //    Stream dataStream = response.GetResponseStream();
            //    // Open the stream using a StreamReader for easy access.
            //    using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("gb2312")))
            //    {
            //        // Read the content.
            //        string input = reader.ReadToEnd();

            //        Regex regex = new Regex(@"""setMedia"",\s*{mp3:""(?<url>.+)""}");
            //        MatchCollection mcs = regex.Matches(input);
            //        if (mcs.Count > 0)
            //        {
            //            return HttpUtility.UrlPathEncode(mcs[0].Groups["url"].Value);
            //        }
            //    }
            //}
            //return "";
        }

        internal static string TranslateToCategory(string raw)
        {
            if (IsVagueMusic(raw))
            {
                return "music";
            }
            if (IsVagueRadio(raw))
            {
                return "radio";
            }
            if (IsVagueStory(raw))
            {
                return "story";
            }
            
            return "";
        }

        public static string DumpDict<T, V>(Dictionary<T, V> dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict.Keys)
            {
                sb.Append(item + "/" + dict[item] + ", ");
            }

            return "[" + sb.ToString() + "]";
        }
    }
}
