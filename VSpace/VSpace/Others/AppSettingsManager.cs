using System.Configuration;

namespace VSpace.Others
{
    public static class AppSettingsManager
    {
        private static Configuration _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static string UserGuid
        {
            get
            {
                return _config.AppSettings.Settings["UserGuid"]?.Value ?? string.Empty;
            }
            set
            {
                if (_config.AppSettings.Settings["UserGuid"] == null)
                {
                    _config.AppSettings.Settings.Add("UserGuid", value);
                }
                else
                {
                    _config.AppSettings.Settings["UserGuid"].Value = value;
                }
                _config.Save(ConfigurationSaveMode.Modified);
            }
        }
    }
} 