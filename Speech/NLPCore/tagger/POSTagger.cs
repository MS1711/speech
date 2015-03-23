using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class POSTagger : AbstractTagger
    {
        public CWSTagger cws;
        private DictPOSLabel dictPipe = null;
        private Pipe oldfeaturePipe = null;

        public override void loadFrom(string modelfile)
        {
            templets = LoadTempltes(modelfile);
            cl = LoadClassifier(modelfile);
        }

        private Linear LoadClassifier(string path)
        {
            AlphabetFactory factory = AlphabetFactory.buildFactory();
            IFeatureAlphabet fab = factory.DefaultFeatureAlphabet();
            if (fab is StringFeatureAlphabet)
            {
                (fab as StringFeatureAlphabet).LoadData(path + "/stringFeaturePOS");
            }
            factory.DefaultLabelAlphabet().LoadDataPOS(path + "/labelPOS");

            LinearViterbi lbi = new LinearViterbi(templets, 41);
            Linear c = new Linear(lbi, factory);
            c.LoadData(path + "/weightsPOS");
            return c;
        }

        private TempletGroup LoadTempltes(string path)
        {
            TempletGroup g = new TempletGroup();
            g.loadPos(path + "/templetsPOS", path + "/charclassxingdict");
            return g;
        }

        public POSTagger(CWSTagger cwsmodel, String str)
            : base(str)
        {
            cws = cwsmodel;
        }

        public string tag(string src)
        {
            if (src == null || src.Length == 0)
                return src;
            if (cws == null)
            {
                return null;
            }
            string[] words = cws.tag2Array(src);
            if (words.Length == 0)
                return src;

            Instance inst = new Instance(words);
            doProcess(inst);
            int[] pred = (int[])getClassifier().classify(inst).getLabel(0);
            string[] target = labels.lookupString(pred);
            string res = format(words, target);
            return res;
        }

        public String[][] tag2Array(string src)
        {
            if (src == null || src.Length == 0)
                return null;
            if (cws == null)
            {
                return null;
            }
            string[] words = cws.tag2Array(src);
            if (words.Length == 0)
                return null;
            string[] target = null;
            Instance inst = new Instance(words);
            doProcess(inst);
            int[] pred = (int[])getClassifier().classify(inst).getLabel(0);
            target = labels.lookupString(pred);


            string[][] tags = new string[2][];
            tags[0] = words;
            tags[1] = target;
            return tags;
        }

        public string format(string[] words, string[] target)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < words.Length; j++)
            {
                sb.Append(words[j]);
                if (Chars.isWhiteSpace(words[j]))//空格不输出词性
                    continue;
                sb.Append("/");
                sb.Append(target[j]);
                if (j < words.Length - 1)
                    sb.Append(delim);
            }

            string res = sb.ToString();
            return res;
        }

        public void setDictionary(NLPDictionary dict, bool isSetSegDict)
        {
            removeDictionary(isSetSegDict);
            if (cws != null && isSetSegDict)
                cws.setDictionary(dict);
            dictPipe = null;
            dictPipe = new DictPOSLabel(dict, labels);
            oldfeaturePipe = featurePipe;
            featurePipe = new SeriesPipes(new Pipe[] { dictPipe, featurePipe });
            LinearViterbi dv = new ConstraintViterbi(
                    (LinearViterbi)getClassifier().getInferencer(), labels.size());
            getClassifier().setInferencer(dv);
        }

        public void removeDictionary(bool isRemoveSegDict)
        {
            if (cws != null && isRemoveSegDict)
                cws.removeDictionary();

            if (oldfeaturePipe != null)
            {
                featurePipe = oldfeaturePipe;
            }
            LinearViterbi dv = new LinearViterbi(
                    (LinearViterbi)getClassifier().getInferencer());
            getClassifier().setInferencer(dv);

            dictPipe = null;
            oldfeaturePipe = null;
        }
    }
}
