using System;
using System.Windows.Input;
using DynamicConfig.ConfigTray.ViewModel;

namespace DynamicConfig.ConfigTray.Commands
{
    internal class CreateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
        {
            return (parameter as ConfigNode)?.NodeType == ConfigNodeType.ARRAY;
        }

        public void Execute(object parameter)
        {
            ((ConfigNode)parameter).CreateNode();
        }
    }
}