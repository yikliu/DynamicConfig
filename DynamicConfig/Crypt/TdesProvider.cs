using System;
using System.Security.Cryptography;
using System.Text;

namespace DynamicConfig.ConfigTray.Crypt
{
    class TdesProvider: IEncryptionProvider
    {
        private string Passphrase { get; set; }
        private string Salt { get; set; }

        public TdesProvider(string passphrase)
        {
            Passphrase = passphrase;
        }

        private static string EncryptTdes(string stringToEncrypt, string decryptedPassphrase)
        {
            var rngGenerator = new RNGCryptoServiceProvider();
            var saltybytes = new byte[8];
            rngGenerator.GetBytes(saltybytes);
            var salt = Convert.ToBase64String(saltybytes);
            byte[] results;
            var utf8 = new UTF8Encoding();
            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(decryptedPassphrase));
            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var tdesAlgorithm = new TripleDESCryptoServiceProvider();
            // Step 3. Setup the encoder
            tdesAlgorithm.Key = tdesKey;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;
            // Step 4. Convert the input string to a byte[]
            byte[] dataToEncrypt = utf8.GetBytes(salt + stringToEncrypt);
            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform encryptor = tdesAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }
            // Step 6. Return the encrypted string as a base64 encoded string
            return string.Format("{0}###{1}", salt, Convert.ToBase64String(results));
        }

        private static string DecryptTdes(string message, string passphrase)
        {
            byte[] results;
            if (!message.Contains("###")) throw new Exception("Cannot find the Salt!");
            var salt = message.Split(new string[] {"###"}, StringSplitOptions.None)[0];
            var encryptedValue = message.Split(new string[] { "###" }, StringSplitOptions.None)[1];
            var utf8 = new UTF8Encoding();
            // Step 1. We hash the passPhrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphrase));
            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var tdesAlgorithm = new TripleDESCryptoServiceProvider { Key = tdesKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            // Step 3. Setup the decoder
            tdesAlgorithm.Key = tdesKey;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;
            // Step 4. Convert the input string to a byte[]
            byte[] dataToDecrypt = Convert.FromBase64String(encryptedValue);
            // Step 5. Attempt to decrypt the string
            try
            {
                var decryptor = tdesAlgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hash provider services of any sensitive information
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }
            // Step 6. Return the decrypted string in UTF8 format
            var decryptedPassword = utf8.GetString(results);
            if (salt != null)
            {
                var index = decryptedPassword.IndexOf(salt, StringComparison.Ordinal);
                decryptedPassword = (index < 1) ? decryptedPassword.Remove(index, salt.Length) : null;
            }
            return decryptedPassword;
        }

        public string Encrypt(string value)
        {
            return EncryptTdes(value, Passphrase);
        }

        public string Decrypt(string value)
        {
            return DecryptTdes(value, Passphrase);
        }
    }
}
