using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID_Prac1.Interface
{
    public interface IFileReport : IReport
    {
        IFormFile File { get; }
    }
}
