﻿using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("DynamicConfig.Test")]
namespace DynamicConfig.ConfigTray.Crypt
{
    internal class DPAPIMachineCipher : ICipher
    {
        IEncryptionProvider _provider = new DpapiProvider(DataProtectionScope.LocalMachine);

        /// <exception cref="CryptographicException">Failed to encrypted value</exception>
        public string EncryptValue(string value)
        {
            string encrypted = EncryptionHelper.Encrypt(_provider, value);
            if (string.IsNullOrEmpty(encrypted))
            {
                throw new CryptographicException("Failed to encrypted value");
            }
            return encrypted;
        }

        /// <exception cref="CryptographicException">Failed to decrypt. It is possible that the cipher text was not encrypted by this machine.</exception>
        public string DecryptValue(string value)
        {
            string decrypted = EncryptionHelper.Decrypt(_provider, value);
            if (string.IsNullOrEmpty(decrypted))
            {
                throw new CryptographicException("Failed to decrypt. It is possible that the cipher text was not encrypted by this machine.");
            }
            return decrypted;
        }
    }
}