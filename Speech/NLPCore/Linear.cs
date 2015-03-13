using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class Linear : AbstractClassifier
    {
	    /**
	     * 特征转换器
	     */
        protected Inferencer inferencer;

        protected Pipe pipe;
	    /**
	     * 特征权重向量数组，每一类对应一个向量
	     */
	    public HashSparseVector[] weights;
	    /**
	     * 推理器
	     */
        public Linear(Inferencer inferencer, AlphabetFactory factory)
        {
            this.inferencer = inferencer;
            this.factory = factory;
        }

        public void LoadData(string file)
        {
            List<float> fw = new List<float>();
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                string str = sr.ReadLine();
                while (str != null)
                {
                    fw.Add(float.Parse(str));
                    str = sr.ReadLine();
                }
            }
            this.inferencer.setWeights(fw.ToArray());
        }

        public Inferencer getInferencer()
        {
            return inferencer;
        }

        public void setInferencer(Inferencer inferencer)
        {
            this.inferencer = inferencer;
        }

        override public Predict<string[]> classify(Instance instance, LabelParser.Type t, int n)
        {
            Predict<int[]> res = (Predict<int[]>)inferencer.getBest(instance, n);
            return LabelParser.parse(res, factory.DefaultLabelAlphabet(), t);
        }

        public override Predict<int[]> classify(Instance instance, int n)
        {
            return (Predict<int[]>)inferencer.getBest(instance, n);
        }
    }
}
