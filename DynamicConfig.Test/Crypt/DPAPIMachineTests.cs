using DynamicConfig.ConfigTray.Crypt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicConfig.Test.Crypt
{
    [TestClass()]
    public class DPAPIMachineTests
    {
        [TestMethod()]
        public void EncryptAndDecryptMatchTest()
        {
            var Encryptor = new DPAPIMachineCipher();
            var toEncrypt = "Donald E. Knuth";
            var cipherText = Encryptor.EncryptValue(toEncrypt);
            var decrypted = Encryptor.DecryptValue(cipherText);
            Assert.AreEqual(toEncrypt, decrypted);
        }
    }
}