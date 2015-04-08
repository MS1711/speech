using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.utils
{
    public class StopWords
    {
        HashSet<string> sWord = new HashSet<string>();
	    string dicPath;
	    Dictionary<string, long> lastModTime = new Dictionary<string, long>();

	    public StopWords(){	
	    }

	    /**
	     * 构造函数
	     * @param dicPath
	     *        stopword所在地址
	     */

	    public StopWords(string dicPath) {		
		    this.dicPath = dicPath;
		    read(dicPath);		
	    }

	    /**
	     * 读取stopword
	     * @param dicPath
	     *       stopword所在地址
	     */

	    public void read(string dicPath) {

		    string[] files = Directory.GetFiles(dicPath);
            if (files != null)
            {
                foreach (var item in files)
	            {
		            if (item.EndsWith(".txt"))
                    {
                        using (StreamReader sr = new StreamReader(item, Encoding.UTF8))
                        {
                            string line = sr.ReadLine();
                            while (line != null)
                            {
                                line = line.Trim();
                                if (!string.IsNullOrEmpty(line))
                                {
                                    sWord.Add(line.Trim());
                                }
                                line = sr.ReadLine();
                            }
                        }
                    }
	            }
            }
	    }

	    /**
	     * 删除stopword
	     * 将string字符串转换为List类型，并返回
	     * @param words
	     *       要进行处理的字符串 
	     * @return
	     *       删除stopword后的List类型
	     */

	    public List<string> phraseDel(string[] words){
            List<string> list = new List<string>(); 
		    string s;
		    int length= words.Length;
		    for(int i = 0; i < length; i++){
			    s = words[i];
			    if(!isStopWord(s))
				    list.Add(s);
		    }
		    return list;
	    }

	    /**
	     * 判断是否为停用词
	     * @param word
	     * @param minLen 最小长度
	     * @param maxLen 最大长度
	     * @return
	     */
	    public bool isStopWord(string word,int minLen, int maxLen) {
            if (word.Length < minLen || word.Length > maxLen)
            {
                return true;
            }

		    if (sWord.Contains(word))
            {
                return true;
            }

		    return false;
	    }
	
	    /**
	     * 判断是否为停用词
	     * @param word
	     * @return
	     */
	    public bool isStopWord(string word) {

            if (sWord.Contains(word.ToLower()))
            {
                return true;
            }

		    return false;
	    }
    }
}
