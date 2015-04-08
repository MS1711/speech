using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.utils
{
    public class Vertex
    {
        public String id;
        public int index;
        private int forwardCount = 0;
        private List<Vertex> next = null;
        private List<int> wNext = null;

        public Vertex(String id)
        {
            this.id = id;
        }

        public void setId(String id)
        {
            this.id = id;
        }

        public String getId()
        {
            return id;
        }

        public void addVer(Vertex ver)
        {
            if (next == null)
            {
                next = new List<Vertex>();
                wNext = new List<int>();
            }
            next.Add(ver);
            wNext.Add(1);
        }

        public List<Vertex> getNext()
        {
            return next;
        }

        public List<int> getWNext()
        {
            return wNext;
        }

        public void setWNext(int index, int wAdd)
        {
            int w = wNext[index];
            wNext[index] = w + wAdd;
        }

        public void setIndex(int index)
        {
            this.index = index;
        }

        public void addForwardCount(int wAdd)
        {
            forwardCount += wAdd;
        }

        public int getForwardCount()
        {
            return forwardCount;
        }

        public string vertexToString()
        {
            string s = id + " " + index + " " + forwardCount;//+ " " + next.toString();
            return s;
        }
    }
}
