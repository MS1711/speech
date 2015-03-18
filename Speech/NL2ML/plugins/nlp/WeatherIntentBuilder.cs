using log4net;
using NL2ML.consts;
using NL2ML.models;
using NL2ML.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                intent.Score = 100;
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
                intent.RawCommand = context.RawString;
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
    }
}
