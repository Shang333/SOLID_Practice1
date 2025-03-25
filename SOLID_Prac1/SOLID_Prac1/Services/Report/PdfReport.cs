using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services.Report
{
    public class PdfReport : IReport
    {
        public string Generate()
        {
            return "This is a PDF (.pdf) Report.";
        }
    }
}
