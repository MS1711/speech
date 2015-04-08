using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class StringFeatureAlphabet : IFeatureAlphabet
    {
        /**
	     * 数据 FILLED
	     */
        protected Dictionary<string, int> data;
        /**
        * 是否冻结
        */
        protected bool frozen;
        /**
         * 最后一个特征的位置
         */
        private int last;

        public StringFeatureAlphabet()
        {
            data = new Dictionary<string, int>();
            frozen = false;
            last = 0;
        }

        public void LoadData(string file)
        {
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                string str = sr.ReadLine();
                while (str != null)
                {
                    int indx = str.LastIndexOf('-');
                    string k = str.Substring(0, indx);
                    int v = int.Parse(str.Substring(indx + 1));
                    data[k] = v;
                    str = sr.ReadLine();
                }
            }

            frozen = true;
        }

        public int lookupIndex(string str, int indent)
        {
            if (indent < 1)
                throw new Exception(
                        "Invalid Argument in FeatureAlphabet: " + indent);

            int ret = data.ContainsKey(str) ? data[str] : -1;

            if (ret == -1 && !frozen)
            {//字典中没有，并且允许插入
                lock (this)
                {
                    data[str] = last;
                    ret = last;
                    last += indent;
                }
            }
            //		if(ret==0)
            //			System.out.println(str);
            return ret;
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
            frozen = b;
        }

        public int lookupIndex(string str)
        {
            return lookupIndex(str, 1);
        }

        public int size()
        {
            return last;
        }

        public void clear()
        {
            throw new NotImplementedException();
        }
    }
}
