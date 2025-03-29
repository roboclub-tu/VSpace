using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Text.Json;
using VSpace.Others;
using VSpace.Views;

namespace VSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://add.com/validate-guid";

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();

            // Check if there's already a saved GUID
            if (!string.IsNullOrEmpty(AppSettingsManager.UserGuid))
            {
                this.Close();
            }
        }

        private void GuidTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string text = textBox.Text;
            
            // Only allow alphanumeric characters
            if (text.Length > 0)
            {
                text = new string(text.Where(c => char.IsLetterOrDigit(c)).ToArray());
                if (text != textBox.Text)
                {
                    textBox.Text = text;
                    textBox.CaretIndex = text.Length;
                }
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuidTextBox.Text.Length != 20)
            {
                MessageBox.Show("Please enter exactly 20 characters for the GUID.", 
                              "Invalid GUID Length", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            try
            {
                SubmitButton.IsEnabled = false;
                var requestData = new { guid = GuidTextBox.Text };
                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<ValidationResponse>(responseContent);

                if (responseData.message)
                {
                    // Save GUID using AppSettingsManager
                    AppSettingsManager.UserGuid = GuidTextBox.Text;

                    // Show dashboard window
                    var dashboardWindow = new DashboardWindow();
                    dashboardWindow.Show();

                    // Close this window
                    this.Close();
                }
                else
                {
                    MessageBox.Show("The user GUID is not recognized", 
                                  "Validation Failed", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}", 
                              "Connection Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
            finally
            {
                SubmitButton.IsEnabled = true;
            }
        }

        private class ValidationResponse
        {
            public bool message { get; set; }
        }
    }
}