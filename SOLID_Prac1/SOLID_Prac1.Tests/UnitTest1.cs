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

            Assert.That(ex.Message, Is.EqualTo("�ɦW�]�t���X�k�r���].. / \\ '�^"));
        }

        [Test]
        public void ExtensionValidator_WithUnsupportedExtension_ThrowsException()
        {
            // Arrange - �����]�w��
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

            Assert.That(ex.Message, Is.EqualTo("���ɦW .exe ���Q���\"));
        }

        [Test]
        public void ExtensionValidator_WithUnsupportedExtension_DoesNotThrowsException()
        {
            // Arrange - �����]�w��
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

            Assert.That(ex.Message, Is.EqualTo("�ɮפj�p����W�L 5 MB")); // ���~�T���n�����@��
        }

        [Test]
        public void FileExistenceValidator_WithNullFile_ThrowsException()
        {
            // Arrange
            var validator = new FileExistenceValidator(); 

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => {
                validator.Validate(null); // �ɮ׬�null
            });

            Assert.That(ex.Message, Is.EqualTo("�п�ܤ@�ӫD�Ū��ɮסC"));
        }

        [Test]
        public void FileExistenceValidator_WithEmptyFile_ThrowsException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0); // �ɮ׬���

            var validator = new FileExistenceValidator();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => {
                validator.Validate(mockFile.Object);
            });

            Assert.That(ex.Message, Is.EqualTo("�п�ܤ@�ӫD�Ū��ɮסC"));
        }

        [Test]
        public void FileExistenceValidator_WithEmptyFile_DoesNotThrow()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1); // �ɮפ�����

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
            //var mockDocReport = new Mock<DocReport>(); // �]���n�ǿ��~���O�A�ҥH�������U����report

            var fakeProvider = new FakeServiceProvider();
            //fakeProvider.Register<DocReport>(mockDocReport.Object); // �]���n�ǿ��~���O�A�ҥH�������U
   
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
            var fakeProvider = new FakeServiceProvider(); // �S���U PdfReport (�\��: �]�w���~���b�B�}�o���q�ֳt��X�`�J���~)
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
        [Test] // �T�{ GenerateReport() �^�Ǫ��O IReport.Generate() �����G
        public void ReportGenerator_GenerateReport_ReturnsExpectedResult()
        {
            // Arrange
            var mockReport = new Mock<IReport>();
            mockReport.Setup(r => r.Generate()).Returns("�����e");

            var generator = new ReportGenerator(mockReport.Object);

            // Act
            var result = generator.GenerateReport();

            // Assert
            Assert.AreEqual("�����e", result);
        }

        [Test] // "�欰����": ���� Generate() �O�_�Q�I�s�L�@��
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

        [Test] // "�ҥ~���~": �� IReport.Generate() ��X InvalidOperationException�AReportGenerator.GenerateReport() �]�|��X�o�Өҥ~
        public void ReportGenerator_GenerateReport_WhenReportThrowsException_ShouldPropagate()
        {
            // Arrange
            var mockReport = new Mock<IReport>();
            mockReport
                .Setup(r => r.Generate())
                .Throws(new InvalidOperationException("�����ͥ���"));

            var generator = new ReportGenerator(mockReport.Object);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                generator.GenerateReport();
            });

            Assert.That(ex.Message, Is.EqualTo("�����ͥ���"));
        }
        #endregion
    }
}