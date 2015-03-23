using log4net;
using NL2ML.consts;
using NL2ML.models;
using NL2ML.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NL2ML.plugins.nlp
{
    public class WeatherIntentBuilder : IntentBuilder
    {
        private static ILog logger = LogManager.GetLogger("common");

        public Intent[] GetIntents(Context context)
        {
            string[][] input = context.Tags;
            List<Intent> intents = new List<Intent>();
            //同时包含查询关键词，地名和天气后缀词. eg.查询北京天气
            if (POSUtils.HasPOS(input, POSConstants.NounLocation) &&
                POSUtils.HasPOS(input, POSConstants.SuffixWeather) &&
                POSUtils.HasPOS(input, POSConstants.VerbSearch))
            {
                logger.Debug("100% match location, weather suffix and search keyword");
                Intent intent = new Intent();
                intent.Domain = Domains.Weather;
                intent.Score = 100;

                WeatherData data = new WeatherData();
                data.Location = POSUtils.GetWordByPOS(input, POSConstants.NounLocation);

                DateTime[] time = GetTimeFromInput(context);

                data.Start = time[0];
                data.End = time[1];
                intent.Data = data;

                intents.Add(intent);
                return intents.ToArray();
            }

            //同时包含地名，天气后缀，询问词。eg. 北京天气怎么样
            if (POSUtils.HasPOS(input, POSConstants.NounLocation) &&
                POSUtils.HasPOS(input, POSConstants.SuffixWeather) &&
                POSUtils.HasPOS(input, POSConstants.SuffixQuestion))
            {
                logger.Debug("100% match location, weather suffix and question keyword");
                Intent intent = new Intent();
                intent.Domain = Domains.Weather;
                intent.RawCommand = context.RawString;

                WeatherData data = new WeatherData();
                data.Location = POSUtils.GetWordByPOS(input, POSConstants.NounLocation);

                DateTime[] time = GetTimeFromInput(context);

                data.Start = time[0];
                data.End = time[1];
                intent.Data = data;

                intent.Score = 100;
                intents.Add(intent);
                return intents.ToArray();
            }

            //只包含地名和天气后缀。eg.北京天气
            if (POSUtils.HasPOS(input, POSConstants.NounLocation) &&
                POSUtils.HasPOS(input, POSConstants.SuffixWeather))
            {

            }
            return intents.ToArray();
        }

        private DateTime[] GetTimeFromInput(Context context)
        {
            DateTime[] time = new DateTime[2];
            bool getDate = true;
            string dateStr = POSUtils.GetWordByPOS(context.Tags, POSConstants.NounTime);
            switch (dateStr)
            {
                case "今天":
                    {
                        time[0] = DateTime.Now;
                        time[1] = DateTime.Now;
                        break;
                    }
                case "明天":
                    {
                        time[0] = DateTime.Now.AddDays(1);
                        time[1] = DateTime.Now.AddDays(1);
                        break;
                    }
                case "后天":
                    {
                        time[0] = DateTime.Now.AddDays(2);
                        time[1] = DateTime.Now.AddDays(2);
                        break;
                    }
                default:
                    {
                        getDate = false;
                        break;
                    }
            }

            if (getDate)
            {
                return time;
            }

            if (POSUtils.HasPOS(context.Tags, POSConstants.NounTimeSpan))
            {
                time[0] = DateTime.Now;
                time[1] = DateTime.Now.AddDays(3);

                return time;
            }

            Regex regex = new Regex(@"(?<mon>\d+)月(?<day>\d+)[日|号][到|至]((?<monTo>\d+)月){0,1}(?<dayTo>\d+)[日|号]");
            MatchCollection mcs = regex.Matches(context.RawString);
            if (mcs.Count > 0)
            {
                string mon = mcs[0].Groups["mon"].Value;
                string day = mcs[0].Groups["day"].Value;
                string monTo = mon;
                if (!string.IsNullOrEmpty(mcs[0].Groups["monTo"].Value))
                {
                    monTo = mcs[0].Groups["monTo"].Value;
                }
                string dayTo = mcs[0].Groups["dayTo"].Value;

                time[0] = new DateTime(DateTime.Now.Year, int.Parse(mon), int.Parse(day));
                time[1] = new DateTime(DateTime.Now.Year, int.Parse(monTo), int.Parse(dayTo));
            }
            else
            {
                regex = new Regex(@"(?<mon>\d+)月(?<day>\d+)[日|号]");
                mcs = regex.Matches(context.RawString);
                if (mcs.Count > 0)
                {
                    string mon = mcs[0].Groups["mon"].Value;
                    string day = mcs[0].Groups["day"].Value;

                    time[0] = new DateTime(DateTime.Now.Year, int.Parse(mon), int.Parse(day));
                    time[1] = time[0];
                }
            }


            return time;
        }
     
    }
}
