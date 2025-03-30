using System.Configuration;
using System.Data;
using System.Windows;

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

            

            //Window mainWindow;
            //if (Properties.Settings.Default.IsLoggedIn)
            //    mainWindow = new DashboardView();
            //else
            //    mainWindow = new LoginView();

            //mainWindow.Show();
        }
    }

}
