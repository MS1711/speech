using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public interface TPredict<T>
    {
        /**
	     * 获得预测结果
	     * @param i	  位置
	     * @return 第i个预测结果；如果不存在，为NULL
	     */
        T getLabel(int i);
        /**
         * 获得预测结果的得分
         * @param i	     位置
         * @return 第i个预测结果的得分；不存在为Double.NEGATIVE_INFINITY
         */
        float getScore(int i);
        /**
         * 归一化得分
         */
        void normalize();
        /**
         * 预测结果数量 
         * @return 预测结果的数量
         */
        int size();
        /**
         * 得到所有标签
         * @return
         */
        T[] getLabels();
        /**
         * 删除位置i的信息
         * @param i
         */
        void remove(int i);
    }
}
