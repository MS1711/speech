using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace NLPWebService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FileInfo file = new FileInfo(Server.MapPath("./log4net.config"));
            log4net.Config.XmlConfigurator.ConfigureAndWatch(file);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
