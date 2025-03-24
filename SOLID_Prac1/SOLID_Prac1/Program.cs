using SOLID_Prac1.Interface;
using SOLID_Prac1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ���U�����l����
builder.Services.AddTransient<PdfReport>();
builder.Services.AddTransient<DocReport>();
builder.Services.AddTransient<XlsxReport>();
builder.Services.AddTransient<IFileValidator, ExtensionValidator>();
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
