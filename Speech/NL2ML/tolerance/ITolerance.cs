using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.tolerance
{
    public interface ITolerance
    {
        string Correct(string orginal, Context context);
    }
}
