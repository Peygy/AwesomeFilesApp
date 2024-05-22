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

var app = builder.Build();

// Middleware ��� ����������� �������� ������������ �� �������
app.UseMiddleware<LogDataMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
