namespace LiebaoAp.Common
{
    using System.Collections;
    using System.Globalization;

    using Newtonsoft.Json;

    public sealed class StatusInfo
    {
        public int WindSpeed { get; private set; }

        public bool AutoMode { get; private set; }

        public bool SilentMode { get; private set; }

        public bool StrongWindMode { get; private set; }

        public bool SleepModeSwitchOff { get; private set; }

        public bool ChildrenGuard { get; private set; }

        public bool SleepMode { get; private set; }

        public bool Power { get; private set; }

        public bool Offline { get; private set; }

        public int PM25 { get; private set; }

        public int Temperature { get; private set; }

        public int Humidity { get; private set; }

        public int TVOCLevel { get; private set; }

        public int TVOCRatio { get; private set; }

        public int PM1 { get; private set; }

        public int PM10 { get; private set; }

        public int Light { get; private set; }

        public int MotoTime { get; private set; }

        public int FilterTime { get; private set; }

        public int FilterPercent { get; private set; }

        public int Reserve { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal static StatusInfo Parse(string statusText)
        {
            var statusBytesLength = statusText.Length / 2;
            var statusBytes = new byte[statusBytesLength];

            for (var idx = 0; idx < statusBytesLength; idx++)
            {
                statusBytes[idx] = byte.Parse(statusText.Substring(idx * 2, 2), NumberStyles.HexNumber);
            }

            var deviceFlags = new BitArray(new[] { statusBytes[1] });

            var statusBytesIndex = 2;

            return new StatusInfo
            {
                WindSpeed = (sbyte)statusBytes[0],
                AutoMode = deviceFlags[0],
                SilentMode = deviceFlags[1],
                StrongWindMode = deviceFlags[2],
                SleepModeSwitchOff = deviceFlags[3],
                ChildrenGuard = deviceFlags[4],
                SleepMode = deviceFlags[5],
                Power = deviceFlags[6],
                Offline = deviceFlags[7],
                PM25 = GetNextShort(statusBytes, ref statusBytesIndex),
                Temperature = GetNextSByte(statusBytes, ref statusBytesIndex),
                Humidity = GetNextSByte(statusBytes, ref statusBytesIndex),
                TVOCLevel = GetNextSByte(statusBytes, ref statusBytesIndex),
                TVOCRatio = GetNextSByte(statusBytes, ref statusBytesIndex),
                PM1 = GetNextShort(statusBytes, ref statusBytesIndex),
                PM10 = GetNextShort(statusBytes, ref statusBytesIndex),
                Light = GetNextShort(statusBytes, ref statusBytesIndex),
                MotoTime = GetNextShort(statusBytes, ref statusBytesIndex),
                FilterTime = GetNextShort(statusBytes, ref statusBytesIndex),
                FilterPercent = GetNextSByte(statusBytes, ref statusBytesIndex),
                Reserve = statusBytes[statusBytesIndex],
            };
        }

        private static short GetNextShort(byte[] bytes, ref int index)
        {
            var result = (short)((bytes[index + 1] << 8) + bytes[index]);
            index += 2;

            return result;
        }

        private static sbyte GetNextSByte(byte[] bytes, ref int index)
        {
            return (sbyte)bytes[index++];
        }
    }
}
