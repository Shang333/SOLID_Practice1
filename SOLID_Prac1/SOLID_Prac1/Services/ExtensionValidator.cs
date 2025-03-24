using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class ExtensionValidator : IFileValidator
    {
        private readonly string[] _allowedExtensions;

        public ExtensionValidator(string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        public void Validate(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
            {
                throw new InvalidOperationException($"副檔名 {ext} 不被允許");
            }
        }
    }

}
