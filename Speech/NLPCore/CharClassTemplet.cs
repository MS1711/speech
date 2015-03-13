using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class CharClassTemplet : Templet
    {
        private int id;
        private CharClassDictionary[] dicts;

        public CharClassTemplet(int id, CharClassDictionary[] dicts)
        {
            this.id = id;
            this.dicts = dicts;
        }

        /**
         *  {@inheritDoc}
         */
        public int generateAt(Instance instance, IFeatureAlphabet features, int pos,
                params int[] numLabels)
        {
            string[][] data = (string[][])instance.getData();
            //		int len = data[0].length;

            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append(':');
            char c = data[0][pos][0]; //这里数据行列和模板中行列相反
            // 得到字符串类型
            for (int i = 0; i < dicts.Length; i++)
            {
                if (dicts[i].contains(c))
                {
                    sb.Append("/");
                    sb.Append(dicts[i].name);
                }
            }
            if (sb.Length < 3)
                return -1;
            int index = features.lookupIndex(sb.ToString(), numLabels[0]);
            return index;
        }

        public int getOrder()
        {
            return 0;
        }

        public int[] getVars()
        {
            return new int[] { 0 };
        }

        public int offset(params int[] curs)
        {
            return 0;
        }
    }
}
