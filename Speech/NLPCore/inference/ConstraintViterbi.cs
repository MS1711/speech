using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class ConstraintViterbi : LinearViterbi
    {
        private int newysize;

        public override int ysize()
        {
            return newysize;
        }

	    public ConstraintViterbi(TempletGroup templets, int ysize):base(templets, ysize){
		    this.newysize = ysize;
	    }

	    /**
	     * 构造函数
	     * @param viterbi 一阶线性解码器
	     */
	    public ConstraintViterbi(LinearViterbi viterbi):this(viterbi.getTemplets(), viterbi._ysize){
		    this.weights = viterbi.getWeights();
		
	    }

	    /**
	     * 构造函数
	     * @param viterbi 一阶线性解码器
	     */
	    public ConstraintViterbi(LinearViterbi viterbi,int ysize):this(viterbi.getTemplets(), viterbi._ysize) {
		    ;
		    this.weights = viterbi.getWeights();
		    this.newysize = ysize;
	    }
	
	    /**
	     * 构造约束网格，不经过的节点设置为NULL
	     */
	    protected override Node[][] initialLattice(Instance carrier) {
		    int[][] data = (int[][]) carrier.getData();

		    int[][] dicData = (int[][]) carrier.getDicData();

		    int length = carrier.length();

		    Node[][] lattice = new Node[length][];

		    for (int l = 0; l < length; l++) {
			    lattice[l] = new Node[newysize];
			    for (int c = 0; c < _ysize; c++) {
				    if (dicData[l][c] == 0) {
					    lattice[l][c] = new Node(newysize);
					    for (int i = 0; i < orders.Length; i++) {
						    if (data[l][i] == -1)
							    continue;
						    if (orders[i] == 0) {
							    lattice[l][c].score += weights[data[l][i] + c];
						    } else if (l > 0 && orders[i] == 1) {
							    for (int p = 0; p < _ysize; p++) {
								    int offset = p * _ysize + c;
								    lattice[l][c].trans[p] += weights[data[l][i]
										    + offset];
							    }
						    }
					    }
				    }
			    }
			    for (int c = _ysize; c < newysize; c++) {
				    if (dicData[l][c] == 0) {
					    lattice[l][c] = new Node(newysize);
				    }
			    }
		    }

		    return lattice;
	    }
    }
}
