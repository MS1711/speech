using LiebaoAp.Common;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NL2ML.handlers
{
    public class SmartDeviceData
    {
        public string Location { get; set; }
        public string Target { get; set; }
        public string TargetClass { get; set; }
        public string Cmd { get; set; }
    }

    public class SmartHouseHandler : IIntentHandler
    {
        private AirController liebaoControler;
        private VirtualSmartDeviceControler vcontroler;

        public SmartHouseHandler()
        {
            liebaoControler = new AirController();
            vcontroler = new VirtualSmartDeviceControler();
        }

        public Result handle(Intent intent)
        {
            Result res = new Result();
            if (intent == null)
            {
                return res;
            }

            switch (intent.Domain)
            {
                case Domains.Media:
                    {
                        return HandleMediaIntent(intent);
                    }
                case Domains.SmartDevice:
                    {
                        return HandleSmartDeviceIntent(intent);
                    }
            }

            return res;
        }

        private Result HandleSmartDeviceIntent(Intent intent)
        {
            SmartDeviceData data = intent.Data as SmartDeviceData;
            if (data != null && !string.IsNullOrEmpty(data.TargetClass) && 
                (data.TargetClass.Equals("airpurifier")))
            {
                return HandleAirControlerAction(intent);
            }
            else
            {
                return HandleVirtualDeviceAction(intent);
            }
            
        }

        private Result HandleMediaIntent(Intent intent)
        {
            Result res = new Result();
            if (intent.Action == Actions.Stop)
            {
                return vcontroler.DoMediaAction("STOP", "X", "X", "X");
            }
            else if (intent.Action == Actions.Play)
            {
                MediaData data = intent.Data as MediaData;
                string url = data.Url;
                if (string.IsNullOrEmpty(url))
                {
                    url = data.Durl;
                }

                if (string.IsNullOrEmpty(url))
                {
                    return res;
                }
                return vcontroler.DoMediaAction("START", data.Category.ToString().ToUpper(), url, (string.IsNullOrEmpty(data.Artist) ? "" : data.Artist + "-") + data.Name);
            }

            return res;
        }

        private Result HandleVirtualDeviceAction(Intent intent)
        {
            SmartDeviceData data = intent.Data as SmartDeviceData;
            return vcontroler.DoVirtualDeviceAction(data.Cmd, data.Location, data.Target, data.TargetClass);
        }

        private Result HandleAirControlerAction(Intent intent)
        {
            Result result = new Result();
            Actions action = intent.Action;
            switch (action)
            {
                case Actions.Open:
                    {
                        int res = liebaoControler.ControlDevice("", "start", "").Result;
                        result.IsOK = true;
                        break;
                    }
                case Actions.Close:
                    {
                        int res = liebaoControler.ControlDevice("", "stop", "").Result;
                        result.IsOK = true;
                        break;
                    }
                case Actions.Query:
                    {
                        string msg = liebaoControler.QueryDeviceStatus("").Result;
                        result.Msg = msg;
                        result.IsOK = true;
                        break;
                    }
            }

            return result;
        }
    }

    public class VirtualSmartDeviceControler
    {
        private const string FilePath = @"C:/workspace/newTemp.txt";

        public Result DoVirtualDeviceAction(string action, string location, string target, string targetClass)
        {
            SendWebRequest(action.ToUpper() + "$" + targetClass.ToUpper() + "$" + location.ToUpper() + "$" + target.ToUpper());
            return new Result() { IsOK = true, Msg = "" };
        }

        internal Result DoMediaAction(string action, string category, string url, string artistAndSong)
        {
            SendWebRequest(action.ToUpper() + "$" + category.ToUpper() + "$" + url + "$" + artistAndSong);
            return new Result() { IsOK = true, Msg = "" };
        }

        private void SendRequest(string request)
        {
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            using (StreamWriter sw = new StreamWriter(FilePath, false, utf8WithoutBom))
            {
                sw.WriteLine(request);
                sw.Flush();
            }
        }

        private void SendWebRequest(string req)
        {
            string url = "http://localhost:3000/sendreq?txt={0}";
            string u = string.Format(url, HttpUtility.UrlEncode(req));
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(u);
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
            }
        }
    }

    public class AirController
    {
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

    public interface IDeviceController
    {
        Task<int> Control(string action, string param);

        Task<string> QueryStatus();
    }

    public class LieBaoAirController : IDeviceController
    {
        public string deviceId { get; set; }

        public LieBaoAirController(object[] param)
        {
            this.deviceId = param[0].ToString();
        }

        public async Task<int> OpenDevice()
        {
            var command = new Command { DeviceId = deviceId, Action = CommandAction.TurnOn, };
            var res = DeviceHelper.ExecuteCommandAsync(command);
            await res;
            return res.Result ? 0 : 1;
        }

        public async Task<int> CloseDevice()
        {
            var command = new Command { DeviceId = deviceId, Action = CommandAction.TurnOff, };
            var res = DeviceHelper.ExecuteCommandAsync(command);
            await res;
            return res.Result ? 0 : 1;
        }

        public async Task<int> SelectMode(string mode)
        {
            CommandAction act = CommandAction.Unknown;
            switch (mode)
            {
                case "strong":
                    {
                        act = CommandAction.SetStrongMode;
                        break;
                    }
                case "mute":
                    {
                        act = CommandAction.SetMuteMode;
                        break;
                    }
                case "auto":
                    {
                        act = CommandAction.SetAutoMode;
                        break;
                    }
            }

            if (act == CommandAction.Unknown)
            {
                return -1;
            }

            var command = new Command { DeviceId = deviceId, Action = act, };
            var res = DeviceHelper.ExecuteCommandAsync(command);
            await res;
            return res.Result ? 0 : 1;
        }

        public async Task<int> Control(string action, string param)
        {
            if (string.IsNullOrEmpty(action))
            {
                return -1;
            }

            action = action.ToLower();
            switch (action)
            {
                case "start":
                    {
                        var res = OpenDevice();
                        await res;
                        return res.Result;
                    }
                case "stop":
                    {
                        var res = CloseDevice();
                        await res;
                        return res.Result;
                    }
                case "up":
                    {
                        var res = SelectMode("strong");
                        await res;
                        return res.Result;
                    }
                case "down":
                    {
                        var res = SelectMode("mute");
                        await res;
                        return res.Result;
                    }
                default:
                    {
                        break;
                    }
            }

            return -1;
        }


        public async Task<string> QueryStatus()
        {
            var command = new Command { DeviceId = deviceId, Action = CommandAction.GetStatus, };
            await DeviceHelper.ExecuteCommandAsync(command);
            StatusInfo sinfo = command.Result as StatusInfo;
            string pattern = "豹米先生说，今天室内温度是{0}°C,PM2.5指数是{1},空气湿度为{2}";
            string res = string.Format(pattern, new object[] { sinfo.Temperature, sinfo.PM25 == -1 ? 5 : sinfo.PM25, sinfo.Humidity });
            return res;
        }
    }

    public enum DeviceType
    {
        Invalid,
        LieBaoAirController
    }

    public sealed class DeviceTypeController
    {
        public static Dictionary<DeviceType, Type> Controller = new Dictionary<DeviceType, Type>();

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
