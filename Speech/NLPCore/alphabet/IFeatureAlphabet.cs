using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public interface IFeatureAlphabet : IAlphabet
    {
        /**
	 * 查询字符串索引编号
	 * @param str 字符串
	 * @param indent 间隔
	 * @return 字符串索引编号，-1表示词典中不存在字符串
	 */
        int lookupIndex(String str, int indent);


        /**
         * 字典键的个数
         * @return
         */
        int keysize();

        /**
         * 实际存储的数据大小
         * @return
         */
        int nonZeroSize();

        /**
         * 索引对应的字符串是否存在在词典中
         * @param id 索引
         * @return 是否存在在词典中
         */
        bool hasIndex(int id);

        IEnumerator iterator();

        /**
         * 按索引建立HashMap并返回
         * @return 按“索引-特征字符串”建立的HashMap
         */
        //TIntHash toInverseIndexMap();
    }
}
