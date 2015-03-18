using log4net;
using NL2ML.models;
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

        public Result handle(Intent intent)
        {
            Result res = new Result();
            res.IsOK = false;

            MediaData md = intent.Data as MediaData;
            if (md == null)
            {
                return res;
            }

            //if stop
            if (intent.Action == Actions.Stop)
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

            if (intent.Action == Actions.Play)
            {
                MediaData data = intent.Data as MediaData;
                logger.Debug("start playing: " + data.Durl);
                Process.Start("wmplayer", "/play /close " + data.Durl);
                res.IsOK = true;
            }

            return res;
        }
    }
}
