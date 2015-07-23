namespace DynamicConfig.ConfigTray.JSONConfig
{
    internal static class CreateDefaulConfig
    {
        private static string content = @"{
          'ConfigJSONNotFound' : 'This is shown because Config cannot find config.json under the executive path, click Save will create one for you. Open that file and fill in your configuration and restart DynamicConfig.'
        }";

        public static string Create()
        {
            return content;
        }
    }
}