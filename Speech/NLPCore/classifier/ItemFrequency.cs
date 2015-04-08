using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.classifier
{
    [Serializable]
    public class ItemFrequency
    {
        private int size, featureSize, typeSize;
        private int[] itemFrequency;
        private int[] typeFrequency;
        private int[] featureFrequency;
        private int total = 0;

        public ItemFrequency(int featureSize, int typeSize)
        {
            init(featureSize, typeSize);
        }
        public ItemFrequency(InstanceSet instSet):
            this(instSet, instSet.getAlphabetFactory())
        {
            
        }
        public ItemFrequency(InstanceSet instSet, AlphabetFactory af) :
            this(af.getFeatureSize(), af.getLabelSize())
        {
            statistic(instSet);
        }

        public void init(int featureSize, int typeSize)
        {
            this.setFeatureSize(featureSize);
            this.setTypeSize(typeSize);
            size = featureSize * typeSize;
            itemFrequency = new int[size];
            typeFrequency = new int[typeSize];
            featureFrequency = new int[featureSize];
            total = 0;
        }
        public int getTypeSize()
        {
            return typeSize;
        }
        public void setTypeSize(int typeSize)
        {
            this.typeSize = typeSize;
        }
        public int getFeatureSize()
        {
            return featureSize;
        }
        public void setFeatureSize(int featureSize)
        {
            this.featureSize = featureSize;
        }


        public int getFeatureFrequency(int index)
        {
            return featureFrequency[index];
        }
        public int getTypeFrequency(int index)
        {
            return typeFrequency[index];
        }
        public int getItemFrequency(int index)
        {
            return itemFrequency[index];
        }
        public int getItemFrequency(int feature, int type)
        {
            return itemFrequency[feature + type * featureSize];
        }
        public void setItemFrequency(int index, int frequency)
        {
            setItemFrequency(index % featureSize, index / featureSize, index, frequency);
        }
        public void setItemFrequency(int feature, int type, int frequency)
        {
            setItemFrequency(feature, type, feature + type * featureSize, frequency);
        }
        private void setItemFrequency(int feature, int type, int index, int frequency)
        {//index=feature+type*featureSize;
            int diff = frequency - itemFrequency[index];
            featureFrequency[feature] += diff;
            typeFrequency[type] += diff;
            itemFrequency[index] = frequency;
            total += diff;
        }
        public void addItemFrequency(int index, int diff)
        {
            addItemFrequency(index % featureSize, index / featureSize, index, diff);
        }
        public void addItemFrequency(int feature, int type, int diff)
        {
            addItemFrequency(feature, type, feature + type * featureSize, diff);
        }
        private void addItemFrequency(int feature, int type, int index, int diff)
        {//index=feature+type*featureSize;
            featureFrequency[feature] += diff;
            typeFrequency[type] += diff;
            itemFrequency[index] += diff;
            total += diff;
        }
        public int getTotal()
        {
            return total;
        }
        public void setTotal(int total)
        {
            this.total = total;
        }
        /**
         * 统计数据集词频
         * @param instSet 数据集
         */
        public void statistic(InstanceSet instSet)
        {
            int numSamples = instSet.Count;
            for (int i = 0; i < numSamples; i++)
            {
                if (i % 1000 == 0)
                    Console.WriteLine((i + 0.0) / numSamples);
                Instance inst = instSet[i];
                oneStepStatistic(inst);
            }
        }

        private bool oneStepStatistic(Instance inst)
        {
            if (inst == null)
                return false;
            int[] type;
            object t = inst.getTarget();
            if (t is int)
            {
                type = new int[1];
                type[0] = int.Parse(t.ToString());
            }
            else
            {
                return false;
            }

            HashSparseVector data = (HashSparseVector)inst.getData();
            Dictionary<int, float>.Enumerator it = data.data.GetEnumerator();
            while (it.MoveNext())
            {
                KeyValuePair<int, float> item = it.Current;
                int feature = item.Key;
                for (int i = 0; i < type.Length; i++)
                {
                    addItemFrequency(feature, type[i], (int)item.Value);
                }
            }

            return true;
        }
    }
}
