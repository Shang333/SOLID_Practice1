using SOLID_Prac1.Interface;
using System.Text;

namespace SOLID_Prac1.Services.Validator
{
    public class SizeValidator : IReportValidator
    {
        private readonly int _maxSize;

        public SizeValidator(int maxSize)
        {
            _maxSize = maxSize;
        }

        public void Validate(string content)
        {
            var size = Encoding.UTF8.GetByteCount(content);
            if (size > _maxSize)
            {
                throw new InvalidOperationException($"Report exceeds {_maxSize} bytes.");
            }
        }
    }
}
