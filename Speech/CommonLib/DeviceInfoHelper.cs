namespace LiebaoAp.Common
{
    internal static class DeviceInfoHelper
    {
        ////TODO: change to real device info repository
        private static readonly IDeviceRepository DeviceRepository = new DummyDeviceRepository();

        public static DeviceInfo GetDeviceInfo(string deviceId)
        {
            return DeviceRepository.Read(deviceId);
        }
    }
}
