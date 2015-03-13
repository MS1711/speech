using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLPCore
{
    public class NLPDictionary
    {
        private int MAX_LEN = 7;
        private int MIN_LEN = 2;
        /**
         * 词典，词和相应的词性
         */
        private MultiValueMap dp;

        private Dictionary<string, int[]> index = new Dictionary<string, int[]>();
        private int indexLen = 2;
        private bool isAmbiguity = false;


        public NLPDictionary()
        {
            MAX_LEN = int.MinValue;
            MIN_LEN = int.MaxValue;
            dp = new MultiValueMap();
        }

        /**
         * 
         * @param b 是否模糊处理
         */
        public NLPDictionary(bool b)
            : this()
        {
            this.setAmbiguity(b);
        }
        /**
         * 
         * @param path
         * @throws IOException
         */
        public NLPDictionary(string path)
            : this(path, false)
        {

        }

        /**
         * 
         * @param path
         * @param b 使用模糊处理
         * @throws IOException
         */
        public NLPDictionary(String path, bool b)
            : this()
        {
            this.setAmbiguity(b);
            List<string[]> al = loadDict(path);
            add(al);
            createIndex();
        }


        /**
         * 加入不带词性的词典
         * @param al 词的数组
         */
        public void addSegDict(ICollection<string> al)
        {
            foreach (string s in al)
            {
                addDict(s);
            }
            createIndex();
        }

        /**
         * 
         * @param word 词
         * @param poses 词性数组
         */
        public void add(string word, params string[] poses)
        {
            addDict(word, poses);
            indexLen = MIN_LEN;
            createIndex();
        }

        /**
         * 
         * @param al 词典 ArrayList&lt;String[]&gt;
         * 						每一个元素为一个单元String[].
         * 						String[] 第一个元素为单词，后面为对应的词性
         */
        public void add(List<string[]> al)
        {
            foreach (string[] pos in al)
            {
                string[] s = new string[pos.Length - 1];
                for (int i = 1; i < pos.Length; i++)
                {
                    s[i - 1] = pos[i];
                }
                addDict(pos[0], s);
            }
            indexLen = MIN_LEN;
            createIndex();
        }
        /**
         * 在目前词典中增加新的词典信息
         * @param path
         * @throws LoadModelException 
         */
        public void addFile(string path)
        {
            List<string[]> al = loadDict(path);
            add(al);
            indexLen = MIN_LEN;
            createIndex();
        }


        /**
         * 通过词典文件建立词典
         * @param path
         * @return 
         * @throws FileNotFoundException
         */
        private List<string[]> loadDict(String path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line = sr.ReadLine();
            List<String[]> al = new List<String[]>();
            while (line != null)
            {
                string[] s = Regex.Split(line, "\\s", RegexOptions.IgnoreCase);
                al.Add(s);
                line = sr.ReadLine();
            }
            sr.Close();

            return al;
        }
        /**
         * 增加词典信息
         * @param word
         * @param poses
         */
        private void addDict(string word, params string[] poses)
        {
            if (word.Length > MAX_LEN)
                MAX_LEN = word.Length;
            if (word.Length < MIN_LEN)
                MIN_LEN = word.Length;
            if (poses == null || poses.Length == 0)
            {
                if (!dp.containsKey(word))
                    dp.put(word, null);
                return;
            }

            for (int j = 0; j < poses.Length; j++)
            {
                dp.put(word, poses[j]);

            }
        }

        /**
         * 建立词的索引
         */
        private void createIndex()
        {
            indexLen = MIN_LEN;
            Dictionary<string, HashSet<int>> indexT = new Dictionary<string, HashSet<int>>();
            foreach (string s in dp.keySet())
            {
                if (s.Length < indexLen)
                    continue;
                string temp = s.Substring(0, indexLen);
                //System.out.println(temp);
                if (indexT.ContainsKey(temp) == false)
                {
                    HashSet<int> set = new HashSet<int>();
                    set.Add(s.Length);
                    indexT[temp] = set;
                }
                else
                {
                    indexT[temp].Add(s.Length);
                }
            }
            foreach (string key in indexT.Keys)
            {
                HashSet<int> set = indexT[key];
                int[] ia = new int[set.Count];
                int i = set.Count;
                //			System.out.println(key);
                foreach (int integer in set)
                {
                    ia[--i] = integer;

                }
                //			for(int j = 0; j < ia.length; j++) 
                //				System.out.println(ia[j]);

                index[key] = ia;
            }
            //		System.out.println(indexT);
        }

        public int getMaxLen()
        {
            return MAX_LEN;
        }

        public int getMinLen()
        {
            return MIN_LEN;
        }

        public bool contains(String s)
        {
            return dp.containsKey(s);
        }

        public int[] getIndex(string s)
        {
            if (!index.ContainsKey(s))
            {
                return null;
            }
            return index[s];
        }

        public HashSet<string> getPOS(String s)
        {
            return dp.getSet(s);
        }

        public int getDictSize()
        {
            return dp.size();
        }

        public int getIndexLen()
        {
            return indexLen;
        }

        public bool IsAmbiguity()
        {
            return isAmbiguity;
        }

        private void setAmbiguity(bool isAmbiguity)
        {
            this.isAmbiguity = isAmbiguity;
        }

        public ICollection<string> getDict()
        {
            return dp.keySet();
        }
        public MultiValueMap getPOSDict()
        {
            return dp;
        }

        public Dictionary<string, int[]> getIndex()
        {
            return index;
        }
        public int size()
        {
            return dp.size();
        }

        public void save(String path)
        {
            //MyCollection.writeMultiValueMap(dp, path);

        }
    }
}
