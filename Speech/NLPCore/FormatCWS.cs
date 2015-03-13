using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class FormatCWS
    {
        public static List<string> toList(Instance inst, string[] labels)
        {
            string[][] data = (string[][])inst.getSource();
            int len = data[0].Length;
            List<string> res = new List<string>(len);
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < len; j++)
            {
                String label = labels[j];
                String w = data[0][j];
                if (data[1][j].Equals("B"))
                {//空格特殊处理
                    if (sb.Length > 0)
                    {
                        res.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                    continue;
                }
                sb.Append(w);
                if (label.Equals("E") || label.Equals("S"))
                {
                    res.Add(sb.ToString());
                    sb = new StringBuilder();
                    //			}else if(j<len-1&&data[1][j].equals("C")&&(data[1][j+1].endsWith("L")||data[1][j+1].endsWith("D"))){
                    //				res.add(sb.toString());
                    //				sb = new StringBuilder();
                    //			}
                    //			else if(j<len-1&&data[1][j+1].equals("C")&&(data[1][j].endsWith("L"))){
                    //				res.add(sb.toString());
                    //				sb = new StringBuilder();
                }
            }
            if (sb.Length > 0)
            {
                res.Add(sb.ToString());
            }
            return res;
        }
        /**
         * 将BMES标签转为#delim#隔开的字符串
         * @param instSet 样本集
         * @param labelsSet  标签集
         * @param delim 字之间的间隔符
         * @return
         */
        public static string toString(InstanceSet instSet, String[][] labelsSet, String delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < instSet.Count; i++)
            {
                Instance inst = instSet.getInstance(i);
                String[] labels = labelsSet[i];
                sb.Append(toString(inst, labels, delim));
                sb.Append("\n");
            }
            return sb.ToString();
        }

        /**
         * 将BMES标签转为#delim#隔开的字符串
         * @param inst 样本
         * @param labels  标签
         * @param delim 字之间的间隔符
         * @return
         */
        public static String toString(Instance inst, String[] labels, String delim)
        {
            string[][] data = (string[][])inst.getSource();
            int len = data[0].Length;
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < len - 1; j++)
            {
                string label = labels[j];
                string w = data[0][j];
                sb.Append(w);
                if (data[1][j].Equals("B") || data[1][j + 1].Equals("B"))
                    continue;
                else if (label.Equals("E") || label.Equals("S"))
                {
                    sb.Append(delim);
                    //			}else if(data[1][j].equals("C")&&(data[1][j+1].endsWith("L")||data[1][j+1].endsWith("D"))){
                    //				sb.append(delim);
                    //			}else if(data[1][j+1].equals("C")&&(data[1][j].endsWith("L"))){//||data[1][j].endsWith("D")
                    //				sb.append(delim);
                }
            }
            sb.Append(data[0][len - 1]);
            return sb.ToString();
        }
    }
}
