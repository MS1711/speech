using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NL2MLService
{
    [ServiceContract]
    public interface INLPService
    {
        [OperationContract]
        ResultInfo GetResult(string input);
    }

    public class NLPService : INLPService
    {
        private static NL2ML.api.NL2ML engine = NL2ML.api.NL2ML.Instance;

        public ResultInfo GetResult(string input)
        {
            Result res = engine.Process(input);
            ResultInfo info = new ResultInfo();
            info.ErrorCode = res.IsOK ? 0 : 1;
            info.Msg = res.Msg;

            return info;
        }
    }
}
