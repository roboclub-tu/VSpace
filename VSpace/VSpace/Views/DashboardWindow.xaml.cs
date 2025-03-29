using System.Windows;
using VSpace.Others;

namespace VSpace.Views
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the stored GUID
            AppSettingsManager.UserGuid = string.Empty;

            // Show the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Close this window
            this.Close();
        }
    }
} 