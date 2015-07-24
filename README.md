# DynamicConfig
===================

DynamicConfig is app.config replacement for accessing configuration data for .net applications. It has both a UI interface and programming interface so that both end users and developers can access configuration data.

Configuration data is loaded from a json file and can be accessed through dynamic object (without name checking).

### QuickStart 
- Reference Dynamic in your project. 
- Generate a json format config file (.conf or .json) in the path of executable file.
- Use Main() argument to either start exe to run service or start Config UI
    ```c#
    private static void Main(string[] args)
    {
    		ConfigDaemon.LoadConfig(); //Load values first
			if (args.Contains("-service")) //start service with arguments
    		{
    			var servicesToRun = new ServiceBase[] { new Program() };
    			Run(servicesToRun);
			}
		else //start UI
    		{
            ConfigDaemon.OnReloadTimerElapsed +=ConfigDaemonOnOnReloadTimerElapsed; 
            ConfigDaemon.StartWPFUIThread();
	    }
    }
    ```
- End users can edit config data through Config UI
- Developers can access config data by Using dynamic objects directly through ConfigDaemon object 

 
License
----

MIT


**Thanks for the freeware**

- [Dynalon/JsonConfig](https://github.com/Dynalon/JsonConfig)
- [MahApps.Metro](https://github.com/MahApps/MahApps.Metro)

