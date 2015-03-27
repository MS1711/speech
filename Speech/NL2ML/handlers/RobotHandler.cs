using Newtonsoft.Json;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NL2ML.handlers
{

    public class RobotHandler : IIntentHandler
    {
        private static string KEY = "f4fcea951c6a9daf4aa498fc98e2461b";
        private static string URL = "http://www.tuling123.com/openapi/api?key={0}&info={1}";

        public Result handle(Intent intent)
        {
            Result res = new Result();
            res.IsOK = false;

            if (intent == null || intent.Domain != Domains.QA)
            {
                return res;
            }

            string question = intent.Data as string;
            string rawAnswer = GetTulingAnswer(question);
            if (!string.IsNullOrEmpty(rawAnswer))
            {
                string abs = MakeAbstract(rawAnswer);
                res.IsOK = true;
                res.Msg = abs;
            }
            return res;
        }

        public string MakeAbstract(string rawAnswer)
        {
            string returnResult = null;
            StringBuilder buildResult = new StringBuilder();
            CommonResult commonResult = JsonConvert.DeserializeObject<CommonResult>(rawAnswer);
            if (commonResult.getCode() == 100000)
            {//文本类型
                returnResult = commonResult.getText();
            }
            else if (commonResult.getCode() == 200000)
            {//url类型
                UrlResult urlResult = new UrlResult();
                urlResult = JsonConvert.DeserializeObject<UrlResult>(rawAnswer);
                buildResult.Append(urlResult.getText() + "。");
                buildResult.Append("具体链接地址是：" + urlResult.getUrl());
                returnResult = buildResult.ToString();
            }
            else if (commonResult.getCode() == 302000)
            {//新闻类型
                NewsResult newsResult = new NewsResult();
                newsResult = JsonConvert.DeserializeObject<NewsResult>(rawAnswer);
                buildResult.Append(newsResult.getText() + "。");
                for (int i = 0; i < newsResult.getList().Count; i++)
                {
                    News news = newsResult.getList()[i];
                    buildResult.Append("。" + news.getSource() + "," + news.getArticle());
                }
                returnResult = buildResult.ToString();
            }
            else if (commonResult.getCode() == 305000)
            {//列车类型
                TrainResult trainResult = new TrainResult();
                trainResult = JsonConvert.DeserializeObject<TrainResult>(rawAnswer);
                buildResult.Append(trainResult.getText() + "。");
                for (int i = 0; i < trainResult.getList().Count; i++)
                {
                    Train train = trainResult.getList()[i];
                    buildResult.Append("编号：" + train.getTrainnum() + ",从" + train.getStarttime() + "始发自" +
                    train.getStart() + ",在" + train.getEndtime() + "到达" + train.getTerminal() + "。");
                }
                returnResult = buildResult.ToString();
            }
            else if (commonResult.getCode() == 306000)
            {//航班类型
                FlightResult flightResult = new FlightResult();
                flightResult = JsonConvert.DeserializeObject<FlightResult>(rawAnswer);
                buildResult.Append(flightResult.getText() + "。");
                for (int i = 0; i < flightResult.getList().Count; i++)
                {
                    Flight flight = flightResult.getList()[i];
                    buildResult.Append(flight.getFlight() + flight.getStarttime() + "出发，" + flight.getEndtime() + "到达。");
                }
                returnResult = buildResult.ToString();
            }
            else if (commonResult.getCode() == 308000)
            {//电影信息，视频信息,菜谱
                FilmResult filmResult = new FilmResult();
                filmResult = JsonConvert.DeserializeObject<FilmResult>(rawAnswer);
                buildResult.Append(filmResult.getText() + "。");
                for (int i = 0; i < filmResult.getList().Count; i++)
                {
                    Film film = filmResult.getList()[i];
                    buildResult.Append(film.getName() + "," + film.getInfo() + ",详情链接为" + film.getDetailurl() + "。");
                }
                returnResult = buildResult.ToString();
            }
            else if (commonResult.getCode() == 309000)
            {//酒店
                HotelResult hotelResult = new HotelResult();
                hotelResult = JsonConvert.DeserializeObject<HotelResult>(rawAnswer);
                buildResult.Append(hotelResult.getText() + "。");
                for (int i = 0; i < hotelResult.getList().Count; i++)
                {
                    Hotel hotel = hotelResult.getList()[i];
                    buildResult.Append(hotel.getName() + "，价钱" + hotel.getPrice() + "," + hotel.getSatisfaction() + "。");
                }
                returnResult = buildResult.ToString();
            }
            else if (commonResult.getCode() == 311000)
            {//价钱
                PriceResult priceResult = new PriceResult();
                priceResult = JsonConvert.DeserializeObject<PriceResult>(rawAnswer);
                buildResult.Append(priceResult.getText() + "。");
                for (int i = 0; i < priceResult.getList().Count; i++)
                {
                    Price price = priceResult.getList()[i];
                    buildResult.Append(price.getName() + "，价钱" + price.getPrice() + "。");
                }
                returnResult = buildResult.ToString();
            }
            else if (commonResult.getCode() == 40002)
            {//文本类型
                returnResult = commonResult.getText();
            }
            else
            {
                returnResult = "主人我还不具备这个功能哦";
            }
            return returnResult;
        }

        public string GetTulingAnswer(string question)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(string.Format(URL, KEY,
                HttpUtility.UrlEncode(question, Encoding.UTF8)));
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
                // Display the status.
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
                {
                    // Read the content.
                    string resp = reader.ReadToEnd();
                    return resp;
                }
            }
        }
    }

    public class CommonResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public int getCode()
        {
            return code;
        }
        public string getText()
        {
            return text;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setText(string text)
        {
            this.text = text;
        }

    }

    public class Film
    {
        public string name { get; set; }
        public string info { get; set; }
        public string detailurl { get; set; }
        public string icon { get; set; }
        public string getDetailurl()
        {
            return detailurl;
        }
        public string getIcon()
        {
            return icon;
        }
        public string getInfo()
        {
            return info;
        }
        public string getName()
        {
            return name;
        }
        public void setDetailurl(string detailurl)
        {
            this.detailurl = detailurl;
        }
        public void setIcon(string icon)
        {
            this.icon = icon;
        }
        public void setInfo(string info)
        {
            this.info = info;
        }
        public void setName(string name)
        {
            this.name = name;
        }
    }
    public class FilmResult : CommonResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public List<Film> list { get; set; }
        public int getCode()
        {
            return code;
        }
        public List<Film> getList()
        {
            return list;
        }
        public string getText()
        {
            return text;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setList(List<Film> list)
        {
            this.list = list;
        }
        public void setText(string text)
        {
            this.text = text;
        }
    }

    public class Flight
    {
        public string flight { get; set; }
        public string route { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string state { get; set; }
        public string detailurl { get; set; }
        public string icon { get; set; }
        public string getDetailurl()
        {
            return detailurl;
        }
        public string getEndtime()
        {
            return endtime;
        }
        public string getFlight()
        {
            return flight;
        }
        public string getIcon()
        {
            return icon;
        }
        public string getRoute()
        {
            return route;
        }
        public string getStarttime()
        {
            return starttime;
        }
        public string getState()
        {
            return state;
        }
        public void setDetailurl(string detailurl)
        {
            this.detailurl = detailurl;
        }
        public void setEndtime(string endtime)
        {
            this.endtime = endtime;
        }
        public void setFlight(string flight)
        {
            this.flight = flight;
        }
        public void setIcon(string icon)
        {
            this.icon = icon;
        }
        public void setRoute(string route)
        {
            this.route = route;
        }
        public void setStarttime(string starttime)
        {
            this.starttime = starttime;
        }
        public void setState(string state)
        {
            this.state = state;
        }
    }
    public class FlightResult : CommonResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public List<Flight> list { get; set; }
        public int getCode()
        {
            return code;
        }
        public List<Flight> getList()
        {
            return list;
        }
        public string getText()
        {
            return text;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setList(List<Flight> list)
        {
            this.list = list;
        }
        public void setText(string text)
        {
            this.text = text;
        }
    }

    public class Hotel
    {
        public string name { get; set; }
        public string price { get; set; }
        public string satisfaction { get; set; }
        public string count { get; set; }
        public string detailurl { get; set; }
        public string icon { get; set; }
        public string getCount()
        {
            return count;
        }
        public string getDetailurl()
        {
            return detailurl;
        }
        public string getIcon()
        {
            return icon;
        }
        public string getName()
        {
            return name;
        }
        public string getPrice()
        {
            return price;
        }
        public string getSatisfaction()
        {
            return satisfaction;
        }
        public void setCount(string count)
        {
            this.count = count;
        }
        public void setDetailurl(string detailurl)
        {
            this.detailurl = detailurl;
        }
        public void setIcon(string icon)
        {
            this.icon = icon;
        }
        public void setName(string name)
        {
            this.name = name;
        }
        public void setPrice(string price)
        {
            this.price = price;
        }
        public void setSatisfaction(string satisfaction)
        {
            this.satisfaction = satisfaction;
        }
    }

    public class HotelResult : CommonResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public List<Hotel> list { get; set; }
        public int getCode()
        {
            return code;
        }
        public List<Hotel> getList()
        {
            return list;
        }
        public string getText()
        {
            return text;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setList(List<Hotel> list)
        {
            this.list = list;
        }
        public void setText(string text)
        {
            this.text = text;
        }
    }

    public class News
    {
        public string article { get; set; }
        public string source { get; set; }
        public string detailurl { get; set; }
        public string icon { get; set; }
        public string getArticle()
        {
            return article;
        }
        public string getDetailurl()
        {
            return detailurl;
        }
        public string getIcon()
        {
            return icon;
        }
        public string getSource()
        {
            return source;
        }
        public void setArticle(string article)
        {
            this.article = article;
        }
        public void setDetailurl(string detailurl)
        {
            this.detailurl = detailurl;
        }
        public void setIcon(string icon)
        {
            this.icon = icon;
        }
        public void setSource(string source)
        {
            this.source = source;
        }
    }
    public class NewsResult : CommonResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public List<News> list { get; set; }
        public int getCode()
        {
            return code;
        }
        public List<News> getList()
        {
            return list;
        }
        public string getText()
        {
            return text;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setList(List<News> list)
        {
            this.list = list;
        }
        public void setText(string text)
        {
            this.text = text;
        }
    }

    public class Price
    {
        public string name { get; set; }
        public string price { get; set; }
        public string detailurl { get; set; }
        public string icon { get; set; }
        public string getDetailurl()
        {
            return detailurl;
        }
        public string getIcon()
        {
            return icon;
        }
        public string getName()
        {
            return name;
        }
        public string getPrice()
        {
            return price;
        }
        public void setDetailurl(string detailurl)
        {
            this.detailurl = detailurl;
        }
        public void setIcon(string icon)
        {
            this.icon = icon;
        }
        public void setName(string name)
        {
            this.name = name;
        }
        public void setPrice(string price)
        {
            this.price = price;
        }
    }
    public class PriceResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public List<Price> list { get; set; }
        public int getCode()
        {
            return code;
        }
        public List<Price> getList()
        {
            return list;
        }
        public string getText()
        {
            return text;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setList(List<Price> list)
        {
            this.list = list;
        }
        public void setText(string text)
        {
            this.text = text;
        }

    }

    public class Train
    {
        public string trainnum { get; set; }
        public string start { get; set; }
        public string terminal { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string detailurl { get; set; }
        public string getDetailurl()
        {
            return detailurl;
        }
        public void setDetailurl(string detailurl)
        {
            this.detailurl = detailurl;
        }
        public string getIcon()
        {
            return icon;
        }
        public void setIcon(string icon)
        {
            this.icon = icon;
        }
        public string icon { get; set; }
        public string getEndtime()
        {
            return endtime;
        }
        public string getStart()
        {
            return start;
        }
        public string getStarttime()
        {
            return starttime;
        }
        public string getTerminal()
        {
            return terminal;
        }
        public string getTrainnum()
        {
            return trainnum;
        }
        public void setEndtime(string endtime)
        {
            this.endtime = endtime;
        }
        public void setStart(string start)
        {
            this.start = start;
        }
        public void setStarttime(string starttime)
        {
            this.starttime = starttime;
        }
        public void setTerminal(string terminal)
        {
            this.terminal = terminal;
        }
        public void setTrainnum(string trainnum)
        {
            this.trainnum = trainnum;
        }
    }
    public class TrainResult : CommonResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public List<Train> list { get; set; }
        public int getCode()
        {
            return code;
        }
        public List<Train> getList()
        {
            return list;
        }
        public string getText()
        {
            return text;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setList(List<Train> list)
        {
            this.list = list;
        }
        public void setText(string text)
        {
            this.text = text;
        }
    }

    public class UrlResult : CommonResult
    {
        public int code { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public int getCode()
        {
            return code;
        }
        public string getText()
        {
            return text;
        }
        public string getUrl()
        {
            return url;
        }
        public void setCode(int code)
        {
            this.code = code;
        }
        public void setText(string text)
        {
            this.text = text;
        }
        public void setUrl(string url)
        {
            this.url = url;
        }
    }

}
