using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services.Validator
{
    public class FileExistenceValidator : IFileValidator
    {
        public void Validate(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("請選擇一個非空的檔案。");
            }
        }
    }
}
