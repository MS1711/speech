using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class CharInStringTemplet : Templet
    {
        private int id;
        /**
         * 字符串中子序列的起始位置
         */
        private int position;
        /**
         * 字符串中子序列的长度
         */
        private int plen;

        /**
         * 构造函数
         * @param id 模版
         * @param pos 子序列起始位置
         * @param len 子序列起始长度
         */
        public CharInStringTemplet(int id, int pos, int len)
        {
            this.id = id;
            this.position = pos;
            this.plen = len;
        }


        /**
         *  {@inheritDoc}
         */
        public int generateAt(Instance instance, IFeatureAlphabet features, int pos,
                params int[] numLabels)
        {
            string[][] data = (string[][])instance.getData();

            int len = data[0][pos].Length;

            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append(':');
            int indx = position;
            if (indx < 0)
                indx = len + indx;
            if (indx < 0)
                indx = 0;
            int endIdx = indx + plen;
            if (endIdx > len)
                endIdx = len;
            String str = data[0][pos].Substring(indx, endIdx - indx); //这里数据行列和模板中行列相反				
            sb.Append(str);
            //		System.out.println(sb.toString());
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
