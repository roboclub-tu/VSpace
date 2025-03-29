using System.Configuration;
using System.Data;
using System.Windows;
using VSpace.Others;
using VSpace.Views;

namespace VSpace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Only start MainWindow if there's no saved GUID
            if (string.IsNullOrEmpty(AppSettingsManager.UserGuid))
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                // If GUID exists, start DashboardWindow
                var dashboardWindow = new DashboardWindow();
                dashboardWindow.Show();
            }
        }
    }
}
