using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class CWSTagger : AbstractTagger
    {
        // 考虑不同CWStagger可能使用不同dict，所以不使用静态
        private DictLabel dictPipe = null;
        private Pipe oldfeaturePipe = null;
        /**
         * 是否对英文单词进行预处理
         */
        private bool isEnFilter = false;

        /**
         * 是否对英文单词进行预处理，将连续的英文字母看成一个单词
         * @param b
         */
        public void setEnFilter(bool b)
        {
            isEnFilter = b;
            prePipe = new String2Sequence(isEnFilter);
        }

        /**
         * 构造函数，使用LinearViterbi解码
         * @param str 模型文件名
         * @throws LoadModelException
         */
        public CWSTagger(String str)
            : base(str)
        {
            prePipe = new String2Sequence(isEnFilter);

            //		DynamicViterbi dv = new DynamicViterbi(
            //				(LinearViterbi) cl.getInferencer(), 
            //				cl.getAlphabetFactory().buildLabelAlphabet("labels"), 
            //				cl.getAlphabetFactory().buildFeatureAlphabet("features"),
            //				false);
            //		dv.setDynamicTemplets(DynamicTagger.getDynamicTemplet("example-data/structure/template_dynamic"));
            //		cl.setInferencer(dv);
        }

        public string tag(string src)
        {
            if (src == null || src.Length == 0)
                return src;
            string[] sents = Sentenizer.split(src);
            string tag = "";

            for (int i = 0; i < sents.Length; i++)
            {
                Instance inst = new Instance(sents[i]);
                String[] preds = _tag(inst);
                String s = FormatCWS.toString(inst, preds, delim);
                tag += s;
                if (i < sents.Length - 1)
                    tag += delim;
            }

            return tag;
        }

        public string[] tag2Array(String src)
        {
            List<string> words = tag2List(src);
            return words.ToArray();
        }

        public List<string> tag2List(string src)
        {
            if (src == null || src.Length == 0)
                return null;
            List<string> res = null;

            Instance inst = new Instance(src);
            String[] preds = _tag(inst);
            res = FormatCWS.toList(inst, preds);

            return res;
        }

        public override void loadFrom(string modelfile)
        {
            templets = LoadTempltes();
            cl = LoadClassifier();
        }

        private Linear LoadClassifier()
        {
            AlphabetFactory factory = AlphabetFactory.buildFactory();
            IFeatureAlphabet fab = factory.DefaultFeatureAlphabet();
            if (fab is StringFeatureAlphabet)
            {
                (fab as StringFeatureAlphabet).LoadData(@"C:/Temp/stringFeature");
            }
            factory.DefaultLabelAlphabet().LoadData("");

            LinearViterbi lbi = new LinearViterbi(templets, 4);
            Linear c = new Linear(lbi, factory);
            c.LoadData(@"C:/Temp/weights");

            return c;
        }

        private TempletGroup LoadTempltes()
        {
            TempletGroup g = new TempletGroup();
            g.load(@"C:\Temp\templets.txt");
            return g;
        }

        public void removeDictionary()
        {
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

        public void setDictionary(NLPDictionary dict)
        {
            removeDictionary();
            initDict(dict);
        }

        private void initDict(NLPDictionary dict)
        {
            dictPipe = new DictLabel(dict, labels);

            oldfeaturePipe = featurePipe;
            featurePipe = new SeriesPipes(new Pipe[] { dictPipe, featurePipe });

            LinearViterbi dv = new ConstraintViterbi(
                    (LinearViterbi)getClassifier().getInferencer());
            getClassifier().setInferencer(dv);
        }
    }
}
