using Microsoft.AspNetCore.Http;
using SOLID_Prac1.Services;
using Moq;
using Microsoft.Extensions.Options;
using SOLID_Prac1.DTO;
using Microsoft.Extensions.DependencyInjection;
using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
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

        [Test]
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
    }
}