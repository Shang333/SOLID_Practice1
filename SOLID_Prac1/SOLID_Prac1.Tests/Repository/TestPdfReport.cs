using Microsoft.AspNetCore.Http;
using SOLID_Prac1.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID_Prac1.Tests.Repository
{
    public class TestPdfReport : IFileReport
    {
        public bool WasGenerateCalled { get; private set; }
        public IFormFile File { get; set; }

        public string Generate()
        {
            Console.WriteLine("TestPdfReport.Generate() 被呼叫！");
            WasGenerateCalled = true;
            return "Fake Pdf Content";
        }
    }
}
