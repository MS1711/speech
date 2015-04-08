using NLPCore.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class InstanceSet : List<Instance>
    {
        /**
         * 本样本集合默认的数据类型转换管道
         */
        private Pipe pipes = null;
        /**
         * 本样本集合对应的特征和标签索引字典管理器
         */
        private AlphabetFactory factory = null;
    
    
        public int numFeatures = 0;
        public String name = "";

        public AlphabetFactory getAlphabetFactory()
        {
            return factory;
        }

        public InstanceSet(Pipe pipes) {
            this.pipes = pipes;
        }

        public Pipe getPipes()
        {
            return pipes;
        }

        public InstanceSet(Pipe pipes, AlphabetFactory factory) {
            this.pipes = pipes;
            this.factory = factory;
        }

        public InstanceSet(AlphabetFactory factory) {
            this.factory = factory;
        }
    
        public InstanceSet() {
        }

        public Instance getInstance(int idx)
        {
            if (idx < 0 || idx > this.Count)
                return null;
            return this[idx];
        }

        public void loadThruStagePipes(Reader reader)
        {
            SeriesPipes p = (SeriesPipes)pipes;
            // 通过迭代加入样本
            Pipe p1 = p.getPipe(0);
            while (reader.hasNext())
            {
                Instance inst = reader.next();
                if (inst != null)
                {
                    if (p1 != null)
                        p1.addThruPipe(inst);
                    this.Add(inst);

                };
            }
            for (int i = 1; i < p.size(); i++)
                p.getPipe(i).process(this);
        }

        public InstanceSet[] Split(float percent)
        {
            Shuffle(this);
            int length = this.Count;
            InstanceSet[] sets = new InstanceSet[2];
            sets[0] = new InstanceSet(pipes, factory);
            sets[1] = new InstanceSet(pipes, factory);
            int idx = (int)Math.Round(percent * length);
            sets[0].AddRange(GetRange(0, idx));
            if (idx < length)
                sets[1].AddRange(GetRange(idx, length - idx - 1));
            return sets;
        }

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
