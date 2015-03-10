namespace LiebaoAp.Common
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using LiebaoAp.Common.Utilities;

    using Newtonsoft.Json;

    public static class DeviceHelper
    {
        private const string GetStatusEndpointUrl = "http://ap.liebao.cn/manage/index.php?r=userDev/getDev";

        private const string SendCommandEndpointUrl = "http://ap.liebao.cn/manage/index.php?r=index/send";

        public static bool ExecuteCommand(Command command)
        {
            return ExecuteCommandAsync(command).Result;
        }

        public static async Task<bool> ExecuteCommandAsync(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            Trace.TraceInformation(
                "DeviceHelper.ExecuteCommandAsync(deviceId:'{0}', action:'{1}', arguments:'{2}')",
                command.DeviceId,
                command.Action,
                command.Arguments);

            var deviceInfo = DeviceInfoHelper.GetDeviceInfo(command.DeviceId);

            if (deviceInfo == null)
            {
                throw new InvalidOperationException("Can't find device: " + command.DeviceId);
            }

            Trace.TraceInformation(
                "DeviceHelper.ExecuteCommandAsync(uid:'{0}', sid:'{1}', did:'{2}')",
                deviceInfo.Did,
                deviceInfo.Sid,
                deviceInfo.Did);

            try
            {
                switch (command.Action)
                {
                    case CommandAction.TurnOn:
                        return await ExecuteTurnOnAction(deviceInfo).ConfigureAwait(false);
                    case CommandAction.TurnOff:
                        return await ExecuteTurnOffAction(deviceInfo).ConfigureAwait(false);
                    case CommandAction.SetMuteMode:
                        return await ExecuteSetMuteModeAction(deviceInfo).ConfigureAwait(false);
                    case CommandAction.SetAutoMode:
                        return await ExecuteSetAutoModeAction(deviceInfo).ConfigureAwait(false);
                    case CommandAction.SetStrongMode:
                        return await ExecuteSetStrongModeAction(deviceInfo).ConfigureAwait(false);
                    case CommandAction.GetStatus:
                        command.Result = await ExecuteGetStatusAction(deviceInfo);
                        return command.Result != null;
                    default:
                        throw new NotSupportedException("Unknown action: " + command.Action);
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.GetDetail());
                return false;
            }
        }

        private static async Task<StatusInfo> ExecuteGetStatusAction(DeviceInfo deviceInfo)
        {
            var messageData = new { uid = deviceInfo.Uid, sid = deviceInfo.Sid };

            var requestText = JsonConvert.SerializeObject(messageData, Formatting.None);

            Trace.TraceInformation("DeviceHelper.ExecuteGetStatusAction(requestText:'{0}')", requestText);

            var responseText = await HttpUtility.PostAsync(GetStatusEndpointUrl, requestText);

            Trace.TraceInformation("DeviceHelper.ExecuteGetStatusAction(responseText:'{0}')", responseText);

            var responseData = JsonConvert.DeserializeObject<dynamic>(responseText);

            StatusInfo statusInfo = null;
            if (((int)responseData.ret) == 0)
            {
                var statusText = (string)responseData.data[0].status;

                statusInfo = StatusInfo.Parse(statusText);
            }

            Trace.TraceInformation("DeviceHelper.ExecuteGetStatusAction(statusInfo:'{0}')", statusInfo);

            return statusInfo;
        }

        private static async Task<bool> ExecuteSetStrongModeAction(DeviceInfo deviceInfo)
        {
            return await ExecuteCommandAction(deviceInfo, CommandMessages.SetStrongMode);
        }

        private static async Task<bool> ExecuteSetAutoModeAction(DeviceInfo deviceInfo)
        {
            return await ExecuteCommandAction(deviceInfo, CommandMessages.SetAutoMode);
        }

        private static async Task<bool> ExecuteSetMuteModeAction(DeviceInfo deviceInfo)
        {
            return await ExecuteCommandAction(deviceInfo, CommandMessages.SetMuteMode);
        }

        private static async Task<bool> ExecuteTurnOffAction(DeviceInfo deviceInfo)
        {
            return await ExecuteCommandAction(deviceInfo, CommandMessages.TurnOff);
        }

        private static async Task<bool> ExecuteTurnOnAction(DeviceInfo deviceInfo)
        {
            return await ExecuteCommandAction(deviceInfo, CommandMessages.TurnOn).ConfigureAwait(false);
        }

        private static async Task<bool> ExecuteCommandAction(DeviceInfo deviceInfo, string commandMessage)
        {
            var messageData =
                new { uid = deviceInfo.Uid, sid = deviceInfo.Sid, did = deviceInfo.Did, message = commandMessage, };

            var requestText = JsonConvert.SerializeObject(messageData, Formatting.None);

            Trace.TraceInformation("DeviceHelper.ExecuteCommandAction(requestText:'{0}')", requestText);

            var responseText = await HttpUtility.PostAsync(SendCommandEndpointUrl, requestText).ConfigureAwait(false);

            Trace.TraceInformation("DeviceHelper.ExecuteCommandAction(responseText:'{0}')", responseText);

            var responseData = JsonConvert.DeserializeObject<dynamic>(responseText);

            return ((int)responseData.ret) == 0;
        }
    }
}
