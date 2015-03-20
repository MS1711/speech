using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLPWebService.Models
{
    public class ResultInfo<T>
    {
        public T Msg { get; set; }
        public int Code { get; set; }
    }
}