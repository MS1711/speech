using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public interface ISparseVector
    {
        /**
	     * 点积
	     * @param vector
	     * @return
	     */
        float dotProduct(float[] vector);

        /**
         * 
         * @param sv
         * @return
         */
        float dotProduct(HashSparseVector sv);

        /**
         * 增加元素
         */
        void put(int i);
        /**
         * 增加多个元素
         */
        void put(int[] idx);
        /**
         * L2模
         */
        float l2Norm2();
    }
}
