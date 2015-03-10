namespace LiebaoAp.Common
{
    public sealed class Command
    {
        public string DeviceId { get; set; }

        public CommandAction Action { get; set; }

        public string Arguments { get; set; }

        public object Result { get; set; }
    }
}
