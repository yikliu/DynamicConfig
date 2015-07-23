using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DynamicConfig.ConfigTray.Commands;
using DynamicConfig.ConfigTray.JSONConfig;
using DynamicConfig.ConfigTray.Util.ValidationRules;
using DynamicConfig.ConfigTray.ViewModel;
using DynamicConfig.ConfigTray.ViewModel.ConfigChangedEvent;

namespace DynamicConfig.ConfigTray
{
    partial class MainWindow : MetroWindow
    {
        private CreateCommand _createCommand;

        private DeleteCommand _deleteCommand;

        private ServiceChecker _serviceChecker;

        private ConfigNode _selectedNode;

        /// <summary>
        ///
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ConfigDaemon.UISynchronizationContext = SynchronizationContext.Current;

            ShowCloseButton = false;

            TitlebarHeight = 30;
            
            LoadConfigAfterWindowInit();//first time loading config

            MinimizeToTray.Enable(this);
        }

        private void LoadConfigAfterWindowInit()
        {
            if (!ConfigDaemon.LoadSuccess)
            {
                ConfigDaemon.LoadConfig();
            }
            
            DataContext = ConfigDaemon.GetCurrentDataContext();
            _createCommand = new CreateCommand();
            _deleteCommand = new DeleteCommand();

            FileLocation.Text = "Source: " + ExpandoConfig.UserPath;

            ConfigDaemon.OnUserConfigChanged += ConfigDaemonOnOnUserConfigChanged;
            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            MinimizeToTray.DisposeNotifyIcon();
        }

        private void ConfigDaemonOnOnUserConfigChanged(ConfigChangedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                LogBlock.Text = " Change: " + e.Description();
            }));
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _selectedNode = ConfigDaemon.Root;

            _serviceChecker = new ServiceChecker(ConfigDaemon.Root.ServiceName);
            
            this.WindowState = WindowState.Minimized; //minimize when loaded
        }

        private void UpdateControlDataContext(ConfigNode model)
        {
            KeyEdit.DataContext = model;
            ValueEdit.DataContext = model;
            TypeLabel.DataContext = model;
            encryptedCheckBox.DataContext = model;

            encryptButton.DataContext = model;
            
            rbMachine.DataContext = model;
            rbUser.DataContext = model;
            
            EncryptPanel.DataContext = model;
            if (model.NodeType == ConfigNodeType.LEAF)
            {
                Binding binding = BindingOperations.GetBinding(ValueEdit, TextBox.TextProperty);
                binding.ValidationRules.Clear();
                Type type = ((ConfigLeafNode)model).DataType;
                binding.ValidationRules.Add(ValidationRulesFactory.GetValidationRule(type));
            }
        }

        private void OnSelectedItemChanged(object sender, RoutedEventArgs e)
        {
            var model = (ConfigNode)JSONTreeView.SelectedItem;
            UpdateControlDataContext(model);
            _selectedNode = model;
        }

        private void CreateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _selectedNode != null && _createCommand.CanExecute(_selectedNode);
        }

        private void CreateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _createCommand.Execute(_selectedNode);
            LogBlock.Text = "Node Created.";
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _selectedNode != null && _deleteCommand.CanExecute(_selectedNode);
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _deleteCommand.Execute(_selectedNode);
            LogBlock.Text = "Node Deleted.";
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConfigDaemon.LoadConfig();
                
                DataContext = ConfigDaemon.GetCurrentDataContext();
                
                LogBlock.Text = "Reload Success!";
            }
            catch (Exception exp)
            {
                ConfigDaemon.PromptMessageBox("Failed to load JSON file: " + exp.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConfigDaemon.WriteBack();
                LogBlock.Text = "Save Success!";
            }
            catch (Exception exp)
            {
                ConfigDaemon.PromptMessageBox("Failed to write data to JSON file: " + exp.Message);
            }
        }

        private void EnableEncryptEdit_OnClick(object sender, RoutedEventArgs e)
        {
            BindingExpression be = ValueEdit.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
            
            var leafNode = _selectedNode as ConfigLeafNode;
            leafNode.Edit();
        }

        private void StartService_Click(object sender, RoutedEventArgs e)
        {
            _serviceChecker.Start();
        }

        private void StopService_Click(object sender, RoutedEventArgs e)
        {
            _serviceChecker.Stop();
        }

        private void RestartService_Click(object sender, RoutedEventArgs e)
        {
            _serviceChecker.Restart();
        }

        private void ISupportMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://isupportweb.inin.com");
        }

        private void ServiceNowMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://inincorp.service-now.com");
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            BindingExpression be = ValueEdit.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ss = ConfigDaemon.Root.Password;
            ConfigDaemon.PromptMessageBox(ss);
        }
        
    }
}