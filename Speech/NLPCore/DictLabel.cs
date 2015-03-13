using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class DictLabel : Pipe
    {
        public class WordInfo
        {
            public string word;
            public int len;

            public WordInfo(string str, int n)
            {
                word = str;
                len = n;
            }
        }

        protected NLPDictionary dict;
        protected LabelAlphabet labels;

        //BMES标签索引
        int idxB;
        int idxM;
        int idxE;
        int idxS;

        private bool mutiple;

        public DictLabel(NLPDictionary dict, LabelAlphabet labels)
        {
            this.dict = dict;
            this.mutiple = dict.IsAmbiguity();
            this.labels = labels;
            idxB = labels.lookupIndex("B");
            idxM = labels.lookupIndex("M");
            idxE = labels.lookupIndex("E");
            idxS = labels.lookupIndex("S");
        }

        public void setDict(NLPDictionary dict)
        {
            this.dict = dict;
        }

        public override void addThruPipe(Instance instance)
        {
            string[][] data = (string[][])instance.getData();

            int length = data[0].Length;
            int[][] dicData = new int[length][];
            for (int i = 0; i < length; i++)
            {
                dicData[i] = new int[labels.size()];
            }

            int indexLen = dict.getIndexLen();
            for (int i = 0; i < length; i++)
            {
                if (i + indexLen <= length)
                {
                    WordInfo s = getNextN(data[0], i, indexLen);
                    int[] index = dict.getIndex(s.word);
                    if (index != null)
                    {
                        for (int k = 0; k < index.Length; k++)
                        {
                            int n = index[k];
                            if (n == indexLen)
                            { //下面那个check函数的特殊情况，只为了加速
                                label(i, s.len, dicData);
                                if (!mutiple)
                                {
                                    i = i + s.len;
                                    break;
                                }
                            }
                            int len = check(i, n, length, data[0], dicData);
                            if (len > 0 && !mutiple)
                            {
                                i = i + len;
                                break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < length; i++)
                if (hasWay(dicData[i]))
                    for (int j = 0; j < dicData[i].Length; j++)
                        dicData[i][j]++;

            instance.setDicData(dicData);
        }

        private bool hasWay(int[] ia)
        {
            for (int i = 0; i < ia.Length; i++)
            {
                if (ia[i] == -1)
                    return true;
            }
            return false;
        }

        /**
         * 
         * @param i
         * @param n
         * @param length
         * @param data
         * @param tempData
         * @return
         */
        private int check(int i, int n, int length, string[] data, int[][] tempData)
        {

            WordInfo s = getNextN(data, i, n);
            if (dict.contains(s.word))
            {
                label(i, s.len, tempData);
                return s.len;
            }
            return 0;
        }
        /**
         * 
         * @param i
         * @param n
         * @param tempData
         */
        private void label(int i, int n, int[][] tempData)
        {
            // 下面这部分依赖{1=B,2=M,3=E,0=S}		
            if (n == 1)
            {
                tempData[i][idxS] = -1;
            }
            else
            {
                tempData[i][idxB] = -1;
                for (int j = i + 1; j < i + n - 1; j++)
                    tempData[j][idxM] = -1;
                tempData[i + n - 1][idxE] = -1;
            }
        }

        /**
         * 得到从位置index开始的长度为N的字串
         * @param data String[]
         * @param index 起始位置
         * @param N 长度
         * @return
         */
        public WordInfo getNextN(string[] data, int index, int N)
        {
            StringBuilder sb = new StringBuilder();
            int i = index;
            while (sb.Length < N && i < data.Length)
            {
                sb.Append(data[i]);
                i++;
            }
            if (sb.Length <= N)
                return new WordInfo(sb.ToString(), i - index);
            else
                return new WordInfo(sb.ToString().Substring(0, N), i - index);
        }
    }
}
