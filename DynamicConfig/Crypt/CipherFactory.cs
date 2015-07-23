using System;
using System.Collections.Generic;

namespace DynamicConfig.ConfigTray.Crypt
{
    /// <summary>
    /// Enum of encryption method used
    /// </summary>
    public enum EncryptionMethod
    {
// ReSharper disable InconsistentNaming
        NONE,
// ReSharper restore InconsistentNaming
        
        /// <summary>
        /// Encryption Method: DPAPI Machine
        /// </summary>
// ReSharper disable InconsistentNaming
        DPAPI_MACHINE,
// ReSharper restore InconsistentNaming

        /// <summary>
        /// Encryption Method: DPAPI User
        /// </summary>
// ReSharper disable InconsistentNaming
        DPAPI_USER,
// ReSharper restore InconsistentNaming

        // TRIPLE_DES,
        // AES
    }

    internal static class CipherFactory
    {
        /// <summary>
        /// Use Lazy to do late initialization
        /// </summary>
        private static readonly Dictionary<EncryptionMethod, Lazy<ICipher>> Dict = new Dictionary<EncryptionMethod, Lazy<ICipher>>
        {
            {EncryptionMethod.DPAPI_MACHINE, new Lazy<ICipher>(()=> new DPAPIMachineCipher())},
            {EncryptionMethod.DPAPI_USER, new Lazy<ICipher>(()=> new DPAPIUserCipher())},
            
        };

        public static ICipher CreateCipher(EncryptionMethod encrypt)
        {
            return Dict[encrypt].Value;
        }
    }
}