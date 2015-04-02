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

namespace NLPTest
{
    class Program
    {
        private static ILog logger = LogManager.GetLogger("common");

        [DataContract]
        public class AdmAccessToken
        {
            [DataMember]
            public string access_token { get; set; }
            [DataMember]
            public string token_type { get; set; }
            [DataMember]
            public string expires_in { get; set; }
            [DataMember]
            public string scope { get; set; }
        }

        public class AdmAuthentication
        {
            public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            private string clientId;
            private string cientSecret;
            private string request;

            public AdmAuthentication(string clientId, string clientSecret)
            {
                this.clientId = clientId;
                this.cientSecret = clientSecret;
                //If clientid or client secret has special characters, encode before sending request
                this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
            }

            public AdmAccessToken GetAccessToken()
            {
                return HttpPost(DatamarketAccessUri, this.request);
            }

            private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
            {
                //Prepare OAuth request 
                WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                webRequest.ContentLength = bytes.Length;
                using (Stream outputStream = webRequest.GetRequestStream())
                {
                    outputStream.Write(bytes, 0, bytes.Length);
                }
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                    //Get deserialized object from JSON stream
                    AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                    return token;
                }
            }
        }

        static void Main(string[] args)
        {
            Sample3();
            Console.Read();
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

        private static void Sample14()
        {
            AdmAccessToken admToken;
            string headerValue;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            AdmAuthentication admAuth = new AdmAuthentication("MyTTS", "GmMgealz1CtUWi4nLzCNepWPr2U8yeKF3g5mWEkPqYU=");
            try
            {
                admToken = admAuth.GetAccessToken();
                DateTime tokenReceived = DateTime.Now;
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                GetLanguagesForSpeakMethod(headerValue);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        private static void Sample13()
        {
            AdmAccessToken admToken;
            string headerValue;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            AdmAuthentication admAuth = new AdmAuthentication("MyTTS", "GmMgealz1CtUWi4nLzCNepWPr2U8yeKF3g5mWEkPqYU=");
            try
            {
                admToken = admAuth.GetAccessToken();
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                SpeakMethod(headerValue);
            }
            catch (WebException e)
            {
                ProcessWebException(e);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        private static void GetLanguagesForSpeakMethod(string authToken)
        {

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/GetLanguagesForSpeak";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {

                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<string>));
                    List<string> languagesForSpeak = (List<string>)dcs.ReadObject(stream);
                    Console.WriteLine("The languages available for speak are: ");
                    languagesForSpeak.ForEach(a => Console.WriteLine(a));
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }

        private static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 2560;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        private static void SpeakMethod(string authToken)
        {
            string ss = @"好的，马上做";
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Speak?text=" + ss + "&language=zh-chs&format=" + HttpUtility.UrlEncode("audio/wav") + "&options=MaxQuality";

            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = webRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    //FileStream writeStream = new FileStream(@"C:/workspace/female.wav", FileMode.Create, FileAccess.Write);
                    //ReadWriteStream(stream, writeStream);
                    using (SoundPlayer player = new SoundPlayer(stream))
                    {
                        player.PlaySync();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
        private static void ProcessWebException(WebException e)
        {
            Console.WriteLine("{0}", e.ToString());
            // Obtain detailed error information
            string strResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
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
                "关闭客厅的窗帘"//6
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
