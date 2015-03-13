using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class Instance
    {
        /**
         * 样本值，相当于x
         */
        protected object data;
        /**
         * 标签或类别，相当于y
         */
        protected object target;
        /**
         * 数据来源等需要记录的信息
         */
        protected object clause;
        /**
         * 保存数据的最原始版本
         */
        private object source;
        /**
         * 临时数据，用来传递一些临时变量
         */
        private object tempData;
        /**
         * 临时数据被占有标志
         */
        private bool tempDataLock = false;
        /**
         * 字典数据
         */
        private object dicData;

        /**
         * 样本权重
         */
        private float weight = 1;

        public Instance()
        {
        }

        public Instance(object data)
        {
            this.data = data;
        }

        public Instance(object data, object target)
        {
            this.data = data;
            this.source = data;
            this.target = target;
        }

        public Instance(object data, object target, object clause)
        {
            this.data = data;
            this.source = data;
            this.target = target;
            this.clause = clause;
        }

        public object getTarget()
        {
            // 注释掉下面2行，可能会引起别的问题
            // if (target == null)
            // return data;

            return this.target;
        }

        public void setTarget(object target)
        {
            this.target = target;
        }

        public object getData()
        {
            return data;
        }

        public void setData(object data)
        {
            this.data = data;
        }

        public void setClasue(String s)
        {
            this.clause = s;

        }

        public String getClasue()
        {
            return (String)this.clause;

        }

        public object getSource()
        {
            return this.source;
        }

        public void setSource(object source)
        {
            this.source = source;
        }
        /**
         * 设置临时数据
         * @param tempData
         */
        public void setTempData(object tempData)
        {
            if (tempDataLock)
            {

            }
            this.tempData = tempData;
            tempDataLock = true;
        }
        /**
         * 得到临时数据
         * @return
         */
        public object getTempData()
        {
            if (!tempDataLock)
            {
                return null;
            }
            return tempData;
        }
        /**
         * 删除临时数据
         */
        public void deleteTempData()
        {
            if (!tempDataLock)
            {
                return;
            }
            tempData = null;
            tempDataLock = false;
        }

        /**
         * 得到数据长度
         * @return
         */
        public int length()
        {
            int ret = 0;
            if (data is int[])
                ret = 1;
            else if (data is int[][])
                ret = ((int[][])data).Length;
            else if (data is int[][][])
            {
                ret = ((int[][][])data)[0].Length;
            }
            return ret;
        }

        public object getDicData()
        {
            return dicData;
        }

        public void setDicData(object dicData)
        {
            this.dicData = dicData;
        }

        /**
         * 得到样本权重
         * @return
         */
        public float getWeight()
        {
            return weight;
        }

        /**
         * 设置权重
         * @param weight
         */
        public void setWeight(float weight)
        {
            this.weight = weight;
            if (weight == 0f)
            {
            }
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(data.ToString());

            return sb.ToString();

        }
    }
}
