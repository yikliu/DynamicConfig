using System;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using DynamicConfig.ConfigTray.JSONConfig;
using DynamicConfig.ConfigTray.ViewModel;
using DynamicConfig.ConfigTray.ViewModel.ConfigChangedEvent;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace DynamicConfig.ConfigTray
{
    /// <summary>
    /// Class ConfigDaemon.
    /// </summary>
    public class ConfigDaemon : DynamicObject
    {
        /// <summary>
        /// Indicating if loading process is success
        /// </summary>
        public static bool LoadSuccess = false;

        /// <summary>
        /// The data context for first generation (trivial)
        /// </summary>
        private static RootModel _context;

        /// <summary>
        /// The config data stored in ConfigObject
        /// </summary>
        private static dynamic _currentConfig;

        /// <summary>
        /// The root of config data stored
        /// </summary>
        private static ConfigNode _root;

        /// <summary>
        /// The reload timer interval
        /// </summary>
#if DEBUG
        private const int DefaultReloadTimerInterval = 3;
#else
        private const int DefaultReloadTimerInterval = 30; //30 seconds reload time
#endif

        /// <summary>
        /// The reload timer
        /// </summary>
        private static Timer _reloadTimer;

        private static SynchronizationContext _uiContext;

        /// <summary>
        /// Delegate UserConfigChangedHandler
        /// </summary>
        /// <param name="e">The <see cref="ConfigChangedEventArgs" /> instance containing the event data.</param>
        internal delegate void UserConfigChangedHandler(ConfigChangedEventArgs e);

        /// <summary>
        /// Occurs when reload timer is elapsed
        /// </summary>
        public static event ElapsedEventHandler OnReloadTimerElapsed;

        /// <summary>
        /// Occurs when there change occurred in config tree, internal use only
        /// </summary>
        internal static event UserConfigChangedHandler OnUserConfigChanged;

        /// <summary>
        /// Gets the root config in DynamicObject.
        /// </summary>
        /// <value>The root.</value>
        public static dynamic Root
        {
            get { return _root; }
        }

        public static ExpandoObject RootAsExpando
        {
            get
            {
                return _root.ConvertToExpando();
            }
        }

        /// <summary>
        /// load config data from source
        /// </summary>
        public static void LoadConfig()
        {
            LoadSuccess = false;

            JSONParser.OnJSONParsingError += delegate(string msg)
            {
                var errorMsg = "There is a format error in .json file: \n\n" + msg +
                              "\n\n Please format the configuration file in correct JSON format and try again.";

                LoadSuccess = false;
                PromptMessageBox(errorMsg);
            };

            try
            {
                ExpandoConfig.LoadConfigFromFile();
                _currentConfig = ExpandoConfig.User;

                if (_currentConfig == null || _currentConfig is NullExceptionPreventer)
                {
                    var errorMsg = @"Failed to load configurations";
                    PromptMessageBox(errorMsg);
                    LoadSuccess = false;
                    return;
                }

                _root = ConfigNodeFactory.CreateConfigObjectNode(_currentConfig, new StringKey("RootConfig"), null);
                _context = new RootModel(_root);
                _root.IsSelected = true;
                _root.SavePropertyChangeHandler(ConfigNodePropertyChanged);

                if (_reloadTimer == null || _reloadTimer.Enabled == false)
                {
                    _reloadTimer = new Timer(DefaultReloadTimerInterval * 1000) { Enabled = true };
                    _reloadTimer.Elapsed += ReloadTimerOnElapsed;
                }

                LoadSuccess = true;
            }
            catch (Exception e)
            {
                LoadSuccess = false;
            }
        }

        /// <summary>
        /// Start WPF Window Thread
        /// </summary>
        public static void StartWPFUIThread()
        {
            var newWindowThread = new Thread(StartConfigTray);
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.Start();
        }

        /// <summary>
        /// Get SynchronizationContext from UI Thread
        /// </summary>
        /// <remarks>Don't call this getter before or immediately after StartWPFUIThread(), as the UI starts in a different thread and takes some time to load the SynchronizationContext. 
        /// </remarks>
        public static SynchronizationContext UISynchronizationContext
        {
            get { return _uiContext; }
            internal set { _uiContext = value; }
        }

        internal static RootModel GetCurrentDataContext()
        {
            return _context;
        }

        internal static void PromptMessageBox(string msg)
        {
            var result = MessageBox.Show(msg, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                LoadSuccess = false;
            }
        }

        internal static void WriteBack()
        {
            ExpandoObject toBeWritten = _root.ConvertToExpando();
            var output = JsonConvert.SerializeObject(toBeWritten, Formatting.Indented);
            using (var sw = new StreamWriter(ExpandoConfig.UserPath, false)) //"false" to enforce a rewrite
            {
                sw.Write(output);
            }
        }

        private static void ConfigNodePropertyChanged(object sender, ConfigChangedEventArgs evtArgs)
        {
            WriteBack();

            //notify consuming applications
            if (OnUserConfigChanged != null)
            {
                OnUserConfigChanged(evtArgs);
            }
        }

        private static void ReloadTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            //LoadConfig();
            if (LoadSuccess && OnReloadTimerElapsed != null)
            {
                OnReloadTimerElapsed(sender, e);
            }
        }

        private static void StartConfigTray()
        {
            string Unique = "SINGLEINSTANCESECRETE";
            if (SingleInstance<ConfigTrayApp>.InitializeAsFirstInstance(Unique))
            {
                if (Application.Current == null)
                {
                    var app = ConfigTrayApp.GetConfigTrayApp();
                    app.InitializeComponent();
                    app.Run();
                }

                // Allow single instance code to perform cleanup operations
                SingleInstance<ConfigTrayApp>.Cleanup();
            }
        }
    }
}