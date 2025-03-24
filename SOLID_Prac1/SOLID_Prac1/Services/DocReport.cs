using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class DocReport : IReport
    {
        public string Generate()
        {
            return "This is a Word (.doc) Report.";
        }
    }
}
