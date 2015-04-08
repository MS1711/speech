using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.classifier
{
    [Serializable]
    public class FeatureSelect
    {
        private bool[] isUseful;
        int size;

        public FeatureSelect(int size)
        {
            this.size = size;
            isUseful = new bool[size];

            for (int i = 0; i < size; i++)
            {
                isUseful[i] = true;
            }
        }

        /**
         * 特征选择 卡方法 保留特征为每个类别前percent的最大卡方值特征的并集
         * @param tf 词频类
         * @param percent 保留百分比
         */
        public void fS_CS(ItemFrequency tf, float percent)
        {
            featureSelectionChiSquare(tf, percent);
        }

        public void featureSelectionChiSquare(ItemFrequency tf, float percent)
        {
            int feaSize = tf.getFeatureSize();
            int typeSize = tf.getTypeSize();
            isUseful = new bool[feaSize];
            for (int i = 0; i < feaSize; i++)
            {
                isUseful[i] = false;
            }

            for (int j = 0; j < typeSize; j++)
            {
                Heap heap = new Heap((int)(feaSize * percent), true);
                for (int i = 0; i < feaSize; i++)
                {
                    double A, B, C, D, AC, AB, N;
                    N = tf.getTotal();
                    A = tf.getItemFrequency(i, j);
                    AB = tf.getFeatureFrequency(i);
                    AC = tf.getTypeFrequency(j);
                    B = AB - A;
                    C = AC - A;
                    D = N - AB - C;
                    double score = (A * D - B * C) * (A * D - B * C) / AB / (C + D);
                    heap.insert(score, i);
                }
                List<int> data = heap.getData();
                for (int i = 1; i < data.Count; i++)
                    isUseful[data[i]] = true;
            }
            int total = 0;
            for (int i = 0; i < feaSize; i++)
            {
                if (isUseful[i])
                    total++;
            }
        }
        /**
         * 特征选择 卡方法 
         * 对特征对每个类别求卡方，保留最大项，保留结果为前percent的特征
         * @param tf 词频类
         * @param percent 保留百分比
         */
        public void fS_CS_Max(ItemFrequency tf, float percent) { featureSelectionChiSquareMax(tf, percent); }
        public void featureSelectionChiSquareMax(ItemFrequency tf, float percent)
        {
            int feaSize = tf.getFeatureSize();
            int typeSize = tf.getTypeSize();
            isUseful = new bool[feaSize];
            for (int i = 0; i < feaSize; i++)
            {
                isUseful[i] = false;
            }
            Heap heap = new Heap((int)(feaSize * percent), true);
            for (int i = 0; i < feaSize; i++)
            {
                double max = 0;
                for (int j = 0; j < typeSize; j++)
                {
                    double A, B, C, D, AC, AB, N;
                    N = tf.getTotal();
                    A = tf.getItemFrequency(i, j);
                    AB = tf.getFeatureFrequency(i);
                    AC = tf.getTypeFrequency(j);
                    B = AB - A;
                    C = AC - A;
                    D = N - AB - C;
                    double score = (A * D - B * C) * (A * D - B * C) / AB / (C + D) / AC / (B + D);
                    if (score > max)
                        max = score;
                }

                heap.insert(max, i);
            }
            List<int> data = heap.getData();
            for (int i = 1; i < data.Count; i++)
                isUseful[data[i]] = true;
            int total = 0;
            for (int i = 0; i < feaSize; i++)
            {
                if (isUseful[i])
                    total++;
            }
        }
        /**
         * 特征选择 信息增益 
         * @param tf 词频类
         * @param percent 保留百分比
         */
        public void fS_IG(ItemFrequency tf, float percent) { featureSelectionInformationGain(tf, percent); }
        public void featureSelectionInformationGain(ItemFrequency tf, float percent)
        {
            int feaSize = tf.getFeatureSize();
            int typeSize = tf.getTypeSize();
            isUseful = new bool[feaSize];
            for (int i = 0; i < feaSize; i++)
            {
                isUseful[i] = false;
            }
            Heap heap = new Heap((int)(feaSize * percent), true);
            for (int i = 0; i < feaSize; i++)
            {
                double ig = 0;
                for (int j = 0; j < typeSize; j++)
                {
                    double A, B, C, D, AC, AB, N;
                    N = tf.getTotal();
                    A = tf.getItemFrequency(i, j);
                    AB = tf.getFeatureFrequency(i);
                    AC = tf.getTypeFrequency(j);
                    B = AB - A;
                    C = AC - A;
                    D = N - AB - C;
                    ig += -AC / N * Math.Log(AC / N);
                    ig += AB / N * A / N * Math.Log(A / N);
                    ig += (1 - AB / N) * C / N * Math.Log(C / N); ;
                }
                heap.insert(ig, i);
            }
            List<int> data = heap.getData();
            for (int i = 1; i < data.Count; i++)
                isUseful[data[i]] = true;
            int total = 0;
            for (int i = 0; i < feaSize; i++)
            {
                if (isUseful[i])
                    total++;
            }
        }
        public void noFeatureSelection()
        {
            for (int i = 0; i < isUseful.Length; i++)
            {
                isUseful[i] = true;
            }
        }
        public HashSparseVector select(HashSparseVector vec)
        {
            HashSparseVector sv = new HashSparseVector();
            Dictionary<int, float>.Enumerator it = vec.data.GetEnumerator();
            while (it.MoveNext())
            {
                KeyValuePair<int, float> item = it.Current;
                if (isUseful[item.Key])
                    sv.put(item.Key, item.Value);
            }
            return sv;
        }
    }
}
