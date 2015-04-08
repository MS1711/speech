using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.pipes
{
    [Serializable]
    public class CNPipe : Pipe
    {
        [NonSerialized]
        private CWSTagger seg;

        public CWSTagger getSeg()
        {
            return seg;
        }

        public void setSeg(CWSTagger seg)
        {
            this.seg = seg;
        }

        public CNPipe()
        {
        }

        public CNPipe(CWSTagger seg)
        {
            this.seg = seg;
        }

        public override void addThruPipe(Instance inst)
        {
            String data = (String)inst.getData();
            String[] newdata = seg.tag2Array(data);
            inst.setData(newdata);
        }
    }
}
