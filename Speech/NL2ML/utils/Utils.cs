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

        public static string GetTom61InnerAudioLink(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }

            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
                // Display the status.
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("gb2312")))
                {
                    // Read the content.
                    string input = reader.ReadToEnd();

                    Regex regex = new Regex(@"""setMedia"",\s*{mp3:""(?<url>.+)""}");
                    MatchCollection mcs = regex.Matches(input);
                    if (mcs.Count > 0)
                    {
                        return HttpUtility.UrlPathEncode(mcs[0].Groups["url"].Value);
                    }
                }
            }
            return "";
        }
    }
}
