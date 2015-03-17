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

        public InstanceSet(Pipe pipes) {
            this.pipes = pipes;
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
    }
}
