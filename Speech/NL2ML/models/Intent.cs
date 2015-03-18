using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.models
{
    public class Intent
    {
        public Domains Domain { get; set; }
        public Actions Action { get; set; }
        public object Data { get; set; }
        public int Score { get; set; }
        public string RawCommand { get; set; }
    }
}
