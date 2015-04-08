using NL2ML.consts;
using NL2ML.utils;
using NLPCore;
using NLPCore.utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.classifier
{
    public class MediaKeyWordExtractor
    {
        private static MediaKeyWordExtractor instance = new MediaKeyWordExtractor();
        private CWSTagger artistCwsTagger;
        private POSTagger artistPosTagger;
        private WordExtract artistExtractor;

        private CWSTagger songCwsTagger;
        private POSTagger songPosTagger;
        private WordExtract songExtractor;

        private MediaKeyWordExtractor()
        {
            string modelPath = ConfigurationManager.AppSettings["ModelPath"];
            StopWords sw = new StopWords(modelPath + "stopwords");
            NLPDictionary dict = new NLPDictionary(true);
            string[] dictPath = new string[]{
					modelPath + "artistDict.txt"
			};
			
			foreach (string str in dictPath) {
				dict.addFile(str);
			}
            

            artistCwsTagger = new CWSTagger(modelPath);
            artistCwsTagger.setDictionary(dict);

            artistPosTagger = new POSTagger(artistCwsTagger, modelPath);
            artistPosTagger.setDictionary(dict, false);

            artistExtractor = new WordExtract(artistCwsTagger, sw);


            dict = new NLPDictionary(true);
            dictPath = new string[]{
					modelPath + "songDict.txt"
			};

            foreach (string str in dictPath)
            {
                dict.addFile(str);
            }


            songCwsTagger = new CWSTagger(modelPath);
            songCwsTagger.setDictionary(dict);

            songPosTagger = new POSTagger(songCwsTagger, modelPath);
            songPosTagger.setDictionary(dict, false);

            songExtractor = new WordExtract(songCwsTagger, sw);
        }

        public static MediaKeyWordExtractor Instance
        {
            get { return MediaKeyWordExtractor.instance; }
        }

        public string[] GetArtist(string str)
        {
            string[][] tags = artistPosTagger.tag2Array(str);
            Dictionary<string, int> words = artistExtractor.extract(str, 20);

            if (POSUtils.HasPOS(tags, POSConstants.NounArtist))
            {
                string[] artists = POSUtils.GetWordsByPOS(tags, POSConstants.NounArtist);
                return artists;
            }

            return null;
        }

        public string[] GetSongs(string str)
        {
            string[][] tags = songPosTagger.tag2Array(str);
            Dictionary<string, int> words = songExtractor.extract(str, 20);

            if (POSUtils.HasPOS(tags, POSConstants.NounSong))
            {
                string[] artists = POSUtils.GetWordsByPOS(tags, POSConstants.NounSong);
                return artists;
            }

            return null;
        }
    }
}
