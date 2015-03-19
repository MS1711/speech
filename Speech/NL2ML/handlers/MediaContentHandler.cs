using log4net;
using NL2ML.models;
using NL2ML.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.handlers
{
    
    public class MediaContentHandler : IIntentHandler
    {
        private static ILog logger = LogManager.GetLogger("common");
        private WMPProxy proxy = WMPProxy.Instance;

        public Result handle(Intent intent)
        {
            Result res = new Result();
            res.IsOK = false;

            //if stop
            if (intent.Action == Actions.Stop)
            {
                proxy.Stop();
            }

            if (intent.Action == Actions.Play)
            {
                MediaData data = intent.Data as MediaData;
                proxy.Play(data.Durl);
                logger.Debug("start playing: " + data.Durl);
                
                res.IsOK = true;
            }

            if (intent.Action == Actions.Pause)
            {
                proxy.Pause();
            }

            if (intent.Action == Actions.Resume)
            {
                proxy.Resume();
            }

            return res;
        }
    }
}
