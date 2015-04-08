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
using System.Runtime.InteropServices;
using System;
using NL2ML.consts;
using NL2ML.utils;
using System.Net;
using System.IO;
using System.Media;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NL2ML.handlers;
using NL2ML.classifier;

namespace NLPTest
{
    class Program
    {
        private static ILog logger = LogManager.GetLogger("common");

        static void Main(string[] args)
        {
            Sample19();
            Console.Read();
        }

        private static void Sample19()
        {
            SmartHomeClassifier cl = new SmartHomeClassifier();
            Domains dom = cl.GetDomain("播放一首音乐");
        }

        private static void Sample18()
        {
            MediaKeyWordExtractor ext = MediaKeyWordExtractor.Instance;
            string[] artists = ext.GetArtist("播放董文华春天的故事");
            string[] songs = ext.GetSongs("播放董文华春天的故事");
        }

        private static void Sample17()
        {
            RobotHandler h = new RobotHandler();
            string ans = h.GetTulingAnswer("北京到上海今天的航班");
            string abs = h.MakeAbstract(ans);
            Console.WriteLine(abs);
        }

        private static void Sample16()
        {
            //string url = @"http://www.tom61.com/youshengduwu/youeryoushenggushi/index_{0}.html";
            //for (int i = 21; i <= 37; i++)
            //{
            //    string t = string.Format(url, i);
            //    Tom61ContentGrabber.GrabContent(t);
            //}

            Tom61ContentGrabber.GenerateMongoFile();
            
        }

        private static void Sample15()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5 };
            var sub = from i in list where i % 2 == 0 select i into test select test;
        }

        private static void Sample12()
        {
            ServiceReference1.NLPServiceClient n = new ServiceReference1.NLPServiceClient();
            ServiceReference1.ResultInfo r = n.GetResult("我想听北京交通广播");
            logger.Debug(r.Msg);
        }

        private static void Sample10()
        {
            WebDataHelper h = new WebDataHelper();
            MediaData s = h.GetRandomMusicByGenre("摇滚");
            logger.Debug(s);
        }

        private static void Sample9()
        {
            string w = WeatherService.GetWeatherByCityName("北京", DateTime.Now);
        }

        private static void Sample3()
        {
            string[] sample = new string[] {
                //category
                "我想听歌", //0
                "我想听故事", //1
                "我想听广播", //2

                //genre
                "我想听摇滚", //3
                "我想听流行歌曲", //4
                //genre error tolerance
                "我想听流行音乐", //5

                //song name
                "我想听东风破", //6
                //song error tolerance
                "我想听把悲伤留个自己", //7
                "我想听把悲痛留个自己", //8
                
                //xxx的xxx,concrete aritst and song
                "我想听周杰伦的东风破", //9
                "我想听周杰伦演唱的东风破", //10
                //concrete artist and vague category
                "我想听周杰伦的歌", //11
                "我想听郭德纲的相声", //12
                
                //with music suffix
                "我想听东风破这首歌", //13
                "我想听东风破这歌", //14
                //with story suffix
                "我想听白雪公主的故事", //15
                "我想听白雪公主这个故事", //16
                //story error tolerance
                "我想听白菜公主这个故事", //17
                //music with story suffix error tolerance
                "我想听春天的故事", //18
                "我想听小城故事", //19
                "我想听张艾嘉的光阴的故事", //20

                //free talk
                "北京到上海今天的航班"//21
            };

            string[] smartDevice = new string[] { 
                "",
                "打开客厅的灯", //1
                "关闭客厅的灯", //2
                "打开餐厅的灯",//3
                "关闭餐厅的灯",//4
                "打开客厅的窗帘",//5
                "关闭客厅的窗帘",//6
                "打开空气净化器",//7
                "关闭空气净化器",//8
                "查询室内空气状况"//9
            };
            NL2ML.api.NL2ML ins = NL2ML.api.NL2ML.Instance;
            Result res = ins.Process(sample[9]);
            string s = res.Msg;
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
            ServiceReference1.NLPServiceClient vv = new ServiceReference1.NLPServiceClient();
            string[] s = {
                             "北京今天天气如何",
                             "我不建议听周杰伦同学的菊花台",
                             "请把客厅的灯打开",
                             "这首歌真好听",
                         };
            //CWSTagger tag = new CWSTagger("");
            ////Console.WriteLine(tag.tag(s));
            //POSTagger tag2 = new POSTagger(tag, "");

            CNFactory f = CNFactory.GetInstance(@"C:/workspace/nlpdictdata/");
            CNFactory.loadDict(@"C:\workspace/nlpdictdata\mydict2.txt");
            CNFactory.loadDict(@"C:\workspace/nlpdictdata\devicedict2.txt");
            CNFactory.loadDict(@"C:\workspace/nlpdictdata\verbdict2.txt");
            CNFactory.loadDict(@"C:\workspace/nlpdictdata\genredict.txt");

            foreach (var item in s)
            {
                DateTime start = DateTime.Now;
                string ss = f.tag2String(item);
                DateTime end = DateTime.Now;
                Console.WriteLine(end.Ticks - start.Ticks);
                Console.WriteLine(ss);
            }

        }
    }
}
