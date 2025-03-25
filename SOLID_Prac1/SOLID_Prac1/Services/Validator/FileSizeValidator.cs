using Microsoft.Extensions.Options;
using SOLID_Prac1.DTO;
using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services.Validator
{
    public class FileSizeValidator : IFileValidator
    {
        private readonly long _maxSize;

        public FileSizeValidator(IOptions<UploadSettings> options)
        {
            _maxSize = options.Value.MaxUploadSize;
        }

        public void Validate(IFormFile file)
        {
            if (file.Length > _maxSize)
            {
                throw new InvalidOperationException($"檔案大小不能超過 {_maxSize / 1024 / 1024} MB");
            }
        }
    }
}
