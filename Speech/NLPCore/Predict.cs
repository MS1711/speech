using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class Predict<T> : TPredict<T>
    {
        /**
	     * 标签数组
	     */
        public T[] labels;
        /**
         * 得分数组
         */
        public float[] scores;
        /**
         * 保存个数
         */
        protected int n;
        /**
         * 缺省只保存一个最大值
         */
        public Predict()
            : this(1)
        {
        }
        /**
         * 保存前n个最大值
         * @param n
         */
        public Predict(int n)
        {
            this.n = n;
            labels = new T[n];
            scores = new float[n];
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i] = float.NegativeInfinity;
            }
        }

        public float getShiftScore()
        {
            for (int i = 0; i < n; i++)
            {
                string s = labels[i].ToString();
                if (s.Equals("S"))
                {
                    return scores[i];
                }
            }
            return 0;
        }


        public int size()
        {
            return labels.Length;
        }



        /**
         * 返回得分最高的标签
         * @return
         */
        public T getLabel()
        {
            return labels[0];
        }
        /**
         * 返回得分第i高的标签
         */
        public T getLabel(int i)
        {
            return labels[i];
        }

        public float getScore(int i)
        {
            return scores[i];
        }


        /**
         * 设置位置i上的标签和得分
         * @param i
         * @param label2
         * @param d
         */
        public void set(int i, T label2, float d)
        {
            labels[i] = label2;
            scores[i] = d;
        }

        /**
         * 增加新的标签和得分，并根据得分调整排序	 * 
         * @param label 标签
         * @param score 得分
         * @return 插入位置
         */
        public int add(T label, float score)
        {
            int i = 0;
            int ret = i;
            if (n != 0)
            {
                for (i = 0; i < n; i++)
                {
                    if (score > scores[i])
                        break;
                }
                if (i != n || n < scores.Length)
                {
                    for (int k = n - 2; k >= i; k--)
                    {
                        scores[k + 1] = scores[k];
                        labels[k + 1] = labels[k];
                    }
                    ret = i;
                }
                else if (n < scores.Length)
                {

                }
                else
                {
                    ret = -1;
                }
            }
            if (ret != -1)
            {
                scores[i] = score;
                labels[i] = label;
            }
            if (n < scores.Length)
                n++;
            return ret;
        }

        /**
         * 将得分归一化到[0,1]区间
         */
        public void normalize()
        {
            float based = 1;
            //		if(scores[0]>=0.0f)
            //			base = scores[0]/2;
            float sum = 0;

            for (int i = 0; i < scores.Length; i++)
            {
                float s = (float)Math.Exp(scores[i] / based);
                scores[i] = s;
                sum += s;
            }
            for (int i = 0; i < scores.Length; i++)
            {
                float s = scores[i] / sum;
                scores[i] = s;

            }

        }
        /**
         * 简单可视输出
         */
        public String ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < labels.Length; i++)
            {
                sb.Append(labels[i]);
                sb.Append(" ");
                sb.Append(scores[i]);
                sb.Append("\n");
            }
            return sb.ToString();

        }
        /**
         * 取得所有返回结果
         * @return
         */
        public T[] getLabels()
        {
            return labels;
        }
        public void remove(int i)
        {
            // TODO Auto-generated method stub
        }
    }
}
