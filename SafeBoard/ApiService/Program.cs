using ApiService.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ���������� DI ��� ������ �������� FileService � ArchiveService
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<ArchiveService>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
