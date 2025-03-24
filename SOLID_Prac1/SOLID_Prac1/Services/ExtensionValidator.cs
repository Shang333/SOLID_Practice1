using Microsoft.Extensions.Options;
using SOLID_Prac1.DTO;
using SOLID_Prac1.Interface;

public class ExtensionValidator : IFileValidator
{
    private readonly string[] _allowedExtensions;

    public ExtensionValidator(IOptions<UploadSettings> options)
    {
        _allowedExtensions = options.Value.AllowedExtensions;
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