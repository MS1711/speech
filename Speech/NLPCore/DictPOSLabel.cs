using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class DictPOSLabel : Pipe
    {

        protected NLPDictionary dict;
        protected LabelAlphabet labels;

        public DictPOSLabel(NLPDictionary dict, LabelAlphabet labels)
        {
            this.dict = dict;
            this.labels = labels;
            checkLabels();
        }

        private void checkLabels()
        {
            MultiValueMap pos = dict.getPOSDict();
            foreach (HashSet<string> pp in pos.valueSets())
            {
                if (pp == null)
                    continue;
                foreach (string p in pp)
                {
                    if (labels.lookupIndex(p) == -1)
                    {
                        Console.WriteLine("Warning: 自定义词性: " + p +
                            "\n标签最好在下面列表中：\n" + labels.ToString());
                        labels.setStopIncrement(false);
                        labels.lookupIndex(p);
                        labels.setStopIncrement(true);
                    }
                }
            }
        }

        public override void addThruPipe(Instance instance)
        {
            string[] data = (string[])instance.getData();

            int length = data.Length;
            int[][] dicData = new int[length][];
            for (int i = 0; i < length; i++)
            {
                dicData[i] = new int[labels.size()];
            }

            for (int i = 0; i < data.Length; i++)
            {
                //			System.out.println(data[i]);
                HashSet<string> posset = dict.getPOS(data[i]);
                if (posset != null && posset.Count > 0)
                {
                    foreach (string pos in posset)
                        dicData[i][labels.lookupIndex(pos)] = -1;
                }
            }

            for (int i = 0; i < length; i++)
                if (hasWay(dicData[i]))
                    for (int j = 0; j < dicData[i].Length; j++)
                        dicData[i][j]++;

            //		for(int i = 0; i < dicData.length; i++) {
            //			for(int j = 0; j < dicData[i].length; j++)
            //				System.out.print(dicData[i][j]);
            //			System.out.println();
            //		}

            instance.setDicData(dicData);
        }

        private bool hasWay(int[] ia)
        {
            for (int i = 0; i < ia.Length; i++)
            {
                if (ia[i] == -1)
                    return true;
            }
            return false;
        }
    }
}
