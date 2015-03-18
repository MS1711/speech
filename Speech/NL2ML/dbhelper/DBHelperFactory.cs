using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.dbhelper
{
    class DBHelperFactory
    {
        private static IDBHelper helper = new MongoDBHelper();

        public static IDBHelper GetInstance()
        {
            return helper;
        }
    }
}
