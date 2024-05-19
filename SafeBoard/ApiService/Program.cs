using ApiService.Data;
using ApiService.Middlewares;
using ApiService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Добавление DI для работы сервисов FileService и ArchiveService
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<ArchiveService>();

// Добавление провайдера данных для SqlLite для записи логов в базу данных SqlLite
builder.Services.AddDbContext<LogDataContext>(options => options.UseSqlite(configuration.GetConnectionString("SqlLite")));

// Добавление поддержки кэширования в памяти
builder.Services.AddMemoryCache();

// Добавление сервиса, для регистрации всего необходимого для разработки веб-API
builder.Services.AddControllers();
// Добавление сервисов для Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Добавление Swagger при Debug-разработке
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware для логирования запросов пользователя на серввис
app.UseMiddleware<LogDataMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
