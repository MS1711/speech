namespace LiebaoAp.Common
{
    using System.Diagnostics;

    internal sealed class DummyDeviceRepository : IDeviceRepository
    {
        public DeviceInfo Read(string deviceId)
        {
            Trace.TraceInformation("DummyDeviceRepository.Read(deviceId:'{0}')", deviceId);

            return new DeviceInfo
                       {
                           Uid = "276b677838e3b6059d1b803e4aa949fe",
                           Sid = "9258c7eed7c90e7c71954831a09791c9",
                           Did = "C8:93:46:4A:1E:3D",
                       };
        }
    }
}
