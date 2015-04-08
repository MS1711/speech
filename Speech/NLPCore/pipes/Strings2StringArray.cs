using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.pipes
{
    [Serializable]
    public class Strings2StringArray : Pipe
    {
        public Strings2StringArray()
        {
        }

        public override void addThruPipe(Instance inst)
        {
            String[] data = (String[])inst.getData();
            List<string> newdata = new List<string>();
            for (int i = 0; i < data.Length; i++)
                newdata.Add(data[i]);
            inst.setData(newdata);
        }
    }
}
