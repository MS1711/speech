using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public abstract class Pipe
    {
        /**
	     * 用来判断是否使用类别，以便在无类别使用时删掉
	     */
	    public bool useTarget = false;
	
	    /**
	     * 基本的数据类型转换处理操作，继承类需重新定义实现
	     * @param inst 样本
	     * @throws Exception
	     */
	    public abstract void addThruPipe(Instance inst);
	
	    /**
	     * 通过pipe直接处理实例
	     * @param instSet
	     * @throws Exception
	     */
	    public void process(InstanceSet instSet) {
		    for(int i=0; i < instSet.Count; i++){
			    Instance inst = instSet.getInstance(i);
			    addThruPipe(inst);
		    }
	    }
    }
}
