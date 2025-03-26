using Microsoft.AspNetCore.Http;
using SOLID_Prac1.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID_Prac1.Tests
{
    public class TestPdfReport2 : IFileReport
    {
        public bool WasGenerateCalled { get; private set; }
        public IFormFile File { get; set; }

        public string Generate()
        {
            WasGenerateCalled = true;
            Console.WriteLine("TestPdfReport2.Generate() 被呼叫！");
            return "Fake Pdf Content";
        }
    }
}
