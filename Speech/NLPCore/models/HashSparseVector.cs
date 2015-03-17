using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class HashSparseVector : ISparseVector
    {
        private const float DefaultValue = 1.0f;

	    public Dictionary<int, float> data = new Dictionary<int, float>();

	    public HashSparseVector(float[] w){
		    for (int i = 0; i < w.Length; i++) {
			    if (Math.Abs((w[i]-0f))>float.MinValue) {
				    put(i, w[i]);
			    }
		    }
	    }
	    /**
	     * 
	     * @param w
	     * @param b 加入一个额外常数项
	     */
	    public HashSparseVector(float[] w, bool b) {
		    for (int i = 0; i < w.Length; i++) {
			    if (Math.Abs((w[i]-0f))>float.MinValue) {
				    put(i, w[i]);
			    }
		    }
		    if(b)
			    put(w.Length,1.0f);
	    }


	    public HashSparseVector() {

	    }

	    public HashSparseVector(HashSparseVector v) {
		    data = new Dictionary<int, float>(v.data);
	    }

	    public void minus(ISparseVector sv) {


	    }

        public void put(int id, float c)
        {
            data[id] = c;
        }

        public float dotProduct(float[] vector)
        {
            throw new NotImplementedException();
        }

        public float dotProduct(HashSparseVector sv)
        {
            throw new NotImplementedException();
        }

        public void put(int i)
        {
            throw new NotImplementedException();
        }

        public void put(int[] idx)
        {
            throw new NotImplementedException();
        }

        public float l2Norm2()
        {
            throw new NotImplementedException();
        }
    }
}
