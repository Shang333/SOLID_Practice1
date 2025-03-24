using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class FileNameValidator : IFileValidator
    {
        public void Validate(IFormFile file)
        {
            var fileName = file.FileName;

            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\") || fileName.Contains("'"))
            {
                throw new InvalidOperationException("檔名包含不合法字元（.. / \\ '）");
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new InvalidOperationException("檔名包含系統不允許的字元。");
            }

            var cleanName = Path.GetFileName(fileName);
            if (fileName != cleanName)
            {
                throw new InvalidOperationException("檔名中包含不允許的路徑資訊。");
            }
        }
    }
}
