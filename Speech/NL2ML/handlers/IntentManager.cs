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
        private IIntentHandler defaultHandler = new RobotHandler();

        private IntentManager()
        {
            if (NL2ML.api.NL2ML.EnableVirtualSmartDevice)
            {
                handlerMap[Domains.Media] = new SmartHouseHandler();
                handlerMap[Domains.SmartDevice] = new SmartHouseHandler();
            }
            else
            {
                handlerMap[Domains.Media] = new MediaContentHandler();
            }
            handlerMap[Domains.Weather] = new WeatherHandler();
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

            foreach (var item in intent)
            {
                if (item.Domain == Domains.QA)
                {
                    Result res = defaultHandler.handle(item);
                    if (res != null && res.IsOK)
                    {
                        return res;
                    } 
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
