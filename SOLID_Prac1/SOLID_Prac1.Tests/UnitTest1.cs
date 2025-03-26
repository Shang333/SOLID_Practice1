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

            Assert.That(ex.Message, Is.EqualTo("�ɦW�]�t���X�k�r���].. / \\ '�^"));
        }
        #endregion

        #region ExtensionValidator
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

        // �h�հ��ɦW����
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
                Console.WriteLine($"���ҥ��ѰT���G{ex.Message}");
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

        /// <summary>
        /// ��X/�y�{����: 
        /// 1.�������� TestPdfReport2�]�����Q�]�˪��~���޿�^ 2.�ɮ����Ҿ����y�{�]�ˬd�ɮ׬O�_�X�k�^
        /// 3.ValidatedReportDecorator �˹����]���A�ެy�{�^ 4.�ϥ� TestCase(...) ����X�ʦh�ر���
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
                Console.WriteLine("���ն}�l");

                // Arrange: �����ɮ�
                var mockFile = new Mock<IFormFile>();
                mockFile.Setup(f => f.FileName).Returns(fileName);
                mockFile.Setup(f => f.Length).Returns(size);

                // �ǳ� validators
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

                // �إ� fake report + decorator
                var fakeReport = new TestPdfReport2 { File = mockFile.Object };

                var decorator = new ValidatedReportDecorator((IFileReport)fakeReport, (IEnumerable<IFileValidator>)validators); // ���w���s�޿誺�غc�l

                // Act & Assert
                if (shouldPass)
                {
                    Console.WriteLine("shouldPass�q�L�A�}�l����");
                    var result = decorator.Generate();

                    Console.WriteLine($"decorator.Generate() �^��: {result}");
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

                    Console.WriteLine($"���ҿ��~�T���G{ex.Message}");
                    //Assert.IsFalse(fakeReport.WasGenerateCalled);
                    Assert.IsFalse(fakeReport.WasGenerateCalled, "Generate() �����ӳQ�I�s�I");
                }

                Console.WriteLine("���է���");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"���յo�ͨҥ~�G{ex.GetType().Name} - {ex?.Message} - {ex?.InnerException}");
                throw; // �O������ե��Ѧ欰
            }
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
            provider.Register<PdfReport>(new PdfReport()); // �ίu�� PdfReport�]���n mock�^

            var factory = new ReportFactory(provider);
            var report = factory.Create("pdf", 1024);

            var result = report.Generate(); // �L�k���ҬO�_���Q�I�s�A���i���ҨS�ҥ~�B�^�ǵ��G

            Assert.IsInstanceOf<ValidatedReportDecorator>(report);
            Assert.That(result, Is.Not.Null); // �Ψ�L�i����������
        }

        #region ���~�d��
        //public void ReportFactory_GeneratedReport_ShouldCallInnerReportGenerate()
        //{
        //    // Arange
        //    var mockPdfReport = new Mock<PdfReport>();
        //    mockPdfReport.Setup(r => r.Generate()).Returns("�ڬO�����e");

        //    var provider = new FakeServiceProvider();
        //    provider.Register<PdfReport>(mockPdfReport.Object);

        //    var factory = new ReportFactory(provider);
        //    var decoratedReport = factory.Create("pdf", 1024);

        //    // Act
        //    var result = decoratedReport.Generate(); // �|Ĳ�o decorator �� PdfReport�A���Ҥ����O�_�Q�I�s

        //    // Assert
        //    Assert.AreEqual("�ڬO�����e", result); // �^�Ǥ��e����
        //    mockPdfReport.Verify(r => r.Generate(), Times.Once); // �欰���ҡA�T�{���Q�I�s�L�@��
        //}
        //public void ReportFactory_GeneratedReport_ShouldCallGenerate_WhenReportIsExecuted()
        //{
        //    // Arrange
        //    var testReport = new TestPdfReport(); // �ۭq�i�[���
        //    var provider = new FakeServiceProvider();
        //    provider.Register<PdfReport>((PdfReport)testReport); // ���O���

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