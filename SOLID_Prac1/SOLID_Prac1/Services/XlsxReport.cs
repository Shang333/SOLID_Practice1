using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class XlsxReport : IReport
    {
        public string Generate()
        {
            return "This is an Excel (.xlsx) Report.";
        }
    }
}
