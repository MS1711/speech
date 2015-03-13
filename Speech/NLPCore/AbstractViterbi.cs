using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public abstract class AbstractViterbi : Inferencer
    {
	
	    //TODO: FILLED
	    protected int[] orders;
	
	    /**
	     * 标记个数
	     */
	    public int _ysize{get;set;}

	    /**
	     * 模板个数
	     */
        protected int numTemplets;
	
	    /**
	     * 模板组
	     */
	    protected TempletGroup templets;

	    /**
	     * 状态组合个数
	     */
	    protected int numStates;
	
	    /**
	     * 抽象最优解码算法实现
	     * @param inst 样本实例
	     */
	    /**
	     * Viterbi解码算法不支持K-Best解码
	     */
        public override Predict<int[]> getBest(Instance inst, int nbest)
        {
		    return getBest(inst);
	    }

	    public TempletGroup getTemplets() {
		    return templets;
	    }
	    public void setTemplets(TempletGroup templets) {
		    this.templets = templets;
	    }

        public void setOrders(int[] orders)
        {
            this.orders = orders;
        }

        virtual public int ysize()
        {
            return _ysize;
        }
    }
}
