using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public abstract class AbstractClassifier
    {
	
	    protected AlphabetFactory factory;
	
	
	    public AlphabetFactory getAlphabetFactory() {
		    return factory;
	    }

        public Predict<string[]> classify(Instance instance, LabelParser.Type t)
        {
            return classify(instance, t, 1);
        }

        public Predict<int[]> classify(Instance instance)
        {
            return classify(instance, 1);
        }

        public abstract Predict<int[]> classify(Instance instance, int n);

        public abstract Predict<string[]> classify(Instance instance, LabelParser.Type type, int n);

        public virtual Predict<int> doClassify(Instance instance, int n)
        {
            return null;
        }

        public virtual Predict<string> doClassify(Instance instance, LabelParser.Type type, int n)
        {
            return null;
        }
    }
}
