using System.Collections.Generic;
using System.Windows;

namespace DynamicConfig.ConfigTray
{
    public partial class ConfigTrayApp : Application, ISingleInstanceApp
    {
        private static ConfigTrayApp _theApp = null;

        private ConfigTrayApp() {}

        public static ConfigTrayApp GetConfigTrayApp()
        {
            return _theApp ?? (_theApp = new ConfigTrayApp());
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }
    }
}