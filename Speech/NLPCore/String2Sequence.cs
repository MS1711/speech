using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class String2Sequence : Pipe
    {
        /**
	     * 是否预处理英文，默认为真
	     */
        bool isEnFilter = true;

        /**
         * 构造函数
         * @param b 是否带标签
         */
        public String2Sequence(bool b)
        {
            isEnFilter = b;
        }


        /**
         * 将一个字符串转换成按标注序列
         * 每列一个字或连续英文token的信息
         * @param inst 样本
         */
        public override void addThruPipe(Instance inst)
        {
            string str = (string)inst.getData();
            string[][] data;
            if (isEnFilter)
            {
                data = genSequence(str);
            }
            else
            {
                data = new string[2][];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new string[str.Length];
                }
                for (int i = 0; i < str.Length; i++)
                {
                    data[0][i] = str.Substring(i, 1);
                    data[1][i] = Chars.getStringType(data[0][i]).ToString();
                }
            }
            inst.setData(data);
        }

        /**
         * 预处理连续的英文和数字
         * @param sent
         * @return
         */
        public static string[][] genSequence(string sent)
        {

            NLPCore.Chars.CharType[] tags = Chars.getType(sent);
            int len = sent.Length;
            List<string> words = new List<string>();
            List<string> types = new List<string>();
            int begin = 0;
            for (int j = 0; j < len; j++)
            {
                if (j < len - 1 && tags[j] == NLPCore.Chars.CharType.L && tags[j + 1] == NLPCore.Chars.CharType.L)
                {//当前是连续英文
                    continue;
                }
                else if (j < len - 1 && tags[j] == NLPCore.Chars.CharType.D && tags[j + 1] == NLPCore.Chars.CharType.D)
                {//当前是连续数字
                    continue;
                }
                NLPCore.Chars.StringType st = Chars.char2StringType(tags[j]);
                string w = sent.Substring(begin, j + 1 - begin);
                words.Add(w);
                types.Add(st.ToString());
                begin = j + 1;
            }
            string[][] data = new string[2][];
            data[0] = words.ToArray();
            data[1] = types.ToArray();
            return data;
        }
    }
}
