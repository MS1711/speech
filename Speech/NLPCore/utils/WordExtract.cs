using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLPCore.utils
{
    public class WDataSet
    {
        public Graph graph = new Graph();
        public List<double> w = new List<double>();
        public List<double> wBack = new List<double>();
        public List<string> list = new List<string>();
        public List<string> subList = new List<string>();
    }

    public class WordExtract : AbstractExtractor
    {
        public WordExtract()
        {
            precision = 1.0;
            dN = 0.85;
        }

        public WordExtract(string segPath, string dicPath)
        {
            tag = new CWSTagger(segPath);
            test = new StopWords(dicPath);
        }

        public WordExtract(CWSTagger tag, string dicPath)
        {
            this.tag = tag;
            test = new StopWords(dicPath);
        }
        public WordExtract(CWSTagger tag, StopWords test)
        {
            this.tag = tag;
            this.test = test;
        }

        private WDataSet getWord(string[] words)
        {
            HashSet<string> set = new HashSet<string>();
            WDataSet wds = new WDataSet();

            if (test != null)
            {
                wds.list = test.phraseDel(words);
            }
            else
            {
                wds.list = new List<string>();
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Length > 0)
                        wds.list.Add(words[i]);
                }
            }



            for (int i = 0; i < wds.list.Count; i++)
            {
                string temp = wds.list[i];
                set.Add(temp);
            }

            IEnumerator<string> ii = set.GetEnumerator();
            while (ii.MoveNext())
            {
                string str = ii.Current;
                wds.subList.Add(str);
            }
            return wds;
        }

        private WDataSet mapInit(int window, WDataSet wds)
        {
            Dictionary<string, int> treeMap = new Dictionary<string, int>();
            IEnumerator<string> ii = wds.subList.GetEnumerator();
            int nnn = 0;
            while (ii.MoveNext())
            {
                string s = ii.Current;
                Vertex vertex = new Vertex(s);
                wds.graph.addVertex(vertex);
                wds.w.Add(1.0);
                wds.wBack.Add(1.0);
                treeMap[s] = nnn;
                nnn++;
            }

            string id1, id2;
            int index1, index2;

            int length = wds.list.Count;
            while (true)
            {
                if (window > length)
                    window /= 2;
                else if (window <= length || window <= 3)
                    break;
            }

            for (int i = 0; i < wds.list.Count - window; i++)
            {
                id1 = wds.list[i];
                index1 = treeMap[id1];
                for (int j = i + 1; j < i + window; j++)
                {
                    id2 = wds.list[j];
                    index2 = treeMap[id2];
                    wds.graph.addEdge(index2, index1);
                }
            }
            return wds;
        }

        bool isCover(WDataSet wds)
        {
            int i;
            double result = 0.0;

            for (i = 0; i < wds.graph.getNVerts(); i++)
            {
                result += Math.Abs(wds.w[i] - wds.wBack[i]);
            }

            if (result < precision)
                return true;
            else
                return false;
        }

        public void toBackW(WDataSet wds)
        {
            int i;

            for (i = 0; i < wds.graph.getNVerts(); i++)
            {
                wds.wBack[i] = wds.w[i];
            }
        }

        public WDataSet cal(WDataSet wds)
        {
            int i, j, forwardCount, times = 0;
            double sumWBackLink, newW;
            List<Vertex> nextList;
            List<int> nextWList;
            Vertex vertex;

            while (true)
            {
                times++;
                for (i = 0; i < wds.graph.getNVerts(); i++)
                {
                    vertex = wds.graph.getVertex(i);
                    nextList = vertex.getNext();
                    nextWList = vertex.getWNext();
                    if (nextList != null)
                    {
                        sumWBackLink = 0;
                        for (j = 0; j < nextWList.Count; j++)
                        {
                            vertex = nextList[j];
                            int ww = nextWList[j];
                            int temp = vertex.index;
                            forwardCount = vertex.getForwardCount();
                            if (forwardCount != 0)
                                sumWBackLink += wds.wBack[temp] * ww / forwardCount;
                        }
                        newW = (1 - dN) + dN * sumWBackLink;
                        wds.w[i] = newW;
                    }
                }
                if (isCover(wds) == true)
                {
                    //				System.out.println("Iteration Times: " + times);
                    break;
                }
                toBackW(wds);
            }
            return wds;
        }

        public List<int> normalized(WDataSet wds)
        {
            List<int> wNormalized = new List<int>();
            double max, min, wNDouble;
            int i, wNormalInt;
            double wNormal;
            max = wds.w.Max();
            min = wds.w.Min();

            if (max != min)
                for (i = 0; i < wds.graph.getNVerts(); i++)
                {
                    wNDouble = wds.w[i];
                    wNormal = (wNDouble - min) / (max - min);
                    wNormalInt = (int)(100 * wNormal);
                    wds.w[i] = wNormal;
                    wNormalized.Add(wNormalInt);
                }
            else
                for (i = 0; i < wds.graph.getNVerts(); i++)
                    wNormalized.Add(100);
            return wNormalized;
        }

        public Dictionary<string, int> selectTop(int selectCount, WDataSet wds)
        {
            int i, j, index;
            double max;
            Dictionary<string, int> mapList = new Dictionary<string, int>();

            if (wds.graph.getNVerts() == 0)
                return mapList;

            List<int> wNormalized = normalized(wds);
            toBackW(wds);

            int temp = wds.subList.Count;
            if (selectCount > temp)
                selectCount = temp;

            for (j = 0; j < selectCount; j++)
            {
                max = -1.0;
                index = -1;
                for (i = 0; i < wds.graph.getNVerts(); i++)
                {
                    if (wds.wBack[i] > max)
                    {
                        max = wds.wBack[i];
                        index = i;
                    }
                }
                if (index != -1)
                {
                    mapList[wds.graph.getVertex(index).getId()] = wNormalized[index];
                    wds.wBack[index] = -2.0f;
                }
            }
            return mapList;
        }

        public WDataSet proceed(string[] words)
        {
            WDataSet wds1, wds2;
            wds1 = getWord(words);
            //		long time1 = System.currentTimeMillis();
            //		System.out.println("InitGraph...");
            wds2 = mapInit(windowN, wds1);
            //		System.out.println("Succeed In InitGraph!");
            //		System.out.println("Now Calculate the PageRank Value...");
            wds1 = cal(wds2);
            //		double time = (System.currentTimeMillis() - time1) / 1000.0;
            //		System.out.println("Time using: " + time + "s");
            //		System.out.println("PageRank Value Has Been Calculated!");
            return wds1;
        }


        public override string extract(string str, int num, bool isWeighted)
        {
            return extract(str, num).ToString();
        }

        public override Dictionary<string, int> extract(string str, int i)
        {
            string[] words;
            if (tag != null)
                words = tag.tag2Array(str);
            else
                words = Regex.Split(str, "\\s+");
            WDataSet wds = proceed(words);
            Dictionary<string, int> mapList = selectTop(i, wds);
            return mapList;
        }
    }
}
