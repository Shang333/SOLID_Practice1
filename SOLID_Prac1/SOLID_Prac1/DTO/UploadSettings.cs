namespace SOLID_Prac1.DTO
{
    public class UploadSettings // 映射 appsettings.json 的 "UploadSettings" 區塊
    {
        public string[] AllowedExtensions { get; set; }
        public long MaxUploadSize { get; set; }
        public string UploadPath { get; set; }
        public bool VirusScanEnabled { get; set; }
    }
}
