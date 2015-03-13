using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class SeriesPipes : Pipe
    {
        private List<Pipe> pipes = null;

        public int size()
        {
            return pipes.Count;
        }

        public SeriesPipes(Pipe[] pipes)
        {
            this.pipes = new List<Pipe>(pipes.Length);
            for (int i = 0; i < pipes.Length; i++)
            {
                if (pipes[i] != null)
                    this.pipes.Add(pipes[i]);
            }
        }

        public List<Pipe> getPipes()
        {
            return pipes;
        }

        public override void addThruPipe(Instance carrier)
        {
            for (int i = 0; i < pipes.Count; i++)
                pipes[i].addThruPipe(carrier);
        }


        public Pipe getPipe(int id)
        {
            if (id < 0 | id > pipes.Count)
                return null;
            return pipes[id];
        }

        /**
         * 删除使用类标签的Pipe
         */
        public void removeTargetPipe()
        {
            for (int i = pipes.Count - 1; i >= 0; i--)
            {
                Pipe p = pipes[i];
                if (p is SeriesPipes)
                {
                    ((SeriesPipes)p).removeTargetPipe();
                }
                else if (p.useTarget)
                {
                    pipes.RemoveAt(i);
                    i--;
                }
            }

        }

        public void addPipe(Pipe pipe)
        {
            if (pipes == null)
                pipes = new List<Pipe>();
            pipes.Add(pipe);
        }
    }
}
