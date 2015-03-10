using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControlService
{
    public interface IDeviceController
    {
        Task<int> Control(string action, string param);

        Task<string> QueryStatus();
    }
}
