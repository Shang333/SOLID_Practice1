using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services.Report
{
    public class ReportGenerator
    {
        private readonly IReport _report;

        public ReportGenerator(IReport report)
        {
            _report = report;
        }

        public string GenerateReport()
        {
            return _report.Generate();
        }
    }
}
