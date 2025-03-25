using SOLID_Prac1.Interface;
using SOLID_Prac1.Services.Validator;

namespace SOLID_Prac1.Services.Report
{
    public class ReportFactory : IReportFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ReportFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IReport Create(string type, int maxSize)
        {
            IReport report = type.ToLower() switch
            {
                "pdf" => _serviceProvider.GetRequiredService<PdfReport>(),
                "doc" => _serviceProvider.GetRequiredService<DocReport>(),
                "xlsx" => _serviceProvider.GetRequiredService<XlsxReport>(),
                _ => throw new ArgumentException($"Unsupported report type: {type}")
            };

            // 動態建立驗證器
            var validator = new SizeValidator(maxSize);
            return new ValidatedReportDecorator(report, new[] { validator });
        }
    }
}
