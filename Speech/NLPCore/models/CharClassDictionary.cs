using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class CharClassDictionary
    {
        public String name;
        private HashSet<char> dict;

        public CharClassDictionary()
        {
            dict = new HashSet<char>();
        }

        public void LoadData(string file, string name)
        {
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                string str = sr.ReadLine();
                while (str != null)
                {
                    dict.Add(char.Parse(str));
                    str = sr.ReadLine();
                }
            }

            this.name = name;
        }

        /**
         * 返回词典标签
         */
        public bool contains(char c)
        {
            return dict.Contains(c);
        }
    }
}
