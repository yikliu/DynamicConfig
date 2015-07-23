// ported from https://github.com/Dynalon/JsonConfig/blob/master/JsonConfig/Config.cs
// Simplified to only keep the part that locates local json file. No Merger.cs needed.
// returns ExpandoObject, No ConfigObject involved.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DynamicConfig.ConfigTray.JSONConfig
{
    /// <summary>
    /// A Dynamic config loader
    /// </summary>
    internal class ExpandoConfig
    {
        public static string UserPath;
        public static ExpandoObject User;

        public static void LoadConfigFromFile()
        {
            string executionPath = AppDomain.CurrentDomain.BaseDirectory;
            const string configFileName = "config";

            var d = new DirectoryInfo(executionPath);
            FileInfo userConfig = (from FileInfo fi in d.GetFiles()
                                   where (fi.FullName.EndsWith(configFileName + ".json"))
                                   select fi).FirstOrDefault();
            if (userConfig != null)
            {
                UserPath = userConfig.FullName;
                User = ParseJsonToExpando(File.ReadAllText(userConfig.FullName));
            }
            else
            {
                var defaultConfig = CreateDefaulConfig.Create();
                UserPath = Path.Combine(d.FullName, "config.json");
                File.WriteAllText(UserPath, defaultConfig);
                User = ParseJsonToExpando(defaultConfig);
                userConfig = new FileInfo(UserPath);
            }


        }
        //returns ExpandoObject
        private static dynamic ParseJsonToExpando(string json)
        {
            string[] lines = json.Split(new[] { '\n' });
            // remove lines that start with a dash # character
            IEnumerable<string> filtered = from l in lines
                                           where !(Regex.IsMatch(l, @"^\s*#(.*)"))
                                           select l;

            string filtered_json = string.Join("\n", filtered);

            //yikliu: use my own json parser (Wrapper of JsonConvert)
            return JSONParser.Deserialize(filtered_json);
        }
    }
}