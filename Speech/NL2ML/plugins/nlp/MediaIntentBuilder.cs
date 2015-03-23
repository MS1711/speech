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
                intents.AddRange(ProcessStart(context));
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
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("music");
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

            if (suffix.Equals("故事") || suffix.Equals("童话"))
            {
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("story");
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
            if (suffix.Equals("广播") || suffix.Equals("电台") || suffix.Equals("调频"))
            {
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("radio");
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
            if (suffix.Equals("歌") || suffix.Equals("歌曲") || suffix.Equals("音乐"))
            {
                MediaData data = MediaInfoHelper.Instance.GetRandomMediaByCategory("music");
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

            MediaData data2 = MediaInfoHelper.Instance.GetMediaByName(suffix);
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

            CorrectedInfo songCorrected = MediaInfoHelper.Instance.GetSimilarSong(suffix);
            songCorrected.Category = MediaCategory.Music;
            songCorrected.SongName = songCorrected.Item;
            perhapsList.Add(songCorrected);


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
                logger.Debug("no last: " + noLast);
                data2 = MediaInfoHelper.Instance.GetMusicByName(noLast);
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

                data2 = MediaInfoHelper.Instance.GetMusic(afterVerbSong1, afterVerbSinger1);
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
                else
                {// 如果没有搜索到陈奕迅的十年，则任选一首十年播放
                    data2 = MediaInfoHelper.Instance.GetMusic(afterVerbSong1, "");
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
                    else
                    {
                        songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong1);
                        songCorrected.Category = MediaCategory.Music;
                        songCorrected.SongName = songCorrected.Item;
                        perhapsList.Add(songCorrected);
                    }
                }
            }

            // 处理故事后缀词 如播放白雪公主这个故事
            if (!string.IsNullOrEmpty(storyLast))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(storyLast) - raw.IndexOf(verb) - verb.Length);
                data2 = MediaInfoHelper.Instance.GetMediaByCategory(noLast, "story");
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
                else
                {
                    songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong1);
                    songCorrected.Category = MediaCategory.Story;
                    songCorrected.SongName = songCorrected.Item;
                    perhapsList.Add(songCorrected);
                }
            }
            // 处理随机歌曲后缀词如播放周杰伦的歌
            if (!string.IsNullOrEmpty(randomLast))
            {
                string noLast = raw.Substring(raw.IndexOf(verb) + verb.Length, raw.IndexOf(randomLast) - raw.IndexOf(verb) - verb.Length);
                data2 = MediaInfoHelper.Instance.GetRandomMediaByArtist(noLast);
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
                else
                {
                    songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong1);
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

                data2 = MediaInfoHelper.Instance.GetMusic(afterVerbSong2, afterVerbSinger2);

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
            else
            {// 如果没有搜索到陈奕迅的十年，则任选一首十年播放
                data2 = MediaInfoHelper.Instance.GetMusic(afterVerbSong2, "");
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
                else
                {
                    songCorrected = MediaInfoHelper.Instance.GetSimilarSong(afterVerbSong2);
                    songCorrected.Category = MediaCategory.Music;
                    songCorrected.SongName = songCorrected.Item;
                    perhapsList.Add(songCorrected);
                }
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

                switch (info.Category)
                {
                    case MediaCategory.Music:
                        {
                            if (!string.IsNullOrEmpty(name))
                            {
                                data2 = MediaInfoHelper.Instance.GetMusic(name, artist);
                                if (data2 != null && data2.IsValid())
                                {
                                    logger.Debug("music found: " + data2);
                                    Intent intent = new Intent();
                                    intent.Action = Actions.Play;
                                    intent.Domain = Domains.Media;
                                    intent.Data = data2;
                                    intent.Score = 90;
                                    intents.Add(intent);

                                    return intents;
                                }
                            }
                            else if (!string.IsNullOrEmpty(artist))
                            {
                                data2 = MediaInfoHelper.Instance.GetRandomMediaByArtist(artist);
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
                            }

                            break;
                        }
                    case MediaCategory.Story:
                        {
                            data2 = MediaInfoHelper.Instance.GetMediaByCategory(info.SongName, "story");
                            if (data2 != null && data2.IsValid())
                            {
                                logger.Debug("story found: " + data2);
                                Intent intent = new Intent();
                                intent.Action = Actions.Play;
                                intent.Domain = Domains.Media;
                                intent.Data = data2;
                                intent.Score = 90;
                                intents.Add(intent);

                                return intents;
                            }

                            break;
                        }
                }
                
            }

            return intents;
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
