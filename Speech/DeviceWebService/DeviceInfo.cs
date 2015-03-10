using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceControlService
{
    public enum DeviceType
    {
        Invalid,
        LieBaoAirController
    }

    public sealed class DeviceTypeController
    {
        public static Dictionary<DeviceType, Type> Controller = new Dictionary<DeviceType,Type>();

        static DeviceTypeController()
        {
            Controller[DeviceType.LieBaoAirController] = typeof(LieBaoAirController);
        }
        
    }

    public class DeviceInfo
    {
        public string DeviceId { get; set; }
        public DeviceType DeviceType { get; set; }

        public object[] controlParams { get; set; }
        private IDeviceController controller;

        public IDeviceController GetController()
        {
            if (controller != null)
            {
                return controller;
            }

            Type t = DeviceTypeController.Controller[DeviceType];
            if (t != null)
            {
                object[] par = new object[1];
                par[0] = controlParams;
                controller = Activator.CreateInstance(t, par) as IDeviceController;
            }

            return controller;
        }
    }

    public class DeviceDirectory
    {
        public static Dictionary<string, DeviceInfo> dict = new Dictionary<string, DeviceInfo>();

        static DeviceDirectory()
        {
            DeviceInfo dinfo = new DeviceInfo();
            dinfo.DeviceId = "airpurifier@room";
            dinfo.DeviceType = DeviceType.LieBaoAirController;
            dinfo.controlParams = new object[] { Guid.NewGuid().ToString("N") };
            Register(dinfo.DeviceId, dinfo);
        }

        public static DeviceInfo LookupDevice(string deviceId)
        {
            return dict["airpurifier@room"];
        }

        public static void Register(string deviceId, DeviceInfo info)
        {
            dict[deviceId] = info;
        }
    }
}