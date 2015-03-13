using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public abstract class AbstractTagger
    {
        protected Linear cl;
	    protected Pipe prePipe=null;
	    protected Pipe featurePipe;
	    public AlphabetFactory factory;
	    protected TempletGroup templets;
	    protected LabelAlphabet labels;
	    /**
	     * 词之间间隔标记，缺省为空格。
	     */
	    protected String delim = " ";

	    /**
	     * 抽象标注器构造函数
	     * @param file 模型文件
	     * @throws LoadModelException
	     */
	    public AbstractTagger(String file) {
		    loadFrom(file);
		    if(getClassifier()==null){
			    throw new LoadModelException("模型为空");
		    }

		    factory = getClassifier().getAlphabetFactory();
		    labels = factory.DefaultLabelAlphabet();
		    IFeatureAlphabet features = factory.DefaultFeatureAlphabet();
		    featurePipe = new Sequence2FeatureSequence(templets, features,
				    labels);
	    }

        public Linear getClassifier()
        {
            return cl;
        }

        public void setClassifier(Linear cl)
        {
            this.cl = cl;
        }
	
        virtual public void loadFrom(string modelfile) {
			//load models
            
        }

        

	    public AbstractTagger() {
	    }
	

        protected string[] _tag(Instance  inst)	{
		    doProcess(inst);
		    Predict<string[]> pred = getClassifier().classify(inst,LabelParser.Type.SEQ);
		    if (pred == null)
			    return new string[0];
		    return (string[]) pred.getLabel(0);
	    }

        public void doProcess(Instance carrier)	{
			    if(prePipe!=null)
				    prePipe.addThruPipe(carrier);
			    carrier.setSource(carrier.getData());
			    featurePipe.addThruPipe(carrier);
		    
	    }
    }
}
