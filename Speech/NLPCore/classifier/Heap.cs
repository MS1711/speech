using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.classifier
{
    public class Heap
    {
        private bool isMinRootHeap;
        private List<int> datas;
        private double[] scores;
        private int maxsize;
        private int size;

        public Heap(int max, bool isMinRootHeap)
        {
            this.isMinRootHeap = isMinRootHeap;
            maxsize = max;
            scores = new double[maxsize + 1];
            datas = new List<int>();
            size = 0;
            datas.Add(0);
            scores[0] = 0;

        }
        public Heap(int max)
            : this(max, true)
        {

        }


        private int leftchild(int pos)
        {
            return 2 * pos;
        }

        private int rightchild(int pos)
        {
            return 2 * pos + 1;
        }

        private int parent(int pos)
        {
            return pos / 2;
        }

        private bool isleaf(int pos)
        {
            return ((pos > size / 2) && (pos <= size));
        }

        private bool needSwapWithParent(int pos)
        {
            return isMinRootHeap ?
                    scores[pos] < scores[parent(pos)] :
                    scores[pos] > scores[parent(pos)];
        }

        private void swap(int pos1, int pos2)
        {
            double tmp;
            tmp = scores[pos1];
            scores[pos1] = scores[pos2];
            scores[pos2] = tmp;
            int t1, t2;
            t1 = datas[pos1];
            t2 = datas[pos2];
            datas[pos1] = t2;
            datas[pos2] = t1;
        }


        public void insert(double score, int data)
        {
            if (size < maxsize)
            {
                size++;
                scores[size] = score;
                datas.Add(data);
                int current = size;
                while (current != 1 && needSwapWithParent(current))
                {
                    swap(current, parent(current));
                    current = parent(current);
                }
            }
            else
            {
                if (isMinRootHeap ?
                        score > scores[1] :
                        score < scores[1])
                {
                    scores[1] = score;
                    datas[1] = data;
                    pushdown(1);
                }
            }
        }

        private int findcheckchild(int pos)
        {
            int rlt;
            rlt = leftchild(pos);
            if (rlt == size)
                return rlt;
            if (isMinRootHeap ? (scores[rlt] > scores[rlt + 1]) : (scores[rlt] < scores[rlt + 1]))
                rlt = rlt + 1;
            return rlt;
        }

        private void pushdown(int pos)
        {
            int checkchild;
            while (!isleaf(pos))
            {
                checkchild = findcheckchild(pos);
                if (needSwapWithParent(checkchild))
                    swap(pos, checkchild);
                else
                    return;
                pos = checkchild;
            }
        }

        public List<int> getData()
        {
            return datas;
        }
    }
}
