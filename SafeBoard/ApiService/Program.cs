using ApiService.Data;
using ApiService.Middlewares;
using ApiService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ���������� DI ��� ������ �������� FileService � ArchiveService
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<ArchiveService>();

// ���������� ���������� ������ ��� SqlLite ��� ������ ����� � ���� ������ SqlLite
builder.Services.AddDbContext<LogDataContext>(options => options.UseSqlite(configuration.GetConnectionString("SqlLite")));

// ���������� ��������� ����������� � ������
builder.Services.AddMemoryCache();

// ���������� �������, ��� ����������� ����� ������������ ��� ���������� ���-API
builder.Services.AddControllers();
// ���������� �������� ��� Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ���������� Swagger ��� Debug-����������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware ��� ����������� �������� ������������ �� �������
app.UseMiddleware<LogDataMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
