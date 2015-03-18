using NL2ML.models;
using NL2ML.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.dbhelper
{
    class MediaInfoCache
    {
        private static MediaInfoCache instance = new MediaInfoCache();
        public SortedSet<MediaData> mediaSet = new SortedSet<MediaData>(new MediaDataComparer());

        internal static MediaInfoCache Instance
        {
            get { return MediaInfoCache.instance; }
        }

        private MediaInfoCache()
        {
        }

        public void Add(MediaData data)
        {
            mediaSet.Add(data);
        }

        public void Load()
        {
            DBHelperFactory.GetInstance().LoadAllMediaInfo(mediaSet);
        }

        public MediaData GetSimilar(string name)
        {
            float max = 0;
            MediaData data = null;
            foreach (var item in mediaSet)
            {
                string n = item.Name;
                float v = Utils.Leven(n, name);
                if (v > max)
                {
                    max = v;
                    data = item;
                }
            }

            return data;
        }
    }

    class MediaDataComparer : IComparer<MediaData>
    {

        public int Compare(MediaData x, MediaData y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
