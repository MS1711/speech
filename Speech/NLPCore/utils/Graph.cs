using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.utils
{
    public class Graph
    {
        private List<Vertex> vertexList = new List<Vertex>();
        private int nVerts = 0;

        public List<Vertex> getVertexList()
        {
            return vertexList;
        }

        public int getNVerts()
        {
            return nVerts;
        }

        public Vertex getVertex(int index)
        {
            return vertexList[index];
        }

        public int getIndex(string id)
        {
            int index;
            for (index = 0; index < nVerts; index++)
                if (vertexList[index].getId() == id)
                    break;
            if (index == nVerts)
                index = -1;
            return index;
        }

        public void addVertex(Vertex vertex)
        {
            vertex.setIndex(nVerts);
            vertexList.Add(vertex);
            nVerts++;
        }

        public void addEdge(int start, int end)
        {
            Vertex vertex1 = vertexList[start];
            Vertex vertex2 = vertexList[end];
            if (vertex1.getNext() != null)
            {
                int index = vertex1.getNext().IndexOf(vertex2);
                if (index != -1)
                {
                    vertex1.setWNext(index, 1);
                }
                else
                    vertex1.addVer(vertex2);
            }
            else
                vertexList[start].addVer(vertexList[end]);
            vertexList[end].addForwardCount(1);
        }
    }
}
