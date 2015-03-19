using log4net;
using NL2ML.models;
using NL2ML.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.handlers
{
    public class WeatherHandler : IIntentHandler
    {
        private static ILog logger = LogManager.GetLogger("common");

        public Result handle(Intent intent)
        {
            Result res = new Result();

            WeatherData data = intent.Data as WeatherData;
            if (data != null)
            {
                string loc = data.Location;
                DateTime start = data.Start;
                DateTime end = data.End;

                res.Msg = WeatherService.GetWeatherByCityName(loc, start, end);
                logger.Debug("weather data: " + res.Msg);
            }

            return res;
        }
    }
}
