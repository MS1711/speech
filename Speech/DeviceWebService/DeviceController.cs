using DeviceControlService;
using LiebaoAp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DeviceWebService
{
    [RoutePrefix("device")]
    public class DeviceController : ApiController
    {
        [Route("update")]
        [HttpPost]
        public string UpdateDeviceStatus(string deviceId, string deviceStatus)
        {
            return "Hello World";
        }

        [Route("control")]
        [HttpGet]
        public async Task<int> ControlDevice(string deviceId, string action, string param)
        {
            DeviceInfo device = DeviceDirectory.LookupDevice(deviceId);
            if (device == null)
            {
                return -1;
            }

            IDeviceController cont = device.GetController();
            var status = cont.Control(action, param);
            await status;
            return status.Result;
        }

        [Route("query")]
        [HttpGet]
        public async Task<string> QueryDeviceStatus(string deviceId)
        {
            DeviceInfo device = DeviceDirectory.LookupDevice(deviceId);
            if (device == null)
            {
                return null;
            }

            IDeviceController cont = device.GetController();
            var status = cont.QueryStatus();
            await status;
            return status.Result;
        }
    }
}