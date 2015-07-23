using System;
using System.Windows.Input;
using DynamicConfig.ConfigTray.ViewModel;

namespace DynamicConfig.ConfigTray.Commands
{
    internal class DeleteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (!(parameter is ConfigNode))
                return false;

            var n = parameter as ConfigNode;
            return !n.IsRoot() //can't delete root
                && n.Parent.NodeType == ConfigNodeType.ARRAY // only array elements can be deleted
                && (((ConfigListNode)n.Parent).Children.Count > 1); // can't delete the last child
        }

        public void Execute(object parameter)
        {
            ((ConfigNode)parameter).DeleteNode();
        }
    }
}