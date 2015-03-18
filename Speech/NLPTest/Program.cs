using NLPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NL2ML.api;
using log4net;
using System.Diagnostics;
using NL2ML.mediaProvider;
using NL2ML.models;
using NL2ML.dbhelper;

namespace NLPTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Sample3();
            Console.Read();
        }

        private static void Sample6()
        {
            MongoDBHelper h = new MongoDBHelper();
            Console.WriteLine(h.TranslateCommand("打开"));
        }

        private static void Sample5()
        {
            BaiduMediaContentProvider p = new BaiduMediaContentProvider();
            MediaData md = p.GetMusic("菊花台","周杰伦");
        }

        private static void Sample4()
        {
            Process[] ps = Process.GetProcessesByName("wmplayer");
            if (ps != null)
            {
                foreach (Process p in ps)
                {
                    p.Kill();
                }
            } 
        }

        private static void Sample3()
        {
            NL2ML.api.NL2ML ins = NL2ML.api.NL2ML.Instance;
            ins.Process("我想听周杰伦的菊花台");
        }

        static void Sample2()
        {
            Regex reg = new Regex(@"听(?<author>\w+)的(?<song>\w+)");
            string word = @"我想听一下周杰伦GG的菊花台";
            Console.WriteLine(reg.IsMatch(word));
            MatchCollection mcs = reg.Matches(word);
            Console.WriteLine(mcs[0].Groups["author"] + ", " + mcs[0].Groups["song"]);
        }

        static void Sample1()
        {
            string[] s = {
                             "我想听北京交通广播台",
                             "我不建议听周杰伦同学的菊花台",
                             "请把客厅的灯打开",
                             "这首歌真好听",
                         };
            //CWSTagger tag = new CWSTagger("");
            ////Console.WriteLine(tag.tag(s));
            //POSTagger tag2 = new POSTagger(tag, "");

            CNFactory f = CNFactory.GetInstance(@"C:/Temp/");
            CNFactory.loadDict(@"C:\Temp\mydict2.txt");
            CNFactory.loadDict(@"C:\Temp\devicedict2.txt");
            CNFactory.loadDict(@"C:\Temp\verbdict2.txt");
            CNFactory.loadDict(@"C:\Temp\genredict.txt");

            foreach (var item in s)
            {
                DateTime start = DateTime.Now;
                string ss = f.tag2String(item);
                DateTime end = DateTime.Now;
                Console.WriteLine(end.Ticks - start.Ticks);
                Console.WriteLine(ss);
            }

            
        }

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
    }
}
