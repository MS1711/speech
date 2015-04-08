using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.pipes
{
    [Serializable]
    public class Target2Label : Pipe
    {

        private LabelAlphabet labelAlphabet;


        public Target2Label(LabelAlphabet labelAlphabet)
        {
            this.labelAlphabet = labelAlphabet;
            useTarget = true;
        }

        public override void addThruPipe(Instance instance)
        {
            // 处理类别
            //		instance.setTempData(instance.getTarget());

            Object t = instance.getTarget();
            if (t == null)
                return;

            if (t is string)
            {
                instance.setTarget(labelAlphabet.lookupIndex((String)t));
            }
            else if (t is object[])
            {
                Object[] l = (Object[])t;
                int[] newTarget = new int[l.Length];
                for (int i = 0; i < l.Length; ++i)
                    newTarget[i] = labelAlphabet.lookupIndex((String)l[i]);
                instance.setTarget(newTarget);
            }
            else if (t is List<string>)
            {
                List<string> l = (List<string>)t;
                int[] newTarget = new int[l.Count];
                for (int i = 0; i < l.Count; ++i)
                    newTarget[i] = labelAlphabet.lookupIndex(l[i]);
                instance.setTarget(newTarget);
            }
        }
    }
}
