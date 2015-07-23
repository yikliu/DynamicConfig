using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using DynamicConfig.ConfigTray.Properties;

namespace DynamicConfig.ConfigTray.ViewModel
{
    internal sealed class ServiceChecker : INotifyPropertyChanged, IDisposable
    {
        private ServiceController _serviceController;

        private readonly bool _isServiceExist = false;

        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(30 * 1000);

        public ServiceChecker(string serviceName)
        {
            if (DoesServiceExist(serviceName))
            {
                _serviceController = new ServiceController(serviceName);
                if (_serviceController != null)
                {
                    _isServiceExist = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string IsServiceButtonVisible
        {
            get
            {
                if (!_isServiceExist)
                {
                    if (_serviceController == null) return "Collapsed";
                    MessageBox.Show("Service: " + _serviceController.ServiceName + " does not exist.");
                }
                return _isServiceExist ? "Visible" : "Collapsed";
            }
        }

        public bool IsRunning
        {
            get
            {
                return (_serviceController != null) && (_serviceController.Status == ServiceControllerStatus.Running);
            }
        }

        public void Start()
        {
            try
            {
                if (_serviceController.Status != ServiceControllerStatus.Running)
                {
                    _serviceController.Start();
                    _serviceController.WaitForStatus(ServiceControllerStatus.Running, _timeout);
                    OnPropertyChanged("IsRunning");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                MessageBox.Show("Failed to Start Service: " + _serviceController.ServiceName + " " + e.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (_serviceController.Status != ServiceControllerStatus.Stopped)
                {
                    _serviceController.Stop();
                    _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    OnPropertyChanged("IsRunning");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                MessageBox.Show("Failed to Stop Service: " + _serviceController.ServiceName + " " + e.Message);
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        //timeout 30 seconds

        private bool DoesServiceExist(string serviceName)
        {
            return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(serviceName));
        }

        public void Dispose()
        {
            if (_serviceController != null)
            {
                _serviceController.Dispose();
                _serviceController = null;
            }
        }

        ~ServiceChecker()
        {
            Dispose();
        }
    }
}