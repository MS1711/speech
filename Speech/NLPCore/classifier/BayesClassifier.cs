using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.classifier
{
    [Serializable]
    public class BayesClassifier : AbstractClassifier
    {
        protected ItemFrequency tf;
        protected Pipe pipe;
        protected FeatureSelect fs;

        public Predict<int> doClassify(Instance instance, int n)
        {

            int typeSize = tf.getTypeSize();
            float[] score = new float[typeSize];

            object obj = instance.getData();
            if (!(obj is HashSparseVector))
            {
                return null;
            }
            HashSparseVector data = (HashSparseVector)obj;
            if (fs != null)
                data = fs.select(data);
            Dictionary<int, float>.Enumerator it = data.data.GetEnumerator();
            float feaSize = tf.getFeatureSize();
            while (it.MoveNext())
            {
                KeyValuePair<int, float> item = it.Current;
                if (item.Key == 0)
                    continue;
                int feature = item.Key;
                for (int type = 0; type < typeSize; type++)
                {
                    float itemF = tf.getItemFrequency(feature, type);
                    float typeF = tf.getTypeFrequency(type);
                    score[type] += (float)(item.Value * Math.Log((itemF + 0.1) / (typeF + feaSize)));
                }
            }

            Predict<int> res = new Predict<int>(n);
            for (int type = 0; type < typeSize; type++)
                res.add(type, score[type]);

            return res;
        }

        /**
         * 得到类标签
         * @param idx 类标签对应的索引
         * @return
         */
        public String getLabel(int idx)
        {
            return factory.DefaultLabelAlphabet().lookupString(idx);
        }


        public void fS_CS(float percent) { featureSelectionChiSquare(percent); }
        public void featureSelectionChiSquare(float percent)
        {
            fs = new FeatureSelect(tf.getFeatureSize());
            fs.fS_CS(tf, percent);
        }
        public void fS_CS_Max(float percent) { featureSelectionChiSquareMax(percent); }
        public void featureSelectionChiSquareMax(float percent)
        {
            fs = new FeatureSelect(tf.getFeatureSize());
            fs.fS_CS_Max(tf, percent);
        }
        public void fS_IG(float percent) { featureSelectionInformationGain(percent); }
        public void featureSelectionInformationGain(float percent)
        {
            fs = new FeatureSelect(tf.getFeatureSize());
            fs.fS_IG(tf, percent);
        }
        public void noFeatureSelection()
        {
            fs = null;
        }
        public ItemFrequency getTf()
        {
            return tf;
        }

        public void setTf(ItemFrequency tf)
        {
            this.tf = tf;
        }
        public Pipe getPipe()
        {
            return pipe;
        }

        public void setPipe(Pipe pipe)
        {
            this.pipe = pipe;
        }

        public void setFactory(AlphabetFactory factory)
        {
            this.factory = factory;
        }
        public AlphabetFactory getFactory()
        {
            return factory;
        }

        public Predict<string> doClassify(Instance instance, LabelParser.Type type, int n)
        {
            Predict<int> res = (Predict<int>)doClassify(instance, n);
            return LabelParser.parse(res, factory.DefaultLabelAlphabet(), type);
        }

        public void saveTo(string file)
        {
            Stream fStream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fStream, this);
            fStream.Close();
        }

        public static BayesClassifier loadFrom(string file)
        {
            Stream fStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            var cl = (BayesClassifier)formatter.Deserialize(fStream);
            fStream.Close();
            return cl;
        }

        public override Predict<int[]> classify(Instance instance, int n)
        {
            throw new NotImplementedException();
        }

        public override Predict<string[]> classify(Instance instance, LabelParser.Type type, int n)
        {
            throw new NotImplementedException();
        }
    }
}
