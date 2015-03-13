using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public enum Models
    {
        SEG,
        TAG,
        SEG_TAG,
        NER,
        PARSER,
        ALL,
    }

    public class CNFactory
    {
        private static CNFactory instance = new CNFactory();
        public static CWSTagger seg;
        public static POSTagger pos;
        //public static NERTagger ner;
        //public static JointParser parser;
        public static NLPDictionary dict = new NLPDictionary(true);

        private static bool isEnFilter = true;

        private CNFactory()
        {
        }

        public static void loadDict(params string[] path)
        {
            foreach (string file in path)
            {
                dict.addFile(file);
            }
            setDict();
        }

        private static void setDict()
        {
            if (dict == null || dict.size() == 0)
                return;
            if (pos != null)
            {
                pos.setDictionary(dict, true);
            }
            else if (seg != null)
            {
                seg.setDictionary(dict);
            }
        }

        public static CNFactory GetInstance(string path)
        {
            return GetInstance(path, Models.ALL);
        }

        public static CNFactory GetInstance(string path, Models model)
        {
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
                
            if (model == Models.SEG)
            {
                LoadSeg(path);
            }
            else if (model == Models.SEG)
            {
                LoadTag(path);
            }
            else if (model == Models.SEG_TAG)
            {
                LoadSeg(path);
                LoadTag(path);
            }
            else if (model == Models.ALL)
            {
                LoadSeg(path);
                LoadTag(path);
                LoadNER(path);
                LoadParser(path);
            }
            SetDict();
            return instance;
        }

        private static void SetDict()
        {
            //if (dict == null || dict.size() == 0)
            //    return;
            //if (pos != null)
            //{
            //    pos.setDictionary(dict, true);
            //}
            //else if (seg != null)
            //{
            //    seg.setDictionary(dict);
            //}
        }

        public static void LoadParser(string path)
        {
            //if (parser == null)
            //{
            //    string file = path + parseModel;
            //    parser = new JointParser(file);
            //}
        }

        public static void LoadNER(string path)
        {
            //if (ner == null && pos != null)
            //{
            //    ner = new NERTagger(pos);
            //}
        }

        public static void LoadSeg(string path)
        {
            if (seg == null)
            {
                //string file = path + segModel;
                seg = new CWSTagger("");
                seg.setEnFilter(isEnFilter);
            }
        }

        public static void LoadTag(string path)
        {
            if (pos == null)
            {
                pos = new POSTagger(seg, "");
            }
        }

        public string tag2String(string input)
        {
            if (pos == null || seg == null)
                return null;
            return pos.tag(input);
        }

        public string[][] tag(string input)
        {
            if (pos == null || seg == null)
                return null;
            return pos.tag2Array(input);
        }
    }
}
