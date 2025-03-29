using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VSpace.Models;

namespace VSpace.Others
{
    public class AppsettingManager
    {
        private static readonly string FolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VSpace");
        private static readonly string FilePath = Path.Combine(FolderPath, "appsettings.json");

        public static AppSettings? Load()
        {
            if (!File.Exists(FilePath))
                return new AppSettings();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json);
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                Directory.CreateDirectory(FolderPath);
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Clear()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}
