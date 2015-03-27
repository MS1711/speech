using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NL2MLService
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo log4netConfig = new FileInfo("log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(log4netConfig);

            using (ServiceHost host = new ServiceHost(typeof(NLPService)))
            {
                ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (smb == null)
                    host.Description.Behaviors.Add(new ServiceMetadataBehavior());
                host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
                host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                host.Open();
                Console.WriteLine("Service listen begin to listen...");
                Console.WriteLine("press any key to teriminate...");
                Console.ReadKey();
                host.Abort();
                host.Close();
            }
        }
    }
}
