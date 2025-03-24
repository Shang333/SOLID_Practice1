using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedReports");

        public async Task<string> SaveAsync(IFormFile file)
        {
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            var filePath = Path.Combine(_uploadPath, Path.GetFileName(file.FileName));
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath;
        }
    }
}
