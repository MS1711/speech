using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLPCore
{
    public class Chars
    {

        /**
         * 字符类型
         * 汉字 字母 数字 标点 :空格;
         * @author xpqiu
         *
         */
        public enum CharType
        {
            /**
             * 汉字  
             */
            C,
            /**
             * 字母
             */
            L,
            /**
             * 数字
             */
            D,
            /**
             * 标点
             */
            P,
            /**
             * 空格
             */
            B
        }

        /**
         * 字符串类型
         */
        public enum StringType
        {
            /**
             * 纯数字
             */
            D,
            /**
             * 纯字母
             */
            L,
            /**
             * 纯汉字
             */
            C,
            /**
             * 混合字符串
             */
            M,
            /**
             * 空格
             */
            B,
            /**
             * 其他，例如标点等
             */
            O
        }

        /**
         * 半角或全角数字英文
         * 
         * @param str
         * @return 0,1
         * @see Chars#isChar(char)
         */

        public static StringType getStringType(string str)
        {
            HashSet<CharType> set = getTypeSet(str);
            if (set.Count == 1)
            {
                CharType c = set.ToList()[0];
                return char2StringType(c);
            }
            return StringType.M;
        }

        public static HashSet<CharType> getTypeSet(string str)
        {
            CharType[] tag = getType(str);
            HashSet<CharType> set = new HashSet<CharType>();
            for (int i = 0; i < tag.Length; i++)
            {
                set.Add(tag[i]);
            }
            return set;
        }

        public static bool isWhiteSpace(string w)
        {
            for (int i = 0; i < w.Length; i++)
            {
                char c = w[i];
                if (!Char.IsWhiteSpace(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static CharType[] getType(string str)
        {
            CharType[] tag = new CharType[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                tag[i] = getType(c);
            }
            return tag;
        }

        public static CharType getType(char c)
        {
            CharType tag;
            if (Char.IsLower(c) || Char.IsUpper(c))
            {
                tag = CharType.L;
            }
            else if (c == 12288 || c == 32)
            {//Character.isWhitespace(c) || Character.isSpaceChar(c)
                tag = CharType.B;
            }
            else if (Char.IsDigit(c))
            {
                tag = CharType.D;
                //		}else if ("一二三四五六七八九十零〇○".indexOf(c) != -1) {
                //			tag[i] = CharType.NUM;
            }
            else if (Char.IsPunctuation(c))
            {
                //			punP.matcher(string.valueOf(c)).matches()){//"/—-()。!,\"'（）！，””<>《》：:#@￥$%^…&*！、.%".
                tag = CharType.P;

            }
            else
            {

                tag = CharType.C;
            }

            return tag;
        }

        public static StringType char2StringType(CharType c)
        {
            switch (c)
            {
                case CharType.D: return StringType.D;
                case CharType.C: return StringType.C;
                case CharType.L: return StringType.L;
                case CharType.B: return StringType.B;
                default: return StringType.O;
            }
        }

    }
}
