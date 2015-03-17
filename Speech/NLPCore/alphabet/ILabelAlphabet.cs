using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public interface ILabelAlphabet<T> : IAlphabet
    {
        /**
	     * 查找索引编号对应的标记
	     * @param id 索引编号
	     * @return 标记
	     */
        T lookupString(int id);

        /**
         * 查找一组编号对应的标记
         * @param ids 索引编号数组
         * @return 标记数组
         */
        T[] lookupString(int[] ids);
    }
}
