using System;
using System.Security.Cryptography;

namespace DynamicConfig.ConfigTray.Crypt
{
    public static class EncryptionHelper
    {
        public static string Encrypt(IEncryptionProvider encryptionProvider, string value)
        {
            return encryptionProvider.Encrypt(value);
        }

        public static string Decrypt(IEncryptionProvider encryptionProvider, string value)
        {
            return encryptionProvider.Decrypt(value);
        }

        public static byte[] GetEntropyBytes()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[8];
                rng.GetBytes(data);
                return data;
            }
        }
        
        public static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        
        public static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}