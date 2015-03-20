using log4net;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NLPWebService.Controllers
{
    [RoutePrefix("nl2ml")]
    public class NL2MLController : ApiController
    {
        private static ILog logger = LogManager.GetLogger("common");
        private static NL2ML.api.NL2ML engine = NL2ML.api.NL2ML.Instance;

        [Route("nl2ml")]
        [HttpGet]
        public Result Parse(string input)
        {
            return engine.Process(input);
        }
    }
}