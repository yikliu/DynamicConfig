using System;
using System.Windows.Input;
using DynamicConfig.ConfigTray.ViewModel;

namespace DynamicConfig.ConfigTray.Commands
{
    internal class CreateCommand : ICommand
    {
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
        
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