using SOLID_Prac1.Interface;
namespace SOLID_Prac1.Services.Validator
{
    public class ValidatedReportDecorator : IReport
    {
        private readonly IReport _inner;
        private readonly IEnumerable<IReportValidator> _validators;
        private readonly IReport _report;
        private readonly IEnumerable<IFileValidator> _fileValidators;
        
        // 舊邏輯：用於內容字串驗證
        public ValidatedReportDecorator(IReport inner, IEnumerable<IReportValidator> validators)
        {
            Console.WriteLine("呼叫的是內容驗證 constructor！");
            _inner = inner;
            _validators = validators;
        }

        // 新邏輯：用於檔案驗證（IFileReport）
        public ValidatedReportDecorator(IFileReport report, IEnumerable<IFileValidator> fileValidators)
        {
            Console.WriteLine("呼叫的是檔案驗證 constructor！");
            _report = report;
            _fileValidators = fileValidators;
        }

        public string Generate()
        {
            if (_fileValidators != null)
            {
                Console.WriteLine("使用檔案驗證流程");
                //var file = (_report as IFileReport)?.File;
                var file = _report as IFileReport;

                if (file == null)
                {
                    Console.WriteLine("無法將 _report 轉型為 IFileReport");
                    throw new InvalidOperationException("報表不支援檔案驗證");
                }

                if (file == null)
                {
                    Console.WriteLine("IFileReport.File 為 null");
                    throw new InvalidOperationException("無檔案可供驗證");
                }

                Console.WriteLine($"檔案名稱: {file.File.FileName}");
                Console.WriteLine($"檔案大小: {file.File.Length}");


                foreach (var validator in _fileValidators)
                {
                    Console.WriteLine($" 執行檔案驗證器: {validator.GetType().Name}");
                    validator.Validate(file.File);
                }

                var result = _report!.Generate(); // _report 一定不為 null，因為 constructor 限定
                Console.WriteLine($"報表產生成功，內容為: {result}");
                return result;
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
