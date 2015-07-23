using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;
using DynamicConfig.ConfigTray;

namespace DynamicConfig.ServiceExample
{
    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        void SampleMethod1();

        [OperationContract]
        void SampleMethod2();
    }

    public class Program : ServiceBase
    {
        public ServiceHost ServiceHost = null;

        public Program()
        {
            ServiceName = "Sample Service";
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ServiceHost?.Close();

                ConfigDaemon.LoadConfig();
                //get value from ConfigDaemon if service need some parameters to start
                int value = ConfigDaemon.Root.SvcLimit;

                //Start the actual service
                ServiceHost = new ServiceHost(new SampleServiceImp(value));
                var behavior = ServiceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                behavior.InstanceContextMode = InstanceContextMode.Single;

                ServiceHost.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (ServiceHost == null) return;
                ServiceHost.Close();
                ServiceHost = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                ConfigDaemon.LoadConfig(); //Load values first

                if (args.Contains("-service"))
                {
                    var servicesToRun = new ServiceBase[] { new Program() };
                    Run(servicesToRun);
                }
                else
                {
                    ConfigDaemon.OnReloadTimerElapsed += ConfigDaemonOnOnReloadTimerElapsed;
                    ConfigDaemon.StartWpfUiThread();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void ConfigDaemonOnOnReloadTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            //data has been reloaded, ConfigDaemon.Root now contains the newest data
        }
    }

    public class SampleServiceImp : ISampleService
    {
        private int internalValue;

        public SampleServiceImp(int init_value)
        {
            internalValue = init_value;
        }

        public int SomeValue
        {
            get { return internalValue; }
            set { internalValue = value; }
        }

        public void SampleMethod1()
        { }

        public void SampleMethod2()
        { }
    }
}