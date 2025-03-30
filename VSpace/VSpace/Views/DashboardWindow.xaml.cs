using System.Windows;
using System.Windows.Controls;
using VSpace.Others;

namespace VSpace.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly BoincClientService _boincService;
        private BoincProject _selectedProject;

        public DashboardWindow()
        {
            InitializeComponent();
            _boincService = new BoincClientService();
            RefreshStatusAsync();
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectButton.IsEnabled = false;
                BoincStatusText.Text = "Connecting...";

                bool connected = await _boincService.ConnectAsync();
                if (connected)
                {
                    BoincStatusText.Text = "Connected";
                    ConnectButton.IsEnabled = false;
                    DisconnectButton.IsEnabled = true;
                    await RefreshProjectsAsync();
                }
                else
                {
                    BoincStatusText.Text = "Connection Failed";
                    ConnectButton.IsEnabled = true;
                    MessageBox.Show("Failed to connect to BOINC client. Please make sure BOINC is installed and running.", 
                                  "Connection Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                BoincStatusText.Text = "BOINC Not Found";
                ConnectButton.IsEnabled = true;
                MessageBox.Show(ex.Message, 
                              "BOINC Not Found", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await _boincService.DisconnectAsync();
            BoincStatusText.Text = "Disconnected";
            ConnectButton.IsEnabled = true;
            DisconnectButton.IsEnabled = false;
            ProjectsListView.ItemsSource = null;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshStatusAsync();
            if (_boincService.IsConnected)
            {
                await RefreshProjectsAsync();
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_boincService.IsConnected)
            {
                await _boincService.DisconnectAsync();
            }

            // Clear the stored GUID
            AppSettingsManager.UserGuid = string.Empty;

            // Show the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Close this window
            this.Close();
        }

        private async Task RefreshStatusAsync()
        {
            string status = await _boincService.GetClientStateAsync();
            BoincStatusText.Text = status;
        }

        private async Task RefreshProjectsAsync()
        {
            var projects = await _boincService.GetProjectsAsync();
            ProjectsListView.ItemsSource = projects;
        }

        private void ProjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProject = ProjectsListView.SelectedItem as BoincProject;
            StartProjectButton.IsEnabled = _selectedProject != null;
            StopProjectButton.IsEnabled = _selectedProject != null;
        }

        private async void StartProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProject == null)
                return;

            try
            {
                bool success = await _boincService.StartProjectAsync(_selectedProject.ProjectUrl);
                if (success)
                {
                    MessageBox.Show("Project started successfully.", 
                                  "Success", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                    await RefreshProjectsAsync();
                }
                else
                {
                    MessageBox.Show("Failed to start project.", 
                                  "Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting project: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private async void StopProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProject == null)
                return;

            try
            {
                bool success = await _boincService.StopProjectAsync(_selectedProject.ProjectUrl);
                if (success)
                {
                    MessageBox.Show("Project stopped successfully.", 
                                  "Success", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                    await RefreshProjectsAsync();
                }
                else
                {
                    MessageBox.Show("Failed to stop project.", 
                                  "Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping project: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }
    }
} 