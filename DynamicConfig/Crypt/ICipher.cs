namespace DynamicConfig.ConfigTray.Crypt
{
    internal interface ICipher
    {
        string EncryptValue(string value);

        string DecryptValue(string value); 
    }
}