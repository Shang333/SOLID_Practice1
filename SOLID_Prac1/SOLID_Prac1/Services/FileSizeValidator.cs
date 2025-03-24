using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class FileSizeValidator : IFileValidator
    {
        private readonly long _maxBytes;

        public FileSizeValidator(long maxBytes)
        {
            _maxBytes = maxBytes;
        }

        public void Validate(IFormFile file)
        {
            if (file.Length > _maxBytes)
            {
                throw new InvalidOperationException($"檔案大小不能超過 {_maxBytes / 1024 / 1024} MB");
            }
        }
    }
}
