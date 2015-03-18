using log4net;
using NL2ML.consts;
using NL2ML.dbhelper;
using NL2ML.mediaProvider;
using NL2ML.models;
using NL2ML.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.plugins.nlp
{
    public class MediaIntentBuilder : IntentBuilder
    {
        private static ILog logger = LogManager.GetLogger("common");

        public Intent[] GetIntents(Context context)
        {
            string[][] input = context.Tags;
            List<Intent> intents = new List<Intent>();
            //有媒体动词，很有可能是个播放/停止流媒体的请求
            if (POSUtils.HasPOS(input, POSConstants.VerbMedia))
            {
                //检查是否有否定媒体动词：停止，停下来...
                string[] allMediaVerb = POSUtils.GetWordsByPOS(input, POSConstants.VerbMedia);
                //检查是否有否定副词存在: 不...
                string[] negAdv = POSUtils.GetWordsByPOS(input, POSConstants.Adv);
                //检查是否有否定词
                bool hasNegtive = false;
                foreach (var item in allMediaVerb)
                {
                    string command = DBHelperFactory.GetInstance().TranslateCommand(item);
                    //有否定词，优先停止播放
                    if (command.Equals("stop", StringComparison.OrdinalIgnoreCase))
                    {
                        hasNegtive = true;
                    }
                }

                
                if (hasNegtive)
                {
                    Intent intent = new Intent();
                    intent.Action = Actions.Stop;
                    intent.Domain = Domains.Media;
                    intent.Score = 100;
                    intents.Add(intent);
                }

                //检查是否有流派词：摇滚...
                if (POSUtils.HasPOS(input, POSConstants.NounGenre))
                {
                    string[] words = POSUtils.GetWordsByPOS(input, POSConstants.NounGenre);
                    MediaData data = MediaDataProviderFactory.GetProvider().GetMusicByGenre(words[0]);

                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Score = 100;
                    intents.Add(intent);
                }
            }
            else//没有媒体动词
            {

            }

            /*
             * 尝试理解动词后面的内容, 先分大类：音乐，故事，广播，
             * 1. 如果有关键词判断大分类，则在各个分类中进一步细化，如果没有则尝试按照
             */


            //找到媒体动词，歌手
            if (POSUtils.HasPOS(input, POSConstants.VerbMedia) &&
                POSUtils.HasPOS(input, POSConstants.NounName))
            {
                //尝试找歌曲名称

                logger.Debug("100% match artist, song name and media keyword");
                Intent intent = new Intent();
                intent.Domain = Domains.Media;
                intent.Action = Actions.Play;
                intent.RawCommand = context.RawString;
                intent.Score = 100;

                MediaData mediaData = MediaDataProviderFactory.GetProvider().GetMusic("菊花台","周杰伦");
                mediaData.Name = "";
                mediaData.Genre = "";
                intent.Data = mediaData;

                intents.Add(intent);
                return intents.ToArray();
            }

            return intents.ToArray();
        }
    }
}
