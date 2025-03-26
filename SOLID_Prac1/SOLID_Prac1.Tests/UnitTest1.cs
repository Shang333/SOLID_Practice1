using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Options;
using SOLID_Prac1.DTO;
using Microsoft.Extensions.DependencyInjection;
using SOLID_Prac1.Interface;
using SOLID_Prac1.Services.Report;
using SOLID_Prac1.Services.Validator;
using System.ComponentModel.DataAnnotations;

namespace SOLID_Prac1.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        #region ReportClassTest
        [TestCase(typeof(PdfReport), "This is a PDF (.pdf) Report.")]
        [TestCase(typeof(DocReport), "This is a Word (.doc) Report.")]
        [TestCase(typeof(XlsxReport), "This is an Excel (.xlsx) Report.")]
        public void Report_Generate_ReturnsExpectedContent(Type reportType, string expected)
        {
            var report = (IReport)Activator.CreateInstance(reportType);
            var result = report.Generate();

            Console.WriteLine($"Report Type: {reportType.Name}");
            Console.WriteLine($"Expected: {expected}");
            Console.WriteLine($"Actual:   {result}");

            Assert.AreEqual(expected, result);
        }
        //[Test]
        //public void PdfReport_Generate_ReturnsExpectedContent()
        //{
        //    var report = new PdfReport();
        //    var result = report.Generate();
        //    Assert.AreEqual("This is a Pdf (.pdf) Report.", result);
        //}

        //[Test]
        //public void DocReport_Generate_ReturnsExpectedContent()
        //{
        //    var report = new DocReport();
        //    var result = report.Generate();
        //    Assert.AreEqual("This is a Word (.doc) Report.", result);
        //}

        //[Test]
        //public void XlsxReport_Generate_ReturnsExpectedContent()
        //{
        //    var report = new XlsxReport();
        //    var result = report.Generate();
        //    Assert.AreEqual("This is an Excel (.xlsx) Report.", result);
        //}
        #endregion

        #region FileNameTest
        [Test]
        public void FileNameValidator_WithValidFileName_DoesNotThrowException()
        {
            // Arrange           
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("hello.pdf");
            var validator = new FileNameValidator();

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                validator.Validate(mockFile.Object);
            });
        }

        [Test]
        public void FileNameValidator_WithValidFileName_ReturnsException1()
        {
            // Arrange           
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("te..st.pdf");
            var validator = new FileNameValidator();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                validator.Validate(mockFile.Object);
            });

            Assert.That(ex.Message, Is.EqualTo("檔名包含不合法字元（.. / \\ '）"));
        }
        #endregion

        #region ExtensionValidator
        [Test]
        public void ExtensionValidator_WithUnsupportedExtension_ThrowsException()
        {
            // Arrange - 模擬設定檔
            var uploadSettings = new UploadSettings
            {
                AllowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" }
            };

            var mockOptions = new Mock<IOptions<UploadSettings>>();
            mockOptions.Setup(opt => opt.Value).Returns(uploadSettings);

            var validator = new ExtensionValidator(mockOptions.Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.exe");

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                validator.Validate(mockFile.Object);
            });

            Assert.That(ex.Message, Is.EqualTo("副檔名 .exe 不被允許"));
        }

        [Test]
        public void ExtensionValidator_WithUnsupportedExtension_DoesNotThrowsException()
        {
            // Arrange - 模擬設定檔
            var uploadSettings = new UploadSettings
            {
                AllowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" }
            };

            var mockOptions = new Mock<IOptions<UploadSettings>>();
            mockOptions.Setup(opt => opt.Value).Returns(uploadSettings);

            var validator = new ExtensionValidator(mockOptions.Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.doc");

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                validator.Validate(mockFile.Object);
            });
        }

        // 多組副檔名測試
        [TestCase("test.pdf", true)]
        [TestCase("test.doc", true)]
        [TestCase("test.exe", false)]
        public void ExtensionValidator_TestCases(string fileName, bool shouldPass)
        {
            var settings = new UploadSettings
            {
                AllowedExtensions = new[] { ".pdf", ".doc", ".xlsx" }
            };

            var options = new Mock<IOptions<UploadSettings>>();
            options.Setup(o => o.Value).Returns(settings);

            var validator = new ExtensionValidator(options.Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);

            if (shouldPass)
            {
                Assert.DoesNotThrow(() => validator.Validate(mockFile.Object));
            }
            else
            {
                var ex = Assert.Throws<InvalidOperationException>(() =>
                {
                    validator.Validate(mockFile.Object);
                });
                Console.WriteLine($"驗證失敗訊息：{ex.Message}");
            }
        }
        #endregion

        #region ValidatorTest
        [Test]
        public void FileSizeValidator_WithAcceptableFileSize_DoesNotThrow()
        {
            // Arrange
            var settings = new UploadSettings { MaxUploadSize = 5 * 1024 * 1024 }; // 5MB
            var mockOptions = new Mock<IOptions<UploadSettings>>();
            mockOptions.Setup(o => o.Value).Returns(settings);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(2 * 1024 * 1024); // 2MB

            var validator = new FileSizeValidator(mockOptions.Object);

            // Act & Assert
            Assert.DoesNotThrow(() => validator.Validate(mockFile.Object));
        }

        [Test]
        public void FileSizeValidator_WithAcceptableFileSize_ThrowsException()
        {
            // Arrange
            var settings = new UploadSettings { MaxUploadSize = 5 * 1024 * 1024 }; // 5MB
            var mockOptions = new Mock<IOptions<UploadSettings>>();
            mockOptions.Setup(o => o.Value).Returns(settings);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(10 * 1024 * 1024); // 10MB

            var validator = new FileSizeValidator(mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                validator.Validate(mockFile.Object);
            });

            Assert.That(ex.Message, Is.EqualTo("檔案大小不能超過 5 MB")); // 錯誤訊息要完全一樣
        }

        [Test]
        public void FileExistenceValidator_WithNullFile_ThrowsException()
        {
            // Arrange
            var validator = new FileExistenceValidator(); 

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => {
                validator.Validate(null); // 檔案為null
            });

            Assert.That(ex.Message, Is.EqualTo("請選擇一個非空的檔案。"));
        }

        [Test]
        public void FileExistenceValidator_WithEmptyFile_ThrowsException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0); // 檔案為空

            var validator = new FileExistenceValidator();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => {
                validator.Validate(mockFile.Object);
            });

            Assert.That(ex.Message, Is.EqualTo("請選擇一個非空的檔案。"));
        }

        [Test]
        public void FileExistenceValidator_WithEmptyFile_DoesNotThrow()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1); // 檔案不為空

            var validator = new FileExistenceValidator();

            // Act & Assert
            Assert.DoesNotThrow(() => validator.Validate(mockFile.Object));
        }

        /// <summary>
        /// 整合/流程測試: 
        /// 1.假的報表 TestPdfReport2（模擬被包裝的業務邏輯） 2.檔案驗證器的流程（檢查檔案是否合法）
        /// 3.ValidatedReportDecorator 裝飾器（幫你管流程） 4.使用 TestCase(...) 資料驅動多種情境
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="size"></param>
        /// <param name="shouldPass"></param>
        [TestCase("valid.pdf", 1000, true)]
        [TestCase("bad..name.pdf", 1000, false)]
        [TestCase("valid.exe", 1000, false)]
        [TestCase("valid.pdf", 0, false)]
        public void Validators_FullValidationProcess_WorksAsExpected(string fileName, long size, bool shouldPass)
        {
            try
            {
                Console.WriteLine("測試開始");

                // Arrange: 模擬檔案
                var mockFile = new Mock<IFormFile>();
                mockFile.Setup(f => f.FileName).Returns(fileName);
                mockFile.Setup(f => f.Length).Returns(size);

                // 準備 validators
                var options = new Mock<IOptions<UploadSettings>>();
                options.Setup(o => o.Value).Returns(new UploadSettings
                {
                    AllowedExtensions = new[] { ".pdf", ".doc", ".xlsx" },
                    MaxUploadSize = 5 * 1024 * 1024 // 5MB
                });

                var validators = new List<IFileValidator>
                                {
                                    new FileExistenceValidator(),
                                    new FileNameValidator(),
                                    new ExtensionValidator(options.Object),
                                    new FileSizeValidator(options.Object)
                                };

                // 建立 fake report + decorator
                var fakeReport = new TestPdfReport2 { File = mockFile.Object };

                var decorator = new ValidatedReportDecorator((IFileReport)fakeReport, (IEnumerable<IFileValidator>)validators); // 指定為新邏輯的建構子

                // Act & Assert
                if (shouldPass)
                {
                    Console.WriteLine("shouldPass通過，開始執行");
                    var result = decorator.Generate();

                    Console.WriteLine($"decorator.Generate() 回傳: {result}");
                    Console.WriteLine($"WasGenerateCalled: {fakeReport.WasGenerateCalled}");
                    Assert.AreEqual("Fake Pdf Content", result);
                    Assert.IsTrue(fakeReport.WasGenerateCalled);
                }
                else
                {
                    var ex = Assert.Throws<InvalidOperationException>(() =>
                    {
                        decorator.Generate();
                    });

                    Console.WriteLine($"驗證錯誤訊息：{ex.Message}");
                    //Assert.IsFalse(fakeReport.WasGenerateCalled);
                    Assert.IsFalse(fakeReport.WasGenerateCalled, "Generate() 不應該被呼叫！");
                }

                Console.WriteLine("測試完成");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試發生例外：{ex.GetType().Name} - {ex?.Message} - {ex?.InnerException}");
                throw; // 保持原測試失敗行為
            }
        }
        #endregion

        #region ReportGeneratorTest
        [Test] // 確認 GenerateReport() 回傳的是 IReport.Generate() 的結果
        public void ReportGenerator_GenerateReport_ReturnsExpectedResult()
        {
            // Arrange
            var mockReport = new Mock<IReport>();
            mockReport.Setup(r => r.Generate()).Returns("報表內容");

            var generator = new ReportGenerator(mockReport.Object);

            // Act
            var result = generator.GenerateReport();

            // Assert
            Assert.AreEqual("報表內容", result);
        }

        [Test] // "行為驗證": 驗證 Generate() 是否被呼叫過一次
        public void ReportGenerator_GenerateReport_CallsReportGenerate()
        {
            // Arrange
            var mockReport = new Mock<IReport>();

            var generator = new ReportGenerator(mockReport.Object);

            // Act
            generator.GenerateReport();

            // Assert
            mockReport.Verify(r => r.Generate(), Times.Once);
        }

        [Test] // "例外錯誤": 當 IReport.Generate() 丟出 InvalidOperationException，ReportGenerator.GenerateReport() 也會丟出這個例外
        public void ReportGenerator_GenerateReport_WhenReportThrowsException_ShouldPropagate()
        {
            // Arrange
            var mockReport = new Mock<IReport>();
            mockReport
                .Setup(r => r.Generate())
                .Throws(new InvalidOperationException("報表產生失敗"));

            var generator = new ReportGenerator(mockReport.Object);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                generator.GenerateReport();
            });

            Assert.That(ex.Message, Is.EqualTo("報表產生失敗"));
        }
        #endregion

        #region ReportFactoryTest
        [Test]
        public void ReportFactory_WithPdfType_ReturnsValidatedReportDecorator()
        {
            // Arrange
            var mockPdfReport = new Mock<PdfReport>();

            var fakeProvider = new FakeServiceProvider();
            fakeProvider.Register<PdfReport>(mockPdfReport.Object);

            var factory = new ReportFactory(fakeProvider);

            // Act
            var result = factory.Create("pdf", 1024);

            // Assert
            Assert.IsInstanceOf<ValidatedReportDecorator>(result);
        }

        [Test] // ReportFactoryTest
        public void ReportFactory_WithUnsupportedType_ThrowsArgumentException()
        {
            // Arrange
            //var mockDocReport = new Mock<DocReport>(); // 因為要傳錯誤型別，所以不須註冊任何report

            var fakeProvider = new FakeServiceProvider();
            //fakeProvider.Register<DocReport>(mockDocReport.Object); // 因為要傳錯誤型別，所以不須註冊

            var factory = new ReportFactory(fakeProvider);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                factory.Create("zip", 1024);
            });

            Assert.That(ex.Message, Is.EqualTo("Unsupported report type: zip"));
        }

        [Test]
        public void ReportFactory_WithRegisteredTypeButMissingService_ThrowsInvalidOperationException()
        {
            // Arrange
            var fakeProvider = new FakeServiceProvider(); // 沒註冊 PdfReport (功能: 設定錯誤防呆、開發階段快速抓出注入錯誤)
            var factory = new ReportFactory(fakeProvider);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                factory.Create("pdf", 1024);
            });

            Assert.That(ex.Message, Does.Contain("has been registered"));
        }

        [Test]
        public void ReportFactory_WithPdfType_DoesNotThrow()
        {
            // Arrange
            var mockPdfReport = new Mock<PdfReport>();

            var fakeProvider = new FakeServiceProvider();
            fakeProvider.Register<PdfReport>(mockPdfReport.Object);

            var factory = new ReportFactory(fakeProvider);

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                factory.Create("pdf", 1024);
            });
        }

        [Test]
        public void ReportFactory_WithPdfType_ReturnsExpectedDecorator()
        {
            var provider = new FakeServiceProvider();
            provider.Register<PdfReport>(new PdfReport()); // 用真實 PdfReport（不要 mock）

            var factory = new ReportFactory(provider);
            var report = factory.Create("pdf", 1024);

            var result = report.Generate(); // 無法驗證是否有被呼叫，但可驗證沒例外、回傳結果

            Assert.IsInstanceOf<ValidatedReportDecorator>(report);
            Assert.That(result, Is.Not.Null); // 或其他可接受的驗證
        }

        #region 錯誤範例
        //public void ReportFactory_GeneratedReport_ShouldCallInnerReportGenerate()
        //{
        //    // Arange
        //    var mockPdfReport = new Mock<PdfReport>();
        //    mockPdfReport.Setup(r => r.Generate()).Returns("我是報表內容");

        //    var provider = new FakeServiceProvider();
        //    provider.Register<PdfReport>(mockPdfReport.Object);

        //    var factory = new ReportFactory(provider);
        //    var decoratedReport = factory.Create("pdf", 1024);

        //    // Act
        //    var result = decoratedReport.Generate(); // 會觸發 decorator → PdfReport，驗證內部是否被呼叫

        //    // Assert
        //    Assert.AreEqual("我是報表內容", result); // 回傳內容驗證
        //    mockPdfReport.Verify(r => r.Generate(), Times.Once); // 行為驗證，確認有被呼叫過一次
        //}
        //public void ReportFactory_GeneratedReport_ShouldCallGenerate_WhenReportIsExecuted()
        //{
        //    // Arrange
        //    var testReport = new TestPdfReport(); // 自訂可觀察版本
        //    var provider = new FakeServiceProvider();
        //    provider.Register<PdfReport>((PdfReport)testReport); // 型別對準

        //    var factory = new ReportFactory(provider);
        //    var decoratedReport = factory.Create("pdf", 1024);

        //    // Act
        //    var result = decoratedReport.Generate();

        //    // Assert
        //    Assert.AreEqual("Fake Pdf Content", result);
        //    Assert.IsTrue(testReport.WasGenerateCalled);
        //}
        #endregion
        
        #endregion
    }
}