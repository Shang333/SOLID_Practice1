using SOLID_Prac1.Interface;
namespace SOLID_Prac1.Services.Validator
{
    public class ValidatedReportDecorator : IReport
    {
        private readonly IReport _inner;
        private readonly IEnumerable<IReportValidator> _validators;
        private readonly IReport _report;
        private readonly IEnumerable<IFileValidator> _fileValidators;

        public ValidatedReportDecorator(IReport inner, IEnumerable<IReportValidator> validators)
        {
            _inner = inner;
            _validators = validators;
        }

        // 新增的：用於檔案驗證的流程
        public ValidatedReportDecorator(IReport report, IEnumerable<IFileValidator> fileValidators)
        {
            _report = report;
            _fileValidators = fileValidators;
        }

        public string Generate()
        {
            if (_fileValidators != null)
            {
                Console.WriteLine("使用檔案驗證流程");
                var file = (_report as IFileReport)?.File;

                foreach (var validator in _fileValidators)
                {
                    Console.WriteLine($" 執行檔案驗證器: {validator.GetType().Name}");
                    validator.Validate(file);
                }

                return _report.Generate();
            }

            if (_validators != null)
            {
                Console.WriteLine("使用內容驗證流程");
                var content = _inner.Generate();

                foreach (var validator in _validators)
                {
                    Console.WriteLine($" 執行內容驗證器: {validator.GetType().Name}");
                    validator.Validate(content);
                }

                return content;
            }

            Console.WriteLine(" 沒有 validator，直接呼叫報表產生");
            return _report?.Generate() ?? _inner?.Generate();
        }

    }
}
