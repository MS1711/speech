namespace LiebaoAp.Common
{
    internal interface IDeviceRepository
    {
        DeviceInfo Read(string deviceId);
    }
}
