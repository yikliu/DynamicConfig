using System;
using System.Security.Cryptography;

namespace DynamicConfig.ConfigTray.Crypt
{
    public class DpapiProvider : IEncryptionProvider
    {
        private DataProtectionScope DataProtectionScope { get; set; }

        public DpapiProvider(DataProtectionScope dataProtectionScope)
        {
            DataProtectionScope = dataProtectionScope;
        }
        
        public string Encrypt(string value)
        {
            return EncryptDPAPI(value, DataProtectionScope);
        }

        public string Decrypt(string value)
        {
            return DecryptDpapi(value, DataProtectionScope);
        }

        private static string EncryptDPAPI(string stringToProtect, DataProtectionScope dataProtectionScope)
        {
            // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
            // only by the same current user.
            var bytes = EncryptionHelper.GetEntropyBytes();
            var encData = Convert.ToBase64String(ProtectedData.Protect(EncryptionHelper.GetBytes(stringToProtect),
                                                                bytes,
                                                                dataProtectionScope));
            var ret = String.Format("{0}###{1}", EncryptionHelper.GetString(bytes), encData);
            return ret;
        }

        private static string DecryptDpapi(string encryptedString, DataProtectionScope dataProtectionScope)
        {
            //Decrypt the data using DataProtectionScope.LocalMachine.
            if (String.IsNullOrEmpty(encryptedString)) return String.Empty;
            if (!encryptedString.Contains("###")) return String.Empty;
            var salt = EncryptionHelper.GetBytes(encryptedString.Split(new string[] { "###" }, StringSplitOptions.None)[0]);

            var hash = encryptedString.Split(new string[] { "###" }, StringSplitOptions.None)[1];
            return EncryptionHelper.GetString(ProtectedData.Unprotect(Convert.FromBase64String(hash), salt, dataProtectionScope));
        }
    }
}
