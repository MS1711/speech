using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class LabelAlphabet : ILabelAlphabet<string>
    {
	    /**
	     * 数据
	     */
	    protected Dictionary<string, int> data;

	    /**
	     * 标记索引
	     */
	    private Dictionary<int, string> index;
	    /**
	     * 是否冻结
	     */
	    protected bool frozen;

	    public LabelAlphabet() {
            data = new Dictionary<string, int>();
		    frozen = false;
		    index = new Dictionary<int, string>();
	    }

        public void LoadData(string file)
        {
            data["S"] = 0;
            data["B"] = 1;
            data["M"] = 2;
            data["E"] = 3;

            index[0] = "S";
            index[1] = "B";
            index[2] = "M";
            index[3] = "E";
        }

	    public int size() {
            
		    return index.Count;
	    }

        public string lookupString(int id)
        {
            return index[id];
        }

        public string[] lookupString(int[] ids)
        {
            string[] vals = new string[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                vals[i] = index[ids[i]];
            }
            return vals;
        }

        public bool isStopIncrement()
        {
            throw new NotImplementedException();
        }

        public void setStopIncrement(bool b)
        {
            throw new NotImplementedException();
        }

        public int lookupIndex(string str)
        {
            int ret = -1;
            if (data.ContainsKey(str))
            {
                ret = data[str];
            }
            if (ret == -1 && !frozen)
            {
                ret = index.Count;
                data[str] = ret;
                index[ret] = str;
            }
            return ret;
        }

        public void clear()
        {
            throw new NotImplementedException();
        }

        public void LoadDataPOS(string file)
        {
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                string str = sr.ReadLine();
                while (str != null)
                {
                    int indx = str.LastIndexOf('=');
                    string k = str.Substring(0, indx);
                    int v = int.Parse(str.Substring(indx + 1));
                    data[k] = v;
                    index[v] = k;
                    str = sr.ReadLine();
                }
            }
        }
    }
}
