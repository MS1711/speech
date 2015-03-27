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
        private IPlayer proxy = new DShowPlayer();
        private IPlayer wpproxy = WMPProxy.Instance;

        public static string GetMediaSuffix(MediaCategory cate)
        {
            switch (cate)
            {
                case MediaCategory.Music:
                    {
                        return "这首歌";
                    }
                case MediaCategory.Story:
                    {
                        return "这个故事";
                    }
            }

            return "";
        }

        public Result handle(Intent intent)
        {
            Result res = new Result();
            res.IsOK = false;

            if (intent.Action == Actions.Suggestion)
            {
                MediaData data = intent.Data as MediaData;
                if (data != null)
                {
                    List<MediaData> sug = data.Suggestions;
                    if (sug != null)
                    {
                        if (sug.Count > 1)
                        {
                            res.Msg = string.Format("您是想听{0}{1},还是{2}{3}", sug[0].Name, GetMediaSuffix(sug[0].Category),
                                sug[1].Name, GetMediaSuffix(sug[1].Category));
                            res.IsOK = true;
                        }
                        else if (sug.Count == 1)
                        {
                            res.Msg = string.Format("您是不是想听{0}{1}", sug[0].Name, GetMediaSuffix(sug[0].Category));
                            res.IsOK = true;
                        }
                    }
                }
                
            }

            //if stop
            if (intent.Action == Actions.Stop)
            {
                wpproxy.Stop();
                proxy.Stop();
                res.IsOK = true;
            }

            if (intent.Action == Actions.Play)
            {
                MediaData data = intent.Data as MediaData;
                string url = data.Durl;
                if (string.IsNullOrEmpty(url))
                {
                    url = data.Url;
                }

                if (string.IsNullOrEmpty(url))
                {
                    return res;
                }

                //special logic for tom61. Because the real mp3 url is nested in the response html.
                if (url.IndexOf("tom61.com") != -1)
                {
                    url = Utils.GetTom61InnerAudioLink(url);
                }

                try
                {
                    if (url.StartsWith("mms:"))
                    {
                        wpproxy.Play(url);
                    }
                    else
                    {
                        proxy.Play(url);
                    }

                    res.IsOK = true;
                }
                catch(Exception e)
                {
                    res.Msg = "对不起，没找到这个东东";
                }
                
                
                logger.Debug("start playing: " + url);
                
                
            }

            if (intent.Action == Actions.Pause)
            {
                wpproxy.Pause();
                proxy.Pause();
                res.IsOK = true;
            }

            if (intent.Action == Actions.Resume)
            {
                wpproxy.Resume();
                proxy.Resume();
                res.IsOK = true;
            }

            return res;
        }
    }
}
