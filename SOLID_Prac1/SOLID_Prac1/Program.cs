using SOLID_Prac1.DTO;
using SOLID_Prac1.Interface;
using SOLID_Prac1.Services.Report;
using SOLID_Prac1.Services.Validator;

var builder = WebApplication.CreateBuilder(args);

// �j�w UploadSettings �]�w�Ϭq
builder.Services.Configure<UploadSettings>(
    builder.Configuration.GetSection("UploadSettings"));

// Add services to the container.
// ���U�����l����
builder.Services.AddTransient<PdfReport>();
builder.Services.AddTransient<DocReport>();
builder.Services.AddTransient<XlsxReport>();

// ���Ҿ�
builder.Services.AddTransient<IFileValidator, FileExistenceValidator>();
builder.Services.AddTransient<IFileValidator, FileSizeValidator>();      // �z�L IOptions Ū�� MaxUploadSize
builder.Services.AddTransient<IFileValidator, ExtensionValidator>();     // �z�L IOptions Ū�� AllowedExtensions
builder.Services.AddTransient<IFileValidator, FileNameValidator>();

// �x�s��@
builder.Services.AddTransient<IFileStorage, LocalFileStorage>();

// ����u�t
builder.Services.AddSingleton<IReportFactory, ReportFactory>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
