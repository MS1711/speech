namespace LiebaoAp.Common.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    internal static class EncodingUtility
    {
        private static readonly byte[] EncryptKey =
            Encoding.ASCII.GetBytes("x1bgQsyMlpItQkYJWulAfZuLYKlOSDFA").Take(16).ToArray();

        private static readonly byte[] EncryptIv = Encoding.ASCII.GetBytes("0123456789ABCDEF");

        public static string Encode(string rawText)
        {
            var encryptedBytes = Encrypt(rawText);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decode(string encodedText)
        {
            var encryptedBytes = Convert.FromBase64String(encodedText);

            return Decrypt(encryptedBytes).Trim();
        }

        private static byte[] Encrypt(string rawText)
        {
            var rawBytes = Encoding.ASCII.GetBytes(rawText);

            if (rawBytes.Length % 16 != 0)
            {
                var paddedRawBytesLength = ((rawBytes.Length / 16) + 1) * 16;
                var paddedRawBytes = new byte[paddedRawBytesLength];
                Array.Copy(rawBytes, paddedRawBytes, rawBytes.Length);

                rawBytes = paddedRawBytes;
            }

            using (var rij = GetRijndaelInstance())
            {
                var encryptor = rij.CreateEncryptor();

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(rawBytes, 0, rawBytes.Length);
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        private static string Decrypt(byte[] encryptedBytes)
        {
            using (var rij = GetRijndaelInstance())
            {
                var decryptor = rij.CreateDecryptor();

                using (var memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static RijndaelManaged GetRijndaelInstance()
        {
            return new RijndaelManaged
            {
                Key = EncryptKey,
                IV = EncryptIv,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
            };
        }
    }
}
