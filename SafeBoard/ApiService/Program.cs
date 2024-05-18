using ApiService.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Добавление DI для работы сервисов FileService и ArchiveService
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<ArchiveService>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
