using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class TempletGroup : List<Templet>
    {

        /**
         * 模板标识
         */
        public int gid;
        /**
         * n阶状态空间映射数组，元素为每一阶对应的一维空间起始地址
         * 以标记个数为进制
         */
        public int[] based;
        /**
         * 最大阶数
         */
        public int maxOrder;
        /**
         * 状态组合个数
         * numStates = numLabels^(maxOrder+1)
         */
        public int numStates;
        /**
         * 模板阶数
         */
        int[] orders;


        /**
         * 不同模板对应状态组合的相对偏移位置
         */
        public int[,] offset;

        public TempletGroup()
        {
            gid = 0;
        }

        /**
         * 从文件中读取
         * @param file
         * @throws Exception 
         */
        public void load(String file)
        {
            StreamReader sr = new StreamReader(file, Encoding.UTF8);
            string str = sr.ReadLine();

            while (str != null)
            {
                if (str.Length == 0)
                    continue;
                if (str[0] == '#')
                    continue;
                Add(new BaseTemplet(gid++, str));
                str = sr.ReadLine();
            }
            sr.Close();

            orders = new int[this.Count];
            orders[0] = 1;
        }

        public void loadPos(string file, string lastName)
        {
            StreamReader sr = new StreamReader(file, Encoding.UTF8);
            string str = sr.ReadLine();

            while (str != null)
            {
                if (str.Length == 0)
                    continue;
                if (str[0] == '#')
                    continue;
                BaseTemplet bt = new BaseTemplet(gid++, str);
                bt.minLen = 5;
                Add(bt);
                str = sr.ReadLine();
            }
            sr.Close();

            Add(new CharInStringTemplet(gid++, -1, 1));
            Add(new CharInStringTemplet(gid++, 0, 1));
            Add(new CharInStringTemplet(gid++, -2, 2));
            Add(new StringTypeTemplet(gid++));

            List<CharClassDictionary> dicts = new List<CharClassDictionary>();
            CharClassDictionary dic = new CharClassDictionary();
            dic.LoadData(lastName, "姓");
            dicts.Add(dic);
            Add(new CharClassTemplet(gid++, dicts.ToArray()));

            orders = new int[this.Count];
            orders[8] = 1;
        }


        /**
         * 计算偏移位置
         * @param numLabels 标记个数
         */
        public void calc(int numLabels)
        {
            //计算最大阶
            int numTemplets = this.Count;
            this.orders = new int[numTemplets];
            for (int j = 0; j < numTemplets; j++)
            {
                Templet t = this[j];
                this.orders[j] = t.getOrder();
                if (orders[j] > maxOrder)
                    maxOrder = orders[j];
            }

            based = new int[maxOrder + 2];
            based[0] = 1;
            for (int i = 1; i < based.Length; i++)
            {
                based[i] = based[i - 1] * numLabels;
            }
            this.numStates = based[maxOrder + 1];
            offset = new int[numTemplets, numStates];

            for (int t = 0; t < numTemplets; t++)
            {
                Templet tpl = this[t];
                int[] vars = tpl.getVars();
                /**
                 * 记录每一阶的状态
                 */
                int[] bits = new int[maxOrder + 1];
                int v;
                for (int s = 0; s < numStates; s++)
                {
                    int d = s;
                    //对于一个n阶状态组合，计算每个成员状态
                    for (int i = 0; i < maxOrder + 1; i++)
                    {
                        bits[i] = d % numLabels;
                        d = d / numLabels;
                    }
                    //对于一个n阶状态组合，记录一个特征模板映射到特征空间中到基地址的偏移
                    //TODO 是否可以和上面合并简化
                    v = 0;
                    for (int i = 0; i < vars.Length; i++)
                    {
                        v = v * numLabels + bits[-vars[i]];
                    }
                    offset[t, s] = v;
                }
            }
        }

        public int[] getOrders()
        {
            orders = new int[this.Count];
            for (int i = 0; i < orders.Length; i++)
            {
                orders[i] = this[i].getOrder();
            }
            return orders;
        }

        public int[] getOrders(int o)
        {
            int cnt = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].getOrder() == o)
                {
                    cnt++;
                }
            }
            int[] ret = new int[cnt];
            for (int i = 0, j = 0; i < this.Count; i++)
            {
                if (this[i].getOrder() == o)
                {
                    ret[j++] = i;
                }
            }
            return ret;
        }
    }
}
