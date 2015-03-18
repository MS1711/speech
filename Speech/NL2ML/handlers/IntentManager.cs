using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.handlers
{
    public class IntentManager
    {
        private static IntentManager instance = new IntentManager();
        private Dictionary<Domains, IIntentHandler> handlerMap = new Dictionary<Domains, IIntentHandler>();

        private IntentManager()
        {
            handlerMap[Domains.Media] = new MediaContentHandler();
            handlerMap[Domains.Weather] = new WeatherHandler();
            handlerMap[Domains.SmartDevice] = new SmartHouseHandler();
        }

        public static IntentManager Instance
        {
            get { return IntentManager.instance; }
        }

        public Result Process(Intent[] intent)
        {
            foreach (var item in intent)
            {
                Result res = Process(item);
                if (res != null && res.IsOK)
                {
                    return res;
                }
            }

            return new Result { IsOK = false };
        }

        public Result Process(Intent intent)
        {
            if (intent == null || !handlerMap.ContainsKey(intent.Domain))
            {
                return null;
            }

            IIntentHandler handler = handlerMap[intent.Domain];
            return handler.handle(intent);
        }
    }
}
