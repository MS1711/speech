using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NLPTest
{
    class Tom61ContentGrabber
    {
        private static ILog logger = LogManager.GetLogger("common");

        public static void GrabContent(string url)
        {
            logger.Debug("get content from " + url);
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
                // Display the status.
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("GBK")))
                {
                    // Read the content.
                    string str = reader.ReadToEnd();
                    FileStream fs = new FileStream(@"C:/workspace/tom61_story.js", FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

                    //<a href="/youshengduwu/youeryoushenggushi/2013-11-24/52260.html" target="_blank" title="新狐假虎威">71.新狐假虎威</a>
                    Regex regex = new Regex(@"\<a href=""(?<url>.+)"" target=""_blank"" title=""(?<title>\w+)""\>");
                    MatchCollection mcs = regex.Matches(str);
                    for (int i = 0; i < mcs.Count; i++)
                    {
                        string name = mcs[i].Groups["title"].Value;
                        string rurl = mcs[i].Groups["url"].Value;
                        string mp3url = GetInner(rurl);

                        sw.WriteLine(name + "-" + mp3url);
                        logger.Debug("get url: " + name + "-" + mp3url);
                    }

                    sw.Flush();
                    fs.Flush();
                    sw.Close();
                }

            }
        }

        private static string GetInner(string rurl)
        {
            string prefix = @"http://www.tom61.com";
            string url = prefix + rurl;

            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
                // Display the status.
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("GBK")))
                {
                    // Read the content.
                    string str = reader.ReadToEnd();

                    //<iframe src="/e/DownSys/play/?classid=137&id=15121&pathid=0" id="play" name="play" frameBorder=0 scrolling=no width="100%" height="60px"></iframe>
                    Regex regex = new Regex(@"\<iframe src=""(?<url>.+)"" id=""play"" name=""play"" frameBorder=0 scrolling=no");
                    MatchCollection mcs = regex.Matches(str);
                    if (mcs.Count > 0)
                    {
                        string mp3url = prefix + mcs[0].Groups["url"].Value;
                        return GetTom61InnerAudioLink(mp3url);
                    }
                }
            }

            return "";
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

        public static void GenerateMongoFile()
        {
            /*
             * > db.mediaCollection.findOne({"Metadata.StoryCategory":{$ne:null}});
                {
                        "_id" : ObjectId("54c6254be8591539a43b7410"),
                        "Category" : "story",
                        "Name" : "白雪公主",
                        "URL" : "http://www.tom61.com/e/DownSys/play/?classid=463&id=10688&pathid=0",
                        "Language" : null,
                        "Region" : null,
                        "Metadata" : {
                                "_id" : ObjectId("000000000000000000000000"),
                                "StoryCategory" : "童话故事",
                                "Author" : null
                        },
                        "NamePinyin" : "bai xue gong zhu"
                }
             */
            string file = @"C:/workspace/tom61_story.js";
            string pattern = "db.mediaCollection.save({{\"Category\" : \"story\", \"Name\":\"{0}\",\"URL\":\"{1}\", \"Metadata\":{{\"StoryCategory\":\"童话故事\"}}}});";
            using (StreamWriter sw = new StreamWriter(@"C:/workspace/mediaCollection.js", true, Encoding.UTF8))
            {
                using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {

                        string name = line.Substring(0, line.IndexOf("-"));
                        string url = line.Substring(line.IndexOf("-") + 1);
                        if (url.StartsWith("http:"))
                        {
                            string js = string.Format(pattern, name, url);
                            sw.WriteLine(js);
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            
        }
    }
}
