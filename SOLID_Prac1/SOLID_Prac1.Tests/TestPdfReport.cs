using SOLID_Prac1.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID_Prac1.Tests
{
    public class TestPdfReport : IReport
    {
        public bool WasGenerateCalled { get; private set; }

        public string Generate()
        {
            WasGenerateCalled = true;
            return "Fake Pdf Content";
        }
    }
}
