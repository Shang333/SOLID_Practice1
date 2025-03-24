using Microsoft.Extensions.Options;
using SOLID_Prac1.DTO;
using SOLID_Prac1.Interface;

public class LocalFileStorage : IFileStorage
{
    private readonly string _uploadPath;

    public LocalFileStorage(IOptions<UploadSettings> options)
    {
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), options.Value.UploadPath);
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);

        var fileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine(_uploadPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }
}
