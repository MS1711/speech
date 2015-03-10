using LiebaoAp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DeviceControlService
{
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
            return res.Result? 0 : 1;
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
            string res = string.Format(pattern, new object[] { sinfo.Temperature, sinfo.PM25==-1?5:sinfo.PM25, sinfo.Humidity });
            return res;
        }
    }
}