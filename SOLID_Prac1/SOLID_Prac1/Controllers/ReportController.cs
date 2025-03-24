using Microsoft.AspNetCore.Mvc;
using SOLID_Prac1.Interface;
using SOLID_Prac1.Services;

namespace SOLID_Prac1.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportFactory _reportFactory;

        public ReportController(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        /// <summary>
        /// 簡單範例：固定使用 PdfReport，檔案大小上限為 50 bytes
        /// </summary>
        [HttpGet("basic")]
        public IActionResult GetBasic()
        {
            try
            {
                var report = new PdfReport();
                var validator = new SizeValidator(50);
                var decorated = new ValidatedReportDecorator(report, new[] { validator });
                var generator = new ReportGenerator(decorated);

                return Ok(generator.GenerateReport());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 自訂範例：可指定報表類型與大小上限(pdf、doc、xlsx)
        /// </summary>
        [HttpGet("custom")]
        public IActionResult GetCustom([FromQuery] string type = "pdf", [FromQuery] int maxSize = 50)
        {
            try
            {
                var report = _reportFactory.Create(type, maxSize);
                var generator = new ReportGenerator(report);
                return Ok(generator.GenerateReport());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
