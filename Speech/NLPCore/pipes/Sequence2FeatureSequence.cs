using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class Sequence2FeatureSequence : Pipe
    {
        TempletGroup templets;
        public IFeatureAlphabet features;
        LabelAlphabet labels;

        public Sequence2FeatureSequence(TempletGroup templets,
                IFeatureAlphabet features, LabelAlphabet labels)
        {
            this.templets = templets;
            this.features = features;
            this.labels = labels;
        }

        public override void addThruPipe(Instance instance)
        {
            object sdata = instance.getData();
            string[][] data;
            if (sdata is string[])
            {
                data = new string[1][];
                data[0] = (string[])sdata;
            }
            else if (sdata is string[][])
            {
                data = (string[][])sdata;
            }
            else if (sdata is List<List<string>>)
            {
                List<List<string>> ssdata = (List<List<string>>)sdata;
                data = new string[ssdata.Count][];
                for (int i = 0; i < ssdata.Count; i++)
                {
                    List<string> idata = (List<string>)ssdata[i];
                    data[i] = idata.ToArray();
                }
            }
            else
            {
                throw new Exception(sdata.ToString());
            }
            instance.setData(data);

            int len = data[0].Length;
            int[][] newData = new int[len][];
            for (int i = 0; i < len; i++)
            {
                newData[i] = new int[templets.Count];
            }
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < templets.Count; j++)
                {
                    newData[i][j] = templets[j].generateAt(instance,
                            this.features, i, labels.size());
                }
            }
            instance.setData(newData);
            instance.setSource(data);
        }
    }
}
