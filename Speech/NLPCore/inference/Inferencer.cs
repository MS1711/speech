using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public abstract class Inferencer
    {

        //TODO: FILLED
        protected float[] weights;

        protected bool _isUseTarget;

        /**
         * 得到前n个最可能的预测值
         * @param inst 
         * @return
         * Sep 9, 2009
         */
        public abstract Predict<int[]> getBest(Instance inst);

        public abstract Predict<int[]> getBest(Instance inst, int n);

        public float[] getWeights()
        {
            return weights;
        }

        public void setWeights(float[] weights)
        {
            this.weights = weights;
        }

        public void isUseTarget(bool b)
        {
            _isUseTarget = b;
        }
    }
}
