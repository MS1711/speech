using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.classifier
{
    public class BayesTrainer
    {
        public AbstractClassifier train(InstanceSet trainset)
        {
            AlphabetFactory af = trainset.getAlphabetFactory();
            SeriesPipes pp = (SeriesPipes)trainset.getPipes();
            pp.removeTargetPipe();
            return train(trainset, af, pp);
        }

        public AbstractClassifier train(InstanceSet trainset, AlphabetFactory af, Pipe pp)
        {
            ItemFrequency tf = new ItemFrequency(trainset, af);
            BayesClassifier classifier = new BayesClassifier();
            classifier.setFactory(af);
            classifier.setPipe(pp);
            classifier.setTf(tf);
            return classifier;
        }
    }
}
