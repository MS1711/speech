using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class HashFeatureAlphabet : IFeatureAlphabet
    {
        AbstractHashCode hashcode = new MurmurHash();
        private Dictionary<string, string> keyMap;
        Dictionary<int, HashSet<string>> map = new Dictionary<int, HashSet<string>>();
        int count = 0;
        /**
         * 数据
         */
        protected Dictionary<int, int> intdata;
        /**
         * 是否冻结
         */
        protected bool frozen;

    

        /**
         * 最后一个特征的位置
         */
        private int last;

        public HashFeatureAlphabet() {
            intdata = new Dictionary<int, int>();
            frozen = false;
            last = 0;
        }

        public int lookupIndex(string str, int indent)
        {
            throw new NotImplementedException();
        }

        public int keysize()
        {
            throw new NotImplementedException();
        }

        public int nonZeroSize()
        {
            throw new NotImplementedException();
        }

        public bool hasIndex(int id)
        {
            throw new NotImplementedException();
        }

        public System.Collections.IEnumerator iterator()
        {
            throw new NotImplementedException();
        }

        public bool isStopIncrement()
        {
            throw new NotImplementedException();
        }

        public void setStopIncrement(bool b)
        {
            throw new NotImplementedException();
        }

        public int lookupIndex(string str)
        {
            throw new NotImplementedException();
        }

        public int size()
        {
            throw new NotImplementedException();
        }

        public void clear()
        {
            throw new NotImplementedException();
        }
    }
}
