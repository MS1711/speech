using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPWebService.Models
{
    sealed class MongoDBConstants
    {
        public const string ConnString = "mongodb://127.0.0.1:27017";
        public const string DBName = "knowledgeDB";
        public const string TableCommandCollection = "commandCollection";
        public const string TableMediaCollection = "mediaCollection";
    }
}
