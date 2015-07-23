using System.Windows.Input;

namespace DynamicConfig.ConfigTray.Commands
{
    internal static class CustomCommands
    {
        public static readonly RoutedUICommand Create = new RoutedUICommand(
                "Create",
                "Create",
                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand Delete = new RoutedUICommand(
                "Delete",
                "Delete",
                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand Clone = new RoutedUICommand(
                "Clone",
                "Clone",
                typeof(CustomCommands)
            );
    }
}