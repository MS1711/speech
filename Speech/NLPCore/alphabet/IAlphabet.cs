using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public interface IAlphabet
    {

	    /**
	     * 判断词典是否冻结
	     * @return true - 词典冻结；false - 词典未冻结
	     */
	    bool isStopIncrement();

	    /**
	     * 不再增加新的词
	     * @param b
	     */
	    void setStopIncrement(bool b);
	
	    /**
	     * 查找字符串索引编号
	     * @param str 字符串
	     * @return 索引编号
	     */
	    int lookupIndex(string str);
	
	    /**
	     * 词典大小
	     * @return 词典大小
	     */
	    int size();

	    /**
	     * 恢复成新字典
	     */
	    void clear();
    }
}
