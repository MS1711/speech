using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NL2MLService
{
    [DataContract]
    public class ResultInfo
    {
        private int errorCode;

        [DataMember]
        public int ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }
        private string msg;

        [DataMember]
        public string Msg
        {
            get { return msg; }
            set { msg = value; }
        }
    }
}
