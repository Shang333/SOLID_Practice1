using SOLID_Prac1.DTO;
using SOLID_Prac1.Interface;
using SOLID_Prac1.Services.Report;
using SOLID_Prac1.Services.Validator;

var builder = WebApplication.CreateBuilder(args);

// 綁定 UploadSettings 設定區段
builder.Services.Configure<UploadSettings>(
    builder.Configuration.GetSection("UploadSettings"));

// Add services to the container.
// 註冊報表原始本體
builder.Services.AddTransient<PdfReport>();
builder.Services.AddTransient<DocReport>();
builder.Services.AddTransient<XlsxReport>();

// 驗證器
builder.Services.AddTransient<IFileValidator, FileExistenceValidator>();
builder.Services.AddTransient<IFileValidator, FileSizeValidator>();      // 透過 IOptions 讀取 MaxUploadSize
builder.Services.AddTransient<IFileValidator, ExtensionValidator>();     // 透過 IOptions 讀取 AllowedExtensions
builder.Services.AddTransient<IFileValidator, FileNameValidator>();

// 儲存實作
builder.Services.AddTransient<IFileStorage, LocalFileStorage>();

// 報表工廠
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
