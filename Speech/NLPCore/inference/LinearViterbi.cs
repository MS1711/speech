using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class Node {

		public float based = 0;
        public float score = 0;
        public int prev = -1;
        public float[] trans = null;

		public Node(int n) {
			based = 0;
			score = 0;
			prev = -1;
			trans = new float[n];
		}

		public void addScore(float score, int path) {
			this.score = score;
			this.prev = path;
		}

		public void clear() {
			based = 0;
			score = 0;
			prev = -1;
            for (int i = 0; i < trans.Length; i++)
            {
                trans[i] = 0;
            }
		}

	}

    public class LinearViterbi : AbstractViterbi
    {
        public LinearViterbi(TempletGroup templets, int ysize)
        {
            this._ysize = ysize;
            this.setTemplets(templets);
            this.orders = templets.getOrders();
        }

        public LinearViterbi(AbstractViterbi viterbi)
            : this(viterbi.getTemplets(), viterbi._ysize)
        {
            this.weights = viterbi.getWeights();
        }

        public override Predict<int[]> getBest(Instance carrier)
        {

            Node[][] node = initialLattice(carrier);

            doForwardViterbi(node, carrier);

            Predict<int[]> res = getPath(node);

            return res;
        }

        protected Predict<int[]> getPath(Node[][] lattice)
        {

            Predict<int[]> res = new Predict<int[]>();
            if (lattice.Length == 0)
                return res;

            float max = float.NegativeInfinity;
            int cur = 0;
            for (int c = 0; c < ysize(); c++)
            {
                if (lattice[lattice.Length - 1][c] == null)
                    continue;

                if (lattice[lattice.Length - 1][c].score > max)
                {
                    max = lattice[lattice.Length - 1][c].score;
                    cur = c;
                }
            }

            int[] path = new int[lattice.Length];
            path[lattice.Length - 1] = cur;
            for (int l = lattice.Length - 1; l > 0; l--)
            {
                cur = lattice[l][cur].prev;
                path[l - 1] = cur;
            }
            res.add(path, max);

            return res;
        }

        protected void doForwardViterbi(Node[][] lattice, Instance carrier)
        {
            for (int l = 1; l < lattice.Length; l++)
            {
                for (int c = 0; c < lattice[l].Length; c++)
                {
                    if (lattice[l][c] == null)
                        continue;

                    float bestScore = float.NegativeInfinity;
                    int bestPath = -1;
                    for (int p = 0; p < lattice[l - 1].Length; p++)
                    {
                        if (lattice[l - 1][p] == null)
                            continue;

                        float score = lattice[l - 1][p].score
                                + lattice[l][c].trans[p];
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestPath = p;
                        }
                    }
                    bestScore += lattice[l][c].score;
                    lattice[l][c].addScore(bestScore, bestPath);
                }
            }
        }

        virtual protected Node[][] initialLattice(Instance carrier)
        {
            int[][] data = (int[][])carrier.getData();

            int length = carrier.length();

            Node[][] lattice = new Node[length][];
            for (int l = 0; l < length; l++)
            {
                lattice[l] = new Node[_ysize];
                for (int c = 0; c < _ysize; c++)
                {
                    lattice[l][c] = new Node(_ysize);
                    for (int i = 0; i < orders.Length; i++)
                    {
                        if (data[l][i] == -1 || data[l][i] >= weights.Length) //TODO: xpqiu 2013.2.1
                            continue;
                        if (orders[i] == 0)
                        {
                            lattice[l][c].score += weights[data[l][i] + c];
                        }
                        else if (orders[i] == 1)
                        {
                            int offset = c;
                            for (int p = 0; p < _ysize; p++)
                            {
                                //weights对应trans(c,p)的按行展开
                                lattice[l][c].trans[p] += weights[data[l][i]
                                        + offset];
                                offset += _ysize;
                            }
                        }
                    }
                }
            }

            return lattice;
        }
    }
}
