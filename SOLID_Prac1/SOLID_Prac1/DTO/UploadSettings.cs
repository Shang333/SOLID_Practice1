namespace SOLID_Prac1.DTO
{
    public class UploadSettings
    {
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public long MaxUploadSize { get; set; } = 5 * 1024 * 1024; // default 5MB
        public string UploadPath { get; set; } = "UploadedReports";
        public bool VirusScanEnabled { get; set; } = false;
    }
}
