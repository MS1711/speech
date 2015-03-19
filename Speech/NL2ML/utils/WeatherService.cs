using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace NL2ML.utils
{
    public class WeatherService
    {
        private static Random rand = new Random();
        public static string URL = "http://wthrcdn.etouch.cn/WeatherApi?city={0}";

        public static string GetWeatherByCityName(string city, DateTime fromDate)
        {
            return GetWeatherByCityName(city, fromDate, fromDate);
        }

        public static string GetWeatherByCityName(string city, DateTime fromDate, DateTime endDate)
        {
            string queryUrl = URL + city;
            string status = "";
            WebRequest request = WebRequest.Create(string.Format(URL,
                HttpUtility.UrlEncode(city, Encoding.UTF8)));
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
                // Display the status.
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                using (var zipStream =
                          new System.IO.Compression.GZipStream(dataStream, System.IO.Compression.CompressionMode.Decompress))
                {
                    // Open the stream using a StreamReader for easy access.
                    using (StreamReader reader = new StreamReader(zipStream, Encoding.UTF8))
                    {
                        // Read the content.
                        string restring = reader.ReadToEnd();
                        status = DigestWeatherInfo(restring, fromDate, endDate, fromDate.ToShortDateString().Equals(endDate.ToShortDateString())
                            && fromDate.ToShortDateString().Equals(DateTime.Now.ToShortDateString()));
                    }
                }
                
            }

            return status;
        }

        private static string DigestWeatherInfo(string restring, DateTime fromDate, DateTime endDate, bool details)
        {
            StringBuilder sb = new StringBuilder();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(restring);
            XmlNode root = doc.DocumentElement;

            long gap = (endDate.DayOfYear - fromDate.DayOfYear);
            long gap2 = gap;
            string highLowTemp = "{0}{1},{2},";
            //string day = new SimpleDateFormat("M月dd日").format(fromDate);

            string high = "";
            string low = "";
            string dayInfo = "";
            string nightInfo = "";
            string city = "";

            string datePattern = fromDate.Day + "日";

            XmlNodeList weathers = root.SelectNodes("//forecast/weather");
            city = root.SelectSingleNode("//city").InnerText;
            sb.Append(city);
            bool start = false;
            for (int i = 0; i < weathers.Count; i++)
            {
                XmlNode node = weathers[i];
                string weatherDay = node.SelectSingleNode("descendant::date").InnerText;//(string) xpath.evaluate("date/text()",node, XPathConstants.STRING);

                if (weatherDay.StartsWith(datePattern) || (start && gap >= 0))
                {
                    start = true;
                    high = node.SelectSingleNode("descendant::high").InnerText;
                    high = high.Replace("高温", "最高温度");
                    low = node.SelectSingleNode("descendant::low").InnerText;
                    low = low.Replace("低温", "最低温度");
                    if (gap2 == 0)
                    {
                        weatherDay = GetDayDesc(fromDate);
                    }
                    else
                    {
                        DateTime d = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day + (int)(gap2 - gap));
                        weatherDay = d.ToString("M月d日");
                    }
                    string info = string.Format(highLowTemp, weatherDay, high, low);
                    sb.Append(info);

                    string dtype = node.SelectSingleNode("descendant::day/type").InnerText;//(string) xpath.evaluate("ay/type/text()", node,XPathConstants.STRING);
                    if (dtype == null || dtype.Length == 0)
                    {
                        dtype = "晴";
                    }
                    string dinfo = "白天" + dtype + ",";
                    string wind = node.SelectSingleNode("descendant::day/fengxiang").InnerText;//(string) xpath.evaluate("day/fengxiang/text()", node,XPathConstants.STRING);
                    if (wind != null && wind.Length != 0 && wind.IndexOf("无") == -1)
                    {
                        dinfo += wind + ",";
                        string windStrongness = node.SelectSingleNode("descendant::day/fengli").InnerText;//(string) xpath.evaluate("day/fengli/text()", node,XPathConstants.STRING);
                        if (windStrongness != null && windStrongness.Length != 0)
                        {
                            dinfo += windStrongness + ",";
                        }

                    }
                    sb.Append(dinfo);


                    string ntype = node.SelectSingleNode("descendant::night/type").InnerText;//(string) xpath.evaluate("night/type/text()", node,XPathConstants.STRING);
                    if (ntype == null || ntype.Length == 0)
                    {
                        ntype = "晴";
                    }
                    string ninfo = "晚上" + ntype + ",";

                    wind = node.SelectSingleNode("descendant::night/fengxiang").InnerText;//(string) xpath.evaluate("night/fengxiang/text()", node,XPathConstants.STRING);
                    if (wind != null && wind.Length != 0 && wind.IndexOf("无") == -1)
                    {
                        ninfo += wind + ",";
                        string windStrongness = node.SelectSingleNode("descendant::night/fengli").InnerText;//(string) xpath.evaluate("night/fengli/text()", node,XPathConstants.STRING);
                        if (windStrongness != null && windStrongness.Length != 0)
                        {
                            ninfo += windStrongness + ",";
                        }

                    }
                    sb.Append(ninfo);

                    gap--;
                }
            }

            if (details)
            {
                string envPattern = "今天PM2.5指数为{0},属于{1},{2}建议{3},";
                string pm25 = root.SelectSingleNode("//environment/pm25").InnerText;//(string) xpath.evaluate("/resp/environment/pm25/text()",document, XPathConstants.STRING);
                string quality = root.SelectSingleNode("//environment/quality").InnerText; //(string)xpath.evaluate("/resp/environment/quality/text()", document, XPathConstants.STRING);
                string major = root.SelectSingleNode("//environment/MajorPollutants").InnerText; //(string)xpath.evaluate("/resp/environment/MajorPollutants/text()", document, XPathConstants.STRING);
                if (major != null && major.Length > 0)
                {
                    major = "主要污染物为" + major + ",";
                }
                string suggest = root.SelectSingleNode("//environment/suggest").InnerText;//(string) xpath.evaluate("/resp/environment/suggest/text()",document, XPathConstants.STRING);
                sb.Append(string.Format(envPattern, pm25, quality, major, suggest));

                string zhiShuPattern = "今天{0}为,{1},{2}";
                XmlNodeList zhishuList = root.SelectNodes("//zhishus/zhishu");//(NodeList) xpath.evaluate("/resp/zhishus/zhishu", document, XPathConstants.NODESET);
                if (zhishuList.Count > 0)
                {
                    int ind = rand.Next(zhishuList.Count);
                    XmlNode zhishu = zhishuList[ind];
                    string zhiShuName = zhishu.SelectSingleNode("descendant::name").InnerText;//(string) xpath.evaluate("name/text()",zhishu, XPathConstants.STRING);
                    string zhishuValue = zhishu.SelectSingleNode("descendant::value").InnerText;//(string) xpath.evaluate("value/text()",zhishu, XPathConstants.STRING);
                    string detail = zhishu.SelectSingleNode("descendant::detail").InnerText;//(string) xpath.evaluate("detail/text()",zhishu, XPathConstants.STRING);
                    sb.Append(string.Format(zhiShuPattern, zhiShuName, zhishuValue, detail));
                }
            }


            return sb.ToString();
        }

        private static string GetDayDesc(DateTime fromDate)
        {
            if (fromDate == null)
            {
                return "";
            }
            int gap = fromDate.DayOfYear - DateTime.Now.DayOfYear;
            switch (gap)
            {
                case 0:
                    return "今天";
                case 1:
                    return "明天";
                case 2:
                    return "后天";
            }

            return fromDate.ToString("M月d日");
        }
    }
}
