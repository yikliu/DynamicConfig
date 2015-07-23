using System.Collections.Generic;
using System.Windows;

namespace DynamicConfig.ConfigTray
{
    /// <summary>
    /// ConfigTray Application
    /// </summary>
    public partial class ConfigTrayApp : Application, ISingleInstanceApp
    {
        private static ConfigTrayApp theApp = null;

        private ConfigTrayApp() {}

        /// <summary>
        /// Get instance of application
        /// </summary>
        /// <returns></returns>
        public static ConfigTrayApp GetConfigTrayApp()
        {
            return theApp ?? (theApp = new ConfigTrayApp());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }
    }
}