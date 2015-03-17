using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public interface Templet
    {
        /**
	     * 返回该模板的阶
	     * @return 阶
	     */
        int getOrder();

        /**
         * 在给定实例的指定位置上抽取特征
         * @param instance 给定实例
         * @param pos 指定位置
         * @param numLabels 标签数量
         * @throws Exception 
         */
        int generateAt(Instance instance,
                                IFeatureAlphabet features,
                                int pos,
                                params int[] numLabels);

        int[] getVars();
    }
}
