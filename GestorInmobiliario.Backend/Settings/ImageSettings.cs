// Settings/ImageSettings.cs
namespace GestorInmobiliario.Backend.Settings
{
    public class ImageSettings
    {
        public string UploadPath { get; set; } = string.Empty;
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public int MaxFileSizeMB { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
    }
}