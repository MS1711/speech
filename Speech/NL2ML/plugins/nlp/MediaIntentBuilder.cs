using log4net;
using NL2ML.consts;
using NL2ML.dbhelper;
using NL2ML.mediaProvider;
using NL2ML.models;
using NL2ML.utils;
using NLPCore;
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
        private static ILog nlplogger = LogManager.GetLogger("nl2ml");

        public Intent[] GetIntents(Context context)
        {
            string[][] input = context.Tags;
            List<Intent> intents = new List<Intent>();

            /*
             * try to understand if the words contains 'media' info.
             * 1. if the words contains artist and a song name, it is probably a media request
             * 2. if the words contains only artist without a concrete song name, randomly pick one
             * 3. if the words has no artist name, try to understand if the words contains media related info. eg, genre, media suffix...
             * 
             */

            string action = GetAction(context);
            logger.Debug("action: " + action);
            if (action.Equals("stop", StringComparison.OrdinalIgnoreCase))
            {
                Intent intent = new Intent();
                intent.Action = Actions.Stop;
                intent.Domain = Domains.Media;
                intent.Score = 100;
                intents.Add(intent);
            }
            else if (action.Equals("pause", StringComparison.OrdinalIgnoreCase))
            {
                Intent intent = new Intent();
                intent.Action = Actions.Pause;
                intent.Domain = Domains.Media;
                intent.Score = 100;
                intents.Add(intent);
            }
            else if (action.Equals("start", StringComparison.OrdinalIgnoreCase))
            {
                intents.AddRange(ProcessStart3(context));
            }
            else if (action.Equals("resume", StringComparison.OrdinalIgnoreCase))
            {
                Intent intent = new Intent();
                intent.Action = Actions.Resume;
                intent.Domain = Domains.Media;
                intent.Score = 100;
                intents.Add(intent);
            }
            else if (action.Equals("switch", StringComparison.OrdinalIgnoreCase))
            {
                intents.AddRange(ProcessSwitch(context));
            }

            return intents.ToArray();
        }

        private List<Intent> ProcessStart3(Context context)
        {
            List<Intent> intents = new List<Intent>();
            List<CorrectedInfo> perhapsList = new List<CorrectedInfo>();

            int[] verbLoc = Utils.LocatePOS(context, POSConstants.VerbMedia);
            if (verbLoc == null || verbLoc.Length == 0)
            {
                verbLoc = Utils.LocatePOS(context, POSConstants.VerbMixed);
            }

            //no media verbs, invalid
            if (verbLoc == null || verbLoc.Length == 0)
            {
                return intents;
            }

            string[][] tags = context.Tags;

            Context ctx = Utils.SubContext(context, verbLoc[verbLoc.Length - 1] + 1, tags[0].Length);
            string keyword = ctx.RawString;
            nlplogger.Debug("key word after verb: " + keyword);

            //try to process the whole word
            nlplogger.Debug("try to process keyword : " + keyword + " as whole word.");
            intents.AddRange(ProcessMediaPlainText(ctx, ""));

            if (POSUtils.HasPOS(tags, POSConstants.SuffixMusic))
            {
                nlplogger.Debug("music suffix detected, try to process in music domain.");
                int[] indx = Utils.LocatePOS(ctx, POSConstants.SuffixMusic);
                Context ct = Utils.SubContext(ctx, 0, indx[0]);
                nlplogger.Debug("music domain processing: " + ctx);
                intents.AddRange(ProcessMediaPlainText(ct, "music"));
            }

            if (POSUtils.HasPOS(tags, POSConstants.SuffixStory))
            {
                nlplogger.Debug("story suffix detected, try to process in story domain.");
                int[] indx = Utils.LocatePOS(ctx, POSConstants.SuffixStory);
                Context ct = Utils.SubContext(ctx, 0, indx[0]);
                nlplogger.Debug("story domain processing: " + ctx);
                intents.AddRange(ProcessMediaPlainText(ct, "story"));
            }

            if (POSUtils.HasPOS(tags, POSConstants.SuffixBroadcast))
            {
                nlplogger.Debug("radio suffix detected, try to process in radio domain.");
                int[] indx = Utils.LocatePOS(ctx, POSConstants.SuffixBroadcast);
                Context ct = Utils.SubContext(ctx, 0, indx[0]);
                nlplogger.Debug("radio domain processing: " + ctx);
                intents.AddRange(ProcessMediaPlainText(ct, "radio"));
            }

            return intents;
        }

        private List<Intent> ProcessMediaPlainText(Context ctx, string category)
        {
            List<Intent> intents = new List<Intent>();
            Dictionary<string, string> query = new Dictionary<string, string>();
            string[][] tags = ctx.Tags;
            string raw = ctx.RawString;
            MediaData data = null;

            /*
             * 1. consider the word as a whole word
             *   a. consider the whole word a song name to query db, get extactly matched items (music, story...)
             *      if the result count > 1, return to user to choose one.
             *      if the result count == 1, emit a 100 score intent.
             *      
             *   b. consider the whole word as category (歌，音乐，故事，广播...).
             *      if extactly match, find random media item from db. emit 100 score intent.
             *      
             *   c. consider the whole word as music genre (摇滚，爵士...)
             *      if exactly match (raw.length == 1, raw == 'GENRE'), emit 100 score intent.
             *      if partly match (raw.length > 1, one of tags == 'GENRE'), emit a 90 score intent.
             *      
             *   d. consider the whole word a song name to search from internet
             *      if get result, emit a 90 score intent.
             *      
             *   e. consider the song name is incorrect, try to fix it and search again
             *      if the result, emit a 100 * similarity score
             *      
             */

            ////////////////////////*********** a *****************////////////////////////////////////
            query.Clear();
            query["Name"] = raw;
            query["Mode"] = "M";
            if (!string.IsNullOrEmpty(category))
            {
                query["Category"] = category;
            }

            nlplogger.Debug("consider the whole word a song name to query db. " + Utils.DumpDict(query));
            intents.AddRange(GetIntentsByKeywords(query));

            ////////////////////////*********** b *****************////////////////////////////////////
            query.Clear();
            string cate = Utils.TranslateToCategory(raw);
            if (!string.IsNullOrEmpty(cate))
            {
                query["Category"] = cate;
                query["Mode"] = "S";
                nlplogger.Debug("consider the whole word as category. " + Utils.DumpDict(query));
                intents.AddRange(GetIntentsByKeywords(query));
            }

            ////////////////////////*********** c *****************////////////////////////////////////
            query.Clear();
            string[] genres = POSUtils.GetWordsByPOS(tags, POSConstants.NounGenre);
            if (genres != null && genres.Length > 0)
            {
                if (!string.IsNullOrEmpty(category))
                {
                    query["Category"] = category;
                }
                query["Metadata.Genre"] = genres[0];
                query["Mode"] = "S";
                nlplogger.Debug("consider the whole word as music genre. " + Utils.DumpDict(query));
                List<Intent> ins = GetIntentsByKeywords(query);
                foreach (var item in ins)
                {
                    item.Score = (int)(100 * ((float)1 / tags.Length));
                }
                intents.AddRange(ins);
            }

            ////////////////////////*********** d *****************////////////////////////////////////
            data = MediaInfoHelper.Instance.GetMusicOnline(raw, "");
            if (data != null && data.IsValid())
            {
                nlplogger.Debug("consider the whole word a song name to search from internet. keyword: " + raw + ", item: " + data);
                Intent intent = new Intent();
                intent.Action = Actions.Play;
                intent.Domain = Domains.Media;
                intent.Data = data;
                intent.Score = 80;
                intents.Add(intent);
            }

            ////////////////////////*********** e *****************////////////////////////////////////
            CorrectedInfo corr = new CorrectedInfo();
            if (NL2ML.api.NL2ML.EnableOnlineErrorFix)
            {
                corr = MediaInfoHelper.Instance.GetSimilarSong(raw);
                if (!string.IsNullOrEmpty(corr.Item))
                {
                    nlplogger.Debug("consider the song name is incorrect. wrong: " + raw + ", correct: " + corr.Item);
                    data = MediaInfoHelper.Instance.GetMusicOnline(corr.Item, "");
                    if (data != null && data.IsValid())
                    {
                        nlplogger.Debug("get search result from web. keyword: " + corr.Item + ", item: " + data);
                        Intent intent = new Intent();
                        intent.Action = Actions.Suggestion;
                        intent.Domain = Domains.Media;
                        intent.Data = data;
                        intent.Score = (int)(100 * corr.Score);
                        if (intent.Score >= 60)
                        {
                            intent.Action = Actions.Play;
                        }
                        intents.Add(intent);
                    }
                }
            }

            /*
             * 2. consider the word as a "xxx的xxx"
             *    split the keyword by "的"， get prefix and suffix
             *    i. prefix==artist and suffix==song eg.周杰伦的菊花台
             *    ii. prefix==artist and suffix==genre eg.郭德纲的相声
             *    iii. prefix==artist and suffix==category eg.周杰伦的歌
             * 
             *    a. try to find the media info from web using prefix as artist and suffix as song name.
             *       if found, emit a 100 score intent.
             *    b. if the prefix is artist and suffix is vague category.
             *       if found, emit a 100 score intent.
             *    c. if the prefix is artist and suffix is vague genre.
             *       if found, emit a 100 score intent.
             *    d. if prefix is artist and suffix is a wrong song name.
             *       correct the song name and search again, if found emit similarity score intent
             *    e. if the artist is wrong and song name is correct.
             *       fix the artist name and search again. emit similarity score intent.
             *    f. if the artist is wrong and song name is wrong.
             *       fix the artist name and search again. emit similarity score intent.
             */
            int indx = raw.IndexOf("的");
            if (indx == -1)
            {
                return intents;
            }

            string prefix = raw.Substring(0, indx);
            string suffix = raw.Substring(indx + 1);

            ////////////////////////*********** a *****************////////////////////////////////////
            nlplogger.Debug("try to find the media info from web using prefix as artist and suffix as song name. artist: " + prefix + ", song: " + suffix);
            data = MediaInfoHelper.Instance.GetMusicOnline(suffix, prefix);
            if (data != null && data.IsValid())
            {
                logger.Debug("get search result from web. keyword:  + artist: " + prefix + ", song: " + suffix + ", item: " + data);
                Intent intent = new Intent();
                intent.Action = Actions.Play;
                intent.Domain = Domains.Media;
                intent.Data = data;
                intent.Score = 100;
                intents.Add(intent);
            }

            ////////////////////////*********** b *****************////////////////////////////////////
            nlplogger.Debug("if the prefix is artist and suffix is vague category");
            query.Clear();
            cate = Utils.TranslateToCategory(suffix);
            if (!string.IsNullOrEmpty(cate))
            {
                nlplogger.Debug("if the prefix is artist and suffix is vague category, category: " + cate);
                query["Category"] = cate;
                query["Mode"] = "S";
                query["Metadata.Singer"] = prefix;
                intents.AddRange(GetIntentsByKeywords(query));
            }

            ////////////////////////*********** c *****************////////////////////////////////////
            nlplogger.Debug("if the prefix is artist and suffix is vague genre.");
            query.Clear();
            string[][] subtags = CNFactory.GetInstance().tag(suffix);
            if (POSUtils.HasPOS(subtags, POSConstants.NounGenre))
            {
                genres = POSUtils.GetWordsByPOS(tags, POSConstants.NounGenre);
                if (genres != null && genres.Length > 0)
                {
                    if (!string.IsNullOrEmpty(category))
                    {
                        query["Category"] = category;
                    }
                    query["Metadata.Singer"] = prefix;
                    query["Metadata.Genre"] = genres[0];
                    query["Mode"] = "S";
                    nlplogger.Debug("if the prefix is artist and suffix is vague genre. genre: " + genres[0] + ", query: " + Utils.DumpDict(query));
                    List<Intent> ins = GetIntentsByKeywords(query);
                    foreach (var item in ins)
                    {
                        item.Score = (int)(100 * ((float)1 / tags.Length));
                    }
                    intents.AddRange(ins);
                }
            }

            ////////////////////////*********** d *****************////////////////////////////////////
            CorrectedInfo artistCorr = new CorrectedInfo();
            CorrectedInfo songCorr = new CorrectedInfo();
            if (NL2ML.api.NL2ML.EnableOnlineErrorFix)
            {
                nlplogger.Debug("if prefix is artist and suffix is a wrong song name.");
                artistCorr = MediaInfoHelper.Instance.GetSimilarArtist(prefix);
                songCorr = MediaInfoHelper.Instance.GetSimilarSong(suffix);

                if (!string.IsNullOrEmpty(songCorr.Item))
                {
                    nlplogger.Debug("if prefix is artist and suffix is a wrong song name. wrong: " + suffix + ", correct song name: " + songCorr);
                    data = MediaInfoHelper.Instance.GetMusicOnline(songCorr.Item, prefix);
                    if (data != null && data.IsValid())
                    {
                        nlplogger.Debug("get search result from web. keyword: " + corr.Item + ", item: " + data);
                        Intent intent = new Intent();
                        intent.Action = Actions.Suggestion;
                        intent.Domain = Domains.Media;
                        intent.Data = data;
                        intent.Score = (int)(100 * songCorr.Score);
                        if (intent.Score >= 60)
                        {
                            intent.Action = Actions.Play;
                        }
                        intents.Add(intent);
                    }
                }
            }

            ////////////////////////*********** e *****************////////////////////////////////////
            if (NL2ML.api.NL2ML.EnableOnlineErrorFix)
            {
                nlplogger.Debug("if the artist is wrong and song name is correct.");
                if (!string.IsNullOrEmpty(artistCorr.Item))
                {
                    nlplogger.Debug("if prefix is wrong artist and suffix is correct song name. wrong: " + prefix + ", correct artist name: " + artistCorr);
                    data = MediaInfoHelper.Instance.GetMusicOnline(suffix, artistCorr.Item);
                    if (data != null && data.IsValid())
                    {
                        nlplogger.Debug("get search result from web. keyword: " + corr.Item + ", item: " + data);
                        Intent intent = new Intent();
                        intent.Action = Actions.Suggestion;
                        intent.Domain = Domains.Media;
                        intent.Data = data;
                        intent.Score = (int)(100 * artistCorr.Score);
                        if (intent.Score >= 60)
                        {
                            intent.Action = Actions.Play;
                        }
                        intents.Add(intent);
                    }
                }
            }

            ////////////////////////*********** f *****************////////////////////////////////////
            if (NL2ML.api.NL2ML.EnableOnlineErrorFix)
            {
                nlplogger.Debug("if the artist is wrong and song name is wrong.");
                if (!string.IsNullOrEmpty(artistCorr.Item) && !string.IsNullOrEmpty(songCorr.Item))
                {
                    nlplogger.Debug("if prefix is wrong and suffix is wrong. wrong: " + prefix + ", " + suffix + ", correct: " + artistCorr + ", " + songCorr);
                    data = MediaInfoHelper.Instance.GetMusicOnline(songCorr.Item, artistCorr.Item);
                    if (data != null && data.IsValid())
                    {
                        nlplogger.Debug("get search result from web. keyword: " + songCorr.Item + ", " + artistCorr.Item + ", item: " + data);
                        Intent intent = new Intent();
                        intent.Action = Actions.Suggestion;
                        intent.Domain = Domains.Media;
                        intent.Data = data;
                        intent.Score = Math.Min((int)(100 * artistCorr.Score), (int)(100 * songCorr.Score));
                        if (intent.Score >= 60)
                        {
                            intent.Action = Actions.Play;
                        }
                        intents.Add(intent);
                    }
                }
            }

            return intents;
        }

        private List<Intent> GetIntentsByKeywords(Dictionary<string, string> query)
        {
            List<Intent> intents = new List<Intent>();
            List<MediaData> datalist = null;
            MediaData data = null;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            nlplogger.Debug("querying db with: " + Utils.DumpDict(query));
            datalist = MediaInfoHelper.Instance.GetMediaListByGenericQuery(query);
            if (datalist != null && datalist.Count > 1)
            {
                nlplogger.Debug("get multiple items. Items: " + string.Join(", " , datalist));
                MediaData data2 = new MediaData();
                data2.Suggestions = datalist;
                Intent intent = new Intent();
                intent.Action = Actions.Suggestion;
                intent.Domain = Domains.Media;
                intent.Data = data2;
                intent.Score = 60;
                intents.Add(intent);

                return intents;
            }
            else if (datalist != null && datalist.Count == 1)
            {
                nlplogger.Debug("get exactly matched item Item: " + datalist[0]);
                Intent intent = new Intent();
                intent.Action = Actions.Play;
                intent.Domain = Domains.Media;
                intent.Data = datalist[0];
                intent.Score = 100;
                intents.Add(intent);
            }

            return intents;
        }

        private List<Intent> GetIntentsByKeywords(Dictionary<string, string> query, Context context)
        {
            List<Intent> intents = new List<Intent>();
            List<MediaData> datalist = null;
            MediaData data = null;

            string[][] tags = context.Tags;
            string raw = context.RawString;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            logger.Debug("consider the whole word a song name to query db. keyword: " + query);
            datalist = MediaInfoHelper.Instance.GetMediaListByGenericQuery(query);
            if (datalist != null && datalist.Count > 1)
            {
                logger.Debug("get multiple items using keyword as song name. Items: " + data);
                MediaData data2 = new MediaData();
                data2.Suggestions = datalist;
                Intent intent = new Intent();
                intent.Action = Actions.Suggestion;
                intent.Domain = Domains.Media;
                intent.Data = data2;
                intent.Score = 100;
                intents.Add(intent);

                return intents;
            }
            else if (data != null && datalist.Count == 1)
            {
                logger.Debug("get exactly matched item using keyword as song name. Item: " + data);
                Intent intent = new Intent();
                intent.Action = Actions.Suggestion;
                intent.Domain = Domains.Media;
                intent.Data = datalist[0];
                intent.Score = 100;
                intents.Add(intent);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Utils.IsVagueStory(raw))
            {
                logger.Debug("the keyword is vague story, try to find random story from db.");
                data = MediaInfoHelper.Instance.GetRandomMediaByCategory("story");
                if (data != null && data.IsValid())
                {
                    logger.Debug("get random story using vague word: " + raw + ", " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 100;
                    intents.Add(intent);
                }
            }
            if (Utils.IsVagueRadio(raw))
            {
                logger.Debug("the keyword is vague radio, try to find random radio from db.");
                data = MediaInfoHelper.Instance.GetRandomMediaByCategory("radio");
                if (data != null && data.IsValid())
                {
                    logger.Debug("get random radio using vague word: " + raw + ", " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 100;
                    intents.Add(intent);
                }
            }
            if (Utils.IsVagueMusic(raw))
            {
                logger.Debug("the keyword is vague music, try to find random music from db.");
                data = MediaInfoHelper.Instance.GetRandomMediaByCategory("music");
                if (data != null && data.IsValid())
                {
                    logger.Debug("get random music using vague word: " + raw + ", " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 100;
                    intents.Add(intent);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            string[] genres = POSUtils.GetWordsByPOS(tags, POSConstants.NounGenre);
            if (genres != null && genres.Length > 0)
            {
                logger.Debug("the keyword contains genre items: " + genres[0]);
                string genre = genres[0];
                data = MediaInfoHelper.Instance.GetMusicByGenre(genre);
                if (data != null && data.IsValid())
                {
                    logger.Debug("get randon music by genre, keyword: " + genre + ", item: " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = (int)(100 * ((float)1 / tags.Length));
                    intents.Add(intent);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            logger.Debug("try to search the keyword as song name from web. keyword: " + raw);
            data = MediaInfoHelper.Instance.GetMusicOnline(query["Name"], query["Metadata.Singer"]);
            if (data != null && data.IsValid())
            {
                logger.Debug("get search result from web. keyword: " + raw + ", item: " + data);
                Intent intent = new Intent();
                intent.Action = Actions.Play;
                intent.Domain = Domains.Media;
                intent.Data = data;
                intent.Score = 80;
                intents.Add(intent);
            }

            return intents;
        }

        private List<Intent> ProcessStart(Context context)
        {
            List<Intent> intents = new List<Intent>();
            List<CorrectedInfo> perhapsList = new List<CorrectedInfo>();

            string[][] tags = context.Tags;
            string raw = context.RawString;

            string[] genres = POSUtils.GetWordsByPOS(tags, POSConstants.NounGenre);
            if (genres != null && genres.Length > 0)
            {
                string genre = genres[0];
                string songName = raw.Substring(raw.IndexOf(genre) + genre.Length);
                MediaData data = MediaInfoHelper.Instance.GetMusicByGenre(genre, songName);
                if (data != null && data.IsValid())
                {
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
            }

            string[] verbs = POSUtils.GetWordsByPOS(tags, POSConstants.VerbMedia);
            if (verbs == null || verbs.Length == 0)
            {
                verbs = POSUtils.GetWordsByPOS(tags, POSConstants.VerbMixed);
            }
            if (verbs == null || verbs.Length == 0)
            {
                return intents;
            }
            string verb = verbs[0];
            string suffix = raw.Substring(raw.IndexOf(verb) + verb.Length);

            //处理：随便来一个这种没有宾语后缀的
            if (string.IsNullOrEmpty(suffix) && POSUtils.HasPOS(tags, POSConstants.SuffixRandom))
            {
                logger.Debug("get from local with random music");
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("music");
                if (data != null && data.IsValid())
                {
                    logger.Debug("get from local with random music: " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
            }

            if (Utils.IsVagueStory(suffix))
            {
                logger.Debug("get from local is vague story");
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("story");
                if (data != null && data.IsValid())
                {
                    logger.Debug("get from local is vague story: " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
            }
            if (Utils.IsVagueRadio(suffix))
            {
                logger.Debug("get from local is vague radio");
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("radio");
                if (data != null && data.IsValid())
                {
                    logger.Debug("get from local is vague radio: " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
            }
            if (Utils.IsVagueMusic(suffix))
            {
                logger.Debug("get from local is vague music");
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("music");
                if (data != null && data.IsValid())
                {
                    logger.Debug("get from local is vague music: " + data);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
            }

            CorrectedInfo songCorrected = null;
            List<MediaData> infolist = MediaInfoHelper.Instance.GetMediaListByName(suffix);
            if (infolist.Count > 1)
            {
                logger.Debug("get from local confused. " + infolist);
                MediaData data = new MediaData();
                data.Suggestions = infolist;
                Intent intent = new Intent();
                intent.Action = Actions.Suggestion;
                intent.Domain = Domains.Media;
                intent.Data = data;
                intent.Score = 90;
                intents.Add(intent);

                return intents;
            }
            else if (infolist.Count == 1)
            {
                logger.Debug("get from local specified. " + infolist);
                Intent intent = new Intent();
                intent.Action = Actions.Play;
                intent.Domain = Domains.Media;
                intent.Data = infolist[0];
                intent.Score = 90;
                intents.Add(intent);

                return intents;
            }
            else
            {  
                songCorrected = MediaInfoHelper.Instance.GetSimilarSong(suffix);
                logger.Debug("get from local try to fix. before: " + suffix + ", after: " + songCorrected.Item);
                songCorrected.Category = MediaCategory.Music;
                songCorrected.SongName = songCorrected.Item;
                perhapsList.Add(songCorrected);
            }

            MediaData data2 = null;
            string last = null;
            string storyLast = null;
            string randomLast = null;
            string radioLast = null;
            // 作者的字定语词
            string deZiDingYu = null;
            last = POSUtils.GetWordByPOS(tags, POSConstants.SuffixMusic);//myWords.get("音乐后缀词");
            storyLast = POSUtils.GetWordByPOS(tags, POSConstants.SuffixStory); //myWords.get("故事后缀词");
            randomLast = POSUtils.GetWordByPOS(tags, POSConstants.SuffixRandom); //myWords.get("随机后缀词");
            radioLast = POSUtils.GetWordByPOS(tags, POSConstants.SuffixBroadcast); //myWords.get("广播后缀词");
            deZiDingYu = POSUtils.GetWordByPOS(tags, POSConstants.AdvAuthor); //myWords.get("作者的字定语词");
            if (!RealyLast(radioLast, raw))
            {
                radioLast = null;
            }

            if (!string.IsNullOrEmpty(last))
            {
                logger.Debug("get from local has music suffix. ");
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(last) - raw.IndexOf(verb) - verb.Length);
                data2 = MediaInfoHelper.Instance.GetMusicByName(noLast);
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get from local has music suffix: " + data2);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data2;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
            }

            // 处理音乐后缀词如播放陈奕迅的十年这首歌
            string afterVerbSinger1 = "";
            string afterVerbSong1 = "";
            if (!string.IsNullOrEmpty(last))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(last) - raw.IndexOf(verb) - verb.Length);
                String afterVerb = noLast;
                if (!string.IsNullOrEmpty(deZiDingYu))
                {
                    afterVerb = afterVerb.Replace(deZiDingYu, "的");
                }

                int deIndex = afterVerb.IndexOf("的");
                afterVerbSinger1 = afterVerb.Substring(0, deIndex);
                afterVerbSong1 = afterVerb.Substring(deIndex + 1);

                logger.Debug("get from local with artist and song. ");
                if (!string.IsNullOrEmpty(afterVerbSong1) && !string.IsNullOrEmpty(afterVerbSinger1))
                {
                    data2 = MediaInfoHelper.Instance.GetMusicOnline(afterVerbSong1, afterVerbSinger1);
                    if (data2 != null && data2.IsValid())
                    {
                        logger.Debug("get from local with artist and song: " + data2);
                        Intent intent = new Intent();
                        intent.Action = Actions.Play;
                        intent.Domain = Domains.Media;
                        intent.Data = data2;
                        intent.Score = 90;
                        intents.Add(intent);

                        return intents;
                    }
                    else
                    {

                        // 如果没有搜索到陈奕迅的十年，则任选一首十年播放
                        logger.Debug("get from local with song. ");
                        data2 = MediaInfoHelper.Instance.GetMusicByName(afterVerbSong1);
                        if (data2 != null && data2.IsValid())
                        {
                            logger.Debug("get from local with song. " + data2);
                            Intent intent = new Intent();
                            intent.Action = Actions.Play;
                            intent.Domain = Domains.Media;
                            intent.Data = data2;
                            intent.Score = 90;
                            intents.Add(intent);

                            return intents;
                        }
                        else
                        {
                            songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong1);
                            logger.Debug("get from local with correct song. before: " + afterVerbSong1 + ", after: " + songCorrected.Item);
                            songCorrected.Category = MediaCategory.Music;
                            songCorrected.SongName = songCorrected.Item;
                            perhapsList.Add(songCorrected);
                        }

                    }
                }
            }

            // 处理故事后缀词 如播放白雪公主这个故事
            if (!string.IsNullOrEmpty(storyLast))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(storyLast) - raw.IndexOf(verb) - verb.Length);
                logger.Debug("get from local with story. " + noLast);
                data2 = MediaInfoHelper.Instance.GetMediaByCategory(noLast, "story");
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get from local with story data: " + data2);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data2;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
                else
                {
                    songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong1);
                    logger.Debug("get from local with correct story. before: " + afterVerbSong1 + ", after: " + songCorrected.Item);
                    songCorrected.Category = MediaCategory.Story;
                    songCorrected.SongName = songCorrected.Item;
                    perhapsList.Add(songCorrected);
                }
            }
            // 处理随机歌曲后缀词如播放周杰伦的歌
            if (!string.IsNullOrEmpty(randomLast))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(randomLast) - raw.IndexOf(verb) - verb.Length);
                logger.Debug("get from local with random artist. " + noLast);
                data2 = MediaInfoHelper.Instance.GetRandomMediaByArtist(noLast);
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get from local with random artist. " + data2);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data2;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
                else
                {
                    songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong1);
                    logger.Debug("get from local with correct random artist. before: " + afterVerbSong1 + ", after: " + songCorrected.Item);
                    songCorrected.Category = MediaCategory.Music;
                    songCorrected.Artist = songCorrected.Item;
                    perhapsList.Add(songCorrected);
                }
            }
            // 处理广播后缀词
            if (!string.IsNullOrEmpty(radioLast))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(radioLast) - raw.IndexOf(verb) - verb.Length);
                data2 = MediaInfoHelper.Instance.GetRandomRadioByCategory(noLast);
                logger.Debug("get from local with random radio category. " + noLast);
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get from local with random radio category, data: " + data2);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data2;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
            }

            // 处理陈奕迅的十年这种情况
            string afterVerbSinger2 = "";
            string afterVerbSong2 = "";
            string afterVerb2 = raw.Substring(raw.IndexOf(verb) + verb.Length);
            if (!string.IsNullOrEmpty(deZiDingYu))
            {
                afterVerb2 = afterVerb2.Replace(deZiDingYu, "的");
            }

            int deIndex2 = afterVerb2.IndexOf("的");
            if (deIndex2 != -1)
            {
                afterVerbSinger2 = afterVerb2.Substring(0, deIndex2);
                afterVerbSong2 = afterVerb2.Substring(deIndex2 + 1);

                logger.Debug("get from local with no suffix artist and song. " + afterVerbSong2 + ", " + afterVerbSinger2);

                if (!string.IsNullOrEmpty(afterVerbSong2) && !string.IsNullOrEmpty(afterVerbSinger2))
                {
                    data2 = MediaInfoHelper.Instance.GetMusicOnline(afterVerbSong2, afterVerbSinger2);
                }
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get from local with no suffix artist and song. data: " + data2);
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data2;
                    intent.Score = 90;
                    intents.Add(intent);

                    return intents;
                }
                else
                {// 如果没有搜索到陈奕迅的十年，则任选一首十年播放
                    logger.Debug("get from local with no suffix song: " + afterVerbSong2);
                    data2 = MediaInfoHelper.Instance.GetMusicByName(afterVerbSong2);
                    if (data2 != null && data2.IsValid())
                    {
                        logger.Debug("get from local with no suffix artist and song. data: " + data2);
                        Intent intent = new Intent();
                        intent.Action = Actions.Play;
                        intent.Domain = Domains.Media;
                        intent.Data = data2;
                        intent.Score = 90;
                        intents.Add(intent);

                        return intents;
                    }
                    else
                    {
                        songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong2);
                        logger.Debug("get from local with correct random artist. before: " + afterVerbSong2 + ", after: " + songCorrected.Item);
                        songCorrected.Category = MediaCategory.Music;
                        songCorrected.SongName = songCorrected.Item;
                        perhapsList.Add(songCorrected);
                    }
                }
            }
            

            if (intents.Count == 0)
            {
                logger.Debug("get from local failed, search from online.");
                data2 = QueryMediaItemsFromWeb(context);
            }
            if (data2 != null && data2.IsValid())
            {
                Intent intent = new Intent();
                intent.Action = Actions.Play;
                intent.Domain = Domains.Media;
                intent.Data = data2;
                intent.Score = 90;
                intents.Add(intent);

                return intents;
            }

            //no intents found, check if there is perhaps list
            if (intents.Count == 0 && perhapsList.Count > 0)
            {
                logger.Debug("no intents found. check perhaps list.");
                CorrectedInfo info = perhapsList[0];
                string category = info.Category.ToString().ToLower();
                string name = info.SongName;
                string artist = info.Artist;
                logger.Debug("perhaps found: " + info);

                MediaData data = new MediaData();
                List<MediaData> sugs = new List<MediaData>();
                data.Suggestions = sugs;
                Intent intent = new Intent();
                intent.Action = Actions.Suggestion;
                intent.Domain = Domains.Media;
                intent.Data = data;
                intents.Add(intent);

                switch (info.Category)
                {
                    case MediaCategory.Music:
                        {
                            if (!string.IsNullOrEmpty(name))
                            {
                                data2 = MediaInfoHelper.Instance.GetMusicOnline(name, artist);
                                if (data2 != null && data2.IsValid())
                                {
                                    logger.Debug("music found: " + data2);
                                    sugs.Add(data2);
                                    return intents;
                                }
                            }
                            else if (!string.IsNullOrEmpty(artist))
                            {
                                data2 = MediaInfoHelper.Instance.GetRandomMediaByArtist(artist);
                                if (data2 != null && data2.IsValid())
                                {
                                    sugs.Add(data2);
                                    return intents;
                                }
                            }

                            break;
                        }
                    case MediaCategory.Story:
                        {
                            data2 = MediaInfoHelper.Instance.GetMediaByCategory(info.SongName, "story");
                            if (data2 != null && data2.IsValid())
                            {
                                sugs.Add(data2);
                                return intents;
                            }

                            break;
                        }
                }
                
            }

            return intents;
        }

        private MediaData QueryMediaItemsFromWeb(Context context)
        {
            logger.Debug("try to get from web.");
            List<Intent> intents = new List<Intent>();

            string[][] tags = context.Tags;
            string raw = context.RawString;

            string[] verbs = POSUtils.GetWordsByPOS(tags, POSConstants.VerbMedia);
            if (verbs == null || verbs.Length == 0)
            {
                verbs = POSUtils.GetWordsByPOS(tags, POSConstants.VerbMixed);
            }
            if (verbs == null || verbs.Length == 0)
            {
                return null;
            }
            string verb = verbs[0];
            string suffix = raw.Substring(raw.IndexOf(verb) + verb.Length);

            MediaData data2 = null;
            string last = null;
            string storyLast = null;
            string randomLast = null;
            string radioLast = null;
            // 作者的字定语词
            string deZiDingYu = null;
            last = POSUtils.GetWordByPOS(tags, POSConstants.SuffixMusic);//myWords.get("音乐后缀词");
            storyLast = POSUtils.GetWordByPOS(tags, POSConstants.SuffixStory); //myWords.get("故事后缀词");
            randomLast = POSUtils.GetWordByPOS(tags, POSConstants.SuffixRandom); //myWords.get("随机后缀词");
            radioLast = POSUtils.GetWordByPOS(tags, POSConstants.SuffixBroadcast); //myWords.get("广播后缀词");
            deZiDingYu = POSUtils.GetWordByPOS(tags, POSConstants.AdvAuthor); //myWords.get("作者的字定语词");
            if (!RealyLast(radioLast, raw))
            {
                radioLast = null;
            }

            if (!string.IsNullOrEmpty(last))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(last) - raw.IndexOf(verb) - verb.Length);
                data2 = MediaInfoHelper.Instance.GetMusicOnline(noLast, "");
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get item from web with whole name: " + noLast);
                    return data2;
                }
            }

            // 处理音乐后缀词如播放陈奕迅的十年这首歌
            string afterVerbSinger1 = "";
            string afterVerbSong1 = "";
            if (!string.IsNullOrEmpty(last))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(last) - raw.IndexOf(verb) - verb.Length);
                String afterVerb = noLast;
                if (!string.IsNullOrEmpty(deZiDingYu))
                {
                    afterVerb = afterVerb.Replace(deZiDingYu, "的");
                }

                int deIndex = afterVerb.IndexOf("的");
                afterVerbSinger1 = afterVerb.Substring(0, deIndex);
                afterVerbSong1 = afterVerb.Substring(deIndex + 1);

                data2 = MediaInfoHelper.Instance.GetMusicOnline(afterVerbSong1, afterVerbSinger1);
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get item from web with suffix artist and song: " + afterVerbSong1 + ", " + afterVerbSinger1);
                    return data2;
                }
                else
                {// 如果没有搜索到陈奕迅的十年，则任选一首十年播放
                    data2 = MediaInfoHelper.Instance.GetMusicOnline(afterVerbSong1, "");
                    if (data2 != null && data2.IsValid())
                    {
                        logger.Debug("get item from web with suffix song: " + afterVerbSong1);
                        return data2;
                    }
                }
            }

            // 处理陈奕迅的十年这种情况
            string afterVerbSinger2 = "";
            string afterVerbSong2 = "";
            string afterVerb2 = raw.Substring(raw.IndexOf(verb) + verb.Length);
            if (!string.IsNullOrEmpty(deZiDingYu))
            {
                afterVerb2 = afterVerb2.Replace(deZiDingYu, "的");
            }

            int deIndex2 = afterVerb2.IndexOf("的");
            if (deIndex2 != -1)
            {
                afterVerbSinger2 = afterVerb2.Substring(0, deIndex2);
                afterVerbSong2 = afterVerb2.Substring(deIndex2 + 1);

                data2 = MediaInfoHelper.Instance.GetMusicOnline(afterVerbSong2, afterVerbSinger2);

            }
            if (data2 != null && data2.IsValid())
            {
                logger.Debug("get item from web with artist and song: " + afterVerbSong2 + ", " + afterVerbSinger2);
                return data2;
            }
            else
            {// 如果没有搜索到陈奕迅的十年，则任选一首十年播放
                data2 = MediaInfoHelper.Instance.GetMusicOnline(afterVerbSong2, "");
                if (data2 != null && data2.IsValid())
                {
                    logger.Debug("get item from web with song: " + afterVerbSong2);
                    return data2;
                }
            }

            return null;
        }

        public bool RealyLast(string last, string initWords)
        {
            if (last != null)
            {
                int index = initWords.IndexOf(last);
                if (initWords.Length == (index + last.Length))
                    return true;
                return false;
            }
            return false;
        }

        private string GetAction(Context context)
        {
            string[][] input = context.Tags;
            string[] allMediaVerb = POSUtils.GetWordsByPOS(input, POSConstants.VerbMedia);
            if (allMediaVerb == null || allMediaVerb.Length == 0)
            {
                allMediaVerb = POSUtils.GetWordsByPOS(input, POSConstants.VerbMixed);
            }

            if (allMediaVerb == null || allMediaVerb.Length == 0)
            {
                return "";
            }
            string item = allMediaVerb[0];
            string command = DBHelperFactory.GetInstance().TranslateCommand(item);
            return command;
        }

        private List<Intent> ProcessSwitch(Context context)
        {
            List<Intent> intents = new List<Intent>();
            MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("music");
            if (data != null)
            {
                Intent intent = new Intent();
                intent.Action = Actions.Play;
                intent.Domain = Domains.Media;
                intent.Data = data;
                intent.Score = 90;
                intents.Add(intent);
            }

            return intents;
        }

        private List<Intent> ProcessStart2(Context context)
        {
            string[][] input = context.Tags;
            List<Intent> intents = new List<Intent>();
            List<string> rawArtistlist = SearchPersonNameList(context);
            logger.Debug("find person name candidates: " + Utils.PrintList<string>(rawArtistlist));
            List<string> rawSonglist = SearchSongNameList(context);
            logger.Debug("find song candidates: " + Utils.PrintList<string>(rawSonglist));
            List<string> rawGenrelist = SearchGenreList(context);
            logger.Debug("find genre candidates: " + Utils.PrintList<string>(rawGenrelist));
            //List<string> rawCategory = SearchCategory(context);

            string artist = GetArtist(rawArtistlist);
            logger.Debug("artist find: " + artist);
            string songName = GetSongName(rawSonglist);
            logger.Debug("song name find: " + songName);
            string genre = GetGenre(rawGenrelist);
            logger.Debug("genre find: " + genre);

            if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(songName))
            {
                MediaData data = MediaDataProviderFactory.GetProvider().GetMusic(artist, songName);
                if (data != null)
                {
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 100;
                    intents.Add(intent);
                }
                else
                {
                    data = MediaInfoHelper.Instance.GetMusicByName(songName);
                    if (data != null)
                    {
                        Intent intent = new Intent();
                        intent.Action = Actions.Play;
                        intent.Domain = Domains.Media;
                        intent.Data = data;
                        intent.Score = 100;
                        intents.Add(intent);
                    }
                }

            }

            if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(genre))
            {
                string[] words = POSUtils.GetWordsByPOS(input, POSConstants.NounGenre);
                MediaData data = MediaInfoHelper.Instance.GetMusicByArtistGenre(artist, words[0]);
                if (data != null)
                {
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 90;
                    intents.Add(intent);
                }
                else
                {
                    data = MediaInfoHelper.Instance.GetMusicByGenre(words[0]);
                    if (data != null)
                    {
                        Intent intent = new Intent();
                        intent.Action = Actions.Play;
                        intent.Domain = Domains.Media;
                        intent.Data = data;
                        intent.Score = 70;
                        intents.Add(intent);
                    }
                }
            }

            if (string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(songName))
            {
                MediaData data = MediaInfoHelper.Instance.GetMusicByName(songName);
                if (data != null)
                {
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 80;
                    intents.Add(intent);
                }
            }

            if (string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(genre))
            {
                string[] words = POSUtils.GetWordsByPOS(input, POSConstants.NounGenre);
                MediaData data = MediaInfoHelper.Instance.GetMusicByGenre(words[0]);
                if (data != null)
                {
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 70;
                    intents.Add(intent);
                }
            }

            //has person name but not a documented artist, has a word which looks like a song name, try to find it online.
            if (rawArtistlist.Count > 0 && string.IsNullOrEmpty(artist)
                && rawSonglist.Count > 0 && string.IsNullOrEmpty(songName))
            {
                MediaData data = MediaDataProviderFactory.GetProvider().GetMusic(rawArtistlist[0], rawSonglist[0]);

                if (data != null)
                {
                    Intent intent = new Intent();
                    intent.Action = Actions.Play;
                    intent.Domain = Domains.Media;
                    intent.Data = data;
                    intent.Score = 50;
                    intents.Add(intent);
                }

            }

            return intents;
        }

        private string GetGenre(List<string> rawGenrelist)
        {
            if (rawGenrelist.Count > 0)
            {
                return rawGenrelist[0];
            }

            return rawGenrelist[0];
        }

        private string GetSongName(List<string> rawSonglist)
        {
            if (rawSonglist.Count > 0)
            {
                return rawSonglist[0];
            }

            return "";
        }

        private string GetArtist(List<string> rawArtistlist)
        {
            return "";
        }

        private List<string> SearchGenreList(Context context)
        {
            List<string> genres = new List<string>();
            string[][] tags = context.Tags;
            string[] genreList = POSUtils.GetWordsByPOS(tags, POSConstants.NounGenre);

            genres.AddRange(genreList);
            return genres;
        }

        private List<string> SearchSongNameList(Context context)
        {
            //find the content after the verb
            string[][] tags = context.Tags;
            int index = -1;
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i][1].Equals(POSConstants.VerbMedia, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            //string content = context.RawString.Substring(context.RawString.IndexOf(tags[index][0]))
            return new List<string>();
        }

        //find the raw person name in the context
        private List<string> SearchPersonNameList(Context context)
        {
            List<string> names = new List<string>();
            string[][] tags = context.Tags;
            string[] nameList = POSUtils.GetWordsByPOS(tags, POSConstants.NounName);

            if (nameList == null || nameList.Length == 0)
            {

            }
            names.AddRange(nameList);
            return names;
        }
    }
}
