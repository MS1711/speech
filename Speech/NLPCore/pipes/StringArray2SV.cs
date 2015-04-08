using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.pipes
{
    [Serializable]
    public class StringArray2SV : Pipe
    {

        protected IFeatureAlphabet features;
        protected LabelAlphabet label;
        protected static string constant = "!#@$";
        /**
         * 常数项。为防止特征字典优化时改变，设为不可序列化
         */
        protected int constIndex;


        /**
         * 特征是否为有序特征
         */
        protected bool isSorted = false;

        public StringArray2SV()
        {
        }

        public StringArray2SV(AlphabetFactory af)
        {
            init(af);
        }
        public StringArray2SV(AlphabetFactory af, bool b)
        {
            init(af);
            isSorted = b;
        }


        protected void init(AlphabetFactory af)
        {
            this.features = af.DefaultFeatureAlphabet();
            this.label = af.DefaultLabelAlphabet();
            // 增加常数项
            constIndex = features.lookupIndex(constant);
        }



        public override void addThruPipe(Instance inst)
        {
            List<string> data = (List<string>)inst.getData();
            int size = data.Count;
            HashSparseVector sv = new HashSparseVector();

            for (int i = 0; i < size; i++)
            {
                string token = data[i];
                if (isSorted)
                {
                    token += "@" + i;
                }
                int id = features.lookupIndex(token);
                if (id == -1)
                    continue;
                sv.put(id, 1.0f);
            }
            sv.put(constIndex, 1.0f);
            inst.setData(sv);
        }

    }
}
