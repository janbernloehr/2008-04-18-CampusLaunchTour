using System;
using System.Windows.Input;
using System.Windows.Controls;

namespace Valil.Chess.WinFX
{
    public static class CustomCommands
    {
        public static RoutedCommand First =
            new RoutedCommand(
            "First",
            typeof(Page));

        public static RoutedCommand Previous = 
            new RoutedCommand(
            "Previous", 
            typeof(Page));

        public static RoutedCommand Next =
            new RoutedCommand(
            "Next",
            typeof(Page));

        public static RoutedCommand Last =
            new RoutedCommand(
            "Last",
            typeof(Page));

        public static RoutedCommand Move =
            new RoutedCommand(
            "Move",
            typeof(Page));

        public static RoutedCommand Rotate =
            new RoutedCommand(
            "Rotate",
            typeof(Page));

        public static RoutedCommand Select =
            new RoutedCommand(
            "Select",
            typeof(Page));
    }
}
