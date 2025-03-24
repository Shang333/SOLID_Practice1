using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class ExtensionValidator : IFileValidator
    {
        private readonly string[] _allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" };

        public void Validate(IFormFile file)
        {
            var fileExt = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExt))
            {
                throw new InvalidOperationException($"不支援的檔案格式：{fileExt}");
            }
        }
    }
}
