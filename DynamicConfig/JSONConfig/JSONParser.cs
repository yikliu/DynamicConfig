using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DynamicConfig.ConfigTray.JSONConfig
{
    internal static class JSONParser
    {
        public delegate void JSONParsingErrorHandler(string msg);

        public static event JSONParsingErrorHandler OnJSONParsingError;

        public static ExpandoObject Deserialize(string json_string)
        {
            var setting = new JsonSerializerSettings();
            setting.Error += delegate(object sender, ErrorEventArgs args)
            {
                if (args.CurrentObject == args.ErrorContext.OriginalObject)
                {
                    args.ErrorContext.Handled = true;
                    if (OnJSONParsingError != null)
                    {
                        OnJSONParsingError(args.ErrorContext.Error.Message);
                    }
                }
            };
            return JsonConvert.DeserializeObject<ExpandoObject>(json_string, setting);
        }
    }
}