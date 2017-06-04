using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Valil.Chess.Model;

namespace Valil.Chess.WinFX
{
    public partial class MainPage : Page
    {
        private void GoTo2DClick(object sender, EventArgs e)
        {
            // navigate to chess 2D page
            NavigationService.GetNavigationService(this).Navigate(new Uri("Chess2DPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void GoTo3DClick(object sender, EventArgs e)
        {
            // navigate to chess 3D page
            NavigationService.GetNavigationService(this).Navigate(new Uri("Chess3DPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void GoToSettingsClick(object sender, EventArgs e)
        {
            // navigate to settings page
            NavigationService.GetNavigationService(this).Navigate(new Uri("SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}