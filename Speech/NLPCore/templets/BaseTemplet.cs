using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class BaseTemplet : Templet
    {
        Regex parser = new Regex("(?:%(x|y)\\[(-?\\d+)(?:,(\\d+))?\\])");
        string templet;
        /**
         * 模板阶数
         */
        int order;
        int id;
        /**
         * x相对位置
         */
        int[][] dims;
        /**
         * y相对位置
         */
        int[] vars;
        /**
         * 对于某些情况不返回特定信息
         */
        public int minLen = 0;

        /**
         * 构造函数
         * 
         * @param templet
         *            模板字符串 格式如：%x[0,0]%y[0]; %y[-1]%y[0]; %x[0,0]%y[-1]%y[0]
         */
        public BaseTemplet(int id, string templet)
        {
            this.id = id;
            this.templet = templet;
            MatchCollection matchers = parser.Matches(this.templet);
            /**
             * 解析y的位置
             */
            List<string> l = new List<string>();
            List<string> x = new List<string>();
            for (int i = 0; i < matchers.Count; i++)
            {
                if (matchers[i].Groups[1].Value.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    l.Add(matchers[i].Groups[2].Value);
                }
                else if (matchers[i].Groups[1].Value.Equals("x", StringComparison.OrdinalIgnoreCase))
                {
                    x.Add(matchers[i].Groups[2].Value);
                    x.Add(matchers[i].Groups[3].Value);
                }
            }

            if (l.Count == 0)
            {//兼容CRF++模板
                vars = new int[] { 0 };
            }
            else
            {
                vars = new int[l.Count];
                for (int j = 0; j < l.Count; j++)
                {
                    vars[j] = int.Parse(l[j]);
                }
            }
            order = vars.Length - 1;
            l = null;

            dims = new int[x.Count / 2][];
            for (int i = 0; i < dims.Length; i++)
            {
                dims[i] = new int[2];
            }
            for (int i = 0; i < x.Count; i += 2)
            {
                dims[i / 2][0] = int.Parse(x[i]);
                dims[i / 2][1] = int.Parse(x[i + 1]);
            }
            x = null;
        }

        /**
         * 得到y相对位置
         */
        public int[] getVars()
        {
            return this.vars;
        }

        /**
         * 得到模板阶数
         */
        public int getOrder()
        {
            return this.order;
        }

        /**
         * {@inheritDoc}
         */
        public int generateAt(Instance instance, IFeatureAlphabet features, int pos,
                params int[] numLabels)
        {
            string[][] data = (string[][])instance.getData();
            int len = data[0].Length;

            if (order > 0 && len == 1) //对于长度为1的序列，不考虑1阶以上特征
                return -1;

            //		if(isForceTrain){
            //			if(len<minLen && order>0 )//训练时，对于长度过小的句子，不考虑开始、结束特征
            //				return -1;
            //		}
            StringBuilder sb = new StringBuilder();
            sb.Append(id);
            sb.Append(':');
            for (int i = 0; i < dims.Length; i++)
            {
                int j = dims[i][0]; //行号
                int k = dims[i][1]; //列号
                if (k > data.Length - 1)
                    return -1;
                int ii = pos + j;
                int iii = ii;
                if (ii < 0)
                {
                    if (len < minLen)//对于长度过小的句子，不考虑开始、结束特征
                        return -1;
                    while (iii++ < 0)
                    {
                        sb.Append("B");
                    }
                    sb.Append("_");
                }
                else if (ii >= len)
                {
                    if (len < minLen)//对于长度过小的句子，不考虑开始、结束特征
                        return -1;
                    while (iii-- >= len)
                    {
                        sb.Append("E");
                    }
                    sb.Append("_");
                }
                else
                {
                    sb.Append(data[k][pos + j]); //这里数据行列和模板中行列相反
                }
                sb.Append("/");
            }
            int index = features.lookupIndex(sb.ToString(), (int)Math.Pow(numLabels[0], order + 1));
            return index;
        }

        public string toString()
        {
            return this.templet;
        }

        public int offset(params int[] curs)
        {
            return 0;
        }
    }
}
