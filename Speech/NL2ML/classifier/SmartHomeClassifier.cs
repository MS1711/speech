using NL2ML.models;
using NLPCore;
using NLPCore.classifier;
using NLPCore.models;
using NLPCore.pipes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.classifier
{
    public class SmartHomeClassifier
    {
        public static string trainDataPath = "C:/workspace/traindata/";
        public static string bayesModelFile = trainDataPath + "modelBayes.gz";
        public static string modelPath = ConfigurationManager.AppSettings["ModelPath"];

        public void Train(string samplePath)
        {
            //分词
            
		    NLPDictionary dict = new NLPDictionary(true);
		    string[] dictPath = new string[]{
				    "C:/workspace/nlpdictdata/mydict2.txt", 
				    "C:/workspace/nlpdictdata/genredict.txt",
                    "C:/workspace/nlpdictdata/verbdict2.txt",
                    "C:/workspace/nlpdictdata/lastdict2.txt",
                    "C:/workspace/nlpdictdata/devicedict2.txt"
		    };
		
		    foreach (var str in dictPath) {
			    dict.addFile(str);
		    }
		    CWSTagger tag = new CWSTagger(modelPath);
            tag.setDictionary(dict);

		    Pipe segpp=new CNPipe(tag);
		    Pipe s2spp=new Strings2StringArray();
		    /**
		     * Bayes
		     */
		    //建立字典管理器
		    AlphabetFactory af = AlphabetFactory.buildFactory();
		    //将字符特征转换成字典索引;	
		    Pipe sparsepp=new StringArray2SV(af);
		    //将目标值对应的索引号作为类别
		    Pipe targetpp = new Target2Label(af.DefaultLabelAlphabet());	
		    //建立pipe组合
		    SeriesPipes pp = new SeriesPipes(new Pipe[]{segpp,s2spp,targetpp,sparsepp});

		    InstanceSet instset = new InstanceSet(pp,af);
            Console.WriteLine("..Reading data complete\n");
		    //Reader reader = new MyDocumentReader(trainDataPath,"utf-8");
		    Reader reader = new FileReader(trainDataPath,"UTF-8",".data");
		    instset.loadThruStagePipes(reader);
		    Console.WriteLine("..Reading data complete\n");
		
		    //将数据集分为训练是和测试集
		    Console.WriteLine("Sspliting....");
		    float percent = 0.8f;
		    InstanceSet[] splitsets = instset.Split(percent);
		
		    InstanceSet trainset = splitsets[0];
		    InstanceSet testset = splitsets[1];	
		    Console.WriteLine("..Spliting complete!\n");

		    Console.WriteLine("Training...\n");
		    af.setStopIncrement(true);
		    BayesTrainer trainer=new BayesTrainer();
		    BayesClassifier classifier= (BayesClassifier) trainer.train(trainset);
		    Console.WriteLine("..Training complete!\n");
		    Console.WriteLine("Saving model...\n");

		    classifier.saveTo(bayesModelFile);	
		    classifier = null;
		    Console.WriteLine("..Saving model complete!\n");
		    /**
		     * 测试
		     */
		    Console.WriteLine("Loading model...\n");
		    BayesClassifier bayes;
		    bayes =BayesClassifier.loadFrom(bayesModelFile);
            Console.WriteLine("..Loading model complete!\n");
		
		
		    bayes.fS_CS(1.0f);
		    string teststr = "打开客厅的灯";
            Console.WriteLine("============\n分类：" + teststr);
            Instance inst = new Instance(teststr);
			pp.addThruPipe(inst);

            Predict<string> pres = bayes.doClassify(inst, LabelParser.Type.STRING, 3);
		    Console.WriteLine(pres);
		    String pred_label=pres.getLabel();
		
    //		String res = bayes.getStringLabel(inst);
    //		System.out.println("xxx");	
		    Console.WriteLine("类别："+ pred_label);
        }

        public Domains GetDomain(string str)
        {
			NLPDictionary dict = new NLPDictionary(true);
			String[] dictPath = new String[]{
					modelPath + "mydict2.txt", 
					modelPath + "genredict.txt",
	                modelPath + "verbdict2.txt",
	                modelPath + "lastdict2.txt",
	                modelPath + "devicedict2.txt"
			};
			
			foreach (var stri in dictPath) {
				dict.addFile(stri);
			}
			CWSTagger tag = new CWSTagger(modelPath);
			
			BayesClassifier bayes = BayesClassifier.loadFrom(bayesModelFile);
			bayes.fS_CS(1.0f);
			Pipe pipe = bayes.getPipe();
			if (pipe is SeriesPipes) {
				SeriesPipes sp = (SeriesPipes) pipe;
				foreach (Pipe pp in sp.getPipes()) {
					if (pp is CNPipe) {
						((CNPipe)pp).setSeg(tag);
					}
				}
			}
			Instance inst = new Instance(str);
			//特征转换
			pipe.addThruPipe(inst);
			
			Predict<string> pres=bayes.doClassify(inst, LabelParser.Type.STRING, 3);
			bool failed = true;
			foreach (float sc in pres.scores) {
				if (sc != 0) {
					failed = false;
					break;
				}
			}
			
			if (failed) {
				return Domains.Invalid;
			} else {
				string pred_label = pres.getLabel();
                switch (pred_label)
                {
                    case "media":
                        return Domains.Media;
                    case "device":
                        return Domains.SmartDevice;
                }
			}

            return Domains.Invalid;
        }
    }
}
