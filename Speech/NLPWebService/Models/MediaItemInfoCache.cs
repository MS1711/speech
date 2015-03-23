using log4net;
using NLPWebService.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPWebService.Models
{
    public class MediaItemInfoCache
    {
        private static ILog logger = LogManager.GetLogger("common");
        private static MediaItemInfoCache instance = new MediaItemInfoCache();
        private SortedSet<string> artistList = new SortedSet<string>();
        private SortedSet<string> songList = new SortedSet<string>();

        private MediaItemInfoCache()
        {
            Load(ConfigurationManager.AppSettings["artistCachePath"], ConfigurationManager.AppSettings["songCachePath"]);
        }

        public CorrectedInfo GetSimilarArtist(string name)
        {
            return GetSimilar(artistList, name);
        }

        public CorrectedInfo GetSimilar(SortedSet<string> set, string name)
        {
            CorrectedInfo info = new CorrectedInfo();
            char first = name[0];
            foreach (var item in set)
            {
                string n = item;
                float v = Utilities.Leven(n, name);
                if (v > info.Score)
                {
                    info.Score = v;
                    info.Item = item;
                }
            }

            
            logger.Debug("similarest: " + info.Item + ", score: " + info.Score);
            return info;
        }

        public CorrectedInfo GetSimilarSong(string name)
        {
            return GetSimilar(songList, name);
        }

        public void AddArtist(string name)
        {
            Add(artistList, name);
        }

        private void Add(SortedSet<string> set, string name)
        {
            if (!set.Contains(name))
            {
                set.Add(name);
            }
        }

        public void AddSong(string name)
        {
            Add(songList, name);
        }

        public void Load(string artist, string song)
        {
            //DBHelperFactory.GetInstance().LoadAllArtist(this);
            //DBHelperFactory.GetInstance().LoadAllSong(this);

            using (StreamReader sr = new StreamReader(artist, Encoding.UTF8))
            {
                string str = sr.ReadLine();
                while (str != null)
                {
                    artistList.Add(str);
                    str = sr.ReadLine();
                }
            }

            using (StreamReader sr = new StreamReader(song, Encoding.UTF8))
            {
                string str = sr.ReadLine();
                while (str != null)
                {
                    songList.Add(str);
                    str = sr.ReadLine();
                }
            }
        }

        public void DumpArtist(string file)
        {
            using(StreamWriter wr = new StreamWriter(file, false, Encoding.UTF8))
            {
                foreach (var item in artistList)
                {
                    wr.WriteLine(item);
                }
            }

        }

        public void DumpSong(string file)
        {
            using (StreamWriter wr = new StreamWriter(file, false, Encoding.UTF8))
            {
                foreach (var item in songList)
                {
                    wr.WriteLine(item);
                }
            }

        }

        public static MediaItemInfoCache Instance
        {
            get { return MediaItemInfoCache.instance; }
            set { MediaItemInfoCache.instance = value; }
        }
    }
}
