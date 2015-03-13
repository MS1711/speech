using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class MultiValueMap
    {
        private Dictionary<string, HashSet<string>> map = new Dictionary<string,HashSet<string>>();

	    public int size() {
		
		    return map.Count;
	    }

	    public bool isEmpty() {
		    return map.Count <= 0;
	    }

	    public bool containsKey(string key) {
		    return map.ContainsKey(key);
	    }

	    public bool containsValue(Object value) {
		    return false;
	    }

	    public HashSet<string> get(string key) {
		    return null;
	    }
	
	
	    public HashSet<string> getSet(string key) {
            if (!map.ContainsKey(key))
            {
                return null;
            }
		    return map[key];
	    }

	    public HashSet<string> put(string key, string value) {
            if (!map.ContainsKey(key) || map[key] == null)
            {
                HashSet<string> set = new HashSet<string>();
                map[key] = set;
            }

            if (value != null)
            {
                map[key].Add(value);
            }
		    return null;
	    }

	    public string remove(string key) {
		    return null;
	    }

	    public void putAll(Dictionary<string, string> d) {

	    }

	    public void clear() {
	    }

        public ICollection<string> keySet()
        {
            return map.Keys;
	    }

	    public List<string> values() {
		    return null;
	    }
	    public ICollection<HashSet<string>> valueSets() {
            return map.Values;
	    }

	    public Object entrySet() {
		    return null;
	    }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();

            return sb.ToString();

        }
    }
}
