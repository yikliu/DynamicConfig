using System;
using System.Collections.Generic;
using DynamicConfig.ConfigTray.Crypt;

namespace DynamicConfig.ConfigTray.ViewModel
{
    internal static class ConfigNodeFactory
    {
        public static ConfigNode CreateConfigObjectNode(dynamic dy, KeyType key, ConfigNode parent)
        {
            var dict = dy as IDictionary<string, object>;
            var isEncrypted = dict.ContainsKey("EncryptionMethod") 
                && dict.ContainsKey("Content") 
                && dict.Count == 2;
            if (isEncrypted)
            {
                var encrypt = dict["EncryptionMethod"];
                var em = (EncryptionMethod) Enum.Parse(typeof (EncryptionMethod), (string) encrypt);
                return new ConfigLeafNode(dict["Content"], key, parent, em);
            }
            return new ConfigObjectNode(dy, key, parent);
        }

        public static ConfigNode CreateConfigListNode(dynamic dy, KeyType key, ConfigNode parent)
        {
            return new ConfigListNode(dy, key, parent);
        }

        public static ConfigNode CreateConfigLeafNode(dynamic dy, KeyType key, ConfigNode parent)
        {
            return new ConfigLeafNode(dy, key, parent);
        }
    }
}